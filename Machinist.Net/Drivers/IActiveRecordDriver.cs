using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Machinist.Net.Drivers
{
    public interface IActiveRecordDriver
    {
        void Save<T>(T objToSave) where T : class;
    }
}
