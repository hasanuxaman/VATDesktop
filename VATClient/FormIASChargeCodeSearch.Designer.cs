namespace VATClient
{
    partial class FormIASChargeCodeSearch
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.dgvIASChargeCode = new System.Windows.Forms.DataGridView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.LRecordCount = new System.Windows.Forms.Label();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.grbProductCategories = new System.Windows.Forms.GroupBox();
            this.txtChargeCode = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtVatRateTo = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtVatRateFrom = new System.Windows.Forms.TextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.txtHSCodeNo = new System.Windows.Forms.TextBox();
            this.bgwSearch = new System.ComponentModel.BackgroundWorker();
            this.Id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ChargeCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Type = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Description = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvIASChargeCode)).BeginInit();
            this.panel1.SuspendLayout();
            this.grbProductCategories.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.progressBar1);
            this.groupBox1.Controls.Add(this.dgvIASChargeCode);
            this.groupBox1.Location = new System.Drawing.Point(9, 73);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(465, 192);
            this.groupBox1.TabIndex = 188;
            this.groupBox1.TabStop = false;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(93, 40);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(250, 25);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 121;
            this.progressBar1.Visible = false;
            // 
            // dgvIASChargeCode
            // 
            this.dgvIASChargeCode.AllowUserToAddRows = false;
            this.dgvIASChargeCode.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.dgvIASChargeCode.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvIASChargeCode.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvIASChargeCode.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Id,
            this.ChargeCode,
            this.Type,
            this.Description});
            this.dgvIASChargeCode.Location = new System.Drawing.Point(3, 11);
            this.dgvIASChargeCode.Name = "dgvIASChargeCode";
            this.dgvIASChargeCode.RowHeadersVisible = false;
            this.dgvIASChargeCode.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvIASChargeCode.Size = new System.Drawing.Size(456, 174);
            this.dgvIASChargeCode.TabIndex = 15;
            this.dgvIASChargeCode.DoubleClick += new System.EventHandler(this.dgvCurrencies_DoubleClick);
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::VATClient.Properties.Resources.Green_Footer;
            this.panel1.Controls.Add(this.LRecordCount);
            this.panel1.Controls.Add(this.btnOk);
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Location = new System.Drawing.Point(3, 269);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(482, 41);
            this.panel1.TabIndex = 186;
            this.panel1.TabStop = true;
            // 
            // LRecordCount
            // 
            this.LRecordCount.AutoSize = true;
            this.LRecordCount.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LRecordCount.Location = new System.Drawing.Point(96, 13);
            this.LRecordCount.Name = "LRecordCount";
            this.LRecordCount.Size = new System.Drawing.Size(94, 16);
            this.LRecordCount.TabIndex = 226;
            this.LRecordCount.Text = "Record Count :";
            // 
            // btnOk
            // 
            this.btnOk.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnOk.Image = global::VATClient.Properties.Resources.Back;
            this.btnOk.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOk.Location = new System.Drawing.Point(399, 7);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 28);
            this.btnOk.TabIndex = 8;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = false;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnCancel.Image = global::VATClient.Properties.Resources.Referesh;
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.Location = new System.Drawing.Point(15, 7);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 28);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "&Refresh";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // grbProductCategories
            // 
            this.grbProductCategories.Controls.Add(this.txtChargeCode);
            this.grbProductCategories.Controls.Add(this.label9);
            this.grbProductCategories.Controls.Add(this.txtVatRateTo);
            this.grbProductCategories.Controls.Add(this.label6);
            this.grbProductCategories.Controls.Add(this.txtVatRateFrom);
            this.grbProductCategories.Controls.Add(this.btnSearch);
            this.grbProductCategories.Controls.Add(this.txtHSCodeNo);
            this.grbProductCategories.Location = new System.Drawing.Point(8, 9);
            this.grbProductCategories.Name = "grbProductCategories";
            this.grbProductCategories.Size = new System.Drawing.Size(461, 59);
            this.grbProductCategories.TabIndex = 187;
            this.grbProductCategories.TabStop = false;
            this.grbProductCategories.Text = "Searching Criteria";
            // 
            // txtChargeCode
            // 
            this.txtChargeCode.Location = new System.Drawing.Point(103, 19);
            this.txtChargeCode.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtChargeCode.MinimumSize = new System.Drawing.Size(250, 20);
            this.txtChargeCode.Name = "txtChargeCode";
            this.txtChargeCode.Size = new System.Drawing.Size(250, 20);
            this.txtChargeCode.TabIndex = 193;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(17, 23);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(32, 13);
            this.label9.TabIndex = 192;
            this.label9.Text = "Code";
            // 
            // txtVatRateTo
            // 
            this.txtVatRateTo.Location = new System.Drawing.Point(639, 89);
            this.txtVatRateTo.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtVatRateTo.MinimumSize = new System.Drawing.Size(60, 20);
            this.txtVatRateTo.Name = "txtVatRateTo";
            this.txtVatRateTo.Size = new System.Drawing.Size(60, 20);
            this.txtVatRateTo.TabIndex = 6;
            this.txtVatRateTo.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtVatRateTo.Visible = false;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(465, 93);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(57, 13);
            this.label6.TabIndex = 41;
            this.label6.Text = "VAT Rate:";
            this.label6.Visible = false;
            // 
            // txtVatRateFrom
            // 
            this.txtVatRateFrom.Location = new System.Drawing.Point(549, 89);
            this.txtVatRateFrom.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtVatRateFrom.MinimumSize = new System.Drawing.Size(60, 20);
            this.txtVatRateFrom.Name = "txtVatRateFrom";
            this.txtVatRateFrom.Size = new System.Drawing.Size(60, 20);
            this.txtVatRateFrom.TabIndex = 5;
            this.txtVatRateFrom.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtVatRateFrom.Visible = false;
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSearch.Image = global::VATClient.Properties.Resources.search;
            this.btnSearch.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSearch.Location = new System.Drawing.Point(371, 15);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 28);
            this.btnSearch.TabIndex = 5;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // txtHSCodeNo
            // 
            this.txtHSCodeNo.Location = new System.Drawing.Point(476, 37);
            this.txtHSCodeNo.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtHSCodeNo.MinimumSize = new System.Drawing.Size(150, 20);
            this.txtHSCodeNo.Name = "txtHSCodeNo";
            this.txtHSCodeNo.Size = new System.Drawing.Size(150, 20);
            this.txtHSCodeNo.TabIndex = 4;
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
            this.Id.Width = 70;
            // 
            // ChargeCode
            // 
            this.ChargeCode.HeaderText = "Charge Code";
            this.ChargeCode.Name = "ChargeCode";
            this.ChargeCode.ReadOnly = true;
            // 
            // Type
            // 
            this.Type.HeaderText = "Type";
            this.Type.Name = "Type";
            // 
            // Description
            // 
            this.Description.HeaderText = "Description";
            this.Description.Name = "Description";
            this.Description.ReadOnly = true;
            this.Description.Width = 290;
            // 
            // FormIASChargeCodeSearch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(484, 318);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.grbProductCategories);
            this.Name = "FormIASChargeCodeSearch";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "IAS Charge Code SearchSearch";
            this.Load += new System.EventHandler(this.FormCurrencySearch_Load);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvIASChargeCode)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.grbProductCategories.ResumeLayout(false);
            this.grbProductCategories.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.DataGridView dgvIASChargeCode;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox grbProductCategories;
        private System.Windows.Forms.TextBox txtVatRateTo;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtVatRateFrom;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.TextBox txtHSCodeNo;
        private System.Windows.Forms.TextBox txtChargeCode;
        private System.Windows.Forms.Label label9;
        private System.ComponentModel.BackgroundWorker bgwSearch;
        private System.Windows.Forms.Label LRecordCount;
        private System.Windows.Forms.DataGridViewTextBoxColumn Id;
        private System.Windows.Forms.DataGridViewTextBoxColumn ChargeCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn Type;
        private System.Windows.Forms.DataGridViewTextBoxColumn Description;

    }
}