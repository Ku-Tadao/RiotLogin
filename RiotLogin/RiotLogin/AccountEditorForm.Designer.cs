namespace RiotLogin
{
    partial class AccountEditorForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AccountEditorForm));
            this.listView1 = new System.Windows.Forms.ListView();
            this.colRiotID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colUsername = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colPassword = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colLevel = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colServer = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colBE = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colRP = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colSoloQ = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colFlexQ = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colChampions = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colSkins = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colLoot = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lblRiotID = new System.Windows.Forms.Label();
            this.txtRiotID = new System.Windows.Forms.TextBox();
            this.lblUsername = new System.Windows.Forms.Label();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.lblPassword = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.lblLevel = new System.Windows.Forms.Label();
            this.numLevel = new System.Windows.Forms.NumericUpDown();
            this.lblServer = new System.Windows.Forms.Label();
            this.txtServer = new System.Windows.Forms.TextBox();
            this.lblBE = new System.Windows.Forms.Label();
            this.numBE = new System.Windows.Forms.NumericUpDown();
            this.lblRP = new System.Windows.Forms.Label();
            this.numRP = new System.Windows.Forms.NumericUpDown();
            this.lblSoloQ = new System.Windows.Forms.Label();
            this.txtSoloQ = new System.Windows.Forms.TextBox();
            this.lblFlexQ = new System.Windows.Forms.Label();
            this.txtFlexQ = new System.Windows.Forms.TextBox();
            this.lblChampions = new System.Windows.Forms.Label();
            this.numChampions = new System.Windows.Forms.NumericUpDown();
            this.lblSkins = new System.Windows.Forms.Label();
            this.numSkins = new System.Windows.Forms.NumericUpDown();
            this.lblLoot = new System.Windows.Forms.Label();
            this.numLoot = new System.Windows.Forms.NumericUpDown();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnPullData = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numLevel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBE)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRP)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numChampions)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSkins)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numLoot)).BeginInit();
            this.SuspendLayout();
            // 
            // listView1
            // 
            this.listView1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colRiotID,
            this.colUsername,
            this.colPassword,
            this.colLevel,
            this.colServer,
            this.colBE,
            this.colRP,
            this.colSoloQ,
            this.colFlexQ,
            this.colChampions,
            this.colSkins,
            this.colLoot});
            this.listView1.FullRowSelect = true;
            this.listView1.GridLines = true;
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(10, 11);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(1060, 200);
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.DoubleClick += new System.EventHandler(this.listView1_DoubleClick);
            // 
            // colRiotID
            // 
            this.colRiotID.Text = "Riot ID";
            this.colRiotID.Width = 110;
            // 
            // colUsername
            // 
            this.colUsername.Text = "Username";
            this.colUsername.Width = 90;
            // 
            // colPassword
            // 
            this.colPassword.Text = "Password";
            this.colPassword.Width = 90;
            // 
            // colLevel
            // 
            this.colLevel.Text = "Level";
            this.colLevel.Width = 50;
            // 
            // colServer
            // 
            this.colServer.Text = "Server";
            this.colServer.Width = 55;
            // 
            // colBE
            // 
            this.colBE.Text = "BE";
            this.colBE.Width = 65;
            // 
            // colRP
            // 
            this.colRP.Text = "RP";
            this.colRP.Width = 55;
            // 
            // colSoloQ
            // 
            this.colSoloQ.Text = "SoloQ";
            this.colSoloQ.Width = 150;
            // 
            // colFlexQ
            // 
            this.colFlexQ.Text = "FlexQ";
            this.colFlexQ.Width = 130;
            // 
            // colChampions
            // 
            this.colChampions.Text = "Champs";
            this.colChampions.Width = 60;
            // 
            // colSkins
            // 
            this.colSkins.Text = "Skins";
            this.colSkins.Width = 55;
            // 
            // colLoot
            // 
            this.colLoot.Text = "Loot";
            this.colLoot.Width = 55;
            //
            // --- Editor panel (row 1: credentials) ---
            //
            int editY = 220;
            int col1 = 10, col2 = 190, col3 = 370;
            int lblH = 13, inputH = 20, rowGap = 42;
            // 
            // lblRiotID
            // 
            this.lblRiotID.AutoSize = true;
            this.lblRiotID.Location = new System.Drawing.Point(col1, editY);
            this.lblRiotID.Name = "lblRiotID";
            this.lblRiotID.Size = new System.Drawing.Size(40, lblH);
            this.lblRiotID.Text = "Riot ID";
            // 
            // txtRiotID
            // 
            this.txtRiotID.Location = new System.Drawing.Point(col1, editY + 16);
            this.txtRiotID.Name = "txtRiotID";
            this.txtRiotID.Size = new System.Drawing.Size(170, inputH);
            this.txtRiotID.TabIndex = 1;
            // 
            // lblUsername
            // 
            this.lblUsername.AutoSize = true;
            this.lblUsername.Location = new System.Drawing.Point(col2, editY);
            this.lblUsername.Name = "lblUsername";
            this.lblUsername.Size = new System.Drawing.Size(55, lblH);
            this.lblUsername.Text = "Username";
            // 
            // txtUsername
            // 
            this.txtUsername.Location = new System.Drawing.Point(col2, editY + 16);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(170, inputH);
            this.txtUsername.TabIndex = 2;
            // 
            // lblPassword
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.Location = new System.Drawing.Point(col3, editY);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(53, lblH);
            this.lblPassword.Text = "Password";
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(col3, editY + 16);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(170, inputH);
            this.txtPassword.TabIndex = 3;
            //
            // --- Editor panel (row 2: basic stats) ---
            //
            int row2Y = editY + rowGap;
            // 
            // lblLevel
            // 
            this.lblLevel.AutoSize = true;
            this.lblLevel.Location = new System.Drawing.Point(col1, row2Y);
            this.lblLevel.Name = "lblLevel";
            this.lblLevel.Size = new System.Drawing.Size(33, lblH);
            this.lblLevel.Text = "Level";
            // 
            // numLevel
            // 
            this.numLevel.Location = new System.Drawing.Point(col1, row2Y + 16);
            this.numLevel.Name = "numLevel";
            this.numLevel.Size = new System.Drawing.Size(80, inputH);
            this.numLevel.Maximum = 9999;
            this.numLevel.TabIndex = 4;
            // 
            // lblServer
            // 
            this.lblServer.AutoSize = true;
            this.lblServer.Location = new System.Drawing.Point(100, row2Y);
            this.lblServer.Name = "lblServer";
            this.lblServer.Size = new System.Drawing.Size(38, lblH);
            this.lblServer.Text = "Server";
            // 
            // txtServer
            // 
            this.txtServer.Location = new System.Drawing.Point(100, row2Y + 16);
            this.txtServer.Name = "txtServer";
            this.txtServer.Size = new System.Drawing.Size(80, inputH);
            this.txtServer.TabIndex = 5;
            // 
            // lblBE
            // 
            this.lblBE.AutoSize = true;
            this.lblBE.Location = new System.Drawing.Point(col2, row2Y);
            this.lblBE.Name = "lblBE";
            this.lblBE.Size = new System.Drawing.Size(20, lblH);
            this.lblBE.Text = "BE";
            // 
            // numBE
            // 
            this.numBE.Location = new System.Drawing.Point(col2, row2Y + 16);
            this.numBE.Name = "numBE";
            this.numBE.Size = new System.Drawing.Size(80, inputH);
            this.numBE.Maximum = 99999999;
            this.numBE.TabIndex = 6;
            // 
            // lblRP
            // 
            this.lblRP.AutoSize = true;
            this.lblRP.Location = new System.Drawing.Point(280, row2Y);
            this.lblRP.Name = "lblRP";
            this.lblRP.Size = new System.Drawing.Size(20, lblH);
            this.lblRP.Text = "RP";
            // 
            // numRP
            // 
            this.numRP.Location = new System.Drawing.Point(280, row2Y + 16);
            this.numRP.Name = "numRP";
            this.numRP.Size = new System.Drawing.Size(80, inputH);
            this.numRP.Maximum = 99999999;
            this.numRP.TabIndex = 7;
            //
            // --- Editor panel (row 3: ranks) ---
            //
            int row3Y = row2Y + rowGap;
            // 
            // lblSoloQ
            // 
            this.lblSoloQ.AutoSize = true;
            this.lblSoloQ.Location = new System.Drawing.Point(col1, row3Y);
            this.lblSoloQ.Name = "lblSoloQ";
            this.lblSoloQ.Size = new System.Drawing.Size(34, lblH);
            this.lblSoloQ.Text = "SoloQ";
            // 
            // txtSoloQ
            // 
            this.txtSoloQ.Location = new System.Drawing.Point(col1, row3Y + 16);
            this.txtSoloQ.Name = "txtSoloQ";
            this.txtSoloQ.Size = new System.Drawing.Size(250, inputH);
            this.txtSoloQ.TabIndex = 8;
            // 
            // lblFlexQ
            // 
            this.lblFlexQ.AutoSize = true;
            this.lblFlexQ.Location = new System.Drawing.Point(270, row3Y);
            this.lblFlexQ.Name = "lblFlexQ";
            this.lblFlexQ.Size = new System.Drawing.Size(33, lblH);
            this.lblFlexQ.Text = "FlexQ";
            // 
            // txtFlexQ
            // 
            this.txtFlexQ.Location = new System.Drawing.Point(270, row3Y + 16);
            this.txtFlexQ.Name = "txtFlexQ";
            this.txtFlexQ.Size = new System.Drawing.Size(250, inputH);
            this.txtFlexQ.TabIndex = 9;
            //
            // --- Editor panel (row 4: counts) ---
            //
            int row4Y = row3Y + rowGap;
            // 
            // lblChampions
            // 
            this.lblChampions.AutoSize = true;
            this.lblChampions.Location = new System.Drawing.Point(col1, row4Y);
            this.lblChampions.Name = "lblChampions";
            this.lblChampions.Size = new System.Drawing.Size(56, lblH);
            this.lblChampions.Text = "Champions";
            // 
            // numChampions
            // 
            this.numChampions.Location = new System.Drawing.Point(col1, row4Y + 16);
            this.numChampions.Name = "numChampions";
            this.numChampions.Size = new System.Drawing.Size(80, inputH);
            this.numChampions.Maximum = 9999;
            this.numChampions.TabIndex = 10;
            // 
            // lblSkins
            // 
            this.lblSkins.AutoSize = true;
            this.lblSkins.Location = new System.Drawing.Point(100, row4Y);
            this.lblSkins.Name = "lblSkins";
            this.lblSkins.Size = new System.Drawing.Size(33, lblH);
            this.lblSkins.Text = "Skins";
            // 
            // numSkins
            // 
            this.numSkins.Location = new System.Drawing.Point(100, row4Y + 16);
            this.numSkins.Name = "numSkins";
            this.numSkins.Size = new System.Drawing.Size(80, inputH);
            this.numSkins.Maximum = 9999;
            this.numSkins.TabIndex = 11;
            // 
            // lblLoot
            // 
            this.lblLoot.AutoSize = true;
            this.lblLoot.Location = new System.Drawing.Point(col2, row4Y);
            this.lblLoot.Name = "lblLoot";
            this.lblLoot.Size = new System.Drawing.Size(28, lblH);
            this.lblLoot.Text = "Loot";
            // 
            // numLoot
            // 
            this.numLoot.Location = new System.Drawing.Point(col2, row4Y + 16);
            this.numLoot.Name = "numLoot";
            this.numLoot.Size = new System.Drawing.Size(80, inputH);
            this.numLoot.Maximum = 9999;
            this.numLoot.TabIndex = 12;
            //
            // --- Buttons row ---
            //
            int btnY = row4Y + rowGap + 4;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(col1, btnY);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(82, 26);
            this.btnSave.TabIndex = 13;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(100, btnY);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(82, 26);
            this.btnAdd.TabIndex = 14;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(col2, btnY);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(82, 26);
            this.btnDelete.TabIndex = 15;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnPullData
            // 
            this.btnPullData.Location = new System.Drawing.Point(280, btnY);
            this.btnPullData.Name = "btnPullData";
            this.btnPullData.Size = new System.Drawing.Size(90, 26);
            this.btnPullData.TabIndex = 16;
            this.btnPullData.Text = "Pull Data";
            this.btnPullData.UseVisualStyleBackColor = true;
            this.btnPullData.Click += new System.EventHandler(this.btnPullData_Click);
            // 
            // AccountEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1084, btnY + 40);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.lblRiotID);
            this.Controls.Add(this.txtRiotID);
            this.Controls.Add(this.lblUsername);
            this.Controls.Add(this.txtUsername);
            this.Controls.Add(this.lblPassword);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.lblLevel);
            this.Controls.Add(this.numLevel);
            this.Controls.Add(this.lblServer);
            this.Controls.Add(this.txtServer);
            this.Controls.Add(this.lblBE);
            this.Controls.Add(this.numBE);
            this.Controls.Add(this.lblRP);
            this.Controls.Add(this.numRP);
            this.Controls.Add(this.lblSoloQ);
            this.Controls.Add(this.txtSoloQ);
            this.Controls.Add(this.lblFlexQ);
            this.Controls.Add(this.txtFlexQ);
            this.Controls.Add(this.lblChampions);
            this.Controls.Add(this.numChampions);
            this.Controls.Add(this.lblSkins);
            this.Controls.Add(this.numSkins);
            this.Controls.Add(this.lblLoot);
            this.Controls.Add(this.numLoot);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnPullData);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(1100, btnY + 80);
            this.Name = "AccountEditorForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Account Editor";
            this.Load += new System.EventHandler(this.AccountEditorForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numLevel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBE)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRP)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numChampions)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSkins)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numLoot)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader colRiotID;
        private System.Windows.Forms.ColumnHeader colUsername;
        private System.Windows.Forms.ColumnHeader colPassword;
        private System.Windows.Forms.ColumnHeader colLevel;
        private System.Windows.Forms.ColumnHeader colServer;
        private System.Windows.Forms.ColumnHeader colBE;
        private System.Windows.Forms.ColumnHeader colRP;
        private System.Windows.Forms.ColumnHeader colSoloQ;
        private System.Windows.Forms.ColumnHeader colFlexQ;
        private System.Windows.Forms.ColumnHeader colChampions;
        private System.Windows.Forms.ColumnHeader colSkins;
        private System.Windows.Forms.ColumnHeader colLoot;
        private System.Windows.Forms.Label lblRiotID;
        private System.Windows.Forms.TextBox txtRiotID;
        private System.Windows.Forms.Label lblUsername;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label lblLevel;
        private System.Windows.Forms.NumericUpDown numLevel;
        private System.Windows.Forms.Label lblServer;
        private System.Windows.Forms.TextBox txtServer;
        private System.Windows.Forms.Label lblBE;
        private System.Windows.Forms.NumericUpDown numBE;
        private System.Windows.Forms.Label lblRP;
        private System.Windows.Forms.NumericUpDown numRP;
        private System.Windows.Forms.Label lblSoloQ;
        private System.Windows.Forms.TextBox txtSoloQ;
        private System.Windows.Forms.Label lblFlexQ;
        private System.Windows.Forms.TextBox txtFlexQ;
        private System.Windows.Forms.Label lblChampions;
        private System.Windows.Forms.NumericUpDown numChampions;
        private System.Windows.Forms.Label lblSkins;
        private System.Windows.Forms.NumericUpDown numSkins;
        private System.Windows.Forms.Label lblLoot;
        private System.Windows.Forms.NumericUpDown numLoot;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnPullData;
    }
}