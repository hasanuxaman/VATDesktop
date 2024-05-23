namespace VATClient
{
    partial class FormTransactionHistorySearch
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle13 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle14 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.dgvTransactionHistory = new System.Windows.Forms.DataGridView();
            this.TransactionNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TransactionType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TransDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Product = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.UOM = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Quantity = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Price = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TradingMarkUp = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SD = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.VATRate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TCost = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CreatedBy = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CreatedOn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LastModifiedBy = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LastModifiedOn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label1 = new System.Windows.Forms.Label();
            this.txtTransactionNo = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.btnSearch = new System.Windows.Forms.Button();
            this.dtpTransFromDate = new System.Windows.Forms.DateTimePicker();
            this.dtpTransToDate = new System.Windows.Forms.DateTimePicker();
            this.label11 = new System.Windows.Forms.Label();
            this.grbTransactionHistory = new System.Windows.Forms.GroupBox();
            this.btnProduct = new System.Windows.Forms.Button();
            this.cmbTransType = new System.Windows.Forms.ComboBox();
            this.txtProduct = new System.Windows.Forms.TextBox();
            this.backgroundWorkerSearch = new System.ComponentModel.BackgroundWorker();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTransactionHistory)).BeginInit();
            this.grbTransactionHistory.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::VATClient.Properties.Resources.Green_Footer;
            this.panel1.Controls.Add(this.btnClose);
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Location = new System.Drawing.Point(-1, 422);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(793, 40);
            this.panel1.TabIndex = 7;
            this.panel1.TabStop = true;
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnClose.Image = global::VATClient.Properties.Resources.Back;
            this.btnClose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnClose.Location = new System.Drawing.Point(698, 7);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 28);
            this.btnClose.TabIndex = 9;
            this.btnClose.Text = "&Close";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnCancel.Image = global::VATClient.Properties.Resources.Referesh;
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.Location = new System.Drawing.Point(17, 7);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 28);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "&Refresh";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.progressBar1);
            this.groupBox1.Location = new System.Drawing.Point(16, 80);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(756, 309);
            this.groupBox1.TabIndex = 106;
            this.groupBox1.TabStop = false;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(227, 83);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(290, 24);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 13;
            this.progressBar1.Visible = false;
            // 
            // dgvTransactionHistory
            // 
            this.dgvTransactionHistory.AllowUserToAddRows = false;
            this.dgvTransactionHistory.AllowUserToDeleteRows = false;
            dataGridViewCellStyle8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.dgvTransactionHistory.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle8;
            this.dgvTransactionHistory.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTransactionHistory.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.TransactionNo,
            this.TransactionType,
            this.TransDate,
            this.Product,
            this.UOM,
            this.Quantity,
            this.Price,
            this.TradingMarkUp,
            this.SD,
            this.VATRate,
            this.TCost,
            this.CreatedBy,
            this.CreatedOn,
            this.LastModifiedBy,
            this.LastModifiedOn});
            this.dgvTransactionHistory.Location = new System.Drawing.Point(22, 90);
            this.dgvTransactionHistory.Name = "dgvTransactionHistory";
            this.dgvTransactionHistory.RowHeadersVisible = false;
            this.dgvTransactionHistory.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvTransactionHistory.Size = new System.Drawing.Size(744, 287);
            this.dgvTransactionHistory.TabIndex = 6;
            // 
            // TransactionNo
            // 
            this.TransactionNo.HeaderText = "Trans No";
            this.TransactionNo.Name = "TransactionNo";
            // 
            // TransactionType
            // 
            this.TransactionType.HeaderText = "Type";
            this.TransactionType.Name = "TransactionType";
            // 
            // TransDate
            // 
            this.TransDate.HeaderText = "Date";
            this.TransDate.Name = "TransDate";
            // 
            // Product
            // 
            this.Product.HeaderText = "Product";
            this.Product.Name = "Product";
            // 
            // UOM
            // 
            this.UOM.HeaderText = "UOM";
            this.UOM.Name = "UOM";
            // 
            // Quantity
            // 
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.Quantity.DefaultCellStyle = dataGridViewCellStyle9;
            this.Quantity.HeaderText = "Quantity";
            this.Quantity.Name = "Quantity";
            // 
            // Price
            // 
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.Price.DefaultCellStyle = dataGridViewCellStyle10;
            this.Price.HeaderText = "Price";
            this.Price.Name = "Price";
            // 
            // TradingMarkUp
            // 
            dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.TradingMarkUp.DefaultCellStyle = dataGridViewCellStyle11;
            this.TradingMarkUp.HeaderText = "TradingMarkUp";
            this.TradingMarkUp.Name = "TradingMarkUp";
            // 
            // SD
            // 
            dataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.SD.DefaultCellStyle = dataGridViewCellStyle12;
            this.SD.HeaderText = "SD";
            this.SD.Name = "SD";
            // 
            // VATRate
            // 
            dataGridViewCellStyle13.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.VATRate.DefaultCellStyle = dataGridViewCellStyle13;
            this.VATRate.HeaderText = "VATRate";
            this.VATRate.Name = "VATRate";
            // 
            // TCost
            // 
            dataGridViewCellStyle14.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.TCost.DefaultCellStyle = dataGridViewCellStyle14;
            this.TCost.HeaderText = "Total";
            this.TCost.Name = "TCost";
            // 
            // CreatedBy
            // 
            this.CreatedBy.HeaderText = "CreateBy";
            this.CreatedBy.Name = "CreatedBy";
            // 
            // CreatedOn
            // 
            this.CreatedOn.HeaderText = "CreateDate";
            this.CreatedOn.Name = "CreatedOn";
            // 
            // LastModifiedBy
            // 
            this.LastModifiedBy.HeaderText = "UpdateBy";
            this.LastModifiedBy.Name = "LastModifiedBy";
            // 
            // LastModifiedOn
            // 
            this.LastModifiedOn.HeaderText = "UpdateDate";
            this.LastModifiedOn.Name = "LastModifiedOn";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(28, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Transaction No";
            // 
            // txtTransactionNo
            // 
            this.txtTransactionNo.Location = new System.Drawing.Point(133, 21);
            this.txtTransactionNo.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtTransactionNo.MinimumSize = new System.Drawing.Size(185, 20);
            this.txtTransactionNo.Name = "txtTransactionNo";
            this.txtTransactionNo.Size = new System.Drawing.Size(185, 21);
            this.txtTransactionNo.TabIndex = 0;
            this.txtTransactionNo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtTransactionNo_KeyDown);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(28, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(90, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Transaction Type";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(336, 24);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(89, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Transaction Date";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(336, 46);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(74, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Product Name";
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSearch.Image = global::VATClient.Properties.Resources.search;
            this.btnSearch.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSearch.Location = new System.Drawing.Point(680, 37);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(72, 28);
            this.btnSearch.TabIndex = 6;
            this.btnSearch.Text = "&Search";
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // dtpTransFromDate
            // 
            this.dtpTransFromDate.Checked = false;
            this.dtpTransFromDate.CustomFormat = "dd/MMM/yyyy";
            this.dtpTransFromDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpTransFromDate.Location = new System.Drawing.Point(441, 20);
            this.dtpTransFromDate.Name = "dtpTransFromDate";
            this.dtpTransFromDate.ShowCheckBox = true;
            this.dtpTransFromDate.Size = new System.Drawing.Size(103, 21);
            this.dtpTransFromDate.TabIndex = 2;
            this.dtpTransFromDate.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dtpAuditFromDate_KeyDown);
            // 
            // dtpTransToDate
            // 
            this.dtpTransToDate.Checked = false;
            this.dtpTransToDate.CustomFormat = "dd/MMM/yyyy";
            this.dtpTransToDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpTransToDate.Location = new System.Drawing.Point(570, 20);
            this.dtpTransToDate.Name = "dtpTransToDate";
            this.dtpTransToDate.ShowCheckBox = true;
            this.dtpTransToDate.Size = new System.Drawing.Size(106, 21);
            this.dtpTransToDate.TabIndex = 3;
            this.dtpTransToDate.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dtpAuditToDate_KeyDown);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(549, 24);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(17, 13);
            this.label11.TabIndex = 112;
            this.label11.Text = "to";
            // 
            // grbTransactionHistory
            // 
            this.grbTransactionHistory.Controls.Add(this.btnProduct);
            this.grbTransactionHistory.Controls.Add(this.cmbTransType);
            this.grbTransactionHistory.Controls.Add(this.txtProduct);
            this.grbTransactionHistory.Controls.Add(this.label11);
            this.grbTransactionHistory.Controls.Add(this.dtpTransToDate);
            this.grbTransactionHistory.Controls.Add(this.dtpTransFromDate);
            this.grbTransactionHistory.Controls.Add(this.btnSearch);
            this.grbTransactionHistory.Controls.Add(this.label4);
            this.grbTransactionHistory.Controls.Add(this.label3);
            this.grbTransactionHistory.Controls.Add(this.label2);
            this.grbTransactionHistory.Controls.Add(this.txtTransactionNo);
            this.grbTransactionHistory.Controls.Add(this.label1);
            this.grbTransactionHistory.Location = new System.Drawing.Point(14, 11);
            this.grbTransactionHistory.Name = "grbTransactionHistory";
            this.grbTransactionHistory.Size = new System.Drawing.Size(756, 72);
            this.grbTransactionHistory.TabIndex = 3;
            this.grbTransactionHistory.TabStop = false;
            this.grbTransactionHistory.Text = "Searching Criteria";
            // 
            // btnProduct
            // 
            this.btnProduct.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnProduct.Image = global::VATClient.Properties.Resources.search;
            this.btnProduct.Location = new System.Drawing.Point(646, 45);
            this.btnProduct.Name = "btnProduct";
            this.btnProduct.Size = new System.Drawing.Size(30, 20);
            this.btnProduct.TabIndex = 5;
            this.btnProduct.UseVisualStyleBackColor = false;
            this.btnProduct.Click += new System.EventHandler(this.btnProduct_Click);
            // 
            // cmbTransType
            // 
            this.cmbTransType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTransType.FormattingEnabled = true;
            this.cmbTransType.Items.AddRange(new object[] {
            "PurchaseInsert",
            "PurchaseUpdate",
            "IssueInsert",
            "IssueUpdate",
            "ReceiveInsert",
            "ReceiveUpdate",
            "SaleInsert",
            "SaleUpdate",
            "DebitNoteInsert",
            "DebitNoteUpdate",
            "CreditNoteInsert",
            "CreditNoteUpdate"});
            this.cmbTransType.Location = new System.Drawing.Point(133, 43);
            this.cmbTransType.Name = "cmbTransType";
            this.cmbTransType.Size = new System.Drawing.Size(185, 21);
            this.cmbTransType.TabIndex = 1;
            this.cmbTransType.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmbTransType_KeyDown);
            // 
            // txtProduct
            // 
            this.txtProduct.Location = new System.Drawing.Point(441, 44);
            this.txtProduct.MaximumSize = new System.Drawing.Size(200, 20);
            this.txtProduct.MinimumSize = new System.Drawing.Size(185, 20);
            this.txtProduct.Name = "txtProduct";
            this.txtProduct.Size = new System.Drawing.Size(199, 21);
            this.txtProduct.TabIndex = 4;
            this.txtProduct.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtProduct_KeyDown);
            // 
            // backgroundWorkerSearch
            // 
            this.backgroundWorkerSearch.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerSearch_DoWork);
            this.backgroundWorkerSearch.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerSearch_RunWorkerCompleted);
            // 
            // FormTransactionHistorySearch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(784, 462);
            this.Controls.Add(this.dgvTransactionHistory);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.grbTransactionHistory);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(800, 500);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(800, 500);
            this.Name = "FormTransactionHistorySearch";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Transaction History Search";
            this.Load += new System.EventHandler(this.FormTransactionHistorySearch_Load);
            this.panel1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvTransactionHistory)).EndInit();
            this.grbTransactionHistory.ResumeLayout(false);
            this.grbTransactionHistory.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridView dgvTransactionHistory;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtTransactionNo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.DateTimePicker dtpTransFromDate;
        private System.Windows.Forms.DateTimePicker dtpTransToDate;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.GroupBox grbTransactionHistory;
        private System.Windows.Forms.TextBox txtProduct;
        private System.Windows.Forms.ComboBox cmbTransType;
        private System.Windows.Forms.Button btnProduct;
        private System.Windows.Forms.DataGridViewTextBoxColumn TransactionNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn TransactionType;
        private System.Windows.Forms.DataGridViewTextBoxColumn TransDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn Product;
        private System.Windows.Forms.DataGridViewTextBoxColumn UOM;
        private System.Windows.Forms.DataGridViewTextBoxColumn Quantity;
        private System.Windows.Forms.DataGridViewTextBoxColumn Price;
        private System.Windows.Forms.DataGridViewTextBoxColumn TradingMarkUp;
        private System.Windows.Forms.DataGridViewTextBoxColumn SD;
        private System.Windows.Forms.DataGridViewTextBoxColumn VATRate;
        private System.Windows.Forms.DataGridViewTextBoxColumn TCost;
        private System.Windows.Forms.DataGridViewTextBoxColumn CreatedBy;
        private System.Windows.Forms.DataGridViewTextBoxColumn CreatedOn;
        private System.Windows.Forms.DataGridViewTextBoxColumn LastModifiedBy;
        private System.Windows.Forms.DataGridViewTextBoxColumn LastModifiedOn;
        private System.ComponentModel.BackgroundWorker backgroundWorkerSearch;
        private System.Windows.Forms.ProgressBar progressBar1;
    }
}