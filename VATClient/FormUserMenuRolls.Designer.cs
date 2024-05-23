namespace VATClient
{
    partial class FormUserRolls
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panel5 = new System.Windows.Forms.Panel();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.dgvSettings = new System.Windows.Forms.DataGridView();
            this.FormID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.UserID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FormName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Access = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.PostAccess = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.AddAccess = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.EditAccess = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.backgroundWorkerUpdate = new System.ComponentModel.BackgroundWorker();
            this.bgwUserMenuAllRolls = new System.ComponentModel.BackgroundWorker();
            this.label2 = new System.Windows.Forms.Label();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.cmbUserName = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.chkAccess = new System.Windows.Forms.CheckBox();
            this.chkPost = new System.Windows.Forms.CheckBox();
            this.chkAdd = new System.Windows.Forms.CheckBox();
            this.chkEdit = new System.Windows.Forms.CheckBox();
            this.chkAll = new System.Windows.Forms.CheckBox();
            this.btnImport = new System.Windows.Forms.Button();
            this.panel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSettings)).BeginInit();
            this.SuspendLayout();
            // 
            // panel5
            // 
            this.panel5.BackgroundImage = global::VATClient.Properties.Resources.Green_Footer;
            this.panel5.Controls.Add(this.btnImport);
            this.panel5.Controls.Add(this.btnExport);
            this.panel5.Controls.Add(this.btnClose);
            this.panel5.Controls.Add(this.btnRefresh);
            this.panel5.Controls.Add(this.btnUpdate);
            this.panel5.Location = new System.Drawing.Point(-1, 312);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(679, 40);
            this.panel5.TabIndex = 1;
            // 
            // btnExport
            // 
            this.btnExport.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnExport.Location = new System.Drawing.Point(329, 8);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(75, 25);
            this.btnExport.TabIndex = 236;
            this.btnExport.Text = "Export";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Visible = false;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnClose.Image = global::VATClient.Properties.Resources.Back;
            this.btnClose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnClose.Location = new System.Drawing.Point(598, 6);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 28);
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
            this.btnRefresh.Location = new System.Drawing.Point(517, 6);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(75, 28);
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
            this.btnUpdate.Location = new System.Drawing.Point(18, 6);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(75, 28);
            this.btnUpdate.TabIndex = 2;
            this.btnUpdate.Text = "&Update";
            this.btnUpdate.UseVisualStyleBackColor = false;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // dgvSettings
            // 
            this.dgvSettings.AllowUserToAddRows = false;
            this.dgvSettings.AllowUserToDeleteRows = false;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.dgvSettings.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvSettings.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSettings.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.FormID,
            this.UserID,
            this.FormName,
            this.Access,
            this.PostAccess,
            this.AddAccess,
            this.EditAccess});
            this.dgvSettings.Location = new System.Drawing.Point(9, 67);
            this.dgvSettings.Name = "dgvSettings";
            this.dgvSettings.RowHeadersVisible = false;
            this.dgvSettings.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvSettings.Size = new System.Drawing.Size(669, 242);
            this.dgvSettings.TabIndex = 21;
            this.dgvSettings.TabStop = false;
            this.dgvSettings.DoubleClick += new System.EventHandler(this.dgvSettings_DoubleClick);
            // 
            // FormID
            // 
            this.FormID.DataPropertyName = "FormID";
            dataGridViewCellStyle4.NullValue = "0";
            this.FormID.DefaultCellStyle = dataGridViewCellStyle4;
            this.FormID.HeaderText = "Form ID";
            this.FormID.Name = "FormID";
            this.FormID.ReadOnly = true;
            this.FormID.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.FormID.Visible = false;
            // 
            // UserID
            // 
            this.UserID.DataPropertyName = "UserID";
            this.UserID.HeaderText = "UserID";
            this.UserID.Name = "UserID";
            this.UserID.Visible = false;
            // 
            // FormName
            // 
            this.FormName.DataPropertyName = "FormName";
            this.FormName.FillWeight = 300F;
            this.FormName.HeaderText = "Form Name";
            this.FormName.Name = "FormName";
            this.FormName.ReadOnly = true;
            this.FormName.Width = 300;
            // 
            // Access
            // 
            this.Access.DataPropertyName = "Access";
            this.Access.FalseValue = "0";
            this.Access.FillWeight = 80F;
            this.Access.HeaderText = "Access";
            this.Access.Name = "Access";
            this.Access.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Access.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Access.TrueValue = "1";
            this.Access.Width = 80;
            // 
            // PostAccess
            // 
            this.PostAccess.DataPropertyName = "PostAccess";
            this.PostAccess.FalseValue = "0";
            this.PostAccess.FillWeight = 80F;
            this.PostAccess.HeaderText = "Post";
            this.PostAccess.Name = "PostAccess";
            this.PostAccess.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.PostAccess.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.PostAccess.TrueValue = "1";
            this.PostAccess.Width = 80;
            // 
            // AddAccess
            // 
            this.AddAccess.DataPropertyName = "AddAccess";
            this.AddAccess.FalseValue = "0";
            this.AddAccess.FillWeight = 80F;
            this.AddAccess.HeaderText = "Add";
            this.AddAccess.Name = "AddAccess";
            this.AddAccess.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.AddAccess.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.AddAccess.TrueValue = "1";
            this.AddAccess.Width = 80;
            // 
            // EditAccess
            // 
            this.EditAccess.DataPropertyName = "EditAccess";
            this.EditAccess.FalseValue = "0";
            this.EditAccess.FillWeight = 80F;
            this.EditAccess.HeaderText = "Edit";
            this.EditAccess.Name = "EditAccess";
            this.EditAccess.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.EditAccess.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.EditAccess.TrueValue = "1";
            this.EditAccess.Width = 80;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(144, 112);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(290, 24);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 22;
            // 
            // backgroundWorkerUpdate
            // 
            this.backgroundWorkerUpdate.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerUpdate_DoWork);
            this.backgroundWorkerUpdate.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerUpdate_RunWorkerCompleted);
            // 
            // bgwUserMenuAllRolls
            // 
            this.bgwUserMenuAllRolls.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwUserMenuAllRolls_DoWork);
            this.bgwUserMenuAllRolls.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwUserMenuAllRolls_RunWorkerCompleted);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 26);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 13);
            this.label2.TabIndex = 155;
            this.label2.Text = "Form Name";
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(84, 23);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(197, 20);
            this.txtSearch.TabIndex = 154;
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSearch.Image = global::VATClient.Properties.Resources.search;
            this.btnSearch.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSearch.Location = new System.Drawing.Point(521, 20);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 25);
            this.btnSearch.TabIndex = 4;
            this.btnSearch.Text = "&Search";
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // cmbUserName
            // 
            this.cmbUserName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbUserName.FormattingEnabled = true;
            this.cmbUserName.Items.AddRange(new object[] {
            "Y",
            "N"});
            this.cmbUserName.Location = new System.Drawing.Point(357, 22);
            this.cmbUserName.Name = "cmbUserName";
            this.cmbUserName.Size = new System.Drawing.Size(158, 21);
            this.cmbUserName.TabIndex = 228;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(292, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 13);
            this.label1.TabIndex = 229;
            this.label1.Text = "User Name";
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.button1.Image = global::VATClient.Properties.Resources.Save;
            this.button1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button1.Location = new System.Drawing.Point(602, 20);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 25);
            this.button1.TabIndex = 230;
            this.button1.Text = "&Add";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // chkAccess
            // 
            this.chkAccess.AutoSize = true;
            this.chkAccess.Location = new System.Drawing.Point(312, 46);
            this.chkAccess.Name = "chkAccess";
            this.chkAccess.Size = new System.Drawing.Size(61, 17);
            this.chkAccess.TabIndex = 231;
            this.chkAccess.Text = "Access";
            this.chkAccess.UseVisualStyleBackColor = true;
            this.chkAccess.Click += new System.EventHandler(this.chkAccess_Click);
            // 
            // chkPost
            // 
            this.chkPost.AutoSize = true;
            this.chkPost.Location = new System.Drawing.Point(398, 46);
            this.chkPost.Name = "chkPost";
            this.chkPost.Size = new System.Drawing.Size(47, 17);
            this.chkPost.TabIndex = 232;
            this.chkPost.Text = "Post";
            this.chkPost.UseVisualStyleBackColor = true;
            this.chkPost.CheckedChanged += new System.EventHandler(this.chkPost_CheckedChanged);
            this.chkPost.Click += new System.EventHandler(this.chkPost_Click);
            // 
            // chkAdd
            // 
            this.chkAdd.AutoSize = true;
            this.chkAdd.Location = new System.Drawing.Point(479, 46);
            this.chkAdd.Name = "chkAdd";
            this.chkAdd.Size = new System.Drawing.Size(45, 17);
            this.chkAdd.TabIndex = 233;
            this.chkAdd.Text = "Add";
            this.chkAdd.UseVisualStyleBackColor = true;
            this.chkAdd.Click += new System.EventHandler(this.chkAdd_Click);
            // 
            // chkEdit
            // 
            this.chkEdit.AutoSize = true;
            this.chkEdit.Location = new System.Drawing.Point(552, 46);
            this.chkEdit.Name = "chkEdit";
            this.chkEdit.Size = new System.Drawing.Size(44, 17);
            this.chkEdit.TabIndex = 234;
            this.chkEdit.Text = "Edit";
            this.chkEdit.UseVisualStyleBackColor = true;
            this.chkEdit.Click += new System.EventHandler(this.chkEdit_Click);
            // 
            // chkAll
            // 
            this.chkAll.AutoSize = true;
            this.chkAll.Location = new System.Drawing.Point(359, 3);
            this.chkAll.Name = "chkAll";
            this.chkAll.Size = new System.Drawing.Size(45, 17);
            this.chkAll.TabIndex = 235;
            this.chkAll.Text = "ALL";
            this.chkAll.UseVisualStyleBackColor = true;
            this.chkAll.CheckedChanged += new System.EventHandler(this.chkAll_CheckedChanged);
            this.chkAll.Click += new System.EventHandler(this.chkAll_Click);
            // 
            // btnImport
            // 
            this.btnImport.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnImport.Location = new System.Drawing.Point(424, 8);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(75, 25);
            this.btnImport.TabIndex = 237;
            this.btnImport.Text = "Import";
            this.btnImport.UseVisualStyleBackColor = true;
            this.btnImport.Visible = false;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // FormUserRolls
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(680, 352);
            this.Controls.Add(this.chkAll);
            this.Controls.Add(this.chkEdit);
            this.Controls.Add(this.chkAdd);
            this.Controls.Add(this.chkPost);
            this.Controls.Add(this.chkAccess);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbUserName);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtSearch);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.dgvSettings);
            this.Controls.Add(this.panel5);
            this.MaximizeBox = false;
            this.Name = "FormUserRolls";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "User Rolls Settings";
            this.Load += new System.EventHandler(this.FormSetting_Load);
            this.panel5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSettings)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.DataGridView dgvSettings;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.ComponentModel.BackgroundWorker backgroundWorkerUpdate;
        private System.ComponentModel.BackgroundWorker bgwUserMenuAllRolls;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.ComboBox cmbUserName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridViewTextBoxColumn FormID;
        private System.Windows.Forms.DataGridViewTextBoxColumn UserID;
        private System.Windows.Forms.DataGridViewTextBoxColumn FormName;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Access;
        private System.Windows.Forms.DataGridViewCheckBoxColumn PostAccess;
        private System.Windows.Forms.DataGridViewCheckBoxColumn AddAccess;
        private System.Windows.Forms.DataGridViewCheckBoxColumn EditAccess;
        public System.Windows.Forms.Button btnUpdate;
        public System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox chkAccess;
        private System.Windows.Forms.CheckBox chkPost;
        private System.Windows.Forms.CheckBox chkAdd;
        private System.Windows.Forms.CheckBox chkEdit;
        private System.Windows.Forms.CheckBox chkAll;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Button btnImport;
    }
}