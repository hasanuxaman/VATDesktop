using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VATServer.Ordinary;
using VATViewModel.DTOs;

namespace VATServer.Library
{
    public class TollContInOutDAL
    {

        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        private const string FieldDelimeter = DBConstant.FieldDelimeter;


        #region Navigation

        public NavigationVM TollCont_Navigation(NavigationVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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
select top 1 inv.Id, inv.Code InvoiceNo from TollContInOuts inv
where 1=1 
and inv.Code=@InvoiceNo

";
                    #endregion
                }
                else if (vm.Id == 0 || vm.ButtonName == "First")
                {

                    #region First Item

                    sqlText = sqlText + @"
--------------------------------------------------First--------------------------------------------------
---------------------------------------------------------------------------------------------------------
select top 1 inv.Id, inv.Code InvoiceNo from TollContInOuts inv
where 1=1 
--and FiscalYear=@FiscalYear
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
select top 1 inv.Id, inv.Code InvoiceNo from TollContInOuts inv
where 1=1 
--and FiscalYear=@FiscalYear
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
select top 1 inv.Id, inv.Code InvoiceNo from TollContInOuts inv
where 1=1 
--and FiscalYear=@FiscalYear
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
select top 1 inv.Id, inv.Code InvoiceNo from TollContInOuts inv
where 1=1 
--and FiscalYear=@FiscalYear
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
                    cmd.Parameters.AddWithValue("@Code", vm.InvoiceNo);
                }
                else
                {
                    //cmd.Parameters.AddWithValue("@FiscalYear", vm.FiscalYear);
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
                        vm = TollCont_Navigation(vm, currConn, transaction);

                    }
                    else if (vm.ButtonName == "Next")
                    {
                        vm.ButtonName = "Last";
                        vm = TollCont_Navigation(vm, currConn, transaction);

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

                FileLogger.Log("TollProductionConsumptionDAL", "TollProduction_Navigation", ex.ToString() + "\n" + sqlText);
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


        public ResultVM SaveData(TollContInOutVM MasterVM, List<TollContInOutDetailVM> DetailVMs, SqlTransaction Vtransaction, SqlConnection VcurrConn, SysDBInfoVMTemp connVM = null)
        {

            #region Initializ

            ResultVM rVM = new ResultVM();
            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            int transResult = 0;
            string sqlText = "";

            string newIDCreate = "";
            string PostStatus = "";

            int IDExist = 0;

            string Id = "";

            #endregion Initializ

            #region Try
            try
            {

                #region Find Month Lock

                string PeriodName = Convert.ToDateTime(MasterVM.DateTime).ToString("MMMM-yyyy");
                string[] vValues = { PeriodName };
                string[] vFields = { "PeriodName" };
                FiscalYearVM varFiscalYearVM = new FiscalYearVM();
                varFiscalYearVM = new FiscalYearDAL().SelectAll(0, vFields, vValues, null, null, connVM).FirstOrDefault();

                if (varFiscalYearVM == null)
                {
                    throw new Exception(PeriodName + ": This Fiscal Period is not Exist!");

                }

                if (varFiscalYearVM.VATReturnPost == "Y")
                {
                    throw new Exception(PeriodName + ": VAT Return (9.1) already submitted for this month!");

                }

                #endregion Find Month Lock

                #region Validation for Header

                if (MasterVM == null)
                {
                    throw new ArgumentNullException(MessageVM.tollOutMsgMethodNameInsert, MessageVM.tollOutMsgNoDataToSave);
                }
                else if (Convert.ToDateTime(MasterVM.DateTime) < DateTime.MinValue || Convert.ToDateTime(MasterVM.DateTime) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException(MessageVM.tollOutMsgMethodNameInsert, "Please Check Invoice Data and Time");
                }

                #endregion Validation for Header

                CommonDAL commonDal = new CommonDAL();

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
                    transaction = currConn.BeginTransaction("InsertData");
                }

                #endregion open connection and transaction

                #region Fiscal Year Check

                string transactionDate = MasterVM.DateTime;
                string transactionYearCheck = Convert.ToDateTime(MasterVM.DateTime).ToString("yyyy-MM-dd");
                if (Convert.ToDateTime(transactionYearCheck) > DateTime.MinValue || Convert.ToDateTime(transactionYearCheck) < DateTime.MaxValue)
                {

                    #region YearLock
                    sqlText = "";

                    sqlText += "select distinct isnull(PeriodLock,'Y') MLock,isnull(GLLock,'Y')YLock from fiscalyear " +
                                                   " where '" + transactionYearCheck + "' between PeriodStart and PeriodEnd";

                    DataTable dataTable = new DataTable("ProductDataT");
                    SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                    cmdIdExist.CommandTimeout = 500;
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
                    cmdYearNotExist.CommandTimeout = 500;
                    cmdYearNotExist.Transaction = transaction;

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
                sqlText = sqlText + "select COUNT(Code) from TollContInOuts WHERE Code=@MasterCode ";
                SqlCommand cmdExistTran = new SqlCommand(sqlText, currConn);
                cmdExistTran.CommandTimeout = 500;
                cmdExistTran.Transaction = transaction;
                cmdExistTran.Parameters.AddWithValueAndNullHandle("@MasterCode", MasterVM.Code);
                IDExist = (int)cmdExistTran.ExecuteScalar();

                if (IDExist > 0)
                {
                    throw new ArgumentNullException("Requested Information already Added", "");
                }

                #endregion Find Transaction Exist

                var latestId = "";
                var fiscalYear = "";

                #region ID Create

                if (string.IsNullOrEmpty(MasterVM.TransactionType))
                {
                    throw new ArgumentNullException(MessageVM.msgTransactionNotDeclared, "");
                }

                #region CodeGenaration

                #region Other

                if (MasterVM.TransactionType == "TollCont6_4Outs")
                {
                    newIDCreate = commonDal.TransactionCode("Toll", "TollCont6_4Outs", "TollContInOuts", "Code", "DateTime", MasterVM.DateTime, MasterVM.BranchId.ToString(), currConn, transaction, connVM);
                    //New Method
                    var resultCode = commonDal.GetCurrentCode("Toll", "TollCont6_4Outs", MasterVM.DateTime, MasterVM.BranchId.ToString(), currConn, transaction, connVM);
                    var newIdara = resultCode.Split('~');
                    latestId = newIdara[0];
                    fiscalYear = newIdara[1];

                }
                if (MasterVM.TransactionType == "TollCont6_4Ins")
                {
                    newIDCreate = commonDal.TransactionCode("Toll", "TollCont6_4Ins", "TollContInOuts", "Code", "DateTime", MasterVM.DateTime, MasterVM.BranchId.ToString(), currConn, transaction, connVM);
                    //New Method
                    var resultCode = commonDal.GetCurrentCode("Toll", "TollCont6_4Ins", MasterVM.DateTime, MasterVM.BranchId.ToString(), currConn, transaction, connVM);
                    var newIdara = resultCode.Split('~');
                    latestId = newIdara[0];
                    fiscalYear = newIdara[1];

                }
                if (MasterVM.TransactionType == "TollCont6_4Backs")
                {
                    newIDCreate = commonDal.TransactionCode("Toll", "TollCont6_4Backs", "TollContInOuts", "Code", "DateTime", MasterVM.DateTime, MasterVM.BranchId.ToString(), currConn, transaction, connVM);
                    //New Method
                    var resultCode = commonDal.GetCurrentCode("Toll", "TollCont6_4Backs", MasterVM.DateTime, MasterVM.BranchId.ToString(), currConn, transaction, connVM);
                    var newIdara = resultCode.Split('~');
                    latestId = newIdara[0];
                    fiscalYear = newIdara[1];

                }

                #endregion

                if (string.IsNullOrEmpty(newIDCreate) || newIDCreate == "")
                {
                    throw new ArgumentNullException(MessageVM.tollOutMsgMethodNameInsert, "ID Prefetch not set please update Prefetch first");
                }

                MasterVM.Code = newIDCreate;

                #region checkId and FiscalYear

                sqlText = @"select count(Code) from TollContInOuts 
where Code = @Code and TransactionType = @TransactionType and BranchId = @BranchId";

                var sqlCmd = new SqlCommand(sqlText, currConn, transaction);

                sqlCmd.CommandTimeout = 500;
                sqlCmd.Parameters.AddWithValue("@Code", newIDCreate);
                sqlCmd.Parameters.AddWithValue("@TransactionType", MasterVM.TransactionType);
                sqlCmd.Parameters.AddWithValue("@BranchId", MasterVM.BranchId);

                var count = (int)sqlCmd.ExecuteScalar();

                if (count > 0)
                {
                    throw new Exception("Code " + newIDCreate + " Already Exists");
                }

                #endregion

                #endregion

                #endregion Purchase ID Create Not Complete

                #region ID generated completed,Insert new Information in Header

                rVM = InsertHeaders(MasterVM, currConn, transaction, connVM);

                MasterVM.Id = Convert.ToInt32(rVM.Id);

                if (rVM.Status != "Success")
                {
                    throw new ArgumentNullException(rVM.Message, "");
                }

                #endregion ID generated completed,Insert new Information in Header

                #region Insert into Details(Insert complete in Header)

                #region Validation for Detail


                if (DetailVMs.Count() < 0)
                {
                    throw new ArgumentNullException(MessageVM.tollOutMsgNoDataToSave, "");
                }

                #endregion Validation for Detail

                rVM = SaveDetails(MasterVM, DetailVMs, transaction, currConn, connVM);

                if (rVM.Status != "Success")
                {
                    throw new ArgumentNullException(rVM.Message, "");
                }
                #endregion Insert into Details(Insert complete in Header)

                #region Update PeriodId

                sqlText = "";
                sqlText += @"

UPDATE TollContInOuts 
SET PeriodID=RIGHT('0'+CONVERT(VARCHAR(2),MONTH(DateTime)) +  CONVERT(VARCHAR(4),YEAR(DateTime)),6)
WHERE Id = @Id


UPDATE TollContInOutDetails 
SET PeriodID=RIGHT('0'+CONVERT(VARCHAR(2),MONTH(DateTime)) +  CONVERT(VARCHAR(4),YEAR(DateTime)),6)
WHERE HeaderId = @Id

";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                cmdUpdate.CommandTimeout = 500;
                cmdUpdate.Parameters.AddWithValue("@Id", MasterVM.Id);

                transResult = cmdUpdate.ExecuteNonQuery();

                #endregion

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion

                #region SuccessResult

                rVM = new ResultVM();
                rVM.Status = "Success";
                rVM.Message = MessageVM.saleMsgSaveSuccessfully;
                rVM.Id = MasterVM.Id.ToString();
                rVM.InvoiceNo = MasterVM.Code;

                #endregion SuccessResult

            }
            #endregion Try

            #region Catch and Finall

            catch (Exception ex)
            {
                rVM = new ResultVM();

                rVM.Status = "Fail";
                rVM.Message = ex.Message;

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Rollback();

                }

                FileLogger.Log("TollContInOutDAL", "InsertData", ex.ToString() + "\n" + sqlText);

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

            return rVM;

            #endregion Result

        }

        public ResultVM InsertHeaders(TollContInOutVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {

            #region Variables

            ResultVM rVM = new ResultVM();

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            #endregion

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

                #region Inser New
                sqlText = "";

                sqlText += @" 
INSERT INTO TollContInOuts(
 BranchId
,Code
,VendorID
,DateTime
,PeriodID
,RefNo
,Comments
,ImportID
,Post
,TransactionType
,CreatedBy
,CreatedOn
,IsCancle


) VALUES (
 @BranchId
,@Code
,@VendorID
,@DateTime
,@PeriodID
,@RefNo
,@Comments
,@ImportID
,@Post
,@TransactionType
,@CreatedBy
,@CreatedOn
,@IsCancle

) SELECT SCOPE_IDENTITY()
";

                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;
                cmdInsert.Parameters.AddWithValue("@BranchId", vm.BranchId);
                cmdInsert.Parameters.AddWithValue("@Code", vm.Code);
                cmdInsert.Parameters.AddWithValue("@VendorID", vm.VendorID);
                cmdInsert.Parameters.AddWithValue("@DateTime", vm.DateTime);
                cmdInsert.Parameters.AddWithValue("@PeriodID", vm.PeriodID);
                cmdInsert.Parameters.AddWithValue("@RefNo", vm.RefNo);
                cmdInsert.Parameters.AddWithValue("@Comments", vm.Comments);
                cmdInsert.Parameters.AddWithValue("@ImportID", vm.ImportID);
                cmdInsert.Parameters.AddWithValue("@Post", vm.Post);
                cmdInsert.Parameters.AddWithValue("@TransactionType", vm.TransactionType);
                cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                cmdInsert.Parameters.AddWithValue("@CreatedOn", vm.CreatedOn);
                cmdInsert.Parameters.AddWithValue("@IsCancle", vm.IsCancle);

                transResult = Convert.ToInt32(cmdInsert.ExecuteScalar());

                rVM.Status = "Success";
                rVM.Message = "Requested Information successfully Added";
                rVM.Id = transResult.ToString();

                #region Commit
                if (VcurrConn == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }

                #endregion Commit

                #endregion Inser new

            }
            #endregion

            #region Catch
            catch (Exception ex)
            {

                rVM.Status = "Fail";
                rVM.Message = ex.Message.Split(new[] { '\r', '\n' }).FirstOrDefault(); ;

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Rollback();
                }

                FileLogger.Log("TollContInOutDAL", "Insert", ex.ToString() + "\n" + sqlText);

                return rVM;
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

            return rVM;
        }

        public ResultVM InsertDetails(TollContInOutDetailVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {

            #region Variables

            ResultVM rVM = new ResultVM();

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            #endregion

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

                #region Inser New
                sqlText = "";

                sqlText += @" 
INSERT INTO TollContInOutDetails(
 HeaderId
,BranchId
,DateTime
,PeriodID
,Post
,TransactionType
,IsCancle
,ItemNo
,Quantity
,UnitCost
,UOM
,UOMc
,UOMn
--,BomId
,SubTotal


) VALUES (
 @HeaderId
,@BranchId
,@DateTime
,@PeriodID
,@Post
,@TransactionType
,@IsCancle
,@ItemNo
,@Quantity
,@UnitCost
,@UOM
,@UOMc
,@UOMn
--,@BomId
,@SubTotal

) 
";

                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;
                cmdInsert.Parameters.AddWithValue("@HeaderId", vm.HeaderId);
                cmdInsert.Parameters.AddWithValue("@BranchId", vm.BranchId);
                cmdInsert.Parameters.AddWithValue("@DateTime", vm.DateTime);
                cmdInsert.Parameters.AddWithValue("@PeriodID", vm.PeriodID);
                cmdInsert.Parameters.AddWithValue("@Post", vm.Post);
                cmdInsert.Parameters.AddWithValue("@TransactionType", vm.TransactionType);
                cmdInsert.Parameters.AddWithValue("@IsCancle", vm.IsCancle);
                cmdInsert.Parameters.AddWithValue("@ItemNo", vm.ItemNo);
                cmdInsert.Parameters.AddWithValue("@Quantity", vm.Quantity);
                cmdInsert.Parameters.AddWithValue("@UnitCost", vm.UnitCost);
                cmdInsert.Parameters.AddWithValue("@UOM", vm.UOM);
                cmdInsert.Parameters.AddWithValue("@UOMc", vm.UOMc);
                cmdInsert.Parameters.AddWithValue("@UOMn", vm.UOMn);
                //cmdInsert.Parameters.AddWithValue("@BomId", vm.BomId);
                cmdInsert.Parameters.AddWithValue("@SubTotal", vm.SubTotal);

                transResult = (int)cmdInsert.ExecuteNonQuery();

                rVM.Status = "Success";
                rVM.Message = "Requested Information successfully Added";

                #region Commit
                if (VcurrConn == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }

                #endregion Commit

                #endregion Inser new customer

            }
            #endregion

            #region Catch
            catch (Exception ex)
            {

                rVM.Status = "Fail";
                rVM.Message = ex.Message.Split(new[] { '\r', '\n' }).FirstOrDefault(); ;

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Rollback();
                }

                FileLogger.Log("TollContInOutDAL", "Insert", ex.ToString() + "\n" + sqlText);

                return rVM;
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

            return rVM;
        }

        public ResultVM UpdateHeaders(TollContInOutVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            ResultVM rVM = new ResultVM();

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            int nextId = 0;

            #endregion

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

                #region Update

                sqlText = "";
                sqlText = "update TollContInOuts set";
                sqlText += "  BranchId              =@BranchId";
                sqlText += " ,DateTime              =@DateTime";
                sqlText += " ,VendorID              =@VendorID";
                sqlText += " ,PeriodID              =@PeriodID";
                sqlText += " ,RefNo                 =@RefNo";
                sqlText += " ,Comments              =@Comments";
                sqlText += " ,ImportID              =@ImportID";
                sqlText += " ,Post                  =@Post";
                sqlText += " ,TransactionType              =@TransactionType";
                sqlText += " ,LastModifiedBy              =@LastModifiedBy";
                sqlText += " ,LastModifiedOn              =@LastModifiedOn";
                sqlText += " ,IsCancle              =@IsCancle";

                sqlText += " WHERE Id           =@Id";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;

                cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);
                cmdUpdate.Parameters.AddWithValue("@BranchId", vm.BranchId);
                cmdUpdate.Parameters.AddWithValue("@DateTime", vm.DateTime);
                cmdUpdate.Parameters.AddWithValue("@VendorID", vm.VendorID);
                cmdUpdate.Parameters.AddWithValue("@PeriodID", vm.PeriodID);
                cmdUpdate.Parameters.AddWithValue("@RefNo", vm.RefNo);
                cmdUpdate.Parameters.AddWithValue("@Comments", vm.Comments);
                cmdUpdate.Parameters.AddWithValue("@ImportID", vm.ImportID);
                cmdUpdate.Parameters.AddWithValue("@Post", vm.Post);
                cmdUpdate.Parameters.AddWithValue("@TransactionType", vm.TransactionType);
                cmdUpdate.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy);
                cmdUpdate.Parameters.AddWithValue("@LastModifiedOn", vm.LastModifiedOn);
                cmdUpdate.Parameters.AddWithValue("@IsCancle", vm.IsCancle);

