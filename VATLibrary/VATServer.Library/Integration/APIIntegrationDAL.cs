
using System;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using VATViewModel.DTOs;
using VATViewModel.Integration.JsonModels;
using System.Data.SqlClient;
using System.Collections.Generic;
using VATServer.Ordinary;

namespace VATServer.Library
{
    public class APIIntegrationDAL
    {
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
     

        public APIIntegrationDAL()
        {
        }

        #region Sale Data
        private void SaleTableValidation(DataTable salesData)
        {
            List<string> oldColumnNames = new List<string> { "BranchCode", "CustomerCode", "DeliveryAddress", "InvoiceDateTime", "ReferenceNo", "PreviousInvoiceNo", "CurrencyCode", "VehicleNo", "IsPrint"
            ,"LCNumber","SaleType","ItemCode","NBRPrice","SDRate","VATRate","VATAmount","DiscountAmount","PromotionalQuantity","VATName","TenderId","CommercialName"};

            List<string> newColumnNames = new List<string> { "Branch_Code", "Customer_Code", "Delivery_Address", "Invoice_Date_Time", "Reference_No", "Previous_Invoice_No", "Currency_Code", "Vehicle_No", "Is_Print"
            ,"LC_Number","Sale_Type","Item_Code","NBR_Price","SD_Rate","VAT_Rate","VAT_Amount","Discount_Amount","Promotional_Quantity","VAT_Name","Tender_Id","ProductDescription"};

            salesData = OrdinaryVATDesktop.DtColumnNameChangeList(salesData, oldColumnNames, newColumnNames);


            var Customer_Name = new DataColumn("Customer_Name") { DefaultValue = "-" };
            var Item_Name = new DataColumn("Item_Name") { DefaultValue = "-" };
            var SL = new DataColumn("SL") { DefaultValue = "" };
            var CreatedBy = new DataColumn("CreatedBy") { DefaultValue = salesData.Rows[0]["User"].ToString() };
            var ReturnId = new DataColumn("ReturnId") { DefaultValue = 0 };
            var BOMId = new DataColumn("BOMId") { DefaultValue = 0 };
            var CreatedOn = new DataColumn("CreatedOn") { DefaultValue = DateTime.Now.ToString() };

            salesData.Columns.Add(Customer_Name);
            salesData.Columns.Add(Item_Name);
            salesData.Columns.Add(SL);
            salesData.Columns.Add(CreatedBy);
            salesData.Columns.Add(CreatedOn);

            if (!salesData.Columns.Contains("ReturnId"))
            {
                salesData.Columns.Add(ReturnId);
            }

            salesData.Columns.Add(BOMId);

            if (salesData.Columns.Contains("ProcessDateTime"))
            {
                salesData.Columns.Remove("ProcessDateTime");
            }

            List<string> ColumnNames = new List<string> { "BIN", "User" };
            OrdinaryVATDesktop.DtDeleteColumns(salesData, ColumnNames);
        }


        public string[] SaveSaleData(DataTable salesData, SysDBInfoVMTemp connVM = null)
        {
            SaleDAL salesDal = new SaleDAL();
            CommonDAL commonDal = new CommonDAL();

            string[] result = new string[6];

            result[0] = "Fail";
            result[1] = "Fail";

            try
            {
                var ProcessDateTime = new DataColumn("ProcessDateTime") { DefaultValue = DateTime.Now.ToString() };
                salesData.Columns.Add(ProcessDateTime);

                result = commonDal.BulkInsert("SaleAPIAudit", salesData, null, null);

                SaleTableValidation(salesData);

                result = salesDal.SaveAndProcess(salesData, () => { }, 1, "", connVM, null, null, null, "");

            }
            catch (Exception ex)
            {
                result[0] = "Fail";
                result[1] = ex.Message;

                throw ex;
            }

            return result;
        }

        #endregion


        #region Purchase Data
        private void PurchaseTableValidation(DataTable purchaseData)
        {
            List<string> oldColumnNames = new List<string> { "VendorCode", "ReferenceNo", "TransactionType","InvoiceDate", "ReceiveDate", "RebateDate", "LCNo", "BENumber", "ItemCode", "WithVDS"
            ,"TotalPrice","CDAmount","RDAmount","SDAmount","VATAmount","ATAmount","OthersAmount"};

            List<string> newColumnNames = new List<string> {"Vendor_Code", "Referance_No", "Transection_Type", "Invoice_Date", "Receive_Date", "Rebate_Date", "LC_No", "BE_Number", "Item_Code", "With_VDS"
            ,"Total_Price","CD_Amount","RD_Amount","SD_Amount","VAT_Amount","AT_Amount","Others_Amount"};

            purchaseData = OrdinaryVATDesktop.DtColumnNameChangeList(purchaseData, oldColumnNames, newColumnNames);


            var Customer_Name = new DataColumn("Vendor_Name") { DefaultValue = "-" };
            var Item_Name = new DataColumn("Item_Name") { DefaultValue = "-" };
            var SL = new DataColumn("SL") { DefaultValue = "" };
            var Rebate_Rate = new DataColumn("Rebate_Rate") { DefaultValue = "0" };


            purchaseData.Columns.Add(Rebate_Rate);
            purchaseData.Columns.Add(Customer_Name);
            purchaseData.Columns.Add(Item_Name);
            purchaseData.Columns.Add(SL);


            if (purchaseData.Columns.Contains("ProcessDateTime"))
            {
                purchaseData.Columns.Remove("ProcessDateTime");
            }
            
            List<string> ColumnNames = new List<string> { "BIN", "User" };
            OrdinaryVATDesktop.DtDeleteColumns(purchaseData, ColumnNames);
        }


