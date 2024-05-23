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

namespace VATClient
{
    public partial class FormTransferRaw : Form
    {
        #region Constructors

        public FormTransferRaw()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();
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

        List<ProductMiniDTO> ProductsMini = new List<ProductMiniDTO>();
        List<ProductMiniDTO> RawsMini = new List<ProductMiniDTO>();
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        private string[] sqlResults;
        private bool SAVE_DOWORK_SUCCESS = false;
        private bool DELETE_DOWORK_SUCCESS = false;
        private bool UPDATE_DOWORK_SUCCESS = false;
        private bool POST_DOWORK_SUCCESS = false;

        private const string LineDelimeter = DBConstant.LineDelimeter;
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
       
        private string NextID = string.Empty;
        private bool ChangeData = false;
        private DataTable ProductResultDs;
        private bool IsUpdate = false;
        private bool Post = false;
        private bool IsPost = false;
        private string CategoryId { get; set; }

        private string UOMIdParam = "";
        private string UOMFromParam = "";
        private string UOMToParam = "";
        private string ActiveStatusUOMParam = "";
        private DataTable uomResult;
        List<UomDTO> UOMs = new List<UomDTO>();
        private int IssuePlaceQty;
        private int IssuePlaceAmt;

        TransferRawDAL transRawDal = new TransferRawDAL();
        private decimal RawStock;
        private decimal RemainQty;

        #region Global Variables For BackGroundWorker

        private string IssueHeaderData = string.Empty;

        private string IssueDetailData = string.Empty;

        private string IssueResult = string.Empty;

        private string IssueResultPost = string.Empty;

        private DataTable IssueDetailResult;

        private string ProductData = string.Empty;

        private string varItemNo, varCategoryID, varIsRaw, varHSCodeNo, varActiveStatus, varTrading, varNonStock, varProductCode;

        private TransferRawMasterVM transRawMasterVM;

        private List<TransferRawDetailVM> transRawDetailVMs = new List<TransferRawDetailVM>();

        private DataTable ProductTypeResult;
        private bool Edit = false;
        private bool Add = false;

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
                    NextID = txtTransferId.Text.Trim();
                }

                
                if (dgvIssue.RowCount <= 0)
                {
                    MessageBox.Show("Please insert Details information for transaction", this.Text, MessageBoxButtons.OK,
                                     MessageBoxIcon.Information);
                    return;
                }

                transRawMasterVM = new TransferRawMasterVM();

                transRawMasterVM.TransferId = NextID.ToString();
                transRawMasterVM.TransferDateTime = dtpTransferDate.Value.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss");
                transRawMasterVM.TransferedQty = Convert.ToDecimal(txtTotalQty.Text.Trim());
                transRawMasterVM.TransferedAmt = Convert.ToDecimal(txtTotalAmount.Text.Trim());
                transRawMasterVM.TransFromItemNo = txtItemNo1.Text.Trim();
                transRawMasterVM.UOM = txtUOM1.Text.Trim();
                transRawMasterVM.Quantity = Convert.ToDecimal(txtQuantity1.Text.Trim());
                transRawMasterVM.CostPrice = Convert.ToDecimal(txtUnitCost1.Text.Trim());
                
                transRawMasterVM.CreatedBy = Program.CurrentUser;
                transRawMasterVM.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                transRawMasterVM.LastModifiedBy = Program.CurrentUser;
                transRawMasterVM.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                
                transRawMasterVM.TransactionType = "Transfer Raw";
                transRawMasterVM.Post = "N";
               

                transRawDetailVMs = new List<TransferRawDetailVM>();
                for (int i = 0; i < dgvIssue.RowCount; i++)
                {
                    TransferRawDetailVM detail = new TransferRawDetailVM();

                    detail.TransferIdD = NextID.ToString();
                    detail.TransLineNo = dgvIssue.Rows[i].Cells["LineNo"].Value.ToString();
                    detail.ItemNo = dgvIssue.Rows[i].Cells["ItemNo"].Value.ToString();
                    detail.Quantity = Convert.ToDecimal(dgvIssue.Rows[i].Cells["Quantity"].Value.ToString());
                    detail.CostPrice = Convert.ToDecimal(dgvIssue.Rows[i].Cells["UnitPrice"].Value.ToString());
                    detail.UOM = dgvIssue.Rows[i].Cells["UOM"].Value.ToString();
                    detail.SubTotal = Convert.ToDecimal(dgvIssue.Rows[i].Cells["SubTotal"].Value.ToString());
                    detail.UOMQty = Convert.ToDecimal(dgvIssue.Rows[i].Cells["UOMQty"].Value.ToString());
                    detail.UOMn = dgvIssue.Rows[i].Cells["UOMn"].Value.ToString();
                    detail.UOMc = Convert.ToDecimal(dgvIssue.Rows[i].Cells["UOMc"].Value.ToString());
                    detail.UOMPrice = Convert.ToDecimal(dgvIssue.Rows[i].Cells["UOMPrice"].Value.ToString());

                    transRawDetailVMs.Add(detail);

                }// End For


                if (transRawDetailVMs.Count() <= 0)
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
                
                sqlResults = transRawDal.TransferRawInsert(transRawMasterVM, transRawDetailVMs,connVM);
                SAVE_DOWORK_SUCCESS = true;

