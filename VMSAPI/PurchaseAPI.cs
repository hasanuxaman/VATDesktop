using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Xml;
using VATServer.Library;
using VATServer.Library.Integration;
using VATServer.Ordinary;
using VATViewModel.DTOs;

namespace VMSAPI
{
    public class PurchaseAPI
    {
        public string SavePurchase(string xml, SysDBInfoVMTemp connVM = null)
        {
            var resultSet = new DataSet();

            DataTable table = new DataTable();
            table.Columns.Add("Result");
            table.Columns.Add("Message");
            var resultXML = "";
            string transactionType = "";

            PurchaseDAL dal = new PurchaseDAL();

            try
            {
                DataSet dataSet = OrdinaryVATDesktop.GetDataSetFromXML(xml);

                DataTable purchaseData = dataSet.Tables[0];

                if (purchaseData == null || purchaseData.Rows.Count == 0)
                {
                    table.Rows.Add("fail", "Purchase Import Fail");
                    resultSet.Tables.Add(table);

                    return OrdinaryVATDesktop.GetXMLFromDataSet(resultSet);
                }

                if (purchaseData.Columns.Contains("Branch_Code"))
                {
                    purchaseData.Columns["Branch_Code"].ColumnName = "BranchCode";
                }

                transactionType = "Other";//purchaseData.Rows[0]["Transection_Type"].ToString();

                //purchaseData.Columns.Remove("Transection_Type");

                if (purchaseData.Columns.Contains("CnF_Amount"))
                {
                    if (purchaseData != null && purchaseData.Rows.Count != 0)
                    {
                        foreach (DataRow row in purchaseData.Rows)
                        {
                            string cnfAmt = row["CnF_Amount"].ToString();
                            if (string.IsNullOrWhiteSpace(cnfAmt))
                            {
                                row["CnF_Amount"] = 0;
                            }

                        }
                    }
                }

                string[] results = dal.SaveTempPurchase(purchaseData, "", transactionType, "API", 0, CallBack, null, null, connVM);

                DataTable resTable = new DataTable();
                resTable.Columns.Add("Result");
                resTable.Columns.Add("Message");
                resTable.Columns.Add("FileName");

                #region File Name Generation

                DataView dataView = new DataView(purchaseData);

                DataTable refs = dataView.ToTable(true, "ID");

                string fileName = "";

                foreach (DataRow dataRow in refs.Rows)
                {
                    fileName += dataRow["ID"] + ",";
                }

                fileName = fileName.TrimEnd(',');

                #endregion

                if (results[0].ToLower() == "success")
                {
                    resTable.Rows.Add("success", "Successfully Imported.", fileName);
                    resultSet.Tables.Add(resTable);
                }
                else
                {
                    resTable.Rows.Add("fail", results[1], fileName);
                    resultSet.Tables.Add(resTable);
                }

                resultXML = OrdinaryVATDesktop.GetXMLFromDataSet(resultSet);

                return resultXML;


            }
            catch (Exception e)
            {
                throw e;
            }
        }

        #region British Council

