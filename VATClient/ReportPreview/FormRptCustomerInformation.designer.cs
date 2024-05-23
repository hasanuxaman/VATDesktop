namespace VATClient.ReportPages
{
    partial class FormRptCustomerInformation
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
            this.btnClose = new System.Windows.Forms.Button();
            this.btnPreview = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.grbBankInformation = new System.Windows.Forms.GroupBox();
            this.btnCGSearch = new System.Windows.Forms.Button();
            this.txtCustomerGroup = new System.Windows.Forms.TextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.txtVATRegistrationNo = new System.Windows.Forms.TextBox();
            this.txtTINNo = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.txtCustomerName = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtCustomerID = new System.Windows.Forms.TextBox();
            this.txtCGId = new System.Windows.Forms.TextBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.backgroundWorkerPreview = new System.ComponentModel.BackgroundWorker();
            this.cmbFontSize = new System.Windows.Forms.ComboBox();
            this.panel1.SuspendLayout();
            this.grbBankInformation.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::VATClient.Properties.Resources.Green_Footer;
            this.panel1.Controls.Add(this.btnClose);
            this.panel1.Controls.Add(this.btnPreview);
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Location = new System.Drawing.Point(0, 146);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(491, 40);
            this.panel1.TabIndex = 63;
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnClose.Image = global::VATClient.Properties.Resources.Back;
            this.btnClose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnClose.Location = new System.Drawing.Point(285, 7);
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
            this.btnPreview.Location = new System.Drawing.Point(25, 7);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new System.Drawing.Size(75, 28);
            this.btnPreview.TabIndex = 73;
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
            this.btnCancel.Location = new System.Drawing.Point(106, 7);
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
            this.grbBankInformation.Controls.Add(this.cmbFontSize);
            this.grbBankInformation.Controls.Add(this.btnCGSearch);
            this.grbBankInformation.Controls.Add(this.txtCustomerGroup);
            this.grbBankInformation.Controls.Add(this.btnSearch);
            this.grbBankInformation.Controls.Add(this.txtVATRegistrationNo);
            this.grbBankInformation.Controls.Add(this.txtTINNo);
            this.grbBankInformation.Controls.Add(this.label12);
            this.grbBankInformation.Controls.Add(this.label13);
            this.grbBankInformation.Controls.Add(this.txtCustomerName);
            this.grbBankInformation.Controls.Add(this.label5);
            this.grbBankInformation.Controls.Add(this.label4);
            this.grbBankInformation.Location = new System.Drawing.Point(12, 4);
            this.grbBankInformation.Name = "grbBankInformation";
            this.grbBankInformation.Size = new System.Drawing.Size(343, 139);
            this.grbBankInformation.TabIndex = 64;
            this.grbBankInformation.TabStop = false;
            this.grbBankInformation.Text = "Reporting Criteria";
            // 
            // btnCGSearch
            // 
            this.btnCGSearch.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnCGSearch.Image = global::VATClient.Properties.Resources.search;
            this.btnCGSearch.Location = new System.Drawing.Point(301, 44);
            this.btnCGSearch.Name = "btnCGSearch";
            this.btnCGSearch.Size = new System.Drawing.Size(30, 20);
            this.btnCGSearch.TabIndex = 73;
            this.btnCGSearch.UseVisualStyleBackColor = false;
            this.btnCGSearch.Click += new System.EventHandler(this.btnCGSearch_Click);
            // 
            // txtCustomerGroup
            // 
            this.txtCustomerGroup.Location = new System.Drawing.Point(114, 43);
            this.txtCustomerGroup.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtCustomerGroup.MinimumSize = new System.Drawing.Size(185, 20);
            this.txtCustomerGroup.Name = "txtCustomerGroup";
            this.txtCustomerGroup.ReadOnly = true;
            this.txtCustomerGroup.Size = new System.Drawing.Size(185, 21);
            this.txtCustomerGroup.TabIndex = 74;
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSearch.Image = global::VATClient.Properties.Resources.search;
            this.btnSearch.Location = new System.Drawing.Point(301, 20);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(30, 20);
            this.btnSearch.TabIndex = 65;
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // txtVATRegistrationNo
            // 
            this.txtVATRegistrationNo.Location = new System.Drawing.Point(114, 92);
            this.txtVATRegistrationNo.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtVATRegistrationNo.MinimumSize = new System.Drawing.Size(185, 20);
            this.txtVATRegistrationNo.Name = "txtVATRegistrationNo";
            this.txtVATRegistrationNo.ReadOnly = true;
            this.txtVATRegistrationNo.Size = new System.Drawing.Size(185, 21);
            this.txtVATRegistrationNo.TabIndex = 70;
            // 
            // txtTINNo
            // 
            this.txtTINNo.Location = new System.Drawing.Point(114, 68);
            this.txtTINNo.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtTINNo.MinimumSize = new System.Drawing.Size(185, 20);
            this.txtTINNo.Name = "txtTINNo";
            this.txtTINNo.ReadOnly = true;
            this.txtTINNo.Size = new System.Drawing.Size(185, 21);
            this.txtTINNo.TabIndex = 69;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(9, 96);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(107, 13);
            this.label12.TabIndex = 72;
            this.label12.Text = "VAT Registration No:";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(9, 72);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(28, 13);
            this.label13.TabIndex = 71;
            this.label13.Text = "TIN:";
            // 
            // txtCustomerName
            // 
            this.txtCustomerName.Location = new System.Drawing.Point(114, 19);
            this.txtCustomerName.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtCustomerName.MinimumSize = new System.Drawing.Size(185, 20);
            this.txtCustomerName.Name = "txtCustomerName";
            this.txtCustomerName.ReadOnly = true;
            this.txtCustomerName.Size = new System.Drawing.Size(185, 21);
            this.txtCustomerName.TabIndex = 67;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(10, 46);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(89, 13);
            this.label5.TabIndex = 66;
            this.label5.Text = "Customer Group:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 23);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(87, 13);
            this.label4.TabIndex = 68;
            this.label4.Text = "Customer Name:";
            // 
            // txtCustomerID
            // 
            this.txtCustomerID.Location = new System.Drawing.Point(361, 21);
            this.txtCustomerID.MaximumSize = new System.Drawing.Size(165, 20);
            this.txtCustomerID.MinimumSize = new System.Drawing.Size(25, 20);
            this.txtCustomerID.Name = "txtCustomerID";
            this.txtCustomerID.ReadOnly = true;
            this.txtCustomerID.Size = new System.Drawing.Size(25, 21);
            this.txtCustomerID.TabIndex = 65;
            this.txtCustomerID.Visible = false;
            // 
            // txtCGId
            // 
            this.txtCGId.Location = new System.Drawing.Point(361, 49);
            this.txtCGId.MaximumSize = new System.Drawing.Size(165, 20);
            this.txtCGId.MinimumSize = new System.Drawing.Size(25, 20);
            this.txtCGId.Name = "txtCGId";
            this.txtCGId.ReadOnly = true;
            this.txtCGId.Size = new System.Drawing.Size(25, 21);
            this.txtCGId.TabIndex = 66;
            this.txtCGId.Visible = false;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(74, 118);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(250, 22);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 194;
            this.progressBar1.Visible = false;
            // 
            // backgroundWorkerPreview
            // 
            this.backgroundWorkerPreview.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerPreview_DoWork);
            this.backgroundWorkerPreview.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerPreview_RunWorkerCompleted);
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
            this.cmbFontSize.Location = new System.Drawing.Point(6, 115);
            this.cmbFontSize.Name = "cmbFontSize";
            this.cmbFontSize.Size = new System.Drawing.Size(36, 21);
            this.cmbFontSize.TabIndex = 195;
            this.cmbFontSize.Text = "8";
            // 
            // FormRptCustomerInformation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(384, 187);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.txtCGId);
            this.Controls.Add(this.grbBankInformation);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.txtCustomerID);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(400, 225);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(400, 220);
            this.Name = "FormRptCustomerInformation";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Report (Customer)";
            this.Load += new System.EventHandler(this.FormRptCustomerInformation_Load);
            this.panel1.ResumeLayout(false);
            this.grbBankInformation.ResumeLayout(false);
            this.grbBankInformation.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox grbBankInformation;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnPreview;
        public System.Windows.Forms.TextBox txtCustomerID;
        private System.Windows.Forms.Button btnSearch;
        public System.Windows.Forms.TextBox txtVATRegistrationNo;
        public System.Windows.Forms.TextBox txtTINNo;
        public System.Windows.Forms.TextBox txtCustomerName;
        private System.Windows.Forms.Button btnCGSearch;
        public System.Windows.Forms.TextBox txtCustomerGroup;
        public System.Windows.Forms.TextBox txtCGId;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.ComponentModel.BackgroundWorker backgroundWorkerPreview;
        private System.Windows.Forms.ComboBox cmbFontSize;
    }
}