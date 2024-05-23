namespace VATClient.ReportPreview
{
    partial class FormMIS19
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
            this.grbBankInformation = new System.Windows.Forms.GroupBox();
            this.cmbFontSize = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.dtpEnd2 = new System.Windows.Forms.DateTimePicker();
            this.dtpFrom2 = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.dtpEnd1 = new System.Windows.Forms.DateTimePicker();
            this.dtpFrom1 = new System.Windows.Forms.DateTimePicker();
            this.label16 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnPrev = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.grbBankInformation.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // grbBankInformation
            // 
            this.grbBankInformation.Controls.Add(this.cmbFontSize);
            this.grbBankInformation.Controls.Add(this.label4);
            this.grbBankInformation.Controls.Add(this.label2);
            this.grbBankInformation.Controls.Add(this.dtpEnd2);
            this.grbBankInformation.Controls.Add(this.dtpFrom2);
            this.grbBankInformation.Controls.Add(this.label1);
            this.grbBankInformation.Controls.Add(this.dtpEnd1);
            this.grbBankInformation.Controls.Add(this.dtpFrom1);
            this.grbBankInformation.Controls.Add(this.label16);
            this.grbBankInformation.Location = new System.Drawing.Point(11, 15);
            this.grbBankInformation.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.grbBankInformation.Name = "grbBankInformation";
            this.grbBankInformation.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.grbBankInformation.Size = new System.Drawing.Size(556, 149);
            this.grbBankInformation.TabIndex = 87;
            this.grbBankInformation.TabStop = false;
            this.grbBankInformation.Text = "Reporting Criteria";
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
            this.cmbFontSize.Location = new System.Drawing.Point(8, 116);
            this.cmbFontSize.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmbFontSize.Name = "cmbFontSize";
            this.cmbFontSize.Size = new System.Drawing.Size(47, 24);
            this.cmbFontSize.TabIndex = 195;
            this.cmbFontSize.Text = "8";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 82);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(182, 17);
            this.label4.TabIndex = 50;
            this.label4.Text = "2nd Comparision Start Date";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(351, 87);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(25, 17);
            this.label2.TabIndex = 49;
            this.label2.Text = "To";
            // 
            // dtpEnd2
            // 
            this.dtpEnd2.CustomFormat = "dd/MMM/yyyy";
            this.dtpEnd2.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpEnd2.Location = new System.Drawing.Point(385, 82);
            this.dtpEnd2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.dtpEnd2.Name = "dtpEnd2";
            this.dtpEnd2.Size = new System.Drawing.Size(147, 22);
            this.dtpEnd2.TabIndex = 48;
            // 
            // dtpFrom2
            // 
            this.dtpFrom2.CustomFormat = "dd/MMM/yyyy";
            this.dtpFrom2.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpFrom2.Location = new System.Drawing.Point(199, 82);
            this.dtpFrom2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.dtpFrom2.Name = "dtpFrom2";
            this.dtpFrom2.Size = new System.Drawing.Size(147, 22);
            this.dtpFrom2.TabIndex = 47;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(351, 54);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(25, 17);
            this.label1.TabIndex = 45;
            this.label1.Text = "To";
            // 
            // dtpEnd1
            // 
            this.dtpEnd1.CustomFormat = "dd/MMM/yyyy";
            this.dtpEnd1.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpEnd1.Location = new System.Drawing.Point(385, 49);
            this.dtpEnd1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.dtpEnd1.Name = "dtpEnd1";
            this.dtpEnd1.Size = new System.Drawing.Size(147, 22);
            this.dtpEnd1.TabIndex = 43;
            // 
            // dtpFrom1
            // 
            this.dtpFrom1.CustomFormat = "dd/MMM/yyyy";
            this.dtpFrom1.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpFrom1.Location = new System.Drawing.Point(199, 49);
            this.dtpFrom1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.dtpFrom1.Name = "dtpFrom1";
            this.dtpFrom1.Size = new System.Drawing.Size(147, 22);
            this.dtpFrom1.TabIndex = 42;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(9, 52);
            this.label16.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(177, 17);
            this.label16.TabIndex = 39;
            this.label16.Text = "1st Comparision Start Date";
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::VATClient.Properties.Resources.Green_Footer;
            this.panel1.Controls.Add(this.btnPrev);
            this.panel1.Controls.Add(this.btnClose);
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Location = new System.Drawing.Point(1, 190);
            this.panel1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(565, 49);
            this.panel1.TabIndex = 88;
            // 
            // btnPrev
            // 
            this.btnPrev.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnPrev.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPrev.Location = new System.Drawing.Point(8, 7);
            this.btnPrev.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnPrev.Name = "btnPrev";
            this.btnPrev.Size = new System.Drawing.Size(96, 34);
            this.btnPrev.TabIndex = 43;
            this.btnPrev.Text = "Preview";
            this.btnPrev.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnPrev.UseVisualStyleBackColor = false;
            this.btnPrev.Click += new System.EventHandler(this.btnPrev_Click);
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnClose.Image = global::VATClient.Properties.Resources.Back;
            this.btnClose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnClose.Location = new System.Drawing.Point(452, 7);
            this.btnClose.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(100, 34);
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
            this.btnCancel.Location = new System.Drawing.Point(344, 7);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 34);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "&Refresh";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // FormMIS19
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(569, 240);
            this.Controls.Add(this.grbBankInformation);
            this.Controls.Add(this.panel1);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormMIS19";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MIS 19";
            this.Load += new System.EventHandler(this.FormMIS19_Load);
            this.grbBankInformation.ResumeLayout(false);
            this.grbBankInformation.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grbBankInformation;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.DateTimePicker dtpEnd1;
        public System.Windows.Forms.DateTimePicker dtpFrom1;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnPrev;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.DateTimePicker dtpEnd2;
        public System.Windows.Forms.DateTimePicker dtpFrom2;
        private System.Windows.Forms.ComboBox cmbFontSize;
    }
}