namespace VATClient
{
    partial class FormBanderolProductSearch
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.dgvBandeProduct = new System.Windows.Forms.DataGridView();
            this.BandProductId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ItemNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ProductCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ProductName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BanderolName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BanderolId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BanderolSize = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BanderolUom = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PackagingId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PackagingName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PackagingSize = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PackagingUom = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BUsedQty = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.txtActiveStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.WastageQty = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OpeningQty = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OpeningDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.bgwSearch = new System.ComponentModel.BackgroundWorker();
            this.panel1 = new System.Windows.Forms.Panel();
            this.LRecordCount = new System.Windows.Forms.Label();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.btnSearch = new System.Windows.Forms.Button();
            this.cmbActive = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnSearchPackage = new System.Windows.Forms.Button();
            this.btnSearchBanderol = new System.Windows.Forms.Button();
            this.txtProductName = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.txtPackID = new System.Windows.Forms.TextBox();
            this.txtBandId = new System.Windows.Forms.TextBox();
            this.txtPacket = new System.Windows.Forms.TextBox();
            this.txtBanderol = new System.Windows.Forms.TextBox();
            this.txtPCode = new System.Windows.Forms.TextBox();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBandeProduct)).BeginInit();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.progressBar1);
            this.groupBox2.Controls.Add(this.dgvBandeProduct);
            this.groupBox2.Location = new System.Drawing.Point(12, 106);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(565, 192);
            this.groupBox2.TabIndex = 190;
            this.groupBox2.TabStop = false;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(101, 79);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(250, 25);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 121;
            this.progressBar1.Visible = false;
            // 
            // dgvBandeProduct
            // 
            this.dgvBandeProduct.AllowUserToAddRows = false;
            this.dgvBandeProduct.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.dgvBandeProduct.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvBandeProduct.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvBandeProduct.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvBandeProduct.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvBandeProduct.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.BandProductId,
            this.ItemNo,
            this.ProductCode,
            this.ProductName,
            this.BanderolName,
            this.BanderolId,
            this.BanderolSize,
            this.BanderolUom,
            this.PackagingId,
            this.PackagingName,
            this.PackagingSize,
            this.PackagingUom,
            this.BUsedQty,
            this.txtActiveStatus,
            this.WastageQty,
            this.OpeningQty,
            this.OpeningDate});
            this.dgvBandeProduct.Location = new System.Drawing.Point(3, 11);
            this.dgvBandeProduct.Name = "dgvBandeProduct";
            this.dgvBandeProduct.RowHeadersVisible = false;
            this.dgvBandeProduct.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvBandeProduct.Size = new System.Drawing.Size(556, 174);
            this.dgvBandeProduct.TabIndex = 15;
            this.dgvBandeProduct.DoubleClick += new System.EventHandler(this.dgvBandeProduct_DoubleClick);
            // 
            // BandProductId
            // 
            this.BandProductId.HeaderText = "ID";
            this.BandProductId.Name = "BandProductId";
            this.BandProductId.Width = 62;
            // 
            // ItemNo
            // 
            this.ItemNo.HeaderText = "ItemNo";
            this.ItemNo.Name = "ItemNo";
            this.ItemNo.Width = 61;
            // 
            // ProductCode
            // 
            this.ProductCode.HeaderText = "Code";
            this.ProductCode.Name = "ProductCode";
            this.ProductCode.ReadOnly = true;
            this.ProductCode.Width = 62;
            // 
            // ProductName
            // 
            this.ProductName.HeaderText = "Product Name";
            this.ProductName.Name = "ProductName";
            this.ProductName.ReadOnly = true;
            this.ProductName.Width = 61;
            // 
            // BanderolName
            // 
            this.BanderolName.HeaderText = "Banderol Name";
            this.BanderolName.Name = "BanderolName";
            this.BanderolName.Width = 62;
            // 
            // BanderolId
            // 
            this.BanderolId.HeaderText = "Banderol Id";
            this.BanderolId.Name = "BanderolId";
            this.BanderolId.Visible = false;
            // 
            // BanderolSize
            // 
            this.BanderolSize.HeaderText = "Banderol Size";
            this.BanderolSize.Name = "BanderolSize";
            this.BanderolSize.Visible = false;
            // 
            // BanderolUom
            // 
            this.BanderolUom.HeaderText = "Banderol Uom";
            this.BanderolUom.Name = "BanderolUom";
            this.BanderolUom.Visible = false;
            // 
            // PackagingId
            // 
            this.PackagingId.HeaderText = "Packaging Id";
            this.PackagingId.Name = "PackagingId";
            this.PackagingId.Visible = false;
            // 
            // PackagingName
            // 
            this.PackagingName.HeaderText = "Packaging ";
            this.PackagingName.Name = "PackagingName";
            this.PackagingName.Width = 61;
            // 
            // PackagingSize
            // 
            this.PackagingSize.HeaderText = "Packaging Size";
            this.PackagingSize.Name = "PackagingSize";
            this.PackagingSize.Visible = false;
            // 
            // PackagingUom
            // 
            this.PackagingUom.HeaderText = "Packaging Uom";
            this.PackagingUom.Name = "PackagingUom";
            this.PackagingUom.Visible = false;
            // 
            // BUsedQty
            // 
            this.BUsedQty.HeaderText = "B Used Qty";
            this.BUsedQty.Name = "BUsedQty";
            this.BUsedQty.Width = 62;
            // 
            // txtActiveStatus
            // 
            this.txtActiveStatus.HeaderText = "Active Status";
            this.txtActiveStatus.Name = "txtActiveStatus";
            this.txtActiveStatus.ReadOnly = true;
            this.txtActiveStatus.Width = 61;
            // 
            // WastageQty
            // 
            this.WastageQty.HeaderText = "Wastage Qty";
            this.WastageQty.Name = "WastageQty";
            this.WastageQty.Width = 62;
            // 
            // OpeningQty
            // 
            this.OpeningQty.HeaderText = "Quantity";
            this.OpeningQty.Name = "OpeningQty";
            this.OpeningQty.ReadOnly = true;
            this.OpeningQty.Visible = false;
            // 
            // OpeningDate
            // 
            this.OpeningDate.HeaderText = "Opening Date";
            this.OpeningDate.Name = "OpeningDate";
            this.OpeningDate.ReadOnly = true;
            this.OpeningDate.Visible = false;
            // 
            // bgwSearch
            // 
            this.bgwSearch.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwSearch_DoWork);
            this.bgwSearch.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwSearch_RunWorkerCompleted);
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::VATClient.Properties.Resources.Green_Footer;
            this.panel1.Controls.Add(this.LRecordCount);
            this.panel1.Controls.Add(this.btnOk);
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Location = new System.Drawing.Point(5, 303);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(570, 40);
            this.panel1.TabIndex = 189;
            this.panel1.TabStop = true;
            // 
            // LRecordCount
            // 
            this.LRecordCount.AutoSize = true;
            this.LRecordCount.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LRecordCount.Location = new System.Drawing.Point(96, 13);
            this.LRecordCount.Name = "LRecordCount";
            this.LRecordCount.Size = new System.Drawing.Size(94, 16);
            this.LRecordCount.TabIndex = 226;
            this.LRecordCount.Text = "Record Count :";
            // 
            // btnOk
            // 
            this.btnOk.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnOk.Image = global::VATClient.Properties.Resources.Back;
            this.btnOk.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOk.Location = new System.Drawing.Point(475, 9);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 28);
            this.btnOk.TabIndex = 8;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = false;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnCancel.Image = global::VATClient.Properties.Resources.Referesh;
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.Location = new System.Drawing.Point(15, 7);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 28);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "&Refresh";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(295, 45);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(58, 13);
            this.label4.TabIndex = 77;
            this.label4.Text = "Packaging";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(14, 44);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(80, 13);
            this.label9.TabIndex = 187;
            this.label9.Text = "Banderol Name";
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSearch.Image = global::VATClient.Properties.Resources.search;
            this.btnSearch.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSearch.Location = new System.Drawing.Point(445, 64);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 28);
            this.btnSearch.TabIndex = 196;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // cmbActive
            // 
            this.cmbActive.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbActive.FormattingEnabled = true;
            this.cmbActive.Items.AddRange(new object[] {
            "Y",
            "N"});
            this.cmbActive.Location = new System.Drawing.Point(101, 64);
            this.cmbActive.Name = "cmbActive";
            this.cmbActive.Size = new System.Drawing.Size(61, 21);
            this.cmbActive.TabIndex = 211;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(14, 67);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(37, 13);
            this.label11.TabIndex = 212;
            this.label11.Text = "Active";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnSearchPackage);
            this.groupBox1.Controls.Add(this.btnSearchBanderol);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.txtProductName);
            this.groupBox1.Controls.Add(this.label17);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.txtPackID);
            this.groupBox1.Controls.Add(this.txtBandId);
            this.groupBox1.Controls.Add(this.txtPacket);
            this.groupBox1.Controls.Add(this.btnSearch);
            this.groupBox1.Controls.Add(this.txtBanderol);
            this.groupBox1.Controls.Add(this.txtPCode);
            this.groupBox1.Controls.Add(this.cmbActive);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Location = new System.Drawing.Point(12, 10);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(565, 95);
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            // 
            // btnSearchPackage
            // 
            this.btnSearchPackage.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSearchPackage.Image = global::VATClient.Properties.Resources.search;
            this.btnSearchPackage.Location = new System.Drawing.Point(529, 41);
            this.btnSearchPackage.Name = "btnSearchPackage";
            this.btnSearchPackage.Size = new System.Drawing.Size(30, 20);
            this.btnSearchPackage.TabIndex = 220;
            this.btnSearchPackage.UseVisualStyleBackColor = false;
            this.btnSearchPackage.Click += new System.EventHandler(this.btnSearchPackage_Click);
            // 
            // btnSearchBanderol
            // 
            this.btnSearchBanderol.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSearchBanderol.Image = global::VATClient.Properties.Resources.search;
            this.btnSearchBanderol.Location = new System.Drawing.Point(227, 41);
            this.btnSearchBanderol.Name = "btnSearchBanderol";
            this.btnSearchBanderol.Size = new System.Drawing.Size(30, 20);
            this.btnSearchBanderol.TabIndex = 219;
            this.btnSearchBanderol.UseVisualStyleBackColor = false;
            this.btnSearchBanderol.Click += new System.EventHandler(this.btnSearchBanderol_Click);
            // 
            // txtProductName
            // 
            this.txtProductName.Location = new System.Drawing.Point(376, 19);
            this.txtProductName.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtProductName.MinimumSize = new System.Drawing.Size(185, 20);
            this.txtProductName.Name = "txtProductName";
            this.txtProductName.Size = new System.Drawing.Size(185, 20);
            this.txtProductName.TabIndex = 203;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(294, 22);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(75, 13);
            this.label17.TabIndex = 208;
            this.label17.Text = "Product Name";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(14, 22);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(72, 13);
            this.label8.TabIndex = 209;
            this.label8.Text = "Product Code";
            // 
            // txtPackID
            // 
            this.txtPackID.Location = new System.Drawing.Point(177, 68);
            this.txtPackID.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtPackID.MinimumSize = new System.Drawing.Size(20, 20);
            this.txtPackID.Name = "txtPackID";
            this.txtPackID.Size = new System.Drawing.Size(20, 20);
            this.txtPackID.TabIndex = 205;
            this.txtPackID.Visible = false;
            // 
            // txtBandId
            // 
            this.txtBandId.Location = new System.Drawing.Point(303, 71);
            this.txtBandId.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtBandId.MinimumSize = new System.Drawing.Size(20, 20);
            this.txtBandId.Name = "txtBandId";
            this.txtBandId.Size = new System.Drawing.Size(20, 20);
            this.txtBandId.TabIndex = 205;
            this.txtBandId.Visible = false;
            // 
            // txtPacket
            // 
            this.txtPacket.Location = new System.Drawing.Point(376, 42);
            this.txtPacket.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtPacket.MinimumSize = new System.Drawing.Size(120, 20);
            this.txtPacket.Name = "txtPacket";
            this.txtPacket.Size = new System.Drawing.Size(120, 20);
            this.txtPacket.TabIndex = 205;
            // 
            // txtBanderol
            // 
            this.txtBanderol.Location = new System.Drawing.Point(101, 41);
            this.txtBanderol.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtBanderol.MinimumSize = new System.Drawing.Size(120, 20);
            this.txtBanderol.Name = "txtBanderol";
            this.txtBanderol.Size = new System.Drawing.Size(120, 20);
            this.txtBanderol.TabIndex = 204;
            // 
            // txtPCode
            // 
            this.txtPCode.Location = new System.Drawing.Point(101, 19);
            this.txtPCode.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtPCode.MinimumSize = new System.Drawing.Size(120, 20);
            this.txtPCode.Name = "txtPCode";
            this.txtPCode.Size = new System.Drawing.Size(120, 20);
            this.txtPCode.TabIndex = 202;
            // 
            // FormBanderolProductSearch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(584, 352);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.groupBox1);
            this.MaximizeBox = false;
            this.Name = "FormBanderolProductSearch";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Product Mapping- Banderol Search";
            this.Load += new System.EventHandler(this.FormBanderolProductSearch_Load);
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvBandeProduct)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.DataGridView dgvBandeProduct;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label LRecordCount;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.ComponentModel.BackgroundWorker bgwSearch;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.ComboBox cmbActive;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtPCode;
        private System.Windows.Forms.TextBox txtProductName;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TextBox txtBanderol;
        private System.Windows.Forms.TextBox txtPacket;
        private System.Windows.Forms.TextBox txtPackID;
        private System.Windows.Forms.TextBox txtBandId;
        private System.Windows.Forms.Button btnSearchPackage;
        private System.Windows.Forms.Button btnSearchBanderol;
        private System.Windows.Forms.DataGridViewTextBoxColumn BandProductId;
        private System.Windows.Forms.DataGridViewTextBoxColumn ItemNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn ProductCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn ProductName;
        private System.Windows.Forms.DataGridViewTextBoxColumn BanderolName;
        private System.Windows.Forms.DataGridViewTextBoxColumn BanderolId;
        private System.Windows.Forms.DataGridViewTextBoxColumn BanderolSize;
        private System.Windows.Forms.DataGridViewTextBoxColumn BanderolUom;
        private System.Windows.Forms.DataGridViewTextBoxColumn PackagingId;
        private System.Windows.Forms.DataGridViewTextBoxColumn PackagingName;
        private System.Windows.Forms.DataGridViewTextBoxColumn PackagingSize;
        private System.Windows.Forms.DataGridViewTextBoxColumn PackagingUom;
        private System.Windows.Forms.DataGridViewTextBoxColumn BUsedQty;
        private System.Windows.Forms.DataGridViewTextBoxColumn txtActiveStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn WastageQty;
        private System.Windows.Forms.DataGridViewTextBoxColumn OpeningQty;
        private System.Windows.Forms.DataGridViewTextBoxColumn OpeningDate;
    }
}