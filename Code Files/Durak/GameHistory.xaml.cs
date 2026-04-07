using System.Windows;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;

namespace Durak
{
    public partial class GameHistory : Window
    {
        public List<DurakDBModel> Games { get; set; }

        public GameHistory()
        {
            InitializeComponent();
            LoadGames();
        }

        private void LoadGames()
        {
            Games = new List<DurakDBModel>();

            using var connection = new SqliteConnection("Data Source=durak.db");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, StartTime, TrumpSuit, StartingAttacker, Winner FROM Games ORDER BY StartTime DESC";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                Games.Add(new DurakDBModel
                {
                    Id = reader.GetInt32(0),
                    StartTime = DateTime.Parse(reader.GetString(1)),
                    TrumpSuit = reader.GetString(2),
                    StartingAttacker = reader.GetString(3),
                    Winner = reader.GetString(4)
                });
            }

            GameHistoryDataGrid.ItemsSource = Games;
        }

        private void ViewMoveLogButton_Click(object sender, RoutedEventArgs e)
        {
            if (GameHistoryDataGrid.SelectedItem is DurakDBModel selectedGame)
            {
                var moveLogWindow = new MoveLogWindow(selectedGame.Id);
                moveLogWindow.Show();
            }
        }
    }
}