        public ResultVM SavePurchaseBritishCouncil(string xml, SysDBInfoVMTemp connVM = null)
        {
            ////var resultSet = new DataSet();

            ////table.Columns.Add("Result");
            ////table.Columns.Add("Message");
            ////var resultXML = "";

            PurchaseDAL dal = new PurchaseDAL();

            //////string[] results;
            DataTable refDt = new DataTable();
            DataTable ApiDt = new DataTable();
            ResultVM rVM = new ResultVM();
            BritishCouncil_IntegrationDAL _dal = new BritishCouncil_IntegrationDAL();
            string fileName = "";
            DataTable table = new DataTable();
            string transactionType = "";
            DataTable rdt = new DataTable();
            string GetNumber = "";

            try
            {
                GetNumber = DateTime.Now.ToString("yyyyMMddHHmmss");

                table = _dal.GetPurchaseData(xml, connVM);

                transactionType = table.Rows[0]["Transection_Type"].ToString();

                if (table.Columns.Contains("CnF_Amount"))
                {
                    if (table != null && table.Rows.Count != 0)
                    {
                        foreach (DataRow row in table.Rows)
                        {
                            string cnfAmt = row["CnF_Amount"].ToString();
                            if (string.IsNullOrWhiteSpace(cnfAmt))
                            {
                                row["CnF_Amount"] = 0;
                            }

                        }
                    }
                }

                #region File Name Generation

                DataView dataView = new DataView(table);

                refDt = dataView.ToTable(true, "ID");

                foreach (DataRow dataRow in refDt.Rows)
                {
                    fileName += dataRow["ID"] + ",";
                }

                fileName = fileName.TrimEnd(',');

                #endregion

                #region Response File Name

                string FileID = table.Rows[0]["FileName"].ToString();

                rVM.ResponseFileName = "Purchase" + FileID + "_" + DateTime.Now.ToString("yyyyMMddHHmmss");

                #endregion

                #region response dt

                dataView = new DataView(table);

                rdt = dataView.ToTable(true, "FileName", "ID", "Vendor_Name", "Invoice_Date");

                #endregion

                if (table != null && table.Rows.Count > 0)
                {
                    DataView view = new DataView(table);
                    DataTable distinctValues = view.ToTable(true, "ID");

                    foreach (DataRow row in distinctValues.Rows)
                    {
                        string ID = row["ID"].ToString();

                        DataRow[] rows = table.Select("ID = '" + ID + "'");

                        DataTable finaldata = rows.CopyToDataTable();

                        try
                        {

                            string[] results = dal.SaveTempPurchase(finaldata, "", transactionType, "API", 0, CallBack, null, null, connVM);

                            if (results[0].ToLower() == "success")
                            {

                                DataView dv = new DataView(finaldata);
                                DataTable ids = dv.ToTable(true, "ID");
                                DataTable getDt = _dal.SelectPurchaseDataForAPI(ids, null, null, connVM);

                                string[] result = _dal.SaveAPIData(getDt, "", "", true, GetNumber, null, null, connVM);
                            }
                            else
                            {
                                string message = results[1].ToString();
                                message = message.Replace("\r\n", " ");

                                DataView dv = new DataView(finaldata.Copy());
                                DataTable apiData = dv.ToTable(true, "ID", "FileName", "Invoice_Date", "Vendor_Name");

                                string[] result = _dal.SaveAPIData(apiData, results[0].ToString(), message, true, GetNumber, null, null, connVM);

                            }

                        }
                        catch (Exception ex)
                        {

                            #region InsertErrorLogs

                            try
                            {
                                ErrorLogVM evm = new ErrorLogVM();

                                evm.ImportId = ID;
                                evm.FileName = fileName;
                                evm.ErrorMassage = ex.Message;
                                evm.ErrorDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                evm.SourceName = "ImportSaleBritishCouncil_InvoiceWise";
                                evm.ActionName = "SalesAPI";
                                evm.TransactionName = "Sales";

                                CommonDAL _cDal = new CommonDAL();

                                string[] result = _cDal.InsertErrorLogs(evm, null, null, connVM);

                            }
                            catch (Exception e)
                            {

                            }
                            #endregion

                            #region Respon save

                            string message = ex.Message.ToString();
                            message = message.Replace("\r\n", " ");

                            DataView dv = new DataView(finaldata.Copy());
                            DataTable apiData = dv.ToTable(true, "ID", "FileName", "Invoice_Date", "Vendor_Name");

                            string[] reresult = _dal.SaveAPIData(apiData, "Fail", message, true, GetNumber, null, null, connVM);

                            #endregion

                        }

                    }
                }

                rVM.XML = CreateXMLAPI_BC(refDt, GetNumber, connVM);

                #region Comments

                ////////string[] results = dal.SaveTempPurchase(table, "", transactionType, "API", 0, CallBack);

                ////////rVM.Status = results[0];
                ////////rVM.Message = results[1];

                ////////if (rVM.Status == "Success")
                ////////{
                ////////    ApiDt = dal.SelectAllForAPI(refDt);
                ////////    rVM.XML = CreateXML(rVM, ApiDt);
                ////////}
                ////////else
                ////////{
                ////////    rVM.XML = getXML(rVM, rdt);
                ////////}

                #endregion

                return rVM;

            }
            catch (Exception ex)
            {
                string message = ex.Message.ToString();
                message = message.Replace("\r\n", " ");

                rVM.Status = "Fail";
                rVM.Message = message;


                #region InsertErrorLogs

                try
                {
                    ErrorLogVM evm = new ErrorLogVM();

                    evm.ImportId = fileName;
                    evm.FileName = fileName;
                    evm.ErrorMassage = ex.Message;
                    evm.ErrorDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    evm.SourceName = "SavePurchaseBritishCouncil";
                    evm.ActionName = "PurchaseAPI";
                    evm.TransactionName = "Purchase";

                    CommonDAL _cDal = new CommonDAL();

                    string[] result = _cDal.InsertErrorLogs(evm, null, null, connVM);

                }
                catch (Exception e)
                {

                }

                #endregion

                #region CreateXMLAPI

                string[] reresult = _dal.SaveAPIData(rdt, rVM.Status, rVM.Message, true, GetNumber, null, null, connVM);

                rVM.XML = CreateXMLAPI_BC(rdt, GetNumber, connVM);

                #endregion

                ////////rVM.XML = getXML(rVM, rdt);

                return rVM;

            }
        }

