using CrystalDecisions.CrystalReports.Engine;
using SymphonySofttech.Reports;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VATServer.Library;
using VATServer.Ordinary;
using VATViewModel.DTOs;

namespace VATClient.Integration.DBH
{
    public partial class FormSaleImportDBH_AutoPrint : Form
    {
        #region Variables

        string[] sqlResults = new string[6];

        private bool isProcessRunning;
        private string _saleRow = "";
        public string transactionType;
        DataTable dtTableResult = new DataTable();
        DataTable dtBranchInfo = new DataTable();
        private DataTable salesData;
        private ReportDocument reportDocument = null;
        CommonDAL commonDal = new CommonDAL();
        private int PrintCopy = 1;
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        #endregion

        public FormSaleImportDBH_AutoPrint()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();
        }

        public static string SelectOne(string transactionType, bool cdn)
        {
            FormSaleImportDBH_AutoPrint form = new FormSaleImportDBH_AutoPrint();

            form.transactionType = transactionType;
            form.Show();

            return form._saleRow;
        }

        private void btnTimerStart_Click(object sender, EventArgs e)
        {
            if (!timer1.Enabled)
            {
                isProcessRunning = false;
                timer1.Start();

            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!isProcessRunning)
            {

                ProcessData();

            }

        }

        private void ProcessData()
        {
            try
            {
                isProcessRunning = true;

                AddlistItem("Checking Data");

                var dal = new BranchProfileDAL();

                dtBranchInfo = dal.SelectAll(Program.BranchId.ToString(), null, null, null, null, true,connVM);

                progressBar1.Visible = true;

                ////string IsServer = ConfigurationManager.AppSettings["IsServer"];
                CommonDAL commonDal = new CommonDAL();
                DataSet CompanyDs = new DataSet();
                CompanyDs = commonDal.SettingInformation();

                string IsServer = "N";

                try
                {
                    CompanyDs = commonDal.SettingInformation();

                    if (CompanyDs != null && CompanyDs.Tables[0].Rows.Count > 0)
                    {
                        IsServer = CompanyDs.Tables[0].Rows[0]["IsServer"].ToString();
                    }
                    
                }
                catch (Exception ex)
                {
                    IsServer = "N";
                }

                if (IsServer == "Y")
                {
                    #region Data Save

                    bgwDataSaveAndProcess.RunWorkerAsync();

                    #endregion
                }
                else
                {
                    #region Auto print

                    bgwBigData.RunWorkerAsync();

                    #endregion
                }

                #region Data Save

                ////bgwDataSaveAndProcess.RunWorkerAsync();

                #endregion

                #region Auto print

                ////bgwBigData.RunWorkerAsync();

                #endregion


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }

        private void btnTimerStop_Click(object sender, EventArgs e)
        {
            if (timer1.Enabled)
            {
                timer1.Stop();
            }
        }

        private void AddlistItem(string ItemText, string ItemValue = "")
        {
            //chkMsgList.DisplayMember = "Text";
            //chkMsgList.ValueMember = "Value";

            listBox1.Items.Add(ItemText);

            ////chkMsgList.Items.Insert(0, new { Text = "Find " + rowCount + " Invoice", Value = "2" });

        }


        private void bgwBigData_DoWork(object sender, DoWorkEventArgs e)
        {
            #region try
            
            try
            {
                //throw new Exception("These Invoices are already in system-");

                ImportDAL importDal = new ImportDAL();
                SaleDAL salesDal = new SaleDAL();

                reportDocument = null;
                salesData = null;
                dtTableResult = null;

                #region Select and print

                #region Select All Data

                if (InvokeRequired) Invoke((MethodInvoker)delegate
                {
                    AddlistItem("Data Searching for Print");
                });

                if (InvokeRequired) Invoke((MethodInvoker)delegate
                {
                    int visibleItems = listBox1.ClientSize.Height / listBox1.ItemHeight;
                    listBox1.TopIndex = Math.Max(listBox1.Items.Count - visibleItems + 1, 0);

                });

                UserInformationDAL uDal = new UserInformationDAL();
                UserInformationVM uVm = uDal.SelectAll(Convert.ToInt32(Program.CurrentUserID)).FirstOrDefault();

                string[] cFields = { "sih.IsPrint", "sih.BranchId", "sih.CreatedBy" };
                string[] cValues = { "N", Program.BranchId.ToString(), uVm.FullName };

                salesData = salesDal.SelectAll(0, cFields, cValues, null, null, null, true, transactionType, "N");

                #endregion

                #region get printer Name

                string printerName = commonDal.settingsDesktop("Printer", "Defaultprinter");

                #endregion

                #region Invoice Count Message

                if (InvokeRequired) Invoke((MethodInvoker)delegate
                {
                    AddlistItem("Find " + salesData.Rows.Count.ToString() + " Invoice for Print");
                });

                if (InvokeRequired) Invoke((MethodInvoker)delegate
                {
                    int visibleItems = listBox1.ClientSize.Height / listBox1.ItemHeight;
                    listBox1.TopIndex = Math.Max(listBox1.Items.Count - visibleItems + 1, 0);

                });

                #endregion

                #region Invoice Print by Loop

                if (salesData != null && salesData.Rows.Count > 0)
                {
                    foreach (DataRow row in salesData.Rows)
                    {
                        reportDocument = null;

                        string InvoiceNo = row["SalesInvoiceNo"].ToString();

                        Report_VAT6_3_Completed(printerName, InvoiceNo);

                        if (InvokeRequired) Invoke((MethodInvoker)delegate
                        {
                            int visibleItems = listBox1.ClientSize.Height / listBox1.ItemHeight;
                            listBox1.TopIndex = Math.Max(listBox1.Items.Count - visibleItems + 1, 0);

                        });

                        ReportPrint(printerName);

                    }

                }

                #endregion

                #endregion

            }

            #endregion

            #region catch
            
            catch (Exception exception)
            {
                if (InvokeRequired) Invoke((MethodInvoker)delegate
                {
                    AddlistItem(exception.Message);
                });

                if (InvokeRequired) Invoke((MethodInvoker)delegate
                {
                    int visibleItems = listBox1.ClientSize.Height / listBox1.ItemHeight;
                    listBox1.TopIndex = Math.Max(listBox1.Items.Count - visibleItems + 1, 0);

                });

                FileLogger.Log("SaleImportDBH_AutoPrint", "bgwBigData_DoWork", exception.Message);
            }

            #endregion

        }

        private void bgwBigData_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            #region try

            try
            {
                if (InvokeRequired) Invoke((MethodInvoker)delegate
                {
                    int visibleItems = listBox1.ClientSize.Height / listBox1.ItemHeight;
                    listBox1.TopIndex = Math.Max(listBox1.Items.Count - visibleItems + 1, 0);

                });


            }

            #endregion

            #region catch

            catch (Exception exception)
            {
                if (InvokeRequired) Invoke((MethodInvoker)delegate
                {
                    AddlistItem(exception.Message);
                });

                if (InvokeRequired) Invoke((MethodInvoker)delegate
                {
                    int visibleItems = listBox1.ClientSize.Height / listBox1.ItemHeight;
                    listBox1.TopIndex = Math.Max(listBox1.Items.Count - visibleItems + 1, 0);

                });

                //////MessageBox.Show(exception.Message);
                FileLogger.Log("SaleImportDBH_AutoPrint", "bgwBigData_RunWorkerCompleted", exception.Message);

            }

            #endregion

            #region finally

            finally
            {
                isProcessRunning = false;
                progressBar1.Visible = false;

            }
            #endregion

        }

