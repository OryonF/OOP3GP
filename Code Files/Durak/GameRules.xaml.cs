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
    /// Interaction logic for GameRules.xaml
    /// </summary>
    public partial class GameRules : Window
    {
        public GameRules()
        {
            InitializeComponent();
        }

        private void Rules_ExitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
