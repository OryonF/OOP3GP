using System.ComponentModel.DataAnnotations;
using System.Windows;

namespace Durak
{
    public partial class StatsSettings : Window
    {
        private Game game;
        private StatsSettingsData stats;
        private string selectedTheme;
        public StatsSettingsData SavedStats => stats;
        public bool SettingsSaved { get; private set; } = false;

        public StatsSettings(Game gameInstance)
        {
            InitializeComponent();
            game = gameInstance;

            // Load stats from JSON
            stats = StatsSettingsManager.Load();

            // Display stats in labels
            StatSet_GamesPlayedValueLabel.Content = stats.TotalGames.ToString();
            StatSet_PlayerWinsValueLabel.Content = stats.Wins.ToString();
            StatSet_CpuWinsValueLabel.Content = stats.Losses.ToString();
            StatSet_CurrentStreakValueLabel.Content = stats.CurrentStreak.ToString();
            StatSet_LongestWinStreakValueLabel.Content = stats.LongestWinStreak.ToString();
            StatSet_LongestLossStreakValueLabel.Content = stats.LongestLossStreak.ToString();

            //Display Player Name:
            StatSet_PlayerNameTextBox.Text = stats.PlayerName ?? "";

            // Set theme
            selectedTheme = stats.SelectedCardTheme;
            switch (selectedTheme)
            {
                case "n": StatSet_NormalThemeRadioButton.IsChecked = true; break;
                case "i": StatSet_InvertedThemeRadioButton.IsChecked = true; break;
                case "s": StatSet_SillyThemeRadioButton.IsChecked = true; break;
            }
        }

        private void StatSet_SaveAndExitButton_Click(object sender, RoutedEventArgs e)
        {
            game.CardThemePrefix = selectedTheme;
            stats.SelectedCardTheme = selectedTheme;
            stats.PlayerName = StatSet_PlayerNameTextBox.Text;
            StatsSettingsManager.Save(stats);

            SettingsSaved = true; // mark that user actually saved
            this.Close();
        }

        private void StatSet_ExitWithoutSavingButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void StatSet_NormalThemeRadioButton_Checked(object sender, RoutedEventArgs e) => selectedTheme = "n";
        private void StatSet_InvertedThemeRadioButton_Checked(object sender, RoutedEventArgs e) => selectedTheme = "i";
        private void StatSet_SillyThemeRadioButton_Checked(object sender, RoutedEventArgs e) => selectedTheme = "s";

        private void StatSet_ViewGameHistoryButton_Click(object sender, RoutedEventArgs e)
        {
            var historyWindow = new GameHistory();
            historyWindow.Show();
        }

        private void StatSet_ResetStats_Click(object sender, RoutedEventArgs e)
        {
            string message = "Do you want to reset the following stats and settings?\n\n" +
                             "Player Name\n" +
                             "Card Theme\n" +
                             "Games Played\n" +
                             "Wins\n" +
                             "Losses\n" +
                             "Win Streak\n\n" +
                             "Additionally, any progress in the current game will be lost; no winner will be declared and a new game will be started.\n\n" +
                             "NOTE: Past games and move logs will still appear in View Game History";

            MessageBoxResult result = MessageBox.Show(
                message,
                "Confirm Reset",
                MessageBoxButton.OKCancel,
                MessageBoxImage.Warning
            );

            if (result == MessageBoxResult.OK)
            {
                stats = new StatsSettingsData();
                StatsSettingsManager.Save(stats);
                SettingsSaved = true;

                if (Owner is MainWindow mainWindow)
                {
                    mainWindow.ResetGameAfterStatsReset(stats);
                }

                MessageBox.Show("Stats have been reset and a new game has started.", "Reset Complete", MessageBoxButton.OK, MessageBoxImage.Information);

                this.Close();
            }
        }
    }
} 

