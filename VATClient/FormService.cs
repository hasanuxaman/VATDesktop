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
//
using VATClient.ModelDTO;

using VATClient.ReportPreview;
using VATServer.Library;
using VATViewModel.DTOs;

namespace VATClient
{
    public partial class FormService : Form
    {
        public FormService()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();
        }
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        private List<ProductMiniDTO> ProductsMini = new List<ProductMiniDTO>();

        private const string LineDelimeter = DBConstant.LineDelimeter;
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private static string PassPhrase = DBConstant.PassPhrase;
        private static string EnKey = DBConstant.EnKey;


        //public const string Program.CurrentUser = DBConstant.Program.CurrentUser;
        //public const string Program.CurrentUser = DBConstant.Program.CurrentUser;

        private string[] sqlResults;
        private bool SAVE_DOWORK_SUCCESS = false;
        private bool DELETE_DOWORK_SUCCESS = false;
        private bool UPDATE_DOWORK_SUCCESS = false;

        private string CategoryId { get; set; }
        private DataTable ProductResultDs;
        public string VFIN = "172";
        private bool IsUpdate = false;
        private bool ChangeData = false;
        #region Global Variables For BackGroundWorker

        private DataTable BOMResult;

        private string varFinishItemNo = string.Empty;
        private string vBOMId = string.Empty;
        
        private string varEffectDate = string.Empty;
        private string varVATName = string.Empty;

        private List<ServiceVM> serviceVMs = new List<ServiceVM>();
        private List<BOMNBRVM> bomnbrvms = new List<BOMNBRVM>();

        #endregion

        private void FormService_Load(object sender, EventArgs e)
        {
            ClearAll();

            ChangeData = false;
        }

        private void chkPCode_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                #region Statement


