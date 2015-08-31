using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Data.Entidades
{
    public class Mastery
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string MasteryTree { get; set; } // Legal values: Defense, Offense, Utility
        //image	ImageDto	
        public string Name { get; set; }
        public string PreReq { get; set; }
        public int Ranks { get; set; }
        public string CleanDescription { get; set; }
        
        public Mastery() { }
        public Mastery(dynamic o)
        {
            Ranks = o.ranks;
            Id = o.id;
            CleanDescription = string.Join("|",o.sanitizedDescription);
            Description = string.Join("|",o.description);
            Name = o.name;
            PreReq = o.prereq;
            MasteryTree = o.masteryTree;
        }
    }
}
