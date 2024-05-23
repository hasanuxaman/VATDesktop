namespace VATClient
{
    partial class FormPurchaseSearch
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPurchaseSearch));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkSelectAll = new System.Windows.Forms.CheckBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.dgvPurchaseHistory = new System.Windows.Forms.DataGridView();
            this.Select = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.pnlHidden = new System.Windows.Forms.Panel();
            this.txtVatAmountTo = new System.Windows.Forms.TextBox();
            this.txtVatAmountFrom = new System.Windows.Forms.TextBox();
            this.btnDutyDownload = new System.Windows.Forms.Button();
            this.txtVendorID = new System.Windows.Forms.TextBox();
            this.txtVendorGroupID = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtGrandTotalTo = new System.Windows.Forms.TextBox();
            this.txtGrandTotalFrom = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.btnAdd = new System.Windows.Forms.Button();
            this.grbTransactionHistory = new System.Windows.Forms.GroupBox();
            this.btnMltSave = new System.Windows.Forms.Button();
            this.btnBankChannel = new System.Windows.Forms.Button();
            this.cmbIsBankingChannelPay = new System.Windows.Forms.ComboBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.cmbIsRebate = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnRebate = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.dtpRebateDate = new System.Windows.Forms.DateTimePicker();
            this.chkDuty = new System.Windows.Forms.CheckBox();
            this.btnVendorGroupRefresh = new System.Windows.Forms.Button();
            this.btnVendorRefresh = new System.Windows.Forms.Button();
            this.cmbRecordCount = new System.Windows.Forms.ComboBox();
            this.cmbTds = new System.Windows.Forms.ComboBox();
            this.lblTds = new System.Windows.Forms.Label();
            this.cmbBranch = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnPost = new System.Windows.Forms.Button();
            this.LVDS = new System.Windows.Forms.Label();
            this.cmbVDS = new System.Windows.Forms.ComboBox();
            this.cmbPost = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtBENumber = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtVendorGroupName = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.txtVendorName = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.txtSerialNo = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.dtpPurchaseToDate = new System.Windows.Forms.DateTimePicker();
            this.dtpPurchaseFromDate = new System.Windows.Forms.DateTimePicker();
            this.btnSearch = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.txtInvoiceNo = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.bgwPurchaseSearch = new System.ComponentModel.BackgroundWorker();
            this.bgwPurchaseSearch2 = new System.ComponentModel.BackgroundWorker();
            this.bgwMultiplePost = new System.ComponentModel.BackgroundWorker();
            this.bgwMultipleRebate = new System.ComponentModel.BackgroundWorker();
            this.btnOk = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnExportTDS = new System.Windows.Forms.Button();
            this.btnBankChannelMIS = new System.Windows.Forms.Button();
            this.btnMultiple = new System.Windows.Forms.Button();
            this.LRecordCount = new System.Windows.Forms.Label();
            this.cmbExport = new System.Windows.Forms.ComboBox();
            this.btnExport = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rbtnBankChannelPayment = new System.Windows.Forms.RadioButton();
            this.backgroundWorkerMIS = new System.ComponentModel.BackgroundWorker();
            this.label22 = new System.Windows.Forms.Label();
            this.dateTimePeriodIdPicker = new System.Windows.Forms.DateTimePicker();
            this.SyncHSCode = new System.Windows.Forms.Button();
            this.bgwSyncHSCode = new System.ComponentModel.BackgroundWorker();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPurchaseHistory)).BeginInit();
            this.pnlHidden.SuspendLayout();
            this.grbTransactionHistory.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkSelectAll);
            this.groupBox1.Controls.Add(this.progressBar1);
            this.groupBox1.Controls.Add(this.dgvPurchaseHistory);
            this.groupBox1.Controls.Add(this.pnlHidden);
            this.groupBox1.Location = new System.Drawing.Point(12, 178);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(758, 239);
            this.groupBox1.TabIndex = 112;
            this.groupBox1.TabStop = false;
            // 
            // chkSelectAll
            // 
            this.chkSelectAll.AutoSize = true;
            this.chkSelectAll.Location = new System.Drawing.Point(6, -1);
            this.chkSelectAll.Name = "chkSelectAll";
            this.chkSelectAll.Size = new System.Drawing.Size(66, 17);
            this.chkSelectAll.TabIndex = 206;
            this.chkSelectAll.Text = "SelectAll";
            this.chkSelectAll.UseVisualStyleBackColor = true;
            this.chkSelectAll.CheckedChanged += new System.EventHandler(this.chkSelectAll_CheckedChanged);
            this.chkSelectAll.Click += new System.EventHandler(this.chkSelectAll_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(233, 126);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(290, 24);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 204;
            this.progressBar1.Visible = false;
            this.progressBar1.Click += new System.EventHandler(this.progressBar1_Click);
            // 
            // dgvPurchaseHistory
            // 
            this.dgvPurchaseHistory.AllowUserToAddRows = false;
            this.dgvPurchaseHistory.AllowUserToDeleteRows = false;
            dataGridViewCellStyle10.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.dgvPurchaseHistory.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle10;
            this.dgvPurchaseHistory.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvPurchaseHistory.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPurchaseHistory.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Select});
            this.dgvPurchaseHistory.Location = new System.Drawing.Point(2, 16);
            this.dgvPurchaseHistory.Name = "dgvPurchaseHistory";
            this.dgvPurchaseHistory.RowHeadersVisible = false;
            this.dgvPurchaseHistory.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvPurchaseHistory.Size = new System.Drawing.Size(753, 217);
            this.dgvPurchaseHistory.TabIndex = 6;
            this.dgvPurchaseHistory.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvPurchaseHistory_CellContentClick);
            this.dgvPurchaseHistory.DragDrop += new System.Windows.Forms.DragEventHandler(this.dgvPurchaseHistory_DragDrop);
            this.dgvPurchaseHistory.DoubleClick += new System.EventHandler(this.dgvPurchaseHistory_DoubleClick);
            // 
            // Select
            // 
            this.Select.HeaderText = "Select";
            this.Select.Name = "Select";
            this.Select.Width = 42;
            // 
            // pnlHidden
            // 
            this.pnlHidden.Controls.Add(this.txtVatAmountTo);
            this.pnlHidden.Controls.Add(this.txtVatAmountFrom);
            this.pnlHidden.Controls.Add(this.btnDutyDownload);
            this.pnlHidden.Controls.Add(this.txtVendorID);
            this.pnlHidden.Controls.Add(this.txtVendorGroupID);
            this.pnlHidden.Controls.Add(this.label9);
            this.pnlHidden.Controls.Add(this.txtGrandTotalTo);
            this.pnlHidden.Controls.Add(this.txtGrandTotalFrom);
            this.pnlHidden.Controls.Add(this.label10);
            this.pnlHidden.Controls.Add(this.btnAdd);
            this.pnlHidden.Location = new System.Drawing.Point(6, 130);
            this.pnlHidden.Name = "pnlHidden";
            this.pnlHidden.Size = new System.Drawing.Size(230, 100);
            this.pnlHidden.TabIndex = 207;
            this.pnlHidden.Visible = false;
            // 
            // txtVatAmountTo
            // 
            this.txtVatAmountTo.Location = new System.Drawing.Point(6, 9);
            this.txtVatAmountTo.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtVatAmountTo.Name = "txtVatAmountTo";
            this.txtVatAmountTo.Size = new System.Drawing.Size(20, 21);
            this.txtVatAmountTo.TabIndex = 8;
            this.txtVatAmountTo.Text = "0.00";
            this.txtVatAmountTo.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtVatAmountTo.Visible = false;
            this.txtVatAmountTo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtVatAmountTo_KeyDown);
            this.txtVatAmountTo.Leave += new System.EventHandler(this.txtVatAmountTo_Leave);
            // 
            // txtVatAmountFrom
            // 
            this.txtVatAmountFrom.Location = new System.Drawing.Point(31, 9);
            this.txtVatAmountFrom.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtVatAmountFrom.Name = "txtVatAmountFrom";
            this.txtVatAmountFrom.Size = new System.Drawing.Size(20, 21);
            this.txtVatAmountFrom.TabIndex = 7;
            this.txtVatAmountFrom.Text = "0.00";
            this.txtVatAmountFrom.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtVatAmountFrom.Visible = false;
            this.txtVatAmountFrom.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtVatAmountFrom_KeyDown);
            this.txtVatAmountFrom.Leave += new System.EventHandler(this.txtVatAmountFrom_Leave);
            // 
            // btnDutyDownload
            // 
            this.btnDutyDownload.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnDutyDownload.Image = global::VATClient.Properties.Resources.search;
            this.btnDutyDownload.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDutyDownload.Location = new System.Drawing.Point(6, 35);
            this.btnDutyDownload.Name = "btnDutyDownload";
            this.btnDutyDownload.Size = new System.Drawing.Size(110, 28);
            this.btnDutyDownload.TabIndex = 204;
            this.btnDutyDownload.Text = "&Duty Download";
            this.btnDutyDownload.UseVisualStyleBackColor = false;
            this.btnDutyDownload.Visible = false;
            this.btnDutyDownload.Click += new System.EventHandler(this.btnDutyDownload_Click);
            // 
            // txtVendorID
            // 
            this.txtVendorID.Location = new System.Drawing.Point(6, 68);
            this.txtVendorID.Name = "txtVendorID";
            this.txtVendorID.Size = new System.Drawing.Size(18, 21);
            this.txtVendorID.TabIndex = 236;
            this.txtVendorID.Visible = false;
            // 
            // txtVendorGroupID
            // 
            this.txtVendorGroupID.Location = new System.Drawing.Point(28, 68);
            this.txtVendorGroupID.MaximumSize = new System.Drawing.Size(4, 4);
            this.txtVendorGroupID.MinimumSize = new System.Drawing.Size(20, 20);
            this.txtVendorGroupID.Name = "txtVendorGroupID";
            this.txtVendorGroupID.Size = new System.Drawing.Size(20, 20);
            this.txtVendorGroupID.TabIndex = 131;
            this.txtVendorGroupID.Visible = false;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(61, 12);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(63, 13);
            this.label9.TabIndex = 7;
            this.label9.Text = "Grand Total";
            this.label9.Visible = false;
            // 
            // txtGrandTotalTo
            // 
            this.txtGrandTotalTo.Location = new System.Drawing.Point(78, 68);
            this.txtGrandTotalTo.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtGrandTotalTo.MinimumSize = new System.Drawing.Size(20, 20);
            this.txtGrandTotalTo.Name = "txtGrandTotalTo";
            this.txtGrandTotalTo.Size = new System.Drawing.Size(20, 21);
            this.txtGrandTotalTo.TabIndex = 10;
            this.txtGrandTotalTo.Text = "0.00";
            this.txtGrandTotalTo.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtGrandTotalTo.Visible = false;
            this.txtGrandTotalTo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtGrandTotalTo_KeyDown);
            this.txtGrandTotalTo.Leave += new System.EventHandler(this.txtGrandTotalTo_Leave);
            // 
            // txtGrandTotalFrom
            // 
            this.txtGrandTotalFrom.Location = new System.Drawing.Point(53, 68);
            this.txtGrandTotalFrom.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtGrandTotalFrom.MinimumSize = new System.Drawing.Size(20, 20);
            this.txtGrandTotalFrom.Name = "txtGrandTotalFrom";
            this.txtGrandTotalFrom.Size = new System.Drawing.Size(20, 21);
            this.txtGrandTotalFrom.TabIndex = 9;
            this.txtGrandTotalFrom.Text = "0.00";
            this.txtGrandTotalFrom.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtGrandTotalFrom.Visible = false;
            this.txtGrandTotalFrom.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtGrandTotalFrom_KeyDown);
            this.txtGrandTotalFrom.Leave += new System.EventHandler(this.txtGrandTotalFrom_Leave);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(132, 12);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(93, 13);
            this.label10.TabIndex = 6;
            this.label10.Text = "Total VAT Amount";
            this.label10.Visible = false;
            // 
            // btnAdd
            // 
            this.btnAdd.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnAdd.Image = global::VATClient.Properties.Resources.add_over;
            this.btnAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAdd.Location = new System.Drawing.Point(122, 35);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 28);
            this.btnAdd.TabIndex = 10;
            this.btnAdd.Text = "&Add";
            this.btnAdd.UseVisualStyleBackColor = false;
            this.btnAdd.Visible = false;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // grbTransactionHistory
            // 
            this.grbTransactionHistory.Controls.Add(this.SyncHSCode);
            this.grbTransactionHistory.Controls.Add(this.dateTimePeriodIdPicker);
            this.grbTransactionHistory.Controls.Add(this.label22);
            this.grbTransactionHistory.Controls.Add(this.btnMltSave);
            this.grbTransactionHistory.Controls.Add(this.btnBankChannel);
            this.grbTransactionHistory.Controls.Add(this.cmbIsBankingChannelPay);
            this.grbTransactionHistory.Controls.Add(this.label12);
            this.grbTransactionHistory.Controls.Add(this.label18);
            this.grbTransactionHistory.Controls.Add(this.cmbIsRebate);
            this.grbTransactionHistory.Controls.Add(this.label8);
            this.grbTransactionHistory.Controls.Add(this.btnCancel);
            this.grbTransactionHistory.Controls.Add(this.btnRebate);
            this.grbTransactionHistory.Controls.Add(this.label7);
            this.grbTransactionHistory.Controls.Add(this.dtpRebateDate);
            this.grbTransactionHistory.Controls.Add(this.chkDuty);
            this.grbTransactionHistory.Controls.Add(this.btnVendorGroupRefresh);
            this.grbTransactionHistory.Controls.Add(this.btnVendorRefresh);
            this.grbTransactionHistory.Controls.Add(this.cmbRecordCount);
            this.grbTransactionHistory.Controls.Add(this.cmbTds);
            this.grbTransactionHistory.Controls.Add(this.lblTds);
            this.grbTransactionHistory.Controls.Add(this.cmbBranch);
            this.grbTransactionHistory.Controls.Add(this.label2);
            this.grbTransactionHistory.Controls.Add(this.btnPost);
            this.grbTransactionHistory.Controls.Add(this.LVDS);
            this.grbTransactionHistory.Controls.Add(this.cmbVDS);
            this.grbTransactionHistory.Controls.Add(this.cmbPost);
            this.grbTransactionHistory.Controls.Add(this.label5);
            this.grbTransactionHistory.Controls.Add(this.txtBENumber);
            this.grbTransactionHistory.Controls.Add(this.label4);
            this.grbTransactionHistory.Controls.Add(this.txtVendorGroupName);
            this.grbTransactionHistory.Controls.Add(this.label16);
            this.grbTransactionHistory.Controls.Add(this.txtVendorName);
            this.grbTransactionHistory.Controls.Add(this.label15);
            this.grbTransactionHistory.Controls.Add(this.txtSerialNo);
            this.grbTransactionHistory.Controls.Add(this.label6);
            this.grbTransactionHistory.Controls.Add(this.label11);
            this.grbTransactionHistory.Controls.Add(this.dtpPurchaseToDate);
            this.grbTransactionHistory.Controls.Add(this.dtpPurchaseFromDate);
            this.grbTransactionHistory.Controls.Add(this.btnSearch);
            this.grbTransactionHistory.Controls.Add(this.label3);
            this.grbTransactionHistory.Controls.Add(this.txtInvoiceNo);
            this.grbTransactionHistory.Controls.Add(this.label1);
            this.grbTransactionHistory.Location = new System.Drawing.Point(14, 1);
            this.grbTransactionHistory.Name = "grbTransactionHistory";
            this.grbTransactionHistory.Size = new System.Drawing.Size(756, 184);
            this.grbTransactionHistory.TabIndex = 111;
            this.grbTransactionHistory.TabStop = false;
            this.grbTransactionHistory.Text = "Searching Criteria";
            // 
            // btnMltSave
            // 
            this.btnMltSave.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnMltSave.Image = global::VATClient.Properties.Resources.Update;
            this.btnMltSave.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnMltSave.Location = new System.Drawing.Point(520, 135);
            this.btnMltSave.Name = "btnMltSave";
            this.btnMltSave.Size = new System.Drawing.Size(75, 28);
            this.btnMltSave.TabIndex = 248;
            this.btnMltSave.Text = "&Multiple";
            this.btnMltSave.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnMltSave.UseVisualStyleBackColor = false;
            this.btnMltSave.Click += new System.EventHandler(this.btnMltSave_Click);
            // 
            // btnBankChannel
            // 
            this.btnBankChannel.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnBankChannel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnBankChannel.Image = global::VATClient.Properties.Resources.Back;
            this.btnBankChannel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnBankChannel.Location = new System.Drawing.Point(406, 135);
            this.btnBankChannel.Name = "btnBankChannel";
            this.btnBankChannel.Size = new System.Drawing.Size(105, 28);
            this.btnBankChannel.TabIndex = 233;
            this.btnBankChannel.Text = "&Bank Channel";
            this.btnBankChannel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnBankChannel.UseVisualStyleBackColor = false;
            this.btnBankChannel.Visible = false;
            this.btnBankChannel.Click += new System.EventHandler(this.btnBankChannel_Click);
            // 
            // cmbIsBankingChannelPay
            // 
            this.cmbIsBankingChannelPay.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbIsBankingChannelPay.FormattingEnabled = true;
            this.cmbIsBankingChannelPay.Items.AddRange(new object[] {
            "Y",
            "N"});
            this.cmbIsBankingChannelPay.Location = new System.Drawing.Point(617, 111);
            this.cmbIsBankingChannelPay.Name = "cmbIsBankingChannelPay";
            this.cmbIsBankingChannelPay.Size = new System.Drawing.Size(80, 21);
            this.cmbIsBankingChannelPay.TabIndex = 246;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(572, 115);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(44, 13);
            this.label12.TabIndex = 247;
            this.label12.Text = "Banking";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(88, 114);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(25, 13);
            this.label18.TabIndex = 245;
            this.label18.Text = "Top";
            // 
            // cmbIsRebate
            // 
            this.cmbIsRebate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbIsRebate.FormattingEnabled = true;
            this.cmbIsRebate.Items.AddRange(new object[] {
            "Y",
            "N"});
            this.cmbIsRebate.Location = new System.Drawing.Point(569, 85);
            this.cmbIsRebate.Name = "cmbIsRebate";
            this.cmbIsRebate.Size = new System.Drawing.Size(80, 21);
            this.cmbIsRebate.TabIndex = 243;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(530, 89);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(42, 13);
            this.label8.TabIndex = 244;
            this.label8.Text = "Rebate";
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnCancel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancel.Image = global::VATClient.Properties.Resources.Referesh;
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.Location = new System.Drawing.Point(678, 43);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 28);
            this.btnCancel.TabIndex = 12;
            this.btnCancel.Text = "&Refresh";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnRebate
            // 
            this.btnRebate.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnRebate.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRebate.Image = global::VATClient.Properties.Resources.Post;
            this.btnRebate.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnRebate.Location = new System.Drawing.Point(678, 135);
            this.btnRebate.Name = "btnRebate";
            this.btnRebate.Size = new System.Drawing.Size(75, 28);
            this.btnRebate.TabIndex = 242;
            this.btnRebate.Text = "Rebate";
            this.btnRebate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnRebate.UseVisualStyleBackColor = false;
            this.btnRebate.Click += new System.EventHandler(this.btnRebate_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(374, 114);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(68, 13);
            this.label7.TabIndex = 241;
            this.label7.Text = "Rebate Date";
            // 
            // dtpRebateDate
            // 
            this.dtpRebateDate.Checked = false;
            this.dtpRebateDate.CustomFormat = "MMMM-yyyy";
            this.dtpRebateDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpRebateDate.Location = new System.Drawing.Point(449, 110);
            this.dtpRebateDate.Name = "dtpRebateDate";
            this.dtpRebateDate.Size = new System.Drawing.Size(117, 21);
            this.dtpRebateDate.TabIndex = 240;
            // 
            // chkDuty
            // 
            this.chkDuty.AutoSize = true;
            this.chkDuty.Checked = true;
            this.chkDuty.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkDuty.Location = new System.Drawing.Point(206, 112);
            this.chkDuty.Name = "chkDuty";
            this.chkDuty.Size = new System.Drawing.Size(81, 17);
            this.chkDuty.TabIndex = 239;
            this.chkDuty.Text = "With Duties";
            this.chkDuty.UseVisualStyleBackColor = true;
            // 
            // btnVendorGroupRefresh
            // 
            this.btnVendorGroupRefresh.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnVendorGroupRefresh.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnVendorGroupRefresh.Image = ((System.Drawing.Image)(resources.GetObject("btnVendorGroupRefresh.Image")));
            this.btnVendorGroupRefresh.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnVendorGroupRefresh.Location = new System.Drawing.Point(319, 60);
            this.btnVendorGroupRefresh.Name = "btnVendorGroupRefresh";
            this.btnVendorGroupRefresh.Size = new System.Drawing.Size(25, 22);
            this.btnVendorGroupRefresh.TabIndex = 238;
            this.btnVendorGroupRefresh.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnVendorGroupRefresh.UseVisualStyleBackColor = false;
            this.btnVendorGroupRefresh.Click += new System.EventHandler(this.btnVendorGroupRefresh_Click);
            // 
            // btnVendorRefresh
            // 
            this.btnVendorRefresh.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnVendorRefresh.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnVendorRefresh.Image = ((System.Drawing.Image)(resources.GetObject("btnVendorRefresh.Image")));
            this.btnVendorRefresh.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnVendorRefresh.Location = new System.Drawing.Point(319, 38);
            this.btnVendorRefresh.Name = "btnVendorRefresh";
            this.btnVendorRefresh.Size = new System.Drawing.Size(25, 22);
            this.btnVendorRefresh.TabIndex = 237;
            this.btnVendorRefresh.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnVendorRefresh.UseVisualStyleBackColor = false;
            this.btnVendorRefresh.Click += new System.EventHandler(this.btnVendorRefresh_Click);
            // 
            // cmbRecordCount
            // 
            this.cmbRecordCount.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRecordCount.FormattingEnabled = true;
            this.cmbRecordCount.Location = new System.Drawing.Point(114, 110);
            this.cmbRecordCount.Name = "cmbRecordCount";
            this.cmbRecordCount.Size = new System.Drawing.Size(80, 21);
            this.cmbRecordCount.TabIndex = 235;
            // 
            // cmbTds
            // 
            this.cmbTds.CausesValidation = false;
            this.cmbTds.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTds.FormattingEnabled = true;
            this.cmbTds.Items.AddRange(new object[] {
            "Y",
            "N"});
            this.cmbTds.Location = new System.Drawing.Point(234, 85);
            this.cmbTds.Name = "cmbTds";
            this.cmbTds.Size = new System.Drawing.Size(80, 21);
            this.cmbTds.TabIndex = 228;
            // 
            // lblTds
            // 
            this.lblTds.AutoSize = true;
            this.lblTds.Location = new System.Drawing.Point(206, 89);
            this.lblTds.Name = "lblTds";
            this.lblTds.Size = new System.Drawing.Size(26, 13);
            this.lblTds.TabIndex = 229;
            this.lblTds.Text = "TDS";
            // 
            // cmbBranch
            // 
            this.cmbBranch.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBranch.FormattingEnabled = true;
            this.cmbBranch.Items.AddRange(new object[] {
            "Y",
            "N"});
            this.cmbBranch.Location = new System.Drawing.Point(114, 135);
            this.cmbBranch.Name = "cmbBranch";
            this.cmbBranch.Size = new System.Drawing.Size(200, 21);
            this.cmbBranch.TabIndex = 227;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(73, 139);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 13);
            this.label2.TabIndex = 226;
            this.label2.Text = "Branch";
            // 
            // btnPost
            // 
            this.btnPost.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnPost.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPost.Image = global::VATClient.Properties.Resources.Post;
            this.btnPost.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPost.Location = new System.Drawing.Point(601, 135);
            this.btnPost.Name = "btnPost";
            this.btnPost.Size = new System.Drawing.Size(75, 28);
            this.btnPost.TabIndex = 205;
            this.btnPost.Text = "Post";
            this.btnPost.UseVisualStyleBackColor = false;
            this.btnPost.Click += new System.EventHandler(this.btnPost_Click);
            // 
            // LVDS
            // 
            this.LVDS.AutoSize = true;
            this.LVDS.Location = new System.Drawing.Point(421, 89);
            this.LVDS.Name = "LVDS";
            this.LVDS.Size = new System.Drawing.Size(26, 13);
            this.LVDS.TabIndex = 203;
            this.LVDS.Text = "VDS";
            // 
            // cmbVDS
            // 
            this.cmbVDS.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbVDS.FormattingEnabled = true;
            this.cmbVDS.Items.AddRange(new object[] {
            "Y",
            "N"});
            this.cmbVDS.Location = new System.Drawing.Point(449, 85);
            this.cmbVDS.Name = "cmbVDS";
            this.cmbVDS.Size = new System.Drawing.Size(80, 21);
            this.cmbVDS.TabIndex = 8;
            // 
            // cmbPost
            // 
            this.cmbPost.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPost.FormattingEnabled = true;
            this.cmbPost.Items.AddRange(new object[] {
            "Y",
            "N"});
            this.cmbPost.Location = new System.Drawing.Point(114, 85);
            this.cmbPost.Name = "cmbPost";
            this.cmbPost.Size = new System.Drawing.Size(80, 21);
            this.cmbPost.TabIndex = 7;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(85, 89);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(28, 13);
            this.label5.TabIndex = 200;
            this.label5.Text = "Post";
            // 
            // txtBENumber
            // 
            this.txtBENumber.BackColor = System.Drawing.SystemColors.Window;
            this.txtBENumber.Location = new System.Drawing.Point(449, 61);
            this.txtBENumber.Name = "txtBENumber";
            this.txtBENumber.Size = new System.Drawing.Size(200, 21);
            this.txtBENumber.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(408, 65);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(39, 13);
            this.label4.TabIndex = 180;
            this.label4.Text = "BE No.";
            // 
            // txtVendorGroupName
            // 
            this.txtVendorGroupName.Location = new System.Drawing.Point(114, 61);
            this.txtVendorGroupName.Name = "txtVendorGroupName";
            this.txtVendorGroupName.Size = new System.Drawing.Size(200, 21);
            this.txtVendorGroupName.TabIndex = 2;
            this.txtVendorGroupName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtVendorGroupName_KeyDown);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(17, 65);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(96, 13);
            this.label16.TabIndex = 130;
            this.label16.Text = "Vendor Group (F9)";
            // 
            // txtVendorName
            // 
            this.txtVendorName.Location = new System.Drawing.Point(114, 39);
            this.txtVendorName.Name = "txtVendorName";
            this.txtVendorName.Size = new System.Drawing.Size(200, 21);
            this.txtVendorName.TabIndex = 1;
            this.txtVendorName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtVendorName_KeyDown);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(52, 43);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(61, 13);
            this.label15.TabIndex = 128;
            this.label15.Text = "Vendor(F9)";
            // 
            // txtSerialNo
            // 
            this.txtSerialNo.BackColor = System.Drawing.SystemColors.Window;
            this.txtSerialNo.Location = new System.Drawing.Point(449, 39);
            this.txtSerialNo.Name = "txtSerialNo";
            this.txtSerialNo.Size = new System.Drawing.Size(200, 21);
            this.txtSerialNo.TabIndex = 4;
            this.txtSerialNo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSerialNo_KeyDown);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(403, 43);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(44, 13);
            this.label6.TabIndex = 126;
            this.label6.Text = "Ref No.";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(564, 42);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(17, 13);
            this.label11.TabIndex = 112;
            this.label11.Text = "to";
            // 
            // dtpPurchaseToDate
            // 
            this.dtpPurchaseToDate.Checked = false;
            this.dtpPurchaseToDate.CustomFormat = "dd/MMM/yyyy";
            this.dtpPurchaseToDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpPurchaseToDate.Location = new System.Drawing.Point(556, 16);
            this.dtpPurchaseToDate.Name = "dtpPurchaseToDate";
            this.dtpPurchaseToDate.ShowCheckBox = true;
            this.dtpPurchaseToDate.Size = new System.Drawing.Size(102, 21);
            this.dtpPurchaseToDate.TabIndex = 6;
            this.dtpPurchaseToDate.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dtpPurchaseToDate_KeyDown);
            // 
            // dtpPurchaseFromDate
            // 
            this.dtpPurchaseFromDate.Checked = false;
            this.dtpPurchaseFromDate.CustomFormat = "dd/MMM/yyyy";
            this.dtpPurchaseFromDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpPurchaseFromDate.Location = new System.Drawing.Point(449, 16);
            this.dtpPurchaseFromDate.Name = "dtpPurchaseFromDate";
            this.dtpPurchaseFromDate.ShowCheckBox = true;
            this.dtpPurchaseFromDate.Size = new System.Drawing.Size(101, 21);
            this.dtpPurchaseFromDate.TabIndex = 5;
            this.dtpPurchaseFromDate.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dtpPurchaseFromDate_KeyDown);
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSearch.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSearch.Image = global::VATClient.Properties.Resources.search;
            this.btnSearch.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSearch.Location = new System.Drawing.Point(678, 15);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 28);
            this.btnSearch.TabIndex = 9;
            this.btnSearch.Text = "&Search";
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(370, 20);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(77, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Purchase Date";
            // 
            // txtInvoiceNo
            // 
            this.txtInvoiceNo.Location = new System.Drawing.Point(114, 16);
            this.txtInvoiceNo.Name = "txtInvoiceNo";
            this.txtInvoiceNo.Size = new System.Drawing.Size(200, 21);
            this.txtInvoiceNo.TabIndex = 0;
            this.txtInvoiceNo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtInvoiceNo_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(46, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Purchase No";
            // 
            // bgwPurchaseSearch
            // 
            this.bgwPurchaseSearch.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwPurchaseSearch_DoWork);
            this.bgwPurchaseSearch.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwPurchaseSearch_RunWorkerCompleted);
            // 
            // bgwPurchaseSearch2
            // 
            this.bgwPurchaseSearch2.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwPurchaseSearch2_DoWork);
            this.bgwPurchaseSearch2.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwPurchaseSearch2_RunWorkerCompleted);
            // 
            // bgwMultiplePost
            // 
            this.bgwMultiplePost.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwMultiplePost_DoWork);
            this.bgwMultiplePost.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwMultiplePost_RunWorkerCompleted);
            // 
            // bgwMultipleRebate
            // 
            this.bgwMultipleRebate.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwMultipleRebate_DoWork);
            this.bgwMultipleRebate.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwMultipleRebate_RunWorkerCompleted);
            // 
            // btnOk
            // 
            this.btnOk.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnOk.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnOk.Image = global::VATClient.Properties.Resources.Back;
            this.btnOk.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOk.Location = new System.Drawing.Point(699, 6);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 28);
            this.btnOk.TabIndex = 13;
            this.btnOk.Text = "&Ok";
            this.btnOk.UseVisualStyleBackColor = false;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::VATClient.Properties.Resources.Green_Footer;
            this.panel1.Controls.Add(this.btnExportTDS);
            this.panel1.Controls.Add(this.btnBankChannelMIS);
            this.panel1.Controls.Add(this.btnMultiple);
            this.panel1.Controls.Add(this.LRecordCount);
            this.panel1.Controls.Add(this.btnOk);
            this.panel1.Controls.Add(this.cmbExport);
            this.panel1.Controls.Add(this.btnExport);
            this.panel1.Location = new System.Drawing.Point(0, 422);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(785, 40);
            this.panel1.TabIndex = 11;
            this.panel1.TabStop = true;
            // 
            // btnExportTDS
            // 
            this.btnExportTDS.BackColor = System.Drawing.Color.White;
            this.btnExportTDS.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnExportTDS.Image = global::VATClient.Properties.Resources.Load;
            this.btnExportTDS.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnExportTDS.Location = new System.Drawing.Point(427, 7);
            this.btnExportTDS.Name = "btnExportTDS";
            this.btnExportTDS.Size = new System.Drawing.Size(89, 28);
            this.btnExportTDS.TabIndex = 234;
            this.btnExportTDS.Text = "Export TDS";
            this.btnExportTDS.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnExportTDS.UseVisualStyleBackColor = false;
            this.btnExportTDS.Click += new System.EventHandler(this.btnExportTDS_Click);
            // 
            // btnBankChannelMIS
            // 
            this.btnBankChannelMIS.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnBankChannelMIS.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnBankChannelMIS.Image = global::VATClient.Properties.Resources.Print;
            this.btnBankChannelMIS.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnBankChannelMIS.Location = new System.Drawing.Point(522, 6);
            this.btnBankChannelMIS.Name = "btnBankChannelMIS";
            this.btnBankChannelMIS.Size = new System.Drawing.Size(57, 28);
            this.btnBankChannelMIS.TabIndex = 233;
            this.btnBankChannelMIS.Text = "MIS";
            this.btnBankChannelMIS.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnBankChannelMIS.UseVisualStyleBackColor = false;
            this.btnBankChannelMIS.Click += new System.EventHandler(this.btnBankChannelMIS_Click);
            // 
            // btnMultiple
            // 
            this.btnMultiple.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnMultiple.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnMultiple.Image = global::VATClient.Properties.Resources.Back;
            this.btnMultiple.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnMultiple.Location = new System.Drawing.Point(641, 7);
            this.btnMultiple.Name = "btnMultiple";
            this.btnMultiple.Size = new System.Drawing.Size(126, 28);
            this.btnMultiple.TabIndex = 202;
            this.btnMultiple.Text = "&Multiple Select";
            this.btnMultiple.UseVisualStyleBackColor = false;
            this.btnMultiple.Visible = false;
            this.btnMultiple.Click += new System.EventHandler(this.btnMultiple_Click);
            // 
            // LRecordCount
            // 
            this.LRecordCount.AutoSize = true;
            this.LRecordCount.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LRecordCount.Location = new System.Drawing.Point(3, 13);
            this.LRecordCount.Name = "LRecordCount";
            this.LRecordCount.Size = new System.Drawing.Size(94, 16);
            this.LRecordCount.TabIndex = 201;
            this.LRecordCount.Text = "Record Count :";
            // 
            // cmbExport
            // 
            this.cmbExport.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbExport.FormattingEnabled = true;
            this.cmbExport.Items.AddRange(new object[] {
            "EXCEL",
            "TEXT",
            "XML"});
            this.cmbExport.Location = new System.Drawing.Point(270, 11);
            this.cmbExport.Name = "cmbExport";
            this.cmbExport.Size = new System.Drawing.Size(70, 21);
            this.cmbExport.TabIndex = 232;
            // 
            // btnExport
            // 
            this.btnExport.BackColor = System.Drawing.Color.White;
            this.btnExport.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnExport.Image = global::VATClient.Properties.Resources.Load;
            this.btnExport.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnExport.Location = new System.Drawing.Point(346, 7);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(70, 28);
            this.btnExport.TabIndex = 230;
            this.btnExport.Text = "Export";
            this.btnExport.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnExport.UseVisualStyleBackColor = false;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rbtnBankChannelPayment);
            this.groupBox2.Location = new System.Drawing.Point(72, 297);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(140, 41);
            this.groupBox2.TabIndex = 113;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "groupBox2";
            this.groupBox2.Visible = false;
            // 
            // rbtnBankChannelPayment
            // 
            this.rbtnBankChannelPayment.AutoSize = true;
            this.rbtnBankChannelPayment.Location = new System.Drawing.Point(8, 18);
            this.rbtnBankChannelPayment.Name = "rbtnBankChannelPayment";
            this.rbtnBankChannelPayment.Size = new System.Drawing.Size(129, 17);
            this.rbtnBankChannelPayment.TabIndex = 37;
            this.rbtnBankChannelPayment.TabStop = true;
            this.rbtnBankChannelPayment.Text = "BankChannelPayment";
            this.rbtnBankChannelPayment.UseVisualStyleBackColor = true;
            // 
            // backgroundWorkerMIS
            // 
            this.backgroundWorkerMIS.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerMIS_DoWork);
            this.backgroundWorkerMIS.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerMIS_RunWorkerCompleted);
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(46, 160);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(67, 13);
            this.label22.TabIndex = 249;
            this.label22.Text = "Period Name";
            this.label22.Visible = false;
            // 
            // dateTimePeriodIdPicker
            // 
            this.dateTimePeriodIdPicker.Checked = false;
            this.dateTimePeriodIdPicker.CustomFormat = "MMMM-yyyy";
            this.dateTimePeriodIdPicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePeriodIdPicker.Location = new System.Drawing.Point(114, 157);
            this.dateTimePeriodIdPicker.Name = "dateTimePeriodIdPicker";
            this.dateTimePeriodIdPicker.ShowCheckBox = true;
            this.dateTimePeriodIdPicker.Size = new System.Drawing.Size(118, 21);
            this.dateTimePeriodIdPicker.TabIndex = 250;
            // 
            // SyncHSCode
            // 
            this.SyncHSCode.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.SyncHSCode.Image = global::VATClient.Properties.Resources.Referesh;
            this.SyncHSCode.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.SyncHSCode.Location = new System.Drawing.Point(239, 155);
            this.SyncHSCode.Name = "SyncHSCode";
            this.SyncHSCode.Size = new System.Drawing.Size(75, 28);
            this.SyncHSCode.TabIndex = 251;
            this.SyncHSCode.Text = "HS Code";
            this.SyncHSCode.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.SyncHSCode.UseVisualStyleBackColor = false;
            this.SyncHSCode.Click += new System.EventHandler(this.SyncHSCode_Click);
            // 
            // bgwSyncHSCode
            // 
            this.bgwSyncHSCode.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwSyncHSCode_DoWork);
            this.bgwSyncHSCode.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwSyncHSCode_RunWorkerCompleted);
            // 
            // FormPurchaseSearch
            // 
            this.AcceptButton = this.btnSearch;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.CancelButton = this.btnOk;
            this.ClientSize = new System.Drawing.Size(784, 461);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.grbTransactionHistory);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(800, 500);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(800, 500);
            this.Name = "FormPurchaseSearch";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Purchase Search";
            this.Load += new System.EventHandler(this.FormPurchaseSearch_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPurchaseHistory)).EndInit();
            this.pnlHidden.ResumeLayout(false);
            this.pnlHidden.PerformLayout();
            this.grbTransactionHistory.ResumeLayout(false);
            this.grbTransactionHistory.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridView dgvPurchaseHistory;
        private System.Windows.Forms.GroupBox grbTransactionHistory;
        private System.Windows.Forms.TextBox txtGrandTotalTo;
        private System.Windows.Forms.TextBox txtGrandTotalFrom;
        private System.Windows.Forms.TextBox txtVatAmountTo;
        private System.Windows.Forms.TextBox txtVatAmountFrom;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.DateTimePicker dtpPurchaseToDate;
        private System.Windows.Forms.DateTimePicker dtpPurchaseFromDate;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtInvoiceNo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TextBox txtVendorGroupName;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox txtVendorName;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox txtSerialNo;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtVendorGroupID;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.TextBox txtBENumber;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cmbPost;
        private System.Windows.Forms.Label label5;
        private System.ComponentModel.BackgroundWorker bgwPurchaseSearch;
        private System.Windows.Forms.ProgressBar progressBar1;
        public System.Windows.Forms.Label LVDS;
        public System.Windows.Forms.ComboBox cmbVDS;
        private System.ComponentModel.BackgroundWorker bgwPurchaseSearch2;
        private System.Windows.Forms.Label LRecordCount;
        private System.Windows.Forms.Button btnDutyDownload;
        private System.Windows.Forms.Button btnPost;
        private System.ComponentModel.BackgroundWorker bgwMultiplePost;
        private System.Windows.Forms.CheckBox chkSelectAll;
        private System.Windows.Forms.ComboBox cmbBranch;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnMultiple;
        private System.Windows.Forms.ComboBox cmbTds;
        private System.Windows.Forms.Label lblTds;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.ComboBox cmbExport;
        public System.Windows.Forms.ComboBox cmbRecordCount;
        private System.Windows.Forms.TextBox txtVendorID;
        private System.Windows.Forms.Button btnVendorRefresh;
        private System.Windows.Forms.Button btnVendorGroupRefresh;
        private System.Windows.Forms.CheckBox chkDuty;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Select;
        private System.Windows.Forms.Button btnRebate;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.DateTimePicker dtpRebateDate;
        private System.ComponentModel.BackgroundWorker bgwMultipleRebate;
        private System.Windows.Forms.ComboBox cmbIsRebate;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Panel pnlHidden;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.ComboBox cmbIsBankingChannelPay;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.GroupBox groupBox2;
        public System.Windows.Forms.RadioButton rbtnBankChannelPayment;
        private System.Windows.Forms.Button btnBankChannel;
        private System.Windows.Forms.Button btnBankChannelMIS;
        private System.ComponentModel.BackgroundWorker backgroundWorkerMIS;
        private System.Windows.Forms.Button btnMltSave;
        private System.Windows.Forms.Button btnExportTDS;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.DateTimePicker dateTimePeriodIdPicker;
        private System.Windows.Forms.Button SyncHSCode;
        private System.ComponentModel.BackgroundWorker bgwSyncHSCode;
    }
}