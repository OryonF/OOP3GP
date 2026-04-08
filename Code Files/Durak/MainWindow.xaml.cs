using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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

            // Disables all player actions when the “Play Again” button is visible (game over state),
            // otherwise re-enables the player’s hand so they can play.
            if (Main_PlayAgainButton.Visibility == Visibility.Visible)
            {
                Main_UseCardButton.IsEnabled = false;
                Main_PassButton.IsEnabled = false;
                Main_PlayerHandListBox.IsEnabled = false;
            }
            else
            {
                Main_PlayerHandListBox.IsEnabled = true;
            }

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
                Card cardToShow = null;

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

            // Displays the CPU’s cards on the table by selecting either attack or defend cards
            // based on the current game state, and updates each slot with the correct card image
            // or an empty placeholder if no card is present.
            for (int i = 0; i < cpuSlots.Length; i++)
            {
                Card cardToShow = null;

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

        // Handles when the player clicks "Use Selected Card"
        // - Gets the selected card from the UI
        // - Attempts to play it using game logic
        // - Shows an error if the move is invalid
        // - Lets the CPU take turns until it's the player's turn again
        // - Updates the UI to reflect the new game state
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
                Main_PlayAgainButton.Visibility = Visibility.Visible;
                UpdateUI();
                return;
            }
            while (!game.PlayerMove && game.CheckEndState() == "")
            {
                game.CpuTurn();
            }
            // Refresh all UI (buttons will enable/disable based on PlayerMove)
            string endResult = game.CheckEndState();
            if (endResult != "")
            {
                MessageBox.Show(endResult);
                Main_PlayAgainButton.Visibility = Visibility.Visible;
            }
            UpdateUI();
        }

        // Handles the pass action, checks if the game ended (and shows the result), then updates the UI.
        private void Main_PassButton_Click(object sender, RoutedEventArgs e)
        {
            game.PlayerPasses();

            string endResult = game.CheckEndState();
            if (endResult != "")
            {
                MessageBox.Show(endResult);
                Main_PlayAgainButton.Visibility = Visibility.Visible;
            }

            UpdateUI();
        }

        // Opens a pop-up that shows the game rules.
        private void Main_RulesButton_Click(object sender, RoutedEventArgs e)
        {
            GameRules rulesWindow = new GameRules();
            rulesWindow.Owner = this;
            rulesWindow.ShowDialog();
        }

        // Opens the stats/settings pop-up, then refreshes the screen after it closes.
        private void Main_StatsSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            StatsSettings statSetWindow = new StatsSettings(game);
            statSetWindow.Owner = this;
            statSetWindow.ShowDialog();
            UpdateUI();
        }

        // When you pick a card, it checks if the move is allowed and enables or disables the “Use Card” button.
        private void Main_PlayerHandListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Gets the selected card’s position, and if nothing is selected, it disables the button and stops the code.

            int index = Main_PlayerHandListBox.SelectedIndex;
            if (index < 0)
            {
                Main_UseCardButton.IsEnabled = false;
                return;
            }

            // Checks if the selected card can be played based on the game situation (attack or defend) and enables or disables the button accordingly.
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

        // Restarts the game, updates the screen, hides the play again button, and lets the CPU go first if needed.
        private void Main_PlayAgainButton_Click(object sender, RoutedEventArgs e)
        {
            game = new Game();
            game.StartGame();

            Main_PlayAgainButton.Visibility = Visibility.Collapsed;

            UpdateUI();

            if (!game.PlayerMove)
            {
                game.CpuTurn();
                UpdateUI();
            }
        }
    }
}