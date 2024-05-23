using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Web.Services.Protocols;
using System.Windows.Forms;
using OfficeOpenXml;
using VATServer.Library;
using VATViewModel.DTOs;

namespace VATClient
{
    public partial class FormUpdatePurchaseDuty : Form
    {
        public FormUpdatePurchaseDuty()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();
        }
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        private bool ChangeData = false;
        public string VFIN = "1111";
        private const string LineDelimeter = DBConstant.LineDelimeter;
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private static string PassPhrase = DBConstant.PassPhrase;
        private static string EnKey = DBConstant.EnKey;
        private static bool NotNumeric = false;
        private static bool empty = false;
        private static bool NotDate = false;
        private static bool NotActiveCharacter = false;
        public OleDbConnection theConnection;

        //List<UOMVM> uoms = new List<UOMVM>();

        private string Productresult = string.Empty;
        private string PurchaseFromDate, PurchaseToDate;


        private string[] sqlResults;
        private bool SAVE_DOWORK_SUCCESS = false;
        private bool DELETE_DOWORK_SUCCESS = false;
        private bool UPDATE_DOWORK_SUCCESS = false;
        private bool POST_DOWORK_SUCCESS = false;

        //System.Data.DataTable dtTrack = new System.Data.DataTable();
        private string IsTracking = string.Empty;
        System.Data.DataTable dt = new System.Data.DataTable();
        List<PurchaseDutiesVM> purchaseDuties = new List<PurchaseDutiesVM>();
        private void btnSearchInvoiceNo_Click(object sender, EventArgs e)
        {
            try
            {

                #region Statement
                if (dtpPurchaseFromDate.Checked == false)
                {
                    PurchaseFromDate = "";
                }
                else
                {
                    PurchaseFromDate = dtpPurchaseFromDate.Value.ToString("yyyy-MMM-dd");
                }
                if (dtpPurchaseToDate.Checked == false)
                {
                    PurchaseToDate = "";
                }
                else
                {
                    //PurchaseToDate = dtpPurchaseToDate.Value.ToString("yyyy-MMM-dd");
                    PurchaseToDate = dtpPurchaseToDate.Value.AddDays(1).ToString("yyyy-MMM-dd");
                }

                Program.fromOpen = "Me";
                #region TransactionType
                //DataGridViewRow selectedRow = null;// FormIssueSearch.SelectOne();
                //selectedRow = FormPurchaseSearch.SelectOne("All");

                #endregion TransactionType

                //if (selectedRow != null && selectedRow.Selected == true)
                //{
                //    txtPurchaseInvoiceNo.Text = selectedRow.Cells["PurchaseInvoiceNo"].Value.ToString();
                PurchaseDAL pdal = new PurchaseDAL();
                dt = pdal.SearchPurchaseDutyDTDownload(txtPurchaseInvoiceNo.Text.Trim(), PurchaseFromDate, PurchaseToDate,connVM);
                dataGridView1.DataSource = dt;
                //}

                #endregion

            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "btnSearchInvoiceNo_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnSearchInvoiceNo_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnSearchInvoiceNo_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "btnSearchInvoiceNo_Click", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnSearchInvoiceNo_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnSearchInvoiceNo_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnSearchInvoiceNo_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.StackTrace;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnSearchInvoiceNo_Click", exMessage);
            }
            #endregion
            finally
            {
                ChangeData = false;

            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            sqlResults = new string[4];
            #region Import Details
            PurchaseDAL purDal = new PurchaseDAL();
            sqlResults = purDal.PurchaseUpdateDuty(dt,connVM);
            if (sqlResults.Length > 0)
            {
                string result = sqlResults[0];
                string message = sqlResults[1];

                if (string.IsNullOrEmpty(result))
                {
                    throw new ArgumentNullException("backgroundWorkerAdd_RunWorkerCompleted",
                                                    "Unexpected error.");
                }
                else if (result == "Success" || result == "Fail")
                {
                    MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

            }
            //if (dt.Rows.Count > 0)
            //{
            //    string importID = txtPurchaseInvoiceNo.Text.Trim();

            //    //DataRow[] ImportRaws;//= new DataRow[];//
            //    //if (!string.IsNullOrEmpty(importID))
            //    //{
            //    //    ImportRaws = dt.Select("ID='" + importID + "'");
            //    //}
            //    //else
            //    //{
            //    //    ImportRaws = null;
            //    //}

            //    //if (ImportRaws != null && ImportRaws.Length > 0)
            //    //{
            //        purchaseDuties = purDal.PurchaseUpdateDuty(ImportRaws, null, null);

            //    //}
            //}

            #endregion Import Details
        }
        private void BrowsFile()
        {
            #region try

            try
            {
                OpenFileDialog fdlg = new OpenFileDialog();
                fdlg.Title = "VAT Import Open File Dialog";
                fdlg.InitialDirectory = @"c:\";
                fdlg.Filter = "Excel files (*.xlsx*)|*.xlsx*|Excel(97-2003)files (*.xls*)|*.xls*";
                fdlg.FilterIndex = 2;
                fdlg.RestoreDirectory = true;
                if (fdlg.ShowDialog() == DialogResult.OK)
                {
                    Program.ImportFileName = fdlg.FileName;
                }
            }
            #endregion

            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "BrowsFile", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "BrowsFile", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "BrowsFile", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "BrowsFile", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "BrowsFile", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "BrowsFile", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "BrowsFile", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "BrowsFile", exMessage);
            }

