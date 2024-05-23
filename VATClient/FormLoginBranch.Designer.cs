namespace VATClient
{
    partial class FormLoginBranch
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgvUserBranch = new System.Windows.Forms.DataGridView();
            this.BranchId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BranchCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BranchName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Address = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.bgwBranchLoad = new System.ComponentModel.BackgroundWorker();
            ((System.ComponentModel.ISupportInitialize)(this.dgvUserBranch)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvUserBranch
            // 
            this.dgvUserBranch.AllowUserToAddRows = false;
            this.dgvUserBranch.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black;
            this.dgvUserBranch.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvUserBranch.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.dgvUserBranch.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvUserBranch.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.BranchId,
            this.BranchCode,
            this.BranchName,
            this.Address});
            this.dgvUserBranch.Location = new System.Drawing.Point(0, 0);
            this.dgvUserBranch.Name = "dgvUserBranch";
            this.dgvUserBranch.RowHeadersVisible = false;
            this.dgvUserBranch.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dgvUserBranch.Size = new System.Drawing.Size(484, 315);
            this.dgvUserBranch.TabIndex = 0;
            this.dgvUserBranch.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvUserBranch_CellContentClick);
            this.dgvUserBranch.DoubleClick += new System.EventHandler(this.dgvUserBranch_DoubleClick);
            this.dgvUserBranch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgvUserBranch_KeyDown);
            this.dgvUserBranch.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.dgvUserBranch_KeyPress);
            // 
            // BranchId
            // 
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BranchId.DefaultCellStyle = dataGridViewCellStyle2;
            this.BranchId.HeaderText = "Branch Id";
            this.BranchId.Name = "BranchId";
            this.BranchId.ReadOnly = true;
            this.BranchId.Visible = false;
            // 
            // BranchCode
            // 
            this.BranchCode.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BranchCode.DefaultCellStyle = dataGridViewCellStyle3;
            this.BranchCode.HeaderText = "Branch Code";
            this.BranchCode.Name = "BranchCode";
            this.BranchCode.ReadOnly = true;
            // 
            // BranchName
            // 
            this.BranchName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BranchName.DefaultCellStyle = dataGridViewCellStyle4;
            this.BranchName.FillWeight = 150F;
            this.BranchName.HeaderText = "Branch Name";
            this.BranchName.Name = "BranchName";
            this.BranchName.ReadOnly = true;
            this.BranchName.Width = 150;
            // 
            // Address
            // 
            this.Address.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Address.DefaultCellStyle = dataGridViewCellStyle5;
            this.Address.FillWeight = 250F;
            this.Address.HeaderText = "Address";
            this.Address.Name = "Address";
            this.Address.ReadOnly = true;
            this.Address.Width = 250;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(66, 119);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(199, 24);
            this.progressBar1.TabIndex = 206;
            this.progressBar1.Visible = false;
            // 
            // bgwBranchLoad
            // 
            this.bgwBranchLoad.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwBranchLoad_DoWork);
            this.bgwBranchLoad.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwBranchLoad_RunWorkerCompleted);
            // 
            // FormLoginBranch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(484, 315);
            this.ControlBox = false;
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.dgvUserBranch);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormLoginBranch";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Login Branch";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormLoginBranch_FormClosed);
            this.Load += new System.EventHandler(this.FormLoginBranch_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvUserBranch)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvUserBranch;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.ComponentModel.BackgroundWorker bgwBranchLoad;
        private System.Windows.Forms.DataGridViewTextBoxColumn BranchId;
        private System.Windows.Forms.DataGridViewTextBoxColumn BranchCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn BranchName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Address;
    }
}