                transResult = (int)cmdUpdate.ExecuteNonQuery();

                rVM.Status = "Success";
                rVM.Message = "Requested Information successfully Update";
                rVM.Id = vm.Id.ToString();


                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion Commit


                #endregion

            }
            #endregion

            #region Catch
            catch (Exception ex)
            {

                rVM.Status = "Fail";
                rVM.Message = ex.Message.Split(new[] { '\r', '\n' }).FirstOrDefault(); //catch ex

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Rollback();
                }

                FileLogger.Log("TollContInOutDAL", "UpdateHeaders", ex.ToString() + "\n" + sqlText);

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

            return rVM;
        }

        public ResultVM DeleteDetails(TollContInOutVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            ResultVM rVM = new ResultVM();

            int transResult = 0;
            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string retVal = "";
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

                sqlText = "";
                sqlText = "Delete TollContInOutDetails ";
                sqlText += " where HeaderId=@HeaderId";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                cmdUpdate.Parameters.AddWithValue("@HeaderId", vm.Id);
                var exeRes = cmdUpdate.ExecuteNonQuery();
                transResult = Convert.ToInt32(exeRes);

                rVM.Status = "Success";
                rVM.Message = "Data Delete Successfully.";

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

            }
            #endregion

            #region catch and finally

            catch (Exception ex)
            {

                rVM.Status = "Fail";
                rVM.Message = ex.Message.Split(new[] { '\r', '\n' }).FirstOrDefault(); //catch ex

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Rollback();
                }

                FileLogger.Log("TollContInOutDAL", "DeleteDetails", ex.ToString() + "\n" + sqlText);

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

            return rVM;

        }

        public ResultVM SaveDetails(TollContInOutVM Master, List<TollContInOutDetailVM> Details, SqlTransaction Vtransaction, SqlConnection VcurrConn, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            ResultVM rVM = new ResultVM();

            string Id = "";

            int transResult = 0;
            string sqlText = "";

            int IDExist = 0;
            SqlConnection currConn = null;
            SqlTransaction transaction = null;

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
                    transaction = currConn.BeginTransaction(MessageVM.tollOutMsgMethodNameInsert);
                }

                #endregion open connection and transaction

                #region Validation for Header

                if (Master == null)
                {
                    throw new ArgumentNullException(MessageVM.tollOutMsgMethodNameInsert, MessageVM.tollOutMsgNoDataToSave);
                }
                else if (Convert.ToDateTime(Master.DateTime) < DateTime.MinValue || Convert.ToDateTime(Master.DateTime) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException(MessageVM.tollOutMsgMethodNameInsert, "Please Check Invoice Data and Time");
                }

                #endregion Validation for Header

                #region Fiscal Year Check

                string transactionDate = Master.DateTime;
                string transactionYearCheck = Convert.ToDateTime(Master.DateTime).ToString("yyyy-MM-dd");

                #endregion Fiscal Year CHECK

                #region Insert into Details(Insert complete in Header)

                #region Validation for Detail

                if (Details.Count() < 0)
                {
                    throw new ArgumentNullException(MessageVM.tollOutMsgMethodNameInsert, MessageVM.tollOutMsgNoDataToSave);
                }

                #endregion Validation for Detail

                #region Insert Detail Table

                List<TollContInOutDetailVM> detailVms = new List<TollContInOutDetailVM>();

                foreach (var Item in Details.ToList())
                {
                    #region Insert only DetailTable

                    Item.HeaderId = Master.Id;
                    Item.BranchId = Master.BranchId;
                    Item.DateTime = Master.DateTime;
                    Item.Post = Master.Post;
                    Item.TransactionType = Master.TransactionType;
                    Item.IsCancle = Item.IsCancle;
                    Item.ItemNo = Item.ItemNo;
                    Item.Quantity = Convert.ToDecimal((Item.Quantity));
                    Item.UnitCost = Convert.ToDecimal((Item.UnitCost));
                    Item.UOM = Item.UOM;
                    Item.UOMc = Item.UOMc;
                    Item.UOMn = Item.UOMn;
                    //Item.BomId = Item.BomId;
                    Item.SubTotal = Item.SubTotal;
                    Item.PeriodID = "-";

                    rVM = InsertDetails(Item, currConn, transaction, connVM);

                    if (rVM.Status != "Success")
                    {
                        throw new ArgumentNullException(rVM.Message, "");
                    }

                    #endregion Insert only DetailTable
                }

                #endregion Insert Detail Table

                #endregion Insert into Details(Insert complete in Header)

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion Commit

                #region SuccessResult

                rVM.Status = "Success";
                rVM.Message = MessageVM.tollOutMsgSaveSuccessfully;

                #endregion SuccessResult

            }
            #endregion Try

            #region Catch and Finall

            catch (Exception ex)
            {
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Rollback();
                }


                rVM.Status = "Fail";
                rVM.Message = ex.Message;

                FileLogger.Log("TollContInOutDAL", "SaveDetails", ex.ToString() + "\n" + sqlText);

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

            return rVM;

            #endregion Result

        }

        public DataTable SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, string[] ids = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            string sqlTextParameter = "";
            string sqlTextOrderBy = "";
            string sqlTextCount = "";
            DataTable dt = new DataTable();
            DataSet ds = new DataSet();
            string count = "100";
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

                #region sql statement

                #region SqlText

                #region SqlText

                if (count == "All")
                {
                    sqlText = @"SELECT";
                }
                else
                {
                    sqlText = @"SELECT top " + count + " ";
                    //sqlText = @"SELECT";
                }

                sqlText += @"

 Id
