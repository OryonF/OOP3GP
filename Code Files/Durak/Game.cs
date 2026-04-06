using Durak;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

public class Game
{

    public Deck deck;
    public Player human;
    public ComputerPlayer cpu;

    public string CardThemePrefix { get; set; } = "n"; // default = normal
    public string TrumpSuit { get; private set; } = "";
    public bool PlayerAttack { get; private set; }  // true = human attacking
    public bool PlayerMove { get; private set; }    // whose move it is

    public List<Card> CurrentRoundAttacks { get; private set; } = new List<Card>();
    public List<Card> CurrentRoundDefends { get; private set; } = new List<Card>();
    public Card? LastPlayedCard { get; private set; }

    private StatsSettingsData stats;
    //private GameLog currentlog;(Will be implemented later)

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

        for (int i = 0; i < 6; i++)
        {
            human.AddCard(deck.Draw());
            cpu.AddCard(deck.Draw());
        }

        TrumpSuit = deck.GetBottomCard().Suit;

        //JSON
        stats = StatsSettingsManager.Load();
        CardThemePrefix = stats.SelectedCardTheme;


        // Initialize game log
        //currentlog = new Gamelog();
        //currentlog.TrumpSuit = TrumpSuit;

        // First Attacker
        PlayerAttack = false; // currently set to cpu always attacking first for testing; replace with DetermineFirstAttacker(); in final
        PlayerMove = PlayerAttack;
    }

    public string Move(Card card, bool isPlayer = true, bool allowCpuResponse = true)
    {
        if (card == null) return "";

        LastPlayedCard = card;

        /* Log move
        string moveText = isPlayer
            ? $"Player played {card.Rank} of {card.Suit}."
            : $"CPU played {card.Rank} of {card.Suit}.";

        currentlog.Moves.Add(moveText); */

        bool isAttack = (PlayerMove && PlayerAttack) || (!PlayerMove && !PlayerAttack);

        if (isAttack)
        {
            if (CurrentRoundAttacks.Count >= 6)
                return "";

            CurrentRoundAttacks.Add(card);
        }
        else
        {
            CurrentRoundDefends.Add(card);
        }

        if (isPlayer)
        {
            human.RemoveCard(card);
        }
        else
        { cpu.RemoveCard(card); }
        SwitchMove();


        // Max 6 resolved → end round
        if (!isAttack &&
            CurrentRoundAttacks.Count == 6 &&
            CurrentRoundDefends.Count == 6)
        {
            foreach (var c in CurrentRoundAttacks)
                deck.DiscardPile.Add(c);

            foreach (var c in CurrentRoundDefends)
                deck.DiscardPile.Add(c);

            SwitchAttack();
            ResetRound();
            RefillHands();

            PlayerMove = PlayerAttack;

            if (!PlayerMove && allowCpuResponse)
                CpuTurn();
        }

        string result = CheckEndState();

        if (result != "")
            EndGame(result);
        

        return result;
    }

        private void EndGame(string result)
    {
        stats.TotalGames++;

        if (result.Contains("You won"))
        {
            stats.Wins++;
        }
        else
        {
            stats.Losses++;
        }

        // Save current card theme
        stats.SelectedCardTheme = CardThemePrefix;

        // Write to JSON file
        StatsSettingsManager.Save(stats);
    }


    public void SwitchAttack()
    {
        PlayerAttack = !PlayerAttack;
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
            // successful defense
            foreach (var card in CurrentRoundAttacks)
                deck.DiscardPile.Add(card);

            foreach (var card in CurrentRoundDefends)
                deck.DiscardPile.Add(card);

            SwitchAttack();
        }

        ResetRound();
        RefillHands();

        PlayerMove = PlayerAttack;

        if (!PlayerMove)
            CpuTurn();
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
        if (PlayerAttack)
        {
            DrawUpToSix(human);
            DrawUpToSix(cpu);
        }
        else
        {
            DrawUpToSix(cpu);
            DrawUpToSix(human);
        }
    }

    public void SwitchMove()
    {
        PlayerMove = !PlayerMove;
    }

    public string CheckEndState()
    {
        if (human.hand.Count == 0) return "You won!";
        if (cpu.hand.Count == 0) return "CPU won!";
        return "";
    }

    public void CpuTurn()
    {
        bool isCpuAttack = !PlayerAttack;

        Card? cpuCard = cpu.MakeMove(isCpuAttack, CurrentRoundAttacks, LastPlayedCard, TrumpSuit);

        if (cpuCard != null)
        {
            var result = Move(cpuCard, isPlayer: false, allowCpuResponse: false);

            if (result != "")
            {
                EndGame(result);
                MessageBox.Show(result);
            }

            return; // 🔥 critical
        }

        // CPU cannot play
        if (!isCpuAttack)
        {
            // CPU fails defense → picks up
            CpuPickupCards();

            ResetRound();
            RefillHands();

            PlayerMove = PlayerAttack;
            return;
        }
        else
        {
            // CPU stops attacking
            foreach (var card in CurrentRoundAttacks)
                deck.DiscardPile.Add(card);

            foreach (var card in CurrentRoundDefends)
                deck.DiscardPile.Add(card);

            SwitchAttack();
            ResetRound();
            RefillHands();

            PlayerMove = PlayerAttack;

            if (!PlayerMove)
                CpuTurn();

            return;
        }
    }

    private void CpuPickupCards()
    {
        foreach (var card in CurrentRoundAttacks)
            cpu.AddCard(card);

        foreach (var card in CurrentRoundDefends)
            cpu.AddCard(card);
    }

    public void ResetRound()
    {
        CurrentRoundAttacks.Clear();
        CurrentRoundDefends.Clear();
        LastPlayedCard = null;
    }

    private bool DetermineFirstAttacker()
    {
        Card? playerTrump = null;
        Card? cpuTrump = null;

        foreach (var card in human.hand)
        {
            if (card.Suit == TrumpSuit)
            {
                if (playerTrump == null || card.Rank < playerTrump.Rank)
                    playerTrump = card;
            }
        }

        foreach (var card in cpu.hand)
        {
            if (card.Suit == TrumpSuit)
            {
                if (cpuTrump == null || card.Rank < cpuTrump.Rank)
                    cpuTrump = card;
            }
        }

        if (playerTrump == null && cpuTrump == null) return true;
        if (cpuTrump == null) return true;
        if (playerTrump == null) return false;

        return playerTrump.Rank < cpuTrump.Rank;
    }
}