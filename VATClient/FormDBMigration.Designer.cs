namespace VATClient
{
    partial class FormDBMigration
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
            this.btnDBMigration = new System.Windows.Forms.Button();
            this.lblDbMigration = new System.Windows.Forms.Label();
            this.bgwCompanyList = new System.ComponentModel.BackgroundWorker();
            this.btnClose = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // btnDBMigration
            // 
            this.btnDBMigration.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDBMigration.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnDBMigration.Location = new System.Drawing.Point(91, 202);
            this.btnDBMigration.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnDBMigration.Name = "btnDBMigration";
            this.btnDBMigration.Size = new System.Drawing.Size(137, 44);
            this.btnDBMigration.TabIndex = 0;
            this.btnDBMigration.Text = "DB Migration";
            this.btnDBMigration.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnDBMigration.UseVisualStyleBackColor = true;
            this.btnDBMigration.Click += new System.EventHandler(this.btnDBMigration_Click);
            // 
            // lblDbMigration
            // 
            this.lblDbMigration.AutoSize = true;
            this.lblDbMigration.Location = new System.Drawing.Point(191, 48);
            this.lblDbMigration.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDbMigration.Name = "lblDbMigration";
            this.lblDbMigration.Size = new System.Drawing.Size(208, 17);
            this.lblDbMigration.TabIndex = 1;
            this.lblDbMigration.Text = "_________________________";
            this.lblDbMigration.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // bgwCompanyList
            // 
            this.bgwCompanyList.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwCompanyList_DoWork);
            this.bgwCompanyList.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwCompanyList_RunWorkerCompleted);
            // 
            // btnClose
            // 
            this.btnClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(337, 202);
            this.btnClose.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(145, 44);
            this.btnClose.TabIndex = 3;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(172, 124);
            this.progressBar1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(241, 28);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 8;
            this.progressBar1.Visible = false;
            // 
            // FormDBMigration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(597, 321);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.lblDbMigration);
            this.Controls.Add(this.btnDBMigration);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.Name = "FormDBMigration";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "DBMigration";
            this.Load += new System.EventHandler(this.FormDBMigration_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnDBMigration;
        private System.Windows.Forms.Label lblDbMigration;
        private System.ComponentModel.BackgroundWorker bgwCompanyList;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.ProgressBar progressBar1;
    }
}