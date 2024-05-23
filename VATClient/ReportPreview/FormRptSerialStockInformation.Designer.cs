namespace VATClient.ReportPreview
{
    partial class FormRptSerialStockInformation
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRptSerialStockInformation));
            this.grbBankInformation = new System.Windows.Forms.GroupBox();
            this.cmbPost = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.dtpToDate = new System.Windows.Forms.DateTimePicker();
            this.dtpFromDate = new System.Windows.Forms.DateTimePicker();
            this.label16 = new System.Windows.Forms.Label();
            this.txtPGroupId = new System.Windows.Forms.TextBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.btnSearchGroup = new System.Windows.Forms.Button();
            this.cmbProductType = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.btnSearchProduct = new System.Windows.Forms.Button();
            this.txtProductName = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.txtCategoryName = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtItemNo = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnPreview = new System.Windows.Forms.Button();
            this.backgroundWorkerPreview = new System.ComponentModel.BackgroundWorker();
            this.cmbFontSize = new System.Windows.Forms.ComboBox();
            this.grbBankInformation.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // grbBankInformation
            // 
            this.grbBankInformation.Controls.Add(this.cmbFontSize);
            this.grbBankInformation.Controls.Add(this.cmbPost);
            this.grbBankInformation.Controls.Add(this.label1);
            this.grbBankInformation.Controls.Add(this.label11);
            this.grbBankInformation.Controls.Add(this.dtpToDate);
            this.grbBankInformation.Controls.Add(this.dtpFromDate);
            this.grbBankInformation.Controls.Add(this.label16);
            this.grbBankInformation.Controls.Add(this.txtPGroupId);
            this.grbBankInformation.Controls.Add(this.progressBar1);
            this.grbBankInformation.Controls.Add(this.btnSearchGroup);
            this.grbBankInformation.Controls.Add(this.cmbProductType);
            this.grbBankInformation.Controls.Add(this.label9);
            this.grbBankInformation.Controls.Add(this.btnSearchProduct);
            this.grbBankInformation.Controls.Add(this.txtProductName);
            this.grbBankInformation.Controls.Add(this.label17);
            this.grbBankInformation.Controls.Add(this.txtCategoryName);
            this.grbBankInformation.Controls.Add(this.label4);
            this.grbBankInformation.Controls.Add(this.txtItemNo);
            this.grbBankInformation.Location = new System.Drawing.Point(11, 5);
            this.grbBankInformation.Name = "grbBankInformation";
            this.grbBankInformation.Size = new System.Drawing.Size(362, 168);
            this.grbBankInformation.TabIndex = 74;
            this.grbBankInformation.TabStop = false;
            this.grbBankInformation.Text = "Reporting Criteria";
            // 
            // cmbPost
            // 
            this.cmbPost.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPost.FormattingEnabled = true;
            this.cmbPost.Items.AddRange(new object[] {
            "Y",
            "N"});
            this.cmbPost.Location = new System.Drawing.Point(114, 91);
            this.cmbPost.Name = "cmbPost";
            this.cmbPost.Size = new System.Drawing.Size(112, 21);
            this.cmbPost.TabIndex = 217;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(32, 97);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(28, 13);
            this.label1.TabIndex = 218;
            this.label1.Text = "Post";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(221, 118);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(16, 13);
            this.label11.TabIndex = 201;
            this.label11.Text = "to";
            // 
            // dtpToDate
            // 
            this.dtpToDate.Checked = false;
            this.dtpToDate.CustomFormat = "dd/MMM/yyyy";
            this.dtpToDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpToDate.Location = new System.Drawing.Point(243, 114);
            this.dtpToDate.Name = "dtpToDate";
            this.dtpToDate.ShowCheckBox = true;
            this.dtpToDate.Size = new System.Drawing.Size(113, 20);
            this.dtpToDate.TabIndex = 200;
            // 
            // dtpFromDate
            // 
            this.dtpFromDate.Checked = false;
            this.dtpFromDate.CustomFormat = "dd/MMM/yyyy";
            this.dtpFromDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpFromDate.Location = new System.Drawing.Point(114, 114);
            this.dtpFromDate.Name = "dtpFromDate";
            this.dtpFromDate.ShowCheckBox = true;
            this.dtpFromDate.Size = new System.Drawing.Size(104, 20);
            this.dtpFromDate.TabIndex = 199;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(32, 118);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(30, 13);
            this.label16.TabIndex = 197;
            this.label16.Text = "Date";
            // 
            // txtPGroupId
            // 
            this.txtPGroupId.Location = new System.Drawing.Point(320, 74);
            this.txtPGroupId.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtPGroupId.MinimumSize = new System.Drawing.Size(25, 20);
            this.txtPGroupId.Name = "txtPGroupId";
            this.txtPGroupId.ReadOnly = true;
            this.txtPGroupId.Size = new System.Drawing.Size(25, 20);
            this.txtPGroupId.TabIndex = 196;
            this.txtPGroupId.Visible = false;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(87, 140);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(250, 22);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 195;
            this.progressBar1.Visible = false;
            // 
            // btnSearchGroup
            // 
            this.btnSearchGroup.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSearchGroup.Image = ((System.Drawing.Image)(resources.GetObject("btnSearchGroup.Image")));
            this.btnSearchGroup.Location = new System.Drawing.Point(305, 49);
            this.btnSearchGroup.Name = "btnSearchGroup";
            this.btnSearchGroup.Size = new System.Drawing.Size(30, 20);
            this.btnSearchGroup.TabIndex = 138;
            this.btnSearchGroup.UseVisualStyleBackColor = false;
            this.btnSearchGroup.Click += new System.EventHandler(this.btnSearchGroup_Click);
            // 
            // cmbProductType
            // 
            this.cmbProductType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbProductType.FormattingEnabled = true;
            this.cmbProductType.Location = new System.Drawing.Point(114, 68);
            this.cmbProductType.Name = "cmbProductType";
            this.cmbProductType.Size = new System.Drawing.Size(185, 21);
            this.cmbProductType.TabIndex = 136;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(32, 74);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(71, 13);
            this.label9.TabIndex = 137;
            this.label9.Text = "Product Type";
            // 
            // btnSearchProduct
            // 
            this.btnSearchProduct.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSearchProduct.Image = ((System.Drawing.Image)(resources.GetObject("btnSearchProduct.Image")));
            this.btnSearchProduct.Location = new System.Drawing.Point(305, 24);
            this.btnSearchProduct.Name = "btnSearchProduct";
            this.btnSearchProduct.Size = new System.Drawing.Size(30, 20);
            this.btnSearchProduct.TabIndex = 135;
            this.btnSearchProduct.UseVisualStyleBackColor = false;
            this.btnSearchProduct.Click += new System.EventHandler(this.btnSearchProduct_Click);
            // 
            // txtProductName
            // 
            this.txtProductName.Location = new System.Drawing.Point(114, 24);
            this.txtProductName.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtProductName.MinimumSize = new System.Drawing.Size(185, 20);
            this.txtProductName.Name = "txtProductName";
            this.txtProductName.ReadOnly = true;
            this.txtProductName.Size = new System.Drawing.Size(185, 20);
            this.txtProductName.TabIndex = 126;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(32, 27);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(75, 13);
            this.label17.TabIndex = 132;
            this.label17.Text = "Product Name";
            // 
            // txtCategoryName
            // 
            this.txtCategoryName.Location = new System.Drawing.Point(114, 46);
            this.txtCategoryName.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtCategoryName.MinimumSize = new System.Drawing.Size(185, 20);
            this.txtCategoryName.Name = "txtCategoryName";
            this.txtCategoryName.ReadOnly = true;
            this.txtCategoryName.Size = new System.Drawing.Size(185, 20);
            this.txtCategoryName.TabIndex = 128;
            this.txtCategoryName.TextChanged += new System.EventHandler(this.txtCategoryName_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(32, 49);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(76, 13);
            this.label4.TabIndex = 130;
            this.label4.Text = "Product Group";
            this.label4.Click += new System.EventHandler(this.label4_Click);
            // 
            // txtItemNo
            // 
            this.txtItemNo.Location = new System.Drawing.Point(351, 21);
            this.txtItemNo.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtItemNo.MinimumSize = new System.Drawing.Size(25, 20);
            this.txtItemNo.Name = "txtItemNo";
            this.txtItemNo.ReadOnly = true;
            this.txtItemNo.Size = new System.Drawing.Size(25, 20);
            this.txtItemNo.TabIndex = 125;
            this.txtItemNo.Visible = false;
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel1.BackgroundImage")));
            this.panel1.Controls.Add(this.btnClose);
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Controls.Add(this.btnPreview);
            this.panel1.Location = new System.Drawing.Point(-2, 179);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(388, 40);
            this.panel1.TabIndex = 75;
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnClose.Image = ((System.Drawing.Image)(resources.GetObject("btnClose.Image")));
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
            this.btnCancel.Image = ((System.Drawing.Image)(resources.GetObject("btnCancel.Image")));
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
            // btnPreview
            // 
            this.btnPreview.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnPreview.Image = ((System.Drawing.Image)(resources.GetObject("btnPreview.Image")));
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
            this.cmbFontSize.Location = new System.Drawing.Point(6, 140);
            this.cmbFontSize.Name = "cmbFontSize";
            this.cmbFontSize.Size = new System.Drawing.Size(36, 21);
            this.cmbFontSize.TabIndex = 219;
            this.cmbFontSize.Text = "8";
            // 
            // FormRptSerialStockInformation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(384, 222);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.grbBankInformation);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(450, 260);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(400, 245);
            this.Name = "FormRptSerialStockInformation";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Serial No. Stock Status";
            this.Load += new System.EventHandler(this.FormRptSerialStockInformation_Load);
            this.grbBankInformation.ResumeLayout(false);
            this.grbBankInformation.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grbBankInformation;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button btnSearchGroup;
        public System.Windows.Forms.ComboBox cmbProductType;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button btnSearchProduct;
        public System.Windows.Forms.TextBox txtProductName;
        private System.Windows.Forms.Label label17;
        public System.Windows.Forms.TextBox txtCategoryName;
        private System.Windows.Forms.Label label4;
        public System.Windows.Forms.TextBox txtItemNo;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnPreview;
        public System.Windows.Forms.TextBox txtPGroupId;
        private System.ComponentModel.BackgroundWorker backgroundWorkerPreview;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.DateTimePicker dtpFromDate;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.DateTimePicker dtpToDate;
        private System.Windows.Forms.ComboBox cmbPost;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbFontSize;
    }
}