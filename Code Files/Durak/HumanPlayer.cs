using System;

public class HumanPlayer
{

    // Player Name
    public string Name { get; set; }

    // Cards in the Player's Hand
    public List<Card> Hand { get; set; }

    // Detect if the player is currently attacking
    public bool IsAttacking { get; set; }

    public HumanPlayer(string name)
    {
        Name = name;
        Hand = new List<Card>();

    }
    
    // 
    public void AddCard(Card card)
    {
        Hand.Add(card);
    }

    // 
    public void RemoveCard(Card card)
    {
        Hand.Remove(card);
    }

    public void SortHand()
    {
        Hand = Hand
            .OrderBy(c => c.Suit)
            .ThenBy(c => c.Rank)
            .ToList();
    }
}
