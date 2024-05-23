namespace VATClient
{
    partial class FormDemandSearch
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.dgvDemandHistory = new System.Windows.Forms.DataGridView();
            this.DemandNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DemandDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ReceiveDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RefNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FiscalYear = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TotalQty = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Post = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Comments = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MonthFrom = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MonthTo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DemandReceiveID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.VehicleID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.VehicleType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.VehicleNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DriverName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RefDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BranchId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.txtSerialNo = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.grbTransactionHistory = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbBranch = new System.Windows.Forms.ComboBox();
            this.cmbPost = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtReceiveNo = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.dtpDemandToDate = new System.Windows.Forms.DateTimePicker();
            this.dtpDemandFromDate = new System.Windows.Forms.DateTimePicker();
            this.btnSearch = new System.Windows.Forms.Button();
            this.lblDate = new System.Windows.Forms.Label();
            this.txtDemandNo = new System.Windows.Forms.TextBox();
            this.lblName = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.LRecordCount = new System.Windows.Forms.Label();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.backgroundWorkerSearch = new System.ComponentModel.BackgroundWorker();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDemandHistory)).BeginInit();
            this.grbTransactionHistory.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.progressBar1);
            this.groupBox1.Controls.Add(this.dgvDemandHistory);
            this.groupBox1.Controls.Add(this.txtSerialNo);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(12, 103);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(756, 312);
            this.groupBox1.TabIndex = 108;
            this.groupBox1.TabStop = false;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(44, 71);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(575, 25);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 109;
            this.progressBar1.Visible = false;
            // 
            // dgvDemandHistory
            // 
            this.dgvDemandHistory.AllowUserToAddRows = false;
            this.dgvDemandHistory.AllowUserToDeleteRows = false;
            this.dgvDemandHistory.AllowUserToOrderColumns = true;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black;
            this.dgvDemandHistory.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvDemandHistory.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvDemandHistory.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvDemandHistory.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDemandHistory.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.DemandNo,
            this.DemandDate,
            this.ReceiveDate,
            this.RefNo,
            this.FiscalYear,
            this.MName,
            this.TotalQty,
            this.Post,
            this.Comments,
            this.MonthFrom,
            this.MonthTo,
            this.DemandReceiveID,
            this.VehicleID,
            this.VehicleType,
            this.VehicleNo,
            this.DriverName,
            this.RefDate,
            this.BranchId});
            this.dgvDemandHistory.Location = new System.Drawing.Point(6, 11);
            this.dgvDemandHistory.Name = "dgvDemandHistory";
            this.dgvDemandHistory.RowHeadersVisible = false;
            this.dgvDemandHistory.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvDemandHistory.Size = new System.Drawing.Size(744, 295);
            this.dgvDemandHistory.TabIndex = 6;
            this.dgvDemandHistory.DoubleClick += new System.EventHandler(this.dgvDemandHistory_DoubleClick);
            // 
            // DemandNo
            // 
            this.DemandNo.HeaderText = "Demand No";
            this.DemandNo.Name = "DemandNo";
            this.DemandNo.ReadOnly = true;
            this.DemandNo.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // DemandDate
            // 
            this.DemandDate.HeaderText = "Demand Date";
            this.DemandDate.Name = "DemandDate";
            this.DemandDate.ReadOnly = true;
            // 
            // ReceiveDate
            // 
            this.ReceiveDate.HeaderText = "ReceiveDate";
            this.ReceiveDate.Name = "ReceiveDate";
            this.ReceiveDate.Visible = false;
            // 
            // RefNo
            // 
            this.RefNo.HeaderText = "RefNo";
            this.RefNo.Name = "RefNo";
            this.RefNo.Visible = false;
            // 
            // FiscalYear
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.FiscalYear.DefaultCellStyle = dataGridViewCellStyle2;
            this.FiscalYear.HeaderText = "Fiscal Year";
            this.FiscalYear.Name = "FiscalYear";
            this.FiscalYear.ReadOnly = true;
            // 
            // MName
            // 
            this.MName.HeaderText = "Month Name";
            this.MName.Name = "MName";
            this.MName.ReadOnly = true;
            // 
            // TotalQty
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.TotalQty.DefaultCellStyle = dataGridViewCellStyle3;
            this.TotalQty.HeaderText = "Total Quantity";
            this.TotalQty.Name = "TotalQty";
            this.TotalQty.ReadOnly = true;
            // 
            // Post
            // 
            this.Post.HeaderText = "Post";
            this.Post.Name = "Post";
            this.Post.ReadOnly = true;
            this.Post.Visible = false;
            // 
            // Comments
            // 
            this.Comments.HeaderText = "Comments";
            this.Comments.Name = "Comments";
            this.Comments.ReadOnly = true;
            this.Comments.Visible = false;
            // 
            // MonthFrom
            // 
            this.MonthFrom.HeaderText = "MonthFrom";
            this.MonthFrom.Name = "MonthFrom";
            this.MonthFrom.Visible = false;
            // 
            // MonthTo
            // 
            this.MonthTo.HeaderText = "MonthTo";
            this.MonthTo.Name = "MonthTo";
            this.MonthTo.Visible = false;
            // 
            // DemandReceiveID
            // 
            this.DemandReceiveID.HeaderText = "Demand No";
            this.DemandReceiveID.Name = "DemandReceiveID";
            this.DemandReceiveID.Visible = false;
            // 
            // VehicleID
            // 
            this.VehicleID.HeaderText = "VehicleID";
            this.VehicleID.Name = "VehicleID";
            this.VehicleID.Visible = false;
            // 
            // VehicleType
            // 
            this.VehicleType.HeaderText = "Vehicle Type";
            this.VehicleType.Name = "VehicleType";
            this.VehicleType.Visible = false;
            // 
            // VehicleNo
            // 
            this.VehicleNo.HeaderText = "Vehicle No";
            this.VehicleNo.Name = "VehicleNo";
            this.VehicleNo.Visible = false;
            // 
            // DriverName
            // 
            this.DriverName.HeaderText = "DriverName";
            this.DriverName.Name = "DriverName";
            this.DriverName.Visible = false;
            // 
            // RefDate
            // 
            this.RefDate.HeaderText = "RefDate";
            this.RefDate.Name = "RefDate";
            this.RefDate.Visible = false;
            // 
            // BranchId
            // 
            this.BranchId.HeaderText = "BranchId";
            this.BranchId.Name = "BranchId";
            this.BranchId.ReadOnly = true;
            // 
            // txtSerialNo
            // 
            this.txtSerialNo.Location = new System.Drawing.Point(763, 79);
            this.txtSerialNo.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtSerialNo.MinimumSize = new System.Drawing.Size(185, 20);
            this.txtSerialNo.Name = "txtSerialNo";
            this.txtSerialNo.Size = new System.Drawing.Size(185, 21);
            this.txtSerialNo.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(688, 83);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Serial No:";
            // 
            // grbTransactionHistory
            // 
            this.grbTransactionHistory.Controls.Add(this.label1);
            this.grbTransactionHistory.Controls.Add(this.cmbBranch);
            this.grbTransactionHistory.Controls.Add(this.cmbPost);
            this.grbTransactionHistory.Controls.Add(this.label9);
            this.grbTransactionHistory.Controls.Add(this.txtReceiveNo);
            this.grbTransactionHistory.Controls.Add(this.label4);
            this.grbTransactionHistory.Controls.Add(this.label11);
            this.grbTransactionHistory.Controls.Add(this.dtpDemandToDate);
            this.grbTransactionHistory.Controls.Add(this.dtpDemandFromDate);
            this.grbTransactionHistory.Controls.Add(this.btnSearch);
            this.grbTransactionHistory.Controls.Add(this.lblDate);
            this.grbTransactionHistory.Controls.Add(this.txtDemandNo);
            this.grbTransactionHistory.Controls.Add(this.lblName);
            this.grbTransactionHistory.Location = new System.Drawing.Point(12, 12);
            this.grbTransactionHistory.Name = "grbTransactionHistory";
            this.grbTransactionHistory.Size = new System.Drawing.Size(756, 90);
            this.grbTransactionHistory.TabIndex = 107;
            this.grbTransactionHistory.TabStop = false;
            this.grbTransactionHistory.Text = "Searching Criteria";
            this.grbTransactionHistory.Enter += new System.EventHandler(this.grbTransactionHistory_Enter);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(374, 54);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 13);
            this.label1.TabIndex = 217;
            this.label1.Text = "Branch";
            // 
            // cmbBranch
            // 
            this.cmbBranch.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBranch.FormattingEnabled = true;
            this.cmbBranch.Items.AddRange(new object[] {
            "Cash Payable",
            "Credit Payable",
            "Rebatable"});
            this.cmbBranch.Location = new System.Drawing.Point(419, 50);
            this.cmbBranch.Name = "cmbBranch";
            this.cmbBranch.Size = new System.Drawing.Size(96, 21);
            this.cmbBranch.Sorted = true;
            this.cmbBranch.TabIndex = 216;
            // 
            // cmbPost
            // 
            this.cmbPost.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPost.FormattingEnabled = true;
            this.cmbPost.Items.AddRange(new object[] {
            "Y",
            "N"});
            this.cmbPost.Location = new System.Drawing.Point(569, 51);
            this.cmbPost.Name = "cmbPost";
            this.cmbPost.Size = new System.Drawing.Size(66, 21);
            this.cmbPost.TabIndex = 4;
            this.cmbPost.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmbPost_KeyDown);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(523, 55);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(28, 13);
            this.label9.TabIndex = 200;
            this.label9.Text = "Post";
            // 
            // txtReceiveNo
            // 
            this.txtReceiveNo.Location = new System.Drawing.Point(420, 21);
            this.txtReceiveNo.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtReceiveNo.MinimumSize = new System.Drawing.Size(185, 20);
            this.txtReceiveNo.Name = "txtReceiveNo";
            this.txtReceiveNo.Size = new System.Drawing.Size(185, 21);
            this.txtReceiveNo.TabIndex = 3;
            this.txtReceiveNo.Visible = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(345, 25);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 13);
            this.label4.TabIndex = 113;
            this.label4.Text = "Receive No:";
            this.label4.Visible = false;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(207, 50);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(17, 13);
            this.label11.TabIndex = 112;
            this.label11.Text = "to";
            // 
            // dtpDemandToDate
            // 
            this.dtpDemandToDate.Checked = false;
            this.dtpDemandToDate.CustomFormat = "d/MMM/yyyy";
            this.dtpDemandToDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpDemandToDate.Location = new System.Drawing.Point(225, 46);
            this.dtpDemandToDate.Name = "dtpDemandToDate";
            this.dtpDemandToDate.ShowCheckBox = true;
            this.dtpDemandToDate.Size = new System.Drawing.Size(103, 21);
            this.dtpDemandToDate.TabIndex = 2;
            this.dtpDemandToDate.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dtpDemandToDate_KeyDown);
            // 
            // dtpDemandFromDate
            // 
            this.dtpDemandFromDate.Checked = false;
            this.dtpDemandFromDate.CustomFormat = "d/MMM/yyyy";
            this.dtpDemandFromDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpDemandFromDate.Location = new System.Drawing.Point(103, 46);
            this.dtpDemandFromDate.Name = "dtpDemandFromDate";
            this.dtpDemandFromDate.ShowCheckBox = true;
            this.dtpDemandFromDate.Size = new System.Drawing.Size(102, 21);
            this.dtpDemandFromDate.TabIndex = 1;
            this.dtpDemandFromDate.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dtpDemandFromDate_KeyDown);
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSearch.Image = global::VATClient.Properties.Resources.search;
            this.btnSearch.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSearch.Location = new System.Drawing.Point(666, 47);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 28);
            this.btnSearch.TabIndex = 5;
            this.btnSearch.Text = "&Search";
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // lblDate
            // 
            this.lblDate.AutoSize = true;
            this.lblDate.Location = new System.Drawing.Point(28, 50);
            this.lblDate.Name = "lblDate";
            this.lblDate.Size = new System.Drawing.Size(76, 13);
            this.lblDate.TabIndex = 3;
            this.lblDate.Text = "Demand Date:";
            // 
            // txtDemandNo
            // 
            this.txtDemandNo.Location = new System.Drawing.Point(103, 21);
            this.txtDemandNo.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtDemandNo.MinimumSize = new System.Drawing.Size(185, 20);
            this.txtDemandNo.Name = "txtDemandNo";
            this.txtDemandNo.Size = new System.Drawing.Size(200, 21);
            this.txtDemandNo.TabIndex = 0;
            this.txtDemandNo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtDemandNo_KeyDown);
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(28, 25);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(66, 13);
            this.lblName.TabIndex = 0;
            this.lblName.Text = "Demand No:";
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::VATClient.Properties.Resources.Green_Footer;
            this.panel1.Controls.Add(this.LRecordCount);
            this.panel1.Controls.Add(this.btnOk);
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Location = new System.Drawing.Point(-1, 421);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(793, 40);
            this.panel1.TabIndex = 6;
            // 
            // LRecordCount
            // 
            this.LRecordCount.AutoSize = true;
            this.LRecordCount.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LRecordCount.Location = new System.Drawing.Point(98, 13);
            this.LRecordCount.Name = "LRecordCount";
            this.LRecordCount.Size = new System.Drawing.Size(94, 16);
            this.LRecordCount.TabIndex = 223;
            this.LRecordCount.Text = "Record Count :";
            // 
            // btnOk
            // 
            this.btnOk.BackColor = System.Drawing.SystemColors.ButtonHighlight;
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
            // backgroundWorkerSearch
            // 
            this.backgroundWorkerSearch.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerSearch_DoWork);
            this.backgroundWorkerSearch.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerSearch_RunWorkerCompleted);
            // 
            // FormDemandSearch
            // 
            this.AcceptButton = this.btnSearch;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.CancelButton = this.btnOk;
            this.ClientSize = new System.Drawing.Size(784, 461);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.grbTransactionHistory);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(800, 500);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(800, 500);
            this.Name = "FormDemandSearch";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Demand Search";
            this.Load += new System.EventHandler(this.FormMonthlyDemandSearch_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDemandHistory)).EndInit();
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
        private System.Windows.Forms.DataGridView dgvDemandHistory;
        private System.Windows.Forms.GroupBox grbTransactionHistory;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.DateTimePicker dtpDemandToDate;
        private System.Windows.Forms.DateTimePicker dtpDemandFromDate;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.TextBox txtSerialNo;
        private System.Windows.Forms.Label lblDate;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtDemandNo;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.TextBox txtReceiveNo;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cmbPost;
        private System.Windows.Forms.Label label9;
        private System.ComponentModel.BackgroundWorker backgroundWorkerSearch;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label LRecordCount;
        private System.Windows.Forms.DataGridViewTextBoxColumn DemandNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn DemandDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn ReceiveDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn RefNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn FiscalYear;
        private System.Windows.Forms.DataGridViewTextBoxColumn MName;
        private System.Windows.Forms.DataGridViewTextBoxColumn TotalQty;
        private System.Windows.Forms.DataGridViewTextBoxColumn Post;
        private System.Windows.Forms.DataGridViewTextBoxColumn Comments;
        private System.Windows.Forms.DataGridViewTextBoxColumn MonthFrom;
        private System.Windows.Forms.DataGridViewTextBoxColumn MonthTo;
        private System.Windows.Forms.DataGridViewTextBoxColumn DemandReceiveID;
        private System.Windows.Forms.DataGridViewTextBoxColumn VehicleID;
        private System.Windows.Forms.DataGridViewTextBoxColumn VehicleType;
        private System.Windows.Forms.DataGridViewTextBoxColumn VehicleNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn DriverName;
        private System.Windows.Forms.DataGridViewTextBoxColumn RefDate;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.ComboBox cmbBranch;
        private System.Windows.Forms.DataGridViewTextBoxColumn BranchId;
    }
}