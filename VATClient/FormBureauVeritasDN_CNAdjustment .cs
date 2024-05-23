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
    public partial class FormBureauVeritasDN_CNAdjustment  : Form
    {
        #region Constructors

        public FormBureauVeritasDN_CNAdjustment()
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
        private bool ChangeData = false;
        private string NextID = string.Empty;
        public string VFIN = "151";
        private bool IsUpdate = false;
        private string SelectedValue = string.Empty;
        private bool IsNull = false;
        bool ApplyAll = false;


        #region Global Variables for BackgroundWorker

        private string result = string.Empty;

        #endregion

        #region sql save, update, delete

        private string[] sqlResults;
        private bool SAVE_DOWORK_SUCCESS = false;
        private bool UPDATE_DOWORK_SUCCESS = false;
        private bool DELETE_DOWORK_SUCCESS = false;

        #endregion

        #endregion

        #region Methods


        private void ClearAll()
        {
            try
            {
                txtInvoiceNo.Text = "";
                txtReason.Text = "";
                txtUom.Text = "";
                txtSDAmount.Text = "";
                txtSD.Text = "";
                txtVATRate.Text = "";
                txtVATAmount.Text = "";
                txtSubTotal.Text = "";
                txtQuantity.Text = "";
                txtNBRPrice.Text = "";
               
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
        private void Claculation()
        {
            decimal Subtotal =0;
            decimal SDAmount = 0;
            decimal VATAmount = 0;
            Subtotal = Convert.ToDecimal(txtQuantity.Text.Trim()) * Convert.ToDecimal(txtNBRPrice.Text.Trim());
            SDAmount = (Subtotal * Convert.ToDecimal(txtSD.Text.Trim())/ 100);
            VATAmount=((Subtotal +SDAmount)*Convert.ToDecimal(txtVATRate.Text.Trim())/100);
            txtSubTotal.Text =Convert.ToString( Subtotal);
            txtVATAmount.Text = Convert.ToString(VATAmount);
            txtSDAmount.Text = Convert.ToString(SDAmount);
        }
        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
        public static string SelectOne(DataGridViewRow row)
        {
            string SearchValue = string.Empty;
            FormBureauVeritasDN_CNAdjustment frmSalePrint = new FormBureauVeritasDN_CNAdjustment();
            try
            {

          //  frmSalePrint.txtInvoiceNo.Text = row.Cells["PreviousSalesInvoiceNo"].Value.ToString();
            //frmSalePrint.txtInvoiceNo.Text = row.Cells["PreviousSalesInvoiceNo"].Value == null ? "" : row.Cells["PreviousSalesInvoiceNo"].Value.ToString();
            //frmSalePrint.dtpInvoiceDate.Value =Convert.ToDateTime((row.Cells["PreviousInvoiceDateTime"].Value.ToString()));
            //frmSalePrint.txtNBRPrice.Text = row.Cells["PreviousNBRPrice"].Value.ToString();
            //frmSalePrint.txtQuantity.Text = row.Cells["PreviousQuantity"].Value.ToString();
            //frmSalePrint.txtSubTotal.Text = row.Cells["PreviousSubTotal"].Value.ToString();
            //frmSalePrint.txtVATAmount.Text = row.Cells["PreviousVATAmount"].Value.ToString();
            //frmSalePrint.txtVATRate.Text = row.Cells["PreviousVATRate"].Value.ToString();
            //frmSalePrint.txtSD.Text = row.Cells["PreviousSD"].Value.ToString();
            //frmSalePrint.txtSDAmount.Text = row.Cells["PreviousSDAmount"].Value.ToString();
            //frmSalePrint.txtUom.Text = row.Cells["PreviousUOM"].Value.ToString();
            //frmSalePrint.txtReason.Text = row.Cells["ReasonOfReturn"].Value.ToString();
            //frmSalePrint.txtProductName.Text = row.Cells["ItemName"].Value.ToString();
            //frmSalePrint.txtCode.Text = row.Cells["PCode"].Value.ToString();
            frmSalePrint.txtInvoiceNo.Text = row.Cells["PreviousSalesInvoiceNo"].Value == null ? "" : row.Cells["PreviousSalesInvoiceNo"].Value.ToString();
            frmSalePrint.dtpInvoiceDate.Value = Convert.ToDateTime((row.Cells["PreviousInvoiceDateTime"].Value==null? DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss") : row.Cells["PreviousInvoiceDateTime"].Value.ToString()));
            frmSalePrint.txtNBRPrice.Text = row.Cells["PreviousNBRPrice"].Value==null ? "0" : row.Cells["PreviousNBRPrice"].Value.ToString();
            frmSalePrint.txtQuantity.Text = row.Cells["PreviousQuantity"].Value==null ? "0" :  row.Cells["PreviousQuantity"].Value.ToString();
            frmSalePrint.txtSubTotal.Text = row.Cells["PreviousSubTotal"].Value==null ? "0" : row.Cells["PreviousSubTotal"].Value.ToString();
            frmSalePrint.txtVATAmount.Text = row.Cells["PreviousVATAmount"].Value==null ? "0" : row.Cells["PreviousVATAmount"].Value.ToString();
            frmSalePrint.txtVATRate.Text = row.Cells["PreviousVATRate"].Value==null ? "0" :row.Cells["PreviousVATRate"].Value.ToString();
            frmSalePrint.txtSD.Text = row.Cells["PreviousSD"].Value==null ? "0" :row.Cells["PreviousSD"].Value.ToString();
            frmSalePrint.txtSDAmount.Text = row.Cells["PreviousSDAmount"].Value==null ?"0" : row.Cells["PreviousSDAmount"].Value.ToString();
            frmSalePrint.txtUom.Text = row.Cells["PreviousUOM"].Value==null ? "Pcs" : row.Cells["PreviousUOM"].Value.ToString();
            frmSalePrint.txtReason.Text = row.Cells["ReasonOfReturn"].Value == null ? "NA" : row.Cells["ReasonOfReturn"].Value.ToString();
           // frmSalePrint.txtProductName.Text = row.Cells["ItemName"].Value.ToString();
           // frmSalePrint.txtCode.Text = row.Cells["PCode"].Value.ToString();
            //frmSalePrint.chkReturn.Checked =false ;
            //frmSalePrint.chkInvoiceNo.Checked =false ;
            //frmSalePrint.chkInvoiceDate.Checked =false;

           

            //FormDN_CNAdjustment frmSalePrint = new FormDN_CNAdjustment();

                frmSalePrint.ShowDialog();
                SearchValue = frmSalePrint.SelectedValue;
                if (SearchValue=="")
                {

                    SearchValue = row.Cells["PreviousSalesInvoiceNo"].Value.ToString() + FieldDelimeter
                                + Convert.ToDateTime((row.Cells["PreviousInvoiceDateTime"].Value.ToString())) + FieldDelimeter
                                + row.Cells["PreviousNBRPrice"].Value.ToString() + FieldDelimeter
                                + row.Cells["PreviousQuantity"].Value.ToString() + FieldDelimeter
                                + row.Cells["PreviousSubTotal"].Value.ToString() + FieldDelimeter
                                + row.Cells["PreviousVATAmount"].Value.ToString()+FieldDelimeter
                                + row.Cells["PreviousVATRate"].Value.ToString() + FieldDelimeter
                                + row.Cells["PreviousSD"].Value.ToString() + FieldDelimeter
                                + row.Cells["PreviousSDAmount"].Value.ToString() + FieldDelimeter
                                + row.Cells["PreviousUOM"].Value.ToString() + FieldDelimeter
                                + row.Cells["ReasonOfReturn"].Value.ToString()
                        
                                ;
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
            return SearchValue;

        }
        private void NullCheck()
        {

            IsNull = false;

            if (txtInvoiceNo.Text.Trim()=="")
                {
                    MessageBox.Show("Please Input Invoice No");
                    txtInvoiceNo.Focus();
                    IsNull=true;
                }

            if (Convert.ToDecimal(txtNBRPrice.Text) <= 0)
            {
                MessageBox.Show("Please declare the NBR Price");
                txtNBRPrice.Focus();
                IsNull = true;
                
            }

            if (Convert.ToDecimal(txtQuantity.Text) <= 0)
            {
                MessageBox.Show("Please Input Quantity");
                txtQuantity.Focus();
                IsNull = true;
            }

            if (txtVATRate.Text.Trim() == "")
            {
                MessageBox.Show("Please Input VAT Rate");
                txtVATRate.Focus();
                IsNull = true;
            }
            if (txtUom.Text.Trim() == "")
            {
                MessageBox.Show("Please Input Uom");
                txtUom.Focus();
                IsNull = true;
            }
            if (txtSD.Text.Trim() == "")
            {
                MessageBox.Show("Please Input SD ");
                txtUom.Focus();
                IsNull = true;
            }
            

           

            if (txtReason.Text.Trim() == "")
            {
                MessageBox.Show("Please Input Reason Of Return ");
                txtUom.Focus();
                IsNull = true;
            }
            
        }
        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                NullCheck();
                Claculation();
                string results = String.Empty;
                results = txtInvoiceNo.Text.ToString() + FieldDelimeter + dtpInvoiceDate.Text.ToString() + FieldDelimeter + txtNBRPrice.Text.ToString()
                  + FieldDelimeter + txtQuantity.Text.ToString() + FieldDelimeter + txtSubTotal.Text.ToString()
                  + FieldDelimeter + txtVATAmount.Text.ToString() + FieldDelimeter + txtVATRate.Text.ToString() + FieldDelimeter + txtSD.Text.ToString()
                  + FieldDelimeter + txtSDAmount.Text.ToString() + FieldDelimeter + txtUom.Text.ToString() + FieldDelimeter + txtReason.Text.ToString();
                SelectedValue = results;
                if (IsNull != true)
                {
                    this.Close();

                }
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

        private void txtNBRPrice_Leave(object sender, EventArgs e)
        {

            txtNBRPrice.Text = Program.ParseDecimalObject(txtNBRPrice.Text.Trim()).ToString();

        }

        private void txtQuantity_Leave(object sender, EventArgs e)
        {

            txtQuantity.Text = Program.ParseDecimalObject(txtQuantity.Text.Trim()).ToString();

        }

        private void txtVATRate_Leave(object sender, EventArgs e)
        {

            txtVATRate.Text = Program.ParseDecimalObject(txtVATRate.Text.Trim()).ToString();

        }

        private void txtVATAmount_Leave(object sender, EventArgs e)
        {
            txtVATAmount.Text = Program.ParseDecimalObject(txtVATAmount.Text.Trim()).ToString();

        }

        private void txtSD_Leave(object sender, EventArgs e)
        {
            txtSD.Text = Program.ParseDecimalObject(txtSD.Text.Trim()).ToString();

        }

        private void txtSDAmount_Leave(object sender, EventArgs e)
        {
            txtSDAmount.Text = Program.ParseDecimalObject(txtSDAmount.Text.Trim()).ToString();

        }

        private void txtSubTotal_Leave(object sender, EventArgs e)
        {
            txtSubTotal.Text = Program.ParseDecimalObject(txtSubTotal.Text.Trim()).ToString();

        }

        private void FormDN_CNAdjustment_Load(object sender, EventArgs e)
        {
            //Claculation();
        }

        private void FormDN_CNAdjustment_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                NullCheck();
                Claculation();
                string results = String.Empty;
                results = txtInvoiceNo.Text.ToString() + FieldDelimeter + dtpInvoiceDate.Text.ToString() + FieldDelimeter + txtNBRPrice.Text.ToString()
                  + FieldDelimeter + txtQuantity.Text.ToString() + FieldDelimeter + txtSubTotal.Text.ToString()
                  + FieldDelimeter + txtVATAmount.Text.ToString() + FieldDelimeter + txtVATRate.Text.ToString() + FieldDelimeter + txtSD.Text.ToString()
                  + FieldDelimeter + txtSDAmount.Text.ToString() + FieldDelimeter + txtUom.Text.ToString() + FieldDelimeter + txtReason.Text.ToString()
                  + FieldDelimeter + chkInvoiceNo.Checked + FieldDelimeter + chkInvoiceDate.Checked + FieldDelimeter + chkReturn.Checked + FieldDelimeter + ApplyAll;
                SelectedValue = results;
                if (IsNull != true)
                
                    this.Close();

                
               
            }
            catch (Exception ex)
            {

            }
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void txtUom_Leave(object sender, EventArgs e)
        {

        }

        private void txtInvoiceNo_Leave(object sender, EventArgs e)
        {
            

        }

        private void btnApplyAll_Click(object sender, EventArgs e)
        {
            try
            {
                ApplyAll = true;
                NullCheck();
                if (chkReturn.Checked == false && chkInvoiceNo.Checked == false && chkInvoiceDate.Checked == false)
                {
                    MessageBox.Show("Please Select At least One Check Box  ");
                    IsNull = true;
                }
                Claculation();
                string results = String.Empty;
                results = txtInvoiceNo.Text.ToString() + FieldDelimeter + dtpInvoiceDate.Text.ToString() + FieldDelimeter + txtNBRPrice.Text.ToString()
                  + FieldDelimeter + txtQuantity.Text.ToString() + FieldDelimeter + txtSubTotal.Text.ToString()
                  + FieldDelimeter + txtVATAmount.Text.ToString() + FieldDelimeter + txtVATRate.Text.ToString() + FieldDelimeter + txtSD.Text.ToString()
                  + FieldDelimeter + txtSDAmount.Text.ToString() + FieldDelimeter + txtUom.Text.ToString() + FieldDelimeter + txtReason.Text.ToString()
                  + FieldDelimeter + chkInvoiceNo.Checked + FieldDelimeter + chkInvoiceDate.Checked + FieldDelimeter + chkReturn.Checked + FieldDelimeter + ApplyAll;
                SelectedValue = results;
                if (IsNull != true)
                {
                    this.Close();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


    }
}
