using System;

namespace Durak
{
    public class DurakDBModel
    {
        public int Id { get; set; }         
        public DateTime StartTime { get; set; }
        public string TrumpSuit { get; set; }
        public string StartingAttacker { get; set; }
        public string Winner { get; set; }
    }
}