        private string CreateXMLAPI_BC(DataTable dt, string GetNumber, SysDBInfoVMTemp connVM = null)
        {
            BritishCouncil_IntegrationDAL _dal = new BritishCouncil_IntegrationDAL();

            DataTable ApiDt = _dal.SelectResponseDataForAPI(dt, GetNumber, null, null, connVM);

            string xmlString = CreateXML_BC(ApiDt, connVM);

            return xmlString;

        }

        private string CreateXML_BC(DataTable dt, SysDBInfoVMTemp connVM = null)
        {
            // Create an XmlDocument object
            XmlDocument xmlDocument = new XmlDocument();

            // Create the XML declaration
            XmlDeclaration xmlDeclaration = xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null);
            xmlDocument.InsertBefore(xmlDeclaration, xmlDocument.DocumentElement);

            // Create the root element
            XmlElement rootElement = xmlDocument.CreateElement("InvoiceInfo");
            xmlDocument.AppendChild(rootElement);

            ////////// Create the master element
            //XmlElement masterElement = xmlDocument.CreateElement("InvoiceStatus");
            //rootElement.AppendChild(masterElement);
            ////////// Set the status and message attributes of the master element
            //masterElement.SetAttribute("Status", rVM.Status);
            //masterElement.SetAttribute("Message", rVM.Message);

            XmlElement FileIdElement = xmlDocument.CreateElement("FileId");
            FileIdElement.InnerText = dt.Rows[0]["FileId"].ToString();
            rootElement.AppendChild(FileIdElement);

            //////////XmlElement StatusElement = xmlDocument.CreateElement("Status");
            //////////StatusElement.InnerText = rVM.Status;
            //////////rootElement.AppendChild(StatusElement);

            //////////XmlElement MessageElement = xmlDocument.CreateElement("Message");
            //////////MessageElement.InnerText = rVM.Message;
            //////////rootElement.AppendChild(MessageElement);

            XmlElement InvoiceDetailsElement = xmlDocument.CreateElement("InvoiceDetails");
            rootElement.AppendChild(InvoiceDetailsElement);

