// ---------form //
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Web.Services.Protocols;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
//
//
using System.Security.Cryptography;
using System.IO;
using SymphonySofttech.Utilities;
////
using VATClient.ReportPreview;
using CrystalDecisions.Shared;
using VATClient.ReportPages;
//
using System.Globalization;
using System.Threading;
//
using VATClient.ModelDTO;
using VATServer.Library;
using VATViewModel.DTOs;
using System.Data.OleDb;
using CrystalDecisions.CrystalReports.Engine;
using SymphonySofttech.Reports.Report;
using VATServer.License;
using VATServer.Ordinary;
using VATDesktop.Repo;
using VATServer.Interface;

namespace VATClient
{
    public partial class FormDemand : Form
    {
        #region Constructors

        public FormDemand()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();
            //connVM.SysdataSource = SysDBInfoVM.SysdataSource;
            //connVM.SysPassword = SysDBInfoVM.SysPassword;
            //connVM.SysUserName = SysDBInfoVM.SysUserName;
            //connVM.SysDatabaseName = DatabaseInfoVM.DatabaseName;

        }

        #endregion

        #region Global Variables
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();

        List<BanderolProductMiniDTO> BanderolProductsMini = new List<BanderolProductMiniDTO>();
        private DemandMasterVM demandMasterVM;
        private List<DemandDetailVM> demandDetailVMs = new List<DemandDetailVM>();
        private List<UomDTO> UOMs = new List<UomDTO>();
        private DataTable uomResult;

        private string NextID = string.Empty;
        private bool ChangeData = false;
        private string[] sqlResults;
        private bool SAVE_DOWORK_SUCCESS = false;
        private bool DELETE_DOWORK_SUCCESS = false;
        private bool UPDATE_DOWORK_SUCCESS = false;
        private bool POST_DOWORK_SUCCESS = false;
        string transactionType = string.Empty;

        private bool IsUpdate = false;
        private bool Post = false;
        private bool IsPost = false;
        private int BandeDPlaceQ;

        private DataTable BanderolProducts;

        private DataSet ReportResult;
        private bool Add = false;
        private bool Edit = false;




        

        private const string LineDelimeter = DBConstant.LineDelimeter;
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        //private static string PassPhrase = DBConstant.PassPhrase;
        //private static string EnKey = DBConstant.EnKey;
        //string encriptedIssueHeaderData;
        
        //private DataTable ProductResultDs;
        

        #region Global Variables For BackGroundWorker

        private string DemandHeaderData = string.Empty;

        private string DemandDetailNo = string.Empty;

        private string DemandResult = string.Empty;

        private string DemandResultPost = string.Empty;

        private DataTable DemandDetailResult;

        private string ProductData = string.Empty;

        //private string varItemNo, varCategoryID, varIsRaw, varHSCodeNo, varActiveStatus, varTrading, varNonStock, varProductCode;


        private DataSet StatusResult;

        private DataTable ProductTypeResult;

        #endregion

        #endregion

        private void cmbProduct_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            #region try

            try
            {
                //if (Add == false)
                //{
                //    MessageBox.Show(MessageVM.msgNotAddAccess, this.Text);
                //    return;
                //}
                if (IsPost == true)
                {
                    MessageBox.Show(MessageVM.ThisTransactionWasPosted, this.Text);
                    return;
                }
                if (Program.CheckLicence(dtpDemandDate.Value) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }
                if (IsUpdate == false)
                {
                    NextID = string.Empty;
                }
                else
                {
                    NextID = txtDemandNo.Text.Trim();
                }

                if (txtComments.Text == "")
                {
                    txtComments.Text = "-";
                }

                if (dgvDemand.RowCount <= 0)
                {
                    MessageBox.Show("Please insert Details information for transaction", this.Text, MessageBoxButtons.OK,
                                     MessageBoxIcon.Information);
                    return;
                }

                //TransactionTypes();
                demandMasterVM = new DemandMasterVM();

                demandMasterVM.DemandNo = NextID.ToString();
                demandMasterVM.DemandDateTime = dtpDemandDate.Value.ToString("yyyy-MMM-dd");
                //demandMasterVM.FiscalYear = "2013/2014";
                demandMasterVM.MonthFrom = dtpMonthFrom.Value.ToString("MMM-yyyy");
                demandMasterVM.MonthTo = dtpMonthTo.Value.ToString("MMM-yyyy");
                demandMasterVM.TotalQty = Convert.ToDecimal(txtTotalQty.Text.Trim());
                demandMasterVM.CreatedBy = Program.CurrentUser;
                demandMasterVM.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                demandMasterVM.LastModifiedBy = Program.CurrentUser;
                demandMasterVM.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                demandMasterVM.Post = "N";
                demandMasterVM.TransactionType = "Demand";
                demandMasterVM.BranchId = Program.BranchId;
                demandDetailVMs = new List<DemandDetailVM>();
                for (int i = 0; i < dgvDemand.RowCount; i++)
                {
                    DemandDetailVM detail = new DemandDetailVM();

                    detail.DemandNo = NextID.ToString();
                    detail.DemandLineNo = dgvDemand.Rows[i].Cells["LineNo"].Value.ToString();
                    detail.BandProductId = dgvDemand.Rows[i].Cells["BandProductId"].Value.ToString();
                    detail.ItemNo = dgvDemand.Rows[i].Cells["ItemNo"].Value.ToString();
                    detail.Quantity = Convert.ToDecimal(dgvDemand.Rows[i].Cells["Quantity"].Value.ToString());
                    detail.NBRPrice = Convert.ToDecimal(dgvDemand.Rows[i].Cells["NBRPrice"].Value.ToString());
                    
                    detail.UOM = dgvDemand.Rows[i].Cells["UOM"].Value.ToString();
                    detail.Comments = txtComments.Text;
                    detail.TransactionDate = dtpDemandDate.Value.ToString("yyyy-MMM-dd");
                    detail.BranchId = Program.BranchId;
                    demandDetailVMs.Add(detail);

                }


                if (demandDetailVMs.Count() <= 0)
                {
                    MessageBox.Show("Please insert Details information for transaction", this.Text, MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                    return;
                }
                this.btnSave.Enabled = false;
                this.progressBar1.Visible = true;

                backgroundWorkerSave.RunWorkerAsync();


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
                FileLogger.Log(this.Name, "btnSave_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnSave_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnSave_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnSave_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnSave_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnSave_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnSave_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnSave_Click", exMessage);
            }
            #endregion
        }
        private void backgroundWorkerSave_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                #region Statement
                SAVE_DOWORK_SUCCESS = false;
                sqlResults = new string[4];
                //DemandDAL demandDal = new DemandDAL();
                IDemand demandDal = OrdinaryVATDesktop.GetObject<DemandDAL, DemandRepo, IDemand>(OrdinaryVATDesktop.IsWCF);

                sqlResults = demandDal.DemandInsert(demandMasterVM, demandDetailVMs, null, null,connVM);
                SAVE_DOWORK_SUCCESS = true;

