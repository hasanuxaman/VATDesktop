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
namespace VATClient
{
    public partial class FormProductTransfer : Form
    {
        #region Constructors
        public FormProductTransfer()
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

        private ReportDocument reportDocument = new ReportDocument();

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
        private string CategoryIdTo { get; set; }
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
        decimal UomConversion = 0;
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

            #endregion Transaction Type
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            #region try
            try
            {

                #region Checkpoint

                #region Null Check

                if (string.IsNullOrEmpty(txtFromItemNo.Text))
                {
                    MessageBox.Show("Please Select From Item No!", this.Text);
                    txtProductName.Focus();
                    return;
                }
                if (txtFromQuantity.Text == "")
                {
                    txtFromQuantity.Text = "0.00";
                }

                if (Convert.ToDecimal(txtFromQuantity.Text) <= 0)
                {
                    MessageBox.Show("Please input Quantity", this.Text);
                    txtFromQuantity.Focus();
                    return;
                }
                if (string.IsNullOrEmpty(txtToItemNo.Text))
                {
                    MessageBox.Show("Please Select To Item No!", this.Text);
                    txtProductNameTo.Focus();
                    return;
                }
                if (string.IsNullOrEmpty(txtFromUOMConversion.Text))
                {
                    MessageBox.Show("Can Not Find UOM Conversion " + txtFromUOM.Text.Trim() + " TO " + txtUOMTo.Text.Trim() + "", this.Text);
                    txtFromUOMConversion.Focus();
                    return;
                }

                #endregion

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
                if (Program.CheckLicence(dtpTransferDate.Value) == true)
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
                    NextID = txtTransferCode.Text.Trim();
                }
                if (txtComments.Text == "")
                {
                    txtComments.Text = "-";
                }

                #endregion

                #region Exist Check

                //TransferIssueVM vm = new TransferIssueVM();
                //if (!string.IsNullOrWhiteSpace(NextID))
                //{
                //    vm = new TransferIssueDAL().SelectAllList(0, new[] { "ti.TransferIssueNo" }, new[] { NextID }).FirstOrDefault();
                //}

                //if (vm != null && !string.IsNullOrWhiteSpace(vm.Id))
                //{
                //    throw new Exception("This Invoice Already Exist! Cannot Add!" + Environment.NewLine + "Invoice No: " + NextID);

                //}

                #endregion

                #region IsWastage Check

                if (string.IsNullOrEmpty(cmbTType.Text) || cmbTType.Text == "Select")
                {
                    MessageBox.Show("Please Select Transfer Type");
                    cmbTType.Focus();
                    return;
                }
                #endregion

                ////if (dgvProductTransfer.RowCount <= 0)
                ////{
                ////    MessageBox.Show("Please insert Details information for transaction", this.Text, MessageBoxButtons.OK,
                ////                     MessageBoxIcon.Information);
                ////    return;
                ////}

                #region Check total value

                decimal TotalValueFrom = 0;
                decimal TotalValueTo = 0;

                if (!string.IsNullOrWhiteSpace(txtFromTotalValue.Text))
                {
                    TotalValueFrom = Convert.ToDecimal(txtFromTotalValue.Text);
                }
                if (!string.IsNullOrWhiteSpace(txtToTotalValue.Text))
                {
                    TotalValueTo = Convert.ToDecimal(txtToTotalValue.Text);
                }

                if (TotalValueFrom > TotalValueTo)
                {
                    throw new ArgumentNullException("From and To Product value not same for transfer");
                }

                #endregion

                #endregion

                #region Transaction Type

                //TransactionTypes();

                #endregion

                #region Value Assign

                #region Master Assign

                ProductTransfersVM ProducttransferVM = new ProductTransfersVM();
                ProducttransferVM.TransferCode = NextID.ToString();
                ProducttransferVM.TransferDate = dtpTransferDate.Value.ToString("yyyy-MMM-dd HH:mm");
                ProducttransferVM.Comments = txtComments.Text.Trim();
                ProducttransferVM.CreatedBy = Program.CurrentUser;
                ProducttransferVM.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                ProducttransferVM.LastModifiedBy = Program.CurrentUser;
                ProducttransferVM.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                ProducttransferVM.BranchId = Program.BranchId;
                ProducttransferVM.Post = "N";
                if (rbtnIsRaw.Checked)
                {
                    ProducttransferVM.IsWastage = "R";
                }
                if (rbtnIsWastage.Checked)
                {
                    ProducttransferVM.IsWastage = "Y";
                }
                if (rbtnIsFinish.Checked)
                {
                    ProducttransferVM.IsWastage = "F";
                }
                ProducttransferVM.ActiveStatus = Convert.ToString(chkActiveStatus.Checked ? "Y" : "N");

                #endregion

                #region Details Assign Old

                //List<ProductTransfersDetailVM> ProducttransferDetailVMs = new List<ProductTransfersDetailVM>();

                // for (int i = 0; i < dgvProductTransfer.RowCount; i++)
                // {
                //     ProductTransfersDetailVM detail = new ProductTransfersDetailVM();
                //     //detail.TransferIssueNo = NextID.ToString();
                //     detail.FromItemNo = dgvProductTransfer.Rows[i].Cells["FromItemNo"].Value.ToString();
                //     detail.FromUOM = dgvProductTransfer.Rows[i].Cells["FromUOM"].Value.ToString();
                //     detail.FromQuantity = Convert.ToDecimal(dgvProductTransfer.Rows[i].Cells["FromQuantity"].Value.ToString());
                //     detail.FromUOMConversion = Convert.ToDecimal(dgvProductTransfer.Rows[i].Cells["FromUOMConversion"].Value.ToString());
                //     detail.ToItemNo = dgvProductTransfer.Rows[i].Cells["ToItemNo"].Value.ToString();
                //     detail.ToUOM = dgvProductTransfer.Rows[i].Cells["ToUOM"].Value.ToString();
                //     detail.ToQuantity = Convert.ToDecimal(dgvProductTransfer.Rows[i].Cells["ToQuantity"].Value.ToString());
                //     if (rbtnIsRaw.Checked)
                //     {
                //         detail.IsWastage = "R";
                //     }
                //     if (rbtnIsWastage.Checked)
                //     {
                //         detail.IsWastage = "Y";
                //     }
                //     if (rbtnIsFinish.Checked)
                //     {
                //         detail.IsWastage = "F";
                //     }
                //     detail.TransferDate = dtpTransferDate.Value.ToString("yyyy-MMM-dd HH:mm");               
                //     detail.BranchId = Program.BranchId;

                //     ProducttransferDetailVMs.Add(detail);
                // }
                #endregion

                #region  Details Assign

                List<ProductTransfersDetailVM> ProducttransferDetailVMs = new List<ProductTransfersDetailVM>();

                ProductTransfersDetailVM detail = new ProductTransfersDetailVM();
                ////detail.TransferIssueNo = NextID.ToString();
                detail.FromItemNo = txtFromItemNo.Text.Trim();
                detail.FromUOM = txtFromUOM.Text.Trim();
                detail.FromQuantity = Convert.ToDecimal(txtFromQuantity.Text.Trim());
                detail.FromUOMConversion = Convert.ToDecimal(txtFromUOMConversion.Text.Trim());
                detail.ToItemNo = txtToItemNo.Text.Trim();
                detail.ToUOM = txtUOMTo.Text.Trim();
                detail.ToQuantity = Convert.ToDecimal(txtToQuantity.Text.Trim());

