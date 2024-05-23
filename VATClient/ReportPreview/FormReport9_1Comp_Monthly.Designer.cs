namespace VATClient
{
    partial class FormReport9_1Comp_Monthly
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
            this.cmbBranch = new System.Windows.Forms.ComboBox();
            this.cmbReportType = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.dtpFromDate = new System.Windows.Forms.DateTimePicker();
            this.dtpToDate = new System.Windows.Forms.DateTimePicker();
            this.cmbFontSize = new System.Windows.Forms.ComboBox();
            this.label16 = new System.Windows.Forms.Label();
            this.cmbTransferTo = new System.Windows.Forms.ComboBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnSumBranch = new System.Windows.Forms.Button();
            this.btnSummary = new System.Windows.Forms.Button();
            this.btnDownloadDtls = new System.Windows.Forms.Button();
            this.Download = new System.Windows.Forms.Button();
            this.bgwPreview = new System.ComponentModel.BackgroundWorker();
            this.grbBankInformation.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // grbBankInformation
            // 
            this.grbBankInformation.Controls.Add(this.cmbBranch);
            this.grbBankInformation.Controls.Add(this.cmbReportType);
            this.grbBankInformation.Controls.Add(this.label1);
            this.grbBankInformation.Controls.Add(this.progressBar1);
            this.grbBankInformation.Controls.Add(this.dtpFromDate);
            this.grbBankInformation.Controls.Add(this.dtpToDate);
            this.grbBankInformation.Controls.Add(this.cmbFontSize);
            this.grbBankInformation.Controls.Add(this.label16);
            this.grbBankInformation.Location = new System.Drawing.Point(7, 6);
            this.grbBankInformation.Margin = new System.Windows.Forms.Padding(4);
            this.grbBankInformation.Name = "grbBankInformation";
            this.grbBankInformation.Padding = new System.Windows.Forms.Padding(4);
            this.grbBankInformation.Size = new System.Drawing.Size(688, 195);
            this.grbBankInformation.TabIndex = 83;
            this.grbBankInformation.TabStop = false;
            this.grbBankInformation.Text = "Reporting Criteria";
            // 
            // cmbBranch
            // 
            this.cmbBranch.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBranch.FormattingEnabled = true;
            this.cmbBranch.Location = new System.Drawing.Point(276, 92);
            this.cmbBranch.Margin = new System.Windows.Forms.Padding(4);
            this.cmbBranch.Name = "cmbBranch";
            this.cmbBranch.Size = new System.Drawing.Size(216, 24);
            this.cmbBranch.TabIndex = 534;
            // 
            // cmbReportType
            // 
            this.cmbReportType.FormattingEnabled = true;
            this.cmbReportType.Items.AddRange(new object[] {
            "Sale",
            "Purchase"});
            this.cmbReportType.Location = new System.Drawing.Point(131, 92);
            this.cmbReportType.Name = "cmbReportType";
            this.cmbReportType.Size = new System.Drawing.Size(138, 24);
            this.cmbReportType.TabIndex = 532;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(318, 21);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(99, 17);
            this.label1.TabIndex = 531;
            this.label1.Text = "Second Month";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(32, 140);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(568, 39);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 530;
            this.progressBar1.Visible = false;
            // 
            // dtpFromDate
            // 
            this.dtpFromDate.CustomFormat = "yyyy-MM-dd";
            this.dtpFromDate.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpFromDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpFromDate.Location = new System.Drawing.Point(144, 49);
            this.dtpFromDate.Margin = new System.Windows.Forms.Padding(4);
            this.dtpFromDate.Name = "dtpFromDate";
            this.dtpFromDate.Size = new System.Drawing.Size(152, 30);
            this.dtpFromDate.TabIndex = 183;
            // 
            // dtpToDate
            // 
            this.dtpToDate.CustomFormat = "yyyy-MM-dd";
            this.dtpToDate.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpToDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpToDate.Location = new System.Drawing.Point(304, 49);
            this.dtpToDate.Margin = new System.Windows.Forms.Padding(4);
            this.dtpToDate.Name = "dtpToDate";
            this.dtpToDate.Size = new System.Drawing.Size(162, 30);
            this.dtpToDate.TabIndex = 529;
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
            this.cmbFontSize.Location = new System.Drawing.Point(702, 111);
            this.cmbFontSize.Margin = new System.Windows.Forms.Padding(4);
            this.cmbFontSize.Name = "cmbFontSize";
            this.cmbFontSize.Size = new System.Drawing.Size(47, 24);
            this.cmbFontSize.TabIndex = 522;
            this.cmbFontSize.Text = "8";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(170, 20);
            this.label16.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(78, 17);
            this.label16.TabIndex = 39;
            this.label16.Text = "First Month";
            // 
            // cmbTransferTo
            // 
            this.cmbTransferTo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTransferTo.FormattingEnabled = true;
            this.cmbTransferTo.Items.AddRange(new object[] {
            "Detail",
            "Summery",
            "SummeryByProduct",
            "Single ",
            "Monthly",
            "TDS"});
            this.cmbTransferTo.Location = new System.Drawing.Point(334, 65);
            this.cmbTransferTo.Margin = new System.Windows.Forms.Padding(4);
            this.cmbTransferTo.Name = "cmbTransferTo";
            this.cmbTransferTo.Size = new System.Drawing.Size(241, 24);
            this.cmbTransferTo.TabIndex = 523;
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::VATClient.Properties.Resources.Green_Footer;
            this.panel1.Controls.Add(this.btnSumBranch);
            this.panel1.Controls.Add(this.btnSummary);
            this.panel1.Controls.Add(this.btnDownloadDtls);
            this.panel1.Controls.Add(this.Download);
            this.panel1.Controls.Add(this.cmbTransferTo);
            this.panel1.Location = new System.Drawing.Point(7, 209);
            this.panel1.Margin = new System.Windows.Forms.Padding(4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(688, 49);
            this.panel1.TabIndex = 82;
            // 
            // btnSumBranch
            // 
            this.btnSumBranch.BackColor = System.Drawing.Color.White;
            this.btnSumBranch.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSumBranch.Location = new System.Drawing.Point(434, 3);
            this.btnSumBranch.Margin = new System.Windows.Forms.Padding(4);
            this.btnSumBranch.Name = "btnSumBranch";
            this.btnSumBranch.Size = new System.Drawing.Size(230, 41);
            this.btnSumBranch.TabIndex = 526;
            this.btnSumBranch.Text = "Download Summary Branch";
            this.btnSumBranch.UseVisualStyleBackColor = false;
            this.btnSumBranch.Visible = false;
            this.btnSumBranch.Click += new System.EventHandler(this.btnSumBranch_Click);
            // 
            // btnSummary
            // 
            this.btnSummary.BackColor = System.Drawing.Color.White;
            this.btnSummary.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSummary.Location = new System.Drawing.Point(279, 4);
            this.btnSummary.Margin = new System.Windows.Forms.Padding(4);
            this.btnSummary.Name = "btnSummary";
            this.btnSummary.Size = new System.Drawing.Size(147, 41);
            this.btnSummary.TabIndex = 525;
            this.btnSummary.Text = "Download Summary";
            this.btnSummary.UseVisualStyleBackColor = false;
            this.btnSummary.Visible = false;
            this.btnSummary.Click += new System.EventHandler(this.btnSummary_Click);
            // 
            // btnDownloadDtls
            // 
            this.btnDownloadDtls.BackColor = System.Drawing.Color.White;
            this.btnDownloadDtls.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDownloadDtls.Location = new System.Drawing.Point(135, 4);
            this.btnDownloadDtls.Margin = new System.Windows.Forms.Padding(4);
            this.btnDownloadDtls.Name = "btnDownloadDtls";
            this.btnDownloadDtls.Size = new System.Drawing.Size(135, 41);
            this.btnDownloadDtls.TabIndex = 524;
            this.btnDownloadDtls.Text = "Download Details";
            this.btnDownloadDtls.UseVisualStyleBackColor = false;
            this.btnDownloadDtls.Click += new System.EventHandler(this.btnDownloadDtls_Click);
            // 
            // Download
            // 
            this.Download.BackColor = System.Drawing.Color.White;
            this.Download.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Download.Location = new System.Drawing.Point(6, 4);
            this.Download.Margin = new System.Windows.Forms.Padding(4);
            this.Download.Name = "Download";
            this.Download.Size = new System.Drawing.Size(124, 41);
            this.Download.TabIndex = 110;
            this.Download.Text = "Download";
            this.Download.UseVisualStyleBackColor = false;
            this.Download.Click += new System.EventHandler(this.Download_Click);
            // 
            // bgwPreview
            // 
            this.bgwPreview.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwPreview_DoWork);
            this.bgwPreview.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwPreview_RunWorkerCompleted);
            // 
            // FormReport9_1Comp_Monthly
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(693, 262);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.grbBankInformation);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormReport9_1Comp_Monthly";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "9.1 Month Comparison";
            this.Load += new System.EventHandler(this.ReportSCBL_Load);
            this.grbBankInformation.ResumeLayout(false);
            this.grbBankInformation.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grbBankInformation;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ComboBox cmbFontSize;
        private System.Windows.Forms.ComboBox cmbTransferTo;
        private System.Windows.Forms.Button Download;
        private System.ComponentModel.BackgroundWorker bgwPreview;
        private System.Windows.Forms.DateTimePicker dtpFromDate;
        private System.Windows.Forms.DateTimePicker dtpToDate;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbReportType;
        private System.Windows.Forms.ComboBox cmbBranch;
        private System.Windows.Forms.Button btnDownloadDtls;
        private System.Windows.Forms.Button btnSummary;
        private System.Windows.Forms.Button btnSumBranch;
    }
}