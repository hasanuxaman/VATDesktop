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
using VATClient.ModelDTO;
////
using System.Security.Cryptography;
using System.IO;
using SymphonySofttech.Utilities;
////
////
using VATClient.ReportPreview;
using CrystalDecisions.Shared;
using VATClient.ReportPages;
using System.Threading;
using VATServer.Library;
using VATViewModel.DTOs;
using System.Deployment.Application;
using CrystalDecisions.CrystalReports.Engine;
using SymphonySofttech.Reports.Report;
using VATClient.Integration.NBR;
using VATDesktop.Repo;
using VATServer.Interface;
using VATServer.Ordinary;

namespace VATClient
{
    public partial class FormProductCategory : Form
    {
        #region Constructors

        public FormProductCategory()
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

        //List<ProductTypeDTO> ProductTypes = new List<ProductTypeDTO>();

        private const string LineDelimeter = DBConstant.LineDelimeter;
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private static string PassPhrase = DBConstant.PassPhrase;
        private static string EnKey = DBConstant.EnKey;
        private string[] ProductTypeLines;
        private string[] ProductTypeFields;
        private bool ChangeData = false;
        private bool IsUpdate = false;
        private string NextID = string.Empty;
        public string VFIN = "112";
        private string propergationRate = string.Empty;
        private string activeStatus = string.Empty;
        private string trading = string.Empty;
        private string nonStock = string.Empty;
        private string type = string.Empty;
        private string reportype;
        MdiClient mdi;
        #region Global Variables for BackgroundWorker

        private string result = string.Empty;
        //private string resultDelete = string.Empty;
        private string ProductCategoryData = string.Empty;
        private DataTable ProductTypeResult;
        private string TypeData = string.Empty;

        #endregion

        #region sql save, update, delete

        private string[] sqlResults;
        private bool SAVE_DOWORK_SUCCESS = false;
        private bool UPDATE_DOWORK_SUCCESS = false;
        private bool DELETE_DOWORK_SUCCESS = false;

        #endregion

        #endregion

        #region Methods 01

        private void PTypeSearch()
        {
            #region try
            try
            {
                //Start Complete
                cmbProductType.DataSource = null;
                cmbProductType.Items.Clear();
                ChangeData = false;
                ProductDAL productTypeDal = new ProductDAL();
                IList<string> ProductTypeList = new List<string>();

                cmbProductType.DataSource = productTypeDal.ProductTypeList;              
                #region Trading Lisence Check
                if (Program.IsTrading == false)
                {
                    ProductTypeList = productTypeDal.ProductTypeList;
                    ProductTypeList.Remove("Trading");
                    cmbProductType.DataSource = productTypeDal.ProductTypeList;

                }
                #endregion

                #region Trading Lisence Check
                if (Program.IsService == false)
                {
                    ProductTypeList = productTypeDal.ProductTypeList;
                    ProductTypeList.Remove("Service");
                    ProductTypeList.Remove("Service(NonStock)");
                    ProductTypeList.Remove("NonInventory");
                    cmbProductType.DataSource = ProductTypeList;

                }
                #region PType Lisence
                if (Program.IsTrading == false && Program.IsService == false)
                {
                    ProductTypeList = productTypeDal.ProductTypeList;
                    ProductTypeList.Remove("Trading");
                    ProductTypeList.Remove("Service");
                    ProductTypeList.Remove("Service(NonStock)");
                    ProductTypeList.Remove("NonInventory");
                    cmbProductType.DataSource = ProductTypeList;

                }
                #endregion

                cmbProductType.SelectedIndex = -1;

                #endregion



                //End Complete
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
                FileLogger.Log(this.Name, "PTypeSearch", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "PTypeSearch", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "PTypeSearch", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "PTypeSearch", exMessage);
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
                FileLogger.Log(this.Name, "PTypeSearch", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "PTypeSearch", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "PTypeSearch", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "PTypeSearch", exMessage);
            }
            #endregion
        }

