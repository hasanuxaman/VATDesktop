namespace VATClient
{
    partial class FormSaleExportSearch
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSaleExportSearch));
            this.dgvHistory = new System.Windows.Forms.DataGridView();
            this.SaleExportNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SaleExportDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Description = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Comments = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Quantity = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.GrossWeight = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NetWeight = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NumberFrom = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NumberTo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PortFrom = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PortTo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Post = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.LRecordCount = new System.Windows.Forms.Label();
            this.dtpToDate = new System.Windows.Forms.DateTimePicker();
            this.dtpFromDate = new System.Windows.Forms.DateTimePicker();
            this.cmbPost = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.btnSearch = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.txtSaleExportNo = new System.Windows.Forms.TextBox();
            this.cmbBranch = new System.Windows.Forms.ComboBox();
            this.label15 = new System.Windows.Forms.Label();
            this.BranchId1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHistory)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvHistory
            // 
            this.dgvHistory.AllowUserToAddRows = false;
            this.dgvHistory.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black;
            this.dgvHistory.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvHistory.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvHistory.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.SaleExportNo,
            this.SaleExportDate,
            this.Description,
            this.Comments,
            this.Quantity,
            this.GrossWeight,
            this.NetWeight,
            this.NumberFrom,
            this.NumberTo,
            this.PortFrom,
            this.PortTo,
            this.Post,
            this.BranchId1});
            this.dgvHistory.Location = new System.Drawing.Point(7, 62);
            this.dgvHistory.Name = "dgvHistory";
            this.dgvHistory.RowHeadersVisible = false;
            this.dgvHistory.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvHistory.Size = new System.Drawing.Size(643, 335);
            this.dgvHistory.TabIndex = 8;
            this.dgvHistory.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvHistory_CellContentClick);
            this.dgvHistory.DoubleClick += new System.EventHandler(this.dgvHistory_DoubleClick);
            // 
            // SaleExportNo
            // 
            this.SaleExportNo.HeaderText = "SaleExportNo";
            this.SaleExportNo.Name = "SaleExportNo";
            this.SaleExportNo.ReadOnly = true;
            // 
            // SaleExportDate
            // 
            this.SaleExportDate.HeaderText = "SaleExportDate";
            this.SaleExportDate.Name = "SaleExportDate";
            this.SaleExportDate.ReadOnly = true;
            // 
            // Description
            // 
            this.Description.HeaderText = "Description";
            this.Description.Name = "Description";
            this.Description.ReadOnly = true;
            // 
            // Comments
            // 
            this.Comments.HeaderText = "Comments";
            this.Comments.Name = "Comments";
            this.Comments.ReadOnly = true;
            // 
            // Quantity
            // 
            this.Quantity.HeaderText = "Quantity";
            this.Quantity.Name = "Quantity";
            this.Quantity.ReadOnly = true;
            // 
            // GrossWeight
            // 
            this.GrossWeight.HeaderText = "GrossWeight";
            this.GrossWeight.Name = "GrossWeight";
            this.GrossWeight.ReadOnly = true;
            // 
            // NetWeight
            // 
            this.NetWeight.HeaderText = "NetWeight";
            this.NetWeight.Name = "NetWeight";
            this.NetWeight.ReadOnly = true;
            // 
            // NumberFrom
            // 
            this.NumberFrom.HeaderText = "NumberFrom";
            this.NumberFrom.Name = "NumberFrom";
            this.NumberFrom.ReadOnly = true;
            // 
            // NumberTo
            // 
            this.NumberTo.HeaderText = "NumberTo";
            this.NumberTo.Name = "NumberTo";
            this.NumberTo.ReadOnly = true;
            // 
            // PortFrom
            // 
            this.PortFrom.HeaderText = "PortFrom";
            this.PortFrom.Name = "PortFrom";
            this.PortFrom.ReadOnly = true;
            // 
            // PortTo
            // 
            this.PortTo.HeaderText = "PortTo";
            this.PortTo.Name = "PortTo";
            this.PortTo.ReadOnly = true;
            // 
            // Post
            // 
            this.Post.HeaderText = "Post";
            this.Post.Name = "Post";
            this.Post.ReadOnly = true;
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::VATClient.Properties.Resources.Green_Footer;
            this.panel1.Controls.Add(this.btnOk);
            this.panel1.Controls.Add(this.btnRefresh);
            this.panel1.Controls.Add(this.LRecordCount);
            this.panel1.Location = new System.Drawing.Point(0, 403);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(667, 40);
            this.panel1.TabIndex = 236;
            // 
            // btnOk
            // 
            this.btnOk.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnOk.Image = global::VATClient.Properties.Resources.Back;
            this.btnOk.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOk.Location = new System.Drawing.Point(567, 9);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 28);
            this.btnOk.TabIndex = 6;
            this.btnOk.Text = "&Ok";
            this.btnOk.UseVisualStyleBackColor = false;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnRefresh.Image = ((System.Drawing.Image)(resources.GetObject("btnRefresh.Image")));
            this.btnRefresh.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnRefresh.Location = new System.Drawing.Point(25, 9);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(75, 28);
            this.btnRefresh.TabIndex = 5;
            this.btnRefresh.Text = "&Refresh";
            this.btnRefresh.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnRefresh.UseVisualStyleBackColor = false;
            // 
            // LRecordCount
            // 
            this.LRecordCount.AutoSize = true;
            this.LRecordCount.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LRecordCount.Location = new System.Drawing.Point(106, 16);
            this.LRecordCount.Name = "LRecordCount";
            this.LRecordCount.Size = new System.Drawing.Size(94, 16);
            this.LRecordCount.TabIndex = 230;
            this.LRecordCount.Text = "Record Count :";
            // 
            // dtpToDate
            // 
            this.dtpToDate.Checked = false;
            this.dtpToDate.CustomFormat = "dd/MMM/yyyy";
            this.dtpToDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpToDate.Location = new System.Drawing.Point(390, 36);
            this.dtpToDate.Name = "dtpToDate";
            this.dtpToDate.ShowCheckBox = true;
            this.dtpToDate.Size = new System.Drawing.Size(108, 20);
            this.dtpToDate.TabIndex = 244;
            // 
            // dtpFromDate
            // 
            this.dtpFromDate.Checked = false;
            this.dtpFromDate.CustomFormat = "dd/MMM/yyyy";
            this.dtpFromDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpFromDate.Location = new System.Drawing.Point(262, 36);
            this.dtpFromDate.Name = "dtpFromDate";
            this.dtpFromDate.ShowCheckBox = true;
            this.dtpFromDate.Size = new System.Drawing.Size(101, 20);
            this.dtpFromDate.TabIndex = 243;
            // 
            // cmbPost
            // 
            this.cmbPost.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPost.FormattingEnabled = true;
            this.cmbPost.Items.AddRange(new object[] {
            "Y",
            "N"});
            this.cmbPost.Location = new System.Drawing.Point(297, 9);
            this.cmbPost.Name = "cmbPost";
            this.cmbPost.Size = new System.Drawing.Size(53, 21);
            this.cmbPost.TabIndex = 239;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(4, 15);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(32, 13);
            this.label9.TabIndex = 242;
            this.label9.Text = "Code";
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSearch.Image = global::VATClient.Properties.Resources.search;
            this.btnSearch.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSearch.Location = new System.Drawing.Point(534, 11);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 28);
            this.btnSearch.TabIndex = 240;
            this.btnSearch.Text = "&Search";
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(187, 39);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 13);
            this.label3.TabIndex = 246;
            this.label3.Text = "Deposit Date";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(263, 13);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(28, 13);
            this.label19.TabIndex = 245;
            this.label19.Text = "Post";
            // 
            // txtSaleExportNo
            // 
            this.txtSaleExportNo.BackColor = System.Drawing.SystemColors.Window;
            this.txtSaleExportNo.Enabled = false;
            this.txtSaleExportNo.Location = new System.Drawing.Point(68, 11);
            this.txtSaleExportNo.MaximumSize = new System.Drawing.Size(4, 5);
            this.txtSaleExportNo.MinimumSize = new System.Drawing.Size(185, 20);
            this.txtSaleExportNo.Name = "txtSaleExportNo";
            this.txtSaleExportNo.Size = new System.Drawing.Size(185, 20);
            this.txtSaleExportNo.TabIndex = 247;
            // 
            // cmbBranch
            // 
            this.cmbBranch.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBranch.FormattingEnabled = true;
            this.cmbBranch.Items.AddRange(new object[] {
            "Y",
            "N"});
            this.cmbBranch.Location = new System.Drawing.Point(407, 9);
            this.cmbBranch.Name = "cmbBranch";
            this.cmbBranch.Size = new System.Drawing.Size(100, 21);
            this.cmbBranch.TabIndex = 249;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(358, 13);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(41, 13);
            this.label15.TabIndex = 248;
            this.label15.Text = "Branch";
            // 
            // BranchId1
            // 
            this.BranchId1.HeaderText = "BranchId";
            this.BranchId1.Name = "BranchId1";
            this.BranchId1.ReadOnly = true;
            // 
            // FormSaleExportSearch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(665, 447);
            this.Controls.Add(this.cmbBranch);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.txtSaleExportNo);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label19);
            this.Controls.Add(this.dtpToDate);
            this.Controls.Add(this.dtpFromDate);
            this.Controls.Add(this.cmbPost);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.dgvHistory);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormSaleExportSearch";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Sale Export Search";
            this.Load += new System.EventHandler(this.FormSaleExportSearch_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvHistory)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvHistory;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Label LRecordCount;
        private System.Windows.Forms.DateTimePicker dtpToDate;
        private System.Windows.Forms.DateTimePicker dtpFromDate;
        private System.Windows.Forms.ComboBox cmbPost;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.DataGridViewTextBoxColumn SaleExportNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn SaleExportDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn Description;
        private System.Windows.Forms.DataGridViewTextBoxColumn Comments;
        private System.Windows.Forms.DataGridViewTextBoxColumn Quantity;
        private System.Windows.Forms.DataGridViewTextBoxColumn GrossWeight;
        private System.Windows.Forms.DataGridViewTextBoxColumn NetWeight;
        private System.Windows.Forms.DataGridViewTextBoxColumn NumberFrom;
        private System.Windows.Forms.DataGridViewTextBoxColumn NumberTo;
        private System.Windows.Forms.DataGridViewTextBoxColumn PortFrom;
        private System.Windows.Forms.DataGridViewTextBoxColumn PortTo;
        private System.Windows.Forms.DataGridViewTextBoxColumn Post;
        private System.Windows.Forms.TextBox txtSaleExportNo;
        private System.Windows.Forms.ComboBox cmbBranch;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.DataGridViewTextBoxColumn BranchId1;
    }
}