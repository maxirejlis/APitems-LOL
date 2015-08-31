using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Data.Entidades.Caches;
using MySql.Data.MySqlClient;

namespace Data.Entidades
{
    public class AvgTimelineDto 
    {
        public int ChampionId { get; set; }
        public string Rank { get; set; }
        public string Role { get; set; }

        public static AvgTimelineDto Mapper(MySqlDataReader reader)
        {
            var a = new AvgTimelineDto();
            a.ChampionId = reader.GetInt32("IdChamp");
            a.Rank = reader.GetString("Rank");
            a.Role = reader.GetString("Role");
            return a;
        }
    }



    public class AvgTimeLine
    {
        public int ChampionId { get; set; }
        public double AvgAp { get; set; }
        public double AvgMr { get; set; }
        public string Items { get; set; }
        public int Minute { get; set; }
        public string Rank {get;set;}
        public string Role {get;set;}
        public string Patch { get; set; }
        public int AvgGoldSpent { get; set; }
        public int TotalGames { get; set; }

        public static AvgTimeLine Mapper(MySqlDataReader reader)
        {
            var a = new AvgTimeLine();
            a.ChampionId = reader.GetInt32("IdChamp");
            a.Rank = reader.GetString("Rank");
            a.Role = reader.GetString("Role");
            a.AvgAp = reader.GetInt32("AvgAp");
            a.AvgMr = reader.GetInt32("AvgMr");
            a.AvgGoldSpent = reader.GetInt32("AvgGoldSpent");
            a.Items = reader.GetString("MostFrequentItems");
            a.Minute = reader.GetInt32("Minute");
            a.TotalGames = reader.GetInt32("TotalGames");          
            a.Patch = reader.GetString("Patch");
            //a.Items = new List<Item>();
            //foreach (var itemId in itemsIds.Split(new []{'|'}, StringSplitOptions.RemoveEmptyEntries))
            //{
            //    if (a.Patch.Contains("5.11"))
            //        a.Items.Add(ItemsCache.GetItemV511(int.Parse(itemId)));
            //    else
            //        a.Items.Add(ItemsCache.GetItemV514(int.Parse(itemId)));
            //}
            
            return a;
        }

    }
}
