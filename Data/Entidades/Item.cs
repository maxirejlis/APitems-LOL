using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Data.Entidades
{
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int GoldCost { get; set; }
        public string Patch { get; set; }
        public ItemStats ItemStats { get; set; }

        public Item() { }
        public Item(dynamic o) 
        {
            Id = (int)o.id;
            Name = o.name;
            Description = o.sanitizedDescription;
            GoldCost = (int)o.gold.total.Value;
            ItemStats = new ItemStats(o.stats, Name);
        }

        public static Item Mapper(MySqlDataReader reader) {
            var i = new Item();
            i.Id = reader.GetInt32("Id");
            i.Name = reader.GetString("Name");
            i.Description = reader.GetString("Description");
            i.GoldCost = reader.GetInt32("GoldCost");
            i.Patch = reader.GetString("Patch");
            i.ItemStats = new ItemStats();
            i.ItemStats.AP = reader.GetInt32("AP");
            i.ItemStats.ApPerStack = reader.GetInt32("AP_Stack");
            i.ItemStats.TotalStacks = reader.GetInt32("TotalStacks");
            i.ItemStats.Mana = reader.GetInt32("Mana");
            i.ItemStats.ManaPerStack = reader.GetInt32("ManaStacks");
            i.ItemStats.TotalManaStacks = reader.GetInt32("TotalManaStacks");
            i.ItemStats.MR = reader.GetInt32("Mr");
            i.ItemStats.MagicPenetrationFlat = reader.GetInt32("MagicPenFlat");
            i.ItemStats.MagicPenetrationPerc = reader.GetInt32("MagicPenPerc");
            return i;

        }
    }

    public class ItemStats 
    {
        public int AP { get; set; }
        public int ApPerStack { get; set; }
        public int TotalStacks { get; set; }
        public int Mana { get; set; }
        public int ManaPerStack { get; set; }
        public int TotalManaStacks { get; set; }
        public int MR { get; set; }
        public int MagicPenetrationPerc { get; set; }
        public int MagicPenetrationFlat { get; set; }

        public ItemStats() 
        {
        
        }
        public ItemStats(dynamic o, string name) 
        {
            if (o.FlatMagicDamageMod != null)
                AP = (int)o.FlatMagicDamageMod;

            if (o.FlatSpellBlockMod != null)
                MR = (int)o.FlatSpellBlockMod;
            if (o.FlatMPPoolMod != null)
                Mana = (int)o.FlatMPPoolMod;
            switch (name)
            {
                case "Rod of Ages":
                    TotalStacks = 10;
                    if (o.Effect3Amount != null)
                        ApPerStack = (int)o.Effect3Amount;
                    TotalManaStacks = 10;
                    if (o.Effect2Amount != null)
                        ManaPerStack = (int)o.Effect2Amount;
                    break;
                case "Guinsoo's Rageblade":
                    TotalStacks = 8;
                    ApPerStack = 4;
                    break;
                case "Mejai's Soulstealer":
                    TotalStacks = 20;
                    ApPerStack = 8;
                    break;
                case "Void Staff":
                    MagicPenetrationPerc = 35;
                    break;
                case "Liandry's Torment":
                    MagicPenetrationFlat = 15;
                    break;
                case "Haunting Guise":
                    MagicPenetrationFlat = 15;
                    break;
                case "Abyssal Scepter":
                    MagicPenetrationFlat = 20;
                    break;
                case "Sorcerer's Shoes":
                    MagicPenetrationFlat = 15;
                    break;
                case "Tear of the Goddess":
                    TotalManaStacks = 750;
                    ManaPerStack = 1;
                    break;
                case "Archangel's Staff":
                    TotalManaStacks = 750;
                    ManaPerStack = 1;
                    break;
            }
        }

        public static double GetTotalAP(Item i, int minute, Champion c)
        {
            switch (i.Name)
            {
                case "Rod of Ages":
                    return i.ItemStats.AP + i.ItemStats.ApPerStack * (minute > 25 ? i.ItemStats.TotalStacks : i.ItemStats.TotalStacks/2);
                case "Mejai's Soulstealer":
                    return i.ItemStats.AP + i.ItemStats.ApPerStack * (i.ItemStats.TotalStacks /2);
                case "Archangel's Staff":
                    return i.ItemStats.AP + (0.03* (i.ItemStats.Mana + i.ItemStats.ManaPerStack * i.ItemStats.TotalManaStacks + c.Stat.Mana + c.Stat.ManaPerLv*9) );
                case "Seraph's Embrace":
                    return i.ItemStats.AP + (0.03 * (i.ItemStats.Mana + i.ItemStats.ManaPerStack * i.ItemStats.TotalManaStacks + c.Stat.Mana + c.Stat.ManaPerLv*9));
            }
            return i.ItemStats.AP;
        }
    }
}
