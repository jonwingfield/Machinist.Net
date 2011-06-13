using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machinist.Net.Drivers;

namespace Machinist.Net
{
    public abstract class BlueprintBase
    {
        private int _serial_number = 1;

        public BlueprintBase() { Init(); Driver = new NullActiveRecordDriver(); }

        private Dictionary<Type, Func<object>> _blueprints = new Dictionary<Type, Func<object>>();

        public IActiveRecordDriver Driver { get; set; }

        protected abstract void Init();

        protected void Blueprint<T>()
            where T : new()
        {
            _blueprints.Add(typeof(T), () =>
            {
                var t = new T();
                StubObject(t);
                return t;
            });
        }

        protected void Blueprint<T>(Action<T> stubs)
            where T : new()
        {
            _blueprints.Add(typeof(T), () => 
                {
                    var t = new T();
                    StubObject(t);
                    stubs(t);
                    return t;
                });
        }

        protected int serial_number
        {
            get { return _serial_number++; }
        }

        public T For<T>() where T : class
        {
            T obj = (T)_blueprints[typeof(T)]();
            Driver.Save(obj);
            return obj;
        }

        protected List<T> listOf<T>(int count)
        {
            return Enumerable.Range(0, count)
                             .Select(x => _blueprints[typeof(T)]())
                             .Cast<T>()
                             .ToList();
        }

        private void StubObject<T>(T obj)
        {
            foreach (var property in typeof(T).GetProperties())
            {
                if (property.GetValue(obj, null) == null)
                {
                    if (_blueprints.ContainsKey(property.PropertyType))
                    {
                        property.SetValue(obj, _blueprints[property.PropertyType](), null);
                    }
                    else if (property.PropertyType.GetInterface("IEnumerable`1") != null &&
                             _blueprints.ContainsKey(property.PropertyType.GetInterface("IEnumerable`1").GetGenericArguments()[0]))
                    {
                        Type argType = property.PropertyType.GetInterface("IEnumerable`1").GetGenericArguments()[0];

                        property.SetValue(obj, 
                            Activator.CreateInstance(typeof(List<>).MakeGenericType(argType)),
                            null);
                    }
                }
            }
        }
    }
}
