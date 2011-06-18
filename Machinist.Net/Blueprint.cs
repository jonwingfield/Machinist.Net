using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machinist.Net.Drivers;
using System.Text.RegularExpressions;
using System.Reflection;

namespace Machinist.Net
{
    public class Blueprint
    {
        public Blueprint()
        {
            Driver = new NullActiveRecordDriver();
            BlueprintDefinition.Inst = DefinitionLoader.Load(Assembly.GetCallingAssembly());
        }

        public Blueprint(BlueprintDefinition def)
        {
            Driver = new NullActiveRecordDriver();
            BlueprintDefinition.Inst = def;
        }

        public IActiveRecordDriver Driver { get; set; }

        public T Make<T>(Action<T> overrides = null) where T : class
        {
            T obj = BlueprintDefinition.Inst.Get<T>();
            if (overrides != null)
                overrides(obj);

            Driver.Save(obj);
            return obj;
        }

        public T Make<T>(string blueprintName, Action<T> overrides = null) where T : class
        {
            T obj = BlueprintDefinition.Inst.Get<T>(blueprintName);
            if (overrides != null)
                overrides(obj);
            
            Driver.Save(obj);
            return obj;
        }
    }
}