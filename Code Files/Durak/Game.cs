using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

public class Game
{
    public Deck deck;
    public Player human;
    public ComputerPlayer cpu;

    public string TrumpSuit { get; private set; } = "";
    public bool PlayerAttack { get; private set; }  // true = human attacking
    public bool PlayerMove { get; private set; }    // whose move it is

    public int RoundAttacksCount { get; private set; } = 1;

    public List<Card> CurrentRoundAttacks { get; private set; } = new List<Card>();
    public List<Card> CurrentRoundDefends { get; private set; } = new List<Card>();
    public Card LastPlayedCard { get; private set; }

    public Game()
    {
        deck = new Deck();
        human = new Player();
        cpu = new ComputerPlayer();
    }

    public void StartGame()
    {
        deck.Initialize();
        deck.Shuffle(deck.Cards);

        // Deal 6 cards each
        for (int i = 0; i < 6; i++)
        {
            human.AddCard(deck.Draw());
            cpu.AddCard(deck.Draw());
        }

        TrumpSuit = deck.GetBottomCard().Suit;

        // Decide first attacker
        PlayerAttack = DetermineFirstAttacker();
        PlayerMove = PlayerAttack;
        if (!PlayerAttack)
        {
            CpuTurn();
        }
    }

    /// <summary>
    /// Play a card and remove it from the appropriate hand.
    /// </summary>
    public string Move(Card card, bool isPlayer = true)
    {
        if (card == null) return "";

        LastPlayedCard = card;

        bool isAttack = (PlayerMove && PlayerAttack) || (!PlayerMove && !PlayerAttack);

        if (isAttack)
        {
            if (CurrentRoundAttacks.Count >= 6)
                return ""; // block extra attacks

            CurrentRoundAttacks.Add(card);
        }
        else
        {
            CurrentRoundDefends.Add(card);
        }

        // Remove the card from the correct hand
        if (isPlayer)
            human.RemoveCard(card);
        else
            cpu.RemoveCard(card);

        // ✅ ADD THIS BLOCK RIGHT HERE
        if (!isAttack) // defender just played
        {
            if (CurrentRoundAttacks.Count == 6 &&
                CurrentRoundDefends.Count == 6)
            {
                // discard all cards
                foreach (var c in CurrentRoundAttacks)
                    deck.DiscardPile.Add(c);

                foreach (var c in CurrentRoundDefends)
                    deck.DiscardPile.Add(c);

                SwitchAttack();   // defender becomes attacker
                RefillHands();
                ResetRound();
            }
        }

        return CheckEndState();
    }

    /// <summary>
    /// Switch the attack role (attacker <-> defender)
    /// </summary>
    public void SwitchAttack()
    {
        PlayerAttack = !PlayerAttack;
        ResetRound();
   
    }

    public void PlayerPasses()
    {
        if (!PlayerAttack)
        {
            // defender picks up
            foreach (var card in CurrentRoundAttacks)
                human.AddCard(card);

            foreach (var card in CurrentRoundDefends)
                human.AddCard(card);
        }
        else
        {
            // successful defense, discard
            foreach (var card in CurrentRoundAttacks)
                deck.DiscardPile.Add(card);

            foreach (var card in CurrentRoundDefends)
                deck.DiscardPile.Add(card);

            SwitchAttack();
        }

        RefillHands();
        ResetRound();
    }

    private void DrawUpToSix(Player player)
    {
        while (player.hand.Count < 6 && deck.Cards.Count > 0)
        {
            player.AddCard(deck.Draw());
        }
    }

    public void RefillHands()
    {
        if (PlayerAttack) // human is attacker
        {
            DrawUpToSix(human);
            DrawUpToSix(cpu);
        }
        else // CPU is attacker
        {
            DrawUpToSix(cpu);
            DrawUpToSix(human);
        }
    }

    /// <summary>
    /// Switch whose move it is
    /// </summary>
    public void SwitchMove()
    {
        PlayerMove = !PlayerMove;
    }

    /// <summary>
    /// Check if the game is over
    /// </summary>
    public string CheckEndState()
    {
        if (human.hand.Count == 0) return "You won!";
        if (cpu.hand.Count == 0) return "CPU won!";
        return ""; // ongoing
    }

    /// <summary>
    /// CPU performs its turn (attack or defend)
    /// </summary>
    public void CpuTurn()
    {
        bool isCpuAttack = !PlayerAttack; // if player is attacking, CPU defends

        Card cpuCard = cpu.MakeMove(isCpuAttack, CurrentRoundAttacks, LastPlayedCard, TrumpSuit);

        if (cpuCard != null)
        {
            var result = Move(cpuCard, isPlayer: false);

            if (result != "")
            {
                MessageBox.Show(result);
                return;
            }
        }
        else
        {
            if (!isCpuAttack)
            {
                // CPU cannot defend, must pick up cards
                CpuPickupCards();
                ResetRound();
                RefillHands();
            }
            // If CPU is attacking and has no valid card, it just passes
            else
            {
                foreach (var card in CurrentRoundAttacks)
                {
                    deck.DiscardPile.Add(card);
                }
                foreach (var card in CurrentRoundDefends)
                {
                    deck.DiscardPile.Add(card);
                }
                SwitchAttack();
                RefillHands();
            }
        }

        SwitchMove();
    }

    /// <summary>
    /// CPU picks up all cards from the current round
    /// </summary>
    private void CpuPickupCards()
    {
        foreach (var card in CurrentRoundAttacks)
        {
            cpu.AddCard(card);
        }
        foreach (var card in CurrentRoundDefends)
        {
            cpu.AddCard(card);
        }

    }

    public void ResetRound()
    {
        CurrentRoundAttacks.Clear();
        CurrentRoundDefends.Clear();
        LastPlayedCard = null;
    }

    private bool DetermineFirstAttacker()
    {
        Card playerTrump = null;
        Card cpuTrump = null;

        // Find lowest trump in player's hand
        foreach (var card in human.hand)
        {
            if (card.Suit == TrumpSuit)
            {
                if (playerTrump == null || card.Rank < playerTrump.Rank)
                {
                    playerTrump = card;
                }
            }
        }

        // Find lowest trump in CPU's hand
        foreach (var card in cpu.hand)
        {
            if (card.Suit == TrumpSuit)
            {
                if (cpuTrump == null || card.Rank < cpuTrump.Rank)
                {
                    cpuTrump = card;
                }
            }
        }

        // Decide first attacker
        if (playerTrump == null && cpuTrump == null) return true;  // default: player starts
        if (cpuTrump == null) return true;
        if (playerTrump == null) return false;
        return playerTrump.Rank < cpuTrump.Rank;
    }
}