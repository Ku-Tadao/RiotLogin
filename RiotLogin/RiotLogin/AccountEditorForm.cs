using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RiotLogin
{
    public partial class AccountEditorForm : Form
    {
        private AppData appData;
        private List<AccountProfile> profiles;

        public AccountEditorForm()
        {
            InitializeComponent();
        }

        private void LoadData()
        {
            listView1.Items.Clear();

            appData = DataService.Load();
            profiles = appData.Profiles;

            foreach (var p in profiles)
            {
                var item = new ListViewItem(p.RiotID);
                item.SubItems.Add(p.Username);
                item.SubItems.Add(p.Password);
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
        }

        private void AccountEditorForm_Load(object sender, EventArgs e)
        {
            listView1.FullRowSelect = true;
            listView1.MultiSelect = false;
            LoadData();
        }

        private void ClearInputs()
        {
            txtRiotID.Text = "";
            txtUsername.Text = "";
            txtPassword.Text = "";
            numLevel.Value = 0;
            txtServer.Text = "";
            numBE.Value = 0;
            numRP.Value = 0;
            txtSoloQ.Text = "";
            txtFlexQ.Text = "";
            numChampions.Value = 0;
            numSkins.Value = 0;
            numLoot.Value = 0;
        }

        private void PopulateInputs(AccountProfile p)
        {
            txtRiotID.Text = p.RiotID;
            txtUsername.Text = p.Username;
            txtPassword.Text = p.Password;
            numLevel.Value = Math.Max(numLevel.Minimum, Math.Min(numLevel.Maximum, p.Level));
            txtServer.Text = p.Server;
            numBE.Value = Math.Max(numBE.Minimum, Math.Min(numBE.Maximum, p.BE));
            numRP.Value = Math.Max(numRP.Minimum, Math.Min(numRP.Maximum, p.RP));
            txtSoloQ.Text = p.SoloQ;
            txtFlexQ.Text = p.FlexQ;
            numChampions.Value = Math.Max(numChampions.Minimum, Math.Min(numChampions.Maximum, p.Champions));
            numSkins.Value = Math.Max(numSkins.Minimum, Math.Min(numSkins.Maximum, p.Skins));
            numLoot.Value = Math.Max(numLoot.Minimum, Math.Min(numLoot.Maximum, p.Loot));
        }

        private AccountProfile ReadInputs()
        {
            return new AccountProfile
            {
                RiotID = txtRiotID.Text,
                Username = txtUsername.Text,
                Password = txtPassword.Text,
                Level = (int)numLevel.Value,
                Server = txtServer.Text,
                BE = (int)numBE.Value,
                RP = (int)numRP.Value,
                SoloQ = txtSoloQ.Text,
                FlexQ = txtFlexQ.Text,
                Champions = (int)numChampions.Value,
                Skins = (int)numSkins.Value,
                Loot = (int)numLoot.Value
            };
        }

        // Save (update selected)
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
            {
                MessageBox.Show("Select a row to edit.", "RiotLogin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int idx = listView1.SelectedItems[0].Index;
            if (idx < 0 || idx >= profiles.Count) return;

            var existing = profiles[idx];
            var updated = ReadInputs();

            // Preserve detail lists that aren't editable in the form
            updated.ChampionList = existing.ChampionList;
            updated.SkinList = existing.SkinList;
            updated.LootList = existing.LootList;

            profiles[idx] = updated;
            DataService.Save(appData);
            LoadData();
        }

        // Add
        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Username and Password are required.", "RiotLogin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var profile = ReadInputs();
            profiles.Add(profile);
            DataService.Save(appData);
            LoadData();
            ClearInputs();
        }

        // Delete
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
            {
                MessageBox.Show("Select a row to delete.", "RiotLogin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int idx = listView1.SelectedItems[0].Index;
            if (idx < 0 || idx >= profiles.Count) return;

            var result = MessageBox.Show(
                "Delete account \"" + profiles[idx].RiotID + "\" (" + profiles[idx].Username + ")?",
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result != DialogResult.Yes) return;

            profiles.RemoveAt(idx);
            DataService.Save(appData);
            LoadData();
            ClearInputs();
        }

        // Pull Data from League Client
        private async void btnPullData_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
            {
                MessageBox.Show("Select an account to pull data for.", "RiotLogin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int idx = listView1.SelectedItems[0].Index;
            if (idx < 0 || idx >= profiles.Count) return;

            btnPullData.Enabled = false;
            btnPullData.Text = "Pulling...";
            try
            {
                profiles[idx] = await LcuService.PullDataAsync(profiles[idx]);
                DataService.Save(appData);
                LoadData();

                // Re-select and populate
                if (idx < listView1.Items.Count)
                {
                    listView1.Items[idx].Selected = true;
                    PopulateInputs(profiles[idx]);
                }

                MessageBox.Show("Data pulled successfully!", "RiotLogin", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to pull data: " + ex.Message, "RiotLogin", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnPullData.Enabled = true;
                btnPullData.Text = "Pull Data";
            }
        }

        // Double-click to populate inputs
        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0) return;
            int idx = listView1.SelectedItems[0].Index;
            if (idx < 0 || idx >= profiles.Count) return;
            PopulateInputs(profiles[idx]);
        }
    }
}
