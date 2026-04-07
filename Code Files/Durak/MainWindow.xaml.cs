using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Durak
{
    public partial class MainWindow : Window
    {
        private Game game;

        public MainWindow()
        {
            InitializeComponent();
            Database.Initialize();
            game = new Game();
            game.StartGame();
            UpdateUI();
            if (!game.PlayerMove)
            {
                game.CpuTurn();
                UpdateUI();
            }
        }

        // Refresh all UI elements
        private void UpdateUI()
        {
            // Player card slots
            Image[] playerSlots =
            {
        Main_PlayerPlayedCard1,
        Main_PlayerPlayedCard2,
        Main_PlayerPlayedCard3,
        Main_PlayerPlayedCard4,
        Main_PlayerPlayedCard5,
        Main_PlayerPlayedCard6
    };

            // CPU card slots
            Image[] cpuSlots =
            {
        Main_CPUPlayedCard1,
        Main_CPUPlayedCard2,
        Main_CPUPlayedCard3,
        Main_CPUPlayedCard4,
        Main_CPUPlayedCard5,
        Main_CPUPlayedCard6
    };

            // Update labels
            Main_TrumpSuitValueLabel.Content = game.TrumpSuit;
            Main_CardsRemainingValueLabel.Content = game.deck.Cards.Count;
            Main_PlayerStatusValueLabel.Content = game.PlayerAttack ? "Attacking" : "Defending";
            Main_PlayerHandValueLabel.Content = game.human.hand.Count;
            Main_CpuHandValueLabel.Content = game.cpu.hand.Count;

            // Reset buttons
            Main_UseCardButton.IsEnabled = game.PlayerMove;
            Main_PassButton.IsEnabled = game.PlayerMove;

            // Repopulate ListBox for player's hand
            Main_PlayerHandListBox.Items.Clear();
            foreach (var card in game.human.hand)
            {
                string rankText = card.Rank switch
                {
                    11 => "Jack",
                    12 => "Queen",
                    13 => "King",
                    14 => "Ace",
                    _ => card.Rank.ToString()
                };
                Main_PlayerHandListBox.Items.Add($"{rankText} of {card.Suit}");
            }

            // Update Player slots
            for (int i = 0; i < playerSlots.Length; i++)
            {
                Card? cardToShow = null;

                if (game.PlayerAttack)
                {
                    if (i < game.CurrentRoundAttacks.Count)
                        cardToShow = game.CurrentRoundAttacks[i];
                }
                else
                {
                    if (i < game.CurrentRoundDefends.Count)
                        cardToShow = game.CurrentRoundDefends[i];
                }

                if (cardToShow != null)
                {
                    string rankTextForFile = cardToShow.Rank switch
                    {
                        11 => "J",
                        12 => "Q",
                        13 => "K",
                        14 => "A",
                        _ => cardToShow.Rank.ToString()
                    };
                    string fileName = $"{game.CardThemePrefix}_{cardToShow.Suit.ToLower()}_{rankTextForFile}.png";
                    playerSlots[i].Source = new BitmapImage(
                        new Uri($"pack://application:,,,/{fileName}", UriKind.Absolute));
                }
                else
                {
                    playerSlots[i].Source = new BitmapImage(
                        new Uri("pack://application:,,,/empty.png", UriKind.Absolute));
                }
            }

            // Update CPU slots
            for (int i = 0; i < cpuSlots.Length; i++)
            {
                Card? cardToShow = null;

                if (game.PlayerAttack)
                {
                    if (i < game.CurrentRoundDefends.Count)
                        cardToShow = game.CurrentRoundDefends[i];
                }
                else
                {
                    if (i < game.CurrentRoundAttacks.Count)
                        cardToShow = game.CurrentRoundAttacks[i];
                }

                if (cardToShow != null)
                {
                    string rankTextForFile = cardToShow.Rank switch
                    {
                        11 => "J",
                        12 => "Q",
                        13 => "K",
                        14 => "A",
                        _ => cardToShow.Rank.ToString()
                    };
                    string fileName = $"{game.CardThemePrefix}_{cardToShow.Suit.ToLower()}_{rankTextForFile}.png";
                    cpuSlots[i].Source = new BitmapImage(
                        new Uri($"pack://application:,,,/{fileName}", UriKind.Absolute));
                }
                else
                {
                    cpuSlots[i].Source = new BitmapImage(
                        new Uri("pack://application:,,,/empty.png", UriKind.Absolute));
                }
            }
        }
        private void Main_UseCardButton_Click(object sender, RoutedEventArgs e)
        {
            int index = Main_PlayerHandListBox.SelectedIndex;
            if (index < 0) return;

            // Get selected card
            var selectedCard = game.human.hand[index];

            // Play the card
            var result = game.Move(selectedCard);

            if (result != "")
            {
                MessageBox.Show(result);
                return;
            }
            while (!game.PlayerMove)
            {
                game.CpuTurn();
            }
            // Refresh all UI (buttons will enable/disable based on PlayerMove)
            UpdateUI();
        }

        // Handle "Pass" button
        private void Main_PassButton_Click(object sender, RoutedEventArgs e)
        {
            game.PlayerPasses();
            UpdateUI();
        }

        private void Main_RulesButton_Click(object sender, RoutedEventArgs e)
        {
            GameRules rulesWindow = new GameRules();
            rulesWindow.Owner = this;
            rulesWindow.ShowDialog();
        }

        private void Main_StatsSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            StatsSettings statSetWindow = new StatsSettings(game);
            statSetWindow.Owner = this;
            statSetWindow.ShowDialog();
            UpdateUI();
        }

        // Enable "Use Card" button only when a valid card is selected
        private void Main_PlayerHandListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = Main_PlayerHandListBox.SelectedIndex;
            if (index < 0)
            {
                Main_UseCardButton.IsEnabled = false;
                return;
            }

            var selectedCard = game.human.hand[index];

            if (game.PlayerMove)
            {
                if (game.PlayerAttack && game.CurrentRoundAttacks.Count >= 1)
                {
                    Main_UseCardButton.IsEnabled = Card.CanAdditionalAttack(selectedCard, game.CurrentRoundAttacks, game.CurrentRoundDefends);
                }
                else if (!game.PlayerAttack)
                {
                    Main_UseCardButton.IsEnabled = Card.CanDefend(game.LastPlayedCard, selectedCard, game.TrumpSuit);
                }
                else
                {
                    Main_UseCardButton.IsEnabled = true; // attacking first card
                }
            }
        }
    }
}