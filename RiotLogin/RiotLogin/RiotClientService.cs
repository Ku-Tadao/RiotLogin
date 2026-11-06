using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RiotLogin
{
    /// <summary>
    /// Local Riot Client API (not the League Client). Port and password come from the client's lockfile:
    /// "Riot Client:pid:port:password:https". Used to check login state, log out, and launch League.
    /// </summary>
    public static class RiotClientService
    {
        private static readonly string LockfilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Riot Games", "Riot Client", "Config", "lockfile");

        private static readonly HttpClient Http = new HttpClient(new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (s, c, ch, e) => true // self-signed localhost cert
        }) { Timeout = TimeSpan.FromSeconds(5) };

        public enum State { Unavailable, SignedOut, MultiFactor, SignedIn, Failed }

        private static HttpRequestMessage Request(HttpMethod method, string path)
        {
            string[] parts;
            try
            {
                // The client keeps the file open, so share read/write.
                using var fs = new FileStream(LockfilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
                parts = new StreamReader(fs).ReadToEnd().Split(':');
            }
            catch { return null; }
            if (parts.Length < 5) return null;

            var req = new HttpRequestMessage(method, $"https://127.0.0.1:{parts[2]}{path}");
            req.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes("riot:" + parts[3])));
            req.Headers.Add("Accept", "application/json");
            return req;
        }

        private static async Task<JObject> Get(string path)
        {
            var req = Request(HttpMethod.Get, path);
            if (req == null) return null;
            try
            {
                using var resp = await Http.SendAsync(req);
                if (!resp.IsSuccessStatusCode) return null;
                return JObject.Parse(await resp.Content.ReadAsStringAsync());
            }
            catch { return null; }
        }

        private static async Task<bool> Send(HttpMethod method, string path, string json = null)
        {
            var req = Request(method, path);
            if (req == null) return false;
            if (json != null) req.Content = new StringContent(json, Encoding.UTF8, "application/json");
            try
            {
                using var resp = await Http.SendAsync(req);
                return resp.IsSuccessStatusCode;
            }
            catch { return false; }
        }

        public static async Task<State> GetStateAsync()
        {
            var session = await Get("/rso-auth/v1/session");
            if (session == null) return State.Unavailable;
            if ((string)session["type"] == "authenticated") return State.SignedIn;

            var auth = await Get("/rso-authenticator/v1/authentication");
            string type = (string)auth?["type"];
            if (type == "multifactor") return State.MultiFactor;
            if (type == "success") return State.SignedIn;
            if (!string.IsNullOrEmpty((string)auth?["error"])) return State.Failed;
            return State.SignedOut;
        }

        /// <summary>Puuid of the signed-in account, or null.</summary>
        public static async Task<string> GetSignedInPuuidAsync() => (string)(await Get("/rso-auth/v1/authorization"))?["subject"];

        public static Task<bool> LogoutAsync() => Send(HttpMethod.Delete, "/rso-auth/v1/session");

        public static Task<bool> LaunchLeagueAsync() =>
            Send(HttpMethod.Post, "/product-launcher/v1/products/league_of_legends/patchlines/live", "{}");

        public static async Task<bool> IsLeagueRunningAsync()
        {
            var sessions = await Get("/product-session/v1/sessions");
            if (sessions == null) return false;
            foreach (var s in sessions.Properties())
                if ((string)s.Value["productId"] == "league_of_legends") return true;
            return false;
        }
    }
}
