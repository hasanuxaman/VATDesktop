namespace VATClient
{
    partial class FormMessageShow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMessageShow));
            this.lblMessage = new System.Windows.Forms.Label();
            this.nudPrintCopy = new System.Windows.Forms.NumericUpDown();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblAlreadyPrint = new System.Windows.Forms.Label();
            this.cmbPrinterName = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblInvoiceNo = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.nudPrintCopy)).BeginInit();
            this.SuspendLayout();
            // 
            // lblMessage
            // 
            this.lblMessage.AutoSize = true;
            this.lblMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMessage.ForeColor = System.Drawing.SystemColors.InfoText;
            this.lblMessage.Location = new System.Drawing.Point(12, 11);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(678, 25);
            this.lblMessage.TabIndex = 12;
            this.lblMessage.Text = "A Latest Version Is Available for the Application. Please Take Update.";
            // 
            // nudPrintCopy
            // 
            this.nudPrintCopy.Location = new System.Drawing.Point(271, 179);
            this.nudPrintCopy.Name = "nudPrintCopy";
            this.nudPrintCopy.Size = new System.Drawing.Size(76, 20);
            this.nudPrintCopy.TabIndex = 14;
            this.nudPrintCopy.Visible = false;
            // 
            // btnOk
            // 
            this.btnOk.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnOk.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOk.Location = new System.Drawing.Point(282, 53);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(134, 28);
            this.btnOk.TabIndex = 52;
            this.btnOk.Text = "&OK";
            this.btnOk.UseVisualStyleBackColor = false;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnCancel.Image = ((System.Drawing.Image)(resources.GetObject("btnCancel.Image")));
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.Location = new System.Drawing.Point(339, 173);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(62, 28);
            this.btnCancel.TabIndex = 52;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Visible = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lblAlreadyPrint
            // 
            this.lblAlreadyPrint.AutoSize = true;
            this.lblAlreadyPrint.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAlreadyPrint.Location = new System.Drawing.Point(12, 149);
            this.lblAlreadyPrint.Name = "lblAlreadyPrint";
            this.lblAlreadyPrint.Size = new System.Drawing.Size(167, 17);
            this.lblAlreadyPrint.TabIndex = 12;
            this.lblAlreadyPrint.Text = "You have already printed";
            this.lblAlreadyPrint.Visible = false;
            // 
            // cmbPrinterName
            // 
            this.cmbPrinterName.FormattingEnabled = true;
            this.cmbPrinterName.Location = new System.Drawing.Point(44, 173);
            this.cmbPrinterName.Name = "cmbPrinterName";
            this.cmbPrinterName.Size = new System.Drawing.Size(221, 21);
            this.cmbPrinterName.TabIndex = 53;
            this.cmbPrinterName.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(185, 149);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(91, 17);
            this.label1.TabIndex = 12;
            this.label1.Text = "Printer Name";
            this.label1.Visible = false;
            // 
            // lblInvoiceNo
            // 
            this.lblInvoiceNo.AutoSize = true;
            this.lblInvoiceNo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblInvoiceNo.ForeColor = System.Drawing.SystemColors.Desktop;
            this.lblInvoiceNo.Location = new System.Drawing.Point(279, 11);
            this.lblInvoiceNo.Name = "lblInvoiceNo";
            this.lblInvoiceNo.Size = new System.Drawing.Size(0, 13);
            this.lblInvoiceNo.TabIndex = 54;
            // 
            // FormMessageShow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(693, 79);
            this.ControlBox = false;
            this.Controls.Add(this.lblInvoiceNo);
            this.Controls.Add(this.cmbPrinterName);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.nudPrintCopy);
            this.Controls.Add(this.lblAlreadyPrint);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblMessage);
            this.Location = new System.Drawing.Point(100, 100);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormMessageShow";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Message";
            this.Load += new System.EventHandler(this.FormSalePrint_Load);
            ((System.ComponentModel.ISupportInitialize)(this.nudPrintCopy)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Label lblMessage;
        public System.Windows.Forms.NumericUpDown nudPrintCopy;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        public System.Windows.Forms.Label lblAlreadyPrint;
        private System.Windows.Forms.ComboBox cmbPrinterName;
        public System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblInvoiceNo;
    }
}