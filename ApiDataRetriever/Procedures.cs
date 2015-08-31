
using System.IO;
using Newtonsoft.Json;
using Data.Entidades;
using ApiDataRetriever.Entidades.Managers;
using System;
using System.Collections.Generic;
using System.Net;
using System.Configuration;
using ApiDataRetriever.Managers;

namespace ApiDataRetriever
{
    public static class Procedures
    {
        private static string apikey = ConfigurationSettings.AppSettings["Api.Key"];
        private static string locale = ConfigurationSettings.AppSettings["Api.Locale"];
        private static string region = ConfigurationSettings.AppSettings["Api.Region"];
        
        private static long summonerId = Convert.ToInt64(ConfigurationSettings.AppSettings["Summoner.Id"]);

        public static IList<Item> getItemsData(string version= "5.15.1") 
        {
            string urlRequest = string.Format("https://global.api.pvp.net/api/lol/static-data/{0}/v1.2/item?locale={2}&version={1}&itemListData=all&api_key={3}", region, version, locale, apikey);
            WebRequest request = WebRequest.Create(urlRequest);
            request.Headers.Add(HttpRequestHeader.AcceptLanguage, "en-us");
            request.Headers.Add(HttpRequestHeader.AcceptCharset, "ISO-8859-1,utf-8");

            string textResponse = string.Empty;

            IList<Item> items = new List<Item>();
            using (WebResponse response = request.GetResponse())
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    textResponse = reader.ReadToEnd();
                    dynamic dynObject = JsonConvert.DeserializeObject(textResponse);
                    foreach (var obj in dynObject.data)
                    {
                        var i = new Item(obj.Value);
                        items.Add(i);
                    }
                }
            }
            return items;
        }

        public static IList<Champion> getChampionData()
        {
            dynamic dynObject;
            var urlRequest = string.Format("https://global.api.pvp.net/api/lol/static-data/{0}/v1.2/champion?champData=all&api_key={1}", region, apikey);
            var request = RequestHelper.CreateRequest(urlRequest);

            using (WebResponse response = request.GetResponse())
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {

                    string data = reader.ReadToEnd();
                    dynObject = JsonConvert.DeserializeObject(data);
                }
            }

            var champions = new List<Champion>();
            foreach (var obj in dynObject.data)
            {
                try
                {
                    var champion = new Champion();
                    champion.Id = obj.Value.id;
                    champion.Name = obj.Value.name;
                    champion.Title = obj.Value.title;
                    champion.Stat = new Stat(obj.Value.stats);
                    champion.Spells = new List<ChampionSpell>();
                    foreach (var spell in obj.Value.spells)
                    {
                        champion.Spells.Add(new ChampionSpell(spell));
                    }
                    champions.Add(champion);
                }
                catch { }
            }
            return champions;
        }

        public static IList<SummonerSpell> getSummonerSpellsData() 
        {        
            //dynamic dynObject;
            //var cacheChampionData = string.Format("{0}/{1}", CacheDirectory, "championData.txt");
            //if (useCache && File.Exists(cacheChampionData))
            //{
            //}
            string urlRequest = string.Format("https://global.api.pvp.net/api/lol/static-data/{0}/v1.2/summoner-spell?locale={1}&spellData=all&api_key={2}", region, locale, apikey);
            WebRequest request = WebRequest.Create(urlRequest);
            request.Headers.Add(HttpRequestHeader.AcceptLanguage, "en-us");
            request.Headers.Add(HttpRequestHeader.AcceptCharset, "ISO-8859-1,utf-8");

            string textResponse = string.Empty;
            IList<SummonerSpell> spells = new List<SummonerSpell>();
            using (WebResponse response = request.GetResponse())
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    textResponse = reader.ReadToEnd();
                    dynamic dynObject = JsonConvert.DeserializeObject(textResponse);
                    foreach (var obj in dynObject.data)
                    {
                        try
                        {
                            var s = new SummonerSpell(obj.Value);
                            spells.Add(s);
                        }
                        catch { };
                    }
                }
            }
            return spells;
        }

        public static Match GetMatch(string id) 
        {
            int retries = 0;
            while (retries < 3)
            {
                try
                {

                    string urlRequest = string.Format("https://{0}.api.pvp.net/api/lol/{0}/v2.2/match/{1}?includeTimeline={2}&api_key={3}", region, id, "true", apikey);
                    WebRequest request = WebRequest.Create(urlRequest);
                    request.Headers.Add(HttpRequestHeader.AcceptLanguage, "en-us");
                    request.Headers.Add(HttpRequestHeader.AcceptCharset, "ISO-8859-1,utf-8");

                    string textResponse = string.Empty;
                    Match m;
                    using (WebResponse response = request.GetResponse())
                    {
                        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                        {
                            textResponse = reader.ReadToEnd();
                            dynamic dynObject = JsonConvert.DeserializeObject(textResponse);
                            m = new Match(dynObject);
                            if (m.QueueType != QueueType.RANKED_SOLO_5x5)
                                return null;
                        }
                    }

                    return m;
                }
                catch (Exception ex)
                {
                    LogHelper.Logger(string.Format("Warn - MatchId: {0}, ex: {1}", id, ex.Message) );
                    System.Threading.Thread.Sleep(2000);
                    retries++;
                }
            }
            LogHelper.Logger(string.Format("Error - MatchId: {0}", id));
            return null;
        }

        internal static IList<Mastery> getMasteriesData()
        {
            string urlRequest = string.Format("https://global.api.pvp.net/api/lol/static-data/{0}/v1.2/mastery?locale={1}&masteryListData=all&api_key={2}", region, locale, apikey);
            WebRequest request = WebRequest.Create(urlRequest);
            request.Headers.Add(HttpRequestHeader.AcceptLanguage, "en-us");
            request.Headers.Add(HttpRequestHeader.AcceptCharset, "ISO-8859-1,utf-8");

            string textResponse = string.Empty;

            IList<Mastery> masteries = new List<Mastery>();
            using (WebResponse response = request.GetResponse())
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    textResponse = reader.ReadToEnd();
                    dynamic dynObject = JsonConvert.DeserializeObject(textResponse);
                    foreach (var obj in dynObject.data)
                    {
                       var m = new Mastery(obj.Value);
                       masteries.Add(m);
                    }
                    
                }
            }
            return masteries;
        }

        internal static IList<Rune> getRunesData()
        {
            string urlRequest = string.Format("https://global.api.pvp.net/api/lol/static-data/{0}/v1.2/rune?locale={1}&runeListData=all&api_key={2}", region, locale, apikey);
            WebRequest request = WebRequest.Create(urlRequest);
            request.Headers.Add(HttpRequestHeader.AcceptLanguage, "en-us");
            request.Headers.Add(HttpRequestHeader.AcceptCharset, "ISO-8859-1,utf-8");

            string textResponse = string.Empty;

            IList<Rune> runes = new List<Rune>();
            using (WebResponse response = request.GetResponse())
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    textResponse = reader.ReadToEnd();
                    dynamic dynObject = JsonConvert.DeserializeObject(textResponse);
                    foreach (var obj in dynObject.data)
                    {
                        var r = new Rune(obj.Value);
                        runes.Add(r);
                    }

                }
            }
            return runes;
        }
    }
}
