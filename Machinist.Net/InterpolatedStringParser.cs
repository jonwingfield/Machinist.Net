using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Machinist.Net
{
    public class InterpolatedStringParser
    {
        private string _stringToParse;
        private object _instance;
        public InterpolatedStringParser(string stringToParse, object instance)
        {
            _stringToParse = stringToParse;
            _instance = instance;
        }

        public string Parse(object obj)
        {
            string sVal = _stringToParse;
            var match = new Regex(@".*\#\{([^\}]*)\}.*").Match(_stringToParse);
            if (match.Success)
            {
                for (int i = 1; i < match.Groups.Count; i++)
                {
                    foreach (var capture in match.Groups[i].Captures.Cast<Capture>())
                    {
                        sVal = sVal.Replace("#{" + capture.Value + "}", getValue(capture.Value, obj));
                    }
                }
            }
            return sVal;
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
                return _instance.GetType().GetProperty(propName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.GetProperty)
                                          .GetValue(_instance, null).ToString();
            }
        }
    }
}