            // Create the detail elements
            foreach (DataRow row in dt.Rows)
            {
                XmlElement detailElement = xmlDocument.CreateElement("Invoice");
                InvoiceDetailsElement.AppendChild(detailElement);

                // Create and set the detail element values
                XmlElement idElement = xmlDocument.CreateElement("SAPTransactionNumber");
                idElement.InnerText = row["TransactionNumber"].ToString();
                detailElement.AppendChild(idElement);

                XmlElement dateElement = xmlDocument.CreateElement("SAPTransactionDateTime");
                dateElement.InnerText = row["InvoiceDateTime"].ToString();
                detailElement.AppendChild(dateElement);

                XmlElement StatusElement = xmlDocument.CreateElement("Status");
                StatusElement.InnerText = row["Status"].ToString();
                detailElement.AppendChild(StatusElement);

                XmlElement MessageElement = xmlDocument.CreateElement("Message");
                MessageElement.InnerText = row["Message"].ToString();
                detailElement.AppendChild(MessageElement);

                XmlElement invoiceNoElement = xmlDocument.CreateElement("InvoiceNo");
                invoiceNoElement.InnerText = row["InvoiceNo"].ToString();
                detailElement.AppendChild(invoiceNoElement);

                XmlElement InvoicedateElement = xmlDocument.CreateElement("InvoiceDateTime");
                InvoicedateElement.InnerText = row["InvoiceDateTime"].ToString();
                detailElement.AppendChild(InvoicedateElement);

                ////XmlElement FileId = xmlDocument.CreateElement("FileId");
                ////FileId.InnerText = row["FileId"].ToString();
                ////detailElement.AppendChild(FileId);

                XmlElement customerElement = xmlDocument.CreateElement("VendorName");
                customerElement.InnerText = row["VendorCustomer"].ToString();
                detailElement.AppendChild(customerElement);

                XmlElement amountElement = xmlDocument.CreateElement("TotalAmount");
                amountElement.InnerText = row["TotalAmount"].ToString();
                detailElement.AppendChild(amountElement);

                XmlElement vatElement = xmlDocument.CreateElement("TotalVATAmount");
                vatElement.InnerText = row["TotalVATAmount"].ToString();
                detailElement.AppendChild(vatElement);
            }

            string xmlString = xmlDocument.OuterXml;

            return xmlString;

        }

        private string CreateXML(ResultVM rVM, DataTable dt, SysDBInfoVMTemp connVM = null)
        {
            XmlDocument xmlDocument = new XmlDocument();

            XmlDeclaration xmlDeclaration = xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null);
            xmlDocument.InsertBefore(xmlDeclaration, xmlDocument.DocumentElement);

            XmlElement rootElement = xmlDocument.CreateElement("InvoiceInfo");
            xmlDocument.AppendChild(rootElement);

            XmlElement FileIdElement = xmlDocument.CreateElement("FileId");
            FileIdElement.InnerText = dt.Rows[0]["FileId"].ToString();
            rootElement.AppendChild(FileIdElement);

            XmlElement StatusElement = xmlDocument.CreateElement("Status");
            StatusElement.InnerText = rVM.Status;
            rootElement.AppendChild(StatusElement);

            XmlElement MessageElement = xmlDocument.CreateElement("Message");
            MessageElement.InnerText = rVM.Message;
            rootElement.AppendChild(MessageElement);

            XmlElement InvoiceDetailsElement = xmlDocument.CreateElement("InvoiceDetails");
            rootElement.AppendChild(InvoiceDetailsElement);

