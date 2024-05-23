namespace VATClient
{
    partial class FormTraking
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormTraking));
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.label15 = new System.Windows.Forms.Label();
            this.txtVendorName = new System.Windows.Forms.TextBox();
            this.l15 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.dtpExpireToDate = new System.Windows.Forms.DateTimePicker();
            this.dtpExpireFromDate = new System.Windows.Forms.DateTimePicker();
            this.label13 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.dtpReceiveToDate = new System.Windows.Forms.DateTimePicker();
            this.dtpReceiveFromDate = new System.Windows.Forms.DateTimePicker();
            this.label9 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.dtpInvoiceTo = new System.Windows.Forms.DateTimePicker();
            this.dtpInvoiceFrom = new System.Windows.Forms.DateTimePicker();
            this.label6 = new System.Windows.Forms.Label();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.label2 = new System.Windows.Forms.Label();
            this.btnExport = new System.Windows.Forms.Button();
            this.label11 = new System.Windows.Forms.Label();
            this.dtpInvoiceToDate = new System.Windows.Forms.DateTimePicker();
            this.dtpInvoiceFromDate = new System.Windows.Forms.DateTimePicker();
            this.l = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtCustomer = new System.Windows.Forms.TextBox();
            this.label37 = new System.Windows.Forms.Label();
            this.tabSaleTraking = new System.Windows.Forms.TabControl();
            this.btnClose = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnPrintList = new System.Windows.Forms.Button();
            this.txtCustomerId = new System.Windows.Forms.TextBox();
            this.txtItemNo = new System.Windows.Forms.TextBox();
            this.txtProduct = new System.Windows.Forms.TextBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnRefreshP = new System.Windows.Forms.Button();
            this.txtProductNameP = new System.Windows.Forms.TextBox();
            this.txtItemNoP = new System.Windows.Forms.TextBox();
            this.txtVendorId = new System.Windows.Forms.TextBox();
            this.tabPage2.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabSaleTraking.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.tabPage2.Controls.Add(this.txtProductNameP);
            this.tabPage2.Controls.Add(this.txtItemNoP);
            this.tabPage2.Controls.Add(this.txtVendorId);
            this.tabPage2.Controls.Add(this.btnRefreshP);
            this.tabPage2.Controls.Add(this.label15);
            this.tabPage2.Controls.Add(this.txtVendorName);
            this.tabPage2.Controls.Add(this.l15);
            this.tabPage2.Controls.Add(this.button1);
            this.tabPage2.Controls.Add(this.label10);
            this.tabPage2.Controls.Add(this.label12);
            this.tabPage2.Controls.Add(this.dtpExpireToDate);
            this.tabPage2.Controls.Add(this.dtpExpireFromDate);
            this.tabPage2.Controls.Add(this.label13);
            this.tabPage2.Controls.Add(this.label7);
            this.tabPage2.Controls.Add(this.label8);
            this.tabPage2.Controls.Add(this.dtpReceiveToDate);
            this.tabPage2.Controls.Add(this.dtpReceiveFromDate);
            this.tabPage2.Controls.Add(this.label9);
            this.tabPage2.Controls.Add(this.label3);
            this.tabPage2.Controls.Add(this.label4);
            this.tabPage2.Controls.Add(this.dtpInvoiceTo);
            this.tabPage2.Controls.Add(this.dtpInvoiceFrom);
            this.tabPage2.Controls.Add(this.label6);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(528, 279);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "PurchaseTraking";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(27, 49);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(52, 13);
            this.label15.TabIndex = 312;
            this.label15.Text = "Item (F9)";
            // 
            // txtVendorName
            // 
            this.txtVendorName.Location = new System.Drawing.Point(101, 16);
            this.txtVendorName.Name = "txtVendorName";
            this.txtVendorName.ReadOnly = true;
            this.txtVendorName.Size = new System.Drawing.Size(249, 21);
            this.txtVendorName.TabIndex = 308;
            this.txtVendorName.DoubleClick += new System.EventHandler(this.txtVendorName_DoubleClick);
            this.txtVendorName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtVendorName_KeyDown);
            // 
            // l15
            // 
            this.l15.AutoSize = true;
            this.l15.Location = new System.Drawing.Point(21, 20);
            this.l15.Name = "l15";
            this.l15.Size = new System.Drawing.Size(61, 13);
            this.l15.TabIndex = 309;
            this.l15.Text = "Vendor(F9)";
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.White;
            this.button1.Image = global::VATClient.Properties.Resources.Load;
            this.button1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button1.Location = new System.Drawing.Point(107, 211);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(91, 31);
            this.button1.TabIndex = 306;
            this.button1.Text = "Export";
            this.button1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label10
            // 
            this.label10.Font = new System.Drawing.Font("Tahoma", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.ForeColor = System.Drawing.Color.Red;
            this.label10.Location = new System.Drawing.Point(95, 163);
            this.label10.MaximumSize = new System.Drawing.Size(10, 10);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(10, 10);
            this.label10.TabIndex = 305;
            this.label10.Text = "*";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(226, 163);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(17, 13);
            this.label12.TabIndex = 304;
            this.label12.Text = "to";
            // 
            // dtpExpireToDate
            // 
            this.dtpExpireToDate.Checked = false;
            this.dtpExpireToDate.CustomFormat = "dd/MMM/yyyy";
            this.dtpExpireToDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpExpireToDate.Location = new System.Drawing.Point(249, 161);
            this.dtpExpireToDate.Name = "dtpExpireToDate";
            this.dtpExpireToDate.Size = new System.Drawing.Size(101, 21);
            this.dtpExpireToDate.TabIndex = 303;
            // 
            // dtpExpireFromDate
            // 
            this.dtpExpireFromDate.Checked = false;
            this.dtpExpireFromDate.CustomFormat = "dd/MMM/yyyy";
            this.dtpExpireFromDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpExpireFromDate.Location = new System.Drawing.Point(107, 159);
            this.dtpExpireFromDate.Name = "dtpExpireFromDate";
            this.dtpExpireFromDate.Size = new System.Drawing.Size(101, 21);
            this.dtpExpireFromDate.TabIndex = 302;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(21, 165);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(63, 13);
            this.label13.TabIndex = 301;
            this.label13.Text = "Expire Date";
            // 
            // label7
            // 
            this.label7.Font = new System.Drawing.Font("Tahoma", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.Color.Red;
            this.label7.Location = new System.Drawing.Point(91, 119);
            this.label7.MaximumSize = new System.Drawing.Size(10, 10);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(10, 10);
            this.label7.TabIndex = 300;
            this.label7.Text = "*";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(226, 118);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(17, 13);
            this.label8.TabIndex = 299;
            this.label8.Text = "to";
            // 
            // dtpReceiveToDate
            // 
            this.dtpReceiveToDate.Checked = false;
            this.dtpReceiveToDate.CustomFormat = "dd/MMM/yyyy";
            this.dtpReceiveToDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpReceiveToDate.Location = new System.Drawing.Point(249, 112);
            this.dtpReceiveToDate.Name = "dtpReceiveToDate";
            this.dtpReceiveToDate.Size = new System.Drawing.Size(101, 21);
            this.dtpReceiveToDate.TabIndex = 298;
            // 
            // dtpReceiveFromDate
            // 
            this.dtpReceiveFromDate.Checked = false;
            this.dtpReceiveFromDate.CustomFormat = "dd/MMM/yyyy";
            this.dtpReceiveFromDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpReceiveFromDate.Location = new System.Drawing.Point(107, 112);
            this.dtpReceiveFromDate.Name = "dtpReceiveFromDate";
            this.dtpReceiveFromDate.Size = new System.Drawing.Size(101, 21);
            this.dtpReceiveFromDate.TabIndex = 297;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(21, 118);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(71, 13);
            this.label9.TabIndex = 296;
            this.label9.Text = "Receive Date";
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Tahoma", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.Red;
            this.label3.Location = new System.Drawing.Point(91, 80);
            this.label3.MaximumSize = new System.Drawing.Size(10, 10);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(10, 10);
            this.label3.TabIndex = 295;
            this.label3.Text = "*";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(226, 79);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(17, 13);
            this.label4.TabIndex = 294;
            this.label4.Text = "to";
            // 
            // dtpInvoiceTo
            // 
            this.dtpInvoiceTo.Checked = false;
            this.dtpInvoiceTo.CustomFormat = "dd/MMM/yyyy";
            this.dtpInvoiceTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpInvoiceTo.Location = new System.Drawing.Point(249, 73);
            this.dtpInvoiceTo.Name = "dtpInvoiceTo";
            this.dtpInvoiceTo.Size = new System.Drawing.Size(101, 21);
            this.dtpInvoiceTo.TabIndex = 293;
            // 
            // dtpInvoiceFrom
            // 
            this.dtpInvoiceFrom.Checked = false;
            this.dtpInvoiceFrom.CustomFormat = "dd/MMM/yyyy";
            this.dtpInvoiceFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpInvoiceFrom.Location = new System.Drawing.Point(107, 73);
            this.dtpInvoiceFrom.Name = "dtpInvoiceFrom";
            this.dtpInvoiceFrom.Size = new System.Drawing.Size(101, 21);
            this.dtpInvoiceFrom.TabIndex = 292;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(21, 79);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(68, 13);
            this.label6.TabIndex = 291;
            this.label6.Text = "Invoice Date";
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.tabPage1.Controls.Add(this.btnCancel);
            this.tabPage1.Controls.Add(this.txtProduct);
            this.tabPage1.Controls.Add(this.txtItemNo);
            this.tabPage1.Controls.Add(this.txtCustomerId);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.btnExport);
            this.tabPage1.Controls.Add(this.label11);
            this.tabPage1.Controls.Add(this.dtpInvoiceToDate);
            this.tabPage1.Controls.Add(this.dtpInvoiceFromDate);
            this.tabPage1.Controls.Add(this.l);
            this.tabPage1.Controls.Add(this.label5);
            this.tabPage1.Controls.Add(this.txtCustomer);
            this.tabPage1.Controls.Add(this.label37);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(528, 279);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "SaleTraking";
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Tahoma", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Red;
            this.label2.Location = new System.Drawing.Point(81, 89);
            this.label2.MaximumSize = new System.Drawing.Size(10, 10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(10, 10);
            this.label2.TabIndex = 290;
            this.label2.Text = "*";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnExport
            // 
            this.btnExport.BackColor = System.Drawing.Color.White;
            this.btnExport.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnExport.Image = global::VATClient.Properties.Resources.Load;
            this.btnExport.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnExport.Location = new System.Drawing.Point(97, 123);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(91, 31);
            this.btnExport.TabIndex = 227;
            this.btnExport.Text = "Export";
            this.btnExport.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnExport.UseVisualStyleBackColor = false;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(216, 84);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(17, 13);
            this.label11.TabIndex = 289;
            this.label11.Text = "to";
            // 
            // dtpInvoiceToDate
            // 
            this.dtpInvoiceToDate.Checked = false;
            this.dtpInvoiceToDate.CustomFormat = "dd/MMM/yyyy";
            this.dtpInvoiceToDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpInvoiceToDate.Location = new System.Drawing.Point(239, 80);
            this.dtpInvoiceToDate.Name = "dtpInvoiceToDate";
            this.dtpInvoiceToDate.Size = new System.Drawing.Size(101, 21);
            this.dtpInvoiceToDate.TabIndex = 288;
            // 
            // dtpInvoiceFromDate
            // 
            this.dtpInvoiceFromDate.Checked = false;
            this.dtpInvoiceFromDate.CustomFormat = "dd/MMM/yyyy";
            this.dtpInvoiceFromDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpInvoiceFromDate.Location = new System.Drawing.Point(97, 80);
            this.dtpInvoiceFromDate.Name = "dtpInvoiceFromDate";
            this.dtpInvoiceFromDate.Size = new System.Drawing.Size(101, 21);
            this.dtpInvoiceFromDate.TabIndex = 287;
            // 
            // l
            // 
            this.l.AutoSize = true;
            this.l.Location = new System.Drawing.Point(11, 84);
            this.l.Name = "l";
            this.l.Size = new System.Drawing.Size(68, 13);
            this.l.TabIndex = 286;
            this.l.Text = "Invoice Date";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(23, 47);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(52, 13);
            this.label5.TabIndex = 284;
            this.label5.Text = "Item (F9)";
            // 
            // txtCustomer
            // 
            this.txtCustomer.Location = new System.Drawing.Point(97, 12);
            this.txtCustomer.MaximumSize = new System.Drawing.Size(350, 20);
            this.txtCustomer.MinimumSize = new System.Drawing.Size(125, 20);
            this.txtCustomer.Name = "txtCustomer";
            this.txtCustomer.ReadOnly = true;
            this.txtCustomer.Size = new System.Drawing.Size(243, 20);
            this.txtCustomer.TabIndex = 225;
            this.txtCustomer.DoubleClick += new System.EventHandler(this.txtCustomer_DoubleClick);
            this.txtCustomer.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtCustomer_KeyDown);
            // 
            // label37
            // 
            this.label37.AutoSize = true;
            this.label37.Location = new System.Drawing.Point(6, 16);
            this.label37.Name = "label37";
            this.label37.Size = new System.Drawing.Size(76, 13);
            this.label37.TabIndex = 224;
            this.label37.Text = "Customer (F9)";
            // 
            // tabSaleTraking
            // 
            this.tabSaleTraking.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabSaleTraking.Controls.Add(this.tabPage1);
            this.tabSaleTraking.Controls.Add(this.tabPage2);
            this.tabSaleTraking.Location = new System.Drawing.Point(4, 12);
            this.tabSaleTraking.Name = "tabSaleTraking";
            this.tabSaleTraking.SelectedIndex = 0;
            this.tabSaleTraking.Size = new System.Drawing.Size(536, 305);
            this.tabSaleTraking.TabIndex = 32;
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Image = ((System.Drawing.Image)(resources.GetObject("btnClose.Image")));
            this.btnClose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnClose.Location = new System.Drawing.Point(441, 7);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 28);
            this.btnClose.TabIndex = 57;
            this.btnClose.Text = "&Close";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel1.BackgroundImage")));
            this.panel1.Controls.Add(this.progressBar1);
            this.panel1.Controls.Add(this.btnPrintList);
            this.panel1.Controls.Add(this.btnClose);
            this.panel1.Location = new System.Drawing.Point(1, 320);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(539, 41);
            this.panel1.TabIndex = 49;
            // 
            // btnPrintList
            // 
            this.btnPrintList.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnPrintList.Image = ((System.Drawing.Image)(resources.GetObject("btnPrintList.Image")));
            this.btnPrintList.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPrintList.Location = new System.Drawing.Point(373, 59);
            this.btnPrintList.Name = "btnPrintList";
            this.btnPrintList.Size = new System.Drawing.Size(75, 28);
            this.btnPrintList.TabIndex = 4;
            this.btnPrintList.Text = "&List";
            this.btnPrintList.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnPrintList.UseVisualStyleBackColor = false;
            // 
            // txtCustomerId
            // 
            this.txtCustomerId.Location = new System.Drawing.Point(366, 9);
            this.txtCustomerId.MaximumSize = new System.Drawing.Size(350, 20);
            this.txtCustomerId.MinimumSize = new System.Drawing.Size(125, 20);
            this.txtCustomerId.Name = "txtCustomerId";
            this.txtCustomerId.ReadOnly = true;
            this.txtCustomerId.Size = new System.Drawing.Size(125, 20);
            this.txtCustomerId.TabIndex = 291;
            this.txtCustomerId.Visible = false;
            // 
            // txtItemNo
            // 
            this.txtItemNo.Location = new System.Drawing.Point(366, 43);
            this.txtItemNo.MaximumSize = new System.Drawing.Size(350, 20);
            this.txtItemNo.MinimumSize = new System.Drawing.Size(125, 20);
            this.txtItemNo.Name = "txtItemNo";
            this.txtItemNo.ReadOnly = true;
            this.txtItemNo.Size = new System.Drawing.Size(125, 20);
            this.txtItemNo.TabIndex = 292;
            this.txtItemNo.Visible = false;
            // 
            // txtProduct
            // 
            this.txtProduct.Location = new System.Drawing.Point(97, 43);
            this.txtProduct.MaximumSize = new System.Drawing.Size(350, 20);
            this.txtProduct.MinimumSize = new System.Drawing.Size(125, 20);
            this.txtProduct.Name = "txtProduct";
            this.txtProduct.ReadOnly = true;
            this.txtProduct.Size = new System.Drawing.Size(243, 20);
            this.txtProduct.TabIndex = 293;
            this.txtProduct.DoubleClick += new System.EventHandler(this.txtProduct_DoubleClick);
            this.txtProduct.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtProduct_KeyDown);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(47, 9);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(300, 22);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 213;
            this.progressBar1.Visible = false;
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnCancel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancel.Image = global::VATClient.Properties.Resources.Referesh;
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.Location = new System.Drawing.Point(234, 123);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(91, 31);
            this.btnCancel.TabIndex = 294;
            this.btnCancel.Text = "&Refresh";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnRefreshP
            // 
            this.btnRefreshP.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnRefreshP.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRefreshP.Image = global::VATClient.Properties.Resources.Referesh;
            this.btnRefreshP.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnRefreshP.Location = new System.Drawing.Point(246, 211);
            this.btnRefreshP.Name = "btnRefreshP";
            this.btnRefreshP.Size = new System.Drawing.Size(91, 31);
            this.btnRefreshP.TabIndex = 314;
            this.btnRefreshP.Text = "&Refresh";
            this.btnRefreshP.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnRefreshP.UseVisualStyleBackColor = false;
            this.btnRefreshP.Click += new System.EventHandler(this.btnRefreshP_Click);
            // 
            // txtProductNameP
            // 
            this.txtProductNameP.Location = new System.Drawing.Point(101, 45);
            this.txtProductNameP.MaximumSize = new System.Drawing.Size(350, 20);
            this.txtProductNameP.MinimumSize = new System.Drawing.Size(125, 20);
            this.txtProductNameP.Name = "txtProductNameP";
            this.txtProductNameP.ReadOnly = true;
            this.txtProductNameP.Size = new System.Drawing.Size(249, 20);
            this.txtProductNameP.TabIndex = 317;
            this.txtProductNameP.DoubleClick += new System.EventHandler(this.txtProductNameP_DoubleClick);
            this.txtProductNameP.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtProductNameP_KeyDown);
            // 
            // txtItemNoP
            // 
            this.txtItemNoP.Location = new System.Drawing.Point(384, 40);
            this.txtItemNoP.MaximumSize = new System.Drawing.Size(350, 20);
            this.txtItemNoP.MinimumSize = new System.Drawing.Size(125, 20);
            this.txtItemNoP.Name = "txtItemNoP";
            this.txtItemNoP.ReadOnly = true;
            this.txtItemNoP.Size = new System.Drawing.Size(125, 20);
            this.txtItemNoP.TabIndex = 316;
            this.txtItemNoP.Visible = false;
            // 
            // txtVendorId
            // 
            this.txtVendorId.Location = new System.Drawing.Point(384, 6);
            this.txtVendorId.MaximumSize = new System.Drawing.Size(350, 20);
            this.txtVendorId.MinimumSize = new System.Drawing.Size(125, 20);
            this.txtVendorId.Name = "txtVendorId";
            this.txtVendorId.ReadOnly = true;
            this.txtVendorId.Size = new System.Drawing.Size(125, 20);
            this.txtVendorId.TabIndex = 315;
            this.txtVendorId.Visible = false;
            // 
            // FormTraking
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(544, 361);
            this.Controls.Add(this.tabSaleTraking);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(800, 450);
            this.MinimumSize = new System.Drawing.Size(400, 250);
            this.Name = "FormTraking";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Traking";
            this.Load += new System.EventHandler(this.FormTraking_Load);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabSaleTraking.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnPrintList;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabControl tabSaleTraking;
        private System.Windows.Forms.TextBox txtCustomer;
        private System.Windows.Forms.Label label37;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.DateTimePicker dtpInvoiceToDate;
        private System.Windows.Forms.DateTimePicker dtpInvoiceFromDate;
        private System.Windows.Forms.Label l;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.DateTimePicker dtpExpireToDate;
        private System.Windows.Forms.DateTimePicker dtpExpireFromDate;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.DateTimePicker dtpReceiveToDate;
        private System.Windows.Forms.DateTimePicker dtpReceiveFromDate;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DateTimePicker dtpInvoiceTo;
        private System.Windows.Forms.DateTimePicker dtpInvoiceFrom;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtVendorName;
        private System.Windows.Forms.Label l15;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox txtCustomerId;
        private System.Windows.Forms.TextBox txtItemNo;
        private System.Windows.Forms.TextBox txtProduct;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnRefreshP;
        private System.Windows.Forms.TextBox txtProductNameP;
        private System.Windows.Forms.TextBox txtItemNoP;
        private System.Windows.Forms.TextBox txtVendorId;
    }
}