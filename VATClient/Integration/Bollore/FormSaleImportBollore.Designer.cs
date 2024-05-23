namespace VATClient.Integration.Bollore
{
    partial class FormSaleImportBollore
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
            this.btnSave = new System.Windows.Forms.Button();
            this.stopBulk = new System.Windows.Forms.Button();
            this.btnBulk = new System.Windows.Forms.Button();
            this.lblDB = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblRecord = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.dgvLoadedTable = new System.Windows.Forms.DataGridView();
            this.backgroundSaveSale = new System.ComponentModel.BackgroundWorker();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLoadedTable)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnSave);
            this.groupBox1.Controls.Add(this.stopBulk);
            this.groupBox1.Controls.Add(this.btnBulk);
            this.groupBox1.Controls.Add(this.lblDB);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(9, 7);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(876, 91);
            this.groupBox1.TabIndex = 221;
            this.groupBox1.TabStop = false;
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.Color.White;
            this.btnSave.Location = new System.Drawing.Point(320, 23);
            this.btnSave.Margin = new System.Windows.Forms.Padding(4);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(114, 42);
            this.btnSave.TabIndex = 20;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // stopBulk
            // 
            this.stopBulk.BackColor = System.Drawing.Color.White;
            this.stopBulk.Location = new System.Drawing.Point(750, 23);
            this.stopBulk.Margin = new System.Windows.Forms.Padding(4);
            this.stopBulk.Name = "stopBulk";
            this.stopBulk.Size = new System.Drawing.Size(118, 42);
            this.stopBulk.TabIndex = 19;
            this.stopBulk.Text = "Process Stop";
            this.stopBulk.UseVisualStyleBackColor = false;
            // 
            // btnBulk
            // 
            this.btnBulk.BackColor = System.Drawing.Color.White;
            this.btnBulk.Location = new System.Drawing.Point(613, 23);
            this.btnBulk.Margin = new System.Windows.Forms.Padding(4);
            this.btnBulk.Name = "btnBulk";
            this.btnBulk.Size = new System.Drawing.Size(114, 42);
            this.btnBulk.TabIndex = 17;
            this.btnBulk.Text = "Process Start";
            this.btnBulk.UseVisualStyleBackColor = false;
            // 
            // lblDB
            // 
            this.lblDB.AutoSize = true;
            this.lblDB.Location = new System.Drawing.Point(724, 90);
            this.lblDB.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDB.Name = "lblDB";
            this.lblDB.Size = new System.Drawing.Size(53, 17);
            this.lblDB.TabIndex = 14;
            this.lblDB.Text = "DB List";
            this.lblDB.Visible = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(204, -23);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(21, 17);
            this.label3.TabIndex = 10;
            this.label3.Text = "ID";
            // 
            // lblRecord
            // 
            this.lblRecord.AutoSize = true;
            this.lblRecord.Location = new System.Drawing.Point(89, 425);
            this.lblRecord.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblRecord.Name = "lblRecord";
            this.lblRecord.Size = new System.Drawing.Size(103, 17);
            this.lblRecord.TabIndex = 232;
            this.lblRecord.Text = "Record Count: ";
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(29, 231);
            this.progressBar1.Margin = new System.Windows.Forms.Padding(4);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(778, 50);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 231;
            this.progressBar1.Visible = false;
            // 
            // dgvLoadedTable
            // 
            this.dgvLoadedTable.AllowUserToAddRows = false;
            this.dgvLoadedTable.AllowUserToDeleteRows = false;
            this.dgvLoadedTable.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvLoadedTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvLoadedTable.Location = new System.Drawing.Point(5, 108);
            this.dgvLoadedTable.Margin = new System.Windows.Forms.Padding(4);
            this.dgvLoadedTable.Name = "dgvLoadedTable";
            this.dgvLoadedTable.ReadOnly = true;
            this.dgvLoadedTable.Size = new System.Drawing.Size(876, 314);
            this.dgvLoadedTable.TabIndex = 230;
            // 
            // backgroundSaveSale
            // 
            this.backgroundSaveSale.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundSaveSale_DoWork);
            this.backgroundSaveSale.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundSaveSale_RunWorkerCompleted);
            // 
            // FormSaleImportBollore
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(886, 453);
            this.Controls.Add(this.lblRecord);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.dgvLoadedTable);
            this.Controls.Add(this.groupBox1);
            this.MaximizeBox = false;
            this.Name = "FormSaleImportBollore";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Sale Import Bollore";
            this.Load += new System.EventHandler(this.FormSaleImportBollore_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLoadedTable)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button stopBulk;
        private System.Windows.Forms.Button btnBulk;
        private System.Windows.Forms.Label lblDB;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblRecord;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.DataGridView dgvLoadedTable;
        private System.Windows.Forms.Button btnSave;
        private System.ComponentModel.BackgroundWorker backgroundSaveSale;
    }
}