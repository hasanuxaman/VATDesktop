namespace VATClient.ReportPreview
{
    partial class FormRptForm5
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.grbBankInformation = new System.Windows.Forms.GroupBox();
            this.chkMLock = new System.Windows.Forms.CheckBox();
            this.dtpDate = new System.Windows.Forms.DateTimePicker();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnLoad = new System.Windows.Forms.Button();
            this.btnPreview = new System.Windows.Forms.Button();
            this.label16 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.dgvBande5 = new System.Windows.Forms.DataGridView();
            this.LineNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FiscalYear = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MonthName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DemandNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ProductName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Pack = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BanderolInfo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DemandQty = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RefOrderNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ReceiveQty = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MTD = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.UsedQty = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.WastageQty = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ClosingQty = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SaleQty = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Remarks = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.backgroundWorkerLoad = new System.ComponentModel.BackgroundWorker();
            this.backgroundWorkerPreview = new System.ComponentModel.BackgroundWorker();
            this.cmbFontSize = new System.Windows.Forms.ComboBox();
            this.grbBankInformation.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBande5)).BeginInit();
            this.SuspendLayout();
            // 
            // grbBankInformation
            // 
            this.grbBankInformation.Controls.Add(this.cmbFontSize);
            this.grbBankInformation.Controls.Add(this.chkMLock);
            this.grbBankInformation.Controls.Add(this.dtpDate);
            this.grbBankInformation.Controls.Add(this.btnClose);
            this.grbBankInformation.Controls.Add(this.btnLoad);
            this.grbBankInformation.Controls.Add(this.btnPreview);
            this.grbBankInformation.Controls.Add(this.label16);
            this.grbBankInformation.Location = new System.Drawing.Point(12, 12);
            this.grbBankInformation.Name = "grbBankInformation";
            this.grbBankInformation.Size = new System.Drawing.Size(970, 76);
            this.grbBankInformation.TabIndex = 83;
            this.grbBankInformation.TabStop = false;
            this.grbBankInformation.Text = "Reporting Criteria";
            // 
            // chkMLock
            // 
            this.chkMLock.AutoSize = true;
            this.chkMLock.Location = new System.Drawing.Point(731, 28);
            this.chkMLock.Name = "chkMLock";
            this.chkMLock.Size = new System.Drawing.Size(83, 17);
            this.chkMLock.TabIndex = 184;
            this.chkMLock.Text = "Month Lock";
            this.chkMLock.UseVisualStyleBackColor = true;
            this.chkMLock.Visible = false;
            // 
            // dtpDate
            // 
            this.dtpDate.Checked = false;
            this.dtpDate.CustomFormat = "MMMM-yyyy";
            this.dtpDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpDate.Location = new System.Drawing.Point(89, 24);
            this.dtpDate.Name = "dtpDate";
            this.dtpDate.Size = new System.Drawing.Size(146, 20);
            this.dtpDate.TabIndex = 97;
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnClose.Image = global::VATClient.Properties.Resources.Back;
            this.btnClose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnClose.Location = new System.Drawing.Point(532, 21);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 27);
            this.btnClose.TabIndex = 94;
            this.btnClose.Text = "&Close";
            this.btnClose.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnLoad
            // 
            this.btnLoad.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnLoad.Image = global::VATClient.Properties.Resources.Load;
            this.btnLoad.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnLoad.Location = new System.Drawing.Point(321, 20);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(75, 28);
            this.btnLoad.TabIndex = 96;
            this.btnLoad.Text = "Load";
            this.btnLoad.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnLoad.UseVisualStyleBackColor = false;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // btnPreview
            // 
            this.btnPreview.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnPreview.Image = global::VATClient.Properties.Resources.Print;
            this.btnPreview.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPreview.Location = new System.Drawing.Point(402, 20);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new System.Drawing.Size(75, 28);
            this.btnPreview.TabIndex = 95;
            this.btnPreview.Text = "Print";
            this.btnPreview.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnPreview.UseVisualStyleBackColor = false;
            this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(15, 28);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(68, 13);
            this.label16.TabIndex = 39;
            this.label16.Text = "Month Name";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.progressBar1);
            this.groupBox1.Controls.Add(this.dgvBande5);
            this.groupBox1.Location = new System.Drawing.Point(12, 94);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(970, 387);
            this.groupBox1.TabIndex = 84;
            this.groupBox1.TabStop = false;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(340, 182);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(291, 22);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 208;
            this.progressBar1.Visible = false;
            // 
            // dgvBande5
            // 
            this.dgvBande5.AllowUserToAddRows = false;
            this.dgvBande5.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.dgvBande5.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvBande5.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvBande5.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.LineNo,
            this.FiscalYear,
            this.MonthName,
            this.DemandNo,
            this.ProductName,
            this.Pack,
            this.BanderolInfo,
            this.DemandQty,
            this.RefOrderNo,
            this.ReceiveQty,
            this.MTD,
            this.UsedQty,
            this.WastageQty,
            this.ClosingQty,
            this.SaleQty,
            this.Remarks});
            this.dgvBande5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvBande5.Location = new System.Drawing.Point(3, 16);
            this.dgvBande5.Name = "dgvBande5";
            this.dgvBande5.RowHeadersVisible = false;
            this.dgvBande5.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvBande5.Size = new System.Drawing.Size(964, 368);
            this.dgvBande5.TabIndex = 0;
            // 
            // LineNo
            // 
            this.LineNo.HeaderText = "LineNo";
            this.LineNo.Name = "LineNo";
            // 
            // FiscalYear
            // 
            this.FiscalYear.HeaderText = "FiscalYear";
            this.FiscalYear.Name = "FiscalYear";
            // 
            // MonthName
            // 
            this.MonthName.HeaderText = "Month Name";
            this.MonthName.Name = "MonthName";
            // 
            // DemandNo
            // 
            this.DemandNo.HeaderText = "DemandNo";
            this.DemandNo.Name = "DemandNo";
            // 
            // ProductName
            // 
            this.ProductName.HeaderText = "ProductName";
            this.ProductName.Name = "ProductName";
            // 
            // Pack
            // 
            this.Pack.HeaderText = "Pack";
            this.Pack.Name = "Pack";
            // 
            // BanderolInfo
            // 
            this.BanderolInfo.HeaderText = "BanderolInfo";
            this.BanderolInfo.Name = "BanderolInfo";
            // 
            // DemandQty
            // 
            this.DemandQty.HeaderText = "Demand Qty";
            this.DemandQty.Name = "DemandQty";
            // 
            // RefOrderNo
            // 
            this.RefOrderNo.HeaderText = "RefOrderNo";
            this.RefOrderNo.Name = "RefOrderNo";
            // 
            // ReceiveQty
            // 
            this.ReceiveQty.HeaderText = "ReceiveQty";
            this.ReceiveQty.Name = "ReceiveQty";
            // 
            // MTD
            // 
            this.MTD.HeaderText = "MTD";
            this.MTD.Name = "MTD";
            // 
            // UsedQty
            // 
            this.UsedQty.HeaderText = "Used Qty";
            this.UsedQty.Name = "UsedQty";
            // 
            // WastageQty
            // 
            this.WastageQty.HeaderText = "WastageQty";
            this.WastageQty.Name = "WastageQty";
            // 
            // ClosingQty
            // 
            this.ClosingQty.HeaderText = "ClosingQty";
            this.ClosingQty.Name = "ClosingQty";
            // 
            // SaleQty
            // 
            this.SaleQty.HeaderText = "SaleQty";
            this.SaleQty.Name = "SaleQty";
            // 
            // Remarks
            // 
            this.Remarks.HeaderText = "Remarks";
            this.Remarks.Name = "Remarks";
            // 
            // backgroundWorkerLoad
            // 
            this.backgroundWorkerLoad.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerLoad_DoWork);
            this.backgroundWorkerLoad.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerLoad_RunWorkerCompleted);
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
            this.cmbFontSize.Location = new System.Drawing.Point(6, 49);
            this.cmbFontSize.Name = "cmbFontSize";
            this.cmbFontSize.Size = new System.Drawing.Size(36, 21);
            this.cmbFontSize.TabIndex = 185;
            this.cmbFontSize.Text = "8";
            // 
            // FormRptForm5
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.ClientSize = new System.Drawing.Size(994, 502);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.grbBankInformation);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormRptForm5";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FormBanderol5";
            this.Load += new System.EventHandler(this.FormBanderol5_Load);
            this.grbBankInformation.ResumeLayout(false);
            this.grbBankInformation.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvBande5)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grbBankInformation;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Button btnPreview;
        private System.Windows.Forms.DateTimePicker dtpDate;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridView dgvBande5;
        private System.ComponentModel.BackgroundWorker backgroundWorkerLoad;
        private System.ComponentModel.BackgroundWorker backgroundWorkerPreview;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.CheckBox chkMLock;
        private System.Windows.Forms.DataGridViewTextBoxColumn LineNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn FiscalYear;
        private System.Windows.Forms.DataGridViewTextBoxColumn MonthName;
        private System.Windows.Forms.DataGridViewTextBoxColumn DemandNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn ProductName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Pack;
        private System.Windows.Forms.DataGridViewTextBoxColumn BanderolInfo;
        private System.Windows.Forms.DataGridViewTextBoxColumn DemandQty;
        private System.Windows.Forms.DataGridViewTextBoxColumn RefOrderNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn ReceiveQty;
        private System.Windows.Forms.DataGridViewTextBoxColumn MTD;
        private System.Windows.Forms.DataGridViewTextBoxColumn UsedQty;
        private System.Windows.Forms.DataGridViewTextBoxColumn WastageQty;
        private System.Windows.Forms.DataGridViewTextBoxColumn ClosingQty;
        private System.Windows.Forms.DataGridViewTextBoxColumn SaleQty;
        private System.Windows.Forms.DataGridViewTextBoxColumn Remarks;
        private System.Windows.Forms.ComboBox cmbFontSize;
    }
}