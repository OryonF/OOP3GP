using System;
using System.Collections.Generic;

public class Deck
{
    public List<Card> Cards { get; private set; } = new List<Card>();
    public List<Card> DiscardPile { get; private set; } = new List<Card>();

    private static readonly string[] Suits = { "Hearts", "Diamonds", "Clubs", "Spades" };

    // Call this at the start of each game to reset the deck
    public void Initialize()
    {
        Cards.Clear();
        foreach (string suit in Suits)
        {
            for (int rank = 6; rank <= 14; rank++) // 6 to Ace (14)
            {
                Cards.Add(new Card(rank, suit));
            }
        }
    }

    // Shuffle the deck using Fisher-Yates algorithm
    public void Shuffle(List<Card> cards)
    {
        Random rng = new Random();
        int n = cards.Count;
        for (int i = n - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            var temp = cards[i];
            cards[i] = cards[j];
            cards[j] = temp;
        }
    }

    // Draw a card from the top of the deck
    public Card Draw()
    {
        if (Cards.Count == 0) return null;
        Card top = Cards[0];
        Cards.RemoveAt(0);
        return top;
    }

    // Add a card to the discard pile
    public Card GetBottomCard()
    {
        if (Cards.Count == 0) return null;
        return Cards[Cards.Count - 1];
    }
}