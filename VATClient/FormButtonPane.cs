using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CrystalDecisions.CrystalReports.Engine;
using OfficeOpenXml;
using SymphonySofttech.Reports.Report;
using VATClient.Integration.NBR;
using VATClient.ReportPreview;
using VATServer.Library;
using VATViewModel.DTOs;
using VATServer.Library.Integration;

namespace VATClient
{
    public partial class FormButtonPane : Form
    {
        public FormButtonPane()
        {
            InitializeComponent();
        }

        private void btnAPi_Click(object sender, EventArgs e)
        {
            try
            {
                FormNBRAPI form = new FormNBRAPI();

                form.Show();
            }
            catch (Exception exception)
            {
                FileLogger.Log("FormNBRAPI", "GETXML", exception.ToString());
                MessageBox.Show(exception.Message);
            }
        }

        private void HSCode_Click(object sender, EventArgs e)
        {
            FormReportSCBL_Production frmHsCode = new FormReportSCBL_Production();
            //FormRptReconsciliation frmHsCode = new FormRptReconsciliation();
            frmHsCode.ShowDialog();
        }

        private void btnException_Click(object sender, EventArgs e)
        {
            try
            {
                throw new Exception();
            }
            catch (Exception exception)
            {
                VATServer.Library.FileLogger.Log("Product Category", "Exception", exception.ToString());
                MessageBox.Show(exception.Message);
            }
        }

