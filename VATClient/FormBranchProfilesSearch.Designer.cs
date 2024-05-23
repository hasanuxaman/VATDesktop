namespace VATClient
{
    partial class FormBranchProfilesSearch
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.dgvCustomer = new System.Windows.Forms.DataGridView();
            this.Select = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.BranchID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CustomerID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ActiveStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BranchCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BranchName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.City = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TelephoneNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FaxNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Email = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ContactPerson = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ContactPersonDesignation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ContactPersonTelephone = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ContactPersonEmail = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Comments = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TINNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.VatRegistrationNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.IsArchive = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BranchLegalName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Address = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ZipCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BIN = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkSelectAll = new System.Windows.Forms.CheckBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtBranchCode = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.txtBranchName = new System.Windows.Forms.TextBox();
            this.txtContactPerson = new System.Windows.Forms.TextBox();
            this.txtTINNo = new System.Windows.Forms.TextBox();
            this.txtVATRegistrationNo = new System.Windows.Forms.TextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.LRecordCount = new System.Windows.Forms.Label();
            this.grbCustomer = new System.Windows.Forms.GroupBox();
            this.btnExport = new System.Windows.Forms.Button();
            this.textBIN = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.backgroundWorkerSearch = new System.ComponentModel.BackgroundWorker();
            this.btnPrint = new System.Windows.Forms.Button();
            this.backgroundWorkerPreview = new System.ComponentModel.BackgroundWorker();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCustomer)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.grbCustomer.SuspendLayout();
            this.SuspendLayout();
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(245, 126);
            this.progressBar1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(420, 28);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 106;
            this.progressBar1.Visible = false;
            // 
            // dgvCustomer
            // 
            this.dgvCustomer.AllowUserToAddRows = false;
            this.dgvCustomer.AllowUserToDeleteRows = false;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.dgvCustomer.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle4;
            this.dgvCustomer.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCustomer.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Select,
            this.BranchID,
            this.CustomerID,
            this.ActiveStatus,
            this.BranchCode,
            this.BranchName,
            this.City,
            this.TelephoneNo,
            this.FaxNo,
            this.Email,
            this.ContactPerson,
            this.ContactPersonDesignation,
            this.ContactPersonTelephone,
            this.ContactPersonEmail,
            this.Comments,
            this.TINNo,
            this.VatRegistrationNo,
            this.IsArchive,
            this.BranchLegalName,
            this.Address,
            this.ZipCode,
            this.BIN});
            this.dgvCustomer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvCustomer.Location = new System.Drawing.Point(4, 19);
            this.dgvCustomer.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.dgvCustomer.Name = "dgvCustomer";
            this.dgvCustomer.RowHeadersVisible = false;
            this.dgvCustomer.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvCustomer.Size = new System.Drawing.Size(1044, 286);
            this.dgvCustomer.TabIndex = 6;
            this.dgvCustomer.DoubleClick += new System.EventHandler(this.dgvCustomer_DoubleClick);
            // 
            // Select
            // 
            this.Select.HeaderText = "Select";
            this.Select.Name = "Select";
            // 
            // BranchID
            // 
            this.BranchID.HeaderText = "BranchID";
            this.BranchID.Name = "BranchID";
            this.BranchID.Visible = false;
            // 
            // CustomerID
            // 
            this.CustomerID.HeaderText = "ID";
            this.CustomerID.Name = "CustomerID";
            this.CustomerID.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.CustomerID.Visible = false;
            // 
            // ActiveStatus
            // 
            this.ActiveStatus.HeaderText = "ActiveStatus";
            this.ActiveStatus.Name = "ActiveStatus";
            this.ActiveStatus.Visible = false;
            // 
            // BranchCode
            // 
            this.BranchCode.HeaderText = "BranchCode";
            this.BranchCode.Name = "BranchCode";
            // 
            // BranchName
            // 
            this.BranchName.HeaderText = "BranchName";
            this.BranchName.Name = "BranchName";
            // 
            // City
            // 
            this.City.HeaderText = "City";
            this.City.Name = "City";
            // 
            // TelephoneNo
            // 
            this.TelephoneNo.HeaderText = "Telephone";
            this.TelephoneNo.Name = "TelephoneNo";
            // 
            // FaxNo
            // 
            this.FaxNo.HeaderText = "FaxNo";
            this.FaxNo.Name = "FaxNo";
            // 
            // Email
            // 
            this.Email.HeaderText = "Email";
            this.Email.Name = "Email";
            // 
            // ContactPerson
            // 
            this.ContactPerson.HeaderText = "Contact Person";
            this.ContactPerson.Name = "ContactPerson";
            this.ContactPerson.Width = 125;
            // 
            // ContactPersonDesignation
            // 
            this.ContactPersonDesignation.HeaderText = "C Person Designation";
            this.ContactPersonDesignation.Name = "ContactPersonDesignation";
            this.ContactPersonDesignation.Visible = false;
            this.ContactPersonDesignation.Width = 5;
            // 
            // ContactPersonTelephone
            // 
            this.ContactPersonTelephone.HeaderText = "C Person Telephone";
            this.ContactPersonTelephone.Name = "ContactPersonTelephone";
            this.ContactPersonTelephone.Visible = false;
            this.ContactPersonTelephone.Width = 5;
            // 
            // ContactPersonEmail
            // 
            this.ContactPersonEmail.HeaderText = "C Person Email";
            this.ContactPersonEmail.Name = "ContactPersonEmail";
            this.ContactPersonEmail.Visible = false;
            this.ContactPersonEmail.Width = 5;
            // 
            // Comments
            // 
            this.Comments.HeaderText = "Comments";
            this.Comments.Name = "Comments";
            this.Comments.Visible = false;
            // 
            // TINNo
            // 
            this.TINNo.HeaderText = "TIN";
            this.TINNo.Name = "TINNo";
            // 
            // VatRegistrationNo
            // 
            this.VatRegistrationNo.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.VatRegistrationNo.HeaderText = "VAT Reg No";
            this.VatRegistrationNo.Name = "VatRegistrationNo";
            this.VatRegistrationNo.Visible = false;
            // 
            // IsArchive
            // 
            this.IsArchive.HeaderText = "IsArchive";
            this.IsArchive.Name = "IsArchive";
            // 
            // BranchLegalName
            // 
            this.BranchLegalName.HeaderText = "BranchLegalName";
            this.BranchLegalName.Name = "BranchLegalName";
            // 
            // Address
            // 
            this.Address.HeaderText = "Address";
            this.Address.Name = "Address";
            // 
            // ZipCode
            // 
            this.ZipCode.HeaderText = "ZipCode";
            this.ZipCode.Name = "ZipCode";
            // 
            // BIN
            // 
            this.BIN.HeaderText = "BIN";
            this.BIN.Name = "BIN";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkSelectAll);
            this.groupBox1.Controls.Add(this.progressBar1);
            this.groupBox1.Controls.Add(this.dgvCustomer);
            this.groupBox1.Controls.Add(this.btnAdd);
            this.groupBox1.Location = new System.Drawing.Point(16, 215);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Size = new System.Drawing.Size(1052, 309);
            this.groupBox1.TabIndex = 106;
            this.groupBox1.TabStop = false;
            // 
            // chkSelectAll
            // 
            this.chkSelectAll.AutoSize = true;
            this.chkSelectAll.Location = new System.Drawing.Point(7, -1);
            this.chkSelectAll.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkSelectAll.Name = "chkSelectAll";
            this.chkSelectAll.Size = new System.Drawing.Size(84, 21);
            this.chkSelectAll.TabIndex = 203;
            this.chkSelectAll.Text = "SelectAll";
            this.chkSelectAll.UseVisualStyleBackColor = true;
            this.chkSelectAll.CheckedChanged += new System.EventHandler(this.chkSelectAll_CheckedChanged);
            this.chkSelectAll.Click += new System.EventHandler(this.chkSelectAll_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnAdd.Image = global::VATClient.Properties.Resources.add_over;
            this.btnAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAdd.Location = new System.Drawing.Point(843, 92);
            this.btnAdd.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(100, 34);
            this.btnAdd.TabIndex = 108;
            this.btnAdd.Text = "&Add";
            this.btnAdd.UseVisualStyleBackColor = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(35, 28);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(90, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Branch Code";
            // 
            // txtBranchCode
            // 
            this.txtBranchCode.Location = new System.Drawing.Point(196, 23);
            this.txtBranchCode.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtBranchCode.MaximumSize = new System.Drawing.Size(265, 20);
            this.txtBranchCode.MinimumSize = new System.Drawing.Size(245, 20);
            this.txtBranchCode.Name = "txtBranchCode";
            this.txtBranchCode.Size = new System.Drawing.Size(245, 22);
            this.txtBranchCode.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(35, 55);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(94, 17);
            this.label2.TabIndex = 2;
            this.label2.Text = "Branch Name";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(475, 25);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(105, 17);
            this.label7.TabIndex = 9;
            this.label7.Text = "Contact Person";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(475, 52);
            this.label13.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(30, 17);
            this.label13.TabIndex = 13;
            this.label13.Text = "TIN";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(475, 81);
            this.label12.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(131, 17);
            this.label12.TabIndex = 14;
            this.label12.Text = "Vat Registration No";
            // 
            // txtBranchName
            // 
            this.txtBranchName.Location = new System.Drawing.Point(196, 52);
            this.txtBranchName.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtBranchName.MaximumSize = new System.Drawing.Size(265, 20);
            this.txtBranchName.MinimumSize = new System.Drawing.Size(245, 20);
            this.txtBranchName.Name = "txtBranchName";
            this.txtBranchName.Size = new System.Drawing.Size(245, 22);
            this.txtBranchName.TabIndex = 1;
            // 
            // txtContactPerson
            // 
            this.txtContactPerson.Location = new System.Drawing.Point(620, 20);
            this.txtContactPerson.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtContactPerson.MaximumSize = new System.Drawing.Size(265, 20);
            this.txtContactPerson.MinimumSize = new System.Drawing.Size(245, 20);
            this.txtContactPerson.Name = "txtContactPerson";
            this.txtContactPerson.Size = new System.Drawing.Size(245, 22);
            this.txtContactPerson.TabIndex = 7;
            // 
            // txtTINNo
            // 
            this.txtTINNo.Location = new System.Drawing.Point(620, 47);
            this.txtTINNo.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtTINNo.MaximumSize = new System.Drawing.Size(265, 20);
            this.txtTINNo.MinimumSize = new System.Drawing.Size(245, 20);
            this.txtTINNo.Name = "txtTINNo";
            this.txtTINNo.Size = new System.Drawing.Size(245, 22);
            this.txtTINNo.TabIndex = 8;
            // 
            // txtVATRegistrationNo
            // 
            this.txtVATRegistrationNo.Location = new System.Drawing.Point(620, 79);
            this.txtVATRegistrationNo.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtVATRegistrationNo.MaximumSize = new System.Drawing.Size(265, 20);
            this.txtVATRegistrationNo.MinimumSize = new System.Drawing.Size(245, 20);
            this.txtVATRegistrationNo.Name = "txtVATRegistrationNo";
            this.txtVATRegistrationNo.Size = new System.Drawing.Size(245, 22);
            this.txtVATRegistrationNo.TabIndex = 9;
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSearch.Image = global::VATClient.Properties.Resources.search;
            this.btnSearch.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSearch.Location = new System.Drawing.Point(895, 17);
            this.btnSearch.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(100, 34);
            this.btnSearch.TabIndex = 11;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnCancel.Image = global::VATClient.Properties.Resources.Referesh;
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.Location = new System.Drawing.Point(895, 53);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 34);
            this.btnCancel.TabIndex = 12;
            this.btnCancel.Text = "&Refresh";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOk
            // 
            this.btnOk.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnOk.Image = global::VATClient.Properties.Resources.Back;
            this.btnOk.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOk.Location = new System.Drawing.Point(895, 87);
            this.btnOk.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(100, 34);
            this.btnOk.TabIndex = 13;
            this.btnOk.Text = "&Ok";
            this.btnOk.UseVisualStyleBackColor = false;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // LRecordCount
            // 
            this.LRecordCount.AutoSize = true;
            this.LRecordCount.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LRecordCount.Location = new System.Drawing.Point(307, 144);
            this.LRecordCount.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LRecordCount.Name = "LRecordCount";
            this.LRecordCount.Size = new System.Drawing.Size(121, 21);
            this.LRecordCount.TabIndex = 225;
            this.LRecordCount.Text = "Record Count :";
            // 
            // grbCustomer
            // 
            this.grbCustomer.Controls.Add(this.btnPrint);
            this.grbCustomer.Controls.Add(this.btnExport);
            this.grbCustomer.Controls.Add(this.textBIN);
            this.grbCustomer.Controls.Add(this.label3);
            this.grbCustomer.Controls.Add(this.LRecordCount);
            this.grbCustomer.Controls.Add(this.btnOk);
            this.grbCustomer.Controls.Add(this.btnCancel);
            this.grbCustomer.Controls.Add(this.btnSearch);
            this.grbCustomer.Controls.Add(this.txtVATRegistrationNo);
            this.grbCustomer.Controls.Add(this.txtTINNo);
            this.grbCustomer.Controls.Add(this.txtContactPerson);
            this.grbCustomer.Controls.Add(this.txtBranchName);
            this.grbCustomer.Controls.Add(this.label12);
            this.grbCustomer.Controls.Add(this.label13);
            this.grbCustomer.Controls.Add(this.label7);
            this.grbCustomer.Controls.Add(this.label2);
            this.grbCustomer.Controls.Add(this.txtBranchCode);
            this.grbCustomer.Controls.Add(this.label1);
            this.grbCustomer.Location = new System.Drawing.Point(20, 16);
            this.grbCustomer.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.grbCustomer.Name = "grbCustomer";
            this.grbCustomer.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.grbCustomer.Size = new System.Drawing.Size(1044, 192);
            this.grbCustomer.TabIndex = 109;
            this.grbCustomer.TabStop = false;
            this.grbCustomer.Text = "Searchig Criteria";
            // 
            // btnExport
            // 
            this.btnExport.BackColor = System.Drawing.Color.White;
            this.btnExport.Image = global::VATClient.Properties.Resources.Load;
            this.btnExport.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnExport.Location = new System.Drawing.Point(895, 129);
            this.btnExport.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(100, 34);
            this.btnExport.TabIndex = 227;
            this.btnExport.Text = "Export";
            this.btnExport.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnExport.UseVisualStyleBackColor = false;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // textBIN
            // 
            this.textBIN.Location = new System.Drawing.Point(196, 79);
            this.textBIN.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textBIN.MaximumSize = new System.Drawing.Size(265, 20);
            this.textBIN.MinimumSize = new System.Drawing.Size(245, 20);
            this.textBIN.Name = "textBIN";
            this.textBIN.Size = new System.Drawing.Size(245, 22);
            this.textBIN.TabIndex = 110;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(35, 82);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(30, 17);
            this.label3.TabIndex = 111;
            this.label3.Text = "BIN";
            // 
            // backgroundWorkerSearch
            // 
            this.backgroundWorkerSearch.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerSearch_DoWork);
            this.backgroundWorkerSearch.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerSearch_RunWorkerCompleted);
            // 
            // btnPrint
            // 
            this.btnPrint.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnPrint.Image = global::VATClient.Properties.Resources.Print;
            this.btnPrint.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPrint.Location = new System.Drawing.Point(781, 129);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(100, 34);
            this.btnPrint.TabIndex = 228;
            this.btnPrint.Text = "&Print";
            this.btnPrint.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnPrint.UseVisualStyleBackColor = false;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // backgroundWorkerPreview
            // 
            this.backgroundWorkerPreview.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerPreview_DoWork);
            this.backgroundWorkerPreview.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerPreview_RunWorkerCompleted);
            // 
            // FormBranchProfilesSearch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(1077, 529);
            this.Controls.Add(this.grbCustomer);
            this.Controls.Add(this.groupBox1);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "FormBranchProfilesSearch";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FromBranchProfilesSearch";
            this.Load += new System.EventHandler(this.FromBranchProfilesSearch_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvCustomer)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.grbCustomer.ResumeLayout(false);
            this.grbCustomer.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.DataGridView dgvCustomer;
        private System.Windows.Forms.GroupBox groupBox1;
        public System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtBranchCode;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox txtBranchName;
        private System.Windows.Forms.TextBox txtContactPerson;
        private System.Windows.Forms.TextBox txtTINNo;
        private System.Windows.Forms.TextBox txtVATRegistrationNo;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Label LRecordCount;
        private System.Windows.Forms.GroupBox grbCustomer;
        private System.Windows.Forms.TextBox textBIN;
        private System.Windows.Forms.Label label3;
        private System.ComponentModel.BackgroundWorker backgroundWorkerSearch;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.CheckBox chkSelectAll;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Select;
        private System.Windows.Forms.DataGridViewTextBoxColumn BranchID;
        private System.Windows.Forms.DataGridViewTextBoxColumn CustomerID;
        private System.Windows.Forms.DataGridViewTextBoxColumn ActiveStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn BranchCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn BranchName;
        private System.Windows.Forms.DataGridViewTextBoxColumn City;
        private System.Windows.Forms.DataGridViewTextBoxColumn TelephoneNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn FaxNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn Email;
        private System.Windows.Forms.DataGridViewTextBoxColumn ContactPerson;
        private System.Windows.Forms.DataGridViewTextBoxColumn ContactPersonDesignation;
        private System.Windows.Forms.DataGridViewTextBoxColumn ContactPersonTelephone;
        private System.Windows.Forms.DataGridViewTextBoxColumn ContactPersonEmail;
        private System.Windows.Forms.DataGridViewTextBoxColumn Comments;
        private System.Windows.Forms.DataGridViewTextBoxColumn TINNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn VatRegistrationNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn IsArchive;
        private System.Windows.Forms.DataGridViewTextBoxColumn BranchLegalName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Address;
        private System.Windows.Forms.DataGridViewTextBoxColumn ZipCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn BIN;
        private System.Windows.Forms.Button btnPrint;
        private System.ComponentModel.BackgroundWorker backgroundWorkerPreview;
    }
}