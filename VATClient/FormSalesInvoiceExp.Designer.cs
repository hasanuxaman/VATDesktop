namespace VATClient
{
    partial class FormSalesInvoiceExp
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
            this.btnAddNew = new System.Windows.Forms.Button();
            this.btnSearch = new System.Windows.Forms.Button();
            this.Remarks = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.txtRemarks = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtLCNumber = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtPortTo = new System.Windows.Forms.TextBox();
            this.txtPortFrom = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtLCBank = new System.Windows.Forms.TextBox();
            this.dtpLCDate = new System.Windows.Forms.DateTimePicker();
            this.dtpEXPDate = new System.Windows.Forms.DateTimePicker();
            this.label4 = new System.Windows.Forms.Label();
            this.txtEXPNo = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.dtpPIDate = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtPINo = new System.Windows.Forms.TextBox();
            this.txtId = new System.Windows.Forms.TextBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.bgwSave = new System.ComponentModel.BackgroundWorker();
            this.bgwUpdate = new System.ComponentModel.BackgroundWorker();
            this.bgwDelete = new System.ComponentModel.BackgroundWorker();
            this.label57 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnAddNew
            // 
            this.btnAddNew.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnAddNew.Image = global::VATClient.Properties.Resources.Add_New;
            this.btnAddNew.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnAddNew.Location = new System.Drawing.Point(286, 23);
            this.btnAddNew.Name = "btnAddNew";
            this.btnAddNew.Size = new System.Drawing.Size(30, 20);
            this.btnAddNew.TabIndex = 4;
            this.btnAddNew.TabStop = false;
            this.btnAddNew.UseVisualStyleBackColor = false;
            this.btnAddNew.Click += new System.EventHandler(this.btnAddNew_Click);
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSearch.Image = global::VATClient.Properties.Resources.search;
            this.btnSearch.Location = new System.Drawing.Point(250, 23);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(30, 20);
            this.btnSearch.TabIndex = 3;
            this.btnSearch.TabStop = false;
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // Remarks
            // 
            this.Remarks.AutoSize = true;
            this.Remarks.Location = new System.Drawing.Point(9, 186);
            this.Remarks.Name = "Remarks";
            this.Remarks.Size = new System.Drawing.Size(48, 13);
            this.Remarks.TabIndex = 42;
            this.Remarks.Text = "Remarks";
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::VATClient.Properties.Resources.Green_Footer;
            this.panel1.Controls.Add(this.button2);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.btnClose);
            this.panel1.Controls.Add(this.btnUpdate);
            this.panel1.Controls.Add(this.btnAdd);
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Controls.Add(this.btnDelete);
            this.panel1.Location = new System.Drawing.Point(4, 231);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(448, 40);
            this.panel1.TabIndex = 213;
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.Image = global::VATClient.Properties.Resources.Back;
            this.button2.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button2.Location = new System.Drawing.Point(358, 5);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 28);
            this.button2.TabIndex = 248;
            this.button2.Text = "&Close";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button1.Image = global::VATClient.Properties.Resources.Back;
            this.button1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button1.Location = new System.Drawing.Point(606, 5);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 28);
            this.button1.TabIndex = 42;
            this.button1.Text = "&Close";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Image = global::VATClient.Properties.Resources.Back;
            this.btnClose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnClose.Location = new System.Drawing.Point(689, 42);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 28);
            this.btnClose.TabIndex = 26;
            this.btnClose.Text = "&Close";
            this.btnClose.UseVisualStyleBackColor = false;
            // 
            // btnUpdate
            // 
            this.btnUpdate.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnUpdate.Image = global::VATClient.Properties.Resources.Update;
            this.btnUpdate.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnUpdate.Location = new System.Drawing.Point(130, 5);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(75, 28);
            this.btnUpdate.TabIndex = 23;
            this.btnUpdate.Text = "&Update";
            this.btnUpdate.UseVisualStyleBackColor = false;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnAdd.Image = global::VATClient.Properties.Resources.Save;
            this.btnAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAdd.Location = new System.Drawing.Point(18, 5);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 28);
            this.btnAdd.TabIndex = 22;
            this.btnAdd.Text = "&Add";
            this.btnAdd.UseVisualStyleBackColor = false;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnCancel.Image = global::VATClient.Properties.Resources.Referesh;
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.Location = new System.Drawing.Point(282, 53);
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
            this.btnDelete.Location = new System.Drawing.Point(240, 5);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 28);
            this.btnDelete.TabIndex = 24;
            this.btnDelete.Text = "&Delete";
            this.btnDelete.UseVisualStyleBackColor = false;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // txtRemarks
            // 
            this.txtRemarks.BackColor = System.Drawing.SystemColors.Window;
            this.txtRemarks.Location = new System.Drawing.Point(68, 174);
            this.txtRemarks.MaximumSize = new System.Drawing.Size(400, 50);
            this.txtRemarks.MinimumSize = new System.Drawing.Size(100, 30);
            this.txtRemarks.Multiline = true;
            this.txtRemarks.Name = "txtRemarks";
            this.txtRemarks.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtRemarks.Size = new System.Drawing.Size(358, 37);
            this.txtRemarks.TabIndex = 214;
            this.txtRemarks.TabStop = false;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(39, 27);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(18, 13);
            this.label11.TabIndex = 25;
            this.label11.Text = "ID";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(241, 75);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(45, 13);
            this.label12.TabIndex = 26;
            this.label12.Text = "LC Date";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(15, 77);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(45, 13);
            this.label13.TabIndex = 28;
            this.label13.Text = "LC Bank";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label57);
            this.groupBox1.Controls.Add(this.txtLCNumber);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.txtPortTo);
            this.groupBox1.Controls.Add(this.txtPortFrom);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.txtLCBank);
            this.groupBox1.Controls.Add(this.dtpLCDate);
            this.groupBox1.Controls.Add(this.dtpEXPDate);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.txtEXPNo);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.dtpPIDate);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.txtPINo);
            this.groupBox1.Controls.Add(this.txtId);
            this.groupBox1.Controls.Add(this.progressBar1);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.txtRemarks);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.Remarks);
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Controls.Add(this.btnSearch);
            this.groupBox1.Controls.Add(this.btnAddNew);
            this.groupBox1.Location = new System.Drawing.Point(5, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(447, 219);
            this.groupBox1.TabIndex = 215;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Basic";
            // 
            // txtLCNumber
            // 
            this.txtLCNumber.Location = new System.Drawing.Point(68, 49);
            this.txtLCNumber.Name = "txtLCNumber";
            this.txtLCNumber.Size = new System.Drawing.Size(165, 21);
            this.txtLCNumber.TabIndex = 254;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(22, 55);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(35, 13);
            this.label7.TabIndex = 253;
            this.label7.Text = "LC No";
            // 
            // txtPortTo
            // 
            this.txtPortTo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPortTo.BackColor = System.Drawing.SystemColors.Window;
            this.txtPortTo.Location = new System.Drawing.Point(291, 147);
            this.txtPortTo.MaximumSize = new System.Drawing.Size(175, 20);
            this.txtPortTo.MinimumSize = new System.Drawing.Size(150, 20);
            this.txtPortTo.Multiline = true;
            this.txtPortTo.Name = "txtPortTo";
            this.txtPortTo.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtPortTo.Size = new System.Drawing.Size(154, 20);
            this.txtPortTo.TabIndex = 252;
            this.txtPortTo.TabStop = false;
            // 
            // txtPortFrom
            // 
            this.txtPortFrom.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPortFrom.BackColor = System.Drawing.SystemColors.Window;
            this.txtPortFrom.Location = new System.Drawing.Point(68, 148);
            this.txtPortFrom.MaximumSize = new System.Drawing.Size(175, 20);
            this.txtPortFrom.MinimumSize = new System.Drawing.Size(150, 20);
            this.txtPortFrom.Multiline = true;
            this.txtPortFrom.Name = "txtPortFrom";
            this.txtPortFrom.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtPortFrom.Size = new System.Drawing.Size(165, 20);
            this.txtPortFrom.TabIndex = 251;
            this.txtPortFrom.TabStop = false;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(241, 151);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(39, 13);
            this.label6.TabIndex = 250;
            this.label6.Text = "PortTo";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 152);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(54, 13);
            this.label5.TabIndex = 249;
            this.label5.Text = "Port From";
            // 
            // txtLCBank
            // 
            this.txtLCBank.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLCBank.BackColor = System.Drawing.SystemColors.Window;
            this.txtLCBank.Location = new System.Drawing.Point(68, 74);
            this.txtLCBank.MaximumSize = new System.Drawing.Size(175, 20);
            this.txtLCBank.MinimumSize = new System.Drawing.Size(150, 20);
            this.txtLCBank.Multiline = true;
            this.txtLCBank.Name = "txtLCBank";
            this.txtLCBank.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLCBank.Size = new System.Drawing.Size(165, 20);
            this.txtLCBank.TabIndex = 248;
            this.txtLCBank.TabStop = false;
            // 
            // dtpLCDate
            // 
            this.dtpLCDate.CustomFormat = "dd/MMM/yyyy ";
            this.dtpLCDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpLCDate.Location = new System.Drawing.Point(292, 72);
            this.dtpLCDate.MaximumSize = new System.Drawing.Size(175, 30);
            this.dtpLCDate.MinimumSize = new System.Drawing.Size(135, 20);
            this.dtpLCDate.Name = "dtpLCDate";
            this.dtpLCDate.Size = new System.Drawing.Size(135, 21);
            this.dtpLCDate.TabIndex = 247;
            // 
            // dtpEXPDate
            // 
            this.dtpEXPDate.CustomFormat = "dd/MMM/yyyy ";
            this.dtpEXPDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpEXPDate.Location = new System.Drawing.Point(292, 121);
            this.dtpEXPDate.MaximumSize = new System.Drawing.Size(175, 30);
            this.dtpEXPDate.MinimumSize = new System.Drawing.Size(135, 20);
            this.dtpEXPDate.Name = "dtpEXPDate";
            this.dtpEXPDate.Size = new System.Drawing.Size(136, 21);
            this.dtpEXPDate.TabIndex = 246;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(241, 127);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(51, 13);
            this.label4.TabIndex = 245;
            this.label4.Text = "EXP Date";
            // 
            // txtEXPNo
            // 
            this.txtEXPNo.Location = new System.Drawing.Point(68, 123);
            this.txtEXPNo.Name = "txtEXPNo";
            this.txtEXPNo.Size = new System.Drawing.Size(165, 21);
            this.txtEXPNo.TabIndex = 244;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(19, 129);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 13);
            this.label3.TabIndex = 243;
            this.label3.Text = "EXP No";
            // 
            // dtpPIDate
            // 
            this.dtpPIDate.CustomFormat = "dd/MMM/yyyy ";
            this.dtpPIDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpPIDate.Location = new System.Drawing.Point(291, 96);
            this.dtpPIDate.MaximumSize = new System.Drawing.Size(175, 30);
            this.dtpPIDate.MinimumSize = new System.Drawing.Size(135, 20);
            this.dtpPIDate.Name = "dtpPIDate";
            this.dtpPIDate.Size = new System.Drawing.Size(136, 21);
            this.dtpPIDate.TabIndex = 242;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(241, 101);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 13);
            this.label2.TabIndex = 224;
            this.label2.Text = "PI Date";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(27, 102);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(33, 13);
            this.label1.TabIndex = 223;
            this.label1.Text = "PI No";
            // 
            // txtPINo
            // 
            this.txtPINo.Location = new System.Drawing.Point(68, 98);
            this.txtPINo.Name = "txtPINo";
            this.txtPINo.Size = new System.Drawing.Size(165, 21);
            this.txtPINo.TabIndex = 220;
            // 
            // txtId
            // 
            this.txtId.Location = new System.Drawing.Point(68, 22);
            this.txtId.Name = "txtId";
            this.txtId.ReadOnly = true;
            this.txtId.Size = new System.Drawing.Size(173, 21);
            this.txtId.TabIndex = 218;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(70, 173);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(288, 24);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 216;
            this.progressBar1.Visible = false;
            // 
            // bgwSave
            // 
            this.bgwSave.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwSave_DoWork);
            this.bgwSave.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwSave_RunWorkerCompleted);
            // 
            // bgwUpdate
            // 
            this.bgwUpdate.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwUpdate_DoWork);
            this.bgwUpdate.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwUpdate_RunWorkerCompleted);
            // 
            // bgwDelete
            // 
            this.bgwDelete.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwDelete_DoWork);
            this.bgwDelete.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwDelete_RunWorkerCompleted);
            // 
            // label57
            // 
            this.label57.Font = new System.Drawing.Font("Tahoma", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label57.ForeColor = System.Drawing.Color.Red;
            this.label57.Location = new System.Drawing.Point(59, 103);
            this.label57.MaximumSize = new System.Drawing.Size(10, 10);
            this.label57.Name = "label57";
            this.label57.Size = new System.Drawing.Size(9, 10);
            this.label57.TabIndex = 285;
            this.label57.Text = "*";
            this.label57.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // FormSalesInvoiceExp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(453, 274);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(490, 420);
            this.MinimumSize = new System.Drawing.Size(300, 300);
            this.Name = "FormSalesInvoiceExp";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SalesInvoiceExp";
            this.Load += new System.EventHandler(this.FormHsCode_Load);
            this.panel1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnAddNew;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Label Remarks;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TextBox txtRemarks;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.ComponentModel.BackgroundWorker bgwSave;
        private System.ComponentModel.BackgroundWorker bgwUpdate;
        private System.ComponentModel.BackgroundWorker bgwDelete;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtPINo;
        private System.Windows.Forms.TextBox txtId;
        private System.Windows.Forms.DateTimePicker dtpEXPDate;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtEXPNo;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DateTimePicker dtpPIDate;
        private System.Windows.Forms.DateTimePicker dtpLCDate;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox txtLCBank;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtPortTo;
        private System.Windows.Forms.TextBox txtPortFrom;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtLCNumber;
        private System.Windows.Forms.Label label7;
        public System.Windows.Forms.Button btnUpdate;
        public System.Windows.Forms.Button btnAdd;
        public System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Label label57;
    }
}