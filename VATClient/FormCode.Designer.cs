namespace VATClient
{
	partial class FormCode
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
            this.btnUpdate = new System.Windows.Forms.Button();
            this.dgvCode = new System.Windows.Forms.DataGridView();
            this.CodeId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CodeGroup = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CodeName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.prefix = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Lenth = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.prefixOld = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.backgroundWorkerLoad = new System.ComponentModel.BackgroundWorker();
            this.backgroundWorkerUpdate = new System.ComponentModel.BackgroundWorker();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbCodeGroup = new System.Windows.Forms.ComboBox();
            this.bgwSettingsValue = new System.ComponentModel.BackgroundWorker();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.panel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCode)).BeginInit();
            this.SuspendLayout();
            // 
            // panel5
            // 
            this.panel5.BackgroundImage = global::VATClient.Properties.Resources.Green_Footer;
            this.panel5.Controls.Add(this.btnClose);
            this.panel5.Controls.Add(this.btnUpdate);
            this.panel5.Location = new System.Drawing.Point(-2, 272);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(469, 40);
            this.panel5.TabIndex = 1;
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnClose.Image = global::VATClient.Properties.Resources.Back;
            this.btnClose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnClose.Location = new System.Drawing.Point(375, 6);
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
            this.btnUpdate.Location = new System.Drawing.Point(16, 6);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(75, 28);
            this.btnUpdate.TabIndex = 2;
            this.btnUpdate.Text = "&Update";
            this.btnUpdate.UseVisualStyleBackColor = false;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // dgvCode
            // 
            this.dgvCode.AllowUserToAddRows = false;
            this.dgvCode.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.dgvCode.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvCode.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCode.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.CodeId,
            this.CodeGroup,
            this.CodeName,
            this.prefix,
            this.Lenth,
            this.prefixOld});
            this.dgvCode.Location = new System.Drawing.Point(12, 36);
            this.dgvCode.Name = "dgvCode";
            this.dgvCode.RowHeadersVisible = false;
            this.dgvCode.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvCode.Size = new System.Drawing.Size(444, 229);
            this.dgvCode.TabIndex = 22;
            this.dgvCode.TabStop = false;
            this.dgvCode.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dgvCode_CellBeginEdit);
            this.dgvCode.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvCode_CellContentClick);
            this.dgvCode.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvCode_CellEndEdit);
            // 
            // CodeId
            // 
            this.CodeId.DataPropertyName = "CodeId";
            this.CodeId.HeaderText = "Code Id";
            this.CodeId.Name = "CodeId";
            this.CodeId.ReadOnly = true;
            this.CodeId.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.CodeId.Visible = false;
            // 
            // CodeGroup
            // 
            this.CodeGroup.DataPropertyName = "CodeGroup";
            this.CodeGroup.HeaderText = "Code Group";
            this.CodeGroup.Name = "CodeGroup";
            this.CodeGroup.ReadOnly = true;
            this.CodeGroup.Width = 125;
            // 
            // CodeName
            // 
            this.CodeName.DataPropertyName = "CodeName";
            this.CodeName.HeaderText = "Code Name";
            this.CodeName.Name = "CodeName";
            this.CodeName.ReadOnly = true;
            // 
            // prefix
            // 
            this.prefix.DataPropertyName = "prefix";
            this.prefix.HeaderText = "Prefix";
            this.prefix.Name = "prefix";
            this.prefix.Width = 115;
            // 
            // Lenth
            // 
            this.Lenth.DataPropertyName = "Lenth";
            this.Lenth.HeaderText = "Length";
            this.Lenth.Name = "Lenth";
            // 
            // prefixOld
            // 
            this.prefixOld.DataPropertyName = "prefixOld";
            this.prefixOld.HeaderText = "prefixOld";
            this.prefixOld.Name = "prefixOld";
            this.prefixOld.ReadOnly = true;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(97, 119);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(290, 24);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 23;
            // 
            // backgroundWorkerLoad
            // 
            this.backgroundWorkerLoad.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerLoad_DoWork);
            this.backgroundWorkerLoad.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerLoad_RunWorkerCompleted);
            // 
            // backgroundWorkerUpdate
            // 
            this.backgroundWorkerUpdate.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerUpdate_DoWork);
            this.backgroundWorkerUpdate.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerUpdate_RunWorkerCompleted);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 13);
            this.label1.TabIndex = 24;
            this.label1.Text = "Code Group:";
            // 
            // cmbCodeGroup
            // 
            this.cmbCodeGroup.FormattingEnabled = true;
            this.cmbCodeGroup.Location = new System.Drawing.Point(87, 9);
            this.cmbCodeGroup.Name = "cmbCodeGroup";
            this.cmbCodeGroup.Size = new System.Drawing.Size(121, 21);
            this.cmbCodeGroup.Sorted = true;
            this.cmbCodeGroup.TabIndex = 0;
            this.cmbCodeGroup.SelectedIndexChanged += new System.EventHandler(this.cmbCodeGroup_SelectedIndexChanged);
            // 
            // bgwSettingsValue
            // 
            this.bgwSettingsValue.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwSettingsValue_DoWork);
            this.bgwSettingsValue.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwSettingsValue_RunWorkerCompleted);
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(268, 9);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(188, 20);
            this.txtSearch.TabIndex = 25;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(215, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 13);
            this.label2.TabIndex = 26;
            this.label2.Text = "Search:";
            // 
            // FormCode
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(469, 312);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtSearch);
            this.Controls.Add(this.cmbCodeGroup);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.dgvCode);
            this.Controls.Add(this.panel5);
            this.MaximizeBox = false;
            this.Name = "FormCode";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Code";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormCode_FormClosing);
            this.Load += new System.EventHandler(this.FormCode_Load);
            this.panel5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvCode)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.DataGridView dgvCode;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.ComponentModel.BackgroundWorker backgroundWorkerLoad;
        private System.ComponentModel.BackgroundWorker backgroundWorkerUpdate;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbCodeGroup;
        private System.ComponentModel.BackgroundWorker bgwSettingsValue;
        private System.Windows.Forms.DataGridViewTextBoxColumn CodeId;
        private System.Windows.Forms.DataGridViewTextBoxColumn CodeGroup;
        private System.Windows.Forms.DataGridViewTextBoxColumn CodeName;
        private System.Windows.Forms.DataGridViewTextBoxColumn prefix;
        private System.Windows.Forms.DataGridViewTextBoxColumn Lenth;
        private System.Windows.Forms.DataGridViewTextBoxColumn prefixOld;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.Button btnUpdate;
	}
}