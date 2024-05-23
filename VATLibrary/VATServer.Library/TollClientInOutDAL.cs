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
    public class TollClientInOutDAL
    {


        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        private const string FieldDelimeter = DBConstant.FieldDelimeter;


        public ResultVM SaveData(TollClientInOutVM MasterVM, List<TollClientInOutDetailVM> DetailVMs, SqlTransaction Vtransaction
            , SqlConnection VcurrConn, SysDBInfoVMTemp connVM = null)
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
                    throw new ArgumentNullException(MessageVM.tollClientMsgMethodNameInsert, MessageVM.tollClientMsgNoDataToSave);
                }
                else if (Convert.ToDateTime(MasterVM.DateTime) < DateTime.MinValue || Convert.ToDateTime(MasterVM.DateTime) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException(MessageVM.tollClientMsgMethodNameInsert, "Please Check Invoice Data and Time");
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
                sqlText = sqlText + "select COUNT(Code) from TollClientInOuts WHERE Code=@MasterCode ";
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

                if (MasterVM.TransactionType == "TollClient6_4Outs")
                {
                    newIDCreate = commonDal.TransactionCode("Toll", "TollClient6_4Outs", "TollClientInOuts", "Code", "DateTime", MasterVM.DateTime, MasterVM.BranchId.ToString(), currConn, transaction, connVM);
                    var resultCode = commonDal.GetCurrentCode("Toll", "TollClient6_4Outs", MasterVM.DateTime, MasterVM.BranchId.ToString(), currConn, transaction, connVM);
                    var newIdara = resultCode.Split('~');
                    latestId = newIdara[0];
                    fiscalYear = newIdara[1];

                }
                if (MasterVM.TransactionType == "TollClient6_4OutWIP")
                {
                    newIDCreate = commonDal.TransactionCode("Toll", "TollClient6_4OutWIP", "TollClientInOuts", "Code", "DateTime", MasterVM.DateTime, MasterVM.BranchId.ToString(), currConn, transaction, connVM);
                    var resultCode = commonDal.GetCurrentCode("Toll", "TollClient6_4OutWIP", MasterVM.DateTime, MasterVM.BranchId.ToString(), currConn, transaction, connVM);
                    var newIdara = resultCode.Split('~');
                    latestId = newIdara[0];
                    fiscalYear = newIdara[1];

                }
                if (MasterVM.TransactionType == "TollClient6_4OutFG")
                {
                    newIDCreate = commonDal.TransactionCode("Toll", "TollClient6_4OutFG", "TollClientInOuts", "Code", "DateTime", MasterVM.DateTime, MasterVM.BranchId.ToString(), currConn, transaction, connVM);
                    var resultCode = commonDal.GetCurrentCode("Toll", "TollClient6_4OutFG", MasterVM.DateTime, MasterVM.BranchId.ToString(), currConn, transaction, connVM);
                    var newIdara = resultCode.Split('~');
                    latestId = newIdara[0];
                    fiscalYear = newIdara[1];

                }
                if (MasterVM.TransactionType == "TollClient6_4Backs")
                {
                    newIDCreate = commonDal.TransactionCode("Toll", "TollClient6_4Backs", "TollClientInOuts", "Code", "DateTime", MasterVM.DateTime, MasterVM.BranchId.ToString(), currConn, transaction, connVM);
                    var resultCode = commonDal.GetCurrentCode("Toll", "TollClient6_4Backs", MasterVM.DateTime, MasterVM.BranchId.ToString(), currConn, transaction, connVM);
                    var newIdara = resultCode.Split('~');
                    latestId = newIdara[0];
                    fiscalYear = newIdara[1];

                }
                if (MasterVM.TransactionType == "TollClient6_4BacksWIP")
                {
                    newIDCreate = commonDal.TransactionCode("Toll", "TollClient6_4BacksWIP", "TollClientInOuts", "Code", "DateTime", MasterVM.DateTime, MasterVM.BranchId.ToString(), currConn, transaction, connVM);
                    var resultCode = commonDal.GetCurrentCode("Toll", "TollClient6_4BacksWIP", MasterVM.DateTime, MasterVM.BranchId.ToString(), currConn, transaction, connVM);
                    var newIdara = resultCode.Split('~');
                    latestId = newIdara[0];
                    fiscalYear = newIdara[1];

                }
                if (MasterVM.TransactionType == "TollClient6_4BacksFG")
                {
                    newIDCreate = commonDal.TransactionCode("Toll", "TollClient6_4BacksFG", "TollClientInOuts", "Code", "DateTime", MasterVM.DateTime, MasterVM.BranchId.ToString(), currConn, transaction, connVM);
                    var resultCode = commonDal.GetCurrentCode("Toll", "TollClient6_4BacksFG", MasterVM.DateTime, MasterVM.BranchId.ToString(), currConn, transaction, connVM);
                    var newIdara = resultCode.Split('~');
                    latestId = newIdara[0];
                    fiscalYear = newIdara[1];

                }
                if (MasterVM.TransactionType == "TollClient6_4Ins")
                {
                    newIDCreate = commonDal.TransactionCode("Toll", "TollClient6_4Ins", "TollClientInOuts", "Code", "DateTime", MasterVM.DateTime, MasterVM.BranchId.ToString(), currConn, transaction, connVM);
                    var resultCode = commonDal.GetCurrentCode("Toll", "TollClient6_4Ins", MasterVM.DateTime, MasterVM.BranchId.ToString(), currConn, transaction, connVM);
                    var newIdara = resultCode.Split('~');
                    latestId = newIdara[0];
                    fiscalYear = newIdara[1];

                }
                if (MasterVM.TransactionType == "TollClient6_4InsWIP")
                {
                    newIDCreate = commonDal.TransactionCode("Toll", "TollClient6_4InsWIP", "TollClientInOuts", "Code", "DateTime", MasterVM.DateTime, MasterVM.BranchId.ToString(), currConn, transaction, connVM);
                    var resultCode = commonDal.GetCurrentCode("Toll", "TollClient6_4InsWIP", MasterVM.DateTime, MasterVM.BranchId.ToString(), currConn, transaction, connVM);
                    var newIdara = resultCode.Split('~');
                    latestId = newIdara[0];
                    fiscalYear = newIdara[1];

                }

                #endregion

                if (string.IsNullOrEmpty(newIDCreate) || newIDCreate == "")
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, "ID Prefetch not set please update Prefetch first");
                }

                MasterVM.Code = newIDCreate;

                #region checkId and FiscalYear

                sqlText = @"select count(Code) from TollClientInOuts 
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
                    throw new ArgumentNullException(MessageVM.saleMsgNoDataToSave, "");
                }

                #endregion Validation for Detail

                rVM = SaveDetails(MasterVM, DetailVMs, transaction, currConn, connVM);

                if (rVM.Status != "Success")
                {
                    throw new ArgumentNullException(rVM.Message, "");
                }
                #endregion Insert into Details(Insert complete in Header)

                #region Insert Issue

                #region Headers


                if (MasterVM.TransactionType.ToLower() == "tollclient6_4outs" || MasterVM.TransactionType.ToLower() == "tollclient6_4outwip"
                    || MasterVM.TransactionType.ToLower() == "tollclient6_4inswip" || MasterVM.TransactionType.ToLower() == "tollclient6_4backs"
                    || MasterVM.TransactionType.ToLower() == "tollclient6_4backswip")
                {
                    MasterVM.latestId = latestId;
                    MasterVM.fiscalYear = fiscalYear;
                    MasterVM.Code = newIDCreate;

                    rVM = SaveDataToIssue(MasterVM, DetailVMs, transaction, currConn, connVM);

                }

                #endregion

                #endregion

                #region Insert Sales

                if (MasterVM.TransactionType.ToLower() == "tollclient6_4outfg")
                {
                    MasterVM.latestId = latestId;
                    MasterVM.fiscalYear = fiscalYear;
                    MasterVM.Code = newIDCreate;

                    rVM = SaveDataToSales(MasterVM, DetailVMs, transaction, currConn, connVM);

                }

                #endregion

                #region Insert Receive

                if (MasterVM.TransactionType.ToLower() == "tollclient6_4ins" || MasterVM.TransactionType.ToLower() == "tollclient6_4backsfg")
                {
                    MasterVM.latestId = latestId;
                    MasterVM.fiscalYear = fiscalYear;
                    MasterVM.Code = newIDCreate;

                    rVM = SaveDataToReceive(MasterVM, DetailVMs, transaction, currConn, connVM);

                }

                #endregion

                #region Update PeriodId

                sqlText = "";
                sqlText += @"

