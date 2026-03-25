using System;

public class ComputerPlayer 
{
	public List<Card> Hand {get; set;}
	public string Name { get; set;}

	public ComputerPlayer(string name)
	{
		Name = name;
		Hand = new List<Card>();
	}

	public void AddCard(Card card)
	{
		Hand.Add(card);
	}

	public void RemoveCard(Card card)
	{
		Hand.Remove(card);
	}
}
