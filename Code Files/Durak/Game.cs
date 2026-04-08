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
    public int CurrentGameID { get; private set; }

    public DateTime StartTime { get; private set; }
    public string StartingAttacker { get; private set; }
    public bool gameSaved { get; private set; }

    public string CardThemePrefix { get; set; } = "n"; // default = normal
    public string TrumpSuit { get; private set; } = "";
    public bool PlayerAttack { get; private set; }  // true = human attacking
    public bool PlayerMove { get; private set; }    // whose move it is

    public List<Card> CurrentRoundAttacks { get; private set; } = new List<Card>();
    public List<Card> CurrentRoundDefends { get; private set; } = new List<Card>();
    public Card? LastPlayedCard { get; private set; }

    private StatsSettingsData stats;
    //private GameLog currentlog;(Will be implemented later)

    // Creates a new game by setting up the deck and both players.
    public Game()
    {
        deck = new Deck();
        human = new Player();
        cpu = new ComputerPlayer();

        stats = StatsSettingsManager.Load();
    }

    // Starts the game by setting everything up, deciding who goes first, saving it to the database, and dealing cards to both players.
    public void StartGame()
    {
        StartTime = DateTime.Now;

        deck.Initialize();
        deck.Shuffle(deck.Cards);

        // Set temporary values first
        TrumpSuit = deck.GetBottomCard().Suit;
        PlayerAttack = DetermineFirstAttacker();
        StartingAttacker = PlayerAttack ? "HUMAN" : "CPU";
        PlayerMove = PlayerAttack;

        var record = new DurakDBModel
        {
            StartTime = StartTime,
            TrumpSuit = TrumpSuit.ToUpper(),
            StartingAttacker = StartingAttacker,
            Winner = "UNDECLARED"
        };

        CurrentGameID = Database.InsertGame(record);

        for (int i = 0; i < 6; i++)
        {
            Card humanCard = deck.Draw();
            human.AddCard(humanCard);
            MoveLogger.LogMove(CurrentGameID, "HUMAN", "DRAW", humanCard.ToString());

            Card cpuCard = deck.Draw();
            cpu.AddCard(cpuCard);
            MoveLogger.LogMove(CurrentGameID, "CPU", "DRAW", cpuCard.ToString());
        }
    }

    // Plays a card (attack or defend), updates the game state and hands, ends the round if needed, and checks if the game is over.
    public string Move(Card card, bool isPlayer = true, bool allowCpuResponse = true)
    {
        if (card == null) return "";

        LastPlayedCard = card;

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
            MoveLogger.LogMove(CurrentGameID,
                PlayerAttack ? "HUMAN" : "CPU",
                "PASS",
                "Attack limit reached");

            foreach (var c in CurrentRoundAttacks) deck.DiscardPile.Add(c);
            foreach (var c in CurrentRoundDefends) deck.DiscardPile.Add(c);

            SwitchAttack();
            ResetRound();
            RefillHands();

            PlayerMove = PlayerAttack;
            if (!PlayerMove && allowCpuResponse) CpuTurn();
        }

        string result = CheckEndState();

        if (result != "")
            EndGame(result);
        

        return result;
    }

    private bool statsSaved = false; // new field

    private void EndGame(string result)
    {
        if (statsSaved) return;
        statsSaved = true;

        stats = StatsSettingsManager.Load(); // ✅ reload latest data

        stats.TotalGames++;

        if (result == "You won! The CPU is the durak!")
        {
            stats.Wins++;
            stats.CurrentStreak = Math.Max(0, stats.CurrentStreak) + 1;
            stats.LongestWinStreak = Math.Max(stats.LongestWinStreak, stats.CurrentStreak);
        }
        else if (result == "CPU won! You are the durak!")
        {
            stats.Losses++;
            stats.CurrentStreak = Math.Min(0, stats.CurrentStreak) - 1;
            stats.LongestLossStreak = Math.Max(stats.LongestLossStreak, -stats.CurrentStreak);
        }

        stats.SelectedCardTheme = CardThemePrefix;

        StatsSettingsManager.Save(stats);
    }


    // Switches the attacking player at the end of a round.
    public void SwitchAttack()
    {
        PlayerAttack = !PlayerAttack;
    }

    // Handles the logic for when a player chooses to pass.
    // This can mean either ending the round after a successful
    // defense or picking up cards after a failed defense.
    public void PlayerPasses()
    {
        if (CurrentRoundAttacks.Count == 0)
        {
            // Skip round start → just swap attacker
            MoveLogger.LogMove(CurrentGameID, "HUMAN", "PASS", "End of round");

            SwitchAttack();
            PlayerMove = PlayerAttack;

            if (!PlayerMove)
                CpuTurn();

            return;
        }
        if (!PlayerAttack)
        {
            MoveLogger.LogMove(CurrentGameID, "HUMAN", "PASS", "End of round");
            // defender picks up
            foreach (var card in CurrentRoundAttacks)
            {
                human.AddCard(card);
                MoveLogger.LogMove(CurrentGameID, "HUMAN", "PICKUP", card.ToString());
            }

            foreach (var card in CurrentRoundDefends)
            {
                human.AddCard(card);
                MoveLogger.LogMove(CurrentGameID, "HUMAN", "PICKUP", card.ToString());
            }

        }
        else
        {
            MoveLogger.LogMove(CurrentGameID, "HUMAN", "PASS", "End of round");
            // successful defense
            foreach (var card in CurrentRoundAttacks)
            {
                deck.DiscardPile.Add(card);
            }

            foreach (var card in CurrentRoundDefends)
            {
                deck.DiscardPile.Add(card);
            }

            SwitchAttack();
        }

        ResetRound();
        RefillHands();

        PlayerMove = PlayerAttack;

        if (!PlayerMove)
            CpuTurn();
    }

    // Draws cards for a player until they have 6 cards or the deck runs out.
    private void DrawUpToSix(Player player)
    {
        while (player.hand.Count < 6 && deck.Cards.Count > 0)
        {
            Card drawnCard = deck.Draw();
            player.AddCard(drawnCard);

            string playerType = player == human ? "HUMAN" : "CPU";

            MoveLogger.LogMove(CurrentGameID, playerType, "DRAW", drawnCard.ToString());
        }
    }

    // Refills both players' hands at the end of a round, with the attacker drawing first.
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

    // Switches the current player after a move is made.
    public void SwitchMove()
    {
        PlayerMove = !PlayerMove;
    }

    // Checks if the game has ended by seeing if either player has run out of cards, and returns a message if so.
    public string CheckEndState()
    {
        if (human.hand.Count == 0)
        {
            SaveGame("HUMAN");
            return "You won! The CPU is the durak!";
        }

        if (cpu.hand.Count == 0)
        {
            SaveGame("CPU");
            return "CPU won! You are the durak!";
        }

        return "";
    }

    // Handles the CPU's turn by determining if it can attack or defend, making the appropriate move, and ending the round if necessary.
    public void CpuTurn()
    {
        // Stop immediately if the game is already over
        if (CheckEndState() != "")
            return;

        bool isCpuAttack = !PlayerAttack;

        Card cpuCard = cpu.MakeMove(isCpuAttack, CurrentRoundAttacks, CurrentRoundDefends, LastPlayedCard, TrumpSuit);

        // CPU is able to play a card
        if (cpuCard != null)
        {

            Move(cpuCard, isPlayer: false, allowCpuResponse: false);
            return;
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
        
        // CPU stops attacking
        foreach (var card in CurrentRoundAttacks)
            deck.DiscardPile.Add(card);

        foreach (var card in CurrentRoundDefends)
            deck.DiscardPile.Add(card);

        MoveLogger.LogMove(CurrentGameID, "CPU", "PASS", "End of round");
        SwitchAttack();
        ResetRound();
        RefillHands();

        PlayerMove = PlayerAttack;

        if (!PlayerMove)
            CpuTurn();
    }

    // Handles the logic for when the CPU fails to defend and has to pick up the cards.
    private void CpuPickupCards()
    {
        MoveLogger.LogMove(CurrentGameID, "CPU", "PASS", "End of round");

        foreach (var card in CurrentRoundAttacks)
        {
            MoveLogger.LogMove(CurrentGameID, "CPU", "PICKUP", card.ToString());
            cpu.AddCard(card);
        }

        foreach (var card in CurrentRoundDefends)
        {
            MoveLogger.LogMove(CurrentGameID, "CPU", "PICKUP", card.ToString());
            cpu.AddCard(card);
        }
    }

    // Resets the current round by clearing the attack and defense lists and resetting the last played card.
    public void ResetRound()
    {
        CurrentRoundAttacks.Clear();
        CurrentRoundDefends.Clear();
        LastPlayedCard = null;
    }

    // Saves the game result to the database if it hasn't already been saved.
    private void SaveGame(string winner)
    {
        if (gameSaved) return;

        Database.UpdateGameWinner(CurrentGameID, winner);
        gameSaved = true;
    }

    // Determines who goes first by comparing the lowest trump card in each player's hand,
    // or defaults to the human player if neither has a trump.
    private bool DetermineFirstAttacker()
    {
        Card? playerTrump = null;
        Card? cpuTrump = null;

        // Find lowest trump card in human hand
        foreach (var card in human.hand)
        {
            if (card.Suit == TrumpSuit)
            {
                if (playerTrump == null || card.Rank < playerTrump.Rank)
                    playerTrump = card;
            }
        }

        // Find lowest trump card in CPU hand
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