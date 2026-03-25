namespace RiotLogin
{
    partial class LoginForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginForm));
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.listView1 = new System.Windows.Forms.ListView();
            this.colRiotID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colUsername = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colLevel = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colServer = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colBE = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colRP = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colSoloQ = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colFlexQ = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colChampions = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colSkins = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colLoot = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.button2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(10, 234);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(95, 17);
            this.checkBox1.TabIndex = 0;
            this.checkBox1.Text = "Remember Me";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(10, 257);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(88, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "Login";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colRiotID,
            this.colUsername,
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
            this.listView1.Size = new System.Drawing.Size(960, 218);
            this.listView1.TabIndex = 6;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
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
            this.colSoloQ.Width = 160;
            // 
            // colFlexQ
            // 
            this.colFlexQ.Text = "FlexQ";
            this.colFlexQ.Width = 140;
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
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(105, 257);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(88, 23);
            this.button2.TabIndex = 8;
            this.button2.Text = "Settings";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 291);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.checkBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(1000, 330);
            this.Name = "LoginForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "RiotLogin";
            this.Load += new System.EventHandler(this.LoginForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader colRiotID;
        private System.Windows.Forms.ColumnHeader colUsername;
        private System.Windows.Forms.ColumnHeader colLevel;
        private System.Windows.Forms.ColumnHeader colServer;
        private System.Windows.Forms.ColumnHeader colBE;
        private System.Windows.Forms.ColumnHeader colRP;
        private System.Windows.Forms.ColumnHeader colSoloQ;
        private System.Windows.Forms.ColumnHeader colFlexQ;
        private System.Windows.Forms.ColumnHeader colChampions;
        private System.Windows.Forms.ColumnHeader colSkins;
        private System.Windows.Forms.ColumnHeader colLoot;
        private System.Windows.Forms.Button button2;
    }
}

// coded by lithellx - https://github.com/lithellx