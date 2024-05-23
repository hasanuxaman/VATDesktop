namespace VATClient.ReportPreview
{
    partial class FormRptDispose
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
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rbtnRaw = new System.Windows.Forms.RadioButton();
            this.rbtnFinish = new System.Windows.Forms.RadioButton();
            this.txtDisposeNo = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.dtpPostTo = new System.Windows.Forms.DateTimePicker();
            this.dtpPostFrom = new System.Windows.Forms.DateTimePicker();
            this.panel1 = new System.Windows.Forms.Panel();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnPreview = new System.Windows.Forms.Button();
            this.backgroundWorkerPreview = new System.ComponentModel.BackgroundWorker();
            this.progressBar2 = new System.Windows.Forms.ProgressBar();
            this.cmbFontSize = new System.Windows.Forms.ComboBox();
            this.groupBox2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rbtnRaw);
            this.groupBox2.Controls.Add(this.rbtnFinish);
            this.groupBox2.Location = new System.Drawing.Point(115, 182);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(157, 68);
            this.groupBox2.TabIndex = 204;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "groupBox2";
            // 
            // rbtnRaw
            // 
            this.rbtnRaw.AutoSize = true;
            this.rbtnRaw.Location = new System.Drawing.Point(6, 18);
            this.rbtnRaw.Name = "rbtnRaw";
            this.rbtnRaw.Size = new System.Drawing.Size(47, 17);
            this.rbtnRaw.TabIndex = 36;
            this.rbtnRaw.TabStop = true;
            this.rbtnRaw.Text = "Raw";
            this.rbtnRaw.UseVisualStyleBackColor = true;
            // 
            // rbtnFinish
            // 
            this.rbtnFinish.AutoSize = true;
            this.rbtnFinish.Location = new System.Drawing.Point(6, 36);
            this.rbtnFinish.Name = "rbtnFinish";
            this.rbtnFinish.Size = new System.Drawing.Size(52, 17);
            this.rbtnFinish.TabIndex = 34;
            this.rbtnFinish.TabStop = true;
            this.rbtnFinish.Text = "Finish";
            this.rbtnFinish.UseVisualStyleBackColor = true;
            // 
            // txtDisposeNo
            // 
            this.txtDisposeNo.BackColor = System.Drawing.SystemColors.Control;
            this.txtDisposeNo.Location = new System.Drawing.Point(121, 21);
            this.txtDisposeNo.MaximumSize = new System.Drawing.Size(4, 25);
            this.txtDisposeNo.MinimumSize = new System.Drawing.Size(185, 20);
            this.txtDisposeNo.Name = "txtDisposeNo";
            this.txtDisposeNo.ReadOnly = true;
            this.txtDisposeNo.Size = new System.Drawing.Size(185, 20);
            this.txtDisposeNo.TabIndex = 205;
            this.txtDisposeNo.TabStop = false;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(45, 25);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(62, 13);
            this.label10.TabIndex = 206;
            this.label10.Text = "Dispose No";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(24, 67);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(54, 13);
            this.label16.TabIndex = 208;
            this.label16.Text = "Post Date";
            this.label16.Visible = false;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(224, 67);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(16, 13);
            this.label11.TabIndex = 211;
            this.label11.Text = "to";
            this.label11.Visible = false;
            // 
            // dtpPostTo
            // 
            this.dtpPostTo.Checked = false;
            this.dtpPostTo.CustomFormat = "dd/MMM/yyyy";
            this.dtpPostTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpPostTo.Location = new System.Drawing.Point(259, 64);
            this.dtpPostTo.Name = "dtpPostTo";
            this.dtpPostTo.ShowCheckBox = true;
            this.dtpPostTo.Size = new System.Drawing.Size(108, 20);
            this.dtpPostTo.TabIndex = 210;
            this.dtpPostTo.Value = new System.DateTime(2015, 3, 20, 13, 6, 0, 0);
            this.dtpPostTo.Visible = false;
            // 
            // dtpPostFrom
            // 
            this.dtpPostFrom.Checked = false;
            this.dtpPostFrom.CustomFormat = "dd/MMM/yyyy";
            this.dtpPostFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpPostFrom.Location = new System.Drawing.Point(99, 64);
            this.dtpPostFrom.Name = "dtpPostFrom";
            this.dtpPostFrom.ShowCheckBox = true;
            this.dtpPostFrom.Size = new System.Drawing.Size(117, 20);
            this.dtpPostFrom.TabIndex = 209;
            this.dtpPostFrom.Value = new System.DateTime(2001, 3, 20, 13, 6, 0, 0);
            this.dtpPostFrom.Visible = false;
            this.dtpPostFrom.ValueChanged += new System.EventHandler(this.dtpPostFrom_ValueChanged);
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::VATClient.Properties.Resources.Green_Footer;
            this.panel1.Controls.Add(this.progressBar1);
            this.panel1.Controls.Add(this.btnClose);
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Controls.Add(this.btnPreview);
            this.panel1.Location = new System.Drawing.Point(-13, 117);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(398, 40);
            this.panel1.TabIndex = 212;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(120, -49);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(250, 22);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 195;
            this.progressBar1.Visible = false;
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnClose.Image = global::VATClient.Properties.Resources.Back;
            this.btnClose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnClose.Location = new System.Drawing.Point(304, 7);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 28);
            this.btnClose.TabIndex = 8;
            this.btnClose.Text = "&Close";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnCancel.Image = global::VATClient.Properties.Resources.Referesh;
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.Location = new System.Drawing.Point(104, 7);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 28);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "&Refresh";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnPreview
            // 
            this.btnPreview.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnPreview.Image = global::VATClient.Properties.Resources.Print;
            this.btnPreview.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPreview.Location = new System.Drawing.Point(23, 7);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new System.Drawing.Size(75, 28);
            this.btnPreview.TabIndex = 41;
            this.btnPreview.Text = "Preview";
            this.btnPreview.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnPreview.UseVisualStyleBackColor = false;
            this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
            // 
            // backgroundWorkerPreview
            // 
            this.backgroundWorkerPreview.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerPreview_DoWork);
            this.backgroundWorkerPreview.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerPreview_RunWorkerCompleted);
            // 
            // progressBar2
            // 
            this.progressBar2.Location = new System.Drawing.Point(38, 43);
            this.progressBar2.Name = "progressBar2";
            this.progressBar2.Size = new System.Drawing.Size(309, 21);
            this.progressBar2.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar2.TabIndex = 196;
            this.progressBar2.Visible = false;
            // 
            // cmbFontSize
            // 
            this.cmbFontSize.FormattingEnabled = true;
            this.cmbFontSize.Items.AddRange(new object[] {
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15"});
            this.cmbFontSize.Location = new System.Drawing.Point(1, 90);
            this.cmbFontSize.Name = "cmbFontSize";
            this.cmbFontSize.Size = new System.Drawing.Size(36, 21);
            this.cmbFontSize.TabIndex = 213;
            this.cmbFontSize.Text = "8";
            // 
            // FormRptDispose
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(384, 158);
            this.Controls.Add(this.cmbFontSize);
            this.Controls.Add(this.progressBar2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.dtpPostTo);
            this.Controls.Add(this.dtpPostFrom);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.txtDisposeNo);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.groupBox2);
            this.Name = "FormRptDispose";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Dispose Report";
            this.Load += new System.EventHandler(this.FormRptDispose_Load);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        public System.Windows.Forms.RadioButton rbtnRaw;
        public System.Windows.Forms.RadioButton rbtnFinish;
        private System.Windows.Forms.Label label10;
        public System.Windows.Forms.TextBox txtDisposeNo;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label11;
        public System.Windows.Forms.DateTimePicker dtpPostTo;
        public System.Windows.Forms.DateTimePicker dtpPostFrom;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnPreview;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.ComponentModel.BackgroundWorker backgroundWorkerPreview;
        private System.Windows.Forms.ProgressBar progressBar2;
        private System.Windows.Forms.ComboBox cmbFontSize;
    }
}