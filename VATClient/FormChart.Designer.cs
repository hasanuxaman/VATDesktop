namespace VATClient
{
    partial class FormChart
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.chartDayWiseSale = new System.Windows.Forms.DataVisualization.Charting.Chart();
            ((System.ComponentModel.ISupportInitialize)(this.chartDayWiseSale)).BeginInit();
            this.SuspendLayout();
            // 
            // chartDayWiseSale
            // 
            chartArea1.Name = "ChartArea1";
            this.chartDayWiseSale.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.chartDayWiseSale.Legends.Add(legend1);
            this.chartDayWiseSale.Location = new System.Drawing.Point(35, 12);
            this.chartDayWiseSale.Name = "chartDayWiseSale";
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series1.Legend = "Legend1";
            series1.Name = "SubTotal";
            this.chartDayWiseSale.Series.Add(series1);
            this.chartDayWiseSale.Size = new System.Drawing.Size(1307, 300);
            this.chartDayWiseSale.TabIndex = 0;
            this.chartDayWiseSale.Text = "DayWiseSale";
            // 
            // FormChart
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1354, 756);
            this.Controls.Add(this.chartDayWiseSale);
            this.Name = "FormChart";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FormChart";
            this.Load += new System.EventHandler(this.FormChart_Load);
            ((System.ComponentModel.ISupportInitialize)(this.chartDayWiseSale)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart chartDayWiseSale;
    }
}