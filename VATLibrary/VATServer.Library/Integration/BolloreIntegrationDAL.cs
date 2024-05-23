using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using VATViewModel.DTOs;
using VATViewModel.Integration.JsonModels;

namespace VATServer.Library.Integration
{
    public class BolloreIntegrationDAL
    {
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        #region Sales
        
        public DataTable GetSaleData(string xml, SqlConnection vConnection = null, SqlTransaction vTransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region variable
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            #endregion variable

            try
            {
                CommonDAL commonDal = new CommonDAL();

                ////string code = commonDal.settingValue("CompanyCode", "Code");


                string[] result = new[] { "Fail" };

                DataTable dt = new DataTable();

                dt = GetSalesXMLData(xml);

                if (dt == null || dt.Rows.Count == 0)
                {
                    return dt;
                }

                #region Open Connection and Transaction

                if (vConnection != null)
                {
                    currConn = vConnection;
                }

                if (vTransaction != null)
                {
                    transaction = vTransaction;
                }

                if (vConnection == null)
                {
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    currConn.Open();

                }

                if (vTransaction == null)
                {
                    transaction = currConn.BeginTransaction();
                }
                #endregion

                #region tempTable

                string tempTable = @"
create table #saletemp(
sl int identity(1,1), 
ID varchar(6000),
Branch_Code varchar(100),
Customer_Code varchar(100),
Customer_Name varchar(100),
Invoice_Date_Time varchar(100),
Reference_No varchar(100),
Item_Code varchar(100),
Item_Name varchar(200),
Quantity decimal(25,7),
UOM varchar(100),
NBR_Price decimal(25,7),
SubTotal decimal(25,7),
VAT_Rate decimal(25,7),
VAT_Amount decimal(25,7),
Vehicle_No varchar(100),
VehicleType varchar(100),
CustomerGroup varchar(100),
Delivery_Address varchar(500),
Comments varchar(2000),
Sale_Type varchar(100),
TransactionType varchar(100),
Discount_Amount decimal(25,7),
Promotional_Quantity decimal(25,7),
SD_Rate decimal(25,7),
SDAmount decimal(25,7),
Is_Print varchar(100),
Tender_Id varchar(100),
Post varchar(100),
LC_Number varchar(100),
Currency_Code varchar(100),
CommentsD varchar(100),
Non_Stock varchar(100),
Trading_MarkUp varchar(100),
[Type] varchar(100),
VAT_Name varchar(100),
Previous_Invoice_No varchar(100),
PreviousInvoiceDateTime varchar(100),
PreviousNBRPrice decimal(25,7),
PreviousQuantity decimal(25,7),
PreviousUOM varchar(100),
PreviousSubTotal decimal(25,7),
PreviousVATAmount decimal(25,7),
PreviousVATRate decimal(25,7),
PreviousSD decimal(25,7),
PreviousSDAmount decimal(25,7),
ReasonOfReturn varchar(400),
CustomerAddress varchar(500),
Option3 varchar(100),
Option4 varchar(100),
Option5 varchar(100),
Option6 varchar(100),
LocalSystemTime varchar(100),

)";

                #endregion

                #region getAllData

                string getAllData = @"

select 
 ID
,Branch_Code
,Customer_Code
,Customer_Name
,Invoice_Date_Time
,Reference_No
,Item_Code
,Item_Name
,UOM
,Quantity
,NBR_Price
,SubTotal
,VAT_Rate
,VAT_Amount
,Vehicle_No
,VehicleType
,CustomerGroup
,Delivery_Address
,Comments
,Sale_Type
,TransactionType
,Discount_Amount
,Promotional_Quantity
,SD_Rate
,SDAmount
,Is_Print
,Tender_Id
,Post
,LC_Number
,Currency_Code
,CommentsD
,Non_Stock
,Trading_MarkUp
,[Type]
,VAT_Name
,Previous_Invoice_No
,PreviousInvoiceDateTime
,PreviousNBRPrice
,PreviousQuantity
,PreviousUOM
,PreviousSubTotal
,PreviousVATAmount
,PreviousVATRate
,PreviousSD
,PreviousSDAmount
,ReasonOfReturn
,CustomerAddress
,Option3
,Option4
,Option5
,Option6
,LocalSystemTime
from #saletemp
where 1=1 

order by ID,TransactionType

";

                getAllData += @" drop table #saletemp";

                #endregion

                SqlCommand cmd = new SqlCommand(tempTable, currConn, transaction);
                cmd.ExecuteNonQuery();

                result = commonDal.BulkInsert("#saletemp", dt, currConn, transaction);

                cmd.CommandText = getAllData;
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                dt = new DataTable();
                adapter.Fill(dt);

                return dt;

            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (currConn != null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }

                }
            }
        }
        
        public DataTable GetSalesXMLData(string xml)
        {
            string rXML = xml.Replace(@"<?xml version=""1.0"" encoding=""UTF-8""?>", "");

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(rXML);

            DataTable dt = new DataTable();

            try
            {

                //string json = JsonConvert.SerializeObject(xmlDocument);

                //DataRootBollore ApiData = JsonConvert.DeserializeObject<DataRootBollore>(json);

                XmlNodeList invoices = xmlDocument.GetElementsByTagName("UniversalInterchange");
                XmlNodeList invoicesLines = xmlDocument.GetElementsByTagName("PostingJournal");

                #region Create DataTable

                dt.Clear();
                dt.Columns.Add("ID");
                ////dt.Columns.Add("InvoiceNo");
                dt.Columns.Add("Branch_Code");
                dt.Columns.Add("Customer_Code");
                dt.Columns.Add("Customer_Name");
                dt.Columns.Add("Invoice_Date_Time");
                dt.Columns.Add("Reference_No");
                dt.Columns.Add("Item_Code");
                dt.Columns.Add("Item_Name");
                dt.Columns.Add("Quantity");
                dt.Columns.Add("UOM");
                dt.Columns.Add("NBR_Price");
                dt.Columns.Add("SubTotal");
                dt.Columns.Add("VAT_Rate");
                dt.Columns.Add("VAT_Amount");
                dt.Columns.Add("Vehicle_No");
                dt.Columns.Add("VehicleType");
                dt.Columns.Add("CustomerGroup");
                dt.Columns.Add("Delivery_Address");
                dt.Columns.Add("Comments");
                dt.Columns.Add("Sale_Type");
                dt.Columns.Add("TransactionType");
                dt.Columns.Add("Discount_Amount");
                dt.Columns.Add("Promotional_Quantity");

                dt.Columns.Add("SD_Rate");
                dt.Columns.Add("SDAmount");
                dt.Columns.Add("Is_Print");
                dt.Columns.Add("Tender_Id");
                dt.Columns.Add("Post");
                dt.Columns.Add("LC_Number");
                dt.Columns.Add("Currency_Code");
                dt.Columns.Add("CommentsD");
                dt.Columns.Add("Non_Stock");
                dt.Columns.Add("Trading_MarkUp");
                dt.Columns.Add("Type");
                dt.Columns.Add("VAT_Name");

                dt.Columns.Add("Previous_Invoice_No");
                dt.Columns.Add("PreviousInvoiceDateTime");
                dt.Columns.Add("PreviousNBRPrice");
                dt.Columns.Add("PreviousQuantity");
                dt.Columns.Add("PreviousUOM");
                dt.Columns.Add("PreviousSubTotal");
                dt.Columns.Add("PreviousVATAmount");
                dt.Columns.Add("PreviousVATRate");
                dt.Columns.Add("PreviousSD");
                dt.Columns.Add("PreviousSDAmount");
                dt.Columns.Add("ReasonOfReturn");
                dt.Columns.Add("CustomerAddress");
                dt.Columns.Add("Option5");
                dt.Columns.Add("Option3");
                dt.Columns.Add("Option4");
                dt.Columns.Add("Option6");
                dt.Columns.Add("LocalSystemTime");

                ////dt.Columns.Add("TransactionType");
                ////dt.Columns.Add("CompanyCode");

                #endregion

                #region Sales

                string CurrentTime = DateTime.Now.ToString("HH:mm:ss");

                foreach (XmlNode invoice in invoices)
                {
                    XmlNode Body = invoice.SelectSingleNode("Body");
                    XmlNode UniversalTransaction = Body.SelectSingleNode("UniversalTransaction");
                    XmlNode TransactionInfo = UniversalTransaction.SelectSingleNode("TransactionInfo");
                    XmlNode Branch = TransactionInfo.SelectSingleNode("Branch");
                    XmlNode RelatedOrgs = TransactionInfo.SelectSingleNode("RelatedOrgs");
                    XmlNode RelatedOrg = RelatedOrgs.SelectSingleNode("RelatedOrg");

                    string ID = TransactionInfo["TransactionNumber"].InnerText;
                    string Branch_Code = Branch["Code"].InnerText;
                    string Invoice_Date_Time = Convert.ToDateTime(TransactionInfo["PostDate"].InnerText).ToString("yyyy-MM-dd " + CurrentTime);
                    string DeliveryDateTime = Invoice_Date_Time;
                    string Customer_Code = TransactionInfo["Organization"].InnerText;
                    string Customer_Name = TransactionInfo["Organization"].InnerText;

                    ////string OriginalTransactionNumber = TransactionInfo["OriginalReferenceOriginalTransactionNumber"].InnerText;
                    ////string PreviousInvoiceDateTime = TransactionInfo["OriginalReferenceOriginalTransactionDate"].InnerText;
                    ////string ReasonOfReturn = TransactionInfo["Description"].InnerText;

                    string TransactionType = TransactionInfo["TransactionType"].InnerText;

                    XmlNode OriginalTransactionNumber = TransactionInfo.SelectSingleNode("OriginalReferenceOriginalTransactionNumber");
                    XmlNode OriginalTransactionDate = TransactionInfo.SelectSingleNode("OriginalReferenceOriginalTransactionDate");
                    XmlNode Description = TransactionInfo.SelectSingleNode("Description");

                    string Previous_Invoice_No = "";
                    string PreviousInvoiceDateTime = "";
                    string ReasonOfReturn = "";

                    if (OriginalTransactionNumber != null)
                    {
                        Previous_Invoice_No = OriginalTransactionNumber.InnerText;
                    }
                    if (OriginalTransactionDate != null)
                    {
                        PreviousInvoiceDateTime = Convert.ToDateTime(OriginalTransactionDate.InnerText).ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    if (Description != null)
                    {
                        ReasonOfReturn = Description.InnerText;
                    }

                    if (string.IsNullOrEmpty(Customer_Name))
                    {
                        Customer_Name = "";
                    }
                    string Comments = "-";

                    string Reference_No = RelatedOrg["JOBID"].InnerText;

                    string Address1 = RelatedOrg["Address1"].InnerText;
                    string Address2 = "";

                    XmlNode Address2Node = RelatedOrg.SelectSingleNode("Address2");
                    if (Address2Node != null)
                    {
                        Address2 = Address2Node.InnerText;
                    }

                    ////string Address2 = RelatedOrg["Address2"].InnerText;

                    ////string CustomerAddress = Address1 + "," + Address2;
                    
                    string CustomerAddress = Address1;

                    if (!string.IsNullOrWhiteSpace(Address2))
                    {
                        CustomerAddress += "," + Address2;
                    }

                    string Delivery_Address = CustomerAddress;

                    string CreatorEMail = TransactionInfo["CreatorEMail"].InnerText;
                    string EditorEMail = TransactionInfo["EditorEMail"].InnerText;

                    XmlNode DebtorCode = TransactionInfo.SelectSingleNode("ExternalDebtorCode");
                    string ExternalDebtorCode = "";

                    if (DebtorCode != null)
                    {
                        ExternalDebtorCode = DebtorCode.InnerText;
                    }

                    XmlNode OSTotal = TransactionInfo.SelectSingleNode("OSTotal");
                    string OSTotalAmount = "";
                    if (OSTotal != null)
                    {
                        OSTotalAmount = OSTotal.InnerText;
                    }

                    XmlNode BatchGenTimeInfo = invoice.SelectSingleNode("BatchGenTimeInfo");
                    string LocalSystemTime = BatchGenTimeInfo["LocalSystemTime"].InnerText;

                    foreach (XmlNode invoicesLine in invoicesLines)
                    {
                        XmlNode ARAccountGroup = invoicesLine.SelectSingleNode("ARAccountGroup");

                        DataRow dr = dt.NewRow();

                        dr["ID"] = ID;
                        dr["Branch_Code"] = Branch_Code;
                        dr["Customer_Code"] = Customer_Code;
                        dr["Customer_Name"] = Customer_Name;
                        dr["Invoice_Date_Time"] = Invoice_Date_Time;
                        dr["Reference_No"] = Reference_No;
                        dr["CustomerAddress"] = CustomerAddress;
                        dr["Delivery_Address"] = Delivery_Address;
                        dr["Comments"] = Comments;
                        dr["Item_Code"] = invoicesLine["ChargeCode"].InnerText;
                        dr["Item_Name"] = invoicesLine["ChargeCodeLocalDescription"].InnerText;
                        dr["UOM"] = invoicesLine["QuantityUnit"].InnerText;
                        decimal Quantity = Convert.ToDecimal(invoicesLine["Quantity"].InnerText);
                        decimal NBR_Price = Convert.ToDecimal(invoicesLine["Rate"].InnerText);
                        ////decimal NBR_Price = Convert.ToDecimal(invoicesLine["LocalTotalAmount"].InnerText);
                        dr["Quantity"] = Quantity;
                        dr["NBR_Price"] = NBR_Price;

                        dr["SD_Rate"] = "0";
                        dr["SDAmount"] = 0;

                        decimal VATRate = Convert.ToDecimal(invoicesLine["VATTaxIDTaxRate"].InnerText);

                        dr["VAT_Rate"] = VATRate;

                        if (VATRate == 15)
                        {
                            dr["Type"] = "VAT";
                        }
                        else if (VATRate == 0)
                        {
                            dr["Type"] = "NoNVAT";
                        }
                        else
                        {
                            dr["Type"] = "OtherRate";
                        }
                        dr["SubTotal"] = Quantity * NBR_Price;

                        ////////dr["VAT_Amount"] = Convert.ToDecimal(TransactionInfo["OSGSTVATAmount"].InnerText);
                        dr["VAT_Amount"] = Convert.ToDecimal(invoicesLine["ChargeTotalVATAmount"].InnerText);
                        string CurrencyCode = invoicesLine["ChargeCurrency"].InnerText;
                        dr["Currency_Code"] = CurrencyCode;
                        //dr["VAT_Amount"] = Convert.ToDecimal(TransactionInfo["LocalVATAmount"].InnerText);
                        //dr["Currency_Code"] = invoicesLine["LocalCurrency"].InnerText;

                        if (TransactionType.ToLower() == "crd")//CRD
                        {
                            dr["TransactionType"] = "Credit";
                            dr["Sale_Type"] = "Credit";

                            dr["Previous_Invoice_No"] = Previous_Invoice_No;
                            dr["PreviousInvoiceDateTime"] = PreviousInvoiceDateTime;
                            dr["ReasonOfReturn"] = ReasonOfReturn;
                        }
                        else
                        {
                            dr["Sale_Type"] = "New";
                            dr["Previous_Invoice_No"] = "0";
                            dr["ReasonOfReturn"] = "NA";

                            if (CurrencyCode.ToLower() == "bdt")
                            {
                                dr["TransactionType"] = "ServiceNS";
                            }
                            else
                            {
                                dr["TransactionType"] = "ExportServiceNS";
                            }
                        }                        

                        dr["Discount_Amount"] = 0;
                        dr["Promotional_Quantity"] = "0";
                        dr["Vehicle_No"] = "NA";
                        dr["VehicleType"] = "NA";
                        dr["Is_Print"] = "N";
                        dr["Tender_Id"] = "0";
                        dr["Post"] = "Y";
                        dr["LC_Number"] = "NA";
                        dr["CommentsD"] = "NA";
                        dr["Non_Stock"] = "N";
                        dr["Trading_MarkUp"] = "0";
                        dr["VAT_Name"] = "VAT 4.3";
                        dr["PreviousNBRPrice"] = "0";
                        dr["PreviousQuantity"] = "0";
                        dr["PreviousSubTotal"] = "0";
                        dr["PreviousVATAmount"] = "0";
                        dr["PreviousVATRate"] = "0";
                        dr["PreviousSD"] = "0";
                        dr["PreviousSDAmount"] = "0";

                        dr["Option5"] = ExternalDebtorCode;
                        dr["Option3"] = CreatorEMail;
                        dr["Option4"] = EditorEMail;
                        dr["Option6"] = OSTotalAmount;
                        dr["LocalSystemTime"] = LocalSystemTime;

                        dt.Rows.Add(dr);
                    }

                }
                #endregion

            }

            catch (ArgumentException e)
            {
                //FileLogger.Log("DHL form", "GetIBS", "Price can not be zero" + srcFileName);
                throw e;
            }
            catch (Exception e)
            {
                throw e;
            }

            return dt;
        }

        public DataTable GetXMLData(string xml)
        {
            string rXML = xml.Replace(@"<?xml version=""1.0"" encoding=""UTF-8""?>", "");

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(rXML);

            DataTable dt = new DataTable();

            try
            {

                string json = JsonConvert.SerializeObject(xmlDocument);

                DataRootBollore ApiData = JsonConvert.DeserializeObject<DataRootBollore>(json);

                #region Create DataTable

                dt.Clear();
                dt.Columns.Add("ID");
                ////dt.Columns.Add("InvoiceNo");
                dt.Columns.Add("Branch_Code");
                dt.Columns.Add("Customer_Code");
                dt.Columns.Add("Customer_Name");
                dt.Columns.Add("Invoice_Date_Time");
                dt.Columns.Add("Reference_No");
                dt.Columns.Add("Item_Code");
                dt.Columns.Add("Item_Name");
                dt.Columns.Add("Quantity");
                dt.Columns.Add("UOM");
                dt.Columns.Add("NBR_Price");
                dt.Columns.Add("SubTotal");
                dt.Columns.Add("VAT_Rate");
                dt.Columns.Add("VAT_Amount");
                dt.Columns.Add("Vehicle_No");
                dt.Columns.Add("VehicleType");
                dt.Columns.Add("CustomerGroup");
                dt.Columns.Add("Delivery_Address");
                dt.Columns.Add("Comments");
                dt.Columns.Add("Sale_Type");
                dt.Columns.Add("TransactionType");
                dt.Columns.Add("Discount_Amount");
                dt.Columns.Add("Promotional_Quantity");

                dt.Columns.Add("SD_Rate");
                dt.Columns.Add("SDAmount");
                dt.Columns.Add("Is_Print");
                dt.Columns.Add("Tender_Id");
                dt.Columns.Add("Post");
                dt.Columns.Add("LC_Number");
                dt.Columns.Add("Currency_Code");
                dt.Columns.Add("CommentsD");
                dt.Columns.Add("Non_Stock");
                dt.Columns.Add("Trading_MarkUp");
                dt.Columns.Add("Type");
                dt.Columns.Add("VAT_Name");

                dt.Columns.Add("Previous_Invoice_No");
                dt.Columns.Add("PreviousInvoiceDateTime");
                dt.Columns.Add("PreviousNBRPrice");
                dt.Columns.Add("PreviousQuantity");
                dt.Columns.Add("PreviousUOM");
                dt.Columns.Add("PreviousSubTotal");
                dt.Columns.Add("PreviousVATAmount");
                dt.Columns.Add("PreviousVATRate");
                dt.Columns.Add("PreviousSD");
                dt.Columns.Add("PreviousSDAmount");
                dt.Columns.Add("ReasonOfReturn");
                dt.Columns.Add("CustomerAddress");
                ////dt.Columns.Add("TransactionType");
                ////dt.Columns.Add("CompanyCode");

                #endregion

                #region Sales

                string CurrentTime = DateTime.Now.ToString("HH:mm:ss");

                foreach (var items in ApiData.UniversalInterchangeBatch.UniversalInterchange)
                {
                    string ID = items.Body.UniversalTransaction.TransactionInfo.TransactionNumber;
                    string Branch_Code = items.Body.UniversalTransaction.TransactionInfo.Branch.Code;
                    string Invoice_Date_Time = Convert.ToDateTime(items.Body.UniversalTransaction.TransactionInfo.PostDate).ToString("yyyy-MM-dd " + CurrentTime);
                    string DeliveryDateTime = Invoice_Date_Time;
                    string Customer_Code = items.Body.UniversalTransaction.TransactionInfo.Organization;
                    string Customer_Name = items.Body.UniversalTransaction.TransactionInfo.Organization;

                    if (string.IsNullOrEmpty(Customer_Name))
                    {
                        Customer_Name = "";
                    }
                    string Comments = "-";
                    string Reference_No = items.Body.UniversalTransaction.TransactionInfo.RelatedOrgs.RelatedOrg.JOBID;

                    string Address1 = items.Body.UniversalTransaction.TransactionInfo.RelatedOrgs.RelatedOrg.Address1;
                    string Address2 = items.Body.UniversalTransaction.TransactionInfo.RelatedOrgs.RelatedOrg.Address2;

                    string CustomerAddress = Address1 + "," + Address2;
                    string Delivery_Address = CustomerAddress;

                    DataRow dr = dt.NewRow();

                    dr["ID"] = ID;
                    dr["Branch_Code"] = Branch_Code;
                    dr["Customer_Code"] = Customer_Code;
                    dr["Customer_Name"] = Customer_Name;
                    dr["Invoice_Date_Time"] = Invoice_Date_Time;
                    dr["Reference_No"] = Reference_No;
                    dr["CustomerAddress"] = CustomerAddress;
                    dr["Delivery_Address"] = Delivery_Address;
                    dr["Comments"] = Comments;
                    //dr["Item_Code"] = items.Body.UniversalTransaction.TransactionInfo.PostingJournalCollection.PostingJournal.ChargeCode;
                    //dr["Item_Name"] = items.Body.UniversalTransaction.TransactionInfo.PostingJournalCollection.PostingJournal.ChargeCodeLocalDescription;
                    //dr["UOM"] = items.Body.UniversalTransaction.TransactionInfo.PostingJournalCollection.PostingJournal.QuantityUnit;

                    //decimal Quantity = Convert.ToDecimal(items.Body.UniversalTransaction.TransactionInfo.PostingJournalCollection.PostingJournal.Quantity);
                    //decimal SubTotal = Convert.ToDecimal(items.Body.UniversalTransaction.TransactionInfo.OSTotalWithNoTax);
                    //decimal NBR_Price = Convert.ToDecimal(items.Body.UniversalTransaction.TransactionInfo.PostingJournalCollection.PostingJournal.Rate);

                    //dr["Quantity"] = Quantity;
                    //dr["NBR_Price"] = NBR_Price;
                    //dr["SubTotal"] = SubTotal;

                    dr["SD_Rate"] = "0";
                    dr["SDAmount"] = 0;

                    //////////string VATType = items.Body.UniversalTransaction.TransactionInfo.PostingJournalCollection.PostingJournal.VATTaxIDTaxCode;

                    //decimal VATRate = Convert.ToDecimal(items.Body.UniversalTransaction.TransactionInfo.PostingJournalCollection.PostingJournal.VATTaxIDTaxRate);

                    //dr["VAT_Rate"] = VATRate;

                    //if (VATRate == 15)
                    //{
                    //    dr["Type"] = "VAT";
                    //}
                    //else if (VATRate == 0)
                    //{
                    //    dr["Type"] = "NoNVAT";
                    //}
                    //else
                    //{
                    //    dr["Type"] = "OtherRate";
                    //}

                    dr["VAT_Amount"] = Convert.ToDecimal(items.Body.UniversalTransaction.TransactionInfo.OSGSTVATAmount);

                    //dr["Currency_Code"] = items.Body.UniversalTransaction.TransactionInfo.PostingJournalCollection.PostingJournal.ChargeCurrency;
                    dr["Discount_Amount"] = 0;

                    dr["TransactionType"] = "ServiceNS";
                    //////////dr["CompanyCode"] = code;

                    dr["Promotional_Quantity"] = "0";
                    dr["Sale_Type"] = "New";
                    dr["Vehicle_No"] = "NA";
                    dr["VehicleType"] = "NA";
                    dr["Is_Print"] = "N";
                    dr["Tender_Id"] = "0";
                    dr["Post"] = "Y";
                    dr["LC_Number"] = "NA";
                    dr["CommentsD"] = "NA";
                    dr["Non_Stock"] = "N";
                    dr["Trading_MarkUp"] = "0";
                    dr["VAT_Name"] = "VAT 4.3";
                    dr["Previous_Invoice_No"] = "0";
                    dr["PreviousNBRPrice"] = "0";
                    dr["PreviousQuantity"] = "0";
                    dr["PreviousSubTotal"] = "0";
                    dr["PreviousVATAmount"] = "0";
                    dr["PreviousVATRate"] = "0";
                    dr["PreviousSD"] = "0";
                    dr["PreviousSDAmount"] = "0";
                    dr["ReasonOfReturn"] = "NA";

                    dt.Rows.Add(dr);

                }
                #endregion

            }

            catch (ArgumentException e)
            {
                //FileLogger.Log("DHL form", "GetIBS", "Price can not be zero" + srcFileName);
                throw e;
            }
            catch (Exception e)
            {
                throw e;
            }

            return dt;
        }

        #endregion

        #region Purchase

        public DataTable GetPurchaseData(string xml, SqlConnection vConnection = null, SqlTransaction vTransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region variable

            string[] result = new string[4];
            result[0] = "Fail";
            result[1] = "Fail";
            result[2] = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            CommonDAL commonDal = new CommonDAL();

            #endregion variable

            try
            {
                DataTable dt = new DataTable();

                dt = GetPurchaseXMLData(xml);

                if (dt == null || dt.Rows.Count == 0)
                {
                    return dt;
                }

                #region Open Connection and Transaction

                if (vConnection != null)
                {
                    currConn = vConnection;
                }

                if (vTransaction != null)
                {
                    transaction = vTransaction;
                }

                if (vConnection == null)
                {
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    currConn.Open();

                }

                if (vTransaction == null)
                {
                    transaction = currConn.BeginTransaction();
                }
                #endregion

                #region tempTable

                string tempTable = @"
create table #purchasetemp(              
sl int identity(1,1), 
ID varchar(6000),
BranchCode varchar(100),
Vendor_Code varchar(100),
Vendor_Name varchar(100),
Receive_Date varchar(100),
Invoice_Date varchar(100),
Referance_No varchar(100),
LC_No varchar(100),
BE_Number varchar(100),
Item_Code varchar(100),
Item_Name varchar(100),
Quantity decimal(25,7),
UOM varchar(100),
VAT_Amount decimal(25,7),
Total_Price decimal(25,7),
Type varchar(100),
SD_Amount decimal(25,7),
Assessable_Value decimal(25, 2),
CD_Amount decimal(25,7),
RD_Amount decimal(25,7),
AT_Amount decimal(25,7),
AITAmount decimal(25,7),
Others_Amount decimal(25,7),
Remarks varchar(100),
With_VDS varchar(100),
Comments varchar(200),
Post varchar(100),
Rebate_Rate decimal(25,7),
Transection_Type varchar(100),
VATRate decimal(25,7),
Previous_Purchase_No nvarchar(100)
)";

                #endregion

                #region getAllData

                string getAllData = @"

select 
 ID
,BranchCode
,Vendor_Code
,Vendor_Name
,Receive_Date
,Invoice_Date
,Referance_No
,LC_No
,BE_Number
,Item_Code
,Item_Name
,UOM
,Quantity
,VAT_Amount
,Total_Price
,Type
,SD_Amount
,Assessable_Value
,CD_Amount
,RD_Amount
,AT_Amount
,AITAmount
,Others_Amount
,Remarks
,With_VDS
,Comments
,Post
,Rebate_Rate
,VATRate
,Transection_Type
,Previous_Purchase_No

from #purchasetemp
where 1=1 

";
                
                getAllData += @" drop table #purchasetemp";

                #endregion

                SqlCommand cmd = new SqlCommand(tempTable, currConn, transaction);
                cmd.ExecuteNonQuery();

                result = commonDal.BulkInsert("#purchasetemp", dt, currConn, transaction);

                cmd.CommandText = getAllData;
                
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                dt = new DataTable();

                adapter.Fill(dt);

                return dt;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public DataTable GetPurchaseXMLData(string xml)
        {
            string rXML = xml.Replace(@"<?xml version=""1.0"" encoding=""UTF-8""?>", "");

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(rXML);

            DataTable dt = new DataTable();

            try
            {
                XmlNodeList invoices = xmlDocument.GetElementsByTagName("UniversalInterchange");
                XmlNodeList invoicesLines = xmlDocument.GetElementsByTagName("PostingJournal");

                #region Create DataTable

                dt.Clear();
                dt.Columns.Add("ID");
                dt.Columns.Add("BranchCode");
                dt.Columns.Add("Vendor_Code");
                dt.Columns.Add("Vendor_Name");
                dt.Columns.Add("Receive_Date");
                dt.Columns.Add("Invoice_Date");
                dt.Columns.Add("Referance_No");
                dt.Columns.Add("LC_No");
                dt.Columns.Add("BE_Number");
                dt.Columns.Add("Item_Code");
                dt.Columns.Add("Item_Name");
                dt.Columns.Add("UOM");
                dt.Columns.Add("Quantity");
                dt.Columns.Add("VAT_Amount");
                dt.Columns.Add("Total_Price");
                dt.Columns.Add("Type");
                dt.Columns.Add("SD_Amount");
                dt.Columns.Add("Assessable_Value");
                dt.Columns.Add("CD_Amount");
                dt.Columns.Add("RD_Amount");
                dt.Columns.Add("AT_Amount");
                dt.Columns.Add("AITAmount");
                dt.Columns.Add("Others_Amount");
                dt.Columns.Add("Remarks");
                dt.Columns.Add("With_VDS");
                dt.Columns.Add("Comments");
                dt.Columns.Add("Post");
                dt.Columns.Add("Rebate_Rate");
                dt.Columns.Add("VATRate");
                dt.Columns.Add("Transection_Type");
                dt.Columns.Add("Previous_Purchase_No");


                #endregion

                #region purchase

                string CurrentTime = DateTime.Now.ToString("HH:mm:ss");

                foreach (XmlNode invoice in invoices)
                {
                    XmlNode Body = invoice.SelectSingleNode("Body");
                    XmlNode UniversalTransaction = Body.SelectSingleNode("UniversalTransaction");
                    XmlNode TransactionInfo = UniversalTransaction.SelectSingleNode("TransactionInfo");
                    XmlNode Branch = TransactionInfo.SelectSingleNode("Branch");

                    XmlNode RelatedOrgs = TransactionInfo.SelectSingleNode("RelatedOrgs");
                    XmlNode RelatedOrg = RelatedOrgs.SelectSingleNode("RelatedOrg");

                    string ID = TransactionInfo["TransactionNumber"].InnerText;
                    ////////string Reference_No = RelatedOrg["JOBID"].InnerText;
                    string Referance_No = ID;
                    string Branch_Code = Branch["Code"].InnerText;
                    string Vendor_Code = TransactionInfo["Organization"].InnerText;
                    string Vendor_Name = TransactionInfo["Organization"].InnerText;
                    string BE_Number = TransactionInfo["TransactionNumber"].InnerText;

                    string Invoice_Date = Convert.ToDateTime(TransactionInfo["InvoiceDate"].InnerText).ToString("yyyy-MM-dd " + CurrentTime);
                    string Receive_Date = Convert.ToDateTime(TransactionInfo["PostDate"].InnerText).ToString("yyyy-MM-dd " + CurrentTime);
                    
                    string TransactionType = TransactionInfo["TransactionType"].InnerText;

                    XmlNode OriginalTransactionNumber = TransactionInfo.SelectSingleNode("OriginalReferenceOriginalTransactionNumber");
                    XmlNode OriginalTransactionDate = TransactionInfo.SelectSingleNode("OriginalReferenceOriginalTransactionDate");
                    XmlNode Description = TransactionInfo.SelectSingleNode("Description");
                   
                    string PrevTransactionNumber = string.Empty;
                    if (OriginalTransactionNumber != null)
                    {
                        PrevTransactionNumber = TransactionInfo["OriginalReferenceOriginalTransactionNumber"].InnerText;  
                    }

                    string Remarks = TransactionInfo["Description"].InnerText;

                    string CreatorEMail = TransactionInfo["CreatorEMail"].InnerText;
                    string EditorEMail = TransactionInfo["EditorEMail"].InnerText;

                    XmlNode DebtorCode = TransactionInfo.SelectSingleNode("ExternalDebtorCode");
                    string ExternalDebtorCode = "";

                    if (DebtorCode != null)
                    {
                        ExternalDebtorCode = DebtorCode.InnerText;
                    }

                    string Comments = "-";

                    XmlNode BatchGenTimeInfo = invoice.SelectSingleNode("BatchGenTimeInfo");
                    string LocalSystemTime = BatchGenTimeInfo["LocalSystemTime"].InnerText;  

                    foreach (XmlNode invoicesLine in invoicesLines)
                    {                       
                        DataRow dr = dt.NewRow();
                        dr["ID"] = ID;
                        dr["BranchCode"] = Branch_Code;
                        dr["Vendor_Code"] = Vendor_Code;
                        dr["Vendor_Name"] = Vendor_Name;
                        dr["BE_Number"] = BE_Number;
                        dr["Receive_Date"] = Receive_Date;
                        dr["Invoice_Date"] = Invoice_Date;
                        dr["Referance_No"] = Referance_No;
                        dr["LC_No"] = "-";
                        dr["Item_Code"] = invoicesLine["ChargeCode"].InnerText;
                        dr["Item_Name"] = invoicesLine["ChargeCodeLocalDescription"].InnerText;
                        dr["UOM"] = invoicesLine["QuantityUnit"].InnerText;

                        string Transection_Type = "";
                        if (!string.IsNullOrWhiteSpace(TransactionType) && TransactionType.ToLower() == "crd")
                        {
                            Transection_Type = "PurchaseReturn"; 
                        }
                        else
                        {
                            Transection_Type = invoicesLine["ChargeCurrency"].InnerText.ToLower() == "bdt" ? "ServiceNS" : "ServiceNSImport";
                        }

                        dr["Transection_Type"] = Transection_Type;

                        if (!string.IsNullOrWhiteSpace(TransactionType) && TransactionType.ToLower() == "crd")
                        {
                            dr["Previous_Purchase_No"] = !string.IsNullOrWhiteSpace(PrevTransactionNumber) ? PrevTransactionNumber : "0";                            
                        }
                        else
                        {
                            dr["Previous_Purchase_No"] = "0";
                        }                        

                        decimal Quantity = Convert.ToDecimal(invoicesLine["Quantity"].InnerText);
                        decimal Total_Price = Convert.ToDecimal(invoicesLine["LocalTotalAmount"].InnerText);
                        string VATType = invoicesLine["VATTaxIDTaxCode"].InnerText;

                        decimal VAT_Amount = Convert.ToDecimal(invoicesLine["LocalVATAmount"].InnerText);

                        dr["Quantity"] = Quantity;
                        dr["VAT_Amount"] = VAT_Amount;
                        dr["Total_Price"] = Total_Price;
                        dr["Type"] = VATType;
                        dr["SD_Amount"] = 0;
                        dr["Assessable_Value"] = Total_Price;
                        dr["CD_Amount"] = 0;
                        dr["RD_Amount"] = 0;
                        dr["AT_Amount"] = 0;
                        dr["AITAmount"] = 0;
                        dr["Others_Amount"] = 0;
                        dr["Remarks"] = Remarks;
                        dr["With_VDS"] = "N";
                        dr["Comments"] = Comments;
                        dr["Post"] = "N";
                        dr["Rebate_Rate"] = 0;
                        //////dr["VATRate"] = VatRate;

                        //dr["Option5"] = ExternalDebtorCode;
                        //dr["Option3"] = CreatorEMail;
                        //dr["Option4"] = EditorEMail;
                        //dr["Option6"] = OSTotalAmount;
                        //dr["LocalSystemTime"] = LocalSystemTime;

                        dt.Rows.Add(dr);

                    }

                }
                #endregion

            }

            catch (ArgumentException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw e;
            }

            return dt;
        }

        #endregion



    }
}
