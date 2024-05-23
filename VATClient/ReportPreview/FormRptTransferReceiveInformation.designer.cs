namespace VATClient.ReportPages
{
    partial class FormRptTransferReceiveInformation
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
            this.grbBankInformation = new System.Windows.Forms.GroupBox();
            this.comtt = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cmbFontSize = new System.Windows.Forms.ComboBox();
            this.cmbBranchName = new System.Windows.Forms.ComboBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.labelBranch = new System.Windows.Forms.Label();
            this.txtProductType = new System.Windows.Forms.TextBox();
            this.txtTransactionType = new System.Windows.Forms.TextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.txtItemNo = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.dtpIssueToDate = new System.Windows.Forms.DateTimePicker();
            this.dtpIssueFromDate = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.txtIssueNo = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnPreview = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.backgroundWorkerPreviewDetails = new System.ComponentModel.BackgroundWorker();
            this.cmbBranchFrom = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.grbBankInformation.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // grbBankInformation
            // 
            this.grbBankInformation.Controls.Add(this.cmbBranchFrom);
            this.grbBankInformation.Controls.Add(this.label2);
            this.grbBankInformation.Controls.Add(this.comtt);
            this.grbBankInformation.Controls.Add(this.label5);
            this.grbBankInformation.Controls.Add(this.cmbFontSize);
            this.grbBankInformation.Controls.Add(this.cmbBranchName);
            this.grbBankInformation.Controls.Add(this.progressBar1);
            this.grbBankInformation.Controls.Add(this.labelBranch);
            this.grbBankInformation.Controls.Add(this.txtProductType);
            this.grbBankInformation.Controls.Add(this.txtTransactionType);
            this.grbBankInformation.Controls.Add(this.btnSearch);
            this.grbBankInformation.Controls.Add(this.txtItemNo);
            this.grbBankInformation.Controls.Add(this.label11);
            this.grbBankInformation.Controls.Add(this.dtpIssueToDate);
            this.grbBankInformation.Controls.Add(this.dtpIssueFromDate);
            this.grbBankInformation.Controls.Add(this.label3);
            this.grbBankInformation.Controls.Add(this.txtIssueNo);
            this.grbBankInformation.Controls.Add(this.label1);
            this.grbBankInformation.Location = new System.Drawing.Point(12, 2);
            this.grbBankInformation.Name = "grbBankInformation";
            this.grbBankInformation.Size = new System.Drawing.Size(365, 242);
            this.grbBankInformation.TabIndex = 79;
            this.grbBankInformation.TabStop = false;
            this.grbBankInformation.Text = "Reporting Criteria";
            this.grbBankInformation.Enter += new System.EventHandler(this.grbBankInformation_Enter);
            // 
            // comtt
            // 
            this.comtt.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comtt.FormattingEnabled = true;
            this.comtt.Items.AddRange(new object[] {
            "6.1 In",
            "6.2 In"});
            this.comtt.Location = new System.Drawing.Point(109, 120);
            this.comtt.Name = "comtt";
            this.comtt.Size = new System.Drawing.Size(115, 21);
            this.comtt.TabIndex = 528;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(20, 125);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(42, 13);
            this.label5.TabIndex = 527;
            this.label5.Text = "R.Type";
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
            this.cmbFontSize.Location = new System.Drawing.Point(6, 215);
            this.cmbFontSize.Name = "cmbFontSize";
            this.cmbFontSize.Size = new System.Drawing.Size(36, 21);
            this.cmbFontSize.TabIndex = 526;
            this.cmbFontSize.Text = "8";
            // 
            // cmbBranchName
            // 
            this.cmbBranchName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBranchName.FormattingEnabled = true;
            this.cmbBranchName.Location = new System.Drawing.Point(108, 96);
            this.cmbBranchName.Name = "cmbBranchName";
            this.cmbBranchName.Size = new System.Drawing.Size(117, 21);
            this.cmbBranchName.TabIndex = 524;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(43, 141);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(300, 15);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 208;
            this.progressBar1.Visible = false;
            // 
            // labelBranch
            // 
            this.labelBranch.AutoSize = true;
            this.labelBranch.Location = new System.Drawing.Point(20, 100);
            this.labelBranch.Name = "labelBranch";
            this.labelBranch.Size = new System.Drawing.Size(55, 13);
            this.labelBranch.TabIndex = 525;
            this.labelBranch.Text = "Branch To";
            // 
            // txtProductType
            // 
            this.txtProductType.Location = new System.Drawing.Point(301, 73);
            this.txtProductType.MinimumSize = new System.Drawing.Size(10, 20);
            this.txtProductType.Name = "txtProductType";
            this.txtProductType.ReadOnly = true;
            this.txtProductType.Size = new System.Drawing.Size(23, 21);
            this.txtProductType.TabIndex = 212;
            this.txtProductType.Visible = false;
            // 
            // txtTransactionType
            // 
            this.txtTransactionType.Location = new System.Drawing.Point(334, 73);
            this.txtTransactionType.Name = "txtTransactionType";
            this.txtTransactionType.Size = new System.Drawing.Size(25, 21);
            this.txtTransactionType.TabIndex = 209;
            this.txtTransactionType.Visible = false;
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSearch.Image = global::VATClient.Properties.Resources.search;
            this.btnSearch.Location = new System.Drawing.Point(313, 19);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(30, 20);
            this.btnSearch.TabIndex = 80;
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // txtItemNo
            // 
            this.txtItemNo.Location = new System.Drawing.Point(365, 68);
            this.txtItemNo.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtItemNo.MinimumSize = new System.Drawing.Size(25, 20);
            this.txtItemNo.Name = "txtItemNo";
            this.txtItemNo.ReadOnly = true;
            this.txtItemNo.Size = new System.Drawing.Size(25, 20);
            this.txtItemNo.TabIndex = 172;
            this.txtItemNo.Visible = false;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(216, 47);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(17, 13);
            this.label11.TabIndex = 120;
            this.label11.Text = "to";
            // 
            // dtpIssueToDate
            // 
            this.dtpIssueToDate.Checked = false;
            this.dtpIssueToDate.CustomFormat = "dd/MMM/yyyy";
            this.dtpIssueToDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpIssueToDate.Location = new System.Drawing.Point(242, 43);
            this.dtpIssueToDate.Name = "dtpIssueToDate";
            this.dtpIssueToDate.ShowCheckBox = true;
            this.dtpIssueToDate.Size = new System.Drawing.Size(101, 21);
            this.dtpIssueToDate.TabIndex = 118;
            // 
            // dtpIssueFromDate
            // 
            this.dtpIssueFromDate.Checked = false;
            this.dtpIssueFromDate.CustomFormat = "dd/MMM/yyyy";
            this.dtpIssueFromDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpIssueFromDate.Location = new System.Drawing.Point(109, 43);
            this.dtpIssueFromDate.Name = "dtpIssueFromDate";
            this.dtpIssueFromDate.ShowCheckBox = true;
            this.dtpIssueFromDate.Size = new System.Drawing.Size(101, 21);
            this.dtpIssueFromDate.TabIndex = 117;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(21, 47);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 13);
            this.label3.TabIndex = 119;
            this.label3.Text = "Issue Date";
            // 
            // txtIssueNo
            // 
            this.txtIssueNo.Location = new System.Drawing.Point(109, 19);
            this.txtIssueNo.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtIssueNo.MinimumSize = new System.Drawing.Size(185, 20);
            this.txtIssueNo.Name = "txtIssueNo";
            this.txtIssueNo.ReadOnly = true;
            this.txtIssueNo.Size = new System.Drawing.Size(192, 21);
            this.txtIssueNo.TabIndex = 113;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(90, 13);
            this.label1.TabIndex = 114;
            this.label1.Text = "TransferIssue No";
            // 
            // btnPreview
            // 
            this.btnPreview.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnPreview.Image = global::VATClient.Properties.Resources.Print;
            this.btnPreview.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPreview.Location = new System.Drawing.Point(13, 6);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new System.Drawing.Size(75, 28);
            this.btnPreview.TabIndex = 6;
            this.btnPreview.Text = "Preview";
            this.btnPreview.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnPreview.UseVisualStyleBackColor = false;
            this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::VATClient.Properties.Resources.Green_Footer;
            this.panel1.Controls.Add(this.btnClose);
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Controls.Add(this.btnPreview);
            this.panel1.Location = new System.Drawing.Point(0, 250);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(390, 40);
            this.panel1.TabIndex = 78;
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnClose.Image = global::VATClient.Properties.Resources.Back;
            this.btnClose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnClose.Location = new System.Drawing.Point(299, 7);
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
            this.btnCancel.Location = new System.Drawing.Point(95, 7);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 28);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "&Refresh";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // backgroundWorkerPreviewDetails
            // 
            this.backgroundWorkerPreviewDetails.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerPreviewDetails_DoWork);
            this.backgroundWorkerPreviewDetails.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerPreviewDetails_RunWorkerCompleted);
            // 
            // cmbBranchFrom
            // 
            this.cmbBranchFrom.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBranchFrom.FormattingEnabled = true;
            this.cmbBranchFrom.Location = new System.Drawing.Point(109, 70);
            this.cmbBranchFrom.Name = "cmbBranchFrom";
            this.cmbBranchFrom.Size = new System.Drawing.Size(117, 21);
            this.cmbBranchFrom.TabIndex = 529;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(19, 74);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 13);
            this.label2.TabIndex = 530;
            this.label2.Text = "Branch From";
            // 
            // FormRptTransferReceiveInformation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(384, 292);
            this.Controls.Add(this.grbBankInformation);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(400, 360);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(400, 330);
            this.Name = "FormRptTransferReceiveInformation";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Report (TransferIssue)";
            this.Load += new System.EventHandler(this.FormRptIssueInformation_Load);
            this.grbBankInformation.ResumeLayout(false);
            this.grbBankInformation.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grbBankInformation;
        private System.Windows.Forms.Button btnPreview;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtItemNo;
        public System.Windows.Forms.TextBox txtIssueNo;
        private System.Windows.Forms.Button btnSearch;
        public System.Windows.Forms.DateTimePicker dtpIssueToDate;
        public System.Windows.Forms.DateTimePicker dtpIssueFromDate;
        private System.ComponentModel.BackgroundWorker backgroundWorkerPreviewDetails;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.TextBox txtProductType;
        public System.Windows.Forms.TextBox txtTransactionType;
        private System.Windows.Forms.ComboBox cmbBranchName;
        private System.Windows.Forms.Label labelBranch;
        private System.Windows.Forms.ComboBox cmbFontSize;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox comtt;
        private System.Windows.Forms.ComboBox cmbBranchFrom;
        private System.Windows.Forms.Label label2;
    }
}