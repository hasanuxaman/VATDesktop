namespace VATClient
{
    partial class FormSetting
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panel5 = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.btnAvgPrice = new System.Windows.Forms.Button();
            this.btnVATSaveLocation = new System.Windows.Forms.Button();
            this.btnMigration = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.dgvSettings = new System.Windows.Forms.DataGridView();
            this.SettingId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SettingGroup = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SettingName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SettingValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SettingType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ActiveStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.backgroundWorkerLoad = new System.ComponentModel.BackgroundWorker();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.backgroundWorkerUpdate = new System.ComponentModel.BackgroundWorker();
            this.cmbSettingGroup = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.bgwSettingsValue = new System.ComponentModel.BackgroundWorker();
            this.bgwDBProcess = new System.ComponentModel.BackgroundWorker();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbtnSetting = new System.Windows.Forms.RadioButton();
            this.rbtnRole = new System.Windows.Forms.RadioButton();
            this.txtUserID = new System.Windows.Forms.TextBox();
            this.lblUser = new System.Windows.Forms.Label();
            this.txtUserName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.bgwStockUpdate = new System.ComponentModel.BackgroundWorker();
            this.panel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSettings)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel5
            // 
            this.panel5.BackgroundImage = global::VATClient.Properties.Resources.Green_Footer;
            this.panel5.Controls.Add(this.button1);
            this.panel5.Controls.Add(this.btnAvgPrice);
            this.panel5.Controls.Add(this.btnVATSaveLocation);
            this.panel5.Controls.Add(this.btnMigration);
            this.panel5.Controls.Add(this.btnClose);
            this.panel5.Controls.Add(this.btnRefresh);
            this.panel5.Controls.Add(this.btnUpdate);
            this.panel5.Location = new System.Drawing.Point(-1, 384);
            this.panel5.Margin = new System.Windows.Forms.Padding(4);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(755, 49);
            this.panel5.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.button1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button1.Location = new System.Drawing.Point(388, 48);
            this.button1.Margin = new System.Windows.Forms.Padding(4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(139, 32);
            this.button1.TabIndex = 8;
            this.button1.Text = "Stock Process";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnAvgPrice
            // 
            this.btnAvgPrice.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnAvgPrice.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAvgPrice.Location = new System.Drawing.Point(243, 49);
            this.btnAvgPrice.Margin = new System.Windows.Forms.Padding(4);
            this.btnAvgPrice.Name = "btnAvgPrice";
            this.btnAvgPrice.Size = new System.Drawing.Size(139, 33);
            this.btnAvgPrice.TabIndex = 7;
            this.btnAvgPrice.Text = "AVG Rate Process";
            this.btnAvgPrice.UseVisualStyleBackColor = false;
            this.btnAvgPrice.Visible = false;
            this.btnAvgPrice.Click += new System.EventHandler(this.btnAvgPrice_Click);
            // 
            // btnVATSaveLocation
            // 
            this.btnVATSaveLocation.Location = new System.Drawing.Point(729, 49);
            this.btnVATSaveLocation.Margin = new System.Windows.Forms.Padding(4);
            this.btnVATSaveLocation.Name = "btnVATSaveLocation";
            this.btnVATSaveLocation.Size = new System.Drawing.Size(25, 28);
            this.btnVATSaveLocation.TabIndex = 6;
            this.btnVATSaveLocation.Text = "Update";
            this.btnVATSaveLocation.UseVisualStyleBackColor = true;
            this.btnVATSaveLocation.Visible = false;
            this.btnVATSaveLocation.Click += new System.EventHandler(this.btnVATSaveLocation_Click);
            // 
            // btnMigration
            // 
            this.btnMigration.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnMigration.Image = global::VATClient.Properties.Resources.Update;
            this.btnMigration.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnMigration.Location = new System.Drawing.Point(117, 6);
            this.btnMigration.Margin = new System.Windows.Forms.Padding(4);
            this.btnMigration.Name = "btnMigration";
            this.btnMigration.Size = new System.Drawing.Size(123, 34);
            this.btnMigration.TabIndex = 4;
            this.btnMigration.Text = "DBMigration";
            this.btnMigration.UseVisualStyleBackColor = false;
            this.btnMigration.Click += new System.EventHandler(this.btnMigration_Click);
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnClose.Image = global::VATClient.Properties.Resources.Back;
            this.btnClose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnClose.Location = new System.Drawing.Point(640, 7);
            this.btnClose.Margin = new System.Windows.Forms.Padding(4);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(100, 34);
            this.btnClose.TabIndex = 3;
            this.btnClose.Text = "&Close";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnRefresh.Image = global::VATClient.Properties.Resources.Referesh;
            this.btnRefresh.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnRefresh.Location = new System.Drawing.Point(532, 7);
            this.btnRefresh.Margin = new System.Windows.Forms.Padding(4);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(100, 34);
            this.btnRefresh.TabIndex = 2;
            this.btnRefresh.Text = "&Refresh";
            this.btnRefresh.UseVisualStyleBackColor = false;
            this.btnRefresh.Visible = false;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnUpdate
            // 
            this.btnUpdate.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnUpdate.Image = global::VATClient.Properties.Resources.Update;
            this.btnUpdate.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnUpdate.Location = new System.Drawing.Point(12, 7);
            this.btnUpdate.Margin = new System.Windows.Forms.Padding(4);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(100, 34);
            this.btnUpdate.TabIndex = 2;
            this.btnUpdate.Text = "&Update";
            this.btnUpdate.UseVisualStyleBackColor = false;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // dgvSettings
            // 
            this.dgvSettings.AllowUserToAddRows = false;
            this.dgvSettings.AllowUserToDeleteRows = false;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.dgvSettings.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvSettings.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSettings.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.SettingId,
            this.SettingGroup,
            this.SettingName,
            this.SettingValue,
            this.SettingType,
            this.ActiveStatus});
            this.dgvSettings.Location = new System.Drawing.Point(12, 76);
            this.dgvSettings.Margin = new System.Windows.Forms.Padding(4);
            this.dgvSettings.Name = "dgvSettings";
            this.dgvSettings.RowHeadersVisible = false;
            this.dgvSettings.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvSettings.Size = new System.Drawing.Size(725, 282);
            this.dgvSettings.TabIndex = 21;
            this.dgvSettings.TabStop = false;
            this.dgvSettings.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dgvSettings_CellBeginEdit);
            this.dgvSettings.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSettings_CellClick);
            this.dgvSettings.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSettings_CellContentClick);
            this.dgvSettings.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSettings_CellEndEdit);
            // 
            // SettingId
            // 
            this.SettingId.DataPropertyName = "SettingId";
            this.SettingId.HeaderText = "Setting ID";
            this.SettingId.Name = "SettingId";
            this.SettingId.ReadOnly = true;
            this.SettingId.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.SettingId.Visible = false;
            // 
            // SettingGroup
            // 
            this.SettingGroup.DataPropertyName = "SettingGroup";
            this.SettingGroup.HeaderText = "Setting Group";
            this.SettingGroup.Name = "SettingGroup";
            this.SettingGroup.ReadOnly = true;
            this.SettingGroup.Width = 125;
            // 
            // SettingName
            // 
            this.SettingName.DataPropertyName = "SettingName";
            this.SettingName.FillWeight = 300F;
            this.SettingName.HeaderText = "Setting Name";
            this.SettingName.Name = "SettingName";
            this.SettingName.ReadOnly = true;
            this.SettingName.Width = 300;
            // 
            // SettingValue
            // 
            this.SettingValue.DataPropertyName = "SettingValue";
            this.SettingValue.HeaderText = "Setting Value";
            this.SettingValue.Name = "SettingValue";
            this.SettingValue.Width = 115;
            // 
            // SettingType
            // 
            this.SettingType.DataPropertyName = "SettingType";
            this.SettingType.HeaderText = "Setting Type";
            this.SettingType.Name = "SettingType";
            this.SettingType.ReadOnly = true;
            this.SettingType.Visible = false;
            // 
            // ActiveStatus
            // 
            this.ActiveStatus.HeaderText = "Active Status";
            this.ActiveStatus.Name = "ActiveStatus";
            this.ActiveStatus.Visible = false;
            // 
            // backgroundWorkerLoad
            // 
            this.backgroundWorkerLoad.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerLoad_DoWork);
            this.backgroundWorkerLoad.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerLoad_RunWorkerCompleted);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(192, 138);
            this.progressBar1.Margin = new System.Windows.Forms.Padding(4);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(387, 30);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 22;
            // 
            // backgroundWorkerUpdate
            // 
            this.backgroundWorkerUpdate.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerUpdate_DoWork);
            this.backgroundWorkerUpdate.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerUpdate_RunWorkerCompleted);
            // 
            // cmbSettingGroup
            // 
            this.cmbSettingGroup.FormattingEnabled = true;
            this.cmbSettingGroup.Location = new System.Drawing.Point(111, 12);
            this.cmbSettingGroup.Margin = new System.Windows.Forms.Padding(4);
            this.cmbSettingGroup.Name = "cmbSettingGroup";
            this.cmbSettingGroup.Size = new System.Drawing.Size(261, 24);
            this.cmbSettingGroup.Sorted = true;
            this.cmbSettingGroup.TabIndex = 0;
            this.cmbSettingGroup.SelectedIndexChanged += new System.EventHandler(this.cmbSettingGroup_SelectedIndexChanged);
            this.cmbSettingGroup.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmbSettingGroup_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 17);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 17);
            this.label1.TabIndex = 24;
            this.label1.Text = "Setting Group:";
            // 
            // bgwSettingsValue
            // 
            this.bgwSettingsValue.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwSettingsValue_DoWork);
            this.bgwSettingsValue.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwSettingsValue_RunWorkerCompleted);
            // 
            // bgwDBProcess
            // 
            this.bgwDBProcess.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwDBProcess_DoWork);
            this.bgwDBProcess.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwDBProcess_RunWorkerCompleted);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbtnSetting);
            this.groupBox1.Controls.Add(this.rbtnRole);
            this.groupBox1.Location = new System.Drawing.Point(444, 175);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(111, 80);
            this.groupBox1.TabIndex = 25;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "groupBox1";
            this.groupBox1.Visible = false;
            // 
            // rbtnSetting
            // 
            this.rbtnSetting.AutoSize = true;
            this.rbtnSetting.Location = new System.Drawing.Point(21, 23);
            this.rbtnSetting.Margin = new System.Windows.Forms.Padding(4);
            this.rbtnSetting.Name = "rbtnSetting";
            this.rbtnSetting.Size = new System.Drawing.Size(73, 21);
            this.rbtnSetting.TabIndex = 0;
            this.rbtnSetting.TabStop = true;
            this.rbtnSetting.Text = "Setting";
            this.rbtnSetting.UseVisualStyleBackColor = true;
            // 
            // rbtnRole
            // 
            this.rbtnRole.AutoSize = true;
            this.rbtnRole.Location = new System.Drawing.Point(21, 52);
            this.rbtnRole.Margin = new System.Windows.Forms.Padding(4);
            this.rbtnRole.Name = "rbtnRole";
            this.rbtnRole.Size = new System.Drawing.Size(58, 21);
            this.rbtnRole.TabIndex = 0;
            this.rbtnRole.TabStop = true;
            this.rbtnRole.Text = "Role";
            this.rbtnRole.UseVisualStyleBackColor = true;
            // 
            // txtUserID
            // 
            this.txtUserID.Location = new System.Drawing.Point(763, 14);
            this.txtUserID.Margin = new System.Windows.Forms.Padding(4);
            this.txtUserID.MaximumSize = new System.Drawing.Size(4, 4);
            this.txtUserID.MinimumSize = new System.Drawing.Size(25, 20);
            this.txtUserID.Name = "txtUserID";
            this.txtUserID.ReadOnly = true;
            this.txtUserID.Size = new System.Drawing.Size(25, 20);
            this.txtUserID.TabIndex = 152;
            this.txtUserID.TabStop = false;
            this.txtUserID.Visible = false;
            // 
            // lblUser
            // 
            this.lblUser.AutoSize = true;
            this.lblUser.Location = new System.Drawing.Point(377, 17);
            this.lblUser.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblUser.Name = "lblUser";
            this.lblUser.Size = new System.Drawing.Size(105, 17);
            this.lblUser.TabIndex = 153;
            this.lblUser.Text = "User Name(F9)";
            // 
            // txtUserName
            // 
            this.txtUserName.Location = new System.Drawing.Point(493, 12);
            this.txtUserName.Margin = new System.Windows.Forms.Padding(4);
            this.txtUserName.MaximumSize = new System.Drawing.Size(4, 4);
            this.txtUserName.MinimumSize = new System.Drawing.Size(245, 20);
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.ReadOnly = true;
            this.txtUserName.Size = new System.Drawing.Size(245, 20);
            this.txtUserName.TabIndex = 151;
            this.txtUserName.TextChanged += new System.EventHandler(this.txtUserName_TextChanged);
            this.txtUserName.DoubleClick += new System.EventHandler(this.txtUserName_DoubleClick);
            this.txtUserName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtUserName_KeyDown);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(40, 48);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 17);
            this.label2.TabIndex = 155;
            this.label2.Text = "Search:";
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(111, 44);
            this.txtSearch.Margin = new System.Windows.Forms.Padding(4);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(261, 22);
            this.txtSearch.TabIndex = 154;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            // 
            // bgwStockUpdate
            // 
            this.bgwStockUpdate.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwStockUpdate_DoWork);
            this.bgwStockUpdate.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwStockUpdate_RunWorkerCompleted);
            // 
            // FormSetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(756, 448);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtSearch);
            this.Controls.Add(this.txtUserID);
            this.Controls.Add(this.lblUser);
            this.Controls.Add(this.txtUserName);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbSettingGroup);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.dgvSettings);
            this.Controls.Add(this.panel5);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.Name = "FormSetting";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Settings";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormSetting_FormClosing);
            this.Load += new System.EventHandler(this.FormSetting_Load);
            this.panel5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSettings)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.DataGridView dgvSettings;
        private System.ComponentModel.BackgroundWorker backgroundWorkerLoad;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.ComponentModel.BackgroundWorker backgroundWorkerUpdate;
        private System.Windows.Forms.ComboBox cmbSettingGroup;
        private System.Windows.Forms.Label label1;
        private System.ComponentModel.BackgroundWorker bgwSettingsValue;
        private System.Windows.Forms.Button btnMigration;
        private System.ComponentModel.BackgroundWorker bgwDBProcess;
        private System.Windows.Forms.DataGridViewTextBoxColumn SettingId;
        private System.Windows.Forms.DataGridViewTextBoxColumn SettingGroup;
        private System.Windows.Forms.DataGridViewTextBoxColumn SettingName;
        private System.Windows.Forms.DataGridViewTextBoxColumn SettingValue;
        private System.Windows.Forms.DataGridViewTextBoxColumn SettingType;
        private System.Windows.Forms.DataGridViewTextBoxColumn ActiveStatus;
        private System.Windows.Forms.Button btnVATSaveLocation;
        private System.Windows.Forms.GroupBox groupBox1;
        public System.Windows.Forms.RadioButton rbtnSetting;
        public System.Windows.Forms.RadioButton rbtnRole;
        private System.Windows.Forms.TextBox txtUserID;
        private System.Windows.Forms.Label lblUser;
        private System.Windows.Forms.TextBox txtUserName;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtSearch;
        public System.Windows.Forms.Button btnUpdate;
        private System.ComponentModel.BackgroundWorker bgwStockUpdate;
        private System.Windows.Forms.Button btnAvgPrice;
        private System.Windows.Forms.Button button1;
    }
}