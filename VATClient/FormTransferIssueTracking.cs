using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VATClient.ModelDTO;
using VATViewModel.DTOs;
using VATServer.Library;
using System.Web.Services.Protocols;
using System.ServiceModel;
using System.Net;
using VATServer.Ordinary;

namespace VATClient
{
    public partial class FormTransferIssueTracking : Form
    {
        #region Vabiable

        List<ProductCmbDTO> productsCmb = new List<ProductCmbDTO>();

        private static List<TrackingCmbDTO> trackingsCmbDto = new List<TrackingCmbDTO>();
        private static List<TrackingVM> trackings = new List<TrackingVM>();
         TrackingDAL trackingDal = new TrackingDAL();
         DataTable TrackingRawsDt = new DataTable();
         private const string FieldDelimeter = DBConstant.FieldDelimeter;

         private static string ReceiveNo = string.Empty;
         private static string IssueNo = string.Empty;
         private static string SaleInvoiceNo = string.Empty;
         private static string TransferIssueno = string.Empty;

         private static string IsReceive = string.Empty;
         private static string IsIssue = string.Empty;
         private static string IsSale = string.Empty;
         private static string TransactionType = string.Empty;
         private decimal ItemQtyForFinish = 0;
         private decimal TotalItemUsedQty = 0;
         private static string TransType = string.Empty;

         private static string[] FinishItemsInfo = new string[0];
         private static List<FinishProductInfoDTO> FProductsInfo = new List<FinishProductInfoDTO>();
        

