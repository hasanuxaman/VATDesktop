namespace VATClient
{
    partial class FormBOMSearch
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
            this.dgvBOM = new System.Windows.Forms.DataGridView();
            this.Select = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.label3 = new System.Windows.Forms.Label();
            this.txtProductName = new System.Windows.Forms.TextBox();
            this.txtItemNo = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkSelect = new System.Windows.Forms.CheckBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.pnlHidden = new System.Windows.Forms.Panel();
            this.button2 = new System.Windows.Forms.Button();
            this.grbVendorGroup = new System.Windows.Forms.GroupBox();
            this.dtpEffectDateTo = new System.Windows.Forms.DateTimePicker();
            this.label6 = new System.Windows.Forms.Label();
            this.cmbProductType = new System.Windows.Forms.ComboBox();
            this.label22 = new System.Windows.Forms.Label();
            this.btnPost = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.cmbRecordCount = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cmbBranch = new System.Windows.Forms.ComboBox();
            this.btnCustomerRefresh = new System.Windows.Forms.Button();
            this.txtCustomer = new System.Windows.Forms.TextBox();
            this.label37 = new System.Windows.Forms.Label();
            this.cmbVAT1Name = new System.Windows.Forms.ComboBox();
            this.txtCode = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbPost = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label30 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.dtpEffectDate = new System.Windows.Forms.DateTimePicker();
            this.btnSearch = new System.Windows.Forms.Button();
            this.btnExport = new System.Windows.Forms.Button();
            this.bgwSearch = new System.ComponentModel.BackgroundWorker();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnCompare = new System.Windows.Forms.Button();
            this.LRecordCount = new System.Windows.Forms.Label();
            this.btnOk = new System.Windows.Forms.Button();
            this.bgwMultiplePost = new System.ComponentModel.BackgroundWorker();
            this.bgwDownload = new System.ComponentModel.BackgroundWorker();
            this.chkOverhead = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBOM)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.pnlHidden.SuspendLayout();
            this.grbVendorGroup.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvBOM
            // 
            this.dgvBOM.AllowUserToAddRows = false;
            this.dgvBOM.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.dgvBOM.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvBOM.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvBOM.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Select});
            this.dgvBOM.Location = new System.Drawing.Point(7, 26);
            this.dgvBOM.Margin = new System.Windows.Forms.Padding(4);
            this.dgvBOM.Name = "dgvBOM";
            this.dgvBOM.RowHeadersVisible = false;
            this.dgvBOM.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvBOM.Size = new System.Drawing.Size(933, 234);
            this.dgvBOM.TabIndex = 18;
            this.dgvBOM.TabStop = false;
            this.dgvBOM.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvVendorGroup_CellContentClick);
            this.dgvBOM.DoubleClick += new System.EventHandler(this.dgvBOM_DoubleClick);
            // 
            // Select
            // 
            this.Select.HeaderText = "Select";
            this.Select.Name = "Select";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(444, 52);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(120, 17);
            this.label3.TabIndex = 33;
            this.label3.Text = "Effect Date(From)";
            // 
            // txtProductName
            // 
            this.txtProductName.Location = new System.Drawing.Point(119, 17);
            this.txtProductName.Margin = new System.Windows.Forms.Padding(4);
            this.txtProductName.Name = "txtProductName";
            this.txtProductName.Size = new System.Drawing.Size(265, 22);
            this.txtProductName.TabIndex = 0;
            // 
            // txtItemNo
            // 
            this.txtItemNo.Location = new System.Drawing.Point(4, 10);
            this.txtItemNo.Margin = new System.Windows.Forms.Padding(4);
            this.txtItemNo.MaximumSize = new System.Drawing.Size(265, 20);
            this.txtItemNo.MinimumSize = new System.Drawing.Size(65, 20);
            this.txtItemNo.Name = "txtItemNo";
            this.txtItemNo.Size = new System.Drawing.Size(65, 22);
            this.txtItemNo.TabIndex = 3;
            this.txtItemNo.Visible = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 22);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(98, 17);
            this.label2.TabIndex = 2;
            this.label2.Text = "Product Name";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkSelect);
            this.groupBox1.Controls.Add(this.progressBar1);
            this.groupBox1.Controls.Add(this.pnlHidden);
            this.groupBox1.Controls.Add(this.dgvBOM);
            this.groupBox1.Location = new System.Drawing.Point(5, 183);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(948, 265);
            this.groupBox1.TabIndex = 110;
            this.groupBox1.TabStop = false;
            // 
            // chkSelect
            // 
            this.chkSelect.AutoSize = true;
            this.chkSelect.Location = new System.Drawing.Point(7, -1);
            this.chkSelect.Margin = new System.Windows.Forms.Padding(4);
            this.chkSelect.Name = "chkSelect";
            this.chkSelect.Size = new System.Drawing.Size(88, 21);
            this.chkSelect.TabIndex = 239;
            this.chkSelect.Text = "Select All";
            this.chkSelect.UseVisualStyleBackColor = true;
            this.chkSelect.CheckedChanged += new System.EventHandler(this.chkSelect_CheckedChanged);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(325, 102);
            this.progressBar1.Margin = new System.Windows.Forms.Padding(4);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(387, 30);
            this.progressBar1.TabIndex = 207;
            this.progressBar1.Visible = false;
            // 
            // pnlHidden
            // 
            this.pnlHidden.Controls.Add(this.button2);
            this.pnlHidden.Controls.Add(this.txtItemNo);
            this.pnlHidden.Location = new System.Drawing.Point(15, 102);
            this.pnlHidden.Margin = new System.Windows.Forms.Padding(4);
            this.pnlHidden.Name = "pnlHidden";
            this.pnlHidden.Size = new System.Drawing.Size(267, 123);
            this.pnlHidden.TabIndex = 208;
            this.pnlHidden.Visible = false;
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.button2.Image = global::VATClient.Properties.Resources.search;
            this.button2.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button2.Location = new System.Drawing.Point(163, 4);
            this.button2.Margin = new System.Windows.Forms.Padding(4);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(100, 34);
            this.button2.TabIndex = 224;
            this.button2.Text = "&Search";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Visible = false;
            this.button2.Click += new System.EventHandler(this.button2_Click_1);
            // 
            // grbVendorGroup
            // 
            this.grbVendorGroup.Controls.Add(this.dtpEffectDateTo);
            this.grbVendorGroup.Controls.Add(this.label6);
            this.grbVendorGroup.Controls.Add(this.cmbProductType);
            this.grbVendorGroup.Controls.Add(this.label22);
            this.grbVendorGroup.Controls.Add(this.btnPost);
            this.grbVendorGroup.Controls.Add(this.btnCancel);
            this.grbVendorGroup.Controls.Add(this.label5);
            this.grbVendorGroup.Controls.Add(this.cmbRecordCount);
            this.grbVendorGroup.Controls.Add(this.label4);
            this.grbVendorGroup.Controls.Add(this.cmbBranch);
            this.grbVendorGroup.Controls.Add(this.btnCustomerRefresh);
            this.grbVendorGroup.Controls.Add(this.txtCustomer);
            this.grbVendorGroup.Controls.Add(this.label37);
            this.grbVendorGroup.Controls.Add(this.cmbVAT1Name);
            this.grbVendorGroup.Controls.Add(this.txtCode);
            this.grbVendorGroup.Controls.Add(this.label1);
            this.grbVendorGroup.Controls.Add(this.cmbPost);
            this.grbVendorGroup.Controls.Add(this.label9);
            this.grbVendorGroup.Controls.Add(this.label30);
            this.grbVendorGroup.Controls.Add(this.button1);
            this.grbVendorGroup.Controls.Add(this.dtpEffectDate);
            this.grbVendorGroup.Controls.Add(this.label3);
            this.grbVendorGroup.Controls.Add(this.txtProductName);
            this.grbVendorGroup.Controls.Add(this.btnSearch);
            this.grbVendorGroup.Controls.Add(this.label2);
            this.grbVendorGroup.Location = new System.Drawing.Point(3, 5);
            this.grbVendorGroup.Margin = new System.Windows.Forms.Padding(4);
            this.grbVendorGroup.Name = "grbVendorGroup";
            this.grbVendorGroup.Padding = new System.Windows.Forms.Padding(4);
            this.grbVendorGroup.Size = new System.Drawing.Size(951, 172);
            this.grbVendorGroup.TabIndex = 107;
            this.grbVendorGroup.TabStop = false;
            this.grbVendorGroup.Text = "Searching Criteria";
            // 
            // dtpEffectDateTo
            // 
            this.dtpEffectDateTo.Checked = false;
            this.dtpEffectDateTo.CustomFormat = "dd/MMM/yyyy";
            this.dtpEffectDateTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpEffectDateTo.Location = new System.Drawing.Point(564, 78);
            this.dtpEffectDateTo.Margin = new System.Windows.Forms.Padding(4);
            this.dtpEffectDateTo.Name = "dtpEffectDateTo";
            this.dtpEffectDateTo.ShowCheckBox = true;
            this.dtpEffectDateTo.Size = new System.Drawing.Size(145, 22);
            this.dtpEffectDateTo.TabIndex = 242;
            this.dtpEffectDateTo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dtpEffectDateTo_KeyDown);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(444, 82);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(105, 17);
            this.label6.TabIndex = 243;
            this.label6.Text = "Effect Date(To)";
            // 
            // cmbProductType
            // 
            this.cmbProductType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbProductType.FormattingEnabled = true;
            this.cmbProductType.Location = new System.Drawing.Point(119, 80);
            this.cmbProductType.Margin = new System.Windows.Forms.Padding(4);
            this.cmbProductType.Name = "cmbProductType";
            this.cmbProductType.Size = new System.Drawing.Size(265, 24);
            this.cmbProductType.TabIndex = 240;
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(8, 85);
            this.label22.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(93, 17);
            this.label22.TabIndex = 241;
            this.label22.Text = "Product Type";
            // 
            // btnPost
            // 
            this.btnPost.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnPost.Image = global::VATClient.Properties.Resources.Post;
            this.btnPost.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPost.Location = new System.Drawing.Point(847, 133);
            this.btnPost.Margin = new System.Windows.Forms.Padding(4);
            this.btnPost.Name = "btnPost";
            this.btnPost.Size = new System.Drawing.Size(100, 34);
            this.btnPost.TabIndex = 239;
            this.btnPost.Text = "Post";
            this.btnPost.UseVisualStyleBackColor = false;
            this.btnPost.Click += new System.EventHandler(this.btnPost_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Image = global::VATClient.Properties.Resources.Referesh;
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.Location = new System.Drawing.Point(847, 46);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 34);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "&Refresh";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(8, 144);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(33, 17);
            this.label5.TabIndex = 237;
            this.label5.Text = "Top";
            // 
            // cmbRecordCount
            // 
            this.cmbRecordCount.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRecordCount.FormattingEnabled = true;
            this.cmbRecordCount.Location = new System.Drawing.Point(119, 139);
            this.cmbRecordCount.Margin = new System.Windows.Forms.Padding(4);
            this.cmbRecordCount.Name = "cmbRecordCount";
            this.cmbRecordCount.Size = new System.Drawing.Size(105, 24);
            this.cmbRecordCount.TabIndex = 236;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(444, 143);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 17);
            this.label4.TabIndex = 223;
            this.label4.Text = "Branch";
            // 
            // cmbBranch
            // 
            this.cmbBranch.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBranch.FormattingEnabled = true;
            this.cmbBranch.Items.AddRange(new object[] {
            "Y",
            "N"});
            this.cmbBranch.Location = new System.Drawing.Point(564, 138);
            this.cmbBranch.Margin = new System.Windows.Forms.Padding(4);
            this.cmbBranch.Name = "cmbBranch";
            this.cmbBranch.Size = new System.Drawing.Size(265, 24);
            this.cmbBranch.TabIndex = 222;
            // 
            // btnCustomerRefresh
            // 
            this.btnCustomerRefresh.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnCustomerRefresh.Image = global::VATClient.Properties.Resources.Referesh;
            this.btnCustomerRefresh.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCustomerRefresh.Location = new System.Drawing.Point(392, 108);
            this.btnCustomerRefresh.Margin = new System.Windows.Forms.Padding(4);
            this.btnCustomerRefresh.Name = "btnCustomerRefresh";
            this.btnCustomerRefresh.Size = new System.Drawing.Size(35, 30);
            this.btnCustomerRefresh.TabIndex = 221;
            this.btnCustomerRefresh.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCustomerRefresh.UseVisualStyleBackColor = false;
            this.btnCustomerRefresh.Click += new System.EventHandler(this.btnCustomerRefresh_Click);
            // 
            // txtCustomer
            // 
            this.txtCustomer.Location = new System.Drawing.Point(119, 111);
            this.txtCustomer.Margin = new System.Windows.Forms.Padding(4);
            this.txtCustomer.Name = "txtCustomer";
            this.txtCustomer.ReadOnly = true;
            this.txtCustomer.Size = new System.Drawing.Size(265, 22);
            this.txtCustomer.TabIndex = 215;
            this.txtCustomer.TextChanged += new System.EventHandler(this.txtCustomer_TextChanged);
            this.txtCustomer.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtCustomer_KeyDown);
            // 
            // label37
            // 
            this.label37.AutoSize = true;
            this.label37.Location = new System.Drawing.Point(8, 114);
            this.label37.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label37.Name = "label37";
            this.label37.Size = new System.Drawing.Size(98, 17);
            this.label37.TabIndex = 213;
            this.label37.Text = "Customer (F9)";
            // 
            // cmbVAT1Name
            // 
            this.cmbVAT1Name.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbVAT1Name.FormattingEnabled = true;
            this.cmbVAT1Name.Location = new System.Drawing.Point(119, 47);
            this.cmbVAT1Name.Margin = new System.Windows.Forms.Padding(4);
            this.cmbVAT1Name.Name = "cmbVAT1Name";
            this.cmbVAT1Name.Size = new System.Drawing.Size(265, 24);
            this.cmbVAT1Name.TabIndex = 208;
            this.cmbVAT1Name.TabStop = false;
            // 
            // txtCode
            // 
            this.txtCode.Location = new System.Drawing.Point(564, 17);
            this.txtCode.Margin = new System.Windows.Forms.Padding(4);
            this.txtCode.MaximumSize = new System.Drawing.Size(265, 20);
            this.txtCode.MinimumSize = new System.Drawing.Size(132, 20);
            this.txtCode.Name = "txtCode";
            this.txtCode.Size = new System.Drawing.Size(145, 22);
            this.txtCode.TabIndex = 203;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(444, 22);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 17);
            this.label1.TabIndex = 204;
            this.label1.Text = "Code";
            // 
            // cmbPost
            // 
            this.cmbPost.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPost.FormattingEnabled = true;
            this.cmbPost.Items.AddRange(new object[] {
            "Y",
            "N"});
            this.cmbPost.Location = new System.Drawing.Point(564, 107);
            this.cmbPost.Margin = new System.Windows.Forms.Padding(4);
            this.cmbPost.Name = "cmbPost";
            this.cmbPost.Size = new System.Drawing.Size(105, 24);
            this.cmbPost.TabIndex = 201;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(444, 112);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(36, 17);
            this.label9.TabIndex = 202;
            this.label9.Text = "Post";
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Location = new System.Drawing.Point(8, 52);
            this.label30.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(35, 17);
            this.label30.TabIndex = 184;
            this.label30.Text = "VAT";
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.button1.Image = global::VATClient.Properties.Resources.search;
            this.button1.Location = new System.Drawing.Point(387, 17);
            this.button1.Margin = new System.Windows.Forms.Padding(4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(40, 25);
            this.button1.TabIndex = 1;
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // dtpEffectDate
            // 
            this.dtpEffectDate.Checked = false;
            this.dtpEffectDate.CustomFormat = "dd/MMM/yyyy";
            this.dtpEffectDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpEffectDate.Location = new System.Drawing.Point(564, 47);
            this.dtpEffectDate.Margin = new System.Windows.Forms.Padding(4);
            this.dtpEffectDate.Name = "dtpEffectDate";
            this.dtpEffectDate.ShowCheckBox = true;
            this.dtpEffectDate.Size = new System.Drawing.Size(145, 22);
            this.dtpEffectDate.TabIndex = 4;
            this.dtpEffectDate.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dtpEffectDate_KeyDown);
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSearch.Image = global::VATClient.Properties.Resources.search;
            this.btnSearch.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSearch.Location = new System.Drawing.Point(847, 10);
            this.btnSearch.Margin = new System.Windows.Forms.Padding(4);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(100, 34);
            this.btnSearch.TabIndex = 5;
            this.btnSearch.Text = "&Search";
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // btnExport
            // 
            this.btnExport.BackColor = System.Drawing.Color.White;
            this.btnExport.Image = global::VATClient.Properties.Resources.Load;
            this.btnExport.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnExport.Location = new System.Drawing.Point(424, 7);
            this.btnExport.Margin = new System.Windows.Forms.Padding(4);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(100, 34);
            this.btnExport.TabIndex = 238;
            this.btnExport.Text = "Export";
            this.btnExport.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnExport.UseVisualStyleBackColor = false;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // bgwSearch
            // 
            this.bgwSearch.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwSearch_DoWork);
            this.bgwSearch.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwSearch_RunWorkerCompleted);
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::VATClient.Properties.Resources.Green_Footer;
            this.panel1.Controls.Add(this.chkOverhead);
            this.panel1.Controls.Add(this.btnCompare);
            this.panel1.Controls.Add(this.LRecordCount);
            this.panel1.Controls.Add(this.btnExport);
            this.panel1.Controls.Add(this.btnOk);
            this.panel1.Location = new System.Drawing.Point(-3, 454);
            this.panel1.Margin = new System.Windows.Forms.Padding(4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(956, 49);
            this.panel1.TabIndex = 6;
            // 
            // btnCompare
            // 
            this.btnCompare.BackColor = System.Drawing.Color.White;
            this.btnCompare.Image = global::VATClient.Properties.Resources.Load;
            this.btnCompare.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCompare.Location = new System.Drawing.Point(532, 7);
            this.btnCompare.Margin = new System.Windows.Forms.Padding(4);
            this.btnCompare.Name = "btnCompare";
            this.btnCompare.Size = new System.Drawing.Size(143, 34);
            this.btnCompare.TabIndex = 239;
            this.btnCompare.Text = "Compare BOM";
            this.btnCompare.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCompare.UseVisualStyleBackColor = false;
            this.btnCompare.Click += new System.EventHandler(this.btnCompare_Click);
            // 
            // LRecordCount
            // 
            this.LRecordCount.AutoSize = true;
            this.LRecordCount.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LRecordCount.Location = new System.Drawing.Point(8, 15);
            this.LRecordCount.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LRecordCount.Name = "LRecordCount";
            this.LRecordCount.Size = new System.Drawing.Size(121, 21);
            this.LRecordCount.TabIndex = 228;
            this.LRecordCount.Text = "Record Count :";
            // 
            // btnOk
            // 
            this.btnOk.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnOk.Image = global::VATClient.Properties.Resources.Back;
            this.btnOk.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOk.Location = new System.Drawing.Point(847, 7);
            this.btnOk.Margin = new System.Windows.Forms.Padding(4);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(100, 34);
            this.btnOk.TabIndex = 8;
            this.btnOk.Text = "Ok";
            this.btnOk.UseVisualStyleBackColor = false;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // bgwMultiplePost
            // 
            this.bgwMultiplePost.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwMultiplePost_DoWork);
            this.bgwMultiplePost.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwMultiplePost_RunWorkerCompleted);
            // 
            // bgwDownload
            // 
            this.bgwDownload.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwDownload_DoWork);
            this.bgwDownload.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwDownload_RunWorkerCompleted);
            // 
            // chkOverhead
            // 
            this.chkOverhead.AutoSize = true;
            this.chkOverhead.BackColor = System.Drawing.Color.LightGreen;
            this.chkOverhead.Location = new System.Drawing.Point(683, 15);
            this.chkOverhead.Margin = new System.Windows.Forms.Padding(4);
            this.chkOverhead.Name = "chkOverhead";
            this.chkOverhead.Size = new System.Drawing.Size(137, 21);
            this.chkOverhead.TabIndex = 240;
            this.chkOverhead.Text = "Export Overhead";
            this.chkOverhead.UseVisualStyleBackColor = false;
            // 
            // FormBOMSearch
            // 
            this.AcceptButton = this.btnSearch;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(955, 506);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.grbVendorGroup);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(1061, 605);
            this.MinimizeBox = false;
            this.Name = "FormBOMSearch";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "BOM Search";
            this.Load += new System.EventHandler(this.FormBOMSearch_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvBOM)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.pnlHidden.ResumeLayout(false);
            this.pnlHidden.PerformLayout();
            this.grbVendorGroup.ResumeLayout(false);
            this.grbVendorGroup.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvBOM;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtProductName;
        private System.Windows.Forms.TextBox txtVATName;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.TextBox txtItemNo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox grbVendorGroup;
        private System.Windows.Forms.DateTimePicker dtpEffectDate;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.ComponentModel.BackgroundWorker bgwSearch;
        private System.Windows.Forms.Label LRecordCount;
        private System.Windows.Forms.ComboBox cmbPost;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtCode;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.ComboBox cmbVAT1Name;
        private System.Windows.Forms.Label label37;
        private System.Windows.Forms.TextBox txtCustomer;
        private System.Windows.Forms.Button btnCustomerRefresh;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cmbBranch;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label5;
        public System.Windows.Forms.ComboBox cmbRecordCount;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Select;
        private System.Windows.Forms.Button btnPost;
        private System.ComponentModel.BackgroundWorker bgwMultiplePost;
        private System.Windows.Forms.ComboBox cmbProductType;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.CheckBox chkSelect;
        private System.Windows.Forms.Panel pnlHidden;
        private System.Windows.Forms.DateTimePicker dtpEffectDateTo;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnCompare;
        private System.ComponentModel.BackgroundWorker bgwDownload;
        private System.Windows.Forms.CheckBox chkOverhead;
    }
}