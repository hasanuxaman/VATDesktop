namespace VATClient
{
    partial class FormSupperAdministrator
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnSysDBCreate = new System.Windows.Forms.Button();
            this.btnDBBackup = new System.Windows.Forms.Button();
            this.btnSuperInfo = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.button2 = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnNewCompany = new System.Windows.Forms.Button();
            this.txtUserPassword = new System.Windows.Forms.TextBox();
            this.btnLogIn = new System.Windows.Forms.Button();
            this.txtUserName = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.gBox = new System.Windows.Forms.GroupBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.TxtChangePWD1 = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.TxtChangePWD = new System.Windows.Forms.TextBox();
            this.TxtChangeUserName = new System.Windows.Forms.TextBox();
            this.bgwLogin = new System.ComponentModel.BackgroundWorker();
            this.bgwUpdate = new System.ComponentModel.BackgroundWorker();
            this.groupBox1.SuspendLayout();
            this.gBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnSysDBCreate);
            this.groupBox1.Controls.Add(this.btnDBBackup);
            this.groupBox1.Controls.Add(this.btnSuperInfo);
            this.groupBox1.Controls.Add(this.progressBar1);
            this.groupBox1.Controls.Add(this.button2);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.btnExit);
            this.groupBox1.Controls.Add(this.btnNewCompany);
            this.groupBox1.Controls.Add(this.txtUserPassword);
            this.groupBox1.Controls.Add(this.btnLogIn);
            this.groupBox1.Controls.Add(this.txtUserName);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(363, 226);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Super Administrator Login";
            this.groupBox1.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // btnSysDBCreate
            // 
            this.btnSysDBCreate.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSysDBCreate.Font = new System.Drawing.Font("Rod", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.btnSysDBCreate.Image = global::VATClient.Properties.Resources.database_process;
            this.btnSysDBCreate.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSysDBCreate.Location = new System.Drawing.Point(183, 144);
            this.btnSysDBCreate.Name = "btnSysDBCreate";
            this.btnSysDBCreate.Size = new System.Drawing.Size(143, 28);
            this.btnSysDBCreate.TabIndex = 212;
            this.btnSysDBCreate.Text = "Sys  DB Create";
            this.btnSysDBCreate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnSysDBCreate.UseVisualStyleBackColor = false;
            this.btnSysDBCreate.Click += new System.EventHandler(this.btnSysDBCreate_Click);
            // 
            // btnDBBackup
            // 
            this.btnDBBackup.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnDBBackup.Font = new System.Drawing.Font("Rod", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.btnDBBackup.Image = global::VATClient.Properties.Resources.database_process;
            this.btnDBBackup.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDBBackup.Location = new System.Drawing.Point(37, 175);
            this.btnDBBackup.Name = "btnDBBackup";
            this.btnDBBackup.Size = new System.Drawing.Size(143, 28);
            this.btnDBBackup.TabIndex = 211;
            this.btnDBBackup.Text = "Backup/Restore";
            this.btnDBBackup.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnDBBackup.UseVisualStyleBackColor = false;
            this.btnDBBackup.Click += new System.EventHandler(this.btnDBBackup_Click);
            // 
            // btnSuperInfo
            // 
            this.btnSuperInfo.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSuperInfo.Font = new System.Drawing.Font("Rod", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.btnSuperInfo.Image = global::VATClient.Properties.Resources.Update;
            this.btnSuperInfo.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSuperInfo.Location = new System.Drawing.Point(37, 144);
            this.btnSuperInfo.Name = "btnSuperInfo";
            this.btnSuperInfo.Size = new System.Drawing.Size(143, 28);
            this.btnSuperInfo.TabIndex = 3;
            this.btnSuperInfo.Text = "Sys Login Info";
            this.btnSuperInfo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnSuperInfo.UseVisualStyleBackColor = false;
            this.btnSuperInfo.Click += new System.EventHandler(this.btnSuperInfo_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(37, 95);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(288, 13);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 210;
            this.progressBar1.Visible = false;
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.button2.Font = new System.Drawing.Font("Rod", 9.75F);
            this.button2.Image = global::VATClient.Properties.Resources.Update;
            this.button2.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button2.Location = new System.Drawing.Point(166, 112);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(108, 28);
            this.button2.TabIndex = 5;
            this.button2.Text = "&Change PWD";
            this.button2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(34, 70);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(40, 17);
            this.label4.TabIndex = 125;
            this.label4.Text = "PWD";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(34, 40);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 17);
            this.label3.TabIndex = 124;
            this.label3.Text = "User";
            // 
            // btnExit
            // 
            this.btnExit.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnExit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnExit.Font = new System.Drawing.Font("Rod", 9.75F);
            this.btnExit.Image = global::VATClient.Properties.Resources.Back;
            this.btnExit.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnExit.Location = new System.Drawing.Point(276, 112);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(49, 28);
            this.btnExit.TabIndex = 4;
            this.btnExit.Text = "B";
            this.btnExit.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnExit.UseVisualStyleBackColor = false;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnNewCompany
            // 
            this.btnNewCompany.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnNewCompany.Font = new System.Drawing.Font("Rod", 9.75F);
            this.btnNewCompany.Image = global::VATClient.Properties.Resources.Company;
            this.btnNewCompany.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnNewCompany.Location = new System.Drawing.Point(183, 175);
            this.btnNewCompany.Name = "btnNewCompany";
            this.btnNewCompany.Size = new System.Drawing.Size(143, 28);
            this.btnNewCompany.TabIndex = 6;
            this.btnNewCompany.Text = "&New Company";
            this.btnNewCompany.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnNewCompany.UseVisualStyleBackColor = false;
            this.btnNewCompany.Click += new System.EventHandler(this.btnNewCompany_Click);
            // 
            // txtUserPassword
            // 
            this.txtUserPassword.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtUserPassword.Location = new System.Drawing.Point(103, 67);
            this.txtUserPassword.MinimumSize = new System.Drawing.Size(175, 22);
            this.txtUserPassword.Name = "txtUserPassword";
            this.txtUserPassword.PasswordChar = '*';
            this.txtUserPassword.Size = new System.Drawing.Size(222, 22);
            this.txtUserPassword.TabIndex = 1;
            this.txtUserPassword.UseSystemPasswordChar = true;
            this.txtUserPassword.TextChanged += new System.EventHandler(this.txtUserPassword_TextChanged);
            this.txtUserPassword.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtUserPassword_KeyDown);
            // 
            // btnLogIn
            // 
            this.btnLogIn.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnLogIn.Font = new System.Drawing.Font("Rod", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.btnLogIn.Image = global::VATClient.Properties.Resources.Login;
            this.btnLogIn.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnLogIn.Location = new System.Drawing.Point(37, 112);
            this.btnLogIn.Name = "btnLogIn";
            this.btnLogIn.Size = new System.Drawing.Size(124, 28);
            this.btnLogIn.TabIndex = 2;
            this.btnLogIn.Text = "&Super Login ";
            this.btnLogIn.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnLogIn.UseVisualStyleBackColor = false;
            this.btnLogIn.Click += new System.EventHandler(this.btnLogIn_Click);
            // 
            // txtUserName
            // 
            this.txtUserName.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtUserName.Location = new System.Drawing.Point(103, 37);
            this.txtUserName.MinimumSize = new System.Drawing.Size(175, 22);
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.Size = new System.Drawing.Size(222, 22);
            this.txtUserName.TabIndex = 0;
            this.txtUserName.UseSystemPasswordChar = true;
            this.txtUserName.TextChanged += new System.EventHandler(this.txtUserName_TextChanged);
            this.txtUserName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtUserName_KeyDown);
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.button1.Font = new System.Drawing.Font("Rod", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button1.Location = new System.Drawing.Point(125, 377);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(77, 28);
            this.button1.TabIndex = 16;
            this.button1.Text = "&DB Login";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // gBox
            // 
            this.gBox.Controls.Add(this.btnClose);
            this.gBox.Controls.Add(this.btnUpdate);
            this.gBox.Controls.Add(this.label1);
            this.gBox.Controls.Add(this.label2);
            this.gBox.Controls.Add(this.TxtChangePWD1);
            this.gBox.Controls.Add(this.label5);
            this.gBox.Controls.Add(this.TxtChangePWD);
            this.gBox.Controls.Add(this.TxtChangeUserName);
            this.gBox.Location = new System.Drawing.Point(12, 12);
            this.gBox.Name = "gBox";
            this.gBox.Size = new System.Drawing.Size(363, 209);
            this.gBox.TabIndex = 132;
            this.gBox.TabStop = false;
            this.gBox.Text = "Super Administrator Update";
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Font = new System.Drawing.Font("Rod", 9.75F);
            this.btnClose.Image = global::VATClient.Properties.Resources.Back;
            this.btnClose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnClose.Location = new System.Drawing.Point(199, 144);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(117, 28);
            this.btnClose.TabIndex = 3;
            this.btnClose.Text = "Clo&se";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnUpdate
            // 
            this.btnUpdate.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnUpdate.Font = new System.Drawing.Font("Rod", 9.75F);
            this.btnUpdate.Image = global::VATClient.Properties.Resources.Update;
            this.btnUpdate.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnUpdate.Location = new System.Drawing.Point(58, 144);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(135, 28);
            this.btnUpdate.TabIndex = 137;
            this.btnUpdate.Text = "&Change";
            this.btnUpdate.UseVisualStyleBackColor = false;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(52, 113);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 17);
            this.label1.TabIndex = 134;
            this.label1.Text = "Re PWD";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(51, 85);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 17);
            this.label2.TabIndex = 136;
            this.label2.Text = "New PWD";
            // 
            // TxtChangePWD1
            // 
            this.TxtChangePWD1.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TxtChangePWD1.Location = new System.Drawing.Point(139, 110);
            this.TxtChangePWD1.MaximumSize = new System.Drawing.Size(4, 4);
            this.TxtChangePWD1.MinimumSize = new System.Drawing.Size(175, 22);
            this.TxtChangePWD1.Name = "TxtChangePWD1";
            this.TxtChangePWD1.PasswordChar = '*';
            this.TxtChangePWD1.Size = new System.Drawing.Size(175, 22);
            this.TxtChangePWD1.TabIndex = 2;
            this.TxtChangePWD1.UseSystemPasswordChar = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(51, 55);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(74, 17);
            this.label5.TabIndex = 135;
            this.label5.Text = "User Name";
            // 
            // TxtChangePWD
            // 
            this.TxtChangePWD.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TxtChangePWD.Location = new System.Drawing.Point(138, 82);
            this.TxtChangePWD.MaximumSize = new System.Drawing.Size(4, 4);
            this.TxtChangePWD.MinimumSize = new System.Drawing.Size(175, 22);
            this.TxtChangePWD.Name = "TxtChangePWD";
            this.TxtChangePWD.PasswordChar = '*';
            this.TxtChangePWD.Size = new System.Drawing.Size(175, 22);
            this.TxtChangePWD.TabIndex = 1;
            this.TxtChangePWD.UseSystemPasswordChar = true;
            // 
            // TxtChangeUserName
            // 
            this.TxtChangeUserName.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TxtChangeUserName.Location = new System.Drawing.Point(138, 52);
            this.TxtChangeUserName.MaximumSize = new System.Drawing.Size(4, 4);
            this.TxtChangeUserName.MinimumSize = new System.Drawing.Size(175, 22);
            this.TxtChangeUserName.Name = "TxtChangeUserName";
            this.TxtChangeUserName.ReadOnly = true;
            this.TxtChangeUserName.Size = new System.Drawing.Size(175, 22);
            this.TxtChangeUserName.TabIndex = 0;
            // 
            // bgwLogin
            // 
            this.bgwLogin.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwLogin_DoWork);
            this.bgwLogin.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwLogin_RunWorkerCompleted);
            // 
            // bgwUpdate
            // 
            this.bgwUpdate.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwUpdate_DoWork);
            this.bgwUpdate.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwUpdate_RunWorkerCompleted);
            // 
            // FormSupperAdministrator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(388, 254);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.gBox);
            this.Controls.Add(this.button1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(303, 225);
            this.Name = "FormSupperAdministrator";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Super Administrator";
            this.Load += new System.EventHandler(this.FormSupperAdministrator_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.gBox.ResumeLayout(false);
            this.gBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtUserPassword;
        private System.Windows.Forms.TextBox txtUserName;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Button btnNewCompany;
        private System.Windows.Forms.Button btnLogIn;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.GroupBox gBox;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox TxtChangePWD1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox TxtChangePWD;
        private System.Windows.Forms.TextBox TxtChangeUserName;
        private System.Windows.Forms.Button btnClose;
        private System.ComponentModel.BackgroundWorker bgwLogin;
        private System.ComponentModel.BackgroundWorker bgwUpdate;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button btnSuperInfo;
        private System.Windows.Forms.Button btnDBBackup;
        private System.Windows.Forms.Button btnSysDBCreate;
    }
}