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
using SymphonySofttech.Utilities;
using VATClient.ModelDTO;
using VATClient.ReportPages;
using VATClient.ReportPreview;
using VATServer.Library;
using VATViewModel.DTOs;
using VATServer.Interface;
using VATServer.Ordinary;
using VATDesktop.Repo;

namespace VATClient
{
    public partial class FormDisposeFinish : Form
    {
        public FormDisposeFinish()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();
            //connVM.SysdataSource = SysDBInfoVM.SysdataSource;
            //connVM.SysPassword = SysDBInfoVM.SysPassword;
            //connVM.SysUserName = SysDBInfoVM.SysUserName;
            //connVM.SysDatabaseName = DatabaseInfoVM.DatabaseName;
        }
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        List<ProductMiniDTO> ProductsMini = new List<ProductMiniDTO>();
        private const string LineDelimeter = DBConstant.LineDelimeter;
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private static string PassPhrase = DBConstant.PassPhrase;
        private static string EnKey = DBConstant.EnKey;
        private string[] sqlResults;
        private bool SAVE_DOWORK_SUCCESS = false;
        private bool DELETE_DOWORK_SUCCESS = false;
        private bool UPDATE_DOWORK_SUCCESS = false;
        private bool POST_DOWORK_SUCCESS = false;
        string[] ProductLines;
        string[] VendorGroupLines;
        string[] ProductFields;
        string ProductResult;
        private DataTable ProductResultDs;
        private DataSet VendorGroupResultDs;
        string ProductName;
        String ProductCode;
        private bool ChangeData = false;
        private string CategoryId { get; set; }
        string NextID = string.Empty;
        private bool IsUpdate = false;
        private bool Post = false;
        private bool IsPost = false;
        private decimal vatAmount = 0;
        private decimal vatAmountImport = 0;
        private int searchBranchId = 0;
        private int IssuePlaceQty;
        private int IssuePlaceAmt;
        private bool Add = false;
        private bool Edit = false;
        #region Global Variable For BackgoundWork

        private DataTable SaleDetailResult;
        private string SaleDetailData = string.Empty;
        private string PurchaseDetailData = string.Empty;
        private string Result = string.Empty;
        private string ResultPost = string.Empty;

        private DisposeMasterVM disposeMasterVM;
        private List<DisposeDetailVM> disposeDetailVMs = new List<DisposeDetailVM>();

        private string varIsRaw, varActiveStatus, varTrading, varNonStock;

        #endregion

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                ////MDIMainInterface mdi = new MDIMainInterface();
                //FormProductCategorySearch frm = new FormProductCategorySearch();
                //mdi.RollDetailsInfo(frm.VFIN);
                //if (Program.Access != "Y")
                //{
                //    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                //    return;
                //}


               // ChangeData = false;
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

