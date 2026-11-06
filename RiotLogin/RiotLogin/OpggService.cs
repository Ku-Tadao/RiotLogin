using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RiotLogin
{
    /// <summary>
    /// Public rank data from op.gg's MCP server (no key, no login). Only fills what op.gg can see:
    /// level, icon, SoloQ/FlexQ and this season's peak. BE, RP, skins, honor and bans still need Pull Data.
    /// </summary>
    public static class OpggService
    {
        private static readonly HttpClient Http = new HttpClient { Timeout = TimeSpan.FromSeconds(20) };

        private static readonly string[] Fields =
        {
            "data.summoner.{level,profile_image_url}",
            "data.summoner.league_stats[].tier_info.{tier,division,lp}",
            "data.summoner.league_stats[].{game_type,win,lose}",
            "data.summoner.current_season_high_tiers.rank_entries[].game_type",
            "data.summoner.current_season_high_tiers.rank_entries[].high_rank_info.{tier,division}",
        };

        /// <summary>Updates the profile in place. Returns null on success, otherwise why it failed.</summary>
        public static async Task<string> FetchAsync(AccountProfile p)
        {
            var id = p.RiotID.Split('#');
            if (id.Length != 2 || id[0].Trim() == "" || id[1].Trim() == "") return "no Riot ID";
            if (string.IsNullOrWhiteSpace(p.Server)) return "no server";

            var body = new JObject
            {
                ["jsonrpc"] = "2.0", ["id"] = 1, ["method"] = "tools/call",
                ["params"] = new JObject
                {
                    ["name"] = "lol_get_summoner_profile",
                    ["arguments"] = new JObject
                    {
                        ["game_name"] = id[0].Trim(), ["tag_line"] = id[1].Trim(),
                        ["region"] = p.Server.Trim().ToLowerInvariant(), ["desired_output_fields"] = new JArray(Fields),
                    }
                }
            };
            var req = new HttpRequestMessage(HttpMethod.Post, "https://mcp-api.op.gg/mcp")
            {
                Content = new StringContent(body.ToString(), Encoding.UTF8, "application/json")
            };
            req.Headers.Add("Accept", "application/json, text/event-stream");

            JObject resp;
            try
            {
                using var r = await Http.SendAsync(req);
                resp = JObject.Parse(await r.Content.ReadAsStringAsync());
            }
            catch (Exception ex) { return "op.gg unreachable (" + ex.GetType().Name + ")"; }

            if (resp["error"] != null) return (string)resp["error"]["message"] ?? "error";
            string text = (string)resp["result"]?["content"]?[0]?["text"];
            var s = Parse(text)?["data"]?["summoner"];
            if (s == null) return "unexpected op.gg response";

            if ((int?)s["level"] is int level) p.Level = level;
            var icon = Regex.Match((string)s["profile_image_url"] ?? "", @"profileIcon(\d+)");
            if (icon.Success) p.ProfileIconId = int.Parse(icon.Groups[1].Value);

            JToken Queue(string type) => s["league_stats"]?.FirstOrDefault(q => (string)q["game_type"] == type);
            p.SoloQ = Rank(Queue("SOLORANKED"));
            p.FlexQ = Rank(Queue("FLEXRANKED"));

            var peak = s["current_season_high_tiers"]?["rank_entries"]?
                .FirstOrDefault(e => (string)e["game_type"] == "SOLORANKED")?["high_rank_info"];
            if (Tier(peak) is string pk) p.PeakRank = pk; // keep an older peak (e.g. last season's) when op.gg has none
            return null;
        }

        private static string Tier(JToken t)
        {
            string tier = (string)t?["tier"];
            if (string.IsNullOrEmpty(tier)) return null;
            tier = tier.Substring(0, 1) + tier.Substring(1).ToLowerInvariant();
            if (tier is "Master" or "Grandmaster" or "Challenger") return tier;
            int d = (int?)t["division"] ?? 0;
            return d is >= 1 and <= 4 ? tier + " " + new[] { "I", "II", "III", "IV" }[d - 1] : tier;
        }

        private static string Rank(JToken q)
        {
            string tier = Tier(q?["tier_info"]);
            if (tier == null) return "Unranked";
            return $"{tier} {(int?)q["tier_info"]["lp"] ?? 0} LP ({(int?)q["win"] ?? 0}W {(int?)q["lose"] ?? 0}L)";
        }

        // op.gg answers in a compact form: a "class Name: field,field" header, then values written as
        // Name(v1,v2,[...]) with JSON strings/numbers/null. This turns that into a JObject tree.
        internal static JToken Parse(string text)
        {
            if (string.IsNullOrEmpty(text)) return null;
            var classes = new Dictionary<string, string[]>();
            int i = 0;
            while (string.CompareOrdinal(text, i, "class ", 0, 6) == 0)
            {
                int end = text.IndexOf('\n', i);
                if (end < 0) return null;
                var line = text.Substring(i + 6, end - i - 6);
                int colon = line.IndexOf(':');
                classes[line.Substring(0, colon).Trim()] = line.Substring(colon + 1).Split(',').Select(f => f.Trim()).ToArray();
                i = end + 1;
            }
            try { return Value(text, ref i, classes); }
            catch { return null; }
        }

        private static JToken Value(string s, ref int i, Dictionary<string, string[]> classes)
        {
            while (char.IsWhiteSpace(s[i])) i++;
            char c = s[i];
            if (c == '"')
            {
                int start = i++;
                while (s[i] != '"') i += s[i] == '\\' ? 2 : 1;
                i++;
                return JToken.Parse(s.Substring(start, i - start));
            }
            if (c == '[')
            {
                i++;
                return new JArray(List(s, ref i, ']', classes));
            }
            int st = i;
            while (i < s.Length && (char.IsLetterOrDigit(s[i]) || s[i] is '_' or '.' or '-' or '+')) i++;
            string word = s.Substring(st, i - st);
            if (i < s.Length && s[i] == '(')
            {
                i++;
                var vals = List(s, ref i, ')', classes);
                if (!classes.TryGetValue(word, out var names)) return new JArray(vals);
                var o = new JObject();
                for (int k = 0; k < names.Length && k < vals.Count; k++) o[names[k]] = vals[k];
                return o;
            }
            return JToken.Parse(word); // number, null, true, false
        }

        private static List<JToken> List(string s, ref int i, char close, Dictionary<string, string[]> classes)
        {
            var items = new List<JToken>();
            while (true)
            {
                while (char.IsWhiteSpace(s[i])) i++;
                if (s[i] == close) { i++; return items; }
                items.Add(Value(s, ref i, classes));
                while (char.IsWhiteSpace(s[i])) i++;
                if (s[i] == ',') i++;
            }
        }
    }
}
