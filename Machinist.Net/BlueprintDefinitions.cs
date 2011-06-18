using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machinist.Net.Drivers;
using System.Text.RegularExpressions;

namespace Machinist.Net
{
    public abstract class BlueprintDefinition
    {
        protected class BlueprintInfo
        {
            public BlueprintInfo(Func<object> callback)
            {
                Callback = callback;
            }

            public Func<object> Callback;
            public bool WithAssociations;
            public bool WithShams { get; set; }
        }

        protected class BlueprintContext
        {
            private readonly BlueprintInfo _info;
            public BlueprintContext(BlueprintInfo info)
            {
                _info = info;
            }

            public void WithAssociations()
            {
                _info.WithAssociations = true;
            }

            public void WithShams()
            {
                _info.WithShams = true;
            }
        }

        private int _serial_number = 1;
        private readonly Dictionary<Type, int> _idLookup = new Dictionary<Type, int>();
        private readonly ShamDefinition _shamDef = new ShamDefinition();

        private readonly Dictionary<Type, BlueprintInfo> _blueprints = new Dictionary<Type, BlueprintInfo>();
        private readonly Dictionary<Type, Dictionary<string, BlueprintInfo>> _namedBlueprints = new Dictionary<Type, Dictionary<string, BlueprintInfo>>();

        internal static BlueprintDefinition Inst;

        protected BlueprintContext Blueprint<T>(string name, Action<T> stubs = null)
            where T : new()
        {
            if (!_namedBlueprints.ContainsKey(typeof(T)))
                _namedBlueprints.Add(typeof(T), new Dictionary<string, BlueprintInfo>());

            var info = GetInfo(stubs);
            _namedBlueprints[typeof(T)].Add(name, info);
            return new BlueprintContext(info);
        }

        protected BlueprintContext Blueprint<T>(Action<T> stubs = null)
            where T : new()
        {
            var info = GetInfo<T>(stubs);

            _blueprints.Add(typeof(T), info);
            _idLookup.Add(typeof(T), 0);

            return new BlueprintContext(info);
        }

        private BlueprintInfo GetInfo<T>(Action<T> stubs) where T : new()
        {
            var info = new BlueprintInfo(() =>
            {
                var t = new T();
                if (stubs != null)
                    stubs(t);
                StubObject(t);
                return t;
            });
            return info;
        }

        protected ShamDefinition Sham
        {
            get { return _shamDef; }
        }

        protected int serial_number
        {
            get { return _serial_number++; }
        }

        protected int sn
        {
            get { return serial_number; }
        }

        protected List<T> listOf<T>(int count)
        {
            return Enumerable.Range(0, count)
                             .Select(x => _blueprints[typeof(T)].Callback())
                             .Cast<T>()
                             .ToList();
        }

        private void StubObject<T>(T obj)
        {
            foreach (var property in typeof(T).GetProperties())
            {
                object val = property.GetValue(obj, null);
                if (val == null)
                {
                    GetObjectOrListValue<T>(obj, property);
                }
                else if (property.PropertyType == typeof(string))
                {
                    ParseString<T>(obj, property, val);
                }
                else if (property.Name.ToLower().EndsWith("id"))
                {
                    GetIdValue<T>(obj, property, val);
                }
            }
        }

        private void GetIdValue<T>(T obj, System.Reflection.PropertyInfo property, object val)
        {
            if (property.PropertyType == typeof(long) && (long)val == 0)
                property.SetValue(obj, ++_idLookup[typeof(T)], null);
            else if (property.PropertyType == typeof(int) && (int)val == 0)
                property.SetValue(obj, ++_idLookup[typeof(T)], null);
            else if (property.PropertyType == typeof(Guid) && (Guid)val == Guid.Empty)
                property.SetValue(obj, Guid.NewGuid(), null);
        }

        private void ParseString<T>(T obj, System.Reflection.PropertyInfo property, object val)
        {
            string sVal = (string)val;
            var match = new Regex(@".*\#\{([^\}]*)\}.*").Match(sVal);
            if (match.Success)
            {
                for (int i = 1; i < match.Groups.Count; i++)
                {
                    foreach (var capture in match.Groups[i].Captures.Cast<Capture>())
                    {
                        sVal = sVal.Replace("#{" + capture.Value + "}", getValue(capture.Value, obj));
                    }
                }

                property.SetValue(obj, sVal, null);
            }
        }

        private void GetObjectOrListValue<T>(T obj, System.Reflection.PropertyInfo property)
        {
            if (_blueprints.ContainsKey(property.PropertyType) && _blueprints[typeof(T)].WithAssociations)
            {
                property.SetValue(obj, _blueprints[property.PropertyType].Callback(), null);
            }
            else if (property.PropertyType.GetInterface("IEnumerable`1") != null &&
                     _blueprints.ContainsKey(property.PropertyType.GetInterface("IEnumerable`1").GetGenericArguments()[0]) &&
                     _blueprints[typeof(T)].WithAssociations)
            {
                Type argType = property.PropertyType.GetInterface("IEnumerable`1").GetGenericArguments()[0];

                property.SetValue(obj,
                    Activator.CreateInstance(typeof(List<>).MakeGenericType(argType)),
                    null);
            }
            else if (_blueprints[typeof(T)].WithShams)
            {
                object val = _shamDef.GetMatch(property.Name);
                if (val != null && (property.PropertyType == val.GetType() ||
                                    property.PropertyType.IsSubclassOf(val.GetType())))
                    property.SetValue(obj, val, null);
            }
        }

        private string getValue(string propName, object obj)
        {
            if (propName.StartsWith("object."))
            {
                propName = propName.Substring(propName.IndexOf('.') + 1);

                string[] props = propName.Split('.');

                foreach (string prop in props)
                {
                    obj = obj.GetType()
                             .GetProperty(prop)
                             .GetValue(obj, null);
                }

                return obj.ToString();
            }
            else
            {
                return typeof(BlueprintDefinition).GetProperty(propName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.GetProperty)
                                                  .GetValue(this, null).ToString();
            }
        }

        internal T Get<T>()
        {
            return (T)_blueprints[typeof(T)].Callback();
        }

        internal T Get<T>(string namedBlueprint)
        {
            return (T) _namedBlueprints[typeof (T)][namedBlueprint].Callback();
        }
    }
}