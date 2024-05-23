using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Web.Services.Protocols;
using System.Windows.Forms;
//
//
using SymphonySofttech.Utilities;
//
//
//
using VATClient.ReportPreview;
using VATClient.ModelDTO;
using System.Collections.Generic;
using VATServer.Library;
using VATViewModel.DTOs;
using System.Data.OleDb;
using System.Diagnostics;
using CrystalDecisions.CrystalReports.Engine;
using SymphonySofttech.Reports.Report;
using SymphonySofttech.Reports.List;
using VATServer.License;
using VATServer.Interface;
using VATServer.Ordinary;
using VATDesktop.Repo;

namespace VATClient
{
    public partial class FormInputValueChanges : Form
    {
        #region Constructors

        public FormInputValueChanges()
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
        
        private const string FieldDelimeter = DBConstant.FieldDelimeter;

        ////new for NBR Price
        private List<string> ItemList = new List<string>();

        BOMDAL bomdal = new BOMDAL();
        //IBOM bomdal = OrdinaryVATDesktop.GetObject<BOMDAL, BOMRepo, IBOM>(OrdinaryVATDesktop.IsWCF);


        private bool IsPost = false;
        private string CurrentDate = "";
        private DataTable BOMResult;
        private decimal OldNBRPrice = 0;
        private decimal NewNBRPrice = 0;
        private decimal DifferencePrice = 0;
        private string BOMId = string.Empty;
        private string type = string.Empty;

        #region variable

        private DataTable BOMRawResult;
        private DataTable BOMMasterResult;
        private string effectDate = string.Empty;
        private string decriptedBOMDData = string.Empty;
        private string[] BOMDLines = new string[] { };
        private string[] BOMDFields = new string[] { };
        private DataTable OHResult;
        //private string decriptedOHData = string.Empty;

        private int BOMDPlaceQ;
        private int BOMDPlaceA;

        #endregion variable

        #endregion

        private void FormInputValueChanges_Load(object sender, EventArgs e)
        {

           
        }

