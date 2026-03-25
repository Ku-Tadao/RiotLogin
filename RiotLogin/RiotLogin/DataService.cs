using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace RiotLogin
{
    public static class DataService
    {
        private const string FileName = "info.json";

        public static AppData Load()
        {
            if (!File.Exists(FileName))
            {
                var blank = new AppData();
                Save(blank);
                return blank;
            }

            string json = File.ReadAllText(FileName);
            JObject root = JObject.Parse(json);

            // Migrate old format: profiles with "AccountTag" instead of "RiotID"
            MigrateOldFormat(root);

            var data = root.ToObject<AppData>() ?? new AppData();
            return data;
        }

        public static void Save(AppData data)
        {
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(FileName, json);
        }

        private static void MigrateOldFormat(JObject root)
        {
            var profiles = root["Profiles"] as JArray;
            if (profiles == null) return;

            bool migrated = false;
            foreach (JObject profile in profiles)
            {
                // Rename "AccountTag" -> "RiotID" if old field exists
                if (profile["AccountTag"] != null && profile["RiotID"] == null)
                {
                    profile["RiotID"] = profile["AccountTag"];
                    profile.Remove("AccountTag");
                    migrated = true;
                }

                // Ensure new fields exist with defaults
                EnsureField(profile, "Level", 0);
                EnsureField(profile, "Server", "");
                EnsureField(profile, "BE", 0);
                EnsureField(profile, "RP", 0);
                EnsureField(profile, "SoloQ", "");
                EnsureField(profile, "FlexQ", "");
                EnsureField(profile, "Champions", 0);
                EnsureField(profile, "ChampionList", "");
                EnsureField(profile, "Skins", 0);
                EnsureField(profile, "SkinList", "");
                EnsureField(profile, "Loot", 0);
                EnsureField(profile, "LootList", "");
            }

            // Handle old RememberMe as int (0/1) -> bool
            var rm = root["RememberMe"];
            if (rm != null && rm.Type == JTokenType.Integer)
            {
                root["RememberMe"] = ((int)rm) != 0;
                migrated = true;
            }

            if (migrated)
            {
                string json = root.ToString(Formatting.Indented);
                File.WriteAllText(FileName, json);
            }
        }

        private static void EnsureField(JObject obj, string name, object defaultValue)
        {
            if (obj[name] == null)
            {
                obj[name] = JToken.FromObject(defaultValue);
            }
        }

        /// <summary>
        /// Auto-detect the full path to RiotClientServices.exe via registry, then default path, then null.
        /// Ported from League Account Manager's Settings.findriot().
        /// </summary>
        public static string FindRiotClient()
        {
            string[] registryEntries =
            {
                @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Uninstall\Riot Game Riot_Client.",
                "UninstallString",

                @"HKEY_CLASSES_ROOT\riotclient\DefaultIcon",
                "(Default)",

                @"HKEY_CLASSES_ROOT\riotclient\shell\open\command",
                "(Default)",

                @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Run",
                "RiotClient",

                @"HKEY_LOCAL_MACHINE\SOFTWARE\Classes\riotclient\DefaultIcon",
                "(Default)"
            };

            for (int i = 0; i < registryEntries.Length; i += 2)
            {
                string key = registryEntries[i];
                string valueName = registryEntries[i + 1];

                string installPath = (string)Registry.GetValue(key, valueName, null);
                if (installPath != null)
                {
                    var match = Regex.Match(installPath, "\"(.*?)\"");
                    if (match.Success && File.Exists(match.Groups[1].Value))
                        return match.Groups[1].Value;

                    // Some entries may not be quoted
                    string trimmed = installPath.Trim().Split(' ')[0].Trim('"');
                    if (File.Exists(trimmed))
                        return trimmed;
                }
            }

            // Default path fallback
            const string defaultPath = @"C:\Riot Games\Riot Client\RiotClientServices.exe";
            if (File.Exists(defaultPath))
                return defaultPath;

            return null;
        }
    }
}