        #endregion
         public FormTransferIssueTracking()
         {
             InitializeComponent();
             connVM = Program.OrdinaryLoad();
         }
         private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
         public static List<TrackingVM> SelectOne(List<TrackingCmbDTO> trackCmb,string type)
         {
             List<TrackingVM> TrackingItems = new List<TrackingVM>();
             try
             {
                 trackingsCmbDto = trackCmb;
                 TransferIssueno = trackingsCmbDto[0].TransferIssueno;
                 //IssueNo = trackingsCmbDto[0].IssueNo;
                 //SaleInvoiceNo = trackingsCmbDto[0].SaleInvoiceNo;
                 TransType = type;
                 FormTransferIssueTracking frm = new FormTransferIssueTracking();
                 if (type == "62Out")
                 {
                     frm.rbtn62out.Checked = true;
                 }
                 //else if (type == "issue")
                 //{
                 //    frm.rbtnIssue.Checked = true;
                 //}
                 //else if (type == "Sale")
                 //{
                 //    frm.rbtnSale.Checked = true;
                 //}
                 FProductsInfo.Clear();
                 frm.ShowDialog();
                 TrackingItems = trackings;
                 trackingsCmbDto.Clear();
             }
             #region Catch
             catch (IndexOutOfRangeException ex)
             {
                 FileLogger.Log("FormReceiveSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
                 MessageBox.Show(ex.Message, "FormReceiveSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);

             }
             catch (NullReferenceException ex)
             {
                 FileLogger.Log("FormReceiveSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
                 MessageBox.Show(ex.Message, "FormReceiveSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);

             }
             catch (FormatException ex)
             {

                 FileLogger.Log("FormReceiveSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
                 MessageBox.Show(ex.Message, "FormReceiveSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);

             }

             catch (SoapHeaderException ex)
             {

                 string exMessage = ex.Message;
                 if (ex.InnerException != null)
                 {
                     exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                 ex.StackTrace;

                 }

                 FileLogger.Log("FormReceiveSearch", "SelectOne", exMessage);
                 MessageBox.Show(ex.Message, "FormReceiveSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);

             }
             catch (SoapException ex)
             {
                 string exMessage = ex.Message;
                 if (ex.InnerException != null)
                 {
                     exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                 ex.StackTrace;

                 }
                 MessageBox.Show(ex.Message, "FormReceiveSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                 FileLogger.Log("FormReceiveSearch", "SelectOne", exMessage);
             }
             catch (EndpointNotFoundException ex)
             {
                 MessageBox.Show(ex.Message, "FormReceiveSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                 FileLogger.Log("FormReceiveSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
             }
             catch (WebException ex)
             {
                 MessageBox.Show(ex.Message, "FormReceiveSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                 FileLogger.Log("FormReceiveSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
             }
             catch (Exception ex)
             {
                 string exMessage = ex.Message;
                 if (ex.InnerException != null)
                 {
                     exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                 ex.StackTrace;

                 }
                 MessageBox.Show(ex.Message, "FormReceiveSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                 FileLogger.Log("FormReceiveSearch", "SelectOne", exMessage);
             }
             #endregion Catch
             return TrackingItems;
         }


         private void FormTracking_Load(object sender, EventArgs e)
         {
             try
             {
                 #region Settings
                 string Heading1, Heading2 = string.Empty;
                 CommonDAL commonDal = new CommonDAL();
 
                 Heading1 = commonDal.settingsDesktop("TrackingTrace", "TrackingHead_1",null,connVM);
                 Heading2 = commonDal.settingsDesktop("TrackingTrace", "TrackingHead_2",null,connVM);

                 if (string.IsNullOrEmpty(Heading1) || string.IsNullOrEmpty(Heading2))
                 {
                     MessageBox.Show(MessageVM.msgSettingValueNotSave, this.Text);
                     return;
                 }

                 dgvTracking.Columns["Heading1"].HeaderText = Heading1.ToString();
                 dgvTracking.Columns["Heading2"].HeaderText = Heading2.ToString();

                 #endregion
                 cmbPName.Items.Clear();
                 cmbPCode.Items.Clear();

                 for (int i = 0; i < trackingsCmbDto.Count; i++)
                 {
                     cmbPCode.Items.Add(trackingsCmbDto[i].ProductCode.ToString()); //Finish item code
                     cmbPName.Items.Add(trackingsCmbDto[i].ProductName.ToString()); //Finish item name
                 }

                 if (rbtn62out.Checked )
                 {
                          //IsReceive = "N";
                     IsSale = "N";
                 }
                 else if (rbtn6out.Checked )
                 {
                     IsSale = "N";
                 }
             }
             #region Catch
             catch (IndexOutOfRangeException ex)
             {
                 FileLogger.Log("FormReceiveSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
                 MessageBox.Show(ex.Message, "FormReceiveSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);

             }
             catch (NullReferenceException ex)
             {
                 FileLogger.Log("FormReceiveSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
                 MessageBox.Show(ex.Message, "FormReceiveSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);

             }
             catch (FormatException ex)
             {

                 FileLogger.Log("FormReceiveSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
                 MessageBox.Show(ex.Message, "FormReceiveSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);

             }

             catch (SoapHeaderException ex)
             {

                 string exMessage = ex.Message;
                 if (ex.InnerException != null)
                 {
                     exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                 ex.StackTrace;

                 }

                 FileLogger.Log("FormReceiveSearch", "SelectOne", exMessage);
                 MessageBox.Show(ex.Message, "FormReceiveSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);

             }
             catch (SoapException ex)
             {
                 string exMessage = ex.Message;
                 if (ex.InnerException != null)
                 {
                     exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                 ex.StackTrace;

                 }
                 MessageBox.Show(ex.Message, "FormReceiveSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                 FileLogger.Log("FormReceiveSearch", "SelectOne", exMessage);
             }
             catch (EndpointNotFoundException ex)
             {
                 MessageBox.Show(ex.Message, "FormReceiveSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                 FileLogger.Log("FormReceiveSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
             }
             catch (WebException ex)
             {
                 MessageBox.Show(ex.Message, "FormReceiveSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                 FileLogger.Log("FormReceiveSearch", "SelectOne", ex.Message + Environment.NewLine + ex.StackTrace);
             }
             catch (Exception ex)
             {
                 string exMessage = ex.Message;
                 if (ex.InnerException != null)
                 {
                     exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                 ex.StackTrace;

                 }
                 MessageBox.Show(ex.Message, "FormReceiveSearch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                 FileLogger.Log("FormReceiveSearch", "SelectOne", exMessage);
             }
             #endregion Catch
             progressBar1.Visible = false;
         }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                //BugsBD
                string FinishItemNo = OrdinaryVATDesktop.SanitizeInput(txtFinishItemNo.Text);
                string DateText = OrdinaryVATDesktop.SanitizeInput(txtDate.Text);

                progressBar1.Visible = true;
                DataTable trackingResults = new DataTable();

                //TrackingRawsDt = trackingDal.FindTrackingItems(txtFinishItemNo.Text, "VAT 4.3", txtDate.Text, connVM);
                TrackingRawsDt = trackingDal.FindTrackingItems(FinishItemNo, "VAT 4.3", DateText, connVM);


                if (TrackingRawsDt.Rows.Count > 0)
                {

                    for (int i = 0; i < TrackingRawsDt.Rows.Count; i++)
                    {
                        string rawItemNo = TrackingRawsDt.Rows[i]["RawItemNo"].ToString();
                        ItemQtyForFinish = Convert.ToDecimal(TrackingRawsDt.Rows[i]["UseQuantity"].ToString());

                        if (rbtn62out.Checked)
                        {
                            //trackingResults = trackingDal.SearchReceiveTrackItems(rawItemNo, IsSale, SaleInvoiceNo, TransType, connVM);
                            trackingResults = trackingDal.SearchReceiveTrackItems(rawItemNo, IsSale, TransferIssueno, "Sale", connVM);
                        }

                        if (trackingResults.Rows.Count > 0)
                        {

                            for (int itemRow = 0; itemRow < dgvTracking.RowCount; itemRow++)
                            {
                                if (dgvTracking.Rows[i].Cells["ItemNo"].Value.ToString() == rawItemNo)
                                {
                                    MessageBox.Show("Same Product already exist.", this.Text);
                                    return;
                                }
                            }

                            int itemCount = 0;
                            for (int j = 0; j < trackingResults.Rows.Count; j++)
                            {

                                DataGridViewRow newRow = new DataGridViewRow();

                                dgvTracking.Rows.Add(newRow);

                                dgvTracking["LineNo", dgvTracking.RowCount - 1].Value = dgvTracking.Rows.Count;
                                dgvTracking["ItemNo", dgvTracking.RowCount - 1].Value = trackingResults.Rows[j]["ItemNo"].ToString();
                                dgvTracking["Heading1", dgvTracking.RowCount - 1].Value = trackingResults.Rows[j]["Heading1"].ToString();
                                dgvTracking["Heading2", dgvTracking.RowCount - 1].Value = trackingResults.Rows[j]["Heading2"].ToString();
                                dgvTracking["ProductName", dgvTracking.RowCount - 1].Value = trackingResults.Rows[j]["ProductName"].ToString();
                                dgvTracking["PCode", dgvTracking.RowCount - 1].Value = trackingResults.Rows[j]["ProductCode"].ToString();
                                dgvTracking["FinishItemNo", dgvTracking.RowCount - 1].Value = txtFinishItemNo.Text;
                                dgvTracking["Quantity", dgvTracking.RowCount - 1].Value = txtFinishQty.Text;
                                dgvTracking["Issue", dgvTracking.RowCount - 1].Value = trackingResults.Rows[j]["IsIssue"].ToString();
                                dgvTracking["TIssueNo", dgvTracking.RowCount - 1].Value = trackingResults.Rows[j]["IssueNo"].ToString();
                                dgvTracking["Receive", dgvTracking.RowCount - 1].Value = trackingResults.Rows[j]["IsReceive"].ToString();
                                dgvTracking["TReceiveNo", dgvTracking.RowCount - 1].Value = trackingResults.Rows[j]["ReceiveNo"].ToString();
                                dgvTracking["Sale", dgvTracking.RowCount - 1].Value = trackingResults.Rows[j]["IsSale"].ToString();
                                dgvTracking["TSaleInvoiceNo", dgvTracking.RowCount - 1].Value = trackingResults.Rows[j]["SaleInvoiceNo"].ToString();
                                if (TransType=="Receive_Return" || TransType=="Sale_Return")
                                {
                                    dgvTracking["ReturnReceive", dgvTracking.RowCount - 1].Value = trackingResults.Rows[j]["ReturnReceive"].ToString();
                                    dgvTracking["ReturnSale", dgvTracking.RowCount - 1].Value = trackingResults.Rows[j]["ReturnSale"].ToString();
                                    
                                }
                                itemCount++;
                            }
                            CalculateQuantity(itemCount, rawItemNo);
                        }
                        else
                        {
                            MessageBox.Show("No Data found.", "TrackingInfo");
                        }
                    }
                }
                ShowSelectedRows();

                #region Old
                
               

                //for (int i = 0; i < TrackingRawsDt.Rows.Count; i++)
                //        {
                //            string rawItemNo = TrackingRawsDt.Rows[i]["RawItemNo"].ToString();
                //            ItemQtyForFinish = Convert.ToDecimal(TrackingRawsDt.Rows[i]["UseQuantity"].ToString());

                //            trackingResults = trackingDal.SearchTrackingItems(rawItemNo, IsIssue, IsReceive, IsSale, "",ReceiveNo, "",SaleInvoiceNo);
                //            //trackingResults = trackingDal.SearchTrackingItems(rawItemNo, "N", IsReceive, "N", "", "", "", "");

                //            if (trackingResults.Rows.Count > 0)
                //            {
                //                for (int j = 0; j < trackingResults.Rows.Count; j++)
                //                {
                //                    DataGridViewRow newRow = new DataGridViewRow();

                //                    dgvTracking.Rows.Add(newRow);

                //                    dgvTracking["LineNo", dgvTracking.RowCount - 1].Value = j + 1;
                //                    dgvTracking["ItemNo", dgvTracking.RowCount - 1].Value = trackingResults.Rows[j]["ItemNo"].ToString();
                //                    dgvTracking["Heading1", dgvTracking.RowCount - 1].Value = trackingResults.Rows[j]["Heading1"].ToString();
                //                    dgvTracking["Heading2", dgvTracking.RowCount - 1].Value = trackingResults.Rows[j]["Heading2"].ToString();
                //                    dgvTracking["ProductName", dgvTracking.RowCount - 1].Value = trackingResults.Rows[j]["ProductName"].ToString();
                //                    dgvTracking["PCode", dgvTracking.RowCount - 1].Value = trackingResults.Rows[j]["ProductCode"].ToString();
                //                    dgvTracking["FinishItemNo", dgvTracking.RowCount - 1].Value = txtFinishItemNo.Text;
                //                    dgvTracking["Issue", dgvTracking.RowCount - 1].Value = trackingResults.Rows[j]["IsIssue"].ToString();
                //                    dgvTracking["TIssueNo", dgvTracking.RowCount - 1].Value = trackingResults.Rows[j]["IssueNo"].ToString();
                //                    dgvTracking["Receive", dgvTracking.RowCount - 1].Value = trackingResults.Rows[j]["IsReceive"].ToString();
                //                    dgvTracking["TReceiveNo", dgvTracking.RowCount - 1].Value = trackingResults.Rows[j]["ReceiveNo"].ToString();
                //                    dgvTracking["Sale", dgvTracking.RowCount - 1].Value = trackingResults.Rows[j]["IsSale"].ToString();
                //                    dgvTracking["TSaleInvoiceNo", dgvTracking.RowCount - 1].Value = trackingResults.Rows[j]["SaleInvoiceNo"].ToString();

                //                }
                //                decimal rowCunt = trackingResults.Rows.Count;
                //                CalculateQuantity(rowCunt);
                //            }
                //            else
                //            {
                //                MessageBox.Show("No Data found.", "TrackingInfo");
                //            }
                //        }
                    
                //}
                //else
                //{
                //    string fitemNo = txtFinishItemNo.Text;
                //    trackingResults = trackingDal.SearchTrackingItems("", IsIssue, IsReceive, IsSale, fitemNo,ReceiveNo,"","");

                //    if (trackingResults.Rows.Count > 0)
                //    {
                //        for (int j = 0; j < trackingResults.Rows.Count; j++)
                //        {
                //            DataGridViewRow newRow = new DataGridViewRow();

                //            dgvTracking.Rows.Add(newRow);

                //            dgvTracking["LineNo", dgvTracking.RowCount - 1].Value = j + 1;
                //            dgvTracking["ItemNo", dgvTracking.RowCount - 1].Value = trackingResults.Rows[j]["ItemNo"].ToString();
                //            dgvTracking["Heading1", dgvTracking.RowCount - 1].Value = trackingResults.Rows[j]["Heading1"].ToString();
                //            dgvTracking["Heading2", dgvTracking.RowCount - 1].Value = trackingResults.Rows[j]["Heading2"].ToString();
                //            dgvTracking["ProductName", dgvTracking.RowCount - 1].Value = trackingResults.Rows[j]["ProductName"].ToString();
                //            dgvTracking["PCode", dgvTracking.RowCount - 1].Value = trackingResults.Rows[j]["ProductCode"].ToString();
                //            //dgvTracking["LineNo", dgvTracking.RowCount - 1].Value = trackingResults.Rows[j]["TrackLineNo"].ToString();
                //            dgvTracking["FinishItemNo", dgvTracking.RowCount - 1].Value = txtFinishItemNo.Text;
                //            dgvTracking["Issue", dgvTracking.RowCount - 1].Value = trackingResults.Rows[j]["IsIssue"].ToString();
                //            dgvTracking["TIssueNo", dgvTracking.RowCount - 1].Value = trackingResults.Rows[j]["IssueNo"].ToString();
                //            dgvTracking["Receive", dgvTracking.RowCount - 1].Value = trackingResults.Rows[j]["IsReceive"].ToString();
                //            dgvTracking["TReceiveNo", dgvTracking.RowCount - 1].Value = trackingResults.Rows[j]["ReceiveNo"].ToString();
                //            dgvTracking["Sale", dgvTracking.RowCount - 1].Value = trackingResults.Rows[j]["IsSale"].ToString();
                //            dgvTracking["TSaleInvoiceNo", dgvTracking.RowCount - 1].Value = trackingResults.Rows[j]["SaleInvoiceNo"].ToString();

                //        }
                //        //decimal rowCunt = trackingResults.Rows.Count;
                //        //CalculateQuantity(rowCunt);
                //    }
                //}
                #endregion
                

            }
            #region Catch

            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (Exception ex)
            {
                string exMessage = ex.Message + Environment.NewLine +
                                ex.StackTrace;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ReceiveImportFromText", exMessage);
            }
            finally
            {
                progressBar1.Visible = false;
            }
            #endregion
        }

        private void CalculateQuantity(decimal rawQty, string rawItemNo)
        {
            decimal finishQty,availableRawQty = 0;
            
            finishQty = Convert.ToDecimal(txtFinishQty.Text);

            TotalItemUsedQty = finishQty * ItemQtyForFinish;
            availableRawQty = rawQty;

            FinishProductInfoDTO fProductInfo = new FinishProductInfoDTO();

            fProductInfo.FinishQty = finishQty;
            fProductInfo.RawItemNo = rawItemNo;
            fProductInfo.TotalUsedRawQty = TotalItemUsedQty;
            fProductInfo.FinishItemNo = cmbPName.Text;
            FProductsInfo.Add(fProductInfo);
            
            if (Convert.ToInt32(TotalItemUsedQty) > availableRawQty)
            {
                MessageBox.Show("Stock not availabe.", "Tracking Info");
                
            }

        }

        private void ShowSelectedRows()
        {
            try
            {
                DataTable TIssuetrackingResults = new DataTable();
                TransferIssueDAL transferIssueDal = new TransferIssueDAL();

                if (rbtn62out.Checked)
                {
                    if (dgvTracking.Rows.Count > 0)
                    {
                        for (int i = 0; i < dgvTracking.Rows.Count; i++)
                        {
                            string isSale = dgvTracking.Rows[i].Cells["Sale"].Value.ToString();
                            string itemNo = dgvTracking.Rows[i].Cells["ItemNo"].Value.ToString();

                            string Heading1 = dgvTracking.Rows[i].Cells["Heading1"].Value.ToString();
                            string Heading2 = dgvTracking.Rows[i].Cells["Heading2"].Value.ToString();

                            if (!string.IsNullOrEmpty(TransferIssueno))
                            {
                                TIssuetrackingResults = transferIssueDal.SearchTransferIssueTrackItems(itemNo, TransferIssueno, connVM);

                            }
                           
                            
                            if (TIssuetrackingResults != null && TIssuetrackingResults.Rows.Count > 0)
                            {


                                string vHeading1 = "";
                                string vHeading2 = "";
                               

                                foreach (DataRow row in TIssuetrackingResults.Rows)
                                {
                                    vHeading1 = row["Heading1"].ToString();
                                    vHeading2 = row["Heading2"].ToString();
                                    if (Heading1 == vHeading1 && Heading2 == vHeading2)
                                    {
                                        dgvTracking["Select", i].Value = true;
                                    }
                                    
                                }


                                

                            }

                        }
                    }
                }
                //else if (rbtnIssue.Checked)
                //{
                //    if (dgvTracking.Rows.Count > 0)
                //    {
                //        for (int i = 0; i < dgvTracking.Rows.Count; i++)
                //        {
                //            string isIssue = dgvTracking.Rows[i].Cells["Issue"].Value.ToString();
                //            string issueNo = dgvTracking.Rows[i].Cells["TIssueNo"].Value.ToString();
                //            if (isIssue == "Y" && issueNo ==IssueNo)
                //            {
                //                dgvTracking["Select", i].Value = true;
                //            }
                //        }
                //    }
                //}
              
               
            }
            #region Catch

            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (Exception ex)
            {
                string exMessage = ex.Message + Environment.NewLine +
                                ex.StackTrace;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ReceiveImportFromText", exMessage);
            }

            #endregion
        }

        private void cmbPCode_Leave(object sender, EventArgs e)
        {
            try
            {
               string searchText = cmbPCode.Text.Trim().ToString();
                if (!string.IsNullOrEmpty(searchText))
                {
                    var p = from prod in trackingsCmbDto.ToList()
                            where prod.ProductCode.ToLower() == searchText.ToLower()
                            select prod;

                    if (p != null && p.Any())
                    {
                        var products = p.First();
                        txtDate.Text = products.EffectiveDate;
                        txtFinishItemNo.Text = products.ItemNo;
                        txtFinishQty.Text = products.Qty;
                        txtVatName.Text = products.VatName;
                        cmbPName.Text = products.ProductName;
                    }


                }
            }
            #region Catch

            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (Exception ex)
            {
                string exMessage = ex.Message + Environment.NewLine +
                                ex.StackTrace;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ReceiveImportFromText", exMessage);
            }

            #endregion
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            GridSeleted();
        }

        private void GridSeleted()
        {
            try
            {
                trackings.Clear();
                
                for (int i = 0; i < dgvTracking.Rows.Count; i++)
                {
                   
                        var tracking = new TrackingVM();
                        tracking.ItemNo = dgvTracking["ItemNo", i].Value.ToString();
                        tracking.Heading1 = dgvTracking["Heading1", i].Value.ToString();
                        tracking.Heading2 = dgvTracking["Heading2", i].Value.ToString();

                        //if (rbtnIssue.Checked)
                        //{
                        //    if (Convert.ToBoolean(dgvTracking["Select", i].Value) == true)
                        //    {
                        //        tracking.IsIssue = "Y";
                                
                        //    }
                        //    else
                        //    {
                        //        tracking.IsIssue = "N";
                        //    }
                        //    tracking.IsSale = dgvTracking["Sale", i].Value.ToString();
                        //    tracking.IsReceive = dgvTracking["Receive", i].Value.ToString();
                        //    tracking.FinishItemNo = dgvTracking["FinishItemNo", i].Value.ToString();
                        //    tracking.Quantity = Convert.ToDecimal(dgvTracking["Quantity", i].Value.ToString());

                        //}

                        if (rbtn62out.Checked)
                        {
                            if (Convert.ToBoolean(dgvTracking["Select", i].Value) == true)
                            {
                                tracking.IsSale = "Y";
                            }
                            else
                            {
                                tracking.IsSale = "N";
                            }
                            tracking.IsIssue = dgvTracking["Issue", i].Value.ToString();
                            tracking.IsReceive = dgvTracking["Receive", i].Value.ToString();
                            tracking.FinishItemNo = dgvTracking["FinishItemNo", i].Value.ToString();
                            tracking.Quantity = Convert.ToDecimal(dgvTracking["Quantity", i].Value.ToString());
                            tracking.transactionType = "Sale";
                        }
                        //else if (rbtnSale.Checked)
                        //{
                        //    if (Convert.ToBoolean(dgvTracking["Select", i].Value) == true)
                        //    {
                        //        tracking.IsSale = "Y";
                        //    }
                        //    else
                        //    {
                        //        tracking.IsSale = "N";
                        //    }
                        //    tracking.IsIssue = dgvTracking["Issue", i].Value.ToString();
                        //    tracking.IsReceive = dgvTracking["Receive", i].Value.ToString();
                        //    tracking.FinishItemNo = dgvTracking["FinishItemNo", i].Value.ToString();
                        //    tracking.Quantity = Convert.ToDecimal(dgvTracking["Quantity", i].Value.ToString());
                        //    tracking.transactionType = "Sale";
                        //}

                        tracking.IssueNo = dgvTracking["TIssueNo", i].Value.ToString();
                        tracking.ReceiveNo = dgvTracking["TReceiveNo", i].Value.ToString();
                        tracking.SaleInvoiceNo = dgvTracking["TSaleInvoiceNo", i].Value.ToString();

                        //if (TransType == "Receive_Return")
                        //{

                        //    if (Convert.ToBoolean(dgvTracking["Select", i].Value) == true)
                        //    {
                        //        tracking.ReturnType = TransType;
                        //        tracking.ReturnReceive = "Y";
                        //        tracking.transactionType = "Receive_Return";
                        //    }
                        //    else
                        //    {
                        //        tracking.ReturnReceive = "N";
                        //        tracking.ReturnReceiveID = "";
                        //        tracking.ReturnType = "";
                        //        tracking.transactionType = "Receive_Return";
                        //    }
                        //}
                        //else if (TransType == "Sale_Return")
                        //{

                        //    if (Convert.ToBoolean(dgvTracking["Select", i].Value) == true)
                        //    {
                        //        tracking.ReturnType = TransType;
                        //        tracking.ReturnSale = "Y";
                        //        tracking.transactionType = "Sale_Return";
                        //    }
                        //    else
                        //    {
                        //        tracking.ReturnSale = "N";
                        //        tracking.ReturnSaleID = "";
                        //        tracking.ReturnType = "";
                        //        tracking.transactionType = "Sale_Return";

                        //    }
                        //}
                
                        trackings.Add(tracking);
                        
                }

                #region Calculate Raw

                for (int i = 0; i < FProductsInfo.Count; i++)
                {
                    int selectedRow = 0;
                    foreach (DataGridViewRow item in dgvTracking.Rows)
                    {
                        if (FProductsInfo[i].RawItemNo==item.Cells["itemNo"].Value.ToString())
                        {
                            if (Convert.ToBoolean(item.Cells["Select"].Value) == true)
                            {
                                selectedRow++;
                            }
                        }
                    }

                    if (Convert.ToInt32(FProductsInfo[i].TotalUsedRawQty) > selectedRow || Convert.ToInt32(FProductsInfo[i].TotalUsedRawQty) < selectedRow)
                    {
                        MessageBox.Show("Please select " + Convert.ToInt32(FProductsInfo[i].TotalUsedRawQty) + " items for Finish Product ("+FProductsInfo[i].FinishItemNo+")",
                            "Tracking Info");
                        trackings.Clear();
                        return;
                    }
                }

                #endregion
                this.Close();
            }
            #region Catch
            catch (IndexOutOfRangeException ex)
            {
                FileLogger.Log(this.Name, "GridSeleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (NullReferenceException ex)
            {
                FileLogger.Log(this.Name, "GridSeleted", ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (FormatException ex)
            {

                FileLogger.Log(this.Name, "GridSeleted", ex.Message + Environment.NewLine + ex.StackTrace);
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

                FileLogger.Log(this.Name, "GridSeleted", exMessage);
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
                FileLogger.Log(this.Name, "GridSeleted", exMessage);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "GridSeleted", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "GridSeleted", ex.Message + Environment.NewLine + ex.StackTrace);
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
                FileLogger.Log(this.Name, "GridSeleted", exMessage);
            }
            #endregion Catch
        }

        private void dgvTracking_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                dgvTracking["Select", e.RowIndex].Value = !Convert.ToBoolean(dgvTracking["Select", e.RowIndex].Value);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            dgvTracking.Rows.Clear();
            cmbPCode.SelectedIndex = -1;
            cmbPName.SelectedIndex = -1;
            txtFinishItemNo.Text = "";
            txtFinishQty.Text = "";
            txtDate.Text = "";
            txtVatName.Text = "";
        }

        private void cmbPName_Leave(object sender, EventArgs e)
        {
            try
            {
                string searchText = cmbPName.Text.Trim().ToString();
                if (!string.IsNullOrEmpty(searchText))
                {
                    var products = from prod in trackingsCmbDto.ToList()
                            where prod.ProductName.ToLower() == searchText.ToLower()
                            select prod;

                    if (products != null && products.Any())
                    {
                        var product = products.First();
                        txtDate.Text = product.EffectiveDate;
                        txtFinishItemNo.Text = product.ItemNo;
                        txtFinishQty.Text = product.Qty;
                        txtVatName.Text = product.VatName;
                        cmbPCode.Text = product.ProductCode;
                    }


                }
            }
            #region Catch

            catch (ArgumentNullException ex)
            {
                string err = ex.Message.ToString();
                string[] error = err.Split(FieldDelimeter.ToCharArray());
                FileLogger.Log(this.Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(error[1], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch (Exception ex)
            {
                string exMessage = ex.Message + Environment.NewLine +
                                ex.StackTrace;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;

                }
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log(this.Name, "ReceiveImportFromText", exMessage);
            }

            #endregion
        }

        private void rBtnCodeT_CheckedChanged(object sender, EventArgs e)
        {
            if (rBtnCodeT.Checked)
            {
                cmbPName.Enabled = false;
                cmbPCode.Enabled = true;

            }
            else if (rBtnNameT.Checked)
            {
                cmbPName.Enabled = true;
                cmbPCode.Enabled = false;

            }
        }

        private void cmbPName_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

    }
}
