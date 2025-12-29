using System;
using System.Collections.Generic;
using System.IO;

namespace AdvancedWebBrowser
{
    public class UserPreferences
    {
        public string HomePage { get; set; } = "https://www.google.com";
        public string Theme { get; set; } = "Light"; // or Dark
        public int SearchEngineIndex { get; set; } = 0;
        public bool ForceSearch { get; set; } = false;

        private static string PrefsPath()
        {
            var dir = AppDomain.CurrentDomain.BaseDirectory;
            return Path.Combine(dir, "userprefs.ini");
        }

        public static UserPreferences Load()
        {
            var prefs = new UserPreferences();
            try
            {
                var path = PrefsPath();
                if (!File.Exists(path)) return prefs;
                var lines = File.ReadAllLines(path);
                foreach (var line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#")) continue;
                    var idx = line.IndexOf('=');
                    if (idx <= 0) continue;
                    var key = line.Substring(0, idx).Trim();
                    var val = line.Substring(idx + 1).Trim();
                    switch (key)
                    {
                        case "HomePage": prefs.HomePage = val; break;
                        case "Theme": prefs.Theme = val; break;
                        case "SearchEngineIndex": int.TryParse(val, out var si); prefs.SearchEngineIndex = si; break;
                        case "ForceSearch": bool.TryParse(val, out var fs); prefs.ForceSearch = fs; break;
                    }
                }
            }
            catch
            {
                // ignore
            }
            return prefs;
        }

        public void Save()
        {
            try
            {
                var path = PrefsPath();
                var lines = new List<string>
                {
                    "# User preferences",
                    $"HomePage={HomePage}",
                    $"Theme={Theme}",
                    $"SearchEngineIndex={SearchEngineIndex}",
                    $"ForceSearch={ForceSearch}"
                };
                File.WriteAllLines(path, lines);
            }
            catch
            {
                // ignore
            }
        }
    }
}
