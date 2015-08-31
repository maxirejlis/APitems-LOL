using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Data.Entidades.Caches
{
    public class AvgTimelinesCache
    {
        private static readonly ReaderWriterLockSlim rwls = new ReaderWriterLockSlim();
        private static ConcurrentBag<AvgTimeLine> cache;
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
                    cache = new ConcurrentBag<AvgTimeLine>(data.GetAvgTimelines());
                }
                expiration = DateTime.Now.AddHours(2);
            }
            finally
            {
                rwls.ExitWriteLock();
            }
        }

        public static IList<AvgTimeLine> Get()
        {
            LoadCache();
            try
            {
                rwls.EnterReadLock();
                return cache.ToList();
            }
            finally
            {
                rwls.ExitReadLock();
            }
        }
    }
}
