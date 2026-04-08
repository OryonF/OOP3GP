using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durak
{
    public partial class StatsSettingsData
    {
        public int Wins { get; set; } = 0;
        public int Losses { get; set; } = 0;
        public int TotalGames { get; set; } = 0;
        public string SelectedCardTheme { get; set; } = "n"; // default theme
        public string PlayerName { get; set; } = "Joker";

        public int CurrentStreak { get; set; } = 0;
        public int LongestWinStreak { get; set; } = 0;
        public int LongestLossStreak { get; set; } = 0;


    }


}