                if (chkPCode.Checked == true)
                {
                    ProductSearchDsLoad();
                }
                else
                {
                    ProductSearchDsLoad();
                }
                cmbProduct.Focus();
                cmbProduct.Text = "";

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
                FileLogger.Log(this.Name, "chkPCode_CheckedChanged", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "chkPCode_CheckedChanged", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "chkPCode_CheckedChanged", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "chkPCode_CheckedChanged", exMessage);
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
                FileLogger.Log(this.Name, "chkPCode_CheckedChanged", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "chkPCode_CheckedChanged", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "chkPCode_CheckedChanged", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "chkPCode_CheckedChanged", exMessage);
            }
            #endregion

        }

        private void cmbProduct_Layout(object sender, LayoutEventArgs e)
        {

        }

        private void cmbProduct_Leave(object sender, EventArgs e)
        {
            try
            {
                #region Statement

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
                            txtProductId.Text = products.ItemNo;
                            //txtUnitCost.Text = products.IssuePrice.ToString();
                            txtUOM.Text = products.UOM;
                            txtHSCode.Text = products.HSCodeNo;
                            //txtQuantityInHand.Text = products.Stock.ToString();
                            txtPCode.Text = products.ProductCode;
                            txtSD.Text = products.SD.ToString();
                            txtVATRate.Text = products.VATRate.ToString();

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
                            txtProductId.Text = products.ItemNo;
                            //txtUnitCost.Text = products.IssuePrice.ToString();
                            txtUOM.Text = products.UOM;
                            txtHSCode.Text = products.HSCodeNo;
                            //txtQuantityInHand.Text = products.Stock.ToString();
                            txtPCode.Text = products.ProductCode;
                            txtSD.Text = products.SD.ToString();
                            txtVATRate.Text = products.VATRate.ToString();
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
            //No SOAP Service

            try
            {

                #region Statement
                if (string.IsNullOrEmpty(txtProductId.Text))
                {
                    MessageBox.Show("Select an Item Before Add to Details Section!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    cmbProduct.Focus();
                    return;
                }
                if (Convert.ToDecimal(txtUnitCost.Text) <= 0)
                {
                    MessageBox.Show("Please input the Base Price Value");
                    return;
                }
                for (int i = 0; i < dgvBOM.RowCount; i++)
                {
                    if (dgvBOM.Rows[i].Cells["ItemNo"].Value.ToString() == txtProductId.Text)
                    {
                        MessageBox.Show("Same item already exist.", this.Text);

                        return;
                    }
                }
                if (txtComments.Text == "")
                {
                    txtComments.Text = "-";
                }

                DataGridViewRow NewRow = new DataGridViewRow();
                dgvBOM.Rows.Add(NewRow);
                dgvBOM["ItemNo", dgvBOM.RowCount - 1].Value = txtProductId.Text.Trim();
                dgvBOM["ProductName", dgvBOM.RowCount - 1].Value = txtProductName.Text.Trim();
                dgvBOM["PCode", dgvBOM.RowCount - 1].Value = txtPCode.Text.Trim();
                dgvBOM["UOM", dgvBOM.RowCount - 1].Value = txtUOM.Text.Trim();
                dgvBOM["BasePrice", dgvBOM.RowCount - 1].Value = Convert.ToDecimal(Program.ParseDecimalObject(txtUnitCost.Text.Trim())).ToString("0.00");
                dgvBOM["SDRate", dgvBOM.RowCount - 1].Value = Convert.ToDecimal(Program.ParseDecimalObject(txtSD.Text.Trim())).ToString("0.00");
                dgvBOM["VATRate", dgvBOM.RowCount - 1].Value = Convert.ToDecimal(Program.ParseDecimalObject(txtVATRate.Text.Trim())).ToString("0.00");
                dgvBOM["Other", dgvBOM.RowCount - 1].Value = Convert.ToDecimal(Program.ParseDecimalObject(txtOther.Text.Trim())).ToString("0.00");
                dgvBOM["Comment", dgvBOM.RowCount - 1].Value = txtComments.Text.Trim();
                dgvBOM["HSCodeNo", dgvBOM.RowCount - 1].Value = txtHSCode.Text.Trim();
                dgvBOM["EffectDate", dgvBOM.RowCount - 1].Value = Convert.ToDateTime(dtpEffectDate.Value.ToString("yyyy-MMM-dd HH:mm:ss"));
                dgvBOM["BOMId", dgvBOM.RowCount - 1].Value = "0";

                Rowcalculate();

                AllClear();

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

            cmbProduct.Focus();
        }

        private void Rowcalculate()
        {
            try
            {
                #region Statement

                decimal BasePrice = 0;
                decimal SDRate = 0;
                decimal SDAmount = 0;
                decimal VATRate = 0;
                decimal VATAmount = 0;
                decimal Other = 0;
                decimal OtherAmount = 0;
                decimal salePrice = 0;

                for (int i = 0; i < dgvBOM.RowCount; i++)
                {

                    BasePrice = Convert.ToDecimal(dgvBOM["BasePrice", i].Value);
                    Other = Convert.ToDecimal(dgvBOM["Other", i].Value);
                    SDRate = Convert.ToDecimal(dgvBOM["SDRate", i].Value);
                    VATRate = Convert.ToDecimal(dgvBOM["VATRate", i].Value);


                    if (chkOther.Checked == true)
                    {
                        OtherAmount = Other;
                    }
                    else
                    {
                        OtherAmount = (BasePrice) * Other / 100;
                    }


                    SDAmount = (BasePrice + OtherAmount) * SDRate / 100;
                    VATAmount = (BasePrice + SDAmount + OtherAmount) * VATRate / 100;
                    salePrice = BasePrice + SDAmount + OtherAmount + VATAmount;

                    dgvBOM[0, i].Value = i + 1;
                    dgvBOM["SDAmount", i].Value = SDAmount.ToString("0,0.000");
                    dgvBOM["OtherAmount", i].Value = OtherAmount.ToString("0,0.000");
                    dgvBOM["VATAmount", i].Value = VATAmount.ToString("0,0.000");
                    dgvBOM["SalePrice", i].Value = salePrice.ToString("0,0.000");
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

        private void AllClear()
        {
            txtProductId.Text = "";
            txtProductName.Text = "";
            txtPCode.Text = "";
            txtHSCode.Text = "";
            txtUnitCost.Text = "0.00";
            txtSD.Text = "0.00";
            txtVATRate.Text = "0.00";
            txtOther.Text = "0.00";
            txtUOM.Text = "";

        }

        private void dgvBOM_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                #region Statement

                if (dgvBOM.Rows.Count <= 0)
                {
                    MessageBox.Show("There is no data to select.", this.Text, MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                    return;
                }
                //txtLineNo.Text = dgvBOM.CurrentCellAddress.Y.ToString();
                txtProductId.Text = dgvBOM.CurrentRow.Cells["ItemNo"].Value.ToString();
                txtProductName.Text = dgvBOM.CurrentRow.Cells["ProductName"].Value.ToString();
                txtPCode.Text = dgvBOM.CurrentRow.Cells["PCode"].Value.ToString();
                txtUOM.Text = dgvBOM.CurrentRow.Cells["UOM"].Value.ToString();
                txtUnitCost.Text = Convert.ToDecimal(dgvBOM.CurrentRow.Cells["Baseprice"].Value).ToString("0.00");
                txtSD.Text = Convert.ToDecimal(dgvBOM.CurrentRow.Cells["SDRate"].Value).ToString("0.00");
                txtVATRate.Text = Convert.ToDecimal(dgvBOM.CurrentRow.Cells["VATRate"].Value).ToString("0.00");
                txtOther.Text = Convert.ToDecimal(dgvBOM.CurrentRow.Cells["Other"].Value).ToString("0.00");
                txtComments.Text = dgvBOM.CurrentRow.Cells["Comment"].Value.ToString();
                txtHSCode.Text = dgvBOM.CurrentRow.Cells["HSCodeNo"].Value.ToString();
                dtpEffectDate.Value =Convert.ToDateTime(Convert.ToDateTime(dgvBOM.CurrentRow.Cells["EffectDate"].Value).ToString("yyyy/MMM/dd"));

                Textcalculate();


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
                FileLogger.Log(this.Name, "dgvBOM_DoubleClick", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "dgvBOM_DoubleClick", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "dgvBOM_DoubleClick", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "dgvBOM_DoubleClick", exMessage);
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
                FileLogger.Log(this.Name, "dgvBOM_DoubleClick", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "dgvBOM_DoubleClick", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "dgvBOM_DoubleClick", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "dgvBOM_DoubleClick", exMessage);
            }
            #endregion

        }

        private void dgvBOM_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnChange_Click(object sender, EventArgs e)
        {
            //No SOAP Service
            try
            {
                #region Statement
                if (dgvBOM.Rows.Count<=0)
                {
                     MessageBox.Show("No Items Found in Details Section.\nAdd New Items OR Load Previous Item !", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                     return;
                }

                //if (dgvBOM.RowCount > 0)
                //{
                //    ReceiveRemoveSingle();
                //}
                //else
                //{
                //    MessageBox.Show("No Items Found in Details Section.\nAdd New Items OR Load Previous Item !", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                //    return;
                //}

                if (string.IsNullOrEmpty(txtUnitCost.Text))
                {
                    MessageBox.Show("Please input the Base Price Value", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtUnitCost.Focus();
                    return;
                }

                if (txtComments.Text == "")
                {
                    txtComments.Text = "-";
                }



                dgvBOM["BasePrice", dgvBOM.CurrentRow.Index].Value = Convert.ToDecimal(Program.ParseDecimalObject(txtUnitCost.Text.Trim())).ToString("0.00");
                dgvBOM["SDRate", dgvBOM.CurrentRow.Index].Value = Convert.ToDecimal(Program.ParseDecimalObject(txtSD.Text.Trim())).ToString("0.00");
                dgvBOM["VATRate", dgvBOM.CurrentRow.Index].Value = Convert.ToDecimal(Program.ParseDecimalObject(txtVATRate.Text.Trim())).ToString("0.00");
                dgvBOM["Other", dgvBOM.CurrentRow.Index].Value = Convert.ToDecimal(Program.ParseDecimalObject(txtOther.Text.Trim())).ToString("0.00");

                dgvBOM["Comment", dgvBOM.CurrentRow.Index].Value = txtComments.Text.Trim();
                dgvBOM["EffectDate", dgvBOM.CurrentRow.Index].Value = Convert.ToDateTime(dtpEffectDate.Value.ToString("yyyy-MMM-dd HH:mm:ss"));


                dgvBOM.CurrentRow.DefaultCellStyle.ForeColor = Color.Green; // Blue;
                dgvBOM.CurrentRow.DefaultCellStyle.Font = new Font(this.Font, FontStyle.Regular);

                Rowcalculate();
                txtPCode.Text = "";
                txtProductName.Text = "";
                txtProductId.Text = "";
                txtCategoryName.Text = "";
                txtHSCode.Text = "";
                txtUnitCost.Text = "0.00";
                txtUOM.Text = "";

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

        private void btnRemove_Click(object sender, EventArgs e)
        {
            //No SOAP Service
            //if (dgvBOM.RowCount > 0)
            //{
            //    ReceiveRemoveSingle();
            //}
            //else
            //{
            //    MessageBox.Show("No Items Found in Details Section.\nAdd New Items OR Load Previous Item !", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //    return;
            //}
        }

        private void ReceiveRemoveSingle()
        {
            dgvBOM["BasePrice", dgvBOM.CurrentRow.Index].Value = "0.00";
            dgvBOM["SDRate", dgvBOM.CurrentRow.Index].Value = "0.00";
            dgvBOM["VATRate", dgvBOM.CurrentRow.Index].Value = "0.00";

            dgvBOM.CurrentRow.DefaultCellStyle.ForeColor = Color.Red;
            dgvBOM.CurrentRow.DefaultCellStyle.Font = new Font(this.Font, FontStyle.Strikeout);
           
            Rowcalculate();

            txtProductId.Text = "";
            txtPCode.Text = "";
            txtProductName.Text = "";
        }

        private void cmbProduct_KeyDown(object sender, KeyEventArgs e)
        {
            button2.Focus();
        }

        private void txtUnitCost_KeyDown(object sender, KeyEventArgs e)
        {
            //txtOther.Focus();
        }

        private void txtSD_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                txtComments.Focus();
            }
        }

        private void txtComments_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                btnAdd.Focus();
            }
        }

        private void txtUnitCost_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;

        }

        private void txtUnitCost_Leave(object sender, EventArgs e)
        {
            //Program.FormatTextBox(txtUnitCost, "Base Price");
            txtUnitCost.Text = Program.ParseDecimalObject(txtUnitCost.Text.Trim()).ToString();
            Textcalculate();
        }

        private void txtSD_Leave(object sender, EventArgs e)
        {
            //Program.FormatTextBoxRate(txtSD, "SD Rate");
            txtSD.Text = Program.ParseDecimalObject(txtSD.Text.Trim()).ToString();
            Textcalculate();
        }

        private void txtOther_Leave(object sender, EventArgs e)
        {
            //Program.FormatTextBox(txtOther, "Others");
            txtOther.Text = Program.ParseDecimalObject(txtOther.Text.Trim()).ToString();
            Textcalculate();
        }

        private void txtSD_TextChanged(object sender, EventArgs e)
        {

        }

        private void Textcalculate()
        {
            try
            {
                #region Statement

                decimal BasePrice = 0;
                decimal SDRate = 0;
                decimal SDAmount = 0;
                decimal VATRate = 0;
                decimal VATAmount = 0;
                decimal Other = 0;
                decimal OtherAmount = 0;
                decimal salePrice = 0;


                BasePrice = Convert.ToDecimal(txtUnitCost.Text);
                if (!string.IsNullOrEmpty(txtOther.Text))
                {
                    if (chkOther.Checked == true)
                    {
                        Other = Convert.ToDecimal(txtOther.Text);
                        OtherAmount = Other;
                    }
                    else
                    {
                        Other = Convert.ToDecimal(txtOther.Text);
                        OtherAmount = (BasePrice) * Other / 100;
                    }
                }
                
                SDRate = Convert.ToDecimal(txtSD.Text);
                SDAmount = (BasePrice + OtherAmount) * SDRate / 100;

                VATRate = Convert.ToDecimal(txtVATRate.Text);
                VATAmount = (BasePrice + SDAmount + OtherAmount) * VATRate / 100;
                salePrice = BasePrice + SDAmount + OtherAmount + VATAmount;

                txtSalePrice.Text = salePrice.ToString("0,0.000");

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
                FileLogger.Log(this.Name, "Textcalculate", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "Textcalculate", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "Textcalculate", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "Textcalculate", exMessage);
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
                FileLogger.Log(this.Name, "Textcalculate", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "Textcalculate", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "Textcalculate", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "Textcalculate", exMessage);
            }
            #endregion

        }

        private void txtVATRate_Leave(object sender, EventArgs e)
        {
            //Program.FormatTextBoxRate(txtVATRate, "VAT Rate");
            txtVATRate.Text = Program.ParseDecimalObject(txtVATRate.Text.Trim()).ToString();
            Textcalculate();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtOther_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;

        }

        private void chkOther_CheckedChanged(object sender, EventArgs e)
        {
            if (chkOther.Checked)
            {
                chkOther.Text = "Other(F)";
            }
            else
            {
                chkOther.Text = "Other(%)";
            }
        }

        private void cmbProduct_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeData = true;

        }

        private void txtVATRate_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            //No SOAP Service
            try
            {
                #region Statement
                if (dgvBOM == null || dgvBOM.RowCount <= 0)
                {
                    MessageBox.Show("No Data selected to print.");
                }
                else
                {


                    if (Program.CheckLicence(dtpEffectDate.Value) == true)
                    {
                        MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                        return;
                    }

                    FormRptVAT1 frmRptVAT1 = new FormRptVAT1();
                    MDIMainInterface mdi = new MDIMainInterface();
                    //mdi.RollDetailsInfo("8101");

                    //if (Program.Access != "Y")
                    //{
                    //    MessageBox.Show("You do not have to access this form", frmRptVAT1.Text, MessageBoxButtons.OK,
                    //                    MessageBoxIcon.Information);
                    //    return;
                    //}

                    if (dgvBOM.Rows.Count < 0)
                    {
                        MessageBox.Show("No Item to preview", this.Text);
                        return;
                    }

                    frmRptVAT1.txtItemNo.Text = dgvBOM.CurrentRow.Cells["ItemNo"].Value.ToString();
                    frmRptVAT1.txtProductName.Text = dgvBOM.CurrentRow.Cells["ProductName"].Value.ToString();
                    frmRptVAT1.txtVATName.Text = "Form Ka(Service)";
                    frmRptVAT1.dtpFromDate.Value = dtpEffectDate.Value;
                    frmRptVAT1.Show();

                    #endregion
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
                FileLogger.Log(this.Name, "btnPreview_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnPreview_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnPreview_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "btnPreview_Click", exMessage);
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
                FileLogger.Log(this.Name, "btnPreview_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnPreview_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnPreview_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "btnPreview_Click", exMessage);
            }
            #endregion

        }
        private void ClearAll()
        {
            try
            {
                #region Statement

                txtCategoryName.Text = "";
                txtComments.Text = "";
                txtHSCode.Text = "";
                txtOther.Text = "";
                txtPCode.Text = "";
                txtProductId.Text = "";
                txtProductName.Text = "";
                txtSD.Text = "";
                txtSalePrice.Text = "";
                txtUOM.Text = "";
                txtUnitCost.Text = "";
                txtVATRate.Text = "";
                cmbProduct.Items.Clear();
                cmbProductType.Items.Clear();
                dgvBOM.Rows.Clear();
                dtpEffectDate.Value = Convert.ToDateTime(Program.SessionDate.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss"));

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
        private void btnCancel_Click(object sender, EventArgs e)
        {
            ClearAll();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            // No SOAP Service

            try
            {
                #region Statement

                
                Program.fromOpen = "Me";

                DataGridViewRow selectedRow = FormBOMSearch.SelectOne("Service");


                if (selectedRow != null && selectedRow.Selected == true)
                {
                    //dtpEffectDate.Value = Convert.ToDateTime(selectedRow.Cells["EffectDate"].Value.ToString());

                    SearchDetails(selectedRow.Cells["BOMId"].Value.ToString());

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

        private void btnSave_Click(object sender, EventArgs e)
        {

            try
            {
                #region Statement

                if (dgvBOM.RowCount == 0)
                {
                    MessageBox.Show("Please add one item.");
                    return;
                }

                if (Program.CheckLicence(dtpEffectDate.Value) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete.");
                    return;
                }

                bomnbrvms = new List<BOMNBRVM>();

                for (int i = 0; i < dgvBOM.RowCount; i++)
                {
                    BOMNBRVM vBomnbrvms = new BOMNBRVM();

                    vBomnbrvms.ItemNo = dgvBOM.Rows[i].Cells["ItemNo"].Value.ToString();
                    vBomnbrvms.EffectDate =dgvBOM.Rows[i].Cells["EffectDate"].Value.ToString();
                    vBomnbrvms.VATName = "Form Ka(Service)";
                    vBomnbrvms.VATRate =Convert.ToDecimal( dgvBOM.Rows[i].Cells["VATRate"].Value.ToString());
                    vBomnbrvms.UOM = dgvBOM.Rows[i].Cells["UOM"].Value.ToString();
                    vBomnbrvms.SDRate = Convert.ToDecimal(dgvBOM.Rows[i].Cells["SDRate"].Value.ToString());
                    vBomnbrvms.TradingMarkup = Convert.ToDecimal(dgvBOM.Rows[i].Cells["Other"].Value.ToString());// Other
                    vBomnbrvms.MarkupValue = Convert.ToDecimal(dgvBOM.Rows[i].Cells["OtherAmount"].Value.ToString());// Other
                    
                    vBomnbrvms.TradingMarkup = 0;
                    vBomnbrvms.Comments = dgvBOM.Rows[i].Cells["Comment"].Value.ToString();
                    vBomnbrvms.ActiveStatus = "Y";
                    vBomnbrvms.CreatedBy = Program.CurrentUser;
                    vBomnbrvms.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                    vBomnbrvms.LastModifiedBy = Program.CurrentUser;
                    vBomnbrvms.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                    vBomnbrvms.RawTotal = 0;
                    vBomnbrvms.PackingTotal = 0;
                    vBomnbrvms.RebateTotal = 0;
                    vBomnbrvms.AdditionalTotal = 0;
                    vBomnbrvms.RebateAdditionTotal = 0;
                    vBomnbrvms.PNBRPrice =Convert.ToDecimal( dgvBOM.Rows[i].Cells["BasePrice"].Value.ToString());
                    vBomnbrvms.PPacketPrice = 0;
                    vBomnbrvms.RawOHCost = 0;
                    vBomnbrvms.LastNBRPrice = 0;
                    vBomnbrvms.LastNBRWithSDAmount = 0;
                    vBomnbrvms.TotalQuantity = 1;
                    vBomnbrvms.SDAmount =Convert.ToDecimal( dgvBOM.Rows[i].Cells["SDAmount"].Value.ToString());
                    vBomnbrvms.VatAmount =Convert.ToDecimal( dgvBOM.Rows[i].Cells["VATAmount"].Value.ToString());
                    vBomnbrvms.WholeSalePrice  =Convert.ToDecimal( dgvBOM.Rows[i].Cells["SalePrice"].Value.ToString());
                    vBomnbrvms.NBRWithSDAmount = Convert.ToDecimal(dgvBOM.Rows[i].Cells["SDAmount"].Value.ToString()) + Convert.ToDecimal(dgvBOM.Rows[i].Cells["BasePrice"].Value.ToString());
                    vBomnbrvms.MarkupValue = 0;
                    vBomnbrvms.LastMarkupValue = 0;
                    vBomnbrvms.LastSDAmount = 0;
                    vBomnbrvms.Post = "N";
                    //vBomnbrvms.BOMId = dgvBOM.Rows[i].Cells["BOMId"].Value.ToString();


                    
                    bomnbrvms.Add(vBomnbrvms);
                }




                this.btnSave.Enabled = false;
                this.progressBar1.Visible = true;

                backgroundWorkerSave.RunWorkerAsync();

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
                sqlResults = new string[3];
                BOMDAL bomdal = new BOMDAL();

                sqlResults = bomdal.ServiceInsert(bomnbrvms,connVM);
                SAVE_DOWORK_SUCCESS = true;
                ClearAll();
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
                            throw new ArgumentNullException("backgroundWorkerAdd_RunWorkerCompleted",
                                                            "Unexpected error.");
                        }
                        else if (result == "Success" || result == "Fail")
                        {
                            MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            IsUpdate = true;
                            string ids = sqlResults[2];
                            SearchDetails(ids);
                          
                        }


                    }


                #endregion
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
        private void SearchDetails(string BOMId)
        {
            try
            {
                #region Statement

                if (BOMId == "")
                {
                    return;
                }
                else
                {
                    vBOMId = BOMId;
                }


              

                this.progressBar1.Visible = true;
                backgroundWorkerSearchDetails.RunWorkerAsync();

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
                FileLogger.Log(this.Name, "SearchDetails", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "SearchDetails", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "SearchDetails", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "SearchDetails", exMessage);
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
                FileLogger.Log(this.Name, "SearchDetails", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "SearchDetails", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "SearchDetails", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "SearchDetails", exMessage);
            }
            #endregion

        }

        private void txtProductId_TextChanged(object sender, EventArgs e)
        {
            //var prodByCode = from prd in ProductsMini.ToList()
            //                 where prd.ItemNo.ToLower() == txtProductId.Text.ToLower()
            //                 select prd;
            //if (prodByCode != null && prodByCode.Any())
            //{
            //    var products = prodByCode.First();
            //    txtProductName.Text = products.ProductName;
            //    txtProductId.Text = products.ItemNo;
            //    //txtUnitCost.Text = products.IssuePrice.ToString();
            //    txtUOM.Text = products.UOM;
            //    txtHSCode.Text = products.HSCodeNo;
            //    //txtQuantityInHand.Text = products.Stock.ToString();
            //    txtPCode.Text = products.ProductCode;
            //    txtSD.Text = products.SD.ToString();
            //    txtVATRate.Text = products.VATRate.ToString();
            //}
        }

        private void btnProductName_Click(object sender, EventArgs e)
        {
            //No SOAP Service
        }
        //---------------------------------------------------------------------------
        private void button3_Click(object sender, EventArgs e)
        {
            LoadSearchItemGroup();
            ChangeData = false;
        }

        private void LoadSearchItemGroup()
        {
            try
            {
                #region Statement

                ////MDIMainInterface mdi = new MDIMainInterface();
                //FormProductCategorySearch frm = new FormProductCategorySearch();
                //mdi.RollDetailsInfo(frm.VFIN);
                //if (Program.Access != "Y")
                //{
                //    MessageBox.Show("You do not have to access this form", frm.Text, MessageBoxButtons.OK,
                //                    MessageBoxIcon.Information);
                //    return;
                //}

                Program.fromOpen = "Other";
                string result = FormProductCategorySearch.SelectOne();
                if (result == "")
                {
                    return;
                }
                else //if (result == ""){return;}else//if (result != "")
                {
                    string[] ProductCategoryInfo = result.Split(FieldDelimeter.ToCharArray());
                    CategoryId = ProductCategoryInfo[0];
                    txtCategoryName.Text = ProductCategoryInfo[1];
                    cmbProductType.Text = ProductCategoryInfo[4];
                }
                if (cmbProductType.Text != "Service")
                {
                    MessageBox.Show("Please select Service Item", this.Text);
                    txtCategoryName.Text = "";
                    cmbProductType.Text = "";
                    return;
                }
                this.progressBar1.Visible = true;
                this.button3.Enabled = false;
                ProductSearchDs(); //SOAP Service
                //ProductSearch();();
                //ProductSearchDsLoad();

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
                FileLogger.Log(this.Name, "button3_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "button3_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (FormatException ex)
            {
                FileLogger.Log(this.Name, "button3_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "button3_Click", exMessage);
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
                FileLogger.Log(this.Name, "button3_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "button3_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "button3_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "button3_Click", exMessage);
            }
            #endregion

            finally
            {
                //this.progressBar1.Visible = false;
            }
        }

        private void ProductSearchDs()
        {
            //string ProductData = string.Empty;
            try
            {
                this.progressBar1.Visible = true;
                this.button3.Enabled = false;
                #region Statement

                //ProductData =
                //                FieldDelimeter + // ItemNo,
                //                CategoryId + FieldDelimeter + // CategoryID,
                //                FieldDelimeter + // IsRaw,
                //                FieldDelimeter + // HSCodeNo,
                //                FieldDelimeter + // ActiveStatus,
                //                FieldDelimeter + // Trading, 
                //                FieldDelimeter + // NonStock,
                //                FieldDelimeter + // ProductCode
                //                LineDelimeter;

                //// Start DoWork
                //string encriptedProductData = Converter.DESEncrypt(PassPhrase, EnKey, ProductData);
                //ProductSoapClient ProductSearch = new ProductSoapClient();
                //ProductResultDs = ProductSearch.SearchMiniDS(encriptedProductData.ToString(), Program.DatabaseName);
                //// End DoWork

                #region Start Complete
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
                //    prod.VATRate = Convert.ToDecimal(item2["VATRate"].ToString());
                //    prod.SD = Convert.ToDecimal(item2["SD"].ToString());
                //    prod.TradingMarkUp = Convert.ToDecimal(item2["TradingMarkUp"].ToString());
                //    prod.Stock = Convert.ToDecimal(item2["Stock"].ToString());
                //    prod.QuantityInHand = Convert.ToDecimal(item2["QuantityInHand"].ToString());
                //    prod.NonStock = item2["NonStock"].ToString() == "Y" ? true : false;
                //    prod.Trading = item2["Trading"].ToString() == "Y" ? true : false;
                //    ProductsMini.Add(prod);

                //}// End For
                #endregion End Complete

                backgroundWorkerProductSearchDs.RunWorkerAsync();

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

        private void ProductSearchDsLoad()
        {
            // No SOAP Service
            try
            {
                #region Statement

                cmbProduct.Items.Clear();

                if (chkPCode.Checked == true)
                {
                    var prodByCode = from prd in ProductsMini.ToList()
                                     select prd.ProductCode;
                    if (prodByCode != null && prodByCode.Any())
                    {
                        cmbProduct.Items.AddRange(prodByCode.ToArray());
                    }
                }
                else
                {
                    var prodByCode = from prd in ProductsMini.ToList()
                                     select prd.ProductName;
                    if (prodByCode != null && prodByCode.Any())
                    {
                        cmbProduct.Items.AddRange(prodByCode.ToArray());
                    }
                }

                cmbProduct.Items.Insert(0, "Select");
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

        #region backgroundWorkerSearch Event

        private void backgroundWorkerProductSearchDs_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                // Start DoWork
                //string encriptedProductData = Converter.DESEncrypt(PassPhrase, EnKey, ProductData);
                //ProductSoapClient ProductSearch = new ProductSoapClient();
                //ProductResultDs = ProductSearch.SearchMiniDS(encriptedProductData.ToString(), Program.DatabaseName);
                ProductResultDs = new DataTable();

                ProductDAL productDAL = new ProductDAL();
                ProductResultDs = productDAL.SearchProductMiniDS("", CategoryId, "", "", "", "", "", "",connVM);

                // End DoWork
            }
            #region catch

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
                // Start Complete
                ProductsMini.Clear(); //aRaFaT commented
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

                }// End For
                // End Complete
                ProductSearchDsLoad();
            }
            #region catch

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

            this.progressBar1.Visible = false;
            this.button3.Enabled = true;
        }

        private void backgroundWorkerSearchDetails_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                BOMDAL bomdal = new BOMDAL();
                //vBOMId = "111','112','113";
                BOMResult = bomdal.SearchServicePrice(vBOMId,connVM);  // Change 04

                //End DoWork
            }
            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerSearchDetails_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerSearchDetails_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerSearchDetails_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "backgroundWorkerSearchDetails_DoWork", exMessage);
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
                FileLogger.Log(this.Name, "backgroundWorkerSearchDetails_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerSearchDetails_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerSearchDetails_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "backgroundWorkerSearchDetails_DoWork", exMessage);
            }
            #endregion
        }

        private void backgroundWorkerSearchDetails_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                //Start Complete
                dgvBOM.Rows.Clear();
                int j = 0;
                foreach (DataRow item in BOMResult.Rows)
                {
                    DataGridViewRow NewRow = new DataGridViewRow();

                    dgvBOM.Rows.Add(NewRow);
                    dgvBOM.Rows[j].Cells["LineNo"].Value = j+1;
                    dgvBOM.Rows[j].Cells["ItemNo"].Value = item["ItemNo"].ToString();
                    dgvBOM.Rows[j].Cells["ProductName"].Value = item["ProductName"].ToString();
                    dgvBOM.Rows[j].Cells["PCode"].Value = item["PCode"].ToString();
                    dgvBOM.Rows[j].Cells["UOM"].Value = item["UOM"].ToString();
                    dgvBOM.Rows[j].Cells["BasePrice"].Value = Program.ParseDecimalObject(item["BasePrice"].ToString());
                    dgvBOM.Rows[j].Cells["Other"].Value = Program.ParseDecimalObject(item["Other"].ToString()); //Convert.ToDecimal(BOMDFields[16].ToString()).ToString("0.00");//Tradingmarkup
                    dgvBOM.Rows[j].Cells["OtherAmount"].Value = Program.ParseDecimalObject(item["OtherAmount"].ToString()); // Convert.ToDecimal(BOMDFields[12].ToString()).ToString("0.00");//Packet price 
                    dgvBOM.Rows[j].Cells["Comment"].Value = item["Comment"].ToString(); // BOMDFields[8].ToString();
                    dgvBOM.Rows[j].Cells["SDRate"].Value = Program.ParseDecimalObject(item["SDRate"].ToString()); // BOMDFields[11].ToString();
                    dgvBOM.Rows[j].Cells["SDAmount"].Value = Program.ParseDecimalObject(item["SDAmount"].ToString()); // BOMDFields[4].ToString();
                    dgvBOM.Rows[j].Cells["VATRate"].Value = Program.ParseDecimalObject(item["VATRate"].ToString()); // BOMDFields[10].ToString();
                    dgvBOM.Rows[j].Cells["VATAmount"].Value = Program.ParseDecimalObject(item["VATAmount"].ToString()); // BOMDFields[4].ToString();
                    dgvBOM.Rows[j].Cells["SalePrice"].Value = Program.ParseDecimalObject(item["SalePrice"].ToString()); // Convert.ToDecimal(BOMDFields[15].ToString()).ToString("0.00");
                    dgvBOM.Rows[j].Cells["HSCodeNo"].Value = item["HSCodeNo"].ToString(); // BOMDFields[18].ToString();
                    dgvBOM.Rows[j].Cells["EffectDate"].Value = item["EffectDate"].ToString(); // BOMDFields[18].ToString();
                    dgvBOM.Rows[j].Cells["BOMId"].Value = item["BOMId"].ToString(); // BOMDFields[18].ToString();
                    j = j + 1;
                }//end For
                //End Complete
                //Rowcalculate();
            }
            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerSearchDetails_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerSearchDetails_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerSearchDetails_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "backgroundWorkerSearchDetails_RunWorkerCompleted", exMessage);
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
                FileLogger.Log(this.Name, "backgroundWorkerSearchDetails_RunWorkerCompleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerSearchDetails_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerSearchDetails_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "backgroundWorkerSearchDetails_RunWorkerCompleted", exMessage);
            }
            #endregion

            this.progressBar1.Visible = false;
        }
        



        #endregion

       

        private void btnUpdate_Click(object sender, EventArgs e)
        {

            try
            {
                #region Statement

                if (dgvBOM.RowCount == 0)
                {
                    MessageBox.Show("Please add one item.");
                    return;
                }

                if (Program.CheckLicence(dtpEffectDate.Value) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }

                bomnbrvms = new List<BOMNBRVM>();

                for (int i = 0; i < dgvBOM.RowCount; i++)
                {
                    BOMNBRVM vBomnbrvms = new BOMNBRVM();

                    vBomnbrvms.ItemNo = dgvBOM.Rows[i].Cells["ItemNo"].Value.ToString();
                    vBomnbrvms.EffectDate = dgvBOM.Rows[i].Cells["EffectDate"].Value.ToString();
                    vBomnbrvms.VATName = "Form Ka(Service)";
                    vBomnbrvms.VATRate = Convert.ToDecimal(dgvBOM.Rows[i].Cells["VATRate"].Value.ToString());
                    vBomnbrvms.UOM = dgvBOM.Rows[i].Cells["UOM"].Value.ToString();
                    vBomnbrvms.SDRate = Convert.ToDecimal(dgvBOM.Rows[i].Cells["SDRate"].Value.ToString());
                    vBomnbrvms.TradingMarkup = Convert.ToDecimal(dgvBOM.Rows[i].Cells["Other"].Value.ToString());// Other
                    vBomnbrvms.MarkupValue = Convert.ToDecimal(dgvBOM.Rows[i].Cells["OtherAmount"].Value.ToString());// Other

                    vBomnbrvms.TradingMarkup = 0;
                    vBomnbrvms.Comments = dgvBOM.Rows[i].Cells["Comment"].Value.ToString();
                    vBomnbrvms.ActiveStatus = "Y";
                    vBomnbrvms.CreatedBy = Program.CurrentUser;
                    vBomnbrvms.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                    vBomnbrvms.LastModifiedBy = Program.CurrentUser;
                    vBomnbrvms.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                    vBomnbrvms.RawTotal = 0;
                    vBomnbrvms.PackingTotal = 0;
                    vBomnbrvms.RebateTotal = 0;
                    vBomnbrvms.AdditionalTotal = 0;
                    vBomnbrvms.RebateAdditionTotal = 0;
                    vBomnbrvms.PNBRPrice = Convert.ToDecimal(dgvBOM.Rows[i].Cells["BasePrice"].Value.ToString());
                    vBomnbrvms.PPacketPrice = 0;
                    vBomnbrvms.RawOHCost = 0;
                    vBomnbrvms.LastNBRPrice = 0;
                    vBomnbrvms.LastNBRWithSDAmount = 0;
                    vBomnbrvms.TotalQuantity = 1;
                    vBomnbrvms.SDAmount = Convert.ToDecimal(dgvBOM.Rows[i].Cells["SDAmount"].Value.ToString());
                    vBomnbrvms.VatAmount = Convert.ToDecimal(dgvBOM.Rows[i].Cells["VATAmount"].Value.ToString());
                    vBomnbrvms.WholeSalePrice = Convert.ToDecimal(dgvBOM.Rows[i].Cells["SalePrice"].Value.ToString());
                    vBomnbrvms.NBRWithSDAmount = Convert.ToDecimal(dgvBOM.Rows[i].Cells["SDAmount"].Value.ToString()) + Convert.ToDecimal(dgvBOM.Rows[i].Cells["BasePrice"].Value.ToString());
                    vBomnbrvms.MarkupValue = 0;
                    vBomnbrvms.LastMarkupValue = 0;
                    vBomnbrvms.LastSDAmount = 0;
                    vBomnbrvms.Post = "N";
                    vBomnbrvms.BOMId = dgvBOM.Rows[i].Cells["BOMId"].Value.ToString();


                    bomnbrvms.Add(vBomnbrvms);
                }




                this.btnUpdate.Enabled = false;
                this.progressBar1.Visible = true;

                bgwUpdate.RunWorkerAsync();

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
                #region Statement


                UPDATE_DOWORK_SUCCESS = false;
                sqlResults = new string[3];
                BOMDAL bomdal = new BOMDAL();

                sqlResults = bomdal.ServiceUpdate(bomnbrvms,connVM);
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
                            throw new ArgumentNullException("bgwUpdate_RunWorkerCompleted",
                                                            "Unexpected error.");
                        }
                        else if (result == "Success" || result == "Fail")
                        {
                            MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            IsUpdate = true;
                           
                        }


                    }

                #endregion
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

        private void cmbProduct_Click(object sender, EventArgs e)
        {
            if (cmbProduct.Items.Count <= 0)
            {
                MessageBox.Show("Please Use Item Group Search & Select Service Item Group. ", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                //cmbProduct.Focus();
                LoadSearchItemGroup();
                //this.button3.Click += new EventHandler(button3_Click);
                //return;
            }
        }

        private void cmdPost_Click(object sender, EventArgs e)
        {

            try
            {
                #region Statement

                if (dgvBOM.RowCount == 0)
                {
                    MessageBox.Show("Please add one item.");
                    return;
                }

                if (Program.CheckLicence(dtpEffectDate.Value) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }

                bomnbrvms = new List<BOMNBRVM>();

                for (int i = 0; i < dgvBOM.RowCount; i++)
                {
                    BOMNBRVM vBomnbrvms = new BOMNBRVM();

                    vBomnbrvms.ItemNo = dgvBOM.Rows[i].Cells["ItemNo"].Value.ToString();
                    vBomnbrvms.EffectDate = dgvBOM.Rows[i].Cells["EffectDate"].Value.ToString();
                    vBomnbrvms.VATName = "Form Ka(Service)";
                    vBomnbrvms.VATRate = Convert.ToDecimal(dgvBOM.Rows[i].Cells["VATRate"].Value.ToString());
                    vBomnbrvms.UOM = dgvBOM.Rows[i].Cells["UOM"].Value.ToString();
                    vBomnbrvms.SDRate = Convert.ToDecimal(dgvBOM.Rows[i].Cells["SDRate"].Value.ToString());
                    vBomnbrvms.TradingMarkup = Convert.ToDecimal(dgvBOM.Rows[i].Cells["Other"].Value.ToString());// Other
                    vBomnbrvms.MarkupValue = Convert.ToDecimal(dgvBOM.Rows[i].Cells["OtherAmount"].Value.ToString());// Other

                    vBomnbrvms.TradingMarkup = 0;
                    vBomnbrvms.Comments = dgvBOM.Rows[i].Cells["Comment"].Value.ToString();
                    vBomnbrvms.ActiveStatus = "Y";
                    vBomnbrvms.CreatedBy = Program.CurrentUser;
                    vBomnbrvms.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                    vBomnbrvms.LastModifiedBy = Program.CurrentUser;
                    vBomnbrvms.LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                    vBomnbrvms.RawTotal = 0;
                    vBomnbrvms.PackingTotal = 0;
                    vBomnbrvms.RebateTotal = 0;
                    vBomnbrvms.AdditionalTotal = 0;
                    vBomnbrvms.RebateAdditionTotal = 0;
                    vBomnbrvms.PNBRPrice = Convert.ToDecimal(dgvBOM.Rows[i].Cells["BasePrice"].Value.ToString());
                    vBomnbrvms.PPacketPrice = 0;
                    vBomnbrvms.RawOHCost = 0;
                    vBomnbrvms.LastNBRPrice = 0;
                    vBomnbrvms.LastNBRWithSDAmount = 0;
                    vBomnbrvms.TotalQuantity = 1;
                    vBomnbrvms.SDAmount = Convert.ToDecimal(dgvBOM.Rows[i].Cells["SDAmount"].Value.ToString());
                    vBomnbrvms.VatAmount = Convert.ToDecimal(dgvBOM.Rows[i].Cells["VATAmount"].Value.ToString());
                    vBomnbrvms.WholeSalePrice = Convert.ToDecimal(dgvBOM.Rows[i].Cells["SalePrice"].Value.ToString());
                    vBomnbrvms.NBRWithSDAmount = 0;
                    vBomnbrvms.MarkupValue = 0;
                    vBomnbrvms.LastMarkupValue = 0;
                    vBomnbrvms.LastSDAmount = 0;
                    vBomnbrvms.Post = "Y";
                    vBomnbrvms.BOMId = dgvBOM.Rows[i].Cells["BOMId"].Value.ToString();


                    bomnbrvms.Add(vBomnbrvms);
                }

                this.btnUpdate.Enabled = false;
                this.progressBar1.Visible = true;

                bgwPost.RunWorkerAsync();

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
                FileLogger.Log(this.Name, "cmdPost_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "cmdPost_Click", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "cmdPost_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "cmdPost_Click", exMessage);
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
                FileLogger.Log(this.Name, "cmdPost_Click", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "cmdPost_Click", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "cmdPost_Click", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "cmdPost_Click", exMessage);
            }
            #endregion
        }

        private void bgwPost_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                #region Statement


                UPDATE_DOWORK_SUCCESS = false;
                sqlResults = new string[3];
                BOMDAL bomdal = new BOMDAL();

                sqlResults = bomdal.ServicePost(bomnbrvms,connVM);
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
                FileLogger.Log(this.Name, "bgwPost_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "bgwPost_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "bgwPost_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "bgwPost_DoWork", exMessage);
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
                FileLogger.Log(this.Name, "bgwPost_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwPost_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwPost_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "bgwPost_DoWork", exMessage);
            }
            #endregion

        }
        private void bgwPost_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
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
                            throw new ArgumentNullException("bgwPost_RunWorkerCompleted",
                                                            "Unexpected error.");
                        }
                        else if (result == "Success" || result == "Fail")
                        {
                            MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            IsUpdate = true;
                           
                        }


                    }

                #endregion
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
                FileLogger.Log(this.Name, "bgwPost_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "bgwPost_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "bgwPost_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "bgwPost_RunWorkerCompleted", exMessage);
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
                FileLogger.Log(this.Name, "bgwPost_RunWorkerCompleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwPost_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "bgwPost_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "bgwPost_RunWorkerCompleted", exMessage);
            }
            #endregion
            finally
            {
                ChangeData = false;
                this.btnUpdate.Enabled = true;
                this.progressBar1.Visible = false;
            }

        }

        private void txtOther_KeyDown(object sender, KeyEventArgs e)
        {
            //txtSalePrice.Focus();
        }

        private void txtSalePrice_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void dtpEffectDate_KeyDown(object sender, KeyEventArgs e)
        {
            cmbProduct.Focus();
        }

        private void btnAdd_KeyDown(object sender, KeyEventArgs e)
        {
            btnSave.Focus();
        }

        private void dtpEffectDate_ValueChanged(object sender, EventArgs e)
        {
            ChangeData = true;

        }

        private void txtSalePrice_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;

        }

        private void txtComments_TextChanged(object sender, EventArgs e)
        {
            ChangeData = true;

        }

        private void FormService_FormClosing(object sender, FormClosingEventArgs e)
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

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvBOM.RowCount > 0)
            {
                try
                {
                    
                    if (MessageBox.Show(MessageVM.msgWantToRemoveRow + "\nItem Code: " + dgvBOM.CurrentRow.Cells["ItemNo"].Value, this.Text, MessageBoxButtons.YesNo,
                                   MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        bomnbrvms = new List<BOMNBRVM>();

                        for (int i = 0; i < dgvBOM.RowCount; i++)
                        {
                            BOMNBRVM vBomnbrvms = new BOMNBRVM();

                            vBomnbrvms.BOMId = dgvBOM.Rows[i].Cells["BOMId"].Value.ToString();
                            vBomnbrvms.EffectDate = dgvBOM.Rows[i].Cells["EffectDate"].Value.ToString();
                            bomnbrvms.Add(vBomnbrvms);
                        }

                       
                        #region Statement
                        try
                        {
                            UPDATE_DOWORK_SUCCESS = false;
                            sqlResults = new string[3];
                            BOMDAL bomdal = new BOMDAL();

                            sqlResults = bomdal.ServiceDelete(bomnbrvms,connVM);
                            UPDATE_DOWORK_SUCCESS = true;
                            DeleteMsg();
                            dgvBOM.Rows.RemoveAt(dgvBOM.CurrentRow.Index);
                            Rowcalculate();

                            txtProductId.Text = "";
                            txtPCode.Text = "";
                            txtProductName.Text = "";
                        }
                        #endregion Statement
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
            else
            {
                MessageBox.Show("No Items Found in Details Section.\nAdd New Items OR Load Previous Item !", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
        }

        private void DeleteMsg()
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
                            throw new ArgumentNullException("btnDelete_Click",
                                                            "Unexpected error.");
                        }
                        else if (result == "Success" || result == "Fail")
                        {
                            MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            IsUpdate = true;

                        }


                    }

                #endregion Statement
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
            finally
            {
                ChangeData = false;
                this.btnDelete.Enabled = true;
                this.progressBar1.Visible = false;
            }
        }

        private void txtSalePrice_Leave(object sender, EventArgs e)
        {
            //Program.FormatTextBox(txtSalePrice, "SalePrice");
            txtSalePrice.Text = Program.ParseDecimalObject(txtSalePrice.Text.Trim()).ToString();
        }
    }
}