                #endregion

            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                if (error.Count()>1)
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
                            txtTransferId.Text = sqlResults[2].ToString();
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
                    NextID = txtTransferId.Text.Trim();
                }
               
                if (dgvIssue.RowCount <= 0)
                {
                    MessageBox.Show("Please insert Details information for transaction", this.Text, MessageBoxButtons.OK,
                                     MessageBoxIcon.Information);
                    return;
                }

                transRawMasterVM = new TransferRawMasterVM();

                transRawMasterVM.TransferId = NextID.ToString();
                transRawMasterVM.TransferDateTime = dtpTransferDate.Value.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss");
                transRawMasterVM.TransferedQty = Convert.ToDecimal(txtTotalQty.Text.Trim());
                transRawMasterVM.TransferedAmt = Convert.ToDecimal(txtTotalAmount.Text.Trim());
                transRawMasterVM.TransFromItemNo = txtItemNo1.Text.Trim();
                transRawMasterVM.UOM = txtUOM1.Text.Trim();
                transRawMasterVM.Quantity = Convert.ToDecimal(txtQuantity1.Text.Trim());
                transRawMasterVM.CostPrice = Convert.ToDecimal(txtUnitCost1.Text.Trim());

                transRawMasterVM.CreatedBy = Program.CurrentUser;
                transRawMasterVM.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                transRawMasterVM.LastModifiedBy = Program.CurrentUser;
                transRawMasterVM.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");

                transRawMasterVM.TransactionType = "Transfer Raw";
                transRawMasterVM.Post = "N";

                transRawDetailVMs = new List<TransferRawDetailVM>();
                for (int i = 0; i < dgvIssue.RowCount; i++)
                {
                    TransferRawDetailVM detail = new TransferRawDetailVM();

                    detail.TransferIdD = NextID.ToString();
                    detail.TransLineNo = dgvIssue.Rows[i].Cells["LineNo"].Value.ToString();
                    detail.ItemNo = dgvIssue.Rows[i].Cells["ItemNo"].Value.ToString();
                    detail.Quantity = Convert.ToDecimal(dgvIssue.Rows[i].Cells["Quantity"].Value.ToString());
                    detail.CostPrice = Convert.ToDecimal(dgvIssue.Rows[i].Cells["UnitPrice"].Value.ToString());
                    detail.UOM = dgvIssue.Rows[i].Cells["UOM"].Value.ToString();
                    detail.SubTotal = Convert.ToDecimal(dgvIssue.Rows[i].Cells["SubTotal"].Value.ToString());
                    detail.UOMQty = Convert.ToDecimal(dgvIssue.Rows[i].Cells["UOMQty"].Value.ToString());
                    detail.UOMn = dgvIssue.Rows[i].Cells["UOMn"].Value.ToString();
                    detail.UOMc = Convert.ToDecimal(dgvIssue.Rows[i].Cells["UOMc"].Value.ToString());
                    detail.UOMPrice = Convert.ToDecimal(dgvIssue.Rows[i].Cells["UOMPrice"].Value.ToString());
                    detail.TransFromItemNo = dgvIssue.Rows[i].Cells["TransferedItemNo"].Value.ToString();

                    transRawDetailVMs.Add(detail);

                }// End For// End For



                if (transRawDetailVMs.Count() <= 0)
                {
                    MessageBox.Show("Please insert Details information for transaction", this.Text, MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                    return;
                }
                this.btnUpdate.Enabled = false;
                this.progressBar1.Visible = true;
                bgwUpdate.RunWorkerAsync();
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
        }
        private void bgwUpdate_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                #region Statement


                UPDATE_DOWORK_SUCCESS = false;
                sqlResults = new string[4];
                sqlResults = transRawDal.TransferRawUpdate(transRawMasterVM, transRawDetailVMs,connVM);
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
                            ChangeData = false;
                        }

                        if (result == "Success")
                        {
                            txtTransferId.Text = sqlResults[2].ToString();
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
                //else if (IsPost == true)
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
                    NextID = txtTransferId.Text.Trim();
                }

               
                if (dgvIssue.RowCount <= 0)
                {
                    MessageBox.Show("Please insert Details information for transaction", this.Text, MessageBoxButtons.OK,
                                     MessageBoxIcon.Information);
                    return;
                }


                transRawMasterVM = new TransferRawMasterVM();

                transRawMasterVM.TransferId = NextID.ToString();
                transRawMasterVM.TransferDateTime = dtpTransferDate.Value.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss");
                transRawMasterVM.LastModifiedBy = Program.CurrentUser;
                transRawMasterVM.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                transRawMasterVM.Post = "Y";

                transRawDetailVMs = new List<TransferRawDetailVM>();


                for (int i = 0; i < dgvIssue.RowCount; i++)
                {
                    TransferRawDetailVM detail = new TransferRawDetailVM();

                    detail.TransferIdD = NextID.ToString();
                    detail.ItemNo = dgvIssue.Rows[i].Cells["ItemNo"].Value.ToString();
                    
                    transRawDetailVMs.Add(detail);
                }


                if (transRawDetailVMs.Count() <= 0)
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
                sqlResults = transRawDal.TransferRawPost(transRawMasterVM, transRawDetailVMs,connVM);
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
                        }

                        if (result == "Success")
                        {
                            txtTransferId.Text = sqlResults[2].ToString();
                            IsPost = Convert.ToString(sqlResults[3].ToString()) == "Y" ? true : false;
                            for (int i = 0; i < dgvIssue.RowCount; i++)
                            {
                                dgvIssue["Status", dgvIssue.RowCount - 1].Value = "Old";
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

        private void selectLastRow()
        {
            #region try
            try
            {
                if (dgvIssue.Rows.Count > 0)
                {
                    dgvIssue.Rows[dgvIssue.Rows.Count - 1].Selected = true;
                    dgvIssue.CurrentCell = dgvIssue.Rows[dgvIssue.Rows.Count - 1].Cells["LineNo"];
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

        private void btnAdd_Click(object sender, EventArgs e)
        {
            #region try
            try
            {
                ChangeData = true;
                
                if (txtItemNo.Text == "")
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
                if (cmbUom.SelectedIndex == -1)
                {
                    throw new ArgumentNullException(this.Text, "Please select pack size");
                }

                UomsValue();  
        
                if (Convert.ToDecimal(txtQuantity.Text) <= 0)
                {
                    MessageBox.Show("Please input Quantity", this.Text);
                    txtQuantity.Focus();
                    return;
                }

                #region Check Stock
                //if (Convert.ToDecimal(txtQuantity.Text) > Convert.ToDecimal(txtQuantity1.Text))
                //{
                //    MessageBox.Show("Stock Not available");
                //    txtQuantity.Focus();
                //    return;
                //}
                bool isStoc = CheckStock();
                if (isStoc == false)
                {
                    MessageBox.Show("Stock Not available");
                    txtQuantity.Focus();
                    return;
                }
                #endregion Check Stock

                for (int i = 0; i < dgvIssue.RowCount; i++)
                {
                    if (dgvIssue.Rows[i].Cells["ItemNo"].Value.ToString() == txtItemNo.Text &&
                        dgvIssue.Rows[i].Cells["TransferedItemNo"].Value.ToString() == txtItemNo1.Text)
                    {
                        MessageBox.Show("Same Product already exist.", this.Text);

                        return;
                    }
                }

                
                


                DataGridViewRow NewRow = new DataGridViewRow();
                dgvIssue.Rows.Add(NewRow);

                #region Master
                dgvIssue["TransferedItemNo", dgvIssue.RowCount - 1].Value = txtItemNo1.Text.Trim();
                dgvIssue["TransferedName", dgvIssue.RowCount - 1].Value = txtProductName1.Text.Trim();
                dgvIssue["TransferedCode", dgvIssue.RowCount - 1].Value = txtPCode1.Text.Trim();
                dgvIssue["TransferedUOM", dgvIssue.RowCount - 1].Value = txtUOM1.Text.Trim();
                if (Program.CheckingNumericTextBox(txtQuantity1, "txtQuantity1") == true)
                    txtQuantity1.Text = Program.FormatingNumeric(txtQuantity1.Text.Trim(), IssuePlaceQty).ToString();
                if (Program.CheckingNumericTextBox(txtUnitCost1, "txtUnitCost1") == true)
                    txtUnitCost1.Text = Program.FormatingNumeric(txtUnitCost1.Text.Trim(), IssuePlaceAmt).ToString();

                dgvIssue["TransferedQty", dgvIssue.RowCount - 1].Value = Convert.ToDecimal(txtQuantity.Text.Trim()).ToString();


                dgvIssue["TransferedPrice", dgvIssue.RowCount - 1].Value =
                    Convert.ToDecimal(Convert.ToDecimal(txtUnitCost1.Text.Trim()) * Convert.ToDecimal(txtUomConv1.Text.Trim())).ToString();
               
                #endregion Master
                dgvIssue["ItemNo", dgvIssue.RowCount - 1].Value = txtItemNo.Text.Trim();
                dgvIssue["ItemName", dgvIssue.RowCount - 1].Value = txtProductName.Text.Trim();
                dgvIssue["PCode", dgvIssue.RowCount - 1].Value = txtPCode.Text.Trim();
                string strUom = cmbUom.SelectedItem.ToString();
                dgvIssue["UOM", dgvIssue.RowCount - 1].Value = strUom.Trim();

                if (Program.CheckingNumericTextBox(txtQuantity, "txtQuantity") == true)
                    txtQuantity.Text = Program.FormatingNumeric(txtQuantity.Text.Trim(), IssuePlaceQty).ToString();
                if (Program.CheckingNumericTextBox(txtUnitCost, "txtUnitCost") == true)
                    txtUnitCost.Text = Program.FormatingNumeric(txtUnitCost.Text.Trim(), IssuePlaceAmt).ToString();

                //if (Program.CheckingNumericTextBox(txtNBRPrice, "txtNBRPrice") == true)
                //    txtNBRPrice.Text = Program.FormatingNumeric(txtNBRPrice.Text.Trim(), IssuePlaceAmt).ToString();

                dgvIssue["Quantity", dgvIssue.RowCount - 1].Value = Convert.ToDecimal(txtQuantity.Text.Trim()).ToString();


                dgvIssue["UnitPrice", dgvIssue.RowCount - 1].Value =
                    Convert.ToDecimal(Convert.ToDecimal(txtUnitCost.Text.Trim()) * Convert.ToDecimal(txtUomConv.Text.Trim())).ToString();
                dgvIssue["UOMPrice", dgvIssue.RowCount - 1].Value = Convert.ToDecimal(Convert.ToDecimal(txtUnitCost.Text.Trim())).ToString();
                //dgvIssue["Comments", dgvIssue.RowCount - 1].Value = "NA";// txtCommentsDetail.Text.Trim();
                //dgvIssue["NBRPrice", dgvIssue.RowCount - 1].Value = Convert.ToDecimal(txtNBRPrice.Text.Trim()).ToString();
                dgvIssue["Status", dgvIssue.RowCount - 1].Value = "New";
                //dgvIssue["Stock", dgvIssue.RowCount - 1].Value = Convert.ToDecimal(txtQuantityInHand.Text.Trim()).ToString();
                dgvIssue["Previous", dgvIssue.RowCount - 1].Value = 0;// txtQuantity.Text.Trim();
                dgvIssue["Change", dgvIssue.RowCount - 1].Value = 0;

                dgvIssue["UOMc", dgvIssue.RowCount - 1].Value =
                    Convert.ToDecimal(txtUomConv.Text.Trim());// txtUOM.Text.Trim();
                dgvIssue["UOMn", dgvIssue.RowCount - 1].Value =
                    txtUOM.Text.Trim();// txtUOM.Text.Trim();
                dgvIssue["UOMQty", dgvIssue.RowCount - 1].Value =
                    Convert.ToDecimal(Convert.ToDecimal(txtQuantity.Text.Trim()) * Convert.ToDecimal(txtUomConv.Text.Trim()));


                Rowcalculate();

                #region Stock

                

                //decimal totalQty = RawStock;
                //decimal transferedQty = Convert.ToDecimal(txtQuantity.Text);
                //RemainQty = totalQty - transferedQty;
                txtQuantity1.Text = RemainQty.ToString();
                if (Program.CheckingNumericTextBox(txtQuantity1, "txtQuantity1") == true)
                    txtQuantity1.Text = Program.FormatingNumeric(txtQuantity1.Text.Trim(), IssuePlaceQty).ToString();
                #endregion
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
                cmbProductName1.Enabled = false;
                cmbProduct1.Enabled = false;
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
                FileLogger.Log(this.Name, "btnAdd_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnAdd_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnAdd_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnAdd_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnAdd_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnAdd_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnAdd_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnAdd_Click", exMessage);
            }
            #endregion

        }

        private void AllClear()
        {
            txtItemNo.Text = "";
            txtProductName.Text = "";
            txtHSCode.Text = "";
            //txtUnitCost.Text = "0.00";
            txtQuantity.Text = "";
            txtUOM.Text = "";
            txtQuantityInHand.Text = "0.00";
            cmbProduct.Text = "Select";
            cmbProductName.Text = "Select";
            cmbUom.Text = "";


        }
        private bool CheckStock()
        {
            bool IsStockAvailable = true;
            decimal transferredQty = 0;
            decimal currentQty = 0;
            for (int i = 0; i < dgvIssue.RowCount; i++)
            {
                transferredQty = transferredQty + Convert.ToDecimal(dgvIssue["Quantity", i].Value) * Convert.ToDecimal(dgvIssue["UOMc", i].Value);

            }
            currentQty=(Convert.ToDecimal(txtQuantity.Text)*Convert.ToDecimal(txtUomConv.Text));
            //transferredQty = transferredQty + currentQty;

            RemainQty = RawStock - transferredQty;

            //if (Convert.ToDecimal(txtQuantity.Text) > Convert.ToDecimal(RemainQty) || RemainQty < 0)
            if (currentQty > RemainQty || RemainQty < 0)
            {
                IsStockAvailable = false;
            }
            return IsStockAvailable;
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
                
                decimal transQty = 0;

                
                for (int i = 0; i < dgvIssue.RowCount; i++)
                {
                    dgvIssue["LineNo", i].Value = i + 1;
                    Quantity = Convert.ToDecimal(dgvIssue["Quantity", i].Value);
                    Cost = Convert.ToDecimal(dgvIssue["UnitPrice", i].Value);

                    SubTotal = Cost * Quantity;
                    if (Program.CheckingNumericString(SubTotal.ToString(), "SubTotal") == true)
                        SubTotal = Convert.ToDecimal(Program.FormatingNumeric(SubTotal.ToString(), IssuePlaceQty));

                    dgvIssue["SubTotal", i].Value = Convert.ToDecimal(SubTotal);

                    
                    SumSubTotal = SumSubTotal + Convert.ToDecimal(dgvIssue["SubTotal", i].Value);
                    transQty =transQty+ Convert.ToDecimal(dgvIssue["Quantity", i].Value) * Convert.ToDecimal(dgvIssue["UOMc", i].Value);
                }
                txtTotalAmount.Text = Convert.ToDecimal(SumSubTotal).ToString();//"0,0.00");
                txtTotalQty.Text = Convert.ToDecimal(transQty).ToString();//"0,0.00");
                RemainQty = RawStock - transQty;
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


        private void btnRemove_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvIssue.RowCount > 0)
                {
                    if (MessageBox.Show(MessageVM.msgWantToRemoveRow + "\nItem Code: " + dgvIssue.CurrentRow.Cells["PCode"].Value, this.Text, MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        dgvIssue.Rows.RemoveAt(dgvIssue.CurrentRow.Index);
                        Rowcalculate();
                        txtUnitCost.Text = txtUnitCost1.Text;
                        txtQuantity1.Text = RemainQty.ToString();
                        if (Program.CheckingNumericTextBox(txtQuantity1, "txtQuantity1") == true)
                            txtQuantity1.Text = Program.FormatingNumeric(txtQuantity1.Text.Trim(), IssuePlaceQty).ToString();

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
        private void remove()
        {
            #region try
            try
            {
                ChangeData = true;

                if (txtItemNo.Text == "")
                {
                    MessageBox.Show("Please Select Desired Item Row Appropriately Using Double Click!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    //MessageBox.Show("Please select a Item", this.Text);
                    return;
                }
                //if (txtCommentsDetail.Text == "")
                //{
                //    txtCommentsDetail.Text = "NA";
                //}

                #region Stock Chekc
                decimal StockValue, PreviousValue, ChangeValue, CurrentValue, purchaseQty;
                StockValue = Convert.ToDecimal(txtQuantityInHand.Text);
                PreviousValue = Convert.ToDecimal(txtPrevious.Text);
                //CurrentValue = Convert.ToDecimal(txtQuantity.Text);
                CurrentValue = 0;

                //if (rbtnIssueReturn.Checked == false)
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
                //    if (CurrentValue < PreviousValue)
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
                //}

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
                txtItemNo.Text = "";
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

        private void ReceiveChangeSingle()
        {
            try
            {
                #region try
                if (
                    string.IsNullOrEmpty(txtUOM.Text)
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
                if (Convert.ToDecimal(txtUnitCost.Text.Trim()) <= 0)
                {
                    MessageBox.Show(MessageVM.msgNegPrice);

                    txtUnitCost.Focus();
                    return;
                }
               
                ChangeData = true;
                if (cmbUom.SelectedIndex == -1)
                {
                    throw new ArgumentNullException("", "Please select pack size");
                }
                UomsValue();
                //bool isStoc = CheckStock();
                if (Convert.ToDecimal(txtQuantity.Text) * Convert.ToDecimal(txtUomConv.Text) > RemainQty)
                {
                     MessageBox.Show("Stock Not available");
                    txtQuantity.Focus();
                    return;
                }
                //if (isStoc == false)
                //{
                   
                //}

                string strUom = cmbUom.SelectedItem.ToString();
                dgvIssue["UOM", dgvIssue.RowCount - 1].Value = strUom.Trim();
                dgvIssue["Quantity", dgvIssue.CurrentRow.Index].Value = Convert.ToDecimal(txtQuantity.Text.Trim()).ToString();//"0,0.000");

                dgvIssue["UnitPrice", dgvIssue.RowCount - 1].Value =
                    Convert.ToDecimal(Convert.ToDecimal(txtUnitCost1.Text.Trim()) * Convert.ToDecimal(txtUomConv.Text.Trim())).ToString();
                dgvIssue["UOMPrice", dgvIssue.RowCount - 1].Value = Convert.ToDecimal(Convert.ToDecimal(txtUnitCost.Text.Trim())).ToString();

                dgvIssue["UOMc", dgvIssue.CurrentRow.Index].Value = Convert.ToDecimal(txtUomConv.Text.Trim());
                dgvIssue["UOMn", dgvIssue.CurrentRow.Index].Value = txtUOM.Text.Trim();
                dgvIssue["UOMQty", dgvIssue.CurrentRow.Index].Value =
                    Convert.ToDecimal(Convert.ToDecimal(txtQuantity.Text.Trim()) *
                    Convert.ToDecimal(txtUomConv.Text.Trim()));

                if (dgvIssue.CurrentRow.Cells["Status"].Value.ToString() != "New")
                {
                    dgvIssue["Status", dgvIssue.CurrentRow.Index].Value = "Change";
                }
                dgvIssue.CurrentRow.DefaultCellStyle.ForeColor = Color.Green;// Blue;

                dgvIssue.CurrentRow.DefaultCellStyle.Font = new Font(this.Font, FontStyle.Regular);

                Rowcalculate();
                #region Stock
                txtQuantity1.Text = RemainQty.ToString();
                if (Program.CheckingNumericTextBox(txtQuantity1, "txtQuantity1") == true)
                    txtQuantity1.Text = Program.FormatingNumeric(txtQuantity1.Text.Trim(), IssuePlaceQty).ToString();
                #endregion


                txtItemNo.Text = "";
                txtProductName.Text = "";
                //txtCategoryName.Text = "";
                txtHSCode.Text = "";
                txtUnitCost.Text = txtUnitCost1.Text;
                txtQuantity.Text = "";
                //txtVATRate.Text = "0.00";
                txtUOM.Text = "";
                //txtCommentsDetail.Text = "NA";
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

        public void ClearAllFields()
        {
            txtTransQty.Visible = false;
            txtTransQty.Enabled = false;
            lblTransQty.Visible = false;

            txtTransferId.Text = "";
            cmbProduct1.Text = "Select";
            rbtnCode1.Checked = true;
            cmbProductName1.Text = "";
            txtPCode1.Text = "";
            txtHSCode1.Text = "";
            txtItemNo1.Text = "";
            txtUOM1.Text = "";
            txtProductName1.Text = "";
            txtQuantity1.Text = "0.00";
            txtTransQty.Text = "0.0";
            txtUnitCost1.Text = "0.00";
            cmbUom1.Text = "";
            cmbProductName1.Enabled = true;
            cmbProduct1.Enabled = true;
           
            cmbProduct.Text = "";
            txtQuantityInHand.Text = "0.0";
            txtPCode.Text = "";
            txtHSCode.Text = "";
            txtItemNo.Text = "";
            txtUOM.Text = "";
            txtProductName.Text = "";
            txtQuantity.Text = "0.00";
            txtUnitCost.Text = "0.00";
            cmbProductName.Text = "";
            cmbUom.Text = "";
            rbtnCode.Checked = true;

            txtTotalAmount.Text = "0.00";
            txtTotalQty.Text = "0.00";
            
            dtpTransferDate.Value = Convert.ToDateTime(Program.SessionDate.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss"));
            dgvIssue.Rows.Clear();
        }


        //private void txtComments_TextChanged(object sender, EventArgs e)
        //{
        //    ChangeData = true;
        //}

        //private void txtVATRate_Leave(object sender, EventArgs e)
        //{
        //    Program.FormatTextBoxRate(txtVATRate, "VAT Rate");
        //}

        private void txtQuantity_Leave(object sender, EventArgs e)
        {
            //Program.FormatTextBoxQty4(txtQuantity, "Quantity");
                       
            if (Program.CheckingNumericTextBox(txtQuantity, "Total Quantity") == true)
            {
                txtQuantity.Text = Program.FormatingNumeric(txtQuantity.Text.Trim(), IssuePlaceQty).ToString();

            }
        }

        private void txtQuantity_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void FormRawTransfer_Load(object sender, EventArgs e)
        {
            #region try
            try
            {
                btnSave.Text = "&Add";
                txtTransferId.Text = "~~~ New ~~~";
                
                FormLoad();
                ChangeData = false;

                Post = Convert.ToString(Program.Post) == "Y" ? true : false;
                Add = Convert.ToString(Program.Add) == "Y" ? true : false;
                Edit = Convert.ToString(Program.Edit) == "Y" ? true : false;
                #region Settings

                string vIssuePlaceQty, vIssuePlaceAmt = string.Empty;


                CommonDAL commonDal = new CommonDAL();

                vIssuePlaceQty = commonDal.settingsDesktop("Issue", "Quantity",null,connVM);
                vIssuePlaceAmt = commonDal.settingsDesktop("Issue", "Amount",null,connVM);
               
                if (string.IsNullOrEmpty(vIssuePlaceQty)
                    || string.IsNullOrEmpty(vIssuePlaceAmt))
                    
                {
                    MessageBox.Show(MessageVM.msgSettingValueNotSave, this.Text);
                    return;
                }

                IssuePlaceQty = Convert.ToInt32(vIssuePlaceQty);
                IssuePlaceAmt = Convert.ToInt32(vIssuePlaceAmt);
                
               
                #endregion Settings
                progressBar1.Visible = true;
                btnIssue.Visible = false;
                bgwLoad.RunWorkerAsync();
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
                FileLogger.Log(this.Name, "FormRawTransfer_Load", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "FormRawTransfer_Load", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "FormRawTransfer_Load", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "FormRawTransfer_Load", exMessage);
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
                FileLogger.Log(this.Name, "FormRawTransfer_Load", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormRawTransfer_Load", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormRawTransfer_Load", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "FormRawTransfer_Load", exMessage);
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
                varIsRaw = "Raw";
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

            txtCategoryName1.Text = "Raw";
            #endregion Product

        }
        private void bgwLoad_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                #region UOM
                UOMDAL uomdal = new UOMDAL();
                string[] cvals = new string[] { UOMIdParam, UOMFromParam, UOMToParam, ActiveStatusUOMParam };
                string[] cfields = new string[] { "UOMId like", "UOMFrom like", "UOMTo like", "ActiveStatus like" };
                uomResult = uomdal.SelectAll(0, cfields, cvals, null, null, false,connVM);

                //uomResult = uomdal.SearchUOM(UOMIdParam, UOMFromParam, UOMToParam, ActiveStatusUOMParam, Program.DatabaseName);

                #endregion UOM

                #region Product
                ProductDAL productDal = new ProductDAL();
                ProductResultDs = productDal.SearchProductMiniDS(varItemNo, varCategoryID, varIsRaw, varHSCodeNo, varActiveStatus, varTrading, varNonStock, varProductCode,connVM);

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
                FileLogger.Log(this.Name, "bgwLoad_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "bgwLoad_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "bgwLoad_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "bgwLoad_DoWork", exMessage);
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
                FileLogger.Log(this.Name, "bgwLoad_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwLoad_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwLoad_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "bgwLoad_DoWork", exMessage);
            }
            #endregion

        }
        private void bgwLoad_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {

                #region Product
                //ProductsMini.Clear();
                RawsMini.Clear();
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

                    RawsMini.Add(prod);
                }//End FOR
                //ProductSearchDsLoad();
                RawSearchDsLoad();
                #endregion Product

                #region UOM
                UOMs.Clear();
                cmbUom.Items.Clear();
                foreach (DataRow item2 in uomResult.Rows)
                {
                    var uom = new UomDTO();
                    uom.UOMId = item2["UOMId"].ToString();
                    uom.UOMFrom = item2["UOMFrom"].ToString();
                    uom.UOMTo = item2["UOMTo"].ToString();
                    uom.Convertion = Convert.ToDecimal(item2["Convertion"].ToString());
                    uom.CTypes = item2["CTypes"].ToString();
                    cmbUom.Items.Add(item2["UOMTo"].ToString());
                    //cmbUom1.Items.Add(item2["UOMTo"].ToString());

                    UOMs.Add(uom);

                }
                #endregion UOM
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
                FileLogger.Log(this.Name, "bgwLoad_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "bgwLoad_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "bgwLoad_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "bgwLoad_RunWorkerCompleted", exMessage);
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
                FileLogger.Log(this.Name, "bgwLoad_RunWorkerCompleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwLoad_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwLoad_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "bgwLoad_RunWorkerCompleted", exMessage);
            }
            #endregion
            finally
            {
                ChangeData = false;
                this.progressBar1.Visible = false;
            }

        }

        private void ProductSearchDsFormLoad( string categoryName)
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
                    varHSCodeNo = "";
                    varActiveStatus = "Y";
                    varTrading = "N";
                    varNonStock = "N";
                    varProductCode = "";
                    //txtCategoryName.Text = "Raw";

                }
                else
                {
                    varItemNo = "";
                    varCategoryID = CategoryId;
                    varIsRaw = "";
                    //varHSCodeNo = txtCategoryName.Text.Trim();
                    varHSCodeNo = categoryName;

                    varActiveStatus = "Y";
                    varTrading = "N";
                    varNonStock = "N";
                    varProductCode = "";
                }


                this.cmbProduct.Enabled = false;
                this.btnSearchCategory.Enabled = false;
                this.progressBar1.Visible = true;

                //backgroundWorkerProductSearchDsFormLoad.RunWorkerAsync();
                #region Product Search
                ProductDAL productDal = new ProductDAL();
                ProductResultDs = new DataTable();
                ProductResultDs = productDal.SearchProductMiniDS(varItemNo, varCategoryID, varIsRaw, varHSCodeNo, varActiveStatus, varTrading, varNonStock, varProductCode,connVM);

                #region Products fill in List
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
                #endregion Products fill in List
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
                this.btnSearchCategory.Enabled = true;
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

                cmbProduct.Text = dgvIssue.CurrentRow.Cells["PCode"].Value.ToString();
                cmbProductName.Text = dgvIssue.CurrentRow.Cells["ItemName"].Value.ToString();

                txtLineNo.Text = dgvIssue.CurrentCellAddress.Y.ToString();
                txtItemNo.Text = dgvIssue.CurrentRow.Cells["ItemNo"].Value.ToString();
                txtPCode.Text = dgvIssue.CurrentRow.Cells["PCode"].Value.ToString();
                txtProductName.Text = dgvIssue.CurrentRow.Cells["ItemName"].Value.ToString();
                txtUOM.Text = dgvIssue.CurrentRow.Cells["UOM"].Value.ToString();
                txtQuantity.Text = Convert.ToDecimal(dgvIssue.CurrentRow.Cells["Quantity"].Value).ToString();//"0,0.0000");
                txtUnitCost.Text = Convert.ToDecimal(dgvIssue.CurrentRow.Cells["UnitPrice"].Value).ToString();//"0,0.00");
                //txtVATRate.Text = Convert.ToDecimal(dgvIssue.CurrentRow.Cells["VATRate"].Value).ToString();//"0.00");
                //txtCommentsDetail.Text = "NA";// dgvIssue.CurrentRow.Cells["Comments"].Value.ToString();
                //txtNBRPrice.Text = Convert.ToDecimal(dgvIssue.CurrentRow.Cells["NBRPrice"].Value).ToString();//"0,0.00");

                //txtPrevious.Text = Convert.ToDecimal(dgvIssue.CurrentRow.Cells["Previous"].Value).ToString();//"0,0.0000");

                txtUOM.Text = dgvIssue.CurrentRow.Cells["UOMn"].Value.ToString();
                cmbUom.Items.Insert(0, txtUOM.Text.Trim());
                Uoms();
                cmbUom.Text = dgvIssue.CurrentRow.Cells["UOM"].Value.ToString();

                /*UOM Convertion */



                txtUomConv.Text = dgvIssue.CurrentRow.Cells["UOMc"].Value.ToString();

                /*UOM Convertion */
                //ProductDAL productDal = new ProductDAL();
                //txtQuantityInHand.Text = productDal.AvgPriceNew(txtItemNo.Text, dtpTransferDate.Value.ToString("yyyy-MMM-dd") +
                //                                                                DateTime.Now.ToString(" HH:mm:ss"),
                //                                                                null, null, false).Rows[0]["Quantity"].ToString();
                #region Stock
                RemainQty = RemainQty +( Convert.ToDecimal(dgvIssue.CurrentRow.Cells["Quantity"].Value)*Convert.ToDecimal(dgvIssue.CurrentRow.Cells["UOMc"].Value.ToString()));
                txtQuantity1.Text = RemainQty.ToString();

                #endregion

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

        private void btnSearchIssueNo_Click(object sender, EventArgs e)
        {
            #region try
            try
            {

                Program.fromOpen = "Me";
                #region Transaction Type
               

                DataGridViewRow selectedRow = new DataGridViewRow();

                selectedRow = FormTransferRawSearch.SelectOne();

                #endregion Transaction Type

                if (selectedRow != null && selectedRow.Selected == true)
                {

                    txtTransferId.Text = selectedRow.Cells["TransferId"].Value.ToString();
                    dtpTransferDate.Value = Convert.ToDateTime(selectedRow.Cells["TransferDateTime"].Value.ToString());

                    txtTotalQty.Text = Convert.ToDecimal(selectedRow.Cells["TransferedQty"].Value.ToString()).ToString();
                    if (Program.CheckingNumericTextBox(txtTotalQty, "txtTotalQty") == true)
                        txtTotalQty.Text = Program.FormatingNumeric(txtTotalQty.Text.Trim(), IssuePlaceQty).ToString();

                    txtTotalAmount.Text = Convert.ToDecimal(selectedRow.Cells["TransferedAmt"].Value.ToString()).ToString();
                    if (Program.CheckingNumericTextBox(txtTotalAmount, "txtTotalAmount") == true)
                        txtTotalAmount.Text = Program.FormatingNumeric(txtTotalAmount.Text.Trim(), IssuePlaceAmt).ToString();

                    txtTransQty.Text = Convert.ToDecimal(selectedRow.Cells["TransferedQty"].Value.ToString()).ToString();
                    if (Program.CheckingNumericTextBox(txtTransQty, "txtTransQty") == true)
                        txtTransQty.Text = Program.FormatingNumeric(txtTransQty.Text.Trim(), IssuePlaceQty).ToString();
                    txtTransQty.Visible = true;
                    txtTransQty.Enabled = false;
                    lblTransQty.Visible = true;

                    txtUnitCost1.Text = Convert.ToDecimal(selectedRow.Cells["CostPrice"].Value.ToString()).ToString();
                    if (Program.CheckingNumericTextBox(txtUnitCost1, "txtUnitCost1") == true)
                        txtUnitCost1.Text = Program.FormatingNumeric(txtUnitCost1.Text.Trim(), IssuePlaceAmt).ToString();

                    txtUnitCost.Text = Convert.ToDecimal(selectedRow.Cells["CostPrice"].Value.ToString()).ToString();
                    if (Program.CheckingNumericTextBox(txtUnitCost, "txtUnitCost") == true)
                        txtUnitCost.Text = Program.FormatingNumeric(txtUnitCost.Text.Trim(), IssuePlaceAmt).ToString();


                    txtUOM1.Text = selectedRow.Cells["UOM"].Value.ToString();//"0,0.00");
                    txtPCode1.Text = selectedRow.Cells["TransFromCode"].Value.ToString();//"0,0.00");
                    txtProductName1.Text = selectedRow.Cells["TransFromName"].Value.ToString();//"0,0.00");
                    txtItemNo1.Text = selectedRow.Cells["TransFromItemNo"].Value.ToString();//"0,0.00");
                    txtUomConv1.Text = "1";
                    cmbProduct1.Text = selectedRow.Cells["TransFromCode"].Value.ToString();
                    cmbProductName1.Text = selectedRow.Cells["TransFromName"].Value.ToString();
                   
                    IsPost = Convert.ToString(selectedRow.Cells["Post"].Value.ToString()) == "Y" ? true : false;
                    
                    IssueDetailData = txtTransferId.Text == "" ? "" : txtTransferId.Text.Trim();

                    ProductDAL productDal = new ProductDAL();
                    txtQuantity1.Text = productDal.AvgPriceNew(txtItemNo1.Text, dtpTransferDate.Value.ToString("yyyy-MMM-dd") +
                                                                                    DateTime.Now.ToString(" HH:mm:ss"),
                                                                                    null, null, false,true,true,true,connVM,Program.CurrentUserID).Rows[0]["Quantity"].ToString();
                    RawStock = Convert.ToDecimal(txtQuantity1.Text);
                    if (Program.CheckingNumericTextBox(txtQuantity1, "txtQuantity1") == true)
                        txtQuantity1.Text = Program.FormatingNumeric(txtQuantity1.Text.Trim(), IssuePlaceQty).ToString();

                }
               
                backgroundWorkerSearchIssueNo.RunWorkerAsync();

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

        #region backgroundWorkerSearchIssueNo Event

        private void backgroundWorkerSearchIssueNo_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                #region Statement

                IssueDetailResult = new DataTable();

                TransferRawDAL transRawDal = new TransferRawDAL();

                IssueDetailResult = transRawDal.SearchTransferRawDetailDTNew(IssueDetailData,connVM); // Change 04

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

        private void backgroundWorkerSearchIssueNo_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            try
            {
                #region Statement

                dgvIssue.Rows.Clear();
                int j = 0;
                foreach (DataRow item in IssueDetailResult.Rows)
                {


                    DataGridViewRow NewRow = new DataGridViewRow();

                    dgvIssue.Rows.Add(NewRow);

                    #region Master
                    dgvIssue.Rows[j].Cells["TransferedItemNo"].Value = item["TransFromItemNo"].ToString();
                    dgvIssue.Rows[j].Cells["TransferedCode"].Value = item["TransFromCode"].ToString();
                    dgvIssue.Rows[j].Cells["TransferedName"].Value = item["TransFromName"].ToString();


                    #endregion Master
                    dgvIssue.Rows[j].Cells["LineNo"].Value = item["TransLineNo"].ToString();
                    dgvIssue.Rows[j].Cells["ItemName"].Value = item["ProductName"].ToString();// IssueDetailFields[11].ToString();
                    dgvIssue.Rows[j].Cells["PCode"].Value = item["ProductCode"].ToString();
                    dgvIssue.Rows[j].Cells["ItemNo"].Value = item["ItemNo"].ToString();// IssueDetailFields[2].ToString();
                    dgvIssue.Rows[j].Cells["Quantity"].Value = item["Quantity"].ToString();//Convert.ToDecimal(IssueDetailFields[3].ToString()).ToString();//"0,0.0000");
                    dgvIssue.Rows[j].Cells["UnitPrice"].Value = item["CostPrice"].ToString();
                    dgvIssue.Rows[j].Cells["UOM"].Value = item["UOM"].ToString();
                    dgvIssue.Rows[j].Cells["SubTotal"].Value = item["SubTotal"].ToString();

                    dgvIssue.Rows[j].Cells["UOMc"].Value = item["UOMc"].ToString();
                    dgvIssue.Rows[j].Cells["UOMn"].Value = item["UOMn"].ToString();
                    dgvIssue.Rows[j].Cells["UOMQty"].Value = item["UOMQty"].ToString();
                    dgvIssue.Rows[j].Cells["UOMPrice"].Value = item["UOMPrice"].ToString();
                    dgvIssue.Rows[j].Cells["Status"].Value = "Old";
                    dgvIssue.Rows[j].Cells["Change"].Value = 0;

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
                this.btnSearchTransferId.Enabled = true;
                this.progressBar1.Visible = false;
            }


        }

        #endregion
        private void btnSearchProductName_Click(object sender, EventArgs e)
        {
            string itemNo = "72";
            decimal costPrice = 550;

            var dd = ProductsMini.ToList();

            var tt = ProductsMini.SingleOrDefault(x => x.ItemNo == itemNo);

            tt.CostPrice = costPrice;

            var aa = tt;

        }

        private void txtItemNo_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;

           
        }

        private void txtIssueNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtSerialNo_KeyDown(object sender, KeyEventArgs e)
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

        private void FormRawTransfer_FormClosing(object sender, FormClosingEventArgs e)
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

        //private void txtSD_Leave(object sender, EventArgs e)
        //{
        //    Program.FormatTextBoxRate(txtSD, "SD Rate");
        //}

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
                        ClearAllFields();
                        btnSave.Text = "&Add";
                        txtTransferId.Text = "~~~ New ~~~";

                    }
                }
                else if (ChangeData == false)
                {
                    ClearAllFields();
                    btnSave.Text = "&Add";
                    txtTransferId.Text = "~~~ New ~~~";
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
                if (Program.CheckLicence(dtpTransferDate.Value) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }
                
                FormRptIssueInformation frmRptIssueInformation = new FormRptIssueInformation();

                if (txtTransferId.Text == "~~~ New ~~~")
                {
                    frmRptIssueInformation.txtIssueNo.Text = "";
                }
                else
                {
                    frmRptIssueInformation.txtIssueNo.Text = txtTransferId.Text.Trim();
                }
                //frmRptIssueInformation.txtTransactionType.Text = transactionType;


                frmRptIssueInformation.ShowDialog();
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

        private void btnVAT16_Click(object sender, EventArgs e)
        {
            #region try
            try
            {
                if (Program.CheckLicence(dtpTransferDate.Value) == true)
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
                    frmRptVAT16.dtpFromDate.Value = dtpTransferDate.Value;
                    frmRptVAT16.dtpToDate.Value = dtpTransferDate.Value;

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

        private void btnVAT18_Click(object sender, EventArgs e)
        {
            #region try
            try
            {
                if (Program.CheckLicence(dtpTransferDate.Value) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }
                MDIMainInterface mdi = new MDIMainInterface();
                FormRptVAT18 frmRptVAT18 = new FormRptVAT18();

                //mdi.RollDetailsInfo("8401");
                //if (Program.Access != "Y")
                //{
                //    MessageBox.Show("You do not have to access this form", frmRptVAT18.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                //    return;
                //}
                frmRptVAT18.dtpToDate.Value = dtpTransferDate.Value;
                frmRptVAT18.dtpFromDate.Value = dtpTransferDate.Value;
                frmRptVAT18.ShowDialog();
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
                FileLogger.Log(this.Name, "btnVAT18_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnVAT18_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnVAT18_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnVAT18_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnVAT18_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnVAT18_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnVAT18_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnVAT18_Click", exMessage);
            }
            #endregion
        }

        //private void chkPCode_CheckedChanged(object sender, EventArgs e)
        //{
        //    if (chkPCode.Checked)
        //    {
        //        chkPCode.Text = "By Code";
        //    }
        //    else
        //    {
        //        chkPCode.Text = "By Name";

        //    }
        //    ProductSearchDsLoad();

        //    cmbProduct.Focus();
        //}

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
                                    txtItemNo.Text = products.ItemNo;
                                    txtUOM.Text = products.UOM;
                                    cmbUom.Text = products.UOM;
                                    txtHSCode.Text = products.HSCodeNo;
                                    txtPCode.Text = products.ProductCode;
                                   
                                }
                            }

                        }
                        string strProductCode = txtItemNo.Text;
                        if (!string.IsNullOrEmpty(strProductCode))
                        {
                        //    ProductDAL productDal = new ProductDAL();

                        //    DataTable priceData = productDal.AvgPriceNew(strProductCode, dtpTransferDate.Value.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss"), null, null, false);
                        //    decimal amount = Convert.ToDecimal(priceData.Rows[0]["Amount"].ToString());
                        //    decimal quan = Convert.ToDecimal(priceData.Rows[0]["Quantity"].ToString());

                        //    if (quan > 0)
                        //    {
                        //        txtUnitCost.Text = (amount / quan).ToString();
                        //    }
                        //    else
                        //    {
                        //        txtUnitCost.Text = "0";
                        //    }
                            
                        //    txtQuantityInHand.Text = quan.ToString();
                        //    PirceCall();
                  
                        txtQuantity.Focus();
                        Uoms();
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
        private void PirceCall()
        {
            ProductDAL productDal = new ProductDAL();
            string strProductCode = txtItemNo.Text;
            if (!string.IsNullOrEmpty(strProductCode))
            {
                if (Program.CheckingNumericTextBox(txtUnitCost, "Unit Cost") == true)
                {
                    txtUnitCost.Text = Program.FormatingNumeric(txtUnitCost.Text.Trim(), IssuePlaceAmt).ToString();
                    txtUnitCost1.Text = Program.FormatingNumeric(txtUnitCost1.Text.Trim(), IssuePlaceAmt).ToString();

                }
                else
                {

                    return;
                }
            }
        }
        private void btnSearchCategory_Click(object sender, EventArgs e)
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

                ProductSearchDsFormLoad(txtCategoryName.Text.Trim());
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

       
       

        //===========================================================

        #region backgroundWorkerSave Event


        #endregion

        //===========================================================

       

       
        private void backgroundWorkerProductSearchDsFormLoad_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                ProductDAL productDal = new ProductDAL();
                ProductResultDs = new DataTable();
                ProductResultDs = productDal.SearchProductMiniDS(varItemNo, varCategoryID, varIsRaw, varHSCodeNo, varActiveStatus, varTrading, varNonStock, varProductCode,connVM);

                //End DoWork
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
                FileLogger.Log(this.Name, "backgroundWorkerProductSearchDsFormLoad_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerProductSearchDsFormLoad_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerProductSearchDsFormLoad_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "backgroundWorkerProductSearchDsFormLoad_DoWork", exMessage);
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
                FileLogger.Log(this.Name, "backgroundWorkerProductSearchDsFormLoad_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerProductSearchDsFormLoad_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerProductSearchDsFormLoad_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "backgroundWorkerProductSearchDsFormLoad_DoWork", exMessage);
            }
            #endregion
        }

        private void backgroundWorkerProductSearchDsFormLoad_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                //Start Complete
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
               //ProductSearchDsLoad();
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
                FileLogger.Log(this.Name, "backgroundWorkerProductSearchDsFormLoad_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerProductSearchDsFormLoad_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerProductSearchDsFormLoad_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "backgroundWorkerProductSearchDsFormLoad_RunWorkerCompleted", exMessage);
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
                FileLogger.Log(this.Name, "backgroundWorkerProductSearchDsFormLoad_RunWorkerCompleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerProductSearchDsFormLoad_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerProductSearchDsFormLoad_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "backgroundWorkerProductSearchDsFormLoad_RunWorkerCompleted", exMessage);
            }
            #endregion

            this.cmbProduct.Enabled = true;
            this.btnSearchCategory.Enabled = true;
            this.progressBar1.Visible = false;
        }

        //===========================================================
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
                                    txtItemNo.Text = products.ItemNo;
                                    txtUOM.Text = products.UOM;
                                    cmbUom.Text = products.UOM;
                                    txtHSCode.Text = products.HSCodeNo;
                                    txtPCode.Text = products.ProductCode;
                                }
                            }

                        }
                        string strProductCode = txtItemNo.Text;
                        if (!string.IsNullOrEmpty(strProductCode))
                        {
                            //ProductDAL productDal = new ProductDAL();

                            //DataTable priceData = productDal.AvgPriceNew(strProductCode, dtpTransferDate.Value.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss"), null, null, false);
                            //decimal amount = Convert.ToDecimal(priceData.Rows[0]["Amount"].ToString());
                            //decimal quan = Convert.ToDecimal(priceData.Rows[0]["Quantity"].ToString());

                            //if (quan > 0)
                            //{
                            //    txtUnitCost.Text = (amount / quan).ToString();
                            //}
                            //else
                            //{
                            //    txtUnitCost.Text = "0";
                            //}

                            //txtQuantityInHand.Text = quan.ToString();
                            //PirceCall();
                        txtQuantity.Focus();
                        Uoms();
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
        
        private void btnSearchCategory1_Click(object sender, EventArgs e)
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
                txtCategoryName1.Text = ProductCategoryInfo[1];
                cmbProductType1.Text = ProductCategoryInfo[4];

                ProductSearchDsFormLoad(txtCategoryName1.Text.Trim());
                RawsMini.Clear();
                RawsMini = ProductsMini;
                RawSearchDsLoad();
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

        private void RawSearchDsLoad()
        {
            //No SOAP Service

            #region try
            try
            {
                cmbProduct1.Items.Clear();
                cmbProductName1.Items.Clear();


                if (rbtnCode1.Checked == true)
                {

                    var prodByCode = from prd in RawsMini.ToList()
                                     orderby prd.ProductCode
                                     select prd.ProductCode;


                    if (prodByCode != null && prodByCode.Any())
                    {
                        cmbProduct1.Items.AddRange(prodByCode.ToArray());
                    }


                }
                else if (rbtnProduct1.Checked == true)
                {
                    var prodByName = from prd in RawsMini.ToList()
                                     orderby prd.ProductName
                                     select prd.ProductName;


                    if (prodByName != null && prodByName.Any())
                    {
                        cmbProductName1.Items.AddRange(prodByName.ToArray());
                    }
                }

                cmbProduct1.Items.Insert(0, "Select");
                cmbProduct1.Text = "Select";
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

        private void rbtnCode1_CheckedChanged(object sender, EventArgs e)
        {
            if (rbtnCode1.Checked)
            {
                cmbProductName1.Enabled = false;
                cmbProduct1.Enabled = true;

            }
            
            RawSearchDsLoad();

        }

        private void rbtnProduct1_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                #region Statement

                if (rbtnProduct1.Checked)
                {
                    cmbProductName1.Enabled = true;
                    cmbProduct1.Enabled = false;


                }
                RawSearchDsLoad();
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

        private void cmbProduct1_Leave(object sender, EventArgs e)
        {
            #region try
            try
            {

                var searchText = cmbProduct1.Text.Trim().ToLower();
                if (!string.IsNullOrEmpty(searchText) && searchText != "select")
                {
                    if (rbtnCode1.Checked)
                    {
                        var prodByCode = from prd in RawsMini.ToList()
                                         where prd.ProductCode.ToLower() == searchText.ToLower()
                                         select prd;
                        if (prodByCode != null && prodByCode.Any())
                        {
                            var products = prodByCode.First();
                            txtProductName1.Text = products.ProductName;
                            cmbProductName1.Text = products.ProductName;
                            txtItemNo1.Text = products.ItemNo;
                            txtUOM1.Text = products.UOM;
                            cmbUom1.Text = products.UOM;
                            txtHSCode1.Text = products.HSCodeNo;
                            txtPCode1.Text = products.ProductCode;

                        }
                    }

                }
                string strProductCode = txtItemNo1.Text;
                if (!string.IsNullOrEmpty(strProductCode))
                {
                    ProductDAL productDal = new ProductDAL();

                    DataTable priceData = productDal.AvgPriceNew(strProductCode, dtpTransferDate.Value.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss"), null, null, false,true,true,true,connVM,Program.CurrentUserID);
                    decimal amount = Convert.ToDecimal(priceData.Rows[0]["Amount"].ToString());
                    decimal quan = Convert.ToDecimal(priceData.Rows[0]["Quantity"].ToString());

                    if (quan > 0)
                    {
                        txtUnitCost1.Text = (amount / quan).ToString();
                        txtUnitCost.Text = (amount / quan).ToString();
                    }
                    else
                    {
                        txtUnitCost1.Text = "0";
                        txtUnitCost.Text = "0";
                    }

                    RawStock = quan;
                    PirceCall();
                    //}
                    //txtQuantity.Focus();
                    txtQuantity1.Text = quan.ToString();
                    if (Program.CheckingNumericTextBox(txtQuantity1, "txtQuantity1") == true)
                        txtQuantity1.Text = Program.FormatingNumeric(txtQuantity1.Text.Trim(), IssuePlaceQty).ToString();
                    if (Program.CheckingNumericTextBox(txtUnitCost1, "txtUnitCost1") == true)
                        txtUnitCost1.Text = Program.FormatingNumeric(txtUnitCost1.Text.Trim(), IssuePlaceAmt).ToString();
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

        private void cmbProductName1_Leave(object sender, EventArgs e)
        {
            #region try
            try
            {
                var searchText = cmbProductName1.Text.Trim().ToLower();

                if (!string.IsNullOrEmpty(searchText) && searchText != "select")
                {
                    if (rbtnProduct1.Checked)
                    {
                        var prodByName = from prd in RawsMini.ToList()
                                         where prd.ProductName.ToLower() == searchText.ToLower()
                                         select prd;

                        if (prodByName != null && prodByName.Any())
                        {
                            var products = prodByName.First();
                            txtProductName1.Text = products.ProductName;
                            cmbProduct1.Text = products.ProductCode;
                            txtItemNo1.Text = products.ItemNo;
                            txtUOM1.Text = products.UOM;
                            cmbUom1.Text = products.UOM;
                            txtHSCode1.Text = products.HSCodeNo;
                            txtPCode1.Text = products.ProductCode;
                        }
                    }

                }
                string strProductCode = txtItemNo1.Text;
                if (!string.IsNullOrEmpty(strProductCode))
                {                                               
                    ProductDAL productDal = new ProductDAL();

                    DataTable priceData = productDal.AvgPriceNew(strProductCode, dtpTransferDate.Value.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss"), null, null, false,true,true,true,connVM,Program.CurrentUserID);
                    decimal amount = Convert.ToDecimal(priceData.Rows[0]["Amount"].ToString());
                    decimal quan = Convert.ToDecimal(priceData.Rows[0]["Quantity"].ToString());

                    if (quan > 0)
                    {
                        txtUnitCost1.Text = (amount / quan).ToString();
                        txtUnitCost.Text = (amount / quan).ToString();
                    }
                    else
                    {
                        txtUnitCost1.Text = "0";
                        txtUnitCost.Text = "0";
                    }

                    RawStock = quan;
                    txtQuantity1.Text = quan.ToString();
                    if (Program.CheckingNumericTextBox(txtQuantity1, "txtQuantity1") == true)
                        txtQuantity1.Text = Program.FormatingNumeric(txtQuantity1.Text.Trim(), IssuePlaceQty).ToString();
                    if (Program.CheckingNumericTextBox(txtUnitCost1, "txtUnitCost1") == true)
                        txtUnitCost1.Text = Program.FormatingNumeric(txtUnitCost1.Text.Trim(), IssuePlaceAmt).ToString();
                    PirceCall();
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

        private void cmbProduct1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void cmbProductName1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtQuantity1_Leave(object sender, EventArgs e)
        {

            if (Program.CheckingNumericTextBox(txtQuantity, "Total Quantity") == true)
            {
                txtQuantity1.Text = Program.FormatingNumeric(txtQuantity1.Text.Trim(), IssuePlaceQty).ToString();

            }
        }

        private void txtQuantity1_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtItemNo1_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void cmbProduct1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                btnSearchCategory.Focus();
            }
        }

        private void cmbProductName1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                btnSearchCategory.Focus();
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void txtUnitCost1_Leave(object sender, EventArgs e)
        {
            Program.FormatTextBox(txtUnitCost1, "UnitCost1");
        }

        private void txtTransQty_Leave(object sender, EventArgs e)
        {
            Program.FormatTextBox(txtTransQty, "TransQty");
        }

        private void txtUnitCost_Leave(object sender, EventArgs e)
        {
            Program.FormatTextBox(txtUnitCost, "UnitCost");
        }

        private void txtQuantityInHand_Leave(object sender, EventArgs e)
        {
            Program.FormatTextBox(txtQuantityInHand, "QuantityInHand");
        }

        private void txtTotalAmount_Leave(object sender, EventArgs e)
        {
            Program.FormatTextBox(txtTotalAmount, "TotalAmount");
        }
    }
}
