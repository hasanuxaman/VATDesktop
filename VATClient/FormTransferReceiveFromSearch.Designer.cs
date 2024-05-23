namespace VATClient
{
    partial class FormTransferReceiveFromSearch
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.dgvReceiveHistory = new System.Windows.Forms.DataGridView();
            this.txtSerialNo = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.grbTransactionHistory = new System.Windows.Forms.GroupBox();
            this.txtVehicleNo = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cmbTransferForm = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.comtt = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.TxtTransferFromNo = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbPost = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.dtpReceiveToDate = new System.Windows.Forms.DateTimePicker();
            this.dtpReceiveFromDate = new System.Windows.Forms.DateTimePicker();
            this.btnSearch = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.txtTransferNo = new System.Windows.Forms.TextBox();
            this.labelReceiveNo = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.LRecordCount = new System.Windows.Forms.Label();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.backgroundWorkerSearch = new System.ComponentModel.BackgroundWorker();
            this.btnAdd = new System.Windows.Forms.Button();
            this.bgwMultipleSave = new System.ComponentModel.BackgroundWorker();
            this.dtpReceiveDate = new System.Windows.Forms.DateTimePicker();
            this.label7 = new System.Windows.Forms.Label();
            this.Select = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.TransferNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TransferFromNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Post = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SerialNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ReferenceNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TotalAmount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TransferFrom = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TransactionType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.VehicleNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.chkSelectAll = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvReceiveHistory)).BeginInit();
            this.grbTransactionHistory.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkSelectAll);
            this.groupBox1.Controls.Add(this.progressBar1);
            this.groupBox1.Controls.Add(this.dgvReceiveHistory);
            this.groupBox1.Controls.Add(this.txtSerialNo);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(12, 114);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(756, 332);
            this.groupBox1.TabIndex = 108;
            this.groupBox1.TabStop = false;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(44, 89);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(575, 25);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 109;
            this.progressBar1.Visible = false;
            // 
            // dgvReceiveHistory
            // 
            this.dgvReceiveHistory.AllowUserToAddRows = false;
            this.dgvReceiveHistory.AllowUserToDeleteRows = false;
            this.dgvReceiveHistory.AllowUserToOrderColumns = true;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            dataGridViewCellStyle5.ForeColor = System.Drawing.Color.Black;
            this.dgvReceiveHistory.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle5;
            this.dgvReceiveHistory.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvReceiveHistory.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvReceiveHistory.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Select,
            this.TransferNo,
            this.TransferFromNo,
            this.Post,
            this.SerialNo,
            this.ReferenceNo,
            this.TotalAmount,
            this.TransferFrom,
            this.TDate,
            this.TransactionType,
            this.VehicleNo});
            this.dgvReceiveHistory.Location = new System.Drawing.Point(6, 31);
            this.dgvReceiveHistory.Name = "dgvReceiveHistory";
            this.dgvReceiveHistory.RowHeadersVisible = false;
            this.dgvReceiveHistory.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvReceiveHistory.Size = new System.Drawing.Size(744, 296);
            this.dgvReceiveHistory.TabIndex = 6;
            this.dgvReceiveHistory.DoubleClick += new System.EventHandler(this.dgvReceiveHistory_DoubleClick);
            // 
            // txtSerialNo
            // 
            this.txtSerialNo.Location = new System.Drawing.Point(763, 79);
            this.txtSerialNo.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtSerialNo.MinimumSize = new System.Drawing.Size(185, 20);
            this.txtSerialNo.Name = "txtSerialNo";
            this.txtSerialNo.Size = new System.Drawing.Size(185, 20);
            this.txtSerialNo.TabIndex = 1;
            this.txtSerialNo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSerialNo_KeyDown);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(688, 83);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 17);
            this.label2.TabIndex = 2;
            this.label2.Text = "Serial No:";
            // 
            // grbTransactionHistory
            // 
            this.grbTransactionHistory.Controls.Add(this.dtpReceiveDate);
            this.grbTransactionHistory.Controls.Add(this.label7);
            this.grbTransactionHistory.Controls.Add(this.btnAdd);
            this.grbTransactionHistory.Controls.Add(this.txtVehicleNo);
            this.grbTransactionHistory.Controls.Add(this.label5);
            this.grbTransactionHistory.Controls.Add(this.cmbTransferForm);
            this.grbTransactionHistory.Controls.Add(this.label6);
            this.grbTransactionHistory.Controls.Add(this.comtt);
            this.grbTransactionHistory.Controls.Add(this.label4);
            this.grbTransactionHistory.Controls.Add(this.TxtTransferFromNo);
            this.grbTransactionHistory.Controls.Add(this.label1);
            this.grbTransactionHistory.Controls.Add(this.cmbPost);
            this.grbTransactionHistory.Controls.Add(this.label9);
            this.grbTransactionHistory.Controls.Add(this.label11);
            this.grbTransactionHistory.Controls.Add(this.dtpReceiveToDate);
            this.grbTransactionHistory.Controls.Add(this.dtpReceiveFromDate);
            this.grbTransactionHistory.Controls.Add(this.btnSearch);
            this.grbTransactionHistory.Controls.Add(this.label3);
            this.grbTransactionHistory.Controls.Add(this.txtTransferNo);
            this.grbTransactionHistory.Controls.Add(this.labelReceiveNo);
            this.grbTransactionHistory.Location = new System.Drawing.Point(12, 3);
            this.grbTransactionHistory.Name = "grbTransactionHistory";
            this.grbTransactionHistory.Size = new System.Drawing.Size(756, 112);
            this.grbTransactionHistory.TabIndex = 107;
            this.grbTransactionHistory.TabStop = false;
            this.grbTransactionHistory.Text = "Searching Criteria";
            this.grbTransactionHistory.Enter += new System.EventHandler(this.grbTransactionHistory_Enter);
            // 
            // txtVehicleNo
            // 
            this.txtVehicleNo.Location = new System.Drawing.Point(119, 67);
            this.txtVehicleNo.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtVehicleNo.MinimumSize = new System.Drawing.Size(185, 20);
            this.txtVehicleNo.Name = "txtVehicleNo";
            this.txtVehicleNo.Size = new System.Drawing.Size(185, 20);
            this.txtVehicleNo.TabIndex = 536;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(19, 69);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(75, 17);
            this.label5.TabIndex = 535;
            this.label5.Text = "Vehicle No:";
            // 
            // cmbTransferForm
            // 
            this.cmbTransferForm.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTransferForm.FormattingEnabled = true;
            this.cmbTransferForm.Location = new System.Drawing.Point(459, 47);
            this.cmbTransferForm.Name = "cmbTransferForm";
            this.cmbTransferForm.Size = new System.Drawing.Size(117, 25);
            this.cmbTransferForm.TabIndex = 533;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(378, 49);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(94, 17);
            this.label6.TabIndex = 534;
            this.label6.Text = "Transfer Form";
            // 
            // comtt
            // 
            this.comtt.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comtt.FormattingEnabled = true;
            this.comtt.Items.AddRange(new object[] {
            "6.1 Out",
            "6.2 Out"});
            this.comtt.Location = new System.Drawing.Point(459, 13);
            this.comtt.Name = "comtt";
            this.comtt.Size = new System.Drawing.Size(92, 25);
            this.comtt.TabIndex = 203;
            this.comtt.Visible = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(417, 17);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(24, 17);
            this.label4.TabIndex = 204;
            this.label4.Text = "TT";
            this.label4.Visible = false;
            // 
            // TxtTransferFromNo
            // 
            this.TxtTransferFromNo.Location = new System.Drawing.Point(120, 17);
            this.TxtTransferFromNo.MaximumSize = new System.Drawing.Size(200, 20);
            this.TxtTransferFromNo.MinimumSize = new System.Drawing.Size(185, 20);
            this.TxtTransferFromNo.Name = "TxtTransferFromNo";
            this.TxtTransferFromNo.Size = new System.Drawing.Size(185, 20);
            this.TxtTransferFromNo.TabIndex = 201;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(19, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(120, 17);
            this.label1.TabIndex = 202;
            this.label1.Text = "Transfer From No:";
            // 
            // cmbPost
            // 
            this.cmbPost.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPost.FormattingEnabled = true;
            this.cmbPost.Items.AddRange(new object[] {
            "Y",
            "N"});
            this.cmbPost.Location = new System.Drawing.Point(658, 45);
            this.cmbPost.Name = "cmbPost";
            this.cmbPost.Size = new System.Drawing.Size(92, 25);
            this.cmbPost.TabIndex = 4;
            this.cmbPost.Visible = false;
            this.cmbPost.SelectedIndexChanged += new System.EventHandler(this.cmbPost_SelectedIndexChanged);
            this.cmbPost.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmbPost_KeyDown);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(604, 49);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(58, 17);
            this.label9.TabIndex = 200;
            this.label9.Text = "Transfer";
            this.label9.Visible = false;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(225, 46);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(21, 17);
            this.label11.TabIndex = 112;
            this.label11.Text = "to";
            // 
            // dtpReceiveToDate
            // 
            this.dtpReceiveToDate.Checked = false;
            this.dtpReceiveToDate.CustomFormat = "d/MMM/yyyy";
            this.dtpReceiveToDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpReceiveToDate.Location = new System.Drawing.Point(245, 42);
            this.dtpReceiveToDate.Name = "dtpReceiveToDate";
            this.dtpReceiveToDate.ShowCheckBox = true;
            this.dtpReceiveToDate.Size = new System.Drawing.Size(103, 24);
            this.dtpReceiveToDate.TabIndex = 2;
            this.dtpReceiveToDate.ValueChanged += new System.EventHandler(this.dtpReceiveToDate_ValueChanged);
            this.dtpReceiveToDate.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dtpReceiveToDate_KeyDown);
            // 
            // dtpReceiveFromDate
            // 
            this.dtpReceiveFromDate.Checked = false;
            this.dtpReceiveFromDate.CustomFormat = "d/MMM/yyyy";
            this.dtpReceiveFromDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpReceiveFromDate.Location = new System.Drawing.Point(120, 42);
            this.dtpReceiveFromDate.Name = "dtpReceiveFromDate";
            this.dtpReceiveFromDate.ShowCheckBox = true;
            this.dtpReceiveFromDate.Size = new System.Drawing.Size(102, 24);
            this.dtpReceiveFromDate.TabIndex = 1;
            this.dtpReceiveFromDate.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dtpReceiveFromDate_KeyDown);
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSearch.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSearch.Image = global::VATClient.Properties.Resources.search;
            this.btnSearch.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSearch.Location = new System.Drawing.Point(674, 10);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 28);
            this.btnSearch.TabIndex = 5;
            this.btnSearch.Text = "&Search";
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(20, 45);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(96, 17);
            this.label3.TabIndex = 3;
            this.label3.Text = "Transfer Date:";
            // 
            // txtTransferNo
            // 
            this.txtTransferNo.Location = new System.Drawing.Point(256, 83);
            this.txtTransferNo.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtTransferNo.MinimumSize = new System.Drawing.Size(185, 20);
            this.txtTransferNo.Name = "txtTransferNo";
            this.txtTransferNo.Size = new System.Drawing.Size(185, 20);
            this.txtTransferNo.TabIndex = 0;
            this.txtTransferNo.Visible = false;
            this.txtTransferNo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtReceiveNo_KeyDown);
            // 
            // labelReceiveNo
            // 
            this.labelReceiveNo.AutoSize = true;
            this.labelReceiveNo.Location = new System.Drawing.Point(182, 88);
            this.labelReceiveNo.Name = "labelReceiveNo";
            this.labelReceiveNo.Size = new System.Drawing.Size(84, 17);
            this.labelReceiveNo.TabIndex = 0;
            this.labelReceiveNo.Text = "Transfer No:";
            this.labelReceiveNo.Visible = false;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.panel1.Controls.Add(this.LRecordCount);
            this.panel1.Controls.Add(this.btnOk);
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Location = new System.Drawing.Point(-1, 448);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(793, 46);
            this.panel1.TabIndex = 6;
            // 
            // LRecordCount
            // 
            this.LRecordCount.AutoSize = true;
            this.LRecordCount.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LRecordCount.Location = new System.Drawing.Point(102, 13);
            this.LRecordCount.Name = "LRecordCount";
            this.LRecordCount.Size = new System.Drawing.Size(121, 21);
            this.LRecordCount.TabIndex = 223;
            this.LRecordCount.Text = "Record Count :";
            // 
            // btnOk
            // 
            this.btnOk.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnOk.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnOk.Image = global::VATClient.Properties.Resources.Back;
            this.btnOk.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOk.Location = new System.Drawing.Point(698, 7);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 28);
            this.btnOk.TabIndex = 8;
            this.btnOk.Text = "&Ok";
            this.btnOk.UseVisualStyleBackColor = false;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnCancel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancel.Image = global::VATClient.Properties.Resources.Referesh;
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.Location = new System.Drawing.Point(17, 7);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 28);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "&Refresh";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnAdd.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAdd.Image = global::VATClient.Properties.Resources.Save;
            this.btnAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAdd.Location = new System.Drawing.Point(672, 70);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 28);
            this.btnAdd.TabIndex = 537;
            this.btnAdd.Text = "&Add";
            this.btnAdd.UseVisualStyleBackColor = false;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // bgwMultipleSave
            // 
            this.bgwMultipleSave.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwMultipleSave_DoWork);
            this.bgwMultipleSave.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwMultipleSave_RunWorkerCompleted);
            // 
            // dtpReceiveDate
            // 
            this.dtpReceiveDate.CustomFormat = "dd/MMM/yyyy";
            this.dtpReceiveDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpReceiveDate.Location = new System.Drawing.Point(458, 77);
            this.dtpReceiveDate.MaximumSize = new System.Drawing.Size(120, 20);
            this.dtpReceiveDate.MinimumSize = new System.Drawing.Size(90, 20);
            this.dtpReceiveDate.Name = "dtpReceiveDate";
            this.dtpReceiveDate.Size = new System.Drawing.Size(118, 20);
            this.dtpReceiveDate.TabIndex = 538;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(353, 81);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(88, 17);
            this.label7.TabIndex = 539;
            this.label7.Text = "Receive Date";
            // 
            // Select
            // 
            this.Select.HeaderText = "select";
            this.Select.Name = "Select";
            this.Select.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Select.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // TransferNo
            // 
            this.TransferNo.HeaderText = "TransferNo";
            this.TransferNo.Name = "TransferNo";
            this.TransferNo.ReadOnly = true;
            this.TransferNo.Visible = false;
            // 
            // TransferFromNo
            // 
            this.TransferFromNo.HeaderText = "Transfer From No";
            this.TransferFromNo.Name = "TransferFromNo";
            this.TransferFromNo.Width = 120;
            // 
            // Post
            // 
            this.Post.HeaderText = "Received";
            this.Post.Name = "Post";
            this.Post.ReadOnly = true;
            // 
            // SerialNo
            // 
            this.SerialNo.HeaderText = "Serial No";
            this.SerialNo.Name = "SerialNo";
            this.SerialNo.ReadOnly = true;
            // 
            // ReferenceNo
            // 
            this.ReferenceNo.HeaderText = "Reference No";
            this.ReferenceNo.Name = "ReferenceNo";
            // 
            // TotalAmount
            // 
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.TotalAmount.DefaultCellStyle = dataGridViewCellStyle6;
            this.TotalAmount.HeaderText = "Grand Total";
            this.TotalAmount.Name = "TotalAmount";
            this.TotalAmount.ReadOnly = true;
            this.TotalAmount.Visible = false;
            this.TotalAmount.Width = 120;
            // 
            // TransferFrom
            // 
            this.TransferFrom.HeaderText = "Receive From";
            this.TransferFrom.Name = "TransferFrom";
            // 
            // TDate
            // 
            this.TDate.HeaderText = "Transfer Date";
            this.TDate.Name = "TDate";
            // 
            // TransactionType
            // 
            this.TransactionType.HeaderText = "TransactionType";
            this.TransactionType.Name = "TransactionType";
            this.TransactionType.ReadOnly = true;
            // 
            // VehicleNo
            // 
            this.VehicleNo.HeaderText = "Vehicle No";
            this.VehicleNo.Name = "VehicleNo";
            this.VehicleNo.ReadOnly = true;
            // 
            // chkSelectAll
            // 
            this.chkSelectAll.AutoSize = true;
            this.chkSelectAll.Location = new System.Drawing.Point(11, 7);
            this.chkSelectAll.Name = "chkSelectAll";
            this.chkSelectAll.Size = new System.Drawing.Size(78, 21);
            this.chkSelectAll.TabIndex = 540;
            this.chkSelectAll.Text = "SelectAll";
            this.chkSelectAll.UseVisualStyleBackColor = true;
            this.chkSelectAll.CheckedChanged += new System.EventHandler(this.chkSelectAll_CheckedChanged);
            // 
            // FormTransferReceiveFromSearch
            // 
            this.AcceptButton = this.btnSearch;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(206)))), ((int)(((byte)(206)))), ((int)(((byte)(246)))));
            this.CancelButton = this.btnOk;
            this.ClientSize = new System.Drawing.Size(782, 493);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.grbTransactionHistory);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(800, 540);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(800, 500);
            this.Name = "FormTransferReceiveFromSearch";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Transfer Receive From Search";
            this.Load += new System.EventHandler(this.FormTransferReceiveFromSearch_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvReceiveHistory)).EndInit();
            this.grbTransactionHistory.ResumeLayout(false);
            this.grbTransactionHistory.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridView dgvReceiveHistory;
        private System.Windows.Forms.GroupBox grbTransactionHistory;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.DateTimePicker dtpReceiveToDate;
        private System.Windows.Forms.DateTimePicker dtpReceiveFromDate;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.TextBox txtSerialNo;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtTransferNo;
        private System.Windows.Forms.Label labelReceiveNo;
        private System.Windows.Forms.ComboBox cmbPost;
        private System.Windows.Forms.Label label9;
        private System.ComponentModel.BackgroundWorker backgroundWorkerSearch;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label LRecordCount;
        private System.Windows.Forms.TextBox TxtTransferFromNo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comtt;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cmbTransferForm;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtVehicleNo;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnAdd;
        private System.ComponentModel.BackgroundWorker bgwMultipleSave;
        private System.Windows.Forms.DateTimePicker dtpReceiveDate;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Select;
        private System.Windows.Forms.DataGridViewTextBoxColumn TransferNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn TransferFromNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn Post;
        private System.Windows.Forms.DataGridViewTextBoxColumn SerialNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn ReferenceNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn TotalAmount;
        private System.Windows.Forms.DataGridViewTextBoxColumn TransferFrom;
        private System.Windows.Forms.DataGridViewTextBoxColumn TDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn TransactionType;
        private System.Windows.Forms.DataGridViewTextBoxColumn VehicleNo;
        private System.Windows.Forms.CheckBox chkSelectAll;
    }
}