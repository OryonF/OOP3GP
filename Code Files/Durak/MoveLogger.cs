using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Durak
{
    public static class MoveLogger
    {
        private static string filePath = "MoveLogs.json";

        // Stores all move logs in memory keyed by Game ID
        private static Dictionary<int, List<string>> allMoves = new Dictionary<int, List<string>>();

        // Load existing move logs from file at startup
        static MoveLogger()
        {
            if (File.Exists(filePath))
            {
                var json = File.ReadAllText(filePath);
                allMoves = JsonSerializer.Deserialize<Dictionary<int, List<string>>>(json) ?? new Dictionary<int, List<string>>();
            }
        }

        // Log a move
        public static void LogMove(int gameId, string playerType, string action, string card)
        {
            if (!allMoves.ContainsKey(gameId))
                allMoves[gameId] = new List<string>();

            allMoves[gameId].Add($"{playerType} {action} {card}");

            Save();
        }

        // Get all moves for a game
        public static List<string> GetMoves(int gameId)
        {
            return allMoves.ContainsKey(gameId) ? allMoves[gameId] : new List<string>();
        }

        // Save all moves to JSON
        private static void Save()
        {
            var json = JsonSerializer.Serialize(allMoves, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
        }
    }
}