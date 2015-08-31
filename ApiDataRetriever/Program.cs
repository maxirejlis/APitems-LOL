using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Entidades;
using Newtonsoft.Json;

namespace ApiDataRetriever
{
    class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        static void Main()
        {
            //IList<ApTimeLine> timelines;
            //IList<Match> matches;
            //IList<RunePage> runes;
            //using (var data = new Data.SqlData())
            //{
            //    timelines = data.GetTimelinesBadItems();
            //}
            //using (var data = new Data.SqlData())
            //{
            //    matches = data.GetMatches();
            //}
            //using (var data = new Data.SqlData())
            //{
            //    runes = data.GetRunePages();
            //}


            //foreach (var timeline in timelines)
            //{
            //    var match = matches.FirstOrDefault(m => m.Id == timeline.MatchId);
            //    var runepage = RunePage.GetRunePage(runes, timeline.MatchId, timeline.PlayerId);
            //    double totalAp = timeline.Items.Sum(i => ItemStats.GetTotalAP(i,timeline.Minute, Data.Entidades.Caches.ChampionsCache.Get(timeline.ChampionId))) + runepage.Runes.Sum(r => r.Ap) + runepage.Runes.Sum(r => r.ApPerLevel * 9);
            //    if (timeline.Items.Any(i => i.Name == "Rabadon's Deathcap"))
            //    {
            //        if (timeline.Patch.StartsWith("5.11"))
            //            totalAp = totalAp * 1.30;
            //        else
            //            totalAp = totalAp * 1.35;
            //    }
            //    timeline.TotalAp = totalAp;
            //    using (var data = new Data.SqlData())
            //    {
            //        data.EditTimeline(timeline);
            //    }
            //}

            ProcessAverages();
        }

        private static void ProcessAverages()
        {
            var champs = Data.Entidades.Caches.ChampionsCache.Get();
            foreach (var champ in champs)
            {
                IList<ApTimeLine> timelines;
                using (var data = new Data.SqlData())
                {
                    timelines = data.GetTimelines(champ.Id);
                }

                var cleanTimelines = new List<ApTimeLine>();
                var matchgroups = timelines.GroupBy(t => t.MatchId);
                int j = 0;
                foreach (var match in matchgroups)
                {
                    var participantgroups = match.OrderBy(m => m.Minute).GroupBy(m => m.PlayerId);
                    foreach (var player in participantgroups)
                    {
                        for (int i = 0; i < player.Count() - 1; i++)
                        {
                            
                            cleanTimelines.Add(player.ElementAt(i));

                            while (player.ElementAt(i + 1).Minute > cleanTimelines.ElementAt(j).Minute + 1)
                            {
                                var newmin = new ApTimeLine(cleanTimelines.ElementAt(j));
                                newmin.Minute++;
                                cleanTimelines.Add(newmin);
                                j++;
                            }
                            j++;
                        }
                    }
                }

                var patchgroups = cleanTimelines.GroupBy(t => t.Patch);
                foreach (var patch in patchgroups)
                {
                    var rankgroups = patch.GroupBy(p => p.Rank);
                    foreach (var rank in rankgroups)
                    {
                        var rolegroups = rank.GroupBy(p => Data.Entidades.EnumHelpers.GetRealRoles(p.Role, p.Lane));
                        foreach (var role in rolegroups)
                        {
                            var minutegroups = role.OrderBy(m => m.Minute).GroupBy(t => t.Minute);
                            foreach (var minute in minutegroups)
                            {
                                var items = new List<Item>();
                                foreach (var match in minute)
                                {
                                    match.Items = Data.Entidades.Caches.ItemsCache.RemoveTrinkets(match.Items);
                                    foreach (var item in match.Items)
                                    {
                                        items.Add(item);
                                    }
                                }

                                var q = from x in items
                                        group x by x into g
                                        let count = g.Count()
                                        orderby count descending
                                        select new { Value = g.Key, Count = count };

                                var e = q.OrderByDescending(i => i.Count).Take(10);

                                var avgt = new AvgTimeLine();
                                avgt.Minute = minute.Key;
                                avgt.ChampionId = champ.Id;
                                avgt.AvgAp = minute.Average(m => m.TotalAp);
                                avgt.AvgMr = minute.Average(m => m.TotalMr);
                                avgt.Patch = patch.Key;
                                avgt.AvgGoldSpent = (int)minute.Average(m => m.GoldSpent);
                                avgt.Rank = rank.Key;
                                avgt.Role = role.Key.ToString();
                                avgt.TotalGames = minute.Count();
                                avgt.Items = string.Join("|", e.Select(i => string.Format("{0}:{1}", i.Value.Id, i.Count)));
                                using (var data = new Data.SqlData())
                                {
                                    data.SaveAvgTimeLine(avgt);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void retrieveMatchData() {
            string NAv511 = @"M:\Users\Maxi\Documents\Proyectos\Riot Games Contest\5.11\RANKED_SOLO\NA.json";
            string NAv514 = @"M:\Users\Maxi\Documents\Proyectos\Riot Games Contest\5.14\RANKED_SOLO\NA.json";
            int i = 0;
            IList<Match> matches = new List<Match>();
            using (StreamReader reader = new StreamReader(NAv511))
            {
                var textResponse = reader.ReadToEnd();
                dynamic dynObject = JsonConvert.DeserializeObject(textResponse);
                foreach (var match in dynObject)
                {
                    Match m = Procedures.GetMatch(match.Value.ToString());
                    if (m != null)
                    {
                        using (var data = new Data.SqlData())
                        {
                            data.SaveMatch(m);
                            data.SaveApTimeline(m, "5.11");
                            data.SaveFinalStats(m, "5.11");
                            data.SaveRunePage(m);
                        }
                    }
                    i++;
                    Console.WriteLine("Patch 5.11: " + i);
                }
            }
            i = 0;
            using (StreamReader reader = new StreamReader(NAv514))
            {
                var textResponse = reader.ReadToEnd();
                dynamic dynObject = JsonConvert.DeserializeObject(textResponse);
                foreach (var match in dynObject)
                {
                    Match m = Procedures.GetMatch(match.Value.ToString());
                    if (m != null)
                    {
                        using (var data = new Data.SqlData())
                        {
                            data.SaveMatch(m);
                            data.SaveApTimeline(m, "5.14");
                            data.SaveFinalStats(m, "5.14");
                            data.SaveRunePage(m);
                        }
                    }
                    i++;
                    Console.WriteLine("Patch 5.14: " + i);
                }
            }
        }


        private void SaveItemsInDB() 
        {
            var items11 = Procedures.getItemsData("5.11.1");
            var items14 = Procedures.getItemsData("5.14.1");
            var champs = Procedures.getChampionData();
            var runes = Procedures.getRunesData();
            using (var data = new Data.SqlData())
            {
                data.SaveItems(items11, "5.11.1");
                data.SaveItems(items14, "5.14.1");
                data.SaveChampions(champs);
                data.SaveRunes(runes);
            }
            Console.ReadKey();
        }
    }
}
