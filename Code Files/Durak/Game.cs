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

    public Game()
    {
        deck = new Deck();
        human = new Player();
        cpu = new ComputerPlayer();
    }

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

        private void EndGame(string result)
    {
        stats.TotalGames++;


        if (result.Contains("You won"))
        {
            stats.Wins++;

            //Determine win or loss streak
            if (stats.CurrentStreak >= 0)
                stats.CurrentStreak++;
            else
                stats.CurrentStreak = 1;

            if (stats.CurrentStreak > stats.LongestWinStreak)
                stats.CurrentStreak = stats.CurrentStreak;
        }
        else
        {
            stats.Losses++;

            // Loss streak
            if (stats.CurrentStreak <= 0)
                stats.CurrentStreak--;

            else
                stats.CurrentStreak = -1;

            if (-stats.CurrentStreak > stats.LongestLossStreak)
                stats.LongestLossStreak = -stats.CurrentStreak;
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
        if (human.hand.Count == 0)
        {
            SaveGame("HUMAN");
            return "You won!";
        }

        if (cpu.hand.Count == 0)
        {
            SaveGame("CPU");
            return "CPU won!";
        }

        return "";
    }

    public void CpuTurn()
    {
        bool isCpuAttack = !PlayerAttack;

        Card cpuCard = cpu.MakeMove(isCpuAttack, CurrentRoundAttacks, CurrentRoundDefends, LastPlayedCard, TrumpSuit);

        if (cpuCard != null)
        {
            var result = Move(cpuCard, isPlayer: false, allowCpuResponse: false);

            if (result != "")
            {
                EndGame(result);
                MessageBox.Show(result);
            }

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
        else
        {
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

            return;
        }
    }

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

    public void ResetRound()
    {
        CurrentRoundAttacks.Clear();
        CurrentRoundDefends.Clear();
        LastPlayedCard = null;
    }

    private void SaveGame(string winner)
    {
        if (gameSaved) return;

        Database.UpdateGameWinner(CurrentGameID, winner);
        gameSaved = true;
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