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
        internal class BlueprintInfo
        {
            internal BlueprintInfo(Func<object> callback)
            {
                Callback = callback;
            }

            public Func<object> Callback;
            public bool WithAssociations;
            public bool WithShams { get; set; }
        }

        protected class BlueprintContext<T> where T : new()
        {
            private readonly IObjectBlueprint<T> _objectBlueprint;
            internal BlueprintContext(IObjectBlueprint<T> objectBlueprint)
            {
                _objectBlueprint = objectBlueprint;
            }

            public void WithAssociations()
            {
                _objectBlueprint.WithAssociations = true;
            }

            public void WithShams()
            {
                _objectBlueprint.WithShams = true;
            }
        }

        private int _serial_number = 1;
        private readonly Dictionary<Type, int> _idLookup = new Dictionary<Type, int>();
        private readonly ShamDefinition _shamDef = new ShamDefinition();

        private readonly ObjectBlueprintCollection _blueprints;
        //private readonly Dictionary<Type, ObjectBlueprint> _blueprints = new Dictionary<Type, ObjectBlueprint>();
        //private readonly Dictionary<Type, BlueprintInfo> _blueprints = new Dictionary<Type, BlueprintInfo>();
        //private readonly Dictionary<Type, Dictionary<string, BlueprintInfo>> _namedBlueprints = new Dictionary<Type, Dictionary<string, BlueprintInfo>>();

        internal static BlueprintDefinition Inst;
        private Random _random = new Random(1);

        protected BlueprintDefinition()
        {
            _blueprints = new ObjectBlueprintCollection(_shamDef);
        }

        protected BlueprintContext<T> Blueprint<T>(Action<T> stubs = null)
            where T : class, new()
        {
            return new BlueprintContext<T>(_blueprints.Add(stubs));
        }

        protected BlueprintContext<T> Blueprint<T>(string name, Action<T> stubs = null)
            where T : class, new()
        {
            return new BlueprintContext<T>(_blueprints.Add(stubs, name));
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

        protected Random Rand
        {
            get { return _random; }
        }

        protected List<T> ListOf<T>(int count)
        {
            return Enumerable.Range(0, count)
                             .Select(x => _blueprints.Get(typeof(T)).Create())
                             .Cast<T>()
                             .ToList();
        }


        internal T Get<T>(string namedBlueprint = null)
        {
            return (T)_blueprints.Get(typeof (T), namedBlueprint).Create();
        }

        protected T Make<T>() where T : class
        {
            return Get<T>();
        }

        protected T Make<T>(string blueprintName) where T : class
        {
            return Get<T>(blueprintName);
        }
    }
}