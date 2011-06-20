using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;
using System.Text.RegularExpressions;
using Faker;
using Machinist.Net.Extensions;

namespace Machinist.Net
{
    public class ShamDefinition : DynamicObject
    {
        private class Generator
        {
            private Func<object> _generator;
            private bool _unique;
            private List<object> _generatedValues;
            private int _offset = 0;
            
            public Generator(Func<object> generator, bool unique = true)
            {
                generator.ThrowIfNull();
                _generator = generator;
                _unique = unique;
                if (_unique)
                {
                    _generatedValues = new List<object>();
                    GenerateValues();
                }
            }

            public object Generate()
            {
                if (!_unique)
                    return _generator();
                else
                {
                    return GetUniqueValue();
                }
            }

            private object GetUniqueValue()
            {
                if (_offset >= _generatedValues.Count)
                {
                    GenerateValues();
                    if (_offset >= _generatedValues.Count)
                        throw new ApplicationException("Could not generate more unique values");
                }

                return _generatedValues[_offset++];
            }

            private void GenerateValues()
            {
                _generatedValues = new List<object>(
                    Enumerable.Range(0, 10)
                        .Select(x => _generator())
                        .Union(_generatedValues));
            }        
        }

        private readonly Dictionary<Regex, Func<object>> _defs = new Dictionary<Regex, Func<object>>();
        private readonly Dictionary<string, Generator> _gens = new Dictionary<string, Generator>();
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

        internal object GetMatch(string propertyToMatch)
        {
            var match = _defs.FirstOrDefault(item => item.Key.Match(propertyToMatch).Success);
            return match.Key != null ? match.Value() : null;
        }

        public void Generate(string propertyName, Func<object> propertyValueGetter, bool unique = true)
        {
            _gens.Add(propertyName, new Generator(propertyValueGetter, unique));
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

            result = _gens[binder.Name].Generate();

            return true;
        }
    }
}
