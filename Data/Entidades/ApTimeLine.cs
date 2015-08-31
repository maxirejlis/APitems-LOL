using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Data.Entidades.Caches;
using MySql.Data.MySqlClient;

namespace Data.Entidades
{
    public class ApTimeLine
    {
        
        public int MatchId { get; set; }
        public int PlayerId { get; set; }
        public int TeamId { get; set; }
        public double TotalAp { get; set; }
        public double TotalMr { get; set; }
        public List<Item> Items { get; set; }
        public int Minute { get; set; }
        public int ChampionId { get; set; }
        public string Rank {get;set;}
        public string Role {get;set;}
        public string Lane { get; set; }
        public string Patch { get; set; }
        public int CurrentGold { get; set; }
        public int GoldSpent { get; set; }
        public bool Winner { get; set; }


        public static IList<ApTimeLine> GetPlayerApTimeline(Player p, string patch, int matchId, int teamId) 
        {
            try
            {
                IList<ApTimeLine> timelines = new List<ApTimeLine>();
                var orderedTime = p.ItemTimeLine.OrderBy(a => a.TimeStamp).ToList();
                IDictionary<int, double> apPerMin = new Dictionary<int, double>();
                var items = new List<Item>();
                var groups = orderedTime.GroupBy(a => (int)Math.Truncate((double)a.TimeStamp / 60000));

                var runesAp = p.Runes.Sum(r => r.Ap);
                var runesApPerLv = p.Runes.Sum(r => r.ApPerLevel);
                var runesMr = p.Runes.Sum(r => r.MagicResist);
                var runesMrPerLv = p.Runes.Sum(r => r.MagicResistPerLevel);
                foreach (var group in groups)
                {
                    try
                    {
                        var t = new ApTimeLine();
                        t.Minute = group.Key;
                        var lv = p.TimeLine[t.Minute].Level;
                        t.PlayerId = p.ParticipantId;
                        t.MatchId = matchId;
                        t.TeamId = teamId;

                        foreach (var itemTimeline in group)
                        {
                            Item item;
                            if (patch.StartsWith("5.11"))
                                item = ItemsCache.GetItemV511(itemTimeline.ItemId);
                            else
                                item = ItemsCache.GetItemV514(itemTimeline.ItemId);

                            double itemAp = 0;
                            if (itemTimeline.EventType == "ITEM_PURCHASED")
                            {
                                items.Add(item);
                            }
                            else
                            {
                                items.Remove(item);
                            }

                            double totalAp = items.Sum(i => ItemStats.GetTotalAP(i,t.Minute,p.SelectedChampion)) + runesAp + runesApPerLv * lv;
                            if (items.Any(i => i.Name == "Rabadon's Deathcap"))
                            {
                                if (patch.StartsWith("5.11"))
                                    totalAp = totalAp * 1.30;
                                else
                                    totalAp = totalAp * 1.35;
                            }

                            if (apPerMin.ContainsKey(t.Minute))
                            {
                                apPerMin[t.Minute] = totalAp;
                            }
                            else
                            {
                                apPerMin.Add(new KeyValuePair<int, double>(t.Minute, totalAp));
                            }

                        }
                        t.Items = items.ToList();

                        t.CurrentGold = p.TimeLine[t.Minute].CurrentGold;
                        t.GoldSpent = items.Sum(i => i.GoldCost);

                        t.TotalMr = items.Sum(i => i.ItemStats.MR) + p.SelectedChampion.Stat.MagicResist + p.SelectedChampion.Stat.MagicResistPerLv * lv + runesMr + runesMrPerLv * lv; ;
                        t.TotalAp = apPerMin.LastOrDefault().Value;
                        timelines.Add(t);
                    }
                    catch {
                        throw;
                    }
                }
                return timelines;
            }
            catch {
                throw;
            }
        }

        public ApTimeLine()
        {
          
        }

        public ApTimeLine(ApTimeLine a)
        {
            MatchId = a.MatchId;
            PlayerId = a.PlayerId;
            TeamId = a.TeamId;
            TotalAp = a.TotalAp;
            TotalMr = a.TotalMr;
            Items = a.Items;
            Minute = a.Minute;
            ChampionId = a.ChampionId;
            Rank = a.Rank;
            Role = a.Role;
            Lane = a.Lane;
            Patch = a.Patch;
            CurrentGold = a.CurrentGold;
            GoldSpent = a.GoldSpent;
            Winner = a.Winner;
        }

        public static ApTimeLine Mapper(MySqlDataReader reader)
        {
            var a = new ApTimeLine();
            a.MatchId = reader.GetInt32("MatchId");
            a.PlayerId = reader.GetInt32("ParticipantId");
            a.Rank = reader.GetString("Rank");
            a.ChampionId = reader.GetInt32("ChampionId");
            a.Role = reader.GetString("Role");
            a.Lane = reader.GetString("Lane");
            a.TeamId = reader.GetInt32("TeamId");
            a.Minute = reader.GetInt32("Minute");
            a.TotalAp = reader.GetInt32("TotalAP");
            a.TotalMr = reader.GetInt32("TotalMR");
            var itemsIds = reader.GetString("ItemsIds");
            a.Patch = reader.GetString("Patch");
            a.Items = new List<Item>();
            foreach (var itemId in itemsIds.Split(new []{'|'}, StringSplitOptions.RemoveEmptyEntries))
            {
                if (a.Patch.Contains("5.11"))
                    a.Items.Add(ItemsCache.GetItemV511(int.Parse(itemId)));
                else
                    a.Items.Add(ItemsCache.GetItemV514(int.Parse(itemId)));
            }
            
            return a;
        }

    }
}