UPDATE TollClientInOuts 
SET PeriodID=RIGHT('0'+CONVERT(VARCHAR(2),MONTH(DateTime)) +  CONVERT(VARCHAR(4),YEAR(DateTime)),6)
WHERE Id = @Id


UPDATE TollClientInOutDetails 
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

                FileLogger.Log("TollClientInOutDAL", "InsertData", ex.ToString() + "\n" + sqlText);

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

        public ResultVM InsertHeaders(TollClientInOutVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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
INSERT INTO TollClientInOuts(
 BranchId
,Code
,CustomerID
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
,@CustomerID
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
                cmdInsert.Parameters.AddWithValue("@CustomerID", vm.CustomerID);
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

                FileLogger.Log("TollClientInOutDAL", "Insert", ex.ToString() + "\n" + sqlText);

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

        public ResultVM InsertDetails(TollClientInOutDetailVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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
INSERT INTO TollClientInOutDetails(
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
,BomId
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
,@BomId
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
                cmdInsert.Parameters.AddWithValue("@BomId", vm.BomId);
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

                FileLogger.Log("TollClientInOutDAL", "Insert", ex.ToString() + "\n" + sqlText);

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

        public ResultVM UpdateHeaders(TollClientInOutVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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
                sqlText = "update TollClientInOuts set";
                sqlText += "  BranchId              =@BranchId";
                sqlText += " ,DateTime              =@DateTime";
                sqlText += " ,CustomerID            =@CustomerID";
                sqlText += " ,PeriodID              =@PeriodID";
                sqlText += " ,RefNo                 =@RefNo";
                sqlText += " ,Comments              =@Comments";
                sqlText += " ,ImportID              =@ImportID";
                sqlText += " ,Post              =@Post";
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
                cmdUpdate.Parameters.AddWithValue("@CustomerID", vm.CustomerID);
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

                FileLogger.Log("TollClientInOutDAL", "UpdateHeaders", ex.ToString() + "\n" + sqlText);

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

        public ResultVM DeleteDetails(TollClientInOutVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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
                sqlText = "Delete TollClientInOutDetails ";
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

                FileLogger.Log("TollClientInOutDAL", "DeleteDetails", ex.ToString() + "\n" + sqlText);

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

        public ResultVM SaveDetails(TollClientInOutVM Master, List<TollClientInOutDetailVM> Details, SqlTransaction Vtransaction, SqlConnection VcurrConn, SysDBInfoVMTemp connVM = null)
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
                    transaction = currConn.BeginTransaction(MessageVM.tollClientMsgMethodNameInsert);
                }

                #endregion open connection and transaction

                #region Validation for Header

                if (Master == null)
                {
                    throw new ArgumentNullException(MessageVM.tollClientMsgMethodNameInsert, MessageVM.tollClientMsgNoDataToSave);
                }
                else if (Convert.ToDateTime(Master.DateTime) < DateTime.MinValue || Convert.ToDateTime(Master.DateTime) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException(MessageVM.tollClientMsgMethodNameInsert, "Please Check Invoice Data and Time");
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
                    throw new ArgumentNullException(MessageVM.tollClientMsgMethodNameInsert, MessageVM.tollClientMsgNoDataToSave);
                }

                #endregion Validation for Detail

                #region Insert Detail Table

                List<TollClientInOutDetailVM> detailVms = new List<TollClientInOutDetailVM>();

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
                    Item.SubTotal = Item.SubTotal;
                    Item.PeriodID = "-";
                    if (Item.BomId == null)
                    {
                        Item.BomId = 0;
                    }
                    Item.BomId = Item.BomId;

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
                rVM.Message = MessageVM.saleMsgSaveSuccessfully;

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

                FileLogger.Log("TollClientInOutDAL", "SaveDetails", ex.ToString() + "\n" + sqlText);

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
                    //sqlText = @"SELECT top " + count + " ";
                    sqlText = @"SELECT";
                }

                sqlText += @"

--SELECT 
 Id
,TollClientInOuts.BranchId
,Code
,TollClientInOuts.CustomerID
,DateTime
,PeriodID
,RefNo
,TollClientInOuts.Comments
,ImportID
,Post
,TransactionType
,IsCancle

,Customers.CustomerName


FROM TollClientInOuts

left outer join Customers on Customers.CustomerID = TollClientInOuts.CustomerID

WHERE  1=1
";
                #endregion SqlText

                sqlTextCount += @" 
select count(Id)RecordCount
FROM TollClientInOuts 

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

                FileLogger.Log("TollClientInOutDAL", "SelectAll", ex.ToString() + "\n" + sqlText);
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
,tcod.BomId
,tcod.SubTotal

FROM TollClientInOutDetails tcod
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

                FileLogger.Log("TollClientInOutDAL", "SelectDetails", ex.ToString() + "\n" + sqlText);
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

        public List<TollClientInOutDetailVM> SearchDetailList(string Id, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<TollClientInOutDetailVM> VMs = new List<TollClientInOutDetailVM>();
            TollClientInOutDetailVM vm;
            #endregion

            #region try

            try
            {

                #region sql statement

                #region SqlExecution

                DataTable dt = SelectDetails(Id, VcurrConn, Vtransaction, connVM);

                foreach (DataRow dr in dt.Rows)
                {

                    vm = new TollClientInOutDetailVM();

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
                    vm.UOMc = Convert.ToDecimal(dr["UOMc"].ToString());
                    vm.UOMn = dr["UOMn"].ToString();
                    vm.BomId = Convert.ToInt32(dr["BomId"].ToString());
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
                FileLogger.Log("TollClientInOutDAL", "SearchDetailList", ex.ToString() + "\n" + sqlText);
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

        public List<TollClientInOutVM> SelectAllList(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, IssueMasterVM likeVM = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            //SqlConnection currConn = null;
            //SqlTransaction transaction = null;
            string sqlText = "";
            List<TollClientInOutVM> VMs = new List<TollClientInOutVM>();
            TollClientInOutVM vm;
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

                            vm = new TollClientInOutVM();
                            vm.Id = Convert.ToInt32(dr["Id"]);
                            vm.Code = dr["Code"].ToString();
                            vm.CustomerID = dr["CustomerID"].ToString();
                            vm.CustomerName = dr["CustomerName"].ToString();
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

                FileLogger.Log("TollClientInOutDAL", "SelectAllList", sqlex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {

                FileLogger.Log("TollClientInOutDAL", "SelectAllList", ex.ToString() + "\n" + sqlText);
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

        public ResultVM UpdateData(TollClientInOutVM Master, List<TollClientInOutDetailVM> Details, SysDBInfoVMTemp connVM = null, string UserId = "", SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
                    throw new ArgumentNullException(MessageVM.tollClientMsgMethodNameUpdate, MessageVM.tollClientMsgNoDataToUpdate);
                }
                else if (Convert.ToDateTime(Master.DateTime) < DateTime.MinValue || Convert.ToDateTime(Master.DateTime) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException(MessageVM.tollClientMsgMethodNameUpdate, "Please Check Invoice Data and Time");

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
                    transaction = currConn.BeginTransaction(MessageVM.tollClientMsgMethodNameUpdate);
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

select Code, Post, BranchId from TollClientInOuts
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

                    throw new ArgumentNullException(MessageVM.tollClientMsgSuccessfullyPostFail, MessageVM.tollClientMsgSuccessfullyPostFail);

                }
                #endregion

                #endregion

                #region Find ID for Update

                sqlText = "";
                sqlText = sqlText + "select COUNT(Code) from TollClientInOuts WHERE Code=@Code ";
                SqlCommand cmdFindIdUpd = new SqlCommand(sqlText, currConn);
                cmdFindIdUpd.Transaction = transaction;
                cmdFindIdUpd.Parameters.AddWithValue("@Code", Master.Code);
                int IDExist = (int)cmdFindIdUpd.ExecuteScalar();
                if (IDExist <= 0)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, retResults[0]);
                }

                #endregion Find ID for Update

                #region update Header

                string[] cFields = new string[] { "Id" };
                string[] cVals = new string[] { Master.Id.ToString() };

                TollClientInOutVM imVM = SelectAllList(0, cFields, cVals, currConn, transaction, null, connVM).FirstOrDefault();

                imVM.DateTime = Master.DateTime;
                imVM.BranchId = Master.BranchId;
                imVM.Code = Master.Code;
                imVM.RefNo = Master.RefNo;
                imVM.Comments = Master.Comments;
                imVM.TransactionType = Master.TransactionType;
                imVM.LastModifiedBy = Master.LastModifiedBy;
                imVM.LastModifiedOn = Master.LastModifiedOn;
                imVM.ImportID = Master.ImportID;
                imVM.CustomerID = Master.CustomerID;

                sqlText = "";


                ResultVM rvm = UpdateHeaders(imVM, currConn, transaction, connVM);

                if (rvm.Status != "Success")
                {
                    throw new ArgumentNullException(MessageVM.tollClientMsgMethodNameUpdate, retResults[1]);
                }

                #endregion update Header

                #region Update Details

                #region Validation for Detail

                if (Details.Count() < 0)
                {
                    throw new ArgumentNullException(MessageVM.tollClientMsgMethodNameUpdate, MessageVM.tollClientMsgNoDataToSave);
                }

                #endregion Validation for Detail

                #region Update Detail Table

                #region Delete Existing Detail Data

                #region Toll Details delete

                ResultVM deleteDetails = DeleteDetails(imVM, currConn, transaction, connVM);

                #endregion

                #endregion

                ResultVM indertDetails = SaveDetails(Master, Details, transaction, currConn, connVM);

                if (indertDetails.Status != "Success")
                {
                    throw new ArgumentNullException(MessageVM.tollMsgMethodNameUpdate, retResults[1]);
                }

                #endregion Update Detail Table

                #endregion

                #region Update Issue

                if (Master.TransactionType.ToLower() == "tollclient6_4outs" || Master.TransactionType.ToLower() == "tollclient6_4outwip")
                {
                    rVM = UpdateDataToIssue(Master, Details, transaction, currConn, connVM);
                }

                #endregion

                #region Update Sales

                if (Master.TransactionType.ToLower() == "tollclient6_4outfg")
                {
                    rVM = UpdateDataToSales(Master, Details, transaction, currConn, connVM);
                }

                #endregion

                #region Update Receive

                if (Master.TransactionType.ToLower() == "tollclient6_4ins" || Master.TransactionType.ToLower() == "tollclient6_4backsfg")
                {
                    rVM = UpdateDataToReceive(Master, Details, transaction, currConn, connVM);
                }

                #endregion

                #region Update PeriodId

                sqlText = "";
                sqlText += @"

UPDATE TollClientInOuts 
SET PeriodId=RIGHT('0'+CONVERT(VARCHAR(2),MONTH(DateTime)) +  CONVERT(VARCHAR(4),YEAR(DateTime)),6)
WHERE Id = @Id

update  TollClientInOutDetails                             
set PeriodId=RIGHT('0'+CONVERT(VARCHAR(2),MONTH(DateTime)) +  CONVERT(VARCHAR(4),YEAR(DateTime)),6)
WHERE HeaderId = @Id
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

                FileLogger.Log("TollClientInOutDAL", "TollProductionConsumptionUpdate", ex.ToString() + "\n" + sqlText);


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

        public ResultVM PostData(TollClientInOutVM Master, List<TollClientInOutDetailVM> Details, SqlTransaction Vtransaction = null, SqlConnection VcurrConn = null, SysDBInfoVMTemp connVM = null, string UserId = "")
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
                    throw new ArgumentNullException(MessageVM.tollClientMsgSuccessfullyPostFail, MessageVM.tollClientMsgNoDataToPost);
                }
                else if (Convert.ToDateTime(Master.DateTime) < DateTime.MinValue || Convert.ToDateTime(Master.DateTime) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException(MessageVM.tollClientMsgSuccessfullyPostFail, MessageVM.tollClientMsgNoDataToPost);

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

                sqlText = sqlText + "update TollClientInOuts set Post='Y',LastModifiedBy=@LastModifiedBy,LastModifiedOn=@LastModifiedOn WHERE Id=@Id ";

                sqlText = sqlText + "update TollClientInOutDetails set Post='Y' WHERE HeaderId=@HeaderId ";

                if (Master.TransactionType.ToLower() == "tollclient6_4outs" || Master.TransactionType.ToLower() == "tollclient6_4outwip"
                    || Master.TransactionType.ToLower() == "tollclient6_4inswip" || Master.TransactionType.ToLower() == "tollclient6_4backs"
                    || Master.TransactionType.ToLower() == "tollclient6_4backswip")
                {
                    sqlText += @"
update Issueheaders set Post='Y' WHERE IssueNo=@IssueNo 
update IssueDetails set Post='Y' WHERE IssueNo=@IssueNo 
";

                }
                if (Master.TransactionType.ToLower() == "tollclient6_4outfg")
                {
                    sqlText += @"
update SalesInvoiceHeaders set Post='Y' WHERE SalesInvoiceNo=@SalesInvoiceNo 
update SalesInvoiceDetails set Post='Y' WHERE SalesInvoiceNo=@SalesInvoiceNo 
";

                }
                if (Master.TransactionType.ToLower() == "tollclient6_4ins" || Master.TransactionType.ToLower() == "tollclient6_4backsfg")
                {
                    sqlText += @"
update ReceiveHeaders set Post='Y' WHERE ReceiveNo=@ReceiveNo 
update ReceiveDetails set Post='Y' WHERE ReceiveNo=@ReceiveNo 
";
                }

                SqlCommand cmdIPS = new SqlCommand(sqlText, currConn);
                cmdIPS.Transaction = transaction;
                cmdIPS.Parameters.AddWithValue("@Id", Master.Id);
                cmdIPS.Parameters.AddWithValue("@HeaderId", Master.Id);
                cmdIPS.Parameters.AddWithValue("@LastModifiedBy", pvm.LastModifiedBy);
                cmdIPS.Parameters.AddWithValue("@LastModifiedOn", pvm.LastModifiedOn);

                if (Master.TransactionType.ToLower() == "tollclient6_4outs" || Master.TransactionType.ToLower() == "tollclient6_4outwip"
                    || Master.TransactionType.ToLower() == "tollclient6_4inswip" || Master.TransactionType.ToLower() == "tollclient6_4backs"
                    || Master.TransactionType.ToLower() == "tollclient6_4backswip")
                {
                    cmdIPS.Parameters.AddWithValue("@IssueNo", Master.Code);

                }
                if (Master.TransactionType.ToLower() == "tollclient6_4outfg")
                {
                    cmdIPS.Parameters.AddWithValue("@SalesInvoiceNo", Master.Code);

                }
                if (Master.TransactionType.ToLower() == "tollclient6_4ins" || Master.TransactionType.ToLower() == "tollclient6_4backsfg")
                {
                    cmdIPS.Parameters.AddWithValue("@ReceiveNo", Master.Code);

                }

                cmdIPS.ExecuteScalar();


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

                FileLogger.Log("TollClientInOutDAL", "DemandPost", ex.ToString() + "\n" + sqlText);
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

        public ResultVM SaveDataToIssue(TollClientInOutVM MasterVM, List<TollClientInOutDetailVM> DetailVMs, SqlTransaction Vtransaction, SqlConnection VcurrConn, SysDBInfoVMTemp connVM = null)
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

                var latestId = "";
                var fiscalYear = "";

                #region Insert Issue

                #region Insert new Information in Header

                IssueMasterVM IssueVM = new IssueMasterVM();

                IssueVM.IssueNo = MasterVM.Code;
                IssueVM.IssueDateTime = MasterVM.DateTime;
                IssueVM.TotalVATAmount = 0;
                IssueVM.TotalAmount = MasterVM.TotalAmount;
                IssueVM.SerialNo = MasterVM.RefNo;
                IssueVM.Comments = MasterVM.Comments;
                IssueVM.CreatedBy = MasterVM.CreatedBy;
                IssueVM.CreatedOn = MasterVM.CreatedOn;
                IssueVM.LastModifiedBy = MasterVM.CreatedBy;
                IssueVM.LastModifiedOn = MasterVM.CreatedOn;
                IssueVM.ReceiveNo = "0";
                IssueVM.transactionType = MasterVM.TransactionType;
                IssueVM.ReturnId = "0";
                IssueVM.ImportId = MasterVM.ImportID;
                IssueVM.Post = MasterVM.Post;
                IssueVM.BranchId = MasterVM.BranchId;
                IssueVM.AppVersion = MasterVM.AppVersion;
                IssueVM.IssueNumber = Convert.ToInt32(MasterVM.latestId);
                IssueVM.FiscalYear = MasterVM.fiscalYear;
                IssueVM.BranchId = MasterVM.BranchId;

                string[] retResults = new IssueDAL().IssueInsertToMaster(IssueVM, currConn, transaction, connVM);

                if (retResults[0] != "Success")
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, retResults[1]);
                }

                #endregion ID generated completed,Insert new Information in Header

                #region Insert into Details(Insert complete in Header)
                #region Validation for Detail

                if (DetailVMs.Count() < 0)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, MessageVM.issueMsgNoDataToSave);
                }

                #endregion Validation for Detail

                #region Insert Detail Table

                int lineNo = 0;

                foreach (var Item in DetailVMs.ToList())
                {
                    lineNo = lineNo + 1;

                    #region Insert only DetailTable

                    IssueDetailVM iDetVM = new IssueDetailVM();

                    iDetVM.BOMId = Item.BomId;
                    iDetVM.IssueNo = MasterVM.Code;
                    iDetVM.IssueLineNo = lineNo.ToString();
                    iDetVM.ItemNo = Item.ItemNo;
                    iDetVM.Quantity = Item.Quantity;
                    iDetVM.NBRPrice = 0;
                    iDetVM.CostPrice = Item.UnitCost;
                    iDetVM.UOM = Item.UOM;
                    iDetVM.VATRate = 0;
                    iDetVM.VATAmount = 0;
                    iDetVM.SubTotal = Item.SubTotal;
                    iDetVM.CommentsD = "-";
                    iDetVM.CreatedBy = MasterVM.CreatedBy;
                    iDetVM.CreatedOn = MasterVM.CreatedOn;
                    iDetVM.LastModifiedBy = MasterVM.LastModifiedBy;
                    iDetVM.LastModifiedOn = MasterVM.LastModifiedOn;
                    iDetVM.ReceiveNo = "0";
                    iDetVM.IssueDateTime = MasterVM.DateTime;
                    iDetVM.SD = 0;
                    iDetVM.SDAmount = 0;
                    iDetVM.Wastage = 0;
                    iDetVM.BOMDate = MasterVM.DateTime;
                    iDetVM.FinishItemNo = Item.ItemNo;
                    iDetVM.transactionType = MasterVM.TransactionType;
                    iDetVM.IssueReturnId = "0";
                    iDetVM.UOMQty = Item.Quantity;
                    iDetVM.UOMPrice = Item.UnitCost;
                    iDetVM.UOMc = Item.UOMc;
                    iDetVM.UOMn = Item.UOMn;
                    iDetVM.Post = MasterVM.Post;
                    iDetVM.BranchId = MasterVM.BranchId;

                    retResults = new IssueDAL().IssueInsertToDetails(iDetVM, currConn, transaction, connVM);

                    if (retResults[0] != "Success")
                    {
                        throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, retResults[1]);
                    }

                    #endregion Insert only DetailTable

                }


                #endregion Insert Detail Table

                #endregion Insert into Details(Insert complete in Header)

                #endregion

                #region Update PeriodId

                sqlText = "";
                sqlText += @"

UPDATE IssueHeaders 
SET PeriodID=RIGHT('0'+CONVERT(VARCHAR(2),MONTH(IssueDateTime)) +  CONVERT(VARCHAR(4),YEAR(IssueDateTime)),6)
WHERE IssueNo = @IssueNo


UPDATE IssueDetails 
SET PeriodID=RIGHT('0'+CONVERT(VARCHAR(2),MONTH(IssueDateTime)) +  CONVERT(VARCHAR(4),YEAR(IssueDateTime)),6)
WHERE IssueNo = @IssueNo

";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                cmdUpdate.CommandTimeout = 500;
                cmdUpdate.Parameters.AddWithValue("@IssueNo", MasterVM.Code);

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

                FileLogger.Log("TollClientInOutDAL", "SaveDataToIssue", ex.ToString() + "\n" + sqlText);

                throw ex;
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

        public ResultVM UpdateDataToIssue(TollClientInOutVM MasterVM, List<TollClientInOutDetailVM> DetailVMs, SqlTransaction Vtransaction, SqlConnection VcurrConn, SysDBInfoVMTemp connVM = null)
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

                var latestId = "";
                var fiscalYear = "";

                #region Update Issue

                #region Insert new Information in Header

                IssueMasterVM IssueVM = new IssueMasterVM();

                IssueVM.IssueNo = MasterVM.Code;
                IssueVM.IssueDateTime = MasterVM.DateTime;
                IssueVM.TotalVATAmount = 0;
                IssueVM.TotalAmount = MasterVM.TotalAmount;
                IssueVM.SerialNo = MasterVM.RefNo;
                IssueVM.Comments = MasterVM.Comments;
                IssueVM.CreatedBy = MasterVM.CreatedBy;
                IssueVM.CreatedOn = MasterVM.CreatedOn;
                IssueVM.LastModifiedBy = MasterVM.LastModifiedBy;
                IssueVM.LastModifiedOn = MasterVM.LastModifiedOn;
                IssueVM.ReceiveNo = "0";
                IssueVM.transactionType = MasterVM.TransactionType;
                IssueVM.ReturnId = "0";
                IssueVM.ImportId = MasterVM.ImportID;
                IssueVM.Post = MasterVM.Post;
                IssueVM.BranchId = MasterVM.BranchId;
                IssueVM.AppVersion = MasterVM.AppVersion;
                IssueVM.IssueNumber = Convert.ToInt32(MasterVM.latestId);
                IssueVM.FiscalYear = MasterVM.fiscalYear;
                IssueVM.BranchId = MasterVM.BranchId;

                string[] retResults = new IssueDAL().IssueUpdateToMaster(IssueVM, currConn, transaction, connVM);

                if (retResults[0] != "Success")
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, retResults[1]);
                }

                #endregion ID generated completed,Insert new Information in Header

                #region Insert into Details(Insert complete in Header)

                #region Validation for Detail

                if (DetailVMs.Count() < 0)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, MessageVM.issueMsgNoDataToSave);
                }


                #endregion Validation for Detail

                #region Delete Existing Issue Detail Data

                sqlText = "";
                sqlText += @" delete FROM IssueDetails WHERE IssueNo=@MasterIssueNo ";

                SqlCommand cmdDeleteDetail = new SqlCommand(sqlText, currConn);
                cmdDeleteDetail.Transaction = transaction;
                cmdDeleteDetail.Parameters.AddWithValueAndNullHandle("@MasterIssueNo", MasterVM.Code);

                transResult = cmdDeleteDetail.ExecuteNonQuery();

                #endregion

                #region Insert Detail Table

                int lineNo = 0;

                foreach (var Item in DetailVMs.ToList())
                {
                    lineNo = lineNo + 1;

                    #region Insert only DetailTable

                    IssueDetailVM iDetVM = new IssueDetailVM();

                    iDetVM.BOMId = Item.BomId;
                    iDetVM.IssueNo = MasterVM.Code;
                    iDetVM.IssueLineNo = lineNo.ToString();
                    iDetVM.ItemNo = Item.ItemNo;
                    iDetVM.Quantity = Item.Quantity;
                    iDetVM.NBRPrice = 0;
                    iDetVM.CostPrice = Item.UnitCost;
                    iDetVM.UOM = Item.UOM;
                    iDetVM.VATRate = 0;
                    iDetVM.VATAmount = 0;
                    iDetVM.SubTotal = Item.SubTotal;
                    iDetVM.CommentsD = "-";
                    iDetVM.CreatedBy = MasterVM.CreatedBy;
                    iDetVM.CreatedOn = MasterVM.CreatedOn;
                    iDetVM.LastModifiedBy = MasterVM.LastModifiedBy;
                    iDetVM.LastModifiedOn = MasterVM.LastModifiedOn;
                    iDetVM.ReceiveNo = "0";
                    iDetVM.IssueDateTime = MasterVM.DateTime;
                    iDetVM.SD = 0;
                    iDetVM.SDAmount = 0;
                    iDetVM.Wastage = 0;
                    iDetVM.BOMDate = MasterVM.DateTime;
                    iDetVM.FinishItemNo = Item.ItemNo;
                    iDetVM.transactionType = MasterVM.TransactionType;
                    iDetVM.IssueReturnId = "0";
                    iDetVM.UOMQty = Item.Quantity;
                    iDetVM.UOMPrice = Item.UnitCost;
                    iDetVM.UOMc = Item.UOMc;
                    iDetVM.UOMn = Item.UOMn;
                    iDetVM.Post = MasterVM.Post;
                    iDetVM.BranchId = MasterVM.BranchId;

                    retResults = new IssueDAL().IssueInsertToDetails(iDetVM, currConn, transaction, connVM);

                    if (retResults[0] != "Success")
                    {
                        throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, retResults[1]);
                    }

                    #endregion Insert only DetailTable

                }


                #endregion Insert Detail Table

                #endregion Insert into Details(Insert complete in Header)

                #endregion

                #region Update PeriodId

                sqlText = "";
                sqlText += @"

