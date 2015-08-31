using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace Data.Entidades
{
    public class Rune
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Ap { get; set; }
        public double ApPerLevel { get; set; }
        public double Mana { get; set; }
        public double ManaPerLevel { get; set; }
        public double MagicResist { get; set; }
        public double MagicResistPerLevel { get; set; }
        public double MagicPenetrationFlat { get; set; }
        
        public Rune() { }
        public Rune(dynamic o)
        {
            Id = o.id;
            Name = o.name;
            Ap = o.stats.FlatMagicDamageMod != null ? (double)o.stats.FlatMagicDamageMod : 0;
            ApPerLevel = o.stats.rFlatMagicDamageModPerLevel != null ? (double)o.stats.rFlatMagicDamageModPerLevel : 0;
            Mana =  o.stats.FlatMPPoolMod != null ? (double)o.stats.FlatMPPoolMod : 0;
            ManaPerLevel = o.stats.rFlatMPModPerLevel != null ? (double)o.stats.rFlatMPModPerLevel : 0;
            MagicResist = o.stats.FlatSpellBlockMod != null ? (double)o.stats.FlatSpellBlockMod : 0;
            MagicResistPerLevel = o.stats.rFlatSpellBlockModPerLevel != null ? (double)o.stats.rFlatSpellBlockModPerLevel : 0;
            MagicPenetrationFlat = o.stats.rFlatMagicPenetrationMod != null ? (double)o.stats.rFlatMagicPenetrationMod : 0;
        }

        public static Rune Mapper(MySqlDataReader reader) 
        {
            var r = new Rune();
            r.Id = reader.GetInt32("Id");
            r.Name = reader.GetString("Name");
            r.Ap = reader.GetDouble("Ap");
            r.ApPerLevel = reader.GetDouble("ApPerLevel");
            r.Mana = reader.GetDouble("Mana");
            r.ManaPerLevel = reader.GetDouble("ManaPerLevel");
            r.MagicResist = reader.GetDouble("MagicResist");
            r.MagicResistPerLevel = reader.GetDouble("MagicResistPerLevel");
            r.MagicPenetrationFlat = reader.GetDouble("MagicPenetrationFlat");
            return r;
        }
    }
}
