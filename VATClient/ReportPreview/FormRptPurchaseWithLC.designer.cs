namespace VATClient.ReportPages
{
    partial class FormRptPurchaseWithLC
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
            this.txtProductTypeId = new System.Windows.Forms.TextBox();
            this.btnVendorGroup = new System.Windows.Forms.Button();
            this.txtVendorGroupID = new System.Windows.Forms.TextBox();
            this.txtVendorGroupName = new System.Windows.Forms.TextBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.label7 = new System.Windows.Forms.Label();
            this.cmbPost = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.btnVendor = new System.Windows.Forms.Button();
            this.btnSearch = new System.Windows.Forms.Button();
            this.txtLCNo = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.txtVendorName = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.dtpLCToDate = new System.Windows.Forms.DateTimePicker();
            this.dtpLCFromDate = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.txtInvoiceNo = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnPreview = new System.Windows.Forms.Button();
            this.txtItemNo = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnMis = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.txtPGroupId = new System.Windows.Forms.TextBox();
            this.backgroundWorkerPreviewDetails = new System.ComponentModel.BackgroundWorker();
            this.backgroundWorkerMIS = new System.ComponentModel.BackgroundWorker();
            this.txtVendorId = new System.Windows.Forms.TextBox();
            this.backgroundWorkerMonth = new System.ComponentModel.BackgroundWorker();
            this.cmbFontSize = new System.Windows.Forms.ComboBox();
            this.grbBankInformation.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // grbBankInformation
            // 
            this.grbBankInformation.Controls.Add(this.cmbFontSize);
            this.grbBankInformation.Controls.Add(this.txtProductTypeId);
            this.grbBankInformation.Controls.Add(this.btnVendorGroup);
            this.grbBankInformation.Controls.Add(this.txtVendorGroupID);
            this.grbBankInformation.Controls.Add(this.txtVendorGroupName);
            this.grbBankInformation.Controls.Add(this.progressBar1);
            this.grbBankInformation.Controls.Add(this.label7);
            this.grbBankInformation.Controls.Add(this.cmbPost);
            this.grbBankInformation.Controls.Add(this.label9);
            this.grbBankInformation.Controls.Add(this.btnVendor);
            this.grbBankInformation.Controls.Add(this.btnSearch);
            this.grbBankInformation.Controls.Add(this.txtLCNo);
            this.grbBankInformation.Controls.Add(this.label12);
            this.grbBankInformation.Controls.Add(this.txtVendorName);
            this.grbBankInformation.Controls.Add(this.label15);
            this.grbBankInformation.Controls.Add(this.label11);
            this.grbBankInformation.Controls.Add(this.dtpLCToDate);
            this.grbBankInformation.Controls.Add(this.dtpLCFromDate);
            this.grbBankInformation.Controls.Add(this.label3);
            this.grbBankInformation.Controls.Add(this.txtInvoiceNo);
            this.grbBankInformation.Controls.Add(this.label1);
            this.grbBankInformation.Location = new System.Drawing.Point(14, 6);
            this.grbBankInformation.Name = "grbBankInformation";
            this.grbBankInformation.Size = new System.Drawing.Size(350, 231);
            this.grbBankInformation.TabIndex = 0;
            this.grbBankInformation.TabStop = false;
            this.grbBankInformation.Text = "Reporting Criteria";
            // 
            // txtProductTypeId
            // 
            this.txtProductTypeId.Location = new System.Drawing.Point(125, 174);
            this.txtProductTypeId.MinimumSize = new System.Drawing.Size(50, 20);
            this.txtProductTypeId.Name = "txtProductTypeId";
            this.txtProductTypeId.ReadOnly = true;
            this.txtProductTypeId.Size = new System.Drawing.Size(50, 21);
            this.txtProductTypeId.TabIndex = 504;
            this.txtProductTypeId.Visible = false;
            // 
            // btnVendorGroup
            // 
            this.btnVendorGroup.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnVendorGroup.Image = global::VATClient.Properties.Resources.search;
            this.btnVendorGroup.Location = new System.Drawing.Point(304, 94);
            this.btnVendorGroup.Name = "btnVendorGroup";
            this.btnVendorGroup.Size = new System.Drawing.Size(30, 20);
            this.btnVendorGroup.TabIndex = 509;
            this.btnVendorGroup.UseVisualStyleBackColor = false;
            this.btnVendorGroup.Click += new System.EventHandler(this.btnVendorGroup_Click);
            // 
            // txtVendorGroupID
            // 
            this.txtVendorGroupID.Location = new System.Drawing.Point(232, 164);
            this.txtVendorGroupID.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtVendorGroupID.MinimumSize = new System.Drawing.Size(125, 20);
            this.txtVendorGroupID.Name = "txtVendorGroupID";
            this.txtVendorGroupID.ReadOnly = true;
            this.txtVendorGroupID.Size = new System.Drawing.Size(125, 20);
            this.txtVendorGroupID.TabIndex = 506;
            this.txtVendorGroupID.TabStop = false;
            this.txtVendorGroupID.Visible = false;
            // 
            // txtVendorGroupName
            // 
            this.txtVendorGroupName.Location = new System.Drawing.Point(105, 94);
            this.txtVendorGroupName.MaximumSize = new System.Drawing.Size(185, 21);
            this.txtVendorGroupName.MinimumSize = new System.Drawing.Size(185, 21);
            this.txtVendorGroupName.Multiline = true;
            this.txtVendorGroupName.Name = "txtVendorGroupName";
            this.txtVendorGroupName.ReadOnly = true;
            this.txtVendorGroupName.Size = new System.Drawing.Size(185, 21);
            this.txtVendorGroupName.TabIndex = 507;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(59, 190);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(254, 25);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 500;
            this.progressBar1.Visible = false;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(19, 98);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(73, 13);
            this.label7.TabIndex = 508;
            this.label7.Text = "Vendor Group";
            // 
            // cmbPost
            // 
            this.cmbPost.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPost.FormattingEnabled = true;
            this.cmbPost.Items.AddRange(new object[] {
            "Y",
            "N"});
            this.cmbPost.Location = new System.Drawing.Point(105, 147);
            this.cmbPost.Name = "cmbPost";
            this.cmbPost.Size = new System.Drawing.Size(184, 21);
            this.cmbPost.TabIndex = 205;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(19, 151);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(28, 13);
            this.label9.TabIndex = 204;
            this.label9.Text = "Post";
            // 
            // btnVendor
            // 
            this.btnVendor.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnVendor.Image = global::VATClient.Properties.Resources.search;
            this.btnVendor.Location = new System.Drawing.Point(304, 119);
            this.btnVendor.Name = "btnVendor";
            this.btnVendor.Size = new System.Drawing.Size(30, 20);
            this.btnVendor.TabIndex = 181;
            this.btnVendor.UseVisualStyleBackColor = false;
            this.btnVendor.Click += new System.EventHandler(this.btnVendor_Click);
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSearch.Image = global::VATClient.Properties.Resources.search;
            this.btnSearch.Location = new System.Drawing.Point(304, 20);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(30, 20);
            this.btnSearch.TabIndex = 180;
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // txtLCNo
            // 
            this.txtLCNo.Location = new System.Drawing.Point(105, 44);
            this.txtLCNo.MaximumSize = new System.Drawing.Size(185, 21);
            this.txtLCNo.MinimumSize = new System.Drawing.Size(185, 21);
            this.txtLCNo.Name = "txtLCNo";
            this.txtLCNo.Size = new System.Drawing.Size(185, 21);
            this.txtLCNo.TabIndex = 177;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(19, 47);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(35, 13);
            this.label12.TabIndex = 179;
            this.label12.Text = "LC No";
            // 
            // txtVendorName
            // 
            this.txtVendorName.Location = new System.Drawing.Point(106, 119);
            this.txtVendorName.MaximumSize = new System.Drawing.Size(185, 21);
            this.txtVendorName.MinimumSize = new System.Drawing.Size(185, 21);
            this.txtVendorName.Name = "txtVendorName";
            this.txtVendorName.ReadOnly = true;
            this.txtVendorName.Size = new System.Drawing.Size(185, 21);
            this.txtVendorName.TabIndex = 131;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(19, 123);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(71, 13);
            this.label15.TabIndex = 132;
            this.label15.Text = "Vendor Name";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(211, 71);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(17, 13);
            this.label11.TabIndex = 136;
            this.label11.Text = "to";
            // 
            // dtpLCToDate
            // 
            this.dtpLCToDate.Checked = false;
            this.dtpLCToDate.CustomFormat = "dd/MMM/yyyy";
            this.dtpLCToDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpLCToDate.Location = new System.Drawing.Point(232, 67);
            this.dtpLCToDate.Name = "dtpLCToDate";
            this.dtpLCToDate.ShowCheckBox = true;
            this.dtpLCToDate.Size = new System.Drawing.Size(101, 21);
            this.dtpLCToDate.TabIndex = 135;
            // 
            // dtpLCFromDate
            // 
            this.dtpLCFromDate.Checked = false;
            this.dtpLCFromDate.CustomFormat = "dd/MMM/yyyy";
            this.dtpLCFromDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpLCFromDate.Location = new System.Drawing.Point(105, 67);
            this.dtpLCFromDate.Name = "dtpLCFromDate";
            this.dtpLCFromDate.ShowCheckBox = true;
            this.dtpLCFromDate.Size = new System.Drawing.Size(101, 21);
            this.dtpLCFromDate.TabIndex = 134;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(18, 71);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(45, 13);
            this.label3.TabIndex = 133;
            this.label3.Text = "LC Date";
            // 
            // txtInvoiceNo
            // 
            this.txtInvoiceNo.Location = new System.Drawing.Point(105, 20);
            this.txtInvoiceNo.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtInvoiceNo.MinimumSize = new System.Drawing.Size(185, 20);
            this.txtInvoiceNo.Name = "txtInvoiceNo";
            this.txtInvoiceNo.ReadOnly = true;
            this.txtInvoiceNo.Size = new System.Drawing.Size(185, 21);
            this.txtInvoiceNo.TabIndex = 129;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(19, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 13);
            this.label1.TabIndex = 130;
            this.label1.Text = "Purchase No";
            // 
            // btnPreview
            // 
            this.btnPreview.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnPreview.Image = global::VATClient.Properties.Resources.Print;
            this.btnPreview.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPreview.Location = new System.Drawing.Point(20, 8);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new System.Drawing.Size(75, 28);
            this.btnPreview.TabIndex = 6;
            this.btnPreview.Text = "Preview";
            this.btnPreview.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnPreview.UseVisualStyleBackColor = false;
            this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
            // 
            // txtItemNo
            // 
            this.txtItemNo.Location = new System.Drawing.Point(463, 119);
            this.txtItemNo.Name = "txtItemNo";
            this.txtItemNo.Size = new System.Drawing.Size(25, 21);
            this.txtItemNo.TabIndex = 176;
            this.txtItemNo.Visible = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(212, 125);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 13);
            this.label2.TabIndex = 178;
            this.label2.Text = "Product No";
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::VATClient.Properties.Resources.Green_Footer;
            this.panel1.Controls.Add(this.btnMis);
            this.panel1.Controls.Add(this.btnClose);
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Controls.Add(this.btnPreview);
            this.panel1.Location = new System.Drawing.Point(0, 244);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(394, 40);
            this.panel1.TabIndex = 75;
            // 
            // btnMis
            // 
            this.btnMis.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnMis.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.btnMis.Location = new System.Drawing.Point(207, 6);
            this.btnMis.Name = "btnMis";
            this.btnMis.Size = new System.Drawing.Size(75, 28);
            this.btnMis.TabIndex = 10;
            this.btnMis.Text = "MIS Report";
            this.btnMis.UseVisualStyleBackColor = false;
            this.btnMis.Visible = false;
            this.btnMis.Click += new System.EventHandler(this.btnMis_Click);
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnClose.Image = global::VATClient.Properties.Resources.Back;
            this.btnClose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnClose.Location = new System.Drawing.Point(301, 8);
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
            this.btnCancel.Location = new System.Drawing.Point(101, 8);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 28);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "&Refresh";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // txtPGroupId
            // 
            this.txtPGroupId.Location = new System.Drawing.Point(463, 142);
            this.txtPGroupId.Name = "txtPGroupId";
            this.txtPGroupId.Size = new System.Drawing.Size(25, 21);
            this.txtPGroupId.TabIndex = 183;
            this.txtPGroupId.Visible = false;
            // 
            // backgroundWorkerPreviewDetails
            // 
            this.backgroundWorkerPreviewDetails.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerPreviewDetails_DoWork);
            this.backgroundWorkerPreviewDetails.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerPreviewDetails_RunWorkerCompleted);
            // 
            // backgroundWorkerMIS
            // 
            this.backgroundWorkerMIS.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerMIS_DoWork);
            this.backgroundWorkerMIS.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerMIS_RunWorkerCompleted);
            // 
            // txtVendorId
            // 
            this.txtVendorId.Location = new System.Drawing.Point(463, 93);
            this.txtVendorId.MinimumSize = new System.Drawing.Size(50, 20);
            this.txtVendorId.Name = "txtVendorId";
            this.txtVendorId.ReadOnly = true;
            this.txtVendorId.Size = new System.Drawing.Size(50, 21);
            this.txtVendorId.TabIndex = 502;
            // 
            // backgroundWorkerMonth
            // 
            this.backgroundWorkerMonth.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerMonth_DoWork);
            this.backgroundWorkerMonth.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerMonth_RunWorkerCompleted);
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
            this.cmbFontSize.Location = new System.Drawing.Point(6, 204);
            this.cmbFontSize.Name = "cmbFontSize";
            this.cmbFontSize.Size = new System.Drawing.Size(36, 21);
            this.cmbFontSize.TabIndex = 503;
            this.cmbFontSize.Text = "8";
            // 
            // FormRptPurchaseWithLC
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(384, 284);
            this.Controls.Add(this.txtVendorId);
            this.Controls.Add(this.txtPGroupId);
            this.Controls.Add(this.grbBankInformation);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.txtItemNo);
            this.Controls.Add(this.label2);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(400, 380);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(400, 200);
            this.Name = "FormRptPurchaseWithLC";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Purchase LC Report";
            this.Load += new System.EventHandler(this.FormRptPurchaseWithLC_Load);
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
        private System.Windows.Forms.TextBox txtVendorName;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.DateTimePicker dtpLCToDate;
        private System.Windows.Forms.DateTimePicker dtpLCFromDate;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtLCNo;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox txtItemNo;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.TextBox txtInvoiceNo;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Button btnVendor;
        private System.Windows.Forms.TextBox txtPGroupId;
        private System.Windows.Forms.ComboBox cmbPost;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.ComponentModel.BackgroundWorker backgroundWorkerPreviewDetails;
        private System.Windows.Forms.Button btnMis;
        private System.ComponentModel.BackgroundWorker backgroundWorkerMIS;
        private System.Windows.Forms.TextBox txtProductTypeId;
        private System.Windows.Forms.TextBox txtVendorId;
        private System.Windows.Forms.Button btnVendorGroup;
        private System.Windows.Forms.TextBox txtVendorGroupName;
        private System.Windows.Forms.Label label7;
        private System.ComponentModel.BackgroundWorker backgroundWorkerMonth;
        private System.Windows.Forms.TextBox txtVendorGroupID;
        private System.Windows.Forms.ComboBox cmbFontSize;
    }
}