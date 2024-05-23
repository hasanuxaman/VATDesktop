using SymphonySofttech.Reports;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using VATServer.Library;
using VATServer.Library.Integration;
using VATServer.Ordinary;
using VATViewModel.DTOs;

namespace VMSAPI
{
    public class SaleAPI
    {
        private string[] InsertSale(string saleXML, SysDBInfoVMTemp connVM = null)
        {
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";
            SaleDAL saleDal = new SaleDAL();
            DataTable dtSaleM = new DataTable();
            DataTable dtSaleD = new DataTable();
            DataTable dtSaleE = new DataTable();
            bool CommercialImporter = false;
            retResults = saleDal.ImportData(dtSaleM, dtSaleD, dtSaleE, CommercialImporter, 0, connVM);

            return retResults;
        }

        public string ImportSale(string saleXML, Action callBack, int branchId, string app = "", string UserId = "", SysDBInfoVMTemp connVM = null)
        {
            try
            {
                var dataSet = new DataSet();
                StringReader reader = new StringReader(saleXML);

                dataSet.ReadXml(reader);

                var table = dataSet.Tables[0];

                var saleDAL = new SaleDAL();


                var results = saleDAL.SaveAndProcess(table, callBack, branchId, app, connVM, null, null, null, UserId);


                var resultSet = new DataSet();

                table = new DataTable();
                table.Columns.Add("Result");
                table.Columns.Add("Message");

                var resultXML = "";

                var memoryStream = new MemoryStream();


                if (results[0].ToLower() == "success")
                {
                    table.Rows.Add("success", "Successfully Imported.");

                    resultSet.Tables.Add(table);

                    resultSet.WriteXml(memoryStream);

                    memoryStream.Seek(0, SeekOrigin.Begin);

                    var sr = new StreamReader(memoryStream);

                    resultXML = sr.ReadToEnd();

                }
                else
                {
                    table.Rows.Add("fail", "Sale Import Fail");

                    resultSet.Tables.Add(table);

                    resultSet.WriteXml(memoryStream);

                    memoryStream.Seek(0, SeekOrigin.Begin);

                    var sr = new StreamReader(memoryStream);

                    resultXML = sr.ReadToEnd();
                }

                return resultXML;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string ImportSale(string saleXML, string UserId = "", SysDBInfoVMTemp connVM = null)
        {
            try
            {
                var dataSet = new DataSet();
                StringReader reader = new StringReader(saleXML);
                BranchProfileDAL branchDAL = new BranchProfileDAL();

                dataSet.ReadXml(reader);

                DataTable table = dataSet.Tables[0];
                SaleDAL saleDAL = new SaleDAL();
                BergerIntegrationDAL _dal = new BergerIntegrationDAL();
                CommonDAL commonDal = new CommonDAL();
                string code = commonDal.settingValue("CompanyCode", "Code", connVM);

                if (code.ToLower() == "bfl")
                {
                    table = _dal.GetSaleDataXML(table, null, null, connVM);
                }

                if (code.ToLower() == "cp")
                {
                    if (table != null && table.Rows.Count > 0)
                    {
                        try
                        {

                            foreach (DataRow dr in table.Rows)
                            {
                                if (!string.IsNullOrWhiteSpace(dr["TransactionType"].ToString()) && dr["TransactionType"].ToString().Trim().ToLower() == "other")
                                {
                                    dr["Currency_Code"] = "BDT";
                                }
                            }
                            DataTable auditDt = table.Copy();
                            var ProcessDate = new DataColumn("ProcessDate") { DefaultValue = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") };

                            auditDt.Columns.Add(ProcessDate);

                            commonDal.BulkInsert("SalesAudit", auditDt, null, null, 10000, null, connVM);
                        }
                        catch (Exception ex)
                        {

                            #region Error log

                            try
                            {
                                ErrorLogVM evm = new ErrorLogVM();

                                evm.ImportId = "0";
                                evm.FileName = "SalesAudit";
                                evm.ErrorMassage = ex.Message;
                                evm.ErrorDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                evm.SourceName = "ImportSale";
                                evm.ActionName = "SalesAPI";
                                evm.TransactionName = "Sales";

                                CommonDAL _cDal = new CommonDAL();

                                string[] result = _cDal.InsertErrorLogs(evm, null, null, connVM);

                            }
                            catch (Exception e)
                            {

                            }

                            #endregion

                        }
                    }

                }

                table = OrdinaryVATDesktop.FormatSaleData(table);

                TableValidation("0", table);

                //DateTime tt = Convert.ToDateTime("2020-03-01T00:00:00+06:00");

                string transactiontype = table.Rows[0]["TransactionType"].ToString();

                foreach (DataRow row in table.Rows)
                {
                    row["Invoice_Date_Time"] = Convert.ToDateTime(row["Invoice_Date_Time"]).ToString("yyyy/MM/dd HH:mm:ss");
                }


                string[] results;
                results = saleDAL.SaveAndProcess(table, CallBack, 0, "", connVM, null, null, null, UserId);//transactiontype == "Other" ? saleDAL.SaveAndProcess(table, CallBack, 0) : saleDAL.SaveAndProcess_WithOutBulk(table, CallBack, 0);


                DataSet resultSet = new DataSet();

                DataTable resTable = new DataTable();
                resTable.Columns.Add("Result");
                resTable.Columns.Add("Message");
                resTable.Columns.Add("FileName");

                #region File Name Generation

                DataView dataView = new DataView(table);

                DataTable refs = dataView.ToTable(true, "Reference_No");

                string fileName = "";

                foreach (DataRow dataRow in refs.Rows)
                {
                    fileName += dataRow["Reference_No"] + ",";
                }

                fileName = fileName.TrimEnd(',');

                #endregion

                string resultXML = "";

                #region Convert to XML

                MemoryStream memoryStream = new MemoryStream();


                if (results[0].ToLower() == "success")
                {
                    resTable.Rows.Add("success", "Successfully Imported.", fileName);

                    resultSet.Tables.Add(resTable);

                    resultSet.WriteXml(memoryStream);

                    memoryStream.Seek(0, SeekOrigin.Begin);

                    StreamReader sr = new StreamReader(memoryStream);

                    resultXML = sr.ReadToEnd();

                }
                else
                {
                    resTable.Rows.Add("fail", "Sale Import Fail", fileName);

                    resultSet.Tables.Add(resTable);

                    resultSet.WriteXml(memoryStream);

                    memoryStream.Seek(0, SeekOrigin.Begin);

                    StreamReader sr = new StreamReader(memoryStream);

                    resultXML = sr.ReadToEnd();
                }


                #endregion

                return resultXML;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void TableValidation(string branchCode, DataTable table)
        {
            if (!table.Columns.Contains("Branch_Code"))
            {
                var columnName = new DataColumn("Branch_Code") { DefaultValue = branchCode };
                table.Columns.Add(columnName);
            }


            var column = new DataColumn("SL") { DefaultValue = "" };
            var CreatedBy = new DataColumn("CreatedBy") { DefaultValue = "API" };
            var ReturnId = new DataColumn("ReturnId") { DefaultValue = 0 };
            var BOMId = new DataColumn("BOMId") { DefaultValue = 0 };

            var CreatedOn = new DataColumn("CreatedOn") { DefaultValue = DateTime.Now.ToString() };

            table.Columns.Add(column);
            table.Columns.Add(CreatedBy);
            table.Columns.Add(CreatedOn);

            if (!table.Columns.Contains("ReturnId"))
            {
                table.Columns.Add(ReturnId);
            }

            table.Columns.Add(BOMId);

            foreach (DataRow row in table.Rows)
            {
                if (row["TransactionType"].ToString() == "Local Sale")
                {
                    row["TransactionType"] = "Other";
                }

                if (Convert.ToDecimal(row["VAT_Rate"]) == 5)
                {
                    row["Type"] = "OtherRate";
                }


            }
        }

        private void CallBack()
        {

        }

        #region Bollore API

        public ResultVM ImportSaleBollore(string saleXML, string UserId = "", SysDBInfoVMTemp connVM = null, SaleAPIVM_Bollore svm = null)
        {
            string[] results;
            ResultVM rVM = new ResultVM();
            DataTable refDt = new DataTable();
            DataTable ApiDt = new DataTable();
            string rXML = "";
            string fileName = "";

            try
            {
                BolloreIntegrationDAL _dal = new BolloreIntegrationDAL();
                var dataSet = new DataSet();

                ////DataTable table = _dal.GetSalesXMLData(saleXML);
                DataTable table = _dal.GetSaleData(saleXML, null, null, connVM);
                SaleDAL saleDAL = new SaleDAL();

                ////table = OrdinaryVATDesktop.FormatSaleData(table);

                TableValidation_Bollore(table, "ServiceNS");

                string transactiontype = table.Rows[0]["TransactionType"].ToString();

                foreach (DataRow row in table.Rows)
                {
                    row["Invoice_Date_Time"] = Convert.ToDateTime(row["Invoice_Date_Time"]).ToString("yyyy/MM/dd HH:mm:ss");

                    if (string.IsNullOrWhiteSpace(row["PreviousInvoiceDateTime"].ToString()))
                    {
                        row["PreviousInvoiceDateTime"] = "1900-01-01 00:00:00";
                    }
                }

                #region File Name Generation

                DataView dataView = new DataView(table);

                refDt = dataView.ToTable(true, "ID", "Branch_Code", "Option5", "Option3", "Option4", "Option6", "LocalSystemTime");

                foreach (DataRow dataRow in refDt.Rows)
                {
                    fileName += dataRow["ID"] + ",";
                }

                fileName = fileName.TrimEnd(',');

                rVM.FileName = fileName;

                #endregion

                table.Columns.Remove("LocalSystemTime");

                results = saleDAL.SaveAndProcess(table, CallBack, 0, "", connVM, null, null, null, UserId);

                rVM.Status = results[0];
                rVM.Message = results[1];

                rVM.XML = SalesResponse_Bollore(rVM, refDt);
                rVM.XML2 = getXML_Bollore(rVM, refDt);

                //if (rVM.Status == "Success")
                //{
                //    ApiDt = saleDAL.SelectAllForAPI_Bollore(refDt);
                //    rVM.XML = SalesResponse_Bollore(rVM, ApiDt);
                //    rVM.XML2 = CreateXML_Bollore(rVM, ApiDt);
                //}
                //else
                //{
                //    rVM.XML = getXML_Bollore(rVM, refDt);
                //    rVM.XML = SalesResponse_Bollore(rVM, ApiDt);
                //}

                return rVM;

            }
            catch (Exception ex)
            {
                rVM.Status = "Fail";
                rVM.Message = ex.Message;

                rVM.XML2 = getXML_Bollore(rVM, refDt);
                rVM.XML = SalesResponse_Bollore(rVM, refDt);

                try
                {
                    ErrorLogVM evm = new ErrorLogVM();

                    evm.ImportId = fileName;
                    evm.FileName = fileName;
                    evm.ErrorMassage = ex.Message;
                    evm.ErrorDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    evm.SourceName = "ImportSaleBollore";
                    evm.ActionName = "SaleAPI";
                    evm.TransactionName = "Sales";

                    CommonDAL _cDal = new CommonDAL();

                    string[] result = _cDal.InsertErrorLogs(evm, null, null, connVM);

                }
                catch (Exception e)
                {

                }

                return rVM;
            }
        }

        private void TableValidation_Bollore(DataTable salesData, string TType)
        {
            ////if (!salesData.Columns.Contains("Branch_Code"))
            ////{
            ////    var columnName = new DataColumn("Branch_Code") { DefaultValue = Program.BranchCode };
            ////    salesData.Columns.Add(columnName);
            ////}

            var column = new DataColumn("SL") { DefaultValue = "" };
            var CreatedBy = new DataColumn("CreatedBy") { DefaultValue = "API" };
            var ReturnId = new DataColumn("ReturnId") { DefaultValue = 0 };
            var BOMId = new DataColumn("BOMId") { DefaultValue = 0 };
            var TransactionType = new DataColumn("TransactionType") { DefaultValue = TType };
            var CreatedOn = new DataColumn("CreatedOn") { DefaultValue = DateTime.Now.ToString() };

            salesData.Columns.Add(column);
            salesData.Columns.Add(CreatedBy);
            salesData.Columns.Add(CreatedOn);

            if (!salesData.Columns.Contains("ReturnId"))
            {
                salesData.Columns.Add(ReturnId);
            }
            if (!salesData.Columns.Contains("TransactionType"))
            {
                salesData.Columns.Add(TransactionType);
            }

            salesData.Columns.Add(BOMId);
        }

        private string SalesResponse_Bollore(ResultVM rVM, DataTable dt, SaleAPIVM_Bollore svm = null)
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

        private string CreateXML_Bollore(ResultVM rVM, DataTable dt, SaleAPIVM_Bollore svm = null)
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

            ////////// Create the master element
            //XmlElement masterElement = xmlDocument.CreateElement("InvoiceStatus");
            //rootElement.AppendChild(masterElement);
            ////////// Set the status and message attributes of the master element
            //masterElement.SetAttribute("Status", rVM.Status);
            //masterElement.SetAttribute("Message", rVM.Message);

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

        private string getXML_Bollore(ResultVM rVM, DataTable dt, SaleAPIVM_Bollore svm = null)
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

        #region British Council

        public ResultVM ImportSaleBritishCouncil(string saleXML, string UserId = "", SysDBInfoVMTemp connVM = null, string pathRoot = "")
        {
            string[] results;
            DataTable refDt = new DataTable();
            ResultVM rVM = new ResultVM();
            DataTable ApiDt = new DataTable();
            DataTable rdt = new DataTable();
            BritishCouncil_IntegrationDAL _dal = new BritishCouncil_IntegrationDAL();

            string fileName = "";
            string GetNumber = "";

            try
            {
                SaleDAL saleDAL = new SaleDAL();
                BranchProfileDAL branchDAL = new BranchProfileDAL();
                CommonDAL commonDal = new CommonDAL();

                GetNumber = DateTime.Now.ToString("yyyyMMddHHmmss");

                DataTable table = _dal.GetSaleData(saleXML, null, null, connVM);

                #region Rename Column

                ////if (table.Constraints("FileID"))
                ////{
                ////    table.Columns["FileID"].ColumnName = "Option3";
                ////}

                ////if (table.Constraints("LineComments"))
                ////{
                ////    table.Columns["LineComments"].ColumnName = "CommentsD";
                ////}

                #endregion

                string transactiontype = table.Rows[0]["TransactionType"].ToString();

                #region File Name Generation

                DataView dataView = new DataView(table);

                refDt = dataView.ToTable(true, "ID");

                foreach (DataRow dataRow in refDt.Rows)
                {
                    fileName += dataRow["ID"] + ",";
                }

                fileName = fileName.TrimEnd(',');

                rVM.FileName = fileName;

                #endregion

                #region Response File Name

                string FileID = table.Rows[0]["Option3"].ToString();

                rVM.ResponseFileName = "Sales" + FileID + "_" + GetNumber;

                #endregion

                #region response dt

                dataView = new DataView(table);

                rdt = dataView.ToTable(true, "Option3", "ID", "Customer_Name", "Invoice_Date_Time");

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
                            TableValidation("001", finaldata);

                            results = saleDAL.SaveAndProcess(finaldata, CallBack, 0, "", connVM, null, null, null, UserId);

                            if (results[0].ToLower() == "success")
                            {
                                ////rVM = PDFSaleBritishCouncil(ID, pathRoot, connVM);

                                DataView dv = new DataView(finaldata);
                                DataTable ids = dv.ToTable(true, "ID");
                                DataTable getDt = _dal.SelectSalesDataForAPI(ids, null, null, connVM);

                                string[] result = _dal.SaveAPIData(getDt, "", "", false, GetNumber, null, null, connVM);
                            }
                            else
                            {
                                DataView dv = new DataView(finaldata.Copy());
                                DataTable apiData = dv.ToTable(true, "ID", "Option3", "Invoice_Date_Time", "Customer_Name");

                                string message = results[1].ToString();
                                message = message.Replace("\r\n", " ");

                                string[] result = _dal.SaveAPIData(apiData, results[0].ToString(), message, false, GetNumber, null, null, connVM);

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
                            DataTable apiData = dv.ToTable(true, "ID", "Invoice_Date_Time", "Customer_Name", "Option3");

                            string[] reresult = _dal.SaveAPIData(apiData, "Fail", message, false, GetNumber, null, null, connVM);

                            #endregion

                        }

                    }
                }

                rVM.XML = CreateXMLAPI_BC(refDt, GetNumber, connVM);

                #region comments

                ////results = saleDAL.SaveAndProcess(table, CallBack, 0, "", null, null, null, null, UserId);//transactiontype == "Other" ? saleDAL.SaveAndProcess(table, CallBack, 0) : saleDAL.SaveAndProcess_WithOutBulk(table, CallBack, 0);

                ////////rVM.Status = results[0];
                ////////rVM.Message = results[1];

                ////////if (rVM.Status == "Success")
                ////////{
                ////////    ApiDt = saleDAL.SelectAllForAPI(refDt);
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
                    evm.SourceName = "ImportSaleBritishCouncil";
                    evm.ActionName = "SalesAPI";
                    evm.TransactionName = "Sales";

                    CommonDAL _cDal = new CommonDAL();

                    string[] result = _cDal.InsertErrorLogs(evm, null, null, connVM);

                }
                catch (Exception e)
                {

                }

                #endregion

                #region CreateXMLAPI

                string[] reresult = _dal.SaveAPIData(rdt, rVM.Status, rVM.Message, false, GetNumber, null, null, connVM);

                rVM.XML = CreateXMLAPI_BC(rdt, GetNumber, connVM);

                #endregion

                return rVM;
            }
        }

        private string CreateXMLAPI_BC(DataTable dt, string GetNumber, SysDBInfoVMTemp connVM = null)
        {
            BritishCouncil_IntegrationDAL _dal = new BritishCouncil_IntegrationDAL();

            DataTable ApiDt = _dal.SelectResponseDataForAPI(dt, GetNumber, null, null, connVM);

            string xmlString = CreateXML_BC(ApiDt);

            return xmlString;

        }

        private string CreateXML_BC(DataTable dt)
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

                XmlElement customerElement = xmlDocument.CreateElement("Customer");
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

        #endregion

        private string CreateXML(ResultVM rVM, DataTable dt)
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

            XmlElement StatusElement = xmlDocument.CreateElement("Status");
            StatusElement.InnerText = rVM.Status;
            rootElement.AppendChild(StatusElement);

            XmlElement MessageElement = xmlDocument.CreateElement("Message");
            MessageElement.InnerText = rVM.Message;
            rootElement.AppendChild(MessageElement);

            XmlElement InvoiceDetailsElement = xmlDocument.CreateElement("InvoiceDetails");
            rootElement.AppendChild(InvoiceDetailsElement);

            // Create the detail elements
            foreach (DataRow row in dt.Rows)
            {
                XmlElement detailElement = xmlDocument.CreateElement("Invoice");
                InvoiceDetailsElement.AppendChild(detailElement);

                // Create and set the detail element values
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

                XmlElement invoiceNoElement = xmlDocument.CreateElement("InvoiceNo");
                invoiceNoElement.InnerText = row["SalesInvoiceNo"].ToString();
                detailElement.AppendChild(invoiceNoElement);

                XmlElement InvoicedateElement = xmlDocument.CreateElement("InvoiceDateTime");
                InvoicedateElement.InnerText = row["InvoiceDateTime"].ToString();
                detailElement.AppendChild(InvoicedateElement);

                ////XmlElement FileId = xmlDocument.CreateElement("FileId");
                ////FileId.InnerText = row["FileId"].ToString();
                ////detailElement.AppendChild(FileId);

                XmlElement customerElement = xmlDocument.CreateElement("Customer");
                customerElement.InnerText = row["CustomerName"].ToString();
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

        private string getXML(ResultVM rVM, DataTable dt)
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

            if (!ApiDt.Columns.Contains("SalesInvoiceNo"))
            {
                var columnName = new DataColumn("SalesInvoiceNo") { DefaultValue = "" };
                ApiDt.Columns.Add(columnName);
            }
            if (!ApiDt.Columns.Contains("Invoice_Date_Time"))
            {
                var columnName = new DataColumn("InvoiceDateTime") { DefaultValue = "" };
                ApiDt.Columns.Add(columnName);
            }
            else
            {
                ApiDt.Columns["Invoice_Date_Time"].ColumnName = "InvoiceDateTime";
            }

            if (!ApiDt.Columns.Contains("Customer_Name"))
            {
                var columnName = new DataColumn("CustomerName") { DefaultValue = "" };
                ApiDt.Columns.Add(columnName);
            }
            else
            {
                ApiDt.Columns["Customer_Name"].ColumnName = "CustomerName";
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
            if (!ApiDt.Columns.Contains("Option3"))
            {
                var columnName = new DataColumn("FileId") { DefaultValue = "" };
                ApiDt.Columns.Add(columnName);
            }
            else
            {
                ApiDt.Columns["Option3"].ColumnName = "FileId";
            }

            #endregion

            string rXML = CreateXML(rVM, ApiDt);

            return rXML;

        }

        public string ErrorLogs(ErrorLogVM vm, SysDBInfoVMTemp connVM = null)
        {
            CommonDAL _cdal = new CommonDAL();

            try
            {
                _cdal.InsertErrorLogs(vm, null, null, connVM);

                return "Success";

            }
            catch (Exception ex)
            {
                return "Fail";
            }
        }

        public ResultVM PDFSaleBritishCouncil(string ImportId, string pathRoot, SysDBInfoVMTemp connVM = null)
        {
            string[] results;
            DataTable refDt = new DataTable();
            ResultVM rVM = new ResultVM();

            string fileName = "";
            string GetNumber = "";

            try
            {
                //string pathRoot = AppDomain.CurrentDomain.BaseDirectory;
                OrdinaryVATDesktop.IsWCF = "N";
                SaleReport _DAL = new SaleReport();

                string result = _DAL.SalesPDFExport(ImportId, pathRoot, connVM);

                rVM.Status = result;
                rVM.Message = "PDF Generation and Mail send successfull. ";

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
                    evm.SourceName = "ImportSaleBritishCouncil";
                    evm.ActionName = "SalesAPI";
                    evm.TransactionName = "Sales";

                    CommonDAL _cDal = new CommonDAL();

                    string[] result = _cDal.InsertErrorLogs(evm, null, null, connVM);

                }
                catch (Exception e)
                {

                }

                #endregion

                return rVM;
            }
        }



    }
}
