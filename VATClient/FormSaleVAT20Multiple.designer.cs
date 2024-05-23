namespace VATClient
{
    partial class FormSaleVAT20Multiple
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
            this.txtGrandTotalTo = new System.Windows.Forms.TextBox();
            this.txtSerialNo = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.grbTransactionHistory = new System.Windows.Forms.GroupBox();
            this.txtEXPFormNo = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.cmbPost = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.cmbPrint = new System.Windows.Forms.ComboBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.txtVehicleType = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtCustomerName = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtPortTo = new System.Windows.Forms.TextBox();
            this.txtPortFrom = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtGrandTotalFrom = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.dtpSaleToDate = new System.Windows.Forms.DateTimePicker();
            this.dtpSaleFromDate = new System.Windows.Forms.DateTimePicker();
            this.btnSearch = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.txtInvoiceNo = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbType = new System.Windows.Forms.ComboBox();
            this.cmbTrading = new System.Windows.Forms.ComboBox();
            this.txtVehicleNo = new System.Windows.Forms.TextBox();
            this.txtVatAmountTo = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.txtCustomerGroupName = new System.Windows.Forms.TextBox();
            this.txtCustomerGroupID = new System.Windows.Forms.TextBox();
            this.txtVehicleID = new System.Windows.Forms.TextBox();
            this.txtCustomerID = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.dgvSalesHistory = new System.Windows.Forms.DataGridView();
            this.Select = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.SalesInvoiceNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CustomerID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CustomerName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CustomerGroupName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DeliveryAddress1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DeliveryAddress2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DeliveryAddress3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.VehicleID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.VehicleType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.VehicleNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TotalAmount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TotalVATAmount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SerialNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.InvoiceDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TenderId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Comments = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SaleType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Trading = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.IsPrint = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Post = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DeliveryDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CurrencyID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CurrencyRateFromBDT = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CurrencyCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SaleReturnId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TransactionType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LCNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AlReadyPrint = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnClose = new System.Windows.Forms.Button();
            this.LRecordCount = new System.Windows.Forms.Label();
            this.btnPrint = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.bgwSearch = new System.ComponentModel.BackgroundWorker();
            this.grbTransactionHistory.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSalesHistory)).BeginInit();
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
            this.txtSerialNo.Location = new System.Drawing.Point(104, 45);
            this.txtSerialNo.MaximumSize = new System.Drawing.Size(4, 5);
            this.txtSerialNo.MinimumSize = new System.Drawing.Size(185, 20);
            this.txtSerialNo.Name = "txtSerialNo";
            this.txtSerialNo.Size = new System.Drawing.Size(185, 20);
            this.txtSerialNo.TabIndex = 2;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(15, 49);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(64, 13);
            this.label6.TabIndex = 126;
            this.label6.Text = "Ref Number";
            // 
            // grbTransactionHistory
            // 
            this.grbTransactionHistory.Controls.Add(this.txtEXPFormNo);
            this.grbTransactionHistory.Controls.Add(this.label14);
            this.grbTransactionHistory.Controls.Add(this.button1);
            this.grbTransactionHistory.Controls.Add(this.cmbPost);
            this.grbTransactionHistory.Controls.Add(this.label9);
            this.grbTransactionHistory.Controls.Add(this.cmbPrint);
            this.grbTransactionHistory.Controls.Add(this.label12);
            this.grbTransactionHistory.Controls.Add(this.label8);
            this.grbTransactionHistory.Controls.Add(this.label7);
            this.grbTransactionHistory.Controls.Add(this.txtVehicleType);
            this.grbTransactionHistory.Controls.Add(this.label5);
            this.grbTransactionHistory.Controls.Add(this.txtCustomerName);
            this.grbTransactionHistory.Controls.Add(this.label16);
            this.grbTransactionHistory.Controls.Add(this.label2);
            this.grbTransactionHistory.Controls.Add(this.txtPortTo);
            this.grbTransactionHistory.Controls.Add(this.txtPortFrom);
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
            this.grbTransactionHistory.Size = new System.Drawing.Size(756, 123);
            this.grbTransactionHistory.TabIndex = 119;
            this.grbTransactionHistory.TabStop = false;
            this.grbTransactionHistory.Text = "Searching Criteria";
            // 
            // txtEXPFormNo
            // 
            this.txtEXPFormNo.Location = new System.Drawing.Point(404, 74);
            this.txtEXPFormNo.MaximumSize = new System.Drawing.Size(4, 4);
            this.txtEXPFormNo.MinimumSize = new System.Drawing.Size(185, 20);
            this.txtEXPFormNo.Name = "txtEXPFormNo";
            this.txtEXPFormNo.Size = new System.Drawing.Size(185, 20);
            this.txtEXPFormNo.TabIndex = 201;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(315, 78);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(68, 13);
            this.label14.TabIndex = 202;
            this.label14.Text = "EXP Form No";
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.button1.Image = global::VATClient.Properties.Resources.search;
            this.button1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button1.Location = new System.Drawing.Point(624, 59);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 28);
            this.button1.TabIndex = 199;
            this.button1.Text = "&Search";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // cmbPost
            // 
            this.cmbPost.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPost.FormattingEnabled = true;
            this.cmbPost.Items.AddRange(new object[] {
            "Y",
            "N"});
            this.cmbPost.Location = new System.Drawing.Point(104, 96);
            this.cmbPost.Name = "cmbPost";
            this.cmbPost.Size = new System.Drawing.Size(125, 21);
            this.cmbPost.TabIndex = 9;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(18, 97);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(28, 13);
            this.label9.TabIndex = 198;
            this.label9.Text = "Post";
            // 
            // cmbPrint
            // 
            this.cmbPrint.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPrint.FormattingEnabled = true;
            this.cmbPrint.Items.AddRange(new object[] {
            "Y",
            "N"});
            this.cmbPrint.Location = new System.Drawing.Point(104, 72);
            this.cmbPrint.Name = "cmbPrint";
            this.cmbPrint.Size = new System.Drawing.Size(125, 21);
            this.cmbPrint.TabIndex = 8;
            this.cmbPrint.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmbPrint_KeyDown);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(15, 138);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(54, 13);
            this.label12.TabIndex = 197;
            this.label12.Text = "Port From";
            this.label12.Visible = false;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(15, 76);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(29, 13);
            this.label8.TabIndex = 150;
            this.label8.Text = "Print";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(573, 128);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(43, 13);
            this.label7.TabIndex = 148;
            this.label7.Text = "Trading";
            this.label7.Visible = false;
            // 
            // txtVehicleType
            // 
            this.txtVehicleType.Location = new System.Drawing.Point(104, 138);
            this.txtVehicleType.MaximumSize = new System.Drawing.Size(4, 4);
            this.txtVehicleType.MinimumSize = new System.Drawing.Size(185, 20);
            this.txtVehicleType.Name = "txtVehicleType";
            this.txtVehicleType.Size = new System.Drawing.Size(185, 20);
            this.txtVehicleType.TabIndex = 3;
            this.txtVehicleType.Visible = false;
            this.txtVehicleType.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtVehicleType_KeyDown);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(315, 138);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(42, 13);
            this.label5.TabIndex = 136;
            this.label5.Text = "Port To";
            this.label5.Visible = false;
            // 
            // txtCustomerName
            // 
            this.txtCustomerName.Location = new System.Drawing.Point(404, 48);
            this.txtCustomerName.MaximumSize = new System.Drawing.Size(4, 4);
            this.txtCustomerName.MinimumSize = new System.Drawing.Size(185, 20);
            this.txtCustomerName.Name = "txtCustomerName";
            this.txtCustomerName.Size = new System.Drawing.Size(185, 20);
            this.txtCustomerName.TabIndex = 3;
            this.txtCustomerName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtCustomerName_KeyDown);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(15, 139);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(67, 13);
            this.label16.TabIndex = 130;
            this.label16.Text = "Vehicle Type";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(315, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 13);
            this.label2.TabIndex = 128;
            this.label2.Text = "Customer Name";
            // 
            // txtPortTo
            // 
            this.txtPortTo.BackColor = System.Drawing.SystemColors.Window;
            this.txtPortTo.Location = new System.Drawing.Point(404, 135);
            this.txtPortTo.MaximumSize = new System.Drawing.Size(4, 5);
            this.txtPortTo.MinimumSize = new System.Drawing.Size(185, 20);
            this.txtPortTo.Name = "txtPortTo";
            this.txtPortTo.Size = new System.Drawing.Size(185, 20);
            this.txtPortTo.TabIndex = 2;
            this.txtPortTo.Visible = false;
            this.txtPortTo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtPortTo_KeyDown);
            // 
            // txtPortFrom
            // 
            this.txtPortFrom.BackColor = System.Drawing.SystemColors.Window;
            this.txtPortFrom.Location = new System.Drawing.Point(104, 135);
            this.txtPortFrom.MaximumSize = new System.Drawing.Size(4, 5);
            this.txtPortFrom.MinimumSize = new System.Drawing.Size(185, 20);
            this.txtPortFrom.Name = "txtPortFrom";
            this.txtPortFrom.Size = new System.Drawing.Size(185, 20);
            this.txtPortFrom.TabIndex = 1;
            this.txtPortFrom.Visible = false;
            this.txtPortFrom.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtPortFrom_KeyDown);
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
            this.label11.Location = new System.Drawing.Point(516, 22);
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
            this.dtpSaleToDate.Location = new System.Drawing.Point(537, 18);
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
            this.dtpSaleFromDate.Location = new System.Drawing.Point(404, 18);
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
            this.btnSearch.Location = new System.Drawing.Point(730, 40);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 28);
            this.btnSearch.TabIndex = 3;
            this.btnSearch.Text = "&Search";
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Visible = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(315, 22);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(58, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Sales Date";
            // 
            // txtInvoiceNo
            // 
            this.txtInvoiceNo.Location = new System.Drawing.Point(104, 18);
            this.txtInvoiceNo.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtInvoiceNo.MinimumSize = new System.Drawing.Size(185, 20);
            this.txtInvoiceNo.Name = "txtInvoiceNo";
            this.txtInvoiceNo.Size = new System.Drawing.Size(185, 21);
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
            // cmbType
            // 
            this.cmbType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbType.FormattingEnabled = true;
            this.cmbType.Items.AddRange(new object[] {
            "New",
            "Debit",
            "Credit"});
            this.cmbType.Location = new System.Drawing.Point(769, 123);
            this.cmbType.Name = "cmbType";
            this.cmbType.Size = new System.Drawing.Size(35, 21);
            this.cmbType.TabIndex = 1;
            this.cmbType.Visible = false;
            this.cmbType.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmbType_KeyDown);
            // 
            // cmbTrading
            // 
            this.cmbTrading.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTrading.FormattingEnabled = true;
            this.cmbTrading.Location = new System.Drawing.Point(730, -15);
            this.cmbTrading.Name = "cmbTrading";
            this.cmbTrading.Size = new System.Drawing.Size(125, 21);
            this.cmbTrading.TabIndex = 10;
            this.cmbTrading.Visible = false;
            // 
            // txtVehicleNo
            // 
            this.txtVehicleNo.Location = new System.Drawing.Point(772, 137);
            this.txtVehicleNo.MaximumSize = new System.Drawing.Size(4, 4);
            this.txtVehicleNo.MinimumSize = new System.Drawing.Size(20, 20);
            this.txtVehicleNo.Name = "txtVehicleNo";
            this.txtVehicleNo.Size = new System.Drawing.Size(20, 20);
            this.txtVehicleNo.TabIndex = 6;
            this.txtVehicleNo.Visible = false;
            this.txtVehicleNo.TextChanged += new System.EventHandler(this.txtVehicleNo_TextChanged);
            this.txtVehicleNo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtVehicleNo_KeyDown);
            // 
            // txtVatAmountTo
            // 
            this.txtVatAmountTo.Location = new System.Drawing.Point(712, 5);
            this.txtVatAmountTo.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtVatAmountTo.MinimumSize = new System.Drawing.Size(75, 20);
            this.txtVatAmountTo.Name = "txtVatAmountTo";
            this.txtVatAmountTo.Size = new System.Drawing.Size(80, 21);
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
            this.label13.Location = new System.Drawing.Point(691, 8);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(17, 13);
            this.label13.TabIndex = 114;
            this.label13.Text = "to";
            this.label13.Visible = false;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(501, 8);
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
            this.txtVehicleID.Visible = false;
            // 
            // txtCustomerID
            // 
            this.txtCustomerID.Location = new System.Drawing.Point(793, 131);
            this.txtCustomerID.MaximumSize = new System.Drawing.Size(4, 4);
            this.txtCustomerID.MinimumSize = new System.Drawing.Size(20, 20);
            this.txtCustomerID.Name = "txtCustomerID";
            this.txtCustomerID.Size = new System.Drawing.Size(20, 20);
            this.txtCustomerID.TabIndex = 127;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.progressBar1);
            this.groupBox1.Controls.Add(this.dgvSalesHistory);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Controls.Add(this.txtVatAmountTo);
            this.groupBox1.Controls.Add(this.cmbTrading);
            this.groupBox1.Location = new System.Drawing.Point(14, 132);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(757, 278);
            this.groupBox1.TabIndex = 120;
            this.groupBox1.TabStop = false;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(187, 121);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(288, 24);
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
            this.Select,
            this.SalesInvoiceNo,
            this.CustomerID,
            this.CustomerName,
            this.CustomerGroupName,
            this.DeliveryAddress1,
            this.DeliveryAddress2,
            this.DeliveryAddress3,
            this.VehicleID,
            this.VehicleType,
            this.VehicleNo,
            this.TotalAmount,
            this.TotalVATAmount,
            this.SerialNo,
            this.InvoiceDate,
            this.TenderId,
            this.Comments,
            this.SaleType,
            this.PID,
            this.Trading,
            this.IsPrint,
            this.Post,
            this.DeliveryDate,
            this.CurrencyID,
            this.CurrencyRateFromBDT,
            this.CurrencyCode,
            this.SaleReturnId,
            this.TransactionType,
            this.LCNumber,
            this.AlReadyPrint});
            this.dgvSalesHistory.Location = new System.Drawing.Point(5, 12);
            this.dgvSalesHistory.Name = "dgvSalesHistory";
            this.dgvSalesHistory.RowHeadersVisible = false;
            this.dgvSalesHistory.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvSalesHistory.Size = new System.Drawing.Size(744, 262);
            this.dgvSalesHistory.TabIndex = 6;
            this.dgvSalesHistory.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSalesHistory_CellContentClick);
            // 
            // Select
            // 
            this.Select.HeaderText = "Select";
            this.Select.Name = "Select";
            // 
            // SalesInvoiceNo
            // 
            this.SalesInvoiceNo.DataPropertyName = "SalesInvoiceNo";
            this.SalesInvoiceNo.HeaderText = "Invoice No";
            this.SalesInvoiceNo.Name = "SalesInvoiceNo";
            this.SalesInvoiceNo.ReadOnly = true;
            this.SalesInvoiceNo.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // CustomerID
            // 
            this.CustomerID.DataPropertyName = "CustomerID";
            this.CustomerID.HeaderText = "Customer ID";
            this.CustomerID.Name = "CustomerID";
            this.CustomerID.ReadOnly = true;
            this.CustomerID.Visible = false;
            // 
            // CustomerName
            // 
            this.CustomerName.DataPropertyName = "CustomerName";
            this.CustomerName.HeaderText = "CustomerName";
            this.CustomerName.Name = "CustomerName";
            this.CustomerName.ReadOnly = true;
            // 
            // CustomerGroupName
            // 
            this.CustomerGroupName.DataPropertyName = "CustomerGroupName";
            this.CustomerGroupName.HeaderText = "GroupName";
            this.CustomerGroupName.Name = "CustomerGroupName";
            this.CustomerGroupName.ReadOnly = true;
            // 
            // DeliveryAddress1
            // 
            this.DeliveryAddress1.DataPropertyName = "DeliveryAddress1";
            this.DeliveryAddress1.HeaderText = "Address1";
            this.DeliveryAddress1.Name = "DeliveryAddress1";
            this.DeliveryAddress1.ReadOnly = true;
            // 
            // DeliveryAddress2
            // 
            this.DeliveryAddress2.DataPropertyName = "DeliveryAddress2";
            this.DeliveryAddress2.HeaderText = "Address2";
            this.DeliveryAddress2.Name = "DeliveryAddress2";
            this.DeliveryAddress2.ReadOnly = true;
            this.DeliveryAddress2.Visible = false;
            // 
            // DeliveryAddress3
            // 
            this.DeliveryAddress3.DataPropertyName = "DeliveryAddress3";
            this.DeliveryAddress3.HeaderText = "Address3";
            this.DeliveryAddress3.Name = "DeliveryAddress3";
            this.DeliveryAddress3.ReadOnly = true;
            this.DeliveryAddress3.Visible = false;
            // 
            // VehicleID
            // 
            this.VehicleID.DataPropertyName = "VehicleID";
            this.VehicleID.HeaderText = "Vehicle ID";
            this.VehicleID.Name = "VehicleID";
            this.VehicleID.ReadOnly = true;
            this.VehicleID.Visible = false;
            // 
            // VehicleType
            // 
            this.VehicleType.DataPropertyName = "VehicleType";
            this.VehicleType.HeaderText = "VehicleType";
            this.VehicleType.Name = "VehicleType";
            this.VehicleType.ReadOnly = true;
            // 
            // VehicleNo
            // 
            this.VehicleNo.DataPropertyName = "VehicleNo";
            this.VehicleNo.HeaderText = "VehicleNo";
            this.VehicleNo.Name = "VehicleNo";
            this.VehicleNo.ReadOnly = true;
            // 
            // TotalAmount
            // 
            this.TotalAmount.DataPropertyName = "TotalAmount";
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.TotalAmount.DefaultCellStyle = dataGridViewCellStyle2;
            this.TotalAmount.HeaderText = "Grand Total";
            this.TotalAmount.Name = "TotalAmount";
            this.TotalAmount.ReadOnly = true;
            // 
            // TotalVATAmount
            // 
            this.TotalVATAmount.DataPropertyName = "TotalVATAmount";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.TotalVATAmount.DefaultCellStyle = dataGridViewCellStyle3;
            this.TotalVATAmount.HeaderText = "Total VAT";
            this.TotalVATAmount.Name = "TotalVATAmount";
            this.TotalVATAmount.ReadOnly = true;
            // 
            // SerialNo
            // 
            this.SerialNo.DataPropertyName = "SerialNo";
            this.SerialNo.HeaderText = "Ref Number";
            this.SerialNo.Name = "SerialNo";
            this.SerialNo.ReadOnly = true;
            // 
            // InvoiceDate
            // 
            this.InvoiceDate.DataPropertyName = "InvoiceDate";
            this.InvoiceDate.HeaderText = "Sales Date";
            this.InvoiceDate.Name = "InvoiceDate";
            this.InvoiceDate.ReadOnly = true;
            // 
            // TenderId
            // 
            this.TenderId.DataPropertyName = "TenderId";
            this.TenderId.HeaderText = "TenderId";
            this.TenderId.Name = "TenderId";
            this.TenderId.ReadOnly = true;
            this.TenderId.Visible = false;
            // 
            // Comments
            // 
            this.Comments.DataPropertyName = "Comments";
            this.Comments.HeaderText = "Comments";
            this.Comments.Name = "Comments";
            this.Comments.ReadOnly = true;
            this.Comments.Visible = false;
            // 
            // SaleType
            // 
            this.SaleType.DataPropertyName = "SaleType";
            this.SaleType.HeaderText = "Sale Type";
            this.SaleType.Name = "SaleType";
            // 
            // PID
            // 
            this.PID.DataPropertyName = "PID";
            this.PID.HeaderText = "PID";
            this.PID.Name = "PID";
            // 
            // Trading
            // 
            this.Trading.DataPropertyName = "Trading";
            this.Trading.HeaderText = "Trading";
            this.Trading.Name = "Trading";
            this.Trading.Visible = false;
            // 
            // IsPrint
            // 
            this.IsPrint.DataPropertyName = "IsPrint";
            this.IsPrint.HeaderText = "Is Print";
            this.IsPrint.Name = "IsPrint";
            // 
            // Post
            // 
            this.Post.DataPropertyName = "Post";
            this.Post.HeaderText = "Post";
            this.Post.Name = "Post";
            this.Post.ReadOnly = true;
            // 
            // DeliveryDate
            // 
            this.DeliveryDate.HeaderText = "DeliveryDate";
            this.DeliveryDate.Name = "DeliveryDate";
            this.DeliveryDate.ReadOnly = true;
            // 
            // CurrencyID
            // 
            this.CurrencyID.HeaderText = "CurrencyID";
            this.CurrencyID.Name = "CurrencyID";
            // 
            // CurrencyRateFromBDT
            // 
            this.CurrencyRateFromBDT.HeaderText = "CurrencyRateFromBDT";
            this.CurrencyRateFromBDT.Name = "CurrencyRateFromBDT";
            this.CurrencyRateFromBDT.ReadOnly = true;
            // 
            // CurrencyCode
            // 
            this.CurrencyCode.HeaderText = "CurrencyCode";
            this.CurrencyCode.Name = "CurrencyCode";
            this.CurrencyCode.ReadOnly = true;
            // 
            // SaleReturnId
            // 
            this.SaleReturnId.HeaderText = "DollerRate";
            this.SaleReturnId.Name = "SaleReturnId";
            this.SaleReturnId.ReadOnly = true;
            // 
            // TransactionType
            // 
            this.TransactionType.HeaderText = "transactionType";
            this.TransactionType.Name = "TransactionType";
            this.TransactionType.ReadOnly = true;
            this.TransactionType.Visible = false;
            // 
            // LCNumber
            // 
            this.LCNumber.HeaderText = "LCNumber";
            this.LCNumber.Name = "LCNumber";
            this.LCNumber.ReadOnly = true;
            // 
            // AlReadyPrint
            // 
            this.AlReadyPrint.HeaderText = "AlReadyPrint";
            this.AlReadyPrint.Name = "AlReadyPrint";
            this.AlReadyPrint.ReadOnly = true;
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::VATClient.Properties.Resources.Green_Footer;
            this.panel1.Controls.Add(this.btnClose);
            this.panel1.Controls.Add(this.LRecordCount);
            this.panel1.Controls.Add(this.btnPrint);
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Location = new System.Drawing.Point(-1, 423);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(793, 40);
            this.panel1.TabIndex = 11;
            this.panel1.TabStop = true;
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Image = global::VATClient.Properties.Resources.Back;
            this.btnClose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnClose.Location = new System.Drawing.Point(672, 6);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(99, 28);
            this.btnClose.TabIndex = 63;
            this.btnClose.Text = "&Close / OK";
            this.btnClose.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // LRecordCount
            // 
            this.LRecordCount.AutoSize = true;
            this.LRecordCount.Location = new System.Drawing.Point(119, 14);
            this.LRecordCount.Name = "LRecordCount";
            this.LRecordCount.Size = new System.Drawing.Size(107, 13);
            this.LRecordCount.TabIndex = 14;
            this.LRecordCount.Text = "Total Record Count: ";
            // 
            // btnPrint
            // 
            this.btnPrint.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnPrint.Image = global::VATClient.Properties.Resources.Print;
            this.btnPrint.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPrint.Location = new System.Drawing.Point(364, 6);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(53, 28);
            this.btnPrint.TabIndex = 12;
            this.btnPrint.Text = "&Print";
            this.btnPrint.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnPrint.UseVisualStyleBackColor = false;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnCancel.Image = global::VATClient.Properties.Resources.Referesh;
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.Location = new System.Drawing.Point(17, 5);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 28);
            this.btnCancel.TabIndex = 12;
            this.btnCancel.Text = "&Refresh";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // bgwSearch
            // 
            this.bgwSearch.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwSearch_DoWork);
            this.bgwSearch.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwSearch_RunWorkerCompleted);
            // 
            // FormSaleVAT20Multiple
            // 
            this.AcceptButton = this.btnSearch;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(784, 462);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.grbTransactionHistory);
            this.Controls.Add(this.txtCustomerGroupName);
            this.Controls.Add(this.cmbType);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.txtCustomerGroupID);
            this.Controls.Add(this.txtVehicleID);
            this.Controls.Add(this.txtCustomerID);
            this.Controls.Add(this.txtVehicleNo);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(800, 500);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(800, 500);
            this.Name = "FormSaleVAT20Multiple";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Multiple Invoice ";
            this.Load += new System.EventHandler(this.FormSaleVAT20Multiple_Load);
            this.grbTransactionHistory.ResumeLayout(false);
            this.grbTransactionHistory.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSalesHistory)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
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
        private System.Windows.Forms.DataGridViewTextBoxColumn SalesInvoiceNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn CustomerID;
        private System.Windows.Forms.DataGridViewTextBoxColumn CustomerName;
        private System.Windows.Forms.DataGridViewTextBoxColumn CustomerGroupName;
        private System.Windows.Forms.DataGridViewTextBoxColumn DeliveryAddress1;
        private System.Windows.Forms.DataGridViewTextBoxColumn DeliveryAddress2;
        private System.Windows.Forms.DataGridViewTextBoxColumn DeliveryAddress3;
        private System.Windows.Forms.DataGridViewTextBoxColumn VehicleID;
        private System.Windows.Forms.DataGridViewTextBoxColumn VehicleType;
        private System.Windows.Forms.DataGridViewTextBoxColumn VehicleNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn TotalAmount;
        private System.Windows.Forms.DataGridViewTextBoxColumn TotalVATAmount;
        private System.Windows.Forms.DataGridViewTextBoxColumn SerialNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn InvoiceDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn TenderId;
        private System.Windows.Forms.DataGridViewTextBoxColumn Comments;
        private System.Windows.Forms.DataGridViewTextBoxColumn SaleType;
        private System.Windows.Forms.DataGridViewTextBoxColumn PID;
        private System.Windows.Forms.DataGridViewTextBoxColumn Trading;
        private System.Windows.Forms.DataGridViewTextBoxColumn IsPrint;
        private System.Windows.Forms.DataGridViewTextBoxColumn Post;
        private System.ComponentModel.BackgroundWorker bgwSearch;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.DataGridViewTextBoxColumn DeliveryDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn CurrencyID;
        private System.Windows.Forms.DataGridViewTextBoxColumn CurrencyRateFromBDT;
        private System.Windows.Forms.DataGridViewTextBoxColumn CurrencyCode;
        private System.Windows.Forms.Label LRecordCount;
        private System.Windows.Forms.DataGridViewTextBoxColumn SaleReturnId;
        private System.Windows.Forms.DataGridViewTextBoxColumn TransactionType;
        private System.Windows.Forms.DataGridViewTextBoxColumn LCNumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn AlReadyPrint;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Select;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.TextBox txtPortTo;
        private System.Windows.Forms.TextBox txtPortFrom;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox txtEXPFormNo;
        private System.Windows.Forms.Label label14;
    }
}