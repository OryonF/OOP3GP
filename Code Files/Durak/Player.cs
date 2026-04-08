using System.Collections.Generic;

public class Player
{
    public List<Card> hand { get; private set; } = new List<Card>();

    public void AddCard(Card card)
    {
        if (card != null) hand.Add(card);
    }

    public void RemoveCard(Card card)
    {
        if (card != null) hand.Remove(card);
    }

    public void Pass(Game game)
    {
        game.SwitchAttack();
    }
}