using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Data.Entidades.Caches
{
    public static class ItemsCache
    {
        private static readonly ReaderWriterLockSlim rwls = new ReaderWriterLockSlim();
        private static ConcurrentDictionary<int,Item> cacheV511;
        private static ConcurrentDictionary<int, Item> cacheV514;
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
                IList<Item> items;
                using (var data = new SqlData())
                {
                    items = data.GetItems();
                }
                cacheV511 = new ConcurrentDictionary<int, Item>(items.Where(i => i.Patch == "5.11.1").ToDictionary(c => c.Id, c => c));
                cacheV514 = new ConcurrentDictionary<int, Item>(items.Where(i => i.Patch == "5.14.1").ToDictionary(c => c.Id, c => c));
                
                expiration = DateTime.Now.AddHours(2);
            }
            finally 
            {
                rwls.ExitWriteLock();
            }            
        }

        public static Item GetItemV511(int id) 
        {
            LoadCache();
            try
            {
                rwls.EnterReadLock();
                Item item;
                return cacheV511.TryGetValue(id, out item) ? item : null;
            }
            finally
            {
                rwls.ExitReadLock();
            }
        }

        public static Item GetItemV514(int id)
        {
            LoadCache();
            try
            {
                rwls.EnterReadLock();
                Item item;
                return cacheV514.TryGetValue(id, out item) ? item : null;
            }
            finally
            {
                rwls.ExitReadLock();
            }
        }

        public static List<Item> RemoveTrinkets(IList<Item> items) 
        {
            var newItems = new List<Item>();
            foreach (var item in items) 
            {
                if (item.Name.Contains("Trinket"))
                    continue;
                newItems.Add(item);
            }
            return newItems;
        }
    }
}
