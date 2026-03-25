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

        [JsonProperty("Profiles")]
        public System.Collections.Generic.List<AccountProfile> Profiles { get; set; }
            = new System.Collections.Generic.List<AccountProfile>();
    }
}
