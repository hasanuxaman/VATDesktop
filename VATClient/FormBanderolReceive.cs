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
using VATViewModel.DTOs;
using VATServer.Ordinary;


namespace VATClient
{
    public partial class FormBanderolReceive : Form
    {
        #region Constructors

        public FormBanderolReceive()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();
        }

        #endregion

        #region Global Variables

        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        List<BanderolProductMiniDTO> BanderolProductsMini = new List<BanderolProductMiniDTO>();
        private DemandMasterVM demandMasterVM;
        private List<DemandDetailVM> demandDetailVMs = new List<DemandDetailVM>();
        //private List<UomDTO> UOMs = new List<UomDTO>();
        //private DataTable uomResult;

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

        private List<VehicleDTO> vehicles = new List<VehicleDTO>();
        private string vehicleId = string.Empty;
        private string vehicleType = string.Empty;
        private string vehicleNo = string.Empty;
        private string vehicleActiveStatus = string.Empty;
        private DataTable vehicleResult;


        private decimal PreviousQty = 0;
        //private string BanderolId = string.Empty;
        //private string BanderolName = string.Empty;

        private const string LineDelimeter = DBConstant.LineDelimeter;
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        //private static string PassPhrase = DBConstant.PassPhrase;
        //private static string EnKey = DBConstant.EnKey;
        //string encriptedIssueHeaderData;
        
        //private DataTable ProductResultDs;
        private decimal StockValue, PreviousValue, CurrentValue;

        #region Global Variables For BackGroundWorker

        private string DemandHeaderData = string.Empty;

        private string DemandDetailNo = string.Empty;

        private string DemandResult = string.Empty;

        private string DemandResultPost = string.Empty;

        private DataTable DemandDetailResult;
        private DemandDAL demandDal = new DemandDAL();

        private string ProductData = string.Empty;

        //private string varItemNo, varCategoryID, varIsRaw, varHSCodeNo, varActiveStatus, varTrading, varNonStock, varProductCode;


        private DataSet StatusResult;

        private DataTable ProductTypeResult;
        private bool Add =false;
        private bool Edit = false;

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
                if (Add == false)
                {
                    MessageBox.Show(MessageVM.msgNotAddAccess, this.Text);
                    return;
                }

                if (IsPost == true)
                {
                    MessageBox.Show(MessageVM.ThisTransactionWasPosted, this.Text);
                    return;
                }
                if (IsUpdate == false)
                {
                    NextID = string.Empty;
                }
                else
                {
                    NextID = txtReceiveNo.Text.Trim();
                }
                
                if (Program.CheckLicence(dtpReceiveDate.Value) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }


                if (txtComments.Text == "")
                {
                    txtComments.Text = "-";
                }

                if (dgvBandeReceive.RowCount <= 0)
                {
                    MessageBox.Show("Please insert Details information for transaction", this.Text, MessageBoxButtons.OK,
                                     MessageBoxIcon.Information);
                    return;
                }

                #region Null Check
                if (cmbVehicleNo.Text == "")
                {
                    MessageBox.Show("Please Enter Vehicle Number");
                    cmbVehicleNo.Focus();
                    return;
                }
                if (txtVehicleType.Text == "")
                {

                    MessageBox.Show("Please Enter Vehicle Type");
                    txtVehicleType.Focus();
                    return;
                }
                if (txtDriverName.Text == "")
                {
                    MessageBox.Show("Please Enter Driver Name");
                    txtDriverName.Focus();
                    return;
                }

                #region Stock

                string banProId;
                
                for (int i = 0; i < dgvBandeReceive.RowCount; i++)
                {
                    banProId = dgvBandeReceive.Rows[i].Cells["BandProductId"].Value.ToString();
                    PreviousValue = demandDal.ReturnQty(txtDemandNo.Text, banProId, "Y",connVM);
                    CurrentValue =Convert.ToDecimal(dgvBandeReceive.Rows[i].Cells["Quantity"].Value.ToString());

                    if (CurrentValue > PreviousValue)
                    {
                        if (PreviousValue==0)
                        {
                            MessageBox.Show("Item No(" + dgvBandeReceive.Rows[i].Cells["ItemNo"].Value.ToString() + "), Already received.");
                            return;
                        }
                        else
                        {
                            MessageBox.Show("Item No(" + dgvBandeReceive.Rows[i].Cells["ItemNo"].Value.ToString() + "), Receive quantity cannot be greater than actual Qty.");
                            return;
                        }
                        
                    }

                }                
                #endregion
              
                #endregion Null Check

                


                //TransactionTypes();
                demandMasterVM = new DemandMasterVM();

                demandMasterVM.DemandNo = NextID.ToString();
                demandMasterVM.ReceiveDate = dtpReceiveDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");
                demandMasterVM.RefDate = dtpRefDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");
                demandMasterVM.RefNo = txtRefNo.Text;
                //demandMasterVM.MonthTo = dtpMonthTo.Value.ToString("MMM-yyyy");
                //demandMasterVM.TotalQty = Convert.ToDecimal(txtTotalQty.Text.Trim());
                demandMasterVM.DemandReceiveID = txtDemandNo.Text;
                demandMasterVM.VehicleNo = cmbVehicleNo.Text;
                demandMasterVM.VehicleType = txtVehicleType.Text;
                demandMasterVM.DriverName = txtDriverName.Text;
                demandMasterVM.VehicleSaveInDB = false;
                if (chkVehicleSaveInDatabase.Checked == true)
                {
                    demandMasterVM.VehicleSaveInDB = true;
                }
                demandMasterVM.CreatedBy = Program.CurrentUser;
                demandMasterVM.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                demandMasterVM.LastModifiedBy = Program.CurrentUser;
                demandMasterVM.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                demandMasterVM.Post = "N";
                demandMasterVM.TransactionType = "Receive";

