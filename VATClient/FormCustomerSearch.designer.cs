namespace VATClient
{
    partial class FormCustomerSearch
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
            this.btnSearch = new System.Windows.Forms.Button();
            this.grbCustomer = new System.Windows.Forms.GroupBox();
            this.cmbRecordCount = new System.Windows.Forms.ComboBox();
            this.chkSelectAll = new System.Windows.Forms.CheckBox();
            this.cmbImport = new System.Windows.Forms.ComboBox();
            this.cmbBranch = new System.Windows.Forms.ComboBox();
            this.label19 = new System.Windows.Forms.Label();
            this.btnExport = new System.Windows.Forms.Button();
            this.label16 = new System.Windows.Forms.Label();
            this.cmbActive = new System.Windows.Forms.ComboBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.cmbType = new System.Windows.Forms.ComboBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label18 = new System.Windows.Forms.Label();
            this.txtCustomerGroupName = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.dtpDateTo = new System.Windows.Forms.DateTimePicker();
            this.dtpDateFrom = new System.Windows.Forms.DateTimePicker();
            this.txtVATRegistrationNo = new System.Windows.Forms.TextBox();
            this.txtTINNo = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtContactPerson = new System.Windows.Forms.TextBox();
            this.txtCity = new System.Windows.Forms.TextBox();
            this.txtCustomerName = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtCustomerCode = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.LRecordCount = new System.Windows.Forms.Label();
            this.btnAdd = new System.Windows.Forms.Button();
            this.txtCustomerGroupID = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtContactPersonEmail = new System.Windows.Forms.TextBox();
            this.txtContactPersonTelephone = new System.Windows.Forms.TextBox();
            this.txtContactPersonDesignation = new System.Windows.Forms.TextBox();
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.txtFaxNo = new System.Windows.Forms.TextBox();
            this.txtTelephoneNo = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.dgvCustomer = new System.Windows.Forms.DataGridView();
            this.Select = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.backgroundWorkerSearch = new System.ComponentModel.BackgroundWorker();
            this.grbCustomer.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCustomer)).BeginInit();
            this.SuspendLayout();
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSearch.Image = global::VATClient.Properties.Resources.search;
            this.btnSearch.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSearch.Location = new System.Drawing.Point(665, 45);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 28);
            this.btnSearch.TabIndex = 11;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // grbCustomer
            // 
            this.grbCustomer.Controls.Add(this.cmbRecordCount);
            this.grbCustomer.Controls.Add(this.chkSelectAll);
            this.grbCustomer.Controls.Add(this.cmbBranch);
            this.grbCustomer.Controls.Add(this.label19);
            this.grbCustomer.Controls.Add(this.label16);
            this.grbCustomer.Controls.Add(this.cmbActive);
            this.grbCustomer.Controls.Add(this.btnOk);
            this.grbCustomer.Controls.Add(this.cmbType);
            this.grbCustomer.Controls.Add(this.btnCancel);
            this.grbCustomer.Controls.Add(this.label18);
            this.grbCustomer.Controls.Add(this.txtCustomerGroupName);
            this.grbCustomer.Controls.Add(this.label17);
            this.grbCustomer.Controls.Add(this.label11);
            this.grbCustomer.Controls.Add(this.btnSearch);
            this.grbCustomer.Controls.Add(this.dtpDateTo);
            this.grbCustomer.Controls.Add(this.dtpDateFrom);
            this.grbCustomer.Controls.Add(this.txtVATRegistrationNo);
            this.grbCustomer.Controls.Add(this.txtTINNo);
            this.grbCustomer.Controls.Add(this.label5);
            this.grbCustomer.Controls.Add(this.txtContactPerson);
            this.grbCustomer.Controls.Add(this.txtCity);
            this.grbCustomer.Controls.Add(this.txtCustomerName);
            this.grbCustomer.Controls.Add(this.label12);
            this.grbCustomer.Controls.Add(this.label13);
            this.grbCustomer.Controls.Add(this.label7);
            this.grbCustomer.Controls.Add(this.label4);
            this.grbCustomer.Controls.Add(this.label2);
            this.grbCustomer.Controls.Add(this.txtCustomerCode);
            this.grbCustomer.Controls.Add(this.label1);
            this.grbCustomer.Location = new System.Drawing.Point(2, -2);
            this.grbCustomer.Name = "grbCustomer";
            this.grbCustomer.Size = new System.Drawing.Size(760, 183);
            this.grbCustomer.TabIndex = 3;
            this.grbCustomer.TabStop = false;
            this.grbCustomer.Text = "Searchig Criteria";
            this.grbCustomer.Enter += new System.EventHandler(this.grbCustomer_Enter);
            // 
            // cmbRecordCount
            // 
            this.cmbRecordCount.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRecordCount.FormattingEnabled = true;
            this.cmbRecordCount.Location = new System.Drawing.Point(147, 159);
            this.cmbRecordCount.Name = "cmbRecordCount";
            this.cmbRecordCount.Size = new System.Drawing.Size(78, 21);
            this.cmbRecordCount.TabIndex = 235;
            // 
            // chkSelectAll
            // 
            this.chkSelectAll.AutoSize = true;
            this.chkSelectAll.Location = new System.Drawing.Point(26, 161);
            this.chkSelectAll.Name = "chkSelectAll";
            this.chkSelectAll.Size = new System.Drawing.Size(66, 17);
            this.chkSelectAll.TabIndex = 215;
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
            this.cmbImport.Location = new System.Drawing.Point(503, 430);
            this.cmbImport.Name = "cmbImport";
            this.cmbImport.Size = new System.Drawing.Size(62, 21);
            this.cmbImport.TabIndex = 231;
            // 
            // cmbBranch
            // 
            this.cmbBranch.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBranch.FormattingEnabled = true;
            this.cmbBranch.Items.AddRange(new object[] {
            "Cash Payable",
            "Credit Payable",
            "Rebatable"});
            this.cmbBranch.Location = new System.Drawing.Point(147, 135);
            this.cmbBranch.Name = "cmbBranch";
            this.cmbBranch.Size = new System.Drawing.Size(120, 21);
            this.cmbBranch.Sorted = true;
            this.cmbBranch.TabIndex = 230;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(26, 140);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(40, 13);
            this.label19.TabIndex = 229;
            this.label19.Text = "Branch";
            // 
            // btnExport
            // 
            this.btnExport.Location = new System.Drawing.Point(583, 428);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(75, 23);
            this.btnExport.TabIndex = 228;
            this.btnExport.Text = "Export";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(26, 110);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(37, 13);
            this.label16.TabIndex = 227;
            this.label16.Text = "Active";
            // 
            // cmbActive
            // 
            this.cmbActive.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbActive.FormattingEnabled = true;
            this.cmbActive.Items.AddRange(new object[] {
            "Y",
            "N"});
            this.cmbActive.Location = new System.Drawing.Point(147, 110);
            this.cmbActive.Name = "cmbActive";
            this.cmbActive.Size = new System.Drawing.Size(44, 21);
            this.cmbActive.TabIndex = 226;
            // 
            // btnOk
            // 
            this.btnOk.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnOk.Image = global::VATClient.Properties.Resources.Back;
            this.btnOk.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOk.Location = new System.Drawing.Point(665, 102);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 28);
            this.btnOk.TabIndex = 13;
            this.btnOk.Text = "&Ok";
            this.btnOk.UseVisualStyleBackColor = false;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // cmbType
            // 
            this.cmbType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbType.FormattingEnabled = true;
            this.cmbType.Items.AddRange(new object[] {
            "Local",
            "Export"});
            this.cmbType.Location = new System.Drawing.Point(474, 109);
            this.cmbType.Name = "cmbType";
            this.cmbType.Size = new System.Drawing.Size(185, 21);
            this.cmbType.TabIndex = 10;
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnCancel.Image = global::VATClient.Properties.Resources.Referesh;
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.Location = new System.Drawing.Point(665, 74);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 28);
            this.btnCancel.TabIndex = 12;
            this.btnCancel.Text = "&Refresh";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(368, 113);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(31, 13);
            this.label18.TabIndex = 191;
            this.label18.Text = "Type";
            // 
            // txtCustomerGroupName
            // 
            this.txtCustomerGroupName.Location = new System.Drawing.Point(147, 63);
            this.txtCustomerGroupName.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtCustomerGroupName.MinimumSize = new System.Drawing.Size(185, 20);
            this.txtCustomerGroupName.Name = "txtCustomerGroupName";
            this.txtCustomerGroupName.Size = new System.Drawing.Size(185, 20);
            this.txtCustomerGroupName.TabIndex = 2;
            this.txtCustomerGroupName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtCustomerGroupName_KeyDown);
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(26, 65);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(66, 13);
            this.label17.TabIndex = 107;
            this.label17.Text = "Group Name";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(582, 23);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(17, 13);
            this.label11.TabIndex = 106;
            this.label11.Text = "to";
            // 
            // dtpDateTo
            // 
            this.dtpDateTo.Checked = false;
            this.dtpDateTo.CustomFormat = "dd/MMM/yyyy";
            this.dtpDateTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpDateTo.Location = new System.Drawing.Point(603, 19);
            this.dtpDateTo.Name = "dtpDateTo";
            this.dtpDateTo.ShowCheckBox = true;
            this.dtpDateTo.Size = new System.Drawing.Size(105, 21);
            this.dtpDateTo.TabIndex = 6;
            this.dtpDateTo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dtpDateTo_KeyDown);
            // 
            // dtpDateFrom
            // 
            this.dtpDateFrom.Checked = false;
            this.dtpDateFrom.CustomFormat = "dd/MMM/yyyy";
            this.dtpDateFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpDateFrom.Location = new System.Drawing.Point(474, 19);
            this.dtpDateFrom.Name = "dtpDateFrom";
            this.dtpDateFrom.ShowCheckBox = true;
            this.dtpDateFrom.Size = new System.Drawing.Size(104, 21);
            this.dtpDateFrom.TabIndex = 5;
            this.dtpDateFrom.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dtpDateFrom_KeyDown);
            // 
            // txtVATRegistrationNo
            // 
            this.txtVATRegistrationNo.Location = new System.Drawing.Point(474, 85);
            this.txtVATRegistrationNo.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtVATRegistrationNo.MinimumSize = new System.Drawing.Size(185, 20);
            this.txtVATRegistrationNo.Name = "txtVATRegistrationNo";
            this.txtVATRegistrationNo.Size = new System.Drawing.Size(185, 21);
            this.txtVATRegistrationNo.TabIndex = 9;
            this.txtVATRegistrationNo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtVATRegistrationNo_KeyDown);
            // 
            // txtTINNo
            // 
            this.txtTINNo.Location = new System.Drawing.Point(474, 63);
            this.txtTINNo.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtTINNo.MinimumSize = new System.Drawing.Size(185, 20);
            this.txtTINNo.Name = "txtTINNo";
            this.txtTINNo.Size = new System.Drawing.Size(185, 21);
            this.txtTINNo.TabIndex = 8;
            this.txtTINNo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtTINNo_KeyDown);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(365, 23);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(57, 13);
            this.label5.TabIndex = 5;
            this.label5.Text = "Start Date";
            // 
            // txtContactPerson
            // 
            this.txtContactPerson.Location = new System.Drawing.Point(474, 41);
            this.txtContactPerson.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtContactPerson.MinimumSize = new System.Drawing.Size(185, 20);
            this.txtContactPerson.Name = "txtContactPerson";
            this.txtContactPerson.Size = new System.Drawing.Size(185, 21);
            this.txtContactPerson.TabIndex = 7;
            this.txtContactPerson.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtContactPerson_KeyDown);
            // 
            // txtCity
            // 
            this.txtCity.Location = new System.Drawing.Point(147, 85);
            this.txtCity.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtCity.MinimumSize = new System.Drawing.Size(185, 20);
            this.txtCity.Name = "txtCity";
            this.txtCity.Size = new System.Drawing.Size(185, 20);
            this.txtCity.TabIndex = 3;
            this.txtCity.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtCity_KeyDown);
            // 
            // txtCustomerName
            // 
            this.txtCustomerName.Location = new System.Drawing.Point(147, 40);
            this.txtCustomerName.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtCustomerName.MinimumSize = new System.Drawing.Size(185, 20);
            this.txtCustomerName.Name = "txtCustomerName";
            this.txtCustomerName.Size = new System.Drawing.Size(185, 20);
            this.txtCustomerName.TabIndex = 1;
            this.txtCustomerName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtCustomerName_KeyDown);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(365, 87);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(100, 13);
            this.label12.TabIndex = 14;
            this.label12.Text = "Vat Registration No";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(365, 67);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(24, 13);
            this.label13.TabIndex = 13;
            this.label13.Text = "TIN";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(365, 45);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(81, 13);
            this.label7.TabIndex = 9;
            this.label7.Text = "Contact Person";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(26, 87);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(26, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "City";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(26, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Customer Name";
            // 
            // txtCustomerCode
            // 
            this.txtCustomerCode.Location = new System.Drawing.Point(147, 17);
            this.txtCustomerCode.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtCustomerCode.MinimumSize = new System.Drawing.Size(185, 20);
            this.txtCustomerCode.Name = "txtCustomerCode";
            this.txtCustomerCode.Size = new System.Drawing.Size(185, 20);
            this.txtCustomerCode.TabIndex = 0;
            this.txtCustomerCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtCustomerID_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(26, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(81, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Customer Code";
            // 
            // LRecordCount
            // 
            this.LRecordCount.AutoSize = true;
            this.LRecordCount.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LRecordCount.Location = new System.Drawing.Point(17, 431);
            this.LRecordCount.Name = "LRecordCount";
            this.LRecordCount.Size = new System.Drawing.Size(94, 16);
            this.LRecordCount.TabIndex = 225;
            this.LRecordCount.Text = "Record Count :";
            // 
            // btnAdd
            // 
            this.btnAdd.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnAdd.Image = global::VATClient.Properties.Resources.add_over;
            this.btnAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAdd.Location = new System.Drawing.Point(632, 75);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 28);
            this.btnAdd.TabIndex = 108;
            this.btnAdd.Text = "&Add";
            this.btnAdd.UseVisualStyleBackColor = false;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // txtCustomerGroupID
            // 
            this.txtCustomerGroupID.Location = new System.Drawing.Point(796, 196);
            this.txtCustomerGroupID.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtCustomerGroupID.MinimumSize = new System.Drawing.Size(185, 20);
            this.txtCustomerGroupID.Name = "txtCustomerGroupID";
            this.txtCustomerGroupID.Size = new System.Drawing.Size(185, 21);
            this.txtCustomerGroupID.TabIndex = 2;
            this.txtCustomerGroupID.Visible = false;
            this.txtCustomerGroupID.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtCustomerGroupID_KeyDown);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(580, 200);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(50, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Group ID";
            this.label3.Visible = false;
            // 
            // txtContactPersonEmail
            // 
            this.txtContactPersonEmail.Location = new System.Drawing.Point(904, 224);
            this.txtContactPersonEmail.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtContactPersonEmail.MinimumSize = new System.Drawing.Size(85, 20);
            this.txtContactPersonEmail.Name = "txtContactPersonEmail";
            this.txtContactPersonEmail.Size = new System.Drawing.Size(85, 21);
            this.txtContactPersonEmail.TabIndex = 27;
            // 
            // txtContactPersonTelephone
            // 
            this.txtContactPersonTelephone.Location = new System.Drawing.Point(904, 200);
            this.txtContactPersonTelephone.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtContactPersonTelephone.MinimumSize = new System.Drawing.Size(85, 20);
            this.txtContactPersonTelephone.Name = "txtContactPersonTelephone";
            this.txtContactPersonTelephone.Size = new System.Drawing.Size(85, 21);
            this.txtContactPersonTelephone.TabIndex = 26;
            // 
            // txtContactPersonDesignation
            // 
            this.txtContactPersonDesignation.Location = new System.Drawing.Point(904, 174);
            this.txtContactPersonDesignation.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtContactPersonDesignation.MinimumSize = new System.Drawing.Size(85, 20);
            this.txtContactPersonDesignation.Name = "txtContactPersonDesignation";
            this.txtContactPersonDesignation.Size = new System.Drawing.Size(85, 21);
            this.txtContactPersonDesignation.TabIndex = 25;
            // 
            // txtEmail
            // 
            this.txtEmail.Location = new System.Drawing.Point(904, 147);
            this.txtEmail.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtEmail.MinimumSize = new System.Drawing.Size(85, 20);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(85, 21);
            this.txtEmail.TabIndex = 23;
            // 
            // txtFaxNo
            // 
            this.txtFaxNo.Location = new System.Drawing.Point(904, 123);
            this.txtFaxNo.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtFaxNo.MinimumSize = new System.Drawing.Size(85, 20);
            this.txtFaxNo.Name = "txtFaxNo";
            this.txtFaxNo.Size = new System.Drawing.Size(85, 21);
            this.txtFaxNo.TabIndex = 22;
            // 
            // txtTelephoneNo
            // 
            this.txtTelephoneNo.Location = new System.Drawing.Point(904, 96);
            this.txtTelephoneNo.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtTelephoneNo.MinimumSize = new System.Drawing.Size(85, 20);
            this.txtTelephoneNo.Name = "txtTelephoneNo";
            this.txtTelephoneNo.Size = new System.Drawing.Size(85, 21);
            this.txtTelephoneNo.TabIndex = 21;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(832, 225);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(35, 13);
            this.label14.TabIndex = 12;
            this.label14.Text = "Email:";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(832, 201);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(61, 13);
            this.label15.TabIndex = 11;
            this.label15.Text = "Telephone:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(832, 177);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(67, 13);
            this.label6.TabIndex = 10;
            this.label6.Text = "Designation:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(820, 149);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(35, 13);
            this.label8.TabIndex = 8;
            this.label8.Text = "Email:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(820, 125);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(45, 13);
            this.label9.TabIndex = 7;
            this.label9.Text = "Fax No:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(820, 98);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(77, 13);
            this.label10.TabIndex = 6;
            this.label10.Text = "Telephone No:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.progressBar1);
            this.groupBox1.Controls.Add(this.dgvCustomer);
            this.groupBox1.Controls.Add(this.btnAdd);
            this.groupBox1.Location = new System.Drawing.Point(3, 182);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(759, 242);
            this.groupBox1.TabIndex = 105;
            this.groupBox1.TabStop = false;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(184, 102);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(315, 23);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 106;
            this.progressBar1.Visible = false;
            // 
            // dgvCustomer
            // 
            this.dgvCustomer.AllowUserToAddRows = false;
            this.dgvCustomer.AllowUserToDeleteRows = false;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.dgvCustomer.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvCustomer.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCustomer.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Select});
            this.dgvCustomer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvCustomer.Location = new System.Drawing.Point(3, 17);
            this.dgvCustomer.Name = "dgvCustomer";
            this.dgvCustomer.RowHeadersVisible = false;
            this.dgvCustomer.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvCustomer.Size = new System.Drawing.Size(753, 222);
            this.dgvCustomer.TabIndex = 6;
            this.dgvCustomer.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvCustomer_CellContentClick_1);
            this.dgvCustomer.DoubleClick += new System.EventHandler(this.dgvCustomer_DoubleClick);
            // 
            // Select
            // 
            this.Select.HeaderText = "Select";
            this.Select.Name = "Select";
            // 
            // backgroundWorkerSearch
            // 
            this.backgroundWorkerSearch.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerSearch_DoWork);
            this.backgroundWorkerSearch.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerSearch_RunWorkerCompleted);
            // 
            // FormCustomerSearch
            // 
            this.AcceptButton = this.btnSearch;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.CancelButton = this.btnOk;
            this.ClientSize = new System.Drawing.Size(762, 456);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.txtCustomerGroupID);
            this.Controls.Add(this.cmbImport);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.grbCustomer);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.txtContactPersonDesignation);
            this.Controls.Add(this.LRecordCount);
            this.Controls.Add(this.txtContactPersonTelephone);
            this.Controls.Add(this.txtContactPersonEmail);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.txtEmail);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.txtFaxNo);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.txtTelephoneNo);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(800, 500);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(80, 50);
            this.Name = "FormCustomerSearch";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Customer Search";
            this.Load += new System.EventHandler(this.FormCustomerSearch_Load);
            this.grbCustomer.ResumeLayout(false);
            this.grbCustomer.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvCustomer)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.GroupBox grbCustomer;
        private System.Windows.Forms.TextBox txtVATRegistrationNo;
        private System.Windows.Forms.TextBox txtTINNo;
        private System.Windows.Forms.TextBox txtContactPersonEmail;
        private System.Windows.Forms.TextBox txtContactPersonTelephone;
        private System.Windows.Forms.TextBox txtContactPersonDesignation;
        private System.Windows.Forms.TextBox txtContactPerson;
        private System.Windows.Forms.TextBox txtEmail;
        private System.Windows.Forms.TextBox txtFaxNo;
        private System.Windows.Forms.TextBox txtTelephoneNo;
        private System.Windows.Forms.TextBox txtCity;
        private System.Windows.Forms.TextBox txtCustomerName;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtCustomerCode;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker dtpDateTo;
        private System.Windows.Forms.DateTimePicker dtpDateFrom;
        private System.Windows.Forms.TextBox txtCustomerGroupID;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txtCustomerGroupName;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridView dgvCustomer;
        public System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.ComboBox cmbType;
        private System.Windows.Forms.Label label18;
        private System.ComponentModel.BackgroundWorker backgroundWorkerSearch;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label LRecordCount;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.ComboBox cmbActive;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Label label19;
        public System.Windows.Forms.ComboBox cmbBranch;
        private System.Windows.Forms.ComboBox cmbImport;
        private System.Windows.Forms.CheckBox chkSelectAll;
        public System.Windows.Forms.ComboBox cmbRecordCount;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Select;
    }
}