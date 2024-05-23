namespace VATClient
{
    partial class FormShifts
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
            this.label4 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.chkNextDay = new System.Windows.Forms.CheckBox();
            this.dtpShiftEnd = new System.Windows.Forms.DateTimePicker();
            this.dtpShiftStart = new System.Windows.Forms.DateTimePicker();
            this.label7 = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.btnAddNew = new System.Windows.Forms.Button();
            this.btnSearch = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtId = new System.Windows.Forms.TextBox();
            this.txtShiftName = new System.Windows.Forms.TextBox();
            this.txtSL = new System.Windows.Forms.TextBox();
            this.txtRemarks = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.bgwAdd = new System.ComponentModel.BackgroundWorker();
            this.bgrDelete = new System.ComponentModel.BackgroundWorker();
            this.bgwUpdate = new System.ComponentModel.BackgroundWorker();
            this.btnClose = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.btnPrint = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.chkNextDay);
            this.groupBox1.Controls.Add(this.dtpShiftEnd);
            this.groupBox1.Controls.Add(this.dtpShiftStart);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.progressBar1);
            this.groupBox1.Controls.Add(this.btnAddNew);
            this.groupBox1.Controls.Add(this.btnSearch);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtId);
            this.groupBox1.Controls.Add(this.txtShiftName);
            this.groupBox1.Controls.Add(this.txtSL);
            this.groupBox1.Controls.Add(this.txtRemarks);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(13, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(459, 204);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.Red;
            this.label4.Location = new System.Drawing.Point(95, 77);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(14, 14);
            this.label4.TabIndex = 278;
            this.label4.Text = "*";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.ForeColor = System.Drawing.Color.Red;
            this.label10.Location = new System.Drawing.Point(95, 53);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(14, 14);
            this.label10.TabIndex = 277;
            this.label10.Text = "*";
            // 
            // chkNextDay
            // 
            this.chkNextDay.AutoSize = true;
            this.chkNextDay.Location = new System.Drawing.Point(237, 124);
            this.chkNextDay.Name = "chkNextDay";
            this.chkNextDay.Size = new System.Drawing.Size(71, 17);
            this.chkNextDay.TabIndex = 213;
            this.chkNextDay.Text = "Next Day";
            this.chkNextDay.UseVisualStyleBackColor = true;
            // 
            // dtpShiftEnd
            // 
            this.dtpShiftEnd.CustomFormat = "HH:mm tt";
            this.dtpShiftEnd.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpShiftEnd.Location = new System.Drawing.Point(111, 122);
            this.dtpShiftEnd.MaxDate = new System.DateTime(9900, 1, 1, 0, 0, 0, 0);
            this.dtpShiftEnd.MaximumSize = new System.Drawing.Size(4, 20);
            this.dtpShiftEnd.MinDate = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
            this.dtpShiftEnd.MinimumSize = new System.Drawing.Size(120, 20);
            this.dtpShiftEnd.Name = "dtpShiftEnd";
            this.dtpShiftEnd.ShowUpDown = true;
            this.dtpShiftEnd.Size = new System.Drawing.Size(120, 20);
            this.dtpShiftEnd.TabIndex = 212;
            this.dtpShiftEnd.Value = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
            // 
            // dtpShiftStart
            // 
            this.dtpShiftStart.CustomFormat = "HH:mm tt";
            this.dtpShiftStart.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpShiftStart.Location = new System.Drawing.Point(111, 99);
            this.dtpShiftStart.MaxDate = new System.DateTime(9900, 1, 1, 0, 0, 0, 0);
            this.dtpShiftStart.MaximumSize = new System.Drawing.Size(4, 20);
            this.dtpShiftStart.MinDate = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
            this.dtpShiftStart.MinimumSize = new System.Drawing.Size(120, 20);
            this.dtpShiftStart.Name = "dtpShiftStart";
            this.dtpShiftStart.ShowUpDown = true;
            this.dtpShiftStart.Size = new System.Drawing.Size(120, 20);
            this.dtpShiftStart.TabIndex = 211;
            this.dtpShiftStart.Value = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(28, 125);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(25, 13);
            this.label7.TabIndex = 210;
            this.label7.Text = "End";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(111, 148);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(271, 24);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 208;
            this.progressBar1.Visible = false;
            // 
            // btnAddNew
            // 
            this.btnAddNew.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnAddNew.Image = global::VATClient.Properties.Resources.Add_New;
            this.btnAddNew.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnAddNew.Location = new System.Drawing.Point(276, 24);
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
            this.btnSearch.Location = new System.Drawing.Point(240, 24);
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
            this.label6.Location = new System.Drawing.Point(28, 154);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(48, 13);
            this.label6.TabIndex = 17;
            this.label6.Text = "Remarks";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(28, 99);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(31, 13);
            this.label5.TabIndex = 16;
            this.label5.Text = "Start";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(28, 75);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(18, 13);
            this.label3.TabIndex = 13;
            this.label3.Text = "SL";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(28, 51);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "Name";
            // 
            // txtId
            // 
            this.txtId.Location = new System.Drawing.Point(111, 24);
            this.txtId.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtId.MinimumSize = new System.Drawing.Size(125, 20);
            this.txtId.Name = "txtId";
            this.txtId.ReadOnly = true;
            this.txtId.Size = new System.Drawing.Size(125, 20);
            this.txtId.TabIndex = 0;
            this.txtId.TabStop = false;
            this.txtId.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtVehicleID_KeyDown);
            // 
            // txtShiftName
            // 
            this.txtShiftName.Location = new System.Drawing.Point(111, 48);
            this.txtShiftName.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtShiftName.MinimumSize = new System.Drawing.Size(250, 20);
            this.txtShiftName.Name = "txtShiftName";
            this.txtShiftName.Size = new System.Drawing.Size(250, 20);
            this.txtShiftName.TabIndex = 3;
            this.txtShiftName.TextChanged += new System.EventHandler(this.txtVehicleType_TextChanged);
            this.txtShiftName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtVehicleType_KeyDown);
            // 
            // txtSL
            // 
            this.txtSL.Location = new System.Drawing.Point(111, 72);
            this.txtSL.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtSL.MinimumSize = new System.Drawing.Size(125, 20);
            this.txtSL.Name = "txtSL";
            this.txtSL.Size = new System.Drawing.Size(125, 20);
            this.txtSL.TabIndex = 4;
            this.txtSL.TextChanged += new System.EventHandler(this.txtVehicleNo_TextChanged);
            this.txtSL.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtVehicleNo_KeyDown);
            this.txtSL.Leave += new System.EventHandler(this.txtVehicleNo_Leave);
            // 
            // txtRemarks
            // 
            this.txtRemarks.Location = new System.Drawing.Point(111, 145);
            this.txtRemarks.MaximumSize = new System.Drawing.Size(4, 20);
            this.txtRemarks.MinimumSize = new System.Drawing.Size(320, 30);
            this.txtRemarks.Multiline = true;
            this.txtRemarks.Name = "txtRemarks";
            this.txtRemarks.Size = new System.Drawing.Size(320, 30);
            this.txtRemarks.TabIndex = 6;
            this.txtRemarks.TabStop = false;
            this.txtRemarks.TextChanged += new System.EventHandler(this.txtComments_TextChanged);
            this.txtRemarks.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtComments_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(28, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(17, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Id";
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
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Image = global::VATClient.Properties.Resources.Back;
            this.btnClose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnClose.Location = new System.Drawing.Point(402, 6);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 28);
            this.btnClose.TabIndex = 13;
            this.btnClose.Text = "Clo&se";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::VATClient.Properties.Resources.Green_Footer;
            this.panel1.Controls.Add(this.btnUpdate);
            this.panel1.Controls.Add(this.btnPrint);
            this.panel1.Controls.Add(this.btnClose);
            this.panel1.Controls.Add(this.btnAdd);
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Controls.Add(this.btnDelete);
            this.panel1.Location = new System.Drawing.Point(-2, 221);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(487, 40);
            this.panel1.TabIndex = 8;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // btnUpdate
            // 
            this.btnUpdate.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnUpdate.Image = global::VATClient.Properties.Resources.Update;
            this.btnUpdate.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnUpdate.Location = new System.Drawing.Point(91, 6);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(75, 28);
            this.btnUpdate.TabIndex = 10;
            this.btnUpdate.Text = "&Update";
            this.btnUpdate.UseVisualStyleBackColor = false;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // btnPrint
            // 
            this.btnPrint.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnPrint.Image = global::VATClient.Properties.Resources.Print;
            this.btnPrint.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPrint.Location = new System.Drawing.Point(321, 6);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(75, 28);
            this.btnPrint.TabIndex = 4;
            this.btnPrint.Text = "&Print";
            this.btnPrint.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnPrint.UseVisualStyleBackColor = false;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
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
            this.btnCancel.Location = new System.Drawing.Point(240, 6);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 28);
            this.btnCancel.TabIndex = 12;
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
            this.btnDelete.Location = new System.Drawing.Point(165, 51);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 28);
            this.btnDelete.TabIndex = 11;
            this.btnDelete.Text = "&Delete";
            this.btnDelete.UseVisualStyleBackColor = false;
            this.btnDelete.Visible = false;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ForeColor = System.Drawing.Color.Red;
            this.label8.Location = new System.Drawing.Point(95, 105);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(14, 14);
            this.label8.TabIndex = 279;
            this.label8.Text = "*";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.ForeColor = System.Drawing.Color.Red;
            this.label9.Location = new System.Drawing.Point(95, 129);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(14, 14);
            this.label9.TabIndex = 280;
            this.label9.Text = "*";
            // 
            // FormShifts
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(484, 261);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(500, 300);
            this.MinimumSize = new System.Drawing.Size(500, 300);
            this.Name = "FormShifts";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Shift";
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
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtId;
        private System.Windows.Forms.TextBox txtShiftName;
        private System.Windows.Forms.TextBox txtSL;
        private System.Windows.Forms.TextBox txtRemarks;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Button btnAddNew;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.ComponentModel.BackgroundWorker bgwAdd;
        private System.ComponentModel.BackgroundWorker bgrDelete;
        private System.ComponentModel.BackgroundWorker bgwUpdate;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.DateTimePicker dtpShiftEnd;
        private System.Windows.Forms.DateTimePicker dtpShiftStart;
        private System.Windows.Forms.CheckBox chkNextDay;
        public System.Windows.Forms.Button btnAdd;
        public System.Windows.Forms.Button btnDelete;
        public System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
    }
}