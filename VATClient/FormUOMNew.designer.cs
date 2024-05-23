namespace VATClient
{
    partial class FormUOMNew
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
            this.UOMInsert = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.txtUOMFrom = new System.Windows.Forms.TextBox();
            this.txtUOMTo = new System.Windows.Forms.TextBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.label6 = new System.Windows.Forms.Label();
            this.chkActiveStatus = new System.Windows.Forms.CheckBox();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnSearch = new System.Windows.Forms.Button();
            this.txtConvertion = new System.Windows.Forms.TextBox();
            this.txtUOMId = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtCTypes = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.bgwSave = new System.ComponentModel.BackgroundWorker();
            this.backgroundWorkerUpdate = new System.ComponentModel.BackgroundWorker();
            this.backgroundWorkerDelete = new System.ComponentModel.BackgroundWorker();
            this.bgwLoad = new System.ComponentModel.BackgroundWorker();
            this.label8 = new System.Windows.Forms.Label();
            this.UOMInsert.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // UOMInsert
            // 
            this.UOMInsert.Controls.Add(this.label8);
            this.UOMInsert.Controls.Add(this.label7);
            this.UOMInsert.Controls.Add(this.label10);
            this.UOMInsert.Controls.Add(this.txtUOMFrom);
            this.UOMInsert.Controls.Add(this.txtUOMTo);
            this.UOMInsert.Controls.Add(this.progressBar1);
            this.UOMInsert.Controls.Add(this.label6);
            this.UOMInsert.Controls.Add(this.chkActiveStatus);
            this.UOMInsert.Controls.Add(this.btnRefresh);
            this.UOMInsert.Controls.Add(this.btnSearch);
            this.UOMInsert.Controls.Add(this.txtConvertion);
            this.UOMInsert.Controls.Add(this.txtUOMId);
            this.UOMInsert.Controls.Add(this.label4);
            this.UOMInsert.Controls.Add(this.label3);
            this.UOMInsert.Controls.Add(this.label1);
            this.UOMInsert.Controls.Add(this.label2);
            this.UOMInsert.Location = new System.Drawing.Point(0, 0);
            this.UOMInsert.Name = "UOMInsert";
            this.UOMInsert.Size = new System.Drawing.Size(366, 146);
            this.UOMInsert.TabIndex = 0;
            this.UOMInsert.TabStop = false;
            this.UOMInsert.Enter += new System.EventHandler(this.UOMInsert_Enter);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.Color.Red;
            this.label7.Location = new System.Drawing.Point(86, 68);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(14, 14);
            this.label7.TabIndex = 281;
            this.label7.Text = "*";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.ForeColor = System.Drawing.Color.Red;
            this.label10.Location = new System.Drawing.Point(86, 43);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(14, 14);
            this.label10.TabIndex = 280;
            this.label10.Text = "*";
            // 
            // txtUOMFrom
            // 
            this.txtUOMFrom.Location = new System.Drawing.Point(103, 37);
            this.txtUOMFrom.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtUOMFrom.MinimumSize = new System.Drawing.Size(125, 20);
            this.txtUOMFrom.Name = "txtUOMFrom";
            this.txtUOMFrom.ReadOnly = true;
            this.txtUOMFrom.Size = new System.Drawing.Size(125, 20);
            this.txtUOMFrom.TabIndex = 212;
            this.txtUOMFrom.TabStop = false;
            this.txtUOMFrom.DoubleClick += new System.EventHandler(this.txtFrom_DoubleClick);
            this.txtUOMFrom.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtFrom_KeyDown);
            // 
            // txtUOMTo
            // 
            this.txtUOMTo.Location = new System.Drawing.Point(103, 61);
            this.txtUOMTo.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtUOMTo.MinimumSize = new System.Drawing.Size(125, 20);
            this.txtUOMTo.Name = "txtUOMTo";
            this.txtUOMTo.ReadOnly = true;
            this.txtUOMTo.Size = new System.Drawing.Size(125, 20);
            this.txtUOMTo.TabIndex = 211;
            this.txtUOMTo.TabStop = false;
            this.txtUOMTo.DoubleClick += new System.EventHandler(this.txtUOMTo_DoubleClick);
            this.txtUOMTo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtUOMTo_KeyDown_1);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(63, 129);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(290, 16);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 206;
            this.progressBar1.Visible = false;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(11, 116);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(70, 13);
            this.label6.TabIndex = 190;
            this.label6.Text = "Active Status";
            // 
            // chkActiveStatus
            // 
            this.chkActiveStatus.AutoSize = true;
            this.chkActiveStatus.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkActiveStatus.Checked = true;
            this.chkActiveStatus.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkActiveStatus.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.chkActiveStatus.Location = new System.Drawing.Point(103, 116);
            this.chkActiveStatus.Name = "chkActiveStatus";
            this.chkActiveStatus.Size = new System.Drawing.Size(15, 14);
            this.chkActiveStatus.TabIndex = 6;
            this.chkActiveStatus.TabStop = false;
            this.chkActiveStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkActiveStatus.UseVisualStyleBackColor = true;
            // 
            // btnRefresh
            // 
            this.btnRefresh.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnRefresh.Image = global::VATClient.Properties.Resources.Add_New;
            this.btnRefresh.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnRefresh.Location = new System.Drawing.Point(270, 14);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(30, 20);
            this.btnRefresh.TabIndex = 2;
            this.btnRefresh.TabStop = false;
            this.btnRefresh.UseVisualStyleBackColor = false;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSearch.Image = global::VATClient.Properties.Resources.search;
            this.btnSearch.Location = new System.Drawing.Point(234, 14);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(30, 20);
            this.btnSearch.TabIndex = 1;
            this.btnSearch.TabStop = false;
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // txtConvertion
            // 
            this.txtConvertion.Location = new System.Drawing.Point(103, 90);
            this.txtConvertion.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtConvertion.MinimumSize = new System.Drawing.Size(250, 20);
            this.txtConvertion.Multiline = true;
            this.txtConvertion.Name = "txtConvertion";
            this.txtConvertion.Size = new System.Drawing.Size(250, 20);
            this.txtConvertion.TabIndex = 5;
            this.txtConvertion.Text = "0";
            this.txtConvertion.TextChanged += new System.EventHandler(this.txtConvertion_TextChanged);
            this.txtConvertion.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtConvertion_KeyDown);
            this.txtConvertion.Leave += new System.EventHandler(this.txtConvertion_Leave);
            // 
            // txtUOMId
            // 
            this.txtUOMId.Location = new System.Drawing.Point(103, 14);
            this.txtUOMId.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtUOMId.MinimumSize = new System.Drawing.Size(125, 20);
            this.txtUOMId.Name = "txtUOMId";
            this.txtUOMId.ReadOnly = true;
            this.txtUOMId.Size = new System.Drawing.Size(125, 20);
            this.txtUOMId.TabIndex = 0;
            this.txtUOMId.TabStop = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 94);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(60, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "Conversion";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 68);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "To (F9)";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "From (F9)";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 18);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(18, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "ID";
            // 
            // txtCTypes
            // 
            this.txtCTypes.Location = new System.Drawing.Point(100, 200);
            this.txtCTypes.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtCTypes.MinimumSize = new System.Drawing.Size(250, 20);
            this.txtCTypes.Multiline = true;
            this.txtCTypes.Name = "txtCTypes";
            this.txtCTypes.Size = new System.Drawing.Size(250, 20);
            this.txtCTypes.TabIndex = 186;
            this.txtCTypes.Text = "-";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(0, 196);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(43, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "CTypes";
            this.label5.Visible = false;
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::VATClient.Properties.Resources.Green_Footer;
            this.panel1.Controls.Add(this.btnDelete);
            this.panel1.Controls.Add(this.btnUpdate);
            this.panel1.Controls.Add(this.btnClose);
            this.panel1.Controls.Add(this.btnSave);
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Location = new System.Drawing.Point(0, 148);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(366, 36);
            this.panel1.TabIndex = 7;
            // 
            // btnDelete
            // 
            this.btnDelete.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnDelete.Image = global::VATClient.Properties.Resources.Delete;
            this.btnDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDelete.Location = new System.Drawing.Point(189, 51);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 28);
            this.btnDelete.TabIndex = 10;
            this.btnDelete.Text = "&Delete";
            this.btnDelete.UseVisualStyleBackColor = false;
            this.btnDelete.Visible = false;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnUpdate
            // 
            this.btnUpdate.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnUpdate.Image = global::VATClient.Properties.Resources.Update;
            this.btnUpdate.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnUpdate.Location = new System.Drawing.Point(95, 5);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(75, 28);
            this.btnUpdate.TabIndex = 9;
            this.btnUpdate.Text = "&Update";
            this.btnUpdate.UseVisualStyleBackColor = false;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Image = global::VATClient.Properties.Resources.Back;
            this.btnClose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnClose.Location = new System.Drawing.Point(276, 5);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(82, 28);
            this.btnClose.TabIndex = 11;
            this.btnClose.Text = "&Close";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSave.Image = global::VATClient.Properties.Resources.Save;
            this.btnSave.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSave.Location = new System.Drawing.Point(3, 5);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(82, 28);
            this.btnSave.TabIndex = 8;
            this.btnSave.Text = "&Add";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnCancel.Image = global::VATClient.Properties.Resources.Referesh;
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.Location = new System.Drawing.Point(96, 59);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 28);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "&Refresh";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancel.UseVisualStyleBackColor = false;
            // 
            // bgwSave
            // 
            this.bgwSave.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwSave_DoWork);
            this.bgwSave.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwSave_RunWorkerCompleted);
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
            // bgwLoad
            // 
            this.bgwLoad.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwLoad_DoWork);
            this.bgwLoad.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwLoad_RunWorkerCompleted);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ForeColor = System.Drawing.Color.Red;
            this.label8.Location = new System.Drawing.Point(86, 95);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(14, 14);
            this.label8.TabIndex = 282;
            this.label8.Text = "*";
            // 
            // FormUOMNew
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(370, 185);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.UOMInsert);
            this.Controls.Add(this.txtCTypes);
            this.Controls.Add(this.label5);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormUOMNew";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Conversion";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormUOMNew_FormClosing);
            this.Load += new System.EventHandler(this.FormUOMNew_Load);
            this.UOMInsert.ResumeLayout(false);
            this.UOMInsert.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox UOMInsert;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnCancel;
        public System.Windows.Forms.Label label5;
        public System.Windows.Forms.Label label4;
        public System.Windows.Forms.Label label3;
        public System.Windows.Forms.Label label1;
        public System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.TextBox txtUOMId;
        private System.Windows.Forms.TextBox txtConvertion;
        private System.Windows.Forms.TextBox txtCTypes;
        private System.Windows.Forms.CheckBox chkActiveStatus;
        private System.Windows.Forms.Label label6;
        private System.ComponentModel.BackgroundWorker bgwSave;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.ComponentModel.BackgroundWorker backgroundWorkerUpdate;
        private System.ComponentModel.BackgroundWorker backgroundWorkerDelete;
        private System.ComponentModel.BackgroundWorker bgwLoad;
        public System.Windows.Forms.Button btnSave;
        public System.Windows.Forms.Button btnUpdate;
        public System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.TextBox txtUOMFrom;
        private System.Windows.Forms.TextBox txtUOMTo;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label8;
    }
}