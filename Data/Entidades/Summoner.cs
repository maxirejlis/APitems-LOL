using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Data.Entidades
{
    public class Summoner
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ProfileIcon { get; set; }
        public string SummonerLevel { get; set; }
        public string RevisionDate { get; set; }

        public Summoner(string wsData)
        {
            wsData = wsData.Split('{').Last().Split('}').First();
            string[] data = wsData.Split(',');
            Id = data[0].Split(':').Last();
            Name = data[1].Split(':').Last().Trim('"');
            ProfileIcon = data[2].Split(':').Last();
            SummonerLevel = data[3].Split(':').Last();
            RevisionDate = data[4].Split(':').Last();
        }
        public Summoner()
        {
            Id = null;
            Name = null;
            ProfileIcon = null;
            SummonerLevel = null;
            RevisionDate = null;
        }

    }
}
