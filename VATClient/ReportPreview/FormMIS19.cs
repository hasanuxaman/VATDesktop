using CrystalDecisions.CrystalReports.Engine;
using SymphonySofttech.Reports.List;
using SymphonySofttech.Reports.Report;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VATServer.Library;
using VATViewModel.DTOs;
using VATServer.License;

namespace VATClient.ReportPreview
{
    public partial class FormMIS19 : Form
    {
        public FormMIS19()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();
        }

        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        private void backgroundWorkerPreview_DoWork(object sender, DoWorkEventArgs e)
        {
        }

        private void backgroundWorkerPreview_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
          
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            try
            {
                Program.FontSize = cmbFontSize.Text.Trim() == "" ? "7" : cmbFontSize.Text.Trim();
                DBSQLConnection _dbsqlConnection = new DBSQLConnection();
             
                #region Statement
                List<MIS19VM> mis19s = new List<MIS19VM>();
                MIS19VM mis19A = new MIS19VM();
                MIS19VM mis19B = new MIS19VM();

            DataTable ReportResult1 = new DataTable();
            DataTable ReportResult2 = new DataTable();

            ReportDSDAL reportDsdal = new ReportDSDAL();
            ReportResult1 = reportDsdal.MIS19(dtpFrom1.Value.ToString("dd-MMM-yyyy"), dtpEnd1.Value.ToString("dd-MMM-yyyy"),connVM);
            ReportResult2 = reportDsdal.MIS19(dtpFrom2.Value.ToString("dd-MMM-yyyy"), dtpEnd2.Value.ToString("dd-MMM-yyyy"), connVM);
            string MonthA = dtpFrom1.Value.ToString("MMM/yy") + " To " + dtpEnd1.Value.ToString("MMM/yy");
            string MonthB = dtpFrom2.Value.ToString("MMM/yy") + " To " + dtpEnd2.Value.ToString("MMM/yy");
            #endregion

            #region preview


            #region 1
            foreach (DataRow item in ReportResult1.Rows)
            {
                mis19A = new MIS19VM();
                mis19A.One = item["One"].ToString();
                mis19A.Two = MonthA;
                mis19A.Three = Convert.ToDecimal(item["Three"].ToString());
                mis19A.FourA = Convert.ToDecimal(item["FourA"].ToString());
                mis19A.FourB = Convert.ToDecimal(item["FourB"].ToString());
                mis19A.FourC = Convert.ToDecimal(item["FourC"].ToString());
                mis19A.FourD = Convert.ToDecimal(item["FourD"].ToString());
                mis19A.Five = Convert.ToDecimal(item["Five"].ToString());
                mis19A.Six = Convert.ToDecimal(item["Six"].ToString());
                mis19A.SevenA = Convert.ToDecimal(item["SevenA"].ToString());
                mis19A.SevenB = Convert.ToDecimal(item["SevenB"].ToString());
                mis19A.SevenC = Convert.ToDecimal(item["SevenC"].ToString());
                mis19A.SevenD = Convert.ToDecimal(item["SevenD"].ToString());
                mis19A.EightA = Convert.ToDecimal(item["EightA"].ToString());
                mis19A.EightB = Convert.ToDecimal(item["EightB"].ToString());
                mis19A.EightC = Convert.ToDecimal(item["EightC"].ToString());
                mis19A.EightD = Convert.ToDecimal(item["EightD"].ToString());
                mis19A.Nine = Convert.ToDecimal(item["Nine"].ToString());
                mis19A.Ten = Convert.ToDecimal(item["Ten"].ToString());
                mis19A.Eleven = Convert.ToDecimal(item["Eleven"].ToString());
                mis19A.Twelve = Convert.ToDecimal(item["Twelve"].ToString());
                mis19A.Thirteen = Convert.ToDecimal(item["Thirteen"].ToString());
                mis19A.Fourteen = Convert.ToDecimal(item["Fourteen"].ToString());
                mis19A.Fifteen = Convert.ToDecimal(item["Fifteen"].ToString());
                mis19A.Sixteen = Convert.ToDecimal(item["Sixteen"].ToString());
                mis19A.Seventeen = Convert.ToDecimal(item["Seventeen"].ToString());
                mis19A.Eighteen = "";//  item["Eighteen"].ToString( );
                mis19s.Add(mis19A);

            }
            #endregion 1
            #region 2
            foreach (DataRow item in ReportResult2.Rows)
            {
                mis19B = new MIS19VM();
                mis19B.One = item["One"].ToString();
                mis19B.Two = MonthB;// item["Two"].ToString();
                mis19B.Three = Convert.ToDecimal(item["Three"].ToString());
                mis19B.FourA = Convert.ToDecimal(item["FourA"].ToString());
                mis19B.FourB = Convert.ToDecimal(item["FourB"].ToString());
                mis19B.FourC = Convert.ToDecimal(item["FourC"].ToString());
                mis19B.FourD = Convert.ToDecimal(item["FourD"].ToString());
                mis19B.Five = Convert.ToDecimal(item["Five"].ToString());
                mis19B.Six = Convert.ToDecimal(item["Six"].ToString());
                mis19B.SevenA = Convert.ToDecimal(item["SevenA"].ToString());
                mis19B.SevenB = Convert.ToDecimal(item["SevenB"].ToString());
                mis19B.SevenC = Convert.ToDecimal(item["SevenC"].ToString());
                mis19B.SevenD = Convert.ToDecimal(item["SevenD"].ToString());
                mis19B.EightA = Convert.ToDecimal(item["EightA"].ToString());
                mis19B.EightB = Convert.ToDecimal(item["EightB"].ToString());
                mis19B.EightC = Convert.ToDecimal(item["EightC"].ToString());
                mis19B.EightD = Convert.ToDecimal(item["EightD"].ToString());
                mis19B.Nine = Convert.ToDecimal(item["Nine"].ToString());
                mis19B.Ten = Convert.ToDecimal(item["Ten"].ToString());
                mis19B.Eleven = Convert.ToDecimal(item["Eleven"].ToString());
                mis19B.Twelve = Convert.ToDecimal(item["Twelve"].ToString());
                mis19B.Thirteen = Convert.ToDecimal(item["Thirteen"].ToString());
                mis19B.Fourteen = Convert.ToDecimal(item["Fourteen"].ToString());
                mis19B.Fifteen = Convert.ToDecimal(item["Fifteen"].ToString());
                mis19B.Sixteen = Convert.ToDecimal(item["Sixteen"].ToString());
                mis19B.Seventeen = Convert.ToDecimal(item["Seventeen"].ToString());
                mis19B.Eighteen = "";// item["Eighteen"].ToString();
                mis19s.Add(mis19B);

            }
                
            #endregion 2

            #region Info
            int TMonthA = 0;
            int TMonthB = 0;
            string Info1 = "";
            string Info2 = "";
            string Info3 = "";
            string Info4 = "";
            string Info5 = "";
            string Info6 = "";
            string Info7 = "";
            string Info8 = "";
            string Info9 = "";
            string Info10 = "";
            string Info11 = "";
            string Info12 = "";
            decimal Result = 0;

            TMonthA = (dtpEnd1.Value.Year - dtpFrom1.Value.Year) * 12 + dtpEnd1.Value.Month - dtpFrom1.Value.Month+1;
            TMonthB = (dtpEnd2.Value.Year - dtpFrom2.Value.Year) * 12 + dtpEnd2.Value.Month - dtpFrom2.Value.Month+1;
            Result = 0;
            if ( mis19A.SevenD>0)

                Result = (mis19B.SevenD - mis19A.SevenD) / mis19A.SevenD * 100;
             
            Info1 = Result.ToString("#.##")+" %";

            Result = 0;
            if (mis19A.Ten > 0)
                Result = (mis19B.Ten - mis19A.Ten) / mis19A.Ten * 100;
            Info2 = Result.ToString("#.##") + " %";

            Result = 0;
            if (mis19A.Twelve > 0)
                Result = (mis19B.Twelve - mis19A.Twelve) / mis19A.Twelve * 100;
            Info3 = Result.ToString("#.##") + " %";

            Result = 0;
            if ((mis19A.Eleven - mis19A.Fifteen) > 0)
                Result = ((mis19B.Eleven - mis19B.Fifteen) - (mis19A.Eleven - mis19A.Fifteen)) / (mis19A.Eleven - mis19A.Fifteen) * 100;
            Info4 = Result.ToString("#.##") + " %";

            Result = 0;
            if (mis19A.Fourteen > 0)
                Result = (mis19B.Fourteen - mis19A.Fourteen) / mis19A.Fourteen * 100;
            Info5 = Result.ToString("#.##") + " %";

            Result = 0;
            if (mis19A.SevenD > 0)
                Result = mis19A.Twelve / mis19A.SevenD * 100;
            Info6 = Result.ToString("#.##") + " %";

            Result = 0;
            if (mis19B.SevenD > 0)
                Result = mis19B.Twelve / mis19B.SevenD * 100;
            Info7 = Result.ToString("#.##") + " %";

            Result = 0;
            if (mis19B.SevenD > 0)
                Info8 = Result.ToString("#.##");

            Result = 0;
            Result = (mis19B.SevenD - mis19B.Twelve);
            Info9 = Result.ToString("#.##");

            Result = 0;
            if (TMonthB > 0)
                Result = (mis19B.SevenD - mis19B.Twelve) / TMonthB;
            Info10 = Result.ToString("#.##");

            Result = 0;
            if (TMonthB > 0)
                Result = mis19B.FourD / TMonthB;
            Info11 = Result.ToString("#.##");

            Result = 0;
            if (mis19A.Seventeen > 0)
                Result = (mis19B.Seventeen - mis19A.Seventeen) / mis19A.Seventeen * 100;
            Info12 = Result.ToString("#.##") + " %";

            #endregion Info

            ReportClass objrpt = new ReportClass();


            objrpt = new MIS19();


            objrpt.SetDataSource(mis19s);
            objrpt.DataDefinition.FormulaFields["Info1"].Text = "'" + Info1 + "'";
            objrpt.DataDefinition.FormulaFields["Info2"].Text = "'" + Info2 + "'";
            objrpt.DataDefinition.FormulaFields["Info3"].Text = "'" + Info3 + "'";
            objrpt.DataDefinition.FormulaFields["Info4"].Text = "'" + Info4 + "'";
            objrpt.DataDefinition.FormulaFields["Info5"].Text = "'" + Info5 + "'";
            objrpt.DataDefinition.FormulaFields["Info6"].Text = "'" + Info6 + "'";
            objrpt.DataDefinition.FormulaFields["Info7"].Text = "'" + Info7 + "'";
            objrpt.DataDefinition.FormulaFields["Info8"].Text = "'" + Info8 + "'";
            objrpt.DataDefinition.FormulaFields["Info9"].Text = "'" + Info9 + "'";
            objrpt.DataDefinition.FormulaFields["Info10"].Text = "'" + Info10 + "'";
            objrpt.DataDefinition.FormulaFields["Info11"].Text = "'" + Info11 + "'";
            objrpt.DataDefinition.FormulaFields["Info12"].Text = "'" + Info12 + "'";
            objrpt.DataDefinition.FormulaFields["MonthA"].Text = "'" + MonthA + "'";
            objrpt.DataDefinition.FormulaFields["MonthB"].Text = "'" + MonthB + "'";
            objrpt.DataDefinition.FormulaFields["TMonthA"].Text = "'= " + TMonthA.ToString()+" Month(s)" + "'";
            objrpt.DataDefinition.FormulaFields["TMonthB"].Text = "'= " + TMonthB.ToString() + " Month(s)" + "'";


            //objrpt.DataDefinition.FormulaFields["Preview"].Text = "''";

            ////objrpt.DataDefinition.FormulaFields["HSCode"].Text = "'" + HSCode + "'";
            //objrpt.DataDefinition.FormulaFields["ProductName"].Text = "'" + ProductName + "'";
            objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + Program.CompanyName + "'";
            //objrpt.DataDefinition.FormulaFields["CompanyLegalName"].Text = "'" + Program.CompanyLegalName + "'";
            objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + Program.Address1 + "'";
            //objrpt.DataDefinition.FormulaFields["Address2"].Text = "'" + Program.Address2 + "'";
            //objrpt.DataDefinition.FormulaFields["Address3"].Text = "'" + Program.Address3 + "'";
            objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + Program.TelephoneNo + "'";
            //objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + Program.FaxNo + "'";
            //objrpt.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'" + Program.VatRegistrationNo + "'";

            FormulaFieldDefinitions crFormulaF;
            crFormulaF = objrpt.DataDefinition.FormulaFields;
            CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
            _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", Program.FontSize);

            FormReport reports = new FormReport();
            //objrpt.DataDefinition.FormulaFields["Trial"].Text = "'" + Program.Trial + "'";
            reports.crystalReportViewer1.Refresh();
            reports.setReportSource(objrpt);
            if (Convert.ToDecimal(LicenseExpite.ExpiteDate(DatabaseInfoVM.DatabaseName)) >= Convert.ToDecimal(_dbsqlConnection.ServerDateTime())) 
                //reports.ShowDialog();
                reports.WindowState = FormWindowState.Maximized;
            reports.Show();
            // End Complete
            #endregion preview
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                ////throw ex;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            dtpEnd1.Value = DateTime.Now;
            dtpEnd2.Value = DateTime.Now;
            dtpFrom1.Value = DateTime.Now;
            dtpFrom2.Value = DateTime.Now;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FormMIS19_Load(object sender, EventArgs e)
        {

        }
    }
}