        private int ErrorReturn()
        {
            #region try
            try
            {
                if (txtCategoryName.Text.Trim() == "")
                {
                    MessageBox.Show("Please enter Product Category Name.");
                    txtCategoryName.Focus();
                    return 1;
                }
                if (cmbProductType.Text.Trim() == "")
                {
                    MessageBox.Show("Please Select Product Type.");
                    cmbProductType.Focus();
                    return 1;
                }
                if (txtVATRate.Text.Trim() == "")
                {
                    MessageBox.Show("Please enter VAT Rate.");
                    txtVATRate.Focus();
                    return 1;
                } if (cmbProductType.Text.Trim() == "Select")
                {
                    MessageBox.Show("Please enter Product Type.");
                    cmbProductType.Focus();
                    return 1;
                }
                if (txtHSDescription.Text == "")
                {
                    txtHSDescription.Text = "-";
                }
                if (txtDescription.Text == "")
                {
                    txtDescription.Text = "-";
                }
                if (txtComments.Text == "")
                {
                    txtComments.Text = "-";
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
                FileLogger.Log(this.Name, "ErrorReturn", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "ErrorReturn", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "ErrorReturn", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "ErrorReturn", exMessage);
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
                FileLogger.Log(this.Name, "ErrorReturn", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ErrorReturn", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ErrorReturn", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "ErrorReturn", exMessage);
            }
            #endregion

            return 0;
        }

        private void ClearAll()
        {
            cmbProductType.Text = "Select";
            txtCategoryID.Text = "";
            txtCategoryName.Text = "";
            txtDescription.Text = "";
            txtComments.Text = "";
            txtHSCodeNo.Text = "";
            txtVATRate.Text = "0.00";
            txtHSDescription.Text = "";
            txtSDRate.Text = "0.00";
            cmbProductType.Text = "Select";
            cmbHSCode.Text = "Select";
            chkNonStock.Checked = false;
            chkTrading.Checked = false;

        }

        #endregion

        #region Methods 02

        private void btnAdd_Click(object sender, EventArgs e)
        {
            #region try
            try
            {
                if (Program.CheckLicence(DateTime.Now) == true)
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
                    NextID = txtCategoryID.Text.Trim();
                }

                if (txtVATRate.Text == "")
                {
                    txtVATRate.Text = "0.00";
                }

                if (txtSDRate.Text == "")
                {
                    txtSDRate.Text = "0.00";
                }

                if (txtHSCodeNo.Text == "")
                {
                    txtHSCodeNo.Text = "0.00";
                }

                int ErR = ErrorReturn();
                if (ErR != 0)
                {
                    return;
                }

                propergationRate = string.Empty;
                activeStatus = string.Empty;
                trading = string.Empty;
                nonStock = string.Empty;
                type = string.Empty;

                propergationRate = Convert.ToString(chkPropagationRate.Checked ? "Y" : "N");
                activeStatus = Convert.ToString(chkActiveStatus.Checked ? "Y" : "N");
                trading = Convert.ToString(chkTrading.Checked ? "Y" : "N");
                nonStock = Convert.ToString(chkNonStock.Checked ? "Y" : "N");
                type = cmbProductType.Text.Trim();

                this.btnAdd.Enabled = false;
                this.progressBar1.Visible = true;

                reportype = cmbReportType.Text.Trim();

                backgroundWorkerAdd.RunWorkerAsync();

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

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            #region try
            try
            {
                
                if (Program.CheckLicence(DateTime.Now) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }

                NextID = txtCategoryID.Text.Trim();

                if (txtVATRate.Text == "")
                {
                    txtVATRate.Text = "0.00";
                }

                if (txtSDRate.Text == "")
                {
                    txtSDRate.Text = "0.00";
                }

                if (txtHSCodeNo.Text == "")
                {
                    txtHSCodeNo.Text = "0.00";
                }

                int ErR = ErrorReturn();
                if (ErR != 0)
                {
                    return;
                }

                propergationRate = string.Empty;
                activeStatus = string.Empty;
                trading = string.Empty;
                nonStock = string.Empty;
                type = string.Empty;

                propergationRate = Convert.ToString(chkPropagationRate.Checked ? "Y" : "N");
                activeStatus = Convert.ToString(chkActiveStatus.Checked ? "Y" : "N");
                trading = Convert.ToString(chkTrading.Checked ? "Y" : "N");
                nonStock = Convert.ToString(chkNonStock.Checked ? "Y" : "N");
                type = cmbProductType.Text.Trim();

                this.btnUpdate.Enabled = false;
                this.progressBar1.Visible = true;
                reportype = cmbReportType.Text.Trim();

                backgroundWorkerUpdate.RunWorkerAsync();

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
                string exMessage = ex.Message;
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

        private void btnDelete_Click(object sender, EventArgs e)
        {
            #region try
            try
            {
                //if (IsUpdate == false)
                //{
                //    MessageBox.Show("No data deleted." + "\n" + "Please select existing information first", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                //    return;
                //}
                if (Program.CheckLicence(DateTime.Now) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }
                else if (txtCategoryID.Text.Trim() == "")
                {
                    MessageBox.Show("No data deleted." + "\n" + "Please select existing information first", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                else if (txtCategoryID.Text.Trim() == "0")
                {
                    MessageBox.Show("This is Default data ." + "\n" + "This can't be deleted", this.Text,
                                     MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                else if (MessageBox.Show("Do you want to delete data?", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    return;
                }

                int ErR = DataAlreadyUsed();
                if (ErR != 0)
                {
                    return;
                }

                this.btnDelete.Enabled = false;
                this.progressBar1.Visible = true;

                backgroundWorkerDelete.RunWorkerAsync();
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
                FileLogger.Log(this.Name, "btnDelete_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnDelete_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnDelete_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnDelete_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnDelete_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnDelete_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnDelete_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnDelete_Click", exMessage);
            }
            #endregion
        }

        private int DataAlreadyUsed()
        {
            #region try

            try
            {
                CommonDAL commonDal = new CommonDAL();
                //ICommon commonDal = OrdinaryVATDesktop.GetObject<CommonDAL, CommonRepo, ICommon>(OrdinaryVATDesktop.IsWCF);

                if (commonDal.DataAlreadyUsed("Products", "CategoryID", txtCategoryID.Text.Trim(), null, null,connVM) != 0)
                {
                    MessageBox.Show(MessageVM.msgDataAlreadyUsed + " Product" + MessageVM.msgDeleteOperationterminated, this.Text);
                    return 1;
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
                FileLogger.Log(this.Name, "ErrorReturn", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "ErrorReturn", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "ErrorReturn", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "ErrorReturn", exMessage);
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
                FileLogger.Log(this.Name, "ErrorReturn", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ErrorReturn", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ErrorReturn", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "ErrorReturn", exMessage);
            }

            #endregion

            return 0;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            #region try
            try
            {

                if (ChangeData == true)
                {
                    if (DialogResult.No != MessageBox.Show(

                        "Recent changes have not been saved ." + "\n" + " Want to refresh without saving?",
                        this.Text,
                         MessageBoxButtons.YesNo,
                         MessageBoxIcon.Question,
                         MessageBoxDefaultButton.Button2))
                    {
                        PTypeSearch();
                        ClearAll();
                        ChangeData = false;
                    }
                }
                if (ChangeData == false)
                {
                    PTypeSearch();
                    ClearAll();
                    ChangeData = false;
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
                FileLogger.Log(this.Name, "btnCancel_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnCancel_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnCancel_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnCancel_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnCancel_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnCancel_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnCancel_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnCancel_Click", exMessage);
            }
            #endregion
        }

        #region TextBox KeyDown Event

        private void txtCategoryID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtCategoryName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtHSCodeNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtHSDescription_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtVATRate_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtDescription_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtComments_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        #endregion

        private void txtVATRate_Leave(object sender, EventArgs e)
        {
            //Program.FormatTextBoxRate(txtVATRate, "VAT Rate");
            txtVATRate.Text = Program.ParseDecimalObject(txtVATRate.Text.Trim()).ToString();
        }

        private void txtVATRate_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void cmbHSCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void btnSearchProductCategory_Click(object sender, EventArgs e)
        {
            // No SOAP Service

            #region try
            try
            {

                Program.fromOpen = "Me";
                string result = FormProductCategorySearch.SelectOne();
                if (result == "") { return; }
                else//if (result == ""){return;}else//if (result != "")
                {
                    string[] ProductCategoryInfo = result.Split(FieldDelimeter.ToCharArray());

                    txtCategoryID.Text = ProductCategoryInfo[0];
                    txtCategoryName.Text = ProductCategoryInfo[1];
                    txtDescription.Text = ProductCategoryInfo[2];
                    txtComments.Text = ProductCategoryInfo[3];
                    txtHSDescription.Text = ProductCategoryInfo[9];
                    txtSDRate.Text = Program.ParseDecimalObject(ProductCategoryInfo[10]);
                    cmbProductType.Text = ProductCategoryInfo[4];
                    txtHSCodeNo.Text = ProductCategoryInfo[5];
                    cmbHSCode.Text = ProductCategoryInfo[5];
                    cmbReportType.Text = ProductCategoryInfo[13];

                    txtVATRate.Text = Program.ParseDecimalObject(Convert.ToDecimal(ProductCategoryInfo[6]).ToString("0.00"));
                    if (ProductCategoryInfo[7] == "Y")
                    {
                        chkPropagationRate.Checked = true;
                    }
                    else
                    {
                        chkPropagationRate.Checked = false;
                    }
                    if (ProductCategoryInfo[8] == "Y")
                    {
                        chkActiveStatus.Checked = true;
                    }
                    else
                    {
                        chkActiveStatus.Checked = false;
                    }
                    if (ProductCategoryInfo[11] == "Y")
                    {
                        chkTrading.Checked = true;
                    }
                    else
                    {
                        chkTrading.Checked = false;
                    }
                    if (ProductCategoryInfo[12] == "Y")
                    {
                        chkNonStock.Checked = true;
                    }
                    else
                    {
                        chkNonStock.Checked = false;
                    }

                    IsUpdate = true;
                    ChangeData = false;
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
                FileLogger.Log(this.Name, "btnSearchProductCategory_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnSearchProductCategory_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnSearchProductCategory_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnSearchProductCategory_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnSearchProductCategory_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnSearchProductCategory_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnSearchProductCategory_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnSearchProductCategory_Click", exMessage);
            }
            #endregion
            finally
            {
                ChangeData = false;
            }
        }

        private void FormProductCategory_Load(object sender, EventArgs e)
        {
            #region try
            try
            {
                System.Windows.Forms.ToolTip ToolTip1 = new System.Windows.Forms.ToolTip();
                ToolTip1.SetToolTip(this.btnSearchProductCategory, "Existing Information");
                ToolTip1.SetToolTip(this.btnAddNew, "New");
                ClearAll();
                PTypeSearch();

                txtCategoryID.Text = "~~~ New ~~~";
                ChangeData = false;
                ReportTypeLoad();
                

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
                FileLogger.Log(this.Name, "FormProductCategory_Load", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "FormProductCategory_Load", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "FormProductCategory_Load", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "FormProductCategory_Load", exMessage);
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
                FileLogger.Log(this.Name, "FormProductCategory_Load", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormProductCategory_Load", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormProductCategory_Load", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "FormProductCategory_Load", exMessage);
            }
            #endregion
        }

        private void ReportTypeLoad()
        {
            CommonDAL commonDal = new CommonDAL();
            Dictionary<string, string> dicVATType = new Dictionary<string, string>();
            cmbReportType.DataSource = null;
            cmbReportType.Items.Clear();

            dicVATType = commonDal.ReportType();

            if (dicVATType != null && dicVATType.Count > 0)
            {
                cmbReportType.DataSource = new BindingSource(dicVATType, null);
                cmbReportType.DisplayMember = "Key";
                cmbReportType.ValueMember = "Value";
            }

        }

        private void txtCategoryName_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void cmbProductType_SelectedIndexChanged(object sender, EventArgs e)
        {
            chkNonStock.Checked = false;
            chkTrading.Checked = false;
            if (cmbProductType.Text == "Service")
            {
                chkNonStock.Checked = true;
            }
            if (cmbProductType.Text == "Trading")
            {
                chkTrading.Checked = true;
            }

            ChangeData = true;
        }

        #endregion

        #region Methods 03

        private void txtSDRate_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtDescription_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void txtComments_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void chkPropagationRate_CheckedChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void chkActiveStatus_CheckedChanged(object sender, EventArgs e)
        {
            ChangeData = true;
        }

        private void FormProductCategory_FormClosing(object sender, FormClosingEventArgs e)
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

        private void txtSDRate_Leave(object sender, EventArgs e)
        {
            btnAdd.Focus();
            //Program.FormatTextBoxRate(txtSDRate, "SD Rate");
            txtSDRate.Text = Program.ParseDecimalObject(txtSDRate.Text.Trim()).ToString();
        }

        private void cmbProductType_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtSDRate_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void btnAddNew_Click(object sender, EventArgs e)
        {
            #region try
            try
            {
                if (ChangeData == true)
                {
                    if (DialogResult.No != MessageBox.Show(

                        "Recent changes have not been saved ." + "\n" + "Want to add new without saving?",
                        this.Text,
                         MessageBoxButtons.YesNo,
                         MessageBoxIcon.Question,
                         MessageBoxDefaultButton.Button2))
                    {
                        PTypeSearch();
                        ClearAll();
                        txtCategoryID.Text = "~~~ New ~~~";
                        ChangeData = false;
                    }
                }
                else if (ChangeData == false)
                {
                    PTypeSearch();
                    ClearAll();
                    txtCategoryID.Text = "~~~ New ~~~";
                    ChangeData = false;
                }
                IsUpdate = false;
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

        }

        private void chkTrading_Click(object sender, EventArgs e)
        {
            #region try
            try
            {
                if (chkTrading.Checked == true)
                {
                    if (chkNonStock.Checked == true)
                    {

                        MessageBox.Show("Product should be either Trading or NonStock");
                        chkTrading.Checked = false;
                        return;
                    }
                }
                ChangeData = true;
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
                FileLogger.Log(this.Name, "chkTrading_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "chkTrading_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "chkTrading_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "chkTrading_Click", exMessage);
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
                FileLogger.Log(this.Name, "chkTrading_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "chkTrading_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "chkTrading_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "chkTrading_Click", exMessage);
            }
            #endregion
        }

        private void chkNonStock_Click(object sender, EventArgs e)
        {
            #region try
            try
            {
                if (chkNonStock.Checked == true)
                {
                    if (chkTrading.Checked == true)
                    {
                        MessageBox.Show("Product should be either Trading or NonStock");
                        chkNonStock.Checked = false;
                        return;
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
                FileLogger.Log(this.Name, "chkNonStock_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "chkNonStock_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "chkNonStock_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "chkNonStock_Click", exMessage);
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
                FileLogger.Log(this.Name, "chkNonStock_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "chkNonStock_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "chkNonStock_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "chkNonStock_Click", exMessage);
            }
            #endregion
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            #region try
            try
            {
                if (Program.CheckLicence(DateTime.Now) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }
                FormRptProductCategoryInformation frmRptProductCategoryInformation = new FormRptProductCategoryInformation();
                frmRptProductCategoryInformation.txtCategoryName.Text = txtCategoryName.Text.Trim();
                frmRptProductCategoryInformation.ShowDialog();
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

        #region backgroundWorker Event

        private void backgroundWorkerAdd_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                SAVE_DOWORK_SUCCESS = false;
                sqlResults = new string[3];
                //ProductCategoryDAL productCategoryDal = new ProductCategoryDAL();
                IProductCategory productCategoryDal = OrdinaryVATDesktop.GetObject<ProductCategoryDAL, ProductCategoryRepo, IProductCategory>(OrdinaryVATDesktop.IsWCF);

                var tt = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                //sqlResults = productCategoryDal.InsertToProductCategory(NextID,
                //    txtCategoryName.Text.Trim(),
                //    txtDescription.Text.Trim(),
                //    txtComments.Text.Trim(),
                //    type,
                //    txtHSCodeNo.Text.Trim(),
                //    Convert.ToDecimal(txtVATRate.Text.Trim()),
                //    propergationRate, activeStatus,
                //    Program.CurrentUser,
                //    DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss"),
                //    Program.CurrentUser,
                //    DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss"),
                //    Convert.ToDecimal(txtSDRate.Text.Trim()),
                //    trading,
                //    nonStock,
                //    Program.DatabaseName);  // Change 04

                ProductCategoryVM vm = new ProductCategoryVM();

                vm.CategoryID = NextID.ToString();
                vm.CategoryName = txtCategoryName.Text.Trim();
                vm.Description = txtDescription.Text.Trim();
                vm.Comments = txtComments.Text.Trim();
                vm.ReportType = reportype;
                vm.IsRaw = type.ToString();
                vm.HSCodeNo = txtHSCodeNo.Text.Trim();
                vm.VATRate = Convert.ToDecimal(txtVATRate.Text.Trim());
                vm.PropergatingRate = propergationRate.ToString();
                vm.ActiveStatus = activeStatus.ToString();
                vm.CreatedBy = Program.CurrentUser;
                vm.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                vm.SD = Convert.ToDecimal(txtSDRate.Text.Trim());
                vm.Trading = trading.ToString();
                vm.NonStock = nonStock.ToString();

                sqlResults = productCategoryDal.InsertToProductCategory(vm,connVM);

                SAVE_DOWORK_SUCCESS = true;

                // End DoWork
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
                FileLogger.Log(this.Name, "backgroundWorkerAdd_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerAdd_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerAdd_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "backgroundWorkerAdd_DoWork", exMessage);
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
                FileLogger.Log(this.Name, "backgroundWorkerAdd_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerAdd_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerAdd_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "backgroundWorkerAdd_DoWork", exMessage);
            }
            #endregion
        }

        private void backgroundWorkerAdd_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (SAVE_DOWORK_SUCCESS)
                {
                    if (sqlResults.Length > 0)
                    {
                        string result = sqlResults[0];
                        string message = sqlResults[1];
                        string newId = sqlResults[2];
                        if (string.IsNullOrEmpty(result))
                        {
                            throw new ArgumentNullException("bgwSave_RunWorkerCompleted", "Unexpected error.");
                        }
                        else if (result == "Success" || result == "Fail")
                        {
                            MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            txtCategoryID.Text = newId;
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
                FileLogger.Log(this.Name, "backgroundWorkerAdd_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerAdd_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerAdd_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "backgroundWorkerAdd_RunWorkerCompleted", exMessage);
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
                FileLogger.Log(this.Name, "backgroundWorkerAdd_RunWorkerCompleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerAdd_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerAdd_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "backgroundWorkerAdd_RunWorkerCompleted", exMessage);
            }
            #endregion
            finally
            {
                ChangeData = false;
                this.btnAdd.Enabled = true;
                this.progressBar1.Visible = false;
            }

        }

        private void backgroundWorkerDelete_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                DELETE_DOWORK_SUCCESS = false;
                sqlResults = new string[3];

                //ProductCategoryDAL productCategoryDal = new ProductCategoryDAL();
                IProductCategory productCategoryDal = OrdinaryVATDesktop.GetObject<ProductCategoryDAL, ProductCategoryRepo, IProductCategory>(OrdinaryVATDesktop.IsWCF);


                ProductCategoryVM vm = new ProductCategoryVM();
                vm.LastModifiedBy = Program.CurrentUser;
                vm.LastModifiedOn = DateTime.Now.ToString();
                string[] ids = new string[] { txtCategoryID.Text.Trim(), "" };
                sqlResults = productCategoryDal.Delete(vm, ids,null,null,connVM);
                //sqlResults = productCategoryDal.DeleteCategoryInformation(txtCategoryID.Text.Trim(), Program.DatabaseName);

                DELETE_DOWORK_SUCCESS = true;

                // End DoWork

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
                FileLogger.Log(this.Name, "backgroundWorkerDelete_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerDelete_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerDelete_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "backgroundWorkerDelete_DoWork", exMessage);
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
                FileLogger.Log(this.Name, "backgroundWorkerDelete_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerDelete_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerDelete_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "backgroundWorkerDelete_DoWork", exMessage);
            }
            #endregion
        }

        private void backgroundWorkerDelete_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (DELETE_DOWORK_SUCCESS)
                {
                    if (sqlResults.Length > 0)
                    {
                        string result = sqlResults[0];
                        string message = sqlResults[1];
                        string newId = sqlResults[2];
                        if (string.IsNullOrEmpty(result))
                        {
                            throw new ArgumentNullException("backgroundWorkerDelete_RunWorkerCompleted", "Unexpected error.");
                        }
                        else if (result == "Success" || result == "Fail")
                        {
                            MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        ClearAll();
                        IsUpdate = false;


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
                FileLogger.Log(this.Name, "backgroundWorkerDelete_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerDelete_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerDelete_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "backgroundWorkerDelete_RunWorkerCompleted", exMessage);
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
                FileLogger.Log(this.Name, "backgroundWorkerDelete_RunWorkerCompleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerDelete_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerDelete_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "backgroundWorkerDelete_RunWorkerCompleted", exMessage);
            }
            #endregion
            finally
            {
                ChangeData = false;
                this.btnDelete.Enabled = true;
                this.progressBar1.Visible = false;

            }

        }

        private void backgroundWorkerUpdate_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {

                UPDATE_DOWORK_SUCCESS = false;
                sqlResults = new string[3];

                //ProductCategoryDAL productCategoryDal = new ProductCategoryDAL();
                IProductCategory productCategoryDal = OrdinaryVATDesktop.GetObject<ProductCategoryDAL, ProductCategoryRepo, IProductCategory>(OrdinaryVATDesktop.IsWCF);
                
                //sqlResults = productCategoryDal.UpdateProductCategory(NextID,
                //    txtCategoryName.Text.Trim(),
                //    txtDescription.Text.Trim(),
                //    txtComments.Text.Trim(),
                //    type,
                //    txtHSCodeNo.Text.Trim(),
                //    Convert.ToDecimal(txtVATRate.Text.Trim()),
                //    propergationRate, activeStatus,
                //    Program.CurrentUser,
                //    DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss"),
                //    Program.CurrentUser,
                //    DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss"),
                //    Convert.ToDecimal(txtSDRate.Text.Trim()),
                //    trading,
                //    nonStock,
                //    Program.DatabaseName);

                ProductCategoryVM vm = new ProductCategoryVM();

                vm.CategoryID = NextID.ToString();
                vm.CategoryName = txtCategoryName.Text.Trim();
                vm.Description = txtDescription.Text.Trim();
                vm.Comments = txtComments.Text.Trim();
                vm.ReportType = reportype;
                vm.IsRaw = type.ToString();
                vm.HSCodeNo = txtHSCodeNo.Text.Trim();
                vm.VATRate = Convert.ToDecimal(txtVATRate.Text.Trim());
                vm.PropergatingRate = propergationRate.ToString();
                vm.ActiveStatus = activeStatus.ToString();
                vm.LastModifiedBy = Program.CurrentUser;
                vm.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                vm.SD = Convert.ToDecimal(txtSDRate.Text.Trim());
                vm.Trading = trading.ToString();
                vm.NonStock = nonStock.ToString();

                sqlResults = productCategoryDal.UpdateProductCategory(vm,connVM);

                UPDATE_DOWORK_SUCCESS = true;

                // End DoWork

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
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerUpdate_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "backgroundWorkerUpdate_DoWork", exMessage);
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
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_DoWork", exMessage);
            }
            #endregion
        }

        private void backgroundWorkerUpdate_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {

                if (UPDATE_DOWORK_SUCCESS)
                {
                    if (sqlResults.Length > 0)
                    {
                        string result = sqlResults[0];
                        string message = sqlResults[1];
                        string newId = sqlResults[2];
                        if (string.IsNullOrEmpty(result))
                        {
                            throw new ArgumentNullException("backgroundWorkerUpdate_RunWorkerCompleted", "Unexpected error.");
                        }
                        else if (result == "Success" || result == "Fail")
                        {
                            if (result == "Success")
                            {
                                ProductVM pvm = new ProductVM();
                                pvm.CategoryID = newId;
                                pvm.ReportType = reportype;
                                pvm.LastModifiedBy = Program.CurrentUser;
                                pvm.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");

                                ProductDAL pDal = new ProductDAL();

                                pDal.UpdateToExitProduct(pvm,null, null, connVM);
                            }
                            

                            MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            txtCategoryID.Text = newId;
                        }
                        IsUpdate = false;


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
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerUpdate_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "backgroundWorkerUpdate_RunWorkerCompleted", exMessage);
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
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_RunWorkerCompleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_RunWorkerCompleted", exMessage);
            }
            #endregion
            finally
            {
                ChangeData = false;
                this.btnUpdate.Enabled = true;
                this.progressBar1.Visible = false;
            }
        }

        #endregion

        #endregion

        #region Methods 04

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void btn12Law_Click(object sender, EventArgs e)
        {
            Form12Law frm12Law = new Form12Law();
            frm12Law.Show();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            FormBranchProfiles frmBranchProfilesInformation = new FormBranchProfiles();



            frmBranchProfilesInformation.Show();
        }

        private void TDSs_Click(object sender, EventArgs e)
        {
            FormTDSs frmTDSs = new FormTDSs();



            frmTDSs.Show();
        }

        private void btnToll6_3Invoice_Click(object sender, EventArgs e)
        {

           

        }

        private void button2_Click(object sender, EventArgs e)
        {
            FormSCBLMIS frmSCBLMIS = new FormSCBLMIS();
            frmSCBLMIS.ShowDialog();
        }

        private void DepositTDS_Click(object sender, EventArgs e)
        {
            FormDepositTDS frmDepositTDS = new FormDepositTDS();
            frmDepositTDS.rbtnTDS.Checked = true;
            frmDepositTDS.ShowDialog();
        }

        private void HSCode_Click(object sender, EventArgs e)
        {
            FormReportSCBL_Production frmHsCode = new FormReportSCBL_Production();
            //FormRptReconsciliation frmHsCode = new FormRptReconsciliation();
            frmHsCode.ShowDialog();
        }

        private void btnDisposeRaw_Click(object sender, EventArgs e)
        {
            FormDisposeRaw frmDisposeRaws = new FormDisposeRaw();
            frmDisposeRaws.Show();
        }

        ////To be replaced soon
        private void btnPreview_Click(object sender, EventArgs e)
        {
            try
            {
                DataSet ReportResult = new DataSet();

                ReportDSDAL reportDsdal = new ReportDSDAL();
                //IReport productCategoryDal = OrdinaryVATDesktop.GetObject<ReportDSDAL, ReportDSRepo, IReport>(OrdinaryVATDesktop.IsWCF);


                ReportResult = reportDsdal.InputValue7_5percent("2021-01-01 00:00:00","2022-01-01 23:59:59",Program.BranchId,connVM);
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

        private void btnNotes_Click(object sender, EventArgs e)
        {
            FormCPCDetails frm = new FormCPCDetails();
            //#region Roll
            //DataTable dt = UserMenuRolls("170130110");

            //if (dt != null && dt.Rows.Count > 0)
            //{

            //    if (dt.Rows[0]["Access"].ToString() != "1")
            //    {
            //        MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            //        return;
            //    }
            //    frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
            //    frm.bthUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
            //    frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            //}
            //else
            //{
            //    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    return;
            //}

            //#endregion Roll
            //frm.MdiParent = this;
            frm.Show();
        }

        #endregion

        #region Methods 05

        private void btnSaleComp_Click(object sender, EventArgs e)
        {
            try
            {
                IntegrationComp formComp = new IntegrationComp();

                formComp.ShowDialog();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void btnRawIssue_Click(object sender, EventArgs e)
        {
            FormSale frm = new FormSale();
            frm.rbtnContractorRawIssue.Checked = true;

            //FormRptReconsciliation frmHsCode = new FormRptReconsciliation();
            frm.ShowDialog();
        }

        private void btnRawReceive_Click(object sender, EventArgs e)
        {
            FormPurchase frm = new FormPurchase();
            frm.rbtnClientRawReceive.Checked = true;

            //FormRptReconsciliation frmHsCode = new FormRptReconsciliation();
            frm.ShowDialog();
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

        private void btnFGReceive_Click(object sender, EventArgs e)
        {
            FormPurchase frm = new FormPurchase();
            frm.rbtnClientFGReceiveWOBOM.Checked = true;

            //FormRptReconsciliation frmHsCode = new FormRptReconsciliation();
            frm.ShowDialog();
        }

        private void btnClient63_Click(object sender, EventArgs e)
        {
            FormClient6_3 frm = new FormClient6_3();
            ////frm.rbtnClient63.Checked = true;
            frm.ShowDialog();

        }

        //private void btnCustomerDiscount_Click(object sender, EventArgs e)
        //{
        private void btnDisposeFinish_Click(object sender, EventArgs e)
        {
            FormDisposeFinishNew frm = new FormDisposeFinishNew();
            frm.Show();
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


        #endregion

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

        private void btnChargeCodes_Click(object sender, EventArgs e)
        {
            try
            {
                FormIASChargeCode chargeCode = new FormIASChargeCode();

                chargeCode.ShowDialog();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
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

        private void button2_Click_1(object sender, EventArgs e)
        {
            try
            {
               // From frm = new FormProductItemSearch();
                FormAVGPriceDownload frm = new FormAVGPriceDownload();
               
               // frm.rbtnBankChannelPayment.Checked = true;
                frm.Show();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            FormPurchase frm = new FormPurchase();
            //#region Roll
            //DataTable dt = UserMenuRolls("120110130");


            //if (dt != null && dt.Rows.Count > 0)
            //{

            //    if (dt.Rows[0]["Access"].ToString() != "1")
            //    {
            //        MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            //        return;
            //    }
            //    frm.btnSave.Enabled = dt.Rows[0]["AddAccess"].ToString() == "1";
            //    frm.bthUpdate.Enabled = dt.Rows[0]["EditAccess"].ToString() == "1";
            //    frm.btnPost.Enabled = dt.Rows[0]["PostAccess"].ToString() == "1";

            //}
            //else
            //{
            //    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    return;
            //}

            //#endregion Roll

           // frm.MdiParent = this;
            frm.rbtnPurchaseTollcharge.Checked = true;
            //frm.rbtnInputService.Checked = true;
            frm.Show();
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

        private void btnSetting_Click(object sender, EventArgs e)
        {
            FormSettingMaster frm = new FormSettingMaster();
            frm.rbtnSetting.Checked = true;
            frm.Show();
        }

        private void btnProcess_Click(object sender, EventArgs e)
        {
            FormRegularProcess process = new FormRegularProcess();
            process.Show();
        }

        private void btnPane_Click(object sender, EventArgs e)
        {
            try
            {
                FormButtonPane buttonPane = new FormButtonPane();
                buttonPane.Show();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }     

    }
}
