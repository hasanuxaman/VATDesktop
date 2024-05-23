namespace VATClient.ReportPreview
{
    partial class FormRptVAT24
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
            this.btnPrev = new System.Windows.Forms.Button();
            this.grbBankInformation = new System.Windows.Forms.GroupBox();
            this.cmbFontSize = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.chkddb = new System.Windows.Forms.CheckBox();
            this.txtSalesInvoiceNo = new System.Windows.Forms.TextBox();
            this.txtItemNo = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.txtItemName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnSearch = new System.Windows.Forms.Button();
            this.txtDutyDrawBackNo = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.backgroundWorkerPreview = new System.ComponentModel.BackgroundWorker();
            this.grbBankInformation.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnPrev
            // 
            this.btnPrev.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnPrev.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPrev.Location = new System.Drawing.Point(127, 174);
            this.btnPrev.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnPrev.Name = "btnPrev";
            this.btnPrev.Size = new System.Drawing.Size(96, 34);
            this.btnPrev.TabIndex = 83;
            this.btnPrev.Text = "Preview";
            this.btnPrev.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnPrev.UseVisualStyleBackColor = false;
            this.btnPrev.Click += new System.EventHandler(this.btnPrev_Click);
            // 
            // grbBankInformation
            // 
            this.grbBankInformation.Controls.Add(this.cmbFontSize);
            this.grbBankInformation.Controls.Add(this.label3);
            this.grbBankInformation.Controls.Add(this.chkddb);
            this.grbBankInformation.Controls.Add(this.txtSalesInvoiceNo);
            this.grbBankInformation.Controls.Add(this.txtItemNo);
            this.grbBankInformation.Controls.Add(this.button2);
            this.grbBankInformation.Controls.Add(this.txtItemName);
            this.grbBankInformation.Controls.Add(this.label1);
            this.grbBankInformation.Controls.Add(this.btnSearch);
            this.grbBankInformation.Controls.Add(this.txtDutyDrawBackNo);
            this.grbBankInformation.Controls.Add(this.label2);
            this.grbBankInformation.Location = new System.Drawing.Point(11, 2);
            this.grbBankInformation.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.grbBankInformation.Name = "grbBankInformation";
            this.grbBankInformation.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.grbBankInformation.Size = new System.Drawing.Size(484, 166);
            this.grbBankInformation.TabIndex = 84;
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
            this.cmbFontSize.Location = new System.Drawing.Point(16, 133);
            this.cmbFontSize.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmbFontSize.Name = "cmbFontSize";
            this.cmbFontSize.Size = new System.Drawing.Size(47, 24);
            this.cmbFontSize.TabIndex = 82;
            this.cmbFontSize.Text = "8";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 87);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 17);
            this.label3.TabIndex = 47;
            this.label3.Text = "Sale ID";
            // 
            // chkddb
            // 
            this.chkddb.AutoSize = true;
            this.chkddb.Location = new System.Drawing.Point(151, 112);
            this.chkddb.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkddb.Name = "chkddb";
            this.chkddb.Size = new System.Drawing.Size(73, 21);
            this.chkddb.TabIndex = 46;
            this.chkddb.Text = "VAT24";
            this.chkddb.UseVisualStyleBackColor = true;
            this.chkddb.CheckedChanged += new System.EventHandler(this.chkddb_CheckedChanged);
            // 
            // txtSalesInvoiceNo
            // 
            this.txtSalesInvoiceNo.AcceptsTab = true;
            this.txtSalesInvoiceNo.Location = new System.Drawing.Point(151, 84);
            this.txtSalesInvoiceNo.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtSalesInvoiceNo.MaximumSize = new System.Drawing.Size(265, 20);
            this.txtSalesInvoiceNo.MinimumSize = new System.Drawing.Size(199, 20);
            this.txtSalesInvoiceNo.Name = "txtSalesInvoiceNo";
            this.txtSalesInvoiceNo.ReadOnly = true;
            this.txtSalesInvoiceNo.Size = new System.Drawing.Size(265, 22);
            this.txtSalesInvoiceNo.TabIndex = 45;
            // 
            // txtItemNo
            // 
            this.txtItemNo.AcceptsTab = true;
            this.txtItemNo.Location = new System.Drawing.Point(348, 112);
            this.txtItemNo.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtItemNo.MaximumSize = new System.Drawing.Size(265, 20);
            this.txtItemNo.MinimumSize = new System.Drawing.Size(199, 20);
            this.txtItemNo.Name = "txtItemNo";
            this.txtItemNo.ReadOnly = true;
            this.txtItemNo.Size = new System.Drawing.Size(199, 22);
            this.txtItemNo.TabIndex = 44;
            this.txtItemNo.Visible = false;
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.button2.Image = global::VATClient.Properties.Resources.search;
            this.button2.Location = new System.Drawing.Point(431, 53);
            this.button2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(40, 25);
            this.button2.TabIndex = 43;
            this.button2.UseVisualStyleBackColor = false;
            // 
            // txtItemName
            // 
            this.txtItemName.AcceptsTab = true;
            this.txtItemName.Location = new System.Drawing.Point(151, 53);
            this.txtItemName.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtItemName.MaximumSize = new System.Drawing.Size(265, 20);
            this.txtItemName.MinimumSize = new System.Drawing.Size(199, 20);
            this.txtItemName.Name = "txtItemName";
            this.txtItemName.ReadOnly = true;
            this.txtItemName.Size = new System.Drawing.Size(265, 22);
            this.txtItemName.TabIndex = 42;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 59);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 17);
            this.label1.TabIndex = 41;
            this.label1.Text = "Item Name";
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSearch.Image = global::VATClient.Properties.Resources.search;
            this.btnSearch.Location = new System.Drawing.Point(431, 22);
            this.btnSearch.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(40, 25);
            this.btnSearch.TabIndex = 40;
            this.btnSearch.UseVisualStyleBackColor = false;
            // 
            // txtDutyDrawBackNo
            // 
            this.txtDutyDrawBackNo.AcceptsTab = true;
            this.txtDutyDrawBackNo.Location = new System.Drawing.Point(151, 22);
            this.txtDutyDrawBackNo.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtDutyDrawBackNo.MaximumSize = new System.Drawing.Size(265, 20);
            this.txtDutyDrawBackNo.MinimumSize = new System.Drawing.Size(199, 20);
            this.txtDutyDrawBackNo.Name = "txtDutyDrawBackNo";
            this.txtDutyDrawBackNo.ReadOnly = true;
            this.txtDutyDrawBackNo.Size = new System.Drawing.Size(265, 22);
            this.txtDutyDrawBackNo.TabIndex = 35;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 28);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(130, 17);
            this.label2.TabIndex = 34;
            this.label2.Text = "Duty DrawBack No:";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(125, 148);
            this.progressBar1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(316, 15);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 80;
            this.progressBar1.Visible = false;
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnClose.Image = global::VATClient.Properties.Resources.Back;
            this.btnClose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnClose.Location = new System.Drawing.Point(375, 174);
            this.btnClose.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(100, 34);
            this.btnClose.TabIndex = 81;
            this.btnClose.Text = "&Close";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnCancel.Image = global::VATClient.Properties.Resources.Referesh;
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.Location = new System.Drawing.Point(255, 174);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 34);
            this.btnCancel.TabIndex = 80;
            this.btnCancel.Text = "&Refresh";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.button1.Image = global::VATClient.Properties.Resources.Print;
            this.button1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button1.Location = new System.Drawing.Point(13, 174);
            this.button1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(100, 34);
            this.button1.TabIndex = 82;
            this.button1.Text = "VAT 24";
            this.button1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // backgroundWorkerPreview
            // 
            this.backgroundWorkerPreview.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerPreview_DoWork);
            this.backgroundWorkerPreview.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerPreview_RunWorkerCompleted);
            // 
            // FormRptVAT24
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(503, 215);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.btnPrev);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.grbBankInformation);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormRptVAT24";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Report DutyDrawBack";
            this.Load += new System.EventHandler(this.FormRptVAT24_Load);
            this.grbBankInformation.ResumeLayout(false);
            this.grbBankInformation.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnPrev;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.GroupBox grbBankInformation;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button btnSearch;
        public System.Windows.Forms.TextBox txtDutyDrawBackNo;
        private System.Windows.Forms.Label label2;
        private System.ComponentModel.BackgroundWorker backgroundWorkerPreview;
        private System.Windows.Forms.Button button2;
        public System.Windows.Forms.TextBox txtItemName;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.TextBox txtItemNo;
        public System.Windows.Forms.TextBox txtSalesInvoiceNo;
        private System.Windows.Forms.CheckBox chkddb;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmbFontSize;
    }
}