namespace VATClient
{
    partial class FormProductTransferSearch
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.dgvProductTransferHistory = new System.Windows.Forms.DataGridView();
            this.txtSerialNo = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.grbTransactionHistory = new System.Windows.Forms.GroupBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnPost = new System.Windows.Forms.Button();
            this.chkSelectAll = new System.Windows.Forms.CheckBox();
            this.cmbPost = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.txtReceiveNo = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.dtpIssueToDate = new System.Windows.Forms.DateTimePicker();
            this.dtpIssueFromDate = new System.Windows.Forms.DateTimePicker();
            this.btnSearch = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.txtTransferCode = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbExport = new System.Windows.Forms.ComboBox();
            this.btnExport = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.LRecordCount = new System.Windows.Forms.Label();
            this.btnOk = new System.Windows.Forms.Button();
            this.backgroundWorkerSearch = new System.ComponentModel.BackgroundWorker();
            this.bgwMultiplePost = new System.ComponentModel.BackgroundWorker();
            this.Select = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.TransferCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TransferDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Post = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.IsWastage = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Comments = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProductTransferHistory)).BeginInit();
            this.grbTransactionHistory.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.progressBar1);
            this.groupBox1.Controls.Add(this.dgvProductTransferHistory);
            this.groupBox1.Controls.Add(this.txtSerialNo);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(12, 119);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(627, 299);
            this.groupBox1.TabIndex = 108;
            this.groupBox1.TabStop = false;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(12, 71);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(575, 25);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 109;
            this.progressBar1.Visible = false;
            // 
            // dgvProductTransferHistory
            // 
            this.dgvProductTransferHistory.AllowUserToAddRows = false;
            this.dgvProductTransferHistory.AllowUserToDeleteRows = false;
            this.dgvProductTransferHistory.AllowUserToOrderColumns = true;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black;
            this.dgvProductTransferHistory.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvProductTransferHistory.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvProductTransferHistory.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvProductTransferHistory.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Select,
            this.TransferCode,
            this.TransferDate,
            this.Post,
            this.IsWastage,
            this.Comments,
            this.Id});
            this.dgvProductTransferHistory.Location = new System.Drawing.Point(6, 12);
            this.dgvProductTransferHistory.Name = "dgvProductTransferHistory";
            this.dgvProductTransferHistory.RowHeadersVisible = false;
            this.dgvProductTransferHistory.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvProductTransferHistory.Size = new System.Drawing.Size(619, 284);
            this.dgvProductTransferHistory.TabIndex = 6;
            this.dgvProductTransferHistory.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvIssueHistory_CellContentClick);
            this.dgvProductTransferHistory.DoubleClick += new System.EventHandler(this.dgvIssueHistory_DoubleClick);
            // 
            // txtSerialNo
            // 
            this.txtSerialNo.Location = new System.Drawing.Point(763, 79);
            this.txtSerialNo.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtSerialNo.MinimumSize = new System.Drawing.Size(185, 20);
            this.txtSerialNo.Name = "txtSerialNo";
            this.txtSerialNo.Size = new System.Drawing.Size(185, 21);
            this.txtSerialNo.TabIndex = 1;
            this.txtSerialNo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSerialNo_KeyDown);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(564, 98);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Serial No:";
            // 
            // grbTransactionHistory
            // 
            this.grbTransactionHistory.Controls.Add(this.btnCancel);
            this.grbTransactionHistory.Controls.Add(this.btnPost);
            this.grbTransactionHistory.Controls.Add(this.chkSelectAll);
            this.grbTransactionHistory.Controls.Add(this.cmbPost);
            this.grbTransactionHistory.Controls.Add(this.label9);
            this.grbTransactionHistory.Controls.Add(this.textBox1);
            this.grbTransactionHistory.Controls.Add(this.txtReceiveNo);
            this.grbTransactionHistory.Controls.Add(this.label11);
            this.grbTransactionHistory.Controls.Add(this.dtpIssueToDate);
            this.grbTransactionHistory.Controls.Add(this.dtpIssueFromDate);
            this.grbTransactionHistory.Controls.Add(this.btnSearch);
            this.grbTransactionHistory.Controls.Add(this.label3);
            this.grbTransactionHistory.Controls.Add(this.txtTransferCode);
            this.grbTransactionHistory.Controls.Add(this.label1);
            this.grbTransactionHistory.Location = new System.Drawing.Point(12, 5);
            this.grbTransactionHistory.Name = "grbTransactionHistory";
            this.grbTransactionHistory.Size = new System.Drawing.Size(627, 117);
            this.grbTransactionHistory.TabIndex = 107;
            this.grbTransactionHistory.TabStop = false;
            this.grbTransactionHistory.Text = "Searching Criteria";
            this.grbTransactionHistory.Enter += new System.EventHandler(this.grbTransactionHistory_Enter);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnCancel.Image = global::VATClient.Properties.Resources.Referesh;
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.Location = new System.Drawing.Point(542, 42);
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
            this.btnPost.Image = global::VATClient.Properties.Resources.Post;
            this.btnPost.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPost.Location = new System.Drawing.Point(542, 72);
            this.btnPost.Name = "btnPost";
            this.btnPost.Size = new System.Drawing.Size(75, 28);
            this.btnPost.TabIndex = 236;
            this.btnPost.Text = "Post";
            this.btnPost.UseVisualStyleBackColor = false;
            this.btnPost.Click += new System.EventHandler(this.btnPost_Click);
            // 
            // chkSelectAll
            // 
            this.chkSelectAll.AutoSize = true;
            this.chkSelectAll.Location = new System.Drawing.Point(10, 100);
            this.chkSelectAll.Name = "chkSelectAll";
            this.chkSelectAll.Size = new System.Drawing.Size(66, 17);
            this.chkSelectAll.TabIndex = 118;
            this.chkSelectAll.Text = "SelectAll";
            this.chkSelectAll.UseVisualStyleBackColor = true;
            this.chkSelectAll.CheckedChanged += new System.EventHandler(this.chkSelectAll_CheckedChanged);
            // 
            // cmbPost
            // 
            this.cmbPost.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPost.FormattingEnabled = true;
            this.cmbPost.Items.AddRange(new object[] {
            "Y",
            "N"});
            this.cmbPost.Location = new System.Drawing.Point(387, 13);
            this.cmbPost.Name = "cmbPost";
            this.cmbPost.Size = new System.Drawing.Size(117, 21);
            this.cmbPost.TabIndex = 4;
            this.cmbPost.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmbPost_KeyDown);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(348, 17);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(28, 13);
            this.label9.TabIndex = 200;
            this.label9.Text = "Post";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(509, 48);
            this.textBox1.MaximumSize = new System.Drawing.Size(200, 20);
            this.textBox1.MinimumSize = new System.Drawing.Size(20, 20);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(20, 21);
            this.textBox1.TabIndex = 3;
            this.textBox1.Visible = false;
            this.textBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtReceiveNo_KeyDown);
            // 
            // txtReceiveNo
            // 
            this.txtReceiveNo.Location = new System.Drawing.Point(509, 74);
            this.txtReceiveNo.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtReceiveNo.MinimumSize = new System.Drawing.Size(20, 20);
            this.txtReceiveNo.Name = "txtReceiveNo";
            this.txtReceiveNo.Size = new System.Drawing.Size(20, 21);
            this.txtReceiveNo.TabIndex = 3;
            this.txtReceiveNo.Visible = false;
            this.txtReceiveNo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtReceiveNo_KeyDown);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(207, 43);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(17, 13);
            this.label11.TabIndex = 112;
            this.label11.Text = "to";
            // 
            // dtpIssueToDate
            // 
            this.dtpIssueToDate.Checked = false;
            this.dtpIssueToDate.CustomFormat = "d/MMM/yyyy";
            this.dtpIssueToDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpIssueToDate.Location = new System.Drawing.Point(229, 39);
            this.dtpIssueToDate.Name = "dtpIssueToDate";
            this.dtpIssueToDate.ShowCheckBox = true;
            this.dtpIssueToDate.Size = new System.Drawing.Size(103, 21);
            this.dtpIssueToDate.TabIndex = 2;
            this.dtpIssueToDate.ValueChanged += new System.EventHandler(this.dtpIssueToDate_ValueChanged);
            this.dtpIssueToDate.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dtpIssueToDate_KeyDown);
            // 
            // dtpIssueFromDate
            // 
            this.dtpIssueFromDate.Checked = false;
            this.dtpIssueFromDate.CustomFormat = "d/MMM/yyyy";
            this.dtpIssueFromDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpIssueFromDate.Location = new System.Drawing.Point(107, 39);
            this.dtpIssueFromDate.Name = "dtpIssueFromDate";
            this.dtpIssueFromDate.ShowCheckBox = true;
            this.dtpIssueFromDate.Size = new System.Drawing.Size(102, 21);
            this.dtpIssueFromDate.TabIndex = 1;
            this.dtpIssueFromDate.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dtpIssueFromDate_KeyDown);
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSearch.Image = global::VATClient.Properties.Resources.search;
            this.btnSearch.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSearch.Location = new System.Drawing.Point(542, 12);
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
            this.label3.Location = new System.Drawing.Point(20, 43);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(78, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Transfer Date:";
            // 
            // txtTransferCode
            // 
            this.txtTransferCode.Location = new System.Drawing.Point(107, 14);
            this.txtTransferCode.MaximumSize = new System.Drawing.Size(225, 20);
            this.txtTransferCode.MinimumSize = new System.Drawing.Size(185, 20);
            this.txtTransferCode.Name = "txtTransferCode";
            this.txtTransferCode.Size = new System.Drawing.Size(225, 21);
            this.txtTransferCode.TabIndex = 0;
            this.txtTransferCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtIssueNo_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Transfer Code:";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // cmbExport
            // 
            this.cmbExport.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbExport.FormattingEnabled = true;
            this.cmbExport.Items.AddRange(new object[] {
            "EXCEL",
            "TEXT",
            "RECEIVE"});
            this.cmbExport.Location = new System.Drawing.Point(237, 10);
            this.cmbExport.Name = "cmbExport";
            this.cmbExport.Size = new System.Drawing.Size(79, 21);
            this.cmbExport.TabIndex = 235;
            // 
            // btnExport
            // 
            this.btnExport.BackColor = System.Drawing.Color.White;
            this.btnExport.Image = global::VATClient.Properties.Resources.Load;
            this.btnExport.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnExport.Location = new System.Drawing.Point(481, 6);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(75, 28);
            this.btnExport.TabIndex = 218;
            this.btnExport.Text = "Export";
            this.btnExport.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnExport.UseVisualStyleBackColor = false;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.panel1.Controls.Add(this.LRecordCount);
            this.panel1.Controls.Add(this.btnOk);
            this.panel1.Controls.Add(this.btnExport);
            this.panel1.Controls.Add(this.cmbExport);
            this.panel1.Location = new System.Drawing.Point(-1, 421);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(638, 40);
            this.panel1.TabIndex = 6;
            // 
            // LRecordCount
            // 
            this.LRecordCount.AutoSize = true;
            this.LRecordCount.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LRecordCount.Location = new System.Drawing.Point(23, 13);
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
            this.btnOk.Location = new System.Drawing.Point(560, 7);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 28);
            this.btnOk.TabIndex = 8;
            this.btnOk.Text = "&Ok";
            this.btnOk.UseVisualStyleBackColor = false;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // bgwMultiplePost
            // 
            this.bgwMultiplePost.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwMultiplePost_DoWork);
            this.bgwMultiplePost.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwMultiplePost_RunWorkerCompleted);
            // 
            // Select
            // 
            this.Select.HeaderText = "Select";
            this.Select.Name = "Select";
            // 
            // TransferCode
            // 
            this.TransferCode.HeaderText = "Transfer Code";
            this.TransferCode.Name = "TransferCode";
            this.TransferCode.ReadOnly = true;
            this.TransferCode.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // TransferDate
            // 
            this.TransferDate.HeaderText = "Transfer Date";
            this.TransferDate.Name = "TransferDate";
            this.TransferDate.ReadOnly = true;
            this.TransferDate.Width = 150;
            // 
            // Post
            // 
            this.Post.HeaderText = "Post";
            this.Post.Name = "Post";
            this.Post.ReadOnly = true;
            // 
            // IsWastage
            // 
            this.IsWastage.HeaderText = "Is Wastage";
            this.IsWastage.Name = "IsWastage";
            this.IsWastage.ReadOnly = true;
            // 
            // Comments
            // 
            this.Comments.HeaderText = "Comments";
            this.Comments.Name = "Comments";
            this.Comments.ReadOnly = true;
            this.Comments.Visible = false;
            this.Comments.Width = 120;
            // 
            // Id
            // 
            this.Id.HeaderText = "Id";
            this.Id.Name = "Id";
            this.Id.ReadOnly = true;
            this.Id.Visible = false;
            // 
            // FormProductTransferSearch
            // 
            this.AcceptButton = this.btnSearch;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(206)))), ((int)(((byte)(206)))), ((int)(((byte)(246)))));
            this.CancelButton = this.btnOk;
            this.ClientSize = new System.Drawing.Size(641, 461);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.grbTransactionHistory);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(800, 500);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(500, 500);
            this.Name = "FormProductTransferSearch";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Product Transfer  Search";
            this.Load += new System.EventHandler(this.FormTransferIssueSearch_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProductTransferHistory)).EndInit();
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
        private System.Windows.Forms.DataGridView dgvProductTransferHistory;
        private System.Windows.Forms.GroupBox grbTransactionHistory;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.DateTimePicker dtpIssueToDate;
        private System.Windows.Forms.DateTimePicker dtpIssueFromDate;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.TextBox txtSerialNo;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtTransferCode;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtReceiveNo;
        private System.Windows.Forms.ComboBox cmbPost;
        private System.Windows.Forms.Label label9;
        private System.ComponentModel.BackgroundWorker backgroundWorkerSearch;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label LRecordCount;
        private System.Windows.Forms.CheckBox chkSelectAll;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.ComboBox cmbExport;
        private System.Windows.Forms.Button btnPost;
        private System.ComponentModel.BackgroundWorker bgwMultiplePost;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Select;
        private System.Windows.Forms.DataGridViewTextBoxColumn TransferCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn TransferDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn Post;
        private System.Windows.Forms.DataGridViewTextBoxColumn IsWastage;
        private System.Windows.Forms.DataGridViewTextBoxColumn Comments;
        private System.Windows.Forms.DataGridViewTextBoxColumn Id;
    }
}