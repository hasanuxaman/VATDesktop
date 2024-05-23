namespace VATClient
{
    partial class FormPurchaseOtherPopUp
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cmpFixedVATRebate = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtItemNo = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbCPCName = new System.Windows.Forms.ComboBox();
            this.lblExpireDate = new System.Windows.Forms.Label();
            this.dptExpireDate = new System.Windows.Forms.DateTimePicker();
            this.txtCode = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.txtProductName = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnAdd = new System.Windows.Forms.Button();
            this.bgwAdd = new System.ComponentModel.BackgroundWorker();
            this.bgwUpdate = new System.ComponentModel.BackgroundWorker();
            this.chkSection21 = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.groupBox1.Controls.Add(this.chkSection21);
            this.groupBox1.Controls.Add(this.cmpFixedVATRebate);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtItemNo);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.cmbCPCName);
            this.groupBox1.Controls.Add(this.lblExpireDate);
            this.groupBox1.Controls.Add(this.dptExpireDate);
            this.groupBox1.Controls.Add(this.txtCode);
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Controls.Add(this.txtProductName);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(438, 160);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // cmpFixedVATRebate
            // 
            this.cmpFixedVATRebate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmpFixedVATRebate.FormattingEnabled = true;
            this.cmpFixedVATRebate.Items.AddRange(new object[] {
            "Y",
            "N"});
            this.cmpFixedVATRebate.Location = new System.Drawing.Point(350, 41);
            this.cmpFixedVATRebate.Name = "cmpFixedVATRebate";
            this.cmpFixedVATRebate.Size = new System.Drawing.Size(74, 25);
            this.cmpFixedVATRebate.TabIndex = 290;
            this.cmpFixedVATRebate.TabStop = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(238, 46);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(115, 17);
            this.label3.TabIndex = 289;
            this.label3.Text = "Fixed VAT Rebate";
            // 
            // txtItemNo
            // 
            this.txtItemNo.Location = new System.Drawing.Point(107, 91);
            this.txtItemNo.MinimumSize = new System.Drawing.Size(125, 20);
            this.txtItemNo.Name = "txtItemNo";
            this.txtItemNo.Size = new System.Drawing.Size(125, 24);
            this.txtItemNo.TabIndex = 287;
            this.txtItemNo.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 95);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 17);
            this.label2.TabIndex = 288;
            this.label2.Text = "BEItemNo";
            // 
            // cmbCPCName
            // 
            this.cmbCPCName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCPCName.FormattingEnabled = true;
            this.cmbCPCName.Location = new System.Drawing.Point(107, 66);
            this.cmbCPCName.Name = "cmbCPCName";
            this.cmbCPCName.Size = new System.Drawing.Size(125, 25);
            this.cmbCPCName.TabIndex = 286;
            this.cmbCPCName.TabStop = false;
            // 
            // lblExpireDate
            // 
            this.lblExpireDate.AutoSize = true;
            this.lblExpireDate.Location = new System.Drawing.Point(250, 71);
            this.lblExpireDate.Name = "lblExpireDate";
            this.lblExpireDate.Size = new System.Drawing.Size(79, 17);
            this.lblExpireDate.TabIndex = 285;
            this.lblExpireDate.Text = "Expire Date";
            // 
            // dptExpireDate
            // 
            this.dptExpireDate.CustomFormat = "dd/MMM/yyyy";
            this.dptExpireDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dptExpireDate.Location = new System.Drawing.Point(322, 66);
            this.dptExpireDate.MaximumSize = new System.Drawing.Size(110, 20);
            this.dptExpireDate.MinimumSize = new System.Drawing.Size(100, 20);
            this.dptExpireDate.Name = "dptExpireDate";
            this.dptExpireDate.Size = new System.Drawing.Size(102, 20);
            this.dptExpireDate.TabIndex = 284;
            // 
            // txtCode
            // 
            this.txtCode.Location = new System.Drawing.Point(107, 41);
            this.txtCode.MinimumSize = new System.Drawing.Size(125, 20);
            this.txtCode.Name = "txtCode";
            this.txtCode.ReadOnly = true;
            this.txtCode.Size = new System.Drawing.Size(125, 24);
            this.txtCode.TabIndex = 234;
            this.txtCode.TabStop = false;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(6, 45);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(93, 17);
            this.label13.TabIndex = 235;
            this.label13.Text = "Product Code";
            // 
            // txtProductName
            // 
            this.txtProductName.Location = new System.Drawing.Point(107, 16);
            this.txtProductName.MinimumSize = new System.Drawing.Size(125, 20);
            this.txtProductName.Name = "txtProductName";
            this.txtProductName.ReadOnly = true;
            this.txtProductName.Size = new System.Drawing.Size(317, 24);
            this.txtProductName.TabIndex = 232;
            this.txtProductName.TabStop = false;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(6, 20);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(96, 17);
            this.label12.TabIndex = 233;
            this.label12.Text = "Product Name";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 71);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 17);
            this.label1.TabIndex = 6;
            this.label1.Text = "CPCName";
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::VATClient.Properties.Resources.Green_Footer;
            this.panel1.Controls.Add(this.btnAdd);
            this.panel1.Location = new System.Drawing.Point(4, 169);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(437, 40);
            this.panel1.TabIndex = 8;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // btnAdd
            // 
            this.btnAdd.BackColor = System.Drawing.SystemColors.ButtonHighlight;
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
            // chkSection21
            // 
            this.chkSection21.AutoSize = true;
            this.chkSection21.Location = new System.Drawing.Point(107, 122);
            this.chkSection21.Name = "chkSection21";
            this.chkSection21.Size = new System.Drawing.Size(95, 21);
            this.chkSection21.TabIndex = 293;
            this.chkSection21.Text = "Section 21";
            this.chkSection21.UseVisualStyleBackColor = true;
            // 
            // FormPurchaseOtherPopUp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(450, 209);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(18, 150);
            this.Name = "FormPurchaseOtherPopUp";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Purchase Details";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormDN_CNAdjustment_FormClosed);
            this.Load += new System.EventHandler(this.FormDN_CNAdjustment_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private System.ComponentModel.BackgroundWorker bgwAdd;
        private System.ComponentModel.BackgroundWorker bgwUpdate;
        public System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.TextBox txtProductName;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox txtCode;
        private System.Windows.Forms.Label label13;
        public System.Windows.Forms.Label lblExpireDate;
        private System.Windows.Forms.DateTimePicker dptExpireDate;
        public System.Windows.Forms.ComboBox cmbCPCName;
        private System.Windows.Forms.TextBox txtItemNo;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.ComboBox cmpFixedVATRebate;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox chkSection21;
    }
}