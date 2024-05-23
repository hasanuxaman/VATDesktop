namespace VATClient
{
    partial class FormCheckStock
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
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnImport = new System.Windows.Forms.Button();
            this.chkSame = new System.Windows.Forms.CheckBox();
            this.lbCount = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.txtPurchaseInvoiceNo = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnSearchInvoiceNo = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.dtpPurchaseToDate = new System.Windows.Forms.DateTimePicker();
            this.dtpPurchaseFromDate = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(12, 76);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(789, 333);
            this.dataGridView1.TabIndex = 179;
            // 
            // btnExport
            // 
            this.btnExport.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnExport.Image = global::VATClient.Properties.Resources.convert;
            this.btnExport.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnExport.Location = new System.Drawing.Point(101, 38);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(78, 26);
            this.btnExport.TabIndex = 181;
            this.btnExport.Text = "Export";
            this.btnExport.UseVisualStyleBackColor = false;
            this.btnExport.UseWaitCursor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // btnImport
            // 
            this.btnImport.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnImport.Image = global::VATClient.Properties.Resources.Load;
            this.btnImport.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnImport.Location = new System.Drawing.Point(101, 6);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(78, 26);
            this.btnImport.TabIndex = 180;
            this.btnImport.Text = "Import";
            this.btnImport.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnImport.UseVisualStyleBackColor = false;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // chkSame
            // 
            this.chkSame.AutoSize = true;
            this.chkSame.Location = new System.Drawing.Point(23, 12);
            this.chkSame.Name = "chkSame";
            this.chkSame.Size = new System.Drawing.Size(72, 17);
            this.chkSame.TabIndex = 182;
            this.chkSame.TabStop = false;
            this.chkSame.Text = "Same File";
            this.chkSame.UseVisualStyleBackColor = true;
            // 
            // lbCount
            // 
            this.lbCount.AutoSize = true;
            this.lbCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbCount.Location = new System.Drawing.Point(12, 417);
            this.lbCount.Name = "lbCount";
            this.lbCount.Size = new System.Drawing.Size(110, 13);
            this.lbCount.TabIndex = 185;
            this.lbCount.Text = "Total Record(s): 0";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(270, 364);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(290, 24);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 207;
            this.progressBar1.Visible = false;
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.button1.Image = global::VATClient.Properties.Resources.convert;
            this.button1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button1.Location = new System.Drawing.Point(676, 42);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(78, 26);
            this.button1.TabIndex = 209;
            this.button1.Text = "Export";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.UseWaitCursor = true;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.button2.Image = global::VATClient.Properties.Resources.Load;
            this.button2.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button2.Location = new System.Drawing.Point(676, 10);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(78, 26);
            this.button2.TabIndex = 208;
            this.button2.Text = "Import";
            this.button2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Visible = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // txtPurchaseInvoiceNo
            // 
            this.txtPurchaseInvoiceNo.BackColor = System.Drawing.Color.White;
            this.txtPurchaseInvoiceNo.Location = new System.Drawing.Point(309, 9);
            this.txtPurchaseInvoiceNo.MaximumSize = new System.Drawing.Size(500, 25);
            this.txtPurchaseInvoiceNo.MinimumSize = new System.Drawing.Size(150, 20);
            this.txtPurchaseInvoiceNo.Name = "txtPurchaseInvoiceNo";
            this.txtPurchaseInvoiceNo.Size = new System.Drawing.Size(209, 20);
            this.txtPurchaseInvoiceNo.TabIndex = 210;
            this.txtPurchaseInvoiceNo.TabStop = false;
            this.txtPurchaseInvoiceNo.Visible = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(267, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 13);
            this.label2.TabIndex = 211;
            this.label2.Text = "Pur No";
            this.label2.Visible = false;
            // 
            // btnSearchInvoiceNo
            // 
            this.btnSearchInvoiceNo.BackColor = System.Drawing.Color.LightCyan;
            this.btnSearchInvoiceNo.Image = global::VATClient.Properties.Resources.search;
            this.btnSearchInvoiceNo.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSearchInvoiceNo.Location = new System.Drawing.Point(524, 39);
            this.btnSearchInvoiceNo.Name = "btnSearchInvoiceNo";
            this.btnSearchInvoiceNo.Size = new System.Drawing.Size(78, 26);
            this.btnSearchInvoiceNo.TabIndex = 212;
            this.btnSearchInvoiceNo.TabStop = false;
            this.btnSearchInvoiceNo.Text = "Search";
            this.btnSearchInvoiceNo.UseVisualStyleBackColor = false;
            this.btnSearchInvoiceNo.Visible = false;
            this.btnSearchInvoiceNo.Click += new System.EventHandler(this.btnSearchInvoiceNo_Click);
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSave.Image = global::VATClient.Properties.Resources.Save;
            this.btnSave.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSave.Location = new System.Drawing.Point(592, 3);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(78, 26);
            this.btnSave.TabIndex = 213;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.UseWaitCursor = true;
            this.btnSave.Visible = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // dtpPurchaseToDate
            // 
            this.dtpPurchaseToDate.Checked = false;
            this.dtpPurchaseToDate.CustomFormat = "dd/MMM/yyyy";
            this.dtpPurchaseToDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpPurchaseToDate.Location = new System.Drawing.Point(416, 39);
            this.dtpPurchaseToDate.Name = "dtpPurchaseToDate";
            this.dtpPurchaseToDate.ShowCheckBox = true;
            this.dtpPurchaseToDate.Size = new System.Drawing.Size(102, 20);
            this.dtpPurchaseToDate.TabIndex = 216;
            this.dtpPurchaseToDate.Visible = false;
            // 
            // dtpPurchaseFromDate
            // 
            this.dtpPurchaseFromDate.Checked = false;
            this.dtpPurchaseFromDate.CustomFormat = "dd/MMM/yyyy";
            this.dtpPurchaseFromDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpPurchaseFromDate.Location = new System.Drawing.Point(309, 39);
            this.dtpPurchaseFromDate.Name = "dtpPurchaseFromDate";
            this.dtpPurchaseFromDate.ShowCheckBox = true;
            this.dtpPurchaseFromDate.Size = new System.Drawing.Size(101, 20);
            this.dtpPurchaseFromDate.TabIndex = 215;
            this.dtpPurchaseFromDate.Visible = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(229, 45);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(78, 13);
            this.label3.TabIndex = 214;
            this.label3.Text = "Purchase Date";
            this.label3.Visible = false;
            // 
            // FormCheckStock
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(813, 439);
            this.Controls.Add(this.dtpPurchaseToDate);
            this.Controls.Add(this.dtpPurchaseFromDate);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnSearchInvoiceNo);
            this.Controls.Add(this.txtPurchaseInvoiceNo);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.lbCount);
            this.Controls.Add(this.chkSame);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.btnImport);
            this.Controls.Add(this.dataGridView1);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(829, 477);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(829, 477);
            this.Name = "FormCheckStock";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Check Stock";
            this.Load += new System.EventHandler(this.FormCheckStock_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Button btnImport;
        private System.Windows.Forms.CheckBox chkSame;
        private System.Windows.Forms.Label lbCount;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox txtPurchaseInvoiceNo;
        public System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnSearchInvoiceNo;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.DateTimePicker dtpPurchaseToDate;
        private System.Windows.Forms.DateTimePicker dtpPurchaseFromDate;
        private System.Windows.Forms.Label label3;
    }
}