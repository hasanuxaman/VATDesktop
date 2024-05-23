namespace VATClient
{
    partial class FormPackaging
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
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.chkActiveStatus = new System.Windows.Forms.CheckBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.cmbUom = new System.Windows.Forms.ComboBox();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtPacketName = new System.Windows.Forms.TextBox();
            this.lblCapacity = new System.Windows.Forms.Label();
            this.btnAddNew = new System.Windows.Forms.Button();
            this.btnSearch = new System.Windows.Forms.Button();
            this.txtPacketSize = new System.Windows.Forms.TextBox();
            this.txtPacketID = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.backgroundWorkerAdd = new System.ComponentModel.BackgroundWorker();
            this.backgroundWorkerUpdate = new System.ComponentModel.BackgroundWorker();
            this.backgroundWorkerDelete = new System.ComponentModel.BackgroundWorker();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.progressBar1);
            this.groupBox1.Controls.Add(this.chkActiveStatus);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.cmbUom);
            this.groupBox1.Controls.Add(this.txtDescription);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtPacketName);
            this.groupBox1.Controls.Add(this.lblCapacity);
            this.groupBox1.Controls.Add(this.btnAddNew);
            this.groupBox1.Controls.Add(this.btnSearch);
            this.groupBox1.Controls.Add(this.txtPacketSize);
            this.groupBox1.Controls.Add(this.txtPacketID);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(5, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(470, 214);
            this.groupBox1.TabIndex = 11;
            this.groupBox1.TabStop = false;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(133, 162);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(225, 24);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 189;
            this.progressBar1.Visible = false;
            // 
            // chkActiveStatus
            // 
            this.chkActiveStatus.AutoSize = true;
            this.chkActiveStatus.Checked = true;
            this.chkActiveStatus.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkActiveStatus.Location = new System.Drawing.Point(368, 83);
            this.chkActiveStatus.Name = "chkActiveStatus";
            this.chkActiveStatus.Size = new System.Drawing.Size(15, 14);
            this.chkActiveStatus.TabIndex = 7;
            this.chkActiveStatus.TabStop = false;
            this.chkActiveStatus.UseVisualStyleBackColor = true;
            this.chkActiveStatus.CheckedChanged += new System.EventHandler(this.chkActiveStatus_CheckedChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(292, 84);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(70, 13);
            this.label11.TabIndex = 9;
            this.label11.Text = "Active Status";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(14, 141);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(60, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Description";
            // 
            // cmbUom
            // 
            this.cmbUom.FormattingEnabled = true;
            this.cmbUom.Location = new System.Drawing.Point(133, 80);
            this.cmbUom.Name = "cmbUom";
            this.cmbUom.Size = new System.Drawing.Size(109, 21);
            this.cmbUom.Sorted = true;
            this.cmbUom.TabIndex = 3;
            this.cmbUom.SelectedIndexChanged += new System.EventHandler(this.cmbUom_SelectedIndexChanged);
            this.cmbUom.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmbUom_KeyDown);
            // 
            // txtDescription
            // 
            this.txtDescription.Location = new System.Drawing.Point(133, 104);
            this.txtDescription.MinimumSize = new System.Drawing.Size(290, 50);
            this.txtDescription.Multiline = true;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtDescription.Size = new System.Drawing.Size(331, 95);
            this.txtDescription.TabIndex = 4;
            this.txtDescription.TextChanged += new System.EventHandler(this.txtDescription_TextChanged);
            this.txtDescription.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtDescription_KeyDown);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 80);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(78, 13);
            this.label3.TabIndex = 188;
            this.label3.Text = "Package UOM";
            // 
            // txtPacketName
            // 
            this.txtPacketName.Location = new System.Drawing.Point(133, 35);
            this.txtPacketName.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtPacketName.MinimumSize = new System.Drawing.Size(250, 20);
            this.txtPacketName.Name = "txtPacketName";
            this.txtPacketName.Size = new System.Drawing.Size(250, 20);
            this.txtPacketName.TabIndex = 1;
            this.txtPacketName.TextChanged += new System.EventHandler(this.txtPacketName_TextChanged);
            this.txtPacketName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtPacketName_KeyDown);
            // 
            // lblCapacity
            // 
            this.lblCapacity.AutoSize = true;
            this.lblCapacity.Location = new System.Drawing.Point(14, 58);
            this.lblCapacity.Name = "lblCapacity";
            this.lblCapacity.Size = new System.Drawing.Size(94, 13);
            this.lblCapacity.TabIndex = 187;
            this.lblCapacity.Text = "Package Capacity";
            // 
            // btnAddNew
            // 
            this.btnAddNew.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnAddNew.Image = global::VATClient.Properties.Resources.Add_New;
            this.btnAddNew.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnAddNew.Location = new System.Drawing.Point(324, 13);
            this.btnAddNew.Name = "btnAddNew";
            this.btnAddNew.Size = new System.Drawing.Size(30, 20);
            this.btnAddNew.TabIndex = 6;
            this.btnAddNew.TabStop = false;
            this.btnAddNew.UseVisualStyleBackColor = false;
            this.btnAddNew.Click += new System.EventHandler(this.btnAddNew_Click);
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSearch.Image = global::VATClient.Properties.Resources.search;
            this.btnSearch.Location = new System.Drawing.Point(287, 12);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(30, 20);
            this.btnSearch.TabIndex = 5;
            this.btnSearch.TabStop = false;
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // txtPacketSize
            // 
            this.txtPacketSize.Location = new System.Drawing.Point(133, 58);
            this.txtPacketSize.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtPacketSize.MinimumSize = new System.Drawing.Size(250, 20);
            this.txtPacketSize.Name = "txtPacketSize";
            this.txtPacketSize.Size = new System.Drawing.Size(250, 20);
            this.txtPacketSize.TabIndex = 2;
            this.txtPacketSize.TextChanged += new System.EventHandler(this.txtPacketSize_TextChanged);
            this.txtPacketSize.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtPacketSize_KeyDown);
            // 
            // txtPacketID
            // 
            this.txtPacketID.Location = new System.Drawing.Point(133, 12);
            this.txtPacketID.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtPacketID.MinimumSize = new System.Drawing.Size(125, 20);
            this.txtPacketID.Name = "txtPacketID";
            this.txtPacketID.ReadOnly = true;
            this.txtPacketID.Size = new System.Drawing.Size(125, 20);
            this.txtPacketID.TabIndex = 0;
            this.txtPacketID.TabStop = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(14, 38);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(97, 13);
            this.label4.TabIndex = 77;
            this.label4.Text = "Nature of Package";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 13);
            this.label1.TabIndex = 71;
            this.label1.Text = "Package ID";
            // 
            // backgroundWorkerAdd
            // 
            this.backgroundWorkerAdd.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerAdd_DoWork);
            this.backgroundWorkerAdd.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerAdd_RunWorkerCompleted);
            // 
            // backgroundWorkerUpdate
            // 
            this.backgroundWorkerUpdate.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerUpdate_DoWork);
            this.backgroundWorkerUpdate.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerUpdate_RunWorkerCompleted);
            // 
            // backgroundWorkerDelete
            // 
            this.backgroundWorkerDelete.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerDelete_DoWork);
            this.backgroundWorkerDelete.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerDelete_RunWorkerCompleted);
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::VATClient.Properties.Resources.Green_Footer;
            this.panel1.Controls.Add(this.btnClose);
            this.panel1.Controls.Add(this.btnUpdate);
            this.panel1.Controls.Add(this.btnAdd);
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Controls.Add(this.btnDelete);
            this.panel1.Location = new System.Drawing.Point(3, 220);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(480, 40);
            this.panel1.TabIndex = 13;
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Image = global::VATClient.Properties.Resources.Back;
            this.btnClose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnClose.Location = new System.Drawing.Point(399, 6);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 28);
            this.btnClose.TabIndex = 3;
            this.btnClose.Text = "&Close";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnUpdate
            // 
            this.btnUpdate.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnUpdate.Image = global::VATClient.Properties.Resources.Update;
            this.btnUpdate.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnUpdate.Location = new System.Drawing.Point(107, 6);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(75, 28);
            this.btnUpdate.TabIndex = 1;
            this.btnUpdate.Text = "&Update";
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnAdd.Image = global::VATClient.Properties.Resources.Save;
            this.btnAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAdd.Location = new System.Drawing.Point(24, 6);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 28);
            this.btnAdd.TabIndex = 0;
            this.btnAdd.Text = "&Add";
            this.btnAdd.UseVisualStyleBackColor = false;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnCancel.Image = global::VATClient.Properties.Resources.Referesh;
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.Location = new System.Drawing.Point(249, 57);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 28);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "&Refresh";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancel.UseVisualStyleBackColor = false;
            // 
            // btnDelete
            // 
            this.btnDelete.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnDelete.Image = global::VATClient.Properties.Resources.Delete;
            this.btnDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDelete.Location = new System.Drawing.Point(188, 59);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 28);
            this.btnDelete.TabIndex = 2;
            this.btnDelete.Text = "&Delete";
            this.btnDelete.UseVisualStyleBackColor = false;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // FormPackaging
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(484, 262);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.groupBox1);
            this.MaximizeBox = false;
            this.Name = "FormPackaging";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Packaging";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormPackaging_FormClosing);
            this.Load += new System.EventHandler(this.FormPackaging_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblCapacity;
        private System.Windows.Forms.Button btnAddNew;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.TextBox txtPacketSize;
        private System.Windows.Forms.TextBox txtPacketID;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label1;
        private System.ComponentModel.BackgroundWorker backgroundWorkerAdd;
        private System.ComponentModel.BackgroundWorker backgroundWorkerUpdate;
        private System.ComponentModel.BackgroundWorker backgroundWorkerDelete;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.TextBox txtPacketName;
        private System.Windows.Forms.Label label3;
        public System.Windows.Forms.ComboBox cmbUom;
        private System.Windows.Forms.CheckBox chkActiveStatus;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ProgressBar progressBar1;
        public System.Windows.Forms.Button btnUpdate;
        public System.Windows.Forms.Button btnAdd;
        public System.Windows.Forms.Button btnDelete;
    }
}