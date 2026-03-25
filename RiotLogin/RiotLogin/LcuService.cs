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

                // Champions
                var champs = await GetArray(client, baseUrl + "/lol-champions/v1/inventories/" + summonerId + "/champions-minimal");
                int champCount = 0;
                string champList = "";
                foreach (var c in champs)
                {
                    if (c["ownership"] != null && (bool)c["ownership"]["owned"])
                    {
                        champList += ":" + (string)c["name"];
                        champCount++;
                    }
                }
                profile.Champions = champCount;
                profile.ChampionList = champList;

                // Skins
                var skins = await GetArray(client, baseUrl + "/lol-catalog/v1/items/CHAMPION_SKIN");
                int skinCount = 0;
                string skinList = "";
                foreach (var s in skins)
                {
                    if ((bool)s["owned"])
                    {
                        skinList += ":" + (string)s["name"];
                        skinCount++;
                    }
                }
                profile.Skins = skinCount;
                profile.SkinList = skinList;

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
            }

            return profile;
        }

        private static string FormatRank(JObject ranked, string queue)
        {
            try
            {
                var q = ranked["queueMap"][queue];
                string tier = (string)q["tier"];
                if (string.IsNullOrEmpty(tier)) return "Unranked";
                return tier + " " + (string)q["division"] + " " + (string)q["leaguePoints"] + " LP, " +
                       (string)q["wins"] + " Wins, " + (string)q["losses"] + " Losses";
            }
            catch
            {
                return "Unranked";
            }
        }

        private static async Task<JObject> GetJson(HttpClient client, string url)
        {
            var resp = await client.GetAsync(url);
            string body = await resp.Content.ReadAsStringAsync();
            return JObject.Parse(body);
        }

        private static async Task<JArray> GetArray(HttpClient client, string url)
        {
            var resp = await client.GetAsync(url);
            string body = await resp.Content.ReadAsStringAsync();
            return JArray.Parse(body);
        }

        private static bool FindLeagueClient(out string port, out string token)
        {
            port = null;
            token = null;

            // Check if the process even exists
            var procs = Process.GetProcessesByName("LeagueClientUx");
            if (procs.Length == 0) return false;

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
