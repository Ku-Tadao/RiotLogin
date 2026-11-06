using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using Wpf.Ui.Controls;
using RcState = RiotLogin.RiotClientService.State;

namespace RiotLogin
{
    public partial class MainWindow : FluentWindow
    {
        private const string RiotClientProc = "Riot Client", RiotClientUxProc = "RiotClientUx", LeagueGameProc = "League of Legends";
        private AppData appData;
        private bool busy;

        public MainWindow()
        {
            InitializeComponent();
            appData = DataService.Load();
            ApplySavedBounds(appData.Window);
        }

        [DllImport("user32.dll")] private static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")] private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        [DllImport("user32.dll")] private static extern void keybd_event(byte vk, byte scan, uint flags, UIntPtr extra);

        private const byte VK_TAB = 0x09, VK_RETURN = 0x0D, VK_CONTROL = 0x11, VK_V = 0x56;

        // Presses the keys together (e.g. Ctrl+V) and releases them in reverse order.
        private static void Press(params byte[] keys)
        {
            foreach (var k in keys) keybd_event(k, 0, 0, UIntPtr.Zero);
            for (int i = keys.Length - 1; i >= 0; i--) keybd_event(keys[i], 0, 2 /* KEYUP */, UIntPtr.Zero);
        }

        private static bool IsProcessRunning(string name)
        {
            var procs = Process.GetProcessesByName(name);
            foreach (var p in procs) p.Dispose();
            return procs.Length > 0;
        }

        private static IntPtr GetProcessWindowHandle(string name)
        {
            IntPtr hwnd = IntPtr.Zero;
            foreach (var p in Process.GetProcessesByName(name))
            {
                if (hwnd == IntPtr.Zero && p.MainWindowHandle != IntPtr.Zero) hwnd = p.MainWindowHandle;
                p.Dispose();
            }
            return hwnd;
        }

        private static IntPtr FindClientWindow()
        {
            var hwnd = GetProcessWindowHandle(RiotClientProc);
            return hwnd != IntPtr.Zero ? hwnd : GetProcessWindowHandle(RiotClientUxProc);
        }

        private void RunRiotClient() => Process.Start(new ProcessStartInfo(appData.RiotGamesPath) { UseShellExecute = true })?.Dispose();

        // ---- window state ----

        private void ApplySavedBounds(double[] b)
        {
            if (b == null || b.Length != 4) return;
            var screen = new Rect(SystemParameters.VirtualScreenLeft, SystemParameters.VirtualScreenTop,
                                  SystemParameters.VirtualScreenWidth, SystemParameters.VirtualScreenHeight);
            var saved = new Rect(b[0], b[1], Math.Max(b[2], MinWidth), Math.Max(b[3], MinHeight));
            if (!screen.IntersectsWith(saved)) return; // monitor gone: fall back to centred
            WindowStartupLocation = WindowStartupLocation.Manual;
            Left = saved.Left; Top = saved.Top; Width = saved.Width; Height = saved.Height;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (appData == null) return;
            var r = WindowState == WindowState.Normal ? new Rect(Left, Top, Width, Height) : RestoreBounds;
            appData.Window = new[] { r.Left, r.Top, r.Width, r.Height };
            if (grid.SelectedItem is AccountProfile p) appData.LastAccount = p.Username;
            DataService.Save(appData);
        }

        private void LoadData()
        {
            string keep = (grid.SelectedItem as AccountProfile)?.Username ?? appData?.LastAccount;
            appData = DataService.Load();

            // Auto-detect Riot Client path if not already set
            if (string.IsNullOrEmpty(appData.RiotGamesPath) || !File.Exists(appData.RiotGamesPath))
            {
                string detected = DataService.FindRiotClient();
                if (detected != null)
                {
                    appData.RiotGamesPath = detected;
                    DataService.Save(appData);
                }
            }

            grid.ItemsSource = appData.Profiles;
            if (grid.Items.SortDescriptions.Count == 0)
            {
                grid.Items.SortDescriptions.Add(new SortDescription(nameof(AccountProfile.Level), ListSortDirection.Descending));
                grid.Columns[2].SortDirection = ListSortDirection.Descending;
            }
            ApplyFilter();

            grid.SelectedItem = appData.Profiles.FirstOrDefault(p => p.Username == keep);
            if (grid.SelectedItem != null) grid.ScrollIntoView(grid.SelectedItem);
        }

        // Do NOT focus the Riot Client on startup: it steals focus from our own window.
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();
            grid.Focus();
        }

        // ---- search & keyboard ----

        private void ApplyFilter()
        {
            string q = search.Text.Trim();
            grid.Items.Filter = q.Length == 0 ? null : o => o is AccountProfile p &&
                (p.RiotID.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                 p.Username.Contains(q, StringComparison.OrdinalIgnoreCase));
        }

