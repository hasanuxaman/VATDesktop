using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.Office.Interop.Excel;
using OfficeOpenXml.FormulaParsing.Excel.Functions.RefAndLookup;
using Oracle.DataAccess.Client;
using VATViewModel.DTOs;
using DataTable = System.Data.DataTable;

namespace VATServer.Library.Integration
{
    public class NourishIntegrationDAL
    {
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        #region Sales

        public DataTable GetSaleMaster(IntegrationParam param, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";

            string insertSQLText = "";
            int Id = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Insert"; //Method Name

            OracleConnection currConn = null;
            OracleTransaction transaction = null;
            #endregion

            #region try

            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetOracleConnection(param.dtConnectionInfo);

                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                transaction = currConn.BeginTransaction();

                #endregion open connection and transaction


                #region DB Select

                string dbName = DatabaseInfoVM.DatabaseName;


                sqlText = @"
select distinct
ID

,'-'CustomerName 
, Customer_Code
,'-' Delivery_Address
, Invoice_Date_Time
, Branch_Code

from vat_invoice

where 1=1 
and ID not in (select REFNO from VAT_SALES_COMPLETED)

";

                if (!string.IsNullOrEmpty(param.RefNo))
                {
                    sqlText += " and ID = :ID";
                }


                if (param.FromDate != "1900-01-01" || param.ToDate != "9000-12-31")
                {
                    sqlText += @" and Invoice_Date_Time BETWEEN  :fromDate
                                AND   :toDate";
                }


                sqlText += @" and Branch_Code = :Branch_Code";
                sqlText += @" and Company_Code = :Company_Code";

                #endregion

                DataTable dtTable = new DataTable();

                OracleCommand command = new OracleCommand(sqlText, currConn);
                command.Transaction = transaction;


                OracleDataAdapter dataAdapter = new OracleDataAdapter(command);

                if (!string.IsNullOrEmpty(param.RefNo))
                {
                    command.Parameters.Add(new OracleParameter("ID",
                        param.RefNo));
                }


                if (param.FromDate != "1900-01-01" || param.ToDate != "9000-12-31")
                {
                    command.Parameters.Add(new OracleParameter("fromDate",
                        Convert.ToDateTime(param.FromDate).ToString("dd-MMM-yyyy")));

                    command.Parameters.Add(new OracleParameter("toDate",
                        Convert.ToDateTime(param.ToDate).ToString("dd-MMM-yyyy")));
                }


                command.Parameters.Add(new OracleParameter("Branch_Code",
                    param.dtConnectionInfo.Rows[0]["IntegrationCode"]));

                command.Parameters.Add(new OracleParameter("Company_Code",
                    param.CompanyCode));


                dataAdapter.Fill(dtTable);

                #region Commit

                transaction.Commit();

                #endregion Commit

                FileLogger.Log("ImportDAL", "GetSale", sqlText + "\n" + param.dtConnectionInfo.Rows[0]["IntegrationCode"] + "\n" + param.CompanyCode);

                return dtTable;

            }
            #endregion

            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message; //catch ex
                if (transaction != null) { transaction.Rollback(); }

                FileLogger.Log("ImportDAL", "GetSale", ex + "\n" + sqlText);

                throw ex;
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
            #endregion
        }

