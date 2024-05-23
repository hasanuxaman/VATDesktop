namespace VATClient
{
    partial class FormDepositSearch
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            this.grbDeposit = new System.Windows.Forms.GroupBox();
            this.btnPreview = new System.Windows.Forms.Button();
            this.label31 = new System.Windows.Forms.Label();
            this.cmbAdjType = new System.Windows.Forms.ComboBox();
            this.cmbRecordCount = new System.Windows.Forms.ComboBox();
            this.chkSelectAll = new System.Windows.Forms.CheckBox();
            this.cmbBranch = new System.Windows.Forms.ComboBox();
            this.label20 = new System.Windows.Forms.Label();
            this.cmbPost = new System.Windows.Forms.ComboBox();
            this.label19 = new System.Windows.Forms.Label();
            this.btnAdd = new System.Windows.Forms.Button();
            this.txtAccountNumber = new System.Windows.Forms.TextBox();
            this.txtBankName = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.txtDepositAmountTo = new System.Windows.Forms.TextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.dtpChequeToDate = new System.Windows.Forms.DateTimePicker();
            this.dtpChequeFromDate = new System.Windows.Forms.DateTimePicker();
            this.dtpDepositToDate = new System.Windows.Forms.DateTimePicker();
            this.dtpDepositFromDate = new System.Windows.Forms.DateTimePicker();
            this.txtBankID = new System.Windows.Forms.TextBox();
            this.txtChequeNo = new System.Windows.Forms.TextBox();
            this.txtDepositAmountFrom = new System.Windows.Forms.TextBox();
            this.txtDepositType = new System.Windows.Forms.TextBox();
            this.txtTreasuryNo = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtDepositID = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbExport = new System.Windows.Forms.ComboBox();
            this.btnExport = new System.Windows.Forms.Button();
            this.LRecordCount = new System.Windows.Forms.Label();
            this.txtBranchName = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.txtChequeBankBranch = new System.Windows.Forms.TextBox();
            this.txtDepositPersonDesignation = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtDepositPerson = new System.Windows.Forms.TextBox();
            this.txtChequeBank = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dgvDeposit = new System.Windows.Forms.DataGridView();
            this.Select = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.backgroundWorkerSearch = new System.ComponentModel.BackgroundWorker();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.backgroundWorkerPreview = new System.ComponentModel.BackgroundWorker();
            this.pnlHidden = new System.Windows.Forms.Panel();
            this.label21 = new System.Windows.Forms.Label();
            this.grbDeposit.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDeposit)).BeginInit();
            this.panel1.SuspendLayout();
            this.pnlHidden.SuspendLayout();
            this.SuspendLayout();
            // 
            // grbDeposit
            // 
            this.grbDeposit.Controls.Add(this.label21);
            this.grbDeposit.Controls.Add(this.btnPreview);
            this.grbDeposit.Controls.Add(this.btnCancel);
            this.grbDeposit.Controls.Add(this.label31);
            this.grbDeposit.Controls.Add(this.cmbAdjType);
            this.grbDeposit.Controls.Add(this.cmbRecordCount);
            this.grbDeposit.Controls.Add(this.cmbBranch);
            this.grbDeposit.Controls.Add(this.label20);
            this.grbDeposit.Controls.Add(this.cmbPost);
            this.grbDeposit.Controls.Add(this.label19);
            this.grbDeposit.Controls.Add(this.btnAdd);
            this.grbDeposit.Controls.Add(this.txtAccountNumber);
            this.grbDeposit.Controls.Add(this.txtBankName);
            this.grbDeposit.Controls.Add(this.label18);
            this.grbDeposit.Controls.Add(this.label16);
            this.grbDeposit.Controls.Add(this.btnSearch);
            this.grbDeposit.Controls.Add(this.label12);
            this.grbDeposit.Controls.Add(this.label11);
            this.grbDeposit.Controls.Add(this.dtpChequeToDate);
            this.grbDeposit.Controls.Add(this.dtpChequeFromDate);
            this.grbDeposit.Controls.Add(this.dtpDepositToDate);
            this.grbDeposit.Controls.Add(this.dtpDepositFromDate);
            this.grbDeposit.Controls.Add(this.txtChequeNo);
            this.grbDeposit.Controls.Add(this.txtDepositType);
            this.grbDeposit.Controls.Add(this.txtTreasuryNo);
            this.grbDeposit.Controls.Add(this.label7);
            this.grbDeposit.Controls.Add(this.label9);
            this.grbDeposit.Controls.Add(this.label4);
            this.grbDeposit.Controls.Add(this.label3);
            this.grbDeposit.Controls.Add(this.label2);
            this.grbDeposit.Controls.Add(this.txtDepositID);
            this.grbDeposit.Controls.Add(this.label1);
            this.grbDeposit.Location = new System.Drawing.Point(15, 6);
            this.grbDeposit.Name = "grbDeposit";
            this.grbDeposit.Size = new System.Drawing.Size(757, 184);
            this.grbDeposit.TabIndex = 9;
            this.grbDeposit.TabStop = false;
            this.grbDeposit.Text = "Searching Criteria";
            // 
            // btnPreview
            // 
            this.btnPreview.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnPreview.Image = global::VATClient.Properties.Resources.Print;
            this.btnPreview.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPreview.Location = new System.Drawing.Point(597, 152);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new System.Drawing.Size(75, 28);
            this.btnPreview.TabIndex = 240;
            this.btnPreview.Text = "Preview";
            this.btnPreview.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnPreview.UseVisualStyleBackColor = false;
            this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.Location = new System.Drawing.Point(360, 109);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(50, 13);
            this.label31.TabIndex = 239;
            this.label31.Text = "Adj Type";
            this.label31.Visible = false;
            // 
            // cmbAdjType
            // 
            this.cmbAdjType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAdjType.FormattingEnabled = true;
            this.cmbAdjType.Items.AddRange(new object[] {
            "All",
            "DevelopmentSurcharge",
            "EnvironmentProtectionSurcharge",
            "ExciseDuty",
            "FineOrPenalty",
            "FinePenaltyForNonSubmissionOfReturn",
            "HelthCareSurcharge",
            "ICTDevelopmentSurcharge",
            "InterestOnOveredSD",
            "InterestOnOveredVAT",
            "WithoutBankPay"});
            this.cmbAdjType.Location = new System.Drawing.Point(433, 105);
            this.cmbAdjType.Name = "cmbAdjType";
            this.cmbAdjType.Size = new System.Drawing.Size(200, 21);
            this.cmbAdjType.Sorted = true;
            this.cmbAdjType.TabIndex = 238;
            this.cmbAdjType.Visible = false;
            // 
            // cmbRecordCount
            // 
            this.cmbRecordCount.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRecordCount.FormattingEnabled = true;
            this.cmbRecordCount.Location = new System.Drawing.Point(116, 128);
            this.cmbRecordCount.Name = "cmbRecordCount";
            this.cmbRecordCount.Size = new System.Drawing.Size(80, 21);
            this.cmbRecordCount.TabIndex = 237;
            // 
            // chkSelectAll
            // 
            this.chkSelectAll.AutoSize = true;
            this.chkSelectAll.Location = new System.Drawing.Point(6, 1);
            this.chkSelectAll.Name = "chkSelectAll";
            this.chkSelectAll.Size = new System.Drawing.Size(66, 17);
            this.chkSelectAll.TabIndex = 235;
            this.chkSelectAll.Text = "SelectAll";
            this.chkSelectAll.UseVisualStyleBackColor = true;
            this.chkSelectAll.CheckedChanged += new System.EventHandler(this.chkSelectAll_CheckedChanged);
            // 
            // cmbBranch
            // 
            this.cmbBranch.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBranch.FormattingEnabled = true;
            this.cmbBranch.Location = new System.Drawing.Point(116, 105);
            this.cmbBranch.Name = "cmbBranch";
            this.cmbBranch.Size = new System.Drawing.Size(200, 21);
            this.cmbBranch.TabIndex = 226;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(21, 109);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(40, 13);
            this.label20.TabIndex = 225;
            this.label20.Text = "Branch";
            this.label20.Click += new System.EventHandler(this.label20_Click);
            // 
            // cmbPost
            // 
            this.cmbPost.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPost.FormattingEnabled = true;
            this.cmbPost.Items.AddRange(new object[] {
            "Y",
            "N"});
            this.cmbPost.Location = new System.Drawing.Point(433, 128);
            this.cmbPost.Name = "cmbPost";
            this.cmbPost.Size = new System.Drawing.Size(80, 21);
            this.cmbPost.TabIndex = 10;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(360, 132);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(28, 13);
            this.label19.TabIndex = 200;
            this.label19.Text = "Post";
            // 
            // btnAdd
            // 
            this.btnAdd.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnAdd.Image = global::VATClient.Properties.Resources.add_over;
            this.btnAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAdd.Location = new System.Drawing.Point(676, 152);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 28);
            this.btnAdd.TabIndex = 12;
            this.btnAdd.Text = "&Add";
            this.btnAdd.UseVisualStyleBackColor = false;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // txtAccountNumber
            // 
            this.txtAccountNumber.Location = new System.Drawing.Point(433, 82);
            this.txtAccountNumber.Name = "txtAccountNumber";
            this.txtAccountNumber.Size = new System.Drawing.Size(200, 21);
            this.txtAccountNumber.TabIndex = 9;
            this.txtAccountNumber.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtAccountNumber_KeyDown);
            // 
            // txtBankName
            // 
            this.txtBankName.Location = new System.Drawing.Point(433, 59);
            this.txtBankName.Name = "txtBankName";
            this.txtBankName.Size = new System.Drawing.Size(200, 21);
            this.txtBankName.TabIndex = 8;
            this.txtBankName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtBankName_KeyDown);
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(360, 86);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(62, 13);
            this.label18.TabIndex = 114;
            this.label18.Text = "Account No";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(360, 63);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(60, 13);
            this.label16.TabIndex = 112;
            this.label16.Text = "Bank Name";
            // 
            // txtDepositAmountTo
            // 
            this.txtDepositAmountTo.Location = new System.Drawing.Point(112, 38);
            this.txtDepositAmountTo.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtDepositAmountTo.MinimumSize = new System.Drawing.Size(75, 20);
            this.txtDepositAmountTo.Name = "txtDepositAmountTo";
            this.txtDepositAmountTo.Size = new System.Drawing.Size(80, 20);
            this.txtDepositAmountTo.TabIndex = 6;
            this.txtDepositAmountTo.Text = "0.00";
            this.txtDepositAmountTo.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtDepositAmountTo.Visible = false;
            this.txtDepositAmountTo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtDepositAmountTo_KeyDown);
            this.txtDepositAmountTo.Leave += new System.EventHandler(this.txtDepositAmountTo_Leave);
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSearch.Image = global::VATClient.Properties.Resources.search;
            this.btnSearch.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSearch.Location = new System.Drawing.Point(676, 10);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 28);
            this.btnSearch.TabIndex = 11;
            this.btnSearch.Text = "&Search";
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(91, 42);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(17, 13);
            this.label13.TabIndex = 110;
            this.label13.Text = "to";
            this.label13.Visible = false;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(536, 41);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(17, 13);
            this.label12.TabIndex = 109;
            this.label12.Text = "to";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(221, 63);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(17, 13);
            this.label11.TabIndex = 109;
            this.label11.Text = "to";
            this.label11.Click += new System.EventHandler(this.label11_Click);
            // 
            // dtpChequeToDate
            // 
            this.dtpChequeToDate.Checked = false;
            this.dtpChequeToDate.CustomFormat = "dd/MMM/yyyy";
            this.dtpChequeToDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpChequeToDate.Location = new System.Drawing.Point(556, 37);
            this.dtpChequeToDate.Name = "dtpChequeToDate";
            this.dtpChequeToDate.ShowCheckBox = true;
            this.dtpChequeToDate.Size = new System.Drawing.Size(108, 21);
            this.dtpChequeToDate.TabIndex = 7;
            this.dtpChequeToDate.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dtpChequeToDate_KeyDown);
            // 
            // dtpChequeFromDate
            // 
            this.dtpChequeFromDate.Checked = false;
            this.dtpChequeFromDate.CustomFormat = "dd/MMM/yyyy";
            this.dtpChequeFromDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpChequeFromDate.Location = new System.Drawing.Point(433, 37);
            this.dtpChequeFromDate.Name = "dtpChequeFromDate";
            this.dtpChequeFromDate.ShowCheckBox = true;
            this.dtpChequeFromDate.Size = new System.Drawing.Size(101, 21);
            this.dtpChequeFromDate.TabIndex = 6;
            this.dtpChequeFromDate.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dtpChequeFromDate_KeyDown);
            // 
            // dtpDepositToDate
            // 
            this.dtpDepositToDate.Checked = false;
            this.dtpDepositToDate.CustomFormat = "dd/MMM/yyyy";
            this.dtpDepositToDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpDepositToDate.Location = new System.Drawing.Point(244, 59);
            this.dtpDepositToDate.Name = "dtpDepositToDate";
            this.dtpDepositToDate.ShowCheckBox = true;
            this.dtpDepositToDate.Size = new System.Drawing.Size(108, 21);
            this.dtpDepositToDate.TabIndex = 3;
            this.dtpDepositToDate.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dtpDepositToDate_KeyDown);
            // 
            // dtpDepositFromDate
            // 
            this.dtpDepositFromDate.Checked = false;
            this.dtpDepositFromDate.CustomFormat = "dd/MMM/yyyy";
            this.dtpDepositFromDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpDepositFromDate.Location = new System.Drawing.Point(116, 59);
            this.dtpDepositFromDate.Name = "dtpDepositFromDate";
            this.dtpDepositFromDate.ShowCheckBox = true;
            this.dtpDepositFromDate.Size = new System.Drawing.Size(101, 21);
            this.dtpDepositFromDate.TabIndex = 2;
            this.dtpDepositFromDate.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dtpDepositFromDate_KeyDown);
            // 
            // txtBankID
            // 
            this.txtBankID.Location = new System.Drawing.Point(7, 12);
            this.txtBankID.Name = "txtBankID";
            this.txtBankID.Size = new System.Drawing.Size(20, 21);
            this.txtBankID.TabIndex = 10;
            this.txtBankID.Visible = false;
            this.txtBankID.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtBankID_KeyDown);
            // 
            // txtChequeNo
            // 
            this.txtChequeNo.Location = new System.Drawing.Point(433, 15);
            this.txtChequeNo.Name = "txtChequeNo";
            this.txtChequeNo.Size = new System.Drawing.Size(200, 21);
            this.txtChequeNo.TabIndex = 5;
            this.txtChequeNo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtChequeNo_KeyDown);
            // 
            // txtDepositAmountFrom
            // 
            this.txtDepositAmountFrom.Location = new System.Drawing.Point(7, 38);
            this.txtDepositAmountFrom.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtDepositAmountFrom.MinimumSize = new System.Drawing.Size(75, 20);
            this.txtDepositAmountFrom.Name = "txtDepositAmountFrom";
            this.txtDepositAmountFrom.Size = new System.Drawing.Size(80, 20);
            this.txtDepositAmountFrom.TabIndex = 5;
            this.txtDepositAmountFrom.Text = "0.00";
            this.txtDepositAmountFrom.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtDepositAmountFrom.Visible = false;
            this.txtDepositAmountFrom.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtDepositAmountFrom_KeyDown);
            this.txtDepositAmountFrom.Leave += new System.EventHandler(this.txtDepositAmountFrom_Leave);
            // 
            // txtDepositType
            // 
            this.txtDepositType.Location = new System.Drawing.Point(116, 82);
            this.txtDepositType.Name = "txtDepositType";
            this.txtDepositType.Size = new System.Drawing.Size(200, 21);
            this.txtDepositType.TabIndex = 4;
            this.txtDepositType.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtDepositType_KeyDown);
            // 
            // txtTreasuryNo
            // 
            this.txtTreasuryNo.Location = new System.Drawing.Point(116, 37);
            this.txtTreasuryNo.Name = "txtTreasuryNo";
            this.txtTreasuryNo.Size = new System.Drawing.Size(200, 21);
            this.txtTreasuryNo.TabIndex = 1;
            this.txtTreasuryNo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtTreasuryNo_KeyDown);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(109, 64);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(44, 13);
            this.label6.TabIndex = 10;
            this.label6.Text = "Bank ID";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(360, 41);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(70, 13);
            this.label7.TabIndex = 9;
            this.label7.Text = "Cheque Date";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(360, 19);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(60, 13);
            this.label9.TabIndex = 7;
            this.label9.Text = "Cheque No";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(4, 64);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(83, 13);
            this.label10.TabIndex = 6;
            this.label10.Text = "Deposit Amount";
            this.label10.Visible = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(21, 86);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(70, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Deposit Type";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(21, 63);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(74, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Effected Date";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(21, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(66, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Treasury No";
            // 
            // txtDepositID
            // 
            this.txtDepositID.Location = new System.Drawing.Point(116, 15);
            this.txtDepositID.Name = "txtDepositID";
            this.txtDepositID.Size = new System.Drawing.Size(200, 21);
            this.txtDepositID.TabIndex = 0;
            this.txtDepositID.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtDepositID_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(18, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "ID";
            // 
            // cmbExport
            // 
            this.cmbExport.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbExport.FormattingEnabled = true;
            this.cmbExport.Items.AddRange(new object[] {
            "EXCEL",
            "TEXT"});
            this.cmbExport.Location = new System.Drawing.Point(325, 11);
            this.cmbExport.Name = "cmbExport";
            this.cmbExport.Size = new System.Drawing.Size(80, 21);
            this.cmbExport.TabIndex = 234;
            // 
            // btnExport
            // 
            this.btnExport.BackColor = System.Drawing.Color.White;
            this.btnExport.Image = global::VATClient.Properties.Resources.Load;
            this.btnExport.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnExport.Location = new System.Drawing.Point(408, 7);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(70, 28);
            this.btnExport.TabIndex = 233;
            this.btnExport.Text = "Export";
            this.btnExport.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnExport.UseVisualStyleBackColor = false;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // LRecordCount
            // 
            this.LRecordCount.AutoSize = true;
            this.LRecordCount.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LRecordCount.Location = new System.Drawing.Point(5, 13);
            this.LRecordCount.Name = "LRecordCount";
            this.LRecordCount.Size = new System.Drawing.Size(94, 16);
            this.LRecordCount.TabIndex = 224;
            this.LRecordCount.Text = "Record Count :";
            // 
            // txtBranchName
            // 
            this.txtBranchName.Location = new System.Drawing.Point(929, 95);
            this.txtBranchName.MaximumSize = new System.Drawing.Size(50, 20);
            this.txtBranchName.MinimumSize = new System.Drawing.Size(50, 20);
            this.txtBranchName.Name = "txtBranchName";
            this.txtBranchName.Size = new System.Drawing.Size(50, 21);
            this.txtBranchName.TabIndex = 116;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(812, 102);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(74, 13);
            this.label17.TabIndex = 113;
            this.label17.Text = "Branch Name:";
            // 
            // txtChequeBankBranch
            // 
            this.txtChequeBankBranch.Location = new System.Drawing.Point(929, 141);
            this.txtChequeBankBranch.MaximumSize = new System.Drawing.Size(50, 20);
            this.txtChequeBankBranch.MinimumSize = new System.Drawing.Size(50, 20);
            this.txtChequeBankBranch.Name = "txtChequeBankBranch";
            this.txtChequeBankBranch.Size = new System.Drawing.Size(50, 21);
            this.txtChequeBankBranch.TabIndex = 33;
            // 
            // txtDepositPersonDesignation
            // 
            this.txtDepositPersonDesignation.Location = new System.Drawing.Point(929, 187);
            this.txtDepositPersonDesignation.MaximumSize = new System.Drawing.Size(50, 20);
            this.txtDepositPersonDesignation.MinimumSize = new System.Drawing.Size(50, 20);
            this.txtDepositPersonDesignation.Name = "txtDepositPersonDesignation";
            this.txtDepositPersonDesignation.Size = new System.Drawing.Size(50, 21);
            this.txtDepositPersonDesignation.TabIndex = 27;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(811, 145);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(110, 13);
            this.label5.TabIndex = 5;
            this.label5.Text = "Cheque Bank Branch:";
            // 
            // txtDepositPerson
            // 
            this.txtDepositPerson.Location = new System.Drawing.Point(929, 164);
            this.txtDepositPerson.MaximumSize = new System.Drawing.Size(50, 20);
            this.txtDepositPerson.MinimumSize = new System.Drawing.Size(50, 20);
            this.txtDepositPerson.Name = "txtDepositPerson";
            this.txtDepositPerson.Size = new System.Drawing.Size(50, 21);
            this.txtDepositPerson.TabIndex = 26;
            // 
            // txtChequeBank
            // 
            this.txtChequeBank.Location = new System.Drawing.Point(929, 118);
            this.txtChequeBank.MaximumSize = new System.Drawing.Size(50, 20);
            this.txtChequeBank.MinimumSize = new System.Drawing.Size(50, 20);
            this.txtChequeBank.Name = "txtChequeBank";
            this.txtChequeBank.Size = new System.Drawing.Size(50, 21);
            this.txtChequeBank.TabIndex = 23;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(816, 187);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(67, 13);
            this.label14.TabIndex = 12;
            this.label14.Text = "Designation:";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(816, 164);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(44, 13);
            this.label15.TabIndex = 11;
            this.label15.Text = "Person:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(811, 122);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(74, 13);
            this.label8.TabIndex = 8;
            this.label8.Text = "Cheque Bank:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dgvDeposit);
            this.groupBox1.Controls.Add(this.chkSelectAll);
            this.groupBox1.Controls.Add(this.pnlHidden);
            this.groupBox1.Location = new System.Drawing.Point(12, 191);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(760, 225);
            this.groupBox1.TabIndex = 106;
            this.groupBox1.TabStop = false;
            // 
            // dgvDeposit
            // 
            this.dgvDeposit.AllowUserToAddRows = false;
            this.dgvDeposit.AllowUserToDeleteRows = false;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.dgvDeposit.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle5;
            this.dgvDeposit.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDeposit.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Select});
            this.dgvDeposit.Location = new System.Drawing.Point(6, 20);
            this.dgvDeposit.Name = "dgvDeposit";
            this.dgvDeposit.RowHeadersVisible = false;
            this.dgvDeposit.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvDeposit.Size = new System.Drawing.Size(745, 199);
            this.dgvDeposit.TabIndex = 12;
            this.dgvDeposit.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvDeposit_CellContentClick);
            this.dgvDeposit.DoubleClick += new System.EventHandler(this.dgvDeposit_DoubleClick);
            // 
            // Select
            // 
            this.Select.HeaderText = "Select";
            this.Select.Name = "Select";
            this.Select.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Select.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::VATClient.Properties.Resources.Green_Footer;
            this.panel1.Controls.Add(this.btnOk);
            this.panel1.Controls.Add(this.LRecordCount);
            this.panel1.Controls.Add(this.btnExport);
            this.panel1.Controls.Add(this.cmbExport);
            this.panel1.Location = new System.Drawing.Point(-1, 421);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(793, 40);
            this.panel1.TabIndex = 13;
            // 
            // btnOk
            // 
            this.btnOk.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnOk.Image = global::VATClient.Properties.Resources.Back;
            this.btnOk.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOk.Location = new System.Drawing.Point(702, 7);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 28);
            this.btnOk.TabIndex = 15;
            this.btnOk.Text = "&Ok";
            this.btnOk.UseVisualStyleBackColor = false;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnCancel.Image = global::VATClient.Properties.Resources.Referesh;
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.Location = new System.Drawing.Point(676, 36);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 28);
            this.btnCancel.TabIndex = 14;
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
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(267, 285);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(300, 30);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 195;
            this.progressBar1.Visible = false;
            // 
            // backgroundWorkerPreview
            // 
            this.backgroundWorkerPreview.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerPreview_DoWork);
            this.backgroundWorkerPreview.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerPreview_RunWorkerCompleted);
            // 
            // pnlHidden
            // 
            this.pnlHidden.Controls.Add(this.txtBankID);
            this.pnlHidden.Controls.Add(this.txtDepositAmountTo);
            this.pnlHidden.Controls.Add(this.label10);
            this.pnlHidden.Controls.Add(this.txtDepositAmountFrom);
            this.pnlHidden.Controls.Add(this.label13);
            this.pnlHidden.Controls.Add(this.label6);
            this.pnlHidden.Location = new System.Drawing.Point(16, 88);
            this.pnlHidden.Name = "pnlHidden";
            this.pnlHidden.Size = new System.Drawing.Size(200, 100);
            this.pnlHidden.TabIndex = 13;
            this.pnlHidden.Visible = false;
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(21, 132);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(25, 13);
            this.label21.TabIndex = 241;
            this.label21.Text = "Top";
            // 
            // FormDepositSearch
            // 
            this.AcceptButton = this.btnSearch;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.CancelButton = this.btnOk;
            this.ClientSize = new System.Drawing.Size(784, 461);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.txtBranchName);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.grbDeposit);
            this.Controls.Add(this.txtDepositPerson);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.txtChequeBankBranch);
            this.Controls.Add(this.txtDepositPersonDesignation);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.txtChequeBank);
            this.Controls.Add(this.label5);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(1100, 500);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(800, 500);
            this.Name = "FormDepositSearch";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Deposit Search";
            this.Load += new System.EventHandler(this.FormDepositSearch_Load);
            this.grbDeposit.ResumeLayout(false);
            this.grbDeposit.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDeposit)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.pnlHidden.ResumeLayout(false);
            this.pnlHidden.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.GroupBox grbDeposit;
        private System.Windows.Forms.TextBox txtChequeBankBranch;
        private System.Windows.Forms.TextBox txtDepositPersonDesignation;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtDepositPerson;
        private System.Windows.Forms.TextBox txtBankID;
        private System.Windows.Forms.TextBox txtChequeBank;
        private System.Windows.Forms.TextBox txtChequeNo;
        private System.Windows.Forms.TextBox txtDepositAmountFrom;
        private System.Windows.Forms.TextBox txtDepositType;
        private System.Windows.Forms.TextBox txtTreasuryNo;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtDepositID;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.DateTimePicker dtpChequeToDate;
        private System.Windows.Forms.DateTimePicker dtpChequeFromDate;
        private System.Windows.Forms.DateTimePicker dtpDepositToDate;
        private System.Windows.Forms.DateTimePicker dtpDepositFromDate;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox txtDepositAmountTo;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox txtAccountNumber;
        private System.Windows.Forms.TextBox txtBranchName;
        private System.Windows.Forms.TextBox txtBankName;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridView dgvDeposit;
        public System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.ComboBox cmbPost;
        private System.Windows.Forms.Label label19;
        private System.ComponentModel.BackgroundWorker backgroundWorkerSearch;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label LRecordCount;
        private System.Windows.Forms.ComboBox cmbBranch;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.ComboBox cmbExport;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.CheckBox chkSelectAll;
        public System.Windows.Forms.ComboBox cmbRecordCount;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Select;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.ComboBox cmbAdjType;
        private System.Windows.Forms.Button btnPreview;
        private System.ComponentModel.BackgroundWorker backgroundWorkerPreview;
        private System.Windows.Forms.Panel pnlHidden;
        private System.Windows.Forms.Label label21;
    }
}