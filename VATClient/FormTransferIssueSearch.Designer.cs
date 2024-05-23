namespace VATClient
{
    partial class FormTransferIssueSearch
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.dgvIssueHistory = new System.Windows.Forms.DataGridView();
            this.Select = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.IssueNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.IssueDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Post = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SerialNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ReferenceNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TotalAmount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Comments = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TransferTo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.VehicleNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.txtSerialNo = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.grbTransactionHistory = new System.Windows.Forms.GroupBox();
            this.txtVehicleNo = new System.Windows.Forms.TextBox();
            this.l5 = new System.Windows.Forms.Label();
            this.cmbTransferTo = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.txtReferenceNo = new System.Windows.Forms.TextBox();
            this.labelReferenceNo = new System.Windows.Forms.Label();
            this.btnPost = new System.Windows.Forms.Button();
            this.chkSelectAll = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cmbPost = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.txtReceiveNo = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.dtpIssueToDate = new System.Windows.Forms.DateTimePicker();
            this.dtpIssueFromDate = new System.Windows.Forms.DateTimePicker();
            this.btnSearch = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.txtIssueNo = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbExport = new System.Windows.Forms.ComboBox();
            this.btnExport = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.LRecordCount = new System.Windows.Forms.Label();
            this.btnOk = new System.Windows.Forms.Button();
            this.backgroundWorkerSearch = new System.ComponentModel.BackgroundWorker();
            this.bgwMultiplePost = new System.ComponentModel.BackgroundWorker();
            this.cmbBranch = new System.Windows.Forms.ComboBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvIssueHistory)).BeginInit();
            this.grbTransactionHistory.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.progressBar1);
            this.groupBox1.Controls.Add(this.dgvIssueHistory);
            this.groupBox1.Controls.Add(this.txtSerialNo);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(12, 119);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(756, 299);
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
            // dgvIssueHistory
            // 
            this.dgvIssueHistory.AllowUserToAddRows = false;
            this.dgvIssueHistory.AllowUserToDeleteRows = false;
            this.dgvIssueHistory.AllowUserToOrderColumns = true;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.Black;
            this.dgvIssueHistory.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvIssueHistory.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvIssueHistory.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvIssueHistory.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Select,
            this.IssueNo,
            this.IssueDate,
            this.Post,
            this.SerialNo,
            this.ReferenceNo,
            this.TotalAmount,
            this.Comments,
            this.TransferTo,
            this.VehicleNo,
            this.Id});
            this.dgvIssueHistory.Location = new System.Drawing.Point(6, 12);
            this.dgvIssueHistory.Name = "dgvIssueHistory";
            this.dgvIssueHistory.RowHeadersVisible = false;
            this.dgvIssueHistory.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvIssueHistory.Size = new System.Drawing.Size(744, 284);
            this.dgvIssueHistory.TabIndex = 6;
            this.dgvIssueHistory.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvIssueHistory_CellContentClick);
            this.dgvIssueHistory.DoubleClick += new System.EventHandler(this.dgvIssueHistory_DoubleClick);
            // 
            // Select
            // 
            this.Select.HeaderText = "Select";
            this.Select.Name = "Select";
            // 
            // IssueNo
            // 
            this.IssueNo.HeaderText = "Issue No";
            this.IssueNo.Name = "IssueNo";
            this.IssueNo.ReadOnly = true;
            this.IssueNo.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // IssueDate
            // 
            this.IssueDate.HeaderText = "Issue Date";
            this.IssueDate.Name = "IssueDate";
            this.IssueDate.ReadOnly = true;
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
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.TotalAmount.DefaultCellStyle = dataGridViewCellStyle4;
            this.TotalAmount.HeaderText = "Grand Total";
            this.TotalAmount.Name = "TotalAmount";
            this.TotalAmount.ReadOnly = true;
            this.TotalAmount.Width = 120;
            // 
            // Comments
            // 
            this.Comments.HeaderText = "Comments";
            this.Comments.Name = "Comments";
            this.Comments.ReadOnly = true;
            this.Comments.Visible = false;
            this.Comments.Width = 120;
            // 
            // TransferTo
            // 
            this.TransferTo.HeaderText = "Transfer To";
            this.TransferTo.Name = "TransferTo";
            // 
            // VehicleNo
            // 
            this.VehicleNo.HeaderText = "Vehicle No";
            this.VehicleNo.Name = "VehicleNo";
            this.VehicleNo.ReadOnly = true;
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
            this.grbTransactionHistory.Controls.Add(this.cmbBranch);
            this.grbTransactionHistory.Controls.Add(this.txtVehicleNo);
            this.grbTransactionHistory.Controls.Add(this.l5);
            this.grbTransactionHistory.Controls.Add(this.cmbTransferTo);
            this.grbTransactionHistory.Controls.Add(this.label6);
            this.grbTransactionHistory.Controls.Add(this.btnCancel);
            this.grbTransactionHistory.Controls.Add(this.txtReferenceNo);
            this.grbTransactionHistory.Controls.Add(this.labelReferenceNo);
            this.grbTransactionHistory.Controls.Add(this.btnPost);
            this.grbTransactionHistory.Controls.Add(this.chkSelectAll);
            this.grbTransactionHistory.Controls.Add(this.label5);
            this.grbTransactionHistory.Controls.Add(this.cmbPost);
            this.grbTransactionHistory.Controls.Add(this.label9);
            this.grbTransactionHistory.Controls.Add(this.textBox1);
            this.grbTransactionHistory.Controls.Add(this.txtReceiveNo);
            this.grbTransactionHistory.Controls.Add(this.label7);
            this.grbTransactionHistory.Controls.Add(this.label11);
            this.grbTransactionHistory.Controls.Add(this.dtpIssueToDate);
            this.grbTransactionHistory.Controls.Add(this.dtpIssueFromDate);
            this.grbTransactionHistory.Controls.Add(this.btnSearch);
            this.grbTransactionHistory.Controls.Add(this.label3);
            this.grbTransactionHistory.Controls.Add(this.txtIssueNo);
            this.grbTransactionHistory.Controls.Add(this.label1);
            this.grbTransactionHistory.Location = new System.Drawing.Point(12, 5);
            this.grbTransactionHistory.Name = "grbTransactionHistory";
            this.grbTransactionHistory.Size = new System.Drawing.Size(756, 117);
            this.grbTransactionHistory.TabIndex = 107;
            this.grbTransactionHistory.TabStop = false;
            this.grbTransactionHistory.Text = "Searching Criteria";
            this.grbTransactionHistory.Enter += new System.EventHandler(this.grbTransactionHistory_Enter);
            // 
            // txtVehicleNo
            // 
            this.txtVehicleNo.Location = new System.Drawing.Point(401, 90);
            this.txtVehicleNo.MaximumSize = new System.Drawing.Size(150, 21);
            this.txtVehicleNo.MinimumSize = new System.Drawing.Size(150, 21);
            this.txtVehicleNo.Name = "txtVehicleNo";
            this.txtVehicleNo.Size = new System.Drawing.Size(150, 24);
            this.txtVehicleNo.TabIndex = 534;
            this.txtVehicleNo.TabStop = false;
            this.txtVehicleNo.TextChanged += new System.EventHandler(this.txtVehicleNo_TextChanged);
            // 
            // l5
            // 
            this.l5.AutoSize = true;
            this.l5.Location = new System.Drawing.Point(334, 94);
            this.l5.Name = "l5";
            this.l5.Size = new System.Drawing.Size(70, 17);
            this.l5.TabIndex = 533;
            this.l5.Text = "Vehicle No";
            // 
            // cmbTransferTo
            // 
            this.cmbTransferTo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTransferTo.FormattingEnabled = true;
            this.cmbTransferTo.Location = new System.Drawing.Point(401, 39);
            this.cmbTransferTo.Name = "cmbTransferTo";
            this.cmbTransferTo.Size = new System.Drawing.Size(117, 25);
            this.cmbTransferTo.TabIndex = 531;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(334, 43);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(78, 17);
            this.label6.TabIndex = 532;
            this.label6.Text = "Transfer To";
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnCancel.Image = global::VATClient.Properties.Resources.Referesh;
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.Location = new System.Drawing.Point(621, 42);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 28);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "&Refresh";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // txtReferenceNo
            // 
            this.txtReferenceNo.BackColor = System.Drawing.SystemColors.Window;
            this.txtReferenceNo.Location = new System.Drawing.Point(402, 14);
            this.txtReferenceNo.MinimumSize = new System.Drawing.Size(80, 20);
            this.txtReferenceNo.Name = "txtReferenceNo";
            this.txtReferenceNo.Size = new System.Drawing.Size(117, 24);
            this.txtReferenceNo.TabIndex = 237;
            // 
            // labelReferenceNo
            // 
            this.labelReferenceNo.AutoSize = true;
            this.labelReferenceNo.Location = new System.Drawing.Point(334, 18);
            this.labelReferenceNo.Name = "labelReferenceNo";
            this.labelReferenceNo.Size = new System.Drawing.Size(42, 17);
            this.labelReferenceNo.TabIndex = 238;
            this.labelReferenceNo.Text = "Ref #";
            // 
            // btnPost
            // 
            this.btnPost.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnPost.Image = global::VATClient.Properties.Resources.Post;
            this.btnPost.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPost.Location = new System.Drawing.Point(621, 72);
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
            this.chkSelectAll.Size = new System.Drawing.Size(78, 21);
            this.chkSelectAll.TabIndex = 118;
            this.chkSelectAll.Text = "SelectAll";
            this.chkSelectAll.UseVisualStyleBackColor = true;
            this.chkSelectAll.CheckedChanged += new System.EventHandler(this.chkSelectAll_CheckedChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(28, 68);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(51, 17);
            this.label5.TabIndex = 217;
            this.label5.Text = "Branch";
            // 
            // cmbPost
            // 
            this.cmbPost.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPost.FormattingEnabled = true;
            this.cmbPost.Items.AddRange(new object[] {
            "Y",
            "N"});
            this.cmbPost.Location = new System.Drawing.Point(401, 64);
            this.cmbPost.Name = "cmbPost";
            this.cmbPost.Size = new System.Drawing.Size(117, 25);
            this.cmbPost.TabIndex = 4;
            this.cmbPost.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmbPost_KeyDown);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(334, 68);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(35, 17);
            this.label9.TabIndex = 200;
            this.label9.Text = "Post";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(730, 14);
            this.textBox1.MaximumSize = new System.Drawing.Size(200, 20);
            this.textBox1.MinimumSize = new System.Drawing.Size(20, 20);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(20, 24);
            this.textBox1.TabIndex = 3;
            this.textBox1.Visible = false;
            this.textBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtReceiveNo_KeyDown);
            // 
            // txtReceiveNo
            // 
            this.txtReceiveNo.Location = new System.Drawing.Point(730, 40);
            this.txtReceiveNo.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtReceiveNo.MinimumSize = new System.Drawing.Size(20, 20);
            this.txtReceiveNo.Name = "txtReceiveNo";
            this.txtReceiveNo.Size = new System.Drawing.Size(20, 24);
            this.txtReceiveNo.TabIndex = 3;
            this.txtReceiveNo.Visible = false;
            this.txtReceiveNo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtReceiveNo_KeyDown);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(526, 12);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(81, 17);
            this.label7.TabIndex = 113;
            this.label7.Text = "Receive No:";
            this.label7.Visible = false;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(207, 43);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(21, 17);
            this.label11.TabIndex = 112;
            this.label11.Text = "to";
            // 
            // dtpIssueToDate
            // 
            this.dtpIssueToDate.Checked = false;
            this.dtpIssueToDate.CustomFormat = "d/MMM/yyyy";
            this.dtpIssueToDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpIssueToDate.Location = new System.Drawing.Point(225, 39);
            this.dtpIssueToDate.Name = "dtpIssueToDate";
            this.dtpIssueToDate.ShowCheckBox = true;
            this.dtpIssueToDate.Size = new System.Drawing.Size(103, 24);
            this.dtpIssueToDate.TabIndex = 2;
            this.dtpIssueToDate.ValueChanged += new System.EventHandler(this.dtpIssueToDate_ValueChanged);
            this.dtpIssueToDate.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dtpIssueToDate_KeyDown);
            // 
            // dtpIssueFromDate
            // 
            this.dtpIssueFromDate.Checked = false;
            this.dtpIssueFromDate.CustomFormat = "d/MMM/yyyy";
            this.dtpIssueFromDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpIssueFromDate.Location = new System.Drawing.Point(103, 39);
            this.dtpIssueFromDate.Name = "dtpIssueFromDate";
            this.dtpIssueFromDate.ShowCheckBox = true;
            this.dtpIssueFromDate.Size = new System.Drawing.Size(102, 24);
            this.dtpIssueFromDate.TabIndex = 1;
            this.dtpIssueFromDate.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dtpIssueFromDate_KeyDown);
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSearch.Image = global::VATClient.Properties.Resources.search;
            this.btnSearch.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSearch.Location = new System.Drawing.Point(621, 12);
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
            this.label3.Location = new System.Drawing.Point(28, 43);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(77, 17);
            this.label3.TabIndex = 3;
            this.label3.Text = "Issue Date:";
            // 
            // txtIssueNo
            // 
            this.txtIssueNo.Location = new System.Drawing.Point(103, 14);
            this.txtIssueNo.MaximumSize = new System.Drawing.Size(225, 20);
            this.txtIssueNo.MinimumSize = new System.Drawing.Size(185, 20);
            this.txtIssueNo.Name = "txtIssueNo";
            this.txtIssueNo.Size = new System.Drawing.Size(225, 24);
            this.txtIssueNo.TabIndex = 0;
            this.txtIssueNo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtIssueNo_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(28, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Issue No:";
            // 
            // cmbExport
            // 
            this.cmbExport.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbExport.FormattingEnabled = true;
            this.cmbExport.Items.AddRange(new object[] {
            "EXCEL",
            "TEXT",
            "RECEIVE"});
            this.cmbExport.Location = new System.Drawing.Point(277, 10);
            this.cmbExport.Name = "cmbExport";
            this.cmbExport.Size = new System.Drawing.Size(79, 25);
            this.cmbExport.TabIndex = 235;
            // 
            // btnExport
            // 
            this.btnExport.BackColor = System.Drawing.Color.White;
            this.btnExport.Image = global::VATClient.Properties.Resources.Load;
            this.btnExport.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnExport.Location = new System.Drawing.Point(369, 6);
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
            this.panel1.Size = new System.Drawing.Size(793, 40);
            this.panel1.TabIndex = 6;
            // 
            // LRecordCount
            // 
            this.LRecordCount.AutoSize = true;
            this.LRecordCount.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LRecordCount.Location = new System.Drawing.Point(23, 13);
            this.LRecordCount.Name = "LRecordCount";
            this.LRecordCount.Size = new System.Drawing.Size(121, 21);
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
            // bgwMultiplePost
            // 
            this.bgwMultiplePost.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwMultiplePost_DoWork);
            this.bgwMultiplePost.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwMultiplePost_RunWorkerCompleted);
            // 
            // cmbBranch
            // 
            this.cmbBranch.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBranch.FormattingEnabled = true;
            this.cmbBranch.Location = new System.Drawing.Point(103, 65);
            this.cmbBranch.Name = "cmbBranch";
            this.cmbBranch.Size = new System.Drawing.Size(225, 25);
            this.cmbBranch.TabIndex = 535;
            // 
            // FormTransferIssueSearch
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
            this.Name = "FormTransferIssueSearch";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Transfer Issue Search";
            this.Load += new System.EventHandler(this.FormTransferIssueSearch_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvIssueHistory)).EndInit();
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
        private System.Windows.Forms.DataGridView dgvIssueHistory;
        private System.Windows.Forms.GroupBox grbTransactionHistory;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.DateTimePicker dtpIssueToDate;
        private System.Windows.Forms.DateTimePicker dtpIssueFromDate;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.TextBox txtSerialNo;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtIssueNo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtReceiveNo;
        private System.Windows.Forms.ComboBox cmbPost;
        private System.Windows.Forms.Label label9;
        private System.ComponentModel.BackgroundWorker backgroundWorkerSearch;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label LRecordCount;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox chkSelectAll;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.ComboBox cmbExport;
        private System.Windows.Forms.Button btnPost;
        private System.ComponentModel.BackgroundWorker bgwMultiplePost;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Select;
        private System.Windows.Forms.DataGridViewTextBoxColumn IssueNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn IssueDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn Post;
        private System.Windows.Forms.DataGridViewTextBoxColumn SerialNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn ReferenceNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn TotalAmount;
        private System.Windows.Forms.DataGridViewTextBoxColumn Comments;
        private System.Windows.Forms.DataGridViewTextBoxColumn TransferTo;
        private System.Windows.Forms.DataGridViewTextBoxColumn VehicleNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn Id;
        private System.Windows.Forms.TextBox txtReferenceNo;
        private System.Windows.Forms.Label labelReferenceNo;
        private System.Windows.Forms.ComboBox cmbTransferTo;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtVehicleNo;
        public System.Windows.Forms.Label l5;
        private System.Windows.Forms.ComboBox cmbBranch;
    }
}