UPDATE IssueHeaders 
SET PeriodID=RIGHT('0'+CONVERT(VARCHAR(2),MONTH(IssueDateTime)) +  CONVERT(VARCHAR(4),YEAR(IssueDateTime)),6)
WHERE IssueNo = @IssueNo


UPDATE IssueDetails 
SET PeriodID=RIGHT('0'+CONVERT(VARCHAR(2),MONTH(IssueDateTime)) +  CONVERT(VARCHAR(4),YEAR(IssueDateTime)),6)
WHERE IssueNo = @IssueNo

";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                cmdUpdate.CommandTimeout = 500;
                cmdUpdate.Parameters.AddWithValue("@IssueNo", MasterVM.Code);

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

                FileLogger.Log("TollClientInOutDAL", "UpdateDataToIssue", ex.ToString() + "\n" + sqlText);
                throw ex;

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

        public ResultVM SaveDataToSales(TollClientInOutVM MasterVM, List<TollClientInOutDetailVM> DetailVMs, SqlTransaction Vtransaction, SqlConnection VcurrConn, SysDBInfoVMTemp connVM = null)
        {

            #region Initializ

            ResultVM rVM = new ResultVM();
            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            int transResult = 0;
            string sqlText = "";

            string Id = "";

            #endregion Initializ

            #region Try
            try
            {

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

                #region Insert new Information in Header

                #region Customers

                string DeliveryAddress = "-";

                CustomerDAL customerDal = new CustomerDAL();

                var customer = customerDal.SelectAllList("0", new[] { "c.CustomerID" }, new[] { MasterVM.CustomerID }, currConn, transaction, connVM).FirstOrDefault();
                if (customer != null)
                {
                    DeliveryAddress = customer.Address1;
                }
                #endregion

                SaleMasterVM saleMaster = new SaleMasterVM();

                saleMaster.SalesInvoiceNo = MasterVM.Code;

                saleMaster.CustomerID = MasterVM.CustomerID;
                saleMaster.DeliveryAddress1 = DeliveryAddress;
                saleMaster.DeliveryAddress2 = DeliveryAddress;
                saleMaster.DeliveryAddress3 = DeliveryAddress;
                saleMaster.InvoiceDateTime = MasterVM.DateTime;
                saleMaster.TotalAmount = MasterVM.TotalAmount;
                saleMaster.SerialNo = MasterVM.RefNo;
                saleMaster.Comments = MasterVM.Comments;
                saleMaster.CreatedBy = MasterVM.CreatedBy;
                saleMaster.CreatedOn = MasterVM.CreatedOn;
                saleMaster.LastModifiedBy = MasterVM.CreatedBy;
                saleMaster.LastModifiedOn = MasterVM.CreatedOn;
                saleMaster.TransactionType = MasterVM.TransactionType;
                saleMaster.DeliveryDate = MasterVM.DateTime;
                saleMaster.ImportIDExcel = MasterVM.ImportID;
                saleMaster.BranchId = MasterVM.BranchId;
                saleMaster.SaleInvoiceNumber = MasterVM.latestId;
                saleMaster.FiscalYear = MasterVM.fiscalYear;

                saleMaster.LCDate = "1900-01-01";
                saleMaster.LCBank = "-";
                saleMaster.VehicleNo = "NA";
                saleMaster.VehicleType = "NA";
                saleMaster.vehicleSaveInDB = true;
                saleMaster.TotalVATAmount = 0;
                saleMaster.VDSAmount = 0;
                saleMaster.SaleType = "New";
                saleMaster.PreviousSalesInvoiceNo = "0";
                saleMaster.Trading = "N";
                saleMaster.IsPrint = "N";
                saleMaster.TenderId = "0";
                saleMaster.Post = "Y";
                saleMaster.CurrencyID = "260";
                saleMaster.CurrencyRateFromBDT = 0;
                saleMaster.ReturnId = "0";
                saleMaster.LCNumber = "-";
                saleMaster.PINo = "-";
                saleMaster.PIDate = "1900-01-01";
                saleMaster.ShiftId = "0";
                saleMaster.EXPFormNo = "-";
                saleMaster.EXPFormDate = "1900-01-01";
                saleMaster.IsDeemedExport = "N";
                saleMaster.HPSTotalAmount = 0;
                saleMaster.IsVDS = "N";
                saleMaster.BOe = "-";
                saleMaster.OrderNumber = "0";
                saleMaster.DeductionAmount = 0;
                saleMaster.AppVersion = "";
                saleMaster.BOeDate = "1900-01-01";

                string[] retResults = new SaleDAL().SalesInsertToMaster(saleMaster, currConn, transaction, connVM, null);

                if (retResults[0] != "Success")
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, retResults[1]);
                }

                #endregion ID generated completed,Insert new Information in Header

                #region Validation for Detail

                if (DetailVMs.Count() < 0)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, MessageVM.issueMsgNoDataToSave);
                }

                #endregion Validation for Detail

                #region Insert Detail Table

                int lineNo = 0;

                foreach (var Item in DetailVMs.ToList())
                {
                    #region Insert only DetailTable

                    SaleDetailVm sVM = new SaleDetailVm();

                    sVM.SalesInvoiceNo = MasterVM.Code;
                    sVM.ItemNo = Item.ItemNo;
                    sVM.Quantity = Item.Quantity;
                    sVM.SalesPrice = Item.UnitCost;
                    sVM.NBRPrice = Item.UnitCost;
                    sVM.SubTotal = Convert.ToDecimal((Item.SubTotal));
                    sVM.CreatedBy = MasterVM.CreatedBy;
                    sVM.CreatedOn = MasterVM.CreatedOn;
                    sVM.LastModifiedBy = MasterVM.LastModifiedBy;
                    sVM.LastModifiedOn = MasterVM.LastModifiedOn;
                    sVM.InvoiceDateTime = MasterVM.DateTime;
                    sVM.TransactionType = MasterVM.TransactionType;
                    sVM.UOMQty = Convert.ToDecimal((Item.Quantity));
                    sVM.UOMc = Convert.ToDecimal((Item.UOMc));
                    sVM.CurrencyValue = Convert.ToDecimal((Item.SubTotal));
                    sVM.UOMPrice = Convert.ToDecimal((Item.UnitCost));
                    sVM.BranchId = MasterVM.BranchId;
                    sVM.PromotionalQuantity = 0;
                    sVM.AvgRate = 0;
                    sVM.VATRate = 0;
                    sVM.VATAmount = 0;
                    sVM.SD = 0;
                    sVM.SDAmount = 0;
                    sVM.VDSAmountD = 0;
                    sVM.ReturnId = "0";
                    sVM.Post = "Y";
                    sVM.DiscountAmount = 0;
                    sVM.DiscountedNBRPrice = 0;
                    sVM.DollerValue = 0;
                    sVM.ProductDescription = "-";
                    sVM.IsFixedVAT = "N";
                    sVM.FixedVATAmount = 0;
                    sVM.BENumber = "-";
                    sVM.BillingPeriodFrom = "1900-01-01";
                    sVM.BillingPeriodTo = "1900-01-01";
                    sVM.BillingDays = 0;
                    sVM.ProductType = "";
                    sVM.CPCName = "";
                    sVM.BEItemNo = "";
                    sVM.HSCode = "";
                    sVM.Option1 = "";
                    sVM.PreviousInvoiceDateTime = "1900-01-01";
                    sVM.PreviousNBRPrice = 0;
                    sVM.PreviousQuantity = 0;
                    sVM.PreviousUOM = "";
                    sVM.PreviousSubTotal = 0;
                    sVM.PreviousVATAmount = 0;
                    sVM.PreviousVATRate = 0;
                    sVM.PreviousSD = 0;
                    sVM.PreviousSDAmount = 0;
                    sVM.ReasonOfReturn = "NA";
                    sVM.IsLeader = "N";
                    sVM.LeaderAmount = 0;
                    sVM.LeaderVATAmount = 0;
                    sVM.NonLeaderAmount = 0;
                    sVM.NonLeaderVATAmount = 0;
                    sVM.HPSRate = 0;

                    string[] rResults = new SaleDAL().SalesInsertToDetail(sVM, currConn, transaction, connVM);

                    if (rResults[0] != "Success")
                    {
                        throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, rResults[1]);
                    }

                    #endregion Insert only DetailTable
                }

                #endregion Insert Detail Table

                #region Update PeriodId

                sqlText = "";
                sqlText += @"

