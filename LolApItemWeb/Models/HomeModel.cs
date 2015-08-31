using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Data;
using Data.Entidades;
using Data.Entidades.Caches;
using LolApItemWeb.ViewModels;

namespace LolApItemWeb.Models
{
    public static class HomeModel
    {
        public static HomeViewModel Load()
        {
            HomeViewModel hmv = new HomeViewModel();
            hmv.Roles = LoadRoles();
            hmv.Ranks = LoadRanks();
            IList<Champion> champions;
            using (var data = new SqlData())
            {
                champions = data.GetChampions();
            }
            hmv.Champions = new List<ChampionViewModel>();
            foreach (var champion in champions)
            {
                var championVM = new ChampionViewModel();
                championVM.Id = champion.Id;
                championVM.Name = champion.Name;
                string linkName;
                if (champion.Name.Contains(" "))
                    linkName = champion.Name.Replace(" ", string.Empty).Replace("'", string.Empty).Replace(".", string.Empty);
                else
                    linkName = champion.Name.Replace("'", string.Empty).Replace(".", string.Empty).ToLower().FirstLetterToUpper();
                switch (linkName) {
                    case "Kogmaw":
                        linkName = "KogMaw";
                        break;
                    case "Reksai":
                        linkName = "RekSai";
                        break;
                    case "Fiddlesticks":
                        linkName = "FiddleSticks";
                        break;
                    case "Wukong":
                        linkName = "MonkeyKing";
                        break;
                }
                championVM.ImageUrl = string.Format(@"http://ddragon.leagueoflegends.com/cdn/5.16.1/img/champion/{0}.png", linkName);
                hmv.Champions.Add(championVM);
            }
            return hmv;
        }

        public static IList<int> GetAvailableChampions(string rank, string role) 
        {
            IList<AvgTimelineDto> avg;
            using (var data = new SqlData())
            {
                avg = data.GetDistinctAvgTimeline();
            }
            if (!string.IsNullOrWhiteSpace(rank) && rank != "All")
            {
                avg = avg.Where(a => a.Rank == rank).ToList();
            }
            if (!string.IsNullOrWhiteSpace(role) && role != "All") 
            {
                avg = avg.Where(a => a.Role == role).ToList();
            }
            return avg.Select(a => a.ChampionId).Distinct().ToList();
        }

        public static object GetChampionData(int id,string role,string rank)
        {
            IList<AvgTimeLine> timelines = AvgTimelinesCache.Get();

            timelines = ApplyFilters(id, role, rank, timelines);

            var itemsDataP11 = new List<ItemsViewModel>();
            var itemsDataP14 = new List<ItemsViewModel>();
            var tableData = new List<object>();
            var timelineData = new List<DataItemViewModel>();

            TableDataAddTitles(tableData);

            foreach (var groupTimes in timelines.OrderBy(t => t.Minute).GroupBy(t => t.Minute))
            {
                var datos = new List<object>();
                foreach (var patch in groupTimes.OrderBy(t => t.Patch).GroupBy(t => t.Patch))
                {
                    var totalGames = patch.Sum(t => t.TotalGames);
                    if (totalGames < 5)
                        break;
                    if (patch.Key.StartsWith("5.11"))
                    {
                        datos.Add(groupTimes.Key.ToString());
                        datos.Add((int)patch.Average(a => a.AvgAp));
                        datos.Add((int)patch.Average(a => a.AvgMr));

                        var itemsView = GetPatchItems(groupTimes.Key, patch, totalGames);
                        itemsDataP11.Add(itemsView);
                    }
                    else
                    {

                        while (datos.Count < 3)
                        {
                            //Horrible Hotfix, we're out of time.
                            if (datos.Count == 0)
                            {
                                datos.Add("5.11");
                            }
                            else
                            {
                                datos.Add(0);
                            }
                        }

                        datos.Add((int)patch.Average(a => a.AvgAp));
                        datos.Add((int)patch.Average(a => a.AvgMr));

                        var itemsView = GetPatchItems(groupTimes.Key, patch, totalGames);
                        itemsDataP14.Add(itemsView);
                    }
                }
                if (datos.Count > 0)
                {
                    while (datos.Count < 5)
                    {
                        datos.Add(0);
                    }
                    tableData.Add(datos);
                }

            }

            //PopulateTimelineData(itemsDataP11, itemsDataP14, timelineData);

            return new { tableData = tableData, itemsData = new { itemsP11 = itemsDataP11, itemsP14 = itemsDataP14 }, timelineData = timelineData};
            //return new { minute = timelines.Select(t => t.Minute).Distinct(), ap11 = avgApxMinP11, mr11 = avgMrxMinP11, ap14 = avgApxMinP14, mr14 = avgMrxMinP14,  };
        }