        public string[] SavePurchaseData(DataTable PurchaseData, SysDBInfoVMTemp connVM = null)
        {
            PurchaseDAL purchaseDal = new PurchaseDAL();
            CommonDAL commonDal = new CommonDAL();

            string[] result = new string[6];

            result[0] = "Fail";
            result[1] = "Fail";


            try
            {

                var ProcessDateTime = new DataColumn("ProcessDateTime") { DefaultValue = DateTime.Now.ToString() };
                PurchaseData.Columns.Add(ProcessDateTime);

                result = commonDal.BulkInsert("PurchaseAPIAudit", PurchaseData, null, null);

                PurchaseTableValidation(PurchaseData);

               result = purchaseDal.SaveTempPurchase(PurchaseData,"", "", "API", 1, () => { }, null, null, connVM);

            }
            catch (Exception ex)
            {
                result[0] = "Fail";
                result[1] = ex.Message;

                throw ex;
            }

            return result;
        }

        #endregion


        #region Transfer Data
        private void TransferTableValidation(DataTable transferData)
        {



            List<string> oldColumnNames = new List<string> { "ItemCode", "VATRate" };

            List<string> newColumnNames = new List<string> { "ProductCode", "VAT_Rate" };

            transferData = OrdinaryVATDesktop.DtColumnNameChangeList(transferData, oldColumnNames, newColumnNames);

            var ProductName = new DataColumn("ProductName") { DefaultValue = "-"};

            transferData.Columns.Add(ProductName);


            if (transferData.Columns.Contains("ProcessDateTime"))
            {
                transferData.Columns.Remove("ProcessDateTime");
            }

            List<string> ColumnNames = new List<string> { "BIN", "User" };
            OrdinaryVATDesktop.DtDeleteColumns(transferData, ColumnNames);
        }


        public string[] SaveTransferData(DataTable TransferData, SysDBInfoVMTemp connVM = null)
        {
            TransferIssueDAL _TransferIssueDAL = new TransferIssueDAL();
            CommonDAL commonDal = new CommonDAL();

            string[] result = new string[6];

            result[0] = "Fail";
            result[1] = "Fail";


            try
            {

                var ProcessDateTime = new DataColumn("ProcessDateTime") { DefaultValue = DateTime.Now.ToString() };
                TransferData.Columns.Add(ProcessDateTime);

                result = commonDal.BulkInsert("TransferAPIAudit", TransferData, null, null);

                TransferTableValidation(TransferData);

                result = _TransferIssueDAL.SaveTempTransfer(TransferData, null, null, "API", 0, () => { }, null, null, true, connVM, DateTime.Now.ToString("HH:mm:ss"),"");

            }
            catch (Exception ex)
            {
                result[0] = "Fail";
                result[1] = ex.Message;

                throw ex;
            }

            return result;
        }

        #endregion


        #region Production Issue Data
        private void ProductionIssueTableValidation(DataTable issueData)
        {
            List<string> oldColumnNames = new List<string> { "IssueDateTime", "ReferenceNo", "ReturnId", "ItemCode" };

            List<string> newColumnNames = new List<string> { "Issue_DateTime", "Reference_No", "Return_Id", "Item_Code" };

            issueData = OrdinaryVATDesktop.DtColumnNameChangeList(issueData, oldColumnNames, newColumnNames);

            var Item_Name = new DataColumn("Item_Name") { DefaultValue = "-" };

            issueData.Columns.Add(Item_Name);


            if (issueData.Columns.Contains("ProcessDateTime"))
            {
                issueData.Columns.Remove("ProcessDateTime");
            }

            List<string> ColumnNames = new List<string> { "BIN", "User" };
            OrdinaryVATDesktop.DtDeleteColumns(issueData, ColumnNames);
        }


        public string[] SaveProductionIssueData(DataTable IssueData, SysDBInfoVMTemp connVM = null)
        {
            IssueDAL issueDal = new IssueDAL();
            CommonDAL commonDal = new CommonDAL();

            string[] result = new string[6];

            result[0] = "Fail";
            result[1] = "Fail";


            try
            {
                var ProcessDateTime = new DataColumn("ProcessDateTime") { DefaultValue = DateTime.Now.ToString() };
                IssueData.Columns.Add(ProcessDateTime);

                result = commonDal.BulkInsert("IssueAPIAudit", IssueData, null, null);

                ProductionIssueTableValidation(IssueData);
                IntegrationParam param = new IntegrationParam()
                {
                    DefaultBranchId = 1,
                    TransactionType = "Other",
                    callBack = () => { },
                    CurrentUser = "API",
                    Data = IssueData

                };
                result = issueDal.SaveIssue_Split(param);


            }
            catch (Exception ex)
            {
                result[0] = "Fail";
                result[1] = ex.Message;

                throw ex;
            }

            return result;
        }

        #endregion










    }

}
