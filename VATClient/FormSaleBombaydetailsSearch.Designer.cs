namespace VATClient
{
    partial class FormSaleBombaydetailsSearch
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
            this.dgvSaleBombay = new System.Windows.Forms.DataGridView();
            this.Select = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.lblInvoiceNo = new System.Windows.Forms.Label();
            this.txtInvoiceNo = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSaleBombay)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvSaleBombay
            // 
            this.dgvSaleBombay.AllowUserToAddRows = false;
            this.dgvSaleBombay.AllowUserToDeleteRows = false;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.dgvSaleBombay.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvSaleBombay.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSaleBombay.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Select});
            this.dgvSaleBombay.Location = new System.Drawing.Point(1, 25);
            this.dgvSaleBombay.Name = "dgvSaleBombay";
            this.dgvSaleBombay.RowHeadersVisible = false;
            this.dgvSaleBombay.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvSaleBombay.Size = new System.Drawing.Size(705, 272);
            this.dgvSaleBombay.TabIndex = 17;
            this.dgvSaleBombay.TabStop = false;
            // 
            // Select
            // 
            this.Select.HeaderText = "Select";
            this.Select.Name = "Select";
            // 
            // lblInvoiceNo
            // 
            this.lblInvoiceNo.AutoSize = true;
            this.lblInvoiceNo.Location = new System.Drawing.Point(12, 9);
            this.lblInvoiceNo.Name = "lblInvoiceNo";
            this.lblInvoiceNo.Size = new System.Drawing.Size(52, 13);
            this.lblInvoiceNo.TabIndex = 18;
            this.lblInvoiceNo.Text = "DETAILS";
            // 
            // txtInvoiceNo
            // 
            this.txtInvoiceNo.Location = new System.Drawing.Point(283, 2);
            this.txtInvoiceNo.Name = "txtInvoiceNo";
            this.txtInvoiceNo.Size = new System.Drawing.Size(100, 20);
            this.txtInvoiceNo.TabIndex = 243;
            // 
            // FormSaleBombaydetailsSearch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(706, 309);
            this.Controls.Add(this.txtInvoiceNo);
            this.Controls.Add(this.lblInvoiceNo);
            this.Controls.Add(this.dgvSaleBombay);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.Name = "FormSaleBombaydetailsSearch";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FormSaleBombaydetailsSearch";
            this.Load += new System.EventHandler(this.FormSaleBombaydetailsSearch_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSaleBombay)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvSaleBombay;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Select;
        private System.Windows.Forms.Label lblInvoiceNo;
        public System.Windows.Forms.TextBox txtInvoiceNo;
    }
}