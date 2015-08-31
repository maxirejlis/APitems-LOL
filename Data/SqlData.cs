using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Entidades;
using MySql.Data.MySqlClient;
namespace Data
{
    public class SqlData : IDisposable
    {
        private MySqlConnection connection;
        private const string ConnectionString = "Server=localhost;Database=leagueoflegends;Uid=admin;Pwd=admin;";


        public SqlData() 
        {
            connection = new MySqlConnection(ConnectionString);
            connection.Open();
        }

        public void SaveItems(IList<Item> items,string patch) 
        {
            foreach (var item in items)
            {
                MySqlCommand cmd = new MySqlCommand("insert_item", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new MySqlParameter("pId", item.Id));
                cmd.Parameters.Add(new MySqlParameter("pName", item.Name));
                cmd.Parameters.Add(new MySqlParameter("pDescription", item.Description));
                cmd.Parameters.Add(new MySqlParameter("pGoldCost", item.GoldCost));
                cmd.Parameters.Add(new MySqlParameter("pAp", item.ItemStats.AP));
                cmd.Parameters.Add(new MySqlParameter("pAp_Stack", item.ItemStats.ApPerStack));
                cmd.Parameters.Add(new MySqlParameter("pMana", item.ItemStats.Mana));
                cmd.Parameters.Add(new MySqlParameter("pManaStacks", item.ItemStats.ManaPerStack));
                cmd.Parameters.Add(new MySqlParameter("pTotalManaStacks", item.ItemStats.TotalManaStacks));
                cmd.Parameters.Add(new MySqlParameter("pTotalStacks", item.ItemStats.TotalStacks));
                cmd.Parameters.Add(new MySqlParameter("pMagicPenFlat", item.ItemStats.MagicPenetrationFlat));
                cmd.Parameters.Add(new MySqlParameter("pMagicPenPerc", item.ItemStats.MagicPenetrationPerc));
                cmd.Parameters.Add(new MySqlParameter("pMr", item.ItemStats.MR));
                cmd.Parameters.Add(new MySqlParameter("pPatch", patch));
               
                cmd.ExecuteNonQuery();
            }
        }

        public void SaveMatch(Match m)
        {

                MySqlCommand cmd = new MySqlCommand("insert_match", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new MySqlParameter("pId", m.Id));
                cmd.Parameters.Add(new MySqlParameter("pRegion", m.Region));
                cmd.Parameters.Add(new MySqlParameter("pDuration", m.Duration.TotalSeconds));
                cmd.Parameters.Add(new MySqlParameter("pMapId", m.MapId));
                cmd.Parameters.Add(new MySqlParameter("pMatchMode", m.QueueType.ToString()));
                cmd.Parameters.Add(new MySqlParameter("pPatch", m.Patch));

                cmd.ExecuteNonQuery();
            
        }

        public void SaveChampions(IList<Champion> champions)
        {
            foreach (var champion in champions)
            {
                MySqlCommand cmd = new MySqlCommand("insert_champion", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new MySqlParameter("pId", champion.Id));
                cmd.Parameters.Add(new MySqlParameter("pName", champion.Name));
                cmd.Parameters.Add(new MySqlParameter("pTitle", champion.Title));
                cmd.Parameters.Add(new MySqlParameter("pMana", champion.Stat.Mana));
                cmd.Parameters.Add(new MySqlParameter("pManaPerLevel", champion.Stat.ManaPerLv));
                cmd.Parameters.Add(new MySqlParameter("pMagicResist", champion.Stat.MagicResist));
                cmd.Parameters.Add(new MySqlParameter("pMagicResistPerLevel", champion.Stat.MagicResistPerLv));

                cmd.ExecuteNonQuery();
            }
        }

        public void SaveRunes(IList<Rune> runes) 
        {
            foreach (var rune in runes)
            {
                MySqlCommand cmd = new MySqlCommand("insert_rune", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new MySqlParameter("pId", rune.Id));
                cmd.Parameters.Add(new MySqlParameter("pName", rune.Name));
                cmd.Parameters.Add(new MySqlParameter("pAp", rune.Ap));
                cmd.Parameters.Add(new MySqlParameter("pApPerLevel", rune.ApPerLevel));
                cmd.Parameters.Add(new MySqlParameter("pMana", rune.Mana));
                cmd.Parameters.Add(new MySqlParameter("pManaPerLevel", rune.ManaPerLevel));
                cmd.Parameters.Add(new MySqlParameter("pMagicResist", rune.MagicResist));
                cmd.Parameters.Add(new MySqlParameter("pMagicResistPerLevel", rune.MagicResistPerLevel));
                cmd.Parameters.Add(new MySqlParameter("pMagicPenetrationFlat", rune.MagicPenetrationFlat));

                cmd.ExecuteNonQuery();
            }
        }

