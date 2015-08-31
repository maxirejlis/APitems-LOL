using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Data.Entidades.Caches;
using MySql.Data.MySqlClient;

namespace Data.Entidades
{
    /// <summary>
    /// Match class containing Riot's API match data.
    /// </summary>
    public class Match
    {
        public int Id { get; set; }
        public string Region { get; set; }
        public QueueType QueueType { get; set; }
        public TimeSpan Duration { get; set; }
        public int MapId { get; set; }
        public string Patch { get; set; }
        public IList<Team> Teams { get; set; }

        public Match() { }
        public Match(dynamic o)
        {
            try
            {
                Id = o.matchId;
                Region = o.region;
                QueueType = EnumHelpers.ParseQueueType(o.queueType.Value);
                if (QueueType != Entidades.QueueType.RANKED_SOLO_5x5)
                    return;
                Duration = (TimeSpan.FromSeconds(o.matchDuration.Value));
                MapId = o.mapId;
                Patch = o.matchVersion;
                Teams = new List<Team>();
                foreach (var team in o.teams)
                {
                    IList<Player> players = new List<Player>();
                    foreach (var player in o.participants)
                    {
                        if (player.teamId == team.teamId)
                        {
                            players.Add(new Player(player,o.timeline, Patch));
                        }
                        
                    }
                    Teams.Add(new Team(team, players));
                }
            }
            catch
            {
                throw;
            }
        }
        public static Match Mapper(MySqlDataReader reader)
        {
            var m = new Match();
            m.Id = reader.GetInt32("Id");
            return m;
        }
    }

    /// <summary>
    /// Finalstats class containing the final stats of a match provided in Riot's API match data.
    /// </summary>
    public class FinalStats 
    {
        public int MagicDamageDealtToChampions { get; set; }
        public int PhysicalDamageDealtToChampions { get; set; }
        public int TrueDamageDealtToChampions { get; set; }
        public int TotalAp { get; set; }
        public int TotalMr { get; set; }
        public List<Item> Items { get; set; }

        public FinalStats() { }
        public FinalStats(dynamic o, string patch)
        {
            MagicDamageDealtToChampions = (int)o.magicDamageDealtToChampions.Value;
            PhysicalDamageDealtToChampions = (int)o.physicalDamageDealtToChampions.Value;
            TrueDamageDealtToChampions = (int)o.trueDamageDealtToChampions.Value;
            Items = new List<Item>();
            Item i = null;
            if (patch.Contains("5.11"))
            {
                i = ItemsCache.GetItemV511((int)o.item0.Value);
                if (i != null)
                    Items.Add(i);
                i = ItemsCache.GetItemV511((int)o.item1.Value);
                if (i != null)
                    Items.Add(i);
                i = ItemsCache.GetItemV511((int)o.item2.Value);
                if (i != null)
                    Items.Add(i);
                i = ItemsCache.GetItemV511((int)o.item3.Value);
                if (i != null)
                    Items.Add(i);
                i = ItemsCache.GetItemV511((int)o.item4.Value);
                if (i != null)
                    Items.Add(i);
                i = ItemsCache.GetItemV511((int)o.item5.Value);
                if (i != null)
                    Items.Add(i);
                i = ItemsCache.GetItemV511((int)o.item6.Value);
                if (i != null)
                    Items.Add(i);
            }
            else
            {
                i = ItemsCache.GetItemV514((int)o.item0.Value);
                if (i != null)
                    Items.Add(i);
                i = ItemsCache.GetItemV514((int)o.item1.Value);
                if (i != null)
                    Items.Add(i);
                i = ItemsCache.GetItemV514((int)o.item2.Value);
                if (i != null)
                    Items.Add(i);
                i = ItemsCache.GetItemV514((int)o.item3.Value);
                if (i != null)
                    Items.Add(i);
                i = ItemsCache.GetItemV514((int)o.item4.Value);
                if (i != null)
                    Items.Add(i);
                i = ItemsCache.GetItemV514((int)o.item5.Value);
                if (i != null)
                    Items.Add(i);
                i = ItemsCache.GetItemV514((int)o.item6.Value);
                if (i != null)
                    Items.Add(i);
            }
        }
    }

    /// <summary>
    /// Player class containing data of a match provided in Riot's API match data.
    /// </summary>
    public class Player
    {
        public int ParticipantId { get; set; }
        public List<SummonerSpell> Spells { get; set; }
        public Champion SelectedChampion { get; set; }
        public string Rank { get; set; }
        public List<Mastery> Masteries { get; set; }
        public FinalStats Finalstats { get; set; }
        public List<Rune> Runes { get; set; }
        public Summoner SummonerData { get; set; }
        public PlayerTimeLine PlayerTimeLine { get; set; }
        public IList<ItemTimeLineFrame> ItemTimeLine { get; set; }
        public IList<TimeLineFrame> TimeLine { get; set; }
        public Role Role { get; set; }
        public Lane Lane { get; set; }

