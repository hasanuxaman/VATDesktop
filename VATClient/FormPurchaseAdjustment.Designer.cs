namespace VATClient
{
    partial class FormPurchaseAdjustment
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
            this.chkInvoiceNo = new System.Windows.Forms.CheckBox();
            this.chkInvoiceDate = new System.Windows.Forms.CheckBox();
            this.chkReturn = new System.Windows.Forms.CheckBox();
            this.label14 = new System.Windows.Forms.Label();
            this.label59 = new System.Windows.Forms.Label();
            this.bgwAdd = new System.ComponentModel.BackgroundWorker();
            this.txtCode = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.txtReason = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtUom = new System.Windows.Forms.TextBox();
            this.txtProductName = new System.Windows.Forms.TextBox();
            this.bgwUpdate = new System.ComponentModel.BackgroundWorker();
            this.label10 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtSD = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.dtpInvoiceDate = new System.Windows.Forms.DateTimePicker();
            this.txtVATRate = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtQuantity = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtNBRPrice = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.txtInvoiceNo = new System.Windows.Forms.TextBox();
            this.bgrDelete = new System.ComponentModel.BackgroundWorker();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnApplyAll = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // chkInvoiceNo
            // 
            this.chkInvoiceNo.AutoSize = true;
            this.chkInvoiceNo.Location = new System.Drawing.Point(488, 182);
            this.chkInvoiceNo.Name = "chkInvoiceNo";
            this.chkInvoiceNo.Size = new System.Drawing.Size(18, 17);
            this.chkInvoiceNo.TabIndex = 297;
            this.chkInvoiceNo.TabStop = false;
            this.chkInvoiceNo.UseVisualStyleBackColor = true;
            this.chkInvoiceNo.Visible = false;
            // 
            // chkInvoiceDate
            // 
            this.chkInvoiceDate.AutoSize = true;
            this.chkInvoiceDate.Location = new System.Drawing.Point(238, 165);
            this.chkInvoiceDate.Name = "chkInvoiceDate";
            this.chkInvoiceDate.Size = new System.Drawing.Size(18, 17);
            this.chkInvoiceDate.TabIndex = 296;
            this.chkInvoiceDate.TabStop = false;
            this.chkInvoiceDate.UseVisualStyleBackColor = true;
            this.chkInvoiceDate.Visible = false;
            // 
            // chkReturn
            // 
            this.chkReturn.AutoSize = true;
            this.chkReturn.Location = new System.Drawing.Point(326, 74);
            this.chkReturn.Name = "chkReturn";
            this.chkReturn.Size = new System.Drawing.Size(18, 17);
            this.chkReturn.TabIndex = 227;
            this.chkReturn.TabStop = false;
            this.chkReturn.UseVisualStyleBackColor = true;
            // 
            // label14
            // 
            this.label14.Font = new System.Drawing.Font("Tahoma", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.ForeColor = System.Drawing.Color.Red;
            this.label14.Location = new System.Drawing.Point(98, 46);
            this.label14.MaximumSize = new System.Drawing.Size(10, 10);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(9, 10);
            this.label14.TabIndex = 289;
            this.label14.Text = "*";
            this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label59
            // 
            this.label59.Font = new System.Drawing.Font("Tahoma", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label59.ForeColor = System.Drawing.Color.Red;
            this.label59.Location = new System.Drawing.Point(98, 22);
            this.label59.MaximumSize = new System.Drawing.Size(10, 10);
            this.label59.Name = "label59";
            this.label59.Size = new System.Drawing.Size(9, 10);
            this.label59.TabIndex = 288;
            this.label59.Text = "*";
            this.label59.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtCode
            // 
            this.txtCode.Location = new System.Drawing.Point(138, 41);
            this.txtCode.MinimumSize = new System.Drawing.Size(125, 20);
            this.txtCode.Name = "txtCode";
            this.txtCode.ReadOnly = true;
            this.txtCode.Size = new System.Drawing.Size(125, 22);
            this.txtCode.TabIndex = 234;
            this.txtCode.TabStop = false;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(6, 45);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(94, 17);
            this.label13.TabIndex = 235;
            this.label13.Text = "Product Code";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(6, 20);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(98, 17);
            this.label12.TabIndex = 233;
            this.label12.Text = "Product Name";
            // 
            // txtReason
            // 
            this.txtReason.Location = new System.Drawing.Point(138, 69);
            this.txtReason.Multiline = true;
            this.txtReason.Name = "txtReason";
            this.txtReason.Size = new System.Drawing.Size(184, 87);
            this.txtReason.TabIndex = 229;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 73);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(123, 17);
            this.label9.TabIndex = 228;
            this.label9.Text = "Reason Of Return";
            // 
            // txtUom
            // 
            this.txtUom.Location = new System.Drawing.Point(102, 189);
            this.txtUom.Name = "txtUom";
            this.txtUom.Size = new System.Drawing.Size(134, 22);
            this.txtUom.TabIndex = 227;
            this.txtUom.Visible = false;
            // 
            // txtProductName
            // 
            this.txtProductName.Location = new System.Drawing.Point(138, 16);
            this.txtProductName.MinimumSize = new System.Drawing.Size(125, 20);
            this.txtProductName.Name = "txtProductName";
            this.txtProductName.ReadOnly = true;
            this.txtProductName.Size = new System.Drawing.Size(317, 22);
            this.txtProductName.TabIndex = 232;
            this.txtProductName.TabStop = false;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(27, 193);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(40, 17);
            this.label10.TabIndex = 226;
            this.label10.Text = "UOM";
            this.label10.Visible = false;
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.groupBox1.Controls.Add(this.chkInvoiceNo);
            this.groupBox1.Controls.Add(this.chkInvoiceDate);
            this.groupBox1.Controls.Add(this.chkReturn);
            this.groupBox1.Controls.Add(this.label14);
            this.groupBox1.Controls.Add(this.label59);
            this.groupBox1.Controls.Add(this.txtCode);
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Controls.Add(this.txtProductName);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.txtReason);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.txtUom);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.txtSD);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.dtpInvoiceDate);
            this.groupBox1.Controls.Add(this.txtVATRate);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.txtQuantity);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtNBRPrice);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.txtInvoiceNo);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(1, 1);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(517, 270);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            // 
            // txtSD
            // 
            this.txtSD.Location = new System.Drawing.Point(102, 242);
            this.txtSD.Name = "txtSD";
            this.txtSD.Size = new System.Drawing.Size(134, 22);
            this.txtSD.TabIndex = 223;
            this.txtSD.Visible = false;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(27, 246);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(27, 17);
            this.label7.TabIndex = 222;
            this.label7.Text = "SD";
            this.label7.Visible = false;
            // 
            // dtpInvoiceDate
            // 
            this.dtpInvoiceDate.CustomFormat = "dd/MMM/yyyy HH:mm";
            this.dtpInvoiceDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpInvoiceDate.Location = new System.Drawing.Point(102, 162);
            this.dtpInvoiceDate.MaximumSize = new System.Drawing.Size(150, 20);
            this.dtpInvoiceDate.MinimumSize = new System.Drawing.Size(120, 20);
            this.dtpInvoiceDate.Name = "dtpInvoiceDate";
            this.dtpInvoiceDate.Size = new System.Drawing.Size(134, 20);
            this.dtpInvoiceDate.TabIndex = 219;
            this.dtpInvoiceDate.Visible = false;
            // 
            // txtVATRate
            // 
            this.txtVATRate.Location = new System.Drawing.Point(102, 216);
            this.txtVATRate.Name = "txtVATRate";
            this.txtVATRate.Size = new System.Drawing.Size(134, 22);
            this.txtVATRate.TabIndex = 218;
            this.txtVATRate.Visible = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(27, 220);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(87, 17);
            this.label4.TabIndex = 217;
            this.label4.Text = "VATRate(%)";
            this.label4.Visible = false;
            // 
            // txtQuantity
            // 
            this.txtQuantity.Location = new System.Drawing.Point(357, 205);
            this.txtQuantity.Name = "txtQuantity";
            this.txtQuantity.Size = new System.Drawing.Size(125, 22);
            this.txtQuantity.TabIndex = 216;
            this.txtQuantity.Visible = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(256, 209);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(61, 17);
            this.label3.TabIndex = 215;
            this.label3.Text = "Quantity";
            this.label3.Visible = false;
            // 
            // txtNBRPrice
            // 
            this.txtNBRPrice.Location = new System.Drawing.Point(357, 232);
            this.txtNBRPrice.Name = "txtNBRPrice";
            this.txtNBRPrice.Size = new System.Drawing.Size(125, 22);
            this.txtNBRPrice.TabIndex = 214;
            this.txtNBRPrice.Visible = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(256, 236);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 17);
            this.label2.TabIndex = 213;
            this.label2.Text = "NBRPrice";
            this.label2.Visible = false;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(27, 166);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(82, 17);
            this.label8.TabIndex = 211;
            this.label8.Text = "InvoiceDate";
            this.label8.Visible = false;
            // 
            // txtInvoiceNo
            // 
            this.txtInvoiceNo.Location = new System.Drawing.Point(357, 178);
            this.txtInvoiceNo.MaximumSize = new System.Drawing.Size(4, 25);
            this.txtInvoiceNo.MinimumSize = new System.Drawing.Size(125, 20);
            this.txtInvoiceNo.Name = "txtInvoiceNo";
            this.txtInvoiceNo.Size = new System.Drawing.Size(125, 22);
            this.txtInvoiceNo.TabIndex = 0;
            this.txtInvoiceNo.TabStop = false;
            this.txtInvoiceNo.Visible = false;
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::VATClient.Properties.Resources.Green_Footer;
            this.panel1.Controls.Add(this.btnApplyAll);
            this.panel1.Controls.Add(this.btnAdd);
            this.panel1.Location = new System.Drawing.Point(5, 281);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(508, 40);
            this.panel1.TabIndex = 10;
            // 
            // btnApplyAll
            // 
            this.btnApplyAll.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnApplyAll.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnApplyAll.Image = global::VATClient.Properties.Resources.Post;
            this.btnApplyAll.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnApplyAll.Location = new System.Drawing.Point(112, 6);
            this.btnApplyAll.Name = "btnApplyAll";
            this.btnApplyAll.Size = new System.Drawing.Size(75, 28);
            this.btnApplyAll.TabIndex = 10;
            this.btnApplyAll.Text = "Apply All";
            this.btnApplyAll.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnApplyAll.UseVisualStyleBackColor = false;
            this.btnApplyAll.Click += new System.EventHandler(this.btnApplyAll_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnAdd.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAdd.Image = global::VATClient.Properties.Resources.Save;
            this.btnAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAdd.Location = new System.Drawing.Point(11, 6);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 28);
            this.btnAdd.TabIndex = 9;
            this.btnAdd.Text = "&Update";
            this.btnAdd.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnAdd.UseVisualStyleBackColor = false;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(256, 182);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 17);
            this.label1.TabIndex = 6;
            this.label1.Text = "Invoice No";
            this.label1.Visible = false;
            // 
            // FormPurchaseAdjustment
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(519, 323);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.Name = "FormPurchaseAdjustment";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Purchase Adjustment";
            this.Load += new System.EventHandler(this.FormPurchaseAdjustment_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        public System.Windows.Forms.Button btnApplyAll;
        public System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.CheckBox chkInvoiceNo;
        private System.Windows.Forms.CheckBox chkInvoiceDate;
        private System.Windows.Forms.CheckBox chkReturn;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label59;
        private System.ComponentModel.BackgroundWorker bgwAdd;
        private System.Windows.Forms.TextBox txtCode;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox txtReason;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtUom;
        private System.Windows.Forms.TextBox txtProductName;
        private System.ComponentModel.BackgroundWorker bgwUpdate;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtSD;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.DateTimePicker dtpInvoiceDate;
        private System.Windows.Forms.TextBox txtVATRate;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtQuantity;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtNBRPrice;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtInvoiceNo;
        private System.ComponentModel.BackgroundWorker bgrDelete;
        private System.Windows.Forms.Label label1;
    }
}