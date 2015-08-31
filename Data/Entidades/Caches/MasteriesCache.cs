using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Data.Entidades.Caches
{
    public static class MasteriesCache
    {
        private static readonly ReaderWriterLockSlim rwls = new ReaderWriterLockSlim();
        private static ConcurrentDictionary<int, Mastery> cache;
        private static DateTime expiration = DateTime.MinValue;

        private static void LoadCache()
        {
            if (DateTime.Now < expiration)
                return;
            try
            {
                rwls.EnterWriteLock();
                if (DateTime.Now < expiration)
                    return;
                //cache = new ConcurrentDictionary<int, Mastery>(Procedures.getMasteriesData(true).ToDictionary(m=>m.Id, m=>m));
                cache = new ConcurrentDictionary<int, Mastery>();
                expiration = DateTime.Now.AddHours(2);
            }
            finally 
            {
                rwls.ExitWriteLock();
            }            
        }

        public static Mastery Get(int id) 
        {
            LoadCache();
            try
            {
                rwls.EnterReadLock();
                Mastery mastery;
                return cache.TryGetValue(id, out mastery) ? mastery : null;
            }
            finally
            {
                rwls.ExitReadLock();
            }
        }
    }
}