        private void AllClear()
        {
            try
            {
                dtpEffectDate.Value =
                    Convert.ToDateTime(Program.SessionDate.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss"));
                //dtpEffectDate.Value = Program.SessionDate.ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss"));

                txtTrading.Text = "";
                txtComments.Text = "";
                txtFCategoryName.Text = "";
                txtFHSCode.Text = "";
                txtFProductCode.Text = "";
                txtFProductName.Text = "";
                txtGTotal.Text = "0.00";
                txtIsRaw.Text = "";
                txtLineNo.Text = "0.00";
                txtNBRPrice.Text = "0.00";
                txtNetCost.Text = "0.00000";
                txtOHCost.Text = "0.00";
                txtOHGTotal.Text = "0.00";
                txtPrevious.Text = "0.00";
                txtRCategoryName.Text = "";
                txtRHSCode.Text = "";
                txtRProductCode.Text = "";
                txtRProductName.Text = "";
                txtRQuantity.Text = "0";
                txtRSD.Text = "0.00";
                txtRUnitCost.Text = "0.00"; //s
                txtRUOM.Text = "";
                txtRVATRate.Text = "0.00";
                txtRWastage.Text = "0";
                txtRWastageInp.Text = "0";
                txtUomConv.Text = "0";
                txtTotalCost.Text = "0.00";
                txtTotalQty.Text = "0";
                cmbFProduct.Text = "Select";
                cmbOverHead.Text = "Select";
                cmbRProduct.Text = "Select";
                txtPacketPrice.Text = "0.00";
                txtPInvoiceNo.Text = "";

                dgvBOM.Rows.Clear();
                dgvOverhead.Rows.Clear();

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
                FileLogger.Log(this.Name, "AllClear", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "AllClear", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "AllClear", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "AllClear", exMessage);
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
                FileLogger.Log(this.Name, "AllClear", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "AllClear", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "AllClear", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "AllClear", exMessage);
            }

            #endregion Catch

        }

        private void Rowcalculate()
        {
            decimal SumRWCost = 0;
            try
            {

                for (int i = 0; i < dgvBOM.RowCount; i++)
                {
                    dgvBOM["LineNo", i].Value = i + 1;

                    dgvBOM["TotalQty", i].Value =
                        (Convert.ToDecimal(dgvBOM["Quantity", i].Value) + Convert.ToDecimal(dgvBOM["Wastage", i].Value))
                            .ToString("");
                    if (cmbType.Text.Trim() == "VAT 4.3 (Toll Issue)")
                    {
                        if (dgvBOM["InputType", i].Value.ToString() == "Overhead")
                        {
                            SumRWCost = SumRWCost + Convert.ToDecimal(dgvBOM["Cost", i].Value);
                        }
                    }
                    else
                    {
                        SumRWCost = SumRWCost + Convert.ToDecimal(dgvBOM["Cost", i].Value);
                    }
                }
                //txtTotalCost.Text = Convert.ToDecimal(SumRWCost).ToString("0.00");
                txtTotalCost.Text =
                    Convert.ToDecimal(Program.FormatingNumeric(SumRWCost.ToString(), BOMDPlaceA)).ToString();
                GTotal();
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

            #endregion Catch

        }

        private void GTotal()
        {
            try
            {
                if (txtRSD.Text == "")
                {
                    txtRSD.Text = "0";
                }
                if (txtRVATRate.Text == "")
                {
                    txtRVATRate.Text = "0";
                }
                if (txtTrading.Text == "")
                {
                    txtTrading.Text = "0";
                }
                if (txtPacketPrice.Text == "")
                {
                    txtPacketPrice.Text = "0";
                }
                decimal SubBOM = 0;
                decimal SubOH = 0;
                decimal PackPrice = 0;
                decimal Trading = 0;
                decimal SD = 0;
                decimal VATRate = 0;
                decimal TradingSum = 0;
                decimal SDSum = 0;
                decimal VATSum = 0;
                decimal SubTotal = 0;
                decimal GrandTotal = 0;
                decimal Margin = 0;


                Trading = Convert.ToDecimal(txtTrading.Text.Trim());
                SD = Convert.ToDecimal(txtRSD.Text.Trim());
                VATRate = Convert.ToDecimal(txtRVATRate.Text.Trim());

                SubBOM = Convert.ToDecimal(txtTotalCost.Text.Trim());
                SubOH = Convert.ToDecimal(txtOHGTotal.Text.Trim());
                Margin = Convert.ToDecimal(txtMargin.Text.Trim());
                SubTotal = SubBOM + SubOH + Margin;
                TradingSum = SubTotal*Trading/100;
                SDSum = (SubTotal + TradingSum)*SD/100;
                VATSum = (SubTotal + SDSum + TradingSum)*VATRate/100;
                GrandTotal = SubTotal + TradingSum + SDSum + VATSum;



                PackPrice = Convert.ToDecimal(txtPacketPrice.Text.Trim());
                if (cmbType.Text.Trim() != "VAT 1 Ka (Tarrif)")
                {
                    //txtGTotal.Text = Convert.ToDecimal(SubTotal).ToString("0");
                    txtGTotal.Text =
                        Convert.ToDecimal(Program.FormatingNumeric(SubTotal.ToString(), BOMDPlaceA)).ToString();
                }

                //txtGrandTotal.Text = Convert.ToDecimal(GrandTotal).ToString("0");
                txtGrandTotal.Text =
                    Convert.ToDecimal(Program.FormatingNumeric(GrandTotal.ToString(), BOMDPlaceA)).ToString();

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
                FileLogger.Log(this.Name, "GTotal", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "GTotal", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "GTotal", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "GTotal", exMessage);
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
                FileLogger.Log(this.Name, "GTotal", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "GTotal", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "GTotal", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "GTotal", exMessage);
            }

            #endregion Catch


        }

        private void RowcalculateOH()
        {
            try
            {
                decimal OHCost = 0;
                for (int i = 0; i < dgvOverhead.RowCount; i++)
                {
                    dgvOverhead["LineNo1", i].Value = i + 1;
                    OHCost = OHCost + Convert.ToDecimal(dgvOverhead["Cost1", i].Value);
                }

                //txtOHGTotal.Text = Convert.ToDecimal(OHCost).ToString("0.00");
                txtOHGTotal.Text = Convert.ToDecimal(Program.FormatingNumeric(OHCost.ToString(), BOMDPlaceA)).ToString();
                GTotal();
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
                FileLogger.Log(this.Name, "RowcalculateOH", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "RowcalculateOH", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "RowcalculateOH", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "RowcalculateOH", exMessage);
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
                FileLogger.Log(this.Name, "RowcalculateOH", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "RowcalculateOH", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "RowcalculateOH", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "RowcalculateOH", exMessage);
            }

            #endregion Catch
        }

        private void SumQty()
        {
            try
            {
                decimal TotalQty = 0;
                decimal TotalCost = 0;
                //decimal NetCost = 0;
                decimal RUnitCost = 0;
                decimal RQuantity = 0;
                decimal RWastage = 0;
                decimal RWastageInp = 0;


                //uomsValue();
                RUnitCost = Convert.ToDecimal(Convert.ToDecimal(txtRUnitCost.Text.Trim())*
                                              Convert.ToDecimal(txtUomConv.Text.Trim()));
                TotalQty = Convert.ToDecimal(txtTotalQty.Text.Trim());
                RWastageInp = Convert.ToDecimal(txtRWastageInp.Text.Trim());


                if (chkWastage.Checked)
                {
                    RWastage = Convert.ToDecimal(RWastageInp);
                }
                else
                {
                    RWastage = (RWastageInp*TotalQty)/(100 + RWastageInp);
                }
                RQuantity = TotalQty - RWastage;


                //TotalQty = RWastage + RQuantity;
                TotalCost = TotalQty*RUnitCost;


                txtRWastage.Text =
                    Convert.ToDecimal(Program.FormatingNumeric(RWastage.ToString(), BOMDPlaceQ)).ToString();
                txtRQuantity.Text =
                    Convert.ToDecimal(Program.FormatingNumeric(RQuantity.ToString(), BOMDPlaceQ)).ToString();
                txtNetCost.Text =
                    Convert.ToDecimal(Program.FormatingNumeric(TotalCost.ToString(), BOMDPlaceA)).ToString();
               
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
                FileLogger.Log(this.Name, "SumQty", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "SumQty", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "SumQty", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "SumQty", exMessage);
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
                FileLogger.Log(this.Name, "SumQty", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "SumQty", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "SumQty", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "SumQty", exMessage);
            }

            #endregion Catch

          
        }
      
        internal void SelectValue(List<string> itemList, string currentDate)
        {
            ItemList = itemList;
            CurrentDate = currentDate;
            VATName vname = new VATName();
            cmbType.DataSource = vname.VATNameList;
            GetSettingsValue();

            Search();
            //StoredData();
            ShowReport();
        }

        private void Search()
        {
            try
            {
                int j = 0;
                for (int i = 0; i < ItemList.Count; i++)
                {
                    BOMResult = bomdal.SearchInputValues("", "", "", "Y", ItemList[i],connVM);
                    //BOMResult = bomdal.SearchVAT1DTNew("", "", "", "Y", "114");
                    if (BOMResult != null || BOMResult.Rows.Count>0)
                    {
                        foreach (DataRow item in BOMResult.Rows)
                        {
                            DataGridViewRow NewRow = new DataGridViewRow();
                            dgvInputValue.Rows.Add(NewRow);

                            dgvInputValue.Rows[j].Cells["ILineNo"].Value = j + 1;
                            dgvInputValue.Rows[j].Cells["IBOMId"].Value = item["BOMId"].ToString();
                            dgvInputValue.Rows[j].Cells["IProCode"].Value = item["ProductCode"].ToString();
                            dgvInputValue.Rows[j].Cells["IProName"].Value = item["productname"].ToString();
                            dgvInputValue.Rows[j].Cells["IUom"].Value = item["UOM"].ToString();
                            dgvInputValue.Rows[j].Cells["IVatName"].Value = item["VATName"].ToString();
                            dgvInputValue.Rows[j].Cells["IEffectDate"].Value = item["EffectDate"].ToString();
                            dgvInputValue.Rows[j].Cells["IApprovedValue"].Value = item["NBRPrice"].ToString();
                            dgvInputValue.Rows[j].Cells["IComments"].Value = item["Comments"].ToString();

                            dgvInputValue.Rows[j].Cells["ILatestValue"].Value = "0";
                            dgvInputValue.Rows[j].Cells["Increase"].Value = "0";
                            dgvInputValue.Rows[j].Cells["IcurrentDate"].Value = CurrentDate;

                            j = j + 1;

                        }

                        for (int k = 0; k < BOMResult.Rows.Count; k++)
                        {
                            AllClear();
                            lblBOMId.Text = BOMResult.Rows[k]["BOMId"].ToString();
                            dtpEffectDate.Value = Convert.ToDateTime(BOMResult.Rows[k]["EffectDate"].ToString());
                            cmbType.Text = BOMResult.Rows[k]["VATName"].ToString();

                            ProductSearchDetails(BOMResult.Rows[k]["ProductCode"].ToString());
                            txtFProductCode.Text = BOMResult.Rows[k]["FinishItemNo"].ToString();
                            txtFProductName.Text = BOMResult.Rows[k]["ProductName"].ToString();
                            txtFProductCode1.Text = BOMResult.Rows[k]["ProductCode"].ToString();
                            txtTrading.Text = BOMResult.Rows[k]["TradingMarkUp"].ToString();
                            txtRSD.Text = BOMResult.Rows[k]["SD"].ToString();
                            txtRVATRate.Text = BOMResult.Rows[0]["VATRate"].ToString();
                            txtFUOM.Text = BOMResult.Rows[k]["UOM"].ToString();
                            txtComments.Text = BOMResult.Rows[k]["Comments"].ToString();
                            //txtFHSCode.Text = BOMResult.Rows[k]["HSCodeNo"].ToString();
                            IsPost = Convert.ToString(BOMResult.Rows[k]["Post"].ToString()) == "Y" ? true : false;

                            Rowcalculate();
                            RowcalculateOH();

                            var oldCost = Program.FormatingNumeric(txtTotalCost.Text, BOMDPlaceA);
                            OldNBRPrice = Convert.ToDecimal(oldCost);
                            BOMId = lblBOMId.Text;
                            type = cmbType.Text;
                            CalculateNBRPrice();
                            var newCost = Program.FormatingNumeric(txtTotalCost.Text, BOMDPlaceA);
                            NewNBRPrice = Convert.ToDecimal(newCost);
                            CalculateDifferencePrice();

                        }
                    }
                }
            }
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

            }
        }

        private void CalculateNBRPrice()
        {
            try
            {


                string PinvoiceNo = string.Empty;
                string InputType = string.Empty;
                string ItemNo = string.Empty;

                decimal uomPrice = 0;
                decimal uomC = 0;
                decimal UnitCost = 0;
                decimal Cost = 0;
                decimal TotalQty = 0;

                decimal PurchaseCostPrice = 0;
                decimal PurchaseQuantity = 0;
                decimal CostPrice = 0;
                decimal NewCostPrice = 0;

                DataTable dt = new DataTable();

                ProductDAL productDal = new ProductDAL();
                //IProduct productDal = OrdinaryVATDesktop.GetObject<ProductDAL, ProductRepo, IProduct>(OrdinaryVATDesktop.IsWCF);

                for (int i = 0; i < dgvBOM.RowCount; i++)
                {
                    ItemNo = dgvBOM["RawItemNo", i].Value.ToString();
                    InputType = dgvBOM["InputType", i].Value.ToString();
                    uomPrice = Convert.ToDecimal(dgvBOM["UOMPrice", i].Value);
                    uomC = Convert.ToDecimal(dgvBOM["UOMc", i].Value);
                    UnitCost = Convert.ToDecimal(dgvBOM["UnitCost", i].Value);
                    Cost = Convert.ToDecimal(dgvBOM["Cost", i].Value);
                    TotalQty = Convert.ToDecimal(dgvBOM["TotalQty", i].Value);
                    PinvoiceNo = dgvBOM["PinvoiceNo", i].Value.ToString();
                    CostPrice = Convert.ToDecimal(dgvBOM["CostPrice", i].Value);

                    if (InputType != "Overhead")
                    {
                        dt = new DataTable();
                        dt = productDal.GetLIFOPurchaseInformation(ItemNo, CurrentDate,"",connVM);
                        if (dt.Rows.Count > 0)
                        {
                            PurchaseCostPrice = Convert.ToDecimal(dt.Rows[0]["PurchaseCostPrice"].ToString());
                            PurchaseQuantity = Convert.ToDecimal(dt.Rows[0]["PurchaseQuantity"].ToString());
                            CostPrice = Convert.ToDecimal(dt.Rows[0]["CostPrice"].ToString());
                            NewCostPrice = 0;

                            PinvoiceNo = dt.Rows[0]["PurchaseInvoiceNo"].ToString();
                            if (PurchaseQuantity != 0)
                            {
                                NewCostPrice = PurchaseCostPrice/PurchaseQuantity;
                            }

                            UnitCost = NewCostPrice*uomC;
                            Cost = NewCostPrice*uomC*TotalQty;

                        }
                        else
                        {
                            PinvoiceNo = "0";
                        }

                        dgvBOM["UOMPrice", i].Value = NewCostPrice;
                        dgvBOM["UnitCost", i].Value = UnitCost;
                        dgvBOM["Cost", i].Value = Cost;
                        dgvBOM["PinvoiceNo", i].Value = PinvoiceNo;
                        dgvBOM["CostPrice", i].Value = CostPrice;

                    }



                }
                Rowcalculate();
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
                FileLogger.Log(this.Name, "btnCosting_Click",

                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "btnCosting_Click",

                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "btnCosting_Click",

                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (SoapHeaderException ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine +

                                ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }

                FileLogger.Log(this.Name, "btnCosting_Click",

                               exMessage);
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (SoapException ex)
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
                FileLogger.Log(this.Name, "btnCosting_Click",

                               exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnCosting_Click",

                               ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text,

                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "btnCosting_Click",

                               ex.Message + Environment.NewLine + ex.StackTrace);
            }
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
                FileLogger.Log(this.Name, "btnCosting_Click",

                               exMessage);
            }

            #endregion
        }

        private void CalculateDifferencePrice()
        {
            try
            {

                if (OldNBRPrice != 0)
                {
                    DifferencePrice = (NewNBRPrice - OldNBRPrice)/OldNBRPrice*100;
                    var tt = Program.FormatingNumeric(DifferencePrice.ToString(), BOMDPlaceA);
                    DifferencePrice = Convert.ToDecimal(tt);

                }
                else
                {
                    DifferencePrice = Convert.ToDecimal(0);
                }
                
               
                for (int i = 0; i < dgvInputValue.Rows.Count; i++)
                {
                    if (BOMId == dgvInputValue["IBOMId", i].Value.ToString() )
                    {
                        if(type == dgvInputValue["IVatName",i].Value.ToString())
                        {
                        dgvInputValue["IApprovedValue", i].Value = OldNBRPrice.ToString();
                        dgvInputValue["ILatestValue", i].Value = NewNBRPrice.ToString();
                        dgvInputValue["Increase", i].Value = DifferencePrice.ToString();
                        }
                    }
                }
            }
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

            }
        }