        public Player() { }
        public Player(dynamic o, dynamic timeline, string patch)
        {
            ParticipantId = o.participantId;
            SelectedChampion = Caches.ChampionsCache.Get((int)o.championId.Value);
            Rank = (string)o.highestAchievedSeasonTier.Value;
            Runes = new List<Rune>();
            if (o.runes != null)
            {
                foreach (var rune in o.runes)
                {
                    for (int i = 0; i < (int)rune.rank.Value; i++)
                        Runes.Add(Caches.RunesCache.Get((int)rune.runeId.Value));
                }
            }
            Role = EnumHelpers.ParseRole((string)o.timeline.role);
            Lane = EnumHelpers.ParseLane((string)o.timeline.lane);
            Finalstats = new FinalStats(o.stats, patch);
            Finalstats.TotalMr = (int)(Finalstats.Items.Sum(item => item.ItemStats.MR) + SelectedChampion.Stat.MagicResist + SelectedChampion.Stat.MagicResistPerLv * (int)o.stats.champLevel.Value + Runes.Sum(r => r.MagicResist) + Runes.Sum(r => r.MagicResistPerLevel) * (int)o.stats.champLevel.Value);
            Finalstats.TotalAp = (int)(Finalstats.Items.Sum(item => item.ItemStats.AP) + Runes.Sum(r => r.Ap) + Runes.Sum(r => r.ApPerLevel) * (int)o.stats.champLevel.Value);
            if (Finalstats.Items.Any(i => i.Name == "Rabadon's Deathcap"))
            {
                if (patch.StartsWith("5.11"))
                    Finalstats.TotalAp = (int)(Finalstats.TotalAp * 1.30);
                else
                    Finalstats.TotalAp = (int)(Finalstats.TotalAp * 1.35);
            }
            TimeLine = new List<TimeLineFrame>();
            ItemTimeLine = new List<ItemTimeLineFrame>();

            if (timeline.frame != null)
            {
                foreach (var frame in timeline.frames)
                {
                    try
                    {
                        foreach (var participantFrame in frame.participantFrames)
                        {
                            try
                            {
                                if (participantFrame.Value.participantId == ParticipantId)
                                {
                                    TimeLineFrame t = new TimeLineFrame(participantFrame.Value, (int)frame.timestamp.Value);
                                    TimeLine.Add(t);
                                    break;
                                }
                            }
                            catch { }
                        }
                    }
                    catch { }
                    try
                    {
                        foreach (var eventframe in frame.events)
                        {
                            try
                            {
                                if (eventframe.participantId != null)
                                {
                                    if (eventframe.participantId.Value == ParticipantId)
                                    {
                                        string s = eventframe.eventType.Value;
                                        if (s.Contains("ITEM"))
                                        {
                                            ItemTimeLineFrame t = new ItemTimeLineFrame(eventframe, (int)frame.timestamp.Value);
                                            ItemTimeLine.Add(t);
                                        }
                                    }
                                }
                            }
                            catch
                            {

                            }
                        }
                    }
                    catch { }
                }
            }
        }
    }

    /// <summary>
    /// Team class containing data of a match provided in Riot's API match data.
    /// </summary>
    public class Team
    {
        public int TeamId { get; set; }
        public IList<Player> Players { get; set; }
        public bool Winner { get; set; }
        public bool FirstBlood { get; set; }
        public bool FirstDragon { get; set; }
        public bool FirstBaron { get; set; }
        public bool FirstTower { get; set; }
        public bool FirstInhibitor { get; set; }
        public int TotalTowerKills { get; set; }
        public int TotalInhibitorKills { get; set; }
        public int TotalBaronKills { get; set; }
        public int TotalDragonKills { get; set; }
        public int TotalVilemawKills { get; set; }
        public int DominionVictoryScore { get; set; }
        public List<Champion> Bans { get; set; }

        public Team()
        {
        }