        private void BulkCallBack()
        {
            //if (InvokeRequired)
            //    Invoke(new MethodInvoker(() => { progressBar1.Value = 1; }));

        }

        public ReportDocument Report_VAT6_3_Completed(string PrinterName = "", string SalesInvoiceNo = "")
        {
            try
            {
                SaleReport _report = new SaleReport();

                ////AddlistItem("VAT 6.3 # " + SalesInvoiceNo + " is printing on " + PrinterName + "");

                if (InvokeRequired) Invoke((MethodInvoker)delegate
                    {
                        AddlistItem("VAT 6.3 # " + SalesInvoiceNo + " is printing on " + PrinterName + "");
                    });

                reportDocument = _report.Report_VAT6_3_Completed(SalesInvoiceNo, transactionType, false, false, false, false, false, false
                    , false, 1, 0, false, false, false, false, false, false,connVM);
            }
            catch (Exception ex)
            {
                reportDocument = new ReportDocument();
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                if (InvokeRequired) Invoke((MethodInvoker)delegate
                    {
                        AddlistItem(ex.Message);
                    });
                //////MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "", exMessage);
            }
            return reportDocument;


        }

        public void ReportPrint(string VPrinterName = "")
        {
            try
            {
                if (reportDocument == null)
                {
                    MessageBox.Show("There is no data to Print", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                PrintCopy = 1;

                for (int i = 1; i <= PrintCopy; i++)
                {
                    PrinterSettings printerSettings =
                        new PrinterSettings();

                    printerSettings.PrinterName = VPrinterName.Trim();

                    reportDocument.PrintToPrinter(printerSettings, new PageSettings(), false);

                }

            }

            #region catch

            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                if (InvokeRequired) Invoke((MethodInvoker)delegate
                {
                    AddlistItem(ex.Message);
                });

                if (InvokeRequired) Invoke((MethodInvoker)delegate
                {
                    int visibleItems = listBox1.ClientSize.Height / listBox1.ItemHeight;
                    listBox1.TopIndex = Math.Max(listBox1.Items.Count - visibleItems + 1, 0);

                });

                //////MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ReportShow", ex.ToString());
            }
            finally
            {
                if (reportDocument != null)
                {
                    reportDocument.Close();
                    reportDocument.Dispose();
                }

            }

            #endregion




        }

        private void UpdateOtherDB(DataTable salesData, string transactionType = "Other")
        {
            if (sqlResults[0].ToLower() == "success")
            {
                try
                {
                    ImportDAL importDal = new ImportDAL();

                    DataView view = new DataView(salesData);

                    salesData = view.ToTable(true, "ID");

                    string[] results = new string[5];

                    results = importDal.UpdateDHLSales(salesData, settingVM.BranchInfoDT,connVM);

                    if (results[0].ToLower() != "success")
                    {
                        string message = "These Id failed to insert to other database\n";

                        foreach (DataRow row in salesData.Rows)
                        {
                            message += row["ID"] + "\n" + " , ";
                        }

                        if (InvokeRequired) Invoke((MethodInvoker)delegate
                            {
                                AddlistItem(message + "\n" + "Staging table may not be updated");
                            });

                        if (InvokeRequired) Invoke((MethodInvoker)delegate
                        {
                            AddlistItem(message);
                        });

                        FileLogger.Log("DHL", "UpdateOtherDB", message);

                    }
                }
                catch (Exception e)
                {
                    string message = "These Id failed to insert to other database\n";

                    foreach (DataRow row in salesData.Rows)
                    {
                        message += row["ID"] + "\n" + " , ";
                    }

                    if (InvokeRequired) Invoke((MethodInvoker)delegate
                    {
                        AddlistItem(message + "\n" + "Staging table may not be updated");
                    });

                    FileLogger.Log("DHL", "UpdateOtherDB", message);

                    //MessageBox.Show(e.Message + "\n" + "Staging table may not be updated");
                }
            }
        }


        private void TableValidation(DataTable salesData)
        {
            if (!salesData.Columns.Contains("Branch_Code"))
            {
                var columnName = new DataColumn("Branch_Code") { DefaultValue = Program.BranchCode };
                salesData.Columns.Add(columnName);
            }

            var column = new DataColumn("SL") { DefaultValue = "" };
            ////var CreatedBy = new DataColumn("CreatedBy") { DefaultValue = Program.CurrentUser };
            var ReturnId = new DataColumn("ReturnId") { DefaultValue = 0 };
            var BOMId = new DataColumn("BOMId") { DefaultValue = 0 };
            var TransactionType = new DataColumn("TransactionType") { DefaultValue = transactionType };
            var CreatedOn = new DataColumn("CreatedOn") { DefaultValue = DateTime.Now.ToString() };
            DataColumn userId = new DataColumn("UserId") { DefaultValue = Program.CurrentUserID };

            salesData.Columns.Add(column);
            //salesData.Columns.Add(CreatedBy);
            salesData.Columns.Add(CreatedOn);
            salesData.Columns.Add(userId);

            if (!salesData.Columns.Contains("ReturnId"))
            {
                salesData.Columns.Add(ReturnId);
            }

            salesData.Columns.Add(BOMId);
            salesData.Columns.Add(TransactionType);
        }

        private void FormSaleImportDHLAirport_Load(object sender, EventArgs e)
        {
            if (listBox1.Items.Count != 0)
            {
                listBox1.Items.Clear();
            }
        }

        private void FormSaleImportDHLAirport_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (timer1.Enabled)
            {
                timer1.Stop();
            }
        }