        private void ProductSearchDetails(string ProductID)
        {
            type = string.Empty;
            effectDate = string.Empty;
            decriptedBOMDData = string.Empty;
            BOMDLines = new string[] { };
            BOMDFields = new string[] { };

            type = cmbType.Text.Trim();
            effectDate = dtpEffectDate.Value.ToString("yyyy-MMM-dd");

            try
            {
                #region Statement

                BOMMasterResult = new DataTable();
                BOMRawResult = new DataTable();
                OHResult = new DataTable();

                BOMDAL bomdal = new BOMDAL();
                //IBOM bomdal = OrdinaryVATDesktop.GetObject<BOMDAL, BOMRepo, IBOM>(OrdinaryVATDesktop.IsWCF);

                string strBomId = lblBOMId.Text;

                BOMMasterResult = bomdal.SearchBOMMaster(ProductID, type, effectDate,"0",connVM);
                BOMRawResult = bomdal.SearchBOMRaw(ProductID, type, effectDate,"0",connVM); // Change 04
                OHResult = bomdal.SearchOH(ProductID, type, effectDate,"0",connVM); // Change 04
                
                #endregion

                #region product search

                // Start Complete

                if (BOMRawResult != null)
                {
                    dgvBOM.Rows.Clear();
                    int j = 0;
                    foreach (DataRow item in BOMRawResult.Rows)
                    {

                        string strUseQty = item["UseQuantity"].ToString();
                        string strWastageQty = item["WastageQuantity"].ToString();

                        decimal vUseQty = 0;
                        decimal vWastageQty = 0;
                        decimal vTotalQty = 0;
                        decimal vCost = 0;
                        decimal vRebateRate = 0;
                        decimal vHeadAmount = 0;
                        if (!string.IsNullOrEmpty(strUseQty))
                        {
                            vUseQty = Convert.ToDecimal(strUseQty);
                        }
                        if (!string.IsNullOrEmpty(strWastageQty))
                        {
                            vWastageQty = Convert.ToDecimal(strWastageQty);
                        }


                        vTotalQty = vUseQty + vWastageQty;


                        DataGridViewRow NewRow = new DataGridViewRow();

                        dgvBOM.Rows.Add(NewRow);

                        dgvBOM.Rows[j].Cells["LineNo"].Value = j + 1;
                        dgvBOM.Rows[j].Cells["RawItemNo"].Value = item["RawItemNo"].ToString();
                        dgvBOM.Rows[j].Cells["ProductName"].Value = item["ProductName"].ToString();
                        dgvBOM.Rows[j].Cells["UOM"].Value = item["UOM"].ToString(); // BOMDFields[4].ToString();
                        dgvBOM.Rows[j].Cells["Quantity"].Value = item["UseQuantity"].ToString();
                        dgvBOM.Rows[j].Cells["Wastage"].Value = item["WastageQuantity"].ToString();
                        dgvBOM.Rows[j].Cells["Cost"].Value = item["Cost"].ToString();
                        dgvBOM.Rows[j].Cells["ActiveStatus"].Value = item["ActiveStatus"].ToString();
                        dgvBOM.Rows[j].Cells["Status"].Value = "Old";
                        //NBR1 = item["NBRPrice"].ToString();
                        dgvBOM.Rows[j].Cells["UnitCost"].Value = item["UnitCost"].ToString();
                        dgvBOM.Rows[j].Cells["RawPCode"].Value = item["ProductCode"].ToString();
                        dgvBOM.Rows[j].Cells["UOMPrice"].Value = item["UOMPrice"].ToString();
                        dgvBOM.Rows[j].Cells["UOMc"].Value = item["UOMc"].ToString(); // BOMDFields[20].ToString();
                        dgvBOM.Rows[j].Cells["UOMn"].Value = item["UOMn"].ToString(); // BOMDFields[21].ToString();
                        dgvBOM.Rows[j].Cells["UOMUQty"].Value = item["UOMUQty"].ToString();
                        dgvBOM.Rows[j].Cells["UOMWQty"].Value = item["UOMWQty"].ToString();
                        dgvBOM.Rows[j].Cells["RebatePercent"].Value = item["RebateRate"].ToString();
                        dgvBOM.Rows[j].Cells["TotalQty"].Value = vTotalQty;
                        dgvBOM.Rows[j].Cells["InputType"].Value = item["rawitemtype"].ToString();
                        dgvBOM.Rows[j].Cells["PBOMId"].Value = item["PBOMId"].ToString();
                        dgvBOM.Rows[j].Cells["PInvoiceNo"].Value = item["PInvoiceNo"].ToString();
                        dgvBOM.Rows[j].Cells["CostPrice"].Value = "0";


                        string strCost = item["Cost"].ToString();
                        string strRebateRate = item["RebateRate"].ToString();
                        if (!string.IsNullOrEmpty(strCost))
                        {
                            vCost = Convert.ToDecimal(strCost);
                        }
                        if (!string.IsNullOrEmpty(strRebateRate))
                        {
                            vRebateRate = Convert.ToDecimal(strRebateRate);
                        }
                        if (vRebateRate > 0)
                        {
                            vHeadAmount = vCost * 100 / vRebateRate;
                        }

                        dgvBOM.Rows[j].Cells["HeadAmountBOM"].Value = vHeadAmount;
                        j = j + 1;

                    }
                }

                // End Complete

                #endregion product search

                #region Overhead search

                if (OHResult != null)
                {
                    dgvOverhead.Rows.Clear();

                    txtMargin.Text = "0.00";

                    int j = 0;

                    foreach (DataRow item in OHResult.Rows)
                    {
                        if (item["HeadName"].ToString() == "Margin")
                        {
                            txtMargin.Text = item["HeadAmount"].ToString();
                        }


                        if (item["HeadName"].ToString() != "Margin")
                        {
                            DataGridViewRow NewRow = new DataGridViewRow();
                            dgvOverhead.Rows.Add(NewRow);
                            dgvOverhead.Rows[j].Cells["HeadName1"].Value = item["HeadName"].ToString();
                            dgvOverhead.Rows[j].Cells["HeadAmount"].Value = item["HeadAmount"].ToString();
                            dgvOverhead.Rows[j].Cells["Cost1"].Value = item["AdditionalCost"].ToString();
                            dgvOverhead.Rows[j].Cells["LineNo1"].Value = item["OHLineNo"].ToString();
                            dgvOverhead.Rows[j].Cells["Percent"].Value = item["RebatePercent"].ToString();
                            dgvOverhead.Rows[j].Cells["OHCode"].Value = item["OHCode"].ToString();
                            dgvOverhead.Rows[j].Cells["HeadID"].Value = item["HeadID"].ToString();
                            j = j + 1;

                        }

                    }
                }

                #endregion Overhead search

                #region BOMMaster Data

                foreach (DataRow item in BOMMasterResult.Rows)
                {


                    //BOMMasterResult
                    txtComments.Text = item["Comments"].ToString(); // BOMDFields[8].ToString();
                    txtRVATRate.Text = item["VATRate"].ToString(); // BOMDFields[10].ToString();
                    txtRSD.Text = item["SD"].ToString(); // BOMDFields[11].ToString();
                    cmbType.Text = item["VATName"].ToString(); // BOMDFields[14].ToString();
                    txtTrading.Text = item["TradingMarkUp"].ToString();
                    txtPacketPrice.Text = item["PacketPrice"].ToString();
                    txtGrandTotal.Text = item["WholeSalePrice"].ToString();
                    txtTotalCost.Text = item["RawOHCost"].ToString();
                    OldNBRPrice = Convert.ToDecimal(txtTotalCost.Text);
                }

                #endregion


                #region Other method call and assignment

                //Rowcalculate();
                RowcalculateOH();
               
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
            catch (SqlException ex)
            {
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_RunWorkerCompleted",
                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_RunWorkerCompleted",
                               ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerUpdate_RunWorkerCompleted",
                               ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_RunWorkerCompleted",
                               ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerUpdate_RunWorkerCompleted",
                               ex.Message + Environment.NewLine + ex.StackTrace);
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
        }

        private void StoredData()
        {
            List<InputValueChange> inputValues = new List<InputValueChange>();
            InputValueChange inputValue = new InputValueChange();

            for (int i = 0; i < dgvInputValue.Rows.Count; i++)
            {
                inputValue.ProductCode = dgvInputValue["IProCode", i].Value.ToString();
                inputValue.ProductName = dgvInputValue["IProName", i].Value.ToString();
                inputValue.UOM = dgvInputValue["IUom", i].Value.ToString();
                inputValue.VATName = dgvInputValue["IVatName", i].Value.ToString();
                inputValue.EffectDate =Convert.ToDateTime(dgvInputValue["IEffectDate", i].Value.ToString());
                inputValue.ApprovedInputValue = Convert.ToDecimal(dgvInputValue["IApprovedValue", i].Value.ToString());
                inputValue.LatestInputValue = Convert.ToDecimal(dgvInputValue["ILatestValue", i].Value.ToString());
                inputValue.Increase = Convert.ToDecimal(dgvInputValue["Increase", i].Value.ToString());
                inputValue.CurrentDate = Convert.ToDateTime(dgvInputValue["IcurrentDate", i].Value.ToString());
                inputValue.Comments = dgvInputValue["IComments", i].Value.ToString();

                inputValues.Add(inputValue);
            }
        }

        private void GetSettingsValue()
        {
            try
            {
                #region Settings

                BOMDPlaceQ = 0;
                BOMDPlaceA = 0;
                string vBOMDPlaceQ, vBOMDPlaceA = string.Empty;
                CommonDAL commonDal = new CommonDAL();
                vBOMDPlaceQ = commonDal.settingsDesktop("BOM", "Quantity",null,connVM);
                vBOMDPlaceA = commonDal.settingsDesktop("BOM", "Amount",null,connVM);


                if (string.IsNullOrEmpty(vBOMDPlaceQ)
                    || string.IsNullOrEmpty(vBOMDPlaceA))
                {
                }

                BOMDPlaceQ = Convert.ToInt32(vBOMDPlaceQ);
                BOMDPlaceA = Convert.ToInt32(vBOMDPlaceA);

                #endregion Settings

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
                FileLogger.Log(this.Name, "GetSettingsValue", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "GetSettingsValue", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "GetSettingsValue", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "GetSettingsValue", exMessage);
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
                FileLogger.Log(this.Name, "GetSettingsValue", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "GetSettingsValue", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "GetSettingsValue", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "GetSettingsValue", exMessage);
            }

            #endregion Catch
        }

        private void ShowReport()
        {
             #region Try

            try
            {

                DBSQLConnection _dbsqlConnection = new DBSQLConnection();
                #region Store data 

                List<InputValueChange> inputValues = new List<InputValueChange>();
                
                for (int i = 0; i < dgvInputValue.Rows.Count; i++)
                {
                    InputValueChange inputValue = new InputValueChange();
                    inputValue.ProductCode = dgvInputValue["IProCode", i].Value.ToString();
                    inputValue.ProductName = dgvInputValue["IProName", i].Value.ToString();
                    inputValue.UOM = dgvInputValue["IUom", i].Value.ToString();
                    inputValue.VATName = dgvInputValue["IVatName", i].Value.ToString();
                    inputValue.EffectDate = Convert.ToDateTime(dgvInputValue["IEffectDate", i].Value.ToString());
                    inputValue.ApprovedInputValue = Convert.ToDecimal(dgvInputValue["IApprovedValue", i].Value.ToString());
                    inputValue.LatestInputValue = Convert.ToDecimal(dgvInputValue["ILatestValue", i].Value.ToString());
                    inputValue.Increase = Convert.ToDecimal(dgvInputValue["Increase", i].Value.ToString());
                    inputValue.CurrentDate = Convert.ToDateTime(dgvInputValue["IcurrentDate", i].Value.ToString());
                    inputValue.Comments = dgvInputValue["IComments", i].Value.ToString();

                    inputValues.Add(inputValue);
                }
                #endregion Store data
                #region Blank Data
                if (inputValues.Count <= 0)
                {
                    MessageBox.Show("There is no data to preview", this.Text, MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                    return;
                }
                #endregion Blank Data

                #region Report preview
                RptValueChanges objrpt=new RptValueChanges();


                objrpt.SetDataSource(inputValues);
                ////objrpt.DataDefinition.FormulaFields["HSCode"].Text = "'" + HSCode + "'";
                ////objrpt.DataDefinition.FormulaFields["ProductName"].Text = "'" + ProductName + "'";

                //// //objrpt.DataDefinition.FormulaFields["UserName"].Text = "'" + Program.CurrentUser + "'";
                // //objrpt.DataDefinition.FormulaFields["ReportName"].Text = "' Information'";
                objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + Program.CompanyName + "'";
                objrpt.DataDefinition.FormulaFields["CompanyLegalName"].Text = "'" + Program.CompanyLegalName + "'";
                objrpt.DataDefinition.FormulaFields["Address1"].Text = "'" + Program.Address1 + "'";
                objrpt.DataDefinition.FormulaFields["Address2"].Text = "'" + Program.Address2 + "'";
                //// objrpt.DataDefinition.FormulaFields["Address3"].Text = "'" + Program.Address3 + "'";
                //objrpt.DataDefinition.FormulaFields["TelephoneNo"].Text = "'" + Program.TelephoneNo + "'";
                //objrpt.DataDefinition.FormulaFields["FaxNo"].Text = "'" + Program.FaxNo + "'";
                //objrpt.DataDefinition.FormulaFields["VatRegistrationNo"].Text = "'" + Program.VatRegistrationNo + "'";
                //// //objrpt.DataDefinition.FormulaFields["CompanyName"].Text = "'" + Program.CompanyName + "'";


                FormulaFieldDefinitions crFormulaF;
                crFormulaF = objrpt.DataDefinition.FormulaFields;
                CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
                _vCommonFormMethod.FormulaField(objrpt, crFormulaF, "FontSize", Program.FontSize);


                FormReport reports = new FormReport();
                //objrpt.DataDefinition.FormulaFields["Trial"].Text = "'" + Program.Trial + "'";
                reports.crystalReportViewer1.Refresh();
                reports.setReportSource(objrpt);
                if (Convert.ToDecimal(LicenseExpite.ExpiteDate(DatabaseInfoVM.DatabaseName)) >= Convert.ToDecimal(_dbsqlConnection.ServerDateTime()))
                    //reports.ShowDialog();
                    reports.WindowState = FormWindowState.Maximized;
                reports.Show();
                // End Complete
                #endregion Report preview
            }
            #endregion
            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "ShowReport", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "ShowReport", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "ShowReport", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "ShowReport", exMessage);
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
                FileLogger.Log(this.Name, "ShowReport", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ShowReport", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ShowReport", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "ShowReport", exMessage);
            }
            #endregion
            finally
            {
                //this.btnPrev.Enabled = true;
                //this.progressBar1.Visible = false;
                //this.button1.Enabled = true;
            }
        }

