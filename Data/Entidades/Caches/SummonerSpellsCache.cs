using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Data.Entidades.Caches
{
    public static class SummonerSpellsCache
    {
        private static readonly ReaderWriterLockSlim rwls = new ReaderWriterLockSlim();
        private static ConcurrentDictionary<int,SummonerSpell> cache;
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
                //cache = new ConcurrentDictionary<int, SummonerSpell>(Procedures.getSummonerSpellsData(true).ToDictionary(s=>s.Id, s=>s));
                cache = new ConcurrentDictionary<int, SummonerSpell>();
                expiration = DateTime.Now.AddHours(2);
            }
            finally 
            {
                rwls.ExitWriteLock();
            }            
        }

        public static SummonerSpell Get(int id) 
        {
            LoadCache();
            try
            {
                rwls.EnterReadLock();
                SummonerSpell spell;
                return cache.TryGetValue(id, out spell) ? spell : null;
            }
            finally
            {
                rwls.ExitReadLock();
            }
        }
    }
}
