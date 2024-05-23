using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VATClient.ReportPreview
{
    public partial class FormReport : Form
    {

        public FormReport()
        {
            InitializeComponent();
        }
        CrystalDecisions.CrystalReports.Engine.ReportDocument Objreport = null;
        public void setReportSource(CrystalDecisions.CrystalReports.Engine.ReportDocument report)
        {
            Objreport = report;
            this.crystalReportViewer1.ReportSource = Objreport;
        }

        private void crystalReportViewer1_Load(object sender, EventArgs e)
        {

        }

        private void FormReport_Load(object sender, EventArgs e)
        {

        }

        private void FormReport_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Objreport != null)
            {
                Objreport.Close();
                Objreport.Dispose();
            }
           
        }

    }
}
