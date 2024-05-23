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
    public partial class FormPurchaseOtherTDSPopUp : Form
    {
        #region Constructors

        public FormPurchaseOtherTDSPopUp()
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
            FormPurchaseOtherTDSPopUp frmSalePrint = new FormPurchaseOtherTDSPopUp();
            try
            {
                #region CPC Load
                string[] Condition = new string[] { "Type='Purchase'" };
                //frmSalePrint.cmbCPCName = new CommonDAL().ComboBoxLoad(frmSalePrint.cmbCPCName, "CPCDetails", "Code", "Name", Condition, "varchar", false, false, frmSalePrint.connVM, true);
                //frmSalePrint.cmbCPCName.Text = "Select";

                #endregion

                //frmSalePrint.ExpireDateTracking = vm.ExpireDateTracking;
                //frmSalePrint.TransactionType = vm.TransactionType;
                frmSalePrint.txtTDSCode.Text = vm.TDSCode;
                frmSalePrint.txtSection.Text = vm.Section;
                frmSalePrint.ShowDialog();

                PopUpVM = frmSalePrint.SelectedValueVM;

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

                PurchasePopUp PVM = new PurchasePopUp();

                //PVM.CPCName = cmbCPCName.Text.ToLower() == "select" ? "-" : cmbCPCName.Text; ;
                //PVM.ItemNo = txtItemNo.Text.Trim()==""? "-": txtItemNo.Text.Trim();
                //PVM.FixedVATRebate = cmpFixedVATRebate.Text;
                PVM.Section = txtSection.Text;
                PVM.TDSCode = txtTDSCode.Text;

                SelectedValueVM = PVM;
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
            //if(ExpireDateTracking=="Y")
            //{
            //    lblExpireDate.Visible = true;
            //    dptExpireDate.Visible = true;
            //}
            //else
            //{
            //    lblExpireDate.Visible = false;
            //    dptExpireDate.Visible = false;
            //}
            //if(TransactionType.ToLower()=="import")
            //{
            //    //label1.Visible = true;
            //    //cmbCPCName.Visible = true;
            //    //label2.Visible = true;
            //    //txtItemNo.Visible = true;
            //}
            //else
            //{
            //    //label1.Visible = false;
            //    //cmbCPCName.Visible = false;
            //    //label2.Visible = false;
            //    //txtItemNo.Visible = false;
            //}
            //Claculation();
        }

        private void FormDN_CNAdjustment_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                PurchasePopUp PVM = new PurchasePopUp();
                PVM.TDSCode = txtTDSCode.Text.Trim() == "" ? "-" : txtTDSCode.Text.Trim();
                PVM.Section = txtSection.Text.Trim() == "" ? "-" : txtSection.Text.Trim();

                SelectedValueVM = PVM;
            }
            catch (Exception ex)
            {

            }
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void btnSearchTDS_Click(object sender, EventArgs e)
        {
            #region try
            try
            {
                DataGridViewRow selectedRow = null;
                //vBranchId = "0";
                Program.fromOpen = "Me";
                //string result = FromBranchProfilesSearch.SelectOne();
                selectedRow = FormTDSsSearch.SelectOne();
                if (selectedRow != null && selectedRow.Selected == true)
                {
                    txtTDSCode.Text = selectedRow.Cells["Code"].Value.ToString();
                    //txtId.Text = selectedRow.Cells["Id"].Value.ToString();
                    //txtDescription.Text = selectedRow.Cells["Discription"].Value.ToString();
                    txtDescription.Text = selectedRow.Cells["Description"].Value.ToString();
                    //txtMinValue.Text = Program.ParseDecimalObject(selectedRow.Cells["MinValue"].Value.ToString());
                    //txtMaxValue.Text = Program.ParseDecimalObject(selectedRow.Cells["MaxValue"].Value.ToString());
                    //txtRate.Text = Program.ParseDecimalObject(selectedRow.Cells["Rate"].Value.ToString());
                    txtSection.Text = selectedRow.Cells["Section"].Value.ToString();
                    //txtComments.Text = selectedRow.Cells["Comments"].Value.ToString();

                }
                //ChangeData = false;



                if (result == "")
                {
                    return;
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
                FileLogger.Log(this.Name, "btnVendorGroup_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnVendorGroup_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnVendorGroup_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnVendorGroup_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnVendorGroup_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnVendorGroup_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnVendorGroup_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnVendorGroup_Click", exMessage);
            }
            #endregion
        }

        private void btnAddNew_Click(object sender, EventArgs e)
        {
            ClearALL();
            //ChangeData = false;
        }
        private void ClearALL()
        {
            //DateTime vdateTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss"));


            txtTDSCode.Text = "";
            txtDescription.Text = "";
            txtSection.Text = "";



        }

       



    }
}
