namespace VATClient.ReportPreview
{
    partial class FormVAT19
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
            this.dtpDate = new System.Windows.Forms.DateTimePicker();
            this.btnPreview = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnLoad = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txt1c = new System.Windows.Forms.TextBox();
            this.txt1b = new System.Windows.Forms.TextBox();
            this.txt1a = new System.Windows.Forms.TextBox();
            this.txt2a = new System.Windows.Forms.TextBox();
            this.txt2b = new System.Windows.Forms.TextBox();
            this.txt2c = new System.Windows.Forms.TextBox();
            this.txt3 = new System.Windows.Forms.TextBox();
            this.txt5 = new System.Windows.Forms.TextBox();
            this.txt6 = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.txt7a = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.txt7b = new System.Windows.Forms.TextBox();
            this.txt8b = new System.Windows.Forms.TextBox();
            this.txt8a = new System.Windows.Forms.TextBox();
            this.txt10 = new System.Windows.Forms.TextBox();
            this.txt9a = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.txt14 = new System.Windows.Forms.TextBox();
            this.txt13 = new System.Windows.Forms.TextBox();
            this.txt12 = new System.Windows.Forms.TextBox();
            this.txt11 = new System.Windows.Forms.TextBox();
            this.txt18 = new System.Windows.Forms.TextBox();
            this.txt17 = new System.Windows.Forms.TextBox();
            this.txt16 = new System.Windows.Forms.TextBox();
            this.txt15 = new System.Windows.Forms.TextBox();
            this.txt19 = new System.Windows.Forms.TextBox();
            this.txt9b = new System.Windows.Forms.TextBox();
            this.label22 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.label27 = new System.Windows.Forms.Label();
            this.label28 = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.label31 = new System.Windows.Forms.Label();
            this.label32 = new System.Windows.Forms.Label();
            this.label33 = new System.Windows.Forms.Label();
            this.label34 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label35 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.label36 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txt4 = new System.Windows.Forms.TextBox();
            this.chkMLock = new System.Windows.Forms.CheckBox();
            this.label37 = new System.Windows.Forms.Label();
            this.label38 = new System.Windows.Forms.Label();
            this.label39 = new System.Windows.Forms.Label();
            this.label40 = new System.Windows.Forms.Label();
            this.label41 = new System.Windows.Forms.Label();
            this.label42 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.label26 = new System.Windows.Forms.Label();
            this.label30 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.chkBreakDown = new System.Windows.Forms.CheckBox();
            this.backgroundWorkerPreview = new System.ComponentModel.BackgroundWorker();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.backgroundWorkerLoad = new System.ComponentModel.BackgroundWorker();
            this.LBDT = new System.Windows.Forms.Label();
            this.chkNewFormat = new System.Windows.Forms.CheckBox();
            this.btn9_1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // dtpDate
            // 
            this.dtpDate.Checked = false;
            this.dtpDate.CustomFormat = "MMMM-yyyy";
            this.dtpDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpDate.Location = new System.Drawing.Point(97, 9);
            this.dtpDate.Name = "dtpDate";
            this.dtpDate.Size = new System.Drawing.Size(146, 20);
            this.dtpDate.TabIndex = 42;
            // 
            // btnPreview
            // 
            this.btnPreview.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnPreview.Image = global::VATClient.Properties.Resources.Print;
            this.btnPreview.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPreview.Location = new System.Drawing.Point(330, 5);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new System.Drawing.Size(75, 28);
            this.btnPreview.TabIndex = 41;
            this.btnPreview.Text = "Print";
            this.btnPreview.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnPreview.UseVisualStyleBackColor = false;
            this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnClose.Image = global::VATClient.Properties.Resources.Back;
            this.btnClose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnClose.Location = new System.Drawing.Point(499, 6);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 27);
            this.btnClose.TabIndex = 8;
            this.btnClose.Text = "&Close";
            this.btnClose.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnLoad
            // 
            this.btnLoad.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnLoad.Image = global::VATClient.Properties.Resources.Load;
            this.btnLoad.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnLoad.Location = new System.Drawing.Point(249, 5);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(75, 28);
            this.btnLoad.TabIndex = 93;
            this.btnLoad.Text = "Load";
            this.btnLoad.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnLoad.UseVisualStyleBackColor = false;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("SutonnyOMJ", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(24, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 18);
            this.label1.TabIndex = 92;
            this.label1.Text = "মাস- বৎসর";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label2.Font = new System.Drawing.Font("SutonnyOMJ", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(8, 45);
            this.label2.MaximumSize = new System.Drawing.Size(440, 17);
            this.label2.MinimumSize = new System.Drawing.Size(440, 17);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(440, 17);
            this.label2.TabIndex = 93;
            this.label2.Text = "বিক্রয় সংক্রান্ত তথ্য                                                            " +
    "                          ";
            // 
            // txt1c
            // 
            this.txt1c.BackColor = System.Drawing.Color.White;
            this.txt1c.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt1c.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt1c.Location = new System.Drawing.Point(663, 63);
            this.txt1c.Name = "txt1c";
            this.txt1c.Size = new System.Drawing.Size(108, 20);
            this.txt1c.TabIndex = 97;
            this.txt1c.Text = "0.00";
            this.txt1c.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txt1c.TextChanged += new System.EventHandler(this.txt1c_TextChanged);
            this.txt1c.Leave += new System.EventHandler(this.txt1c_Leave);
            // 
            // txt1b
            // 
            this.txt1b.BackColor = System.Drawing.Color.White;
            this.txt1b.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt1b.Location = new System.Drawing.Point(554, 63);
            this.txt1b.Name = "txt1b";
            this.txt1b.Size = new System.Drawing.Size(108, 20);
            this.txt1b.TabIndex = 98;
            this.txt1b.Text = "0.00";
            this.txt1b.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txt1b.TextChanged += new System.EventHandler(this.txt1b_TextChanged);
            this.txt1b.Leave += new System.EventHandler(this.txt1b_Leave);
            // 
            // txt1a
            // 
            this.txt1a.BackColor = System.Drawing.Color.White;
            this.txt1a.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt1a.Location = new System.Drawing.Point(445, 63);
            this.txt1a.Name = "txt1a";
            this.txt1a.ReadOnly = true;
            this.txt1a.Size = new System.Drawing.Size(108, 20);
            this.txt1a.TabIndex = 99;
            this.txt1a.Text = "0.00";
            this.txt1a.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txt2a
            // 
            this.txt2a.BackColor = System.Drawing.Color.White;
            this.txt2a.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt2a.Location = new System.Drawing.Point(445, 82);
            this.txt2a.Name = "txt2a";
            this.txt2a.ReadOnly = true;
            this.txt2a.Size = new System.Drawing.Size(108, 20);
            this.txt2a.TabIndex = 102;
            this.txt2a.Text = "0.00";
            this.txt2a.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txt2b
            // 
            this.txt2b.BackColor = System.Drawing.Color.White;
            this.txt2b.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt2b.Location = new System.Drawing.Point(554, 82);
            this.txt2b.Name = "txt2b";
            this.txt2b.ReadOnly = true;
            this.txt2b.Size = new System.Drawing.Size(108, 20);
            this.txt2b.TabIndex = 101;
            this.txt2b.Text = "0.00";
            this.txt2b.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txt2c
            // 
            this.txt2c.BackColor = System.Drawing.Color.White;
            this.txt2c.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt2c.Location = new System.Drawing.Point(663, 82);
            this.txt2c.Name = "txt2c";
            this.txt2c.ReadOnly = true;
            this.txt2c.Size = new System.Drawing.Size(108, 20);
            this.txt2c.TabIndex = 100;
            this.txt2c.Text = "0.00";
            this.txt2c.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txt3
            // 
            this.txt3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt3.Location = new System.Drawing.Point(445, 101);
            this.txt3.Name = "txt3";
            this.txt3.Size = new System.Drawing.Size(108, 20);
            this.txt3.TabIndex = 105;
            this.txt3.Text = "0.00";
            this.txt3.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txt5
            // 
            this.txt5.BackColor = System.Drawing.Color.White;
            this.txt5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt5.Location = new System.Drawing.Point(445, 162);
            this.txt5.Name = "txt5";
            this.txt5.ReadOnly = true;
            this.txt5.Size = new System.Drawing.Size(326, 20);
            this.txt5.TabIndex = 117;
            this.txt5.Text = "0.00";
            this.txt5.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txt5.TextChanged += new System.EventHandler(this.txt5_TextChanged);
            // 
            // txt6
            // 
            this.txt6.BackColor = System.Drawing.Color.White;
            this.txt6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt6.Location = new System.Drawing.Point(445, 189);
            this.txt6.Name = "txt6";
            this.txt6.ReadOnly = true;
            this.txt6.Size = new System.Drawing.Size(326, 20);
            this.txt6.TabIndex = 120;
            this.txt6.Text = "0.00";
            this.txt6.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txt6.TextChanged += new System.EventHandler(this.txt6_TextChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("SutonnyOMJ", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(5, 82);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(187, 15);
            this.label7.TabIndex = 122;
            this.label7.Text = "২। শূন্য হারের পণ্য বা সেবার বিক্রয় (রপ্তানী)";
            this.label7.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("SutonnyOMJ", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(5, 101);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(232, 15);
            this.label8.TabIndex = 123;
            this.label8.Text = "৩। অব্যহতি প্রাপ্ত পণ্য, সেবা বা পণ্য ও সেবার নীট বিক্রয়";
            this.label8.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("SutonnyOMJ", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(5, 162);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(247, 15);
            this.label10.TabIndex = 125;
            this.label10.Text = "৫। অন্যান্য সমন্নয়করন(প্রদেয়/উৎসে/করতন/বয়েয়া/অর্থদন্ড/";
            this.label10.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.label12.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label12.Font = new System.Drawing.Font("SutonnyOMJ", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(445, 45);
            this.label12.MaximumSize = new System.Drawing.Size(108, 17);
            this.label12.MinimumSize = new System.Drawing.Size(108, 17);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(108, 17);
            this.label12.TabIndex = 127;
            this.label12.Text = "বিক্রয় মূল্য                                ";
            // 
            // txt7a
            // 
            this.txt7a.BackColor = System.Drawing.Color.White;
            this.txt7a.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt7a.Location = new System.Drawing.Point(445, 231);
            this.txt7a.Name = "txt7a";
            this.txt7a.ReadOnly = true;
            this.txt7a.Size = new System.Drawing.Size(162, 20);
            this.txt7a.TabIndex = 128;
            this.txt7a.Text = "0.00";
            this.txt7a.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.label14.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label14.Font = new System.Drawing.Font("SutonnyOMJ", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(8, 213);
            this.label14.MaximumSize = new System.Drawing.Size(440, 17);
            this.label14.MinimumSize = new System.Drawing.Size(440, 17);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(440, 17);
            this.label14.TabIndex = 130;
            this.label14.Text = "ক্রয় সংক্রান্ত তথ্য                                                              " +
    "                         ";
            // 
            // txt7b
            // 
            this.txt7b.BackColor = System.Drawing.Color.White;
            this.txt7b.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt7b.Location = new System.Drawing.Point(609, 231);
            this.txt7b.Name = "txt7b";
            this.txt7b.ReadOnly = true;
            this.txt7b.Size = new System.Drawing.Size(162, 20);
            this.txt7b.TabIndex = 131;
            this.txt7b.Text = "0.00";
            this.txt7b.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txt7b.TextChanged += new System.EventHandler(this.txt7b_TextChanged);
            // 
            // txt8b
            // 
            this.txt8b.BackColor = System.Drawing.Color.White;
            this.txt8b.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt8b.Location = new System.Drawing.Point(609, 250);
            this.txt8b.Name = "txt8b";
            this.txt8b.ReadOnly = true;
            this.txt8b.Size = new System.Drawing.Size(162, 20);
            this.txt8b.TabIndex = 133;
            this.txt8b.Text = "0.00";
            this.txt8b.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txt8b.TextChanged += new System.EventHandler(this.txt8b_TextChanged);
            // 
            // txt8a
            // 
            this.txt8a.BackColor = System.Drawing.Color.White;
            this.txt8a.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt8a.Location = new System.Drawing.Point(445, 250);
            this.txt8a.Name = "txt8a";
            this.txt8a.ReadOnly = true;
            this.txt8a.Size = new System.Drawing.Size(162, 20);
            this.txt8a.TabIndex = 132;
            this.txt8a.Text = "0.00";
            this.txt8a.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txt10
            // 
            this.txt10.BackColor = System.Drawing.Color.White;
            this.txt10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt10.Location = new System.Drawing.Point(445, 288);
            this.txt10.Name = "txt10";
            this.txt10.ReadOnly = true;
            this.txt10.Size = new System.Drawing.Size(162, 20);
            this.txt10.TabIndex = 135;
            this.txt10.Text = "0.00";
            this.txt10.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txt9a
            // 
            this.txt9a.BackColor = System.Drawing.Color.White;
            this.txt9a.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt9a.Location = new System.Drawing.Point(445, 269);
            this.txt9a.Name = "txt9a";
            this.txt9a.ReadOnly = true;
            this.txt9a.Size = new System.Drawing.Size(162, 20);
            this.txt9a.TabIndex = 134;
            this.txt9a.Text = "0.00";
            this.txt9a.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("SutonnyOMJ", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.Location = new System.Drawing.Point(5, 250);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(212, 15);
            this.label16.TabIndex = 137;
            this.label16.Text = "৮। করযোগ্য পণ্য,  সেবা বা পণ্য ও সেবার আমদানি";
            this.label16.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("SutonnyOMJ", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label17.Location = new System.Drawing.Point(5, 269);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(154, 15);
            this.label17.TabIndex = 138;
            this.label17.Text = "৯। রপ্তানির ক্ষেত্রে অন্যান্য কর রেয়াত";
            this.label17.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.label13.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label13.Font = new System.Drawing.Font("SutonnyOMJ", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(445, 213);
            this.label13.MaximumSize = new System.Drawing.Size(162, 17);
            this.label13.MinimumSize = new System.Drawing.Size(162, 17);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(162, 17);
            this.label13.TabIndex = 139;
            this.label13.Text = "ক্রয় মূল্য                                                         ";
            // 
            // txt14
            // 
            this.txt14.BackColor = System.Drawing.Color.White;
            this.txt14.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt14.Location = new System.Drawing.Point(445, 387);
            this.txt14.Name = "txt14";
            this.txt14.ReadOnly = true;
            this.txt14.Size = new System.Drawing.Size(326, 20);
            this.txt14.TabIndex = 144;
            this.txt14.Text = "0.00";
            this.txt14.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txt14.TextChanged += new System.EventHandler(this.txt14_TextChanged);
            // 
            // txt13
            // 
            this.txt13.BackColor = System.Drawing.Color.White;
            this.txt13.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt13.Location = new System.Drawing.Point(445, 368);
            this.txt13.Name = "txt13";
            this.txt13.ReadOnly = true;
            this.txt13.Size = new System.Drawing.Size(326, 20);
            this.txt13.TabIndex = 143;
            this.txt13.Text = "0.00";
            this.txt13.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txt13.TextChanged += new System.EventHandler(this.txt13_TextChanged);
            // 
            // txt12
            // 
            this.txt12.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt12.Location = new System.Drawing.Point(445, 349);
            this.txt12.Name = "txt12";
            this.txt12.Size = new System.Drawing.Size(326, 20);
            this.txt12.TabIndex = 142;
            this.txt12.Text = "0.00";
            this.txt12.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txt12.TextChanged += new System.EventHandler(this.txt12_TextChanged);
            // 
            // txt11
            // 
            this.txt11.BackColor = System.Drawing.Color.White;
            this.txt11.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt11.Location = new System.Drawing.Point(445, 330);
            this.txt11.Name = "txt11";
            this.txt11.ReadOnly = true;
            this.txt11.Size = new System.Drawing.Size(326, 20);
            this.txt11.TabIndex = 141;
            this.txt11.Text = "0.00";
            this.txt11.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txt11.TextChanged += new System.EventHandler(this.txt11_TextChanged);
            // 
            // txt18
            // 
            this.txt18.BackColor = System.Drawing.Color.White;
            this.txt18.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt18.Location = new System.Drawing.Point(445, 486);
            this.txt18.Name = "txt18";
            this.txt18.ReadOnly = true;
            this.txt18.Size = new System.Drawing.Size(326, 20);
            this.txt18.TabIndex = 148;
            this.txt18.Text = "0.00";
            this.txt18.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txt17
            // 
            this.txt17.BackColor = System.Drawing.Color.White;
            this.txt17.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt17.Location = new System.Drawing.Point(445, 467);
            this.txt17.Name = "txt17";
            this.txt17.ReadOnly = true;
            this.txt17.Size = new System.Drawing.Size(326, 20);
            this.txt17.TabIndex = 147;
            this.txt17.Text = "0.00";
            this.txt17.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txt16
            // 
            this.txt16.BackColor = System.Drawing.Color.White;
            this.txt16.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt16.Location = new System.Drawing.Point(445, 448);
            this.txt16.Name = "txt16";
            this.txt16.ReadOnly = true;
            this.txt16.Size = new System.Drawing.Size(326, 20);
            this.txt16.TabIndex = 146;
            this.txt16.Text = "0.00";
            this.txt16.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txt15
            // 
            this.txt15.BackColor = System.Drawing.Color.White;
            this.txt15.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt15.Location = new System.Drawing.Point(445, 429);
            this.txt15.Name = "txt15";
            this.txt15.ReadOnly = true;
            this.txt15.Size = new System.Drawing.Size(326, 20);
            this.txt15.TabIndex = 145;
            this.txt15.Text = "0.00";
            this.txt15.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txt19
            // 
            this.txt19.BackColor = System.Drawing.Color.White;
            this.txt19.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt19.Location = new System.Drawing.Point(445, 528);
            this.txt19.Name = "txt19";
            this.txt19.ReadOnly = true;
            this.txt19.Size = new System.Drawing.Size(326, 20);
            this.txt19.TabIndex = 151;
            this.txt19.Text = "0.00";
            this.txt19.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txt9b
            // 
            this.txt9b.BackColor = System.Drawing.Color.White;
            this.txt9b.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt9b.Location = new System.Drawing.Point(609, 269);
            this.txt9b.Name = "txt9b";
            this.txt9b.ReadOnly = true;
            this.txt9b.Size = new System.Drawing.Size(162, 20);
            this.txt9b.TabIndex = 152;
            this.txt9b.Text = "0.00";
            this.txt9b.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txt9b.TextChanged += new System.EventHandler(this.txt9b_TextChanged);
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Font = new System.Drawing.Font("SutonnyOMJ", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label22.Location = new System.Drawing.Point(5, 288);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(233, 15);
            this.label22.TabIndex = 154;
            this.label22.Text = "১০। অব্যাহতি প্রাপ্ত পণ্য, সেবা বা পণ্য অ সেবার নীট ক্রয়";
            this.label22.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Font = new System.Drawing.Font("SutonnyOMJ", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label23.Location = new System.Drawing.Point(5, 349);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(357, 15);
            this.label23.TabIndex = 155;
            this.label23.Text = "১২। অন্যান্য সমন্নয়করন(রেয়াত/পাওনা/আমদানি পর্যায়ে অগ্রিম মূসক/উৎসে প্রদত্ত মূসক)";
            this.label23.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Font = new System.Drawing.Font("SutonnyOMJ", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label24.Location = new System.Drawing.Point(5, 368);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(107, 15);
            this.label24.TabIndex = 156;
            this.label24.Text = "১৩। পূর্ববর্তী মাসের জের";
            this.label24.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Font = new System.Drawing.Font("SutonnyOMJ", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label25.Location = new System.Drawing.Point(5, 387);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(170, 15);
            this.label25.TabIndex = 157;
            this.label25.Text = "১৪। সর্বমোট রেয়াত ( সারি ১১+১২+১৩)";
            this.label25.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Font = new System.Drawing.Font("SutonnyOMJ", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label27.Location = new System.Drawing.Point(5, 448);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(88, 15);
            this.label27.TabIndex = 159;
            this.label27.Text = "১৬। ট্রেজারীতে জমা";
            this.label27.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Font = new System.Drawing.Font("SutonnyOMJ", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label28.Location = new System.Drawing.Point(5, 467);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(140, 15);
            this.label28.TabIndex = 160;
            this.label28.Text = "১৭। পরবর্তী মাসের প্রারম্ভিক জের";
            this.label28.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Font = new System.Drawing.Font("SutonnyOMJ", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label29.Location = new System.Drawing.Point(5, 486);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(120, 15);
            this.label29.TabIndex = 161;
            this.label29.Text = "১৮। পরিদপ্তর হইতে প্রত্যর্পণ";
            this.label29.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.label31.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label31.Font = new System.Drawing.Font("SutonnyOMJ", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label31.Location = new System.Drawing.Point(609, 213);
            this.label31.MaximumSize = new System.Drawing.Size(162, 17);
            this.label31.MinimumSize = new System.Drawing.Size(162, 17);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(162, 17);
            this.label31.TabIndex = 163;
            this.label31.Text = "মূল্য সংযোজন কর                                            ";
            // 
            // label32
            // 
            this.label32.AutoSize = true;
            this.label32.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.label32.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label32.Font = new System.Drawing.Font("SutonnyOMJ", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label32.Location = new System.Drawing.Point(8, 411);
            this.label32.MaximumSize = new System.Drawing.Size(440, 17);
            this.label32.MinimumSize = new System.Drawing.Size(440, 17);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(440, 17);
            this.label32.TabIndex = 164;
            this.label32.Text = "চুরান্ত হিসাব                                                                    " +
    "                                 ";
            // 
            // label33
            // 
            this.label33.AutoSize = true;
            this.label33.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.label33.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label33.Font = new System.Drawing.Font("SutonnyOMJ", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label33.Location = new System.Drawing.Point(8, 510);
            this.label33.MaximumSize = new System.Drawing.Size(440, 17);
            this.label33.MinimumSize = new System.Drawing.Size(440, 17);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(440, 17);
            this.label33.TabIndex = 165;
            this.label33.Text = "সরবরাহকারী কর্তৃক উৎসে মূসক কর্তনের হিসাব                                        " +
    "      ";
            // 
            // label34
            // 
            this.label34.AutoSize = true;
            this.label34.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.label34.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label34.Font = new System.Drawing.Font("SutonnyOMJ", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label34.Location = new System.Drawing.Point(8, 312);
            this.label34.MaximumSize = new System.Drawing.Size(440, 17);
            this.label34.MinimumSize = new System.Drawing.Size(440, 17);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(440, 17);
            this.label34.TabIndex = 166;
            this.label34.Text = "রেয়াত / প্রত্যর্পণ হিসাব                                                         " +
    "                               ";
            this.label34.Click += new System.EventHandler(this.label34_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label3.Font = new System.Drawing.Font("SutonnyOMJ", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(554, 45);
            this.label3.MaximumSize = new System.Drawing.Size(108, 17);
            this.label3.MinimumSize = new System.Drawing.Size(108, 17);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(108, 17);
            this.label3.TabIndex = 167;
            this.label3.Text = "সম্পূরক শুল্ক                               ";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.label5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label5.Font = new System.Drawing.Font("SutonnyOMJ", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(663, 45);
            this.label5.MaximumSize = new System.Drawing.Size(108, 17);
            this.label5.MinimumSize = new System.Drawing.Size(108, 17);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(108, 17);
            this.label5.TabIndex = 168;
            this.label5.Text = "মূল্য সংযোজন কর                   ";
            // 
            // label35
            // 
            this.label35.AutoSize = true;
            this.label35.Font = new System.Drawing.Font("SutonnyOMJ", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label35.Location = new System.Drawing.Point(24, 175);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(168, 15);
            this.label35.TabIndex = 169;
            this.label35.Text = "জরিমানা/স্থান ও স্থাপনা ভাড়া গ্রহনকারী)";
            this.label35.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.label18.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label18.Font = new System.Drawing.Font("SutonnyOMJ", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label18.Location = new System.Drawing.Point(445, 312);
            this.label18.MaximumSize = new System.Drawing.Size(330, 17);
            this.label18.MinimumSize = new System.Drawing.Size(330, 17);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(330, 17);
            this.label18.TabIndex = 170;
            this.label18.Text = "পরিমান                                                                           " +
    "                                                      ";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.label19.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label19.Font = new System.Drawing.Font("SutonnyOMJ", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label19.Location = new System.Drawing.Point(445, 411);
            this.label19.MaximumSize = new System.Drawing.Size(330, 17);
            this.label19.MinimumSize = new System.Drawing.Size(330, 17);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(330, 17);
            this.label19.TabIndex = 171;
            this.label19.Text = "পরিমান                                                                           " +
    "                                                      ";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.label20.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label20.Font = new System.Drawing.Font("SutonnyOMJ", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label20.Location = new System.Drawing.Point(445, 510);
            this.label20.MaximumSize = new System.Drawing.Size(330, 17);
            this.label20.MinimumSize = new System.Drawing.Size(330, 17);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(330, 17);
            this.label20.TabIndex = 172;
            this.label20.Text = "পরিমান                                                                           " +
    "                                                      ";
            // 
            // label36
            // 
            this.label36.AutoSize = true;
            this.label36.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.label36.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label36.Font = new System.Drawing.Font("SutonnyOMJ", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label36.Location = new System.Drawing.Point(8, 125);
            this.label36.MaximumSize = new System.Drawing.Size(440, 17);
            this.label36.MinimumSize = new System.Drawing.Size(440, 17);
            this.label36.Name = "label36";
            this.label36.Size = new System.Drawing.Size(440, 17);
            this.label36.TabIndex = 173;
            this.label36.Text = "প্রদেয় হিসাব                                                                     " +
    "                       ";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.label4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label4.Font = new System.Drawing.Font("SutonnyOMJ", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(445, 125);
            this.label4.MaximumSize = new System.Drawing.Size(330, 17);
            this.label4.MinimumSize = new System.Drawing.Size(330, 17);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(330, 17);
            this.label4.TabIndex = 174;
            this.label4.Text = "পরিমান                                                                           " +
    "                                                      ";
            // 
            // txt4
            // 
            this.txt4.BackColor = System.Drawing.SystemColors.Window;
            this.txt4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt4.Location = new System.Drawing.Point(445, 143);
            this.txt4.Name = "txt4";
            this.txt4.ReadOnly = true;
            this.txt4.Size = new System.Drawing.Size(326, 20);
            this.txt4.TabIndex = 177;
            this.txt4.Text = "0.00";
            this.txt4.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txt4.TextChanged += new System.EventHandler(this.txt4_TextChanged);
            // 
            // chkMLock
            // 
            this.chkMLock.AutoSize = true;
            this.chkMLock.Location = new System.Drawing.Point(705, 25);
            this.chkMLock.Name = "chkMLock";
            this.chkMLock.Size = new System.Drawing.Size(83, 17);
            this.chkMLock.TabIndex = 183;
            this.chkMLock.Text = "Month Lock";
            this.chkMLock.UseVisualStyleBackColor = true;
            this.chkMLock.Visible = false;
            // 
            // label37
            // 
            this.label37.AutoSize = true;
            this.label37.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.label37.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label37.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label37.Location = new System.Drawing.Point(769, 45);
            this.label37.MaximumSize = new System.Drawing.Size(108, 17);
            this.label37.MinimumSize = new System.Drawing.Size(108, 17);
            this.label37.Name = "label37";
            this.label37.Size = new System.Drawing.Size(108, 17);
            this.label37.TabIndex = 184;
            this.label37.Text = "         ";
            // 
            // label38
            // 
            this.label38.AutoSize = true;
            this.label38.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.label38.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label38.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label38.Location = new System.Drawing.Point(769, 125);
            this.label38.MaximumSize = new System.Drawing.Size(330, 17);
            this.label38.MinimumSize = new System.Drawing.Size(330, 17);
            this.label38.Name = "label38";
            this.label38.Size = new System.Drawing.Size(330, 17);
            this.label38.TabIndex = 185;
            this.label38.Text = "         ";
            // 
            // label39
            // 
            this.label39.AutoSize = true;
            this.label39.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.label39.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label39.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label39.Location = new System.Drawing.Point(769, 213);
            this.label39.MaximumSize = new System.Drawing.Size(162, 17);
            this.label39.MinimumSize = new System.Drawing.Size(162, 17);
            this.label39.Name = "label39";
            this.label39.Size = new System.Drawing.Size(162, 17);
            this.label39.TabIndex = 186;
            this.label39.Text = "         ";
            // 
            // label40
            // 
            this.label40.AutoSize = true;
            this.label40.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.label40.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label40.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label40.Location = new System.Drawing.Point(769, 312);
            this.label40.MaximumSize = new System.Drawing.Size(440, 17);
            this.label40.MinimumSize = new System.Drawing.Size(440, 17);
            this.label40.Name = "label40";
            this.label40.Size = new System.Drawing.Size(440, 17);
            this.label40.TabIndex = 187;
            this.label40.Text = "         ";
            // 
            // label41
            // 
            this.label41.AutoSize = true;
            this.label41.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.label41.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label41.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label41.Location = new System.Drawing.Point(770, 411);
            this.label41.MaximumSize = new System.Drawing.Size(440, 17);
            this.label41.MinimumSize = new System.Drawing.Size(440, 17);
            this.label41.Name = "label41";
            this.label41.Size = new System.Drawing.Size(440, 17);
            this.label41.TabIndex = 188;
            this.label41.Text = "         ";
            // 
            // label42
            // 
            this.label42.AutoSize = true;
            this.label42.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.label42.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label42.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label42.Location = new System.Drawing.Point(769, 510);
            this.label42.MaximumSize = new System.Drawing.Size(330, 17);
            this.label42.MinimumSize = new System.Drawing.Size(330, 17);
            this.label42.Name = "label42";
            this.label42.Size = new System.Drawing.Size(330, 17);
            this.label42.TabIndex = 189;
            this.label42.Text = "         ";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("SutonnyOMJ", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(5, 63);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(186, 15);
            this.label6.TabIndex = 199;
            this.label6.Text = "১। করযোগ্য পণ্য, সেবা বা সেবার নীট বিক্রয়";
            this.label6.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("SutonnyOMJ", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(5, 143);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(195, 15);
            this.label9.TabIndex = 200;
            this.label9.Text = "৪। মোট প্রদেয় কর(সারি ১ হইতে এসডি+ভ্যাট)";
            this.label9.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("SutonnyOMJ", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.Location = new System.Drawing.Point(5, 231);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(248, 15);
            this.label15.TabIndex = 201;
            this.label15.Text = "৭। স্থানীয় পর্যায়ে করযোগ্য পণ্য, সেবা বা পণ্য ও সেবার ক্রয়";
            this.label15.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label21
            // 
            this.label21.AllowDrop = true;
            this.label21.AutoSize = true;
            this.label21.Font = new System.Drawing.Font("SutonnyOMJ", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label21.Location = new System.Drawing.Point(5, 330);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(176, 15);
            this.label21.TabIndex = 202;
            this.label21.Text = "১১। মোট রেয়াতযোগ্য কর (সারি ৭+৮+৯)";
            this.label21.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Font = new System.Drawing.Font("SutonnyOMJ", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label26.Location = new System.Drawing.Point(5, 429);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(118, 15);
            this.label26.TabIndex = 203;
            this.label26.Text = "১৫। নীট প্রদেয়(সারি ৬-১৪)";
            this.label26.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Font = new System.Drawing.Font("SutonnyOMJ", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label30.Location = new System.Drawing.Point(5, 528);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(165, 15);
            this.label30.TabIndex = 204;
            this.label30.Text = "১৯। উৎসে কর্তিত মোট মূসকের পরিমান";
            this.label30.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("SutonnyOMJ", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(5, 189);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(132, 15);
            this.label11.TabIndex = 205;
            this.label11.Text = "৬। সর্বমোট প্রদেয় (সারি ৪+৫)";
            this.label11.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // chkBreakDown
            // 
            this.chkBreakDown.AutoSize = true;
            this.chkBreakDown.Location = new System.Drawing.Point(411, 11);
            this.chkBreakDown.Name = "chkBreakDown";
            this.chkBreakDown.Size = new System.Drawing.Size(82, 17);
            this.chkBreakDown.TabIndex = 206;
            this.chkBreakDown.Text = "BreakDown";
            this.chkBreakDown.UseVisualStyleBackColor = true;
            // 
            // backgroundWorkerPreview
            // 
            this.backgroundWorkerPreview.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerPreview_DoWork);
            this.backgroundWorkerPreview.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerPreview_RunWorkerCompleted);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(149, 479);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(291, 22);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 207;
            this.progressBar1.Visible = false;
            // 
            // backgroundWorkerLoad
            // 
            this.backgroundWorkerLoad.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerLoad_DoWork);
            this.backgroundWorkerLoad.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerLoad_RunWorkerCompleted);
            // 
            // LBDT
            // 
            this.LBDT.AutoSize = true;
            this.LBDT.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.LBDT.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LBDT.Font = new System.Drawing.Font("SutonnyOMJ", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LBDT.Location = new System.Drawing.Point(396, 82);
            this.LBDT.MaximumSize = new System.Drawing.Size(50, 20);
            this.LBDT.MinimumSize = new System.Drawing.Size(50, 20);
            this.LBDT.Name = "LBDT";
            this.LBDT.Size = new System.Drawing.Size(50, 20);
            this.LBDT.TabIndex = 208;
            this.LBDT.Text = "BDT";
            // 
            // chkNewFormat
            // 
            this.chkNewFormat.AutoSize = true;
            this.chkNewFormat.Location = new System.Drawing.Point(609, 9);
            this.chkNewFormat.Name = "chkNewFormat";
            this.chkNewFormat.Size = new System.Drawing.Size(83, 17);
            this.chkNewFormat.TabIndex = 209;
            this.chkNewFormat.Text = "New Format";
            this.chkNewFormat.UseVisualStyleBackColor = true;
            // 
            // btn9_1
            // 
            this.btn9_1.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btn9_1.Image = global::VATClient.Properties.Resources.Print;
            this.btn9_1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btn9_1.Location = new System.Drawing.Point(315, 76);
            this.btn9_1.Name = "btn9_1";
            this.btn9_1.Size = new System.Drawing.Size(75, 28);
            this.btn9_1.TabIndex = 210;
            this.btn9_1.Text = "Print 9.1";
            this.btn9_1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btn9_1.UseVisualStyleBackColor = false;
            this.btn9_1.Click += new System.EventHandler(this.btn9_1_Click);
            // 
            // FormVAT19
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.ClientSize = new System.Drawing.Size(784, 552);
            this.Controls.Add(this.btn9_1);
            this.Controls.Add(this.chkNewFormat);
            this.Controls.Add(this.LBDT);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.chkBreakDown);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label30);
            this.Controls.Add(this.label26);
            this.Controls.Add(this.label21);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label42);
            this.Controls.Add(this.label41);
            this.Controls.Add(this.label40);
            this.Controls.Add(this.label39);
            this.Controls.Add(this.label38);
            this.Controls.Add(this.label37);
            this.Controls.Add(this.chkMLock);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.txt4);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label36);
            this.Controls.Add(this.dtpDate);
            this.Controls.Add(this.label20);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label19);
            this.Controls.Add(this.btnPreview);
            this.Controls.Add(this.label18);
            this.Controls.Add(this.label35);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label34);
            this.Controls.Add(this.label33);
            this.Controls.Add(this.label32);
            this.Controls.Add(this.label31);
            this.Controls.Add(this.label29);
            this.Controls.Add(this.label28);
            this.Controls.Add(this.label27);
            this.Controls.Add(this.label25);
            this.Controls.Add(this.label24);
            this.Controls.Add(this.label23);
            this.Controls.Add(this.label22);
            this.Controls.Add(this.txt9b);
            this.Controls.Add(this.txt19);
            this.Controls.Add(this.txt18);
            this.Controls.Add(this.txt17);
            this.Controls.Add(this.txt16);
            this.Controls.Add(this.txt15);
            this.Controls.Add(this.txt14);
            this.Controls.Add(this.txt13);
            this.Controls.Add(this.txt12);
            this.Controls.Add(this.txt11);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.txt10);
            this.Controls.Add(this.txt9a);
            this.Controls.Add(this.txt8b);
            this.Controls.Add(this.txt8a);
            this.Controls.Add(this.txt7b);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.txt7a);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.txt6);
            this.Controls.Add(this.txt5);
            this.Controls.Add(this.txt3);
            this.Controls.Add(this.txt2a);
            this.Controls.Add(this.txt2b);
            this.Controls.Add(this.txt2c);
            this.Controls.Add(this.txt1a);
            this.Controls.Add(this.txt1b);
            this.Controls.Add(this.txt1c);
            this.Controls.Add(this.label2);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(800, 680);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(800, 590);
            this.Name = "FormVAT19";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "VAT 19";
            this.Load += new System.EventHandler(this.FormVAT19_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DateTimePicker dtpDate;
        private System.Windows.Forms.Button btnPreview;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txt1c;
        private System.Windows.Forms.TextBox txt1b;
        private System.Windows.Forms.TextBox txt1a;
        private System.Windows.Forms.TextBox txt2a;
        private System.Windows.Forms.TextBox txt2b;
        private System.Windows.Forms.TextBox txt2c;
        private System.Windows.Forms.TextBox txt3;
        private System.Windows.Forms.TextBox txt5;
        private System.Windows.Forms.TextBox txt6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox txt7a;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox txt7b;
        private System.Windows.Forms.TextBox txt8b;
        private System.Windows.Forms.TextBox txt8a;
        private System.Windows.Forms.TextBox txt10;
        private System.Windows.Forms.TextBox txt9a;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox txt14;
        private System.Windows.Forms.TextBox txt13;
        private System.Windows.Forms.TextBox txt12;
        private System.Windows.Forms.TextBox txt11;
        private System.Windows.Forms.TextBox txt18;
        private System.Windows.Forms.TextBox txt17;
        private System.Windows.Forms.TextBox txt16;
        private System.Windows.Forms.TextBox txt15;
        private System.Windows.Forms.TextBox txt19;
        private System.Windows.Forms.TextBox txt9b;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.Label label32;
        private System.Windows.Forms.Label label33;
        private System.Windows.Forms.Label label34;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label35;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label36;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.TextBox txt4;
        private System.Windows.Forms.CheckBox chkMLock;
        private System.Windows.Forms.Label label37;
        private System.Windows.Forms.Label label38;
        private System.Windows.Forms.Label label39;
        private System.Windows.Forms.Label label40;
        private System.Windows.Forms.Label label41;
        private System.Windows.Forms.Label label42;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.CheckBox chkBreakDown;
        private System.ComponentModel.BackgroundWorker backgroundWorkerPreview;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.ComponentModel.BackgroundWorker backgroundWorkerLoad;
        private System.Windows.Forms.Label LBDT;
        private System.Windows.Forms.CheckBox chkNewFormat;
        private System.Windows.Forms.Button btn9_1;
    }
}