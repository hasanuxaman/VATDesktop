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
////
using System.Security.Cryptography;
using System.IO;
using SymphonySofttech.Utilities;
using VATClient.ReportPreview;
using VATServer.Library;
using VATViewModel.DTOs;
using VATServer.Interface;
using VATDesktop.Repo;
using VATServer.Ordinary;

namespace VATClient
{
    public partial class FormPurchaseOtherPopUp : Form
    {
        #region Constructors

        public FormPurchaseOtherPopUp()
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

        private const string LineDelimeter = DBConstant.LineDelimeter;
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private static string PassPhrase = DBConstant.PassPhrase;
        private static string EnKey = DBConstant.EnKey;
        private string NextID = string.Empty;
        public string VFIN = "151";
        //private string SelectedValue = string.Empty;
        PurchasePopUp SelectedValueVM = new PurchasePopUp();
        private string ExpireDateTracking = string.Empty;
        private string TransactionType = string.Empty;


        #region Global Variables for BackgroundWorker

        private string result = string.Empty;

        #endregion


        #endregion

        #region Methods


        private void ClearAll()
        {
            try
            {
               
               
            }
            #region Catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "ClearAll", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "ClearAll", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "ClearAll", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "ClearAll", exMessage);
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
                FileLogger.Log(this.Name, "ClearAll", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ClearAll", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ClearAll", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "ClearAll", exMessage);
            }
            #endregion Catch
        }

        #endregion
        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
        public static PurchasePopUp SelectOne(PurchasePopUp vm)
        {
            //string SearchValue = string.Empty;
            PurchasePopUp PopUpVM = new PurchasePopUp();
            FormPurchaseOtherPopUp frmSalePrint = new FormPurchaseOtherPopUp();
            try
            {
                #region CPC Load
                string[] Condition = new string[] { "Type='Purchase'" };
                frmSalePrint.cmbCPCName = new CommonDAL().ComboBoxLoad(frmSalePrint.cmbCPCName, "CPCDetails", "Code", "Name", Condition, "varchar", false, false, frmSalePrint.connVM, true);
                frmSalePrint.cmbCPCName.Text = "Select";

                #endregion


                frmSalePrint.txtProductName.Text = vm.ProductName;
                frmSalePrint.txtCode.Text = vm.ProductCode;
                frmSalePrint.cmbCPCName.Text = vm.CPCName;
                frmSalePrint.dptExpireDate.Text = vm.ExpireDate;
                frmSalePrint.txtItemNo.Text = vm.ItemNo;
                frmSalePrint.ExpireDateTracking = vm.ExpireDateTracking;
                frmSalePrint.TransactionType = vm.TransactionType;
                frmSalePrint.cmpFixedVATRebate.Text = vm.FixedVATRebate;

                frmSalePrint.chkSection21.Checked = vm.Section21 == "Y";

                frmSalePrint.ShowDialog();

                PopUpVM = frmSalePrint.SelectedValueVM;

                if (string.IsNullOrEmpty(PopUpVM.CPCName))
                {
                    PopUpVM.CPCName = vm.ProductName;
                }
                if (string.IsNullOrEmpty(PopUpVM.ItemNo))
                {
                    PopUpVM.ItemNo = vm.ItemNo;
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
                MessageBox.Show(ex.Message, "FormDN_CNAdjustment", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("FormDN_CNAdjustment", "SelectOne", exMessage);
            }
            #endregion Catch
            return PopUpVM;

        }
        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {

                //PurchasePopUp PVM = new PurchasePopUp();

                //PVM.CPCName = cmbCPCName.Text.ToLower() == "select" ? "-" : cmbCPCName.Text; ;
                //PVM.ExpireDate = ExpireDateTracking == "Y"
                //    ? dptExpireDate.Value.ToString("yyyy-MM-dd")
                //    : "2100-01-01";
                //PVM.ItemNo = txtItemNo.Text.Trim()==""? "-": txtItemNo.Text.Trim();
                //PVM.FixedVATRebate = cmpFixedVATRebate.Text;
                //PVM.Section21 = chkSection21.Checked ? "Y" : "N";

                //SelectedValueVM = PVM;


                GetFormData();

                this.Close();
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


     
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ClearAll();
        }

      

        private void FormDN_CNAdjustment_Load(object sender, EventArgs e)
        {
            if(ExpireDateTracking=="Y")
            {
                lblExpireDate.Visible = true;
                dptExpireDate.Visible = true;
            }
            else
            {
                lblExpireDate.Visible = false;
                dptExpireDate.Visible = false;
            }
            if(TransactionType.ToLower()=="import")
            {
                label1.Visible = true;
                cmbCPCName.Visible = true;
                label2.Visible = true;
                txtItemNo.Visible = true;
            }
            else if (TransactionType.ToLower() == "inputserviceimport")
            {

                label1.Visible = true;
                cmbCPCName.Visible = true;
                label2.Visible = true;
                txtItemNo.Visible = true;

            }
            else
            {
                label1.Visible = false;
                cmbCPCName.Visible = false;
                label2.Visible = false;
                txtItemNo.Visible = false;
            }
            //Claculation();
        }

        private void FormDN_CNAdjustment_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                GetFormData();
            }
            catch (Exception ex)
            {

            }
        }

        private void GetFormData()
        {
            PurchasePopUp PVM = new PurchasePopUp();

            PVM.CPCName = cmbCPCName.Text.ToLower() == "select" ? "-" : cmbCPCName.Text;
            ;
            PVM.ExpireDate = ExpireDateTracking == "Y"
                ? dptExpireDate.Value.ToString("yyyy-MM-dd")
                : "2100-01-01";
            PVM.ItemNo = txtItemNo.Text.Trim() == "" ? "-" : txtItemNo.Text.Trim();
            PVM.FixedVATRebate = cmpFixedVATRebate.Text;
            PVM.Section21 = chkSection21.Checked ? "Y" : "N";
            SelectedValueVM = PVM;
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

       



    }
}
