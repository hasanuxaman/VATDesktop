namespace VATClient
{
    partial class FormMultipleVAT6_5
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMultipleVAT6_5));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.numericUDPrintCopy = new System.Windows.Forms.NumericUpDown();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.cmbPrinterName = new System.Windows.Forms.ComboBox();
            this.grbBankInformation = new System.Windows.Forms.GroupBox();
            this.comtt = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.cmbBranch = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtSalesInvoiceNo = new System.Windows.Forms.TextBox();
            this.cmbTransferTo = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnSearch = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.dtpToDate = new System.Windows.Forms.DateTimePicker();
            this.dtpFromDate = new System.Windows.Forms.DateTimePicker();
            this.label16 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.grbPrintInformation = new System.Windows.Forms.GroupBox();
            this.chk3rd = new System.Windows.Forms.CheckBox();
            this.chk2nd = new System.Windows.Forms.CheckBox();
            this.chk1st = new System.Windows.Forms.CheckBox();
            this.lblPrintPage = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnPreview = new System.Windows.Forms.Button();
            this.btnSelectedPrint = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkSelectAll = new System.Windows.Forms.CheckBox();
            this.dgvIssue = new System.Windows.Forms.DataGridView();
            this.Select = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TransferIssueNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TransactionDateTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BranchName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TransferBranchName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.numericUDPrintCopy)).BeginInit();
            this.grbBankInformation.SuspendLayout();
            this.grbPrintInformation.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvIssue)).BeginInit();
            this.SuspendLayout();
            // 
            // numericUDPrintCopy
            // 
            this.numericUDPrintCopy.Location = new System.Drawing.Point(252, 54);
            this.numericUDPrintCopy.Name = "numericUDPrintCopy";
            this.numericUDPrintCopy.Size = new System.Drawing.Size(76, 20);
            this.numericUDPrintCopy.TabIndex = 14;
            this.numericUDPrintCopy.Visible = false;
            // 
            // btnOk
            // 
            this.btnOk.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnOk.Image = global::VATClient.Properties.Resources.Print;
            this.btnOk.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOk.Location = new System.Drawing.Point(418, 397);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(62, 28);
            this.btnOk.TabIndex = 52;
            this.btnOk.Text = "&Print";
            this.btnOk.UseVisualStyleBackColor = false;
            this.btnOk.Visible = false;
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnCancel.Image = ((System.Drawing.Image)(resources.GetObject("btnCancel.Image")));
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.Location = new System.Drawing.Point(397, 6);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(62, 28);
            this.btnCancel.TabIndex = 52;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // cmbPrinterName
            // 
            this.cmbPrinterName.FormattingEnabled = true;
            this.cmbPrinterName.Location = new System.Drawing.Point(114, 361);
            this.cmbPrinterName.Name = "cmbPrinterName";
            this.cmbPrinterName.Size = new System.Drawing.Size(208, 21);
            this.cmbPrinterName.TabIndex = 53;
            this.cmbPrinterName.Leave += new System.EventHandler(this.cmbPrinterName_Leave);
            // 
            // grbBankInformation
            // 
            this.grbBankInformation.Controls.Add(this.comtt);
            this.grbBankInformation.Controls.Add(this.label7);
            this.grbBankInformation.Controls.Add(this.cmbBranch);
            this.grbBankInformation.Controls.Add(this.label6);
            this.grbBankInformation.Controls.Add(this.txtSalesInvoiceNo);
            this.grbBankInformation.Controls.Add(this.cmbTransferTo);
            this.grbBankInformation.Controls.Add(this.label2);
            this.grbBankInformation.Controls.Add(this.btnSearch);
            this.grbBankInformation.Controls.Add(this.label3);
            this.grbBankInformation.Controls.Add(this.dtpToDate);
            this.grbBankInformation.Controls.Add(this.dtpFromDate);
            this.grbBankInformation.Controls.Add(this.label16);
            this.grbBankInformation.Controls.Add(this.label4);
            this.grbBankInformation.Location = new System.Drawing.Point(22, 11);
            this.grbBankInformation.Name = "grbBankInformation";
            this.grbBankInformation.Size = new System.Drawing.Size(458, 99);
            this.grbBankInformation.TabIndex = 83;
            this.grbBankInformation.TabStop = false;
            this.grbBankInformation.Text = "Reporting Criteria";
            // 
            // comtt
            // 
            this.comtt.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comtt.FormattingEnabled = true;
            this.comtt.Items.AddRange(new object[] {
            "=All=",
            "6.1 Out",
            "6.2 Out"});
            this.comtt.Location = new System.Drawing.Point(309, 18);
            this.comtt.Name = "comtt";
            this.comtt.Size = new System.Drawing.Size(92, 21);
            this.comtt.TabIndex = 205;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(267, 22);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(38, 13);
            this.label7.TabIndex = 206;
            this.label7.Text = "TType";
            // 
            // cmbBranch
            // 
            this.cmbBranch.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBranch.FormattingEnabled = true;
            this.cmbBranch.Items.AddRange(new object[] {
            "Cash Payable",
            "Credit Payable",
            "Rebatable"});
            this.cmbBranch.Location = new System.Drawing.Point(92, 45);
            this.cmbBranch.Name = "cmbBranch";
            this.cmbBranch.Size = new System.Drawing.Size(143, 21);
            this.cmbBranch.Sorted = true;
            this.cmbBranch.TabIndex = 218;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(245, 49);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(59, 13);
            this.label6.TabIndex = 217;
            this.label6.Text = "TransferTo";
            // 
            // txtSalesInvoiceNo
            // 
            this.txtSalesInvoiceNo.BackColor = System.Drawing.SystemColors.Window;
            this.txtSalesInvoiceNo.Location = new System.Drawing.Point(92, 19);
            this.txtSalesInvoiceNo.MaximumSize = new System.Drawing.Size(4, 25);
            this.txtSalesInvoiceNo.MinimumSize = new System.Drawing.Size(150, 20);
            this.txtSalesInvoiceNo.Name = "txtSalesInvoiceNo";
            this.txtSalesInvoiceNo.Size = new System.Drawing.Size(150, 20);
            this.txtSalesInvoiceNo.TabIndex = 87;
            this.txtSalesInvoiceNo.TabStop = false;
            // 
            // cmbTransferTo
            // 
            this.cmbTransferTo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTransferTo.FormattingEnabled = true;
            this.cmbTransferTo.Items.AddRange(new object[] {
            "Cash Payable",
            "Credit Payable",
            "Rebatable"});
            this.cmbTransferTo.Location = new System.Drawing.Point(309, 45);
            this.cmbTransferTo.Name = "cmbTransferTo";
            this.cmbTransferTo.Size = new System.Drawing.Size(143, 21);
            this.cmbTransferTo.Sorted = true;
            this.cmbTransferTo.TabIndex = 216;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 13);
            this.label2.TabIndex = 88;
            this.label2.Text = "IssueNo";
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSearch.Image = global::VATClient.Properties.Resources.search;
            this.btnSearch.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSearch.Location = new System.Drawing.Point(377, 68);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 28);
            this.btnSearch.TabIndex = 8;
            this.btnSearch.Text = "&Search";
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(198, 76);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(20, 13);
            this.label3.TabIndex = 45;
            this.label3.Text = "To";
            // 
            // dtpToDate
            // 
            this.dtpToDate.CustomFormat = "dd/MMM/yyyy";
            this.dtpToDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpToDate.Location = new System.Drawing.Point(225, 72);
            this.dtpToDate.Name = "dtpToDate";
            this.dtpToDate.Size = new System.Drawing.Size(103, 20);
            this.dtpToDate.TabIndex = 43;
            // 
            // dtpFromDate
            // 
            this.dtpFromDate.CustomFormat = "dd/MMM/yyyy";
            this.dtpFromDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpFromDate.Location = new System.Drawing.Point(92, 72);
            this.dtpFromDate.Name = "dtpFromDate";
            this.dtpFromDate.Size = new System.Drawing.Size(103, 20);
            this.dtpFromDate.TabIndex = 42;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(11, 75);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(53, 13);
            this.label16.TabIndex = 39;
            this.label16.Text = "FromDate";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(11, 49);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 13);
            this.label4.TabIndex = 34;
            this.label4.Text = "Branch";
            // 
            // grbPrintInformation
            // 
            this.grbPrintInformation.Controls.Add(this.chk3rd);
            this.grbPrintInformation.Controls.Add(this.chk2nd);
            this.grbPrintInformation.Controls.Add(this.chk1st);
            this.grbPrintInformation.Controls.Add(this.lblPrintPage);
            this.grbPrintInformation.Controls.Add(this.label1);
            this.grbPrintInformation.Controls.Add(this.numericUDPrintCopy);
            this.grbPrintInformation.Location = new System.Drawing.Point(510, 313);
            this.grbPrintInformation.Name = "grbPrintInformation";
            this.grbPrintInformation.Size = new System.Drawing.Size(458, 107);
            this.grbPrintInformation.TabIndex = 84;
            this.grbPrintInformation.TabStop = false;
            this.grbPrintInformation.Text = "Print Information";
            // 
            // chk3rd
            // 
            this.chk3rd.AllowDrop = true;
            this.chk3rd.AutoSize = true;
            this.chk3rd.Location = new System.Drawing.Point(287, 81);
            this.chk3rd.Name = "chk3rd";
            this.chk3rd.Size = new System.Drawing.Size(41, 17);
            this.chk3rd.TabIndex = 56;
            this.chk3rd.Text = "3rd";
            this.chk3rd.UseVisualStyleBackColor = true;
            this.chk3rd.Visible = false;
            // 
            // chk2nd
            // 
            this.chk2nd.AllowDrop = true;
            this.chk2nd.AutoSize = true;
            this.chk2nd.Location = new System.Drawing.Point(237, 81);
            this.chk2nd.Name = "chk2nd";
            this.chk2nd.Size = new System.Drawing.Size(44, 17);
            this.chk2nd.TabIndex = 56;
            this.chk2nd.Text = "2nd";
            this.chk2nd.UseVisualStyleBackColor = true;
            this.chk2nd.Visible = false;
            // 
            // chk1st
            // 
            this.chk1st.AllowDrop = true;
            this.chk1st.AutoSize = true;
            this.chk1st.Location = new System.Drawing.Point(188, 81);
            this.chk1st.Name = "chk1st";
            this.chk1st.Size = new System.Drawing.Size(40, 17);
            this.chk1st.TabIndex = 56;
            this.chk1st.Text = "1st";
            this.chk1st.UseVisualStyleBackColor = true;
            this.chk1st.Visible = false;
            // 
            // lblPrintPage
            // 
            this.lblPrintPage.AutoSize = true;
            this.lblPrintPage.Location = new System.Drawing.Point(11, 81);
            this.lblPrintPage.Name = "lblPrintPage";
            this.lblPrintPage.Size = new System.Drawing.Size(170, 13);
            this.lblPrintPage.TabIndex = 54;
            this.lblPrintPage.Text = "Which page do you want  to print?";
            this.lblPrintPage.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 56);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(181, 13);
            this.label1.TabIndex = 54;
            this.label1.Text = "How many copies you want  to print?";
            this.label1.Visible = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(40, 364);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(68, 13);
            this.label5.TabIndex = 55;
            this.label5.Text = "Printer Name";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(79, 397);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(300, 23);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 85;
            this.progressBar1.Visible = false;
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel1.BackgroundImage")));
            this.panel1.Controls.Add(this.btnPreview);
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Controls.Add(this.btnSelectedPrint);
            this.panel1.Controls.Add(this.btnRefresh);
            this.panel1.Location = new System.Drawing.Point(10, 426);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(470, 40);
            this.panel1.TabIndex = 86;
            // 
            // btnPreview
            // 
            this.btnPreview.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnPreview.Image = global::VATClient.Properties.Resources.Print;
            this.btnPreview.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPreview.Location = new System.Drawing.Point(139, 5);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new System.Drawing.Size(74, 28);
            this.btnPreview.TabIndex = 54;
            this.btnPreview.Text = "Preview";
            this.btnPreview.UseVisualStyleBackColor = false;
            this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
            // 
            // btnSelectedPrint
            // 
            this.btnSelectedPrint.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSelectedPrint.Image = global::VATClient.Properties.Resources.Print;
            this.btnSelectedPrint.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSelectedPrint.Location = new System.Drawing.Point(3, 5);
            this.btnSelectedPrint.Name = "btnSelectedPrint";
            this.btnSelectedPrint.Size = new System.Drawing.Size(74, 28);
            this.btnSelectedPrint.TabIndex = 52;
            this.btnSelectedPrint.Text = "Print";
            this.btnSelectedPrint.UseVisualStyleBackColor = false;
            this.btnSelectedPrint.Click += new System.EventHandler(this.btnSelectedPrint_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnRefresh.Image = global::VATClient.Properties.Resources.Referesh;
            this.btnRefresh.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnRefresh.Location = new System.Drawing.Point(319, 6);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(75, 28);
            this.btnRefresh.TabIndex = 7;
            this.btnRefresh.Text = "&Refresh";
            this.btnRefresh.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnRefresh.UseVisualStyleBackColor = false;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkSelectAll);
            this.groupBox1.Controls.Add(this.dgvIssue);
            this.groupBox1.Location = new System.Drawing.Point(22, 116);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(458, 239);
            this.groupBox1.TabIndex = 54;
            this.groupBox1.TabStop = false;
            // 
            // chkSelectAll
            // 
            this.chkSelectAll.AutoSize = true;
            this.chkSelectAll.Location = new System.Drawing.Point(11, 0);
            this.chkSelectAll.Name = "chkSelectAll";
            this.chkSelectAll.Size = new System.Drawing.Size(67, 17);
            this.chkSelectAll.TabIndex = 214;
            this.chkSelectAll.Text = "SelectAll";
            this.chkSelectAll.UseVisualStyleBackColor = true;
            this.chkSelectAll.CheckedChanged += new System.EventHandler(this.chkSelectAll_CheckedChanged);
            // 
            // dgvIssue
            // 
            this.dgvIssue.AllowUserToAddRows = false;
            this.dgvIssue.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black;
            this.dgvIssue.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvIssue.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvIssue.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Select,
            this.ID,
            this.TransferIssueNo,
            this.TransactionDateTime,
            this.BranchName,
            this.TransferBranchName});
            this.dgvIssue.Location = new System.Drawing.Point(6, 19);
            this.dgvIssue.Name = "dgvIssue";
            this.dgvIssue.RowHeadersVisible = false;
            this.dgvIssue.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvIssue.Size = new System.Drawing.Size(446, 214);
            this.dgvIssue.TabIndex = 7;
            // 
            // Select
            // 
            this.Select.HeaderText = "Select";
            this.Select.Name = "Select";
            this.Select.Width = 50;
            // 
            // ID
            // 
            this.ID.HeaderText = "ID";
            this.ID.Name = "ID";
            this.ID.ReadOnly = true;
            this.ID.Visible = false;
            // 
            // TransferIssueNo
            // 
            this.TransferIssueNo.HeaderText = "Issue No";
            this.TransferIssueNo.Name = "TransferIssueNo";
            this.TransferIssueNo.ReadOnly = true;
            // 
            // TransactionDateTime
            // 
            this.TransactionDateTime.DataPropertyName = "TransactionDateTime";
            this.TransactionDateTime.HeaderText = "Transaction Date";
            this.TransactionDateTime.Name = "TransactionDateTime";
            this.TransactionDateTime.ReadOnly = true;
            this.TransactionDateTime.Width = 130;
            // 
            // BranchName
            // 
            this.BranchName.DataPropertyName = "BranchName";
            this.BranchName.HeaderText = "Branch Name";
            this.BranchName.Name = "BranchName";
            this.BranchName.ReadOnly = true;
            // 
            // TransferBranchName
            // 
            this.TransferBranchName.HeaderText = "TransferTo";
            this.TransferBranchName.Name = "TransferBranchName";
            this.TransferBranchName.ReadOnly = true;
            // 
            // FormMultipleVAT6_5
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(481, 471);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.grbPrintInformation);
            this.Controls.Add(this.cmbPrinterName);
            this.Controls.Add(this.grbBankInformation);
            this.Location = new System.Drawing.Point(100, 100);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(1200, 650);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(400, 400);
            this.Name = "FormMultipleVAT6_5";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Report (VAT 6.5) Transfer Challan";
            this.Load += new System.EventHandler(this.FormMultipleVAT11_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numericUDPrintCopy)).EndInit();
            this.grbBankInformation.ResumeLayout(false);
            this.grbBankInformation.PerformLayout();
            this.grbPrintInformation.ResumeLayout(false);
            this.grbPrintInformation.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvIssue)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.NumericUpDown numericUDPrintCopy;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ComboBox cmbPrinterName;
        private System.Windows.Forms.GroupBox grbBankInformation;
        private System.Windows.Forms.Label label3;
        public System.Windows.Forms.DateTimePicker dtpToDate;
        public System.Windows.Forms.DateTimePicker dtpFromDate;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox grbPrintInformation;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.CheckBox chk3rd;
        private System.Windows.Forms.CheckBox chk2nd;
        private System.Windows.Forms.CheckBox chk1st;
        private System.Windows.Forms.Label lblPrintPage;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.DataGridView dgvIssue;
        private System.Windows.Forms.CheckBox chkSelectAll;
        private System.Windows.Forms.Button btnSelectedPrint;
        private System.Windows.Forms.TextBox txtSalesInvoiceNo;
        public System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label6;
        public System.Windows.Forms.ComboBox cmbTransferTo;
        private System.Windows.Forms.Button btnPreview;
        public System.Windows.Forms.ComboBox cmbBranch;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Select;
        private System.Windows.Forms.DataGridViewTextBoxColumn ID;
        private System.Windows.Forms.DataGridViewTextBoxColumn TransferIssueNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn TransactionDateTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn BranchName;
        private System.Windows.Forms.DataGridViewTextBoxColumn TransferBranchName;
        private System.Windows.Forms.ComboBox comtt;
        private System.Windows.Forms.Label label7;
    }
}