            #endregion
        }

        private void button2_Click(object sender, EventArgs e)
        {
            #region try

            try
            {
                progressBar1.Visible = true;
                NotNumeric = false;
                empty = false;
                NotDate = false;
                NotActiveCharacter = false;
                if (chkSame.Checked == false)
                {
                    BrowsFile();
                }
                string fileName = Program.ImportFileName;
                if (string.IsNullOrEmpty(fileName))
                {
                    MessageBox.Show("Please select the right file for import");
                    return;
                }
                ImportFromExcelFilePurchaseDuties();



                if (dataGridView1.Rows.Count > 0)
                {
                    //chkSame.Checked = true;
                }
            }
            #endregion

            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "btnImport_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnImport_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnImport_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "btnImport_Click", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnImport_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnImport_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnImport_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnImport_Click", exMessage);
            }

            #endregion
            finally
            {
                progressBar1.Visible = false;

                lbCount.Text = "Total Record(s): " + dataGridView1.Rows.Count.ToString();

            }
        }
        private void ImportFromExcelFilePurchaseDuties()
        {
            #region try

            try
            {
                string fileName = Program.ImportFileName;

                string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;" +
                                          "Data Source=" + fileName + ";" + "Extended Properties=" + "\"" +
                                          "Excel 12.0;HDR=YES;" + "\"";
                OleDbConnection theConnection = new OleDbConnection(connectionString);
                theConnection.Open();
                OleDbDataAdapter theDataAdapter = new OleDbDataAdapter("SELECT * FROM [PurchaseI$]", theConnection);
                DataSet theDS = new DataSet();
                dt = new System.Data.DataTable();
                theDataAdapter.Fill(dt);
                theConnection.Close();
                CommonImportDAL cImport = new CommonImportDAL();
                ProductDAL pDal = new ProductDAL();
                PurchaseDAL purDal = new PurchaseDAL();

                //for (int i = 0; i < dt.Rows.Count; i++)
                //{
                //    #region FindItemId
                //    if (string.IsNullOrEmpty(dt.Rows[i]["Item_Code"].ToString().Trim()))
                //    {
                //        throw new ArgumentNullException("Please insert value in 'Item_Code' field.");
                //    }
                //    string ItemNo = cImport.FindItemId(dt.Rows[i]["Item_Name"].ToString().Trim()
                //                                 , dt.Rows[i]["Item_Code"].ToString().Trim(), null, null);
                //    if (string.IsNullOrEmpty(ItemNo))
                //    {
                //        throw new ArgumentNullException("Item Not Found In Database. Item Name: " + dt.Rows[i]["Item_Name"].ToString().Trim()
                //            + "(" + dt.Rows[i]["Item_Code"].ToString().Trim() + ")");

                //    }
                //    else
                //    {

                //        string accvalue = purDal.FindItemIdFromPurchase(dt.Rows[i]["PurchaseInvoiceNo"].ToString().Trim()
                //                                 , ItemNo, null, null);
                //        if (Convert.ToDecimal(accvalue)<=0)
                //        {
                //            throw new ArgumentNullException("This Item Not Found In This Purchase. Item Name: " + dt.Rows[i]["Item_Name"].ToString().Trim()
                //            + "(" + dt.Rows[i]["Item_Code"].ToString().Trim() + ")");
                //        }

                //    }
                //    #endregion FindItemId
                //}
                dataGridView1.DataSource = dt;

            }
            #endregion

            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "CustomerGroupImport", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "CustomerGroupImport", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "CustomerGroupImport", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "CustomerGroupImport", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "CustomerGroupImport", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "CustomerGroupImport", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "CustomerGroupImport", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "CustomerGroupImport", exMessage);
            }

            #endregion
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ExcelPackage excel = new ExcelPackage();
            var workSheet = excel.Workbook.Worksheets.Add("PurchaseI");
            workSheet.Cells["B2:B" + (dt.Rows.Count + 1)].Style.Numberformat.Format = "dd/MMM/yyyy";

            workSheet.Cells[1, 1].LoadFromDataTable(dt, true);

            string p_strPath = Program.AppPath + @"\" + DateTime.Now.ToString("dd_MMM_yyyy_HH_mm_ss") + "_PurchaseDuties_" + txtPurchaseInvoiceNo.Text.Trim().Replace("/", "_") + ".xlsx";
            if (File.Exists(p_strPath))
                File.Delete(p_strPath);

            FileStream objFileStrm = File.Create(p_strPath);
            objFileStrm.Close();

            //Write content to excel file    
            File.WriteAllBytes(p_strPath, excel.GetAsByteArray());
            MessageBox.Show("File Save " + p_strPath + " Successfully");
            ProcessStartInfo psi = new ProcessStartInfo(p_strPath);
            psi.UseShellExecute = true;
            Process.Start(psi);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {

                #region Statement
                if (dtpPurchaseFromDate.Checked == false)
                {
                    PurchaseFromDate = "";
                }
                else
                {
                    PurchaseFromDate = dtpPurchaseFromDate.Value.ToString("yyyy-MMM-dd");
                }
                if (dtpPurchaseToDate.Checked == false)
                {
                    PurchaseToDate = "";
                }
                else
                {
                    //PurchaseToDate = dtpPurchaseToDate.Value.ToString("yyyy-MMM-dd");
                    PurchaseToDate = dtpPurchaseToDate.Value.AddDays(1).ToString("yyyy-MMM-dd");
                }

                Program.fromOpen = "Me";
                #region TransactionType
                DataGridViewRow selectedRow = null;// FormIssueSearch.SelectOne();
                selectedRow = FormPurchaseSearch.SelectOne("All");

                #endregion TransactionType

                if (selectedRow != null && selectedRow.Selected == true)
                {
                    txtPurchaseInvoiceNo.Text = selectedRow.Cells["PurchaseInvoiceNo"].Value.ToString();
                    PurchaseDAL pdal = new PurchaseDAL();
                    dt = pdal.SearchPurchaseDutyDTDownload(txtPurchaseInvoiceNo.Text.Trim(), PurchaseFromDate, PurchaseToDate);
                    dataGridView1.DataSource = dt;
                }

                #endregion

            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "btnSearchInvoiceNo_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnSearchInvoiceNo_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnSearchInvoiceNo_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "btnSearchInvoiceNo_Click", exMessage);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnSearchInvoiceNo_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnSearchInvoiceNo_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnSearchInvoiceNo_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.StackTrace;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnSearchInvoiceNo_Click", exMessage);
            }
            #endregion
            finally
            {
                ChangeData = false;

            }
        }

        private void FormUpdatePurchaseDuty_Load(object sender, EventArgs e)
        {

        }
    }
}
