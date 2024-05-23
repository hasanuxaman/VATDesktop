namespace VATClient
{
    partial class FormVendorSearch
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
            this.grbVendor = new System.Windows.Forms.GroupBox();
            this.cmbRecordCount = new System.Windows.Forms.ComboBox();
            this.chkSelectAll = new System.Windows.Forms.CheckBox();
            this.cmbImport = new System.Windows.Forms.ComboBox();
            this.label18 = new System.Windows.Forms.Label();
            this.cmbBranch = new System.Windows.Forms.ComboBox();
            this.label16 = new System.Windows.Forms.Label();
            this.cmbActive = new System.Windows.Forms.ComboBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.txtVendorGroupName = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.dtpStartDateTo = new System.Windows.Forms.DateTimePicker();
            this.dtpStartDateFrom = new System.Windows.Forms.DateTimePicker();
            this.btnSearch = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.txtVATRegistrationNo = new System.Windows.Forms.TextBox();
            this.txtTINNo = new System.Windows.Forms.TextBox();
            this.txtContactPerson = new System.Windows.Forms.TextBox();
            this.txtVendorName = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtVendorCode = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.LRecordCount = new System.Windows.Forms.Label();
            this.txtVendorGroupID = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtContactPersonEmail = new System.Windows.Forms.TextBox();
            this.txtContactPersonTelephone = new System.Windows.Forms.TextBox();
            this.txtContactPersonDesignation = new System.Windows.Forms.TextBox();
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.txtFaxNo = new System.Windows.Forms.TextBox();
            this.txtTelephoneNo = new System.Windows.Forms.TextBox();
            this.txtCity = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.dgvVendor = new System.Windows.Forms.DataGridView();
            this.Select = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.backgroundWorkerSearch = new System.ComponentModel.BackgroundWorker();
            this.label19 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.cmbType = new System.Windows.Forms.ComboBox();
            this.grbVendor.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvVendor)).BeginInit();
            this.SuspendLayout();
            // 
            // grbVendor
            // 
            this.grbVendor.Controls.Add(this.cmbType);
            this.grbVendor.Controls.Add(this.label20);
            this.grbVendor.Controls.Add(this.label19);
            this.grbVendor.Controls.Add(this.cmbRecordCount);
            this.grbVendor.Controls.Add(this.label18);
            this.grbVendor.Controls.Add(this.cmbBranch);
            this.grbVendor.Controls.Add(this.label16);
            this.grbVendor.Controls.Add(this.cmbActive);
            this.grbVendor.Controls.Add(this.btnCancel);
            this.grbVendor.Controls.Add(this.txtVendorGroupName);
            this.grbVendor.Controls.Add(this.label17);
            this.grbVendor.Controls.Add(this.label11);
            this.grbVendor.Controls.Add(this.dtpStartDateTo);
            this.grbVendor.Controls.Add(this.dtpStartDateFrom);
            this.grbVendor.Controls.Add(this.btnSearch);
            this.grbVendor.Controls.Add(this.label5);
            this.grbVendor.Controls.Add(this.txtVATRegistrationNo);
            this.grbVendor.Controls.Add(this.txtTINNo);
            this.grbVendor.Controls.Add(this.txtContactPerson);
            this.grbVendor.Controls.Add(this.txtVendorName);
            this.grbVendor.Controls.Add(this.label12);
            this.grbVendor.Controls.Add(this.label13);
            this.grbVendor.Controls.Add(this.label7);
            this.grbVendor.Controls.Add(this.label2);
            this.grbVendor.Controls.Add(this.txtVendorCode);
            this.grbVendor.Controls.Add(this.label1);
            this.grbVendor.Controls.Add(this.btnAdd);
            this.grbVendor.Location = new System.Drawing.Point(1, -7);
            this.grbVendor.Name = "grbVendor";
            this.grbVendor.Size = new System.Drawing.Size(758, 162);
            this.grbVendor.TabIndex = 3;
            this.grbVendor.TabStop = false;
            // 
            // cmbRecordCount
            // 
            this.cmbRecordCount.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRecordCount.FormattingEnabled = true;
            this.cmbRecordCount.Location = new System.Drawing.Point(121, 136);
            this.cmbRecordCount.Name = "cmbRecordCount";
            this.cmbRecordCount.Size = new System.Drawing.Size(80, 21);
            this.cmbRecordCount.TabIndex = 236;
            // 
            // chkSelectAll
            // 
            this.chkSelectAll.AutoSize = true;
            this.chkSelectAll.Location = new System.Drawing.Point(6, -1);
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
            this.cmbImport.Location = new System.Drawing.Point(296, 472);
            this.cmbImport.Name = "cmbImport";
            this.cmbImport.Size = new System.Drawing.Size(66, 21);
            this.cmbImport.TabIndex = 219;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(364, 92);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(40, 13);
            this.label18.TabIndex = 218;
            this.label18.Text = "Branch";
            // 
            // cmbBranch
            // 
            this.cmbBranch.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBranch.FormattingEnabled = true;
            this.cmbBranch.Items.AddRange(new object[] {
            "Cash Payable",
            "Credit Payable",
            "Rebatable"});
            this.cmbBranch.Location = new System.Drawing.Point(474, 88);
            this.cmbBranch.Name = "cmbBranch";
            this.cmbBranch.Size = new System.Drawing.Size(200, 21);
            this.cmbBranch.Sorted = true;
            this.cmbBranch.TabIndex = 216;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(364, 116);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(37, 13);
            this.label16.TabIndex = 213;
            this.label16.Text = "Active";
            // 
            // cmbActive
            // 
            this.cmbActive.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbActive.FormattingEnabled = true;
            this.cmbActive.Items.AddRange(new object[] {
            "Y",
            "N"});
            this.cmbActive.Location = new System.Drawing.Point(474, 112);
            this.cmbActive.Name = "cmbActive";
            this.cmbActive.Size = new System.Drawing.Size(80, 21);
            this.cmbActive.TabIndex = 212;
            // 
            // btnOk
            // 
            this.btnOk.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnOk.Image = global::VATClient.Properties.Resources.Back;
            this.btnOk.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOk.Location = new System.Drawing.Point(665, 468);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 28);
            this.btnOk.TabIndex = 12;
            this.btnOk.Text = "&Ok";
            this.btnOk.UseVisualStyleBackColor = false;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnCancel.Image = global::VATClient.Properties.Resources.Referesh;
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.Location = new System.Drawing.Point(677, 40);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 28);
            this.btnCancel.TabIndex = 11;
            this.btnCancel.Text = "&Refresh";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // txtVendorGroupName
            // 
            this.txtVendorGroupName.Location = new System.Drawing.Point(121, 64);
            this.txtVendorGroupName.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtVendorGroupName.MinimumSize = new System.Drawing.Size(185, 20);
            this.txtVendorGroupName.Name = "txtVendorGroupName";
            this.txtVendorGroupName.Size = new System.Drawing.Size(200, 20);
            this.txtVendorGroupName.TabIndex = 2;
            this.txtVendorGroupName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtVendorGroupName_KeyDown);
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(10, 68);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(103, 13);
            this.label17.TabIndex = 116;
            this.label17.Text = "Vendor Group Name";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(230, 116);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(17, 13);
            this.label11.TabIndex = 113;
            this.label11.Text = "to";
            // 
            // dtpStartDateTo
            // 
            this.dtpStartDateTo.Checked = false;
            this.dtpStartDateTo.CustomFormat = "dd/MMM/yyyy";
            this.dtpStartDateTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpStartDateTo.Location = new System.Drawing.Point(247, 112);
            this.dtpStartDateTo.Name = "dtpStartDateTo";
            this.dtpStartDateTo.ShowCheckBox = true;
            this.dtpStartDateTo.Size = new System.Drawing.Size(103, 21);
            this.dtpStartDateTo.TabIndex = 4;
            this.dtpStartDateTo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dtpStartDateTo_KeyDown);
            // 
            // dtpStartDateFrom
            // 
            this.dtpStartDateFrom.Checked = false;
            this.dtpStartDateFrom.CustomFormat = "dd/MMM/yyyy";
            this.dtpStartDateFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpStartDateFrom.Location = new System.Drawing.Point(121, 112);
            this.dtpStartDateFrom.Name = "dtpStartDateFrom";
            this.dtpStartDateFrom.ShowCheckBox = true;
            this.dtpStartDateFrom.Size = new System.Drawing.Size(109, 21);
            this.dtpStartDateFrom.TabIndex = 3;
            this.dtpStartDateFrom.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dtpStartDateFrom_KeyDown);
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSearch.Image = global::VATClient.Properties.Resources.search;
            this.btnSearch.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSearch.Location = new System.Drawing.Point(677, 11);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 28);
            this.btnSearch.TabIndex = 10;
            this.btnSearch.Text = "&Search";
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(10, 116);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(57, 13);
            this.label5.TabIndex = 110;
            this.label5.Text = "Start Date";
            // 
            // txtVATRegistrationNo
            // 
            this.txtVATRegistrationNo.Location = new System.Drawing.Point(474, 64);
            this.txtVATRegistrationNo.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtVATRegistrationNo.MinimumSize = new System.Drawing.Size(185, 20);
            this.txtVATRegistrationNo.Name = "txtVATRegistrationNo";
            this.txtVATRegistrationNo.Size = new System.Drawing.Size(200, 20);
            this.txtVATRegistrationNo.TabIndex = 7;
            this.txtVATRegistrationNo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtVATRegistrationNo_KeyDown);
            // 
            // txtTINNo
            // 
            this.txtTINNo.Location = new System.Drawing.Point(474, 40);
            this.txtTINNo.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtTINNo.MinimumSize = new System.Drawing.Size(185, 20);
            this.txtTINNo.Name = "txtTINNo";
            this.txtTINNo.Size = new System.Drawing.Size(200, 20);
            this.txtTINNo.TabIndex = 6;
            this.txtTINNo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtTINNo_KeyDown);
            // 
            // txtContactPerson
            // 
            this.txtContactPerson.Location = new System.Drawing.Point(474, 15);
            this.txtContactPerson.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtContactPerson.MinimumSize = new System.Drawing.Size(185, 20);
            this.txtContactPerson.Name = "txtContactPerson";
            this.txtContactPerson.Size = new System.Drawing.Size(200, 20);
            this.txtContactPerson.TabIndex = 5;
            this.txtContactPerson.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtContactPerson_KeyDown);
            // 
            // txtVendorName
            // 
            this.txtVendorName.Location = new System.Drawing.Point(121, 40);
            this.txtVendorName.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtVendorName.MinimumSize = new System.Drawing.Size(185, 20);
            this.txtVendorName.Name = "txtVendorName";
            this.txtVendorName.Size = new System.Drawing.Size(200, 20);
            this.txtVendorName.TabIndex = 1;
            this.txtVendorName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtVendorName_KeyDown);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(364, 68);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(100, 13);
            this.label12.TabIndex = 14;
            this.label12.Text = "Vat Registration No";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(364, 44);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(24, 13);
            this.label13.TabIndex = 13;
            this.label13.Text = "TIN";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(364, 19);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(81, 13);
            this.label7.TabIndex = 9;
            this.label7.Text = "Contact Person";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Vendor Name";
            // 
            // txtVendorCode
            // 
            this.txtVendorCode.Location = new System.Drawing.Point(121, 15);
            this.txtVendorCode.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtVendorCode.MinimumSize = new System.Drawing.Size(185, 20);
            this.txtVendorCode.Name = "txtVendorCode";
            this.txtVendorCode.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.txtVendorCode.Size = new System.Drawing.Size(200, 20);
            this.txtVendorCode.TabIndex = 0;
            this.txtVendorCode.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtVendorCode.TextChanged += new System.EventHandler(this.txtVendorID_TextChanged);
            this.txtVendorCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtVendorID_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Vendor Code";
            // 
            // btnExport
            // 
            this.btnExport.BackColor = System.Drawing.Color.White;
            this.btnExport.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnExport.Image = global::VATClient.Properties.Resources.Load;
            this.btnExport.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnExport.Location = new System.Drawing.Point(374, 468);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(75, 28);
            this.btnExport.TabIndex = 214;
            this.btnExport.Text = "Export";
            this.btnExport.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnExport.UseVisualStyleBackColor = false;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnAdd.Image = global::VATClient.Properties.Resources.add_over;
            this.btnAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAdd.Location = new System.Drawing.Point(664, 128);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 28);
            this.btnAdd.TabIndex = 9;
            this.btnAdd.Text = "&Add";
            this.btnAdd.UseVisualStyleBackColor = false;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // LRecordCount
            // 
            this.LRecordCount.AutoSize = true;
            this.LRecordCount.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LRecordCount.Location = new System.Drawing.Point(11, 474);
            this.LRecordCount.Name = "LRecordCount";
            this.LRecordCount.Size = new System.Drawing.Size(94, 16);
            this.LRecordCount.TabIndex = 211;
            this.LRecordCount.Text = "Record Count :";
            // 
            // txtVendorGroupID
            // 
            this.txtVendorGroupID.Location = new System.Drawing.Point(806, 259);
            this.txtVendorGroupID.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtVendorGroupID.MinimumSize = new System.Drawing.Size(185, 20);
            this.txtVendorGroupID.Name = "txtVendorGroupID";
            this.txtVendorGroupID.Size = new System.Drawing.Size(185, 21);
            this.txtVendorGroupID.TabIndex = 18;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(826, 292);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(91, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Vendor Group ID:";
            // 
            // txtContactPersonEmail
            // 
            this.txtContactPersonEmail.Location = new System.Drawing.Point(909, 232);
            this.txtContactPersonEmail.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtContactPersonEmail.MinimumSize = new System.Drawing.Size(85, 20);
            this.txtContactPersonEmail.Name = "txtContactPersonEmail";
            this.txtContactPersonEmail.Size = new System.Drawing.Size(85, 21);
            this.txtContactPersonEmail.TabIndex = 27;
            // 
            // txtContactPersonTelephone
            // 
            this.txtContactPersonTelephone.Location = new System.Drawing.Point(909, 208);
            this.txtContactPersonTelephone.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtContactPersonTelephone.MinimumSize = new System.Drawing.Size(85, 20);
            this.txtContactPersonTelephone.Name = "txtContactPersonTelephone";
            this.txtContactPersonTelephone.Size = new System.Drawing.Size(85, 21);
            this.txtContactPersonTelephone.TabIndex = 26;
            // 
            // txtContactPersonDesignation
            // 
            this.txtContactPersonDesignation.Location = new System.Drawing.Point(909, 182);
            this.txtContactPersonDesignation.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtContactPersonDesignation.MinimumSize = new System.Drawing.Size(85, 20);
            this.txtContactPersonDesignation.Name = "txtContactPersonDesignation";
            this.txtContactPersonDesignation.Size = new System.Drawing.Size(85, 21);
            this.txtContactPersonDesignation.TabIndex = 25;
            // 
            // txtEmail
            // 
            this.txtEmail.Location = new System.Drawing.Point(909, 156);
            this.txtEmail.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtEmail.MinimumSize = new System.Drawing.Size(85, 20);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(85, 21);
            this.txtEmail.TabIndex = 23;
            // 
            // txtFaxNo
            // 
            this.txtFaxNo.Location = new System.Drawing.Point(909, 132);
            this.txtFaxNo.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtFaxNo.MinimumSize = new System.Drawing.Size(85, 20);
            this.txtFaxNo.Name = "txtFaxNo";
            this.txtFaxNo.Size = new System.Drawing.Size(85, 21);
            this.txtFaxNo.TabIndex = 22;
            // 
            // txtTelephoneNo
            // 
            this.txtTelephoneNo.Location = new System.Drawing.Point(909, 108);
            this.txtTelephoneNo.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtTelephoneNo.MinimumSize = new System.Drawing.Size(85, 20);
            this.txtTelephoneNo.Name = "txtTelephoneNo";
            this.txtTelephoneNo.Size = new System.Drawing.Size(85, 21);
            this.txtTelephoneNo.TabIndex = 21;
            // 
            // txtCity
            // 
            this.txtCity.Location = new System.Drawing.Point(909, 84);
            this.txtCity.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtCity.MinimumSize = new System.Drawing.Size(85, 20);
            this.txtCity.Name = "txtCity";
            this.txtCity.Size = new System.Drawing.Size(85, 21);
            this.txtCity.TabIndex = 19;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(826, 231);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(35, 13);
            this.label14.TabIndex = 12;
            this.label14.Text = "Email:";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(826, 210);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(61, 13);
            this.label15.TabIndex = 11;
            this.label15.Text = "Telephone:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(826, 187);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(67, 13);
            this.label6.TabIndex = 10;
            this.label6.Text = "Designation:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(826, 161);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(35, 13);
            this.label8.TabIndex = 8;
            this.label8.Text = "Email:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(826, 137);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(45, 13);
            this.label9.TabIndex = 7;
            this.label9.Text = "Fax No:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(826, 113);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(77, 13);
            this.label10.TabIndex = 6;
            this.label10.Text = "Telephone No:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(826, 91);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(30, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "City:";
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.groupBox1.Controls.Add(this.progressBar1);
            this.groupBox1.Controls.Add(this.chkSelectAll);
            this.groupBox1.Controls.Add(this.dgvVendor);
            this.groupBox1.Location = new System.Drawing.Point(-1, 160);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(743, 304);
            this.groupBox1.TabIndex = 106;
            this.groupBox1.TabStop = false;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(173, 142);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(418, 24);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 107;
            this.progressBar1.Visible = false;
            // 
            // dgvVendor
            // 
            this.dgvVendor.AllowUserToAddRows = false;
            this.dgvVendor.AllowUserToDeleteRows = false;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.dgvVendor.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvVendor.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvVendor.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Select});
            this.dgvVendor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvVendor.Location = new System.Drawing.Point(3, 17);
            this.dgvVendor.Name = "dgvVendor";
            this.dgvVendor.RowHeadersVisible = false;
            this.dgvVendor.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvVendor.Size = new System.Drawing.Size(737, 284);
            this.dgvVendor.TabIndex = 6;
            this.dgvVendor.TabStop = false;
            this.dgvVendor.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvVendor_CellContentClick);
            this.dgvVendor.DoubleClick += new System.EventHandler(this.dgvVendor_DoubleClick);
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
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(10, 92);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(68, 13);
            this.label19.TabIndex = 238;
            this.label19.Text = "Vendor Type";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(10, 139);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(25, 13);
            this.label20.TabIndex = 239;
            this.label20.Text = "Top";
            // 
            // cmbType
            // 
            this.cmbType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbType.FormattingEnabled = true;
            this.cmbType.Items.AddRange(new object[] {
            "Local",
            "Import"});
            this.cmbType.Location = new System.Drawing.Point(121, 88);
            this.cmbType.Name = "cmbType";
            this.cmbType.Size = new System.Drawing.Size(200, 21);
            this.cmbType.TabIndex = 240;
            // 
            // FormVendorSearch
            // 
            this.AcceptButton = this.btnSearch;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.CancelButton = this.btnOk;
            this.ClientSize = new System.Drawing.Size(759, 498);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.grbVendor);
            this.Controls.Add(this.cmbImport);
            this.Controls.Add(this.txtCity);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.txtContactPersonEmail);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.LRecordCount);
            this.Controls.Add(this.txtContactPersonTelephone);
            this.Controls.Add(this.txtVendorGroupID);
            this.Controls.Add(this.txtTelephoneNo);
            this.Controls.Add(this.txtContactPersonDesignation);
            this.Controls.Add(this.txtFaxNo);
            this.Controls.Add(this.txtEmail);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.btnExport);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(775, 550);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(760, 500);
            this.Name = "FormVendorSearch";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Vendor Search";
            this.Load += new System.EventHandler(this.FormVendorSearch_Load);
            this.grbVendor.ResumeLayout(false);
            this.grbVendor.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvVendor)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.GroupBox grbVendor;
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
        private System.Windows.Forms.TextBox txtVendorGroupID;
        private System.Windows.Forms.TextBox txtVendorName;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
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
        private System.Windows.Forms.TextBox txtVendorCode;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.DateTimePicker dtpStartDateTo;
        private System.Windows.Forms.DateTimePicker dtpStartDateFrom;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridView dgvVendor;
        private System.Windows.Forms.TextBox txtVendorGroupName;
        private System.Windows.Forms.Label label17;
        public System.Windows.Forms.Button btnAdd;
        private System.ComponentModel.BackgroundWorker backgroundWorkerSearch;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label LRecordCount;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.ComboBox cmbActive;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Label label18;
        public System.Windows.Forms.ComboBox cmbBranch;
        private System.Windows.Forms.ComboBox cmbImport;
        private System.Windows.Forms.CheckBox chkSelectAll;
        public System.Windows.Forms.ComboBox cmbRecordCount;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Select;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.ComboBox cmbType;
    }
}