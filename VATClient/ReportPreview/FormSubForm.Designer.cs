namespace VATClient.ReportPreview
{
    partial class FormSubForm
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
            this.btnPrint = new System.Windows.Forms.Button();
            this.dgvSubForm = new System.Windows.Forms.DataGridView();
            this.cmbNoteNo = new System.Windows.Forms.ComboBox();
            this.dtpDate = new System.Windows.Forms.DateTimePicker();
            this.btnload = new System.Windows.Forms.Button();
            this.txtBranchName = new System.Windows.Forms.TextBox();
            this.Download = new System.Windows.Forms.Button();
            this.cmbFontSize = new System.Windows.Forms.ComboBox();
            this.ckbVAT9_1 = new System.Windows.Forms.CheckBox();
            this.cmbPost = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.chkSummary = new System.Windows.Forms.CheckBox();
            this.lblRowCount = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbDecimalPlace = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSubForm)).BeginInit();
            this.SuspendLayout();
            // 
            // btnPrint
            // 
            this.btnPrint.BackColor = System.Drawing.SystemColors.Control;
            this.btnPrint.Image = global::VATClient.Properties.Resources.Print;
            this.btnPrint.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPrint.Location = new System.Drawing.Point(373, 8);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(78, 28);
            this.btnPrint.TabIndex = 100;
            this.btnPrint.Text = "Print";
            this.btnPrint.UseVisualStyleBackColor = false;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // dgvSubForm
            // 
            this.dgvSubForm.AllowUserToAddRows = false;
            this.dgvSubForm.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.dgvSubForm.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvSubForm.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSubForm.Location = new System.Drawing.Point(0, 69);
            this.dgvSubForm.Name = "dgvSubForm";
            this.dgvSubForm.RowHeadersVisible = false;
            this.dgvSubForm.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvSubForm.Size = new System.Drawing.Size(705, 346);
            this.dgvSubForm.TabIndex = 102;
            this.dgvSubForm.TabStop = false;
            // 
            // cmbNoteNo
            // 
            this.cmbNoteNo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbNoteNo.FormattingEnabled = true;
            this.cmbNoteNo.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15",
            "16",
            "17",
            "18",
            "19",
            "20",
            "21",
            "22",
            "24",
            "26",
            "27",
            "29",
            "30",
            "31",
            "32",
            "52",
            "53",
            "54",
            "55",
            "56",
            "57",
            "58",
            "59",
            "60",
            "61"});
            this.cmbNoteNo.Location = new System.Drawing.Point(175, 11);
            this.cmbNoteNo.Name = "cmbNoteNo";
            this.cmbNoteNo.Size = new System.Drawing.Size(105, 21);
            this.cmbNoteNo.TabIndex = 105;
            // 
            // dtpDate
            // 
            this.dtpDate.CustomFormat = "MMMM-yyyy";
            this.dtpDate.Enabled = false;
            this.dtpDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpDate.Location = new System.Drawing.Point(12, 12);
            this.dtpDate.Name = "dtpDate";
            this.dtpDate.Size = new System.Drawing.Size(121, 20);
            this.dtpDate.TabIndex = 106;
            // 
            // btnload
            // 
            this.btnload.BackColor = System.Drawing.SystemColors.Control;
            this.btnload.Image = global::VATClient.Properties.Resources.Load;
            this.btnload.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnload.Location = new System.Drawing.Point(286, 8);
            this.btnload.Name = "btnload";
            this.btnload.Size = new System.Drawing.Size(81, 28);
            this.btnload.TabIndex = 107;
            this.btnload.Text = "Load";
            this.btnload.UseVisualStyleBackColor = false;
            this.btnload.Click += new System.EventHandler(this.btnload_Click);
            // 
            // txtBranchName
            // 
            this.txtBranchName.Location = new System.Drawing.Point(550, 12);
            this.txtBranchName.Name = "txtBranchName";
            this.txtBranchName.ReadOnly = true;
            this.txtBranchName.Size = new System.Drawing.Size(144, 20);
            this.txtBranchName.TabIndex = 108;
            // 
            // Download
            // 
            this.Download.Image = global::VATClient.Properties.Resources.Load;
            this.Download.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Download.Location = new System.Drawing.Point(458, 8);
            this.Download.Name = "Download";
            this.Download.Size = new System.Drawing.Size(86, 27);
            this.Download.TabIndex = 109;
            this.Download.Text = "Download";
            this.Download.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Download.UseVisualStyleBackColor = true;
            this.Download.Click += new System.EventHandler(this.Download_Click);
            // 
            // cmbFontSize
            // 
            this.cmbFontSize.FormattingEnabled = true;
            this.cmbFontSize.Items.AddRange(new object[] {
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15"});
            this.cmbFontSize.Location = new System.Drawing.Point(72, 41);
            this.cmbFontSize.Name = "cmbFontSize";
            this.cmbFontSize.Size = new System.Drawing.Size(36, 21);
            this.cmbFontSize.TabIndex = 110;
            this.cmbFontSize.Text = "8";
            // 
            // ckbVAT9_1
            // 
            this.ckbVAT9_1.AutoSize = true;
            this.ckbVAT9_1.Checked = true;
            this.ckbVAT9_1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckbVAT9_1.Enabled = false;
            this.ckbVAT9_1.Location = new System.Drawing.Point(642, 39);
            this.ckbVAT9_1.Name = "ckbVAT9_1";
            this.ckbVAT9_1.Size = new System.Drawing.Size(48, 17);
            this.ckbVAT9_1.TabIndex = 221;
            this.ckbVAT9_1.Text = "New";
            this.ckbVAT9_1.UseVisualStyleBackColor = true;
            // 
            // cmbPost
            // 
            this.cmbPost.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPost.Enabled = false;
            this.cmbPost.FormattingEnabled = true;
            this.cmbPost.Items.AddRange(new object[] {
            "Y",
            "N",
            "All"});
            this.cmbPost.Location = new System.Drawing.Point(289, 40);
            this.cmbPost.Name = "cmbPost";
            this.cmbPost.Size = new System.Drawing.Size(50, 21);
            this.cmbPost.TabIndex = 235;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(256, 44);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(28, 13);
            this.label9.TabIndex = 236;
            this.label9.Text = "Post";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 44);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(51, 13);
            this.label3.TabIndex = 237;
            this.label3.Text = "Font Size";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(139, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 13);
            this.label1.TabIndex = 238;
            this.label1.Text = "Note";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(208, 205);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(290, 33);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 239;
            this.progressBar1.Visible = false;
            // 
            // chkSummary
            // 
            this.chkSummary.AutoSize = true;
            this.chkSummary.Checked = true;
            this.chkSummary.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSummary.Location = new System.Drawing.Point(346, 42);
            this.chkSummary.Name = "chkSummary";
            this.chkSummary.Size = new System.Drawing.Size(69, 17);
            this.chkSummary.TabIndex = 240;
            this.chkSummary.Text = "Summary";
            this.chkSummary.UseVisualStyleBackColor = true;
            // 
            // lblRowCount
            // 
            this.lblRowCount.AutoSize = true;
            this.lblRowCount.Location = new System.Drawing.Point(2, 423);
            this.lblRowCount.Name = "lblRowCount";
            this.lblRowCount.Size = new System.Drawing.Size(63, 13);
            this.lblRowCount.TabIndex = 241;
            this.lblRowCount.Text = "Row Count:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(149, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 13);
            this.label2.TabIndex = 243;
            this.label2.Text = "Decimal";
            // 
            // cmbDecimalPlace
            // 
            this.cmbDecimalPlace.FormattingEnabled = true;
            this.cmbDecimalPlace.Items.AddRange(new object[] {
            "0",
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9"});
            this.cmbDecimalPlace.Location = new System.Drawing.Point(200, 44);
            this.cmbDecimalPlace.Name = "cmbDecimalPlace";
            this.cmbDecimalPlace.Size = new System.Drawing.Size(36, 21);
            this.cmbDecimalPlace.TabIndex = 564;
            this.cmbDecimalPlace.Text = "2";
            // 
            // FormSubForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(706, 443);
            this.Controls.Add(this.cmbDecimalPlace);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblRowCount);
            this.Controls.Add(this.chkSummary);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cmbPost);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.ckbVAT9_1);
            this.Controls.Add(this.cmbFontSize);
            this.Controls.Add(this.Download);
            this.Controls.Add(this.txtBranchName);
            this.Controls.Add(this.btnload);
            this.Controls.Add(this.dtpDate);
            this.Controls.Add(this.cmbNoteNo);
            this.Controls.Add(this.dgvSubForm);
            this.Controls.Add(this.btnPrint);
            this.Name = "FormSubForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Sub Forms";
            this.Load += new System.EventHandler(this.FormSubForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSubForm)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnPrint;
        public System.Windows.Forms.ComboBox cmbNoteNo;
        private System.Windows.Forms.DataGridView dgvSubForm;
        public System.Windows.Forms.DateTimePicker dtpDate;
        private System.Windows.Forms.Button btnload;
        public System.Windows.Forms.TextBox txtBranchName;
        private System.Windows.Forms.Button Download;
        private System.Windows.Forms.ComboBox cmbFontSize;
        public System.Windows.Forms.CheckBox ckbVAT9_1;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.ComboBox cmbPost;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.CheckBox chkSummary;
        private System.Windows.Forms.Label lblRowCount;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbDecimalPlace;
    }
}