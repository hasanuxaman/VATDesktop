namespace VATClient
{
    partial class FormSDDepositSearch
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.dgvDeposit = new System.Windows.Forms.DataGridView();
            this.label2 = new System.Windows.Forms.Label();
            this.txtDepositID = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.backgroundWorkerSearch = new System.ComponentModel.BackgroundWorker();
            this.label3 = new System.Windows.Forms.Label();
            this.txtDepositAmountTo = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.dtpChequeToDate = new System.Windows.Forms.DateTimePicker();
            this.dtpChequeFromDate = new System.Windows.Forms.DateTimePicker();
            this.dtpDepositToDate = new System.Windows.Forms.DateTimePicker();
            this.grbDeposit = new System.Windows.Forms.GroupBox();
            this.cmbPost = new System.Windows.Forms.ComboBox();
            this.label19 = new System.Windows.Forms.Label();
            this.txtAccountNumber = new System.Windows.Forms.TextBox();
            this.txtBankName = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
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
            this.txtBranchName = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.txtDepositPerson = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.txtChequeBankBranch = new System.Windows.Forms.TextBox();
            this.txtDepositPersonDesignation = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtChequeBank = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnSearch = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.LRecordCount = new System.Windows.Forms.Label();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.DepositID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Post = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TreasuryNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DepositDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DepositType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DepositAmount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ChequeNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ChequeBank = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ChequeBankBranch = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ChequeDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BankID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BankName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BranchName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AccountNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DepositPerson = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Comments = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DepositPersonDesignation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TransactionType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ReverseID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDeposit)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.grbDeposit.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(225, 219);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(250, 22);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 209;
            this.progressBar1.Visible = false;
            // 
            // dgvDeposit
            // 
            this.dgvDeposit.AllowUserToAddRows = false;
            this.dgvDeposit.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.dgvDeposit.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvDeposit.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDeposit.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.DepositID,
            this.Post,
            this.TreasuryNo,
            this.DepositDate,
            this.DepositType,
            this.DepositAmount,
            this.ChequeNo,
            this.ChequeBank,
            this.ChequeBankBranch,
            this.ChequeDate,
            this.BankID,
            this.BankName,
            this.BranchName,
            this.AccountNumber,
            this.DepositPerson,
            this.Comments,
            this.DepositPersonDesignation,
            this.TransactionType,
            this.ReverseID});
            this.dgvDeposit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvDeposit.Location = new System.Drawing.Point(3, 16);
            this.dgvDeposit.Name = "dgvDeposit";
            this.dgvDeposit.RowHeadersVisible = false;
            this.dgvDeposit.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvDeposit.Size = new System.Drawing.Size(751, 211);
            this.dgvDeposit.TabIndex = 12;
            this.dgvDeposit.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvDeposit_CellContentClick);
            this.dgvDeposit.DoubleClick += new System.EventHandler(this.dgvDeposit_DoubleClick);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(21, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Treasury No";
            // 
            // txtDepositID
            // 
            this.txtDepositID.Location = new System.Drawing.Point(116, 15);
            this.txtDepositID.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtDepositID.MinimumSize = new System.Drawing.Size(185, 20);
            this.txtDepositID.Name = "txtDepositID";
            this.txtDepositID.Size = new System.Drawing.Size(185, 20);
            this.txtDepositID.TabIndex = 0;
            this.txtDepositID.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtDepositID_KeyDown_1);
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
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dgvDeposit);
            this.groupBox1.Location = new System.Drawing.Point(4, 139);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(757, 230);
            this.groupBox1.TabIndex = 206;
            this.groupBox1.TabStop = false;
            // 
            // backgroundWorkerSearch
            // 
            this.backgroundWorkerSearch.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerSearch_DoWork);
            this.backgroundWorkerSearch.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerSearch_RunWorkerCompleted);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(21, 70);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Deposit Date";
            // 
            // txtDepositAmountTo
            // 
            this.txtDepositAmountTo.Location = new System.Drawing.Point(221, 145);
            this.txtDepositAmountTo.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtDepositAmountTo.MinimumSize = new System.Drawing.Size(75, 20);
            this.txtDepositAmountTo.Name = "txtDepositAmountTo";
            this.txtDepositAmountTo.Size = new System.Drawing.Size(80, 20);
            this.txtDepositAmountTo.TabIndex = 6;
            this.txtDepositAmountTo.Text = "0.00";
            this.txtDepositAmountTo.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtDepositAmountTo.Visible = false;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(200, 149);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(16, 13);
            this.label13.TabIndex = 110;
            this.label13.Text = "to";
            this.label13.Visible = false;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(559, 44);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(16, 13);
            this.label12.TabIndex = 109;
            this.label12.Text = "to";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(218, 70);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(16, 13);
            this.label11.TabIndex = 109;
            this.label11.Text = "to";
            // 
            // dtpChequeToDate
            // 
            this.dtpChequeToDate.Checked = false;
            this.dtpChequeToDate.CustomFormat = "dd/MMM/yyyy";
            this.dtpChequeToDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpChequeToDate.Location = new System.Drawing.Point(579, 40);
            this.dtpChequeToDate.Name = "dtpChequeToDate";
            this.dtpChequeToDate.ShowCheckBox = true;
            this.dtpChequeToDate.Size = new System.Drawing.Size(108, 20);
            this.dtpChequeToDate.TabIndex = 7;
            this.dtpChequeToDate.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dtpChequeToDate_KeyDown_1);
            // 
            // dtpChequeFromDate
            // 
            this.dtpChequeFromDate.Checked = false;
            this.dtpChequeFromDate.CustomFormat = "dd/MMM/yyyy";
            this.dtpChequeFromDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpChequeFromDate.Location = new System.Drawing.Point(456, 40);
            this.dtpChequeFromDate.Name = "dtpChequeFromDate";
            this.dtpChequeFromDate.ShowCheckBox = true;
            this.dtpChequeFromDate.Size = new System.Drawing.Size(101, 20);
            this.dtpChequeFromDate.TabIndex = 6;
            this.dtpChequeFromDate.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dtpChequeFromDate_KeyDown_1);
            // 
            // dtpDepositToDate
            // 
            this.dtpDepositToDate.Checked = false;
            this.dtpDepositToDate.CustomFormat = "dd/MMM/yyyy";
            this.dtpDepositToDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpDepositToDate.Location = new System.Drawing.Point(244, 66);
            this.dtpDepositToDate.Name = "dtpDepositToDate";
            this.dtpDepositToDate.ShowCheckBox = true;
            this.dtpDepositToDate.Size = new System.Drawing.Size(108, 20);
            this.dtpDepositToDate.TabIndex = 3;
            this.dtpDepositToDate.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dtpDepositToDate_KeyDown_1);
            // 
            // grbDeposit
            // 
            this.grbDeposit.Controls.Add(this.cmbPost);
            this.grbDeposit.Controls.Add(this.label19);
            this.grbDeposit.Controls.Add(this.btnAdd);
            this.grbDeposit.Controls.Add(this.txtAccountNumber);
            this.grbDeposit.Controls.Add(this.txtBankName);
            this.grbDeposit.Controls.Add(this.label18);
            this.grbDeposit.Controls.Add(this.label16);
            this.grbDeposit.Controls.Add(this.txtDepositAmountTo);
            this.grbDeposit.Controls.Add(this.btnSearch);
            this.grbDeposit.Controls.Add(this.label13);
            this.grbDeposit.Controls.Add(this.label12);
            this.grbDeposit.Controls.Add(this.label11);
            this.grbDeposit.Controls.Add(this.dtpChequeToDate);
            this.grbDeposit.Controls.Add(this.dtpChequeFromDate);
            this.grbDeposit.Controls.Add(this.dtpDepositToDate);
            this.grbDeposit.Controls.Add(this.dtpDepositFromDate);
            this.grbDeposit.Controls.Add(this.txtBankID);
            this.grbDeposit.Controls.Add(this.txtChequeNo);
            this.grbDeposit.Controls.Add(this.txtDepositAmountFrom);
            this.grbDeposit.Controls.Add(this.txtDepositType);
            this.grbDeposit.Controls.Add(this.txtTreasuryNo);
            this.grbDeposit.Controls.Add(this.label6);
            this.grbDeposit.Controls.Add(this.label7);
            this.grbDeposit.Controls.Add(this.label9);
            this.grbDeposit.Controls.Add(this.label10);
            this.grbDeposit.Controls.Add(this.label4);
            this.grbDeposit.Controls.Add(this.label3);
            this.grbDeposit.Controls.Add(this.label2);
            this.grbDeposit.Controls.Add(this.txtDepositID);
            this.grbDeposit.Controls.Add(this.label1);
            this.grbDeposit.Location = new System.Drawing.Point(2, -2);
            this.grbDeposit.Name = "grbDeposit";
            this.grbDeposit.Size = new System.Drawing.Size(757, 141);
            this.grbDeposit.TabIndex = 198;
            this.grbDeposit.TabStop = false;
            this.grbDeposit.Text = "Searching Criteria";
            // 
            // cmbPost
            // 
            this.cmbPost.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPost.FormattingEnabled = true;
            this.cmbPost.Items.AddRange(new object[] {
            "Y",
            "N"});
            this.cmbPost.Location = new System.Drawing.Point(456, 116);
            this.cmbPost.Name = "cmbPost";
            this.cmbPost.Size = new System.Drawing.Size(125, 21);
            this.cmbPost.TabIndex = 10;
            this.cmbPost.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmbPost_KeyDown);
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(422, 122);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(28, 13);
            this.label19.TabIndex = 200;
            this.label19.Text = "Post";
            // 
            // txtAccountNumber
            // 
            this.txtAccountNumber.Location = new System.Drawing.Point(456, 91);
            this.txtAccountNumber.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtAccountNumber.MinimumSize = new System.Drawing.Size(185, 20);
            this.txtAccountNumber.Name = "txtAccountNumber";
            this.txtAccountNumber.Size = new System.Drawing.Size(185, 20);
            this.txtAccountNumber.TabIndex = 9;
            this.txtAccountNumber.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtAccountNumber_KeyDown_1);
            // 
            // txtBankName
            // 
            this.txtBankName.Location = new System.Drawing.Point(456, 66);
            this.txtBankName.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtBankName.MinimumSize = new System.Drawing.Size(185, 20);
            this.txtBankName.Name = "txtBankName";
            this.txtBankName.Size = new System.Drawing.Size(185, 20);
            this.txtBankName.TabIndex = 8;
            this.txtBankName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtBankName_KeyDown_1);
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(371, 95);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(64, 13);
            this.label18.TabIndex = 114;
            this.label18.Text = "Account No";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(371, 70);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(63, 13);
            this.label16.TabIndex = 112;
            this.label16.Text = "Bank Name";
            // 
            // dtpDepositFromDate
            // 
            this.dtpDepositFromDate.Checked = false;
            this.dtpDepositFromDate.CustomFormat = "dd/MMM/yyyy";
            this.dtpDepositFromDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpDepositFromDate.Location = new System.Drawing.Point(116, 66);
            this.dtpDepositFromDate.Name = "dtpDepositFromDate";
            this.dtpDepositFromDate.ShowCheckBox = true;
            this.dtpDepositFromDate.Size = new System.Drawing.Size(101, 20);
            this.dtpDepositFromDate.TabIndex = 2;
            this.dtpDepositFromDate.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dtpDepositFromDate_KeyDown_1);
            // 
            // txtBankID
            // 
            this.txtBankID.Location = new System.Drawing.Point(502, 147);
            this.txtBankID.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtBankID.MinimumSize = new System.Drawing.Size(185, 20);
            this.txtBankID.Name = "txtBankID";
            this.txtBankID.Size = new System.Drawing.Size(185, 20);
            this.txtBankID.TabIndex = 10;
            // 
            // txtChequeNo
            // 
            this.txtChequeNo.Location = new System.Drawing.Point(456, 15);
            this.txtChequeNo.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtChequeNo.MinimumSize = new System.Drawing.Size(185, 20);
            this.txtChequeNo.Name = "txtChequeNo";
            this.txtChequeNo.Size = new System.Drawing.Size(185, 20);
            this.txtChequeNo.TabIndex = 5;
            this.txtChequeNo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtChequeNo_KeyDown_1);
            // 
            // txtDepositAmountFrom
            // 
            this.txtDepositAmountFrom.Location = new System.Drawing.Point(116, 145);
            this.txtDepositAmountFrom.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtDepositAmountFrom.MinimumSize = new System.Drawing.Size(75, 20);
            this.txtDepositAmountFrom.Name = "txtDepositAmountFrom";
            this.txtDepositAmountFrom.Size = new System.Drawing.Size(80, 20);
            this.txtDepositAmountFrom.TabIndex = 5;
            this.txtDepositAmountFrom.Text = "0.00";
            this.txtDepositAmountFrom.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtDepositAmountFrom.Visible = false;
            // 
            // txtDepositType
            // 
            this.txtDepositType.Location = new System.Drawing.Point(116, 91);
            this.txtDepositType.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtDepositType.MinimumSize = new System.Drawing.Size(185, 20);
            this.txtDepositType.Name = "txtDepositType";
            this.txtDepositType.Size = new System.Drawing.Size(185, 20);
            this.txtDepositType.TabIndex = 4;
            this.txtDepositType.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtDepositType_KeyDown_1);
            // 
            // txtTreasuryNo
            // 
            this.txtTreasuryNo.Location = new System.Drawing.Point(116, 40);
            this.txtTreasuryNo.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtTreasuryNo.MinimumSize = new System.Drawing.Size(185, 20);
            this.txtTreasuryNo.Name = "txtTreasuryNo";
            this.txtTreasuryNo.Size = new System.Drawing.Size(185, 20);
            this.txtTreasuryNo.TabIndex = 1;
            this.txtTreasuryNo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtTreasuryNo_KeyDown_1);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(417, 151);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(46, 13);
            this.label6.TabIndex = 10;
            this.label6.Text = "Bank ID";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(371, 44);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(70, 13);
            this.label7.TabIndex = 9;
            this.label7.Text = "Cheque Date";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(371, 19);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(61, 13);
            this.label9.TabIndex = 7;
            this.label9.Text = "Cheque No";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(21, 147);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(82, 13);
            this.label10.TabIndex = 6;
            this.label10.Text = "Deposit Amount";
            this.label10.Visible = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(21, 95);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(70, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Deposit Type";
            // 
            // txtBranchName
            // 
            this.txtBranchName.Location = new System.Drawing.Point(921, 119);
            this.txtBranchName.MaximumSize = new System.Drawing.Size(50, 20);
            this.txtBranchName.MinimumSize = new System.Drawing.Size(50, 20);
            this.txtBranchName.Name = "txtBranchName";
            this.txtBranchName.Size = new System.Drawing.Size(50, 20);
            this.txtBranchName.TabIndex = 219;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(804, 126);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(75, 13);
            this.label17.TabIndex = 218;
            this.label17.Text = "Branch Name:";
            // 
            // txtDepositPerson
            // 
            this.txtDepositPerson.Location = new System.Drawing.Point(921, 188);
            this.txtDepositPerson.MaximumSize = new System.Drawing.Size(50, 20);
            this.txtDepositPerson.MinimumSize = new System.Drawing.Size(50, 20);
            this.txtDepositPerson.Name = "txtDepositPerson";
            this.txtDepositPerson.Size = new System.Drawing.Size(50, 20);
            this.txtDepositPerson.TabIndex = 215;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(808, 188);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(43, 13);
            this.label15.TabIndex = 212;
            this.label15.Text = "Person:";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(808, 211);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(66, 13);
            this.label14.TabIndex = 213;
            this.label14.Text = "Designation:";
            // 
            // txtChequeBankBranch
            // 
            this.txtChequeBankBranch.Location = new System.Drawing.Point(921, 165);
            this.txtChequeBankBranch.MaximumSize = new System.Drawing.Size(50, 20);
            this.txtChequeBankBranch.MinimumSize = new System.Drawing.Size(50, 20);
            this.txtChequeBankBranch.Name = "txtChequeBankBranch";
            this.txtChequeBankBranch.Size = new System.Drawing.Size(50, 20);
            this.txtChequeBankBranch.TabIndex = 217;
            // 
            // txtDepositPersonDesignation
            // 
            this.txtDepositPersonDesignation.Location = new System.Drawing.Point(921, 211);
            this.txtDepositPersonDesignation.MaximumSize = new System.Drawing.Size(50, 20);
            this.txtDepositPersonDesignation.MinimumSize = new System.Drawing.Size(50, 20);
            this.txtDepositPersonDesignation.Name = "txtDepositPersonDesignation";
            this.txtDepositPersonDesignation.Size = new System.Drawing.Size(50, 20);
            this.txtDepositPersonDesignation.TabIndex = 216;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(803, 146);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(75, 13);
            this.label8.TabIndex = 211;
            this.label8.Text = "Cheque Bank:";
            // 
            // txtChequeBank
            // 
            this.txtChequeBank.Location = new System.Drawing.Point(921, 142);
            this.txtChequeBank.MaximumSize = new System.Drawing.Size(50, 20);
            this.txtChequeBank.MinimumSize = new System.Drawing.Size(50, 20);
            this.txtChequeBank.Name = "txtChequeBank";
            this.txtChequeBank.Size = new System.Drawing.Size(50, 20);
            this.txtChequeBank.TabIndex = 214;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(803, 169);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(112, 13);
            this.label5.TabIndex = 210;
            this.label5.Text = "Cheque Bank Branch:";
            // 
            // btnAdd
            // 
            this.btnAdd.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnAdd.Image = global::VATClient.Properties.Resources.add_over;
            this.btnAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAdd.Location = new System.Drawing.Point(660, 102);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 28);
            this.btnAdd.TabIndex = 12;
            this.btnAdd.Text = "&Add";
            this.btnAdd.UseVisualStyleBackColor = false;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSearch.Image = global::VATClient.Properties.Resources.search;
            this.btnSearch.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSearch.Location = new System.Drawing.Point(660, 72);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 28);
            this.btnSearch.TabIndex = 11;
            this.btnSearch.Text = "&Search";
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click_1);
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::VATClient.Properties.Resources.Green_Footer;
            this.panel1.Controls.Add(this.LRecordCount);
            this.panel1.Controls.Add(this.btnOk);
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Location = new System.Drawing.Point(4, 370);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(757, 40);
            this.panel1.TabIndex = 13;
            this.panel1.TabStop = true;
            // 
            // LRecordCount
            // 
            this.LRecordCount.AutoSize = true;
            this.LRecordCount.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LRecordCount.Location = new System.Drawing.Point(138, 13);
            this.LRecordCount.Name = "LRecordCount";
            this.LRecordCount.Size = new System.Drawing.Size(94, 16);
            this.LRecordCount.TabIndex = 220;
            this.LRecordCount.Text = "Record Count :";
            // 
            // btnOk
            // 
            this.btnOk.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnOk.Image = global::VATClient.Properties.Resources.Back;
            this.btnOk.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOk.Location = new System.Drawing.Point(604, 7);
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
            this.btnCancel.Location = new System.Drawing.Point(54, 7);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 28);
            this.btnCancel.TabIndex = 14;
            this.btnCancel.Text = "&Refresh";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // DepositID
            // 
            this.DepositID.HeaderText = "ID";
            this.DepositID.Name = "DepositID";
            this.DepositID.ReadOnly = true;
            this.DepositID.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // Post
            // 
            this.Post.HeaderText = "Post";
            this.Post.Name = "Post";
            this.Post.ReadOnly = true;
            this.Post.Visible = false;
            // 
            // TreasuryNo
            // 
            this.TreasuryNo.HeaderText = "Treasury No";
            this.TreasuryNo.Name = "TreasuryNo";
            this.TreasuryNo.ReadOnly = true;
            // 
            // DepositDate
            // 
            dataGridViewCellStyle2.Format = "dd/MMM/yyyy";
            this.DepositDate.DefaultCellStyle = dataGridViewCellStyle2;
            this.DepositDate.HeaderText = "Date";
            this.DepositDate.Name = "DepositDate";
            this.DepositDate.ReadOnly = true;
            // 
            // DepositType
            // 
            this.DepositType.HeaderText = "Type";
            this.DepositType.Name = "DepositType";
            this.DepositType.ReadOnly = true;
            // 
            // DepositAmount
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.DepositAmount.DefaultCellStyle = dataGridViewCellStyle3;
            this.DepositAmount.HeaderText = "Amount";
            this.DepositAmount.Name = "DepositAmount";
            this.DepositAmount.ReadOnly = true;
            // 
            // ChequeNo
            // 
            this.ChequeNo.HeaderText = "Cheque No";
            this.ChequeNo.Name = "ChequeNo";
            this.ChequeNo.ReadOnly = true;
            // 
            // ChequeBank
            // 
            this.ChequeBank.HeaderText = "Cheque Bank";
            this.ChequeBank.Name = "ChequeBank";
            this.ChequeBank.ReadOnly = true;
            // 
            // ChequeBankBranch
            // 
            this.ChequeBankBranch.HeaderText = "Cheque Bank Branch";
            this.ChequeBankBranch.Name = "ChequeBankBranch";
            this.ChequeBankBranch.ReadOnly = true;
            this.ChequeBankBranch.Visible = false;
            // 
            // ChequeDate
            // 
            this.ChequeDate.HeaderText = "Cheque Date";
            this.ChequeDate.Name = "ChequeDate";
            this.ChequeDate.ReadOnly = true;
            // 
            // BankID
            // 
            this.BankID.HeaderText = "Bank ID";
            this.BankID.Name = "BankID";
            this.BankID.ReadOnly = true;
            this.BankID.Visible = false;
            this.BankID.Width = 5;
            // 
            // BankName
            // 
            this.BankName.HeaderText = "BankName";
            this.BankName.Name = "BankName";
            this.BankName.ReadOnly = true;
            // 
            // BranchName
            // 
            this.BranchName.HeaderText = "BranchName";
            this.BranchName.Name = "BranchName";
            this.BranchName.ReadOnly = true;
            // 
            // AccountNumber
            // 
            this.AccountNumber.HeaderText = "AccountNumber";
            this.AccountNumber.Name = "AccountNumber";
            this.AccountNumber.ReadOnly = true;
            // 
            // DepositPerson
            // 
            this.DepositPerson.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.DepositPerson.HeaderText = "Deposit Person";
            this.DepositPerson.Name = "DepositPerson";
            this.DepositPerson.ReadOnly = true;
            this.DepositPerson.Visible = false;
            this.DepositPerson.Width = 104;
            // 
            // Comments
            // 
            this.Comments.HeaderText = "Comments";
            this.Comments.Name = "Comments";
            this.Comments.ReadOnly = true;
            this.Comments.Visible = false;
            // 
            // DepositPersonDesignation
            // 
            this.DepositPersonDesignation.HeaderText = "Deposit Person Designation";
            this.DepositPersonDesignation.Name = "DepositPersonDesignation";
            this.DepositPersonDesignation.ReadOnly = true;
            this.DepositPersonDesignation.Visible = false;
            // 
            // TransactionType
            // 
            this.TransactionType.HeaderText = "TransactionType";
            this.TransactionType.Name = "TransactionType";
            this.TransactionType.ReadOnly = true;
            // 
            // ReverseID
            // 
            this.ReverseID.HeaderText = "Reverse ID";
            this.ReverseID.Name = "ReverseID";
            // 
            // FormSDDepositSearch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(763, 411);
            this.Controls.Add(this.txtBranchName);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.txtDepositPerson);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.txtChequeBankBranch);
            this.Controls.Add(this.txtDepositPersonDesignation);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.txtChequeBank);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.grbDeposit);
            this.Controls.Add(this.panel1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormSDDepositSearch";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SD Deposit Search";
            this.Load += new System.EventHandler(this.FormSDDepositSearch_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDeposit)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.grbDeposit.ResumeLayout(false);
            this.grbDeposit.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.DataGridView dgvDeposit;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtDepositID;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.ComponentModel.BackgroundWorker backgroundWorkerSearch;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtDepositAmountTo;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.DateTimePicker dtpChequeToDate;
        private System.Windows.Forms.DateTimePicker dtpChequeFromDate;
        private System.Windows.Forms.DateTimePicker dtpDepositToDate;
        private System.Windows.Forms.GroupBox grbDeposit;
        private System.Windows.Forms.ComboBox cmbPost;
        private System.Windows.Forms.Label label19;
        public System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.TextBox txtAccountNumber;
        private System.Windows.Forms.TextBox txtBankName;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.DateTimePicker dtpDepositFromDate;
        private System.Windows.Forms.TextBox txtBankID;
        private System.Windows.Forms.TextBox txtChequeNo;
        private System.Windows.Forms.TextBox txtDepositAmountFrom;
        private System.Windows.Forms.TextBox txtDepositType;
        private System.Windows.Forms.TextBox txtTreasuryNo;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox txtBranchName;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TextBox txtDepositPerson;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox txtChequeBankBranch;
        private System.Windows.Forms.TextBox txtDepositPersonDesignation;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtChequeBank;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label LRecordCount;
        private System.Windows.Forms.DataGridViewTextBoxColumn DepositID;
        private System.Windows.Forms.DataGridViewTextBoxColumn Post;
        private System.Windows.Forms.DataGridViewTextBoxColumn TreasuryNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn DepositDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn DepositType;
        private System.Windows.Forms.DataGridViewTextBoxColumn DepositAmount;
        private System.Windows.Forms.DataGridViewTextBoxColumn ChequeNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn ChequeBank;
        private System.Windows.Forms.DataGridViewTextBoxColumn ChequeBankBranch;
        private System.Windows.Forms.DataGridViewTextBoxColumn ChequeDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn BankID;
        private System.Windows.Forms.DataGridViewTextBoxColumn BankName;
        private System.Windows.Forms.DataGridViewTextBoxColumn BranchName;
        private System.Windows.Forms.DataGridViewTextBoxColumn AccountNumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn DepositPerson;
        private System.Windows.Forms.DataGridViewTextBoxColumn Comments;
        private System.Windows.Forms.DataGridViewTextBoxColumn DepositPersonDesignation;
        private System.Windows.Forms.DataGridViewTextBoxColumn TransactionType;
        private System.Windows.Forms.DataGridViewTextBoxColumn ReverseID;
    }
}