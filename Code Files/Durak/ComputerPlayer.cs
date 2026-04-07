public class ComputerPlayer : Player
{
    private Random rand = new Random();

    // Call this when it's the CPU's turn to make a move
    public Card MakeMove(bool isAttack, List<Card> currentRoundAttacks, List<Card> currentRoundDefends, Card lastPlayedCard, string trumpSuit)
    {
        // Starting attack: choose a random card
        if (isAttack && currentRoundAttacks.Count == 0)
        {
            int index = rand.Next(hand.Count);
            Card chosen = hand[index];
            return chosen;
        }

        // Additional attacks: must match values on current round
        if (isAttack && currentRoundAttacks.Count > 0)
        {
            List<Card> possible = new List<Card>();
            foreach (var c in hand)
            {
                if (Card.CanAdditionalAttack(c, currentRoundAttacks,currentRoundDefends))
                {
                    possible.Add(c);
                }
            }

            if (possible.Count == 0) return null; // no valid additional attack
            int index = rand.Next(possible.Count);
            Card chosen = possible[index];
            return chosen;
        }

        // Defend
        if (!isAttack)
        {
            List<Card> possible = new List<Card>();
            foreach (var c in hand)
            {
                if (Card.CanDefend(lastPlayedCard, c, trumpSuit))
                {
                    possible.Add(c);
                }
            }

            if (possible.Count == 0) return null; // cannot defend, must pick up
            int index = rand.Next(possible.Count);
            Card chosen = possible[index];
            return chosen;
        }

        return null; // fallback
    }
}