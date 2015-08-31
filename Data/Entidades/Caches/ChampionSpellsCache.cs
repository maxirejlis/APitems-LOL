using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Data.Entidades.Caches
{
    public static class ChampionSpellsCache
    {
        private static readonly ReaderWriterLockSlim rwls = new ReaderWriterLockSlim();
        private static ConcurrentDictionary<int, ChampionSpell> cache;
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
                //cache = new ConcurrentBag<ChampionSpell>(Procedimientos.getChampionsSpellsData(true));
                cache = new ConcurrentDictionary<int, ChampionSpell>();
                expiration = DateTime.Now.AddHours(2);
            }
            finally 
            {
                rwls.ExitWriteLock();
            }            
        }

        public static ChampionSpell Get(int id) 
        {
            LoadCache();
            try
            {
                rwls.EnterReadLock();
                ChampionSpell spell;
                return cache.TryGetValue(id, out spell) ? spell : null;
            }
            finally
            {
                rwls.ExitReadLock();
            }
        }
    }
}