UPDATE SalesInvoiceHeaders 
SET PeriodID=RIGHT('0'+CONVERT(VARCHAR(2),MONTH(InvoiceDateTime)) +  CONVERT(VARCHAR(4),YEAR(InvoiceDateTime)),6)
WHERE SalesInvoiceNo = @SalesInvoiceNo


UPDATE SalesInvoiceDetails 
SET PeriodID=RIGHT('0'+CONVERT(VARCHAR(2),MONTH(InvoiceDateTime)) +  CONVERT(VARCHAR(4),YEAR(InvoiceDateTime)),6)
WHERE SalesInvoiceNo = @SalesInvoiceNo

";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                cmdUpdate.CommandTimeout = 500;
                cmdUpdate.Parameters.AddWithValue("@SalesInvoiceNo", MasterVM.Code);

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

                FileLogger.Log("TollClientInOutDAL", "SaveDataToIssue", ex.ToString() + "\n" + sqlText);

                throw ex;
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

        public ResultVM UpdateDataToSales(TollClientInOutVM MasterVM, List<TollClientInOutDetailVM> DetailVMs, SqlTransaction Vtransaction, SqlConnection VcurrConn, SysDBInfoVMTemp connVM = null)
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

                var latestId = "";
                var fiscalYear = "";

                #region Update Information in Header

                #region Customers

                string DeliveryAddress = "-";

                CustomerDAL customerDal = new CustomerDAL();

                var customer = customerDal.SelectAllList("0", new[] { "c.CustomerID" }, new[] { MasterVM.CustomerID }, currConn, transaction, connVM).FirstOrDefault();
                if (customer != null)
                {
                    DeliveryAddress = customer.Address1;
                }
                #endregion

                SaleMasterVM svm = new SaleDAL().SelectAllList(0, new[] { "sih.SalesInvoiceNo" }, new[] { MasterVM.Code }, currConn, transaction, null, connVM).FirstOrDefault();


                SaleMasterVM saleMaster = new SaleMasterVM();

                saleMaster.Id = svm.Id;
                saleMaster.SalesInvoiceNo = MasterVM.Code;
                saleMaster.CustomerID = MasterVM.CustomerID;
                saleMaster.DeliveryAddress1 = DeliveryAddress;
                saleMaster.DeliveryAddress2 = DeliveryAddress;
                saleMaster.DeliveryAddress3 = DeliveryAddress;
                saleMaster.InvoiceDateTime = MasterVM.DateTime;
                saleMaster.TotalAmount = MasterVM.TotalAmount;
                saleMaster.SerialNo = MasterVM.RefNo;
                saleMaster.Comments = MasterVM.Comments;
                saleMaster.CreatedBy = MasterVM.CreatedBy;
                saleMaster.CreatedOn = MasterVM.CreatedOn;
                saleMaster.LastModifiedBy = MasterVM.CreatedBy;
                saleMaster.LastModifiedOn = MasterVM.CreatedOn;
                saleMaster.TransactionType = MasterVM.TransactionType;
                saleMaster.DeliveryDate = MasterVM.DateTime;
                saleMaster.ImportIDExcel = MasterVM.ImportID;
                saleMaster.BranchId = MasterVM.BranchId;
                saleMaster.SaleInvoiceNumber = MasterVM.latestId;
                saleMaster.FiscalYear = MasterVM.fiscalYear;

                saleMaster.LCDate = "1900-01-01";
                saleMaster.LCBank = "-";
                saleMaster.VehicleNo = "NA";
                saleMaster.VehicleType = "NA";
                saleMaster.vehicleSaveInDB = true;
                saleMaster.TotalVATAmount = 0;
                saleMaster.VDSAmount = 0;
                saleMaster.SaleType = "New";
                saleMaster.PreviousSalesInvoiceNo = "0";
                saleMaster.Trading = "N";
                saleMaster.IsPrint = "N";
                saleMaster.TenderId = "0";
                saleMaster.Post = "Y";
                saleMaster.CurrencyID = "260";
                saleMaster.CurrencyRateFromBDT = 0;
                saleMaster.ReturnId = "0";
                saleMaster.LCNumber = "-";
                saleMaster.PINo = "-";
                saleMaster.PIDate = "1900-01-01";
                saleMaster.ShiftId = "0";
                saleMaster.EXPFormNo = "-";
                saleMaster.EXPFormDate = "1900-01-01";
                saleMaster.IsDeemedExport = "N";
                saleMaster.HPSTotalAmount = 0;
                saleMaster.IsVDS = "N";
                saleMaster.BOe = "-";
                saleMaster.OrderNumber = "0";
                saleMaster.DeductionAmount = 0;
                saleMaster.AppVersion = "";
                saleMaster.BOeDate = "1900-01-01";

                string[] retResults = new SaleDAL().SalesUpdateToMaster(saleMaster, currConn, transaction, connVM);

                if (retResults[0] != "Success")
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, retResults[1]);
                }

                #endregion Update Information in Header

                #region Validation for Detail

                if (DetailVMs.Count() < 0)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, MessageVM.issueMsgNoDataToSave);
                }


                #endregion Validation for Detail

                #region Delete Existing Detail Data

                sqlText = "";
                sqlText += @" delete FROM SalesInvoiceDetails WHERE SalesInvoiceNo=@SalesInvoiceNo ";

                SqlCommand cmdDeleteDetail = new SqlCommand(sqlText, currConn);
                cmdDeleteDetail.Transaction = transaction;
                cmdDeleteDetail.Parameters.AddWithValueAndNullHandle("@SalesInvoiceNo", MasterVM.Code);

                transResult = cmdDeleteDetail.ExecuteNonQuery();

                #endregion

                #region Insert Detail Table

                int lineNo = 0;

                foreach (var Item in DetailVMs.ToList())
                {
                    #region Insert only DetailTable

                    SaleDetailVm sVM = new SaleDetailVm();

                    sVM.SalesInvoiceNo = MasterVM.Code;
                    sVM.ItemNo = Item.ItemNo;
                    sVM.Quantity = Item.Quantity;
                    sVM.SalesPrice = Item.UnitCost;
                    sVM.NBRPrice = Item.UnitCost;
                    sVM.SubTotal = Convert.ToDecimal((Item.SubTotal));
                    sVM.CreatedBy = MasterVM.CreatedBy;
                    sVM.CreatedOn = MasterVM.CreatedOn;
                    sVM.LastModifiedBy = MasterVM.LastModifiedBy;
                    sVM.LastModifiedOn = MasterVM.LastModifiedOn;
                    sVM.InvoiceDateTime = MasterVM.DateTime;
                    sVM.TransactionType = MasterVM.TransactionType;
                    sVM.UOMQty = Convert.ToDecimal((Item.Quantity));
                    sVM.UOMc = Convert.ToDecimal((Item.UOMc));
                    sVM.CurrencyValue = Convert.ToDecimal((Item.SubTotal));
                    sVM.UOMPrice = Convert.ToDecimal((Item.UnitCost));
                    sVM.BranchId = MasterVM.BranchId;
                    sVM.PromotionalQuantity = 0;
                    sVM.AvgRate = 0;
                    sVM.VATRate = 0;
                    sVM.VATAmount = 0;
                    sVM.SD = 0;
                    sVM.SDAmount = 0;
                    sVM.VDSAmountD = 0;
                    sVM.ReturnId = "0";
                    sVM.Post = "Y";
                    sVM.DiscountAmount = 0;
                    sVM.DiscountedNBRPrice = 0;
                    sVM.DollerValue = 0;
                    sVM.ProductDescription = "-";
                    sVM.IsFixedVAT = "N";
                    sVM.FixedVATAmount = 0;
                    sVM.BENumber = "-";
                    sVM.BillingPeriodFrom = "1900-01-01";
                    sVM.BillingPeriodTo = "1900-01-01";
                    sVM.BillingDays = 0;
                    sVM.ProductType = "";
                    sVM.CPCName = "";
                    sVM.BEItemNo = "";
                    sVM.HSCode = "";
                    sVM.Option1 = "";
                    sVM.PreviousInvoiceDateTime = "1900-01-01";
                    sVM.PreviousNBRPrice = 0;
                    sVM.PreviousQuantity = 0;
                    sVM.PreviousUOM = "";
                    sVM.PreviousSubTotal = 0;
                    sVM.PreviousVATAmount = 0;
                    sVM.PreviousVATRate = 0;
                    sVM.PreviousSD = 0;
                    sVM.PreviousSDAmount = 0;
                    sVM.ReasonOfReturn = "NA";
                    sVM.IsLeader = "N";
                    sVM.LeaderAmount = 0;
                    sVM.LeaderVATAmount = 0;
                    sVM.NonLeaderAmount = 0;
                    sVM.NonLeaderVATAmount = 0;
                    sVM.HPSRate = 0;

                    string[] rResults = new SaleDAL().SalesInsertToDetail(sVM, currConn, transaction, connVM);

                    if (rResults[0] != "Success")
                    {
                        throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, rResults[1]);
                    }

                    #endregion Insert only DetailTable
                }

                #endregion Insert Detail Table

                #region Update PeriodId

                sqlText = "";
                sqlText += @"

