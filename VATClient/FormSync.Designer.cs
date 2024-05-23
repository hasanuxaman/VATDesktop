namespace VATClient
{
    partial class FormSync
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
            this.cmbTable = new System.Windows.Forms.ComboBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.dgvLoadedTable = new System.Windows.Forms.DataGridView();
            this.cmbBranch = new System.Windows.Forms.ComboBox();
            this.backgroundWorkerBranch = new System.ComponentModel.BackgroundWorker();
            this.btnLoad = new System.Windows.Forms.Button();
            this.btnSync = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.backgroundWorkerSynchronize = new System.ComponentModel.BackgroundWorker();
            this.cmbProductType = new System.Windows.Forms.ComboBox();
            this.bgwProductTypes = new System.ComponentModel.BackgroundWorker();
            this.txtCategoryId = new System.Windows.Forms.TextBox();
            this.btnExport = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLoadedTable)).BeginInit();
            this.SuspendLayout();
            // 
            // cmbTable
            // 
            this.cmbTable.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTable.FormattingEnabled = true;
            this.cmbTable.Location = new System.Drawing.Point(12, 25);
            this.cmbTable.Name = "cmbTable";
            this.cmbTable.Size = new System.Drawing.Size(121, 21);
            this.cmbTable.TabIndex = 207;
            this.cmbTable.SelectedIndexChanged += new System.EventHandler(this.cmbTable_SelectedIndexChanged);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(195, 205);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(218, 23);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 208;
            this.progressBar1.Visible = false;
            // 
            // dgvLoadedTable
            // 
            this.dgvLoadedTable.AllowUserToAddRows = false;
            this.dgvLoadedTable.AllowUserToDeleteRows = false;
            this.dgvLoadedTable.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.ColumnHeader;
            this.dgvLoadedTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvLoadedTable.Location = new System.Drawing.Point(12, 61);
            this.dgvLoadedTable.Name = "dgvLoadedTable";
            this.dgvLoadedTable.Size = new System.Drawing.Size(668, 288);
            this.dgvLoadedTable.TabIndex = 209;
            // 
            // cmbBranch
            // 
            this.cmbBranch.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBranch.FormattingEnabled = true;
            this.cmbBranch.Location = new System.Drawing.Point(149, 25);
            this.cmbBranch.Name = "cmbBranch";
            this.cmbBranch.Size = new System.Drawing.Size(121, 21);
            this.cmbBranch.TabIndex = 210;
            // 
            // backgroundWorkerBranch
            // 
            this.backgroundWorkerBranch.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerBranch_DoWork);
            this.backgroundWorkerBranch.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerBranch_RunWorkerCompleted);
            // 
            // btnLoad
            // 
            this.btnLoad.BackColor = System.Drawing.Color.White;
            this.btnLoad.Image = global::VATClient.Properties.Resources.search;
            this.btnLoad.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnLoad.Location = new System.Drawing.Point(277, 24);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(60, 26);
            this.btnLoad.TabIndex = 211;
            this.btnLoad.Text = "Load";
            this.btnLoad.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnLoad.UseVisualStyleBackColor = false;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // btnSync
            // 
            this.btnSync.BackColor = System.Drawing.Color.White;
            this.btnSync.Image = global::VATClient.Properties.Resources.Referesh;
            this.btnSync.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSync.Location = new System.Drawing.Point(343, 24);
            this.btnSync.Name = "btnSync";
            this.btnSync.Size = new System.Drawing.Size(60, 26);
            this.btnSync.TabIndex = 212;
            this.btnSync.Text = "Sync";
            this.btnSync.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnSync.UseVisualStyleBackColor = false;
            this.btnSync.Click += new System.EventHandler(this.btnSync_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 13);
            this.label1.TabIndex = 213;
            this.label1.Text = "Table";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(151, 6);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 13);
            this.label2.TabIndex = 213;
            this.label2.Text = "Branch";
            // 
            // backgroundWorkerSynchronize
            // 
            this.backgroundWorkerSynchronize.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerSynchronize_DoWork);
            this.backgroundWorkerSynchronize.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerSynchronize_RunWorkerCompleted);
            // 
            // cmbProductType
            // 
            this.cmbProductType.FormattingEnabled = true;
            this.cmbProductType.Location = new System.Drawing.Point(409, 25);
            this.cmbProductType.Name = "cmbProductType";
            this.cmbProductType.Size = new System.Drawing.Size(121, 21);
            this.cmbProductType.TabIndex = 214;
            this.cmbProductType.Visible = false;
            this.cmbProductType.SelectedIndexChanged += new System.EventHandler(this.cmbProductType_SelectedIndexChanged);
            // 
            // bgwProductTypes
            // 
            this.bgwProductTypes.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwProductTypes_DoWork);
            this.bgwProductTypes.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwProductTypes_RunWorkerCompleted);
            // 
            // txtCategoryId
            // 
            this.txtCategoryId.Location = new System.Drawing.Point(532, 25);
            this.txtCategoryId.Name = "txtCategoryId";
            this.txtCategoryId.Size = new System.Drawing.Size(13, 20);
            this.txtCategoryId.TabIndex = 215;
            this.txtCategoryId.Visible = false;
            // 
            // btnExport
            // 
            this.btnExport.BackColor = System.Drawing.Color.White;
            this.btnExport.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnExport.Image = global::VATClient.Properties.Resources.Print;
            this.btnExport.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnExport.Location = new System.Drawing.Point(552, 25);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(75, 23);
            this.btnExport.TabIndex = 216;
            this.btnExport.Text = "Export";
            this.btnExport.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnExport.UseVisualStyleBackColor = false;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // FormSync
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(692, 361);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.txtCategoryId);
            this.Controls.Add(this.cmbProductType);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnSync);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.cmbBranch);
            this.Controls.Add(this.dgvLoadedTable);
            this.Controls.Add(this.cmbTable);
            this.MaximizeBox = false;
            this.Name = "FormSync";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Synchronisation";
            this.Load += new System.EventHandler(this.FormSync_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvLoadedTable)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbTable;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.DataGridView dgvLoadedTable;
        private System.Windows.Forms.ComboBox cmbBranch;
        private System.ComponentModel.BackgroundWorker backgroundWorkerBranch;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Button btnSync;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.ComponentModel.BackgroundWorker backgroundWorkerSynchronize;
        private System.Windows.Forms.ComboBox cmbProductType;
        private System.ComponentModel.BackgroundWorker bgwProductTypes;
        private System.Windows.Forms.TextBox txtCategoryId;
        private System.Windows.Forms.Button btnExport;
    }
}