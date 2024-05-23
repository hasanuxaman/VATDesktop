namespace VATClient.ReportPreview
{
    partial class FormRptVDS12
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
            this.label11 = new System.Windows.Forms.Label();
            this.dtpDepositDateTo = new System.Windows.Forms.DateTimePicker();
            this.dtpDepositDateFrom = new System.Windows.Forms.DateTimePicker();
            this.label16 = new System.Windows.Forms.Label();
            this.txtVendorName = new System.Windows.Forms.TextBox();
            this.VendorName = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnTDSPreveiw = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.btnSend = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnPreview = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.dtpBillDateTo = new System.Windows.Forms.DateTimePicker();
            this.dtpBillDateFrom = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.dtpIssueDateFrom = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.dtpIssueDateTo = new System.Windows.Forms.DateTimePicker();
            this.txtDepositNumber = new System.Windows.Forms.TextBox();
            this.Deposit = new System.Windows.Forms.Label();
            this.txtPurchaseNumber = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtVendorId = new System.Windows.Forms.TextBox();
            this.backgroundWorkerPreview = new System.ComponentModel.BackgroundWorker();
            this.chkPurchaseVDS = new System.Windows.Forms.CheckBox();
            this.cmbFontSize = new System.Windows.Forms.ComboBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.btnSendMail = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(222, 43);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(16, 13);
            this.label11.TabIndex = 217;
            this.label11.Text = "to";
            // 
            // dtpDepositDateTo
            // 
            this.dtpDepositDateTo.Checked = false;
            this.dtpDepositDateTo.CustomFormat = "dd/MMM/yyyy";
            this.dtpDepositDateTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpDepositDateTo.Location = new System.Drawing.Point(257, 40);
            this.dtpDepositDateTo.Name = "dtpDepositDateTo";
            this.dtpDepositDateTo.ShowCheckBox = true;
            this.dtpDepositDateTo.Size = new System.Drawing.Size(108, 20);
            this.dtpDepositDateTo.TabIndex = 216;
            this.dtpDepositDateTo.Value = new System.DateTime(2015, 3, 20, 13, 6, 0, 0);
            // 
            // dtpDepositDateFrom
            // 
            this.dtpDepositDateFrom.Checked = false;
            this.dtpDepositDateFrom.CustomFormat = "dd/MMM/yyyy";
            this.dtpDepositDateFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpDepositDateFrom.Location = new System.Drawing.Point(97, 40);
            this.dtpDepositDateFrom.Name = "dtpDepositDateFrom";
            this.dtpDepositDateFrom.ShowCheckBox = true;
            this.dtpDepositDateFrom.Size = new System.Drawing.Size(117, 20);
            this.dtpDepositDateFrom.TabIndex = 215;
            this.dtpDepositDateFrom.Value = new System.DateTime(2001, 3, 20, 13, 6, 0, 0);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(22, 43);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(43, 13);
            this.label16.TabIndex = 214;
            this.label16.Text = "Deposit";
            // 
            // txtVendorName
            // 
            this.txtVendorName.BackColor = System.Drawing.SystemColors.Control;
            this.txtVendorName.Location = new System.Drawing.Point(98, 12);
            this.txtVendorName.MaximumSize = new System.Drawing.Size(4, 25);
            this.txtVendorName.MinimumSize = new System.Drawing.Size(185, 20);
            this.txtVendorName.Name = "txtVendorName";
            this.txtVendorName.ReadOnly = true;
            this.txtVendorName.Size = new System.Drawing.Size(185, 20);
            this.txtVendorName.TabIndex = 212;
            this.txtVendorName.TabStop = false;
            // 
            // VendorName
            // 
            this.VendorName.AutoSize = true;
            this.VendorName.Location = new System.Drawing.Point(3, 16);
            this.VendorName.Name = "VendorName";
            this.VendorName.Size = new System.Drawing.Size(72, 13);
            this.VendorName.TabIndex = 213;
            this.VendorName.Text = "Vendor Name";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.panel1.Controls.Add(this.btnSendMail);
            this.panel1.Controls.Add(this.btnTDSPreveiw);
            this.panel1.Controls.Add(this.progressBar1);
            this.panel1.Controls.Add(this.btnSend);
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Controls.Add(this.btnPreview);
            this.panel1.Controls.Add(this.dtpDepositDateFrom);
            this.panel1.Controls.Add(this.label16);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.dtpDepositDateTo);
            this.panel1.Controls.Add(this.dtpBillDateTo);
            this.panel1.Controls.Add(this.label11);
            this.panel1.Controls.Add(this.dtpBillDateFrom);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.dtpIssueDateFrom);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.dtpIssueDateTo);
            this.panel1.Location = new System.Drawing.Point(0, 113);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(422, 40);
            this.panel1.TabIndex = 218;
            // 
            // btnTDSPreveiw
            // 
            this.btnTDSPreveiw.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnTDSPreveiw.Image = global::VATClient.Properties.Resources.Print;
            this.btnTDSPreveiw.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnTDSPreveiw.Location = new System.Drawing.Point(10, 6);
            this.btnTDSPreveiw.Name = "btnTDSPreveiw";
            this.btnTDSPreveiw.Size = new System.Drawing.Size(75, 28);
            this.btnTDSPreveiw.TabIndex = 233;
            this.btnTDSPreveiw.Text = "Preveiw";
            this.btnTDSPreveiw.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnTDSPreveiw.UseVisualStyleBackColor = false;
            this.btnTDSPreveiw.Click += new System.EventHandler(this.btnTDSPreveiw_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(95, -51);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(190, 23);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 232;
            this.progressBar1.Visible = false;
            // 
            // btnSend
            // 
            this.btnSend.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSend.Image = global::VATClient.Properties.Resources.Back;
            this.btnSend.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSend.Location = new System.Drawing.Point(333, 7);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(75, 28);
            this.btnSend.TabIndex = 8;
            this.btnSend.Text = "&Close";
            this.btnSend.UseVisualStyleBackColor = false;
            this.btnSend.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnCancel.Image = global::VATClient.Properties.Resources.Referesh;
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.Location = new System.Drawing.Point(250, 6);
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
            this.btnPreview.Location = new System.Drawing.Point(169, 6);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new System.Drawing.Size(75, 28);
            this.btnPreview.TabIndex = 41;
            this.btnPreview.Text = "Preview";
            this.btnPreview.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnPreview.UseVisualStyleBackColor = false;
            this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(222, 95);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(16, 13);
            this.label3.TabIndex = 226;
            this.label3.Text = "to";
            // 
            // dtpBillDateTo
            // 
            this.dtpBillDateTo.Checked = false;
            this.dtpBillDateTo.CustomFormat = "dd/MMM/yyyy";
            this.dtpBillDateTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpBillDateTo.Location = new System.Drawing.Point(257, 92);
            this.dtpBillDateTo.Name = "dtpBillDateTo";
            this.dtpBillDateTo.ShowCheckBox = true;
            this.dtpBillDateTo.Size = new System.Drawing.Size(108, 20);
            this.dtpBillDateTo.TabIndex = 225;
            this.dtpBillDateTo.Value = new System.DateTime(2015, 3, 20, 13, 6, 0, 0);
            // 
            // dtpBillDateFrom
            // 
            this.dtpBillDateFrom.Checked = false;
            this.dtpBillDateFrom.CustomFormat = "dd/MMM/yyyy";
            this.dtpBillDateFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpBillDateFrom.Location = new System.Drawing.Point(97, 92);
            this.dtpBillDateFrom.Name = "dtpBillDateFrom";
            this.dtpBillDateFrom.ShowCheckBox = true;
            this.dtpBillDateFrom.Size = new System.Drawing.Size(117, 20);
            this.dtpBillDateFrom.TabIndex = 224;
            this.dtpBillDateFrom.Value = new System.DateTime(2001, 3, 20, 13, 6, 0, 0);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(22, 69);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 13);
            this.label2.TabIndex = 219;
            this.label2.Text = "Issue";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(22, 95);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(52, 13);
            this.label4.TabIndex = 223;
            this.label4.Text = "Purchase";
            // 
            // dtpIssueDateFrom
            // 
            this.dtpIssueDateFrom.Checked = false;
            this.dtpIssueDateFrom.CustomFormat = "dd/MMM/yyyy";
            this.dtpIssueDateFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpIssueDateFrom.Location = new System.Drawing.Point(97, 66);
            this.dtpIssueDateFrom.Name = "dtpIssueDateFrom";
            this.dtpIssueDateFrom.ShowCheckBox = true;
            this.dtpIssueDateFrom.Size = new System.Drawing.Size(117, 20);
            this.dtpIssueDateFrom.TabIndex = 220;
            this.dtpIssueDateFrom.Value = new System.DateTime(2001, 3, 20, 13, 6, 0, 0);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(222, 69);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(16, 13);
            this.label1.TabIndex = 222;
            this.label1.Text = "to";
            // 
            // dtpIssueDateTo
            // 
            this.dtpIssueDateTo.Checked = false;
            this.dtpIssueDateTo.CustomFormat = "dd/MMM/yyyy";
            this.dtpIssueDateTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpIssueDateTo.Location = new System.Drawing.Point(257, 66);
            this.dtpIssueDateTo.Name = "dtpIssueDateTo";
            this.dtpIssueDateTo.ShowCheckBox = true;
            this.dtpIssueDateTo.Size = new System.Drawing.Size(108, 20);
            this.dtpIssueDateTo.TabIndex = 221;
            this.dtpIssueDateTo.Value = new System.DateTime(2015, 3, 20, 13, 6, 0, 0);
            // 
            // txtDepositNumber
            // 
            this.txtDepositNumber.BackColor = System.Drawing.SystemColors.Control;
            this.txtDepositNumber.Location = new System.Drawing.Point(99, 35);
            this.txtDepositNumber.MaximumSize = new System.Drawing.Size(4, 25);
            this.txtDepositNumber.MinimumSize = new System.Drawing.Size(185, 20);
            this.txtDepositNumber.Name = "txtDepositNumber";
            this.txtDepositNumber.ReadOnly = true;
            this.txtDepositNumber.Size = new System.Drawing.Size(185, 20);
            this.txtDepositNumber.TabIndex = 227;
            this.txtDepositNumber.TabStop = false;
            // 
            // Deposit
            // 
            this.Deposit.AutoSize = true;
            this.Deposit.Location = new System.Drawing.Point(3, 39);
            this.Deposit.Name = "Deposit";
            this.Deposit.Size = new System.Drawing.Size(83, 13);
            this.Deposit.TabIndex = 228;
            this.Deposit.Text = "Deposit Number";
            // 
            // txtPurchaseNumber
            // 
            this.txtPurchaseNumber.BackColor = System.Drawing.SystemColors.Control;
            this.txtPurchaseNumber.Location = new System.Drawing.Point(99, 59);
            this.txtPurchaseNumber.MaximumSize = new System.Drawing.Size(4, 25);
            this.txtPurchaseNumber.MinimumSize = new System.Drawing.Size(185, 20);
            this.txtPurchaseNumber.Name = "txtPurchaseNumber";
            this.txtPurchaseNumber.ReadOnly = true;
            this.txtPurchaseNumber.Size = new System.Drawing.Size(185, 20);
            this.txtPurchaseNumber.TabIndex = 229;
            this.txtPurchaseNumber.TabStop = false;
            this.txtPurchaseNumber.Visible = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 63);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(92, 13);
            this.label5.TabIndex = 230;
            this.label5.Text = "Purchase Number";
            this.label5.Visible = false;
            // 
            // txtVendorId
            // 
            this.txtVendorId.BackColor = System.Drawing.SystemColors.Control;
            this.txtVendorId.Location = new System.Drawing.Point(310, 16);
            this.txtVendorId.MaximumSize = new System.Drawing.Size(4, 25);
            this.txtVendorId.MinimumSize = new System.Drawing.Size(50, 20);
            this.txtVendorId.Name = "txtVendorId";
            this.txtVendorId.ReadOnly = true;
            this.txtVendorId.Size = new System.Drawing.Size(50, 20);
            this.txtVendorId.TabIndex = 231;
            this.txtVendorId.TabStop = false;
            this.txtVendorId.Visible = false;
            // 
            // backgroundWorkerPreview
            // 
            this.backgroundWorkerPreview.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerPreview_DoWork);
            this.backgroundWorkerPreview.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerPreview_RunWorkerCompleted);
            // 
            // chkPurchaseVDS
            // 
            this.chkPurchaseVDS.AutoSize = true;
            this.chkPurchaseVDS.Checked = true;
            this.chkPurchaseVDS.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkPurchaseVDS.Enabled = false;
            this.chkPurchaseVDS.Location = new System.Drawing.Point(295, 59);
            this.chkPurchaseVDS.Name = "chkPurchaseVDS";
            this.chkPurchaseVDS.Size = new System.Drawing.Size(71, 17);
            this.chkPurchaseVDS.TabIndex = 239;
            this.chkPurchaseVDS.TabStop = false;
            this.chkPurchaseVDS.Text = "Purchase";
            this.chkPurchaseVDS.UseVisualStyleBackColor = true;
            this.chkPurchaseVDS.Visible = false;
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
            this.cmbFontSize.Location = new System.Drawing.Point(6, 86);
            this.cmbFontSize.Name = "cmbFontSize";
            this.cmbFontSize.Size = new System.Drawing.Size(36, 21);
            this.cmbFontSize.TabIndex = 234;
            this.cmbFontSize.Text = "8";
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSearch.Image = global::VATClient.Properties.Resources.search;
            this.btnSearch.Location = new System.Drawing.Point(290, 35);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(30, 20);
            this.btnSearch.TabIndex = 234;
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // btnSendMail
            // 
            this.btnSendMail.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSendMail.Image = global::VATClient.Properties.Resources.Print;
            this.btnSendMail.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSendMail.Location = new System.Drawing.Point(88, 7);
            this.btnSendMail.Name = "btnSendMail";
            this.btnSendMail.Size = new System.Drawing.Size(75, 28);
            this.btnSendMail.TabIndex = 234;
            this.btnSendMail.Text = "Send Mail";
            this.btnSendMail.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnSendMail.UseVisualStyleBackColor = false;
            this.btnSendMail.Click += new System.EventHandler(this.btnSendMail_Click);
            // 
            // FormRptVDS12
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(206)))), ((int)(((byte)(206)))), ((int)(((byte)(246)))));
            this.ClientSize = new System.Drawing.Size(413, 159);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.cmbFontSize);
            this.Controls.Add(this.chkPurchaseVDS);
            this.Controls.Add(this.txtVendorId);
            this.Controls.Add(this.txtPurchaseNumber);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtDepositNumber);
            this.Controls.Add(this.Deposit);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.txtVendorName);
            this.Controls.Add(this.VendorName);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(450, 200);
            this.MinimumSize = new System.Drawing.Size(390, 163);
            this.Name = "FormRptVDS12";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "VDS (VAT 6.6)";
            this.Load += new System.EventHandler(this.FormRptVDS_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label11;
        public System.Windows.Forms.DateTimePicker dtpDepositDateTo;
        public System.Windows.Forms.DateTimePicker dtpDepositDateFrom;
        private System.Windows.Forms.Label label16;
        public System.Windows.Forms.TextBox txtVendorName;
        private System.Windows.Forms.Label VendorName;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnPreview;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.DateTimePicker dtpIssueDateTo;
        public System.Windows.Forms.DateTimePicker dtpIssueDateFrom;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        public System.Windows.Forms.DateTimePicker dtpBillDateTo;
        public System.Windows.Forms.DateTimePicker dtpBillDateFrom;
        private System.Windows.Forms.Label label4;
        public System.Windows.Forms.TextBox txtDepositNumber;
        private System.Windows.Forms.Label Deposit;
        public System.Windows.Forms.TextBox txtPurchaseNumber;
        private System.Windows.Forms.Label label5;
        public System.Windows.Forms.TextBox txtVendorId;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.ComponentModel.BackgroundWorker backgroundWorkerPreview;
        public System.Windows.Forms.CheckBox chkPurchaseVDS;
        private System.Windows.Forms.Button btnTDSPreveiw;
        private System.Windows.Forms.ComboBox cmbFontSize;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Button btnSendMail;
    }
}