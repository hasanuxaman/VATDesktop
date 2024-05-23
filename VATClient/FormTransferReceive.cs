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
using VATDesktop.Repo;
using VATServer.Ordinary;
namespace VATClient
{
    public partial class FormTransferReceive : Form
    {
        #region Constructors
        public FormTransferReceive()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();
            //connVM.SysdataSource = SysDBInfoVM.SysdataSource;
            //connVM.SysPassword = SysDBInfoVM.SysPassword;
            //connVM.SysUserName = SysDBInfoVM.SysUserName;
            //connVM.SysDatabaseName = DatabaseInfoVM.DatabaseName;
            //SetupBOMReceive();
            //if (Program.ReceiveFromBOM == "Y")
            //{
            //    MessageBox.Show("Raw product is Receive with Finish product receiving", this.Text, MessageBoxButtons.OK,
            //                    MessageBoxIcon.Information);
            //    //this.Close();
            //    return;
            //}
        }
        #endregion

        #region Global Variables
        List<ProductMiniDTO> ProductsMini = new List<ProductMiniDTO>();
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();

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
        string encriptedReceiveHeaderData;
        private string NextID = string.Empty;
        private bool ChangeData = false;
        private DataTable ProductResultDs;
        public string VFIN = "221";
        private bool IsUpdate = false;
        private bool Post = false;
        private bool IsPost = false;
        private string CategoryId { get; set; }
        private string UOMIdParam = "";
        private string UOMFromParam = "";
        private string UOMToParam = "";
        private string ActiveStatusUOMParam = "";
        DataSet formLoadDS = new DataSet();
        private DataTable uomResult;
        List<UomDTO> UOMs = new List<UomDTO>();
        private string branchName = "";
        private DataTable dtbranchNames;
        List<BranchDTO> BranchLists = new List<BranchDTO>();
        //List
        private int ReceivePlaceQty;
        private int ReceivePlaceAmt;
        private BOMNBRVM bomNbrs = new BOMNBRVM();
        private List<BOMOHVM> bomOhs = new List<BOMOHVM>();
        private List<BOMItemVM> bomItems = new List<BOMItemVM>();

        private int SearchBranchId = 0;


        #region Global Variables For BackGroundWorker
        private string ReceiveHeaderData = string.Empty;
        private string ReceiveDetailData = string.Empty;
        private string ReceiveResult = string.Empty;
        private string ReceiveResultPost = string.Empty;
        private DataTable ReceiveDetailResult;
        private DataTable transferDetailResult;
        private string ProductData = string.Empty;
        private string varItemNo, varCategoryID, varIsRaw, varHSCodeNo, varActiveStatus, varTrading, varNonStock, varProductCode;
        private TransferReceiveVM TransferReceiveVM;
        private List<TransferReceiveDetailVM> TransferReceiveDetailVMs = new List<TransferReceiveDetailVM>();
        private DataSet StatusResult;
        private DataTable ProductTypeResult;
        //private bool Edit = false;
        //private bool Add = false;
        #endregion
        #endregion

        #region Methods 01 / Save, Update, Post, Search


        private void TransactionTypes()
        {
            #region Transaction Type
            transactionType = string.Empty;
            if (rbtn62In.Checked)
            {
                transactionType = "62In";
            }
            else if (rbtn61In.Checked)
            {
                transactionType = "61In";
            }
            #endregion Transaction Type
        }

