namespace VATClient
{
    partial class FormReleaseNotes
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
            this.panel5 = new System.Windows.Forms.Panel();
            this.btnClose = new System.Windows.Forms.Button();
            this.dgvNotes = new System.Windows.Forms.DataGridView();
            this.cmbVersionGroup = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtUserID = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.bgwDeleteInsert = new System.ComponentModel.BackgroundWorker();
            this.bgwLoad = new System.ComponentModel.BackgroundWorker();
            this.ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SL = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Version = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Name1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Issue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Description = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvNotes)).BeginInit();
            this.SuspendLayout();
            // 
            // panel5
            // 
            this.panel5.BackgroundImage = global::VATClient.Properties.Resources.Green_Footer;
            this.panel5.Controls.Add(this.btnClose);
            this.panel5.Location = new System.Drawing.Point(-3, 297);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(566, 40);
            this.panel5.TabIndex = 1;
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnClose.Image = global::VATClient.Properties.Resources.Back;
            this.btnClose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnClose.Location = new System.Drawing.Point(444, 6);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 28);
            this.btnClose.TabIndex = 3;
            this.btnClose.Text = "&Close";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // dgvNotes
            // 
            this.dgvNotes.AllowUserToAddRows = false;
            this.dgvNotes.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.dgvNotes.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvNotes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvNotes.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ID,
            this.SL,
            this.Version,
            this.Date,
            this.Name1,
            this.Issue,
            this.Description});
            this.dgvNotes.Location = new System.Drawing.Point(9, 62);
            this.dgvNotes.Name = "dgvNotes";
            this.dgvNotes.RowHeadersVisible = false;
            this.dgvNotes.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvNotes.Size = new System.Drawing.Size(528, 229);
            this.dgvNotes.TabIndex = 21;
            this.dgvNotes.TabStop = false;
            this.dgvNotes.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvNotes_CellContentClick);
            this.dgvNotes.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvNotes_CellDoubleClick);
            this.dgvNotes.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvNotes_CellFormatting);
            // 
            // cmbVersionGroup
            // 
            this.cmbVersionGroup.FormattingEnabled = true;
            this.cmbVersionGroup.Location = new System.Drawing.Point(83, 10);
            this.cmbVersionGroup.Name = "cmbVersionGroup";
            this.cmbVersionGroup.Size = new System.Drawing.Size(197, 21);
            this.cmbVersionGroup.Sorted = true;
            this.cmbVersionGroup.TabIndex = 0;
            this.cmbVersionGroup.SelectedIndexChanged += new System.EventHandler(this.cmbVersionGroup_SelectedIndexChanged);
            this.cmbVersionGroup.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmbSettingGroup_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 13);
            this.label1.TabIndex = 24;
            this.label1.Text = "Versions:";
            // 
            // txtUserID
            // 
            this.txtUserID.Location = new System.Drawing.Point(572, 11);
            this.txtUserID.MaximumSize = new System.Drawing.Size(4, 4);
            this.txtUserID.MinimumSize = new System.Drawing.Size(20, 20);
            this.txtUserID.Name = "txtUserID";
            this.txtUserID.ReadOnly = true;
            this.txtUserID.Size = new System.Drawing.Size(20, 20);
            this.txtUserID.TabIndex = 152;
            this.txtUserID.TabStop = false;
            this.txtUserID.Visible = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(30, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 13);
            this.label2.TabIndex = 155;
            this.label2.Text = "Search:";
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(83, 36);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(197, 20);
            this.txtSearch.TabIndex = 154;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            // 
            // bgwDeleteInsert
            // 
            this.bgwDeleteInsert.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwDeleteInsert_DoWork);
            this.bgwDeleteInsert.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwDeleteInsert_RunWorkerCompleted);
            // 
            // bgwLoad
            // 
            this.bgwLoad.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwLoad_DoWork);
            this.bgwLoad.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwLoad_RunWorkerCompleted);
            // 
            // ID
            // 
            this.ID.HeaderText = "ID";
            this.ID.Name = "ID";
            this.ID.Visible = false;
            // 
            // SL
            // 
            this.SL.HeaderText = "SL";
            this.SL.Name = "SL";
            this.SL.Visible = false;
            // 
            // Version
            // 
            this.Version.HeaderText = "Version";
            this.Version.Name = "Version";
            // 
            // Date
            // 
            this.Date.HeaderText = "Date";
            this.Date.Name = "Date";
            // 
            // Name1
            // 
            this.Name1.HeaderText = "Name";
            this.Name1.Name = "Name1";
            this.Name1.Width = 150;
            // 
            // Issue
            // 
            this.Issue.HeaderText = "Issue";
            this.Issue.Name = "Issue";
            this.Issue.Width = 175;
            // 
            // Description
            // 
            this.Description.HeaderText = "Description";
            this.Description.Name = "Description";
            this.Description.Visible = false;
            // 
            // FormReleaseNotes
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(540, 294);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtSearch);
            this.Controls.Add(this.txtUserID);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbVersionGroup);
            this.Controls.Add(this.dgvNotes);
            this.Controls.Add(this.panel5);
            this.MaximizeBox = false;
            this.Name = "FormReleaseNotes";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Release Notes";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormSetting_FormClosing);
            this.Load += new System.EventHandler(this.FormReleaseNotes_Load);
            this.panel5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvNotes)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.DataGridView dgvNotes;
        private System.Windows.Forms.ComboBox cmbVersionGroup;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtUserID;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtSearch;
        private System.ComponentModel.BackgroundWorker bgwDeleteInsert;
        private System.ComponentModel.BackgroundWorker bgwLoad;
        private System.Windows.Forms.DataGridViewTextBoxColumn ID;
        private System.Windows.Forms.DataGridViewTextBoxColumn SL;
        private System.Windows.Forms.DataGridViewTextBoxColumn Version;
        private System.Windows.Forms.DataGridViewTextBoxColumn Date;
        private System.Windows.Forms.DataGridViewTextBoxColumn Name1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Issue;
        private System.Windows.Forms.DataGridViewTextBoxColumn Description;
    }
}