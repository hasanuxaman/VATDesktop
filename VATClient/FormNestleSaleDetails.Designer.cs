namespace VATClient.ReportPreview
{
    partial class FormNestleSaleDetails
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgvSales = new System.Windows.Forms.DataGridView();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.txtChalanNo = new System.Windows.Forms.TextBox();
            this.dgvLoadedTable = new System.Windows.Forms.DataGridView();
            this.Invoices = new System.Windows.Forms.GroupBox();
            this.chkSelectAll = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cmbPrinterName = new System.Windows.Forms.ComboBox();
            this.lblRowCount = new System.Windows.Forms.Label();
            this.bgwChallanSearch = new System.ComponentModel.BackgroundWorker();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnPreview = new System.Windows.Forms.Button();
            this.btnSelectedPrint = new System.Windows.Forms.Button();
            this.LRecordCount = new System.Windows.Forms.Label();
            this.btnOk = new System.Windows.Forms.Button();
            this.bgwPrint = new System.ComponentModel.BackgroundWorker();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSales)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLoadedTable)).BeginInit();
            this.Invoices.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvSales
            // 
            this.dgvSales.AllowUserToAddRows = false;
            this.dgvSales.AllowUserToDeleteRows = false;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.dgvSales.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvSales.BackgroundColor = System.Drawing.SystemColors.ActiveCaption;
            this.dgvSales.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSales.Location = new System.Drawing.Point(7, 16);
            this.dgvSales.Name = "dgvSales";
            this.dgvSales.ReadOnly = true;
            this.dgvSales.RowHeadersVisible = false;
            this.dgvSales.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvSales.Size = new System.Drawing.Size(758, 183);
            this.dgvSales.TabIndex = 102;
            this.dgvSales.TabStop = false;
            this.dgvSales.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSales_CellClick);
            this.dgvSales.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSales_CellContentClick);
            this.dgvSales.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.dgvSales_CellPainting);
            this.dgvSales.DoubleClick += new System.EventHandler(this.dgvSubForm_DoubleClick);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(282, 73);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(227, 22);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 239;
            this.progressBar1.Visible = false;
            // 
            // txtChalanNo
            // 
            this.txtChalanNo.Location = new System.Drawing.Point(221, 2);
            this.txtChalanNo.Name = "txtChalanNo";
            this.txtChalanNo.Size = new System.Drawing.Size(100, 20);
            this.txtChalanNo.TabIndex = 242;
            this.txtChalanNo.Visible = false;
            // 
            // dgvLoadedTable
            // 
            this.dgvLoadedTable.AllowUserToAddRows = false;
            this.dgvLoadedTable.AllowUserToDeleteRows = false;
            this.dgvLoadedTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvLoadedTable.Location = new System.Drawing.Point(6, 15);
            this.dgvLoadedTable.Name = "dgvLoadedTable";
            this.dgvLoadedTable.ReadOnly = true;
            this.dgvLoadedTable.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvLoadedTable.Size = new System.Drawing.Size(758, 166);
            this.dgvLoadedTable.TabIndex = 243;
            // 
            // Invoices
            // 
            this.Invoices.Controls.Add(this.chkSelectAll);
            this.Invoices.Controls.Add(this.dgvSales);
            this.Invoices.Location = new System.Drawing.Point(5, 2);
            this.Invoices.Name = "Invoices";
            this.Invoices.Size = new System.Drawing.Size(772, 203);
            this.Invoices.TabIndex = 244;
            this.Invoices.TabStop = false;
            // 
            // chkSelectAll
            // 
            this.chkSelectAll.AutoSize = true;
            this.chkSelectAll.Location = new System.Drawing.Point(13, -1);
            this.chkSelectAll.Name = "chkSelectAll";
            this.chkSelectAll.Size = new System.Drawing.Size(67, 17);
            this.chkSelectAll.TabIndex = 247;
            this.chkSelectAll.Text = "SelectAll";
            this.chkSelectAll.UseVisualStyleBackColor = true;
            this.chkSelectAll.CheckedChanged += new System.EventHandler(this.chkSelectAll_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.cmbPrinterName);
            this.groupBox1.Controls.Add(this.progressBar1);
            this.groupBox1.Controls.Add(this.dgvLoadedTable);
            this.groupBox1.Location = new System.Drawing.Point(6, 224);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(770, 280);
            this.groupBox1.TabIndex = 245;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Details";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(8, 193);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(68, 13);
            this.label5.TabIndex = 245;
            this.label5.Text = "Printer Name";
            // 
            // cmbPrinterName
            // 
            this.cmbPrinterName.FormattingEnabled = true;
            this.cmbPrinterName.Location = new System.Drawing.Point(82, 190);
            this.cmbPrinterName.Name = "cmbPrinterName";
            this.cmbPrinterName.Size = new System.Drawing.Size(208, 21);
            this.cmbPrinterName.TabIndex = 244;
            this.cmbPrinterName.Leave += new System.EventHandler(this.cmbPrinterName_Leave);
            // 
            // lblRowCount
            // 
            this.lblRowCount.AutoSize = true;
            this.lblRowCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRowCount.Location = new System.Drawing.Point(471, 208);
            this.lblRowCount.Name = "lblRowCount";
            this.lblRowCount.Size = new System.Drawing.Size(86, 16);
            this.lblRowCount.TabIndex = 246;
            this.lblRowCount.Text = "Memo Count:";
            // 
            // bgwChallanSearch
            // 
            this.bgwChallanSearch.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwChallanSearch_DoWork);
            this.bgwChallanSearch.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwChallanSearch_RunWorkerCompleted);
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::VATClient.Properties.Resources.Green_Footer;
            this.panel1.Controls.Add(this.btnPreview);
            this.panel1.Controls.Add(this.btnSelectedPrint);
            this.panel1.Controls.Add(this.LRecordCount);
            this.panel1.Controls.Add(this.btnOk);
            this.panel1.Location = new System.Drawing.Point(6, 510);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(770, 32);
            this.panel1.TabIndex = 244;
            this.panel1.TabStop = true;
            // 
            // btnPreview
            // 
            this.btnPreview.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnPreview.Image = global::VATClient.Properties.Resources.Print;
            this.btnPreview.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPreview.Location = new System.Drawing.Point(141, 1);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new System.Drawing.Size(70, 28);
            this.btnPreview.TabIndex = 56;
            this.btnPreview.Text = "Preview";
            this.btnPreview.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnPreview.UseVisualStyleBackColor = false;
            this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
            // 
            // btnSelectedPrint
            // 
            this.btnSelectedPrint.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSelectedPrint.Image = global::VATClient.Properties.Resources.Print;
            this.btnSelectedPrint.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSelectedPrint.Location = new System.Drawing.Point(5, 1);
            this.btnSelectedPrint.Name = "btnSelectedPrint";
            this.btnSelectedPrint.Size = new System.Drawing.Size(74, 28);
            this.btnSelectedPrint.TabIndex = 55;
            this.btnSelectedPrint.Text = "Print";
            this.btnSelectedPrint.UseVisualStyleBackColor = false;
            this.btnSelectedPrint.Click += new System.EventHandler(this.btnSelectedPrint_Click);
            // 
            // LRecordCount
            // 
            this.LRecordCount.AutoSize = true;
            this.LRecordCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LRecordCount.Location = new System.Drawing.Point(471, 9);
            this.LRecordCount.Name = "LRecordCount";
            this.LRecordCount.Size = new System.Drawing.Size(93, 16);
            this.LRecordCount.TabIndex = 14;
            this.LRecordCount.Text = "Details Count: ";
            // 
            // btnOk
            // 
            this.btnOk.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnOk.Image = global::VATClient.Properties.Resources.Back;
            this.btnOk.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOk.Location = new System.Drawing.Point(670, 2);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 28);
            this.btnOk.TabIndex = 13;
            this.btnOk.Text = "&Ok";
            this.btnOk.UseVisualStyleBackColor = false;
            // 
            // bgwPrint
            // 
            this.bgwPrint.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwPrint_DoWork);
            this.bgwPrint.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwPrint_RunWorkerCompleted);
            // 
            // FormNestleSaleDetails
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(782, 545);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.lblRowCount);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.Invoices);
            this.Controls.Add(this.txtChalanNo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.Name = "FormNestleSaleDetails";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Sales Information Details";
            this.Load += new System.EventHandler(this.FormSubForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSales)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLoadedTable)).EndInit();
            this.Invoices.ResumeLayout(false);
            this.Invoices.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvSales;
        private System.Windows.Forms.ProgressBar progressBar1;
        public System.Windows.Forms.TextBox txtChalanNo;
        private System.Windows.Forms.DataGridView dgvLoadedTable;
        private System.Windows.Forms.GroupBox Invoices;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblRowCount;
        private System.Windows.Forms.CheckBox chkSelectAll;
        private System.ComponentModel.BackgroundWorker bgwChallanSearch;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label LRecordCount;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnPreview;
        private System.Windows.Forms.Button btnSelectedPrint;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cmbPrinterName;
        private System.ComponentModel.BackgroundWorker bgwPrint;
    }
}