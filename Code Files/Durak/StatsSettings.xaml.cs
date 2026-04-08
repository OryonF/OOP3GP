using System.Windows;

namespace Durak
{
    public partial class StatsSettings : Window
    {
        private Game game;
        private StatsSettingsData stats;
        private string selectedTheme;

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
            // Reset Stats
            stats.Wins = 0;
            stats.Losses = 0;
            stats.TotalGames = 0;
            stats.SelectedCardTheme = "n";
            stats.PlayerName = "";
            stats.CurrentStreak = 0;
            stats.LongestWinStreak = 0;
            stats.LongestLossStreak = 0;

            // Update labels 
            StatSet_GamesPlayedValueLabel.Content = stats.TotalGames.ToString();
            StatSet_CurrentStreakValueLabel.Content = "0";

            // Update Radio Buttons
            StatSet_NormalThemeRadioButton.IsChecked = true;
            StatSet_InvertedThemeRadioButton.IsChecked = false;
            StatSet_SillyThemeRadioButton.IsChecked = false;

            StatSet_PlayerNameTextBox.Text = "";

            // Save reset stats to json file
            StatsSettingsManager.Save(stats);

            // Show message to notify stats have been reset
            MessageBox.Show("Player statistics have been reset.", "Stats Reset", MessageBoxButton.OK, 
                MessageBoxImage.Information);







        }
    }
} 

