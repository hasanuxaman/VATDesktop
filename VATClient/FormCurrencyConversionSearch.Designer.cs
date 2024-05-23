namespace VATClient
{
    partial class FormCurrencyConversionSearch
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
            this.dgvCurrencyConversion = new System.Windows.Forms.DataGridView();
            this.CurrencyConversionID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.From = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.To = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Rate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ConversionDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Comments = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ActiveStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel1 = new System.Windows.Forms.Panel();
            this.LRecordCount = new System.Windows.Forms.Label();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.grbProductCategories = new System.Windows.Forms.GroupBox();
            this.label11 = new System.Windows.Forms.Label();
            this.cmbActive = new System.Windows.Forms.ComboBox();
            this.txtTo = new System.Windows.Forms.TextBox();
            this.txtFrom = new System.Windows.Forms.TextBox();
            this.dtpConversionDate = new System.Windows.Forms.DateTimePicker();
            this.label7 = new System.Windows.Forms.Label();
            this.txtCurrencyRate = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtVatRateTo = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtVatRateFrom = new System.Windows.Forms.TextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.txtHSCodeNo = new System.Windows.Forms.TextBox();
            this.bgwSearch = new System.ComponentModel.BackgroundWorker();
            this.bgwLoad = new System.ComponentModel.BackgroundWorker();
            this.CurrencyCodeF = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CurrencyCodeT = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCurrencyConversion)).BeginInit();
            this.panel1.SuspendLayout();
            this.grbProductCategories.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.progressBar1);
            this.groupBox1.Controls.Add(this.dgvCurrencyConversion);
            this.groupBox1.Location = new System.Drawing.Point(12, 184);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(465, 192);
            this.groupBox1.TabIndex = 191;
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
            // dgvCurrencyConversion
            // 
            this.dgvCurrencyConversion.AllowUserToAddRows = false;
            this.dgvCurrencyConversion.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.dgvCurrencyConversion.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvCurrencyConversion.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCurrencyConversion.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.CurrencyConversionID,
            this.From,
            this.To,
            this.Rate,
            this.ConversionDate,
            this.Comments,
            this.ActiveStatus,
            this.CurrencyCodeF,
            this.CurrencyCodeT});
            this.dgvCurrencyConversion.Location = new System.Drawing.Point(6, 12);
            this.dgvCurrencyConversion.Name = "dgvCurrencyConversion";
            this.dgvCurrencyConversion.RowHeadersVisible = false;
            this.dgvCurrencyConversion.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvCurrencyConversion.Size = new System.Drawing.Size(456, 174);
            this.dgvCurrencyConversion.TabIndex = 15;
            this.dgvCurrencyConversion.DoubleClick += new System.EventHandler(this.dgvCurrencyConversion_DoubleClick);
            // 
            // CurrencyConversionID
            // 
            this.CurrencyConversionID.HeaderText = "ID";
            this.CurrencyConversionID.Name = "CurrencyConversionID";
            this.CurrencyConversionID.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.CurrencyConversionID.Width = 70;
            // 
            // From
            // 
            this.From.HeaderText = "From";
            this.From.Name = "From";
            this.From.ReadOnly = true;
            this.From.Width = 70;
            // 
            // To
            // 
            this.To.HeaderText = "To";
            this.To.Name = "To";
            this.To.ReadOnly = true;
            this.To.Width = 70;
            // 
            // Rate
            // 
            this.Rate.HeaderText = "Rate";
            this.Rate.Name = "Rate";
            this.Rate.ReadOnly = true;
            this.Rate.Width = 70;
            // 
            // ConversionDate
            // 
            this.ConversionDate.HeaderText = "Conversion Date";
            this.ConversionDate.Name = "ConversionDate";
            this.ConversionDate.Width = 70;
            // 
            // Comments
            // 
            this.Comments.HeaderText = "Comments";
            this.Comments.Name = "Comments";
            this.Comments.ReadOnly = true;
            this.Comments.Visible = false;
            // 
            // ActiveStatus
            // 
            this.ActiveStatus.HeaderText = "Active Status";
            this.ActiveStatus.Name = "ActiveStatus";
            this.ActiveStatus.ReadOnly = true;
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::VATClient.Properties.Resources.Green_Footer;
            this.panel1.Controls.Add(this.LRecordCount);
            this.panel1.Controls.Add(this.btnOk);
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Location = new System.Drawing.Point(13, 382);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(482, 40);
            this.panel1.TabIndex = 189;
            this.panel1.TabStop = true;
            // 
            // LRecordCount
            // 
            this.LRecordCount.AutoSize = true;
            this.LRecordCount.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LRecordCount.Location = new System.Drawing.Point(101, 13);
            this.LRecordCount.Name = "LRecordCount";
            this.LRecordCount.Size = new System.Drawing.Size(94, 16);
            this.LRecordCount.TabIndex = 227;
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
            this.grbProductCategories.Controls.Add(this.label11);
            this.grbProductCategories.Controls.Add(this.cmbActive);
            this.grbProductCategories.Controls.Add(this.txtTo);
            this.grbProductCategories.Controls.Add(this.txtFrom);
            this.grbProductCategories.Controls.Add(this.dtpConversionDate);
            this.grbProductCategories.Controls.Add(this.label7);
            this.grbProductCategories.Controls.Add(this.txtCurrencyRate);
            this.grbProductCategories.Controls.Add(this.label2);
            this.grbProductCategories.Controls.Add(this.label5);
            this.grbProductCategories.Controls.Add(this.label4);
            this.grbProductCategories.Controls.Add(this.txtVatRateTo);
            this.grbProductCategories.Controls.Add(this.label6);
            this.grbProductCategories.Controls.Add(this.txtVatRateFrom);
            this.grbProductCategories.Controls.Add(this.btnSearch);
            this.grbProductCategories.Controls.Add(this.txtHSCodeNo);
            this.grbProductCategories.Location = new System.Drawing.Point(12, 12);
            this.grbProductCategories.Name = "grbProductCategories";
            this.grbProductCategories.Size = new System.Drawing.Size(461, 168);
            this.grbProductCategories.TabIndex = 190;
            this.grbProductCategories.TabStop = false;
            this.grbProductCategories.Text = "Searching Criteria";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(16, 128);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(37, 13);
            this.label11.TabIndex = 208;
            this.label11.Text = "Active";
            // 
            // cmbActive
            // 
            this.cmbActive.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbActive.FormattingEnabled = true;
            this.cmbActive.Items.AddRange(new object[] {
            "Y",
            "N"});
            this.cmbActive.Location = new System.Drawing.Point(105, 125);
            this.cmbActive.Name = "cmbActive";
            this.cmbActive.Size = new System.Drawing.Size(44, 21);
            this.cmbActive.TabIndex = 207;
            // 
            // txtTo
            // 
            this.txtTo.Location = new System.Drawing.Point(105, 47);
            this.txtTo.Name = "txtTo";
            this.txtTo.Size = new System.Drawing.Size(125, 20);
            this.txtTo.TabIndex = 192;
            // 
            // txtFrom
            // 
            this.txtFrom.Location = new System.Drawing.Point(105, 20);
            this.txtFrom.Name = "txtFrom";
            this.txtFrom.Size = new System.Drawing.Size(125, 20);
            this.txtFrom.TabIndex = 206;
            // 
            // dtpConversionDate
            // 
            this.dtpConversionDate.CustomFormat = "dd/MMM/yyyy";
            this.dtpConversionDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpConversionDate.Location = new System.Drawing.Point(105, 99);
            this.dtpConversionDate.Name = "dtpConversionDate";
            this.dtpConversionDate.Size = new System.Drawing.Size(200, 20);
            this.dtpConversionDate.TabIndex = 205;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(16, 102);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(86, 13);
            this.label7.TabIndex = 204;
            this.label7.Text = "Conversion Date";
            // 
            // txtCurrencyRate
            // 
            this.txtCurrencyRate.Location = new System.Drawing.Point(105, 72);
            this.txtCurrencyRate.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtCurrencyRate.MinimumSize = new System.Drawing.Size(200, 20);
            this.txtCurrencyRate.Multiline = true;
            this.txtCurrencyRate.Name = "txtCurrencyRate";
            this.txtCurrencyRate.Size = new System.Drawing.Size(200, 20);
            this.txtCurrencyRate.TabIndex = 202;
            this.txtCurrencyRate.TabStop = false;
            this.txtCurrencyRate.Leave += new System.EventHandler(this.txtCurrencyRate_Leave);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 75);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(30, 13);
            this.label2.TabIndex = 203;
            this.label2.Text = "Rate";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(16, 51);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(20, 13);
            this.label5.TabIndex = 199;
            this.label5.Text = "To";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(16, 22);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(30, 13);
            this.label4.TabIndex = 198;
            this.label4.Text = "From";
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
            this.btnSearch.Location = new System.Drawing.Point(236, 17);
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
            // bgwLoad
            // 
            this.bgwLoad.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwLoad_DoWork);
            this.bgwLoad.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwLoad_RunWorkerCompleted);
            // 
            // CurrencyCodeF
            // 
            this.CurrencyCodeF.HeaderText = "CurrencyCodeFrom";
            this.CurrencyCodeF.Name = "CurrencyCodeF";
            this.CurrencyCodeF.ReadOnly = true;
            // 
            // CurrencyCodeT
            // 
            this.CurrencyCodeT.HeaderText = "CurrencyCodeTo";
            this.CurrencyCodeT.Name = "CurrencyCodeT";
            this.CurrencyCodeT.ReadOnly = true;
            // 
            // FormCurrencyConversionSearch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(500, 429);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.grbProductCategories);
            this.Name = "FormCurrencyConversionSearch";
            this.Text = "Currency Conversion Search";
            this.Load += new System.EventHandler(this.FormCurrencyConversionSearch_Load);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvCurrencyConversion)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.grbProductCategories.ResumeLayout(false);
            this.grbProductCategories.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.DataGridView dgvCurrencyConversion;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox grbProductCategories;
        private System.Windows.Forms.TextBox txtVatRateTo;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtVatRateFrom;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.TextBox txtHSCodeNo;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DateTimePicker dtpConversionDate;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtCurrencyRate;
        private System.Windows.Forms.Label label2;
        private System.ComponentModel.BackgroundWorker bgwSearch;
        private System.ComponentModel.BackgroundWorker bgwLoad;
        private System.Windows.Forms.DataGridViewTextBoxColumn CurrencyConversionID;
        private System.Windows.Forms.DataGridViewTextBoxColumn From;
        private System.Windows.Forms.DataGridViewTextBoxColumn To;
        private System.Windows.Forms.DataGridViewTextBoxColumn Rate;
        private System.Windows.Forms.DataGridViewTextBoxColumn ConversionDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn Comments;
        private System.Windows.Forms.DataGridViewTextBoxColumn ActiveStatus;
        private System.Windows.Forms.TextBox txtTo;
        private System.Windows.Forms.TextBox txtFrom;
        private System.Windows.Forms.Label LRecordCount;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ComboBox cmbActive;
        private System.Windows.Forms.DataGridViewTextBoxColumn CurrencyCodeF;
        private System.Windows.Forms.DataGridViewTextBoxColumn CurrencyCodeT;
    }
}