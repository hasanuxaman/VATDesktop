namespace VATClient.ReportPreview
{
    partial class FormRptVAT6_5
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
            this.txtIssueNo = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.grpTransactionType = new System.Windows.Forms.GroupBox();
            this.rbtn61Out = new System.Windows.Forms.RadioButton();
            this.rbtn62Out = new System.Windows.Forms.RadioButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnPreview = new System.Windows.Forms.Button();
            this.btnSearch = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.cmbBranchName = new System.Windows.Forms.ComboBox();
            this.labelTransferTo = new System.Windows.Forms.Label();
            this.cmbFontSize = new System.Windows.Forms.ComboBox();
            this.chbInEnglish = new System.Windows.Forms.CheckBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtIssueNo
            // 
            this.txtIssueNo.Location = new System.Drawing.Point(107, 12);
            this.txtIssueNo.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtIssueNo.MinimumSize = new System.Drawing.Size(185, 20);
            this.txtIssueNo.Name = "txtIssueNo";
            this.txtIssueNo.ReadOnly = true;
            this.txtIssueNo.Size = new System.Drawing.Size(192, 20);
            this.txtIssueNo.TabIndex = 116;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 13);
            this.label1.TabIndex = 117;
            this.label1.Text = "Issue No";
            // 
            // grpTransactionType
            // 
            this.grpTransactionType.Location = new System.Drawing.Point(143, 41);
            this.grpTransactionType.Name = "grpTransactionType";
            this.grpTransactionType.Size = new System.Drawing.Size(162, 34);
            this.grpTransactionType.TabIndex = 513;
            this.grpTransactionType.TabStop = false;
            // 
            // rbtn61Out
            // 
            this.rbtn61Out.AutoSize = true;
            this.rbtn61Out.Location = new System.Drawing.Point(170, 259);
            this.rbtn61Out.Name = "rbtn61Out";
            this.rbtn61Out.Size = new System.Drawing.Size(54, 17);
            this.rbtn61Out.TabIndex = 26;
            this.rbtn61Out.TabStop = true;
            this.rbtn61Out.Text = "61Out";
            this.rbtn61Out.UseVisualStyleBackColor = true;
            // 
            // rbtn62Out
            // 
            this.rbtn62Out.AutoSize = true;
            this.rbtn62Out.Location = new System.Drawing.Point(170, 236);
            this.rbtn62Out.Name = "rbtn62Out";
            this.rbtn62Out.Size = new System.Drawing.Size(54, 17);
            this.rbtn62Out.TabIndex = 27;
            this.rbtn62Out.Text = "62Out";
            this.rbtn62Out.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.panel1.Controls.Add(this.btnClose);
            this.panel1.Controls.Add(this.grpTransactionType);
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Controls.Add(this.btnPreview);
            this.panel1.Location = new System.Drawing.Point(1, 102);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(401, 40);
            this.panel1.TabIndex = 514;
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnClose.Image = global::VATClient.Properties.Resources.Back;
            this.btnClose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnClose.Location = new System.Drawing.Point(265, 7);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 28);
            this.btnClose.TabIndex = 8;
            this.btnClose.Text = "&Close";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnCancel.Image = global::VATClient.Properties.Resources.Referesh;
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.Location = new System.Drawing.Point(24, 7);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 28);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "&Refresh";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancel.UseVisualStyleBackColor = false;
            // 
            // btnPreview
            // 
            this.btnPreview.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnPreview.Image = global::VATClient.Properties.Resources.Print;
            this.btnPreview.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPreview.Location = new System.Drawing.Point(124, 6);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new System.Drawing.Size(112, 28);
            this.btnPreview.TabIndex = 6;
            this.btnPreview.Text = "VAT 6.5(Preview)";
            this.btnPreview.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnPreview.UseVisualStyleBackColor = false;
            this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSearch.Image = global::VATClient.Properties.Resources.search;
            this.btnSearch.Location = new System.Drawing.Point(310, 12);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(30, 20);
            this.btnSearch.TabIndex = 115;
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(75, 78);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(300, 15);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 514;
            this.progressBar1.Visible = false;
            // 
            // cmbBranchName
            // 
            this.cmbBranchName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBranchName.FormattingEnabled = true;
            this.cmbBranchName.Location = new System.Drawing.Point(107, 45);
            this.cmbBranchName.Name = "cmbBranchName";
            this.cmbBranchName.Size = new System.Drawing.Size(192, 21);
            this.cmbBranchName.TabIndex = 522;
            this.cmbBranchName.Visible = false;
            this.cmbBranchName.Leave += new System.EventHandler(this.cmbBranchName_Leave);
            // 
            // labelTransferTo
            // 
            this.labelTransferTo.AutoSize = true;
            this.labelTransferTo.Location = new System.Drawing.Point(18, 49);
            this.labelTransferTo.Name = "labelTransferTo";
            this.labelTransferTo.Size = new System.Drawing.Size(72, 13);
            this.labelTransferTo.TabIndex = 521;
            this.labelTransferTo.Text = "Branch Name";
            this.labelTransferTo.Visible = false;
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
            this.cmbFontSize.Location = new System.Drawing.Point(5, 75);
            this.cmbFontSize.Name = "cmbFontSize";
            this.cmbFontSize.Size = new System.Drawing.Size(36, 21);
            this.cmbFontSize.TabIndex = 523;
            this.cmbFontSize.Text = "8";
            // 
            // chbInEnglish
            // 
            this.chbInEnglish.AutoSize = true;
            this.chbInEnglish.Checked = true;
            this.chbInEnglish.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbInEnglish.Location = new System.Drawing.Point(109, 38);
            this.chbInEnglish.Name = "chbInEnglish";
            this.chbInEnglish.Size = new System.Drawing.Size(69, 17);
            this.chbInEnglish.TabIndex = 527;
            this.chbInEnglish.Text = "InEnglish";
            this.chbInEnglish.UseVisualStyleBackColor = true;
            // 
            // FormRptVAT6_5
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(206)))), ((int)(((byte)(206)))), ((int)(((byte)(246)))));
            this.ClientSize = new System.Drawing.Size(404, 142);
            this.Controls.Add(this.chbInEnglish);
            this.Controls.Add(this.cmbFontSize);
            this.Controls.Add(this.cmbBranchName);
            this.Controls.Add(this.labelTransferTo);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.rbtn62Out);
            this.Controls.Add(this.rbtn61Out);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.txtIssueNo);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormRptVAT6_5";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "VAT 6.5";
            this.Load += new System.EventHandler(this.FormRptVAT6_5_Load);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSearch;
        public System.Windows.Forms.TextBox txtIssueNo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox grpTransactionType;
        public System.Windows.Forms.RadioButton rbtn61Out;
        public System.Windows.Forms.RadioButton rbtn62Out;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnPreview;
        private System.Windows.Forms.ProgressBar progressBar1;
        public System.Windows.Forms.ComboBox cmbBranchName;
        private System.Windows.Forms.Label labelTransferTo;
        private System.Windows.Forms.ComboBox cmbFontSize;
        private System.Windows.Forms.CheckBox chbInEnglish;
    }
}