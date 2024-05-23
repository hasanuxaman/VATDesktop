namespace VATClient
{
    partial class FormSaleTempProcess
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
            this.btnLocal = new System.Windows.Forms.Button();
            this.btnProcess = new System.Windows.Forms.Button();
            this.LUnprocessed = new System.Windows.Forms.Label();
            this.bgwGetUnprocessedData = new System.ComponentModel.BackgroundWorker();
            this.pbProcess = new System.Windows.Forms.ProgressBar();
            this.bgwProcessTempData = new System.ComponentModel.BackgroundWorker();
            this.SuspendLayout();
            // 
            // btnLocal
            // 
            this.btnLocal.Location = new System.Drawing.Point(31, 52);
            this.btnLocal.Name = "btnLocal";
            this.btnLocal.Size = new System.Drawing.Size(75, 23);
            this.btnLocal.TabIndex = 0;
            this.btnLocal.Text = "Local";
            this.btnLocal.UseVisualStyleBackColor = true;
            this.btnLocal.Click += new System.EventHandler(this.btnLocal_Click);
            // 
            // btnProcess
            // 
            this.btnProcess.Location = new System.Drawing.Point(238, 52);
            this.btnProcess.Name = "btnProcess";
            this.btnProcess.Size = new System.Drawing.Size(75, 23);
            this.btnProcess.TabIndex = 1;
            this.btnProcess.Text = "Process";
            this.btnProcess.UseVisualStyleBackColor = true;
            this.btnProcess.Click += new System.EventHandler(this.btnProcess_Click);
            // 
            // LUnprocessed
            // 
            this.LUnprocessed.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LUnprocessed.Location = new System.Drawing.Point(67, 145);
            this.LUnprocessed.Name = "LUnprocessed";
            this.LUnprocessed.Size = new System.Drawing.Size(183, 13);
            this.LUnprocessed.TabIndex = 2;
            this.LUnprocessed.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // bgwGetUnprocessedData
            // 
            this.bgwGetUnprocessedData.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwGetUnprocessedData_DoWork);
            this.bgwGetUnprocessedData.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwGetUnprocessedData_RunWorkerCompleted);
            // 
            // pbProcess
            // 
            this.pbProcess.Location = new System.Drawing.Point(89, 135);
            this.pbProcess.Name = "pbProcess";
            this.pbProcess.Size = new System.Drawing.Size(135, 23);
            this.pbProcess.TabIndex = 3;
            this.pbProcess.Visible = false;
            // 
            // bgwProcessTempData
            // 
            this.bgwProcessTempData.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwProcessTempData_DoWork);
            this.bgwProcessTempData.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwProcessTempData_RunWorkerCompleted);
            // 
            // FormSaleTempProcess
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(362, 262);
            this.Controls.Add(this.pbProcess);
            this.Controls.Add(this.LUnprocessed);
            this.Controls.Add(this.btnProcess);
            this.Controls.Add(this.btnLocal);
            this.Name = "FormSaleTempProcess";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Sale Data Process";
            this.Load += new System.EventHandler(this.FormSaleTempProcess_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnLocal;
        private System.Windows.Forms.Button btnProcess;
        private System.Windows.Forms.Label LUnprocessed;
        private System.ComponentModel.BackgroundWorker bgwGetUnprocessedData;
        private System.Windows.Forms.ProgressBar pbProcess;
        private System.ComponentModel.BackgroundWorker bgwProcessTempData;
    }
}