namespace VATClient
{
    partial class FormRegularProcess
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnOk = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblCurrentProcess = new System.Windows.Forms.Label();
            this.btnProcess = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.bgwProcess = new System.ComponentModel.BackgroundWorker();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::VATClient.Properties.Resources.Green_Footer;
            this.panel1.Controls.Add(this.btnOk);
            this.panel1.Location = new System.Drawing.Point(1, 314);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(480, 48);
            this.panel1.TabIndex = 12;
            this.panel1.TabStop = true;
            // 
            // btnOk
            // 
            this.btnOk.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnOk.Image = global::VATClient.Properties.Resources.Back;
            this.btnOk.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOk.Location = new System.Drawing.Point(390, 8);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(87, 37);
            this.btnOk.TabIndex = 14;
            this.btnOk.Text = "&Ok";
            this.btnOk.UseVisualStyleBackColor = false;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblCurrentProcess);
            this.groupBox1.Controls.Add(this.btnProcess);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(454, 215);
            this.groupBox1.TabIndex = 579;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Process Control";
            // 
            // lblCurrentProcess
            // 
            this.lblCurrentProcess.AutoSize = true;
            this.lblCurrentProcess.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCurrentProcess.Location = new System.Drawing.Point(64, 156);
            this.lblCurrentProcess.Name = "lblCurrentProcess";
            this.lblCurrentProcess.Size = new System.Drawing.Size(0, 20);
            this.lblCurrentProcess.TabIndex = 11;
            this.lblCurrentProcess.Visible = false;
            // 
            // btnProcess
            // 
            this.btnProcess.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.btnProcess.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnProcess.Image = global::VATClient.Properties.Resources.Referesh;
            this.btnProcess.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnProcess.Location = new System.Drawing.Point(93, 65);
            this.btnProcess.Name = "btnProcess";
            this.btnProcess.Size = new System.Drawing.Size(243, 64);
            this.btnProcess.TabIndex = 10;
            this.btnProcess.Text = "Process";
            this.btnProcess.UseVisualStyleBackColor = false;
            this.btnProcess.Click += new System.EventHandler(this.btnProcess_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(12, 257);
            this.progressBar1.Maximum = 4;
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(454, 39);
            this.progressBar1.TabIndex = 580;
            this.progressBar1.Visible = false;
            // 
            // bgwProcess
            // 
            this.bgwProcess.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwProcess_DoWork);
            this.bgwProcess.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwProcess_RunWorkerCompleted);
            // 
            // FormRegularProcess
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(478, 362);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.panel1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormRegularProcess";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Daily Activities";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormRegularProcess_FormClosing);
            this.panel1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnProcess;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.ComponentModel.BackgroundWorker bgwProcess;
        private System.Windows.Forms.Label lblCurrentProcess;
    }
}