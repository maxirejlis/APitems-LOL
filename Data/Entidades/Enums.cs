using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Data.Entidades
{
    public static class EnumHelpers
    {

        public static Rank ParseRank(string s)
        {
            switch (s.ToUpper()) 
            {
                case "BRONZE":
                    return Rank.Bronze;
                case "SILVER":
                    return Rank.Silver;
                case "GOLD":
                    return Rank.Gold;
                case "PLATINUM":
                    return Rank.Platinum;
                case "DIAMOND":
                    return Rank.Diamond;
                case "MASTER":
                    return Rank.Master;
                case "CHALLENGER":
                    return Rank.Challenger;
            }
            return Rank.Bronze;
        }
        public static Role ParseRole(string s)
        {
            switch (s.ToUpper())
            {
                case "JUNGLE":
                    return Role.Jungle;
                case "DUO_SUPPORT":
                    return Role.Support;
                case "DUO_CARRY":
                    return Role.ADC;
                case "SOLO":
                    return Role.Solo;
                case "NONE":
                    return Role.None;
            }
            return Role.None;
        }
        public static Role GetRealRoles(string role, string lane)
        {
            switch (role.ToUpper())
            {
                case "JUNGLE":
                    return Role.Jungle;
                case "SUPPORT":
                    return Role.Support;
                case "ADC":
                    return Role.ADC;
                case "SOLO":
                    switch (lane.ToUpper()){
                        case "TOP":
                            return Role.Top;
                        case "MID":
                            return Role.Mid;
                        case "BOTTOM":
                            return Role.Support;
                    }
                    return Role.Solo;
                case "NONE":
                    switch (lane.ToUpper()){
                        case "JUNGLE":
                            return Role.Jungle;
                        default:
                            return Role.None;
                    }
            }
            return Role.None;
        }

        public static Lane ParseLane(string s)
        {
            switch (s.ToUpper())
            {
                case "JUNGLE":
                case "NONE":
                    return Lane.Jungle;
                case "BOTTOM":
                    return Lane.Bottom;
                case "TOP":
                    return Lane.Top;
                case "MIDDLE":
                    return Lane.Mid;
            }
            return Lane.Unknown;
        }
        public static QueueType ParseQueueType(string s) 
        {
            switch (s)
            {
                case "RANKED_PREMADE_5x5":
                case "RANKED_SOLO_5x5":
                    return QueueType.RANKED_SOLO_5x5;
                default: return QueueType.URF_5x5;
            }
        }
    }

    public enum Rank
    {
        Bronze,
        Silver,
        Gold, 
        Platinum,
        Diamond,
        Master,
        Challenger
    }

    public enum Lane
    {
        Bottom,
        Mid,
        Top,
        Jungle,
        Unknown

    }
    public enum Role { 
        ADC,
        Support,
        Mid,
        Jungle,
        Top,
        Solo,
        None

    }
    public enum ClassType
    {
        Fighter = 1,
        Mage = 2,
        Tank = 3,
        Marksman = 4,
        Assassin = 5,
    }

    public enum ResourceType
    {
        Unidentified = 0,
        Mana = 1,
        Fury = 5,
        Rage = 2,
        Health = 3,
        NoCost = 4,
        Energy = 6,
    }

    public enum QueueType
    {
        CUSTOM = 0,//Custom games
        NORMAL_3x3 = 8,//Normal 3v3 games
        NORMAL_5x5_BLIND = 2,//Normal 5v5 Blind Pick games
        NORMAL_5x5_DRAFT = 14,//Normal 5v5 Draft Pick games
        RANKED_SOLO_5x5 = 4,//Ranked Solo 5v5 games
        RANKED_PREMADE_5x5 = 6,//	Ranked Premade 5v5 games
        RANKED_PREMADE_3x3 = 9,//	Ranked Premade 3v3 games
        RANKED_TEAM_3x3 = 41,//Ranked Team 3v3 games
        RANKED_TEAM_5x5 = 42,//Ranked Team 5v5 games
        ODIN_5x5_BLIND = 16,//Dominion 5v5 Blind Pick games
        ODIN_5x5_DRAFT = 17,//Dominion 5v5 Draft Pick games
        BOT_5x5 = 7,//	Historical Summoner's Rift Coop vs AI games
        BOT_ODIN_5x5 = 25,//Dominion Coop vs AI games
        BOT_5x5_INTRO = 31,//Summoner's Rift Coop vs AI Intro Bot games
        BOT_5x5_BEGINNER = 32,//Summoner's Rift Coop vs AI Beginner Bot games
        BOT_5x5_INTERMEDIATE = 33,//Historical Summoner's Rift Coop vs AI Intermediate Bot games
        BOT_TT_3x3 = 52,//Twisted Treeline Coop vs AI games
        GROUP_FINDER_5x5 = 61,//Team Builder games
        ARAM_5x5 = 65,//ARAM games
        ONEFORALL_5x5 = 70,//One for All games
        FIRSTBLOOD_1x1 = 72,//Snowdown Showdown 1v1 games
        FIRSTBLOOD_2x2 = 73,//Snowdown Showdown 2v2 games
        SR_6x6 = 75,//Summoner's Rift 6x6 Hexakill games
        URF_5x5 = 76,//Ultra Rapid Fire games
        BOT_URF_5x5 = 83,//Ultra Rapid Fire games played against AI games
        NIGHTMARE_BOT_5x5_RANK1 = 91,//Doom Bots Rank 1 games
        NIGHTMARE_BOT_5x5_RANK2 = 92,//Doom Bots Rank 2 games
        NIGHTMARE_BOT_5x5_RANK5 = 93,//Doom Bots Rank 5 games
        ASCENSION_5x5 = 96,//Ascension games
        HEXAKILL = 98,//Twisted Treeline 6x6 Hexakill games
        BILGEWATER_ARAM_5x5 = 100,//Butcher's Bridge games
        KING_PORO_5x5 = 300,//King Poro games
        COUNTER_PICK = 310,//Nemesis games
        BILGEWATER_5x5 = 313,//Black Market Brawlers games
    }
    //public enum Region { 
    //    BR,
    //    EUNE,
    //    EUW,
    //    KR,
    //    LAN,
    //    LAS,
    //    NA,
    //    OCE,
    //    RU,
    //    TR
    //}

}
