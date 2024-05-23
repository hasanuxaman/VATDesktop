namespace VATClient
{
    partial class FormDutyDrawBackSearch
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
            this.grbTransactionHistory = new System.Windows.Forms.GroupBox();
            this.cmbBranch = new System.Windows.Forms.ComboBox();
            this.label15 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.ddbackSalesToDate = new System.Windows.Forms.DateTimePicker();
            this.ddbackSalesFormDate = new System.Windows.Forms.DateTimePicker();
            this.label5 = new System.Windows.Forms.Label();
            this.cmbPost = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtFinishGood = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtCustomerName = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtSalesInvoicNo = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.ddbackToDate = new System.Windows.Forms.DateTimePicker();
            this.ddbackFromDate = new System.Windows.Forms.DateTimePicker();
            this.btnSearch = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.txtDDBackNo = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.LRecordCount = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.dgvDDBackHistory = new System.Windows.Forms.DataGridView();
            this.backgroundWorkerSearch = new System.ComponentModel.BackgroundWorker();
            this.DDBackNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DDBackDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SalesInvoiceNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SalesDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CustormerID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CustormerName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CurrencyId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CurrencyCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ExpCurrency = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BDTCurrency = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FgItemNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FgItemName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TotalClaimCD = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TotalClaimRD = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TotalClaimSD = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TotalDDBack = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TotalClaimVAT = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TotalClaimCnFAmount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TotalClaimInsuranceAmount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TotalClaimTVBAmount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TotalClaimTVAAmount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TotalClaimATVAmount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TotalClaimOthersAmount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Comments = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CreatedBy = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CreatedOn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LastModifiedBy = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LastModifiedOn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Post = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BranchId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ApprovedSD = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TotalSDAmount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.grbTransactionHistory.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDDBackHistory)).BeginInit();
            this.SuspendLayout();
            // 
            // grbTransactionHistory
            // 
            this.grbTransactionHistory.Controls.Add(this.cmbBranch);
            this.grbTransactionHistory.Controls.Add(this.label15);
            this.grbTransactionHistory.Controls.Add(this.label2);
            this.grbTransactionHistory.Controls.Add(this.ddbackSalesToDate);
            this.grbTransactionHistory.Controls.Add(this.ddbackSalesFormDate);
            this.grbTransactionHistory.Controls.Add(this.label5);
            this.grbTransactionHistory.Controls.Add(this.cmbPost);
            this.grbTransactionHistory.Controls.Add(this.label9);
            this.grbTransactionHistory.Controls.Add(this.txtFinishGood);
            this.grbTransactionHistory.Controls.Add(this.label7);
            this.grbTransactionHistory.Controls.Add(this.txtCustomerName);
            this.grbTransactionHistory.Controls.Add(this.label6);
            this.grbTransactionHistory.Controls.Add(this.txtSalesInvoicNo);
            this.grbTransactionHistory.Controls.Add(this.label4);
            this.grbTransactionHistory.Controls.Add(this.label11);
            this.grbTransactionHistory.Controls.Add(this.ddbackToDate);
            this.grbTransactionHistory.Controls.Add(this.ddbackFromDate);
            this.grbTransactionHistory.Controls.Add(this.btnSearch);
            this.grbTransactionHistory.Controls.Add(this.label3);
            this.grbTransactionHistory.Controls.Add(this.txtDDBackNo);
            this.grbTransactionHistory.Controls.Add(this.label1);
            this.grbTransactionHistory.Location = new System.Drawing.Point(7, 11);
            this.grbTransactionHistory.Name = "grbTransactionHistory";
            this.grbTransactionHistory.Size = new System.Drawing.Size(756, 135);
            this.grbTransactionHistory.TabIndex = 108;
            this.grbTransactionHistory.TabStop = false;
            this.grbTransactionHistory.Text = "Searching Criteria";
            // 
            // cmbBranch
            // 
            this.cmbBranch.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBranch.FormattingEnabled = true;
            this.cmbBranch.Items.AddRange(new object[] {
            "Y",
            "N"});
            this.cmbBranch.Location = new System.Drawing.Point(103, 97);
            this.cmbBranch.Name = "cmbBranch";
            this.cmbBranch.Size = new System.Drawing.Size(100, 21);
            this.cmbBranch.TabIndex = 227;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(35, 101);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(41, 13);
            this.label15.TabIndex = 226;
            this.label15.Text = "Branch";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(208, 75);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(16, 13);
            this.label2.TabIndex = 204;
            this.label2.Text = "to";
            // 
            // ddbackSalesToDate
            // 
            this.ddbackSalesToDate.Checked = false;
            this.ddbackSalesToDate.CustomFormat = "d/MMM/yyyy";
            this.ddbackSalesToDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.ddbackSalesToDate.Location = new System.Drawing.Point(226, 71);
            this.ddbackSalesToDate.Name = "ddbackSalesToDate";
            this.ddbackSalesToDate.ShowCheckBox = true;
            this.ddbackSalesToDate.Size = new System.Drawing.Size(103, 20);
            this.ddbackSalesToDate.TabIndex = 202;
            // 
            // ddbackSalesFormDate
            // 
            this.ddbackSalesFormDate.Checked = false;
            this.ddbackSalesFormDate.CustomFormat = "d/MMM/yyyy";
            this.ddbackSalesFormDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.ddbackSalesFormDate.Location = new System.Drawing.Point(104, 71);
            this.ddbackSalesFormDate.Name = "ddbackSalesFormDate";
            this.ddbackSalesFormDate.ShowCheckBox = true;
            this.ddbackSalesFormDate.Size = new System.Drawing.Size(102, 20);
            this.ddbackSalesFormDate.TabIndex = 201;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(14, 75);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(62, 13);
            this.label5.TabIndex = 203;
            this.label5.Text = "Sales Date:";
            // 
            // cmbPost
            // 
            this.cmbPost.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPost.FormattingEnabled = true;
            this.cmbPost.Items.AddRange(new object[] {
            "Y",
            "N"});
            this.cmbPost.Location = new System.Drawing.Point(512, 103);
            this.cmbPost.Name = "cmbPost";
            this.cmbPost.Size = new System.Drawing.Size(105, 21);
            this.cmbPost.TabIndex = 4;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(478, 109);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(28, 13);
            this.label9.TabIndex = 200;
            this.label9.Text = "Post";
            // 
            // txtFinishGood
            // 
            this.txtFinishGood.AccessibleName = "e";
            this.txtFinishGood.Location = new System.Drawing.Point(512, 73);
            this.txtFinishGood.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtFinishGood.MinimumSize = new System.Drawing.Size(185, 20);
            this.txtFinishGood.Name = "txtFinishGood";
            this.txtFinishGood.Size = new System.Drawing.Size(185, 20);
            this.txtFinishGood.TabIndex = 3;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(415, 77);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(94, 13);
            this.label7.TabIndex = 113;
            this.label7.Text = "Finish Good Name";
            // 
            // txtCustomerName
            // 
            this.txtCustomerName.AccessibleName = "e";
            this.txtCustomerName.Location = new System.Drawing.Point(512, 47);
            this.txtCustomerName.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtCustomerName.MinimumSize = new System.Drawing.Size(185, 20);
            this.txtCustomerName.Name = "txtCustomerName";
            this.txtCustomerName.Size = new System.Drawing.Size(185, 20);
            this.txtCustomerName.TabIndex = 3;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(415, 51);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(85, 13);
            this.label6.TabIndex = 113;
            this.label6.Text = "Customer Name:";
            // 
            // txtSalesInvoicNo
            // 
            this.txtSalesInvoicNo.AccessibleName = "e";
            this.txtSalesInvoicNo.Location = new System.Drawing.Point(512, 21);
            this.txtSalesInvoicNo.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtSalesInvoicNo.MinimumSize = new System.Drawing.Size(185, 20);
            this.txtSalesInvoicNo.Name = "txtSalesInvoicNo";
            this.txtSalesInvoicNo.Size = new System.Drawing.Size(185, 20);
            this.txtSalesInvoicNo.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(415, 25);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(91, 13);
            this.label4.TabIndex = 113;
            this.label4.Text = "SalesInvoicceNo:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(207, 49);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(16, 13);
            this.label11.TabIndex = 112;
            this.label11.Text = "to";
            // 
            // ddbackToDate
            // 
            this.ddbackToDate.Checked = false;
            this.ddbackToDate.CustomFormat = "d/MMM/yyyy";
            this.ddbackToDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.ddbackToDate.Location = new System.Drawing.Point(225, 45);
            this.ddbackToDate.Name = "ddbackToDate";
            this.ddbackToDate.ShowCheckBox = true;
            this.ddbackToDate.Size = new System.Drawing.Size(103, 20);
            this.ddbackToDate.TabIndex = 2;
            // 
            // ddbackFromDate
            // 
            this.ddbackFromDate.Checked = false;
            this.ddbackFromDate.CustomFormat = "d/MMM/yyyy";
            this.ddbackFromDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.ddbackFromDate.Location = new System.Drawing.Point(103, 45);
            this.ddbackFromDate.Name = "ddbackFromDate";
            this.ddbackFromDate.ShowCheckBox = true;
            this.ddbackFromDate.Size = new System.Drawing.Size(102, 20);
            this.ddbackFromDate.TabIndex = 1;
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSearch.Image = global::VATClient.Properties.Resources.search;
            this.btnSearch.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSearch.Location = new System.Drawing.Point(622, 98);
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
            this.label3.Location = new System.Drawing.Point(13, 49);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(83, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "D.D.Back Date:";
            // 
            // txtDDBackNo
            // 
            this.txtDDBackNo.Location = new System.Drawing.Point(104, 21);
            this.txtDDBackNo.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtDDBackNo.MinimumSize = new System.Drawing.Size(185, 20);
            this.txtDDBackNo.Name = "txtDDBackNo";
            this.txtDDBackNo.Size = new System.Drawing.Size(200, 20);
            this.txtDDBackNo.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "D.D.Back No:";
            // 
            // LRecordCount
            // 
            this.LRecordCount.AutoSize = true;
            this.LRecordCount.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LRecordCount.Location = new System.Drawing.Point(42, 414);
            this.LRecordCount.Name = "LRecordCount";
            this.LRecordCount.Size = new System.Drawing.Size(94, 16);
            this.LRecordCount.TabIndex = 223;
            this.LRecordCount.Text = "Record Count :";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(76, 214);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(575, 25);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 111;
            this.progressBar1.Visible = false;
            // 
            // dgvDDBackHistory
            // 
            this.dgvDDBackHistory.AllowUserToAddRows = false;
            this.dgvDDBackHistory.AllowUserToDeleteRows = false;
            this.dgvDDBackHistory.AllowUserToOrderColumns = true;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black;
            this.dgvDDBackHistory.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvDDBackHistory.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvDDBackHistory.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDDBackHistory.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.DDBackNo,
            this.DDBackDate,
            this.SalesInvoiceNo,
            this.SalesDate,
            this.CustormerID,
            this.CustormerName,
            this.CurrencyId,
            this.CurrencyCode,
            this.ExpCurrency,
            this.BDTCurrency,
            this.FgItemNo,
            this.FgItemName,
            this.TotalClaimCD,
            this.TotalClaimRD,
            this.TotalClaimSD,
            this.TotalDDBack,
            this.TotalClaimVAT,
            this.TotalClaimCnFAmount,
            this.TotalClaimInsuranceAmount,
            this.TotalClaimTVBAmount,
            this.TotalClaimTVAAmount,
            this.TotalClaimATVAmount,
            this.TotalClaimOthersAmount,
            this.Comments,
            this.CreatedBy,
            this.CreatedOn,
            this.LastModifiedBy,
            this.LastModifiedOn,
            this.Post,
            this.BranchId,
            this.ApprovedSD,
            this.TotalSDAmount});
            this.dgvDDBackHistory.Location = new System.Drawing.Point(7, 152);
            this.dgvDDBackHistory.Name = "dgvDDBackHistory";
            this.dgvDDBackHistory.RowHeadersVisible = false;
            this.dgvDDBackHistory.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvDDBackHistory.Size = new System.Drawing.Size(756, 255);
            this.dgvDDBackHistory.TabIndex = 110;
            this.dgvDDBackHistory.DoubleClick += new System.EventHandler(this.dgvDDBackHistory_DoubleClick);
            // 
            // backgroundWorkerSearch
            // 
            this.backgroundWorkerSearch.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerSearch_DoWork);
            this.backgroundWorkerSearch.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerSearch_RunWorkerCompleted);
            // 
            // DDBackNo
            // 
            this.DDBackNo.HeaderText = "DDBackNo";
            this.DDBackNo.Name = "DDBackNo";
            this.DDBackNo.ReadOnly = true;
            this.DDBackNo.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // DDBackDate
            // 
            this.DDBackDate.HeaderText = "DDBackDate";
            this.DDBackDate.Name = "DDBackDate";
            this.DDBackDate.ReadOnly = true;
            this.DDBackDate.Visible = false;
            // 
            // SalesInvoiceNo
            // 
            this.SalesInvoiceNo.FillWeight = 150F;
            this.SalesInvoiceNo.HeaderText = "Sales InvoiceNo";
            this.SalesInvoiceNo.Name = "SalesInvoiceNo";
            this.SalesInvoiceNo.ReadOnly = true;
            this.SalesInvoiceNo.Width = 150;
            // 
            // SalesDate
            // 
            this.SalesDate.HeaderText = "SalesDate";
            this.SalesDate.Name = "SalesDate";
            this.SalesDate.ReadOnly = true;
            // 
            // CustormerID
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.CustormerID.DefaultCellStyle = dataGridViewCellStyle2;
            this.CustormerID.HeaderText = "Custormer ID";
            this.CustormerID.Name = "CustormerID";
            this.CustormerID.ReadOnly = true;
            this.CustormerID.Visible = false;
            // 
            // CustormerName
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.CustormerName.DefaultCellStyle = dataGridViewCellStyle3;
            this.CustormerName.FillWeight = 130F;
            this.CustormerName.HeaderText = "Custormer Name";
            this.CustormerName.Name = "CustormerName";
            this.CustormerName.ReadOnly = true;
            this.CustormerName.Width = 150;
            // 
            // CurrencyId
            // 
            this.CurrencyId.HeaderText = "Currency Id";
            this.CurrencyId.Name = "CurrencyId";
            this.CurrencyId.ReadOnly = true;
            this.CurrencyId.Visible = false;
            // 
            // CurrencyCode
            // 
            this.CurrencyCode.HeaderText = "CurrencyName";
            this.CurrencyCode.Name = "CurrencyCode";
            // 
            // ExpCurrency
            // 
            this.ExpCurrency.HeaderText = "Exp Currency";
            this.ExpCurrency.Name = "ExpCurrency";
            // 
            // BDTCurrency
            // 
            this.BDTCurrency.HeaderText = "BDT Currency";
            this.BDTCurrency.Name = "BDTCurrency";
            // 
            // FgItemNo
            // 
            this.FgItemNo.HeaderText = "FgItem No";
            this.FgItemNo.Name = "FgItemNo";
            // 
            // FgItemName
            // 
            this.FgItemName.FillWeight = 150F;
            this.FgItemName.HeaderText = "FgItem Name";
            this.FgItemName.Name = "FgItemName";
            // 
            // TotalClaimCD
            // 
            this.TotalClaimCD.FillWeight = 150F;
            this.TotalClaimCD.HeaderText = "Total ClaimCD";
            this.TotalClaimCD.Name = "TotalClaimCD";
            // 
            // TotalClaimRD
            // 
            this.TotalClaimRD.FillWeight = 150F;
            this.TotalClaimRD.HeaderText = "Total ClaimRD";
            this.TotalClaimRD.Name = "TotalClaimRD";
            // 
            // TotalClaimSD
            // 
            this.TotalClaimSD.HeaderText = "Total ClaimSD";
            this.TotalClaimSD.Name = "TotalClaimSD";
            this.TotalClaimSD.Width = 150;
            // 
            // TotalDDBack
            // 
            this.TotalDDBack.HeaderText = "Total DDBack";
            this.TotalDDBack.Name = "TotalDDBack";
            this.TotalDDBack.Width = 150;
            // 
            // TotalClaimVAT
            // 
            this.TotalClaimVAT.HeaderText = "Total ClaimVAT";
            this.TotalClaimVAT.Name = "TotalClaimVAT";
            this.TotalClaimVAT.Width = 150;
            // 
            // TotalClaimCnFAmount
            // 
            this.TotalClaimCnFAmount.HeaderText = "Total ClaimCnFAmount";
            this.TotalClaimCnFAmount.Name = "TotalClaimCnFAmount";
            this.TotalClaimCnFAmount.Width = 150;
            // 
            // TotalClaimInsuranceAmount
            // 
            this.TotalClaimInsuranceAmount.HeaderText = "Total ClaimInsuranceAmount";
            this.TotalClaimInsuranceAmount.Name = "TotalClaimInsuranceAmount";
            this.TotalClaimInsuranceAmount.Width = 180;
            // 
            // TotalClaimTVBAmount
            // 
            this.TotalClaimTVBAmount.HeaderText = "Total ClaimTVBAmount";
            this.TotalClaimTVBAmount.Name = "TotalClaimTVBAmount";
            this.TotalClaimTVBAmount.Width = 150;
            // 
            // TotalClaimTVAAmount
            // 
            this.TotalClaimTVAAmount.HeaderText = "Total ClaimTVAAmount";
            this.TotalClaimTVAAmount.Name = "TotalClaimTVAAmount";
            this.TotalClaimTVAAmount.Width = 150;
            // 
            // TotalClaimATVAmount
            // 
            this.TotalClaimATVAmount.HeaderText = "Total ClaimATVAmount";
            this.TotalClaimATVAmount.Name = "TotalClaimATVAmount";
            this.TotalClaimATVAmount.Width = 150;
            // 
            // TotalClaimOthersAmount
            // 
            this.TotalClaimOthersAmount.HeaderText = "Total ClaimOthersAmount";
            this.TotalClaimOthersAmount.Name = "TotalClaimOthersAmount";
            this.TotalClaimOthersAmount.Width = 180;
            // 
            // Comments
            // 
            this.Comments.HeaderText = "Comments";
            this.Comments.Name = "Comments";
            this.Comments.Width = 150;
            // 
            // CreatedBy
            // 
            this.CreatedBy.HeaderText = "CreatedBy";
            this.CreatedBy.Name = "CreatedBy";
            this.CreatedBy.Visible = false;
            this.CreatedBy.Width = 130;
            // 
            // CreatedOn
            // 
            this.CreatedOn.HeaderText = "CreatedOn";
            this.CreatedOn.Name = "CreatedOn";
            this.CreatedOn.Visible = false;
            // 
            // LastModifiedBy
            // 
            this.LastModifiedBy.HeaderText = "LastModifiedBy";
            this.LastModifiedBy.Name = "LastModifiedBy";
            this.LastModifiedBy.Visible = false;
            // 
            // LastModifiedOn
            // 
            this.LastModifiedOn.HeaderText = "LastModifiedOn";
            this.LastModifiedOn.Name = "LastModifiedOn";
            this.LastModifiedOn.Visible = false;
            // 
            // Post
            // 
            this.Post.HeaderText = "Post";
            this.Post.Name = "Post";
            this.Post.Width = 10;
            // 
            // BranchId
            // 
            this.BranchId.HeaderText = "Branch Id";
            this.BranchId.Name = "BranchId";
            this.BranchId.ReadOnly = true;
            this.BranchId.Visible = false;
            // 
            // ApprovedSD
            // 
            this.ApprovedSD.HeaderText = "Approved SD";
            this.ApprovedSD.Name = "ApprovedSD";
            this.ApprovedSD.ReadOnly = true;
            // 
            // TotalSDAmount
            // 
            this.TotalSDAmount.HeaderText = "Total SD Amount";
            this.TotalSDAmount.Name = "TotalSDAmount";
            this.TotalSDAmount.ReadOnly = true;
            // 
            // FormDutyDrawBackSearch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(768, 436);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.dgvDDBackHistory);
            this.Controls.Add(this.LRecordCount);
            this.Controls.Add(this.grbTransactionHistory);
            this.Name = "FormDutyDrawBackSearch";
            this.Text = "FormDutyDrawBackSearch";
            this.Load += new System.EventHandler(this.FormDutyDrawBackSearch_Load);
            this.grbTransactionHistory.ResumeLayout(false);
            this.grbTransactionHistory.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDDBackHistory)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox grbTransactionHistory;
        private System.Windows.Forms.ComboBox cmbPost;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtSalesInvoicNo;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.DateTimePicker ddbackToDate;
        private System.Windows.Forms.DateTimePicker ddbackFromDate;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtDDBackNo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.DataGridView dgvDDBackHistory;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker ddbackSalesToDate;
        private System.Windows.Forms.DateTimePicker ddbackSalesFormDate;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtCustomerName;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtFinishGood;
        private System.Windows.Forms.Label label7;
        private System.ComponentModel.BackgroundWorker backgroundWorkerSearch;
        private System.Windows.Forms.Label LRecordCount;
        private System.Windows.Forms.ComboBox cmbBranch;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.DataGridViewTextBoxColumn DDBackNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn DDBackDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn SalesInvoiceNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn SalesDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn CustormerID;
        private System.Windows.Forms.DataGridViewTextBoxColumn CustormerName;
        private System.Windows.Forms.DataGridViewTextBoxColumn CurrencyId;
        private System.Windows.Forms.DataGridViewTextBoxColumn CurrencyCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn ExpCurrency;
        private System.Windows.Forms.DataGridViewTextBoxColumn BDTCurrency;
        private System.Windows.Forms.DataGridViewTextBoxColumn FgItemNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn FgItemName;
        private System.Windows.Forms.DataGridViewTextBoxColumn TotalClaimCD;
        private System.Windows.Forms.DataGridViewTextBoxColumn TotalClaimRD;
        private System.Windows.Forms.DataGridViewTextBoxColumn TotalClaimSD;
        private System.Windows.Forms.DataGridViewTextBoxColumn TotalDDBack;
        private System.Windows.Forms.DataGridViewTextBoxColumn TotalClaimVAT;
        private System.Windows.Forms.DataGridViewTextBoxColumn TotalClaimCnFAmount;
        private System.Windows.Forms.DataGridViewTextBoxColumn TotalClaimInsuranceAmount;
        private System.Windows.Forms.DataGridViewTextBoxColumn TotalClaimTVBAmount;
        private System.Windows.Forms.DataGridViewTextBoxColumn TotalClaimTVAAmount;
        private System.Windows.Forms.DataGridViewTextBoxColumn TotalClaimATVAmount;
        private System.Windows.Forms.DataGridViewTextBoxColumn TotalClaimOthersAmount;
        private System.Windows.Forms.DataGridViewTextBoxColumn Comments;
        private System.Windows.Forms.DataGridViewTextBoxColumn CreatedBy;
        private System.Windows.Forms.DataGridViewTextBoxColumn CreatedOn;
        private System.Windows.Forms.DataGridViewTextBoxColumn LastModifiedBy;
        private System.Windows.Forms.DataGridViewTextBoxColumn LastModifiedOn;
        private System.Windows.Forms.DataGridViewTextBoxColumn Post;
        private System.Windows.Forms.DataGridViewTextBoxColumn BranchId;
        private System.Windows.Forms.DataGridViewTextBoxColumn ApprovedSD;
        private System.Windows.Forms.DataGridViewTextBoxColumn TotalSDAmount;
    }
}