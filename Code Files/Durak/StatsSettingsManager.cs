using System.IO;
using System.Text.Json;

namespace Durak
{
    public static class StatsSettingsManager
    {
        private static string filePath = "stats.json";

        public static StatsSettingsData Load()
        {
            // If file doesn't exist, create it with default stats
            if (!File.Exists(filePath))
            {
                var defaultStats = new StatsSettingsData();
                Save(defaultStats); // creates stats.json immediately
                return defaultStats;
            }

            string json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<StatsSettingsData>(json) ?? new StatsSettingsData();
        }

        public static void Save(StatsSettingsData stats)
        {
            string json = JsonSerializer.Serialize(stats, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
        }
    }
}


