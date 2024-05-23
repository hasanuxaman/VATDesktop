namespace VATClient
{
    partial class FormSuperInfo
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
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkIsWindowsAuthentication = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtDataSource = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtUserPassword = new System.Windows.Forms.TextBox();
            this.txtUserName = new System.Windows.Forms.TextBox();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnLogIn = new System.Windows.Forms.Button();
            this.btnTestConn = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(13, 189);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(303, 19);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 68;
            this.progressBar1.Visible = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkIsWindowsAuthentication);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtDataSource);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtUserPassword);
            this.groupBox1.Controls.Add(this.txtUserName);
            this.groupBox1.Location = new System.Drawing.Point(17, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(295, 135);
            this.groupBox1.TabIndex = 64;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Super LogIn";
            // 
            // chkIsWindowsAuthentication
            // 
            this.chkIsWindowsAuthentication.AutoSize = true;
            this.chkIsWindowsAuthentication.Location = new System.Drawing.Point(103, 105);
            this.chkIsWindowsAuthentication.Name = "chkIsWindowsAuthentication";
            this.chkIsWindowsAuthentication.Size = new System.Drawing.Size(141, 17);
            this.chkIsWindowsAuthentication.TabIndex = 69;
            this.chkIsWindowsAuthentication.Text = "Windows Authentication";
            this.chkIsWindowsAuthentication.UseVisualStyleBackColor = true;
            this.chkIsWindowsAuthentication.CheckedChanged += new System.EventHandler(this.chkIsWindowsAuthentication_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(10, 80);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(87, 17);
            this.label2.TabIndex = 129;
            this.label2.Text = "Server Name";
            // 
            // txtDataSource
            // 
            this.txtDataSource.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDataSource.Location = new System.Drawing.Point(103, 77);
            this.txtDataSource.MaximumSize = new System.Drawing.Size(4, 4);
            this.txtDataSource.MinimumSize = new System.Drawing.Size(175, 22);
            this.txtDataSource.Name = "txtDataSource";
            this.txtDataSource.Size = new System.Drawing.Size(175, 22);
            this.txtDataSource.TabIndex = 2;
            this.txtDataSource.Text = ".";
            this.txtDataSource.TextChanged += new System.EventHandler(this.txtDataSource_TextChanged);
            this.txtDataSource.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtDataSource_KeyDown);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(10, 52);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(66, 17);
            this.label4.TabIndex = 125;
            this.label4.Text = "Password";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(10, 22);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 17);
            this.label3.TabIndex = 124;
            this.label3.Text = "Login";
            // 
            // txtUserPassword
            // 
            this.txtUserPassword.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtUserPassword.Location = new System.Drawing.Point(103, 47);
            this.txtUserPassword.MaximumSize = new System.Drawing.Size(4, 4);
            this.txtUserPassword.MinimumSize = new System.Drawing.Size(175, 22);
            this.txtUserPassword.Name = "txtUserPassword";
            this.txtUserPassword.PasswordChar = '*';
            this.txtUserPassword.Size = new System.Drawing.Size(175, 22);
            this.txtUserPassword.TabIndex = 1;
            this.txtUserPassword.UseSystemPasswordChar = true;
            this.txtUserPassword.TextChanged += new System.EventHandler(this.txtUserPassword_TextChanged);
            this.txtUserPassword.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtUserPassword_KeyDown);
            // 
            // txtUserName
            // 
            this.txtUserName.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtUserName.Location = new System.Drawing.Point(103, 20);
            this.txtUserName.MaximumSize = new System.Drawing.Size(4, 4);
            this.txtUserName.MinimumSize = new System.Drawing.Size(175, 22);
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.Size = new System.Drawing.Size(175, 22);
            this.txtUserName.TabIndex = 0;
            this.txtUserName.Text = "sa";
            this.txtUserName.TextChanged += new System.EventHandler(this.txtUserName_TextChanged);
            this.txtUserName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtUserName_KeyDown);
            // 
            // btnExit
            // 
            this.btnExit.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnExit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnExit.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExit.Image = global::VATClient.Properties.Resources.Back;
            this.btnExit.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnExit.Location = new System.Drawing.Point(219, 152);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(97, 35);
            this.btnExit.TabIndex = 5;
            this.btnExit.Text = "Back";
            this.btnExit.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnExit.UseVisualStyleBackColor = false;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnLogIn
            // 
            this.btnLogIn.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnLogIn.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLogIn.Image = global::VATClient.Properties.Resources.Save;
            this.btnLogIn.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnLogIn.Location = new System.Drawing.Point(116, 152);
            this.btnLogIn.Name = "btnLogIn";
            this.btnLogIn.Size = new System.Drawing.Size(97, 35);
            this.btnLogIn.TabIndex = 4;
            this.btnLogIn.Text = "&Save";
            this.btnLogIn.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnLogIn.UseVisualStyleBackColor = false;
            this.btnLogIn.Click += new System.EventHandler(this.btnLogIn_Click);
            // 
            // btnTestConn
            // 
            this.btnTestConn.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnTestConn.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTestConn.Image = global::VATClient.Properties.Resources.Post;
            this.btnTestConn.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnTestConn.Location = new System.Drawing.Point(13, 152);
            this.btnTestConn.Name = "btnTestConn";
            this.btnTestConn.Size = new System.Drawing.Size(97, 35);
            this.btnTestConn.TabIndex = 3;
            this.btnTestConn.Text = "Test Conn";
            this.btnTestConn.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnTestConn.UseVisualStyleBackColor = false;
            this.btnTestConn.Click += new System.EventHandler(this.btnTestConn_Click);
            // 
            // FormSuperInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(330, 223);
            this.Controls.Add(this.btnTestConn);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnLogIn);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormSuperInfo";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Database Login";
            this.Load += new System.EventHandler(this.FormSuperInfo_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtDataSource;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtUserPassword;
        private System.Windows.Forms.TextBox txtUserName;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Button btnLogIn;
        private System.Windows.Forms.Button btnTestConn;
        private System.Windows.Forms.CheckBox chkIsWindowsAuthentication;
    }
}