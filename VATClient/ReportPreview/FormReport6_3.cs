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
    public partial class FormReport6_3 : Form
    {
        public FormReport6_3()
        { 
            InitializeComponent();
        }

        private CrystalDecisions.CrystalReports.Engine.ReportDocument reportObj = null;


        public void setReportSource(CrystalDecisions.CrystalReports.Engine.ReportDocument report)
        {
            reportObj = report;
            this.crystalReportViewer1.ReportSource = report;
        }

        private void crystalReportViewer1_Load(object sender, EventArgs e)
        {

        }

        private void FormReport_Load(object sender, EventArgs e)
        {

        }

        private void FormReport6_3_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (reportObj != null)
            {
                reportObj.Close();
                reportObj.Dispose();
            }

        }

    }
}
