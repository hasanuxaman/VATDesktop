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
    public partial class FormLeaderPolicy : Form
    {
        #region Constructors

        public FormLeaderPolicy()
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
        private string SelectedValue = string.Empty;
        private bool IsNull = false;

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
        //private void Claculation()
        //{
        //    decimal Subtotal =0;
        //    decimal SDAmount = 0;
        //    decimal VATAmount = 0;
        //    Subtotal = Convert.ToDecimal(txtQuantity.Text.Trim()) * Convert.ToDecimal(txtNBRPrice.Text.Trim());
        //    SDAmount = (Subtotal * Convert.ToDecimal(txtSD.Text.Trim())/ 100);
        //    VATAmount=((Subtotal +SDAmount)*Convert.ToDecimal(txtVATRate.Text.Trim())/100);
        //    txtSubTotal.Text =Convert.ToString( Subtotal);
        //    txtVATAmount.Text = Convert.ToString(VATAmount);
        //    txtSDAmount.Text = Convert.ToString(SDAmount);
        //}
        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
        public static string SelectOne(DataGridViewRow row)
        {
            string SearchValue = string.Empty;
            FormLeaderPolicy LeaderPolicy = new FormLeaderPolicy();
            try
            {


                LeaderPolicy.cmbIsLeader.Text = row.Cells["IsLeader"].Value.ToString();
                LeaderPolicy.txtLeaderAmount.Text = row.Cells["LeaderAmount"].Value.ToString();

           

            //FormDN_CNAdjustment frmSalePrint = new FormDN_CNAdjustment();

                LeaderPolicy.ShowDialog();
                SearchValue = LeaderPolicy.SelectedValue;
                if (SearchValue=="")
                {

                    SearchValue = row.Cells["IsLeader"].Value.ToString() + FieldDelimeter
                                + row.Cells["LeaderAmount"].Value.ToString() + FieldDelimeter                                                    
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
        //private void NullCheck()
        //{

        //    IsNull = false;

        //    if (txtInvoiceNo.Text.Trim()=="")
        //        {
        //            MessageBox.Show("Please Input Invoice No");
        //            txtInvoiceNo.Focus();
        //            IsNull=true;
        //        }

        //    if (Convert.ToDecimal(txtNBRPrice.Text) <= 0)
        //    {
        //        MessageBox.Show("Please declare the NBR Price");
        //        txtNBRPrice.Focus();
        //        IsNull = true;
                
        //    }

        //    if (Convert.ToDecimal(txtQuantity.Text) <= 0)
        //    {
        //        MessageBox.Show("Please Input Quantity");
        //        txtQuantity.Focus();
        //        IsNull = true;
        //    }

        //    if (txtVATRate.Text.Trim() == "")
        //    {
        //        MessageBox.Show("Please Input VAT Rate");
        //        txtVATRate.Focus();
        //        IsNull = true;
        //    }
        //    if (txtUom.Text.Trim() == "")
        //    {
        //        MessageBox.Show("Please Input Uom");
        //        txtUom.Focus();
        //        IsNull = true;
        //    }
        //    if (txtSD.Text.Trim() == "")
        //    {
        //        MessageBox.Show("Please Input SD ");
        //        txtUom.Focus();
        //        IsNull = true;
        //    }

           

        //    if (txtReason.Text.Trim() == "")
        //    {
        //        MessageBox.Show("Please Input Reason Of Return ");
        //        txtUom.Focus();
        //        IsNull = true;
        //    }
        //}
        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {

                string results = String.Empty;
                results = cmbIsLeader.Text.ToString() + FieldDelimeter + txtLeaderAmount.Text.ToString();
                SelectedValue = results;

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
            //Claculation();
        }

        private void FormDN_CNAdjustment_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {

                //string results = String.Empty;
                //results = cmbIsLeader.Text.ToString() + FieldDelimeter + txtLeaderAmount.Text.ToString();
                //SelectedValue = results;

                    this.Close();

                
               
            }
            catch (Exception ex)
            {

            }
        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void cmbIsLeader_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void txtLeaderAmount_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtLeaderAmount_Leave(object sender, EventArgs e)
        {
            txtLeaderAmount.Text = Program.ParseDecimalObject(txtLeaderAmount.Text.Trim()).ToString();
        }

        

      

    }
}
