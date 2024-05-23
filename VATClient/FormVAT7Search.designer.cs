namespace VATClient
{
    partial class FormVAT7Search
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.dgvIssueHistory = new System.Windows.Forms.DataGridView();
            this.label2 = new System.Windows.Forms.Label();
            this.grbTransactionHistory = new System.Windows.Forms.GroupBox();
            this.cmbPost = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.dtpVAT7ToDate = new System.Windows.Forms.DateTimePicker();
            this.dtpVAT7FromDate = new System.Windows.Forms.DateTimePicker();
            this.btnSearch = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.txtVAT7No = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.LRecordCount = new System.Windows.Forms.Label();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.backgroundWorkerSearch = new System.ComponentModel.BackgroundWorker();
            this.VAT7No = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.VAT7Date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Post = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvIssueHistory)).BeginInit();
            this.grbTransactionHistory.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.progressBar1);
            this.groupBox1.Controls.Add(this.dgvIssueHistory);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(6, 86);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(473, 194);
            this.groupBox1.TabIndex = 108;
            this.groupBox1.TabStop = false;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(44, 71);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(383, 25);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 109;
            this.progressBar1.Visible = false;
            // 
            // dgvIssueHistory
            // 
            this.dgvIssueHistory.AllowUserToAddRows = false;
            this.dgvIssueHistory.AllowUserToDeleteRows = false;
            this.dgvIssueHistory.AllowUserToOrderColumns = true;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black;
            this.dgvIssueHistory.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvIssueHistory.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvIssueHistory.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvIssueHistory.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.VAT7No,
            this.VAT7Date,
            this.Post});
            this.dgvIssueHistory.Location = new System.Drawing.Point(6, 12);
            this.dgvIssueHistory.Name = "dgvIssueHistory";
            this.dgvIssueHistory.RowHeadersVisible = false;
            this.dgvIssueHistory.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvIssueHistory.Size = new System.Drawing.Size(460, 174);
            this.dgvIssueHistory.TabIndex = 6;
            this.dgvIssueHistory.DoubleClick += new System.EventHandler(this.dgvIssueHistory_DoubleClick);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(688, 83);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Serial No:";
            // 
            // grbTransactionHistory
            // 
            this.grbTransactionHistory.Controls.Add(this.cmbPost);
            this.grbTransactionHistory.Controls.Add(this.label9);
            this.grbTransactionHistory.Controls.Add(this.label11);
            this.grbTransactionHistory.Controls.Add(this.dtpVAT7ToDate);
            this.grbTransactionHistory.Controls.Add(this.dtpVAT7FromDate);
            this.grbTransactionHistory.Controls.Add(this.btnSearch);
            this.grbTransactionHistory.Controls.Add(this.label3);
            this.grbTransactionHistory.Controls.Add(this.txtVAT7No);
            this.grbTransactionHistory.Controls.Add(this.label1);
            this.grbTransactionHistory.Location = new System.Drawing.Point(4, 4);
            this.grbTransactionHistory.Name = "grbTransactionHistory";
            this.grbTransactionHistory.Size = new System.Drawing.Size(475, 75);
            this.grbTransactionHistory.TabIndex = 107;
            this.grbTransactionHistory.TabStop = false;
            this.grbTransactionHistory.Text = "Searching Criteria";
            // 
            // cmbPost
            // 
            this.cmbPost.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPost.FormattingEnabled = true;
            this.cmbPost.Items.AddRange(new object[] {
            "Y",
            "N"});
            this.cmbPost.Location = new System.Drawing.Point(304, 21);
            this.cmbPost.Name = "cmbPost";
            this.cmbPost.Size = new System.Drawing.Size(125, 21);
            this.cmbPost.TabIndex = 4;
            this.cmbPost.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmbPost_KeyDown);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(270, 26);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(28, 13);
            this.label9.TabIndex = 200;
            this.label9.Text = "Post";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(187, 50);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(17, 13);
            this.label11.TabIndex = 112;
            this.label11.Text = "to";
            // 
            // dtpVAT7ToDate
            // 
            this.dtpVAT7ToDate.Checked = false;
            this.dtpVAT7ToDate.CustomFormat = "d/MMM/yyyy";
            this.dtpVAT7ToDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpVAT7ToDate.Location = new System.Drawing.Point(205, 46);
            this.dtpVAT7ToDate.Name = "dtpVAT7ToDate";
            this.dtpVAT7ToDate.ShowCheckBox = true;
            this.dtpVAT7ToDate.Size = new System.Drawing.Size(103, 21);
            this.dtpVAT7ToDate.TabIndex = 2;
            this.dtpVAT7ToDate.ValueChanged += new System.EventHandler(this.dtpIssueToDate_ValueChanged);
            this.dtpVAT7ToDate.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dtpVAT7ToDate_KeyDown);
            // 
            // dtpVAT7FromDate
            // 
            this.dtpVAT7FromDate.Checked = false;
            this.dtpVAT7FromDate.CustomFormat = "d/MMM/yyyy";
            this.dtpVAT7FromDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpVAT7FromDate.Location = new System.Drawing.Point(83, 46);
            this.dtpVAT7FromDate.Name = "dtpVAT7FromDate";
            this.dtpVAT7FromDate.ShowCheckBox = true;
            this.dtpVAT7FromDate.Size = new System.Drawing.Size(102, 21);
            this.dtpVAT7FromDate.TabIndex = 1;
            this.dtpVAT7FromDate.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dtpVAT7FromDate_KeyDown);
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSearch.Image = global::VATClient.Properties.Resources.search;
            this.btnSearch.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSearch.Location = new System.Drawing.Point(354, 44);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 28);
            this.btnSearch.TabIndex = 5;
            this.btnSearch.Text = "&Search";
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 50);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "VAT 7 Date:";
            // 
            // txtVAT7No
            // 
            this.txtVAT7No.Location = new System.Drawing.Point(83, 21);
            this.txtVAT7No.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtVAT7No.MinimumSize = new System.Drawing.Size(150, 20);
            this.txtVAT7No.Name = "txtVAT7No";
            this.txtVAT7No.Size = new System.Drawing.Size(150, 20);
            this.txtVAT7No.TabIndex = 0;
            this.txtVAT7No.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtVAT7No_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "VAT 7 No";
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::VATClient.Properties.Resources.Green_Footer;
            this.panel1.Controls.Add(this.LRecordCount);
            this.panel1.Controls.Add(this.btnOk);
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Location = new System.Drawing.Point(0, 294);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(487, 40);
            this.panel1.TabIndex = 6;
            // 
            // LRecordCount
            // 
            this.LRecordCount.AutoSize = true;
            this.LRecordCount.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LRecordCount.Location = new System.Drawing.Point(98, 13);
            this.LRecordCount.Name = "LRecordCount";
            this.LRecordCount.Size = new System.Drawing.Size(94, 16);
            this.LRecordCount.TabIndex = 223;
            this.LRecordCount.Text = "Record Count :";
            // 
            // btnOk
            // 
            this.btnOk.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnOk.Image = global::VATClient.Properties.Resources.Back;
            this.btnOk.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOk.Location = new System.Drawing.Point(397, 7);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 28);
            this.btnOk.TabIndex = 8;
            this.btnOk.Text = "&Ok";
            this.btnOk.UseVisualStyleBackColor = false;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnCancel.Image = global::VATClient.Properties.Resources.Referesh;
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.Location = new System.Drawing.Point(17, 7);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 28);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "&Refresh";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // backgroundWorkerSearch
            // 
            this.backgroundWorkerSearch.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerSearch_DoWork);
            this.backgroundWorkerSearch.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerSearch_RunWorkerCompleted);
            // 
            // VAT7No
            // 
            this.VAT7No.HeaderText = "VAT 7 No";
            this.VAT7No.Name = "VAT7No";
            this.VAT7No.ReadOnly = true;
            this.VAT7No.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.VAT7No.Width = 200;
            // 
            // VAT7Date
            // 
            this.VAT7Date.HeaderText = "VAT 7 Date";
            this.VAT7Date.Name = "VAT7Date";
            this.VAT7Date.ReadOnly = true;
            this.VAT7Date.Width = 150;
            // 
            // Post
            // 
            this.Post.HeaderText = "Post";
            this.Post.Name = "Post";
            this.Post.ReadOnly = true;
            // 
            // FormVAT7Search
            // 
            this.AcceptButton = this.btnSearch;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.CancelButton = this.btnOk;
            this.ClientSize = new System.Drawing.Size(484, 337);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.grbTransactionHistory);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(500, 375);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(500, 375);
            this.Name = "FormVAT7Search";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "VAT7 Search";
            this.Load += new System.EventHandler(this.FormVAT7Search_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvIssueHistory)).EndInit();
            this.grbTransactionHistory.ResumeLayout(false);
            this.grbTransactionHistory.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridView dgvIssueHistory;
        private System.Windows.Forms.GroupBox grbTransactionHistory;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.DateTimePicker dtpVAT7ToDate;
        private System.Windows.Forms.DateTimePicker dtpVAT7FromDate;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtVAT7No;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbPost;
        private System.Windows.Forms.Label label9;
        private System.ComponentModel.BackgroundWorker backgroundWorkerSearch;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label LRecordCount;
        private System.Windows.Forms.DataGridViewTextBoxColumn VAT7No;
        private System.Windows.Forms.DataGridViewTextBoxColumn VAT7Date;
        private System.Windows.Forms.DataGridViewTextBoxColumn Post;
    }
}