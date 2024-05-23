namespace VATClient
{
    partial class FormSaleSearch
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSaleSearch));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.txtGrandTotalTo = new System.Windows.Forms.TextBox();
            this.txtSerialNo = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.grbTransactionHistory = new System.Windows.Forms.GroupBox();
            this.dateTimePeriodIdPicker = new System.Windows.Forms.DateTimePicker();
            this.label22 = new System.Windows.Forms.Label();
            this.SyncHSCode = new System.Windows.Forms.Button();
            this.label21 = new System.Windows.Forms.Label();
            this.cmbVDSCompleted = new System.Windows.Forms.ComboBox();
            this.LVDS = new System.Windows.Forms.Label();
            this.cmbVDS = new System.Windows.Forms.ComboBox();
            this.label20 = new System.Windows.Forms.Label();
            this.txtSearchValue = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.cmbSearchFields = new System.Windows.Forms.ComboBox();
            this.label18 = new System.Windows.Forms.Label();
            this.cmbConvComltd = new System.Windows.Forms.ComboBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label17 = new System.Windows.Forms.Label();
            this.btnCustomerRefresh = new System.Windows.Forms.Button();
            this.cmbRecordCount = new System.Windows.Forms.ComboBox();
            this.cmbBranch = new System.Windows.Forms.ComboBox();
            this.label15 = new System.Windows.Forms.Label();
            this.btnPost = new System.Windows.Forms.Button();
            this.txtEXPFormNo = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.cmbPost = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.cmbType = new System.Windows.Forms.ComboBox();
            this.label12 = new System.Windows.Forms.Label();
            this.cmbPrint = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtVehicleNo = new System.Windows.Forms.TextBox();
            this.txtCustomerName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtGrandTotalFrom = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.dtpSaleToDate = new System.Windows.Forms.DateTimePicker();
            this.dtpSaleFromDate = new System.Windows.Forms.DateTimePicker();
            this.btnSearch = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.txtInvoiceNo = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.chkSelectAll = new System.Windows.Forms.CheckBox();
            this.cmbExport = new System.Windows.Forms.ComboBox();
            this.btnExport = new System.Windows.Forms.Button();
            this.txtCustomerID = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtVehicleType = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.cmbTrading = new System.Windows.Forms.ComboBox();
            this.txtVatAmountTo = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.txtCustomerGroupName = new System.Windows.Forms.TextBox();
            this.txtCustomerGroupID = new System.Windows.Forms.TextBox();
            this.txtVehicleID = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.dgvSalesHistory = new System.Windows.Forms.DataGridView();
            this.ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlHidden = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.LRecordCount = new System.Windows.Forms.Label();
            this.btnOk = new System.Windows.Forms.Button();
            this.bgwSearch = new System.ComponentModel.BackgroundWorker();
            this.bgwMultiplePost = new System.ComponentModel.BackgroundWorker();
            this.bgwSyncHSCode = new System.ComponentModel.BackgroundWorker();
            this.grbTransactionHistory.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSalesHistory)).BeginInit();
            this.pnlHidden.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtGrandTotalTo
            // 
            this.txtGrandTotalTo.Location = new System.Drawing.Point(867, 64);
            this.txtGrandTotalTo.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtGrandTotalTo.MinimumSize = new System.Drawing.Size(75, 20);
            this.txtGrandTotalTo.Name = "txtGrandTotalTo";
            this.txtGrandTotalTo.Size = new System.Drawing.Size(131, 21);
            this.txtGrandTotalTo.TabIndex = 9;
            this.txtGrandTotalTo.Text = "0.00";
            this.txtGrandTotalTo.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtGrandTotalTo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtGrandTotalTo_KeyDown);
            this.txtGrandTotalTo.Leave += new System.EventHandler(this.txtGrandTotalTo_Leave);
            // 
            // txtSerialNo
            // 
            this.txtSerialNo.BackColor = System.Drawing.SystemColors.Window;
            this.txtSerialNo.Location = new System.Drawing.Point(104, 63);
            this.txtSerialNo.Name = "txtSerialNo";
            this.txtSerialNo.Size = new System.Drawing.Size(200, 21);
            this.txtSerialNo.TabIndex = 2;
            this.txtSerialNo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSerialNo_KeyDown);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(15, 67);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(64, 13);
            this.label6.TabIndex = 126;
            this.label6.Text = "Ref Number";
            // 
            // grbTransactionHistory
            // 
            this.grbTransactionHistory.Controls.Add(this.dateTimePeriodIdPicker);
            this.grbTransactionHistory.Controls.Add(this.label22);
            this.grbTransactionHistory.Controls.Add(this.SyncHSCode);
            this.grbTransactionHistory.Controls.Add(this.label21);
            this.grbTransactionHistory.Controls.Add(this.cmbVDSCompleted);
            this.grbTransactionHistory.Controls.Add(this.LVDS);
            this.grbTransactionHistory.Controls.Add(this.cmbVDS);
            this.grbTransactionHistory.Controls.Add(this.label20);
            this.grbTransactionHistory.Controls.Add(this.txtSearchValue);
            this.grbTransactionHistory.Controls.Add(this.label19);
            this.grbTransactionHistory.Controls.Add(this.cmbSearchFields);
            this.grbTransactionHistory.Controls.Add(this.label18);
            this.grbTransactionHistory.Controls.Add(this.cmbConvComltd);
            this.grbTransactionHistory.Controls.Add(this.btnCancel);
            this.grbTransactionHistory.Controls.Add(this.label17);
            this.grbTransactionHistory.Controls.Add(this.btnCustomerRefresh);
            this.grbTransactionHistory.Controls.Add(this.cmbRecordCount);
            this.grbTransactionHistory.Controls.Add(this.cmbBranch);
            this.grbTransactionHistory.Controls.Add(this.label15);
            this.grbTransactionHistory.Controls.Add(this.btnPost);
            this.grbTransactionHistory.Controls.Add(this.txtEXPFormNo);
            this.grbTransactionHistory.Controls.Add(this.label14);
            this.grbTransactionHistory.Controls.Add(this.cmbPost);
            this.grbTransactionHistory.Controls.Add(this.label9);
            this.grbTransactionHistory.Controls.Add(this.cmbType);
            this.grbTransactionHistory.Controls.Add(this.label12);
            this.grbTransactionHistory.Controls.Add(this.cmbPrint);
            this.grbTransactionHistory.Controls.Add(this.label8);
            this.grbTransactionHistory.Controls.Add(this.label5);
            this.grbTransactionHistory.Controls.Add(this.txtVehicleNo);
            this.grbTransactionHistory.Controls.Add(this.txtCustomerName);
            this.grbTransactionHistory.Controls.Add(this.label2);
            this.grbTransactionHistory.Controls.Add(this.txtSerialNo);
            this.grbTransactionHistory.Controls.Add(this.label6);
            this.grbTransactionHistory.Controls.Add(this.txtGrandTotalTo);
            this.grbTransactionHistory.Controls.Add(this.label4);
            this.grbTransactionHistory.Controls.Add(this.txtGrandTotalFrom);
            this.grbTransactionHistory.Controls.Add(this.label11);
            this.grbTransactionHistory.Controls.Add(this.dtpSaleToDate);
            this.grbTransactionHistory.Controls.Add(this.dtpSaleFromDate);
            this.grbTransactionHistory.Controls.Add(this.btnSearch);
            this.grbTransactionHistory.Controls.Add(this.label3);
            this.grbTransactionHistory.Controls.Add(this.txtInvoiceNo);
            this.grbTransactionHistory.Controls.Add(this.label1);
            this.grbTransactionHistory.Location = new System.Drawing.Point(14, 3);
            this.grbTransactionHistory.Name = "grbTransactionHistory";
            this.grbTransactionHistory.Size = new System.Drawing.Size(756, 189);
            this.grbTransactionHistory.TabIndex = 119;
            this.grbTransactionHistory.TabStop = false;
            this.grbTransactionHistory.Text = "Searching Criteria";
            // 
            // dateTimePeriodIdPicker
            // 
            this.dateTimePeriodIdPicker.Checked = false;
            this.dateTimePeriodIdPicker.CustomFormat = "MMMM-yyyy";
            this.dateTimePeriodIdPicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePeriodIdPicker.Location = new System.Drawing.Point(104, 161);
            this.dateTimePeriodIdPicker.Name = "dateTimePeriodIdPicker";
            this.dateTimePeriodIdPicker.ShowCheckBox = true;
            this.dateTimePeriodIdPicker.Size = new System.Drawing.Size(119, 21);
            this.dateTimePeriodIdPicker.TabIndex = 249;
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(18, 165);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(67, 13);
            this.label22.TabIndex = 248;
            this.label22.Text = "Period Name";
            this.label22.Visible = false;
            // 
            // SyncHSCode
            // 
            this.SyncHSCode.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.SyncHSCode.Image = global::VATClient.Properties.Resources.Referesh;
            this.SyncHSCode.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.SyncHSCode.Location = new System.Drawing.Point(229, 159);
            this.SyncHSCode.Name = "SyncHSCode";
            this.SyncHSCode.Size = new System.Drawing.Size(75, 28);
            this.SyncHSCode.TabIndex = 247;
            this.SyncHSCode.Text = "HS Code";
            this.SyncHSCode.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.SyncHSCode.UseVisualStyleBackColor = false;
            this.SyncHSCode.Click += new System.EventHandler(this.SyncHSCode_Click);
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(330, 138);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(80, 13);
            this.label21.TabIndex = 246;
            this.label21.Text = "VDS Completed";
            // 
            // cmbVDSCompleted
            // 
            this.cmbVDSCompleted.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbVDSCompleted.FormattingEnabled = true;
            this.cmbVDSCompleted.Items.AddRange(new object[] {
            "Y",
            "N"});
            this.cmbVDSCompleted.Location = new System.Drawing.Point(420, 134);
            this.cmbVDSCompleted.Name = "cmbVDSCompleted";
            this.cmbVDSCompleted.Size = new System.Drawing.Size(80, 21);
            this.cmbVDSCompleted.TabIndex = 245;
            // 
            // LVDS
            // 
            this.LVDS.AutoSize = true;
            this.LVDS.Location = new System.Drawing.Point(510, 135);
            this.LVDS.Name = "LVDS";
            this.LVDS.Size = new System.Drawing.Size(26, 13);
            this.LVDS.TabIndex = 244;
            this.LVDS.Text = "VDS";
            // 
            // cmbVDS
            // 
            this.cmbVDS.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbVDS.FormattingEnabled = true;
            this.cmbVDS.Items.AddRange(new object[] {
            "Y",
            "N"});
            this.cmbVDS.Location = new System.Drawing.Point(540, 132);
            this.cmbVDS.Name = "cmbVDS";
            this.cmbVDS.Size = new System.Drawing.Size(80, 21);
            this.cmbVDS.TabIndex = 243;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(341, 161);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(69, 13);
            this.label20.TabIndex = 242;
            this.label20.Text = "Search Value";
            this.label20.Visible = false;
            // 
            // txtSearchValue
            // 
            this.txtSearchValue.Location = new System.Drawing.Point(420, 157);
            this.txtSearchValue.Name = "txtSearchValue";
            this.txtSearchValue.Size = new System.Drawing.Size(200, 21);
            this.txtSearchValue.TabIndex = 241;
            this.txtSearchValue.Visible = false;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(15, 134);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(70, 13);
            this.label19.TabIndex = 240;
            this.label19.Text = "Search Fields";
            this.label19.Visible = false;
            // 
            // cmbSearchFields
            // 
            this.cmbSearchFields.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSearchFields.FormattingEnabled = true;
            this.cmbSearchFields.Items.AddRange(new object[] {
            "RefNo",
            "ChallanNo",
            "SSO",
            "Section"});
            this.cmbSearchFields.Location = new System.Drawing.Point(104, 130);
            this.cmbSearchFields.Name = "cmbSearchFields";
            this.cmbSearchFields.Size = new System.Drawing.Size(200, 21);
            this.cmbSearchFields.TabIndex = 239;
            this.cmbSearchFields.Visible = false;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(510, 112);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(25, 13);
            this.label18.TabIndex = 238;
            this.label18.Text = "Top";
            // 
            // cmbConvComltd
            // 
            this.cmbConvComltd.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbConvComltd.FormattingEnabled = true;
            this.cmbConvComltd.Items.AddRange(new object[] {
            "Y",
            "N"});
            this.cmbConvComltd.Location = new System.Drawing.Point(420, 107);
            this.cmbConvComltd.Name = "cmbConvComltd";
            this.cmbConvComltd.Size = new System.Drawing.Size(80, 21);
            this.cmbConvComltd.TabIndex = 236;
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnCancel.Image = global::VATClient.Properties.Resources.Referesh;
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.Location = new System.Drawing.Point(678, 46);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 28);
            this.btnCancel.TabIndex = 12;
            this.btnCancel.Text = "&Refresh";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(329, 111);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(86, 13);
            this.label17.TabIndex = 237;
            this.label17.Text = "Conv Completed";
            // 
            // btnCustomerRefresh
            // 
            this.btnCustomerRefresh.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnCustomerRefresh.Image = ((System.Drawing.Image)(resources.GetObject("btnCustomerRefresh.Image")));
            this.btnCustomerRefresh.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCustomerRefresh.Location = new System.Drawing.Point(307, 83);
            this.btnCustomerRefresh.Name = "btnCustomerRefresh";
            this.btnCustomerRefresh.Size = new System.Drawing.Size(25, 24);
            this.btnCustomerRefresh.TabIndex = 235;
            this.btnCustomerRefresh.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCustomerRefresh.UseVisualStyleBackColor = false;
            this.btnCustomerRefresh.Click += new System.EventHandler(this.btnCustomerRefresh_Click);
            // 
            // cmbRecordCount
            // 
            this.cmbRecordCount.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRecordCount.FormattingEnabled = true;
            this.cmbRecordCount.Location = new System.Drawing.Point(540, 108);
            this.cmbRecordCount.Name = "cmbRecordCount";
            this.cmbRecordCount.Size = new System.Drawing.Size(80, 21);
            this.cmbRecordCount.TabIndex = 234;
            // 
            // cmbBranch
            // 
            this.cmbBranch.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBranch.FormattingEnabled = true;
            this.cmbBranch.Items.AddRange(new object[] {
            "Y",
            "N"});
            this.cmbBranch.Location = new System.Drawing.Point(104, 107);
            this.cmbBranch.Name = "cmbBranch";
            this.cmbBranch.Size = new System.Drawing.Size(200, 21);
            this.cmbBranch.TabIndex = 225;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(15, 111);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(40, 13);
            this.label15.TabIndex = 224;
            this.label15.Text = "Branch";
            // 
            // btnPost
            // 
            this.btnPost.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnPost.Image = global::VATClient.Properties.Resources.Post;
            this.btnPost.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPost.Location = new System.Drawing.Point(678, 125);
            this.btnPost.Name = "btnPost";
            this.btnPost.Size = new System.Drawing.Size(75, 28);
            this.btnPost.TabIndex = 201;
            this.btnPost.Text = "Post";
            this.btnPost.UseVisualStyleBackColor = false;
            this.btnPost.Click += new System.EventHandler(this.btnPost_Click);
            // 
            // txtEXPFormNo
            // 
            this.txtEXPFormNo.Location = new System.Drawing.Point(420, 63);
            this.txtEXPFormNo.Name = "txtEXPFormNo";
            this.txtEXPFormNo.Size = new System.Drawing.Size(200, 21);
            this.txtEXPFormNo.TabIndex = 199;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(347, 67);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(68, 13);
            this.label14.TabIndex = 200;
            this.label14.Text = "EXP Form No";
            // 
            // cmbPost
            // 
            this.cmbPost.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPost.FormattingEnabled = true;
            this.cmbPost.Items.AddRange(new object[] {
            "Y",
            "N"});
            this.cmbPost.Location = new System.Drawing.Point(420, 85);
            this.cmbPost.Name = "cmbPost";
            this.cmbPost.Size = new System.Drawing.Size(80, 21);
            this.cmbPost.TabIndex = 9;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(387, 89);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(28, 13);
            this.label9.TabIndex = 198;
            this.label9.Text = "Post";
            // 
            // cmbType
            // 
            this.cmbType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbType.FormattingEnabled = true;
            this.cmbType.Items.AddRange(new object[] {
            "New",
            "Debit",
            "Credit"});
            this.cmbType.Location = new System.Drawing.Point(104, 40);
            this.cmbType.Name = "cmbType";
            this.cmbType.Size = new System.Drawing.Size(200, 21);
            this.cmbType.TabIndex = 1;
            this.cmbType.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmbType_KeyDown);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(14, 44);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(31, 13);
            this.label12.TabIndex = 197;
            this.label12.Text = "Type";
            // 
            // cmbPrint
            // 
            this.cmbPrint.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPrint.FormattingEnabled = true;
            this.cmbPrint.Items.AddRange(new object[] {
            "Y",
            "N"});
            this.cmbPrint.Location = new System.Drawing.Point(540, 85);
            this.cmbPrint.Name = "cmbPrint";
            this.cmbPrint.Size = new System.Drawing.Size(80, 21);
            this.cmbPrint.TabIndex = 8;
            this.cmbPrint.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmbPrint_KeyDown);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(512, 89);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(29, 13);
            this.label8.TabIndex = 150;
            this.label8.Text = "Print";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(359, 44);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(56, 13);
            this.label5.TabIndex = 136;
            this.label5.Text = "Vehicle No";
            // 
            // txtVehicleNo
            // 
            this.txtVehicleNo.Location = new System.Drawing.Point(420, 40);
            this.txtVehicleNo.Name = "txtVehicleNo";
            this.txtVehicleNo.Size = new System.Drawing.Size(200, 21);
            this.txtVehicleNo.TabIndex = 6;
            this.txtVehicleNo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtVehicleNo_KeyDown);
            // 
            // txtCustomerName
            // 
            this.txtCustomerName.Location = new System.Drawing.Point(104, 85);
            this.txtCustomerName.Name = "txtCustomerName";
            this.txtCustomerName.Size = new System.Drawing.Size(200, 21);
            this.txtCustomerName.TabIndex = 3;
            this.txtCustomerName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtCustomerName_KeyDown);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 89);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 13);
            this.label2.TabIndex = 128;
            this.label2.Text = "Customer (F9)";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(846, 67);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(17, 13);
            this.label4.TabIndex = 117;
            this.label4.Text = "to";
            // 
            // txtGrandTotalFrom
            // 
            this.txtGrandTotalFrom.Location = new System.Drawing.Point(762, 64);
            this.txtGrandTotalFrom.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtGrandTotalFrom.MinimumSize = new System.Drawing.Size(75, 20);
            this.txtGrandTotalFrom.Name = "txtGrandTotalFrom";
            this.txtGrandTotalFrom.Size = new System.Drawing.Size(131, 21);
            this.txtGrandTotalFrom.TabIndex = 8;
            this.txtGrandTotalFrom.Text = "0.00";
            this.txtGrandTotalFrom.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtGrandTotalFrom.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtGrandTotalFrom_KeyDown);
            this.txtGrandTotalFrom.Leave += new System.EventHandler(this.txtGrandTotalFrom_Leave);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(527, 22);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(17, 13);
            this.label11.TabIndex = 112;
            this.label11.Text = "to";
            // 
            // dtpSaleToDate
            // 
            this.dtpSaleToDate.Checked = false;
            this.dtpSaleToDate.CustomFormat = "dd/MMM/yyyy";
            this.dtpSaleToDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpSaleToDate.Location = new System.Drawing.Point(548, 18);
            this.dtpSaleToDate.Name = "dtpSaleToDate";
            this.dtpSaleToDate.ShowCheckBox = true;
            this.dtpSaleToDate.Size = new System.Drawing.Size(103, 21);
            this.dtpSaleToDate.TabIndex = 5;
            this.dtpSaleToDate.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dtpSaleToDate_KeyDown);
            // 
            // dtpSaleFromDate
            // 
            this.dtpSaleFromDate.Checked = false;
            this.dtpSaleFromDate.CustomFormat = "dd/MMM/yyyy";
            this.dtpSaleFromDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpSaleFromDate.Location = new System.Drawing.Point(420, 18);
            this.dtpSaleFromDate.Name = "dtpSaleFromDate";
            this.dtpSaleFromDate.ShowCheckBox = true;
            this.dtpSaleFromDate.Size = new System.Drawing.Size(103, 21);
            this.dtpSaleFromDate.TabIndex = 4;
            this.dtpSaleFromDate.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dtpSaleFromDate_KeyDown);
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSearch.Image = global::VATClient.Properties.Resources.search;
            this.btnSearch.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSearch.Location = new System.Drawing.Point(678, 16);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 28);
            this.btnSearch.TabIndex = 7;
            this.btnSearch.Text = "&Search";
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(357, 22);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(58, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Sales Date";
            // 
            // txtInvoiceNo
            // 
            this.txtInvoiceNo.Location = new System.Drawing.Point(104, 18);
            this.txtInvoiceNo.Name = "txtInvoiceNo";
            this.txtInvoiceNo.Size = new System.Drawing.Size(200, 21);
            this.txtInvoiceNo.TabIndex = 0;
            this.txtInvoiceNo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtInvoiceNo_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Invoice No";
            // 
            // chkSelectAll
            // 
            this.chkSelectAll.AutoSize = true;
            this.chkSelectAll.Location = new System.Drawing.Point(0, -1);
            this.chkSelectAll.Name = "chkSelectAll";
            this.chkSelectAll.Size = new System.Drawing.Size(66, 17);
            this.chkSelectAll.TabIndex = 213;
            this.chkSelectAll.Text = "SelectAll";
            this.chkSelectAll.UseVisualStyleBackColor = true;
            this.chkSelectAll.CheckedChanged += new System.EventHandler(this.chkSelectAll_CheckedChanged);
            this.chkSelectAll.Click += new System.EventHandler(this.chkSelectAll_Click);
            // 
            // cmbExport
            // 
            this.cmbExport.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbExport.FormattingEnabled = true;
            this.cmbExport.Items.AddRange(new object[] {
            "EXCEL",
            "TEXT",
            "XML",
            "Json"});
            this.cmbExport.Location = new System.Drawing.Point(316, 6);
            this.cmbExport.Name = "cmbExport";
            this.cmbExport.Size = new System.Drawing.Size(80, 21);
            this.cmbExport.TabIndex = 233;
            // 
            // btnExport
            // 
            this.btnExport.BackColor = System.Drawing.Color.White;
            this.btnExport.Image = global::VATClient.Properties.Resources.Load;
            this.btnExport.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnExport.Location = new System.Drawing.Point(399, 2);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(75, 28);
            this.btnExport.TabIndex = 226;
            this.btnExport.Text = "Export";
            this.btnExport.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnExport.UseVisualStyleBackColor = false;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // txtCustomerID
            // 
            this.txtCustomerID.Location = new System.Drawing.Point(5, 3);
            this.txtCustomerID.MaximumSize = new System.Drawing.Size(4, 4);
            this.txtCustomerID.MinimumSize = new System.Drawing.Size(20, 20);
            this.txtCustomerID.Name = "txtCustomerID";
            this.txtCustomerID.Size = new System.Drawing.Size(20, 20);
            this.txtCustomerID.TabIndex = 127;
            this.txtCustomerID.Visible = false;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 53);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(43, 13);
            this.label7.TabIndex = 148;
            this.label7.Text = "Trading";
            this.label7.Visible = false;
            // 
            // txtVehicleType
            // 
            this.txtVehicleType.Location = new System.Drawing.Point(76, 75);
            this.txtVehicleType.MaximumSize = new System.Drawing.Size(4, 4);
            this.txtVehicleType.MinimumSize = new System.Drawing.Size(40, 20);
            this.txtVehicleType.Name = "txtVehicleType";
            this.txtVehicleType.Size = new System.Drawing.Size(40, 20);
            this.txtVehicleType.TabIndex = 3;
            this.txtVehicleType.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtVehicleType_KeyDown);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(3, 78);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(67, 13);
            this.label16.TabIndex = 130;
            this.label16.Text = "Vehicle Type";
            // 
            // cmbTrading
            // 
            this.cmbTrading.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTrading.FormattingEnabled = true;
            this.cmbTrading.Location = new System.Drawing.Point(5, 28);
            this.cmbTrading.Name = "cmbTrading";
            this.cmbTrading.Size = new System.Drawing.Size(40, 21);
            this.cmbTrading.TabIndex = 10;
            this.cmbTrading.Visible = false;
            // 
            // txtVatAmountTo
            // 
            this.txtVatAmountTo.Location = new System.Drawing.Point(31, 3);
            this.txtVatAmountTo.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtVatAmountTo.MinimumSize = new System.Drawing.Size(75, 20);
            this.txtVatAmountTo.Name = "txtVatAmountTo";
            this.txtVatAmountTo.Size = new System.Drawing.Size(75, 21);
            this.txtVatAmountTo.TabIndex = 7;
            this.txtVatAmountTo.Text = "0.00";
            this.txtVatAmountTo.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtVatAmountTo.Visible = false;
            this.txtVatAmountTo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtVatAmountTo_KeyDown);
            this.txtVatAmountTo.Leave += new System.EventHandler(this.txtVatAmountTo_Leave);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(52, 53);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(17, 13);
            this.label13.TabIndex = 114;
            this.label13.Text = "to";
            this.label13.Visible = false;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(75, 53);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(97, 13);
            this.label10.TabIndex = 6;
            this.label10.Text = "Total VAT Amount:";
            this.label10.Visible = false;
            // 
            // txtCustomerGroupName
            // 
            this.txtCustomerGroupName.Location = new System.Drawing.Point(793, 105);
            this.txtCustomerGroupName.MaximumSize = new System.Drawing.Size(4, 4);
            this.txtCustomerGroupName.MinimumSize = new System.Drawing.Size(20, 20);
            this.txtCustomerGroupName.Name = "txtCustomerGroupName";
            this.txtCustomerGroupName.Size = new System.Drawing.Size(20, 20);
            this.txtCustomerGroupName.TabIndex = 133;
            // 
            // txtCustomerGroupID
            // 
            this.txtCustomerGroupID.Location = new System.Drawing.Point(793, 149);
            this.txtCustomerGroupID.MaximumSize = new System.Drawing.Size(4, 4);
            this.txtCustomerGroupID.MinimumSize = new System.Drawing.Size(20, 20);
            this.txtCustomerGroupID.Name = "txtCustomerGroupID";
            this.txtCustomerGroupID.Size = new System.Drawing.Size(20, 20);
            this.txtCustomerGroupID.TabIndex = 132;
            // 
            // txtVehicleID
            // 
            this.txtVehicleID.Location = new System.Drawing.Point(793, 83);
            this.txtVehicleID.MaximumSize = new System.Drawing.Size(4, 4);
            this.txtVehicleID.MinimumSize = new System.Drawing.Size(20, 20);
            this.txtVehicleID.Name = "txtVehicleID";
            this.txtVehicleID.Size = new System.Drawing.Size(20, 20);
            this.txtVehicleID.TabIndex = 129;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.progressBar1);
            this.groupBox1.Controls.Add(this.dgvSalesHistory);
            this.groupBox1.Controls.Add(this.chkSelectAll);
            this.groupBox1.Controls.Add(this.pnlHidden);
            this.groupBox1.Location = new System.Drawing.Point(12, 192);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(759, 229);
            this.groupBox1.TabIndex = 120;
            this.groupBox1.TabStop = false;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(224, 112);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(300, 30);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 212;
            this.progressBar1.Visible = false;
            // 
            // dgvSalesHistory
            // 
            this.dgvSalesHistory.AllowUserToAddRows = false;
            this.dgvSalesHistory.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black;
            this.dgvSalesHistory.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvSalesHistory.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSalesHistory.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ID});
            this.dgvSalesHistory.Location = new System.Drawing.Point(3, 20);
            this.dgvSalesHistory.Name = "dgvSalesHistory";
            this.dgvSalesHistory.RowHeadersVisible = false;
            this.dgvSalesHistory.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvSalesHistory.Size = new System.Drawing.Size(752, 202);
            this.dgvSalesHistory.TabIndex = 6;
            this.dgvSalesHistory.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSalesHistory_CellContentClick);
            this.dgvSalesHistory.DoubleClick += new System.EventHandler(this.dgvSalesHistory_DoubleClick);
            // 
            // ID
            // 
            this.ID.DataPropertyName = "ID";
            this.ID.HeaderText = "ID";
            this.ID.Name = "ID";
            this.ID.ReadOnly = true;
            this.ID.Visible = false;
            this.ID.Width = 24;
            // 
            // pnlHidden
            // 
            this.pnlHidden.Controls.Add(this.txtCustomerID);
            this.pnlHidden.Controls.Add(this.label7);
            this.pnlHidden.Controls.Add(this.cmbTrading);
            this.pnlHidden.Controls.Add(this.label10);
            this.pnlHidden.Controls.Add(this.label13);
            this.pnlHidden.Controls.Add(this.txtVatAmountTo);
            this.pnlHidden.Controls.Add(this.label16);
            this.pnlHidden.Controls.Add(this.txtVehicleType);
            this.pnlHidden.Location = new System.Drawing.Point(6, 137);
            this.pnlHidden.Name = "pnlHidden";
            this.pnlHidden.Size = new System.Drawing.Size(200, 100);
            this.pnlHidden.TabIndex = 213;
            this.pnlHidden.Visible = false;
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::VATClient.Properties.Resources.Green_Footer;
            this.panel1.Controls.Add(this.LRecordCount);
            this.panel1.Controls.Add(this.btnOk);
            this.panel1.Controls.Add(this.btnExport);
            this.panel1.Controls.Add(this.cmbExport);
            this.panel1.Location = new System.Drawing.Point(-1, 427);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(784, 33);
            this.panel1.TabIndex = 11;
            this.panel1.TabStop = true;
            // 
            // LRecordCount
            // 
            this.LRecordCount.AutoSize = true;
            this.LRecordCount.Location = new System.Drawing.Point(4, 10);
            this.LRecordCount.Name = "LRecordCount";
            this.LRecordCount.Size = new System.Drawing.Size(107, 13);
            this.LRecordCount.TabIndex = 14;
            this.LRecordCount.Text = "Total Record Count: ";
            // 
            // btnOk
            // 
            this.btnOk.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnOk.Image = global::VATClient.Properties.Resources.Back;
            this.btnOk.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOk.Location = new System.Drawing.Point(699, 2);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 28);
            this.btnOk.TabIndex = 13;
            this.btnOk.Text = "&Ok";
            this.btnOk.UseVisualStyleBackColor = false;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // bgwSearch
            // 
            this.bgwSearch.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwSearch_DoWork);
            this.bgwSearch.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwSearch_RunWorkerCompleted);
            // 
            // bgwMultiplePost
            // 
            this.bgwMultiplePost.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwMultiplePost_DoWork);
            this.bgwMultiplePost.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwMultiplePost_RunWorkerCompleted);
            // 
            // bgwSyncHSCode
            // 
            this.bgwSyncHSCode.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwSyncHSCode_DoWork);
            this.bgwSyncHSCode.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwSyncHSCode_RunWorkerCompleted);
            // 
            // FormSaleSearch
            // 
            this.AcceptButton = this.btnSearch;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.CancelButton = this.btnOk;
            this.ClientSize = new System.Drawing.Size(784, 461);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.grbTransactionHistory);
            this.Controls.Add(this.txtCustomerGroupName);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.txtCustomerGroupID);
            this.Controls.Add(this.txtVehicleID);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(800, 500);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(800, 500);
            this.Name = "FormSaleSearch";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Sale Search";
            this.Load += new System.EventHandler(this.FormSaleSearch_Load);
            this.grbTransactionHistory.ResumeLayout(false);
            this.grbTransactionHistory.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSalesHistory)).EndInit();
            this.pnlHidden.ResumeLayout(false);
            this.pnlHidden.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TextBox txtGrandTotalTo;
        private System.Windows.Forms.TextBox txtSerialNo;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox grbTransactionHistory;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtGrandTotalFrom;
        private System.Windows.Forms.TextBox txtVatAmountTo;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.DateTimePicker dtpSaleToDate;
        private System.Windows.Forms.DateTimePicker dtpSaleFromDate;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtInvoiceNo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridView dgvSalesHistory;
        private System.Windows.Forms.TextBox txtVehicleID;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox txtCustomerID;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtCustomerName;
        private System.Windows.Forms.TextBox txtCustomerGroupName;
        private System.Windows.Forms.TextBox txtCustomerGroupID;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtVehicleNo;
        private System.Windows.Forms.TextBox txtVehicleType;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox cmbType;
        private System.Windows.Forms.Label label12;
        public System.Windows.Forms.ComboBox cmbTrading;
        private System.Windows.Forms.Label label9;
        public System.Windows.Forms.ComboBox cmbPrint;
        public System.Windows.Forms.ComboBox cmbPost;
        private System.ComponentModel.BackgroundWorker bgwSearch;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label LRecordCount;
        private System.Windows.Forms.TextBox txtEXPFormNo;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Button btnPost;
        private System.ComponentModel.BackgroundWorker bgwMultiplePost;
        private System.Windows.Forms.CheckBox chkSelectAll;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.ComboBox cmbBranch;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.ComboBox cmbExport;
        public System.Windows.Forms.ComboBox cmbRecordCount;
        private System.Windows.Forms.Button btnCustomerRefresh;
        private System.Windows.Forms.DataGridViewTextBoxColumn ID;
        public System.Windows.Forms.ComboBox cmbConvComltd;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Panel pnlHidden;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.TextBox txtSearchValue;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.ComboBox cmbSearchFields;
        public System.Windows.Forms.Label LVDS;
        public System.Windows.Forms.ComboBox cmbVDS;
        public System.Windows.Forms.Label label21;
        public System.Windows.Forms.ComboBox cmbVDSCompleted;
        private System.Windows.Forms.Button SyncHSCode;
        private System.ComponentModel.BackgroundWorker bgwSyncHSCode;
        private System.Windows.Forms.DateTimePicker dateTimePeriodIdPicker;
        private System.Windows.Forms.Label label22;
    }
}