        private static void PopulateTimelineData(List<ItemsViewModel> itemsDataP11, List<ItemsViewModel> itemsDataP14, List<DataItemViewModel> timelineData)
        {
            #region Rabadon
            IList<DataItemViewModelAux> divm = new List<DataItemViewModelAux>();
            foreach (var item in itemsDataP11)
            {
                var raba = item.Items.FirstOrDefault(i => i.Name.StartsWith("Rabadon"));
                var pickrate = int.Parse(raba.PickRate.Replace("%", string.Empty));
                if (pickrate > 15)
                    divm.Add(new DataItemViewModelAux { Minute = item.Minute, Name = raba.Name, Pickrate = pickrate });
            }
            timelineData.Add(new DataItemViewModel { Name = divm.First().Name, Patch = "5.11", MinuteFrom = divm.First().Minute, MinuteTo = divm.Last().Minute });

            divm = new List<DataItemViewModelAux>();
            foreach (var item in itemsDataP14)
            {
                var raba = item.Items.FirstOrDefault(i => i.Name.StartsWith("Rabadon"));
                var pickrate = int.Parse(raba.PickRate.Replace("%", string.Empty));
                if (pickrate > 15)
                    divm.Add(new DataItemViewModelAux { Minute = item.Minute, Name = raba.Name, Pickrate = pickrate });
            }
            timelineData.Add(new DataItemViewModel { Name = divm.First().Name, Patch = "5.11", MinuteFrom = divm.First().Minute, MinuteTo = divm.Last().Minute });
            #endregion

            #region Needlessly
            divm = new List<DataItemViewModelAux>();
            foreach (var item in itemsDataP11)
            {
                var raba = item.Items.FirstOrDefault(i => i.Name.StartsWith("Needlessly"));
                var pickrate = int.Parse(raba.PickRate.Replace("%", string.Empty));
                if (pickrate > 15)
                    divm.Add(new DataItemViewModelAux { Minute = item.Minute, Name = raba.Name, Pickrate = pickrate });
            }
            timelineData.Add(new DataItemViewModel { Name = divm.First().Name, Patch = "5.11", MinuteFrom = divm.First().Minute, MinuteTo = divm.Last().Minute });

            divm = new List<DataItemViewModelAux>();
            foreach (var item in itemsDataP14)
            {
                var raba = item.Items.FirstOrDefault(i => i.Name.StartsWith("Needlessly"));
                var pickrate = int.Parse(raba.PickRate.Replace("%", string.Empty));
                if (pickrate > 15)
                    divm.Add(new DataItemViewModelAux { Minute = item.Minute, Name = raba.Name, Pickrate = pickrate });
            }
            timelineData.Add(new DataItemViewModel { Name = divm.First().Name, Patch = "5.11", MinuteFrom = divm.First().Minute, MinuteTo = divm.Last().Minute });
            #endregion

            #region Zhonya
            divm = new List<DataItemViewModelAux>();
            foreach (var item in itemsDataP11)
            {
                var raba = item.Items.FirstOrDefault(i => i.Name.StartsWith("Zhonya"));
                var pickrate = int.Parse(raba.PickRate.Replace("%", string.Empty));
                if (pickrate > 15)
                    divm.Add(new DataItemViewModelAux { Minute = item.Minute, Name = raba.Name, Pickrate = pickrate });
            }
            timelineData.Add(new DataItemViewModel { Name = divm.First().Name, Patch = "5.11", MinuteFrom = divm.First().Minute, MinuteTo = divm.Last().Minute });

            divm = new List<DataItemViewModelAux>();
            foreach (var item in itemsDataP14)
            {
                var raba = item.Items.FirstOrDefault(i => i.Name.StartsWith("Zhonya"));
                var pickrate = int.Parse(raba.PickRate.Replace("%", string.Empty));
                if (pickrate > 15)
                    divm.Add(new DataItemViewModelAux { Minute = item.Minute, Name = raba.Name, Pickrate = pickrate });
            }
            timelineData.Add(new DataItemViewModel { Name = divm.First().Name, Patch = "5.11", MinuteFrom = divm.First().Minute, MinuteTo = divm.Last().Minute });
            #endregion

            #region Luden's Echo
            divm = new List<DataItemViewModelAux>();
            foreach (var item in itemsDataP11)
            {
                var raba = item.Items.FirstOrDefault(i => i.Name.StartsWith("Luden"));
                var pickrate = int.Parse(raba.PickRate.Replace("%", string.Empty));
                if (pickrate > 15)
                    divm.Add(new DataItemViewModelAux { Minute = item.Minute, Name = raba.Name, Pickrate = pickrate });
            }
            timelineData.Add(new DataItemViewModel { Name = divm.First().Name, Patch = "5.11", MinuteFrom = divm.First().Minute, MinuteTo = divm.Last().Minute });

            divm = new List<DataItemViewModelAux>();
            foreach (var item in itemsDataP14)
            {
                var raba = item.Items.FirstOrDefault(i => i.Name.StartsWith("Luden"));
                var pickrate = int.Parse(raba.PickRate.Replace("%", string.Empty));
                if (pickrate > 15)
                    divm.Add(new DataItemViewModelAux { Minute = item.Minute, Name = raba.Name, Pickrate = pickrate });
            }
            timelineData.Add(new DataItemViewModel { Name = divm.First().Name, Patch = "5.11", MinuteFrom = divm.First().Minute, MinuteTo = divm.Last().Minute });
            #endregion
        }

