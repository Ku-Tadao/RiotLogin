using Newtonsoft.Json;

namespace RiotLogin
{
    public class AccountProfile
    {
        [JsonProperty("RiotID")]
        public string RiotID { get; set; } = "";

        [JsonProperty("Username")]
        public string Username { get; set; } = "";

        [JsonProperty("Password")]
        public string Password { get; set; } = "";

        [JsonIgnore]
        public string PasswordMask => string.IsNullOrEmpty(Password) ? "" : "••••••••";

        [JsonProperty("Level")]
        public int Level { get; set; }

        [JsonProperty("Server")]
        public string Server { get; set; } = "";

        [JsonProperty("BE")]
        public int BE { get; set; }

        [JsonProperty("RP")]
        public int RP { get; set; }

        [JsonProperty("SoloQ")]
        public string SoloQ { get; set; } = "";

        [JsonProperty("FlexQ")]
        public string FlexQ { get; set; } = "";

        [JsonProperty("Champions")]
        public int Champions { get; set; }

        [JsonProperty("ChampionList")]
        public string ChampionList { get; set; } = "";

        [JsonProperty("Skins")]
        public int Skins { get; set; }

        [JsonProperty("SkinList")]
        public string SkinList { get; set; } = "";

        // Filled by Pull Data. Puuid lets login skip accounts that are already signed in.
        [JsonProperty("Puuid")]
        public string Puuid { get; set; } = "";

        [JsonProperty("ProfileIconId")]
        public int ProfileIconId { get; set; }

        [JsonIgnore]
        public string IconUrl => ProfileIconId <= 0 ? null :
            $"https://raw.communitydragon.org/latest/plugins/rcp-be-lol-game-data/global/default/v1/profile-icons/{ProfileIconId}.jpg";

        /// <summary>"OK", "Banned …", "Ranked restricted …" or "Chat restricted". Empty = never pulled.</summary>
        [JsonProperty("Status")]
        public string Status { get; set; } = "";

        [JsonProperty("PeakRank")]
        public string PeakRank { get; set; } = "";

        [JsonProperty("Honor")]
        public int Honor { get; set; }

        [JsonProperty("Mastery")]
        public int Mastery { get; set; }

        [JsonProperty("Loot")]
        public int Loot { get; set; }

        [JsonProperty("LootList")]
        public string LootList { get; set; } = "";
    }

    public class AppData
    {
        [JsonProperty("RememberMe")]
        public bool RememberMe { get; set; }

        [JsonProperty("RiotGamesPath")]
        public string RiotGamesPath { get; set; } = "";

        // UI state: last selected account (by username) and main window Left, Top, Width, Height.
        [JsonProperty("LastAccount")]
        public string LastAccount { get; set; } = "";

        [JsonProperty("Window")]
        public double[] Window { get; set; }

        [JsonProperty("Profiles")]
        public System.Collections.Generic.List<AccountProfile> Profiles { get; set; }
            = new System.Collections.Generic.List<AccountProfile>();
    }
}
