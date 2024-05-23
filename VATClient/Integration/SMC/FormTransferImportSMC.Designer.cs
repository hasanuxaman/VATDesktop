namespace VATClient
{
    partial class FormTransferImportSMC
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
            this.btnLoad = new System.Windows.Forms.Button();
            this.btnImport = new System.Windows.Forms.Button();
            this.dgvLoadedTable = new System.Windows.Forms.DataGridView();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.bgwTransferIssue = new System.ComponentModel.BackgroundWorker();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dtpSaleToDate = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.dtpSaleFromDate = new System.Windows.Forms.DateTimePicker();
            this.txtId = new System.Windows.Forms.TextBox();
            this.bgwBigData = new System.ComponentModel.BackgroundWorker();
            this.lblRecord = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLoadedTable)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnLoad
            // 
            this.btnLoad.BackColor = System.Drawing.Color.White;
            this.btnLoad.Image = global::VATClient.Properties.Resources.Referesh;
            this.btnLoad.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnLoad.Location = new System.Drawing.Point(409, 25);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(60, 23);
            this.btnLoad.TabIndex = 4;
            this.btnLoad.Text = "Load";
            this.btnLoad.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnLoad.UseVisualStyleBackColor = false;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // btnImport
            // 
            this.btnImport.BackColor = System.Drawing.Color.White;
            this.btnImport.Image = global::VATClient.Properties.Resources.Load;
            this.btnImport.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnImport.Location = new System.Drawing.Point(475, 25);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(75, 23);
            this.btnImport.TabIndex = 5;
            this.btnImport.Text = "Save";
            this.btnImport.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnImport.UseVisualStyleBackColor = false;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // dgvLoadedTable
            // 
            this.dgvLoadedTable.AllowUserToAddRows = false;
            this.dgvLoadedTable.AllowUserToDeleteRows = false;
            this.dgvLoadedTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvLoadedTable.Location = new System.Drawing.Point(17, 88);
            this.dgvLoadedTable.Name = "dgvLoadedTable";
            this.dgvLoadedTable.Size = new System.Drawing.Size(657, 276);
            this.dgvLoadedTable.TabIndex = 6;
            this.dgvLoadedTable.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvLoadedTable_CellMouseDoubleClick);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(232, 226);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(181, 23);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 7;
            this.progressBar1.Visible = false;
            // 
            // bgwTransferIssue
            // 
            this.bgwTransferIssue.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwTransferIssue_DoWork);
            this.bgwTransferIssue.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwTransferIssue_RunWorkerCompleted);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dtpSaleToDate);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.dtpSaleFromDate);
            this.groupBox1.Controls.Add(this.txtId);
            this.groupBox1.Controls.Add(this.btnLoad);
            this.groupBox1.Controls.Add(this.btnImport);
            this.groupBox1.Location = new System.Drawing.Point(12, 5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(657, 77);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            // 
            // dtpSaleToDate
            // 
            this.dtpSaleToDate.Checked = false;
            this.dtpSaleToDate.CustomFormat = "dd/MMM/yyyy";
            this.dtpSaleToDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpSaleToDate.Location = new System.Drawing.Point(258, 28);
            this.dtpSaleToDate.Name = "dtpSaleToDate";
            this.dtpSaleToDate.ShowCheckBox = true;
            this.dtpSaleToDate.Size = new System.Drawing.Size(103, 20);
            this.dtpSaleToDate.TabIndex = 19;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 10);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(18, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "ID";
            // 
            // dtpSaleFromDate
            // 
            this.dtpSaleFromDate.Checked = false;
            this.dtpSaleFromDate.CustomFormat = "dd/MMM/yyyy";
            this.dtpSaleFromDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpSaleFromDate.Location = new System.Drawing.Point(149, 28);
            this.dtpSaleFromDate.Name = "dtpSaleFromDate";
            this.dtpSaleFromDate.ShowCheckBox = true;
            this.dtpSaleFromDate.Size = new System.Drawing.Size(103, 20);
            this.dtpSaleFromDate.TabIndex = 18;
            // 
            // txtId
            // 
            this.txtId.Location = new System.Drawing.Point(12, 27);
            this.txtId.Name = "txtId";
            this.txtId.Size = new System.Drawing.Size(128, 20);
            this.txtId.TabIndex = 9;
            // 
            // lblRecord
            // 
            this.lblRecord.AutoSize = true;
            this.lblRecord.Location = new System.Drawing.Point(68, 367);
            this.lblRecord.Name = "lblRecord";
            this.lblRecord.Size = new System.Drawing.Size(84, 13);
            this.lblRecord.TabIndex = 13;
            this.lblRecord.Text = "Records Count: ";
            // 
            // FormTransferImportSMC
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(699, 386);
            this.Controls.Add(this.lblRecord);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.dgvLoadedTable);
            this.Name = "FormTransferImportSMC";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Master Import";
            this.Load += new System.EventHandler(this.FormMasterImport_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvLoadedTable)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.DataGridView dgvLoadedTable;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button btnImport;
        private System.ComponentModel.BackgroundWorker bgwTransferIssue;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtId;
        private System.Windows.Forms.Label label3;
        private System.ComponentModel.BackgroundWorker bgwBigData;
        private System.Windows.Forms.Label lblRecord;
        private System.Windows.Forms.DateTimePicker dtpSaleToDate;
        private System.Windows.Forms.DateTimePicker dtpSaleFromDate;
    }
}