using System.IO;
using System.Text.Json;

namespace Durak
{
    public static class StatsSettingsManager
    {
        private static string filePath = "stats.json";

        public static StatsSettingsData Load()
        {
            if (!File.Exists(filePath))
                return new StatsSettingsData();

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


