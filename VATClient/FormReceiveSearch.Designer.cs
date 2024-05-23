namespace VATClient
{
    partial class FormReceiveSearch
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
            this.txtGrandTotalTo = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtRefNo = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtGrandTotalFrom = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkSelectAll = new System.Windows.Forms.CheckBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.dgvReceiveHistory = new System.Windows.Forms.DataGridView();
            this.Select = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.pnlHidden = new System.Windows.Forms.Panel();
            this.txtVatAmountFrom = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.txtVatAmountTo = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.dtpReceiveToDate = new System.Windows.Forms.DateTimePicker();
            this.dtpReceiveFromDate = new System.Windows.Forms.DateTimePicker();
            this.grbTransactionHistory = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.btnMultiple = new System.Windows.Forms.Button();
            this.txtImportID = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.cmbRecordCount = new System.Windows.Forms.ComboBox();
            this.cmbBranch = new System.Windows.Forms.ComboBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label15 = new System.Windows.Forms.Label();
            this.txtSerialNo = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btnPost = new System.Windows.Forms.Button();
            this.btnCustomerRefresh = new System.Windows.Forms.Button();
            this.txtCustomer = new System.Windows.Forms.TextBox();
            this.label37 = new System.Windows.Forms.Label();
            this.cmbPost = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnSearch = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.txtReceiveNo = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbExport = new System.Windows.Forms.ComboBox();
            this.btnImport = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.LRecordCount = new System.Windows.Forms.Label();
            this.btnOk = new System.Windows.Forms.Button();
            this.bgwSearch = new System.ComponentModel.BackgroundWorker();
            this.bgwMultiplePost = new System.ComponentModel.BackgroundWorker();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvReceiveHistory)).BeginInit();
            this.pnlHidden.SuspendLayout();
            this.grbTransactionHistory.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtGrandTotalTo
            // 
            this.txtGrandTotalTo.Location = new System.Drawing.Point(111, 26);
            this.txtGrandTotalTo.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtGrandTotalTo.MinimumSize = new System.Drawing.Size(75, 20);
            this.txtGrandTotalTo.Name = "txtGrandTotalTo";
            this.txtGrandTotalTo.Size = new System.Drawing.Size(80, 21);
            this.txtGrandTotalTo.TabIndex = 7;
            this.txtGrandTotalTo.Text = "0.00";
            this.txtGrandTotalTo.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtGrandTotalTo.Visible = false;
            this.txtGrandTotalTo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtGrandTotalTo_KeyDown);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(90, 29);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(17, 13);
            this.label4.TabIndex = 117;
            this.label4.Text = "to";
            this.label4.Visible = false;
            // 
            // txtRefNo
            // 
            this.txtRefNo.BackColor = System.Drawing.SystemColors.Window;
            this.txtRefNo.Location = new System.Drawing.Point(102, 41);
            this.txtRefNo.Name = "txtRefNo";
            this.txtRefNo.Size = new System.Drawing.Size(200, 21);
            this.txtRefNo.TabIndex = 1;
            this.txtRefNo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSerialNo_KeyDown);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(23, 45);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(64, 13);
            this.label6.TabIndex = 126;
            this.label6.Text = "Ref Number";
            // 
            // txtGrandTotalFrom
            // 
            this.txtGrandTotalFrom.Location = new System.Drawing.Point(6, 26);
            this.txtGrandTotalFrom.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtGrandTotalFrom.MinimumSize = new System.Drawing.Size(75, 20);
            this.txtGrandTotalFrom.Name = "txtGrandTotalFrom";
            this.txtGrandTotalFrom.Size = new System.Drawing.Size(80, 21);
            this.txtGrandTotalFrom.TabIndex = 6;
            this.txtGrandTotalFrom.Text = "0.00";
            this.txtGrandTotalFrom.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtGrandTotalFrom.Visible = false;
            this.txtGrandTotalFrom.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtGrandTotalFrom_KeyDown);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkSelectAll);
            this.groupBox1.Controls.Add(this.progressBar1);
            this.groupBox1.Controls.Add(this.dgvReceiveHistory);
            this.groupBox1.Controls.Add(this.pnlHidden);
            this.groupBox1.Location = new System.Drawing.Point(16, 147);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(756, 267);
            this.groupBox1.TabIndex = 116;
            this.groupBox1.TabStop = false;
            // 
            // chkSelectAll
            // 
            this.chkSelectAll.AutoSize = true;
            this.chkSelectAll.Location = new System.Drawing.Point(6, -1);
            this.chkSelectAll.Name = "chkSelectAll";
            this.chkSelectAll.Size = new System.Drawing.Size(66, 17);
            this.chkSelectAll.TabIndex = 117;
            this.chkSelectAll.Text = "SelectAll";
            this.chkSelectAll.UseVisualStyleBackColor = true;
            this.chkSelectAll.Click += new System.EventHandler(this.chkSelectAll_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(225, 111);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(300, 30);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 211;
            this.progressBar1.Visible = false;
            // 
            // dgvReceiveHistory
            // 
            this.dgvReceiveHistory.AllowUserToAddRows = false;
            this.dgvReceiveHistory.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.dgvReceiveHistory.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvReceiveHistory.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvReceiveHistory.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Select});
            this.dgvReceiveHistory.Location = new System.Drawing.Point(0, 17);
            this.dgvReceiveHistory.Name = "dgvReceiveHistory";
            this.dgvReceiveHistory.RowHeadersVisible = false;
            this.dgvReceiveHistory.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvReceiveHistory.Size = new System.Drawing.Size(751, 245);
            this.dgvReceiveHistory.TabIndex = 6;
            this.dgvReceiveHistory.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvReceiveHistory_CellContentClick);
            this.dgvReceiveHistory.DoubleClick += new System.EventHandler(this.dgvReceiveHistory_DoubleClick);
            // 
            // Select
            // 
            this.Select.HeaderText = "Select";
            this.Select.Name = "Select";
            // 
            // pnlHidden
            // 
            this.pnlHidden.Controls.Add(this.txtGrandTotalFrom);
            this.pnlHidden.Controls.Add(this.txtVatAmountFrom);
            this.pnlHidden.Controls.Add(this.label13);
            this.pnlHidden.Controls.Add(this.txtVatAmountTo);
            this.pnlHidden.Controls.Add(this.label4);
            this.pnlHidden.Controls.Add(this.txtGrandTotalTo);
            this.pnlHidden.Controls.Add(this.label10);
            this.pnlHidden.Controls.Add(this.label9);
            this.pnlHidden.Location = new System.Drawing.Point(6, 152);
            this.pnlHidden.Name = "pnlHidden";
            this.pnlHidden.Size = new System.Drawing.Size(344, 100);
            this.pnlHidden.TabIndex = 212;
            this.pnlHidden.Visible = false;
            // 
            // txtVatAmountFrom
            // 
            this.txtVatAmountFrom.Location = new System.Drawing.Point(6, 2);
            this.txtVatAmountFrom.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtVatAmountFrom.MinimumSize = new System.Drawing.Size(75, 20);
            this.txtVatAmountFrom.Name = "txtVatAmountFrom";
            this.txtVatAmountFrom.Size = new System.Drawing.Size(80, 21);
            this.txtVatAmountFrom.TabIndex = 4;
            this.txtVatAmountFrom.Text = "0.00";
            this.txtVatAmountFrom.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtVatAmountFrom.Visible = false;
            this.txtVatAmountFrom.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtVatAmountFrom_KeyDown);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(90, 5);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(17, 13);
            this.label13.TabIndex = 114;
            this.label13.Text = "to";
            this.label13.Visible = false;
            // 
            // txtVatAmountTo
            // 
            this.txtVatAmountTo.Location = new System.Drawing.Point(111, 2);
            this.txtVatAmountTo.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtVatAmountTo.MinimumSize = new System.Drawing.Size(75, 20);
            this.txtVatAmountTo.Name = "txtVatAmountTo";
            this.txtVatAmountTo.Size = new System.Drawing.Size(80, 21);
            this.txtVatAmountTo.TabIndex = 5;
            this.txtVatAmountTo.Text = "0.00";
            this.txtVatAmountTo.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtVatAmountTo.Visible = false;
            this.txtVatAmountTo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtVatAmountTo_KeyDown);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(197, 5);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(97, 13);
            this.label10.TabIndex = 6;
            this.label10.Text = "Total VAT Amount:";
            this.label10.Visible = false;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(197, 27);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(67, 13);
            this.label9.TabIndex = 7;
            this.label9.Text = "Grand Total:";
            this.label9.Visible = false;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(530, 23);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(17, 13);
            this.label11.TabIndex = 112;
            this.label11.Text = "to";
            // 
            // dtpReceiveToDate
            // 
            this.dtpReceiveToDate.Checked = false;
            this.dtpReceiveToDate.CustomFormat = "dd/MMM/yyyy";
            this.dtpReceiveToDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpReceiveToDate.Location = new System.Drawing.Point(547, 19);
            this.dtpReceiveToDate.Name = "dtpReceiveToDate";
            this.dtpReceiveToDate.ShowCheckBox = true;
            this.dtpReceiveToDate.Size = new System.Drawing.Size(103, 21);
            this.dtpReceiveToDate.TabIndex = 3;
            this.dtpReceiveToDate.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dtpReceiveToDate_KeyDown);
            // 
            // dtpReceiveFromDate
            // 
            this.dtpReceiveFromDate.Checked = false;
            this.dtpReceiveFromDate.CustomFormat = "dd/MMM/yyyy";
            this.dtpReceiveFromDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpReceiveFromDate.Location = new System.Drawing.Point(428, 19);
            this.dtpReceiveFromDate.Name = "dtpReceiveFromDate";
            this.dtpReceiveFromDate.ShowCheckBox = true;
            this.dtpReceiveFromDate.Size = new System.Drawing.Size(102, 21);
            this.dtpReceiveFromDate.TabIndex = 2;
            this.dtpReceiveFromDate.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dtpReceiveFromDate_KeyDown);
            // 
            // grbTransactionHistory
            // 
            this.grbTransactionHistory.Controls.Add(this.label7);
            this.grbTransactionHistory.Controls.Add(this.btnMultiple);
            this.grbTransactionHistory.Controls.Add(this.txtImportID);
            this.grbTransactionHistory.Controls.Add(this.label18);
            this.grbTransactionHistory.Controls.Add(this.cmbRecordCount);
            this.grbTransactionHistory.Controls.Add(this.cmbBranch);
            this.grbTransactionHistory.Controls.Add(this.btnCancel);
            this.grbTransactionHistory.Controls.Add(this.label15);
            this.grbTransactionHistory.Controls.Add(this.txtSerialNo);
            this.grbTransactionHistory.Controls.Add(this.label5);
            this.grbTransactionHistory.Controls.Add(this.btnPost);
            this.grbTransactionHistory.Controls.Add(this.btnCustomerRefresh);
            this.grbTransactionHistory.Controls.Add(this.txtCustomer);
            this.grbTransactionHistory.Controls.Add(this.label37);
            this.grbTransactionHistory.Controls.Add(this.cmbPost);
            this.grbTransactionHistory.Controls.Add(this.label2);
            this.grbTransactionHistory.Controls.Add(this.btnAdd);
            this.grbTransactionHistory.Controls.Add(this.txtRefNo);
            this.grbTransactionHistory.Controls.Add(this.label6);
            this.grbTransactionHistory.Controls.Add(this.label11);
            this.grbTransactionHistory.Controls.Add(this.dtpReceiveToDate);
            this.grbTransactionHistory.Controls.Add(this.dtpReceiveFromDate);
            this.grbTransactionHistory.Controls.Add(this.btnSearch);
            this.grbTransactionHistory.Controls.Add(this.label3);
            this.grbTransactionHistory.Controls.Add(this.txtReceiveNo);
            this.grbTransactionHistory.Controls.Add(this.label1);
            this.grbTransactionHistory.Location = new System.Drawing.Point(14, 7);
            this.grbTransactionHistory.Name = "grbTransactionHistory";
            this.grbTransactionHistory.Size = new System.Drawing.Size(756, 136);
            this.grbTransactionHistory.TabIndex = 115;
            this.grbTransactionHistory.TabStop = false;
            this.grbTransactionHistory.Text = "Searching Criteria";
            this.grbTransactionHistory.Enter += new System.EventHandler(this.grbTransactionHistory_Enter);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(351, 69);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(53, 13);
            this.label7.TabIndex = 242;
            this.label7.Text = "Import ID";
            // 
            // btnMultiple
            // 
            this.btnMultiple.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnMultiple.Image = global::VATClient.Properties.Resources.Update;
            this.btnMultiple.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnMultiple.Location = new System.Drawing.Point(597, 103);
            this.btnMultiple.Name = "btnMultiple";
            this.btnMultiple.Size = new System.Drawing.Size(75, 28);
            this.btnMultiple.TabIndex = 240;
            this.btnMultiple.Text = "&Multiple";
            this.btnMultiple.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnMultiple.UseVisualStyleBackColor = false;
            this.btnMultiple.Click += new System.EventHandler(this.btnMultiple_Click);
            // 
            // txtImportID
            // 
            this.txtImportID.Location = new System.Drawing.Point(428, 65);
            this.txtImportID.Name = "txtImportID";
            this.txtImportID.Size = new System.Drawing.Size(200, 21);
            this.txtImportID.TabIndex = 241;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(23, 113);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(25, 13);
            this.label18.TabIndex = 239;
            this.label18.Text = "Top";
            // 
            // cmbRecordCount
            // 
            this.cmbRecordCount.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRecordCount.FormattingEnabled = true;
            this.cmbRecordCount.Location = new System.Drawing.Point(102, 110);
            this.cmbRecordCount.Name = "cmbRecordCount";
            this.cmbRecordCount.Size = new System.Drawing.Size(80, 21);
            this.cmbRecordCount.TabIndex = 235;
            // 
            // cmbBranch
            // 
            this.cmbBranch.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBranch.FormattingEnabled = true;
            this.cmbBranch.Items.AddRange(new object[] {
            "Y",
            "N"});
            this.cmbBranch.Location = new System.Drawing.Point(102, 87);
            this.cmbBranch.Name = "cmbBranch";
            this.cmbBranch.Size = new System.Drawing.Size(200, 21);
            this.cmbBranch.TabIndex = 226;
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnCancel.Image = global::VATClient.Properties.Resources.Referesh;
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.Location = new System.Drawing.Point(675, 40);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 28);
            this.btnCancel.TabIndex = 12;
            this.btnCancel.Text = "&Refresh";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(23, 91);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(40, 13);
            this.label15.TabIndex = 225;
            this.label15.Text = "Branch";
            // 
            // txtSerialNo
            // 
            this.txtSerialNo.Location = new System.Drawing.Point(428, 42);
            this.txtSerialNo.Name = "txtSerialNo";
            this.txtSerialNo.Size = new System.Drawing.Size(200, 21);
            this.txtSerialNo.TabIndex = 223;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(351, 45);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(45, 13);
            this.label5.TabIndex = 222;
            this.label5.Text = "Batch #";
            // 
            // btnPost
            // 
            this.btnPost.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnPost.Image = global::VATClient.Properties.Resources.Post;
            this.btnPost.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPost.Location = new System.Drawing.Point(675, 103);
            this.btnPost.Name = "btnPost";
            this.btnPost.Size = new System.Drawing.Size(75, 28);
            this.btnPost.TabIndex = 221;
            this.btnPost.Text = "Post";
            this.btnPost.UseVisualStyleBackColor = false;
            this.btnPost.Click += new System.EventHandler(this.btnPost_Click);
            // 
            // btnCustomerRefresh
            // 
            this.btnCustomerRefresh.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnCustomerRefresh.Image = global::VATClient.Properties.Resources.Referesh;
            this.btnCustomerRefresh.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCustomerRefresh.Location = new System.Drawing.Point(306, 62);
            this.btnCustomerRefresh.Name = "btnCustomerRefresh";
            this.btnCustomerRefresh.Size = new System.Drawing.Size(26, 24);
            this.btnCustomerRefresh.TabIndex = 220;
            this.btnCustomerRefresh.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCustomerRefresh.UseVisualStyleBackColor = false;
            this.btnCustomerRefresh.Click += new System.EventHandler(this.btnCustomerRefresh_Click);
            // 
            // txtCustomer
            // 
            this.txtCustomer.Location = new System.Drawing.Point(102, 64);
            this.txtCustomer.Name = "txtCustomer";
            this.txtCustomer.ReadOnly = true;
            this.txtCustomer.Size = new System.Drawing.Size(200, 21);
            this.txtCustomer.TabIndex = 219;
            this.txtCustomer.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtCustomer_KeyDown);
            // 
            // label37
            // 
            this.label37.AutoSize = true;
            this.label37.Location = new System.Drawing.Point(23, 68);
            this.label37.Name = "label37";
            this.label37.Size = new System.Drawing.Size(76, 13);
            this.label37.TabIndex = 218;
            this.label37.Text = "Customer (F9)";
            // 
            // cmbPost
            // 
            this.cmbPost.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPost.FormattingEnabled = true;
            this.cmbPost.Items.AddRange(new object[] {
            "Y",
            "N"});
            this.cmbPost.Location = new System.Drawing.Point(428, 87);
            this.cmbPost.Name = "cmbPost";
            this.cmbPost.Size = new System.Drawing.Size(80, 21);
            this.cmbPost.TabIndex = 8;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(351, 91);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(28, 13);
            this.label2.TabIndex = 200;
            this.label2.Text = "Post";
            // 
            // btnAdd
            // 
            this.btnAdd.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnAdd.Image = global::VATClient.Properties.Resources.add_over;
            this.btnAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAdd.Location = new System.Drawing.Point(520, 103);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 28);
            this.btnAdd.TabIndex = 9;
            this.btnAdd.Text = "&New";
            this.btnAdd.UseVisualStyleBackColor = false;
            this.btnAdd.Visible = false;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSearch.Image = global::VATClient.Properties.Resources.search;
            this.btnSearch.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSearch.Location = new System.Drawing.Point(675, 12);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 28);
            this.btnSearch.TabIndex = 10;
            this.btnSearch.Text = "&Search";
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(351, 23);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Receive Date";
            // 
            // txtReceiveNo
            // 
            this.txtReceiveNo.Location = new System.Drawing.Point(102, 19);
            this.txtReceiveNo.Name = "txtReceiveNo";
            this.txtReceiveNo.Size = new System.Drawing.Size(200, 21);
            this.txtReceiveNo.TabIndex = 0;
            this.txtReceiveNo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtReceiveNo_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(23, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Receive No";
            // 
            // cmbExport
            // 
            this.cmbExport.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbExport.FormattingEnabled = true;
            this.cmbExport.Items.AddRange(new object[] {
            "EXCEL",
            "TEXT"});
            this.cmbExport.Location = new System.Drawing.Point(313, 10);
            this.cmbExport.Name = "cmbExport";
            this.cmbExport.Size = new System.Drawing.Size(80, 21);
            this.cmbExport.TabIndex = 234;
            // 
            // btnImport
            // 
            this.btnImport.BackColor = System.Drawing.Color.White;
            this.btnImport.Image = global::VATClient.Properties.Resources.Load;
            this.btnImport.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnImport.Location = new System.Drawing.Point(395, 6);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(75, 28);
            this.btnImport.TabIndex = 117;
            this.btnImport.Text = "Export";
            this.btnImport.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnImport.UseVisualStyleBackColor = false;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::VATClient.Properties.Resources.Green_Footer;
            this.panel1.Controls.Add(this.LRecordCount);
            this.panel1.Controls.Add(this.cmbExport);
            this.panel1.Controls.Add(this.btnOk);
            this.panel1.Controls.Add(this.btnImport);
            this.panel1.Location = new System.Drawing.Point(1, 422);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(784, 40);
            this.panel1.TabIndex = 11;
            this.panel1.TabStop = true;
            // 
            // LRecordCount
            // 
            this.LRecordCount.AutoSize = true;
            this.LRecordCount.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LRecordCount.Location = new System.Drawing.Point(4, 12);
            this.LRecordCount.Name = "LRecordCount";
            this.LRecordCount.Size = new System.Drawing.Size(94, 16);
            this.LRecordCount.TabIndex = 221;
            this.LRecordCount.Text = "Record Count :";
            // 
            // btnOk
            // 
            this.btnOk.BackColor = System.Drawing.SystemColors.ButtonHighlight;
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
            // FormReceiveSearch
            // 
            this.AcceptButton = this.btnSearch;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.CancelButton = this.btnOk;
            this.ClientSize = new System.Drawing.Size(784, 461);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.grbTransactionHistory);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(800, 500);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(800, 500);
            this.Name = "FormReceiveSearch";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Receive Search";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormReceiveSearch_FormClosing);
            this.Load += new System.EventHandler(this.FormReceiveSearch_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvReceiveHistory)).EndInit();
            this.pnlHidden.ResumeLayout(false);
            this.pnlHidden.PerformLayout();
            this.grbTransactionHistory.ResumeLayout(false);
            this.grbTransactionHistory.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TextBox txtGrandTotalTo;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtRefNo;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtGrandTotalFrom;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridView dgvReceiveHistory;
        private System.Windows.Forms.TextBox txtVatAmountTo;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox txtVatAmountFrom;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.DateTimePicker dtpReceiveToDate;
        private System.Windows.Forms.DateTimePicker dtpReceiveFromDate;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.GroupBox grbTransactionHistory;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtReceiveNo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.ComboBox cmbPost;
        private System.Windows.Forms.Label label2;
        private System.ComponentModel.BackgroundWorker bgwSearch;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label LRecordCount;
        private System.Windows.Forms.TextBox txtCustomer;
        private System.Windows.Forms.Label label37;
        private System.Windows.Forms.Button btnCustomerRefresh;
        private System.Windows.Forms.Button btnPost;
        private System.ComponentModel.BackgroundWorker bgwMultiplePost;
        private System.Windows.Forms.CheckBox chkSelectAll;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtSerialNo;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.ComboBox cmbBranch;
        private System.Windows.Forms.Button btnImport;
        private System.Windows.Forms.ComboBox cmbExport;
        public System.Windows.Forms.ComboBox cmbRecordCount;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Select;
        private System.Windows.Forms.Panel pnlHidden;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Button btnMultiple;
        private System.Windows.Forms.TextBox txtImportID;
        private System.Windows.Forms.Label label7;
    }
}