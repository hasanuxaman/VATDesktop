namespace VATClient
{
    partial class FormReportSCBL
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
            this.chbToll = new System.Windows.Forms.CheckBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.label3 = new System.Windows.Forms.Label();
            this.chbInEnglish = new System.Windows.Forms.CheckBox();
            this.lblTransferTo = new System.Windows.Forms.Label();
            this.cmbTransferTo = new System.Windows.Forms.ComboBox();
            this.cmbFontSize = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbBranchName = new System.Windows.Forms.ComboBox();
            this.label16 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.Download = new System.Windows.Forms.Button();
            this.btnPrev = new System.Windows.Forms.Button();
            this.bgwPreview = new System.ComponentModel.BackgroundWorker();
            this.dtpFromDate = new System.Windows.Forms.DateTimePicker();
            this.dtpToDate = new System.Windows.Forms.DateTimePicker();
            this.grbBankInformation.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // grbBankInformation
            // 
            this.grbBankInformation.Controls.Add(this.dtpToDate);
            this.grbBankInformation.Controls.Add(this.chbToll);
            this.grbBankInformation.Controls.Add(this.progressBar1);
            this.grbBankInformation.Controls.Add(this.label3);
            this.grbBankInformation.Controls.Add(this.chbInEnglish);
            this.grbBankInformation.Controls.Add(this.lblTransferTo);
            this.grbBankInformation.Controls.Add(this.cmbTransferTo);
            this.grbBankInformation.Controls.Add(this.cmbFontSize);
            this.grbBankInformation.Controls.Add(this.label2);
            this.grbBankInformation.Controls.Add(this.cmbBranchName);
            this.grbBankInformation.Controls.Add(this.label16);
            this.grbBankInformation.Location = new System.Drawing.Point(5, 5);
            this.grbBankInformation.Name = "grbBankInformation";
            this.grbBankInformation.Size = new System.Drawing.Size(472, 116);
            this.grbBankInformation.TabIndex = 83;
            this.grbBankInformation.TabStop = false;
            this.grbBankInformation.Text = "Reporting Criteria";
            // 
            // chbToll
            // 
            this.chbToll.AutoSize = true;
            this.chbToll.Location = new System.Drawing.Point(92, 46);
            this.chbToll.Name = "chbToll";
            this.chbToll.Size = new System.Drawing.Size(43, 17);
            this.chbToll.TabIndex = 528;
            this.chbToll.Text = "Toll";
            this.chbToll.UseVisualStyleBackColor = true;
            this.chbToll.Visible = false;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(92, 60);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(250, 22);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 196;
            this.progressBar1.Visible = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(263, 25);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(16, 13);
            this.label3.TabIndex = 527;
            this.label3.Text = "to";
            // 
            // chbInEnglish
            // 
            this.chbInEnglish.AutoSize = true;
            this.chbInEnglish.Location = new System.Drawing.Point(89, 86);
            this.chbInEnglish.Name = "chbInEnglish";
            this.chbInEnglish.Size = new System.Drawing.Size(59, 17);
            this.chbInEnglish.TabIndex = 222;
            this.chbInEnglish.Text = "Bangla";
            this.chbInEnglish.UseVisualStyleBackColor = true;
            this.chbInEnglish.Click += new System.EventHandler(this.chbInEnglish_Click);
            // 
            // lblTransferTo
            // 
            this.lblTransferTo.AutoSize = true;
            this.lblTransferTo.Location = new System.Drawing.Point(9, 93);
            this.lblTransferTo.Name = "lblTransferTo";
            this.lblTransferTo.Size = new System.Drawing.Size(59, 13);
            this.lblTransferTo.TabIndex = 524;
            this.lblTransferTo.Text = "TransferTo";
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
            this.cmbTransferTo.Location = new System.Drawing.Point(89, 91);
            this.cmbTransferTo.Name = "cmbTransferTo";
            this.cmbTransferTo.Size = new System.Drawing.Size(182, 21);
            this.cmbTransferTo.TabIndex = 523;
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
            this.cmbFontSize.Location = new System.Drawing.Point(431, 90);
            this.cmbFontSize.Name = "cmbFontSize";
            this.cmbFontSize.Size = new System.Drawing.Size(36, 21);
            this.cmbFontSize.TabIndex = 522;
            this.cmbFontSize.Text = "8";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 69);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 13);
            this.label2.TabIndex = 521;
            this.label2.Text = "Branch";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // cmbBranchName
            // 
            this.cmbBranchName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBranchName.FormattingEnabled = true;
            this.cmbBranchName.Items.AddRange(new object[] {
            "Detail",
            "Summery",
            "SummeryByProduct",
            "Single ",
            "Monthly",
            "TDS"});
            this.cmbBranchName.Location = new System.Drawing.Point(89, 65);
            this.cmbBranchName.Name = "cmbBranchName";
            this.cmbBranchName.Size = new System.Drawing.Size(182, 21);
            this.cmbBranchName.TabIndex = 520;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(9, 25);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(33, 13);
            this.label16.TabIndex = 39;
            this.label16.Text = "Date:";
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::VATClient.Properties.Resources.Green_Footer;
            this.panel1.Controls.Add(this.Download);
            this.panel1.Controls.Add(this.btnPrev);
            this.panel1.Location = new System.Drawing.Point(5, 124);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(472, 40);
            this.panel1.TabIndex = 82;
            // 
            // Download
            // 
            this.Download.Image = global::VATClient.Properties.Resources.Load;
            this.Download.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Download.Location = new System.Drawing.Point(141, 7);
            this.Download.Name = "Download";
            this.Download.Size = new System.Drawing.Size(86, 27);
            this.Download.TabIndex = 110;
            this.Download.Text = "Download";
            this.Download.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Download.UseVisualStyleBackColor = true;
            this.Download.Click += new System.EventHandler(this.Download_Click);
            // 
            // btnPrev
            // 
            this.btnPrev.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnPrev.Image = global::VATClient.Properties.Resources.Print;
            this.btnPrev.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPrev.Location = new System.Drawing.Point(36, 5);
            this.btnPrev.Name = "btnPrev";
            this.btnPrev.Size = new System.Drawing.Size(72, 28);
            this.btnPrev.TabIndex = 42;
            this.btnPrev.Text = "Preview";
            this.btnPrev.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnPrev.UseVisualStyleBackColor = false;
            this.btnPrev.Click += new System.EventHandler(this.btnPrev_Click);
            // 
            // bgwPreview
            // 
            this.bgwPreview.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwPreview_DoWork);
            this.bgwPreview.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwPreview_RunWorkerCompleted);
            // 
            // dtpFromDate
            // 
            this.dtpFromDate.Checked = false;
            this.dtpFromDate.CustomFormat = "dd/MMM/yyyy HH:mm:ss";
            this.dtpFromDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpFromDate.Location = new System.Drawing.Point(91, 26);
            this.dtpFromDate.Name = "dtpFromDate";
            this.dtpFromDate.ShowCheckBox = true;
            this.dtpFromDate.Size = new System.Drawing.Size(172, 20);
            this.dtpFromDate.TabIndex = 183;
            // 
            // dtpToDate
            // 
            this.dtpToDate.Checked = false;
            this.dtpToDate.CustomFormat = "dd/MMM/yyyy HH:mm:ss";
            this.dtpToDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpToDate.Location = new System.Drawing.Point(283, 22);
            this.dtpToDate.Name = "dtpToDate";
            this.dtpToDate.ShowCheckBox = true;
            this.dtpToDate.Size = new System.Drawing.Size(174, 20);
            this.dtpToDate.TabIndex = 529;
            // 
            // FormReportSCBL
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(479, 168);
            this.Controls.Add(this.dtpFromDate);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.grbBankInformation);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormReportSCBL";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Report_for_SCBL";
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
        private System.Windows.Forms.Button btnPrev;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbBranchName;
        private System.Windows.Forms.ComboBox cmbFontSize;
        private System.Windows.Forms.Label lblTransferTo;
        private System.Windows.Forms.ComboBox cmbTransferTo;
        private System.Windows.Forms.Button Download;
        private System.Windows.Forms.CheckBox chbInEnglish;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.ComponentModel.BackgroundWorker bgwPreview;
        private System.Windows.Forms.CheckBox chbToll;
        private System.Windows.Forms.DateTimePicker dtpFromDate;
        private System.Windows.Forms.DateTimePicker dtpToDate;
    }
}