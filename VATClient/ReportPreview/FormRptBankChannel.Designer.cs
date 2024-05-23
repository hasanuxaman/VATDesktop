namespace VATClient.ReportPreview
{
    partial class FormRptBankChannel
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
            this.dtpPurchaseToDate = new System.Windows.Forms.DateTimePicker();
            this.dtpPurchaseFromDate = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.grbBank = new System.Windows.Forms.GroupBox();
            this.cmbPaymentType = new System.Windows.Forms.ComboBox();
            this.cmbFontSize = new System.Windows.Forms.ComboBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.label11 = new System.Windows.Forms.Label();
            this.cmbIsBankingChannelPay = new System.Windows.Forms.ComboBox();
            this.label12 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnPreview = new System.Windows.Forms.Button();
            this.label24 = new System.Windows.Forms.Label();
            this.chkBankingPay = new System.Windows.Forms.CheckBox();
            this.backgroundWorkerMIS = new System.ComponentModel.BackgroundWorker();
            this.grbBank.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dtpPurchaseToDate
            // 
            this.dtpPurchaseToDate.Checked = false;
            this.dtpPurchaseToDate.CustomFormat = "dd/MMM/yyyy";
            this.dtpPurchaseToDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpPurchaseToDate.Location = new System.Drawing.Point(256, 21);
            this.dtpPurchaseToDate.Name = "dtpPurchaseToDate";
            this.dtpPurchaseToDate.ShowCheckBox = true;
            this.dtpPurchaseToDate.Size = new System.Drawing.Size(121, 20);
            this.dtpPurchaseToDate.TabIndex = 9;
            // 
            // dtpPurchaseFromDate
            // 
            this.dtpPurchaseFromDate.Checked = false;
            this.dtpPurchaseFromDate.CustomFormat = "dd/MMM/yyyy";
            this.dtpPurchaseFromDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpPurchaseFromDate.Location = new System.Drawing.Point(107, 21);
            this.dtpPurchaseFromDate.Name = "dtpPurchaseFromDate";
            this.dtpPurchaseFromDate.ShowCheckBox = true;
            this.dtpPurchaseFromDate.Size = new System.Drawing.Size(121, 20);
            this.dtpPurchaseFromDate.TabIndex = 8;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(23, 25);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(78, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Purchase Date";
            // 
            // grbBank
            // 
            this.grbBank.Controls.Add(this.chkBankingPay);
            this.grbBank.Controls.Add(this.label24);
            this.grbBank.Controls.Add(this.cmbIsBankingChannelPay);
            this.grbBank.Controls.Add(this.label12);
            this.grbBank.Controls.Add(this.dtpPurchaseToDate);
            this.grbBank.Controls.Add(this.dtpPurchaseFromDate);
            this.grbBank.Controls.Add(this.label3);
            this.grbBank.Controls.Add(this.cmbPaymentType);
            this.grbBank.Controls.Add(this.cmbFontSize);
            this.grbBank.Controls.Add(this.progressBar1);
            this.grbBank.Controls.Add(this.label11);
            this.grbBank.Location = new System.Drawing.Point(6, 5);
            this.grbBank.Name = "grbBank";
            this.grbBank.Size = new System.Drawing.Size(458, 171);
            this.grbBank.TabIndex = 80;
            this.grbBank.TabStop = false;
            this.grbBank.Text = "Reporting Criteria";
            // 
            // cmbPaymentType
            // 
            this.cmbPaymentType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPaymentType.FormattingEnabled = true;
            this.cmbPaymentType.Items.AddRange(new object[] {
            "All",
            "Cheque",
            "Online"});
            this.cmbPaymentType.Location = new System.Drawing.Point(107, 75);
            this.cmbPaymentType.Name = "cmbPaymentType";
            this.cmbPaymentType.Size = new System.Drawing.Size(121, 21);
            this.cmbPaymentType.TabIndex = 528;
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
            this.cmbFontSize.Location = new System.Drawing.Point(6, 135);
            this.cmbFontSize.Name = "cmbFontSize";
            this.cmbFontSize.Size = new System.Drawing.Size(36, 21);
            this.cmbFontSize.TabIndex = 526;
            this.cmbFontSize.Text = "8";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(83, 114);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(300, 15);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 208;
            this.progressBar1.Visible = false;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(234, 25);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(16, 13);
            this.label11.TabIndex = 120;
            this.label11.Text = "to";
            // 
            // cmbIsBankingChannelPay
            // 
            this.cmbIsBankingChannelPay.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbIsBankingChannelPay.FormattingEnabled = true;
            this.cmbIsBankingChannelPay.Items.AddRange(new object[] {
            "All",
            "Y",
            "N"});
            this.cmbIsBankingChannelPay.Location = new System.Drawing.Point(107, 47);
            this.cmbIsBankingChannelPay.Name = "cmbIsBankingChannelPay";
            this.cmbIsBankingChannelPay.Size = new System.Drawing.Size(121, 21);
            this.cmbIsBankingChannelPay.TabIndex = 534;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(23, 51);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(46, 13);
            this.label12.TabIndex = 535;
            this.label12.Text = "Banking";
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::VATClient.Properties.Resources.Green_Footer;
            this.panel1.Controls.Add(this.btnClose);
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Controls.Add(this.btnPreview);
            this.panel1.Location = new System.Drawing.Point(-3, 181);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(477, 40);
            this.panel1.TabIndex = 81;
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnClose.Cursor = System.Windows.Forms.Cursors.Hand;
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
            this.btnCancel.Cursor = System.Windows.Forms.Cursors.Hand;
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
            // btnPreview
            // 
            this.btnPreview.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnPreview.Cursor = System.Windows.Forms.Cursors.Hand;
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
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(23, 79);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(75, 13);
            this.label24.TabIndex = 537;
            this.label24.Text = "Payment Type";
            // 
            // chkBankingPay
            // 
            this.chkBankingPay.AutoSize = true;
            this.chkBankingPay.Location = new System.Drawing.Point(255, 51);
            this.chkBankingPay.Name = "chkBankingPay";
            this.chkBankingPay.Size = new System.Drawing.Size(86, 17);
            this.chkBankingPay.TabIndex = 538;
            this.chkBankingPay.Text = "Banking Pay";
            this.chkBankingPay.UseVisualStyleBackColor = true;
            // 
            // backgroundWorkerMIS
            // 
            this.backgroundWorkerMIS.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerMIS_DoWork);
            this.backgroundWorkerMIS.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerMIS_RunWorkerCompleted);
            // 
            // FormRptBankChannel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(470, 220);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.grbBank);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormRptBankChannel";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Bank Channel Payment";
            this.Load += new System.EventHandler(this.FormRptBankChannel_Load);
            this.grbBank.ResumeLayout(false);
            this.grbBank.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DateTimePicker dtpPurchaseToDate;
        private System.Windows.Forms.DateTimePicker dtpPurchaseFromDate;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox grbBank;
        private System.Windows.Forms.ComboBox cmbPaymentType;
        private System.Windows.Forms.ComboBox cmbFontSize;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ComboBox cmbIsBankingChannelPay;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnPreview;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.CheckBox chkBankingPay;
        private System.ComponentModel.BackgroundWorker backgroundWorkerMIS;
    }
}