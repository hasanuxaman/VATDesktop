namespace VATClient
{
    partial class FormAdjustment
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAdjustment));
            this.label2 = new System.Windows.Forms.Label();
            this.txtAdjAmount = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbAdjType = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtAdjDescription = new System.Windows.Forms.TextBox();
            this.dtpAdjDate = new System.Windows.Forms.DateTimePicker();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.backgroundWorkerAdd = new System.ComponentModel.BackgroundWorker();
            this.backgroundWorkerUpdate = new System.ComponentModel.BackgroundWorker();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.txtAdjHistoryID = new System.Windows.Forms.TextBox();
            this.bgwLoad = new System.ComponentModel.BackgroundWorker();
            this.label6 = new System.Windows.Forms.Label();
            this.txtInputAmount = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtInputPercent = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtAdjReferance = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtAdjHistoryNo = new System.Windows.Forms.TextBox();
            this.bgwPost = new System.ComponentModel.BackgroundWorker();
            this.btnAddNew = new System.Windows.Forms.Button();
            this.btnSearch = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnVAT18 = new System.Windows.Forms.Button();
            this.btnPost = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.btnPrint = new System.Windows.Forms.Button();
            this.btnPrintGrid = new System.Windows.Forms.Button();
            this.btnPrintList = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.txtAdjHead = new System.Windows.Forms.TextBox();
            this.txtAdjId = new System.Windows.Forms.TextBox();
            this.label52 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.chkSD = new System.Windows.Forms.CheckBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(31, 135);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(80, 17);
            this.label2.TabIndex = 13;
            this.label2.Text = "Adj Amount";
            // 
            // txtAdjAmount
            // 
            this.txtAdjAmount.Location = new System.Drawing.Point(137, 130);
            this.txtAdjAmount.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtAdjAmount.MaximumSize = new System.Drawing.Size(159, 200);
            this.txtAdjAmount.MinimumSize = new System.Drawing.Size(159, 20);
            this.txtAdjAmount.Name = "txtAdjAmount";
            this.txtAdjAmount.ReadOnly = true;
            this.txtAdjAmount.Size = new System.Drawing.Size(159, 22);
            this.txtAdjAmount.TabIndex = 8;
            this.txtAdjAmount.TabStop = false;
            this.txtAdjAmount.Leave += new System.EventHandler(this.txtAdjAmount_Leave);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(31, 37);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 17);
            this.label1.TabIndex = 15;
            this.label1.Text = "Name (F9)";
            // 
            // cmbAdjType
            // 
            this.cmbAdjType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAdjType.FormattingEnabled = true;
            this.cmbAdjType.Items.AddRange(new object[] {
            "DecreasingAdjustment",
            "IncreasingAdjustment"});
            this.cmbAdjType.Location = new System.Drawing.Point(404, 65);
            this.cmbAdjType.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmbAdjType.Name = "cmbAdjType";
            this.cmbAdjType.Size = new System.Drawing.Size(159, 24);
            this.cmbAdjType.Sorted = true;
            this.cmbAdjType.TabIndex = 1;
            this.cmbAdjType.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmbAdjType_KeyDown);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(31, 180);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(103, 17);
            this.label3.TabIndex = 175;
            this.label3.Text = "Adj Description";
            // 
            // txtAdjDescription
            // 
            this.txtAdjDescription.Location = new System.Drawing.Point(137, 162);
            this.txtAdjDescription.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtAdjDescription.MaximumSize = new System.Drawing.Size(4, 24);
            this.txtAdjDescription.MinimumSize = new System.Drawing.Size(425, 48);
            this.txtAdjDescription.Multiline = true;
            this.txtAdjDescription.Name = "txtAdjDescription";
            this.txtAdjDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtAdjDescription.Size = new System.Drawing.Size(425, 48);
            this.txtAdjDescription.TabIndex = 10;
            this.txtAdjDescription.TabStop = false;
            this.txtAdjDescription.TextChanged += new System.EventHandler(this.txtAdjDescription_TextChanged);
            // 
            // dtpAdjDate
            // 
            this.dtpAdjDate.CustomFormat = "dd/MMM/yyyy";
            this.dtpAdjDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpAdjDate.Location = new System.Drawing.Point(137, 65);
            this.dtpAdjDate.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.dtpAdjDate.MaxDate = new System.DateTime(9900, 1, 1, 0, 0, 0, 0);
            this.dtpAdjDate.MaximumSize = new System.Drawing.Size(4, 20);
            this.dtpAdjDate.MinDate = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
            this.dtpAdjDate.MinimumSize = new System.Drawing.Size(159, 20);
            this.dtpAdjDate.Name = "dtpAdjDate";
            this.dtpAdjDate.Size = new System.Drawing.Size(159, 20);
            this.dtpAdjDate.TabIndex = 0;
            this.dtpAdjDate.Value = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
            this.dtpAdjDate.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dtpAdjDate_KeyDown);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(327, 70);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(64, 17);
            this.label4.TabIndex = 176;
            this.label4.Text = "Adj Type";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(31, 70);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(62, 17);
            this.label5.TabIndex = 178;
            this.label5.Text = "Adj Date";
            // 
            // backgroundWorkerAdd
            // 
            this.backgroundWorkerAdd.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerAdd_DoWork);
            this.backgroundWorkerAdd.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerAdd_RunWorkerCompleted);
            // 
            // backgroundWorkerUpdate
            // 
            this.backgroundWorkerUpdate.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerUpdate_DoWork);
            this.backgroundWorkerUpdate.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerUpdate_RunWorkerCompleted);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(155, 237);
            this.progressBar1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(300, 17);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 185;
            this.progressBar1.Visible = false;
            // 
            // txtAdjHistoryID
            // 
            this.txtAdjHistoryID.Location = new System.Drawing.Point(463, 219);
            this.txtAdjHistoryID.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtAdjHistoryID.MaximumSize = new System.Drawing.Size(159, 200);
            this.txtAdjHistoryID.MinimumSize = new System.Drawing.Size(159, 20);
            this.txtAdjHistoryID.Name = "txtAdjHistoryID";
            this.txtAdjHistoryID.Size = new System.Drawing.Size(159, 22);
            this.txtAdjHistoryID.TabIndex = 11;
            this.txtAdjHistoryID.TabStop = false;
            this.txtAdjHistoryID.Visible = false;
            // 
            // bgwLoad
            // 
            this.bgwLoad.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwLoad_DoWork);
            this.bgwLoad.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwLoad_RunWorkerCompleted);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(31, 103);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(91, 17);
            this.label6.TabIndex = 188;
            this.label6.Text = "Input Amount";
            // 
            // txtInputAmount
            // 
            this.txtInputAmount.Location = new System.Drawing.Point(137, 98);
            this.txtInputAmount.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtInputAmount.MaximumSize = new System.Drawing.Size(159, 200);
            this.txtInputAmount.MinimumSize = new System.Drawing.Size(159, 20);
            this.txtInputAmount.Name = "txtInputAmount";
            this.txtInputAmount.Size = new System.Drawing.Size(159, 22);
            this.txtInputAmount.TabIndex = 6;
            this.txtInputAmount.TextChanged += new System.EventHandler(this.txtInputAmount_TextChanged);
            this.txtInputAmount.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtInputAmount_KeyDown);
            this.txtInputAmount.Leave += new System.EventHandler(this.txtInputAmount_Leave);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(367, 103);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(30, 17);
            this.label7.TabIndex = 190;
            this.label7.Text = "(%)";
            // 
            // txtInputPercent
            // 
            this.txtInputPercent.Location = new System.Drawing.Point(404, 98);
            this.txtInputPercent.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtInputPercent.MaximumSize = new System.Drawing.Size(159, 200);
            this.txtInputPercent.MinimumSize = new System.Drawing.Size(159, 20);
            this.txtInputPercent.Name = "txtInputPercent";
            this.txtInputPercent.Size = new System.Drawing.Size(159, 22);
            this.txtInputPercent.TabIndex = 7;
            this.txtInputPercent.TextChanged += new System.EventHandler(this.txtInputPercent_TextChanged);
            this.txtInputPercent.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtInputPercent_KeyDown);
            this.txtInputPercent.Leave += new System.EventHandler(this.txtInputPercent_Leave);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(316, 135);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(74, 17);
            this.label8.TabIndex = 194;
            this.label8.Text = "Referance";
            // 
            // txtAdjReferance
            // 
            this.txtAdjReferance.Location = new System.Drawing.Point(404, 130);
            this.txtAdjReferance.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtAdjReferance.MaximumSize = new System.Drawing.Size(159, 200);
            this.txtAdjReferance.MinimumSize = new System.Drawing.Size(159, 20);
            this.txtAdjReferance.Name = "txtAdjReferance";
            this.txtAdjReferance.Size = new System.Drawing.Size(159, 22);
            this.txtAdjReferance.TabIndex = 9;
            this.txtAdjReferance.TextChanged += new System.EventHandler(this.txtAdjReferance_TextChanged);
            this.txtAdjReferance.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtAdjReferance_KeyDown);
            this.txtAdjReferance.Leave += new System.EventHandler(this.txtAdjReferance_Leave);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(31, 9);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(41, 17);
            this.label9.TabIndex = 196;
            this.label9.Text = "Code";
            // 
            // txtAdjHistoryNo
            // 
            this.txtAdjHistoryNo.Location = new System.Drawing.Point(137, 4);
            this.txtAdjHistoryNo.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtAdjHistoryNo.MaximumSize = new System.Drawing.Size(159, 200);
            this.txtAdjHistoryNo.MinimumSize = new System.Drawing.Size(159, 20);
            this.txtAdjHistoryNo.Name = "txtAdjHistoryNo";
            this.txtAdjHistoryNo.ReadOnly = true;
            this.txtAdjHistoryNo.Size = new System.Drawing.Size(159, 22);
            this.txtAdjHistoryNo.TabIndex = 0;
            this.txtAdjHistoryNo.TabStop = false;
            // 
            // bgwPost
            // 
            this.bgwPost.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwPost_DoWork);
            this.bgwPost.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwPost_RunWorkerCompleted);
            // 
            // btnAddNew
            // 
            this.btnAddNew.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnAddNew.Image = global::VATClient.Properties.Resources.Add_New;
            this.btnAddNew.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnAddNew.Location = new System.Drawing.Point(367, 2);
            this.btnAddNew.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnAddNew.Name = "btnAddNew";
            this.btnAddNew.Size = new System.Drawing.Size(40, 25);
            this.btnAddNew.TabIndex = 2;
            this.btnAddNew.TabStop = false;
            this.btnAddNew.UseVisualStyleBackColor = false;
            this.btnAddNew.Click += new System.EventHandler(this.btnAddNew_Click);
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSearch.Image = global::VATClient.Properties.Resources.search;
            this.btnSearch.Location = new System.Drawing.Point(317, 2);
            this.btnSearch.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(40, 25);
            this.btnSearch.TabIndex = 1;
            this.btnSearch.TabStop = false;
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnVendorGroup_Click);
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::VATClient.Properties.Resources.Green_Footer;
            this.panel1.Controls.Add(this.btnVAT18);
            this.panel1.Controls.Add(this.btnPost);
            this.panel1.Controls.Add(this.btnRefresh);
            this.panel1.Controls.Add(this.btnUpdate);
            this.panel1.Controls.Add(this.btnPrint);
            this.panel1.Controls.Add(this.btnPrintGrid);
            this.panel1.Controls.Add(this.btnPrintList);
            this.panel1.Controls.Add(this.btnClose);
            this.panel1.Controls.Add(this.btnAdd);
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Location = new System.Drawing.Point(0, 260);
            this.panel1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(608, 49);
            this.panel1.TabIndex = 12;
            // 
            // btnVAT18
            // 
            this.btnVAT18.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnVAT18.Image = global::VATClient.Properties.Resources.Print;
            this.btnVAT18.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnVAT18.Location = new System.Drawing.Point(263, 9);
            this.btnVAT18.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnVAT18.Name = "btnVAT18";
            this.btnVAT18.Size = new System.Drawing.Size(72, 34);
            this.btnVAT18.TabIndex = 16;
            this.btnVAT18.Text = "(18)";
            this.btnVAT18.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnVAT18.UseVisualStyleBackColor = false;
            this.btnVAT18.Visible = false;
            this.btnVAT18.Click += new System.EventHandler(this.btnVAT18_Click);
            // 
            // btnPost
            // 
            this.btnPost.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnPost.Image = global::VATClient.Properties.Resources.Post;
            this.btnPost.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPost.Location = new System.Drawing.Point(176, 9);
            this.btnPost.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnPost.Name = "btnPost";
            this.btnPost.Size = new System.Drawing.Size(87, 34);
            this.btnPost.TabIndex = 15;
            this.btnPost.Text = "Post";
            this.btnPost.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnPost.UseVisualStyleBackColor = false;
            this.btnPost.Click += new System.EventHandler(this.btnPost_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnRefresh.Image = ((System.Drawing.Image)(resources.GetObject("btnRefresh.Image")));
            this.btnRefresh.Location = new System.Drawing.Point(339, 9);
            this.btnRefresh.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(47, 34);
            this.btnRefresh.TabIndex = 17;
            this.btnRefresh.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnRefresh.UseVisualStyleBackColor = false;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnUpdate
            // 
            this.btnUpdate.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnUpdate.Image = global::VATClient.Properties.Resources.Update;
            this.btnUpdate.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnUpdate.Location = new System.Drawing.Point(91, 9);
            this.btnUpdate.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(87, 34);
            this.btnUpdate.TabIndex = 14;
            this.btnUpdate.Text = "&Update";
            this.btnUpdate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnUpdate.UseVisualStyleBackColor = false;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // btnPrint
            // 
            this.btnPrint.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnPrint.Image = global::VATClient.Properties.Resources.Print;
            this.btnPrint.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPrint.Location = new System.Drawing.Point(417, 9);
            this.btnPrint.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(77, 34);
            this.btnPrint.TabIndex = 18;
            this.btnPrint.Text = "&Print";
            this.btnPrint.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnPrint.UseVisualStyleBackColor = false;
            this.btnPrint.Visible = false;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // btnPrintGrid
            // 
            this.btnPrintGrid.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnPrintGrid.Image = global::VATClient.Properties.Resources.Print;
            this.btnPrintGrid.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPrintGrid.Location = new System.Drawing.Point(628, 68);
            this.btnPrintGrid.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnPrintGrid.Name = "btnPrintGrid";
            this.btnPrintGrid.Size = new System.Drawing.Size(100, 34);
            this.btnPrintGrid.TabIndex = 24;
            this.btnPrintGrid.Text = "&Grid";
            this.btnPrintGrid.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnPrintGrid.UseVisualStyleBackColor = false;
            // 
            // btnPrintList
            // 
            this.btnPrintList.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnPrintList.Image = global::VATClient.Properties.Resources.Print;
            this.btnPrintList.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPrintList.Location = new System.Drawing.Point(504, 65);
            this.btnPrintList.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnPrintList.Name = "btnPrintList";
            this.btnPrintList.Size = new System.Drawing.Size(100, 34);
            this.btnPrintList.TabIndex = 4;
            this.btnPrintList.Text = "&Print";
            this.btnPrintList.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnPrintList.UseVisualStyleBackColor = false;
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Image = global::VATClient.Properties.Resources.Back;
            this.btnClose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnClose.Location = new System.Drawing.Point(503, 9);
            this.btnClose.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(100, 34);
            this.btnClose.TabIndex = 19;
            this.btnClose.Text = "&Close";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnAdd.Image = global::VATClient.Properties.Resources.Save;
            this.btnAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAdd.Location = new System.Drawing.Point(4, 9);
            this.btnAdd.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(87, 34);
            this.btnAdd.TabIndex = 13;
            this.btnAdd.Text = "&Add";
            this.btnAdd.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnAdd.UseVisualStyleBackColor = false;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnCancel.Image = global::VATClient.Properties.Resources.Referesh;
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.Location = new System.Drawing.Point(380, 66);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 34);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "&Refresh";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancel.UseVisualStyleBackColor = false;
            // 
            // txtAdjHead
            // 
            this.txtAdjHead.Location = new System.Drawing.Point(137, 33);
            this.txtAdjHead.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtAdjHead.MinimumSize = new System.Drawing.Size(159, 20);
            this.txtAdjHead.Name = "txtAdjHead";
            this.txtAdjHead.ReadOnly = true;
            this.txtAdjHead.Size = new System.Drawing.Size(425, 22);
            this.txtAdjHead.TabIndex = 197;
            this.txtAdjHead.TabStop = false;
            this.txtAdjHead.TextChanged += new System.EventHandler(this.txtAdjHead_TextChanged);
            this.txtAdjHead.DoubleClick += new System.EventHandler(this.txtAdjHead_DoubleClick);
            this.txtAdjHead.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtAdjHead_KeyDown);
            // 
            // txtAdjId
            // 
            this.txtAdjId.Location = new System.Drawing.Point(484, 239);
            this.txtAdjId.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtAdjId.MaximumSize = new System.Drawing.Size(159, 200);
            this.txtAdjId.MinimumSize = new System.Drawing.Size(159, 20);
            this.txtAdjId.Name = "txtAdjId";
            this.txtAdjId.Size = new System.Drawing.Size(159, 22);
            this.txtAdjId.TabIndex = 198;
            this.txtAdjId.Visible = false;
            // 
            // label52
            // 
            this.label52.Font = new System.Drawing.Font("Tahoma", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label52.ForeColor = System.Drawing.Color.Red;
            this.label52.Location = new System.Drawing.Point(121, 38);
            this.label52.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label52.MaximumSize = new System.Drawing.Size(13, 12);
            this.label52.Name = "label52";
            this.label52.Size = new System.Drawing.Size(12, 12);
            this.label52.TabIndex = 280;
            this.label52.Text = "*";
            this.label52.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label10
            // 
            this.label10.Font = new System.Drawing.Font("Tahoma", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.ForeColor = System.Drawing.Color.Red;
            this.label10.Location = new System.Drawing.Point(121, 73);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.MaximumSize = new System.Drawing.Size(13, 12);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(12, 12);
            this.label10.TabIndex = 281;
            this.label10.Text = "*";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label11
            // 
            this.label11.Font = new System.Drawing.Font("Tahoma", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.ForeColor = System.Drawing.Color.Red;
            this.label11.Location = new System.Drawing.Point(392, 73);
            this.label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label11.MaximumSize = new System.Drawing.Size(13, 12);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(12, 12);
            this.label11.TabIndex = 282;
            this.label11.Text = "*";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // chkSD
            // 
            this.chkSD.AutoSize = true;
            this.chkSD.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkSD.Location = new System.Drawing.Point(137, 218);
            this.chkSD.Name = "chkSD";
            this.chkSD.Size = new System.Drawing.Size(57, 24);
            this.chkSD.TabIndex = 283;
            this.chkSD.Text = "SD";
            this.chkSD.UseVisualStyleBackColor = true;
            // 
            // FormAdjustment
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(611, 309);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.chkSD);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label52);
            this.Controls.Add(this.txtAdjId);
            this.Controls.Add(this.txtAdjHead);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.txtAdjHistoryNo);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.txtAdjReferance);
            this.Controls.Add(this.btnAddNew);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.txtInputPercent);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtInputAmount);
            this.Controls.Add(this.txtAdjHistoryID);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cmbAdjType);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtAdjDescription);
            this.Controls.Add(this.dtpAdjDate);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtAdjAmount);
            this.Controls.Add(this.panel1);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.Name = "FormAdjustment";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Adjustment";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormAdjustment_FormClosing);
            this.Load += new System.EventHandler(this.FormAdjustment_Load);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.Button btnPrintGrid;
        private System.Windows.Forms.Button btnPrintList;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtAdjAmount;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbAdjType;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtAdjDescription;
        private System.Windows.Forms.DateTimePicker dtpAdjDate;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.ComponentModel.BackgroundWorker backgroundWorkerAdd;
        private System.ComponentModel.BackgroundWorker backgroundWorkerUpdate;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.TextBox txtAdjHistoryID;
        private System.ComponentModel.BackgroundWorker bgwLoad;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtInputAmount;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtInputPercent;
        private System.Windows.Forms.Button btnAddNew;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtAdjReferance;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtAdjHistoryNo;
        private System.ComponentModel.BackgroundWorker bgwPost;
        private System.Windows.Forms.Button btnVAT18;
        public System.Windows.Forms.Button btnUpdate;
        public System.Windows.Forms.Button btnAdd;
        public System.Windows.Forms.Button btnPost;
        private System.Windows.Forms.TextBox txtAdjHead;
        private System.Windows.Forms.TextBox txtAdjId;
        private System.Windows.Forms.Label label52;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.CheckBox chkSD;
    }
}