                #endregion

            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                if (error.Count() > 1)
                {
                    MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show(error[0], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

                }
            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerSave_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerSave_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerSave_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "backgroundWorkerSave_DoWork", exMessage);
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
                FileLogger.Log(this.Name, "backgroundWorkerSave_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerSave_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerSave_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message + Environment.NewLine +
                                ex.StackTrace;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerSave_DoWork", exMessage);
            }
            #endregion
        }
        private void backgroundWorkerSave_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                #region Statement
                if (SAVE_DOWORK_SUCCESS)
                    if (sqlResults.Length > 0)
                    {
                        string result = sqlResults[0];
                        string message = sqlResults[1];
                        string newId = sqlResults[2];

                        if (string.IsNullOrEmpty(result))
                        {
                            throw new ArgumentNullException("backgroundWorkerAdd_RunWorkerCompleted",
                                                            "Unexpected error.");
                        }
                        else if (result == "Success" || result == "Fail")
                        {
                            MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            IsUpdate = true;

                            //txtItemNo.Text = newId;
                            ChangeData = false;
                        }

                        if (result == "Success")
                        {
                            txtDemandNo.Text = sqlResults[2].ToString();
                            IsPost = Convert.ToString(sqlResults[3].ToString()) == "Y" ? true : false;
                            for (int i = 0; i < dgvDemand.RowCount; i++)
                            {
                                dgvDemand["Status", dgvDemand.RowCount - 1].Value = "Old";
                            }
                        }
                    }
                ChangeData = false;

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
                FileLogger.Log(this.Name, "backgroundWorkerSave_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerSave_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerSave_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "backgroundWorkerSave_RunWorkerCompleted", exMessage);
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
                FileLogger.Log(this.Name, "backgroundWorkerSave_RunWorkerCompleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "Search", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerSave_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "backgroundWorkerSave_RunWorkerCompleted", exMessage);
            }
            #endregion
            finally
            {
                ChangeData = false;
                this.btnSave.Enabled = true;
                this.progressBar1.Visible = false;
            }

        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            #region Try
            try
            {
                //if (Edit == false)
                //{
                //    MessageBox.Show(MessageVM.msgNotEditAccess, this.Text);
                //    return;
                //}

                if (IsPost == true)
                {
                    MessageBox.Show(MessageVM.ThisTransactionWasPosted, this.Text);
                    return;
                }
                if (Program.CheckLicence(dtpDemandDate.Value) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }
                if (IsUpdate == false)
                {
                    NextID = string.Empty;
                }
                else
                {
                    NextID = txtDemandNo.Text.Trim();
                }
                //TransactionTypes();
                //if (txtComments.Text == "")
                //{
                //    //txtComments.Text = "-";
                //}

                if (dgvDemand.RowCount <= 0)
                {
                    MessageBox.Show("Please insert Details information for transaction", this.Text, MessageBoxButtons.OK,
                                     MessageBoxIcon.Information);
                    return;
                }

                demandMasterVM = new DemandMasterVM();

                demandMasterVM.DemandNo = NextID.ToString();
                demandMasterVM.DemandDateTime = dtpDemandDate.Value.ToString("yyyy-MMM-dd");
                //demandMasterVM.FiscalYear = "2013/2014";
                demandMasterVM.MonthFrom = dtpMonthFrom.Value.ToString("MMM-yyyy");
                demandMasterVM.MonthTo = dtpMonthTo.Value.ToString("MMM-yyyy");
                demandMasterVM.TotalQty = Convert.ToDecimal(txtTotalQty.Text.Trim());
                demandMasterVM.LastModifiedBy = Program.CurrentUser;
                demandMasterVM.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                demandMasterVM.Post = "N";
                demandMasterVM.TransactionType = "Demand";
                demandMasterVM.BranchId = Program.BranchId;

                demandDetailVMs = new List<DemandDetailVM>();
                for (int i = 0; i < dgvDemand.RowCount; i++)
                {
                    DemandDetailVM detail = new DemandDetailVM();

                    detail.DemandNo = NextID.ToString();
                    detail.DemandLineNo = dgvDemand.Rows[i].Cells["LineNo"].Value.ToString();
                    detail.BandProductId = dgvDemand.Rows[i].Cells["BandProductId"].Value.ToString();
                    detail.ItemNo = dgvDemand.Rows[i].Cells["ItemNo"].Value.ToString();
                    detail.Quantity = Convert.ToDecimal(dgvDemand.Rows[i].Cells["Quantity"].Value.ToString());
                    detail.NBRPrice = Convert.ToDecimal(dgvDemand.Rows[i].Cells["NBRPrice"].Value.ToString());
                    
                    detail.UOM = dgvDemand.Rows[i].Cells["UOM"].Value.ToString();
                    //detail.SubTotal = Convert.ToDecimal(dgvDemand.Rows[i].Cells["SubTotal"].Value.ToString());
                    detail.Comments = txtComments.Text;
                    detail.TransactionDate = dtpDemandDate.Value.ToString("yyyy-MMM-dd");
                    detail.BranchId = Program.BranchId;
                    demandDetailVMs.Add(detail);

                }

                if (demandDetailVMs.Count() <= 0)
                {
                    MessageBox.Show("Please insert Details information for transaction", this.Text, MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                    return;
                }
                this.btnUpdate.Enabled = false;
                this.progressBar1.Visible = true;
                bgwUpdate.RunWorkerAsync();
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
                FileLogger.Log(this.Name, "btnUpdate_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnUpdate_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnUpdate_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnUpdate_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnUpdate_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnUpdate_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnUpdate_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message + Environment.NewLine + ex.StackTrace;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnUpdate_Click", exMessage);
            }
            #endregion
        }

        private void bgwUpdate_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                #region Statement


                UPDATE_DOWORK_SUCCESS = false;
                sqlResults = new string[4];
                //DemandDAL demandDal = new DemandDAL();
                IDemand demandDal = OrdinaryVATDesktop.GetObject<DemandDAL, DemandRepo, IDemand>(OrdinaryVATDesktop.IsWCF);

                sqlResults = demandDal.DemandUpdate(demandMasterVM, demandDetailVMs,connVM);
                UPDATE_DOWORK_SUCCESS = true;

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
                FileLogger.Log(this.Name, "bgwUpdate_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "bgwUpdate_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "bgwUpdate_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "bgwUpdate_DoWork", exMessage);
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
                FileLogger.Log(this.Name, "bgwUpdate_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwUpdate_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwUpdate_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "bgwUpdate_DoWork", exMessage);
            }
            #endregion

        }
        private void bgwUpdate_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                #region Statement

                if (UPDATE_DOWORK_SUCCESS)
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
                            IsUpdate = true;

                            //txtItemNo.Text = newId;
                            ChangeData = false;
                        }

                        if (result == "Success")
                        {
                            txtDemandNo.Text = sqlResults[2].ToString();
                            IsPost = Convert.ToString(sqlResults[3].ToString()) == "Y" ? true : false;
                            for (int i = 0; i < dgvDemand.RowCount; i++)
                            {
                                dgvDemand["Status", dgvDemand.RowCount - 1].Value = "Old";
                            }
                        }
                    }
                ChangeData = false;
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
                FileLogger.Log(this.Name, "bgwUpdate_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "bgwUpdate_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "bgwUpdate_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "bgwUpdate_RunWorkerCompleted", exMessage);
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
                FileLogger.Log(this.Name, "bgwUpdate_RunWorkerCompleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwUpdate_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwUpdate_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "bgwUpdate_RunWorkerCompleted", exMessage);
            }
            #endregion
            finally
            {
                ChangeData = false;
                this.btnUpdate.Enabled = true;
                this.progressBar1.Visible = false;
            }

        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            ChangeData = true;
            if (txtComments.Text == "")
            {
                txtComments.Text = " ";
            }
            if (txtBandProId.Text == "")
            {
                MessageBox.Show("Please select a Item", this.Text);
                return;
            }
            if (txtQuantity.Text == "")
            {
                txtQuantity.Text = "0.00";
            }


            if (txtNBRPrice.Text == "")
            {
                txtNBRPrice.Text = "0.00";
            }
            //if (Convert.ToDecimal(txtNBRPrice.Text.Trim()) <= 0)
            //{
            //    MessageBox.Show(MessageVM.msgNegPrice);
            //    txtNBRPrice.Focus();
            //    return;
            //}

            if (Convert.ToDecimal(txtQuantity.Text) <= 0)
            {
                MessageBox.Show("Please input Quantity", this.Text);
                txtQuantity.Focus();
                return;
            }
            if (cmbUom.SelectedIndex == 0 || cmbUom.SelectedIndex == -1)
            {
                MessageBox.Show(this.Text, "Please select pack size");
                return;
            }

            for (int i = 0; i < dgvDemand.RowCount; i++)
            {
                if (dgvDemand.Rows[i].Cells["BandProductId"].Value.ToString() == txtBandProId.Text)
                {
                    MessageBox.Show("Same Product already exist.", this.Text);

                    return;
                }
            }
           

            DataGridViewRow NewRow = new DataGridViewRow();
            dgvDemand.Rows.Add(NewRow);
           
            dgvDemand["PCode", dgvDemand.RowCount - 1].Value = cmbProduct.Text.Trim();
            dgvDemand["ItemName", dgvDemand.RowCount - 1].Value = cmbProductName.Text.Trim();
            dgvDemand["Banderol", dgvDemand.RowCount - 1].Value = txtBanderol.Text.Trim();
            dgvDemand["PackNature", dgvDemand.RowCount - 1].Value = txtPackaging.Text.Trim();
            if (Program.CheckingNumericTextBox(txtQuantity, "txtQuantity") == true)
                //txtQuantity.Text = Program.FormatingNumeric(txtQuantity.Text.Trim(), 4).ToString();
            dgvDemand["Quantity", dgvDemand.RowCount - 1].Value = Convert.ToDecimal(txtQuantity.Text.Trim()).ToString();
            string strUom = cmbUom.SelectedItem.ToString();
            dgvDemand["UOM", dgvDemand.RowCount - 1].Value = strUom.Trim();
            dgvDemand["Comments", dgvDemand.RowCount - 1].Value = txtComments.Text.Trim();
            NBRPriceCall();
            if (Program.CheckingNumericTextBox(txtNBRPrice, "txtNBRPrice") == true)
                //txtNBRPrice.Text = Program.FormatingNumeric(txtNBRPrice.Text.Trim(), 4).ToString();
            dgvDemand["NBRPrice", dgvDemand.RowCount - 1].Value = Convert.ToDecimal(txtNBRPrice.Text.Trim()).ToString();
            dgvDemand["ItemNo", dgvDemand.RowCount - 1].Value = txtItemNo.Text.Trim();
            dgvDemand["BandProductId", dgvDemand.RowCount - 1].Value = txtBandProId.Text.Trim();
            dgvDemand["Status", dgvDemand.RowCount - 1].Value = "New";

            Rowcalculate();
            ClearAll();
            selectLastRow();

            if (rbtnCode.Checked)
            {
                cmbProduct.Focus();
            }
            else
            {
                cmbProductName.Focus();
            }
        }

        private void NBRPriceCall()
        {
            //BugsBD
            string ItemNo = OrdinaryVATDesktop.SanitizeInput(txtItemNo.Text.Trim());
            
            ProductDAL productDal = new ProductDAL();
            //IProduct productDal = OrdinaryVATDesktop.GetObject<ProductDAL, ProductRepo, IProduct>(OrdinaryVATDesktop.IsWCF);

            txtNBRPrice.Text =
                productDal.GetLastNBRPriceFromBOM_VatName(ItemNo, "VAT 1",
                                                     dtpDemandDate.Value.ToString("yyyy-MMM-dd"), null, null, "0", connVM).ToString();

               //productDal.GetLastNBRPriceFromBOM_VatName(txtItemNo.Text.Trim(), "VAT 1",dtpDemandDate.Value.ToString("yyyy-MMM-dd"), null, null,"0",connVM).ToString();
        }

        private void Rowcalculate()
        {
            #region try
            try
            {

                decimal SumTotalQty = 0;
                decimal Quantity = 0;
                //decimal Cost = 0;
                //decimal SubTotal = 0;

                for (int i = 0; i < dgvDemand.RowCount; i++)
                {
                    dgvDemand[0, i].Value = i + 1;


                    Quantity = Convert.ToDecimal(dgvDemand["Quantity", i].Value);
                    //Cost = Convert.ToDecimal(dgvIssue["UnitPrice", i].Value);

                    //SubTotal = Cost * Quantity;
                    //if (Program.CheckingNumericString(SubTotal.ToString(), "SubTotal") == true)
                    //    SubTotal = Convert.ToDecimal(Program.FormatingNumeric(SubTotal.ToString(), IssuePlaceQty));

                    //dgvIssue["SubTotal", i].Value = Convert.ToDecimal(SubTotal);

                    //dgvIssue["VATAmount", i].Value = 0;
                    //dgvIssue["SDAmount", i].Value = 0;
                    //dgvIssue["SubTotal", i].Value = Convert.ToDecimal(SubAmount).ToString();//"0,0.00");

                    SumTotalQty = SumTotalQty + Convert.ToDecimal(dgvDemand["Quantity", i].Value);

                    //dgvDemand["Change", i].Value = Convert.ToDecimal(Convert.ToDecimal(dgvIssue["Quantity", i].Value)
                    //    - Convert.ToDecimal(dgvIssue["Previous", i].Value)).ToString();//"0,0.0000");


                }
                //txtTotalVATAmount.Text = "0";
                txtTotalQty.Text = Convert.ToDecimal(SumTotalQty).ToString();//"0,0.00");
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
                FileLogger.Log(this.Name, "Rowcalculate", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "Rowcalculate", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "Rowcalculate", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "Rowcalculate", exMessage);
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
                FileLogger.Log(this.Name, "Rowcalculate", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "Rowcalculate", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "Rowcalculate", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "Rowcalculate", exMessage);
            }
            #endregion
        }
        private void selectLastRow()
        {
            #region try
            try
            {
                if (dgvDemand.Rows.Count > 0)
                {
                    dgvDemand.Rows[dgvDemand.Rows.Count - 1].Selected = true;
                    dgvDemand.CurrentCell = dgvDemand.Rows[dgvDemand.Rows.Count - 1].Cells[1];
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
                FileLogger.Log(this.Name, "selectLastRow", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "selectLastRow", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "selectLastRow", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "selectLastRow", exMessage);
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
                FileLogger.Log(this.Name, "selectLastRow", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "selectLastRow", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "selectLastRow", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "selectLastRow", exMessage);
            }
            #endregion
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvDemand.RowCount > 0)
                {
                    if (MessageBox.Show(MessageVM.msgWantToRemoveRow + "\nItem Code: " + dgvDemand.CurrentRow.Cells["PCode"].Value, this.Text, MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        dgvDemand.Rows.RemoveAt(dgvDemand.CurrentRow.Index);
                        Rowcalculate();
                        
                    }
                }
                else
                {
                    MessageBox.Show("No Items Found in Remove.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
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
                FileLogger.Log(this.Name, "btnRemove_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnRemove_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnRemove_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnRemove_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnRemove_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnRemove_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnRemove_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnRemove_Click", exMessage);
            }

            #endregion

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnChange_Click(object sender, EventArgs e)
        {
            if (dgvDemand.RowCount > 0)
            {
                ReceiveChangeSingle();
            }
            else
            {
                MessageBox.Show("No Items Found in Details Section.\nAdd New Items OR Load Previous Item !", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
        }

        private void ReceiveChangeSingle()
        {
            try
            {
                #region try
                if (
                    string.IsNullOrEmpty(txtBandProId.Text)
                    //|| string.IsNullOrEmpty(txtItemNo.Text)
                    || string.IsNullOrEmpty(txtQuantity.Text))
                {
                    MessageBox.Show("Please Select Desired Item Row Appropriately Using Double Click!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (Convert.ToDecimal(txtQuantity.Text) <= 0)
                {
                    MessageBox.Show("Zero Quantity Is not Allowed  for Change Button!\nPlease Provide Quantity.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                //if (Convert.ToDecimal(txtUnitCost.Text.Trim()) <= 0)
                //{
                //    MessageBox.Show(MessageVM.msgNegPrice);

                //    txtUnitCost.Focus();
                //    return;
                //}
                //if (txtCommentsDetail.Text == "")
                //{
                //    txtCommentsDetail.Text = "NA";
                //}

                ChangeData = true;
                if (cmbUom.SelectedIndex == -1)
                {
                    throw new ArgumentNullException("", "Please select pack size");
                }

                dgvDemand["PCode", dgvDemand.CurrentRow.Index].Value = cmbProduct.Text.Trim();
                dgvDemand["ItemName", dgvDemand.CurrentRow.Index].Value = cmbProductName.Text.Trim();
                dgvDemand["ItemNo", dgvDemand.CurrentRow.Index].Value = txtItemNo.Text.Trim();
                dgvDemand["Banderol", dgvDemand.CurrentRow.Index].Value = txtBanderol.Text.Trim();
                dgvDemand["PackNature", dgvDemand.CurrentRow.Index].Value = txtPackaging.Text.Trim();
                if (Program.CheckingNumericTextBox(txtQuantity, "txtQuantity") == true)
                    //txtQuantity.Text = Program.FormatingNumeric(txtQuantity.Text.Trim(), 4).ToString();
                dgvDemand["Quantity", dgvDemand.CurrentRow.Index].Value = Convert.ToDecimal(txtQuantity.Text.Trim()).ToString();
                string strUom = cmbUom.SelectedItem.ToString();
                dgvDemand["UOM", dgvDemand.CurrentRow.Index].Value = strUom.Trim();
                dgvDemand["Comments", dgvDemand.CurrentRow.Index].Value = txtComments.Text.Trim();
                NBRPriceCall();
                if (Program.CheckingNumericTextBox(txtNBRPrice, "txtNBRPrice") == true)
                    //txtNBRPrice.Text = Program.FormatingNumeric(txtNBRPrice.Text.Trim(), 4).ToString();
                    dgvDemand["NBRPrice", dgvDemand.RowCount - 1].Value = Convert.ToDecimal(txtNBRPrice.Text.Trim()).ToString();
                dgvDemand["BandProductId", dgvDemand.CurrentRow.Index].Value = txtBandProId.Text.Trim();
               
                if (dgvDemand.CurrentRow.Cells["Status"].Value.ToString() != "New")
                {
                    dgvDemand["Status", dgvDemand.CurrentRow.Index].Value = "Change";
                }
                dgvDemand.CurrentRow.DefaultCellStyle.ForeColor = Color.Green;// Blue;
                dgvDemand.CurrentRow.DefaultCellStyle.Font = new Font(this.Font, FontStyle.Regular);

                Rowcalculate();
                ClearAll();
                if (rbtnCode.Checked)
                {
                    cmbProduct.Focus();

                }
                else
                {
                    cmbProductName.Focus();
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
                FileLogger.Log(this.Name, "btnChange_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnChange_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnChange_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnChange_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnChange_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnChange_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnChange_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnChange_Click", exMessage);
            }
            #endregion
        }

        private void FormMonthlyDemand_Load(object sender, EventArgs e)
        {
            #region try
            try
            {
                #region Settings
                 BandeDPlaceQ = 0;
                 string vBandeDPlaceQ = string.Empty;
                CommonDAL commonDal = new CommonDAL();
                vBandeDPlaceQ = commonDal.settingsDesktop("Banderol", "Quantity",null,connVM);
               
                if (string.IsNullOrEmpty(vBandeDPlaceQ))
                    
                {
                    MessageBox.Show(MessageVM.msgSettingValueNotSave, this.Text);
                    return;
                }

                BandeDPlaceQ = Convert.ToInt32(vBandeDPlaceQ);
                #endregion
                System.Windows.Forms.ToolTip ToolTip1 = new System.Windows.Forms.ToolTip();
                ToolTip1.SetToolTip(this.btnSearchDemandNo, "Existing Information");
                ToolTip1.SetToolTip(this.btnAddNew, "New");
                ClearAllFields();
                UOMSearch();
                LoadProducts();

                this.Text = "Demand Entry";
                txtDemandNo.Text = "~~~ New ~~~";
                ChangeData = false;
                Post = Convert.ToString(Program.Post) == "Y" ? true : false;
                Add = Convert.ToString(Program.Add) == "Y" ? true : false;
                Edit = Convert.ToString(Program.Edit) == "Y" ? true : false;
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
                FileLogger.Log(this.Name, "FormMonthlyDemand_Load", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "FormMonthlyDemand_Load", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "FormMonthlyDemand_Load", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "FormMonthlyDemand_Load", exMessage);
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
                FileLogger.Log(this.Name, "FormMonthlyDemand_Load", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormMonthlyDemand_Load", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormMonthlyDemand_Load", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "FormMonthlyDemand_Load", exMessage);
            }
            #endregion

        }

        private void ClearAllFields()
        {
            txtDemandNo.Text = "~~~ New ~~~";
            dtpDemandDate.Value = Convert.ToDateTime(Program.SessionDate.ToString("yyyy-MMM-dd"));
            dtpMonthFrom.Value = Convert.ToDateTime(Program.SessionDate.ToString("yyyy-MMM"));
            dtpMonthTo.Value = Convert.ToDateTime(Program.SessionDate.ToString("yyyy-MMM"));
            rbtnCode.Checked = true;
            txtTotalQty.Text = "0.00";
            chkDemand.Checked = false;
            dgvDemand.Rows.Clear();
            ClearAll();
        }

        private void ClearAll()
        {
            txtBanderol.Text = "";
            txtPackaging.Text = "";
            txtNBRPrice.Text = "0.00";
            txtQuantity.Text = "0.00";
            
            cmbProduct.SelectedIndex = -1;
            cmbProductName.SelectedIndex = -1;
            cmbUom.SelectedIndex = -1;
            txtComments.Text = "";
            txtBandProId.Text = "";
            txtItemNo.Text = "";
            cmbProductName.Text = "";
            
        }

        private void UOMSearch()
        {
            #region try

            try
            {
                this.progressBar1.Visible = true;

                #region Search UOM from DAl
                //UOMDAL uomdal = new UOMDAL();
                IUOM uomdal = OrdinaryVATDesktop.GetObject<UOMDAL, UOMRepo, IUOM>(OrdinaryVATDesktop.IsWCF);

                uomResult = uomdal.SearchUOMCodeOnly("Y",connVM);
                #endregion

                #region Assign UOM
                UOMs.Clear();
                cmbUom.Items.Clear();
                if (uomResult != null)
                {
                    foreach (DataRow item2 in uomResult.Rows)
                    {
                        cmbUom.Items.Add(item2["UOMCode"].ToString());

                    }
                }

                cmbUom.Items.Insert(0, "Select");
                //cmbUom.SelectedIndex = 0;
                cmbUom.Text = "Select";

                this.progressBar1.Visible = false;

                #endregion
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
                FileLogger.Log(this.Name, "UOMSearch", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "UOMSearch", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "UOMSearch", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "UOMSearch", exMessage);
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
                FileLogger.Log(this.Name, "UOMSearch", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "UOMSearch", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "UOMSearch", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "UOMSearch", exMessage);
            }
            finally
            {
                this.progressBar1.Visible = false;
            }

            #endregion
        }
        private void LoadProducts()
        {
            try
            {
                #region Product Search
                //ProductDAL productDal = new ProductDAL();
                //BanderolProducts = productDal.SearchBanderolProducts();

                //BanderolProductsDAL bandProDal=new BanderolProductsDAL();
                IBanderolProducts bandProDal = OrdinaryVATDesktop.GetObject<BanderolProductsDAL, BanderolProductsRepo, IBanderolProducts>(OrdinaryVATDesktop.IsWCF);

                BanderolProducts=bandProDal.SearchBanderolProducts("","","","","","","",connVM);

                #endregion Product Search 

                #region Product Load
                BanderolProductsMini.Clear();
                foreach (DataRow item2 in BanderolProducts.Rows)
                {
                    var prod = new BanderolProductMiniDTO();
                    prod.BandProductId = item2["BandProductId"].ToString();
                    prod.ItemNo = item2["ItemNo"].ToString();
                    prod.ProductName = item2["ProductName"].ToString();
                    prod.ProductCode = item2["ProductCode"].ToString();
                    
                    prod.BanderolId = item2["BanderolId"].ToString();
                    prod.BanderolName = item2["BanderolName"].ToString();
                    prod.BanderolSize = item2["BanderolSize"].ToString();
                    prod.BanderolUom = item2["BanderolUom"].ToString();

                    prod.PackagingId = item2["PackagingId"].ToString();
                    prod.PackagingName = item2["PackagingName"].ToString();
                    prod.PackagingSize = item2["PackagingSize"].ToString();
                    prod.PackagingUom = item2["PackagingUom"].ToString();
                    
                    prod.BUsedQty = Convert.ToDecimal(item2["BUsedQty"].ToString());
                    
                    BanderolProductsMini.Add(prod);
                }//End FOR
                BanderolProductSearchDsLoad();

                #endregion Product
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
                FileLogger.Log(this.Name, "LoadProducts", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "LoadProducts", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "LoadProducts", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "LoadProducts", exMessage);
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
                FileLogger.Log(this.Name, "LoadProducts", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "LoadProducts", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "LoadProducts", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "LoadProducts", exMessage);
            }
            #endregion
            finally
            {
                ChangeData = false;
                this.progressBar1.Visible = false;
            }
        }
        private void BanderolProductSearchDsLoad()
        {
            #region try
            try
            {
                cmbProduct.Items.Clear();
                cmbProductName.Items.Clear();

                if (rbtnCode.Checked == true)
                {

                    var prodByCode = from prd in BanderolProductsMini.ToList()
                                     orderby prd.ProductCode
                                     select prd.ProductCode;


                    if (prodByCode != null && prodByCode.Any())
                    {
                        cmbProduct.Items.AddRange(prodByCode.ToArray());
                    }

                    cmbProduct.Items.Insert(0, "Select");
                }
                else if (rbtnProduct.Checked == true)
                {
                    var prodByName = from prd in BanderolProductsMini.ToList()
                                     orderby prd.ProductName
                                     select prd.ProductName;


                    if (prodByName != null && prodByName.Any())
                    {
                        cmbProductName.Items.AddRange(prodByName.ToArray());
                    }
                    cmbProductName.Items.Insert(0, "Select");

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
                FileLogger.Log(this.Name, "ProductSearchDsLoad", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "ProductSearchDsLoad", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "ProductSearchDsLoad", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "ProductSearchDsLoad", exMessage);
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
                FileLogger.Log(this.Name, "ProductSearchDsLoad", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ProductSearchDsLoad", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ProductSearchDsLoad", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "ProductSearchDsLoad", exMessage);
            }
            #endregion


        }

        private void FormMonthlyDemand_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (ChangeData == true)
            {
                if (DialogResult.Yes != MessageBox.Show(

                    "Recent changes have not been saved ." + "\n" + " Want to close without saving?",
                    this.Text,

                     MessageBoxButtons.YesNo,

                     MessageBoxIcon.Question,

                     MessageBoxDefaultButton.Button2))
                {
                    e.Cancel = true;
                }

            }
        }

        private void cmbProduct_Leave(object sender, EventArgs e)
        {
            #region try
            try
            {
                var searchText = cmbProduct.Text.Trim().ToLower();
                if (!string.IsNullOrEmpty(searchText) && searchText != "select")
                {
                    if (rbtnCode.Checked)
                    {
                        var prodByCode = from prd in BanderolProductsMini.ToList()
                                         where prd.ProductCode.ToLower() == searchText.ToLower()
                                         select prd;
                        if (prodByCode != null && prodByCode.Any())
                        {
                            var products = prodByCode.First();
                            txtItemNo.Text = products.ItemNo;
                            cmbProductName.Text = products.ProductName;
                            txtBanderol.Text = products.BanderolName + " - " + products.BanderolSize.ToString() + " " + products.BanderolUom;
                            txtPackaging.Text = products.PackagingName + " - " + products.PackagingSize.ToString() + products.PackagingUom;
                            txtBandProId.Text = products.BandProductId;

                        }
                    }

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
                FileLogger.Log(this.Name, "cmbProductCode_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "cmbProductCode_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "cmbProductCode_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "cmbProductCode_Leave", exMessage);
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
                FileLogger.Log(this.Name, "cmbProductCode_Leave", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "cmbProductCode_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "cmbProductCode_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "cmbProductCode_Leave", exMessage);
            }
            #endregion
        }

        private void btnPost_Click(object sender, EventArgs e)
        {
            #region try
            try
            {
               if (IsPost == true)
                {
                    MessageBox.Show(MessageVM.ThisTransactionWasPosted, this.Text);
                    return;
                }
                else if (IsUpdate == false)
                {
                    MessageBox.Show(MessageVM.msgNotPost, this.Text);
                    return;
                }

                if (Program.CheckLicence(dtpDemandDate.Value) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }
                //TransactionTypes();
                if (IsUpdate == false)
                {
                    NextID = string.Empty;
                }
                else
                {
                    NextID = txtDemandNo.Text.Trim();
                }

                

                if (dgvDemand.RowCount <= 0)
                {
                    MessageBox.Show("Please insert Details information for transaction", this.Text, MessageBoxButtons.OK,
                                     MessageBoxIcon.Information);
                    return;
                }


                demandMasterVM = new DemandMasterVM();

                demandMasterVM.DemandNo = NextID.ToString();
                demandMasterVM.DemandDateTime = dtpDemandDate.Value.ToString("yyyy-MMM-dd");
                //demandMasterVM.FiscalYear = "2013/2014";
                demandMasterVM.MonthFrom = dtpMonthFrom.Value.ToString("MMM-yyyy");
                demandMasterVM.MonthTo = dtpMonthTo.Value.ToString("MMM-yyyy");
                demandMasterVM.TotalQty = Convert.ToDecimal(txtTotalQty.Text.Trim());
                demandMasterVM.LastModifiedBy = Program.CurrentUser;
                demandMasterVM.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                demandMasterVM.Post = "Y";
                demandMasterVM.TransactionType = "Demand";
                demandMasterVM.BranchId = Program.BranchId;
                demandDetailVMs = new List<DemandDetailVM>();
                for (int i = 0; i < dgvDemand.RowCount; i++)
                {
                    DemandDetailVM detail = new DemandDetailVM();

                    detail.DemandNo = NextID.ToString();
                    detail.DemandLineNo = dgvDemand.Rows[i].Cells["LineNo"].Value.ToString();
                    detail.BandProductId = dgvDemand.Rows[i].Cells["BandProductId"].Value.ToString();
                    //detail.ItemNo = dgvDemand.Rows[i].Cells["ItemNo"].Value.ToString();
                    detail.Quantity = Convert.ToDecimal(dgvDemand.Rows[i].Cells["Quantity"].Value.ToString());
                    detail.NBRPrice = 0;
                    
                    detail.UOM = dgvDemand.Rows[i].Cells["UOM"].Value.ToString();
                    //detail.SubTotal = Convert.ToDecimal(dgvDemand.Rows[i].Cells["SubTotal"].Value.ToString());
                    detail.Comments = dgvDemand.Rows[i].Cells["Comments"].Value.ToString();
                    detail.BranchId = Program.BranchId;
                    demandDetailVMs.Add(detail);

                }


                if (demandDetailVMs.Count() <= 0)
                {
                    MessageBox.Show("Please insert Details information for transaction", this.Text, MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                    return;
                }
                this.btnPost.Enabled = false;
                this.progressBar1.Visible = true;

                backgroundWorkerPost.RunWorkerAsync();


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
                FileLogger.Log(this.Name, "btnPost_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnPost_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnPost_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnPost_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnPost_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnPost_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnPost_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnPost_Click", exMessage);
            }
            #endregion
        }
        private void backgroundWorkerPost_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                POST_DOWORK_SUCCESS = false;
                sqlResults = new string[4];
                //DemandDAL demandDal = new DemandDAL();
                IDemand demandDal = OrdinaryVATDesktop.GetObject<DemandDAL, DemandRepo, IDemand>(OrdinaryVATDesktop.IsWCF);

                sqlResults = demandDal.DemandPost(demandMasterVM, demandDetailVMs,connVM);
                POST_DOWORK_SUCCESS = true;

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
                FileLogger.Log(this.Name, "backgroundWorkerPost_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerPost_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerPost_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "backgroundWorkerPost_DoWork", exMessage);
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
                FileLogger.Log(this.Name, "backgroundWorkerPost_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerPost_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerPost_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "backgroundWorkerPost_DoWork", exMessage);
            }
            #endregion
        }
        private void backgroundWorkerPost_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (POST_DOWORK_SUCCESS)
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
                            IsUpdate = true;

                            //txtItemNo.Text = newId;

                        }

                        if (result == "Success")
                        {
                            txtDemandNo.Text = sqlResults[2].ToString();
                            IsPost = Convert.ToString(sqlResults[3].ToString()) == "Y" ? true : false;
                            for (int i = 0; i < dgvDemand.RowCount; i++)
                            {
                                dgvDemand["Status", dgvDemand.RowCount - 1].Value = "Old";
                            }
                        }
                    }
                ChangeData = false;
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
                FileLogger.Log(this.Name, "backgroundWorkerPost_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerPost_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerPost_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "backgroundWorkerPost_RunWorkerCompleted", exMessage);
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
                FileLogger.Log(this.Name, "backgroundWorkerPost_RunWorkerCompleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerPost_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerPost_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "backgroundWorkerPost_RunWorkerCompleted", exMessage);
            }
            #endregion
            finally
            {
                ChangeData = false;
                this.btnPost.Enabled = true;
                this.progressBar1.Visible = false;
            }

        }

        private void rbtnCode_CheckedChanged(object sender, EventArgs e)
        {
            if (rbtnCode.Checked)
            {
                cmbProductName.Enabled = false;
                cmbProduct.Enabled = true;

            }

            BanderolProductSearchDsLoad();
        }

        private void rbtnProduct_CheckedChanged(object sender, EventArgs e)
        {
            if (rbtnProduct.Checked)
            {
                cmbProductName.Enabled = true;
                cmbProduct.Enabled = false;

            }

            BanderolProductSearchDsLoad();
        }

        private void cmbProductName_Leave(object sender, EventArgs e)
        {
            #region try
            try
            {

                var searchText = cmbProductName.Text.Trim().ToLower();

                if (!string.IsNullOrEmpty(searchText) && searchText != "select")
                {
                    if (rbtnProduct.Checked)
                    {
                        var prodByName = from prd in BanderolProductsMini.ToList()
                                         where prd.ProductName.ToLower() == searchText.ToLower()
                                         select prd;

                        if (prodByName != null && prodByName.Any())
                        {
                            var products = prodByName.First();
                            txtItemNo.Text = products.ItemNo;
                            cmbProduct.Text = products.ProductCode;
                            txtBandProId.Text = products.BandProductId;
                            //txtProductName.Text = products.ProductName;
                            //txtProductCode.Text = products.ProductCode;
                            txtBanderol.Text = products.BanderolName + " - " + products.BanderolSize.ToString() + " " + products.BanderolUom;
                            txtPackaging.Text = products.PackagingName + " - " + products.PackagingSize.ToString() +" "+ products.PackagingUom;
                            

                        }
                    }

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
                FileLogger.Log(this.Name, "cmbProductName_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "cmbProductName_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "cmbProductName_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "cmbProductName_Leave", exMessage);
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
                FileLogger.Log(this.Name, "cmbProductName_Leave", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "cmbProductName_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "cmbProductName_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "cmbProductName_Leave", exMessage);
            }
            #endregion
        }

        private void btnAddNew_Click(object sender, EventArgs e)
        {
            ClearAllFields();
            ChangeData = false;
        }

        private void dgvDemand_DoubleClick(object sender, EventArgs e)
        {
            #region try
            try
            {
                if (dgvDemand.Rows.Count <= 0)
                {
                    MessageBox.Show("There is no data to select.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                cmbProduct.Text = dgvDemand.CurrentRow.Cells["PCode"].Value.ToString();
                cmbProductName.Text = dgvDemand.CurrentRow.Cells["ItemName"].Value.ToString();
               var t= dgvDemand.CurrentCellAddress.Y.ToString();
                //txtLineNo.Text = dgvIssue.CurrentCellAddress.Y.ToString();
                cmbUom.Text = dgvDemand.CurrentRow.Cells["UOM"].Value.ToString();
                txtQuantity.Text = Convert.ToDecimal(dgvDemand.CurrentRow.Cells["Quantity"].Value).ToString();//"0,0.0000");
                txtComments.Text = dgvDemand.CurrentRow.Cells["Comments"].Value.ToString();
                txtNBRPrice.Text = Convert.ToDecimal(dgvDemand.CurrentRow.Cells["NBRPrice"].Value).ToString();//"0,0.00");
                txtBanderol.Text = dgvDemand.CurrentRow.Cells["Banderol"].Value.ToString();//"0,0.00");
                txtPackaging.Text = dgvDemand.CurrentRow.Cells["PackNature"].Value.ToString();//"0,0.00");
                txtBandProId.Text = dgvDemand.CurrentRow.Cells["BandProductId"].Value.ToString();
                txtItemNo.Text = dgvDemand.CurrentRow.Cells["ItemNo"].Value.ToString();
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
                FileLogger.Log(this.Name, "dgvDemand_DoubleClick", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "dgvDemand_DoubleClick", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "dgvDemand_DoubleClick", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "dgvDemand_DoubleClick", exMessage);
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
                FileLogger.Log(this.Name, "dgvDemand_DoubleClick", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "dgvDemand_DoubleClick", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "dgvDemand_DoubleClick", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "dgvDemand_DoubleClick", exMessage);
            }
            #endregion
        }

        private void btnSearchDemandNo_Click(object sender, EventArgs e)
        {
            #region try
            try
            {

                Program.fromOpen = "Me";
                #region Transaction Type
                //TransactionTypes();
                transactionType = "Demand";
                DataGridViewRow selectedRow = new DataGridViewRow();

                selectedRow = FormDemandSearch.SelectOne(transactionType);

                #endregion Transaction Type

                if (selectedRow != null && selectedRow.Selected == true)
                {
                    txtDemandNo.Text = selectedRow.Cells["DemandNo"].Value.ToString();
                    dtpDemandDate.Value = Convert.ToDateTime(selectedRow.Cells["DemandDate"].Value.ToString());
                    txtTotalQty.Text = Convert.ToDecimal(selectedRow.Cells["TotalQty"].Value.ToString()).ToString();//"0,0.00");
                    //txtF.Text = selectedRow.Cells["FiscalYear"].Value.ToString();
                    //var mFrom = selectedRow.Cells["MonthFrom"].Value.ToString();
                    //dtpMonthFrom.Text = Convert.ToDateTime(mFrom).ToString("MMM-yyyyy");
                    dtpMonthFrom.Text = Convert.ToDateTime(selectedRow.Cells["MonthFrom"].Value.ToString()).ToString();
                    dtpMonthTo.Text = Convert.ToDateTime(selectedRow.Cells["MonthTo"].Value.ToString()).ToString();

                    IsPost = Convert.ToString(selectedRow.Cells["Post"].Value.ToString()) == "Y" ? true : false;
                    
                    DemandDetailNo = txtDemandNo.Text == "" ? "" : txtDemandNo.Text.Trim();
                }

                bgwSearchDemandNo.RunWorkerAsync();

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
                FileLogger.Log(this.Name, "btnSearchIssueNo_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnSearchIssueNo_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnSearchIssueNo_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnSearchIssueNo_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnSearchIssueNo_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnSearchIssueNo_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnSearchIssueNo_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnSearchIssueNo_Click", exMessage);
            }
            #endregion
        }

        private void bgwSearchDemandNo_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                #region Statement

                DemandDetailResult = new DataTable();

                //DemandDAL demandDal = new DemandDAL();
                IDemand demandDal = OrdinaryVATDesktop.GetObject<DemandDAL, DemandRepo, IDemand>(OrdinaryVATDesktop.IsWCF);

                DemandDetailResult = demandDal.SearchDemandDetailDTNew(DemandDetailNo,connVM); // Change 04

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
                FileLogger.Log(this.Name, "backgroundWorkerSearchIssueNo_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerSearchIssueNo_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerSearchIssueNo_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "backgroundWorkerSearchIssueNo_DoWork", exMessage);
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
                FileLogger.Log(this.Name, "backgroundWorkerSearchIssueNo_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerSearchIssueNo_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerSearchIssueNo_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "backgroundWorkerSearchIssueNo_DoWork", exMessage);
            }
            #endregion
        }

        string packNature, banderolNature;

        private void bgwSearchDemandNo_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                #region Statement

                dgvDemand.Rows.Clear();
                int j = 0;
                
                foreach (DataRow item in DemandDetailResult.Rows)
                {


                    DataGridViewRow NewRow = new DataGridViewRow();

                    packNature= item["PackagingName"].ToString()+" -"+item["PackagingSize"].ToString()+" "+item["PackagingUom"].ToString();
                    banderolNature = item["BanderolName"].ToString() + " -" + item["BanderolSize"].ToString() + " " + item["BanderolUom"].ToString();

                    dgvDemand.Rows.Add(NewRow);
                    dgvDemand.Rows[j].Cells["LineNo"].Value = item["DemandLineNo"].ToString();
                    dgvDemand.Rows[j].Cells["PCode"].Value = item["ProductCode"].ToString();
                    dgvDemand.Rows[j].Cells["ItemName"].Value = item["ProductName"].ToString();
                    dgvDemand.Rows[j].Cells["Banderol"].Value = banderolNature;
                    dgvDemand.Rows[j].Cells["PackNature"].Value = packNature;
                    dgvDemand.Rows[j].Cells["Quantity"].Value = item["Quantity"].ToString();
                    dgvDemand.Rows[j].Cells["UOM"].Value = item["UOM"].ToString();
                    dgvDemand.Rows[j].Cells["Comments"].Value = item["Comments"].ToString();
                    dgvDemand.Rows[j].Cells["NBRPrice"].Value = item["NBRPrice"].ToString();
                    dgvDemand.Rows[j].Cells["BandProductId"].Value = item["BandProductId"].ToString();
                    dgvDemand.Rows[j].Cells["Status"].Value = "Old";
                    dgvDemand.Rows[j].Cells["ItemNo"].Value = item["ItemNo"].ToString();
                    

                    j = j + 1;
                }  //End For


                //btnSave.Text = "&Save";
                IsUpdate = true;
                ChangeData = false;

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
                FileLogger.Log(this.Name, "backgroundWorkerSearchIssueNo_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerSearchIssueNo_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerSearchIssueNo_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "backgroundWorkerSearchIssueNo_RunWorkerCompleted", exMessage);
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
                FileLogger.Log(this.Name, "backgroundWorkerSearchIssueNo_RunWorkerCompleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerSearchIssueNo_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerSearchIssueNo_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "backgroundWorkerSearchIssueNo_RunWorkerCompleted", exMessage);
            }
            #endregion
            finally
            {
                ChangeData = false;
                this.btnSearchDemandNo.Enabled = true;
                this.progressBar1.Visible = false;
            }
        }

        private void btnForm1_Click(object sender, EventArgs e)
        {
            chkDemand.Checked = true;
            try
            {
                if (txtDemandNo.Text == "~~~ New ~~~")
                {
                    MessageBox.Show("Demand information not save.");
                    return;
                }

                if (Program.CheckLicence(dtpDemandDate.Value) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }
                btnForm1.Enabled = false;
                progressBar1.Visible = true;
                bgwPrint.RunWorkerAsync();
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
                FileLogger.Log(this.Name, "btnForm1_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnForm1_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnForm1_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnForm1_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnForm1_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnForm1_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnForm1_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnForm1_Click", exMessage);
            }
            #endregion
        }

        private void bgwPrint_DoWork(object sender, DoWorkEventArgs e)
        {

            try
            {
                #region Statement
                string demandPost = IsPost == true ? "Y" : "N";
                transactionType = "Demand";
                ReportResult = new DataSet();
                //ReportDSDAL reportDsdal = new ReportDSDAL();
                IReport reportDsdal = OrdinaryVATDesktop.GetObject<ReportDSDAL, ReportDSRepo, IReport>(OrdinaryVATDesktop.IsWCF);


                ReportResult = reportDsdal.DemandReport(txtDemandNo.Text.Trim(), "", "", transactionType, demandPost,connVM);



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
                FileLogger.Log(this.Name, "bgwPrint_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "bgwPrint_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "bgwPrint_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "bgwPrint_DoWork", exMessage);
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
                FileLogger.Log(this.Name, "bgwPrint_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwPrint_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwPrint_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "bgwPrint_DoWork", exMessage);
            }
            #endregion
        }

        private void bgwPrint_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                #region Statement
                ReportClass objrpt = new ReportClass();

                DBSQLConnection _dbsqlConnection = new DBSQLConnection();
                if (ReportResult.Tables.Count <= 0)
                {
                    MessageBox.Show("There is no data to preview", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                ReportResult.Tables[0].TableName = "DsDemand";
                if (chkDemand.Checked)
                {
                    objrpt = new RptDemand();
                }
                else
                {
                    objrpt = new RptDemandConfirm();
                }
                
                objrpt.SetDataSource(ReportResult);

                //if (PreviewOnly == true)
                //{
                //    objrpt.DataDefinition.FormulaFields["Preview"].Text = "'Preview Only'";
                //}
                //else
                //{
                //    objrpt.DataDefinition.FormulaFields["Preview"].Text = "''";
                //}

                //objrpt.DataDefinition.FormulaFields["UserName"].Text = "'" + Program.CurrentUser + "'";
                //objrpt.DataDefinition.FormulaFields["ReportName"].Text = "' Information'";
                objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + Program.CompanyName + "'";
                //objrpt.DataDefinition.FormulaFields["CompanyLegalName"].Text = "'" + Program.CompanyLegalName + "'";
                objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + Program.Address1 + "'";
                objrpt.DataDefinition.FormulaFields["Address2"].Text = "'" + Program.Address2 + "'";
                objrpt.DataDefinition.FormulaFields["Address3"].Text = "'" + Program.Address3 + "'";
                //objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + Program.TelephoneNo + "'";
                //objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + Program.FaxNo + "'";
                objrpt.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'" + Program.VatRegistrationNo + "'";
                //objrpt.DataDefinition.FormulaFields["ZipCode"].Text = "'" + Program.ZipCode + "'";
                //objrpt.DataDefinition.FormulaFields["VATCircle"].Text = "'" + Program.Comments + "'";
                //objrpt.DataDefinition.FormulaFields["UsedQuantity"].Text = "" + 2 + "";
               
                //objrpt.DataDefinition.FormulaFields["copiesNo"].Text = "'" + copiesNo + "'";

                FormReport reports = new FormReport();
                //objrpt.DataDefinition.FormulaFields["Trial"].Text = "'" + Program.Trial + "'";
                reports.crystalReportViewer1.Refresh();
                reports.setReportSource(objrpt);
                if (Convert.ToDecimal(LicenseExpite.ExpiteDate(DatabaseInfoVM.DatabaseName)) >= Convert.ToDecimal(_dbsqlConnection.ServerDateTime())) 
                    //reports.ShowDialog();
                    reports.WindowState = FormWindowState.Maximized;
                reports.Show();
                // End Complete

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
                FileLogger.Log(this.Name, "backgroundWorkerPreview_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerPreview_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerPreview_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "backgroundWorkerPreview_RunWorkerCompleted", exMessage);
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
                FileLogger.Log(this.Name, "backgroundWorkerPreview_RunWorkerCompleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerPreview_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerPreview_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "backgroundWorkerPreview_RunWorkerCompleted", exMessage);
            }
            #endregion

            finally
            {
                btnForm1.Enabled = true;
                btnForm2.Enabled = true;
                progressBar1.Visible = false;
            }
        }

        private void btnForm2_Click(object sender, EventArgs e)
        {
            chkDemand.Checked = false;
            try
            {
                if (txtDemandNo.Text == "~~~ New ~~~")
                {
                    MessageBox.Show("Demand information not save.");
                    return;
                }

                if (Program.CheckLicence(dtpDemandDate.Value) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }
                btnForm2.Enabled = false;
                progressBar1.Visible = true;
                bgwPrint.RunWorkerAsync();
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
                FileLogger.Log(this.Name, "btnForm2_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnForm2_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnForm2_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnForm2_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnForm2_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnForm2_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnForm2_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnForm2_Click", exMessage);
            }
            #endregion
        }

        private void txtQuantity_Leave(object sender, EventArgs e)
        {
            if (Program.CheckingNumericTextBox(txtQuantity, "Quantity") == true)
            {
                txtQuantity.Text = Program.FormatingNumeric(txtQuantity.Text.Trim(), BandeDPlaceQ).ToString();
            }
            else
            {
                MessageBox.Show("Please enter numeric value.", "Total Quantity", MessageBoxButtons.OK,
                                MessageBoxIcon.Information);


                txtQuantity.Text = "0";
            }
        }

        private void cmbProduct_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void cmbUom_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void dtpMonthFrom_ValueChanged(object sender, EventArgs e)
        {

        }

        private void cmbProduct_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void cmbProductName_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void cmbProductName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void txtBanderol_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtBanderol_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void txtPackaging_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtPackaging_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void txtQuantity_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtQuantity_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void cmbUom_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtComments_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtComments_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void dtpMonthFrom_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void dtpMonthTo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void dtpDemandDate_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }
    }
}