                detail.FromUnitPrice = Convert.ToDecimal(txtFromPrice.Text.Trim());
                detail.ToUnitPrice = Convert.ToDecimal(txtToPrice.Text.Trim());
                detail.PackSize = Convert.ToDecimal(txtPackSize.Text.Trim());

                detail.IssuePrice = Convert.ToDecimal(txtFromTotalValue.Text.Trim());
                detail.ReceivePrice = Convert.ToDecimal(txtToTotalValue.Text.Trim());

                if (rbtnIsRaw.Checked)
                {
                    detail.IsWastage = "R";
                }
                if (rbtnIsWastage.Checked)
                {
                    detail.IsWastage = "Y";
                }
                if (rbtnIsFinish.Checked)
                {
                    detail.IsWastage = "F";
                }

                detail.TransferDate = dtpTransferDate.Value.ToString("yyyy-MMM-dd HH:mm");
                detail.BranchId = Program.BranchId;

                ProducttransferDetailVMs.Add(detail);

                #endregion

                #endregion

                #region Checkpoint

                ////if (ProducttransferDetailVMs.Count() <= 0)
                ////{
                ////    MessageBox.Show("Please insert Details information for transaction", this.Text, MessageBoxButtons.OK,
                ////                    MessageBoxIcon.Information);
                ////    return;
                ////}

                #endregion

                #region Value Assign

                //SearchBranchId = Program.BranchId;
                sqlResults = new string[4];

                #endregion

                #region Button Stats

                this.btnSave.Enabled = false;
                this.progressBar1.Visible = true;

                #endregion

                #region Save Data

                ProductTransferDAL ProductTransferDal = new ProductTransferDAL();
                //ITransferIssue issueDal = OrdinaryVATDesktop.GetObject<TransferIssueDAL, TransferIssueRepo, ITransferIssue>(OrdinaryVATDesktop.IsWCF);

                sqlResults = ProductTransferDal.Insert(ProducttransferVM, ProducttransferDetailVMs, null, null, connVM, Program.CurrentUserID);

                #endregion

                #region Statement

                if (sqlResults.Length > 0)
                {
                    string result = sqlResults[0];
                    string message = sqlResults[1];
                    string newId = sqlResults[2];
                    string TransferId = sqlResults[4];
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
                        txtTransferCode.Text = sqlResults[2].ToString();
                        IsPost = Convert.ToString(sqlResults[3].ToString()) == "Y" ? true : false;
                        txtTransferId.Text = sqlResults[4].ToString();

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

                if (IsPost == true)
                {
                    MessageBox.Show(MessageVM.ThisTransactionWasPosted, this.Text);
                    return;
                }
                if (string.IsNullOrEmpty(cmbTType.Text) || cmbTType.Text == "Select")
                {
                    MessageBox.Show("Please Select Transfer Type");
                    cmbTType.Focus();
                    return;
                }
                #endregion

                if (Program.CheckLicence(dtpTransferDate.Value) == true)
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
                    NextID = txtTransferCode.Text.Trim();
                }

                if (txtComments.Text == "")
                {
                    txtComments.Text = "-";
                }

                ////if (dgvProductTransfer.RowCount <= 0)
                ////{
                ////    MessageBox.Show("Please insert Details information for transaction", this.Text, MessageBoxButtons.OK,
                ////                     MessageBoxIcon.Information);
                ////    return;
                ////}

                #endregion

                #endregion

                #region Check total value

                decimal TotalValueFrom = 0;
                decimal TotalValueTo = 0;

                if (!string.IsNullOrWhiteSpace(txtFromTotalValue.Text))
                {
                    TotalValueFrom = Convert.ToDecimal(txtFromTotalValue.Text);
                }
                if (!string.IsNullOrWhiteSpace(txtToTotalValue.Text))
                {
                    TotalValueTo = Convert.ToDecimal(txtToTotalValue.Text);
                }

                if (TotalValueFrom > TotalValueTo)
                {
                    throw new ArgumentNullException("From and To Product value not same for transfer");
                }

                #endregion

                #region Value Assign

                #region Master Assign

                ProductTransfersVM ProducttransferVM = new ProductTransfersVM();
                ProducttransferVM.TransferCode = NextID.ToString();
                ProducttransferVM.TransferDate = dtpTransferDate.Value.ToString("yyyy-MMM-dd HH:mm");
                ProducttransferVM.Comments = txtComments.Text.Trim();
                ProducttransferVM.CreatedBy = Program.CurrentUser;
                ProducttransferVM.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                ProducttransferVM.LastModifiedBy = Program.CurrentUser;
                ProducttransferVM.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                ProducttransferVM.BranchId = Program.BranchId;
                ProducttransferVM.Post = "N";
                if (rbtnIsRaw.Checked)
                {
                    ProducttransferVM.IsWastage = "R";
                }
                if (rbtnIsWastage.Checked)
                {
                    ProducttransferVM.IsWastage = "Y";
                }
                if (rbtnIsFinish.Checked)
                {
                    ProducttransferVM.IsWastage = "F";
                }
                ProducttransferVM.ActiveStatus = Convert.ToString(chkActiveStatus.Checked ? "Y" : "N");
                ProducttransferVM.Id = Convert.ToInt32(txtTransferId.Text);

                #endregion

                #region Details Assign old

                //////List<ProductTransfersDetailVM> ProducttransferDetailVMs = new List<ProductTransfersDetailVM>();
                //////for (int i = 0; i < dgvProductTransfer.RowCount; i++)
                //////{
                //////    ProductTransfersDetailVM detail = new ProductTransfersDetailVM();
                //////    //detail.TransferIssueNo = NextID.ToString();
                //////    detail.FromItemNo = dgvProductTransfer.Rows[i].Cells["FromItemNo"].Value.ToString();
                //////    detail.FromUOM = dgvProductTransfer.Rows[i].Cells["FromUOM"].Value.ToString();
                //////    detail.FromQuantity = Convert.ToDecimal(dgvProductTransfer.Rows[i].Cells["FromQuantity"].Value.ToString());
                //////    detail.FromUOMConversion = Convert.ToDecimal(dgvProductTransfer.Rows[i].Cells["FromUOMConversion"].Value.ToString());
                //////    detail.ToItemNo = dgvProductTransfer.Rows[i].Cells["ToItemNo"].Value.ToString();
                //////    detail.ToUOM = dgvProductTransfer.Rows[i].Cells["ToUOM"].Value.ToString();
                //////    detail.ToQuantity = Convert.ToDecimal(dgvProductTransfer.Rows[i].Cells["ToQuantity"].Value.ToString());
                //////    if (rbtnIsRaw.Checked)
                //////    {
                //////        detail.IsWastage = "R";
                //////    }
                //////    if (rbtnIsWastage.Checked)
                //////    {
                //////        detail.IsWastage = "Y";
                //////    }
                //////    if (rbtnIsFinish.Checked)
                //////    {
                //////        detail.IsWastage = "F";
                //////    }
                //////    detail.TransferDate = dtpTransferDate.Value.ToString("yyyy-MMM-dd HH:mm");
                //////    detail.BranchId = Program.BranchId;

