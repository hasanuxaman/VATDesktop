namespace VATClient
{
    partial class FormHsCode
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
            this.btnAddNew = new System.Windows.Forms.Button();
            this.btnSearch = new System.Windows.Forms.Button();
            this.txtVAT = new System.Windows.Forms.TextBox();
            this.txtAIT = new System.Windows.Forms.TextBox();
            this.txtRD = new System.Windows.Forms.TextBox();
            this.txtAT = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.txtCode = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.txtHSCode = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.txtCD = new System.Windows.Forms.TextBox();
            this.txtSD = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cmbFiscalyear = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label35 = new System.Windows.Forms.Label();
            this.txtId = new System.Windows.Forms.TextBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.txtComments = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.chkAT = new System.Windows.Forms.CheckBox();
            this.chkRD = new System.Windows.Forms.CheckBox();
            this.chkAIT = new System.Windows.Forms.CheckBox();
            this.chkVAT = new System.Windows.Forms.CheckBox();
            this.chkSD = new System.Windows.Forms.CheckBox();
            this.chkCD = new System.Windows.Forms.CheckBox();
            this.bgwSave = new System.ComponentModel.BackgroundWorker();
            this.bgwUpdate = new System.ComponentModel.BackgroundWorker();
            this.bgwDelete = new System.ComponentModel.BackgroundWorker();
            this.Other = new System.Windows.Forms.GroupBox();
            this.chkIsVDS = new System.Windows.Forms.CheckBox();
            this.chkOtherCD = new System.Windows.Forms.CheckBox();
            this.chkOtherVAT = new System.Windows.Forms.CheckBox();
            this.txtOtherVAT = new System.Windows.Forms.TextBox();
            this.txtOtherSD = new System.Windows.Forms.TextBox();
            this.btnImport = new System.Windows.Forms.Button();
            this.bgwSelectYear = new System.ComponentModel.BackgroundWorker();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.Other.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnAddNew
            // 
            this.btnAddNew.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnAddNew.Image = global::VATClient.Properties.Resources.Add_New;
            this.btnAddNew.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnAddNew.Location = new System.Drawing.Point(286, 23);
            this.btnAddNew.Name = "btnAddNew";
            this.btnAddNew.Size = new System.Drawing.Size(30, 20);
            this.btnAddNew.TabIndex = 4;
            this.btnAddNew.TabStop = false;
            this.btnAddNew.UseVisualStyleBackColor = false;
            this.btnAddNew.Click += new System.EventHandler(this.btnAddNew_Click);
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSearch.Image = global::VATClient.Properties.Resources.search;
            this.btnSearch.Location = new System.Drawing.Point(250, 23);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(30, 20);
            this.btnSearch.TabIndex = 3;
            this.btnSearch.TabStop = false;
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // txtVAT
            // 
            this.txtVAT.Location = new System.Drawing.Point(81, 89);
            this.txtVAT.Name = "txtVAT";
            this.txtVAT.Size = new System.Drawing.Size(84, 21);
            this.txtVAT.TabIndex = 35;
            this.txtVAT.Text = "0";
            this.txtVAT.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtVAT.TextChanged += new System.EventHandler(this.txtVAT_TextChanged);
            this.txtVAT.Leave += new System.EventHandler(this.txtVAT_Leave);
            // 
            // txtAIT
            // 
            this.txtAIT.Location = new System.Drawing.Point(81, 141);
            this.txtAIT.Name = "txtAIT";
            this.txtAIT.Size = new System.Drawing.Size(84, 21);
            this.txtAIT.TabIndex = 37;
            this.txtAIT.Text = "0";
            this.txtAIT.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtAIT.TextChanged += new System.EventHandler(this.txtAIT_TextChanged);
            this.txtAIT.Leave += new System.EventHandler(this.txtAIT_Leave);
            // 
            // txtRD
            // 
            this.txtRD.Location = new System.Drawing.Point(81, 37);
            this.txtRD.Name = "txtRD";
            this.txtRD.Size = new System.Drawing.Size(84, 21);
            this.txtRD.TabIndex = 39;
            this.txtRD.Text = "0";
            this.txtRD.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtRD.TextChanged += new System.EventHandler(this.txtRD_TextChanged);
            this.txtRD.Leave += new System.EventHandler(this.txtRD_Leave);
            // 
            // txtAT
            // 
            this.txtAT.Location = new System.Drawing.Point(81, 115);
            this.txtAT.Name = "txtAT";
            this.txtAT.Size = new System.Drawing.Size(84, 21);
            this.txtAT.TabIndex = 41;
            this.txtAT.Text = "0";
            this.txtAT.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtAT.TextChanged += new System.EventHandler(this.txtAT_TextChanged);
            this.txtAT.Leave += new System.EventHandler(this.txtAT_Leave);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(8, 141);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(57, 13);
            this.label10.TabIndex = 42;
            this.label10.Text = "Comments";
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::VATClient.Properties.Resources.Green_Footer;
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.btnClose);
            this.panel1.Controls.Add(this.btnUpdate);
            this.panel1.Controls.Add(this.btnAdd);
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Controls.Add(this.btnDelete);
            this.panel1.Location = new System.Drawing.Point(4, 185);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(719, 40);
            this.panel1.TabIndex = 213;
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button1.Image = global::VATClient.Properties.Resources.Back;
            this.button1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button1.Location = new System.Drawing.Point(606, 5);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 28);
            this.button1.TabIndex = 42;
            this.button1.Text = "&Close";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Image = global::VATClient.Properties.Resources.Back;
            this.btnClose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnClose.Location = new System.Drawing.Point(689, 42);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 28);
            this.btnClose.TabIndex = 26;
            this.btnClose.Text = "&Close";
            this.btnClose.UseVisualStyleBackColor = false;
            // 
            // btnUpdate
            // 
            this.btnUpdate.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnUpdate.Image = global::VATClient.Properties.Resources.Update;
            this.btnUpdate.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnUpdate.Location = new System.Drawing.Point(106, 5);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(75, 28);
            this.btnUpdate.TabIndex = 23;
            this.btnUpdate.Text = "&Update";
            this.btnUpdate.UseVisualStyleBackColor = false;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnAdd.Image = global::VATClient.Properties.Resources.Save;
            this.btnAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAdd.Location = new System.Drawing.Point(11, 5);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 28);
            this.btnAdd.TabIndex = 22;
            this.btnAdd.Text = "&Add";
            this.btnAdd.UseVisualStyleBackColor = false;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnCancel.Image = global::VATClient.Properties.Resources.Referesh;
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.Location = new System.Drawing.Point(282, 53);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 28);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "&Refresh";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancel.UseVisualStyleBackColor = false;
            // 
            // btnDelete
            // 
            this.btnDelete.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnDelete.Image = global::VATClient.Properties.Resources.Delete;
            this.btnDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDelete.Location = new System.Drawing.Point(211, 46);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 28);
            this.btnDelete.TabIndex = 24;
            this.btnDelete.Text = "&Delete";
            this.btnDelete.UseVisualStyleBackColor = false;
            this.btnDelete.Visible = false;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // txtDescription
            // 
            this.txtDescription.BackColor = System.Drawing.SystemColors.Window;
            this.txtDescription.Location = new System.Drawing.Point(69, 102);
            this.txtDescription.MaximumSize = new System.Drawing.Size(400, 50);
            this.txtDescription.MinimumSize = new System.Drawing.Size(100, 30);
            this.txtDescription.Multiline = true;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtDescription.Size = new System.Drawing.Size(246, 30);
            this.txtDescription.TabIndex = 214;
            this.txtDescription.TabStop = false;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(8, 25);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(32, 13);
            this.label11.TabIndex = 25;
            this.label11.Text = "Code";
            // 
            // txtCode
            // 
            this.txtCode.Location = new System.Drawing.Point(71, 22);
            this.txtCode.MaximumSize = new System.Drawing.Size(4, 25);
            this.txtCode.MinimumSize = new System.Drawing.Size(172, 20);
            this.txtCode.Name = "txtCode";
            this.txtCode.ReadOnly = true;
            this.txtCode.Size = new System.Drawing.Size(172, 21);
            this.txtCode.TabIndex = 24;
            this.txtCode.TabStop = false;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(6, 53);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(45, 13);
            this.label12.TabIndex = 26;
            this.label12.Text = "HSCode";
            // 
            // txtHSCode
            // 
            this.txtHSCode.Location = new System.Drawing.Point(71, 50);
            this.txtHSCode.Name = "txtHSCode";
            this.txtHSCode.Size = new System.Drawing.Size(172, 21);
            this.txtHSCode.TabIndex = 27;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(6, 107);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(60, 13);
            this.label13.TabIndex = 28;
            this.label13.Text = "Description";
            // 
            // txtCD
            // 
            this.txtCD.Location = new System.Drawing.Point(81, 13);
            this.txtCD.Name = "txtCD";
            this.txtCD.Size = new System.Drawing.Size(84, 21);
            this.txtCD.TabIndex = 31;
            this.txtCD.Text = "0";
            this.txtCD.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtCD.TextChanged += new System.EventHandler(this.textBox4_TextChanged);
            this.txtCD.Leave += new System.EventHandler(this.txtCD_Leave);
            // 
            // txtSD
            // 
            this.txtSD.Location = new System.Drawing.Point(81, 62);
            this.txtSD.Name = "txtSD";
            this.txtSD.Size = new System.Drawing.Size(84, 21);
            this.txtSD.TabIndex = 33;
            this.txtSD.Text = "0";
            this.txtSD.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtSD.TextChanged += new System.EventHandler(this.textBox5_TextChanged);
            this.txtSD.Leave += new System.EventHandler(this.txtSD_Leave);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cmbFiscalyear);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label35);
            this.groupBox1.Controls.Add(this.txtId);
            this.groupBox1.Controls.Add(this.progressBar1);
            this.groupBox1.Controls.Add(this.txtComments);
            this.groupBox1.Controls.Add(this.txtCode);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.txtDescription);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.txtHSCode);
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Controls.Add(this.btnSearch);
            this.groupBox1.Controls.Add(this.btnAddNew);
            this.groupBox1.Location = new System.Drawing.Point(4, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(323, 175);
            this.groupBox1.TabIndex = 215;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Basic";
            // 
            // cmbFiscalyear
            // 
            this.cmbFiscalyear.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFiscalyear.FormattingEnabled = true;
            this.cmbFiscalyear.Location = new System.Drawing.Point(71, 76);
            this.cmbFiscalyear.Name = "cmbFiscalyear";
            this.cmbFiscalyear.Size = new System.Drawing.Size(130, 21);
            this.cmbFiscalyear.TabIndex = 262;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 80);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 13);
            this.label1.TabIndex = 261;
            this.label1.Text = "Fiscal Year";
            // 
            // label35
            // 
            this.label35.AutoSize = true;
            this.label35.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label35.ForeColor = System.Drawing.Color.Red;
            this.label35.Location = new System.Drawing.Point(55, 55);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(14, 14);
            this.label35.TabIndex = 260;
            this.label35.Text = "*";
            // 
            // txtId
            // 
            this.txtId.Location = new System.Drawing.Point(262, 50);
            this.txtId.Name = "txtId";
            this.txtId.Size = new System.Drawing.Size(53, 21);
            this.txtId.TabIndex = 217;
            this.txtId.Visible = false;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(173, 141);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(288, 24);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 216;
            this.progressBar1.Visible = false;
            // 
            // txtComments
            // 
            this.txtComments.BackColor = System.Drawing.SystemColors.Window;
            this.txtComments.Location = new System.Drawing.Point(69, 135);
            this.txtComments.MaximumSize = new System.Drawing.Size(400, 50);
            this.txtComments.MinimumSize = new System.Drawing.Size(100, 30);
            this.txtComments.Multiline = true;
            this.txtComments.Name = "txtComments";
            this.txtComments.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtComments.Size = new System.Drawing.Size(246, 30);
            this.txtComments.TabIndex = 215;
            this.txtComments.TabStop = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.chkAT);
            this.groupBox2.Controls.Add(this.chkRD);
            this.groupBox2.Controls.Add(this.chkAIT);
            this.groupBox2.Controls.Add(this.chkVAT);
            this.groupBox2.Controls.Add(this.chkSD);
            this.groupBox2.Controls.Add(this.chkCD);
            this.groupBox2.Controls.Add(this.txtAIT);
            this.groupBox2.Controls.Add(this.txtRD);
            this.groupBox2.Controls.Add(this.txtVAT);
            this.groupBox2.Controls.Add(this.txtAT);
            this.groupBox2.Controls.Add(this.txtCD);
            this.groupBox2.Controls.Add(this.txtSD);
            this.groupBox2.Location = new System.Drawing.Point(333, 6);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(176, 175);
            this.groupBox2.TabIndex = 216;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Import";
            // 
            // chkAT
            // 
            this.chkAT.AutoSize = true;
            this.chkAT.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkAT.Location = new System.Drawing.Point(12, 118);
            this.chkAT.Name = "chkAT";
            this.chkAT.Size = new System.Drawing.Size(58, 17);
            this.chkAT.TabIndex = 256;
            this.chkAT.TabStop = false;
            this.chkAT.Text = "AT(%)";
            this.chkAT.UseVisualStyleBackColor = true;
            this.chkAT.CheckedChanged += new System.EventHandler(this.chkAT_CheckedChanged);
            // 
            // chkRD
            // 
            this.chkRD.AutoSize = true;
            this.chkRD.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkRD.Location = new System.Drawing.Point(11, 39);
            this.chkRD.Name = "chkRD";
            this.chkRD.Size = new System.Drawing.Size(59, 17);
            this.chkRD.TabIndex = 255;
            this.chkRD.TabStop = false;
            this.chkRD.Text = "RD(%)";
            this.chkRD.UseVisualStyleBackColor = true;
            this.chkRD.CheckedChanged += new System.EventHandler(this.chkRD_CheckedChanged);
            // 
            // chkAIT
            // 
            this.chkAIT.AutoSize = true;
            this.chkAIT.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkAIT.Location = new System.Drawing.Point(8, 145);
            this.chkAIT.Name = "chkAIT";
            this.chkAIT.Size = new System.Drawing.Size(62, 17);
            this.chkAIT.TabIndex = 254;
            this.chkAIT.TabStop = false;
            this.chkAIT.Text = "AIT(%)";
            this.chkAIT.UseVisualStyleBackColor = true;
            this.chkAIT.CheckedChanged += new System.EventHandler(this.chkAIT_CheckedChanged);
            // 
            // chkVAT
            // 
            this.chkVAT.AutoSize = true;
            this.chkVAT.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkVAT.Location = new System.Drawing.Point(5, 91);
            this.chkVAT.Name = "chkVAT";
            this.chkVAT.Size = new System.Drawing.Size(64, 17);
            this.chkVAT.TabIndex = 253;
            this.chkVAT.TabStop = false;
            this.chkVAT.Text = "VAT(%)";
            this.chkVAT.UseVisualStyleBackColor = true;
            this.chkVAT.CheckedChanged += new System.EventHandler(this.chkVAT_CheckedChanged);
            // 
            // chkSD
            // 
            this.chkSD.AutoSize = true;
            this.chkSD.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkSD.Location = new System.Drawing.Point(11, 64);
            this.chkSD.Name = "chkSD";
            this.chkSD.Size = new System.Drawing.Size(58, 17);
            this.chkSD.TabIndex = 252;
            this.chkSD.TabStop = false;
            this.chkSD.Text = "SD(%)";
            this.chkSD.UseVisualStyleBackColor = true;
            this.chkSD.CheckedChanged += new System.EventHandler(this.chkSD_CheckedChanged);
            // 
            // chkCD
            // 
            this.chkCD.AutoSize = true;
            this.chkCD.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkCD.Location = new System.Drawing.Point(10, 17);
            this.chkCD.Name = "chkCD";
            this.chkCD.Size = new System.Drawing.Size(59, 17);
            this.chkCD.TabIndex = 251;
            this.chkCD.TabStop = false;
            this.chkCD.Text = "CD(%)";
            this.chkCD.UseVisualStyleBackColor = true;
            this.chkCD.CheckedChanged += new System.EventHandler(this.chkCD_CheckedChanged);
            // 
            // bgwSave
            // 
            this.bgwSave.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwSave_DoWork);
            this.bgwSave.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwSave_RunWorkerCompleted);
            // 
            // bgwUpdate
            // 
            this.bgwUpdate.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwUpdate_DoWork);
            this.bgwUpdate.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwUpdate_RunWorkerCompleted);
            // 
            // bgwDelete
            // 
            this.bgwDelete.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwDelete_DoWork);
            this.bgwDelete.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwDelete_RunWorkerCompleted);
            // 
            // Other
            // 
            this.Other.Controls.Add(this.chkIsVDS);
            this.Other.Controls.Add(this.chkOtherCD);
            this.Other.Controls.Add(this.chkOtherVAT);
            this.Other.Controls.Add(this.txtOtherVAT);
            this.Other.Controls.Add(this.txtOtherSD);
            this.Other.Location = new System.Drawing.Point(512, 46);
            this.Other.Name = "Other";
            this.Other.Size = new System.Drawing.Size(176, 135);
            this.Other.TabIndex = 217;
            this.Other.TabStop = false;
            this.Other.Text = "Local";
            // 
            // chkIsVDS
            // 
            this.chkIsVDS.AutoSize = true;
            this.chkIsVDS.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkIsVDS.Location = new System.Drawing.Point(31, 63);
            this.chkIsVDS.Name = "chkIsVDS";
            this.chkIsVDS.Size = new System.Drawing.Size(45, 17);
            this.chkIsVDS.TabIndex = 252;
            this.chkIsVDS.TabStop = false;
            this.chkIsVDS.Text = "VDS";
            this.chkIsVDS.UseVisualStyleBackColor = true;
            // 
            // chkOtherCD
            // 
            this.chkOtherCD.AutoSize = true;
            this.chkOtherCD.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkOtherCD.Location = new System.Drawing.Point(15, 17);
            this.chkOtherCD.Name = "chkOtherCD";
            this.chkOtherCD.Size = new System.Drawing.Size(61, 17);
            this.chkOtherCD.TabIndex = 251;
            this.chkOtherCD.TabStop = false;
            this.chkOtherCD.Text = "SD (%)";
            this.chkOtherCD.UseVisualStyleBackColor = true;
            this.chkOtherCD.CheckedChanged += new System.EventHandler(this.chkLocalCD_CheckedChanged);
            // 
            // chkOtherVAT
            // 
            this.chkOtherVAT.AutoSize = true;
            this.chkOtherVAT.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkOtherVAT.Location = new System.Drawing.Point(9, 40);
            this.chkOtherVAT.Name = "chkOtherVAT";
            this.chkOtherVAT.Size = new System.Drawing.Size(67, 17);
            this.chkOtherVAT.TabIndex = 250;
            this.chkOtherVAT.TabStop = false;
            this.chkOtherVAT.Text = "VAT (%)";
            this.chkOtherVAT.UseVisualStyleBackColor = true;
            this.chkOtherVAT.CheckedChanged += new System.EventHandler(this.chkIsFixedVAT_CheckedChanged);
            // 
            // txtOtherVAT
            // 
            this.txtOtherVAT.Location = new System.Drawing.Point(84, 38);
            this.txtOtherVAT.Name = "txtOtherVAT";
            this.txtOtherVAT.Size = new System.Drawing.Size(84, 21);
            this.txtOtherVAT.TabIndex = 3;
            this.txtOtherVAT.Text = "0";
            this.txtOtherVAT.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtOtherVAT.Leave += new System.EventHandler(this.txtOtherVAT_Leave);
            // 
            // txtOtherSD
            // 
            this.txtOtherSD.Location = new System.Drawing.Point(84, 15);
            this.txtOtherSD.Name = "txtOtherSD";
            this.txtOtherSD.Size = new System.Drawing.Size(84, 21);
            this.txtOtherSD.TabIndex = 1;
            this.txtOtherSD.Text = "0";
            this.txtOtherSD.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtOtherSD.Leave += new System.EventHandler(this.txtOtherSD_Leave);
            // 
            // btnImport
            // 
            this.btnImport.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnImport.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnImport.Location = new System.Drawing.Point(580, 12);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(75, 28);
            this.btnImport.TabIndex = 218;
            this.btnImport.Text = "Import";
            this.btnImport.UseVisualStyleBackColor = false;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // bgwSelectYear
            // 
            this.bgwSelectYear.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwSelectYear_DoWork);
            this.bgwSelectYear.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwSelectYear_RunWorkerCompleted);
            // 
            // FormHsCode
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(694, 225);
            this.Controls.Add(this.btnImport);
            this.Controls.Add(this.Other);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(742, 264);
            this.MinimumSize = new System.Drawing.Size(710, 264);
            this.Name = "FormHsCode";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "HSCode";
            this.Load += new System.EventHandler(this.FormHsCode_Load);
            this.panel1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.Other.ResumeLayout(false);
            this.Other.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnAddNew;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.TextBox txtVAT;
        private System.Windows.Forms.TextBox txtAIT;
        private System.Windows.Forms.TextBox txtRD;
        private System.Windows.Forms.TextBox txtAT;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txtCode;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox txtHSCode;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox txtCD;
        private System.Windows.Forms.TextBox txtSD;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox txtComments;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.ComponentModel.BackgroundWorker bgwSave;
        private System.Windows.Forms.TextBox txtId;
        private System.ComponentModel.BackgroundWorker bgwUpdate;
        private System.ComponentModel.BackgroundWorker bgwDelete;
        private System.Windows.Forms.GroupBox Other;
        private System.Windows.Forms.TextBox txtOtherVAT;
        private System.Windows.Forms.TextBox txtOtherSD;
        public System.Windows.Forms.CheckBox chkOtherVAT;
        public System.Windows.Forms.CheckBox chkSD;
        public System.Windows.Forms.CheckBox chkCD;
        public System.Windows.Forms.CheckBox chkVAT;
        public System.Windows.Forms.CheckBox chkAT;
        public System.Windows.Forms.CheckBox chkRD;
        public System.Windows.Forms.CheckBox chkAIT;
        public System.Windows.Forms.CheckBox chkOtherCD;
        private System.Windows.Forms.Button btnImport;
        public System.Windows.Forms.Button btnUpdate;
        public System.Windows.Forms.Button btnAdd;
        public System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Label label35;
        public System.Windows.Forms.CheckBox chkIsVDS;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbFiscalyear;
        private System.ComponentModel.BackgroundWorker bgwSelectYear;
    }
}