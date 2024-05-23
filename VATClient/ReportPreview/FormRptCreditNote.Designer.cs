namespace VATClient.ReportPreview
{
    partial class FormRptCreditNote
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
            this.btnVAT11 = new System.Windows.Forms.Button();
            this.grbBankInformation = new System.Windows.Forms.GroupBox();
            this.txtVehicleID = new System.Windows.Forms.TextBox();
            this.txtCustomerID = new System.Windows.Forms.TextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.txtCustomerName = new System.Windows.Forms.TextBox();
            this.label21 = new System.Windows.Forms.Label();
            this.txtSalesInvoiceNo = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.dtpInvoiceDate = new System.Windows.Forms.DateTimePicker();
            this.label5 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.cmbFontSize = new System.Windows.Forms.ComboBox();
            this.grbBankInformation.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnVAT11
            // 
            this.btnVAT11.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnVAT11.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnVAT11.Location = new System.Drawing.Point(11, 8);
            this.btnVAT11.Name = "btnVAT11";
            this.btnVAT11.Size = new System.Drawing.Size(75, 28);
            this.btnVAT11.TabIndex = 186;
            this.btnVAT11.Text = "Preview";
            this.btnVAT11.UseVisualStyleBackColor = false;
            this.btnVAT11.Click += new System.EventHandler(this.btnVAT11_Click);
            // 
            // grbBankInformation
            // 
            this.grbBankInformation.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.grbBankInformation.Controls.Add(this.cmbFontSize);
            this.grbBankInformation.Controls.Add(this.txtVehicleID);
            this.grbBankInformation.Controls.Add(this.txtCustomerID);
            this.grbBankInformation.Controls.Add(this.btnSearch);
            this.grbBankInformation.Controls.Add(this.txtCustomerName);
            this.grbBankInformation.Controls.Add(this.label21);
            this.grbBankInformation.Controls.Add(this.txtSalesInvoiceNo);
            this.grbBankInformation.Controls.Add(this.label3);
            this.grbBankInformation.Controls.Add(this.dtpInvoiceDate);
            this.grbBankInformation.Controls.Add(this.label5);
            this.grbBankInformation.Location = new System.Drawing.Point(0, 12);
            this.grbBankInformation.Name = "grbBankInformation";
            this.grbBankInformation.Size = new System.Drawing.Size(379, 117);
            this.grbBankInformation.TabIndex = 98;
            this.grbBankInformation.TabStop = false;
            this.grbBankInformation.Text = "Reporting Criteria";
            // 
            // txtVehicleID
            // 
            this.txtVehicleID.Location = new System.Drawing.Point(331, 120);
            this.txtVehicleID.MaximumSize = new System.Drawing.Size(4, 4);
            this.txtVehicleID.MinimumSize = new System.Drawing.Size(50, 20);
            this.txtVehicleID.Name = "txtVehicleID";
            this.txtVehicleID.Size = new System.Drawing.Size(50, 20);
            this.txtVehicleID.TabIndex = 191;
            this.txtVehicleID.Visible = false;
            // 
            // txtCustomerID
            // 
            this.txtCustomerID.Location = new System.Drawing.Point(331, 96);
            this.txtCustomerID.MaximumSize = new System.Drawing.Size(4, 4);
            this.txtCustomerID.MinimumSize = new System.Drawing.Size(50, 20);
            this.txtCustomerID.Name = "txtCustomerID";
            this.txtCustomerID.Size = new System.Drawing.Size(50, 20);
            this.txtCustomerID.TabIndex = 190;
            this.txtCustomerID.Visible = false;
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSearch.Image = global::VATClient.Properties.Resources.search;
            this.btnSearch.Location = new System.Drawing.Point(289, 18);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(30, 20);
            this.btnSearch.TabIndex = 187;
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // txtCustomerName
            // 
            this.txtCustomerName.Location = new System.Drawing.Point(98, 65);
            this.txtCustomerName.MaximumSize = new System.Drawing.Size(4, 4);
            this.txtCustomerName.MinimumSize = new System.Drawing.Size(185, 20);
            this.txtCustomerName.Name = "txtCustomerName";
            this.txtCustomerName.ReadOnly = true;
            this.txtCustomerName.Size = new System.Drawing.Size(185, 20);
            this.txtCustomerName.TabIndex = 156;
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(9, 67);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(82, 13);
            this.label21.TabIndex = 155;
            this.label21.Text = "Customer Name";
            // 
            // txtSalesInvoiceNo
            // 
            this.txtSalesInvoiceNo.BackColor = System.Drawing.SystemColors.Window;
            this.txtSalesInvoiceNo.Location = new System.Drawing.Point(98, 19);
            this.txtSalesInvoiceNo.MaximumSize = new System.Drawing.Size(4, 25);
            this.txtSalesInvoiceNo.MinimumSize = new System.Drawing.Size(185, 20);
            this.txtSalesInvoiceNo.Name = "txtSalesInvoiceNo";
            this.txtSalesInvoiceNo.ReadOnly = true;
            this.txtSalesInvoiceNo.Size = new System.Drawing.Size(185, 20);
            this.txtSalesInvoiceNo.TabIndex = 157;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 23);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 13);
            this.label3.TabIndex = 158;
            this.label3.Text = "Invoice No";
            // 
            // dtpInvoiceDate
            // 
            this.dtpInvoiceDate.CustomFormat = "dd/MMM/yyyy";
            this.dtpInvoiceDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpInvoiceDate.Location = new System.Drawing.Point(98, 42);
            this.dtpInvoiceDate.Name = "dtpInvoiceDate";
            this.dtpInvoiceDate.Size = new System.Drawing.Size(104, 20);
            this.dtpInvoiceDate.TabIndex = 182;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 45);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(59, 13);
            this.label5.TabIndex = 183;
            this.label5.Text = "Sales Date";
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::VATClient.Properties.Resources.Green_Footer;
            this.panel1.Controls.Add(this.btnClose);
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Controls.Add(this.btnVAT11);
            this.panel1.Location = new System.Drawing.Point(0, 135);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(392, 40);
            this.panel1.TabIndex = 97;
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnClose.Image = global::VATClient.Properties.Resources.Back;
            this.btnClose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnClose.Location = new System.Drawing.Point(296, 8);
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
            this.btnCancel.Location = new System.Drawing.Point(90, 8);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 28);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "&Refresh";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(82, 95);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(265, 23);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 99;
            this.progressBar1.Visible = false;
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
            this.cmbFontSize.Location = new System.Drawing.Point(6, 90);
            this.cmbFontSize.Name = "cmbFontSize";
            this.cmbFontSize.Size = new System.Drawing.Size(36, 21);
            this.cmbFontSize.TabIndex = 195;
            this.cmbFontSize.Text = "8";
            // 
            // FormRptCreditNote
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(384, 178);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.grbBankInformation);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(400, 220);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(400, 200);
            this.Name = "FormRptCreditNote";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "VAT 12 (Credit Note)";
            this.Load += new System.EventHandler(this.FormRptCreditNote_Load);
            this.grbBankInformation.ResumeLayout(false);
            this.grbBankInformation.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnVAT11;
        private System.Windows.Forms.GroupBox grbBankInformation;
        private System.Windows.Forms.TextBox txtCustomerName;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.TextBox txtSalesInvoiceNo;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DateTimePicker dtpInvoiceDate;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtVehicleID;
        private System.Windows.Forms.TextBox txtCustomerID;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.ComboBox cmbFontSize;
    }
}