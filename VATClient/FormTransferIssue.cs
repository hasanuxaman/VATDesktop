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
using VATServer.Interface;
using VATServer.Ordinary;
using VATDesktop.Repo;
using CrystalDecisions.CrystalReports.Engine;
using SymphonySofttech.Reports;
using System.Drawing.Printing;
using VATClient.Integration.EON;
using VATClient.Integration.Berger;
namespace VATClient
{
    public partial class FormTransferIssue : Form
    {
        #region Constructors
        public FormTransferIssue()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();
            //connVM.SysdataSource = SysDBInfoVM.SysdataSource;
            //connVM.SysPassword = SysDBInfoVM.SysPassword;
            //connVM.SysUserName = SysDBInfoVM.SysUserName;
            //connVM.SysDatabaseName = DatabaseInfoVM.DatabaseName;
            //SetupBOMIssue();
            //if (Program.IssueFromBOM == "Y")
            //{
            //    MessageBox.Show("Raw product is issue with Finish product receiving", this.Text, MessageBoxButtons.OK,
            //                    MessageBoxIcon.Information);
            //    //this.Close();
            //    return;
            //}
        }
        #endregion

        #region Global Variables
        CommonDAL commonDal = new CommonDAL();
        public static string vItemNo = "0";

        private bool CustomerWiseBOM = false;
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();

        //private ReportDocument reportDocument = new ReportDocument();

        List<ProductMiniDTO> ProductsMini = new List<ProductMiniDTO>();
        private string[] sqlResults;
        private bool SAVE_DOWORK_SUCCESS = false;
        private bool DELETE_DOWORK_SUCCESS = false;
        private bool UPDATE_DOWORK_SUCCESS = false;
        private bool POST_DOWORK_SUCCESS = false;
        string transactionType = string.Empty;
        private const string LineDelimeter = DBConstant.LineDelimeter;
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private static string PassPhrase = DBConstant.PassPhrase;
        private static string EnKey = DBConstant.EnKey;
        string encriptedIssueHeaderData;
        private string NextID = string.Empty;
        private bool ChangeData = false;
        private DataTable ProductResultDs;
        public string VFIN = "221";
        private bool IsUpdate = false;
        private bool Post = false;
        private bool IsPost = false;
        private string VPrinterName = string.Empty;
        private int AlReadyPrintNo;
        private bool PreviewOnly;
        private string WantToPrint = "N";
        private string CategoryId { get; set; }
        private string UOMIdParam = "";
        private string UOMFromParam = "";
        private string UOMToParam = "";
        private string ActiveStatusUOMParam = "";
        private DataTable uomResult;
        List<UomDTO> UOMs = new List<UomDTO>();
        DataSet formLoadDS = new DataSet();
        private string branchName = "";
        private DataTable dtbranchNames;
        List<BranchDTO> BranchLists = new List<BranchDTO>();
        //List
        private int IssuePlaceQty;
        private int IssuePlaceAmt;
        private BOMNBRVM bomNbrs = new BOMNBRVM();
        private List<BOMOHVM> bomOhs = new List<BOMOHVM>();
        private List<BOMItemVM> bomItems = new List<BOMItemVM>();

        private int SearchBranchId = 0;

        #region Vabiables for tracking
        List<TrackingCmbDTO> trackingsCmb = new List<TrackingCmbDTO>();
        List<TrackingVM> trackingsVm = new List<TrackingVM>();
        private bool TrackingTrace;
        private string Heading1 = string.Empty;
        private string Heading2 = string.Empty;

        #endregion

        #region Global Variables For BackGroundWorker
        private string IssueHeaderData = string.Empty;
        private string IssueDetailData = string.Empty;
        private string IssueResult = string.Empty;
        private string IssueResultPost = string.Empty;
        private DataTable IssueDetailResult;
        private string ProductData = string.Empty;
        private string varItemNo, varCategoryID, varIsRaw, varHSCodeNo, varActiveStatus, varTrading, varNonStock, varProductCode;
        private TransferIssueVM transferIssueVM;
        private List<TransferIssueDetailVM> transferIssueDetailVMs = new List<TransferIssueDetailVM>();
        private DataSet StatusResult;
        private DataTable ProductTypeResult;
        private bool Edit = false;
        private bool Add = false;
        #endregion
        #endregion

        #region Methods 01 / Save, Update, Post, Search

        private void cmbProduct_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void picIssue_Click(object sender, EventArgs e)
        {
        }

        private void TransactionTypes()
        {
            #region Transaction Type
            transactionType = string.Empty;
            if (rbtn62Out.Checked)
            {
                transactionType = "62Out";
            }
            else if (rbtn61Out.Checked)
            {
                transactionType = "61Out";
            }
            #endregion Transaction Type
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            #region try
            try
            {

                #region Checkpoint

                #region Null Check

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
                if (Program.CheckLicence(dtpIssueDate.Value) == true)
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
                    NextID = txtIssueNo.Text.Trim();
                }
                if (txtComments.Text == "")
                {
                    txtComments.Text = "-";
                }
                if (txtSerialNo.Text == "")
                {
                    txtSerialNo.Text = "-";
                }
                if (txtReferenceNo.Text == "")
                {
                    txtReferenceNo.Text = "-";
                }

                #endregion

                #region Exist Check

                TransferIssueVM vm = new TransferIssueVM();
                if (!string.IsNullOrWhiteSpace(NextID))
                {
                    vm = new TransferIssueDAL().SelectAllList(0, new[] { "ti.TransferIssueNo" }, new[] { NextID },null,null,connVM).FirstOrDefault();
                }

                if (vm != null && !string.IsNullOrWhiteSpace(vm.Id))
                {
                    throw new Exception("This Invoice Already Exist! Cannot Add!" + Environment.NewLine + "Invoice No: " + NextID);

                }

                #endregion

                //int TransferToBranchId = 0;
                //TransferToBranchId = Convert.ToInt32(cmbTransferTo.SelectedValue);

                if (string.IsNullOrEmpty(cmbShift.Text.Trim()))
                {
                    MessageBox.Show("Please Select Shift");
                    cmbShift.Focus();
                    return;
                }
                #region TransferToBranchId
                int TransferToBranchId = 0;
                TransferToBranchId = Convert.ToInt32(txtTransferToId.Text.Trim());
                if (TransferToBranchId == 0 )
                {
                    MessageBox.Show("Please Enter Branch Name to Transfer");
                    txtTransferto.Focus();
                    return;
                }
                #endregion

                //if (TransferToBranchId == 0)
                //{
                //    MessageBox.Show("Please Enter Branch Name to Transfer");
                //    cmbTransferTo.Focus();
                //    return;
                //}

                if (dgvIssue.RowCount <= 0)
                {
                    MessageBox.Show("Please insert Details information for transaction", this.Text, MessageBoxButtons.OK,
                                     MessageBoxIcon.Information);
                    return;
                }

                #region vehicleRequired
                CommonDAL commonDal = new CommonDAL();
                string vehicleRequired = commonDal.settingsDesktop("Sale", "VehicleRequired",null,connVM);
                if (vehicleRequired.ToLower() == "y")
                {

                    if (txtVehicleNo.Text == "")
                    {
                        MessageBox.Show("Please Enter Vehicle Number");
                        txtVehicleNo.Focus();
                        return;
                    }

                }
                else
                {
                    if (txtVehicleNo.Text == "")
                    {
                        txtVehicleNo.Text = "-";
                    }

                }


                #endregion



                #endregion

                #region Transaction Type

                TransactionTypes();

                #endregion

                #region Tracking Check

                if (TrackingTrace == true)
                {
                    if (trackingsVm.Count <= 0)
                    {
                        if (DialogResult.Yes != MessageBox.Show(

                        "Tracking information have not been added ." + "\n" + " Want to save without Tracking ?",
                        this.Text,
                         MessageBoxButtons.YesNo,
                         MessageBoxIcon.Question,
                         MessageBoxDefaultButton.Button2))
                        {
                            btnTracking_Click(sender, e);
                            return;
                        }

                    }

                }

                #endregion

                #region Value Assign

                #region Master Assign

                transferIssueVM = new TransferIssueVM();
                transferIssueVM.TransferIssueNo = NextID.ToString();
                transferIssueVM.TransactionDateTime = dtpIssueDate.Value.ToString("yyyy-MMM-dd HH:mm");
                transferIssueVM.TotalAmount = Convert.ToDecimal(txtTotalAmount.Text.Trim());
                transferIssueVM.SerialNo = txtSerialNo.Text.Trim().Replace(" ", "");
                transferIssueVM.ReferenceNo = txtReferenceNo.Text.Trim().Replace(" ", "");
                transferIssueVM.Comments = txtComments.Text.Trim();
                transferIssueVM.TransferTo = TransferToBranchId;//// txtBranchDBName.Text.Trim();
                transferIssueVM.CreatedBy = Program.CurrentUser;
                transferIssueVM.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                transferIssueVM.LastModifiedBy = Program.CurrentUser;
                transferIssueVM.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                transferIssueVM.TransactionType = transactionType;
                transferIssueVM.BranchId = Program.BranchId;
                transferIssueVM.VehicleNo = txtVehicleNo.Text.Trim();
                transferIssueVM.VehicleType = txtVehicleType.Text.Trim();
                transferIssueVM.TripNo = txtTrifNo.Text.Trim();

                //transferIssueVM.TotalVATAmount = Convert.ToDecimal(txtTotalVATAmount.Text.Trim());
                //transferIssueVM.TotalSDAmount = Convert.ToDecimal(txtTotalSDAmount.Text.Trim());
                transferIssueVM.Post = "N";
                transferIssueVM.ShiftId = Convert.ToInt32(cmbShift.SelectedValue);
                #region Find UserInfo

                DataRow[] userInfo = settingVM.UserInfoDT.Select("UserID='" + Program.CurrentUserID + "'");
                transferIssueVM.SignatoryName = userInfo[0]["FullName"].ToString();
                transferIssueVM.SignatoryDesig = userInfo[0]["Designation"].ToString();
                #endregion
                #endregion

                #region Details Assign

                transferIssueDetailVMs = new List<TransferIssueDetailVM>();
                for (int i = 0; i < dgvIssue.RowCount; i++)
                {
                    TransferIssueDetailVM detail = new TransferIssueDetailVM();
                    detail.TransferIssueNo = NextID.ToString();
                    detail.BOMId = Convert.ToInt32(dgvIssue.Rows[i].Cells["BOMId"].Value);

                    detail.IssueLineNo = dgvIssue.Rows[i].Cells["LineNo"].Value.ToString();
                    detail.ItemNo = dgvIssue.Rows[i].Cells["ItemNo"].Value.ToString();
                    detail.ItemName = dgvIssue.Rows[i].Cells["ItemName"].Value.ToString();
                    detail.Quantity = Convert.ToDecimal(dgvIssue.Rows[i].Cells["Quantity"].Value.ToString());
                    detail.CostPrice = Convert.ToDecimal(dgvIssue.Rows[i].Cells["UnitPrice"].Value.ToString());
                    detail.UOM = dgvIssue.Rows[i].Cells["UOM"].Value.ToString();
                    detail.SubTotal = Convert.ToDecimal(dgvIssue.Rows[i].Cells["SubTotal"].Value.ToString());
                    detail.Comments = "FromTransferIssue";
                    detail.TransactionDateTime = dtpIssueDate.Value.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss");
                    detail.TransferTo = TransferToBranchId; ////txtBranchDBName.Text.Trim();
                    detail.UOMQty = Convert.ToDecimal(dgvIssue.Rows[i].Cells["UOMQty"].Value.ToString());
                    detail.UOMn = dgvIssue.Rows[i].Cells["UOMn"].Value.ToString();
                    detail.UOMc = Convert.ToDecimal(dgvIssue.Rows[i].Cells["UOMc"].Value.ToString());
                    detail.UOMPrice = Convert.ToDecimal(dgvIssue.Rows[i].Cells["UOMPrice"].Value.ToString());
                    detail.VATRate = Convert.ToDecimal(dgvIssue.Rows[i].Cells["VATRate"].Value.ToString());
                    detail.VATAmount = Convert.ToDecimal(dgvIssue.Rows[i].Cells["VATAmount"].Value.ToString());
                    detail.SDRate = Convert.ToDecimal(dgvIssue.Rows[i].Cells["SDRate"].Value.ToString());
                    detail.SDAmount = Convert.ToDecimal(dgvIssue.Rows[i].Cells["SDAmount"].Value.ToString());
                    detail.Weight = dgvIssue.Rows[i].Cells["Weight"].Value.ToString();
                    detail.BranchId = Program.BranchId;

                    transferIssueDetailVMs.Add(detail);
                }
                #endregion

                #endregion

                #region Checkpoint

                if (transferIssueDetailVMs.Count() <= 0)
                {
                    MessageBox.Show("Please insert Details information for transaction", this.Text, MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                    return;
                }

                #endregion

                #region Value Assign

                SearchBranchId = Program.BranchId;
                sqlResults = new string[4];

                #endregion

                #region Tracking


                if (TrackingTrace == true)
                {
                    for (int i = 0; i < dgvIssue.Rows.Count; i++)
                    {
                        string itemNo = dgvIssue.Rows[i].Cells["ItemNo"].Value.ToString();
                        decimal Quantity = Convert.ToDecimal(dgvIssue.Rows[i].Cells["Quantity"].Value.ToString());

                        var p = from productCmb in trackingsVm.ToList()
                                where productCmb.FinishItemNo == itemNo
                                select productCmb;

                        if (p != null && p.Any())
                        {
                            var trackingInfo = p.First();
                            decimal fQty = trackingInfo.Quantity;

                            if (Quantity > fQty || Quantity < fQty)
                            {
                                MessageBox.Show("Please insert correct number of tracking.", this.Text, MessageBoxButtons.OK,
                                     MessageBoxIcon.Information);
                                return;
                            }
                        }
                    }

                }

                #endregion

                #region Button Stats

                this.btnSave.Enabled = false;
                this.progressBar1.Visible = true;

                #endregion

                #region Save Data

                //TransferIssueDAL issueDal = new TransferIssueDAL();
                ITransferIssue issueDal = OrdinaryVATDesktop.GetObject<TransferIssueDAL, TransferIssueRepo, ITransferIssue>(OrdinaryVATDesktop.IsWCF);

                sqlResults = issueDal.Insert(transferIssueVM, transferIssueDetailVMs, null, null, connVM, trackingsVm);

                #endregion

                #region Statement

                if (sqlResults.Length > 0)
                {
                    string result = sqlResults[0];
                    string message = sqlResults[1];
                    string newId = sqlResults[2];
                    if (string.IsNullOrEmpty(result))
                    {
                        throw new ArgumentNullException("Insert",
                                                        "Unexpected error.");
                    }
                    else if (result == "Success" || result == "Fail")
                    {
                        MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        IsUpdate = true;
                        ChangeData = false;
                    }
                    if (result == "Success")
                    {
                        txtIssueNo.Text = sqlResults[2].ToString();
                        IsPost = Convert.ToString(sqlResults[3].ToString()) == "Y" ? true : false;
                        for (int i = 0; i < dgvIssue.RowCount; i++)
                        {
                            dgvIssue["Status", dgvIssue.RowCount - 1].Value = "Old";
                        }
                    }
                }

                ChangeData = false;

                #endregion

            }

            #endregion

            #region catch

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

            #region Finally
            finally
            {
                ChangeData = false;
                this.btnSave.Enabled = true;
                this.progressBar1.Visible = false;
            }
            #endregion Finally
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {

                #region Checkpoint

                #region Null Check

                #region Comment 28 Oct 2020

                //////if (IsPost == true)
                //////{
                //////    MessageBox.Show(MessageVM.ThisTransactionWasPosted, this.Text);
                //////    return;
                //////}
                if (IsUpdate)
                {
                    DataTable dt = new TransferReceiveDAL().SelectAll(0, new[] { "tr.TransferFromNo" }, new[] { txtIssueNo.Text.Trim() });
                    if (dt.Rows.Count > 0)
                    {
                        MessageBox.Show(MessageVM.UnableUpdate+", This Invoice Already Received!", this.Text);
                        return;
                    }

                }
                #endregion

                if (SearchBranchId != Program.BranchId)
                {
                    MessageBox.Show(MessageVM.SelfBranchNotMatch, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }


                if (Program.CheckLicence(dtpIssueDate.Value) == true)
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
                    NextID = txtIssueNo.Text.Trim();
                }


                if (txtComments.Text == "")
                {
                    txtComments.Text = "-";
                }
                if (txtSerialNo.Text == "")
                {
                    txtSerialNo.Text = "-";
                }
                if (txtReferenceNo.Text == "")
                {
                    txtReferenceNo.Text = "-";
                }

                //int TransferToBranchId = 0;
                //TransferToBranchId = Convert.ToInt32(cmbTransferTo.SelectedValue);

                #region TransferToBranchId
                int TransferToBranchId = 0;
                TransferToBranchId = Convert.ToInt32(txtTransferToId.Text.Trim());
                if (TransferToBranchId == 0)
                {
                    MessageBox.Show("Please Enter Branch Name to Transfer");
                    txtTransferto.Focus();
                    return;
                }
                #endregion

                if (TransferToBranchId == 0)
                {
                    MessageBox.Show("Please Enter Branch Name to Transfer");
                    cmbTransferTo.Focus();
                    return;
                }
                if (dgvIssue.RowCount <= 0)
                {
                    MessageBox.Show("Please insert Details information for transaction", this.Text, MessageBoxButtons.OK,
                                     MessageBoxIcon.Information);
                    return;
                }

                #region vehicleRequired

                CommonDAL commonDal = new CommonDAL();
                string vehicleRequired = commonDal.settingsDesktop("Sale", "VehicleRequired",null,connVM);
                if (vehicleRequired.ToLower() == "y")
                {

                    if (txtVehicleNo.Text == "")
                    {
                        MessageBox.Show("Please Enter Vehicle Number");
                        txtVehicleNo.Focus();
                        return;
                    }

                }

                else
                {
                    if (txtVehicleNo.Text == "")
                    {
                        txtVehicleNo.Text = "-";
                    }

                }


                #endregion

                #endregion

                #endregion

                #region Transaction Types

                TransactionTypes();

                #endregion

                #region Value Assign

                #region Assign Master Data

                transferIssueVM = new TransferIssueVM();

                transferIssueVM.BranchId = Program.BranchId;
                transferIssueVM.TransferIssueNo = NextID.ToString();
                transferIssueVM.TransactionDateTime = dtpIssueDate.Value.ToString("yyyy-MMM-dd HH:mm");
                transferIssueVM.TotalAmount = Convert.ToDecimal(txtTotalAmount.Text.Trim());
                transferIssueVM.SerialNo = txtSerialNo.Text.Trim().Replace(" ", "");
                transferIssueVM.ReferenceNo = txtReferenceNo.Text.Trim().Replace(" ", "");
                transferIssueVM.Comments = txtComments.Text.Trim();
                transferIssueVM.TransferTo = TransferToBranchId;
                transferIssueVM.CreatedBy = Program.CurrentUser;
                transferIssueVM.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                transferIssueVM.LastModifiedBy = Program.CurrentUser;
                transferIssueVM.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                transferIssueVM.TransactionType = transactionType;
                transferIssueVM.Post = "N";
                transferIssueVM.VehicleNo = txtVehicleNo.Text.Trim();
                transferIssueVM.VehicleType = txtVehicleType.Text.Trim();
                transferIssueVM.ShiftId = Convert.ToInt32(cmbShift.SelectedValue);
                #endregion

                #region Find UserInfo

                DataRow[] userInfo = settingVM.UserInfoDT.Select("UserID='" + Program.CurrentUserID + "'");
                transferIssueVM.SignatoryName = userInfo[0]["FullName"].ToString();
                transferIssueVM.SignatoryDesig = userInfo[0]["Designation"].ToString();

                #endregion

                #region Assign Detail Data

                transferIssueDetailVMs = new List<TransferIssueDetailVM>();
                for (int i = 0; i < dgvIssue.RowCount; i++)
                {
                    TransferIssueDetailVM detail = new TransferIssueDetailVM();

                    detail.BranchId = Program.BranchId;

                    detail.TransferIssueNo = NextID.ToString();
                    detail.BOMId = Convert.ToInt32(dgvIssue.Rows[i].Cells["BOMId"].Value);

                    detail.IssueLineNo = dgvIssue.Rows[i].Cells["LineNo"].Value.ToString();
                    detail.ItemNo = dgvIssue.Rows[i].Cells["ItemNo"].Value.ToString();
                    detail.ItemName = dgvIssue.Rows[i].Cells["ItemName"].Value.ToString();
                    detail.Quantity = Convert.ToDecimal(dgvIssue.Rows[i].Cells["Quantity"].Value.ToString());
                    detail.CostPrice = Convert.ToDecimal(dgvIssue.Rows[i].Cells["UnitPrice"].Value.ToString());
                    detail.UOM = dgvIssue.Rows[i].Cells["UOM"].Value.ToString();
                    detail.SubTotal = Convert.ToDecimal(dgvIssue.Rows[i].Cells["SubTotal"].Value.ToString());
                    detail.Comments = "FromTransferIssue";
                    //detail.ReceiveNoD = NextID.ToString();
                    detail.TransactionDateTime = dtpIssueDate.Value.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss");
                    detail.TransferTo = TransferToBranchId;
                    detail.UOMQty = Convert.ToDecimal(dgvIssue.Rows[i].Cells["UOMQty"].Value.ToString());
                    detail.UOMn = dgvIssue.Rows[i].Cells["UOMn"].Value.ToString();
                    detail.UOMc = Convert.ToDecimal(dgvIssue.Rows[i].Cells["UOMc"].Value.ToString());
                    detail.UOMPrice = Convert.ToDecimal(dgvIssue.Rows[i].Cells["UOMPrice"].Value.ToString());

                    detail.VATRate = Convert.ToDecimal(dgvIssue.Rows[i].Cells["VATRate"].Value.ToString());
                    detail.VATAmount = Convert.ToDecimal(dgvIssue.Rows[i].Cells["VATAmount"].Value.ToString());
                    detail.SDRate = Convert.ToDecimal(dgvIssue.Rows[i].Cells["SDRate"].Value.ToString());
                    detail.SDAmount = Convert.ToDecimal(dgvIssue.Rows[i].Cells["SDAmount"].Value.ToString());
                    detail.Weight = dgvIssue.Rows[i].Cells["Weight"].Value.ToString();
                    //detail.OtherRef = dgvIssue.Rows[i].Cells["OtherRef"].Value.ToString();


                    transferIssueDetailVMs.Add(detail);
                }// End For

                if (transferIssueDetailVMs.Count() <= 0)
                {
                    MessageBox.Show("Please insert Details information for transaction", this.Text, MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                    return;
                }

                #endregion

                #endregion

                #region Button Stats

                this.btnUpdate.Enabled = false;
                this.progressBar1.Visible = true;

                #endregion

                #region Background Worker Update

                //////bgwUpdate.RunWorkerAsync();

                #endregion

                #region Update

                sqlResults = new string[4];
                //TransferIssueDAL issueDal = new TransferIssueDAL();
                ITransferIssue issueDal = OrdinaryVATDesktop.GetObject<TransferIssueDAL, TransferIssueRepo, ITransferIssue>(OrdinaryVATDesktop.IsWCF);

                sqlResults = issueDal.Update(transferIssueVM, transferIssueDetailVMs, connVM, Program.CurrentUserID);

                #endregion

                #region Statement

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
                        txtIssueNo.Text = sqlResults[2].ToString();
                        IsPost = Convert.ToString(sqlResults[3].ToString()) == "Y" ? true : false;
                        for (int i = 0; i < dgvIssue.RowCount; i++)
                        {
                            dgvIssue["Status", dgvIssue.RowCount - 1].Value = "Old";
                        }
                    }
                }
                ChangeData = false;
                #endregion

            }