            foreach (DataRow row in dt.Rows)
            {
                XmlElement detailElement = xmlDocument.CreateElement("Invoice");
                InvoiceDetailsElement.AppendChild(detailElement);

                XmlElement idElement = xmlDocument.CreateElement("SAPTransactionNumber");
                idElement.InnerText = row["ImportID"].ToString();
                detailElement.AppendChild(idElement);

                XmlElement dateElement = xmlDocument.CreateElement("SAPTransactionDateTime");
                dateElement.InnerText = row["InvoiceDateTime"].ToString();
                detailElement.AppendChild(dateElement);

                ////////XmlElement StatusElement = xmlDocument.CreateElement("Status");
                ////////StatusElement.InnerText = rVM.Status;
                ////////detailElement.AppendChild(StatusElement);

                ////////XmlElement MessageElement = xmlDocument.CreateElement("Message");
                ////////MessageElement.InnerText = rVM.Message;
                ////////detailElement.AppendChild(MessageElement);

                //////////XmlElement fileId = xmlDocument.CreateElement("FileId");
                //////////fileId.InnerText = row["FileId"].ToString();
                //////////detailElement.AppendChild(fileId);

                XmlElement invoiceNoElement = xmlDocument.CreateElement("InvoiceNo");
                invoiceNoElement.InnerText = row["PurchaseInvoiceNo"].ToString();
                detailElement.AppendChild(invoiceNoElement);

                XmlElement InvoicedateElement = xmlDocument.CreateElement("InvoiceDateTime");
                InvoicedateElement.InnerText = row["InvoiceDateTime"].ToString();
                detailElement.AppendChild(InvoicedateElement);

                XmlElement customerElement = xmlDocument.CreateElement("Vendor");
                customerElement.InnerText = row["VendorName"].ToString();
                detailElement.AppendChild(customerElement);

                XmlElement amountElement = xmlDocument.CreateElement("TotalAmount");
                amountElement.InnerText = row["TotalAmount"].ToString();
                detailElement.AppendChild(amountElement);

                XmlElement vatElement = xmlDocument.CreateElement("TotalVATAmount");
                vatElement.InnerText = row["TotalVATAmount"].ToString();
                detailElement.AppendChild(vatElement);
            }

            string xmlString = xmlDocument.OuterXml;

