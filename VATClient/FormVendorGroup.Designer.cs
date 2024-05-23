namespace VATClient
{
    partial class FormVendorGroup
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
            this.label33 = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.cmbType = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.btnAddNew = new System.Windows.Forms.Button();
            this.btnSearch = new System.Windows.Forms.Button();
            this.txtComments = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.chkActiveStatus = new System.Windows.Forms.CheckBox();
            this.txtVendorGroupName = new System.Windows.Forms.TextBox();
            this.txtVendorGroupDescription = new System.Windows.Forms.TextBox();
            this.txtVendorGroupID = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnPrint = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.backgroundWorkerAdd = new System.ComponentModel.BackgroundWorker();
            this.backgroundWorkerDelete = new System.ComponentModel.BackgroundWorker();
            this.backgroundWorkerUpdate = new System.ComponentModel.BackgroundWorker();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label33);
            this.groupBox1.Controls.Add(this.progressBar1);
            this.groupBox1.Controls.Add(this.cmbType);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.btnAddNew);
            this.groupBox1.Controls.Add(this.btnSearch);
            this.groupBox1.Controls.Add(this.txtComments);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.chkActiveStatus);
            this.groupBox1.Controls.Add(this.txtVendorGroupName);
            this.groupBox1.Controls.Add(this.txtVendorGroupDescription);
            this.groupBox1.Controls.Add(this.txtVendorGroupID);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(3, -6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(429, 204);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // label33
            // 
            this.label33.AutoSize = true;
            this.label33.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label33.ForeColor = System.Drawing.Color.Red;
            this.label33.Location = new System.Drawing.Point(81, 40);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(14, 14);
            this.label33.TabIndex = 259;
            this.label33.Text = "*";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(117, 164);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(304, 24);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 5;
            this.progressBar1.Visible = false;
            // 
            // cmbType
            // 
            this.cmbType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbType.FormattingEnabled = true;
            this.cmbType.Items.AddRange(new object[] {
            "Local",
            "Import"});
            this.cmbType.Location = new System.Drawing.Point(95, 60);
            this.cmbType.Name = "cmbType";
            this.cmbType.Size = new System.Drawing.Size(180, 21);
            this.cmbType.TabIndex = 4;
            this.cmbType.SelectedIndexChanged += new System.EventHandler(this.cmbType_SelectedIndexChanged);
            this.cmbType.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmbType_KeyDown);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(3, 64);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(31, 13);
            this.label9.TabIndex = 185;
            this.label9.Text = "Type";
            // 
            // btnAddNew
            // 
            this.btnAddNew.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnAddNew.Image = global::VATClient.Properties.Resources.Add_New;
            this.btnAddNew.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnAddNew.Location = new System.Drawing.Point(263, 10);
            this.btnAddNew.Name = "btnAddNew";
            this.btnAddNew.Size = new System.Drawing.Size(30, 20);
            this.btnAddNew.TabIndex = 2;
            this.btnAddNew.TabStop = false;
            this.btnAddNew.UseVisualStyleBackColor = false;
            this.btnAddNew.Click += new System.EventHandler(this.btnAddNew_Click);
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSearch.Image = global::VATClient.Properties.Resources.search;
            this.btnSearch.Location = new System.Drawing.Point(227, 10);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(30, 20);
            this.btnSearch.TabIndex = 1;
            this.btnSearch.TabStop = false;
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // txtComments
            // 
            this.txtComments.Location = new System.Drawing.Point(96, 110);
            this.txtComments.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtComments.MinimumSize = new System.Drawing.Size(325, 50);
            this.txtComments.Multiline = true;
            this.txtComments.Name = "txtComments";
            this.txtComments.Size = new System.Drawing.Size(325, 50);
            this.txtComments.TabIndex = 6;
            this.txtComments.TabStop = false;
            this.txtComments.TextChanged += new System.EventHandler(this.txtComments_TextChanged);
            this.txtComments.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtComments_KeyDown);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 113);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(56, 13);
            this.label5.TabIndex = 7;
            this.label5.Text = "Comments";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 168);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(70, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Active Status";
            // 
            // chkActiveStatus
            // 
            this.chkActiveStatus.AutoSize = true;
            this.chkActiveStatus.Checked = true;
            this.chkActiveStatus.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkActiveStatus.Location = new System.Drawing.Point(96, 169);
            this.chkActiveStatus.Name = "chkActiveStatus";
            this.chkActiveStatus.Size = new System.Drawing.Size(15, 14);
            this.chkActiveStatus.TabIndex = 7;
            this.chkActiveStatus.TabStop = false;
            this.chkActiveStatus.UseVisualStyleBackColor = true;
            this.chkActiveStatus.CheckedChanged += new System.EventHandler(this.chkActiveStatus_CheckedChanged);
            // 
            // txtVendorGroupName
            // 
            this.txtVendorGroupName.Location = new System.Drawing.Point(96, 35);
            this.txtVendorGroupName.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtVendorGroupName.MinimumSize = new System.Drawing.Size(250, 20);
            this.txtVendorGroupName.Multiline = true;
            this.txtVendorGroupName.Name = "txtVendorGroupName";
            this.txtVendorGroupName.Size = new System.Drawing.Size(250, 20);
            this.txtVendorGroupName.TabIndex = 3;
            this.txtVendorGroupName.TextChanged += new System.EventHandler(this.txtVendorGroupName_TextChanged);
            this.txtVendorGroupName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtVendorGroupName_KeyDown);
            // 
            // txtVendorGroupDescription
            // 
            this.txtVendorGroupDescription.Location = new System.Drawing.Point(96, 85);
            this.txtVendorGroupDescription.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtVendorGroupDescription.MinimumSize = new System.Drawing.Size(250, 20);
            this.txtVendorGroupDescription.Multiline = true;
            this.txtVendorGroupDescription.Name = "txtVendorGroupDescription";
            this.txtVendorGroupDescription.Size = new System.Drawing.Size(250, 20);
            this.txtVendorGroupDescription.TabIndex = 5;
            this.txtVendorGroupDescription.TabStop = false;
            this.txtVendorGroupDescription.TextChanged += new System.EventHandler(this.txtVendorGroupDescription_TextChanged);
            this.txtVendorGroupDescription.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtVendorGroupDescription_KeyDown);
            // 
            // txtVendorGroupID
            // 
            this.txtVendorGroupID.Location = new System.Drawing.Point(96, 9);
            this.txtVendorGroupID.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtVendorGroupID.MinimumSize = new System.Drawing.Size(125, 20);
            this.txtVendorGroupID.Name = "txtVendorGroupID";
            this.txtVendorGroupID.ReadOnly = true;
            this.txtVendorGroupID.Size = new System.Drawing.Size(125, 20);
            this.txtVendorGroupID.TabIndex = 0;
            this.txtVendorGroupID.TabStop = false;
            this.txtVendorGroupID.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtVendorGroupID_KeyDown);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 39);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Name";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 88);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Description";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Group ID";
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::VATClient.Properties.Resources.Green_Footer;
            this.panel1.Controls.Add(this.btnPrint);
            this.panel1.Controls.Add(this.btnClose);
            this.panel1.Controls.Add(this.btnUpdate);
            this.panel1.Controls.Add(this.btnAdd);
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Controls.Add(this.btnDelete);
            this.panel1.Location = new System.Drawing.Point(2, 201);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(430, 40);
            this.panel1.TabIndex = 8;
            // 
            // btnPrint
            // 
            this.btnPrint.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnPrint.Image = global::VATClient.Properties.Resources.Print;
            this.btnPrint.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPrint.Location = new System.Drawing.Point(264, 7);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(75, 28);
            this.btnPrint.TabIndex = 4;
            this.btnPrint.Text = "&Print";
            this.btnPrint.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnPrint.UseVisualStyleBackColor = false;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Image = global::VATClient.Properties.Resources.Back;
            this.btnClose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnClose.Location = new System.Drawing.Point(346, 7);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 28);
            this.btnClose.TabIndex = 12;
            this.btnClose.Text = "Clo&se";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnUpdate
            // 
            this.btnUpdate.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnUpdate.Image = global::VATClient.Properties.Resources.Update;
            this.btnUpdate.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnUpdate.Location = new System.Drawing.Point(100, 7);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(75, 28);
            this.btnUpdate.TabIndex = 10;
            this.btnUpdate.Text = "&Update";
            this.btnUpdate.UseVisualStyleBackColor = false;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnAdd.Image = global::VATClient.Properties.Resources.Save;
            this.btnAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAdd.Location = new System.Drawing.Point(19, 6);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 28);
            this.btnAdd.TabIndex = 9;
            this.btnAdd.Text = "&Add";
            this.btnAdd.UseVisualStyleBackColor = false;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnCancel.Image = global::VATClient.Properties.Resources.Referesh;
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.Location = new System.Drawing.Point(247, 49);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 28);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "&Refresh";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnDelete.Image = global::VATClient.Properties.Resources.Delete;
            this.btnDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDelete.Location = new System.Drawing.Point(183, 51);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 28);
            this.btnDelete.TabIndex = 11;
            this.btnDelete.Text = "&Delete";
            this.btnDelete.UseVisualStyleBackColor = false;
            this.btnDelete.Visible = false;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // backgroundWorkerAdd
            // 
            this.backgroundWorkerAdd.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerAdd_DoWork);
            this.backgroundWorkerAdd.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerAdd_RunWorkerCompleted);
            // 
            // backgroundWorkerDelete
            // 
            this.backgroundWorkerDelete.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerDelete_DoWork);
            this.backgroundWorkerDelete.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerDelete_RunWorkerCompleted);
            // 
            // backgroundWorkerUpdate
            // 
            this.backgroundWorkerUpdate.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerUpdate_DoWork);
            this.backgroundWorkerUpdate.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerUpdate_RunWorkerCompleted);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.Red;
            this.label6.Location = new System.Drawing.Point(81, 67);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(14, 14);
            this.label6.TabIndex = 260;
            this.label6.Text = "*";
            // 
            // FormVendorGroup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(435, 242);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.groupBox1);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(500, 290);
            this.MinimizeBox = false;
            this.Name = "FormVendorGroup";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Vendor Group";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormVendorGroup_FormClosing);
            this.Load += new System.EventHandler(this.FormVendorGroup_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtComments;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox chkActiveStatus;
        private System.Windows.Forms.TextBox txtVendorGroupName;
        private System.Windows.Forms.TextBox txtVendorGroupDescription;
        private System.Windows.Forms.TextBox txtVendorGroupID;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Button btnAddNew;
        private System.Windows.Forms.ComboBox cmbType;
        private System.Windows.Forms.Label label9;
        private System.ComponentModel.BackgroundWorker backgroundWorkerAdd;
        private System.ComponentModel.BackgroundWorker backgroundWorkerDelete;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.ComponentModel.BackgroundWorker backgroundWorkerUpdate;
        public System.Windows.Forms.Button btnUpdate;
        public System.Windows.Forms.Button btnAdd;
        public System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Label label33;
        private System.Windows.Forms.Label label6;
    }
}