namespace VATClient
{
    partial class FormSDDeposit
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnPrintTR = new System.Windows.Forms.Button();
            this.btnPost = new System.Windows.Forms.Button();
            this.btnPrint = new System.Windows.Forms.Button();
            this.btnVAT18 = new System.Windows.Forms.Button();
            this.btnPrintGrid = new System.Windows.Forms.Button();
            this.btnPrintList = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.btnSearchBankName = new System.Windows.Forms.Button();
            this.cmbBankName = new System.Windows.Forms.ComboBox();
            this.txtAccountNumber = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.txtBranchName = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.txtBankName = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.txtBankID = new System.Windows.Forms.TextBox();
            this.txtTreasuryCopy = new System.Windows.Forms.TextBox();
            this.txtDepositPersonDesignation = new System.Windows.Forms.TextBox();
            this.txtDepositPerson = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtComments = new System.Windows.Forms.TextBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.dtpChequeDate = new System.Windows.Forms.DateTimePicker();
            this.txtChequeBankBranch = new System.Windows.Forms.TextBox();
            this.txtChequeBank = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.txtChequeNo = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lstDepositType = new System.Windows.Forms.ComboBox();
            this.btnAddNew = new System.Windows.Forms.Button();
            this.btnSearchDepositNo = new System.Windows.Forms.Button();
            this.txtTreasuryNo = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.txtDepositAmount = new System.Windows.Forms.TextBox();
            this.txtDepositId = new System.Windows.Forms.TextBox();
            this.dtpDepositDate = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.bgwSave = new System.ComponentModel.BackgroundWorker();
            this.bgwUpdate = new System.ComponentModel.BackgroundWorker();
            this.bgwPost = new System.ComponentModel.BackgroundWorker();
            //this.cachedTestReport1 = new VATClient.CustomReports.CachedTestReport();
            this.bgwTR = new System.ComponentModel.BackgroundWorker();
            this.panel1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::VATClient.Properties.Resources.Green_Footer;
            this.panel1.Controls.Add(this.btnPrintTR);
            this.panel1.Controls.Add(this.btnPost);
            this.panel1.Controls.Add(this.btnPrint);
            this.panel1.Controls.Add(this.btnVAT18);
            this.panel1.Controls.Add(this.btnPrintGrid);
            this.panel1.Controls.Add(this.btnPrintList);
            this.panel1.Controls.Add(this.btnClose);
            this.panel1.Controls.Add(this.btnUpdate);
            this.panel1.Controls.Add(this.btnAdd);
            this.panel1.Location = new System.Drawing.Point(2, 308);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(758, 41);
            this.panel1.TabIndex = 21;
            // 
            // btnPrintTR
            // 
            this.btnPrintTR.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnPrintTR.Image = global::VATClient.Properties.Resources.Print;
            this.btnPrintTR.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPrintTR.Location = new System.Drawing.Point(243, 5);
            this.btnPrintTR.Name = "btnPrintTR";
            this.btnPrintTR.Size = new System.Drawing.Size(84, 28);
            this.btnPrintTR.TabIndex = 57;
            this.btnPrintTR.Text = "TR";
            this.btnPrintTR.UseVisualStyleBackColor = false;
            this.btnPrintTR.Click += new System.EventHandler(this.btnPrintTR_Click);
            // 
            // btnPost
            // 
            this.btnPost.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnPost.Image = global::VATClient.Properties.Resources.Post;
            this.btnPost.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPost.Location = new System.Drawing.Point(165, 6);
            this.btnPost.Name = "btnPost";
            this.btnPost.Size = new System.Drawing.Size(72, 28);
            this.btnPost.TabIndex = 24;
            this.btnPost.Text = "Post";
            this.btnPost.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnPost.UseVisualStyleBackColor = false;
            this.btnPost.Click += new System.EventHandler(this.btnPost_Click);
            // 
            // btnPrint
            // 
            this.btnPrint.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnPrint.Image = global::VATClient.Properties.Resources.Print;
            this.btnPrint.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPrint.Location = new System.Drawing.Point(533, 6);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(84, 28);
            this.btnPrint.TabIndex = 26;
            this.btnPrint.Text = "MIS";
            this.btnPrint.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnPrint.UseVisualStyleBackColor = false;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // btnVAT18
            // 
            this.btnVAT18.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnVAT18.Image = global::VATClient.Properties.Resources.Print;
            this.btnVAT18.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnVAT18.Location = new System.Drawing.Point(366, 6);
            this.btnVAT18.Name = "btnVAT18";
            this.btnVAT18.Size = new System.Drawing.Size(84, 28);
            this.btnVAT18.TabIndex = 25;
            this.btnVAT18.Text = "SD Report";
            this.btnVAT18.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnVAT18.UseVisualStyleBackColor = false;
            this.btnVAT18.Click += new System.EventHandler(this.btnVAT18_Click);
            // 
            // btnPrintGrid
            // 
            this.btnPrintGrid.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnPrintGrid.Image = global::VATClient.Properties.Resources.Print;
            this.btnPrintGrid.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPrintGrid.Location = new System.Drawing.Point(467, 52);
            this.btnPrintGrid.Name = "btnPrintGrid";
            this.btnPrintGrid.Size = new System.Drawing.Size(75, 28);
            this.btnPrintGrid.TabIndex = 22;
            this.btnPrintGrid.Text = "&Grid";
            this.btnPrintGrid.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnPrintGrid.UseVisualStyleBackColor = false;
            // 
            // btnPrintList
            // 
            this.btnPrintList.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnPrintList.Image = global::VATClient.Properties.Resources.Print;
            this.btnPrintList.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPrintList.Location = new System.Drawing.Point(373, 59);
            this.btnPrintList.Name = "btnPrintList";
            this.btnPrintList.Size = new System.Drawing.Size(75, 28);
            this.btnPrintList.TabIndex = 4;
            this.btnPrintList.Text = "&List";
            this.btnPrintList.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnPrintList.UseVisualStyleBackColor = false;
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Image = global::VATClient.Properties.Resources.Back;
            this.btnClose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnClose.Location = new System.Drawing.Point(645, 6);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 28);
            this.btnClose.TabIndex = 29;
            this.btnClose.Text = "&Close";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnUpdate
            // 
            this.btnUpdate.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnUpdate.Image = global::VATClient.Properties.Resources.Update;
            this.btnUpdate.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnUpdate.Location = new System.Drawing.Point(91, 6);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(75, 28);
            this.btnUpdate.TabIndex = 23;
            this.btnUpdate.Text = "&Update";
            this.btnUpdate.UseVisualStyleBackColor = false;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnAdd.Image = global::VATClient.Properties.Resources.Save;
            this.btnAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAdd.Location = new System.Drawing.Point(15, 6);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 28);
            this.btnAdd.TabIndex = 22;
            this.btnAdd.Text = "&Add";
            this.btnAdd.UseVisualStyleBackColor = false;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.progressBar1);
            this.groupBox3.Controls.Add(this.btnSearchBankName);
            this.groupBox3.Controls.Add(this.cmbBankName);
            this.groupBox3.Controls.Add(this.txtAccountNumber);
            this.groupBox3.Controls.Add(this.label17);
            this.groupBox3.Controls.Add(this.txtBranchName);
            this.groupBox3.Controls.Add(this.label16);
            this.groupBox3.Controls.Add(this.txtBankName);
            this.groupBox3.Controls.Add(this.label15);
            this.groupBox3.Controls.Add(this.txtBankID);
            this.groupBox3.Controls.Add(this.txtTreasuryCopy);
            this.groupBox3.Controls.Add(this.txtDepositPersonDesignation);
            this.groupBox3.Controls.Add(this.txtDepositPerson);
            this.groupBox3.Controls.Add(this.label13);
            this.groupBox3.Controls.Add(this.label12);
            this.groupBox3.Controls.Add(this.label11);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.txtComments);
            this.groupBox3.Location = new System.Drawing.Point(3, 142);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(755, 161);
            this.groupBox3.TabIndex = 13;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Deposited to";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(446, 71);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(290, 24);
            this.progressBar1.TabIndex = 206;
            this.progressBar1.Visible = false;
            // 
            // btnSearchBankName
            // 
            this.btnSearchBankName.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSearchBankName.Image = global::VATClient.Properties.Resources.search;
            this.btnSearchBankName.Location = new System.Drawing.Point(364, 18);
            this.btnSearchBankName.Name = "btnSearchBankName";
            this.btnSearchBankName.Size = new System.Drawing.Size(30, 20);
            this.btnSearchBankName.TabIndex = 15;
            this.btnSearchBankName.TabStop = false;
            this.btnSearchBankName.UseVisualStyleBackColor = false;
            this.btnSearchBankName.Click += new System.EventHandler(this.btnSearchBankName_Click);
            // 
            // cmbBankName
            // 
            this.cmbBankName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBankName.FormattingEnabled = true;
            this.cmbBankName.Location = new System.Drawing.Point(109, 18);
            this.cmbBankName.Name = "cmbBankName";
            this.cmbBankName.Size = new System.Drawing.Size(249, 21);
            this.cmbBankName.Sorted = true;
            this.cmbBankName.TabIndex = 14;
            this.cmbBankName.SelectedIndexChanged += new System.EventHandler(this.cmbBankName_SelectedIndexChanged);
            this.cmbBankName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmbBankName_KeyDown);
            // 
            // txtAccountNumber
            // 
            this.txtAccountNumber.Location = new System.Drawing.Point(109, 75);
            this.txtAccountNumber.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtAccountNumber.MinimumSize = new System.Drawing.Size(250, 20);
            this.txtAccountNumber.Name = "txtAccountNumber";
            this.txtAccountNumber.ReadOnly = true;
            this.txtAccountNumber.Size = new System.Drawing.Size(250, 20);
            this.txtAccountNumber.TabIndex = 17;
            this.txtAccountNumber.TabStop = false;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(21, 79);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(64, 13);
            this.label17.TabIndex = 128;
            this.label17.Text = "Account No";
            // 
            // txtBranchName
            // 
            this.txtBranchName.Location = new System.Drawing.Point(109, 47);
            this.txtBranchName.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtBranchName.MinimumSize = new System.Drawing.Size(250, 20);
            this.txtBranchName.Name = "txtBranchName";
            this.txtBranchName.ReadOnly = true;
            this.txtBranchName.Size = new System.Drawing.Size(250, 20);
            this.txtBranchName.TabIndex = 16;
            this.txtBranchName.TabStop = false;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(21, 51);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(72, 13);
            this.label16.TabIndex = 126;
            this.label16.Text = "Branch Name";
            // 
            // txtBankName
            // 
            this.txtBankName.Location = new System.Drawing.Point(109, 19);
            this.txtBankName.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtBankName.MinimumSize = new System.Drawing.Size(250, 20);
            this.txtBankName.Name = "txtBankName";
            this.txtBankName.ReadOnly = true;
            this.txtBankName.Size = new System.Drawing.Size(250, 20);
            this.txtBankName.TabIndex = 1;
            this.txtBankName.Visible = false;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(21, 22);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(63, 13);
            this.label15.TabIndex = 124;
            this.label15.Text = "Bank Name";
            // 
            // txtBankID
            // 
            this.txtBankID.Location = new System.Drawing.Point(364, 207);
            this.txtBankID.MaximumSize = new System.Drawing.Size(20, 20);
            this.txtBankID.MinimumSize = new System.Drawing.Size(20, 20);
            this.txtBankID.Name = "txtBankID";
            this.txtBankID.ReadOnly = true;
            this.txtBankID.Size = new System.Drawing.Size(20, 20);
            this.txtBankID.TabIndex = 0;
            this.txtBankID.TabStop = false;
            this.txtBankID.Visible = false;
            // 
            // txtTreasuryCopy
            // 
            this.txtTreasuryCopy.Location = new System.Drawing.Point(109, 200);
            this.txtTreasuryCopy.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtTreasuryCopy.MinimumSize = new System.Drawing.Size(250, 20);
            this.txtTreasuryCopy.Name = "txtTreasuryCopy";
            this.txtTreasuryCopy.ReadOnly = true;
            this.txtTreasuryCopy.Size = new System.Drawing.Size(250, 20);
            this.txtTreasuryCopy.TabIndex = 5;
            this.txtTreasuryCopy.TabStop = false;
            // 
            // txtDepositPersonDesignation
            // 
            this.txtDepositPersonDesignation.Location = new System.Drawing.Point(486, 47);
            this.txtDepositPersonDesignation.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtDepositPersonDesignation.MinimumSize = new System.Drawing.Size(250, 20);
            this.txtDepositPersonDesignation.Name = "txtDepositPersonDesignation";
            this.txtDepositPersonDesignation.Size = new System.Drawing.Size(250, 20);
            this.txtDepositPersonDesignation.TabIndex = 19;
            this.txtDepositPersonDesignation.TextChanged += new System.EventHandler(this.txtDepositPersonDesignation_TextChanged);
            this.txtDepositPersonDesignation.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtDepositPersonDesignation_KeyDown);
            // 
            // txtDepositPerson
            // 
            this.txtDepositPerson.Location = new System.Drawing.Point(486, 18);
            this.txtDepositPerson.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtDepositPerson.MinimumSize = new System.Drawing.Size(250, 20);
            this.txtDepositPerson.Name = "txtDepositPerson";
            this.txtDepositPerson.Size = new System.Drawing.Size(250, 20);
            this.txtDepositPerson.TabIndex = 18;
            this.txtDepositPerson.TextChanged += new System.EventHandler(this.txtDepositPerson_TextChanged);
            this.txtDepositPerson.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtDepositPerson_KeyDown);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(400, 51);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(63, 13);
            this.label13.TabIndex = 119;
            this.label13.Text = "Designation";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(400, 22);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(79, 13);
            this.label12.TabIndex = 118;
            this.label12.Text = "Deposit Person";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(21, 207);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(75, 13);
            this.label11.TabIndex = 117;
            this.label11.Text = "Treasury Copy";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(21, 107);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(56, 13);
            this.label5.TabIndex = 114;
            this.label5.Text = "Comments";
            // 
            // txtComments
            // 
            this.txtComments.Location = new System.Drawing.Point(109, 102);
            this.txtComments.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtComments.MinimumSize = new System.Drawing.Size(625, 50);
            this.txtComments.Multiline = true;
            this.txtComments.Name = "txtComments";
            this.txtComments.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtComments.Size = new System.Drawing.Size(625, 50);
            this.txtComments.TabIndex = 20;
            this.txtComments.TabStop = false;
            this.txtComments.TextChanged += new System.EventHandler(this.txtComments_TextChanged);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.dtpChequeDate);
            this.groupBox4.Controls.Add(this.txtChequeBankBranch);
            this.groupBox4.Controls.Add(this.txtChequeBank);
            this.groupBox4.Controls.Add(this.label9);
            this.groupBox4.Controls.Add(this.label8);
            this.groupBox4.Controls.Add(this.label7);
            this.groupBox4.Controls.Add(this.txtChequeNo);
            this.groupBox4.Controls.Add(this.label6);
            this.groupBox4.Location = new System.Drawing.Point(387, -1);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(370, 121);
            this.groupBox4.TabIndex = 7;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Through Cheque";
            // 
            // dtpChequeDate
            // 
            this.dtpChequeDate.CustomFormat = "dd/MMM/yyyy";
            this.dtpChequeDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpChequeDate.Location = new System.Drawing.Point(106, 46);
            this.dtpChequeDate.MaximumSize = new System.Drawing.Size(4, 20);
            this.dtpChequeDate.MinimumSize = new System.Drawing.Size(125, 20);
            this.dtpChequeDate.Name = "dtpChequeDate";
            this.dtpChequeDate.Size = new System.Drawing.Size(125, 20);
            this.dtpChequeDate.TabIndex = 9;
            this.dtpChequeDate.ValueChanged += new System.EventHandler(this.dtpChequeDate_ValueChanged);
            this.dtpChequeDate.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dtpChequeDate_KeyDown);
            // 
            // txtChequeBankBranch
            // 
            this.txtChequeBankBranch.Location = new System.Drawing.Point(106, 96);
            this.txtChequeBankBranch.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtChequeBankBranch.MinimumSize = new System.Drawing.Size(250, 20);
            this.txtChequeBankBranch.Name = "txtChequeBankBranch";
            this.txtChequeBankBranch.Size = new System.Drawing.Size(250, 20);
            this.txtChequeBankBranch.TabIndex = 11;
            this.txtChequeBankBranch.TextChanged += new System.EventHandler(this.txtChequeBankBranch_TextChanged);
            this.txtChequeBankBranch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtChequeBankBranch_KeyDown);
            // 
            // txtChequeBank
            // 
            this.txtChequeBank.Location = new System.Drawing.Point(106, 71);
            this.txtChequeBank.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtChequeBank.MinimumSize = new System.Drawing.Size(250, 20);
            this.txtChequeBank.Name = "txtChequeBank";
            this.txtChequeBank.Size = new System.Drawing.Size(250, 20);
            this.txtChequeBank.TabIndex = 10;
            this.txtChequeBank.TextChanged += new System.EventHandler(this.txtChequeBank_TextChanged);
            this.txtChequeBank.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtChequeBank_KeyDown);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(24, 49);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(30, 13);
            this.label9.TabIndex = 109;
            this.label9.Text = "Date";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(24, 99);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(69, 13);
            this.label8.TabIndex = 108;
            this.label8.Text = "Bank Branch";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(24, 74);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(71, 13);
            this.label7.TabIndex = 107;
            this.label7.Text = "Deposit Bank";
            // 
            // txtChequeNo
            // 
            this.txtChequeNo.Location = new System.Drawing.Point(106, 21);
            this.txtChequeNo.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtChequeNo.MinimumSize = new System.Drawing.Size(250, 20);
            this.txtChequeNo.Name = "txtChequeNo";
            this.txtChequeNo.Size = new System.Drawing.Size(250, 20);
            this.txtChequeNo.TabIndex = 8;
            this.txtChequeNo.TextChanged += new System.EventHandler(this.txtChequeNo_TextChanged);
            this.txtChequeNo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtChequeNo_KeyDown);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(24, 24);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(61, 13);
            this.label6.TabIndex = 105;
            this.label6.Text = "Cheque No";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lstDepositType);
            this.groupBox1.Controls.Add(this.btnAddNew);
            this.groupBox1.Controls.Add(this.btnSearchDepositNo);
            this.groupBox1.Controls.Add(this.txtTreasuryNo);
            this.groupBox1.Controls.Add(this.label14);
            this.groupBox1.Controls.Add(this.txtDepositAmount);
            this.groupBox1.Controls.Add(this.txtDepositId);
            this.groupBox1.Controls.Add(this.dtpDepositDate);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(3, -5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(382, 147);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // lstDepositType
            // 
            this.lstDepositType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.lstDepositType.FormattingEnabled = true;
            this.lstDepositType.Items.AddRange(new object[] {
            "Cash",
            "Cheque"});
            this.lstDepositType.Location = new System.Drawing.Point(126, 71);
            this.lstDepositType.Name = "lstDepositType";
            this.lstDepositType.Size = new System.Drawing.Size(127, 21);
            this.lstDepositType.TabIndex = 4;
            this.lstDepositType.SelectedIndexChanged += new System.EventHandler(this.lstDepositType_SelectedIndexChanged);
            this.lstDepositType.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lstDepositType_KeyDown);
            // 
            // btnAddNew
            // 
            this.btnAddNew.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnAddNew.Image = global::VATClient.Properties.Resources.Add_New;
            this.btnAddNew.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnAddNew.Location = new System.Drawing.Point(293, 21);
            this.btnAddNew.Name = "btnAddNew";
            this.btnAddNew.Size = new System.Drawing.Size(30, 20);
            this.btnAddNew.TabIndex = 2;
            this.btnAddNew.TabStop = false;
            this.btnAddNew.UseVisualStyleBackColor = false;
            this.btnAddNew.Click += new System.EventHandler(this.btnAddNew_Click);
            // 
            // btnSearchDepositNo
            // 
            this.btnSearchDepositNo.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSearchDepositNo.Image = global::VATClient.Properties.Resources.search;
            this.btnSearchDepositNo.Location = new System.Drawing.Point(257, 21);
            this.btnSearchDepositNo.Name = "btnSearchDepositNo";
            this.btnSearchDepositNo.Size = new System.Drawing.Size(30, 20);
            this.btnSearchDepositNo.TabIndex = 1;
            this.btnSearchDepositNo.TabStop = false;
            this.btnSearchDepositNo.UseVisualStyleBackColor = false;
            this.btnSearchDepositNo.Click += new System.EventHandler(this.btnSearchDepositNo_Click);
            // 
            // txtTreasuryNo
            // 
            this.txtTreasuryNo.Location = new System.Drawing.Point(126, 46);
            this.txtTreasuryNo.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtTreasuryNo.MinimumSize = new System.Drawing.Size(250, 20);
            this.txtTreasuryNo.Name = "txtTreasuryNo";
            this.txtTreasuryNo.Size = new System.Drawing.Size(250, 20);
            this.txtTreasuryNo.TabIndex = 3;
            this.txtTreasuryNo.TextChanged += new System.EventHandler(this.txtTreasuryNo_TextChanged);
            this.txtTreasuryNo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtTreasuryNo_KeyDown);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(12, 49);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(103, 13);
            this.label14.TabIndex = 105;
            this.label14.Text = "Treasury Challan No";
            // 
            // txtDepositAmount
            // 
            this.txtDepositAmount.Location = new System.Drawing.Point(126, 96);
            this.txtDepositAmount.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtDepositAmount.MinimumSize = new System.Drawing.Size(125, 20);
            this.txtDepositAmount.Name = "txtDepositAmount";
            this.txtDepositAmount.Size = new System.Drawing.Size(125, 20);
            this.txtDepositAmount.TabIndex = 5;
            this.txtDepositAmount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtDepositAmount.TextChanged += new System.EventHandler(this.txtDepositAmount_TextChanged);
            this.txtDepositAmount.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtDepositAmount_KeyDown);
            // 
            // txtDepositId
            // 
            this.txtDepositId.Location = new System.Drawing.Point(127, 21);
            this.txtDepositId.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtDepositId.MinimumSize = new System.Drawing.Size(125, 20);
            this.txtDepositId.Name = "txtDepositId";
            this.txtDepositId.ReadOnly = true;
            this.txtDepositId.Size = new System.Drawing.Size(125, 20);
            this.txtDepositId.TabIndex = 0;
            this.txtDepositId.TabStop = false;
            // 
            // dtpDepositDate
            // 
            this.dtpDepositDate.CustomFormat = "dd/MMM/yyyy";
            this.dtpDepositDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpDepositDate.Location = new System.Drawing.Point(126, 121);
            this.dtpDepositDate.MaximumSize = new System.Drawing.Size(120, 20);
            this.dtpDepositDate.MinimumSize = new System.Drawing.Size(125, 20);
            this.dtpDepositDate.Name = "dtpDepositDate";
            this.dtpDepositDate.Size = new System.Drawing.Size(125, 20);
            this.dtpDepositDate.TabIndex = 6;
            this.dtpDepositDate.ValueChanged += new System.EventHandler(this.dtpDepositDate_ValueChanged);
            this.dtpDepositDate.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dtpDepositDate_KeyDown);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 74);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(70, 13);
            this.label3.TabIndex = 29;
            this.label3.Text = "Deposit Type";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 99);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(82, 13);
            this.label4.TabIndex = 25;
            this.label4.Text = "Deposit Amount";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 124);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 13);
            this.label2.TabIndex = 23;
            this.label2.Text = "Deposit Date";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 13);
            this.label1.TabIndex = 21;
            this.label1.Text = "Auto No";
            // 
            // bgwSave
            // 
            this.bgwSave.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwSave_DoWork);
            this.bgwSave.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwSave_RunWorkerCompleted);
            // 
            // bgwUpdate
            // 
            this.bgwUpdate.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwUpdate_DoWork);
            this.bgwUpdate.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwUpdate_RunWorkerCompleted);
            // 
            // bgwPost
            // 
            this.bgwPost.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwPost_DoWork);
            this.bgwPost.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwPost_RunWorkerCompleted);
            // 
            // bgwTR
            // 
            this.bgwTR.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwTR_DoWork);
            this.bgwTR.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwTR_RunWorkerCompleted);
            // 
            // FormSDDeposit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(759, 352);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormSDDeposit";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SD Deposit";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormSDDeposit_FormClosing);
            this.Load += new System.EventHandler(this.FormSDDeposit_Load);
            this.panel1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnPost;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.Button btnVAT18;
        private System.Windows.Forms.Button btnPrintGrid;
        private System.Windows.Forms.Button btnPrintList;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button btnSearchBankName;
        private System.Windows.Forms.ComboBox cmbBankName;
        private System.Windows.Forms.TextBox txtAccountNumber;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TextBox txtBranchName;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox txtBankName;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox txtBankID;
        private System.Windows.Forms.TextBox txtTreasuryCopy;
        private System.Windows.Forms.TextBox txtDepositPersonDesignation;
        private System.Windows.Forms.TextBox txtDepositPerson;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtComments;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.DateTimePicker dtpChequeDate;
        private System.Windows.Forms.TextBox txtChequeBankBranch;
        private System.Windows.Forms.TextBox txtChequeBank;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtChequeNo;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox lstDepositType;
        private System.Windows.Forms.Button btnAddNew;
        private System.Windows.Forms.Button btnSearchDepositNo;
        private System.Windows.Forms.TextBox txtTreasuryNo;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox txtDepositAmount;
        private System.Windows.Forms.TextBox txtDepositId;
        private System.Windows.Forms.DateTimePicker dtpDepositDate;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.ComponentModel.BackgroundWorker bgwSave;
        private System.ComponentModel.BackgroundWorker bgwUpdate;
        private System.ComponentModel.BackgroundWorker bgwPost;
        //private CustomReports.CachedTestReport cachedTestReport1;
        private System.Windows.Forms.Button btnPrintTR;
        private System.ComponentModel.BackgroundWorker bgwTR;


    }
}