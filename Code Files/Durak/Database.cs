using Microsoft.Data.Sqlite;

namespace Durak
{
    public static class Database
    {
        private static string connectionString = "Data Source=durak.db";

        public static void Initialize()
        {
            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText =
            @"
            CREATE TABLE IF NOT EXISTS Games (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                StartTime TEXT,
                TrumpSuit TEXT,
                StartingAttacker TEXT,
                Winner TEXT
            );
            ";
            command.ExecuteNonQuery();
        }

        public static int InsertGame(DurakDBModel record)
        {
            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText =
            @"
    INSERT INTO Games (StartTime, TrumpSuit, StartingAttacker, Winner)
    VALUES ($start, $trump, $attacker, $winner);
    ";
            command.Parameters.AddWithValue("$start", record.StartTime.ToString("o"));
            command.Parameters.AddWithValue("$trump", record.TrumpSuit);
            command.Parameters.AddWithValue("$attacker", record.StartingAttacker);
            command.Parameters.AddWithValue("$winner", record.Winner);

            command.ExecuteNonQuery();

            // Get the last inserted row ID
            command.CommandText = "SELECT last_insert_rowid();";
            var lastId = (long)command.ExecuteScalar();

            return (int)lastId;
        }

        public static void UpdateGameWinner(int gameId, string winner)
        {
            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText =
            @"
        UPDATE Games
        SET Winner = $winner
        WHERE Id = $id;
    ";

            command.Parameters.AddWithValue("$winner", winner);
            command.Parameters.AddWithValue("$id", gameId);

            command.ExecuteNonQuery();
        }
    }
}