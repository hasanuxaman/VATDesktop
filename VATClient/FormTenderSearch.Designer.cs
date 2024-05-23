namespace VATClient
{
    partial class FormTenderSearch
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
            this.label1 = new System.Windows.Forms.Label();
            this.txtTenderId = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtRefNo = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtCustomerName = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cmbBranch = new System.Windows.Forms.ComboBox();
            this.label15 = new System.Windows.Forms.Label();
            this.chkCustCode = new System.Windows.Forms.CheckBox();
            this.cmbCustGrp = new System.Windows.Forms.ComboBox();
            this.cmbCustomerName = new System.Windows.Forms.ComboBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.dgvTenderSearch = new System.Windows.Forms.DataGridView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.LRecordCount = new System.Windows.Forms.Label();
            this.btnOk = new System.Windows.Forms.Button();
            this.backgroundWorkerSearch = new System.ComponentModel.BackgroundWorker();
            this.backgroundWorkerSearchCustomer = new System.ComponentModel.BackgroundWorker();
            this.TenderId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RefNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CustomerGroupName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.GroupId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CustomerId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CustName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TenderDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DeleveryDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Comments = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BranchId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTenderSearch)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Tender Id";
            // 
            // txtTenderId
            // 
            this.txtTenderId.Location = new System.Drawing.Point(88, 16);
            this.txtTenderId.Name = "txtTenderId";
            this.txtTenderId.Size = new System.Drawing.Size(174, 20);
            this.txtTenderId.TabIndex = 0;
            this.txtTenderId.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtTenderId_KeyDown);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(270, 19);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Ref No";
            // 
            // txtRefNo
            // 
            this.txtRefNo.Location = new System.Drawing.Point(317, 16);
            this.txtRefNo.Name = "txtRefNo";
            this.txtRefNo.Size = new System.Drawing.Size(124, 20);
            this.txtRefNo.TabIndex = 1;
            this.txtRefNo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtRefNo_KeyDown);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 70);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(51, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Customer";
            // 
            // txtCustomerName
            // 
            this.txtCustomerName.Location = new System.Drawing.Point(426, 62);
            this.txtCustomerName.Name = "txtCustomerName";
            this.txtCustomerName.Size = new System.Drawing.Size(46, 20);
            this.txtCustomerName.TabIndex = 5;
            this.txtCustomerName.Visible = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cmbBranch);
            this.groupBox1.Controls.Add(this.label15);
            this.groupBox1.Controls.Add(this.chkCustCode);
            this.groupBox1.Controls.Add(this.cmbCustGrp);
            this.groupBox1.Controls.Add(this.cmbCustomerName);
            this.groupBox1.Controls.Add(this.btnSearch);
            this.groupBox1.Controls.Add(this.txtTenderId);
            this.groupBox1.Controls.Add(this.txtCustomerName);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtRefNo);
            this.groupBox1.Location = new System.Drawing.Point(-1, -2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(472, 93);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Basic";
            // 
            // cmbBranch
            // 
            this.cmbBranch.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBranch.FormattingEnabled = true;
            this.cmbBranch.Items.AddRange(new object[] {
            "Y",
            "N"});
            this.cmbBranch.Location = new System.Drawing.Point(317, 39);
            this.cmbBranch.Name = "cmbBranch";
            this.cmbBranch.Size = new System.Drawing.Size(100, 21);
            this.cmbBranch.TabIndex = 227;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(268, 43);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(41, 13);
            this.label15.TabIndex = 226;
            this.label15.Text = "Branch";
            // 
            // chkCustCode
            // 
            this.chkCustCode.AutoSize = true;
            this.chkCustCode.Location = new System.Drawing.Point(273, 71);
            this.chkCustCode.Name = "chkCustCode";
            this.chkCustCode.Size = new System.Drawing.Size(54, 17);
            this.chkCustCode.TabIndex = 3;
            this.chkCustCode.TabStop = false;
            this.chkCustCode.Text = "Name";
            this.chkCustCode.UseVisualStyleBackColor = true;
            this.chkCustCode.CheckedChanged += new System.EventHandler(this.chkCustCode_CheckedChanged);
            // 
            // cmbCustGrp
            // 
            this.cmbCustGrp.FormattingEnabled = true;
            this.cmbCustGrp.Location = new System.Drawing.Point(88, 42);
            this.cmbCustGrp.Name = "cmbCustGrp";
            this.cmbCustGrp.Size = new System.Drawing.Size(174, 21);
            this.cmbCustGrp.Sorted = true;
            this.cmbCustGrp.TabIndex = 2;
            this.cmbCustGrp.SelectedIndexChanged += new System.EventHandler(this.cmbCustGrp_SelectedIndexChanged);
            this.cmbCustGrp.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmbCustomerName_KeyDown);
            this.cmbCustGrp.Leave += new System.EventHandler(this.cmbCustGrp_Leave);
            // 
            // cmbCustomerName
            // 
            this.cmbCustomerName.FormattingEnabled = true;
            this.cmbCustomerName.Location = new System.Drawing.Point(88, 66);
            this.cmbCustomerName.Name = "cmbCustomerName";
            this.cmbCustomerName.Size = new System.Drawing.Size(174, 21);
            this.cmbCustomerName.Sorted = true;
            this.cmbCustomerName.TabIndex = 2;
            this.cmbCustomerName.SelectedIndexChanged += new System.EventHandler(this.cmbCustomerName_SelectedIndexChanged);
            this.cmbCustomerName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmbCustomerName_KeyDown);
            this.cmbCustomerName.Leave += new System.EventHandler(this.cmbCustomerName_Leave);
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSearch.Image = global::VATClient.Properties.Resources.search;
            this.btnSearch.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSearch.Location = new System.Drawing.Point(362, 62);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 28);
            this.btnSearch.TabIndex = 4;
            this.btnSearch.Text = "&Search";
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(4, 46);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(71, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Customer Grp";
            // 
            // btnRefresh
            // 
            this.btnRefresh.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnRefresh.Image = global::VATClient.Properties.Resources.Referesh;
            this.btnRefresh.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnRefresh.Location = new System.Drawing.Point(6, 6);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(75, 28);
            this.btnRefresh.TabIndex = 6;
            this.btnRefresh.Text = "&Refresh";
            this.btnRefresh.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnRefresh.UseVisualStyleBackColor = false;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.progressBar1);
            this.groupBox2.Controls.Add(this.dgvTenderSearch);
            this.groupBox2.Location = new System.Drawing.Point(-1, 89);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(470, 183);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Search Result";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(50, 81);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(290, 24);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 6;
            this.progressBar1.Visible = false;
            // 
            // dgvTenderSearch
            // 
            this.dgvTenderSearch.AllowUserToAddRows = false;
            this.dgvTenderSearch.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black;
            this.dgvTenderSearch.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvTenderSearch.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTenderSearch.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.TenderId,
            this.RefNo,
            this.CustomerGroupName,
            this.GroupId,
            this.CustomerId,
            this.CustName,
            this.TenderDate,
            this.DeleveryDate,
            this.Comments,
            this.BranchId});
            this.dgvTenderSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvTenderSearch.Location = new System.Drawing.Point(3, 16);
            this.dgvTenderSearch.Name = "dgvTenderSearch";
            this.dgvTenderSearch.ReadOnly = true;
            this.dgvTenderSearch.RowHeadersVisible = false;
            this.dgvTenderSearch.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvTenderSearch.Size = new System.Drawing.Size(464, 164);
            this.dgvTenderSearch.TabIndex = 7;
            this.dgvTenderSearch.DoubleClick += new System.EventHandler(this.dgvTenderSearch_DoubleClick);
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::VATClient.Properties.Resources.Green_Footer;
            this.panel1.Controls.Add(this.LRecordCount);
            this.panel1.Controls.Add(this.btnOk);
            this.panel1.Controls.Add(this.btnRefresh);
            this.panel1.Location = new System.Drawing.Point(6, 277);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(465, 40);
            this.panel1.TabIndex = 7;
            // 
            // LRecordCount
            // 
            this.LRecordCount.AutoSize = true;
            this.LRecordCount.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LRecordCount.Location = new System.Drawing.Point(161, 12);
            this.LRecordCount.Name = "LRecordCount";
            this.LRecordCount.Size = new System.Drawing.Size(94, 16);
            this.LRecordCount.TabIndex = 212;
            this.LRecordCount.Text = "Record Count :";
            // 
            // btnOk
            // 
            this.btnOk.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnOk.Image = global::VATClient.Properties.Resources.Back;
            this.btnOk.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOk.Location = new System.Drawing.Point(355, 6);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 28);
            this.btnOk.TabIndex = 8;
            this.btnOk.Text = "Ok";
            this.btnOk.UseVisualStyleBackColor = false;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // backgroundWorkerSearch
            // 
            this.backgroundWorkerSearch.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerSearch_DoWork);
            this.backgroundWorkerSearch.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerSearch_RunWorkerCompleted);
            // 
            // backgroundWorkerSearchCustomer
            // 
            this.backgroundWorkerSearchCustomer.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerSearchCustomer_DoWork);
            this.backgroundWorkerSearchCustomer.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerSearchCustomer_RunWorkerCompleted);
            // 
            // TenderId
            // 
            this.TenderId.HeaderText = "Tender Id";
            this.TenderId.Name = "TenderId";
            this.TenderId.ReadOnly = true;
            this.TenderId.Visible = false;
            // 
            // RefNo
            // 
            this.RefNo.HeaderText = "Ref No";
            this.RefNo.Name = "RefNo";
            this.RefNo.ReadOnly = true;
            // 
            // CustomerGroupName
            // 
            this.CustomerGroupName.HeaderText = "Group Name";
            this.CustomerGroupName.Name = "CustomerGroupName";
            this.CustomerGroupName.ReadOnly = true;
            // 
            // GroupId
            // 
            this.GroupId.HeaderText = "GroupId";
            this.GroupId.Name = "GroupId";
            this.GroupId.ReadOnly = true;
            this.GroupId.Visible = false;
            // 
            // CustomerId
            // 
            this.CustomerId.HeaderText = "CustomerId";
            this.CustomerId.Name = "CustomerId";
            this.CustomerId.ReadOnly = true;
            this.CustomerId.Visible = false;
            // 
            // CustName
            // 
            this.CustName.HeaderText = "Customer";
            this.CustName.Name = "CustName";
            this.CustName.ReadOnly = true;
            // 
            // TenderDate
            // 
            this.TenderDate.HeaderText = "Tender Date";
            this.TenderDate.Name = "TenderDate";
            this.TenderDate.ReadOnly = true;
            // 
            // DeleveryDate
            // 
            this.DeleveryDate.HeaderText = "Delevery Date";
            this.DeleveryDate.Name = "DeleveryDate";
            this.DeleveryDate.ReadOnly = true;
            // 
            // Comments
            // 
            this.Comments.HeaderText = "Comments";
            this.Comments.Name = "Comments";
            this.Comments.ReadOnly = true;
            this.Comments.Visible = false;
            // 
            // BranchId
            // 
            this.BranchId.HeaderText = "Branch Id";
            this.BranchId.Name = "BranchId";
            this.BranchId.ReadOnly = true;
            this.BranchId.Visible = false;
            // 
            // FormTenderSearch
            // 
            this.AcceptButton = this.btnSearch;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(475, 317);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "FormTenderSearch";
            this.Text = "FormTenderSearch";
            this.Load += new System.EventHandler(this.FormTenderSearch_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvTenderSearch)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtTenderId;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtRefNo;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtCustomerName;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnRefresh;
        private System.ComponentModel.BackgroundWorker backgroundWorkerSearch;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.CheckBox chkCustCode;
        private System.Windows.Forms.ComboBox cmbCustomerName;
        private System.ComponentModel.BackgroundWorker backgroundWorkerSearchCustomer;
        private System.Windows.Forms.Label LRecordCount;
        private System.Windows.Forms.ComboBox cmbCustGrp;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DataGridView dgvTenderSearch;
        private System.Windows.Forms.ComboBox cmbBranch;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.DataGridViewTextBoxColumn TenderId;
        private System.Windows.Forms.DataGridViewTextBoxColumn RefNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn CustomerGroupName;
        private System.Windows.Forms.DataGridViewTextBoxColumn GroupId;
        private System.Windows.Forms.DataGridViewTextBoxColumn CustomerId;
        private System.Windows.Forms.DataGridViewTextBoxColumn CustName;
        private System.Windows.Forms.DataGridViewTextBoxColumn TenderDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn DeleveryDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn Comments;
        private System.Windows.Forms.DataGridViewTextBoxColumn BranchId;
        //private System.Windows.Forms.DataGridViewTextBoxColumn TenderId;
        //private System.Windows.Forms.DataGridViewTextBoxColumn RefNo;
        //private System.Windows.Forms.DataGridViewTextBoxColumn CustomerId;
    }
}