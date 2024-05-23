namespace VATClient
{
    partial class FormSearchUserLog
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
            this.dgvLogs = new System.Windows.Forms.DataGridView();
            this.LogID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SoftwareUserId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SoftwareUserName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SessionDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LogInDateTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LogOutDateTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ComputerName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ComputerLoginUserName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ComputerIPAddress = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label11 = new System.Windows.Forms.Label();
            this.dtpIssueToDate = new System.Windows.Forms.DateTimePicker();
            this.dtpIssueFromDate = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbSoftwareUserName = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtComputerLoginUserName = new System.Windows.Forms.TextBox();
            this.txtCostPriceTo = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtSalesPriceTo = new System.Windows.Forms.TextBox();
            this.txtNBRPriceTo = new System.Windows.Forms.TextBox();
            this.txtCostPriceFrom = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtSalesPriceFrom = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtNBRPriceFrom = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.btnSearch = new System.Windows.Forms.Button();
            this.txtComputerName = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.backgroundWorkerSearch = new System.ComponentModel.BackgroundWorker();
            this.LRecordCount = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.bgwUserSearch = new System.ComponentModel.BackgroundWorker();
            this.btnCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLogs)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvLogs
            // 
            this.dgvLogs.AllowUserToAddRows = false;
            this.dgvLogs.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.dgvLogs.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvLogs.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvLogs.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.LogID,
            this.SoftwareUserId,
            this.SoftwareUserName,
            this.SessionDate,
            this.LogInDateTime,
            this.LogOutDateTime,
            this.ComputerName,
            this.ComputerLoginUserName,
            this.ComputerIPAddress});
            this.dgvLogs.Location = new System.Drawing.Point(12, 93);
            this.dgvLogs.Name = "dgvLogs";
            this.dgvLogs.RowHeadersVisible = false;
            this.dgvLogs.RowTemplate.Height = 24;
            this.dgvLogs.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvLogs.Size = new System.Drawing.Size(746, 197);
            this.dgvLogs.TabIndex = 78;
            // 
            // LogID
            // 
            this.LogID.HeaderText = "LogID";
            this.LogID.Name = "LogID";
            this.LogID.ReadOnly = true;
            // 
            // SoftwareUserId
            // 
            this.SoftwareUserId.HeaderText = "Software User ID";
            this.SoftwareUserId.Name = "SoftwareUserId";
            this.SoftwareUserId.ReadOnly = true;
            // 
            // SoftwareUserName
            // 
            this.SoftwareUserName.HeaderText = "Software User Name";
            this.SoftwareUserName.Name = "SoftwareUserName";
            this.SoftwareUserName.ReadOnly = true;
            // 
            // SessionDate
            // 
            this.SessionDate.HeaderText = "Session Date";
            this.SessionDate.Name = "SessionDate";
            this.SessionDate.ReadOnly = true;
            // 
            // LogInDateTime
            // 
            this.LogInDateTime.HeaderText = "LogIn Date";
            this.LogInDateTime.Name = "LogInDateTime";
            this.LogInDateTime.ReadOnly = true;
            // 
            // LogOutDateTime
            // 
            this.LogOutDateTime.HeaderText = "LogOut Date";
            this.LogOutDateTime.Name = "LogOutDateTime";
            this.LogOutDateTime.ReadOnly = true;
            // 
            // ComputerName
            // 
            this.ComputerName.HeaderText = "Computer Name";
            this.ComputerName.Name = "ComputerName";
            this.ComputerName.ReadOnly = true;
            // 
            // ComputerLoginUserName
            // 
            this.ComputerLoginUserName.HeaderText = "Computer User Name";
            this.ComputerLoginUserName.Name = "ComputerLoginUserName";
            this.ComputerLoginUserName.ReadOnly = true;
            // 
            // ComputerIPAddress
            // 
            this.ComputerIPAddress.HeaderText = "IP Address";
            this.ComputerIPAddress.Name = "ComputerIPAddress";
            this.ComputerIPAddress.ReadOnly = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.dtpIssueToDate);
            this.groupBox1.Controls.Add(this.dtpIssueFromDate);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.cmbSoftwareUserName);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.txtComputerLoginUserName);
            this.groupBox1.Controls.Add(this.txtCostPriceTo);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.txtSalesPriceTo);
            this.groupBox1.Controls.Add(this.txtNBRPriceTo);
            this.groupBox1.Controls.Add(this.txtCostPriceFrom);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.txtSalesPriceFrom);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtNBRPriceFrom);
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.btnSearch);
            this.groupBox1.Controls.Add(this.txtComputerName);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Location = new System.Drawing.Point(10, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(748, 75);
            this.groupBox1.TabIndex = 105;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Searching Criteria";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(232, 49);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(16, 13);
            this.label11.TabIndex = 205;
            this.label11.Text = "to";
            // 
            // dtpIssueToDate
            // 
            this.dtpIssueToDate.Checked = false;
            this.dtpIssueToDate.CustomFormat = "d/MMM/yyyy";
            this.dtpIssueToDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpIssueToDate.Location = new System.Drawing.Point(250, 45);
            this.dtpIssueToDate.Name = "dtpIssueToDate";
            this.dtpIssueToDate.ShowCheckBox = true;
            this.dtpIssueToDate.Size = new System.Drawing.Size(103, 20);
            this.dtpIssueToDate.TabIndex = 203;
            // 
            // dtpIssueFromDate
            // 
            this.dtpIssueFromDate.Checked = false;
            this.dtpIssueFromDate.CustomFormat = "d/MMM/yyyy";
            this.dtpIssueFromDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpIssueFromDate.Location = new System.Drawing.Point(128, 45);
            this.dtpIssueFromDate.Name = "dtpIssueFromDate";
            this.dtpIssueFromDate.ShowCheckBox = true;
            this.dtpIssueFromDate.Size = new System.Drawing.Size(102, 20);
            this.dtpIssueFromDate.TabIndex = 202;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(53, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 13);
            this.label2.TabIndex = 204;
            this.label2.Text = "Login Date:";
            // 
            // cmbSoftwareUserName
            // 
            this.cmbSoftwareUserName.FormattingEnabled = true;
            this.cmbSoftwareUserName.Location = new System.Drawing.Point(128, 19);
            this.cmbSoftwareUserName.Name = "cmbSoftwareUserName";
            this.cmbSoftwareUserName.Size = new System.Drawing.Size(225, 21);
            this.cmbSoftwareUserName.Sorted = true;
            this.cmbSoftwareUserName.TabIndex = 2;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(372, 45);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(77, 13);
            this.label8.TabIndex = 201;
            this.label8.Text = "Computer User";
            // 
            // txtComputerLoginUserName
            // 
            this.txtComputerLoginUserName.Location = new System.Drawing.Point(468, 42);
            this.txtComputerLoginUserName.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtComputerLoginUserName.MinimumSize = new System.Drawing.Size(185, 20);
            this.txtComputerLoginUserName.Name = "txtComputerLoginUserName";
            this.txtComputerLoginUserName.Size = new System.Drawing.Size(185, 20);
            this.txtComputerLoginUserName.TabIndex = 0;
            // 
            // txtCostPriceTo
            // 
            this.txtCostPriceTo.Location = new System.Drawing.Point(938, 88);
            this.txtCostPriceTo.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtCostPriceTo.MinimumSize = new System.Drawing.Size(65, 20);
            this.txtCostPriceTo.Name = "txtCostPriceTo";
            this.txtCostPriceTo.Size = new System.Drawing.Size(65, 20);
            this.txtCostPriceTo.TabIndex = 142;
            this.txtCostPriceTo.Text = "0.00";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(776, 114);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(54, 13);
            this.label6.TabIndex = 134;
            this.label6.Text = "VAT Rate";
            // 
            // txtSalesPriceTo
            // 
            this.txtSalesPriceTo.Location = new System.Drawing.Point(938, 113);
            this.txtSalesPriceTo.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtSalesPriceTo.MinimumSize = new System.Drawing.Size(65, 20);
            this.txtSalesPriceTo.Name = "txtSalesPriceTo";
            this.txtSalesPriceTo.Size = new System.Drawing.Size(65, 20);
            this.txtSalesPriceTo.TabIndex = 141;
            this.txtSalesPriceTo.Text = "0.00";
            // 
            // txtNBRPriceTo
            // 
            this.txtNBRPriceTo.Location = new System.Drawing.Point(938, 138);
            this.txtNBRPriceTo.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtNBRPriceTo.MinimumSize = new System.Drawing.Size(65, 20);
            this.txtNBRPriceTo.Name = "txtNBRPriceTo";
            this.txtNBRPriceTo.Size = new System.Drawing.Size(65, 20);
            this.txtNBRPriceTo.TabIndex = 140;
            this.txtNBRPriceTo.Text = "0.00";
            // 
            // txtCostPriceFrom
            // 
            this.txtCostPriceFrom.Location = new System.Drawing.Point(818, 88);
            this.txtCostPriceFrom.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtCostPriceFrom.MinimumSize = new System.Drawing.Size(65, 20);
            this.txtCostPriceFrom.Name = "txtCostPriceFrom";
            this.txtCostPriceFrom.Size = new System.Drawing.Size(65, 20);
            this.txtCostPriceFrom.TabIndex = 138;
            this.txtCostPriceFrom.Text = "0.00";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(757, 91);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 139;
            this.label1.Text = "Cost Price";
            // 
            // txtSalesPriceFrom
            // 
            this.txtSalesPriceFrom.Location = new System.Drawing.Point(818, 113);
            this.txtSalesPriceFrom.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtSalesPriceFrom.MinimumSize = new System.Drawing.Size(65, 20);
            this.txtSalesPriceFrom.Name = "txtSalesPriceFrom";
            this.txtSalesPriceFrom.Size = new System.Drawing.Size(65, 20);
            this.txtSalesPriceFrom.TabIndex = 136;
            this.txtSalesPriceFrom.Text = "0.00";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(757, 116);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 13);
            this.label3.TabIndex = 137;
            this.label3.Text = "Sales Price";
            // 
            // txtNBRPriceFrom
            // 
            this.txtNBRPriceFrom.Location = new System.Drawing.Point(818, 138);
            this.txtNBRPriceFrom.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtNBRPriceFrom.MinimumSize = new System.Drawing.Size(65, 20);
            this.txtNBRPriceFrom.Name = "txtNBRPriceFrom";
            this.txtNBRPriceFrom.Size = new System.Drawing.Size(65, 20);
            this.txtNBRPriceFrom.TabIndex = 135;
            this.txtNBRPriceFrom.Text = "0.00";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(372, 22);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(83, 13);
            this.label13.TabIndex = 116;
            this.label13.Text = "Computer Name";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(757, 141);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(57, 13);
            this.label5.TabIndex = 134;
            this.label5.Text = "NRB Price";
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSearch.Image = global::VATClient.Properties.Resources.search;
            this.btnSearch.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSearch.Location = new System.Drawing.Point(659, 25);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 28);
            this.btnSearch.TabIndex = 12;
            this.btnSearch.Text = "&Search";
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // txtComputerName
            // 
            this.txtComputerName.Location = new System.Drawing.Point(468, 18);
            this.txtComputerName.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtComputerName.MinimumSize = new System.Drawing.Size(185, 20);
            this.txtComputerName.Name = "txtComputerName";
            this.txtComputerName.Size = new System.Drawing.Size(185, 20);
            this.txtComputerName.TabIndex = 6;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 23);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(105, 13);
            this.label4.TabIndex = 103;
            this.label4.Text = "Software User Name";
            // 
            // backgroundWorkerSearch
            // 
            this.backgroundWorkerSearch.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerSearch_DoWork);
            this.backgroundWorkerSearch.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerSearch_RunWorkerCompleted);
            // 
            // LRecordCount
            // 
            this.LRecordCount.AutoSize = true;
            this.LRecordCount.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LRecordCount.Location = new System.Drawing.Point(93, 298);
            this.LRecordCount.Name = "LRecordCount";
            this.LRecordCount.Size = new System.Drawing.Size(94, 16);
            this.LRecordCount.TabIndex = 222;
            this.LRecordCount.Text = "Record Count :";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(508, 296);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(250, 25);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 224;
            this.progressBar1.Visible = false;
            // 
            // bgwUserSearch
            // 
            this.bgwUserSearch.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwUserSearch_DoWork);
            this.bgwUserSearch.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwUserSearch_RunWorkerCompleted);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnCancel.Image = global::VATClient.Properties.Resources.Referesh;
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.Location = new System.Drawing.Point(12, 292);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 28);
            this.btnCancel.TabIndex = 225;
            this.btnCancel.Text = "&Refresh";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // FormSearchUserLog
            // 
            this.AcceptButton = this.btnSearch;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(790, 323);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.LRecordCount);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.dgvLogs);
            this.MaximizeBox = false;
            this.Name = "FormSearchUserLog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Search User Log";
            this.Load += new System.EventHandler(this.FormSearchUserLog_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvLogs)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvLogs;
        private System.Windows.Forms.DataGridViewTextBoxColumn LogID;
        private System.Windows.Forms.DataGridViewTextBoxColumn SoftwareUserId;
        private System.Windows.Forms.DataGridViewTextBoxColumn SoftwareUserName;
        private System.Windows.Forms.DataGridViewTextBoxColumn SessionDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn LogInDateTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn LogOutDateTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn ComputerName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ComputerLoginUserName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ComputerIPAddress;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox cmbSoftwareUserName;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtComputerLoginUserName;
        private System.Windows.Forms.TextBox txtCostPriceTo;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtSalesPriceTo;
        private System.Windows.Forms.TextBox txtNBRPriceTo;
        private System.Windows.Forms.TextBox txtCostPriceFrom;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtSalesPriceFrom;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtNBRPriceFrom;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.TextBox txtComputerName;
        private System.Windows.Forms.Label label4;
        private System.ComponentModel.BackgroundWorker backgroundWorkerSearch;
        private System.Windows.Forms.Label LRecordCount;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.ComponentModel.BackgroundWorker bgwUserSearch;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.DateTimePicker dtpIssueToDate;
        private System.Windows.Forms.DateTimePicker dtpIssueFromDate;
        private System.Windows.Forms.Label label2;
    }
}