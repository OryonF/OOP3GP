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
}
