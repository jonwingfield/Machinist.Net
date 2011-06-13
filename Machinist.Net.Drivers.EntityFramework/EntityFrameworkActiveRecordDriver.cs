using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;

namespace Machinist.Net.Drivers.EntityFramework
{
    public class EntityFrameworkActiveRecordDriver : IActiveRecordDriver
    {
        private DbContext _context;
        public EntityFrameworkActiveRecordDriver(DbContext context)
        {
            if (context == null) throw new ArgumentNullException("context");
            _context = context;
        }

        #region IActiveRecordDriver Members

        public void Save<T>(T objToSave) where T : class
        {
            _context.Set<T>().Add(objToSave);
            _context.SaveChanges();
        }

        #endregion
    }
}