,TollContInOuts.BranchId
,Code
,TollContInOuts.VendorID
,DateTime
,PeriodID
,RefNo
,TollContInOuts.Comments
,ImportID
,Post
,TransactionType
,IsCancle
,Vendors.VendorName

FROM TollContInOuts

left outer join Vendors on Vendors.VendorID = TollContInOuts.VendorID

WHERE  1=1
";
                #endregion SqlText

                sqlTextCount += @" 
select count(Id)RecordCount
FROM TollContInOuts 

WHERE  1=1";

                if (ids != null && ids.Length > 0) //JBR
                {
                    int len = ids.Count();
                    string sqlText2 = "";

                    for (int i = 0; i < len; i++)
                    {
                        sqlText2 += "'" + ids[i] + "'";

                        if (i != (len - 1))
                        {
                            sqlText2 += ",";
                        }
                    }

                    if (len == 0)
                    {
                        sqlText2 += "''";
                    }
                    sqlText += " AND Code IN(" + sqlText2 + ")";
                }

                if (Id > 0)
                {
                    sqlTextParameter += @" and Id=@Id";
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


                #endregion SqlText

                sqlTextOrderBy += " order by Code desc, DateTime desc";

                #region SqlExecution
                sqlText = sqlText + " " + sqlTextParameter + " " + sqlTextOrderBy;
                sqlTextCount = sqlTextCount + " " + sqlTextParameter;
                sqlText = sqlText + " " + sqlTextCount;

                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;
                da.SelectCommand.CommandTimeout = 500;

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

            catch (Exception ex)
            {

                FileLogger.Log("TollContInOutDAL", "SelectAll", ex.ToString() + "\n" + sqlText);
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

        public DataTable SelectDetails(string Id, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlTransaction transaction = null;

            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            DataTable dataTable = new DataTable("SearchDetail");

            #endregion

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

                #region SQL Statement

                sqlText = @"
SELECT 
 tcod.Id
,tcod.HeaderId
,tcod.BranchId
,tcod.DateTime
,tcod.PeriodID
,tcod.Post
,tcod.TransactionType
,tcod.IsCancle
,tcod.ItemNo
,p.ProductCode
,p.ProductName
,tcod.Quantity
,tcod.UnitCost
,tcod.UOM
,tcod.UOMc
,tcod.UOMn
--,tcod.BomId
,tcod.SubTotal

FROM TollContInOutDetails tcod
left outer join Products p on p.ItemNo=tcod.ItemNo        
WHERE  1=1

                            ";

                if (!string.IsNullOrWhiteSpace(Id))
                {
                    sqlText += @" and tcod.HeaderId =@HeaderId";
                }


                #endregion

                #region SQL Command

                SqlCommand objCommSaleDetail = new SqlCommand();
                objCommSaleDetail.Connection = currConn;

                objCommSaleDetail.CommandText = sqlText;
                objCommSaleDetail.CommandType = CommandType.Text;




                #endregion.

                #region Parameter
                if (!string.IsNullOrWhiteSpace(Id))
                {
                    objCommSaleDetail.Parameters.AddWithValue("@HeaderId", Id);
                }

                #endregion Parameter

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommSaleDetail);

                //SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                dataAdapter.SelectCommand.Transaction = transaction;
                dataAdapter.SelectCommand.CommandTimeout = 500;

                dataAdapter.Fill(dataTable);
            }
            #endregion

            #region Catch & Finally

            catch (Exception ex)
            {

                FileLogger.Log("TollContInOutDAL", "SelectDetails", ex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", ex.Message.ToString());

            }
            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }

            #endregion

            return dataTable;

        }

        public List<TollContInOutDetailVM> SearchDetailList(string Id, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<TollContInOutDetailVM> VMs = new List<TollContInOutDetailVM>();
            TollContInOutDetailVM vm;
            #endregion

            #region try

            try
            {

                #region sql statement

                #region SqlExecution

                DataTable dt = SelectDetails(Id, VcurrConn, Vtransaction, connVM);

                foreach (DataRow dr in dt.Rows)
                {

                    vm = new TollContInOutDetailVM();

                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.HeaderId = Convert.ToInt32(dr["HeaderId"].ToString());
                    vm.BranchId = Convert.ToInt32(dr["BranchId"].ToString());
                    vm.DateTime = Convert.ToDateTime(dr["DateTime"].ToString()).ToString();
                    vm.PeriodID = dr["PeriodID"].ToString();
                    vm.Post = dr["Post"].ToString();
                    vm.TransactionType = dr["TransactionType"].ToString();
                    vm.IsCancle = Convert.ToBoolean(dr["IsCancle"]);
                    vm.ItemNo = dr["ItemNo"].ToString();
                    vm.ProductCode = dr["ProductCode"].ToString();
                    vm.ProductName = dr["ProductName"].ToString();
                    vm.Quantity = Convert.ToDecimal(dr["Quantity"].ToString());
                    vm.UnitCost = Convert.ToDecimal(dr["UnitCost"].ToString());
                    vm.UOM = dr["UOM"].ToString();
                    vm.UOMc = dr["UOMc"].ToString();
                    vm.UOMn = dr["UOMn"].ToString();
                    //vm.BomId = dr["BomId"].ToString();
                    vm.SubTotal = Convert.ToDecimal(dr["SubTotal"]);

                    VMs.Add(vm);

                }

                #endregion SqlExecution

                #endregion

            }
            #endregion

            #region catch

            catch (Exception ex)
            {
                FileLogger.Log("TollContInOutDAL", "SearchDetailList", ex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", ex.Message.ToString());

            }
            #endregion

            #region finally
            finally
            {

            }
            #endregion

            return VMs;
        }

        public List<TollContInOutVM> SelectAllList(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, IssueMasterVM likeVM = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            string sqlText = "";
            List<TollContInOutVM> VMs = new List<TollContInOutVM>();
            TollContInOutVM vm;
            #endregion
            #region try

            try
            {

                #region sql statement

                #region SqlExecution


                DataTable dt = SelectAll(Id, conditionFields, conditionValues, null, VcurrConn, Vtransaction, connVM);

                if (dt != null && dt.Rows.Count > 0)
                {

                    foreach (DataRow dr in dt.Rows)
                    {

                        try
                        {

                            vm = new TollContInOutVM();
                            vm.Id = Convert.ToInt32(dr["Id"]);
                            vm.Code = dr["Code"].ToString();
                            vm.VendorID = dr["VendorID"].ToString();
                            vm.VendorName = dr["VendorName"].ToString();
                            vm.DateTime = OrdinaryVATDesktop.DateTimeToDate(dr["DateTime"].ToString());
                            vm.RefNo = dr["RefNo"].ToString();
                            vm.Comments = dr["Comments"].ToString();
                            vm.BranchId = Convert.ToInt32(dr["BranchId"].ToString());
                            vm.PeriodID = dr["PeriodID"].ToString();
                            vm.ImportID = dr["ImportID"].ToString();
                            vm.Post = dr["Post"].ToString();
                            vm.IsCancle = (bool)dr["IsCancle"];
                            vm.TransactionType = dr["TransactionType"].ToString();
                            

                            VMs.Add(vm);
                        }

                        catch (Exception e)
                        {

                        }
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

                FileLogger.Log("TollContInOutDAL", "SelectAllList", sqlex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {

                FileLogger.Log("TollContInOutDAL", "SelectAllList", ex.ToString() + "\n" + sqlText);
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

        public ResultVM UpdateData(TollContInOutVM Master, List<TollContInOutDetailVM> Details, SysDBInfoVMTemp connVM = null, string UserId = "", SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            ResultVM rVM = new ResultVM();


            #endregion Initializ

            #region Try
            try
            {
                #region Validation for Header

                if (Master == null)
                {
                    throw new ArgumentNullException(MessageVM.tollOutMethodNameUpdate, MessageVM.tollOutNoDataToUpdate);
                }
                else if (Convert.ToDateTime(Master.DateTime) < DateTime.MinValue || Convert.ToDateTime(Master.DateTime) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException(MessageVM.tollOutMethodNameUpdate, "Please Check Invoice Data and Time");

                }

                #region Add BOMId
                CommonDAL commonDal = new CommonDAL();


                #endregion Add BOMId

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
                    transaction = currConn.BeginTransaction(MessageVM.tollOutMsgMethodNameUpdate);
                }
                #endregion open connection and transaction

                #endregion open connection and transaction

                #region Find Month Lock

                string PeriodName = Convert.ToDateTime(Master.DateTime).ToString("MMMM-yyyy");
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

                string transactionDate = Master.DateTime;
                string transactionYearCheck = Convert.ToDateTime(Master.DateTime).ToString("yyyy-MM-dd");
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

                #region Current Status

                #region Post Status

                string currentPostStatus = "N";

                sqlText = "";
                sqlText = @"

select Code, Post, BranchId from TollContInOuts
where 1=1 
and Code=@Code

";
                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
                cmd.Parameters.AddWithValue("@Code", Master.Code);

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
                    throw new ArgumentNullException(MessageVM.tollOutMsgPostFail, MessageVM.tollOutMsgPostFail);

                }
                #endregion

                #endregion


                #region Find ID for Update

                sqlText = "";
                sqlText = sqlText + "select COUNT(Code) from TollContInOuts WHERE Code=@Code ";
                SqlCommand cmdFindIdUpd = new SqlCommand(sqlText, currConn);
                cmdFindIdUpd.Transaction = transaction;
                cmdFindIdUpd.Parameters.AddWithValue("@Code", Master.Code);
                int IDExist = (int)cmdFindIdUpd.ExecuteScalar();
                if (IDExist <= 0)
                {
                    throw new ArgumentNullException(MessageVM.tollOutMethodNameUpdate, retResults[0]);
                }

                #endregion Find ID for Update


                #region update Header

                string[] cFields = new string[] { "Id" };
                string[] cVals = new string[] { Master.Id.ToString() };

                TollContInOutVM imVM = SelectAllList(0, cFields, cVals, currConn, transaction, null, connVM).FirstOrDefault();

                imVM.DateTime = Master.DateTime;
                imVM.BranchId = Master.BranchId;
                imVM.Code = Master.Code;
                imVM.RefNo = Master.RefNo;
                imVM.Comments = Master.Comments;
                imVM.TransactionType = Master.TransactionType;
                imVM.LastModifiedBy = Master.LastModifiedBy;
                imVM.LastModifiedOn = Master.LastModifiedOn;
                imVM.ImportID = Master.ImportID;
                imVM.VendorID = Master.VendorID;

                sqlText = "";


                ResultVM rvm = UpdateHeaders(imVM, currConn, transaction, connVM);

                if (rvm.Status != "Success")
                {
                    throw new ArgumentNullException(MessageVM.tollOutMethodNameUpdate, retResults[1]);
                }

                #endregion update Header

                #region Update Details

                #region Validation for Detail

                if (Details.Count() < 0)
                {
                    throw new ArgumentNullException(MessageVM.tollOutMethodNameUpdate, MessageVM.tollOutNoDataToUpdate);
                }


                #endregion Validation for Detail
                #region Update Detail Table
                #region Delete Existing Detail Data
                #region Purchase/Receive/Issue Data


                ResultVM deleteDetails = DeleteDetails(imVM, currConn, transaction, connVM);

                #endregion


                #endregion

                ResultVM indertDetails = SaveDetails(Master, Details, transaction, currConn, connVM);

                if (indertDetails.Status != "Success")
                {
                    throw new ArgumentNullException(MessageVM.tollOutMethodNameUpdate, retResults[1]);
                }


                #endregion Update Detail Table
                #endregion
                #region Update PeriodId

                sqlText = "";
                sqlText += @"

UPDATE TollContInOuts 
SET PeriodId=RIGHT('0'+CONVERT(VARCHAR(2),MONTH(DateTime)) +  CONVERT(VARCHAR(4),YEAR(DateTime)),6)
WHERE Id = @Id

update  TollContInOutDetails                             
set PeriodId=RIGHT('0'+CONVERT(VARCHAR(2),MONTH(DateTime)) +  CONVERT(VARCHAR(4),YEAR(DateTime)),6)
WHERE Id = @Id
";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                cmdUpdate.Parameters.AddWithValue("@Id", imVM.Id);
                transResult = cmdUpdate.ExecuteNonQuery();

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


                #endregion Commit

                #region SuccessResult


                rVM = new ResultVM();
                rVM.Status = "Success";
                rVM.Message = MessageVM.tollMsgMethodNameUpdate;
                rVM.Id = Master.Id.ToString();
                rVM.InvoiceNo = Master.Code;

                #endregion SuccessResult

            }
            #endregion Try

            #region Catch and Finall

            catch (Exception ex)
            {
                rVM = new ResultVM();
                rVM.Status = "Fail";
                rVM.Message = ex.Message;

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Rollback();
                }

                FileLogger.Log("TollContInOutDAL", "TollProductionConsumptionUpdate", ex.ToString() + "\n" + sqlText);

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

            return rVM;
            #endregion Result

        }

        public ResultVM PostData(TollContInOutVM Master, List<TollContInOutDetailVM> Details, SqlTransaction Vtransaction = null, SqlConnection VcurrConn = null, SysDBInfoVMTemp connVM = null, string UserId = "")
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
            ResultVM rVM = new ResultVM();

            string PostStatus = "";

            #endregion Initializ

            #region Try
            try
            {


                #region Validation for Header

                if (Master == null)
                {
                    throw new ArgumentNullException(MessageVM.tollOutMsgMethodNamePost, MessageVM.tollOutMsgNoDataToPost);
                }
                else if (Convert.ToDateTime(Master.DateTime) < DateTime.MinValue || Convert.ToDateTime(Master.DateTime) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException(MessageVM.tollOutMsgMethodNamePost, MessageVM.tollOutMsgNoDataToPost);

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

                string PeriodName = Convert.ToDateTime(Master.DateTime).ToString("MMMM-yyyy");
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

                string transactionDate = Master.DateTime;
                string transactionYearCheck = Convert.ToDateTime(Master.DateTime).ToString("yyyy-MM-dd");
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

                #region ID check completed,update Information in Header

                PostVM pvm = new PostVM();
                pvm.LastModifiedBy = Master.LastModifiedBy;
                pvm.LastModifiedOn = Master.LastModifiedOn;
                pvm.Post = Master.Post;
                pvm.Code = Master.Code;

                #endregion ID check completed,update Information in Header

                #region return Current ID and Post Status

                sqlText = "";

                sqlText = sqlText + "update TollContInOuts set Post='Y',LastModifiedBy=@LastModifiedBy,LastModifiedOn=@LastModifiedOn WHERE Id=@Id ";

                sqlText = sqlText + "update TollContInOutDetails set Post='Y' WHERE HeaderId=@HeaderId ";

                SqlCommand cmdIPS = new SqlCommand(sqlText, currConn);
                cmdIPS.Transaction = transaction;
                cmdIPS.Parameters.AddWithValue("@Id", Master.Id);
                cmdIPS.Parameters.AddWithValue("@HeaderId", Master.Id);
                cmdIPS.Parameters.AddWithValue("@LastModifiedBy", pvm.LastModifiedBy);
                cmdIPS.Parameters.AddWithValue("@LastModifiedOn", pvm.LastModifiedOn);
                //PostStatus = (string)cmdIPS.ExecuteScalar();
                //PostStatus = cmdIPS.ExecuteScalar().ToString();

                cmdIPS.ExecuteScalar();

                //if (string.IsNullOrEmpty(PostStatus))
                //{
                //    throw new ArgumentNullException(MessageVM.issueMsgMethodNamePost, MessageVM.issueMsgPostNotSelect);
                //}

                #endregion Prefetch

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion

                #region SuccessResult


                rVM = new ResultVM();
                rVM.Status = "Success";
                rVM.Message = MessageVM.tollMsgSuccessfullyPost;
                rVM.Id = Master.Id.ToString();
                rVM.InvoiceNo = Master.Code;

                #endregion SuccessResult

            }
            #endregion Try

            #region Catch and Finall

            catch (Exception ex)
            {

                rVM = new ResultVM();
                rVM.Status = "Fail";
                rVM.Message = ex.Message;

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Rollback();
                }

                FileLogger.Log("TollContInOutDAL", "DemandPost", ex.ToString() + "\n" + sqlText);
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

            return rVM;

            #endregion Result

        }


    }
}
