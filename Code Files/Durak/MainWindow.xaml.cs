using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Durak
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Main_RulesButton_Click(object sender, RoutedEventArgs e)
        {
            GameRules rulesWindow = new GameRules();
            rulesWindow.Owner = this;
            rulesWindow.ShowDialog();
        }

        private void Main_StatsSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            StatsSettings statSetWindow = new StatsSettings();
            statSetWindow.Owner = this;
            statSetWindow.ShowDialog();
        }
    }
}