        public DataTable GetDetails(IntegrationParam param)
        {
            string sqlText;
            OracleConnection currConn = null;
            OracleTransaction transaction = null;
            #region DB Select

            string dbName = DatabaseInfoVM.DatabaseName;


            sqlText = @"
  SELECT 
vt.ID as ""ID""
,nvl(vcf.Name,'-') as ""Customer_Name""
, Customer_Code as ""Customer_Code""
,vcf.ADDRESS as ""Delivery_Address"" 
, vt.BRANCH_CODE as ""Branch_Code""
, Invoice_Date_Time as ""Invoice_Date_Time""
,'N'  as ""Post""
,Item_Code as ""Item_Code""
,Item_Name as ""Item_Name""
,Quantity as ""Quantity""
,NBR_Price as ""NBR_Price""
,vi.Unit as ""UOM""
,0 as ""VAT_Rate""
,0 as ""SD_Rate""
,'N' as ""Non_Stock""
,'0' as ""Trading_MarkUp""
,0 as ""Discount_Amount""
,0 as ""Promotional_Quantity""
,'VAT 4.3' as ""VAT_Name""
,'NA' as ""LC_Number""
,'BDT' as ""Currency_Code""
,'NEW' as ""Sale_Type""
,'' as ""Previous_Invoice_No""
,'N' as ""Is_Print""
,'0' as ""Tender_Id""
,vt.ID as ""Reference_No""
,0 as ""SubTotal""
,'' as ""Type""
, Vehicle_No  as ""Vehicle_No""
,'NA' as ""VehicleType""
,Invoice_Date_Time as ""PreviousInvoiceDateTime""
,0 as ""PreviousNBRPrice""
,0 as ""PreviousQuantity""
,'' as ""PreviousUOM""
,0 as ""PreviousSubTotal""
,0 as ""PreviousVATAmount""
,0 as ""PreviousVATRate""
,0 as ""PreviousSD""
,0 as ""PreviousSDAmount""
,'' as ""ReasonOfReturn""
from

VAT_INVOICE vt left outer join VAT_Item vi on

vt.ITEM_CODE = (vi.Type_CD || vi.Cat_CD ||vi.Item_CD)
--left outer join INVFEED.VAT_CLIENT_FEED vcf
left outer join VAT_CLIENT_FEED vcf
on vt.Customer_Code = vcf.ID

where 1=1
and vt.ID not in (select REFNO from VAT_SALES_COMPLETED)




";


            if (param.IDs.Any())
            {
                sqlText += " and vt.ID in (";
                sqlText += "'" + string.Join("','", param.IDs) + "'";
                sqlText += ")";
            }


            sqlText += @" and Branch_Code = :Branch_Code";
            sqlText += @" and Company_Code = :Company_Code";

            #endregion

            DataTable dtTable = new DataTable();

            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetOracleConnection(param.dtConnectionInfo);

                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                transaction = currConn.BeginTransaction();

                #endregion open connection and transaction

                OracleCommand command = new OracleCommand(sqlText, currConn);
                command.Transaction = transaction;


                OracleDataAdapter dataAdapter = new OracleDataAdapter(command);

                if (!string.IsNullOrEmpty(param.RefNo))
                {
                    command.Parameters.Add(new OracleParameter("ID",
                        param.RefNo));
                }


                command.Parameters.Add(new OracleParameter("Branch_Code",
                    param.dtConnectionInfo.Rows[0]["IntegrationCode"]));

                command.Parameters.Add(new OracleParameter("Company_Code",
                    param.CompanyCode));

                dataAdapter.Fill(dtTable);

                #region Commit

                transaction.Commit();

                #endregion Commit
            }
            catch (Exception e)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }

                FileLogger.Log("ImportDAL", "GetSale", e + "\n" + sqlText);
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

            return dtTable;
        }

        public ResultVM SaveSale(IntegrationParam param, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";

            string insertSQLText = "";
            int Id = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Insert"; //Method Name

            OracleConnection currConn = null;
            OracleTransaction transaction = null;
            #endregion

            #region try

            try
            {

                // Get Details
                var dtTable = GetDetails(param);
                TableValidation(dtTable, param);

                SaleDAL saleDal = new SaleDAL();

                retResults = saleDal.SaveAndProcess(dtTable, () => { }, Convert.ToInt32(param.BranchId), "", connVM, param,
                    null, null, param.CurrentUserId);

                try
                {
                    currConn = _dbsqlConnection.GetOracleConnection(param.dtConnectionInfo);

                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }

                    transaction = currConn.BeginTransaction();

                    var completedData = dtTable.DefaultView.ToTable(true, "ID");

                    completedData.Columns.Add(new DataColumn() { ColumnName = "UNIT_CD", DefaultValue = param.BranchCode });
                    completedData.Columns.Add(new DataColumn() { ColumnName = "COM_CD", DefaultValue = param.CompanyCode });
                    completedData.Columns["ID"].ColumnName = "REFNO";

                    //REFNO

                    CommonDAL commonDal = new CommonDAL();

                    commonDal.BulkInsertOracle("VAT_SALES_COMPLETED", completedData, null, currConn, transaction);


                    transaction.Commit();


                }
                catch (Exception e)
                {
                    if (transaction != null) { transaction.Rollback(); }
                    FileLogger.Log("ImportDAL", "GetSale", e + "\n" + sqlText);

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

                ResultVM resultVm = new ResultVM();
                resultVm.Status = retResults[0];
                resultVm.Message = retResults[1];


                return resultVm;
            }
            #endregion

            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message; //catch ex

                FileLogger.Log("ImportDAL", "GetSale", ex + "\n" + sqlText);

                throw ex;
            }

            #endregion
        }

        private void TableValidation(DataTable salesData, IntegrationParam param)
        {
            if (!salesData.Columns.Contains("Branch_Code"))
            {
                var columnName = new DataColumn("Branch_Code") { DefaultValue = param.BranchCode };
                salesData.Columns.Add(columnName);
            }

            var column = new DataColumn("SL") { DefaultValue = "" };
            var CreatedBy = new DataColumn("CreatedBy") { DefaultValue = param.CurrentUser };
            var ReturnId = new DataColumn("ReturnId") { DefaultValue = 0 };
            var BOMId = new DataColumn("BOMId") { DefaultValue = 0 };
            var TransactionType = new DataColumn("TransactionType") { DefaultValue = param.TransactionType };
            var CreatedOn = new DataColumn("CreatedOn") { DefaultValue = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") };

            salesData.Columns.Add(column);
            salesData.Columns.Add(CreatedBy);
            salesData.Columns.Add(CreatedOn);

            if (!salesData.Columns.Contains("ReturnId"))
            {
                salesData.Columns.Add(ReturnId);
            }

            salesData.Columns.Add(BOMId);
            salesData.Columns.Add(TransactionType);
        }

        #endregion

        #region Purchase

        public DataTable GetPurchaseMaster(IntegrationParam param, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";

            string insertSQLText = "";
            int Id = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Insert"; //Method Name

            OracleConnection currConn = null;
            OracleTransaction transaction = null;
            #endregion

            #region try

            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetOracleNourishConnection(param.dtConnectionInfo);

                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                transaction = currConn.BeginTransaction();

                #endregion open connection and transaction


                #region DB Select

                string dbName = DatabaseInfoVM.DatabaseName;


                sqlText = @"
select distinct
0   Selected
,ID 
,vendor_code 
,supplier.supname vendor_name
,BranchCode 
,Receive_Date

from grn_vw left outer join supplier 
on grn_vw.vendor_code = supplier.supid
where 1=1 and ID not in (select REFNO from VAT_PURCHASE_COMPLETED)

";

                if (!string.IsNullOrEmpty(param.RefNo))
                {
                    sqlText += " and ID = :ID";
                }


                if (param.FromDate != "1900-01-01" || param.ToDate != "9000-12-31")
                {
                    sqlText += @" and Receive_Date BETWEEN  :fromDate
                                AND   :toDate";
                }


                //sqlText += @" and BranchCode = :Branch_Code";
                sqlText += @" and CompanyCode = :Company_Code";

                #endregion

                DataTable dtTable = new DataTable();

                OracleCommand command = new OracleCommand(sqlText, currConn);
                command.Transaction = transaction;

                OracleDataAdapter dataAdapter = new OracleDataAdapter(command);

                if (!string.IsNullOrEmpty(param.RefNo))
                {
                    command.Parameters.Add(new OracleParameter("ID",
                        param.RefNo));
                }

                if (param.FromDate != "1900-01-01" || param.ToDate != "9000-12-31")
                {
                    command.Parameters.Add(new OracleParameter("fromDate",
                        Convert.ToDateTime(param.FromDate).ToString("dd-MMM-yyyy")));

                    command.Parameters.Add(new OracleParameter("toDate",
                        Convert.ToDateTime(param.ToDate).ToString("dd-MMM-yyyy")));
                }

                //command.Parameters.Add(new OracleParameter("BranchCode",
                //    param.dtConnectionInfo.Rows[0]["IntegrationCode"]));

                command.Parameters.Add(new OracleParameter("CompanyCode",
                    param.CompanyCode));


                dataAdapter.Fill(dtTable);

                #region Commit

                transaction.Commit();

                #endregion Commit

                FileLogger.Log("ImportDAL", "GetSale",
                    param.dtConnectionInfo.Rows[0]["IntegrationCode"] + "\n" + param.CompanyCode + "\n" + sqlText + "\n " + dtTable.Rows.Count + "\n" + Convert.ToDateTime(param.FromDate).ToString("dd-MMM-yyyy") + "\n" + Convert.ToDateTime(param.ToDate).ToString("dd-MMM-yyyy") + "\n" + param.CompanyCode);

                return dtTable;

            }
            #endregion

            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message; //catch ex
                if (transaction != null) { transaction.Rollback(); }

                FileLogger.Log("ImportDAL", "GetSale", ex + "\n" + sqlText);

                throw ex;
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
            #endregion
        }

        public DataTable GetPurchaseDetails(IntegrationParam param)
        {
            string sqlText = "";
            OracleConnection currConn = null;
            OracleTransaction transaction = null;

            DataTable dtTable = new DataTable();

            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetOracleNourishConnection(param.dtConnectionInfo);

                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                transaction = currConn.BeginTransaction();

                #endregion open connection and transaction

                #region DB Select

                string dbName = DatabaseInfoVM.DatabaseName;


                sqlText = @"
 SELECT 
       ID
      ,Vendor_Code  ""Vendor_Code""
      ,BranchCode ""BranchCode""
      ,sp.supname ""Vendor_Name""
      ,'-'  ""BE_Number""
      ,Invoice_Date  ""Invoice_Date""
      ,VAT_Amount  ""VAT_Amount""
      ,ID  ""Referance_No""
      ,'-'  ""LC_No""
      ,Receive_Date  ""Receive_Date""
      ,ProductCode  ""Item_Code""
      ,'-' ""Item_Name""
      ,Quantity  ""Quantity""
      ,  Total_Price  ""Total_Price""
      ,'KG'  ""UOM""
      ,(case when VAT_Rate = 15 then 'VAT' when VAT_Rate = 0 then 'Exempted' else 'OtherRate'end) ""Type""
      ,0  ""SD_Amount""
      ,0  ""Assessable_Value""
      ,0  ""CD_Amount""
      ,0  ""RD_Amount""
      ,0  ""AT_Amount""
      ,0  ""AITAmount""
      ,0  ""Others_Amount""
      ,'-'  ""Remarks""
      ,'N'  ""Post""
      ,'N'  ""With_VDS""
      ,'-'   ""Comments""
, '0' ""Rebate_Rate""
--,  VAT_Rate  ""VAT_Rate""
  FROM grn_vw gv left outer join supplier sp on gv.Vendor_Code = sp.supid
where 1=1 and ID not in (select RefNo from VAT_Purchase_Completed)


";

                if (param.IDs.Any())
                {
                    sqlText += " and gv.ID in (";
                    sqlText += "'" + string.Join("','", param.IDs) + "'";
                    sqlText += ")";
                }

                ////sqlText += @" and BranchCode = :Branch_Code";
                sqlText += @" and CompanyCode = :Company_Code";

                #endregion

                OracleCommand command = new OracleCommand(sqlText, currConn);
                command.Transaction = transaction;

                OracleDataAdapter dataAdapter = new OracleDataAdapter(command);

                if (!string.IsNullOrEmpty(param.RefNo))
                {
                    command.Parameters.Add(new OracleParameter("ID",
                        param.RefNo));
                }


                ////command.Parameters.Add(new OracleParameter("BranchCode",
                ////    param.dtConnectionInfo.Rows[0]["IntegrationCode"]));

                command.Parameters.Add(new OracleParameter("CompanyCode",
                    param.CompanyCode));

                dataAdapter.Fill(dtTable);

                #region Commit

                transaction.Commit();

                #endregion Commit


                //FileLogger.LogWeb("ImportDAL", "Get Purchase Detail", sqlText + "  \n" + param.dtConnectionInfo.Rows[0]["IntegrationCode"] + "  \n" + param.IDs + "  \n" + param.CompanyCode + "  \n" + param.RefNo + dtTable);

            }
            catch (Exception e)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }

                FileLogger.Log("ImportDAL", "Get Purchase Detail", e + "\n" + sqlText);
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

            return dtTable;
        }

        public ResultVM SavePurchase(IntegrationParam param, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";

            string insertSQLText = "";
            int Id = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Insert"; //Method Name

            OracleConnection currConn = null;
            OracleTransaction transaction = null;
            #endregion

            #region try

            try
            {

                // Get Details
                var dtTable = GetPurchaseDetails(param);

                FileLogger.Log("NourishIntegrationDAL", "SavePurchase", "Total Data count : " + dtTable.Rows.Count.ToString());

                PurchaseDAL purchaseDal = new PurchaseDAL();

                retResults = purchaseDal.SaveTempPurchase(dtTable, param.BranchCode, param.TransactionType,
                    param.CurrentUser,
                    Convert.ToInt32(param.BranchId),
                    () => { }, null,
                    null, connVM, param.CurrentUserId, true);

                try
                {
                    currConn = _dbsqlConnection.GetOracleNourishConnection(param.dtConnectionInfo);

                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }

                    transaction = currConn.BeginTransaction();

                    var completedData = dtTable.DefaultView.ToTable(true, "ID");

                    completedData.Columns.Add(new DataColumn() { ColumnName = "UNIT_CD", DefaultValue = param.BranchCode });
                    completedData.Columns.Add(new DataColumn() { ColumnName = "COM_CD", DefaultValue = param.CompanyCode });
                    completedData.Columns["ID"].ColumnName = "REFNO";

                    //REFNO

                    CommonDAL commonDal = new CommonDAL();

                    commonDal.BulkInsertOracle("VAT_PURCHASE_COMPLETED", completedData, null, currConn, transaction);


                    transaction.Commit();


                }
                catch (Exception e)
                {
                    if (transaction != null) { transaction.Rollback(); }
                    FileLogger.Log("NourishDAL", "Get Purchase", e.ToString());

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


                ResultVM resultVm = new ResultVM();
                resultVm.Status = retResults[0];
                resultVm.Message = retResults[1];


                return resultVm;
            }
            #endregion

            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message; //catch ex

                FileLogger.Log("NourishDAL", "Get Purchase", ex.ToString());


                throw ex;
            }

            #endregion
        }

        #endregion

        #region Issue

        public DataTable GetIssueMaster(IntegrationParam param, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";

            string insertSQLText = "";
            int Id = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Insert"; //Method Name

            OracleConnection currConn = null;
            OracleTransaction transaction = null;
            #endregion

            #region try

            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetOracleNourishConnection(param.dtConnectionInfo);

                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                transaction = currConn.BeginTransaction();

                #endregion open connection and transaction

                #region DB Select

                string dbName = DatabaseInfoVM.DatabaseName;


                sqlText = @"
select distinct
0   Selected
,ID 
,'001' BranchCode 
,Issue_Date

from issue_vw 
where 1=1 and ID not in (select REFNO from VAT_Issue_COMPLETED)

";

                if (!string.IsNullOrEmpty(param.RefNo))
                {
                    sqlText += " and ID = :ID";
                }


                if (param.FromDate != "1900-01-01" || param.ToDate != "9000-12-31")
                {
                    sqlText += @" and Issue_Date BETWEEN  :fromDate
                                AND   :toDate";
                }


                ////sqlText += @" and BranchCode = :Branch_Code";
                sqlText += @" and CompanyCode = :Company_Code";

                #endregion

                DataTable dtTable = new DataTable();

                OracleCommand command = new OracleCommand(sqlText, currConn);
                command.Transaction = transaction;


                OracleDataAdapter dataAdapter = new OracleDataAdapter(command);

                if (!string.IsNullOrEmpty(param.RefNo))
                {
                    command.Parameters.Add(new OracleParameter("ID",
                        param.RefNo));
                }


                if (param.FromDate != "1900-01-01" || param.ToDate != "9000-12-31")
                {
                    command.Parameters.Add(new OracleParameter("fromDate",
                        Convert.ToDateTime(param.FromDate).ToString("dd-MMM-yyyy")));

                    command.Parameters.Add(new OracleParameter("toDate",
                        Convert.ToDateTime(param.ToDate).ToString("dd-MMM-yyyy")));
                }


                ////command.Parameters.Add(new OracleParameter("BranchCode",
                ////    param.dtConnectionInfo.Rows[0]["IntegrationCode"]));

                command.Parameters.Add(new OracleParameter("CompanyCode",
                    param.CompanyCode));


                dataAdapter.Fill(dtTable);

                #region Commit

                transaction.Commit();

                #endregion Commit

                //FileLogger.Log("ImportDAL", "GetSale",
                //    param.dtConnectionInfo.Rows[0]["IntegrationCode"] + "\n" + param.CompanyCode + "\n" + sqlText + "\n " + dtTable.Rows.Count);

                return dtTable;

            }
            #endregion

            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message; //catch ex
                if (transaction != null) { transaction.Rollback(); }

                FileLogger.Log("ImportDAL", "GetSale", ex + "\n" + sqlText);

                throw ex;
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
            #endregion
        }

        public DataTable GetIssueDetails(IntegrationParam param)
        {
            string sqlText="";
            OracleConnection currConn = null;
            OracleTransaction transaction = null;
            
            DataTable dtTable = new DataTable();

            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetOracleNourishConnection(param.dtConnectionInfo);

                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                transaction = currConn.BeginTransaction();

                #endregion open connection and transaction

                #region DB Select

                string dbName = DatabaseInfoVM.DatabaseName;

                sqlText = @"
 SELECT 
      ID
      ,'001' ""BranchCode""
      ,Issue_Date ""Issue_DateTime""
      ,ProductCode ""Item_Code""
      ,'-' ""Item_Name""
      ,Quantity ""Quantity""
      ,'KG' UOM
      ,ID ""Reference_No""
      ,'-' ""Comments""
      ,'N' ""Post""

  FROM issue_vw iv
where 1=1 and ID not in (select REFNO from VAT_Issue_COMPLETED)

";

                if (param.IDs.Any())
                {
                    sqlText += " and iv.ID in (";
                    sqlText += "'" + string.Join("','", param.IDs) + "'";
                    sqlText += ")";
                }


                ////sqlText += @" and BranchCode = :Branch_Code";
                sqlText += @" and CompanyCode = :Company_Code";

                #endregion

                OracleCommand command = new OracleCommand(sqlText, currConn);
                command.Transaction = transaction;


                OracleDataAdapter dataAdapter = new OracleDataAdapter(command);

                if (!string.IsNullOrEmpty(param.RefNo))
                {
                    command.Parameters.Add(new OracleParameter("ID",
                        param.RefNo));
                }


                ////command.Parameters.Add(new OracleParameter("BranchCode",
                ////    param.dtConnectionInfo.Rows[0]["IntegrationCode"]));

                command.Parameters.Add(new OracleParameter("CompanyCode",
                    param.CompanyCode));

                dataAdapter.Fill(dtTable);

                #region Commit

                transaction.Commit();

                #endregion Commit


                FileLogger.Log("ImportDAL", "Get Purchase Detail", sqlText + "  \n" + param.dtConnectionInfo.Rows[0]["IntegrationCode"]);

            }
            catch (Exception e)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }

                FileLogger.Log("ImportDAL", "Get Purchase Detail", e + "\n" + sqlText);
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

            return dtTable;
        }

        public ResultVM SaveIssue(IntegrationParam param, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";

            string insertSQLText = "";
            int Id = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Insert"; //Method Name

            OracleConnection currConn = null;
            OracleTransaction transaction = null;
            #endregion

            #region try

            try
            {

                // Get Details
                var dtTable = GetIssueDetails(param);

                IssueDAL issueDal = new IssueDAL();

                retResults = issueDal.SaveTempIssue(dtTable, param.TransactionType, param.CurrentUser,
                    Convert.ToInt32(param.BranchId), () => { }, null, null, null, param.BranchCode);

                try
                {
                    currConn = _dbsqlConnection.GetOracleNourishConnection(param.dtConnectionInfo);

                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }

                    transaction = currConn.BeginTransaction();

                    var completedData = dtTable.DefaultView.ToTable(true, "ID");

                    completedData.Columns.Add(new DataColumn() { ColumnName = "UNIT_CD", DefaultValue = param.BranchCode });
                    completedData.Columns.Add(new DataColumn() { ColumnName = "COM_CD", DefaultValue = param.CompanyCode });
                    completedData.Columns["ID"].ColumnName = "REFNO";

                    //REFNO

                    CommonDAL commonDal = new CommonDAL();

                    commonDal.BulkInsertOracle("VAT_ISSUE_COMPLETED", completedData, null, currConn, transaction);


                    transaction.Commit();


                }
                catch (Exception e)
                {
                    if (transaction != null) { transaction.Rollback(); }
                    FileLogger.Log("NourishDAL", "Get Issue", e.ToString());

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


                ResultVM resultVm = new ResultVM();
                resultVm.Status = retResults[0];
                resultVm.Message = retResults[1];


                return resultVm;
            }
            #endregion

            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message; //catch ex

                FileLogger.Log("NourishDAL", "Get Issue", ex.ToString());


                throw ex;
            }

            #endregion
        }

        #endregion

        #region Receive

        public DataTable GetReceiveMaster(IntegrationParam param, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";

            string insertSQLText = "";
            int Id = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Insert"; //Method Name

            OracleConnection currConn = null;
            OracleTransaction transaction = null;
            #endregion

            #region try

            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetOracleNourishConnection(param.dtConnectionInfo);

                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                transaction = currConn.BeginTransaction();

                #endregion open connection and transaction


                #region DB Select

                string dbName = DatabaseInfoVM.DatabaseName;


                sqlText = @"
select distinct
0   Selected
,ID 
,'001' BranchCode 
,Receive_Date

from fg_receive_vw 
where 1=1 and ID not in (select REFNO from VAT_Receive_COMPLETED)

";

                if (!string.IsNullOrEmpty(param.RefNo))
                {
                    sqlText += " and ID = :ID";
                }


                if (param.FromDate != "1900-01-01" || param.ToDate != "9000-12-31")
                {
                    sqlText += @" and Receive_Date BETWEEN  :fromDate
                                AND   :toDate";
                }


                ////sqlText += @" and BranchCode = :Branch_Code";
                sqlText += @" and CompanyCode = :Company_Code";

                #endregion

                DataTable dtTable = new DataTable();

                OracleCommand command = new OracleCommand(sqlText, currConn);
                command.Transaction = transaction;


                OracleDataAdapter dataAdapter = new OracleDataAdapter(command);

                if (!string.IsNullOrEmpty(param.RefNo))
                {
                    command.Parameters.Add(new OracleParameter("ID",
                        param.RefNo));
                }


                if (param.FromDate != "1900-01-01" || param.ToDate != "9000-12-31")
                {
                    command.Parameters.Add(new OracleParameter("fromDate",
                        Convert.ToDateTime(param.FromDate).ToString("dd-MMM-yyyy")));

                    command.Parameters.Add(new OracleParameter("toDate",
                        Convert.ToDateTime(param.ToDate).ToString("dd-MMM-yyyy")));
                }


                ////command.Parameters.Add(new OracleParameter("BranchCode",
                ////    param.dtConnectionInfo.Rows[0]["IntegrationCode"]));

                command.Parameters.Add(new OracleParameter("CompanyCode",
                    param.CompanyCode));


                dataAdapter.Fill(dtTable);

                #region Commit

                transaction.Commit();

                #endregion Commit

                //FileLogger.Log("ImportDAL", "GetSale",
                //    param.dtConnectionInfo.Rows[0]["IntegrationCode"] + "\n" + param.CompanyCode + "\n" + sqlText + "\n " + dtTable.Rows.Count);

                return dtTable;

            }
            #endregion

            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message; //catch ex
                if (transaction != null) { transaction.Rollback(); }

                FileLogger.Log("ImportDAL", "GetSale", ex + "\n" + sqlText);

                throw ex;
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
            #endregion
        }

        public DataTable GetReceiveDetails(IntegrationParam param)
        {
            string sqlText = "";
            OracleConnection currConn = null;
            OracleTransaction transaction = null;
            
            DataTable dtTable = new DataTable();

            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetOracleNourishConnection(param.dtConnectionInfo);

                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                transaction = currConn.BeginTransaction();

                #endregion open connection and transaction

                #region DB Select

                string dbName = DatabaseInfoVM.DatabaseName;


                sqlText = @"
SELECT 
      ID
      ,'001' ""BranchCode""
      ,Receive_Date ""Receive_DateTime""
      ,ProductCode ""Item_Code""
      ,'-' ""Item_Name""
      ,Quantity ""Quantity""
      ,'KG' UOM
      ,ID ""Reference_No""
      ,'-' ""Comments""
      ,'N' ""Post""
	  ,'0' ""Return_Id""
	  ,'N' ""With_Toll""
	  ,'0' ""NBR_Price""
	  ,'VAT 4.3' ""VAT_Name""
	  ,'N/A' ""CustomerCode""
  FROM fg_receive_vw iv
where 1=1 and ID not in (select REFNO from VAT_receive_COMPLETED)

";
                
                if (param.IDs.Any())
                {
                    sqlText += " and iv.ID in (";
                    sqlText += "'" + string.Join("','", param.IDs) + "'";
                    sqlText += ")";
                }

                ////sqlText += @" and BranchCode = :Branch_Code";
                sqlText += @" and CompanyCode = :Company_Code";

                #endregion

                OracleCommand command = new OracleCommand(sqlText, currConn);
                command.Transaction = transaction;

                OracleDataAdapter dataAdapter = new OracleDataAdapter(command);

                if (!string.IsNullOrEmpty(param.RefNo))
                {
                    command.Parameters.Add(new OracleParameter("ID",
                        param.RefNo));
                }

                ////command.Parameters.Add(new OracleParameter("BranchCode",
                ////    param.dtConnectionInfo.Rows[0]["IntegrationCode"]));

                command.Parameters.Add(new OracleParameter("CompanyCode",
                    param.CompanyCode));

                dataAdapter.Fill(dtTable);

                #region Commit

                transaction.Commit();

                #endregion Commit


                FileLogger.Log("ImportDAL", "Get Purchase Detail", sqlText + "  \n" + param.dtConnectionInfo.Rows[0]["IntegrationCode"] + "\n " + dtTable.Rows.Count);

            }
            catch (Exception e)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }

                FileLogger.Log("ImportDAL", "Get Purchase Detail", e + "\n" + sqlText);
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

            return dtTable;
        }

        public ResultVM SaveReceive(IntegrationParam param, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";

            string insertSQLText = "";
            int Id = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Insert"; //Method Name

            OracleConnection currConn = null;
            OracleTransaction transaction = null;
            #endregion

            #region try

            try
            {

                // Get Details
                var dtTable = GetReceiveDetails(param);

                ReceiveDAL receiveDal = new ReceiveDAL();

                retResults = receiveDal.SaveTempReceive(dtTable, param.TransactionType, param.CurrentUser,
                    Convert.ToInt32(param.BranchId), () => { }, null, null, null, param.BranchCode);

                try
                {
                    currConn = _dbsqlConnection.GetOracleNourishConnection(param.dtConnectionInfo);

                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }

                    transaction = currConn.BeginTransaction();

                    var completedData = dtTable.DefaultView.ToTable(true, "ID");

                    completedData.Columns.Add(new DataColumn() { ColumnName = "UNIT_CD", DefaultValue = param.BranchCode });
                    completedData.Columns.Add(new DataColumn() { ColumnName = "COM_CD", DefaultValue = param.CompanyCode });
                    completedData.Columns["ID"].ColumnName = "REFNO";

                    //REFNO

                    CommonDAL commonDal = new CommonDAL();

                    commonDal.BulkInsertOracle("VAT_RECEIVE_COMPLETED", completedData, null, currConn, transaction);


                    transaction.Commit();


                }
                catch (Exception e)
                {
                    if (transaction != null) { transaction.Rollback(); }
                    FileLogger.Log("NourishDAL", "Get Receive", e.ToString());

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


                ResultVM resultVm = new ResultVM();
                resultVm.Status = retResults[0];
                resultVm.Message = retResults[1];


                return resultVm;
            }
            #endregion

            #region Catch and Finally
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message; //catch ex

                FileLogger.Log("NourishDAL", "Get Receive", ex.ToString());


                throw ex;
            }

            #endregion
        }

        #endregion

        #region Transfer

        public DataTable GetTransferMaster(IntegrationParam param, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";

            string insertSQLText = "";
            int Id = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Insert"; //Method Name

            OracleConnection currConn = null;
            OracleTransaction transaction = null;
            #endregion

            #region try

            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetOracleConnection(param.dtConnectionInfo);

                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                transaction = currConn.BeginTransaction();

                #endregion open connection and transaction


                #region DB Select

                sqlText = @"
select 
distinct ID
,BranchCode
,TransferToBRanchCode Destination
,TransactionDatetime
from (

select * from VAT_TRANSFER_FG

--union all
--
--select * from VAT_TRANSFER_RW

)  TransferIssues

where 1=1 
and ID not in (select REFNO from VAT_TRANSFERS_COMPLETED)

";

                if (!string.IsNullOrEmpty(param.RefNo))
                {
                    sqlText += " and ID = :ID";
                }


                if (param.FromDate != "1900-01-01" || param.ToDate != "9000-12-31")
                {
                    sqlText += @" and TransactionDatetime BETWEEN  :fromDate
                                AND   :toDate";
                }


                sqlText += @" and BranchCode = :Branch_Code";
                sqlText += @" and Company_Code = :Company_Code";
                sqlText += @" and TransactionType = :TransactionType";

                #endregion

                DataTable dtTable = new DataTable();

                OracleCommand command = new OracleCommand(sqlText, currConn);
                command.Transaction = transaction;


                OracleDataAdapter dataAdapter = new OracleDataAdapter(command);

                if (!string.IsNullOrEmpty(param.RefNo))
                {
                    command.Parameters.Add(new OracleParameter("ID",
                        param.RefNo));
                }


                if (param.FromDate != "1900-01-01" || param.ToDate != "9000-12-31")
                {
                    command.Parameters.Add(new OracleParameter("fromDate",
                        Convert.ToDateTime(param.FromDate).ToString("dd-MMM-yyyy")));

                    command.Parameters.Add(new OracleParameter("toDate",
                        Convert.ToDateTime(param.ToDate).ToString("dd-MMM-yyyy")));
                }


                command.Parameters.Add(new OracleParameter("BranchCode",
                    param.dtConnectionInfo.Rows[0]["IntegrationCode"]));

                command.Parameters.Add(new OracleParameter("Company_Code",
                    param.CompanyCode));

                command.Parameters.Add(new OracleParameter("TransactionType",
                    param.TransactionType));


                dataAdapter.Fill(dtTable);

                #region Commit

                transaction.Commit();

                #endregion Commit


                return dtTable;

            }
            #endregion

            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message; //catch ex
                if (transaction != null) { transaction.Rollback(); }

                FileLogger.Log("ImportDAL", "GetSale", ex + "\n" + sqlText);

                throw ex;
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
            #endregion
        }

        public DataTable GetTransferDetails(IntegrationParam param)
        {
            string sqlText;
            OracleConnection currConn = null;
            OracleTransaction transaction = null;
            #region DB Select

            string dbName = DatabaseInfoVM.DatabaseName;


            sqlText = @"
  select 
ti.ID
      , BranchCode as ""BranchCode""
      , TransferToBranchCode as ""TransferToBranchCode""
      -- ,TO_DATE (TransactionDateTime, 'DD-MM-yy')   as ""TransactionDateTime""
      , TransactionType as ""TransactionType""
      ,ProductCode as ""ProductCode""
      ,vt.Item_NM as ""ProductName""
      ,vt.Unit as ""UOM""
      ,Quantity  as ""Quantity""
      ,0 as ""CostPrice""
      ,'N' as ""Post""
      ,0 as ""VAT_Rate""
      ,ID as ""ReferenceNo""
      , '' as ""Comments"" 
      , NVL(NULLIF(VehicleNo, ''), 'NA') as ""VehicleNo""
      , NVL(case when VehicleType ='' then null end, 'NA' ) as ""VehicleType""
from (

select * from VAT_TRANSFER_FG

--union all
--
--select * from VAT_TRANSFER_RW

)  ti  left outer join VAT_Item vt on
ti.ProductCode = (vt.Type_CD || vt.Cat_CD ||vt.Item_CD)

where 1=1 
and ID not in (select REFNO from VAT_TRANSFERS_COMPLETED)


";


            if (param.IDs.Any())
            {
                sqlText += " and ti.ID in (";
                sqlText += "'" + string.Join("','", param.IDs) + "'";
                sqlText += ")";
            }


            sqlText += @" and BranchCode = :Branch_Code";
            sqlText += @" and Company_Code = :Company_Code";
            sqlText += @" and TransactionType = :TransactionType";

            #endregion

            DataTable dtTable = new DataTable();

            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetOracleConnection(param.dtConnectionInfo);

                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                transaction = currConn.BeginTransaction();

                #endregion open connection and transaction

                OracleCommand command = new OracleCommand(sqlText, currConn);
                command.Transaction = transaction;


                FileLogger.Log("ImportDAL", "GetTransfer", param.CompanyCode + "\n" + param.dtConnectionInfo.Rows[0]["IntegrationCode"] + "\n" + param.TransactionType);


                OracleDataAdapter dataAdapter = new OracleDataAdapter(command);

                if (!string.IsNullOrEmpty(param.RefNo))
                {
                    command.Parameters.Add(new OracleParameter("ID",
                        param.RefNo));
                }


                command.Parameters.Add(new OracleParameter("BranchCode",
                    param.dtConnectionInfo.Rows[0]["IntegrationCode"]));

                command.Parameters.Add(new OracleParameter("Company_Code",
                    param.CompanyCode));

                command.Parameters.Add(new OracleParameter("TransactionType",
                    param.TransactionType));

                dataAdapter.Fill(dtTable);

                dtTable.Columns.Add(new DataColumn()
                {
                    ColumnName = "TransactionDateTime",
                    DefaultValue = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                });

                #region Commit

                transaction.Commit();

                #endregion Commit
            }
            catch (Exception e)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }

                FileLogger.Log("ImportDAL", "GetTransfer", e.ToString() + "\n" + sqlText);
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

            return dtTable;
        }

        public ResultVM SaveTransfer(IntegrationParam param, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";

            string insertSQLText = "";
            int Id = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Insert"; //Method Name

            OracleConnection currConn = null;
            OracleTransaction transaction = null;
            #endregion

            #region try

            try
            {
                // Get Details
                var dtTable = GetTransferDetails(param);

                TransferIssueDAL transferIssueDAL = new TransferIssueDAL();

                retResults = transferIssueDAL.SaveTempTransfer(dtTable, param.BranchCode, param.TransactionType,
                    param.CurrentUserName, Convert.ToInt32(param.BranchId), () => { }, null, null, false, connVM,
                    "", param.CurrentUserId);



                try
                {
                    currConn = _dbsqlConnection.GetOracleConnection(param.dtConnectionInfo);

                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }

                    transaction = currConn.BeginTransaction();

                    var completedData = dtTable.DefaultView.ToTable(true, "ID");

                    completedData.Columns.Add(new DataColumn() { ColumnName = "UNIT_CD", DefaultValue = param.BranchCode });
                    completedData.Columns.Add(new DataColumn() { ColumnName = "COM_CD", DefaultValue = param.CompanyCode });
                    completedData.Columns["ID"].ColumnName = "REFNO";

                    //REFNO

                    CommonDAL commonDal = new CommonDAL();

                    commonDal.BulkInsertOracle("VAT_TRANSFERS_COMPLETED", completedData, null, currConn, transaction);


                    transaction.Commit();


                }
                catch (Exception e)
                {
                    if (transaction != null) { transaction.Rollback(); }
                    FileLogger.Log("ImportDAL", "GetSale", e + "\n" + sqlText);

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


                ResultVM resultVm = new ResultVM();
                resultVm.Status = retResults[0];
                resultVm.Message = retResults[1];


                return resultVm;
            }
            #endregion

            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message; //catch ex

                FileLogger.Log("ImportDAL", "GetSale", ex + "\n" + sqlText);

                throw ex;
            }

            #endregion
        }

        #endregion

        public DataTable GetCustomer(DataTable conInfo, SysDBInfoVMTemp connVM = null)
        {

            #region Initializ
            string sqlText = "";
            CommonDAL commonDal = new CommonDAL();
            string insertSQLText = "";
            int Id = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Insert"; //Method Name
            OracleConnection currConn = null;
            OracleTransaction transaction = null;
            DataTable table = new DataTable();

            #endregion

            #region try

            try
            {
                try
                {

                    #region open connection and transaction

                    currConn = _dbsqlConnection.GetOracleConnection(conInfo);

                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }

                    transaction = currConn.BeginTransaction();

                    #endregion open connection and transaction

                    #region sqlText

                    sqlText = @"
SELECT 
        ROW_NUMBER () OVER (order by ID) as ""SL""
      , ID ""CustomerCode""
      ,name as ""CustomerName""
      ,'-' as ""CustomerGroup""
      ,Address as ""Address""
      ,CELL_NO as ""BIN_No""
      ,'-' as ""Comments"" 
      from VAT_CLIENT_FEED
 ";


                    #endregion

                    OracleCommand cmd = new OracleCommand(sqlText, currConn);
                    cmd.Transaction = transaction;

                    #region Peram
                    #endregion

                    OracleDataAdapter adapter = new OracleDataAdapter(cmd);

                    adapter.Fill(table);


                    FileLogger.Log("NourishIntegration DAL", "GetCustomer", "Total Row: "+table.Rows.Count);



                    #region Commit


                    if (transaction != null)
                    {
                        transaction.Commit();
                    }

                    #endregion Commit



                }
                catch (Exception e)
                {
                    if (transaction != null) { transaction.Rollback(); }
                    FileLogger.Log("NourishIntegration DAL", "GetCustomer", e + "\n" + sqlText);
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


                SqlConnection currSqlConn = null;
                SqlTransaction Sqltransaction = null;

                try
                {
                    #region open connection and transaction

                    currSqlConn = _dbsqlConnection.GetConnection();

                    if (currSqlConn.State != ConnectionState.Open)
                    {
                        currSqlConn.Open();
                    }

                    Sqltransaction = currSqlConn.BeginTransaction();

                    #endregion open connection and transaction

                    sqlText = @"

           create table #tempCustomers(
SL int
,CustomerCode varchar(100)
,CustomerName varchar(100)
,CustomerGroup varchar(100)
,Address varchar(100)
,BIN_No varchar(100)
,Comments varchar(100))

";

                    SqlCommand sqlCommand = new SqlCommand(sqlText, currSqlConn, Sqltransaction);
                    sqlCommand.ExecuteNonQuery();

                    commonDal.BulkInsert("#tempCustomers", table, currSqlConn,
                        Sqltransaction);


                    sqlText = @"  delete #tempCustomers
                                from #tempCustomers join Customers 
                                on #tempCustomers.CustomerCode = customers.CustomerCode";

                    sqlCommand.CommandText = sqlText;
                    sqlCommand.ExecuteNonQuery();

                    sqlCommand.CommandText = "select * from #tempCustomers";

                    SqlDataAdapter adapter = new SqlDataAdapter(sqlCommand);
                    table.Clear();

                    adapter.Fill(table);

                    Sqltransaction.Commit();

                }
                catch (Exception e)
                {
                    if (Sqltransaction != null) { Sqltransaction.Rollback(); }
                    FileLogger.Log("NourishIntegration DAL", "GetCustomer", e + "\n" + sqlText);

                    throw e;
                }
                finally
                {

                    if (currSqlConn != null)
                    {
                        if (currSqlConn.State == ConnectionState.Open)
                        {
                            currSqlConn.Close();
                        }

                    }
                }


                return table;

            }
            #endregion

            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
                //if (transaction != null) { transaction.Rollback(); }
                FileLogger.Log("NourishIntegration DAL", "GetCustomer", ex.ToString() + "\n" + sqlText);

                throw ex;
            }

            #endregion
        }

        public DataTable GetProduct(DataTable conInfo, SysDBInfoVMTemp connVM = null)
        {

            #region Initializ
            string sqlText = "";
            CommonDAL commonDal = new CommonDAL();
            string insertSQLText = "";
            int Id = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Insert"; //Method Name
            OracleConnection currConn = null;
            OracleTransaction transaction = null;
            DataTable table = new DataTable();

            #endregion

            #region try

            try
            {
                try
                {

                    #region open connection and transaction

                    currConn = _dbsqlConnection.GetOracleConnection(conInfo);

                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }

                    transaction = currConn.BeginTransaction();

                    #endregion open connection and transaction

                    #region sqlText

                    sqlText = @"
select 
ROW_NUMBER () OVER (order by (Type_CD || CAT_CD || Item_CD)) as ""SL""
      , (Type_CD || CAT_CD || Item_CD) as ""ProductCode""
      , Item_NM as ""ProductName""
      , '-' as ""ProductGroup"" 
      , Unit as ""UOM""
      , '-' as ""HSCode"" 
      , 0 as ""UnitPrice"" 
      , 0 as ""SD_AblePrice""
      , 0 as ""SDRate""
      , 0 as ""VATRate"" 
      , '-' as ""Description""
      from VAT_Item 
 ";


                    #endregion

                    OracleCommand cmd = new OracleCommand(sqlText, currConn);
                    cmd.Transaction = transaction;

                    #region Peram
                    #endregion

                    OracleDataAdapter adapter = new OracleDataAdapter(cmd);

                    adapter.Fill(table);

                    #region Commit


                    if (transaction != null)
                    {
                        transaction.Commit();
                    }

                    #endregion Commit



                }
                catch (Exception e)
                {
                    if (transaction != null) { transaction.Rollback(); }
                    FileLogger.Log("NourishIntegration DAL", "GetCustomer", e + "\n" + sqlText);
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


                SqlConnection currSqlConn = null;
                SqlTransaction Sqltransaction = null;

                try
                {
                    #region open connection and transaction

                    currSqlConn = _dbsqlConnection.GetConnection();

                    if (currSqlConn.State != ConnectionState.Open)
                    {
                        currSqlConn.Open();
                    }

                    Sqltransaction = currSqlConn.BeginTransaction();

                    #endregion open connection and transaction

                    sqlText = @"

           create table #tempProducts(
SL int
,ProductCode varchar(100)
,ProductName varchar(100)
,ProductGroup varchar(100)
,UOM varchar(100)
,HSCode varchar(100)
,UnitPrice varchar(100)
,SD_AblePrice varchar(100)
,SDRate varchar(100)
,VATRate varchar(100)
,Description varchar(100)

)

";

                    SqlCommand sqlCommand = new SqlCommand(sqlText, currSqlConn, Sqltransaction);
                    sqlCommand.ExecuteNonQuery();

                    commonDal.BulkInsert("#tempProducts", table, currSqlConn,
                        Sqltransaction);


                    sqlText = @"  delete #tempProducts
from #tempProducts join Products 
on #tempProducts.ProductCode = Products.ProductCode";

                    sqlCommand.CommandText = sqlText;
                    sqlCommand.ExecuteNonQuery();

                    sqlCommand.CommandText = "select * from #tempProducts";

                    SqlDataAdapter adapter = new SqlDataAdapter(sqlCommand);
                    table.Clear();

                    adapter.Fill(table);

                    Sqltransaction.Commit();

                }
                catch (Exception e)
                {
                    if (Sqltransaction != null) { Sqltransaction.Rollback(); }
                    FileLogger.Log("NourishIntegration DAL", "GetProduct", e + "\n" + sqlText);

                    throw e;
                }
                finally
                {

                    if (currSqlConn != null)
                    {
                        if (currSqlConn.State == ConnectionState.Open)
                        {
                            currSqlConn.Close();
                        }

                    }
                }


                return table;

            }
            #endregion

            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
                //if (transaction != null) { transaction.Rollback(); }
                FileLogger.Log("NourishIntegration DAL", "GetCustomer", ex.ToString() + "\n" + sqlText);

                throw ex;
            }

            #endregion
        }

    }
}
