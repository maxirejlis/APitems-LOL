using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LolApItemWeb.ViewModels
{
    public class HomeViewModel
    {
        public IList<ChampionViewModel> Champions { get; set; }
        public SelectList Roles { get; set; }
        public SelectList Ranks { get; set; }
    }

    public class ChampionViewModel {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
    }


    public class DataItemViewModel
    {
        public string Patch { get; set; }

        public string Name { get; set; }
        public int MinuteFrom { get; set; }
        public int MinuteTo { get; set; }
        
    }
    public class DataItemViewModelAux
    {
        public int Minute { get; set; }
        public string Name { get; set; }
        public int Pickrate { get; set; }
    }

    public class ItemsViewModel 
    {
        public int Minute { get; set; }
        public int TotalGames { get; set; }
        public List<ItemViewModel> Items { get; set; }
    }
    public class ItemViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public string PickRate { get; set; }
    }
}