        public void SaveRunePage(Match m)
        {
            foreach (var t in m.Teams)
            {
                foreach (var p in t.Players)
                {
                    MySqlCommand cmd = new MySqlCommand("insert_runepage", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new MySqlParameter("pParticipantId", p.ParticipantId));
                    cmd.Parameters.Add(new MySqlParameter("pMatchId", m.Id));
                    cmd.Parameters.Add(new MySqlParameter("pRunes", string.Join("|",p.Runes.Select(r=>r.Id))));
                
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void SaveFinalStats(Match m, string patch)
        {
            foreach (var t in m.Teams)
            {
                foreach (var p in t.Players)
                {
                    MySqlCommand cmd = new MySqlCommand("insert_endgamestats", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new MySqlParameter("pParticipantId", p.ParticipantId));
                    cmd.Parameters.Add(new MySqlParameter("pMatchId", m.Id));
                    cmd.Parameters.Add(new MySqlParameter("pRank", p.Rank));
                    cmd.Parameters.Add(new MySqlParameter("pTeamId", t.TeamId));
                    cmd.Parameters.Add(new MySqlParameter("pChampionId", p.SelectedChampion.Id));
                    cmd.Parameters.Add(new MySqlParameter("pRole", p.Role.ToString()));
                    cmd.Parameters.Add(new MySqlParameter("pLane", p.Lane.ToString()));
                    cmd.Parameters.Add(new MySqlParameter("pMagicDamageDealtToChampions", p.Finalstats.MagicDamageDealtToChampions));
                    cmd.Parameters.Add(new MySqlParameter("pPhysicalDamageDealtToChampions", p.Finalstats.PhysicalDamageDealtToChampions));
                    cmd.Parameters.Add(new MySqlParameter("pTrueDamageDealtToChampions", p.Finalstats.TrueDamageDealtToChampions));                    
                    cmd.Parameters.Add(new MySqlParameter("pTotalAP", p.Finalstats.TotalAp));
                    cmd.Parameters.Add(new MySqlParameter("pTotalMR", p.Finalstats.TotalMr));
                    cmd.Parameters.Add(new MySqlParameter("pWinner", t.Winner));
                    cmd.Parameters.Add(new MySqlParameter("pItemsIds", string.Join("|", p.Finalstats.Items.Select(i=>i.Id))));
                    cmd.Parameters.Add(new MySqlParameter("pPatch", patch));
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void SaveApTimeline(Match m, string patch)
        {
            foreach (var t in m.Teams)
            {
                foreach (var p in t.Players)
                {
                    IList<ApTimeLine> timelines = ApTimeLine.GetPlayerApTimeline(p, m.Patch, m.Id, t.TeamId);
                    foreach (var time in timelines)
                    {
                        MySqlCommand cmd = new MySqlCommand("insert_timeline", connection);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new MySqlParameter("pMatchId", time.MatchId));
                        cmd.Parameters.Add(new MySqlParameter("pParticipantId", time.PlayerId));
                        cmd.Parameters.Add(new MySqlParameter("pRank", p.Rank));
                        cmd.Parameters.Add(new MySqlParameter("pChampionId", p.SelectedChampion.Id));
                        cmd.Parameters.Add(new MySqlParameter("pRole", p.Role.ToString()));
                        cmd.Parameters.Add(new MySqlParameter("pLane", p.Lane.ToString()));
                        cmd.Parameters.Add(new MySqlParameter("pTeamId", time.TeamId));
                        cmd.Parameters.Add(new MySqlParameter("pTotalAP", time.TotalAp));
                        cmd.Parameters.Add(new MySqlParameter("pTotalMR", time.TotalMr));
                        cmd.Parameters.Add(new MySqlParameter("pMinute", time.Minute));
                        cmd.Parameters.Add(new MySqlParameter("pWinner", t.Winner));
                        cmd.Parameters.Add(new MySqlParameter("pCurrentGold", time.CurrentGold));
                        cmd.Parameters.Add(new MySqlParameter("pGoldSpent", time.GoldSpent));
                        cmd.Parameters.Add(new MySqlParameter("pItemsIds", string.Join("|", time.Items.Select(i=>i.Id))));
                        cmd.Parameters.Add(new MySqlParameter("pPatch", patch));
                        cmd.ExecuteNonQuery();
                    }
                    

                }
            }
        }

        public void SaveAvgTimeLine(AvgTimeLine av) {

            MySqlCommand cmd = new MySqlCommand("insert_avg_timeline", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new MySqlParameter("pIdChamp",av.ChampionId));
            cmd.Parameters.Add(new MySqlParameter("pRank", av.Rank));
            cmd.Parameters.Add(new MySqlParameter("pRole", av.Role));
            cmd.Parameters.Add(new MySqlParameter("pAvgAp", av.AvgAp));
            cmd.Parameters.Add(new MySqlParameter("pAvgMr", av.AvgMr));
            cmd.Parameters.Add(new MySqlParameter("pAvgGoldSpent", av.AvgGoldSpent));
            cmd.Parameters.Add(new MySqlParameter("pMostFrequentItems", av.Items));
            cmd.Parameters.Add(new MySqlParameter("pMinute", av.Minute));
            cmd.Parameters.Add(new MySqlParameter("pTotalGames", av.TotalGames));
            cmd.Parameters.Add(new MySqlParameter("pPatch", av.Patch));

            cmd.ExecuteNonQuery();
        }

        public IList<Match> GetMatches()
        {
            MySqlCommand cmd = new MySqlCommand("get_matches", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            var reader = cmd.ExecuteReader();
            IList<Match> matches = new List<Match>();
            while (reader.Read())
            {
                Match m = Match.Mapper(reader);
                matches.Add(m);
            }
            return matches;
        }   

        public IList<Champion> GetChampions() 
        {
            MySqlCommand cmd = new MySqlCommand("get_champions", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            var reader = cmd.ExecuteReader();
            IList<Champion> champions = new List<Champion>();
            while (reader.Read()) 
            {
                Champion c = Champion.Mapper(reader);
                champions.Add(c);
            }
            return champions;
        }

        public IList<Rune> GetRunes()
        {
            MySqlCommand cmd = new MySqlCommand("get_runes", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            var reader = cmd.ExecuteReader();
            IList<Rune> runes = new List<Rune>();
            while (reader.Read())
            {
                Rune r = Rune.Mapper(reader);
                runes.Add(r);
            }
            return runes;
        }

        public IList<Item> GetItems()
        {
            MySqlCommand cmd = new MySqlCommand("get_items", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            var reader = cmd.ExecuteReader();
            IList<Item> items = new List<Item>();
            while (reader.Read())
            {
                Item i = Item.Mapper(reader);
                items.Add(i);
            }
            return items;
        }

        public IList<AvgTimelineDto> GetDistinctAvgTimeline()
        { 
            MySqlCommand cmd = new MySqlCommand("get_idchamp_distinct_avgtimeline", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            var reader = cmd.ExecuteReader();
            IList<AvgTimelineDto> timeline = new List<AvgTimelineDto>();
            while (reader.Read())
            {
                AvgTimelineDto t = AvgTimelineDto.Mapper(reader);
                timeline.Add(t);
            }
            return timeline;
        }

        public IList<ApTimeLine> GetTimelinesBadItems()
        {
            MySqlCommand cmd = new MySqlCommand("get_timelines_baditems", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            var reader = cmd.ExecuteReader();
            IList<ApTimeLine> timeline = new List<ApTimeLine>();
            while (reader.Read())
            {
                ApTimeLine t = ApTimeLine.Mapper(reader);
                timeline.Add(t);
            }
            return timeline;
        }

        public void EditTimeline(ApTimeLine time)
        {
            MySqlCommand cmd = new MySqlCommand("edit_timeline", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new MySqlParameter("pMatchId", time.MatchId));
            cmd.Parameters.Add(new MySqlParameter("pParticipantId", time.PlayerId));
            cmd.Parameters.Add(new MySqlParameter("pTotalAP", time.TotalAp));
            cmd.Parameters.Add(new MySqlParameter("pMinute", time.Minute));
            var reader = cmd.ExecuteReader();
        }

        public IList<ApTimeLine> GetEndGameStatsBadItems()
        {
            MySqlCommand cmd = new MySqlCommand("get_endgamestats_baditems", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            var reader = cmd.ExecuteReader();
            IList<ApTimeLine> timeline = new List<ApTimeLine>();
            while (reader.Read())
            {
                ApTimeLine t = ApTimeLine.Mapper(reader);
                timeline.Add(t);
            }
            return timeline;
        }

        public IList<RunePage> GetRunePages() 
        {
            MySqlCommand cmd = new MySqlCommand("get_runepages", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            var reader = cmd.ExecuteReader();
            IList<RunePage> runepages = new List<RunePage>();
            while (reader.Read())
            {
                RunePage r = RunePage.Mapper(reader);
                runepages.Add(r);
            }
            return runepages;
        }

        public IList<ApTimeLine> GetTimelines() 
        {
             MySqlCommand cmd = new MySqlCommand("get_timelines", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            var reader = cmd.ExecuteReader();
            IList<ApTimeLine> timeline = new List<ApTimeLine>();
            while (reader.Read())
            {
                ApTimeLine t = ApTimeLine.Mapper(reader);
                timeline.Add(t);
            }
            return timeline;
        }

        public IList<AvgTimeLine> GetAvgTimelines()
        {
            MySqlCommand cmd = new MySqlCommand("get_avgtimeline", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            var reader = cmd.ExecuteReader();
            IList<AvgTimeLine> timeline = new List<AvgTimeLine>();
            while (reader.Read())
            {
                AvgTimeLine t = AvgTimeLine.Mapper(reader);
                timeline.Add(t);
            }
            return timeline;
        }

        public IList<ApTimeLine> GetTimelines(int idchamp)
        {
            MySqlCommand cmd = new MySqlCommand("get_timelines_pChampion", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new MySqlParameter("pIdChampion", idchamp));
            var reader = cmd.ExecuteReader();
            IList<ApTimeLine> timeline = new List<ApTimeLine>();
            while (reader.Read())
            {
                ApTimeLine t = ApTimeLine.Mapper(reader);
                timeline.Add(t);
            }
            return timeline;
        }

        public void Dispose()
        {
            connection.Close(); 
        }
    }
}