        private void btnDisposeRawSale_Click(object sender, EventArgs e)
        {
            try
            {
                FormSale frm = new FormSale();

                frm.rbtnDisposeRaw.Checked = true;
                frm.Show();

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private void btnDisposeFinishSale_Click(object sender, EventArgs e)
        {
            try
            {
                FormSale frm = new FormSale();

                frm.rbtnDisposeFinish.Checked = true;
                frm.Show();

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private void btnDisposeRaw_Click(object sender, EventArgs e)
        {
            FormDisposeRaw frmDisposeRaws = new FormDisposeRaw();
            frmDisposeRaws.Show();
        }

        private void btnRawReceive_Click(object sender, EventArgs e)
        {
            FormPurchase frm = new FormPurchase();
            frm.rbtnClientRawReceive.Checked = true;

            //FormRptReconsciliation frmHsCode = new FormRptReconsciliation();
            frm.ShowDialog();
        }

        private void btnRawIssue_Click(object sender, EventArgs e)
        {
            FormSale frm = new FormSale();
            frm.rbtnContractorRawIssue.Checked = true;

            //FormRptReconsciliation frmHsCode = new FormRptReconsciliation();
            frm.ShowDialog();
        }

        private void btnDisposeFinish_Click(object sender, EventArgs e)
        {
            FormDisposeFinishNew frm = new FormDisposeFinishNew();
            frm.Show();
        }

        private void btnFGReceive_Click(object sender, EventArgs e)
        {
            FormPurchase frm = new FormPurchase();
            frm.rbtnClientFGReceiveWOBOM.Checked = true;

            //FormRptReconsciliation frmHsCode = new FormRptReconsciliation();
            frm.ShowDialog();
        }

        private void btnProcess_Click(object sender, EventArgs e)
        {
            FormRegularProcess process = new FormRegularProcess();
            process.Show();
        }

        private void btnPermanent_Click(object sender, EventArgs e)
        {
            try
            {
                // From frm = new FormProductItemSearch();
                FormPermanentProcess frm = new FormPermanentProcess();

                // frm.rbtnBankChannelPayment.Checked = true;
                frm.Show();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnBankChannel_Click(object sender, EventArgs e)
        {

            try
            {
                FormPurchaseSearch frm = new FormPurchaseSearch();

                frm.rbtnBankChannelPayment.Checked = true;
                frm.Show();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                FormDBMigration frm = new FormDBMigration();
                frm.Show();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnTollSale_Click(object sender, EventArgs e)
        {
            try
            {
                FormSale frm = new FormSale();

                frm.rbtnTollSale.Checked = true;
                frm.Show();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnChargeCodes_Click(object sender, EventArgs e)
        {
            try
            {


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnSetting_Click(object sender, EventArgs e)
        {
            FormSettingMaster frm = new FormSettingMaster();
            frm.rbtnSetting.Checked = true;
            frm.Show();
        }

        private void btnClient63_Click(object sender, EventArgs e)
        {
            FormClient6_3 frm = new FormClient6_3();
            ////frm.rbtnClient63.Checked = true;
            frm.ShowDialog();
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            try
            {
                DataSet ReportResult = new DataSet();

                ReportDSDAL reportDsdal = new ReportDSDAL();
                //IReport productCategoryDal = OrdinaryVATDesktop.GetObject<ReportDSDAL, ReportDSRepo, IReport>(OrdinaryVATDesktop.IsWCF);


                ReportResult = reportDsdal.InputValue7_5percent("2021-01-01 00:00:00", "2022-01-01 23:59:59", Program.BranchId);
                ReportDocument objrpt = new ReportDocument();

                ReportResult.Tables[0].TableName = "dtInputValue7_5";



                objrpt = new RptScblInputValue7_5();

                objrpt.SetDataSource(ReportResult);


                FormulaFieldDefinitions crFormulaF;
                crFormulaF = objrpt.DataDefinition.FormulaFields;
                CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", Program.FontSize);

                objrpt.DataDefinition.FormulaFields["ReportName"].Text = "'Statement of Input Value (7.5%) Change'";
                //objrpt.DataDefinition.FormulaFields["UserName"].Text = "'" + Program.CurrentUser + "'";
                objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + Program.CompanyName + "'";
                objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + Program.Address1 + "'";
                //objrpt.DataDefinition.FormulaFields["Address2"].Text = "'" + Program.Address2 + "'";
                //objrpt.DataDefinition.FormulaFields["Address3"].Text = "'" + Program.Address3 + "'";
                //objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + Program.TelephoneNo + "'";
                //objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + Program.FaxNo + "'";


                FormReport reports = new FormReport();
                reports.crystalReportViewer1.Refresh();

                reports.setReportSource(objrpt);

                //reports.ShowDialog();
                reports.WindowState = FormWindowState.Maximized;
                reports.Show();
            }

            #region catch


            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine +

                                ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerPreview_DoWork",

                               exMessage);
            }

            #endregion
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                FormIntegrationRecon formIntegration = new FormIntegrationRecon();
                formIntegration.Show();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                FormNestleReconsile formIntegration = new FormNestleReconsile();
                formIntegration.Show();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void btnChart_Click(object sender, EventArgs e)
        {
            try
            {
                FormChart formIntegration = new FormChart();
                formIntegration.Show();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void btnYearEndProcess_Click(object sender, EventArgs e)
        {
            try
            {
                FormYearEndProcess formIntegration = new FormYearEndProcess();
                formIntegration.Show();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void btnTraking_Click(object sender, EventArgs e)
        {
            try
            {
                FormTraking formtraking = new FormTraking();
                formtraking.Show();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void btnTollFGWithoutBOM_Click(object sender, EventArgs e)
        {
            FormReceive frm = new FormReceive();
            frm.rbtnTollFinishReceiveWithoutBOM.Checked = true;
            //////frm.MdiParent = this;
            frm.Show();

        }

        private void btnTollitemIssueWithoutBOM_Click(object sender, EventArgs e)
        {
            FormIssue frm = new FormIssue();

            frm.rbtnTollitemIssueWithoutBOM.Checked = true;
            frm.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                FormNBRAPI formNbrapi = new FormNBRAPI();
                formNbrapi.Show();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            CommonDAL COM = new CommonDAL();

            try
            {
                COM.UpdateNestle(settingVM.BranchInfoDT);
                MessageBox.Show("Process Compleate");
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }



        }

        private void btnExcelAppend_Click(object sender, EventArgs e)
        {
            try
            {
                string excelPath = Program.AppPath + @"\Excel Files\_Stock_Report.xlsx";

                if (File.Exists(excelPath))
                    File.Delete(excelPath);

                FileInfo file = new FileInfo(excelPath);

                using (ExcelPackage excel = new ExcelPackage(file))
                {
                    ExcelWorksheet workSheet = excel.Workbook.Worksheets.Add("Stock");
                    excel.Save();
                }


                var dt = new DataTable();
                dt.Columns.Add("Col1");
                dt.Rows.Add(dt.NewRow());

                using (ExcelPackage excel = new ExcelPackage(file))
                {

                    ExcelWorksheet worksheet = excel.Workbook.Worksheets[1];

                    worksheet.Cells[4, 1].LoadFromDataTable(dt, true);

                    excel.Save();
                }


                var dt1 = new DataTable();
                dt1.Columns.Add("Col1");
                dt1.Rows.Add(dt1.NewRow());


                using (ExcelPackage excel = new ExcelPackage(file))
                {

                    ExcelWorksheet worksheet = excel.Workbook.Worksheets[1];

                    worksheet.Cells[10, 1].LoadFromDataTable(dt1, true);

                    excel.Save();
                }


                ProcessStartInfo psi = new ProcessStartInfo(excelPath)
                {
                    UseShellExecute = true
                };
                Process.Start(psi);

            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            FormReport9_1Comp form = new FormReport9_1Comp();
            form.Show();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            CommonDAL COM = new CommonDAL();

            try
            {
                COM.NestleIntermidateDBMigration(settingVM.BranchInfoDT);
                MessageBox.Show("Process Compleate");
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            CommonDAL COM = new CommonDAL();

            try
            {
                DateTime CurrentDate = DateTime.Now;
                DateTime date = DateTime.Now;
                string EndDate = "2023-Jul-31";
                date = Convert.ToDateTime(EndDate);
                if (CurrentDate < date)
                {
                    ResultVM vm = COM.FiscalYear();
                    MessageBox.Show("Process Compleate");
                }

            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void btnIVASUserCreate_Click(object sender, EventArgs e)
        {
            FormUserCreateIVAS frm = new FormUserCreateIVAS();
            frm.Show();

        }

        private void button9_Click(object sender, EventArgs e)
        {

            FormTollProductionConsumption frm = new FormTollProductionConsumption();
            frm.rbtnTollContProductions.Checked = true;
            frm.Show();
        }

        private void button10_Click(object sender, EventArgs e)
        {

            FormTollContInOut frm = new FormTollContInOut();
            frm.rbtnTollCont6_4Outs.Checked = true;
            frm.Show();

        }

        private void button11_Click(object sender, EventArgs e)
        {
            FormTollClientInOuts frm = new FormTollClientInOuts();
            frm.rbtnTollClient6_4Outs.Checked = true;
            frm.Show();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            FormTollProductionConsumption frm = new FormTollProductionConsumption();
            frm.btnTollContConsumptions.Checked = true;
            frm.Show();
        }

        private void button13_Click(object sender, EventArgs e)
        {

            FormTollProductionConsumption frm = new FormTollProductionConsumption();
            frm.rbtnTollClientConsumptions.Checked = true;
            frm.Show();

        }

        private void button14_Click(object sender, EventArgs e)
        {
            FormTollContInOut frm = new FormTollContInOut();
            frm.rbtnTollCont6_4Ins.Checked = true;
            frm.Show();
        }

        private void button15_Click(object sender, EventArgs e)
        {
            FormTollContInOut frm = new FormTollContInOut();
            frm.rbtnTollCont6_4Backs.Checked = true;
            frm.Show();
        }

        private void button16_Click(object sender, EventArgs e)
        {
            FormTollClientInOuts frm = new FormTollClientInOuts();
            frm.rbtnTollClient6_4Backs.Checked = true;
            frm.Show();
        }

        private void button17_Click(object sender, EventArgs e)
        {
            FormTollClientInOuts frm = new FormTollClientInOuts();
            frm.rbtnTollClient6_4Ins.Checked = true;
            frm.Show();
        }

        private void button18_Click(object sender, EventArgs e)
        {
            FormTollClientInOuts frm = new FormTollClientInOuts();
            frm.rbtnTollClient6_4OutsWIP.Checked = true;
            frm.Show();
        }

        private void button19_Click(object sender, EventArgs e)
        {
            FormTollClientInOuts frm = new FormTollClientInOuts();
            frm.rbtnTollClient6_4OutsFG.Checked = true;
            frm.Show();
        }

        private void button20_Click(object sender, EventArgs e)
        {
            FormTollClientInOuts frm = new FormTollClientInOuts();
            frm.rbtnTollClient6_4InsWIP.Checked = true;
            frm.Show();
        }

        private void button21_Click(object sender, EventArgs e)
        {
            FormTollClientInOuts frm = new FormTollClientInOuts();
            frm.rbtnTollClient6_4BacksWIP.Checked = true;
            frm.Show();
        }

        private void button22_Click(object sender, EventArgs e)
        {
            FormTollClientInOuts frm = new FormTollClientInOuts();
            frm.rbtnTollClient6_4BacksFG.Checked = true;
            frm.Show();
        }

        private void button23_Click(object sender, EventArgs e)
        {
            try
            {
                BritishCouncil_IntegrationDAL _dal = new BritishCouncil_IntegrationDAL();
                _dal.SaveCustomerMasterData(true, false);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        private void button24_Click(object sender, EventArgs e)
        {
            FormTollProductionConsumption frm = new FormTollProductionConsumption();
            frm.rbtnTollClientConsumptionsWIP.Checked = true;
            frm.Show();
        }

        private void button25_Click(object sender, EventArgs e)
        {
            FormTollProductionConsumption frm = new FormTollProductionConsumption();
            frm.rbtnTollClientConsumptionsFG.Checked = true;
            frm.Show();
        }
    }
}
