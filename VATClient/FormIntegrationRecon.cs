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
////
//
using System.Security.Cryptography;
using System.IO;
using Newtonsoft.Json;
using SymphonySofttech.Utilities;
using VATServer.Library;
using VATServer.Ordinary;
using VATViewModel.DTOs;
using VATDesktop.Repo;
using VATServer.Interface;
namespace VATClient
{
    public partial class FormIntegrationRecon : Form
    {

        public FormIntegrationRecon()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();
        }
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        #region Declarations

        IssueMasterVM vm = new IssueMasterVM();
        IssueDAL _IssueDAL = new IssueDAL();
        public string ItemNo { get; set; }

        string ReceiveFromDate = "";
        string ReceiveToDate = "";
        public bool showReproc = true;

        #endregion

        private void FormIssueMultiple_Load(object sender, EventArgs e)
        {

        }


        private void bgwUpdate_DoWork(object sender, DoWorkEventArgs e)
        {
           

        }

        private void bgwUpdate_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (e.Error == null)
                {
                    MessageBox.Show("Data Successfully Saved Permanently(6.1)  !");
                }
                else
                {
                    MessageBox.Show(e.Error.Message);
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
                FileLogger.Log(this.Name, "bgwUpdate_RunWorkerCompleted", exMessage);
            }

            #endregion

            finally
            {
                progressBar1.Visible = false;

            }

            #region Element Stats

            this.progressBar1.Visible = false;

            #endregion

        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.Close();
        }





        private void bgwDeleteProcess_DoWork(object sender, DoWorkEventArgs e)
        {
           
        }

        private void bgwDeleteProcess_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                MessageBox.Show(e.Error == null ? "Data has been deleted in VAT 6.1 permanent table!" : e.Error.Message);
            }
            finally
            {
                progressBar1.Visible = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(cmbType.Text))
                {
                    throw new Exception("Please select a invoice type");

                }

                string searchValue = txtRef.Text;
                if (string.IsNullOrEmpty(searchValue))
                {
                    throw new Exception("Please enter a invoice no");

                }

                ImportDAL importDal = new ImportDAL();
                SaleDAL salesDal = new SaleDAL();
                BranchProfileDAL branchProfileDal = new BranchProfileDAL();

                DataTable branchConInfo = branchProfileDal.SelectAll(Program.BranchId.ToString());
                DataSet dsFinal = new DataSet();

                if (cmbType.Text == "Invoice_No")
                {
                    List<SaleDetailVm> saleDetails = salesDal.SelectSaleDetail(null, new[] {"sd.SalesInvoiceNo"}, new[]
                    {
                        searchValue
                    });
                    DataTable dtHeader = salesDal.SelectAll(0, new[] {"sih.SalesInvoiceNo"}, new[] {searchValue});


                    if (saleDetails != null && saleDetails.Count > 0)
                    {
                        DataTable dtSymResult = saleDetails.ToDataTable();
                        dtSymResult.TableName = "ShampanVAT";
                        dsFinal.Tables.Add(dtSymResult);

                        string refId = dtHeader.Rows[0]["ImportIDExcel"].ToString();

                        if (!string.IsNullOrWhiteSpace(refId))
                        {
                            DataTable dtSaleResult = importDal.GetSaleACIDbData_Check(new IntegrationParam()
                            {
                                RefNo = refId,
                                dtConnectionInfo = branchConInfo
                            });

                            DataTable dtSaleResultAudit = salesDal.GetSaleACI_Audit(new IntegrationParam()
                            {
                                RefNo = refId,
                                dtConnectionInfo = branchConInfo
                            });

                            dtSaleResult.TableName = "Middleware";
                            dtSaleResultAudit.TableName = "Middleware_Audit";

                            dsFinal.Tables.Add(dtSaleResult);
                            dsFinal.Tables.Add(dtSaleResultAudit);
                        }

                    }
                    else
                    {
                        throw new Exception("No data found for invoice no");

                    }

                }
                else if(cmbType.Text == "Ref")
                {
                    DataTable dtSaleResult = importDal.GetSaleACIDbData_Check(new IntegrationParam()
                    {
                        RefNo = searchValue,
                        dtConnectionInfo = branchConInfo
                    });

                    DataTable dtSaleResultAudit = salesDal.GetSaleACI_Audit(new IntegrationParam()
                    {
                        RefNo = searchValue,
                        dtConnectionInfo = branchConInfo
                    });

                    dtSaleResult.TableName = "Middleware";
                    dtSaleResultAudit.TableName = "Middleware_Audit";

                    dsFinal.Tables.Add(dtSaleResult);
                    dsFinal.Tables.Add(dtSaleResultAudit);

                    DataTable dtHeader = salesDal.SelectAll(0, new[] { "sih.ImportIDExcel" }, new[] { searchValue });

                    if (dtHeader.Rows.Count > 0)
                    {
                        List<SaleDetailVm> saleDetails = salesDal.SelectSaleDetail(null, new[] { "sd.SalesInvoiceNo" }, new[]
                        {
                            dtHeader.Rows[0]["SalesInvoiceNo"].ToString()
                        });


                        DataTable dtSymResult = saleDetails.ToDataTable();
                        dtSymResult.TableName = "ShampanVAT";

                        dsFinal.Tables.Add(dtSymResult);

                    }
                }

                OrdinaryVATDesktop.SaveExcelMultiple(dsFinal, "Sale_Reconciliation",
                    (from DataTable dsFinalTable in dsFinal.Tables select dsFinalTable.TableName).ToArray());
            }
            catch (Exception exception)
            {
                FileLogger.Log("Data Recon","Recon",exception.Message);
                MessageBox.Show(exception.Message);
            }
        }

      

    }
}
