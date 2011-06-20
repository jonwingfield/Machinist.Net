using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Machinist.Net.Extensions
{
    static class ObjectExtensions
    {
        internal static void ThrowIfNull(this object value)
        {
            if (value == null)
                throw new ArgumentNullException();
        }
    }
}
