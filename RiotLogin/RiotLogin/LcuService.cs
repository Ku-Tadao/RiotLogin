using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RiotLogin
{
    public static class LcuService
    {
        public static async Task<AccountProfile> PullDataAsync(AccountProfile profile)
        {
            string port, token;
            if (!FindLeagueClient(out port, out token))
                throw new InvalidOperationException("Could not connect to League Client. Ensure LeagueClientUx is running (not just the Riot Client).");

            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (s, c, ch, e) => true
            };
            using (var client = new HttpClient(handler))
            {
                byte[] cred = Encoding.UTF8.GetBytes("riot:" + token);
                client.DefaultRequestHeaders.Add("Authorization", "Basic " + Convert.ToBase64String(cred));
                client.DefaultRequestHeaders.Add("Accept", "application/json");

                string baseUrl = "https://127.0.0.1:" + port;

                // Summoner info
                var summoner = await GetJson(client, baseUrl + "/lol-summoner/v1/current-summoner");
                profile.RiotID = (string)summoner["gameName"] + "#" + (string)summoner["tagLine"];
                profile.Level = (int)summoner["summonerLevel"];
                profile.Puuid = (string)summoner["puuid"] ?? "";
                profile.ProfileIconId = (int?)summoner["profileIconId"] ?? 0;
                string summonerId = summoner["summonerId"].ToString();

                // Region
                var region = await GetJson(client, baseUrl + "/riotclient/region-locale");
                profile.Server = (string)region["region"] ?? "";

                // Wallet
                var wallet = await GetJson(client, baseUrl + "/lol-inventory/v1/wallet?currencyTypes=[%22RP%22,%22lol_blue_essence%22]");
                profile.BE = wallet["lol_blue_essence"] != null ? (int)wallet["lol_blue_essence"] : 0;
                profile.RP = wallet["RP"] != null ? (int)wallet["RP"] : 0;

                // Ranked
                var ranked = await GetJson(client, baseUrl + "/lol-ranked/v1/current-ranked-stats");
                profile.SoloQ = FormatRank(ranked, "RANKED_SOLO_5x5");
                profile.FlexQ = FormatRank(ranked, "RANKED_FLEX_SR");
                profile.PeakRank = FormatPeak(ranked);

                // Champions
                var champs = await GetArray(client, baseUrl + "/lol-champions/v1/inventories/" + summonerId + "/champions-minimal");
                var champNames = champs.Where(c => (bool?)c["ownership"]?["owned"] == true).Select(c => (string)c["name"]).ToList();
                profile.Champions = champNames.Count;
                profile.ChampionList = string.Concat(champNames.Select(n => ":" + n));

                // Skins (owned, not counting each champion's default skin)
                var skins = await GetArray(client, baseUrl + "/lol-champions/v1/inventories/" + summonerId + "/skins-minimal");
                var skinNames = skins.Where(x => (bool?)x["ownership"]?["owned"] == true && (bool?)x["isBase"] != true)
                                     .Select(x => (string)x["name"]).ToList();
                profile.Skins = skinNames.Count;
                profile.SkinList = string.Concat(skinNames.Select(n => ":" + n));

                // Loot
                var lootMap = await GetJson(client, baseUrl + "/lol-loot/v1/player-loot-map");
                int lootCount = 0;
                string lootList = "";
                foreach (var prop in lootMap.Properties())
                {
                    var item = prop.Value;
                    if (item["count"] != null && (int)item["count"] > 0)
                    {
                        string desc = (string)item["itemDesc"];
                        if (string.IsNullOrEmpty(desc)) desc = (string)item["localizedName"];
                        if (string.IsNullOrEmpty(desc)) desc = (string)item["lootId"];
                        lootList += ":" + desc + " x " + (int)item["count"];
                        lootCount++;
                    }
                }
                profile.Loot = lootCount;
                profile.LootList = lootList;

                // Optional extras: a missing endpoint shouldn't fail the whole pull.
                profile.Honor = (int?)(await TryGet(client, baseUrl + "/lol-honor-v2/v1/profile"))?["honorLevel"] ?? 0;
                var score = await TryGet(client, baseUrl + "/lol-champion-mastery/v1/local-player/champion-mastery-score");
                profile.Mastery = score?.Type == JTokenType.Integer ? (int)score : 0;
                profile.Status = await GetStatus(client, baseUrl);
            }

            return profile;
        }

        private static string Tier(string tier, string division)
        {
            if (string.IsNullOrEmpty(tier) || tier == "NONE") return null;
            string t = tier.Substring(0, 1) + tier.Substring(1).ToLowerInvariant();
            return string.IsNullOrEmpty(division) || division == "NA" ? t : t + " " + division; // apex tiers have no division
        }

        private static string FormatRank(JObject ranked, string queue)
        {
            var q = ranked["queueMap"]?[queue];
            if (q == null) return "Unranked";
            if ((bool?)q["isProvisional"] == true && (int?)q["provisionalGamesRemaining"] > 0)
                return $"Placements, {(int)q["provisionalGamesRemaining"]} left";
            string tier = Tier((string)q["tier"], (string)q["division"]);
            if (tier == null) return "Unranked";
            return $"{tier} {(int?)q["leaguePoints"] ?? 0} LP ({(int?)q["wins"] ?? 0}W {(int?)q["losses"] ?? 0}L)";
        }

        // Highest solo rank this season, else last season's.
        private static string FormatPeak(JObject ranked)
        {
            var q = ranked["queueMap"]?["RANKED_SOLO_5x5"];
            if (q == null) return "";
            return Tier((string)q["highestTier"], (string)q["highestDivision"])
                ?? (Tier((string)q["previousSeasonHighestTier"], (string)q["previousSeasonHighestDivision"]) is string last ? last + " (last season)" : "");
        }

        private static async Task<string> GetStatus(HttpClient client, string baseUrl)
        {
            var activeBan = await TryGet(client, baseUrl + "/lol-player-behavior/v1/active-ban");
            if (activeBan?.Type == JTokenType.Boolean && (bool)activeBan)
            {
                var ban = await TryGet(client, baseUrl + "/lol-player-behavior/v1/ban");
                if ((bool?)ban?["isPermaBan"] == true) return "Banned (permanent)";
                long ms = (long?)ban?["timeUntilBanExpires"] ?? 0;
                return ms > 0 ? $"Banned, {Math.Max(1, (int)Math.Ceiling(ms / 86400000.0))}d left" : "Banned";
            }
            int games = (int?)(await TryGet(client, baseUrl + "/lol-leaver-buster/v1/ranked-restriction"))?["punishedGamesRemaining"] ?? 0;
            if (games > 0) return $"Ranked restricted, {games} games";
            var chat = await TryGet(client, baseUrl + "/lol-player-behavior/v1/chat-restriction");
            if (chat != null && chat.Type == JTokenType.Object && ((long?)chat["expirationMillis"] > 0 || (int?)chat["gamesRemaining"] > 0))
                return "Chat restricted";
            return "OK";
        }

        // 404 means "nothing to report" for most of these (no ban, no restriction).
        private static async Task<JToken> TryGet(HttpClient client, string url)
        {
            try
            {
                var resp = await client.GetAsync(url);
                if (!resp.IsSuccessStatusCode) return null;
                return JToken.Parse(await resp.Content.ReadAsStringAsync());
            }
            catch { return null; }
        }

        private static async Task<JObject> GetJson(HttpClient client, string url)
        {
            var resp = await client.GetAsync(url);
            string body = await resp.Content.ReadAsStringAsync();
            if (!resp.IsSuccessStatusCode)
                throw new HttpRequestException("LCU request failed (" + (int)resp.StatusCode + "): " + url);
            return JObject.Parse(body);
        }

        private static async Task<JArray> GetArray(HttpClient client, string url)
        {
            var resp = await client.GetAsync(url);
            string body = await resp.Content.ReadAsStringAsync();
            if (!resp.IsSuccessStatusCode)
                throw new HttpRequestException("LCU request failed (" + (int)resp.StatusCode + "): " + url);
            return JArray.Parse(body);
        }

        private static bool FindLeagueClient(out string port, out string token)
        {
            port = null;
            token = null;

            // Check if the process even exists
            var procs = Process.GetProcessesByName("LeagueClientUx");
            if (procs.Length == 0) return false;

            try
            {
                // Method 1: WMI — most reliable, no special access needed
                try
                {
                    using (var searcher = new ManagementObjectSearcher(
                        "SELECT CommandLine FROM Win32_Process WHERE Name = 'LeagueClientUx.exe'"))
                    {
                        foreach (ManagementObject obj in searcher.Get())
                        {
                            string cmdLine = obj["CommandLine"]?.ToString();
                            if (string.IsNullOrEmpty(cmdLine)) continue;

                            string p = ExtractRegex(cmdLine, @"--app-port=(\d+)");
                            string t = ExtractRegex(cmdLine, @"--remoting-auth-token=([\w-]+)");
                            if (p != null && t != null)
                            {
                                port = p;
                                token = t;
                                return true;
                            }
                        }
                    }
                }
                catch { /* WMI failed, fall through to PEB reading */ }

                // Method 2: PEB memory reading — fallback
                foreach (var proc in procs)
                {
                    try
                    {
                        string cmdLine;
                        int rc = RetrieveCommandLine(proc, out cmdLine);
                        if (rc != 0 || string.IsNullOrEmpty(cmdLine)) continue;

                        string portMatch = ExtractRegex(cmdLine, @"--app-port=(\d+)");
                        string tokenMatch = ExtractRegex(cmdLine, @"--remoting-auth-token=([\w-]+)");
                        if (portMatch != null && tokenMatch != null)
                        {
                            port = portMatch;
                            token = tokenMatch;
                            return true;
                        }
                    }
                    catch { }
                }
                return false;
            }
            finally
            {
                foreach (var p in procs) p.Dispose();
            }
        }

        private static string ExtractRegex(string text, string pattern)
        {
            var m = Regex.Match(text, pattern);
            return m.Success ? m.Groups[1].Value : null;
        }

        #region ProcessCommandLine (PEB reading)

        [DllImport("ntdll.dll")]
        private static extern uint NtQueryInformationProcess(
            IntPtr ProcessHandle, uint ProcessInformationClass,
            IntPtr ProcessInformation, uint ProcessInformationLength, out uint ReturnLength);

        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, uint dwProcessId);

        [DllImport("kernel32.dll")]
        private static extern bool ReadProcessMemory(
            IntPtr hProcess, IntPtr lpBaseAddress, IntPtr lpBuffer, uint nSize, out uint lpNumberOfBytesRead);

        [DllImport("kernel32.dll")]
        private static extern bool CloseHandle(IntPtr hObject);

        private const uint PROCESS_QUERY_INFORMATION = 0x0400;
        private const uint PROCESS_VM_READ = 0x0010;

        [StructLayout(LayoutKind.Sequential)]
        private struct PROCESS_BASIC_INFORMATION
        {
            public IntPtr Reserved1;
            public IntPtr PebBaseAddress;
            public IntPtr Reserved2a;
            public IntPtr Reserved2b;
            public IntPtr UniqueProcessId;
            public IntPtr Reserved3;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct UNICODE_STRING
        {
            public ushort Length;
            public ushort MaximumLength;
            public IntPtr Buffer;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct PEB
        {
            public IntPtr Reserved0;
            public IntPtr Reserved1;
            public IntPtr Reserved2;
            public IntPtr Reserved3;
            public IntPtr ProcessParameters;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RTL_USER_PROCESS_PARAMETERS
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public byte[] Reserved1;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
            public IntPtr[] Reserved2;
            public UNICODE_STRING ImagePathName;
            public UNICODE_STRING CommandLine;
        }

        private static int RetrieveCommandLine(Process process, out string commandLine)
        {
            commandLine = null;
            IntPtr hProcess = OpenProcess(PROCESS_QUERY_INFORMATION | PROCESS_VM_READ, false, (uint)process.Id);
            if (hProcess == IntPtr.Zero) return -1;

            try
            {
                int pbiSize = Marshal.SizeOf<PROCESS_BASIC_INFORMATION>();
                IntPtr pbiMem = Marshal.AllocHGlobal(pbiSize);
                try
                {
                    uint retLen;
                    uint status = NtQueryInformationProcess(hProcess, 0, pbiMem, (uint)pbiSize, out retLen);
                    if (status != 0) return -2;

                    var pbi = Marshal.PtrToStructure<PROCESS_BASIC_INFORMATION>(pbiMem);
                    if (pbi.PebBaseAddress == IntPtr.Zero) return -3;

                    PEB peb;
                    if (!ReadStruct(hProcess, pbi.PebBaseAddress, out peb)) return -4;

                    RTL_USER_PROCESS_PARAMETERS rupp;
                    if (!ReadStruct(hProcess, peb.ProcessParameters, out rupp)) return -5;

                    ushort clLen = rupp.CommandLine.MaximumLength;
                    IntPtr clMem = Marshal.AllocHGlobal(clLen);
                    try
                    {
                        uint bytesRead;
                        if (ReadProcessMemory(hProcess, rupp.CommandLine.Buffer, clMem, clLen, out bytesRead))
                        {
                            commandLine = Marshal.PtrToStringUni(clMem);
                            return 0;
                        }
                        return -6;
                    }
                    finally { Marshal.FreeHGlobal(clMem); }
                }
                finally { Marshal.FreeHGlobal(pbiMem); }
            }
            finally { CloseHandle(hProcess); }
        }

        private static bool ReadStruct<T>(IntPtr hProcess, IntPtr address, out T val) where T : struct
        {
            val = default;
            int size = Marshal.SizeOf<T>();
            IntPtr mem = Marshal.AllocHGlobal(size);
            try
            {
                uint read;
                if (ReadProcessMemory(hProcess, address, mem, (uint)size, out read) && read == (uint)size)
                {
                    val = Marshal.PtrToStructure<T>(mem);
                    return true;
                }
                return false;
            }
            finally { Marshal.FreeHGlobal(mem); }
        }

        #endregion
    }
}