UPDATE SalesInvoiceHeaders 
SET PeriodID=RIGHT('0'+CONVERT(VARCHAR(2),MONTH(InvoiceDateTime)) +  CONVERT(VARCHAR(4),YEAR(InvoiceDateTime)),6)
WHERE SalesInvoiceNo = @SalesInvoiceNo


UPDATE SalesInvoiceDetails 
SET PeriodID=RIGHT('0'+CONVERT(VARCHAR(2),MONTH(InvoiceDateTime)) +  CONVERT(VARCHAR(4),YEAR(InvoiceDateTime)),6)
WHERE SalesInvoiceNo = @SalesInvoiceNo

";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                cmdUpdate.CommandTimeout = 500;
                cmdUpdate.Parameters.AddWithValue("@SalesInvoiceNo", MasterVM.Code);

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

                FileLogger.Log("TollClientInOutDAL", "UpdateDataToSales", ex.ToString() + "\n" + sqlText);
                throw ex;

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

        public ResultVM SaveDataToReceive(TollClientInOutVM MasterVM, List<TollClientInOutDetailVM> DetailVMs, SqlTransaction Vtransaction, SqlConnection VcurrConn, SysDBInfoVMTemp connVM = null)
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

                var latestId = "";
                var fiscalYear = "";

                #region Insert Issue

                #region Insert new Information in Header

                ReceiveMasterVM rmVM = new ReceiveMasterVM();
                rmVM.ReceiveNo = MasterVM.Code;
                rmVM.ReceiveDateTime = MasterVM.DateTime;
                rmVM.TotalAmount = MasterVM.TotalAmount;
                rmVM.SerialNo = MasterVM.RefNo;
                rmVM.Comments = MasterVM.Comments;
                rmVM.CreatedBy = MasterVM.CreatedBy;
                rmVM.CreatedOn = MasterVM.CreatedOn;
                rmVM.LastModifiedBy = MasterVM.LastModifiedBy;
                rmVM.LastModifiedOn = MasterVM.LastModifiedOn;
                rmVM.transactionType = MasterVM.TransactionType;
                rmVM.ImportId = MasterVM.ImportID;
                rmVM.ReferenceNo = MasterVM.RefNo;
                rmVM.CustomerID = MasterVM.CustomerID;
                rmVM.ReceiveNumber = MasterVM.latestId;
                rmVM.FiscalYear = MasterVM.fiscalYear;
                rmVM.AppVersion = MasterVM.AppVersion;
                rmVM.BranchId = MasterVM.BranchId;
                rmVM.ReturnId = "0";
                rmVM.TotalVATAmount = 0;
                rmVM.WithToll = "N";
                rmVM.ShiftId = "0";
                rmVM.IssueFromBOM = "Y";
                rmVM.Post = "Y";

                string[] retResults = new ReceiveDAL().ReceiveInsertToMaster(rmVM, currConn, transaction, connVM);

                if (retResults[0] != "Success")
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, retResults[1]);
                }

                #endregion ID generated completed,Insert new Information in Header

                #region Insert into Details(Insert complete in Header)
                #region Validation for Detail

                if (DetailVMs.Count() < 0)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, MessageVM.issueMsgNoDataToSave);
                }

                #endregion Validation for Detail

                #region Insert Detail Table

                #region Currency Conversion

                CurrencyConversionDAL currencyConversionDal = new CurrencyConversionDAL();

                CurrencyConversionVM currencyVM = new CurrencyConversionVM();
                currencyVM.CurrencyCodeF = "USD";
                currencyVM.CurrencyCodeT = "BDT";
                currencyVM.ActiveStatus = "Y";
                currencyVM.ConvertionDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                DataTable CurrencyConversionResult = currencyConversionDal.ConvRate(currencyVM, null, null);

                #endregion

                int lineNo = 0;

                foreach (var Item in DetailVMs.ToList())
                {
                    lineNo = lineNo + 1;

                    #region Insert only DetailTable

                    ReceiveDetailVM rdVM = new ReceiveDetailVM();
                    rdVM.ReceiveNo = MasterVM.Code;
                    rdVM.ReceiveLineNo = lineNo.ToString();
                    rdVM.ItemNo = Item.ItemNo;
                    rdVM.Quantity = Item.Quantity;
                    rdVM.CostPrice = Item.UnitCost;
                    rdVM.NBRPrice = Item.UnitCost;
                    rdVM.UOM = Item.UOM;
                    rdVM.CurrencyValue = Item.SubTotal;
                    rdVM.SubTotal = Item.SubTotal;
                    rdVM.CreatedBy = MasterVM.CreatedBy;
                    rdVM.CreatedOn = MasterVM.CreatedOn;
                    rdVM.LastModifiedBy = MasterVM.LastModifiedBy;
                    rdVM.LastModifiedOn = MasterVM.LastModifiedOn;
                    rdVM.ReceiveDateTime = MasterVM.DateTime;
                    rdVM.transactionType = MasterVM.TransactionType;
                    rdVM.BOMId = Item.BomId;
                    rdVM.UOMPrice = Item.UnitCost;
                    rdVM.UOMQty = Item.Quantity;
                    rdVM.BranchId = MasterVM.BranchId;
                    rdVM.UOMn = Item.UOMn;
                    rdVM.UOMc = Item.UOMc;
                    rdVM.VATRate = 0;
                    rdVM.VATAmount = 0;
                    rdVM.CommentsD = "-";
                    rdVM.SD = 0;
                    rdVM.SDAmount = 0;
                    rdVM.ReturnId = "0";
                    rdVM.VatName = "";
                    rdVM.Weight = "0";
                    rdVM.Post = "Y";

                    if (CurrencyConversionResult != null)
                    {
                        decimal currencyRate = Convert.ToDecimal(CurrencyConversionResult.Rows[0]["CurrencyRate"].ToString());
                        decimal dollerValue = Item.SubTotal / currencyRate;
                        retResults[1] = dollerValue.ToString();
                        rdVM.DollerValue = dollerValue;
                    }

                    retResults = new ReceiveDAL().ReceiveInsertToDetail(rdVM, VcurrConn, Vtransaction, connVM);

                    if (retResults[0] != "Success")
                    {
                        throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert, retResults[1]);
                    }
                    #endregion Insert only DetailTable  //done

                }


                #endregion Insert Detail Table

                #endregion Insert into Details(Insert complete in Header)

                #endregion

                #region Update PeriodId

                sqlText = "";
                sqlText += @"

