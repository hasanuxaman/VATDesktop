namespace VATClient
{
    partial class FormCustomerGroupSearch
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
            this.btnSearch = new System.Windows.Forms.Button();
            this.txtCustomerGroupID = new System.Windows.Forms.TextBox();
            this.txtCustomerGroupName = new System.Windows.Forms.TextBox();
            this.txtCustomerGroupDescription = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cmbType = new System.Windows.Forms.ComboBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.btnAdd = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.LRecordCount = new System.Windows.Forms.Label();
            this.btnOk = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.dgvCustomerGroup = new System.Windows.Forms.DataGridView();
            this.backgroundWorkerSearch = new System.ComponentModel.BackgroundWorker();
            this.CustomerGroupID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CustomerGroupName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CustomerGroupDescription = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label11 = new System.Windows.Forms.Label();
            this.cmbActive = new System.Windows.Forms.ComboBox();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCustomerGroup)).BeginInit();
            this.SuspendLayout();
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSearch.Image = global::VATClient.Properties.Resources.search;
            this.btnSearch.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSearch.Location = new System.Drawing.Point(378, 15);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 28);
            this.btnSearch.TabIndex = 3;
            this.btnSearch.Text = "&Search";
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // txtCustomerGroupID
            // 
            this.txtCustomerGroupID.Location = new System.Drawing.Point(42, 20);
            this.txtCustomerGroupID.MaximumSize = new System.Drawing.Size(125, 20);
            this.txtCustomerGroupID.MinimumSize = new System.Drawing.Size(150, 20);
            this.txtCustomerGroupID.Name = "txtCustomerGroupID";
            this.txtCustomerGroupID.Size = new System.Drawing.Size(150, 20);
            this.txtCustomerGroupID.TabIndex = 0;
            this.txtCustomerGroupID.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtCustomerGroupID_KeyDown);
            // 
            // txtCustomerGroupName
            // 
            this.txtCustomerGroupName.Location = new System.Drawing.Point(42, 44);
            this.txtCustomerGroupName.MaximumSize = new System.Drawing.Size(125, 20);
            this.txtCustomerGroupName.MinimumSize = new System.Drawing.Size(150, 20);
            this.txtCustomerGroupName.Name = "txtCustomerGroupName";
            this.txtCustomerGroupName.Size = new System.Drawing.Size(150, 20);
            this.txtCustomerGroupName.TabIndex = 1;
            this.txtCustomerGroupName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtCustomerGroupName_KeyDown);
            // 
            // txtCustomerGroupDescription
            // 
            this.txtCustomerGroupDescription.Location = new System.Drawing.Point(615, 102);
            this.txtCustomerGroupDescription.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtCustomerGroupDescription.MinimumSize = new System.Drawing.Size(50, 20);
            this.txtCustomerGroupDescription.Name = "txtCustomerGroupDescription";
            this.txtCustomerGroupDescription.Size = new System.Drawing.Size(50, 20);
            this.txtCustomerGroupDescription.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(18, 13);
            this.label1.TabIndex = 43;
            this.label1.Text = "ID";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 13);
            this.label2.TabIndex = 44;
            this.label2.Text = "Name";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(541, 105);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(64, 13);
            this.label3.TabIndex = 45;
            this.label3.Text = "Description:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.cmbType);
            this.groupBox1.Controls.Add(this.cmbActive);
            this.groupBox1.Controls.Add(this.btnCancel);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.txtCustomerGroupID);
            this.groupBox1.Controls.Add(this.txtCustomerGroupName);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.btnSearch);
            this.groupBox1.Location = new System.Drawing.Point(0, -3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(460, 76);
            this.groupBox1.TabIndex = 47;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Searching Criteria";
            // 
            // cmbType
            // 
            this.cmbType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbType.FormattingEnabled = true;
            this.cmbType.Items.AddRange(new object[] {
            "Local",
            "Export"});
            this.cmbType.Location = new System.Drawing.Point(275, 43);
            this.cmbType.Name = "cmbType";
            this.cmbType.Size = new System.Drawing.Size(97, 21);
            this.cmbType.TabIndex = 4;
            this.cmbType.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmbType_KeyDown);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnCancel.Image = global::VATClient.Properties.Resources.Referesh;
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.Location = new System.Drawing.Point(378, 44);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 28);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "&Refresh";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(202, 48);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(31, 13);
            this.label9.TabIndex = 189;
            this.label9.Text = "Type";
            // 
            // btnAdd
            // 
            this.btnAdd.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnAdd.Image = global::VATClient.Properties.Resources.add_over;
            this.btnAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAdd.Location = new System.Drawing.Point(16, 6);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 28);
            this.btnAdd.TabIndex = 7;
            this.btnAdd.Text = "&Add";
            this.btnAdd.UseVisualStyleBackColor = false;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::VATClient.Properties.Resources.Green_Footer;
            this.panel1.Controls.Add(this.LRecordCount);
            this.panel1.Controls.Add(this.btnOk);
            this.panel1.Controls.Add(this.btnAdd);
            this.panel1.Location = new System.Drawing.Point(0, 261);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(459, 40);
            this.panel1.TabIndex = 6;
            this.panel1.TabStop = true;
            // 
            // LRecordCount
            // 
            this.LRecordCount.AutoSize = true;
            this.LRecordCount.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LRecordCount.Location = new System.Drawing.Point(98, 12);
            this.LRecordCount.Name = "LRecordCount";
            this.LRecordCount.Size = new System.Drawing.Size(94, 16);
            this.LRecordCount.TabIndex = 226;
            this.LRecordCount.Text = "Record Count :";
            // 
            // btnOk
            // 
            this.btnOk.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnOk.Image = global::VATClient.Properties.Resources.Back;
            this.btnOk.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOk.Location = new System.Drawing.Point(347, 6);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 28);
            this.btnOk.TabIndex = 8;
            this.btnOk.Text = "&Ok";
            this.btnOk.UseVisualStyleBackColor = false;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.progressBar1);
            this.groupBox2.Controls.Add(this.dgvCustomerGroup);
            this.groupBox2.Location = new System.Drawing.Point(-1, 73);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(460, 184);
            this.groupBox2.TabIndex = 106;
            this.groupBox2.TabStop = false;
            this.groupBox2.Enter += new System.EventHandler(this.groupBox2_Enter);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(48, 95);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(349, 23);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 107;
            this.progressBar1.Visible = false;
            // 
            // dgvCustomerGroup
            // 
            this.dgvCustomerGroup.AllowUserToAddRows = false;
            this.dgvCustomerGroup.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.dgvCustomerGroup.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvCustomerGroup.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCustomerGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvCustomerGroup.Location = new System.Drawing.Point(3, 17);
            this.dgvCustomerGroup.Name = "dgvCustomerGroup";
            this.dgvCustomerGroup.RowHeadersVisible = false;
            this.dgvCustomerGroup.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvCustomerGroup.Size = new System.Drawing.Size(454, 164);
            this.dgvCustomerGroup.TabIndex = 40;
            this.dgvCustomerGroup.DoubleClick += new System.EventHandler(this.dgvCustomerGroup_DoubleClick);
            // 
            // backgroundWorkerSearch
            // 
            this.backgroundWorkerSearch.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerSearch_DoWork);
            this.backgroundWorkerSearch.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerSearch_RunWorkerCompleted);
            // 
            // CustomerGroupID
            // 
            this.CustomerGroupID.DataPropertyName = "CustomerGroupID";
            this.CustomerGroupID.HeaderText = "Group ID";
            this.CustomerGroupID.Name = "CustomerGroupID";
            this.CustomerGroupID.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // CustomerGroupName
            // 
            this.CustomerGroupName.DataPropertyName = "CustomerGroupName";
            this.CustomerGroupName.HeaderText = "Name";
            this.CustomerGroupName.Name = "CustomerGroupName";
            this.CustomerGroupName.Width = 150;
            // 
            // CustomerGroupDescription
            // 
            this.CustomerGroupDescription.DataPropertyName = "CustomerGroupDescription";
            this.CustomerGroupDescription.HeaderText = "Description";
            this.CustomerGroupDescription.Name = "CustomerGroupDescription";
            this.CustomerGroupDescription.Visible = false;
            this.CustomerGroupDescription.Width = 115;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(202, 20);
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
            this.cmbActive.Location = new System.Drawing.Point(275, 16);
            this.cmbActive.Name = "cmbActive";
            this.cmbActive.Size = new System.Drawing.Size(44, 21);
            this.cmbActive.TabIndex = 211;
            // 
            // FormCustomerGroupSearch
            // 
            this.AcceptButton = this.btnSearch;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.CancelButton = this.btnOk;
            this.ClientSize = new System.Drawing.Size(459, 302);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtCustomerGroupDescription);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(500, 375);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(50, 50);
            this.Name = "FormCustomerGroupSearch";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Customer Group Search";
            this.Load += new System.EventHandler(this.FormCustomerGroupSearch_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvCustomerGroup)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.TextBox txtCustomerGroupID;
        private System.Windows.Forms.TextBox txtCustomerGroupName;
        private System.Windows.Forms.TextBox txtCustomerGroupDescription;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.DataGridView dgvCustomerGroup;
        public System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.ComboBox cmbType;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.ComponentModel.BackgroundWorker backgroundWorkerSearch;
        private System.Windows.Forms.DataGridViewTextBoxColumn CustomerGroupID;
        private System.Windows.Forms.DataGridViewTextBoxColumn CustomerGroupName;
        private System.Windows.Forms.DataGridViewTextBoxColumn CustomerGroupDescription;
        private System.Windows.Forms.Label LRecordCount;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ComboBox cmbActive;
    }
}