        private static void TableDataAddTitles(List<object> tableData)
        {
            var titles = new List<object>();
            titles.Add("Minute");
            titles.Add("Average AP - 5.11");
            titles.Add("Average MR - 5.11");
            titles.Add("Average AP - 5.14");
            titles.Add("Average MR - 5.14");
            tableData.Add(titles);
        }

        private static IList<AvgTimeLine> ApplyFilters(int id, string role, string rank, IList<AvgTimeLine> timelines)
        {
            if (id > 0)
                timelines = timelines.Where(i => i.ChampionId == id).ToList();
            if (!string.IsNullOrWhiteSpace(role) && role != "All")
                timelines = timelines.Where(t => t.Role == role).ToList();
            if (!string.IsNullOrWhiteSpace(rank) && role != "All")
                timelines = timelines.Where(t => t.Rank == rank).ToList();
            return timelines;
        }

        private static ItemsViewModel GetPatchItems(int minute, IGrouping<string, AvgTimeLine> patch, int totalGames )
        {
            var itemsView = new ItemsViewModel();
            itemsView.TotalGames = totalGames;
            itemsView.Minute = minute;
            itemsView.Items = new List<ItemViewModel>();
            var dicItems = new Dictionary<int, int>();
            foreach (var a in patch)
            {
                var itemsPatch = a.Items.Split(new []{'|'}, StringSplitOptions.RemoveEmptyEntries);
                foreach (var item in itemsPatch)
                {
                    var dataItem = item.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                    var key = int.Parse(dataItem[0]);
                    var val = int.Parse(dataItem[1]);

                    if (patch.Key.Contains("5.11"))
                    {
                        var name = ItemsCache.GetItemV511(key).Name;
                        if (name.Contains("Potion") || name.Contains("Biscuit"))
                            continue;
                    }
                    else {
                        var name = ItemsCache.GetItemV514(key).Name;
                        if (name.Contains("Potion") || name.Contains("Biscuit"))
                            continue;
                    }
                    if (dicItems.ContainsKey(key))
                    {
                        dicItems[key] += (val);
                    }
                    else
                    {
                        dicItems.Add(key, (val));
                    }
                }
            }
            try
            {
                dicItems = dicItems.OrderByDescending(d => d.Value).Take(8).ToDictionary(d => d.Key, d => d.Value);
                foreach (var item in dicItems)
                {
                    var itemVM = new ItemViewModel();
                    itemVM.Id = item.Key;
                    if (patch.Key.StartsWith("5.11"))
                    {
                        itemVM.Name = ItemsCache.GetItemV511(item.Key).Name;
                        itemVM.ImageUrl = string.Format(@"http://ddragon.leagueoflegends.com/cdn/5.11.1/img/item/{0}.png", item.Key);
                    }
                    else
                    {
                        itemVM.Name = ItemsCache.GetItemV514(item.Key).Name;
                        itemVM.ImageUrl = string.Format(@"http://ddragon.leagueoflegends.com/cdn/5.14.1/img/item/{0}.png", item.Key);
                    }
                    itemVM.PickRate = string.Format("{0:0.00}%", ((double)item.Value / totalGames) * 100);
                    
                    itemsView.Items.Add(itemVM);
                }
            }
            catch { }
            return itemsView;
        }

        private static SelectList LoadRoles()
        {
            SelectList list = new SelectList(new List<string>
            {
                "ADC", 
                "Jungle",
                "Mid", 
                "Support",
                "Top"
             });  
            return list;
        }
        private static SelectList LoadRanks()
        {
            SelectList list = new SelectList(new List<string>
            {
                "UNRANKED",
                "BRONZE", 
                "SILVER",
                "GOLD", 
                "PLATINUM",
                "DIAMOND",
                "MASTER",
                "CHALLENGER"
             });
            return list;
        }

    }
}