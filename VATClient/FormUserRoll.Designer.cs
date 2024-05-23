namespace VATClient
{
    partial class FormUserRoll
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgvRoll = new System.Windows.Forms.DataGridView();
            this.Sl = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Root = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Child = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ChildChild = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Access = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Post = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Add = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Update = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnUserSearch = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtUserName = new System.Windows.Forms.TextBox();
            this.txtUserID = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.btnSellectAll = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.backgroundWorkerSave = new System.ComponentModel.BackgroundWorker();
            this.backgroundWorkerSearch = new System.ComponentModel.BackgroundWorker();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.backgroundWorkerUserRollSearchClose = new System.ComponentModel.BackgroundWorker();
            this.btnAllAdd = new System.Windows.Forms.Button();
            this.btnNoAdd = new System.Windows.Forms.Button();
            this.btnAllUpdate = new System.Windows.Forms.Button();
            this.btnNoUpdate = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRoll)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvRoll
            // 
            this.dgvRoll.AllowUserToAddRows = false;
            this.dgvRoll.AllowUserToOrderColumns = true;
            this.dgvRoll.AllowUserToResizeRows = false;
            this.dgvRoll.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvRoll.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvRoll.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvRoll.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Sl,
            this.Root,
            this.Child,
            this.ChildChild,
            this.Access,
            this.Post,
            this.Add,
            this.Update,
            this.ID});
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle9.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle9.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle9.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle9.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvRoll.DefaultCellStyle = dataGridViewCellStyle9;
            this.dgvRoll.GridColor = System.Drawing.SystemColors.HotTrack;
            this.dgvRoll.Location = new System.Drawing.Point(9, 33);
            this.dgvRoll.Name = "dgvRoll";
            this.dgvRoll.ReadOnly = true;
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle10.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle10.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle10.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle10.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle10.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvRoll.RowHeadersDefaultCellStyle = dataGridViewCellStyle10;
            this.dgvRoll.RowHeadersVisible = false;
            this.dgvRoll.Size = new System.Drawing.Size(695, 459);
            this.dgvRoll.TabIndex = 58;
            this.dgvRoll.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvRoll_CellContentClick);
            this.dgvRoll.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvRoll_CellMouseDoubleClick);
            this.dgvRoll.Scroll += new System.Windows.Forms.ScrollEventHandler(this.dgvRoll_Scroll);
            this.dgvRoll.DoubleClick += new System.EventHandler(this.dgvRoll_DoubleClick);
            // 
            // Sl
            // 
            this.Sl.Frozen = true;
            this.Sl.HeaderText = "SL";
            this.Sl.Name = "Sl";
            this.Sl.ReadOnly = true;
            this.Sl.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Sl.Width = 40;
            // 
            // Root
            // 
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.Lime;
            this.Root.DefaultCellStyle = dataGridViewCellStyle2;
            this.Root.Frozen = true;
            this.Root.HeaderText = "Root";
            this.Root.Name = "Root";
            this.Root.ReadOnly = true;
            this.Root.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Root.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Root.Width = 130;
            // 
            // Child
            // 
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.Yellow;
            this.Child.DefaultCellStyle = dataGridViewCellStyle3;
            this.Child.Frozen = true;
            this.Child.HeaderText = "Child";
            this.Child.Name = "Child";
            this.Child.ReadOnly = true;
            this.Child.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Child.Width = 130;
            // 
            // ChildChild
            // 
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.ChildChild.DefaultCellStyle = dataGridViewCellStyle4;
            this.ChildChild.Frozen = true;
            this.ChildChild.HeaderText = "Child-Child";
            this.ChildChild.Name = "ChildChild";
            this.ChildChild.ReadOnly = true;
            this.ChildChild.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ChildChild.Width = 130;
            // 
            // Access
            // 
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Access.DefaultCellStyle = dataGridViewCellStyle5;
            this.Access.Frozen = true;
            this.Access.HeaderText = "Access";
            this.Access.Name = "Access";
            this.Access.ReadOnly = true;
            this.Access.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Access.Width = 65;
            // 
            // Post
            // 
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Post.DefaultCellStyle = dataGridViewCellStyle6;
            this.Post.Frozen = true;
            this.Post.HeaderText = "Post";
            this.Post.Name = "Post";
            this.Post.ReadOnly = true;
            this.Post.Width = 65;
            // 
            // Add
            // 
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.Add.DefaultCellStyle = dataGridViewCellStyle7;
            this.Add.Frozen = true;
            this.Add.HeaderText = "Add";
            this.Add.Name = "Add";
            this.Add.ReadOnly = true;
            this.Add.Width = 65;
            // 
            // Update
            // 
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.Update.DefaultCellStyle = dataGridViewCellStyle8;
            this.Update.Frozen = true;
            this.Update.HeaderText = "Update";
            this.Update.Name = "Update";
            this.Update.ReadOnly = true;
            this.Update.Width = 65;
            // 
            // ID
            // 
            this.ID.HeaderText = "ID";
            this.ID.Name = "ID";
            this.ID.ReadOnly = true;
            this.ID.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ID.Visible = false;
            this.ID.Width = 50;
            // 
            // btnSave
            // 
            this.btnSave.Image = global::VATClient.Properties.Resources.Save;
            this.btnSave.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSave.Location = new System.Drawing.Point(711, 33);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(82, 34);
            this.btnSave.TabIndex = 3;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnUserSearch
            // 
            this.btnUserSearch.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnUserSearch.Image = global::VATClient.Properties.Resources.search;
            this.btnUserSearch.Location = new System.Drawing.Point(269, 7);
            this.btnUserSearch.Name = "btnUserSearch";
            this.btnUserSearch.Size = new System.Drawing.Size(30, 20);
            this.btnUserSearch.TabIndex = 1;
            this.btnUserSearch.TabStop = false;
            this.btnUserSearch.UseVisualStyleBackColor = false;
            this.btnUserSearch.Click += new System.EventHandler(this.btnUserSearch_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 10);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 13);
            this.label3.TabIndex = 146;
            this.label3.Text = "User Name";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(361, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 13);
            this.label2.TabIndex = 145;
            this.label2.Text = "User ID";
            this.label2.Visible = false;
            // 
            // txtUserName
            // 
            this.txtUserName.Location = new System.Drawing.Point(76, 7);
            this.txtUserName.MaximumSize = new System.Drawing.Size(4, 4);
            this.txtUserName.MinimumSize = new System.Drawing.Size(185, 20);
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.ReadOnly = true;
            this.txtUserName.Size = new System.Drawing.Size(185, 20);
            this.txtUserName.TabIndex = 0;
            this.txtUserName.TextChanged += new System.EventHandler(this.txtUserName_TextChanged);
            // 
            // txtUserID
            // 
            this.txtUserID.Location = new System.Drawing.Point(445, 8);
            this.txtUserID.MaximumSize = new System.Drawing.Size(4, 4);
            this.txtUserID.MinimumSize = new System.Drawing.Size(150, 20);
            this.txtUserID.Name = "txtUserID";
            this.txtUserID.ReadOnly = true;
            this.txtUserID.Size = new System.Drawing.Size(150, 20);
            this.txtUserID.TabIndex = 2;
            this.txtUserID.TabStop = false;
            this.txtUserID.Visible = false;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(711, 113);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(82, 34);
            this.button2.TabIndex = 5;
            this.button2.Text = "No Access";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // btnSellectAll
            // 
            this.btnSellectAll.Location = new System.Drawing.Point(711, 73);
            this.btnSellectAll.Name = "btnSellectAll";
            this.btnSellectAll.Size = new System.Drawing.Size(82, 34);
            this.btnSellectAll.TabIndex = 4;
            this.btnSellectAll.Text = "All Access";
            this.btnSellectAll.UseVisualStyleBackColor = true;
            this.btnSellectAll.Click += new System.EventHandler(this.btnSellectAll_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(730, 465);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(63, 25);
            this.button1.TabIndex = 150;
            this.button1.Text = "Merge";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button3
            // 
            this.button3.Image = global::VATClient.Properties.Resources.Back;
            this.button3.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button3.Location = new System.Drawing.Point(711, 393);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(82, 34);
            this.button3.TabIndex = 8;
            this.button3.Text = "Close";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(711, 153);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(82, 34);
            this.button4.TabIndex = 6;
            this.button4.Text = "All Post";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click_1);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(711, 193);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(82, 34);
            this.button5.TabIndex = 7;
            this.button5.Text = "No Post";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // backgroundWorkerSave
            // 
            this.backgroundWorkerSave.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerSave_DoWork);
            this.backgroundWorkerSave.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerSave_RunWorkerCompleted);
            // 
            // backgroundWorkerSearch
            // 
            this.backgroundWorkerSearch.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerSearch_DoWork);
            this.backgroundWorkerSearch.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerSearch_RunWorkerCompleted);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(147, 112);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(290, 24);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 154;
            this.progressBar1.Visible = false;
            // 
            // backgroundWorkerUserRollSearchClose
            // 
            this.backgroundWorkerUserRollSearchClose.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerUserRollSearchClose_DoWork);
            this.backgroundWorkerUserRollSearchClose.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerUserRollSearchClose_RunWorkerCompleted);
            // 
            // btnAllAdd
            // 
            this.btnAllAdd.Location = new System.Drawing.Point(711, 233);
            this.btnAllAdd.Name = "btnAllAdd";
            this.btnAllAdd.Size = new System.Drawing.Size(82, 34);
            this.btnAllAdd.TabIndex = 155;
            this.btnAllAdd.Text = "All Add";
            this.btnAllAdd.UseVisualStyleBackColor = true;
            this.btnAllAdd.Click += new System.EventHandler(this.btnAllAdd_Click);
            // 
            // btnNoAdd
            // 
            this.btnNoAdd.Location = new System.Drawing.Point(711, 273);
            this.btnNoAdd.Name = "btnNoAdd";
            this.btnNoAdd.Size = new System.Drawing.Size(82, 34);
            this.btnNoAdd.TabIndex = 156;
            this.btnNoAdd.Text = "No Add";
            this.btnNoAdd.UseVisualStyleBackColor = true;
            this.btnNoAdd.Click += new System.EventHandler(this.btnNoAdd_Click);
            // 
            // btnAllUpdate
            // 
            this.btnAllUpdate.Location = new System.Drawing.Point(711, 313);
            this.btnAllUpdate.Name = "btnAllUpdate";
            this.btnAllUpdate.Size = new System.Drawing.Size(82, 34);
            this.btnAllUpdate.TabIndex = 157;
            this.btnAllUpdate.Text = "All Update";
            this.btnAllUpdate.UseVisualStyleBackColor = true;
            this.btnAllUpdate.Click += new System.EventHandler(this.btnAllUpdate_Click);
            // 
            // btnNoUpdate
            // 
            this.btnNoUpdate.Location = new System.Drawing.Point(711, 353);
            this.btnNoUpdate.Name = "btnNoUpdate";
            this.btnNoUpdate.Size = new System.Drawing.Size(82, 34);
            this.btnNoUpdate.TabIndex = 158;
            this.btnNoUpdate.Text = "No Update";
            this.btnNoUpdate.UseVisualStyleBackColor = true;
            this.btnNoUpdate.Click += new System.EventHandler(this.btnNoUpdate_Click);
            // 
            // FormUserRoll
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(804, 501);
            this.Controls.Add(this.btnAllUpdate);
            this.Controls.Add(this.btnNoUpdate);
            this.Controls.Add(this.btnAllAdd);
            this.Controls.Add(this.btnNoAdd);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnSellectAll);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtUserName);
            this.Controls.Add(this.txtUserID);
            this.Controls.Add(this.btnUserSearch);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.dgvRoll);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(820, 540);
            this.MinimumSize = new System.Drawing.Size(700, 540);
            this.Name = "FormUserRoll";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "User Roll";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormUserRoll_FormClosing);
            this.Load += new System.EventHandler(this.FormUserRoll_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvRoll)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvRoll;
        private System.Windows.Forms.Button btnUserSearch;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtUserName;
        private System.Windows.Forms.TextBox txtUserID;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button btnSellectAll;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private System.ComponentModel.BackgroundWorker backgroundWorkerSave;
        private System.ComponentModel.BackgroundWorker backgroundWorkerSearch;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.ComponentModel.BackgroundWorker backgroundWorkerUserRollSearchClose;
        private System.Windows.Forms.Button btnAllAdd;
        private System.Windows.Forms.Button btnNoAdd;
        private System.Windows.Forms.Button btnAllUpdate;
        private System.Windows.Forms.Button btnNoUpdate;
        private System.Windows.Forms.DataGridViewTextBoxColumn Sl;
        private System.Windows.Forms.DataGridViewTextBoxColumn Root;
        private System.Windows.Forms.DataGridViewTextBoxColumn Child;
        private System.Windows.Forms.DataGridViewTextBoxColumn ChildChild;
        private System.Windows.Forms.DataGridViewTextBoxColumn Access;
        private System.Windows.Forms.DataGridViewTextBoxColumn Post;
        private System.Windows.Forms.DataGridViewTextBoxColumn Add;
        private System.Windows.Forms.DataGridViewTextBoxColumn Update;
        private System.Windows.Forms.DataGridViewTextBoxColumn ID;
        public System.Windows.Forms.Button btnSave;
    }
}