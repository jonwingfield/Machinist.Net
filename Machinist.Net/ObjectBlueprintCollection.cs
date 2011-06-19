using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Machinist.Net
{
    class ObjectBlueprintCollection
    {
        private readonly Dictionary<Type, Dictionary<string, IObjectBlueprint<object>>>  _blueprints =
            new Dictionary<Type, Dictionary<string, IObjectBlueprint<object>>>();

        private readonly Dictionary<Type, int> _idLookup = new Dictionary<Type, int>();
        private ShamDefinition _shamDef;

        internal ObjectBlueprintCollection(ShamDefinition shamDef)
        {
            if (shamDef == null) throw new ArgumentNullException("shamDef");
            _shamDef = shamDef;
        }

        internal IObjectBlueprint<T> Add<T>(Action<T> stubs = null, string name = null) where T : class, new()
        {
            if (!_blueprints.ContainsKey(typeof(T)))
                _blueprints.Add(typeof (T), new Dictionary<string, IObjectBlueprint<object>>());

            var objectBlueprint = new ObjectBlueprint<T>(this, _shamDef, stubs, name);
            _blueprints[typeof (T)].Add(name ?? "", objectBlueprint);

            if (!_idLookup.ContainsKey(typeof(T)))
                _idLookup.Add(typeof (T), 0);

            return objectBlueprint;
        }

        internal IObjectBlueprint<object> Get(Type type, string name = "")
        {
            if (!_blueprints.ContainsKey(type))
                return null;
            return _blueprints[type][name ?? ""];
        }

        internal int NextId(Type type)
        {
            if (_idLookup.ContainsKey(type))
                return ++_idLookup[type];
                
            throw new InvalidOperationException();
        }
    }
}
