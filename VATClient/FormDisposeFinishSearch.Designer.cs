namespace VATClient
{
    partial class FormDisposeFinishSearch
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle15 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle16 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle13 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle14 = new System.Windows.Forms.DataGridViewCellStyle();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cmbBranch = new System.Windows.Forms.ComboBox();
            this.label20 = new System.Windows.Forms.Label();
            this.cmbPost = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.btnSearch = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.txtDisposeNumber = new System.Windows.Forms.TextBox();
            this.dtpPostTo = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.dtpPostFrom = new System.Windows.Forms.DateTimePicker();
            this.label4 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.dgvDFI = new System.Windows.Forms.DataGridView();
            this.DisposeNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DisposeDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AppVATAmountImport = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AppTotalPriceImport = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AppTotalPrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RefNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AppVATAmount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AppDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Remarks = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AppRefNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AppRemarks = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Post = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BranchId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel1 = new System.Windows.Forms.Panel();
            this.LRecordCount = new System.Windows.Forms.Label();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.backgroundWorkerSearch = new System.ComponentModel.BackgroundWorker();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDFI)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cmbBranch);
            this.groupBox1.Controls.Add(this.label20);
            this.groupBox1.Controls.Add(this.cmbPost);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.btnSearch);
            this.groupBox1.Controls.Add(this.btnAdd);
            this.groupBox1.Controls.Add(this.txtDisposeNumber);
            this.groupBox1.Controls.Add(this.dtpPostTo);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.dtpPostFrom);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(-1, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(628, 94);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Searching Criteria";
            // 
            // cmbBranch
            // 
            this.cmbBranch.FormattingEnabled = true;
            this.cmbBranch.Location = new System.Drawing.Point(331, 67);
            this.cmbBranch.Name = "cmbBranch";
            this.cmbBranch.Size = new System.Drawing.Size(140, 21);
            this.cmbBranch.TabIndex = 228;
            this.cmbBranch.SelectedIndexChanged += new System.EventHandler(this.cmbBranch_SelectedIndexChanged);
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(286, 72);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(41, 13);
            this.label20.TabIndex = 227;
            this.label20.Text = "Branch";
            this.label20.Click += new System.EventHandler(this.label20_Click);
            // 
            // cmbPost
            // 
            this.cmbPost.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPost.FormattingEnabled = true;
            this.cmbPost.Items.AddRange(new object[] {
            "Y",
            "N"});
            this.cmbPost.Location = new System.Drawing.Point(95, 44);
            this.cmbPost.Name = "cmbPost";
            this.cmbPost.Size = new System.Drawing.Size(147, 21);
            this.cmbPost.TabIndex = 1;
            this.cmbPost.SelectedIndexChanged += new System.EventHandler(this.cmbPost_SelectedIndexChanged);
            this.cmbPost.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmbPost_KeyDown);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 50);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(28, 13);
            this.label9.TabIndex = 202;
            this.label9.Text = "Post";
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSearch.Image = global::VATClient.Properties.Resources.search;
            this.btnSearch.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSearch.Location = new System.Drawing.Point(532, 13);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 28);
            this.btnSearch.TabIndex = 4;
            this.btnSearch.Text = "&Search";
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnAdd.Image = global::VATClient.Properties.Resources.add_over;
            this.btnAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAdd.Location = new System.Drawing.Point(534, 49);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 28);
            this.btnAdd.TabIndex = 5;
            this.btnAdd.Text = "&Add";
            this.btnAdd.UseVisualStyleBackColor = false;
            this.btnAdd.Visible = false;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // txtDisposeNumber
            // 
            this.txtDisposeNumber.Location = new System.Drawing.Point(95, 18);
            this.txtDisposeNumber.Name = "txtDisposeNumber";
            this.txtDisposeNumber.Size = new System.Drawing.Size(147, 20);
            this.txtDisposeNumber.TabIndex = 0;
            this.txtDisposeNumber.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtDisposeNumber_KeyDown);
            // 
            // dtpPostTo
            // 
            this.dtpPostTo.Checked = false;
            this.dtpPostTo.CustomFormat = "dd/MMM/yyyy";
            this.dtpPostTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpPostTo.Location = new System.Drawing.Point(331, 44);
            this.dtpPostTo.Name = "dtpPostTo";
            this.dtpPostTo.ShowCheckBox = true;
            this.dtpPostTo.Size = new System.Drawing.Size(140, 20);
            this.dtpPostTo.TabIndex = 3;
            this.dtpPostTo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dtpPostTo_KeyDown);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(283, 49);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(44, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Post To";
            // 
            // dtpPostFrom
            // 
            this.dtpPostFrom.Checked = false;
            this.dtpPostFrom.CustomFormat = "dd/MMM/yyyy";
            this.dtpPostFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpPostFrom.Location = new System.Drawing.Point(331, 18);
            this.dtpPostFrom.Name = "dtpPostFrom";
            this.dtpPostFrom.ShowCheckBox = true;
            this.dtpPostFrom.Size = new System.Drawing.Size(140, 20);
            this.dtpPostFrom.TabIndex = 2;
            this.dtpPostFrom.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dtpPostFrom_KeyDown);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(273, 21);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(54, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Post From";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(85, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Dispose Number";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.progressBar1);
            this.groupBox2.Controls.Add(this.dgvDFI);
            this.groupBox2.Location = new System.Drawing.Point(0, 95);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(627, 188);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Search Result";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(54, 76);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(516, 29);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 204;
            this.progressBar1.Visible = false;
            // 
            // dgvDFI
            // 
            this.dgvDFI.AllowUserToAddRows = false;
            this.dgvDFI.AllowUserToDeleteRows = false;
            dataGridViewCellStyle9.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            dataGridViewCellStyle9.ForeColor = System.Drawing.Color.Black;
            this.dgvDFI.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle9;
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle10.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle10.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle10.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle10.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvDFI.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle10;
            this.dgvDFI.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDFI.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.DisposeNumber,
            this.DisposeDate,
            this.AppVATAmountImport,
            this.AppTotalPriceImport,
            this.AppTotalPrice,
            this.RefNumber,
            this.AppVATAmount,
            this.AppDate,
            this.Remarks,
            this.AppRefNumber,
            this.AppRemarks,
            this.Post,
            this.BranchId});
            dataGridViewCellStyle15.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle15.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle15.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle15.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle15.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle15.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle15.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvDFI.DefaultCellStyle = dataGridViewCellStyle15;
            this.dgvDFI.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvDFI.Location = new System.Drawing.Point(3, 16);
            this.dgvDFI.Name = "dgvDFI";
            dataGridViewCellStyle16.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle16.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle16.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle16.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle16.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle16.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle16.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvDFI.RowHeadersDefaultCellStyle = dataGridViewCellStyle16;
            this.dgvDFI.RowHeadersVisible = false;
            this.dgvDFI.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvDFI.Size = new System.Drawing.Size(621, 169);
            this.dgvDFI.TabIndex = 0;
            this.dgvDFI.DoubleClick += new System.EventHandler(this.dgvDFI_DoubleClick);
            // 
            // DisposeNumber
            // 
            this.DisposeNumber.DataPropertyName = "DisposeNumber";
            this.DisposeNumber.HeaderText = "Dispose No";
            this.DisposeNumber.Name = "DisposeNumber";
            // 
            // DisposeDate
            // 
            this.DisposeDate.DataPropertyName = "DisposeDate";
            this.DisposeDate.HeaderText = "Dispose Date";
            this.DisposeDate.Name = "DisposeDate";
            // 
            // AppVATAmountImport
            // 
            this.AppVATAmountImport.DataPropertyName = "AppVATAmountImport";
            dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.AppVATAmountImport.DefaultCellStyle = dataGridViewCellStyle11;
            this.AppVATAmountImport.HeaderText = "App VAT Amount Import";
            this.AppVATAmountImport.Name = "AppVATAmountImport";
            this.AppVATAmountImport.ReadOnly = true;
            // 
            // AppTotalPriceImport
            // 
            this.AppTotalPriceImport.DataPropertyName = "AppTotalPriceImport";
            dataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.AppTotalPriceImport.DefaultCellStyle = dataGridViewCellStyle12;
            this.AppTotalPriceImport.HeaderText = "App Total Price Import";
            this.AppTotalPriceImport.Name = "AppTotalPriceImport";
            this.AppTotalPriceImport.ReadOnly = true;
            // 
            // AppTotalPrice
            // 
            this.AppTotalPrice.DataPropertyName = "AppTotalPrice";
            dataGridViewCellStyle13.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.AppTotalPrice.DefaultCellStyle = dataGridViewCellStyle13;
            this.AppTotalPrice.HeaderText = "App Total Price";
            this.AppTotalPrice.Name = "AppTotalPrice";
            this.AppTotalPrice.ReadOnly = true;
            // 
            // RefNumber
            // 
            this.RefNumber.DataPropertyName = "RefNumber";
            this.RefNumber.HeaderText = "RefNumber";
            this.RefNumber.Name = "RefNumber";
            this.RefNumber.Visible = false;
            // 
            // AppVATAmount
            // 
            this.AppVATAmount.DataPropertyName = "AppVATAmount";
            dataGridViewCellStyle14.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.AppVATAmount.DefaultCellStyle = dataGridViewCellStyle14;
            this.AppVATAmount.HeaderText = "App VAT Amount";
            this.AppVATAmount.Name = "AppVATAmount";
            this.AppVATAmount.ReadOnly = true;
            // 
            // AppDate
            // 
            this.AppDate.DataPropertyName = "AppDate";
            this.AppDate.HeaderText = "App Date";
            this.AppDate.Name = "AppDate";
            this.AppDate.ReadOnly = true;
            // 
            // Remarks
            // 
            this.Remarks.DataPropertyName = "Remarks";
            this.Remarks.HeaderText = "Remarks";
            this.Remarks.Name = "Remarks";
            this.Remarks.ReadOnly = true;
            this.Remarks.Visible = false;
            // 
            // AppRefNumber
            // 
            this.AppRefNumber.DataPropertyName = "AppRefNumber";
            this.AppRefNumber.HeaderText = "AppRefNumber";
            this.AppRefNumber.Name = "AppRefNumber";
            this.AppRefNumber.ReadOnly = true;
            this.AppRefNumber.Visible = false;
            // 
            // AppRemarks
            // 
            this.AppRemarks.DataPropertyName = "AppRemarks";
            this.AppRemarks.HeaderText = "AppRemarks";
            this.AppRemarks.Name = "AppRemarks";
            this.AppRemarks.ReadOnly = true;
            this.AppRemarks.Visible = false;
            // 
            // Post
            // 
            this.Post.DataPropertyName = "Post";
            this.Post.HeaderText = "Post";
            this.Post.Name = "Post";
            this.Post.ReadOnly = true;
            // 
            // BranchId
            // 
            this.BranchId.HeaderText = "BranchId";
            this.BranchId.Name = "BranchId";
            this.BranchId.ReadOnly = true;
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::VATClient.Properties.Resources.Green_Footer;
            this.panel1.Controls.Add(this.LRecordCount);
            this.panel1.Controls.Add(this.btnOk);
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Location = new System.Drawing.Point(0, 289);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(627, 40);
            this.panel1.TabIndex = 6;
            // 
            // LRecordCount
            // 
            this.LRecordCount.AutoSize = true;
            this.LRecordCount.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LRecordCount.Location = new System.Drawing.Point(95, 12);
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
            this.btnOk.Location = new System.Drawing.Point(533, 6);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 28);
            this.btnOk.TabIndex = 8;
            this.btnOk.Text = "Ok";
            this.btnOk.UseVisualStyleBackColor = false;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click_1);
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
            // backgroundWorkerSearch
            // 
            this.backgroundWorkerSearch.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerSearch_DoWork);
            this.backgroundWorkerSearch.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerSearch_RunWorkerCompleted);
            // 
            // FormDisposeFinishSearch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(635, 330);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "FormDisposeFinishSearch";
            this.Text = "Dispose Finish Item Search";
            this.Load += new System.EventHandler(this.FormDisposeFinishSearch_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDFI)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txtDisposeNumber;
        private System.Windows.Forms.DateTimePicker dtpPostFrom;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnSearch;
        public System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.DateTimePicker dtpPostTo;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.DataGridView dgvDFI;
        private System.Windows.Forms.ComboBox cmbPost;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.ComponentModel.BackgroundWorker backgroundWorkerSearch;
        private System.Windows.Forms.Label LRecordCount;
        private System.Windows.Forms.DataGridViewTextBoxColumn DisposeNumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn DisposeDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn AppVATAmountImport;
        private System.Windows.Forms.DataGridViewTextBoxColumn AppTotalPriceImport;
        private System.Windows.Forms.DataGridViewTextBoxColumn AppTotalPrice;
        private System.Windows.Forms.DataGridViewTextBoxColumn RefNumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn AppVATAmount;
        private System.Windows.Forms.DataGridViewTextBoxColumn AppDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn Remarks;
        private System.Windows.Forms.DataGridViewTextBoxColumn AppRefNumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn AppRemarks;
        private System.Windows.Forms.DataGridViewTextBoxColumn Post;
        private System.Windows.Forms.ComboBox cmbBranch;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.DataGridViewTextBoxColumn BranchId;
    }
}