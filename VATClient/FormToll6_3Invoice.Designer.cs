namespace VATClient
{
    partial class FormToll6_3Invoice
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormToll6_3Invoice));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgvSale = new System.Windows.Forms.DataGridView();
            this.txtCustomer = new System.Windows.Forms.TextBox();
            this.label37 = new System.Windows.Forms.Label();
            this.txtTollNo = new System.Windows.Forms.TextBox();
            this.txtCustomerAddress = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.dtpTollDate = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.txtCustomerID = new System.Windows.Forms.TextBox();
            this.label23 = new System.Windows.Forms.Label();
            this.txtComments = new System.Windows.Forms.TextBox();
            this.btnTollNo = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.btnPost = new System.Windows.Forms.Button();
            this.btnToll6_3 = new System.Windows.Forms.Button();
            this.btnDebitCredit = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSearch = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnAddNew = new System.Windows.Forms.Button();
            this.txtPost = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rbtnContractor63 = new System.Windows.Forms.RadioButton();
            this.rbtnClient63 = new System.Windows.Forms.RadioButton();
            this.TollLineNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SalesInvoiceNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.InvoiceDateTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSale)).BeginInit();
            this.panel1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvSale
            // 
            this.dgvSale.AllowUserToAddRows = false;
            this.dgvSale.AllowUserToOrderColumns = true;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Blue;
            this.dgvSale.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvSale.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvSale.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvSale.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSale.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.TollLineNo,
            this.SalesInvoiceNo,
            this.InvoiceDateTime});
            this.dgvSale.GridColor = System.Drawing.SystemColors.HotTrack;
            this.dgvSale.Location = new System.Drawing.Point(29, 82);
            this.dgvSale.MultiSelect = false;
            this.dgvSale.Name = "dgvSale";
            this.dgvSale.RowHeadersVisible = false;
            this.dgvSale.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvSale.Size = new System.Drawing.Size(609, 251);
            this.dgvSale.TabIndex = 244;
            // 
            // txtCustomer
            // 
            this.txtCustomer.Location = new System.Drawing.Point(113, 26);
            this.txtCustomer.MaximumSize = new System.Drawing.Size(350, 20);
            this.txtCustomer.MinimumSize = new System.Drawing.Size(125, 20);
            this.txtCustomer.Name = "txtCustomer";
            this.txtCustomer.ReadOnly = true;
            this.txtCustomer.Size = new System.Drawing.Size(242, 20);
            this.txtCustomer.TabIndex = 255;
            // 
            // label37
            // 
            this.label37.AutoSize = true;
            this.label37.Location = new System.Drawing.Point(27, 29);
            this.label37.Name = "label37";
            this.label37.Size = new System.Drawing.Size(51, 13);
            this.label37.TabIndex = 254;
            this.label37.Text = "Customer";
            // 
            // txtTollNo
            // 
            this.txtTollNo.BackColor = System.Drawing.SystemColors.Control;
            this.txtTollNo.Location = new System.Drawing.Point(113, 4);
            this.txtTollNo.MaximumSize = new System.Drawing.Size(4, 25);
            this.txtTollNo.MinimumSize = new System.Drawing.Size(150, 20);
            this.txtTollNo.Name = "txtTollNo";
            this.txtTollNo.ReadOnly = true;
            this.txtTollNo.Size = new System.Drawing.Size(150, 20);
            this.txtTollNo.TabIndex = 248;
            this.txtTollNo.TabStop = false;
            // 
            // txtCustomerAddress
            // 
            this.txtCustomerAddress.Location = new System.Drawing.Point(113, 48);
            this.txtCustomerAddress.MaximumSize = new System.Drawing.Size(400, 21);
            this.txtCustomerAddress.Name = "txtCustomerAddress";
            this.txtCustomerAddress.Size = new System.Drawing.Size(303, 20);
            this.txtCustomerAddress.TabIndex = 249;
            this.txtCustomerAddress.TabStop = false;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(27, 52);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(45, 13);
            this.label15.TabIndex = 252;
            this.label15.Text = "Address";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(27, 8);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 13);
            this.label2.TabIndex = 250;
            this.label2.Text = "Toll No";
            // 
            // dtpTollDate
            // 
            this.dtpTollDate.CustomFormat = "dd/MMM/yyyy HH:mm";
            this.dtpTollDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpTollDate.Location = new System.Drawing.Point(524, 4);
            this.dtpTollDate.MaximumSize = new System.Drawing.Size(135, 20);
            this.dtpTollDate.MinimumSize = new System.Drawing.Size(135, 20);
            this.dtpTollDate.Name = "dtpTollDate";
            this.dtpTollDate.Size = new System.Drawing.Size(135, 20);
            this.dtpTollDate.TabIndex = 256;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(459, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 13);
            this.label1.TabIndex = 250;
            this.label1.Text = "Toll Date";
            // 
            // txtCustomerID
            // 
            this.txtCustomerID.Location = new System.Drawing.Point(616, 26);
            this.txtCustomerID.MaximumSize = new System.Drawing.Size(350, 20);
            this.txtCustomerID.MinimumSize = new System.Drawing.Size(20, 20);
            this.txtCustomerID.Name = "txtCustomerID";
            this.txtCustomerID.ReadOnly = true;
            this.txtCustomerID.Size = new System.Drawing.Size(43, 20);
            this.txtCustomerID.TabIndex = 255;
            this.txtCustomerID.Visible = false;
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(31, 339);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(56, 13);
            this.label23.TabIndex = 258;
            this.label23.Text = "Comments";
            // 
            // txtComments
            // 
            this.txtComments.BackColor = System.Drawing.SystemColors.Window;
            this.txtComments.Location = new System.Drawing.Point(91, 339);
            this.txtComments.MaximumSize = new System.Drawing.Size(400, 30);
            this.txtComments.MinimumSize = new System.Drawing.Size(400, 30);
            this.txtComments.Multiline = true;
            this.txtComments.Name = "txtComments";
            this.txtComments.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtComments.Size = new System.Drawing.Size(400, 30);
            this.txtComments.TabIndex = 257;
            this.txtComments.TabStop = false;
            // 
            // btnTollNo
            // 
            this.btnTollNo.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnTollNo.Image = ((System.Drawing.Image)(resources.GetObject("btnTollNo.Image")));
            this.btnTollNo.Location = new System.Drawing.Point(269, 3);
            this.btnTollNo.Name = "btnTollNo";
            this.btnTollNo.Size = new System.Drawing.Size(30, 20);
            this.btnTollNo.TabIndex = 63;
            this.btnTollNo.TabStop = false;
            this.btnTollNo.UseVisualStyleBackColor = false;
            this.btnTollNo.Click += new System.EventHandler(this.btnTollNo_Click);
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::VATClient.Properties.Resources.Green_Footer;
            this.panel1.Controls.Add(this.btnUpdate);
            this.panel1.Controls.Add(this.btnPost);
            this.panel1.Controls.Add(this.btnToll6_3);
            this.panel1.Controls.Add(this.btnDebitCredit);
            this.panel1.Controls.Add(this.button2);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.btnClose);
            this.panel1.Controls.Add(this.btnSave);
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Location = new System.Drawing.Point(7, 389);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(652, 40);
            this.panel1.TabIndex = 247;
            // 
            // btnUpdate
            // 
            this.btnUpdate.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnUpdate.Image = global::VATClient.Properties.Resources.Update;
            this.btnUpdate.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnUpdate.Location = new System.Drawing.Point(66, 5);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(60, 28);
            this.btnUpdate.TabIndex = 51;
            this.btnUpdate.Text = "&Update";
            this.btnUpdate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnUpdate.UseVisualStyleBackColor = false;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // btnPost
            // 
            this.btnPost.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnPost.Image = global::VATClient.Properties.Resources.Post;
            this.btnPost.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPost.Location = new System.Drawing.Point(127, 5);
            this.btnPost.Name = "btnPost";
            this.btnPost.Size = new System.Drawing.Size(60, 28);
            this.btnPost.TabIndex = 52;
            this.btnPost.Text = "&Post";
            this.btnPost.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnPost.UseVisualStyleBackColor = false;
            this.btnPost.Click += new System.EventHandler(this.btnPost_Click);
            // 
            // btnToll6_3
            // 
            this.btnToll6_3.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnToll6_3.Image = global::VATClient.Properties.Resources.Print;
            this.btnToll6_3.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnToll6_3.Location = new System.Drawing.Point(431, 5);
            this.btnToll6_3.Name = "btnToll6_3";
            this.btnToll6_3.Size = new System.Drawing.Size(71, 28);
            this.btnToll6_3.TabIndex = 59;
            this.btnToll6_3.Text = "  Toll 6.3 ";
            this.btnToll6_3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnToll6_3.UseVisualStyleBackColor = false;
            this.btnToll6_3.Click += new System.EventHandler(this.btnToll6_3_Click);
            // 
            // btnDebitCredit
            // 
            this.btnDebitCredit.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnDebitCredit.Image = global::VATClient.Properties.Resources.Print;
            this.btnDebitCredit.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDebitCredit.Location = new System.Drawing.Point(289, 54);
            this.btnDebitCredit.Name = "btnDebitCredit";
            this.btnDebitCredit.Size = new System.Drawing.Size(90, 28);
            this.btnDebitCredit.TabIndex = 23;
            this.btnDebitCredit.Text = "VAT 12 Ka";
            this.btnDebitCredit.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnDebitCredit.UseVisualStyleBackColor = false;
            this.btnDebitCredit.Visible = false;
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.button2.Image = global::VATClient.Properties.Resources.Print;
            this.button2.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button2.Location = new System.Drawing.Point(431, 71);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(110, 28);
            this.button2.TabIndex = 22;
            this.button2.Text = "Report(Grid)";
            this.button2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button2.UseVisualStyleBackColor = false;
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.button1.Image = global::VATClient.Properties.Resources.Print;
            this.button1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button1.Location = new System.Drawing.Point(329, 71);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(102, 28);
            this.button1.TabIndex = 21;
            this.button1.Text = "Report List";
            this.button1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button1.UseVisualStyleBackColor = false;
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Image = global::VATClient.Properties.Resources.Back;
            this.btnClose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnClose.Location = new System.Drawing.Point(571, 4);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(60, 28);
            this.btnClose.TabIndex = 62;
            this.btnClose.Text = "&Close";
            this.btnClose.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnClose.UseVisualStyleBackColor = false;
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSave.Image = global::VATClient.Properties.Resources.Save;
            this.btnSave.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSave.Location = new System.Drawing.Point(5, 5);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(60, 28);
            this.btnSave.TabIndex = 50;
            this.btnSave.Text = "&Add";
            this.btnSave.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnCancel.Image = global::VATClient.Properties.Resources.Referesh;
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.Location = new System.Drawing.Point(96, 59);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 28);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "&Refresh";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancel.UseVisualStyleBackColor = false;
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSearch.Image = global::VATClient.Properties.Resources.search;
            this.btnSearch.Location = new System.Drawing.Point(562, 51);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(37, 25);
            this.btnSearch.TabIndex = 246;
            this.btnSearch.TabStop = false;
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnRemove.Image = global::VATClient.Properties.Resources.Remove;
            this.btnRemove.Location = new System.Drawing.Point(603, 51);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(37, 25);
            this.btnRemove.TabIndex = 245;
            this.btnRemove.TabStop = false;
            this.btnRemove.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnRemove.UseVisualStyleBackColor = false;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnAddNew
            // 
            this.btnAddNew.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnAddNew.Image = ((System.Drawing.Image)(resources.GetObject("btnAddNew.Image")));
            this.btnAddNew.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnAddNew.Location = new System.Drawing.Point(304, 3);
            this.btnAddNew.Name = "btnAddNew";
            this.btnAddNew.Size = new System.Drawing.Size(30, 20);
            this.btnAddNew.TabIndex = 259;
            this.btnAddNew.TabStop = false;
            this.btnAddNew.UseVisualStyleBackColor = false;
            this.btnAddNew.Click += new System.EventHandler(this.btnAddNew_Click);
            // 
            // txtPost
            // 
            this.txtPost.Location = new System.Drawing.Point(567, 26);
            this.txtPost.MaximumSize = new System.Drawing.Size(350, 20);
            this.txtPost.MinimumSize = new System.Drawing.Size(20, 20);
            this.txtPost.Name = "txtPost";
            this.txtPost.ReadOnly = true;
            this.txtPost.Size = new System.Drawing.Size(43, 20);
            this.txtPost.TabIndex = 260;
            this.txtPost.Visible = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rbtnContractor63);
            this.groupBox2.Controls.Add(this.rbtnClient63);
            this.groupBox2.Location = new System.Drawing.Point(555, 331);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(104, 56);
            this.groupBox2.TabIndex = 261;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "groupBox2";
            this.groupBox2.Visible = false;
            // 
            // rbtnContractor63
            // 
            this.rbtnContractor63.AutoSize = true;
            this.rbtnContractor63.Location = new System.Drawing.Point(8, 18);
            this.rbtnContractor63.Name = "rbtnContractor63";
            this.rbtnContractor63.Size = new System.Drawing.Size(86, 17);
            this.rbtnContractor63.TabIndex = 37;
            this.rbtnContractor63.TabStop = true;
            this.rbtnContractor63.Text = "Contractor63";
            this.rbtnContractor63.UseVisualStyleBackColor = true;
            // 
            // rbtnClient63
            // 
            this.rbtnClient63.AutoSize = true;
            this.rbtnClient63.Location = new System.Drawing.Point(8, 36);
            this.rbtnClient63.Name = "rbtnClient63";
            this.rbtnClient63.Size = new System.Drawing.Size(63, 17);
            this.rbtnClient63.TabIndex = 38;
            this.rbtnClient63.TabStop = true;
            this.rbtnClient63.Text = "Client63";
            this.rbtnClient63.UseVisualStyleBackColor = true;
            // 
            // TollLineNo
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TollLineNo.DefaultCellStyle = dataGridViewCellStyle3;
            this.TollLineNo.FillWeight = 50F;
            this.TollLineNo.Frozen = true;
            this.TollLineNo.HeaderText = "Line No";
            this.TollLineNo.Name = "TollLineNo";
            this.TollLineNo.ReadOnly = true;
            this.TollLineNo.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.TollLineNo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.TollLineNo.Width = 55;
            // 
            // SalesInvoiceNo
            // 
            this.SalesInvoiceNo.HeaderText = "Invoice No";
            this.SalesInvoiceNo.Name = "SalesInvoiceNo";
            this.SalesInvoiceNo.ReadOnly = true;
            this.SalesInvoiceNo.Width = 150;
            // 
            // InvoiceDateTime
            // 
            this.InvoiceDateTime.HeaderText = "Invoice Date";
            this.InvoiceDateTime.Name = "InvoiceDateTime";
            this.InvoiceDateTime.ReadOnly = true;
            // 
            // FormToll6_3Invoice
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(666, 435);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.txtPost);
            this.Controls.Add(this.btnAddNew);
            this.Controls.Add(this.btnTollNo);
            this.Controls.Add(this.label23);
            this.Controls.Add(this.txtComments);
            this.Controls.Add(this.dtpTollDate);
            this.Controls.Add(this.txtCustomerID);
            this.Controls.Add(this.txtCustomer);
            this.Controls.Add(this.label37);
            this.Controls.Add(this.txtTollNo);
            this.Controls.Add(this.txtCustomerAddress);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.dgvSale);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormToll6_3Invoice";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Toll 6.3 Invoice";
            this.Load += new System.EventHandler(this.FormToll6_3Invoice_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSale)).EndInit();
            this.panel1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.DataGridView dgvSale;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnToll6_3;
        public System.Windows.Forms.Button btnDebitCredit;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TextBox txtCustomer;
        private System.Windows.Forms.Label label37;
        private System.Windows.Forms.TextBox txtTollNo;
        private System.Windows.Forms.TextBox txtCustomerAddress;
        private System.Windows.Forms.Label label15;
        public System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker dtpTollDate;
        public System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtCustomerID;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.TextBox txtComments;
        private System.Windows.Forms.Button btnTollNo;
        private System.Windows.Forms.Button btnAddNew;
        private System.Windows.Forms.TextBox txtPost;
        public System.Windows.Forms.Button btnUpdate;
        public System.Windows.Forms.Button btnPost;
        public System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.GroupBox groupBox2;
        public System.Windows.Forms.RadioButton rbtnContractor63;
        public System.Windows.Forms.RadioButton rbtnClient63;
        private System.Windows.Forms.DataGridViewTextBoxColumn TollLineNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn SalesInvoiceNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn InvoiceDateTime;
    }
}