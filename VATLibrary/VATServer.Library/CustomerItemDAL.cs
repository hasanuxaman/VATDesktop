using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VATServer.Ordinary;
using VATViewModel.DTOs;

namespace VATServer.Library
{
    public class CustomerItemDAL
    {
        #region Declarations

        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        ProductDAL _ProductDAL = new ProductDAL();
        CommonDAL commonDal = new CommonDAL();

        #endregion


        public string[] CustomerItemInsert(CustomerItemVM Master, List<CustomerItemDetailsVM> Details, SqlTransaction Vtransaction, SqlConnection VcurrConn, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";
            var Id = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            int transResult = 0;
            string sqlText = "";
            string newIDCreate = "";

            int IDExist = 0;

            #endregion Initializ

            #region Try
            try
            {

                #region Validation for Header

                if (Master == null)
                {
                    throw new ArgumentNullException("", MessageVM.receiveMsgNoDataToSave);
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

                #region Find Transaction Exist

                sqlText = "";
                sqlText = sqlText + @"  select max(Id) from CustomerItemHeaders ";
                SqlCommand cmdExistTran = new SqlCommand(sqlText, currConn);
                cmdExistTran.Transaction = transaction;
                ////cmdExistTran.Parameters.AddWithValueAndNullHandle("@MasterInvoiceNo", Master.InvoiceNo);

                IDExist = (int)cmdExistTran.ExecuteScalar();

                newIDCreate = (IDExist + 1).ToString();

                #endregion Find Transaction Exist

                #region Find Customer Exist

                sqlText = "";
                sqlText = sqlText + @"select COUNT(InvoiceNo) from CustomerItemHeaders WHERE CustomerID=@MasterCustomerID ";
                cmdExistTran = new SqlCommand(sqlText, currConn);
                cmdExistTran.Transaction = transaction;
                cmdExistTran.Parameters.AddWithValueAndNullHandle("@MasterCustomerID", Master.CustomerID);

                IDExist = (int)cmdExistTran.ExecuteScalar();

                if (IDExist > 0)
                {
                    throw new ArgumentNullException("", MessageVM.customerItemMsgFindExistID);
                }

                #endregion Find Transaction Exist

                #region Customer Item ID Create

                #region Customer Item ID Create For Other

                ////var fiscalYear = "";
                ////var latestId = "";

                //////CommonDAL commonDal = new CommonDAL();

                ////newIDCreate = commonDal.TransactionCode("CustomerItem", "Other", "CustomerItemHeaders", "InvoiceNo",
                ////                               "", DateTime.Now.ToString("yyyy-MM-dd"), Master.BranchId.ToString(), currConn, transaction, connVM);

                ////var resultCode = commonDal.GetCurrentCode("CustomerItem", "Other", DateTime.Now.ToString("yyyy-MM-dd"), Master.BranchId.ToString(), currConn, transaction, connVM);

                ////var newIdara = resultCode.Split('~');

                ////latestId = newIdara[0];
                ////fiscalYear = newIdara[1];


                #region Check Existing Id

////                sqlText = @"select COUNT(InvoiceNo) from CustomerItemHeaders 
////                 WHERE InvoiceNo=@InvoiceNo";

////                var cmd = new SqlCommand(sqlText, currConn, transaction);

////                cmd.Parameters.AddWithValue("@InvoiceNo", newIDCreate);

////                var count = (int)cmd.ExecuteScalar();

////                if (count > 0)
////                {
////                    FileLogger.Log("CustomerItemDAL", "CustomerItemInsert", "Customer Item Id " + newIDCreate + " Already Exists");
////                    throw new Exception("Customer Item Id " + newIDCreate + " Already Exists");
////                }

                #endregion

                #endregion Purchase ID Create For Other

                #endregion Purchase ID Create Not Complete

                #region ID generated completed,Insert new Information in Header

                CustomerItemVM rmVM = new CustomerItemVM();

                Master.InvoiceNo = newIDCreate;
                rmVM.InvoiceNo = Master.InvoiceNo;
                rmVM.CreatedBy = Master.CreatedBy;
                rmVM.CreatedOn = Master.CreatedOn;
                rmVM.LastModifiedBy = Master.LastModifiedBy;
                rmVM.LastModifiedOn = Master.LastModifiedOn;
                rmVM.TransactionType = Master.TransactionType;
                rmVM.CustomerID = Master.CustomerID;
                rmVM.Post = Master.Post;
                rmVM.BranchId = Master.BranchId;
                rmVM.Attention = Master.Attention;
                rmVM.Notes = Master.Notes;

                retResults = InsertToMaster(rmVM, currConn, transaction);

                Id = retResults[4];

                if (retResults[0] != "Success")
                {
                    throw new ArgumentNullException("", retResults[1]);
                }

                #endregion ID generated completed,Insert new Information in Header

                #region Insert into Details(Insert complete in Header)

                #region Validation for Detail

                if (Details.Count() < 0)
                {
                    throw new ArgumentNullException("", MessageVM.receiveMsgNoDataToSave);
                }
                retResults = DetailsInsert(Master, Details, transaction, currConn, connVM);
                if (retResults[0] != "Success")
                {
                    throw new ArgumentNullException("", retResults[1]);
                }
                #endregion Validation for Detail

                #endregion Insert into Details(Insert complete in Header)

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion

                #region SuccessResult
                retResults = new string[5];
                retResults[0] = "Success";
                retResults[1] = MessageVM.receiveMsgSaveSuccessfully;
                retResults[2] = "" + Master.InvoiceNo;
                retResults[3] = "N";
                retResults[4] = Id;
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
                FileLogger.Log("CustomerItemDAL", "ReceiveInsert", ex.ToString() + "\n" + sqlText);

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

            return retResults;

            #endregion Result

        }

        public string[] InsertToMaster(CustomerItemVM Master, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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
                sqlText += " insert into CustomerItemHeaders";
                sqlText += " (";
                sqlText += "  InvoiceNo";
                sqlText += " ,CustomerID";
                sqlText += " ,Attention";
                sqlText += " ,Notes";
                sqlText += " ,TransactionType";
                sqlText += " ,CreatedBy";
                sqlText += " ,CreatedOn";
                sqlText += " ,Post";
                sqlText += " ,BranchId";
                sqlText += " )";
                sqlText += " values";
                sqlText += " (";
                sqlText += " @InvoiceNo";
                sqlText += ",@CustomerID";
                sqlText += ",@Attention";
                sqlText += ",@Notes";
                sqlText += ",@TransactionType";
                sqlText += ",@CreatedBy";
                sqlText += ",@CreatedOn";
                sqlText += ",@Post";
                sqlText += ",@BranchId";
                sqlText += ")  SELECT SCOPE_IDENTITY()";


                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;
                cmdInsert.Parameters.AddWithValueAndNullHandle("@InvoiceNo", Master.InvoiceNo);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@CustomerID", Master.CustomerID);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Attention", Master.Attention);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Notes", Master.Notes);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@TransactionType", Master.TransactionType);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@CreatedBy", Master.CreatedBy);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@CreatedOn", Master.CreatedOn);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Post", Master.Post);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@BranchId", Master.BranchId);

                transResult = Convert.ToInt32(cmdInsert.ExecuteScalar());
                retResults[4] = transResult.ToString();

