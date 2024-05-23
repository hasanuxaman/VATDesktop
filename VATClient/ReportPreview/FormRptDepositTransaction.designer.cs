namespace VATClient.ReportPages
{
    partial class FormRptDepositTransaction
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnPreview = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.grbBankInformation = new System.Windows.Forms.GroupBox();
            this.txtVendorId = new System.Windows.Forms.TextBox();
            this.btnVendorSearch = new System.Windows.Forms.Button();
            this.txtVendorName = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cmbFontSize = new System.Windows.Forms.ComboBox();
            this.cmbReport = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtTransactionType = new System.Windows.Forms.TextBox();
            this.txtBankID = new System.Windows.Forms.TextBox();
            this.btnBankSearch = new System.Windows.Forms.Button();
            this.cmbPost = new System.Windows.Forms.ComboBox();
            this.progressBar2 = new System.Windows.Forms.ProgressBar();
            this.btnSearch = new System.Windows.Forms.Button();
            this.txtBankName = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.txtDepositNo = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.dtpDepositToDate = new System.Windows.Forms.DateTimePicker();
            this.dtpDepositFromDate = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.backgroundWorkerPreview = new System.ComponentModel.BackgroundWorker();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.cmbDecimal = new System.Windows.Forms.ComboBox();
            this.panel1.SuspendLayout();
            this.grbBankInformation.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::VATClient.Properties.Resources.Green_Footer;
            this.panel1.Controls.Add(this.progressBar1);
            this.panel1.Controls.Add(this.btnClose);
            this.panel1.Controls.Add(this.btnPreview);
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Location = new System.Drawing.Point(-5, 218);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(389, 40);
            this.panel1.TabIndex = 65;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(79, -33);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(250, 22);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 195;
            this.progressBar1.Visible = false;
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnClose.Image = global::VATClient.Properties.Resources.Back;
            this.btnClose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnClose.Location = new System.Drawing.Point(291, 7);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 28);
            this.btnClose.TabIndex = 8;
            this.btnClose.Text = "&Close";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnPreview
            // 
            this.btnPreview.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnPreview.Image = global::VATClient.Properties.Resources.Print;
            this.btnPreview.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPreview.Location = new System.Drawing.Point(17, 7);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new System.Drawing.Size(75, 28);
            this.btnPreview.TabIndex = 123;
            this.btnPreview.Text = "Preview";
            this.btnPreview.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnPreview.UseVisualStyleBackColor = false;
            this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnCancel.Image = global::VATClient.Properties.Resources.Referesh;
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.Location = new System.Drawing.Point(98, 7);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 28);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "&Refresh";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // grbBankInformation
            // 
            this.grbBankInformation.Controls.Add(this.cmbDecimal);
            this.grbBankInformation.Controls.Add(this.label6);
            this.grbBankInformation.Controls.Add(this.label5);
            this.grbBankInformation.Controls.Add(this.txtVendorId);
            this.grbBankInformation.Controls.Add(this.btnVendorSearch);
            this.grbBankInformation.Controls.Add(this.txtVendorName);
            this.grbBankInformation.Controls.Add(this.label4);
            this.grbBankInformation.Controls.Add(this.cmbFontSize);
            this.grbBankInformation.Controls.Add(this.cmbReport);
            this.grbBankInformation.Controls.Add(this.label9);
            this.grbBankInformation.Controls.Add(this.label2);
            this.grbBankInformation.Controls.Add(this.txtTransactionType);
            this.grbBankInformation.Controls.Add(this.txtBankID);
            this.grbBankInformation.Controls.Add(this.btnBankSearch);
            this.grbBankInformation.Controls.Add(this.cmbPost);
            this.grbBankInformation.Controls.Add(this.progressBar2);
            this.grbBankInformation.Controls.Add(this.btnSearch);
            this.grbBankInformation.Controls.Add(this.txtBankName);
            this.grbBankInformation.Controls.Add(this.label16);
            this.grbBankInformation.Controls.Add(this.txtDepositNo);
            this.grbBankInformation.Controls.Add(this.label11);
            this.grbBankInformation.Controls.Add(this.label1);
            this.grbBankInformation.Controls.Add(this.dtpDepositToDate);
            this.grbBankInformation.Controls.Add(this.dtpDepositFromDate);
            this.grbBankInformation.Controls.Add(this.label3);
            this.grbBankInformation.Location = new System.Drawing.Point(13, 1);
            this.grbBankInformation.Name = "grbBankInformation";
            this.grbBankInformation.Size = new System.Drawing.Size(358, 218);
            this.grbBankInformation.TabIndex = 66;
            this.grbBankInformation.TabStop = false;
            this.grbBankInformation.Text = "Reporting Criteria";
            // 
            // txtVendorId
            // 
            this.txtVendorId.Location = new System.Drawing.Point(230, 127);
            this.txtVendorId.Name = "txtVendorId";
            this.txtVendorId.Size = new System.Drawing.Size(25, 21);
            this.txtVendorId.TabIndex = 226;
            this.txtVendorId.Visible = false;
            // 
            // btnVendorSearch
            // 
            this.btnVendorSearch.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnVendorSearch.Image = global::VATClient.Properties.Resources.search;
            this.btnVendorSearch.Location = new System.Drawing.Point(307, 95);
            this.btnVendorSearch.Name = "btnVendorSearch";
            this.btnVendorSearch.Size = new System.Drawing.Size(30, 20);
            this.btnVendorSearch.TabIndex = 225;
            this.btnVendorSearch.UseVisualStyleBackColor = false;
            this.btnVendorSearch.Click += new System.EventHandler(this.btnVendorSearch_Click);
            // 
            // txtVendorName
            // 
            this.txtVendorName.Location = new System.Drawing.Point(104, 94);
            this.txtVendorName.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtVendorName.MinimumSize = new System.Drawing.Size(185, 20);
            this.txtVendorName.Name = "txtVendorName";
            this.txtVendorName.ReadOnly = true;
            this.txtVendorName.Size = new System.Drawing.Size(185, 21);
            this.txtVendorName.TabIndex = 223;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(25, 98);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(71, 13);
            this.label4.TabIndex = 224;
            this.label4.Text = "Vendor Name";
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
            this.cmbFontSize.Location = new System.Drawing.Point(38, 191);
            this.cmbFontSize.Name = "cmbFontSize";
            this.cmbFontSize.Size = new System.Drawing.Size(36, 21);
            this.cmbFontSize.TabIndex = 222;
            this.cmbFontSize.Text = "8";
            // 
            // cmbReport
            // 
            this.cmbReport.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbReport.FormattingEnabled = true;
            this.cmbReport.Items.AddRange(new object[] {
            "Deposit",
            "VDS",
            "VDS-TR6"});
            this.cmbReport.Location = new System.Drawing.Point(106, 143);
            this.cmbReport.Name = "cmbReport";
            this.cmbReport.Size = new System.Drawing.Size(99, 21);
            this.cmbReport.TabIndex = 220;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(26, 146);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(67, 13);
            this.label9.TabIndex = 221;
            this.label9.Text = "Report Type";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(26, 123);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(28, 13);
            this.label2.TabIndex = 219;
            this.label2.Text = "Post";
            // 
            // txtTransactionType
            // 
            this.txtTransactionType.Location = new System.Drawing.Point(263, 126);
            this.txtTransactionType.Name = "txtTransactionType";
            this.txtTransactionType.Size = new System.Drawing.Size(25, 21);
            this.txtTransactionType.TabIndex = 210;
            this.txtTransactionType.Visible = false;
            // 
            // txtBankID
            // 
            this.txtBankID.Location = new System.Drawing.Point(298, 127);
            this.txtBankID.MaximumSize = new System.Drawing.Size(50, 20);
            this.txtBankID.MinimumSize = new System.Drawing.Size(50, 20);
            this.txtBankID.Name = "txtBankID";
            this.txtBankID.ReadOnly = true;
            this.txtBankID.Size = new System.Drawing.Size(50, 21);
            this.txtBankID.TabIndex = 218;
            this.txtBankID.Visible = false;
            // 
            // btnBankSearch
            // 
            this.btnBankSearch.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnBankSearch.Image = global::VATClient.Properties.Resources.search;
            this.btnBankSearch.Location = new System.Drawing.Point(307, 71);
            this.btnBankSearch.Name = "btnBankSearch";
            this.btnBankSearch.Size = new System.Drawing.Size(30, 20);
            this.btnBankSearch.TabIndex = 213;
            this.btnBankSearch.UseVisualStyleBackColor = false;
            this.btnBankSearch.Click += new System.EventHandler(this.btnBankSearch_Click);
            // 
            // cmbPost
            // 
            this.cmbPost.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPost.FormattingEnabled = true;
            this.cmbPost.Items.AddRange(new object[] {
            "Y",
            "N"});
            this.cmbPost.Location = new System.Drawing.Point(105, 120);
            this.cmbPost.Name = "cmbPost";
            this.cmbPost.Size = new System.Drawing.Size(100, 21);
            this.cmbPost.TabIndex = 212;
            // 
            // progressBar2
            // 
            this.progressBar2.Location = new System.Drawing.Point(58, 171);
            this.progressBar2.Name = "progressBar2";
            this.progressBar2.Size = new System.Drawing.Size(292, 21);
            this.progressBar2.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar2.TabIndex = 195;
            this.progressBar2.Visible = false;
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSearch.Image = global::VATClient.Properties.Resources.search;
            this.btnSearch.Location = new System.Drawing.Point(307, 22);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(30, 20);
            this.btnSearch.TabIndex = 67;
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // txtBankName
            // 
            this.txtBankName.Location = new System.Drawing.Point(104, 70);
            this.txtBankName.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtBankName.MinimumSize = new System.Drawing.Size(185, 20);
            this.txtBankName.Name = "txtBankName";
            this.txtBankName.ReadOnly = true;
            this.txtBankName.Size = new System.Drawing.Size(185, 21);
            this.txtBankName.TabIndex = 120;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(26, 74);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(60, 13);
            this.label16.TabIndex = 122;
            this.label16.Text = "Bank Name";
            // 
            // txtDepositNo
            // 
            this.txtDepositNo.Location = new System.Drawing.Point(104, 22);
            this.txtDepositNo.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtDepositNo.MinimumSize = new System.Drawing.Size(185, 20);
            this.txtDepositNo.Name = "txtDepositNo";
            this.txtDepositNo.ReadOnly = true;
            this.txtDepositNo.Size = new System.Drawing.Size(185, 21);
            this.txtDepositNo.TabIndex = 117;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(209, 50);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(17, 13);
            this.label11.TabIndex = 121;
            this.label11.Text = "to";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(26, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 13);
            this.label1.TabIndex = 116;
            this.label1.Text = "Deposit No";
            // 
            // dtpDepositToDate
            // 
            this.dtpDepositToDate.Checked = false;
            this.dtpDepositToDate.CustomFormat = "dd/MMM/yyyy";
            this.dtpDepositToDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpDepositToDate.Location = new System.Drawing.Point(230, 46);
            this.dtpDepositToDate.Name = "dtpDepositToDate";
            this.dtpDepositToDate.ShowCheckBox = true;
            this.dtpDepositToDate.Size = new System.Drawing.Size(108, 21);
            this.dtpDepositToDate.TabIndex = 114;
            this.dtpDepositToDate.Value = new System.DateTime(2015, 3, 20, 13, 6, 0, 0);
            // 
            // dtpDepositFromDate
            // 
            this.dtpDepositFromDate.Checked = false;
            this.dtpDepositFromDate.CustomFormat = "dd/MMM/yyyy";
            this.dtpDepositFromDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpDepositFromDate.Location = new System.Drawing.Point(104, 46);
            this.dtpDepositFromDate.Name = "dtpDepositFromDate";
            this.dtpDepositFromDate.ShowCheckBox = true;
            this.dtpDepositFromDate.Size = new System.Drawing.Size(101, 21);
            this.dtpDepositFromDate.TabIndex = 113;
            this.dtpDepositFromDate.Value = new System.DateTime(2001, 3, 20, 13, 6, 0, 0);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(26, 50);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 13);
            this.label3.TabIndex = 115;
            this.label3.Text = "Deposit Date";
            // 
            // backgroundWorkerPreview
            // 
            this.backgroundWorkerPreview.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerPreview_DoWork);
            this.backgroundWorkerPreview.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerPreview_RunWorkerCompleted);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 194);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(29, 13);
            this.label5.TabIndex = 227;
            this.label5.Text = "Font";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(227, 199);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(43, 13);
            this.label6.TabIndex = 228;
            this.label6.Text = "Decimal";
            // 
            // cmbDecimal
            // 
            this.cmbDecimal.FormattingEnabled = true;
            this.cmbDecimal.Items.AddRange(new object[] {
            "0",
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10"});
            this.cmbDecimal.Location = new System.Drawing.Point(273, 194);
            this.cmbDecimal.Name = "cmbDecimal";
            this.cmbDecimal.Size = new System.Drawing.Size(36, 21);
            this.cmbDecimal.TabIndex = 229;
            this.cmbDecimal.Text = "8";
            // 
            // FormRptDepositTransaction
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(384, 261);
            this.Controls.Add(this.grbBankInformation);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(400, 300);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(400, 260);
            this.Name = "FormRptDepositTransaction";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MIS Report (Deposit)";
            this.Load += new System.EventHandler(this.FormRptDepositTransaction_Load);
            this.panel1.ResumeLayout(false);
            this.grbBankInformation.ResumeLayout(false);
            this.grbBankInformation.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox grbBankInformation;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnPreview;
        public System.Windows.Forms.TextBox txtBankName;
        private System.Windows.Forms.Button btnSearch;
        public System.Windows.Forms.DateTimePicker dtpDepositToDate;
        public System.Windows.Forms.DateTimePicker dtpDepositFromDate;
        public System.Windows.Forms.TextBox txtDepositNo;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.ComponentModel.BackgroundWorker backgroundWorkerPreview;
        private System.Windows.Forms.ProgressBar progressBar2;
        private System.Windows.Forms.ComboBox cmbPost;
        private System.Windows.Forms.Button btnBankSearch;
        public System.Windows.Forms.TextBox txtBankID;
        public System.Windows.Forms.TextBox txtTransactionType;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbReport;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox cmbFontSize;
        public System.Windows.Forms.TextBox txtVendorId;
        private System.Windows.Forms.Button btnVendorSearch;
        public System.Windows.Forms.TextBox txtVendorName;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cmbDecimal;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
    }
}