UPDATE ReceiveHeaders 
SET PeriodID=RIGHT('0'+CONVERT(VARCHAR(2),MONTH(ReceiveDateTime)) +  CONVERT(VARCHAR(4),YEAR(ReceiveDateTime)),6)
WHERE ReceiveNo = @ReceiveNo


UPDATE ReceiveDetails 
SET PeriodID=RIGHT('0'+CONVERT(VARCHAR(2),MONTH(ReceiveDateTime)) +  CONVERT(VARCHAR(4),YEAR(ReceiveDateTime)),6)
WHERE ReceiveNo = @ReceiveNo

";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                cmdUpdate.CommandTimeout = 500;
                cmdUpdate.Parameters.AddWithValue("@ReceiveNo", MasterVM.Code);

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

                FileLogger.Log("TollClientInOutDAL", "SaveDataToReceive", ex.ToString() + "\n" + sqlText);

                throw ex;
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

        public ResultVM UpdateDataToReceive(TollClientInOutVM MasterVM, List<TollClientInOutDetailVM> DetailVMs, SqlTransaction Vtransaction, SqlConnection VcurrConn, SysDBInfoVMTemp connVM = null)
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

                var latestId = "";
                var fiscalYear = "";

                #region Update Issue

                string[] conditionFields = { "rh.ReceiveNo" };
                string[] conditionValues = { MasterVM.Code };


                ReceiveMasterVM rMasterVM = new ReceiveDAL().SelectAllList(0, conditionFields, conditionValues, currConn, transaction, null, connVM).FirstOrDefault();

                #region Insert new Information in Header

                ReceiveMasterVM rmVM = new ReceiveMasterVM();

                rmVM.Id = rMasterVM.Id;
                rmVM.ReceiveNo = MasterVM.Code;
                rmVM.ReceiveDateTime = MasterVM.DateTime;
                rmVM.TotalAmount = MasterVM.TotalAmount;
                rmVM.SerialNo = MasterVM.RefNo;
                rmVM.Comments = MasterVM.Comments;
                rmVM.CreatedBy = MasterVM.CreatedBy;
                rmVM.CreatedOn = MasterVM.CreatedOn;
                rmVM.LastModifiedBy = MasterVM.LastModifiedBy;
                rmVM.LastModifiedOn = MasterVM.LastModifiedOn;
                rmVM.transactionType = MasterVM.TransactionType;
                rmVM.ImportId = MasterVM.ImportID;
                rmVM.ReferenceNo = MasterVM.RefNo;
                rmVM.CustomerID = MasterVM.CustomerID;
                rmVM.ReceiveNumber = MasterVM.latestId;
                rmVM.FiscalYear = MasterVM.fiscalYear;
                rmVM.AppVersion = MasterVM.AppVersion;
                rmVM.BranchId = MasterVM.BranchId;
                rmVM.ReturnId = "0";
                rmVM.TotalVATAmount = 0;
                rmVM.WithToll = "N";
                rmVM.ShiftId = "0";
                rmVM.IssueFromBOM = "Y";
                rmVM.Post = "Y";

                string[] retResults = new ReceiveDAL().ReceiveUpdateToMaster(rmVM, currConn, transaction, connVM);

                if (retResults[0] != "Success")
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, retResults[1]);
                }

                #endregion ID generated completed,Insert new Information in Header

                #region Insert into Details(Insert complete in Header)

                #region Currency Conversion

                CurrencyConversionDAL currencyConversionDal = new CurrencyConversionDAL();

                CurrencyConversionVM currencyVM = new CurrencyConversionVM();
                currencyVM.CurrencyCodeF = "USD";
                currencyVM.CurrencyCodeT = "BDT";
                currencyVM.ActiveStatus = "Y";
                currencyVM.ConvertionDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                DataTable CurrencyConversionResult = currencyConversionDal.ConvRate(currencyVM, null, null);

                #endregion

                #region Validation for Detail

                if (DetailVMs.Count() < 0)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, MessageVM.issueMsgNoDataToSave);
                }

                #endregion Validation for Detail

                #region Delete Existing Issue Detail Data

                sqlText = "";
                sqlText += @" delete FROM ReceiveDetails WHERE ReceiveNo=@MasterReceiveNo ";

                SqlCommand cmdDeleteDetail = new SqlCommand(sqlText, currConn);
                cmdDeleteDetail.Transaction = transaction;
                cmdDeleteDetail.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", MasterVM.Code);

                transResult = cmdDeleteDetail.ExecuteNonQuery();

                #endregion

                #region Insert Detail Table

                int lineNo = 0;

                foreach (var Item in DetailVMs.ToList())
                {
                    lineNo = lineNo + 1;

                    #region Insert only DetailTable

                    ReceiveDetailVM rdVM = new ReceiveDetailVM();
                    rdVM.ReceiveNo = MasterVM.Code;
                    rdVM.ReceiveLineNo = lineNo.ToString();
                    rdVM.ItemNo = Item.ItemNo;
                    rdVM.Quantity = Item.Quantity;
                    rdVM.CostPrice = Item.UnitCost;
                    rdVM.NBRPrice = Item.UnitCost;
                    rdVM.UOM = Item.UOM;
                    rdVM.CurrencyValue = Item.SubTotal;
                    rdVM.SubTotal = Item.SubTotal;
                    rdVM.CreatedBy = MasterVM.CreatedBy;
                    rdVM.CreatedOn = MasterVM.CreatedOn;
                    rdVM.LastModifiedBy = MasterVM.LastModifiedBy;
                    rdVM.LastModifiedOn = MasterVM.LastModifiedOn;
                    rdVM.ReceiveDateTime = MasterVM.DateTime;
                    rdVM.transactionType = MasterVM.TransactionType;
                    rdVM.BOMId = Item.BomId;
                    rdVM.UOMPrice = Item.UnitCost;
                    rdVM.UOMQty = Item.Quantity;
                    rdVM.BranchId = MasterVM.BranchId;
                    rdVM.UOMn = Item.UOMn;
                    rdVM.UOMc = Item.UOMc;
                    rdVM.VATRate = 0;
                    rdVM.VATAmount = 0;
                    rdVM.CommentsD = "-";
                    rdVM.SD = 0;
                    rdVM.SDAmount = 0;
                    rdVM.ReturnId = "0";
                    rdVM.VatName = "";
                    rdVM.Weight = "0";
                    rdVM.Post = "Y";

                    if (CurrencyConversionResult != null)
                    {
                        decimal currencyRate = Convert.ToDecimal(CurrencyConversionResult.Rows[0]["CurrencyRate"].ToString());
                        decimal dollerValue = Item.SubTotal / currencyRate;
                        retResults[1] = dollerValue.ToString();
                        rdVM.DollerValue = dollerValue;
                    }

                    retResults = new ReceiveDAL().ReceiveInsertToDetail(rdVM, VcurrConn, Vtransaction, connVM);

                    if (retResults[0] != "Success")
                    {
                        throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert, retResults[1]);
                    }
                    #endregion Insert only DetailTable  //done

                }


                #endregion Insert Detail Table

                #endregion Insert into Details(Insert complete in Header)

                #endregion

                #region Update PeriodId

                sqlText = "";
                sqlText += @"

UPDATE ReceiveHeaders 
SET PeriodID=RIGHT('0'+CONVERT(VARCHAR(2),MONTH(ReceiveDateTime)) +  CONVERT(VARCHAR(4),YEAR(ReceiveDateTime)),6)
WHERE ReceiveNo = @ReceiveNo


UPDATE ReceiveDetails 
SET PeriodID=RIGHT('0'+CONVERT(VARCHAR(2),MONTH(ReceiveDateTime)) +  CONVERT(VARCHAR(4),YEAR(ReceiveDateTime)),6)
WHERE ReceiveNo = @ReceiveNo

";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                cmdUpdate.CommandTimeout = 500;
                cmdUpdate.Parameters.AddWithValue("@ReceiveNo", MasterVM.Code);

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

                FileLogger.Log("TollClientInOutDAL", "UpdateDataToReceive", ex.ToString() + "\n" + sqlText);
                throw ex;

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
