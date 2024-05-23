namespace VATClient
{
    partial class FormSaleImportmMarico
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
            this.components = new System.ComponentModel.Container();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.backgroundSaveSale = new System.ComponentModel.BackgroundWorker();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.stopBulk = new System.Windows.Forms.Button();
            this.btnBulk = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.bgwBigData = new System.ComponentModel.BackgroundWorker();
            this.bgwSaveUnprocessed = new System.ComponentModel.BackgroundWorker();
            this.bgwMARICO = new System.ComponentModel.BackgroundWorker();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.bgwIAS = new System.ComponentModel.BackgroundWorker();
            this.bgwMARICOPurchase = new System.ComponentModel.BackgroundWorker();
            this.bgwMaricoTransfer = new System.ComponentModel.BackgroundWorker();
            this.bgwSaveSales = new System.ComponentModel.BackgroundWorker();
            this.bgwMaricoFactoryTransfer = new System.ComponentModel.BackgroundWorker();
            this.bgwMaricoContractManufacturing = new System.ComponentModel.BackgroundWorker();
            this.bgwMARICOFactoryPurchase = new System.ComponentModel.BackgroundWorker();
            this.bgwMARICOFactoryIssue = new System.ComponentModel.BackgroundWorker();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(13, 129);
            this.progressBar1.Margin = new System.Windows.Forms.Padding(4);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(288, 28);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 7;
            this.progressBar1.Visible = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.stopBulk);
            this.groupBox1.Controls.Add(this.progressBar1);
            this.groupBox1.Controls.Add(this.btnBulk);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(16, 6);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(309, 176);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            // 
            // stopBulk
            // 
            this.stopBulk.BackColor = System.Drawing.Color.White;
            this.stopBulk.Location = new System.Drawing.Point(174, 52);
            this.stopBulk.Margin = new System.Windows.Forms.Padding(4);
            this.stopBulk.Name = "stopBulk";
            this.stopBulk.Size = new System.Drawing.Size(117, 42);
            this.stopBulk.TabIndex = 19;
            this.stopBulk.Text = "Process Stop";
            this.stopBulk.UseVisualStyleBackColor = false;
            this.stopBulk.Click += new System.EventHandler(this.stopBulk_Click);
            // 
            // btnBulk
            // 
            this.btnBulk.BackColor = System.Drawing.Color.White;
            this.btnBulk.Location = new System.Drawing.Point(25, 52);
            this.btnBulk.Margin = new System.Windows.Forms.Padding(4);
            this.btnBulk.Name = "btnBulk";
            this.btnBulk.Size = new System.Drawing.Size(115, 42);
            this.btnBulk.TabIndex = 17;
            this.btnBulk.Text = "Process Start";
            this.btnBulk.UseVisualStyleBackColor = false;
            this.btnBulk.Click += new System.EventHandler(this.btnBulk_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(204, -23);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(21, 17);
            this.label3.TabIndex = 10;
            this.label3.Text = "ID";
            // 
            // bgwSaveUnprocessed
            // 
            this.bgwSaveUnprocessed.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwSaveUnprocessed_DoWork);
            this.bgwSaveUnprocessed.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwSaveUnprocessed_RunWorkerCompleted);
            // 
            // bgwMARICO
            // 
            this.bgwMARICO.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwMARICO_DoWork);
            this.bgwMARICO.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwMARICO_RunWorkerCompleted);
            // 
            // timer1
            // 
            this.timer1.Interval = 30000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // bgwMARICOPurchase
            // 
            this.bgwMARICOPurchase.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwMARICOPurchase_DoWork);
            this.bgwMARICOPurchase.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwMARICOPurchase_RunWorkerCompleted);
            // 
            // bgwMaricoTransfer
            // 
            this.bgwMaricoTransfer.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwMaricoTransfer_DoWork);
            this.bgwMaricoTransfer.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwMaricoTransfer_RunWorkerCompleted);
            // 
            // bgwSaveSales
            // 
            this.bgwSaveSales.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwSaveSales_DoWork);
            this.bgwSaveSales.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwSaveSales_RunWorkerCompleted);
            // 
            // bgwMaricoFactoryTransfer
            // 
            this.bgwMaricoFactoryTransfer.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwMaricoFactoryTransfer_DoWork);
            this.bgwMaricoFactoryTransfer.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwMaricoFactoryTransfer_RunWorkerCompleted);
            // 
            // bgwMaricoContractManufacturing
            // 
            this.bgwMaricoContractManufacturing.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwMaricoContractManufacturing_DoWork);
            this.bgwMaricoContractManufacturing.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwMaricoContractManufacturing_RunWorkerCompleted);
            // 
            // bgwMARICOFactoryPurchase
            // 
            this.bgwMARICOFactoryPurchase.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwMARICOFactoryPurchase_DoWork);
            this.bgwMARICOFactoryPurchase.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwMARICOFactoryPurchase_RunWorkerCompleted);
            // 
            // bgwMARICOFactoryIssue
            // 
            this.bgwMARICOFactoryIssue.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwMARICOFactoryIssue_DoWork);
            this.bgwMARICOFactoryIssue.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwMARICOFactoryIssue_RunWorkerCompleted);
            // 
            // FormSaleImportmMarico
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(192)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(338, 195);
            this.Controls.Add(this.groupBox1);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "FormSaleImportmMarico";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Sale Import";
            this.Load += new System.EventHandler(this.FormMasterImport_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBar1;
        private System.ComponentModel.BackgroundWorker backgroundSaveSale;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label3;
        private System.ComponentModel.BackgroundWorker bgwBigData;
        private System.ComponentModel.BackgroundWorker bgwSaveUnprocessed;
        private System.Windows.Forms.Button btnBulk;
        private System.Windows.Forms.Button stopBulk;
        private System.ComponentModel.BackgroundWorker bgwMARICO;
        private System.Windows.Forms.Timer timer1;
        private System.ComponentModel.BackgroundWorker bgwIAS;
        private System.ComponentModel.BackgroundWorker bgwMARICOPurchase;
        private System.ComponentModel.BackgroundWorker bgwMaricoTransfer;
        private System.ComponentModel.BackgroundWorker bgwSaveSales;
        private System.ComponentModel.BackgroundWorker bgwMaricoFactoryTransfer;
        private System.ComponentModel.BackgroundWorker bgwMaricoContractManufacturing;
        private System.ComponentModel.BackgroundWorker bgwMARICOFactoryPurchase;
        private System.ComponentModel.BackgroundWorker bgwMARICOFactoryIssue;
    }
}