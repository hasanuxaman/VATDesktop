namespace VATClient
{
    partial class FormUpdatePurchaseDuty
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
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.lbCount = new System.Windows.Forms.Label();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.dtpPurchaseToDate = new System.Windows.Forms.DateTimePicker();
            this.dtpPurchaseFromDate = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnSearchInvoiceNo = new System.Windows.Forms.Button();
            this.txtPurchaseInvoiceNo = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.chkSame = new System.Windows.Forms.CheckBox();
            this.button3 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(270, 343);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(290, 24);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 210;
            this.progressBar1.Visible = false;
            // 
            // lbCount
            // 
            this.lbCount.AutoSize = true;
            this.lbCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbCount.Location = new System.Drawing.Point(12, 407);
            this.lbCount.Name = "lbCount";
            this.lbCount.Size = new System.Drawing.Size(110, 13);
            this.lbCount.TabIndex = 209;
            this.lbCount.Text = "Total Record(s): 0";
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(12, 71);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(789, 333);
            this.dataGridView1.TabIndex = 208;
            // 
            // dtpPurchaseToDate
            // 
            this.dtpPurchaseToDate.Checked = false;
            this.dtpPurchaseToDate.CustomFormat = "dd/MMM/yyyy";
            this.dtpPurchaseToDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpPurchaseToDate.Location = new System.Drawing.Point(203, 42);
            this.dtpPurchaseToDate.Name = "dtpPurchaseToDate";
            this.dtpPurchaseToDate.ShowCheckBox = true;
            this.dtpPurchaseToDate.Size = new System.Drawing.Size(102, 20);
            this.dtpPurchaseToDate.TabIndex = 225;
            // 
            // dtpPurchaseFromDate
            // 
            this.dtpPurchaseFromDate.Checked = false;
            this.dtpPurchaseFromDate.CustomFormat = "dd/MMM/yyyy";
            this.dtpPurchaseFromDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpPurchaseFromDate.Location = new System.Drawing.Point(96, 42);
            this.dtpPurchaseFromDate.Name = "dtpPurchaseFromDate";
            this.dtpPurchaseFromDate.ShowCheckBox = true;
            this.dtpPurchaseFromDate.Size = new System.Drawing.Size(101, 20);
            this.dtpPurchaseFromDate.TabIndex = 224;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 48);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(78, 13);
            this.label3.TabIndex = 223;
            this.label3.Text = "Purchase Date";
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSave.Image = global::VATClient.Properties.Resources.Save;
            this.btnSave.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSave.Location = new System.Drawing.Point(458, 12);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(78, 56);
            this.btnSave.TabIndex = 222;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.UseWaitCursor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnSearchInvoiceNo
            // 
            this.btnSearchInvoiceNo.BackColor = System.Drawing.Color.LightCyan;
            this.btnSearchInvoiceNo.Image = global::VATClient.Properties.Resources.search;
            this.btnSearchInvoiceNo.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSearchInvoiceNo.Location = new System.Drawing.Point(374, 12);
            this.btnSearchInvoiceNo.Name = "btnSearchInvoiceNo";
            this.btnSearchInvoiceNo.Size = new System.Drawing.Size(78, 56);
            this.btnSearchInvoiceNo.TabIndex = 221;
            this.btnSearchInvoiceNo.TabStop = false;
            this.btnSearchInvoiceNo.Text = "Search";
            this.btnSearchInvoiceNo.UseVisualStyleBackColor = false;
            this.btnSearchInvoiceNo.Click += new System.EventHandler(this.btnSearchInvoiceNo_Click);
            // 
            // txtPurchaseInvoiceNo
            // 
            this.txtPurchaseInvoiceNo.BackColor = System.Drawing.Color.White;
            this.txtPurchaseInvoiceNo.Location = new System.Drawing.Point(96, 12);
            this.txtPurchaseInvoiceNo.MaximumSize = new System.Drawing.Size(500, 25);
            this.txtPurchaseInvoiceNo.MinimumSize = new System.Drawing.Size(150, 20);
            this.txtPurchaseInvoiceNo.Name = "txtPurchaseInvoiceNo";
            this.txtPurchaseInvoiceNo.Size = new System.Drawing.Size(209, 20);
            this.txtPurchaseInvoiceNo.TabIndex = 219;
            this.txtPurchaseInvoiceNo.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(54, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 13);
            this.label2.TabIndex = 220;
            this.label2.Text = "Pur No";
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.button1.Image = global::VATClient.Properties.Resources.convert;
            this.button1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button1.Location = new System.Drawing.Point(542, 42);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(78, 26);
            this.button1.TabIndex = 218;
            this.button1.Text = "Export";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.UseWaitCursor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.button2.Image = global::VATClient.Properties.Resources.Load;
            this.button2.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button2.Location = new System.Drawing.Point(544, 12);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(78, 26);
            this.button2.TabIndex = 217;
            this.button2.Text = "Import";
            this.button2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // chkSame
            // 
            this.chkSame.AutoSize = true;
            this.chkSame.Location = new System.Drawing.Point(57, 90);
            this.chkSame.Name = "chkSame";
            this.chkSame.Size = new System.Drawing.Size(72, 17);
            this.chkSame.TabIndex = 226;
            this.chkSame.TabStop = false;
            this.chkSame.Text = "Same File";
            this.chkSame.UseVisualStyleBackColor = true;
            this.chkSame.Visible = false;
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.button3.Image = global::VATClient.Properties.Resources.search;
            this.button3.Location = new System.Drawing.Point(311, 13);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(30, 20);
            this.button3.TabIndex = 227;
            this.button3.TabStop = false;
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // FormUpdatePurchaseDuty
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(813, 431);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.chkSame);
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
            this.Controls.Add(this.dataGridView1);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(829, 470);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(829, 470);
            this.Name = "FormUpdatePurchaseDuty";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Update Purchase Duty";
            this.Load += new System.EventHandler(this.FormUpdatePurchaseDuty_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label lbCount;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DateTimePicker dtpPurchaseToDate;
        private System.Windows.Forms.DateTimePicker dtpPurchaseFromDate;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnSearchInvoiceNo;
        private System.Windows.Forms.TextBox txtPurchaseInvoiceNo;
        public System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.CheckBox chkSame;
        private System.Windows.Forms.Button button3;
    }
}