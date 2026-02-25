using System;
using System.Collections.Generic;

namespace Durak
{
    public class Deck
    {
        // Instance variable
        protected List<Card> cards;

        // Constructor
        public Deck()
        {
            cards = new List<Card>();
        }

        // Deck method to be overridden by subclasses
        public virtual void InitializeDeck() { }

        // Function to handle shuffling cards in deck
        public void Shuffle()
        {
            // Get a random number
            Random rnd = new Random();
            // Fisher-Yates shuffle (start at final index and move backwards by swapping current index item with a random lower index)
            for (int i = cards.Count - 1; i > 0; i--)
            {
                int j = rnd.Next(i + 1);
                Card temp = cards[i];
                cards[i] = cards[j];
                cards[j] = temp;
            }
        }

        // Function for showing all cards (rank and suite) in the deck
        public List<Card> GetCards()
        {
            return cards;
        }

        // Function for showing the number of cards still in the deck
        public int Count()
        {
            return cards.Count;
        }

        /*
         * Function for handling card dealing
         * Take int as parameter for number of cards to deal
         */
        public List<Card> Deal(int count)
        {

            // Exception handle for negative or 0 int, or asking for more cards than are in the deck
            if (count <= 0 || count > cards.Count)
                throw new ArgumentOutOfRangeException("count", "Invalid number of cards to deal.");

            // Removes cards from the top of the deck
            List<Card> dealtCards = cards.GetRange(0, count);
            cards.RemoveRange(0, count);
            return dealtCards;
        }
    }
}
