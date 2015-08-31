using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace Data.Entidades
{
    public class Champion
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Title {get;set;}
        public IList<ClassType> Classes { get; set; }
        public Stat Stat { get; set; }
        public IList<ChampionSpell> Spells {get;set;}

        public Champion() { }

        public Champion(dynamic obj) 
        {
            Id = obj.Value.id;
            Name = obj.Value.name;
            Title = obj.Value.title;
            Stat = new Stat(obj.Value.stats);
            Spells = new List<ChampionSpell>();
            foreach (var spell in obj.Value.spells)
            {
                Spells.Add(new ChampionSpell(spell));
            }
        }

        public static Champion Mapper(MySqlDataReader r) 
        {
            var c = new Champion();
            c.Id = r.GetInt32("Id");
            c.Name = r.GetString("Name");
            c.Title = r.GetString("Title");
            c.Stat = new Stat();
            c.Stat.Mana = r.GetDouble("Mana");
            c.Stat.ManaPerLv = r.GetDouble("ManaPerLevel");
            c.Stat.MagicResist = r.GetDouble("MagicResist");
            c.Stat.MagicResistPerLv = r.GetDouble("MagicResistPerLevel");
            return c;
        }
    }

    public class ChampionSpell 
    {
        public string Name {get;set;}
        public string Description {get;set;}
        public IList<int> Cost {get;set;}
        public ResourceType CostType {get;set;}
        public IList<int> CoolDown {get;set;}
        public ChampionSpell() {
        }

        public ChampionSpell(dynamic o) 
        {
            Name = o.name;
            Description = o.description;
            CostType = ParseResource(o.costType.Value);
            Cost = new List<int>();
            foreach (var ob in o.cost) 
            {
                Cost.Add((int)ob.Value);
            }
            CoolDown = new List<int>();
            foreach (var ob in o.cooldown)
            {
                CoolDown.Add((int)ob.Value);
            }
        }
        private ResourceType ParseResource(string s) 
        {
            switch (s) 
            {
                case "Energy": return ResourceType.Energy;
                case "NoCostor50Fury": return ResourceType.Fury;
                case "NoCost": 
                    return ResourceType.NoCost;
                case "Mana":
                    return ResourceType.Mana;
                case "pofcurrentHealth":
                case "Health":
                    return ResourceType.Health;
                default: return ResourceType.Unidentified;
            }
        }
    }

    public class Stat 
    {
        public double AttackRange {get;set;}
        public double AttackDamage {get;set;}
        public double AttackDamagePerLv {get;set;}
        public double AttackSpeedPerLv {get;set;}
        public double AttackSpeedOffset {get;set;}
        
        public double Mana {get;set;}
        public double ManaPerLv {get;set;}
        public double ManaRegenPerLv {get;set;}
        public double ManaRegen {get;set;}
        
        public double HP {get;set;}
        public double HPPerLv {get;set;}
        public double HPRegen {get;set;}
        public double HpRegenPerLv {get;set;}

        public double Armor {get;set;}
        public double ArmorPerLv {get;set;}
        public double MagicResistPerLv {get;set;}
        public double MagicResist {get;set;}
        
        public double Critical {get;set;}
        public double CriticalPerLv {get;set;}    
                
        public double MovementSpeed {get;set;}    

        public Stat(){}
        public Stat(dynamic o)
        {
            AttackRange = o.attackrange;
            ManaPerLv = o.mpperlevel;
            Mana = o.mp;
            AttackDamage = o.attackdamage;
            HP = o.hp;
            HPPerLv = o.hpperlevel;
            AttackDamagePerLv = o.attackdamageperlevel;
            Armor = o.armor;
            ManaRegenPerLv = o.mpregenperlevel;
            HPRegen = o.hpregen;
            CriticalPerLv = o.critperlevel;
            MagicResistPerLv = o.spellblockperlevel;
            ManaRegen = o.mpregen;
            AttackSpeedPerLv = o.attackspeedperlevel;
            MagicResist = o.spellblock;
            MovementSpeed = o.movespeed;
            AttackSpeedOffset = o.attackspeedoffset;
            Critical = o.crit;
            HpRegenPerLv = o.hpregenperlevel;
            ArmorPerLv = o.armorperlevel;
        }
    }
}
