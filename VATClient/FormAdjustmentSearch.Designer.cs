namespace VATClient
{
    partial class FormAdjustmentSearch
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAdjustmentSearch));
            this.dgvAdjHistory = new System.Windows.Forms.DataGridView();
            this.btnSearch = new System.Windows.Forms.Button();
            this.bgwSearch = new System.ComponentModel.BackgroundWorker();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.label4 = new System.Windows.Forms.Label();
            this.cmbAdjType = new System.Windows.Forms.ComboBox();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.txtAdjHistoryNo = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtAdjReferance = new System.Windows.Forms.TextBox();
            this.cmbPost = new System.Windows.Forms.ComboBox();
            this.label19 = new System.Windows.Forms.Label();
            this.LRecordCount = new System.Windows.Forms.Label();
            this.dtpAdjToDate = new System.Windows.Forms.DateTimePicker();
            this.dtpAdjFromDate = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.cmbBranch = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.AdjHistoryNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AdjName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AdjDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AdjInputAmount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AdjInputPercent = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AdjAmount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AdjReferance = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AdjDescription = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Post = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AdjType1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AdjHistoryID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AdjId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BranchId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.IsAdjSD = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAdjHistory)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvAdjHistory
            // 
            this.dgvAdjHistory.AllowUserToAddRows = false;
            this.dgvAdjHistory.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black;
            this.dgvAdjHistory.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvAdjHistory.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAdjHistory.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.AdjHistoryNo,
            this.AdjName,
            this.AdjDate,
            this.AdjInputAmount,
            this.AdjInputPercent,
            this.AdjAmount,
            this.AdjReferance,
            this.AdjDescription,
            this.Post,
            this.AdjType1,
            this.AdjHistoryID,
            this.AdjId,
            this.BranchId,
            this.IsAdjSD});
            this.dgvAdjHistory.Location = new System.Drawing.Point(16, 124);
            this.dgvAdjHistory.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.dgvAdjHistory.Name = "dgvAdjHistory";
            this.dgvAdjHistory.RowHeadersVisible = false;
            this.dgvAdjHistory.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvAdjHistory.Size = new System.Drawing.Size(992, 236);
            this.dgvAdjHistory.TabIndex = 7;
            this.dgvAdjHistory.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSalesHistory_CellContentClick_1);
            this.dgvAdjHistory.DoubleClick += new System.EventHandler(this.dgvSalesHistory_DoubleClick);
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSearch.Image = global::VATClient.Properties.Resources.search;
            this.btnSearch.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSearch.Location = new System.Drawing.Point(872, 15);
            this.btnSearch.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(100, 34);
            this.btnSearch.TabIndex = 4;
            this.btnSearch.Text = "&Search";
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnVendorGroup_Click);
            // 
            // bgwSearch
            // 
            this.bgwSearch.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwSearch_DoWork);
            this.bgwSearch.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwSearch_RunWorkerCompleted);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(320, 199);
            this.progressBar1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(384, 30);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 213;
            this.progressBar1.Visible = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(341, 14);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(64, 17);
            this.label4.TabIndex = 215;
            this.label4.Text = "Adj Type";
            // 
            // cmbAdjType
            // 
            this.cmbAdjType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAdjType.FormattingEnabled = true;
            this.cmbAdjType.Items.AddRange(new object[] {
            "Cash Payable",
            "Credit Payable",
            "Rebatable"});
            this.cmbAdjType.Location = new System.Drawing.Point(441, 9);
            this.cmbAdjType.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmbAdjType.Name = "cmbAdjType";
            this.cmbAdjType.Size = new System.Drawing.Size(133, 24);
            this.cmbAdjType.Sorted = true;
            this.cmbAdjType.TabIndex = 2;
            this.cmbAdjType.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmbAdjType_KeyDown);
            // 
            // btnRefresh
            // 
            this.btnRefresh.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnRefresh.Image = ((System.Drawing.Image)(resources.GetObject("btnRefresh.Image")));
            this.btnRefresh.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnRefresh.Location = new System.Drawing.Point(33, 11);
            this.btnRefresh.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(100, 34);
            this.btnRefresh.TabIndex = 5;
            this.btnRefresh.Text = "&Refresh";
            this.btnRefresh.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnRefresh.UseVisualStyleBackColor = false;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnOk
            // 
            this.btnOk.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnOk.Image = global::VATClient.Properties.Resources.Back;
            this.btnOk.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOk.Location = new System.Drawing.Point(856, 11);
            this.btnOk.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(100, 34);
            this.btnOk.TabIndex = 6;
            this.btnOk.Text = "&Ok";
            this.btnOk.UseVisualStyleBackColor = false;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(55, 14);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(41, 17);
            this.label9.TabIndex = 221;
            this.label9.Text = "Code";
            // 
            // txtAdjHistoryNo
            // 
            this.txtAdjHistoryNo.Location = new System.Drawing.Point(140, 9);
            this.txtAdjHistoryNo.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtAdjHistoryNo.MaximumSize = new System.Drawing.Size(159, 200);
            this.txtAdjHistoryNo.MinimumSize = new System.Drawing.Size(159, 20);
            this.txtAdjHistoryNo.Name = "txtAdjHistoryNo";
            this.txtAdjHistoryNo.Size = new System.Drawing.Size(159, 22);
            this.txtAdjHistoryNo.TabIndex = 0;
            this.txtAdjHistoryNo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtAdjHistoryNo_KeyDown);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(55, 44);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(74, 17);
            this.label8.TabIndex = 219;
            this.label8.Text = "Referance";
            // 
            // txtAdjReferance
            // 
            this.txtAdjReferance.Location = new System.Drawing.Point(140, 39);
            this.txtAdjReferance.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtAdjReferance.MaximumSize = new System.Drawing.Size(159, 200);
            this.txtAdjReferance.MinimumSize = new System.Drawing.Size(159, 20);
            this.txtAdjReferance.Name = "txtAdjReferance";
            this.txtAdjReferance.Size = new System.Drawing.Size(159, 22);
            this.txtAdjReferance.TabIndex = 1;
            this.txtAdjReferance.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtAdjReferance_KeyDown);
            // 
            // cmbPost
            // 
            this.cmbPost.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPost.FormattingEnabled = true;
            this.cmbPost.Items.AddRange(new object[] {
            "Y",
            "N"});
            this.cmbPost.Location = new System.Drawing.Point(685, 7);
            this.cmbPost.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmbPost.Name = "cmbPost";
            this.cmbPost.Size = new System.Drawing.Size(69, 24);
            this.cmbPost.TabIndex = 3;
            this.cmbPost.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmbPost_KeyDown);
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(617, 15);
            this.label19.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(36, 17);
            this.label19.TabIndex = 222;
            this.label19.Text = "Post";
            // 
            // LRecordCount
            // 
            this.LRecordCount.AutoSize = true;
            this.LRecordCount.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LRecordCount.Location = new System.Drawing.Point(141, 20);
            this.LRecordCount.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LRecordCount.Name = "LRecordCount";
            this.LRecordCount.Size = new System.Drawing.Size(121, 21);
            this.LRecordCount.TabIndex = 230;
            this.LRecordCount.Text = "Record Count :";
            // 
            // dtpAdjToDate
            // 
            this.dtpAdjToDate.Checked = false;
            this.dtpAdjToDate.CustomFormat = "dd/MMM/yyyy";
            this.dtpAdjToDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpAdjToDate.Location = new System.Drawing.Point(612, 39);
            this.dtpAdjToDate.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.dtpAdjToDate.Name = "dtpAdjToDate";
            this.dtpAdjToDate.ShowCheckBox = true;
            this.dtpAdjToDate.Size = new System.Drawing.Size(143, 22);
            this.dtpAdjToDate.TabIndex = 232;
            // 
            // dtpAdjFromDate
            // 
            this.dtpAdjFromDate.Checked = false;
            this.dtpAdjFromDate.CustomFormat = "dd/MMM/yyyy";
            this.dtpAdjFromDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpAdjFromDate.Location = new System.Drawing.Point(441, 39);
            this.dtpAdjFromDate.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.dtpAdjFromDate.Name = "dtpAdjFromDate";
            this.dtpAdjFromDate.ShowCheckBox = true;
            this.dtpAdjFromDate.Size = new System.Drawing.Size(133, 22);
            this.dtpAdjFromDate.TabIndex = 231;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(315, 44);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(90, 17);
            this.label3.TabIndex = 233;
            this.label3.Text = "Deposit Date";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(583, 47);
            this.label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(20, 17);
            this.label11.TabIndex = 234;
            this.label11.Text = "to";
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::VATClient.Properties.Resources.Green_Footer;
            this.panel1.Controls.Add(this.btnOk);
            this.panel1.Controls.Add(this.btnRefresh);
            this.panel1.Controls.Add(this.LRecordCount);
            this.panel1.Location = new System.Drawing.Point(15, 368);
            this.panel1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(993, 49);
            this.panel1.TabIndex = 235;
            // 
            // cmbBranch
            // 
            this.cmbBranch.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBranch.FormattingEnabled = true;
            this.cmbBranch.Items.AddRange(new object[] {
            "Cash Payable",
            "Credit Payable",
            "Rebatable"});
            this.cmbBranch.Location = new System.Drawing.Point(140, 71);
            this.cmbBranch.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmbBranch.Name = "cmbBranch";
            this.cmbBranch.Size = new System.Drawing.Size(159, 24);
            this.cmbBranch.Sorted = true;
            this.cmbBranch.TabIndex = 2;
            this.cmbBranch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmbAdjType_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(65, 76);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 17);
            this.label1.TabIndex = 215;
            this.label1.Text = "Branch";
            // 
            // AdjHistoryNo
            // 
            this.AdjHistoryNo.HeaderText = "AdjHistoryNo";
            this.AdjHistoryNo.Name = "AdjHistoryNo";
            this.AdjHistoryNo.ReadOnly = true;
            // 
            // AdjName
            // 
            this.AdjName.HeaderText = "AdjName";
            this.AdjName.Name = "AdjName";
            this.AdjName.ReadOnly = true;
            // 
            // AdjDate
            // 
            this.AdjDate.HeaderText = "AdjDate";
            this.AdjDate.Name = "AdjDate";
            this.AdjDate.ReadOnly = true;
            // 
            // AdjInputAmount
            // 
            this.AdjInputAmount.HeaderText = "AdjInputAmount";
            this.AdjInputAmount.Name = "AdjInputAmount";
            this.AdjInputAmount.ReadOnly = true;
            // 
            // AdjInputPercent
            // 
            this.AdjInputPercent.HeaderText = "AdjInputPercent";
            this.AdjInputPercent.Name = "AdjInputPercent";
            this.AdjInputPercent.ReadOnly = true;
            // 
            // AdjAmount
            // 
            this.AdjAmount.HeaderText = "AdjAmount";
            this.AdjAmount.Name = "AdjAmount";
            this.AdjAmount.ReadOnly = true;
            // 
            // AdjReferance
            // 
            this.AdjReferance.HeaderText = "AdjReferance";
            this.AdjReferance.Name = "AdjReferance";
            this.AdjReferance.ReadOnly = true;
            // 
            // AdjDescription
            // 
            this.AdjDescription.HeaderText = "AdjDescription";
            this.AdjDescription.Name = "AdjDescription";
            this.AdjDescription.ReadOnly = true;
            // 
            // Post
            // 
            this.Post.HeaderText = "Post";
            this.Post.Name = "Post";
            this.Post.ReadOnly = true;
            // 
            // AdjType1
            // 
            this.AdjType1.HeaderText = "AdjType";
            this.AdjType1.Name = "AdjType1";
            this.AdjType1.ReadOnly = true;
            // 
            // AdjHistoryID
            // 
            this.AdjHistoryID.HeaderText = "AdjHistoryID";
            this.AdjHistoryID.Name = "AdjHistoryID";
            this.AdjHistoryID.ReadOnly = true;
            this.AdjHistoryID.Visible = false;
            // 
            // AdjId
            // 
            this.AdjId.HeaderText = "AdjId";
            this.AdjId.Name = "AdjId";
            this.AdjId.ReadOnly = true;
            this.AdjId.Visible = false;
            // 
            // BranchId
            // 
            this.BranchId.HeaderText = "BranchId";
            this.BranchId.Name = "BranchId";
            this.BranchId.ReadOnly = true;
            this.BranchId.Visible = false;
            // 
            // IsAdjSD
            // 
            this.IsAdjSD.HeaderText = "AdjSD";
            this.IsAdjSD.Name = "IsAdjSD";
            // 
            // FormAdjustmentSearch
            // 
            this.AcceptButton = this.btnSearch;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(1013, 420);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.dtpAdjToDate);
            this.Controls.Add(this.dtpAdjFromDate);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cmbPost);
            this.Controls.Add(this.label19);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.txtAdjHistoryNo);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.txtAdjReferance);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cmbBranch);
            this.Controls.Add(this.cmbAdjType);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.dgvAdjHistory);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "FormAdjustmentSearch";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FormAdjustmentSearch";
            this.Load += new System.EventHandler(this.FormAdjustmentSearch_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvAdjHistory)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvAdjHistory;

        private System.Windows.Forms.Button btnSearch;
        private System.ComponentModel.BackgroundWorker bgwSearch;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnOk;
        public System.Windows.Forms.ComboBox cmbAdjType;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtAdjHistoryNo;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtAdjReferance;

        private System.Windows.Forms.ComboBox cmbPost;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label LRecordCount;
        private System.Windows.Forms.DateTimePicker dtpAdjToDate;
        private System.Windows.Forms.DateTimePicker dtpAdjFromDate;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Panel panel1;
        public System.Windows.Forms.ComboBox cmbBranch;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridViewTextBoxColumn AdjHistoryNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn AdjName;
        private System.Windows.Forms.DataGridViewTextBoxColumn AdjDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn AdjInputAmount;
        private System.Windows.Forms.DataGridViewTextBoxColumn AdjInputPercent;
        private System.Windows.Forms.DataGridViewTextBoxColumn AdjAmount;
        private System.Windows.Forms.DataGridViewTextBoxColumn AdjReferance;
        private System.Windows.Forms.DataGridViewTextBoxColumn AdjDescription;
        private System.Windows.Forms.DataGridViewTextBoxColumn Post;
        private System.Windows.Forms.DataGridViewTextBoxColumn AdjType1;
        private System.Windows.Forms.DataGridViewTextBoxColumn AdjHistoryID;
        private System.Windows.Forms.DataGridViewTextBoxColumn AdjId;
        private System.Windows.Forms.DataGridViewTextBoxColumn BranchId;
        private System.Windows.Forms.DataGridViewTextBoxColumn IsAdjSD;
        
    }
}