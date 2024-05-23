namespace VATClient
{
    partial class FormBankInformationSearch
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
            this.grbBankInformation = new System.Windows.Forms.GroupBox();
            this.chkSelectAll = new System.Windows.Forms.CheckBox();
            this.cmbImport = new System.Windows.Forms.ComboBox();
            this.cmbBranch = new System.Windows.Forms.ComboBox();
            this.btnExport = new System.Windows.Forms.Button();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.cmbActive = new System.Windows.Forms.ComboBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.txtCity = new System.Windows.Forms.TextBox();
            this.txtAccountNumber = new System.Windows.Forms.TextBox();
            this.txtBranchName = new System.Windows.Forms.TextBox();
            this.txtBankName = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.btnSearch = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtBankCode = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.txtContactPersonEmail = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtContactPersonTelephone = new System.Windows.Forms.TextBox();
            this.txtContactPersonDesignation = new System.Windows.Forms.TextBox();
            this.txtContactPerson = new System.Windows.Forms.TextBox();
            this.txtFaxNo = new System.Windows.Forms.TextBox();
            this.txtTelephoneNo = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dgvBankInformation = new System.Windows.Forms.DataGridView();
            this.Select = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Code = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BankID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BankName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BranchName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AccountNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.City = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TelephoneNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FaxNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Email = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ContactPerson = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ContactPersonDesignation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ContactPersonTelephone = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ContactPersonEmail = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Comments = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Address1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Address2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Address3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ActiveStatus1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BranchId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel1 = new System.Windows.Forms.Panel();
            this.LRecordCount = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.bgwSearch = new System.ComponentModel.BackgroundWorker();
            this.cachedRptVendorListing1 = new SymphonySofttech.Reports.Report.CachedRptVendorListing();
            this.grbBankInformation.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBankInformation)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // grbBankInformation
            // 
            this.grbBankInformation.Controls.Add(this.chkSelectAll);
            this.grbBankInformation.Controls.Add(this.cmbImport);
            this.grbBankInformation.Controls.Add(this.cmbBranch);
            this.grbBankInformation.Controls.Add(this.btnExport);
            this.grbBankInformation.Controls.Add(this.label12);
            this.grbBankInformation.Controls.Add(this.label11);
            this.grbBankInformation.Controls.Add(this.cmbActive);
            this.grbBankInformation.Controls.Add(this.btnAdd);
            this.grbBankInformation.Controls.Add(this.txtCity);
            this.grbBankInformation.Controls.Add(this.txtAccountNumber);
            this.grbBankInformation.Controls.Add(this.txtBranchName);
            this.grbBankInformation.Controls.Add(this.txtBankName);
            this.grbBankInformation.Controls.Add(this.label10);
            this.grbBankInformation.Controls.Add(this.btnSearch);
            this.grbBankInformation.Controls.Add(this.label4);
            this.grbBankInformation.Controls.Add(this.label3);
            this.grbBankInformation.Controls.Add(this.label2);
            this.grbBankInformation.Controls.Add(this.txtBankCode);
            this.grbBankInformation.Controls.Add(this.label1);
            this.grbBankInformation.Location = new System.Drawing.Point(16, 1);
            this.grbBankInformation.Name = "grbBankInformation";
            this.grbBankInformation.Size = new System.Drawing.Size(753, 118);
            this.grbBankInformation.TabIndex = 6;
            this.grbBankInformation.TabStop = false;
            this.grbBankInformation.Text = "Searching Criteria";
            // 
            // chkSelectAll
            // 
            this.chkSelectAll.AutoSize = true;
            this.chkSelectAll.Location = new System.Drawing.Point(2, 93);
            this.chkSelectAll.Name = "chkSelectAll";
            this.chkSelectAll.Size = new System.Drawing.Size(66, 17);
            this.chkSelectAll.TabIndex = 216;
            this.chkSelectAll.Text = "SelectAll";
            this.chkSelectAll.UseVisualStyleBackColor = true;
            this.chkSelectAll.CheckedChanged += new System.EventHandler(this.chkSelectAll_CheckedChanged);
            // 
            // cmbImport
            // 
            this.cmbImport.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbImport.FormattingEnabled = true;
            this.cmbImport.Items.AddRange(new object[] {
            "Excel",
            "Text"});
            this.cmbImport.Location = new System.Drawing.Point(577, 91);
            this.cmbImport.Name = "cmbImport";
            this.cmbImport.Size = new System.Drawing.Size(66, 21);
            this.cmbImport.TabIndex = 221;
            // 
            // cmbBranch
            // 
            this.cmbBranch.FormattingEnabled = true;
            this.cmbBranch.Location = new System.Drawing.Point(522, 68);
            this.cmbBranch.Name = "cmbBranch";
            this.cmbBranch.Size = new System.Drawing.Size(121, 21);
            this.cmbBranch.TabIndex = 200;
            // 
            // btnExport
            // 
            this.btnExport.Location = new System.Drawing.Point(667, 89);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(75, 23);
            this.btnExport.TabIndex = 220;
            this.btnExport.Text = "Export";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(475, 73);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(40, 13);
            this.label12.TabIndex = 199;
            this.label12.Text = "Branch";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(330, 73);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(37, 13);
            this.label11.TabIndex = 198;
            this.label11.Text = "Active";
            // 
            // cmbActive
            // 
            this.cmbActive.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbActive.FormattingEnabled = true;
            this.cmbActive.Items.AddRange(new object[] {
            "Y",
            "N"});
            this.cmbActive.Location = new System.Drawing.Point(429, 69);
            this.cmbActive.Name = "cmbActive";
            this.cmbActive.Size = new System.Drawing.Size(44, 21);
            this.cmbActive.TabIndex = 197;
            // 
            // btnAdd
            // 
            this.btnAdd.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnAdd.Image = global::VATClient.Properties.Resources.add_over;
            this.btnAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAdd.Location = new System.Drawing.Point(667, 54);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 28);
            this.btnAdd.TabIndex = 33;
            this.btnAdd.Text = "&Add";
            this.btnAdd.UseVisualStyleBackColor = false;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // txtCity
            // 
            this.txtCity.Location = new System.Drawing.Point(429, 45);
            this.txtCity.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtCity.MinimumSize = new System.Drawing.Size(185, 20);
            this.txtCity.Name = "txtCity";
            this.txtCity.Size = new System.Drawing.Size(185, 21);
            this.txtCity.TabIndex = 4;
            this.txtCity.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtCity_KeyDown);
            // 
            // txtAccountNumber
            // 
            this.txtAccountNumber.Location = new System.Drawing.Point(429, 21);
            this.txtAccountNumber.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtAccountNumber.MinimumSize = new System.Drawing.Size(185, 20);
            this.txtAccountNumber.Name = "txtAccountNumber";
            this.txtAccountNumber.Size = new System.Drawing.Size(185, 21);
            this.txtAccountNumber.TabIndex = 3;
            this.txtAccountNumber.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtAccountNumber_KeyDown);
            // 
            // txtBranchName
            // 
            this.txtBranchName.Location = new System.Drawing.Point(109, 69);
            this.txtBranchName.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtBranchName.MinimumSize = new System.Drawing.Size(185, 20);
            this.txtBranchName.Name = "txtBranchName";
            this.txtBranchName.Size = new System.Drawing.Size(185, 21);
            this.txtBranchName.TabIndex = 2;
            this.txtBranchName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtBranchName_KeyDown);
            // 
            // txtBankName
            // 
            this.txtBankName.Location = new System.Drawing.Point(109, 45);
            this.txtBankName.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtBankName.MinimumSize = new System.Drawing.Size(185, 20);
            this.txtBankName.Name = "txtBankName";
            this.txtBankName.Size = new System.Drawing.Size(185, 21);
            this.txtBankName.TabIndex = 1;
            this.txtBankName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtBankName_KeyDown);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(330, 49);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(30, 13);
            this.label10.TabIndex = 6;
            this.label10.Text = "City:";
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSearch.Image = global::VATClient.Properties.Resources.search;
            this.btnSearch.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSearch.Location = new System.Drawing.Point(667, 24);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 28);
            this.btnSearch.TabIndex = 6;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(330, 25);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(90, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Account Number:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(27, 73);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(74, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Branch Name:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(27, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Name:";
            // 
            // txtBankCode
            // 
            this.txtBankCode.Location = new System.Drawing.Point(109, 21);
            this.txtBankCode.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtBankCode.MinimumSize = new System.Drawing.Size(185, 20);
            this.txtBankCode.Name = "txtBankCode";
            this.txtBankCode.Size = new System.Drawing.Size(185, 21);
            this.txtBankCode.TabIndex = 0;
            this.txtBankCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtBankID_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(27, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(36, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Code:";
            // 
            // txtEmail
            // 
            this.txtEmail.Location = new System.Drawing.Point(915, 69);
            this.txtEmail.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtEmail.MinimumSize = new System.Drawing.Size(85, 20);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(85, 21);
            this.txtEmail.TabIndex = 33;
            // 
            // txtContactPersonEmail
            // 
            this.txtContactPersonEmail.Location = new System.Drawing.Point(915, 165);
            this.txtContactPersonEmail.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtContactPersonEmail.MinimumSize = new System.Drawing.Size(85, 20);
            this.txtContactPersonEmail.Name = "txtContactPersonEmail";
            this.txtContactPersonEmail.Size = new System.Drawing.Size(85, 21);
            this.txtContactPersonEmail.TabIndex = 27;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(843, 72);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(35, 13);
            this.label5.TabIndex = 5;
            this.label5.Text = "Email:";
            // 
            // txtContactPersonTelephone
            // 
            this.txtContactPersonTelephone.Location = new System.Drawing.Point(915, 141);
            this.txtContactPersonTelephone.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtContactPersonTelephone.MinimumSize = new System.Drawing.Size(85, 20);
            this.txtContactPersonTelephone.Name = "txtContactPersonTelephone";
            this.txtContactPersonTelephone.Size = new System.Drawing.Size(85, 21);
            this.txtContactPersonTelephone.TabIndex = 26;
            // 
            // txtContactPersonDesignation
            // 
            this.txtContactPersonDesignation.Location = new System.Drawing.Point(915, 117);
            this.txtContactPersonDesignation.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtContactPersonDesignation.MinimumSize = new System.Drawing.Size(85, 20);
            this.txtContactPersonDesignation.Name = "txtContactPersonDesignation";
            this.txtContactPersonDesignation.Size = new System.Drawing.Size(85, 21);
            this.txtContactPersonDesignation.TabIndex = 25;
            // 
            // txtContactPerson
            // 
            this.txtContactPerson.Location = new System.Drawing.Point(915, 93);
            this.txtContactPerson.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtContactPerson.MinimumSize = new System.Drawing.Size(85, 20);
            this.txtContactPerson.Name = "txtContactPerson";
            this.txtContactPerson.Size = new System.Drawing.Size(85, 21);
            this.txtContactPerson.TabIndex = 24;
            // 
            // txtFaxNo
            // 
            this.txtFaxNo.Location = new System.Drawing.Point(915, 191);
            this.txtFaxNo.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtFaxNo.MinimumSize = new System.Drawing.Size(85, 20);
            this.txtFaxNo.Name = "txtFaxNo";
            this.txtFaxNo.Size = new System.Drawing.Size(85, 21);
            this.txtFaxNo.TabIndex = 23;
            // 
            // txtTelephoneNo
            // 
            this.txtTelephoneNo.Location = new System.Drawing.Point(915, 220);
            this.txtTelephoneNo.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtTelephoneNo.MinimumSize = new System.Drawing.Size(85, 20);
            this.txtTelephoneNo.Name = "txtTelephoneNo";
            this.txtTelephoneNo.Size = new System.Drawing.Size(85, 21);
            this.txtTelephoneNo.TabIndex = 22;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(843, 168);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(35, 13);
            this.label14.TabIndex = 12;
            this.label14.Text = "Email:";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(843, 144);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(61, 13);
            this.label15.TabIndex = 11;
            this.label15.Text = "Telephone:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(843, 120);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(67, 13);
            this.label6.TabIndex = 10;
            this.label6.Text = "Designation:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(843, 96);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(44, 13);
            this.label7.TabIndex = 9;
            this.label7.Text = "Person:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(843, 198);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(45, 13);
            this.label8.TabIndex = 8;
            this.label8.Text = "Fax No:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(832, 227);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(77, 13);
            this.label9.TabIndex = 7;
            this.label9.Text = "Telephone No:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dgvBankInformation);
            this.groupBox1.Location = new System.Drawing.Point(16, 114);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(753, 248);
            this.groupBox1.TabIndex = 60;
            this.groupBox1.TabStop = false;
            // 
            // dgvBankInformation
            // 
            this.dgvBankInformation.AllowUserToAddRows = false;
            this.dgvBankInformation.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.dgvBankInformation.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvBankInformation.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvBankInformation.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Select,
            this.Code,
            this.BankID,
            this.BankName,
            this.BranchName,
            this.AccountNumber,
            this.City,
            this.TelephoneNo,
            this.FaxNo,
            this.Email,
            this.ContactPerson,
            this.ContactPersonDesignation,
            this.ContactPersonTelephone,
            this.ContactPersonEmail,
            this.Comments,
            this.Address1,
            this.Address2,
            this.Address3,
            this.ActiveStatus1,
            this.BranchId});
            this.dgvBankInformation.Location = new System.Drawing.Point(5, 11);
            this.dgvBankInformation.Name = "dgvBankInformation";
            this.dgvBankInformation.RowHeadersVisible = false;
            this.dgvBankInformation.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvBankInformation.Size = new System.Drawing.Size(742, 231);
            this.dgvBankInformation.TabIndex = 9;
            this.dgvBankInformation.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvBankInformation_CellContentClick);
            this.dgvBankInformation.DoubleClick += new System.EventHandler(this.dgvBankInformation_DoubleClick);
            // 
            // Select
            // 
            this.Select.HeaderText = "Select";
            this.Select.Name = "Select";
            // 
            // Code
            // 
            this.Code.HeaderText = "Code";
            this.Code.Name = "Code";
            this.Code.ReadOnly = true;
            // 
            // BankID
            // 
            this.BankID.HeaderText = "Bank ID";
            this.BankID.Name = "BankID";
            this.BankID.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.BankID.Visible = false;
            this.BankID.Width = 75;
            // 
            // BankName
            // 
            this.BankName.HeaderText = "Bank Name";
            this.BankName.Name = "BankName";
            this.BankName.Width = 85;
            // 
            // BranchName
            // 
            this.BranchName.HeaderText = "Branch Name";
            this.BranchName.Name = "BranchName";
            this.BranchName.Width = 95;
            // 
            // AccountNumber
            // 
            this.AccountNumber.HeaderText = "Account Number";
            this.AccountNumber.Name = "AccountNumber";
            this.AccountNumber.Width = 115;
            // 
            // City
            // 
            this.City.HeaderText = "City";
            this.City.Name = "City";
            this.City.Visible = false;
            this.City.Width = 51;
            // 
            // TelephoneNo
            // 
            this.TelephoneNo.HeaderText = "Telephone";
            this.TelephoneNo.Name = "TelephoneNo";
            this.TelephoneNo.Width = 90;
            // 
            // FaxNo
            // 
            this.FaxNo.HeaderText = "Fax No";
            this.FaxNo.Name = "FaxNo";
            // 
            // Email
            // 
            this.Email.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.Email.HeaderText = "Email";
            this.Email.Name = "Email";
            this.Email.Visible = false;
            // 
            // ContactPerson
            // 
            this.ContactPerson.HeaderText = "Contact Person";
            this.ContactPerson.Name = "ContactPerson";
            this.ContactPerson.Width = 115;
            // 
            // ContactPersonDesignation
            // 
            this.ContactPersonDesignation.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.ContactPersonDesignation.HeaderText = "C Person Designation";
            this.ContactPersonDesignation.Name = "ContactPersonDesignation";
            this.ContactPersonDesignation.Visible = false;
            // 
            // ContactPersonTelephone
            // 
            this.ContactPersonTelephone.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.ContactPersonTelephone.HeaderText = "C Person Telephone";
            this.ContactPersonTelephone.Name = "ContactPersonTelephone";
            this.ContactPersonTelephone.Visible = false;
            // 
            // ContactPersonEmail
            // 
            this.ContactPersonEmail.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.ContactPersonEmail.HeaderText = "Contact Person Email";
            this.ContactPersonEmail.Name = "ContactPersonEmail";
            this.ContactPersonEmail.Visible = false;
            // 
            // Comments
            // 
            this.Comments.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.Comments.HeaderText = "Comments";
            this.Comments.Name = "Comments";
            this.Comments.Visible = false;
            // 
            // Address1
            // 
            this.Address1.HeaderText = "Address1";
            this.Address1.Name = "Address1";
            this.Address1.Visible = false;
            this.Address1.Width = 5;
            // 
            // Address2
            // 
            this.Address2.HeaderText = "Address2";
            this.Address2.Name = "Address2";
            this.Address2.Visible = false;
            this.Address2.Width = 5;
            // 
            // Address3
            // 
            this.Address3.HeaderText = "Address3";
            this.Address3.Name = "Address3";
            this.Address3.Visible = false;
            this.Address3.Width = 5;
            // 
            // ActiveStatus1
            // 
            this.ActiveStatus1.HeaderText = "ActiveStatus";
            this.ActiveStatus1.Name = "ActiveStatus1";
            this.ActiveStatus1.Visible = false;
            // 
            // BranchId
            // 
            this.BranchId.HeaderText = "BranchId";
            this.BranchId.Name = "BranchId";
            this.BranchId.ReadOnly = true;
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::VATClient.Properties.Resources.Green_Footer;
            this.panel1.Controls.Add(this.LRecordCount);
            this.panel1.Controls.Add(this.progressBar1);
            this.panel1.Controls.Add(this.btnRefresh);
            this.panel1.Controls.Add(this.btnOk);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Location = new System.Drawing.Point(0, 362);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(792, 40);
            this.panel1.TabIndex = 8;
            // 
            // LRecordCount
            // 
            this.LRecordCount.AutoSize = true;
            this.LRecordCount.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LRecordCount.Location = new System.Drawing.Point(277, 14);
            this.LRecordCount.Name = "LRecordCount";
            this.LRecordCount.Size = new System.Drawing.Size(94, 16);
            this.LRecordCount.TabIndex = 229;
            this.LRecordCount.Text = "Record Count :";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(377, 6);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(288, 24);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 211;
            this.progressBar1.Visible = false;
            // 
            // btnRefresh
            // 
            this.btnRefresh.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnRefresh.Image = global::VATClient.Properties.Resources.Referesh;
            this.btnRefresh.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnRefresh.Location = new System.Drawing.Point(16, 8);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(75, 28);
            this.btnRefresh.TabIndex = 9;
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
            this.btnOk.Location = new System.Drawing.Point(690, 6);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 28);
            this.btnOk.TabIndex = 11;
            this.btnOk.Text = "&Ok";
            this.btnOk.UseVisualStyleBackColor = false;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button1.Image = global::VATClient.Properties.Resources.Referesh;
            this.button1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button1.Location = new System.Drawing.Point(196, 8);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 28);
            this.button1.TabIndex = 10;
            this.button1.Text = "&Refresh";
            this.button1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // bgwSearch
            // 
            this.bgwSearch.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwSearch_DoWork);
            this.bgwSearch.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwSearch_RunWorkerCompleted);
            // 
            // FormBankInformationSearch
            // 
            this.AcceptButton = this.btnSearch;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.CancelButton = this.btnOk;
            this.ClientSize = new System.Drawing.Size(784, 401);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.txtEmail);
            this.Controls.Add(this.txtTelephoneNo);
            this.Controls.Add(this.txtFaxNo);
            this.Controls.Add(this.txtContactPersonEmail);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.grbBankInformation);
            this.Controls.Add(this.txtContactPersonTelephone);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.txtContactPersonDesignation);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtContactPerson);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.label14);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(800, 440);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(800, 440);
            this.Name = "FormBankInformationSearch";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Bank Search";
            this.Load += new System.EventHandler(this.FormBankInformationSearch_Load);
            this.grbBankInformation.ResumeLayout(false);
            this.grbBankInformation.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvBankInformation)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.GroupBox grbBankInformation;
        private System.Windows.Forms.TextBox txtContactPersonEmail;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtContactPersonTelephone;
        private System.Windows.Forms.TextBox txtContactPersonDesignation;
        private System.Windows.Forms.TextBox txtContactPerson;
        private System.Windows.Forms.TextBox txtFaxNo;
        private System.Windows.Forms.TextBox txtTelephoneNo;
        private System.Windows.Forms.TextBox txtCity;
        private System.Windows.Forms.TextBox txtAccountNumber;
        private System.Windows.Forms.TextBox txtBranchName;
        private System.Windows.Forms.TextBox txtBankName;
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
        private System.Windows.Forms.TextBox txtBankCode;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtEmail;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridView dgvBankInformation;
        public System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.ComponentModel.BackgroundWorker bgwSearch;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnRefresh;
        private SymphonySofttech.Reports.Report.CachedRptVendorListing cachedRptVendorListing1;
        private System.Windows.Forms.Label LRecordCount;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ComboBox cmbActive;
        private System.Windows.Forms.ComboBox cmbBranch;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.CheckBox chkSelectAll;
        private System.Windows.Forms.ComboBox cmbImport;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Select;
        private System.Windows.Forms.DataGridViewTextBoxColumn Code;
        private System.Windows.Forms.DataGridViewTextBoxColumn BankID;
        private System.Windows.Forms.DataGridViewTextBoxColumn BankName;
        private System.Windows.Forms.DataGridViewTextBoxColumn BranchName;
        private System.Windows.Forms.DataGridViewTextBoxColumn AccountNumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn City;
        private System.Windows.Forms.DataGridViewTextBoxColumn TelephoneNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn FaxNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn Email;
        private System.Windows.Forms.DataGridViewTextBoxColumn ContactPerson;
        private System.Windows.Forms.DataGridViewTextBoxColumn ContactPersonDesignation;
        private System.Windows.Forms.DataGridViewTextBoxColumn ContactPersonTelephone;
        private System.Windows.Forms.DataGridViewTextBoxColumn ContactPersonEmail;
        private System.Windows.Forms.DataGridViewTextBoxColumn Comments;
        private System.Windows.Forms.DataGridViewTextBoxColumn Address1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Address2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Address3;
        private System.Windows.Forms.DataGridViewTextBoxColumn ActiveStatus1;
        private System.Windows.Forms.DataGridViewTextBoxColumn BranchId;
    }
}