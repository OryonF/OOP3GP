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
    }
}


