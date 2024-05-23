using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using SymphonySofttech.Utilities;
using VATViewModel.DTOs;
using VATServer.Ordinary;
using VATServer.Interface;

namespace VATServer.Library
{
    public class TenderDAL : ITender
    {
        #region Global Variables

        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private static string PassPhrase = DBConstant.PassPhrase;
        private static string EnKey = DBConstant.EnKey;

        #endregion

        #region Old Methods
        //currConn to VcurrConn 25-Aug-2020
        public string[] TenderInsert(TenderMasterVM Master, List<TenderDetailVM> Details, SqlTransaction Vtransaction, SqlConnection VcurrConn, SysDBInfoVMTemp connVM = null)
        {

            #region Initializ
            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";

            //SqlConnection currConn = null;
            //SqlTransaction transaction = null;
            int transResult = 0;
            string sqlText = "";

            int Present = 0;
            int newID = 0;
            string PostStatus = "";

            int IDExist = 0;

            SqlConnection vcurrConn = VcurrConn;
            if (vcurrConn == null)
            {
                VcurrConn = null;
                Vtransaction = null;
            }


            #endregion Initializ

            #region Try
            try
            {
                #region Validation for Header


                if (Master == null)
                {
                    throw new ArgumentNullException(MessageVM.tpMsgMethodNameInsert, MessageVM.PurchasemsgNoDataToSave);
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

                    Vtransaction = VcurrConn.BeginTransaction(MessageVM.tpMsgMethodNameInsert);
                }

                //currConn = _dbsqlConnection.GetConnection(connVM);
                //if (currConn.State != ConnectionState.Open)
                //{
                //    currConn.Open();
                //}

                //transaction = currConn.BeginTransaction(MessageVM.tpMsgMethodNameInsert);


                #endregion open connection and transaction

                #region Find Transaction Exist

                sqlText = "";
                sqlText = sqlText + "select COUNT(TenderId) from TenderHeaders WHERE TenderId=@TenderId ";
                SqlCommand cmdExistTran = new SqlCommand(sqlText, VcurrConn);
                cmdExistTran.Transaction = Vtransaction;
                IDExist = (int)cmdExistTran.ExecuteScalar();


                SqlParameter parameter = new SqlParameter("@TenderId", SqlDbType.VarChar, 20);
                parameter.Value = Master.TenderId;
                cmdExistTran.Parameters.Add(parameter);


                if (IDExist > 0)
                {
                    throw new ArgumentNullException(MessageVM.tpMsgMethodNameInsert, MessageVM.tpMsgFindExistID);
                }

                #endregion Find Transaction Exist

                #region Reference Number Check
                sqlText = "";
                sqlText += " select count(distinct RefNo) from TenderHeaders where  RefNo='" + Master.RefNo + "'";
                sqlText += " and CustomerId='" + Master.CustomerId + "'";
                SqlCommand cmdIDR = new SqlCommand(sqlText, VcurrConn);
                parameter = new SqlParameter("@RefNo", SqlDbType.VarChar, 200);
                parameter.Value = Master.RefNo;
                cmdIDR.Parameters.Add(parameter);

                cmdIDR.Transaction = Vtransaction;
                Present = (int)cmdIDR.ExecuteScalar();

                if (Present > 0)
                {
                    throw new ArgumentNullException(MessageVM.tpMsgMethodNameInsert, MessageVM.tpMsgFindRef);
                }
                #endregion Reference Number Check

                #region ID Create
                sqlText = "";
                sqlText = "select isnull(max(cast(TenderId as int)),0)+1 FROM  TenderHeaders";

                //sqlText = sqlText + "select max(isnull(cast(TenderId as int),0))+1 FROM  TenderHeaders";
                SqlCommand cmdIDc = new SqlCommand(sqlText, VcurrConn);
                cmdIDc.Transaction = Vtransaction;
                newID = (int)cmdIDc.ExecuteScalar();

                if (newID <= 0)
                {
                    throw new ArgumentNullException(MessageVM.tpMsgMethodNameInsert, MessageVM.tpMsgIDNotCreate);
                }
                #endregion ID Create
                #region ID generated completed,Insert new Information in Header

                sqlText = "";
                sqlText += " insert into TenderHeaders";
                sqlText += " (";
                sqlText += " TenderId,";
                sqlText += " RefNo,";
                sqlText += " CustomerId,";
                sqlText += " TenderDate,";
                sqlText += " DeleveryDate,";
                sqlText += " CreatedBy,";
                sqlText += " CreatedOn,";
                sqlText += " LastModifiedBy,";
                sqlText += " LastModifiedOn,";
                sqlText += " Comments,";
                sqlText += " BranchId,";
                sqlText += " CustomerGroupID";

                sqlText += " )";

                sqlText += " values";
                sqlText += " (";
                sqlText += "@newID,";
                sqlText += "@MasterRefNo,";
                sqlText += "@MasterCustomerId,";
                sqlText += "@MasterTenderDate,";
                sqlText += "@MasterDeleveryDate,";
                sqlText += "@MasterCreatedBy,";
                sqlText += "@MasterCreatedOn,";
                sqlText += "@MasterLastModifiedBy,";
                sqlText += "@MasterLastModifiedOn,";
                sqlText += "@MasterComments,";
                sqlText += "@MasterBranchId,";
                sqlText += "@MasterCustomerGrpID";
                sqlText += ")";


                SqlCommand cmdInsert = new SqlCommand(sqlText, VcurrConn);
                cmdInsert.Transaction = Vtransaction;

                cmdInsert.Parameters.AddWithValueAndNullHandle("@newID", newID);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterRefNo", Master.RefNo);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterCustomerId", Master.CustomerId);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterTenderDate", Master.TenderDate);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterDeleveryDate", Master.DeleveryDate);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterCreatedBy", Master.CreatedBy);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterCreatedOn", Master.CreatedOn);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterComments", Master.Comments);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterBranchId", Master.BranchId);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterCustomerGrpID", Master.CustomerGrpID);

                transResult = (int)cmdInsert.ExecuteNonQuery();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.tpMsgMethodNameInsert, MessageVM.tpMsgSaveNotSuccessfully);
                }

                #endregion ID generated completed,Insert new Information in Header

                #region Insert into Details(Insert complete in Header)
                #region Validation for Detail



                if (Details.Count() < 0)
                {
                    throw new ArgumentNullException(MessageVM.tpMsgMethodNameInsert, MessageVM.tpMsgNoDataToSave);
                }


                #endregion Validation for Detail

                #region Insert Detail Table

                foreach (var Item in Details.ToList())
                {
                    #region Find Transaction Exist

                    sqlText = "";
                    sqlText += "select COUNT(TenderId) from TenderDetails WHERE TenderId=@newID  and ItemNo = @ItemNo";

                    SqlCommand cmdFindId = new SqlCommand(sqlText, VcurrConn);

                    parameter = new SqlParameter("@ItemNo", SqlDbType.VarChar, 20);
                    parameter.Value = Item.ItemNo;
                    cmdFindId.Parameters.Add(parameter);

                    parameter = new SqlParameter("@newID", SqlDbType.VarChar, 20);
                    parameter.Value = newID;
                    cmdFindId.Parameters.Add(parameter);


                    cmdFindId.Transaction = Vtransaction;
                    IDExist = (int)cmdFindId.ExecuteScalar();

                    if (IDExist > 0)
                    {
                        throw new ArgumentNullException(MessageVM.tpMsgMethodNameInsert, MessageVM.tpMsgFindExistID);
                    }

                    #endregion Find Transaction Exist

                    #region Insert only DetailTable

                    sqlText = "";
                    sqlText += " insert into TenderDetails(";

                    sqlText += " TenderId,";
                    sqlText += " ItemNo,";
                    sqlText += " TenderQty,";
                    sqlText += " TenderPrice,";
                    sqlText += " CreatedBy,";
                    sqlText += " CreatedOn,";
                    sqlText += " LastModifiedBy,";
                    sqlText += " BOMId,";
                    sqlText += " BranchId,";
                    sqlText += " LastModifiedOn";

                    sqlText += " )";
                    sqlText += " values(	";
                    //sqlText += "'" + Master.Id + "',";
                    sqlText += "@newID,";
                    sqlText += "@ItemItemNo,";
                    sqlText += "@ItemTenderQty,";
                    sqlText += "@ItemTenderPrice,";
                    sqlText += "@MasterCreatedBy,";
                    sqlText += "@MasterCreatedOn,";
                    sqlText += "@MasterLastModifiedBy,";
                    sqlText += "@ItemBOMId,";
                    sqlText += "@ItemBranchId,";
                    sqlText += "@MasterLastModifiedOn";

                    sqlText += ")	";


                    SqlCommand cmdInsDetail = new SqlCommand(sqlText, VcurrConn);
                    cmdInsDetail.Transaction = Vtransaction;
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@newID", newID);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemTenderQty", Item.TenderQty);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemTenderPrice", Item.TenderPrice);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterCreatedBy", Master.CreatedBy);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterCreatedOn", Master.CreatedOn);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemBOMId", Item.BOMId);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemBranchId", Item.BranchId);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);


                    transResult = (int)cmdInsDetail.ExecuteNonQuery();

                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.tpMsgMethodNameInsert, MessageVM.tpMsgSaveNotSuccessfully);
                    }

                    #endregion Insert only DetailTable

                    #region Update product Tender Price

                    sqlText = "";
                    sqlText += "update Products set TenderPrice=@TenderPrice  where itemNo=@ItemNo";
                    SqlCommand cmdUpdTenderPrice = new SqlCommand(sqlText, VcurrConn);

                    parameter = new SqlParameter("@TenderPrice", SqlDbType.Decimal);
                    parameter.Precision = 25;
                    parameter.Scale = 9;
                    parameter.Value = Item.TenderPrice;
                    cmdUpdTenderPrice.Parameters.Add(parameter);

                    parameter = new SqlParameter("@ItemNo", SqlDbType.VarChar, 20);
                    parameter.Value = Item.ItemNo;
                    cmdUpdTenderPrice.Parameters.Add(parameter);

                    cmdUpdTenderPrice.Transaction = Vtransaction;
                    transResult = (int)cmdUpdTenderPrice.ExecuteNonQuery();

                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.tpMsgMethodNameInsert, MessageVM.tpMsgSaveNotSuccessfully);
                    }

                    #endregion Update product Tender Price
                }


                #endregion Insert Detail Table
                #endregion Insert into Details(Insert complete in Header)

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
                retResults[1] = MessageVM.tpMsgSaveSuccessfully;
                retResults[2] = "" + newID;
                #endregion SuccessResult

            }
            #endregion Try

            #region Catch and Finall
            catch (SqlException sqlex)
            {
                if (vcurrConn == null)
                {
                    Vtransaction.Rollback();
                }
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                if (vcurrConn == null)
                {
                    Vtransaction.Rollback();
                }
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
            }
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
        public string[] TenderUpdate(TenderMasterVM Master, List<TenderDetailVM> Details, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            int Present = 0;
            string MLock = "";
            string YLock = "";
            DateTime MinDate = DateTime.MinValue;
            DateTime MaxDate = DateTime.MaxValue;

            int count = 0;
            string n = "";
            string Prefetch = "";
            int Len = 0;
            int SetupLen = 0;
            int CurrentID = 0;
            string newID = "";
            int IssueInsertSuccessfully = 0;
            int ReceiveInsertSuccessfully = 0;
            string PostStatus = "";

            int SetupIDUpdate = 0;
            int InsertDetail = 0;


            #endregion Initializ

            #region Try
            try
            {
                #region Validation for Header

                if (Master == null)
                {
                    throw new ArgumentNullException(MessageVM.tpMsgMethodNameUpdate, MessageVM.PurchasemsgNoDataToUpdate);
                }

                #endregion Validation for Header
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                transaction = currConn.BeginTransaction(MessageVM.tpMsgMethodNameInsert);


                #endregion open connection and transaction
                #region Find ID for Update

                sqlText = "";
                sqlText = sqlText + "select COUNT(TenderId) from TenderHeaders WHERE TenderId=@TenderId ";
                SqlCommand cmdFindIdUpd = new SqlCommand(sqlText, currConn);

                SqlParameter parameter = new SqlParameter("@TenderId", SqlDbType.VarChar, 20);
                parameter.Value = Master.TenderId;
                cmdFindIdUpd.Parameters.Add(parameter);


                cmdFindIdUpd.Transaction = transaction;
                int IDExist = (int)cmdFindIdUpd.ExecuteScalar();

                if (IDExist <= 0)
                {
                    throw new ArgumentNullException(MessageVM.tpMsgMethodNameUpdate, MessageVM.tpMsgUnableFindExistID);
                }

                #endregion Find ID for Update



                #region ID check completed,update Information in Header

                sqlText = "";

                sqlText += " update TenderHeaders set  ";

                sqlText += " RefNo= @RefNo,";
                sqlText += " CustomerId= @CustomerId ,";
                sqlText += " TenderDate= @TenderDate ,";
                sqlText += " DeleveryDate= @DeleveryDate ,";
                sqlText += " LastModifiedBy= @LastModifiedBy ,";
                sqlText += " LastModifiedOn= @LastModifiedOn ,";
                sqlText += " CustomerGroupID= @CustomerGrpID ,";
                sqlText += " Comments= @Comments ";
                sqlText += " where  TenderId = @TenderId ";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;
                transResult = (int)cmdUpdate.ExecuteNonQuery();

                parameter = new SqlParameter("@RefNo", SqlDbType.VarChar, 200);
                parameter.Value = Master.RefNo;
                cmdUpdate.Parameters.Add(parameter);

                parameter = new SqlParameter("@CustomerId", SqlDbType.VarChar, 20);
                parameter.Value = Master.CustomerId;
                cmdUpdate.Parameters.Add(parameter);

                parameter = new SqlParameter("@TenderDate", SqlDbType.DateTime);
                parameter.Value = Master.TenderDate;
                cmdUpdate.Parameters.Add(parameter);

                parameter = new SqlParameter("@DeleveryDate", SqlDbType.DateTime);
                parameter.Value = Master.DeleveryDate;
                cmdUpdate.Parameters.Add(parameter);

                parameter = new SqlParameter("@LastModifiedBy", SqlDbType.VarChar, 120);
                parameter.Value = Master.LastModifiedBy;
                cmdUpdate.Parameters.Add(parameter);

                parameter = new SqlParameter("@LastModifiedOn", SqlDbType.DateTime);
                parameter.Value = Master.LastModifiedOn;
                cmdUpdate.Parameters.Add(parameter);


                parameter = new SqlParameter("@CustomerGrpID", SqlDbType.VarChar, 20);
                parameter.Value = Master.CustomerGrpID;
                cmdUpdate.Parameters.Add(parameter);

                parameter = new SqlParameter("@Comments", SqlDbType.VarChar, 200);
                parameter.Value = Master.Comments;
                cmdUpdate.Parameters.Add(parameter);

                parameter = new SqlParameter("@TenderId", SqlDbType.VarChar, 20);
                parameter.Value = Master.TenderId;
                cmdUpdate.Parameters.Add(parameter);


                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.tpMsgMethodNameUpdate, MessageVM.PurchasemsgUpdateNotSuccessfully);
                }

                #endregion ID check completed,update Information in Header

                #region Update into Details(Update complete in Header)
                #region Validation for Detail

                if (Details.Count() < 0)
                {
                    throw new ArgumentNullException(MessageVM.tpMsgMethodNameUpdate, MessageVM.PurchasemsgNoDataToUpdate);
                }


                #endregion Validation for Detail

                #region Update Detail Table

                foreach (var Item in Details.ToList())
                {
                    #region Find Transaction Mode Insert or Update

                    sqlText = "";
                    sqlText += "select COUNT(TenderId) from TenderDetails WHERE TenderId=@TenderId ";
                    sqlText += " AND ItemNo='" + Item.ItemNo + "'";
                    SqlCommand cmdFindId = new SqlCommand(sqlText, currConn);
                    cmdFindId.Transaction = transaction;
                    IDExist = (int)cmdFindId.ExecuteScalar();

                    parameter = new SqlParameter("@TenderId", SqlDbType.VarChar, 20);
                    parameter.Value = Master.TenderId;
                    cmdFindId.Parameters.Add(parameter);

                    if (IDExist <= 0)
                    {
                        // Insert
                        #region ID generated completed,Insert new Information in Header

                        sqlText = "";
                        sqlText += " insert into TenderDetails(";

                        sqlText += " TenderId,";
                        sqlText += " ItemNo,";
                        sqlText += " TenderQty,";
                        sqlText += " TenderPrice,";
                        sqlText += " CreatedBy,";
                        sqlText += " CreatedOn,";
                        sqlText += " LastModifiedBy,";
                        sqlText += " LastModifiedOn";

                        sqlText += " )";
                        sqlText += " values(	";
                        //sqlText += "'" + Master.Id + "',";
                        sqlText += "@TenderId,";

                        sqlText += "@ItemNo,";
                        sqlText += "@TenderQty,";
                        sqlText += "@TenderPrice,";
                        sqlText += "@CreatedBy,";
                        sqlText += "@CreatedOn,";
                        sqlText += "@LastModifiedBy,";
                        sqlText += "@LastModifiedOn";

                        sqlText += ")	";


                        SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                        cmdInsert.Transaction = transaction;
                        transResult = (int)cmdInsert.ExecuteNonQuery();

                        parameter = new SqlParameter("@TenderId", SqlDbType.VarChar, 20);
                        parameter.Value = Master.TenderId;
                        cmdInsert.Parameters.Add(parameter);

                        parameter = new SqlParameter("@ItemNo", SqlDbType.VarChar, 20);
                        parameter.Value = Item.ItemNo;
                        cmdInsert.Parameters.Add(parameter);

                        parameter = new SqlParameter("@TenderQty", SqlDbType.Decimal);
                        parameter.Precision = 25;
                        parameter.Scale = 9;
                        parameter.Value = Item.TenderQty;
                        cmdInsert.Parameters.Add(parameter);

                        parameter = new SqlParameter("@TenderPrice", SqlDbType.Decimal);
                        parameter.Precision = 25;
                        parameter.Scale = 9;
                        parameter.Value = Item.TenderPrice;
                        cmdInsert.Parameters.Add(parameter);

                        parameter = new SqlParameter("@CreatedBy", SqlDbType.VarChar, 120);
                        parameter.Value = Master.CreatedBy;
                        cmdInsert.Parameters.Add(parameter);

                        parameter = new SqlParameter("@CreatedOn", SqlDbType.DateTime);
                        parameter.Value = Master.CreatedOn;
                        cmdInsert.Parameters.Add(parameter);

                        parameter = new SqlParameter("@LastModifiedBy", SqlDbType.VarChar, 120);
                        parameter.Value = Master.LastModifiedBy;
                        cmdInsert.Parameters.Add(parameter);

                        parameter = new SqlParameter("@LastModifiedOn", SqlDbType.DateTime);
                        parameter.Value = Master.LastModifiedOn;
                        cmdInsert.Parameters.Add(parameter);

                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.tpMsgMethodNameInsert, MessageVM.tpMsgUpdateNotSuccessfully);
                        }

                        #endregion ID generated completed,Insert new Information in Header

                    }
                    else
                    {
                        //Update

                        #region Update only DetailTable

                        sqlText = "";

                        sqlText += " update TenderDetails set ";
                        sqlText += " TenderQty = @TenderQty,";
                        sqlText += " TenderPrice = @TenderPrice,";
                        sqlText += " LastModifiedBy = @LastModifiedBy,";
                        sqlText += " LastModifiedOn = @LastModifiedOn";
                        sqlText += " where  TenderId  = @TenderId";
                        sqlText += " and ItemNo = @ItemNo";

                        SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                        cmdInsDetail.Transaction = transaction;
                        transResult = (int)cmdInsDetail.ExecuteNonQuery();

                        parameter = new SqlParameter("@TenderQty", SqlDbType.Decimal);
                        parameter.Precision = 25;
                        parameter.Scale = 9;
                        parameter.Value = Item.TenderQty;
                        cmdInsDetail.Parameters.Add(parameter);

                        parameter = new SqlParameter("@TenderPrice", SqlDbType.Decimal);
                        parameter.Precision = 25;
                        parameter.Scale = 9;
                        parameter.Value = Item.TenderPrice;
                        cmdInsDetail.Parameters.Add(parameter);

                        parameter = new SqlParameter("@LastModifiedBy", SqlDbType.VarChar, 120);
                        parameter.Value = Master.LastModifiedBy;
                        cmdInsDetail.Parameters.Add(parameter);

                        parameter = new SqlParameter("@LastModifiedOn", SqlDbType.DateTime);
                        parameter.Value = Master.LastModifiedOn;
                        cmdInsDetail.Parameters.Add(parameter);

                        parameter = new SqlParameter("@TenderId", SqlDbType.VarChar, 20);
                        parameter.Value = Master.TenderId;
                        cmdInsDetail.Parameters.Add(parameter);

                        parameter = new SqlParameter("@ItemNo", SqlDbType.VarChar, 20);
                        parameter.Value = Item.ItemNo;
                        cmdInsDetail.Parameters.Add(parameter);

                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.tpMsgMethodNameUpdate, MessageVM.tpMsgUpdateNotSuccessfully);
                        }
                        #endregion Update only DetailTable

                    }

                    #endregion Find Transaction Mode Insert or Update
                    #region Update product Tender Price

                    sqlText = "";
                    sqlText += "update Products set TenderPrice=@TenderPrice  where itemNo=@ItemNo";
                    SqlCommand cmdUpdTenderPrice = new SqlCommand(sqlText, currConn);
                    cmdUpdTenderPrice.Transaction = transaction;
                    transResult = (int)cmdUpdTenderPrice.ExecuteNonQuery();

                    parameter = new SqlParameter("@TenderPrice", SqlDbType.Decimal);
                    parameter.Precision = 25;
                    parameter.Scale = 9;
                    parameter.Value = Item.TenderPrice;
                    cmdUpdTenderPrice.Parameters.Add(parameter);

                    parameter = new SqlParameter("@ItemNo", SqlDbType.VarChar, 20);
                    parameter.Value = Item.ItemNo;
                    cmdUpdTenderPrice.Parameters.Add(parameter);

                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.tpMsgMethodNameInsert, MessageVM.tpMsgUpdateNotSuccessfully);
                    }

                    #endregion Update product Tender Price
                }

                #region Remove row
                sqlText = "";
                sqlText += " SELECT  distinct ItemNo";
                sqlText += " from TenderDetails WHERE TenderId=@TenderId";

                DataTable dt = new DataTable("Previous");
                SqlCommand cmdRIF = new SqlCommand(sqlText, currConn);
                cmdRIF.Transaction = transaction;
                SqlDataAdapter dta = new SqlDataAdapter(cmdRIF);

                parameter = new SqlParameter("@TenderId", SqlDbType.VarChar, 20);
                parameter.Value = Master.TenderId;
                cmdRIF.Parameters.Add(parameter);

                dta.Fill(dt);
                foreach (DataRow pItem in dt.Rows)
                {
                    var p = pItem["ItemNo"].ToString();

                    //var tt= Details.Find(x => x.ItemNo == p);
                    var tt = Details.Count(x => x.ItemNo.Trim() == p.Trim());
                    if (tt == 0)
                    {
                        sqlText = "";
                        sqlText += " delete FROM TenderDetails ";
                        sqlText += " WHERE TenderId=@TenderId ";
                        sqlText += " AND ItemNo=@ItemNo";
                        SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                        cmdInsDetail.Transaction = transaction;
                        transResult = (int)cmdInsDetail.ExecuteNonQuery();

                        parameter = new SqlParameter("@TenderId", SqlDbType.VarChar, 20);
                        parameter.Value = Master.TenderId;
                        cmdInsDetail.Parameters.Add(parameter);

                        parameter = new SqlParameter("@ItemNo", SqlDbType.VarChar, 20);
                        parameter.Value = pItem["ItemNo"].ToString();
                        cmdInsDetail.Parameters.Add(parameter);

                    }

                }

                #endregion Remove row

                #endregion Update Detail Table

                #endregion  Update into Details(Update complete in Header)


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
                retResults[1] = MessageVM.tpMsgUpdateSuccessfully;
                retResults[2] = Master.TenderId;
                #endregion SuccessResult

            }
            #endregion Try

            #region Catch and Finall
            catch (SqlException sqlex)
            {
                transaction.Rollback();
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
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

        public DataTable SearchTenderHeaderByCustomerGrp(string TenderId, string RefNo, string CustomerGrpID, SysDBInfoVMTemp connVM = null)
        {
            #region Objects & Variables

            //string Description = "";

            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable("Search Tender Header");
            #endregion
            #region try
            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                #endregion open connection and transaction
                #region sql statement
//                sqlText = @"SELECT     T.TenderId, T.RefNo, T.CustomerGroupID, ' ' CustomerName, ' ' Address1, 
//' ' Address2, ' ' Address3, convert (varchar,T.TenderDate,120)TenderDate, convert (varchar,T.DeleveryDate,120)DeleveryDate, T.Comments,cg.CustomerGroupName
//FROM         dbo.TenderHeaders AS T LEFT OUTER JOIN
//--dbo.Customers AS C ON T.CustomerId = C.CustomerID LEFT OUTER JOIN
//customergroups AS cg ON T.CustomerGroupID=cg.CustomerGroupID
//
//WHERE 
//(t.TenderId  LIKE '%' +  @TenderId   + '%' OR @TenderId IS NULL) 
//and (RefNo  LIKE '%' +  @RefNo   + '%' OR @RefNo IS NULL) 
//and (T.CustomerGroupID  LIKE '%' +  @CustomerGrpID   + '%' OR @CustomerGrpID IS NULL)
//";

                sqlText = @"
SELECT     T.TenderId, T.RefNo,' ' CustomerId, ' ' CustomerName, T.CustomerGroupID,   
convert (varchar,T.TenderDate,120)TenderDate, convert (varchar,T.DeleveryDate,120)DeleveryDate, T.Comments,cg.CustomerGroupName
FROM         dbo.TenderHeaders AS T LEFT OUTER JOIN
customergroups AS cg ON T.CustomerGroupID=cg.CustomerGroupID

WHERE 
T.CustomerGroupID <> '0' AND
(t.TenderId  LIKE '%' +  @TenderId   + '%' OR @TenderId IS NULL) 
and (RefNo  LIKE '%' +  @RefNo   + '%' OR @RefNo IS NULL) 
and (T.CustomerGroupID  LIKE '%' +  @CustomerGrpID   + '%' OR @CustomerGrpID IS NULL)


";
                SqlCommand objCommTenderHeader = new SqlCommand();
                objCommTenderHeader.Connection = currConn;
                objCommTenderHeader.CommandText = sqlText;
                objCommTenderHeader.CommandType = CommandType.Text;
                #region Parameter

                if (!objCommTenderHeader.Parameters.Contains("@TenderId"))
                { objCommTenderHeader.Parameters.AddWithValue("@TenderId", TenderId); }
                else { objCommTenderHeader.Parameters["@TenderId"].Value = TenderId; }

                if (RefNo == "")
                {
                    if (!objCommTenderHeader.Parameters.Contains("@RefNo"))
                    { objCommTenderHeader.Parameters.AddWithValue("@RefNo", System.DBNull.Value); }
                    else { objCommTenderHeader.Parameters["@RefNo"].Value = System.DBNull.Value; }
                }
                else
                {
                    if (!objCommTenderHeader.Parameters.Contains("@RefNo"))
                    { objCommTenderHeader.Parameters.AddWithValue("@RefNo", RefNo); }
                    else { objCommTenderHeader.Parameters["@RefNo"].Value = RefNo; }
                }
                if (CustomerGrpID == "")
                {
                    if (!objCommTenderHeader.Parameters.Contains("@CustomerGrpID"))
                    { objCommTenderHeader.Parameters.AddWithValue("@CustomerGrpID", System.DBNull.Value); }
                    else { objCommTenderHeader.Parameters["@CustomerGrpID"].Value = System.DBNull.Value; }
                }
                else
                {
                    if (!objCommTenderHeader.Parameters.Contains("@CustomerGrpID"))
                    { objCommTenderHeader.Parameters.AddWithValue("@CustomerGrpID", CustomerGrpID); }
                    else { objCommTenderHeader.Parameters["@CustomerGrpID"].Value = CustomerGrpID; }
                }


                #endregion Parameter

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommTenderHeader);
                dataAdapter.Fill(dataTable);
                #endregion
            }
            #endregion
            #region catch
            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
            }
            #endregion
            #region finally
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
            return dataTable;
        }

        public DataTable SearchTenderDetail(string TenderId, string transactionDate, SysDBInfoVMTemp connVM = null)//start
        {
            #region Objects & Variables

            string Description = "";

            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable("Tender");
            #endregion
            #region try
            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                #endregion open connection and transaction
                #region sql statement


                sqlText = "";
                sqlText +=
                    " SELECT td.TenderId,td.ItemNo ItemNo,isnull(isnull(td.TenderQty,0)-isnull(td.SaleQty,0),0) Stock,";
                sqlText += " isnull(td.TenderPrice,0) NBRPrice, isnull(td.Post,'N')Post, p.ProductCode ProductCode,p.ProductName ProductName";
                sqlText += " ,p.UOM,isnull(td.BOMId,0)BOMId,p.CategoryID,isnull(td.TenderQty,0)TenderQty,";
                sqlText += " pc.CategoryName CategoryName,p.HSCodeNo HSCodeNo,pc.IsRaw IsRaw,p.VATRate,";
                sqlText += " p.SD,p.NonStock,p.Trading,isnull(p.TradingMarkUp,0)TradingMarkUp";
                sqlText += " FROM TenderDetails td LEFT OUTER JOIN  ";
                sqlText += "  TenderHeaders th ON td.TenderId=th.TenderId  LEFT OUTER JOIN ";
                sqlText += "  products p ON td.ItemNo=p.ItemNo ";
                sqlText += "  left outer join ProductCategories pc ON p.CategoryID=pc.CategoryID ";
                sqlText += " WHERE td.tenderid=@TenderId";
                if (!string.IsNullOrEmpty(transactionDate))
                {
                    sqlText += " and th.TenderDate <=@transactionDate";
                }
                //sqlText += " ";
                SqlCommand objCommTenderDetail = new SqlCommand();
                objCommTenderDetail.Connection = currConn;
                objCommTenderDetail.CommandText = sqlText;
                objCommTenderDetail.CommandType = CommandType.Text;

                SqlParameter parameter = new SqlParameter("@TenderId", SqlDbType.VarChar, 20);
                parameter.Value = TenderId;
                objCommTenderDetail.Parameters.Add(parameter);

                parameter = new SqlParameter("@transactionDate", SqlDbType.DateTime);
                parameter.Value = transactionDate;
                objCommTenderDetail.Parameters.Add(parameter);

                #region Parameter
                //if (!objCommTenderDetail.Parameters.Contains("@TenderId"))
                //{ objCommTenderDetail.Parameters.AddWithValue("@TenderId", TenderId); }
                //else { objCommTenderDetail.Parameters["@TenderId"].Value = TenderId; }
                #endregion Parameter

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommTenderDetail);
                dataAdapter.Fill(dataTable);
                #endregion
            }
            #endregion
            #region catch

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
            }
            #endregion
            #region finally
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
            return dataTable;
        }

        public DataTable SearchTenderDetailSale(string TenderId, string transactiondate, SysDBInfoVMTemp connVM = null)
        {
            #region Objects & Variables

            string Description = "";

            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable("Tender");
            #endregion
            #region try
            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                #endregion open connection and transaction
                #region sql statement

                sqlText = " ";
                sqlText += @"SELECT     TD.TenderId, P.ItemNo, ISNULL(P.ProductName, 'N/A') AS ProductName, ISNULL(P.ProductCode, 'N/A') AS ProductCode, P.CategoryID, ISNULL(PC.CategoryName, 'N/A')
AS CategoryName, ISNULL(P.UOM, 'N/A') AS UOM, ISNULL(P.HSCodeNo, 'N/A') AS HSCodeNo, ISNULL(PC.IsRaw, 'N/A') AS IsRaw, ISNULL(P.CostPrice, 0) 
AS CostPrice, ISNULL(P.OpeningBalance, 0) AS OpeningBalance, ISNULL(P.SalesPrice, 0) AS SalesPrice, isnull(td.TenderPrice,0) NBRPrice, 
ISNULL(P.ReceivePrice, 0) AS ReceivePrice, ISNULL(P.IssuePrice, 0) AS IssuePrice,
ISNULL(P.PacketPrice, 0) AS PacketPrice, ISNULL(td.TenderPrice, 0)AS TenderPrice, ISNULL(P.ExportPrice, 0) AS ExportPrice,
ISNULL(P.InternalIssuePrice, 0) AS InternalIssuePrice, ISNULL(P.TollIssuePrice, 0) AS TollIssuePrice, 
ISNULL(P.TollCharge, 0) AS TollCharge, ISNULL(P.VATRate, 0) AS VATRate, ISNULL(P.SD, 0) AS SD, ISNULL(P.TradingMarkUp, 0) AS TradingMarkUp, 
isnull(ISNULL(td.TenderQty,0)-ISNULL(td.SaleQty,0),0) AS TenderStock,
isnull(isnull(p.OpeningBalance,0)+isnull(p.QuantityInHand,0),0) as Stock,
ISNULL(P.QuantityInHand, 0) AS QuantityInHand, ISNULL(P.Trading, 'N') 
AS Trading, ISNULL(P.NonStock, 'N') AS NonStock,
ISNULL(td.BOMId, '0') AS BOMId,
ISNULL(td.TenderQty,0)TenderQty

FROM         dbo.TenderDetails AS TD  LEFT OUTER JOIN 
 TenderHeaders th ON td.TenderId=th.TenderId 
INNER JOIN
dbo.Products AS P ON TD.ItemNo = P.ItemNo INNER JOIN
dbo.ProductCategories AS PC ON P.CategoryID = PC.CategoryID
WHERE 
(td.TenderId = @TenderId or  @TenderId is null)";
                if (!string.IsNullOrEmpty(transactiondate))
                {
                    sqlText += " and th.TenderDate <=@transactionDate";
                }

                //sqlText += " ";
                SqlCommand objCommTenderDetail = new SqlCommand();
                objCommTenderDetail.Connection = currConn;
                objCommTenderDetail.CommandText = sqlText;
                objCommTenderDetail.CommandType = CommandType.Text;
                #region Parameter
                if (!objCommTenderDetail.Parameters.Contains("@TenderId"))
                { objCommTenderDetail.Parameters.AddWithValue("@TenderId", TenderId); }
                else { objCommTenderDetail.Parameters["@TenderId"].Value = TenderId; }

                SqlParameter parameter = new SqlParameter("@transactionDate", SqlDbType.DateTime);
                parameter.Value = transactiondate;
                objCommTenderDetail.Parameters.Add(parameter);

                #endregion Parameter

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommTenderDetail);
                dataAdapter.Fill(dataTable);
                #endregion
            }
            #endregion
            #region catch
            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
            }
            #endregion
            #region finally
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
            return dataTable;
        }

        public DataTable SearchTenderDetailSaleNew(string TenderId, string transactiondate, SysDBInfoVMTemp connVM = null)
        {
            #region Objects & Variables

            string Description = "";

            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable("Tender");
            #endregion
            #region try
            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                #endregion open connection and transaction
                #region sql statement

                sqlText = " ";
                sqlText += @"SELECT     '0' SalesInvoiceNo,P.ItemNo,ISNULL(td.TenderQty,0)Quantity,'0' PromotionalQuantity,ISNULL(td.TenderQty,0)SaleQuantity,'0' SalePrice,
     ISNULL(td.TenderPrice, 0)AS NBRPrice,ISNULL(P.UOM, 'N/A') AS UOM,ISNULL(P.VATRate, 0) AS VATRate,'0'VATAmount,
    '0'SubTotal,'N\A'Comments, ISNULL(P.ProductName, 'N/A') AS ProductName,isnull(isnull(p.OpeningBalance,0)+isnull(p.QuantityInHand,0),0) as Stock, 
    ISNULL(P.SD, 0) AS SD,'0'SDAmount,'New',
    ISNULL(P.QuantityInHand, 0) AS QuantityInHand, ISNULL(P.Trading, 'N') AS Trading, ISNULL(P.NonStock, 'N') AS NonStock,
    ISNULL(P.TradingMarkUp, 0) AS TradingMarkUp,'VAT' Type,ISNULL(P.ProductCode, 'N/A') AS ProductCode,
    ISNULL(td.TenderQty,0)UOMQty,ISNULL(P.UOM, 'N/A') AS UOMn,'1' UOMc,isnull(td.TenderPrice,0) UOMPrice,'0' DiscountAmount,
    isnull(td.TenderPrice,0) DiscountedNBRPrice,isnull(td.TenderPrice,0) CurrencyValue,'0' DollerValue, td.BOMId AS BOMID
    

    FROM         dbo.TenderDetails AS TD  LEFT OUTER JOIN 
    TenderHeaders th ON td.TenderId=th.TenderId 
    INNER JOIN
    dbo.Products AS P ON TD.ItemNo = P.ItemNo INNER JOIN
    dbo.ProductCategories AS PC ON P.CategoryID = PC.CategoryID
    WHERE 
    (td.TenderId = @TenderId or  @TenderId is null)";
                if (!string.IsNullOrEmpty(transactiondate))
                {
                    sqlText += " and th.TenderDate <=@transactionDate";
                }

                //sqlText += " ";
                SqlCommand objCommTenderDetail = new SqlCommand();
                objCommTenderDetail.Connection = currConn;
                objCommTenderDetail.CommandText = sqlText;
                objCommTenderDetail.CommandType = CommandType.Text;

                SqlParameter parameter = new SqlParameter("@transactionDate", SqlDbType.DateTime);
                parameter.Value = transactiondate;
                objCommTenderDetail.Parameters.Add(parameter);

                #region Parameter
                if (!objCommTenderDetail.Parameters.Contains("@TenderId"))
                { objCommTenderDetail.Parameters.AddWithValue("@TenderId", TenderId); }
                else { objCommTenderDetail.Parameters["@TenderId"].Value = TenderId; }
                #endregion Parameter

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommTenderDetail);
                dataAdapter.Fill(dataTable);
                #endregion
            }
            #endregion
            #region catch
            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
            }
            #endregion
            #region finally
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
            return dataTable;
        }

        #endregion

        public string[] ImportData(DataTable dtTenderM, DataTable dtTenderD, SysDBInfoVMTemp connVM = null)
        {
            #region variable
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";

            TenderMasterVM tenderMasterVM;
            List<TenderDetailVM> tenderDetailVMs = new List<TenderDetailVM>();

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion variable
            #region  Try
            try
            {
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    currConn.Open();
                }


                #region RowCount
                int MRowCount = 0;
                int MRow = dtTenderM.Rows.Count;
                for (int i = 0; i < dtTenderM.Rows.Count; i++)
                {
                    if (!string.IsNullOrEmpty(dtTenderM.Rows[i]["ID"].ToString()))
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
                    string importID = dtTenderM.Rows[i]["ID"].ToString();
                    if (!string.IsNullOrEmpty(importID))
                    {
                        DataRow[] DetailRaws = dtTenderD.Select("ID='" + importID + "'");
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
                    string id = dtTenderM.Rows[i]["ID"].ToString();
                    DataRow[] tt = dtTenderM.Select("ID='" + id + "'");
                    if (tt.Length > 1)
                    {
                        throw new ArgumentNullException("you have duplicate master id " + id + " in import file.");
                    }

                }

                #endregion Double ID in Master

                CommonDAL commonDal = new CommonDAL();
                string vCustGrp = commonDal.settings("ImportTender", "CustomerGroup");
                //if (string.IsNullOrEmpty(vCustGrp))
                //{
                //    return;
                //}

                bool ImportByCustGrp = Convert.ToBoolean(vCustGrp == "Y" ? true : false);



                CommonImportDAL cImport = new CommonImportDAL();

                #region checking from database is exist the information(NULL Check)

                #region Master

                for (int j = 0; j < MRowCount; j++)
                {
                    #region Checking Date is null or different formate

                    #region FindCustomerId

                    if (ImportByCustGrp == false)
                    {
                        cImport.FindCustomerId(dtTenderM.Rows[j]["Customer_Name"].ToString().Trim(),
                                          dtTenderM.Rows[j]["Customer_Code"].ToString().Trim(), currConn, transaction);
                    }
                    else
                    {
                        cImport.FindCustGroupID(dtTenderM.Rows[j]["Customer_Grp_Name"].ToString().Trim(),
                                                                  dtTenderM.Rows[j]["Customer_Grp_ID"].ToString().Trim(), currConn, transaction);
                    }
                   

                   

                    #endregion FindCustomerId


                    bool IsTenderDate;
                    IsTenderDate = cImport.CheckDate(dtTenderM.Rows[j]["Tender_Date"].ToString().Trim());
                    if (IsTenderDate != true)
                    {
                        throw new ArgumentNullException("Please insert correct date format 'DD/MMM/YY' such as 31/Jan/13 in Issue_Date field.");
                    }

                    bool IsDeleveryDate;
                    IsDeleveryDate = cImport.CheckDate(dtTenderM.Rows[j]["Delevery_Date"].ToString().Trim());
                    if (IsDeleveryDate != true)
                    {
                        throw new ArgumentNullException("Please insert correct date format 'DD/MMM/YY' such as 31/Jan/13 in Issue_Date field.");
                    }

                    #endregion Checking Date is null or different formate

                    #region Checking Y/N value
                    //bool post;
                    //post = cImport.CheckYN(dtTenderM.Rows[j]["Post"].ToString().Trim());
                    //if (post != true)
                    //{
                    //    throw new ArgumentNullException("Please insert Y/N in Post field.");
                    //}
                    #endregion Checking Y/N value

                    #region Check Ref id
                    string RefId = string.Empty;
                    RefId = dtTenderM.Rows[j]["Ref_No"].ToString().Trim();
                    if (string.IsNullOrEmpty(RefId))
                    {
                        throw new ArgumentNullException("Please insert value in RefId field.");
                    }
                    #endregion Check Ref id
                }

                #endregion Master

                #region Details

                #region Row count for details table
                int DRowCount = 0;
                for (int i = 0; i < dtTenderD.Rows.Count; i++)
                {
                    if (!string.IsNullOrEmpty(dtTenderD.Rows[i]["ID"].ToString()))
                    {
                        DRowCount++;
                    }

                }

                #endregion Row count for details table

                for (int i = 0; i < DRowCount; i++)
                {
                    string ItemNo = string.Empty;
                    //string UOMn = string.Empty;

                    #region FindItemId

                    ItemNo = cImport.FindItemId(dtTenderD.Rows[i]["Product_Name"].ToString().Trim()
                                                , dtTenderD.Rows[i]["Product_Code"].ToString().Trim(), currConn, transaction);

                    #endregion FindItemId

                    #region FindUOMn

                    //UOMn = cImport.FindUOMn(ItemNo, currConn, transaction);

                    //#endregion FindUOMn

                    //#region FindUOMn

                    //cImport.FindUOMc(UOMn, dtIssueD.Rows[i]["UOM"].ToString().Trim(), currConn, transaction);

                    #endregion FindUOMn

                    #region Numeric value check
                    //bool IsQuantity = cImport.CheckNumericBool(dtTenderD.Rows[i]["Quantity"].ToString().Trim());
                    //if (IsQuantity != true)
                    //{
                    //    throw new ArgumentNullException("Please insert decimal value in Quantity field.");
                    //}

                    bool IsTenderPrice = cImport.CheckNumericBool(dtTenderD.Rows[i]["Price"].ToString().Trim());
                    if (IsTenderPrice != true)
                    {
                        throw new ArgumentNullException("Please insert decimal value in Quantity field.");
                    }

                    #endregion Numeric value check
                }

                #endregion Details


                #endregion checking from database is exist the information(NULL Check)

                if (currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                    currConn.Open();
                    transaction = currConn.BeginTransaction("Import Data.");
                }

                for (int j = 0; j < MRowCount; j++)
                {


                    #region Master Issue

                    string importID = dtTenderM.Rows[j]["ID"].ToString().Trim();
                    #region FindCustomerId
                    string custGrpID = "0";
                    string customerId = "0";

                    if (ImportByCustGrp == false)
                    {
                        string customerName = dtTenderM.Rows[j]["Customer_Name"].ToString().Trim();
                        string customerCode = dtTenderM.Rows[j]["Customer_Code"].ToString().Trim();
                        customerId = cImport.FindCustomerId(customerName, customerCode, currConn, transaction);
                        
                    }
                    else
                    {
                        string custGrpName = dtTenderM.Rows[j]["Customer_Grp_Name"].ToString().Trim();
                        string custGrpCode = dtTenderM.Rows[j]["Customer_Grp_ID"].ToString().Trim();
                        custGrpID = cImport.FindCustGroupID(custGrpName, custGrpCode, currConn, transaction);
                    }

                    
                    #endregion FindCustomerId


                    DateTime tenderDate = Convert.ToDateTime(dtTenderM.Rows[j]["Tender_Date"].ToString().Trim());
                    DateTime DeleveryDate = Convert.ToDateTime(dtTenderM.Rows[j]["Delevery_Date"].ToString().Trim());
                    #region CheckNull
                    string comments = cImport.ChecKNullValue(dtTenderM.Rows[j]["Comments"].ToString().Trim());
                    #endregion CheckNull
                    string RefId = dtTenderM.Rows[j]["Ref_No"].ToString().Trim();
                    //string post = dtTenderM.Rows[j]["Post"].ToString().Trim();
                    string createdBy = dtTenderM.Rows[j]["Created_By"].ToString().Trim();
                    //string lastModifiedBy = dtTenderM.Rows[j]["LastModified_By"].ToString().Trim();

                    tenderMasterVM = new TenderMasterVM();

                    tenderMasterVM.TenderDate = tenderDate.ToString("yyyy-MM-dd") + DateTime.Now.ToString(" HH:mm:ss");
                    tenderMasterVM.DeleveryDate = DeleveryDate.ToString("yyyy-MM-dd") + DateTime.Now.ToString(" HH:mm:ss");
                    tenderMasterVM.RefNo = RefId;
                    tenderMasterVM.CustomerId = customerId;
                    tenderMasterVM.CustomerGrpID = custGrpID;
                    tenderMasterVM.Comments = comments;
                    tenderMasterVM.CreatedBy = createdBy;
                    tenderMasterVM.CreatedOn = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    tenderMasterVM.LastModifiedBy = createdBy;
                    tenderMasterVM.LastModifiedOn = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    tenderMasterVM.ImportID = importID;
                    DataRow[] DetailRaws; //= new DataRow[];//

                    #region MAtch

                    if (!string.IsNullOrEmpty(importID))
                    {
                        DetailRaws = dtTenderD.Select("ID='" + importID + "'");
                    }
                    else
                    {
                        DetailRaws = null;
                    }

                    #endregion MAtch

                    #endregion Master Issue

                    #region Details Issue

                    int counter = 1;
                    tenderDetailVMs = new List<TenderDetailVM>();


                    foreach (DataRow row in DetailRaws)
                    {

                        string itemCode = row["Product_Code"].ToString().Trim();
                        string itemName = row["Product_Name"].ToString().Trim();

                        string itemNo = cImport.FindItemId(itemName, itemCode, currConn, transaction);

                        string quantity = row["Quantity"].ToString().Trim();
                        string price = row["Price"].ToString().Trim();
                        TenderDetailVM detail = new TenderDetailVM();
                        detail.ItemNo = itemNo;
                        detail.TenderQty = Convert.ToDecimal(quantity);
                        detail.TenderPrice = Convert.ToDecimal(price);

                        tenderDetailVMs.Add(detail);
                        counter++;
                    } // detail


                    #endregion Details Issue


                    string[] sqlResults = TenderInsert(tenderMasterVM, tenderDetailVMs, transaction, currConn);
                    retResults[0] = sqlResults[0];
                }

                if (retResults[0] == "Success")
                {
                    transaction.Commit();
                    #region SuccessResult

                    retResults[0] = "Success";
                    retResults[1] = MessageVM.tpMsgSaveSuccessfully;
                    retResults[2] = "" + "1";
                    retResults[3] = "" + "N";
                    #endregion SuccessResult
                }

            }

            #endregion  Try
            #region catch & final
            catch (SqlException sqlex)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }
                throw new ArgumentNullException("", "SQL:" + "" + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (ArgumentNullException aeg)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }
                throw new ArgumentNullException("", "SQL:" + "" + FieldDelimeter + aeg.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
            }
            catch (Exception ex)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }
                throw new ArgumentNullException("", "SQL:" + "" + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
            }
            finally
            {
                if (currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion catch & final

            return retResults;
        }

        #region web methods

        public List<TenderMasterVM> SelectAllList(string Id = "0", string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<TenderMasterVM> VMs = new List<TenderMasterVM>();
            TenderMasterVM vm;
            #endregion
            try
            {
                
                #region sql statement
             
                #region SqlExecution

                DataTable dt = SelectAll(Id, conditionFields, conditionValues, currConn, transaction, false,connVM);

                foreach (DataRow dr in dt.Rows)
                {
                    vm = new TenderMasterVM();
                    vm.TenderId = dr["TenderId"].ToString();
                    vm.RefNo = dr["RefNo"].ToString();
                    vm.CustomerId = dr["CustomerId"].ToString();
                    vm.TenderDate = OrdinaryVATDesktop.DateTimeToDate(dr["TenderDate"].ToString());
                    vm.DeleveryDate = OrdinaryVATDesktop.DateTimeToDate(dr["DeleveryDate"].ToString());
                    vm.Comments = dr["Comments"].ToString();
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedOn = dr["CreatedOn"].ToString();
                    vm.LastModifiedBy = dr["LastModifiedBy"].ToString();
                    vm.LastModifiedOn = dr["LastModifiedOn"].ToString();
                    vm.Post = dr["Post"].ToString();
                    vm.CustomerGrpID = dr["CustomerGroupID"].ToString();
                    vm.GroupName = dr["CustomerGroupName"].ToString();
                    vm.CustomerName = dr["CustomerName"].ToString();

                    VMs.Add(vm);
                }
               

                #endregion SqlExecution

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
                #endregion
            }
            #region catch
            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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


        public DataTable SelectAll(string Id = "0", string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, bool Dt = false, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            DataTable dt = new DataTable();
            #endregion
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
 th.TenderId
,th.RefNo
,th.CustomerId
,th.TenderDate
,th.DeleveryDate
,th.Comments
,th.CreatedBy
,th.CreatedOn
,th.LastModifiedBy
,th.LastModifiedOn
,th.Post
,isnull(th.BranchId,0) BranchId
,th.CustomerGroupID
,c.CustomerName
,cg.CustomerGroupName

FROM TenderHeaders th left outer join Customers c on th.CustomerId=c.CustomerId
left outer join CustomerGroups cg on c.CustomerGroupID=cg.CustomerGroupID
WHERE  1=1 
";


                if (Id != "0")
                {
                    sqlText += @" and th.TenderId=@Id";
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
                if (Id != "0")
                {
                    da.SelectCommand.Parameters.AddWithValue("@TenderId", Id.ToString());
                }
                da.Fill(dt);
                #endregion SqlExecution

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
                #endregion
            }
            #region catch
            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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


        public List<TenderDetailVM> SelectAllDetails(string tenderId = "0", string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<TenderDetailVM> VMs = new List<TenderDetailVM>();
            TenderDetailVM vm;
            #endregion
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
 td.TenderId
,td.ItemNo
,isnull(td.TenderQty,0) TenderQty
,isnull(td.SaleQty,0) SaleQty
,isnull(td.TenderPrice,0) TenderPrice
,td.CreatedBy
,td.CreatedOn
,td.LastModifiedBy
,td.LastModifiedOn
,td.Post
,td.BOMId
,p.ProductName
,p.ProductCode
,p.UOM

FROM TenderDetails td left outer join Products p on td.ItemNo=p.ItemNo
WHERE  1=1 
";


                if (tenderId != "0")
                {
                    sqlText += @" and td.TenderId=@tenderId";
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
                        objComm.Parameters.AddWithValue("@" + cField, conditionValues[j]);
                    }
                }

                if (tenderId != "0")
                {
                    objComm.Parameters.AddWithValue("@tenderId", tenderId);
                }
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new TenderDetailVM();
                    vm.TenderIdD = dr["TenderId"].ToString();
                    vm.ItemNo = dr["ItemNo"].ToString();
                    vm.TenderQty = Convert.ToDecimal(dr["TenderQty"].ToString());
                    //vm.SaleQty =  Convert.ToDecimal(dr["SaleQty"].ToString());
                    vm.TenderPrice = Convert.ToDecimal(dr["TenderPrice"].ToString());
                    vm.BOMId = dr["BOMId"].ToString();
                    vm.PCode = dr["ProductCode"].ToString();
                    vm.ItemName = dr["ProductName"].ToString();
                    vm.UOM = dr["UOM"].ToString();
                    vm.SubTotal = vm.TenderPrice * vm.TenderQty;

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
            #region catch
            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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
        #endregion

    }
}
