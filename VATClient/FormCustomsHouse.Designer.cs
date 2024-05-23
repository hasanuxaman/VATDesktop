namespace VATClient
{
    partial class FormCustomsHouse
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
            this.txtCustomsHAddress = new System.Windows.Forms.TextBox();
            this.txtCustomsHCode = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.txtCustomsHName = new System.Windows.Forms.TextBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.btnAddNew = new System.Windows.Forms.Button();
            this.btnSearch = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.chkActiveStatus = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtCustomsHID = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.bgwAdd = new System.ComponentModel.BackgroundWorker();
            this.bgrDelete = new System.ComponentModel.BackgroundWorker();
            this.bgwUpdate = new System.ComponentModel.BackgroundWorker();
            this.label31 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label31);
            this.groupBox1.Controls.Add(this.txtCustomsHAddress);
            this.groupBox1.Controls.Add(this.txtCustomsHCode);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.txtCustomsHName);
            this.groupBox1.Controls.Add(this.progressBar1);
            this.groupBox1.Controls.Add(this.btnAddNew);
            this.groupBox1.Controls.Add(this.btnSearch);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.chkActiveStatus);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.txtCustomsHID);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(381, 163);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // txtCustomsHAddress
            // 
            this.txtCustomsHAddress.Location = new System.Drawing.Point(106, 96);
            this.txtCustomsHAddress.Multiline = true;
            this.txtCustomsHAddress.Name = "txtCustomsHAddress";
            this.txtCustomsHAddress.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtCustomsHAddress.Size = new System.Drawing.Size(250, 36);
            this.txtCustomsHAddress.TabIndex = 213;
            this.txtCustomsHAddress.TabStop = false;
            // 
            // txtCustomsHCode
            // 
            this.txtCustomsHCode.Location = new System.Drawing.Point(106, 49);
            this.txtCustomsHCode.Name = "txtCustomsHCode";
            this.txtCustomsHCode.Size = new System.Drawing.Size(125, 21);
            this.txtCustomsHCode.TabIndex = 212;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(8, 53);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(35, 13);
            this.label8.TabIndex = 211;
            this.label8.Text = " Code";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(8, 75);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(85, 13);
            this.label7.TabIndex = 210;
            this.label7.Text = "CustomsH Name";
            // 
            // txtCustomsHName
            // 
            this.txtCustomsHName.Location = new System.Drawing.Point(106, 73);
            this.txtCustomsHName.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtCustomsHName.MinimumSize = new System.Drawing.Size(250, 20);
            this.txtCustomsHName.Name = "txtCustomsHName";
            this.txtCustomsHName.Size = new System.Drawing.Size(250, 20);
            this.txtCustomsHName.TabIndex = 209;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(106, 96);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(245, 24);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 208;
            this.progressBar1.Visible = false;
            // 
            // btnAddNew
            // 
            this.btnAddNew.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnAddNew.Image = global::VATClient.Properties.Resources.Add_New;
            this.btnAddNew.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnAddNew.Location = new System.Drawing.Point(271, 24);
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
            this.btnSearch.Location = new System.Drawing.Point(235, 24);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(30, 20);
            this.btnSearch.TabIndex = 1;
            this.btnSearch.TabStop = false;
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(8, 106);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(97, 13);
            this.label6.TabIndex = 17;
            this.label6.Text = "CustomsH Address";
            // 
            // chkActiveStatus
            // 
            this.chkActiveStatus.AutoSize = true;
            this.chkActiveStatus.Checked = true;
            this.chkActiveStatus.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkActiveStatus.Location = new System.Drawing.Point(94, 138);
            this.chkActiveStatus.Name = "chkActiveStatus";
            this.chkActiveStatus.Size = new System.Drawing.Size(15, 14);
            this.chkActiveStatus.TabIndex = 7;
            this.chkActiveStatus.TabStop = false;
            this.chkActiveStatus.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(11, 137);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(71, 13);
            this.label4.TabIndex = 14;
            this.label4.Text = "Active Status";
            // 
            // txtCustomsHID
            // 
            this.txtCustomsHID.Location = new System.Drawing.Point(106, 24);
            this.txtCustomsHID.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtCustomsHID.MinimumSize = new System.Drawing.Size(125, 20);
            this.txtCustomsHID.Name = "txtCustomsHID";
            this.txtCustomsHID.ReadOnly = true;
            this.txtCustomsHID.Size = new System.Drawing.Size(125, 20);
            this.txtCustomsHID.TabIndex = 0;
            this.txtCustomsHID.TabStop = false;
            this.txtCustomsHID.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtVehicleID_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(21, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = " ID";
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::VATClient.Properties.Resources.Green_Footer;
            this.panel1.Controls.Add(this.btnUpdate);
            this.panel1.Controls.Add(this.btnAdd);
            this.panel1.Controls.Add(this.btnDelete);
            this.panel1.Location = new System.Drawing.Point(3, 172);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(379, 40);
            this.panel1.TabIndex = 8;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // btnUpdate
            // 
            this.btnUpdate.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnUpdate.Image = global::VATClient.Properties.Resources.Update;
            this.btnUpdate.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnUpdate.Location = new System.Drawing.Point(169, 6);
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
            this.btnAdd.Location = new System.Drawing.Point(63, 6);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 28);
            this.btnAdd.TabIndex = 9;
            this.btnAdd.Text = "&Add";
            this.btnAdd.UseVisualStyleBackColor = false;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnDelete.Image = global::VATClient.Properties.Resources.Delete;
            this.btnDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDelete.Location = new System.Drawing.Point(271, 54);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 28);
            this.btnDelete.TabIndex = 11;
            this.btnDelete.Text = "&Delete";
            this.btnDelete.UseVisualStyleBackColor = false;
            this.btnDelete.Visible = false;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // bgwAdd
            // 
            this.bgwAdd.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwAdd_DoWork);
            this.bgwAdd.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwAdd_RunWorkerCompleted);
            // 
            // bgrDelete
            // 
            this.bgrDelete.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgrDelete_DoWork);
            this.bgrDelete.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgrDelete_RunWorkerCompleted);
            // 
            // bgwUpdate
            // 
            this.bgwUpdate.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwUpdate_DoWork);
            this.bgwUpdate.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwUpdate_RunWorkerCompleted);
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label31.ForeColor = System.Drawing.Color.Red;
            this.label31.Location = new System.Drawing.Point(92, 55);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(14, 14);
            this.label31.TabIndex = 192;
            this.label31.Text = "*";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Red;
            this.label2.Location = new System.Drawing.Point(92, 79);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(14, 14);
            this.label2.TabIndex = 214;
            this.label2.Text = "*";
            // 
            // FormCustomsHouse
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(384, 214);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(500, 350);
            this.MinimumSize = new System.Drawing.Size(400, 250);
            this.Name = "FormCustomsHouse";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Customs House";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormVehicle_FormClosing);
            this.Load += new System.EventHandler(this.FormVehicle_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox chkActiveStatus;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtCustomsHID;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Button btnAddNew;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.ComponentModel.BackgroundWorker bgwAdd;
        private System.ComponentModel.BackgroundWorker bgrDelete;
        private System.ComponentModel.BackgroundWorker bgwUpdate;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtCustomsHName;
        private System.Windows.Forms.TextBox txtCustomsHCode;
        private System.Windows.Forms.Label label8;
        public System.Windows.Forms.Button btnAdd;
        public System.Windows.Forms.Button btnDelete;
        public System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.TextBox txtCustomsHAddress;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label31;
    }
}