        private void FormSaleImportDHLAirport_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (timer1.Enabled)
            {
                timer1.Stop();
            }
        }

        private void bgwDataSaveAndProcess_DoWork(object sender, DoWorkEventArgs e)
        {
            #region try

            try
            {
                //throw new Exception("These Invoices are already in system-");

                ImportDAL importDal = new ImportDAL();
                SaleDAL salesDal = new SaleDAL();

                reportDocument = null;
                salesData = null;
                dtTableResult = null;
                string[] ids = new string[] { };

                #region Select And Save

                #region Update User Data oracle

                ResultVM rVm = importDal.UserUpdateDBHTable(Program.BranchId.ToString());
                

                #endregion

                #region Get Sale DBH DbData

                UserInformationDAL uDal = new UserInformationDAL();
                UserInformationVM uVm = uDal.SelectAll(Convert.ToInt32(Program.CurrentUserID), null, null, null, null, connVM).FirstOrDefault();

                dtTableResult = importDal.GetSaleDBHDbData(ids, dtBranchInfo, "", "", transactionType, true);//, uVm.FullName

                if (InvokeRequired) Invoke((MethodInvoker)delegate { AddlistItem("Find " + dtTableResult.Rows.Count.ToString() + " Invoice"); });

                if (InvokeRequired) Invoke((MethodInvoker)delegate
                {
                    int visibleItems = listBox1.ClientSize.Height / listBox1.ItemHeight;
                    listBox1.TopIndex = Math.Max(listBox1.Items.Count - visibleItems + 1, 0);

                });


                #endregion

                if (dtTableResult != null && dtTableResult.Rows.Count != 0)
                {
                    #region TableValidation and date Save

                    if (InvokeRequired) Invoke((MethodInvoker)delegate { AddlistItem("Data Saveing in VAT Server"); });

                    TableValidation(dtTableResult);

                    IntegrationParam param = new IntegrationParam();

                    param.InvoiceTime = DateTime.Now.ToString(" HH:mm:ss");

                    sqlResults = salesDal.SaveAndProcess(dtTableResult, () => { }, Program.BranchId, "", connVM, param, null, null, Program.CurrentUserID);

                    if (InvokeRequired) Invoke((MethodInvoker)delegate
                    {
                        int visibleItems = listBox1.ClientSize.Height / listBox1.ItemHeight;
                        listBox1.TopIndex = Math.Max(listBox1.Items.Count - visibleItems + 1, 0);

                    });


                    #endregion

                    if (sqlResults[0].ToLower() == "success")
                    {
                        #region Successfully Message

                        if (InvokeRequired) Invoke((MethodInvoker)delegate { AddlistItem("Data Save Successfully in VAT Server"); });

                        if (InvokeRequired) Invoke((MethodInvoker)delegate
                        {
                            int visibleItems = listBox1.ClientSize.Height / listBox1.ItemHeight;
                            listBox1.TopIndex = Math.Max(listBox1.Items.Count - visibleItems + 1, 0);

                        });

                        #endregion

                    }
                    else
                    {
                        if (InvokeRequired) Invoke((MethodInvoker)delegate { AddlistItem("Data Save Fail in VAT Server"); });

                        if (InvokeRequired) Invoke((MethodInvoker)delegate
                        {
                            int visibleItems = listBox1.ClientSize.Height / listBox1.ItemHeight;
                            listBox1.TopIndex = Math.Max(listBox1.Items.Count - visibleItems + 1, 0);

                        });
                    }

                    #region Update OtherDB

                    if (sqlResults[0].ToLower() == "success")
                    {
                        ResultVM resultVm = importDal.UpdateDBHTable(Program.CurrentUserID, Program.BranchId.ToString());
                        sqlResults = resultVm.GetResultArray();

                        ////UpdateOtherDB(dtTableResult, transactionType);
                    }

                    #endregion

                }

                #endregion

            }

            #endregion

            #region catch

            catch (Exception exception)
            {
                if (InvokeRequired) Invoke((MethodInvoker)delegate
                {
                    AddlistItem(exception.Message);
                });

                if (InvokeRequired) Invoke((MethodInvoker)delegate
                {
                    int visibleItems = listBox1.ClientSize.Height / listBox1.ItemHeight;
                    listBox1.TopIndex = Math.Max(listBox1.Items.Count - visibleItems + 1, 0);

                });

                FileLogger.Log("SaleImportDBH_AutoPrint", "bgwDataSaveAndProcess_DoWork", exception.Message);
            }

            #endregion
        }

        private void bgwDataSaveAndProcess_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            #region try

            try
            {
                if (InvokeRequired) Invoke((MethodInvoker)delegate
                {
                    int visibleItems = listBox1.ClientSize.Height / listBox1.ItemHeight;
                    listBox1.TopIndex = Math.Max(listBox1.Items.Count - visibleItems + 1, 0);

                });


            }

            #endregion

            #region catch

            catch (Exception exception)
            {
                if (InvokeRequired) Invoke((MethodInvoker)delegate
                {
                    AddlistItem(exception.Message);
                });

                if (InvokeRequired) Invoke((MethodInvoker)delegate
                {
                    int visibleItems = listBox1.ClientSize.Height / listBox1.ItemHeight;
                    listBox1.TopIndex = Math.Max(listBox1.Items.Count - visibleItems + 1, 0);

                });

                //////MessageBox.Show(exception.Message);
                FileLogger.Log("SaleImportDBH_AutoPrint", "bgwDataSaveAndProcess_RunWorkerCompleted", exception.Message);

            }

            #endregion

            #region finally

            finally
            {
                isProcessRunning = false;
                progressBar1.Visible = false;

            }

            #endregion

        }

    }
}