                //////    ProducttransferDetailVMs.Add(detail);
                //////}
                #endregion

                #region  Details Assign

                List<ProductTransfersDetailVM> ProducttransferDetailVMs = new List<ProductTransfersDetailVM>();

                ProductTransfersDetailVM detail = new ProductTransfersDetailVM();
                ////detail.TransferIssueNo = NextID.ToString();
                detail.FromItemNo = txtFromItemNo.Text.Trim();
                detail.FromUOM = txtFromUOM.Text.Trim();
                detail.FromQuantity = Convert.ToDecimal(txtFromQuantity.Text.Trim());
                detail.FromUOMConversion = Convert.ToDecimal(txtFromUOMConversion.Text.Trim());
                detail.ToItemNo = txtToItemNo.Text.Trim();
                detail.ToUOM = txtUOMTo.Text.Trim();
                detail.ToQuantity = Convert.ToDecimal(txtToQuantity.Text.Trim());

                detail.FromUnitPrice = Convert.ToDecimal(txtFromPrice.Text.Trim());
                detail.ToUnitPrice = Convert.ToDecimal(txtToPrice.Text.Trim());
                detail.PackSize = Convert.ToDecimal(txtPackSize.Text.Trim());

                detail.IssuePrice = Convert.ToDecimal(txtFromTotalValue.Text.Trim());
                detail.ReceivePrice = Convert.ToDecimal(txtToTotalValue.Text.Trim());

                if (rbtnIsRaw.Checked)
                {
                    detail.IsWastage = "R";
                }
                if (rbtnIsWastage.Checked)
                {
                    detail.IsWastage = "Y";
                }
                if (rbtnIsFinish.Checked)
                {
                    detail.IsWastage = "F";
                }

                detail.TransferDate = dtpTransferDate.Value.ToString("yyyy-MMM-dd HH:mm");
                detail.BranchId = Program.BranchId;

                ProducttransferDetailVMs.Add(detail);

                #endregion

                #endregion

                #region Button Stats

                this.btnUpdate.Enabled = false;
                this.progressBar1.Visible = true;

                #endregion

                #region Update

                sqlResults = new string[4];
                ProductTransferDAL ProductTransferDal = new ProductTransferDAL();

                sqlResults = ProductTransferDal.Update(ProducttransferVM, ProducttransferDetailVMs, connVM, Program.CurrentUserID);

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
                        txtTransferCode.Text = sqlResults[2].ToString();
                        IsPost = Convert.ToString(sqlResults[3].ToString()) == "Y" ? true : false;
                        txtTransferId.Text = sqlResults[4].ToString();

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


                if (Program.CheckLicence(dtpTransferDate.Value) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }

                #endregion


                #region Issue Post

                //TransferIssueDAL issueDal = new TransferIssueDAL();

                ProductTransfersVM ProducttransferVM = new ProductTransfersVM();

                ProducttransferVM.TransferCode = txtTransferCode.Text.Trim();
                ProducttransferVM.Id = Convert.ToInt32(txtTransferId.Text);
                ProducttransferVM.TransferDate = dtpTransferDate.Value.ToString("yyyy-MMM-dd HH:mm");
                ProducttransferVM.LastModifiedBy = Program.CurrentUser;
                ProducttransferVM.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                if (rbtnIsRaw.Checked)
                {
                    ProducttransferVM.IsWastage = "R";
                }
                if (rbtnIsWastage.Checked)
                {
                    ProducttransferVM.IsWastage = "Y";
                }
                if (rbtnIsFinish.Checked)
                {
                    ProducttransferVM.IsWastage = "F";
                }

                ProductTransferDAL ProductTransferDal = new ProductTransferDAL();


                sqlResults = ProductTransferDal.PostTransfer(ProducttransferVM, null, null, connVM, Program.CurrentUserID);

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
                        txtTransferCode.Text = sqlResults[2].ToString();
                        IsPost = Convert.ToString(sqlResults[3].ToString()) == "Y" ? true : false;
                        txtTransferId.Text = sqlResults[4].ToString();

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

                if (rbtnIsRaw.Checked)
                {
                    transactionType = "RawCTC";
                }
                if (rbtnIsWastage.Checked)
                {
                    transactionType = "WastageCTC";
                }
                if (rbtnIsFinish.Checked)
                {
                    transactionType = "FinishCTC";
                }


                #endregion

                #region Select invoiceNo

                string invoiceNo = FormProductTransferSearch.SelectOne(transactionType);

                #endregion

                #region Check Point

                if (string.IsNullOrEmpty(invoiceNo))
                    return;

                #endregion

                #region Value Assign

                txtTransferCode.Text = invoiceNo;

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
                if (dgvProductTransfer.Rows.Count > 0)
                {
                    dgvProductTransfer.Rows[dgvProductTransfer.Rows.Count - 1].Selected = true;
                    dgvProductTransfer.CurrentCell = dgvProductTransfer.Rows[dgvProductTransfer.Rows.Count - 1].Cells[1];
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

                if (string.IsNullOrEmpty(txtFromItemNo.Text))
                {
                    MessageBox.Show("Please Select From Item No!", this.Text);
                    txtProductName.Focus();
                    return;
                }
                if (txtFromQuantity.Text == "")
                {
                    txtFromQuantity.Text = "0.00";
                }


                if (Convert.ToDecimal(txtFromQuantity.Text) <= 0)
                {
                    MessageBox.Show("Please input Quantity", this.Text);
                    txtFromQuantity.Focus();
                    return;
                }
                if (string.IsNullOrEmpty(txtToItemNo.Text))
                {
                    MessageBox.Show("Please Select To Item No!", this.Text);
                    txtProductNameTo.Focus();
                    return;
                }
                if (string.IsNullOrEmpty(txtFromUOMConversion.Text))
                {
                    MessageBox.Show("Can Not Find UOM Conversion " + txtFromUOM.Text.Trim() + " TO " + txtUOMTo.Text.Trim() + "", this.Text);
                    txtFromUOMConversion.Focus();
                    return;
                }

                #endregion

                #region Check Multiple Product Allow

                //var common = new CommonDAL();

                //var multiple = common.settingsDesktop("TransferIssue", "MultipleProduct");


                //if (multiple == "N")
                //{
                //    for (int i = 0; i < dgvProductTransfer.RowCount; i++)
                //    {
                //        if (dgvProductTransfer.Rows[i].Cells["ItemNo"].Value.ToString() == txtProductCode.Text)
                //        {
                //            MessageBox.Show("Same Product already exist.", this.Text);
                //            return;
                //        }
                //    }
                //}

                #endregion

                #endregion
                #region Value Assign to DataGridView - dgvIssue
                DataGridViewRow NewRow = new DataGridViewRow();
                dgvProductTransfer.Rows.Add(NewRow);

                dgvProductTransfer["LineNo", dgvProductTransfer.RowCount - 1].Value = dgvProductTransfer.Rows.Count;
                dgvProductTransfer["FromItemNo", dgvProductTransfer.RowCount - 1].Value = txtFromItemNo.Text.Trim();
                dgvProductTransfer["FromItemName", dgvProductTransfer.RowCount - 1].Value = txtProductName.Text.Trim();
                dgvProductTransfer["FromUOM", dgvProductTransfer.RowCount - 1].Value = txtFromUOM.Text.Trim();
                dgvProductTransfer["FromQuantity", dgvProductTransfer.RowCount - 1].Value = txtFromQuantity.Text.Trim();
                dgvProductTransfer["FromUOMConversion", dgvProductTransfer.RowCount - 1].Value = txtFromUOMConversion.Text.Trim();
                dgvProductTransfer["ToItemNo", dgvProductTransfer.RowCount - 1].Value = txtToItemNo.Text.Trim();
                dgvProductTransfer["ToItemName", dgvProductTransfer.RowCount - 1].Value = txtProductNameTo.Text.Trim();
                dgvProductTransfer["ToUOM", dgvProductTransfer.RowCount - 1].Value = txtUOMTo.Text.Trim();
                dgvProductTransfer["ToQuantity", dgvProductTransfer.RowCount - 1].Value = txtToQuantity.Text.Trim();

                #endregion

                AllClear();
                selectLastRow();
                txtProductName.Focus();
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
            txtProductName.Text = "";
            txtFromItemNo.Text = "";
            txtFromQuantity.Text = "0.00";
            txtFromUOM.Text = "";
            txtUOMTo.Text = "";
            txtProductNameTo.Text = "";
            txtToItemNo.Text = "";
            txtToQuantity.Text = "0.00";

            txtFromUOMConversion.Text = "";

        }


