using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Machinist.Net.Drivers
{
    public class NullActiveRecordDriver : IActiveRecordDriver
    {
        #region IActiveRecordDriver Members

        public void Save<T>(T objToSave) where T : class
        {
            
        }

        #endregion
    }
}
