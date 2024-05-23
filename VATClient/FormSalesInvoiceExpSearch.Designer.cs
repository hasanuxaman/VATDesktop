namespace VATClient
{
    partial class FormSalesInvoiceExpSearch
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
            this.txtId = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtPINo = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnSearch = new System.Windows.Forms.Button();
            this.dgvSalesInvoiceExp = new System.Windows.Forms.DataGridView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.LRecordCount = new System.Windows.Forms.Label();
            this.btnOk = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.bgwSearch = new System.ComponentModel.BackgroundWorker();
            this.Id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LCDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LCBank = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PINo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PIDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.EXPNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.EXPDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Remarks = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PortFrom = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PortTo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LCNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSalesInvoiceExp)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtId
            // 
            this.txtId.Location = new System.Drawing.Point(45, 19);
            this.txtId.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtId.MinimumSize = new System.Drawing.Size(185, 20);
            this.txtId.Name = "txtId";
            this.txtId.Size = new System.Drawing.Size(185, 20);
            this.txtId.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(21, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "ID:";
            // 
            // txtPINo
            // 
            this.txtPINo.Location = new System.Drawing.Point(292, 19);
            this.txtPINo.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtPINo.MinimumSize = new System.Drawing.Size(185, 20);
            this.txtPINo.Name = "txtPINo";
            this.txtPINo.Size = new System.Drawing.Size(185, 20);
            this.txtPINo.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(236, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "PINo:";
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSearch.Image = global::VATClient.Properties.Resources.search;
            this.btnSearch.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSearch.Location = new System.Drawing.Point(488, 15);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 28);
            this.btnSearch.TabIndex = 7;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // dgvSalesInvoiceExp
            // 
            this.dgvSalesInvoiceExp.AllowUserToAddRows = false;
            this.dgvSalesInvoiceExp.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.dgvSalesInvoiceExp.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvSalesInvoiceExp.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSalesInvoiceExp.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Id,
            this.LCDate,
            this.LCBank,
            this.PINo,
            this.PIDate,
            this.EXPNo,
            this.EXPDate,
            this.Remarks,
            this.PortFrom,
            this.PortTo,
            this.LCNumber});
            this.dgvSalesInvoiceExp.Location = new System.Drawing.Point(3, 76);
            this.dgvSalesInvoiceExp.Name = "dgvSalesInvoiceExp";
            this.dgvSalesInvoiceExp.RowHeadersVisible = false;
            this.dgvSalesInvoiceExp.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvSalesInvoiceExp.Size = new System.Drawing.Size(582, 152);
            this.dgvSalesInvoiceExp.TabIndex = 10;
            this.dgvSalesInvoiceExp.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvHSCode_CellContentClick);
            this.dgvSalesInvoiceExp.DoubleClick += new System.EventHandler(this.dgvHSCode_DoubleClick);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtId);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.btnSearch);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtPINo);
            this.groupBox1.Location = new System.Drawing.Point(3, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(582, 58);
            this.groupBox1.TabIndex = 11;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Searching Criteria";
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::VATClient.Properties.Resources.Green_Footer;
            this.panel1.Controls.Add(this.LRecordCount);
            this.panel1.Controls.Add(this.btnOk);
            this.panel1.Location = new System.Drawing.Point(-51, 228);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(639, 40);
            this.panel1.TabIndex = 12;
            // 
            // LRecordCount
            // 
            this.LRecordCount.AutoSize = true;
            this.LRecordCount.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LRecordCount.Location = new System.Drawing.Point(120, 9);
            this.LRecordCount.Name = "LRecordCount";
            this.LRecordCount.Size = new System.Drawing.Size(94, 16);
            this.LRecordCount.TabIndex = 229;
            this.LRecordCount.Text = "Record Count :";
            // 
            // btnOk
            // 
            this.btnOk.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnOk.Image = global::VATClient.Properties.Resources.Back;
            this.btnOk.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOk.Location = new System.Drawing.Point(690, 6);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 28);
            this.btnOk.TabIndex = 11;
            this.btnOk.Text = "&Ok";
            this.btnOk.UseVisualStyleBackColor = false;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(159, 154);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(288, 24);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 211;
            this.progressBar1.Visible = false;
            // 
            // bgwSearch
            // 
            this.bgwSearch.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwSearch_DoWork);
            this.bgwSearch.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwSearch_RunWorkerCompleted);
            // 
            // Id
            // 
            this.Id.HeaderText = "ID";
            this.Id.Name = "Id";
            this.Id.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Id.Width = 75;
            // 
            // LCDate
            // 
            this.LCDate.HeaderText = "LC Date";
            this.LCDate.Name = "LCDate";
            this.LCDate.Width = 85;
            // 
            // LCBank
            // 
            this.LCBank.HeaderText = "LC Bank";
            this.LCBank.Name = "LCBank";
            this.LCBank.Width = 95;
            // 
            // PINo
            // 
            this.PINo.HeaderText = "PI No";
            this.PINo.Name = "PINo";
            this.PINo.Width = 115;
            // 
            // PIDate
            // 
            this.PIDate.HeaderText = "PI Date";
            this.PIDate.Name = "PIDate";
            this.PIDate.Width = 51;
            // 
            // EXPNo
            // 
            this.EXPNo.HeaderText = "EXP No";
            this.EXPNo.Name = "EXPNo";
            this.EXPNo.Width = 90;
            // 
            // EXPDate
            // 
            this.EXPDate.HeaderText = "EXP Date";
            this.EXPDate.Name = "EXPDate";
            // 
            // Remarks
            // 
            this.Remarks.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.Remarks.HeaderText = "Remarks";
            this.Remarks.Name = "Remarks";
            this.Remarks.Visible = false;
            this.Remarks.Width = 74;
            // 
            // PortFrom
            // 
            this.PortFrom.HeaderText = "PortFrom";
            this.PortFrom.Name = "PortFrom";
            this.PortFrom.ReadOnly = true;
            // 
            // PortTo
            // 
            this.PortTo.HeaderText = "PortTo";
            this.PortTo.Name = "PortTo";
            this.PortTo.ReadOnly = true;
            // 
            // LCNumber
            // 
            this.LCNumber.HeaderText = "LC Number";
            this.LCNumber.Name = "LCNumber";
            this.LCNumber.ReadOnly = true;
            // 
            // FormSalesInvoiceExpSearch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(587, 262);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.dgvSalesInvoiceExp);
            this.Name = "FormSalesInvoiceExpSearch";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SalesInvoiceExpSearch";
            this.Load += new System.EventHandler(this.FormHSCodeSearch_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSalesInvoiceExp)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox txtId;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtPINo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.DataGridView dgvSalesInvoiceExp;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label LRecordCount;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button btnOk;
        private System.ComponentModel.BackgroundWorker bgwSearch;
        private System.Windows.Forms.DataGridViewTextBoxColumn Id;
        private System.Windows.Forms.DataGridViewTextBoxColumn LCDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn LCBank;
        private System.Windows.Forms.DataGridViewTextBoxColumn PINo;
        private System.Windows.Forms.DataGridViewTextBoxColumn PIDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn EXPNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn EXPDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn Remarks;
        private System.Windows.Forms.DataGridViewTextBoxColumn PortFrom;
        private System.Windows.Forms.DataGridViewTextBoxColumn PortTo;
        private System.Windows.Forms.DataGridViewTextBoxColumn LCNumber;
    }
}