        public Team(dynamic o, IList<Player> players)
        {
            TeamId = (int)o.teamId.Value;
            Winner = o.winner.Value;
            FirstBlood = o.firstBlood.Value;
            FirstTower = o.firstTower.Value;
            FirstInhibitor = o.firstInhibitor.Value;
            FirstBaron = o.firstBaron.Value;
            FirstDragon = o.firstDragon.Value;
            TotalTowerKills = (int)o.towerKills.Value;
            TotalInhibitorKills = (int)o.inhibitorKills.Value;
            TotalDragonKills = (int)o.dragonKills.Value;
            TotalBaronKills = (int)o.baronKills.Value;
            TotalVilemawKills = (int)o.vilemawKills.Value;
            DominionVictoryScore = (int)o.dominionVictoryScore;
            Bans = new List<Champion>();
            foreach (var ob in o.bans)
            {
                Bans.Add(Caches.ChampionsCache.Get((int)ob.championId.Value));
            }
            Players = players;
        }
    }

    /// <summary>
    /// Delta class containing data of a timeline provided in Riot's API match data.
    /// </summary>
    public class Delta {
        double ZeroToTen { get; set; }
        double TenToTwenty { get; set; }
        double TwentyToThirty { get; set; }
        double ThirtyToEnd { get; set; }

        public Delta(dynamic o) 
        {
            ZeroToTen = o.zeroToTen;
            TenToTwenty = o.tenToTwenty;
            TwentyToThirty = o.twentyToThirty;
            ThirtyToEnd = o.thirtyToEnd;
        }
    }

    /// <summary>
    /// Player TimeLine class containing data of a player provided in Riot's API match data.
    /// </summary>
    public class PlayerTimeLine
    {
        public Delta CreepsPerMinDeltas { get; set; }
        public Delta XpPerMinDeltas { get; set; }
        public Delta GoldPerMinDeltas { get; set; }
        public Delta CsDiffPerMinDeltas { get; set; }
        public Delta XpDiffPerMinDeltas { get; set; }
        public Delta DamageTakenPerMinDeltas { get; set; }
        public Delta DamageTakenDiffPerMinDeltas { get; set; }

        public PlayerTimeLine() { }
        public PlayerTimeLine(dynamic o)
        {
            CreepsPerMinDeltas = new Delta(o.creepsPerMinDeltas);
            XpPerMinDeltas = new Delta(o.xpPerMinDeltas);
            GoldPerMinDeltas = new Delta(o.goldPerMinDeltas);
            CsDiffPerMinDeltas = new Delta(o.csDiffPerMinDeltas);
            XpDiffPerMinDeltas = new Delta(o.xpDiffPerMinDeltas);
            DamageTakenPerMinDeltas = new Delta(o.damageTakenPerMinDeltas);
            DamageTakenDiffPerMinDeltas = new Delta(o.damageTakenDiffPerMinDeltas);
        }
    }

    /// <summary>
    /// ItemTimeLine class containing data of a player in a determined minute, provided in Riot's API match data.
    /// </summary>
    public class ItemTimeLineFrame 
    {
        public int TimeStamp { get; set; }
        public string EventType { get; set; }
        public int ItemId { get; set; }
        public int ParticipantId { get; set; }

        public ItemTimeLineFrame() { }

        public ItemTimeLineFrame(dynamic o, int timestamp) 
        {
            TimeStamp = timestamp;
            EventType = o.eventType;
            if (o.itemId != null)
                ItemId = (int)o.itemId;                
            else if (o.itemBefore != null)
                ItemId = (int)o.itemBefore;
            else if (o.itemAfter != null)
                ItemId = (int)o.itemAfter;
            ParticipantId = (int)o.participantId;
        }
    }

    /// <summary>
    /// TimeLine class containing data of a player provided in Riot's API match data.
    /// </summary>
    public class TimeLineFrame
    {
        public int Timestamp { get; set; }
        public int ParticipantId { get; set; }
        public Tuple<int,int> Position { get; set; } // X,Y
        public int CurrentGold { get; set; }
        public int TotalGold { get; set; }
        public int Level { get; set; }
        public int Xp { get; set; }
        public int MinionsKilled { get; set; }
        public int JungleMinionsKilled { get; set; }
        public int DominionScore { get; set; }
        public int TeamScore { get; set; }
        
        public TimeLineFrame() { }

        public TimeLineFrame(dynamic o,int timestamp)
        {
            Timestamp = timestamp;
            ParticipantId = o.participantId;
            if (o.position != null)
                Position = new Tuple<int,int>((int)o.position.x.Value, (int)o.position.y.Value);
            CurrentGold = o.currentGold;
            TotalGold = o.totalGold;
            Level = o.level;
            Xp = o.xp;
            MinionsKilled = o.minionsKilled;
            JungleMinionsKilled = o.jungleMinionsKilled;
            DominionScore = o.dominionScore;
            TeamScore = o.teamScore;
        }
    }
}