        private void Search_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            ApplyFilter();
            if (grid.SelectedItem == null && grid.Items.Count > 0) grid.SelectedIndex = 0;
        }

        private void Search_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) { search.Text = ""; grid.Focus(); e.Handled = true; }
            else if (e.Key == Key.Enter) { Login_Click(loginButton, null); e.Handled = true; }
            else if (e.Key == Key.Down) { grid.Focus(); e.Handled = true; }
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F && Keyboard.Modifiers == ModifierKeys.Control)
            {
                search.Focus();
                search.SelectAll();
                e.Handled = true;
            }
        }

        private void Grid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) { Login_Click(loginButton, null); e.Handled = true; }
        }

        private void Grid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) => SetBusy(null);

        // ---- login ----

        // null = idle. Otherwise shows a spinner and the step being performed.
        private void SetBusy(string step)
        {
            busy = step != null;
            busyRing.Visibility = busy ? Visibility.Visible : Visibility.Collapsed;
            statusText.Text = step ?? "";
            loginButton.IsEnabled = !busy && grid.SelectedItem != null;
            fetchButton.IsEnabled = !busy;
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            if (busy || !(grid.SelectedItem is AccountProfile profile)) return;
            try { await PerformLoginAsync(profile); }
            finally { SetBusy(null); }
        }

        private async Task PerformLoginAsync(AccountProfile profile)
        {
            // If client isn't running, start it and wait for it
            if (!IsProcessRunning(RiotClientProc) && !IsProcessRunning(RiotClientUxProc))
            {
                if (string.IsNullOrEmpty(appData.RiotGamesPath) || !File.Exists(appData.RiotGamesPath))
                {
                    if (!await App.Confirm("RiotLogin couldn't find RiotClientServices.exe. Select it manually? (No = open Riot Client yourself)"))
                        return;
                    var ofd = new OpenFileDialog { Filter = "Executable Files (*.exe)|*.exe", FileName = "RiotClientServices.exe" };
                    if (ofd.ShowDialog(this) != true) return;
                    appData.RiotGamesPath = ofd.FileName;
                    DataService.Save(appData);
                }
                SetBusy("Starting Riot Client…");
                RunRiotClient();

                // Wait up to 30s for the client window to actually appear
                IntPtr clientHwnd = IntPtr.Zero;
                for (int i = 0; i < 60 && clientHwnd == IntPtr.Zero; i++)
                {
                    await Task.Delay(500);
                    clientHwnd = FindClientWindow();
                }
                if (clientHwnd == IntPtr.Zero)
                {
                    App.Notify(info, "Riot Client didn't start in time. Try again.", InfoBarSeverity.Warning);
                    return;
                }
            }

            // Ask the Riot Client's local API what state it's in. Unavailable = old client or no lockfile:
            // fall back to blind typing like before.
            SetBusy("Checking Riot Client…");
            var state = await WaitForState(s => s != RcState.Unavailable, 20);
            bool api = state != RcState.Unavailable;

            if (state == RcState.SignedIn)
            {
                string current = await RiotClientService.GetSignedInPuuidAsync();
                if (!string.IsNullOrEmpty(profile.Puuid) && current == profile.Puuid)
                {
                    await LaunchLeague(profile, alreadySignedIn: true);
                    return;
                }
                // Logging out closes League too, so never do it under a running game.
                if (IsProcessRunning(LeagueGameProc))
                {
                    App.Notify(info, "Another account is in a game. Finish it before switching.", InfoBarSeverity.Warning);
                    return;
                }
                SetBusy("Signing out the current account…");
                await RiotClientService.LogoutAsync();
                state = await WaitForState(s => s != RcState.SignedIn, 15);
                if (state == RcState.SignedIn)
                {
                    App.Notify(info, "Couldn't sign out the current account.", InfoBarSeverity.Error);
                    return;
                }
            }

            SetBusy("Waiting for the login screen…");
            IntPtr hwnd = IntPtr.Zero;
            for (int i = 0; i < 60 && hwnd == IntPtr.Zero; i++)
            {
                hwnd = FindClientWindow();
                if (hwnd == IntPtr.Zero) await Task.Delay(500);
            }
            if (hwnd == IntPtr.Zero)
            {
                App.Notify(info, "Could not find the Riot Client window.", InfoBarSeverity.Warning);
                return;
            }
            await Task.Delay(api ? 1500 : 3000); // let the login fields render
            SetBusy("Typing credentials…");
            ShowWindow(hwnd, 9); // SW_RESTORE
            SetForegroundWindow(hwnd);
            await Task.Delay(500);

            // Preserve whatever the user had on the clipboard; restore it when done.
            string savedClipboard = null;
            try { if (Clipboard.ContainsText()) savedClipboard = Clipboard.GetText(); } catch { }

            try
            {
                App.Copy(profile.Username);
                Press(VK_CONTROL, VK_V);
                await Task.Delay(120);

                Press(VK_TAB);
                await Task.Delay(120);

                App.Copy(profile.Password);
                Press(VK_CONTROL, VK_V);
                await Task.Delay(120);

                // Press Enter to submit. DO NOT tab from here: the next focusable elements in the
                // Riot Client are the social login buttons. Enter submits regardless of focus.
                Press(VK_RETURN);
            }
            finally
            {
                // Always wipe the password from the clipboard, and restore what was there before.
                App.Copy(savedClipboard);
            }

            appData.LastAccount = profile.Username;
            if (!api)
            {
                App.Notify(info, $"Sent login for {Label(profile)}.", InfoBarSeverity.Success);
                return;
            }

            SetBusy("Signing in…");
            bool mfaShown = false;
            // ponytail: an error left over from an earlier attempt could read as "rejected"; the 4 s grace covers the usual case.
            for (int i = 0; i < 240; i++) // 2 min, enough to type a 2FA code
            {
                await Task.Delay(500);
                state = await RiotClientService.GetStateAsync();
                if (state == RcState.SignedIn) { await LaunchLeague(profile, alreadySignedIn: false); return; }
                if (state == RcState.MultiFactor && !mfaShown)
                {
                    mfaShown = true;
                    SetBusy("Waiting for your 2FA code…");
                    App.Notify(info, "Enter the verification code in the Riot Client.", InfoBarSeverity.Warning);
                }
                if (state == RcState.Failed && i >= 8)
                {
                    App.Notify(info, $"Riot rejected the login for {Label(profile)}. Check the username and password.", InfoBarSeverity.Error);
                    return;
                }
                if (!mfaShown && i >= 60) break; // 30 s with no answer
            }
            App.Notify(info, "Login didn't finish. Check the Riot Client.", InfoBarSeverity.Warning);
        }

        private static string Label(AccountProfile p) => string.IsNullOrEmpty(p.RiotID) ? p.Username : p.RiotID;

        private static async Task<RcState> WaitForState(Func<RcState, bool> done, int seconds)
        {
            var s = RcState.Unavailable;
            for (int i = 0; i < seconds * 2; i++)
            {
                s = await RiotClientService.GetStateAsync();
                if (done(s)) break;
                await Task.Delay(500);
            }
            return s;
        }

        private async Task LaunchLeague(AccountProfile profile, bool alreadySignedIn)
        {
            if (string.IsNullOrEmpty(profile.Puuid))
            {
                profile.Puuid = await RiotClientService.GetSignedInPuuidAsync() ?? "";
                DataService.Save(appData);
            }
            string who = alreadySignedIn ? $"{Label(profile)} is already signed in" : $"Signed in as {Label(profile)}";
            if (await RiotClientService.IsLeagueRunningAsync())
            {
                App.Notify(info, who + ".", InfoBarSeverity.Success);
                return;
            }
            SetBusy("Launching League…");
            bool ok = await RiotClientService.LaunchLeagueAsync();
            App.Notify(info, ok ? who + ". Launching League." : who + ", but League didn't launch. Press Play in the Riot Client.",
                       ok ? InfoBarSeverity.Success : InfoBarSeverity.Warning);
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            new EditorWindow { Owner = this }.ShowDialog();
            LoadData();
        }

        // Refreshes public data (level, icon, ranks, peak) for every account from op.gg. No login needed.
        private async void FetchAll_Click(object sender, RoutedEventArgs e)
        {
            if (busy) return;
            var list = appData.Profiles.ToList();
            var failed = new System.Collections.Generic.List<string>();
            try
            {
                for (int i = 0; i < list.Count; i++)
                {
                    SetBusy($"op.gg {i + 1}/{list.Count}: {Label(list[i])}…");
                    if (await OpggService.FetchAsync(list[i]) is string err) failed.Add($"{Label(list[i])} ({err})");
                }
            }
            finally
            {
                DataService.Save(appData);
                grid.Items.Refresh();
                SetBusy(null);
            }
            int ok = list.Count - failed.Count;
            App.Notify(info, failed.Count == 0 ? $"Updated all {ok} accounts from op.gg."
                : $"Updated {ok} of {list.Count}. Skipped: {string.Join(", ", failed)}",
                failed.Count == 0 ? InfoBarSeverity.Success : InfoBarSeverity.Warning);
        }
    }
}

// coded by lithellx - https://github.com/lithellx
