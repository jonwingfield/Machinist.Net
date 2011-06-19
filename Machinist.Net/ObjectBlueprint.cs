using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Machinist.Net
{
    class ObjectBlueprint<T> : IObjectBlueprint<T> where T : new()
    {
        private string _name;
        private Func<object> _objectCreator;
        private readonly ShamDefinition _shamDef;
        private readonly ObjectBlueprintCollection _collection;

        internal ObjectBlueprint(ObjectBlueprintCollection collection,
            ShamDefinition shamDef,
            Action<T> objectBuilder = null,
            string name = null)
        {
            if (collection == null) throw new ArgumentNullException("collection");
            if (shamDef == null) throw new ArgumentNullException("shamDef");

            _shamDef = shamDef;
            _collection = collection;
            _objectCreator = GetInfo(objectBuilder);
            _name = name;
        }

        public object Create()
        {
            return _objectCreator();
        }

        private Func<object> GetInfo(Action<T> stubs)
        {
            return () =>
            {
                var t = new T();
                if (stubs != null)
                    stubs(t);
                StubObject(t);
                return t;
            };
        }

        public bool WithAssociations { get; set; }

        public bool WithShams { get; set; }

        private void StubObject(T obj)
        {
            foreach (var property in typeof(T).GetProperties())
            {
                object val = property.GetValue(obj, null);
                if (val == null)
                {
                    GetObjectOrListValue(obj, property);
                }
                else if (property.PropertyType == typeof(string))
                {
                    property.SetValue(obj,
                                      new InterpolatedStringParser((string)val, BlueprintDefinition.Inst)
                                          .Parse(obj),
                                      null);
                }
                else if (property.Name.ToLower().EndsWith("id"))
                {
                    GetIdValue(obj, property, val);
                }
            }
        }

        private void GetIdValue(T obj, System.Reflection.PropertyInfo property, object val)
        {
            if (property.PropertyType == typeof(long) && (long)val == 0)
                property.SetValue(obj, _collection.NextId(typeof(T)), null);
            else if (property.PropertyType == typeof(int) && (int)val == 0)
                property.SetValue(obj, _collection.NextId(typeof(T)), null);
            else if (property.PropertyType == typeof(Guid) && (Guid)val == Guid.Empty)
                property.SetValue(obj, Guid.NewGuid(), null);
        }

        private void GetObjectOrListValue(T obj, System.Reflection.PropertyInfo property)
        {
            IObjectBlueprint<object> objectBlueprint = _collection.Get(property.PropertyType);
            if (objectBlueprint != null && WithAssociations)
            {
                property.SetValue(obj, objectBlueprint.Create(), null);
            }
            else if (property.PropertyType.GetInterface("IEnumerable`1") != null &&
                     _collection.Get(property.PropertyType.GetInterface("IEnumerable`1").GetGenericArguments()[0]) != null &&
                     WithAssociations)
            {
                Type argType = property.PropertyType.GetInterface("IEnumerable`1").GetGenericArguments()[0];

                property.SetValue(obj,
                    Activator.CreateInstance(typeof(List<>).MakeGenericType(argType)),
                    null);
            }
            else if (WithShams)
            {
                object val = _shamDef.GetMatch(property.Name);
                if (val != null && (property.PropertyType == val.GetType() ||
                                    property.PropertyType.IsSubclassOf(val.GetType())))
                    property.SetValue(obj, val, null);
            }
        }
    }
}
