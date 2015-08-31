using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Data.Entidades
{
    public class SummonerSpell
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }

        public SummonerSpell() { }
        public SummonerSpell(dynamic o)
        {
            Id = o.id;
            Description = o.description;
            Name = o.name;
        }
    }
}
