using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using VATViewModel.DTOs;

using Excel;
using VATServer.Ordinary;
using Newtonsoft.Json;
using VATServer.Interface;
using System.Globalization;

namespace VATServer.Library
{
    public class IssueDAL// : IIssue
    {
        #region Global Variables

        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        CommonDAL _cDal = new CommonDAL();
        ProductDAL _ProductDAL = new ProductDAL();

        #endregion

        #region Navigation

        public NavigationVM Issue_Navigation(NavigationVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {

            #region Variables

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            string sqlText = "";

            #endregion

            #region try

            try
            {
                #region open connection and transaction

                #region New open connection and transaction

                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                #endregion New open connection and transaction

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }

                #endregion open connection and transaction

                #region Check Point

                if (vm.FiscalYear == 0)
                {
                    DateTime now = DateTime.Now;
                    string startDate = new DateTime(now.Year, now.Month, 1).ToString("yyyy-MMM-dd");
                    FiscalYearVM varFiscalYearVM = new FiscalYearDAL().SelectAll(0, new[] { "PeriodStart" }, new[] { startDate }, currConn, transaction).FirstOrDefault();
                    if (string.IsNullOrWhiteSpace(varFiscalYearVM.PeriodID))
                    {
                        throw new ArgumentNullException("Fiscal Year Not Available for Date: " + now);
                    }

                    vm.FiscalYear = Convert.ToInt32(varFiscalYearVM.CurrentYear);

                }


                #endregion

                #region SQL Statement

                #region SQL Text

                sqlText = "";
                sqlText = @"
------declare @Id as int = 6696;
------declare @FiscalYear as int = 2020;
------declare @TransactionType as varchar(50) = 'Other';
------declare @BranchId as int = 1;

";
                if (vm.ButtonName == "Current")
                {
                    #region Current Item

                    sqlText = sqlText + @"
--------------------------------------------------Current--------------------------------------------------
------------------------------------------------------------------------------------------------------------
select top 1 inv.Id, inv.IssueNo InvoiceNo from IssueHeaders inv
where 1=1 
and inv.IssueNo=@InvoiceNo

";
                    #endregion
                }
                else if (vm.Id == 0 || vm.ButtonName == "First")
                {

                    #region First Item

                    sqlText = sqlText + @"
--------------------------------------------------First--------------------------------------------------
---------------------------------------------------------------------------------------------------------
select top 1 inv.Id, inv.IssueNo InvoiceNo from IssueHeaders inv
where 1=1 
and FiscalYear=@FiscalYear
and TransactionType =@TransactionType
and BranchId = @BranchId
order by Id asc

";
                    #endregion

                }
                else if (vm.ButtonName == "Last")
                {

                    #region Last Item

                    sqlText = sqlText + @"
--------------------------------------------------Last--------------------------------------------------
------------------------------------------------------------------------------------------------------------
select top 1 inv.Id, inv.IssueNo InvoiceNo from IssueHeaders inv
where 1=1 
and FiscalYear=@FiscalYear
and TransactionType =@TransactionType
and BranchId = @BranchId
order by Id desc


";
                    #endregion

                }
                else if (vm.ButtonName == "Next")
                {

                    #region Next Item

                    sqlText = sqlText + @"
--------------------------------------------------Next--------------------------------------------------
------------------------------------------------------------------------------------------------------------
select top 1 inv.Id, inv.IssueNo InvoiceNo from IssueHeaders inv
where 1=1 
and FiscalYear=@FiscalYear
and TransactionType =@TransactionType
and BranchId = @BranchId
and Id > @Id
order by Id asc

";
                    #endregion

                }
                else if (vm.ButtonName == "Previous")
                {
                    #region Previous Item

                    sqlText = sqlText + @"
--------------------------------------------------Previous--------------------------------------------------
------------------------------------------------------------------------------------------------------------
select top 1 inv.Id, inv.IssueNo InvoiceNo from IssueHeaders inv
where 1=1 
and FiscalYear=@FiscalYear
and TransactionType =@TransactionType
and BranchId = @BranchId
and Id < @Id
order by Id desc

";
                    #endregion
                }


                #endregion

                #region SQL Execution
                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);


                if (vm.ButtonName == "Current")
                {
                    cmd.Parameters.AddWithValue("@InvoiceNo", vm.InvoiceNo);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@FiscalYear", vm.FiscalYear);
                    cmd.Parameters.AddWithValue("@TransactionType", vm.TransactionType);
                    cmd.Parameters.AddWithValue("@BranchId", vm.BranchId);

                    if (vm.Id > 0)
                    {
                        cmd.Parameters.AddWithValue("@Id", vm.Id);
                    }
                }



                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt != null && dt.Rows.Count > 0)
                {
                    vm.Id = Convert.ToInt32(dt.Rows[0]["Id"]);
                    vm.InvoiceNo = dt.Rows[0]["InvoiceNo"].ToString();
                }
                else
                {
                    if (vm.ButtonName == "Previous" || vm.ButtonName == "Current")
                    {
                        vm.ButtonName = "First";
                        vm = Issue_Navigation(vm, currConn, transaction);

                    }
                    else if (vm.ButtonName == "Next")
                    {
                        vm.ButtonName = "Last";
                        vm = Issue_Navigation(vm, currConn, transaction);

                    }
                }


                #endregion

                #endregion

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion

            }
            #endregion

            #region catch

            catch (Exception ex)
            {
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Rollback();
                }
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());

                FileLogger.Log("IssueDAL", "Issue_Navigation", ex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", ex.Message.ToString());

            }
            #endregion

            #region finally

            finally
            {
                if (VcurrConn == null)
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

            #endregion

            return vm;

        }

        #endregion

        public DataTable SearchIssueDetailDTNew(string IssueNo, string databaseName, SysDBInfoVMTemp connVM = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            DataTable dataTable = new DataTable("IssueSearchDetail");

            #endregion

            #region Try
            try
            {
                #region open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                #endregion open connection and transaction

                #region open connection and transaction

                ////currConn = _dbsqlConnection.GetConnection(connVM);
                ////if (currConn.State != ConnectionState.Open)
                ////{
                ////    currConn.Open();
                ////}

                #endregion open connection and transaction

                #region SQL Statement

                sqlText = @"
SELECT  
idd.IssueNo, 
idd.IssueLineNo,
idd.ItemNo, 
isnull(idd.Quantity,0)Quantity ,
isnull(idd.CostPrice,0)CostPrice,
isnull(idd.NBRPrice,0)NBRPrice,
isnull(idd.UOM,'N/A')UOM ,
isnull(idd.VATRate,0)VATRate,
isnull(idd.VATAmount,0)VATAmount,
isnull(idd.SubTotal,0)SubTotal,
isnull(idd.Comments,'N/A')Comments,
isnull(Products.ProductName,'N/A')ProductName,
isnull(isnull(Products.OpeningBalance,0)+
isnull(Products.QuantityInHand,0),0) as Stock,
isnull(idd.SD,0)SD,
isnull(idd.SDAmount,0)SDAmount,
isnull(Products.ProductCode,'N/A')ProductCode,
isnull(idd.UOMQty,isnull(idd.Quantity,0))UOMQty,
isnull(idd.UOMn,idd.UOM)UOMn,
isnull(idd.UOMc,1)UOMc,
isnull(idd.UOMPrice,isnull(idd.CostPrice,0))UOMPrice,
isnull(idd.UOMWastage,isnull(idd.Wastage,0))UOMWastage,
isnull(idd.BOMId,0)	BOMId,
idd.VATName,
isnull(idd.FinishItemNo,'0')FinishItemNo,
isnull(fp.ProductCode,'N/A')FinishProductCode,
isnull(fp.ProductName,'N/A')FinishProductName

                            FROM         dbo.IssueDetails idd  left outer join
                            Products on idd.ItemNo=Products.ItemNo LEFT OUTER JOIN
                            Products fp on idd.FinishItemNo=fp.ItemNo 
                            
                               
                            WHERE 
                            (IssueNo = @IssueNo ) 
                            ";

                #endregion

                #region SQL Command

                SqlCommand objCommIssueDetail = new SqlCommand();
                objCommIssueDetail.Connection = currConn;
                objCommIssueDetail.Transaction = transaction;

                objCommIssueDetail.CommandText = sqlText;
                objCommIssueDetail.CommandType = CommandType.Text;

                #endregion

                #region Parameter

                if (!objCommIssueDetail.Parameters.Contains("@IssueNo"))
                { objCommIssueDetail.Parameters.AddWithValue("@IssueNo", IssueNo); }
                else { objCommIssueDetail.Parameters["@IssueNo"].Value = IssueNo; }

                #endregion Parameter

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommIssueDetail);
                dataAdapter.Fill(dataTable);

                #region Commit

                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }

                #endregion Commit

            }
            #endregion

            #region Catch & Finally
            catch (SqlException sqlex)
            {
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Rollback();
                }
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;

                FileLogger.Log("IssueDAL", "SearchIssueDetailDTNew", sqlex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Rollback();
                }
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;

                FileLogger.Log("IssueDAL", "SearchIssueDetailDTNew", ex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", ex.Message.ToString());

            }
            finally
            {
                if (VcurrConn == null)
                {
                    if (currConn != null)
                    {
                        if (currConn.State == ConnectionState.Open)
                        {
                            currConn.Close();
                        }
                    }
                }

                ////if (currConn != null)
                ////{
                ////    if (currConn.State == ConnectionState.Open)
                ////    {
                ////        currConn.Close();
                ////    }
                ////}
            }

            #endregion

            return dataTable;
        }

        public string[] ImportData(DataTable dtIssueM, DataTable dtIssueD, int branchId = 0, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, Action callBack = null, SysDBInfoVMTemp connVM = null, string UserId = "")
        {
            #region variable
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";

            IssueMasterVM issueMasterVM;
            List<IssueDetailVM> issueDetailVMs = new List<IssueDetailVM>();
            CommonDAL commonDal = new CommonDAL();

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion variable

            #region try
            try
            {
                #region open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnectionNoPooling(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                #endregion open connection and transaction

                #region RowCount
                int MRowCount = 0;
                int MRow = dtIssueM.Rows.Count;
                for (int i = 0; i < dtIssueM.Rows.Count; i++)
                {
                    if (!string.IsNullOrEmpty(dtIssueM.Rows[i]["ID"].ToString()))
                    {
                        MRowCount++;
                    }

                }
                if (MRow != MRowCount)
                {
                    throw new ArgumentNullException("you have select " + MRow.ToString() + " data for import, but you have " + MRowCount + " id.");
                }
                #endregion RowCount

                #region ID in Master or Detail table

                for (int i = 0; i < MRowCount; i++)
                {
                    string importID = dtIssueM.Rows[i]["ID"].ToString();
                    if (!string.IsNullOrEmpty(importID))
                    {
                        DataRow[] DetailRaws = dtIssueD.Select("ID='" + importID + "'");
                        if (DetailRaws.Length == 0)
                        {
                            throw new ArgumentNullException("The ID " + importID + " is not available in detail table");
                        }

                    }

                }

                #endregion

                #region Double ID in Master

                for (int i = 0; i < MRowCount; i++)
                {
                    string id = dtIssueM.Rows[i]["ID"].ToString();
                    DataRow[] tt = dtIssueM.Select("ID='" + id + "'");
                    if (tt.Length > 1)
                    {
                        throw new ArgumentNullException("you have duplicate master id " + id + " in import file.");
                    }

                }

                #endregion Double ID in Master


                CommonImportDAL cImport = new CommonImportDAL();

                #region checking from database is exist the information(NULL Check)

                #region Master

                for (int j = 0; j < MRowCount; j++)
                {
                    #region Checking Date is null or different formate

                    bool IsIssueDate;
                    IsIssueDate = cImport.CheckDate(dtIssueM.Rows[j]["Issue_DateTime"].ToString().Trim());
                    if (IsIssueDate != true)
                    {
                        throw new ArgumentNullException("Please insert correct date format 'DD/MMM/YY' such as 31/Jan/13 in Issue_Date field.");
                    }
                    #endregion Checking Date is null or different formate

                    #region Checking Y/N value
                    bool post;
                    post = cImport.CheckYN(dtIssueM.Rows[j]["Post"].ToString().Trim());
                    if (post != true)
                    {
                        throw new ArgumentNullException("Please insert Y/N in Post field.");
                    }
                    #endregion Checking Y/N value

                    #region Check Return issue id
                    string ReturnId = string.Empty;
                    ReturnId = cImport.CheckIssueReturnID(dtIssueM.Rows[j]["Return_Id"].ToString().Trim(), currConn, transaction, connVM);
                    #endregion Check Return issue id
                }

                #endregion Master

                #region Details

                #region Row count for details table
                int DRowCount = 0;
                for (int i = 0; i < dtIssueD.Rows.Count; i++)
                {
                    if (!string.IsNullOrEmpty(dtIssueD.Rows[i]["ID"].ToString()))
                    {
                        DRowCount++;
                    }

                }

                #endregion Row count for details table

                for (int i = 0; i < DRowCount; i++)
                {
                    string ItemNo = string.Empty;
                    string UOMn = string.Empty;

                    #region FindItemId
                    if (string.IsNullOrEmpty(dtIssueD.Rows[i]["Item_Code"].ToString().Trim()))
                    {
                        throw new ArgumentNullException("Please insert value in 'Item_Code' field.");
                    }
                    //bool ItemAutoSave = Convert.ToBoolean(commonDal.settingValue("AutoSave", "IssueProduct") == "Y" ? true : false);

                    //ItemNo = cImport.FindItemId(dtIssueD.Rows[i]["Item_Name"].ToString().Trim()
                    //                            , dtIssueD.Rows[i]["Item_Code"].ToString().Trim(), currConn, transaction, ItemAutoSave);

                    #endregion FindItemId

                    #region FindUOMn

                    //UOMn = cImport.FindUOMn(ItemNo, currConn, transaction);

                    #endregion FindUOMn

                    #region FindUOMn
                    if (DatabaseInfoVM.DatabaseName.ToString() != "Sanofi_DB")
                    {
                        //cImport.FindUOMc(UOMn, dtIssueD.Rows[i]["UOM"].ToString().Trim(), currConn, transaction);
                    }
                    #endregion FindUOMn

                    #region Numeric value check
                    bool IsQuantity = cImport.CheckNumericBool(dtIssueD.Rows[i]["Quantity"].ToString().Trim());
                    if (IsQuantity != true)
                    {
                        throw new ArgumentNullException("Please insert decimal value in Quantity field.");
                    }
                    #endregion Numeric value check
                }

                #endregion Details


                #endregion checking from database is exist the information(NULL Check)

                //if (currConn.State == ConnectionState.Open)
                //{
                //    currConn.Close();
                //    currConn.Open();
                //    transaction = currConn.BeginTransaction("Import Data.");
                //}
                CommonDAL commonDalssue = new CommonDAL();
                string negStock = commonDalssue.settings("Issue", "NegStockAllow", currConn, transaction, connVM);

                decimal TotalAmount;
                var tempBranch = branchId;
                for (int j = 0; j < MRowCount; j++)
                {
                    TotalAmount = 0;

                    #region Master Issue

                    string importID = dtIssueM.Rows[j]["ID"].ToString().Trim();
                    DateTime issueDateTime = Convert.ToDateTime(dtIssueM.Rows[j]["Issue_DateTime"].ToString().Trim());
                    #region CheckNull
                    string serialNo = cImport.ChecKNullValue(dtIssueM.Rows[j]["Reference_No"].ToString().Trim());
                    string comments = cImport.ChecKNullValue(dtIssueM.Rows[j]["Comments"].ToString().Trim());
                    #endregion CheckNull

                    #region Check Return issue id
                    string issueReturnId = cImport.CheckIssueReturnID(dtIssueM.Rows[j]["Return_Id"].ToString().Trim(), currConn, transaction, connVM);
                    #endregion Check Return receive id
                    string post = dtIssueM.Rows[j]["Post"].ToString().Trim();
                    string createdBy = dtIssueM.Rows[j]["Created_By"].ToString().Trim();
                    string lastModifiedBy = dtIssueM.Rows[j]["LastModified_By"].ToString().Trim();
                    string transactionType = dtIssueM.Rows[j]["Transection_Type"].ToString().Trim();
                    string branchCode = dtIssueM.Rows[j]["BranchCode"].ToString().Trim();
                    string branch_Id = dtIssueM.Rows[j]["BranchId"].ToString().Trim();

                    //if (string.IsNullOrEmpty(branchCode))
                    //{
                    //    throw new ArgumentNullException("branchId Code Not Found For ID= "+importID);
                    //}
                    var branchCodeInDb = "";
                    if (string.IsNullOrEmpty(branch_Id))
                    {
                        branchCodeInDb = cImport.FindBranchId(branchCode, currConn, transaction, connVM);

                        branchId = branchCodeInDb != "0" ? Convert.ToInt32(branchCodeInDb) : tempBranch;
                    }
                    else
                    {
                        branchId = Convert.ToInt32(branch_Id);
                    }

                    issueMasterVM = new IssueMasterVM();

                    issueMasterVM.IssueDateTime = issueDateTime.ToString("yyyy-MM-dd") + DateTime.Now.ToString(" HH:mm:ss");
                    issueMasterVM.TotalVATAmount = 0;
                    issueMasterVM.TotalAmount = Convert.ToDecimal(0);
                    issueMasterVM.SerialNo = serialNo.Replace(" ", "");
                    issueMasterVM.Comments = comments;
                    issueMasterVM.CreatedBy = createdBy;
                    issueMasterVM.CreatedOn = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    issueMasterVM.LastModifiedBy = lastModifiedBy;
                    issueMasterVM.LastModifiedOn = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    issueMasterVM.ReturnId = issueReturnId;
                    issueMasterVM.transactionType = transactionType;
                    issueMasterVM.Post = post;
                    issueMasterVM.ImportId = importID;
                    issueMasterVM.BranchId = branchId;
                    DataRow[] DetailRaws; //= new DataRow[];//

                    #region MAtch

                    if (!string.IsNullOrEmpty(importID))
                    {
                        DetailRaws = dtIssueD.Select("ID='" + importID + "'");
                    }
                    else
                    {
                        DetailRaws = null;
                    }

                    #endregion MAtch

                    #endregion Master Issue

                    #region Details Issue

                    int counter = 1;
                    issueDetailVMs = new List<IssueDetailVM>();
                    // Juwel 13/10/2015
                    DataTable dtDistinctItem = DetailRaws.CopyToDataTable().DefaultView.ToTable(true, "Item_Code", "Item_Name", "ItemNo", "BomId");

                    DataTable dtIssueDetail = DetailRaws.CopyToDataTable();

                    foreach (DataRow item in dtDistinctItem.Rows)
                    {
                        DataTable dtRepeatedItems = new DataTable();
                        string ItemNo = item["ItemNo"].ToString();
                        string itemCode = item["Item_Code"].ToString();
                        string Item_Name = item["Item_Name"].ToString();
                        string ImportId = dtIssueDetail.Rows[0]["ID"].ToString();

                        if (!string.IsNullOrWhiteSpace(ItemNo) && ItemNo != "0" && ItemNo != "-")
                        {
                            dtRepeatedItems = dtIssueDetail.Select("[ItemNo] ='" + ItemNo + "'").CopyToDataTable();

                        }
                        else if (!string.IsNullOrWhiteSpace(itemCode) && itemCode != "-")
                        {
                            dtRepeatedItems = dtIssueDetail.Select("[Item_Code] ='" + itemCode + "'").CopyToDataTable();

                        }
                        else if (!string.IsNullOrWhiteSpace(Item_Name) && Item_Name != "-")
                        {
                            dtRepeatedItems = dtIssueDetail.Select("[Item_Name] ='" + Item_Name + "'").CopyToDataTable();
                        }
                        else
                        {
                            throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, "Item Code and Item Name not found for ID : " + ImportId);
                        }

                        //string itemCode = itemCode.Trim();
                        string itemName = item["Item_Name"].ToString().Trim();
                        string itemNo = item["ItemNo"].ToString().Trim();

                        if (string.IsNullOrEmpty(itemNo))
                        {
                            bool ItemAutoSave = commonDal.settingValue("AutoSave", "IssueProduct") == "Y";

                            itemNo = cImport.FindItemId(itemName, itemCode, currConn, transaction, ItemAutoSave, "-", 1, 0, 0, connVM, "-");
                        }

                        decimal quantity = 0;
                        decimal avgPrice;
                        CommonDAL cmnDal = new CommonDAL();

                        ProductDAL productDal = new ProductDAL();

                        OrdinaryVATDesktop.BranchId = branchId;

                        DataTable priceData = productDal.AvgPriceNew(itemNo, issueDateTime.ToString("yyyy-MM-dd") + DateTime.Now.ToString(" HH:mm:ss"), currConn,
                            transaction, true, true, true, false, null, UserId);

                        decimal amount = Convert.ToDecimal(priceData.Rows[0]["Amount"].ToString());
                        decimal quan = Convert.ToDecimal(priceData.Rows[0]["Quantity"].ToString());
                        if (quan > 0)
                        {
                            avgPrice = cmnDal.FormatingDecimal((amount / quan).ToString(), connVM);
                        }
                        else
                        {
                            avgPrice = 0;
                        }

                        //if (avgPrice < 0)
                        //{
                        //    avgPrice = 0;
                        //}

                        string uOM = "";
                        string uOMn = "";
                        string uOMc = "";

                        IssueDetailVM detail = new IssueDetailVM();

                        foreach (DataRow row in dtRepeatedItems.Rows)
                        {

                            if (DatabaseInfoVM.DatabaseName.ToString() == "Sanofi_DB")
                            {
                                uOMn = cImport.FindUOMn(itemNo, currConn, transaction, connVM);
                                uOM = uOMn;
                                uOMc = "1";
                            }
                            else
                            {
                                uOM = row["UOM"].ToString().Trim();
                                uOMn = cImport.FindUOMn(itemNo, currConn, transaction, connVM);
                                uOMc = cImport.FindUOMc(uOMn, uOM, currConn, transaction, connVM);
                            }
                            quantity = quantity + Convert.ToDecimal(row["Quantity"].ToString().Trim());
                        }

                        if (negStock == "N")
                        {
                            if (
                                (quantity * Convert.ToDecimal(uOMc)) >
                                Convert.ToDecimal(quan))
                            {
                                throw new Exception("Stock Not Available for " + itemName + " (" + itemCode + ")");
                            }
                        }

                        detail.ItemNo = itemNo;
                        detail.IssueLineNo = counter.ToString();
                        detail.Quantity = Convert.ToDecimal(quantity);
                        detail.NBRPrice = 0;
                        detail.VATRate = 0;
                        detail.VATAmount = 0;
                        detail.UOM = uOM;
                        detail.SD = 0;
                        detail.SDAmount = 0;
                        detail.CommentsD = "NA";
                        detail.IssueDateTimeD = issueDateTime.ToString("yyyy-MM-dd") + DateTime.Now.ToString(" HH:mm:ss");
                        detail.BOMDate = "1900-01-01";
                        detail.FinishItemNo = "0";
                        detail.UOMn = uOMn;
                        detail.UOMc = Convert.ToDecimal(uOMc);
                        detail.Wastage = 0;
                        detail.CostPrice = Convert.ToDecimal(avgPrice);
                        detail.SubTotal = Convert.ToDecimal(Convert.ToDecimal(avgPrice) * Convert.ToDecimal(quantity));
                        TotalAmount = TotalAmount + Convert.ToDecimal(Convert.ToDecimal(avgPrice) * Convert.ToDecimal(quantity));
                        detail.UOMQty = Convert.ToDecimal(Convert.ToDecimal(quantity) * Convert.ToDecimal(uOMc));
                        detail.UOMPrice = Convert.ToDecimal(Convert.ToDecimal(avgPrice) * Convert.ToDecimal(uOMc));
                        detail.BranchId = branchId;
                        detail.BOMId = !string.IsNullOrEmpty(item["BomId"].ToString()) ? Convert.ToInt32(item["BomId"].ToString()) : 0;

                        if (issueDetailVMs.Any(x => x.ItemNo == itemNo))
                        {
                            continue;
                        }

                        issueDetailVMs.Add(detail);
                        counter++;
                    }

                    #region Previous code by ruba apu 13/10/2015

                    //foreach (DataRow row in DetailRaws)
                    //{
                    //    //string itemCode = row["Item_Code"].ToString().Trim();
                    //    //string itemName = row["Item_Name"].ToString().Trim();

                    //    //string itemNo = cImport.FindItemId(itemName, itemCode, currConn, transaction);

                    //    //string quantity = row["Quantity"].ToString().Trim();
                    //    //string uOM ="";
                    //    //string uOMn="";
                    //    //string uOMc = "";
                    //    //if (DatabaseInfoVM.DatabaseName.ToString() == "Sanofi_DB")
                    //    //{
                    //    //    uOMn = cImport.FindUOMn(itemNo, currConn, transaction);
                    //    //    uOM = uOMn;
                    //    //    uOMc = "1";
                    //    //}
                    //    //else
                    //    //{
                    //    //    uOM = row["UOM"].ToString().Trim();
                    //    //    uOMn = cImport.FindUOMn(itemNo, currConn, transaction);
                    //    //    uOMc = cImport.FindUOMc(uOMn, uOM, currConn, transaction);
                    //    //}

                    //    //IssueDetailVM detail = new IssueDetailVM();
                    //    detail.ItemNo = itemNo;
                    //    detail.Quantity = Convert.ToDecimal(quantity);
                    //    detail.NBRPrice = 0;
                    //    detail.VATRate = 0;
                    //    detail.VATAmount = 0;
                    //    detail.UOM = uOM;
                    //    detail.SD = 0;
                    //    detail.SDAmount = 0;
                    //    detail.CommentsD = "NA";
                    //    detail.IssueDateTimeD =
                    //        issueDateTime.ToString("yyyy-MM-dd") +
                    //                           DateTime.Now.ToString(" HH:mm:ss");
                    //    detail.BOMDate = "1900-01-01";
                    //    detail.FinishItemNo = "0";
                    //    detail.UOMn = uOMn;
                    //    detail.UOMc = Convert.ToDecimal(uOMc);
                    //    detail.Wastage = 0;

                    //    CommonDAL cmnDal = new CommonDAL();
                    //    decimal avgPrice;

                    //        DataTable priceData = cImport.FindAvgPriceImport(itemNo, issueDateTime.ToString("yyyy-MM-dd") + DateTime.Now.ToString(" HH:mm:ss"), currConn, transaction);
                    //        decimal amount = Convert.ToDecimal(priceData.Rows[0]["Amount"].ToString());
                    //        decimal quan = Convert.ToDecimal(priceData.Rows[0]["Quantity"].ToString());

                    //        if (quan > 0)
                    //        {
                    //            avgPrice = cmnDal.FormatingDecimal((amount/quan).ToString());
                    //        }
                    //        else
                    //        {
                    //            avgPrice = 0;
                    //        }

                    //    //detail.CostPrice = cmnDal.FormatingDecimal(avgPrice);
                    //    detail.CostPrice = Convert.ToDecimal(avgPrice);

                    //    detail.SubTotal = Convert.ToDecimal(Convert.ToDecimal(avgPrice) * Convert.ToDecimal(quantity));
                    //    detail.UOMQty = Convert.ToDecimal(Convert.ToDecimal(quantity) / Convert.ToDecimal(uOMc));
                    //    detail.UOMPrice = Convert.ToDecimal(Convert.ToDecimal(avgPrice) / Convert.ToDecimal(uOMc));

                    //    issueDetailVMs.Add(detail);
                    //    counter++;
                    //} // detail

                    #endregion previous code

                    #endregion Details Issue
                    issueMasterVM.TotalAmount = Convert.ToDecimal(TotalAmount);

                    string[] sqlResults = IssueInsert(issueMasterVM, issueDetailVMs, transaction, currConn, connVM);
                    retResults[0] = sqlResults[0];

                    if (callBack != null)
                    {
                        callBack();

                    }
                }

                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                #endregion Commit
                #region SuccessResult

                retResults[0] = "Success";
                retResults[1] = MessageVM.issueMsgSaveSuccessfully;
                retResults[2] = "" + "1";
                retResults[3] = "" + "N";
                #endregion SuccessResult
            }
            #endregion try
            #region Catch and Finall
            catch (Exception ex)
            {
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Rollback();
                }
                retResults[0] = "Fail";//Success or Fail
                retResults[1] = ex.Message.ToString(); //catch ex
                //retResults[4] = ex.Message.ToString(); //catch ex
                //////if (transaction != null && Vtransaction == null) { transaction.Rollback(); }
                FileLogger.Log("IssueDAL", "ImportData", ex.ToString());

                throw ex;
            }
            finally
            {
                if (VcurrConn == null)
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
            #endregion

            return retResults;
        }

        public int GetUnProcessedCount(SysDBInfoVMTemp connVM = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            #region try

            try
            {

                #region open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                #endregion open connection and transaction

                #region Open Connection and Transaction

                ////if (currConn == null)
                ////{
                ////    currConn = _dbsqlConnection.GetConnection(connVM);
                ////    currConn.Open();
                ////    transaction = currConn.BeginTransaction();
                ////}

                #endregion

                #region Sql Text

                var sql = @"select count(distinct(Id)) from IssueTempData where IsProcessed = 0";

                #endregion


                #region Sql Command

                var cmd = new SqlCommand(sql, currConn, transaction);
                var rowCount = (int)cmd.ExecuteScalar();
                #endregion

                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                #endregion Commit

                ////transaction.Commit();

                return rowCount;

            }
            #endregion

            #region Catch and finally

            catch (Exception ex)
            {

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Rollback();
                }
                ////throw new ArgumentNullException("", "SQL:" + "" + FieldDelimeter + ex.Message.ToString());

                FileLogger.Log("IssueDAL", "GetUnProcessedCount", ex.ToString());
                throw new ArgumentNullException("", ex.Message.ToString());

            }
            finally
            {
                if (VcurrConn == null)
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

            #endregion
        }

        public string[] ImportBigData(DataTable issueData, SysDBInfoVMTemp connVM = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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

            #region try

            try
            {
                #region open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.StatisticsEnabled = true;
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("Import Big Data");
                }
                #endregion open connection and transaction

                #region Open Connection and Transaction

                ////if (currConn == null)
                ////{
                ////    currConn = _dbsqlConnection.GetConnection(connVM);
                ////    currConn.StatisticsEnabled = true;
                ////    currConn.Open();
                ////    transaction = currConn.BeginTransaction("Import Big Data");
                ////}

                #endregion

                #region SQL Text

                var updateItemNo = @"
update IssueTempData set ItemNo=Products.ItemNo from Products where Products.ProductCode=IssueTempData.Item_Code;
update IssueTempData set ItemNo=Products.ItemNo from Products where Products.productName =IssueTempData.Item_Name;
";

                var getdefaultData = @"select * from IssueTempData where ItemNo = 0;";
                var branchUpdate = @"
update IssueTempData set BranchId=BranchProfiles.BranchID from BranchProfiles where BranchProfiles.BranchCode=IssueTempData.Branch_Code;
";


                var deleteTemp = @"delete from IssueTempData; ";

                deleteTemp += " DBCC CHECKIDENT ('IssueTempData', RESEED, 0);";

                #endregion

                var cmd = new SqlCommand(deleteTemp, currConn, transaction);
                cmd.ExecuteNonQuery();

                var commonDal = new CommonDAL();

                retResults = commonDal.BulkInsert("IssueTempData", issueData, currConn, transaction, 10000, null, connVM);

                #region Sql Command

                cmd.CommandText = branchUpdate + " " + updateItemNo;
                var updateItemResult = cmd.ExecuteNonQuery();

                cmd.CommandText = getdefaultData;
                var defaultData = new DataTable();
                var dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(defaultData);

                #endregion

                #region Insert Customer and Products

                var commonImportDal = new CommonImportDAL();

                foreach (DataRow row in defaultData.Rows)
                {
                    if (row["ItemNo"].ToString().Trim() == "0")
                    {
                        var itemCode = row["Item_Code"].ToString().Trim();
                        var itemName = row["Item_Name"].ToString().Trim();

                        commonImportDal.FindItemId(itemName, itemCode, currConn, transaction, true, "-", 1, 0, 0, connVM, "-");
                    }
                }

                #endregion

                #region Reupdate

                cmd.CommandText = updateItemNo;
                var reUpdate = cmd.ExecuteNonQuery();

                #endregion

                ////////if (retResults[0].ToLower() == "success" && updateItemResult > 0)
                ////////{
                ////////    transaction.Commit();
                ////////}

                #region Commit
                if (retResults[0].ToLower() == "success" && updateItemResult > 0)
                {
                    if (Vtransaction == null)
                    {
                        if (transaction != null)
                        {
                            transaction.Commit();
                        }
                    }
                }
                #endregion Commit

                retResults[0] = retResults[0].ToLower() == "success" && (updateItemResult > 0 || reUpdate > 0) ? "success" : "fail";

                retResults[2] = currConn.RetrieveStatistics()["ExecutionTime"].ToString();


            }
            #endregion

            #region Catch and finally

            catch (Exception ex)
            {
                if (transaction != null && Vtransaction == null)
                {
                    transaction.Rollback();
                }
                ////throw new ArgumentNullException("", "SQL:" + "" + FieldDelimeter + ex.Message.ToString());

                FileLogger.Log("IssueDAL", "ImportBigData", ex.ToString());
                throw new ArgumentNullException("", ex.Message.ToString());

            }
            finally
            {
                if (VcurrConn == null)
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

            #endregion

            return retResults;
        }

        public string[] SaveTempIssue(DataTable dtTable, string transactionType, string CurrentUser, int branchId, Action callBack, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null, string BranchCode = "")
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
            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            CommonDAL commonDal = new CommonDAL();

            #endregion

            #region try

            try
            {
                #region open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnectionNoPooling(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                #endregion open connection and transaction

                #region DateFormat

                string CustomeDateFormat = commonDal.settingValue("Integration", "CustomeDateFormat", connVM, currConn, transaction);

                if (dtTable.Columns.Contains("Issue_DateTime"))
                {
                    if (!string.IsNullOrWhiteSpace(CustomeDateFormat) && CustomeDateFormat != "-")
                    {
                        if (CustomeDateFormat == "DD.MM.YYYY")
                        {
                            CustomeDateFormat = "dd.MM.yyyy";
                        }
                        string pattern = CustomeDateFormat;
                        foreach (DataRow dataRow in dtTable.Rows)
                        {
                            DateTime parsedDate;

                            if (DateTime.TryParseExact(dataRow["Issue_DateTime"].ToString().Trim(), pattern, null, DateTimeStyles.None, out parsedDate))
                            {
                                dataRow["Issue_DateTime"] = parsedDate.ToString("yyyy-MMM-dd HH:mm:ss");
                            }

                        }
                    }
                    else
                    {
                        foreach (DataRow dataRow in dtTable.Rows)
                        {
                            dataRow["Issue_DateTime"] = OrdinaryVATDesktop.DateToDate(dataRow["Issue_DateTime"].ToString());
                        }
                    }

                }

                #endregion

                sqlText = @"delete from TempIssueData";


                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);

                cmd.ExecuteNonQuery();

                commonDal.BulkInsert("TempIssueData", dtTable, currConn, transaction, 0, null, connVM);

                string itemUpdate = @"
update TempIssueData set ItemNo = Products.ItemNo from Products where Products.ProductCode = TempIssueData.Item_Code 
and (TempIssueData.ItemNo is null or TempIssueData.ItemNo = '0') and TempIssueData.Item_Code !='-' and TempIssueData.Item_Code !='0';
update TempIssueData set ItemNo = Products.ItemNo from Products where Products.ProductName = TempIssueData.Item_Name 
and (TempIssueData.ItemNo is null or TempIssueData.ItemNo = '0') and TempIssueData.Item_Name !='-' and TempIssueData.Item_Name !='';";

                string branchCode =
                    @"
update TempIssueData set BranchId = BranchProfiles.BranchID 
from BranchProfiles where BranchProfiles.BranchCode = TempIssueData.BranchCode 
or BranchProfiles.IntegrationCode = TempIssueData.BranchCode

update TempIssueData set BranchId = BranchMapDetails.BranchID 
from BranchMapDetails where BranchMapDetails.IntegrationCode = TempIssueData.BranchCode
and (TempIssueData.BranchId is null or TempIssueData.BranchId = '' or TempIssueData.BranchId = '0');

";

                string bom = @"update TempIssueData 
set BOMId = BOMRaws.BOMId 
from BOMRaws where BOMRaws.RawItemNo = TempIssueData.ItemNo 
and BOMRaws.effectdate<= cast(TempIssueData.Issue_DateTime as datetime) and BOMRaws.post='Y';";

                string updateUOM = @"update TempIssueData set UOM = Products.UOM 
from Products where Products.ItemNo = TempIssueData.ItemNo and TempIssueData.UOM = '-'";


                string getAll = @"select * from TempIssueData";

                cmd.CommandText = itemUpdate + " " + branchCode + " " + " " + bom;

                cmd.ExecuteNonQuery();

                string autoUOM = commonDal.settingValue("Issue", "AutoUOM", connVM, currConn, transaction);

                if (autoUOM == "Y")
                {
                    cmd.CommandText += " " + updateUOM;
                    cmd.ExecuteNonQuery();

                }


                #region Delete Duplicate

                string CompanyCode = commonDal.settings("CompanyCode", "Code", currConn, transaction, connVM);
                string duplicate = commonDal.settings("Import", "IssueDuplicateInsert", currConn, transaction, connVM);

                string deleteDuplicate = @"
delete TempIssueData from 
TempIssueData 
join IssueHeaders on TempIssueData.ID = IssueHeaders.ImportIDExcel";

                string selectDuplicate = @"select distinct TempIssueData.ID,IssueHeaders.ImportIDExcel from 
TempIssueData join IssueHeaders on TempIssueData.ID = IssueHeaders.ImportIDExcel";

                if (CompanyCode.ToLower() == "cp" || CompanyCode.ToLower() == "smc" || CompanyCode.ToLower() == "smcholding" || CompanyCode.ToLower() == "eon" || CompanyCode.ToLower() == "DBH".ToLower()
                    || CompanyCode.ToLower() == "purofood" || CompanyCode.ToLower() == "eahpl" || CompanyCode.ToLower() == "eail" || CompanyCode.ToLower() == "eeufl"
                    || CompanyCode.ToLower() == "exfl" || CompanyCode.ToLower() == "mbl" || CompanyCode.ToLower() == "MBLShirirchala".ToLower() || CompanyCode.ToLower() == "MBLMouchak".ToLower()
                    || CompanyCode.ToLower() == "MBLMirsarai".ToLower() || OrdinaryVATDesktop.IsNourishCompany(CompanyCode)
                    || CompanyCode.ToLower() == "SMC".ToLower() || CompanyCode.ToLower() == "SMCHOLDING".ToLower())// 
                {
                    if (duplicate.ToLower() == "n")
                    {
                        cmd.CommandText = selectDuplicate;
                        SqlDataAdapter ddataAdapter = new SqlDataAdapter(cmd);
                        DataTable duplicates = new DataTable();
                        ddataAdapter.Fill(duplicates);

                        string duplicateIds = string.Join(", ", duplicates.Rows.OfType<DataRow>().Select(r => r[0].ToString()));

                        if (duplicates.Rows.Count > 0)
                        {
                            throw new Exception("These Invoices are already in system-" + duplicateIds);
                        }
                    }
                    else if (duplicate.ToLower() == "y")
                    {
                        cmd.CommandText = deleteDuplicate;

                        int deletedData = cmd.ExecuteNonQuery();
                    }
                }

                #endregion

                #region Skip Items

                string skipItems = commonDal.settings("Issue", "SkipItem", currConn, transaction, connVM);
                string autoSave = commonDal.settings("AutoSave", "IssueProduct", currConn, transaction, connVM);

                string deleteItems = @"

delete from VAT_Source_Issue
where ID  in (
select ID from TempIssueData
where TempIssueData.ItemNo is null or TempIssueData.ItemNo = '0')
and Item_Code in (
select Item_Code from TempIssueData
where TempIssueData.ItemNo is null or TempIssueData.ItemNo = '0'
)

delete from TempIssueData where  TempIssueData.ItemNo is null or TempIssueData.ItemNo = '0'
";
                if (skipItems == "Y" && autoSave == "N")
                {
                    cmd.CommandText = deleteItems;
                    cmd.ExecuteNonQuery();
                }


                #endregion

                #region Select Data

                cmd.CommandText = getAll;

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable tempData = new DataTable();

                adapter.Fill(tempData);

                #endregion

                if (CompanyCode.ToUpper() == "FORMULATION".ToUpper())
                {
                    #region tempTable

                    string tempTable = @"
create table #Issuetemp(
SL varchar(6000),
ID varchar(6000),
BranchCode varchar(100),
Issue_DateTime varchar(100),
Reference_No varchar(100),
Comments varchar(500),
Return_Id varchar(500),
Post varchar(100),
Item_Code varchar(100),
Item_Name varchar(100),
Quantity decimal(25,9),
UOM varchar(100),
ItemNo varchar(100),
BranchId varchar(100),
BomId varchar(100),
UserId varchar(100),
UOMc varchar(100),
UOMn varchar(100),
ProductType varchar(100),
Transection_Type varchar(100)

)";

                    cmd.CommandText = tempTable;
                    cmd.ExecuteNonQuery();

                    commonDal.BulkInsert("#Issuetemp", tempData, currConn, transaction, 0, null, connVM);

                    #endregion

                    #region Product Type and Transection Type update

                    string ProductTypeUpdate = @"
update #Issuetemp set #Issuetemp.ProductType=pc.IsRaw from Products p 
left outer join ProductCategories pc on pc.CategoryID=p.CategoryID where p.ItemNo=#Issuetemp.ItemNo

update #Issuetemp set #Issuetemp.Transection_Type='TollitemIssueWithoutBOM' where #Issuetemp.ProductType='Toll'

";
                    cmd.CommandText = ProductTypeUpdate;
                    cmd.ExecuteNonQuery();

                    string TransectionTypeUpdate = @"
update #Issuetemp set #Issuetemp.Transection_Type=@Transection_Type where (#Issuetemp.Transection_Type is null or #Issuetemp.Transection_Type='')

";
                    cmd.CommandText = TransectionTypeUpdate;
                    cmd.Parameters.AddWithValue("@Transection_Type", transactionType);
                    cmd.ExecuteNonQuery();


                    string IDUpdate = @"
update #Issuetemp set #Issuetemp.ID=#Issuetemp.ID+'-Toll' where #Issuetemp.Transection_Type='TollitemIssueWithoutBOM'

";
                    cmd.CommandText = IDUpdate;
                    cmd.ExecuteNonQuery();


                    #endregion

                    #region Select all

                    getAll = @"
select 
 SL
,ID
,BranchCode
,Issue_DateTime
,Reference_No
,Comments
,Return_Id
,Post
,Item_Code
,Item_Name
,Quantity
,UOM
,ItemNo
,BranchId
,BomId
,UserId
,UOMc
,UOMn
,Transection_Type
from #Issuetemp
";


                    cmd.CommandText = getAll;

                    adapter = new SqlDataAdapter(cmd);
                    tempData = new DataTable();

                    adapter.Fill(tempData);

                    #endregion

                }

                retResults = SaveIssue(tempData, transactionType, CurrentUser, branchId, currConn, transaction, callBack, connVM, BranchCode);

                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                #endregion Commit
            }
            #endregion

            #region Catch and Finall
            catch (Exception ex)
            {
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Rollback();
                }
                retResults[0] = "Fail";//Success or Fail
                retResults[1] = ex.Message.ToString(); //catch ex
                //retResults[4] = ex.Message.ToString(); //catch ex
                //////if (transaction != null && Vtransaction == null) { transaction.Rollback(); }
                FileLogger.Log("IssueDAL", "SaveTempIssue", ex.ToString() + "\n" + sqlText);

                throw;
            }
            finally
            {
                if (VcurrConn == null)
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
            #endregion
            #region Results
            return retResults;
            #endregion
        }


        public string[] SaveVAT6_1_Permanent_Backup28082023(VAT6_1ParamVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string openingQuery = "";

            string insertSQLText = "";
            int Id = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = openingQuery; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Insert"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            ReportDSDAL reportDsdal = new ReportDSDAL();

            CommonDAL commonDal = new CommonDAL();
            #endregion

            #region try

            try
            {
                #region open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnectionNoPooling(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                #endregion open connection and transaction

                #region Opening

                openingQuery = reportDsdal.GetOpeningQuery_Backup28082023(ProcessConfig.Permanent);

                if (!string.IsNullOrEmpty(vm.ItemNo))
                {
                    openingQuery = openingQuery.Replace("@itemCondition", "and p.ItemNo = @ItemNo");
                }
                else
                {
                    openingQuery = openingQuery.Replace("@itemCondition", "");
                }

                SqlCommand cmd = new SqlCommand(openingQuery, currConn, transaction);
                cmd.CommandTimeout = 500;

                if (!string.IsNullOrEmpty(vm.ItemNo))
                {
                    cmd.Parameters.AddWithValue("@ItemNo", vm.ItemNo);
                }
                cmd.ExecuteNonQuery();


                #endregion

                #region Delete Existing Data

                string periodId = vm.StartDate.ToDateString("MMyyyy");
                string deleteText = @" delete from VAT6_1_Permanent where 
  TransType != 'Opening'  and  StartDateTime>=@StartDate and StartDateTime<dateadd(d,1,@EndDate)
 @itemCondition";

                if (!string.IsNullOrEmpty(vm.ItemNo))
                {
                    deleteText = deleteText.Replace("@itemCondition", "and ItemNo = @ItemNo");
                }
                else if (vm.FilterProcessItems)
                {
                    deleteText = deleteText.Replace("@itemCondition", " and ItemNo in (select ItemNo from Products where ProcessFlag='Y' and (ReportType='VAT6_1' or ReportType='VAT6_1_And_6_2'))");
                }
                else
                {
                    deleteText = deleteText.Replace("@itemCondition", "");
                }

                cmd.CommandText = deleteText;
                cmd.Parameters.AddWithValue("@StartDate", vm.StartDate);
                cmd.Parameters.AddWithValue("@EndDate", vm.EndDate);
                if (!string.IsNullOrEmpty(vm.ItemNo))
                {
                    cmd.Parameters.AddWithValueAndParamCheck("@ItemNo", vm.ItemNo);
                }

                cmd.ExecuteNonQuery();

                #endregion

                DateTime paramMonth = Convert.ToDateTime(vm.StartDate);

                vm.PermanentProcess = true;

                DataSet dsResult = reportDsdal.VAT6_1_WithConn_Backup28082023(vm, currConn, transaction, connVM);

                string updateMasterItem = @"
update VAT6_1
set ItemNo = P.MasterProductItemNo
from Products p inner join VAT6_1 vp
on p.ItemNo = vp.ItemNo and p.MasterProductItemNo is not null and LEN(p.MasterProductItemNo) >0
where UserId = @UserId

";

                cmd.CommandText = updateMasterItem;
                cmd.Parameters.AddWithValueAndParamCheck("@UserId", vm.UserId);

                cmd.ExecuteNonQuery();


                string insertToPermanent = reportDsdal.GetInsertQuery_Backup28082023(ProcessConfig.Permanent);

                string partitionQuery = reportDsdal.Get6_1PartitionQuery_Backup28082023(ProcessConfig.Permanent);

                insertToPermanent += "  " + partitionQuery;


                if (!string.IsNullOrEmpty(vm.ItemNo))
                {
                    insertToPermanent = insertToPermanent.Replace("@itemCondition1", " and ItemNo = @ItemNo");
                    insertToPermanent = insertToPermanent.Replace("@itemCondition2", " and VAT6_1_Permanent.ItemNo = @ItemNo");
                }
                else
                {
                    insertToPermanent = insertToPermanent.Replace("@itemCondition1", "");
                    insertToPermanent = insertToPermanent.Replace("@itemCondition2", "");
                }

                cmd.CommandText = insertToPermanent;

                if (!string.IsNullOrEmpty(vm.ItemNo))
                {
                    cmd.Parameters.AddWithValueAndParamCheck("@ItemNo", vm.ItemNo);
                }


                cmd.Parameters.AddWithValueAndParamCheck("@UserId", vm.UserId);
                cmd.Parameters.AddWithValueAndParamCheck("@PeriodId", periodId);

                cmd.ExecuteNonQuery();


                #region Update Negative

                string updateNegativeValue =
                    @"
update VAT6_1_Permanent set AvgRate = 0,RunningValue= 0, RunningOpeningValue=0
from (
select distinct ItemNo,min(StartDateTime)StartDateTime  from VAT6_1_Permanent 
where 1=1 and TransType not in ('opening') 
and RunningTotal<0
@itemCondition2

group by ItemNo
) a
where VAT6_1_Permanent.ItemNo = a.ItemNo and VAT6_1_Permanent.StartDateTime>= a.StartDateTime
@itemCondition2

";

                if (!string.IsNullOrEmpty(vm.ItemNo))
                {
                    updateNegativeValue = updateNegativeValue.Replace("@itemCondition2", " and VAT6_1_Permanent.ItemNo = @ItemNo");
                }
                else
                {
                    updateNegativeValue = updateNegativeValue.Replace("@itemCondition2", "");
                }

                cmd.CommandText = updateNegativeValue;

                if (!string.IsNullOrEmpty(vm.ItemNo))
                {
                    cmd.Parameters.AddWithValueAndParamCheck("@ItemNo", vm.ItemNo);
                }
                cmd.ExecuteNonQuery();


                #endregion

                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                #endregion Commit
            }
            #endregion

            #region Catch and Finall
            catch (Exception ex)
            {
                //transaction.Commit();

                retResults[0] = "Fail";//Success or Fail
                retResults[1] = ex.Message.ToString(); //catch ex
                retResults[4] = ex.Message.ToString(); //catch ex
                if (transaction != null && Vtransaction == null) { transaction.Rollback(); }
                FileLogger.Log("IssueDAL", "SaveTempIssue", ex.ToString() + "\n" + openingQuery);

                throw;
            }
            finally
            {
                if (VcurrConn == null)
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
            #endregion
            #region Results
            return retResults;
            #endregion
        }

        public string[] SaveVAT6_1_Permanent_Stored(VAT6_1ParamVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            ReportDSDAL reportDsdal = new ReportDSDAL();

            CommonDAL commonDal = new CommonDAL();
            #endregion

            #region try

            try
            {
                #region open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnectionNoPooling(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }


                string dbCreate = @"

DECLARE @DatabaseName nvarchar(50)
SET @DatabaseName = N'VAT_Process61'

DECLARE @SQL varchar(max)

SELECT @SQL = COALESCE(@SQL,'') + 'Kill ' + Convert(varchar, SPId) + ';'
FROM MASTER..SysProcesses
WHERE DBId = DB_ID(@DatabaseName) AND SPId <> @@SPId

--SELECT @SQL 
EXEC(@SQL)


          DROP DATABASE IF EXISTS [VAT_Process61]

            IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = 'VAT_Process61')
                    BEGIN
                        CREATE DATABASE [VAT_Process61]
                    END

";

                string tableCreate = @"
IF NOT EXISTS(SELECT *
                      FROM INFORMATION_SCHEMA.TABLES
                      WHERE TABLE_SCHEMA = 'dbo'
                        AND TABLE_NAME = 'VATTemp_16'
                        AND TABLE_CATALOG = 'VAT_Process61'
            )
            BEGIN

CREATE TABLE VAT_Process61.dbo.VATTemp_16([SerialNo] [varchar] (2) NULL,[Dailydate] [datetime] NULL,[InvoiceDateTime] [datetime] NULL,
[TransID] [varchar](200) NULL,	[TransType] [varchar](200) NULL,[BENumber] [varchar](200) NULL,
[ItemNo] [varchar](200) NULL,	[UnitCost] [decimal](25, 9) NULL,
[Quantity] [decimal](25, 9) NULL,	[VATRate] [decimal](25, 9) NULL,	[SD] [decimal](25, 9) NULL,[Remarks] [varchar](200) NULL,[CreateDateTime] [datetime] NULL
,TransactionType [varchar] (200) , BranchId int 
)


               
CREATE TABLE VAT_Process61.dbo.[VAT6_1_Permanent](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SerialNo] [varchar](200) NULL,
	[ItemNo] [varchar](50) NULL,
	[StartDateTime] [datetime] NULL,
	[StartingQuantity] [decimal](25, 9) NULL,
	[StartingAmount] [decimal](25, 9) NULL,
	[VendorID] [varchar](50) NULL,
	[SD] [decimal](25, 9) NULL,
	[VATRate] [decimal](25, 9) NULL,
	[Quantity] [decimal](25, 9) NULL,
	[UnitCost] [decimal](25, 9) NULL,
	[TransID] [varchar](50) NULL,
	[TransType] [varchar](50) NULL,
	[BENumber] [varchar](300) NULL,
	[InvoiceDateTime] [datetime] NULL,
	[Remarks] [varchar](100) NULL,
	[CreateDateTime] [datetime] NULL,
	[TransactionType] [varchar](50) NULL,
	[BranchId] [varchar](50) NULL,
	[UserId] [varchar](50) NULL,
	[AvgRate] [decimal](25, 9) NULL,
	[PeriodID] [varchar](50) NULL,
	[RunningTotal] [decimal](25, 9) NULL,
	[RunningValue] [decimal](25, 9) NULL,
	[RunningOpeningValue] [decimal](25, 9) NULL,
	[RunningOpeningQuantity] [decimal](25, 9) NULL,
 CONSTRAINT [PK_VAT6_1_p] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
				
				            END

";

                SqlCommand cmdDbCommand = new SqlCommand(dbCreate, currConn);
                cmdDbCommand.ExecuteNonQuery();
                cmdDbCommand.CommandText = tableCreate;
                cmdDbCommand.ExecuteNonQuery();


                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                #endregion open connection and transaction

                #region Opening

                sqlText = "spInsertOpening_61";

                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValueAndParamCheck("@UserId", vm.UserId);
                cmd.CommandTimeout = 0;

                cmd.ExecuteNonQuery();



                #endregion

                foreach (DateTime dateTime in OrdinaryVATDesktop.EachDay(Convert.ToDateTime(vm.StartDate),
                             Convert.ToDateTime(vm.EndDate)))
                {

                    cmd.Parameters.Clear();

                    #region Delete Existing Data

                    string deleteText = @"spDelete6_1";

                    cmd.CommandText = deleteText;
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValueAndParamCheck("@StartDate", dateTime.ToString("yyyy-MM-dd 00:00:00"));
                    cmd.Parameters.AddWithValueAndParamCheck("@EndDate", dateTime.ToString("yyyy-MM-dd 23:59:59"));

                    if (!string.IsNullOrEmpty(vm.ItemNo))
                    {
                        cmd.Parameters.AddWithValueAndParamCheck("@ItemNo", vm.ItemNo);

                    }
                    else if (vm.FilterProcessItems)
                    {
                        cmd.Parameters.AddWithValueAndParamCheck("@FilteredItems", vm.FilterProcessItems ? "Y" : "N");
                    }

                    cmd.ExecuteNonQuery();


                    cmd.CommandType = CommandType.StoredProcedure;

                    #endregion

                    DateTime paramMonth = Convert.ToDateTime(dateTime.ToString("yyyy-MM-dd 00:00:00"));

                    vm.PermanentProcess = true;

                    cmd.Parameters.Clear();

                    cmd.CommandText = "spVAT6_1";
                    cmd.Parameters.AddWithValueAndParamCheck("@StartDate", dateTime.ToString("yyyy-MM-dd 00:00:00"));
                    cmd.Parameters.AddWithValueAndParamCheck("@EndDate", dateTime.ToString("yyyy-MM-dd 23:59:59"));
                    cmd.Parameters.AddWithValueAndParamCheck("@ItemNo", vm.ItemNo);
                    cmd.Parameters.AddWithValueAndParamCheck("@Opening", vm.Opening);
                    cmd.Parameters.AddWithValueAndParamCheck("@VAT6_2_1", vm.VAT6_2_1);
                    cmd.Parameters.AddWithValueAndParamCheck("@stockMovement", vm.StockMovement);
                    cmd.Parameters.AddWithValueAndParamCheck("@UserId", vm.UserId);
                    cmd.Parameters.AddWithValueAndParamCheck("@TotalIncludeSD", true);
                    cmd.Parameters.AddWithValueAndParamCheck("@othervalue", true);

                    cmd.ExecuteNonQuery();

                    #region Order By Insert

                    sqlText = @"    

delete from [VAT_Process61].[dbo].VAT6_1_Permanent

INSERT INTO [VAT_Process61].[dbo].VAT6_1_Permanent([SerialNo]
      ,[StartDateTime]
      ,[InvoiceDateTime]
      ,[TransID]
      ,[TransType]
      ,[BENumber]
      ,[ItemNo]
      ,[UnitCost]
      ,[Quantity]
      ,[VATRate]
      ,[SD]
      ,[Remarks]
      ,[CreateDateTime]
      ,[TransactionType]
      , UserId
      , BranchId
)
select * from (
SELECT   distinct 

       'A' [SerialNo]
      ,@StartDate [Dailydate]
      ,@StartDate [InvoiceDateTime]
      ,''[TransID]
      ,'MonthOpening'[TransType]
      ,'' [BENumber]
      ,[ItemNo]
      ,0 [UnitCost]
      ,0 [Quantity]
      ,0 [VATRate]
      ,0 [SD]
      ,'MonthOpening' [Remarks]
      ,@StartDate [CreateDateTime]
      ,'MonthOpening' [TransactionType]
      ,@UserId UserId
      ,@BranchId BranchId
from [VAT_Process61].[dbo].VATTemp_16 

union all


    SELECT [SerialNo]
      ,[Dailydate]
      ,[InvoiceDateTime]
      ,[TransID]
      ,[TransType]
      ,[BENumber]
      ,[ItemNo]
      ,[UnitCost]
      ,[Quantity]
      ,[VATRate]
      ,[SD]
      ,[Remarks]
      ,[CreateDateTime]
      ,[TransactionType]
,@UserId UserId
,@BranchId BranchId
    FROM [VAT_Process61].[dbo].VATTemp_16

) as VAT6_1
    ORDER BY ItemNo, dailydate, SerialNo
    OPTION (OPTIMIZE FOR UNKNOWN)


      UPDATE [VAT_Process61].[dbo].VAT6_1_Permanent SET StartDateTime=@StartDate WHERE SerialNo = 'A'
    
  
";


                    #endregion

                    cmd.Parameters.Clear();

                    cmd.CommandText = sqlText;
                    cmd.CommandType = CommandType.Text;

                    cmd.Parameters.AddWithValueAndParamCheck("@post1", "Y");
                    cmd.Parameters.AddWithValueAndParamCheck("@post2", "Y");
                    cmd.Parameters.AddWithValueAndParamCheck("@BranchId", 0);
                    cmd.Parameters.AddWithValueAndParamCheck("@StartDate", dateTime.ToString("yyyy-MM-dd 00:00:00"));
                    cmd.Parameters.AddWithValueAndParamCheck("@UserId", vm.UserId);

                    cmd.ExecuteNonQuery();

                    // Partiton
                    #region Partition Query

                    string insertToPermanent = @"


update [VAT_Process61].[dbo].VAT6_1_Permanent
set ItemNo = P.MasterProductItemNo
from Products p inner join [VAT_Process61].[dbo].VAT6_1_Permanent vp
on p.ItemNo = vp.ItemNo and p.MasterProductItemNo is not null and LEN(p.MasterProductItemNo) >0




UPDATE [VAT_Process61].[dbo].VAT6_1_Permanent
SET
    Quantity=a.ClosingQty,
    UnitCost=a.UnitCost,
    RunningTotal=a.ClosingQty,
    RunningValue=a.ClosingValue

FROM (
SELECT VAT6_1_Permanent.Id,
             VAT6_1_Permanent.ItemNo,
             RunningTotal           ClosingQty,
             RunningValue ClosingValue,
			 UnitCost

      FROM VAT6_1_Permanent
               RIGHT OUTER JOIN (SELECT DISTINCT ItemNo, MAX(Id) Id
                                 FROM VAT6_1_Permanent
                                 WHERE StartDateTime < @StartDate
                                 GROUP BY ItemNo) AS a
                                ON a.Id = VAT6_1_Permanent.ID) AS a
WHERE a.ItemNo = [VAT_Process61].[dbo].VAT6_1_Permanent.ItemNo
  AND [VAT_Process61].[dbo].VAT6_1_Permanent.TransType = 'MonthOpening'


update VAT_Process61.dbo.VAT6_1_Permanent set AvgRate = 0 where 1=1 @itemCondition2

update VAT_Process61.dbo.VAT6_1_Permanent set  AvgRate=ProductAvgPrice.AvgPrice,RunningTotal=0
from ProductAvgPrice
where ProductAvgPrice.ItemNo=VAT_Process61.dbo.VAT6_1_Permanent.ItemNo
and VAT_Process61.dbo.VAT6_1_Permanent.TransID=ProductAvgPrice.PurchaseNo
@itemCondition2

update VAT_Process61.dbo.VAT6_1_Permanent
set  AvgRate=pap.AvgPrice,RunningTotal=0
from VAT_Process61.dbo.VAT6_1_Permanent id inner join 
ProductAvgPrice pap
on id.ItemNo = pap.ItemNo
and cast(id.StartDateTime as date) = pap.AgvPriceDate
and pap.TransactionType = 'Issue'
@itemCondition2


create table #Temp(SL int identity(1,1),Id int,ItemNo varchar(100),TransType varchar(100)
,Quantity decimal(25,9),UnitCost decimal(25,9) ) -- ,BranchId int

insert into #Temp(Id,ItemNo,TransType,Quantity,UnitCost )
select Id,ItemNo,TransType,Quantity,UnitCost from VAT_Process61.dbo.VAT6_1_Permanent
where 1=1 @itemCondition2
order by  ItemNo,StartDateTime,SerialNo,TransID

update VAT_Process61.dbo.VAT6_1_Permanent set  RunningTotal=RT.RunningTotal
from (SELECT id,SL, ItemNo, TransType  ,Quantity,
SUM (case when TransType in('Issue') then -1*Quantity else Quantity end ) 
OVER (PARTITION BY  ItemNo ORDER BY SL) AS RunningTotal
FROM #Temp)RT
where RT.Id=VAT_Process61.dbo.VAT6_1_Permanent.Id @itemCondition3 --branch



Update VAT_Process61.dbo.VAT6_1_Permanent set RunningValue = AvgRate*RunningTotal where 1=1  @itemCondition2
update VAT_Process61.dbo.VAT6_1_Permanent set RunningTotal=Quantity, RunningValue=UnitCost where TransType in('opening') @itemCondition2

update VAT_Process61.dbo.VAT6_1_Permanent set  RunningOpeningQuantity=0,RunningOpeningValue=0 where 1=1 @itemCondition2
";
                    #endregion

                    if (!string.IsNullOrEmpty(vm.ItemNo))
                    {
                        insertToPermanent = insertToPermanent.Replace("@itemCondition2", " and VAT_Process61.dbo.VAT6_1_Permanent.ItemNo = @ItemNo");
                        insertToPermanent = insertToPermanent.Replace("@itemCondition3", " and VAT_Process61.dbo.VAT6_1_Permanent.ItemNo = @ItemNo");
                    }
                    else
                    {
                        insertToPermanent = insertToPermanent.Replace("@itemCondition1", "");
                        insertToPermanent = insertToPermanent.Replace("@itemCondition2", "");
                        insertToPermanent = insertToPermanent.Replace("@itemCondition3", "");
                    }


                    cmd.CommandType = CommandType.Text;

                    cmd.Parameters.AddWithValueAndParamCheck("@StartDate", dateTime.ToString("yyyy-MM-dd 00:00:00"));
                    cmd.Parameters.AddWithValueAndParamCheck("@EndDate", dateTime.ToString("yyyy-MM-dd 23:59:59"));

                    cmd.CommandText = insertToPermanent;
                    cmd.ExecuteNonQuery();


                    // insert back to main db

                    string insertBackToMain = @"

INSERT INTO VAT6_1_Permanent( [SerialNo]
      ,[ItemNo]
      ,[StartDateTime]
      ,[StartingQuantity]
      ,[StartingAmount]
      ,[VendorID]
      ,[SD]
      ,[VATRate]
      ,[Quantity]
      ,[UnitCost]
      ,[TransID]
      ,[TransType]
      ,[BENumber]
      ,[InvoiceDateTime]
      ,[Remarks]
      ,[CreateDateTime]
      ,[TransactionType]
      ,[BranchId]
      ,[UserId]
      ,[AvgRate]
      ,[PeriodID]
      ,[RunningTotal]
      ,[RunningValue]
      ,[RunningOpeningValue]
      ,[RunningOpeningQuantity])

Select 
 [SerialNo]
      ,[ItemNo]
      ,[StartDateTime]
      ,[StartingQuantity]
      ,[StartingAmount]
      ,[VendorID]
      ,[SD]
      ,[VATRate]
      ,[Quantity]
      ,[UnitCost]
      ,[TransID]
      ,[TransType]
      ,[BENumber]
      ,[InvoiceDateTime]
      ,[Remarks]
      ,[CreateDateTime]
      ,[TransactionType]
      ,[BranchId]
      ,[UserId]
      ,[AvgRate]
      ,[PeriodID]
      ,[RunningTotal]
      ,[RunningValue]
      ,[RunningOpeningValue]
      ,[RunningOpeningQuantity]
from [VAT_Process61].[dbo].VAT6_1_Permanent
where [VAT_Process61].[dbo].VAT6_1_Permanent.TransType != 'MonthOpening'
Order by  ItemNo, StartDateTime,SerialNo


";
                    cmd.CommandText = insertBackToMain;
                    cmd.ExecuteNonQuery();


                }




                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                #endregion Commit


            }
            #endregion

            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[1] = ex.Message.ToString(); //catch ex
                retResults[4] = ex.Message.ToString(); //catch ex
                if (transaction != null && Vtransaction == null)
                {
                    transaction.Rollback();
                    //transaction.Commit();
                }
                FileLogger.Log("IssueDAL", "SaveTempIssue", ex.ToString() + "\n" + sqlText);

                throw;
            }
            finally
            {
                if (VcurrConn == null)
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
            #endregion
            #region Results
            return retResults;
            #endregion
        }

        public string[] SaveVAT6_1_Permanent_DayWise(VAT6_1ParamVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            ReportDSDAL reportDsdal = new ReportDSDAL();

            CommonDAL commonDal = new CommonDAL();
            #endregion


            #region try

            try
            {               
                #region open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }
               
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnectionNoPooling(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }


                string dbCreate = @"

DECLARE @DatabaseName nvarchar(50)
SET @DatabaseName = N'VAT_Process'

DECLARE @SQL varchar(max)

SELECT @SQL = COALESCE(@SQL,'') + 'Kill ' + Convert(varchar, SPId) + ';'
FROM MASTER..SysProcesses
WHERE DBId = DB_ID(@DatabaseName) AND SPId <> @@SPId

--SELECT @SQL 
EXEC(@SQL)


          DROP DATABASE IF EXISTS [VAT_Process]

            IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = 'VAT_Process')
                    BEGIN
                        CREATE DATABASE [VAT_Process]
                    END

";

                string tableCreate = @"
IF NOT EXISTS(SELECT *
                      FROM INFORMATION_SCHEMA.TABLES
                      WHERE TABLE_SCHEMA = 'dbo'
                        AND TABLE_NAME = 'VATTemp_17_1'
                        AND TABLE_CATALOG = 'VAT_Process'
            )
            BEGIN
                CREATE TABLE VAT_Process.[dbo].VATTemp_17_1
                (
                    SerialNo        VARCHAR(10)    NULL,
                    Dailydate       DATETIME       NULL,
                    TransID         VARCHAR(200)   NULL,
                    TransType       VARCHAR(200)   NULL,
                    ItemNo          VARCHAR(200)   NULL,
                    UnitCost        DECIMAL(25, 9) NULL,
                    Quantity        DECIMAL(25, 9) NULL,
                    VATRate         DECIMAL(25, 9) NULL,
                    SD              DECIMAL(25, 9) NULL,
                    Remarks         VARCHAR(200),
                    CreatedDateTime DATETIME       NULL,
                    UnitRate        DECIMAL(25, 9),
                    AdjustmentValue DECIMAL(25, 9)
                )


                CREATE TABLE VAT_Process.[dbo].[VAT6_2_Permanent]
                (
                    [ID]                          [int] IDENTITY (1,1) NOT NULL,
                    [SerialId]                          [int] ,
                    [SerialNo]                    [varchar](200)       NULL,
                    [StartDateTime]               [datetime]           NULL,
                    [StartingQuantity]            [decimal](25, 9)     NULL,
                    [StartingAmount]              [decimal](25, 9)     NULL,
                    [TransID]                     [varchar](200)       NULL,
                    [TransType]                   [varchar](200)       NULL,
                    [CustomerName]                [varchar](200)       NULL,
                    [Address1]                    [varchar](500)       NULL,
                    [Address2]                    [varchar](500)       NULL,
                    [Address3]                    [varchar](500)       NULL,
                    [VATRegistrationNo]           [varchar](500)       NULL,
                    [ProductName]                 [varchar](500)       NULL,
                    [ProductCode]                 [varchar](500)       NULL,
                    [UOM]                         [varchar](50)        NULL,
                    [HSCodeNo]                    [varchar](500)       NULL,
                    [Quantity]                    [decimal](25, 9)     NULL,
                    [VATRate]                     [decimal](25, 9)     NULL,
                    [SD]                          [decimal](25, 9)     NULL,
                    [UnitCost]                    [decimal](25, 9)     NULL,
                    [Remarks]                     [varchar](200)       NULL,
                    [CreatedDateTime]             [datetime]           NULL,
                    [UnitRate]                    [decimal](25, 9)     NULL,
                    [ItemNo]                      [varchar](50)        NULL,
                    [AdjustmentValue]             [decimal](25, 9)     NULL,
                    [UserId]                      [varchar](50)        NULL,
                    [BranchId]                    [varchar](50)        NULL,
                    [CustomerID]                  [varchar](50)        NULL,
                    [ProductDesc]                 [varchar](500)       NULL,
                    [ClosingRate]                 [decimal](25, 9)     NULL,
                    [PeriodId]                    [varchar](50)        NULL,
                    [RunningTotal]                [decimal](18, 8)     NULL,
                    [RunningTotalValue]           [decimal](18, 8)     NULL,
                    [RunningTotalValueFinal]      [decimal](18, 8)     NULL,
                    [DeclaredPrice]               [decimal](25, 9)     NULL,
                    [RunningOpeningValueFinal]    [decimal](18, 8)     NULL,
                    [RunningOpeningQuantityFinal] [decimal](18, 8)     NULL,
                    PRIMARY KEY CLUSTERED
                        (
                         [ID] ASC
                            ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                ) ON [PRIMARY]
				
				            END

";

                SqlCommand cmdDbCommand = new SqlCommand(dbCreate, currConn, transaction);
                cmdDbCommand.ExecuteNonQuery();
                cmdDbCommand.CommandText = tableCreate;
                cmdDbCommand.ExecuteNonQuery();


                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                #endregion open connection and transaction

                sqlText = @"
delete from VAT6_1_Permanent_DayWise where StartDateTime >= @StartDateTime 
                and StartDateTime <= @EndDateTime

";

                if (!string.IsNullOrEmpty(vm.ItemNo))
                {
                    sqlText += " and ItemNo='" + vm.ItemNo + "'";
                }

                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
                cmd.Parameters.AddWithValue("@StartDateTime", vm.StartDate);
                cmd.Parameters.AddWithValue("@EndDateTime", vm.EndDate);
                cmd.ExecuteNonQuery();


                sqlText = @"
insert into VAT6_1_Permanent_DayWise
(
[SerialNo]
      ,[ItemNo]
      ,[StartDateTime]
      ,[StartingQuantity]
      ,[StartingAmount]
      ,[VendorID]
      ,[SD]
      ,[VATRate]
      ,[Quantity]
      ,[UnitCost]
      ,[TransID]
      ,[TransType]
      ,[BENumber]
      ,[InvoiceDateTime]
      ,[Remarks]
      ,[CreateDateTime]
      ,[TransactionType]
      ,[BranchId]
      ,[UserId]
      ,[AvgRate]
      ,[PeriodID]
      ,[RunningTotal]
      ,[RunningValue]
      ,[RunningOpeningValue]
      ,[RunningOpeningQuantity]
	)



Select * from 
(select 
      distinct 
        'A' [SerialNo]
      ,[ItemNo]
      ,@StartDateTime [StartDateTime]
      ,0 [StartingQuantity]
      ,0 [StartingAmount]
      ,''[VendorID]
      ,0 [SD]
      ,0 [VATRate]
      ,0 [Quantity]
      ,0 [UnitCost]
      ,''[TransID]
      ,'MonthOpening'[TransType]
      ,''[BENumber]
      ,'1900-01-01' [InvoiceDateTime]
      ,'' [Remarks]
      ,'1900-01-01' [CreateDateTime]
      ,'MonthOpening'[TransactionType]
      ,0 [BranchId]
      ,0 [UserId]
      ,0 [AvgRate]
      ,0 [PeriodID]
      ,0 [RunningTotal]
      ,0 [RunningValue]
      ,0 [RunningOpeningValue]
      ,0 [RunningOpeningQuantity]
	  from VAT6_1_Permanent
	  where StartDateTime >= @StartDateTime 
	  and StartDateTime <= @EndDateTime

	union all
	
select 
      [SerialNo]
      ,[ItemNo]
      ,CAST(StartDateTime AS DATE) [StartDateTime]
      ,0 [StartingQuantity]
      ,0 [StartingAmount]
      ,''[VendorID]
      ,0 [SD]
      ,0 [VATRate]
      ,sum([Quantity])Quantity
      ,sum([UnitCost])UnitCost
      ,''[TransID]
      ,[TransType]
      ,''[BENumber]
      ,''[InvoiceDateTime]
      ,''[Remarks]
      ,''[CreateDateTime]
      ,'' [TransactionType]
      ,'' [BranchId]
      ,'' [UserId]
      ,avg(AvgRate)AvgRate
      ,0 [PeriodID]
      ,0 [RunningTotal]
      ,0 [RunningValue]
      ,0 [RunningOpeningValue]
      ,0 [RunningOpeningQuantity]
	  from VAT6_1_Permanent
	  where StartDateTime >= @StartDateTime 
	  and StartDateTime <= @EndDateTime
	  group by 
	  ItemNo, CAST(StartDateTime AS DATE)
	  , SerialNo
	  ,TransType
	  ) VAT6_1_Day

	  order by 	ItemNo, StartDateTime
	  , SerialNo
";


                cmd.CommandText = sqlText;
                cmd.CommandTimeout = 500;
                cmd.ExecuteNonQuery();



                string insertToPermanent = @"

UPDATE VAT6_1_Permanent_DayWise
SET
    Quantity=a.ClosingQty,
    UnitCost=a.UnitCost,
    RunningTotal=a.ClosingQty,
    RunningValue=a.ClosingValue

FROM (
SELECT VAT6_1_Permanent.Id,
             VAT6_1_Permanent.ItemNo,
             RunningTotal           ClosingQty,
             RunningValue ClosingValue,
			 UnitCost

      FROM VAT6_1_Permanent
               RIGHT OUTER JOIN (SELECT DISTINCT ItemNo, MAX(Id) Id
                                 FROM VAT6_1_Permanent
                                 WHERE StartDateTime < @StartDate
                                 GROUP BY ItemNo) AS a
                                ON a.Id = VAT6_1_Permanent.ID) AS a
WHERE a.ItemNo = [VAT6_1_Permanent_DayWise].ItemNo
  AND VAT6_1_Permanent_DayWise.TransType = 'MonthOpening'
  AND VAT6_1_Permanent_DayWise.StartDateTime = @StartDate


--update VAT6_1_Permanent_DayWise set AvgRate = 0 where 1=1 @itemCondition2

--update VAT6_1_Permanent_DayWise set  AvgRate=ProductAvgPrice.AvgPrice,RunningTotal=0
--from ProductAvgPrice
--where ProductAvgPrice.ItemNo=VAT6_1_Permanent_DayWise.ItemNo
--and VAT6_1_Permanent_DayWise.TransID=ProductAvgPrice.PurchaseNo
--@itemCondition2

update VAT6_1_Permanent_DayWise
set  AvgRate=pap.AvgPrice,RunningTotal=0
from VAT6_1_Permanent_DayWise id inner join 
ProductAvgPrice pap
on id.ItemNo = pap.ItemNo
and cast(id.StartDateTime as date) = pap.AgvPriceDate
--and pap.TransactionType = 'Issue'
where
 id.StartDateTime >= @StartDateTime
and id.StartDateTime <= @EndDateTime
@itemCondition2


create table #Temp(SL int identity(1,1),Id int,ItemNo varchar(100),TransType varchar(100)
,Quantity decimal(25,9),UnitCost decimal(25,9) ) -- ,BranchId int

insert into #Temp(Id,ItemNo,TransType,Quantity,UnitCost )
select Id,ItemNo,TransType,Quantity,UnitCost from VAT6_1_Permanent_DayWise
where 1=1 
and StartDateTime >= @StartDateTime
and StartDateTime <= @EndDateTime
@itemCondition2
order by  ItemNo,StartDateTime,SerialNo,TransID

update VAT6_1_Permanent_DayWise set  RunningTotal=RT.RunningTotal
from (SELECT id,SL, ItemNo, TransType  ,Quantity,
SUM (case when TransType in('Issue') then -1*Quantity else Quantity end ) 
OVER (PARTITION BY  ItemNo ORDER BY SL) AS RunningTotal
FROM #Temp)RT
where RT.Id=VAT6_1_Permanent_DayWise.Id @itemCondition3 --branch



Update VAT6_1_Permanent_DayWise set RunningValue = AvgRate*RunningTotal where 1=1  @itemCondition2
update VAT6_1_Permanent_DayWise set RunningTotal=Quantity, RunningValue=UnitCost where TransType in('opening') @itemCondition2

update VAT6_1_Permanent_DayWise set  RunningOpeningQuantity=0,RunningOpeningValue=0 where 1=1 @itemCondition2

";
                if (!string.IsNullOrEmpty(vm.ItemNo))
                {
                    insertToPermanent = insertToPermanent.Replace("@itemCondition2", " and VAT6_1_Permanent_DayWise.ItemNo = @ItemNo");
                    insertToPermanent = insertToPermanent.Replace("@itemCondition3", " and VAT6_1_Permanent_DayWise.ItemNo = @ItemNo");
                }
                else
                {
                    insertToPermanent = insertToPermanent.Replace("@itemCondition1", "");
                    insertToPermanent = insertToPermanent.Replace("@itemCondition2", "");
                    insertToPermanent = insertToPermanent.Replace("@itemCondition3", "");
                }


                cmd.CommandText = insertToPermanent;

                cmd.Parameters.AddWithValue("@StartDate", vm.StartDate);
                cmd.Parameters.AddWithValue("@EndDate", vm.EndDate);
                cmd.ExecuteNonQuery();

                #region Commit
                //if (Vtransaction == null)
                //{
                //    if (transaction != null)
                //    {
                //        transaction.Commit();
                //    }
                //}
                #endregion Commit

            }
            #endregion

            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
                if (transaction != null && Vtransaction == null)
                {
                    transaction.Rollback();
                }
                FileLogger.Log("IssueDAL", "SaveTempIssue", ex.ToString() + "\n" + sqlText);

                throw;
            }
            finally
            {
                if (VcurrConn == null)
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
            #endregion
            #region Results
            return retResults;
            #endregion
        }

        public string[] SaveVAT6_1_Permanent_Stored_Branch(VAT6_1ParamVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            ReportDSDAL reportDsdal = new ReportDSDAL();

            CommonDAL commonDal = new CommonDAL();
            #endregion

            #region try

            try
            {
                #region open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnectionNoPooling(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }


                string dbCreate = @"

DECLARE @DatabaseName nvarchar(50)
SET @DatabaseName = N'VAT_Process61'

DECLARE @SQL varchar(max)

SELECT @SQL = COALESCE(@SQL,'') + 'Kill ' + Convert(varchar, SPId) + ';'
FROM MASTER..SysProcesses
WHERE DBId = DB_ID(@DatabaseName) AND SPId <> @@SPId

--SELECT @SQL 
EXEC(@SQL)


          DROP DATABASE IF EXISTS [VAT_Process61]

            IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = 'VAT_Process61')
                    BEGIN
                        CREATE DATABASE [VAT_Process61]
                    END

";

                string tableCreate = @"
IF NOT EXISTS(SELECT *
                      FROM INFORMATION_SCHEMA.TABLES
                      WHERE TABLE_SCHEMA = 'dbo'
                        AND TABLE_NAME = 'VATTemp_16'
                        AND TABLE_CATALOG = 'VAT_Process61'
            )
            BEGIN

CREATE TABLE VAT_Process61.dbo.VATTemp_16([SerialNo] [varchar] (2) NULL,[Dailydate] [datetime] NULL,[InvoiceDateTime] [datetime] NULL,
[TransID] [varchar](200) NULL,	[TransType] [varchar](200) NULL,[BENumber] [varchar](200) NULL,
[ItemNo] [varchar](200) NULL,	[UnitCost] [decimal](25, 9) NULL,
[Quantity] [decimal](25, 9) NULL,	[VATRate] [decimal](25, 9) NULL,	[SD] [decimal](25, 9) NULL,[Remarks] [varchar](200) NULL,[CreateDateTime] [datetime] NULL
,TransactionType [varchar] (200)  
,BranchId int null
)


               
CREATE TABLE VAT_Process61.dbo.[VAT6_1_Permanent](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SerialNo] [varchar](200) NULL,
	[ItemNo] [varchar](50) NULL,
	[StartDateTime] [datetime] NULL,
	[StartingQuantity] [decimal](25, 9) NULL,
	[StartingAmount] [decimal](25, 9) NULL,
	[VendorID] [varchar](50) NULL,
	[SD] [decimal](25, 9) NULL,
	[VATRate] [decimal](25, 9) NULL,
	[Quantity] [decimal](25, 9) NULL,
	[UnitCost] [decimal](25, 9) NULL,
	[TransID] [varchar](50) NULL,
	[TransType] [varchar](50) NULL,
	[BENumber] [varchar](300) NULL,
	[InvoiceDateTime] [datetime] NULL,
	[Remarks] [varchar](100) NULL,
	[CreateDateTime] [datetime] NULL,
	[TransactionType] [varchar](50) NULL,
	[BranchId] [varchar](50) NULL,
	[UserId] [varchar](50) NULL,
	[AvgRate] [decimal](25, 9) NULL,
	[PeriodID] [varchar](50) NULL,
	[RunningTotal] [decimal](25, 9) NULL,
	[RunningValue] [decimal](25, 9) NULL,
	[RunningOpeningValue] [decimal](25, 9) NULL,
	[RunningOpeningQuantity] [decimal](25, 9) NULL,
 CONSTRAINT [PK_VAT6_1_p] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
				
				            END

";

                SqlCommand cmdDbCommand = new SqlCommand(dbCreate, currConn, transaction);
                cmdDbCommand.ExecuteNonQuery();
                cmdDbCommand.CommandText = tableCreate;
                cmdDbCommand.ExecuteNonQuery();


                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                #endregion open connection and transaction

                #region Opening

                sqlText = "spInsertOpening_61_Branch";

                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValueAndParamCheck("@UserId", vm.UserId);
                cmd.CommandTimeout = 0;

                cmd.ExecuteNonQuery();

                #endregion

                foreach (DateTime dateTime in OrdinaryVATDesktop.EachDay(Convert.ToDateTime(vm.StartDate),
                             Convert.ToDateTime(vm.EndDate)))
                {

                    cmd.Parameters.Clear();

                    #region Delete Existing Data

                    string deleteText = @"spDelete6_1_Branch";

                    cmd.CommandText = deleteText;
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValueAndParamCheck("@StartDate", dateTime.ToString("yyyy-MM-dd 00:00:00"));
                    cmd.Parameters.AddWithValueAndParamCheck("@EndDate", dateTime.ToString("yyyy-MM-dd 23:59:59"));

                    if (!string.IsNullOrEmpty(vm.ItemNo))
                    {
                        cmd.Parameters.AddWithValueAndParamCheck("@ItemNo", vm.ItemNo);

                    }
                    else if (vm.FilterProcessItems)
                    {
                        cmd.Parameters.AddWithValueAndParamCheck("@FilteredItems", vm.FilterProcessItems ? "Y" : "N");
                    }

                    cmd.ExecuteNonQuery();

                    cmd.CommandType = CommandType.StoredProcedure;

                    #endregion

                    DateTime paramMonth = Convert.ToDateTime(dateTime.ToString("yyyy-MM-dd 00:00:00"));

                    vm.PermanentProcess = true;

                    cmd.Parameters.Clear();

                    cmd.CommandText = "spVAT6_1";
                    cmd.Parameters.AddWithValueAndParamCheck("@StartDate", dateTime.ToString("yyyy-MM-dd 00:00:00"));
                    cmd.Parameters.AddWithValueAndParamCheck("@EndDate", dateTime.ToString("yyyy-MM-dd 23:59:59"));
                    cmd.Parameters.AddWithValueAndParamCheck("@ItemNo", vm.ItemNo);
                    cmd.Parameters.AddWithValueAndParamCheck("@Opening", vm.Opening);
                    cmd.Parameters.AddWithValueAndParamCheck("@VAT6_2_1", vm.VAT6_2_1);
                    cmd.Parameters.AddWithValueAndParamCheck("@stockMovement", vm.StockMovement);
                    cmd.Parameters.AddWithValueAndParamCheck("@UserId", vm.UserId);
                    cmd.Parameters.AddWithValueAndParamCheck("@TotalIncludeSD", true);
                    cmd.Parameters.AddWithValueAndParamCheck("@othervalue", true);

                    cmd.ExecuteNonQuery();

                    #region Order By Insert

                    sqlText = @"    

delete from [VAT_Process61].[dbo].VAT6_1_Permanent

INSERT INTO [VAT_Process61].[dbo].VAT6_1_Permanent([SerialNo]
      ,[StartDateTime]
      ,[InvoiceDateTime]
      ,[TransID]
      ,[TransType]
      ,[BENumber]
      ,[ItemNo]
      ,[UnitCost]
      ,[Quantity]
      ,[VATRate]
      ,[SD]
      ,[Remarks]
      ,[CreateDateTime]
      ,[TransactionType]
      , UserId
      , BranchId
)
select * from (
SELECT   distinct 

       'A' [SerialNo]
      ,@StartDate [Dailydate]
      ,@StartDate [InvoiceDateTime]
      ,''[TransID]
      ,'MonthOpening'[TransType]
      ,'' [BENumber]
      ,[ItemNo]
      ,0 [UnitCost]
      ,0 [Quantity]
      ,0 [VATRate]
      ,0 [SD]
      ,'MonthOpening' [Remarks]
      ,@StartDate [CreateDateTime]
      ,'MonthOpening' [TransactionType]
      ,@UserId UserId
      , BranchId
from [VAT_Process61].[dbo].VATTemp_16 

union all


    SELECT [SerialNo]
      ,[Dailydate]
      ,[InvoiceDateTime]
      ,[TransID]
      ,[TransType]
      ,[BENumber]
      ,[ItemNo]
      ,[UnitCost]
      ,[Quantity]
      ,[VATRate]
      ,[SD]
      ,[Remarks]
      ,[CreateDateTime]
      ,[TransactionType]
,@UserId UserId
, BranchId
    FROM [VAT_Process61].[dbo].VATTemp_16

) as VAT6_1
    ORDER BY ItemNo, dailydate, SerialNo
    OPTION (OPTIMIZE FOR UNKNOWN)


      UPDATE [VAT_Process61].[dbo].VAT6_1_Permanent SET StartDateTime=@StartDate WHERE SerialNo = 'A'
    
  
";


                    #endregion

                    cmd.Parameters.Clear();

                    cmd.CommandText = sqlText;
                    cmd.CommandType = CommandType.Text;

                    cmd.Parameters.AddWithValueAndParamCheck("@post1", "Y");
                    cmd.Parameters.AddWithValueAndParamCheck("@post2", "Y");
                    cmd.Parameters.AddWithValueAndParamCheck("@BranchId", 0);
                    cmd.Parameters.AddWithValueAndParamCheck("@StartDate", dateTime.ToString("yyyy-MM-dd 00:00:00"));
                    cmd.Parameters.AddWithValueAndParamCheck("@UserId", vm.UserId);

                    cmd.ExecuteNonQuery();

                    // Partiton
                    #region Partition Query

                    string insertToPermanent = @"

update [VAT_Process61].[dbo].VAT6_1_Permanent
set ItemNo = P.MasterProductItemNo
from Products p inner join [VAT_Process61].[dbo].VAT6_1_Permanent vp
on p.ItemNo = vp.ItemNo and p.MasterProductItemNo is not null and LEN(p.MasterProductItemNo) >0

UPDATE [VAT_Process61].[dbo].VAT6_1_Permanent
SET
    Quantity=a.ClosingQty,
    UnitCost=a.UnitCost,
    RunningTotal=a.ClosingQty,
    RunningValue=a.ClosingValue

FROM (
SELECT VAT6_1_Permanent_Branch.Id,
             VAT6_1_Permanent_Branch.ItemNo,
             RunningTotal           ClosingQty,
             RunningValue ClosingValue,
			 UnitCost
             ,VAT6_1_Permanent_Branch.BranchId
      FROM VAT6_1_Permanent_Branch
               RIGHT OUTER JOIN (SELECT DISTINCT ItemNo, BranchId,MAX(Id) Id
                                 FROM VAT6_1_Permanent_Branch
                                 WHERE StartDateTime < @StartDate
                                 GROUP BY ItemNo, BranchId) AS a
                                ON a.Id = VAT6_1_Permanent_Branch.ID) AS a
WHERE a.ItemNo = [VAT_Process61].[dbo].VAT6_1_Permanent.ItemNo
  AND [VAT_Process61].[dbo].VAT6_1_Permanent.TransType = 'MonthOpening'
  AND [VAT_Process61].[dbo].VAT6_1_Permanent.BranchId = a.BranchId


update VAT_Process61.dbo.VAT6_1_Permanent set AvgRate = 0 where 1=1 @itemCondition2

update VAT_Process61.dbo.VAT6_1_Permanent set  AvgRate=ProductAvgPrice.AvgPrice,RunningTotal=0
from ProductAvgPrice
where ProductAvgPrice.ItemNo=VAT_Process61.dbo.VAT6_1_Permanent.ItemNo
and VAT_Process61.dbo.VAT6_1_Permanent.TransID=ProductAvgPrice.PurchaseNo
@itemCondition2

update VAT_Process61.dbo.VAT6_1_Permanent
set  AvgRate=pap.AvgPrice,RunningTotal=0
from VAT_Process61.dbo.VAT6_1_Permanent id inner join 
ProductAvgPrice pap
on id.ItemNo = pap.ItemNo
and cast(id.StartDateTime as date) = pap.AgvPriceDate
and pap.TransactionType = 'Issue'
and id.TransType != 'Purchase'
@itemCondition2


create table #Temp(SL int identity(1,1),Id int,ItemNo varchar(100),TransType varchar(100)
,Quantity decimal(25,9),UnitCost decimal(25,9),BranchId int ) -- ,BranchId int

insert into #Temp(Id,ItemNo,TransType,Quantity,UnitCost,BranchId )
select Id,ItemNo,TransType,Quantity,UnitCost,BranchId from VAT_Process61.dbo.VAT6_1_Permanent
where 1=1 @itemCondition2
order by  BranchId,ItemNo,StartDateTime,SerialNo,TransID

update VAT_Process61.dbo.VAT6_1_Permanent set  RunningTotal=RT.RunningTotal
from (SELECT id,SL, ItemNo, TransType ,Quantity,
SUM (case when TransType in('Issue') then -1*Quantity else Quantity end ) 
OVER (PARTITION BY  BranchId,ItemNo ORDER BY SL) AS RunningTotal
FROM #Temp)RT
where RT.Id=VAT_Process61.dbo.VAT6_1_Permanent.Id @itemCondition3 --branch



Update VAT_Process61.dbo.VAT6_1_Permanent set RunningValue = AvgRate*RunningTotal where 1=1  @itemCondition2
update VAT_Process61.dbo.VAT6_1_Permanent set RunningTotal=Quantity, RunningValue=UnitCost where TransType in('opening') @itemCondition2

update VAT_Process61.dbo.VAT6_1_Permanent set  RunningOpeningQuantity=0,RunningOpeningValue=0 where 1=1 @itemCondition2
";
                    #endregion

                    if (!string.IsNullOrEmpty(vm.ItemNo))
                    {
                        insertToPermanent = insertToPermanent.Replace("@itemCondition2", " and VAT_Process61.dbo.VAT6_1_Permanent.ItemNo = @ItemNo");
                        insertToPermanent = insertToPermanent.Replace("@itemCondition3", " and VAT_Process61.dbo.VAT6_1_Permanent.ItemNo = @ItemNo");
                    }
                    else
                    {
                        insertToPermanent = insertToPermanent.Replace("@itemCondition1", "");
                        insertToPermanent = insertToPermanent.Replace("@itemCondition2", "");
                        insertToPermanent = insertToPermanent.Replace("@itemCondition3", "");
                    }


                    cmd.CommandType = CommandType.Text;

                    cmd.Parameters.AddWithValueAndParamCheck("@StartDate", dateTime.ToString("yyyy-MM-dd 00:00:00"));
                    cmd.Parameters.AddWithValueAndParamCheck("@EndDate", dateTime.ToString("yyyy-MM-dd 23:59:59"));

                    cmd.CommandText = insertToPermanent;
                    cmd.ExecuteNonQuery();

                    // insert back to main db

                    string insertBackToMain = @"

INSERT INTO VAT6_1_Permanent_Branch( [SerialNo]
      ,[ItemNo]
      ,[StartDateTime]
      ,[StartingQuantity]
      ,[StartingAmount]
      ,[VendorID]
      ,[SD]
      ,[VATRate]
      ,[Quantity]
      ,[UnitCost]
      ,[TransID]
      ,[TransType]
      ,[BENumber]
      ,[InvoiceDateTime]
      ,[Remarks]
      ,[CreateDateTime]
      ,[TransactionType]
      ,[BranchId]
      ,[UserId]
      ,[AvgRate]
      ,[PeriodID]
      ,[RunningTotal]
      ,[RunningValue]
      ,[RunningOpeningValue]
      ,[RunningOpeningQuantity])

Select 
 [SerialNo]
      ,[ItemNo]
      ,[StartDateTime]
      ,[StartingQuantity]
      ,[StartingAmount]
      ,[VendorID]
      ,[SD]
      ,[VATRate]
      ,[Quantity]
      ,[UnitCost]
      ,[TransID]
      ,[TransType]
      ,[BENumber]
      ,[InvoiceDateTime]
      ,[Remarks]
      ,[CreateDateTime]
      ,[TransactionType]
      ,[BranchId]
      ,[UserId]
      ,[AvgRate]
      ,[PeriodID]
      ,[RunningTotal]
      ,[RunningValue]
      ,[RunningOpeningValue]
      ,[RunningOpeningQuantity]
from [VAT_Process61].[dbo].VAT6_1_Permanent
where [VAT_Process61].[dbo].VAT6_1_Permanent.TransType != 'MonthOpening'
Order by  ItemNo, StartDateTime,SerialNo

";
                    cmd.CommandText = insertBackToMain;
                    cmd.ExecuteNonQuery();

                }

                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                #endregion Commit


            }
            #endregion

            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
                if (transaction != null && Vtransaction == null)
                {
                    transaction.Rollback();
                    //transaction.Commit();
                }
                FileLogger.Log("IssueDAL", "SaveTempIssue", ex.ToString() + "\n" + sqlText);

                throw;
            }
            finally
            {
                if (VcurrConn == null)
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
            #endregion

            #region Results
            return retResults;
            #endregion
        }

        public string[] SaveVAT6_1_Permanent_Branch_Backup28082023(VAT6_1ParamVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            ReportDSDAL reportDsdal = new ReportDSDAL();

            CommonDAL commonDal = new CommonDAL();
            #endregion

            #region try

            try
            {
                #region open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnectionNoPooling(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                #endregion open connection and transaction

                #region Opening

                sqlText = reportDsdal.GetOpeningQuery_Backup28082023(ProcessConfig.Permanent_Branch);

                if (!string.IsNullOrEmpty(vm.ItemNo))
                {
                    sqlText = sqlText.Replace("@itemCondition", "and p.ItemNo = @ItemNo");
                }
                else
                {
                    sqlText = sqlText.Replace("@itemCondition", "");
                }

                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
                cmd.CommandTimeout = 500;

                if (!string.IsNullOrEmpty(vm.ItemNo))
                {
                    cmd.Parameters.AddWithValue("@ItemNo", vm.ItemNo);
                }
                cmd.ExecuteNonQuery();


                #endregion

                #region Delete Existing Data

                string periodId = vm.StartDate.ToDateString("MMyyyy");
                string deleteText = @" delete from VAT6_1_Permanent_Branch where  TransType != 'Opening'  and  StartDateTime>=@StartDate and StartDateTime<dateadd(d,1,@EndDate)
     @itemCondition";

                if (!string.IsNullOrEmpty(vm.ItemNo))
                {
                    deleteText = deleteText.Replace("@itemCondition", "and ItemNo = @ItemNo");
                }
                else if (vm.FilterProcessItems)
                {
                    deleteText = deleteText.Replace("@itemCondition", " and ItemNo in (select ItemNo from Products where ProcessFlag='Y' and (ReportType='VAT6_1' or ReportType='VAT6_1_And_6_2') )");
                }
                else
                {
                    deleteText = deleteText.Replace("@itemCondition", "");
                }

                cmd.CommandText = deleteText;
                cmd.Parameters.AddWithValue("@StartDate", vm.StartDate);
                cmd.Parameters.AddWithValue("@EndDate", vm.EndDate);

                if (!string.IsNullOrEmpty(vm.ItemNo))
                {
                    cmd.Parameters.AddWithValueAndParamCheck("@ItemNo", vm.ItemNo);
                }

                cmd.ExecuteNonQuery();

                #endregion

                BranchProfileDAL branchProfileDal = new BranchProfileDAL();

                List<BranchProfileVM> branchList = branchProfileDal.SelectAllList(null, null, null, currConn, transaction, connVM);

                DateTime paramMonth = Convert.ToDateTime(vm.StartDate);

                vm.PermanentProcess = true;

                #region Branch wise data insert

                string insertToPermanent = reportDsdal.GetInsertQuery_Backup28082023(ProcessConfig.Permanent_Branch);

                string updateMasterItem = @"
update VAT6_1
set ItemNo = P.MasterProductItemNo
from Products p inner join VAT6_1 vp
on p.ItemNo = vp.ItemNo and p.MasterProductItemNo is not null and LEN(p.MasterProductItemNo) >0
where UserId = @UserId

";


                if (!string.IsNullOrEmpty(vm.ItemNo))
                {
                    insertToPermanent = insertToPermanent.Replace("@itemCondition1", " and ItemNo = @ItemNo");
                }
                else
                {
                    insertToPermanent = insertToPermanent.Replace("@itemCondition1", "");
                }

                cmd.CommandText = insertToPermanent;

                if (!string.IsNullOrEmpty(vm.ItemNo))
                {
                    cmd.Parameters.AddWithValueAndParamCheck("@ItemNo", vm.ItemNo);
                }


                cmd.Parameters.AddWithValueAndParamCheck("@UserId", vm.UserId);
                cmd.Parameters.AddWithValueAndParamCheck("@PeriodId", periodId);


                foreach (var branchProfileVm in branchList)
                {
                    vm.BranchId = branchProfileVm.BranchID;
                    DataSet dsResult = reportDsdal.VAT6_1_WithConn_Backup28082023(vm, currConn, transaction, connVM);

                    cmd.CommandText = updateMasterItem;
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = insertToPermanent;
                    cmd.ExecuteNonQuery();

                }

                #endregion



                string updateValues = reportDsdal.Get6_1PartitionQuery_Backup28082023(ProcessConfig.Permanent_Branch);


                if (!string.IsNullOrEmpty(vm.ItemNo))
                {
                    updateValues = updateValues.Replace("@itemCondition1", " and ItemNo = @ItemNo");
                    updateValues = updateValues.Replace("@itemCondition2", " and VAT6_1_Permanent_Branch.ItemNo = @ItemNo");
                }
                else
                {
                    updateValues = updateValues.Replace("@itemCondition1", "");
                    updateValues = updateValues.Replace("@itemCondition2", "");
                }

                cmd.CommandText = updateValues;


                if (!string.IsNullOrEmpty(vm.ItemNo))
                {
                    cmd.Parameters.AddWithValueAndParamCheck("@ItemNo", vm.ItemNo);
                }


                cmd.Parameters.AddWithValueAndParamCheck("@UserId", vm.UserId);
                cmd.Parameters.AddWithValueAndParamCheck("@PeriodId", periodId);
                cmd.CommandTimeout = 500;

                cmd.ExecuteNonQuery();


                #region Update Negative

                string updateNegativeValue =
                    @"
update VAT6_1_Permanent_Branch set AvgRate = 0,RunningValue= 0, RunningOpeningValue=0
from (
select distinct ItemNo,BranchId,min(StartDateTime)StartDateTime  from VAT6_1_Permanent_Branch 
where 1=1 and TransType not in ('opening') 
and RunningTotal<0
@itemCondition2

group by ItemNo,BranchId
) a
where VAT6_1_Permanent_Branch.ItemNo = a.ItemNo 
and  VAT6_1_Permanent_Branch.ItemNo = a.BranchId 
and VAT6_1_Permanent_Branch.StartDateTime>= a.StartDateTime
@itemCondition2

";

                if (!string.IsNullOrEmpty(vm.ItemNo))
                {
                    updateNegativeValue = updateNegativeValue.Replace("@itemCondition2", " and VAT6_1_Permanent_Branch.ItemNo = @ItemNo");
                }
                else
                {
                    updateNegativeValue = updateNegativeValue.Replace("@itemCondition2", "");
                }

                cmd.CommandText = updateNegativeValue;

                if (!string.IsNullOrEmpty(vm.ItemNo))
                {
                    cmd.Parameters.AddWithValueAndParamCheck("@ItemNo", vm.ItemNo);
                }
                //    cmd.ExecuteNonQuery();


                #endregion

                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                #endregion Commit
            }
            #endregion

            #region Catch and Finall
            catch (Exception ex)
            {
                //transaction.Commit();

                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
                if (transaction != null && Vtransaction == null) { transaction.Rollback(); }
                FileLogger.Log("IssueDAL", "SaveTempIssue", ex.ToString() + "\n" + sqlText);

                throw;
            }
            finally
            {
                if (VcurrConn == null)
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
            #endregion
            #region Results
            return retResults;
            #endregion
        }

        public string[] SaveVAT6_1_Permanent_DayWise_Branch(VAT6_1ParamVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            ReportDSDAL reportDsdal = new ReportDSDAL();

            CommonDAL commonDal = new CommonDAL();
            #endregion

            #region try

            try
            {
                #region open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                    transaction.Commit();
                }

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnectionNoPooling(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                    transaction.Commit();
                }
                #endregion open connection and transaction

                #region dbCreate

                string dbCreate = @"

DECLARE @DatabaseName nvarchar(50)
SET @DatabaseName = N'VAT_Process'

DECLARE @SQL varchar(max)

SELECT @SQL = COALESCE(@SQL,'') + 'Kill ' + Convert(varchar, SPId) + ';'
FROM MASTER..SysProcesses
WHERE DBId = DB_ID(@DatabaseName) AND SPId <> @@SPId

--SELECT @SQL 
EXEC(@SQL)


          DROP DATABASE IF EXISTS [VAT_Process]

            IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = 'VAT_Process')
                    BEGIN
                        CREATE DATABASE [VAT_Process]
                    END

";
                #endregion

                #region tableCreate

                string tableCreate = @"
IF NOT EXISTS(SELECT *
                      FROM INFORMATION_SCHEMA.TABLES
                      WHERE TABLE_SCHEMA = 'dbo'
                        AND TABLE_NAME = 'VATTemp_17_1'
                        AND TABLE_CATALOG = 'VAT_Process'
            )
            BEGIN
                CREATE TABLE VAT_Process.[dbo].VATTemp_17_1
                (
                    SerialNo        VARCHAR(10)    NULL,
                    Dailydate       DATETIME       NULL,
                    TransID         VARCHAR(200)   NULL,
                    TransType       VARCHAR(200)   NULL,
                    ItemNo          VARCHAR(200)   NULL,
                    UnitCost        DECIMAL(25, 9) NULL,
                    Quantity        DECIMAL(25, 9) NULL,
                    VATRate         DECIMAL(25, 9) NULL,
                    SD              DECIMAL(25, 9) NULL,
                    Remarks         VARCHAR(200),
                    CreatedDateTime DATETIME       NULL,
                    UnitRate        DECIMAL(25, 9),
                    AdjustmentValue DECIMAL(25, 9)
                )


                CREATE TABLE VAT_Process.[dbo].[VAT6_2_Permanent]
                (
                    [ID]                          [int] IDENTITY (1,1) NOT NULL,
                    [SerialId]                          [int] ,
                    [SerialNo]                    [varchar](200)       NULL,
                    [StartDateTime]               [datetime]           NULL,
                    [StartingQuantity]            [decimal](25, 9)     NULL,
                    [StartingAmount]              [decimal](25, 9)     NULL,
                    [TransID]                     [varchar](200)       NULL,
                    [TransType]                   [varchar](200)       NULL,
                    [CustomerName]                [varchar](200)       NULL,
                    [Address1]                    [varchar](500)       NULL,
                    [Address2]                    [varchar](500)       NULL,
                    [Address3]                    [varchar](500)       NULL,
                    [VATRegistrationNo]           [varchar](500)       NULL,
                    [ProductName]                 [varchar](500)       NULL,
                    [ProductCode]                 [varchar](500)       NULL,
                    [UOM]                         [varchar](50)        NULL,
                    [HSCodeNo]                    [varchar](500)       NULL,
                    [Quantity]                    [decimal](25, 9)     NULL,
                    [VATRate]                     [decimal](25, 9)     NULL,
                    [SD]                          [decimal](25, 9)     NULL,
                    [UnitCost]                    [decimal](25, 9)     NULL,
                    [Remarks]                     [varchar](200)       NULL,
                    [CreatedDateTime]             [datetime]           NULL,
                    [UnitRate]                    [decimal](25, 9)     NULL,
                    [ItemNo]                      [varchar](50)        NULL,
                    [AdjustmentValue]             [decimal](25, 9)     NULL,
                    [UserId]                      [varchar](50)        NULL,
                    [BranchId]                    [varchar](50)        NULL,
                    [CustomerID]                  [varchar](50)        NULL,
                    [ProductDesc]                 [varchar](500)       NULL,
                    [ClosingRate]                 [decimal](25, 9)     NULL,
                    [PeriodId]                    [varchar](50)        NULL,
                    [RunningTotal]                [decimal](18, 8)     NULL,
                    [RunningTotalValue]           [decimal](18, 8)     NULL,
                    [RunningTotalValueFinal]      [decimal](18, 8)     NULL,
                    [DeclaredPrice]               [decimal](25, 9)     NULL,
                    [RunningOpeningValueFinal]    [decimal](18, 8)     NULL,
                    [RunningOpeningQuantityFinal] [decimal](18, 8)     NULL,
                    PRIMARY KEY CLUSTERED
                        (
                         [ID] ASC
                            ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                ) ON [PRIMARY]
				
				            END

";

                SqlCommand cmdDbCommand = new SqlCommand(dbCreate, currConn, transaction);
                cmdDbCommand.ExecuteNonQuery();
                cmdDbCommand.CommandText = tableCreate;
                cmdDbCommand.ExecuteNonQuery();

                #endregion

                #region delete VAT6_1_Permanent_DayWise_Branch

                sqlText = @"
delete from VAT6_1_Permanent_DayWise_Branch where StartDateTime >= @StartDateTime 
                and StartDateTime <= @EndDateTime

";

                if (!string.IsNullOrEmpty(vm.ItemNo))
                {
                    sqlText += " and ItemNo='" + vm.ItemNo + "'";
                }

                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
                cmd.Parameters.AddWithValue("@StartDateTime", vm.StartDate);
                cmd.Parameters.AddWithValue("@EndDateTime", vm.EndDate);
                cmd.ExecuteNonQuery();

                #endregion

                #region sqlText

                sqlText = @"
insert into VAT6_1_Permanent_DayWise_Branch
(
[SerialNo]
      ,[ItemNo]
      ,[StartDateTime]
      ,[StartingQuantity]
      ,[StartingAmount]
      ,[VendorID]
      ,[SD]
      ,[VATRate]
      ,[Quantity]
      ,[UnitCost]
      ,[TransID]
      ,[TransType]
      ,[BENumber]
      ,[InvoiceDateTime]
      ,[Remarks]
      ,[CreateDateTime]
      ,[TransactionType]
      ,[BranchId]
      ,[UserId]
      ,[AvgRate]
      ,[PeriodID]
      ,[RunningTotal]
      ,[RunningValue]
      ,[RunningOpeningValue]
      ,[RunningOpeningQuantity]
	)

Select * from 
(select 
      distinct 
        'A' [SerialNo]
      ,[ItemNo]
      ,@StartDateTime [StartDateTime]
      ,0 [StartingQuantity]
      ,0 [StartingAmount]
      ,''[VendorID]
      ,0 [SD]
      ,0 [VATRate]
      ,0 [Quantity]
      ,0 [UnitCost]
      ,''[TransID]
      ,'MonthOpening'[TransType]
      ,''[BENumber]
      ,'1900-01-01' [InvoiceDateTime]
      ,'' [Remarks]
      ,'1900-01-01' [CreateDateTime]
      ,'MonthOpening'[TransactionType]
      ,[BranchId]
      ,0 [UserId]
      ,0 [AvgRate]
      ,0 [PeriodID]
      ,0 [RunningTotal]
      ,0 [RunningValue]
      ,0 [RunningOpeningValue]
      ,0 [RunningOpeningQuantity]
	  from VAT6_1_Permanent_branch
	  where StartDateTime >= @StartDateTime 
	  and StartDateTime <= @EndDateTime

	union all
	
select 
      [SerialNo]
      ,[ItemNo]
      ,CAST(StartDateTime AS DATE) [StartDateTime]
      ,0 [StartingQuantity]
      ,0 [StartingAmount]
      ,''[VendorID]
      ,0 [SD]
      ,0 [VATRate]
      ,sum([Quantity])Quantity
      ,sum([UnitCost])UnitCost
      ,''[TransID]
      ,[TransType]
      ,''[BENumber]
      ,''[InvoiceDateTime]
      ,''[Remarks]
      ,''[CreateDateTime]
      ,'' [TransactionType]
      ,[BranchId]
      ,'' [UserId]
      ,avg(AvgRate)AvgRate
      ,0 [PeriodID]
      ,0 [RunningTotal]
      ,0 [RunningValue]
      ,0 [RunningOpeningValue]
      ,0 [RunningOpeningQuantity]
	  from VAT6_1_Permanent_Branch
	  where StartDateTime >= @StartDateTime 
	  and StartDateTime <= @EndDateTime
	  group by 
	  BranchId, ItemNo, CAST(StartDateTime AS DATE)
	  , SerialNo
	  ,TransType
	  ) VAT6_1_Day

	  order by 	ItemNo, StartDateTime
	  , SerialNo
";


                cmd.CommandText = sqlText;
                cmd.CommandTimeout = 500;
                cmd.ExecuteNonQuery();

                #endregion

                #region insertToPermanent

                string insertToPermanent = @"

UPDATE VAT6_1_Permanent_DayWise_Branch
SET
    Quantity=a.ClosingQty,
    UnitCost=a.UnitCost,
    RunningTotal=a.ClosingQty,
    RunningValue=a.ClosingValue

FROM (
SELECT VAT6_1_Permanent_Branch.Id,
             VAT6_1_Permanent_Branch.ItemNo,
             RunningTotal           ClosingQty,
             RunningValue ClosingValue,
			 UnitCost,
             VAT6_1_Permanent_Branch.BranchId
      FROM VAT6_1_Permanent_Branch
               RIGHT OUTER JOIN (SELECT DISTINCT ItemNo,BranchId, MAX(Id) Id
                                 FROM VAT6_1_Permanent_Branch
                                 WHERE StartDateTime < @StartDate
                                 GROUP BY ItemNo,BranchId) AS a
                                ON a.Id = VAT6_1_Permanent_Branch.ID) AS a
WHERE a.ItemNo = [VAT6_1_Permanent_DayWise_Branch].ItemNo
  AND VAT6_1_Permanent_DayWise_Branch.TransType = 'MonthOpening'
  AND VAT6_1_Permanent_DayWise_Branch.BranchId = a.BranchId
AND VAT6_1_Permanent_DayWise_Branch.StartDateTime = @StartDate

--update VAT6_1_Permanent_DayWise_Branch set AvgRate = 0 where 1=1 @itemCondition2

--update VAT6_1_Permanent_DayWise_Branch set  AvgRate=ProductAvgPrice.AvgPrice,RunningTotal=0
--from ProductAvgPrice
--where ProductAvgPrice.ItemNo=VAT6_1_Permanent_DayWise_Branch.ItemNo
--and VAT6_1_Permanent_DayWise_Branch.TransID=ProductAvgPrice.PurchaseNo
--@itemCondition2

update VAT6_1_Permanent_DayWise_Branch
set  AvgRate=pap.AvgPrice,RunningTotal=0
from VAT6_1_Permanent_DayWise_Branch id inner join 
ProductAvgPrice pap
on id.ItemNo = pap.ItemNo
and cast(id.StartDateTime as date) = pap.AgvPriceDate
--and pap.TransactionType = 'Issue'
where
 id.StartDateTime >= @StartDateTime
and id.StartDateTime <= @EndDateTime
@itemCondition2


create table #Temp(SL int identity(1,1),Id int,ItemNo varchar(100),TransType varchar(100)
,Quantity decimal(25,9),UnitCost decimal(25,9),BranchId int ) -- ,BranchId int

insert into #Temp(Id,ItemNo,TransType,Quantity,UnitCost,BranchId )
select Id,ItemNo,TransType,Quantity,UnitCost,BranchId from VAT6_1_Permanent_DayWise_Branch
where 1=1 
and StartDateTime >= @StartDateTime
and StartDateTime <= @EndDateTime
@itemCondition2
order by  ItemNo,StartDateTime,SerialNo,TransID

update VAT6_1_Permanent_DayWise_Branch set  RunningTotal=RT.RunningTotal
from (SELECT id,SL, ItemNo, TransType  ,Quantity,
SUM (case when TransType in('Issue') then -1*Quantity else Quantity end ) 
OVER (PARTITION BY BranchId, ItemNo ORDER BY SL) AS RunningTotal
FROM #Temp)RT
where RT.Id=VAT6_1_Permanent_DayWise_Branch.Id @itemCondition3 --branch

Update VAT6_1_Permanent_DayWise_Branch set RunningValue = AvgRate*RunningTotal where 1=1  @itemCondition2
update VAT6_1_Permanent_DayWise_Branch set RunningTotal=Quantity, RunningValue=UnitCost where TransType in('opening') @itemCondition2

update VAT6_1_Permanent_DayWise_Branch set  RunningOpeningQuantity=0,RunningOpeningValue=0 where 1=1 @itemCondition2

";
                if (!string.IsNullOrEmpty(vm.ItemNo))
                {
                    insertToPermanent = insertToPermanent.Replace("@itemCondition2", " and VAT6_1_Permanent_DayWise_Branch.ItemNo = @ItemNo");
                    insertToPermanent = insertToPermanent.Replace("@itemCondition3", " and VAT6_1_Permanent_DayWise_Branch.ItemNo = @ItemNo");
                }
                else
                {
                    insertToPermanent = insertToPermanent.Replace("@itemCondition1", "");
                    insertToPermanent = insertToPermanent.Replace("@itemCondition2", "");
                    insertToPermanent = insertToPermanent.Replace("@itemCondition3", "");
                }

                cmd.CommandText = insertToPermanent;

                cmd.Parameters.AddWithValue("@StartDate", vm.StartDate);
                cmd.Parameters.AddWithValue("@EndDate", vm.EndDate);
                cmd.ExecuteNonQuery();

                #endregion

                #region Commit
                //if (Vtransaction == null)
                //{
                //    if (transaction != null)
                //    {
                //        transaction.Commit();
                //    }
                //}
                #endregion Commit

            }
            #endregion

            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
                if (transaction != null && Vtransaction == null)
                {
                    transaction.Rollback();
                    //transaction.Commit();
                }
                FileLogger.Log("IssueDAL", "SaveTempIssue", ex.ToString() + "\n" + sqlText);

                throw;
            }
            finally
            {
                if (VcurrConn == null)
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
            #endregion
            #region Results
            return retResults;
            #endregion
        }

        public string[] SaveVAT6_2_Permanent_Backup28082023(VAT6_2ParamVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            ReportDSDAL reportDsdal = new ReportDSDAL();

            CommonDAL commonDal = new CommonDAL();
            #endregion

            #region try

            try
            {
                #region open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnectionNoPooling(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                #endregion open connection and transaction

                #region Opening

                sqlText = reportDsdal.Get6_2OpeningQuery(ProcessConfig.Permanent);

                if (!string.IsNullOrEmpty(vm.ItemNo))
                {
                    sqlText = sqlText.Replace("@itemCondition", "and p.ItemNo = @ItemNo");
                }
                else
                {
                    sqlText = sqlText.Replace("@itemCondition", "");
                }

                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
                cmd.Parameters.AddWithValueAndParamCheck("@UserId", vm.UserId);
                cmd.Parameters.AddWithValueAndParamCheck("@BranchId", vm.BranchId);
                cmd.CommandTimeout = 800;

                if (!string.IsNullOrEmpty(vm.ItemNo))
                {
                    cmd.Parameters.AddWithValue("@ItemNo", vm.ItemNo);
                }

                cmd.ExecuteNonQuery();

                #endregion

                #region Delete Existing Data

                string deleteText = @" delete from VAT6_2_Permanent where 1=1  and TransType != 'Opening'
                                and  StartDateTime>=@StartDate and StartDateTime<dateadd(d,1,@EndDate)
                                @itemCondition";

                if (!string.IsNullOrEmpty(vm.ItemNo))
                {
                    deleteText = deleteText.Replace("@itemCondition", " and ItemNo = @ItemNo");
                }
                else if (vm.FilterProcessItems)
                {
                    deleteText = deleteText.Replace("@itemCondition", " and ItemNo in (select ItemNo from Products where ProcessFlag='Y' and (ReportType='VAT6_2' or ReportType='VAT6_1_And_6_2'))");
                }
                else
                {
                    deleteText = deleteText.Replace("@itemCondition", "");
                }

                cmd.CommandText = deleteText;
                cmd.Parameters.AddWithValue("@StartDate", vm.StartDate);
                cmd.Parameters.AddWithValue("@EndDate", vm.EndDate);
                //cmd.Parameters.AddWithValue("@PeriodId", periodId);

                if (!string.IsNullOrEmpty(vm.ItemNo))
                {
                    cmd.Parameters.AddWithValueAndParamCheck("@ItemNo", vm.ItemNo);
                }

                cmd.ExecuteNonQuery();


                #endregion

                DateTime paramMonth = Convert.ToDateTime(vm.StartDate);

                //var firstDayOfMonth = new DateTime(paramMonth.Year, paramMonth.Month, 1);
                //var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

                //vm.StartDate = firstDayOfMonth.ToString("yyyy-MM-dd");
                //vm.EndDate = lastDayOfMonth.ToString("yyyy-MM-dd");

                vm.PermanentProcess = true;

                ////DataSet dsResult = reportDsdal.VAT6_2(vm, currConn, transaction, connVM);

                string updateMasterItem = @"
update VAT6_2
set ItemNo = P.MasterProductItemNo
from Products p inner join VAT6_2 vp
on p.ItemNo = vp.ItemNo and p.MasterProductItemNo is not null and LEN(p.MasterProductItemNo) >0
where UserId = @UserId

";

                cmd.CommandText = updateMasterItem;
                cmd.ExecuteNonQuery();


                #region VAT 6.2 Permanent Update Query

                string insertToPermanent = reportDsdal.Get6_2PartitionQuery(ProcessConfig.Permanent);


                #endregion

                if (!string.IsNullOrEmpty(vm.ItemNo))
                {
                    insertToPermanent = insertToPermanent.Replace("@itemCondition1", " and VAT6_2.ItemNo = @ItemNo");
                    insertToPermanent = insertToPermanent.Replace("@itemCondition2", " and #NBRPrive.ItemNo = @ItemNo");
                    insertToPermanent = insertToPermanent.Replace("@itemCondition3", " and VAT6_2_Permanent.ItemNo = @ItemNo");
                }
                else
                {
                    insertToPermanent = insertToPermanent.Replace("@itemCondition1", "");
                    insertToPermanent = insertToPermanent.Replace("@itemCondition2", "");
                    insertToPermanent = insertToPermanent.Replace("@itemCondition3", "");
                }

                cmd.CommandText = insertToPermanent;

                cmd.Parameters.AddWithValueAndParamCheck("@UserId", vm.UserId);
                cmd.Parameters.AddWithValueAndParamCheck("@PeriodId", Convert.ToDateTime(vm.StartDate).ToString("MMyyyy"));
                cmd.Parameters.AddWithValueAndParamCheck("@StartDate", vm.StartDate);

                if (!string.IsNullOrEmpty(vm.ItemNo))
                {
                    cmd.Parameters.AddWithValueAndParamCheck("@ItemNo", vm.ItemNo);
                }

                cmd.ExecuteNonQuery();

                #region Update Negative

                //                string updateNegativeValue =
                //                    @"
                //update VAT6_2_Permanent set RunningOpeningValueFinal = 0,RunningTotalValue= 0, RunningTotalValueFinal=0
                //from (
                //select distinct ItemNo,min(StartDateTime)StartDateTime  from VAT6_2_Permanent 
                //where 1=1 and TransType not in ('opening') 
                //and RunningTotal<0
                //--@itemCondition2
                //
                //group by ItemNo
                //) a
                //where VAT6_2_Permanent.ItemNo = a.ItemNo and VAT6_2_Permanent.StartDateTime>= a.StartDateTime
                //--@itemCondition2
                //
                //";

                //                if (!string.IsNullOrEmpty(vm.ItemNo))
                //                {
                //                    updateNegativeValue = updateNegativeValue.Replace("@itemCondition2", " and VAT6_2_Permanent.ItemNo = @ItemNo");
                //                }
                //                else
                //                {
                //                    updateNegativeValue = updateNegativeValue.Replace("@itemCondition2", "");
                //                }

                //                cmd.CommandText = updateNegativeValue;

                //                if (!string.IsNullOrEmpty(vm.ItemNo))
                //                {
                //                    cmd.Parameters.AddWithValueAndParamCheck("@ItemNo", vm.ItemNo);
                //                }
                //                cmd.ExecuteNonQuery();


                #endregion


                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                #endregion Commit
            }
            #endregion

            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
                if (transaction != null && Vtransaction == null)
                {
                    try
                    {
                        transaction.Rollback();
                    }
                    catch
                    {

                    }
                    //transaction.Commit();
                }
                FileLogger.Log("IssueDAL", "SaveTempIssue", ex.ToString() + "\n" + sqlText);

                throw;
            }
            finally
            {
                if (VcurrConn == null)
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
            #endregion
            #region Results
            return retResults;
            #endregion
        }



        public string[] SaveVAT6_2_1_Permanent(VAT6_2ParamVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            ReportDSDAL reportDsdal = new ReportDSDAL();
            ProductDAL productDal = new ProductDAL();

            CommonDAL commonDal = new CommonDAL();
            #endregion

            #region try

            try
            {
                #region open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnectionNoPooling(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                #endregion open connection and transaction

                #region Opening
                sqlText = @"
Insert into VAT6_2_1_Permanent(
[SerialNo]
,[StartDateTime]
,[StartingQuantity]
,[StartingAmount]
,[TransID]
,[TransType] 
,[Quantity]
,[SD]
,[UnitCost]   
,[Remarks] 
,[ItemNo]  
,RunningOpeningQuantityFinal
,RunningOpeningValueFinal
,UnitRate)


SELECT distinct 'A'SerialNo, '1900-01-01' StartDateTime, 0 StartingQuantity, 0 StartingAmount,
0 TransID, 'Opening' TransType,
sum(isnull(p.StockQuantity,0)) Quantity,0 SD, sum(isnull(p.StockValue,0)) UnitCost, 'Opening'Remarks, p.ItemNo
,sum(isnull(StockQuantity,0)) Quantity, sum(isnull(p.StockValue,0)) UnitCost, sum(isnull(p.StockValue,0))/(case when sum(isnull(StockQuantity,0)) =0 then 1 else sum(isnull(StockQuantity,0)) end) UnitRate

FROM ProductStocks p  left outer join Products pd on pd.ItemNo = p.ItemNo
WHERE 
p.ItemNo  in(select ItemNo from Products where ReportType in ('VAT6_2_1'))
and p.ItemNo not in (select ItemNo from VAT6_2_1_Permanent where transType = 'Opening')

group by p.ItemNo,pd.ProductName";

                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
                cmd.Parameters.AddWithValueAndParamCheck("@UserId", vm.UserId);

                cmd.CommandTimeout = 500;
                cmd.ExecuteNonQuery();

                #endregion

                #region Delete Existing Data

                string deleteText = @" delete from VAT6_2_1_Permanent where 1=1  and TransType != 'Opening'
                                and  StartDateTime>=@StartDate and StartDateTime<dateadd(d,1,@EndDate) @itemCondition";


                if (!string.IsNullOrEmpty(vm.ItemNo))
                {
                    deleteText = deleteText.Replace("@itemCondition", " and ItemNo = @ItemNo");
                }
                else
                {
                    deleteText = deleteText.Replace("@itemCondition", "");
                }


                cmd.CommandText = deleteText;
                cmd.Parameters.AddWithValue("@StartDate", vm.StartDate);
                cmd.Parameters.AddWithValue("@EndDate", vm.EndDate);

                if (!string.IsNullOrEmpty(vm.ItemNo))
                {
                    cmd.Parameters.AddWithValueAndParamCheck("@ItemNo", vm.ItemNo);
                }

                cmd.ExecuteNonQuery();

                #endregion

                DateTime paramMonth = Convert.ToDateTime(vm.StartDate);

                vm.PermanentProcess = true;

                string[] result = productDal.StockMovement6_2_1_PermanentProcess(vm, currConn, transaction, connVM);

                string insertToPermanent = @"

delete from ProductStockMISKas where  ProductStockMISKas.TransType='Opening' @itemCondition

update ProductStockMISKas
set ItemNo = P.MasterProductItemNo
from Products p inner join ProductStockMISKas vp
on p.ItemNo = vp.ItemNo and p.MasterProductItemNo is not null and LEN(p.MasterProductItemNo) >0
where UserId = @UserId

insert into VAT6_2_1_Permanent(
[SerialNo]
      ,[StartDateTime]
      ,[StartingQuantity]
      ,[StartingAmount]
      ,[TransID]
      ,[TransType]
      ,[VendorName]
      ,[Address1]
      ,[Address2]
      ,[Address3]
      ,[VATRegistrationNo]
      ,[ProductName]
      ,[Quantity]
      ,[VATRate]
      ,[SD]
      ,[UnitCost]
      ,[HSCodeNo]
      ,[BENumber]
      ,[InvoiceDateTime]
      ,[Remarks]
      ,[ItemNo]
      ,[StockType]
      ,[BranchId]
	  )
SELECT 
	   [SerialNo]
      ,[StartDateTime]
      ,[StartingQuantity]
      ,[StartingAmount]
      ,[TransID]
      ,[TransType]
      ,[VendorName]
      ,[Address1]
      ,[Address2]
      ,[Address3]
      ,[VATRegistrationNo]
      ,[ProductName]
      ,[Quantity]
      ,[VATRate]
      ,[SD]
      ,[UnitCost]
      ,[HSCodeNo]
      ,[BENumber]
      ,[InvoiceDateTime]
      ,[Remarks]
      ,[ItemNo]
      ,[StockType]
      ,[BranchId]
  FROM ProductStockMISKas where UserId = @UserId @itemCondition
   order by ItemNo, StartDateTime, SerialNo
";


                if (!string.IsNullOrEmpty(vm.ItemNo))
                {
                    insertToPermanent = insertToPermanent.Replace("@itemCondition", " and ItemNo = @ItemNo");
                }
                else
                {
                    insertToPermanent = insertToPermanent.Replace("@itemCondition", "");
                }

                cmd.CommandText = insertToPermanent;

                cmd.Parameters.AddWithValueAndParamCheck("@PeriodId", Convert.ToDateTime(vm.StartDate).ToString("MMyyyy"));
                cmd.Parameters.AddWithValueAndParamCheck("@StartDate", vm.StartDate);
                cmd.Parameters.AddWithValueAndParamCheck("@UserId", vm.UserId);

                if (!string.IsNullOrEmpty(vm.ItemNo))
                {
                    cmd.Parameters.AddWithValueAndParamCheck("@ItemNo", vm.ItemNo);
                }

                cmd.ExecuteNonQuery();


                string generateClosingValues = @"
create table #NBRPrive(id int identity(1,1),ItemNo varchar(100) ,CustomerId varchar(100),Rate decimal(18,8), EffectDate datetime,ToDate datetime)
create table #Temp(SL int identity(1,1),Id int,ItemNo varchar(100),TransType varchar(100),Quantity decimal(18,6),TotalCost decimal(25,8))

update VAT6_2_1_Permanent set CustomerID=SalesInvoiceHeaders.CustomerID
from SalesInvoiceHeaders
where SalesInvoiceHeaders.SalesInvoiceNo=VAT6_2_1_Permanent.TransID 
and VAT6_2_1_Permanent.TransType = 'sale'  @itemCondition3

insert into #NBRPrive
select itemNo, '' CustomerId ,
(
case 
when NBRPrice = 0 then ( case when OpeningBalance = 0 then 0 else OpeningTotalCost/OpeningBalance end) else ISNULL(NBRPrice,0) 
end
) NBRPrice, '1900/01/01'EffectDate ,null ToDate from products
where ItemNo in(select distinct Itemno from VAT6_2_1_Permanent where 1=1 @itemCondition3) 

insert into #NBRPrive
select FinishItemNo,customerId, ISNULL(NBRPrice,0) NBRPrice,  EffectDate EffectDate ,null ToDate from BOMs
where FinishItemNo in(select distinct Itemno from VAT6_2_1_Permanent where 1=1 @itemCondition3)
order by EffectDate

update #NBRPrive set  ToDate=null where 1=1 @itemCondition2


update #NBRPrive set  ToDate=RT.RunningTotal
from ( SELECT id,Customerid, ItemNo,
LEAD(EffectDate) 
OVER (PARTITION BY Customerid,ItemNo ORDER BY id) AS RunningTotal
FROM #NBRPrive
where customerid>0 
)RT
where RT.Id=#NBRPrive.Id  and  RT.Customerid=#NBRPrive.Customerid 
and ToDate is null
@itemCondition2

update #NBRPrive set  ToDate=RT.RunningTotal
from (SELECT id, ItemNo,
LEAD(EffectDate) 
OVER (PARTITION BY ItemNo ORDER BY id) AS RunningTotal
FROM #NBRPrive
where isnull(nullif(customerid,''),0)<=0 
)RT
where RT.Id=#NBRPrive.Id  
and isnull(nullif(customerid,''),0)<=0
and ToDate is null
@itemCondition2


update #NBRPrive set  ToDate='2199/12/31' where ToDate is null @itemCondition2

insert into #Temp(Id,ItemNo,TransType,Quantity,TotalCost)
select Id,ItemNo,TransType,Quantity,UnitCost from VAT6_2_1_Permanent 
where 1=1 @itemCondition3
order by ItemNo,StartDateTime,SerialNo

update VAT6_2_1_Permanent set  RunningTotal=RT.RunningTotal
from (SELECT id,SL, ItemNo, TransType ,Quantity,
SUM (case when TransType in('Sale') then -1*Quantity else Quantity end ) 
OVER (PARTITION BY ItemNo ORDER BY  ItemNo,SL) AS RunningTotal
FROM #Temp)RT
where RT.Id=VAT6_2_1_Permanent.Id
@itemCondition3

update VAT6_2_1_Permanent set  RunningTotalValue=RT.RunningTotalCost
from (SELECT id,SL, ItemNo, TransType ,Quantity,
SUM (case when TransType in('Sale') then -1*TotalCost else TotalCost end ) 
OVER (PARTITION BY ItemNo ORDER BY SL) AS RunningTotalCost
FROM #Temp)RT
where RT.Id=VAT6_2_1_Permanent.Id
@itemCondition3

update VAT6_2_1_Permanent set DeclaredPrice =0

update VAT6_2_1_Permanent set DeclaredPrice =#NBRPrive.Rate
from #NBRPrive
where #NBRPrive.ItemNo=VAT6_2_1_Permanent.ItemNo
and VAT6_2_1_Permanent.StartDateTime >=#NBRPrive.EffectDate and VAT6_2_1_Permanent.StartDateTime<#NBRPrive.ToDate
and VAT6_2_1_Permanent.CustomerID=#NBRPrive.CustomerId
and isnull(VAT6_2_1_Permanent.DeclaredPrice,0)=0
@itemCondition3


update VAT6_2_1_Permanent set DeclaredPrice =#NBRPrive.Rate
from #NBRPrive
where #NBRPrive.ItemNo=VAT6_2_1_Permanent.ItemNo
and VAT6_2_1_Permanent.StartDateTime >=#NBRPrive.EffectDate and VAT6_2_1_Permanent.StartDateTime<#NBRPrive.ToDate
and isnull(VAT6_2_1_Permanent.DeclaredPrice,0)=0
@itemCondition3

update VAT6_2_1_Permanent set AdjustmentValue=0 where 1=1 @itemCondition3
update VAT6_2_1_Permanent set AdjustmentValue=   RunningTotalValue-RunningTotalValueFinal where 1=1 @itemCondition3
update VAT6_2_1_Permanent set RunningTotalValueFinal= DeclaredPrice*RunningTotal where 1=1 @itemCondition3

update VAT6_2_1_Permanent set Remarks= 'Sale' where 1=1 and Remarks = 'Other' @itemCondition3

drop table #Temp
drop table #NBRPrive
";
                if (!string.IsNullOrEmpty(vm.ItemNo))
                {
                    //generateClosingValues = generateClosingValues.Replace("@itemCondition1", " and VAT6_2.ItemNo = @ItemNo");
                    generateClosingValues = generateClosingValues.Replace("@itemCondition2", " and #NBRPrive.ItemNo = @ItemNo");
                    generateClosingValues = generateClosingValues.Replace("@itemCondition3", " and VAT6_2_1_Permanent.ItemNo = @ItemNo");
                }
                else
                {
                    //generateClosingValues = generateClosingValues.Replace("@itemCondition1", "");
                    generateClosingValues = generateClosingValues.Replace("@itemCondition2", "");
                    generateClosingValues = generateClosingValues.Replace("@itemCondition3", "");
                }

                cmd.CommandText = generateClosingValues;

                if (!string.IsNullOrEmpty(vm.ItemNo))
                {
                    cmd.Parameters.AddWithValueAndParamCheck("@ItemNo", vm.ItemNo);
                }

                cmd.ExecuteNonQuery();

                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                #endregion Commit
            }
            #endregion

            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";
                retResults[4] = ex.Message.ToString();
                if (transaction != null && Vtransaction == null) { transaction.Commit(); }

                throw;
            }
            finally
            {
                if (VcurrConn == null)
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
            #endregion
            #region Results
            return retResults;
            #endregion
        }


        public string[] SaveVAT6_2_1_Permanent_Branch(VAT6_2ParamVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            ReportDSDAL reportDsdal = new ReportDSDAL();
            ProductDAL productDal = new ProductDAL();

            CommonDAL commonDal = new CommonDAL();
            #endregion

            #region try

            try
            {
                #region open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnectionNoPooling(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                #endregion open connection and transaction

                #region Opening
                sqlText = @"
Insert into VAT6_2_1_Permanent_Branch(
[SerialNo]
,[StartDateTime]
,[StartingQuantity]
,[StartingAmount]
,[TransID]
,[TransType] 
,[Quantity]
,[SD]
,[UnitCost]   
,[Remarks] 
,[ItemNo]  
,RunningOpeningQuantityFinal
,RunningOpeningValueFinal
,UnitRate
,BranchId
)


SELECT distinct 'A'SerialNo, '1900-01-01' StartDateTime, 0 StartingQuantity, 0 StartingAmount,
0 TransID, 'Opening' TransType,
sum(isnull(p.StockQuantity,0)) Quantity,0 SD, sum(isnull(p.StockValue,0)) UnitCost, 'Opening'Remarks, p.ItemNo
,sum(isnull(StockQuantity,0)) Quantity, sum(isnull(p.StockValue,0)) UnitCost, sum(isnull(p.StockValue,0))/(case when sum(isnull(StockQuantity,0)) =0 then 1 else sum(isnull(StockQuantity,0)) end) UnitRate
,p.BranchId
FROM ProductStocks p  left outer join Products pd on pd.ItemNo = p.ItemNo
WHERE 
p.ItemNo  in(select ItemNo from Products where ReportType in ('VAT6_2_1'))
and p.ItemNo not in (select ItemNo from VAT6_2_1_Permanent_Branch where transType = 'Opening')

group by p.ItemNo,pd.ProductName,p.BranchId";

                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
                cmd.Parameters.AddWithValueAndParamCheck("@UserId", vm.UserId);
                cmd.CommandTimeout = 500;

                cmd.ExecuteNonQuery();

                #endregion

                #region Delete Existing Data

                string deleteText = @" delete from VAT6_2_1_Permanent_Branch where 1=1  and TransType != 'Opening'
                                and  StartDateTime>=@StartDate and StartDateTime<dateadd(d,1,@EndDate) @itemCondition";


                if (!string.IsNullOrEmpty(vm.ItemNo))
                {
                    deleteText = deleteText.Replace("@itemCondition", " and ItemNo = @ItemNo");
                }
                else
                {
                    deleteText = deleteText.Replace("@itemCondition", "");
                }


                cmd.CommandText = deleteText;
                cmd.Parameters.AddWithValue("@StartDate", vm.StartDate);
                cmd.Parameters.AddWithValue("@EndDate", vm.EndDate);

                if (!string.IsNullOrEmpty(vm.ItemNo))
                {
                    cmd.Parameters.AddWithValueAndParamCheck("@ItemNo", vm.ItemNo);
                }

                cmd.ExecuteNonQuery();

                #endregion

                DateTime paramMonth = Convert.ToDateTime(vm.StartDate);

                #region Insert Query

                string insertToPermanent = @"

delete from ProductStockMISKas where  ProductStockMISKas.TransType='Opening' @itemCondition

update ProductStockMISKas
set ItemNo = P.MasterProductItemNo
from Products p inner join ProductStockMISKas vp
on p.ItemNo = vp.ItemNo and p.MasterProductItemNo is not null and LEN(p.MasterProductItemNo) >0
where UserId = @UserId

insert into VAT6_2_1_Permanent_Branch(
[SerialNo]
      ,[StartDateTime]
      ,[StartingQuantity]
      ,[StartingAmount]
      ,[TransID]
      ,[TransType]
      ,[VendorName]
      ,[Address1]
      ,[Address2]
      ,[Address3]
      ,[VATRegistrationNo]
      ,[ProductName]
      ,[Quantity]
      ,[VATRate]
      ,[SD]
      ,[UnitCost]
      ,[HSCodeNo]
      ,[BENumber]
      ,[InvoiceDateTime]
      ,[Remarks]
      ,[ItemNo]
      ,[StockType]
      ,[BranchId]
	  )
SELECT 
	   [SerialNo]
      ,[StartDateTime]
      ,[StartingQuantity]
      ,[StartingAmount]
      ,[TransID]
      ,[TransType]
      ,[VendorName]
      ,[Address1]
      ,[Address2]
      ,[Address3]
      ,[VATRegistrationNo]
      ,[ProductName]
      ,[Quantity]
      ,[VATRate]
      ,[SD]
      ,[UnitCost]
      ,[HSCodeNo]
      ,[BENumber]
      ,[InvoiceDateTime]
      ,[Remarks]
      ,[ItemNo]
      ,[StockType]
      ,@BranchId
  FROM ProductStockMISKas where UserId = @UserId @itemCondition
  order by ItemNo, StartDateTime, SerialNo
";


                if (!string.IsNullOrEmpty(vm.ItemNo))
                {
                    insertToPermanent = insertToPermanent.Replace("@itemCondition", " and ItemNo = @ItemNo");
                }
                else
                {
                    insertToPermanent = insertToPermanent.Replace("@itemCondition", "");
                }

                cmd.CommandText = insertToPermanent;

                cmd.Parameters.AddWithValueAndParamCheck("@PeriodId", Convert.ToDateTime(vm.StartDate).ToString("MMyyyy"));
                cmd.Parameters.AddWithValueAndParamCheck("@StartDate", vm.StartDate);
                cmd.Parameters.AddWithValueAndParamCheck("@UserId", vm.UserId);

                if (!string.IsNullOrEmpty(vm.ItemNo))
                {
                    cmd.Parameters.AddWithValueAndParamCheck("@ItemNo", vm.ItemNo);
                }

                #endregion

                vm.PermanentProcess = true;

                BranchProfileDAL branchProfileDal = new BranchProfileDAL();
                List<BranchProfileVM> branchList = branchProfileDal.SelectAllList(null, null, null, currConn, transaction, connVM);

                foreach (BranchProfileVM branchProfileVm in branchList)
                {
                    vm.BranchId = branchProfileVm.BranchID;
                    string[] result = productDal.StockMovement6_2_1_PermanentProcess(vm, currConn, transaction, connVM);

                    cmd.Parameters.AddWithValueAndParamCheck("@BranchId", branchProfileVm.BranchID);
                    cmd.ExecuteNonQuery();

                }

                #region Generate Closings

                string generateClosingValues = @"
create table #NBRPrive(id int identity(1,1),ItemNo varchar(100) ,CustomerId varchar(100),Rate decimal(18,8), EffectDate datetime,ToDate datetime, BranchId int)
create table #Temp(SL int identity(1,1),Id int,ItemNo varchar(100),TransType varchar(100),Quantity decimal(18,8),TotalCost decimal(20,8),BranchId int)

update VAT6_2_1_Permanent_Branch set CustomerID=SalesInvoiceHeaders.CustomerID
from SalesInvoiceHeaders
where SalesInvoiceHeaders.Salesinvoiceno=VAT6_2_1_Permanent_Branch.TransID and TransType='Sale' @itemCondition3

insert into #NBRPrive
select itemNo, '' CustomerId ,
(
case 
when NBRPrice = 0 then ( case when OpeningBalance = 0 then 0 else OpeningTotalCost/OpeningBalance end) else ISNULL(NBRPrice,0) 
end
) NBRPrice, '1900/01/01'EffectDate ,null,BranchId ToDate from products
where ItemNo in(select distinct Itemno from VAT6_2_1_Permanent_Branch where 1=1 @itemCondition3) 

insert into #NBRPrive
select FinishItemNo,customerId, ISNULL(NBRPrice,0) NBRPrice,  EffectDate EffectDate ,null ToDate,BranchId from BOMs
where FinishItemNo in(select distinct Itemno from VAT6_2_1_Permanent_Branch where 1=1 @itemCondition3)
order by EffectDate

update #NBRPrive set  ToDate=null where 1=1 @itemCondition2

----######################----------------
update #NBRPrive set  ToDate=dateadd(s,-1,RT.RunningTotal)
from (SELECT Customerid,id, ItemNo,
LEAD(EffectDate) 
OVER (PARTITION BY BranchId,Customerid,ItemNo ORDER BY id) AS RunningTotal
FROM #NBRPrive
where customerid>0
)RT
where RT.Id=#NBRPrive.Id  and  RT.Customerid=#NBRPrive.Customerid 
and ToDate is null  @itemCondition2

update #NBRPrive set  ToDate=dateadd(s,-1,RT.RunningTotal)
from (SELECT id, ItemNo,
LEAD(EffectDate) 
OVER (PARTITION BY BranchId,ItemNo ORDER BY id) AS RunningTotal
FROM #NBRPrive
where isnull(nullif(customerid,''),0)<=0
)RT
where RT.Id=#NBRPrive.Id  
and ToDate is null and isnull(nullif(customerid,''),0)<=0  @itemCondition2
----######################----------------

update #NBRPrive set  ToDate='2199/12/31' where ToDate is null @itemCondition2

insert into #Temp(Id,ItemNo,TransType,Quantity,TotalCost,BranchId)
select Id,ItemNo,TransType,Quantity,UnitCost,BranchId from VAT6_2_1_Permanent_Branch 
where 1=1 @itemCondition3
order by BranchId,ItemNo,StartDateTime,SerialNo


update VAT6_2_1_Permanent_Branch set  RunningTotal=RT.RunningTotal
from (SELECT id,SL, ItemNo, TransType ,Quantity,BranchId,
SUM (case when TransType in('Sale') then -1*Quantity else Quantity end ) 
OVER (PARTITION BY BranchId,ItemNo ORDER BY  ItemNo,SL) AS RunningTotal
FROM #Temp)RT
where 
RT.Id=VAT6_2_1_Permanent_Branch.Id
and RT.BranchId=VAT6_2_1_Permanent_Branch.BranchId
@itemCondition3

update VAT6_2_1_Permanent_Branch set  RunningTotalValue=RT.RunningTotalCost
from (SELECT id,SL, ItemNo, TransType ,Quantity,BranchId,
SUM (case when TransType in('Sale') then -1*TotalCost else TotalCost end ) 
OVER (PARTITION BY BranchId,ItemNo ORDER BY SL) AS RunningTotalCost
FROM #Temp)RT
where 
RT.Id=VAT6_2_1_Permanent_Branch.Id
and RT.BranchId=VAT6_2_1_Permanent_Branch.BranchId
@itemCondition3

update VAT6_2_1_Permanent_Branch set DeclaredPrice =0

update VAT6_2_1_Permanent_Branch set DeclaredPrice =#NBRPrive.Rate
from #NBRPrive
where #NBRPrive.ItemNo=VAT6_2_1_Permanent_Branch.ItemNo
and VAT6_2_1_Permanent_Branch.StartDateTime >=#NBRPrive.EffectDate and VAT6_2_1_Permanent_Branch.StartDateTime<#NBRPrive.ToDate
and VAT6_2_1_Permanent_Branch.CustomerID=#NBRPrive.CustomerId
and isnull(VAT6_2_1_Permanent_Branch.DeclaredPrice,0)=0
@itemCondition3

update VAT6_2_1_Permanent_Branch set DeclaredPrice =#NBRPrive.Rate
from #NBRPrive
where #NBRPrive.ItemNo=VAT6_2_1_Permanent_Branch.ItemNo
and VAT6_2_1_Permanent_Branch.StartDateTime >=#NBRPrive.EffectDate and VAT6_2_1_Permanent_Branch.StartDateTime<#NBRPrive.ToDate
and isnull(VAT6_2_1_Permanent_Branch.DeclaredPrice,0)=0
@itemCondition3


update VAT6_2_1_Permanent_Branch set   RunningTotalValueFinal= DeclaredPrice*RunningTotal where 1=1 @itemCondition3
update VAT6_2_1_Permanent_Branch set AdjustmentValue=0 where 1=1 @itemCondition3
update VAT6_2_1_Permanent_Branch set AdjustmentValue=   RunningTotalValue-RunningTotalValueFinal where 1=1 @itemCondition3
update VAT6_2_1_Permanent_Branch set Remarks= 'Sale' where 1=1 and Remarks = 'Other' @itemCondition3


update VAT6_2_1_Permanent_Branch set  RunningOpeningQuantityFinal=RT.RunningTotalV
from ( SELECT  Id,BranchId,
LAG(RunningTotal) 
OVER (PARTITION BY BranchId,ItemNo ORDER BY  BranchId, itemno,StartDateTime,SerialNo) AS RunningTotalV
FROM VAT6_2_1_Permanent_Branch
)RT
where 
RT.Id=VAT6_2_1_Permanent_Branch.Id
and RT.BranchId=VAT6_2_1_Permanent_Branch.BranchId
@itemCondition3

 update VAT6_2_1_Permanent_Branch set  RunningOpeningValueFinal=RT.RunningTotalV
from ( SELECT  Id,BranchId,
LAG(RunningTotalValueFinal) 
OVER (PARTITION BY BranchId,ItemNo ORDER BY BranchId, itemno,StartDateTime,SerialNo) AS RunningTotalV
FROM VAT6_2_1_Permanent_Branch
)RT
where 
RT.Id=VAT6_2_1_Permanent_Branch.Id
and RT.BranchId=VAT6_2_1_Permanent_Branch.BranchId
@itemCondition3


drop table #Temp
drop table #NBRPrive
";
                if (!string.IsNullOrEmpty(vm.ItemNo))
                {

                    generateClosingValues = generateClosingValues.Replace("@itemCondition2", " and #NBRPrive.ItemNo = @ItemNo");
                    generateClosingValues = generateClosingValues.Replace("@itemCondition3", " and VAT6_2_1_Permanent_Branch.ItemNo = @ItemNo");
                }
                else
                {

                    generateClosingValues = generateClosingValues.Replace("@itemCondition2", "");
                    generateClosingValues = generateClosingValues.Replace("@itemCondition3", "");
                }

                cmd.CommandText = generateClosingValues;

                if (!string.IsNullOrEmpty(vm.ItemNo))
                {
                    cmd.Parameters.AddWithValueAndParamCheck("@ItemNo", vm.ItemNo);
                }

                cmd.ExecuteNonQuery();


                #endregion


                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                #endregion Commit
            }
            #endregion

            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";
                retResults[4] = ex.Message.ToString();
                if (transaction != null && Vtransaction == null) { transaction.Rollback(); }

                throw;
            }
            finally
            {
                if (VcurrConn == null)
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
            #endregion
            #region Results
            return retResults;
            #endregion
        }



        public string[] SaveVAT6_2_Permanent_Branch_Backup28082023(VAT6_2ParamVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            ReportDSDAL reportDsdal = new ReportDSDAL();

            CommonDAL commonDal = new CommonDAL();
            #endregion

            #region try

            try
            {
                #region open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnectionNoPooling(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                #endregion open connection and transaction

                #region Opening

                sqlText = reportDsdal.Get6_2OpeningQuery(ProcessConfig.Permanent_Branch);

                if (!string.IsNullOrEmpty(vm.ItemNo))
                {
                    sqlText = sqlText.Replace("@itemCondition", "and p.ItemNo = @ItemNo");
                }
                else
                {
                    sqlText = sqlText.Replace("@itemCondition", "");
                }

                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
                cmd.Parameters.AddWithValueAndParamCheck("@UserId", vm.UserId);
                cmd.Parameters.AddWithValueAndParamCheck("@BranchId", vm.BranchId);
                cmd.CommandTimeout = 500;

                if (!string.IsNullOrEmpty(vm.ItemNo))
                {
                    cmd.Parameters.AddWithValue("@ItemNo", vm.ItemNo);
                }

                cmd.ExecuteNonQuery();

                #endregion

                #region Delete Existing Data

                string deleteText = @" delete from VAT6_2_Permanent_Branch where 1=1  and TransType != 'Opening'
                                and  StartDateTime>=@StartDate and StartDateTime<dateadd(d,1,@EndDate)
                                @itemCondition";

                if (!string.IsNullOrEmpty(vm.ItemNo))
                {
                    deleteText = deleteText.Replace("@itemCondition", " and ItemNo = @ItemNo");
                }
                else if (vm.FilterProcessItems)
                {
                    deleteText = deleteText.Replace("@itemCondition", " and ItemNo in (select ItemNo from Products where ProcessFlag='Y' and (ReportType='VAT6_2' or ReportType='VAT6_1_And_6_2'))");
                }
                else
                {
                    deleteText = deleteText.Replace("@itemCondition", "");
                }

                cmd.CommandText = deleteText;
                cmd.Parameters.AddWithValue("@StartDate", vm.StartDate);
                cmd.Parameters.AddWithValue("@EndDate", vm.EndDate);
                //cmd.Parameters.AddWithValue("@PeriodId", periodId);

                if (!string.IsNullOrEmpty(vm.ItemNo))
                {
                    cmd.Parameters.AddWithValueAndParamCheck("@ItemNo", vm.ItemNo);
                }

                cmd.ExecuteNonQuery();


                #endregion

                string insertToPermanentTable = @"

create table #NBRPrive(id int identity(1,1),ItemNo varchar(100) ,CustomerId varchar(100),Rate decimal(18,6), EffectDate datetime,ToDate datetime, BranchId int)
create table #Temp(SL int identity(1,1),Id int,ItemNo varchar(100),TransType varchar(100),Quantity decimal(18,6),TotalCost decimal(18,6), BranchId int)

update VAT6_2 set CustomerID=ReceiveHeaders.CustomerID
from ReceiveHeaders
where ReceiveHeaders.ReceiveNo=VAT6_2.TransID
@itemCondition1

insert into #NBRPrive
select itemNo, 0 CustomerId ,ISNULL(NBRPrice,0) NBRPrice, '1900/01/01'EffectDate ,null ToDate,0 BranchId  from products
where ItemNo in(select distinct Itemno from VAT6_2 where 1=1 @itemCondition1) 

insert into #NBRPrive
select FinishItemNo,customerId, ISNULL(NBRPrice,0) NBRPrice,  EffectDate EffectDate ,null ToDate,0 BranchId  from BOMs
where FinishItemNo in(select distinct Itemno from VAT6_2 where 1=1 @itemCondition1 ) 
order by EffectDate

update #NBRPrive set  ToDate=null where 1=1 @itemCondition2

update #NBRPrive set  ToDate=RT.RunningTotal
from (SELECT id, ItemNo,
LEAD(EffectDate) 
OVER (PARTITION BY BranchId,customerId,ItemNo ORDER BY id) AS RunningTotal
FROM #NBRPrive)RT
where RT.Id=#NBRPrive.Id 
@itemCondition2


update #NBRPrive set  ToDate=RT.RunningTotal
from (SELECT id, ItemNo,
LEAD(EffectDate) 
OVER (PARTITION BY BranchId,Customerid,ItemNo ORDER BY id) AS RunningTotal
FROM #NBRPrive
where customerid>0
)RT
where RT.Id=#NBRPrive.Id  @itemCondition2

update #NBRPrive set  ToDate=RT.RunningTotal
from (SELECT id, ItemNo,
LEAD(EffectDate) 
OVER (PARTITION BY BranchId,ItemNo ORDER BY id) AS RunningTotal
FROM #NBRPrive

)RT
where RT.Id=#NBRPrive.Id @itemCondition2



update #NBRPrive set  ToDate='2199/12/31' where ToDate is null @itemCondition2

delete from VAT6_2 where  VAT6_2.TransType='Opening' @itemCondition1

insert into #Temp(Id,ItemNo,TransType,Quantity,TotalCost,BranchId)
select Id,ItemNo,TransType,Quantity,UnitCost,BranchId from VAT6_2 
where 1=1 @itemCondition1
order by BranchId,ItemNo,StartDateTime,SerialNo



update VAT6_2 set  RunningTotal=RT.RunningTotal
from (SELECT id,SL, ItemNo, TransType ,Quantity,BranchId,
SUM (case when TransType in('Sale') then -1*Quantity else Quantity end ) 
OVER (PARTITION BY BranchId,ItemNo ORDER BY  ItemNo,SL) AS RunningTotal
FROM #Temp)RT
where RT.Id=VAT6_2.Id 
and RT.BranchId = VAT6_2.BranchId
@itemCondition1


update VAT6_2 set  RunningTotalValue=RT.RunningTotalCost
from (SELECT id,SL, ItemNo, TransType ,Quantity,BranchId,
SUM (case when TransType in('Sale') then -1*TotalCost else TotalCost end ) 
OVER (PARTITION BY BranchId,ItemNo ORDER BY SL) AS RunningTotalCost
FROM #Temp)RT
where 
RT.Id=VAT6_2.Id
and RT.BranchId=VAT6_2.BranchId
@itemCondition1

update VAT6_2 set DeclaredPrice =#NBRPrive.Rate
from #NBRPrive
where #NBRPrive.ItemNo=VAT6_2.ItemNo
and VAT6_2.StartDateTime >=#NBRPrive.EffectDate and VAT6_2.StartDateTime<#NBRPrive.ToDate
and VAT6_2.CustomerID=#NBRPrive.CustomerId
@itemCondition1


----######################----------------
 update VAT6_2 set DeclaredPrice =#NBRPrive.Rate
from #NBRPrive
where #NBRPrive.ItemNo=VAT6_2.ItemNo
and VAT6_2.StartDateTime >=#NBRPrive.EffectDate and VAT6_2.StartDateTime<#NBRPrive.ToDate
and VAT6_2.CustomerID=0
@itemCondition1
------######################----------------
--
--
update VAT6_2 set DeclaredPrice =#NBRPrive.Rate
from #NBRPrive
where #NBRPrive.ItemNo=VAT6_2.ItemNo
and VAT6_2.StartDateTime >=#NBRPrive.EffectDate and VAT6_2.StartDateTime<#NBRPrive.ToDate
and nullif( VAT6_2.CustomerID,'') is null
@itemCondition1

update VAT6_2 set DeclaredPrice =#NBRPrive.Rate
from #NBRPrive
where #NBRPrive.ItemNo=VAT6_2.ItemNo
and VAT6_2.StartDateTime >=#NBRPrive.EffectDate and VAT6_2.StartDateTime<#NBRPrive.ToDate
and VAT6_2.CustomerID>0
@itemCondition1

update VAT6_2 set DeclaredPrice =#NBRPrive.Rate
from #NBRPrive
where #NBRPrive.ItemNo=VAT6_2.ItemNo
and VAT6_2.StartDateTime >=#NBRPrive.EffectDate and VAT6_2.StartDateTime<#NBRPrive.ToDate
and VAT6_2.CustomerID=#NBRPrive.CustomerId
@itemCondition1



UPDATE VAT6_2
SET RunningTotalValueFinal = CAST(DeclaredPrice  * RunningTotal AS decimal(18,4))
 where 1=1 @itemCondition1

update VAT6_2 set AdjustmentValue=0 where 1=1  @itemCondition1
update VAT6_2 set AdjustmentValue=RunningTotalValue-RunningTotalValueFinal where 1=1  @itemCondition1

 ----######################----------------

 
--delete from  #Temp
--delete from   #NBRPrive

--DBCC CHECKIDENT ('#Temp', RESEED, 0);
--DBCC CHECKIDENT ('#NBRPrive', RESEED, 0);

drop table #Temp
drop table #NBRPrive

insert into VAT6_2_Permanent_Branch(
[SerialNo]
,[StartDateTime]
,[StartingQuantity]
,[StartingAmount]
,[TransID]
,[TransType]
,[CustomerName]  
,[Address1] 
,[Address2]
,[Address3] 
,[VATRegistrationNo]
,[ProductName] 
,[ProductCode]
,[UOM]
,[HSCodeNo]
,[Quantity] 
,[VATRate]   
,[SD]   
,[UnitCost]
,[Remarks]   
,[CreatedDateTime]   
,[UnitRate]   
,[ItemNo]   
,[AdjustmentValue]
,[UserId]   
,[BranchId]    
,[CustomerID]  
,[ProductDesc]    
,[ClosingRate]    
,[DeclaredPrice]    
,[RunningTotal]
,[RunningTotalValue]
,[RunningTotalValueFinal]
,[RunningOpeningValueFinal]
,[RunningOpeningQuantityFinal]
,PeriodId
	  )

SELECT [SerialNo]
      ,[StartDateTime]
      ,[StartingQuantity]
      ,[StartingAmount]
      ,[TransID]
      ,[TransType]
      ,[CustomerName]
      ,[Address1]
      ,[Address2]
      ,[Address3]
      ,[VATRegistrationNo]
      ,[ProductName]
      ,[ProductCode]
      ,[UOM]
      ,[HSCodeNo]
      ,[Quantity]
      ,[VATRate]
      ,[SD]
      ,[UnitCost]
      ,[Remarks]
      ,[CreatedDateTime]
      ,[UnitRate]
      ,[ItemNo]
      ,[AdjustmentValue]
      ,[UserId]
      ,[BranchId]
      ,[CustomerID]
      ,[ProductDesc]
      ,[ClosingRate]
      ,[DeclaredPrice]
      ,[RunningTotal]
      ,[RunningTotalValue]
      ,[RunningTotalValueFinal]
      ,[RunningOpeningValueFinal]
      ,[RunningOpeningQuantityFinal]
	  ,Format(StartDateTime,'MMyyyy')PeriodId
  FROM VAT6_2 
where UserId = @UserId @itemCondition1
 order by ItemNo,StartDateTime,SerialNo

";

                #region VAT 6.2 Insert Condition

                if (!string.IsNullOrEmpty(vm.ItemNo))
                {
                    insertToPermanentTable = insertToPermanentTable.Replace("@itemCondition1", " and VAT6_2.ItemNo = @ItemNo");
                    insertToPermanentTable = insertToPermanentTable.Replace("@itemCondition2", " and #NBRPrive.ItemNo = @ItemNo");
                    insertToPermanentTable = insertToPermanentTable.Replace("@itemCondition3", " and VAT6_2_Permanent_Branch.ItemNo = @ItemNo");
                }
                else
                {
                    insertToPermanentTable = insertToPermanentTable.Replace("@itemCondition1", "");
                    insertToPermanentTable = insertToPermanentTable.Replace("@itemCondition2", "");
                    insertToPermanentTable = insertToPermanentTable.Replace("@itemCondition3", "");
                }

                cmd.CommandText = insertToPermanentTable;

                cmd.Parameters.AddWithValueAndParamCheck("@UserId", vm.UserId);
                cmd.Parameters.AddWithValueAndParamCheck("@PeriodId", Convert.ToDateTime(vm.StartDate).ToString("MMyyyy"));
                cmd.Parameters.AddWithValueAndParamCheck("@StartDate", vm.StartDate);

                if (!string.IsNullOrEmpty(vm.ItemNo))
                {
                    cmd.Parameters.AddWithValueAndParamCheck("@ItemNo", vm.ItemNo);
                }

                #endregion

                string updateMasterItem = @"
update VAT6_2
set ItemNo = isnull(P.MasterProductItemNo,P.ItemNo)
from Products p inner join VAT6_2 vp
on p.ItemNo = vp.ItemNo
where UserId = @UserId

";

                DateTime paramMonth = Convert.ToDateTime(vm.StartDate);
                vm.PermanentProcess = true;

                BranchProfileDAL branchProfileDal = new BranchProfileDAL();
                List<BranchProfileVM> branchList = branchProfileDal.SelectAllList();

                foreach (BranchProfileVM branchProfileVm in branchList)
                {
                    vm.BranchId = branchProfileVm.BranchID;
                    //DataSet dsResult = reportDsdal.VAT6_2(vm, currConn, transaction, connVM);

                    cmd.CommandText = updateMasterItem;
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = insertToPermanentTable;
                    cmd.ExecuteNonQuery();

                }


                #region VAT 6.2 Permanent Update Query

                string updatePermanentTable = reportDsdal.Get6_2PartitionQuery(ProcessConfig.Permanent_Branch);


                #endregion

                #region VAT 6.2 Update Condition

                if (!string.IsNullOrEmpty(vm.ItemNo))
                {
                    updatePermanentTable = updatePermanentTable.Replace("@itemCondition1", " and VAT6_2.ItemNo = @ItemNo");
                    updatePermanentTable = updatePermanentTable.Replace("@itemCondition2", " and #NBRPrive.ItemNo = @ItemNo");
                    updatePermanentTable = updatePermanentTable.Replace("@itemCondition3", " and VAT6_2_Permanent_Branch.ItemNo = @ItemNo");
                }
                else
                {
                    updatePermanentTable = updatePermanentTable.Replace("@itemCondition1", "");
                    updatePermanentTable = updatePermanentTable.Replace("@itemCondition2", "");
                    updatePermanentTable = updatePermanentTable.Replace("@itemCondition3", "");
                }

                cmd.CommandText = updatePermanentTable;

                cmd.Parameters.AddWithValueAndParamCheck("@UserId", vm.UserId);
                cmd.Parameters.AddWithValueAndParamCheck("@PeriodId", Convert.ToDateTime(vm.StartDate).ToString("MMyyyy"));
                cmd.Parameters.AddWithValueAndParamCheck("@StartDate", vm.StartDate);

                if (!string.IsNullOrEmpty(vm.ItemNo))
                {
                    cmd.Parameters.AddWithValueAndParamCheck("@ItemNo", vm.ItemNo);
                }

                cmd.ExecuteNonQuery();

                #endregion


                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                #endregion Commit


            }
            #endregion

            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
                if (transaction != null && Vtransaction == null)
                {
                    transaction.Rollback();
                    //transaction.Commit();
                }
                FileLogger.Log("IssueDAL", "SaveTempIssue", ex.ToString() + "\n" + sqlText);

                throw;
            }
            finally
            {
                if (VcurrConn == null)
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
            #endregion
            #region Results
            return retResults;
            #endregion
        }

        public string[] SaveToTempIssue(DataTable dtTable, string transactionType, string userId, int branchId, Action callBack, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion

            #region try

            try
            {
                #region open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnectionNoPooling(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                #endregion open connection and transaction

                #region DateFormat

                foreach (DataRow dataRow in dtTable.Rows)
                {
                    dataRow["Issue_DateTime"] = OrdinaryVATDesktop.DateToDate(dataRow["Issue_DateTime"].ToString());
                }
                #endregion

                CommonDAL commonDal = new CommonDAL();

                sqlText = @"delete from TempIssueData where UserId = '" + userId + "'";

                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);

                cmd.ExecuteNonQuery();

                dtTable.Columns.Add(new DataColumn() { ColumnName = "UserId", DefaultValue = userId });
                dtTable.Columns.Add(new DataColumn() { ColumnName = "BranchId", DefaultValue = branchId });

                commonDal.BulkInsert("TempIssueData", dtTable, currConn, transaction, 10000, null, connVM);

                string UpdateReference = @"

--update TempIssueData set ID = FORMAT(Issue_DateTime, 'ddMMyyyy') where UserId = @userID

create table #Refs
(
	SL int Identity(1,1),
	ID varchar(100),
	RefNo varchar(6000),
	UserID varchar(50),

)
Insert Into #Refs (ID, RefNo,userID)
select distinct ID, Reference_No,UserId from TempIssueData where UserId = @userID

create table #updtRefs
(
	SL int Identity(1,1),
	ID varchar(100),
	RefNo varchar(6000),
	UserID varchar(50),
)

insert into #updtRefs (ID, RefNo, UserID)
SELECT DISTINCT temp2.ID, 
    SUBSTRING(
        (
            SELECT ','+temp1.RefNo  AS [text()]
            FROM #Refs temp1
            WHERE temp1.ID = temp2.ID and temp1.UserID = temp2.UserID
            ORDER BY temp1.ID
            FOR XML PATH ('')
        ), 2, 6000) [Refs], temp2.UserID
FROM #Refs temp2



update TempIssueData set Reference_No = #updtRefs.RefNo 
from #updtRefs where TempIssueData.ID = #updtRefs.ID and #updtRefs.UserId = TempIssueData.UserId

drop table #Refs
drop table #updtRefs";

                string itemUpdate = @"
update TempIssueData set ItemNo = Products.ItemNo from Products where Products.ProductCode = TempIssueData.Item_Code and (TempIssueData.ItemNo is null or TempIssueData.ItemNo = '0');
update TempIssueData set ItemNo = Products.ItemNo from Products where Products.ProductName = TempIssueData.Item_Name and (TempIssueData.ItemNo is null or TempIssueData.ItemNo = '0');

update TempIssueData set UOMn = Products.UOM 
from Products where Products.ItemNo = TempIssueData.ItemNo 


update TempIssueData set TempIssueData.UOMc = UOMs.Convertion from UOMs 
where  UOMs.UOMFrom = TempIssueData.UOMn 
and UOMs.UOMTo = TempIssueData.UOM and (TempIssueData.UOMc = 0 or TempIssueData.UOMc is null)

update TempIssueData set UOMc = (case when UOM = UOMn then 1.00 else UOMc end)
where UOMc = 0 or UOMc is null

";

                string branchCode =
                    @"update TempIssueData set BranchId = BranchProfiles.BranchID from BranchProfiles where BranchProfiles.BranchCode = TempIssueData.BranchCode;";

                string bom = @"update TempIssueData 
set BOMId = BOMRaws.BOMId 
from BOMRaws where BOMRaws.RawItemNo = TempIssueData.ItemNo 
and BOMRaws.effectdate<= cast(TempIssueData.Issue_DateTime as datetime) and BOMRaws.post='Y';";

                cmd.CommandText = UpdateReference;

                cmd.Parameters.AddWithValue("@userID", userId);

                cmd.ExecuteNonQuery();
                cmd.ExecuteNonQuery();
                cmd.CommandText = itemUpdate + " " + branchCode + " " + bom;

                cmd.ExecuteNonQuery();

                cmd.CommandText = "select  top 1 * from TempIssueData where (ItemNo is null or ItemNo = '' or ItemNo = '0') and UserId = @userID";
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dtProducts = new DataTable();
                adapter.Fill(dtProducts);

                if (dtProducts.Rows.Count > 0)
                {
                    throw new Exception("Product Not Found " + dtProducts.Rows[0]["Item_Code"]);
                }

                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                #endregion Commit

                #region SuccessResult
                retResults[0] = "Success";
                retResults[1] = "Data Synchronized Successfully.";
                retResults[2] = "";
                #endregion SuccessResult
            }
            #endregion

            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex

                ////FileLogger.Log("Issue DAL", "Save issue", ex.Message + " " + ex.StackTrace);

                FileLogger.Log("IssueDAL", "SaveToTempIssue", ex.ToString() + "\n" + sqlText);

                if (transaction != null && Vtransaction == null) { transaction.Rollback(); }

                throw ex;
            }
            finally
            {
                if (VcurrConn == null)
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
            #endregion
            #region Results
            return retResults;
            #endregion
        }

        private string[] SaveIssue(DataTable dtTableResult, string transactionType, string CurrentUser, int branchId, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, Action callBack = null, SysDBInfoVMTemp connVM = null, string branchCode = "")
        {
            #region try

            #region variables

            DataTable dtIssueM = new DataTable();
            DataTable dtIssueD = new DataTable();

            #endregion

            try
            {
                if (!dtTableResult.Columns.Contains("BranchCode"))
                {
                    var column = new DataColumn("BranchCode") { DefaultValue = branchCode };
                    dtTableResult.Columns.Add(column);
                }
                DataView view = new DataView(dtTableResult);
                try
                {
                    List<string> ara = new List<string>() {"ID", "Issue_DateTime", "Reference_No", "Comments", "Return_Id",
                        "Post", "BranchCode", "BranchId"};

                    if (dtTableResult.Columns.Contains("Transection_Type"))
                    {
                        ara.Add("Transection_Type");
                    }

                    dtIssueM = view.ToTable(true, ara.ToArray());
                    dtIssueD = view.ToTable(false, "ID", "Item_Code", "Item_Name", "Quantity", "UOM", "ItemNo", "BomId");
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                if (!dtTableResult.Columns.Contains("Transection_Type"))
                {
                    dtIssueM = OrdinaryVATDesktop.DtColumnAdd(dtIssueM, "Transection_Type", transactionType, "string");
                }
                dtIssueM = OrdinaryVATDesktop.DtColumnAdd(dtIssueM, "Created_By", CurrentUser, "string");
                dtIssueM = OrdinaryVATDesktop.DtColumnAdd(dtIssueM, "LastModified_By", CurrentUser, "string");

                dtIssueM = OrdinaryVATDesktop.DtDateCheck(dtIssueM, new string[] { "Issue_DateTime" });

                return ImportData(dtIssueM, dtIssueD, branchId, VcurrConn, Vtransaction, callBack, connVM, CurrentUser);

            }

            #endregion

            catch (Exception ex)
            {
                FileLogger.Log("IssueDAL", "SaveIssue", ex.ToString());

                throw ex;
            }
        }

        public DataTable GetExcelData(List<string> invoiceList, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion

            #region try

            try
            {
                #region open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnectionNoPooling(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                #endregion open connection and transaction


                sqlText = @"SELECT 
      ih.[IssueNo] ID
	  ,bp.BranchCode 
      ,

CONVERT(varchar, 
convert(varchar(100),ih.[IssueDateTime],111) +' '+convert(varchar(100),ih.[IssueDateTime],108) 
)

Issue_DateTime

      ,ih.[SerialNo] Reference_No
      ,ih.[Comments]

      ,ih.[IssueReturnId] Return_Id
      ,ih.[Post]
	  , pd.ProductCode Item_Code 
	  , pd.ProductName Item_Name 
	  , ids.Quantity
	  ,ids.UOM
	  ,SUBSTRING(ih.ImportIDExcel, 1, CHARINDEX('~', ih.ImportIDExcel + '~') - 1) AS ImportID
  FROM [IssueHeaders] ih left outer join IssueDetails ids
  on ih.IssueNo = ids.IssueNo left outer join Products pd
  on ids.ItemNo = pd.ItemNo left outer join BranchProfiles bp
  on ih.BranchId = bp.BranchID

  where ih.IssueNo in ( ";

                var len = invoiceList.Count;

                for (int i = 0; i < len; i++)
                {
                    sqlText += "'" + invoiceList[i] + "'";

                    if (i != (len - 1))
                    {
                        sqlText += ",";
                    }
                }

                if (len == 0)
                {
                    sqlText += "''";
                }

                sqlText += ")";

                var cmd = new SqlCommand(sqlText, currConn, transaction);

                var table = new DataTable();
                var adapter = new SqlDataAdapter(cmd);

                adapter.Fill(table);

                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                #endregion Commit

                return table;

            }
            #endregion

            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
                if (transaction != null && Vtransaction == null) { transaction.Rollback(); }
                FileLogger.Log("IssueDAL", "GetExcelData", ex.ToString() + "\n" + sqlText);

                throw ex;
            }
            finally
            {
                if (VcurrConn == null)
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
            #endregion
        }

        public DataTable GetExcelDataWeb(List<string> invoiceList, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion

            #region try

            try
            {
                #region open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnectionNoPooling(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                #endregion open connection and transaction


                sqlText = @"SELECT 
      ih.[IssueNo] ID
	  ,bp.BranchCode 
      ,

CONVERT(varchar, 
convert(varchar(100),ih.[IssueDateTime],111) +' '+convert(varchar(100),ih.[IssueDateTime],108) 
)

Issue_DateTime

      ,ih.[SerialNo] Reference_No
      ,ih.[Comments]

      ,ih.[IssueReturnId] Return_Id
      ,ih.[Post]
	  , pd.ProductCode Item_Code 
	  , pd.ProductName Item_Name 
	  , ids.Quantity
	  ,ids.UOM
	  ,SUBSTRING(ih.ImportIDExcel, 1, CHARINDEX('~', ih.ImportIDExcel + '~') - 1) AS ImportID

  FROM [IssueHeaders] ih left outer join IssueDetails ids
  on ih.IssueNo = ids.IssueNo left outer join Products pd
  on ids.ItemNo = pd.ItemNo left outer join BranchProfiles bp
  on ih.BranchId = bp.BranchID

  where ih.Id in ( ";

                var len = invoiceList.Count;

                for (int i = 0; i < len; i++)
                {
                    sqlText += "'" + invoiceList[i] + "'";

                    if (i != (len - 1))
                    {
                        sqlText += ",";
                    }
                }

                if (len == 0)
                {
                    sqlText += "''";
                }

                sqlText += ")";

                var cmd = new SqlCommand(sqlText, currConn, transaction);

                var table = new DataTable();
                var adapter = new SqlDataAdapter(cmd);

                adapter.Fill(table);

                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                #endregion Commit

                return table;

            }
            #endregion

            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
                if (transaction != null && Vtransaction == null) { transaction.Rollback(); }
                FileLogger.Log("IssueDAL", "GetExcelDataWeb", ex.ToString() + "\n" + sqlText);

                throw ex;
            }
            finally
            {
                if (VcurrConn == null)
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
            #endregion
        }

        public DataTable SelectAllHeaderTemp(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SaleMasterVM likeVM = null, bool Dt = false, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            DataTable dt = new DataTable();
            #endregion

            #region try

            try
            {
                #region open connection and transaction
                #region New open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }
                #endregion New open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                #endregion open connection and transaction
                #region sql statement
                #region SqlText

                sqlText = @"
SELECT distinct
Ish.SL
,Ish.ID
,Ish.BranchCode
,Ish.Issue_DateTime IssueDateTime
,Ish.Reference_No SerialNo
,Ish.Comments
,Ish.Return_Id IssueReturnId
,Ish.Post
,Ish.Item_Code
,Ish.Item_Name
,Ish.Quantity
,Ish.UOM
,Ish.ItemNo
,Ish.BranchId
,Ish.BomId
,Ish.UserId

FROM TempIssueData Ish 
WHERE  1=1 
        ";


                if (Id > 0)
                {
                    sqlText += @" and Ish.ID=@Id";
                }


                string cField = "";
                if (conditionFields != null && conditionValues != null && conditionFields.Length == conditionValues.Length)
                {
                    for (int i = 0; i < conditionFields.Length; i++)
                    {
                        if (string.IsNullOrEmpty(conditionFields[i]) || string.IsNullOrEmpty(conditionValues[i]) || conditionValues[i] == "0")
                        {
                            continue;
                        }
                        cField = conditionFields[i].ToString();
                        cField = OrdinaryVATDesktop.StringReplacing(cField);
                        if (conditionFields[i].ToLower().Contains("like"))
                        {
                            sqlText += " AND " + conditionFields[i] + " '%'+ " + " @" + cField.Replace("like", "").Trim() + " +'%'";
                        }
                        else if (conditionFields[i].Contains(">") || conditionFields[i].Contains("<"))
                        {
                            sqlText += " AND " + conditionFields[i] + " @" + cField;

                        }
                        else
                        {
                            sqlText += " AND " + conditionFields[i] + "= @" + cField;
                        }
                    }
                }


                #endregion SqlText

                #region SqlExecution

                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;

                if (conditionFields != null && conditionValues != null && conditionFields.Length == conditionValues.Length)
                {
                    for (int j = 0; j < conditionFields.Length; j++)
                    {
                        if (string.IsNullOrEmpty(conditionFields[j]) || string.IsNullOrEmpty(conditionValues[j]) || conditionValues[j] == "0")
                        {
                            continue;
                        }
                        cField = conditionFields[j].ToString();
                        cField = OrdinaryVATDesktop.StringReplacing(cField);
                        if (conditionFields[j].ToLower().Contains("like"))
                        {
                            da.SelectCommand.Parameters.AddWithValue("@" + cField.Replace("like", "").Trim(), conditionValues[j]);
                        }
                        else
                        {
                            da.SelectCommand.Parameters.AddWithValue("@" + cField, conditionValues[j]);
                        }
                    }
                }
                //if (likeVM != null)
                //{
                //    if (!string.IsNullOrEmpty(likeVM.SalesInvoiceNo))
                //    {
                //        da.SelectCommand.Parameters.AddWithValue("@SalesInvoiceNo", "%" + likeVM.SalesInvoiceNo + "%");
                //    }
                //    if (!string.IsNullOrEmpty(likeVM.SerialNo))
                //    {
                //        da.SelectCommand.Parameters.AddWithValue("@SerialNo", "%" + likeVM.SerialNo + "%");
                //    }
                //}
                if (Id > 0)
                {
                    da.SelectCommand.Parameters.AddWithValue("@Id", Id);
                }
                da.Fill(dt);

                #endregion SqlExecution




                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
                #endregion
            }
            #endregion

            #region catch
            catch (SqlException sqlex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());

                FileLogger.Log("IssueDAL", "SelectAllHeaderTemp", sqlex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());

                FileLogger.Log("IssueDAL", "SelectAllHeaderTemp", ex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", ex.Message.ToString());

            }
            #endregion
            #region finally
            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion
            return dt;
        }

        public DataTable SearchIssueDetailTemp(string IssueNo, string userId, SysDBInfoVMTemp connVM = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            DataTable dataTable = new DataTable("SearchIssueDetail");

            #endregion

            #region Try
            try
            {
                #region open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                #endregion open connection and transaction

                #region open connection and transaction

                ////currConn = _dbsqlConnection.GetConnection(connVM);
                ////if (currConn.State != ConnectionState.Open)
                ////{
                ////    currConn.Open();
                ////}

                //transaction = currConn.BeginTransaction();
                #endregion open connection and transaction

                #region SQL Statement

                sqlText = @"
                            SELECT    
ISNULL(Isd.BOMId,0) BOMId
,'' IssueNo
,'0' IssueLineNo
,Isd.ItemNo
,Isd.BranchCode
,Isd.Issue_DateTime
,Isd.Comments
,Isd.Post
,Isd.Item_Code
,Isd.Item_Name
,Sum(Isd.Quantity)Quantity
,Isd.UOM
,Isd.ItemNo
,Isd.BranchId
,Isd.BomId
,Isd.UserId
,Products.ProductCode
,Products.ProductName
,isd.UOMn
,isnull(isd.UOMc,0)UOMc
,isd.Issue_DateTime IssueDateTime

FROM  TempIssueData Isd LEFT OUTER JOIN
Products ON Isd.ItemNo = Products.ItemNo          
 
where ID = @Id and UserId = @userId 
group by 
Isd.ItemNo
,Isd.BranchCode
,Isd.Issue_DateTime
,Isd.Comments
,Isd.Post
,Isd.Item_Code
,Isd.Item_Name
,Isd.BomId
,Isd.UOM
,Isd.BranchId
,Isd.BomId
,Isd.UserId
,Products.ProductCode
,Products.ProductName
,isd.UOMn
,isd.Issue_DateTime
,isd.UOMc
--order by InvoiceLineNo
                            ";



                string VAT6_3OrderByProduct = new CommonDAL().settings("Reports", "VAT6_3OrderByProduct", currConn, transaction, connVM);


                if (VAT6_3OrderByProduct.ToLower() == "y")
                {
                    sqlText += @"  order by Products.ProductCode";
                }
                else
                {
                    sqlText += @"  order by IssueLineNo";

                }

                #endregion

                #region SQL Command

                SqlCommand objCommSaleDetail = new SqlCommand();
                objCommSaleDetail.Connection = currConn;

                objCommSaleDetail.Transaction = transaction;

                objCommSaleDetail.CommandText = sqlText;
                objCommSaleDetail.CommandType = CommandType.Text;

                #endregion.

                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                #endregion Commit

                #region Parameter

                objCommSaleDetail.Parameters.AddWithValue("@Id", IssueNo);
                objCommSaleDetail.Parameters.AddWithValue("@userId", userId);

                #endregion Parameter

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommSaleDetail);
                dataAdapter.Fill(dataTable);
            }
            #endregion

            #region Catch & Finally
            catch (Exception ex)
            {
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Rollback();
                }
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;

                FileLogger.Log("IssueDAL", "SearchIssueDetailTemp", ex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", ex.Message.ToString());

            }
            finally
            {
                if (VcurrConn == null)
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

            #endregion

            return dataTable;

        }

        #region Web Methods

        public List<IssueMasterVM> SelectAllList(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, IssueMasterVM likeVM = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            //SqlConnection currConn = null;
            //SqlTransaction transaction = null;
            string sqlText = "";
            List<IssueMasterVM> VMs = new List<IssueMasterVM>();
            IssueMasterVM vm;
            #endregion
            #region try

            try
            {

                #region sql statement

                #region SqlExecution


                int index = -1;
                if (conditionFields != null && conditionValues != null)
                {
                    index = Array.IndexOf(conditionFields, "SelectTop");

                }

                DataTable dt = SelectAll(Id, conditionFields, conditionValues, VcurrConn, Vtransaction, null, false, connVM);

                if (dt != null && dt.Rows.Count > 0)
                {

                    if (index >= 0)
                    {
                        dt.Rows.RemoveAt(dt.Rows.Count - 1);
                    }


                    foreach (DataRow dr in dt.Rows)
                    {
                        vm = new IssueMasterVM();
                        vm.Id = dr["Id"].ToString();
                        vm.IssueNo = dr["IssueNo"].ToString();
                        vm.IssueDateTime = OrdinaryVATDesktop.DateTimeToDate(dr["IssueDateTime"].ToString());
                        vm.TotalVATAmount = Convert.ToDecimal(dr["TotalVATAmount"]);
                        vm.TotalAmount = Convert.ToDecimal(dr["TotalAmount"]);
                        vm.SerialNo = dr["SerialNo"].ToString();
                        vm.Comments = dr["Comments"].ToString();
                        vm.CreatedBy = dr["CreatedBy"].ToString();
                        vm.CreatedOn = dr["CreatedOn"].ToString();
                        vm.LastModifiedBy = dr["LastModifiedBy"].ToString();
                        vm.LastModifiedOn = dr["LastModifiedOn"].ToString();
                        vm.ReceiveNo = dr["ReceiveNo"].ToString();
                        vm.transactionType = dr["TransactionType"].ToString();
                        vm.ShiftId = dr["ShiftId"].ToString();
                        vm.Post = dr["Post"].ToString();
                        ////reading newly added fields
                        vm.ReturnId = dr["IssueReturnId"].ToString();
                        vm.ImportId = dr["ImportIDExcel"].ToString();
                        if (vm.ImportId != null && !string.IsNullOrWhiteSpace(vm.ImportId))
                        {
                            vm.ImportId = dr["ImportIDExcel"].ToString().Split('~')[0];
                        }
                        vm.BranchId = Convert.ToInt32(dr["BranchId"].ToString());

                        VMs.Add(vm);
                    }

                }


                #endregion SqlExecution

                //if (Vtransaction == null && transaction != null)
                //{
                //    transaction.Commit();
                //}
                #endregion
            }
            #endregion

            #region catch
            catch (SqlException sqlex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());

                FileLogger.Log("IssueDAL", "SelectAllList", sqlex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());

                FileLogger.Log("IssueDAL", "SelectAllList", ex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", ex.Message.ToString());

            }
            #endregion
            #region finally
            //finally
            //{
            //    if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
            //    {
            //        currConn.Close();
            //    }
            //}
            #endregion
            return VMs;
        }

        public DataTable SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, IssueMasterVM paramVM = null, bool Dt = false, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            string sqlTextParameter = "";
            string sqlTextOrderBy = "";
            string sqlTextCount = "";
            string count = "100";

            DataTable dt = new DataTable();
            DataSet ds = new DataSet();

            #endregion

            #region try

            try
            {
                #region open connection and transaction
                #region New open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }
                #endregion New open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                #endregion open connection and transaction

                #region Select Top

                int index = -1;
                if (conditionFields != null && conditionValues != null)
                {
                    index = Array.IndexOf(conditionFields, "SelectTop");
                    if (index >= 0)
                    {
                        count = conditionValues[index].ToString();

                        var field = conditionFields.ToList();
                        var Values = conditionValues.ToList();
                        field.RemoveAt(index);
                        Values.RemoveAt(index);
                        conditionFields = field.ToArray();
                        conditionValues = Values.ToArray();
                    }
                }

                #endregion

                #region sql statement
                #region SqlText
                #region Sql Text

                if (count.ToLower() == "All".ToLower())
                {
                    sqlText = @"SELECT";
                }
                else
                {
                    sqlText = @"SELECT top " + count + " ";
                }

                sqlText += @"

 
IssueNo
,IssueDateTime
,ISNULL(TotalVATAmount,0) TotalVATAmount 
,ISNULL(TotalAmount,0) TotalAmount 
,SerialNo
,Comments
,CreatedBy
,CreatedOn
,LastModifiedBy
,LastModifiedOn
,ReceiveNo
,TransactionType
,Post
,ImportIDExcel
,isnull(BranchId,0) BranchId
,IssueReturnId
,ShiftId
,Id
,ISNULL(FiscalYear,0) FiscalYear
FROM IssueHeaders  
WHERE  1=1
";

                #endregion SqlText

                sqlTextCount += @" 
select count(IssueNo)RecordCount
FROM IssueHeaders  
WHERE  1=1
";

                if (Id > 0)
                {
                    sqlTextParameter += @" and Id=@Id";
                }

                string cField = "";
                if (conditionFields != null && conditionValues != null && conditionFields.Length == conditionValues.Length)
                {
                    for (int i = 0; i < conditionFields.Length; i++)
                    {
                        if (string.IsNullOrWhiteSpace(conditionFields[i]) || string.IsNullOrWhiteSpace(conditionValues[i]) || conditionValues[i] == "0")
                        {
                            continue;
                        }
                        cField = conditionFields[i].ToString();
                        cField = OrdinaryVATDesktop.StringReplacing(cField);
                        if (conditionFields[i].ToLower().Contains("like"))
                        {
                            sqlTextParameter += " AND " + conditionFields[i] + " '%'+ " + " @" + cField.Replace("like", "").Trim() + " +'%'";
                        }
                        else if (conditionFields[i].Contains(">") || conditionFields[i].Contains("<"))
                        {
                            sqlTextParameter += " AND " + conditionFields[i] + " @" + cField;

                        }
                        else
                        {
                            sqlTextParameter += " AND " + conditionFields[i] + "= @" + cField;
                        }
                    }
                }

                if (paramVM != null)
                {
                    if (paramVM.IssueOnly == true)
                    {
                        sqlTextParameter += @" AND TransactionType IN 
(
'Other'
,'TollitemIssueWithoutBOM'
,'IssueReturn'
,'IssueWastage'
)";

                    }
                }
                #endregion SqlText

                #region SqlExecution

                sqlText = sqlText + " " + sqlTextParameter;
                sqlTextCount = sqlTextCount + " " + sqlTextParameter;
                sqlText = sqlText + " " + sqlTextCount;

                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;

                if (conditionFields != null && conditionValues != null && conditionFields.Length == conditionValues.Length)
                {
                    for (int j = 0; j < conditionFields.Length; j++)
                    {
                        if (string.IsNullOrWhiteSpace(conditionFields[j]) || string.IsNullOrWhiteSpace(conditionValues[j]) || conditionValues[j] == "0")
                        {
                            continue;
                        }

                        cField = conditionFields[j].ToString();
                        cField = OrdinaryVATDesktop.StringReplacing(cField);
                        if (conditionFields[j].ToLower().Contains("like"))
                        {
                            da.SelectCommand.Parameters.AddWithValue("@" + cField.Replace("like", "").Trim(), conditionValues[j]);
                        }
                        else
                        {
                            da.SelectCommand.Parameters.AddWithValue("@" + cField, conditionValues[j]);
                        }
                    }
                }
                if (Id > 0)
                {
                    da.SelectCommand.Parameters.AddWithValue("@Id", Id);
                }
                da.Fill(ds);


                #endregion SqlExecution

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }


                dt = ds.Tables[0].Copy();
                if (index >= 0)
                {
                    dt.Rows.Add(ds.Tables[1].Rows[0][0]);
                }
                #endregion
            }
            #endregion

            #region catch
            catch (SqlException sqlex)
            {
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Rollback();
                }
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());

                FileLogger.Log("IssueDAL", "SelectAll", sqlex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Rollback();
                }
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());

                FileLogger.Log("IssueDAL", "SelectAll", ex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", ex.Message.ToString());

            }
            #endregion
            #region finally

            finally
            {
                if (VcurrConn == null)
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
            ////finally
            ////{
            ////    if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
            ////    {
            ////        currConn.Close();
            ////    }
            ////}
            #endregion
            return dt;
        }

        public List<IssueDetailVM> SelectIssueDetail(string issueNo, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null, bool multipleIssue = false)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<IssueDetailVM> VMs = new List<IssueDetailVM>();
            IssueDetailVM vm;
            #endregion

            #region try

            try
            {
                #region open connection and transaction
                #region New open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }
                #endregion New open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                #endregion open connection and transaction
                #region sql statement
                #region SqlText

                sqlText = @"
 SELECT
 iss.Id
,iss.IssueNo
,iss.IssueLineNo
,iss.ItemNo
,ISNULL(iss.Quantity,0) Quantity  
,ISNULL(iss.NBRPrice,0) NBRPrice  
,ISNULL(iss.CostPrice,0) CostPrice  
,iss.UOM
,ISNULL(iss.VATRate,0) VATRate  
,ISNULL(iss.VATAmount,0) VATAmount  
,ISNULL(iss.SubTotal,0) SubTotal  
,iss.Comments
,iss.CreatedBy
,iss.CreatedOn
,iss.LastModifiedBy
,iss.LastModifiedOn
,iss.ReceiveNo
,iss.IssueDateTime
,ISNULL(iss.SD,0) SD  
,ISNULL(iss.SDAmount,0) SDAmount  
,ISNULL(iss.Wastage,0) Wastage  
,iss.BOMDate
,iss.FinishItemNo
,iss.Post
,iss.TransactionType
,iss.IssueReturnId
,ISNULL(iss.DiscountAmount,0) DiscountAmount  
,ISNULL(iss.DiscountedNBRPrice,0) DiscountedNBRPrice  
,iss.UOMQty
,ISNULL(iss.UOMPrice,0) UOMPrice  
,ISNULL(iss.UOMc,0) UOMc  
,iss.UOMn
,iss.BOMId
,ISNULL(iss.UOMWastage,0) UOMWastage  
,iss.IsProcess
,p.ProductCode
,p.ProductName
FROM IssueDetails iss left outer join Products p on iss.ItemNo=p.ItemNo
WHERE  1=1

";
                if (issueNo != null)
                {
                    sqlText += "AND iss.IssueNo=@issueNo";
                }

                string cField = "";
                if (conditionFields != null && conditionValues != null && conditionFields.Length == conditionValues.Length)
                {
                    for (int i = 0; i < conditionFields.Length; i++)
                    {
                        if (string.IsNullOrWhiteSpace(conditionFields[i]) || string.IsNullOrWhiteSpace(conditionValues[i]))
                        {
                            continue;
                        }
                        cField = conditionFields[i].ToString();
                        cField = OrdinaryVATDesktop.StringReplacing(cField);
                        sqlText += " AND " + conditionFields[i] + "=@" + cField;
                    }
                }

                sqlText += " order by iss.IssueNo";

                #endregion SqlText
                #region SqlExecution

                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);

                if (issueNo != null)
                {
                    objComm.Parameters.AddWithValue("@issueNo", issueNo);
                }

                if (conditionFields != null && conditionValues != null && conditionFields.Length == conditionValues.Length)
                {
                    for (int j = 0; j < conditionFields.Length; j++)
                    {
                        if (string.IsNullOrWhiteSpace(conditionFields[j]) || string.IsNullOrWhiteSpace(conditionValues[j]))
                        {
                            continue;
                        }
                        cField = conditionFields[j].ToString();
                        cField = OrdinaryVATDesktop.StringReplacing(cField);
                        objComm.Parameters.AddWithValue("@" + cField, conditionValues[j]);
                    }
                }
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new IssueDetailVM();

                    if (!multipleIssue)
                    {
                        vm.Id = dr["Id"].ToString();
                        vm.IssueNo = dr["IssueNo"].ToString();
                        vm.ItemNo = dr["ItemNo"].ToString();
                        vm.Quantity = Convert.ToDecimal(dr["Quantity"].ToString());
                        vm.NBRPrice = Convert.ToDecimal(dr["NBRPrice"].ToString());
                        vm.CostPrice = Convert.ToDecimal(dr["CostPrice"].ToString());
                        vm.UOM = dr["UOM"].ToString();
                        vm.VATRate = Convert.ToDecimal(dr["VATRate"].ToString());
                        vm.VATAmount = Convert.ToDecimal(dr["VATAmount"].ToString());
                        vm.SubTotal = Convert.ToDecimal(dr["SubTotal"].ToString());
                        vm.SD = Convert.ToDecimal(dr["SD"].ToString());
                        vm.SDAmount = Convert.ToDecimal(dr["SDAmount"].ToString());
                        vm.Wastage = Convert.ToDecimal(dr["Wastage"].ToString());
                        vm.FinishItemNo = dr["FinishItemNo"].ToString();
                        vm.Post = dr["Post"].ToString();
                        vm.UOMQty = Convert.ToDecimal(dr["UOMQty"].ToString());
                        vm.UOMPrice = Convert.ToDecimal(dr["UOMPrice"].ToString());
                        vm.UOMc = Convert.ToDecimal(dr["UOMc"].ToString());
                        vm.UOMn = dr["UOMn"].ToString();
                        vm.ReceiveNo = dr["ReceiveNo"].ToString();
                        vm.IssueDateTime = dr["IssueDateTime"].ToString();
                        vm.transactionType = dr["TransactionType"].ToString();
                        vm.IssueReturnId = dr["IssueReturnId"].ToString();
                        vm.DiscountAmount = Convert.ToDecimal(dr["DiscountAmount"].ToString());
                        vm.DiscountedNBRPrice = Convert.ToDecimal(dr["DiscountedNBRPrice"].ToString());
                        vm.BOMId = Convert.ToInt32(dr["BOMId"]);
                        vm.IsProcess = dr["IsProcess"].ToString();
                        vm.UOMWastage = Convert.ToDecimal(dr["UOMWastage"].ToString());
                        vm.ItemName = dr["ProductName"].ToString();
                        vm.ProductCode = dr["ProductCode"].ToString();
                        vm.CreatedBy = dr["CreatedBy"].ToString();
                        vm.CreatedOn = dr["CreatedOn"].ToString();
                        vm.LastModifiedBy = dr["LastModifiedBy"].ToString();
                        vm.LastModifiedOn = dr["LastModifiedOn"].ToString();
                        ////reading newly added fields
                        vm.IssueLineNo = dr["IssueLineNo"].ToString();
                        vm.CommentsD = dr["Comments"].ToString();
                        vm.BOMDate = dr["BOMDate"].ToString();
                    }
                    else
                    {
                        vm.Id = dr["Id"].ToString();
                        vm.IssueNo = dr["IssueNo"].ToString();
                        vm.ItemNo = dr["ItemNo"].ToString();
                        vm.Quantity = Convert.ToDecimal(dr["Quantity"].ToString());
                        vm.NBRPrice = Convert.ToDecimal(dr["NBRPrice"].ToString());
                        vm.CostPrice = Convert.ToDecimal(dr["CostPrice"].ToString());
                        vm.UOM = dr["UOM"].ToString();
                        vm.UOMn = dr["UOMn"].ToString();
                        vm.IssueDateTime = dr["IssueDateTime"].ToString();

                    }


                    VMs.Add(vm);
                }
                dr.Close();

                #endregion SqlExecution

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
                #endregion
            }
            #endregion

            #region catch
            //catch (SqlException sqlex)
            //{
            //    throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            //}
            catch (Exception ex)
            {

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Rollback();
                }
                ////FileLogger.Log("IssueDAL", "Search Issue Details", ex.Message + ex.StackTrace);
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());

                FileLogger.Log("IssueDAL", "SelectIssueDetail", ex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", ex.Message.ToString());

            }
            #endregion
            #region finally

            finally
            {
                if (VcurrConn == null)
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

            ////finally
            ////{
            ////    if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
            ////    {
            ////        currConn.Close();
            ////    }
            ////}
            #endregion
            return VMs;
        }

        public string[] ImportExcelFile(IssueMasterVM paramVM, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";
            int Id = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "ImportExcelFile"; //Method Name
            #endregion

            #region try
            try
            {


                DataSet ds = new DataSet();
                //DataTable dt = new DataTable();
                #region Excel Reader

                string FileName = paramVM.File.FileName;
                //string Fullpath = AppDomain.CurrentDomain.BaseDirectory + "Files\\Export\\" + FileName;
                //System.IO.File.Delete(Fullpath);
                //if (paramVM.File != null && paramVM.File.ContentLength > 0)
                //{
                //    paramVM.File.SaveAs(Fullpath);
                //}


                //FileStream stream = File.Open(Fullpath, FileMode.Open, FileAccess.Read);
                IExcelDataReader reader = null;
                if (FileName.EndsWith(".xls"))
                {
                    reader = ExcelReaderFactory.CreateBinaryReader(paramVM.File.InputStream);
                }
                else if (FileName.EndsWith(".xlsx"))
                {
                    reader = ExcelReaderFactory.CreateOpenXmlReader(paramVM.File.InputStream);
                }
                reader.IsFirstRowAsColumnNames = true;
                ds = reader.AsDataSet();


                //dt = ds.Tables[0];
                reader.Close();
                // System.IO.File.Delete(Fullpath);
                #endregion

                DataTable dtIssueM = new DataTable();
                dtIssueM = ds.Tables["IssueM"];
                DataTable dtTableResult = new DataTable();
                CommonDAL commonDal = new CommonDAL();

                if (dtIssueM != null)
                {
                    if (dtIssueM.Columns.Contains("ImportID"))
                    {
                        dtIssueM.Columns.Remove("ImportID");
                    }
                }

                string CustomeDateFormat = commonDal.settingValue("Integration", "CustomeDateFormat", connVM, null, null);

                if (string.IsNullOrWhiteSpace(CustomeDateFormat) && CustomeDateFormat == "-")
                {
                    dtTableResult = OrdinaryVATDesktop.DtDateCheck(dtIssueM, new string[] { "Issue_DateTime" });
                }
                else
                {
                    dtTableResult = dtIssueM.Copy();
                }

                //DataTable dtIssueD = new DataTable();
                //dtIssueD = ds.Tables["IssueD"];


                //dtIssueM.Columns.Add("Transection_Type");
                //dtIssueM.Columns.Add("Created_By");
                //dtIssueM.Columns.Add("LastModified_By");
                //foreach (DataRow row in dtIssueM.Rows)
                //{
                //    row["Transection_Type"] = paramVM.transactionType;
                //    row["Created_By"] = paramVM.CreatedBy;
                //    row["LastModified_By"] = paramVM.CreatedBy;
                //}

                #region Data Insert
                //PurchaseDAL puchaseDal = new PurchaseDAL();
                retResults = SaveTempIssue(dtTableResult, paramVM.transactionType, paramVM.CreatedBy, paramVM.BranchId,
                    () => { }, null, null, connVM);

                if (retResults[0] == "Fail")
                {
                    throw new ArgumentNullException("", retResults[1]);
                }
                #endregion

                #region SuccessResult
                retResults[0] = "Success";
                retResults[1] = "Data Save Successfully.";
                //retResults[2] = vm.Id.ToString();
                #endregion SuccessResult
            }
            #endregion try
            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[1] = ex.Message.ToString(); //catch ex
                retResults[4] = ex.Message.ToString(); //catch ex
                FileLogger.Log("IssueDAL", "ImportExcelFile", ex.ToString() + "\n" + sqlText);

                return retResults;
            }
            finally
            {
            }
            #endregion
            #region Results
            return retResults;
            #endregion
        }

        private void SetDefaultValue(IssueMasterVM vm, SysDBInfoVMTemp connVM = null)
        {
            if (string.IsNullOrWhiteSpace(vm.Comments))
            {
                vm.Comments = "-";
            }
            if (string.IsNullOrWhiteSpace(vm.SerialNo))
            {
                vm.SerialNo = "-";
            }
            if (string.IsNullOrWhiteSpace(vm.ReceiveNo))
            {
                vm.ReceiveNo = "-";
            }
        }

        #endregion

        #region Plain Methods

        public string[] IssueInsertToMaster(IssueMasterVM Master, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string[] retResults = new string[5];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            int transResult = 0;
            string sqlText = "";

            string PostStatus = "";

            int IDExist = 0;


            #endregion Initializ

            #region Try
            try
            {
                #region open connection and transaction
                #region New open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }
                #endregion New open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                #endregion open connection and transaction
                #region Entry Date Check

                string firstDate = "01-July-2019";
                if (Convert.ToDateTime(Master.IssueDateTime) < Convert.ToDateTime(firstDate))
                {
                    retResults[1] = "No Entry Allowed Before " + firstDate + "!";
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, retResults[1]);
                }

                #endregion
                #region  Insert new Information in Header


                sqlText = "";
                sqlText += " insert into IssueHeaders(";
                sqlText += " IssueNo,";
                sqlText += " IssueDateTime,";
                sqlText += " TotalVATAmount,";
                sqlText += " TotalAmount,";
                sqlText += " SerialNo,";
                sqlText += " Comments,";
                sqlText += " CreatedBy,";
                sqlText += " CreatedOn,";
                sqlText += " LastModifiedBy,";
                sqlText += " LastModifiedOn,";
                sqlText += " ReceiveNo,";
                sqlText += " transactionType,";
                sqlText += " IssueReturnId,";
                sqlText += " ImportIDExcel,";
                sqlText += " ShiftId,";
                sqlText += " BranchId,";
                sqlText += " Post,";
                sqlText += " IssueNumber,";
                sqlText += " AppVersion,";
                sqlText += " FiscalYear";
                sqlText += " )";

                sqlText += " values";
                sqlText += " (";
                sqlText += " @MasterIssueNo,";
                sqlText += " @MasterIssueDateTime,";
                sqlText += " @MasterTotalVATAmount,";
                sqlText += " @MasterTotalAmount,";
                sqlText += " @MasterSerialNo,";
                sqlText += " @MasterComments,";
                sqlText += " @MasterCreatedBy,";
                sqlText += " @MasterCreatedOn,";
                sqlText += " @MasterLastModifiedBy,";
                sqlText += " @MasterLastModifiedOn,";
                sqlText += " @MasterReceiveNo,";
                sqlText += " @MastertransactionType,";
                sqlText += " @MasterReturnId,";
                sqlText += " @MasterImportId,";
                sqlText += " @MasterShiftId,";
                sqlText += " @BranchId,";
                sqlText += " @MasterPost,";
                sqlText += " @IssueNumber,";
                sqlText += " @AppVersion,";
                sqlText += " @FiscalYear";
                sqlText += ")  SELECT SCOPE_IDENTITY()";


                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;

                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterIssueNo", Master.IssueNo);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterIssueDateTime", Master.IssueDateTime);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterTotalVATAmount", Master.TotalVATAmount);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterTotalAmount", Master.TotalAmount);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterSerialNo", Master.SerialNo);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterComments", Master.Comments);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterCreatedBy", Master.CreatedBy);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterCreatedOn", OrdinaryVATDesktop.DateToDate(Master.CreatedOn));
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", OrdinaryVATDesktop.DateToDate(Master.LastModifiedOn));
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MastertransactionType", Master.transactionType);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterReturnId", Master.ReturnId);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterImportId", Master.ImportId);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterShiftId", Master.ShiftId);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@BranchId", Master.BranchId);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterPost", Master.Post);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@IssueNumber", Master.IssueNumber);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@FiscalYear", Master.FiscalYear);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@AppVersion", Master.AppVersion);

                //transResult = Convert.ToInt32(cmdInsert.ExecuteNonQuery());
                transResult = Convert.ToInt32(cmdInsert.ExecuteScalar());
                retResults[4] = transResult.ToString();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, MessageVM.issueMsgSaveNotSuccessfully);
                }


                #endregion ID generated completed,Insert new Information in Header

                #region Commit

                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        if (transResult > 0)
                        {
                            transaction.Commit();
                        }
                    }
                }

                #endregion Commit
                #region SuccessResult

                retResults[0] = "Success";
                retResults[1] = MessageVM.issueMsgSaveSuccessfully;
                retResults[2] = "" + Master.IssueNo;
                retResults[3] = "" + PostStatus;
                #endregion SuccessResult

            }
            #endregion Try

            #region Catch and Finall
            //catch (SqlException sqlex)
            //{
            //    if (Vtransaction == null)
            //    {
            //        transaction.Rollback();
            //    }
            //    throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            //    //throw sqlex;
            //}
            catch (Exception ex)
            {
                retResults = new string[5];
                retResults[0] = "Fail";
                retResults[1] = ex.Message;
                retResults[2] = "0";
                retResults[3] = "N";
                //retResults[4] = "0";
                if (Vtransaction == null && transaction != null)
                {

                    transaction.Rollback();
                }

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;

                FileLogger.Log("IssueDAL", "IssueInsertToMaster", ex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", ex.Message.ToString());

            }
            finally
            {
                if (VcurrConn == null)
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
            #endregion Catch and Finall

            #region Result
            return retResults;
            #endregion Result

        }

        public string[] IssueInsertToDetails(IssueDetailVM Detail, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            int transResult = 0;
            string sqlText = "";

            string PostStatus = "";

            #endregion Initializ

            #region Try
            try
            {
                #region open connection and transaction
                #region New open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }
                #endregion New open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                #endregion open connection and transaction

                #region Insert

                sqlText = "";
                sqlText += @" insert into IssueDetails(

VATName
,WIPBOMId
,BOMId
,IssueNo
,IssueLineNo
,ItemNo
,Quantity
,NBRPrice
,CostPrice
,UOM
,VATRate
,VATAmount
,SubTotal
,Comments
,CreatedBy
,CreatedOn
,LastModifiedBy
,LastModifiedOn
,ReceiveNo
,IssueDateTime
,SD
,SDAmount
,Wastage
,BOMDate
,FinishItemNo
,transactionType
,IssueReturnId
,UOMQty
,UOMPrice
,UOMc
,UOMn
,BranchId
,Post

) Values (	

@VATName
,@WIPBOMId
,@BOMId
,@DetailIssueNo
,@DetailIssueLineNo
,@DetailItemNo
,@DetailQuantity
,0
,@DetailCostPrice
,@DetailUOM
,@DetailVATRate
,@DetailVATAmount
,@DetailSubTotal
,@DetailCommentsD
,@DetailCreatedBy
,@DetailCreatedOn
,@DetailLastModifiedBy
,@DetailLastModifiedOn
,@DetailReceiveNoD
,@DetailIssueDateTimeD
,0
,0
,@DetailWastage
,@DetailBOMDate
,@DetailFinishItemNo
,@DetailtransactionType
,@DetailReturnId
,@DetailUOMQty
,@DetailUOMPrice
,@DetailUOMc
,@DetailUOMn
,@BranchId
,@DetailPost
)	
";


                SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                cmdInsDetail.Transaction = transaction;

                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@VATName", Detail.VATName);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@WIPBOMId", Detail.WIPBOMId);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BOMId", Detail.BOMId);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@DetailIssueNo", Detail.IssueNo);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@DetailIssueLineNo", Detail.IssueLineNo);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@DetailItemNo", Detail.ItemNo);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@DetailQuantity", Detail.Quantity);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@DetailCostPrice", Detail.CostPrice);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@DetailUOM", Detail.UOM);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@DetailVATRate", Detail.VATRate);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@DetailVATAmount", Detail.VATAmount);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@DetailSubTotal", Detail.SubTotal);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@DetailCommentsD", Detail.CommentsD);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@DetailCreatedBy", Detail.CreatedBy);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@DetailCreatedOn", OrdinaryVATDesktop.DateToDate(Detail.CreatedOn));
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@DetailLastModifiedBy", Detail.LastModifiedBy);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@DetailLastModifiedOn", OrdinaryVATDesktop.DateToDate(Detail.LastModifiedOn));
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@DetailReceiveNoD", Detail.ReceiveNoD);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@DetailIssueDateTimeD", Detail.IssueDateTime);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@DetailWastage", Detail.Wastage);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@DetailBOMDate", Detail.BOMDate);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@DetailFinishItemNo", Detail.FinishItemNo);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@DetailtransactionType", Detail.transactionType);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@DetailReturnId", Detail.ReturnId);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@DetailUOMQty", Detail.UOMQty);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@DetailUOMPrice", Detail.UOMPrice);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@DetailUOMc", Detail.UOMc);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@DetailUOMn", Detail.UOMn);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BranchId", Detail.BranchId);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@DetailPost", Detail.Post);

                transResult = (int)cmdInsDetail.ExecuteNonQuery();

                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, MessageVM.issueMsgSaveNotSuccessfully);
                }
                #endregion Insert only DetailTable

                #region Commit

                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        if (transResult > 0)
                        {
                            transaction.Commit();
                        }
                    }
                }

                #endregion Commit

                #region SuccessResult

                retResults[0] = "Success";
                retResults[1] = MessageVM.issueMsgSaveSuccessfully;
                retResults[2] = "" + Detail.IssueNo;
                retResults[3] = "" + PostStatus;
                #endregion SuccessResult

            }
            #endregion Try

            #region Catch and Finall
            catch (OverflowException exception)
            {
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Rollback();
                }

                throw new OverflowException(exception.Message + "\n" + Detail.ItemNo + "\n" + Detail.SubTotal);

            }
            catch (Exception ex)
            {
                retResults = new string[5];
                retResults[0] = "Fail";
                retResults[1] = ex.Message;
                retResults[2] = "0";
                retResults[3] = "N";
                //retResults[4] = "0";
                if (Vtransaction == null && transaction != null)
                {

                    transaction.Rollback();
                }

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;

                FileLogger.Log("IssueDAL", "IssueInsertToDetails", ex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", ex.Message.ToString());

            }
            finally
            {
                if (VcurrConn == null)
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
            #endregion Catch and Finall

            #region Result
            return retResults;
            #endregion Result

        }

        public string[] IssueUpdateToMaster(IssueMasterVM Master, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            string sqlText = "";
            int nextId = 0;

            #endregion Initializ

            #region Try
            try
            {
                #region open connection and transaction
                #region New open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }
                #endregion New open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                #endregion open connection and transaction

                #region Entry Date Check

                string firstDate = "01-July-2019";
                if (Convert.ToDateTime(Master.IssueDateTime) < Convert.ToDateTime(firstDate))
                {
                    retResults[1] = "No Entry Allowed Before " + firstDate + "!";
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, retResults[1]);
                }

                #endregion

                #region update
                sqlText = "";

                sqlText += " update IssueHeaders set  ";

                sqlText += "  IssueDateTime     =@MasterIssueDateTime";
                sqlText += " ,TotalVATAmount    =@MasterTotalVATAmount";
                sqlText += " ,TotalAmount       =@MasterTotalAmount";
                sqlText += " ,SerialNo          =@MasterSerialNo";
                sqlText += " ,Comments          =@MasterComments";
                sqlText += " ,LastModifiedBy    =@MasterLastModifiedBy ";
                sqlText += " ,LastModifiedOn    =@MasterLastModifiedOn";
                sqlText += " ,ReceiveNo         =@MasterReceiveNo";
                sqlText += " ,transactionType   =@MastertransactionType";
                sqlText += " ,IssueReturnId     =@MasterReturnId ";
                sqlText += " ,ShiftId           =@MasterShiftId ";
                sqlText += " ,BranchId          =@BranchId ";
                sqlText += " ,Post              =@Post ";
                sqlText += "  where  IssueNo =@MasterIssueNo ";


                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;

                cmdUpdate.Parameters.AddWithValue("@MasterIssueDateTime", Master.IssueDateTime);
                cmdUpdate.Parameters.AddWithValue("@MasterTotalVATAmount", Master.TotalVATAmount);
                cmdUpdate.Parameters.AddWithValue("@BranchId", Master.BranchId);
                cmdUpdate.Parameters.AddWithValue("@MasterTotalAmount", Master.TotalAmount);
                cmdUpdate.Parameters.AddWithValue("@MasterSerialNo", Master.SerialNo ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterComments", Master.Comments ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn);
                cmdUpdate.Parameters.AddWithValue("@MasterReceiveNo", Master.ReceiveNo ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MastertransactionType", Master.transactionType ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterReturnId", Master.ReturnId ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterShiftId", Master.ShiftId ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterIssueNo", Master.IssueNo ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@Post", "N");

                transResult = (int)cmdUpdate.ExecuteNonQuery();

                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, MessageVM.issueMsgUpdateNotSuccessfully);
                }

                #endregion

                #region Commit

                if (Vtransaction == null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                    }

                }

                #endregion Commit

                #region SuccessResult

                retResults[0] = "Success";
                retResults[1] = MessageVM.issueMsgUpdateSuccessfully;
                retResults[2] = Master.Id;
                #endregion SuccessResult

            }
            #endregion Try

            #region Catch and Finall
            #region Comments

            //catch (SqlException sqlex)
            //{
            //    if (Vtransaction == null)
            //    {
            //        transaction.Rollback();
            //    }
            //    throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            //    //throw sqlex;
            //}
            #endregion

            catch (Exception ex)
            {
                retResults = new string[5];
                retResults[0] = "Fail";
                retResults[1] = ex.Message;
                retResults[2] = "0";
                retResults[3] = "N";
                retResults[4] = "0";
                if (Vtransaction == null && transaction != null)
                {

                    transaction.Rollback();
                }

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;

                FileLogger.Log("IssueDAL", "IssueUpdateToMaster", ex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", ex.Message.ToString());

            }
            finally
            {
                if (VcurrConn == null)
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
            #endregion Catch and Finall

            #region Result
            return retResults;
            #endregion Result

        }

        public string[] IssueUpdateToDetails(IssueDetailVM Detail, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            string sqlText = "";

            #endregion Initializ

            #region Try
            try
            {
                #region open connection and transaction
                #region New open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }
                #endregion New open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                #endregion open connection and transaction

                #region update
                sqlText = "";

                sqlText += " update IssueDetails set  ";

                sqlText += "  IssueNo            =@IssueNo";
                sqlText += " ,IssueLineNo        =@IssueLineNo";
                sqlText += " ,ItemNo             =@ItemNo";
                sqlText += " ,Quantity           =@Quantity";
                sqlText += " ,NBRPrice           =@NBRPrice";
                sqlText += " ,CostPrice          =@CostPrice";
                sqlText += " ,UOM                =@UOM";
                sqlText += " ,VATRate            =@VATRate";
                sqlText += " ,VATAmount          =@VATAmount";
                sqlText += " ,SubTotal           =@SubTotal";
                sqlText += " ,CreatedBy          =@CreatedBy";
                sqlText += " ,CreatedOn          =@CreatedOn";
                sqlText += " ,LastModifiedBy     =@LastModifiedBy";
                sqlText += " ,LastModifiedOn     =@LastModifiedOn";
                sqlText += " ,ReceiveNo          =@ReceiveNo";
                sqlText += " ,IssueDateTime      =@IssueDateTime";
                sqlText += " ,SD                 =@SD";
                sqlText += " ,SDAmount           =@SDAmount";
                sqlText += " ,Wastage            =@Wastage";
                sqlText += " ,BOMDate            =@BOMDate";
                sqlText += " ,FinishItemNo       =@FinishItemNo";
                sqlText += " ,Post               =@Post";
                sqlText += " ,TransactionType    =@TransactionType";
                sqlText += " ,IssueReturnId      =@IssueReturnId";
                sqlText += " ,DiscountAmount     =@DiscountAmount";
                sqlText += " ,DiscountedNBRPrice =@DiscountedNBRPrice";
                sqlText += " ,UOMQty             =@UOMQty";
                sqlText += " ,UOMPrice           =@UOMPrice";
                sqlText += " ,UOMc               =@UOMc";
                sqlText += " ,UOMn               =@UOMn";
                sqlText += " ,BOMId              =@BOMId";
                sqlText += " ,UOMWastage         =@UOMWastage";
                sqlText += " ,IsProcess          =@IsProcess";
                sqlText += " ,BranchId          =@BranchId";
                sqlText += " where             Id=@Id ";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;

                cmdUpdate.Parameters.AddWithValueAndNullHandle("@IssueNo", Detail.IssueNo);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@IssueLineNo", Detail.IssueLineNo);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@ItemNo", Detail.ItemNo);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@Quantity", Detail.Quantity);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@NBRPrice", Detail.NBRPrice);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@CostPrice", Detail.CostPrice);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@UOM", Detail.UOM);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@VATRate", Detail.VATRate);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@VATAmount", Detail.VATAmount);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@SubTotal", Detail.SubTotal);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@CreatedBy", Detail.CreatedBy);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@CreatedOn", OrdinaryVATDesktop.DateToDate(Detail.CreatedOn));
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@LastModifiedBy", Detail.LastModifiedBy);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@LastModifiedOn", OrdinaryVATDesktop.DateToDate(Detail.LastModifiedOn));
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@ReceiveNo", Detail.ReceiveNo);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@IssueDateTime", Detail.IssueDateTime);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@SD", Detail.SD);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@SDAmount", Detail.SDAmount);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@Wastage", Detail.Wastage);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@BOMDate", Detail.BOMDate);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@FinishItemNo", Detail.FinishItemNo);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@Post", "N");
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@TransactionType", Detail.transactionType);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@IssueReturnId", Detail.IssueReturnId);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@DiscountAmount", Detail.DiscountAmount);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@DiscountedNBRPrice", Detail.DiscountedNBRPrice);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@UOMQty", Detail.UOMQty);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@UOMPrice", Detail.UOMPrice);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@UOMc", Detail.UOMc);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@UOMn", Detail.UOMn);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@BOMId", Detail.BOMId);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@UOMWastage", Detail.UOMWastage);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@IsProcess", Detail.IsProcess);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@BranchId", Detail.BranchId);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@Id", Detail.Id);


                transResult = (int)cmdUpdate.ExecuteNonQuery();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, MessageVM.issueMsgUpdateNotSuccessfully);
                }
                #endregion update Header

                #region Commit

                if (Vtransaction == null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                    }

                }

                #endregion Commit

                #region SuccessResult

                retResults[0] = "Success";
                retResults[1] = MessageVM.issueMsgUpdateSuccessfully;
                retResults[2] = Detail.Id;
                #endregion SuccessResult

            }
            #endregion Try

            #region Catch and Finall
            //catch (SqlException sqlex)
            //{
            //    if (Vtransaction == null)
            //    {
            //        transaction.Rollback();
            //    }
            //    throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            //    //throw sqlex;
            //}
            catch (Exception ex)
            {
                retResults = new string[5];
                retResults[0] = "Fail";
                retResults[1] = ex.Message;
                retResults[2] = "0";
                retResults[3] = "N";
                retResults[4] = "0";
                if (Vtransaction == null && transaction != null)
                {

                    transaction.Rollback();
                }

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;

                FileLogger.Log("IssueDAL", "IssueUpdateToDetails", ex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", ex.Message.ToString());

            }
            finally
            {
                if (VcurrConn == null)
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
            #endregion Catch and Finall

            #region Result
            return retResults;
            #endregion Result

        }

        public string[] IssueAllPost(PostVM Master, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {

            #region Initializ
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            int transResult = 0;
            string sqlText = "";
            string PostStatus = "";

            #endregion Initializ

            #region Try
            try
            {
                #region open connection and transaction
                #region New open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }
                #endregion New open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                #endregion open connection and transaction

                #region Update Master

                sqlText = "";
                sqlText += " update IssueHeaders set  ";
                sqlText += " LastModifiedBy             = @MasterLastModifiedBy,";
                sqlText += " LastModifiedOn             = @MasterLastModifiedOn,";
                sqlText += " Post                       = @MasterPost";
                sqlText += " where  IssueNo   = @MasterPurchaseInvoiceNo ";

                sqlText += " update IssueDetails set ";
                sqlText += " LastModifiedBy= @MasterLastModifiedBy,";
                sqlText += " LastModifiedOn= @MasterLastModifiedOn,";
                sqlText += " Post=@MasterPost ";
                sqlText += " where  IssueNo =@MasterPurchaseInvoiceNo ";


                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterPost", Master.Post);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterPurchaseInvoiceNo", Master.Code);

                transResult = (int)cmdUpdate.ExecuteNonQuery();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                    MessageVM.PurchasemsgSaveNotSuccessfully);
                }


                #endregion

                #region Commit


                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        if (transResult > 0)
                        {
                            transaction.Commit();
                        }
                    }
                }

                #endregion Commit

                #region SuccessResult

                retResults[0] = "Success";
                retResults[1] = MessageVM.PurchasemsgSaveSuccessfully;
                retResults[2] = "";
                retResults[3] = "" + PostStatus;
                #endregion SuccessResult

            }
            #endregion Try

            #region Catch and Finall
            //catch (SqlException sqlex)
            //{
            //    if (Vtransaction == null)
            //    {
            //        transaction.Rollback();
            //    }
            //    throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            //    //throw sqlex;
            //}
            catch (Exception ex)
            {
                retResults = new string[5];
                retResults[0] = "Fail";
                retResults[1] = ex.Message;
                retResults[2] = "0";
                retResults[3] = "N";
                retResults[4] = "0";
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Rollback();
                }
                FileLogger.Log("IssueDAL", "IssueAllPost", ex.ToString() + "\n" + sqlText);

            }
            finally
            {
                if (VcurrConn == null)
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
            #endregion Catch and Finall

            #region Result
            return retResults;
            #endregion Result

        }

        #endregion

        #region Basic Methods

        //currConn to VcurrConn 25-Aug-2020 ok
        public string[] IssueInsert(IssueMasterVM Master, List<IssueDetailVM> Details, SqlTransaction Vtransaction, SqlConnection VcurrConn, SysDBInfoVMTemp connVM = null)
        {

            #region Initializ
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";
            var Id = "";
            #region Check user from settings
            //SettingDAL settingDal=new SettingDAL();
            //   bool isAllowUser = settingDal.CheckUserAccess();
            //   if (!isAllowUser)
            //   {
            //       throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, MessageVM.msgAccessPermision);
            //   }
            #endregion

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            ////SqlConnection vcurrConn = currConn;
            ////if (vcurrConn == null)
            ////{
            ////    currConn = null;
            ////    Vtransaction = null;
            ////}

            int transResult = 0;
            string sqlText = "";

            string newID = "";
            string PostStatus = "";

            int IDExist = 0;


            #endregion Initializ

            #region Try
            try
            {
                #region Validation for Header


                if (Master == null)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, MessageVM.issueMsgNoDataToSave);
                }
                else if (Convert.ToDateTime(Master.IssueDateTime) < DateTime.MinValue || Convert.ToDateTime(Master.IssueDateTime) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, "Please Check Issue Data and Time");

                }


                #endregion Validation for Header

                #region Old connection

                #region open connection and transaction
                ////if (vcurrConn == null)
                ////{
                ////    currConn = _dbsqlConnection.GetConnection(connVM);
                ////    if (currConn.State != ConnectionState.Open)
                ////    {
                ////        currConn.Open();
                ////    }

                ////    Vtransaction = currConn.BeginTransaction(MessageVM.issueMsgMethodNameInsert);
                ////}

                #endregion open connection and transaction

                #endregion Old connection

                #region open connection and transaction

                #region New open connection and transaction

                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                #endregion New open connection and transaction

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction(MessageVM.depMsgMethodNameInsert);
                }

                #endregion open connection and transaction

                #region Find Month Lock

                string PeriodName = Convert.ToDateTime(Master.IssueDateTime).ToString("MMMM-yyyy");
                string[] vValues = { PeriodName };
                string[] vFields = { "PeriodName" };
                FiscalYearVM varFiscalYearVM = new FiscalYearVM();
                varFiscalYearVM = new FiscalYearDAL().SelectAll(0, vFields, vValues, currConn, transaction, connVM).FirstOrDefault();

                if (varFiscalYearVM == null)
                {
                    throw new Exception(PeriodName + ": This Fiscal Period is not Exist!");

                }

                if (varFiscalYearVM.VATReturnPost == "Y")
                {
                    throw new Exception(PeriodName + ": VAT Return (9.1) already submitted for this month!");

                }

                #endregion Find Month Lock

                #region Fiscal Year Check

                string transactionDate = Master.IssueDateTime;

                string transactionYearCheck = Convert.ToDateTime(Master.IssueDateTime).ToString("yyyy-MM-dd");

                if (Convert.ToDateTime(transactionYearCheck) > DateTime.MinValue || Convert.ToDateTime(transactionYearCheck) < DateTime.MaxValue)
                {

                    #region YearLock
                    sqlText = "";

                    sqlText += "select distinct isnull(PeriodLock,'Y') MLock,isnull(GLLock,'Y')YLock from fiscalyear " +
                                                   " where '" + transactionYearCheck + "' between PeriodStart and PeriodEnd";

                    DataTable dataTable = new DataTable("ProductDataT");
                    SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                    cmdIdExist.Transaction = transaction;
                    SqlDataAdapter reportDataAdapt = new SqlDataAdapter(cmdIdExist);
                    reportDataAdapt.Fill(dataTable);

                    if (dataTable == null)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                    }

                    else if (dataTable.Rows.Count <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                    }
                    else
                    {
                        if (dataTable.Rows[0]["MLock"].ToString() != "N")
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                        }
                        else if (dataTable.Rows[0]["YLock"].ToString() != "N")
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                        }
                    }
                    #endregion YearLock

                    #region YearNotExist
                    sqlText = "";
                    sqlText = sqlText + "select  min(PeriodStart) MinDate, max(PeriodEnd)  MaxDate from fiscalyear";

                    DataTable dtYearNotExist = new DataTable("ProductDataT");

                    SqlCommand cmdYearNotExist = new SqlCommand(sqlText, currConn);
                    cmdYearNotExist.Transaction = transaction;
                    //countId = (int)cmdIdExist.ExecuteScalar();

                    SqlDataAdapter YearNotExistDataAdapt = new SqlDataAdapter(cmdYearNotExist);
                    YearNotExistDataAdapt.Fill(dtYearNotExist);

                    if (dtYearNotExist == null)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearNotExist);
                    }

                    else if (dtYearNotExist.Rows.Count < 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearNotExist);
                    }
                    else
                    {
                        if (Convert.ToDateTime(transactionYearCheck) < Convert.ToDateTime(dtYearNotExist.Rows[0]["MinDate"].ToString())
                            || Convert.ToDateTime(transactionYearCheck) > Convert.ToDateTime(dtYearNotExist.Rows[0]["MaxDate"].ToString()))
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearNotExist);
                        }
                    }
                    #endregion YearNotExist

                }

                var latestId = "";
                var fiscalYear = "";
                #endregion Fiscal Year CHECK

                #region Find Transaction Exist

                sqlText = "";
                sqlText = sqlText + "select COUNT(IssueNo) from IssueHeaders WHERE IssueNo=@MasterIssueNo";
                SqlCommand cmdExistTran = new SqlCommand(sqlText, currConn);
                cmdExistTran.Transaction = transaction;
                cmdExistTran.Parameters.AddWithValueAndNullHandle("@MasterIssueNo", Master.IssueNo);
                IDExist = (int)cmdExistTran.ExecuteScalar();

                if (IDExist > 0)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, MessageVM.issueMsgFindExistID);
                }

                #endregion Find Transaction Exist

                #region Purchase ID Create
                if (string.IsNullOrEmpty(Master.transactionType)) //start
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, MessageVM.msgTransactionNotDeclared);
                }
                #region Purchase ID Create For Other

                CommonDAL commonDal = new CommonDAL();

                if (Master.transactionType == "Other")
                {
                    newID = commonDal.TransactionCode("Issue", "Other", "IssueHeaders", "IssueNo",
                                              "IssueDateTime", Master.IssueDateTime, Master.BranchId.ToString(), currConn, transaction, connVM);

                    var resultCode = commonDal.GetCurrentCode("Issue", "Other", Master.IssueDateTime, Master.BranchId.ToString(), currConn,
                        transaction, connVM);

                    var newIdara = resultCode.Split('~');

                    latestId = newIdara[0];
                    fiscalYear = newIdara[1];

                }
                if (Master.transactionType == "TollitemIssueWithoutBOM")
                {
                    newID = commonDal.TransactionCode("TollIssueWithoutBOM", "TollIssueWithoutBOM", "IssueHeaders", "IssueNo",
                                              "IssueDateTime", Master.IssueDateTime, Master.BranchId.ToString(), currConn, transaction, connVM);

                    var resultCode = commonDal.GetCurrentCode("TollIssueWithoutBOM", "TollIssueWithoutBOM", Master.IssueDateTime, Master.BranchId.ToString(), currConn,
                        transaction, connVM);

                    var newIdara = resultCode.Split('~');

                    latestId = newIdara[0];
                    fiscalYear = newIdara[1];

                }
                if (Master.transactionType == "IssueReturn")
                {
                    newID = commonDal.TransactionCode("Issue", "IssueReturn", "IssueHeaders", "IssueNo",
                                              "IssueDateTime", Master.IssueDateTime, Master.BranchId.ToString(), currConn, transaction, connVM);

                    var resultCode = commonDal.GetCurrentCode("Issue", "IssueReturn", Master.IssueDateTime, Master.BranchId.ToString(), currConn,
                        transaction, connVM);

                    var newIdara = resultCode.Split('~');

                    latestId = newIdara[0];
                    fiscalYear = newIdara[1];

                }

                if (Master.transactionType == "IssueWastage")
                {
                    newID = commonDal.TransactionCode("Issue", "IssueWastage", "IssueHeaders", "IssueNo",
                                              "IssueDateTime", Master.IssueDateTime, Master.BranchId.ToString(), currConn, transaction, connVM);

                    var resultCode = commonDal.GetCurrentCode("Issue", "IssueWastage", Master.IssueDateTime, Master.BranchId.ToString(), currConn,
                        transaction, connVM);

                    var newIdara = resultCode.Split('~');

                    latestId = newIdara[0];
                    fiscalYear = newIdara[1];

                }
                if (Master.transactionType == "ContractorIssueWoBOM")
                {
                    newID = commonDal.TransactionCode("Issue", "ContractorIssueWoBOM", "IssueHeaders", "IssueNo",
                                              "IssueDateTime", Master.IssueDateTime, Master.BranchId.ToString(), currConn, transaction, connVM);

                    var resultCode = commonDal.GetCurrentCode("Issue", "ContractorIssueWoBOM", Master.IssueDateTime, Master.BranchId.ToString(), currConn,
                        transaction, connVM);

                    var newIdara = resultCode.Split('~');

                    latestId = newIdara[0];
                    fiscalYear = newIdara[1];

                }


                #endregion Purchase ID Create For Other


                #region checkId and FiscalYear

                sqlText = @"select count(IssueNo) from IssueHeaders 
where IssueNumber = @IssueNumber and FiscalYear = @FiscalYear and TransactionType = @TransactionType and BranchId = @BranchId";

                var sqlCmd = new SqlCommand(sqlText, currConn, transaction);

                // latestId = (Convert.ToInt32(latestId) - 1).ToString();

                sqlCmd.Parameters.AddWithValue("@IssueNumber", latestId);
                sqlCmd.Parameters.AddWithValue("@FiscalYear", fiscalYear);
                sqlCmd.Parameters.AddWithValue("@TransactionType", Master.transactionType);
                sqlCmd.Parameters.AddWithValue("@BranchId", Master.BranchId);

                var count1 = (int)sqlCmd.ExecuteScalar();

                if (count1 > 0)
                {
                    FileLogger.Log("IssueDAL", "Insert", "IssueNumber " + newID + " Already Exists");
                    throw new Exception("Issue Id " + newID + " Already Exists");
                }

                #endregion


                #region Check Existing Id

                var count = 0;
                try
                {
                    sqlText = "select COUNT(IssueNo) from IssueHeaders WHERE IssueNo=@MasterIssueNo";

                    var cmd = new SqlCommand(sqlText, currConn, transaction);

                    cmd.Parameters.AddWithValue("@MasterIssueNo", newID);

                    count = (int)cmd.ExecuteScalar();
                }
                catch (Exception e)
                {
                }

                if (count > 0)
                {
                    FileLogger.Log("IssueDAL", "Insert", "Issue Id " + newID + " Already Exists");
                    throw new Exception("Issue Id " + newID + " Already Exists");
                }

                #endregion

                #endregion Purchase ID Create Not Complete

                #region ID generated completed,Insert new Information in Header

                IssueMasterVM imVM = new IssueMasterVM();
                imVM.IssueNo = newID;
                imVM.IssueDateTime = Master.IssueDateTime;
                imVM.TotalVATAmount = Master.TotalVATAmount;
                imVM.TotalAmount = Master.TotalAmount;
                imVM.SerialNo = Master.SerialNo;
                imVM.Comments = Master.Comments;
                imVM.CreatedBy = Master.CreatedBy;
                imVM.CreatedOn = Master.CreatedOn;
                imVM.LastModifiedBy = Master.LastModifiedBy;
                imVM.LastModifiedOn = Master.LastModifiedOn;
                imVM.ReceiveNo = Master.ReceiveNo;
                imVM.transactionType = Master.transactionType;
                imVM.ReturnId = Master.ReturnId;
                imVM.ImportId = Master.ImportId;
                imVM.Post = Master.Post;
                imVM.BranchId = Master.BranchId;
                imVM.AppVersion = Master.AppVersion;

                imVM.IssueNumber = Convert.ToInt32(latestId);
                imVM.FiscalYear = fiscalYear;

                imVM.BranchId = Master.BranchId;

                if (Master.transactionType == "TollitemIssueWithoutBOM")
                {
                    imVM.TotalAmount = 0;

                }

                retResults = IssueInsertToMaster(imVM, currConn, transaction, connVM);
                Id = retResults[4];
                if (retResults[0] != "Success")
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, retResults[1]);
                }




                #endregion ID generated completed,Insert new Information in Header

                #region Insert into Details(Insert complete in Header)
                #region Validation for Detail

                if (Details.Count() < 0)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, MessageVM.issueMsgNoDataToSave);
                }


                #endregion Validation for Detail

                #region Insert Detail Table

                foreach (var Item in Details.ToList())
                {
                    #region Find Transaction Exist


                    //sqlText = "";
                    //sqlText += "select COUNT(IssueNo) from IssueDetails WHERE IssueNo='" + newID + "' ";
                    //sqlText += " AND ItemNo=@ItemItemNo";
                    //SqlCommand cmdFindId = new SqlCommand(sqlText, currConn);
                    //cmdFindId.Transaction = transaction;
                    //cmdFindId.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo);
                    //IDExist = (int)cmdFindId.ExecuteScalar();

                    //if (IDExist > 0)
                    //{
                    //    throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, MessageVM.issueMsgFindExistID);
                    //}

                    #endregion Find Transaction Exist

                    #region Insert only DetailTable
                    IssueDetailVM iDetVM = new IssueDetailVM();


                    iDetVM.BOMId = Item.BOMId;
                    iDetVM.IssueNo = newID;
                    iDetVM.IssueLineNo = Item.IssueLineNo;
                    iDetVM.ItemNo = Item.ItemNo;
                    iDetVM.Quantity = Item.Quantity;
                    iDetVM.NBRPrice = 0;
                    iDetVM.CostPrice = Item.CostPrice;
                    iDetVM.UOM = Item.UOM;
                    iDetVM.VATRate = Item.VATRate;
                    iDetVM.VATAmount = Item.VATAmount;
                    iDetVM.SubTotal = Item.SubTotal;
                    iDetVM.CommentsD = Item.CommentsD;
                    iDetVM.CreatedBy = Master.CreatedBy;
                    iDetVM.CreatedOn = Master.CreatedOn;
                    iDetVM.LastModifiedBy = Master.LastModifiedBy;
                    iDetVM.LastModifiedOn = Master.LastModifiedOn;
                    iDetVM.ReceiveNo = Item.ReceiveNoD;
                    iDetVM.IssueDateTime = Master.IssueDateTime;
                    iDetVM.SD = 0;
                    iDetVM.SDAmount = 0;
                    iDetVM.Wastage = Item.Wastage;
                    iDetVM.BOMDate = Item.BOMDate;
                    iDetVM.FinishItemNo = Item.FinishItemNo;
                    iDetVM.transactionType = Master.transactionType;
                    iDetVM.IssueReturnId = Master.ReturnId;
                    iDetVM.UOMQty = Item.UOMQty;
                    iDetVM.UOMPrice = Item.UOMPrice;
                    iDetVM.UOMc = Item.UOMc;
                    iDetVM.UOMn = Item.UOMn;
                    iDetVM.Post = Master.Post;
                    iDetVM.BranchId = Master.BranchId;

                    if (Master.transactionType == "TollitemIssueWithoutBOM")
                    {
                        iDetVM.CostPrice = 0;
                        iDetVM.UOMPrice = 0;
                        iDetVM.SubTotal = 0;

                    }

                    retResults = IssueInsertToDetails(iDetVM, currConn, transaction, connVM);
                    if (retResults[0] != "Success")
                    {
                        throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, retResults[1]);
                    }

                    #region Comments
                    //sqlText = "";
                    //sqlText += " insert into IssueDetails(";
                    //sqlText += " IssueNo                                ,";
                    //sqlText += " IssueLineNo                                ,";
                    //sqlText += " ItemNo                                ,";
                    //sqlText += " Quantity                                ,";
                    //sqlText += " NBRPrice                                ,";
                    //sqlText += " CostPrice                                ,";
                    //sqlText += " UOM                                ,";
                    //sqlText += " VATRate                                ,";
                    //sqlText += " VATAmount                                ,";
                    //sqlText += " SubTotal                                ,";
                    //sqlText += " Comments                                ,";
                    //sqlText += " CreatedBy                                ,";
                    //sqlText += " CreatedOn                                ,";
                    //sqlText += " LastModifiedBy                                ,";
                    //sqlText += " LastModifiedOn                                ,";
                    //sqlText += " ReceiveNo                                ,";
                    //sqlText += " IssueDateTime                                ,";
                    //sqlText += " SD                                ,";
                    //sqlText += " SDAmount                                ,";
                    //sqlText += " Wastage                                ,";
                    //sqlText += " BOMDate                                ,";
                    //sqlText += " FinishItemNo                                ,";
                    //sqlText += " transactionType                                ,";
                    //sqlText += " IssueReturnId                                ,";
                    //sqlText += " UOMQty                                ,";
                    //sqlText += " UOMPrice                                ,";
                    //sqlText += " UOMc                                ,";
                    //sqlText += " UOMn                                ,";
                    //sqlText += " Post                   ";
                    //sqlText += " )";

                    //sqlText += " values(	";
                    ////sqlText += "'" + Master.Id + "',";

                    //sqlText += "'" + newID                                    + "',";
                    //sqlText += "'" + Item.IssueLineNo                                    + "',";
                    //sqlText += "'" + Item.ItemNo                                    + "',";
                    //sqlText += "'" + Item.Quantity                                    + "',";
                    //sqlText += " 0,";
                    //sqlText += "'" + Item.CostPrice                                    + "',";
                    //sqlText += "'" + Item.UOM                                    + "',";
                    //sqlText += "'" + Item.VATRate                                    + "',";
                    //sqlText += "'" + Item.VATAmount                                    + "',";
                    //sqlText += "'" + Item.SubTotal                                    + "',";
                    //sqlText += "'" + Item.CommentsD                                    + "',";
                    //sqlText += "'" + Master.CreatedBy                                    + "',";
                    //sqlText += "'" + Master.CreatedOn                                    + "',";
                    //sqlText += "'" + Master.LastModifiedBy                                    + "',";
                    //sqlText += "'" + Master.LastModifiedOn                                    + "',";
                    //sqlText += "'" + Item.ReceiveNoD                                    + "',";
                    //sqlText += "'" + Item.IssueDateTimeD                                    + "',";
                    //sqlText += "0,";
                    //sqlText += "0,";
                    //sqlText += "'" + Item.Wastage                                    + "',";
                    //sqlText += "'" + Item.BOMDate                                    + "',";
                    //sqlText += "'" + Item.FinishItemNo                                    + "',";
                    //sqlText += "'" + Master.transactionType                                    + "',";
                    //sqlText += "'" + Master.ReturnId                                    + "',";
                    //sqlText += "" +  Item.UOMQty                                    + ",";
                    //sqlText += "" +  Item.UOMPrice                                    + ",";
                    //sqlText += "" +  Item.UOMc                                    + ",";
                    //sqlText += "'" + Item.UOMn                                    + "',";
                    //sqlText += "'" + Master.Post                                    + "'";
                    //sqlText += ")	";


                    //SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                    //cmdInsDetail.Transaction = transaction;
                    //transResult = (int)cmdInsDetail.ExecuteNonQuery();

                    //if (transResult <= 0)
                    //{
                    //    throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, MessageVM.issueMsgSaveNotSuccessfully);
                    //}
                    #endregion Comments
                    #endregion Insert only DetailTable


                }


                #endregion Insert Detail Table
                #endregion Insert into Details(Insert complete in Header)

                #region return Current ID and Post Status

                sqlText = "";
                sqlText = sqlText + "select distinct  Post from dbo.IssueHeaders WHERE IssueNo='" + newID + "'";
                SqlCommand cmdIPS = new SqlCommand(sqlText, currConn);
                cmdIPS.Transaction = transaction;
                PostStatus = (string)cmdIPS.ExecuteScalar();
                if (string.IsNullOrEmpty(PostStatus))
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, MessageVM.issueMsgUnableCreatID);
                }


                #endregion Prefetch

                #region Update PeriodId

                sqlText = "";
                sqlText += @"

UPDATE IssueHeaders 
SET PeriodId=RIGHT('0'+CONVERT(VARCHAR(2),MONTH(IssueDateTime)) +  CONVERT(VARCHAR(4),YEAR(IssueDateTime)),6)
WHERE IssueNo = @IssueNo


update  IssueDetails                             
set PeriodId=RIGHT('0'+CONVERT(VARCHAR(2),MONTH(IssueDateTime)) +  CONVERT(VARCHAR(4),YEAR(IssueDateTime)),6)
WHERE IssueNo = @IssueNo
";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                cmdUpdate.Parameters.AddWithValue("@IssueNo", imVM.IssueNo);

                transResult = cmdUpdate.ExecuteNonQuery();

                #endregion

                #region Master Setting Update

                commonDal.UpdateProcessFlag(newID, Master.IssueDateTime, currConn, transaction, connVM);

                #endregion

                #region Commit

                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        //if (transResult > 0)
                        //{
                        transaction.Commit();
                        //}
                    }
                }

                #endregion Commit

                #region SuccessResult
                retResults = new string[5];
                retResults[0] = "Success";
                retResults[1] = MessageVM.issueMsgSaveSuccessfully;
                retResults[2] = "" + newID;
                retResults[3] = "" + PostStatus;
                retResults[4] = Id;
                #endregion SuccessResult

            }
            #endregion Try

            #region Catch and Finall
            //catch (SqlException sqlex)
            //{
            //    if (vcurrConn == null)
            //    {
            //        transaction.Rollback();
            //    }
            //    throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            //    //throw sqlex;
            //}
            catch (Exception ex)
            {

                retResults = new string[5];
                retResults[0] = "Fail";
                retResults[1] = ex.Message;
                retResults[2] = "0";
                retResults[3] = "N";
                //retResults[4] = "0";
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Rollback();
                }

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;

                FileLogger.Log("IssueDAL", "IssueInsert", ex.ToString() + "\n" + sqlText);
                throw new Exception(ex.ToString() + "\n" + Master.ImportId);

            }
            finally
            {
                if (VcurrConn == null)
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
            #endregion Catch and Finall

            #region Result
            return retResults;
            #endregion Result

        }

        public string[] IssueUpdate(IssueMasterVM Master, List<IssueDetailVM> Details, SysDBInfoVMTemp connVM = null, string UserId = "", SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Initializ
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            string sqlText = "";
            //DateTime MinDate = DateTime.MinValue;
            //DateTime MaxDate = DateTime.MaxValue;
            string PostStatus = "";

            #endregion Initializ

            #region Try
            try
            {
                #region Validation for Header

                if (Master == null)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, MessageVM.issueMsgNoDataToUpdate);
                }
                else if (Convert.ToDateTime(Master.IssueDateTime) < DateTime.MinValue || Convert.ToDateTime(Master.IssueDateTime) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, "Please Check Invoice Data and Time");

                }



                #endregion Validation for Header

                #region open connection and transaction
                #region open new connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction(MessageVM.issueMsgMethodNameUpdate);
                }
                #endregion open connection and transaction

                ////currConn = _dbsqlConnection.GetConnection(connVM);
                ////if (currConn.State != ConnectionState.Open)
                ////{
                ////    currConn.Open();
                ////}
                #region Add BOMId
                CommonDAL commonDal = new CommonDAL();
                //commonDal.TableFieldAdd("IssueDetails", "BOMId", "varchar(20)", currConn); //tablename,fieldName, datatype
                //commonDal.TableFieldAdd("IssueDetails", "UOMQty", "decimal(25, 9)", currConn); //tablename,fieldName, datatype
                //commonDal.TableFieldAdd("IssueDetails", "UOMPrice", "decimal(25, 9)", currConn); //tablename,fieldName, datatype
                //commonDal.TableFieldAdd("IssueDetails", "UOMc", "decimal(25, 9)", currConn); //tablename,fieldName, datatype
                //commonDal.TableFieldAdd("IssueDetails", "UOMn", "varchar(50)", currConn); //tablename,fieldName, datatype
                //commonDal.TableFieldAdd("IssueDetails", "UOMWastage", "decimal(25, 9)", currConn); //tablename,fieldName, datatype

                #endregion Add BOMId

                //transaction = currConn.BeginTransaction(MessageVM.issueMsgMethodNameUpdate);

                #endregion open connection and transaction

                #region Current Status

                #region Post Status

                string currentPostStatus = "N";

                sqlText = "";
                sqlText = @"
----------declare @InvoiceNo as varchar(100)='ISU-2/0001/1220'

select IssueNo, Post, BranchId from IssueHeaders
where 1=1 
and IssueNo=@InvoiceNo

";
                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
                cmd.Parameters.AddWithValue("@InvoiceNo", Master.IssueNo);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt != null && dt.Rows.Count > 0)
                {
                    currentPostStatus = dt.Rows[0]["Post"].ToString();
                }

                #endregion

                #region Current Items
                DataTable dtcurrentItems = new DataTable();

                if (currentPostStatus == "Y")
                {
                    sqlText = "";
                    sqlText = @"
----------declare @InvoiceNo as varchar(100)='ISU-2/0001/1220'


select ItemNo, IssueNo from IssueDetails
where 1=1 
and IssueNo=@InvoiceNo

";

                    cmd = new SqlCommand(sqlText, currConn, transaction);
                    cmd.Parameters.AddWithValue("@InvoiceNo", Master.IssueNo);

                    da = new SqlDataAdapter(cmd);
                    da.Fill(dtcurrentItems);

                }
                #endregion

                #endregion

                #region Find Month Lock

                string PeriodName = Convert.ToDateTime(Master.IssueDateTime).ToString("MMMM-yyyy");
                string[] vValues = { PeriodName };
                string[] vFields = { "PeriodName" };
                FiscalYearVM varFiscalYearVM = new FiscalYearVM();
                varFiscalYearVM = new FiscalYearDAL().SelectAll(0, vFields, vValues, currConn, transaction, connVM).FirstOrDefault();

                if (varFiscalYearVM.VATReturnPost == "Y")
                {
                    throw new Exception(PeriodName + ": VAT Return (9.1) already submitted for this month!");

                }
                else if (varFiscalYearVM == null)
                {
                    throw new Exception(PeriodName + ": This Fiscal Period is not Exist!");

                }

                #endregion Find Month Lock

                #region Fiscal Year Check

                string transactionDate = Master.IssueDateTime;
                string transactionYearCheck = Convert.ToDateTime(Master.IssueDateTime).ToString("yyyy-MM-dd");
                if (Convert.ToDateTime(transactionYearCheck) > DateTime.MinValue || Convert.ToDateTime(transactionYearCheck) < DateTime.MaxValue)
                {

                    #region YearLock
                    sqlText = "";

                    sqlText += "select distinct isnull(PeriodLock,'Y') MLock,isnull(GLLock,'Y')YLock from fiscalyear " +
                                                   " where '" + transactionYearCheck + "' between PeriodStart and PeriodEnd";

                    DataTable dataTable = new DataTable("ProductDataT");
                    SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                    cmdIdExist.Transaction = transaction;
                    SqlDataAdapter reportDataAdapt = new SqlDataAdapter(cmdIdExist);
                    reportDataAdapt.Fill(dataTable);

                    if (dataTable == null)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                    }

                    else if (dataTable.Rows.Count <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                    }
                    else
                    {
                        if (dataTable.Rows[0]["MLock"].ToString() != "N")
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                        }
                        else if (dataTable.Rows[0]["YLock"].ToString() != "N")
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                        }
                    }
                    #endregion YearLock
                    #region YearNotExist
                    sqlText = "";
                    sqlText = sqlText + "select  min(PeriodStart) MinDate, max(PeriodEnd)  MaxDate from fiscalyear";

                    DataTable dtYearNotExist = new DataTable("ProductDataT");

                    SqlCommand cmdYearNotExist = new SqlCommand(sqlText, currConn);
                    cmdYearNotExist.Transaction = transaction;
                    //countId = (int)cmdIdExist.ExecuteScalar();

                    SqlDataAdapter YearNotExistDataAdapt = new SqlDataAdapter(cmdYearNotExist);
                    YearNotExistDataAdapt.Fill(dtYearNotExist);

                    if (dtYearNotExist == null)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearNotExist);
                    }

                    else if (dtYearNotExist.Rows.Count < 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearNotExist);
                    }
                    else
                    {
                        if (Convert.ToDateTime(transactionYearCheck) < Convert.ToDateTime(dtYearNotExist.Rows[0]["MinDate"].ToString())
                            || Convert.ToDateTime(transactionYearCheck) > Convert.ToDateTime(dtYearNotExist.Rows[0]["MaxDate"].ToString()))
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearNotExist);
                        }
                    }
                    #endregion YearNotExist

                }


                #endregion Fiscal Year CHECK


                List<IssueDetailVM> previousDetailVMs = SelectIssueDetail(Master.IssueNo, null, null, currConn, transaction, connVM);


                #region Find ID for Update

                sqlText = "";
                sqlText = sqlText + "select COUNT(IssueNo) from IssueHeaders WHERE IssueNo=@MasterIssueNo ";
                SqlCommand cmdFindIdUpd = new SqlCommand(sqlText, currConn);
                cmdFindIdUpd.Transaction = transaction;
                cmdFindIdUpd.Parameters.AddWithValue("@MasterIssueNo", Master.IssueNo);
                int IDExist = (int)cmdFindIdUpd.ExecuteScalar();

                if (IDExist <= 0)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, MessageVM.issueMsgUnableFindExistID);
                }

                #endregion Find ID for Update

                #region update Header

                string[] cFields = new string[] { "IssueNo" };
                string[] cVals = new string[] { Master.IssueNo };

                IssueMasterVM imVM = SelectAllList(0, cFields, cVals, currConn, transaction, null, connVM).FirstOrDefault();
                string issuedatetime = Convert.ToDateTime(imVM.IssueDateTime) <= Convert.ToDateTime(Master.IssueDateTime)
                    ? imVM.IssueDateTime
                    : Master.IssueDateTime;

                #region Master Setting Update

                commonDal.UpdateProcessFlag(Master.IssueNo, issuedatetime, currConn, transaction, connVM);

                #endregion


                string settingValue = commonDal.settingValue("EntryProcess", "UpdateOnPost", connVM);

                if (settingValue != "Y")
                {
                    if (imVM.Post == "Y")
                    {
                        throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, MessageVM.ThisTransactionWasPosted);
                    }

                }

                imVM.IssueDateTime = Master.IssueDateTime;
                imVM.TotalVATAmount = Master.TotalVATAmount;
                imVM.TotalAmount = Master.TotalAmount;
                imVM.SerialNo = Master.SerialNo;
                imVM.Comments = Master.Comments;
                imVM.LastModifiedBy = Master.LastModifiedBy;
                imVM.LastModifiedOn = Master.LastModifiedOn;
                imVM.ReceiveNo = Master.ReceiveNo;
                imVM.transactionType = Master.transactionType;
                imVM.ReturnId = Master.ReturnId;
                imVM.Post = Master.Post;
                imVM.BranchId = Master.BranchId;
                sqlText = "";

                if (Master.transactionType == "TollitemIssueWithoutBOM")
                {
                    imVM.TotalAmount = 0;

                }

                retResults = IssueUpdateToMaster(imVM, currConn, transaction, connVM);
                if (retResults[0] != "Success")
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, retResults[1]);
                }

                #endregion update Header

                #region Update Details

                #region Validation for Detail

                if (Details.Count() < 0)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, MessageVM.issueMsgNoDataToUpdate);
                }


                #endregion Validation for Detail

                #region Update Detail Table


                #region Delete Existing Detail Data

                #region Purchase/Receive/Issue Data

                sqlText = "";
                sqlText += @" delete FROM PurchaseInvoiceDetails WHERE PurchaseInvoiceNo=@MasterIssueNo ";
                sqlText += @" delete FROM PurchaseInvoiceDuties WHERE PurchaseInvoiceNo=@MasterIssueNo ";

                //sqlText += @" delete FROM ReceiveDetails WHERE ReceiveNo=@MasterIssueNo ";

                sqlText += @" delete FROM IssueDetails WHERE IssueNo=@MasterIssueNo ";
                sqlText += @" delete FROM IssueDetailBOMs WHERE IssueNo=@MasterIssueNo ";


                SqlCommand cmdDeleteDetail = new SqlCommand(sqlText, currConn);
                cmdDeleteDetail.Transaction = transaction;
                cmdDeleteDetail.Parameters.AddWithValueAndNullHandle("@MasterIssueNo", Master.IssueNo);

                transResult = cmdDeleteDetail.ExecuteNonQuery();


                #endregion


                #endregion

                foreach (var Item in Details.ToList())
                {
                    #region Find Transaction Mode Insert or Update

                    #region Comments // 11-Sep-2019

                    ////sqlText = "";
                    ////sqlText += "select COUNT(IssueNo) from IssueDetails WHERE IssueNo=@MasterIssueNo  ";
                    ////sqlText += " AND ItemNo=@ItemItemNo ";
                    ////SqlCommand cmdFindId = new SqlCommand(sqlText, currConn);
                    ////cmdFindId.Transaction = transaction;
                    ////cmdFindId.Parameters.AddWithValue("@MasterIssueNo", Master.IssueNo);
                    ////cmdFindId.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo);
                    ////IDExist = (int)cmdFindId.ExecuteScalar();
                    #endregion



                    // Insert
                    #region Insert only DetailTable
                    IssueDetailVM iDetVM = new IssueDetailVM();

                    iDetVM.BOMId = Item.BOMId;
                    iDetVM.IssueNo = Master.IssueNo;
                    iDetVM.IssueLineNo = Item.IssueLineNo;
                    iDetVM.ItemNo = Item.ItemNo;
                    iDetVM.Quantity = Item.Quantity;
                    iDetVM.NBRPrice = 0;
                    iDetVM.CostPrice = Item.CostPrice;
                    iDetVM.UOM = Item.UOM;
                    iDetVM.VATRate = Item.VATRate;
                    iDetVM.VATAmount = Item.VATAmount;
                    iDetVM.SubTotal = Item.SubTotal;
                    iDetVM.CommentsD = Item.CommentsD;
                    iDetVM.CreatedBy = Master.CreatedBy;
                    iDetVM.CreatedOn = Master.CreatedOn;
                    iDetVM.LastModifiedBy = Master.LastModifiedBy;
                    iDetVM.LastModifiedOn = Master.LastModifiedOn;
                    iDetVM.ReceiveNo = Item.ReceiveNoD;
                    iDetVM.IssueDateTime = Master.IssueDateTime;
                    iDetVM.SD = 0;
                    iDetVM.SDAmount = 0;
                    iDetVM.Wastage = Item.Wastage;
                    iDetVM.BOMDate = Item.BOMDate;
                    iDetVM.FinishItemNo = Item.FinishItemNo;
                    iDetVM.transactionType = Master.transactionType;
                    iDetVM.IssueReturnId = Master.ReturnId;
                    iDetVM.UOMQty = Item.UOMQty;
                    iDetVM.UOMPrice = Item.UOMPrice;
                    iDetVM.UOMc = Item.UOMc;
                    iDetVM.UOMn = Item.UOMn;
                    iDetVM.Post = Master.Post;
                    iDetVM.BranchId = Master.BranchId;

                    if (Master.transactionType == "TollitemIssueWithoutBOM")
                    {
                        iDetVM.CostPrice = 0;
                        iDetVM.UOMPrice = 0;
                        iDetVM.SubTotal = 0;

                    }

                    retResults = IssueInsertToDetails(iDetVM, currConn, transaction, connVM);
                    if (retResults[0] != "Success")
                    {
                        throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, retResults[1]);
                    }

                    #endregion Insert only DetailTable

                    #endregion Find Transaction Mode Insert or Update
                }

                #endregion Update Detail Table

                #endregion

                #region return Current ID and Post Status

                sqlText = "";
                sqlText = sqlText + "select distinct Post from IssueHeaders WHERE IssueNo=@MasterIssueNo";
                SqlCommand cmdIPS = new SqlCommand(sqlText, currConn);
                cmdIPS.Transaction = transaction;
                cmdIPS.Parameters.AddWithValue("@MasterIssueNo", Master.IssueNo);
                PostStatus = (string)cmdIPS.ExecuteScalar();
                if (string.IsNullOrEmpty(PostStatus))
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, MessageVM.issueMsgUnableCreatID);
                }


                #endregion Prefetch

                #region Update PeriodId

                sqlText = "";
                sqlText += @"

UPDATE IssueHeaders 
SET PeriodId=RIGHT('0'+CONVERT(VARCHAR(2),MONTH(IssueDateTime)) +  CONVERT(VARCHAR(4),YEAR(IssueDateTime)),6)
WHERE IssueNo = @IssueNo


update  IssueDetails                             
set PeriodId=RIGHT('0'+CONVERT(VARCHAR(2),MONTH(IssueDateTime)) +  CONVERT(VARCHAR(4),YEAR(IssueDateTime)),6)
WHERE IssueNo = @IssueNo
";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                cmdUpdate.Parameters.AddWithValue("@IssueNo", imVM.IssueNo);

                transResult = cmdUpdate.ExecuteNonQuery();

                #endregion

                #region Update Product Stock

                if (currentPostStatus == "Y" && dtcurrentItems != null && dtcurrentItems.Rows.Count > 0)
                {
                    ProductDAL productDal = new ProductDAL();
                    List<string> transactionTypes = new List<string>() { "other" };

                    if (transactionTypes.Contains(Master.transactionType.ToLower()))
                    {
                        DataTable dtItemNo = previousDetailVMs.Select(x => new { x.ItemNo, Quantity = x.UOMQty }).ToList().ToDataTable();
                        productDal.Product_IN_OUT(new ParameterVM() { dt = dtItemNo, BranchId = Master.BranchId }, currConn, transaction, connVM);
                    }
                    else if (string.Equals(Master.transactionType, "issueReturn",
                                 StringComparison.CurrentCultureIgnoreCase))
                    {
                        DataTable dtItemNo = previousDetailVMs.Select(x => new { x.ItemNo, Quantity = x.UOMQty * -1 }).ToList().ToDataTable();

                        productDal.Product_IN_OUT(new ParameterVM() { dt = dtItemNo, BranchId = Master.BranchId }, currConn, transaction, connVM);
                    }
                    else
                    {
                        ResultVM rVM = new ResultVM();

                        ParameterVM paramVM = new ParameterVM();
                        paramVM.BranchId = Master.BranchId;
                        paramVM.InvoiceNo = Master.IssueNo;

                        paramVM.dt = dtcurrentItems;

                        paramVM.IDs = new List<string>();

                        foreach (DataRow dr in paramVM.dt.Rows)
                        {
                            paramVM.IDs.Add(dr["ItemNo"].ToString());
                        }

                        if (paramVM.IDs.Count > 0)
                        {
                            rVM = _ProductDAL.Product_Stock_Update(paramVM, currConn, transaction, connVM, UserId);
                        }
                    }
                }

                #endregion

                #region Master Setting Update

                commonDal.UpdateProcessFlag(Master.IssueNo, issuedatetime, currConn, transaction, connVM);

                #endregion


                #region Commit

                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                #endregion Commit

                ////if (transaction != null)
                ////{
                ////    //if (transResult > 0)
                ////    //{
                ////    transaction.Commit();
                ////    //}

                ////}

                #endregion Commit

                #region SuccessResult

                retResults[0] = "Success";
                retResults[1] = MessageVM.issueMsgUpdateSuccessfully;
                retResults[2] = Master.IssueNo;
                retResults[3] = PostStatus;
                #endregion SuccessResult

            }
            #endregion Try

            #region Catch and Finall
            //catch (SqlException sqlex)
            //{
            //    transaction.Rollback();
            //    throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            //    //throw sqlex;
            //}
            catch (Exception ex)
            {
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Rollback();
                }
                retResults = new string[5];
                retResults[0] = "Fail";
                retResults[1] = ex.Message;
                retResults[2] = "0";
                retResults[3] = "N";
                retResults[4] = "0";
                //////////transaction.Rollback();
                //throw ex;
                FileLogger.Log("IssueDAL", "IssueUpdate", ex.ToString() + "\n" + sqlText);

            }
            finally
            {
                if (VcurrConn == null)
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
            ////finally
            ////{
            ////    if (currConn != null)
            ////    {
            ////        if (currConn.State == ConnectionState.Open)
            ////        {
            ////            currConn.Close();
            ////        }
            ////    }

            ////}
            #endregion Catch and Finall

            #region Result
            return retResults;
            #endregion Result

        }

        public string[] IssuePost(IssueMasterVM Master, List<IssueDetailVM> Details, SqlTransaction Vtransaction = null, SqlConnection VcurrConn = null, SysDBInfoVMTemp connVM = null, string UserId = "")
        {
            #region Initializ
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";

            string PostStatus = "";


            #endregion Initializ

            #region Try
            try
            {
                #region Check Negative Stock Allow

                string vNegStockAllow = string.Empty;
                CommonDAL commonDal = new CommonDAL();
                vNegStockAllow = commonDal.settings("Issue", "NegStockAllow", currConn, transaction, connVM);
                if (string.IsNullOrEmpty(vNegStockAllow))
                {
                    throw new ArgumentNullException(MessageVM.msgSettingValueNotSave, "Issue");
                }
                bool NegStockAllow = Convert.ToBoolean(vNegStockAllow == "Y" ? true : false);

                #endregion

                #region Validation for Header

                if (Master == null)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNamePost, MessageVM.issueMsgNoDataToPost);
                }
                else if (Convert.ToDateTime(Master.IssueDateTime) < DateTime.MinValue || Convert.ToDateTime(Master.IssueDateTime) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNamePost, MessageVM.issueMsgCheckDatePost);

                }



                #endregion Validation for Header

                #region open connection and transaction

                #region New open connection and transaction

                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                #endregion New open connection and transaction

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }

                #endregion open connection and transaction

                #region Find Month Lock

                string PeriodName = Convert.ToDateTime(Master.IssueDateTime).ToString("MMMM-yyyy");
                string[] vValues = { PeriodName };
                string[] vFields = { "PeriodName" };
                FiscalYearVM varFiscalYearVM = new FiscalYearVM();
                varFiscalYearVM = new FiscalYearDAL().SelectAll(0, vFields, vValues, currConn, transaction, connVM).FirstOrDefault();

                if (varFiscalYearVM == null)
                {
                    throw new Exception(PeriodName + ": This Fiscal Period is not Exist!");

                }

                if (varFiscalYearVM.VATReturnPost == "Y")
                {
                    throw new Exception(PeriodName + ": VAT Return (9.1) already submitted for this month!");

                }

                #endregion Find Month Lock

                #region Fiscal Year Check

                string transactionDate = Master.IssueDateTime;
                string transactionYearCheck = Convert.ToDateTime(Master.IssueDateTime).ToString("yyyy-MM-dd");
                if (Convert.ToDateTime(transactionYearCheck) > DateTime.MinValue || Convert.ToDateTime(transactionYearCheck) < DateTime.MaxValue)
                {

                    #region YearLock
                    sqlText = "";

                    sqlText += "select distinct isnull(PeriodLock,'Y') MLock,isnull(GLLock,'Y')YLock from fiscalyear " +
                                                   " where '" + transactionYearCheck + "' between PeriodStart and PeriodEnd";

                    DataTable dataTable = new DataTable("ProductDataT");
                    SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                    cmdIdExist.Transaction = transaction;
                    SqlDataAdapter reportDataAdapt = new SqlDataAdapter(cmdIdExist);
                    reportDataAdapt.Fill(dataTable);

                    if (dataTable == null)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                    }

                    else if (dataTable.Rows.Count <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                    }
                    else
                    {
                        if (dataTable.Rows[0]["MLock"].ToString() != "N")
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                        }
                        else if (dataTable.Rows[0]["YLock"].ToString() != "N")
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                        }
                    }
                    #endregion YearLock
                    #region YearNotExist
                    sqlText = "";
                    sqlText = sqlText + "select  min(PeriodStart) MinDate, max(PeriodEnd)  MaxDate from fiscalyear";

                    DataTable dtYearNotExist = new DataTable("ProductDataT");

                    SqlCommand cmdYearNotExist = new SqlCommand(sqlText, currConn);
                    cmdYearNotExist.Transaction = transaction;
                    //countId = (int)cmdIdExist.ExecuteScalar();

                    SqlDataAdapter YearNotExistDataAdapt = new SqlDataAdapter(cmdYearNotExist);
                    YearNotExistDataAdapt.Fill(dtYearNotExist);

                    if (dtYearNotExist == null)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearNotExist);
                    }

                    else if (dtYearNotExist.Rows.Count < 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearNotExist);
                    }
                    else
                    {
                        if (Convert.ToDateTime(transactionYearCheck) < Convert.ToDateTime(dtYearNotExist.Rows[0]["MinDate"].ToString())
                            || Convert.ToDateTime(transactionYearCheck) > Convert.ToDateTime(dtYearNotExist.Rows[0]["MaxDate"].ToString()))
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearNotExist);
                        }
                    }
                    #endregion YearNotExist

                }


                #endregion Fiscal Year CHECK

                #region Find ID for Update

                sqlText = "";
                sqlText = sqlText + "select COUNT(IssueNo) from IssueHeaders WHERE IssueNo=@MasterIssueNo  ";
                SqlCommand cmdFindIdUpd = new SqlCommand(sqlText, currConn);
                cmdFindIdUpd.Transaction = transaction;
                cmdFindIdUpd.Parameters.AddWithValue("@MasterIssueNo", Master.IssueNo);

                int IDExist = (int)cmdFindIdUpd.ExecuteScalar();

                if (IDExist <= 0)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNamePost, MessageVM.issueMsgUnableFindExistIDPost);
                }

                #endregion Find ID for Update

                #region Stock Check

                if (NegStockAllow == false)
                {
                    decimal StockQuantity = 0;

                    DataTable dtAvgPrice = new DataTable();

                    List<IssueDetailVM> dVMs = new List<IssueDetailVM>();

                    dVMs = SelectIssueDetail(Master.IssueNo, null, null, currConn, transaction, connVM);

                    foreach (var Item in dVMs)
                    {
                        StockQuantity = 0;
                        ProductDAL productDal = new ProductDAL();

                        if (string.Equals(Master.transactionType, "ContractorIssueWoBOM", StringComparison.OrdinalIgnoreCase))
                        {
                            var dtTollStock = productDal.GetTollStock(new ParameterVM()
                            {
                                ItemNo = Item.ItemNo.Trim(),
                            });

                            StockQuantity = Convert.ToDecimal(dtTollStock.Rows[0][0]);
                        }
                        else
                        {
                            dtAvgPrice = productDal.AvgPriceNew(Item.ItemNo.Trim(), Convert.ToDateTime(Master.IssueDateTime).ToString("yyyy-MMM-dd") +
                                DateTime.Now.ToString(" HH:mm:00"), currConn, transaction, true, true, true, false, connVM, UserId);
                            StockQuantity = Convert.ToDecimal(dtAvgPrice.Rows[0]["Quantity"].ToString());
                        }




                        if (Item.UOMQty > StockQuantity)
                        {
                            throw new ArgumentNullException(MessageVM.saleMsgMethodNamePost, "(" + Item.ProductCode + ") " + Item.ItemName + Environment.NewLine + MessageVM.saleMsgStockNotAvailablePost);

                        }
                    }
                }
                #endregion

                #region ID check completed,update Information in Header

                PostVM pvm = new PostVM();
                pvm.LastModifiedBy = Master.LastModifiedBy;
                pvm.LastModifiedOn = Master.LastModifiedOn;
                pvm.Post = Master.Post;
                pvm.Code = Master.IssueNo;
                retResults = IssueAllPost(pvm, currConn, transaction, connVM);
                if (retResults[0] != "Success")
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNamePost, retResults[1]);
                }

                #endregion ID check completed,update Information in Header

                #region return Current ID and Post Status

                sqlText = "";
                sqlText = sqlText + "select distinct Post from IssueHeaders WHERE IssueNo=@MasterIssueNo ";
                SqlCommand cmdIPS = new SqlCommand(sqlText, currConn);
                cmdIPS.Transaction = transaction;
                cmdIPS.Parameters.AddWithValue("@MasterIssueNo", Master.IssueNo);
                PostStatus = (string)cmdIPS.ExecuteScalar();
                if (string.IsNullOrEmpty(PostStatus))
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNamePost, MessageVM.issueMsgPostNotSelect);
                }


                #endregion Prefetch

                #region Issue_Product_IN_OUT
                ResultVM rVM = new ResultVM();

                ParameterVM paramVM = new ParameterVM();
                paramVM.InvoiceNo = Master.IssueNo;

                rVM = Issue_Product_IN_OUT(paramVM, currConn, transaction, connVM);

                #endregion

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion

                #region SuccessResult

                retResults[0] = "Success";
                retResults[1] = MessageVM.issueMsgSuccessfullyPost;
                retResults[2] = Master.IssueNo;
                retResults[3] = PostStatus;
                #endregion SuccessResult

            }
            #endregion Try

            #region Catch and Finall

            catch (Exception ex)
            {
                retResults = new string[5];
                retResults[0] = "Fail";
                retResults[1] = ex.Message;
                retResults[2] = "0";
                retResults[3] = "N";
                retResults[4] = "0";
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Rollback();
                }
                ////throw new ArgumentNullException("", ex.Message.ToString());
                //////throw ex;

                FileLogger.Log("IssueDAL", "DemandPost", ex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", ex.Message.ToString());

            }
            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }

            }
            #endregion Catch and Finall

            #region Result
            return retResults;
            #endregion Result

        }

        public ResultVM Issue_Product_IN_OUT(ParameterVM paramVM, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Declarations

            ResultVM rVM = new ResultVM();
            string sqlText = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            #endregion

            #region Try Statement

            try
            {

                #region open connection and transaction

                #region New open connection and transaction

                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                #endregion New open connection and transaction

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }

                #endregion open connection and transaction

                #region Invoice Status

                sqlText = "";
                sqlText = @"
----------declare @InvoiceNo as varchar(100)='ISU-2/0001/1220'

select IssueNo, BranchId, Post from IssueHeaders
where 1=1 
and IssueNo=@InvoiceNo

";
                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
                cmd.Parameters.AddWithValue("@InvoiceNo", paramVM.InvoiceNo);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt != null && dt.Rows.Count > 0)
                {
                    paramVM.BranchId = Convert.ToInt32(dt.Rows[0]["BranchId"]);
                }

                #endregion

                #region Update Product Stock

                #region SQL Text

                sqlText = "";
                sqlText = @"
----------declare @InvoiceNo as varchar(100)='ISU-2/0001/1220'
declare @MultiplicationFactor as int=1


select 
@MultiplicationFactor = case 
when TransactionType in
(
'IssueReturn','ReceiveReturn'

) then  1 
when TransactionType in
(
'Other','Trading','TradingAuto','ExportTrading','TradingTender'
,'ExportTradingTender','InternalIssue','Service','ExportService','InputServiceImport'
,'InputService','Tender','WIP','PackageProduction'
,'TollIssue'
,'TollReceive-NotWIP'
,'TollReceive'

) then  -1 
end

from IssueDetails
where 1=1 
and IssueNo=@InvoiceNo


select ItemNo, IssueNo, TransactionType, UOMQty * (@MultiplicationFactor) as Quantity, Post from IssueDetails
where 1=1 
and IssueNo=@InvoiceNo

";
                #endregion

                cmd = new SqlCommand(sqlText, currConn, transaction);
                cmd.Parameters.AddWithValue("@InvoiceNo", paramVM.InvoiceNo);

                da = new SqlDataAdapter(cmd);
                dt = new DataTable();
                da.Fill(dt);

                paramVM.dt = dt;

                if (dt != null && dt.Rows.Count > 0)
                {
                    rVM = _ProductDAL.Product_IN_OUT(paramVM, currConn, transaction, connVM);
                }

                #endregion

                #region Success Result

                rVM.Status = "Success";
                rVM.Message = "Product Stock Updated Successfully!";

                #endregion

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion
            }

            #endregion

            #region Catch and Finally

            catch (Exception ex)
            {
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Rollback();
                }

                FileLogger.Log("IssueDAL", "Issue_Product_IN_OUT", ex.ToString() + "\n" + sqlText);

                throw ex;
            }

            finally
            {
                if (VcurrConn == null)
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

            ////finally
            ////{
            ////    if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
            ////    {
            ////        currConn.Close();
            ////    }
            ////}

            #endregion

            return rVM;
        }

        public decimal ReturnIssueQty(string issueReturnId, string itemNo, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            decimal retResults = 0;
            int countId = 0;
            string sqlText = "";
            SqlConnection currConn = null;
            #endregion

            #region Try

            try
            {
                #region open connection and transaction

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }

                #endregion open connection and transaction

                #region Return Qty

                sqlText = "  ";

                sqlText = "select Sum(isnull(IssueDetails.Quantity,0)) from IssueDetails ";
                sqlText += "where ItemNo =@itemNo and IssueReturnId =@issueReturnId ";
                sqlText += "group by ItemNo";

                SqlCommand cmd = new SqlCommand(sqlText, currConn);
                cmd.Parameters.AddWithValue("@itemNo", itemNo);
                cmd.Parameters.AddWithValue("@issueReturnId", issueReturnId);
                if (cmd.ExecuteScalar() == null)
                {
                    retResults = 0;
                }
                else
                {
                    retResults = (decimal)cmd.ExecuteScalar();
                }

                #endregion Return Qty

            }

            #endregion try

            #region Catch and Finall
            catch (SqlException sqlex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;

                FileLogger.Log("IssueDAL", "ReturnIssueQty", sqlex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;

                FileLogger.Log("IssueDAL", "ReturnIssueQty", ex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", ex.Message.ToString());

            }
            finally
            {
                if (currConn == null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();

                    }
                }
            }

            #endregion

            #region Results

            return retResults;

            #endregion

        }

        public ResultVM MultipleUpdateX(IssueMasterVM paramVM, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Declarations

            ResultVM rVM = new ResultVM();
            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            #endregion

            #region Try Statement

            try
            {
                #region open connection and transaction

                #region New open connection and transaction

                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                #endregion New open connection and transaction

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }

                #endregion open connection and transaction

                #region Update

                if (paramVM != null)
                {
                    List<IssueMasterVM> MasterVMs = new List<IssueMasterVM>();

                    #region Data Call

                    string[] cFields = { "IssueDateTime>=", "IssueDateTime<", "SelectTop" };
                    string[] cValues = { paramVM.IssueDateTimeFrom, paramVM.IssueDateTimeTo, "ALL" };
                    paramVM.IssueOnly = true;

                    MasterVMs = SelectAllList(0, cFields, cValues, currConn, transaction, null, connVM);

                    #endregion

                    if (MasterVMs != null && MasterVMs.Count > 0)
                    {

                        #region For Loop / Issue No

                        foreach (IssueMasterVM vm in MasterVMs)
                        {
                            List<IssueDetailVM> DetailVMs = new List<IssueDetailVM>();

                            DetailVMs = SelectIssueDetail(vm.IssueNo, null, null, currConn, transaction, connVM);

                            #region Declarations

                            decimal AvgRate = 0;
                            decimal CostPrice = 0;

                            decimal amount = 0;
                            decimal quantity = 0;

                            string rwUom = "";
                            string rwMajorUom = "";

                            decimal ConversionRate = 0;

                            #endregion

                            #region For Loop / Item No


                            foreach (IssueDetailVM dVM in DetailVMs)
                            {

                                #region Price Call

                                AvgRate = 0;
                                CostPrice = 0;
                                amount = 0;
                                quantity = 0;

                                DataTable priceData = new ProductDAL().AvgPriceNew(dVM.ItemNo, Convert.ToDateTime(dVM.IssueDateTime).ToString("yyyy-MMM-dd HH:mm:ss"), currConn, transaction, true, true, true, true, connVM);

                                if (priceData != null && priceData.Rows.Count > 0)
                                {
                                    amount = Convert.ToDecimal(priceData.Rows[0]["Amount"].ToString());
                                    quantity = Convert.ToDecimal(priceData.Rows[0]["Quantity"].ToString());

                                }

                                if (quantity > 0)
                                {
                                    AvgRate = (amount / quantity);
                                }

                                #endregion

                                #region UOM Conversion

                                rwUom = "";
                                rwMajorUom = "";

                                rwUom = dVM.UOM;
                                rwMajorUom = dVM.UOMn;

                                ConversionRate = 0;
                                UOMDAL uomdal = new UOMDAL();
                                ConversionRate = uomdal.GetConvertionRate(rwMajorUom, rwUom, "Y", VcurrConn, Vtransaction, connVM); //uomc

                                #endregion

                                CostPrice = AvgRate * ConversionRate;

                                #region Update

                                string sqlText = "";

                                sqlText = @"

------declare @IssueNo as varchar(50) = 'REC-1466/0320'
------declare @ItemNo as varchar(50) = '846'

------declare @CostPrice as decimal(18,9) 


------SELECT * 
UPDATE IssueDetails SET CostPrice = @CostPrice, SubTotal = Quantity * @CostPrice
FROM IssueDetails
WHERE 1=1
AND IssueNo = @IssueNo
AND ItemNo = @ItemNo
----ORDER BY IssueDateTime DESC
";

                                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                                cmdUpdate.Parameters.AddWithValue("@IssueNo", dVM.IssueNo);
                                cmdUpdate.Parameters.AddWithValue("@ItemNo", dVM.ItemNo);
                                cmdUpdate.Parameters.AddWithValue("@CostPrice", CostPrice);

                                cmdUpdate.ExecuteNonQuery();

                                #endregion

                            }

                            #endregion

                        }

                        #endregion

                    }

                }

                rVM.Status = "Success";
                rVM.Message = "Day End Process Completed Successfully!";

                #endregion

                #region Transaction Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion
            }

            #endregion

            #region Catch and Finally

            catch (Exception ex)
            {
                FileLogger.Log("IssueDAL", "MultipleUpdateX", ex.ToString());

                rVM = new ResultVM();
                rVM.Message = ex.Message;

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Rollback();
                }

                throw ex;
            }
            finally
            {
                if (VcurrConn == null)
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

            ////finally
            ////{
            ////    if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
            ////    {
            ////        currConn.Close();
            ////    }
            ////}

            #endregion

            return rVM;
        }

        public ResultVM MultipleUpdate(IssueMasterVM paramVM, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Declarations

            ResultVM rVM = new ResultVM();
            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            #endregion

            #region Try Statement

            try
            {
                #region open connection and transaction

                #region New open connection and transaction

                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                #endregion New open connection and transaction

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction(DateTime.Now.ToString("ddMMyyyHH:mm:ss"));
                }

                #endregion open connection and transaction

                #region Update

                string sqlText = "";
                if (paramVM != null)
                {
                    List<IssueMasterVM> MasterVMs = new List<IssueMasterVM>();

                    #region Previous Method Commented

                    #region Data Call

                    //string[] cFields = { "IssueDateTime>", "IssueDateTime<", "transactionType" };
                    //string[] cValues = { paramVM.IssueDateTimeFrom, paramVM.IssueDateTimeTo, "Other" };
                    //paramVM.IssueOnly = true;

                    //List<IssueDetailVM> DetailVMs = new List<IssueDetailVM>();

                    //DetailVMs = SelectIssueDetail(null, cFields, cValues, currConn, transaction);

                    //MasterVMs = SelectAllList(0, cFields, cValues, currConn, transaction);

                    #endregion


                    #region Declarations

                    decimal AvgRate = 0;
                    decimal CostPrice = 0;

                    decimal amount = 0;
                    decimal quantity = 0;

                    string rwUom = "";
                    string rwMajorUom = "";

                    decimal ConversionRate = 0;

                    #endregion

                    #region For Loop / Item No


                    //                    foreach (IssueDetailVM dVM in DetailVMs)
                    //                    {

                    //                        #region Price Call

                    //                        AvgRate = 0;
                    //                        CostPrice = 0;
                    //                        amount = 0;
                    //                        quantity = 0;

                    //                        DataTable priceData = new ProductDAL().AvgPriceNew(dVM.ItemNo, Convert.ToDateTime(dVM.IssueDateTime).ToString("yyyy-MMM-dd HH:mm:ss"), currConn, transaction, true, true, true, true);

                    //                        if (priceData != null && priceData.Rows.Count > 0)
                    //                        {
                    //                            amount = Convert.ToDecimal(priceData.Rows[0]["Amount"].ToString());
                    //                            quantity = Convert.ToDecimal(priceData.Rows[0]["Quantity"].ToString());

                    //                        }

                    //                        if (quantity > 0)
                    //                        {
                    //                            AvgRate = (amount / quantity);
                    //                        }

                    //                        #endregion

                    //                        #region UOM Conversion

                    //                        rwUom = "";
                    //                        rwMajorUom = "";

                    //                        rwUom = dVM.UOM;
                    //                        rwMajorUom = dVM.UOMn;

                    //                        ConversionRate = 0;
                    //                        UOMDAL uomdal = new UOMDAL();
                    //                        ConversionRate = uomdal.GetConvertionRate(rwMajorUom, rwUom, "Y", VcurrConn, Vtransaction); //uomc

                    //                        #endregion

                    //                        CostPrice = AvgRate * ConversionRate;

                    //                        #region Update

                    //                        string sqlText = "";

                    //                        sqlText = @"
                    //
                    //------declare @IssueNo as varchar(50) = 'REC-1466/0320'
                    //------declare @ItemNo as varchar(50) = '846'
                    //
                    //------declare @CostPrice as decimal(18,9) 
                    //
                    //
                    //------SELECT * 
                    //UPDATE IssueDetails SET CostPrice = @CostPrice, SubTotal = Quantity * @CostPrice
                    //FROM IssueDetails
                    //WHERE 1=1
                    //AND IssueNo = @IssueNo
                    //AND ItemNo = @ItemNo
                    //----ORDER BY IssueDateTime DESC
                    //";

                    //                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                    //                        cmdUpdate.Parameters.AddWithValue("@IssueNo", dVM.IssueNo);
                    //                        cmdUpdate.Parameters.AddWithValue("@ItemNo", dVM.ItemNo);
                    //                        cmdUpdate.Parameters.AddWithValue("@CostPrice", CostPrice);

                    //                        cmdUpdate.ExecuteNonQuery();

                    //                        #endregion

                    //                    }

                    #endregion

                    #endregion

                    #region Insert Initial Products & Purchase Data

                    sqlText = "select count(SL) from ProductAvgPrice";


                    SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
                    cmd.CommandTimeout = 500;
                    int rows = Convert.ToInt32(cmd.ExecuteScalar());

                    if (rows == 0)
                    {

                        #region Comments 27-Feb-2021

                        //                        sqlText = @"
                        //
                        //--select min(PeriodStart) from FiscalYear
                        //--where CurrentYear in (select CurrentYear from FiscalYear
                        //--where GETDATE() >= PeriodStart and GETDATE() <= PeriodEnd)
                        //
                        //select StartDateTime from CompanyProfiles
                        //
                        //";
                        //                        cmd.CommandText = sqlText;

                        #endregion


                        DateTime agvDateTime = new DateTime(1990, 01, 01, 00, 00, 00);


                        ProductDAL productDal = new ProductDAL();
                        CommonDAL commonDal = new CommonDAL();

                        List<ProductVM> allProdcuts = productDal.SelectAll("0", new[] { "Pc.IsRaw!=" }, new[] { "OverHead" }, VcurrConn, Vtransaction, null, connVM);


                        var ProductAvgPriceRows = allProdcuts.Select(x => new
                        {
                            x.ItemNo,
                            AgvPriceDate = agvDateTime,
                            PurchaseQty = 0,
                            PurchaseValue = 0,
                            RuntimeQty = 0,
                            RuntimeTotal = 0,
                            AvgPrice = 0
                        });


                        DataTable dtAvgPrices = JsonConvert.DeserializeObject<DataTable>(JsonConvert.SerializeObject(ProductAvgPriceRows));

                        string[] results = commonDal.BulkInsert("ProductAvgPrice", dtAvgPrices, VcurrConn, Vtransaction, 10000, null, connVM);



                        #region Update Initial Product Stocks

                        sqlText = @"
    declare @count int = (select count(BranchId) from BranchProfiles)

    IF @count = 1
    BEGIN
	    update ProductAvgPrice set RuntimeQty = Products.OpeningBalance, 
	    RuntimeTotal=Products.OpeningTotalCost
	    from Products 
	    where ProductAvgPrice.ItemNo = Products.ItemNo and PurchaseQty = 0 and PurchaseValue = 0
    END

    ELSE

    BEGIN


	    create table #tempStocks(id int identity(1,1), ItemNo varchar(50), TotalQty decimal(25,9), TotalValue decimal(25,9))

	    insert into #tempStocks (ItemNo, TotalQty,TotalValue)
	    SELECT distinct ItemNo, isnull(sum(p.StockQuantity),0) Quantity, isnull(sum(p.StockValue),0) 
	    from ProductStocks p
	    group by ItemNo

	    update ProductAvgPrice set RuntimeQty = #tempStocks.TotalQty, 
	    RuntimeTotal=#tempStocks.TotalValue
	    from #tempStocks 
	    where ProductAvgPrice.ItemNo = #tempStocks.ItemNo and PurchaseQty = 0 and PurchaseValue = 0

	    drop table #tempStocks

    END


    update ProductAvgPrice set AvgPrice = RuntimeTotal/RuntimeQty
    where PurchaseQty = 0 and PurchaseValue = 0
    and RuntimeQty >0";


                        cmd.CommandText = sqlText;
                        cmd.ExecuteNonQuery();

                        #endregion

                        #region Insert Item from Purchase

                        sqlText = @"
    
create table #tempAVGPrice(
ID int identity(1,1),
ItemNo varchar(50),
AgvPriceDate datetime,
PurchaseQty decimal(25,9),
PurchaseValue decimal(25,9),
AvgPrice decimal(25,9),
PurchaseNo varchar(50),
InsertTime datetime2(7),
)


-----Check if with SD---------------'
declare @withSD varchar(50) = (select SettingValue from Settings 
where SettingGroup= 'VAT6_1' and 
SettingName= 'TotalIncludeSD'
)

--------- 'Other','PurchaseCN','Trading','Service','ServiceNS','CommercialImporter','InputService' -----------------
insert into #tempAVGPrice(ItemNo,AgvPriceDate,PurchaseQty,PurchaseValue,AvgPrice,PurchaseNo,InsertTime)
select ItemNo, ReceiveDate, sum(isnull(UOMQty,0)),
case when @withSD='Y' then sum(SubTotal + SDAmount) else sum(SubTotal) end,
0 AvgPrice,PurchaseInvoiceNo,GetDate()
from PurchaseInvoiceDetails
where TransactionType  in ('Other','PurchaseCN','Trading','Service','ServiceNS','CommercialImporter','InputService')
and Post = 'Y'
group by ItemNo, ReceiveDate,PurchaseInvoiceNo



--------- 'Import','ServiceImport','ServiceNSImport','TradingImport','InputServiceImport' -----------------
insert into #tempAVGPrice(ItemNo,AgvPriceDate,PurchaseQty,PurchaseValue,AvgPrice,PurchaseNo,InsertTime)
select ItemNo, ReceiveDate, sum(isnull(UOMQty,0)),

case when @withSD='Y' then 
sum(isnull(AssessableValue,0)+ isnull(CDAmount,0)+ isnull(RDAmount,0)+ isnull(TVBAmount,0)+ isnull(TVAAmount,0)
+isnull(OthersAmount,0)
+isnull(SDAmount,0))
else 
sum(isnull(AssessableValue,0)+ isnull(CDAmount,0)+ isnull(RDAmount,0)+ isnull(TVBAmount,0)+ isnull(TVAAmount,0)
+isnull(OthersAmount,0))
end,

0 AvgPrice,PurchaseInvoiceNo,GetDate()
from PurchaseInvoiceDetails
where TransactionType  in ('Import','ServiceImport','ServiceNSImport','TradingImport','InputServiceImport')
and Post = 'Y'
group by ItemNo, ReceiveDate,PurchaseInvoiceNo


--------- 'PurchaseReturn','PurchaseDN' -----------------
insert into #tempAVGPrice(ItemNo,AgvPriceDate,PurchaseQty,PurchaseValue,AvgPrice,PurchaseNo,InsertTime)
select ItemNo, ReceiveDate, sum(-1*isnull(UOMQty,0)),

case when @withSD='Y' then 
sum(-1*(isnull(subtotal,0)+isnull(SDAmount,0)))
else 
sum(-1*(isnull(subtotal,0)))
end,

0 AvgPrice,PurchaseInvoiceNo,GetDate()
from PurchaseInvoiceDetails
where TransactionType  in ('PurchaseReturn','PurchaseDN')
and Post = 'Y'
group by ItemNo, ReceiveDate,PurchaseInvoiceNo

-------- insert into ProductAvgPrice -------------

insert into ProductAvgPrice(ItemNo,AgvPriceDate,PurchaseQty,PurchaseValue,AvgPrice,PurchaseNo,InsertTime)
select ItemNo,AgvPriceDate,PurchaseQty,PurchaseValue,AvgPrice,PurchaseNo,InsertTime from #tempAVGPrice
order by AgvPriceDate


drop table #tempAVGPrice
    ";

                        cmd.CommandText = sqlText;

                        rows = cmd.ExecuteNonQuery();

                        #endregion

                        #region Update Issue Day Flag

                        sqlText = @"
UPDATE IssueDetails set IsDayEnd = 'Y' 
where ItemNo in (Select distinct ItemNo from ProductAvgPrice)";


                        cmd.CommandText = sqlText;

                        cmd.ExecuteNonQuery();

                        #endregion

                        #region Update AVG Price

                        rows = UpdateAveragePrice(cmd);

                        #endregion


                        #region Issue Day End Process

                        IssueDayEndProcess(paramVM, currConn, transaction, connVM);

                        #endregion

                    }
                    #endregion

                    else
                    {

                        #region Update Item From Receive Date

                        sqlText = @"
create table #tempZeroPrice
(
 id int identity(1,1),
 AgvpriceDate datetime,
 itemNo varchar(50)

)

insert into #tempZeroPrice(AgvpriceDate,itemNo)
select AgvpriceDate,itemNo from ProductAvgPrice
where AvgPrice = 0



    declare @start int  = (select min(id) from #tempZeroPrice )
    declare @end int  = (select max(id) from #tempZeroPrice )

	declare @itemNo varchar(50) = ''
	declare @receiveDate datetime


	while @start <= @end
	begin
		
		select @itemNo = itemNo, @receiveDate = AgvpriceDate  from #tempZeroPrice
		where id = @start



		update ProductAvgPrice set AvgPrice = 0
		where ItemNo = @itemNo and AgvPriceDate >= @receiveDate

		update IssueDetails set IsDayEnd = 'Y'
		where ItemNo = @itemNo and IssueDateTime >= @receiveDate

		set @start = @start + 1;

	end



	drop table #tempZeroPrice";

                        cmd.CommandText = sqlText;
                        cmd.ExecuteNonQuery();

                        #endregion


                        #region Update AVG Price

                        rows = UpdateAveragePrice(cmd);

                        #endregion

                        #region Issue Day End Process

                        IssueDayEndProcess(paramVM, currConn, transaction, connVM);

                        #endregion

                    }

                }

                rVM.Status = "Success";
                rVM.Message = "Day End Process Completed Successfully!";

                #endregion

                #region Transaction Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion
            }

            #endregion

            #region Catch and Finally

            catch (Exception ex)
            {

                rVM = new ResultVM();
                rVM.Message = ex.Message;

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Rollback();
                }

                ////FileLogger.Log("IssueDAL", "Issue Multiple Save", ex.Message + ex.StackTrace);
                FileLogger.Log("IssueDAL", "MultipleUpdate", ex.ToString());

                throw ex;
            }

            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }

            #endregion

            return rVM;
        }


        public ResultVM UpdateAvgPrice_New_31072021(AVGPriceVm paramVM, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Declarations

            ResultVM rVM = new ResultVM();
            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            #endregion

            #region Try Statement

            try
            {
                #region open connection and transaction

                #region New open connection and transaction

                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                #endregion New open connection and transaction

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction(DateTime.Now.ToString("ddMMyyyHH:mm:ss"));
                }

                #endregion open connection and transaction

                #region Update

                bool IssueFrom6_1 = _cDal.settings("Toll6_4", "IssueFrom6_1", currConn, transaction, connVM) == "Y"; // toll Issue
                bool TollReceiveNotWIP = _cDal.settings("IssueFromBOM", "TollReceive-NotWIP", currConn, transaction, connVM) == "Y"; // TollReceive-NotWIP
                bool TollReceiveWithIssue = _cDal.settings("TollReceive", "WithIssue", currConn, transaction, connVM) == "Y"; // TollReceive

                string[] transactionTypes = new[]
                {
                    "Other", "Trading", "TradingAuto", "ExportTrading", "TradingTender", "ExportTradingTender",
                    "InternalIssue", "Service", "ExportService", "InputServiceImport", "InputService", "Tender", "WIP",
                    "PackageProduction", "TollReceive-NotWIP", "TollIssue", "TollFinishReceive",
                    "ReceiveReturn", "TollReceive"
                }; // , "IssueReturn"

                if (!IssueFrom6_1)
                {
                    transactionTypes = transactionTypes.Where(x => x != "TollIssue").ToArray();
                }
                if (!TollReceiveNotWIP)
                {
                    transactionTypes = transactionTypes.Where(x => x != "TollReceive-NotWIP").ToArray();
                }
                if (!TollReceiveWithIssue)
                {
                    transactionTypes = transactionTypes.Where(x => x != "TollReceive").ToArray();
                }

                string sqlText = "";
                if (paramVM != null)
                {
                    List<IssueMasterVM> MasterVMs = new List<IssueMasterVM>();

                    sqlText = "select count(SL) from ProductAvgPrice";


                    SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
                    cmd.CommandTimeout = 500;
                    int rows = Convert.ToInt32(cmd.ExecuteScalar());

                    if (rows == 0)
                    {
                        #region Initial AVG Price Query

                        sqlText = @"
--delete from ProductAvgPrice




create table #tempAVGPrice(
ID int identity(1,1),
ItemNo varchar(50),
AgvPriceDate datetime,
PurchaseQty   decimal(25,9),
PurchaseValue   decimal(25,9),
AvgPrice   decimal(25,9),
PurchaseNo varchar(50),
InsertTime datetime2(7),
TransactionType varchar(50),
SerialNo varchar(50)
)


insert into #tempAVGPrice(ItemNo, AgvPriceDate,PurchaseQty,PurchaseValue,AvgPrice,PurchaseNo,InsertTime,TransactionType,SerialNo)
select p.ItemNO, '1900-01-01 00:00:00', 0,0,0,'',GetDate(),'Opening','A'
from Products p left outer join ProductCategories pc on p.CategoryID = pc.CategoryID
where pc.IsRaw in( 'raw','pack','trading') 
--and ItemNo = '37'



------------------------------------------------------------OPENING-------------------------------------------------------


  declare @count int = (select count(BranchId) from BranchProfiles)

    IF @count = 1
    BEGIN
	    update #tempAVGPrice set PurchaseQty = Products.OpeningBalance, 
	    PurchaseValue=Products.OpeningTotalCost
	    from Products 
	    where #tempAVGPrice.ItemNo = Products.ItemNo and #tempAVGPrice.TransactionType='Opening'

		
    END

    ELSE

    BEGIN


	    create table #tempStocks(id int identity(1,1), ItemNo varchar(50), TotalQty   decimal(25,9), TotalValue   decimal(25,9))

	    insert into #tempStocks (ItemNo, TotalQty,TotalValue)
	    SELECT distinct ItemNo, isnull(sum(p.StockQuantity),0) Quantity, isnull(sum(p.StockValue),0) 
	    from ProductStocks p
	    group by ItemNo

	    update #tempAVGPrice set PurchaseQty = #tempStocks.TotalQty, 
	    PurchaseValue=#tempStocks.TotalValue
	    from #tempStocks 
	    where #tempAVGPrice.ItemNo = #tempStocks.ItemNo  and #tempAVGPrice.TransactionType='Opening'

	    drop table #tempStocks

    END


-----------------------------------------------------Opening-------------------------------------------------------------------




-----------------------------------------------------Purchase------------------------------------------------------------------
-----Check if with SD---------------'
declare @withSD varchar(50) = (select SettingValue from Settings 
where SettingGroup= 'VAT6_1' and 
SettingName= 'TotalIncludeSD'
)

declare @withOtherAMT varchar(50) = (select SettingValue from Settings 
where SettingGroup= 'VAT6_1' and 
SettingName= 'IncludeOtherAMT'
)

--------- 'Other','PurchaseCN','Trading','Service','ServiceNS','CommercialImporter','InputService' -----------------
insert into #tempAVGPrice(ItemNo,AgvPriceDate,PurchaseQty,PurchaseValue,AvgPrice,PurchaseNo,InsertTime,TransactionType,SerialNo)
select ItemNo, ReceiveDate, sum(isnull(UOMQty,0)),
case when @withSD='Y' then sum(SubTotal + SDAmount) else sum(SubTotal) end,
0 AvgPrice,PurchaseInvoiceNo,GetDate(),'Purchase', 'A1'
from PurchaseInvoiceDetails
where TransactionType  in ('Other','PurchaseCN','Trading','Service','ServiceNS','CommercialImporter','InputService')
and Post = 'Y'
--and ItemNo = '37'
group by ItemNo, ReceiveDate,PurchaseInvoiceNo


--------- RawCTC, WastageCTC -----------------
insert into #tempAVGPrice(ItemNo,AgvPriceDate,PurchaseQty,PurchaseValue,AvgPrice,PurchaseNo,InsertTime,TransactionType,SerialNo)
select ToItemNo, pd.TransferDate, sum(isnull(ToQuantity,0)),sum(isnull(ReceivePrice,0)),
0 AvgPrice,pt.TransferCode,GetDate(),'Purchase', 'B'
from ProductTransfersDetails pd left outer join ProductTransfers pt on pd.ProductTransferId = pt.id
where pd.TransactionType  in ('RawCTC')
and pd.Post = 'Y'
--and ItemNo = '37'
group by ToItemNo, pd.TransferDate,pt.TransferCode



--------- 'Import','ServiceImport','ServiceNSImport','TradingImport','InputServiceImport' -----------------
insert into #tempAVGPrice(ItemNo,AgvPriceDate,PurchaseQty,PurchaseValue,AvgPrice,PurchaseNo,InsertTime,TransactionType,SerialNo)
select ItemNo, ReceiveDate, sum(isnull(UOMQty,0)),

case when @withSD='Y' then 
sum(isnull(AssessableValue,0)+ isnull(CDAmount,0)+ isnull(RDAmount,0)+ isnull(TVBAmount,0)+ isnull(TVAAmount,0)
+(case when @withOtherAMT = 'Y' then isnull(OthersAmount,0) else 0 end)
+isnull(SDAmount,0))
else 
sum(isnull(AssessableValue,0)+ isnull(CDAmount,0)+ isnull(RDAmount,0)+ isnull(TVBAmount,0)+ isnull(TVAAmount,0)
+(case when @withOtherAMT = 'Y' then isnull(OthersAmount,0) else 0 end)
)
end,

0 AvgPrice,PurchaseInvoiceNo,GetDate(),'Purchase', 'A1'
from PurchaseInvoiceDetails
where TransactionType  in ('Import','ServiceImport','ServiceNSImport','TradingImport','InputServiceImport')
and Post = 'Y'
--and ItemNo = '37'
group by ItemNo, ReceiveDate,PurchaseInvoiceNo


--------- 'PurchaseReturn','PurchaseDN' -----------------
insert into #tempAVGPrice(ItemNo,AgvPriceDate,PurchaseQty,PurchaseValue,AvgPrice,PurchaseNo,InsertTime,TransactionType,SerialNo)
select ItemNo, ReceiveDate, sum(-1*isnull(UOMQty,0)),

case when @withSD='Y' then 
sum(-1*(isnull(subtotal,0)+isnull(SDAmount,0)))
else 
sum(-1*(isnull(subtotal,0)))
end,

0 AvgPrice,PurchaseInvoiceNo,GetDate(),'Purchase', 'A1'
from PurchaseInvoiceDetails
where TransactionType  in ('PurchaseReturn','PurchaseDN')
and Post = 'Y' 
--and ItemNo = '37'
group by ItemNo, ReceiveDate,PurchaseInvoiceNo

-----------------------------------------------------Purchase------------------------------------------------------------------

-----------------------------------------------------Ordering Data by Date------------------------------------------------------------------
insert into ProductAvgPrice(ItemNo,AgvPriceDate,PurchaseQty,PurchaseValue,AvgPrice,PurchaseNo,InsertTime,TransactionType,SerialNo)
select ItemNo,AgvPriceDate,PurchaseQty,PurchaseValue,AvgPrice,PurchaseNo,InsertTime,TransactionType,,SerialNo 
from #tempAVGPrice
order by AgvPriceDate

delete from #tempAVGPrice

insert into #tempAVGPrice(ItemNo,AgvPriceDate,PurchaseQty,PurchaseValue,AvgPrice,PurchaseNo,InsertTime,TransactionType,,SerialNo)
select ItemNo,AgvPriceDate,PurchaseQty,PurchaseValue,AvgPrice,PurchaseNo,InsertTime,TransactionType,,SerialNo 
from ProductAvgPrice
order by SerialNo,ItemNo,AgvPriceDate, SL desc

delete from ProductAvgPrice
-----------------------------------------------------Ordering Data by Date------------------------------------------------------------------

update #tempAVGPrice set AgvPriceDate = FORMAT(AgvPriceDate, 'yyyy-MM-dd 00:00:00')

--select * from #tempAVGPrice
--where ItemNo = '37'
--order by AgvPriceDate


	declare @start int  = (select min(ID) from #tempAvgPrice)
    declare @end int  = (select max(ID) from #tempAvgPrice)

	declare @purchaseQty   decimal(25,9)
	declare @purchaseValue   decimal(25,9)

	declare @purchaseNo varchar(50)
    declare @itemNo varchar(50) = ''
    declare @date datetime 
	declare @transactionType  varchar(50)
	declare @lastPurchasedate datetime
	declare @lastAvgPrice   decimal(25,9)
	declare @currentAvgPrice   decimal(25,9)
	declare @nextPurchaseDate datetime
	declare @lastPurchaseValue   decimal(25,9)
	declare @lastPurchaseQty   decimal(25,9)

	declare @issueQty   decimal(25,9)
	declare @issueValue   decimal(25,9)
	declare @runtimeValue decimal(25,9)
	declare @runtimeQty decimal(25,9)

    while @start <= @end
    begin
	   
	   select
	   @itemNo =ItemNo,
	   @date=AgvPriceDate,
	   @purchaseQty=PurchaseQty,
	   @purchaseValue=PurchaseValue,
	   @purchaseNo = PurchaseNo,
	   @transactionType = transactionType
	   --,@currentAvgPrice = AvgPrice
	   from #tempAvgPrice 
	   where ID = @start 

	   select top 1
	   @nextPurchaseDate = AgvPriceDate
	   from #tempAvgPrice 
	   where ID > @start and AgvPriceDate >= @date and ItemNo = @itemNo
	   order by AgvPriceDate asc, ID asc

	   select top 1 @lastPurchasedate=AgvPriceDate,@lastAvgPrice=AvgPrice,
	   @lastPurchaseValue = RunTimeTotal, @lastPurchaseQty = RunTimeQty
	   from ProductAvgPrice
	   where  AgvPriceDate <= @date and ItemNo = @itemNo and (TransactionType = 'Purchase' or TransactionType = 'Opening')
	   order by SL desc,AgvPriceDate desc

	   

	  if @transactionType = 'Purchase'
	  begin 
		
	-- @issueQty=sum(UOMQty), @issueValue=sum(Subtotal)

	select @issueQty=sum(UOMQty), @issueValue=sum(Subtotal) 

	from(

	    select sum(UOMQty)UOMQty, sum(Subtotal)Subtotal		  
		from IssueDetails 
		where ItemNo = @itemNo and IssueDateTime >= @lastPurchasedate and IssueDateTime < @date
		and TransactionType in ('" + string.Join("','", transactionTypes) + @"') --,'TollReceive'

		union all

        select sum(UOMQty*-1)UOMQty, sum(Subtotal*-1)Subtotal		  
		from IssueDetails 
		where ItemNo = @itemNo and IssueDateTime >= @lastPurchasedate and IssueDateTime < @date
		and TransactionType in ('IssueReturn') --,'TollReceive'

        union all

	    select sum(UOMQty),sum(UOMQty*AVGPrice) from SalesInvoiceDetails
	    where ItemNo = @itemNo and InvoiceDateTime >= @lastPurchasedate and InvoiceDateTime < @date
	    and TransactionType in ('RawSale')

		union all

	    select sum(FromQuantity),sum(IssuePrice) from ProductTransfersDetails
	    where FromItemNo = @itemNo and TransferDate >= @lastPurchasedate and TransferDate < @date
	    and TransactionType in ('RawCTC') --'WastageCTC',

		) Issues

	 if @issueQty is not null
	 begin
	  insert into ProductAvgPrice(ItemNo,AgvPriceDate, PurchaseQty, PurchaseValue, AvgPrice, InsertTime, TransactionType,FromDate)
	  select @ItemNo,DATEADD(d,-1,@date),@issueQty,@lastAvgPrice*@issueQty , @lastAvgPrice, GetDate(), 'Issue',@lastPurchasedate
	 end



	  insert into ProductAvgPrice(ItemNo,AgvPriceDate, PurchaseQty, PurchaseValue,RuntimeTotal,RuntimeQty, AvgPrice, InsertTime, TransactionType,PurchaseNo,FromDate)
	  select @itemNo, @date, @purchaseQty, @purchaseValue,((@purchaseValue+isnull(@lastPurchaseValue,0))-isnull(@issueValue,0)),((@purchaseQty+isnull(@lastPurchaseQty,0))-isnull(@issueQty,0)),0,	  
	  GETDATE(), 'Purchase',@purchaseNo,@date



	  end
	else
	begin

	  insert into ProductAvgPrice(ItemNo,AgvPriceDate, PurchaseQty, PurchaseValue,RuntimeTotal,RuntimeQty, AvgPrice, InsertTime, TransactionType,PurchaseNo)
	  select @itemNo, @date, @purchaseQty, @purchaseValue, @purchaseValue,@purchaseQty,0,GETDATE(), 'Opening',@purchaseNo

	end


	   update ProductAvgPrice set AvgPrice = (RuntimeTotal/RuntimeQty)
	   where RuntimeQty > 0

	   if(((@purchaseQty+isnull(@lastPurchaseQty,0))-isnull(@issueQty,0)) > 0)
	   begin
			set @currentAvgPrice = ((@purchaseValue+isnull(@lastPurchaseValue,0))-isnull(@issueValue,0)) / ((@purchaseQty+isnull(@lastPurchaseQty,0))-isnull(@issueQty,0))
	   end
	   else
	   begin
			set @currentAvgPrice = 0
	   end


	 --  if @itemNo = '37'
	 --  begin
		----select @itemNo,@date, @nextPurchaseDate,@transactionType, @currentAvgPrice
		--select @issueQty
	 --  end


	  -------------------------------------------- updating issue --------------------------------------
	   update IssueDetails set    UOMPrice = @currentAvgPrice
	   where IssueDateTime >= @date and IssueDateTime <= isnull(@nextPurchaseDate,GETDATE()) and ItemNo = @itemNo

       update IssueDetails set CostPrice=  convert(decimal(25,9), UOMPrice * UOMc), SubTotal= convert(decimal(25,9),(UOMPrice * UOMc)*Quantity )
	   where IssueDateTime >= @date and IssueDateTime <= isnull(@nextPurchaseDate,GETDATE()) and ItemNo = @itemNo



	   update SalesInvoiceDetails set AVGPrice = @currentAvgPrice 
	   where InvoiceDateTime >= @date 
	   and InvoiceDateTime <= isnull(@nextPurchaseDate,GETDATE())
	   and ItemNo = @itemNo 
       and TransactionType in ('RawSale')

       
	   update ProductTransfersDetails set IssuePrice = @currentAvgPrice * FromQuantity
	   where TransferDate >= @date 
	   and TransferDate <= isnull(@nextPurchaseDate,GETDATE())
	   and FromItemNo = @itemNo 
	   and TransactionType in  ('RawCTC')

	   update ProductTransfersDetails set IssuePrice = @currentAvgPrice * FromQuantity, ReceivePrice=@currentAvgPrice * FromQuantity
	   where TransferDate >= @date 
	   and TransferDate <= isnull(@nextPurchaseDate,GETDATE())
	   and FromItemNo = @itemNo 
	   and TransactionType in  ('WastageCTC')

	 -------------------------------------------- updating issue --------------------------------------
	  set @start = @start + 1

	 set @currentAvgPrice = 0
	 set @issueValue = 0
	 set @issueQty = 0
	 set @purchaseQty = 0
	 set @lastPurchaseValue = 0
	 set @lastPurchaseQty = 0
	 set @nextPurchaseDate = null
    end



select * from ProductAvgPrice
--select * from #tempAVGPrice


drop table #tempAVGPrice
";

                        #endregion

                        cmd.CommandText = sqlText;
                        cmd.CommandTimeout = 3600;
                        cmd.ExecuteNonQuery();

                    }
                    else
                    {

                        #region AVG Price Re-Process

                        string deletePurchaseIssues = @"

----declare @processDate datetime = '2021-02-28 00:00:00.000'


delete from ProductAvgPrice
where AgvPriceDate >= dateadd(d,-1,@processDate) and TransactionType='Issue'


delete from ProductAvgPrice
where AgvPriceDate >= @processDate



create table #tempAVGPrice(
ID int identity(1,1),
ItemNo varchar(50),
AgvPriceDate datetime,
PurchaseQty   decimal(25,9),
PurchaseValue   decimal(25,9),
AvgPrice   decimal(25,9),
PurchaseNo varchar(50),
InsertTime datetime2(7),
TransactionType varchar(50)
)

create table #tempAVGPriceOrder(
ID int identity(1,1),
ItemNo varchar(50),
AgvPriceDate datetime,
PurchaseQty   decimal(25,9),
PurchaseValue   decimal(25,9),
AvgPrice   decimal(25,9),
PurchaseNo varchar(50),
InsertTime datetime2(7),
TransactionType varchar(50)
)


-----Check if with SD---------------'
declare @withSD varchar(50) = (select SettingValue from Settings 
where SettingGroup= 'VAT6_1' and 
SettingName= 'TotalIncludeSD'
)

declare @withOtherAMT varchar(50) = (select SettingValue from Settings 
where SettingGroup= 'VAT6_1' and 
SettingName= 'IncludeOtherAMT'
)
--------- 'Other','PurchaseCN','Trading','Service','ServiceNS','CommercialImporter','InputService' -----------------
insert into #tempAVGPrice(ItemNo,AgvPriceDate,PurchaseQty,PurchaseValue,AvgPrice,PurchaseNo,InsertTime,TransactionType)
select ItemNo, ReceiveDate, sum(isnull(UOMQty,0)),
case when @withSD='Y' then sum(SubTotal + SDAmount) else sum(SubTotal) end,
0 AvgPrice,PurchaseInvoiceNo,GetDate(),'Purchase'
from PurchaseInvoiceDetails

where TransactionType  in ('Other','PurchaseCN','Trading','Service','ServiceNS','CommercialImporter','InputService')
and Post = 'Y'
and ReceiveDate >= @processDate
--and ItemNo = '37'
group by ItemNo, ReceiveDate,PurchaseInvoiceNo


--------- RawCTC, WastageCTC -----------------
insert into #tempAVGPrice(ItemNo,AgvPriceDate,PurchaseQty,PurchaseValue,AvgPrice,PurchaseNo,InsertTime,TransactionType)
select ToItemNo, pd.TransferDate, sum(isnull(ToQuantity,0)),sum(isnull(ReceivePrice,0)),
0 AvgPrice,pt.TransferCode,GetDate(),'Purchase'
from ProductTransfersDetails pd left outer join ProductTransfers pt on pd.ProductTransferId = pt.id
where pd.TransactionType  in ('WastageCTC','RawCTC')
and pd.Post = 'Y'
and pd.TransferDate >= @processDate
--and ItemNo = '37'
group by ToItemNo, pd.TransferDate,pt.TransferCode



--------- 'Import','ServiceImport','ServiceNSImport','TradingImport','InputServiceImport' -----------------
insert into #tempAVGPrice(ItemNo,AgvPriceDate,PurchaseQty,PurchaseValue,AvgPrice,PurchaseNo,InsertTime,TransactionType)
select ItemNo, ReceiveDate, sum(isnull(UOMQty,0)),

case when @withSD='Y' then 
sum(isnull(AssessableValue,0)+ isnull(CDAmount,0)+ isnull(RDAmount,0)+ isnull(TVBAmount,0)+ isnull(TVAAmount,0)
+(case when @withOtherAMT = 'Y' then isnull(OthersAmount,0) else 0 end)
+isnull(SDAmount,0))
else 
sum(isnull(AssessableValue,0)+ isnull(CDAmount,0)+ isnull(RDAmount,0)+ isnull(TVBAmount,0)+ isnull(TVAAmount,0)
+(case when @withOtherAMT = 'Y' then isnull(OthersAmount,0) else 0 end))
end,

0 AvgPrice,PurchaseInvoiceNo,GetDate(),'Purchase'
from PurchaseInvoiceDetails
where TransactionType  in ('Import','ServiceImport','ServiceNSImport','TradingImport','InputServiceImport')
and Post = 'Y'
and ReceiveDate >= @processDate
--and ItemNo = '37'
group by ItemNo, ReceiveDate,PurchaseInvoiceNo


--------- 'PurchaseReturn','PurchaseDN' -----------------
insert into #tempAVGPrice(ItemNo,AgvPriceDate,PurchaseQty,PurchaseValue,AvgPrice,PurchaseNo,InsertTime,TransactionType)
select ItemNo, ReceiveDate, sum(-1*isnull(UOMQty,0)),

case when @withSD='Y' then 
sum(-1*(isnull(subtotal,0)+isnull(SDAmount,0)))
else 
sum(-1*(isnull(subtotal,0)))
end,

0 AvgPrice,PurchaseInvoiceNo,GetDate(),'Purchase'
from PurchaseInvoiceDetails
where TransactionType  in ('PurchaseReturn','PurchaseDN')
and Post = 'Y'
and ReceiveDate >= @processDate
--and ItemNo = '37'
group by ItemNo, ReceiveDate,PurchaseInvoiceNo



insert into #tempAVGPriceOrder(ItemNo,AgvPriceDate,PurchaseQty,PurchaseValue,AvgPrice,PurchaseNo,InsertTime,TransactionType)
select ItemNo,AgvPriceDate,PurchaseQty,PurchaseValue,AvgPrice,PurchaseNo,InsertTime,TransactionType from #tempAVGPrice
order by AgvPriceDate

delete from #tempAVGPrice
insert into #tempAVGPrice(ItemNo,AgvPriceDate,PurchaseQty,PurchaseValue,AvgPrice,PurchaseNo,InsertTime,TransactionType)
select ItemNo,AgvPriceDate,PurchaseQty,PurchaseValue,AvgPrice,PurchaseNo,InsertTime,TransactionType from #tempAVGPriceOrder
order by AgvPriceDate

delete from #tempAVGPriceOrder

update #tempAVGPrice set AgvPriceDate = FORMAT(AgvPriceDate, 'yyyy-MM-dd 00:00:00')

select * from #tempAVGPrice
--where PurchaseNo = 'IMP-0013/0719'




	declare @start int  = (select min(ID) from #tempAvgPrice)
    declare @end int  = (select max(ID) from #tempAvgPrice)

	declare @purchaseQty   decimal(25,9)
	declare @purchaseValue   decimal(25,9)

	declare @purchaseNo varchar(50)
    declare @itemNo varchar(50) = ''
    declare @date datetime 
	declare @transactionType  varchar(50)
	declare @lastPurchasedate datetime
	declare @lastAvgPrice   decimal(25,9)
	declare @currentAvgPrice   decimal(25,9)
	declare @nextPurchaseDate datetime
	declare @lastPurchaseValue   decimal(25,9)
	declare @lastPurchaseQty   decimal(25,9)

	declare @issueQty   decimal(25,9)
	declare @issueValue   decimal(25,9)
	declare @runtimeValue decimal(25,9)
	declare @runtimeQty decimal(25,9)

    while @start <= @end
    begin
	   
	   select
	   @itemNo =ItemNo,
	   @date=AgvPriceDate,
	   @purchaseQty=PurchaseQty,
	   @purchaseValue=PurchaseValue,
	   @purchaseNo = PurchaseNo,
	   @transactionType = transactionType
	   --,@currentAvgPrice = AvgPrice
	   from #tempAvgPrice 
	   where ID = @start 

	   select top 1
	   @nextPurchaseDate = AgvPriceDate
	   from #tempAvgPrice 
	   where ID > @start and AgvPriceDate >= @date and ItemNo = @itemNo
	   order by AgvPriceDate asc, ID asc

	   select top 1 @lastPurchasedate=AgvPriceDate,@lastAvgPrice=AvgPrice,
	   @lastPurchaseValue = RunTimeTotal, @lastPurchaseQty = RunTimeQty
	   from ProductAvgPrice
	   where  AgvPriceDate <= @date and ItemNo = @itemNo and (TransactionType = 'Purchase' or TransactionType = 'Opening')
	   order by SL desc,AgvPriceDate desc

	   

	  if @transactionType = 'Purchase'
	  begin 
		
	select @issueQty=sum(UOMQty), @issueValue=sum(Subtotal) 

	from(
	   select sum(UOMQty)UOMQty, sum(Subtotal)Subtotal
		  
		from IssueDetails 
		where ItemNo = @itemNo and IssueDateTime >= @lastPurchasedate and IssueDateTime < @date
		and TransactionType in ('" + string.Join("','", transactionTypes) + @"') -- ,'TollIssue'

		union all

        select sum(UOMQty*-1)UOMQty, sum(Subtotal*-1)Subtotal		  
		from IssueDetails 
		where ItemNo = @itemNo and IssueDateTime >= @lastPurchasedate and IssueDateTime < @date
		and TransactionType in ('IssueReturn') --,'TollReceive'

		union all

	    select sum(UOMQty),sum(UOMQty*AVGPrice) from SalesInvoiceDetails
	    where ItemNo = @itemNo and InvoiceDateTime >= @lastPurchasedate and InvoiceDateTime < @date
	    and TransactionType in ('RawSale')

        union all

	    select sum(FromQuantity),sum(IssuePrice) from ProductTransfersDetails
	    where FromItemNo = @itemNo and TransferDate >= @lastPurchasedate and TransferDate < @date
	    and TransactionType in ('RawCTC')

		) Issues


	 if @issueQty is not null
	 begin
			  insert into ProductAvgPrice(ItemNo,AgvPriceDate, PurchaseQty, PurchaseValue, AvgPrice, InsertTime, TransactionType,FromDate)
	  select @ItemNo,DATEADD(d,-1,@date),@issueQty,@lastAvgPrice*@issueQty , @lastAvgPrice, GetDate(), 'Issue',@lastPurchasedate
	 end



	  insert into ProductAvgPrice(ItemNo,AgvPriceDate, PurchaseQty, PurchaseValue,RuntimeTotal,RuntimeQty, AvgPrice, InsertTime, TransactionType,PurchaseNo,FromDate)
	  select @itemNo, @date, @purchaseQty, @purchaseValue,((@purchaseValue+isnull(@lastPurchaseValue,0))-isnull(@issueValue,0)),((@purchaseQty+isnull(@lastPurchaseQty,0))-isnull(@issueQty,0)),0,	  
	  GETDATE(), 'Purchase',@purchaseNo,@date



	  end
	else
	begin



	  insert into ProductAvgPrice(ItemNo,AgvPriceDate, PurchaseQty, PurchaseValue,RuntimeTotal,RuntimeQty, AvgPrice, InsertTime, TransactionType,PurchaseNo)
	  select @itemNo, @date, @purchaseQty, @purchaseValue, @purchaseValue,@purchaseQty,0,GETDATE(), 'Opening',@purchaseNo

	end


	   update ProductAvgPrice set AvgPrice = (RuntimeTotal/RuntimeQty)
	   where RuntimeQty > 0

	   if(((@purchaseQty+isnull(@lastPurchaseQty,0))-isnull(@issueQty,0)) > 0)
	   begin
		--select @itemNo, @purchaseNo, @purchaseQty,@lastPurchaseQty
			set @currentAvgPrice = ((@purchaseValue+isnull(@lastPurchaseValue,0))-isnull(@issueValue,0)) / ((@purchaseQty+isnull(@lastPurchaseQty,0))-isnull(@issueQty,0))

	   end
	   else
	   begin
			set @currentAvgPrice = 0

	   end

	  -------------------------------------------- updating issue --------------------------------------
	   update IssueDetails set CostPrice= @currentAvgPrice * UOMc, SubTotal=(@currentAvgPrice * UOMc)*Quantity,
	   UOMPrice = @currentAvgPrice
	   where IssueDateTime >= @date and IssueDateTime <= isnull(@nextPurchaseDate,GETDATE()) and ItemNo = @itemNo

	   
	   update SalesInvoiceDetails set AVGPrice = @currentAvgPrice 
	   where InvoiceDateTime >= @date 
	   and InvoiceDateTime <= isnull(@nextPurchaseDate,GETDATE())
	   and ItemNo = @itemNo 

	   update ProductTransfersDetails set IssuePrice = @currentAvgPrice 
	   where TransferDate >= @date 
	   and TransferDate <= isnull(@nextPurchaseDate,GETDATE())
	   and FromItemNo = @itemNo 
	   and TransactionType in  ('RawCTC')

	  set @start = @start + 1

	 set @currentAvgPrice = 0
	 set @issueValue = 0
	 set @issueQty = 0
	 set @purchaseQty = 0
	 set @lastPurchaseValue = 0
	 set @lastPurchaseQty = 0

    end



--select * from ProductAvgPrice
--select * from #tempAVGPrice


drop table #tempAVGPrice
drop table #tempAVGPriceOrder
";

                        #endregion

                        cmd.CommandText = deletePurchaseIssues;
                        cmd.Parameters.AddWithValue("@processDate", paramVM.AvgDateTime);
                        cmd.ExecuteNonQuery();
                    }

                }

                rVM.Status = "Success";
                rVM.Message = "Day End Process Completed Successfully!";

                #endregion

                #region Transaction Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion
            }

            #endregion

            #region Catch and Finally

            catch (Exception ex)
            {

                rVM = new ResultVM();
                rVM.Message = ex.Message;

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Rollback();
                }

                ////FileLogger.Log("IssueDAL", "Issue Multiple Save", ex.Message + ex.StackTrace);
                FileLogger.Log("IssueDAL", "MultipleUpdate", ex.ToString());

                throw ex;
            }

            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }

            #endregion

            return rVM;
        }


        public ResultVM UpdateAvgPrice_New(AVGPriceVm paramVM, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Declarations

            ResultVM rVM = new ResultVM();
            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            #endregion

            #region Try Statement

            try
            {
                #region open connection and transaction

                #region New open connection and transaction

                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                #endregion New open connection and transaction

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction(DateTime.Now.ToString("ddMMyyyHH:mm:ss"));
                }

                #endregion open connection and transaction

                #region Update

                bool IssueFrom6_1 = _cDal.settings("Toll6_4", "IssueFrom6_1", currConn, transaction, connVM) == "Y"; // toll Issue
                bool TollReceiveNotWIP = _cDal.settings("IssueFromBOM", "TollReceive-NotWIP", currConn, transaction, connVM) == "Y"; // TollReceive-NotWIP
                bool TollReceiveWithIssue = _cDal.settings("TollReceive", "WithIssue", currConn, transaction, connVM) == "Y"; // TollReceive

                string[] transactionTypes = new[]
                {
                    "Other", "Trading", "TradingAuto", "ExportTrading", "TradingTender", "ExportTradingTender",
                    "InternalIssue", "Service", "ExportService", "InputServiceImport", "InputService", "Tender", "WIP",
                    "PackageProduction", "TollReceive-NotWIP", "TollIssue", "TollFinishReceive",
                    "ReceiveReturn", "TollReceive"
                }; // , "IssueReturn"

                if (!IssueFrom6_1)
                {
                    transactionTypes = transactionTypes.Where(x => x != "TollIssue").ToArray();
                }
                if (!TollReceiveNotWIP)
                {
                    transactionTypes = transactionTypes.Where(x => x != "TollReceive-NotWIP").ToArray();
                }
                if (!TollReceiveWithIssue)
                {
                    transactionTypes = transactionTypes.Where(x => x != "TollReceive").ToArray();
                }

                string sqlText = "";
                if (paramVM != null)
                {
                    List<IssueMasterVM> MasterVMs = new List<IssueMasterVM>();

                    sqlText = "select count(SL) from ProductAvgPrice";


                    SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
                    cmd.CommandTimeout = 3600;
                    int rows = Convert.ToInt32(cmd.ExecuteScalar());

                    if (rows == 0 || paramVM.FullProcess)
                    {
                        #region Initial AVG Price Query

                        sqlText = @"
--delete from ProductAvgPrice




create table #tempAVGPrice(
ID int identity(1,1),
ItemNo varchar(50),
AgvPriceDate datetime,
PurchaseQty   decimal(25,9),
PurchaseValue   decimal(25,9),
AvgPrice   decimal(25,9),
PurchaseNo varchar(50),
InsertTime datetime2(7),
TransactionType varchar(50)
)

create table #tempAVGPriceOrder(
ID int identity(1,1),
ItemNo varchar(50),
AgvPriceDate datetime,
PurchaseQty   decimal(25,9),
PurchaseValue   decimal(25,9),
AvgPrice   decimal(25,9),
PurchaseNo varchar(50),
InsertTime datetime2(7),
TransactionType varchar(50)
)


insert into #tempAVGPrice(ItemNo, AgvPriceDate,PurchaseQty,PurchaseValue,AvgPrice,PurchaseNo,InsertTime,TransactionType)
select p.ItemNO, '1900-01-01 00:00:00', 0,0,0,'',GetDate(),'Opening'
from Products p left outer join ProductCategories pc on p.CategoryID = pc.CategoryID
where pc.IsRaw in( 'raw','pack','trading','WIP') 
--and ItemNo = '37'
@itemCondition


------------------------------------------------------------OPENING-------------------------------------------------------


  declare @count int = (select count(BranchId) from BranchProfiles)

    IF @count = 1
    BEGIN
	    update #tempAVGPrice set PurchaseQty = Products.OpeningBalance, 
	    PurchaseValue=Products.OpeningTotalCost
	    from Products 
	    where #tempAVGPrice.ItemNo = Products.ItemNo and #tempAVGPrice.TransactionType='Opening'

		
    END

    ELSE

    BEGIN


	    create table #tempStocks(id int identity(1,1), ItemNo varchar(50), TotalQty   decimal(25,9), TotalValue   decimal(25,9))

	    insert into #tempStocks (ItemNo, TotalQty,TotalValue)
	    SELECT distinct ItemNo, isnull(sum(p.StockQuantity),0) Quantity, isnull(sum(p.StockValue),0) 
	    from ProductStocks p
	    group by ItemNo

	    update #tempAVGPrice set PurchaseQty = #tempStocks.TotalQty, 
	    PurchaseValue=#tempStocks.TotalValue
	    from #tempStocks 
	    where #tempAVGPrice.ItemNo = #tempStocks.ItemNo  and #tempAVGPrice.TransactionType='Opening'

	    drop table #tempStocks

    END


-----------------------------------------------------Opening-------------------------------------------------------------------




-----------------------------------------------------Purchase------------------------------------------------------------------
-----Check if with SD---------------'
declare @withSD varchar(50) = (select SettingValue from Settings 
where SettingGroup= 'VAT6_1' and 
SettingName= 'TotalIncludeSD'
)

declare @withOtherAMT varchar(50) = (select SettingValue from Settings 
where SettingGroup= 'VAT6_1' and 
SettingName= 'IncludeOtherAMT'
)

--------- 'Other','PurchaseCN','Trading','Service','ServiceNS','CommercialImporter','InputService' -----------------
insert into #tempAVGPrice(ItemNo,AgvPriceDate,PurchaseQty,PurchaseValue,AvgPrice,PurchaseNo,InsertTime,TransactionType)
select ItemNo, ReceiveDate, sum(isnull(UOMQty,0)),
case when @withSD='Y' then sum(SubTotal + SDAmount) else sum(SubTotal) end,
0 AvgPrice,PurchaseInvoiceNo,GetDate(),'Purchase'
from PurchaseInvoiceDetails
where TransactionType  in ('Other','PurchaseCN','Trading','Service','ServiceNS','CommercialImporter','InputService','ClientFGReceiveWOBOM')
and Post = 'Y'
--and ItemNo = '37'
@itemCondition
group by ItemNo, ReceiveDate,PurchaseInvoiceNo


--------- 'Toll Receive' -----------------
insert into #tempAVGPrice(ItemNo,AgvPriceDate,PurchaseQty,PurchaseValue,AvgPrice,PurchaseNo,InsertTime,TransactionType)
select ItemNo, ReceiveDate, sum(isnull(UOMQty,0))
,sum(SubTotal)
,0 AvgPrice
,PurchaseInvoiceNo,GetDate()
,'Purchase'
from PurchaseInvoiceDetails
where TransactionType  in ('TollReceive-WIP')
and Post = 'Y'
--and ItemNo = '37'
@itemCondition
group by ItemNo, ReceiveDate,PurchaseInvoiceNo


--------- RawCTC, WastageCTC -----------------
insert into #tempAVGPrice(ItemNo,AgvPriceDate,PurchaseQty,PurchaseValue,AvgPrice,PurchaseNo,InsertTime,TransactionType)
select ToItemNo, pd.TransferDate, sum(isnull(ToQuantity,0)),sum(isnull(ReceivePrice,0)),
0 AvgPrice,pt.TransferCode,GetDate(),'Purchase'
from ProductTransfersDetails pd left outer join ProductTransfers pt on pd.ProductTransferId = pt.id
where pd.TransactionType  in ('RawCTC')
and pd.Post = 'Y'
--and ItemNo = '37'
@itemCondition2
group by ToItemNo, pd.TransferDate,pt.TransferCode



--------- 'Import','ServiceImport','ServiceNSImport','TradingImport','InputServiceImport' -----------------
insert into #tempAVGPrice(ItemNo,AgvPriceDate,PurchaseQty,PurchaseValue,AvgPrice,PurchaseNo,InsertTime,TransactionType)
select ItemNo, ReceiveDate, sum(isnull(UOMQty,0)),

case when @withSD='Y' then 
sum(isnull(AssessableValue,0)+ isnull(CDAmount,0)+ isnull(RDAmount,0)+ isnull(TVBAmount,0)+ isnull(TVAAmount,0)
+(case when @withOtherAMT = 'Y' then isnull(OthersAmount,0) else 0 end)
+isnull(SDAmount,0))
else 
sum(isnull(AssessableValue,0)+ isnull(CDAmount,0)+ isnull(RDAmount,0)+ isnull(TVBAmount,0)+ isnull(TVAAmount,0)
+(case when @withOtherAMT = 'Y' then isnull(OthersAmount,0) else 0 end)
)
end,

0 AvgPrice,PurchaseInvoiceNo,GetDate(),'Purchase'
from PurchaseInvoiceDetails
where TransactionType  in ('Import','ServiceImport','ServiceNSImport','TradingImport','InputServiceImport')
and Post = 'Y'
--and ItemNo = '37'
@itemCondition
group by ItemNo, ReceiveDate,PurchaseInvoiceNo


--------- 'PurchaseReturn','PurchaseDN' -----------------
insert into #tempAVGPrice(ItemNo,AgvPriceDate,PurchaseQty,PurchaseValue,AvgPrice,PurchaseNo,InsertTime,TransactionType)
select ItemNo, ReceiveDate, sum(-1*isnull(UOMQty,0)),

case when @withSD='Y' then 
sum(-1*(isnull(subtotal,0)+isnull(SDAmount,0)))
else 
sum(-1*(isnull(subtotal,0)))
end,

0 AvgPrice,PurchaseInvoiceNo,GetDate(),'Purchase'
from PurchaseInvoiceDetails
where TransactionType  in ('PurchaseReturn','PurchaseDN')
and Post = 'Y' 
--and ItemNo = '37'
@itemCondition
group by ItemNo, ReceiveDate,PurchaseInvoiceNo

-----------------------------------------------------Purchase------------------------------------------------------------------

-----------------------------------------------------Ordering Data by Date------------------------------------------------------------------
insert into #tempAVGPriceOrder(ItemNo,AgvPriceDate,PurchaseQty,PurchaseValue,AvgPrice,PurchaseNo,InsertTime,TransactionType)
select ItemNo,AgvPriceDate,PurchaseQty,PurchaseValue,AvgPrice,PurchaseNo,InsertTime,TransactionType 
from #tempAVGPrice
order by AgvPriceDate

delete from #tempAVGPrice

insert into #tempAVGPrice(ItemNo,AgvPriceDate,PurchaseQty,PurchaseValue,AvgPrice,PurchaseNo,InsertTime,TransactionType)
select ItemNo,AgvPriceDate,PurchaseQty,PurchaseValue,AvgPrice,PurchaseNo,InsertTime,TransactionType 
from #tempAVGPriceOrder
order by AgvPriceDate, ID desc

delete from #tempAVGPriceOrder
-----------------------------------------------------Ordering Data by Date------------------------------------------------------------------

update #tempAVGPrice set AgvPriceDate = FORMAT(AgvPriceDate, 'yyyy-MM-dd 00:00:00')

--select * from #tempAVGPrice
--where ItemNo = '37'
--order by AgvPriceDate


	declare @start int  = (select min(ID) from #tempAvgPrice)
    declare @end int  = (select max(ID) from #tempAvgPrice)

	declare @purchaseQty   decimal(25,9)
	declare @purchaseValue   decimal(25,9)

	declare @purchaseNo varchar(50)
    declare @itemNo varchar(50) = ''
    declare @date datetime 
	declare @transactionType  varchar(50)
	declare @lastPurchasedate datetime
	declare @lastAvgPrice   decimal(25,9)
	declare @currentAvgPrice   decimal(25,9)
	declare @nextPurchaseDate datetime
	declare @lastPurchaseValue   decimal(25,9)
	declare @lastPurchaseQty   decimal(25,9)

	declare @issueQty   decimal(25,9)
	declare @issueValue   decimal(25,9)
	declare @runtimeValue decimal(25,9)
	declare @runtimeQty decimal(25,9)

    while @start <= @end
    begin
	   
	   select
	   @itemNo =ItemNo,
	   @date=AgvPriceDate,
	   @purchaseQty=PurchaseQty,
	   @purchaseValue=PurchaseValue,
	   @purchaseNo = PurchaseNo,
	   @transactionType = transactionType
	   --,@currentAvgPrice = AvgPrice
	   from #tempAvgPrice 
	   where ID = @start 

	   select top 1
	   @nextPurchaseDate = AgvPriceDate
	   from #tempAvgPrice 
	   where ID > @start and AgvPriceDate >= @date and ItemNo = @itemNo
	   order by AgvPriceDate asc, ID asc

	   select top 1 @lastPurchasedate=AgvPriceDate,@lastAvgPrice=AvgPrice,
	   @lastPurchaseValue = RunTimeTotal, @lastPurchaseQty = RunTimeQty
	   from ProductAvgPrice
	   where  AgvPriceDate <= @date and ItemNo = @itemNo and (TransactionType = 'Purchase' or TransactionType = 'Opening')
	   order by SL desc,AgvPriceDate desc

	   

	  if @transactionType = 'Purchase'
	  begin 
		
	-- @issueQty=sum(UOMQty), @issueValue=sum(Subtotal)

	select @issueQty=sum(UOMQty), @issueValue=sum(Subtotal) 

	from(

	    select sum(UOMQty)UOMQty, sum(Subtotal)Subtotal		  
		from IssueDetails 
		where ItemNo = @itemNo and IssueDateTime >= @lastPurchasedate and IssueDateTime < @date --format(CAST(@date as datetime),'yyyy-MM-dd 23:59:59')
		and TransactionType in ('" + string.Join("','", transactionTypes) + @"') --,'TollReceive'
        and Post = 'Y'

		union all

        select sum(UOMQty*-1)UOMQty, sum(Subtotal*-1)Subtotal		  
		from IssueDetails 
		where ItemNo = @itemNo and IssueDateTime >= @lastPurchasedate and IssueDateTime < @date
		and TransactionType in ('IssueReturn') --,'TollReceive'
        and Post = 'Y'

        union all

	    select sum(UOMQty),sum(UOMQty*AVGPrice) from SalesInvoiceDetails
	    where ItemNo = @itemNo and InvoiceDateTime >= @lastPurchasedate and InvoiceDateTime < @date
	    and TransactionType in ('RawSale')
        and Post = 'Y'

		union all

	    select sum(FromQuantity),sum(IssuePrice) from ProductTransfersDetails
	    where FromItemNo = @itemNo and TransferDate >= @lastPurchasedate and TransferDate < @date
	    and TransactionType in ('RawCTC') --'WastageCTC',
        and Post = 'Y'

		) Issues

	 if @issueQty is not null
	 begin
	  insert into ProductAvgPrice(ItemNo,AgvPriceDate, PurchaseQty, PurchaseValue, AvgPrice, InsertTime, TransactionType,FromDate ,RuntimeQty,RuntimeTotal)
	  select @ItemNo,DATEADD(d,-1,@date),@issueQty,@lastAvgPrice*@issueQty , @lastAvgPrice, GetDate(), 'Issue',@lastPurchasedate ,(isnull(@lastPurchaseQty,0)-isnull(@issueQty,0)),(isnull(@lastPurchaseValue,0)-isnull(@issueValue,0))
	 end



	  insert into ProductAvgPrice(ItemNo,AgvPriceDate, PurchaseQty, PurchaseValue,RuntimeTotal,RuntimeQty, AvgPrice, InsertTime, TransactionType,PurchaseNo,FromDate)
	  select @itemNo, @date, @purchaseQty, @purchaseValue,((@purchaseValue+isnull(@lastPurchaseValue,0))-isnull(@issueValue,0)),((@purchaseQty+isnull(@lastPurchaseQty,0))-isnull(@issueQty,0)),0,	  
	  GETDATE(), 'Purchase',@purchaseNo,@date



	  end
	else
	begin

	  insert into ProductAvgPrice(ItemNo,AgvPriceDate, PurchaseQty, PurchaseValue,RuntimeTotal,RuntimeQty, AvgPrice, InsertTime, TransactionType,PurchaseNo)
	  select @itemNo, @date, @purchaseQty, @purchaseValue, @purchaseValue,@purchaseQty,0,GETDATE(), 'Opening',@purchaseNo

	end


	   update ProductAvgPrice set AvgPrice = (RuntimeTotal/RuntimeQty)
	   where RuntimeQty > 0 and TransactionType in( 'Purchase','Opening')

	   if(((@purchaseQty+isnull(@lastPurchaseQty,0))-isnull(@issueQty,0)) > 0 and ((@purchaseValue+isnull(@lastPurchaseValue,0))-isnull(@issueValue,0)) >= 0)
	   begin
			set @currentAvgPrice = ((@purchaseValue+isnull(@lastPurchaseValue,0))-isnull(@issueValue,0)) / ((@purchaseQty+isnull(@lastPurchaseQty,0))-isnull(@issueQty,0))
	   end
	   else
	   begin
			set @currentAvgPrice = 0
	   end


	 --  if @itemNo = '37'
	 --  begin
		----select @itemNo,@date, @nextPurchaseDate,@transactionType, @currentAvgPrice
		--select @issueQty
	 --  end


	  -------------------------------------------- updating issue --------------------------------------
	   update IssueDetails set    UOMPrice = @currentAvgPrice
	   where IssueDateTime >= @date and IssueDateTime <= isnull(@nextPurchaseDate,GETDATE()) and ItemNo = @itemNo

       update IssueDetails set CostPrice=  convert(decimal(25,9), UOMPrice * UOMc), SubTotal= convert(decimal(25,9),(UOMPrice * UOMc)*Quantity )
	   where IssueDateTime >= @date and IssueDateTime <= isnull(@nextPurchaseDate,GETDATE()) and ItemNo = @itemNo



	   update SalesInvoiceDetails set AVGPrice = @currentAvgPrice 
	   where InvoiceDateTime >= @date 
	   and InvoiceDateTime <= isnull(@nextPurchaseDate,GETDATE())
	   and ItemNo = @itemNo 
       and TransactionType in ('RawSale')

       
	   update ProductTransfersDetails set IssuePrice = @currentAvgPrice 
	   where TransferDate >= @date 
	   and TransferDate <= isnull(@nextPurchaseDate,GETDATE())
	   and FromItemNo = @itemNo 
	   and TransactionType in  ('RawCTC')

	 -------------------------------------------- updating issue --------------------------------------
	  set @start = @start + 1

	 set @currentAvgPrice = 0
	 set @issueValue = 0
	 set @issueQty = 0
	 set @purchaseQty = 0
	 set @lastPurchaseValue = 0
	 set @lastPurchaseQty = 0
	 set @nextPurchaseDate = null
    end



select * from ProductAvgPrice
--select * from #tempAVGPrice


drop table #tempAVGPrice
drop table #tempAVGPriceOrder

";

                        #endregion

                        if (!string.IsNullOrEmpty(paramVM.ItemNo))
                        {
                            sqlText = sqlText.Replace("@itemCondition2", " and ToItemNo = @ItemNoSingle");
                            sqlText = sqlText.Replace("@itemCondition", " and ItemNo = @ItemNoSingle");

                        }
                        else
                        {
                            sqlText = sqlText.Replace("@itemCondition2", "");
                            sqlText = sqlText.Replace("@itemCondition", "");


                        }


                        cmd.CommandText = sqlText;
                        //cmd.CommandTimeout = 3600;


                        if (!string.IsNullOrEmpty(paramVM.ItemNo))
                        {
                            cmd.Parameters.AddWithValue("@ItemNoSingle", paramVM.ItemNo);
                        }

                        cmd.ExecuteNonQuery();

                    }
                    else
                    {

                        #region AVG Price Re-Process

                        string deletePurchaseIssues = @"

----declare @processDate datetime = '2021-02-28 00:00:00.000'


delete from ProductAvgPrice
where AgvPriceDate >= dateadd(d,-1,@processDate) and TransactionType='Issue'
@itemCondition

delete from ProductAvgPrice
where AgvPriceDate >= @processDate @itemCondition



create table #tempAVGPrice(
ID int identity(1,1),
ItemNo varchar(50),
AgvPriceDate datetime,
PurchaseQty   decimal(25,9),
PurchaseValue   decimal(25,9),
AvgPrice   decimal(25,9),
PurchaseNo varchar(50),
InsertTime datetime2(7),
TransactionType varchar(50)
)

create table #tempAVGPriceOrder(
ID int identity(1,1),
ItemNo varchar(50),
AgvPriceDate datetime,
PurchaseQty   decimal(25,9),
PurchaseValue   decimal(25,9),
AvgPrice   decimal(25,9),
PurchaseNo varchar(50),
InsertTime datetime2(7),
TransactionType varchar(50)
)


-----Check if with SD---------------'
declare @withSD varchar(50) = (select SettingValue from Settings 
where SettingGroup= 'VAT6_1' and 
SettingName= 'TotalIncludeSD'
)

declare @withOtherAMT varchar(50) = (select SettingValue from Settings 
where SettingGroup= 'VAT6_1' and 
SettingName= 'IncludeOtherAMT'
)
--------- 'Other','PurchaseCN','Trading','Service','ServiceNS','CommercialImporter','InputService' -----------------
insert into #tempAVGPrice(ItemNo,AgvPriceDate,PurchaseQty,PurchaseValue,AvgPrice,PurchaseNo,InsertTime,TransactionType)
select ItemNo, ReceiveDate, sum(isnull(UOMQty,0)),
case when @withSD='Y' then sum(SubTotal + SDAmount) else sum(SubTotal) end,
0 AvgPrice,PurchaseInvoiceNo,GetDate(),'Purchase'
from PurchaseInvoiceDetails

where TransactionType  in ('Other','PurchaseCN','Trading','Service','ServiceNS','CommercialImporter','InputService','ClientFGReceiveWOBOM')
and Post = 'Y'
and ReceiveDate >= @processDate
--and ItemNo = '37'
@itemCondition

group by ItemNo, ReceiveDate,PurchaseInvoiceNo


--------- RawCTC, WastageCTC -----------------
insert into #tempAVGPrice(ItemNo,AgvPriceDate,PurchaseQty,PurchaseValue,AvgPrice,PurchaseNo,InsertTime,TransactionType)
select ToItemNo, pd.TransferDate, sum(isnull(ToQuantity,0)),sum(isnull(ReceivePrice,0)),
0 AvgPrice,pt.TransferCode,GetDate(),'Purchase'
from ProductTransfersDetails pd left outer join ProductTransfers pt on pd.ProductTransferId = pt.id
where pd.TransactionType  in ('WastageCTC','RawCTC')
and pd.Post = 'Y'
and pd.TransferDate >= @processDate
--and ItemNo = '37'
@itemCondition2

group by ToItemNo, pd.TransferDate,pt.TransferCode



--------- 'Import','ServiceImport','ServiceNSImport','TradingImport','InputServiceImport' -----------------
insert into #tempAVGPrice(ItemNo,AgvPriceDate,PurchaseQty,PurchaseValue,AvgPrice,PurchaseNo,InsertTime,TransactionType)
select ItemNo, ReceiveDate, sum(isnull(UOMQty,0)),

case when @withSD='Y' then 
sum(isnull(AssessableValue,0)+ isnull(CDAmount,0)+ isnull(RDAmount,0)+ isnull(TVBAmount,0)+ isnull(TVAAmount,0)
+(case when @withOtherAMT = 'Y' then isnull(OthersAmount,0) else 0 end)
+isnull(SDAmount,0))
else 
sum(isnull(AssessableValue,0)+ isnull(CDAmount,0)+ isnull(RDAmount,0)+ isnull(TVBAmount,0)+ isnull(TVAAmount,0)
+(case when @withOtherAMT = 'Y' then isnull(OthersAmount,0) else 0 end))
end,

0 AvgPrice,PurchaseInvoiceNo,GetDate(),'Purchase'
from PurchaseInvoiceDetails
where TransactionType  in ('Import','ServiceImport','ServiceNSImport','TradingImport','InputServiceImport')
and Post = 'Y'
and ReceiveDate >= @processDate
--and ItemNo = '37'
@itemCondition

group by ItemNo, ReceiveDate,PurchaseInvoiceNo


--------- 'PurchaseReturn','PurchaseDN' -----------------
insert into #tempAVGPrice(ItemNo,AgvPriceDate,PurchaseQty,PurchaseValue,AvgPrice,PurchaseNo,InsertTime,TransactionType)
select ItemNo, ReceiveDate, sum(-1*isnull(UOMQty,0)),

case when @withSD='Y' then 
sum(-1*(isnull(subtotal,0)+isnull(SDAmount,0)))
else 
sum(-1*(isnull(subtotal,0)))
end,

0 AvgPrice,PurchaseInvoiceNo,GetDate(),'Purchase'
from PurchaseInvoiceDetails
where TransactionType  in ('PurchaseReturn','PurchaseDN')
and Post = 'Y'
and ReceiveDate >= @processDate
--and ItemNo = '37'
@itemCondition

group by ItemNo, ReceiveDate,PurchaseInvoiceNo



insert into #tempAVGPriceOrder(ItemNo,AgvPriceDate,PurchaseQty,PurchaseValue,AvgPrice,PurchaseNo,InsertTime,TransactionType)
select ItemNo,AgvPriceDate,PurchaseQty,PurchaseValue,AvgPrice,PurchaseNo,InsertTime,TransactionType from #tempAVGPrice
order by AgvPriceDate

delete from #tempAVGPrice
insert into #tempAVGPrice(ItemNo,AgvPriceDate,PurchaseQty,PurchaseValue,AvgPrice,PurchaseNo,InsertTime,TransactionType)
select ItemNo,AgvPriceDate,PurchaseQty,PurchaseValue,AvgPrice,PurchaseNo,InsertTime,TransactionType from #tempAVGPriceOrder
order by AgvPriceDate,ID

delete from #tempAVGPriceOrder

update #tempAVGPrice set AgvPriceDate = FORMAT(AgvPriceDate, 'yyyy-MM-dd 00:00:00')

select * from #tempAVGPrice
--where PurchaseNo = 'IMP-0013/0719'




	declare @start int  = (select min(ID) from #tempAvgPrice)
    declare @end int  = (select max(ID) from #tempAvgPrice)

	declare @purchaseQty   decimal(25,9)
	declare @purchaseValue   decimal(25,9)

	declare @purchaseNo varchar(50)
    declare @itemNo varchar(50) = ''
    declare @date datetime 
	declare @transactionType  varchar(50)
	declare @lastPurchasedate datetime
	declare @lastAvgPrice   decimal(25,9)
	declare @currentAvgPrice   decimal(25,9)
	declare @nextPurchaseDate datetime
	declare @lastPurchaseValue   decimal(25,9)
	declare @lastPurchaseQty   decimal(25,9)

	declare @issueQty   decimal(25,9)
	declare @issueValue   decimal(25,9)
	declare @runtimeValue decimal(25,9)
	declare @runtimeQty decimal(25,9)

    while @start <= @end
    begin
	   
	   select
	   @itemNo =ItemNo,
	   @date=AgvPriceDate,
	   @purchaseQty=PurchaseQty,
	   @purchaseValue=PurchaseValue,
	   @purchaseNo = PurchaseNo,
	   @transactionType = transactionType
	   --,@currentAvgPrice = AvgPrice
	   from #tempAvgPrice 
	   where ID = @start 

	   select top 1
	   @nextPurchaseDate = AgvPriceDate
	   from #tempAvgPrice 
	   where ID > @start and AgvPriceDate >= @date and ItemNo = @itemNo
	   order by AgvPriceDate asc, ID asc

	   select top 1 @lastPurchasedate=AgvPriceDate,@lastAvgPrice=AvgPrice,
	   @lastPurchaseValue = RunTimeTotal, @lastPurchaseQty = RunTimeQty
	   from ProductAvgPrice
	   where  AgvPriceDate <= @date and ItemNo = @itemNo and (TransactionType = 'Purchase' or TransactionType = 'Opening')
	   order by SL desc,AgvPriceDate desc

	   

	  if @transactionType = 'Purchase'
	  begin 
		
	select @issueQty=sum(UOMQty), @issueValue=sum(Subtotal) 

	from(
	   select sum(UOMQty)UOMQty, sum(Subtotal)Subtotal
		  
		from IssueDetails 
		where ItemNo = @itemNo and IssueDateTime >= @lastPurchasedate and IssueDateTime < @date
		and TransactionType in ('" + string.Join("','", transactionTypes) + @"') -- ,'TollIssue'
        and Post = 'Y'

		union all

        select sum(UOMQty*-1)UOMQty, sum(Subtotal*-1)Subtotal		  
		from IssueDetails 
		where ItemNo = @itemNo and IssueDateTime >= @lastPurchasedate and IssueDateTime < @date
		and TransactionType in ('IssueReturn') --,'TollReceive'

		union all

	    select sum(UOMQty),sum(UOMQty*AVGPrice) from SalesInvoiceDetails
	    where ItemNo = @itemNo and InvoiceDateTime >= @lastPurchasedate and InvoiceDateTime < @date
	    and TransactionType in ('RawSale')

        union all

	    select sum(FromQuantity),sum(IssuePrice) from ProductTransfersDetails
	    where FromItemNo = @itemNo and TransferDate >= @lastPurchasedate and TransferDate < @date
	    and TransactionType in ('RawCTC')

		) Issues


	 if @issueQty is not null
	 begin
			  insert into ProductAvgPrice(ItemNo,AgvPriceDate, PurchaseQty, PurchaseValue, AvgPrice, InsertTime, TransactionType,FromDate  ,RuntimeQty,RuntimeTotal)
	  select @ItemNo,DATEADD(d,-1,@date),@issueQty,@lastAvgPrice*@issueQty , @lastAvgPrice, GetDate(), 'Issue',@lastPurchasedate ,(isnull(@lastPurchaseQty,0)-isnull(@issueQty,0)),(isnull(@lastPurchaseValue,0)-isnull(@issueValue,0))
	 end



	  insert into ProductAvgPrice(ItemNo,AgvPriceDate, PurchaseQty, PurchaseValue,RuntimeTotal,RuntimeQty, AvgPrice, InsertTime, TransactionType,PurchaseNo,FromDate)
	  select @itemNo, @date, @purchaseQty, @purchaseValue,((@purchaseValue+isnull(@lastPurchaseValue,0))-isnull(@issueValue,0)),((@purchaseQty+isnull(@lastPurchaseQty,0))-isnull(@issueQty,0)),0,	  
	  GETDATE(), 'Purchase',@purchaseNo,@date



	  end
	else
	begin



	  insert into ProductAvgPrice(ItemNo,AgvPriceDate, PurchaseQty, PurchaseValue,RuntimeTotal,RuntimeQty, AvgPrice, InsertTime, TransactionType,PurchaseNo)
	  select @itemNo, @date, @purchaseQty, @purchaseValue, @purchaseValue,@purchaseQty,0,GETDATE(), 'Opening',@purchaseNo

	end


	   update ProductAvgPrice set AvgPrice = (RuntimeTotal/RuntimeQty)
	   where RuntimeQty > 0 and TransactionType in( 'Purchase','Opening')

	   if(((@purchaseQty+isnull(@lastPurchaseQty,0))-isnull(@issueQty,0)) > 0 and ((@purchaseValue+isnull(@lastPurchaseValue,0))-isnull(@issueValue,0)) >= 0)
	   begin
		--select @itemNo, @purchaseNo, @purchaseQty,@lastPurchaseQty
			set @currentAvgPrice = ((@purchaseValue+isnull(@lastPurchaseValue,0))-isnull(@issueValue,0)) / ((@purchaseQty+isnull(@lastPurchaseQty,0))-isnull(@issueQty,0))

	   end
	   else
	   begin
			set @currentAvgPrice = 0

	   end

	  -------------------------------------------- updating issue --------------------------------------
	   update IssueDetails set CostPrice= @currentAvgPrice * UOMc, SubTotal=(@currentAvgPrice * UOMc)*Quantity,
	   UOMPrice = @currentAvgPrice
	   where cast(IssueDateTime as date) >= @date and cast(IssueDateTime as date) <= isnull(@nextPurchaseDate,GETDATE()) and ItemNo = @itemNo

	   
	   update SalesInvoiceDetails set AVGPrice = @currentAvgPrice 
	   where InvoiceDateTime >= @date 
	   and InvoiceDateTime <= isnull(@nextPurchaseDate,GETDATE())
	   and ItemNo = @itemNo 

	   update ProductTransfersDetails set IssuePrice = @currentAvgPrice 
	   where TransferDate >= @date 
	   and TransferDate <= isnull(@nextPurchaseDate,GETDATE())
	   and FromItemNo = @itemNo 
	   and TransactionType in  ('RawCTC')

	  set @start = @start + 1

	 set @currentAvgPrice = 0
	 set @issueValue = 0
	 set @issueQty = 0
	 set @purchaseQty = 0
	 set @lastPurchaseValue = 0
	 set @lastPurchaseQty = 0
	 set @nextPurchaseDate = null

    end



--select * from ProductAvgPrice
--select * from #tempAVGPrice


drop table #tempAVGPrice
drop table #tempAVGPriceOrder
";

                        #endregion

                        if (!string.IsNullOrEmpty(paramVM.ItemNo))
                        {
                            deletePurchaseIssues = deletePurchaseIssues.Replace("@itemCondition2", " and ToItemNo = @ItemNoSingle");
                            deletePurchaseIssues = deletePurchaseIssues.Replace("@itemCondition", " and ItemNo = @ItemNoSingle");
                        }
                        else
                        {
                            deletePurchaseIssues = deletePurchaseIssues.Replace("@itemCondition2", "");
                            deletePurchaseIssues = deletePurchaseIssues.Replace("@itemCondition", "");

                        }

                        cmd.CommandText = deletePurchaseIssues;
                        cmd.Parameters.AddWithValue("@processDate", paramVM.AvgDateTime);

                        if (!string.IsNullOrEmpty(paramVM.ItemNo))
                        {
                            cmd.Parameters.AddWithValue("@ItemNoSingle", paramVM.ItemNo);
                        }
                        cmd.ExecuteNonQuery();
                    }

                }

                rVM.Status = "Success";
                rVM.Message = "Day End Process Completed Successfully!";

                #endregion

                #region Transaction Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion
            }

            #endregion

            #region Catch and Finally

            catch (Exception ex)
            {

                rVM = new ResultVM();
                rVM.Message = ex.Message;

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Rollback();
                }

                ////FileLogger.Log("IssueDAL", "Issue Multiple Save", ex.Message + ex.StackTrace);
                FileLogger.Log("IssueDAL", "MultipleUpdate", ex.ToString());

                throw ex;
            }

            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }

            #endregion

            return rVM;
        }



        public ResultVM UpdateAvgPrice_New_Partition(AVGPriceVm paramVM, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Declarations

            ResultVM rVM = new ResultVM();
            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            #endregion

            #region Try Statement

            try
            {
                #region open connection and transaction

                #region New open connection and transaction

                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                #endregion New open connection and transaction

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction(DateTime.Now.ToString("ddMMyyyHH:mm:ss"));
                }

                #endregion open connection and transaction

                #region Update

                bool IssueFrom6_1 = _cDal.settings("Toll6_4", "IssueFrom6_1", currConn, transaction, connVM) == "Y"; // toll Issue
                bool TollReceiveNotWIP = _cDal.settings("IssueFromBOM", "TollReceive-NotWIP", currConn, transaction, connVM) == "Y"; // TollReceive-NotWIP
                bool TollReceiveWithIssue = _cDal.settings("TollReceive", "WithIssue", currConn, transaction, connVM) == "Y"; // TollReceive

                string[] transactionTypes = new[]
                {
                    "Other", "Trading", "TradingAuto", "ExportTrading", "TradingTender", "ExportTradingTender",
                    "InternalIssue", "Service", "ExportService", "InputServiceImport", "InputService", "Tender", "WIP",
                    "PackageProduction", "TollReceive-NotWIP", "TollIssue", "TollFinishReceive",
                    "ReceiveReturn", "TollReceive"
                }; // , "IssueReturn"

                if (!IssueFrom6_1)
                {
                    transactionTypes = transactionTypes.Where(x => x != "TollIssue").ToArray();
                }
                if (!TollReceiveNotWIP)
                {
                    transactionTypes = transactionTypes.Where(x => x != "TollReceive-NotWIP").ToArray();
                }
                if (!TollReceiveWithIssue)
                {
                    transactionTypes = transactionTypes.Where(x => x != "TollReceive").ToArray();
                }

                string sqlText = "";
                if (paramVM != null)
                {
                    List<IssueMasterVM> MasterVMs = new List<IssueMasterVM>();

                    sqlText = "select count(SL) from ProductAvgPrice";

                    SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
                    cmd.CommandTimeout = 3600;
                    int rows = Convert.ToInt32(cmd.ExecuteScalar());

                    #region Initial AVG Price Query

                    sqlText = @"
--delete from ProductAvgPrice


create table #tempAVGPrice(
ID int identity(1,1),
SerialNo varchar(50),
ItemNo varchar(50),
AgvPriceDate datetime,
PurchaseQty   decimal(25,9),
PurchaseValue   decimal(25,9),
AvgPrice   decimal(25,9),
PurchaseNo varchar(50),
InsertTime datetime2(7),
TransactionType varchar(50)
)

create table #tempAVGPriceOrder(
ID int identity(1,1),
SerialNo varchar(50),
ItemNo varchar(50),
AgvPriceDate datetime,
PurchaseQty   decimal(25,9),
PurchaseValue   decimal(25,9),
AvgPrice   decimal(25,9),
PurchaseNo varchar(50),
InsertTime datetime2(7),
TransactionType varchar(50)
)


insert into #tempAVGPrice(SerialNo,ItemNo, AgvPriceDate,PurchaseQty,PurchaseValue,AvgPrice,PurchaseNo,InsertTime,TransactionType)
select 'A',p.ItemNO, '1900-01-01 00:00:00', 0,0,0,'',GetDate(),'Opening'
from Products p left outer join ProductCategories pc on p.CategoryID = pc.CategoryID
where pc.IsRaw in( 'raw','pack','trading','WIP','Overhead') 
and p.ItemNo not in (select ItemNo from ProductAvgPrice where TransactionType = 'Opening')
--and ItemNo = '37'
@itemCondition

------------------------------------------------------------OPENING-------------------------------------------------------

  declare @count int = (select count(BranchId) from BranchProfiles)

    IF @count = 1
    BEGIN
	    update #tempAVGPrice set PurchaseQty = Products.OpeningBalance, 
	    PurchaseValue=Products.OpeningTotalCost
	    from Products 
	    where #tempAVGPrice.ItemNo = Products.ItemNo and #tempAVGPrice.TransactionType='Opening'

    END

    ELSE

    BEGIN

	    create table #tempStocks(id int identity(1,1), ItemNo varchar(50), TotalQty   decimal(25,9), TotalValue   decimal(25,9))

	    insert into #tempStocks (ItemNo, TotalQty,TotalValue)
	    SELECT distinct ItemNo, isnull(sum(StockQuantity),0) Quantity, isnull(sum(StockValue),0) 
	    from ProductStocks 
        where 1=1
        @itemCondition
	    group by ItemNo

	    update #tempAVGPrice set PurchaseQty = #tempStocks.TotalQty, 
	    PurchaseValue=#tempStocks.TotalValue
	    from #tempStocks 
	    where #tempAVGPrice.ItemNo = #tempStocks.ItemNo  and #tempAVGPrice.TransactionType='Opening'

	    drop table #tempStocks

    END


-----------------------------------------------------Opening-------------------------------------------------------------------


-----------------------------------------------------Purchase------------------------------------------------------------------
-----Check if with SD---------------'
declare @withSD varchar(50) = (select SettingValue from Settings 
where SettingGroup= 'VAT6_1' and 
SettingName= 'TotalIncludeSD'
)

declare @withOtherAMT varchar(50) = (select SettingValue from Settings 
where SettingGroup= 'VAT6_1' and 
SettingName= 'IncludeOtherAMT'
)

--------- 'Other','PurchaseCN','Trading','Service','ServiceNS','CommercialImporter','InputService' -----------------
insert into #tempAVGPrice(SerialNo,ItemNo,AgvPriceDate,PurchaseQty,PurchaseValue,AvgPrice,PurchaseNo,InsertTime,TransactionType)
select 'A1',ItemNo, ReceiveDate, sum(isnull(UOMQty,0)),
case when @withSD='Y' then sum(SubTotal + SDAmount) else sum(SubTotal) end,
0 AvgPrice,PurchaseInvoiceNo,GetDate(),'Purchase'
from PurchaseInvoiceDetails
where TransactionType  in ('Other','PurchaseCN','Trading','Service','ServiceNS','CommercialImporter','InputService','ClientFGReceiveWOBOM')
and Post = 'Y'
--and ItemNo = '37'
@itemCondition
@itemCondition5
group by ItemNo, ReceiveDate,PurchaseInvoiceNo


--------- 'Toll Receive' -----------------
insert into #tempAVGPrice(SerialNo,ItemNo,AgvPriceDate,PurchaseQty,PurchaseValue,AvgPrice,PurchaseNo,InsertTime,TransactionType)
select 'A1',ItemNo, ReceiveDate, sum(isnull(UOMQty,0))
,sum(SubTotal)
,0 AvgPrice
,PurchaseInvoiceNo,GetDate()
,'Purchase'
from PurchaseInvoiceDetails
where TransactionType  in ('TollReceive-WIP')
and Post = 'Y'
--and ItemNo = '37'
@itemCondition
@itemCondition5
group by ItemNo, ReceiveDate,PurchaseInvoiceNo


--------- RawCTC, WastageCTC -----------------
insert into #tempAVGPrice(SerialNo,ItemNo,AgvPriceDate,PurchaseQty,PurchaseValue,AvgPrice,PurchaseNo,InsertTime,TransactionType)
select 'A1',ToItemNo, pd.TransferDate, sum(isnull(ToQuantity,0)),sum(isnull(ReceivePrice,0)),
0 AvgPrice,pt.TransferCode,GetDate(),'Purchase'
from ProductTransfersDetails pd left outer join ProductTransfers pt on pd.ProductTransferId = pt.id
where pd.TransactionType  in ('RawCTC')
and pd.Post = 'Y'
--and ItemNo = '37'
@itemCondition2
@itemCondition6
group by ToItemNo, pd.TransferDate,pt.TransferCode


--------- 'Import','ServiceImport','ServiceNSImport','TradingImport','InputServiceImport' -----------------
insert into #tempAVGPrice(SerialNo,ItemNo,AgvPriceDate,PurchaseQty,PurchaseValue,AvgPrice,PurchaseNo,InsertTime,TransactionType)
select 'A1',ItemNo, ReceiveDate, sum(isnull(UOMQty,0)),

case when @withSD='Y' then 
sum(isnull(AssessableValue,0)+ isnull(CDAmount,0)+ isnull(RDAmount,0)+ isnull(TVBAmount,0)+ isnull(TVAAmount,0)
+(case when @withOtherAMT = 'Y' then isnull(OthersAmount,0) else 0 end)
+isnull(SDAmount,0))
else 
sum(isnull(AssessableValue,0)+ isnull(CDAmount,0)+ isnull(RDAmount,0)+ isnull(TVBAmount,0)+ isnull(TVAAmount,0)
+(case when @withOtherAMT = 'Y' then isnull(OthersAmount,0) else 0 end)
)
end,

0 AvgPrice,PurchaseInvoiceNo,GetDate(),'Purchase'
from PurchaseInvoiceDetails
where TransactionType  in ('Import','ServiceImport','ServiceNSImport','TradingImport','InputServiceImport')
and Post = 'Y'
--and ItemNo = '37'
@itemCondition
@itemCondition5
group by ItemNo, ReceiveDate,PurchaseInvoiceNo

--------- 'PurchaseReturn','PurchaseDN' -----------------
insert into #tempAVGPrice(SerialNo,ItemNo,AgvPriceDate,PurchaseQty,PurchaseValue,AvgPrice,PurchaseNo,InsertTime,TransactionType)
select 'A1',ItemNo, ReceiveDate, sum(-1*isnull(UOMQty,0)),

case when @withSD='Y' then 
sum(-1*(isnull(subtotal,0)+isnull(SDAmount,0)))
else 
sum(-1*(isnull(subtotal,0)))
end,

0 AvgPrice,PurchaseInvoiceNo,GetDate(),'Purchase'
from PurchaseInvoiceDetails
where TransactionType  in ('PurchaseReturn','PurchaseDN')
and Post = 'Y' 
--and ItemNo = '37'
@itemCondition
@itemCondition5
group by ItemNo, ReceiveDate,PurchaseInvoiceNo

-----------------------------------------------------Purchase------------------------------------------------------------------

----------------------------------------------------- Issue -------------------------------------------------------------------

update #tempAVGPrice set AgvPriceDate = FORMAT(AgvPriceDate, 'yyyy-MM-dd 00:00:00')

INSERT INTO #tempAVGPrice(SerialNo, ItemNo, AgvPriceDate, PurchaseQty, PurchaseValue, AvgPrice, PurchaseNo, InsertTime,
                          TransactionType)

SELECT 'B',
       ItemNo,
       CAST(IssueDateTime AS DATE) IssueDateTime,
       SUM(UOMQty)                 UOMQty,
       SUM(Subtotal)               Subtotal,
       0,
       '',
       GETDATE(),
       'Issue'
FROM IssueDetails
WHERE 1 = 1
  AND TransactionType IN ('" + string.Join("','", transactionTypes) + @"') --,'TollReceive'
  AND Post = 'Y'
@itemCondition
@itemCondition7
GROUP BY ItemNo, CAST(IssueDateTime AS DATE)


INSERT INTO #tempAVGPrice(SerialNo, ItemNo, AgvPriceDate, PurchaseQty, PurchaseValue, AvgPrice, PurchaseNo, InsertTime,
                          TransactionType)

SELECT 'B',
       ItemNo,
       CAST(IssueDateTime AS DATE) IssueDateTime,
       -1 * SUM(UOMQty)            UOMQty,
       -1 * SUM(Subtotal)          Subtotal,
       0,
       '',
       GETDATE(),
       'Issue'
FROM IssueDetails
WHERE 1 = 1
  AND TransactionType IN ('IssueReturn') --,'TollReceive'
  AND Post = 'Y' 
@itemCondition
@itemCondition7
GROUP BY ItemNo, CAST(IssueDateTime AS DATE)


INSERT INTO #tempAVGPrice(SerialNo, ItemNo, AgvPriceDate, PurchaseQty, PurchaseValue, AvgPrice, PurchaseNo, InsertTime,
                          TransactionType)

SELECT 'B',
       ItemNo,
       CAST(InvoiceDateTime AS DATE) InvoiceDateTime,
       SUM(UOMQty)                   UOMQty,
       SUM(Subtotal)                 Subtotal,
       0,
       '',
       GETDATE(),
       'Issue'
FROM SalesInvoiceDetails
WHERE 1 = 1
  AND TransactionType IN ('RawSale') --,'TollReceive'
  AND Post = 'Y' 
@itemCondition
@itemCondition8
GROUP BY ItemNo, CAST(InvoiceDateTime AS DATE)


INSERT INTO #tempAVGPrice(SerialNo, ItemNo, AgvPriceDate, PurchaseQty, PurchaseValue, AvgPrice, PurchaseNo, InsertTime,
                          TransactionType)

SELECT 'B',
       FromItemNo,
       CAST(TransferDate AS DATE) InvoiceDateTime,
       SUM(FromQuantity)          UOMQty,
       SUM(IssuePrice)            Subtotal,
       0,
       '',
       GETDATE(),
       'Issue'
FROM ProductTransfersDetails pd
WHERE 1 = 1 
@itemCondition3
@itemCondition6
  AND pd.TransactionType IN ('RawCTC') --,'TollReceive'
  AND pd.Post = 'Y'
GROUP BY FromItemNo, CAST(TransferDate AS DATE)


-----------------------------------------------------Ordering Data by Date------------------------------------------------------------------
insert into ProductAvgPrice(SerialNo,ItemNo,AgvPriceDate,PurchaseQty,PurchaseValue,AvgPrice,PurchaseNo,InsertTime,TransactionType)
select SerialNo, ItemNo,AgvPriceDate,PurchaseQty,PurchaseValue,AvgPrice,PurchaseNo,InsertTime,TransactionType 
from #tempAVGPrice
order by ItemNo, AgvPriceDate, SerialNo

delete from #tempAVGPrice

-----------------------------------------------------Ordering Data by Date------------------------------------------------------------------

Update ProductAvgPrice set RuntimeQty
= ProductTotals.RunningQty
From (
select SL, ItemNo, sum(case when (TransactionType = 'purchase' or TransactionType = 'Opening') then PurchaseQty else PurchaseQty*-1 end) over (partition by ItemNo order by ItemNo,agvPriceDate,SerialNo,SL) RunningQty
from ProductAvgPrice 
where 1=1 @itemCondition

) as ProductTotals

where ProductAvgPrice.SL = ProductTotals.SL


update ProductAvgPrice set RuntimeTotal = PurchaseValue, AvgPrice = PurchaseValue/RuntimeQty
where RuntimeQty != 0 and TransactionType = 'Opening' @itemCondition

update ProductAvgPrice set RuntimeTotal = PurchaseValue, AvgPrice = 0
where RuntimeQty = 0 and TransactionType = 'Opening' @itemCondition


create table #InitialData ( 
 ItemNo varchar(50)
 , AgvPriceDate datetime
 , PurchaseQty decimal(25,9)
, PurchaseValue decimal(25,9)
, RuntimeQty decimal(25,9)
, RuntimeTotal decimal(25,9)
, row_no int 
, SerialNo varchar(50)
, AvgPrice decimal(25,9)
, TransactionType varchar(50)
, SL int
CONSTRAINT PK_Person PRIMARY KEY (row_no,ItemNo)
)

insert into #InitialData 
SELECT       ItemNo,
                       AgvPriceDate,
                       PurchaseQty,
                       CAST(PurchaseValue AS DECIMAL(25,9)) PurchaseValue,
                       RuntimeQty,
                       CAST(RuntimeTotal AS DECIMAL(25,9)) RuntimeTotal,
              row_no = ROW_NUMBER() OVER (PARTITION BY ItemNo ORDER BY ItemNo, AgvPriceDate, SerialNo, SL),
                       SerialNo,
					   CAST(AvgPrice AS DECIMAL(25,9)) AvgPrice,
					   TransactionType,
					   SL
          FROM ProductAvgPrice where 1=1 
 
 ; with   rcte
         AS
         (SELECT ItemNo,
                 AgvPriceDate,
                 PurchaseQty,
                 CAST(PurchaseValue AS DECIMAL(25,9)) PurchaseValue,
                 RuntimeQty,
                 CAST(RuntimeTotal AS DECIMAL(25,9)) RuntimeTotal,
                 row_no,
                 SerialNo,
				 CAST(AvgPrice AS DECIMAL(25,9)) AvgPrice,
				 TransactionType
				 ,SL
          FROM #InitialData
          WHERE row_no = 1

          UNION ALL

          SELECT c.ItemNo,
                 c.AgvPriceDate,
                 c.PurchaseQty,
                 PurchaseValue = case when c.TransactionType = 'Issue' then CAST((r.AvgPrice * c.PurchaseQty) AS DECIMAL(25,9)) else c.PurchaseValue end,
                 c.RuntimeQty,
                 RuntimeTotal =  (case when c.TransactionType = 'Issue' then case when CAST((r.RuntimeTotal - (r.AvgPrice * c.PurchaseQty)) as decimal(25,9)) >0 then CAST((r.RuntimeTotal - (r.AvgPrice * c.PurchaseQty)) as decimal(25,9)) else 0 end else
				 CAST((r.RuntimeTotal + c.PurchaseValue) as decimal(25,9)) end) ,
                 c.row_no,
                 c.SerialNo,
--AvgPrice = case when c.RuntimeQty > 0 then  (case when c.TransactionType = 'Issue' then case when CAST((r.RuntimeTotal - (r.AvgPrice * c.PurchaseQty))/c.RuntimeQty as decimal(25,9)) > 0 then CAST((r.RuntimeTotal - (r.AvgPrice * c.PurchaseQty))/c.RuntimeQty as decimal(25,9)) else 0 end
--else case when CAST((r.RuntimeTotal + c.PurchaseValue)/c.RuntimeQty as decimal(25,9)) > 0 then CAST((r.RuntimeTotal + c.PurchaseValue)/c.RuntimeQty as decimal(25,9)) else 0 end end) else 0 end ,
				
AvgPrice= case when c.RuntimeQty  >0
then cast(cast(( case when c.RuntimeQty >0 then   (case when c.TransactionType = 'Issue' then case when CAST((cast(r.RuntimeTotal AS DECIMAL(25,9)) - (cast(r.AvgPrice AS DECIMAL(25,9)) * cast(c.PurchaseQty AS DECIMAL(25,9)))) as decimal(25,9)) >0 then CAST((cast(r.RuntimeTotal  AS DECIMAL(25,9))- (cast(r.AvgPrice AS DECIMAL(25,9)) * cast(c.PurchaseQty AS DECIMAL(25,9)))) as decimal(25,9)) else 0.0 end else CAST((cast(r.RuntimeTotal  AS DECIMAL(25,9))+ cast(c.PurchaseValue AS DECIMAL(25,9))) as decimal(25,9)) end) else 0 end ) AS DECIMAL(25,9))/cast(c.RuntimeQty AS DECIMAL(25,9))AS DECIMAL(25,9))
when c.RuntimeQty =0 and c.TransactionType = 'Issue'then cast(r.AvgPrice   AS DECIMAL(25,9))
else 0.0 end,	

c.TransactionType
				,c.SL
          FROM #InitialData c
                   INNER JOIN rcte r ON c.ItemNo = r.ItemNo
              AND c.row_no = r.row_no + 1)


select * into #tempResult
from rcte
ORDER BY ItemNo, AgvPriceDate, SerialNo
OPTION (MAXRECURSION 0)

 

update ProductAvgPrice set AvgPrice = isnull(ap.AvgPrice,0)
, PurchaseValue = isnull(ap.PurchaseValue,0)
,RuntimeTotal= isnull(ap.RuntimeTotal,0)
from  (
SELECT *
FROM #tempResult
--ORDER BY ItemNo, AgvPriceDate, SerialNo
) as AP 
inner join ProductAvgPrice pap on AP.SL = pap.SL and AP.ItemNo = pap.ItemNo



update IssueDetails
set CostPrice = UOMc*pap.AvgPrice ,SubTotal = UOMc*pap.AvgPrice *Quantity, UOMPrice = pap.AvgPrice
from IssueDetails id inner join 
ProductAvgPrice pap
on id.ItemNo = pap.ItemNo
and cast(id.IssueDateTime as date) = pap.AgvPriceDate
and pap.TransactionType = 'Issue'
where id.TransactionType in ('" + string.Join("','", transactionTypes) + @"') 
@itemCondition4
@itemCondition7


update SalesInvoiceDetails
set AVGPrice = pap.AvgPrice
--, SubTotal = pap.AvgPrice * Quantity

from SalesInvoiceDetails id inner join 
ProductAvgPrice pap
on id.ItemNo = pap.ItemNo
and cast(id.InvoiceDateTime as date) = pap.AgvPriceDate
and pap.TransactionType = 'Issue'
where id.TransactionType in ('rawsale') 
@itemCondition4
@itemCondition8

 
drop table #tempResult
drop table #InitialData

drop table #tempAVGPrice
drop table #tempAVGPriceOrder

";

                    #endregion

                    if (!string.IsNullOrEmpty(paramVM.AvgDateTime))
                    {
                        sqlText = sqlText.Replace("@itemCondition5", " and ReceiveDate >= @date");
                        sqlText = sqlText.Replace("@itemCondition6", " and pd.TransferDate >= @date");
                        sqlText = sqlText.Replace("@itemCondition7", " and IssueDateTime >= @date");
                        sqlText = sqlText.Replace("@itemCondition8", " and InvoiceDateTime >= @date");
                    }
                    else
                    {
                        sqlText = sqlText.Replace("@itemCondition5", "");
                        sqlText = sqlText.Replace("@itemCondition6", "");
                        sqlText = sqlText.Replace("@itemCondition7", "");
                        sqlText = sqlText.Replace("@itemCondition8", "");
                    }

                    if (!string.IsNullOrEmpty(paramVM.ItemNo))
                    {
                        sqlText = sqlText.Replace("@itemCondition4", " and id.ItemNo = @ItemNoSingle");
                        sqlText = sqlText.Replace("@itemCondition3", " and pd.FromItemNo = @ItemNoSingle");
                        sqlText = sqlText.Replace("@itemCondition2", " and ToItemNo = @ItemNoSingle");
                        sqlText = sqlText.Replace("@itemCondition", " and ItemNo = @ItemNoSingle");

                    }
                    else
                    {
                        sqlText = sqlText.Replace("@itemCondition4", "");
                        sqlText = sqlText.Replace("@itemCondition2", "");
                        sqlText = sqlText.Replace("@itemCondition3", "");
                        sqlText = sqlText.Replace("@itemCondition", "");

                    }

                    cmd.CommandText = sqlText;
                    //cmd.CommandTimeout = 3600;

                    if (!string.IsNullOrEmpty(paramVM.ItemNo))
                    {
                        cmd.Parameters.AddWithValue("@ItemNoSingle", paramVM.ItemNo);
                    }

                    if (!string.IsNullOrEmpty(paramVM.AvgDateTime))
                    {
                        cmd.Parameters.AddWithValue("@date", paramVM.AvgDateTime);
                    }

                    cmd.ExecuteNonQuery();

                }

                rVM.Status = "Success";
                rVM.Message = "Day End Process Completed Successfully!";

                #endregion

                #region Transaction Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion
            }

            #endregion

            #region Catch and Finally

            catch (Exception ex)
            {

                rVM = new ResultVM();
                rVM.Message = ex.Message;

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Rollback();
                }

                ////FileLogger.Log("IssueDAL", "Issue Multiple Save", ex.Message + ex.StackTrace);
                FileLogger.Log("IssueDAL", "MultipleUpdate", ex.ToString());

                throw ex;
            }

            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }

            #endregion

            return rVM;
        }



        public ResultVM UpdateAvgPrice_NewXX(AVGPriceVm paramVM, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Declarations

            ResultVM rVM = new ResultVM();
            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            #endregion

            #region Try Statement

            try
            {
                #region open connection and transaction

                #region New open connection and transaction

                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                #endregion New open connection and transaction

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction(DateTime.Now.ToString("ddMMyyyHH:mm:ss"));
                }

                #endregion open connection and transaction

                #region Update

                bool IssueFrom6_1 = _cDal.settings("Toll6_4", "IssueFrom6_1", currConn, transaction, connVM) == "Y"; // toll Issue
                bool TollReceiveNotWIP = _cDal.settings("IssueFromBOM", "TollReceive-NotWIP", currConn, transaction, connVM) == "Y"; // TollReceive-NotWIP
                bool TollReceiveWithIssue = _cDal.settings("TollReceive", "WithIssue", currConn, transaction, connVM) == "Y"; // TollReceive

                string[] transactionTypes = new[]
                {
                    "Other", "Trading", "TradingAuto", "ExportTrading", "TradingTender", "ExportTradingTender",
                    "InternalIssue", "Service", "ExportService", "InputServiceImport", "InputService", "Tender", "WIP",
                    "PackageProduction", "TollReceive-NotWIP", "TollIssue", "TollFinishReceive",
                    "ReceiveReturn", "TollReceive"
                }; // , "IssueReturn"

                if (!IssueFrom6_1)
                {
                    transactionTypes = transactionTypes.Where(x => x != "TollIssue").ToArray();
                }
                if (!TollReceiveNotWIP)
                {
                    transactionTypes = transactionTypes.Where(x => x != "TollReceive-NotWIP").ToArray();
                }
                if (!TollReceiveWithIssue)
                {
                    transactionTypes = transactionTypes.Where(x => x != "TollReceive").ToArray();
                }

                string sqlText = "";
                if (paramVM != null)
                {
                    List<IssueMasterVM> MasterVMs = new List<IssueMasterVM>();

                    sqlText = "select count(SL) from ProductAvgPrice";


                    SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
                    cmd.CommandTimeout = 500;
                    int rows = Convert.ToInt32(cmd.ExecuteScalar());

                    if (rows == 0)
                    {
                        #region Initial AVG Price Query

                        sqlText = @"

--delete from ProductAvgPrice




create table #tempAVGPrice(
ID int identity(1,1),
ItemNo varchar(50),
AgvPriceDate datetime,
PurchaseQty   decimal(25,9),
PurchaseValue   decimal(25,9),
AvgPrice   decimal(25,9),
PurchaseNo varchar(50),
InsertTime datetime2(7),
TransactionType varchar(50)
)


insert into #tempAVGPrice(ItemNo, AgvPriceDate,PurchaseQty,PurchaseValue,AvgPrice,PurchaseNo,InsertTime,TransactionType)
select p.ItemNO, '1900-01-01 00:00:00', 0,0,0,'',GetDate(),'Opening'
from Products p left outer join ProductCategories pc on p.CategoryID = pc.CategoryID
where pc.IsRaw in( 'raw','pack','trading') 
--and ItemNo = '37'



------------------------------------------------------------OPENING-------------------------------------------------------


  declare @count int = (select count(BranchId) from BranchProfiles)

    IF @count = 1
    BEGIN
	    update #tempAVGPrice set PurchaseQty = Products.OpeningBalance, 
	    PurchaseValue=Products.OpeningTotalCost
	    from Products 
	    where #tempAVGPrice.ItemNo = Products.ItemNo and #tempAVGPrice.TransactionType='Opening'

		
    END

    ELSE

    BEGIN


	    create table #tempStocks(id int identity(1,1), ItemNo varchar(50), TotalQty   decimal(25,9), TotalValue   decimal(25,9))

	    insert into #tempStocks (ItemNo, TotalQty,TotalValue)
	    SELECT distinct ItemNo, isnull(sum(p.StockQuantity),0) Quantity, isnull(sum(p.StockValue),0) 
	    from ProductStocks p
	    group by ItemNo

	    update #tempAVGPrice set PurchaseQty = #tempStocks.TotalQty, 
	    PurchaseValue=#tempStocks.TotalValue
	    from #tempStocks 
	    where #tempAVGPrice.ItemNo = #tempStocks.ItemNo  and #tempAVGPrice.TransactionType='Opening'

	    drop table #tempStocks

    END


-----------------------------------------------------Opening-------------------------------------------------------------------




-----------------------------------------------------Purchase------------------------------------------------------------------
-----Check if with SD---------------'
declare @withSD varchar(50) = (select SettingValue from Settings 
where SettingGroup= 'VAT6_1' and 
SettingName= 'TotalIncludeSD'
)

declare @withOtherAMT varchar(50) = (select SettingValue from Settings 
where SettingGroup= 'VAT6_1' and 
SettingName= 'IncludeOtherAMT'
)

--------- 'Other','PurchaseCN','Trading','Service','ServiceNS','CommercialImporter','InputService' -----------------
insert into #tempAVGPrice(ItemNo,AgvPriceDate,PurchaseQty,PurchaseValue,AvgPrice,PurchaseNo,InsertTime,TransactionType)
select ItemNo, ReceiveDate, sum(isnull(UOMQty,0)),
case when @withSD='Y' then sum(SubTotal + SDAmount) else sum(SubTotal) end,
0 AvgPrice,PurchaseInvoiceNo,GetDate(),'Purchase'
from PurchaseInvoiceDetails
where TransactionType  in ('Other','PurchaseCN','Trading','Service','ServiceNS','CommercialImporter','InputService')
and Post = 'Y'
--and ItemNo = '37'
group by ItemNo, ReceiveDate,PurchaseInvoiceNo


--------- RawCTC, WastageCTC -----------------
insert into #tempAVGPrice(ItemNo,AgvPriceDate,PurchaseQty,PurchaseValue,AvgPrice,PurchaseNo,InsertTime,TransactionType)
select ToItemNo, pd.TransferDate, sum(isnull(ToQuantity,0)),sum(isnull(ReceivePrice,0)),
0 AvgPrice,pt.TransferCode,GetDate(),'Purchase'
from ProductTransfersDetails pd left outer join ProductTransfers pt on pd.ProductTransferId = pt.id
where pd.TransactionType  in ('RawCTC')
and pd.Post = 'Y'
--and ItemNo = '37'
group by ToItemNo, pd.TransferDate,pt.TransferCode



--------- 'Import','ServiceImport','ServiceNSImport','TradingImport','InputServiceImport' -----------------
insert into #tempAVGPrice(ItemNo,AgvPriceDate,PurchaseQty,PurchaseValue,AvgPrice,PurchaseNo,InsertTime,TransactionType)
select ItemNo, ReceiveDate, sum(isnull(UOMQty,0)),

case when @withSD='Y' then 
sum(isnull(AssessableValue,0)+ isnull(CDAmount,0)+ isnull(RDAmount,0)+ isnull(TVBAmount,0)+ isnull(TVAAmount,0)
+(case when @withOtherAMT = 'Y' then isnull(OthersAmount,0) else 0 end)
+isnull(SDAmount,0))
else 
sum(isnull(AssessableValue,0)+ isnull(CDAmount,0)+ isnull(RDAmount,0)+ isnull(TVBAmount,0)+ isnull(TVAAmount,0)
+(case when @withOtherAMT = 'Y' then isnull(OthersAmount,0) else 0 end)
)
end,

0 AvgPrice,PurchaseInvoiceNo,GetDate(),'Purchase'
from PurchaseInvoiceDetails
where TransactionType  in ('Import','ServiceImport','ServiceNSImport','TradingImport','InputServiceImport')
and Post = 'Y'
--and ItemNo = '37'
group by ItemNo, ReceiveDate,PurchaseInvoiceNo


--------- 'PurchaseReturn','PurchaseDN' -----------------
insert into #tempAVGPrice(ItemNo,AgvPriceDate,PurchaseQty,PurchaseValue,AvgPrice,PurchaseNo,InsertTime,TransactionType)
select ItemNo, ReceiveDate, sum(-1*isnull(UOMQty,0)),

case when @withSD='Y' then 
sum(-1*(isnull(subtotal,0)+isnull(SDAmount,0)))
else 
sum(-1*(isnull(subtotal,0)))
end,

0 AvgPrice,PurchaseInvoiceNo,GetDate(),'Purchase'
from PurchaseInvoiceDetails
where TransactionType  in ('PurchaseReturn','PurchaseDN')
and Post = 'Y' 
--and ItemNo = '37'
group by ItemNo, ReceiveDate,PurchaseInvoiceNo

-----------------------------------------------------Purchase------------------------------------------------------------------

-----------------------------------------------------Ordering Data by Date------------------------------------------------------------------
insert into ProductAvgPrice(ItemNo,AgvPriceDate,PurchaseQty,PurchaseValue,AvgPrice,PurchaseNo,InsertTime,TransactionType)
select ItemNo,AgvPriceDate,PurchaseQty,PurchaseValue,AvgPrice,PurchaseNo,InsertTime,TransactionType 
from #tempAVGPrice
order by AgvPriceDate

delete from #tempAVGPrice

insert into #tempAVGPrice(ItemNo,AgvPriceDate,PurchaseQty,PurchaseValue,AvgPrice,PurchaseNo,InsertTime,TransactionType)
select ItemNo,AgvPriceDate,PurchaseQty,PurchaseValue,AvgPrice,PurchaseNo,InsertTime,TransactionType 
from ProductAvgPrice
order by AgvPriceDate, SL desc

delete from ProductAvgPrice
-----------------------------------------------------Ordering Data by Date------------------------------------------------------------------

update #tempAVGPrice set AgvPriceDate = FORMAT(AgvPriceDate, 'yyyy-MM-dd 00:00:00')

--select * from #tempAVGPrice
--where ItemNo = '37'
--order by AgvPriceDate


	declare @start int  = (select min(ID) from #tempAvgPrice)
    declare @end int  = (select max(ID) from #tempAvgPrice)

	declare @purchaseQty   decimal(25,9)
	declare @purchaseValue   decimal(25,9)

	declare @purchaseNo varchar(50)
    declare @itemNo varchar(50) = ''
    declare @date datetime 
	declare @transactionType  varchar(50)
	declare @lastPurchasedate datetime
	declare @lastAvgPrice   decimal(25,9)
	declare @currentAvgPrice   decimal(25,9)
	declare @nextPurchaseDate datetime
	declare @lastPurchaseValue   decimal(25,9)
	declare @lastPurchaseQty   decimal(25,9)

	declare @issueQty   decimal(25,9)
	declare @issueValue   decimal(25,9)
	declare @runtimeValue decimal(25,9)
	declare @runtimeQty decimal(25,9)
	declare @lastId int = 0

    while @start <= @end
    begin
	   
	   select
	   @itemNo =ItemNo,
	   @date=AgvPriceDate,
	   @purchaseQty=PurchaseQty,
	   @purchaseValue=PurchaseValue,
	   @purchaseNo = PurchaseNo,
	   @transactionType = transactionType
	   --,@currentAvgPrice = AvgPrice
	   from #tempAvgPrice 
	   where ID = @start 

	   select top 1
	   @nextPurchaseDate = AgvPriceDate
	   from #tempAvgPrice 
	   where ID > @start and AgvPriceDate >= @date and ItemNo = @itemNo
	   order by AgvPriceDate asc, ID asc

	   select top 1 @lastPurchasedate=AgvPriceDate,@lastAvgPrice=AvgPrice,
	   @lastPurchaseValue = RunTimeTotal, @lastPurchaseQty = RunTimeQty, @lastId = SL
	   from ProductAvgPrice
	   where  AgvPriceDate <= @date and ItemNo = @itemNo and (TransactionType = 'Purchase' or TransactionType = 'Opening')
	   order by SL desc,AgvPriceDate desc

	   

	  if @transactionType = 'Purchase'
	  begin 
		

	  insert into ProductAvgPrice(ItemNo,AgvPriceDate, PurchaseQty, PurchaseValue,RuntimeTotal,RuntimeQty, AvgPrice, InsertTime, TransactionType,PurchaseNo,FromDate,ToDate)
	  select @itemNo, @date, @purchaseQty, @purchaseValue,((@purchaseValue+isnull(@lastPurchaseValue,0))),((@purchaseQty+isnull(@lastPurchaseQty,0))),0,	  
	  GETDATE(), 'Purchase',@purchaseNo,@date,isnull(@nextPurchaseDate,'2100-01-01')


	  end
	else
	begin

	  insert into ProductAvgPrice(ItemNo,AgvPriceDate, PurchaseQty, PurchaseValue,RuntimeTotal,RuntimeQty, AvgPrice, InsertTime, TransactionType,PurchaseNo,ToDate,FromDate)
	  select @itemNo, @date, @purchaseQty, @purchaseValue, @purchaseValue,@purchaseQty,0,GETDATE(), 'Opening',@purchaseNo,isnull(@nextPurchaseDate,'2100-01-01'),@date

	end

	   update ProductAvgPrice set AvgPrice = (RuntimeTotal/RuntimeQty)
	   where RuntimeQty != 0

	   update ProductAvgPrice set ToDate = DATEADD(d,-1,@date) where SL = @lastId

	 --  if @itemNo = '37'
	 --  begin
		----select @itemNo,@date, @nextPurchaseDate,@transactionType, @currentAvgPrice
		--select @issueQty
	 --  end


	  set @start = @start + 1

	 set @currentAvgPrice = 0
	 set @issueValue = 0
	 set @issueQty = 0
	 set @purchaseQty = 0
	 set @lastPurchaseValue = 0
	 set @lastPurchaseQty = 0
	 set @nextPurchaseDate = null
	 set @lastId = 0
    end



--select * from ProductAvgPrice
--select * from #tempAVGPrice


drop table #tempAVGPrice


--update IssueDetails set  uom=ProductAvgPrice.AvgPrice
--from ProductAvgPrice
--where ProductAvgPrice.ItemNo=IssueDetails.ItemNo
--and IssueDetails.IssueDateTime >= ProductAvgPrice.FromDate 
--and IssueDetails.IssueDateTime< DATEADD(d,1, ProductAvgPrice.ToDate) 
--and IssueDetails.ItemNo=ProductAvgPrice.ItemNo


";

                        #endregion

                        cmd.CommandText = sqlText;
                        cmd.ExecuteNonQuery();

                    }
                    else
                    {

                    }

                }

                rVM.Status = "Success";
                rVM.Message = "Day End Process Completed Successfully!";

                #endregion

                #region Transaction Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion
            }

            #endregion

            #region Catch and Finally

            catch (Exception ex)
            {

                rVM = new ResultVM();
                rVM.Message = ex.Message;

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Rollback();
                }

                ////FileLogger.Log("IssueDAL", "Issue Multiple Save", ex.Message + ex.StackTrace);
                FileLogger.Log("IssueDAL", "MultipleUpdate", ex.ToString());

                throw ex;
            }

            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }

            #endregion

            return rVM;
        }

        public ResultVM RefreshAvgPriceProcess(IssueMasterVM paramVM, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Declarations

            ResultVM rVM = new ResultVM();
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            CommonDAL commonDAl = new CommonDAL();
            #endregion

            try
            {
                #region open connection and transaction
                #region open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                #endregion open connection and transaction

                ////commonDAl.ConnectionTransactionOpen(ref VcurrConn, ref Vtransaction, ref currConn, ref transaction,connVM);

                #endregion open connection and transaction


                string sqlText = "delete from ProductAvgPrice";

                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
                cmd.ExecuteNonQuery();

                rVM = MultipleUpdate(paramVM, currConn, transaction, connVM);


                #region Transaction Commit
                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion
                ////commonDAl.TransactionCommit(ref Vtransaction, ref transaction);

                #endregion

            }
            #region Catch and Finally

            catch (Exception ex)
            {

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Rollback();
                }
                FileLogger.Log("IssueDAL", "RefreshAvgPriceProcess", ex.ToString());

                rVM = new ResultVM();
                rVM.Message = ex.Message;

                commonDAl.TransactionRollBack(ref Vtransaction, ref transaction);

                throw ex;
            }

            finally
            {
                if (VcurrConn == null)
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
            ////finally
            ////{
            ////    commonDAl.CloseConnection(ref VcurrConn, ref currConn);
            ////}

            #endregion

            return rVM;

        }

        public ResultVM UpdateAvgPrice_New_Refresh(AVGPriceVm paramVM, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Declarations

            ResultVM rVM = new ResultVM();
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            #endregion

            #region Try Statement

            try
            {
                #region open connection and transaction

                #region New open connection and transaction

                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                #endregion New open connection and transaction

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction(DateTime.Now.ToString("ddMMyyyHH:mm:ss"));
                }

                #endregion open connection and transaction
                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);

                  sqlText = @"delete from ProductAvgPrice where 1=1 ";

                if (!string.IsNullOrEmpty(paramVM.ItemNo))
                {
                    sqlText += " and ItemNo = @ItemNo";
                }

                if (!string.IsNullOrEmpty(paramVM.AvgDateTime))
                {
                    sqlText += " and AgvPriceDate >= @AgvPriceDate";
                }

                cmd = new SqlCommand(sqlText, currConn, transaction);

                if (!string.IsNullOrEmpty(paramVM.ItemNo))
                {
                    cmd.Parameters.AddWithValue("@ItemNo", paramVM.ItemNo);
                }

                if (!string.IsNullOrEmpty(paramVM.AvgDateTime))
                {
                    cmd.Parameters.AddWithValue("@AgvPriceDate", paramVM.AvgDateTime);
                }
                cmd.ExecuteNonQuery();


                //rVM = UpdateAvgPrice_New(paramVM, currConn, transaction);
                rVM = UpdateAvgPrice_New_Partition(paramVM, currConn, transaction, connVM);

                #region Transaction Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion
            }

            #endregion

            #region Catch and Finally

            catch (Exception ex)
            {

                rVM = new ResultVM();
                rVM.Message = ex.Message;

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Rollback();
                }

                ////FileLogger.Log("IssueDAL", "Issue Multiple Save", ex.Message + ex.StackTrace);
                FileLogger.Log("IssueDAL", "MultipleUpdate", ex.ToString());

                throw ex;
            }

            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }

            #endregion

            return rVM;
        }

        public ResultVM UpdateAvgPrice_New_Refresh_Old(AVGPriceVm paramVM, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Declarations

            ResultVM rVM = new ResultVM();
            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            #endregion

            #region Try Statement

            try
            {
                #region open connection and transaction

                #region New open connection and transaction

                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                #endregion New open connection and transaction

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction(DateTime.Now.ToString("ddMMyyyHH:mm:ss"));
                }

                #endregion open connection and transaction

                string sqlText = @"delete from ProductAvgPrice where 1=1 ";

                if (!string.IsNullOrEmpty(paramVM.ItemNo))
                {
                    sqlText += " and ItemNo = @ItemNo";
                }


                if (!string.IsNullOrEmpty(paramVM.AvgDateTime))
                {
                    sqlText += " and AgvPriceDate >= @AgvPriceDate";

                }

                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);

                if (!string.IsNullOrEmpty(paramVM.ItemNo))
                {
                    cmd.Parameters.AddWithValue("@ItemNo", paramVM.ItemNo);
                }

                if (!string.IsNullOrEmpty(paramVM.AvgDateTime))
                {
                    cmd.Parameters.AddWithValue("@AgvPriceDate", paramVM.AvgDateTime);
                }
                cmd.ExecuteNonQuery();


                rVM = UpdateAvgPrice_New(paramVM, currConn, transaction);
                //rVM = UpdateAvgPrice_New_Partition(paramVM, currConn, transaction);

                #region Transaction Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion
            }

            #endregion

            #region Catch and Finally

            catch (Exception ex)
            {

                rVM = new ResultVM();
                rVM.Message = ex.Message;

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Rollback();
                }

                ////FileLogger.Log("IssueDAL", "Issue Multiple Save", ex.Message + ex.StackTrace);
                FileLogger.Log("IssueDAL", "MultipleUpdate", ex.ToString());

                throw ex;
            }

            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }

            #endregion

            return rVM;
        }



        private int UpdateAveragePrice(SqlCommand cmd)
        {
            int rows;
            string sqlText = @"

CREATE TABLE #tempAvgPrice(
	[SL] [int] IDENTITY(1,1) NOT NULL,
	[ItemNo] [varchar](50) NOT NULL,
	[AgvPriceDate] [datetime] NOT NULL,
	[PurchaseQty] [decimal](25, 9) NULL,
	[PurchaseValue] [decimal](25, 9) NULL,
	[RuntimeQty] [decimal](25, 9) NULL,
	[RuntimeTotal] [decimal](25, 9) NULL,
	[AvgPrice] [decimal](25, 9) NOT NULL,
	[PurchaseNo] [varchar](50) NULL,
	[InsertTime] [datetime2](7) NULL,
	[RefNo] int NULL,

)

insert into #tempAvgPrice(
      ItemNo
      ,AgvPriceDate
      ,PurchaseQty
      ,PurchaseValue
      ,RuntimeQty
      ,RuntimeTotal
      ,AvgPrice
      ,PurchaseNo
      ,InsertTime,RefNo)
	  select ItemNo
      ,AgvPriceDate
      ,PurchaseQty
      ,PurchaseValue
      ,RuntimeQty
      ,RuntimeTotal
      ,AvgPrice
      ,PurchaseNo
      ,InsertTime 
      ,SL 
from ProductAvgPrice
	  order by AgvPriceDate,SL,ItemNo

    declare @start int  = (select min(SL) from #tempAvgPrice where AvgPrice = 0)
    declare @end int  = (select max(SL) from #tempAvgPrice where AvgPrice = 0)

    declare @previousValue int  = 0
    declare @previousQty int  = 0
    declare @itemNo varchar(50) = ''
    declare @date datetime 

    while @start <= @end
    begin
	    
	    select @itemNo =ItemNo,@date=AgvPriceDate from #tempAvgPrice where SL = @start and AvgPrice = 0

	    select top 1 @previousQty=RuntimeQty,@previousValue=RuntimeTotal
	    from #tempAvgPrice 
	    where ItemNo = @itemNo and SL < @start and AgvPriceDate <=@date
	    order by SL desc

	    --select @previousQty,@previousValue,@itemNo

	    update #tempAvgPrice set RuntimeQty = PurchaseQty + @previousQty, 
	    RuntimeTotal = PurchaseValue + @previousValue 
	    where SL = @start and AvgPrice = 0

	    set @start = @start + 1

    end

	update ProductAvgPrice set RuntimeTotal = #tempAvgPrice.RuntimeTotal, RuntimeQty = #tempAvgPrice.RuntimeQty
	from #tempAvgPrice 
	where ProductAvgPrice.SL = #tempAvgPrice.RefNo 

    update ProductAvgPrice set AvgPrice = RuntimeTotal/RuntimeQty
    where  AvgPrice = 0 and RuntimeQty > 0

	drop table #tempAvgPrice

--#tempAvgPrice.AgvPriceDate and ProductAvgPrice.ItemNo = #tempAvgPrice.ItemNo
    ";

            cmd.CommandText = sqlText;

            rows = cmd.ExecuteNonQuery();
            return rows;
        }

        private void IssueDayEndProcess(IssueMasterVM paramVM, SqlConnection currConn, SqlTransaction transaction, SysDBInfoVMTemp connVM = null)
        {

            string sqlText = @"
            select CEILING(count(Id)/10000.00) from IssueDetails where IsDayEnd = 'Y' ";


            SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
            cmd.CommandTimeout = 800;

            int count = Convert.ToInt32(cmd.ExecuteScalar());



            sqlText = @"
--declare @issueFromDate datetime = '2019-07-01 00:00:00'
--declare @issueToDate datetime = '2020-12-06 23:59:59'


create table #tempIssue(
Id int identity(1,1),
IssueNo varchar(50),
IssueDateTime datetime,
ItemNo varchar(50),
CostPrice decimal(25,9)
)



insert into #tempIssue(IssueNo,ItemNo,IssueDateTime,CostPrice)
select top 10000 IssueNo,ItemNo,IssueDateTime,0 from IssueDetails
where IsDayEnd = 'Y'
--and IssueDateTime >= @issueFromDate
--and IssueDateTime <= @issueToDate


declare @startId int  = (select min(id) from #tempIssue)
declare @endId int  = (select max(id) from #tempIssue)

declare @itemNo varchar(50)
declare @issueDate datetime
declare @costPrice decimal(25,9)



while @startId <= @endId
begin
		
	select @itemNo = ItemNo,@issueDate=IssueDateTime 
	from #tempIssue
	where Id = @startId 


	select top 1 @costPrice = AvgPrice from ProductAvgPrice
	where ItemNo = @itemNo and    AgvPriceDate <= @issueDate
	order by AgvPriceDate desc

	update #tempIssue set CostPrice = @costPrice
	where Id = @startId


	set @startId = @startId+1
end


update IssueDetails set CostPrice = #tempIssue.CostPrice * IssueDetails.UOMc, SubTotal = (#tempIssue.CostPrice * IssueDetails.UOMc)*Quantity,
UOMPrice = #tempIssue.CostPrice
,IsDayEnd = 'N'
from #tempIssue
where IssueDetails.IssueNo = #tempIssue.IssueNo and IssueDetails.ItemNo = #tempIssue.ItemNo





drop table #tempIssue


";

            cmd.CommandText = sqlText;

            cmd.Parameters.AddWithValue("@issueFromDate", paramVM.IssueDateTimeFrom);
            cmd.Parameters.AddWithValue("@issueToDate", paramVM.IssueDateTimeTo);

            for (int i = 0; i < count; i++)
            {
                cmd.ExecuteNonQuery();
            }

        }

        #endregion

        #region Not Used / Jul-2020

        //currConn to VcurrConn 25-Aug-2020
        public string[] IssueInsert1(IssueMasterVM Master, SqlTransaction Vtransaction, SqlConnection VcurrConn, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";
            #region Check user from settings
            //SettingDAL settingDal=new SettingDAL();
            //   bool isAllowUser = settingDal.CheckUserAccess();
            //   if (!isAllowUser)
            //   {
            //       throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, MessageVM.msgAccessPermision);
            //   }
            #endregion
            //SqlConnection currConn = null;
            //SqlTransaction transaction = null;
            SqlConnection vcurrConn = VcurrConn;
            if (vcurrConn == null)
            {
                VcurrConn = null;
                Vtransaction = null;
            }
            int transResult = 0;
            string sqlText = "";

            string newID = "";
            string PostStatus = "";

            int IDExist = 0;
            int nextId = 0;

            #endregion Initializ

            #region Try
            try
            {
                SetDefaultValue(Master);
                #region Validation for Header


                if (Master == null)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, MessageVM.issueMsgNoDataToSave);
                }
                else if (Convert.ToDateTime(Master.IssueDateTime) < DateTime.MinValue || Convert.ToDateTime(Master.IssueDateTime) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, "Please Check Issue Data and Time");

                }


                #endregion Validation for Header
                #region open connection and transaction
                if (vcurrConn == null)
                {
                    VcurrConn = _dbsqlConnection.GetConnection(connVM);
                    if (VcurrConn.State != ConnectionState.Open)
                    {
                        VcurrConn.Open();
                    }

                    Vtransaction = VcurrConn.BeginTransaction(MessageVM.issueMsgMethodNameInsert);
                }


                #endregion open connection and transaction
                #region Fiscal Year Check

                string transactionDate = Master.IssueDateTime;
                string transactionYearCheck = Convert.ToDateTime(Master.IssueDateTime).ToString("yyyy-MM-dd");
                if (Convert.ToDateTime(transactionYearCheck) > DateTime.MinValue || Convert.ToDateTime(transactionYearCheck) < DateTime.MaxValue)
                {

                    #region YearLock
                    sqlText = "";

                    sqlText += "select distinct isnull(PeriodLock,'Y') MLock,isnull(GLLock,'Y')YLock from fiscalyear " +
                                                   " where '" + transactionYearCheck + "' between PeriodStart and PeriodEnd";

                    DataTable dataTable = new DataTable("ProductDataT");
                    SqlCommand cmdIdExist = new SqlCommand(sqlText, VcurrConn);
                    cmdIdExist.Transaction = Vtransaction;
                    SqlDataAdapter reportDataAdapt = new SqlDataAdapter(cmdIdExist);
                    reportDataAdapt.Fill(dataTable);

                    if (dataTable == null)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                    }

                    else if (dataTable.Rows.Count <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                    }
                    else
                    {
                        if (dataTable.Rows[0]["MLock"].ToString() != "N")
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                        }
                        else if (dataTable.Rows[0]["YLock"].ToString() != "N")
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                        }
                    }
                    #endregion YearLock
                    #region YearNotExist
                    sqlText = "";
                    sqlText = sqlText + "select  min(PeriodStart) MinDate, max(PeriodEnd)  MaxDate from fiscalyear";

                    DataTable dtYearNotExist = new DataTable("ProductDataT");

                    SqlCommand cmdYearNotExist = new SqlCommand(sqlText, VcurrConn);
                    cmdYearNotExist.Transaction = Vtransaction;
                    //countId = (int)cmdIdExist.ExecuteScalar();

                    SqlDataAdapter YearNotExistDataAdapt = new SqlDataAdapter(cmdYearNotExist);
                    YearNotExistDataAdapt.Fill(dtYearNotExist);

                    if (dtYearNotExist == null)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearNotExist);
                    }

                    else if (dtYearNotExist.Rows.Count < 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearNotExist);
                    }
                    else
                    {
                        if (Convert.ToDateTime(transactionYearCheck) < Convert.ToDateTime(dtYearNotExist.Rows[0]["MinDate"].ToString())
                            || Convert.ToDateTime(transactionYearCheck) > Convert.ToDateTime(dtYearNotExist.Rows[0]["MaxDate"].ToString()))
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearNotExist);
                        }
                    }
                    #endregion YearNotExist

                }


                #endregion Fiscal Year CHECK
                #region Find Transaction Exist

                sqlText = "";
                sqlText = sqlText + "select COUNT(IssueNo) from IssueHeaders WHERE IssueNo=@MasterIssueNo ";
                SqlCommand cmdExistTran = new SqlCommand(sqlText, VcurrConn);
                cmdExistTran.Transaction = Vtransaction;

                cmdExistTran.Parameters.AddWithValue("@MasterIssueNo", Master.IssueNo ?? Convert.DBNull);

                IDExist = (int)cmdExistTran.ExecuteScalar();

                if (IDExist > 0)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, MessageVM.issueMsgFindExistID);
                }

                #endregion Find Transaction Exist
                #region Purchase ID Create
                if (string.IsNullOrEmpty(Master.transactionType)) //start
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, MessageVM.msgTransactionNotDeclared);
                }
                #region Purchase ID Create For Other

                CommonDAL commonDal = new CommonDAL();

                if (Master.transactionType == "Other")
                {
                    newID = commonDal.TransactionCode("Issue", "Other", "IssueHeaders", "IssueNo",
                                              "IssueDateTime", Master.IssueDateTime, Master.BranchId.ToString(), VcurrConn, Vtransaction, connVM);


                }
                if (Master.transactionType == "IssueReturn")
                {
                    newID = commonDal.TransactionCode("Issue", "IssueReturn", "IssueHeaders", "IssueNo",
                                              "IssueDateTime", Master.IssueDateTime, Master.BranchId.ToString(), VcurrConn, Vtransaction, connVM);


                }

                if (Master.transactionType == "IssueWastage")
                {
                    newID = commonDal.TransactionCode("Issue", "IssueWastage", "IssueHeaders", "IssueNo",
                                              "IssueDateTime", Master.IssueDateTime, Master.BranchId.ToString(), VcurrConn, Vtransaction, connVM);


                }

                #endregion Purchase ID Create For Other



                #endregion Purchase ID Create Not Complete
                #region ID generated completed,Insert new Information in Header


                sqlText = "";
                sqlText += " insert into IssueHeaders(";
                //////sqlText += " Id,";
                sqlText += " IssueNo,";
                sqlText += " IssueDateTime,";
                sqlText += " TotalVATAmount,";
                sqlText += " TotalAmount,";
                sqlText += " SerialNo,";
                sqlText += " Comments,";
                sqlText += " CreatedBy,";
                sqlText += " CreatedOn,";
                sqlText += " ReceiveNo,";
                sqlText += " transactionType,";
                sqlText += " IssueReturnId,";
                sqlText += " ImportIDExcel,";

                sqlText += " Post";
                sqlText += " )";

                sqlText += " values";
                sqlText += " (";
                sqlText += "@newID,";
                sqlText += "@MasterIssueDateTime,";
                sqlText += "@MasterTotalVATAmount,";
                sqlText += "@MasterTotalAmount,";
                sqlText += "@MasterSerialNo,";
                sqlText += "@MasterComments,";
                sqlText += "@MasterCreatedBy,";
                sqlText += "@MasterCreatedOn,";
                sqlText += "@MasterReceiveNo,";
                sqlText += "@MastertransactionType,";
                sqlText += "@MasterReturnId,";
                sqlText += "@MasterImportId,";
                sqlText += "@MasterPost";
                sqlText += ") SELECT SCOPE_IDENTITY() ";

                var Id = _cDal.NextId("IssueHeaders", VcurrConn, Vtransaction, connVM).ToString();

                SqlCommand cmdInsert = new SqlCommand(sqlText, VcurrConn);
                cmdInsert.Transaction = Vtransaction;
                //////cmdInsert.Parameters.AddWithValue("@Id", Id);
                cmdInsert.Parameters.AddWithValue("@newID", newID);
                cmdInsert.Parameters.AddWithValue("@MasterIssueDateTime", Master.IssueDateTime);
                cmdInsert.Parameters.AddWithValue("@MasterTotalVATAmount", Master.TotalVATAmount);
                cmdInsert.Parameters.AddWithValue("@MasterTotalAmount", Master.TotalAmount);
                cmdInsert.Parameters.AddWithValue("@MasterSerialNo", Master.SerialNo ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterComments", Master.Comments ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterCreatedBy", Master.CreatedBy ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterCreatedOn", OrdinaryVATDesktop.DateToDate(Master.CreatedOn));
                cmdInsert.Parameters.AddWithValue("@MasterReceiveNo", Master.ReceiveNo ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MastertransactionType", Master.transactionType ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterReturnId", Master.ReturnId ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterImportId", Master.ImportId ?? Convert.DBNull);
                cmdInsert.Parameters.AddWithValue("@MasterPost", "N");


                var exec = cmdInsert.ExecuteScalar();

                transResult = Convert.ToInt32(exec);
                Master.Id = transResult.ToString();

                //transResult = (int)cmdInsert.ExecuteNonQuery();
                //Master.Id = Id.ToString();

                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, MessageVM.issueMsgSaveNotSuccessfully);
                }


                #endregion ID generated completed,Insert new Information in Header

                #region Insert into Details(Insert complete in Header)
                #region Validation for Detail

                if (Master.Details.Count() < 0)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, MessageVM.issueMsgNoDataToSave);
                }


                #endregion Validation for Detail

                #region Insert Detail Table
                var lineNo = 1;
                foreach (var Item in Master.Details)
                {
                    Item.IssueLineNo = lineNo.ToString();
                    lineNo++;
                    #region Find Transaction Exist

                    sqlText = "";
                    sqlText += "select COUNT(IssueNo) from IssueDetails WHERE IssueNo='" + newID + "' ";
                    sqlText += " AND ItemNo=@ItemItemNo";
                    SqlCommand cmdFindId = new SqlCommand(sqlText, VcurrConn);
                    cmdFindId.Transaction = Vtransaction;

                    cmdFindId.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo);

                    IDExist = (int)cmdFindId.ExecuteScalar();

                    if (IDExist > 0)
                    {
                        throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, MessageVM.issueMsgFindExistID);
                    }

                    #endregion Find Transaction Exist

                    #region Insert only DetailTable

                    sqlText = "";
                    sqlText += " insert into IssueDetails(";
                    //sqlText += " IssueNo,";
                    sqlText += " IssueNo,";
                    sqlText += " IssueLineNo,";
                    sqlText += " ItemNo,";
                    sqlText += " Quantity,";
                    sqlText += " NBRPrice,";
                    sqlText += " CostPrice,";
                    sqlText += " UOM,";
                    sqlText += " VATRate,";
                    sqlText += " VATAmount,";
                    sqlText += " SubTotal,";
                    sqlText += " Comments,";
                    sqlText += " CreatedBy,";
                    sqlText += " CreatedOn,";
                    sqlText += " ReceiveNo,";
                    sqlText += " IssueDateTime,";
                    sqlText += " SD,";
                    sqlText += " SDAmount,";
                    sqlText += " Wastage,";
                    sqlText += " BOMDate,";
                    sqlText += " FinishItemNo,";
                    sqlText += " transactionType,";
                    sqlText += " IssueReturnId,";
                    sqlText += " UOMQty,";
                    sqlText += " UOMPrice,";
                    sqlText += " UOMc,";
                    sqlText += " UOMn,";
                    sqlText += " Post";
                    sqlText += " )";

                    sqlText += " values(	";
                    //sqlText += "'" + Master.Id + "',";

                    sqlText += "@newID,";
                    sqlText += "@ItemIssueLineNo,";
                    sqlText += "@ItemItemNo,";
                    sqlText += "@ItemQuantity,";
                    sqlText += " 0,";
                    sqlText += "@ItemCostPrice,";
                    sqlText += "@ItemUOM,";
                    sqlText += "@ItemVATRate,";
                    sqlText += "@ItemVATAmount,";
                    sqlText += "@ItemSubTotal,";
                    sqlText += "@ItemCommentsD,";
                    sqlText += "@MasterCreatedBy,";
                    sqlText += "@MasterCreatedOn,";
                    sqlText += "@ItemReceiveNoD,";
                    sqlText += "@ItemIssueDateTimeD,";
                    sqlText += " 0,	";
                    sqlText += " 0,";
                    sqlText += "@ItemWastage,";
                    sqlText += "@ItemBOMDate,";
                    sqlText += "@ItemFinishItemNo,";
                    sqlText += "@MastertransactionType,";
                    sqlText += "@MasterReturnId,";
                    sqlText += "@ItemUOMQty,";
                    sqlText += "@ItemUOMPrice,";
                    sqlText += "@ItemUOMc,";
                    sqlText += "@ItemUOMn,";
                    sqlText += "@MasterPost";
                    sqlText += ")	";


                    SqlCommand cmdInsDetail = new SqlCommand(sqlText, VcurrConn);
                    cmdInsDetail.Transaction = Vtransaction;
                    cmdInsDetail.Parameters.AddWithValue("@newID", newID);
                    cmdInsDetail.Parameters.AddWithValue("@ItemIssueLineNo", Item.IssueLineNo ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@ItemQuantity", Item.Quantity);
                    cmdInsDetail.Parameters.AddWithValue("@ItemCostPrice", Item.CostPrice);
                    cmdInsDetail.Parameters.AddWithValue("@ItemUOM", Item.UOM ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@ItemVATRate", Item.VATRate);
                    cmdInsDetail.Parameters.AddWithValue("@ItemVATAmount", Item.VATAmount);
                    cmdInsDetail.Parameters.AddWithValue("@ItemSubTotal", Item.SubTotal);
                    cmdInsDetail.Parameters.AddWithValue("@ItemCommentsD", Item.CommentsD ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@MasterCreatedBy", Master.CreatedBy ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@MasterCreatedOn", OrdinaryVATDesktop.DateToDate(Master.CreatedOn));
                    cmdInsDetail.Parameters.AddWithValue("@ItemReceiveNoD", Item.ReceiveNoD ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@ItemIssueDateTimeD", OrdinaryVATDesktop.DateToDate(Item.IssueDateTimeD));
                    cmdInsDetail.Parameters.AddWithValue("@ItemWastage", Item.Wastage);
                    cmdInsDetail.Parameters.AddWithValue("@ItemBOMDate", Item.BOMDate ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@ItemFinishItemNo", Item.FinishItemNo ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@MastertransactionType", Master.transactionType ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@MasterReturnId", Master.ReturnId ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValue("@ItemUOMQty", Item.UOMQty);
                    cmdInsDetail.Parameters.AddWithValue("@ItemUOMPrice", Item.UOMPrice);
                    cmdInsDetail.Parameters.AddWithValue("@ItemUOMc", Item.UOMc);
                    cmdInsDetail.Parameters.AddWithValue("@ItemUOMn", Item.UOMn);
                    cmdInsDetail.Parameters.AddWithValue("@MasterPost", "N");

                    transResult = (int)cmdInsDetail.ExecuteNonQuery();

                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, MessageVM.issueMsgSaveNotSuccessfully);
                    }
                    #endregion Insert only DetailTable


                }


                #endregion Insert Detail Table
                #endregion Insert into Details(Insert complete in Header)
                #region return Current ID and Post Status

                sqlText = "";
                sqlText = sqlText + "select distinct  Post from dbo.IssueHeaders WHERE IssueNo='" + newID + "'";
                SqlCommand cmdIPS = new SqlCommand(sqlText, VcurrConn);
                cmdIPS.Transaction = Vtransaction;
                PostStatus = (string)cmdIPS.ExecuteScalar();
                if (string.IsNullOrEmpty(PostStatus))
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, MessageVM.issueMsgUnableCreatID);
                }


                #endregion Prefetch
                #region Commit

                if (vcurrConn == null)
                {
                    if (Vtransaction != null)
                    {
                        if (transResult > 0)
                        {
                            Vtransaction.Commit();
                        }
                    }
                }

                #endregion Commit
                #region SuccessResult

                retResults[0] = "Success";
                retResults[1] = MessageVM.issueMsgSaveSuccessfully;
                retResults[2] = "" + Master.Id;
                retResults[3] = "" + PostStatus;
                #endregion SuccessResult

            }
            #endregion Try

            #region Catch and Finall
            #region Catch
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[1] = ex.Message.Split(new[] { '\r', '\n' }).FirstOrDefault(); //catch ex
                retResults[2] = nextId.ToString(); //catch ex

                if (Vtransaction != null) { Vtransaction.Rollback(); }
                FileLogger.Log("IssueDAL", "IssueInsert1", ex.ToString() + "\n" + sqlText);

                ////FileLogger.Log(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + sqlText);
                return retResults;
            }
            #endregion

            finally
            {
                if (vcurrConn == null)
                {
                    if (VcurrConn != null)
                    {
                        if (VcurrConn.State == ConnectionState.Open)
                        {
                            VcurrConn.Close();
                        }
                    }
                }

            }
            #endregion Catch and Finall

            #region Result
            return retResults;
            #endregion Result

        }

        public string[] IssueUpdate1(IssueMasterVM Master, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            string sqlText = "";
            //DateTime MinDate = DateTime.MinValue;
            //DateTime MaxDate = DateTime.MaxValue;
            string PostStatus = "";
            int nextId = 0;

            #endregion Initializ

            #region Try
            try
            {
                SetDefaultValue(Master);

                #region Validation for Header

                if (Master == null)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, MessageVM.issueMsgNoDataToUpdate);
                }
                else if (Convert.ToDateTime(Master.IssueDateTime) < DateTime.MinValue || Convert.ToDateTime(Master.IssueDateTime) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, "Please Check Invoice Data and Time");

                }



                #endregion Validation for Header
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                #region Add BOMId
                CommonDAL commonDal = new CommonDAL();
                //commonDal.TableFieldAdd("IssueDetails", "BOMId", "varchar(20)", currConn); //tablename,fieldName, datatype
                //commonDal.TableFieldAdd("IssueDetails", "UOMQty", "decimal(25, 9)", currConn); //tablename,fieldName, datatype
                //commonDal.TableFieldAdd("IssueDetails", "UOMPrice", "decimal(25, 9)", currConn); //tablename,fieldName, datatype
                //commonDal.TableFieldAdd("IssueDetails", "UOMc", "decimal(25, 9)", currConn); //tablename,fieldName, datatype
                //commonDal.TableFieldAdd("IssueDetails", "UOMn", "varchar(50)", currConn); //tablename,fieldName, datatype
                //commonDal.TableFieldAdd("IssueDetails", "UOMWastage", "decimal(25, 9)", currConn); //tablename,fieldName, datatype

                #endregion Add BOMId
                transaction = currConn.BeginTransaction(MessageVM.issueMsgMethodNameUpdate);

                #endregion open connection and transaction
                #region Fiscal Year Check

                string transactionDate = Master.IssueDateTime;
                string transactionYearCheck = Convert.ToDateTime(Master.IssueDateTime).ToString("yyyy-MM-dd");
                if (Convert.ToDateTime(transactionYearCheck) > DateTime.MinValue || Convert.ToDateTime(transactionYearCheck) < DateTime.MaxValue)
                {

                    #region YearLock
                    sqlText = "";

                    sqlText += "select distinct isnull(PeriodLock,'Y') MLock,isnull(GLLock,'Y')YLock from fiscalyear " +
                                                   " where '" + transactionYearCheck + "' between PeriodStart and PeriodEnd";

                    DataTable dataTable = new DataTable("ProductDataT");
                    SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                    cmdIdExist.Transaction = transaction;
                    SqlDataAdapter reportDataAdapt = new SqlDataAdapter(cmdIdExist);
                    reportDataAdapt.Fill(dataTable);

                    if (dataTable == null)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                    }

                    else if (dataTable.Rows.Count <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                    }
                    else
                    {
                        if (dataTable.Rows[0]["MLock"].ToString() != "N")
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                        }
                        else if (dataTable.Rows[0]["YLock"].ToString() != "N")
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                        }
                    }
                    #endregion YearLock
                    #region YearNotExist
                    sqlText = "";
                    sqlText = sqlText + "select  min(PeriodStart) MinDate, max(PeriodEnd)  MaxDate from fiscalyear";

                    DataTable dtYearNotExist = new DataTable("ProductDataT");

                    SqlCommand cmdYearNotExist = new SqlCommand(sqlText, currConn);
                    cmdYearNotExist.Transaction = transaction;
                    //countId = (int)cmdIdExist.ExecuteScalar();

                    SqlDataAdapter YearNotExistDataAdapt = new SqlDataAdapter(cmdYearNotExist);
                    YearNotExistDataAdapt.Fill(dtYearNotExist);

                    if (dtYearNotExist == null)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearNotExist);
                    }

                    else if (dtYearNotExist.Rows.Count < 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearNotExist);
                    }
                    else
                    {
                        if (Convert.ToDateTime(transactionYearCheck) < Convert.ToDateTime(dtYearNotExist.Rows[0]["MinDate"].ToString())
                            || Convert.ToDateTime(transactionYearCheck) > Convert.ToDateTime(dtYearNotExist.Rows[0]["MaxDate"].ToString()))
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearNotExist);
                        }
                    }
                    #endregion YearNotExist

                }


                #endregion Fiscal Year CHECK
                #region Find ID for Update

                sqlText = "";
                sqlText = sqlText + "select COUNT(IssueNo) from IssueHeaders WHERE IssueNo=@MasterIssueNo ";
                SqlCommand cmdFindIdUpd = new SqlCommand(sqlText, currConn);
                cmdFindIdUpd.Transaction = transaction;

                cmdFindIdUpd.Parameters.AddWithValue("@MasterIssueNo", Master.IssueNo);

                int IDExist = (int)cmdFindIdUpd.ExecuteScalar();

                if (IDExist <= 0)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, MessageVM.issueMsgUnableFindExistID);
                }

                #endregion Find ID for Update



                #region ID check completed,update Information in Header

                #region update Header
                sqlText = "";

                sqlText += " update IssueHeaders set  ";

                sqlText += " IssueDateTime  =@MasterIssueDateTime ,";
                sqlText += " TotalVATAmount =@MasterTotalVATAmount ,";
                sqlText += " TotalAmount    =@MasterTotalAmount ,";
                sqlText += " SerialNo       =@MasterSerialNo ,";
                sqlText += " Comments       =@MasterComments ,";
                sqlText += " LastModifiedBy =@MasterLastModifiedBy ,";
                sqlText += " LastModifiedOn =@MasterLastModifiedOn ,";
                sqlText += " ReceiveNo      =@MasterReceiveNo ,";
                sqlText += " transactionType=@MastertransactionType ,";
                sqlText += " IssueReturnId  =@MasterReturnId ";
                sqlText += " where  IssueNo =@MasterIssueNo ";


                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;

                cmdUpdate.Parameters.AddWithValue("@MasterIssueDateTime", Master.IssueDateTime);
                cmdUpdate.Parameters.AddWithValue("@MasterTotalVATAmount", Master.TotalVATAmount);
                cmdUpdate.Parameters.AddWithValue("@MasterTotalAmount", Master.TotalAmount);
                cmdUpdate.Parameters.AddWithValue("@MasterSerialNo", Master.SerialNo ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterComments", Master.Comments ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn);
                cmdUpdate.Parameters.AddWithValue("@MasterReceiveNo", Master.ReceiveNo ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MastertransactionType", Master.transactionType ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterReturnId", Master.ReturnId ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterIssueNo", Master.IssueNo ?? Convert.DBNull);

                transResult = (int)cmdUpdate.ExecuteNonQuery();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, MessageVM.issueMsgUpdateNotSuccessfully);
                }
                #endregion update Header



                #endregion ID check completed,update Information in Header

                #region Update into Details(Update complete in Header)
                #region Validation for Detail

                if (Master.Details.Count() < 0)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, MessageVM.issueMsgNoDataToUpdate);
                }


                #endregion Validation for Detail

                #region Update Detail Table
                var lineNo = 1;
                foreach (var Item in Master.Details)
                {
                    #region Find Transaction Mode Insert or Update
                    Item.IssueLineNo = lineNo.ToString();
                    lineNo++;
                    sqlText = "";
                    sqlText += "select COUNT(IssueNo) from IssueDetails WHERE IssueNo=@MasterIssueNo ";
                    sqlText += " AND ItemNo=@ItemItemNo";
                    SqlCommand cmdFindId = new SqlCommand(sqlText, currConn);
                    cmdFindId.Transaction = transaction;

                    cmdFindId.Parameters.AddWithValue("@MasterIssueNo", Master.IssueNo);
                    cmdFindId.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo);

                    IDExist = (int)cmdFindId.ExecuteScalar();

                    if (IDExist <= 0)
                    {
                        // Insert
                        #region Insert only DetailTable

                        sqlText = "";
                        sqlText += " insert into IssueDetails(";

                        sqlText += " IssueNo,";
                        sqlText += " IssueLineNo,";
                        sqlText += " ItemNo,";
                        sqlText += " Quantity,";
                        sqlText += " NBRPrice,";
                        sqlText += " CostPrice,";
                        sqlText += " UOM,";
                        sqlText += " VATRate,";
                        sqlText += " VATAmount,";
                        sqlText += " SubTotal,";
                        sqlText += " Comments,";
                        sqlText += " CreatedBy,";
                        sqlText += " CreatedOn,";
                        sqlText += " LastModifiedBy,";
                        sqlText += " LastModifiedOn,";
                        sqlText += " ReceiveNo,";
                        sqlText += " IssueDateTime,";
                        sqlText += " SD,";
                        sqlText += " SDAmount,";
                        sqlText += " Wastage,";
                        sqlText += " BOMDate,";
                        sqlText += " FinishItemNo,";
                        sqlText += " transactionType,";
                        sqlText += " IssueReturnId,";
                        sqlText += " UOMQty,";
                        sqlText += " UOMPrice,";
                        sqlText += " UOMc,";
                        sqlText += " UOMn,";
                        sqlText += " Post";
                        sqlText += " )";
                        sqlText += " values(	";
                        //sqlText += "'" + Master.Id + "',";

                        sqlText += "@MasterIssueNo,";
                        sqlText += "@ItemIssueLineNo,";
                        sqlText += "@ItemItemNo,";
                        sqlText += "@ItemQuantity,";
                        sqlText += " 0,	";
                        sqlText += "@ItemCostPrice,";
                        sqlText += "@ItemUOM,";
                        sqlText += "@ItemVATRate,";
                        sqlText += "@ItemVATAmount,";
                        sqlText += "@ItemSubTotal,";
                        sqlText += "@ItemCommentsD,";
                        sqlText += "@MasterCreatedBy,";
                        sqlText += "@MasterCreatedOn,";
                        sqlText += "@MasterLastModifiedBy,";
                        sqlText += "@MasterLastModifiedOn,";
                        sqlText += "@ItemReceiveNoD,";
                        sqlText += "@ItemIssueDateTimeD,";
                        sqlText += " 0,";
                        sqlText += " 0,";
                        sqlText += "@ItemWastage,";
                        sqlText += "@ItemBOMDate,";
                        sqlText += "@ItemFinishItemNo,";
                        sqlText += "@MastertransactionType,";
                        sqlText += "@MasterReturnId,";
                        sqlText += "@ItemUOMQty,";
                        sqlText += "@ItemUOMPrice,";
                        sqlText += "@ItemUOMc,";
                        sqlText += "@ItemUOMn,";
                        sqlText += "@MasterPost";
                        sqlText += ")	";


                        SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                        cmdInsDetail.Transaction = transaction;
                        cmdInsDetail.Parameters.AddWithValue("@MasterIssueNo", Master.IssueNo ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemIssueLineNo", Item.IssueLineNo ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemQuantity", Item.Quantity);
                        cmdInsDetail.Parameters.AddWithValue("@ItemCostPrice", Item.CostPrice);
                        cmdInsDetail.Parameters.AddWithValue("@ItemUOM", Item.UOM ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemVATRate", Item.VATRate);
                        cmdInsDetail.Parameters.AddWithValue("@ItemVATAmount", Item.VATAmount);
                        cmdInsDetail.Parameters.AddWithValue("@ItemSubTotal", Item.SubTotal);
                        cmdInsDetail.Parameters.AddWithValue("@ItemCommentsD", Item.CommentsD ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterCreatedBy", Master.CreatedBy ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterCreatedOn", Master.CreatedOn ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterLastModifiedOn", DateTime.Now.ToString());
                        cmdInsDetail.Parameters.AddWithValue("@ItemReceiveNoD", Item.ReceiveNoD ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemIssueDateTimeD", OrdinaryVATDesktop.DateToDate(Item.IssueDateTimeD));
                        cmdInsDetail.Parameters.AddWithValue("@ItemWastage", Item.Wastage);
                        cmdInsDetail.Parameters.AddWithValue("@ItemBOMDate", Item.BOMDate ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemFinishItemNo", Item.FinishItemNo ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MastertransactionType", Master.transactionType ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterReturnId", Master.ReturnId ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemUOMQty", Item.UOMQty);
                        cmdInsDetail.Parameters.AddWithValue("@ItemUOMPrice", Item.UOMPrice);
                        cmdInsDetail.Parameters.AddWithValue("@ItemUOMc", Item.UOMc);
                        cmdInsDetail.Parameters.AddWithValue("@ItemUOMn", Item.UOMn ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterPost", Master.Post ?? Convert.DBNull);

                        transResult = (int)cmdInsDetail.ExecuteNonQuery();

                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, MessageVM.issueMsgUpdateNotSuccessfully);
                        }
                        #endregion Insert only DetailTable
                        #region Insert Issue and Receive if Transaction is not Other

                        #endregion Insert Issue and Receive if Transaction is not Other
                    }
                    else
                    {
                        //Update

                        #region Update only DetailTable

                        sqlText = "";
                        sqlText += " update IssueDetails set ";

                        sqlText += " IssueLineNo    =@ItemIssueLineNo,";
                        sqlText += " Quantity       =@ItemQuantity,";
                        sqlText += " CostPrice      =@ItemCostPrice,";
                        sqlText += " UOM            =@ItemUOM,";
                        sqlText += " SubTotal       =@ItemSubTotal,";
                        sqlText += " Comments       =@ItemComments,";
                        sqlText += " LastModifiedBy =@MasterLastModifiedBy,";
                        sqlText += " LastModifiedOn =@MasterLastModifiedOn,";
                        sqlText += " ReceiveNo      =@ItemReceiveNo,";
                        sqlText += " IssueDateTime  =@ItemIssueDateTime,";
                        sqlText += " Wastage        =@ItemWastage,";
                        sqlText += " BOMDate        =@ItemBOMDate,";
                        sqlText += " transactionType=@MastertransactionType,";
                        sqlText += " IssueReturnId  =@MasterReturnId,";
                        sqlText += " UOMQty         =@ItemUOMQty,";
                        sqlText += " UOMPrice       =@ItemUOMPrice,";
                        sqlText += " UOMc           =@ItemUOMc,";
                        sqlText += " UOMn           =@ItemUOMn";
                        sqlText += " where  IssueNo =@MasterIssueNo ";
                        sqlText += " and ItemNo     =@ItemItemNo";
                        if (!string.IsNullOrEmpty(Item.FinishItemNo))
                        {
                            if (Item.FinishItemNo != "N/A" && Item.FinishItemNo != "0")
                            {
                                sqlText += " and FinishItemNo=@ItemFinishItemNo";
                            }
                        }


                        SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                        cmdInsDetail.Transaction = transaction;

                        cmdInsDetail.Parameters.AddWithValue("@ItemIssueLineNo", Item.IssueLineNo ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemQuantity", Item.Quantity);
                        cmdInsDetail.Parameters.AddWithValue("@ItemCostPrice", Item.CostPrice);
                        cmdInsDetail.Parameters.AddWithValue("@ItemUOM", Item.UOM ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemSubTotal", Item.SubTotal);
                        cmdInsDetail.Parameters.AddWithValue("@ItemComments", Item.CommentsD ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn);
                        cmdInsDetail.Parameters.AddWithValue("@ItemReceiveNo", Item.ReceiveNoD ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemIssueDateTime", Item.IssueDateTimeD ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemWastage", Item.Wastage);
                        cmdInsDetail.Parameters.AddWithValue("@ItemBOMDate", Item.BOMDate ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MastertransactionType", Master.transactionType ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterReturnId", Master.ReturnId ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemUOMQty", Item.UOMQty);
                        cmdInsDetail.Parameters.AddWithValue("@ItemUOMPrice", Item.UOMPrice);
                        cmdInsDetail.Parameters.AddWithValue("@ItemUOMc", Item.UOMc);
                        cmdInsDetail.Parameters.AddWithValue("@ItemUOMn", Item.UOMn ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@MasterIssueNo", Master.IssueNo ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValue("@ItemFinishItemNo", Item.FinishItemNo ?? Convert.DBNull);

                        transResult = (int)cmdInsDetail.ExecuteNonQuery();

                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, MessageVM.issueMsgUpdateNotSuccessfully);
                        }
                        #endregion Update only DetailTable

                    }

                    #endregion Find Transaction Mode Insert or Update
                }//foreach (var Item in Details.ToList())

                #region Remove row
                sqlText = "";
                sqlText += " SELECT  distinct ItemNo";
                sqlText += " from IssueDetails WHERE IssueNo=@MasterIssueNo";

                DataTable dt = new DataTable("Previous");
                SqlCommand cmdRIFB = new SqlCommand(sqlText, currConn);
                cmdRIFB.Transaction = transaction;
                cmdRIFB.Parameters.AddWithValue("@MasterIssueNo", Master.IssueNo);
                SqlDataAdapter dta = new SqlDataAdapter(cmdRIFB);
                dta.Fill(dt);
                foreach (DataRow pItem in dt.Rows)
                {
                    var p = pItem["ItemNo"].ToString();

                    //var tt= Details.Find(x => x.ItemNo == p);
                    var tt = Master.Details.Count(x => x.ItemNo.Trim() == p.Trim());
                    if (tt == 0)
                    {
                        sqlText = "";
                        sqlText += " delete FROM IssueDetails ";
                        sqlText += " WHERE IssueNo=@MasterIssueNo ";
                        sqlText += " AND ItemNo=@p";
                        SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                        cmdInsDetail.Transaction = transaction;
                        cmdInsDetail.Parameters.AddWithValue("@MasterIssueNo", Master.IssueNo);
                        cmdInsDetail.Parameters.AddWithValue("@p", p);

                        transResult = (int)cmdInsDetail.ExecuteNonQuery();
                    }

                }
                #endregion Remove row


                #endregion Update Detail Table

                #endregion  Update into Details(Update complete in Header)


                #region return Current ID and Post Status

                sqlText = "";
                sqlText = sqlText + "select distinct Post from IssueHeaders WHERE IssueNo=@MasterIssueNo ";
                SqlCommand cmdIPS = new SqlCommand(sqlText, currConn);
                cmdIPS.Transaction = transaction;

                cmdIPS.Parameters.AddWithValue("@MasterIssueNo", Master.IssueNo);

                PostStatus = (string)cmdIPS.ExecuteScalar();
                if (string.IsNullOrEmpty(PostStatus))
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, MessageVM.issueMsgUnableCreatID);
                }


                #endregion Prefetch
                #region Commit

                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                    }

                }

                #endregion Commit
                #region SuccessResult

                retResults[0] = "Success";
                retResults[1] = MessageVM.issueMsgUpdateSuccessfully;
                retResults[2] = Master.Id;
                retResults[3] = PostStatus;
                #endregion SuccessResult

            }
            #endregion Try

            #region Catch and Finall
            #region Catch
            catch (Exception ex)
            {

                retResults[0] = "Fail";//Success or Fail
                retResults[1] = ex.Message.Split(new[] { '\r', '\n' }).FirstOrDefault(); //catch ex
                retResults[2] = nextId.ToString(); //catch ex

                transaction.Rollback();
                FileLogger.Log("IssueDAL", "IssueUpdate1", ex.ToString() + "\n" + sqlText);

                ////FileLogger.Log(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + sqlText);
                return retResults;
            }
            #endregion
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
            #endregion Catch and Finall

            #region Result
            return retResults;
            #endregion Result

        }

        public string[] IssuePost1(IssueMasterVM Master, SysDBInfoVMTemp connVM = null, string UserId = "")
        {
            #region Initializ
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            string sqlText = "";

            string PostStatus = "";


            #endregion Initializ

            #region Try
            try
            {
                string vNegStockAllow = string.Empty;
                CommonDAL commonDal = new CommonDAL();
                vNegStockAllow = commonDal.settings("Sale", "NegStockAllow");
                if (string.IsNullOrEmpty(vNegStockAllow))
                {
                    throw new ArgumentNullException(MessageVM.msgSettingValueNotSave, "Sale");
                }
                bool NegStockAllow = Convert.ToBoolean(vNegStockAllow == "Y" ? true : false);
                #region Validation for Header

                if (Master == null)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNamePost, MessageVM.issueMsgNoDataToPost);
                }
                else if (Convert.ToDateTime(Master.IssueDateTime) < DateTime.MinValue || Convert.ToDateTime(Master.IssueDateTime) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNamePost, MessageVM.issueMsgCheckDatePost);

                }



                #endregion Validation for Header
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                #region Add BOMId

                //commonDal.TableFieldAdd("IssueDetails", "BOMId", "varchar(20)", currConn); //tablename,fieldName, datatype
                //commonDal.TableFieldAdd("IssueDetails", "UOMQty", "decimal(25, 9)", currConn); //tablename,fieldName, datatype
                //commonDal.TableFieldAdd("IssueDetails", "UOMPrice", "decimal(25, 9)", currConn); //tablename,fieldName, datatype
                //commonDal.TableFieldAdd("IssueDetails", "UOMc", "decimal(25, 9)", currConn); //tablename,fieldName, datatype
                //commonDal.TableFieldAdd("IssueDetails", "UOMn", "varchar(50)", currConn); //tablename,fieldName, datatype
                //commonDal.TableFieldAdd("IssueDetails", "UOMWastage", "decimal(25, 9)", currConn); //tablename,fieldName, datatype


                #endregion Add BOMId
                transaction = currConn.BeginTransaction(MessageVM.issueMsgMethodNamePost);

                #endregion open connection and transaction
                #region Fiscal Year Check

                string transactionDate = Master.IssueDateTime;
                string transactionYearCheck = Convert.ToDateTime(Master.IssueDateTime).ToString("yyyy-MM-dd");
                if (Convert.ToDateTime(transactionYearCheck) > DateTime.MinValue || Convert.ToDateTime(transactionYearCheck) < DateTime.MaxValue)
                {

                    #region YearLock
                    sqlText = "";

                    sqlText += "select distinct isnull(PeriodLock,'Y') MLock,isnull(GLLock,'Y')YLock from fiscalyear " +
                                                   " where '" + transactionYearCheck + "' between PeriodStart and PeriodEnd";

                    DataTable dataTable = new DataTable("ProductDataT");
                    SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                    cmdIdExist.Transaction = transaction;
                    SqlDataAdapter reportDataAdapt = new SqlDataAdapter(cmdIdExist);
                    reportDataAdapt.Fill(dataTable);

                    if (dataTable == null)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                    }

                    else if (dataTable.Rows.Count <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                    }
                    else
                    {
                        if (dataTable.Rows[0]["MLock"].ToString() != "N")
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                        }
                        else if (dataTable.Rows[0]["YLock"].ToString() != "N")
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
                        }
                    }
                    #endregion YearLock
                    #region YearNotExist
                    sqlText = "";
                    sqlText = sqlText + "select  min(PeriodStart) MinDate, max(PeriodEnd)  MaxDate from fiscalyear";

                    DataTable dtYearNotExist = new DataTable("ProductDataT");

                    SqlCommand cmdYearNotExist = new SqlCommand(sqlText, currConn);
                    cmdYearNotExist.Transaction = transaction;
                    //countId = (int)cmdIdExist.ExecuteScalar();

                    SqlDataAdapter YearNotExistDataAdapt = new SqlDataAdapter(cmdYearNotExist);
                    YearNotExistDataAdapt.Fill(dtYearNotExist);

                    if (dtYearNotExist == null)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearNotExist);
                    }

                    else if (dtYearNotExist.Rows.Count < 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearNotExist);
                    }
                    else
                    {
                        if (Convert.ToDateTime(transactionYearCheck) < Convert.ToDateTime(dtYearNotExist.Rows[0]["MinDate"].ToString())
                            || Convert.ToDateTime(transactionYearCheck) > Convert.ToDateTime(dtYearNotExist.Rows[0]["MaxDate"].ToString()))
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearNotExist);
                        }
                    }
                    #endregion YearNotExist

                }


                #endregion Fiscal Year CHECK

                #region Find ID for Update

                sqlText = "";
                sqlText = sqlText + "select COUNT(IssueNo) from IssueHeaders WHERE IssueNo=@MasterIssueNo ";
                SqlCommand cmdFindIdUpd = new SqlCommand(sqlText, currConn);
                cmdFindIdUpd.Transaction = transaction;
                cmdFindIdUpd.Parameters.AddWithValue("@MasterIssueNo", Master.IssueNo);

                int IDExist = (int)cmdFindIdUpd.ExecuteScalar();

                if (IDExist <= 0)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNamePost, MessageVM.issueMsgUnableFindExistIDPost);
                }

                #endregion Find ID for Update

                #region ID check completed,update Information in Header

                #region update Header
                sqlText = "";

                sqlText += " update IssueHeaders set  ";
                sqlText += " LastModifiedBy=@MasterLastModifiedBy ,";
                sqlText += " LastModifiedOn=@MasterLastModifiedOn ,";
                sqlText += " Post=@MasterPost ";
                sqlText += " where  IssueNo=@MasterIssueNo ";


                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;
                cmdUpdate.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy);
                cmdUpdate.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn);
                cmdUpdate.Parameters.AddWithValue("@MasterPost", "Y");
                cmdUpdate.Parameters.AddWithValue("@MasterIssueNo", Master.IssueNo);

                transResult = (int)cmdUpdate.ExecuteNonQuery();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNamePost, MessageVM.issueMsgPostNotSuccessfully);
                }
                #endregion update Header



                #endregion ID check completed,update Information in Header

                #region Update into Details(Update complete in Header)
                #region Validation for Detail

                if (Master.Details.Count() < 0)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNamePost, MessageVM.issueMsgNoDataToPost);
                }


                #endregion Validation for Detail

                #region Update Detail Table

                foreach (var Item in Master.Details)
                {
                    #region Find Transaction Mode Insert or Update

                    sqlText = "";
                    sqlText += "select COUNT(IssueNo) from IssueDetails WHERE IssueNo=@MasterIssueNo ";
                    sqlText += " AND ItemNo=@ItemItemNo";
                    SqlCommand cmdFindId = new SqlCommand(sqlText, currConn);
                    cmdFindId.Transaction = transaction;

                    cmdFindId.Parameters.AddWithValue("@MasterIssueNo", Master.IssueNo);
                    cmdFindId.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo);

                    IDExist = (int)cmdFindId.ExecuteScalar();

                    if (IDExist <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.issueMsgMethodNamePost, MessageVM.issueMsgNoDataToPost);
                    }
                    else
                    {
                        #region Update only DetailTable

                        sqlText = "";
                        sqlText += " update IssueDetails set ";
                        sqlText += " Post=@MasterPost";
                        sqlText += " where  IssueNo =@MasterIssueNo ";
                        sqlText += " and ItemNo=@ItemItemNo";

                        SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                        cmdInsDetail.Transaction = transaction;
                        cmdInsDetail.Parameters.AddWithValue("@MasterPost", "Y");
                        cmdInsDetail.Parameters.AddWithValue("@MasterIssueNo", Master.IssueNo);
                        cmdInsDetail.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo);

                        transResult = (int)cmdInsDetail.ExecuteNonQuery();

                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.issueMsgMethodNamePost, MessageVM.issueMsgPostNotSuccessfully);
                        }
                        #endregion Update only DetailTable

                        #region Update Item Qty
                        else
                        {
                            #region Find Quantity From Products
                            ProductDAL productDal = new ProductDAL();
                            //decimal oldStock = productDal.StockInHand(Item.ItemNo, Master.IssueDateTime, currConn, transaction);
                            decimal oldStock = Convert.ToDecimal(productDal.AvgPriceNew(Item.ItemNo, Master.IssueDateTime,
                                                              currConn, transaction, true, true, true, true, connVM, UserId).Rows[0]["Quantity"].ToString());


                            #endregion Find Quantity From Products

                            #region Find Quantity From Transaction

                            sqlText = "";
                            sqlText += "select isnull(Quantity ,0) from IssueDetails ";
                            sqlText += " WHERE ItemNo=@ItemItemNo  and IssueNo= @MasterIssueNo ";
                            SqlCommand cmdTranQty = new SqlCommand(sqlText, currConn);
                            cmdTranQty.Transaction = transaction;

                            cmdTranQty.Parameters.AddWithValue("@MasterIssueNo", Master.IssueNo);
                            cmdTranQty.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo);

                            decimal TranQty = (decimal)cmdTranQty.ExecuteScalar();

                            #endregion Find Quantity From Transaction

                            #region Qty  check and Update
                            if (NegStockAllow == false)
                            {
                                if (TranQty > (oldStock + TranQty))
                                {
                                    throw new ArgumentNullException(MessageVM.issueMsgMethodNamePost,
                                                                    MessageVM.issueMsgStockNotAvailablePost);
                                }
                            }


                            #endregion Qty  check and Update
                        }

                        #endregion Qty  check and Update
                    }
                    #endregion Find Transaction Mode Insert or Update
                }

                #endregion Update Detail Table

                #endregion  Update into Details(Update complete in Header)


                #region return Current ID and Post Status

                sqlText = "";
                sqlText = sqlText + "select distinct Post from IssueHeaders WHERE IssueNo=@MasterIssueNo";
                SqlCommand cmdIPS = new SqlCommand(sqlText, currConn);
                cmdIPS.Transaction = transaction;

                cmdIPS.Parameters.AddWithValue("@MasterIssueNo", Master.IssueNo);

                PostStatus = (string)cmdIPS.ExecuteScalar();
                if (string.IsNullOrEmpty(PostStatus))
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNamePost, MessageVM.issueMsgPostNotSelect);
                }


                #endregion Prefetch
                #region Commit

                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                    }

                }

                #endregion Commit
                #region SuccessResult

                retResults[0] = "Success";
                retResults[1] = MessageVM.issueMsgSuccessfullyPost;
                retResults[2] = Master.IssueNo;
                retResults[3] = PostStatus;
                #endregion SuccessResult

            }
            #endregion Try

            #region Catch and Finall
            catch (SqlException sqlex)
            {
                transaction.Rollback();
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;

                FileLogger.Log("IssueDAL", "IssueInsertToMaster", sqlex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                transaction.Rollback();
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;

                FileLogger.Log("IssueDAL", "DemandPost", ex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", ex.Message.ToString());

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
            #endregion Catch and Finall

            #region Result
            return retResults;
            #endregion Result

        }

        #endregion

        public string[] MultiplePost(string[] Ids, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion Initializ

            #region try

            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                transaction = currConn.BeginTransaction(MessageVM.issueMsgMethodNameUpdate);
                CommonDAL commonDal = new CommonDAL();
                bool TollReceiveWithIssue = Convert.ToBoolean(commonDal.settings("TollReceive", "WithIssue", null, null, connVM) == "Y" ? true : false);

                Ids = Ids.Where(x => !string.IsNullOrEmpty(x)).ToArray();

                #endregion open connection and transaction
                //for (int i = 0; i < Ids.Length - 1; i++)
                for (int i = 0; i < Ids.Length; i++)
                {
                    IssueMasterVM master = SelectAllList(Convert.ToInt32(Ids[i]), null, null, currConn, transaction, null, connVM).FirstOrDefault();
                    List<IssueDetailVM> Details = SelectIssueDetail(master.IssueNo, null, null, currConn, transaction, connVM);
                    List<TrackingVM> Trakings = new List<TrackingVM>();
                    master.Post = "Y";
                    retResults = IssuePost(master, Details, transaction, currConn, connVM);
                    if (retResults[0] != "Success")
                    {
                        throw new ArgumentNullException("", retResults[1]);
                    }
                }
                #region Commit
                if (transaction != null)
                {
                    transaction.Commit();
                }
                #endregion


            }
            #endregion

            #region catch

            catch (Exception ex)
            {
                retResults = new string[5];
                retResults[0] = "Fail";
                retResults[1] = ex.Message;
                retResults[2] = "0";
                retResults[3] = "N";
                retResults[4] = "0";

                FileLogger.Log("IssueDAL", "MultiplePost", ex.ToString());

                throw ex;
            }
            #endregion

            #region finally

            finally
            {

            }
            #endregion

            #region Result
            return retResults;
            #endregion Result
        }

        public string[] ImportReceiveBigData(DataTable receiveData, SysDBInfoVMTemp connVM = null)
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

            #region try

            try
            {
                #region Open Connection and Transaction

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    currConn.StatisticsEnabled = true;
                    currConn.Open();
                    transaction = currConn.BeginTransaction();
                }

                #endregion

                #region SQL Text

                var updateItemNo = @"
update ReceiveTempData set ItemNo=Products.ItemNo from Products where Products.ProductCode=ReceiveTempData.Item_Code;
update ReceiveTempData set ItemNo=Products.ItemNo from Products where Products.productName =ReceiveTempData.Item_Name;

update ReceiveTempData set CustomerID=Customers.CustomerID from Customers where Customers.CustomerCode =ReceiveTempData.Customer_Code;
update ReceiveTempData set CustomerID=Customers.CustomerID from Customers where Customers.CustomerName =ReceiveTempData.Customer_Name;
";


                var getdefaultData = @"select * from ReceiveTempData where ItemNo = 0 or CustomerID = 0;";
                var branchUpdate = @"
update ReceiveTempData set BranchId=BranchProfiles.BranchID from BranchProfiles where BranchProfiles.BranchCode=ReceiveTempData.Branch_Code;
";


                var deleteTemp = @"delete from ReceiveTempData; ";

                deleteTemp += " DBCC CHECKIDENT ('ReceiveTempData', RESEED, 0);";

                #endregion

                var cmd = new SqlCommand(deleteTemp, currConn, transaction);
                cmd.ExecuteNonQuery();

                var commonDal = new CommonDAL();

                retResults = commonDal.BulkInsert("ReceiveTempData", receiveData, currConn, transaction, 10000, null, connVM);

                #region Sql Command

                cmd.CommandText = branchUpdate + " " + updateItemNo;
                var updateItemResult = cmd.ExecuteNonQuery();

                cmd.CommandText = getdefaultData;
                var defaultData = new DataTable();
                var dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(defaultData);

                #endregion

                #region Insert Customer and Products

                var commonImportDal = new CommonImportDAL();

                foreach (DataRow row in defaultData.Rows)
                {
                    if (row["CustomerID"].ToString().Trim() == "0")
                    {
                        var customerName = row["Customer_Name"].ToString().Trim();
                        var customerCode = row["Customer_Code"].ToString().Trim();

                        commonImportDal.FindCustomerId(customerName, customerCode, currConn, transaction, true, null, 1, connVM);
                    }

                    if (row["ItemNo"].ToString().Trim() == "0")
                    {
                        var itemCode = row["Item_Code"].ToString().Trim();
                        var itemName = row["Item_Name"].ToString().Trim();

                        commonImportDal.FindItemId(itemName, itemCode, currConn, transaction, true, "-", 1, 0, 0, connVM);
                    }
                }

                #endregion

                #region Reupdate

                cmd.CommandText = updateItemNo;
                var reUpdate = cmd.ExecuteNonQuery();

                #endregion


                if (retResults[0].ToLower() == "success" && updateItemResult > 0)
                {
                    transaction.Commit();
                }

                retResults[0] = retResults[0].ToLower() == "success" && (updateItemResult > 0 || reUpdate > 0) ? "success" : "fail";

                retResults[2] = currConn.RetrieveStatistics()["ExecutionTime"].ToString();
            }
            #endregion

            #region Catch and finally

            catch (Exception ex)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }
                ////throw new ArgumentNullException("", "SQL:" + "" + FieldDelimeter + ex.Message.ToString());

                FileLogger.Log("IssueDAL", "ImportReceiveBigData", ex.ToString());
                throw new ArgumentNullException("", ex.Message.ToString());

            }
            finally
            {
                if (currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }

            #endregion

            return retResults;
        }

        #region Issue Split Methods

        public string[] SaveIssue_Split(IntegrationParam param, SysDBInfoVMTemp connVM = null)
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
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion

            #region try

            try
            {
                #region open connection and transaction

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                #endregion open connection and transaction


                CommonDAL commonDal = new CommonDAL();
                var value = commonDal.settingValue("CompanyCode", "Code", connVM, currConn, transaction);

                #region delete and bulk insert to Source

                string deleteSource = @"delete from VAT_Source_Issue";
                SqlCommand cmd = new SqlCommand(deleteSource, currConn, transaction);
                cmd.ExecuteNonQuery();

                string[] result = commonDal.BulkInsert("VAT_Source_Issue", param.Data, currConn, transaction, 0, null, connVM);

                #endregion

                if (value.ToLower() == "smc")
                {
                    string updateDate = @"update VAT_Source_Issue set ID='Issue against previous BPR', Reference_No='Issue against previous BPR'
where ID='Issue against previous BPR'
";
                    cmd.CommandText = updateDate;

                    cmd.ExecuteNonQuery();
                }

                #region update ID

                string updateID = "update VAT_Source_Issue set ID = ID+'~'+FORMAT(Issue_DateTime,'ddMMyy')+'~'+Reference_No";
                cmd.CommandText = updateID;

                cmd.ExecuteNonQuery();



                #endregion

                #region delete duplicate

                string deleteDuplicate = @"
update  VAT_Source_Issue                             
set PeriodId=RIGHT('0'+CONVERT(VARCHAR(2),MONTH(Issue_DateTime)) +  CONVERT(VARCHAR(4),YEAR(Issue_DateTime)),6)
where PeriodId is null or PeriodId = ''

delete from VAT_Source_Issue where ID in (
                select vr.ID from VAT_Source_Issue vr inner join IssueHeaders rh
                on vr.ID = rh.ImportIDExcel and vr.PeriodId = rh.PeriodId)

Update VAT_Source_Issue set UOM = Products.UOM
from Products
where VAT_Source_Issue.Item_Code = Products.ProductCode


";

                cmd.CommandText = deleteDuplicate;
                cmd.ExecuteNonQuery();
                #endregion

                #region Loop counter

                string getLoopCount = @"select Ceiling(count(distinct ID)/500.00) from VAT_Source_Issue";

                cmd.CommandText = getLoopCount;
                int counter = Convert.ToInt32(cmd.ExecuteScalar());

                #endregion

                transaction.Commit();
                currConn.Close();

                // if condition

                if (counter == 0)
                {

                    //throw new ArgumentNullException("These Invoices are already in system-");
                    retResults[1] = ("These Invoices are already in system-");

                }

                DataTable sourceData = new DataTable();

                for (int i = 0; i < counter; i++)
                {
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    currConn.Open();
                    transaction = currConn.BeginTransaction();
                    cmd.Connection = currConn;
                    cmd.Transaction = transaction;

                    #region Create Temp tables

                    string tempTableCreate = @"create table #tempIds(sl int identity(1,1), ID varchar(500))";
                    cmd.CommandText = tempTableCreate;
                    cmd.ExecuteNonQuery();

                    #endregion


                    #region Get Top Rows

                    string insertIds = @"insert into #tempIds(ID)
select  distinct top 500 ID 
from VAT_Source_Issue
where isnull(IsProcessed,'N') = 'N'";

                    cmd.CommandText = insertIds;
                    cmd.ExecuteNonQuery();

                    string getData = @"SELECT 
      [ID]
      ,[BranchCode]
      ----,[Issue_DateTime]
      ,MAX([Issue_DateTime]) over(order by ID) [Issue_DateTime]
      ,[Reference_No]
      ,[Comments]
      ,[Return_Id]
      ,[Post]
      ,[Item_Code]
      ,[Item_Name]
      ,[Quantity]
      ,[UOM]
  FROM VAT_Source_Issue where ID in (select ID from #tempIds)";

                    cmd.CommandText = getData;
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(sourceData);

                    //sourceData.Columns.Remove("IsProcessed");

                    #endregion

                    retResults = SaveTempIssue(sourceData, param.TransactionType, param.CurrentUser, param.DefaultBranchId,
                        param.callBack, currConn, transaction, connVM, param.BranchCode);


                    #region updateSourceTable

                    string updateSourceAndClearTemp = @"update VAT_Source_Issue set IsProcessed = 'Y' where ID  in (select ID from #tempIds);
                                            --delete from #tempIds;";

                    cmd.CommandText = updateSourceAndClearTemp;
                    cmd.ExecuteNonQuery();

                    #endregion

                    if (OrdinaryVATDesktop.IsACICompany(value))
                    {
                        //currConn = _dbsqlConnection.GetConnection();
                        //currConn.Open();
                        //transaction = currConn.BeginTransaction();
                        //cmd.Connection = currConn;
                        //cmd.Transaction = transaction;


                        //                        string updateIntegration =
                        //                            @"
                        //update  ACIData.dbo.Issues set JoinId  = ID+'~'+FORMAT(Issue_DateTime,'ddMMyy')+'~'+Reference_No
                        //where JoinId is null
                        //
                        //update ACIData.dbo.Issues set IsProcessed = isnull(VAT_Source_Issue.IsProcessed,'N')
                        //from VAT_Source_Issue
                        //where VAT_Source_Issue.ID =  ACIData.dbo.Issues.JoinId and Item_Code = ACIData.dbo.Issues.ProductCode and ACIData.dbo.Issues.CompanyCode = '" +
                        //                            value + "'";

                        string updateIntegration = "";

                        if (value == "ACI-1")
                        {
                            updateIntegration =
                            @"
update  ACIData.dbo.Issues set JoinId  = ID+'~'+FORMAT(Issue_DateTime,'ddMMyy')+'~'+Reference_No
where JoinId is null

update ACIData.dbo.Issues set IsProcessed = isnull(VAT_Source_Issue.IsProcessed,'N')
from VAT_Source_Issue
where VAT_Source_Issue.ID =  ACIData.dbo.Issues.JoinId and Item_Code = ACIData.dbo.Issues.ProductCode and ACIData.dbo.Issues.CompanyCode = '" +
                            value + "'";
                        }
                        else
                        {
                            updateIntegration =
                            @"
update  ACIData.dbo.Issues set JoinId  = ID+'~'+FORMAT(Issue_DateTime,'ddMMyy')+'~'+Reference_No
where JoinId is null

update ACIData.dbo.Issues set IsProcessed = isnull(VAT_Source_Issue.IsProcessed,'N')
from VAT_Source_Issue
where VAT_Source_Issue.ID =  ACIData.dbo.Issues.JoinId and ACIData.dbo.Issues.CompanyCode = '" +
                            value + "'";
                        }

                        cmd.CommandText = updateIntegration;
                        cmd.ExecuteNonQuery();

                    }

                    transaction.Commit();
                    currConn.Close();
                    transaction.Dispose();
                    currConn.Dispose();

                    sourceData.Clear();
                }

                return retResults;
            }
            #endregion

            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[1] = ex.Message.ToString(); //catch ex
                //retResults[4] = ex.Message.ToString(); //catch ex
                if (transaction != null) { transaction.Rollback(); }
                FileLogger.Log("IssueDAL", "SaveIssue_Split", ex.ToString() + "\n" + sqlText);

                throw ex;
            }
            finally
            {
                if (currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion
        }

        #endregion


        public string[] UpdateIssueFlag(string issueDatetime, string itemNo, SysDBInfoVMTemp connVM = null)
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
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion

            #region try

            try
            {
                #region open connection and transaction

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                #endregion open connection and transaction

                sqlText = @"UPDATE IssueDetails set IsDayEnd = 'Y' where IssueDateTime >= @issueDateTime and ItemNo = @ItemNo";

                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);

                cmd.Parameters.AddWithValue(@"issueDateTime", issueDatetime);
                cmd.Parameters.AddWithValue(@"ItemNo", itemNo);

                cmd.ExecuteNonQuery();

                transaction.Commit();

                return retResults;

            }
            #endregion

            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
                if (transaction != null) { transaction.Rollback(); }
                FileLogger.Log("IssueDAL", "UpdateIssueFlag", ex.ToString() + "\n" + sqlText);

                throw ex;
            }
            finally
            {
                if (currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion
        }

        #region Excell upload Validation

        public ResultVM ExcelReceiveValidation(DataTable data, string columnName, SysDBInfoVMTemp connVM = null)
        {

            ResultVM result = new ResultVM();
            List<ErrorMessage> errorList = new List<ErrorMessage>();
            #region try

            try
            {
                // process

                foreach (DataRow row in data.Rows)
                {
                    if (columnName.ToLower() == "quantity")
                    {
                        if (!OrdinaryVATDesktop.IsNumeric(row[columnName].ToString()))
                        {
                            errorList.Add(new ErrorMessage()
                            {
                                ColumnName = columnName,
                                Message = "Quantity Field is numeric of ID :" + row["ID"]
                            });
                        }
                    }

                    if (columnName.ToLower() == "issue_datetime")
                    {
                        if (!OrdinaryVATDesktop.IsDate(row[columnName].ToString()))
                        {
                            errorList.Add(new ErrorMessage()
                            {
                                ColumnName = columnName,
                                Message = "Issue_DateTime Field is datetime of ID :" + row["ID"]
                            });
                        }
                    }



                    if (columnName.ToLower() == "post")
                    {
                        if (!OrdinaryVATDesktop.IsYNCheck(row[columnName].ToString()))
                        {
                            errorList.Add(new ErrorMessage()
                            {
                                ColumnName = columnName,
                                Message = "Post Field is only Y/N of ID :" + row["ID"]
                            });
                        }
                    }

                }



                if (errorList.Count > 0)
                {
                    result.Status = "fail";
                    result.Message = "fail";
                    result.ErrorList = errorList;

                }
                else
                {

                    result.Status = "Success";
                    result.Message = "Success";
                }


                return result;
            }
            #endregion

            #region catch
            catch (Exception ex)
            {
                FileLogger.Log("ReceiveDAL", "ExcelReceiveValidation", ex.ToString());
                throw ex;
            }

            #endregion
        }

        public ResultVM ExcelReceiveValidationRM(DataTable data, string columnName, SysDBInfoVMTemp connVM = null)
        {

            ResultVM result = new ResultVM();
            List<ErrorMessage> errorList = new List<ErrorMessage>();
            #region try

            try
            {
                // process

                foreach (DataRow row in data.Rows)
                {
                    if (columnName.ToLower() == "quantity")
                    {
                        if (!OrdinaryVATDesktop.IsNumeric(row[columnName].ToString()))
                        {
                            errorList.Add(new ErrorMessage()
                            {
                                ColumnName = columnName,
                                Message = "Quantity Field is numeric of ID :" + row["ID"]
                            });
                        }
                    }


                    if (columnName.ToLower() == "post")
                    {
                        if (!OrdinaryVATDesktop.IsYNCheck(row[columnName].ToString()))
                        {
                            errorList.Add(new ErrorMessage()
                            {
                                ColumnName = columnName,
                                Message = "Post Field is only Y/N of ID :" + row["ID"]
                            });
                        }
                    }

                    if (columnName.ToLower() == "vat_rate")
                    {
                        if (!OrdinaryVATDesktop.IsNumeric(row[columnName].ToString()))
                        {
                            errorList.Add(new ErrorMessage()
                            {
                                ColumnName = columnName,
                                Message = "VAT_Rate Field is only Y/N of ID :" + row["ID"]
                            });
                        }
                    }

                    if (columnName.ToLower() == "costprice")
                    {
                        if (!OrdinaryVATDesktop.IsNumeric(row[columnName].ToString()))
                        {
                            errorList.Add(new ErrorMessage()
                            {
                                ColumnName = columnName,
                                Message = "CostPrice Field is only Y/N of ID :" + row["ID"]
                            });
                        }
                    }

                }


                if (errorList.Count > 0)
                {
                    result.Status = "fail";
                    result.Message = "fail";
                    result.ErrorList = errorList;

                }
                else
                {

                    result.Status = "Success";
                    result.Message = "Success";
                }


                return result;
            }
            #endregion

            #region catch
            catch (Exception ex)
            {
                FileLogger.Log("ReceiveDAL", "ExcelReceiveValidationRM", ex.ToString());
                throw ex;
            }

            #endregion
        }


        #endregion

    }
}

