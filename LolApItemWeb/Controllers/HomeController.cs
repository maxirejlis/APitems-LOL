using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LolApItemWeb.Models;

namespace LolApItemWeb.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View(HomeModel.Load());
        }

        [HttpPost]
        public ActionResult GetChampionData(int id, string role, string rank) 
        {
            return Json(new { result = true, data = HomeModel.GetChampionData(id,role, rank) });
        }
        [HttpPost]
        public ActionResult GetAvailableChampions(string role, string rank)
        {
            return Json(new { result = true, data = HomeModel.GetAvailableChampions(rank,role) });
        }
        //[HttpPost]
        //public ActionResult GetLaneData(string role)
        //{
        //    return Json(new { result = true, data = HomeModel.GetChampionData(id, role) });
        //} 
    }
}
