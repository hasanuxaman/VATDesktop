namespace VATClient
{
    partial class FormDBBackupRestore
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormDBBackupRestore));
            this.saveBakFile = new System.Windows.Forms.SaveFileDialog();
            this.openBakFile = new System.Windows.Forms.OpenFileDialog();
            this.grpRestore = new System.Windows.Forms.GroupBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.chckReplace = new System.Windows.Forms.CheckBox();
            this.btnRestore = new System.Windows.Forms.Button();
            this.btnFileToRestore = new System.Windows.Forms.Button();
            this.cmbRestoreDb = new System.Windows.Forms.ComboBox();
            this.txtFileToRestore = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.grpBackup = new System.Windows.Forms.GroupBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.btnFileToBack = new System.Windows.Forms.Button();
            this.cmbBackupMode = new System.Windows.Forms.ComboBox();
            this.btnBackupDb = new System.Windows.Forms.Button();
            this.txtFileToBack = new System.Windows.Forms.TextBox();
            this.cmbBackupDb = new System.Windows.Forms.ComboBox();
            this.lblBackupMode = new System.Windows.Forms.Label();
            this.lblFileToBack = new System.Windows.Forms.Label();
            this.lblDb = new System.Windows.Forms.Label();
            this.grpRestore.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.grpBackup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // saveBakFile
            // 
            this.saveBakFile.Filter = "Backup Files (*.bak, *.trn)|*.bak;*.trn|All files (*)|*.*";
            this.saveBakFile.Title = "Select file for saving backup";
            this.saveBakFile.FileOk += new System.ComponentModel.CancelEventHandler(this.saveBakFile_FileOk);
            // 
            // openBakFile
            // 
            this.openBakFile.Filter = "Backup Files (*.bak, *.trn)|*.bak;*.trn|All files (*)|*.*";
            this.openBakFile.Title = "Select Backup File";
            this.openBakFile.FileOk += new System.ComponentModel.CancelEventHandler(this.openBakFile_FileOk);
            // 
            // grpRestore
            // 
            this.grpRestore.Controls.Add(this.pictureBox2);
            this.grpRestore.Controls.Add(this.chckReplace);
            this.grpRestore.Controls.Add(this.btnRestore);
            this.grpRestore.Controls.Add(this.btnFileToRestore);
            this.grpRestore.Controls.Add(this.cmbRestoreDb);
            this.grpRestore.Controls.Add(this.txtFileToRestore);
            this.grpRestore.Controls.Add(this.label1);
            this.grpRestore.Controls.Add(this.label2);
            this.grpRestore.Location = new System.Drawing.Point(-7, 207);
            this.grpRestore.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.grpRestore.Name = "grpRestore";
            this.grpRestore.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.grpRestore.Size = new System.Drawing.Size(575, 202);
            this.grpRestore.TabIndex = 1;
            this.grpRestore.TabStop = false;
            this.grpRestore.Text = "Restore";
            this.grpRestore.Visible = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::VATClient.Properties.Resources.database_up;
            this.pictureBox2.Location = new System.Drawing.Point(8, 25);
            this.pictureBox2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(67, 62);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox2.TabIndex = 6;
            this.pictureBox2.TabStop = false;
            // 
            // chckReplace
            // 
            this.chckReplace.AutoSize = true;
            this.chckReplace.Checked = true;
            this.chckReplace.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chckReplace.Enabled = false;
            this.chckReplace.Location = new System.Drawing.Point(231, 91);
            this.chckReplace.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chckReplace.Name = "chckReplace";
            this.chckReplace.Size = new System.Drawing.Size(82, 21);
            this.chckReplace.TabIndex = 5;
            this.chckReplace.Text = "Replace";
            this.chckReplace.UseVisualStyleBackColor = true;
            // 
            // btnRestore
            // 
            this.btnRestore.Location = new System.Drawing.Point(227, 119);
            this.btnRestore.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnRestore.Name = "btnRestore";
            this.btnRestore.Size = new System.Drawing.Size(89, 28);
            this.btnRestore.TabIndex = 3;
            this.btnRestore.Text = "Restore";
            this.btnRestore.UseVisualStyleBackColor = true;
            this.btnRestore.Click += new System.EventHandler(this.btnRestore_Click);
            // 
            // btnFileToRestore
            // 
            this.btnFileToRestore.Location = new System.Drawing.Point(461, 55);
            this.btnFileToRestore.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnFileToRestore.Name = "btnFileToRestore";
            this.btnFileToRestore.Size = new System.Drawing.Size(89, 28);
            this.btnFileToRestore.TabIndex = 3;
            this.btnFileToRestore.Text = "Browse...";
            this.btnFileToRestore.UseVisualStyleBackColor = true;
            this.btnFileToRestore.Click += new System.EventHandler(this.btnFileToRestore_Click);
            // 
            // cmbRestoreDb
            // 
            this.cmbRestoreDb.DisplayMember = "Name";
            this.cmbRestoreDb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRestoreDb.FormattingEnabled = true;
            this.cmbRestoreDb.Location = new System.Drawing.Point(231, 25);
            this.cmbRestoreDb.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmbRestoreDb.Name = "cmbRestoreDb";
            this.cmbRestoreDb.Size = new System.Drawing.Size(221, 24);
            this.cmbRestoreDb.TabIndex = 1;
            this.cmbRestoreDb.ValueMember = "ID";
            // 
            // txtFileToRestore
            // 
            this.txtFileToRestore.Location = new System.Drawing.Point(231, 58);
            this.txtFileToRestore.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtFileToRestore.Name = "txtFileToRestore";
            this.txtFileToRestore.ReadOnly = true;
            this.txtFileToRestore.Size = new System.Drawing.Size(221, 22);
            this.txtFileToRestore.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(149, 28);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Select db:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(97, 62);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(123, 17);
            this.label2.TabIndex = 0;
            this.label2.Text = "Select backup file:";
            // 
            // grpBackup
            // 
            this.grpBackup.Controls.Add(this.pictureBox1);
            this.grpBackup.Controls.Add(this.progressBar1);
            this.grpBackup.Controls.Add(this.grpRestore);
            this.grpBackup.Controls.Add(this.btnFileToBack);
            this.grpBackup.Controls.Add(this.cmbBackupMode);
            this.grpBackup.Controls.Add(this.btnBackupDb);
            this.grpBackup.Controls.Add(this.txtFileToBack);
            this.grpBackup.Controls.Add(this.cmbBackupDb);
            this.grpBackup.Controls.Add(this.lblBackupMode);
            this.grpBackup.Controls.Add(this.lblFileToBack);
            this.grpBackup.Controls.Add(this.lblDb);
            this.grpBackup.Location = new System.Drawing.Point(16, 9);
            this.grpBackup.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.grpBackup.Name = "grpBackup";
            this.grpBackup.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.grpBackup.Size = new System.Drawing.Size(753, 182);
            this.grpBackup.TabIndex = 0;
            this.grpBackup.TabStop = false;
            this.grpBackup.Text = "Backup";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::VATClient.Properties.Resources.database_down;
            this.pictureBox1.Location = new System.Drawing.Point(12, 25);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(67, 62);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 6;
            this.pictureBox1.TabStop = false;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(247, 154);
            this.progressBar1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(387, 28);
            this.progressBar1.TabIndex = 18;
            this.progressBar1.Visible = false;
            // 
            // btnFileToBack
            // 
            this.btnFileToBack.Location = new System.Drawing.Point(656, 57);
            this.btnFileToBack.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnFileToBack.Name = "btnFileToBack";
            this.btnFileToBack.Size = new System.Drawing.Size(76, 28);
            this.btnFileToBack.TabIndex = 3;
            this.btnFileToBack.Text = "Browse...";
            this.btnFileToBack.UseVisualStyleBackColor = true;
            this.btnFileToBack.Click += new System.EventHandler(this.btnFileToBack_Click);
            // 
            // cmbBackupMode
            // 
            this.cmbBackupMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBackupMode.Enabled = false;
            this.cmbBackupMode.FormattingEnabled = true;
            this.cmbBackupMode.Items.AddRange(new object[] {
            "Overwrite",
            "Append"});
            this.cmbBackupMode.Location = new System.Drawing.Point(208, 91);
            this.cmbBackupMode.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmbBackupMode.Name = "cmbBackupMode";
            this.cmbBackupMode.Size = new System.Drawing.Size(165, 24);
            this.cmbBackupMode.TabIndex = 5;
            // 
            // btnBackupDb
            // 
            this.btnBackupDb.Location = new System.Drawing.Point(208, 124);
            this.btnBackupDb.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnBackupDb.Name = "btnBackupDb";
            this.btnBackupDb.Size = new System.Drawing.Size(89, 41);
            this.btnBackupDb.TabIndex = 3;
            this.btnBackupDb.Text = "Backup";
            this.btnBackupDb.UseVisualStyleBackColor = true;
            this.btnBackupDb.Click += new System.EventHandler(this.btnBackupDb_Click);
            // 
            // txtFileToBack
            // 
            this.txtFileToBack.Location = new System.Drawing.Point(208, 58);
            this.txtFileToBack.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtFileToBack.Name = "txtFileToBack";
            this.txtFileToBack.ReadOnly = true;
            this.txtFileToBack.Size = new System.Drawing.Size(439, 22);
            this.txtFileToBack.TabIndex = 2;
            // 
            // cmbBackupDb
            // 
            this.cmbBackupDb.DisplayMember = "Name";
            this.cmbBackupDb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBackupDb.FormattingEnabled = true;
            this.cmbBackupDb.Location = new System.Drawing.Point(208, 21);
            this.cmbBackupDb.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmbBackupDb.Name = "cmbBackupDb";
            this.cmbBackupDb.Size = new System.Drawing.Size(439, 24);
            this.cmbBackupDb.TabIndex = 1;
            this.cmbBackupDb.ValueMember = "ID";
            // 
            // lblBackupMode
            // 
            this.lblBackupMode.AutoSize = true;
            this.lblBackupMode.Location = new System.Drawing.Point(101, 95);
            this.lblBackupMode.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblBackupMode.Name = "lblBackupMode";
            this.lblBackupMode.Size = new System.Drawing.Size(98, 17);
            this.lblBackupMode.TabIndex = 0;
            this.lblBackupMode.Text = "Backup mode:";
            // 
            // lblFileToBack
            // 
            this.lblFileToBack.AutoSize = true;
            this.lblFileToBack.Location = new System.Drawing.Point(81, 62);
            this.lblFileToBack.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblFileToBack.Name = "lblFileToBack";
            this.lblFileToBack.Size = new System.Drawing.Size(113, 17);
            this.lblFileToBack.TabIndex = 0;
            this.lblFileToBack.Text = "Path to save file:";
            // 
            // lblDb
            // 
            this.lblDb.AutoSize = true;
            this.lblDb.Location = new System.Drawing.Point(127, 28);
            this.lblDb.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDb.Name = "lblDb";
            this.lblDb.Size = new System.Drawing.Size(71, 17);
            this.lblDb.TabIndex = 0;
            this.lblDb.Text = "Select db:";
            // 
            // FormDBBackupRestore
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(785, 206);
            this.Controls.Add(this.grpBackup);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.Name = "FormDBBackupRestore";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "DBBackup";
            this.Load += new System.EventHandler(this.FormDBBackupRestore_Load);
            this.grpRestore.ResumeLayout(false);
            this.grpRestore.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.grpBackup.ResumeLayout(false);
            this.grpBackup.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SaveFileDialog saveBakFile;
        private System.Windows.Forms.OpenFileDialog openBakFile;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.GroupBox grpRestore;
        private System.Windows.Forms.CheckBox chckReplace;
        private System.Windows.Forms.Button btnRestore;
        private System.Windows.Forms.Button btnFileToRestore;
        private System.Windows.Forms.ComboBox cmbRestoreDb;
        private System.Windows.Forms.TextBox txtFileToRestore;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox grpBackup;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ComboBox cmbBackupMode;
        private System.Windows.Forms.Button btnBackupDb;
        private System.Windows.Forms.Button btnFileToBack;
        private System.Windows.Forms.TextBox txtFileToBack;
        private System.Windows.Forms.ComboBox cmbBackupDb;
        private System.Windows.Forms.Label lblBackupMode;
        private System.Windows.Forms.Label lblFileToBack;
        private System.Windows.Forms.Label lblDb;
        private System.Windows.Forms.ProgressBar progressBar1;
    }
}