                if (transResult <= 0)
                {
                    throw new ArgumentNullException("", MessageVM.receiveMsgSaveNotSuccessfully);
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
                retResults[1] = MessageVM.receiveMsgSaveSuccessfully;
                retResults[2] = "" + Master.InvoiceNo;
                retResults[3] = "" + PostStatus;
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
                if (Vtransaction == null)
                {

                    transaction.Rollback();
                }

                FileLogger.Log("CustomerItemDAL", "InsertToMaster", ex.ToString() + "\n" + sqlText);
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

        public string[] DetailsInsert(CustomerItemVM Master, List<CustomerItemDetailsVM> Details, SqlTransaction Vtransaction, SqlConnection VcurrConn, SysDBInfoVMTemp connVM = null, string UserId = "")
        {
            #region Initializ
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";
            var Id = "";
            PurchaseDAL purDal = new PurchaseDAL();
            IssueDAL issDal = new IssueDAL();
            SaleDAL salDal = new SaleDAL();

            //SqlConnection currConn = null;
            //SqlTransaction transaction = null;

            int transResult = 0;
            string sqlText = "";
            string issueAutoPostValue = "N";

            #endregion Initializ

            #region Try
            try
            {

                #region Validation for Header

                if (Master == null)
                {
                    throw new ArgumentNullException("", MessageVM.receiveMsgNoDataToSave);
                }

                #endregion Validation for Header

                #region Insert into Details(Insert complete in Header)

                #region Validation for Detail

                if (Details.Count() < 0)
                {
                    throw new ArgumentNullException("", MessageVM.receiveMsgNoDataToSave);
                }

                #endregion Validation for Detail

                #region Insert Detail Table

                foreach (var Item in Details.ToList())
                {

                    #region Insert only DetailTable

                    CustomerItemDetailsVM rdVM = new CustomerItemDetailsVM();
                    rdVM.InvoiceNo = Master.InvoiceNo;
                    rdVM.ItemNo = Item.ItemNo;
                    rdVM.Value = Item.Value;
                    rdVM.VATRate = Item.VATRate;
                    rdVM.CreatedBy = Master.CreatedBy;
                    rdVM.CreatedOn = Master.CreatedOn;
                    rdVM.LastModifiedBy = Master.LastModifiedBy;
                    rdVM.LastModifiedOn = Master.LastModifiedOn;
                    rdVM.Post = Master.Post;
                    rdVM.BranchId = Master.BranchId;
                    rdVM.TransactionType = Master.TransactionType;
                    rdVM.CustomerID = Master.CustomerID;

                    retResults = InsertToDetail(rdVM, VcurrConn, Vtransaction);

                    if (retResults[0] != "Success")
                    {
                        throw new ArgumentNullException("", retResults[1]);
                    }
                    #endregion Insert only DetailTable  //done

                }

                #endregion Insert Detail Table

                #endregion Insert into Details(Insert complete in Header)

                #region SuccessResult

                retResults = new string[5];
                retResults[0] = "Success";
                retResults[1] = MessageVM.receiveMsgSaveSuccessfully;
                retResults[2] = "" + Master.InvoiceNo;
                retResults[3] = "N";
                retResults[4] = Id;

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

                FileLogger.Log("CustomerItemDAL", "DetailsInsert", ex.ToString() + "\n" + sqlText);

            }
            finally
            {

            }
            #endregion Catch and Finall

            #region Result

            return retResults;

            #endregion Result

        }

        public string[] InsertToDetail(CustomerItemDetailsVM Detail, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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

                #region Insert Detail Table

                #region Insert only DetailTable

                sqlText = "";
                sqlText += " insert into CustomerItemDetails(";
                sqlText += "  InvoiceNo";
                sqlText += " ,CustomerID";
                sqlText += " ,ItemNo";
                sqlText += " ,Value";
                sqlText += " ,TransactionType";
                sqlText += " ,CreatedBy";
                sqlText += " ,CreatedOn";
                sqlText += " ,LastModifiedBy";
                sqlText += " ,LastModifiedOn";
                sqlText += " ,Post";
                sqlText += " ,BranchId";
                sqlText += " ,VATRate";
                sqlText += " )";
                sqlText += " values( ";
                sqlText += "@InvoiceNo";
                sqlText += ",@CustomerID";
                sqlText += ",@ItemNo";
                sqlText += ",@Value";
                sqlText += ",@TransactionType";
                sqlText += ",@CreatedBy";
                sqlText += ",@CreatedOn";
                sqlText += ",@LastModifiedBy";
                sqlText += ",@LastModifiedOn";
                sqlText += ",@Post";
                sqlText += ",@BranchId";
                sqlText += ",@VATRate";
                sqlText += ")";

                SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                cmdInsDetail.Transaction = transaction;

                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@InvoiceNo", Detail.InvoiceNo);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@CustomerID", Detail.CustomerID);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemNo", Detail.ItemNo);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@Value", Detail.Value);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@TransactionType", Detail.TransactionType);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@CreatedBy", Detail.CreatedBy);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@CreatedOn", Detail.CreatedOn);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@LastModifiedBy", Detail.LastModifiedBy);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@LastModifiedOn", Detail.LastModifiedOn);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@Post", Detail.Post);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BranchId", Detail.BranchId);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@VATRate", Detail.VATRate);

                transResult = cmdInsDetail.ExecuteNonQuery();

                if (transResult <= 0)
                {
                    throw new ArgumentNullException("", MessageVM.receiveMsgSaveNotSuccessfully);
                }

                #endregion Insert only DetailTable  //done

                #endregion Insert Detail Table

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
                retResults[1] = MessageVM.receiveMsgSaveSuccessfully;
                retResults[2] = "" + Detail.InvoiceNo;
                retResults[3] = "" + PostStatus;
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

                if (Vtransaction == null)
                {
                    transaction.Rollback();
                }

                FileLogger.Log("CustomerItemDAL", "InsertToDetail", ex.ToString() + "\n" + sqlText);

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

        public string[] UpdateToMaster(CustomerItemVM Master, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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

                #region Update

                sqlText = "";
                sqlText += " update CustomerItemHeaders set  ";

                //sqlText += "   CustomerID       =@CustomerID";
                sqlText += "   Attention             =@Attention";
                sqlText += "  ,Notes                 =@Notes";
                sqlText += "  ,LastModifiedBy       =@LastModifiedBy";
                sqlText += "  ,LastModifiedOn       =@LastModifiedOn";

                sqlText += "  where CustomerID         =@CustomerID";


                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;

                ////cmdUpdate.Parameters.AddWithValueAndNullHandle("@InvoiceNo", Master.InvoiceNo);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@CustomerID", Master.CustomerID);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@Attention", Master.Attention);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@Notes", Master.Notes);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@LastModifiedBy", Master.LastModifiedBy);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@LastModifiedOn", Master.LastModifiedOn);

                transResult = (int)cmdUpdate.ExecuteNonQuery();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException("", MessageVM.PurchasemsgUpdateNotSuccessfully);
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
                retResults[1] = MessageVM.PurchasemsgSaveSuccessfully;
                retResults[2] = "" + Master.InvoiceNo;
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
                if (Vtransaction == null)
                {

                    transaction.Rollback();
                }

                FileLogger.Log("CustomerItemDAL", "UpdateToMaster", ex.ToString() + "\n" + sqlText);
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

        public string[] CustomerItemUpdate(CustomerItemVM Master, List<CustomerItemDetailsVM> Details, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";
            IssueDAL issDal = new IssueDAL();
            PurchaseDAL purDal = new PurchaseDAL();
            SaleDAL salDal = new SaleDAL();

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            string sqlText = "";

            #endregion Initializ

            #region Try
            try
            {
                #region Validation for Header

                if (Master == null)
                {
                    throw new ArgumentNullException("", MessageVM.receiveMsgNoDataToUpdate);
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

                #region Current Status

                #region Post Status

                string currentPostStatus = "N";

                sqlText = "";
                sqlText = @"
select InvoiceNo, Post from CustomerItemHeaders
where 1=1 
and InvoiceNo=@InvoiceNo

";
                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
                cmd.Parameters.AddWithValue("@InvoiceNo", Master.InvoiceNo);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt != null && dt.Rows.Count > 0)
                {
                    currentPostStatus = dt.Rows[0]["Post"].ToString();
                }

                #endregion

                #endregion

                #region Master

                #region Find ID for Update

                sqlText = "";
                sqlText = sqlText + "select COUNT(InvoiceNo) from CustomerItemHeaders WHERE CustomerID=@MasterCustomerID ";
                SqlCommand cmdFindIdUpd = new SqlCommand(sqlText, currConn);
                cmdFindIdUpd.Transaction = transaction;
                cmdFindIdUpd.Parameters.AddWithValue("@MasterCustomerID", Master.CustomerID);
                int IDExist = (int)cmdFindIdUpd.ExecuteScalar();

                if (IDExist <= 0)
                {
                    throw new ArgumentNullException("", MessageVM.receiveMsgUnableFindExistID);
                }

                #endregion Find ID for Update

                #region Find Customer Exist

                //////sqlText = "";
                //////sqlText = sqlText + @"select COUNT(InvoiceNo) from CustomerItemHeaders WHERE CustomerID=@MasterCustomerID and InvoiceNo!=@MInvoiceNo ";
                //////SqlCommand cmdExistTran = new SqlCommand(sqlText, currConn);
                //////cmdExistTran.Transaction = transaction;
                //////cmdExistTran.Parameters.AddWithValueAndNullHandle("@MasterCustomerID", Master.CustomerID);
                //////cmdExistTran.Parameters.AddWithValue("@MInvoiceNo", Master.InvoiceNo);

                //////IDExist = (int)cmdExistTran.ExecuteScalar();

                //////if (IDExist > 0)
                //////{
                //////    throw new ArgumentNullException("", MessageVM.customerItemMsgFindExistID);
                //////}

                #endregion Find Transaction Exist

                #region Update Header

                #region Select Data

                string settingValue = commonDal.settingValue("EntryProcess", "UpdateOnPost", connVM);

                if (settingValue != "Y")
                {
                    if (currentPostStatus == "Y")
                    {
                        throw new ArgumentNullException("", MessageVM.ThisTransactionWasPosted);
                    }

                }

                #endregion

                #region Assign Data
                CustomerItemVM rmVm = new CustomerItemVM();
                rmVm.InvoiceNo = Master.InvoiceNo;
                rmVm.CustomerID = Master.CustomerID;
                rmVm.Attention = Master.Attention;
                rmVm.Notes = Master.Notes;
                rmVm.TransactionType = Master.TransactionType;
                rmVm.LastModifiedBy = Master.LastModifiedBy;
                rmVm.LastModifiedOn = Master.LastModifiedOn;
                rmVm.Post = Master.Post;
                rmVm.BranchId = Master.BranchId;

                #endregion

                #region Update Data

                retResults = UpdateToMaster(rmVm, currConn, transaction, connVM);

                if (retResults[0] != "Success")
                {
                    throw new ArgumentNullException("", retResults[1]);
                }

                #endregion

                #endregion update Header

                #endregion Master

                #region Update into Details(Update complete in Header)

                #region Validation for Detail

                if (Details.Count() < 0)
                {
                    throw new ArgumentNullException("", MessageVM.receiveMsgNoDataToUpdate);
                }

                #endregion Validation for Detail

                #region Delete Existing Detail Data

                #region Delete Existing Data

                sqlText = "";
                sqlText += @" delete FROM CustomerItemDetails WHERE InvoiceNo=@MasterInvoiceNo ";

                SqlCommand cmdDeleteDetail = new SqlCommand(sqlText, currConn);
                cmdDeleteDetail.Transaction = transaction;
                cmdDeleteDetail.Parameters.AddWithValueAndNullHandle("@MasterInvoiceNo", Master.InvoiceNo);

                transResult = cmdDeleteDetail.ExecuteNonQuery();

                #endregion

                #endregion

                #endregion  Update into Details(Update complete in Header)

                #region Details Insert

                Master.CreatedBy = Master.LastModifiedBy;
                Master.CreatedOn = Master.LastModifiedOn;

                retResults = DetailsInsert(Master, Details, transaction, currConn, connVM);

                if (retResults[0] != "Success")
                {
                    throw new ArgumentNullException("", retResults[1]);
                }

                #endregion

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion

                #region SuccessResult

                retResults[0] = "Success";
                retResults[1] = MessageVM.receiveMsgUpdateSuccessfully;
                retResults[2] = Master.InvoiceNo;
                retResults[3] = "N";
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

                FileLogger.Log("CustomerItemDAL", "CustomerItemUpdate", ex.ToString() + "\n" + sqlText);

                ////throw ex;
            }
            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion Catch and Finall

            return retResults;

        }

        public string[] CustomerItemPost(CustomerItemVM Master, List<CustomerItemDetailsVM> Details, SqlTransaction Vtransaction = null, SqlConnection VcurrConn = null, SysDBInfoVMTemp connVM = null, string UserId = "")
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


                #region Validation for Header

                if (Master == null)
                {
                    throw new ArgumentNullException("", MessageVM.receiveMsgNoDataToPost);
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


                #region Find ID for Update

                sqlText = "";
                sqlText = sqlText + "select COUNT(InvoiceNo) from CustomerItemHeaders WHERE InvoiceNo=@MasterInvoiceNo ";
                SqlCommand cmdFindIdUpd = new SqlCommand(sqlText, currConn);
                cmdFindIdUpd.Transaction = transaction;
                cmdFindIdUpd.Parameters.AddWithValue("@MasterInvoiceNo", Master.InvoiceNo);
                int IDExist = (int)cmdFindIdUpd.ExecuteScalar();

                if (IDExist <= 0)
                {
                    throw new ArgumentNullException("", MessageVM.receiveMsgUnableFindExistIDPost);
                }

                #endregion Find ID for Update

                #region Post

                sqlText = "";
                sqlText += @" Update  CustomerItemDetails set  Post ='Y',LastModifiedBy =@MasterLastModifiedBy,LastModifiedOn =@MasterLastModifiedOn    WHERE InvoiceNo=@MasterInvoiceNo ";
                sqlText += @" Update  CustomerItemHeaders set  Post ='Y',LastModifiedBy =@MasterLastModifiedBy,LastModifiedOn =@MasterLastModifiedOn    WHERE InvoiceNo=@MasterInvoiceNo ";

                SqlCommand cmdDeleteDetail = new SqlCommand(sqlText, currConn);
                cmdDeleteDetail.Transaction = transaction;
                cmdDeleteDetail.Parameters.AddWithValueAndNullHandle("@MasterInvoiceNo", Master.InvoiceNo);
                cmdDeleteDetail.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", OrdinaryVATDesktop.DateToDate(Master.LastModifiedOn));
                cmdDeleteDetail.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);

                transResult = cmdDeleteDetail.ExecuteNonQuery();


                #endregion

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion

                #region SuccessResult

                retResults[0] = "Success";
                retResults[1] = MessageVM.receiveMsgSuccessfullyPost;
                retResults[2] = Master.InvoiceNo;
                retResults[3] = "Y";
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

                FileLogger.Log("CustomerItemDAL", "CustomerItemPost", ex.ToString() + "\n" + sqlText);
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

        //==================SelectAll Master=================

        public List<CustomerItemVM> SelectAllList(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            //SqlConnection currConn = null;
            //SqlTransaction transaction = null;
            string sqlText = "";
            List<CustomerItemVM> VMs = new List<CustomerItemVM>();
            CustomerItemVM vm;
            #endregion

            #region try

            try
            {
                #region sql statement

                #region SqlExecution

                DataTable dt = SelectAll(Id, conditionFields, conditionValues, VcurrConn, Vtransaction, connVM);

                foreach (DataRow dr in dt.Rows)
                {
                    try
                    {
                        vm = new CustomerItemVM();
                        vm.Id = Convert.ToInt32(dr["Id"].ToString());
                        vm.InvoiceNo = dr["InvoiceNo"].ToString();
                        vm.CustomerID = dr["CustomerID"].ToString();
                        vm.CustomerCode = dr["CustomerCode"].ToString();
                        vm.CustomerName = dr["CustomerName"].ToString();
                        vm.Attention = dr["Attention"].ToString();
                        vm.Notes = dr["Notes"].ToString();
                        vm.TransactionType = dr["TransactionType"].ToString();
                        vm.Post = dr["Post"].ToString();
                        vm.BranchId = Convert.ToInt32(dr["BranchId"]);
                        vm.CreatedBy = dr["CreatedBy"].ToString();
                        vm.CreatedOn = dr["CreatedOn"].ToString();
                        vm.LastModifiedBy = dr["LastModifiedBy"].ToString();
                        vm.LastModifiedOn = dr["LastModifiedOn"].ToString();

                        VMs.Add(vm);
                    }
                    catch (Exception e)
                    {

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

            catch (Exception ex)
            {
                FileLogger.Log("CustomerItemDAL", "SelectAllList", ex.ToString() + "\n" + sqlText);
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

        public DataTable SelectAll(int CustomerID = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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

                #region Sql Text

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

 cih.Id
,cih.InvoiceNo
,cih.CustomerID
,cus.CustomerCode
,cus.CustomerName
,cih.Attention
,cih.Notes
,cih.TransactionType
,cih.CreatedBy
,cih.CreatedOn
,cih.LastModifiedBy
,cih.LastModifiedOn
,cih.Post
,cih.BranchId
      
  FROM CustomerItemHeaders cih
  left outer join Customers cus on cih.CustomerID= cus.CustomerID
WHERE  1=1
";
                #endregion

                sqlTextCount += @" 
SELECT COUNT(cih.CustomerID)RecordCount
FROM CustomerItemHeaders cih 
LEFT OUTER JOIN Customers cus ON cih.CustomerID=cus.CustomerID
WHERE  1=1
";
                if (CustomerID > 0)
                {
                    sqlTextParameter += @" AND cih.CustomerID=@Id";
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

                #endregion

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
                //    if (!string.IsNullOrEmpty(likeVM.ReceiveNo))
                //    {
                //        da.SelectCommand.Parameters.AddWithValue("@ReceiveNo", "%" + likeVM.ReceiveNo + "%");
                //    }
                //}
                if (CustomerID > 0)
                {
                    da.SelectCommand.Parameters.AddWithValueAndNullHandle("@Id", CustomerID);
                }
                da.Fill(ds);

                #endregion SqlExecution

                #region Commit


                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion

                #region DataSet to DataTable

                dt = ds.Tables[0].Copy();
                if (index >= 0)
                {
                    dt.Rows.Add(ds.Tables[1].Rows[0][0]);
                }

                #endregion

                #endregion
            }
            #endregion

            #region catch

            catch (Exception ex)
            {
                FileLogger.Log("CustomerItemDAL", "SelectAll", ex.ToString() + "\n" + sqlText);
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

        //==================SelectAll Detail=================
        public List<CustomerItemDetailsVM> SelectAllDetail(string CustomerID, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<CustomerItemDetailsVM> VMs = new List<CustomerItemDetailsVM>();
            CustomerItemDetailsVM vm;
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
cid.Id
,cid.InvoiceNo
,cid.ItemNo
,pro.ProductCode
,pro.ProductName
,cid.Value
,cid.TransactionType
,cid.CreatedBy
,cid.CreatedOn
,cid.LastModifiedBy
,cid.LastModifiedOn
,cid.Post
,cid.BranchId
,isnull(cid.VATRate,0)VATRate
,pro.UOM
,pro.HSCodeNo
FROM CustomerItemDetails cid 
left outer join Products pro on cid.ItemNo=pro.ItemNo
WHERE  1=1

";

                if (CustomerID != null)
                {
                    sqlText += " AND cid.CustomerID=@CustomerID";
                }
                string cField = "";
                if (conditionFields != null && conditionValues != null && conditionFields.Length == conditionValues.Length)
                {
                    for (int i = 0; i < conditionFields.Length; i++)
                    {
                        if (string.IsNullOrEmpty(conditionFields[i]) || string.IsNullOrEmpty(conditionValues[i]))
                        {
                            continue;
                        }
                        cField = conditionFields[i].ToString();
                        cField = OrdinaryVATDesktop.StringReplacing(cField);
                        sqlText += " AND " + conditionFields[i] + "=@" + cField;
                    }
                }
                #endregion SqlText

                #region SqlExecution

                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);

                if (CustomerID != null)
                {
                    objComm.Parameters.AddWithValueAndNullHandle("@CustomerID", CustomerID);
                }
                if (conditionFields != null && conditionValues != null && conditionFields.Length == conditionValues.Length)
                {
                    for (int j = 0; j < conditionFields.Length; j++)
                    {
                        if (string.IsNullOrEmpty(conditionFields[j]) || string.IsNullOrEmpty(conditionValues[j]))
                        {
                            continue;
                        }
                        cField = conditionFields[j].ToString();
                        cField = OrdinaryVATDesktop.StringReplacing(cField);
                        objComm.Parameters.AddWithValueAndNullHandle("@" + cField, conditionValues[j]);
                    }
                }
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {

                    vm = new CustomerItemDetailsVM();

                    vm.InvoiceNo = dr["InvoiceNo"].ToString();
                    vm.ItemNo = dr["ItemNo"].ToString();
                    vm.ProductCode = dr["ProductCode"].ToString();
                    vm.ProductName = dr["ProductName"].ToString();
                    vm.Value = Convert.ToDecimal(dr["Value"].ToString());
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedOn = dr["CreatedOn"].ToString();
                    vm.LastModifiedBy = dr["LastModifiedBy"].ToString();
                    vm.LastModifiedOn = dr["LastModifiedOn"].ToString();
                    vm.TransactionType = dr["TransactionType"].ToString();
                    vm.Post = dr["Post"].ToString();
                    vm.UOM = dr["UOM"].ToString();
                    vm.BranchId = Convert.ToInt32(dr["BranchId"].ToString());
                    vm.VATRate = Convert.ToDecimal(dr["VATRate"].ToString());
                    vm.HSCode = dr["HSCodeNo"].ToString();

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

            catch (Exception ex)
            {
                FileLogger.Log("CustomerItemDAL", "SelectAllDetail", ex.ToString() + "\n" + sqlText);
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
            return VMs;
        }

        #region Bill Process

        public List<CustomerItemVM> SelectAllCustomerList(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            //SqlConnection currConn = null;
            //SqlTransaction transaction = null;
            string sqlText = "";
            List<CustomerItemVM> VMs = new List<CustomerItemVM>();
            CustomerItemVM vm;
            #endregion

            #region try

            try
            {
                #region sql statement

                #region SqlExecution

                DataTable dt = SelectAllCustomer(Id, conditionFields, conditionValues, VcurrConn, Vtransaction, connVM);

                foreach (DataRow dr in dt.Rows)
                {
                    try
                    {
                        vm = new CustomerItemVM();
                        ////vm.Id = Convert.ToInt32(dr["Id"].ToString());
                        vm.InvoiceNo = dr["InvoiceNo"].ToString();
                        vm.CustomerID = dr["CustomerID"].ToString();
                        vm.CustomerCode = dr["CustomerCode"].ToString();
                        vm.CustomerName = dr["CustomerName"].ToString();
                        vm.Attention = dr["Attention"].ToString();
                        vm.Notes = dr["Notes"].ToString();
                        vm.TransactionType = dr["TransactionType"].ToString();
                        vm.Post = dr["Post"].ToString();
                        vm.BranchId = Convert.ToInt32(dr["BranchId"]);
                        vm.CreatedBy = dr["CreatedBy"].ToString();
                        vm.CreatedOn = dr["CreatedOn"].ToString();
                        vm.LastModifiedBy = dr["LastModifiedBy"].ToString();
                        vm.LastModifiedOn = dr["LastModifiedOn"].ToString();
                        vm.TotalValue = Convert.ToDecimal(dr["TotalValue"].ToString());

                        VMs.Add(vm);
                    }
                    catch (Exception e)
                    {

                    }

                }


                #endregion SqlExecution

                #endregion
            }
            #endregion

            #region catch

            catch (Exception ex)
            {
                FileLogger.Log("CustomerItemDAL", "SelectAllCustomerList", ex.ToString() + "\n" + sqlText);
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

        public DataTable SelectAllCustomer(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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

                #region Sql Text

                #region Sql Text

                sqlText += @"
SELECT
 cih.Id
,cih.InvoiceNo
,cih.CustomerID
,cus.CustomerCode
,cus.CustomerName
,cih.Attention
,cih.Notes
,cih.TransactionType
,cih.CreatedBy
,cih.CreatedOn
,cih.LastModifiedBy
,cih.LastModifiedOn
,cih.Post
,cih.BranchId
,isnull(sum(cid.Value),0)TotalValue
   
FROM CustomerItemHeaders cih
left outer join Customers cus on cih.CustomerID= cus.CustomerID
left outer join CustomerItemDetails cid on cih.InvoiceNo= cid.InvoiceNo
left outer join CustomerBillProcess cbp on cih.CustomerID= cbp.CustomerID

WHERE  1=1
";
                #endregion

                #region sqlText Parameters

                if (Id > 0)
                {
                    sqlText += @" AND cih.Id=@Id";
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

                #endregion

                #region group by

                sqlText += @"
group by 

 cih.Id
,cih.InvoiceNo
,cih.CustomerID
,cus.CustomerCode
,cus.CustomerName
,cih.Attention
,cih.Notes
,cih.TransactionType
,cih.CreatedBy
,cih.CreatedOn
,cih.LastModifiedBy
,cih.LastModifiedOn
,cih.Post
,cih.BranchId
";

                #endregion

                #endregion

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

                if (Id > 0)
                {
                    da.SelectCommand.Parameters.AddWithValueAndNullHandle("@Id", Id);
                }
                da.Fill(dt);

                #endregion SqlExecution

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion

                #endregion
            }
            #endregion

            #region catch

            catch (Exception ex)
            {
                FileLogger.Log("CustomerItemDAL", "SelectAllCustomer", ex.ToString() + "\n" + sqlText);
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

        public DataTable SelectAllCustomerItem(int Id = 0, List<string> IDs = null, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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

                #region Sql Text

                #region Sql Text

                sqlText += @"
SELECT
 cih.InvoiceNo ID
,cih.CustomerID CustomerID
,cus.CustomerCode Customer_Code
,cus.CustomerName Customer_Name
,isnull(cus.Address1,'-') Delivery_Address
--,cih.TransactionType
,cih.BranchId
,cid.ItemNo
,pro.ProductCode Item_Code
,pro.ProductName Item_Name
,isnull(cid.VATRate,0) VAT_Rate
,pro.UOM
,'' Reference_No
,'N' Post
,isnull(pro.SDRate,0) SD_Rate
,'1' Quantity
,cid.Value NBR_Price
,'0' SubTotal
,'VAT 4.3' VAT_Name
,'N' Non_Stock
,'0' Trading_MarkUp
,0 Discount_Amount
,0 Promotional_Quantity
,'NA' LC_Number
,'BDT' Currency_Code
,'NEW' Sale_Type
,'' Previous_Invoice_No
,'N' Is_Print
,'0' Tender_Id
,Type = case when isnull(cid.VATRate,0) = 15 then 'VAT' when isnull(cid.VATRate,0) = 0 then 'NoNVAT' else 'OtherRate' end

FROM CustomerItemHeaders cih
left outer join Customers cus on cih.CustomerID= cus.CustomerID
left outer join CustomerItemDetails cid on cih.InvoiceNo= cid.InvoiceNo
left outer join Products pro on cid.ItemNo= pro.ItemNo

WHERE  1=1
";
                #endregion


                #region sqlText Parameters

                if (Id > 0)
                {
                    sqlText += @" AND cih.Id=@Id";
                }

                if (IDs != null && IDs.Count > 0)
                {
                    string customerIDs = string.Join("','", IDs);

                    sqlText = sqlText + @" AND cih.CustomerID IN('" + customerIDs + "')";
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

                #endregion

                #endregion

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

                if (Id > 0)
                {
                    da.SelectCommand.Parameters.AddWithValueAndNullHandle("@Id", Id);
                }
                da.Fill(dt);

                #endregion SqlExecution

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion

                #endregion
            }
            #endregion

            #region catch

            catch (Exception ex)
            {
                FileLogger.Log("CustomerItemDAL", "SelectAllCustomerItem", ex.ToString() + "\n" + sqlText);
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

        public ResultVM BillProcess(CustomerItemVM Master, IntegrationParam paramVM, SqlTransaction Vtransaction = null, SqlConnection VcurrConn = null, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            ResultVM rVM = new ResultVM();
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            string sqlText = "";
            DataTable dt = new DataTable();
            SaleDAL salesDal = new SaleDAL();
            string[] sqlResults = new string[6];
            CommonDAL commonDal = new CommonDAL();

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

                #region Process

                if (Master.IDs != null && Master.IDs.Count > 0)
                {
                    #region Fiscal Year Check

                    ////////string periodName = Master.PeriodName;

                    string periodName = Convert.ToDateTime(Master.ProcessDate).ToString("MMMM-yyyy");

                    FiscalYearVM varFiscalYearVM = new FiscalYearDAL().SelectAll(0, new[] { "PeriodName" }, new[] { periodName }, currConn, transaction).FirstOrDefault();

                    if (varFiscalYearVM == null)
                    {
                        throw new ArgumentNullException("", "Fiscal Year Not Available for this month " + periodName);
                    }

                    string PeriodID = varFiscalYearVM.PeriodID;

                    #endregion

                    #region Check Exits

                    ////rVM = CheckExits(Master.IDs, PeriodID,paramVM.TransactionType, transaction, currConn);

                    ////if (rVM.Status == "Fail")
                    ////{
                    ////    return rVM;
                    ////}

                    #endregion

                    #region Get Data
                    
                    dt = SelectAllCustomerItem(0, Master.IDs, null, null, currConn, transaction, connVM);

                    #endregion

                    #region Data filtering

                    #region create temp

                    #region sqlText

                    sqlText = @"
create table #temp(
ID varchar(50)
,CustomerID varchar(20)
,Customer_Code varchar(50)
,Customer_Name varchar(500)
,Delivery_Address varchar(500)
,BranchId int
,ItemNo varchar(20)
,Item_Code varchar(100)
,Item_Name varchar(100)
,VAT_Rate decimal(25, 9)
,UOM varchar(100)
,Reference_No varchar(6000)
,Post varchar(1)
,SD_Rate decimal(25, 9)
,Quantity decimal(25, 9)
,NBR_Price decimal(25, 9)
,SubTotal decimal(25, 9)
,VAT_Name varchar(100)
,Non_Stock varchar(100)
,Trading_MarkUp decimal(25, 9)
,Discount_Amount decimal(25, 9)
,Promotional_Quantity decimal(25, 9)
,LC_Number varchar(50)
,Currency_Code varchar(50)
,Sale_Type varchar(100)
,Previous_Invoice_No varchar(100)
,Is_Print varchar(1)
,Tender_Id varchar(100)
,Type varchar(100)
)

";
                    #endregion

                    #region Execute

                    SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
                    transResult = (int)cmd.ExecuteNonQuery();

                    #endregion

                    #endregion

                    #region bulk insert temp

                    sqlResults = commonDal.BulkInsert("#temp", dt, currConn, transaction, 0, null, connVM);

                    if (sqlResults[0] == "Fail")
                    {
                        rVM.Status = sqlResults[0];
                        rVM.Message = sqlResults[1];
                        return rVM;
                    }

                    #endregion

                    #region delete exits data temp

                    sqlText = @"
delete #temp where CustomerID in (select CustomerID from SalesInvoiceHeaders where PeriodID=@PeriodID and TransactionType=@TransactionType)
";

                    #region Execute

                    cmd = new SqlCommand(sqlText, currConn, transaction);
                    cmd.Parameters.AddWithValue("@PeriodID", PeriodID);
                    cmd.Parameters.AddWithValue("@TransactionType", paramVM.TransactionType);

                    transResult = (int)cmd.ExecuteNonQuery();

                    #endregion

                    #endregion

                    #region Select and drop temp

                    sqlText = @"
select 

Customer_Code ID
,CustomerID 
,Customer_Code 
,Customer_Name 
,Delivery_Address 
,BranchId 
,ItemNo
,Item_Code 
,Item_Name 
,VAT_Rate 
,UOM 
,Reference_No
,Post 
,SD_Rate 
,Quantity
,NBR_Price
,SubTotal 
,VAT_Name 
,Non_Stock
,Trading_MarkUp 
,Discount_Amount
,Promotional_Quantity 
,LC_Number
,Currency_Code 
,Sale_Type 
,Previous_Invoice_No
,Is_Print
,Tender_Id
,Type

from #temp

drop table #temp

";
                    cmd = new SqlCommand(sqlText, currConn, transaction);

                    dt = new DataTable();

                    SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                    dataAdapter.Fill(dt);

                    #endregion

                    #endregion

                    if (dt == null || dt.Rows.Count <= 0)
                    {
                        rVM.Status = "Fail";
                        rVM.Message = "Requested all transaction already processed ";
                        return rVM;

                    }

                    #region Days Calculation

                    string time = DateTime.Now.ToString("HH:mm:ss");

                    DateTime BillingPeriodFrom = Convert.ToDateTime(varFiscalYearVM.PeriodStart);
                    DateTime BillingPeriodTo = Convert.ToDateTime(varFiscalYearVM.PeriodEnd);

                    TimeSpan Totaldays = (BillingPeriodTo) - (BillingPeriodFrom.AddDays(-1));
                    double NrOfDays = Totaldays.TotalDays;
                    int tDays = Convert.ToInt32(NrOfDays);


                    #endregion

                    #region TableValidation

                    TableValidation(dt, paramVM, BillingPeriodFrom.ToString("yyyy-MM-dd"), BillingPeriodTo.ToString("yyyy-MM-dd"), tDays, Master.ProcessDate);

                    #endregion

                    #region Comments
                    
                    //////foreach (DataRow row in dt.Rows)
                    //////{

                    //////    #region Date_Time

                    //////    row["Delivery_Date_Time"] = BillingPeriodTo.ToString("yyyy-MM-dd");
                    //////    row["Invoice_Date_Time"] = BillingPeriodTo.ToString("yyyy-MM-dd");

                    //////    #endregion

                    //////    row["BillingPeriodFrom"] = BillingPeriodFrom.ToString("yyyy-MM-dd");
                    //////    row["BillingPeriodTo"] = BillingPeriodTo.ToString("yyyy-MM-dd");

                    //////    row["BillingDays"] = tDays;

                    //////    #region Comments

                    //////    ////decimal VatRate = 0;
                    //////    ////decimal SDRate = 0;
                    //////    ////decimal Quantity = 0;
                    //////    ////decimal NBR_Price = 0;
                    //////    ////////decimal SubTotal = 0;
                    //////    ////////decimal SDAmount = 0;
                    //////    ////////decimal VAT_Amount = 0;

                    //////    ////VatRate = Convert.ToDecimal(row["VAT_Rate"].ToString());
                    //////    ////SDRate = Convert.ToDecimal(row["SD_Rate"].ToString());
                    //////    ////Quantity = Convert.ToDecimal(row["Quantity"].ToString());
                    //////    ////NBR_Price = Convert.ToDecimal(row["NBR_Price"].ToString());

                    //////    //////SubTotal = NBR_Price * Quantity;

                    //////    //////SDAmount = (SubTotal * SDRate) / 100;

                    //////    //////VAT_Amount = (SubTotal + SDAmount) * VatRate / 100;

                    //////    #endregion

                    //////}
                    #endregion

                    sqlResults = salesDal.SaveAndProcess(dt, () => { }, Convert.ToInt32(paramVM.BranchId), "", connVM, paramVM, currConn, transaction, paramVM.CurrentUser);

                }

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

                #region SuccessResult

                rVM.Status = sqlResults[0];

                if (sqlResults[0].ToLower() == "success")
                {
                    rVM.Message = " Data Successfully processed";

                }
                else
                {
                    rVM.Message = sqlResults[1];
                }

                #endregion SuccessResult

            }
            #endregion Try

            #region Catch and Finall

            catch (Exception ex)
            {
                rVM = new ResultVM();
                rVM.Message = ex.Message;
                if (Vtransaction == null)
                {
                    transaction.Rollback();
                }
                FileLogger.Log("CustomerItemDAL", "BillProcess", ex.ToString() + "\n" + sqlText);

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

        private void TableValidation(DataTable salesData, IntegrationParam paramVM, string PeriodFrom, string PeriodTo, int tDays,string InvoiceDate)
        {
            if (!salesData.Columns.Contains("Branch_Code"))
            {
                var columnName = new DataColumn("Branch_Code") { DefaultValue = paramVM.BranchCode };
                salesData.Columns.Add(columnName);
            }

            var column = new DataColumn("SL") { DefaultValue = "" };
            var CreatedBy = new DataColumn("CreatedBy") { DefaultValue = paramVM.CurrentUserName };
            var ReturnId = new DataColumn("ReturnId") { DefaultValue = 0 };
            var BOMId = new DataColumn("BOMId") { DefaultValue = 0 };
            var TransactionType = new DataColumn("TransactionType") { DefaultValue = paramVM.TransactionType };
            var CreatedOn = new DataColumn("CreatedOn") { DefaultValue = DateTime.Now.ToString() };
            DataColumn userId = new DataColumn("UserId") { DefaultValue = paramVM.CurrentUser };

            DataColumn Vehicle_No = new DataColumn("Vehicle_No") { DefaultValue = "-" };
            DataColumn Delivery_Date_Time = new DataColumn("Delivery_Date_Time") { DefaultValue = InvoiceDate };
            DataColumn Invoice_Date_Time = new DataColumn("Invoice_Date_Time") { DefaultValue = InvoiceDate };
            DataColumn BENumber = new DataColumn("BENumber") { DefaultValue = "" };
            DataColumn BillingPeriodFrom = new DataColumn("BillingPeriodFrom") { DefaultValue = PeriodFrom };
            DataColumn BillingPeriodTo = new DataColumn("BillingPeriodTo") { DefaultValue = PeriodTo };
            DataColumn BillingDays = new DataColumn("BillingDays") { DefaultValue = tDays };

            salesData.Columns.Add(column);
            salesData.Columns.Add(CreatedBy);
            salesData.Columns.Add(CreatedOn);
            salesData.Columns.Add(userId);

            salesData.Columns.Add(Vehicle_No);
            salesData.Columns.Add(Delivery_Date_Time);
            salesData.Columns.Add(Invoice_Date_Time);
            salesData.Columns.Add(BENumber);
            salesData.Columns.Add(BillingPeriodFrom);
            salesData.Columns.Add(BillingPeriodTo);
            salesData.Columns.Add(BillingDays);

            if (!salesData.Columns.Contains("ReturnId"))
            {
                salesData.Columns.Add(ReturnId);
            }

            salesData.Columns.Add(BOMId);
            salesData.Columns.Add(TransactionType);

        }

        public ResultVM CheckExits(List<string> IDs, string PeriodId, string TransactionType, SqlTransaction Vtransaction = null, SqlConnection VcurrConn = null, SysDBInfoVMTemp connVM = null)
        {

            #region Initializ
            ResultVM rVM = new ResultVM();
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            string sqlText = "";
            DataTable dt = new DataTable();
            SaleDAL salesDal = new SaleDAL();
            string[] sqlResults = new string[6];

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

                #region Process

                CommonDAL commonDal = new CommonDAL();

                foreach (var item in IDs)
                {

                    string CustomerID = item;

                    #region Find Transaction Exist

                    sqlText = "";
                    sqlText = @"
select COUNT(SalesInvoiceNo) from SalesInvoiceHeaders WHERE 1=1 
and CustomerID=@CustomerID  and TransactionType=@TransactionType and  PeriodID=@PeriodID";

                    SqlCommand cmdExistTran = new SqlCommand(sqlText, currConn);
                    cmdExistTran.Transaction = transaction;
                    cmdExistTran.Parameters.AddWithValueAndNullHandle("@CustomerID", CustomerID);
                    cmdExistTran.Parameters.AddWithValueAndNullHandle("@TransactionType", TransactionType);
                    cmdExistTran.Parameters.AddWithValueAndNullHandle("@PeriodID", PeriodId);

                    int IDExist = (int)cmdExistTran.ExecuteScalar();

                    if (IDExist > 0)
                    {
                        sqlText = @"select CustomerName+' ( '+CustomerCode+' )' from Customers where CustomerID=@vCustomerID";

                        cmdExistTran = new SqlCommand(sqlText, currConn);
                        cmdExistTran.Transaction = transaction;
                        cmdExistTran.Parameters.AddWithValueAndNullHandle("@vCustomerID", CustomerID);

                        string Customer = (string)cmdExistTran.ExecuteScalar();

                        throw new ArgumentNullException("", MessageVM.MsgFindExistID + " for this month Customer : " + Customer);
                    }

                    #endregion Find Transaction Exist

                }

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

                #region SuccessResult

                rVM.Status = "Success";

                #endregion SuccessResult

            }
            #endregion Try

            #region Catch and Finall

            catch (Exception ex)
            {
                rVM = new ResultVM();
                rVM.Status = "Fail";
                rVM.Message = ex.Message;

                FileLogger.Log("CustomerItemDAL", "CheckExits", ex.ToString() + "\n" + sqlText);

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

        #endregion

        #region Select Customer

        public List<CustomerItemVM> SelectCustomerList(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            //SqlConnection currConn = null;
            //SqlTransaction transaction = null;
            string sqlText = "";
            List<CustomerItemVM> VMs = new List<CustomerItemVM>();
            CustomerItemVM vm;
            #endregion

            #region try

            try
            {
                #region sql statement

                #region SqlExecution

                DataTable dt = SelectCustomer(Id, conditionFields, conditionValues, VcurrConn, Vtransaction, connVM);

                foreach (DataRow dr in dt.Rows)
                {
                    try
                    {
                        vm = new CustomerItemVM();                        
                        vm.CustomerID = dr["CustomerID"].ToString();
                        vm.CustomerCode = dr["CustomerCode"].ToString();
                        vm.CustomerName = dr["CustomerName"].ToString();
                        vm.Attention = dr["Attention"].ToString();
                        vm.Notes = dr["Notes"].ToString();
                        //vm.TransactionType = dr["TransactionType"].ToString();
                        vm.TotalItem = Convert.ToInt32(dr["TotalItem"].ToString());
                        vm.TotalValue = Convert.ToDecimal(dr["TotalValue"].ToString());
                       
                        VMs.Add(vm);
                    }
                    catch (Exception e)
                    {

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

            catch (Exception ex)
            {
                FileLogger.Log("CustomerItemDAL", "SelectAllList", ex.ToString() + "\n" + sqlText);
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

        public DataTable SelectCustomer(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null, List<string> IDs=null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            string sqlTextParameter = "";
            DataTable dt = new DataTable();
            DataSet ds = new DataSet();
            string sqlTextGroupBy = "";

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

                #region Sql Text

                #region Sql Text

                sqlText += @"
select
 cus.CustomerID
,cus.CustomerCode
,cus.CustomerName
,count(cid.ItemNo) TotalItem
,sum(isnull(cid.Value,0)) TotalValue
,Isnull(cih.Attention,'N/A')Attention
,Isnull(cih.Notes,'N/A')Notes
--,Isnull(cih.TransactionType,'Other')TransactionType
FROM  Customers cus
left outer join CustomerItemHeaders cih on cih.CustomerID= cus.CustomerID
left outer join CustomerItemDetails cid on cid.CustomerID= cus.CustomerID
WHERE  1=1
";
                #endregion

                if (IDs != null)
                {
                    var len = IDs.Count;

                    sqlText += @" and cus.CustomerID in(";

                    for (int i = 0; i < len; i++)
                    {
                        sqlText += "'" + IDs[i] + "'";

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

                }
               

                if (Id > 0)
                {
                    sqlTextParameter += @" AND cih.Id=@Id";
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

                #endregion

                #region Group By

                sqlTextGroupBy = @"
group by 
cus.CustomerID
,cus.CustomerCode
,cus.CustomerName
,cih.Attention
,cih.Notes
,cih.TransactionType
";


                #endregion


                #region SqlExecution

                sqlText = sqlText + " " + sqlTextParameter + " " + sqlTextGroupBy;
              
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
                
                if (Id > 0)
                {
                    da.SelectCommand.Parameters.AddWithValueAndNullHandle("@Id", Id);
                }
                da.Fill(dt);

                #endregion SqlExecution

                #region Commit


                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion

                #region DataSet to DataTable

                #endregion

                #endregion
            }
            #endregion

            #region catch

            catch (Exception ex)
            {
                FileLogger.Log("CustomerItemDAL", "SelectAll", ex.ToString() + "\n" + sqlText);
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

        #endregion

        public DataTable SelectAllCustomer_Export(List<string> IDs, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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

                #region Sql Text

                #region Sql Text

                sqlText += @"
SELECT
 cus.CustomerCode
,cus.CustomerName
,pro.ProductCode
,pro.ProductName
,isnull(cid.Value,0)Value
,cih.Attention
,cih.Notes

FROM CustomerItemHeaders cih
left outer join Customers cus on cih.CustomerID= cus.CustomerID
left outer join CustomerItemDetails cid on cih.CustomerID= cid.CustomerID
left outer join Products pro on pro.ItemNo= cid.ItemNo

WHERE  1=1
";
                #endregion

                if (IDs != null)
                {
                    var len = IDs.Count;

                    sqlText += @" and cih.CustomerID in(";

                    for (int i = 0; i < len; i++)
                    {
                        sqlText += "'" + IDs[i] + "'";

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

                }

                #region SqlExecution

                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;

                da.Fill(dt);

                #endregion SqlExecution

                #endregion

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion

                #endregion
            }

            #endregion

            #region catch

            catch (Exception ex)
            {
                FileLogger.Log("CustomerItemDAL", "SelectAllCustomer_Export", ex.ToString() + "\n" + sqlText);
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


        #region Customer Bill Process

        public ResultVM AddCustomerBillProcess(CustomerBillProcessVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {

            #region Variables

            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int countId = 0;
            string sqlText = "";

            ResultVM rVM = new ResultVM();

            #endregion

            #region Try

            try
            {
                #region Validation

                if (string.IsNullOrEmpty(vm.CustomerID))
                {
                    throw new ArgumentNullException("UpdateToCustomer", "Invalid Customer bill process");
                }

                #endregion Validation

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

                #region Customer  name existence checking

                sqlText = "select count(CustomerID) from CustomerBillProcess where  CustomerID=@CustomerID";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;
                cmdExist.Parameters.AddWithValueAndNullHandle("@CustomerID", vm.CustomerID);

                countId = (int)cmdExist.ExecuteScalar();

                if (countId > 0)
                {
                    retResults = UpdateToCustomerBillProcess(vm, currConn, transaction, connVM);

                }
                else
                {
                    retResults = InsertToCustomerBillProcess(vm, currConn, transaction, connVM);
                }

                rVM.Status = retResults[0];
                rVM.Message = retResults[1];

                #endregion Customer Group name existence checking

                #region Commit

                if (Vtransaction == null)
                {
                    transaction.Commit();

                }
                #endregion Commit

            }

            #endregion

            #region Catch

            catch (Exception ex)
            {
                rVM.Status = "Fail";
                rVM.Message = ex.Message.Split(new[] { '\r', '\n' }).FirstOrDefault();

                if (VcurrConn == null && transaction != null)
                {
                    transaction.Rollback();
                }

                FileLogger.Log("CustomerItemDAL", "AddCustomerBillProcess", ex.ToString() + "\n" + sqlText);

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

        public string[] InsertToCustomerBillProcess(CustomerBillProcessVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
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

                #region Inser new customer Bill
                sqlText = "";
                sqlText += "insert into CustomerBillProcess";
                sqlText += "(";
                sqlText += "CustomerID,";
                sqlText += "Jan,";
                sqlText += "Feb,";
                sqlText += "Mar,";
                sqlText += "Apr,";
                sqlText += "May,";
                sqlText += "Jun,";
                sqlText += "Jul,";
                sqlText += "Aug,";
                sqlText += "Sep,";
                sqlText += "Oct,";
                sqlText += "Nov,";
                sqlText += "Dec,";
                sqlText += "CreatedBy,";
                sqlText += "CreatedOn";
                sqlText += ")";
                sqlText += " values(";
                sqlText += "@CustomerID,";
                sqlText += "@Jan,";
                sqlText += "@Feb,";
                sqlText += "@Mar,";
                sqlText += "@Apr,";
                sqlText += "@May,";
                sqlText += "@Jun,";
                sqlText += "@Jul,";
                sqlText += "@Aug,";
                sqlText += "@Sep,";
                sqlText += "@Oct,";
                sqlText += "@Nov,";
                sqlText += "@Dec,";
                sqlText += "@CreatedBy,";
                sqlText += "@CreatedOn";
                sqlText += ") SELECT SCOPE_IDENTITY()";

                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;
                cmdInsert.Parameters.AddWithValueAndNullHandle("@CustomerID", vm.CustomerID);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Jan", vm.Jan);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Feb", vm.Feb);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Mar", vm.Mar);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Apr", vm.Apr);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@May", vm.May);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Jun", vm.Jun);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Jul", vm.Jul);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Aug", vm.Aug);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Sep", vm.Sep);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Oct", vm.Oct);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Nov", vm.Nov);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Dec", vm.Dec);

                cmdInsert.Parameters.AddWithValueAndNullHandle("@CreatedBy", vm.CreatedBy);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@CreatedOn", vm.CreatedOn);
                transResult = (int)cmdInsert.ExecuteNonQuery();

                if (transResult > 0)
                {
                    retResults[0] = "Success";
                    retResults[1] = "Requested information successfully Added";
                    retResults[2] = "" + transResult;
                }

                #region Commit

                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        if (transResult > 0)
                        {
                            transaction.Commit();
                            retResults[0] = "Success";
                            retResults[1] = "Requested information successfully Added";
                            retResults[2] = "" + transResult;

                        }
                        else
                        {
                            transaction.Rollback();
                            retResults[0] = "Fail";
                            retResults[1] = "Unexpected erro to add information";
                            retResults[2] = "";
                            retResults[3] = "";
                        }

                    }
                    else
                    {
                        retResults[0] = "Fail";
                        retResults[1] = "Unexpected erro to add information ";
                        retResults[2] = "";
                        retResults[3] = "";
                    }
                }
                #endregion Commit

                #endregion Commit

                #endregion Inser new customer Bill

            }
            #endregion

            #region catch
          
            catch (Exception ex)
            {

                transaction.Rollback();

                FileLogger.Log("CustomerItemDAL", "InsertToCustomerBillProcess", ex.ToString() + "\n" + sqlText);

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

            return retResults;

        }

        public string[] UpdateToCustomerBillProcess(CustomerBillProcessVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            string sqlText = "";

            #endregion

            #region Try

            try
            {
                #region Validation
                if (string.IsNullOrEmpty(vm.CustomerID.ToString()))
                {
                    throw new ArgumentNullException("UpdateToCustomers", "Invalid Customer  Bill");
                }

                #endregion Validation

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

                #region Update new customer group
                sqlText = "";
                sqlText = "update CustomerBillProcess set";
                sqlText += "    Jan             =@Jan";
                sqlText += "   ,Feb             =@Feb";
                sqlText += "   ,Mar             =@Mar";
                sqlText += "   ,Apr             =@Apr";
                sqlText += "   ,May             =@May";
                sqlText += "   ,Jun             =@Jun";
                sqlText += "   ,Jul             =@Jul";
                sqlText += "   ,Aug             =@Aug";
                sqlText += "   ,Sep             =@Sep";
                sqlText += "   ,Oct             =@Oct";
                sqlText += "   ,Nov             =@Nov";
                sqlText += "   ,Dec             =@Dec";
                sqlText += "   ,LastModifiedBy             =@LastModifiedBy";
                sqlText += "   ,LastModifiedOn             =@LastModifiedOn";

                sqlText += "  where CustomerID         =@CustomerID";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@CustomerID", vm.CustomerID);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@Jan", vm.Jan);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@Feb", vm.Feb);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@Mar", vm.Mar);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@Apr", vm.Apr);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@May", vm.May);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@Jun", vm.Jun);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@Jul", vm.Jul);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@Aug", vm.Aug);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@Sep", vm.Sep);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@Oct", vm.Oct);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@Nov", vm.Nov);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@Dec", vm.Dec);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@LastModifiedBy", vm.LastModifiedBy);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@LastModifiedOn", vm.LastModifiedOn);

                transResult = (int)cmdUpdate.ExecuteNonQuery();

                if (transResult > 0)
                {
                    retResults[0] = "Success";
                    retResults[1] = "Requested information successfully Updated";
                    retResults[2] = "" + transResult;
                }

                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        if (transResult > 0)
                        {
                            transaction.Commit();
                            retResults[0] = "Success";
                            retResults[1] = "Requested information successfully Update";
                            retResults[2] = vm.Id.ToString();
                            retResults[3] = "";

                        }
                        else
                        {
                            transaction.Rollback();
                            retResults[0] = "Fail";
                            retResults[1] = "Unexpected error to update customers bill information";
                        }

                    }
                    else
                    {
                        retResults[0] = "Fail";
                        retResults[1] = "Unexpected error to update customer bill information";
                    }
                }

                #endregion Commit

                #endregion

            }
            #endregion

            #region Catch

            catch (Exception ex)
            {

                transaction.Rollback();

                FileLogger.Log("CustomerItemDAL", "UpdateToCustomerBillProcess", ex.ToString() + "\n" + sqlText);

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


            return retResults;
        }

        public DataTable SelectAllCustomerBillProcess(string CustomerID = "", string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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

                #region Sql Text

                #region Sql Text

                sqlText += @"
SELECT Id
      ,CustomerID
      ,Jan
      ,Feb
      ,Mar
      ,Apr
      ,May
      ,Jun
      ,Jul
      ,Aug
      ,Sep
      ,Oct
      ,Nov
      ,Dec
      ,CreatedBy
      ,CreatedOn
      ,LastModifiedBy
      ,LastModifiedOn
  FROM CustomerBillProcess
WHERE  1=1
";
                #endregion

                if (!string.IsNullOrWhiteSpace(CustomerID))
                {
                    sqlText += " AND CustomerID=@CustomerID";
                }
                string cField = "";
                if (conditionFields != null && conditionValues != null && conditionFields.Length == conditionValues.Length)
                {
                    for (int i = 0; i < conditionFields.Length; i++)
                    {
                        if (string.IsNullOrEmpty(conditionFields[i]) || string.IsNullOrEmpty(conditionValues[i]))
                        {
                            continue;
                        }
                        cField = conditionFields[i].ToString();
                        cField = OrdinaryVATDesktop.StringReplacing(cField);
                        sqlText += " AND " + conditionFields[i] + "=@" + cField;
                    }
                }

                #region SqlExecution

                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);

                if (!string.IsNullOrWhiteSpace(CustomerID))
                {
                    objComm.Parameters.AddWithValueAndNullHandle("@CustomerID", CustomerID);
                }
                if (conditionFields != null && conditionValues != null && conditionFields.Length == conditionValues.Length)
                {
                    for (int j = 0; j < conditionFields.Length; j++)
                    {
                        if (string.IsNullOrEmpty(conditionFields[j]) || string.IsNullOrEmpty(conditionValues[j]))
                        {
                            continue;
                        }
                        cField = conditionFields[j].ToString();
                        cField = OrdinaryVATDesktop.StringReplacing(cField);
                        objComm.Parameters.AddWithValueAndNullHandle("@" + cField, conditionValues[j]);
                    }
                }

                SqlDataAdapter da = new SqlDataAdapter(objComm);

                da.Fill(dt);

                #endregion SqlExecution

                #endregion

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion

                #endregion
            }

            #endregion

            #region catch

            catch (Exception ex)
            {
                FileLogger.Log("CustomerItemDAL", "SelectAllCustomerBillProcess", ex.ToString() + "\n" + sqlText);
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

        public List<CustomerBillProcessVM> SelectAllCustomerBillProcessList(string CustomerID = "", string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            List<CustomerBillProcessVM> VMs = new List<CustomerBillProcessVM>();
            CustomerBillProcessVM vm;
            #endregion

            #region try

            try
            {
                #region sql statement

                #region SqlExecution

                DataTable dt = SelectAllCustomerBillProcess(CustomerID, conditionFields, conditionValues, VcurrConn, Vtransaction, connVM);

                foreach (DataRow dr in dt.Rows)
                {
                    try
                    {
                        vm = new CustomerBillProcessVM();
                        vm.Id = Convert.ToInt32(dr["Id"].ToString());
                        vm.CustomerID = dr["CustomerID"].ToString();
                        vm.Jan = dr["Jan"].ToString();
                        vm.Feb = dr["Feb"].ToString();
                        vm.Mar = dr["Mar"].ToString();
                        vm.Apr = dr["Apr"].ToString();
                        vm.May = dr["May"].ToString();
                        vm.Jun = dr["Jun"].ToString();
                        vm.Jul = dr["Jul"].ToString();
                        vm.Aug = dr["Aug"].ToString();
                        vm.Sep = dr["Sep"].ToString();
                        vm.Oct = dr["Oct"].ToString();
                        vm.Nov = dr["Nov"].ToString();
                        vm.Dec = dr["Dec"].ToString();

                        vm.JanChecked = vm.Jan == "Y";
                        vm.FebChecked = vm.Feb == "Y";
                        vm.MarChecked = vm.Mar == "Y";
                        vm.AprChecked = vm.Apr == "Y";
                        vm.MayChecked = vm.May == "Y";
                        vm.JunChecked = vm.Jun == "Y";
                        vm.JulChecked = vm.Jul == "Y";
                        vm.AugChecked = vm.Aug == "Y";
                        vm.SepChecked = vm.Sep == "Y";
                        vm.OctChecked = vm.Oct == "Y";
                        vm.NovChecked = vm.Nov == "Y";
                        vm.DecChecked = vm.Dec == "Y";
                        
                        vm.CreatedBy = dr["CreatedBy"].ToString();
                        vm.CreatedOn = dr["CreatedOn"].ToString();
                        vm.LastModifiedBy = dr["LastModifiedBy"].ToString();
                        vm.LastModifiedOn = dr["LastModifiedOn"].ToString();

                        VMs.Add(vm);
                    }
                    catch (Exception e)
                    {
                    }

                }


                #endregion SqlExecution

                #endregion
            }
            #endregion

            #region catch

            catch (Exception ex)
            {
                FileLogger.Log("CustomerItemDAL", "SelectAllCustomerBillProcessList", ex.ToString());
                throw new ArgumentNullException("", ex.Message.ToString());

            }
            #endregion
            #region finally
           
            #endregion
            return VMs;
        }

        #endregion

    }
}
