namespace VATClient
{
    partial class FormBanderolNameSearch
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
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.dgvBanderol = new System.Windows.Forms.DataGridView();
            this.BanderolID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BanderolName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BanderolSize = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.UOM = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Description = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.txtActiveStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.bgwSearch = new System.ComponentModel.BackgroundWorker();
            this.panel1 = new System.Windows.Forms.Panel();
            this.LRecordCount = new System.Windows.Forms.Label();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.txtBanderol = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.btnSearch = new System.Windows.Forms.Button();
            this.cmbActive = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtSize = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.dtpOpenDateTo = new System.Windows.Forms.DateTimePicker();
            this.dtpOpenDateFrom = new System.Windows.Forms.DateTimePicker();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBanderol)).BeginInit();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.progressBar1);
            this.groupBox2.Controls.Add(this.dgvBanderol);
            this.groupBox2.Location = new System.Drawing.Point(12, 106);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(465, 192);
            this.groupBox2.TabIndex = 190;
            this.groupBox2.TabStop = false;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(93, 40);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(250, 25);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 121;
            this.progressBar1.Visible = false;
            // 
            // dgvBanderol
            // 
            this.dgvBanderol.AllowUserToAddRows = false;
            this.dgvBanderol.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.dgvBanderol.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvBanderol.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvBanderol.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.BanderolID,
            this.BanderolName,
            this.BanderolSize,
            this.UOM,
            this.Description,
            this.txtActiveStatus});
            this.dgvBanderol.Location = new System.Drawing.Point(3, 11);
            this.dgvBanderol.Name = "dgvBanderol";
            this.dgvBanderol.RowHeadersVisible = false;
            this.dgvBanderol.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvBanderol.Size = new System.Drawing.Size(456, 174);
            this.dgvBanderol.TabIndex = 15;
            this.dgvBanderol.DoubleClick += new System.EventHandler(this.dgvBanderol_DoubleClick);
            // 
            // BanderolID
            // 
            this.BanderolID.HeaderText = "ID";
            this.BanderolID.Name = "BanderolID";
            this.BanderolID.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.BanderolID.Width = 70;
            // 
            // BanderolName
            // 
            this.BanderolName.HeaderText = "Name";
            this.BanderolName.Name = "BanderolName";
            this.BanderolName.ReadOnly = true;
            // 
            // BanderolSize
            // 
            this.BanderolSize.HeaderText = "Size";
            this.BanderolSize.Name = "BanderolSize";
            this.BanderolSize.ReadOnly = true;
            // 
            // UOM
            // 
            this.UOM.HeaderText = "UOM";
            this.UOM.Name = "UOM";
            // 
            // Description
            // 
            this.Description.HeaderText = "Description";
            this.Description.Name = "Description";
            this.Description.Visible = false;
            // 
            // txtActiveStatus
            // 
            this.txtActiveStatus.HeaderText = "Active Status";
            this.txtActiveStatus.Name = "txtActiveStatus";
            this.txtActiveStatus.ReadOnly = true;
            // 
            // bgwSearch
            // 
            this.bgwSearch.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwSearch_DoWork);
            this.bgwSearch.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwSearch_RunWorkerCompleted);
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::VATClient.Properties.Resources.Green_Footer;
            this.panel1.Controls.Add(this.LRecordCount);
            this.panel1.Controls.Add(this.btnOk);
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Location = new System.Drawing.Point(2, 311);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(488, 40);
            this.panel1.TabIndex = 189;
            this.panel1.TabStop = true;
            // 
            // LRecordCount
            // 
            this.LRecordCount.AutoSize = true;
            this.LRecordCount.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LRecordCount.Location = new System.Drawing.Point(96, 13);
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
            this.btnOk.Location = new System.Drawing.Point(399, 7);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 28);
            this.btnOk.TabIndex = 8;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = false;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnCancel.Image = global::VATClient.Properties.Resources.Referesh;
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.Location = new System.Drawing.Point(15, 7);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 28);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "&Refresh";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(14, 22);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 13);
            this.label4.TabIndex = 77;
            this.label4.Text = "Name";
            // 
            // txtBanderol
            // 
            this.txtBanderol.Location = new System.Drawing.Point(106, 19);
            this.txtBanderol.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtBanderol.MinimumSize = new System.Drawing.Size(250, 20);
            this.txtBanderol.Name = "txtBanderol";
            this.txtBanderol.Size = new System.Drawing.Size(250, 20);
            this.txtBanderol.TabIndex = 3;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(14, 70);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(73, 13);
            this.label9.TabIndex = 187;
            this.label9.Text = "Opening Date";
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSearch.Image = global::VATClient.Properties.Resources.search;
            this.btnSearch.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSearch.Location = new System.Drawing.Point(375, 27);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 28);
            this.btnSearch.TabIndex = 196;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // cmbActive
            // 
            this.cmbActive.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbActive.FormattingEnabled = true;
            this.cmbActive.Items.AddRange(new object[] {
            "Y",
            "N"});
            this.cmbActive.Location = new System.Drawing.Point(295, 43);
            this.cmbActive.Name = "cmbActive";
            this.cmbActive.Size = new System.Drawing.Size(61, 21);
            this.cmbActive.TabIndex = 211;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(226, 46);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(37, 13);
            this.label11.TabIndex = 212;
            this.label11.Text = "Active";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtSize);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.dtpOpenDateTo);
            this.groupBox1.Controls.Add(this.dtpOpenDateFrom);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.cmbActive);
            this.groupBox1.Controls.Add(this.btnSearch);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.txtBanderol);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Location = new System.Drawing.Point(12, 10);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(465, 95);
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            // 
            // txtSize
            // 
            this.txtSize.Location = new System.Drawing.Point(106, 43);
            this.txtSize.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtSize.MinimumSize = new System.Drawing.Size(100, 20);
            this.txtSize.Name = "txtSize";
            this.txtSize.Size = new System.Drawing.Size(100, 20);
            this.txtSize.TabIndex = 218;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(27, 13);
            this.label2.TabIndex = 219;
            this.label2.Text = "Size";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(223, 71);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(16, 13);
            this.label1.TabIndex = 217;
            this.label1.Text = "to";
            // 
            // dtpOpenDateTo
            // 
            this.dtpOpenDateTo.Checked = false;
            this.dtpOpenDateTo.CustomFormat = "dd/MMM/yyyy";
            this.dtpOpenDateTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpOpenDateTo.Location = new System.Drawing.Point(243, 67);
            this.dtpOpenDateTo.Name = "dtpOpenDateTo";
            this.dtpOpenDateTo.ShowCheckBox = true;
            this.dtpOpenDateTo.Size = new System.Drawing.Size(113, 20);
            this.dtpOpenDateTo.TabIndex = 216;
            // 
            // dtpOpenDateFrom
            // 
            this.dtpOpenDateFrom.Checked = false;
            this.dtpOpenDateFrom.CustomFormat = "dd/MMM/yyyy";
            this.dtpOpenDateFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpOpenDateFrom.Location = new System.Drawing.Point(106, 67);
            this.dtpOpenDateFrom.Name = "dtpOpenDateFrom";
            this.dtpOpenDateFrom.ShowCheckBox = true;
            this.dtpOpenDateFrom.Size = new System.Drawing.Size(113, 20);
            this.dtpOpenDateFrom.TabIndex = 214;
            // 
            // FormBanderolNameSearch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(490, 355);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.groupBox1);
            this.MaximizeBox = false;
            this.Name = "FormBanderolNameSearch";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Banderol Name Search";
            this.Load += new System.EventHandler(this.FormBanderolNameSearch_Load);
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvBanderol)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.DataGridView dgvBanderol;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label LRecordCount;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.ComponentModel.BackgroundWorker bgwSearch;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtBanderol;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.ComboBox cmbActive;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker dtpOpenDateTo;
        private System.Windows.Forms.DateTimePicker dtpOpenDateFrom;
        private System.Windows.Forms.TextBox txtSize;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridViewTextBoxColumn BanderolID;
        private System.Windows.Forms.DataGridViewTextBoxColumn BanderolName;
        private System.Windows.Forms.DataGridViewTextBoxColumn BanderolSize;
        private System.Windows.Forms.DataGridViewTextBoxColumn UOM;
        private System.Windows.Forms.DataGridViewTextBoxColumn Description;
        private System.Windows.Forms.DataGridViewTextBoxColumn txtActiveStatus;
    }
}