        private void btnSave_Click(object sender, EventArgs e)
        {

            #region Initializ

            SearchBranchId = Program.BranchId;
            sqlResults = new string[4];

            #endregion

            #region try

            try
            {

                #region Check Point

                #region Null Check

                #region Comment before 28 Oct 2020

                //////if (Add == false)
                //////{
                //////    MessageBox.Show(MessageVM.msgNotAddAccess, this.Text);
                //////    return;
                //////}
                //////if (IsPost == true)
                //////{
                //////    MessageBox.Show(MessageVM.ThisTransactionWasPosted, this.Text);
                //////    return;
                //////}

                #endregion

                if (Program.CheckLicence(dtpReceiveDate.Value) == true)
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

                if (txtBranchDBName.Text == "")
                {
                    MessageBox.Show("Please Enter Branch Name Receive From");
                    txtTransferFrom.Focus();
                    return;
                }

                #endregion

                #region Exist Check

                TransferReceiveVM vm = new TransferReceiveVM();

                if (!string.IsNullOrWhiteSpace(NextID))
                {
                    vm = new TransferReceiveDAL().SelectAllList(0, new[] { "tr.TransferReceiveNo" }, new[] { NextID }, null, null, connVM).FirstOrDefault();
                }

                if (vm != null && !string.IsNullOrWhiteSpace(vm.Id))
                {
                    throw new Exception("This Invoice Already Exist! Cannot Add!" + Environment.NewLine + "Invoice No: " + NextID);

                }

                #region Transfer From No Check

                ////txtTransferFromNo.Text.Trim()


                if (!string.IsNullOrWhiteSpace(txtTransferFromNo.Text.Trim()))
                {
                    vm = new TransferReceiveDAL().SelectAllList(0, new[] { "tr.TransferFromNo" }, new[] { txtTransferFromNo.Text.Trim() }, null, null, connVM).FirstOrDefault();
                }

                if (vm != null && !string.IsNullOrWhiteSpace(vm.Id))
                {
                    throw new Exception("This Transfer From No Already Exist! Cannot Add!" + Environment.NewLine + "Transfer From No: " + vm.TransferFromNo);

                }

                #endregion

                #endregion

                #region Details Check

                if (dgvReceive.RowCount <= 0)
                {
                    MessageBox.Show("Please insert Details information for transaction", this.Text, MessageBoxButtons.OK,
                                     MessageBoxIcon.Information);
                    return;
                }

                #endregion

                #endregion

                #region Transaction Type

                TransactionTypes();

                #endregion

                #region Value Assign

                #region Master Model

                TransferReceiveVM = new TransferReceiveVM();
                TransferReceiveVM.TransferReceiveNo = NextID.ToString();
                TransferReceiveVM.TransactionDateTime = dtpReceiveDate.Value.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss");
                TransferReceiveVM.TotalAmount = Convert.ToDecimal(txtTotalAmount.Text.Trim());
                TransferReceiveVM.SerialNo = txtSerialNo.Text.Trim().Replace(" ", "");
                TransferReceiveVM.ReferenceNo = txtReferenceNo.Text.Trim().Replace(" ", "");
                TransferReceiveVM.Comments = txtComments.Text.Trim();
                TransferReceiveVM.CreatedBy = Program.CurrentUser;
                TransferReceiveVM.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                TransferReceiveVM.LastModifiedBy = Program.CurrentUser;
                TransferReceiveVM.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                TransferReceiveVM.TransactionType = transactionType;
                TransferReceiveVM.TransferNo = txtTransferNo.Text.Trim();
                TransferReceiveVM.TransferFromNo = txtTransferFromNo.Text.Trim();
                TransferReceiveVM.TransferFrom = Convert.ToInt32(txtBranchDBName.Text.Trim());
                TransferReceiveVM.Post = "N";
                TransferReceiveVM.BranchId = Program.BranchId;

                #endregion

                #region Detail Model

                //TransferReceiveVM.TotalVATAmount = Convert.ToDecimal(txtTotalVATAmount.Text.Trim());
                //TransferReceiveVM.TotalSDAmount = Convert.ToDecimal(txtTotalSDAmount.Text.Trim());
                TransferReceiveDetailVMs = new List<TransferReceiveDetailVM>();
                for (int i = 0; i < dgvReceive.RowCount; i++)
                {
                    TransferReceiveDetailVM detail = new TransferReceiveDetailVM();
                    detail.TransferReceiveNo = NextID.ToString();
                    detail.ReceiveLineNo = dgvReceive.Rows[i].Cells["LineNo"].Value.ToString();
                    detail.ItemNo = dgvReceive.Rows[i].Cells["ItemNo"].Value.ToString();
                    detail.Quantity = Convert.ToDecimal(dgvReceive.Rows[i].Cells["Quantity"].Value.ToString());
                    detail.CostPrice = Convert.ToDecimal(dgvReceive.Rows[i].Cells["UnitPrice"].Value.ToString());
                    detail.UOM = dgvReceive.Rows[i].Cells["UOM"].Value.ToString();
                    detail.SubTotal = Convert.ToDecimal(dgvReceive.Rows[i].Cells["SubTotal"].Value.ToString());
                    detail.Comments = "FromTransferReceive";
                    detail.TransactionDateTime = dtpReceiveDate.Value.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss");
                    detail.TransferFrom = Convert.ToInt32(txtBranchDBName.Text.Trim());
                    detail.UOMQty = Convert.ToDecimal(dgvReceive.Rows[i].Cells["UOMQty"].Value.ToString());
                    detail.UOMn = dgvReceive.Rows[i].Cells["UOMn"].Value.ToString();
                    detail.UOMc = Convert.ToDecimal(dgvReceive.Rows[i].Cells["UOMc"].Value.ToString());
                    detail.UOMPrice = Convert.ToDecimal(dgvReceive.Rows[i].Cells["UOMPrice"].Value.ToString());

                    detail.VATRate = Convert.ToDecimal(dgvReceive.Rows[i].Cells["VATRate"].Value.ToString());
                    detail.VATAmount = Convert.ToDecimal(dgvReceive.Rows[i].Cells["VATAmount"].Value.ToString());
                    detail.SDRate = Convert.ToDecimal(dgvReceive.Rows[i].Cells["SDRate"].Value.ToString());
                    detail.SDAmount = Convert.ToDecimal(dgvReceive.Rows[i].Cells["SDAmount"].Value.ToString());
                    detail.Weight = dgvReceive.Rows[i].Cells["Weight"].Value.ToString();

                    detail.BranchId = Program.BranchId;
                    detail.TransferFromNo = txtTransferFromNo.Text.Trim();


                    TransferReceiveDetailVMs.Add(detail);

                }// End For

                #endregion

                #endregion

                #region Check Point

                if (TransferReceiveDetailVMs.Count() <= 0)
                {
                    MessageBox.Show("Please insert Details information for transaction", this.Text, MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                    return;
                }

                #endregion

                #region Button Stats

                this.btnSave.Enabled = false;
                this.progressBar1.Visible = true;

                #endregion

                #region Save Data

                ITransferReceive receiveDal = OrdinaryVATDesktop.GetObject<TransferReceiveDAL, TransferReceiveRepo, ITransferReceive>(OrdinaryVATDesktop.IsWCF);

                sqlResults = receiveDal.Insert(TransferReceiveVM, TransferReceiveDetailVMs, null, null, connVM);



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
                        txtReceiveNo.Text = sqlResults[2].ToString();
                        IsPost = Convert.ToString(sqlResults[3].ToString()) == "Y" ? true : false;
                        for (int i = 0; i < dgvReceive.RowCount; i++)
                        {
                            dgvReceive["Status", dgvReceive.RowCount - 1].Value = "Old";
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
                #region comment before 28 Oct 2020

                ////////if (Edit == false)
                ////////{
                ////////    MessageBox.Show(MessageVM.msgNotEditAccess, this.Text);
                ////////    return;
                ////////}

                #endregion

                #region comment 28 Oct 2020

                //if (IsPost == true)
                //{
                //    MessageBox.Show(MessageVM.ThisTransactionWasPosted, this.Text);
                //    return;
                //}

                #endregion

                #region Check Point

                if (SearchBranchId != Program.BranchId)
                {
                    MessageBox.Show(MessageVM.SelfBranchNotMatch, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (Program.CheckLicence(dtpReceiveDate.Value) == true)
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

                #endregion

                #region Transaction Types

                TransactionTypes();

                #endregion

                #region Check Point

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
                if (txtBranchDBName.Text == "")
                {
                    MessageBox.Show("Please Enter Branch Name Receive From");
                    //////cmbTransferFrom.Focus();
                    txtTransferFrom.Focus();

                    return;
                }
                if (dgvReceive.RowCount <= 0)
                {
                    MessageBox.Show("Please insert Details information for transaction", this.Text, MessageBoxButtons.OK,
                                     MessageBoxIcon.Information);
                    return;
                }

                #endregion

                #region Value Assign

                #region Master Value Assign

                TransferReceiveVM = new TransferReceiveVM();
                TransferReceiveVM.TransferReceiveNo = NextID.ToString();
                TransferReceiveVM.TransactionDateTime = dtpReceiveDate.Value.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss");
                TransferReceiveVM.TotalAmount = Convert.ToDecimal(txtTotalAmount.Text.Trim());
                TransferReceiveVM.SerialNo = txtSerialNo.Text.Trim().Replace(" ", "");
                TransferReceiveVM.ReferenceNo = txtReferenceNo.Text.Trim().Replace(" ", "");
                TransferReceiveVM.Comments = txtComments.Text.Trim();
                TransferReceiveVM.CreatedBy = Program.CurrentUser;
                TransferReceiveVM.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                TransferReceiveVM.LastModifiedBy = Program.CurrentUser;
                TransferReceiveVM.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                //////TransferReceiveVM.ReceiveNo = NextID.ToString();
                TransferReceiveVM.TransactionType = transactionType;
                TransferReceiveVM.Post = "N";
                TransferReceiveVM.TransferFromNo = txtTransferFromNo.Text.Trim();
                TransferReceiveVM.TransferFrom = Convert.ToInt32(txtBranchDBName.Text.Trim());
                //////TransferReceiveVM.TotalVATAmount = Convert.ToDecimal(txtTotalVATAmount.Text.Trim());
                //////TransferReceiveVM.TotalSDAmount = Convert.ToDecimal(txtTotalSDAmount.Text.Trim());
                //////encriptedReceiveHeaderData = Converter.DESEncrypt(PassPhrase, EnKey, ReceiveHeaderData);

                #endregion

                #region Detail Value Assign

                TransferReceiveDetailVMs = new List<TransferReceiveDetailVM>();
                for (int i = 0; i < dgvReceive.RowCount; i++)
                {
                    TransferReceiveDetailVM detail = new TransferReceiveDetailVM();
                    detail.TransferReceiveNo = NextID.ToString();
                    detail.ReceiveLineNo = dgvReceive.Rows[i].Cells["LineNo"].Value.ToString();
                    detail.ItemNo = dgvReceive.Rows[i].Cells["ItemNo"].Value.ToString();
                    detail.Quantity = Convert.ToDecimal(dgvReceive.Rows[i].Cells["Quantity"].Value.ToString());
                    detail.CostPrice = Convert.ToDecimal(dgvReceive.Rows[i].Cells["UnitPrice"].Value.ToString());
                    detail.UOM = dgvReceive.Rows[i].Cells["UOM"].Value.ToString();
                    detail.SubTotal = Convert.ToDecimal(dgvReceive.Rows[i].Cells["SubTotal"].Value.ToString());
                    detail.Comments = "FromTransferReceive";
                    //detail.ReceiveNoD = NextID.ToString();
                    detail.TransactionDateTime = dtpReceiveDate.Value.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss");
                    detail.TransferFrom = Convert.ToInt32(txtBranchDBName.Text.Trim());
                    detail.UOMQty = Convert.ToDecimal(dgvReceive.Rows[i].Cells["UOMQty"].Value.ToString());
                    detail.UOMn = dgvReceive.Rows[i].Cells["UOMn"].Value.ToString();
                    detail.UOMc = Convert.ToDecimal(dgvReceive.Rows[i].Cells["UOMc"].Value.ToString());
                    detail.UOMPrice = Convert.ToDecimal(dgvReceive.Rows[i].Cells["UOMPrice"].Value.ToString());

                    detail.VATRate = Convert.ToDecimal(dgvReceive.Rows[i].Cells["VATRate"].Value.ToString());
                    detail.VATAmount = Convert.ToDecimal(dgvReceive.Rows[i].Cells["VATAmount"].Value.ToString());
                    detail.SDRate = Convert.ToDecimal(dgvReceive.Rows[i].Cells["SDRate"].Value.ToString());
                    detail.SDAmount = Convert.ToDecimal(dgvReceive.Rows[i].Cells["SDAmount"].Value.ToString());
                    detail.Weight = dgvReceive.Rows[i].Cells["Weight"].Value.ToString();

                    TransferReceiveDetailVMs.Add(detail);
                }// End For

                #endregion

                #endregion

                #region Detail Value Check

                if (TransferReceiveDetailVMs.Count() <= 0)
                {
                    MessageBox.Show("Please insert Details information for transaction", this.Text, MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                    return;
                }

                #endregion

                #region Button Stats

                this.btnUpdate.Enabled = false;
                this.progressBar1.Visible = true;

                #endregion

                #region Background Worker Save

                //////bgwUpdate.RunWorkerAsync();

                #endregion

                #region Statement

                sqlResults = new string[4];
                //TransferReceiveDAL receiveDal = new TransferReceiveDAL();
                ITransferReceive receiveDal = OrdinaryVATDesktop.GetObject<TransferReceiveDAL, TransferReceiveRepo, ITransferReceive>(OrdinaryVATDesktop.IsWCF);

                sqlResults = receiveDal.Update(TransferReceiveVM, TransferReceiveDetailVMs, connVM, Program.CurrentUserID);

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
                        txtReceiveNo.Text = sqlResults[2].ToString();
                        IsPost = Convert.ToString(sqlResults[3].ToString()) == "Y" ? true : false;
                        for (int i = 0; i < dgvReceive.RowCount; i++)
                        {
                            dgvReceive["Status", dgvReceive.RowCount - 1].Value = "Old";
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

                #region Transfer Receive Post

                #region Check Point


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

                if (Program.CheckLicence(dtpReceiveDate.Value) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }

                #endregion Check Point

                #region Comment before 28 Oct 2020

                ////////TransactionTypes();
                ////////if (IsUpdate == false)
                ////////{
                ////////    NextID = string.Empty;
                ////////}
                ////////else
                ////////{
                ////////    NextID = txtReceiveNo.Text.Trim();
                ////////}
                ////////if (txtComments.Text == "")
                ////////{
                ////////    txtComments.Text = "-";
                ////////}
                ////////if (txtSerialNo.Text == "")
                ////////{
                ////////    txtSerialNo.Text = "-";
                ////////}
                ////////if (txtReferenceNo.Text == "")
                ////////{
                ////////    txtReferenceNo.Text = "-";
                ////////}
                ////////if (txtBranchDBName.Text == "")
                ////////{
                ////////    MessageBox.Show("Please Enter Branch Name Receive From");
                ////////    cmbTransferFrom.Focus();
                ////////    return;
                ////////}
                ////////if (dgvReceive.RowCount <= 0)
                ////////{
                ////////    MessageBox.Show("Please insert Details information for transaction", this.Text, MessageBoxButtons.OK,
                ////////                     MessageBoxIcon.Information);
                ////////    return;
                ////////}
                ////////TransferReceiveVM = new TransferReceiveVM();
                ////////TransferReceiveVM.TransferReceiveNo = NextID.ToString();
                ////////TransferReceiveVM.TransactionDateTime = dtpReceiveDate.Value.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss");
                ////////TransferReceiveVM.TotalAmount = Convert.ToDecimal(txtTotalAmount.Text.Trim());
                ////////TransferReceiveVM.SerialNo = txtSerialNo.Text.Trim();
                ////////TransferReceiveVM.ReferenceNo = txtReferenceNo.Text.Trim();
                ////////TransferReceiveVM.Comments = txtComments.Text.Trim();
                ////////TransferReceiveVM.TransferFrom = txtBranchDBName.Text.Trim();
                ////////TransferReceiveVM.CreatedBy = Program.CurrentUser;
                ////////TransferReceiveVM.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                ////////TransferReceiveVM.LastModifiedBy = Program.CurrentUser;
                ////////TransferReceiveVM.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                ////////TransferReceiveVM.ReceiveNo = NextID.ToString();
                ////////TransferReceiveVM.TransactionType = transactionType;
                ////////TransferReceiveVM.Post = "Y";
                ////////TransferReceiveVM.TransferFromNo = txtTransferFromNo.Text.Trim();
                ////////TransferReceiveVM.TransferFrom = txtBranchDBName.Text.Trim();
                ////////TransferReceiveVM.TotalVATAmount = Convert.ToDecimal(txtTotalVATAmount.Text.Trim());
                ////////TransferReceiveVM.TotalSDAmount = Convert.ToDecimal(txtTotalSDAmount.Text.Trim());
                ////////TransferReceiveVM.ReturnId = txtReceiveNoP.Text.Trim();
                ////////TransferReceiveDetailVMs = new List<TransferReceiveDetailVM>();
                ////////for (int i = 0; i < dgvReceive.RowCount; i++)
                ////////{
                ////////    TransferReceiveDetailVM detail = new TransferReceiveDetailVM();
                ////////    detail.TransferReceiveNo = NextID.ToString();
                ////////    detail.ReceiveLineNo = dgvReceive.Rows[i].Cells["LineNo"].Value.ToString();
                ////////    detail.ItemNo = dgvReceive.Rows[i].Cells["ItemNo"].Value.ToString();
                ////////    detail.Quantity = Convert.ToDecimal(dgvReceive.Rows[i].Cells["Quantity"].Value.ToString());
                ////////    detail.CostPrice = Convert.ToDecimal(dgvReceive.Rows[i].Cells["UnitPrice"].Value.ToString());
                ////////    detail.UOM = dgvReceive.Rows[i].Cells["UOM"].Value.ToString();
                ////////    detail.SubTotal = Convert.ToDecimal(dgvReceive.Rows[i].Cells["SubTotal"].Value.ToString());
                ////////    detail.Comments = "FromTransferReceive";
                ////////    detail.TransactionDateTime = dtpReceiveDate.Value.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss");
                ////////    detail.TransferFrom = txtBranchDBName.Text.Trim();
                ////////    detail.UOMQty = Convert.ToDecimal(dgvReceive.Rows[i].Cells["UOMQty"].Value.ToString());
                ////////    detail.UOMn = dgvReceive.Rows[i].Cells["UOMn"].Value.ToString();
                ////////    detail.UOMc = Convert.ToDecimal(dgvReceive.Rows[i].Cells["UOMc"].Value.ToString());
                ////////    detail.UOMPrice = Convert.ToDecimal(dgvReceive.Rows[i].Cells["UOMPrice"].Value.ToString());

                ////////    detail.VATRate = Convert.ToDecimal(dgvReceive.Rows[i].Cells["VATRate"].Value.ToString());
                ////////    detail.VATAmount = Convert.ToDecimal(dgvReceive.Rows[i].Cells["VATAmount"].Value.ToString());
                ////////    detail.SDRate = Convert.ToDecimal(dgvReceive.Rows[i].Cells["SDRate"].Value.ToString());
                ////////    detail.SDAmount = Convert.ToDecimal(dgvReceive.Rows[i].Cells["SDAmount"].Value.ToString());
                ////////    TransferReceiveDetailVMs.Add(detail);
                ////////}
                ////////if (TransferReceiveDetailVMs.Count() <= 0)
                ////////{
                ////////    MessageBox.Show("Please insert Details information for transaction", this.Text, MessageBoxButtons.OK,
                ////////                    MessageBoxIcon.Information);
                ////////    return;
                ////////}
                ////////this.btnPost.Enabled = false;
                ////////this.progressBar1.Visible = true;
                ////////sqlResults = new string[4];
                #endregion

                #region Receive Post

                //TransferReceiveDAL receiveDal = new TransferReceiveDAL();
                ITransferReceive receiveDal = OrdinaryVATDesktop.GetObject<TransferReceiveDAL, TransferReceiveRepo, ITransferReceive>(OrdinaryVATDesktop.IsWCF);
                TransferReceiveVM = new TransferReceiveVM();

                TransferReceiveVM.TransferReceiveNo = txtReceiveNo.Text.Trim();
                sqlResults = receiveDal.Post(TransferReceiveVM, connVM);

                #endregion

                #endregion

                #region Statement

                if (sqlResults.Length > 0)
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
                        txtReceiveNo.Text = sqlResults[2].ToString();
                        IsPost = Convert.ToString(sqlResults[3].ToString()) == "Y" ? true : false;
                        for (int i = 0; i < dgvReceive.RowCount; i++)
                        {
                            dgvReceive["Status", dgvReceive.RowCount - 1].Value = "Old";
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

        private void selectLastRow()
        {
            #region try
            try
            {
                if (dgvReceive.Rows.Count > 0)
                {
                    dgvReceive.Rows[dgvReceive.Rows.Count - 1].Selected = true;
                    dgvReceive.CurrentCell = dgvReceive.Rows[dgvReceive.Rows.Count - 1].Cells[1];
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
                decimal SumQty = 0;
                decimal Quantity = 0;
                decimal Cost = 0;
                decimal SubTotal = 0;
                for (int i = 0; i < dgvReceive.RowCount; i++)
                {
                    dgvReceive[0, i].Value = i + 1;
                    Quantity = Convert.ToDecimal(dgvReceive["Quantity", i].Value);
                    Cost = Convert.ToDecimal(dgvReceive["UnitPrice", i].Value);
                    SubTotal = Cost * Quantity;
                    if (Program.CheckingNumericString(SubTotal.ToString(), "SubTotal") == true)
                        SubTotal = Convert.ToDecimal(Program.FormatingNumeric(SubTotal.ToString(), ReceivePlaceQty));
                    dgvReceive["SubTotal", i].Value = Convert.ToDecimal(SubTotal);
                    //dgvReceive["SubTotal", i].Value = Convert.ToDecimal(SubAmount).ToString();//"0,0.00");
                    SumSubTotal = SumSubTotal + Convert.ToDecimal(dgvReceive["SubTotal", i].Value);
                    dgvReceive["Change", i].Value = Convert.ToDecimal(Convert.ToDecimal(dgvReceive["Quantity", i].Value)
                        - Convert.ToDecimal(dgvReceive["Previous", i].Value)).ToString();//"0,0.0000");

                    SumQty += Quantity;
                }
                txtTotalVATAmount.Text = "0";
                txtTotalAmount.Text = Convert.ToDecimal(SumSubTotal).ToString();

                txtTotalQuantity.Text = SumQty.ToString();
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
                if (rbtn61In.Checked == false)
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
                dgvReceive.CurrentRow.Cells["Status"].Value = "Delete";
                dgvReceive.CurrentRow.Cells["Quantity"].Value = 0.00;
                dgvReceive.CurrentRow.Cells["UnitPrice"].Value = 0.00;
                dgvReceive.CurrentRow.DefaultCellStyle.ForeColor = Color.Red;
                dgvReceive.CurrentRow.DefaultCellStyle.Font = new Font(this.Font, FontStyle.Strikeout);
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
            // cmbTransferFrom.Text = "Select";
            txtTransferFrom.Text = "";
            txtBranchDBName.Text = "";
            txtCommentsDetail.Text = "NA";
            txtHSCode.Text = "";
            txtReceiveNo.Text = "";
            txtProductCode.Text = "";
            txtProductName.Text = "";
            txtQuantity.Text = "0.00";
            txtSerialNo.Text = "";
            txtReferenceNo.Text = "";
            txtTotalAmount.Text = "0.00";
            txtUnitCost.Text = "0.00";
            cmbIsRaw.Text = "";
            txtUOM.Text = "";
            txtTransferFromNo.Text = "";
            dtpReceiveDate.Value = Convert.ToDateTime(Program.SessionDate.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss"));
            dgvReceive.Rows.Clear();
        }

        #endregion

        #region Methods 02

        private void cmbProduct_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }
        private void txtComments_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }
        private void txtVATRate_Leave(object sender, EventArgs e)
        {
            Program.FormatTextBoxRate(txtVATRate, "VAT Rate");
        }
        private void txtQuantity_Leave(object sender, EventArgs e)
        {
            //Program.FormatTextBoxQty4(txtQuantity, "Quantity");
            if (Program.CheckingNumericTextBox(txtQuantity, "Total Quantity") == true)
            {
                txtQuantity.Text = Program.FormatingNumeric(txtQuantity.Text.Trim(), ReceivePlaceQty).ToString();
            }
        }
        private void txtQuantity_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }
        private void FormTransferReceive_Load(object sender, EventArgs e)
        {
            #region try
            try
            {
                btnSave.Text = "&Add";
                txtReceiveNo.Text = "~~~ New ~~~";
                FormMaker();
                FormLoad();
                ClearAllFields();
                ChangeData = false;
                System.Windows.Forms.ToolTip ToolTip1 = new System.Windows.Forms.ToolTip();
                ToolTip1.SetToolTip(this.btnImport, "Import from Excel file");
                ToolTip1.SetToolTip(this.chkSame, "Import from same Excel file");
                Post = true;// Convert.ToString(Program.Post) == "Y" ? true : false;
                //Add = true;//Convert.ToString(Program.Add) == "Y" ? true : false;
                //Edit = true;//Convert.ToString(Program.Edit) == "Y" ? true : false;
                #region Settings
                string vReceivePlaceQty, vReceivePlaceAmt, vReceiveFromBOM, vReceiveAutoPost = string.Empty;
                CommonDAL commonDal = new CommonDAL();
                vReceivePlaceQty = commonDal.settingsDesktop("Issue", "Quantity", null, connVM);
                vReceivePlaceAmt = commonDal.settingsDesktop("Issue", "Amount", null, connVM);
                vReceiveFromBOM = commonDal.settingsDesktop("IssueFromBOM", "IssueFromBOM", null, connVM);
                vReceiveAutoPost = commonDal.settingsDesktop("IssueFromBOM", "IssueAutoPost", null, connVM);
                if (string.IsNullOrEmpty(vReceivePlaceQty)
                    || string.IsNullOrEmpty(vReceivePlaceAmt)
                    || string.IsNullOrEmpty(vReceiveFromBOM)
                    || string.IsNullOrEmpty(vReceiveAutoPost))
                {
                    MessageBox.Show(MessageVM.msgSettingValueNotSave, this.Text);
                    return;
                }
                ReceivePlaceQty = Convert.ToInt32(vReceivePlaceQty);
                ReceivePlaceAmt = Convert.ToInt32(vReceivePlaceAmt);
                bool IssueFromBOM = Convert.ToBoolean(vReceiveFromBOM == "Y" ? true : false);
                if (IssueFromBOM == true)
                {
                    bool IssueAutoPost = Convert.ToBoolean(vReceiveAutoPost == "Y" ? true : false);
                    if (IssueAutoPost == false)
                    {
                        btnSave.Enabled = false;
                    }
                }
                #endregion Settings
                progressBar1.Visible = true;
                //bgwLoad.RunWorkerAsync();
                #region Loading UOM, Product, TransferTo
                UOMDAL uomdal = new UOMDAL();

                UOMVM uomVM = new UOMVM();
                uomVM.UOMID = UOMIdParam;
                uomVM.UOMFrom = UOMFromParam;
                uomVM.UOMTo = UOMToParam;
                uomVM.ActiveStatus = ActiveStatusUOMParam;
                uomVM.DatabaseName = Program.DatabaseName;
                //uomResult = uomdal.SearchUOM(UOMIdParam, UOMFromParam, UOMToParam, ActiveStatusUOMParam, Program.DatabaseName);
                //uomResult = uomdal.SearchUOM(uomVM);


                ProductDAL productDal = new ProductDAL();
                ProductVM productVM = new ProductVM();
                productVM.ItemNo = varItemNo;
                productVM.CategoryID = varCategoryID;
                productVM.IsRaw = varIsRaw;
                productVM.CategoryName = varHSCodeNo;
                productVM.ActiveStatus = varActiveStatus;
                productVM.Trading = varTrading;
                productVM.NonStock = varNonStock;
                productVM.ProductCode = varProductCode;
                //ProductResultDs = productDal.SearchProductMiniDS(productVM);

                //TransferReceiveDAL _transferReceiveDal = new TransferReceiveDAL();
                ITransferReceive _transferReceiveDal = OrdinaryVATDesktop.GetObject<TransferReceiveDAL, TransferReceiveRepo, ITransferReceive>(OrdinaryVATDesktop.IsWCF);

                formLoadDS = _transferReceiveDal.FormLoad(uomVM, productVM, branchName, connVM);

                //BranchDAL branchDal = new BranchDAL();
                //dtbranchNames = branchDal.SearchBranchName(branchName);
                BranchLists.Clear();
                //foreach(DataRow item in dtbranchNames.Rows)
                foreach (DataRow item in formLoadDS.Tables[2].Rows)
                {
                    BranchDTO br = new BranchDTO();
                    br.Id = Convert.ToInt32(item["Id"].ToString()); ;
                    br.Name = item["Name"].ToString();
                    br.DBName = item["DBName"].ToString();
                    BranchLists.Add(br);
                }
                BranchNameLoad();
                ProductsMini.Clear();
                foreach (DataRow item2 in formLoadDS.Tables[1].Rows)
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
                #endregion UOM
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
                FileLogger.Log(this.Name, "FormTransferReceive_Load", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "FormTransferReceive_Load", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (FormatException ex)
            {
                FileLogger.Log(this.Name, "FormTransferReceive_Load", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "FormTransferReceive_Load", exMessage);
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
                FileLogger.Log(this.Name, "FormTransferReceive_Load", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormTransferReceive_Load", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormTransferReceive_Load", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "FormTransferReceive_Load", exMessage);
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
                //btnSearchReceiveNoP.Visible = false;
                //txtReceiveNoP.Visible = false;
                #region Transaction Type
                if (rbtn62In.Checked)
                {
                    //btn62.Visible = true;

                    this.Text = "Transfer Receive VAT FG (In)";
                }
                else if (rbtn61In.Checked)
                {
                    //btnVAT16.Visible = true;
                    this.Text = "Transfer Receive VAT RM (In)";
                    //btnSearchReceiveNoP.Visible = true;
                    //txtReceiveNoP.Visible = true;
                }
                #endregion Transaction Type


                TransactionTypes();
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
            #region Product
            if (CategoryId == null)
            {
                varItemNo = "";
                varCategoryID = "";
                if (rbtn62In.Checked)
                {
                    varIsRaw = "Finish";
                }
                else if (rbtn61In.Checked)
                {
                    varIsRaw = "Raw";
                }
                varHSCodeNo = "";
                varActiveStatus = "Y";
                varTrading = "N";
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
            if (rbtn62In.Checked)
            {
                txtCategoryName.Text = "Finish";
            }
            else if (rbtn61In.Checked)
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
                //////cmbTransferFrom.Items.Clear();
                //////var branchName = from br in BranchLists.ToList()
                //////                 orderby br.Name
                //////                 select br.Name;
                //////cmbTransferFrom.Items.AddRange(branchName.ToArray());
                //////cmbTransferFrom.Items.Insert(0, "Select");

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
                    varIsRaw = "Raw";
                    if (rbtn62In.Checked)
                    {
                        varIsRaw = "Finish";
                        txtCategoryName.Text = "Finish";
                    }
                    else if (rbtn61In.Checked)
                    {
                        varIsRaw = "Raw";
                        txtCategoryName.Text = "Raw";
                    }
                    varHSCodeNo = "";
                    varActiveStatus = "Y";
                    varTrading = "N";
                    varNonStock = "N";
                    varProductCode = "";

                }
                else
                {
                    varItemNo = "";
                    varCategoryID = CategoryId;
                    varIsRaw = "";
                    varHSCodeNo = txtCategoryName.Text.Trim();
                    varActiveStatus = "Y";
                    varTrading = "N";
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

                ProductVM productVM = new ProductVM();
                productVM.ItemNo = varItemNo;
                productVM.CategoryID = varCategoryID;
                productVM.IsRaw = varIsRaw;
                productVM.CategoryName = varHSCodeNo;
                productVM.Trading = varTrading;
                productVM.ActiveStatus = varActiveStatus;
                productVM.NonStock = varNonStock;
                productVM.ProductCode = varProductCode;

                ProductResultDs = new DataTable();
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
                    ProductsMini.Add(prod);
                }//End For
                //End Complete
                ProductSearchDsLoad();
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
                FileLogger.Log(this.Name, "ProductSearchDsFormLoad", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "ProductSearchDsFormLoad", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (FormatException ex)
            {
                FileLogger.Log(this.Name, "ProductSearchDsFormLoad", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "ProductSearchDsFormLoad", exMessage);
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
                FileLogger.Log(this.Name, "ProductSearchDsFormLoad", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ProductSearchDsFormLoad", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ProductSearchDsFormLoad", ex.Message + Environment.NewLine + ex.StackTrace);
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
                                     select prd.ProductName;
                    if (prodByName != null && prodByName.Any())
                    {
                        cmbProductName.Items.AddRange(prodByName.ToArray());
                    }
                }
                cmbProduct.Items.Insert(0, "Select");
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
        private void dgvReceive_DoubleClick(object sender, EventArgs e)
        {
            #region try
            try
            {
                if (dgvReceive.Rows.Count <= 0)
                {
                    MessageBox.Show("There is no data to select.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                //if (chkPCode.Checked)
                //{
                //    cmbProduct.Text = dgvReceive.CurrentRow.Cells["PCode"].Value.ToString();
                //}
                //else
                //{
                //    cmbProduct.Text = dgvReceive.CurrentRow.Cells["ItemName"].Value.ToString();
                //}
                cmbProduct.Text = dgvReceive.CurrentRow.Cells["PCode"].Value.ToString();
                cmbProductName.Text = dgvReceive.CurrentRow.Cells["ItemName"].Value.ToString();
                txtLineNo.Text = dgvReceive.CurrentCellAddress.Y.ToString();
                txtProductCode.Text = dgvReceive.CurrentRow.Cells["ItemNo"].Value.ToString();
                txtPCode.Text = dgvReceive.CurrentRow.Cells["PCode"].Value.ToString();
                txtProductName.Text = dgvReceive.CurrentRow.Cells["ItemName"].Value.ToString();
                txtUOM.Text = dgvReceive.CurrentRow.Cells["UOM"].Value.ToString();
                txtQuantity.Text = Convert.ToDecimal(dgvReceive.CurrentRow.Cells["Quantity"].Value).ToString();//"0,0.0000");
                txtUnitCost.Text = Convert.ToDecimal(dgvReceive.CurrentRow.Cells["UnitPrice"].Value).ToString();//"0,0.00");
                txtCommentsDetail.Text = "NA";// dgvReceive.CurrentRow.Cells["Comments"].Value.ToString();
                txtPrevious.Text = Convert.ToDecimal(dgvReceive.CurrentRow.Cells["Previous"].Value).ToString();//"0,0.0000");
                txtUOM.Text = dgvReceive.CurrentRow.Cells["UOMn"].Value.ToString();
                cmbUom.Items.Insert(0, txtUOM.Text.Trim());
                Uoms();
                cmbUom.Text = dgvReceive.CurrentRow.Cells["UOM"].Value.ToString();
                /*UOM Conversion */
                txtUomConv.Text = dgvReceive.CurrentRow.Cells["UOMc"].Value.ToString();


                /*UOM Convertion */
                ProductDAL productDal = new ProductDAL();
                //IProduct productDal = OrdinaryVATDesktop.GetObject<ProductDAL, ProductRepo, IProduct>(OrdinaryVATDesktop.IsWCF);

                DataTable priceDS = new DataTable();
                priceDS = productDal.AvgPriceNew(txtProductCode.Text, dtpReceiveDate.Value.ToString("yyyy-MMM-dd") +
                DateTime.Now.ToString(" HH:mm:ss"), null, null, false, true, true, true, connVM, Program.CurrentUserID);
                //txtQuantityInHand.Text = productDal.StockInHand(txtProductCode.Text, dtpReceiveDate.Value.ToString("yyyy-MMM-dd") +
                //                                                                DateTime.Now.ToString(" HH:mm:ss"),
                //                                                            null,
                //                                                            null).ToString();
                txtQuantityInHand.Text = priceDS.Rows[0]["Quantity"].ToString();
                PirceCall();
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
                FileLogger.Log(this.Name, "dgvReceive_DoubleClick", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "dgvReceive_DoubleClick", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (FormatException ex)
            {
                FileLogger.Log(this.Name, "dgvReceive_DoubleClick", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "dgvReceive_DoubleClick", exMessage);
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
                FileLogger.Log(this.Name, "dgvReceive_DoubleClick", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "dgvReceive_DoubleClick", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "dgvReceive_DoubleClick", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "dgvReceive_DoubleClick", exMessage);
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
        private void btnSearchReceiveNo_Click(object sender, EventArgs e)
        {
            #region try
            try
            {
                Program.fromOpen = "Me";
                TransactionTypes();
                string invoiceNo = FormTransferReceiveSearch.SelectOne(transactionType);
                if (!string.IsNullOrWhiteSpace(invoiceNo))
                {

                    txtReceiveNo.Text = invoiceNo;

                    SearchInvoice();
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
                FileLogger.Log(this.Name, "btnSearchReceiveNo_Click", exMessage);
            }
            #endregion
            finally
            {
                ChangeData = false;
                this.btnSearchReceiveNo.Enabled = true;
                this.progressBar1.Visible = false;
            }
        }

        private void SearchInvoice()
        {
            try
            {

                #region Data Call

                ITransferReceive receiveDal = OrdinaryVATDesktop.GetObject<TransferReceiveDAL, TransferReceiveRepo, ITransferReceive>(OrdinaryVATDesktop.IsWCF);

                TransferReceiveVM vm = new TransferReceiveVM();
                vm.TransferReceiveNo = txtReceiveNo.Text;
                vm.TransactionType = transactionType;
                vm.ReceiveDateFrom = "";
                vm.ReceiveDateTo = "";
                vm.TransferNo = "";
                vm.TransferFromNo = "";
                vm.Post = "";
                vm.BranchId = Program.BranchId;
                DataTable ReceiveResult = receiveDal.SearchTransferReceive(vm, connVM);  // Change 04

                #endregion

                #region Value Assign to Form Elements

                if (ReceiveResult.Rows.Count >= 1)
                {
                    DataRow dr = ReceiveResult.Rows[0];

                    txtId.Text = dr["Id"].ToString();
                    txtFiscalYear.Text = dr["FiscalYear"].ToString();
                    txtReceiveNo.Text = dr["TransferReceiveNo"].ToString();
                    dtpReceiveDate.Value = Convert.ToDateTime(dr["ReceiveDateTime"].ToString());
                    txtTotalAmount.Text = Program.ParseDecimalObject(Convert.ToDecimal(dr["TotalAmount"].ToString()).ToString());//"0,0.00");
                    txtSerialNo.Text = dr["SerialNo"].ToString();
                    txtReferenceNo.Text = dr["ReferenceNo"].ToString();
                    txtComments.Text = dr["Comments"].ToString();
                    txtBranchDBName.Text = dr["TransferFrom"].ToString();
                    txtTransferNo.Text = dr["TransferNo"].ToString();
                    txtTransferFromNo.Text = dr["TransferFromNo"].ToString();
                    // cmbTransferFrom.Text = dr["BranchName"].ToString();
                    txtTransferFrom.Text = dr["BranchName"].ToString();

                    IsPost = Convert.ToString(dr["Post"].ToString()) == "Y" ? true : false;
                    SearchBranchId = Convert.ToInt32(dr["BranchId"]);
                    ReceiveDetailData = txtReceiveNo.Text == "" ? "" : txtReceiveNo.Text.Trim();

                    #region Statement

                    ReceiveDetailResult = new DataTable();
                    if (!string.IsNullOrEmpty(txtReceiveNo.Text))
                    {
                        ReceiveDetailResult = receiveDal.SearchTransferDetail(txtReceiveNo.Text, connVM); // Change 04
                    }

                    #region DataGridView
                    dgvReceive.Rows.Clear();
                    int j = 0;
                    foreach (DataRow item in ReceiveDetailResult.Rows)
                    {
                        DataGridViewRow NewRow = new DataGridViewRow();
                        dgvReceive.Rows.Add(NewRow);
                        dgvReceive.Rows[j].Cells["LineNo"].Value = item["ReceiveLineNo"].ToString();        // ReceiveDetailFields[1].ToString();
                        dgvReceive.Rows[j].Cells["ItemNo"].Value = item["ItemNo"].ToString();             // ReceiveDetailFields[2].ToString();
                        dgvReceive.Rows[j].Cells["Quantity"].Value = Program.ParseDecimalObject(item["Quantity"].ToString());         //Convert.ToDecimal(ReceiveDetailFields[3].ToString()).ToString();//"0,0.0000");
                        dgvReceive.Rows[j].Cells["UnitPrice"].Value = Program.ParseDecimalObject(item["CostPrice"].ToString());       //Convert.ToDecimal(ReceiveDetailFields[4].ToString()).ToString();//"0,0.00");
                        dgvReceive.Rows[j].Cells["UOM"].Value = item["UOM"].ToString();                   // ReceiveDetailFields[6].ToString();
                        dgvReceive.Rows[j].Cells["SubTotal"].Value = Program.ParseDecimalObject(item["SubTotal"].ToString());         //Convert.ToDecimal(ReceiveDetailFields[9].ToString()).ToString();//"0,0.00");
                        dgvReceive.Rows[j].Cells["Comments"].Value = item["Comments"].ToString();         // ReceiveDetailFields[10].ToString();
                        dgvReceive.Rows[j].Cells["ItemName"].Value = item["ProductName"].ToString();      // ReceiveDetailFields[11].ToString();
                        dgvReceive.Rows[j].Cells["PCode"].Value = item["ProductCode"].ToString();         // ReceiveDetailFields[15].ToString();
                        dgvReceive.Rows[j].Cells["Status"].Value = "Old";
                        dgvReceive.Rows[j].Cells["Previous"].Value = Program.ParseDecimalObject(item["Quantity"].ToString());         //Convert.ToDecimal(ReceiveDetailFields[3].ToString()).ToString();//"0,0.0000"); //Quantity
                        dgvReceive.Rows[j].Cells["Stock"].Value = Program.ParseDecimalObject(item["Stock"].ToString());               //Convert.ToDecimal(ReceiveDetailFields[12].ToString()).ToString();//"0,0.0000");
                        dgvReceive.Rows[j].Cells["Change"].Value = 0;
                        dgvReceive.Rows[j].Cells["UOMc"].Value = item["UOMc"].ToString();
                        dgvReceive.Rows[j].Cells["UOMn"].Value = item["UOMn"].ToString();
                        dgvReceive.Rows[j].Cells["UOMQty"].Value = Program.ParseDecimalObject(item["UOMQty"].ToString());
                        dgvReceive.Rows[j].Cells["UOMPrice"].Value = Program.ParseDecimalObject(item["UOMPrice"].ToString());

                        dgvReceive.Rows[j].Cells["VATRate"].Value = Program.ParseDecimalObject(item["VATRate"].ToString());
                        dgvReceive.Rows[j].Cells["VATAmount"].Value = Program.ParseDecimalObject(item["VATAmount"].ToString());
                        dgvReceive.Rows[j].Cells["SDRate"].Value = Program.ParseDecimalObject(item["SDRate"].ToString());
                        dgvReceive.Rows[j].Cells["SDAmount"].Value = Program.ParseDecimalObject(item["SDAmount"].ToString());
                        dgvReceive.Rows[j].Cells["Weight"].Value = item["Weight"].ToString();
                        j = j + 1;
                    }  //End For
                    #endregion DataGridView

                    #region Button Stats

                    btnSave.Text = "&Save";

                    #endregion

                    #region Flag Update

                    IsUpdate = true;
                    ChangeData = false;

                    #endregion

                    #endregion
                }

                #endregion

                #region Button Stats

                btnSave.Enabled = true;
                btnUpdate.Enabled = true;
                btnPost.Enabled = true;

                #endregion
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
                FileLogger.Log(this.Name, "SearchInvoice", exMessage);
            }

        }

        #endregion

        #region Methods 03

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
        private void txtReceiveNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }

            if (e.KeyCode.Equals(Keys.Enter))
            {
                TransferReceiveNavigation("Current");
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
        private void dtpReceiveDate_KeyDown(object sender, KeyEventArgs e)
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


        #endregion

        #region Methods 04

        private void txtHSCode_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }
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
        private void dtpReceiveDate_ValueChanged(object sender, EventArgs e)
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
        private void FormTransferReceive_FormClosing(object sender, FormClosingEventArgs e)
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
            Program.FormatTextBoxRate(txtSD, "SD Rate");
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
        private void cmbUom_KeyDown(object sender, KeyEventArgs e)
        {
            btnAdd.Focus();
        }

        #endregion

        #region Methods 05 / Add Row, Change Row, Remove Row

        private void btnAdd_Click(object sender, EventArgs e)
        {
            #region try
            try
            {
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

                for (int i = 0; i < dgvReceive.RowCount; i++)
                {
                    if (dgvReceive.Rows[i].Cells["ItemNo"].Value.ToString() == txtProductCode.Text)
                    {
                        MessageBox.Show("Same Product already exist.", this.Text);
                        return;
                    }
                }

                if (string.IsNullOrWhiteSpace(cmbUom.Text.Trim()))
                {
                    throw new ArgumentNullException(this.Text, "Please select pack size");
                }
                UomsValue();
                DataGridViewRow NewRow = new DataGridViewRow();
                dgvReceive.Rows.Add(NewRow);
                dgvReceive["ItemNo", dgvReceive.RowCount - 1].Value = txtProductCode.Text.Trim();
                dgvReceive["ItemName", dgvReceive.RowCount - 1].Value = txtProductName.Text.Trim();
                dgvReceive["PCode", dgvReceive.RowCount - 1].Value = txtPCode.Text.Trim();
                //dgvReceive["UOM", dgvReceive.RowCount - 1].Value = txtUOM.Text.Trim();
                string strUom = cmbUom.Text.Trim().ToString();
                dgvReceive["UOM", dgvReceive.RowCount - 1].Value = strUom.Trim();
                if (Program.CheckingNumericTextBox(txtQuantity, "txtQuantity") == true)
                    txtQuantity.Text = Program.FormatingNumeric(txtQuantity.Text.Trim(), ReceivePlaceQty).ToString();
                if (Program.CheckingNumericTextBox(txtUnitCost, "txtUnitCost") == true)
                    txtUnitCost.Text = Program.FormatingNumeric(txtUnitCost.Text.Trim(), ReceivePlaceAmt).ToString();
                dgvReceive["Quantity", dgvReceive.RowCount - 1].Value = Convert.ToDecimal(txtQuantity.Text.Trim()).ToString();
                dgvReceive["UnitPrice", dgvReceive.RowCount - 1].Value =
                    Convert.ToDecimal(Convert.ToDecimal(txtUnitCost.Text.Trim()) * Convert.ToDecimal(txtUomConv.Text.Trim())).ToString();
                dgvReceive["UOMPrice", dgvReceive.RowCount - 1].Value = Convert.ToDecimal(Convert.ToDecimal(txtUnitCost.Text.Trim())).ToString();
                dgvReceive["Comments", dgvReceive.RowCount - 1].Value = "NA";// txtCommentsDetail.Text.Trim();
                dgvReceive["Status", dgvReceive.RowCount - 1].Value = "New";
                dgvReceive["Stock", dgvReceive.RowCount - 1].Value = Convert.ToDecimal(txtQuantityInHand.Text.Trim()).ToString();
                dgvReceive["Previous", dgvReceive.RowCount - 1].Value = 0;// txtQuantity.Text.Trim();
                dgvReceive["Change", dgvReceive.RowCount - 1].Value = 0;
                dgvReceive["UOMPrice", dgvReceive.RowCount - 1].Value = Convert.ToDecimal(Convert.ToDecimal(txtUnitCost.Text.Trim())).ToString();
                dgvReceive["UOMc", dgvReceive.RowCount - 1].Value =
                    Convert.ToDecimal(txtUomConv.Text.Trim());// txtUOM.Text.Trim();
                dgvReceive["UOMn", dgvReceive.RowCount - 1].Value =
                    txtUOM.Text.Trim();// txtUOM.Text.Trim();
                dgvReceive["UOMQty", dgvReceive.RowCount - 1].Value =
                    Convert.ToDecimal(Convert.ToDecimal(txtQuantity.Text.Trim()) * Convert.ToDecimal(txtUomConv.Text.Trim()));
                Rowcalculate();
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
        private void btnChange_Click(object sender, EventArgs e)
        {
            if (dgvReceive.RowCount > 0)
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
                if (cmbUom.SelectedIndex == -1)
                {
                    throw new ArgumentNullException("", "Please select pack size");
                }
                UomsValue();
                //decimal quantity = Convert.ToDecimal(dgvReceive.CurrentRow.Cells["Quantity"].Value);
                //if (Convert.ToDecimal(txtQuantity.Text) > quantity)
                //{
                //    MessageBox.Show("Return quantity can not be greater than actual quantity.");
                //    txtQuantity.Text = quantity.ToString();
                //    txtQuantity.Focus();
                //    return;
                //}   
                #region Stock Chekc
                decimal StockValue, PreviousValue, ChangeValue, CurrentValue, purchaseQty;
                StockValue = Convert.ToDecimal(txtQuantityInHand.Text);
                PreviousValue = Convert.ToDecimal(txtPrevious.Text);
                CurrentValue = Convert.ToDecimal(txtQuantity.Text);


                #endregion Stock Chekc

                string strUom = cmbUom.SelectedItem.ToString();
                dgvReceive["UOM", dgvReceive.RowCount - 1].Value = strUom.Trim();
                //dgvReceive["PCode", dgvReceive.CurrentRow.Index].Value = txtPCode.Text.Trim();
                dgvReceive["Quantity", dgvReceive.CurrentRow.Index].Value = Convert.ToDecimal(txtQuantity.Text.Trim()).ToString();//"0,0.000");
                dgvReceive["UnitPrice", dgvReceive.RowCount - 1].Value =
                    Convert.ToDecimal(Convert.ToDecimal(txtUnitCost.Text.Trim()) * Convert.ToDecimal(txtUomConv.Text.Trim())).ToString();
                dgvReceive["UOMPrice", dgvReceive.RowCount - 1].Value = Convert.ToDecimal(Convert.ToDecimal(txtUnitCost.Text.Trim())).ToString();
                //dgvReceive["UnitPrice", dgvReceive.CurrentRow.Index].Value = Convert.ToDecimal(txtUnitCost.Text.Trim()).ToString();//"0,0.00");
                dgvReceive["Comments", dgvReceive.CurrentRow.Index].Value = "NA";// txtCommentsDetail.Text.Trim();
                dgvReceive["UOMc", dgvReceive.CurrentRow.Index].Value =
                Convert.ToDecimal(txtUomConv.Text.Trim());// txtUOM.Text.Trim();
                dgvReceive["UOMn", dgvReceive.CurrentRow.Index].Value = txtUOM.Text.Trim();// txtUOM.Text.Trim();
                dgvReceive["UOMQty", dgvReceive.CurrentRow.Index].Value =
                    Convert.ToDecimal(Convert.ToDecimal(txtQuantity.Text.Trim()) *
                    Convert.ToDecimal(txtUomConv.Text.Trim()));
                if (dgvReceive.CurrentRow.Cells["Status"].Value.ToString() != "New")
                {
                    dgvReceive["Status", dgvReceive.CurrentRow.Index].Value = "Change";
                }
                dgvReceive.CurrentRow.DefaultCellStyle.ForeColor = Color.Green;// Blue;
                //dgvReceive.CurrentRow.DefaultCellStyle.ForeColor = Color.Red;
                dgvReceive.CurrentRow.DefaultCellStyle.Font = new Font(this.Font, FontStyle.Regular);
                Rowcalculate();
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
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnChange_Click", exMessage);
            }
            #endregion

        }
        private void btnRemove_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvReceive.RowCount > 0)
                {
                    if (MessageBox.Show(MessageVM.msgWantToRemoveRow + "\nItem Code: " + dgvReceive.CurrentRow.Cells["PCode"].Value, this.Text, MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        dgvReceive.Rows.RemoveAt(dgvReceive.CurrentRow.Index);
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
                        txtReceiveNo.Text = "~~~ New ~~~";
                    }
                }
                else if (ChangeData == false)
                {
                    //ProductSearchDsFormLoad();
                    //ProductSearchDsLoad();
                    ClearAllFields();
                    btnSave.Text = "&Add";
                    txtReceiveNo.Text = "~~~ New ~~~";
                }
                IsUpdate = false;
                ChangeData = false;
                btnSave.Enabled = true;
                btnUpdate.Enabled = true;
                btnPost.Enabled = true;
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
        private void btnReceive_Click(object sender, EventArgs e)
        {
            #region try
            try
            {
                if (Program.CheckLicence(dtpReceiveDate.Value) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }
                TransactionTypes();
                FormRptTransferReceiveInformation frmTransferReceiveInformation = new FormRptTransferReceiveInformation();
                //mdi.RollDetailsInfo(frmRptIssueInformation.VFIN);
                //if (Program.Access != "Y")
                //{
                //    MessageBox.Show("You do not have to access this form", frmRptIssueInformation.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                //    return;
                //}
                if (txtReceiveNo.Text == "~~~ New ~~~")
                {
                    frmTransferReceiveInformation.txtIssueNo.Text = "";
                }
                else
                {
                    frmTransferReceiveInformation.txtIssueNo.Text = txtReceiveNo.Text.Trim();
                }
                frmTransferReceiveInformation.txtTransactionType.Text = transactionType;
                frmTransferReceiveInformation.ShowDialog();
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
                FileLogger.Log(this.Name, "btnReceive_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnReceive_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (FormatException ex)
            {
                FileLogger.Log(this.Name, "btnReceive_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnReceive_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnReceive_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnReceive_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnReceive_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnReceive_Click", exMessage);
            }
            #endregion
        }
        private void btnVAT16_Click(object sender, EventArgs e)
        {
            #region try
            try
            {
                if (Program.CheckLicence(dtpReceiveDate.Value) == true)
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
                if (dgvReceive.Rows.Count > 0)
                {
                    frmRptVAT16.txtItemNo.Text = dgvReceive.CurrentRow.Cells["ItemNo"].Value.ToString();
                    frmRptVAT16.txtProductName.Text = dgvReceive.CurrentRow.Cells["ItemName"].Value.ToString();
                    frmRptVAT16.dtpFromDate.Value = dtpReceiveDate.Value;
                    frmRptVAT16.dtpToDate.Value = dtpReceiveDate.Value;

                }

                frmRptVAT16.ShowDialog();
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
                        }
                    }
                }
                string strProductCode = txtProductCode.Text;
                if (!string.IsNullOrEmpty(strProductCode))
                {
                    ProductDAL productDal = new ProductDAL();
                    //IProduct productDal = OrdinaryVATDesktop.GetObject<ProductDAL, ProductRepo, IProduct>(OrdinaryVATDesktop.IsWCF);


                    DataTable priceData = productDal.AvgPriceNew(strProductCode, dtpReceiveDate.Value.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss"), null, null, false, true, true, true, connVM, Program.CurrentUserID);
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
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "cmbProduct_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "cmbProduct_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (FormatException ex)
            {
                FileLogger.Log(this.Name, "cmbProduct_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "cmbProduct_Leave", exMessage);
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
                FileLogger.Log(this.Name, "cmbProduct_Leave", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "cmbProduct_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "cmbProduct_Leave", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "cmbProduct_Leave", exMessage);
            }
            #endregion
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
        private void PirceCall()
        {
            ProductDAL productDal = new ProductDAL();
            string strProductCode = txtProductCode.Text;
            if (!string.IsNullOrEmpty(strProductCode))
            {
                if (Program.CheckingNumericTextBox(txtUnitCost, "Unit Cost") == true)
                {
                    txtUnitCost.Text = Program.FormatingNumeric(txtUnitCost.Text.Trim(), ReceivePlaceAmt).ToString();
                }
                else
                {
                    return;
                }
            }
        }


        #endregion

        #region Methods 06

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
                //txtQuantity.Focus();
                //cmbProductName.Items.Clear();
                //if (cmbProductName.SelectedIndex != -1)
                //{
                //if (cmbProductName.SelectedItem != null)
                //{
                var searchText = cmbProductName.Text.Trim().ToLower();
                if (!string.IsNullOrEmpty(searchText) && searchText != "select")
                {
                    if (rbtnProduct.Checked)
                    {
                        var prodByName = from prd in ProductsMini.ToList()
                                         where prd.ProductName.ToLower() == searchText.ToLower()
                                         select prd;
                        if (prodByName != null && prodByName.Any())
                        {
                            var products = prodByName.First();
                            txtProductName.Text = products.ProductName;
                            cmbProduct.Text = products.ProductCode;
                            txtProductCode.Text = products.ItemNo;
                            //txtUnitCost.Text = products.ReceivePrice.ToString();//"0,0.00");
                            txtUOM.Text = products.UOM;
                            cmbUom.Text = products.UOM;
                            txtHSCode.Text = products.HSCodeNo;
                            //txtQuantityInHand.Text = products.Stock.ToString();//"0,0.0000");
                            txtPCode.Text = products.ProductCode;
                        }
                    }
                }
                string strProductCode = txtProductCode.Text;
                if (!string.IsNullOrEmpty(strProductCode))
                {
                    //PirceCall();
                    //ProductDAL productDal = new ProductDAL();
                    //txtQuantityInHand.Text = productDal.AvgPriceNew(txtProductCode.Text, dtpReceiveDate.Value.ToString("yyyy-MMM-dd") +
                    //                                                   DateTime.Now.ToString(" HH:mm:ss"),
                    //                                                   null, null).Rows[0]["Quantity"].ToString();
                    ProductDAL productDal = new ProductDAL();
                    //IProduct productDal = OrdinaryVATDesktop.GetObject<ProductDAL, ProductRepo, IProduct>(OrdinaryVATDesktop.IsWCF);

                    DataTable priceData = productDal.AvgPriceNew(strProductCode, dtpReceiveDate.Value.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss"), null, null, false, true, true, true, connVM, Program.CurrentUserID);
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
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }
        private void label1_Click(object sender, EventArgs e)
        {
        }
        private void btnImport_Click(object sender, EventArgs e)
        {
            string fileName = "";
            if (chkSame.Checked == false)
            {
                BrowsFile();
            }
            fileName = Program.ImportFileName;
            if (string.IsNullOrEmpty(fileName))
            {
                MessageBox.Show("Please select the right file for import");
                return;
            }
            #region new process for bom import
            string[] extention = fileName.Split(".".ToCharArray());
            //string[] extention = fileNameM.Split(".".ToCharArray());
            string[] retResults = new string[4];
            if (extention[extention.Length - 1] == "txt")
            {
                retResults = ImportFromText();
            }
            else
            {
                retResults = ImportFromExcel();
            }
            //string[] retResults = Import();
            string result = retResults[0];
            string message = retResults[1];
            if (string.IsNullOrEmpty(result))
            {
                throw new ArgumentNullException("BomImport", "Unexpected error.");
            }
            else if (result == "Success" || result == "Fail")
            {
                MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            #endregion new process for bom import
        }
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
        private string[] ImportFromExcel()
        {
            #region Close1
            string[] sqlResults = new string[4];
            sqlResults[0] = "Fail";
            sqlResults[1] = "Fail";
            sqlResults[2] = "";
            sqlResults[3] = "";
            #endregion Close1
            #region try
            OleDbConnection theConnection = null;
            TransactionTypes();
            try
            {
                #region Load Excel
                CommonDAL commonDal = new CommonDAL();
                bool AutoItem = Convert.ToBoolean(commonDal.settingValue("AutoCode", "Item", connVM) == "Y" ? true : false);
                string fileName = Program.ImportFileName;
                if (string.IsNullOrEmpty(fileName))
                {
                    MessageBox.Show("Please select the right file for import");
                    return sqlResults;
                }
                string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;"
                                          + "Data Source=" + fileName + ";"
                                          + "Extended Properties=" + "\""
                                          + "Excel 12.0;HDR=YES;" + "\"";
                theConnection = new OleDbConnection(connectionString);
                theConnection.Open();
                OleDbDataAdapter adapterIssueM = new OleDbDataAdapter("SELECT * FROM [IssueM$]", theConnection);
                System.Data.DataTable dtIssueM = new System.Data.DataTable();
                adapterIssueM.Fill(dtIssueM);
                OleDbDataAdapter adapterDetail = new OleDbDataAdapter("SELECT * FROM [IssueD$]", theConnection);
                System.Data.DataTable dtIssueD = new System.Data.DataTable();
                adapterDetail.Fill(dtIssueD);
                theConnection.Close();
                #endregion Load Excel
                dtIssueM.Columns.Add("Transection_Type");
                dtIssueM.Columns.Add("Created_By");
                dtIssueM.Columns.Add("LastModified_By");
                foreach (DataRow row in dtIssueM.Rows)
                {
                    row["Transection_Type"] = transactionType;
                    row["Created_By"] = Program.CurrentUser;
                    row["LastModified_By"] = Program.CurrentUser;
                }
                SAVE_DOWORK_SUCCESS = false;
                //sqlResults = new string[4];
                IssueDAL issueDal = new IssueDAL();
                //IIssue issueDal = OrdinaryVATDesktop.GetObject<IssueDAL, IssueRepo, IIssue>(OrdinaryVATDesktop.IsWCF);

                sqlResults = issueDal.ImportData(dtIssueM, dtIssueD, 0, null, null, null, connVM);
                SAVE_DOWORK_SUCCESS = true;
            }
            #endregion try
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                if (error.Length > 1)
                {
                    MessageBox.Show(error[error.Length - 1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show(error[0], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "IssueImport", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "IssueImport", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (FormatException ex)
            {
                FileLogger.Log(this.Name, "IssueImport", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "IssueImport", exMessage);
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
                FileLogger.Log(this.Name, "IssueImport", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "IssueImport", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "IssueImport", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "IssueImport", exMessage);
            }
            return sqlResults;
            #endregion
        }
        private string[] ImportFromText()
        {
            #region Close1
            string[] sqlResults = new string[4];
            sqlResults[0] = "Fail";
            sqlResults[1] = "Fail";
            sqlResults[2] = "";
            sqlResults[3] = "";
            TransactionTypes();
            #endregion Close1
            string files = Program.ImportFileName;
            if (string.IsNullOrEmpty(files))
            {
                MessageBox.Show("Please select the right file for import");
                return sqlResults;
            }
            DataTable dtIssueM = new DataTable();
            DataTable dtIssueD = new DataTable();
            #region Master table
            dtIssueM.Columns.Add("Identifier");
            dtIssueM.Columns.Add("ID");
            dtIssueM.Columns.Add("Issue_DateTime");
            dtIssueM.Columns.Add("Reference_No");
            dtIssueM.Columns.Add("Comments");
            dtIssueM.Columns.Add("Return_Id");
            dtIssueM.Columns.Add("Post");
            dtIssueM.Columns.Add("Transection_Type").DefaultValue = transactionType;
            dtIssueM.Columns.Add("Created_By").DefaultValue = Program.CurrentUser;
            dtIssueM.Columns.Add("LastModified_By").DefaultValue = Program.CurrentUser;
            #endregion Master table
            #region Details table
            dtIssueD.Columns.Add("Identifier");
            dtIssueD.Columns.Add("ID");
            dtIssueD.Columns.Add("Item_Code");
            dtIssueD.Columns.Add("Item_Name");
            dtIssueD.Columns.Add("Quantity");
            dtIssueD.Columns.Add("UOM");
            #endregion Details table
            //string fileNameM = fileNames[fileNames.Length - 1].ToString();
            //string fileNameD = fileNames[fileNames.Length - 2].ToString();
            //StreamReader sr = new StreamReader(fileName);
            string[] fileNames = files.Split(";".ToCharArray());
            StreamReader sr = new StreamReader(fileNames[0]);
            try
            {
                #region Load Master Text file
                foreach (var fileName in fileNames)
                {
                    sr = new StreamReader(fileName);
                    string masterData = sr.ReadToEnd();
                    string[] masterRows = masterData.Split("\r".ToCharArray());
                    string delimeter = "|";
                    string recordType = masterRows[1].Split(delimeter.ToCharArray())[0].Replace("\n", "").ToUpper();
                    if (recordType == "M")
                    {
                        foreach (string mRow in masterRows)
                        {
                            string[] mItems = mRow.Split(delimeter.ToCharArray());
                            if (mItems[0].Replace("\n", "").ToUpper() == "M")
                            {
                                dtIssueM.Rows.Add(mItems);
                            }
                        }
                    }
                    else
                    {
                        foreach (string dRow in masterRows)
                        {
                            string[] dItems = dRow.Split(delimeter.ToCharArray());
                            if (dItems[0].Replace("\n", "").ToUpper() == "D")
                            {
                                dtIssueD.Rows.Add(dItems);
                            }
                        }
                    }
                }
                if (sr != null)
                {
                    sr.Close();
                }
                #endregion Load Master Text file
                #region Load Text file
                //string allData = sr.ReadToEnd();
                //string[] rows = allData.Split("\r".ToCharArray());
                //string delimeter = "|";
                //foreach (string r in rows)
                //{
                //    string[] items = r.Split(delimeter.ToCharArray());
                //    if (items[0].Replace("\n", "").ToUpper() == "M")
                //    {
                //        dtIssueM.Rows.Add(items);
                //    }
                //    else if (items[0].Replace("\n", "").ToUpper() == "D")
                //    {
                //        dtIssueD.Rows.Add(items);
                //    }
                //}
                //if (sr != null)
                //{
                //    sr.Close();
                //}
                #endregion Load Text file
                SAVE_DOWORK_SUCCESS = false;
                IssueDAL issueDal = new IssueDAL();
                //IIssue issueDal = OrdinaryVATDesktop.GetObject<IssueDAL, IssueRepo, IIssue>(OrdinaryVATDesktop.IsWCF);

                sqlResults = issueDal.ImportData(dtIssueM, dtIssueD, 0, null, null, null, connVM);
                SAVE_DOWORK_SUCCESS = true;
            }
            #region catch & finally
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                if (error.Length > 1)
                {
                    MessageBox.Show(error[error.Length - 1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show(error[0], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
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
                FileLogger.Log(this.Name, "IssueImport", exMessage);
            }
            finally
            {
                if (sr != null)
                {
                    sr.Close();
                }
            }
            #endregion catch & finally
            return sqlResults;
        }
        private void bgwP_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                #region Statement
                ReceiveDetailResult = new DataTable();
                IssueDAL issueDal = new IssueDAL();
                //IIssue issueDal = OrdinaryVATDesktop.GetObject<IssueDAL, IssueRepo, IIssue>(OrdinaryVATDesktop.IsWCF);

                ReceiveDetailResult = issueDal.SearchIssueDetailDTNew(ReceiveDetailData, Program.DatabaseName, connVM); // Change 04
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
        private void bgwP_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                #region Statement
                //string decriptedPurchaseDetailData = Converter.DESDecrypt(PassPhrase, EnKey, IssueDetailResult);
                //string[] IssueDetailLines = decriptedPurchaseDetailData.Split(LineDelimeter.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                dgvReceive.Rows.Clear();
                int j = 0;
                foreach (DataRow item in ReceiveDetailResult.Rows)
                {
                    DataGridViewRow NewRow = new DataGridViewRow();
                    dgvReceive.Rows.Add(NewRow);
                    dgvReceive.Rows[j].Cells["LineNo"].Value = item["IssueLineNo"].ToString();// IssueDetailFields[1].ToString();
                    dgvReceive.Rows[j].Cells["ItemNo"].Value = item["ItemNo"].ToString();// IssueDetailFields[2].ToString();
                    dgvReceive.Rows[j].Cells["Quantity"].Value = item["Quantity"].ToString();//Convert.ToDecimal(IssueDetailFields[3].ToString()).ToString();//"0,0.0000");
                    dgvReceive.Rows[j].Cells["UnitPrice"].Value = item["CostPrice"].ToString();//Convert.ToDecimal(IssueDetailFields[4].ToString()).ToString();//"0,0.00");
                    dgvReceive.Rows[j].Cells["UOM"].Value = item["UOM"].ToString();// IssueDetailFields[6].ToString();
                    dgvReceive.Rows[j].Cells["SubTotal"].Value = item["SubTotal"].ToString();//Convert.ToDecimal(IssueDetailFields[9].ToString()).ToString();//"0,0.00");
                    dgvReceive.Rows[j].Cells["Comments"].Value = item["Comments"].ToString();// IssueDetailFields[10].ToString();
                    dgvReceive.Rows[j].Cells["ItemName"].Value = item["ProductName"].ToString();// IssueDetailFields[11].ToString();
                    dgvReceive.Rows[j].Cells["PCode"].Value = item["ProductCode"].ToString();// IssueDetailFields[15].ToString();
                    dgvReceive.Rows[j].Cells["Status"].Value = "Old";
                    dgvReceive.Rows[j].Cells["Previous"].Value = item["Quantity"].ToString();//Convert.ToDecimal(IssueDetailFields[3].ToString()).ToString();//"0,0.0000"); //Quantity
                    dgvReceive.Rows[j].Cells["Stock"].Value = item["Stock"].ToString();//Convert.ToDecimal(IssueDetailFields[12].ToString()).ToString();//"0,0.0000");
                    dgvReceive.Rows[j].Cells["Change"].Value = 0;
                    dgvReceive.Rows[j].Cells["UOMc"].Value = item["UOMc"].ToString();
                    dgvReceive.Rows[j].Cells["UOMn"].Value = item["UOMn"].ToString();
                    dgvReceive.Rows[j].Cells["UOMQty"].Value = item["UOMQty"].ToString();
                    dgvReceive.Rows[j].Cells["UOMPrice"].Value = item["UOMPrice"].ToString();
                    j = j + 1;
                }  //End For
                btnSave.Text = "&Save";
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
                this.btnSearchReceiveNo.Enabled = true;
                this.progressBar1.Visible = false;
            }
        }

        #endregion

        #region Methods 07

        private void dgvIssue_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }
        private void cmbTransferTo_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }
        private void cmbTransferFrom_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.F9))
            {
                // TransferToBranchLoad();
            }
        }
        private void cmbTransferTo_Leave(object sender, EventArgs e)
        {
            try
            {
                #region Statement
                txtBranchDBName.Text = "";
                //////var searchText = cmbTransferFrom.Text.Trim().ToLower();
                var searchText = txtTransferFrom.Text.Trim().ToLower();
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

        private void btnSearchReceiveFrom_Click(object sender, EventArgs e)
        {
            #region try
            try
            {
                Program.fromOpen = "Me";
                TransactionTypes();
                if (transactionType == "62In")
                    transactionType = "62Out";
                else if (transactionType == "61In")
                    transactionType = "61Out";

                string TransferIssueNo = FormTransferReceiveFromSearch.SelectOne(transactionType);
                TransferDAL transferDal = new TransferDAL();
                if (!string.IsNullOrWhiteSpace(TransferIssueNo))
                {
                    //var transferIssueDal = new TransferIssueDAL();
                    ITransferIssue transferIssueDal = OrdinaryVATDesktop.GetObject<TransferIssueDAL, TransferIssueRepo, ITransferIssue>(OrdinaryVATDesktop.IsWCF);

                    var vm = new TransferIssueVM();


                    vm.TransferIssueNo = TransferIssueNo;
                    vm.TransactionType = "";// transactionType;
                    vm.IssueDateFrom = "";
                    vm.IssueDateTo = "";
                    vm.Post = "";
                    vm.ReferenceNo = "";



                    DataTable transferResult = transferIssueDal.SearchTransferIssue(vm, null, null, connVM); //transferDal.SearchTransfer(vm);  // Change 04
                    //DataTable transferResult = transferDal.SearchTransfer(invoiceNo, "", "", transactionType, "");  // Change 04
                    if (transferResult.Rows.Count > 0)
                    {
                        dtpReceiveDate.Value = DateTime.Now;
                        txtReceiveNo.Text = "";// transferResult.Rows[0]["TransferNo"].ToString();

                        txtTotalAmount.Text = "";// Convert.ToDecimal(transferResult.Rows[0]["TotalAmount"].ToString()).ToString();//"0,0.00");
                        txtSerialNo.Text = "";// transferResult.Rows[0]["SerialNo"].ToString();
                        txtReferenceNo.Text = "";// transferResult.Rows[0]["ReferenceNo"].ToString();
                        txtComments.Text = "";// transferResult.Rows[0]["Comments"].ToString();
                        txtBranchDBName.Text = transferResult.Rows[0]["BranchId"].ToString();
                        //////cmbTransferFrom.Text = transferResult.Rows[0]["BranchNameFrom"].ToString();
                        txtTransferFrom.Text = transferResult.Rows[0]["BranchNameFrom"].ToString();
                        //////txtTransferNo.Text = transferResult.Rows[0]["TransferNo"].ToString();
                        txtTransferFromNo.Text = transferResult.Rows[0]["TransferIssueNo"].ToString();
                        //////txtTransfer.Text=invoiceNo

                        IsPost = Convert.ToString(transferResult.Rows[0]["Post"].ToString()) == "Y" ? true : false;
                        var isTransfer = Convert.ToString(transferResult.Rows[0]["IsTransfer"].ToString()) == "Y" ? true : false;
                        btnAdd.Enabled = false;
                        btnChange.Enabled = false;
                        btnRemove.Enabled = false;
                        ////cmbTransferFrom.Enabled = false;

                        btnSave.Enabled = true;
                        btnUpdate.Enabled = true;
                        btnPost.Enabled = true;
                        if (isTransfer)
                        {
                            btnSave.Enabled = false;
                            btnUpdate.Enabled = false;
                            btnPost.Enabled = false;

                        }

                    }
                    ////string ReceiveDetailData;
                    ReceiveDetailData = txtReceiveNo.Text == "" ? "" : txtReceiveNo.Text.Trim();
                    var tt = transactionType;//
                    #region Statement
                    transferDetailResult = new DataTable();
                    if (!string.IsNullOrEmpty(TransferIssueNo))
                    {
                        transferDetailResult = transferIssueDal.SearchTransferDetail(TransferIssueNo, null, null, connVM);
                    }// Change 04
                    #region DataGridView
                    dgvReceive.Rows.Clear();
                    int j = 0;
                    foreach (DataRow item in transferDetailResult.Rows)
                    {
                        DataGridViewRow NewRow = new DataGridViewRow();
                        dgvReceive.Rows.Add(NewRow);
                        // dgvReceive.Rows[j].Cells["LineNo"].Value = item["TransferLineNo"].ToString();        // ReceiveDetailFields[1].ToString();
                        dgvReceive.Rows[j].Cells["ItemNo"].Value = item["ItemNo"].ToString();             // ReceiveDetailFields[2].ToString();
                        dgvReceive.Rows[j].Cells["Quantity"].Value = item["Quantity"].ToString();         //Convert.ToDecimal(ReceiveDetailFields[3].ToString()).ToString();//"0,0.0000");
                        dgvReceive.Rows[j].Cells["UnitPrice"].Value = item["CostPrice"].ToString();       //Convert.ToDecimal(ReceiveDetailFields[4].ToString()).ToString();//"0,0.00");
                        dgvReceive.Rows[j].Cells["UOM"].Value = item["UOM"].ToString();                   // ReceiveDetailFields[6].ToString();
                        dgvReceive.Rows[j].Cells["SubTotal"].Value = item["SubTotal"].ToString();         //Convert.ToDecimal(ReceiveDetailFields[9].ToString()).ToString();//"0,0.00");
                        dgvReceive.Rows[j].Cells["Comments"].Value = item["Comments"].ToString();         // ReceiveDetailFields[10].ToString();
                        dgvReceive.Rows[j].Cells["ItemName"].Value = item["ProductName"].ToString();      // ReceiveDetailFields[11].ToString();
                        dgvReceive.Rows[j].Cells["PCode"].Value = item["ProductCode"].ToString();         // ReceiveDetailFields[15].ToString();
                        dgvReceive.Rows[j].Cells["Status"].Value = "Old";
                        dgvReceive.Rows[j].Cells["Previous"].Value = item["Quantity"].ToString();         //Convert.ToDecimal(ReceiveDetailFields[3].ToString()).ToString();//"0,0.0000"); //Quantity
                        dgvReceive.Rows[j].Cells["Stock"].Value = item["Stock"].ToString();               //Convert.ToDecimal(ReceiveDetailFields[12].ToString()).ToString();//"0,0.0000");
                        dgvReceive.Rows[j].Cells["Change"].Value = 0;
                        dgvReceive.Rows[j].Cells["UOMc"].Value = item["UOMc"].ToString();
                        dgvReceive.Rows[j].Cells["UOMn"].Value = item["UOMn"].ToString();
                        dgvReceive.Rows[j].Cells["UOMQty"].Value = item["UOMQty"].ToString();
                        dgvReceive.Rows[j].Cells["UOMPrice"].Value = item["UOMPrice"].ToString();

                        dgvReceive.Rows[j].Cells["VATRate"].Value = item["VATRate"].ToString();
                        dgvReceive.Rows[j].Cells["VATAmount"].Value = item["VATAmount"].ToString();
                        dgvReceive.Rows[j].Cells["SDRate"].Value = item["SDRate"].ToString();
                        dgvReceive.Rows[j].Cells["SDAmount"].Value = item["SDAmount"].ToString();
                        dgvReceive.Rows[j].Cells["Weight"].Value = item["Weight"].ToString();

                        j = j + 1;
                    }  //End For
                    #endregion DataGridView
                    Rowcalculate();
                    btnSave.Text = "&Add";
                    IsUpdate = false;
                    ChangeData = false;
                    #endregion
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
                FileLogger.Log(this.Name, "btnSearchReceiveFrom_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnSearchReceiveFrom_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (FormatException ex)
            {
                FileLogger.Log(this.Name, "btnSearchReceiveFrom_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnSearchReceiveFrom_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnSearchReceiveFrom_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnSearchReceiveFrom_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnSearchReceiveFrom_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnSearchReceiveFrom_Click", exMessage);
            }
            #endregion
            finally
            {
                ChangeData = false;
                this.btnSearchReceiveNo.Enabled = true;
                this.progressBar1.Visible = false;
            }
        }

        private void btn62_Click(object sender, EventArgs e)
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
                FormRptVAT17 frmRptVAT17 = new FormRptVAT17();

                //mdi.RollDetailsInfo("8301");
                //if (Program.Access != "Y")
                //{
                //    MessageBox.Show("You do not have to access this form", frmRptVAT17.Text, MessageBoxButtons.OK,
                //                    MessageBoxIcon.Information);
                //    return;
                //}
                if (dgvReceive.Rows.Count > 0)
                {
                    frmRptVAT17.txtItemNo.Text = dgvReceive.CurrentRow.Cells["ItemNo"].Value.ToString();
                    frmRptVAT17.txtProductName.Text = dgvReceive.CurrentRow.Cells["ItemName"].Value.ToString();
                    frmRptVAT17.dtpFromDate.Value = dtpReceiveDate.Value;
                    frmRptVAT17.dtpToDate.Value = dtpReceiveDate.Value;

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

        private void label15_Click(object sender, EventArgs e)
        {

        }

        private void btnImportRece_Click(object sender, EventArgs e)
        {
            try
            {
                TransactionTypes();

                var import = new FormMasterImport { transactionType = transactionType, preSelectTable = "TransferReceive" };


                import.ShowDialog();
            }
            catch (Exception exception)
            {
                FileLogger.Log("Transfer Receive", "btnImportRec", exception.Message);
            }
        }

        private void txtTotalAmount_Leave(object sender, EventArgs e)
        {
            Program.FormatTextBox(txtTotalAmount, "TotalAmount");
        }

        private void dtpReceiveDate_Leave(object sender, EventArgs e)
        {
            dtpReceiveDate.Value = Program.ParseDate(dtpReceiveDate);
        }

        #endregion

        #region Transfer Receive Navigation

        private void btnFirst_Click(object sender, EventArgs e)
        {
            TransferReceiveNavigation("First");

        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            TransferReceiveNavigation("Previous");
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            TransferReceiveNavigation("Next");
        }

        private void btnLast_Click(object sender, EventArgs e)
        {
            TransferReceiveNavigation("Last");
        }

        private void TransferReceiveNavigation(string ButtonName)
        {
            try
            {
                TransferReceiveDAL _TransferReceiveDAL = new TransferReceiveDAL();
                NavigationVM vm = new NavigationVM();
                vm.ButtonName = ButtonName;

                vm.FiscalYear = Convert.ToInt32(txtFiscalYear.Text);
                vm.BranchId = Convert.ToInt32(SearchBranchId);
                vm.TransactionType = transactionType;
                vm.Id = Convert.ToInt32(txtId.Text);
                vm.InvoiceNo = txtReceiveNo.Text;


                if (vm.BranchId == 0)
                {
                    vm.BranchId = Program.BranchId;
                }

                vm = _TransferReceiveDAL.TransferReceive_Navigation(vm, null, null, connVM);

                txtReceiveNo.Text = vm.InvoiceNo;

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
                FileLogger.Log(this.Name, "TransferReceiveNavigation", exMessage);
            }
            #endregion Catch

        }

        #endregion





    }
}
