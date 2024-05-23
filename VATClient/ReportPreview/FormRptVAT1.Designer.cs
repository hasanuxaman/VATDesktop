namespace VATClient.ReportPreview
{
    partial class FormRptVAT1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRptVAT1));
            this.dtpFromDate = new System.Windows.Forms.DateTimePicker();
            this.txtItemNo = new System.Windows.Forms.TextBox();
            this.grbBankInformation = new System.Windows.Forms.GroupBox();
            this.chbInEnglish = new System.Windows.Forms.CheckBox();
            this.cmbFontSize = new System.Windows.Forms.ComboBox();
            this.cmbBranchName = new System.Windows.Forms.ComboBox();
            this.labelBranch = new System.Windows.Forms.Label();
            this.txtVATName = new System.Windows.Forms.TextBox();
            this.label30 = new System.Windows.Forms.Label();
            this.btnSearch = new System.Windows.Forms.Button();
            this.label16 = new System.Windows.Forms.Label();
            this.txtProductName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtBomID = new System.Windows.Forms.TextBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnDownload = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnPreview = new System.Windows.Forms.Button();
            this.bgwVAT1New = new System.ComponentModel.BackgroundWorker();
            this.btnBOMAnnexure = new System.Windows.Forms.Button();
            this.grbBankInformation.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dtpFromDate
            // 
            this.dtpFromDate.CustomFormat = "dd/MMM/yyyy";
            this.dtpFromDate.Enabled = false;
            this.dtpFromDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpFromDate.Location = new System.Drawing.Point(119, 71);
            this.dtpFromDate.Name = "dtpFromDate";
            this.dtpFromDate.ShowCheckBox = true;
            this.dtpFromDate.Size = new System.Drawing.Size(150, 20);
            this.dtpFromDate.TabIndex = 42;
            // 
            // txtItemNo
            // 
            this.txtItemNo.Enabled = false;
            this.txtItemNo.Location = new System.Drawing.Point(339, 111);
            this.txtItemNo.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtItemNo.Name = "txtItemNo";
            this.txtItemNo.Size = new System.Drawing.Size(35, 20);
            this.txtItemNo.TabIndex = 44;
            // 
            // grbBankInformation
            // 
            this.grbBankInformation.Controls.Add(this.chbInEnglish);
            this.grbBankInformation.Controls.Add(this.cmbFontSize);
            this.grbBankInformation.Controls.Add(this.cmbBranchName);
            this.grbBankInformation.Controls.Add(this.txtItemNo);
            this.grbBankInformation.Controls.Add(this.txtBomID);
            this.grbBankInformation.Controls.Add(this.labelBranch);
            this.grbBankInformation.Controls.Add(this.txtVATName);
            this.grbBankInformation.Controls.Add(this.label30);
            this.grbBankInformation.Controls.Add(this.dtpFromDate);
            this.grbBankInformation.Controls.Add(this.btnSearch);
            this.grbBankInformation.Controls.Add(this.label16);
            this.grbBankInformation.Controls.Add(this.txtProductName);
            this.grbBankInformation.Controls.Add(this.label2);
            this.grbBankInformation.Location = new System.Drawing.Point(12, 12);
            this.grbBankInformation.Name = "grbBankInformation";
            this.grbBankInformation.Size = new System.Drawing.Size(405, 164);
            this.grbBankInformation.TabIndex = 88;
            this.grbBankInformation.TabStop = false;
            this.grbBankInformation.Text = "Reporting Criteria";
            // 
            // chbInEnglish
            // 
            this.chbInEnglish.AutoSize = true;
            this.chbInEnglish.Location = new System.Drawing.Point(119, 119);
            this.chbInEnglish.Name = "chbInEnglish";
            this.chbInEnglish.Size = new System.Drawing.Size(59, 17);
            this.chbInEnglish.TabIndex = 528;
            this.chbInEnglish.Text = "Bangla";
            this.chbInEnglish.UseVisualStyleBackColor = true;
            this.chbInEnglish.Click += new System.EventHandler(this.chbInEnglish_Click);
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
            this.cmbFontSize.Location = new System.Drawing.Point(6, 137);
            this.cmbFontSize.Name = "cmbFontSize";
            this.cmbFontSize.Size = new System.Drawing.Size(36, 21);
            this.cmbFontSize.TabIndex = 185;
            this.cmbFontSize.Text = "8";
            // 
            // cmbBranchName
            // 
            this.cmbBranchName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBranchName.FormattingEnabled = true;
            this.cmbBranchName.Location = new System.Drawing.Point(119, 95);
            this.cmbBranchName.Name = "cmbBranchName";
            this.cmbBranchName.Size = new System.Drawing.Size(188, 21);
            this.cmbBranchName.TabIndex = 89;
            // 
            // labelBranch
            // 
            this.labelBranch.AutoSize = true;
            this.labelBranch.Location = new System.Drawing.Point(54, 99);
            this.labelBranch.Name = "labelBranch";
            this.labelBranch.Size = new System.Drawing.Size(41, 13);
            this.labelBranch.TabIndex = 90;
            this.labelBranch.Text = "Branch";
            // 
            // txtVATName
            // 
            this.txtVATName.Enabled = false;
            this.txtVATName.Location = new System.Drawing.Point(119, 19);
            this.txtVATName.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtVATName.MinimumSize = new System.Drawing.Size(150, 20);
            this.txtVATName.Name = "txtVATName";
            this.txtVATName.ReadOnly = true;
            this.txtVATName.Size = new System.Drawing.Size(188, 20);
            this.txtVATName.TabIndex = 183;
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Location = new System.Drawing.Point(32, 17);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(59, 13);
            this.label30.TabIndex = 182;
            this.label30.Text = "VAT Name";
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSearch.Image = global::VATClient.Properties.Resources.search;
            this.btnSearch.Location = new System.Drawing.Point(273, 45);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(30, 20);
            this.btnSearch.TabIndex = 40;
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(65, 74);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(30, 13);
            this.label16.TabIndex = 39;
            this.label16.Text = "Date";
            // 
            // txtProductName
            // 
            this.txtProductName.Enabled = false;
            this.txtProductName.Location = new System.Drawing.Point(119, 45);
            this.txtProductName.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtProductName.MinimumSize = new System.Drawing.Size(150, 20);
            this.txtProductName.Name = "txtProductName";
            this.txtProductName.ReadOnly = true;
            this.txtProductName.Size = new System.Drawing.Size(150, 20);
            this.txtProductName.TabIndex = 35;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(20, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(75, 13);
            this.label2.TabIndex = 34;
            this.label2.Text = "Product Name";
            // 
            // txtBomID
            // 
            this.txtBomID.Enabled = false;
            this.txtBomID.Location = new System.Drawing.Point(339, 137);
            this.txtBomID.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtBomID.Name = "txtBomID";
            this.txtBomID.ReadOnly = true;
            this.txtBomID.Size = new System.Drawing.Size(35, 20);
            this.txtBomID.TabIndex = 184;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(129, 147);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(216, 23);
            this.progressBar1.Step = 5;
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 42;
            this.progressBar1.Visible = false;
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::VATClient.Properties.Resources.Green_Footer;
            this.panel1.Controls.Add(this.btnBOMAnnexure);
            this.panel1.Controls.Add(this.btnDownload);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.btnClose);
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Controls.Add(this.btnPreview);
            this.panel1.Location = new System.Drawing.Point(1, 182);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(425, 40);
            this.panel1.TabIndex = 87;
            // 
            // btnDownload
            // 
            this.btnDownload.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnDownload.Image = global::VATClient.Properties.Resources.Load;
            this.btnDownload.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDownload.Location = new System.Drawing.Point(163, 9);
            this.btnDownload.Name = "btnDownload";
            this.btnDownload.Size = new System.Drawing.Size(79, 28);
            this.btnDownload.TabIndex = 43;
            this.btnDownload.Text = "Download";
            this.btnDownload.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnDownload.UseVisualStyleBackColor = false;
            this.btnDownload.Click += new System.EventHandler(this.btnDownload_Click);
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.button1.Image = global::VATClient.Properties.Resources.Print;
            this.button1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button1.Location = new System.Drawing.Point(3, 9);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 28);
            this.button1.TabIndex = 42;
            this.button1.Text = "Preview";
            this.button1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnClose.Image = global::VATClient.Properties.Resources.Back;
            this.btnClose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnClose.Location = new System.Drawing.Point(331, 9);
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
            this.btnCancel.Location = new System.Drawing.Point(250, 9);
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
            this.btnPreview.Image = global::VATClient.Properties.Resources.Print;
            this.btnPreview.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPreview.Location = new System.Drawing.Point(3, 9);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new System.Drawing.Size(75, 28);
            this.btnPreview.TabIndex = 41;
            this.btnPreview.Text = "Preview";
            this.btnPreview.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnPreview.UseVisualStyleBackColor = false;
            // 
            // bgwVAT1New
            // 
            this.bgwVAT1New.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwVAT1New_DoWork);
            this.bgwVAT1New.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwVAT1New_RunWorkerCompleted);
            // 
            // btnBOMAnnexure
            // 
            this.btnBOMAnnexure.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnBOMAnnexure.Image = ((System.Drawing.Image)(resources.GetObject("btnBOMAnnexure.Image")));
            this.btnBOMAnnexure.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnBOMAnnexure.Location = new System.Drawing.Point(84, 9);
            this.btnBOMAnnexure.Name = "btnBOMAnnexure";
            this.btnBOMAnnexure.Size = new System.Drawing.Size(75, 28);
            this.btnBOMAnnexure.TabIndex = 213;
            this.btnBOMAnnexure.Text = "Annexure";
            this.btnBOMAnnexure.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnBOMAnnexure.UseVisualStyleBackColor = false;
            this.btnBOMAnnexure.Click += new System.EventHandler(this.btnBOMAnnexure_Click);
            // 
            // FormRptVAT1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(427, 223);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.grbBankInformation);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(500, 300);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(400, 230);
            this.Name = "FormRptVAT1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Report (VAT 1)";
            this.Load += new System.EventHandler(this.FormRptVAT1_Load);
            this.grbBankInformation.ResumeLayout(false);
            this.grbBankInformation.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnCancel;
        public System.Windows.Forms.TextBox txtItemNo;
        private System.Windows.Forms.GroupBox grbBankInformation;
        private System.Windows.Forms.Button btnPreview;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Label label16;
        public System.Windows.Forms.TextBox txtProductName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label30;
        public System.Windows.Forms.TextBox txtVATName;
        public System.Windows.Forms.DateTimePicker dtpFromDate;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button button1;
        private System.ComponentModel.BackgroundWorker bgwVAT1New;
        public System.Windows.Forms.TextBox txtBomID;
        private System.Windows.Forms.Button btnDownload;
        private System.Windows.Forms.ComboBox cmbBranchName;
        private System.Windows.Forms.Label labelBranch;
        private System.Windows.Forms.ComboBox cmbFontSize;
        private System.Windows.Forms.CheckBox chbInEnglish;
        private System.Windows.Forms.Button btnBOMAnnexure;
    }
}