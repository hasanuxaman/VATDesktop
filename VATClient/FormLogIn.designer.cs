namespace VATClient
{
    partial class FormLogIn
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormLogIn));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.dtpSessionDate = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbCompany = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtUserPassword = new System.Windows.Forms.TextBox();
            this.txtUserName = new System.Windows.Forms.TextBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.txtUserNameDB = new System.Windows.Forms.TextBox();
            this.txtPasswordDB = new System.Windows.Forms.TextBox();
            this.chkActiveStatusDB = new System.Windows.Forms.CheckBox();
            this.bgwUserHas = new System.ComponentModel.BackgroundWorker();
            this.bgwCompanyList = new System.ComponentModel.BackgroundWorker();
            this.bgwLoginInfo = new System.ComponentModel.BackgroundWorker();
            this.button2 = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnLogIn = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.Transparent;
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.dtpSessionDate);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.cmbCompany);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtUserPassword);
            this.groupBox1.Controls.Add(this.txtUserName);
            this.groupBox1.Location = new System.Drawing.Point(-1, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(341, 152);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Login";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Image = global::VATClient.Properties.Resources.Calendar;
            this.label2.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label2.Location = new System.Drawing.Point(9, 119);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(106, 21);
            this.label2.TabIndex = 130;
            this.label2.Text = "      Session";
            // 
            // dtpSessionDate
            // 
            this.dtpSessionDate.CustomFormat = "dd/MMM/yyyy";
            this.dtpSessionDate.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpSessionDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpSessionDate.Location = new System.Drawing.Point(104, 116);
            this.dtpSessionDate.MaximumSize = new System.Drawing.Size(214, 22);
            this.dtpSessionDate.MinimumSize = new System.Drawing.Size(214, 22);
            this.dtpSessionDate.Name = "dtpSessionDate";
            this.dtpSessionDate.Size = new System.Drawing.Size(214, 22);
            this.dtpSessionDate.TabIndex = 129;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Image = global::VATClient.Properties.Resources.Company;
            this.label1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label1.Location = new System.Drawing.Point(9, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(120, 21);
            this.label1.TabIndex = 127;
            this.label1.Tag = "  ";
            this.label1.Text = "      Company";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cmbCompany
            // 
            this.cmbCompany.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCompany.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbCompany.FormattingEnabled = true;
            this.cmbCompany.Location = new System.Drawing.Point(104, 18);
            this.cmbCompany.MaximumSize = new System.Drawing.Size(214, 0);
            this.cmbCompany.MinimumSize = new System.Drawing.Size(214, 0);
            this.cmbCompany.Name = "cmbCompany";
            this.cmbCompany.Size = new System.Drawing.Size(214, 27);
            this.cmbCompany.Sorted = true;
            this.cmbCompany.TabIndex = 0;
            this.cmbCompany.SelectedIndexChanged += new System.EventHandler(this.cmbCompany_SelectedIndexChanged);
            this.cmbCompany.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmbCompany_KeyDown);
            this.cmbCompany.Leave += new System.EventHandler(this.cmbCompany_Leave);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Image = global::VATClient.Properties.Resources.LoginPassword;
            this.label4.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label4.Location = new System.Drawing.Point(9, 87);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(123, 21);
            this.label4.TabIndex = 125;
            this.label4.Text = "      Password";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Image = global::VATClient.Properties.Resources.LoginUser;
            this.label3.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label3.Location = new System.Drawing.Point(9, 55);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(79, 21);
            this.label3.TabIndex = 124;
            this.label3.Text = "      User";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtUserPassword
            // 
            this.txtUserPassword.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtUserPassword.Location = new System.Drawing.Point(104, 84);
            this.txtUserPassword.MaximumSize = new System.Drawing.Size(214, 22);
            this.txtUserPassword.MinimumSize = new System.Drawing.Size(214, 22);
            this.txtUserPassword.Name = "txtUserPassword";
            this.txtUserPassword.PasswordChar = '*';
            this.txtUserPassword.Size = new System.Drawing.Size(214, 27);
            this.txtUserPassword.TabIndex = 2;
            this.txtUserPassword.UseSystemPasswordChar = true;
            this.txtUserPassword.TextChanged += new System.EventHandler(this.txtUserPassword_TextChanged);
            this.txtUserPassword.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtUserPassword_KeyDown);
            // 
            // txtUserName
            // 
            this.txtUserName.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtUserName.Location = new System.Drawing.Point(104, 52);
            this.txtUserName.MaximumSize = new System.Drawing.Size(214, 22);
            this.txtUserName.MinimumSize = new System.Drawing.Size(214, 22);
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.Size = new System.Drawing.Size(214, 27);
            this.txtUserName.TabIndex = 1;
            this.txtUserName.TextChanged += new System.EventHandler(this.txtUserName_TextChanged);
            this.txtUserName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtUserName_KeyDown);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(40, 260);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(260, 11);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 209;
            this.progressBar1.Visible = false;
            // 
            // txtUserNameDB
            // 
            this.txtUserNameDB.Location = new System.Drawing.Point(362, 39);
            this.txtUserNameDB.Name = "txtUserNameDB";
            this.txtUserNameDB.Size = new System.Drawing.Size(127, 24);
            this.txtUserNameDB.TabIndex = 4;
            // 
            // txtPasswordDB
            // 
            this.txtPasswordDB.Location = new System.Drawing.Point(363, 66);
            this.txtPasswordDB.Name = "txtPasswordDB";
            this.txtPasswordDB.Size = new System.Drawing.Size(127, 24);
            this.txtPasswordDB.TabIndex = 5;
            // 
            // chkActiveStatusDB
            // 
            this.chkActiveStatusDB.AutoSize = true;
            this.chkActiveStatusDB.Location = new System.Drawing.Point(353, 101);
            this.chkActiveStatusDB.Name = "chkActiveStatusDB";
            this.chkActiveStatusDB.Size = new System.Drawing.Size(98, 21);
            this.chkActiveStatusDB.TabIndex = 6;
            this.chkActiveStatusDB.Text = "checkBox1";
            this.chkActiveStatusDB.UseVisualStyleBackColor = true;
            // 
            // bgwUserHas
            // 
            this.bgwUserHas.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwUserHas_DoWork);
            this.bgwUserHas.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwUserHas_RunWorkerCompleted);
            // 
            // bgwCompanyList
            // 
            this.bgwCompanyList.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwCompanyList_DoWork);
            this.bgwCompanyList.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwCompanyList_RunWorkerCompleted);
            // 
            // bgwLoginInfo
            // 
            this.bgwLoginInfo.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwLoginInfo_DoWork);
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.button2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button2.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button2.Location = new System.Drawing.Point(360, 263);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(97, 36);
            this.button2.TabIndex = 7;
            this.button2.Text = "Other Check";
            this.button2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Visible = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Cursor = System.Windows.Forms.Cursors.Hand;
            this.label5.Location = new System.Drawing.Point(105, 273);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(167, 17);
            this.label5.TabIndex = 211;
            this.label5.Text = "www.symphonysoftt.com";
            this.label5.Click += new System.EventHandler(this.label5_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.Color.Transparent;
            this.groupBox2.Controls.Add(this.groupBox1);
            this.groupBox2.Controls.Add(this.button1);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.btnExit);
            this.groupBox2.Controls.Add(this.btnLogIn);
            this.groupBox2.Controls.Add(this.progressBar1);
            this.groupBox2.Location = new System.Drawing.Point(5, 92);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(340, 302);
            this.groupBox2.TabIndex = 212;
            this.groupBox2.TabStop = false;
            this.groupBox2.Enter += new System.EventHandler(this.groupBox2_Enter);
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.button1.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.button1.Image = global::VATClient.Properties.Resources.administrator;
            this.button1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button1.Location = new System.Drawing.Point(85, 221);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(170, 30);
            this.button1.TabIndex = 4;
            this.button1.Text = "&Super Administrator";
            this.button1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnExit
            // 
            this.btnExit.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnExit.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnExit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnExit.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.btnExit.Image = global::VATClient.Properties.Resources.logout;
            this.btnExit.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnExit.Location = new System.Drawing.Point(171, 182);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(166, 30);
            this.btnExit.TabIndex = 5;
            this.btnExit.Text = "&Exit";
            this.btnExit.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnExit.UseVisualStyleBackColor = false;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnLogIn
            // 
            this.btnLogIn.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnLogIn.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.btnLogIn.Image = global::VATClient.Properties.Resources.Login;
            this.btnLogIn.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnLogIn.Location = new System.Drawing.Point(3, 182);
            this.btnLogIn.Name = "btnLogIn";
            this.btnLogIn.Size = new System.Drawing.Size(166, 30);
            this.btnLogIn.TabIndex = 3;
            this.btnLogIn.Text = "&Login";
            this.btnLogIn.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnLogIn.UseVisualStyleBackColor = false;
            this.btnLogIn.Click += new System.EventHandler(this.btnLogIn_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::VATClient.Properties.Resources.ShampanLogo;
            this.pictureBox1.Location = new System.Drawing.Point(60, 8);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(245, 50);
            this.pictureBox1.TabIndex = 213;
            this.pictureBox1.TabStop = false;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Tahoma", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(128, 58);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(120, 48);
            this.label6.TabIndex = 214;
            this.label6.Text = "2012";
            // 
            // FormLogIn
            // 
            this.AcceptButton = this.btnLogIn;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.CancelButton = this.btnExit;
            this.ClientSize = new System.Drawing.Size(350, 400);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.chkActiveStatusDB);
            this.Controls.Add(this.txtPasswordDB);
            this.Controls.Add(this.txtUserNameDB);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(350, 400);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(350, 400);
            this.Name = "FormLogIn";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Log In";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormLogIn_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormLogIn_FormClosed);
            this.Load += new System.EventHandler(this.FormLogIn_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnLogIn;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtUserPassword;
        private System.Windows.Forms.TextBox txtUserName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox txtUserNameDB;
        private System.Windows.Forms.TextBox txtPasswordDB;
        private System.Windows.Forms.CheckBox chkActiveStatusDB;
        public System.Windows.Forms.ComboBox cmbCompany;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.ComponentModel.BackgroundWorker bgwUserHas;
        private System.ComponentModel.BackgroundWorker bgwCompanyList;
        private System.ComponentModel.BackgroundWorker bgwLoginInfo;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.DateTimePicker dtpSessionDate;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label6;
    }
}