using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Data.Entidades.Caches
{
    public static class ChampionsCache
    {
        private static readonly ReaderWriterLockSlim rwls = new ReaderWriterLockSlim();
        private static ConcurrentDictionary<int,Champion> cache;
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
                using (var data = new SqlData())
                {
                    cache = new ConcurrentDictionary<int, Champion>(data.GetChampions().ToDictionary(c => c.Id, c => c));
                }
                expiration = DateTime.Now.AddHours(2);
            }
            finally 
            {
                rwls.ExitWriteLock();
            }            
        }

        public static Champion Get(int id) 
        {
            LoadCache();
            try
            {
                rwls.EnterReadLock();
                Champion champion;
                return cache.TryGetValue(id, out champion) ? champion : null;
            }
            finally
            {
                rwls.ExitReadLock();
            }
        }

        public static IList<Champion> Get()
        {
            LoadCache();
            try
            {
                rwls.EnterReadLock();
                return cache.Values.ToList();
            }
            finally
            {
                rwls.ExitReadLock();
            }
        }
    }
}
