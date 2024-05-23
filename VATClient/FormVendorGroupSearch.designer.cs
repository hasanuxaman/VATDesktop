namespace VATClient
{
    partial class FormVendorGroupSearch
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
            this.grbVendorGroup = new System.Windows.Forms.GroupBox();
            this.label11 = new System.Windows.Forms.Label();
            this.cmbActive = new System.Windows.Forms.ComboBox();
            this.cmbType = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.btnAdd = new System.Windows.Forms.Button();
            this.txtVendorGroupName = new System.Windows.Forms.TextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.txtVendorGroupID = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.dgvVendorGroup = new System.Windows.Forms.DataGridView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.LRecordCount = new System.Windows.Forms.Label();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.bgwSearch = new System.ComponentModel.BackgroundWorker();
            this.VendorGroupID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.VendorGroupName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Description = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Comments = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.GroupType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ActiveStatus1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.grbVendorGroup.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvVendorGroup)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // grbVendorGroup
            // 
            this.grbVendorGroup.Controls.Add(this.label11);
            this.grbVendorGroup.Controls.Add(this.cmbActive);
            this.grbVendorGroup.Controls.Add(this.cmbType);
            this.grbVendorGroup.Controls.Add(this.label9);
            this.grbVendorGroup.Controls.Add(this.btnAdd);
            this.grbVendorGroup.Controls.Add(this.txtVendorGroupName);
            this.grbVendorGroup.Controls.Add(this.btnSearch);
            this.grbVendorGroup.Controls.Add(this.label2);
            this.grbVendorGroup.Controls.Add(this.txtVendorGroupID);
            this.grbVendorGroup.Controls.Add(this.label1);
            this.grbVendorGroup.Location = new System.Drawing.Point(12, 1);
            this.grbVendorGroup.Name = "grbVendorGroup";
            this.grbVendorGroup.Size = new System.Drawing.Size(460, 94);
            this.grbVendorGroup.TabIndex = 15;
            this.grbVendorGroup.TabStop = false;
            this.grbVendorGroup.Text = "Searching Criteria";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(292, 22);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(37, 13);
            this.label11.TabIndex = 212;
            this.label11.Text = "Active";
            // 
            // cmbActive
            // 
            this.cmbActive.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbActive.FormattingEnabled = true;
            this.cmbActive.Items.AddRange(new object[] {
            "Y",
            "N"});
            this.cmbActive.Location = new System.Drawing.Point(330, 19);
            this.cmbActive.Name = "cmbActive";
            this.cmbActive.Size = new System.Drawing.Size(44, 21);
            this.cmbActive.TabIndex = 211;
            // 
            // cmbType
            // 
            this.cmbType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbType.FormattingEnabled = true;
            this.cmbType.Items.AddRange(new object[] {
            "Local",
            "Import"});
            this.cmbType.Location = new System.Drawing.Point(329, 41);
            this.cmbType.Name = "cmbType";
            this.cmbType.Size = new System.Drawing.Size(125, 21);
            this.cmbType.TabIndex = 3;
            this.cmbType.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmbType_KeyDown);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(292, 45);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(31, 13);
            this.label9.TabIndex = 187;
            this.label9.Text = "Type";
            // 
            // btnAdd
            // 
            this.btnAdd.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnAdd.Image = global::VATClient.Properties.Resources.add_over;
            this.btnAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAdd.Location = new System.Drawing.Point(380, 66);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 28);
            this.btnAdd.TabIndex = 5;
            this.btnAdd.Text = "&Add";
            this.btnAdd.UseVisualStyleBackColor = false;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // txtVendorGroupName
            // 
            this.txtVendorGroupName.Location = new System.Drawing.Point(83, 45);
            this.txtVendorGroupName.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtVendorGroupName.MinimumSize = new System.Drawing.Size(125, 20);
            this.txtVendorGroupName.Name = "txtVendorGroupName";
            this.txtVendorGroupName.Size = new System.Drawing.Size(125, 21);
            this.txtVendorGroupName.TabIndex = 1;
            this.txtVendorGroupName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtVendorGroupName_KeyDown);
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSearch.Image = global::VATClient.Properties.Resources.search;
            this.btnSearch.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSearch.Location = new System.Drawing.Point(299, 66);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 28);
            this.btnSearch.TabIndex = 4;
            this.btnSearch.Text = "&Search";
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(66, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Group Name";
            // 
            // txtVendorGroupID
            // 
            this.txtVendorGroupID.Location = new System.Drawing.Point(83, 18);
            this.txtVendorGroupID.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtVendorGroupID.MinimumSize = new System.Drawing.Size(125, 20);
            this.txtVendorGroupID.Name = "txtVendorGroupID";
            this.txtVendorGroupID.Size = new System.Drawing.Size(125, 21);
            this.txtVendorGroupID.TabIndex = 0;
            this.txtVendorGroupID.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtVendorGroupID_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Group ID";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.progressBar1);
            this.groupBox1.Controls.Add(this.dgvVendorGroup);
            this.groupBox1.Location = new System.Drawing.Point(12, 95);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(460, 198);
            this.groupBox1.TabIndex = 106;
            this.groupBox1.TabStop = false;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(84, 174);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(290, 24);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 209;
            this.progressBar1.Visible = false;
            // 
            // dgvVendorGroup
            // 
            this.dgvVendorGroup.AllowUserToAddRows = false;
            this.dgvVendorGroup.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.dgvVendorGroup.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvVendorGroup.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvVendorGroup.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.VendorGroupID,
            this.VendorGroupName,
            this.Description,
            this.Comments,
            this.GroupType,
            this.ActiveStatus1});
            this.dgvVendorGroup.Location = new System.Drawing.Point(6, 8);
            this.dgvVendorGroup.Name = "dgvVendorGroup";
            this.dgvVendorGroup.RowHeadersVisible = false;
            this.dgvVendorGroup.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvVendorGroup.Size = new System.Drawing.Size(448, 183);
            this.dgvVendorGroup.TabIndex = 18;
            this.dgvVendorGroup.TabStop = false;
            this.dgvVendorGroup.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvVendorGroup_CellContentClick);
            this.dgvVendorGroup.DoubleClick += new System.EventHandler(this.dgvVendorGroup_DoubleClick);
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::VATClient.Properties.Resources.Green_Footer;
            this.panel1.Controls.Add(this.LRecordCount);
            this.panel1.Controls.Add(this.btnOk);
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Location = new System.Drawing.Point(-2, 298);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(487, 40);
            this.panel1.TabIndex = 6;
            // 
            // LRecordCount
            // 
            this.LRecordCount.AutoSize = true;
            this.LRecordCount.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LRecordCount.Location = new System.Drawing.Point(95, 12);
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
            this.btnOk.Location = new System.Drawing.Point(399, 6);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 28);
            this.btnOk.TabIndex = 8;
            this.btnOk.Text = "Ok";
            this.btnOk.UseVisualStyleBackColor = false;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnCancel.Image = global::VATClient.Properties.Resources.Referesh;
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.Location = new System.Drawing.Point(14, 6);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 28);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "&Refresh";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // bgwSearch
            // 
            this.bgwSearch.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwSearch_DoWork);
            this.bgwSearch.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwSearch_RunWorkerCompleted);
            // 
            // VendorGroupID
            // 
            this.VendorGroupID.DataPropertyName = "VendorGroupID";
            this.VendorGroupID.HeaderText = "Group ID";
            this.VendorGroupID.Name = "VendorGroupID";
            this.VendorGroupID.ReadOnly = true;
            this.VendorGroupID.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.VendorGroupID.Visible = false;
            // 
            // VendorGroupName
            // 
            this.VendorGroupName.DataPropertyName = "VendorGroupName";
            this.VendorGroupName.HeaderText = "Name";
            this.VendorGroupName.Name = "VendorGroupName";
            this.VendorGroupName.ReadOnly = true;
            this.VendorGroupName.Width = 150;
            // 
            // Description
            // 
            this.Description.DataPropertyName = "Description";
            this.Description.HeaderText = "Description";
            this.Description.Name = "Description";
            this.Description.ReadOnly = true;
            // 
            // Comments
            // 
            this.Comments.DataPropertyName = "Comments";
            this.Comments.HeaderText = "Comments";
            this.Comments.Name = "Comments";
            this.Comments.ReadOnly = true;
            this.Comments.Visible = false;
            // 
            // GroupType
            // 
            this.GroupType.DataPropertyName = "GroupType";
            this.GroupType.HeaderText = "GroupType";
            this.GroupType.Name = "GroupType";
            this.GroupType.ReadOnly = true;
            // 
            // ActiveStatus1
            // 
            this.ActiveStatus1.HeaderText = "Active Status";
            this.ActiveStatus1.Name = "ActiveStatus1";
            // 
            // FormVendorGroupSearch
            // 
            this.AcceptButton = this.btnSearch;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.CancelButton = this.btnOk;
            this.ClientSize = new System.Drawing.Size(484, 336);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.grbVendorGroup);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(500, 375);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(500, 375);
            this.Name = "FormVendorGroupSearch";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Vendor Group Search";
            this.Load += new System.EventHandler(this.FormVendorGroupSearch_Load);
            this.grbVendorGroup.ResumeLayout(false);
            this.grbVendorGroup.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvVendorGroup)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.GroupBox grbVendorGroup;
        private System.Windows.Forms.TextBox txtVendorGroupName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtVendorGroupID;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridView dgvVendorGroup;
        public System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.ComboBox cmbType;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.ComponentModel.BackgroundWorker bgwSearch;
        private System.Windows.Forms.Label LRecordCount;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ComboBox cmbActive;
        private System.Windows.Forms.DataGridViewTextBoxColumn VendorGroupID;
        private System.Windows.Forms.DataGridViewTextBoxColumn VendorGroupName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Description;
        private System.Windows.Forms.DataGridViewTextBoxColumn Comments;
        private System.Windows.Forms.DataGridViewTextBoxColumn GroupType;
        private System.Windows.Forms.DataGridViewTextBoxColumn ActiveStatus1;
    }
}