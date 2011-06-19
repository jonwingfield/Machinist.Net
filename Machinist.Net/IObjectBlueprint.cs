using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Machinist.Net
{
    interface IObjectBlueprint<out T> where T : new()
    {
        bool WithAssociations { get; set; }
        bool WithShams { get; set; }
        object Create();
    }
}