        private void txtUomConv_Leave(object sender, EventArgs e)
        {
            Program.FormatTextBox(txtUomConv, "UomConv");
        }

        private void txtPBOMId_Leave(object sender, EventArgs e)
        {
            Program.FormatTextBox(txtPBOMId, "PBOMId");
        }

        private void txtFIssuePrice_Leave(object sender, EventArgs e)
        {
            Program.FormatTextBox(txtFIssuePrice, "FIssuePrice");
        }

        private void txtRUnitCost_Leave(object sender, EventArgs e)
        {
            Program.FormatTextBox(txtRUnitCost, "RUnitCost");
        }

        private void txtTotalQty_Leave(object sender, EventArgs e)
        {
            Program.FormatTextBox(txtTotalQty, "TotalQty");
        }

        private void txtRWastageInp_Leave(object sender, EventArgs e)
        {
            Program.FormatTextBox(txtRWastageInp, "RWastageInp");
        }

        private void txtRQuantity_Leave(object sender, EventArgs e)
        {
            Program.FormatTextBox(txtRQuantity, "RQuantity");
        }

        private void txtNetCost_Leave(object sender, EventArgs e)
        {
            Program.FormatTextBox(txtNetCost, "NetCost");
        }

        private void txtOHCost_Leave(object sender, EventArgs e)
        {
            Program.FormatTextBox(txtOHCost, "OHCost");
        }