            #region catch

            catch (Exception ex)
            {
                string exMessage = ex.Message + Environment.NewLine + ex.StackTrace;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;
                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwUpdate_DoWork", exMessage);
            }
            #endregion

            #region Finally
            finally
            {
                ChangeData = false;
                this.btnUpdate.Enabled = true;
                this.progressBar1.Visible = false;
            }
            #endregion Finally

        }

        private void btnPost_Click(object sender, EventArgs e)
        {
            #region try
            try
            {
                #region Checkpoint

                if (IsPost == true)
                {
                    MessageBox.Show(MessageVM.ThisTransactionWasPosted, this.Text);
                    return;
                }
                else if (MessageBox.Show(MessageVM.msgWantToPost + "\n" + MessageVM.msgWantToPost1, this.Text, MessageBoxButtons.YesNo,
                               MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    return;
                }
                else if (IsUpdate == false)
                {
                    MessageBox.Show(MessageVM.msgNotPost, this.Text);
                    return;
                }

                if (SearchBranchId != Program.BranchId)
                {
                    MessageBox.Show(MessageVM.SelfBranchNotMatch, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (Program.CheckLicence(dtpIssueDate.Value) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }

                #endregion

                #region comments

                //////TransactionTypes();
                //////if (IsUpdate == false)
                //////{
                //////    NextID = string.Empty;
                //////}
                //////else
                //////{
                //////    NextID = txtIssueNo.Text.Trim();
                //////}
                //////if (txtComments.Text == "")
                //////{
                //////    txtComments.Text = "-";
                //////}
                //////if (txtSerialNo.Text == "")
                //////{
                //////    txtSerialNo.Text = "-";
                //////}
                //////if (txtReferenceNo.Text == "")
                //////{
                //////    txtReferenceNo.Text = "-";
                //////}
                //////if (txtBranchDBName.Text == "")
                //////{
                //////    MessageBox.Show("Please Enter Branch Name to Transfer");
                //////    cmbTransferTo.Focus();
                //////    return;
                //////}
                //////if (dgvIssue.RowCount <= 0)
                //////{
                //////    MessageBox.Show("Please insert Details information for transaction", this.Text, MessageBoxButtons.OK,
                //////                     MessageBoxIcon.Information);
                //////    return;
                //////}
                //////transferIssueVM = new TransferIssueVM();
                //////transferIssueVM.TransferIssueNo = NextID.ToString();
                //////transferIssueVM.TransactionDateTime = dtpIssueDate.Value.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss");
                //////transferIssueVM.TotalAmount = Convert.ToDecimal(txtTotalAmount.Text.Trim());
                //////transferIssueVM.SerialNo = txtSerialNo.Text.Trim();
                //////transferIssueVM.ReferenceNo = txtReferenceNo.Text.Trim();
                //////transferIssueVM.Comments = txtComments.Text.Trim();
                //////transferIssueVM.TransferTo = txtBranchDBName.Text.Trim();
                //////transferIssueVM.CreatedBy = Program.CurrentUser;
                //////transferIssueVM.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                //////transferIssueVM.LastModifiedBy = Program.CurrentUser;
                //////transferIssueVM.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                ////////transferIssueVM.ReceiveNo = NextID.ToString();
                //////transferIssueVM.TransactionType = transactionType;
                //////transferIssueVM.Post = "Y";
                ////////transferIssueVM.ReturnId = txtIssueNoP.Text.Trim();
                //////transferIssueDetailVMs = new List<TransferIssueDetailVM>();
                //////for (int i = 0; i < dgvIssue.RowCount; i++)
                //////{
                //////    TransferIssueDetailVM detail = new TransferIssueDetailVM();
                //////    detail.TransferIssueNo = NextID.ToString();
                //////    detail.IssueLineNo = dgvIssue.Rows[i].Cells["LineNo"].Value.ToString();
                //////    detail.ItemNo = dgvIssue.Rows[i].Cells["ItemNo"].Value.ToString();
                //////    detail.Quantity = Convert.ToDecimal(dgvIssue.Rows[i].Cells["Quantity"].Value.ToString());
                //////    detail.CostPrice = Convert.ToDecimal(dgvIssue.Rows[i].Cells["UnitPrice"].Value.ToString());
                //////    detail.UOM = dgvIssue.Rows[i].Cells["UOM"].Value.ToString();
                //////    detail.SubTotal = Convert.ToDecimal(dgvIssue.Rows[i].Cells["SubTotal"].Value.ToString());
                //////    detail.Comments = "FromTransferIssue";
                //////    detail.TransactionDateTime = dtpIssueDate.Value.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss");
                //////    detail.TransferTo = txtBranchDBName.Text.Trim();
                //////    detail.UOMQty = Convert.ToDecimal(dgvIssue.Rows[i].Cells["UOMQty"].Value.ToString());
                //////    detail.UOMn = dgvIssue.Rows[i].Cells["UOMn"].Value.ToString();
                //////    detail.UOMc = Convert.ToDecimal(dgvIssue.Rows[i].Cells["UOMc"].Value.ToString());
                //////    detail.UOMPrice = Convert.ToDecimal(dgvIssue.Rows[i].Cells["UOMPrice"].Value.ToString());

                //////    detail.VATRate = Convert.ToDecimal(dgvIssue.Rows[i].Cells["VATRate"].Value.ToString());
                //////    detail.VATAmount = Convert.ToDecimal(dgvIssue.Rows[i].Cells["VATAmount"].Value.ToString());
                //////    detail.SDRate = Convert.ToDecimal(dgvIssue.Rows[i].Cells["SDRate"].Value.ToString());
                //////    detail.SDAmount = Convert.ToDecimal(dgvIssue.Rows[i].Cells["SDAmount"].Value.ToString());

                //////    transferIssueDetailVMs.Add(detail);
                //////}
                //////if (transferIssueDetailVMs.Count() <= 0)
                //////{
                //////    MessageBox.Show("Please insert Details information for transaction", this.Text, MessageBoxButtons.OK,
                //////                    MessageBoxIcon.Information);
                //////    return;
                //////}
                //////this.btnPost.Enabled = false;
                //////this.progressBar1.Visible = true;
                //////sqlResults = new string[10];
                //////TransferIssueDAL issueDal = new TransferIssueDAL();
                #endregion

                #region Issue Post

                //TransferIssueDAL issueDal = new TransferIssueDAL();
                ITransferIssue issueDal = OrdinaryVATDesktop.GetObject<TransferIssueDAL, TransferIssueRepo, ITransferIssue>(OrdinaryVATDesktop.IsWCF);

                transferIssueVM = new TransferIssueVM();
                transferIssueVM.TransferIssueNo = txtIssueNo.Text.Trim();
                transferIssueVM.IsTransfer = "N";

                #region Find UserInfo

                DataRow[] userInfo = settingVM.UserInfoDT.Select("UserID='" + Program.CurrentUserID + "'");
                transferIssueVM.SignatoryName = userInfo[0]["FullName"].ToString();
                transferIssueVM.SignatoryDesig = userInfo[0]["Designation"].ToString();
                #endregion

                sqlResults = issueDal.PostTransfer(transferIssueVM, null, null, connVM);

                #endregion

                #region Statement

                if (sqlResults.Count() > 0)
                {
                    string result = sqlResults[0];
                    string message = sqlResults[1];
                    if (string.IsNullOrEmpty(result))
                    {
                        throw new ArgumentNullException("Post", "Unexpected error.");
                    }
                    else if (result == "Success" || result == "Fail")
                    {
                        MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        IsUpdate = true;
                    }
                    if (result == "Success")
                    {
                        txtIssueNo.Text = sqlResults[2].ToString();
                        IsPost = Convert.ToString(sqlResults[3].ToString()) == "Y" ? true : false;
                        for (int i = 0; i < dgvIssue.RowCount; i++)
                        {
                            dgvIssue["Status", dgvIssue.RowCount - 1].Value = "Old";
                        }
                    }
                }

                #endregion

                ChangeData = false;
            }

            #endregion

            #region catch

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

            #region Finally
            finally
            {
                ChangeData = false;
                this.btnPost.Enabled = true;
                this.progressBar1.Visible = false;
            }
            #endregion

        }

        private void btnSearchIssueNo_Click(object sender, EventArgs e)
        {
            #region try
            try
            {
                #region Static Values

                Program.fromOpen = "Me";

                #endregion

                #region Transaction Type

                TransactionTypes();

                #endregion

                #region Select invoiceNo

                string invoiceNo = FormTransferIssueSearch.SelectOne(transactionType);

                #endregion

                #region Check Point

                if (string.IsNullOrEmpty(invoiceNo))
                    return;

                #endregion

                #region Value Assign

                txtIssueNo.Text = invoiceNo;

                #endregion

                #region Search Invoice

                SearchInvoice();

                #endregion

            }

            #endregion

            #region catch

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

            #region finally

            finally
            {
                ChangeData = false;
                this.btnSearchIssueNo.Enabled = true;
                this.progressBar1.Visible = false;
            }

            #endregion

        }

        private void selectLastRow()
        {
            #region try
            try
            {
                if (dgvIssue.Rows.Count > 0)
                {
                    dgvIssue.Rows[dgvIssue.Rows.Count - 1].Selected = true;
                    dgvIssue.CurrentCell = dgvIssue.Rows[dgvIssue.Rows.Count - 1].Cells[1];
                }
            }
            #endregion

            #region catch

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

        private void btnAdd_Click(object sender, EventArgs e)
        {
            #region try
            try
            {
                #region Checkpoint

                #region Null Check

                ChangeData = true;
                if (txtCommentsDetail.Text == "")
                {
                    txtCommentsDetail.Text = "NA";
                }
                if (txtProductCode.Text == "")
                {
                    MessageBox.Show("Please select a Item", this.Text);
                    return;
                }
                if (txtQuantity.Text == "")
                {
                    txtQuantity.Text = "0.00";
                }
                if (txtUnitCost.Text == "")
                {
                    txtUnitCost.Text = "0.00";
                }
                if (Convert.ToDecimal(txtUnitCost.Text.Trim()) <= 0)
                {
                    MessageBox.Show(MessageVM.msgNegPrice);
                    txtUnitCost.Focus();
                    return;
                }
                if (txtCommentsDetail.Text == "")
                {
                    txtCommentsDetail.Text = "NA";
                }
                if (Convert.ToDecimal(txtQuantity.Text) <= 0)
                {
                    MessageBox.Show("Please input Quantity", this.Text);
                    txtQuantity.Focus();
                    return;
                }

                #endregion

                #region Check Multiple Product Allow

                var common = new CommonDAL();

                var multiple = common.settingsDesktop("TransferIssue", "MultipleProduct");


                if (multiple == "N")
                {
                    for (int i = 0; i < dgvIssue.RowCount; i++)
                    {
                        if (dgvIssue.Rows[i].Cells["ItemNo"].Value.ToString() == txtProductCode.Text)
                        {
                            MessageBox.Show("Same Product already exist.", this.Text);
                            return;
                        }
                    }
                }

                #endregion

                UomsValue();

                #region Check Stock
                //if (rbtn61Out.Checked == false)
                //{
                //    if (
                //        Convert.ToDecimal(txtQuantity.Text) * Convert.ToDecimal(txtUomConv.Text.Trim()) > Convert.ToDecimal(txtQuantityInHand.Text))
                //    {
                //        MessageBox.Show("Stock Not available");
                //        txtQuantity.Focus();
                //        return;
                //    }
                //}
                #endregion Check Stock

                UomsValue();

                if (cmbUom.SelectedIndex == -1)
                {
                    //throw new ArgumentNullException(this.Text, "Please select pack size");
                }

                #endregion

                #region Value Assign to DataGridView - dgvIssue

                DataGridViewRow NewRow = new DataGridViewRow();
                dgvIssue.Rows.Add(NewRow);
                dgvIssue["BOMId", dgvIssue.RowCount - 1].Value = txtBOMId.Text.Trim();

                dgvIssue["ItemNo", dgvIssue.RowCount - 1].Value = txtProductCode.Text.Trim();
                dgvIssue["ItemName", dgvIssue.RowCount - 1].Value = txtProductName.Text.Trim();
                dgvIssue["PCode", dgvIssue.RowCount - 1].Value = txtPCode.Text.Trim();
                //dgvIssue["UOM", dgvIssue.RowCount - 1].Value = txtUOM.Text.Trim();
                //string strUom = cmbUom.SelectedItem.ToString();
                dgvIssue["UOM", dgvIssue.RowCount - 1].Value = cmbUom.Text.Trim();
                if (Program.CheckingNumericTextBox(txtQuantity, "txtQuantity") == true)
                    txtQuantity.Text = Program.FormatingNumeric(txtQuantity.Text.Trim(), IssuePlaceQty).ToString();
                if (Program.CheckingNumericTextBox(txtUnitCost, "txtUnitCost") == true)
                    txtUnitCost.Text = Program.FormatingNumeric(txtUnitCost.Text.Trim(), IssuePlaceAmt).ToString();
                dgvIssue["Quantity", dgvIssue.RowCount - 1].Value = Program.ParseDecimalObject(Convert.ToDecimal(txtQuantity.Text.Trim()).ToString());
                dgvIssue["UnitPrice", dgvIssue.RowCount - 1].Value =
                    Program.ParseDecimalObject(Convert.ToDecimal(Convert.ToDecimal(txtUnitCost.Text.Trim()) * Convert.ToDecimal(txtUomConv.Text.Trim())).ToString());
                //dgvIssue["UOMPrice", dgvIssue.RowCount - 1].Value = Program.ParseDecimalObject(Convert.ToDecimal(Convert.ToDecimal(txtUnitCost.Text.Trim())).ToString());
              
                dgvIssue["Comments", dgvIssue.RowCount - 1].Value = "NA";// txtCommentsDetail.Text.Trim();
                dgvIssue["Status", dgvIssue.RowCount - 1].Value = "New";
                dgvIssue["Stock", dgvIssue.RowCount - 1].Value = Program.ParseDecimalObject(Convert.ToDecimal(txtQuantityInHand.Text.Trim()).ToString());
                dgvIssue["Previous", dgvIssue.RowCount - 1].Value = 0;// txtQuantity.Text.Trim();
                dgvIssue["Change", dgvIssue.RowCount - 1].Value = 0;
               
                dgvIssue["UOMPrice", dgvIssue.RowCount - 1].Value = Program.ParseDecimalObject(Convert.ToDecimal(Convert.ToDecimal(txtUnitCost.Text.Trim())).ToString());
               
                dgvIssue["UOMc", dgvIssue.RowCount - 1].Value =
                    Program.ParseDecimalObject(Convert.ToDecimal(txtUomConv.Text.Trim()));// txtUOM.Text.Trim();
                dgvIssue["UOMn", dgvIssue.RowCount - 1].Value =
                    txtUOM.Text.Trim();// txtUOM.Text.Trim();
                dgvIssue["UOMQty", dgvIssue.RowCount - 1].Value =
                    Program.ParseDecimalObject(Convert.ToDecimal(Convert.ToDecimal(txtQuantity.Text.Trim()) * Convert.ToDecimal(txtUomConv.Text.Trim())));


                dgvIssue["Weight", dgvIssue.RowCount - 1].Value = txtWeight.Text.Trim();

                dgvIssue["VATRate", dgvIssue.RowCount - 1].Value = Program.ParseDecimalObject(Convert.ToDecimal(txtVATRate.Text.Trim()));
                dgvIssue["VATAmount", dgvIssue.RowCount - 1].Value = Program.ParseDecimalObject(Convert.ToDecimal(txtVATAmount.Text.Trim()));
                dgvIssue["SDRate", dgvIssue.RowCount - 1].Value = Program.ParseDecimalObject(Convert.ToDecimal(txtSDRate.Text.Trim()));
                dgvIssue["SDAmount", dgvIssue.RowCount - 1].Value = Program.ParseDecimalObject(Convert.ToDecimal(txtSDAmount.Text.Trim()));

                #endregion

                Rowcalculate();
                GTotal();

                AllClear();

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
            #endregion

            #region catch

            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;
                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnAdd_Click", exMessage);
            }
            #endregion
        }

        private void AllClear()
        {
            txtProductCode.Text = "";
            txtProductName.Text = "";
            //txtCategoryName.Text = "";
            txtHSCode.Text = "";
            txtUnitCost.Text = "0.00";
            txtQuantity.Text = "";
            txtVATRate.Text = "0.00";
            txtUOM.Text = "";
            txtCommentsDetail.Text = "NA";
            txtQuantityInHand.Text = "0.00";
            cmbProduct.Text = "";
            cmbProductName.Text = "";
            cmbUom.Text = "";
        }

        private void Rowcalculate()
        {
            #region try
            try
            {
                decimal SumSubTotal = 0;
                decimal Quantity = 0;
                decimal Cost = 0;
                decimal SubTotal = 0;
                for (int i = 0; i < dgvIssue.RowCount; i++)
                {
                    dgvIssue[0, i].Value = i + 1;
                    Quantity = Convert.ToDecimal(dgvIssue["Quantity", i].Value);
                    Cost = Convert.ToDecimal(dgvIssue["UnitPrice", i].Value);
                    SubTotal = Cost * Quantity;
                    if (Program.CheckingNumericString(SubTotal.ToString(), "SubTotal") == true)
                        SubTotal = Convert.ToDecimal(Program.FormatingNumeric(SubTotal.ToString(), IssuePlaceQty));
                    dgvIssue["SubTotal", i].Value = Convert.ToDecimal(SubTotal);
                    //dgvIssue["VATAmount", i].Value = 0;
                    //dgvIssue["SDAmount", i].Value = 0;
                    //dgvIssue["SubTotal", i].Value = Convert.ToDecimal(SubAmount).ToString();//"0,0.00");
                    SumSubTotal = SumSubTotal + Convert.ToDecimal(dgvIssue["SubTotal", i].Value);
                    dgvIssue["Change", i].Value = Convert.ToDecimal(Convert.ToDecimal(dgvIssue["Quantity", i].Value)
                        - Convert.ToDecimal(dgvIssue["Previous", i].Value)).ToString();//"0,0.0000");
                }
                txtTotalVATAmount.Text = "0";
                //txtTotalAmount.Text = Convert.ToDecimal(SumSubTotal).ToString();//"0,0.00");
                txtTotalAmount.Text = Program.ParseDecimalObject(SumSubTotal).ToString();//"0,0.00");
            }
            #endregion

            #region catch

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

        private void btnRemove_Click(object sender, EventArgs e)
        {
            #region try
            try
            {
                if (dgvIssue.RowCount > 0)
                {
                    if (MessageBox.Show(MessageVM.msgWantToRemoveRow + "\nItem Code: " + dgvIssue.CurrentRow.Cells["PCode"].Value, this.Text, MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        dgvIssue.Rows.RemoveAt(dgvIssue.CurrentRow.Index);
                        Rowcalculate();
                        GTotal();
                    }
                }
                else
                {
                    MessageBox.Show("No Items Found in Remove.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
            }

            #endregion

            #region catch

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

        private void remove()
        {
            #region try
            try
            {
                ChangeData = true;
                if (txtProductCode.Text == "")
                {
                    MessageBox.Show("Please Select Desired Item Row Appropriately Using Double Click!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    //MessageBox.Show("Please select a Item", this.Text);
                    return;
                }
                if (txtCommentsDetail.Text == "")
                {
                    txtCommentsDetail.Text = "NA";
                }
                #region Stock Chekc
                decimal StockValue, PreviousValue, ChangeValue, CurrentValue, purchaseQty;
                StockValue = Convert.ToDecimal(txtQuantityInHand.Text);
                PreviousValue = Convert.ToDecimal(txtPrevious.Text);
                //CurrentValue = Convert.ToDecimal(txtQuantity.Text);
                CurrentValue = 0;
                if (rbtn61Out.Checked == false)
                {
                    if (CurrentValue > PreviousValue)
                    {
                        if (
                            Convert.ToDecimal(CurrentValue - PreviousValue) > Convert.ToDecimal(StockValue))
                        {
                            MessageBox.Show("Stock Not available");
                            txtQuantity.Focus();
                            return;
                        }
                    }
                }
                else
                {
                    if (CurrentValue < PreviousValue)
                    {
                        if (
                            Convert.ToDecimal(PreviousValue - CurrentValue) >
                            Convert.ToDecimal(StockValue))
                        {
                            MessageBox.Show("Stock Not available");
                            txtQuantity.Focus();
                            return;
                        }
                    }
                }
                #endregion Stock Chekc
                dgvIssue.CurrentRow.Cells["Status"].Value = "Delete";
                dgvIssue.CurrentRow.Cells["Quantity"].Value = 0.00;
                dgvIssue.CurrentRow.Cells["UnitPrice"].Value = 0.00;
                dgvIssue.CurrentRow.Cells["VATRate"].Value = 0.00;
                dgvIssue.CurrentRow.Cells["VATAmount"].Value = 0.00;
                dgvIssue.CurrentRow.Cells["SubTotal"].Value = 0.00;
                dgvIssue.CurrentRow.Cells["NBRPrice"].Value = 0.00;
                dgvIssue.CurrentRow.Cells["SD"].Value = 0.00;
                dgvIssue.CurrentRow.Cells["SDAmount"].Value = 0.00;
                dgvIssue.CurrentRow.DefaultCellStyle.ForeColor = Color.Red;
                dgvIssue.CurrentRow.DefaultCellStyle.Font = new Font(this.Font, FontStyle.Strikeout);
                Rowcalculate();
                //AllClear();
                txtProductCode.Text = "";
                txtProductName.Text = "";
                if (rbtnCode.Checked)
                {
                    cmbProduct.Focus();
                }
                else
                {
                    cmbProductName.Focus();
                }
            }
            #endregion

            #region catch

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
            if (dgvIssue.RowCount > 0)
            {
                ReceiveChangeSingle();
            }
            else
            {
                MessageBox.Show("No Items Found in Details Section.\nAdd New Items OR Load Previous Item !", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
        }

        #endregion

        #region Methods 02

        private void ReceiveChangeSingle()
        {
            try
            {
                #region try

                #region Checkpoint

                #region Null Check

                if (
                    string.IsNullOrEmpty(txtUOM.Text)
                    || string.IsNullOrEmpty(txtProductCode.Text)
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
                if (Convert.ToDecimal(txtUnitCost.Text.Trim()) <= 0)
                {
                    MessageBox.Show(MessageVM.msgNegPrice);
                    txtUnitCost.Focus();
                    return;
                }
                if (txtCommentsDetail.Text == "")
                {
                    txtCommentsDetail.Text = "NA";
                }
                ChangeData = true;
                if (string.IsNullOrEmpty(cmbUom.Text))
                {
                    throw new ArgumentNullException("", "Please select pack size");
                }

                #endregion

                UomsValue();
                //decimal quantity = Convert.ToDecimal(dgvIssue.CurrentRow.Cells["Quantity"].Value);
                //if (Convert.ToDecimal(txtQuantity.Text) > quantity)
                //{
                //    MessageBox.Show("Return quantity can not be greater than actual quantity.");
                //    txtQuantity.Text = quantity.ToString();
                //    txtQuantity.Focus();
                //    return; dgvIssue.CurrentRow.Index
                //}   
                #region Stock Check
                //decimal StockValue, PreviousValue, ChangeValue, CurrentValue, purchaseQty;
                //StockValue = Convert.ToDecimal(txtQuantityInHand.Text);
                //PreviousValue = Convert.ToDecimal(txtPrevious.Text);
                //CurrentValue = Convert.ToDecimal(txtQuantity.Text);
                //if (rbtn61Out.Checked == false)
                //{
                //    if (CurrentValue > PreviousValue)
                //    {
                //        if (
                //            Convert.ToDecimal(CurrentValue - PreviousValue) > Convert.ToDecimal(StockValue))
                //        {
                //            MessageBox.Show("Stock Not available");
                //            txtQuantity.Focus();
                //            return;
                //        }
                //    }
                //}
                //else
                //{
                //    if (CurrentValue <= PreviousValue)
                //    {
                //        if (
                //            Convert.ToDecimal(PreviousValue - CurrentValue) >
                //            Convert.ToDecimal(StockValue))
                //        {
                //            MessageBox.Show("Stock Not available");
                //            txtQuantity.Focus();
                //            return;
                //        }
                //    }
                //    else
                //    {
                //    }
                //}
                #endregion Stock Chekc

                #endregion

                #region Value Assign to DataGridView dgvIssue

                string strUom = cmbUom.Text.ToString();
                dgvIssue["BOMId", dgvIssue.CurrentRow.Index].Value = txtBOMId.Text;

                dgvIssue["UOM", dgvIssue.CurrentRow.Index].Value = strUom.Trim();
                //dgvIssue["PCode", dgvIssue.CurrentRow.Index].Value = txtPCode.Text.Trim();
                dgvIssue["Quantity", dgvIssue.CurrentRow.Index].Value = Program.ParseDecimalObject(Convert.ToDecimal(txtQuantity.Text.Trim()).ToString());//"0,0.000");
                dgvIssue["UnitPrice", dgvIssue.CurrentRow.Index].Value =
                    Program.ParseDecimalObject(Convert.ToDecimal(Convert.ToDecimal(txtUnitCost.Text.Trim()) * Convert.ToDecimal(txtUomConv.Text.Trim())).ToString());
                dgvIssue["UOMPrice", dgvIssue.CurrentRow.Index].Value = Program.ParseDecimalObject(Convert.ToDecimal(Convert.ToDecimal(txtUnitCost.Text.Trim())).ToString());
                //dgvIssue["UnitPrice", dgvIssue.CurrentRow.Index].Value = Convert.ToDecimal(txtUnitCost.Text.Trim()).ToString();//"0,0.00");
                dgvIssue["Comments", dgvIssue.CurrentRow.Index].Value = "NA";// txtCommentsDetail.Text.Trim();
                dgvIssue["UOMc", dgvIssue.CurrentRow.Index].Value =
                Convert.ToDecimal(txtUomConv.Text.Trim());// txtUOM.Text.Trim();
                dgvIssue["UOMn", dgvIssue.CurrentRow.Index].Value = txtUOM.Text.Trim();// txtUOM.Text.Trim();
                dgvIssue["UOMQty", dgvIssue.CurrentRow.Index].Value =
                    Program.ParseDecimalObject(Convert.ToDecimal(Convert.ToDecimal(txtQuantity.Text.Trim()) *
                    Convert.ToDecimal(txtUomConv.Text.Trim())));

                dgvIssue["VATRate", dgvIssue.CurrentRow.Index].Value = Program.ParseDecimalObject(Convert.ToDecimal(txtVATRate.Text.Trim()));
                dgvIssue["VATAmount", dgvIssue.CurrentRow.Index].Value = Program.ParseDecimalObject(Convert.ToDecimal(txtVATAmount.Text.Trim()));
                dgvIssue["SDRate", dgvIssue.CurrentRow.Index].Value = Program.ParseDecimalObject(Convert.ToDecimal(txtSDRate.Text.Trim()));
                dgvIssue["SDAmount", dgvIssue.CurrentRow.Index].Value = Program.ParseDecimalObject(Convert.ToDecimal(txtSDAmount.Text.Trim()));
                dgvIssue["Weight", dgvIssue.CurrentRow.Index].Value = txtWeight.Text.Trim();

                if (dgvIssue.CurrentRow.Cells["Status"].Value.ToString() != "New")
                {
                    dgvIssue["Status", dgvIssue.CurrentRow.Index].Value = "Change";
                }
                dgvIssue.CurrentRow.DefaultCellStyle.ForeColor = Color.Green;// Blue;
                //dgvIssue.CurrentRow.DefaultCellStyle.ForeColor = Color.Red;
                dgvIssue.CurrentRow.DefaultCellStyle.Font = new Font(this.Font, FontStyle.Regular);

                #endregion

                Rowcalculate();
                GTotal();

                #region Reset Fields

                txtProductCode.Text = "";
                txtProductName.Text = "";
                //txtCategoryName.Text = "";
                txtHSCode.Text = "";
                txtUnitCost.Text = "0.00";
                txtQuantity.Text = "";
                txtVATRate.Text = "0.00";
                txtUOM.Text = "";
                txtCommentsDetail.Text = "NA";
                txtQuantityInHand.Text = "0.00";
                if (rbtnCode.Checked)
                {
                    cmbProduct.Focus();
                }
                else
                {
                    cmbProductName.Focus();
                }

                #endregion

                #endregion
            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(err, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

        public void ClearAllFields()
        {

            txtId.Text = "0";
            SearchBranchId = 0;
            txtFiscalYear.Text = "0";

            cmbIsRaw.Text = "Select";
            cmbProduct.Text = "Select";
            txtQuantityInHand.Text = "0.0";
            txtPCode.Text = "";
            //cmbProductType.Text = "Select";
            txtComments.Text = "";
            cmbTransferTo.Text = "Select";
            txtBranchDBName.Text = "";
            txtCommentsDetail.Text = "NA";
            txtHSCode.Text = "";
            txtIssueNo.Text = "";
            txtProductCode.Text = "";
            txtProductName.Text = "";
            txtQuantity.Text = "0.00";
            txtSerialNo.Text = "";
            txtReferenceNo.Text = "";
            txtTotalAmount.Text = "0.00";
            txtUnitCost.Text = "0.00";
            cmbIsRaw.Text = "";
            txtTrifNo.Text = "";
            txtUOM.Text = "";
            txtVehicleNo.Text = "";
            txtVehicleType.Text = "";
            txtWeight.Text = "";
            txtTotalQuantity.Text = "";
            txtTransferto.Text = "";
            txtTransferToId.Text = "0";

            string AutoSessionDate = commonDal.settingsDesktop("SessionDate", "AutoSessionDate",null,connVM);
            if (AutoSessionDate.ToLower() != "y")
            {
                dtpIssueDate.Value = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss"));

            }
            else
            {
                dtpIssueDate.Value = Convert.ToDateTime(Program.SessionDate.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss"));

            }

            dgvIssue.Rows.Clear();
        }

        private void txtComments_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtVATRate_Leave(object sender, EventArgs e)
        {
            //Program.FormatTextBoxRate(txtVATRate, "VAT Rate");
            txtVATRate.Text = Program.ParseDecimalObject(txtVATRate.Text.Trim()).ToString();
        }

        private void txtQuantity_Leave(object sender, EventArgs e)
        {
            //////Program.FormatTextBoxQty4(txtQuantity, "Quantity");
            //if (Program.CheckingNumericTextBox(txtQuantity, "Total Quantity") == true)
            //{
            //    txtQuantity.Text = Program.FormatingNumeric(txtQuantity.Text.Trim(), IssuePlaceQty).ToString();
            //}
            txtQuantity.Text = Program.ParseDecimalObject(txtQuantity.Text.Trim()).ToString();
            SDVATAmount();


        }

        private void SDVATAmount()
        {

            decimal Quantity = Convert.ToDecimal(txtQuantity.Text);
            decimal UnitCost = Convert.ToDecimal(txtUnitCost.Text);

            decimal SDRate = Convert.ToDecimal(txtSDRate.Text);

            decimal SDAmount = (Quantity * UnitCost) * (SDRate / 100);

            txtSDAmount.Text = Program.FormatingNumeric(SDAmount.ToString(), IssuePlaceQty).ToString();

            decimal VATRate = Convert.ToDecimal(txtVATRate.Text);

            decimal VATAmount = ((Quantity * UnitCost) + SDAmount) * (VATRate / 100);

            txtVATAmount.Text = Program.FormatingNumeric(VATAmount.ToString(), IssuePlaceQty).ToString();

        }

        private void txtQuantity_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void FormTransferIssue_Load(object sender, EventArgs e)
        {
            #region try
            try
            {
                #region Initial Setup

                #region Transaction Type

                TransactionTypes();

                #endregion

                #region Button Stats

                btnSave.Text = "&Add";
                txtIssueNo.Text = "~~~ New ~~~";

                #endregion

                #region Form Maker

                FormMaker();

                #endregion

                #region Form Load

                FormLoad();

                #endregion

                #region Reset Form Elements

                ClearAllFields();

                #endregion

                #region Flag Update

                ChangeData = false;
                System.Windows.Forms.ToolTip ToolTip1 = new System.Windows.Forms.ToolTip();
                Post = true;// Convert.ToString(Program.Post) == "Y" ? true : false;
                Add = true;//Convert.ToString(Program.Add) == "Y" ? true : false;
                Edit = true;//Convert.ToString(Program.Edit) == "Y" ? true : false;

                #endregion

                #region VAT Names

                VATName vname = new VATName();
                cmbVAT1Name.DataSource = vname.VATNameList;

                #endregion

                #endregion

                #region Settings
                string vIssuePlaceQty, vIssuePlaceAmt = string.Empty;
                CommonDAL commonDal = new CommonDAL();
                vIssuePlaceQty = commonDal.settingsDesktop("Issue", "Quantity",null,connVM);
                vIssuePlaceAmt = commonDal.settingsDesktop("Issue", "Amount",null,connVM);
                if (string.IsNullOrEmpty(vIssuePlaceQty)
                    || string.IsNullOrEmpty(vIssuePlaceAmt)
                    )
                {
                    MessageBox.Show(MessageVM.msgSettingValueNotSave, this.Text);
                    return;
                }
                IssuePlaceQty = Convert.ToInt32(vIssuePlaceQty);
                IssuePlaceAmt = Convert.ToInt32(vIssuePlaceAmt);

                #endregion Settings

                #region cmbShift

                ShiftDAL _sDal = new ShiftDAL();

                cmbShift.DataSource = _sDal.SearchForTime(DateTime.Now.ToString("HH:mm"),connVM);
                cmbShift.DisplayMember = "ShiftName";
                cmbShift.ValueMember = "Id";

                //cmbShift = cDal.ComboBoxLoad(cmbShift, "Shifts", "Id", "ShiftName", Condition, "varchar", true);
                #endregion cmbShift

                #region Button Stats

                progressBar1.Visible = true;

                #endregion

                #region Loading UOM, Product, Branch
                UOMDAL uomdal = new UOMDAL();
                UOMVM uomVM = new UOMVM();
                uomVM.UOMID = UOMIdParam;
                uomVM.UOMFrom = UOMFromParam;
                uomVM.UOMTo = UOMToParam;
                uomVM.ActiveStatus = ActiveStatusUOMParam;
                uomVM.DatabaseName = Program.DatabaseName;
                //uomResult = uomdal.SearchUOM(uomVM);

                ProductDAL productDal = new ProductDAL();
                ProductVM productVM = new ProductVM();
                productVM.ItemNo = varItemNo;
                productVM.CategoryID = varCategoryID;
                productVM.IsRaw = varIsRaw;
                productVM.CategoryName = varHSCodeNo;
                productVM.ActiveStatus = varActiveStatus;
                //////productVM.Trading = varTrading;
                productVM.NonStock = varNonStock;
                productVM.ProductCode = varProductCode;
                //ProductResultDs = productDal.SearchProductMiniDS(productVM);
                //TransferIssueDAL _transferIssueDal = new TransferIssueDAL();
                ITransferIssue _transferIssueDal = OrdinaryVATDesktop.GetObject<TransferIssueDAL, TransferIssueRepo, ITransferIssue>(OrdinaryVATDesktop.IsWCF);

                formLoadDS = _transferIssueDal.FormLoad(uomVM, productVM, branchName, connVM);

                #region Branch Load

                string[] Condition = new string[] { "ActiveStatus='Y' AND BranchID NOT IN(" + Program.BranchId + ")" };
                cmbTransferTo = new CommonDAL().ComboBoxLoad(cmbTransferTo, "BranchProfiles", "BranchID", "BranchName", Condition, "varchar", true, false);



                //BranchDAL branchDal = new BranchDAL();
                //dtbranchNames = branchDal.SearchBranchName(branchName);




                ////////BranchLists.Clear();
                ////////foreach (DataRow item in formLoadDS.Tables[2].Rows)
                ////////{
                ////////    BranchDTO br = new BranchDTO();
                ////////    br.Id = Convert.ToInt32(item["Id"].ToString()); ;
                ////////    br.Name = item["Name"].ToString();
                ////////    br.DBName = item["DBName"].ToString();
                ////////    BranchLists.Add(br);
                ////////}
                ////////BranchNameLoad();

                #endregion

                ProductsMini.Clear();
                //foreach (DataRow item2 in ProductResultDs.Rows)
                foreach (DataRow item2 in formLoadDS.Tables[1].Rows)
                {
                    var prod = new ProductMiniDTO();
                    prod.ItemNo = item2["ItemNo"].ToString();
                    prod.ProductName = item2["ProductName"].ToString();
                    prod.ProductCode = item2["ProductCode"].ToString();
                    prod.ProductNameCode = item2["ProductNameCode"].ToString();
                    prod.CategoryID = item2["CategoryID"].ToString();
                    prod.CategoryName = item2["CategoryName"].ToString();
                    prod.UOM = item2["UOM"].ToString();
                    prod.HSCodeNo = item2["HSCodeNo"].ToString();
                    prod.IsRaw = item2["IsRaw"].ToString();
                    prod.CostPrice = Convert.ToDecimal(item2["CostPrice"].ToString());
                    prod.SalesPrice = Convert.ToDecimal(item2["SalesPrice"].ToString());
                    prod.NBRPrice = Convert.ToDecimal(item2["NBRPrice"].ToString());
                    prod.ReceivePrice = Convert.ToDecimal(item2["ReceivePrice"].ToString());
                    prod.IssuePrice = Convert.ToDecimal(item2["IssuePrice"].ToString());
                    prod.Packetprice = Convert.ToDecimal(item2["Packetprice"].ToString());
                    prod.VATRate = Convert.ToDecimal(item2["VATRate"].ToString());
                    prod.SD = Convert.ToDecimal(item2["SD"].ToString());
                    prod.TradingMarkUp = Convert.ToDecimal(item2["TradingMarkUp"].ToString());
                    prod.Stock = Convert.ToDecimal(item2["Stock"].ToString());
                    prod.QuantityInHand = Convert.ToDecimal(item2["QuantityInHand"].ToString());
                    prod.NonStock = item2["NonStock"].ToString() == "Y" ? true : false;
                    prod.Trading = item2["Trading"].ToString() == "Y" ? true : false;
                    ProductsMini.Add(prod);
                }//End FOR
                ProductSearchDsLoad();

                UOMs.Clear();

                cmbUom.Items.Clear();

                foreach (DataRow item2 in formLoadDS.Tables[0].Rows)
                {
                    var uom = new UomDTO();
                    uom.UOMId = item2["UOMId"].ToString();
                    uom.UOMFrom = item2["UOMFrom"].ToString();
                    uom.UOMTo = item2["UOMTo"].ToString();
                    uom.Convertion = Convert.ToDecimal(item2["Convertion"].ToString());
                    uom.CTypes = item2["CTypes"].ToString();
                    cmbUom.Items.Add(item2["UOMTo"].ToString());
                    UOMs.Add(uom);
                }
                #endregion Loading UOM, Product, Branch
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
                FileLogger.Log(this.Name, "FormTransferIssue_Load", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "FormTransferIssue_Load", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (FormatException ex)
            {
                FileLogger.Log(this.Name, "FormTransferIssue_Load", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "FormTransferIssue_Load", exMessage);
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
                FileLogger.Log(this.Name, "FormTransferIssue_Load", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormTransferIssue_Load", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormTransferIssue_Load", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "FormTransferIssue_Load", exMessage);
            }
            #endregion
            #region Finally
            finally
            {
                ChangeData = false;
                this.progressBar1.Visible = false;
            }
            #endregion
        }

        private void FormMaker()
        {
            try
            {
                //btnSearchIssueNoP.Visible = false;
                //txtIssueNoP.Visible = false;

                #region Transaction Type
                if (rbtn62Out.Checked)
                {
                    this.Text = "Transfer Issue VAT FG (Out)";
                    //btn62.Visible = true;
                    txtTrifNo.Visible = true;
                    label25.Visible = true;
                    btnTripLoad.Visible = true;
                }
                else if (rbtn61Out.Checked)
                {
                    //btnVAT16.Visible = true;
                    this.Text = "Transfer Issue VAT RM (Out)";
                    //btnSearchIssueNoP.Visible = true;
                    //txtIssueNoP.Visible = true;
                    txtTrifNo.Visible = false;
                    label25.Visible = false;
                    btnTripLoad.Visible = false;
                }
                #endregion Transaction Type


                #region Tracking

                string vTracking = string.Empty;
                string vHeading1, vHeading2 = string.Empty;
                vTracking = commonDal.settingsDesktop("TrackingTrace", "Tracking", null, connVM);
                vHeading1 = commonDal.settingsDesktop("TrackingTrace", "TrackingHead_1", null, connVM);
                vHeading2 = commonDal.settingsDesktop("TrackingTrace", "TrackingHead_2", null, connVM);

                if (string.IsNullOrEmpty(vTracking) || string.IsNullOrEmpty(vHeading1) || string.IsNullOrEmpty(vHeading2))
                {
                    MessageBox.Show(MessageVM.msgSettingValueNotSave, this.Text);
                    return;
                }
                TrackingTrace = vTracking == "Y" ? true : false;

              
                if (TrackingTrace == true)
                {
                    btnTracking.Visible = true;
                    Heading1 = vHeading1;
                    Heading2 = vHeading2;
                }
                #endregion

                #region Form Elements Visibility Control

                if (transactionType == "61Out")
                {
                    lblReferenceNo.Visible = false;
                    cmbBOMReferenceNo.Visible = false;
                }

                #endregion

                #region Button Import Integration Lisence
                if (Program.IsIntegrationExcel == false && Program.IsIntegrationOthers == false)
                {
                    btnImport.Visible = false;
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
                FileLogger.Log(this.Name, "FormMaker", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "FormMaker", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (FormatException ex)
            {
                FileLogger.Log(this.Name, "FormMaker", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "FormMaker", exMessage);
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
                FileLogger.Log(this.Name, "FormMaker", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormMaker", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormMaker", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "FormMaker", exMessage);
            }
            #endregion
        }

        private void FormLoad()
        {
            #region cmbProduct DropDownWidth Change

            string ProductDropDownWidth = commonDal.settingsDesktop("Product", "ProductDropDownWidth");
            cmbProductName.DropDownWidth = Convert.ToInt32(ProductDropDownWidth);

            #endregion
            #region Product
            if (CategoryId == null)
            {
                varItemNo = "";
                varCategoryID = "";
                if (rbtn62Out.Checked)
                {
                    varIsRaw = "Finish";
                }
                else if (rbtn61Out.Checked)
                {
                    varIsRaw = "Raw";
                }

                varHSCodeNo = "";
                varActiveStatus = "Y";
                //////varTrading = "N";
                varNonStock = "N";
                varProductCode = "";
            }
            else
            {
                varItemNo = "";
                varCategoryID = "CategoryId";
                varIsRaw = "";
                varHSCodeNo = "";
                varActiveStatus = "";
                varTrading = "";
                varNonStock = "";
                varProductCode = "";
            }

            #region UOM
            UOMIdParam = string.Empty;
            UOMFromParam = string.Empty;
            UOMToParam = string.Empty;
            ActiveStatusUOMParam = string.Empty;
            UOMIdParam = string.Empty;
            UOMFromParam = string.Empty;
            UOMToParam = string.Empty;
            ActiveStatusUOMParam = "Y";
            #endregion UOM

            if (rbtn62Out.Checked)
            {
                txtCategoryName.Text = "Finish";

            }
            else if (rbtn61Out.Checked)
            {
                txtCategoryName.Text = "Raw";
            }


            #endregion Product
        }

        private void BranchNameLoad()
        {
            #region try
            try
            {
                cmbTransferTo.Items.Clear();
                var branchName = from br in BranchLists.ToList()
                                 orderby br.Name
                                 select br.Name;
                cmbTransferTo.Items.AddRange(branchName.ToArray());
                cmbTransferTo.Items.Insert(0, "Select");
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
                FileLogger.Log(this.Name, "BranchNameLoad", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "BranchNameLoad", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (FormatException ex)
            {
                FileLogger.Log(this.Name, "BranchNameLoad", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                FileLogger.Log(this.Name, "BranchNameLoad", exMessage);
            }
            #endregion
        }

        private void ProductSearchDsFormLoad()
        {
            //string ProductData = string.Empty;
            #region try
            try
            {
                if (CategoryId == null)
                {
                    varItemNo = "";
                    varCategoryID = "";
                    if (rbtn62Out.Checked)
                    {
                        varIsRaw = "Finish";
                    }
                    else if (rbtn61Out.Checked)
                    {
                        varIsRaw = "Raw";
                    }
                    varHSCodeNo = "";
                    varActiveStatus = "Y";
                    ////////varTrading = "N";
                    varNonStock = "N";
                    varProductCode = "";
                    if (rbtn62Out.Checked)
                    {
                        txtCategoryName.Text = "Finish";
                    }
                    else if (rbtn61Out.Checked)
                    {
                        txtCategoryName.Text = "Raw";
                    }
                }
                else
                {
                    varItemNo = "";
                    varCategoryID = CategoryId;
                    varIsRaw = "";
                    varHSCodeNo = txtCategoryName.Text.Trim();
                    varActiveStatus = "Y";
                    //////varTrading = "N";
                    varNonStock = "N";
                    varProductCode = "";
                }
                this.cmbProduct.Enabled = false;
                this.chkPCode.Enabled = false;
                this.btnProductGroup.Enabled = false;
                this.progressBar1.Visible = true;
                //backgroundWorkerProductSearchDsFormLoad.RunWorkerAsync();
                ProductDAL productDal = new ProductDAL();
                //IProduct productDal = OrdinaryVATDesktop.GetObject<ProductDAL, ProductRepo, IProduct>(OrdinaryVATDesktop.IsWCF);

                ProductResultDs = new DataTable();
                ProductVM productVM = new ProductVM();
                productVM.ItemNo = varItemNo;
                productVM.CategoryID = varCategoryID;
                productVM.IsRaw = varIsRaw;
                productVM.CategoryName = varHSCodeNo;
                productVM.ActiveStatus = varActiveStatus;
                //////productVM.Trading = varTrading;
                productVM.NonStock = varNonStock;
                productVM.ProductCode = varProductCode;

                ProductResultDs = productDal.SearchProductMiniDS_WithProductvm(productVM, connVM);
                //ProductResultDs = productDal.SearchProductMiniDS(varItemNo, varCategoryID, varIsRaw, varHSCodeNo, varActiveStatus, varTrading, varNonStock, varProductCode);
                ProductsMini.Clear();
                foreach (DataRow item2 in ProductResultDs.Rows)
                {
                    var prod = new ProductMiniDTO();
                    prod.ItemNo = item2["ItemNo"].ToString();
                    prod.ProductName = item2["ProductName"].ToString();
                    prod.ProductCode = item2["ProductCode"].ToString();
                    prod.CategoryID = item2["CategoryID"].ToString();
                    prod.CategoryName = item2["CategoryName"].ToString();
                    prod.UOM = item2["UOM"].ToString();
                    prod.HSCodeNo = item2["HSCodeNo"].ToString();
                    prod.IsRaw = item2["IsRaw"].ToString();
                    prod.CostPrice = Convert.ToDecimal(item2["CostPrice"].ToString());
                    prod.SalesPrice = Convert.ToDecimal(item2["SalesPrice"].ToString());
                    prod.NBRPrice = Convert.ToDecimal(item2["NBRPrice"].ToString());
                    prod.ReceivePrice = Convert.ToDecimal(item2["ReceivePrice"].ToString());
                    prod.IssuePrice = Convert.ToDecimal(item2["IssuePrice"].ToString());
                    prod.Packetprice = Convert.ToDecimal(item2["Packetprice"].ToString());
                    prod.VATRate = Convert.ToDecimal(item2["VATRate"].ToString());
                    prod.SD = Convert.ToDecimal(item2["SD"].ToString());
                    prod.TradingMarkUp = Convert.ToDecimal(item2["TradingMarkUp"].ToString());
                    prod.Stock = Convert.ToDecimal(item2["Stock"].ToString());
                    prod.QuantityInHand = Convert.ToDecimal(item2["QuantityInHand"].ToString());
                    prod.NonStock = item2["NonStock"].ToString() == "Y" ? true : false;
                    prod.Trading = item2["Trading"].ToString() == "Y" ? true : false;
                    prod.ProductNameCode = prod.ProductName + "~" + prod.ProductCode;
                    ProductsMini.Add(prod);
                }//End For
                //End Complete
                ProductSearchDsLoad();
            }
            #endregion
            #region catch
           
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;
                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ProductSearchDsFormLoad", exMessage);
            }
            #endregion
            finally
            {
                this.cmbProduct.Enabled = true;
                this.chkPCode.Enabled = true;
                this.btnProductGroup.Enabled = true;
                this.progressBar1.Visible = false;
            }
        }

        private void ProductSearchDsLoad()
        {
            //No SOAP Service
            #region try
            try
            {
                cmbProduct.Items.Clear();
                cmbProductName.DataSource = null;
                cmbProductName.Items.Clear();
                if (rbtnCode.Checked == true)
                {
                    var prodByCode = from prd in ProductsMini.ToList()
                                     orderby prd.ProductCode
                                     select prd.ProductCode;
                    if (prodByCode != null && prodByCode.Any())
                    {
                        cmbProduct.Items.AddRange(prodByCode.ToArray());
                    }
                }
                else if (rbtnProduct.Checked == true)
                {
                    var prodByName = from prd in ProductsMini.ToList()
                                     orderby prd.ProductName
                                     select prd.ProductNameCode;
                    if (prodByName != null && prodByName.Any())
                    {
                        cmbProductName.Items.AddRange(prodByName.ToArray());
                    }
                }
                cmbProduct.Items.Insert(0, "Select");
            }
            #endregion
            #region catch
            
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

        private void dgvIssue_DoubleClick(object sender, EventArgs e)
        {
            #region try
            try
            {
                if (dgvIssue.Rows.Count <= 0)
                {
                    MessageBox.Show("There is no data to select.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                //if (chkPCode.Checked)
                //{
                //    cmbProduct.Text = dgvIssue.CurrentRow.Cells["PCode"].Value.ToString();
                //}
                //else
                //{
                //    cmbProduct.Text = dgvIssue.CurrentRow.Cells["ItemName"].Value.ToString();
                //}

                #region Value Assign to Form Elements

                txtBOMId.Text = dgvIssue.CurrentRow.Cells["BOMId"].Value.ToString();


                cmbProduct.Text = dgvIssue.CurrentRow.Cells["PCode"].Value.ToString();
                cmbProductName.Text = dgvIssue.CurrentRow.Cells["ItemName"].Value.ToString() + "~" + dgvIssue.CurrentRow.Cells["PCode"].Value.ToString();
                txtLineNo.Text = dgvIssue.CurrentCellAddress.Y.ToString();
                txtProductCode.Text = dgvIssue.CurrentRow.Cells["ItemNo"].Value.ToString();
                txtPCode.Text = dgvIssue.CurrentRow.Cells["PCode"].Value.ToString();
                txtProductName.Text = dgvIssue.CurrentRow.Cells["ItemName"].Value.ToString();
                txtUOM.Text = dgvIssue.CurrentRow.Cells["UOM"].Value.ToString();
                txtQuantity.Text = Convert.ToDecimal(dgvIssue.CurrentRow.Cells["Quantity"].Value).ToString();//"0,0.0000");
                txtUnitCost.Text = Convert.ToDecimal(dgvIssue.CurrentRow.Cells["UnitPrice"].Value).ToString();//"0,0.00");
                txtCommentsDetail.Text = "NA";// dgvIssue.CurrentRow.Cells["Comments"].Value.ToString();
                txtPrevious.Text = Convert.ToDecimal(dgvIssue.CurrentRow.Cells["Previous"].Value).ToString();//"0,0.0000");
                txtUOM.Text = dgvIssue.CurrentRow.Cells["UOMn"].Value.ToString();
                cmbUom.Items.Insert(0, txtUOM.Text.Trim());
                txtVATRate.Text = Convert.ToDecimal(dgvIssue.CurrentRow.Cells["VATRate"].Value).ToString();
                txtVATAmount.Text = Convert.ToDecimal(dgvIssue.CurrentRow.Cells["VATAmount"].Value).ToString();
                txtSDRate.Text = Convert.ToDecimal(dgvIssue.CurrentRow.Cells["SDRate"].Value).ToString();
                txtSDAmount.Text = Convert.ToDecimal(dgvIssue.CurrentRow.Cells["SDAmount"].Value).ToString();
                txtWeight.Text = dgvIssue.CurrentRow.Cells["Weight"].Value.ToString().Trim();

                #region UOM Function

                Uoms();

                cmbUom.Text = dgvIssue.CurrentRow.Cells["UOM"].Value.ToString();
                txtUomConv.Text = dgvIssue.CurrentRow.Cells["UOMc"].Value.ToString();

                #endregion
                ProductDAL productDal = new ProductDAL();
                //IProduct productDal = OrdinaryVATDesktop.GetObject<ProductDAL, ProductRepo, IProduct>(OrdinaryVATDesktop.IsWCF);

                DataTable priceDS = new DataTable();
                priceDS = productDal.AvgPriceNew(txtProductCode.Text, dtpIssueDate.Value.ToString("yyyy-MMM-dd") +
                            DateTime.Now.ToString(" HH:mm:ss"), null, null, true, true, true, true, connVM,Program.CurrentUserID);
                txtQuantityInHand.Text = priceDS.Rows[0]["Quantity"].ToString();

                #endregion

                CommonDAL commonDal = new CommonDAL();

                string priceCall = commonDal.settingsDesktop("TransferIssue", "PriceCallDoubleClick",null,connVM);


                #region 62Out/61Out

                if (priceCall.ToLower() == "y")
                {
                    if (transactionType == "62Out")
                    {

                        PirceCall();

                    }
                    else if (transactionType == "61Out")
                    {

                        PirceCallIssue();

                    }
                }




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
                FileLogger.Log(this.Name, "dgvIssue_DoubleClick", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "dgvIssue_DoubleClick", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (FormatException ex)
            {
                FileLogger.Log(this.Name, "dgvIssue_DoubleClick", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "dgvIssue_DoubleClick", exMessage);
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
                FileLogger.Log(this.Name, "dgvIssue_DoubleClick", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "dgvIssue_DoubleClick", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "dgvIssue_DoubleClick", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "dgvIssue_DoubleClick", exMessage);
            }
            #endregion
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            #region try
            try
            {
                FormRptIssueInformation frmRptIssueInformation = new FormRptIssueInformation();
                //frmRptIssueInformation.txtIssueNo.Text = txtIssueNo.Text.Trim();
                frmRptIssueInformation.Show();
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
                FileLogger.Log(this.Name, "btnPrint_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnPrint_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (FormatException ex)
            {
                FileLogger.Log(this.Name, "btnPrint_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnPrint_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnPrint_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnPrint_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnPrint_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnPrint_Click", exMessage);
            }
            #endregion
        }

        #endregion

        #region Methods 03

        private void SearchInvoice()
        {
            try
            {

                #region Data Call

                ITransferIssue issueDal = OrdinaryVATDesktop.GetObject<TransferIssueDAL, TransferIssueRepo, ITransferIssue>(OrdinaryVATDesktop.IsWCF);

                TransferIssueVM vm = new TransferIssueVM();
                vm.TransferIssueNo = txtIssueNo.Text;
                vm.IssueDateFrom = "";
                vm.IssueDateTo = "";
                vm.Post = "";
                vm.TransactionType = transactionType;
                vm.ReferenceNo = "";

                DataTable IssueResult = issueDal.SearchTransferIssue(vm, null, null, connVM);  // Change 04

                #endregion

                #region Load Data

                LoadForm(IssueResult);

                #endregion

                #region Button Stats

                btnSave.Text = "&Save";

                #endregion

                #region Flag Update

                IsUpdate = true;
                ChangeData = false;

                #endregion

            }
            #region Catch
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;
                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "SearchInvoice", exMessage);
            }
            #endregion

        }

        private void LoadForm(DataTable IssueResult)
        {
            string invoiceNo = "";
            ITransferIssue issueDal = OrdinaryVATDesktop.GetObject<TransferIssueDAL, TransferIssueRepo, ITransferIssue>(OrdinaryVATDesktop.IsWCF);

            if (IssueResult.Rows.Count <= 0)
            {
                return;
            }

            DataRow dr = IssueResult.Rows[0];

            #region Value Assign to Form Elements


            txtIssueNo.Text = dr["TransferIssueNo"].ToString();
            txtId.Text = dr["Id"].ToString();
            txtFiscalYear.Text = dr["FiscalYear"].ToString();

            invoiceNo = dr["TransferIssueNo"].ToString();
            dtpIssueDate.Value = Convert.ToDateTime(dr["IssueDateTime"].ToString());
            txtTotalAmount.Text =
                Program.ParseDecimalObject(Convert.ToDecimal(dr["TotalAmount"].ToString()).ToString()); //"0,0.00");
            txtSerialNo.Text = dr["SerialNo"].ToString();
            txtReferenceNo.Text = dr["ReferenceNo"].ToString();
            txtComments.Text = dr["Comments"].ToString();
            txtVehicleNo.Text = dr["VehicleNo"].ToString();
            txtVehicleType.Text = dr["VehicleType"].ToString();
            txtBranchDBName.Text = dr["TransferTo"].ToString();
            txtTransferToId.Text = dr["TransferTo"].ToString();
            txtTrifNo.Text = dr["TripNo"].ToString();
            cmbTransferTo.Text = dr["BranchName"].ToString();
            txtTransferto.Text = dr["BranchName"].ToString();
            IsPost = Convert.ToString(dr["Post"].ToString()) == "Y" ? true : false;
            SearchBranchId = Convert.ToInt32(dr["BranchId"]);
            cmbShift.SelectedValue = dr["ShiftId"];
            IssueDetailData = txtIssueNo.Text == "" ? "" : txtIssueNo.Text.Trim();

            #endregion

            #region Statement

            IssueDetailResult = new DataTable();
            if (!string.IsNullOrEmpty(invoiceNo))
            {
                IssueDetailResult = issueDal.SearchTransferDetail(invoiceNo,null,null,connVM);
            }

            #region DataGridView

            dgvIssue.Rows.Clear();
            int j = 0;
            foreach (DataRow item in IssueDetailResult.Rows)
            {
                DataGridViewRow NewRow = new DataGridViewRow();
                dgvIssue.Rows.Add(NewRow);
                dgvIssue.Rows[j].Cells["BOMId"].Value = item["BOMId"].ToString();

                dgvIssue.Rows[j].Cells["LineNo"].Value = item["IssueLineNo"].ToString();
                dgvIssue.Rows[j].Cells["ItemNo"].Value = item["ItemNo"].ToString();
                dgvIssue.Rows[j].Cells["Quantity"].Value = Program.ParseDecimalObject(item["Quantity"].ToString());
                dgvIssue.Rows[j].Cells["UnitPrice"].Value = Program.ParseDecimalObject(item["CostPrice"].ToString());
                dgvIssue.Rows[j].Cells["UOM"].Value = item["UOM"].ToString();
                dgvIssue.Rows[j].Cells["SubTotal"].Value = Program.ParseDecimalObject(item["SubTotal"].ToString());
                dgvIssue.Rows[j].Cells["Comments"].Value = item["Comments"].ToString();
                dgvIssue.Rows[j].Cells["ItemName"].Value = item["ProductName"].ToString();
                dgvIssue.Rows[j].Cells["PCode"].Value = item["ProductCode"].ToString();
                dgvIssue.Rows[j].Cells["Status"].Value = "Old";
                dgvIssue.Rows[j].Cells["Previous"].Value = Program.ParseDecimalObject(item["Quantity"].ToString());
                dgvIssue.Rows[j].Cells["Stock"].Value = Program.ParseDecimalObject(item["Stock"].ToString());
                dgvIssue.Rows[j].Cells["Change"].Value = 0;
                dgvIssue.Rows[j].Cells["UOMc"].Value = Program.ParseDecimalObject(item["UOMc"].ToString());
                dgvIssue.Rows[j].Cells["UOMn"].Value = item["UOMn"].ToString();
                dgvIssue.Rows[j].Cells["UOMQty"].Value = Program.ParseDecimalObject(item["UOMQty"].ToString());
                dgvIssue.Rows[j].Cells["UOMPrice"].Value = Program.ParseDecimalObject(item["UOMPrice"].ToString());

                dgvIssue.Rows[j].Cells["VATRate"].Value = Program.ParseDecimalObject(item["VATRate"].ToString());
                dgvIssue.Rows[j].Cells["VATAmount"].Value = Program.ParseDecimalObject(item["VATAmount"].ToString());
                dgvIssue.Rows[j].Cells["SDRate"].Value = Program.ParseDecimalObject(item["SDRate"].ToString());
                dgvIssue.Rows[j].Cells["SDAmount"].Value = Program.ParseDecimalObject(item["SDAmount"].ToString());
                dgvIssue.Rows[j].Cells["Weight"].Value = item["Weight"].ToString();

                j = j + 1;
            } //End For
            GTotal();
            #endregion DataGridView



            #endregion

        }

        private void LoadForm_FromTempTable(DataTable IssueResult)
        {
            string invoiceNo = "";
            TransferIssueDAL issueDal = new TransferIssueDAL();

            if (IssueResult.Rows.Count <= 0)
            {
                return;
            }

            txtIssueNo.Text = IssueResult.Rows[0]["TransferIssueNo"].ToString();
            invoiceNo = IssueResult.Rows[0]["Id"].ToString();
            dtpIssueDate.Value = Convert.ToDateTime(IssueResult.Rows[0]["IssueDateTime"].ToString());
            txtTotalAmount.Text =
                Convert.ToDecimal(IssueResult.Rows[0]["TotalAmount"].ToString()).ToString(); //"0,0.00");
            txtSerialNo.Text = IssueResult.Rows[0]["SerialNo"].ToString();
            txtReferenceNo.Text = IssueResult.Rows[0]["ReferenceNo"].ToString();
            txtComments.Text = IssueResult.Rows[0]["Comments"].ToString();
            txtVehicleNo.Text = IssueResult.Rows[0]["VehicleNo"].ToString();
            txtBranchDBName.Text = IssueResult.Rows[0]["TransferTo"].ToString();
            txtTransferToId.Text = IssueResult.Rows[0]["TransferTo"].ToString();
            txtTrifNo.Text = IssueResult.Rows[0]["TripNo"].ToString();
            cmbTransferTo.Text = IssueResult.Rows[0]["BranchName"].ToString();
            txtTransferto.Text = IssueResult.Rows[0]["BranchName"].ToString();
            IsPost = Convert.ToString(IssueResult.Rows[0]["Post"].ToString()) == "Y" ? true : false;

            SearchBranchId = Convert.ToInt32(IssueResult.Rows[0]["BranchId"]);

            cmbShift.SelectedValue = IssueResult.Rows[0]["ShiftId"];


            ////string IssueDetailData;
            IssueDetailData = txtIssueNo.Text == "" ? "" : txtIssueNo.Text.Trim();

            #region Statement

            IssueDetailResult = new DataTable();
            if (!string.IsNullOrEmpty(invoiceNo))
            {
                IssueDetailResult = issueDal.SearchTransferDetail_TempTable(invoiceNo, Program.CurrentUserID,null,null,connVM);
            }

            #region DataGridView

            dgvIssue.Rows.Clear();
            int j = 0;
            foreach (DataRow item in IssueDetailResult.Rows)
            {
                DataGridViewRow NewRow = new DataGridViewRow();
                dgvIssue.Rows.Add(NewRow);
                dgvIssue.Rows[j].Cells["BOMId"].Value = item["BOMId"].ToString();

                dgvIssue.Rows[j].Cells["LineNo"].Value = item["IssueLineNo"].ToString();
                dgvIssue.Rows[j].Cells["ItemNo"].Value = item["ItemNo"].ToString();
                dgvIssue.Rows[j].Cells["Quantity"].Value = item["Quantity"].ToString();
                dgvIssue.Rows[j].Cells["UnitPrice"].Value = item["CostPrice"].ToString();
                dgvIssue.Rows[j].Cells["UOM"].Value = item["UOM"].ToString();
                dgvIssue.Rows[j].Cells["SubTotal"].Value = item["SubTotal"].ToString();
                dgvIssue.Rows[j].Cells["Comments"].Value = item["Comments"].ToString();
                dgvIssue.Rows[j].Cells["ItemName"].Value = item["ProductName"].ToString();
                dgvIssue.Rows[j].Cells["PCode"].Value = item["ProductCode"].ToString();
                dgvIssue.Rows[j].Cells["Status"].Value = "Old";
                dgvIssue.Rows[j].Cells["Previous"].Value = item["Quantity"].ToString();
                dgvIssue.Rows[j].Cells["Stock"].Value = item["Stock"].ToString();
                dgvIssue.Rows[j].Cells["Change"].Value = 0;
                dgvIssue.Rows[j].Cells["UOMc"].Value = item["UOMc"].ToString();
                dgvIssue.Rows[j].Cells["UOMn"].Value = item["UOMn"].ToString();
                dgvIssue.Rows[j].Cells["UOMQty"].Value = item["UOMQty"].ToString();
                dgvIssue.Rows[j].Cells["UOMPrice"].Value = item["UOMPrice"].ToString();

                dgvIssue.Rows[j].Cells["VATRate"].Value = item["VATRate"].ToString();
                dgvIssue.Rows[j].Cells["VATAmount"].Value = item["VATAmount"].ToString();
                dgvIssue.Rows[j].Cells["SDRate"].Value = item["SDRate"].ToString();
                dgvIssue.Rows[j].Cells["SDAmount"].Value = item["SDAmount"].ToString();
                dgvIssue.Rows[j].Cells["Weight"].Value = item["Weight"].ToString();

                j = j + 1;
            } //End For
            GTotal();
            Rowcalculate();

            #endregion DataGridView



            #endregion

        }

        private void GTotal()
        {
            decimal TotalQuantuty = 0;

            for (int i = 0; i < dgvIssue.Rows.Count; i++)
            {
                TotalQuantuty = TotalQuantuty + Convert.ToDecimal(dgvIssue["Quantity", i].Value);
            }

            #region SubTotal Load

            //txtTotalQuantity.Text = Convert.ToDecimal(Program.FormatingNumeric(TotalQuantuty.ToString(), 4)).ToString(); //"0,0.00");
            txtTotalQuantity.Text = Program.ParseDecimalObject(TotalQuantuty.ToString()).ToString(); //"0,0.00");



            #endregion

        }

        private void btnSearchProductName_Click(object sender, EventArgs e)
        {
            string itemNo = "72";
            decimal costPrice = 550;
            var dd = ProductsMini.ToList();
            var tt = ProductsMini.SingleOrDefault(x => x.ItemNo == itemNo);
            tt.CostPrice = costPrice;
            var aa = tt;
        }

        private void txtProductCode_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtIssueNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }

            if (e.KeyCode.Equals(Keys.Enter))
            {
                TransferIssueNavigation("Current");
            }
        }

        private void txtSerialNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtReferenceNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void dtpIssueDate_KeyDown(object sender, KeyEventArgs e)
        {
            //cmbProduct.Focus();
        }

        private void cmbProduct_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtQuantity_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                btnAdd.Focus();
            }
        }

        private void txtVATRate_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtCommentsDetail_KeyDown(object sender, KeyEventArgs e)
        {
            btnAdd.Focus();
        }

        private void btnProductType_Click(object sender, EventArgs e)
        {
            //#region try
            //try
            //{
            //    string result = FormProductTypeSearch.SelectOne();
            //    if (result == "") { return; }
            //    else//if (result == ""){return;}else//if (result != "")
            //    {
            //        string[] TypeInfo = result.Split(FieldDelimeter.ToCharArray());
            //        cmbProductType.Text = TypeInfo[1];
            //    }
            //    //ProductSearchDs();
            //    //ProductSearch();
            //}
            //#endregion
            //#region catch
            //catch (ArgumentNullException ex)
            //{
            //    string err = ex.Message.ToString();
            //    string[] error = err.Split(FieldDelimeter.ToCharArray());
            //    FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
            //    MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //}
            //catch (IndexOutOfRangeException ex)
            //{
            //    FileLogger.Log(this.Name, "btnProductType_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            //    MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //}
            //catch (NullReferenceException ex)
            //{
            //    FileLogger.Log(this.Name, "btnProductType_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            //    MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //}
            //catch (FormatException ex)
            //{
            //    FileLogger.Log(this.Name, "btnProductType_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            //    MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //}
            //catch (SoapHeaderException ex)
            //{
            //    string exMessage = ex.Message;
            //    if (ex.InnerException != null)
            //    {
            //        exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
            //                    ex.StackTrace;
            //    }
            //    FileLogger.Log(this.Name, "btnProductType_Click", exMessage);
            //    MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //}
            //catch (SoapException ex)
            //{
            //    string exMessage = ex.Message;
            //    if (ex.InnerException != null)
            //    {
            //        exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
            //                    ex.StackTrace;
            //    }
            //    MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    FileLogger.Log(this.Name, "btnProductType_Click", exMessage);
            //}
            //catch (EndpointNotFoundException ex)
            //{
            //    MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    FileLogger.Log(this.Name, "btnProductType_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            //}
            //catch (WebException ex)
            //{
            //    MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    FileLogger.Log(this.Name, "btnProductType_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            //}
            //catch (Exception ex)
            //{
            //    string exMessage = ex.Message;
            //    if (ex.InnerException != null)
            //    {
            //        exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
            //                    ex.StackTrace;
            //    }
            //    MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    FileLogger.Log(this.Name, "btnProductType_Click", exMessage);
            //}
            //#endregion
        }

        private void cmbProductType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtCategoryName_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtHSCode_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        #endregion

        #region Methods 04

        private void txtUOM_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtUnitCost_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtVATRate_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtSD_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtCommentsDetail_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtQuantityInHand_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtSerialNo_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtReferenceNo_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void dtpIssueDate_ValueChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtTotalVATAmount_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtTotalAmount_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void FormTransferIssue_FormClosing(object sender, FormClosingEventArgs e)
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

        private void txtSD_Leave(object sender, EventArgs e)
        {
            //Program.FormatTextBoxRate(txtSDRate, "SD Rate");
            txtSDRate.Text = Program.ParseDecimalObject(txtSDRate.Text.Trim()).ToString();
        }

        private void btnAddNew_Click(object sender, EventArgs e)
        {
            #region try
            try
            {
                IsPost = false;
                if (ChangeData == true)
                {
                    if (DialogResult.No != MessageBox.Show(
                        "Recent changes have not been saved ." + "\n" + "Want to add new without saving?",
                        this.Text,
                         MessageBoxButtons.YesNo,
                         MessageBoxIcon.Question,
                         MessageBoxDefaultButton.Button2))
                    {
                        //ProductSearchDsFormLoad();
                        ClearAllFields();
                        btnSave.Text = "&Add";
                        txtIssueNo.Text = "~~~ New ~~~";
                    }
                }
                else if (ChangeData == false)
                {
                    //ProductSearchDsFormLoad();
                    //ProductSearchDsLoad();
                    ClearAllFields();
                    btnSave.Text = "&Add";
                    txtIssueNo.Text = "~~~ New ~~~";
                }
                IsUpdate = false;
                ChangeData = false;
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
                FileLogger.Log(this.Name, "btnAddNew_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnAddNew_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (FormatException ex)
            {
                FileLogger.Log(this.Name, "btnAddNew_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnAddNew_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnAddNew_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnAddNew_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnAddNew_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnAddNew_Click", exMessage);
            }
            #endregion
            finally
            {
                ChangeData = false;
            }
        }

        private void btnIssue_Click(object sender, EventArgs e)
        {
            #region try
            try
            {
                if (Program.CheckLicence(dtpIssueDate.Value) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }
                TransactionTypes();
                FormRptTransferIssueInformation frmRptTransferIssueInformation = new FormRptTransferIssueInformation();

                if (txtIssueNo.Text == "~~~ New ~~~")
                {
                    frmRptTransferIssueInformation.txtIssueNo.Text = "";
                }
                else
                {
                    frmRptTransferIssueInformation.txtIssueNo.Text = txtIssueNo.Text.Trim();
                }
                frmRptTransferIssueInformation.txtTransactionType.Text = transactionType;
                frmRptTransferIssueInformation.ShowDialog();
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
                FileLogger.Log(this.Name, "btnIssue_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnIssue_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (FormatException ex)
            {
                FileLogger.Log(this.Name, "btnIssue_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnIssue_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnIssue_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnIssue_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnIssue_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnIssue_Click", exMessage);
            }
            #endregion
        }

        #endregion

        #region Methods 05

        private void btnVAT16_Click(object sender, EventArgs e)
        {
            #region try
            try
            {
                if (Program.CheckLicence(dtpIssueDate.Value) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }
                MDIMainInterface mdi = new MDIMainInterface();
                FormRptVAT16 frmRptVAT16 = new FormRptVAT16();

                //mdi.RollDetailsInfo("8201");
                //if (Program.Access != "Y")
                //{
                //    MessageBox.Show("You do not have to access this form", frmRptVAT16.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                //    return;
                //}
                if (dgvIssue.Rows.Count > 0)
                {
                    frmRptVAT16.txtItemNo.Text = dgvIssue.CurrentRow.Cells["ItemNo"].Value.ToString();
                    frmRptVAT16.txtProductName.Text = dgvIssue.CurrentRow.Cells["ItemName"].Value.ToString();
                    frmRptVAT16.dtpFromDate.Value = dtpIssueDate.Value;
                    frmRptVAT16.dtpToDate.Value = dtpIssueDate.Value;

                }

                frmRptVAT16.ShowDialog();
            }
            #endregion

            #region catch

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

        private void chkPCode_CheckedChanged(object sender, EventArgs e)
        {
            if (chkPCode.Checked)
            {
                chkPCode.Text = "By Code";
            }
            else
            {
                chkPCode.Text = "By Name";
            }
            ProductSearchDsLoad();
            cmbProduct.Focus();
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
                        var prodByCode = from prd in ProductsMini.ToList()
                                         where prd.ProductCode.ToLower() == searchText.ToLower()
                                         select prd;
                        if (prodByCode != null && prodByCode.Any())
                        {
                            var products = prodByCode.First();
                            txtProductName.Text = products.ProductName;
                            cmbProductName.Text = products.ProductName;
                            txtProductCode.Text = products.ItemNo;
                            txtUOM.Text = products.UOM;
                            cmbUom.Text = products.UOM;
                            txtHSCode.Text = products.HSCodeNo;
                            txtPCode.Text = products.ProductCode;

                            txtVATRate.Text = products.VATRate.ToString();
                            txtSDRate.Text = products.SD.ToString();
                        }
                    }
                }
                string strProductCode = txtProductCode.Text;
                if (!string.IsNullOrEmpty(strProductCode))
                {
                    ProductDAL productDal = new ProductDAL();
                    //IProduct productDal = OrdinaryVATDesktop.GetObject<ProductDAL, ProductRepo, IProduct>(OrdinaryVATDesktop.IsWCF);

                    DataTable priceData = productDal.AvgPriceNew(strProductCode, dtpIssueDate.Value.ToString("yyyy-MMM-dd") +
                                            DateTime.Now.ToString(" HH:mm:ss"), null, null, true, true, true, true, connVM,Program.CurrentUserID);
                    decimal amount = Convert.ToDecimal(priceData.Rows[0]["Amount"].ToString());
                    decimal quanA = Convert.ToDecimal(priceData.Rows[0]["Quantity"].ToString());
                    if (quanA > 0)
                    {
                        txtUnitCost.Text = (amount / quanA).ToString();
                    }
                    else
                    {
                        txtUnitCost.Text = "0";
                    }
                    txtQuantityInHand.Text = priceData.Rows[0]["Quantity"].ToString();
                    PirceCall();
                    //}
                    txtQuantity.Focus();
                    Uoms();
                }
                //}
            }
            #endregion

            #region catch

            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;
                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "cmbProduct_Leave", exMessage);
            }
            #endregion

        }

        private void PirceCall()
        {
            ProductDAL productDal = new ProductDAL();
            //IProduct productDal = OrdinaryVATDesktop.GetObject<ProductDAL, ProductRepo, IProduct>(OrdinaryVATDesktop.IsWCF);

            string strProductCode = txtProductCode.Text;
            if (!string.IsNullOrEmpty(strProductCode))
            {
                #region BOM - NBRPrice

                DataTable dt = new DataTable();

                //BugsBD
                string ProductCode = OrdinaryVATDesktop.SanitizeInput(txtProductCode.Text.Trim());
                string VAT1Name = OrdinaryVATDesktop.SanitizeInput(cmbVAT1Name.Text.Trim());

                dt = productDal.GetBOMReferenceNo(
                      ProductCode
                    , VAT1Name
                    , dtpIssueDate.Value.ToString("yyyy-MMM-dd")
                    , null
                    , null
                    , "0"
                    , connVM);


                //dt = productDal.GetBOMReferenceNo(txtProductCode.Text.Trim(), cmbVAT1Name.Text.Trim(),
                                                                        //dtpIssueDate.Value.ToString("yyyy-MMM-dd"), null, null, "0", connVM);


                #region Comments

                //////     if (CustomerWiseBOM)
                //////{
                //////    dt = productDal.GetBOMReferenceNo(txtProductCode.Text.Trim(), cmbVAT1Name.Text.Trim(),
                //////                                                             dtpReceiveDate.Value.ToString("yyyy-MMM-dd"), null, null, vCustomerID);

                //////}
                //////else
                //////{
                //////    dt = productDal.GetBOMReferenceNo(txtProductCode.Text.Trim(), cmbVAT1Name.Text.Trim(),
                //////                                  dtpReceiveDate.Value.ToString("yyyy-MMM-dd"), null, null);

                //////}

                #endregion



                int BOMId = 0;
                decimal NBRPrice = 0;

                if (dt != null && dt.Rows.Count > 0)
                {
                    #region ReferenceNo

                    cmbBOMReferenceNo.DataSource = dt;
                    cmbBOMReferenceNo.DisplayMember = "ReferenceNo";
                    cmbBOMReferenceNo.ValueMember = "ReferenceNo";

                    cmbBOMReferenceNo.SelectedIndex = 0;

                    #endregion

                    #region BOMId and NBRPrice

                    DataRow dr = dt.Rows[0];
                    string tempBOMId = dr["BOMId"].ToString();
                    string tempNBRPrice = dr["NBRPrice"].ToString();
                    if (!string.IsNullOrWhiteSpace(tempBOMId))
                    {
                        BOMId = Convert.ToInt32(tempBOMId);
                    }

                    if (!string.IsNullOrWhiteSpace(tempNBRPrice))
                    {
                        NBRPrice = Convert.ToDecimal(tempNBRPrice);
                    }
                    #endregion

                }

                if (!string.IsNullOrWhiteSpace(txtBOMId.Text))
                {
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        SetBOM(dt);
                    }
                }
                else
                {
                    txtBOMId.Text = BOMId.ToString();
                }


                #endregion

                #region Value Assign

                txtUnitCost.Text = Program.FormatingNumeric(NBRPrice.ToString(), IssuePlaceAmt).ToString();

                #endregion

                #region Comments

                ////if (Program.CheckingNumericTextBox(txtUnitCost, "Unit Cost") == true)
                ////{
                ////    txtUnitCost.Text = Program.FormatingNumeric(txtUnitCost.Text.Trim(), IssuePlaceAmt).ToString();
                ////}
                ////else
                ////{
                ////    return;
                ////}

                #endregion

            }
        }

        private void PirceCallIssue()
        {
            ProductDAL productDal = new ProductDAL();
            //IProduct productDal = OrdinaryVATDesktop.GetObject<ProductDAL, ProductRepo, IProduct>(OrdinaryVATDesktop.IsWCF);


            #region BOM

            DataTable dt = new DataTable();

            //BugsBD
            string ProductCode = OrdinaryVATDesktop.SanitizeInput(txtProductCode.Text.Trim());

            dt = productDal.SelectBOMRaw(ProductCode, dtpIssueDate.Value.ToString("yyyy-MMM-dd"), null, null, connVM);

            //dt = productDal.SelectBOMRaw(txtProductCode.Text.Trim(), dtpIssueDate.Value.ToString("yyyy-MMM-dd"), null, null, connVM);

            int BOMId = 0;

            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                string tempBOMId = dr["BOMId"].ToString();
                if (!string.IsNullOrWhiteSpace(tempBOMId))
                {
                    BOMId = Convert.ToInt32(tempBOMId);
                }
            }

            txtBOMId.Text = BOMId.ToString();
            #endregion

            string strProductCode = txtProductCode.Text;

            if (!string.IsNullOrEmpty(strProductCode))
            {
                if (Program.CheckingNumericTextBox(txtUnitCost, "Unit Cost") == true)
                {
                    txtUnitCost.Text = Program.FormatingNumeric(txtUnitCost.Text.Trim(), IssuePlaceAmt).ToString();

                }
                else
                {

                    return;
                }
            }
        }

        private void btnProductGroup_Click(object sender, EventArgs e)
        {
            #region try
            try
            {
                Program.fromOpen = "Other";
                string result = FormProductCategorySearch.SelectOne();
                if (result == "")
                {
                    return;
                }
                string[] ProductCategoryInfo = result.Split(FieldDelimeter.ToCharArray());
                CategoryId = ProductCategoryInfo[0];
                txtCategoryName.Text = ProductCategoryInfo[1];
                cmbProductType.Text = ProductCategoryInfo[4];
                ProductSearchDsFormLoad();
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
                FileLogger.Log(this.Name, "button1_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "button1_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (FormatException ex)
            {
                FileLogger.Log(this.Name, "button1_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "button1_Click", exMessage);
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
                FileLogger.Log(this.Name, "button1_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "button1_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "button1_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "button1_Click", exMessage);
            }
            #endregion
        }


        private void UomsValue()
        {
            try
            {
                #region Statement
                string uOMFrom = txtUOM.Text.Trim().ToLower();
                string uOMTo = cmbUom.Text.Trim().ToLower();
                if (!string.IsNullOrEmpty(uOMTo) && uOMTo != "select")
                {
                    txtUomConv.Text = "0";
                    if (uOMFrom == uOMTo)
                    {
                        txtUomConv.Text = "1";
                        txtQuantity.Focus();
                        return;
                    }
                    else if (UOMs != null && UOMs.Any())
                    {
                        var uoms = from uom in UOMs.Where(x => x.UOMFrom.Trim().ToLower() == uOMFrom && x.UOMTo.Trim().ToLower() == uOMTo).ToList()
                                   select uom.Convertion;
                        if (uoms != null && uoms.Any())
                        {
                            txtUomConv.Text = uoms.First().ToString();
                        }
                        else
                        {
                            MessageBox.Show("Please select the UOM", this.Text);
                            txtUomConv.Text = "0";
                            return;
                        }
                    }
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
                FileLogger.Log(this.Name, "UomsValue", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "UomsValue", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (FormatException ex)
            {
                FileLogger.Log(this.Name, "UomsValue", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "UomsValue", exMessage);
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
                FileLogger.Log(this.Name, "UomsValue", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "UomsValue", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "UomsValue", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "UomsValue", exMessage);
            }
            #endregion
        }

        private void Uoms()
        {
            try
            {
                #region Statement
                string uOMFrom = txtUOM.Text.Trim().ToLower();
                //string uOMTo = cmbUom.Text.ToLower();
                cmbUom.Items.Clear();
                if (UOMs != null && UOMs.Any())
                {
                    var uoms = from uom in UOMs.Where(x => x.UOMFrom.Trim().ToLower() == uOMFrom).ToList()
                               select uom.UOMTo;
                    if (uoms != null && uoms.Any())
                    {
                        cmbUom.Items.AddRange(uoms.ToArray());
                        cmbUom.Items.Add(txtUOM.Text.Trim());
                        //txtUomConv.Text = uoms.First().ToString();
                    }
                }
                //cmbUom.Text = uOMTo;
                cmbUom.Text = txtUOM.Text.Trim();
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
                FileLogger.Log(this.Name, "Uoms", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "Uoms", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (FormatException ex)
            {
                FileLogger.Log(this.Name, "Uoms", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "Uoms", exMessage);
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
                FileLogger.Log(this.Name, "Uoms", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "Uoms", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "Uoms", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "Uoms", exMessage);
            }
            #endregion
        }

        private void cmbUom_KeyDown(object sender, KeyEventArgs e)
        {
            btnAdd.Focus();
        }

        private void cmbUom_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void rbtnCode_CheckedChanged(object sender, EventArgs e)
        {
            if (rbtnCode.Checked)
            {
                //chkPCode.Text = "By Code";
                cmbProductName.Enabled = false;
                cmbProduct.Enabled = true;
            }
            //else
            //{
            //chkPCode.Text = "By Name";
            //cmbProduct.Enabled = false;
            //cmbProductName.Enabled = true;
            //}
            ProductSearchDsLoad();
            //cmbProduct.Focus();
        }

        private void rbtnProduct_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                #region Statement
                if (rbtnProduct.Checked)
                {
                    cmbProductName.Enabled = true;
                    cmbProduct.Enabled = false;
                }
                ProductSearchDsLoad();
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
                FileLogger.Log(this.Name, "rbtnProduct_CheckedChanged", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "rbtnProduct_CheckedChanged", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (FormatException ex)
            {
                FileLogger.Log(this.Name, "rbtnProduct_CheckedChanged", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "rbtnProduct_CheckedChanged", exMessage);
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
                FileLogger.Log(this.Name, "rbtnProduct_CheckedChanged", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "rbtnProduct_CheckedChanged", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "rbtnProduct_CheckedChanged", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "rbtnProduct_CheckedChanged", exMessage);
            }
            #endregion
        }

        private void cmbProductName_Leave(object sender, EventArgs e)
        {
            #region try
            try
            {
                var searchText = cmbProductName.Text.Trim().ToLower();
                if (searchText.Contains('~'))
                {
                    searchText = searchText.Split('~')[1];
                }
                //.Split('~')[1]
                if (!string.IsNullOrEmpty(searchText) && searchText != "select")
                {
                    if (rbtnProduct.Checked)
                    {

                        #region Value Assign

                        var prodByName = from prd in ProductsMini.ToList()
                                         where prd.ProductCode.ToLower() == searchText.ToLower()
                                         select prd;
                        if (prodByName != null && prodByName.Any())
                        {
                            var products = prodByName.First();
                            txtProductName.Text = products.ProductName;
                            cmbProduct.Text = products.ProductCode;
                            txtProductCode.Text = products.ItemNo;
                            //txtUnitCost.Text = products.IssuePrice.ToString();//"0,0.00");
                            txtUOM.Text = products.UOM;
                            cmbUom.Text = products.UOM;
                            txtHSCode.Text = products.HSCodeNo;
                            //txtQuantityInHand.Text = products.Stock.ToString();//"0,0.0000");
                            txtPCode.Text = products.ProductCode;

                            txtVATRate.Text = products.VATRate.ToString();
                            txtSDRate.Text = products.SD.ToString();

                        }

                        #endregion

                    }
                }
                string strProductCode = txtProductCode.Text;
                if (!string.IsNullOrEmpty(strProductCode))
                {
                    ProductDAL productDal = new ProductDAL();
                    //IProduct productDal = OrdinaryVATDesktop.GetObject<ProductDAL, ProductRepo, IProduct>(OrdinaryVATDesktop.IsWCF);


                    #region 62Out / 61Out

                    if (transactionType == "62Out" || txtCategoryName.Text.Trim() == "WIP")
                    {
                        #region 62Out

                        #region Stock / PirceCall

                        DataTable priceData = productDal.AvgPriceNew(strProductCode, dtpIssueDate.Value.ToString("yyyy-MMM-dd") +
                                                DateTime.Now.ToString(" HH:mm:ss"), null, null, true, true, true, true, connVM,Program.CurrentUserID);

                        txtQuantityInHand.Text = priceData.Rows[0]["Quantity"].ToString();

                        #region Comments

                        //////decimal amount = Convert.ToDecimal(priceData.Rows[0]["Amount"].ToString());
                        //////decimal quanA = Convert.ToDecimal(priceData.Rows[0]["Quantity"].ToString());
                        //////if (quanA > 0)
                        //////{
                        //////    txtUnitCost.Text = (amount / quanA).ToString();
                        //////}
                        //////else
                        //////{
                        //////    txtUnitCost.Text = "0";
                        //////}

                        #endregion

                        PirceCall();

                        Uoms();

                        #endregion

                        #endregion

                    }
                    else if (transactionType == "61Out")
                    {
                        #region 61Out

                        #region Stock and PriceCall

                        DataTable priceData = productDal.AvgPriceNew(strProductCode, dtpIssueDate.Value.ToString("yyyy-MMM-dd") +
                                                DateTime.Now.ToString(" HH:mm:ss"), null, null, true, true, true, true,connVM,Program.CurrentUserID);
                        decimal amount = Convert.ToDecimal(priceData.Rows[0]["Amount"].ToString());
                        decimal quan = Convert.ToDecimal(priceData.Rows[0]["Quantity"].ToString());

                        if (quan > 0)
                        {
                            txtUnitCost.Text = (amount / quan).ToString();
                        }
                        else
                        {
                            txtUnitCost.Text = "0";
                        }

                        txtQuantityInHand.Text = quan.ToString();

                        PirceCallIssue();

                        txtQuantity.Focus();

                        Uoms();

                        #endregion

                        #endregion
                    }

                    #endregion

                    #region Quantity
                    //ReceiveDAL _sDal = new ReceiveDAL();
                    IReceive _sDal = OrdinaryVATDesktop.GetObject<ReceiveDAL, ReceiveRepo, IReceive>(OrdinaryVATDesktop.IsWCF);

                    DataTable dt = new DataTable();
                    if (txtTrifNo.Text.Trim().Length >= 2)
                    {
                        dt = _sDal.SearchByReferenceNo(txtTrifNo.Text.Trim(), txtProductCode.Text.Trim(), connVM);
                        if (dt.Rows.Count > 0)
                        {
                            txtQuantity.Text = Program.ParseDecimal(dt.Rows[0]["Quantity"].ToString());

                        }
                    }
                    txtQuantity.Focus();

                    #endregion
                }
                //}
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

        private void cmbProductName_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void cmbProductName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.F9))
            {
                ProductLoad();
                cmbProduct.Focus();
            }
        }
        private void ProductLoad()
        {
            string saleFromProduction = commonDal.settingsDesktop("Sale", "SaleFromProduction",null,connVM);

            try
            {
                DataGridViewRow selectedRow = new DataGridViewRow();
                if (saleFromProduction == "Y" && !string.IsNullOrEmpty(txtTrifNo.Text.Trim()))
                {

                    string SqlText = @" select p.ItemNo,p.ProductCode,p.ProductName,h.ReferenceNo,d.Quantity ,d.UOM,h.ReceiveNo,h.ReceiveDateTime
                    from ReceiveDetails d
                    left outer join  ReceiveHeaders h on d.ReceiveNo=h.ReceiveNo
                    left outer join  Products p on d.ItemNo=p.ItemNo";

                    string SQLTextRecordCount = @" select count(ProductCode)RecordNo from Products";

                    string[] shortColumnName = { "h.ReferenceNo" };
                    string tableName = "";
                    FormMultipleSearch frm = new FormMultipleSearch();
                    //frm.txtSearch.Text = txtSerialNo.Text.Trim();
                    selectedRow = FormMultipleSearch.SelectOne(SqlText, tableName, txtSerialNo.Text.Trim(), shortColumnName, "", SQLTextRecordCount);
                    if (selectedRow != null && selectedRow.Index >= 0)
                    {
                        vItemNo = selectedRow.Cells["ItemNo"].Value.ToString();//ProductInfo[1];
                        txtProductCode.Text = vItemNo;

                        cmbProductName.Text = selectedRow.Cells["ProductName"].Value.ToString() + "~" + selectedRow.Cells["ProductCode"].Value.ToString();//ProductInfo[1];
                        //////cmbProduct.SelectedText = selectedRow.Cells["ProductName"].Value.ToString();//ProductInfo[1];


                    }
                }
                else if (CustomerWiseBOM)
                {
                    selectedRow = FormProductFinder.SelectOne("", varIsRaw);
                    if (selectedRow != null && selectedRow.Index >= 0)
                    {
                        //txtItemNo.Text = selectedRow.Cells["ItemNo"].Value.ToString();//ProductInfo[0];
                        vItemNo = selectedRow.Cells["ItemNo"].Value.ToString();//ProductInfo[1];
                        txtProductCode.Text = vItemNo;

                        cmbProductName.Text = selectedRow.Cells["ProductName"].Value.ToString() + "~" + selectedRow.Cells["ProductCode"].Value.ToString();//ProductInfo[1];
                        //////cmbProduct.SelectedText = selectedRow.Cells["ProductName"].Value.ToString();//ProductInfo[1];
                    }
                }
                else
                {


                    string SqlText = @"   select p.ItemNo,p.ProductCode,p.ProductName,p.UOM,pc.CategoryName,pc.IsRaw ProductType  from Products p
 left outer join ProductCategories pc on p.CategoryID=pc.CategoryID
where 1=1 and  p.ActiveStatus='Y'  ";
                    if (!string.IsNullOrEmpty(CategoryId))
                    {
                        SqlText += @"  and pc.CategoryID='" + CategoryId + "'  ";
                    }
                    else if (!string.IsNullOrEmpty(varIsRaw))
                    {
                        SqlText += @"  and pc.IsRaw='" + varIsRaw + "'  ";
                    }

                    string SQLTextRecordCount = @" select count(ProductCode)RecordNo from Products";

                    string[] shortColumnName = { "p.ItemNo", "p.ProductCode", "p.ProductName", "p.UOM", "pc.CategoryName", "pc.IsRaw" };
                    string tableName = "";
                    FormMultipleSearch frm = new FormMultipleSearch();
                    //frm.txtSearch.Text = txtSerialNo.Text.Trim();
                    selectedRow = FormMultipleSearch.SelectOne(SqlText, tableName, "", shortColumnName, null, SQLTextRecordCount);
                    if (selectedRow != null && selectedRow.Index >= 0)
                    {
                        vItemNo = selectedRow.Cells["ItemNo"].Value.ToString();//ProductInfo[1];
                        txtProductCode.Text = vItemNo;
                        cmbProductName.SelectedText = selectedRow.Cells["ProductName"].Value.ToString() + "~" + selectedRow.Cells["ProductCode"].Value.ToString();//ProductInfo[1];

                    }

                }
            }
            catch (Exception ex)
            {
                FileLogger.Log(this.Name, "ProductLoad", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        #endregion

        #region Methods 06

        private void BrowsFile()
        {
            #region try
            try
            {
                OpenFileDialog fdlg = new OpenFileDialog();
                fdlg.Title = "VAT Import Open File Dialog";
                fdlg.InitialDirectory = @"c:\";
                fdlg.Filter = "Excel files (*.xlsx*)|*.xlsx*|Excel(97-2003)files (*.xls*)|*.xls*|Text files (*.txt*)|*.txt*";
                //fdlg.Filter = "CSV files (*.csv*)|*.csv*|Text files (*.txt*)|*.txt*";
                fdlg.Multiselect = true;
                fdlg.FilterIndex = 2;
                fdlg.RestoreDirectory = true;
                int count = 0;
                if (fdlg.ShowDialog() == DialogResult.OK)
                {
                    foreach (String file in fdlg.FileNames)
                    {
                        //Program.ImportFileName = fdlg.FileName;
                        if (count == 0)
                        {
                            Program.ImportFileName = file;
                        }
                        else
                        {
                            Program.ImportFileName = Program.ImportFileName + " ; " + file;
                        }
                        count++;
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

        private void cmbTransferTo_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void cmbTransferTo_Leave(object sender, EventArgs e)
        {
            try
            {
                #region Statement
                txtBranchDBName.Text = "";
                var searchText = cmbTransferTo.Text.Trim().ToLower();
                if (!string.IsNullOrEmpty(searchText) && searchText != "select")
                {
                    var branchs = new List<BranchDTO>();
                    branchs = BranchLists.Where(x => x.Name.ToString().ToLower() == searchText).ToList();

                    if (branchs.Any())
                    {
                        var branch = branchs.First();
                        txtBranchDBName.Text = branch.DBName.ToString().Trim();
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
                FileLogger.Log(this.Name, "cmbCustomerName_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "cmbCustomerName_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (FormatException ex)
            {
                FileLogger.Log(this.Name, "cmbCustomerName_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                FileLogger.Log(this.Name, "cmbCustomerName_Leave", exMessage);
            }
            #endregion
        }

        private void btn6_5_Click(object sender, EventArgs e)
        {

            try
            {
                if (txtIssueNo.Text == "")
                {
                    MessageBox.Show("Please , Select Issue No First!", this.Text);
                    return;
                }
                PreviewDetails(true);

            }
            catch (Exception)
            {

                throw;
            }
            //FormRptVAT6_5 frm = new FormRptVAT6_5();

            //if (txtIssueNo.Text == "~~~ New ~~~")
            //{
            //    frm.txtIssueNo.Text = "";
            //}
            //else
            //{
            //    frm.txtIssueNo.Text = txtIssueNo.Text.Trim();
            //}
            //frm.rbtn61Out.Checked = rbtn61Out.Checked;
            //frm.rbtn62Out.Checked = rbtn62Out.Checked;
            ////////////frm.txtIssueNo.Text = transactionType;
            //frm.ShowDialog();

        }


        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btn62_Click(object sender, EventArgs e)
        {
            try
            {
                #region Statement

                if (Program.CheckLicence(dtpIssueDate.Value) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }
                MDIMainInterface mdi = new MDIMainInterface();
                FormRptVAT17 frmRptVAT17 = new FormRptVAT17();

                //mdi.RollDetailsInfo("8301");
                //if (Program.Access != "Y")
                //{
                //    MessageBox.Show("You do not have to access this form", frmRptVAT17.Text, MessageBoxButtons.OK,
                //                    MessageBoxIcon.Information);
                //    return;
                //}
                if (dgvIssue.Rows.Count > 0)
                {
                    frmRptVAT17.txtItemNo.Text = dgvIssue.CurrentRow.Cells["ItemNo"].Value.ToString();
                    frmRptVAT17.txtProductName.Text = dgvIssue.CurrentRow.Cells["ItemName"].Value.ToString();
                    frmRptVAT17.dtpFromDate.Value = dtpIssueDate.Value;
                    frmRptVAT17.dtpToDate.Value = dtpIssueDate.Value;

                }



                frmRptVAT17.Show();

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
                FileLogger.Log(this.Name, "btnVAT17_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnVAT17_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnVAT17_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnVAT17_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnVAT17_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnVAT17_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnVAT17_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnVAT17_Click", exMessage);
            }

            #endregion

        }

        private void dgvIssue_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void label25_Click(object sender, EventArgs e)
        {

        }

        private void label25_Click_1(object sender, EventArgs e)
        {

        }

        private void txtBranchDBName_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                TransactionTypes();


                CommonDAL commonDal = new CommonDAL();

                string value = commonDal.settingValue("CompanyCode", "Code",connVM);


                if (value.ToLower() == "kccl")
                {
                    FormTransferImportKCCL fmi = new FormTransferImportKCCL();
                    fmi.preSelectTable = "TransferIssues";
                    fmi.transactionType = transactionType;
                    fmi.Show();
                }
                else
                {
                    FormMasterImport fmi = new FormMasterImport();
                    fmi.preSelectTable = "TransferIssues";
                    fmi.transactionType = transactionType;
                    fmi.Show();
                }



            }
            catch (Exception)
            {

                throw;
            }

        }

        private void bgwP_DoWork(object sender, DoWorkEventArgs e)
        {

        }

        private void btnImport_Click_1(object sender, EventArgs e)
        {
            TransactionTypes();


            CommonDAL commonDal = new CommonDAL();

            string value = commonDal.settingValue("CompanyCode", "Code");

            if (value.ToLower() == "kccl")
            {
                string importId = FormTransferImportKCCL.SelectOne(transactionType);

                ITransferIssue issueDal = OrdinaryVATDesktop.GetObject<TransferIssueDAL, TransferIssueRepo, ITransferIssue>(OrdinaryVATDesktop.IsWCF);
                if (!string.IsNullOrEmpty(importId))
                {
                    TransferIssueVM vm = new TransferIssueVM();
                    vm.TransferIssueNo = "";
                    vm.IssueDateFrom = "";
                    vm.IssueDateTo = "";
                    vm.Post = "";
                    vm.TransactionType = transactionType;
                    vm.ReferenceNo = importId;
                    vm.BranchId = Program.BranchId;
                    DataTable result = issueDal.SearchTransferIssue(vm, null, null, connVM);  // Change 04
                    LoadForm(result);

                    btnSave.Text = "&Save";
                    IsUpdate = true;
                    ChangeData = false;
                }

            }
            else if (value.ToLower() == "BERGER".ToLower())
            {
                string importId = FormTransferIssueImportBerger.SelectOne(transactionType);
            }
            else if (value.ToLower() == "sqr")
            {
                string importId = FormTransferImportSQR.SelectOne(transactionType);

                TransferIssueDAL issueDal = new TransferIssueDAL();
                if (!string.IsNullOrEmpty(importId))
                {
                    TransferIssueVM vm = new TransferIssueVM();
                    vm.TransferIssueNo = "";
                    vm.IssueDateFrom = "";
                    vm.IssueDateTo = "";
                    vm.Post = "";
                    vm.TransactionType = transactionType;
                    vm.ReferenceNo = importId;
                    vm.BranchId = Program.BranchId;
                    vm.UserId = Program.CurrentUserID;
                    DataTable result = issueDal.SearchTransferIssue_TempTable(vm, null, null, connVM);  // Change 04
                    LoadForm_FromTempTable(result);

                    btnSave.Text = "&Save";
                    IsUpdate = true;
                    ChangeData = false;
                }

            }
            else if (value.ToLower() == "bata")
            {
                string importId = FormTransferImportBata.SelectOne(transactionType);

                TransferIssueDAL issueDal = new TransferIssueDAL();
                if (!string.IsNullOrEmpty(importId))
                {
                    TransferIssueVM vm = new TransferIssueVM();
                    vm.TransferIssueNo = "";
                    vm.IssueDateFrom = "";
                    vm.IssueDateTo = "";
                    vm.Post = "";
                    vm.TransactionType = transactionType;
                    vm.ReferenceNo = importId;
                    vm.BranchId = Program.BranchId;
                    vm.UserId = Program.CurrentUserID;
                    DataTable result = issueDal.SearchTransferIssue(vm, null, null, connVM);  // Change 04
                    LoadForm(result);

                    btnSave.Text = "&Save";
                    IsUpdate = true;
                    ChangeData = false;
                }

            }
            else if (OrdinaryVATDesktop.IsACICompany(value))
            {

                //FormTransferImportACI aci = new FormTransferImportACI();

                //aci.preSelectTable = "TransferIssues";
                //aci.transactionType = transactionType;
                //aci.Show();


                string importId = FormTransferImportACI.SelectOne(transactionType);

                TransferIssueDAL issueDal = new TransferIssueDAL();
                if (!string.IsNullOrEmpty(importId))
                {
                    TransferIssueVM vm = new TransferIssueVM();
                    vm.TransferIssueNo = "";
                    vm.IssueDateFrom = "";
                    vm.IssueDateTo = "";
                    vm.Post = "";
                    vm.TransactionType = transactionType;
                    vm.ReferenceNo = importId;
                    vm.BranchId = Program.BranchId;
                    vm.UserId = Program.CurrentUserID;
                    DataTable result = issueDal.SearchTransferIssue(vm, null, null, connVM);  // Change 04
                    LoadForm(result);

                    btnSave.Text = "&Save";
                    IsUpdate = true;
                    ChangeData = false;
                }

            }
            else if (OrdinaryVATDesktop.IsNourishCompany(value))
            {

                //FormTransferImportACI aci = new FormTransferImportACI();

                //aci.preSelectTable = "TransferIssues";
                //aci.transactionType = transactionType;
                //aci.Show();

                string importId = FormTransferImportNourish.SelectOne(transactionType);

                //TransferIssueDAL issueDal = new TransferIssueDAL();
                //if (!string.IsNullOrEmpty(importId))
                //{
                //    TransferIssueVM vm = new TransferIssueVM();
                //    vm.TransferIssueNo = "";
                //    vm.IssueDateFrom = "";
                //    vm.IssueDateTo = "";
                //    vm.Post = "";
                //    vm.TransactionType = transactionType;
                //    vm.ReferenceNo = importId;
                //    vm.BranchId = Program.BranchId;
                //    vm.UserId = Program.CurrentUserID;
                //    DataTable result = issueDal.SearchTransferIssue(vm, null, null, connVM);  // Change 04
                //    LoadForm(result);

                //    btnSave.Text = "&Save";
                //    IsUpdate = true;
                //    ChangeData = false;
                //}

            }
            else if (value.ToLower() == "SMC".ToLower() || value.ToLower() == "smcholding")
            {
                ////FormTransferImportSMC FormImport = new FormTransferImportSMC();

                ////FormImport.preSelectTable = "TransferIssues";
                ////FormImport.transactionType = transactionType;
                ////FormImport.Show();

                string importId = FormTransferImportSMC.SelectOne(transactionType);

                ITransferIssue issueDal = OrdinaryVATDesktop.GetObject<TransferIssueDAL, TransferIssueRepo, ITransferIssue>(OrdinaryVATDesktop.IsWCF);
                if (!string.IsNullOrEmpty(importId))
                {
                    TransferIssueVM vm = new TransferIssueVM();
                    vm.TransferIssueNo = "";
                    vm.IssueDateFrom = "";
                    vm.IssueDateTo = "";
                    vm.Post = "";
                    vm.TransactionType = transactionType;
                    vm.ReferenceNo = importId;
                    ////vm.BranchId = Program.BranchId;
                    DataTable result = issueDal.SearchTransferIssue(vm, null, null, connVM);  // Change 04
                    LoadForm(result);

                    btnSave.Text = "&Save";
                    IsUpdate = true;
                    ChangeData = false;
                }


            }
            else if (value.ToUpper() == "PUROFOOD" || value.ToUpper() == "EON" || value.ToLower() == "eahpl" || value.ToLower() == "eail" || value.ToLower() == "eeufl" || value.ToLower() == "exfl")
            {
                string importId = FormTransferIssueImportEON.SelectOne(transactionType);

                ////TransferIssueDAL issueDal = new TransferIssueDAL();
                ////if (!string.IsNullOrEmpty(importId))
                ////{
                ////    TransferIssueVM vm = new TransferIssueVM();
                ////    vm.TransferIssueNo = "";
                ////    vm.IssueDateFrom = "";
                ////    vm.IssueDateTo = "";
                ////    vm.Post = "";
                ////    vm.TransactionType = transactionType;
                ////    vm.ReferenceNo = importId;
                ////    vm.BranchId = Program.BranchId;
                ////    vm.UserId = Program.CurrentUserID;
                ////    DataTable result = issueDal.SearchTransferIssue(vm, null, null, connVM);  // Change 04
                ////    LoadForm(result);

                ////    btnSave.Text = "&Save";
                ////    IsUpdate = true;
                ////    ChangeData = false;
                ////}

            }
            else
            {
                //FormMasterImport fmi = new FormMasterImport();
                //fmi.preSelectTable = "TransferIssues";
                //fmi.transactionType = transactionType;
                //fmi.Show();

                string importId = FormMasterImport.SelectOne(transactionType);

                ITransferIssue issueDal = OrdinaryVATDesktop.GetObject<TransferIssueDAL, TransferIssueRepo, ITransferIssue>(OrdinaryVATDesktop.IsWCF);
                if (!string.IsNullOrEmpty(importId))
                {
                    TransferIssueVM vm = new TransferIssueVM();
                    vm.TransferIssueNo = "";
                    vm.IssueDateFrom = "";
                    vm.IssueDateTo = "";
                    vm.Post = "";
                    vm.TransactionType = transactionType;
                    vm.ReferenceNo = importId;
                    vm.BranchId = Program.BranchId;
                    DataTable result = issueDal.SearchTransferIssue(vm, null, null, connVM);  // Change 04
                    LoadForm(result);

                    btnSave.Text = "&Save";
                    IsUpdate = true;
                    ChangeData = false;
                }
            }
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void txtUnitCost_Leave(object sender, EventArgs e)
        {
            //Program.FormatTextBox(txtUnitCost, "UnitCost");
            txtUnitCost.Text = Program.ParseDecimalObject(txtUnitCost.Text.Trim()).ToString();
            SDVATAmount();
        }

        #endregion

        #region Methods 07

        private void txtSDAmount_Leave(object sender, EventArgs e)
        {
            //Program.FormatTextBox(txtSDAmount, "SDAmount");
            txtSDAmount.Text = Program.ParseDecimalObject(txtSDAmount.Text.Trim()).ToString();
        }

        private void txtVATAmount_Leave(object sender, EventArgs e)
        {
            //Program.FormatTextBox(txtVATAmount, "VATAmount");
            txtVATAmount.Text = Program.ParseDecimalObject(txtVATAmount.Text.Trim()).ToString();
        }

        private void txtQuantityInHand_Leave(object sender, EventArgs e)
        {
            //Program.FormatTextBox(txtQuantityInHand, "QuantityInHand");
            txtQuantityInHand.Text = Program.ParseDecimalObject(txtQuantityInHand.Text.Trim()).ToString();
        }

        private void txtTotalAmount_Leave(object sender, EventArgs e)
        {
            //Program.FormatTextBox(txtTotalAmount, "TotalAmount");
            txtTotalAmount.Text = Program.ParseDecimalObject(txtTotalAmount.Text.Trim()).ToString();
        }

        private void dtpIssueDate_Leave(object sender, EventArgs e)
        {
            dtpIssueDate.Value = Program.ParseDate(dtpIssueDate);
        }

        private void txtVehicleNo_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.KeyCode.Equals(Keys.F9))
            {
                VehicleLoad();
            }
        }

        private void VehicleLoad()
        {
            try
            {


                DataGridViewRow selectedRow = new DataGridViewRow();
                string SqlText = @" select  VehicleID,VehicleNo,VehicleType,Description,Comments from Vehicles
 where 1=1 and ActiveStatus='Y' ";


                string SQLTextRecordCount = @" select count(VehicleNo)RecordNo from Vehicles";

                string[] shortColumnName = { "VehicleNo", "VehicleType" };
                string tableName = "";
                FormMultipleSearch frm = new FormMultipleSearch();
                //frm.txtSearch.Text = txtSerialNo.Text.Trim();
                selectedRow = FormMultipleSearch.SelectOne(SqlText, tableName, "", shortColumnName, "", SQLTextRecordCount);


                //string[] shortColumnName = {  "VehicleNo", "VehicleType"};
                //string tableName = "Vehicles";
                //selectedRow = FormMultipleSearch.SelectOne("", tableName, "", shortColumnName);
                if (selectedRow != null && selectedRow.Index >= 0)
                {
                    txtVehicleNo.Text = selectedRow.Cells["VehicleNo"].Value.ToString();//products.VehicleNo;
                    txtVehicleType.Text = selectedRow.Cells["VehicleType"].Value.ToString();//products.VehicleNo;

                }
            }
            catch (Exception ex)
            {
                FileLogger.Log(this.Name, "VehicleLoad", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void txtVehicleNo_DoubleClick(object sender, EventArgs e)
        {
            VehicleLoad();
        }

        private void TripLoad()
        {
            CommonDAL commonDal = new CommonDAL();

            String saleFromProduction = commonDal.settingsDesktop("Sale", "SaleFromProduction");
            try
            {
                if (saleFromProduction.ToLower() == "y")
                {
                    //ReceiveDAL _sDal = new ReceiveDAL();
                    IReceive _sDal = OrdinaryVATDesktop.GetObject<ReceiveDAL, ReceiveRepo, IReceive>(OrdinaryVATDesktop.IsWCF);

                    DataTable dt = new DataTable();
                    dt = _sDal.SearchByReferenceNo(txtTrifNo.Text.Trim(), "", connVM);

                    if (dt.Rows.Count <= 0)
                    {

                        MessageBox.Show("This Ref/Trip # Not found .");
                        txtTrifNo.Text = "";
                        return;
                    }
                    string IsTripComplete = dt.Rows[0]["IsTripComplete"].ToString();
                    if (IsTripComplete.ToLower() == "y")
                    {
                        MessageBox.Show("This Ref/Trip # Already Used.");
                        txtTrifNo.Text = "";
                        return;
                    }
                    else
                    {

                        cmbProduct.DataSource = null;
                        cmbProduct.Items.Clear();
                        cmbProductName.DataSource = dt;
                        cmbProductName.DisplayMember = "ProductName";// displayMember.Replace("[", "").Replace("]", "").Trim();
                        cmbProductName.ValueMember = "ItemNo";

                    }


                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void txtTrifNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                TripLoad();

                cmbProductName.Focus();
            }
            //private void btnSearchIssueTo_Click(object sender, EventArgs e)
            //{

            //    #region try
            //    try
            //    {
            //        Program.fromOpen = "Me";
            //        TransactionTypes();
            //        string invoiceNo = FormTransferIssueSearch.SelectOne(transactionType);
            //        TransferIssueDAL issueDal = new TransferIssueDAL();
            //        TransferIssueVM vm = new TransferIssueVM();
            //        vm.TransferIssueNo = invoiceNo;
            //        vm.IssueDateFrom = "";
            //        vm.IssueDateTo = "";
            //        vm.Post = "";
            //        vm.TransactionType = transactionType;

            //        DataTable IssueResult = issueDal.SearchTransferIssue(vm);  // Change 04
            //        if (IssueResult.Rows.Count > 0)
            //        {
            //            txtIssueNo.Text = IssueResult.Rows[0]["TransferIssueNo"].ToString();
            //            dtpIssueDate.Value = Convert.ToDateTime(IssueResult.Rows[0]["IssueDateTime"].ToString());
            //            txtTotalAmount.Text = Convert.ToDecimal(IssueResult.Rows[0]["TotalAmount"].ToString()).ToString();//"0,0.00");
            //            txtSerialNo.Text = IssueResult.Rows[0]["SerialNo"].ToString();
            //            txtReferenceNo.Text = IssueResult.Rows[0]["ReferenceNo"].ToString();
            //            txtComments.Text = IssueResult.Rows[0]["Comments"].ToString();
            //            txtBranchDBName.Text = IssueResult.Rows[0]["TransferTo"].ToString();
            //            cmbTransferTo.Text = IssueResult.Rows[0]["BranchName"].ToString();
            //            IsPost = Convert.ToString(IssueResult.Rows[0]["Post"].ToString()) == "Y" ? true : false;
            //        }
            //        //string IssueDetailData;
            //        IssueDetailData = txtIssueNo.Text == "" ? "" : txtIssueNo.Text.Trim();
            //        #region Statement
            //        IssueDetailResult = new DataTable();
            //        if (!string.IsNullOrEmpty(invoiceNo))
            //        {
            //            IssueDetailResult = issueDal.SearchTransferDetail(invoiceNo); // Change 04
            //        }
            //        #region DataGridView
            //        dgvIssue.Rows.Clear();
            //        int j = 0;
            //        foreach (DataRow item in IssueDetailResult.Rows)
            //        {
            //            DataGridViewRow NewRow = new DataGridViewRow();
            //            dgvIssue.Rows.Add(NewRow);
            //            dgvIssue.Rows[j].Cells["LineNo"].Value = item["IssueLineNo"].ToString();        // IssueDetailFields[1].ToString();
            //            dgvIssue.Rows[j].Cells["ItemNo"].Value = item["ItemNo"].ToString();             // IssueDetailFields[2].ToString();
            //            dgvIssue.Rows[j].Cells["Quantity"].Value = item["Quantity"].ToString();         //Convert.ToDecimal(IssueDetailFields[3].ToString()).ToString();//"0,0.0000");
            //            dgvIssue.Rows[j].Cells["UnitPrice"].Value = item["CostPrice"].ToString();       //Convert.ToDecimal(IssueDetailFields[4].ToString()).ToString();//"0,0.00");
            //            dgvIssue.Rows[j].Cells["UOM"].Value = item["UOM"].ToString();                   // IssueDetailFields[6].ToString();
            //            dgvIssue.Rows[j].Cells["SubTotal"].Value = item["SubTotal"].ToString();         //Convert.ToDecimal(IssueDetailFields[9].ToString()).ToString();//"0,0.00");
            //            dgvIssue.Rows[j].Cells["Comments"].Value = item["Comments"].ToString();         // IssueDetailFields[10].ToString();
            //            dgvIssue.Rows[j].Cells["ItemName"].Value = item["ProductName"].ToString();      // IssueDetailFields[11].ToString();
            //            dgvIssue.Rows[j].Cells["PCode"].Value = item["ProductCode"].ToString();         // IssueDetailFields[15].ToString();
            //            dgvIssue.Rows[j].Cells["Status"].Value = "Old";
            //            dgvIssue.Rows[j].Cells["Previous"].Value = item["Quantity"].ToString();         //Convert.ToDecimal(IssueDetailFields[3].ToString()).ToString();//"0,0.0000"); //Quantity
            //            dgvIssue.Rows[j].Cells["Stock"].Value = item["Stock"].ToString();               //Convert.ToDecimal(IssueDetailFields[12].ToString()).ToString();//"0,0.0000");
            //            dgvIssue.Rows[j].Cells["Change"].Value = 0;
            //            dgvIssue.Rows[j].Cells["UOMc"].Value = item["UOMc"].ToString();
            //            dgvIssue.Rows[j].Cells["UOMn"].Value = item["UOMn"].ToString();
            //            dgvIssue.Rows[j].Cells["UOMQty"].Value = item["UOMQty"].ToString();
            //            dgvIssue.Rows[j].Cells["UOMPrice"].Value = item["UOMPrice"].ToString();

            //            dgvIssue.Rows[j].Cells["VATRate"].Value = item["VATRate"].ToString();
            //            dgvIssue.Rows[j].Cells["VATAmount"].Value = item["VATAmount"].ToString();
            //            dgvIssue.Rows[j].Cells["SDRate"].Value = item["SDRate"].ToString();
            //            dgvIssue.Rows[j].Cells["SDAmount"].Value = item["SDAmount"].ToString();

            //            j = j + 1;
            //        }  //End For
            //        #endregion DataGridView
            //        btnSave.Text = "&Save";
            //        IsUpdate = true;
            //        ChangeData = false;
            //        #endregion
            //        backgroundWorkerSearchIssueNo.RunWorkerAsync();
            //    }
            //    #endregion
            //    #region catch
            //    catch (ArgumentNullException ex)
            //    {
            //        string err = ex.Message.ToString();
            //        string[] error = err.Split(FieldDelimeter.ToCharArray());
            //        FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
            //        MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    }
            //    catch (IndexOutOfRangeException ex)
            //    {
            //        FileLogger.Log(this.Name, "btnSearchIssueTo_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            //        MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    }
            //    catch (NullReferenceException ex)
            //    {
            //        FileLogger.Log(this.Name, "btnSearchIssueTo_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            //        MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    }
            //    catch (FormatException ex)
            //    {
            //        FileLogger.Log(this.Name, "btnSearchIssueTo_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            //        MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    }
            //    catch (SoapHeaderException ex)
            //    {
            //        string exMessage = ex.Message;
            //        if (ex.InnerException != null)
            //        {
            //            exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
            //                        ex.StackTrace;
            //        }
            //        FileLogger.Log(this.Name, "btnSearchIssueTo_Click", exMessage);
            //        MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    }
            //    catch (SoapException ex)
            //    {
            //        string exMessage = ex.Message;
            //        if (ex.InnerException != null)
            //        {
            //            exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
            //                        ex.StackTrace;
            //        }
            //        MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //        FileLogger.Log(this.Name, "btnSearchIssueTo_Click", exMessage);
            //    }
            //    catch (EndpointNotFoundException ex)
            //    {
            //        MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //        FileLogger.Log(this.Name, "btnSearchIssueTo_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            //    }
            //    catch (WebException ex)
            //    {
            //        MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //        FileLogger.Log(this.Name, "btnSearchIssueTo_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            //    }
            //    catch (Exception ex)
            //    {
            //        string exMessage = ex.Message;
            //        if (ex.InnerException != null)
            //        {
            //            exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
            //                        ex.StackTrace;
            //        }
            //        MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //        FileLogger.Log(this.Name, "btnSearchIssueTo_Click", exMessage);
            //    }
            //    #endregion
            //    finally
            //    {
            //        ChangeData = false;
            //        this.btnSearchIssueTo.Enabled = true;
            //        this.progressBar1.Visible = false;
            //    }
            //}
        }

        private void btnTripLoad_Click(object sender, EventArgs e)
        {
            TripLoad();

        }

        private void label25_Click_2(object sender, EventArgs e)
        {

        }

        private void txtTrifNo_TextChanged(object sender, EventArgs e)
        {

        }

        #endregion

        private void cmbBOMReferenceNo_Leave(object sender, EventArgs e)
        {
            try
            {

                DataTable dt = new DataTable();
                ProductDAL _sDal = new ProductDAL();

                //IProduct _sDal = OrdinaryVATDesktop.GetObject<ProductDAL, ProductRepo, IProduct>(OrdinaryVATDesktop.IsWCF);

                //BugsBD
                string ProductCode = OrdinaryVATDesktop.SanitizeInput(txtProductCode.Text.Trim());
                string VAT1Name = OrdinaryVATDesktop.SanitizeInput(cmbVAT1Name.Text.Trim());

                dt = _sDal.GetBOMReferenceNo(ProductCode, VAT1Name, dtpIssueDate.Value.ToString("yyyy-MMM-dd"), null, null, "0", connVM);

                //dt = _sDal.GetBOMReferenceNo(txtProductCode.Text.Trim(), cmbVAT1Name.Text.Trim(),
                                                                            //dtpIssueDate.Value.ToString("yyyy-MMM-dd"), null, null, "0", connVM);

                #region Comments

                //////if (CustomerWiseBOM)
                //////{
                //////    dt = new ProductDAL().GetBOMReferenceNo(txtProductCode.Text.Trim(), cmbVAT1Name.Text.Trim(),
                //////                                                             dtpReceiveDate.Value.ToString("yyyy-MMM-dd"), null, null, vCustomerID);

                //////}
                //////else
                //////{
                //////    dt = new ProductDAL().GetBOMReferenceNo(txtProductCode.Text.Trim(), cmbVAT1Name.Text.Trim(),
                //////                                  dtpReceiveDate.Value.ToString("yyyy-MMM-dd"), null, null);

                //////}

                #endregion

                txtBOMReferenceNo.Text = cmbBOMReferenceNo.Text;
                txtBOMId.Text = "";
                SetBOM(dt);

            }
            #region Catch

            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "cmbReferenceNo_Leave", exMessage);
            }
            #endregion Catch
        }

        private void SetBOM(DataTable dt)
        {
            try
            {
                int BOMId = 0;

                if (dt != null && dt.Rows.Count > 0)
                {
                    #region ReferenceNo
                    if (!string.IsNullOrWhiteSpace(txtBOMId.Text))
                    {
                        IBOM _sDal = OrdinaryVATDesktop.GetObject<BOMDAL, BOMRepo, IBOM>(OrdinaryVATDesktop.IsWCF);

                        BOMNBRVM BOMNBRVM = _sDal.SelectAllList(txtBOMId.Text, null, null, null, null, null, connVM).FirstOrDefault();

                        if (BOMNBRVM != null)
                        {
                            txtBOMReferenceNo.Text = BOMNBRVM.ReferenceNo;
                        }

                    }


                    string ReferenceNo = txtBOMReferenceNo.Text;

                    DataView dv = new DataView(dt);
                    dv.RowFilter = "ReferenceNo = '" + ReferenceNo + "'";

                    DataTable dtBOM = new DataTable();
                    dtBOM = dv.ToTable();


                    #endregion

                    #region BOMId
                    if (dtBOM != null && dtBOM.Rows.Count > 0)
                    {
                        DataRow dr = dtBOM.Rows[0];
                        string tempBOMId = dr["BOMId"].ToString();
                        if (!string.IsNullOrWhiteSpace(tempBOMId))
                        {
                            BOMId = Convert.ToInt32(tempBOMId);
                        }

                        if (!string.IsNullOrWhiteSpace(ReferenceNo))
                        {
                            cmbBOMReferenceNo.Text = ReferenceNo;
                        }
                    }
                    #endregion
                }


                txtBOMId.Text = BOMId.ToString();

            }
            #region Catch

            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "cmbReferenceNo_Leave", exMessage);
            }
            #endregion Catch
        }

        private void btPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (IsPost == false)
                {
                    MessageBox.Show("This transaction not posted, please post first", this.Text);
                    return;
                }
                PreviewOnly = false;

                this.progressBar1.Visible = true;
                Program.fromOpen = "Me";
                string result = FormSalePrint.SelectOne(AlReadyPrintNo);
                string[] PrintResult = result.Split(FieldDelimeter.ToCharArray());
                WantToPrint = PrintResult[1];
                if (WantToPrint == "N")
                {
                    this.progressBar1.Visible = false;
                    return;


                }
                else
                {

                    WantToPrint = PrintResult[1];
                    VPrinterName = PrintResult[2];

                }
                PreviewDetails(false);

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
                FileLogger.Log(this.Name, "btPrint_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btPrint_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btPrint_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btPrint_Click", exMessage);
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
                FileLogger.Log(this.Name, "btPrint_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btPrint_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btPrint_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btPrint_Click", exMessage);
            }

            #endregion
        }

        private void PreviewDetails(bool PreviewOnly = false)
        {
            #region Try
            try
            {
                CommonDAL cdal = new CommonDAL();
                string VAT11Name = cdal.settingsDesktop("Reports", "VAT6_3",null,connVM);

                #region getData

                 ReportDocument reportDocument = new ReportDocument();


                NBRReports _report = new NBRReports();
                _report.VAT11Name = VAT11Name;
                reportDocument = _report.TransferIssueNew(txtIssueNo.Text.Trim(), "", "", "", "", "", "", "", PreviewOnly,null,connVM);
                ////////DBName = cmbBranchName.SelectedValue.ToString();
                //DataSet ReportResult = new DataSet();
                //ReportDSDAL reportDsdal = new ReportDSDAL();
                //ReportResult = reportDsdal.TransferIssueNew(
                //     txtIssueNo.Text.Trim()
                //     , ""
                //     , ""
                //     , ""
                //     , ""
                //     , ""
                //     , ""
                //     , ""
                //     );
                #endregion
                #region showReport
                #region Comment
                //if (ReportResult.Tables.Count <= 0)
                //{
                //    MessageBox.Show("There is no data to preview", this.Text, MessageBoxButtons.OK,
                //                    MessageBoxIcon.Information);
                //    return;
                //}

                //ReportResult.Tables[0].TableName = "DsTransferIssue";
                //ReportDocument objrpt = new ReportDocument();
                //var dal = new BranchProfileDAL();

                //var toVM = dal.SelectAll(ReportResult.Tables[0].Rows[0]["TransferTo"].ToString(), null, null, null, null).FirstOrDefault();
                //var FromVM = dal.SelectAll(ReportResult.Tables[0].Rows[0]["FromBranchId"].ToString(), null, null, null, null).FirstOrDefault();

                //var legalName = toVM == null ? branchName : toVM.BranchLegalName;
                //var legalName1 = FromVM == null ? branchName : FromVM.BranchLegalName;
                //var fromBranchAddress = FromVM == null ? "-" : FromVM.Address;

                //if (VAT11Name.ToLower() == "scbl")
                //{
                //    UserInformationVM uvm = new UserInformationVM();
                //    UserInformationDAL _udal = new UserInformationDAL();

                //    uvm = _udal.SelectAll(Convert.ToInt32(Program.CurrentUserID)).FirstOrDefault();
                //    objrpt = new RptVAT6_5_SCBL();
                //    //objrpt.Load(Program.ReportAppPath + @"\RptVAT6_5_SCBL.rpt");

                //    objrpt.DataDefinition.FormulaFields["UserFullName"].Text = "'" + uvm.FullName + "'";
                //    objrpt.DataDefinition.FormulaFields["UserDesignation"].Text = "'" + uvm.Designation + "'";

                //}
                //else
                //{
                //    objrpt = new RptVAT6_5();


                //}
                ////objrpt = new RptVAT6_5();
                ////objrpt.Load(Program.ReportAppPath + @"\RptVAT6_5.rpt");




                //objrpt.SetDataSource(ReportResult);
                //objrpt.DataDefinition.FormulaFields["UserName"].Text = "'" + Program.CurrentUser + "'";
                //objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + Program.CompanyName + "'";
                //objrpt.DataDefinition.FormulaFields["Address1"].Text =   "'" + Program.Address1 + "'";
                //objrpt.DataDefinition.FormulaFields["Address2"].Text = "'" + Program.Address2 + "'";
                //objrpt.DataDefinition.FormulaFields["Address3"].Text = "'" + Program.Address3 + "'";
                //objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + Program.TelephoneNo + "'";
                //objrpt.DataDefinition.FormulaFields["VATRegNo"].Text = "'" + Program.VatRegistrationNo + "'";
                //objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + Program.FaxNo + "'";
                //objrpt.DataDefinition.FormulaFields["BranchName"].Text = "'" + legalName + "'";
                //objrpt.DataDefinition.FormulaFields["FromBranchName"].Text = "'" + legalName1 + "'";
                //objrpt.DataDefinition.FormulaFields["FromBranchAddress"].Text = "'" + fromBranchAddress + "'";


                //FormulaFieldDefinitions crFormulaF;
                //crFormulaF = objrpt.DataDefinition.FormulaFields;
                //CommonFormMethod _vCommonFormMethod = new CommonFormMethod();

                //_vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", Program.FontSize);
                #endregion
                if (reportDocument == null)
                {
                    MessageBox.Show("There is no data to preview", this.Text, MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);
                    return;
                }



                if (PreviewOnly == true)
                {
                    FormReport reports = new FormReport();
                    //objrpt.DataDefinition.FormulaFields["Trial"].Text = "'" + Program.Trial + "'";
                    reports.crystalReportViewer1.Refresh();
                    reports.setReportSource(reportDocument);
                    //reports.ShowDialog();
                    reports.WindowState = FormWindowState.Maximized;
                    reports.Show();
                }
                else
                {
                    ////var VPrinterName = new CommonDAL().settingValue("Printer", "DefaultPrinter");

                    ////reportDocument.PrintOptions.PrinterName = VPrinterName;
                    ////reportDocument.PrintToPrinter(1, false, 0, 0);



                    System.Drawing.Printing.PrinterSettings printerSettings = new System.Drawing.Printing.PrinterSettings();
                    printerSettings.PrinterName = VPrinterName;
                    reportDocument.PrintToPrinter(printerSettings, new PageSettings(), false);


                    MessageBox.Show("You have successfully print ");

                }

            }
                #endregion
            #endregion
            #region Catch
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
                FileLogger.Log(this.Name, "PreviewDetails",
                               exMessage);
            }
            #endregion
            #region Finally
            finally
            {
                this.progressBar1.Visible = false;
                this.btnPrint.Enabled = true;
            }
            #endregion
        }

        private void txtTotalQuantity_Leave(object sender, EventArgs e)
        {
            txtTotalQuantity.Text = Program.ParseDecimalObject(txtTotalQuantity.Text.Trim()).ToString();
        }

        #region Purchase Navigation

        private void btnFirst_Click(object sender, EventArgs e)
        {
            TransferIssueNavigation("First");

        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            TransferIssueNavigation("Previous");
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            TransferIssueNavigation("Next");
        }

        private void btnLast_Click(object sender, EventArgs e)
        {
            TransferIssueNavigation("Last");
        }

        private void TransferIssueNavigation(string ButtonName)
        {
            try
            {
                TransferIssueDAL _TransferIssueDAL = new TransferIssueDAL();
                NavigationVM vm = new NavigationVM();
                vm.ButtonName = ButtonName;

                vm.FiscalYear = Convert.ToInt32(txtFiscalYear.Text);
                vm.BranchId = Convert.ToInt32(SearchBranchId);
                vm.TransactionType = transactionType;
                vm.Id = Convert.ToInt32(txtId.Text);
                vm.InvoiceNo = txtIssueNo.Text;


                if (vm.BranchId == 0)
                {
                    vm.BranchId = Program.BranchId;
                }

                vm = _TransferIssueDAL.TransferIssue_Navigation(vm,null,null,connVM);

                txtIssueNo.Text = vm.InvoiceNo;

                if (!string.IsNullOrWhiteSpace(vm.InvoiceNo))
                {
                    SearchInvoice();
                }



            }
            #region Catch

            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "TransferIssueNavigation", exMessage);
            }
            #endregion Catch

        }

        #endregion


      

        private void TransferToBranchLoad()
        {
            try
            {


                DataGridViewRow selectedRow = new DataGridViewRow();
                string SqlText = @" select  BranchID,BranchCode,BranchName,BranchLegalName,Address from BranchProfiles
 where 1=1 and ActiveStatus='Y' and BranchID NOT IN(" + Program.BranchId + ") ";


                string SQLTextRecordCount = @" select count(BranchID)RecordNo from BranchProfiles";

                string[] shortColumnName = { "BranchCode", "BranchName", "BranchLegalName" };
                string tableName = "";
                FormMultipleSearch frm = new FormMultipleSearch();
                //frm.txtSearch.Text = txtSerialNo.Text.Trim();
                selectedRow = FormMultipleSearch.SelectOne(SqlText, tableName, "", shortColumnName, "", SQLTextRecordCount);


                //string[] shortColumnName = {  "VehicleNo", "VehicleType"};
                //string tableName = "Vehicles";
                //selectedRow = FormMultipleSearch.SelectOne("", tableName, "", shortColumnName);
                if (selectedRow != null && selectedRow.Index >= 0)
                {
                    txtTransferto.Text = selectedRow.Cells["BranchName"].Value.ToString();//products.VehicleNo;
                    txtTransferToId.Text = selectedRow.Cells["BranchID"].Value.ToString();//products.VehicleNo;

                }
            }
            catch (Exception ex)
            {
                FileLogger.Log(this.Name, "BranchLoad", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void txtTransferto_DoubleClick(object sender, EventArgs e)
        {
            TransferToBranchLoad();
        }

        private void txtTransferto_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.F9))
            {
                TransferToBranchLoad();
            }
        }

        private void btnTracking_Click(object sender, EventArgs e)
        {
            #region Try


            try
            {
                trackingsCmb.Clear();

                if (dgvIssue.Rows.Count > 0)
                {
                    for (int i = 0; i < dgvIssue.Rows.Count; i++)
                    {
                        var trackingCmb = new TrackingCmbDTO();
                        trackingCmb.ItemNo = dgvIssue.Rows[i].Cells["ItemNo"].Value.ToString();  //FinishItemNo
                        trackingCmb.ProductCode = dgvIssue.Rows[i].Cells["PCode"].Value.ToString(); //FinishItemCode
                        trackingCmb.ProductName = dgvIssue.Rows[i].Cells["ItemName"].Value.ToString(); //FinishItemName
                        //trackingCmb.VatName = dgvIssue.Rows[i].Cells["VATName"].Value.ToString();
                        //////trackingCmb.BOMId = dgvSale.Rows[i].Cells["BOMId"].Value.ToString();
                        trackingCmb.Qty = dgvIssue.Rows[i].Cells["Quantity"].Value.ToString();
                        //trackingCmb.EffectiveDate = dtpInvoiceDate.Value.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss");
                        trackingCmb.EffectiveDate = dtpIssueDate.Value.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss");
                        //if (rbtn61Out.Checked || rbtn62Out.Checked)
                        //{
                        //    trackingCmb.TransferIssueNo = txtOldID.Text;
                        //}
                        //else if (IsUpdate == true)
                        //{
                        //    //trackingCmb.TransferIssueNo = txtSalesInvoiceNo.Text;
                        //    trackingCmb.TransferIssueNo = txtIssueNo.Text;
                        //}
                        trackingCmb.TransferIssueno = txtIssueNo.Text;


                        trackingsCmb.Add(trackingCmb);
                    }

                    trackingsVm.Clear();

                    Program.fromOpen = "Me";

                    trackingsVm = FormTransferIssueTracking.SelectOne(trackingsCmb, "62Out");


                }
            }
            #endregion
            #region Catch

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
                FileLogger.Log(this.Name, "ReceiveImportFromText", exMessage);
            }
            #endregion
        }

    }
}