                if (rbtnFinish.Checked)
                {
                    if (cmbProductType.Text.Trim() == "Raw" 
                        || cmbProductType.Text.Trim() == "Pack"
                        || cmbProductType.Text.Trim() == "Finish")
                    {
                        ProductSearchDs();
                    }
                    else
                    {
                    MessageBox.Show("Please select right items", this.Text);
                        cmbProductType.Text = "";
                        txtCategoryName.Text = "";
                        return;
                    }
                }
               else if (rbtnRaw.Checked)
                {
                    //if (cmbProductType.Text.Trim() != "Raw" || cmbProductType.Text.Trim() != "Pack")
                    //{
                    //    ProductSearchDs();
                    //}
                    if (cmbProductType.Text.Trim() == "Raw" || cmbProductType.Text.Trim() == "Pack")
                    {
                        ProductSearchDs();
                    }
                    else
                    {
                    MessageBox.Show("Please select Raw items", this.Text);
                        cmbProductType.Text = "";
                        txtCategoryName.Text = "";
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("Please select right items", this.Text);
                    cmbProductType.Text = "";
                    txtCategoryName.Text = "";
                    return;
                }

                //ProductSearchDs(); //SOAP Service

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
                FileLogger.Log(this.Name, "button2_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "button2_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "button2_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "button2_Click", exMessage);
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
                FileLogger.Log(this.Name, "button2_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "button2_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "button2_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "button2_Click", exMessage);
            }
            #endregion
        }
        private void ProductSearchDsLoad()
        {
            try
            {

                cmbProduct.Items.Clear();

                if (chkPCode.Checked == true)
                {

                    var prodByCode = from prd in ProductsMini.ToList()
                                     orderby prd.ProductCode
                                     select prd.ProductCode;


                    if (prodByCode != null && prodByCode.Any())
                    {
                        cmbProduct.Items.AddRange(prodByCode.ToArray());
                    }


                }
                else
                {
                    var prodByCode = from prd in ProductsMini.ToList()
                                     orderby prd.ProductName
                                     select prd.ProductName;


                    if (prodByCode != null && prodByCode.Any())
                    {
                        cmbProduct.Items.AddRange(prodByCode.ToArray());
                    }
                }

                cmbProduct.Items.Insert(0, "Select");
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
        private void btnProductSearch_Click(object sender, EventArgs e)
        {
            try
            {

                ////MDIMainInterface mdi = new MDIMainInterface();
                //FormProductSearch frm = new FormProductSearch();
                //mdi.RollDetailsInfo(frm.VFIN);
                //if (Program.Access != "Y")
                //{
                //    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                //    return;
                //}

                Program.fromOpen = "Other";
                Program.R_F = cmbProductType.Text.Trim();


                DataGridViewRow selectedRow = new DataGridViewRow();
                selectedRow = FormProductSearch.SelectOne();
                if (selectedRow != null && selectedRow.Selected == true)
                {
                    
                   
                    ProductCode = selectedRow.Cells["ItemNo"].Value.ToString();//ProductInfo[0];
                    ProductName = selectedRow.Cells["ProductName"].Value.ToString();//ProductInfo[1];
                    txtPCode.Text = selectedRow.Cells["ProductCode"].Value.ToString();//ProductInfo[27];

                    txtCategoryName.Text = selectedRow.Cells["CategoryName"].Value.ToString();//ProductInfo[4];
                    txtHSCode.Text = selectedRow.Cells["HSCodeNo"].Value.ToString();//ProductInfo[11];
                    txtVATRate.Text = "0.00";

                    txtVATRate.Text = Convert.ToDecimal(selectedRow.Cells["VATRate"].Value.ToString()).ToString("0.00");//12

                    txtQuantityInHand.Text = Convert.ToDecimal(selectedRow.Cells["Stock"].Value.ToString()).ToString("0.00");//17
                    txtUOM.Text = selectedRow.Cells["UOM"].Value.ToString();//ProductInfo[5];
                    txtNBRPrice.Text = Convert.ToDecimal(selectedRow.Cells["NBRPrice"].Value.ToString()).ToString("0.00"); // NBR Price 8


                }

               
                ProductSearchDs();
                txtProductName.Text = ProductName;
                txtProductCode.Text = ProductCode;
                cmbProduct.Text = ProductName;
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
                FileLogger.Log(this.Name, "btnProductSearch_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnProductSearch_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnProductSearch_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnProductSearch_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnProductSearch_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnProductSearch_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnProductSearch_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnProductSearch_Click", exMessage);
            }
            #endregion

        }
        private void chkPCode_CheckedChanged(object sender, EventArgs e)
        {

            ProductSearchDsLoad();

            cmbProduct.Focus();
        }
        private void cmbProduct_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeData = true;

        }
        private void cmbProduct_Leave(object sender, EventArgs e)
        {
            try
            {
                ProductDAL productDal = new ProductDAL();
                var searchText = cmbProduct.Text.Trim().ToLower();
                if (!string.IsNullOrEmpty(searchText) && searchText != "select")
                {


                    if (chkPCode.Checked)
                    {
                        var prodByCode = from prd in ProductsMini.ToList()
                                         where prd.ProductCode.ToLower() == searchText.ToLower()
                                         select prd;
                        
                        if (prodByCode != null && prodByCode.Any())
                        {
                            var products = prodByCode.First();
                            txtProductName.Text = products.ProductName;
                            txtProductCode.Text = products.ItemNo;
                            //txtRealPrice.Text = products.IssuePrice.ToString("0,0.00");
                            //if (rbtnIssueReturn.Checked)
                            //{
                            //    txtRealPrice.Text = products.IssuePrice.ToString("0,0.00");
                            //}
                            //else if (rbtnTollReceive.Checked)
                            //{
                            //    txtRealPrice.Text = products.TollCharge.ToString("0,0.00");
                            //}
                            //else
                            //{
                            //    txtRealPrice.Text = "0.00";
                            //}
                            txtUOM.Text = products.UOM;
                            txtHSCode.Text = products.HSCodeNo;
                            txtVATRate.Text = products.VATRate.ToString("0.00");
                            txtPurchaseQty.Text = Program.FormatingNumeric(products.PurchaseQty.ToString(), IssuePlaceQty).ToString();
                            txtPCode.Text = products.ProductCode;
                            txtNBRPrice.Text = products.NBRPrice.ToString("0,0.00");
                            txtNBRPrice.Text = Program.FormatingNumeric(products.NBRPrice.ToString(), IssuePlaceAmt).ToString();

                            if (rbtnFinish.Checked)
                            {
                                //txtRealPrice.Text = products.NBRPrice.ToString("0,0.00");
                                txtRealPrice.Text = Program.FormatingNumeric(products.NBRPrice.ToString(), IssuePlaceAmt).ToString();

                            }
                            else if (rbtnRaw.Checked)
                            {
                                txtRealPrice.Text = products.IssuePrice.ToString("0,0.00");
                                txtRealPrice.Text = Program.FormatingNumeric(products.IssuePrice.ToString(), IssuePlaceAmt).ToString();


                                DataTable priceData = productDal.AvgPriceNew(products.ItemNo, dtpDisposeDate.Value.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss"),
                                    null, null, false,true,true,true,connVM,Program.CurrentUserID);
                                decimal amount = Convert.ToDecimal(priceData.Rows[0]["Amount"].ToString());
                                decimal quan = Convert.ToDecimal(priceData.Rows[0]["Quantity"].ToString());

                                if (quan > 0)
                                {
                                    //txtRealPrice.Text = (amount / quan).ToString("0,0.00");
                                    txtRealPrice.Text = Program.FormatingNumeric((amount / quan).ToString(), IssuePlaceAmt).ToString();

                                }
                                
                            }
                            if (chkFromStock.Checked)
                            {

                                txtQuantityInHand.Text = productDal.AvgPriceNew(products.ItemNo,
                                                                                         dtpDisposeDate.Value.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss"), 
                                                                                         null, null,false,true,true,true,connVM,Program.CurrentUserID).Rows[0]["Quantity"].ToString();

                                txtQuantityInHand.Text = Program.FormatingNumeric(txtQuantityInHand.Text, IssuePlaceQty).ToString();
                                //txtQuantityInHand.Text = products.Stock.ToString("0,0.0000");
                            }
                            else
                            {
                                txtQuantityInHand.Text = "0";
                            }
                        }
                    }
                    else
                    {
                        var prodByName = from prd in ProductsMini.ToList()
                                         where prd.ProductName.ToLower() == searchText.ToLower()
                                         select prd;

                        if (prodByName != null && prodByName.Any())
                        {
                            var products = prodByName.First();
                            txtProductName.Text = products.ProductName;
                            txtProductCode.Text = products.ItemNo;
                            //txtRealPrice.Text = products.IssuePrice.ToString("0,0.00");

                            //if (rbtnIssueReturn.Checked)
                            //{
                            //    txtRealPrice.Text = products.IssuePrice.ToString("0,0.00");
                            //}
                            //else if (rbtnTollReceive.Checked)
                            //{
                            //    txtRealPrice.Text = products.TollCharge.ToString("0,0.00");
                            //}
                            //else
                            //{
                            //    txtRealPrice.Text = "0.00";
                            //}
                            txtUOM.Text = products.UOM;
                            txtHSCode.Text = products.HSCodeNo;
                            txtVATRate.Text = products.VATRate.ToString("0.00");
                            //txtQuantityInHand.Text = products.Stock.ToString("0,0.0000");
                            //txtPurchaseQty.Text = products.PurchaseQty.ToString("0,0.0000");
                            txtPurchaseQty.Text = Program.FormatingNumeric(products.PurchaseQty.ToString(), IssuePlaceQty).ToString();


                            txtPCode.Text = products.ProductCode;
                            //txtNBRPrice.Text = products.NBRPrice.ToString("0,0.00");
                            txtNBRPrice.Text = Program.FormatingNumeric(products.NBRPrice.ToString(), IssuePlaceAmt).ToString();

                            if (chkFromStock.Checked)
                            {

                                txtQuantityInHand.Text = productDal.AvgPriceNew(products.ItemNo,
                                                                                    dtpDisposeDate.Value.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss"),
                                                                                    null, null, false,true,true,true,connVM,Program.CurrentUserID).Rows[0]["Quantity"].ToString();

                                //txtQuantityInHand.Text = products.Stock.ToString("0,0.0000");
                            }
                            else
                            {
                                txtQuantityInHand.Text = "0";
                            }


                            if (rbtnFinish.Checked)
                            {
                                //txtRealPrice.Text = products.NBRPrice.ToString("0,0.00");
                                txtRealPrice.Text = Program.FormatingNumeric(products.NBRPrice.ToString(), IssuePlaceAmt).ToString();

                            }
                            else if (rbtnRaw.Checked)
                            {
                                //txtRealPrice.Text = products.IssuePrice.ToString("0,0.00");
                                txtRealPrice.Text = Program.FormatingNumeric(products.IssuePrice.ToString(), IssuePlaceAmt).ToString();

                                DataTable priceData = productDal.AvgPriceNew(products.ItemNo, dtpDisposeDate.Value.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss"),
                                    null, null, false,true,true,true,connVM,Program.CurrentUserID);
                                decimal amount = Convert.ToDecimal(priceData.Rows[0]["Amount"].ToString());
                                decimal quan = Convert.ToDecimal(priceData.Rows[0]["Quantity"].ToString());

                                if (quan > 0)
                                {
                                    //txtRealPrice.Text = (amount / quan).ToString("0,0.00");
                                    txtRealPrice.Text = Program.FormatingNumeric((amount / quan).ToString(), IssuePlaceAmt).ToString();

                                }

                            }
                        }
                    }


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
        private void btnAdd_Click(object sender, EventArgs e)
        {
            #region Null

            if (txtCommentsDetail.Text == "")
            {
                txtCommentsDetail.Text = "-";
            }
            if (txtProductCode.Text == "")
            {
                MessageBox.Show("Please Select a Item");
                return;
            }
            if (txtQuantity.Text == "")
            {
                txtQuantity.Text = "0.00";
            }
            if (txtQuantityImport.Text == "")
            {
                txtQuantityImport.Text = "0.00";
            }

            if (txtRealPrice.Text == "")
            {
                txtRealPrice.Text = "0.00";
            }
            if (txtNBRPrice.Text == "")
            {
                txtNBRPrice.Text = "0.00";
            }
            if (txtVATRate.Text == "")
            {
                txtVATRate.Text = "0.00";
            }
            if (txtSaleNumber.Text == "")
            {
                txtSaleNumber.Text = "NA";
            }
            if (txtPurchaseNumber.Text == "")
            {
                txtPurchaseNumber.Text = "NA";
            }
            if (txtPresentPrice.Text == "")
            {
                txtPresentPrice.Text = "0.00";
            }

            if (Convert.ToDecimal(txtRealPrice.Text) <= 0)
            {
                MessageBox.Show("Please input Real Price");
                txtRealPrice.Focus();
                return;
            }
            if (Convert.ToDecimal(txtPresentPrice.Text) <= 0)
            {
                txtPresentPrice.Text = "0.00";
            }
            if (Convert.ToDecimal(txtQuantity.Text) <= 0)
            {
                MessageBox.Show("Please input Quantity");
                txtQuantity.Focus();
                return;
            }
            if (Convert.ToDecimal(txtQuantityImport.Text) <= 0)
            {
                txtQuantityImport.Text = "0";
            }
            #endregion Null

            try
            {

                ChangeData = true;
                if (rbtnRaw.Checked)
                {
                    if (txtPurchaseNumber.Text !="NA")
                    {
                        //if (Convert.ToDecimal(txtQuantity.Text) > Convert.ToDecimal(txtQuantityInHand.Text))
                        //{
                        //    MessageBox.Show("Quantity exist the purchase quantity");
                        //    return;
                        //}

                        if (Convert.ToDecimal(txtQuantity.Text) > Convert.ToDecimal(txtPurchaseQty.Text))
                        {
                            MessageBox.Show("Quantity exist the purchase quantity");
                            return;
                        }
                    }
                }
                if (Convert.ToDecimal(txtQuantity.Text) > Convert.ToDecimal(txtQuantityInHand.Text))
                {
                    MessageBox.Show("Stock Not Available");
                    return;
                }


                for (int i = 0; i < dgvDispose.RowCount; i++)
                {
                    if (dgvDispose.Rows[i].Cells["ItemNo"].Value.ToString() == txtProductCode.Text.Trim() &&
                        dgvDispose.Rows[i].Cells["PurchaseNumber"].Value.ToString() == txtPurchaseNumber.Text.Trim())
                    {
                        MessageBox.Show("Same Product already exist.", this.Text);
                        cmbProduct.Focus();
                        return;
                    }
                }
                if (rbtnFinish.Checked)
                {
                    txtVATRate.Text = "0";
                    txtTotalVATAmount.Text = "0";
                }


                decimal qty = 0, qtyImport = 0, RPrice = 0, vatAmount = 0;


                DataGridViewRow NewRow = new DataGridViewRow();
                dgvDispose.Rows.Add(NewRow);

                dgvDispose["ItemNo", dgvDispose.RowCount - 1].Value = txtProductCode.Text.Trim();
                dgvDispose["ItemName", dgvDispose.RowCount - 1].Value = txtProductName.Text.Trim();
                dgvDispose["UOM", dgvDispose.RowCount - 1].Value = txtUOM.Text.Trim();
                dgvDispose["Quantity", dgvDispose.RowCount - 1].Value =
                    //Convert.ToDecimal(txtQuantity.Text.Trim()).ToString("0,0.0000");
                Program.FormatingNumeric(txtQuantity.Text.Trim(), IssuePlaceQty).ToString();

                dgvDispose["QuantityImport", dgvDispose.RowCount - 1].Value =
                    //Convert.ToDecimal(txtQuantityImport.Text.Trim()).ToString("0,0.0000");
                    Program.FormatingNumeric(txtQuantityImport.Text.Trim(), IssuePlaceQty).ToString();

                dgvDispose["PresentPrice", dgvDispose.RowCount - 1].Value =
                    //Convert.ToDecimal(txtPresentPrice.Text.Trim()).ToString("0,0.00");
                    Program.FormatingNumeric(txtPresentPrice.Text.Trim(), IssuePlaceAmt).ToString();
                dgvDispose["RealPrice", dgvDispose.RowCount - 1].Value =
                    //Convert.ToDecimal(txtRealPrice.Text.Trim()).ToString("0,0.00");
                Program.FormatingNumeric(txtRealPrice.Text.Trim(), IssuePlaceAmt).ToString();

                dgvDispose["VATRate", dgvDispose.RowCount - 1].Value =
                    //Convert.ToDecimal(txtVATRate.Text.Trim()).ToString("0.00");
                Program.FormatingNumeric(txtVATRate.Text.Trim(), IssuePlaceAmt).ToString();


                //dgvDispose["VATAmt", dgvDispose.RowCount - 1].Value = Convert.ToDecimal(txtRealPrice.Text.Trim()) *
                //    Convert.ToDecimal(Convert.ToDecimal(txtQuantity.Text.Trim()) + Convert.ToDecimal(txtQuantity.Text.Trim())) * Convert.ToDecimal(txtVATRate.Text.Trim()) / 100;

                dgvDispose["Comments", dgvDispose.RowCount - 1].Value = txtCommentsDetail.Text.Trim();
                dgvDispose["SaleNumber", dgvDispose.RowCount - 1].Value = txtSaleNumber.Text.Trim();
                dgvDispose["PurchaseNumber", dgvDispose.RowCount - 1].Value = txtPurchaseNumber.Text.Trim();


                dgvDispose["Status", dgvDispose.RowCount - 1].Value = "New";
                dgvDispose["Stock", dgvDispose.RowCount - 1].Value =
                    //Convert.ToDecimal(txtQuantityInHand.Text.Trim()).ToString("0,0.0000");
                Program.FormatingNumeric(txtQuantityInHand.Text.Trim(), IssuePlaceQty).ToString();

                dgvDispose["Previous", dgvDispose.RowCount - 1].Value = 0; // txtQuantity.Text.Trim();
                dgvDispose["Change", dgvDispose.RowCount - 1].Value = 0;

                dgvDispose["PCode", dgvDispose.RowCount - 1].Value = txtPCode.Text.Trim();
                Rowcalculate();


                txtProductCode.Text = "";
                txtPCode.Text = "";
                txtProductName.Text = "";
                //txtCategoryName.Text = "";
                txtHSCode.Text = "";
                txtRealPrice.Text = "";
                txtPresentPrice.Text = "";
                txtQuantity.Text = "";
                txtQuantityImport.Text = "";
                txtVATRate.Text = "0.00";
                txtUOM.Text = "";
                txtQuantityInHand.Text = "";
                selectLastRow();
                cmbProduct.Text = "Select";
                //cmbProduct.Focus();
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
        private void Rowcalculate()
        {
            decimal Quantity = 0;
            decimal QuantityImport = 0;
            decimal UnitPrice = 0;
            decimal SubTotal = 0;

            decimal VATRate = 0;
            decimal VATAmount = 0;
            decimal Total = 0;


            decimal SumSDAmount = 0;
            decimal SumVATAmount = 0;
            decimal SumSubTotal = 0;
            decimal SumTotal = 0;
            decimal LocalAmount = 0;
            decimal ImportAmount = 0;
            decimal LocalVAT = 0;
            decimal ImportVAT = 0;

            decimal SubLocalAmount = 0;
            decimal SubImportAmount = 0;
            decimal SubLocalVAT = 0;
            decimal SubImportVAT = 0;

            for (int i = 0; i < dgvDispose.RowCount; i++)
            {
                Quantity = Convert.ToDecimal(dgvDispose["Quantity", i].Value);
                QuantityImport = Convert.ToDecimal(dgvDispose["QuantityImport", i].Value);

                UnitPrice = Convert.ToDecimal(dgvDispose["RealPrice", i].Value);
                SubTotal = UnitPrice * (Quantity + QuantityImport);
                VATRate = Convert.ToDecimal(dgvDispose["VATRate", i].Value);
                VATAmount = SubTotal * VATRate / 100;

                LocalAmount = UnitPrice * Quantity;
                ImportAmount = UnitPrice * QuantityImport;

                LocalVAT = LocalAmount * VATRate / 100;
                ImportVAT = ImportAmount * VATRate / 100;



                Total = (VATAmount + SubTotal);
                dgvDispose[0, i].Value = i + 1;

                //dgvDispose["VATAmt", i].Value = Convert.ToDecimal(VATAmount).ToString("0.00");
                dgvDispose["VATAmt", i].Value = Program.FormatingNumeric(VATAmount.ToString(), IssuePlaceAmt).ToString();

                //dgvDispose["SubTotal", i].Value = Convert.ToDecimal(SubTotal).ToString("0,0.00");
                dgvDispose["SubTotal", i].Value = Program.FormatingNumeric(SubTotal.ToString(), IssuePlaceAmt).ToString();

                //dgvDispose["Total", i].Value = Convert.ToDecimal(Total).ToString("0,0.00");
                dgvDispose["Total", i].Value = Program.FormatingNumeric(Total.ToString(), IssuePlaceAmt).ToString();


                SumTotal = SumTotal + Convert.ToDecimal(dgvDispose["Total", i].Value);
                dgvDispose["Change", i].Value = Convert.ToDecimal(Convert.ToDecimal(dgvDispose["Quantity", i].Value) - Convert.ToDecimal(dgvDispose["Previous", i].Value)).ToString("0.00");
                dgvDispose["Change", i].Value = Program.FormatingNumeric(dgvDispose["Change", i].Value.ToString(), IssuePlaceQty).ToString();

                SubLocalAmount = SubLocalAmount + LocalAmount;
                SubImportAmount = SubImportAmount + ImportAmount;
                SubLocalVAT = SubLocalVAT + LocalVAT;
                SubImportVAT = SubImportVAT + ImportVAT;
            }

            //txtTotalSubTotal.Text = Convert.ToDecimal(SumSubTotal).ToString("0,0.00");
            //txtTotalAmount.Text = Convert.ToDecimal(SumTotal).ToString("0,0.00");
            txtTotalAmount.Text = Program.FormatingNumeric(SumTotal.ToString(), IssuePlaceAmt).ToString();


            //txtTotalSubTotal.Text = Convert.ToDecimal(SubLocalAmount).ToString("0,0.00");
            txtTotalSubTotal.Text = Program.FormatingNumeric(SubLocalAmount.ToString(), IssuePlaceAmt).ToString();

            //txtTotalSubTotalImport.Text = Convert.ToDecimal(SubImportAmount).ToString("0,0.00");
            txtTotalSubTotalImport.Text = Program.FormatingNumeric(SubImportAmount.ToString(), IssuePlaceAmt).ToString();

            txtTotalVATAmount.Text = Convert.ToDecimal(SubLocalVAT).ToString("0,0.00");
            txtTotalVATAmount.Text = Program.FormatingNumeric(SubLocalVAT.ToString(), IssuePlaceAmt).ToString();

            //txtTotalVATAmountImport.Text = Convert.ToDecimal(SubImportVAT).ToString("0,0.00");
            txtTotalVATAmountImport.Text = Program.FormatingNumeric(SubImportVAT.ToString(), IssuePlaceAmt).ToString();


        }
        private void selectLastRow()
        {
            try
            {
                if (dgvDispose.Rows.Count > 0)
                {

                    dgvDispose.Rows[dgvDispose.Rows.Count - 1].Selected = true;
                    dgvDispose.CurrentCell = dgvDispose.Rows[dgvDispose.Rows.Count - 1].Cells[1];


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
        private void btnChange_Click(object sender, EventArgs e)
        {
            #region Null

            if (txtCommentsDetail.Text == "")
            {
                txtCommentsDetail.Text = "-";
            }
            if (txtProductCode.Text == "")
            {
                MessageBox.Show("Please Select a Item");
                return;
            }
            if (txtQuantity.Text == "")
            {
                txtQuantity.Text = "0.00";
            }
            if (txtQuantityImport.Text == "")
            {
                txtQuantityImport.Text = "0.00";
            }

            if (txtRealPrice.Text == "")
            {
                txtRealPrice.Text = "0.00";
            }
            if (txtNBRPrice.Text == "")
            {
                txtNBRPrice.Text = "0.00";
            }
            if (txtVATRate.Text == "")
            {
                txtVATRate.Text = "0.00";
            }
            if (txtSaleNumber.Text == "")
            {
                txtSaleNumber.Text = "NA";
            }
            if (txtPurchaseNumber.Text == "")
            {
                txtPurchaseNumber.Text = "NA";
            }
            if (txtPresentPrice.Text == "")
            {
                txtPresentPrice.Text = "0.00";
            }

            if (Convert.ToDecimal(txtRealPrice.Text) <= 0)
            {
                MessageBox.Show("Please input Real Price");
                txtRealPrice.Focus();
                return;
            }
            if (Convert.ToDecimal(txtPresentPrice.Text) <= 0)
            {
                txtPresentPrice.Text = "0.00";
            }
            if (Convert.ToDecimal(txtQuantity.Text) <= 0)
            {
                MessageBox.Show("Please input Quantity");
                txtQuantity.Focus();
                return;
            }
            if (Convert.ToDecimal(txtQuantityImport.Text) <= 0)
            {
                txtQuantityImport.Text = "0";
            }
            if (txtProductCode.Text == "")
            {
                MessageBox.Show("Please Select a Item");
                return;
            }
            if (txtCommentsDetail.Text == "")
            {
                txtCommentsDetail.Text = "-";
            }
            if (rbtnFinish.Checked)
            {
                txtVATRate.Text = "0";
                txtTotalVATAmount.Text = "0";
            }
            #endregion Null



            try
            {
                ChangeData = true;

                decimal StockValue, PreviousValue, ChangeValue, CurrentValue, PurCurrentValue;

                StockValue = Convert.ToDecimal(dgvDispose.CurrentRow.Cells["Stock"].Value);
                PreviousValue = Convert.ToDecimal(dgvDispose.CurrentRow.Cells["Previous"].Value.ToString());

                PurCurrentValue = Convert.ToDecimal(txtPurchaseQty.Text);

                CurrentValue = Convert.ToDecimal(txtQuantity.Text);
                if (rbtnRaw.Checked)
                {
                    if (PurCurrentValue > CurrentValue)
                    {
                        CurrentValue = Convert.ToDecimal(txtQuantity.Text);
                    }
                    else
                    {
                        CurrentValue = Convert.ToDecimal(txtPurchaseQty.Text);
                    }
                    ChangeValue = CurrentValue - PreviousValue;

                    if (dgvDispose.CurrentRow.Cells["Status"].Value.ToString() == "New")
                    {
                        
                       
                        if (Convert.ToDecimal(txtQuantity.Text) > Convert.ToDecimal(txtQuantityInHand.Text))
                        {
                            MessageBox.Show("Stock Not Available", this.Text);
                            return;
                        }
                        else if (Convert.ToDecimal(txtQuantity.Text) > Convert.ToDecimal(txtPurchaseQty.Text))
                        {
                            if (txtPurchaseNumber.Text != "NA")
                            {
                                MessageBox.Show("Stock Not Available", this.Text);
                                return;
                            }
                            
                        }

                        else if (ChangeValue > 0)
                        {
                            if (StockValue < Math.Abs(ChangeValue))
                            {
                                MessageBox.Show("Can't Change, Stock not Available", this.Text);
                                txtQuantity.Focus();
                                return;
                            }
                        }
                    }
                }
                else
                {
                    CurrentValue = Convert.ToDecimal(txtQuantity.Text);
                    ChangeValue = CurrentValue - PreviousValue;
                    if (dgvDispose.CurrentRow.Cells["Status"].Value.ToString() == "New")
                    {
                        if (Convert.ToDecimal(txtQuantity.Text) > Convert.ToDecimal(txtQuantityInHand.Text))
                        {
                            MessageBox.Show("Stock Not Available", this.Text);
                            return;
                        }
                    }

                    if (dgvDispose.CurrentRow.Cells["Status"].Value.ToString() != "New")
                    {
                        if (ChangeValue > 0)
                        {
                            if (StockValue < Math.Abs(ChangeValue))
                            {
                                MessageBox.Show("Can't Change, Stock not Available", this.Text);
                                txtQuantity.Focus();
                                return;
                            }
                        }
                    }
                }

                dgvDispose["UOM", dgvDispose.CurrentRow.Index].Value = txtUOM.Text.Trim();
                dgvDispose["Quantity", dgvDispose.CurrentRow.Index].Value =
                    //Convert.ToDecimal(txtQuantity.Text.Trim()).ToString("0,0.0000");
                Program.FormatingNumeric(txtQuantity.Text.Trim(), IssuePlaceQty).ToString();
                dgvDispose["QuantityImport", dgvDispose.CurrentRow.Index].Value =
                    //Convert.ToDecimal(txtQuantityImport.Text.Trim()).ToString("0,0.0000");
                Program.FormatingNumeric(txtQuantityImport.Text.Trim(), IssuePlaceQty).ToString();

                //dgvDispose["VATAmt", dgvDispose.CurrentRow.Index].Value = Convert.ToDecimal(txtRealPrice.Text.Trim()) *
                //    Convert.ToDecimal(txtQuantity.Text.Trim() + Convert.ToDecimal(txtQuantityImport.Text.Trim())) * Convert.ToDecimal(txtVATRate.Text.Trim()) / 100;



                dgvDispose["RealPrice", dgvDispose.CurrentRow.Index].Value =
                    //Convert.ToDecimal(txtRealPrice.Text.Trim()).ToString("0,0.00");
                Program.FormatingNumeric(txtRealPrice.Text.Trim(), IssuePlaceAmt).ToString();

                dgvDispose["VATRate", dgvDispose.CurrentRow.Index].Value =
                    //Convert.ToDecimal(txtVATRate.Text.Trim()).ToString("0,0.00");
                Program.FormatingNumeric(txtVATRate.Text.Trim(), IssuePlaceAmt).ToString();



                dgvDispose["Comments", dgvDispose.CurrentRow.Index].Value = txtCommentsDetail.Text.Trim();
                dgvDispose["SaleNumber", dgvDispose.CurrentRow.Index].Value = txtSaleNumber.Text.Trim();
                dgvDispose["PurchaseNumber", dgvDispose.CurrentRow.Index].Value = txtPurchaseNumber.Text.Trim();
                if (dgvDispose.CurrentRow.Cells["Status"].Value.ToString() != "New")
                {
                    dgvDispose["Status", dgvDispose.CurrentRow.Index].Value = "Change";
                }
                dgvDispose.CurrentRow.DefaultCellStyle.ForeColor = Color.Green; // Blue;

                dgvDispose.CurrentRow.DefaultCellStyle.Font = new Font(this.Font, FontStyle.Regular);

                Rowcalculate();
                txtProductCode.Text = "";
                txtProductName.Text = "";
                //txtCategoryName.Text = "";
                txtHSCode.Text = "";
                txtRealPrice.Text = "0";
                txtPresentPrice.Text = "0";
                txtQuantity.Text = "";
                txtQuantityImport.Text = "";

                txtVATRate.Text = "0.00";
                txtUOM.Text = "";
                txtQuantityInHand.Text = "";
                cmbProduct.Text = "Select";
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
        private void btnRemove_Click(object sender, EventArgs e)
        {
            try
            {
                try
                {
                    if (dgvDispose.RowCount > 0)
                    {
                        if (MessageBox.Show(MessageVM.msgWantToRemoveRow + "\nItem Code: " + dgvDispose.CurrentRow.Cells["PCode"].Value, this.Text, MessageBoxButtons.YesNo,
                                    MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            dgvDispose.Rows.RemoveAt(dgvDispose.CurrentRow.Index);
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
                catch (ArgumentNullException ex)
                {
                    string err = ex.ToString();
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

                //if (txtProductCode.Text == "")
                //{
                //    MessageBox.Show("Please select a Item");
                //    return;
                //}
                //if (txtCommentsDetail.Text == "")
                //{
                //    txtCommentsDetail.Text = "-";
                //}


                //ChangeData = true;
                //decimal StockValue, PreviousValue, ChangeValue, CurrentValue;
                //StockValue = Convert.ToDecimal(dgvDispose.CurrentRow.Cells["Stock"].Value);
                //PreviousValue = Convert.ToDecimal(dgvDispose.CurrentRow.Cells["Previous"].Value.ToString());
                //CurrentValue = 0; // Convert.ToDecimal(txtQuantity.Text);
                //ChangeValue = CurrentValue - PreviousValue;

                //if (dgvDispose.CurrentRow.Cells["Status"].Value.ToString() != "New")
                //{
                //    if (ChangeValue < 0)
                //    {
                //        if (StockValue < Math.Abs(ChangeValue))
                //        {
                //            MessageBox.Show("Can't Change, Stock not Available");
                //            return;
                //        }
                //    }
                //}

                //if (Convert.ToDecimal(dgvDispose.CurrentCellAddress.Y) < 0)
                //{
                //    MessageBox.Show("There have no Data to Delete");
                //    return;
                //}
                ////dgvPurchase.Rows.RemoveAt(dgvPurchase.CurrentCellAddress.Y);
                //dgvDispose.CurrentRow.Cells["Status"].Value = "Delete";
                //dgvDispose.CurrentRow.Cells["Quantity"].Value = 0.00;
                //dgvDispose.CurrentRow.Cells["QuantityImport"].Value = 0.00;
                //dgvDispose.CurrentRow.Cells["RealPrice"].Value = 0.00;
                //dgvDispose.CurrentRow.Cells["PresentPrice"].Value = 0.00;
                //dgvDispose.CurrentRow.Cells["VATAmt"].Value = 0.00;


                //dgvDispose.CurrentRow.DefaultCellStyle.ForeColor = Color.Red;
                //dgvDispose.CurrentRow.DefaultCellStyle.Font = new Font(this.Font, FontStyle.Strikeout);
                //Rowcalculate();
                //txtProductCode.Text = "";
                //txtProductName.Text = "";
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
        private void dgvPurchase_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                if (dgvDispose.Rows.Count <= 0)
                {
                    MessageBox.Show("There is no data to select.", this.Text, MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                    return;
                }
                txtLineNo.Text = dgvDispose.CurrentCellAddress.Y.ToString();
                txtProductCode.Text = dgvDispose.CurrentRow.Cells["ItemNo"].Value.ToString();
                txtProductName.Text = dgvDispose.CurrentRow.Cells["ItemName"].Value.ToString();
                if (chkPCode.Checked)
                {
                    cmbProduct.Text = dgvDispose.CurrentRow.Cells["ItemNo"].Value.ToString(); 
                }
                else
                {
                    cmbProduct.Text = dgvDispose.CurrentRow.Cells["ItemName"].Value.ToString();
                }
                
                txtUOM.Text = dgvDispose.CurrentRow.Cells["UOM"].Value.ToString();
                txtQuantity.Text = Convert.ToDecimal(dgvDispose.CurrentRow.Cells["Quantity"].Value).ToString("0,0.0000");
                txtQuantityImport.Text =
                    Convert.ToDecimal(dgvDispose.CurrentRow.Cells["QuantityImport"].Value).ToString("0,0.0000");


                
                txtRealPrice.Text = Convert.ToDecimal(dgvDispose.CurrentRow.Cells["RealPrice"].Value).ToString("0,0.00");
               
                txtVATRate.Text = Convert.ToDecimal(dgvDispose.CurrentRow.Cells["VATRate"].Value).ToString("0.00");

                txtCommentsDetail.Text = dgvDispose.CurrentRow.Cells["Comments"].Value.ToString();
                txtSaleNumber.Text = dgvDispose.CurrentRow.Cells["SaleNumber"].Value.ToString();
                txtPurchaseNumber.Text = dgvDispose.CurrentRow.Cells["PurchaseNumber"].Value.ToString();
                txtQuantityInHand.Text =
                    Convert.ToDecimal(dgvDispose.CurrentRow.Cells["Stock"].Value).ToString("0,0.0000");

                txtPrevious.Text = Convert.ToDecimal(dgvDispose.CurrentRow.Cells["Previous"].Value).ToString("0.00");

                SaleDAL saleDal = new SaleDAL();
                string categoryName = saleDal.GetCategoryName(txtProductCode.Text,connVM);
                if (!string.IsNullOrEmpty(categoryName))
                {
                    txtCategoryName.Text = categoryName;
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
                FileLogger.Log(this.Name, "dgvPurchase_DoubleClick", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "dgvPurchase_DoubleClick", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "dgvPurchase_DoubleClick", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "dgvPurchase_DoubleClick", exMessage);
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
                FileLogger.Log(this.Name, "dgvPurchase_DoubleClick", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "dgvPurchase_DoubleClick", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "dgvPurchase_DoubleClick", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "dgvPurchase_DoubleClick", exMessage);
            }
            #endregion

        }
        private void FormDisposeFinish_Load(object sender, EventArgs e)
        {
            try
            {
                Post = Convert.ToString(Program.Post) == "Y" ? true : false;
                Add = Convert.ToString(Program.Add) == "Y" ? true : false;
                Edit = Convert.ToString(Program.Edit) == "Y" ? true : false;

                IsUpdate = false;
                AllClear();
                formMaker();
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
                FileLogger.Log(this.Name, "FormDisposeFinish_Load", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "FormDisposeFinish_Load", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "FormDisposeFinish_Load", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "FormDisposeFinish_Load", exMessage);
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
                FileLogger.Log(this.Name, "FormDisposeFinish_Load", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormDisposeFinish_Load", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormDisposeFinish_Load", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "FormDisposeFinish_Load", exMessage);
            }
            #endregion


        }
        private void ProductSearchDsFormLoad()
        {
            //string ProductData = string.Empty;

            try
            {
                this.progressBar1.Visible = true;

                if (rbtnFinish.Checked)
                {
                    //ProductData = FieldDelimeter + // ItemNo,
                    //    FieldDelimeter + // CategoryID,
                    //    "Finish" + FieldDelimeter + // IsRaw,
                    //    FieldDelimeter + // HSCodeNo,
                    //    "Y" + FieldDelimeter + // ActiveStatus,
                    //    "N" + FieldDelimeter + // Trading, 
                    //    "N" + FieldDelimeter + // NonStock,
                    //    FieldDelimeter + // ProductCode
                    //    LineDelimeter;
                    //txtCategoryName.Text = "Finish";

                    varIsRaw = "Finish";
                    varActiveStatus = "Y";
                    varTrading = "N";
                    varNonStock = "N";
                    txtCategoryName.Text = "Finish";


                }
                else if (rbtnRaw.Checked) //transfer
                {

                    //ProductData = FieldDelimeter + // ItemNo,
                    //    FieldDelimeter + // CategoryID,
                    //    "Raw" + FieldDelimeter + // IsRaw,
                    //    FieldDelimeter + // HSCodeNo,
                    //    "Y" + FieldDelimeter + // ActiveStatus,
                    //    "N" + FieldDelimeter + // Trading, 
                    //    "N" + FieldDelimeter + // NonStock,
                    //    FieldDelimeter + // ProductCode
                    //    LineDelimeter;
                    //txtCategoryName.Text = "Raw";

                    varIsRaw = "Raw";
                    varActiveStatus = "Y";
                    varTrading = "N";
                    varNonStock = "N";
                    txtCategoryName.Text = "Raw";

                }

                

                backgroundWorkerProductSearchDsFormLoad.RunWorkerAsync();

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
        }
        private void formMaker()
        {
            #region Settings        
            string vIssuePlaceQty, vIssuePlaceAmt  = string.Empty;


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

            txtPurchaseNumber.Visible = true;
            txtSaleNumber.Visible = true;
            L2.Visible = true;
            L4.Visible = true;
            chkFromStock.Checked = true;
            btnPurchaseSearch.Visible = true;
            txtPurchaseQty.Visible = true;
            label31.Visible = true;
            label15.Visible = true;
            label16.Visible = true;
            txtVATRate.ReadOnly = false;
            button2.Visible = true;


            txtTotalVATAmountImport.Visible = true;
            txtTotalVATAmount.Visible = true;

            if (rbtnFinish.Checked)
            {
                ProductSearchDsFormLoad();
                

                txtVATRate.Text = "0";
                txtVATRate.ReadOnly = true;
                txtPurchaseNumber.Visible = false;
                txtSaleNumber.Visible = false;
                btnPurchaseSearch.Visible = false;

                txtTotalVATAmountImport.Visible = false;
                txtTotalVATAmount.Visible = false;

                L2.Visible = false;
                L4.Visible = false;
                txtPurchaseQty.Visible = false;
                label31.Visible = false;
                label15.Visible = false;
                label16.Visible = false;

                dgvDispose.Columns["VATAmt"].Visible = false;
                dgvDispose.Columns["VATRate"].Visible = false;
                dgvDispose.Columns["PurchaseNumber"].Visible = false;
                dgvDispose.Columns["SaleNumber"].Visible = false;


                this.Text = "VAT 27";
                btnPrint.Text = "VAT 27";
            }
            else
            {
                txtPurchaseNumber.Visible = true;
                txtSaleNumber.Visible = true;
                L2.Visible = true;
                L4.Visible = true;
                //button2.Visible = false;
                this.Text = "VAT 26";
                btnPrint.Text = "VAT 26";

            }

        }
        private void btnAddNew_Click(object sender, EventArgs e)
        {
            try
            {
                IsUpdate = false;
                AllClear();
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
        private void AllClear()
        {
            txtCategoryName.Text = "";
            txtCommentsDetail.Text = "";
            txtDisposeNo.Text = "";
            txtHSCode.Text = "";
            txtIsRaw.Text = "";
            txtLineNo.Text = "";
            txtNBRPrice.Text = "0.00";
            txtPCode.Text = "";
            txtPresentPrice.Text = "0.00";
            txtPrevious.Text = "0.00";
            txtProductCode.Text = "";
            txtProductName.Text = "";
            txtQuantity.Text = "0.00";
            txtQuantityInHand.Text = "0.00";
            txtRealPrice.Text = "0.00";
            txtTotalAmount.Text = "0.00";
            txtTotalSubTotal.Text = "0.00";
            txtTotalVATAmount.Text = "0.00";
            txtUOM.Text = "";
            txtVATRate.Text = "0.00";
            dtpDisposeDate.Value = Convert.ToDateTime(Program.SessionDate.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss"));
            dtpAppDate.Value = Convert.ToDateTime(Program.SessionDate.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss"));
            cmbProduct.SelectedIndex = -1;
            dgvDispose.Rows.Clear();
        }
        private void dgvPurchase_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                ////if (IsPost == false)
                ////{
                ////    MessageBox.Show("This transaction not posted", this.Text);
                ////    return;
                ////}
                if (Program.CheckLicence(dtpDisposeDate.Value) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }

                ////MDIMainInterface mdi = new MDIMainInterface();
                FormRptDispose frmRptDispose = new FormRptDispose();

                //mdi.RollDetailsInfo(frmRptPurchaseInformation.VFIN);
                //if (Program.Access != "Y")
                //{
                //    MessageBox.Show("You do not have to access this form", frmRptPurchaseInformation.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                //    return;
                //}

                #region TransactionType

                if (rbtnFinish.Checked)
                {
                    frmRptDispose.rbtnFinish.Checked = true; // result = FormPurchaseSearch.SelectOne("Other");
                }
                else if (rbtnRaw.Checked)
                {
                    frmRptDispose.rbtnRaw.Checked = true;
                    //result = FormPurchaseSearch.SelectOne("Trading"); //frm.rbtnTrading.Checked = true;
                }

                else
                {
                }

                #endregion

                if (txtDisposeNo.Text == "~~~ New ~~~")
                {
                    frmRptDispose.txtDisposeNo.Text = "";
                }
                else
                {
                    frmRptDispose.txtDisposeNo.Text = txtDisposeNo.Text.Trim();

                }
                frmRptDispose.ShowDialog();
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
        private void txtBENumber_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;

        }
        private void chkGotoVAT_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (rbtnFinish.Checked)
                {
                    if (chkFromStock.Checked == true)
                    {
                        chkFromStock.Text = "From warehouse";
                    }
                    else
                    {
                        chkFromStock.Text = "From outside";
                    }
                }
                else if (rbtnRaw.Checked)
                {
                    if (chkFromStock.Checked == true)
                    {
                        chkFromStock.Text = "From warehouse";
                    }
                    else
                    {
                        chkFromStock.Text = "From BOM";
                    }
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
                FileLogger.Log(this.Name, "chkGotoVAT_CheckedChanged", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "chkGotoVAT_CheckedChanged", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "chkGotoVAT_CheckedChanged", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "chkGotoVAT_CheckedChanged", exMessage);
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
                FileLogger.Log(this.Name, "chkGotoVAT_CheckedChanged", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "chkGotoVAT_CheckedChanged", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "chkGotoVAT_CheckedChanged", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "chkGotoVAT_CheckedChanged", exMessage);
            }
            #endregion
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #region backgroundWorker Event
        #endregion
        
        //===========================
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                string transactionType = string.Empty;
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
                if (Program.CheckLicence(dtpDisposeDate.Value) == true)
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
                    NextID = txtDisposeNo.Text.Trim();
                }

                if (txtComments.Text == "")
                {
                    txtComments.Text = "-";
                }

                if (txtRefNumber.Text == "")
                {
                    txtRefNumber.Text = "-";
                }

                if (txtAppRefNumber.Text == "")
                {
                    txtAppRefNumber.Text = "-";
                }
                if (txtAppRemarks.Text == "")
                {
                    txtAppRemarks.Text = "-";
                }
                if (txtAppTotalPrice.Text == "")
                {
                    txtAppTotalPrice.Text = "0";
                }
                if (txtAppVATAmount.Text == "")
                {
                    txtAppVATAmount.Text = "0";
                }
                if (txtAppTotalPriceImport.Text == "")
                {
                    txtAppTotalPriceImport.Text = "0";
                }
                if (txtAppVATAmountImport.Text == "")
                {
                    txtAppVATAmountImport.Text = "0";
                }


                if (dgvDispose.RowCount <= 0)
                {
                    MessageBox.Show("No Data In Grid to Save");
                    return;
                }

                disposeMasterVM = new DisposeMasterVM();

                #region Transaction Type

                if (rbtnRaw.Checked)
                {
                    transactionType = "VAT26";
                }
                else if (rbtnFinish.Checked)
                {
                    transactionType = "VAT27";

                }
                #endregion Transaction Type

                

                disposeMasterVM.DisposeNumber = NextID.ToString();
                disposeMasterVM.DisposeDate = dtpDisposeDate.Value.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss");
                disposeMasterVM.RefNumber = txtRefNumber.Text.Trim();
                disposeMasterVM.VATAmount = Convert.ToDecimal(txtTotalVATAmount.Text.Trim());
                disposeMasterVM.Remarks = txtComments.Text.Trim();
                disposeMasterVM.CreatedBy = Program.CurrentUser;
                disposeMasterVM.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                disposeMasterVM.LastModifiedBy = Program.CurrentUser;
                disposeMasterVM.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                disposeMasterVM.TransactionType = transactionType;
                disposeMasterVM.Post = "N";
                disposeMasterVM.FromStock = Convert.ToString(chkFromStock.Checked ? "Y" : "N");
                disposeMasterVM.ImportVATAmount = Convert.ToDecimal(txtTotalVATAmountImport.Text.Trim());
                disposeMasterVM.TotalPrice = Convert.ToDecimal(txtTotalSubTotal.Text.Trim());
                disposeMasterVM.TotalPriceImport = Convert.ToDecimal(txtTotalSubTotalImport.Text.Trim());
                disposeMasterVM.AppVATAmount = Convert.ToDecimal(txtAppVATAmount.Text.Trim());
                disposeMasterVM.AppTotalPrice = Convert.ToDecimal(txtAppTotalPrice.Text.Trim());
                disposeMasterVM.AppDate = dtpAppDate.Value.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss");
                disposeMasterVM.AppRefNumber = txtAppRefNumber.Text.Trim();
                disposeMasterVM.AppRemarks = txtAppRemarks.Text.Trim();
                disposeMasterVM.AppVATAmountImport = Convert.ToDecimal(txtAppVATAmountImport.Text.Trim());
                disposeMasterVM.AppTotalPriceImport = Convert.ToDecimal(txtAppTotalPriceImport.Text.Trim());
                disposeMasterVM.BranchId = Program.BranchId;



                disposeDetailVMs= new List<DisposeDetailVM>();

                for (int i = 0; i < dgvDispose.RowCount; i++)
                {
                    DisposeDetailVM disposeDetailVm = new DisposeDetailVM();

                    disposeDetailVm.DisposeNumberD = NextID.ToString();
                    disposeDetailVm.LineNumber = dgvDispose.Rows[i].Cells["LineNumber"].Value.ToString();
                    disposeDetailVm.ItemNo = dgvDispose.Rows[i].Cells["ItemNo"].Value.ToString();
                    disposeDetailVm.Quantity = Convert.ToDecimal(dgvDispose.Rows[i].Cells["Quantity"].Value.ToString());
                    disposeDetailVm.RealPrice = Convert.ToDecimal(dgvDispose.Rows[i].Cells["RealPrice"].Value.ToString());
                    disposeDetailVm.VATAmountD = Convert.ToDecimal(dgvDispose.Rows[i].Cells["VATAmt"].Value.ToString());
                    disposeDetailVm.SaleNumber = dgvDispose.Rows[i].Cells["SaleNumber"].Value.ToString();
                    disposeDetailVm.PurchaseNumber = dgvDispose.Rows[i].Cells["PurchaseNumber"].Value.ToString();
                    disposeDetailVm.PresentPrice = Convert.ToDecimal(dgvDispose.Rows[i].Cells["PresentPrice"].Value.ToString());
                    disposeDetailVm.CreatedByD = Program.CurrentUser;
                    disposeDetailVm.CreatedOnD = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                    disposeDetailVm.LastModifiedByD = Program.CurrentUser;
                    disposeDetailVm.LastModifiedOnD = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                    disposeDetailVm.DisposeDateD = dtpDisposeDate.Value.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss");
                    disposeDetailVm.UOM = dgvDispose.Rows[i].Cells["UOM"].Value.ToString();
                    disposeDetailVm.RemarksD = dgvDispose.Rows[i].Cells["Comments"].Value.ToString();
                    disposeDetailVm.QuantityImport = dgvDispose.Rows[i].Cells["QuantityImport"].Value.ToString();
                    disposeDetailVm.VATRate = Convert.ToDecimal(dgvDispose.Rows[i].Cells["VATRate"].Value.ToString());

                    

                    disposeDetailVMs.Add(disposeDetailVm);
                }//end for

                this.btnSave.Enabled = false;
                this.progressBar1.Visible = true;

                backgroundWorkerSave.RunWorkerAsync();

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
                DisposeDAL disposeDal = new DisposeDAL();
                sqlResults = disposeDal.DisposeInsert(disposeMasterVM, disposeDetailVMs,connVM);
                SAVE_DOWORK_SUCCESS = true;
                #endregion Statement

                //End DoWork
            }
            #region catch
            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                if (error.Length>1)
                {
                    MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning); 
                }
                else
                {
                    MessageBox.Show(error[0], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
               

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
                string exMessage = ex.Message;
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

                        if (string.IsNullOrEmpty(result))
                        {
                            throw new ArgumentNullException("backgroundWorkerSave_RunWorkerCompleted",
                                                            "Unexpected error.");
                        }
                        else if (result == "Success" || result == "Fail")
                        {
                            MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            IsUpdate = true;

                            txtDisposeNo.Text = sqlResults[2].ToString();
                            IsPost = Convert.ToString(sqlResults[3].ToString()) == "Y" ? true : false;
                            
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
                FileLogger.Log(this.Name, "backgroundWorkerSave_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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
        //===========================


        //=========================
        
        private void btnPost_Click(object sender, EventArgs e)
        {
            try
            {
                 if (
                MessageBox.Show(MessageVM.msgWantToPost + "\n" + MessageVM.msgWantToPost1, this.Text, MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    return;
                }
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

                if (searchBranchId != Program.BranchId && searchBranchId != 0)
                {
                    MessageBox.Show(MessageVM.SelfBranchNotMatch, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                string transactionType = string.Empty;

                if (Program.CheckLicence(dtpDisposeDate.Value) == true)
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
                    NextID = txtDisposeNo.Text.Trim();
                }

                if (txtComments.Text == "")
                {
                    txtComments.Text = "-";
                }
                if (txtRefNumber.Text == "")
                {
                    txtRefNumber.Text = "-";
                }

                if (txtAppRefNumber.Text == "")
                {
                    txtAppRefNumber.Text = "-";
                }
                if (txtAppRemarks.Text == "")
                {
                    txtAppRemarks.Text = "-";
                }
                if (txtAppTotalPrice.Text == "")
                {
                    txtAppTotalPrice.Text = "0";
                }
                if (txtAppVATAmount.Text == "")
                {
                    txtAppVATAmount.Text = "0";
                }

                if (txtAppVATAmountImport.Text == "")
                {
                    txtAppVATAmountImport.Text = "0";
                }
                if (txtAppTotalPriceImport.Text == "")
                {
                    txtAppTotalPriceImport.Text = "0";
                }

                if (dgvDispose.RowCount <= 0)
                {
                    MessageBox.Show("No Data In Grid to Save");
                    return;
                }

                disposeMasterVM = new DisposeMasterVM();

                #region Transaction Type

                if (rbtnRaw.Checked)
                {
                    transactionType = "VAT26";
                }
                else if (rbtnFinish.Checked)
                {
                    transactionType = "VAT27";

                }
                #endregion Transaction Type

                disposeMasterVM.DisposeNumber = NextID.ToString();
                disposeMasterVM.DisposeDate = dtpDisposeDate.Value.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss");
                disposeMasterVM.RefNumber = txtRefNumber.Text.Trim();
                disposeMasterVM.VATAmount = Convert.ToDecimal(txtTotalVATAmount.Text.Trim());
                disposeMasterVM.Remarks = txtComments.Text.Trim();
                disposeMasterVM.CreatedBy = Program.CurrentUser;
                disposeMasterVM.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                disposeMasterVM.LastModifiedBy = Program.CurrentUser;
                disposeMasterVM.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                disposeMasterVM.TransactionType = transactionType;
                disposeMasterVM.Post = "Y";
                disposeMasterVM.FromStock = Convert.ToString(chkFromStock.Checked ? "Y" : "N");
                disposeMasterVM.ImportVATAmount = Convert.ToDecimal(txtTotalVATAmountImport.Text.Trim());
                disposeMasterVM.TotalPrice = Convert.ToDecimal(txtTotalSubTotal.Text.Trim());
                disposeMasterVM.TotalPriceImport = Convert.ToDecimal(txtTotalSubTotalImport.Text.Trim());
                disposeMasterVM.AppVATAmount = Convert.ToDecimal(txtAppVATAmount.Text.Trim());
                disposeMasterVM.AppTotalPrice = Convert.ToDecimal(txtAppTotalPrice.Text.Trim());
                disposeMasterVM.AppDate = dtpAppDate.Value.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss");
                disposeMasterVM.AppRefNumber = txtAppRefNumber.Text.Trim();
                disposeMasterVM.AppRemarks = txtAppRemarks.Text.Trim();
                disposeMasterVM.AppVATAmountImport = Convert.ToDecimal(txtAppVATAmountImport.Text.Trim());
                disposeMasterVM.AppTotalPriceImport = Convert.ToDecimal(txtAppTotalPriceImport.Text.Trim());


                //encriptedIssueHeaderData = Converter.DESEncrypt(PassPhrase, EnKey, IssueHeaderData);
                disposeDetailVMs= new List<DisposeDetailVM>();
                for (int i = 0; i < dgvDispose.RowCount; i++)
                {
                    DisposeDetailVM disposeDetailVm = new DisposeDetailVM();

                    

                    disposeDetailVm.DisposeNumberD = NextID.ToString();
                    disposeDetailVm.LineNumber = dgvDispose.Rows[i].Cells["LineNumber"].Value.ToString();
                    disposeDetailVm.ItemNo = dgvDispose.Rows[i].Cells["ItemNo"].Value.ToString();
                    disposeDetailVm.Quantity = Convert.ToDecimal(dgvDispose.Rows[i].Cells["Quantity"].Value.ToString());
                    disposeDetailVm.RealPrice = Convert.ToDecimal(dgvDispose.Rows[i].Cells["RealPrice"].Value.ToString());
                    disposeDetailVm.VATAmountD = Convert.ToDecimal(dgvDispose.Rows[i].Cells["VATAmt"].Value.ToString());
                    disposeDetailVm.SaleNumber = dgvDispose.Rows[i].Cells["SaleNumber"].Value.ToString();
                    disposeDetailVm.PurchaseNumber = dgvDispose.Rows[i].Cells["PurchaseNumber"].Value.ToString();
                    disposeDetailVm.PresentPrice = Convert.ToDecimal(dgvDispose.Rows[i].Cells["PresentPrice"].Value.ToString());
                    disposeDetailVm.CreatedByD = Program.CurrentUser;
                    disposeDetailVm.CreatedOnD = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                    disposeDetailVm.LastModifiedByD = Program.CurrentUser;
                    disposeDetailVm.LastModifiedOnD = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                    disposeDetailVm.DisposeDateD = dtpDisposeDate.Value.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss");
                    disposeDetailVm.UOM = dgvDispose.Rows[i].Cells["UOM"].Value.ToString();
                    disposeDetailVm.RemarksD = dgvDispose.Rows[i].Cells["Comments"].Value.ToString();
                    disposeDetailVm.QuantityImport = dgvDispose.Rows[i].Cells["QuantityImport"].Value.ToString();
                    disposeDetailVm.VATRate = Convert.ToDecimal(dgvDispose.Rows[i].Cells["VATRate"].Value.ToString());

                    disposeDetailVMs.Add(disposeDetailVm);

                }//End For

                this.btnPost.Enabled = false;
                this.progressBar1.Visible = true;

                backgroundWorkerPost.RunWorkerAsync();
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
                DisposeDAL disposeDal = new DisposeDAL();
                sqlResults = disposeDal.DisposePost(disposeMasterVM, disposeDetailVMs,connVM);
                POST_DOWORK_SUCCESS = true;
                
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
                            throw new ArgumentNullException("backgroundWorkerPost_RunWorkerCompleted",
                                                            "Unexpected error.");
                        }
                        else if (result == "Success" || result == "Fail")
                        {
                            MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            IsUpdate = true;
                            txtDisposeNo.Text = sqlResults[2].ToString();
                            IsPost = Convert.ToString(sqlResults[3].ToString()) == "Y" ? true : false;
                            //txtItemNo.Text = newId;
                           
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
        //=========================


        //==========================
        
        private void ProductSearchDs()
        {

            //string ProductData = string.Empty;
            try
            {
                //ProductData =
                //    FieldDelimeter + // ItemNo,
                //    CategoryId + FieldDelimeter + // CategoryID,
                //    FieldDelimeter + // IsRaw,
                //    FieldDelimeter + // HSCodeNo,
                //    FieldDelimeter + // ActiveStatus,
                //    FieldDelimeter + // Trading, 
                //    FieldDelimeter + // NonStock,
                //    FieldDelimeter + // ProductCode

                //    LineDelimeter;

                ////Start DoWork
                //string encriptedProductData = Converter.DESEncrypt(PassPhrase, EnKey, ProductData);
                //ProductSoapClient ProductSearch = new ProductSoapClient();
                //ProductResultDs = ProductSearch.SearchMiniDS(encriptedProductData.ToString(), Program.DatabaseName);
                ////End DoWork

                ////Start Complete
                //ProductsMini.Clear();
                //foreach (DataRow item2 in ProductResultDs.Rows)
                //{
                //    var prod = new ProductMiniDTO();
                //    prod.ItemNo = item2["ItemNo"].ToString();
                //    prod.ProductName = item2["ProductName"].ToString();
                //    prod.ProductCode = item2["ProductCode"].ToString();
                //    prod.CategoryID = item2["CategoryID"].ToString();
                //    prod.CategoryName = item2["CategoryName"].ToString();
                //    prod.UOM = item2["UOM"].ToString();
                //    prod.HSCodeNo = item2["HSCodeNo"].ToString();
                //    prod.IsRaw = item2["IsRaw"].ToString();

                //    prod.CostPrice = Convert.ToDecimal(item2["CostPrice"].ToString());
                //    prod.SalesPrice = Convert.ToDecimal(item2["SalesPrice"].ToString());
                //    prod.NBRPrice = Convert.ToDecimal(item2["NBRPrice"].ToString());
                //    prod.ReceivePrice = Convert.ToDecimal(item2["ReceivePrice"].ToString());
                //    prod.IssuePrice = Convert.ToDecimal(item2["IssuePrice"].ToString());
                //    prod.Packetprice = Convert.ToDecimal(item2["Packetprice"].ToString());

                //    prod.TenderPrice = Convert.ToDecimal(item2["TenderPrice"].ToString());
                //    prod.ExportPrice = Convert.ToDecimal(item2["ExportPrice"].ToString());
                //    prod.InternalIssuePrice = Convert.ToDecimal(item2["InternalIssuePrice"].ToString());
                //    prod.TollIssuePrice = Convert.ToDecimal(item2["TollIssuePrice"].ToString());
                //    prod.TollCharge = Convert.ToDecimal(item2["TollCharge"].ToString());
                //    prod.OpeningBalance = Convert.ToDecimal(item2["OpeningBalance"].ToString());

                //    prod.VATRate = Convert.ToDecimal(item2["VATRate"].ToString());
                //    prod.SD = Convert.ToDecimal(item2["SD"].ToString());
                //    prod.TradingMarkUp = Convert.ToDecimal(item2["TradingMarkUp"].ToString());
                //    prod.Stock = Convert.ToDecimal(item2["Stock"].ToString());
                //    prod.QuantityInHand = Convert.ToDecimal(item2["QuantityInHand"].ToString());
                //    prod.NonStock = item2["NonStock"].ToString() == "Y" ? true : false;
                //    prod.Trading = item2["Trading"].ToString() == "Y" ? true : false;

                //    ProductsMini.Add(prod);

                //}//End For
                ////End Complete
                this.button2.Enabled = false;
                this.progressBar1.Visible = true;
                backgroundWorkerProductSearchDs.RunWorkerAsync();

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
                FileLogger.Log(this.Name, "ProductSearchDs", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "ProductSearchDs", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "ProductSearchDs", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "ProductSearchDs", exMessage);
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
                FileLogger.Log(this.Name, "ProductSearchDs", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ProductSearchDs", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ProductSearchDs", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "ProductSearchDs", exMessage);
            }
            #endregion
        }
        
        private void backgroundWorkerProductSearchDs_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                //Start DoWork
                //string encriptedProductData = Converter.DESEncrypt(PassPhrase, EnKey, ProductData);
                //ProductSoapClient ProductSearch = new ProductSoapClient();
                //ProductResultDs = ProductSearch.SearchMiniDS(encriptedProductData.ToString(), Program.DatabaseName);
                ProductResultDs= new DataTable();
                ProductDAL productDal = new ProductDAL();
                ProductResultDs = productDal.SearchProductMiniDS("", CategoryId, "", "", "", "", "", "",connVM);

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
                FileLogger.Log(this.Name, "backgroundWorkerProductSearchDs_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerProductSearchDs_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerProductSearchDs_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "backgroundWorkerProductSearchDs_DoWork", exMessage);
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
                FileLogger.Log(this.Name, "backgroundWorkerProductSearchDs_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerProductSearchDs_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerProductSearchDs_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "backgroundWorkerProductSearchDs_DoWork", exMessage);
            }
            #endregion
        }
        
        private void backgroundWorkerProductSearchDs_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
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

                    prod.TenderPrice = Convert.ToDecimal(item2["TenderPrice"].ToString());
                    prod.ExportPrice = Convert.ToDecimal(item2["ExportPrice"].ToString());
                    prod.InternalIssuePrice = Convert.ToDecimal(item2["InternalIssuePrice"].ToString());
                    prod.TollIssuePrice = Convert.ToDecimal(item2["TollIssuePrice"].ToString());
                    prod.TollCharge = Convert.ToDecimal(item2["TollCharge"].ToString());
                    prod.OpeningBalance = Convert.ToDecimal(item2["OpeningBalance"].ToString());

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
                ProductSearchDsLoad(); //No SOAP Service
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
                FileLogger.Log(this.Name, "backgroundWorkerProductSearchDs_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerProductSearchDs_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerProductSearchDs_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "backgroundWorkerProductSearchDs_RunWorkerCompleted", exMessage);
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
                FileLogger.Log(this.Name, "backgroundWorkerProductSearchDs_RunWorkerCompleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerProductSearchDs_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerProductSearchDs_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "backgroundWorkerProductSearchDs_RunWorkerCompleted", exMessage);
            }
            #endregion

            this.button2.Enabled = true;
            this.progressBar1.Visible = false;
        }
        //==========================


        //================PROBLEM========================
        
        private void btnPurchaseSearch_Click(object sender, EventArgs e)
        {
            try
            {
                ChangeData = false;
                this.btnPurchaseSearch.Enabled = false;
                this.progressBar1.Visible = true;

                string result;
                
                Program.fromOpen = "Me";


                DataGridViewRow selectedRow = FormPurchaseSearch.SelectOne("Other");
                if (selectedRow != null && selectedRow.Selected==true)
                {

                    txtPurchaseNumber.Text = selectedRow.Cells["PurchaseInvoiceNo"].Value.ToString();

                    txtCategoryName.Text = "Raw";
                    //string PurchaseDetailData = string.Empty;
                    if (txtPurchaseNumber.Text == "")
                    {
                        PurchaseDetailData = "00";
                    }
                    else
                    {
                        PurchaseDetailData = txtPurchaseNumber.Text.Trim();
                    }



                    backgroundWorkerbtnPurchaseSearch.RunWorkerAsync();
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
                FileLogger.Log(this.Name, "btnPurchaseSearch_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnPurchaseSearch_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnPurchaseSearch_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnPurchaseSearch_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnPurchaseSearch_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnPurchaseSearch_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnPurchaseSearch_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnPurchaseSearch_Click", exMessage);
            }
            #endregion
            finally
            {
                this.btnPurchaseSearch.Enabled =true ;
                this.progressBar1.Visible = false;
            }

        }
        
        private void backgroundWorkerbtnPurchaseSearch_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                
                ProductDAL productDal = new ProductDAL();
                ProductResultDs = new DataTable();
                ProductResultDs = productDal.SearchProductMiniDSDispose(PurchaseDetailData,connVM);

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
                FileLogger.Log(this.Name, "backgroundWorkerbtnPurchaseSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerbtnPurchaseSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerbtnPurchaseSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "backgroundWorkerbtnPurchaseSearch_DoWork", exMessage);
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
                FileLogger.Log(this.Name, "backgroundWorkerbtnPurchaseSearch_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerbtnPurchaseSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerbtnPurchaseSearch_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "backgroundWorkerbtnPurchaseSearch_DoWork", exMessage);
            }
            #endregion
        }
        
        private void backgroundWorkerbtnPurchaseSearch_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                //Start Complete
                ProductsMini.Clear();
                if (ProductResultDs == null)
                {
                    MessageBox.Show("there is no data");
                    return;
                }
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

                    prod.TenderPrice = Convert.ToDecimal(item2["TenderPrice"].ToString());
                    prod.ExportPrice = Convert.ToDecimal(item2["ExportPrice"].ToString());
                    prod.InternalIssuePrice = Convert.ToDecimal(item2["InternalIssuePrice"].ToString());
                    prod.TollIssuePrice = Convert.ToDecimal(item2["TollIssuePrice"].ToString());
                    prod.TollCharge = Convert.ToDecimal(item2["TollCharge"].ToString());
                    prod.OpeningBalance = Convert.ToDecimal(item2["OpeningBalance"].ToString());

                    prod.VATRate = Convert.ToDecimal(item2["VATRate"].ToString());
                    prod.SD = Convert.ToDecimal(item2["SD"].ToString());
                    prod.TradingMarkUp = Convert.ToDecimal(item2["TradingMarkUp"].ToString());
                    prod.Stock = Convert.ToDecimal(item2["Stock"].ToString());
                    prod.QuantityInHand = Convert.ToDecimal(item2["QuantityInHand"].ToString());
                    prod.NonStock = item2["NonStock"].ToString() == "Y" ? true : false;
                    prod.Trading = item2["Trading"].ToString() == "Y" ? true : false;
                    prod.PurchaseQty = Convert.ToDecimal(item2["PurchaseQty"].ToString());

                    ProductsMini.Add(prod);
                }
                ProductSearchDsLoad();
                //End Complete
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
                FileLogger.Log(this.Name, "backgroundWorkerbtnPurchaseSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerbtnPurchaseSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerbtnPurchaseSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "backgroundWorkerbtnPurchaseSearch_RunWorkerCompleted", exMessage);
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
                FileLogger.Log(this.Name, "backgroundWorkerbtnPurchaseSearch_RunWorkerCompleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerbtnPurchaseSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerbtnPurchaseSearch_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "backgroundWorkerbtnPurchaseSearch_RunWorkerCompleted", exMessage);
            }
            #endregion

            this.btnPurchaseSearch.Enabled = true;
            this.progressBar1.Visible = false;
        }
        //===============================================


        //===============================================
        
        private void btnSearchInvoiceNo_Click(object sender, EventArgs e)
        {
            //SOAP Service
            try
            {
               
                this.btnSearchInvoiceNo.Enabled = true;
                this.progressBar1.Visible = false;

                string result = string.Empty;
                //MDIMainInterface mdi = new MDIMainInterface();
                FormDisposeFinishSearch frm = new FormDisposeFinishSearch();
                //mdi.RollDetailsInfo(frm.VFIN);
                //if (Program.Access != "Y")
                //{
                //    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                //    return;
                //}

                Program.fromOpen = "Me";

                #region Transaction Type

                if (rbtnFinish.Checked)
                {
                    result = FormDisposeFinishSearch.SelectOne("VAT27"); //transactionType = "Other";
                }
                else if (rbtnRaw.Checked)
                {
                    result = FormDisposeFinishSearch.SelectOne("VAT26"); //transactionType = "Trading";
                }

                #endregion Transaction Type


                if (result == "")
                {
                    return;
                }
                else //if (result == ""){return;}else//if (result != "")
                {
                    string[] Saleinfo = result.Split(FieldDelimeter.ToCharArray());

                    txtDisposeNo.Text = Saleinfo[0];
                    dtpDisposeDate.Value = Convert.ToDateTime(Saleinfo[1]);
                    txtRefNumber.Text = Saleinfo[2];
                    txtComments.Text = Saleinfo[3];
                    IsPost = Convert.ToString(Saleinfo[4]) == "Y" ? true : false;
                    txtAppTotalPrice.Text = Saleinfo[5];
                    txtAppVATAmount.Text = Saleinfo[6];
                    dtpAppDate.Value = Convert.ToDateTime(Saleinfo[7]);
                    txtAppRefNumber.Text = Saleinfo[8];
                    txtAppRemarks.Text = Saleinfo[9];

                    txtAppVATAmountImport.Text = Saleinfo[10];
                    txtAppTotalPriceImport.Text = Saleinfo[11];

                    //string SaleDetailData = string.Empty;
                    if (txtDisposeNo.Text == "")
                    {
                        SaleDetailData = "0";
                        return;
                    }
                    else
                    {
                        SaleDetailData = txtDisposeNo.Text.Trim();
                    }

                    

                    backgroundWorkerSearchInvoiceNo.RunWorkerAsync();

                    //btnSave.Text = "&Save";

                    IsUpdate = true;
                    ChangeData = false;
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
                FileLogger.Log(this.Name, "btnSearchInvoiceNo_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnSearchInvoiceNo_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnSearchInvoiceNo_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnSearchInvoiceNo_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnSearchInvoiceNo_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnSearchInvoiceNo_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnSearchInvoiceNo_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnSearchInvoiceNo_Click", exMessage);
            }
            #endregion

        }

        private void backgroundWorkerSearchInvoiceNo_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                SaleDetailResult= new DataTable();
                //DisposeDAL disposeDal = new DisposeDAL();
                IDispose disposeDal = OrdinaryVATDesktop.GetObject<DisposeDAL, DisposeRepo, IDispose>(OrdinaryVATDesktop.IsWCF);
                SaleDetailResult = disposeDal.SearchDisposeDetailDTNew(SaleDetailData,connVM); // Change 04

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
                FileLogger.Log(this.Name, "backgroundWorkerSearchInvoiceNo_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerSearchInvoiceNo_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerSearchInvoiceNo_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "backgroundWorkerSearchInvoiceNo_DoWork", exMessage);
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
                FileLogger.Log(this.Name, "backgroundWorkerSearchInvoiceNo_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerSearchInvoiceNo_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerSearchInvoiceNo_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "backgroundWorkerSearchInvoiceNo_DoWork", exMessage);
            }
            #endregion
        }
        
        private void backgroundWorkerSearchInvoiceNo_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                //Start Complete
                dgvDispose.Rows.Clear();
                //dgvDispose.DataSource = null;
                if (SaleDetailResult != null)
                {
                    for (int i = 0; i < SaleDetailResult.Rows.Count; i++)
                    {
                        DataGridViewRow NewRow = new DataGridViewRow();
                        dgvDispose.Rows.Add(NewRow);

                        dgvDispose["LineNumber", dgvDispose.RowCount - 1].Value =SaleDetailResult.Rows[i]["LineNumber"].ToString();
                        dgvDispose["ItemNo", dgvDispose.RowCount - 1].Value =
                            SaleDetailResult.Rows[i]["ItemNo"].ToString();
                        dgvDispose["ItemName", dgvDispose.RowCount - 1].Value =
                            SaleDetailResult.Rows[i]["ItemName"].ToString();
                        dgvDispose["PCode", dgvDispose.RowCount - 1].Value =
                            SaleDetailResult.Rows[i]["PCode"].ToString();
                        dgvDispose["UOM", dgvDispose.RowCount - 1].Value =
                            SaleDetailResult.Rows[i]["UOM"].ToString();
                        dgvDispose["Quantity", dgvDispose.RowCount - 1].Value =
                            SaleDetailResult.Rows[i]["Quantity"].ToString();
                        dgvDispose["QuantityImport", dgvDispose.RowCount - 1].Value =
                            SaleDetailResult.Rows[i]["QuantityImport"].ToString();
                        dgvDispose["RealPrice", dgvDispose.RowCount - 1].Value =
                            SaleDetailResult.Rows[i]["RealPrice"].ToString();
                        dgvDispose["VATAmt", dgvDispose.RowCount - 1].Value =
                            SaleDetailResult.Rows[i]["VATAmt"].ToString();
                        dgvDispose["VATRate", dgvDispose.RowCount - 1].Value =
                            SaleDetailResult.Rows[i]["VATRate"].ToString();
                        dgvDispose["SaleNumber", dgvDispose.RowCount - 1].Value =
                            SaleDetailResult.Rows[i]["SaleNumber"].ToString();
                        dgvDispose["PurchaseNumber", dgvDispose.RowCount - 1].Value =
                            SaleDetailResult.Rows[i]["PurchaseNumber"].ToString();
                        dgvDispose["PresentPrice", dgvDispose.RowCount - 1].Value =
                            SaleDetailResult.Rows[i]["PresentPrice"].ToString();
                        dgvDispose["Comments", dgvDispose.RowCount - 1].Value =
                            SaleDetailResult.Rows[i]["Comments"].ToString();
                        dgvDispose["Stock", dgvDispose.RowCount - 1].Value = SaleDetailResult.Rows[i]["Stock"].ToString();
                        //SaleDetailResult.Rows[i]["Quantity"].ToString();
                        //SaleDetailResult.Rows[i]["QuantityImport"].ToString();

                        dgvDispose["Previous", dgvDispose.RowCount - 1].Value = 0;
                        dgvDispose["Status", dgvDispose.RowCount - 1].Value = 0;

                        //dgvIssue.Rows[j].Cells["Previous"].Value = item["Quantity"].ToString();//Convert.ToDecimal(IssueDetailFields[3].ToString()).ToString("0,0.0000"); //Quantity
                        //dgvIssue.Rows[j].Cells["Stock"].Value = item["Stock"].ToString();//Convert.ToDecimal(IssueDetailFields[12].ToString()).ToString("0,0.0000");


                    }

                    //dgvDispose.DataSource = SaleDetailResult;
                }
                Rowcalculate();
                //End Complete
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
                FileLogger.Log(this.Name, "backgroundWorkerSearchInvoiceNo_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerSearchInvoiceNo_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerSearchInvoiceNo_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "backgroundWorkerSearchInvoiceNo_RunWorkerCompleted", exMessage);
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
                FileLogger.Log(this.Name, "backgroundWorkerSearchInvoiceNo_RunWorkerCompleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerSearchInvoiceNo_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerSearchInvoiceNo_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "backgroundWorkerSearchInvoiceNo_RunWorkerCompleted", exMessage);
            }
            #endregion
            //Rowcalculate();
            finally
            {
                ChangeData = false;
                this.btnSearchInvoiceNo.Enabled = true;
                this.progressBar1.Visible = false;
            }


        }

        private void backgroundWorkerProductSearchDsFormLoad_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                //Start DoWork
                //string encriptedProductData = Converter.DESEncrypt(PassPhrase, EnKey, ProductData);
                //ProductSoapClient ProductSearch = new ProductSoapClient();
                //ProductResultDs = ProductSearch.SearchMiniDS(encriptedProductData.ToString(), Program.DatabaseName);

                ProductResultDs = new DataTable();

                ProductDAL productDal = new ProductDAL();
                ProductResultDs = productDal.SearchProductMiniDS("", "", varIsRaw, "", varActiveStatus, varTrading, varNonStock, "",connVM);

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

                    prod.TenderPrice = Convert.ToDecimal(item2["TenderPrice"].ToString());
                    prod.ExportPrice = Convert.ToDecimal(item2["ExportPrice"].ToString());
                    prod.InternalIssuePrice = Convert.ToDecimal(item2["InternalIssuePrice"].ToString());
                    prod.TollIssuePrice = Convert.ToDecimal(item2["TollIssuePrice"].ToString());
                    prod.TollCharge = Convert.ToDecimal(item2["TollCharge"].ToString());
                    prod.OpeningBalance = Convert.ToDecimal(item2["OpeningBalance"].ToString());

                    prod.VATRate = Convert.ToDecimal(item2["VATRate"].ToString());
                    prod.SD = Convert.ToDecimal(item2["SD"].ToString());
                    prod.TradingMarkUp = Convert.ToDecimal(item2["TradingMarkUp"].ToString());
                    prod.Stock = Convert.ToDecimal(item2["Stock"].ToString());
                    prod.QuantityInHand = Convert.ToDecimal(item2["QuantityInHand"].ToString());
                    prod.NonStock = item2["NonStock"].ToString() == "Y" ? true : false;
                    prod.Trading = item2["Trading"].ToString() == "Y" ? true : false;
                    //prod.PurchaseQty = Convert.ToDecimal(item2["PurchaseQty"].ToString());

                    ProductsMini.Add(prod);


                }//End For
                //End Complete
                ProductSearchDsLoad();
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
            finally
            {
                ChangeData = false;
                this.progressBar1.Visible = false;
            }
            
        }

        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                string transactionType = string.Empty;
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
                if (searchBranchId != Program.BranchId && searchBranchId != 0)
                {
                    MessageBox.Show(MessageVM.SelfBranchNotMatch, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (Program.CheckLicence(dtpDisposeDate.Value) == true)
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
                    NextID = txtDisposeNo.Text.Trim();
                }

                if (txtComments.Text == "")
                {
                    txtComments.Text = "-";
                }

                if (txtRefNumber.Text == "")
                {
                    txtRefNumber.Text = "-";
                }

                if (txtAppRefNumber.Text == "")
                {
                    txtAppRefNumber.Text = "-";
                }
                if (txtAppRemarks.Text == "")
                {
                    txtAppRemarks.Text = "-";
                }
                if (txtAppTotalPrice.Text == "")
                {
                    txtAppTotalPrice.Text = "0";
                }
                if (txtAppVATAmount.Text == "")
                {
                    txtAppVATAmount.Text = "0";
                }
                if (txtAppTotalPriceImport.Text == "")
                {
                    txtAppTotalPriceImport.Text = "0";
                }
                if (txtAppVATAmountImport.Text == "")
                {
                    txtAppVATAmountImport.Text = "0";
                }


                if (dgvDispose.RowCount <= 0)
                {
                    MessageBox.Show("No Data In Grid to Save");
                    return;
                }

                disposeMasterVM = new DisposeMasterVM();

                #region Transaction Type

                if (rbtnRaw.Checked)
                {
                    transactionType = "VAT26";
                }
                else if (rbtnFinish.Checked)
                {
                    transactionType = "VAT27";

                }

                #endregion Transaction Type

                disposeMasterVM.DisposeNumber = NextID.ToString();
                disposeMasterVM.DisposeDate = dtpDisposeDate.Value.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss");
                disposeMasterVM.RefNumber = txtRefNumber.Text.Trim();
                disposeMasterVM.VATAmount = Convert.ToDecimal(txtTotalVATAmount.Text.Trim());
                disposeMasterVM.Remarks = txtComments.Text.Trim();
                disposeMasterVM.CreatedBy = Program.CurrentUser;
                disposeMasterVM.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                disposeMasterVM.LastModifiedBy = Program.CurrentUser;
                disposeMasterVM.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                disposeMasterVM.TransactionType = transactionType;
                disposeMasterVM.Post = "N";
                disposeMasterVM.FromStock = Convert.ToString(chkFromStock.Checked ? "Y" : "N");
                disposeMasterVM.ImportVATAmount = Convert.ToDecimal(txtTotalVATAmountImport.Text.Trim());
                disposeMasterVM.TotalPrice = Convert.ToDecimal(txtTotalSubTotal.Text.Trim());
                disposeMasterVM.TotalPriceImport = Convert.ToDecimal(txtTotalSubTotalImport.Text.Trim());
                disposeMasterVM.AppVATAmount = Convert.ToDecimal(txtAppVATAmount.Text.Trim());
                disposeMasterVM.AppTotalPrice = Convert.ToDecimal(txtAppTotalPrice.Text.Trim());
                disposeMasterVM.AppDate = dtpAppDate.Value.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss");
                disposeMasterVM.AppRefNumber = txtAppRefNumber.Text.Trim();
                disposeMasterVM.AppRemarks = txtAppRemarks.Text.Trim();
                disposeMasterVM.AppVATAmountImport = Convert.ToDecimal(txtAppVATAmountImport.Text.Trim());
                disposeMasterVM.AppTotalPriceImport = Convert.ToDecimal(txtAppTotalPriceImport.Text.Trim());


                disposeDetailVMs = new List<DisposeDetailVM>();

                for (int i = 0; i < dgvDispose.RowCount; i++)
                {
                    DisposeDetailVM disposeDetailVm = new DisposeDetailVM();

                    disposeDetailVm.DisposeNumberD = NextID.ToString();
                    disposeDetailVm.LineNumber = dgvDispose.Rows[i].Cells["LineNumber"].Value.ToString();
                    disposeDetailVm.ItemNo = dgvDispose.Rows[i].Cells["ItemNo"].Value.ToString();
                    disposeDetailVm.Quantity = Convert.ToDecimal(dgvDispose.Rows[i].Cells["Quantity"].Value.ToString());
                    disposeDetailVm.RealPrice = Convert.ToDecimal(dgvDispose.Rows[i].Cells["RealPrice"].Value.ToString());
                    disposeDetailVm.VATAmountD = Convert.ToDecimal(dgvDispose.Rows[i].Cells["VATAmt"].Value.ToString());
                    disposeDetailVm.SaleNumber = dgvDispose.Rows[i].Cells["SaleNumber"].Value.ToString();
                    disposeDetailVm.PurchaseNumber = dgvDispose.Rows[i].Cells["PurchaseNumber"].Value.ToString();
                    disposeDetailVm.PresentPrice = Convert.ToDecimal(dgvDispose.Rows[i].Cells["PresentPrice"].Value.ToString());
                    disposeDetailVm.CreatedByD = Program.CurrentUser;
                    disposeDetailVm.CreatedOnD = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                    disposeDetailVm.LastModifiedByD = Program.CurrentUser;
                    disposeDetailVm.LastModifiedOnD = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                    disposeDetailVm.DisposeDateD = dtpDisposeDate.Value.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss");
                    disposeDetailVm.UOM = dgvDispose.Rows[i].Cells["UOM"].Value.ToString();
                    disposeDetailVm.RemarksD = dgvDispose.Rows[i].Cells["Comments"].Value.ToString();
                    disposeDetailVm.QuantityImport = dgvDispose.Rows[i].Cells["QuantityImport"].Value.ToString();
                    disposeDetailVm.VATRate = Convert.ToDecimal(dgvDispose.Rows[i].Cells["VATRate"].Value.ToString());



                    disposeDetailVMs.Add(disposeDetailVm);
                }//end for

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

        private void bgwUpdate_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {

                UPDATE_DOWORK_SUCCESS = false;
                sqlResults = new string[4];
                DisposeDAL disposeDal = new DisposeDAL();
                sqlResults = disposeDal.DisposeUpdate(disposeMasterVM, disposeDetailVMs,connVM);
                UPDATE_DOWORK_SUCCESS = true;

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

                if (UPDATE_DOWORK_SUCCESS)
                    if (sqlResults.Length > 0)
                    {
                        string result = sqlResults[0];
                        string message = sqlResults[1];

                        if (string.IsNullOrEmpty(result))
                        {
                            throw new ArgumentNullException("bgwUpdate_RunWorkerCompleted",
                                                            "Unexpected error.");
                        }
                        else if (result == "Success" || result == "Fail")
                        {
                            MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            IsUpdate = true;
                            txtDisposeNo.Text = sqlResults[2].ToString();
                            IsPost = Convert.ToString(sqlResults[3].ToString()) == "Y" ? true : false;
                            //txtItemNo.Text = newId;
                          
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

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void cmbProduct_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtRealPrice_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtQuantity_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtPresentPrice_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void txtVATRate_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { SendKeys.Send("{TAB}"); }
        }

        private void dtpDisposeDate_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { txtRefNumber.Focus(); }
            
        }

        private void txtRefNumber_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter)) { cmbProduct.Focus(); }
            
        }

        private void txtQuantityImport_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.KeyCode.Equals(Keys.Enter)) { btnAdd.Focus(); }
        }

        private void dtpDisposeDate_ValueChanged(object sender, EventArgs e)
        {
            ChangeData = true;

        }

        private void txtRefNumber_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;

        }

        private void txtRealPrice_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;

        }

        private void txtQuantity_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;

        }

        private void txtPresentPrice_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;

        }

        private void txtVATRate_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;

        }

        private void txtCommentsDetail_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;

        }

        private void txtQuantityImport_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;

        }

        private void txtComments_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;

        }

        private void txtAppTotalPrice_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;

        }

        private void txtAppTotalPriceImport_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;

        }

        private void txtAppVATAmount_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;

        }

        private void txtAppVATAmountImport_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;

        }

        private void dtpAppDate_ValueChanged(object sender, EventArgs e)
        {
            ChangeData = true;

        }

        private void txtAppRefNumber_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;

        }

        private void txtAppRemarks_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;

        }

        private void FormDisposeFinish_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
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
                FileLogger.Log(this.Name, "FormDeposit_FormClosing", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "FormDeposit_FormClosing", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "FormDeposit_FormClosing", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "FormDeposit_FormClosing", exMessage);
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
                FileLogger.Log(this.Name, "FormDeposit_FormClosing", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormDeposit_FormClosing", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "FormDeposit_FormClosing", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "FormDeposit_FormClosing", exMessage);
            }
            #endregion
        }

        private void txtQuantityInHand_Leave(object sender, EventArgs e)
        {
            if (Program.CheckingNumericTextBox(txtQuantityInHand, "Total Quantity") == true)
            {
                txtQuantityInHand.Text = Program.FormatingNumeric(txtQuantityInHand.Text.Trim(), IssuePlaceQty).ToString();

            }
        }

        private void txtQuantity_Leave(object sender, EventArgs e)
        {
            if (Program.CheckingNumericTextBox(txtQuantity, "Total Quantity") == true)
            {
                txtQuantity.Text = Program.FormatingNumeric(txtQuantity.Text.Trim(), IssuePlaceQty).ToString();

            }
        }

        private void txtPurchaseQty_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtPurchaseQty_Leave(object sender, EventArgs e)
        {
            Program.FormatTextBox(txtPurchaseQty, "PurchaseQty");
        }

        private void txtRealPrice_Leave(object sender, EventArgs e)
        {
            Program.FormatTextBox(txtRealPrice, "RealPrice");
        }

        private void txtPresentPrice_Leave(object sender, EventArgs e)
        {
            Program.FormatTextBox(txtPresentPrice, "PresentPrice");
        }

        private void txtVATRate_Leave(object sender, EventArgs e)
        {
            Program.FormatTextBox(txtVATRate, "VATRate");
        }

        private void txtTotalSubTotal_Leave(object sender, EventArgs e)
        {
            Program.FormatTextBox(txtTotalSubTotal, "TotalSubTotal");
        }

        private void txtAppVATAmountImport_Leave(object sender, EventArgs e)
        {
            Program.FormatTextBox(txtAppVATAmountImport, "AppVATAmountImport");
        }

        private void txtTotalSubTotalImport_Leave(object sender, EventArgs e)
        {
            Program.FormatTextBox(txtTotalSubTotalImport, "TotalSubTotalImport");
        }

        private void txtTotalVATAmount_Leave(object sender, EventArgs e)
        {
            Program.FormatTextBox(txtTotalVATAmount, "TotalVATAmount");
        }

        private void txtTotalVATAmountImport_Leave(object sender, EventArgs e)
        {
            Program.FormatTextBox(txtTotalVATAmountImport, "TotalVATAmountImport");
        }

        private void txtTotalAmount_Leave(object sender, EventArgs e)
        {
            Program.FormatTextBox(txtTotalAmount, "TotalAmount");
        }

        private void txtAppTotalPrice_Leave(object sender, EventArgs e)
        {
            Program.FormatTextBox(txtAppTotalPrice, "AppTotalPrice");
        }

        private void txtAppTotalPriceImport_Leave(object sender, EventArgs e)
        {
            Program.FormatTextBox(txtAppTotalPriceImport, "AppTotalPriceImport");
        }

        private void txtAppVATAmountImport_Leave_1(object sender, EventArgs e)
        {
            
        }

        private void txtAppVATAmount_Leave(object sender, EventArgs e)
        {
            Program.FormatTextBox(txtAppVATAmount, "AppVATAmount");
        }

   
    }
}
