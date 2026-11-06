using System;
using System.Windows;
using System.Windows.Controls;
using Wpf.Ui.Controls;

namespace RiotLogin
{
    public partial class EditorWindow : FluentWindow
    {
        private AppData appData;
        private bool pulling;

        public EditorWindow()
        {
            InitializeComponent();
            Reload();
            UpdateState();
        }

        private AccountProfile Selected => grid.SelectedItem as AccountProfile;

        // Profiles are reloaded from disk, so reselect by position in the saved list.
        private void Reload(int select = -1)
        {
            appData = DataService.Load();
            grid.ItemsSource = appData.Profiles;
            if (select >= 0 && select < appData.Profiles.Count)
            {
                grid.SelectedItem = appData.Profiles[select];
                grid.ScrollIntoView(grid.SelectedItem);
            }
        }

        private void ClearInputs() => Populate(new AccountProfile());

        private void Populate(AccountProfile p)
        {
            txtRiotID.Text = p.RiotID;
            txtUsername.Text = p.Username;
            txtPassword.Password = p.Password;
            txtServer.Text = p.Server;
            txtSoloQ.Text = p.SoloQ;
            txtFlexQ.Text = p.FlexQ;
            numLevel.Value = p.Level;
            numBE.Value = p.BE;
            numRP.Value = p.RP;
            numChampions.Value = p.Champions;
            numSkins.Value = p.Skins;
            numLoot.Value = p.Loot;
        }

        private static int Int(NumberBox n) => (int)Math.Round(n.Value ?? 0);

        private AccountProfile ReadInputs() => new AccountProfile
        {
            RiotID = txtRiotID.Text,
            Username = txtUsername.Text,
            Password = txtPassword.Password,
            Server = txtServer.Text,
            SoloQ = txtSoloQ.Text,
            FlexQ = txtFlexQ.Text,
            Level = Int(numLevel),
            BE = Int(numBE),
            RP = Int(numRP),
            Champions = Int(numChampions),
            Skins = Int(numSkins),
            Loot = Int(numLoot),
        };

        private void Grid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Selected != null) Populate(Selected);
            UpdateState();
        }

        private void Input_Changed(object sender, RoutedEventArgs e) => UpdateState();

        // Buttons are only enabled when they can do something, instead of erroring on click.
        private void UpdateState()
        {
            var p = Selected;
            formHeader.Text = p == null ? "New account" : $"Editing {(string.IsNullOrEmpty(p.RiotID) ? p.Username : p.RiotID)}";
            btnSave.IsEnabled = btnDelete.IsEnabled = p != null;
            btnPullData.IsEnabled = p != null && !pulling;
            btnPullData.ToolTip = p == null ? "Select an account first" : "Fill the selected account from the running League Client";
            btnAdd.IsEnabled = !string.IsNullOrWhiteSpace(txtUsername.Text) && !string.IsNullOrWhiteSpace(txtPassword.Password);
        }

        private void New_Click(object sender, RoutedEventArgs e)
        {
            grid.SelectedItem = null;
            ClearInputs();
            UpdateState();
            txtRiotID.Focus();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var existing = Selected;
            if (existing == null) return;

            var updated = ReadInputs();
            // Preserve detail lists that aren't editable in the form
            updated.ChampionList = existing.ChampionList;
            updated.SkinList = existing.SkinList;
            updated.LootList = existing.LootList;
            updated.Puuid = existing.Puuid;
            updated.ProfileIconId = existing.ProfileIconId;
            updated.Status = existing.Status;
            updated.PeakRank = existing.PeakRank;
            updated.Honor = existing.Honor;
            updated.Mastery = existing.Mastery;

            int idx = appData.Profiles.IndexOf(existing);
            appData.Profiles[idx] = updated;
            DataService.Save(appData);
            Reload(idx);
            App.Notify(info, "Saved.", InfoBarSeverity.Success);
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (!btnAdd.IsEnabled) return;
            appData.Profiles.Add(ReadInputs());
            DataService.Save(appData);
            Reload(appData.Profiles.Count - 1);
            App.Notify(info, "Account added.", InfoBarSeverity.Success);
        }

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            var p = Selected;
            if (p == null) return;
            if (!await App.Confirm($"Delete account \"{p.RiotID}\" ({p.Username})?")) return;

            appData.Profiles.Remove(p);
            DataService.Save(appData);
            Reload();
            ClearInputs();
            UpdateState();
            App.Notify(info, $"Deleted {p.Username}.", InfoBarSeverity.Success);
        }

        private async void PullData_Click(object sender, RoutedEventArgs e)
        {
            var p = Selected;
            if (p == null || pulling) return;

            int idx = appData.Profiles.IndexOf(p);
            pulling = true;
            btnPullData.IsEnabled = false;
            btnPullData.Content = "Pulling...";
            try
            {
                appData.Profiles[idx] = await LcuService.PullDataAsync(p);
                DataService.Save(appData);
                Reload(idx);
                App.Notify(info, "Data pulled from the League Client.", InfoBarSeverity.Success);
            }
            catch (Exception ex)
            {
                App.Notify(info, "Failed to pull data: " + ex.Message, InfoBarSeverity.Error);
            }
            finally
            {
                pulling = false;
                btnPullData.Content = "Pull Data";
                UpdateState();
            }
        }

        private void CopyPassword_Click(object sender, RoutedEventArgs e) => App.Copy(txtPassword.Password);
    }
}
