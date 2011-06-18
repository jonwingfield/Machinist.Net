using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Machinist.Net
{
    public class DefinitionLoader
    {
        public static BlueprintDefinition Load(Assembly assembly)
        {
            return (BlueprintDefinition)
                Activator.CreateInstance(
                    assembly.GetExportedTypes()
                        .First(item => item.IsSubclassOf(typeof(BlueprintDefinition))));

        }
    }
}
