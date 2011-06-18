using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;
using System.Text.RegularExpressions;
using Faker;

namespace Machinist.Net
{
    public class ShamDefinition : DynamicObject
    {
        private readonly Dictionary<Regex, Func<object>> _defs = new Dictionary<Regex, Func<object>>();
        private readonly Dictionary<string, Func<object>> _gens = new Dictionary<string, Func<object>>();
        private Random _random;

        public ShamDefinition()
        {
            FakerRandom.Seed(1);
            _random = new Random(1);
        }

        public void Match(string name, Func<object> func)
        {
            _defs.Add(new Regex(name), func);
        }

        public void Match<T>(string name, Func<int, T> func)
        {
            throw new NotImplementedException();
        }

        public void For(dynamic fieldValue)
        {
            
        }

        internal object GetMatch(string propertyToMatch)
        {
            var match = _defs.FirstOrDefault(item => item.Key.Match(propertyToMatch).Success);
            return match.Key != null ? match.Value() : null;
        }

        public void Generate(string propertyName, Func<object> propertyValueGetter)
        {
            _gens.Add(propertyName, propertyValueGetter);
        }

        public dynamic Get
        {
            get { return this; }
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (!_gens.ContainsKey(binder.Name))
            {
                result = null;
                return false;
            }

            result = _gens[binder.Name]();
            return true;
        }
    }
}
