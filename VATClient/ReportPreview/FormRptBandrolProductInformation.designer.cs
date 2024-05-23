namespace VATClient.ReportPages
{
    partial class FormRptBandrolProductInformation
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
            this.grbBankInformation = new System.Windows.Forms.GroupBox();
            this.rbtnProduct = new System.Windows.Forms.RadioButton();
            this.cmbProductName = new System.Windows.Forms.ComboBox();
            this.rbtnCode = new System.Windows.Forms.RadioButton();
            this.cmbProductCode = new System.Windows.Forms.ComboBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.label17 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.btnPreview = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.backgroundWorkerPreview = new System.ComponentModel.BackgroundWorker();
            this.backgroundWorkerPTypeSearch = new System.ComponentModel.BackgroundWorker();
            this.cmbFontSize = new System.Windows.Forms.ComboBox();
            this.grbBankInformation.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // grbBankInformation
            // 
            this.grbBankInformation.Controls.Add(this.cmbFontSize);
            this.grbBankInformation.Controls.Add(this.rbtnProduct);
            this.grbBankInformation.Controls.Add(this.cmbProductName);
            this.grbBankInformation.Controls.Add(this.rbtnCode);
            this.grbBankInformation.Controls.Add(this.cmbProductCode);
            this.grbBankInformation.Controls.Add(this.progressBar1);
            this.grbBankInformation.Controls.Add(this.label17);
            this.grbBankInformation.Controls.Add(this.label4);
            this.grbBankInformation.Location = new System.Drawing.Point(12, 32);
            this.grbBankInformation.Name = "grbBankInformation";
            this.grbBankInformation.Size = new System.Drawing.Size(362, 123);
            this.grbBankInformation.TabIndex = 73;
            this.grbBankInformation.TabStop = false;
            this.grbBankInformation.Text = "Reporting Criteria";
            // 
            // rbtnProduct
            // 
            this.rbtnProduct.AutoSize = true;
            this.rbtnProduct.BackColor = System.Drawing.SystemColors.Window;
            this.rbtnProduct.Checked = true;
            this.rbtnProduct.Location = new System.Drawing.Point(280, 52);
            this.rbtnProduct.Name = "rbtnProduct";
            this.rbtnProduct.Size = new System.Drawing.Size(14, 13);
            this.rbtnProduct.TabIndex = 214;
            this.rbtnProduct.TabStop = true;
            this.rbtnProduct.UseVisualStyleBackColor = false;
            this.rbtnProduct.CheckedChanged += new System.EventHandler(this.rbtnProduct_CheckedChanged);
            // 
            // cmbProductName
            // 
            this.cmbProductName.FormattingEnabled = true;
            this.cmbProductName.Location = new System.Drawing.Point(129, 48);
            this.cmbProductName.Name = "cmbProductName";
            this.cmbProductName.Size = new System.Drawing.Size(185, 21);
            this.cmbProductName.TabIndex = 215;
            this.cmbProductName.SelectedIndexChanged += new System.EventHandler(this.cmbProductName_SelectedIndexChanged);
            this.cmbProductName.Leave += new System.EventHandler(this.cmbProductName_Leave);
            // 
            // rbtnCode
            // 
            this.rbtnCode.AutoSize = true;
            this.rbtnCode.BackColor = System.Drawing.SystemColors.Window;
            this.rbtnCode.Checked = true;
            this.rbtnCode.Location = new System.Drawing.Point(279, 25);
            this.rbtnCode.Name = "rbtnCode";
            this.rbtnCode.Size = new System.Drawing.Size(14, 13);
            this.rbtnCode.TabIndex = 213;
            this.rbtnCode.TabStop = true;
            this.rbtnCode.UseVisualStyleBackColor = false;
            this.rbtnCode.CheckedChanged += new System.EventHandler(this.rbtnCode_CheckedChanged);
            // 
            // cmbProductCode
            // 
            this.cmbProductCode.FormattingEnabled = true;
            this.cmbProductCode.Location = new System.Drawing.Point(129, 22);
            this.cmbProductCode.Name = "cmbProductCode";
            this.cmbProductCode.Size = new System.Drawing.Size(185, 21);
            this.cmbProductCode.TabIndex = 196;
            this.cmbProductCode.SelectedIndexChanged += new System.EventHandler(this.cmbProductCode_SelectedIndexChanged);
            this.cmbProductCode.Leave += new System.EventHandler(this.cmbProductCode_Leave);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(88, 91);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(250, 22);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 195;
            this.progressBar1.Visible = false;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(22, 27);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(72, 13);
            this.label17.TabIndex = 132;
            this.label17.Text = "Product Code";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(22, 52);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(74, 13);
            this.label4.TabIndex = 130;
            this.label4.Text = "Product Name";
            // 
            // btnPreview
            // 
            this.btnPreview.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnPreview.Image = global::VATClient.Properties.Resources.Print;
            this.btnPreview.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPreview.Location = new System.Drawing.Point(24, 8);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new System.Drawing.Size(75, 28);
            this.btnPreview.TabIndex = 6;
            this.btnPreview.Text = "Preview";
            this.btnPreview.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnPreview.UseVisualStyleBackColor = false;
            this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::VATClient.Properties.Resources.Green_Footer;
            this.panel1.Controls.Add(this.btnClose);
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Controls.Add(this.btnPreview);
            this.panel1.Location = new System.Drawing.Point(0, 168);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(388, 40);
            this.panel1.TabIndex = 72;
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnClose.Image = global::VATClient.Properties.Resources.Back;
            this.btnClose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnClose.Location = new System.Drawing.Point(288, 8);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 28);
            this.btnClose.TabIndex = 8;
            this.btnClose.Text = "&Close";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnCancel.Image = global::VATClient.Properties.Resources.Referesh;
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.Location = new System.Drawing.Point(105, 8);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 28);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "&Refresh";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // backgroundWorkerPreview
            // 
            this.backgroundWorkerPreview.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerPreview_DoWork);
            this.backgroundWorkerPreview.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerPreview_RunWorkerCompleted);
            // 
            // cmbFontSize
            // 
            this.cmbFontSize.FormattingEnabled = true;
            this.cmbFontSize.Items.AddRange(new object[] {
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15"});
            this.cmbFontSize.Location = new System.Drawing.Point(6, 96);
            this.cmbFontSize.Name = "cmbFontSize";
            this.cmbFontSize.Size = new System.Drawing.Size(36, 21);
            this.cmbFontSize.TabIndex = 195;
            this.cmbFontSize.Text = "8";
            // 
            // FormRptBandrolProductInformation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(384, 207);
            this.Controls.Add(this.grbBankInformation);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(400, 245);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(400, 245);
            this.Name = "FormRptBandrolProductInformation";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Report (BanderolProduct)";
            this.Load += new System.EventHandler(this.FormRptBandrolProductInformation_Load);
            this.grbBankInformation.ResumeLayout(false);
            this.grbBankInformation.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grbBankInformation;
        private System.Windows.Forms.Button btnPreview;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.ComponentModel.BackgroundWorker backgroundWorkerPreview;
        private System.ComponentModel.BackgroundWorker backgroundWorkerPTypeSearch;
        private System.Windows.Forms.ComboBox cmbProductCode;
        private System.Windows.Forms.RadioButton rbtnProduct;
        private System.Windows.Forms.ComboBox cmbProductName;
        private System.Windows.Forms.RadioButton rbtnCode;
        private System.Windows.Forms.ComboBox cmbFontSize;
    }
}