        private void btnRemove_Click(object sender, EventArgs e)
        {
            #region try
            try
            {
                if (dgvProductTransfer.RowCount > 0)
                {
                    if (MessageBox.Show(MessageVM.msgWantToRemoveRow + "\nItem Code: " + dgvProductTransfer.CurrentRow.Cells["FromItemNo"].Value, this.Text, MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        dgvProductTransfer.Rows.RemoveAt(dgvProductTransfer.CurrentRow.Index);

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


        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnChange_Click(object sender, EventArgs e)
        {
            if (dgvProductTransfer.RowCount > 0)
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
                    string.IsNullOrEmpty(txtFromUOM.Text)
                    || string.IsNullOrEmpty(txtFromItemNo.Text)
                    || string.IsNullOrEmpty(txtFromQuantity.Text))
                {
                    MessageBox.Show("Please Select Desired Item Row Appropriately Using Double Click!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                if (Convert.ToDecimal(txtFromQuantity.Text) <= 0)
                {
                    MessageBox.Show("Zero Quantity Is not Allowed  for Change Button!\nPlease Provide Quantity.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (string.IsNullOrEmpty(txtFromUOMConversion.Text))
                {
                    MessageBox.Show("Can Not Find UOM Conversion " + txtFromUOM.Text.Trim() + " TO " + txtUOMTo.Text.Trim() + "", this.Text);
                    txtFromUOMConversion.Focus();
                    return;
                }
                ChangeData = true;


                #endregion


                #endregion

                #region Value Assign to DataGridView dgvIssue

                dgvProductTransfer["FromItemNo", dgvProductTransfer.CurrentRow.Index].Value = txtFromItemNo.Text;
                dgvProductTransfer["FromItemName", dgvProductTransfer.CurrentRow.Index].Value = txtProductName.Text;

                dgvProductTransfer["FromUOM", dgvProductTransfer.CurrentRow.Index].Value = txtFromUOM.Text.Trim();
                dgvProductTransfer["FromQuantity", dgvProductTransfer.CurrentRow.Index].Value = Program.ParseDecimalObject(Convert.ToDecimal(txtFromQuantity.Text.Trim()).ToString());
                dgvProductTransfer["FromUOMConversion", dgvProductTransfer.CurrentRow.Index].Value = Program.ParseDecimalObject(Convert.ToDecimal(txtFromUOMConversion.Text.Trim()).ToString());

                dgvProductTransfer["ToItemNo", dgvProductTransfer.CurrentRow.Index].Value = txtToItemNo.Text;
                dgvProductTransfer["ToItemName", dgvProductTransfer.CurrentRow.Index].Value = txtProductNameTo.Text;

                dgvProductTransfer["ToUOM", dgvProductTransfer.CurrentRow.Index].Value = txtUOMTo.Text.Trim();
                dgvProductTransfer["ToQuantity", dgvProductTransfer.CurrentRow.Index].Value = Program.ParseDecimalObject(Convert.ToDecimal(txtToQuantity.Text.Trim()).ToString());//"0,0.000");



                dgvProductTransfer.CurrentRow.DefaultCellStyle.ForeColor = Color.Green;// Blue;
                //dgvIssue.CurrentRow.DefaultCellStyle.ForeColor = Color.Red;
                dgvProductTransfer.CurrentRow.DefaultCellStyle.Font = new Font(this.Font, FontStyle.Regular);

                #endregion


                #region Reset Fields


                txtProductName.Text = "";
                txtFromItemNo.Text = "";
                txtFromQuantity.Text = "0.00";
                txtFromUOM.Text = "";
                txtUOMTo.Text = "";
                txtProductNameTo.Text = "";
                txtToItemNo.Text = "";
                txtToQuantity.Text = "0.00";

                txtFromUOMConversion.Text = "";

                #endregion

                #endregion
                txtProductName.Focus();

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

            SearchBranchId = 0;
            txtFromUOM.Text = "";
            txtUOMTo.Text = "";
            txtComments.Text = "";
            txtProductNameTo.Text = "";
            txtFromItemNo.Text = "";
            txtToItemNo.Text = "";
            txtFromQuantity.Text = "0.00";
            txtToQuantity.Text = "0.00";

            txtProductName.Text = "";
            txtTransferId.Text = "0";
            if (rbtnIsRaw.Checked)
            {
                cmbTType.Text = "Raw";
            }
            if (rbtnIsWastage.Checked)
            {
                cmbTType.Text = "Wastage";
            }
            if (rbtnIsFinish.Checked)
            {
                cmbTType.Text = "Finish";
            }

            string AutoSessionDate = commonDal.settingsDesktop("SessionDate", "AutoSessionDate");
            if (AutoSessionDate.ToLower() != "y")
            {
                dtpTransferDate.Value = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss"));

            }
            else
            {
                dtpTransferDate.Value = Convert.ToDateTime(Program.SessionDate.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss"));

            }

            dgvProductTransfer.Rows.Clear();
        }

        private void txtComments_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }



        private void txtQuantity_Leave(object sender, EventArgs e)
        {
            txtFromQuantity.Text = Program.ParseDecimalObject(txtFromQuantity.Text.Trim()).ToString();
            if (!string.IsNullOrEmpty(txtToItemNo.Text))
            {
                FindUOMConversuon();
            }

            GetFromProductTotalValue();
            GetToProductPackSizeCalculation();

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
                txtTransferCode.Text = "~~~ New ~~~";

                #endregion

                #region Form Maker

                FormMaker();

                #endregion

                #region Form Load


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


                #endregion



                #region Button Stats

                progressBar1.Visible = true;

                #endregion

                #region Branch



                #endregion Branch
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
                if (rbtnIsRaw.Checked)
                {
                    cmbTType.Text = "Raw";
                }
                if (rbtnIsWastage.Checked)
                {
                    cmbTType.Text = "Wastage";
                }
                if (rbtnIsFinish.Checked)
                {
                    cmbTType.Text = "Finish";
                }
                #region Transaction Type

                #endregion Transaction Type


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





        private void dgvIssue_DoubleClick(object sender, EventArgs e)
        {
            #region try
            try
            {
                if (dgvProductTransfer.Rows.Count <= 0)
                {
                    MessageBox.Show("There is no data to select.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }


                #region Value Assign to Form Elements

                txtFromItemNo.Text = dgvProductTransfer.CurrentRow.Cells["FromItemNo"].Value.ToString();
                txtProductName.Text = dgvProductTransfer.CurrentRow.Cells["FromItemName"].Value.ToString();
                txtFromUOM.Text = dgvProductTransfer.CurrentRow.Cells["FromUOM"].Value.ToString();
                txtFromQuantity.Text = Convert.ToDecimal(dgvProductTransfer.CurrentRow.Cells["FromQuantity"].Value).ToString();
                txtFromUOMConversion.Text = Convert.ToDecimal(dgvProductTransfer.CurrentRow.Cells["FromUOMConversion"].Value).ToString();
                txtToItemNo.Text = dgvProductTransfer.CurrentRow.Cells["ToItemNo"].Value.ToString();
                txtProductNameTo.Text = dgvProductTransfer.CurrentRow.Cells["ToItemName"].Value.ToString();
                txtUOMTo.Text = dgvProductTransfer.CurrentRow.Cells["ToUOM"].Value.ToString();
                txtToQuantity.Text = Convert.ToDecimal(dgvProductTransfer.CurrentRow.Cells["ToQuantity"].Value).ToString();





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

                //ITransferIssue issueDal = OrdinaryVATDesktop.GetObject<TransferIssueDAL, TransferIssueRepo, ITransferIssue>(OrdinaryVATDesktop.IsWCF);
                ProductTransferDAL ProductTransferDal = new ProductTransferDAL();

                ProductTransfersVM vm = new ProductTransfersVM();
                vm.TransferCode = txtTransferCode.Text;
                vm.TransferDateFrom = "";
                vm.TransferDateTo = "";
                vm.Post = "";

                DataTable IssueResult = ProductTransferDal.SearchTransfer(vm, null, null, connVM);  // Change 04

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

        private void LoadForm(DataTable TransferResult)
        {
            string invoiceNo = "";
            ProductTransferDAL ProductTransferDal = new ProductTransferDAL();

            if (TransferResult.Rows.Count <= 0)
            {
                return;
            }

            DataRow dr = TransferResult.Rows[0];

            #region Value Assign to Form Elements


            txtTransferCode.Text = dr["TransferCode"].ToString();
            txtTransferId.Text = dr["Id"].ToString();

            dtpTransferDate.Value = Convert.ToDateTime(dr["TransferDate"].ToString());
            string type = dr["IsWastage"].ToString();
            if (type == "Y")
            {
                cmbTType.Text = "Wastage";
            }
            if (type == "R")
            {
                cmbTType.Text = "Raw";
            }
            if (type == "F")
            {
                cmbTType.Text = "Finish";
            }
            txtComments.Text = dr["Comments"].ToString();
            //txtBranchDBName.Text = dr["TransferTo"].ToString();
            //cmbTransferTo.Text = dr["BranchName"].ToString();
            IsPost = Convert.ToString(dr["Post"].ToString()) == "Y" ? true : false;
            IssueDetailData = txtTransferId.Text == "" ? "" : txtTransferId.Text.Trim();

            #endregion

            #region Statement

            IssueDetailResult = new DataTable();
            if (!string.IsNullOrEmpty(IssueDetailData))
            {
                IssueDetailResult = ProductTransferDal.SearchTransferDetail(IssueDetailData, null, null, connVM);
            }

            #region DataGridView

            ////dgvProductTransfer.Rows.Clear();
            ////int j = 0;
            ////int l = 0;
            ////foreach (DataRow item in IssueDetailResult.Rows)
            ////{
            ////    DataGridViewRow NewRow = new DataGridViewRow();
            ////    dgvProductTransfer.Rows.Add(NewRow);
            ////    dgvProductTransfer.Rows[j].Cells["LineNo"].Value = j + 1;
            ////    dgvProductTransfer.Rows[j].Cells["FromItemNo"].Value = item["FromItemNo"].ToString();
            ////    dgvProductTransfer.Rows[j].Cells["FromItemName"].Value = item["FromItemName"].ToString();
            ////    dgvProductTransfer.Rows[j].Cells["FromUOM"].Value = item["FromUOM"].ToString();
            ////    dgvProductTransfer.Rows[j].Cells["FromQuantity"].Value = Program.ParseDecimalObject(item["FromQuantity"].ToString());
            ////    dgvProductTransfer.Rows[j].Cells["FromUOMConversion"].Value = Program.ParseDecimalObject(item["FromUOMConversion"].ToString());
            ////    dgvProductTransfer.Rows[j].Cells["ToItemNo"].Value = item["ToItemNo"].ToString();
            ////    dgvProductTransfer.Rows[j].Cells["ToItemName"].Value = item["ToItemName"].ToString();
            ////    dgvProductTransfer.Rows[j].Cells["ToUOM"].Value = item["ToUOM"].ToString();
            ////    dgvProductTransfer.Rows[j].Cells["ToQuantity"].Value = Program.ParseDecimalObject(item["ToQuantity"].ToString());


            ////    j = j + 1;
            ////} //End For

            #endregion DataGridView

            #region Get Data

            #region  Details Assign

            if (IssueDetailResult != null && IssueDetailResult.Rows.Count > 0)
            {

                foreach (DataRow item in IssueDetailResult.Rows)
                {
                    txtFromItemNo.Text = item["FromItemNo"].ToString();
                    txtProductName.Text = item["FromItemName"].ToString();
                    txtFromUOM.Text = item["FromUOM"].ToString();
                    txtFromQuantity.Text = item["FromQuantity"].ToString();
                    txtFromUOMConversion.Text = item["FromUOMConversion"].ToString();
                    txtToItemNo.Text = item["ToItemNo"].ToString();
                    txtProductNameTo.Text = item["ToItemName"].ToString();

                    txtUOMTo.Text = item["ToUOM"].ToString();
                    txtToQuantity.Text = item["ToQuantity"].ToString();
                    txtFromPrice.Text = item["FromUnitPrice"].ToString();
                    txtToPrice.Text = item["ToUnitPrice"].ToString();
                    txtPackSize.Text = item["PackSize"].ToString();
                    txtFromTotalValue.Text = item["IssuePrice"].ToString();
                    txtToTotalValue.Text = item["ReceivePrice"].ToString();

                } //End For

            }

            #endregion

            #endregion


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

            txtTransferCode.Text = IssueResult.Rows[0]["TransferIssueNo"].ToString();
            invoiceNo = IssueResult.Rows[0]["Id"].ToString();
            dtpTransferDate.Value = Convert.ToDateTime(IssueResult.Rows[0]["IssueDateTime"].ToString());

            txtComments.Text = IssueResult.Rows[0]["Comments"].ToString();
            //txtBranchDBName.Text = IssueResult.Rows[0]["TransferTo"].ToString();
            //txtTransferToId.Text = IssueResult.Rows[0]["TransferTo"].ToString();
            //cmbTransferTo.Text = IssueResult.Rows[0]["BranchName"].ToString();
            IsPost = Convert.ToString(IssueResult.Rows[0]["Post"].ToString()) == "Y" ? true : false;

            ////string IssueDetailData;
            IssueDetailData = txtTransferCode.Text == "" ? "" : txtTransferCode.Text.Trim();

            #region Statement

            IssueDetailResult = new DataTable();
            if (!string.IsNullOrEmpty(invoiceNo))
            {
                IssueDetailResult = issueDal.SearchTransferDetail_TempTable(invoiceNo, Program.CurrentUserID, null, null, connVM);
            }

            #region DataGridView

            dgvProductTransfer.Rows.Clear();
            int j = 0;
            foreach (DataRow item in IssueDetailResult.Rows)
            {
                DataGridViewRow NewRow = new DataGridViewRow();
                dgvProductTransfer.Rows.Add(NewRow);
                dgvProductTransfer.Rows[j].Cells["BOMId"].Value = item["BOMId"].ToString();

                dgvProductTransfer.Rows[j].Cells["LineNo"].Value = item["IssueLineNo"].ToString();
                dgvProductTransfer.Rows[j].Cells["ItemNo"].Value = item["ItemNo"].ToString();
                dgvProductTransfer.Rows[j].Cells["Quantity"].Value = item["Quantity"].ToString();
                dgvProductTransfer.Rows[j].Cells["UnitPrice"].Value = item["CostPrice"].ToString();
                dgvProductTransfer.Rows[j].Cells["UOM"].Value = item["UOM"].ToString();
                dgvProductTransfer.Rows[j].Cells["SubTotal"].Value = item["SubTotal"].ToString();
                dgvProductTransfer.Rows[j].Cells["Comments"].Value = item["Comments"].ToString();
                dgvProductTransfer.Rows[j].Cells["ItemName"].Value = item["ProductName"].ToString();
                dgvProductTransfer.Rows[j].Cells["PCode"].Value = item["ProductCode"].ToString();
                dgvProductTransfer.Rows[j].Cells["Status"].Value = "Old";
                dgvProductTransfer.Rows[j].Cells["Previous"].Value = item["Quantity"].ToString();
                dgvProductTransfer.Rows[j].Cells["Stock"].Value = item["Stock"].ToString();
                dgvProductTransfer.Rows[j].Cells["Change"].Value = 0;
                dgvProductTransfer.Rows[j].Cells["UOMc"].Value = item["UOMc"].ToString();
                dgvProductTransfer.Rows[j].Cells["UOMn"].Value = item["UOMn"].ToString();
                dgvProductTransfer.Rows[j].Cells["UOMQty"].Value = item["UOMQty"].ToString();
                dgvProductTransfer.Rows[j].Cells["UOMPrice"].Value = item["UOMPrice"].ToString();

                dgvProductTransfer.Rows[j].Cells["VATRate"].Value = item["VATRate"].ToString();
                dgvProductTransfer.Rows[j].Cells["VATAmount"].Value = item["VATAmount"].ToString();
                dgvProductTransfer.Rows[j].Cells["SDRate"].Value = item["SDRate"].ToString();
                dgvProductTransfer.Rows[j].Cells["SDAmount"].Value = item["SDAmount"].ToString();
                dgvProductTransfer.Rows[j].Cells["Weight"].Value = item["Weight"].ToString();

                j = j + 1;
            } //End For

            #endregion DataGridView



            #endregion

        }

        private void btnSearchProductName_Click(object sender, EventArgs e)
        {
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
                TransferNavigation("Current");
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
                txtProductNameTo.Focus();
            }
        }

        private void txtCommentsDetail_KeyDown(object sender, KeyEventArgs e)
        {
            btnAdd.Focus();
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
                        txtTransferCode.Text = "~~~ New ~~~";
                    }
                }
                else if (ChangeData == false)
                {
                    //ProductSearchDsFormLoad();
                    //ProductSearchDsLoad();
                    ClearAllFields();
                    btnSave.Text = "&Add";
                    txtTransferCode.Text = "~~~ New ~~~";
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


        #endregion

        #region Methods 05


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
                            //cmbProductName.Text = products.ProductName;
                            txtProductCode.Text = products.ItemNo;
                            txtFromUOM.Text = products.UOM;

                        }
                    }
                }
                string strProductCode = txtProductCode.Text;

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

        private void ProductLoad()
        {
            string saleFromProduction = commonDal.settingsDesktop("Sale", "SaleFromProduction", null, connVM);

            try
            {
                DataGridViewRow selectedRow = new DataGridViewRow();
                if (saleFromProduction == "Y" && !string.IsNullOrEmpty(""))
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
                    selectedRow = FormMultipleSearch.SelectOne(SqlText, tableName, "", shortColumnName, "", SQLTextRecordCount);
                    if (selectedRow != null && selectedRow.Index >= 0)
                    {
                        vItemNo = selectedRow.Cells["ItemNo"].Value.ToString();//ProductInfo[1];
                        txtProductCode.Text = vItemNo;

                        //cmbProductName.Text = selectedRow.Cells["ProductName"].Value.ToString() + "~" + selectedRow.Cells["ProductCode"].Value.ToString();//ProductInfo[1];
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

                        //cmbProductName.Text = selectedRow.Cells["ProductName"].Value.ToString() + "~" + selectedRow.Cells["ProductCode"].Value.ToString();//ProductInfo[1];
                        //////cmbProduct.SelectedText = selectedRow.Cells["ProductName"].Value.ToString();//ProductInfo[1];
                    }
                }
                else
                {


                    string SqlText = @"   select p.ItemNo,p.ProductCode,p.ProductName,p.UOM,pc.CategoryName,pc.IsRaw ProductType  from Products p
 left outer join ProductCategories pc on p.CategoryID=pc.CategoryID
where 1=1 and  p.ActiveStatus='Y'  ";

                    if (rbtnIsRaw.Checked || rbtnIsWastage.Checked)
                    {
                        SqlText += @"  and pc.IsRaw in ('Raw','Pack')";
                    }
                    if (rbtnIsFinish.Checked)
                    {
                        SqlText += @"  and pc.IsRaw in ('Finish')";
                    }

                    string SQLTextRecordCount = @" select count(ProductCode)RecordNo from Products";

                    string[] shortColumnName = { "p.ItemNo", "p.ProductCode", "p.ProductName", "p.UOM", "pc.CategoryName", "pc.IsRaw" };
                    string tableName = "";
                    FormMultipleSearch frm = new FormMultipleSearch();
                    //frm.txtSearch.Text = txtSerialNo.Text.Trim();
                    selectedRow = FormMultipleSearch.SelectOne(SqlText, tableName, "", shortColumnName, null, SQLTextRecordCount);
                    if (selectedRow != null && selectedRow.Index >= 0)
                    {
                        txtFromItemNo.Text = selectedRow.Cells["ItemNo"].Value.ToString();//ProductInfo[1];
                        txtProductName.Text = selectedRow.Cells["ProductName"].Value.ToString();
                        txtFromUOM.Text = selectedRow.Cells["UOM"].Value.ToString();
                        //cmbProductName.SelectedText = selectedRow.Cells["ProductName"].Value.ToString() + "~" + selectedRow.Cells["ProductCode"].Value.ToString();//ProductInfo[1];

                    }

                }

                GetFromProductPriceBOM();

            }
            catch (Exception ex)
            {
                FileLogger.Log(this.Name, "ProductLoad", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void ProductLoadTo()
        {
            string saleFromProduction = commonDal.settingsDesktop("Sale", "SaleFromProduction", null, connVM);

            try
            {
                DataGridViewRow selectedRow = new DataGridViewRow();
                if (saleFromProduction == "Y" && !string.IsNullOrEmpty(""))
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
                    selectedRow = FormMultipleSearch.SelectOne(SqlText, tableName, "", shortColumnName, "", SQLTextRecordCount);
                    if (selectedRow != null && selectedRow.Index >= 0)
                    {
                        vItemNo = selectedRow.Cells["ItemNo"].Value.ToString();//ProductInfo[1];
                        txtProductCode.Text = vItemNo;

                        //cmbProductName.Text = selectedRow.Cells["ProductName"].Value.ToString() + "~" + selectedRow.Cells["ProductCode"].Value.ToString();//ProductInfo[1];
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

                        //cmbProductName.Text = selectedRow.Cells["ProductName"].Value.ToString() + "~" + selectedRow.Cells["ProductCode"].Value.ToString();//ProductInfo[1];
                        //////cmbProduct.SelectedText = selectedRow.Cells["ProductName"].Value.ToString();//ProductInfo[1];
                    }
                }
                else
                {


                    string SqlText = @"   select p.ItemNo,p.ProductCode,p.ProductName,p.UOM,pc.CategoryName,pc.IsRaw ProductType  from Products p
 left outer join ProductCategories pc on p.CategoryID=pc.CategoryID
where 1=1 and  p.ActiveStatus='Y'  ";


                    if (rbtnIsRaw.Checked || rbtnIsWastage.Checked)
                    {
                        SqlText += @"  and pc.IsRaw in ('Raw','Pack')";
                    }
                    if (rbtnIsFinish.Checked)
                    {
                        SqlText += @"  and pc.IsRaw in ('Finish')";
                    }


                    string SQLTextRecordCount = @" select count(ProductCode)RecordNo from Products";

                    string[] shortColumnName = { "p.ItemNo", "p.ProductCode", "p.ProductName", "p.UOM", "pc.CategoryName", "pc.IsRaw" };
                    string tableName = "";
                    FormMultipleSearch frm = new FormMultipleSearch();
                    //frm.txtSearch.Text = txtSerialNo.Text.Trim();
                    selectedRow = FormMultipleSearch.SelectOne(SqlText, tableName, "", shortColumnName, null, SQLTextRecordCount);
                    if (selectedRow != null && selectedRow.Index >= 0)
                    {
                        txtToItemNo.Text = selectedRow.Cells["ItemNo"].Value.ToString();//ProductInfo[1];
                        txtProductNameTo.Text = selectedRow.Cells["ProductName"].Value.ToString();
                        txtUOMTo.Text = selectedRow.Cells["UOM"].Value.ToString();
                        //cmbProductName.SelectedText = selectedRow.Cells["ProductName"].Value.ToString() + "~" + selectedRow.Cells["ProductCode"].Value.ToString();//ProductInfo[1];

                    }

                }
            }
            catch (Exception ex)
            {
                FileLogger.Log(this.Name, "ProductLoadTo", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        #endregion

        #region Methods 06


        private void cmbTransferTo_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }



        #endregion

        #region Purchase Navigation

        private void btnFirst_Click(object sender, EventArgs e)
        {
            TransferNavigation("First");

        }

        private void btnPrevious_Click_1(object sender, EventArgs e)
        {
            TransferNavigation("Previous");

        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            TransferNavigation("Next");
        }

        private void btnLast_Click(object sender, EventArgs e)
        {
            TransferNavigation("Last");
        }

        private void TransferNavigation(string ButtonName)
        {
            try
            {

                ProductTransferDAL _ProductTransferDAL = new ProductTransferDAL();
                NavigationVM vm = new NavigationVM();
                vm.ButtonName = ButtonName;

                vm.BranchId = Program.BranchId;
                vm.Id = Convert.ToInt32(txtTransferId.Text);
                vm.InvoiceNo = txtTransferCode.Text;
                if (rbtnIsRaw.Checked)
                {
                    vm.TransactionType = "RawCTC";
                }
                if (rbtnIsWastage.Checked)
                {
                    vm.TransactionType = "WastageCTC";
                }
                if (rbtnIsFinish.Checked)
                {
                    vm.TransactionType = "FinishCTC";
                }


                if (vm.BranchId == 0)
                {
                    vm.BranchId = Program.BranchId;
                }

                vm = _ProductTransferDAL.Transfer_Navigation(vm, null, null, connVM);

                txtTransferCode.Text = vm.InvoiceNo;

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
                FileLogger.Log(this.Name, "TransferNavigation", exMessage);
            }
            #endregion Catch

        }

        #endregion

        private void txtProductName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.F9))
            {

                ProductLoad();
            }

            if (e.KeyCode.Equals(Keys.Enter))
            {

                txtFromQuantity.Focus();
            }
        }

        private void txtProductNameTo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.F9))
            {
                if (txtFromItemNo.Text == "")
                {
                    MessageBox.Show("Please select From Transfer Item Fist!", this.Text);
                    txtProductName.Focus();
                    return;
                }
                if (Convert.ToDecimal(txtFromQuantity.Text) <= 0)
                {
                    MessageBox.Show("Please input From Transfer Quantity", this.Text);
                    txtFromQuantity.Focus();
                    return;
                }
                ProductLoadTo();
                FindUOMConversuon();
                GetToProductPriceBOM();
            }
        }

        private void dtpIssueDate_Leave(object sender, EventArgs e)
        {
            dtpTransferDate.Value = Program.ParseDate(dtpTransferDate);
        }

        private void FindUOMConversuon()
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(txtFromUOM.Text.Trim()))
                {


                    IUOM uomdal = OrdinaryVATDesktop.GetObject<UOMDAL, UOMRepo, IUOM>(OrdinaryVATDesktop.IsWCF);

                    string[] cvals = new string[] { txtFromUOM.Text.Trim(), txtUOMTo.Text.Trim(), "Y" };
                    string[] cfields = new string[] { "UOMFrom", "UOMTo", "ActiveStatus like" };
                    uomResult = uomdal.SelectAll(0, cfields, cvals, null, null, false, connVM);

                    if (uomResult != null && uomResult.Rows.Count > 0)
                    {
                        txtFromUOMConversion.Text = Program.ParseDecimalObject(uomResult.Rows[0]["Convertion"].ToString());
                        UomConversion = Convert.ToDecimal(txtFromUOMConversion.Text.Trim());
                    }
                    else
                    {

                        MessageBox.Show("Can Not Find UOM Conversion " + txtFromUOM.Text.Trim() + " TO " + txtUOMTo.Text.Trim() + "", this.Text);
                        txtProductNameTo.Focus();
                        return;
                    }
                    txtToQuantity.Text = Program.ParseDecimalObject((UomConversion * Convert.ToDecimal(txtFromQuantity.Text)));

                }
            }
            catch (Exception ex)
            {

                FileLogger.Log(this.Name, "FindUOMConversuon", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void txtProductName_DoubleClick(object sender, EventArgs e)
        {
            ProductLoad();
        }

        private void txtProductNameTo_DoubleClick(object sender, EventArgs e)
        {
            if (txtFromItemNo.Text == "")
            {
                MessageBox.Show("Please select From Transfer Item Fist!", this.Text);
                txtProductName.Focus();
                return;
            }

            if (Convert.ToDecimal(txtFromQuantity.Text) <= 0)
            {
                MessageBox.Show("Please input From Transfer Quantity", this.Text);
                txtFromQuantity.Focus();
                return;
            }
            ProductLoadTo();
            FindUOMConversuon();
            GetToProductPriceBOM();

        }

        private void GetFromProductPriceBOM()
        {
            ProductDAL productDal = new ProductDAL();

            try
            {
                string TransferDate = dtpTransferDate.Value.ToString("yyyy-MMM-dd HH:mm");

                if (rbtnIsFinish.Checked)
                {

                    //BugsBD
                    string FromItemNo = OrdinaryVATDesktop.SanitizeInput(txtFromItemNo.Text.ToString());
                    decimal FromItemNoValue = Convert.ToDecimal(productDal.GetBOMReferenceNo(FromItemNo, "VAT 4.3", TransferDate, null, null, "0", connVM).Rows[0]["NBRPrice"]);

                    //decimal FromItemNoValue = Convert.ToDecimal(productDal.GetBOMReferenceNo(txtFromItemNo.Text.ToString(), "VAT 4.3", TransferDate, null, null, "0", connVM).Rows[0]["NBRPrice"]);

                    if (FromItemNoValue == null || FromItemNoValue < 0)
                    {
                        throw new ArgumentNullException("Transfer Rate", "From Product Rate not Found to transfer");
                    }
                    txtFromPrice.Text = FromItemNoValue.ToString();
                }
                else
                {
                    DataTable priceData = productDal.AVGStockNewMethod(txtFromItemNo.Text.ToString(), TransferDate, null, null, connVM);
                    if (priceData == null || priceData.Rows.Count < 0)
                    {
                        throw new ArgumentNullException("Transfer Rate", "From Product Ave Rate not Found to transfer");
                    }

                    decimal avgRate = Convert.ToDecimal(priceData.Rows[0]["AvgRate"].ToString());
                    txtFromPrice.Text = avgRate.ToString();

                }

            }
            catch (Exception ex)
            {
                FileLogger.Log(this.Name, "ProductPriceLoadFrom", ex.Message + Environment.NewLine + ex.StackTrace);
                throw ex;
            }
        }

        private void GetToProductPriceBOM()
        {
            ProductDAL productDal = new ProductDAL();

            try
            {
                string TransferDate = dtpTransferDate.Value.ToString("yyyy-MMM-dd HH:mm");

                if (rbtnIsFinish.Checked)
                {
                    //BugsBD
                    string ToItemNo = OrdinaryVATDesktop.SanitizeInput(txtToItemNo.Text.ToString());

                    decimal FromItemNoValue = Convert.ToDecimal(productDal.GetBOMReferenceNo(ToItemNo, "VAT 4.3", TransferDate, null, null, "0", connVM).Rows[0]["NBRPrice"]);

                    //decimal FromItemNoValue = Convert.ToDecimal(productDal.GetBOMReferenceNo(txtToItemNo.Text.ToString(), "VAT 4.3", TransferDate, null, null, "0", connVM).Rows[0]["NBRPrice"]);

                    if (FromItemNoValue == null || FromItemNoValue < 0)
                    {
                        throw new ArgumentNullException("Transfer Rate", "From Product Rate not Found to transfer");
                    }
                    txtToPrice.Text = FromItemNoValue.ToString();
                }
                else
                {
                    DataTable priceData = productDal.AVGStockNewMethod(txtFromItemNo.Text.ToString(), TransferDate, null, null, connVM);
                    if (priceData == null || priceData.Rows.Count < 0)
                    {
                        throw new ArgumentNullException("Transfer Rate", "From Product Ave Rate not Found to transfer");
                    }

                    decimal avgRate = Convert.ToDecimal(priceData.Rows[0]["AvgRate"].ToString());
                    txtToPrice.Text = avgRate.ToString();

                }

            }
            catch (Exception ex)
            {
                FileLogger.Log(this.Name, "ProductPriceLoadTo", ex.Message + Environment.NewLine + ex.StackTrace);
                throw ex;
            }
        }

        private void txtPackSize_Leave(object sender, EventArgs e)
        {

            GetToProductPackSizeCalculation();
        }

        private void GetToProductPackSizeCalculation()
        {
            try
            {

                decimal packSize = 0;
                decimal UOMc = 0;
                decimal fromQty = 0;

                if (!string.IsNullOrWhiteSpace(txtFromQuantity.Text))
                {
                    fromQty = Convert.ToDecimal(txtFromQuantity.Text);
                }
                if (!string.IsNullOrWhiteSpace(txtPackSize.Text))
                {
                    packSize = Convert.ToDecimal(txtPackSize.Text);
                }
                if (!string.IsNullOrWhiteSpace(txtFromUOMConversion.Text))
                {
                    UOMc = Convert.ToDecimal(txtFromUOMConversion.Text);
                }

                if (packSize != 0)
                {
                    decimal toQty = (UOMc * fromQty) / packSize;

                    txtToQuantity.Text = toQty.ToString();

                }

                GetToProductTotalValue();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "txtPackSize_Leave", ex.Message);
            }
        }


        private void GetToProductTotalValue()
        {
            try
            {
                decimal Price = 0;
                decimal Qty = 0;

                if (!string.IsNullOrWhiteSpace(txtToQuantity.Text))
                {
                    Qty = Convert.ToDecimal(txtToQuantity.Text);
                }
                if (!string.IsNullOrWhiteSpace(txtToPrice.Text))
                {
                    Price = Convert.ToDecimal(txtToPrice.Text);
                }

                decimal totalValue = (Qty * Price);

                txtToTotalValue.Text = totalValue.ToString();

            }
            catch (Exception ex)
            {
                FileLogger.Log(this.Name, "GetToProductTotalValue", ex.Message + Environment.NewLine + ex.StackTrace);
                throw ex;
            }
        }

        private void GetFromProductTotalValue()
        {
            try
            {
                decimal Price = 0;
                decimal Qty = 0;

                if (!string.IsNullOrWhiteSpace(txtFromQuantity.Text))
                {
                    Qty = Convert.ToDecimal(txtFromQuantity.Text);
                }
                if (!string.IsNullOrWhiteSpace(txtFromPrice.Text))
                {
                    Price = Convert.ToDecimal(txtFromPrice.Text);
                }

                decimal totalValue = (Qty * Price);

                txtFromTotalValue.Text = totalValue.ToString();

            }
            catch (Exception ex)
            {
                FileLogger.Log(this.Name, "GetFromProductTotalValue", ex.Message + Environment.NewLine + ex.StackTrace);
                throw ex;
            }
        }


    }
}
