namespace VATClient.ReportPages
{
    partial class FormRptVendorInformation
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
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.btnSearch = new System.Windows.Forms.Button();
            this.txtVATRegistrationNo = new System.Windows.Forms.TextBox();
            this.txtTINNo = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtVendorName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtVendorID = new System.Windows.Forms.TextBox();
            this.btnPreview = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.txtVGroupId = new System.Windows.Forms.TextBox();
            this.txtVGroup = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.backgroundWorkerPreview = new System.ComponentModel.BackgroundWorker();
            this.grbBankInformation.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // grbBankInformation
            // 
            this.grbBankInformation.Controls.Add(this.cmbFontSize);
            this.grbBankInformation.Controls.Add(this.progressBar1);
            this.grbBankInformation.Controls.Add(this.btnSearch);
            this.grbBankInformation.Controls.Add(this.txtVATRegistrationNo);
            this.grbBankInformation.Controls.Add(this.txtTINNo);
            this.grbBankInformation.Controls.Add(this.label1);
            this.grbBankInformation.Controls.Add(this.label2);
            this.grbBankInformation.Controls.Add(this.txtVendorName);
            this.grbBankInformation.Controls.Add(this.label3);
            this.grbBankInformation.Location = new System.Drawing.Point(2, 12);
            this.grbBankInformation.Name = "grbBankInformation";
            this.grbBankInformation.Size = new System.Drawing.Size(383, 140);
            this.grbBankInformation.TabIndex = 67;
            this.grbBankInformation.TabStop = false;
            this.grbBankInformation.Text = "Reporting Criteria";
            this.grbBankInformation.Enter += new System.EventHandler(this.grbBankInformation_Enter);
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
            this.cmbFontSize.Location = new System.Drawing.Point(6, 113);
            this.cmbFontSize.Name = "cmbFontSize";
            this.cmbFontSize.Size = new System.Drawing.Size(36, 25);
            this.cmbFontSize.TabIndex = 195;
            this.cmbFontSize.Text = "8";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(81, 113);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(226, 23);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar1.TabIndex = 194;
            this.progressBar1.Visible = false;
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSearch.Image = global::VATClient.Properties.Resources.search;
            this.btnSearch.Location = new System.Drawing.Point(317, 18);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(30, 20);
            this.btnSearch.TabIndex = 189;
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // txtVATRegistrationNo
            // 
            this.txtVATRegistrationNo.Location = new System.Drawing.Point(122, 89);
            this.txtVATRegistrationNo.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtVATRegistrationNo.MinimumSize = new System.Drawing.Size(225, 20);
            this.txtVATRegistrationNo.Name = "txtVATRegistrationNo";
            this.txtVATRegistrationNo.ReadOnly = true;
            this.txtVATRegistrationNo.Size = new System.Drawing.Size(225, 20);
            this.txtVATRegistrationNo.TabIndex = 73;
            // 
            // txtTINNo
            // 
            this.txtTINNo.Location = new System.Drawing.Point(122, 66);
            this.txtTINNo.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtTINNo.MinimumSize = new System.Drawing.Size(225, 20);
            this.txtTINNo.Name = "txtTINNo";
            this.txtTINNo.ReadOnly = true;
            this.txtTINNo.Size = new System.Drawing.Size(225, 20);
            this.txtTINNo.TabIndex = 72;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 92);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(134, 17);
            this.label1.TabIndex = 75;
            this.label1.Text = "VAT Registration No:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 69);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 17);
            this.label2.TabIndex = 74;
            this.label2.Text = "TIN:";
            // 
            // txtVendorName
            // 
            this.txtVendorName.Location = new System.Drawing.Point(122, 18);
            this.txtVendorName.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtVendorName.MinimumSize = new System.Drawing.Size(185, 20);
            this.txtVendorName.Name = "txtVendorName";
            this.txtVendorName.ReadOnly = true;
            this.txtVendorName.Size = new System.Drawing.Size(185, 24);
            this.txtVendorName.TabIndex = 70;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 22);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(96, 17);
            this.label3.TabIndex = 71;
            this.label3.Text = "Vendor Name:";
            // 
            // txtVendorID
            // 
            this.txtVendorID.Location = new System.Drawing.Point(365, 33);
            this.txtVendorID.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtVendorID.MinimumSize = new System.Drawing.Size(25, 20);
            this.txtVendorID.Name = "txtVendorID";
            this.txtVendorID.ReadOnly = true;
            this.txtVendorID.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.txtVendorID.Size = new System.Drawing.Size(34, 24);
            this.txtVendorID.TabIndex = 68;
            this.txtVendorID.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtVendorID.Visible = false;
            // 
            // btnPreview
            // 
            this.btnPreview.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnPreview.Image = global::VATClient.Properties.Resources.Print;
            this.btnPreview.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPreview.Location = new System.Drawing.Point(15, 6);
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
            this.panel1.Location = new System.Drawing.Point(2, 158);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(388, 40);
            this.panel1.TabIndex = 66;
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnClose.Image = global::VATClient.Properties.Resources.Back;
            this.btnClose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnClose.Location = new System.Drawing.Point(302, 6);
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
            this.btnCancel.Location = new System.Drawing.Point(96, 6);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 28);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "&Refresh";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.button1.Image = global::VATClient.Properties.Resources.search;
            this.button1.Location = new System.Drawing.Point(318, 54);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(30, 20);
            this.button1.TabIndex = 193;
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // txtVGroupId
            // 
            this.txtVGroupId.Location = new System.Drawing.Point(370, 57);
            this.txtVGroupId.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtVGroupId.MinimumSize = new System.Drawing.Size(25, 20);
            this.txtVGroupId.Name = "txtVGroupId";
            this.txtVGroupId.ReadOnly = true;
            this.txtVGroupId.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.txtVGroupId.Size = new System.Drawing.Size(34, 24);
            this.txtVGroupId.TabIndex = 190;
            this.txtVGroupId.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtVGroupId.Visible = false;
            // 
            // txtVGroup
            // 
            this.txtVGroup.Location = new System.Drawing.Point(124, 54);
            this.txtVGroup.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtVGroup.MinimumSize = new System.Drawing.Size(185, 20);
            this.txtVGroup.Name = "txtVGroup";
            this.txtVGroup.ReadOnly = true;
            this.txtVGroup.Size = new System.Drawing.Size(185, 24);
            this.txtVGroup.TabIndex = 191;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 58);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(99, 17);
            this.label4.TabIndex = 192;
            this.label4.Text = "Vendor Group:";
            // 
            // backgroundWorkerPreview
            // 
            this.backgroundWorkerPreview.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerPreview_DoWork);
            this.backgroundWorkerPreview.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerPreview_RunWorkerCompleted);
            // 
            // FormRptVendorInformation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(382, 183);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.txtVGroupId);
            this.Controls.Add(this.txtVGroup);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.grbBankInformation);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.txtVendorID);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(400, 230);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(400, 230);
            this.Name = "FormRptVendorInformation";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Report (Vendor)";
            this.Load += new System.EventHandler(this.FormRptVendorInformation_Load);
            this.grbBankInformation.ResumeLayout(false);
            this.grbBankInformation.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox grbBankInformation;
        private System.Windows.Forms.Button btnPreview;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        public System.Windows.Forms.TextBox txtVendorID;
        private System.Windows.Forms.Button btnSearch;
        public System.Windows.Forms.TextBox txtVATRegistrationNo;
        public System.Windows.Forms.TextBox txtTINNo;
        public System.Windows.Forms.TextBox txtVendorName;
        private System.Windows.Forms.Button button1;
        public System.Windows.Forms.TextBox txtVGroupId;
        public System.Windows.Forms.TextBox txtVGroup;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.ComponentModel.BackgroundWorker backgroundWorkerPreview;
        private System.Windows.Forms.ComboBox cmbFontSize;
    }
}