using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace RiotLogin
{
    public partial class LoginForm : Form
    {
        private AppData appData;
        private List<AccountProfile> profiles;
        private int sortColumn = 2; // Default sort on Level column
        private SortOrder sortOrder = SortOrder.Descending;

        public LoginForm()
        {
            InitializeComponent();
        }

        [DllImport("user32.dll")]
        internal static extern IntPtr SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        internal static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        public string RiotClientProc = "Riot Client", RiotClientUxProc = "RiotClientUx";
        public string RiotClientPath = "";

        private static bool IsProcessRunning(string processName)
        {
            Process[] procs = Process.GetProcessesByName(processName);
            bool running = procs.Length > 0;
            foreach (var p in procs) p.Dispose();
            return running;
        }

        private static IntPtr GetProcessWindowHandle(string processName)
        {
            Process[] procs = Process.GetProcessesByName(processName);
            IntPtr hwnd = IntPtr.Zero;
            foreach (Process pr in procs)
            {
                if (hwnd == IntPtr.Zero && pr.MainWindowHandle != IntPtr.Zero)
                    hwnd = pr.MainWindowHandle;
                pr.Dispose();
            }
            return hwnd;
        }

        private static void FocusWindow(IntPtr hWnd)
        {
            if (hWnd == IntPtr.Zero) return;
            ShowWindow(hWnd, 9); // SW_RESTORE — ensures window isn't minimized
            SetForegroundWindow(hWnd);
        }

        void RunRiotClient()
        {
            var proc = Process.Start(RiotClientPath);
            if (proc != null) proc.Dispose();
        }

        private void LoadData()
        {
            listView1.Items.Clear();

            appData = DataService.Load();
            profiles = appData.Profiles;
            checkBox1.Checked = appData.RememberMe;

            // Auto-detect Riot Client path if not already set
            if (string.IsNullOrEmpty(appData.RiotGamesPath) || !System.IO.File.Exists(appData.RiotGamesPath))
            {
                string detected = DataService.FindRiotClient();
                if (detected != null)
                {
                    appData.RiotGamesPath = detected;
                    DataService.Save(appData);
                }
            }
            RiotClientPath = appData.RiotGamesPath;

            foreach (var p in profiles)
            {
                var item = new ListViewItem(p.RiotID);
                item.SubItems.Add(p.Username);
                item.SubItems.Add(p.Level.ToString());
                item.SubItems.Add(p.Server);
                item.SubItems.Add(p.BE.ToString());
                item.SubItems.Add(p.RP.ToString());
                item.SubItems.Add(p.SoloQ);
                item.SubItems.Add(p.FlexQ);
                item.SubItems.Add(p.Champions.ToString());
                item.SubItems.Add(p.Skins.ToString());
                item.SubItems.Add(p.Loot.ToString());
                listView1.Items.Add(item);
            }

            // Apply current sort
            listView1.ListViewItemSorter = new ListViewItemComparer(sortColumn, sortOrder);
            listView1.Sort();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            IntPtr hWnd = GetProcessWindowHandle("Riot Client");
            if (hWnd == IntPtr.Zero)
                hWnd = GetProcessWindowHandle("RiotClientUx");
            FocusWindow(hWnd);
            LoadData();
        }

        private AccountProfile GetSelectedProfile()
        {
            if (listView1.SelectedItems.Count == 0) return null;
            int idx = listView1.SelectedItems[0].Index;
            if (idx < 0 || idx >= profiles.Count) return null;
            return profiles[idx];
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            var profile = GetSelectedProfile();
            if (profile == null)
            {
                MessageBox.Show("Select account info to log in.", "RiotLogin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // If client isn't running, start it and wait for it
            if (!IsProcessRunning(RiotClientProc) && !IsProcessRunning(RiotClientUxProc))
            {
                if (!string.IsNullOrEmpty(RiotClientPath) && File.Exists(RiotClientPath))
                {
                    RunRiotClient();
                }
                else
                {
                    DialogResult dialogResult = MessageBox.Show("RiotLogin couldn't find RiotClientServices.exe. Would you like to select it manually (Yes) or open Riot Client yourself (No)?", "RiotLogin", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                    if (dialogResult == DialogResult.Yes)
                    {
                        using (OpenFileDialog ofd = new OpenFileDialog())
                        {
                            ofd.Filter = "Executable Files (*.exe)|*.exe";
                            ofd.FileName = "RiotClientServices.exe";
                            if (ofd.ShowDialog() == DialogResult.OK)
                            {
                                RiotClientPath = ofd.FileName;
                                appData.RiotGamesPath = RiotClientPath;
                                DataService.Save(appData);
                                RunRiotClient();
                            }
                            else return;
                        }
                    }
                    else return;
                }

                // Wait up to 30s for the client window to actually appear
                button1.Enabled = false;
                button1.Text = "Starting...";
                IntPtr clientHwnd = IntPtr.Zero;
                for (int i = 0; i < 60; i++) // poll every 500ms, up to 30s
                {
                    await System.Threading.Tasks.Task.Delay(500);
                    clientHwnd = GetProcessWindowHandle(RiotClientProc);
                    if (clientHwnd == IntPtr.Zero)
                        clientHwnd = GetProcessWindowHandle(RiotClientUxProc);
                    if (clientHwnd != IntPtr.Zero)
                    {
                        // Window handle exists — give UI a moment to fully render login fields
                        await System.Threading.Tasks.Task.Delay(3000);
                        break;
                    }
                }
                button1.Enabled = true;
                button1.Text = "Login";
                if (clientHwnd == IntPtr.Zero)
                {
                    MessageBox.Show("Riot Client didn't start in time. Try again.", "RiotLogin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            // Find and focus the client window
            IntPtr hwnd = GetProcessWindowHandle(RiotClientProc);
            if (hwnd == IntPtr.Zero)
                hwnd = GetProcessWindowHandle(RiotClientUxProc);
            if (hwnd == IntPtr.Zero)
            {
                MessageBox.Show("Could not find Riot Client window.", "RiotLogin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            FocusWindow(hwnd);
            await System.Threading.Tasks.Task.Delay(500);

            string username = profile.Username;
            string password = profile.Password;

            // Paste username via clipboard
            Clipboard.SetText(username);
            SendKeys.SendWait("^v");
            await System.Threading.Tasks.Task.Delay(100);

            // Tab to password field
            SendKeys.SendWait("{TAB}");
            await System.Threading.Tasks.Task.Delay(100);

            // Paste password via clipboard
            Clipboard.SetText(password);
            SendKeys.SendWait("^v");
            await System.Threading.Tasks.Task.Delay(100);

            // Tab to "Stay signed in" checkbox in Riot Client
            SendKeys.SendWait("{TAB}");
            await System.Threading.Tasks.Task.Delay(100);

            // Toggle "Stay signed in" if Remember Me is checked
            if (checkBox1.Checked)
            {
                SendKeys.SendWait(" ");
                await System.Threading.Tasks.Task.Delay(100);
            }

            // Submit login
            SendKeys.SendWait("{ENTER}");

            // Clear clipboard so password doesn't linger
            Clipboard.Clear();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            AccountEditorForm editor = new AccountEditorForm();
            editor.ShowDialog();
            LoadData();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (appData == null) return;
            appData.RememberMe = checkBox1.Checked;
            DataService.Save(appData);
        }

        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (e.Column == sortColumn)
                sortOrder = sortOrder == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;
            else
            {
                sortColumn = e.Column;
                sortOrder = SortOrder.Ascending;
            }
            listView1.ListViewItemSorter = new ListViewItemComparer(sortColumn, sortOrder);
            listView1.Sort();
        }
    }

    public class ListViewItemComparer : IComparer
    {
        private readonly int column;
        private readonly SortOrder order;

        public ListViewItemComparer(int column, SortOrder order)
        {
            this.column = column;
            this.order = order;
        }

        public int Compare(object x, object y)
        {
            var itemX = (ListViewItem)x;
            var itemY = (ListViewItem)y;
            string textX = itemX.SubItems[column].Text;
            string textY = itemY.SubItems[column].Text;

            int result;
            // Try numeric comparison for numeric columns
            if (int.TryParse(textX, out int numX) && int.TryParse(textY, out int numY))
                result = numX.CompareTo(numY);
            else
                result = string.Compare(textX, textY, StringComparison.OrdinalIgnoreCase);

            return order == SortOrder.Descending ? -result : result;
        }
    }
}

// coded by lithellx - https://github.com/lithellx