            return xmlString;

        }

        private string getXML(ResultVM rVM, DataTable dt, SysDBInfoVMTemp connVM = null)
        {
            DataTable ApiDt = new DataTable();

            ApiDt = dt.Copy();

            #region Make Datatable

            if (!ApiDt.Columns.Contains("ID"))
            {
                var columnName = new DataColumn("ImportID") { DefaultValue = "" };
                ApiDt.Columns.Add(columnName);
            }
            else
            {
                ApiDt.Columns["ID"].ColumnName = "ImportID";
            }

            if (!ApiDt.Columns.Contains("PurchaseInvoiceNo"))
            {
                var columnName = new DataColumn("PurchaseInvoiceNo") { DefaultValue = "" };
                ApiDt.Columns.Add(columnName);
            }
            if (!ApiDt.Columns.Contains("Invoice_Date"))
            {
                var columnName = new DataColumn("InvoiceDateTime") { DefaultValue = "" };
                ApiDt.Columns.Add(columnName);
            }
            else
            {
                ApiDt.Columns["Invoice_Date"].ColumnName = "InvoiceDateTime";
            }

            if (!ApiDt.Columns.Contains("Vendor_Name"))
            {
                var columnName = new DataColumn("VendorName") { DefaultValue = "" };
                ApiDt.Columns.Add(columnName);
            }
            else
            {
                ApiDt.Columns["Vendor_Name"].ColumnName = "VendorName";
            }

            if (!ApiDt.Columns.Contains("TotalAmount"))
            {
                var columnName = new DataColumn("TotalAmount") { DefaultValue = "" };
                ApiDt.Columns.Add(columnName);
            }
            if (!ApiDt.Columns.Contains("TotalVATAmount"))
            {
                var columnName = new DataColumn("TotalVATAmount") { DefaultValue = "" };
                ApiDt.Columns.Add(columnName);
            }

            if (!ApiDt.Columns.Contains("FileName"))
            {
                var columnName = new DataColumn("FileId") { DefaultValue = "" };
                ApiDt.Columns.Add(columnName);
            }
            else
            {
                ApiDt.Columns["FileName"].ColumnName = "FileId";
            }

            #endregion

            string rXML = CreateXML(rVM, ApiDt, connVM);

            return rXML;

        }

        #endregion

        #region Purchase Bollore API

        public ResultVM ImportPurchaseBollore(string PurchaseXML, string UserId = "", SysDBInfoVMTemp connVM = null, PurchaseAPIVM_Bollore svm = null)
        {
            string[] results = new string[6];
            results[0] = "Fail";
            results[1] = "Fail";

            ResultVM rVM = new ResultVM();
            DataTable refDt = new DataTable();
            DataTable ApiDt = new DataTable();
            string rXML = "";
            string fileName = "";
            string transactionType = "";
            string BranchCode = "";

            try
            {
                BolloreIntegrationDAL _dal = new BolloreIntegrationDAL();
                var dataSet = new DataSet();

                DataTable table = _dal.GetPurchaseData(PurchaseXML, null, null, connVM);

                transactionType = table.Rows[0]["Transection_Type"].ToString();
                BranchCode = table.Rows[0]["BranchCode"].ToString();


                #region File Name Generation

                DataView dataView = new DataView(table);

                refDt = dataView.ToTable(true, "ID", "BranchCode");

                foreach (DataRow dataRow in refDt.Rows)
                {
                    fileName += dataRow["ID"] + ",";
                }

                fileName = fileName.TrimEnd(',');

                rVM.FileName = fileName;

                #endregion

                PurchaseDAL purchaseDal = new PurchaseDAL();

                if (!table.Columns.Contains("ProcessDateTime"))
                {
                    var processDateTime = new DataColumn("ProcessDateTime") { DefaultValue = DateTime.Now.ToString() };
                    table.Columns.Add(processDateTime);
                }


                CommonDAL commonDal = new CommonDAL();
                results = commonDal.BulkInsert("PurchaseAudits", table, null, null);

                if (table.Columns.Contains("VATRate"))
                {
                    table.Columns.Remove("VATRate");
                }
                if (table.Columns.Contains("ProcessDateTime"))
                {
                    table.Columns.Remove("ProcessDateTime");
                }

                results = purchaseDal.SaveTempPurchase(table, BranchCode, transactionType, "API", 1, () => { }, null, null, connVM);

                rVM.Status = results[0];
                rVM.Message = results[1];

                rVM.XML = PurchaseResponse_Bollore(rVM, refDt);
                //rVM.XML2 = getXML_Bollore(rVM, refDt);

                return rVM;
            }
            catch (Exception ex)
            {
                rVM.Status = "Fail";
                rVM.Message = ex.Message;

                rVM.XML2 = getXML_Bollore(rVM, refDt);
                rVM.XML = PurchaseResponse_Bollore(rVM, refDt);

                try
                {
                    ErrorLogVM evm = new ErrorLogVM();

                    evm.ImportId = fileName;
                    evm.FileName = fileName;
                    evm.ErrorMassage = ex.Message;
                    evm.ErrorDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    evm.SourceName = "ImportPurchaseBollore";
                    evm.ActionName = "PurchaseAPI";
                    evm.TransactionName = "Purchase";

                    CommonDAL _cDal = new CommonDAL();

                    string[] result = _cDal.InsertErrorLogs(evm, null, null, connVM);

                }
                catch (Exception e)
                {
                    //
                }

                return rVM;
            }
        }

        private void TableValidation_Bollore(DataTable PurchaseData, string TType)
        {

            var column = new DataColumn("SL") { DefaultValue = "" };
            var CreatedBy = new DataColumn("CreatedBy") { DefaultValue = "API" };
            var ReturnId = new DataColumn("ReturnId") { DefaultValue = 0 };
            var BOMId = new DataColumn("BOMId") { DefaultValue = 0 };
            var TransactionType = new DataColumn("TransactionType") { DefaultValue = TType };
            var CreatedOn = new DataColumn("CreatedOn") { DefaultValue = DateTime.Now.ToString() };

            PurchaseData.Columns.Add(column);
            PurchaseData.Columns.Add(CreatedBy);
            PurchaseData.Columns.Add(CreatedOn);

            if (!PurchaseData.Columns.Contains("ReturnId"))
            {
                PurchaseData.Columns.Add(ReturnId);
            }
            if (!PurchaseData.Columns.Contains("TransactionType"))
            {
                PurchaseData.Columns.Add(TransactionType);
            }

            PurchaseData.Columns.Add(BOMId);
        }

        private string PurchaseResponse_Bollore(ResultVM rVM, DataTable dt, PurchaseAPIVM_Bollore svm = null)
        {
            // Create an XmlDocument object
            XmlDocument xmlDocument = new XmlDocument();

            // Create the XML declaration
            XmlDeclaration xmlDeclaration = xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null);
            xmlDocument.InsertBefore(xmlDeclaration, xmlDocument.DocumentElement);

            // Create the root element
            XmlElement rootElement = xmlDocument.CreateElement("TIMIStatus");
            xmlDocument.AppendChild(rootElement);

            XmlElement ValueElement = xmlDocument.CreateElement("Value");
            rootElement.AppendChild(ValueElement);

            XmlElement StatusElement = xmlDocument.CreateElement("TIMIStatus");
            string status = "";
            if (rVM.Status.ToLower() == "success")
            {
                status = "OK";
            }
            else
            {
                status = "KO";
            }
            StatusElement.InnerText = status;
            ValueElement.AppendChild(StatusElement);

            XmlElement MessageElement = xmlDocument.CreateElement("TIMIMessage");
            MessageElement.InnerText = rVM.Message;
            ValueElement.AppendChild(MessageElement);

            string xmlString = xmlDocument.OuterXml;

            return xmlString;

        }

        private string CreateXML_Bollore(ResultVM rVM, DataTable dt, PurchaseAPIVM_Bollore svm = null)
        {
            // Create an XmlDocument object
            XmlDocument xmlDocument = new XmlDocument();

            // Create the XML declaration
            XmlDeclaration xmlDeclaration = xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null);
            xmlDocument.InsertBefore(xmlDeclaration, xmlDocument.DocumentElement);

            // Create the root element
            XmlElement rootElement = xmlDocument.CreateElement("TIMIStatus");
            xmlDocument.AppendChild(rootElement);

            XmlElement ValueElement = xmlDocument.CreateElement("Value");
            rootElement.AppendChild(ValueElement);


            XmlElement StatusElement = xmlDocument.CreateElement("TIMIStatus");

            string status = "";
            if (rVM.Status.ToLower() == "success")
            {
                status = "OK";
            }
            else
            {
                status = "KO";
            }

            StatusElement.InnerText = status;
            ValueElement.AppendChild(StatusElement);

            XmlElement MessageElement = xmlDocument.CreateElement("TIMIMessage");
            MessageElement.InnerText = rVM.Message;
            ValueElement.AppendChild(MessageElement);

            XmlElement TaxIDsElement = xmlDocument.CreateElement("TaxIDs");
            rootElement.AppendChild(TaxIDsElement);

            // Create the detail elements
            foreach (DataRow row in dt.Rows)
            {
                XmlElement detailElement = xmlDocument.CreateElement("TaxID");
                TaxIDsElement.AppendChild(detailElement);

                // Create and set the detail element values

                XmlElement invoiceNoElement = xmlDocument.CreateElement("ExternalDebtorCode");
                invoiceNoElement.InnerText = row["ExternalDebtorCode"].ToString();
                detailElement.AppendChild(invoiceNoElement);

                XmlElement BranchElement = xmlDocument.CreateElement("Branch");
                BranchElement.InnerText = row["Branch"].ToString();
                detailElement.AppendChild(BranchElement);

                XmlElement idElement = xmlDocument.CreateElement("TransactionNumber");
                idElement.InnerText = row["TransactionNumber"].ToString();
                detailElement.AppendChild(idElement);

                XmlElement TransactionReferenceElement = xmlDocument.CreateElement("TransactionReference");
                TransactionReferenceElement.InnerText = row["TransactionReference"].ToString();
                detailElement.AppendChild(TransactionReferenceElement);

                XmlElement BatchTimeElement = xmlDocument.CreateElement("BatchTime");
                BatchTimeElement.InnerText = row["TransactionTime"].ToString();
                detailElement.AppendChild(BatchTimeElement);

                XmlElement dateElement = xmlDocument.CreateElement("TransactionTime");
                dateElement.InnerText = row["TransactionTime"].ToString();
                detailElement.AppendChild(dateElement);

                XmlElement ChargeLinesElement = xmlDocument.CreateElement("ChargeLines");
                ChargeLinesElement.InnerText = row["ChargeLines"].ToString();
                detailElement.AppendChild(ChargeLinesElement);

                XmlElement amountElement = xmlDocument.CreateElement("Amount");
                amountElement.InnerText = row["Amount"].ToString();
                detailElement.AppendChild(amountElement);

                XmlElement EditorEMailElement = xmlDocument.CreateElement("EditorEMail");
                EditorEMailElement.InnerText = row["EditorEMail"].ToString();
                detailElement.AppendChild(EditorEMailElement);

                XmlElement CreatorEMailElement = xmlDocument.CreateElement("CreatorEMail");
                CreatorEMailElement.InnerText = row["CreatorEMail"].ToString();
                detailElement.AppendChild(CreatorEMailElement);

            }

            string xmlString = xmlDocument.OuterXml;

            return xmlString;

        }

        private string getXML_Bollore(ResultVM rVM, DataTable dt, PurchaseAPIVM_Bollore svm = null)
        {
            DataTable ApiDt = new DataTable();

            ApiDt = dt.Copy();

            #region Make Datatable

            if (!ApiDt.Columns.Contains("ID"))
            {
                var columnName = new DataColumn("TransactionNumber") { DefaultValue = "" };
                ApiDt.Columns.Add(columnName);
            }
            else
            {
                ApiDt.Columns["ID"].ColumnName = "TransactionNumber";
            }

            if (!ApiDt.Columns.Contains("TransactionReference"))
            {
                var columnName = new DataColumn("TransactionReference") { DefaultValue = "" };
                ApiDt.Columns.Add(columnName);
            }
            if (!ApiDt.Columns.Contains("BatchTime"))
            {
                var columnName = new DataColumn("BatchTime") { DefaultValue = "" };
                ApiDt.Columns.Add(columnName);
            }
            if (!ApiDt.Columns.Contains("ChargeLines"))
            {
                var columnName = new DataColumn("ChargeLines") { DefaultValue = "" };
                ApiDt.Columns.Add(columnName);
            }
            if (!ApiDt.Columns.Contains("SalesInvoiceNo"))
            {
                var columnName = new DataColumn("SalesInvoiceNo") { DefaultValue = "" };
                ApiDt.Columns.Add(columnName);
            }

            ApiDt.Columns["Option5"].ColumnName = "ExternalDebtorCode";
            ApiDt.Columns["Branch_Code"].ColumnName = "Branch";
            ApiDt.Columns["LocalSystemTime"].ColumnName = "TransactionTime";
            ApiDt.Columns["Option6"].ColumnName = "Amount";
            ApiDt.Columns["Option3"].ColumnName = "CreatorEMail";
            ApiDt.Columns["Option4"].ColumnName = "EditorEMail";

            #endregion

            string rXML = CreateXML_Bollore(rVM, ApiDt);

            return rXML;

        }


        #endregion

        #region CallBack

        private void CallBack()
        {
        }

        #endregion

    }
}