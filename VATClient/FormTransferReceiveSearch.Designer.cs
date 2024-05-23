namespace VATClient
{
    partial class FormTransferReceiveSearch
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.dgvReceiveHistory = new System.Windows.Forms.DataGridView();
            this.Select = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.ReceiveNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ReceiveDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Post = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SerialNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ReferenceNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TotalAmount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TransferFrom = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TransferFromNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TransferNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.txtSerialNo = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.grbTransactionHistory = new System.Windows.Forms.GroupBox();
            this.cmbReceiveForm = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnPost = new System.Windows.Forms.Button();
            this.chkSelectAll = new System.Windows.Forms.CheckBox();
            this.txtTransferNo = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtTransferFromNo = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbPost = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.dtpReceiveToDate = new System.Windows.Forms.DateTimePicker();
            this.dtpReceiveFromDate = new System.Windows.Forms.DateTimePicker();
            this.btnSearch = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.txtReceiveNo = new System.Windows.Forms.TextBox();
            this.labelReceiveNo = new System.Windows.Forms.Label();
            this.backgroundWorkerSearch = new System.ComponentModel.BackgroundWorker();
            this.btnOk = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnExport = new System.Windows.Forms.Button();
            this.cmbExport = new System.Windows.Forms.ComboBox();
            this.LRecordCount = new System.Windows.Forms.Label();
            this.bgwMultiplePost = new System.ComponentModel.BackgroundWorker();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvReceiveHistory)).BeginInit();
            this.grbTransactionHistory.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.progressBar1);
            this.groupBox1.Controls.Add(this.dgvReceiveHistory);
            this.groupBox1.Controls.Add(this.txtSerialNo);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(12, 112);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(756, 300);
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
            // dgvReceiveHistory
            // 
            this.dgvReceiveHistory.AllowUserToAddRows = false;
            this.dgvReceiveHistory.AllowUserToDeleteRows = false;
            this.dgvReceiveHistory.AllowUserToOrderColumns = true;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black;
            this.dgvReceiveHistory.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvReceiveHistory.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvReceiveHistory.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvReceiveHistory.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Select,
            this.ReceiveNo,
            this.ReceiveDate,
            this.Post,
            this.SerialNo,
            this.ReferenceNo,
            this.TotalAmount,
            this.TransferFrom,
            this.TransferFromNo,
            this.TransferNo,
            this.Id});
            this.dgvReceiveHistory.Location = new System.Drawing.Point(6, 11);
            this.dgvReceiveHistory.Name = "dgvReceiveHistory";
            this.dgvReceiveHistory.RowHeadersVisible = false;
            this.dgvReceiveHistory.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvReceiveHistory.Size = new System.Drawing.Size(744, 282);
            this.dgvReceiveHistory.TabIndex = 6;
            this.dgvReceiveHistory.DoubleClick += new System.EventHandler(this.dgvReceiveHistory_DoubleClick);
            // 
            // Select
            // 
            this.Select.HeaderText = "Select";
            this.Select.Name = "Select";
            // 
            // ReceiveNo
            // 
            this.ReceiveNo.HeaderText = "Receive No";
            this.ReceiveNo.Name = "ReceiveNo";
            this.ReceiveNo.ReadOnly = true;
            this.ReceiveNo.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // ReceiveDate
            // 
            this.ReceiveDate.HeaderText = "Receive Date";
            this.ReceiveDate.Name = "ReceiveDate";
            this.ReceiveDate.ReadOnly = true;
            // 
            // Post
            // 
            this.Post.HeaderText = "Post";
            this.Post.Name = "Post";
            this.Post.ReadOnly = true;
            // 
            // SerialNo
            // 
            this.SerialNo.HeaderText = "Serial No";
            this.SerialNo.Name = "SerialNo";
            this.SerialNo.ReadOnly = true;
            this.SerialNo.Width = 120;
            // 
            // ReferenceNo
            // 
            this.ReferenceNo.HeaderText = "Reference No";
            this.ReferenceNo.Name = "ReferenceNo";
            // 
            // TotalAmount
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.TotalAmount.DefaultCellStyle = dataGridViewCellStyle2;
            this.TotalAmount.HeaderText = "Grand Total";
            this.TotalAmount.Name = "TotalAmount";
            this.TotalAmount.ReadOnly = true;
            this.TotalAmount.Width = 120;
            // 
            // TransferFrom
            // 
            this.TransferFrom.HeaderText = "Receive From";
            this.TransferFrom.Name = "TransferFrom";
            // 
            // TransferFromNo
            // 
            this.TransferFromNo.HeaderText = "TransferFromNo";
            this.TransferFromNo.Name = "TransferFromNo";
            this.TransferFromNo.ReadOnly = true;
            // 
            // TransferNo
            // 
            this.TransferNo.HeaderText = "TransferNo";
            this.TransferNo.Name = "TransferNo";
            this.TransferNo.ReadOnly = true;
            this.TransferNo.Visible = false;
            // 
            // Id
            // 
            this.Id.HeaderText = "Id";
            this.Id.Name = "Id";
            this.Id.ReadOnly = true;
            this.Id.Visible = false;
            // 
            // txtSerialNo
            // 
            this.txtSerialNo.Location = new System.Drawing.Point(763, 79);
            this.txtSerialNo.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtSerialNo.MinimumSize = new System.Drawing.Size(185, 20);
            this.txtSerialNo.Name = "txtSerialNo";
            this.txtSerialNo.Size = new System.Drawing.Size(185, 24);
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
            this.grbTransactionHistory.Controls.Add(this.cmbReceiveForm);
            this.grbTransactionHistory.Controls.Add(this.label6);
            this.grbTransactionHistory.Controls.Add(this.btnCancel);
            this.grbTransactionHistory.Controls.Add(this.btnPost);
            this.grbTransactionHistory.Controls.Add(this.chkSelectAll);
            this.grbTransactionHistory.Controls.Add(this.txtTransferNo);
            this.grbTransactionHistory.Controls.Add(this.label4);
            this.grbTransactionHistory.Controls.Add(this.txtTransferFromNo);
            this.grbTransactionHistory.Controls.Add(this.label1);
            this.grbTransactionHistory.Controls.Add(this.cmbPost);
            this.grbTransactionHistory.Controls.Add(this.label9);
            this.grbTransactionHistory.Controls.Add(this.label11);
            this.grbTransactionHistory.Controls.Add(this.dtpReceiveToDate);
            this.grbTransactionHistory.Controls.Add(this.dtpReceiveFromDate);
            this.grbTransactionHistory.Controls.Add(this.btnSearch);
            this.grbTransactionHistory.Controls.Add(this.label3);
            this.grbTransactionHistory.Controls.Add(this.txtReceiveNo);
            this.grbTransactionHistory.Controls.Add(this.labelReceiveNo);
            this.grbTransactionHistory.Location = new System.Drawing.Point(12, 12);
            this.grbTransactionHistory.Name = "grbTransactionHistory";
            this.grbTransactionHistory.Size = new System.Drawing.Size(756, 100);
            this.grbTransactionHistory.TabIndex = 107;
            this.grbTransactionHistory.TabStop = false;
            this.grbTransactionHistory.Text = "Searching Criteria";
            this.grbTransactionHistory.Enter += new System.EventHandler(this.grbTransactionHistory_Enter);
            // 
            // cmbReceiveForm
            // 
            this.cmbReceiveForm.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbReceiveForm.FormattingEnabled = true;
            this.cmbReceiveForm.Location = new System.Drawing.Point(419, 71);
            this.cmbReceiveForm.Name = "cmbReceiveForm";
            this.cmbReceiveForm.Size = new System.Drawing.Size(150, 25);
            this.cmbReceiveForm.TabIndex = 533;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(335, 75);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(91, 17);
            this.label6.TabIndex = 534;
            this.label6.Text = "Receive Form";
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnCancel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancel.Image = global::VATClient.Properties.Resources.Referesh;
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.Location = new System.Drawing.Point(666, 67);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 28);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "&Refresh";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnPost
            // 
            this.btnPost.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnPost.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPost.Image = global::VATClient.Properties.Resources.Post;
            this.btnPost.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPost.Location = new System.Drawing.Point(580, 16);
            this.btnPost.Name = "btnPost";
            this.btnPost.Size = new System.Drawing.Size(75, 28);
            this.btnPost.TabIndex = 237;
            this.btnPost.Text = "Post";
            this.btnPost.UseVisualStyleBackColor = false;
            this.btnPost.Click += new System.EventHandler(this.btnPost_Click);
            // 
            // chkSelectAll
            // 
            this.chkSelectAll.AutoSize = true;
            this.chkSelectAll.Location = new System.Drawing.Point(6, 79);
            this.chkSelectAll.Name = "chkSelectAll";
            this.chkSelectAll.Size = new System.Drawing.Size(78, 21);
            this.chkSelectAll.TabIndex = 205;
            this.chkSelectAll.Text = "SelectAll";
            this.chkSelectAll.UseVisualStyleBackColor = true;
            this.chkSelectAll.CheckedChanged += new System.EventHandler(this.chkSelectAll_CheckedChanged);
            // 
            // txtTransferNo
            // 
            this.txtTransferNo.Location = new System.Drawing.Point(178, 74);
            this.txtTransferNo.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtTransferNo.MinimumSize = new System.Drawing.Size(150, 20);
            this.txtTransferNo.Name = "txtTransferNo";
            this.txtTransferNo.Size = new System.Drawing.Size(150, 24);
            this.txtTransferNo.TabIndex = 203;
            this.txtTransferNo.Visible = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(108, 77);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(79, 17);
            this.label4.TabIndex = 204;
            this.label4.Text = "Transfer No";
            this.label4.Visible = false;
            // 
            // txtTransferFromNo
            // 
            this.txtTransferFromNo.Location = new System.Drawing.Point(419, 20);
            this.txtTransferFromNo.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtTransferFromNo.MinimumSize = new System.Drawing.Size(150, 20);
            this.txtTransferFromNo.Name = "txtTransferFromNo";
            this.txtTransferFromNo.Size = new System.Drawing.Size(150, 24);
            this.txtTransferFromNo.TabIndex = 201;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(335, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(94, 17);
            this.label1.TabIndex = 202;
            this.label1.Text = "Transfer From";
            // 
            // cmbPost
            // 
            this.cmbPost.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPost.FormattingEnabled = true;
            this.cmbPost.Items.AddRange(new object[] {
            "Y",
            "N"});
            this.cmbPost.Location = new System.Drawing.Point(419, 46);
            this.cmbPost.Name = "cmbPost";
            this.cmbPost.Size = new System.Drawing.Size(150, 25);
            this.cmbPost.TabIndex = 4;
            this.cmbPost.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmbPost_KeyDown);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(335, 50);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(35, 17);
            this.label9.TabIndex = 200;
            this.label9.Text = "Post";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(207, 50);
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
            this.dtpReceiveToDate.Location = new System.Drawing.Point(225, 46);
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
            this.dtpReceiveFromDate.Location = new System.Drawing.Point(103, 46);
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
            this.btnSearch.Location = new System.Drawing.Point(666, 16);
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
            this.label3.Location = new System.Drawing.Point(28, 50);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(93, 17);
            this.label3.TabIndex = 3;
            this.label3.Text = "Receive Date:";
            // 
            // txtReceiveNo
            // 
            this.txtReceiveNo.Location = new System.Drawing.Point(103, 20);
            this.txtReceiveNo.MaximumSize = new System.Drawing.Size(250, 20);
            this.txtReceiveNo.MinimumSize = new System.Drawing.Size(150, 20);
            this.txtReceiveNo.Name = "txtReceiveNo";
            this.txtReceiveNo.Size = new System.Drawing.Size(225, 24);
            this.txtReceiveNo.TabIndex = 0;
            this.txtReceiveNo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtReceiveNo_KeyDown);
            // 
            // labelReceiveNo
            // 
            this.labelReceiveNo.AutoSize = true;
            this.labelReceiveNo.Location = new System.Drawing.Point(28, 25);
            this.labelReceiveNo.Name = "labelReceiveNo";
            this.labelReceiveNo.Size = new System.Drawing.Size(81, 17);
            this.labelReceiveNo.TabIndex = 0;
            this.labelReceiveNo.Text = "Receive No:";
            // 
            // btnOk
            // 
            this.btnOk.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnOk.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnOk.Image = global::VATClient.Properties.Resources.Back;
            this.btnOk.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOk.Location = new System.Drawing.Point(698, 3);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 28);
            this.btnOk.TabIndex = 8;
            this.btnOk.Text = "&Ok";
            this.btnOk.UseVisualStyleBackColor = false;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.panel1.Controls.Add(this.btnExport);
            this.panel1.Controls.Add(this.cmbExport);
            this.panel1.Controls.Add(this.LRecordCount);
            this.panel1.Controls.Add(this.btnOk);
            this.panel1.Location = new System.Drawing.Point(-1, 421);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(793, 40);
            this.panel1.TabIndex = 6;
            // 
            // btnExport
            // 
            this.btnExport.BackColor = System.Drawing.Color.White;
            this.btnExport.Image = global::VATClient.Properties.Resources.Load;
            this.btnExport.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnExport.Location = new System.Drawing.Point(433, 2);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(75, 28);
            this.btnExport.TabIndex = 236;
            this.btnExport.Text = "Export";
            this.btnExport.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnExport.UseVisualStyleBackColor = false;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // cmbExport
            // 
            this.cmbExport.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbExport.FormattingEnabled = true;
            this.cmbExport.Items.AddRange(new object[] {
            "EXCEL",
            "TEXT",
            "RECEIVE"});
            this.cmbExport.Location = new System.Drawing.Point(341, 5);
            this.cmbExport.Name = "cmbExport";
            this.cmbExport.Size = new System.Drawing.Size(79, 25);
            this.cmbExport.TabIndex = 237;
            this.cmbExport.Visible = false;
            // 
            // LRecordCount
            // 
            this.LRecordCount.AutoSize = true;
            this.LRecordCount.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LRecordCount.Location = new System.Drawing.Point(15, 8);
            this.LRecordCount.Name = "LRecordCount";
            this.LRecordCount.Size = new System.Drawing.Size(121, 21);
            this.LRecordCount.TabIndex = 223;
            this.LRecordCount.Text = "Record Count :";
            // 
            // bgwMultiplePost
            // 
            this.bgwMultiplePost.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwMultiplePost_DoWork);
            this.bgwMultiplePost.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwMultiplePost_RunWorkerCompleted);
            // 
            // FormTransferReceiveSearch
            // 
            this.AcceptButton = this.btnSearch;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(206)))), ((int)(((byte)(206)))), ((int)(((byte)(246)))));
            this.CancelButton = this.btnOk;
            this.ClientSize = new System.Drawing.Size(782, 453);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.grbTransactionHistory);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(800, 500);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(800, 500);
            this.Name = "FormTransferReceiveSearch";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Transfer Receive Search";
            this.Load += new System.EventHandler(this.FormTransferReceiveSearch_Load);
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
        private System.Windows.Forms.TextBox txtReceiveNo;
        private System.Windows.Forms.Label labelReceiveNo;
        private System.Windows.Forms.ComboBox cmbPost;
        private System.Windows.Forms.Label label9;
        private System.ComponentModel.BackgroundWorker backgroundWorkerSearch;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label LRecordCount;
        private System.Windows.Forms.TextBox txtTransferNo;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtTransferFromNo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chkSelectAll;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Select;
        private System.Windows.Forms.DataGridViewTextBoxColumn ReceiveNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn ReceiveDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn Post;
        private System.Windows.Forms.DataGridViewTextBoxColumn SerialNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn ReferenceNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn TotalAmount;
        private System.Windows.Forms.DataGridViewTextBoxColumn TransferFrom;
        private System.Windows.Forms.DataGridViewTextBoxColumn TransferFromNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn TransferNo;
        private System.Windows.Forms.Button btnPost;
        private System.ComponentModel.BackgroundWorker bgwMultiplePost;
        private System.Windows.Forms.DataGridViewTextBoxColumn Id;
        private System.Windows.Forms.ComboBox cmbReceiveForm;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.ComboBox cmbExport;
    }
}