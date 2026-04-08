using System;

public class Card
{
    public string Suit { get; set; }
    public int Rank { get; set; }

    public Card(int rank, string suit)
    {
        Rank = rank;
        Suit = suit;
    }

    //Compares two cards to check if defendCard can be used against attackCard
    //Use to iterate through hand and disable WPF 'Use Selected Card' button when player is defending
    public static bool CanDefend(Card attackCard, Card defendCard, string trumpSuit)
    {
        // Ordinary defense rules (attack card was not a trump suit)
        if (attackCard.Suit != trumpSuit)
        {
            if (defendCard.Suit == attackCard.Suit)
            {
                return defendCard.Rank > attackCard.Rank;
            }
            else if (defendCard.Suit == trumpSuit)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        // Defense rules if attack card was trump
        else
        {
            if (defendCard.Suit == trumpSuit)
            {
                return defendCard.Rank > attackCard.Rank;
            }
            else
            {
                return false;
            }
        }
    }

    //Compares card against previously used attack cards in given round to determine if attackCard can be used for follow up attacks
    //Use to iterate through hand and disable WPF 'Use Selected Card' button when player is attacking

    public static bool CanAdditionalAttack(Card card, List<Card> attackCards, List<Card> defendCards)
    {
        // Check against attack cards
        foreach (var c in attackCards)
        {
            if (card.Rank == c.Rank)
                return true;
        }

        // Check against defense cards
        foreach (var c in defendCards)
        {
            if (card.Rank == c.Rank)
                return true;
        }

        return false;
    }

    public override string ToString()
    {
        string rankText = Rank switch
        {
            11 => "Jack",
            12 => "Queen",
            13 => "King",
            14 => "Ace",
            _ => Rank.ToString()
        };

        return $"{rankText} of {Suit}";
    }
}