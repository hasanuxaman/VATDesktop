namespace VATClient
{
    partial class FormUserMenuAllRolls
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgvSettings = new System.Windows.Forms.DataGridView();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.backgroundWorkerUpdate = new System.ComponentModel.BackgroundWorker();
            this.bgwUserMenuAllRolls = new System.ComponentModel.BackgroundWorker();
            this.label2 = new System.Windows.Forms.Label();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.panel5 = new System.Windows.Forms.Panel();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.chkAccess = new System.Windows.Forms.CheckBox();
            this.FormID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AccessType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FormName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Access = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSettings)).BeginInit();
            this.panel5.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvSettings
            // 
            this.dgvSettings.AllowUserToAddRows = false;
            this.dgvSettings.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.dgvSettings.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvSettings.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSettings.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.FormID,
            this.AccessType,
            this.FormName,
            this.Access});
            this.dgvSettings.Location = new System.Drawing.Point(9, 62);
            this.dgvSettings.Name = "dgvSettings";
            this.dgvSettings.RowHeadersVisible = false;
            this.dgvSettings.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvSettings.Size = new System.Drawing.Size(556, 229);
            this.dgvSettings.TabIndex = 21;
            this.dgvSettings.TabStop = false;
            this.dgvSettings.DoubleClick += new System.EventHandler(this.dgvSettings_DoubleClick);
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
            this.label2.Location = new System.Drawing.Point(22, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 13);
            this.label2.TabIndex = 155;
            this.label2.Text = "Form Name";
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(85, 19);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(197, 20);
            this.txtSearch.TabIndex = 154;
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSearch.Image = global::VATClient.Properties.Resources.search;
            this.btnSearch.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSearch.Location = new System.Drawing.Point(287, 14);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 28);
            this.btnSearch.TabIndex = 4;
            this.btnSearch.Text = "&Search";
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // panel5
            // 
            this.panel5.BackgroundImage = global::VATClient.Properties.Resources.Green_Footer;
            this.panel5.Controls.Add(this.btnClose);
            this.panel5.Controls.Add(this.btnUpdate);
            this.panel5.Location = new System.Drawing.Point(-1, 312);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(679, 40);
            this.panel5.TabIndex = 1;
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnClose.Image = global::VATClient.Properties.Resources.Back;
            this.btnClose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnClose.Location = new System.Drawing.Point(453, 9);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 28);
            this.btnClose.TabIndex = 3;
            this.btnClose.Text = "&Close";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnUpdate
            // 
            this.btnUpdate.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnUpdate.Image = global::VATClient.Properties.Resources.Update;
            this.btnUpdate.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnUpdate.Location = new System.Drawing.Point(9, 6);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(75, 28);
            this.btnUpdate.TabIndex = 2;
            this.btnUpdate.Text = "&Update";
            this.btnUpdate.UseVisualStyleBackColor = false;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(593, 16);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(75, 23);
            this.btnRefresh.TabIndex = 4;
            // 
            // chkAccess
            // 
            this.chkAccess.AutoSize = true;
            this.chkAccess.Location = new System.Drawing.Point(411, 39);
            this.chkAccess.Name = "chkAccess";
            this.chkAccess.Size = new System.Drawing.Size(75, 17);
            this.chkAccess.TabIndex = 156;
            this.chkAccess.Text = "All Access";
            this.chkAccess.UseVisualStyleBackColor = true;
            this.chkAccess.CheckedChanged += new System.EventHandler(this.chkAccess_CheckedChanged);
            this.chkAccess.Click += new System.EventHandler(this.chkAccess_Click);
            // 
            // FormID
            // 
            this.FormID.DataPropertyName = "FormID";
            dataGridViewCellStyle2.NullValue = "0";
            this.FormID.DefaultCellStyle = dataGridViewCellStyle2;
            this.FormID.HeaderText = "Form ID";
            this.FormID.Name = "FormID";
            this.FormID.ReadOnly = true;
            this.FormID.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.FormID.Visible = false;
            // 
            // AccessType
            // 
            this.AccessType.DataPropertyName = "AccessType";
            this.AccessType.HeaderText = "AccessType";
            this.AccessType.Name = "AccessType";
            this.AccessType.ReadOnly = true;
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
            this.Access.HeaderText = "Access";
            this.Access.Name = "Access";
            this.Access.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Access.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Access.TrueValue = "1";
            // 
            // FormUserMenuAllRolls
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(566, 364);
            this.Controls.Add(this.chkAccess);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtSearch);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.dgvSettings);
            this.Controls.Add(this.panel5);
            this.MaximizeBox = false;
            this.Name = "FormUserMenuAllRolls";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "All Rolls Settings";
            this.Load += new System.EventHandler(this.FormSetting_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSettings)).EndInit();
            this.panel5.ResumeLayout(false);
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
        public System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.CheckBox chkAccess;
        private System.Windows.Forms.DataGridViewTextBoxColumn FormID;
        private System.Windows.Forms.DataGridViewTextBoxColumn AccessType;
        private System.Windows.Forms.DataGridViewTextBoxColumn FormName;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Access;
    }
}