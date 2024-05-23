namespace VATClient.Integration.NBR
{
    partial class FormNBRAPI
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormNBRAPI));
            this.btnXML = new System.Windows.Forms.Button();
            this.txtXMLText = new System.Windows.Forms.TextBox();
            this.bgwLoadXML = new System.ComponentModel.BackgroundWorker();
            this.progressBar2 = new System.Windows.Forms.ProgressBar();
            this.btnSend = new System.Windows.Forms.Button();
            this.dptMonth = new System.Windows.Forms.DateTimePicker();
            this.btnFile = new System.Windows.Forms.Button();
            this.btnJson = new System.Windows.Forms.Button();
            this.bgwLoadJson = new System.ComponentModel.BackgroundWorker();
            this.button1 = new System.Windows.Forms.Button();
            this.btnSubmit = new System.Windows.Forms.Button();
            this.bgwSubmitAPI = new System.ComponentModel.BackgroundWorker();
            this.label3 = new System.Windows.Forms.Label();
            this.txtPeriodName = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnXML
            // 
            this.btnXML.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnXML.Location = new System.Drawing.Point(3, 261);
            this.btnXML.Margin = new System.Windows.Forms.Padding(4);
            this.btnXML.Name = "btnXML";
            this.btnXML.Size = new System.Drawing.Size(126, 44);
            this.btnXML.TabIndex = 0;
            this.btnXML.Text = "Get XML";
            this.btnXML.UseVisualStyleBackColor = false;
            this.btnXML.Visible = false;
            this.btnXML.Click += new System.EventHandler(this.btnXML_Click);
            // 
            // txtXMLText
            // 
            this.txtXMLText.Location = new System.Drawing.Point(13, 269);
            this.txtXMLText.Margin = new System.Windows.Forms.Padding(4);
            this.txtXMLText.Multiline = true;
            this.txtXMLText.Name = "txtXMLText";
            this.txtXMLText.Size = new System.Drawing.Size(396, 138);
            this.txtXMLText.TabIndex = 1;
            this.txtXMLText.Visible = false;
            // 
            // bgwLoadXML
            // 
            this.bgwLoadXML.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwLoadXML_DoWork);
            this.bgwLoadXML.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwLoadXML_RunWorkerCompleted);
            // 
            // progressBar2
            // 
            this.progressBar2.Location = new System.Drawing.Point(35, 239);
            this.progressBar2.Margin = new System.Windows.Forms.Padding(4);
            this.progressBar2.Name = "progressBar2";
            this.progressBar2.Size = new System.Drawing.Size(336, 20);
            this.progressBar2.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar2.TabIndex = 3;
            this.progressBar2.Visible = false;
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(137, 269);
            this.btnSend.Margin = new System.Windows.Forms.Padding(4);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(88, 28);
            this.btnSend.TabIndex = 4;
            this.btnSend.Text = "Send XML";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Visible = false;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // dptMonth
            // 
            this.dptMonth.CustomFormat = "MMMM-yyyy";
            this.dptMonth.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dptMonth.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dptMonth.Location = new System.Drawing.Point(325, 157);
            this.dptMonth.Margin = new System.Windows.Forms.Padding(4);
            this.dptMonth.Name = "dptMonth";
            this.dptMonth.Size = new System.Drawing.Size(87, 24);
            this.dptMonth.TabIndex = 5;
            this.dptMonth.Visible = false;
            // 
            // btnFile
            // 
            this.btnFile.Location = new System.Drawing.Point(233, 262);
            this.btnFile.Margin = new System.Windows.Forms.Padding(4);
            this.btnFile.Name = "btnFile";
            this.btnFile.Size = new System.Drawing.Size(116, 28);
            this.btnFile.TabIndex = 6;
            this.btnFile.Text = "Select Files";
            this.btnFile.UseVisualStyleBackColor = true;
            this.btnFile.Visible = false;
            this.btnFile.Click += new System.EventHandler(this.btnFile_Click);
            // 
            // btnJson
            // 
            this.btnJson.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnJson.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnJson.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnJson.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnJson.Image = ((System.Drawing.Image)(resources.GetObject("btnJson.Image")));
            this.btnJson.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnJson.Location = new System.Drawing.Point(119, 49);
            this.btnJson.Margin = new System.Windows.Forms.Padding(4);
            this.btnJson.Name = "btnJson";
            this.btnJson.Size = new System.Drawing.Size(187, 89);
            this.btnJson.TabIndex = 4;
            this.btnJson.Text = "Send";
            this.btnJson.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnJson.UseVisualStyleBackColor = false;
            this.btnJson.Click += new System.EventHandler(this.btnJson_Click);
            // 
            // bgwLoadJson
            // 
            this.bgwLoadJson.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwLoadJson_DoWork);
            this.bgwLoadJson.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwLoadJson_RunWorkerCompleted);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(347, 269);
            this.button1.Margin = new System.Windows.Forms.Padding(4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(116, 28);
            this.button1.TabIndex = 7;
            this.button1.Text = "Select Files";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnSubmit
            // 
            this.btnSubmit.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnSubmit.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnSubmit.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSubmit.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSubmit.Image = ((System.Drawing.Image)(resources.GetObject("btnSubmit.Image")));
            this.btnSubmit.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnSubmit.Location = new System.Drawing.Point(119, 147);
            this.btnSubmit.Margin = new System.Windows.Forms.Padding(4);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(187, 89);
            this.btnSubmit.TabIndex = 8;
            this.btnSubmit.Text = "Submit";
            this.btnSubmit.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnSubmit.UseVisualStyleBackColor = false;
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // bgwSubmitAPI
            // 
            this.bgwSubmitAPI.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwSubmitAPI_DoWork);
            this.bgwSubmitAPI.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwSubmitAPI_RunWorkerCompleted);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 11);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(89, 17);
            this.label3.TabIndex = 249;
            this.label3.Text = "Fiscal Period";
            // 
            // txtPeriodName
            // 
            this.txtPeriodName.Location = new System.Drawing.Point(119, 8);
            this.txtPeriodName.Margin = new System.Windows.Forms.Padding(4);
            this.txtPeriodName.Name = "txtPeriodName";
            this.txtPeriodName.ReadOnly = true;
            this.txtPeriodName.Size = new System.Drawing.Size(160, 22);
            this.txtPeriodName.TabIndex = 248;
            // 
            // FormNBRAPI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(425, 294);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtPeriodName);
            this.Controls.Add(this.btnSubmit);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnXML);
            this.Controls.Add(this.btnJson);
            this.Controls.Add(this.btnFile);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.dptMonth);
            this.Controls.Add(this.progressBar2);
            this.Controls.Add(this.txtXMLText);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.Name = "FormNBRAPI";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "NBR API";
            this.Load += new System.EventHandler(this.FormNBRAPI_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnXML;
        private System.Windows.Forms.TextBox txtXMLText;
        private System.ComponentModel.BackgroundWorker bgwLoadXML;
        private System.Windows.Forms.ProgressBar progressBar2;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.Button btnFile;
        private System.Windows.Forms.Button btnJson;
        private System.ComponentModel.BackgroundWorker bgwLoadJson;
        private System.Windows.Forms.Button button1;
        public System.Windows.Forms.DateTimePicker dptMonth;
        private System.Windows.Forms.Button btnSubmit;
        private System.ComponentModel.BackgroundWorker bgwSubmitAPI;
        private System.Windows.Forms.Label label3;
        public System.Windows.Forms.TextBox txtPeriodName;
    }
}