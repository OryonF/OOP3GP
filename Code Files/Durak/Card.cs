using System;

namespace Durak
{
    public class Card
    {
        // Setters and getters
        public string Suit { get; set; }
        public string Rank { get; set; }

        // Constructor
        public Card() { }

        // Parameterized constructor
        public Card(string rank, string suit)
        {
            Rank = rank;
            Suit = suit;
        }

        // Method for displaying card information
        public override string ToString()
        {
            return Rank + " of " + Suit;
        }
    }
}