        private void txtPercent1_Leave(object sender, EventArgs e)
        {
            Program.FormatTextBox(txtPercent1, "Percent1");
        }

        private void txtPercent2_Leave(object sender, EventArgs e)
        {
            Program.FormatTextBox(txtPercent2, "Percent2");
        }

        private void txtTrading_Leave(object sender, EventArgs e)
        {
            Program.FormatTextBox(txtTrading, "Trading");
        }

        private void txtRSD_Leave(object sender, EventArgs e)
        {
            Program.FormatTextBox(txtRSD, "RSD");
        }

        private void txtRVATRate_Leave(object sender, EventArgs e)
        {
            Program.FormatTextBox(txtRVATRate, "RVATRate");
        }

        private void txtTotalCost_Leave(object sender, EventArgs e)
        {
            Program.FormatTextBox(txtTotalCost, "TotalCost");
        }

        private void txtGTotal_Layout(object sender, LayoutEventArgs e)
        {
            Program.FormatTextBox(txtGTotal, "GTotal");
        }

        private void txtPacketPrice_Leave(object sender, EventArgs e)
        {
            Program.FormatTextBox(txtPacketPrice, "PacketPrice");
        }

        private void txtGrandTotal_Leave(object sender, EventArgs e)
        {
            Program.FormatTextBox(txtGrandTotal, "GrandTotal");
        }

        private void txtMargin_Leave(object sender, EventArgs e)
        {
            Program.FormatTextBox(txtMargin, "Margin");
        }

        private void txtOHGTotal_Leave(object sender, EventArgs e)
        {
            Program.FormatTextBox(txtOHGTotal, "OHGTotal");
        }
            
            
    }
}

