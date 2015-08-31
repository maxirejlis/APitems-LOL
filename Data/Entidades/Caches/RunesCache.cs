using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Data.Entidades.Caches
{
    public static class RunesCache
    {
        private static readonly ReaderWriterLockSlim rwls = new ReaderWriterLockSlim();
        private static ConcurrentDictionary<int, Rune> cache;
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
                    cache = new ConcurrentDictionary<int, Rune>(data.GetRunes().ToDictionary(r => r.Id, r => r));
                }
                expiration = DateTime.Now.AddHours(2);
            }
            finally
            {
                rwls.ExitWriteLock();
            }
        }

        public static Rune Get(int id)
        {
            LoadCache();
            try
            {
                rwls.EnterReadLock();
                Rune rune;
                return cache.TryGetValue(id, out rune) ? rune : null;
            }
            finally
            {
                rwls.ExitReadLock();
            }
        }
    }
}