                demandDetailVMs = new List<DemandDetailVM>();
                for (int i = 0; i < dgvBandeReceive.RowCount; i++)
                {
                    DemandDetailVM detail = new DemandDetailVM();

                    detail.DemandNo = NextID.ToString();
                    detail.DemandLineNo = dgvBandeReceive.Rows[i].Cells["LineNo"].Value.ToString();
                    detail.BandProductId = dgvBandeReceive.Rows[i].Cells["BandProductId"].Value.ToString();
                    detail.ItemNo = dgvBandeReceive.Rows[i].Cells["ItemNo"].Value.ToString();
                    detail.Quantity = Convert.ToDecimal(dgvBandeReceive.Rows[i].Cells["Quantity"].Value.ToString());
                    detail.NBRPrice = 0;
                    detail.UOM = dgvBandeReceive.Rows[i].Cells["UOM"].Value.ToString();
                    detail.Comments = txtComments.Text;
                    //detail.DemandQty = Convert.ToDecimal(dgvBandeReceive.Rows[i].Cells["DemandQty"].Value.ToString());

                    #region demand
                    DataTable dt = new DataTable();
                    //BugsBD
                    string DemandNo = OrdinaryVATDesktop.SanitizeInput(txtDemandNo.Text);

                    dt = demandDal.DemandQty(DemandNo, dgvBandeReceive.Rows[i].Cells["BandProductId"].Value.ToString(), "Y", connVM);

                    //dt = demandDal.DemandQty(txtDemandNo.Text, dgvBandeReceive.Rows[i].Cells["BandProductId"].Value.ToString(), "Y",connVM);

                    detail.DemandQty = Convert.ToDecimal(dt.Rows[0]["DemandQty"].ToString());
                    demandMasterVM.DemandDateTime = Convert.ToDateTime(dt.Rows[0]["TransactionDate"].ToString()).ToString("yyyy-MMM-dd HH:mm:ss");
                    detail.TransactionDate = dtpReceiveDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");
                    #endregion demand


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
        //
        private void backgroundWorkerSave_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                #region Statement
                SAVE_DOWORK_SUCCESS = false;
                sqlResults = new string[4];
                //DemandDAL demandDal = new DemandDAL();
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
                            txtReceiveNo.Text = sqlResults[2].ToString();
                            IsPost = Convert.ToString(sqlResults[3].ToString()) == "Y" ? true : false;
                            for (int i = 0; i < dgvBandeReceive.RowCount; i++)
                            {
                                dgvBandeReceive["Status", dgvBandeReceive.RowCount - 1].Value = "Old";
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
                if (Program.CheckLicence(dtpRefDate.Value) == true)
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
                    NextID = txtReceiveNo.Text.Trim();
                }
                //TransactionTypes();
                //if (txtComments.Text == "")
                //{
                //    //txtComments.Text = "-";
                //}

                if (dgvBandeReceive.RowCount <= 0)
                {
                    MessageBox.Show("Please insert Details information for transaction", this.Text, MessageBoxButtons.OK,
                                     MessageBoxIcon.Information);
                    return;
                }

                #region Stock

                string banProId;

                for (int i = 0; i < dgvBandeReceive.RowCount; i++)
                {
                    banProId = dgvBandeReceive.Rows[i].Cells["BandProductId"].Value.ToString();
                    PreviousValue = demandDal.ReturnQty(txtDemandNo.Text, banProId, "Y",connVM);
                    CurrentValue = Convert.ToDecimal(dgvBandeReceive.Rows[i].Cells["Quantity"].Value.ToString());

                    if (CurrentValue > PreviousValue)
                    {
                        MessageBox.Show("Receive can not be greater than actual quantity.");
                        return;
                    }

                }
                #endregion


                demandMasterVM = new DemandMasterVM();

                demandMasterVM.DemandNo = NextID.ToString();
                demandMasterVM.ReceiveDate = dtpReceiveDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");
                demandMasterVM.RefDate = dtpRefDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");
                demandMasterVM.RefNo = txtRefNo.Text;
                demandMasterVM.DemandReceiveID = txtDemandNo.Text;
                demandMasterVM.VehicleNo = cmbVehicleNo.Text;
                demandMasterVM.VehicleType = txtVehicleType.Text;
                demandMasterVM.DriverName = txtDriverName.Text;
                demandMasterVM.VehicleSaveInDB = false;
                if (chkVehicleSaveInDatabase.Checked == true)
                {
                    demandMasterVM.VehicleSaveInDB = true;
                }
                demandMasterVM.CreatedBy = Program.CurrentUser;
                demandMasterVM.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                demandMasterVM.LastModifiedBy = Program.CurrentUser;
                demandMasterVM.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                demandMasterVM.Post = "N";
                demandMasterVM.TransactionType = "Receive";

                demandDetailVMs = new List<DemandDetailVM>();
                for (int i = 0; i < dgvBandeReceive.RowCount; i++)
                {
                    DemandDetailVM detail = new DemandDetailVM();
                    detail.DemandNo = NextID.ToString();
                    detail.DemandLineNo = dgvBandeReceive.Rows[i].Cells["LineNo"].Value.ToString();
                    detail.BandProductId = dgvBandeReceive.Rows[i].Cells["BandProductId"].Value.ToString();
                    detail.ItemNo = dgvBandeReceive.Rows[i].Cells["ItemNo"].Value.ToString();
                    detail.Quantity = Convert.ToDecimal(dgvBandeReceive.Rows[i].Cells["Quantity"].Value.ToString());
                    detail.NBRPrice = 0;
                    detail.UOM = dgvBandeReceive.Rows[i].Cells["UOM"].Value.ToString();
                    detail.Comments = txtComments.Text;
                    //detail.DemandQty = Convert.ToDecimal(dgvBandeReceive.Rows[i].Cells["DemandQty"].Value.ToString());

                    #region demand
                    DataTable dt = new DataTable();
                    //BugsBD
                    string DemandNo = OrdinaryVATDesktop.SanitizeInput(txtDemandNo.Text);
                    dt = demandDal.DemandQty(DemandNo, dgvBandeReceive.Rows[i].Cells["BandProductId"].Value.ToString(), "Y", connVM);

                    //dt = demandDal.DemandQty(txtDemandNo.Text, dgvBandeReceive.Rows[i].Cells["BandProductId"].Value.ToString(), "Y",connVM);
                    detail.DemandQty = Convert.ToDecimal(dt.Rows[0]["DemandQty"].ToString());
                    demandMasterVM.DemandDateTime = Convert.ToDateTime(dt.Rows[0]["TransactionDate"].ToString()).ToString("yyyy-MMM-dd HH:mm:ss");
                    detail.TransactionDate = dtpReceiveDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");
                    #endregion demand


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
                            throw new ArgumentNullException("bgwUpdate_RunWorkerCompleted",
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
                            txtReceiveNo.Text = sqlResults[2].ToString();
                            IsPost = Convert.ToString(sqlResults[3].ToString()) == "Y" ? true : false;
                            for (int i = 0; i < dgvBandeReceive.RowCount; i++)
                            {
                                dgvBandeReceive["Status", dgvBandeReceive.RowCount - 1].Value = "Old";
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
            if (Convert.ToDecimal(txtQuantity.Text) <= 0)
            {
                MessageBox.Show("Please input Quantity", this.Text);
                txtQuantity.Focus();
                return;
            }
            if (Convert.ToDecimal(txtQuantity.Text) > Convert.ToDecimal(txtStock.Text))
            {
                MessageBox.Show("Stock Not available");
                txtQuantity.Focus();
                return;
            }

            for (int i = 0; i < dgvBandeReceive.RowCount; i++)
            {
                if (dgvBandeReceive.Rows[i].Cells["BandProductId"].Value.ToString() == txtBandProId.Text)
                {
                    MessageBox.Show("Same Product already exist.", this.Text);

                    return;
                }
            }
            
            ClearAll();
            selectLastRow();

            
        }

       
       
        private void selectLastRow()
        {
            #region try
            try
            {
                if (dgvBandeReceive.Rows.Count > 0)
                {
                    dgvBandeReceive.Rows[dgvBandeReceive.Rows.Count - 1].Selected = true;
                    dgvBandeReceive.CurrentCell = dgvBandeReceive.Rows[dgvBandeReceive.Rows.Count - 1].Cells[1];
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
                if (dgvBandeReceive.RowCount > 0)
                {
                    if (MessageBox.Show(MessageVM.msgWantToRemoveRow + "\nItem Code: " + dgvBandeReceive.CurrentRow.Cells["PCode"].Value, this.Text, MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        dgvBandeReceive.Rows.RemoveAt(dgvBandeReceive.CurrentRow.Index);
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
            if (dgvBandeReceive.RowCount > 0)
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
                    || string.IsNullOrEmpty(txtItemNo.Text)
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
                #region Stock
                
                StockValue = Convert.ToDecimal(txtStock.Text);
                PreviousValue = Convert.ToDecimal(PreviousQty);
                CurrentValue = Convert.ToDecimal(txtQuantity.Text);

                
                if (CurrentValue > PreviousValue)
                {
                    if (PreviousValue==0)
                    {
                        MessageBox.Show("Already Received.");
                        txtQuantity.Text = PreviousValue.ToString();
                        txtQuantity.Focus();
                        return;
                    }
                    else
                    {
                        MessageBox.Show("Receive can not be greater than actual quantity.");
                        txtQuantity.Text = PreviousValue.ToString();
                        txtQuantity.Focus();
                        return;
                    }
                   
                }
                
                #endregion

                ChangeData = true;

                dgvBandeReceive["PCode", dgvBandeReceive.CurrentRow.Index].Value = txtProduct.Text.Trim();
                dgvBandeReceive["ItemName", dgvBandeReceive.CurrentRow.Index].Value = txtProductName.Text.Trim();
                dgvBandeReceive["Banderol", dgvBandeReceive.CurrentRow.Index].Value = txtBanderol.Text.Trim();
                dgvBandeReceive["PackNature", dgvBandeReceive.CurrentRow.Index].Value = txtPackaging.Text.Trim();
                if (Program.CheckingNumericTextBox(txtQuantity, "txtQuantity") == true)
                    txtQuantity.Text = Program.FormatingNumeric(txtQuantity.Text.Trim(), 4).ToString();
                dgvBandeReceive["Quantity", dgvBandeReceive.CurrentRow.Index].Value = Convert.ToDecimal(txtQuantity.Text.Trim()).ToString();
                dgvBandeReceive["Comments", dgvBandeReceive.CurrentRow.Index].Value = txtComments.Text.Trim();
                dgvBandeReceive["ItemNo", dgvBandeReceive.CurrentRow.Index].Value = txtItemNo.Text.Trim();
                //dgvBandeReceive["DemandQty", dgvBandeReceive.CurrentRow.Index].Value = DemandQuantity.ToString();

                dgvBandeReceive["Status", dgvBandeReceive.CurrentRow.Index].Value = "Change";

                dgvBandeReceive.CurrentRow.DefaultCellStyle.ForeColor = Color.Green;
                dgvBandeReceive.CurrentRow.DefaultCellStyle.Font = new Font(this.Font, FontStyle.Regular);


                ClearAll();

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

        //
        private void FormBanderolReceive_Load(object sender, EventArgs e)
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
                ToolTip1.SetToolTip(this.btnSearchReceiveNo, "Existing Information");
                ToolTip1.SetToolTip(this.btnAddNew, "New");
                ClearAllFields();
                
                LoadVehicles();

                this.Text = "Banderol Receive Entry";
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
            txtReceiveNo.Text = "~~~ New ~~~";
            txtDemandNo.Text = "";
            dtpRefDate.Value = Convert.ToDateTime(Program.SessionDate.ToString("yyyy-MMM-dd"));
            dtpReceiveDate.Value = Convert.ToDateTime(Program.SessionDate.ToString("yyyy-MMM-dd"));
            cmbVehicleNo.SelectedIndex = -1;
            txtVehicleId.Text = "";
            txtVehicleNo.Text = "";
            txtVehicleType.Text = "";
            txtDriverName.Text = "";
            txtRefNo.Text = "";
            chkSameVehicle.Checked = false;
            chkVehicleSaveInDatabase.Checked = false;
            dgvBandeReceive.Rows.Clear();
            ClearAll();
        }

        private void ClearAll()
        {
            txtProduct.Text = "";
            txtProductName.Text = "";
            txtBanderol.Text = "";
            txtPackaging.Text = "";
            txtQuantity.Text = "0.00";
            txtStock.Text = "0.00";
            txtUom.Text = "";
            txtComments.Text = "";
            txtBandProId.Text = "";
            txtItemNo.Text = "";
            
        }

        //
        private void LoadVehicles()
        {
            try
            {
                //#region Product Search
                //BanderolProductsDAL bandProDal=new BanderolProductsDAL();
                //BanderolProducts=bandProDal.SearchBanderolProducts("","","","","","","");

                //#endregion Product Search 

                //#region Product Load
                //BanderolProductsMini.Clear();
                //foreach (DataRow item2 in BanderolProducts.Rows)
                //{
                //    var prod = new BanderolProductMiniDTO();
                //    prod.BandProductId = item2["BandProductId"].ToString();
                //    prod.ItemNo = item2["ItemNo"].ToString();
                //    prod.ProductName = item2["ProductName"].ToString();
                //    prod.ProductCode = item2["ProductCode"].ToString();
                    
                //    prod.BanderolId = item2["BanderolId"].ToString();
                //    prod.BanderolName = item2["BanderolName"].ToString();
                //    prod.BanderolSize = item2["BanderolSize"].ToString();
                //    prod.BanderolUom = item2["BanderolUom"].ToString();

                //    prod.PackagingId = item2["PackagingId"].ToString();
                //    prod.PackagingName = item2["PackagingName"].ToString();
                //    prod.PackagingSize = item2["PackagingSize"].ToString();
                //    prod.PackagingUom = item2["PackagingUom"].ToString();
                    
                //    prod.BUsedQty = Convert.ToDecimal(item2["BUsedQty"].ToString());
                    
                //    BanderolProductsMini.Add(prod);
                //}//End FOR
                //BanderolProductSearchDsLoad();

                //#endregion Product

                #region vehicle
                vehicleId = string.Empty;
                vehicleType = string.Empty;
                vehicleNo = string.Empty;
                vehicleActiveStatus = string.Empty;
                #endregion vehicle

                #region vehicle

                vehicleResult = new DataTable();
                VehicleDAL vehicleDal = new VehicleDAL();
                //vehicleResult = vehicleDal.SearchVehicleDataTable(vehicleId, vehicleType, vehicleNo, vehicleActiveStatus);
                string[] cValues = { vehicleId, vehicleType, vehicleNo, vehicleActiveStatus };
                string[] cFields = { "VehicleID like", "VehicleType like", "VehicleNo like", "ActiveStatus like" };
                vehicleResult = vehicleDal.SelectAll(0, cFields, cValues, null, null, true,connVM);
                #endregion vehicle

                #region vehicle
                vehicles.Clear();
                cmbVehicleNo.Items.Clear();

                DataRow[] newVehicles = vehicleResult.Select("ActiveStatus='Y'");
                foreach (DataRow item2 in newVehicles)
                {
                    var vehicle = new VehicleDTO();
                    vehicle.VehicleID = item2["VehicleID"].ToString();
                    vehicle.VehicleType = item2["VehicleType"].ToString();
                    vehicle.VehicleNo = item2["VehicleNo"].ToString();
                    vehicle.Description = item2["Description"].ToString();
                    vehicle.Comments = item2["Comments"].ToString();
                    vehicle.ActiveStatus = item2["ActiveStatus"].ToString();
                    vehicle.DriverName = item2["DriverName"].ToString();
                    
                    cmbVehicleNo.Items.Add(item2["VehicleNo"]);
                    vehicles.Add(vehicle);
                }
                cmbVehicleNo.Items.Insert(0, "Select");
                #endregion vehicle
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

        private void FormBanderolReceive_FormClosing(object sender, FormClosingEventArgs e)
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

        

        private void btnPost_Click(object sender, EventArgs e)
        {
            #region try
            try
            {
                //if (Post == false)
                //{
                //    MessageBox.Show(MessageVM.msgNotPostAccess, this.Text);
                //    return;
                //}
                //else 
                    if (
                MessageBox.Show(MessageVM.msgWantToPost + "\n" + MessageVM.msgWantToPost1, this.Text, MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    return;
                }
                else if (IsPost == true)
                {
                    MessageBox.Show(MessageVM.ThisTransactionWasPosted, this.Text);
                    return;
                }
                else if (IsUpdate == false)
                {
                    MessageBox.Show(MessageVM.msgNotPost, this.Text);
                    return;
                }

                if (Program.CheckLicence(dtpRefDate.Value) == true)
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
                    NextID = txtReceiveNo.Text.Trim();
                }



                if (dgvBandeReceive.RowCount <= 0)
                {
                    MessageBox.Show("Please insert Details information for transaction", this.Text, MessageBoxButtons.OK,
                                     MessageBoxIcon.Information);
                    return;
                }


                demandMasterVM = new DemandMasterVM();

                demandMasterVM.DemandNo = NextID.ToString();
                demandMasterVM.DemandDateTime = dtpReceiveDate.Value.ToString("yyyy-MMM-dd HH:mm:ss");
                //demandMasterVM.DemandDateTime = dtpDemandDate.Value.ToString("yyyy-MMM-dd");
                demandMasterVM.LastModifiedBy = Program.CurrentUser;
                demandMasterVM.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                demandMasterVM.Post = "Y";
                demandMasterVM.TransactionType = "Receive";

                demandDetailVMs = new List<DemandDetailVM>();
                for (int i = 0; i < dgvBandeReceive.RowCount; i++)
                {
                    DemandDetailVM detail = new DemandDetailVM();

                    detail.DemandNo = NextID.ToString();
                    detail.DemandLineNo = dgvBandeReceive.Rows[i].Cells["LineNo"].Value.ToString();
                    detail.BandProductId = dgvBandeReceive.Rows[i].Cells["BandProductId"].Value.ToString();
                    ////detail.ItemNo = dgvDemand.Rows[i].Cells["ItemNo"].Value.ToString();
                    //detail.Quantity = Convert.ToDecimal(dgvDemand.Rows[i].Cells["Quantity"].Value.ToString());
                    //detail.NBRPrice = 0;

                    //detail.UOM = dgvDemand.Rows[i].Cells["UOM"].Value.ToString();
                    ////detail.SubTotal = Convert.ToDecimal(dgvDemand.Rows[i].Cells["SubTotal"].Value.ToString());
                    //detail.Comments = dgvDemand.Rows[i].Cells["Comments"].Value.ToString();

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
                            throw new ArgumentNullException("backgroundWorkerPost_RunWorkerCompleted",
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
                            txtReceiveNo.Text = sqlResults[2].ToString();
                            IsPost = Convert.ToString(sqlResults[3].ToString()) == "Y" ? true : false;
                            for (int i = 0; i < dgvBandeReceive.RowCount; i++)
                            {
                                dgvBandeReceive["Status", dgvBandeReceive.RowCount - 1].Value = "Old";
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

        
       
        private void btnAddNew_Click(object sender, EventArgs e)
        {
            ClearAllFields();
            ChangeData = false;
            IsPost = false;
        }

        private void dgvBandeReceive_DoubleClick(object sender, EventArgs e)
        {
            #region try
            try
            {
                if (dgvBandeReceive.Rows.Count <= 0)
                {
                    MessageBox.Show("There is no data to select.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                txtProduct.Text = dgvBandeReceive.CurrentRow.Cells["PCode"].Value.ToString();
                txtProductName.Text = dgvBandeReceive.CurrentRow.Cells["ItemName"].Value.ToString();
                txtUom.Text = dgvBandeReceive.CurrentRow.Cells["UOM"].Value.ToString();
                txtQuantity.Text = Convert.ToDecimal(dgvBandeReceive.CurrentRow.Cells["Quantity"].Value).ToString();

                txtComments.Text = dgvBandeReceive.CurrentRow.Cells["Comments"].Value.ToString();
                txtBanderol.Text = dgvBandeReceive.CurrentRow.Cells["Banderol"].Value.ToString();//"0,0.00");
                txtPackaging.Text = dgvBandeReceive.CurrentRow.Cells["PackNature"].Value.ToString();//"0,0.00");
                txtBandProId.Text = dgvBandeReceive.CurrentRow.Cells["BandProductId"].Value.ToString();
                txtItemNo.Text = dgvBandeReceive.CurrentRow.Cells["ItemNo"].Value.ToString();

                decimal stock = demandDal.BanderolStock(txtItemNo.Text, txtBandProId.Text, dtpReceiveDate.Value.ToString("yyyy-MMM-dd"),connVM);
                txtStock.Text = stock.ToString("0.00");

                decimal returnQty = demandDal.ReturnQty(txtDemandNo.Text, txtBandProId.Text, "Y",connVM);
                //PreviousQty = Convert.ToDecimal(txtQuantity.Text) - returnQty;
                PreviousQty = returnQty;
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
                dgvReceiveHistory.Top = dgvBandeReceive.Top;
                dgvReceiveHistory.Left = dgvBandeReceive.Left;
                dgvReceiveHistory.Height = dgvBandeReceive.Height;
                dgvReceiveHistory.Width = dgvBandeReceive.Width;

                dgvBandeReceive.Visible = false;
                dgvReceiveHistory.Visible = true;


                Program.fromOpen = "Me";
                
                #region Transaction Type
                //TransactionTypes();
                transactionType = "Demand";
                DataGridViewRow selectedRow = new DataGridViewRow();

                selectedRow = FormDemandSearch.SelectOne(transactionType);

                #endregion Transaction Type

                if (selectedRow != null && selectedRow.Selected == true)
                {
                    if (selectedRow.Cells["Post"].Value.ToString() == "N")
                    {
                        MessageBox.Show("this transaction was not posted ", this.Text);
                        return;
                    }
                    string demandNo = selectedRow.Cells["DemandNo"].Value.ToString();
                    txtDemandNo.Text = demandNo;
                    //dtpRefDate.Value = Convert.ToDateTime(selectedRow.Cells["DemandDate"].Value.ToString());
                   
                    //dtpMonthFrom.Text = Convert.ToDateTime(selectedRow.Cells["MonthFrom"].Value.ToString()).ToString();
                    //dtpMonthTo.Text = Convert.ToDateTime(selectedRow.Cells["MonthTo"].Value.ToString()).ToString();

                    //IsPost = Convert.ToString(selectedRow.Cells["Post"].Value.ToString()) == "Y" ? true : false;

                    DemandDetailNo = txtDemandNo.Text == "" ? "" : txtDemandNo.Text.Trim();
                }
                this.btnSearchDemand.Enabled = true;
                this.progressBar1.Visible = false;
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
                FileLogger.Log(this.Name, "bgwSearchDemandNo_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "bgwSearchDemandNo_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "bgwSearchDemandNo_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "bgwSearchDemandNo_DoWork", exMessage);
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
                FileLogger.Log(this.Name, "bgwSearchDemandNo_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwSearchDemandNo_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwSearchDemandNo_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "bgwSearchDemandNo_DoWork", exMessage);
            }
            #endregion
        }

        string packNature, banderolNature;

        private void bgwSearchDemandNo_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                #region Statement
                if (transactionType == "Demand")
                {
                    dgvReceiveHistory.Rows.Clear();
                    int j = 0;
                    foreach (DataRow item in DemandDetailResult.Rows)
                    {
                        DataGridViewRow NewRow = new DataGridViewRow();

                        packNature = item["PackagingName"].ToString() + " - " + item["PackagingSize"].ToString() + " " + item["PackagingUom"].ToString();
                        banderolNature = item["BanderolName"].ToString() + " - " + item["BanderolSize"].ToString() + " " + item["BanderolUom"].ToString();
                        
                        dgvReceiveHistory.Rows.Add(NewRow);
                        dgvReceiveHistory.Rows[j].Cells["LineNoP"].Value = item["DemandLineNo"].ToString();
                        dgvReceiveHistory.Rows[j].Cells["PCodeP"].Value = item["ProductCode"].ToString();
                        dgvReceiveHistory.Rows[j].Cells["ItemNameP"].Value = item["ProductName"].ToString();
                        dgvReceiveHistory.Rows[j].Cells["BanderolP"].Value = banderolNature;
                        dgvReceiveHistory.Rows[j].Cells["PackagingP"].Value = packNature;
                        dgvReceiveHistory.Rows[j].Cells["QuantityP"].Value = item["Quantity"].ToString();
                        dgvReceiveHistory.Rows[j].Cells["UOMP"].Value = item["UOM"].ToString();
                        dgvReceiveHistory.Rows[j].Cells["CommentsP"].Value = item["Comments"].ToString();
                        dgvReceiveHistory.Rows[j].Cells["BandProductIdP"].Value = item["BandProductId"].ToString();
                        dgvReceiveHistory.Rows[j].Cells["ItemNoP"].Value = item["ItemNo"].ToString();

                        j = j + 1;
                    }  //End For
                }
                else
                {
                    dgvBandeReceive.Rows.Clear();
                    int j = 0;
                    foreach (DataRow item in DemandDetailResult.Rows)
                    {
                        DataGridViewRow NewRow = new DataGridViewRow();

                        packNature = item["PackagingName"].ToString() + " - " + item["PackagingSize"].ToString() + " " + item["PackagingUom"].ToString();
                        banderolNature = item["BanderolName"].ToString() + " - " + item["BanderolSize"].ToString() + " " + item["BanderolUom"].ToString();
                        string demandNo = item["DemandReceiveID"].ToString();
                        txtDemandNo.Text = demandNo;


                        dgvBandeReceive.Rows.Add(NewRow);
                        dgvBandeReceive.Rows[j].Cells["LineNo"].Value = item["DemandLineNo"].ToString();
                        dgvBandeReceive.Rows[j].Cells["PCode"].Value = item["ProductCode"].ToString();
                        dgvBandeReceive.Rows[j].Cells["ItemName"].Value = item["ProductName"].ToString();
                        dgvBandeReceive.Rows[j].Cells["Banderol"].Value = banderolNature;
                        dgvBandeReceive.Rows[j].Cells["PackNature"].Value = packNature;
                        dgvBandeReceive.Rows[j].Cells["Quantity"].Value = item["Quantity"].ToString();
                        dgvBandeReceive.Rows[j].Cells["UOM"].Value = item["UOM"].ToString();
                        dgvBandeReceive.Rows[j].Cells["Comments"].Value = item["Comments"].ToString();
                        dgvBandeReceive.Rows[j].Cells["BandProductId"].Value = item["BandProductId"].ToString();
                        dgvBandeReceive.Rows[j].Cells["ItemNo"].Value = item["ItemNo"].ToString();

                        j = j + 1;
                    }  //End For
                    IsUpdate = true;
                }


                //btnSave.Text = "&Save";
                
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
                this.btnSearchReceiveNo.Enabled = true;
                this.progressBar1.Visible = false;
            }
        }

        private void btnForm3_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtReceiveNo.Text == "~~~ New ~~~")
                {
                    MessageBox.Show("Receive information not save.");
                    return;
                }

                if (Program.CheckLicence(dtpReceiveDate.Value) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }
                //btnForm3.Enabled = false;
                //progressBar1.Visible = true;
                //bgwPrint.RunWorkerAsync();
                MDIMainInterface mdi = new MDIMainInterface();
                FormRptForm3 frmRptForm3 = new FormRptForm3();

                if (dgvBandeReceive.Rows.Count > 0)
                {
                    frmRptForm3.txtReceiveNo.Text = txtReceiveNo.Text;
                    frmRptForm3.dtpReceiveFromDate.Value = dtpReceiveDate.Value;
                    frmRptForm3.dtpReceiveToDate.Value = dtpReceiveDate.Value;
                }

                frmRptForm3.ShowDialog();

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
                FileLogger.Log(this.Name, "btnVAT16_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnVAT16_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnVAT16_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnVAT16_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnVAT16_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnVAT16_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnVAT16_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnVAT16_Click", exMessage);
            }
            #endregion

        }

        //
        private void bgwPrint_DoWork(object sender, DoWorkEventArgs e)
        {

            try
            {
                #region Statement
                string receivePost = IsPost == true ? "Y" : "N";
                transactionType = "Receive";
                ReportResult = new DataSet();
                ReportDSDAL reportDsdal = new ReportDSDAL();

                ReportResult = reportDsdal.DemandReport(txtReceiveNo.Text.Trim(), "", "", transactionType, receivePost,connVM);
                #endregion Statement
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

        //
        private void bgwPrint_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                #region Statement
                DBSQLConnection _dbsqlConnection = new DBSQLConnection();
                if (ReportResult.Tables.Count <= 0)
                {
                    MessageBox.Show("There is no data to preview", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                ReportResult.Tables[0].TableName = "DsDemand";
                ReportClass objrpt = new ReportClass();
                objrpt = new RptBanderolReceive();
                
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
                FileLogger.Log(this.Name, "bgwPrint_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "bgwPrint_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "bgwPrint_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "bgwPrint_RunWorkerCompleted", exMessage);
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
                FileLogger.Log(this.Name, "bgwPrint_RunWorkerCompleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwPrint_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwPrint_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "bgwPrint_RunWorkerCompleted", exMessage);
            }
            #endregion

            finally
            {
                btnForm3.Enabled = true;
                progressBar1.Visible = false;
            }
        }

        private void btnSearchVehicle_Click(object sender, EventArgs e)
        {
            //Program.fromOpen = "Other";
            //string result = FormVehicleSearch.SelectOne();
            //if (result == "")
            //{
            //    return;
            //}
            //else //if (result == ""){return;}else//if (result != "")
            //{
            //    string[] VehicleInfo = result.Split(FieldDelimeter.ToCharArray());

            //    txtVehicleId.Text = VehicleInfo[0];
            //    txtVehicleType.Text = VehicleInfo[1];
            //    txtVehicleNo.Text = VehicleInfo[2];
            //    cmbVehicleNo.Text = VehicleInfo[2];
            //    txtDriverName.Text = VehicleInfo[3];
            //}
        }

        private void btnSearchReceiveNo_Click(object sender, EventArgs e)
        {
            #region try
            try
            {

                Program.fromOpen = "Me";
                #region Transaction Type
                //TransactionTypes();
                transactionType = "Receive";
                DataGridViewRow selectedRow = new DataGridViewRow();

                //selectedRow = FormBanderolReceiveSearch.SelectOne(transactionType);
                selectedRow = FormDemandSearch.SelectOne(transactionType);

                #endregion Transaction Type

                if (selectedRow != null && selectedRow.Selected == true)
                {
                    string receiveNo = selectedRow.Cells["DemandNo"].Value.ToString();
                    txtReceiveNo.Text = receiveNo; 
                    //txtDemandNo.Text = selectedRow.Cells["DemandNo"].Value.ToString();
                    dtpRefDate.Value = Convert.ToDateTime(selectedRow.Cells["RefDate"].Value.ToString());
                    dtpReceiveDate.Value = Convert.ToDateTime(selectedRow.Cells["ReceiveDate"].Value.ToString());
                    txtRefNo.Text = selectedRow.Cells["RefNo"].Value.ToString();
                    txtVehicleId.Text = selectedRow.Cells["VehicleID"].Value.ToString();
                    txtVehicleNo.Text = selectedRow.Cells["VehicleNo"].Value.ToString();
                    txtVehicleType.Text = selectedRow.Cells["VehicleType"].Value.ToString();
                    txtDriverName.Text = selectedRow.Cells["DriverName"].Value.ToString();
                    cmbVehicleNo.Text = selectedRow.Cells["VehicleNo"].Value.ToString();
                    IsPost = Convert.ToString(selectedRow.Cells["Post"].Value.ToString()) == "Y" ? true : false;

                    DemandDetailNo = txtReceiveNo.Text == "" ? "" : txtReceiveNo.Text.Trim();
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

        private void dgvReceiveHistory_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode.Equals(Keys.F2))
                {
                    dgvBandeReceive.Rows.Clear();
                    int j = 0;
                    for (int i = 0; i < dgvReceiveHistory.Rows.Count; i++)
                    {
                        if (Convert.ToBoolean(dgvReceiveHistory["Select", i].Value) == true)
                        {
                            DataGridViewRow NewRow = new DataGridViewRow();

                            dgvBandeReceive.Rows.Add(NewRow);

                            dgvBandeReceive.Rows[j].Cells["LineNo"].Value = j + 1;
                            dgvBandeReceive.Rows[j].Cells["PCode"].Value = dgvReceiveHistory["PCodeP", i].Value.ToString();
                            dgvBandeReceive.Rows[j].Cells["ItemNo"].Value = dgvReceiveHistory["ItemNoP", i].Value.ToString();
                            dgvBandeReceive.Rows[j].Cells["ItemName"].Value = dgvReceiveHistory["ItemNameP", i].Value.ToString();
                            dgvBandeReceive.Rows[j].Cells["Banderol"].Value = dgvReceiveHistory["BanderolP", i].Value.ToString();
                            dgvBandeReceive.Rows[j].Cells["PackNature"].Value = dgvReceiveHistory["PackagingP", i].Value.ToString();
                            dgvBandeReceive.Rows[j].Cells["Quantity"].Value = dgvReceiveHistory["QuantityP", i].Value.ToString();
                            dgvBandeReceive.Rows[j].Cells["UOM"].Value = dgvReceiveHistory["UOMP", i].Value.ToString();
                            dgvBandeReceive.Rows[j].Cells["BandProductId"].Value = dgvReceiveHistory["BandProductIdP", i].Value.ToString();
                            dgvBandeReceive.Rows[j].Cells["Comments"].Value = dgvReceiveHistory["CommentsP", i].Value.ToString();

                            j++;
                        }
                    }
                    dgvReceiveHistory.Visible = false;
                    dgvBandeReceive.Visible = true;
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
                FileLogger.Log(this.Name, "dgvReceiveHistory_KeyDown", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "dgvReceiveHistory_KeyDown", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "dgvReceiveHistory_KeyDown", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "dgvReceiveHistory_KeyDown", exMessage);
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
                FileLogger.Log(this.Name, "dgvReceiveHistory_KeyDown", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "dgvReceiveHistory_KeyDown", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "dgvReceiveHistory_KeyDown", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "dgvReceiveHistory_KeyDown", exMessage);
            }
            #endregion
        }

        private void cmbVehicleNo_Leave(object sender, EventArgs e)
        {
            try
            {
                #region Statement


                // txtVehicleNo.Text = "";
                txtVehicleType.ReadOnly = true;

                var searchText = cmbVehicleNo.Text.Trim().ToLower();
                if (!string.IsNullOrEmpty(searchText) && searchText != "select")
                {
                    var vehicleNo = from prd in vehicles.ToList()
                                    where prd.VehicleNo.ToLower() == searchText.ToLower()
                                    select prd;
                    if (vehicleNo != null && vehicleNo.Any())
                    {
                        var vihicles = vehicleNo.First();
                        txtVehicleNo.Text = vihicles.VehicleNo;
                        txtVehicleType.Text = vihicles.VehicleType;
                        txtDriverName.Text = vihicles.DriverName;
                        //dtpInvoiceDate.Focus();
                    }
                    else
                    {
                        chkVehicleSaveInDatabase.Checked = true;
                        if (
                        MessageBox.Show("vehicle Number not in database, Add new Vehicle?", this.Text, MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question) != DialogResult.Yes)
                        {

                            chkVehicleSaveInDatabase.Checked = false;

                        }
                        txtVehicleType.ReadOnly = false;

                        //txtVehicleType.Focus();
                    }
                }


                #endregion

            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "cmbVehicleNo_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "cmbVehicleNo_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "cmbVehicleNo_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "cmbVehicleNo_Leave", exMessage);
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
                FileLogger.Log(this.Name, "cmbVehicleNo_Leave", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "cmbVehicleNo_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "cmbVehicleNo_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "cmbVehicleNo_Leave", exMessage);
            }

            #endregion
        }

        private void dgvReceiveHistory_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                dgvReceiveHistory["Select", e.RowIndex].Value = !Convert.ToBoolean(dgvReceiveHistory["Select", e.RowIndex].Value);
            }
        }

        //
        private void btnForm4_Click(object sender, EventArgs e)
        {
            try
            {
                #region Statement

                if (Program.CheckLicence(dtpReceiveDate.Value) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }
                MDIMainInterface mdi = new MDIMainInterface();
                FormRptForm4 frmRptForm4 = new FormRptForm4();
                              
                if (dgvBandeReceive.Rows.Count > 0)
                {
                    DataTable dtBanderol=new DataTable();
                    BanderolProductsDAL banderolProDal=new BanderolProductsDAL();
                    dtBanderol = banderolProDal.SearchBanderol(dgvBandeReceive.CurrentRow.Cells["BandProductId"].Value.ToString(),connVM);
                    frmRptForm4.txtBanderolId.Text = dtBanderol.Rows[0]["BanderolId"].ToString();
                    frmRptForm4.txtBanderolName.Text = dtBanderol.Rows[0]["BanderolName"].ToString();
                    frmRptForm4.dtpFromDate.Value = dtpReceiveDate.Value;
                    frmRptForm4.dtpToDate.Value = dtpReceiveDate.Value;
                }

                frmRptForm4.ShowDialog();

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
                FileLogger.Log(this.Name, "btnVAT16_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnVAT16_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnVAT16_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnVAT16_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnVAT16_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnVAT16_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnVAT16_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnVAT16_Click", exMessage);
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
                MessageBox.Show("Please enter numeric value.", "Quantity", MessageBoxButtons.OK,
                                MessageBoxIcon.Information);


                txtQuantity.Text = "0";
            }
        }

        private void dtpReceiveDate_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void txtRefNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void cmbVehicleNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void txtVehicleType_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void txtDriverName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void dtpRefDate_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void txtQuantity_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void txtStock_Leave(object sender, EventArgs e)
        {
            Program.FormatTextBox(txtStock, "Stock");
        }

        
    }
}
