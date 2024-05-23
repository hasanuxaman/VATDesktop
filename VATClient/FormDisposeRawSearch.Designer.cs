namespace VATClient
{
    partial class FormDisposeRawSearch
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
            this.txtRefNo = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.dgvDisposeRawsSearch = new System.Windows.Forms.DataGridView();
            this.Select = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.chkSelectAll = new System.Windows.Forms.CheckBox();
            this.label11 = new System.Windows.Forms.Label();
            this.dtpToDate = new System.Windows.Forms.DateTimePicker();
            this.dtpFromDate = new System.Windows.Forms.DateTimePicker();
            this.grbTransactionHistory = new System.Windows.Forms.GroupBox();
            this.cmbRecordCount = new System.Windows.Forms.ComboBox();
            this.cmbBranch = new System.Windows.Forms.ComboBox();
            this.label15 = new System.Windows.Forms.Label();
            this.btnPost = new System.Windows.Forms.Button();
            this.cmbPost = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnSearch = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.txtDisposeNo = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbExport = new System.Windows.Forms.ComboBox();
            this.btnImport = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.LRecordCount = new System.Windows.Forms.Label();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.bgwSearch = new System.ComponentModel.BackgroundWorker();
            this.bgwMultiplePost = new System.ComponentModel.BackgroundWorker();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDisposeRawsSearch)).BeginInit();
            this.grbTransactionHistory.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtRefNo
            // 
            this.txtRefNo.BackColor = System.Drawing.SystemColors.Window;
            this.txtRefNo.Location = new System.Drawing.Point(102, 47);
            this.txtRefNo.MaximumSize = new System.Drawing.Size(4, 5);
            this.txtRefNo.MinimumSize = new System.Drawing.Size(185, 20);
            this.txtRefNo.Name = "txtRefNo";
            this.txtRefNo.Size = new System.Drawing.Size(185, 20);
            this.txtRefNo.TabIndex = 1;
            this.txtRefNo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSerialNo_KeyDown);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(23, 53);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(64, 13);
            this.label6.TabIndex = 126;
            this.label6.Text = "Ref Number";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.progressBar1);
            this.groupBox1.Controls.Add(this.dgvDisposeRawsSearch);
            this.groupBox1.Location = new System.Drawing.Point(14, 141);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(756, 275);
            this.groupBox1.TabIndex = 116;
            this.groupBox1.TabStop = false;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(253, 129);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(250, 25);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 211;
            this.progressBar1.Visible = false;
            // 
            // dgvDisposeRawsSearch
            // 
            this.dgvDisposeRawsSearch.AllowUserToAddRows = false;
            this.dgvDisposeRawsSearch.AllowUserToDeleteRows = false;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.dgvDisposeRawsSearch.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvDisposeRawsSearch.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDisposeRawsSearch.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Select});
            this.dgvDisposeRawsSearch.Location = new System.Drawing.Point(6, 13);
            this.dgvDisposeRawsSearch.Name = "dgvDisposeRawsSearch";
            this.dgvDisposeRawsSearch.RowHeadersVisible = false;
            this.dgvDisposeRawsSearch.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvDisposeRawsSearch.Size = new System.Drawing.Size(744, 256);
            this.dgvDisposeRawsSearch.TabIndex = 6;
            this.dgvDisposeRawsSearch.DoubleClick += new System.EventHandler(this.dgvDisposeRawsSearch_DoubleClick);
            // 
            // Select
            // 
            this.Select.HeaderText = "Select";
            this.Select.Name = "Select";
            // 
            // chkSelectAll
            // 
            this.chkSelectAll.AutoSize = true;
            this.chkSelectAll.Location = new System.Drawing.Point(21, 105);
            this.chkSelectAll.Name = "chkSelectAll";
            this.chkSelectAll.Size = new System.Drawing.Size(66, 17);
            this.chkSelectAll.TabIndex = 117;
            this.chkSelectAll.Text = "SelectAll";
            this.chkSelectAll.UseVisualStyleBackColor = true;
            this.chkSelectAll.Click += new System.EventHandler(this.chkSelectAll_Click);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(202, 79);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(17, 13);
            this.label11.TabIndex = 112;
            this.label11.Text = "to";
            // 
            // dtpToDate
            // 
            this.dtpToDate.Checked = false;
            this.dtpToDate.CustomFormat = "dd/MMM/yyyy";
            this.dtpToDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpToDate.Location = new System.Drawing.Point(219, 75);
            this.dtpToDate.Name = "dtpToDate";
            this.dtpToDate.ShowCheckBox = true;
            this.dtpToDate.Size = new System.Drawing.Size(103, 21);
            this.dtpToDate.TabIndex = 3;
            this.dtpToDate.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dtpReceiveToDate_KeyDown);
            // 
            // dtpFromDate
            // 
            this.dtpFromDate.Checked = false;
            this.dtpFromDate.CustomFormat = "dd/MMM/yyyy";
            this.dtpFromDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpFromDate.Location = new System.Drawing.Point(100, 75);
            this.dtpFromDate.Name = "dtpFromDate";
            this.dtpFromDate.ShowCheckBox = true;
            this.dtpFromDate.Size = new System.Drawing.Size(102, 21);
            this.dtpFromDate.TabIndex = 2;
            this.dtpFromDate.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dtpReceiveFromDate_KeyDown);
            // 
            // grbTransactionHistory
            // 
            this.grbTransactionHistory.Controls.Add(this.cmbRecordCount);
            this.grbTransactionHistory.Controls.Add(this.chkSelectAll);
            this.grbTransactionHistory.Controls.Add(this.cmbBranch);
            this.grbTransactionHistory.Controls.Add(this.btnCancel);
            this.grbTransactionHistory.Controls.Add(this.label15);
            this.grbTransactionHistory.Controls.Add(this.btnPost);
            this.grbTransactionHistory.Controls.Add(this.cmbPost);
            this.grbTransactionHistory.Controls.Add(this.label2);
            this.grbTransactionHistory.Controls.Add(this.txtRefNo);
            this.grbTransactionHistory.Controls.Add(this.label6);
            this.grbTransactionHistory.Controls.Add(this.label11);
            this.grbTransactionHistory.Controls.Add(this.dtpToDate);
            this.grbTransactionHistory.Controls.Add(this.dtpFromDate);
            this.grbTransactionHistory.Controls.Add(this.btnSearch);
            this.grbTransactionHistory.Controls.Add(this.label3);
            this.grbTransactionHistory.Controls.Add(this.txtDisposeNo);
            this.grbTransactionHistory.Controls.Add(this.label1);
            this.grbTransactionHistory.Location = new System.Drawing.Point(14, 7);
            this.grbTransactionHistory.Name = "grbTransactionHistory";
            this.grbTransactionHistory.Size = new System.Drawing.Size(756, 132);
            this.grbTransactionHistory.TabIndex = 115;
            this.grbTransactionHistory.TabStop = false;
            this.grbTransactionHistory.Text = "Searching Criteria";
            // 
            // cmbRecordCount
            // 
            this.cmbRecordCount.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRecordCount.FormattingEnabled = true;
            this.cmbRecordCount.Location = new System.Drawing.Point(100, 103);
            this.cmbRecordCount.Name = "cmbRecordCount";
            this.cmbRecordCount.Size = new System.Drawing.Size(78, 21);
            this.cmbRecordCount.TabIndex = 235;
            // 
            // cmbBranch
            // 
            this.cmbBranch.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBranch.FormattingEnabled = true;
            this.cmbBranch.Items.AddRange(new object[] {
            "Y",
            "N"});
            this.cmbBranch.Location = new System.Drawing.Point(415, 47);
            this.cmbBranch.Name = "cmbBranch";
            this.cmbBranch.Size = new System.Drawing.Size(125, 21);
            this.cmbBranch.TabIndex = 226;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(359, 55);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(40, 13);
            this.label15.TabIndex = 225;
            this.label15.Text = "Branch";
            // 
            // btnPost
            // 
            this.btnPost.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnPost.Image = global::VATClient.Properties.Resources.Post;
            this.btnPost.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPost.Location = new System.Drawing.Point(674, 79);
            this.btnPost.Name = "btnPost";
            this.btnPost.Size = new System.Drawing.Size(75, 28);
            this.btnPost.TabIndex = 221;
            this.btnPost.Text = "Post";
            this.btnPost.UseVisualStyleBackColor = false;
            this.btnPost.Click += new System.EventHandler(this.btnPost_Click);
            // 
            // cmbPost
            // 
            this.cmbPost.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPost.FormattingEnabled = true;
            this.cmbPost.Items.AddRange(new object[] {
            "Y",
            "N"});
            this.cmbPost.Location = new System.Drawing.Point(415, 19);
            this.cmbPost.Name = "cmbPost";
            this.cmbPost.Size = new System.Drawing.Size(125, 21);
            this.cmbPost.TabIndex = 8;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(359, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(28, 13);
            this.label2.TabIndex = 200;
            this.label2.Text = "Post";
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSearch.Image = global::VATClient.Properties.Resources.search;
            this.btnSearch.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSearch.Location = new System.Drawing.Point(674, 19);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 28);
            this.btnSearch.TabIndex = 10;
            this.btnSearch.Text = "&Search";
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(23, 79);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(30, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Date";
            // 
            // txtDisposeNo
            // 
            this.txtDisposeNo.Location = new System.Drawing.Point(102, 19);
            this.txtDisposeNo.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtDisposeNo.MinimumSize = new System.Drawing.Size(185, 20);
            this.txtDisposeNo.Name = "txtDisposeNo";
            this.txtDisposeNo.Size = new System.Drawing.Size(185, 21);
            this.txtDisposeNo.TabIndex = 0;
            this.txtDisposeNo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtReceiveNo_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(23, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Dispose No";
            // 
            // cmbExport
            // 
            this.cmbExport.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbExport.FormattingEnabled = true;
            this.cmbExport.Items.AddRange(new object[] {
            "EXCEL",
            "TEXT"});
            this.cmbExport.Location = new System.Drawing.Point(324, 10);
            this.cmbExport.Name = "cmbExport";
            this.cmbExport.Size = new System.Drawing.Size(70, 21);
            this.cmbExport.TabIndex = 234;
            // 
            // btnImport
            // 
            this.btnImport.BackColor = System.Drawing.Color.White;
            this.btnImport.Image = global::VATClient.Properties.Resources.Load;
            this.btnImport.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnImport.Location = new System.Drawing.Point(416, 6);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(75, 28);
            this.btnImport.TabIndex = 117;
            this.btnImport.Text = "Export";
            this.btnImport.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnImport.UseVisualStyleBackColor = false;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::VATClient.Properties.Resources.Green_Footer;
            this.panel1.Controls.Add(this.LRecordCount);
            this.panel1.Controls.Add(this.cmbExport);
            this.panel1.Controls.Add(this.btnOk);
            this.panel1.Controls.Add(this.btnImport);
            this.panel1.Location = new System.Drawing.Point(3, 422);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(781, 40);
            this.panel1.TabIndex = 11;
            this.panel1.TabStop = true;
            // 
            // LRecordCount
            // 
            this.LRecordCount.AutoSize = true;
            this.LRecordCount.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LRecordCount.Location = new System.Drawing.Point(3, 12);
            this.LRecordCount.Name = "LRecordCount";
            this.LRecordCount.Size = new System.Drawing.Size(94, 16);
            this.LRecordCount.TabIndex = 221;
            this.LRecordCount.Text = "Record Count :";
            // 
            // btnOk
            // 
            this.btnOk.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnOk.Image = global::VATClient.Properties.Resources.Back;
            this.btnOk.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOk.Location = new System.Drawing.Point(699, 6);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 28);
            this.btnOk.TabIndex = 13;
            this.btnOk.Text = "&Ok";
            this.btnOk.UseVisualStyleBackColor = false;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnCancel.Image = global::VATClient.Properties.Resources.Referesh;
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.Location = new System.Drawing.Point(674, 47);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 28);
            this.btnCancel.TabIndex = 12;
            this.btnCancel.Text = "&Refresh";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // bgwSearch
            // 
            this.bgwSearch.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwSearch_DoWork);
            this.bgwSearch.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwSearch_RunWorkerCompleted);
            // 
            // bgwMultiplePost
            // 
            this.bgwMultiplePost.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwMultiplePost_DoWork);
            this.bgwMultiplePost.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwMultiplePost_RunWorkerCompleted);
            // 
            // FormDisposeRawsSearch
            // 
            this.AcceptButton = this.btnSearch;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.CancelButton = this.btnOk;
            this.ClientSize = new System.Drawing.Size(784, 461);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.grbTransactionHistory);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(800, 500);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(800, 500);
            this.Name = "FormDisposeRawsSearch";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "DisposeRaws Search";
            this.Load += new System.EventHandler(this.FormReceiveSearch_Load);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDisposeRawsSearch)).EndInit();
            this.grbTransactionHistory.ResumeLayout(false);
            this.grbTransactionHistory.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TextBox txtRefNo;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridView dgvDisposeRawsSearch;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.DateTimePicker dtpToDate;
        private System.Windows.Forms.DateTimePicker dtpFromDate;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.GroupBox grbTransactionHistory;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtDisposeNo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbPost;
        private System.Windows.Forms.Label label2;
        private System.ComponentModel.BackgroundWorker bgwSearch;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label LRecordCount;
        private System.Windows.Forms.Button btnPost;
        private System.ComponentModel.BackgroundWorker bgwMultiplePost;
        private System.Windows.Forms.CheckBox chkSelectAll;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.ComboBox cmbBranch;
        private System.Windows.Forms.Button btnImport;
        private System.Windows.Forms.ComboBox cmbExport;
        public System.Windows.Forms.ComboBox cmbRecordCount;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Select;
    }
}