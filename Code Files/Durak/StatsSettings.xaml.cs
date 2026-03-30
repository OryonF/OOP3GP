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
        public StatsSettings()
        {
            InitializeComponent();
        }

        private void StatSet_SaveAndExitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void StatSet_ExitWithoutSavingButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void StatSet_NormalThemeRadioButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void StatSet_InvertedThemeRadioButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void StatSet_SillyThemeRadioButton_Checked(object sender, RoutedEventArgs e)
        {

        }
    }
}
