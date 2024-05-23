namespace VATClient
{
    partial class FormUserBranchDetail
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormUserBranchDetail));
            this.dgvUserGrid = new System.Windows.Forms.DataGridView();
            this.Id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Username = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.UserId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BranchId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BranchName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Comments = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label3 = new System.Windows.Forms.Label();
            this.txtUserName = new System.Windows.Forms.TextBox();
            this.btnUserSearch = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.txtComments = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbBranch = new System.Windows.Forms.ComboBox();
            this.label15 = new System.Windows.Forms.Label();
            this.txtUserID = new System.Windows.Forms.TextBox();
            this.bgwUserSearch = new System.ComponentModel.BackgroundWorker();
            this.btnSave = new System.Windows.Forms.Button();
            this.bgwUserSave = new System.ComponentModel.BackgroundWorker();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnImport1 = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.btnImport = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvUserGrid)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvUserGrid
            // 
            this.dgvUserGrid.AllowUserToAddRows = false;
            this.dgvUserGrid.AllowUserToDeleteRows = false;
            this.dgvUserGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvUserGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Id,
            this.Username,
            this.UserId,
            this.BranchId,
            this.BranchName,
            this.Comments});
            this.dgvUserGrid.Location = new System.Drawing.Point(4, 111);
            this.dgvUserGrid.Name = "dgvUserGrid";
            this.dgvUserGrid.Size = new System.Drawing.Size(578, 189);
            this.dgvUserGrid.TabIndex = 0;
            this.dgvUserGrid.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvUserGrid_CellContentClick);
            // 
            // Id
            // 
            this.Id.HeaderText = "Id";
            this.Id.Name = "Id";
            this.Id.ReadOnly = true;
            this.Id.Visible = false;
            // 
            // Username
            // 
            this.Username.HeaderText = "Username";
            this.Username.Name = "Username";
            this.Username.ReadOnly = true;
            this.Username.Visible = false;
            // 
            // UserId
            // 
            this.UserId.HeaderText = "UserId";
            this.UserId.Name = "UserId";
            this.UserId.ReadOnly = true;
            this.UserId.Visible = false;
            // 
            // BranchId
            // 
            this.BranchId.HeaderText = "BranchId";
            this.BranchId.Name = "BranchId";
            this.BranchId.ReadOnly = true;
            this.BranchId.Visible = false;
            // 
            // BranchName
            // 
            this.BranchName.HeaderText = "Branch Name";
            this.BranchName.Name = "BranchName";
            this.BranchName.ReadOnly = true;
            // 
            // Comments
            // 
            this.Comments.HeaderText = "Comments";
            this.Comments.Name = "Comments";
            this.Comments.ReadOnly = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 11);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 13);
            this.label3.TabIndex = 149;
            this.label3.Text = "User Name";
            // 
            // txtUserName
            // 
            this.txtUserName.Location = new System.Drawing.Point(85, 8);
            this.txtUserName.MaximumSize = new System.Drawing.Size(4, 4);
            this.txtUserName.MinimumSize = new System.Drawing.Size(185, 20);
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.ReadOnly = true;
            this.txtUserName.Size = new System.Drawing.Size(185, 20);
            this.txtUserName.TabIndex = 147;
            // 
            // btnUserSearch
            // 
            this.btnUserSearch.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnUserSearch.Image = global::VATClient.Properties.Resources.search;
            this.btnUserSearch.Location = new System.Drawing.Point(278, 8);
            this.btnUserSearch.Name = "btnUserSearch";
            this.btnUserSearch.Size = new System.Drawing.Size(30, 20);
            this.btnUserSearch.TabIndex = 148;
            this.btnUserSearch.TabStop = false;
            this.btnUserSearch.UseVisualStyleBackColor = false;
            this.btnUserSearch.Click += new System.EventHandler(this.btnUserSearch_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnRemove);
            this.groupBox1.Controls.Add(this.btnAdd);
            this.groupBox1.Controls.Add(this.txtComments);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.cmbBranch);
            this.groupBox1.Controls.Add(this.label15);
            this.groupBox1.Location = new System.Drawing.Point(4, 29);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(577, 75);
            this.groupBox1.TabIndex = 150;
            this.groupBox1.TabStop = false;
            this.groupBox1.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // btnRemove
            // 
            this.btnRemove.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnRemove.Image = ((System.Drawing.Image)(resources.GetObject("btnRemove.Image")));
            this.btnRemove.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnRemove.Location = new System.Drawing.Point(424, 46);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(74, 21);
            this.btnRemove.TabIndex = 231;
            this.btnRemove.TabStop = false;
            this.btnRemove.Text = "Remove";
            this.btnRemove.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnRemove.UseVisualStyleBackColor = false;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnAdd.Image = ((System.Drawing.Image)(resources.GetObject("btnAdd.Image")));
            this.btnAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAdd.Location = new System.Drawing.Point(504, 46);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(63, 21);
            this.btnAdd.TabIndex = 230;
            this.btnAdd.Text = "Add";
            this.btnAdd.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnAdd.UseVisualStyleBackColor = false;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // txtComments
            // 
            this.txtComments.Location = new System.Drawing.Point(302, 20);
            this.txtComments.Name = "txtComments";
            this.txtComments.Size = new System.Drawing.Size(265, 20);
            this.txtComments.TabIndex = 229;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(243, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 13);
            this.label1.TabIndex = 228;
            this.label1.Text = "Comments";
            // 
            // cmbBranch
            // 
            this.cmbBranch.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBranch.FormattingEnabled = true;
            this.cmbBranch.Items.AddRange(new object[] {
            "Y",
            "N"});
            this.cmbBranch.Location = new System.Drawing.Point(55, 19);
            this.cmbBranch.Name = "cmbBranch";
            this.cmbBranch.Size = new System.Drawing.Size(158, 21);
            this.cmbBranch.TabIndex = 227;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(11, 23);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(41, 13);
            this.label15.TabIndex = 226;
            this.label15.Text = "Branch";
            // 
            // txtUserID
            // 
            this.txtUserID.Location = new System.Drawing.Point(339, 11);
            this.txtUserID.Name = "txtUserID";
            this.txtUserID.Size = new System.Drawing.Size(100, 20);
            this.txtUserID.TabIndex = 151;
            this.txtUserID.Visible = false;
            // 
            // bgwUserSearch
            // 
            this.bgwUserSearch.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwUserSearch_DoWork);
            this.bgwUserSearch.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwUserSearch_RunWorkerCompleted);
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSave.Image = global::VATClient.Properties.Resources.Save;
            this.btnSave.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSave.Location = new System.Drawing.Point(4, 310);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(62, 28);
            this.btnSave.TabIndex = 152;
            this.btnSave.Text = " Save";
            this.btnSave.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // bgwUserSave
            // 
            this.bgwUserSave.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwUserSave_DoWork);
            this.bgwUserSave.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwUserSave_RunWorkerCompleted);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(218, 216);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(221, 24);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 206;
            this.progressBar1.Visible = false;
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Image = global::VATClient.Properties.Resources.Back;
            this.btnClose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnClose.Location = new System.Drawing.Point(506, 310);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 28);
            this.btnClose.TabIndex = 207;
            this.btnClose.Text = "&Close";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnImport1
            // 
            this.btnImport1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnImport1.Location = new System.Drawing.Point(496, 7);
            this.btnImport1.Name = "btnImport1";
            this.btnImport1.Size = new System.Drawing.Size(75, 23);
            this.btnImport1.TabIndex = 232;
            this.btnImport1.Text = "Import";
            this.btnImport1.UseVisualStyleBackColor = true;
            this.btnImport1.Visible = false;
            this.btnImport1.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.Red;
            this.label6.Location = new System.Drawing.Point(68, 13);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(14, 14);
            this.label6.TabIndex = 233;
            this.label6.Text = "*";
            // 
            // btnImport
            // 
            this.btnImport.Image = global::VATClient.Properties.Resources.Load;
            this.btnImport.Location = new System.Drawing.Point(455, 8);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(35, 23);
            this.btnImport.TabIndex = 234;
            this.btnImport.UseVisualStyleBackColor = true;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click_1);
            // 
            // FormUserBranchDetail
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(584, 350);
            this.Controls.Add(this.btnImport);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.btnImport1);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.txtUserID);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtUserName);
            this.Controls.Add(this.btnUserSearch);
            this.Controls.Add(this.dgvUserGrid);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormUserBranchDetail";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "User Branch";
            this.Load += new System.EventHandler(this.FormUserBranchDetail_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvUserGrid)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvUserGrid;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtUserName;
        private System.Windows.Forms.Button btnUserSearch;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox cmbBranch;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox txtComments;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtUserID;
        private System.ComponentModel.BackgroundWorker bgwUserSearch;
        private System.Windows.Forms.DataGridViewTextBoxColumn Id;
        private System.Windows.Forms.DataGridViewTextBoxColumn Username;
        private System.Windows.Forms.DataGridViewTextBoxColumn UserId;
        private System.Windows.Forms.DataGridViewTextBoxColumn BranchId;
        private System.Windows.Forms.DataGridViewTextBoxColumn BranchName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Comments;
        private System.ComponentModel.BackgroundWorker bgwUserSave;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnImport1;
        public System.Windows.Forms.Button btnRemove;
        public System.Windows.Forms.Button btnAdd;
        public System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnImport;
    }
}