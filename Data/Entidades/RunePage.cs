using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Entidades.Caches;
using MySql.Data.MySqlClient;

namespace Data.Entidades
{
    public class RunePage
    {
        public int ParticipantId { get; set; }
        public int MatchId { get; set; }
        public IList<Rune> Runes { get; set; }

        public static RunePage Mapper(MySqlDataReader reader) 
        {
            RunePage r = new RunePage();
            r.ParticipantId = reader.GetInt32("ParticipantId");
            r.MatchId = reader.GetInt32("MatchId");
            r.Runes = new List<Rune>();
            try
            {
                var runesPage = reader.GetString("Runes");
                var runes = runesPage.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var rune in runes)
                {
                    r.Runes.Add(RunesCache.Get(int.Parse(rune)));
                }
            }
            catch
            { }
            return r;
        }

        public static RunePage GetRunePage(IList<RunePage> runes, int mId, int pId) 
        {
            var mrunes = runes.Where(r => r.MatchId == mId);
            if (mrunes != null)
                return mrunes.FirstOrDefault(r => r.ParticipantId == pId);
            return null;
        }
    }
}
