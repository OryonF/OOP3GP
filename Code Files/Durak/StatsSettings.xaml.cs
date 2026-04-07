using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Durak
{
    /// <summary>
    /// Interaction logic for StatsSettings.xaml
    /// </summary>
    public partial class StatsSettings : Window
    {
        private Game game;
        private string selectedTheme;
        public StatsSettings(Game gameInstance)
        {
            InitializeComponent();
            game = gameInstance;

            selectedTheme = game.CardThemePrefix;

            switch (selectedTheme)
            {
                case "n":
                    StatSet_NormalThemeRadioButton.IsChecked = true;
                    break;
                case "i":
                    StatSet_InvertedThemeRadioButton.IsChecked = true;
                    break;
                case "s":
                    StatSet_SillyThemeRadioButton.IsChecked = true;
                    break;
            }
        }

        private void StatSet_SaveAndExitButton_Click(object sender, RoutedEventArgs e)
        {
            game.CardThemePrefix=selectedTheme;
            this.Close();
        }

        private void StatSet_ExitWithoutSavingButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void StatSet_NormalThemeRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            selectedTheme = "n";
        }

        private void StatSet_InvertedThemeRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            selectedTheme = "i";
        }

        private void StatSet_SillyThemeRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            selectedTheme = "s";
        }

        private void StatSet_ViewGameHistoryButton_Click(object sender, RoutedEventArgs e)
        {
            var historyWindow = new GameHistory();
            historyWindow.Show();
        }
    }
}
