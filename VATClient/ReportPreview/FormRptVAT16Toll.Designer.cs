namespace VATClient.ReportPreview
{
    partial class FormRptVAT16Toll
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
            this.cmbFontSize = new System.Windows.Forms.ComboBox();
            this.btnVAT6_1 = new System.Windows.Forms.Button();
            this.labelBranch = new System.Windows.Forms.Label();
            this.txtTransType = new System.Windows.Forms.TextBox();
            this.rbtnTollRegisterRaw = new System.Windows.Forms.RadioButton();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.label1 = new System.Windows.Forms.Label();
            this.dtpToDate = new System.Windows.Forms.DateTimePicker();
            this.dtpFromDate = new System.Windows.Forms.DateTimePicker();
            this.btnSearch = new System.Windows.Forms.Button();
            this.label16 = new System.Windows.Forms.Label();
            this.txtProductName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtItemNo = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnPrev = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.backgroundWorkerPreview = new System.ComponentModel.BackgroundWorker();
            this.rbtnTollRegisterFinish = new System.Windows.Forms.RadioButton();
            this.cmbBranchName = new System.Windows.Forms.ComboBox();
            this.grbBankInformation.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // grbBankInformation
            // 
            this.grbBankInformation.Controls.Add(this.cmbFontSize);
            this.grbBankInformation.Controls.Add(this.btnVAT6_1);
            this.grbBankInformation.Controls.Add(this.labelBranch);
            this.grbBankInformation.Controls.Add(this.txtTransType);
            this.grbBankInformation.Controls.Add(this.rbtnTollRegisterRaw);
            this.grbBankInformation.Controls.Add(this.progressBar1);
            this.grbBankInformation.Controls.Add(this.label1);
            this.grbBankInformation.Controls.Add(this.dtpToDate);
            this.grbBankInformation.Controls.Add(this.dtpFromDate);
            this.grbBankInformation.Controls.Add(this.btnSearch);
            this.grbBankInformation.Controls.Add(this.label16);
            this.grbBankInformation.Controls.Add(this.txtProductName);
            this.grbBankInformation.Controls.Add(this.label2);
            this.grbBankInformation.Location = new System.Drawing.Point(1, 0);
            this.grbBankInformation.Name = "grbBankInformation";
            this.grbBankInformation.Size = new System.Drawing.Size(363, 136);
            this.grbBankInformation.TabIndex = 79;
            this.grbBankInformation.TabStop = false;
            this.grbBankInformation.Text = "Reporting Criteria";
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
            this.cmbFontSize.Location = new System.Drawing.Point(6, 108);
            this.cmbFontSize.Name = "cmbFontSize";
            this.cmbFontSize.Size = new System.Drawing.Size(36, 21);
            this.cmbFontSize.TabIndex = 84;
            this.cmbFontSize.Text = "8";
            // 
            // btnVAT6_1
            // 
            this.btnVAT6_1.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnVAT6_1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnVAT6_1.Location = new System.Drawing.Point(259, 85);
            this.btnVAT6_1.Name = "btnVAT6_1";
            this.btnVAT6_1.Size = new System.Drawing.Size(72, 28);
            this.btnVAT6_1.TabIndex = 42;
            this.btnVAT6_1.Text = "VAT 6.1";
            this.btnVAT6_1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnVAT6_1.UseVisualStyleBackColor = false;
            this.btnVAT6_1.Visible = false;
            this.btnVAT6_1.Click += new System.EventHandler(this.btnVAT6_1_Click);
            // 
            // labelBranch
            // 
            this.labelBranch.AutoSize = true;
            this.labelBranch.Location = new System.Drawing.Point(9, 90);
            this.labelBranch.Name = "labelBranch";
            this.labelBranch.Size = new System.Drawing.Size(41, 13);
            this.labelBranch.TabIndex = 83;
            this.labelBranch.Text = "Branch";
            this.labelBranch.Click += new System.EventHandler(this.labelBranch_Click);
            // 
            // txtTransType
            // 
            this.txtTransType.Location = new System.Drawing.Point(171, 12);
            this.txtTransType.Name = "txtTransType";
            this.txtTransType.Size = new System.Drawing.Size(100, 20);
            this.txtTransType.TabIndex = 81;
            this.txtTransType.Visible = false;
            // 
            // rbtnTollRegisterRaw
            // 
            this.rbtnTollRegisterRaw.AutoSize = true;
            this.rbtnTollRegisterRaw.Location = new System.Drawing.Point(41, 162);
            this.rbtnTollRegisterRaw.Name = "rbtnTollRegisterRaw";
            this.rbtnTollRegisterRaw.Size = new System.Drawing.Size(103, 17);
            this.rbtnTollRegisterRaw.TabIndex = 80;
            this.rbtnTollRegisterRaw.TabStop = true;
            this.rbtnTollRegisterRaw.Text = "TollRegisterRaw";
            this.rbtnTollRegisterRaw.UseVisualStyleBackColor = true;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(58, 45);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(262, 23);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 80;
            this.progressBar1.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(203, 63);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(20, 13);
            this.label1.TabIndex = 46;
            this.label1.Text = "To";
            // 
            // dtpToDate
            // 
            this.dtpToDate.CustomFormat = "dd/MMM/yyyy";
            this.dtpToDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpToDate.Location = new System.Drawing.Point(228, 59);
            this.dtpToDate.Name = "dtpToDate";
            this.dtpToDate.Size = new System.Drawing.Size(103, 20);
            this.dtpToDate.TabIndex = 43;
            // 
            // dtpFromDate
            // 
            this.dtpFromDate.CustomFormat = "dd/MMM/yyyy";
            this.dtpFromDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpFromDate.Location = new System.Drawing.Point(96, 60);
            this.dtpFromDate.Name = "dtpFromDate";
            this.dtpFromDate.Size = new System.Drawing.Size(104, 20);
            this.dtpFromDate.TabIndex = 42;
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSearch.Image = global::VATClient.Properties.Resources.search;
            this.btnSearch.Location = new System.Drawing.Point(301, 34);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(30, 20);
            this.btnSearch.TabIndex = 40;
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(9, 63);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(33, 13);
            this.label16.TabIndex = 39;
            this.label16.Text = "Date:";
            // 
            // txtProductName
            // 
            this.txtProductName.Location = new System.Drawing.Point(94, 34);
            this.txtProductName.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtProductName.MinimumSize = new System.Drawing.Size(150, 20);
            this.txtProductName.Name = "txtProductName";
            this.txtProductName.ReadOnly = true;
            this.txtProductName.Size = new System.Drawing.Size(200, 20);
            this.txtProductName.TabIndex = 35;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(78, 13);
            this.label2.TabIndex = 34;
            this.label2.Text = "Product Name:";
            // 
            // txtItemNo
            // 
            this.txtItemNo.Location = new System.Drawing.Point(367, 50);
            this.txtItemNo.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtItemNo.MinimumSize = new System.Drawing.Size(50, 20);
            this.txtItemNo.Name = "txtItemNo";
            this.txtItemNo.Size = new System.Drawing.Size(50, 20);
            this.txtItemNo.TabIndex = 44;
            this.txtItemNo.Visible = false;
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.button1.Image = global::VATClient.Properties.Resources.Print;
            this.button1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button1.Location = new System.Drawing.Point(6, 9);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 28);
            this.button1.TabIndex = 41;
            this.button1.Text = "VAT 6.1";
            this.button1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::VATClient.Properties.Resources.Green_Footer;
            this.panel1.Controls.Add(this.btnPrev);
            this.panel1.Controls.Add(this.btnClose);
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Location = new System.Drawing.Point(3, 133);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(361, 40);
            this.panel1.TabIndex = 78;
            // 
            // btnPrev
            // 
            this.btnPrev.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnPrev.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPrev.Location = new System.Drawing.Point(91, 9);
            this.btnPrev.Name = "btnPrev";
            this.btnPrev.Size = new System.Drawing.Size(72, 28);
            this.btnPrev.TabIndex = 42;
            this.btnPrev.Text = "Preview";
            this.btnPrev.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnPrev.UseVisualStyleBackColor = false;
            this.btnPrev.Click += new System.EventHandler(this.btnPrev_Click);
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnClose.Image = global::VATClient.Properties.Resources.Back;
            this.btnClose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnClose.Location = new System.Drawing.Point(277, 9);
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
            this.btnCancel.Location = new System.Drawing.Point(187, 9);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 28);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "&Refresh";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // backgroundWorkerPreview
            // 
            this.backgroundWorkerPreview.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerPreview_DoWork);
            this.backgroundWorkerPreview.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerPreview_RunWorkerCompleted);
            // 
            // rbtnTollRegisterFinish
            // 
            this.rbtnTollRegisterFinish.AutoSize = true;
            this.rbtnTollRegisterFinish.Location = new System.Drawing.Point(151, 173);
            this.rbtnTollRegisterFinish.Name = "rbtnTollRegisterFinish";
            this.rbtnTollRegisterFinish.Size = new System.Drawing.Size(108, 17);
            this.rbtnTollRegisterFinish.TabIndex = 81;
            this.rbtnTollRegisterFinish.TabStop = true;
            this.rbtnTollRegisterFinish.Text = "TollRegisterFinish";
            this.rbtnTollRegisterFinish.UseVisualStyleBackColor = true;
            // 
            // cmbBranchName
            // 
            this.cmbBranchName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBranchName.FormattingEnabled = true;
            this.cmbBranchName.Location = new System.Drawing.Point(96, 86);
            this.cmbBranchName.Name = "cmbBranchName";
            this.cmbBranchName.Size = new System.Drawing.Size(136, 21);
            this.cmbBranchName.TabIndex = 82;
            this.cmbBranchName.SelectedIndexChanged += new System.EventHandler(this.cmbBranchName_SelectedIndexChanged);
            // 
            // FormRptVAT16Toll
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(365, 177);
            this.Controls.Add(this.cmbBranchName);
            this.Controls.Add(this.rbtnTollRegisterFinish);
            this.Controls.Add(this.grbBankInformation);
            this.Controls.Add(this.txtItemNo);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormRptVAT16Toll";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Report (VAT 6.1) Purchase Register";
            this.Load += new System.EventHandler(this.FormRptVAT16_Load);
            this.grbBankInformation.ResumeLayout(false);
            this.grbBankInformation.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox grbBankInformation;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Label label16;
        public System.Windows.Forms.TextBox txtProductName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Panel panel1;
        public System.Windows.Forms.TextBox txtItemNo;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.DateTimePicker dtpFromDate;
        public System.Windows.Forms.DateTimePicker dtpToDate;
        private System.Windows.Forms.Button btnPrev;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.ComponentModel.BackgroundWorker backgroundWorkerPreview;
        public System.Windows.Forms.TextBox txtTransType;
        public System.Windows.Forms.Button button1;
        public System.Windows.Forms.RadioButton rbtnTollRegisterRaw;
        public System.Windows.Forms.RadioButton rbtnTollRegisterFinish;
        private System.Windows.Forms.ComboBox cmbBranchName;
        private System.Windows.Forms.Label labelBranch;
        private System.Windows.Forms.Button btnVAT6_1;
        private System.Windows.Forms.ComboBox cmbFontSize;
    }
}