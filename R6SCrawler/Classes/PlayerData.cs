using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R6SCrawler
{
    class PlayerData
    {
        public string RankedTimePlayed        { get; set; }
        public int    RankedKills             { get; set; }
        public int    RankedDeaths            { get; set; }
        public int    RankedWins              { get; set; }
        public int    RankedLosses            { get; set; }
        public string CasualTimePlayed        { get; set; }
        public int    CasualKills             { get; set; }
        public int    CasualDeaths            { get; set; }
        public int    CasualWins              { get; set; }
        public int    CasualLosses            { get; set; }
        public string TerroristHuntTimePlayed { get; set; }
    }
}
