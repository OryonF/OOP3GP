/*
 * Author: Lukas Cadman
 * Date: March 25th, 2026
 * Description: This contains the logic for the AI player.
 */
using System;
using System.Windows;

public class ComputerPlayer : HumanPlayer 
{

	public ComputerPlayer(string name) : base(name)
	{
	}


    // Computer Player attack logic
    public Card? ChooseAttackCard(List<Card> tableCards)
    {
        if (tableCards.Count == 0)
        {
            return Hand
                .OrderBy(c => c.Suit)
                .ThenBy(c => c.Rank)
                .FirstOrDefault();
        }

        // Follow up attack logic
        var validRanks = tableCards
            .Select(c => c.Rank)
            .Distinct()
            .ToList();
        return Hand
            .Where(c => validRanks.Contains(c.Rank))
            .OrderBy(c => c.Suit)
            .ThenBy(c => c.Rank)
            .FirstOrDefault();
    }



}
