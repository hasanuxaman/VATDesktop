using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Web.Services.Protocols;
using System.ServiceModel;
using System.Net;
using VATServer.Library;
using SymphonySofttech.Reports.List;
using SymphonySofttech.Reports.Report;
using VATViewModel.DTOs;
using VATServer.License;
using CrystalDecisions.CrystalReports.Engine;

namespace VATClient.ReportPreview
{
    public partial class FormRptForm5 : Form
    {
        public FormRptForm5()
        {
            InitializeComponent();       
            connVM = Program.OrdinaryLoad();
			 
			 
        }
			 private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        private DataSet ReportResult;
        private List<BanderolForm_5> form5s = new List<BanderolForm_5>();
        private decimal TCloseQty = 0;
        

        private void FormBanderol5_Load(object sender, EventArgs e)
        {
            dtpDate.Text = DateTime.Now.ToString("MMMM-yyyy");
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            if (Program.CheckLicence(dtpDate.Value) == true)
            {
                MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                return;
            }

            ReportShowDsLoad();
        }
        private void ReportShowDsLoad()
        {
            try
            {
                this.progressBar1.Visible = true;
                this.btnLoad.Enabled = false;
                this.btnPreview.Enabled = false;

                backgroundWorkerLoad.RunWorkerAsync();


            }
            #region Catch
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "ReportShowDsLoad", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "ReportShowDsLoad", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "ReportShowDsLoad", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "ReportShowDsLoad", exMessage);
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
                FileLogger.Log(this.Name, "ReportShowDsLoad", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ReportShowDsLoad", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ReportShowDsLoad", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "ReportShowDsLoad", exMessage);
            }
            #endregion Catch
        }

        private void backgroundWorkerLoad_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                #region Statement

                ReportResult = new DataSet();

                ReportDSDAL reportDsdal = new ReportDSDAL();

                ReportResult = reportDsdal.BanderolForm_5(dtpDate.Value.ToString("MMMM-yyyy"),connVM);

                #endregion
            }
            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerPreview_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerPreview_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerPreview_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "backgroundWorkerPreview_DoWork", exMessage);
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
                FileLogger.Log(this.Name, "backgroundWorkerPreview_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerPreview_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerPreview_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "backgroundWorkerPreview_DoWork", exMessage);
            }
            #endregion
        }

        private void backgroundWorkerLoad_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                #region Statement

                // Start Complete

                if (ReportResult.Tables.Count <= 0)
                {
                    MessageBox.Show("There is no data to preview", this.Text, MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                    return;
                }

                 #region
                decimal vColSerialNo = 0;
                string vFiscalYear = string.Empty;
                string vMonthName = string.Empty;
                string vDemandNo_Date = string.Empty;
                string vProductName = string.Empty;
                string vPackNature = string.Empty;
                string vBanderolInfo = string.Empty;
                decimal vDemandQty = 0;
                string vRefNo_Date = string.Empty;
                decimal vReceiveQty = 0;
                decimal vTotalRecQty = 0;
                decimal vUsedQty = 0;
                decimal vWastageQty = 0;
                decimal vClosingQty = 0;
                decimal vSaleQty = 0;
                string vRemarks = string.Empty;
                string vTransType = string.Empty;

                string vBanderolID = string.Empty;
                string vPackagingId = string.Empty;

                decimal vClosingQuantity = 0;
                
                
                
               
                
                decimal OpQty = 0;

                decimal OpeningQty = 0;
                decimal DemandQty = 0;
                decimal ReceiveQty = 0;
                decimal CFYRQty = 0;
                decimal UsedQty = 0;
                decimal WastageQty = 0;
                decimal ClosingQuantity = 0;
                decimal SaleQty = 0;
                


                string vTempFiscalYear = string.Empty;
                string vTempMonthName = string.Empty;
                string vTempDemandNo_Date = string.Empty;
                string vTempProductName = string.Empty;
                string vTempPackNature = string.Empty;
                string vTempBanderolInfo = string.Empty;
                decimal vTempDemandQty = 0;
                string vTempRefNo_Date = string.Empty;
                decimal vTempReceiveQty = 0;

                decimal vTempTotalRecQty = 0;
                decimal vTempUsedQty = 0;
                decimal vTempWastageQty = 0;
                decimal vTempSaleQty = 0;
                string vTempRemarks = string.Empty;
                string vTempTransType = string.Empty;


                #endregion

                if (ReportResult.Tables[1].Rows.Count > 0)
                {
                    chkMLock.Checked = true ? ReportResult.Tables[1].Rows[0][0].ToString() == "Y" :
                            ReportResult.Tables[1].Rows[0][0].ToString() == "N";
                   vMonthName = ReportResult.Tables[1].Rows[0][1].ToString() + " to " + ReportResult.Tables[1].Rows[0][2].ToString();
                   vFiscalYear = ReportResult.Tables[1].Rows[0][3].ToString();
           
                }

                List<string> finishItemList = new List<string>();
                foreach (DataRow item in ReportResult.Tables[2].Rows)
                {
                    finishItemList.Add(item["ItemNo"].ToString());

                }

                form5s = new List<BanderolForm_5>();
                BanderolForm_5 form5 = new BanderolForm_5();

                int i = 1;

                for (int fItem = 0; fItem < finishItemList.Count; fItem++)
                {
                    DataRow[] foundRows = ReportResult.Tables[0].Select("ItemNo='" + finishItemList[fItem] + "'");
                    DataTable dtsorted = foundRows.CopyToDataTable();
                    
                    OpeningQty = 0;
                    DemandQty = 0;
                    ReceiveQty = 0;
                    CFYRQty = 0;
                    UsedQty = 0;
                    WastageQty = 0;
                    ClosingQuantity = 0;
                    SaleQty = 0;
                    vDemandNo_Date = "";
                    vRefNo_Date = "";
                    foreach (DataRow item in dtsorted.Rows) 
                    {
                        #region Get from Datatable

                         
                        vTempDemandNo_Date = item["DemandNo"].ToString(); 
                        vTempProductName = item["ProductName"].ToString();
                        vTempPackNature = item["Pack"].ToString();
                        vBanderolInfo = item["Banderol"].ToString();
                        vTempRefNo_Date = item["RefDate"].ToString();
                        vTempDemandQty = Convert.ToDecimal(item["DemQuantity"].ToString());
                        vTempReceiveQty = Convert.ToDecimal(item["RecQuantity"].ToString());
                        vTempUsedQty = Convert.ToDecimal(item["UsedQty"].ToString());
                        vTempWastageQty = Convert.ToDecimal(item["WastageQty"].ToString());
                        vTempSaleQty = Convert.ToDecimal(item["SaleQty"].ToString());
                        vTempRemarks = item["remarks"].ToString();
                        vTempTransType = item["TransType"].ToString().Trim();


                        #endregion Get from Datatable


                        if (vTempDemandNo_Date != "-")
                        {
                            if (vDemandNo_Date=="")
                            {
                                vDemandNo_Date = vTempDemandNo_Date; 
                            }
                            else
                            {
                                vDemandNo_Date = vDemandNo_Date + " , " + vTempDemandNo_Date; 
                            }
                            
                        }
                        
                        vProductName = vTempProductName;
                        if (vTempRefNo_Date != "-")
                        {
                            if (vRefNo_Date == "")
                            {
                                vRefNo_Date = vTempRefNo_Date;
                            }
                            else
                            {
                                vRefNo_Date = vRefNo_Date + " , " + vTempRefNo_Date;
                            }
                            
                        }
                        vPackNature = vTempPackNature;

                        vRemarks = vTempRemarks;
                        vTransType = vTempTransType;

                        if (vRemarks == "Opening")
                        {
                            OpeningQty = vTempDemandQty;
                            DemandQty = 0;
                            ReceiveQty = 0;
                            UsedQty = 0;
                            WastageQty = 0;
                            SaleQty = 0;
                            CFYRQty = CFYRQty + OpeningQty;
                        }
                        else
                        {
                            if (vRemarks == "Receive")
                            {
                                DemandQty = DemandQty + vTempDemandQty;
                                ReceiveQty = ReceiveQty + vTempReceiveQty;
                                CFYRQty = CFYRQty + vTempReceiveQty; 
                            }

                            else if (vRemarks == "Sale")
                            {
                                UsedQty = UsedQty + vTempUsedQty;
                                WastageQty = WastageQty + vTempWastageQty;
                                SaleQty = SaleQty + vTempSaleQty;
                            }

                            
                            
                        }


                        //TCloseQty = (Convert.ToDecimal(ReceiveQty) - Convert.ToDecimal(UsedQty) - Convert.ToDecimal(WastageQty));
                        TCloseQty = (Convert.ToDecimal(CFYRQty) - Convert.ToDecimal(UsedQty) - Convert.ToDecimal(WastageQty));
                        
                    }

                    #region AssignValue to List
                    form5 = new BanderolForm_5();
                    form5.SerialNo = i;
                    form5.FiscalYear = vFiscalYear; 
                    form5.MonthName = vMonthName; 
                    form5.DemandNo_Date = vDemandNo_Date;
                    form5.ProductName = vProductName; 
                    form5.PackNature = vPackNature;  
                    form5.BanderolInfo = vBanderolInfo; 
                    form5.DemandQty = DemandQty;    
                    form5.RefNo_Date = vRefNo_Date;   
                    form5.ReceiveQty = ReceiveQty;  
                    form5.TotalRecQty = CFYRQty; 
                    form5.UsedQty = UsedQty; 
                    form5.WastageQty = WastageQty;  
                    form5.ClosingQty = TCloseQty; 
                    form5.SaleQty = SaleQty; 
                    form5.Remarks = vRemarks; 
                    form5.TransType = vTransType; 

                    form5s.Add(form5);
                    i = i + 1;

                    #endregion AssignValue to List


                }

                #region Old
                //DataRow[] DetailRawsOpening = ReportResult.Tables[0].Select("Remarks='Opening'");
                //#region Opening
                //foreach (DataRow row in DetailRawsOpening)
                //{
                //    vTempRemarks = row["remarks"].ToString().Trim();
                //    vTempTransType = row["TransType"].ToString().Trim();

                //    //vTempFiscalYear = row["FiscalYear"].ToString().Trim();
                //    //vTempColTranDate = Convert.ToDateTime(row["TranDate"].ToString().Trim());
                //    //vTempTotalRecQty = Convert.ToDecimal(row["CFyRecQty"].ToString().Trim());

                //    OpQty = Convert.ToDecimal(row["DemQuantity"].ToString().Trim());

                //    form5 = new BanderolForm_5();

                //    #region if row 1 Opening

                //    OpeningQty = OpQty;
                //    OpQty = 0;

                //    ReceiveQty = 0;
                //    UsedQty = 0;
                //    WastageQty = 0;
                //    SaleQty = 0;

                //    CFYRQty = vTempTotalRecQty+OpeningQty;
                //    TCloseQty =
                //        (Convert.ToDecimal(OpeningQty) + Convert.ToDecimal(ReceiveQty) -
                //     Convert.ToDecimal(UsedQty) - Convert.ToDecimal(WastageQty));

                //    vFiscalYear = vTempFiscalYear;
                //    vDemandNo_Date = "-";
                //    vProductName = "-";
                //    vPackNature = "-";
                    
                //    vDemandQty = OpeningQty;
                //    vRefNo_Date = "";
                //    vReceiveQty = ReceiveQty;
                //    vTotalRecQty = CFYRQty;
                //    vUsedQty = UsedQty;
                //    vWastageQty = WastageQty;
                //    vClosingQty = TCloseQty;
                //    vSaleQty = SaleQty;
                //    vRemarks = vTempRemarks;
                //    vTransType = vTempTransType;


                //    vClosingQuantity = (Convert.ToDecimal(OpeningQty) + Convert.ToDecimal(ReceiveQty) -
                //                        Convert.ToDecimal(UsedQty) - Convert.ToDecimal(WastageQty));

                //    #endregion

                //    #region AssignValue to List

                //    //form5.SerialNo = i; //    i.ToString();      // Serial No   
                //    //form5.FiscalYear = vFiscalYear; //    item["StartDateTime"].ToString();      // Date
                //    //form5.MonthName = vMonthName; //    item["StartingQuantity"].ToString();      // Opening Quantity
                //    //form5.DemandNo_Date = vDemandNo_Date; //    item["StartingAmount"].ToString();      // Opening Price
                //    //form5.ProductName = vProductName; //    item["Quantity"].ToString();      // Production Quantity
                //    //form5.PackNature = vPackNature; //    item["UnitCost"].ToString();      // Production Price
                //    //form5.BanderolInfo = vBanderolInfo; //    item["CustomerName"].ToString();      // Customer Name
                //    //form5.DemandQty = vDemandQty;   //    item["VATRegistrationNo"].ToString();      // Customer VAT Reg No
                //    //form5.RefNo_Date = vRefNo_Date;   //    item["Address1"].ToString();      // Customer Address
                //    //form5.ReceiveQty = vReceiveQty; //    item["TransID"].ToString();      // Sale Invoice No
                //    //form5.TotalRecQty = vTotalRecQty; //    item["StartDateTime"].ToString();      // Sale Invoice Date and Time
                //    //form5.UsedQty = vUsedQty; //    item["StartDateTime"].ToString();      // Sale Invoice Date and Time
                //    //form5.WastageQty = vWastageQty; //    item["ProductName"].ToString();      // Sale Product Name
                //    //form5.ClosingQty = vClosingQty; //    item["Quantity"].ToString();      // Sale Product Quantity
                //    //form5.SaleQty = vSaleQty; //    item["UnitCost"].ToString();      // Sale Product Sale Price(NBR Price with out VAT and SD amount)
                //    //form5.Remarks = vRemarks; //    item["SD"].ToString();      // SD Amount
                //    //form5.TransType = vRemarks; //    item["VATRate"].ToString();      // VAT Amount

                //    //form5s.Add(form5);
                //    //i = i + 1;

                //    #endregion AssignValue to List

                //}
                //#endregion Opening

                //DataRow[] DetailRawsOthers = ReportResult.Tables[0].Select("Remarks<>'Opening'");
                //if (DetailRawsOthers == null || !DetailRawsOthers.Any())
                //{
                //    MessageBox.Show("There is no data to preview", this.Text);
                //    return;
                //}
                //string strSort = "SerialNo ASC";

                //DataView dtview = new DataView(DetailRawsOthers.CopyToDataTable());
                //dtview.Sort = strSort;
                //DataTable dtsorted = dtview.ToTable();
                ////DataTable dtsorted = ReportResult.Tables[0].DefaultView.ToTable(true, "ItemNo");

                //#region Sum  Qty
                //                #endregion
                //foreach (DataRow item in dtsorted.Rows)
                //{
                //    #region Get from Datatable


                //    OpeningQty = 0;
                //    ReceiveQty = 0;
                //    UsedQty = 0;
                //    WastageQty = 0;
                //    SaleQty = 0;
                //    TCloseQty = 0;


                //    vColSerialNo = i;
                //    vTempDemandNo_Date = item["DemandNo"].ToString(); // Opening Price
                //    vTempProductName = item["ProductName"].ToString(); // Production Quantity
                //    vPackNature = item["Pack"].ToString(); // Production Price
                //    vTempDemandQty = Convert.ToDecimal(item["DemQuantity"].ToString()); // Customer VAT Reg No
                //    vTempRefNo_Date = item["RefDate"].ToString(); // Customer Address
                //    vTempReceiveQty = Convert.ToDecimal(item["RecQuantity"].ToString()); // Sale Invoice No
                //    vTempUsedQty = Convert.ToDecimal(item["UsedQty"].ToString()); // Sale Invoice Date and Time
                //    vTempWastageQty = Convert.ToDecimal(item["WastageQty"].ToString()); // Sale Product Name
                //    vTempSaleQty = Convert.ToDecimal(item["SaleQty"].ToString()); // SD Amount
                //    vTempRemarks = item["remarks"].ToString(); // Remarks
                //    vTempTransType = item["TransType"].ToString().Trim();


                //    #endregion Get from Datatable

                //    if (vTempRemarks.Trim() == "Receive")
                //    {
                //        form5 = new BanderolForm_5();

                //        #region if row 1 Opening


                //        OpeningQty = OpQty + vClosingQuantity;
                //        OpQty = 0;
                //        ReceiveQty = vTempReceiveQty;
                //        UsedQty = vTempUsedQty;
                //        WastageQty = vTempWastageQty;
                //        SaleQty = vTempSaleQty;
                //        CFYRQty = CFYRQty + ReceiveQty;
                //        TCloseQty =
                //            (Convert.ToDecimal(OpeningQty) + Convert.ToDecimal(ReceiveQty) -
                //             Convert.ToDecimal(UsedQty) - Convert.ToDecimal(WastageQty));

                //        vFiscalYear = vTempFiscalYear;
                //        vDemandNo_Date = vTempDemandNo_Date;
                //        vProductName = vTempProductName;
                //        vPackNature = vTempPackNature;
                //        //vColBanderolInfo = " ";
                //        vDemandQty = Convert.ToDecimal(Program.FormatingNumeric(vTempDemandQty.ToString(), 2));
                //        vRefNo_Date = vTempRefNo_Date;
                //        vReceiveQty = Convert.ToDecimal(Program.FormatingNumeric(vTempReceiveQty.ToString(), 2));
                //        vTotalRecQty = Convert.ToDecimal(Program.FormatingNumeric(CFYRQty.ToString(), 2));
                //        vUsedQty = Convert.ToDecimal(Program.FormatingNumeric(vTempUsedQty.ToString(), 2));
                //        vWastageQty = Convert.ToDecimal(Program.FormatingNumeric(vTempWastageQty.ToString(), 2));
                //        vClosingQty = Convert.ToDecimal(Program.FormatingNumeric(TCloseQty.ToString(), 2));
                //        vSaleQty = Convert.ToDecimal(Program.FormatingNumeric(vTempSaleQty.ToString(), 2));
                //        vRemarks = vTempRemarks;
                //        vTransType = vTempTransType;

                //        vClosingQuantity = (Convert.ToDecimal(OpeningQty) + Convert.ToDecimal(ReceiveQty) -
                //                            Convert.ToDecimal(UsedQty) - Convert.ToDecimal(WastageQty));

                //        #endregion

                //        #region AssignValue to List

                //        form5.SerialNo = i;
                //        form5.FiscalYear = vFiscalYear;
                //        form5.MonthName = vMonthName;
                //        form5.DemandNo_Date = vDemandNo_Date;
                //        form5.ProductName = vProductName;
                //        form5.PackNature = vPackNature;
                //        form5.BanderolInfo = vBanderolInfo;
                //        form5.DemandQty = vDemandQty;
                //        form5.RefNo_Date = vRefNo_Date;
                //        form5.ReceiveQty = vReceiveQty;
                //        form5.TotalRecQty = vTotalRecQty;
                //        form5.UsedQty = vUsedQty;
                //        form5.WastageQty = vWastageQty;
                //        form5.ClosingQty = vClosingQty;
                //        form5.SaleQty = vSaleQty;
                //        form5.Remarks = vRemarks;
                //        form5.TransType = vTransType;

                //        form5s.Add(form5);
                //        i = i + 1;

                //        #endregion AssignValue to List
                //    }
                //    else if (vTempRemarks == "Sale")
                //    {
                //        form5 = new BanderolForm_5();
                //        #region if row 1 Opening

                //        OpeningQty = OpQty + vClosingQuantity;
                //        OpQty = 0;
                //        ReceiveQty = 0;
                //        UsedQty = vTempUsedQty;
                //        WastageQty = vTempWastageQty;
                //        SaleQty = vTempSaleQty;

                //        TCloseQty =
                //            (Convert.ToDecimal(OpeningQty) + Convert.ToDecimal(ReceiveQty) -
                //             Convert.ToDecimal(UsedQty) - Convert.ToDecimal(WastageQty));

                //        vFiscalYear = vTempFiscalYear;
                //        //vColTranDate = vTempColTranDate;
                //        vDemandNo_Date = vTempDemandNo_Date;
                //        vProductName = vTempProductName;
                //        vPackNature = vTempPackNature;
                //        //vColBanderolInfo = " ";
                //        vDemandQty = Convert.ToDecimal(Program.FormatingNumeric(vTempDemandQty.ToString(), 2));
                //        vRefNo_Date = vTempRefNo_Date;
                //        vReceiveQty = Convert.ToDecimal(Program.FormatingNumeric(vTempReceiveQty.ToString(), 2));
                //        //vColCFYRecQty = Convert.ToDecimal(Program.FormatingNumeric(CFYRQty.ToString(), 2));
                //        vTotalRecQty = 0;
                //        vUsedQty = Convert.ToDecimal(Program.FormatingNumeric(vTempUsedQty.ToString(), 2));
                //        vWastageQty = Convert.ToDecimal(Program.FormatingNumeric(vTempWastageQty.ToString(), 2));
                //        vClosingQty = Convert.ToDecimal(Program.FormatingNumeric(TCloseQty.ToString(), 2));
                //        vSaleQty = Convert.ToDecimal(Program.FormatingNumeric(vTempSaleQty.ToString(), 2));
                //        vRemarks = vTempRemarks;
                //        vTransType = vTempTransType;

                //        vClosingQuantity = (Convert.ToDecimal(OpeningQty) + Convert.ToDecimal(ReceiveQty) -
                //                            Convert.ToDecimal(UsedQty) - Convert.ToDecimal(WastageQty));

                //        #endregion

                //        #region AssignValue to List

                //        form5.SerialNo = i;
                //        form5.FiscalYear = vFiscalYear;
                //        form5.MonthName = vMonthName;
                //        form5.DemandNo_Date = vDemandNo_Date;
                //        form5.ProductName = vProductName;
                //        form5.PackNature = vPackNature;
                //        form5.BanderolInfo = vBanderolInfo;
                //        form5.DemandQty = vDemandQty;
                //        form5.RefNo_Date = vRefNo_Date;
                //        form5.ReceiveQty = vReceiveQty;
                //        form5.TotalRecQty = vTotalRecQty;
                //        form5.UsedQty = vUsedQty;
                //        form5.WastageQty = vWastageQty;
                //        form5.ClosingQty = vClosingQty;
                //        form5.SaleQty = vSaleQty;
                //        form5.Remarks = vRemarks;
                //        form5.TransType = vTransType;


                //        form5s.Add(form5);
                //        i = i + 1;

                //        #endregion AssignValue to List
                //    }
                //}

                #endregion Old
                #region Statement

                // Start Complete

                dgvBande5.Rows.Clear();


                for (int y = 0; y < form5s.Count; y++)
                {
                    DataGridViewRow NewRow = new DataGridViewRow();
                    dgvBande5.Rows.Add(NewRow);
                    dgvBande5.Rows[y].Cells["LineNo"].Value = form5s[y].SerialNo.ToString();
                    dgvBande5.Rows[y].Cells["FiscalYear"].Value = form5s[y].FiscalYear.ToString();
                    dgvBande5.Rows[y].Cells["MonthName"].Value = form5s[y].MonthName.ToString();
                    dgvBande5.Rows[y].Cells["DemandNo"].Value = form5s[y].DemandNo_Date.ToString();
                    dgvBande5.Rows[y].Cells["ProductName"].Value = form5s[y].ProductName.ToString();
                    dgvBande5.Rows[y].Cells["Pack"].Value = form5s[y].PackNature.ToString();
                    dgvBande5.Rows[y].Cells["BanderolInfo"].Value = form5s[y].BanderolInfo.ToString();
                    dgvBande5.Rows[y].Cells["DemandQty"].Value = form5s[y].DemandQty.ToString();
                    dgvBande5.Rows[y].Cells["RefOrderNo"].Value = form5s[y].RefNo_Date.ToString();
                    dgvBande5.Rows[y].Cells["ReceiveQty"].Value = form5s[y].ReceiveQty.ToString();
                    dgvBande5.Rows[y].Cells["MTD"].Value = form5s[y].TotalRecQty.ToString();
                    dgvBande5.Rows[y].Cells["UsedQty"].Value = form5s[y].UsedQty.ToString();
                    dgvBande5.Rows[y].Cells["WastageQty"].Value = form5s[y].WastageQty.ToString();
                    dgvBande5.Rows[y].Cells["ClosingQty"].Value = form5s[y].ClosingQty.ToString();
                    dgvBande5.Rows[y].Cells["SaleQty"].Value = form5s[y].SaleQty.ToString();
                    dgvBande5.Rows[y].Cells["Remarks"].Value = form5s[y].Remarks.ToString();


                }
               

                #endregion
                
                #endregion

            }

            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerPreview_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerPreview_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerPreview_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "backgroundWorkerPreview_RunWorkerCompleted", exMessage);
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
                FileLogger.Log(this.Name, "backgroundWorkerPreview_RunWorkerCompleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerPreview_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerPreview_RunWorkerCompleted", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "backgroundWorkerPreview_RunWorkerCompleted", exMessage);
            }
            #endregion
            finally
            {
                this.btnPreview.Enabled = true;
                this.progressBar1.Visible = false;
                this.btnLoad.Enabled = true;
            }
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            try
            {
                Program.FontSize = cmbFontSize.Text.Trim() == "" ? "7" : cmbFontSize.Text.Trim();
                if (Program.CheckLicence(dtpDate.Value) == true)
                {
                    MessageBox.Show("Fiscal year not create or your system date not ok, Transaction not complete");
                    return;
                }
                if (chkMLock.Checked == false)
                {
                    MessageBox.Show("Fiscal month not lock, please lock the month first. ", this.Text,
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                ReportShowDsNew();

            }
            #region Catch
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
            #endregion Catch
        }
        private void ReportShowDsNew()
        {
            try
            {

                this.progressBar1.Visible = true;
                this.btnPreview.Enabled = false;
                backgroundWorkerPreview.RunWorkerAsync();
            }
            #region Catch
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "ReportShowDsNew", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "ReportShowDsNew", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "ReportShowDsNew", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "ReportShowDsNew", exMessage);
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
                FileLogger.Log(this.Name, "ReportShowDsNew", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ReportShowDsNew", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ReportShowDsNew", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "ReportShowDsNew", exMessage);
            }
            #endregion Catch
        }

        private void backgroundWorkerPreview_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                #region Statement

                ReportResult = new DataSet();

                ReportDSDAL reportDsdal = new ReportDSDAL();

                ReportResult = reportDsdal.BanderolForm_5(dtpDate.Value.ToString("MMMM-yyyy"),connVM);

                #endregion
            }
            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerPreview_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerPreview_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerPreview_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "backgroundWorkerPreview_DoWork", exMessage);
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
                FileLogger.Log(this.Name, "backgroundWorkerPreview_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerPreview_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerPreview_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "backgroundWorkerPreview_DoWork", exMessage);
            }
            #endregion
        }

        private void backgroundWorkerPreview_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            
            try
            {
                #region Statement
                DBSQLConnection _dbsqlConnection = new DBSQLConnection();

                if (ReportResult.Tables.Count <= 0)
                {
                    MessageBox.Show("There is no data to preview", this.Text, MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                    return;
                }

                RptBanderolForm5 objrpt = new RptBanderolForm5();

                //if (PreviewOnly == true)
                //{
                //    objrpt.DataDefinition.FormulaFields["Preview"].Text = "'Preview Only'";
                //}

                objrpt.SetDataSource(form5s);

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

                #endregion
            }
            #region catch

            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerPreview_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "backgroundWorkerPreview_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "backgroundWorkerPreview_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "backgroundWorkerPreview_DoWork", exMessage);
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
                FileLogger.Log(this.Name, "backgroundWorkerPreview_DoWork", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerPreview_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "backgroundWorkerPreview_DoWork", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "backgroundWorkerPreview_DoWork", exMessage);
            }
            #endregion

            finally
            {
                this.progressBar1.Visible = false;
                this.btnPreview.Enabled = true;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
