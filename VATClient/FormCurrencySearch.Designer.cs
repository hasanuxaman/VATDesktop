namespace VATClient
{
    partial class FormCurrencySearch
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
            this.dgvCurrencies = new System.Windows.Forms.DataGridView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.LRecordCount = new System.Windows.Forms.Label();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.grbProductCategories = new System.Windows.Forms.GroupBox();
            this.label11 = new System.Windows.Forms.Label();
            this.cmbActive = new System.Windows.Forms.ComboBox();
            this.txtCurrencyCode = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtCurrencyCountry = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtCurrencyName = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtVatRateTo = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtVatRateFrom = new System.Windows.Forms.TextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.txtHSCodeNo = new System.Windows.Forms.TextBox();
            this.bgwSearch = new System.ComponentModel.BackgroundWorker();
            this.CurrencyID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CurrencyName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CurrencyCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Country = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.IsActive = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Comments = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCurrencies)).BeginInit();
            this.panel1.SuspendLayout();
            this.grbProductCategories.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.progressBar1);
            this.groupBox1.Controls.Add(this.dgvCurrencies);
            this.groupBox1.Location = new System.Drawing.Point(9, 147);
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
            // dgvCurrencies
            // 
            this.dgvCurrencies.AllowUserToAddRows = false;
            this.dgvCurrencies.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.dgvCurrencies.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvCurrencies.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCurrencies.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.CurrencyID,
            this.CurrencyName,
            this.CurrencyCode,
            this.Country,
            this.IsActive,
            this.Comments});
            this.dgvCurrencies.Location = new System.Drawing.Point(3, 11);
            this.dgvCurrencies.Name = "dgvCurrencies";
            this.dgvCurrencies.RowHeadersVisible = false;
            this.dgvCurrencies.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvCurrencies.Size = new System.Drawing.Size(456, 174);
            this.dgvCurrencies.TabIndex = 15;
            this.dgvCurrencies.DoubleClick += new System.EventHandler(this.dgvCurrencies_DoubleClick);
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::VATClient.Properties.Resources.Green_Footer;
            this.panel1.Controls.Add(this.LRecordCount);
            this.panel1.Controls.Add(this.btnOk);
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Location = new System.Drawing.Point(3, 345);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(482, 40);
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
            this.grbProductCategories.Controls.Add(this.label11);
            this.grbProductCategories.Controls.Add(this.cmbActive);
            this.grbProductCategories.Controls.Add(this.txtCurrencyCode);
            this.grbProductCategories.Controls.Add(this.label9);
            this.grbProductCategories.Controls.Add(this.txtCurrencyCountry);
            this.grbProductCategories.Controls.Add(this.label2);
            this.grbProductCategories.Controls.Add(this.txtCurrencyName);
            this.grbProductCategories.Controls.Add(this.label4);
            this.grbProductCategories.Controls.Add(this.txtVatRateTo);
            this.grbProductCategories.Controls.Add(this.label6);
            this.grbProductCategories.Controls.Add(this.txtVatRateFrom);
            this.grbProductCategories.Controls.Add(this.btnSearch);
            this.grbProductCategories.Controls.Add(this.txtHSCodeNo);
            this.grbProductCategories.Location = new System.Drawing.Point(8, 9);
            this.grbProductCategories.Name = "grbProductCategories";
            this.grbProductCategories.Size = new System.Drawing.Size(461, 138);
            this.grbProductCategories.TabIndex = 187;
            this.grbProductCategories.TabStop = false;
            this.grbProductCategories.Text = "Searching Criteria";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(17, 103);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(37, 13);
            this.label11.TabIndex = 210;
            this.label11.Text = "Active";
            // 
            // cmbActive
            // 
            this.cmbActive.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbActive.FormattingEnabled = true;
            this.cmbActive.Items.AddRange(new object[] {
            "Y",
            "N"});
            this.cmbActive.Location = new System.Drawing.Point(103, 100);
            this.cmbActive.Name = "cmbActive";
            this.cmbActive.Size = new System.Drawing.Size(44, 21);
            this.cmbActive.TabIndex = 209;
            // 
            // txtCurrencyCode
            // 
            this.txtCurrencyCode.Location = new System.Drawing.Point(103, 46);
            this.txtCurrencyCode.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtCurrencyCode.MinimumSize = new System.Drawing.Size(250, 20);
            this.txtCurrencyCode.Name = "txtCurrencyCode";
            this.txtCurrencyCode.Size = new System.Drawing.Size(250, 20);
            this.txtCurrencyCode.TabIndex = 193;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(17, 53);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(32, 13);
            this.label9.TabIndex = 192;
            this.label9.Text = "Code";
            // 
            // txtCurrencyCountry
            // 
            this.txtCurrencyCountry.Location = new System.Drawing.Point(103, 71);
            this.txtCurrencyCountry.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtCurrencyCountry.MinimumSize = new System.Drawing.Size(250, 20);
            this.txtCurrencyCountry.Multiline = true;
            this.txtCurrencyCountry.Name = "txtCurrencyCountry";
            this.txtCurrencyCountry.Size = new System.Drawing.Size(250, 20);
            this.txtCurrencyCountry.TabIndex = 190;
            this.txtCurrencyCountry.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 78);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 13);
            this.label2.TabIndex = 191;
            this.label2.Text = "Country";
            // 
            // txtCurrencyName
            // 
            this.txtCurrencyName.Location = new System.Drawing.Point(103, 21);
            this.txtCurrencyName.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtCurrencyName.MinimumSize = new System.Drawing.Size(250, 20);
            this.txtCurrencyName.Name = "txtCurrencyName";
            this.txtCurrencyName.Size = new System.Drawing.Size(250, 20);
            this.txtCurrencyName.TabIndex = 78;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(17, 28);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 13);
            this.label4.TabIndex = 79;
            this.label4.Text = "Name";
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
            this.btnSearch.Location = new System.Drawing.Point(371, 45);
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
            // CurrencyID
            // 
            this.CurrencyID.HeaderText = "ID";
            this.CurrencyID.Name = "CurrencyID";
            this.CurrencyID.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.CurrencyID.Visible = false;
            this.CurrencyID.Width = 70;
            // 
            // CurrencyName
            // 
            this.CurrencyName.HeaderText = "Name";
            this.CurrencyName.Name = "CurrencyName";
            this.CurrencyName.ReadOnly = true;
            // 
            // CurrencyCode
            // 
            this.CurrencyCode.HeaderText = "Code";
            this.CurrencyCode.Name = "CurrencyCode";
            this.CurrencyCode.ReadOnly = true;
            // 
            // Country
            // 
            this.Country.HeaderText = "Country";
            this.Country.Name = "Country";
            this.Country.ReadOnly = true;
            // 
            // IsActive
            // 
            this.IsActive.HeaderText = "Active Status";
            this.IsActive.Name = "IsActive";
            this.IsActive.ReadOnly = true;
            // 
            // Comments
            // 
            this.Comments.HeaderText = "Comments";
            this.Comments.Name = "Comments";
            this.Comments.ReadOnly = true;
            this.Comments.Visible = false;
            // 
            // FormCurrencySearch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(486, 397);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.grbProductCategories);
            this.Name = "FormCurrencySearch";
            this.Text = "Currency Search";
            this.Load += new System.EventHandler(this.FormCurrencySearch_Load);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvCurrencies)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.grbProductCategories.ResumeLayout(false);
            this.grbProductCategories.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.DataGridView dgvCurrencies;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox grbProductCategories;
        private System.Windows.Forms.TextBox txtVatRateTo;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtVatRateFrom;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.TextBox txtHSCodeNo;
        private System.Windows.Forms.TextBox txtCurrencyName;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtCurrencyCode;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtCurrencyCountry;
        private System.Windows.Forms.Label label2;
        private System.ComponentModel.BackgroundWorker bgwSearch;
        private System.Windows.Forms.Label LRecordCount;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ComboBox cmbActive;
        private System.Windows.Forms.DataGridViewTextBoxColumn CurrencyID;
        private System.Windows.Forms.DataGridViewTextBoxColumn CurrencyName;
        private System.Windows.Forms.DataGridViewTextBoxColumn CurrencyCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn Country;
        private System.Windows.Forms.DataGridViewTextBoxColumn IsActive;
        private System.Windows.Forms.DataGridViewTextBoxColumn Comments;

    }
}