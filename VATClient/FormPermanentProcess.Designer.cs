namespace VATClient
{
    partial class FormPermanentProcess
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
            this.label11 = new System.Windows.Forms.Label();
            this.dtpToDate = new System.Windows.Forms.DateTimePicker();
            this.dtpDate = new System.Windows.Forms.DateTimePicker();
            this.grbTransactionHistory = new System.Windows.Forms.GroupBox();
            this.grpDeleteProcess = new System.Windows.Forms.GroupBox();
            this.btnDbAll = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.btnReProcess = new System.Windows.Forms.Button();
            this.btnProcess = new System.Windows.Forms.Button();
            this.dtpIssueFromDate = new System.Windows.Forms.DateTimePicker();
            this.label4 = new System.Windows.Forms.Label();
            this.grpProcess = new System.Windows.Forms.GroupBox();
            this.rbtn6_2_1 = new System.Windows.Forms.RadioButton();
            this.rbtn6_2Neg = new System.Windows.Forms.RadioButton();
            this.rbtn6_1Neg = new System.Windows.Forms.RadioButton();
            this.rtnBoth = new System.Windows.Forms.RadioButton();
            this.rbtn6_2 = new System.Windows.Forms.RadioButton();
            this.rbtn6_1 = new System.Windows.Forms.RadioButton();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.cmbToMonth = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbFromMonth = new System.Windows.Forms.ComboBox();
            this.chkProduct = new System.Windows.Forms.CheckBox();
            this.txtProName = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.rbtn6_2_1del = new System.Windows.Forms.RadioButton();
            this.rbtnBothDelete = new System.Windows.Forms.RadioButton();
            this.rbtn6_2Delete = new System.Windows.Forms.RadioButton();
            this.rbtn6_1Delete = new System.Windows.Forms.RadioButton();
            this.btnDelete = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.dtpFromDate = new System.Windows.Forms.DateTimePicker();
            this.btnDownload = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.btnStartProcess = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.LRecordCount = new System.Windows.Forms.Label();
            this.btnOk = new System.Windows.Forms.Button();
            this.txtItemNo = new System.Windows.Forms.TextBox();
            this.txtProductCode = new System.Windows.Forms.TextBox();
            this.bgwUpdate = new System.ComponentModel.BackgroundWorker();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.bgwDeleteProcess = new System.ComponentModel.BackgroundWorker();
            this.bgw6_2Process = new System.ComponentModel.BackgroundWorker();
            this.bgwBothProcess = new System.ComponentModel.BackgroundWorker();
            this.bgwVAT6_2delete = new System.ComponentModel.BackgroundWorker();
            this.bgwBothDelete = new System.ComponentModel.BackgroundWorker();
            this.bgwVAT6_2_1Process = new System.ComponentModel.BackgroundWorker();
            this.bgw6_2_1 = new System.ComponentModel.BackgroundWorker();
            this.bgwAvg = new System.ComponentModel.BackgroundWorker();
            this.bgwRefresh = new System.ComponentModel.BackgroundWorker();
            this.bgwFreshStock = new System.ComponentModel.BackgroundWorker();
            this.cachedRptDutyDrawBack1 = new SymphonySofttech.Reports.Report.CachedRptDutyDrawBack();
            this.grbTransactionHistory.SuspendLayout();
            this.grpDeleteProcess.SuspendLayout();
            this.grpProcess.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(325, 65);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(80, 21);
            this.label11.TabIndex = 112;
            this.label11.Text = "To Month";
            // 
            // dtpToDate
            // 
            this.dtpToDate.Checked = false;
            this.dtpToDate.CustomFormat = "dd/MMM/yyyy";
            this.dtpToDate.Enabled = false;
            this.dtpToDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpToDate.Location = new System.Drawing.Point(399, 338);
            this.dtpToDate.Name = "dtpToDate";
            this.dtpToDate.Size = new System.Drawing.Size(103, 24);
            this.dtpToDate.TabIndex = 3;
            this.dtpToDate.Visible = false;
            // 
            // dtpDate
            // 
            this.dtpDate.Checked = false;
            this.dtpDate.CustomFormat = "MMM-yyyy";
            this.dtpDate.Font = new System.Drawing.Font("Tahoma", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpDate.Location = new System.Drawing.Point(546, -26);
            this.dtpDate.Name = "dtpDate";
            this.dtpDate.Size = new System.Drawing.Size(173, 29);
            this.dtpDate.TabIndex = 2;
            this.dtpDate.Visible = false;
            this.dtpDate.ValueChanged += new System.EventHandler(this.dtpIssueFromDate_ValueChanged);
            // 
            // grbTransactionHistory
            // 
            this.grbTransactionHistory.Controls.Add(this.grpDeleteProcess);
            this.grbTransactionHistory.Controls.Add(this.grpProcess);
            this.grbTransactionHistory.Controls.Add(this.cmbToMonth);
            this.grbTransactionHistory.Controls.Add(this.label1);
            this.grbTransactionHistory.Controls.Add(this.cmbFromMonth);
            this.grbTransactionHistory.Controls.Add(this.label11);
            this.grbTransactionHistory.Controls.Add(this.chkProduct);
            this.grbTransactionHistory.Controls.Add(this.txtProName);
            this.grbTransactionHistory.Controls.Add(this.label10);
            this.grbTransactionHistory.Location = new System.Drawing.Point(2, 7);
            this.grbTransactionHistory.Name = "grbTransactionHistory";
            this.grbTransactionHistory.Size = new System.Drawing.Size(777, 304);
            this.grbTransactionHistory.TabIndex = 115;
            this.grbTransactionHistory.TabStop = false;
            this.grbTransactionHistory.Text = "Prosess";
            // 
            // grpDeleteProcess
            // 
            this.grpDeleteProcess.Controls.Add(this.btnDbAll);
            this.grpDeleteProcess.Controls.Add(this.button1);
            this.grpDeleteProcess.Controls.Add(this.btnReProcess);
            this.grpDeleteProcess.Controls.Add(this.btnProcess);
            this.grpDeleteProcess.Controls.Add(this.dtpIssueFromDate);
            this.grpDeleteProcess.Controls.Add(this.label4);
            this.grpDeleteProcess.Location = new System.Drawing.Point(397, 99);
            this.grpDeleteProcess.Name = "grpDeleteProcess";
            this.grpDeleteProcess.Size = new System.Drawing.Size(365, 162);
            this.grpDeleteProcess.TabIndex = 577;
            this.grpDeleteProcess.TabStop = false;
            this.grpDeleteProcess.Text = "Avg process";
            // 
            // btnDbAll
            // 
            this.btnDbAll.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnDbAll.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDbAll.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDbAll.Location = new System.Drawing.Point(156, 107);
            this.btnDbAll.Margin = new System.Windows.Forms.Padding(4);
            this.btnDbAll.Name = "btnDbAll";
            this.btnDbAll.Size = new System.Drawing.Size(131, 44);
            this.btnDbAll.TabIndex = 576;
            this.btnDbAll.Text = "DB Migration All ";
            this.btnDbAll.UseVisualStyleBackColor = false;
            this.btnDbAll.Visible = false;
            this.btnDbAll.Click += new System.EventHandler(this.btnDbAll_Click);
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.button1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button1.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Image = global::VATClient.Properties.Resources.Referesh;
            this.button1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button1.Location = new System.Drawing.Point(9, 58);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(137, 42);
            this.button1.TabIndex = 15;
            this.button1.Text = "Fresh Stock Process";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnReProcess
            // 
            this.btnReProcess.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.btnReProcess.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnReProcess.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnReProcess.Image = global::VATClient.Properties.Resources.Referesh;
            this.btnReProcess.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnReProcess.Location = new System.Drawing.Point(156, 58);
            this.btnReProcess.Name = "btnReProcess";
            this.btnReProcess.Size = new System.Drawing.Size(131, 42);
            this.btnReProcess.TabIndex = 14;
            this.btnReProcess.Text = "Re-Process";
            this.btnReProcess.UseVisualStyleBackColor = false;
            this.btnReProcess.Click += new System.EventHandler(this.btnReProcess_Click);
            // 
            // btnProcess
            // 
            this.btnProcess.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.btnProcess.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnProcess.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnProcess.Image = global::VATClient.Properties.Resources.Referesh;
            this.btnProcess.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnProcess.Location = new System.Drawing.Point(10, 107);
            this.btnProcess.Name = "btnProcess";
            this.btnProcess.Size = new System.Drawing.Size(139, 40);
            this.btnProcess.TabIndex = 13;
            this.btnProcess.Text = "Process";
            this.btnProcess.UseVisualStyleBackColor = false;
            this.btnProcess.Visible = false;
            this.btnProcess.Click += new System.EventHandler(this.btnProcess_Click);
            // 
            // dtpIssueFromDate
            // 
            this.dtpIssueFromDate.Checked = false;
            this.dtpIssueFromDate.CustomFormat = "dd/MMM/yyyy";
            this.dtpIssueFromDate.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpIssueFromDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpIssueFromDate.Location = new System.Drawing.Point(106, 17);
            this.dtpIssueFromDate.Name = "dtpIssueFromDate";
            this.dtpIssueFromDate.Size = new System.Drawing.Size(137, 26);
            this.dtpIssueFromDate.TabIndex = 11;
            this.dtpIssueFromDate.Visible = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(2, 23);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(78, 18);
            this.label4.TabIndex = 12;
            this.label4.Text = "From Date";
            this.label4.Visible = false;
            // 
            // grpProcess
            // 
            this.grpProcess.Controls.Add(this.rbtn6_2_1);
            this.grpProcess.Controls.Add(this.rbtn6_2Neg);
            this.grpProcess.Controls.Add(this.rbtn6_1Neg);
            this.grpProcess.Controls.Add(this.rtnBoth);
            this.grpProcess.Controls.Add(this.rbtn6_2);
            this.grpProcess.Controls.Add(this.rbtn6_1);
            this.grpProcess.Controls.Add(this.btnUpdate);
            this.grpProcess.Location = new System.Drawing.Point(13, 99);
            this.grpProcess.Name = "grpProcess";
            this.grpProcess.Size = new System.Drawing.Size(372, 162);
            this.grpProcess.TabIndex = 576;
            this.grpProcess.TabStop = false;
            this.grpProcess.Text = "Permanent Process";
            // 
            // rbtn6_2_1
            // 
            this.rbtn6_2_1.AutoSize = true;
            this.rbtn6_2_1.Location = new System.Drawing.Point(8, 50);
            this.rbtn6_2_1.Name = "rbtn6_2_1";
            this.rbtn6_2_1.Size = new System.Drawing.Size(120, 21);
            this.rbtn6_2_1.TabIndex = 15;
            this.rbtn6_2_1.TabStop = true;
            this.rbtn6_2_1.Text = "6_2_1 Process";
            this.rbtn6_2_1.UseVisualStyleBackColor = true;
            this.rbtn6_2_1.CheckedChanged += new System.EventHandler(this.rbtn6_2_1_CheckedChanged);
            // 
            // rbtn6_2Neg
            // 
            this.rbtn6_2Neg.AutoSize = true;
            this.rbtn6_2Neg.Location = new System.Drawing.Point(187, 73);
            this.rbtn6_2Neg.Name = "rbtn6_2Neg";
            this.rbtn6_2Neg.Size = new System.Drawing.Size(175, 21);
            this.rbtn6_2Neg.TabIndex = 14;
            this.rbtn6_2Neg.TabStop = true;
            this.rbtn6_2Neg.Text = "6_2 Negative Download";
            this.rbtn6_2Neg.UseVisualStyleBackColor = true;
            // 
            // rbtn6_1Neg
            // 
            this.rbtn6_1Neg.AutoSize = true;
            this.rbtn6_1Neg.Location = new System.Drawing.Point(8, 73);
            this.rbtn6_1Neg.Name = "rbtn6_1Neg";
            this.rbtn6_1Neg.Size = new System.Drawing.Size(175, 21);
            this.rbtn6_1Neg.TabIndex = 13;
            this.rbtn6_1Neg.TabStop = true;
            this.rbtn6_1Neg.Text = "6_1 Negative Download";
            this.rbtn6_1Neg.UseVisualStyleBackColor = true;
            // 
            // rtnBoth
            // 
            this.rtnBoth.AutoSize = true;
            this.rtnBoth.Location = new System.Drawing.Point(253, 23);
            this.rtnBoth.Name = "rtnBoth";
            this.rtnBoth.Size = new System.Drawing.Size(109, 21);
            this.rtnBoth.TabIndex = 12;
            this.rtnBoth.TabStop = true;
            this.rtnBoth.Text = "Both Process";
            this.rtnBoth.UseVisualStyleBackColor = true;
            // 
            // rbtn6_2
            // 
            this.rbtn6_2.AutoSize = true;
            this.rbtn6_2.Location = new System.Drawing.Point(139, 23);
            this.rbtn6_2.Name = "rbtn6_2";
            this.rbtn6_2.Size = new System.Drawing.Size(104, 21);
            this.rbtn6_2.TabIndex = 11;
            this.rbtn6_2.TabStop = true;
            this.rbtn6_2.Text = "6_2 Process";
            this.rbtn6_2.UseVisualStyleBackColor = true;
            // 
            // rbtn6_1
            // 
            this.rbtn6_1.AutoSize = true;
            this.rbtn6_1.Location = new System.Drawing.Point(6, 23);
            this.rbtn6_1.Name = "rbtn6_1";
            this.rbtn6_1.Size = new System.Drawing.Size(104, 21);
            this.rbtn6_1.TabIndex = 10;
            this.rbtn6_1.TabStop = true;
            this.rbtn6_1.Text = "6_1 Process";
            this.rbtn6_1.UseVisualStyleBackColor = true;
            // 
            // btnUpdate
            // 
            this.btnUpdate.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.btnUpdate.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnUpdate.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnUpdate.Image = global::VATClient.Properties.Resources.Referesh;
            this.btnUpdate.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnUpdate.Location = new System.Drawing.Point(76, 101);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(200, 50);
            this.btnUpdate.TabIndex = 9;
            this.btnUpdate.Text = "Process";
            this.btnUpdate.UseVisualStyleBackColor = false;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // cmbToMonth
            // 
            this.cmbToMonth.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbToMonth.FormattingEnabled = true;
            this.cmbToMonth.Location = new System.Drawing.Point(422, 64);
            this.cmbToMonth.Name = "cmbToMonth";
            this.cmbToMonth.Size = new System.Drawing.Size(121, 29);
            this.cmbToMonth.TabIndex = 575;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(85, 68);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(99, 21);
            this.label1.TabIndex = 574;
            this.label1.Text = "From Month";
            // 
            // cmbFromMonth
            // 
            this.cmbFromMonth.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbFromMonth.FormattingEnabled = true;
            this.cmbFromMonth.Location = new System.Drawing.Point(191, 64);
            this.cmbFromMonth.Name = "cmbFromMonth";
            this.cmbFromMonth.Size = new System.Drawing.Size(121, 29);
            this.cmbFromMonth.TabIndex = 572;
            // 
            // chkProduct
            // 
            this.chkProduct.AutoSize = true;
            this.chkProduct.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkProduct.Location = new System.Drawing.Point(413, 19);
            this.chkProduct.Name = "chkProduct";
            this.chkProduct.Size = new System.Drawing.Size(127, 25);
            this.chkProduct.TabIndex = 562;
            this.chkProduct.Text = "With Product";
            this.chkProduct.UseVisualStyleBackColor = true;
            this.chkProduct.CheckedChanged += new System.EventHandler(this.chkProduct_CheckedChanged);
            // 
            // txtProName
            // 
            this.txtProName.Location = new System.Drawing.Point(209, 17);
            this.txtProName.Margin = new System.Windows.Forms.Padding(4);
            this.txtProName.MaximumSize = new System.Drawing.Size(265, 20);
            this.txtProName.MinimumSize = new System.Drawing.Size(199, 20);
            this.txtProName.Name = "txtProName";
            this.txtProName.Size = new System.Drawing.Size(199, 24);
            this.txtProName.TabIndex = 561;
            this.txtProName.DoubleClick += new System.EventHandler(this.txtProName_DoubleClick);
            this.txtProName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtProName_KeyDown);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(62, 20);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(146, 21);
            this.label10.TabIndex = 560;
            this.label10.Text = "Product Name(F9)";
            // 
            // rbtn6_2_1del
            // 
            this.rbtn6_2_1del.AutoSize = true;
            this.rbtn6_2_1del.Location = new System.Drawing.Point(803, 254);
            this.rbtn6_2_1del.Name = "rbtn6_2_1del";
            this.rbtn6_2_1del.Size = new System.Drawing.Size(120, 21);
            this.rbtn6_2_1del.TabIndex = 575;
            this.rbtn6_2_1del.TabStop = true;
            this.rbtn6_2_1del.Text = "6_2_1 Process";
            this.rbtn6_2_1del.UseVisualStyleBackColor = true;
            // 
            // rbtnBothDelete
            // 
            this.rbtnBothDelete.AutoSize = true;
            this.rbtnBothDelete.Location = new System.Drawing.Point(1035, 214);
            this.rbtnBothDelete.Name = "rbtnBothDelete";
            this.rbtnBothDelete.Size = new System.Drawing.Size(109, 21);
            this.rbtnBothDelete.TabIndex = 574;
            this.rbtnBothDelete.TabStop = true;
            this.rbtnBothDelete.Text = "Both Process";
            this.rbtnBothDelete.UseVisualStyleBackColor = true;
            // 
            // rbtn6_2Delete
            // 
            this.rbtn6_2Delete.AutoSize = true;
            this.rbtn6_2Delete.Location = new System.Drawing.Point(921, 214);
            this.rbtn6_2Delete.Name = "rbtn6_2Delete";
            this.rbtn6_2Delete.Size = new System.Drawing.Size(104, 21);
            this.rbtn6_2Delete.TabIndex = 573;
            this.rbtn6_2Delete.TabStop = true;
            this.rbtn6_2Delete.Text = "6_2 Process";
            this.rbtn6_2Delete.UseVisualStyleBackColor = true;
            // 
            // rbtn6_1Delete
            // 
            this.rbtn6_1Delete.AutoSize = true;
            this.rbtn6_1Delete.Location = new System.Drawing.Point(800, 214);
            this.rbtn6_1Delete.Name = "rbtn6_1Delete";
            this.rbtn6_1Delete.Size = new System.Drawing.Size(104, 21);
            this.rbtn6_1Delete.TabIndex = 572;
            this.rbtn6_1Delete.TabStop = true;
            this.rbtn6_1Delete.Text = "6_1 Process";
            this.rbtn6_1Delete.UseVisualStyleBackColor = true;
            // 
            // btnDelete
            // 
            this.btnDelete.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.btnDelete.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDelete.Image = global::VATClient.Properties.Resources.Referesh;
            this.btnDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDelete.Location = new System.Drawing.Point(847, 294);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(207, 57);
            this.btnDelete.TabIndex = 571;
            this.btnDelete.Text = "&Delete Process";
            this.btnDelete.UseVisualStyleBackColor = false;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(489, -5);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 21);
            this.label2.TabIndex = 573;
            this.label2.Text = "To";
            this.label2.Visible = false;
            // 
            // dtpFromDate
            // 
            this.dtpFromDate.Checked = false;
            this.dtpFromDate.CustomFormat = "dd/MMM/yyyy";
            this.dtpFromDate.Enabled = false;
            this.dtpFromDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpFromDate.Location = new System.Drawing.Point(524, 3);
            this.dtpFromDate.Name = "dtpFromDate";
            this.dtpFromDate.Size = new System.Drawing.Size(103, 24);
            this.dtpFromDate.TabIndex = 572;
            this.dtpFromDate.Visible = false;
            // 
            // btnDownload
            // 
            this.btnDownload.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.btnDownload.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDownload.Image = global::VATClient.Properties.Resources.Print;
            this.btnDownload.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDownload.Location = new System.Drawing.Point(195, 0);
            this.btnDownload.Name = "btnDownload";
            this.btnDownload.Size = new System.Drawing.Size(150, 57);
            this.btnDownload.TabIndex = 570;
            this.btnDownload.Text = "Download Negative Data";
            this.btnDownload.UseVisualStyleBackColor = false;
            this.btnDownload.Visible = false;
            this.btnDownload.Click += new System.EventHandler(this.btnDownload_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(417, 1);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 21);
            this.label3.TabIndex = 3;
            this.label3.Text = "Month";
            this.label3.Visible = false;
            // 
            // btnStartProcess
            // 
            this.btnStartProcess.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.btnStartProcess.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStartProcess.Image = global::VATClient.Properties.Resources.Referesh;
            this.btnStartProcess.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnStartProcess.Location = new System.Drawing.Point(434, 25);
            this.btnStartProcess.Name = "btnStartProcess";
            this.btnStartProcess.Size = new System.Drawing.Size(159, 32);
            this.btnStartProcess.TabIndex = 10;
            this.btnStartProcess.Text = "Re-Process";
            this.btnStartProcess.UseVisualStyleBackColor = false;
            this.btnStartProcess.Visible = false;
            this.btnStartProcess.Click += new System.EventHandler(this.btnStartProcess_Click);
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::VATClient.Properties.Resources.Green_Footer;
            this.panel1.Controls.Add(this.LRecordCount);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.btnStartProcess);
            this.panel1.Controls.Add(this.btnDownload);
            this.panel1.Controls.Add(this.btnOk);
            this.panel1.Controls.Add(this.dtpFromDate);
            this.panel1.Controls.Add(this.dtpDate);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Location = new System.Drawing.Point(2, 374);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(777, 50);
            this.panel1.TabIndex = 11;
            this.panel1.TabStop = true;
            // 
            // LRecordCount
            // 
            this.LRecordCount.AutoSize = true;
            this.LRecordCount.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LRecordCount.Location = new System.Drawing.Point(4, 12);
            this.LRecordCount.Name = "LRecordCount";
            this.LRecordCount.Size = new System.Drawing.Size(121, 21);
            this.LRecordCount.TabIndex = 221;
            this.LRecordCount.Text = "Record Count :";
            this.LRecordCount.Visible = false;
            // 
            // btnOk
            // 
            this.btnOk.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnOk.Image = global::VATClient.Properties.Resources.Back;
            this.btnOk.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOk.Location = new System.Drawing.Point(677, 3);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(97, 44);
            this.btnOk.TabIndex = 13;
            this.btnOk.Text = "&Ok";
            this.btnOk.UseVisualStyleBackColor = false;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // txtItemNo
            // 
            this.txtItemNo.Location = new System.Drawing.Point(331, 394);
            this.txtItemNo.Margin = new System.Windows.Forms.Padding(4);
            this.txtItemNo.MaximumSize = new System.Drawing.Size(265, 20);
            this.txtItemNo.MinimumSize = new System.Drawing.Size(199, 20);
            this.txtItemNo.Name = "txtItemNo";
            this.txtItemNo.Size = new System.Drawing.Size(199, 24);
            this.txtItemNo.TabIndex = 562;
            this.txtItemNo.Visible = false;
            // 
            // txtProductCode
            // 
            this.txtProductCode.Location = new System.Drawing.Point(252, 404);
            this.txtProductCode.Margin = new System.Windows.Forms.Padding(4);
            this.txtProductCode.MaximumSize = new System.Drawing.Size(265, 20);
            this.txtProductCode.MinimumSize = new System.Drawing.Size(199, 20);
            this.txtProductCode.Name = "txtProductCode";
            this.txtProductCode.Size = new System.Drawing.Size(199, 24);
            this.txtProductCode.TabIndex = 563;
            this.txtProductCode.Visible = false;
            // 
            // bgwUpdate
            // 
            this.bgwUpdate.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwUpdate_DoWork);
            this.bgwUpdate.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwUpdate_RunWorkerCompleted);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(10, 317);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(752, 45);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 211;
            this.progressBar1.Visible = false;
            // 
            // bgwDeleteProcess
            // 
            this.bgwDeleteProcess.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwDeleteProcess_DoWork);
            this.bgwDeleteProcess.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwDeleteProcess_RunWorkerCompleted);
            // 
            // bgw6_2Process
            // 
            this.bgw6_2Process.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgw6_2Process_DoWork);
            this.bgw6_2Process.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgw6_2Process_RunWorkerCompleted);
            // 
            // bgwBothProcess
            // 
            this.bgwBothProcess.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwBothProcess_DoWork);
            this.bgwBothProcess.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwBothProcess_RunWorkerCompleted);
            // 
            // bgwVAT6_2delete
            // 
            this.bgwVAT6_2delete.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwVAT6_2delete_DoWork);
            this.bgwVAT6_2delete.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwVAT6_2delete_RunWorkerCompleted);
            // 
            // bgwBothDelete
            // 
            this.bgwBothDelete.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwBothDelete_DoWork);
            this.bgwBothDelete.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwBothDelete_RunWorkerCompleted);
            // 
            // bgwVAT6_2_1Process
            // 
            this.bgwVAT6_2_1Process.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwVAT6_2_1_DoWork);
            this.bgwVAT6_2_1Process.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwVAT6_2_1Process_RunWorkerCompleted);
            // 
            // bgw6_2_1
            // 
            this.bgw6_2_1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgw6_2_1_DoWork);
            this.bgw6_2_1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgw6_2_1_RunWorkerCompleted);
            // 
            // bgwAvg
            // 
            this.bgwAvg.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwAvg_DoWork);
            this.bgwAvg.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwAvg_RunWorkerCompleted);
            // 
            // bgwRefresh
            // 
            this.bgwRefresh.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwRefresh_DoWork);
            this.bgwRefresh.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwRefresh_RunWorkerCompleted);
            // 
            // bgwFreshStock
            // 
            this.bgwFreshStock.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwFreshStock_DoWork);
            this.bgwFreshStock.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwFreshStock_RunWorkerCompleted);
            // 
            // FormPermanentProcess
            // 
            this.AcceptButton = this.btnUpdate;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.CancelButton = this.btnOk;
            this.ClientSize = new System.Drawing.Size(782, 427);
            this.Controls.Add(this.rbtn6_2_1del);
            this.Controls.Add(this.txtProductCode);
            this.Controls.Add(this.rbtnBothDelete);
            this.Controls.Add(this.txtItemNo);
            this.Controls.Add(this.rbtn6_2Delete);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.rbtn6_1Delete);
            this.Controls.Add(this.dtpToDate);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.grbTransactionHistory);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(800, 500);
            this.MinimizeBox = false;
            this.Name = "FormPermanentProcess";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "6.1 & 6.2 Permanent Process";
            this.Load += new System.EventHandler(this.FormIssueMultiple_Load);
            this.grbTransactionHistory.ResumeLayout(false);
            this.grbTransactionHistory.PerformLayout();
            this.grpDeleteProcess.ResumeLayout(false);
            this.grpDeleteProcess.PerformLayout();
            this.grpProcess.ResumeLayout(false);
            this.grpProcess.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.DateTimePicker dtpToDate;
        private System.Windows.Forms.DateTimePicker dtpDate;
        private System.Windows.Forms.GroupBox grbTransactionHistory;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.Label LRecordCount;
        private System.ComponentModel.BackgroundWorker bgwUpdate;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button btnStartProcess;
        public System.Windows.Forms.TextBox txtProName;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.CheckBox chkProduct;
        public System.Windows.Forms.TextBox txtItemNo;
        public System.Windows.Forms.TextBox txtProductCode;
        private System.Windows.Forms.Button btnDownload;
        private System.Windows.Forms.Button btnDelete;
        private System.ComponentModel.BackgroundWorker bgwDeleteProcess;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker dtpFromDate;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbToMonth;
        private System.Windows.Forms.ComboBox cmbFromMonth;
        private System.Windows.Forms.GroupBox grpDeleteProcess;
        private System.Windows.Forms.GroupBox grpProcess;
        private System.ComponentModel.BackgroundWorker bgw6_2Process;
        private System.ComponentModel.BackgroundWorker bgwBothProcess;
        private System.ComponentModel.BackgroundWorker bgwVAT6_2delete;
        private System.ComponentModel.BackgroundWorker bgwBothDelete;
        private System.Windows.Forms.RadioButton rtnBoth;
        private System.Windows.Forms.RadioButton rbtn6_2;
        private System.Windows.Forms.RadioButton rbtn6_1;
        private System.Windows.Forms.RadioButton rbtn6_1Neg;
        private System.Windows.Forms.RadioButton rbtn6_2Neg;
        private System.Windows.Forms.RadioButton rbtnBothDelete;
        private System.Windows.Forms.RadioButton rbtn6_2Delete;
        private System.Windows.Forms.RadioButton rbtn6_1Delete;
        private System.Windows.Forms.RadioButton rbtn6_2_1;
        private System.ComponentModel.BackgroundWorker bgwVAT6_2_1Process;
        private System.Windows.Forms.RadioButton rbtn6_2_1del;
        private System.ComponentModel.BackgroundWorker bgw6_2_1;
        private System.Windows.Forms.Button btnReProcess;
        private System.Windows.Forms.Button btnProcess;
        private System.Windows.Forms.DateTimePicker dtpIssueFromDate;
        private System.Windows.Forms.Label label4;
        private SymphonySofttech.Reports.Report.CachedRptDutyDrawBack cachedRptDutyDrawBack1;
        private System.ComponentModel.BackgroundWorker bgwAvg;
        private System.ComponentModel.BackgroundWorker bgwRefresh;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnDbAll;
        private System.ComponentModel.BackgroundWorker bgwFreshStock;
    }
}