namespace VATClient
{
    partial class FormReportSCBL_Production
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
            this.label1 = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.cmbFiscalYear = new System.Windows.Forms.ComboBox();
            this.cmbFontSize = new System.Windows.Forms.ComboBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnPrev = new System.Windows.Forms.Button();
            this.bgwSelectYear = new System.ComponentModel.BackgroundWorker();
            this.bgwPreview = new System.ComponentModel.BackgroundWorker();
            this.txtProName = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.txtItemNo = new System.Windows.Forms.TextBox();
            this.txtUOM = new System.Windows.Forms.TextBox();
            this.grbBankInformation.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // grbBankInformation
            // 
            this.grbBankInformation.Controls.Add(this.txtUOM);
            this.grbBankInformation.Controls.Add(this.txtItemNo);
            this.grbBankInformation.Controls.Add(this.txtProName);
            this.grbBankInformation.Controls.Add(this.label10);
            this.grbBankInformation.Controls.Add(this.label1);
            this.grbBankInformation.Controls.Add(this.progressBar1);
            this.grbBankInformation.Controls.Add(this.cmbFiscalYear);
            this.grbBankInformation.Controls.Add(this.cmbFontSize);
            this.grbBankInformation.Location = new System.Drawing.Point(5, 5);
            this.grbBankInformation.Name = "grbBankInformation";
            this.grbBankInformation.Size = new System.Drawing.Size(449, 116);
            this.grbBankInformation.TabIndex = 83;
            this.grbBankInformation.TabStop = false;
            this.grbBankInformation.Text = "Reporting Criteria";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 47);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 188;
            this.label1.Text = "Year";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(102, 71);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(196, 22);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 196;
            this.progressBar1.Visible = false;
            // 
            // cmbFiscalYear
            // 
            this.cmbFiscalYear.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFiscalYear.FormattingEnabled = true;
            this.cmbFiscalYear.Location = new System.Drawing.Point(102, 44);
            this.cmbFiscalYear.Name = "cmbFiscalYear";
            this.cmbFiscalYear.Size = new System.Drawing.Size(176, 21);
            this.cmbFiscalYear.Sorted = true;
            this.cmbFiscalYear.TabIndex = 187;
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
            this.cmbFontSize.Location = new System.Drawing.Point(2, 95);
            this.cmbFontSize.Name = "cmbFontSize";
            this.cmbFontSize.Size = new System.Drawing.Size(36, 21);
            this.cmbFontSize.TabIndex = 522;
            this.cmbFontSize.Text = "8";
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::VATClient.Properties.Resources.Green_Footer;
            this.panel1.Controls.Add(this.btnPrev);
            this.panel1.Location = new System.Drawing.Point(5, 124);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(449, 40);
            this.panel1.TabIndex = 82;
            // 
            // btnPrev
            // 
            this.btnPrev.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnPrev.Image = global::VATClient.Properties.Resources.Print;
            this.btnPrev.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPrev.Location = new System.Drawing.Point(197, 4);
            this.btnPrev.Name = "btnPrev";
            this.btnPrev.Size = new System.Drawing.Size(72, 28);
            this.btnPrev.TabIndex = 42;
            this.btnPrev.Text = "Preview";
            this.btnPrev.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnPrev.UseVisualStyleBackColor = false;
            this.btnPrev.Click += new System.EventHandler(this.btnPrev_Click_1);
            // 
            // bgwSelectYear
            // 
            this.bgwSelectYear.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwSelectYear_DoWork);
            this.bgwSelectYear.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwSelectYear_RunWorkerCompleted);
            // 
            // bgwPreview
            // 
            this.bgwPreview.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwPreview_DoWork);
            this.bgwPreview.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwPreview_RunWorkerCompleted);
            // 
            // txtProName
            // 
            this.txtProName.Location = new System.Drawing.Point(102, 18);
            this.txtProName.MaximumSize = new System.Drawing.Size(400, 20);
            this.txtProName.MinimumSize = new System.Drawing.Size(150, 20);
            this.txtProName.Name = "txtProName";
            this.txtProName.ReadOnly = true;
            this.txtProName.Size = new System.Drawing.Size(341, 20);
            this.txtProName.TabIndex = 561;
            this.txtProName.DoubleClick += new System.EventHandler(this.txtProName_DoubleClick);
            this.txtProName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtProName_KeyDown);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(6, 22);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(93, 13);
            this.label10.TabIndex = 560;
            this.label10.Text = "Product Name(F9)";
            // 
            // txtItemNo
            // 
            this.txtItemNo.Location = new System.Drawing.Point(318, 44);
            this.txtItemNo.Name = "txtItemNo";
            this.txtItemNo.ReadOnly = true;
            this.txtItemNo.Size = new System.Drawing.Size(48, 20);
            this.txtItemNo.TabIndex = 562;
            this.txtItemNo.Visible = false;
            // 
            // txtUOM
            // 
            this.txtUOM.Location = new System.Drawing.Point(372, 44);
            this.txtUOM.Name = "txtUOM";
            this.txtUOM.ReadOnly = true;
            this.txtUOM.Size = new System.Drawing.Size(48, 20);
            this.txtUOM.TabIndex = 563;
            this.txtUOM.Visible = false;
            // 
            // FormReportSCBL_Production
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(456, 168);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.grbBankInformation);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormReportSCBL_Production";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Monthly Production & Delivery Report";
            this.Load += new System.EventHandler(this.ReportSCBL_Load);
            this.grbBankInformation.ResumeLayout(false);
            this.grbBankInformation.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grbBankInformation;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnPrev;
        private System.Windows.Forms.ComboBox cmbFontSize;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.ComboBox cmbFiscalYear;
        private System.ComponentModel.BackgroundWorker bgwSelectYear;
        private System.ComponentModel.BackgroundWorker bgwPreview;
        public System.Windows.Forms.TextBox txtProName;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtItemNo;
        private System.Windows.Forms.TextBox txtUOM;
    }
}