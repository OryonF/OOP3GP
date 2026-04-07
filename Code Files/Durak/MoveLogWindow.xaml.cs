using System.Windows;

namespace Durak
{
    public partial class MoveLogWindow : Window
    {
        private int GameId;

        public MoveLogWindow(int gameId)
        {
            InitializeComponent();
            GameId = gameId;

            LoadMoveLog();
        }

        private void LoadMoveLog()
        {
            // Get the move list from MoveLogger
            var moves = MoveLogger.GetMoves(GameId);

            // If no moves, show a friendly message
            if (moves.Count == 0)
            {
                MoveLogListBox.Items.Add("No moves recorded for this game.");
                return;
            }

            // Otherwise, populate the ListBox
            foreach (var move in moves)
            {
                MoveLogListBox.Items.Add(move);
            }
        }
    }
}