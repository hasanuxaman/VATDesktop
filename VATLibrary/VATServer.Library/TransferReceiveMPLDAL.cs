using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using SymphonySofttech.Utilities;
using VATServer.Ordinary;
using System.IO;
using Excel;
using VATViewModel.DTOs;
using System.Data.OracleClient;
using System.Globalization;
using System.Reflection;
using Newtonsoft.Json;
using VATServer.Interface;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using VATServer.Library.Integration;

namespace VATServer.Library
{
    public class TransferReceiveMPLDAL : ITransferReceiveMPL
    {
        #region Global Variables

        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        CommonDAL _cDAL = new CommonDAL();
        TransferReceiveDAL _TransferReceiveDAL = new TransferReceiveDAL();

        #endregion


        public string[] TransReceiveMPLInsert(TransferMPLReceiveVM MasterVM, SqlTransaction Vtransaction = null,
            SqlConnection VcurrConn = null, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";
            string Id = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            int transResult = 0;
            string sqlText = "";
            string newIDCreate = "";
            string PostStatus = "";
            int IDExist = 0;
            TransferReceiveVM TransferReceiveMaster = new TransferReceiveVM();
            List<TransferReceiveDetailVM> Details = new List<TransferReceiveDetailVM>();
            #endregion Initializ

            #region Try

            try
            {

                #region Validation for Header


                if (MasterVM == null)
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgNoDataToSave);
                }
                else if (Convert.ToDateTime(MasterVM.TransferDateTime) < DateTime.MinValue ||
                         Convert.ToDateTime(MasterVM.TransferDateTime) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert,
                        "Please Check Invoice Data and Time");

                }

                #endregion Validation for Header

                CommonDAL commonDal = new CommonDAL();

                DataTable settings = new DataTable();
                if (settingVM.SettingsDTUser != null && settingVM.SettingsDTUser.Rows.Count > 0)
                {
                    settings = settingVM.SettingsDTUser;
                }
                else
                {
                    settings = new CommonDAL().SettingDataAll(null, null);
                }

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
                    transaction = currConn.BeginTransaction(MessageVM.saleMsgMethodNameInsert);
                }


                #endregion open connection and transaction


                #region Find Transaction Exist

                int transferIssueId = MasterVM.Id;
                sqlText = "";
                sqlText = sqlText +
                          "select COUNT(TransferReceiveNo) from TransferMPLReceives WHERE TransferReceiveNo=@MasterTransferReceiveNo ";
                SqlCommand cmdExistTran = new SqlCommand(sqlText, currConn, transaction);
                cmdExistTran.Parameters.AddWithValueAndNullHandle("@MasterTransferReceiveNo",
                    MasterVM.TransferReceiveNo);
                IDExist = (int)cmdExistTran.ExecuteScalar();

                if (IDExist > 0)
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgFindExistID);
                }

                #endregion Find Transaction Exist

                #region Sale ID Create

                if (string.IsNullOrEmpty(MasterVM.TransactionType)) // start
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert,
                        MessageVM.msgTransactionNotDeclared);
                }

                #region Other

                if (MasterVM.TransactionType == "62In")
                {
                    newIDCreate = commonDal.TransactionCode("Transfer", "62In", "TransferMPLReceives",
                        "TransferReceiveNo",
                        "ReceiveDateTime", MasterVM.ReceiveDateTime, MasterVM.BranchId.ToString(), currConn,
                        transaction);

                }

                #endregion


                if (string.IsNullOrEmpty(newIDCreate) || newIDCreate == "")
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert,
                        "ID Prefetch not set please update Prefetch first");
                }


                #region checkId

                sqlText = @"
SELECT COUNT(TransferReceiveNo) FROM TransferMPLReceives 
where TransferReceiveNo = @MasterTransferReceiveNo and TransactionType = @TransactionType and BranchId = @BranchId";

                var sqlCmd = new SqlCommand(sqlText, currConn, transaction);
                sqlCmd.Parameters.AddWithValue("@MasterTransferReceiveNo", newIDCreate);
                sqlCmd.Parameters.AddWithValue("@TransactionType", MasterVM.TransactionType);
                sqlCmd.Parameters.AddWithValue("@BranchId", MasterVM.BranchId);

                var count = (int)sqlCmd.ExecuteScalar();

                if (count > 0)
                {
                    FileLogger.Log("TransferReceiveMPLDAL", "Insert",
                        "TransferReceiveNo " + newIDCreate + " Already Exists");
                    throw new Exception("Trans Id " + newIDCreate + " Already Exists");
                }

                #endregion

                #region Check Existing Id

                sqlText =
                    "select count(TransferReceiveNo) from TransferMPLReceives where TransferReceiveNo = @MasterTransferReceiveNo";

                var cmd = new SqlCommand(sqlText, currConn, transaction);
                cmd.Parameters.AddWithValue("@MasterTransferReceiveNo", newIDCreate);

                var count1 = (int)cmd.ExecuteScalar();

                if (count1 > 0)
                {
                    FileLogger.Log("TransferReceiveMPLDAL", "Insert", "Trans Id " + newIDCreate + " Already Exists");
                    throw new Exception("Trans Id " + newIDCreate + " Already Exists");
                }

                #endregion

                MasterVM.TransferReceiveNo = newIDCreate;

                #endregion Purchase ID Create Not Complete

                #region ID generated completed,Insert new Information in Header

                MasterVM.TotalAmount = Convert.ToDecimal(commonDal.decimal259(MasterVM.TotalAmount));
                MasterVM.TotalSDAmount = Convert.ToDecimal(commonDal.decimal259(MasterVM.TotalSDAmount));
                MasterVM.TotalVATAmount = Convert.ToDecimal(commonDal.decimal259(MasterVM.TotalVATAmount));

                MasterVM.TransferDateTime = OrdinaryVATDesktop.DateToDate(MasterVM.TransferDateTime);
                MasterVM.ReceiveDateTime = OrdinaryVATDesktop.DateToDate(MasterVM.ReceiveDateTime);
                MasterVM.RailwayReceiptDate = OrdinaryVATDesktop.DateToDate(MasterVM.RailwayReceiptDate);
                MasterVM.ArrivalDate = OrdinaryVATDesktop.DateToDate(MasterVM.ArrivalDate);
                MasterVM.Post = "N";
                MasterVM.IsTransfer = "N";
                retResults = TransReceiveMPLInsertToMaster(MasterVM, currConn, transaction, null, settings);

                Id = retResults[4];

                if (retResults[0] != "Success")
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, retResults[1]);
                }

                #endregion ID generated completed,Insert new Information in Header


                #region Insert into Details(Insert complete in Header)

                if (MasterVM.TransferMPLReceiveDetailVMs != null)
                {
                    if (MasterVM.TransferMPLReceiveDetailVMs.Count() < 0)
                    {
                        throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert,
                            MessageVM.saleMsgNoDataToSave);
                    }
                    int ReceiveLineNo = 1;
                    foreach (TransferMPLReceiveDetailVM collection in MasterVM.TransferMPLReceiveDetailVMs)
                    {
                        collection.TransferMPLReceiveId = Convert.ToInt32(Id);
                        collection.BranchId = MasterVM.BranchId;
                        collection.ReceiveLineNo = ReceiveLineNo.ToString();

                        collection.TransferFrom = MasterVM.TransferFrom.ToString();
                        collection.TransferDateTime = MasterVM.TransferDateTime;
                        collection.ReceiveDateTime = MasterVM.ReceiveDateTime;
                        collection.Post = "N";

                        collection.UOMQty = collection.Quantity * collection.UOMc;
                        collection.UOMPrice = (collection.CostPrice * collection.UOMQty);
                        collection.SubTotal = collection.UOMPrice;
                        collection.UOMn = "LTR";
                        collection.AlReadyReceivedQuantity = collection.AlReadyReceivedQuantity ;
                        if (collection.UOMQty == 0)
                        {
                            continue;
                        }

                        #region TransferReceiveDeatils

                        TransferReceiveDetailVM transferDetails = new TransferReceiveDetailVM();
                        transferDetails.TransferReceiveNo = newIDCreate;
                        transferDetails.ItemNo = collection.ItemNo;
                        transferDetails.BranchId = MasterVM.BranchId;
                        transferDetails.TransferFrom = MasterVM.TransferFrom;
                        transferDetails.TransactionDateTime = MasterVM.ReceiveDateTime;
                        transferDetails.Post = "N";

                        transferDetails.UOMQty = collection.Quantity * collection.UOMc;
                        transferDetails.UOMPrice = (collection.CostPrice * collection.UOMQty);
                        transferDetails.SubTotal = transferDetails.UOMPrice;
                        transferDetails.UOMn = "LTR";
                        transferDetails.ReceiveLineNo = ReceiveLineNo.ToString();

                        Details.Add(transferDetails);


                        #endregion

                        retResults = TransReceiveMPLInsertToDetails(collection, currConn, transaction, null, settings);

                        #region Update Transfer Issue Data

                        sqlText = "";
                        sqlText +=
                            @" UPDATE TransferMPLIssueDetails SET TransferReceiveNo=@TransferReceiveNo, IsReceiveCompleted=@IsReceiveCompleted WHERE TransferMPLIssueId=@Id  and Id=@TransferIssueDetailsRefId and ItemNo=@ItemNo";

                        SqlCommand cmdUpdateDetail = new SqlCommand(sqlText, currConn, transaction);
                        cmdUpdateDetail.Parameters.AddWithValueAndNullHandle("@Id", transferIssueId);
                        cmdUpdateDetail.Parameters.AddWithValueAndNullHandle("@ItemNo", collection.ItemNo);
                        cmdUpdateDetail.Parameters.AddWithValueAndNullHandle("@TransferReceiveNo", newIDCreate);
                        cmdUpdateDetail.Parameters.AddWithValueAndNullHandle("@IsReceiveCompleted", collection.IsReceiveCompleted);
                        cmdUpdateDetail.Parameters.AddWithValueAndNullHandle("@TransferIssueDetailsRefId", collection.TransferIssueDetailsRefId);

                        transResult = cmdUpdateDetail.ExecuteNonQuery();

                        #endregion

                    }
                }

                if (retResults[0] != "Success")
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, retResults[1]);
                }

                #region TransferReceiveInsert
                TransferReceiveMaster.TransferReceiveNo = MasterVM.TransferReceiveNo;
                TransferReceiveMaster.TransferFromNo = MasterVM.TransferIssueNo;

                TransferReceiveMaster.TransactionDateTime = MasterVM.TransferDateTime;
                TransferReceiveMaster.TransferFrom = MasterVM.TransferFrom;
                TransferReceiveMaster.ReferenceNo = MasterVM.ReferenceNo;
                TransferReceiveMaster.BranchId = MasterVM.BranchId;
                TransferReceiveMaster.TransactionType = MasterVM.TransactionType;
                TransferReceiveMaster.Post = "N";
                TransferReceiveMaster.CreatedBy = MasterVM.CreatedBy;
                TransferReceiveMaster.CreatedOn = MasterVM.CreatedOn;
                TransferReceiveMaster.LastModifiedBy = MasterVM.CreatedOn;
                TransferReceiveMaster.LastModifiedOn = MasterVM.CreatedOn;
                retResults = _TransferReceiveDAL.Insert(TransferReceiveMaster, Details,transaction, currConn, connVM,false);
                #endregion

                if (retResults[0] != "Success")
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, retResults[1]);
                }


                #endregion Insert into Details(Insert complete in Header)


                //#region Update Transfer Issue Data

                //sqlText = "";
                //sqlText +=
                //    @" UPDATE TransferMPLIssues SET TransferReceiveNo = @TransferReceiveNo WHERE Id=@Id ";

                //SqlCommand cmdUpdateHeaders = new SqlCommand(sqlText, currConn, transaction);
                //cmdUpdateHeaders.Parameters.AddWithValueAndNullHandle("@Id", transferIssueId);
                //cmdUpdateHeaders.Parameters.AddWithValueAndNullHandle("@TransferReceiveNo", newIDCreate);

                //transResult = cmdUpdateHeaders.ExecuteNonQuery();

                //#endregion

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion

                #region SuccessResult

                retResults = new string[5];
                retResults[0] = "Success";
                retResults[1] = MessageVM.saleMsgSaveSuccessfully;
                retResults[2] = "" + MasterVM.TransferReceiveNo;
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
                if (currConn != null)
                {
                    transaction.Rollback();
                }

                FileLogger.Log("TransferReceiveMPLDAL", "SalesInsert", ex.ToString() + "\n" + sqlText);

            }
            finally
            {

                #region Comment 28 Oct 2020


                #endregion

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

        public string[] TransReceiveMPLInsertToMaster(TransferMPLReceiveVM Master, SqlConnection VcurrConn = null,
            SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null, DataTable settingsDt = null)
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
                sqlText += " INSERT INTO TransferMPLReceives";
                sqlText += " (";
                sqlText += "  TransferIssueMasterRefId";
                sqlText += " ,TransferReceiveNo";
                sqlText += " ,BranchId";
                sqlText += " ,TransferFrom";
                sqlText += " ,TransferTRCode";
                sqlText += " ,ReceiveTRCode";
                sqlText += " ,TransferDateTime";
                sqlText += " ,ReceiveDateTime";
                sqlText += " ,TotalAmount";
                sqlText += " ,TransactionType";
                sqlText += " ,ReportType";
                sqlText += " ,Comments";
                sqlText += " ,VehicleType";
                sqlText += " ,VehicleNo";
                sqlText += " ,Post";
                sqlText += " ,SerialNo";
                sqlText += " ,ReferenceNo";
                sqlText += " ,TotalVATAmount";
                sqlText += " ,TotalSDAmount";
                sqlText += " ,BranchFromRef";
                sqlText += " ,BranchToRef";
                sqlText += " ,IsTransfer";
                sqlText += " ,CreatedBy";
                sqlText += " ,CreatedOn";
                sqlText += " ,SignatoryDesig";
                sqlText += " ,RailwayReceiptNo";
                sqlText += " ,RailwayReceiptDate";
                sqlText += " ,RailwayInvoiceNo";
                sqlText += " ,WeightChargeed";
                sqlText += " ,FreightToPay";
                sqlText += " ,FreightPrepaid";
                sqlText += " ,ArrivalDate";
                sqlText += " ,TransferType";
                sqlText += " )";

                sqlText += " VALUES";
                sqlText += " (";
                sqlText += "   @TransferIssueMasterRefId";
                sqlText += "  ,@TransferReceiveNo";
                sqlText += "  ,@BranchId";
                sqlText += "  ,@TransferFrom";
                sqlText += "  ,@TransferTRCode";
                sqlText += "  ,@ReceiveTRCode";
                sqlText += "  ,@TransferDateTime";
                sqlText += "  ,@ReceiveDateTime";
                sqlText += "  ,@TotalAmount";
                sqlText += "  ,@TransactionType";
                sqlText += "  ,@ReportType";
                sqlText += "  ,@Comments";
                sqlText += "  ,@VehicleType";
                sqlText += "  ,@VehicleNo";
                sqlText += "  ,@Post";
                sqlText += "  ,@SerialNo";
                sqlText += "  ,@ReferenceNo";
                sqlText += "  ,@TotalVATAmount";
                sqlText += "  ,@TotalSDAmount";
                sqlText += "  ,@BranchFromRef";
                sqlText += "  ,@BranchToRef";
                sqlText += "  ,@IsTransfer";
                sqlText += "  ,@CreatedBy";
                sqlText += "  ,@CreatedOn";
                sqlText += "  ,@SignatoryDesig";
                sqlText += "  ,@RailwayReceiptNo";
                sqlText += "  ,@RailwayReceiptDate";
                sqlText += "  ,@RailwayInvoiceNo";
                sqlText += "  ,@WeightChargeed";
                sqlText += "  ,@FreightToPay";
                sqlText += "  ,@FreightPrepaid";
                sqlText += "  ,@ArrivalDate";
                sqlText += "  ,@TransferType";

                sqlText += ")  SELECT SCOPE_IDENTITY() ";

                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);

                cmdInsert.Parameters.AddWithValueAndNullHandle("@TransferIssueMasterRefId", Master.TransferIssueMasterRefId);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@TransferReceiveNo", Master.TransferReceiveNo);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@BranchId", Master.BranchId);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@TransferFrom", Master.TransferFrom);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@TransferTRCode", Master.TransferTRCode);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@ReceiveTRCode", Master.ReceiveTRCode);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@TransferDateTime", Master.TransferDateTime);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@ReceiveDateTime", Master.ReceiveDateTime);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@TotalAmount", Master.TotalAmount);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@TransactionType", Master.TransactionType);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@ReportType", Master.ReportType);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Comments", Master.Comments);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@VehicleType", Master.VehicleType);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@VehicleNo", Master.VehicleNo);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Post", Master.Post);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@SerialNo", Master.SerialNo);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@ReferenceNo", Master.ReferenceNo);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@TotalVATAmount", Master.TotalVATAmount);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@TotalSDAmount", Master.TotalSDAmount);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@BranchFromRef", Master.BranchFromRef);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@BranchToRef", Master.BranchToRef);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@IsTransfer", Master.IsTransfer);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@CreatedBy", Master.CreatedBy);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@CreatedOn", Master.CreatedOn);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@SignatoryDesig", Master.SignatoryDesig);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@RailwayReceiptNo", Master.RailwayReceiptNo);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@RailwayReceiptDate", Master.RailwayReceiptDate);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@RailwayInvoiceNo", Master.RailwayInvoiceNo);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@WeightChargeed", Master.WeightChargeed);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@FreightToPay", Master.FreightToPay);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@FreightPrepaid", Master.FreightPrepaid);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@ArrivalDate", Master.ArrivalDate);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@TransferType", Master.TransferType);


                transResult = Convert.ToInt32(cmdInsert.ExecuteScalar());
                retResults[4] = transResult.ToString();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert,
                        MessageVM.saleMsgSaveNotSuccessfully);
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
                retResults[1] = MessageVM.saleMsgSaveSuccessfully;
                retResults[2] = "" + Master.TransferReceiveNo;
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

                FileLogger.Log("TransferReceiveMPLDAL", "SalesMPLInsertToMaster", ex.ToString() + "\n" + sqlText);
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

        public string[] TransReceiveMPLInsertToDetails(TransferMPLReceiveDetailVM details,
            SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null,
            DataTable settingsDt = null)
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
                sqlText += " INSERT INTO TransferMPLReceiveDetails";
                sqlText += " (";
                sqlText += "TransferMPLReceiveId ";
                sqlText += ",BranchId ";
                sqlText += ",ReceiveLineNo ";
                sqlText += ",TransferFrom ";
                sqlText += ",TransferDateTime ";
                sqlText += ",ReceiveDateTime ";
                sqlText += ",Post ";
                sqlText += ",ItemNo ";
                sqlText += ",RequestedQuantity ";
                sqlText += ",RequestedVolumn ";
                sqlText += ",AlReadyReceivedQuantity ";
                sqlText += ",IsReceiveCompleted ";
                sqlText += ",Quantity ";
                sqlText += ",CostPrice ";
                sqlText += ",UOM ";
                sqlText += ",SubTotal ";
                sqlText += ",LCF ";
                sqlText += ",QU ";
                sqlText += ",IsExcise ";
                sqlText += ",IsCustoms ";
                sqlText += ",Comments ";
                sqlText += ",TransactionType ";
                sqlText += ",ReportType ";
                sqlText += ",UOMQty ";
                sqlText += ",UOMPrice ";
                sqlText += ",UOMc ";
                sqlText += ",UOMn ";
                sqlText += ",VATRate ";
                sqlText += ",VATAmount ";
                sqlText += ",SDRate ";
                sqlText += ",SDAmount ";
                sqlText += ",PeriodID ";
                sqlText += ",TankId ";
                sqlText += ",Temperature ";
                sqlText += ",SP_Gravity ";
                sqlText += ",QtyAt30Temperature ";
                sqlText += ",TransferIssueMasterRefId ";
                sqlText += ",TransferIssueDetailsRefId ";
                sqlText += ",WagonNo ";

                sqlText += " )";

                sqlText += " VALUES";
                sqlText += " (";
                sqlText += " @TransferMPLReceiveId";
                sqlText += " ,@BranchId";
                sqlText += " ,@ReceiveLineNo";
                sqlText += " ,@TransferFrom";
                sqlText += " ,@TransferDateTime";
                sqlText += " ,@ReceiveDateTime";
                sqlText += " ,@Post";
                sqlText += " ,@ItemNo";
                sqlText += " ,@RequestedQuantity";
                sqlText += " ,@RequestedVolumn";
                sqlText += " ,@AlReadyReceivedQuantity";
                sqlText += " ,@IsReceiveCompleted";
                sqlText += " ,@Quantity";
                sqlText += " ,@CostPrice";
                sqlText += " ,@UOM";
                sqlText += " ,@SubTotal";
                sqlText += " ,@LCF";
                sqlText += " ,@QU";
                sqlText += " ,@IsExcise";
                sqlText += " ,@IsCustoms";
                sqlText += " ,@Comments";
                sqlText += " ,@TransactionType";
                sqlText += " ,@ReportType";
                sqlText += " ,@UOMQty";
                sqlText += " ,@UOMPrice";
                sqlText += " ,@UOMc";
                sqlText += " ,@UOMn";
                sqlText += " ,@VATRate";
                sqlText += " ,@VATAmount";
                sqlText += " ,@SDRate";
                sqlText += " ,@SDAmount";
                sqlText += " ,@PeriodID";
                sqlText += " ,@TankId";
                sqlText += " ,@Temperature";
                sqlText += " ,@SP_Gravity";
                sqlText += " ,@QtyAt30Temperature";
                sqlText += " ,@TransferIssueMasterRefId";
                sqlText += " ,@TransferIssueDetailsRefId";
                sqlText += " ,@WagonNo";


                sqlText += ")  SELECT SCOPE_IDENTITY() ";

                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);

                cmdInsert.Parameters.AddWithValueAndNullHandle("@TransferMPLReceiveId", details.TransferMPLReceiveId);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@BranchId", details.BranchId);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@ReceiveLineNo", details.ReceiveLineNo);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@TransferFrom", details.TransferFrom);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@TransferDateTime", details.TransferDateTime);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@ReceiveDateTime", details.ReceiveDateTime);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Post", details.Post);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@ItemNo", details.ItemNo);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@RequestedQuantity", details.RequestedQuantity);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@RequestedVolumn", details.RequestedVolumn);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@AlReadyReceivedQuantity", details.AlReadyReceivedQuantity);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@IsReceiveCompleted", details.IsReceiveCompleted);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Quantity", details.Quantity);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@CostPrice", details.CostPrice);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@UOM", details.UOM);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@SubTotal", details.SubTotal);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@LCF", details.LCF);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@QU", details.QU);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@IsExcise", details.IsExcise);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@IsCustoms", details.IsCustoms);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Comments", details.Comments);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@TransactionType", details.TransactionType);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@ReportType", details.ReportType);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@UOMQty", details.UOMQty);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@UOMPrice", details.UOMPrice);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@UOMc", details.UOMc);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@UOMn", details.UOMn);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@VATRate", details.VATRate);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@VATAmount", details.VATAmount);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@SDRate", details.SDRate);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@SDAmount", details.SDAmount);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@PeriodID", details.PeriodID);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@TankId", details.TankId);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Temperature", details.Temperature);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@SP_Gravity", details.SP_Gravity);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@QtyAt30Temperature", details.QtyAt30Temperature);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@TransferIssueMasterRefId", details.TransferIssueMasterRefId);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@TransferIssueDetailsRefId", details.TransferIssueDetailsRefId);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@WagonNo", details.WagonNo);


                transResult = Convert.ToInt32(cmdInsert.ExecuteScalar());
                retResults[4] = transResult.ToString();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert,
                        MessageVM.saleMsgSaveNotSuccessfully);
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
                retResults[1] = MessageVM.saleMsgSaveSuccessfully;
                retResults[2] = "" + details.TransferMPLReceiveId;
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

                FileLogger.Log("TransferReceiveMPLDAL", "TransReceiveMPLInsertToDetails",
                    ex.ToString() + "\n" + sqlText);
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


        public string[] TransReceiveMPLUpdate(TransferMPLReceiveVM MasterVM, SqlTransaction transaction,
            SqlConnection currConn, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";
            string Id = "";


            int transResult = 0;
            string sqlText = "";
            string PostStatus = "";
            TransferReceiveVM TransferReceiveMaster = new TransferReceiveVM();
            List<TransferReceiveDetailVM> Details = new List<TransferReceiveDetailVM>();
            #endregion Initializ

            #region Try

            try
            {

                #region Validation for Header

                if (MasterVM == null)
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgNoDataToSave);
                }
                else if (Convert.ToDateTime(MasterVM.TransferDateTime) < DateTime.MinValue ||
                         Convert.ToDateTime(MasterVM.TransferDateTime) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert,
                        "Please Check Invoice Data and Time");

                }

                #endregion Validation for Header

                CommonDAL commonDal = new CommonDAL();

                DataTable settings = new DataTable();
                if (settingVM.SettingsDTUser != null && settingVM.SettingsDTUser.Rows.Count > 0)
                {
                    settings = settingVM.SettingsDTUser;
                }
                else
                {
                    settings = new CommonDAL().SettingDataAll(null, null);
                }

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
                    transaction = currConn.BeginTransaction(MessageVM.saleMsgMethodNameInsert);
                }


                #endregion open connection and transaction


                #region Update Information in Header

                MasterVM.TotalAmount = Convert.ToDecimal(commonDal.decimal259(MasterVM.TotalAmount));
                MasterVM.TotalSDAmount = Convert.ToDecimal(commonDal.decimal259(MasterVM.TotalSDAmount));
                MasterVM.TotalVATAmount = Convert.ToDecimal(commonDal.decimal259(MasterVM.TotalVATAmount));
                MasterVM.BranchFromRef = MasterVM.TransferFrom.ToString();
                MasterVM.BranchToRef = MasterVM.BranchId.ToString();
                MasterVM.TransferDateTime = OrdinaryVATDesktop.DateToDate(MasterVM.TransferDateTime);
                MasterVM.ReceiveDateTime = OrdinaryVATDesktop.DateToDate(MasterVM.ReceiveDateTime);
                MasterVM.RailwayReceiptDate = OrdinaryVATDesktop.DateToDate(MasterVM.RailwayReceiptDate);
                MasterVM.ArrivalDate = OrdinaryVATDesktop.DateToDate(MasterVM.ArrivalDate);

                retResults = TransReceiveMPLUpdateToMaster(MasterVM, currConn, transaction, null, settings);

                Id = retResults[2];

                if (retResults[0] != "Success")
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, retResults[1]);
                }

                #endregion Update Information in Header

                #region Delete Existing Detail Data

                sqlText = "";
                sqlText +=
                    @" DELETE FROM TransferMPLReceiveDetails WHERE TransferMPLReceiveId=@MasterTransferMPLReceiveId ";


                SqlCommand cmdDeleteDetail = new SqlCommand(sqlText, currConn, transaction);
                cmdDeleteDetail.Parameters.AddWithValueAndNullHandle("@MasterTransferMPLReceiveId", MasterVM.Id);

                transResult = cmdDeleteDetail.ExecuteNonQuery();

                #endregion

                #region Insert into Details(Insert complete in Header)

                if (MasterVM.TransferMPLReceiveDetailVMs != null)
                {
                    if (MasterVM.TransferMPLReceiveDetailVMs.Count() < 0)
                    {
                        throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert,
                            MessageVM.saleMsgNoDataToSave);
                    }
                    int ReceiveLineNo = 1;
                    foreach (TransferMPLReceiveDetailVM collection in MasterVM.TransferMPLReceiveDetailVMs)
                    {
                        collection.TransferMPLReceiveId = MasterVM.Id;
                        collection.BranchId = MasterVM.BranchId;
                        collection.ReceiveLineNo = ReceiveLineNo.ToString();
                        collection.TransferFrom = MasterVM.TransferFrom.ToString();
                        collection.TransferDateTime = MasterVM.TransferDateTime;
                        collection.ReceiveDateTime = MasterVM.ReceiveDateTime;
                        collection.Post = "N";
                        collection.AlReadyReceivedQuantity = collection.AlReadyReceivedQuantity;

                        collection.UOMQty = collection.Quantity * collection.UOMc;
                        collection.UOMPrice = (collection.CostPrice * collection.UOMQty);
                        collection.UOMn = "LTR";

                        if (collection.UOMQty == 0)
                        {
                            continue;
                        }
                        #region TransferReceiveDeatils

                        TransferReceiveDetailVM transferDetails = new TransferReceiveDetailVM();
                        transferDetails.TransferReceiveNo = MasterVM.TransferReceiveNo;
                        transferDetails.ItemNo = collection.ItemNo;
                        transferDetails.BranchId = MasterVM.BranchId;
                        transferDetails.TransferFrom = MasterVM.TransferFrom;
                        transferDetails.TransactionDateTime = MasterVM.ReceiveDateTime;
                        transferDetails.Post = "N";

                        transferDetails.UOMQty = collection.Quantity * collection.UOMc;
                        transferDetails.UOMPrice = (collection.CostPrice * collection.UOMQty);
                        transferDetails.SubTotal = transferDetails.UOMPrice;
                        transferDetails.UOMn = "LTR";
                        transferDetails.ReceiveLineNo = ReceiveLineNo.ToString();

                        Details.Add(transferDetails);


                        #endregion


                        retResults = TransReceiveMPLInsertToDetails(collection, currConn, transaction, null, settings);
                        #region Update Transfer Issue Data

                        sqlText = "";
                        sqlText +=
                            @" UPDATE TransferMPLIssueDetails SET TransferReceiveNo=@TransferReceiveNo, IsReceiveCompleted=@IsReceiveCompleted WHERE TransferMPLIssueId=@Id  and Id=@TransferIssueDetailsRefId and ItemNo=@ItemNo";

                        SqlCommand cmdUpdateDetail = new SqlCommand(sqlText, currConn, transaction);
                        cmdUpdateDetail.Parameters.AddWithValueAndNullHandle("@Id", MasterVM.TransferIssueMasterRefId);
                        cmdUpdateDetail.Parameters.AddWithValueAndNullHandle("@ItemNo", collection.ItemNo);
                        cmdUpdateDetail.Parameters.AddWithValueAndNullHandle("@TransferReceiveNo", MasterVM.TransferIssueNo);
                        cmdUpdateDetail.Parameters.AddWithValueAndNullHandle("@IsReceiveCompleted", collection.IsReceiveCompleted);
                        cmdUpdateDetail.Parameters.AddWithValueAndNullHandle("@TransferIssueDetailsRefId", collection.TransferIssueDetailsRefId);

                        transResult = cmdUpdateDetail.ExecuteNonQuery();

                        #endregion


                    }
                }

                if (retResults[0] != "Success")
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, retResults[1]);
                }
                #region TransferReceiveInsert
                TransferReceiveMaster.TransferReceiveNo = MasterVM.TransferReceiveNo;
                TransferReceiveMaster.TransferFromNo = MasterVM.TransferIssueNo;

                TransferReceiveMaster.TransactionDateTime = MasterVM.TransferDateTime;
                TransferReceiveMaster.TransferFrom = MasterVM.TransferFrom;
                TransferReceiveMaster.ReferenceNo = MasterVM.ReferenceNo;
                TransferReceiveMaster.BranchId = MasterVM.BranchId;
                TransferReceiveMaster.TransactionType = MasterVM.TransactionType;
                TransferReceiveMaster.Post = "N";
                TransferReceiveMaster.CreatedBy = MasterVM.LastModifiedBy;
                TransferReceiveMaster.CreatedOn = MasterVM.LastModifiedOn;
                TransferReceiveMaster.LastModifiedBy = MasterVM.LastModifiedBy;
                TransferReceiveMaster.LastModifiedOn = MasterVM.LastModifiedOn;
                retResults = _TransferReceiveDAL.Update(TransferReceiveMaster, Details,  connVM,"",transaction, currConn);
                #endregion

                if (retResults[0] != "Success")
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, retResults[1]);
                }
                #endregion Insert into Details(Insert complete in Header)

                #region Commit

                if (transaction != null)
                {
                    transaction.Commit();
                }

                #endregion

                #region SuccessResult

                retResults = new string[5];
                retResults[0] = "Success";
                retResults[1] = MessageVM.saleMsgUpdateSuccessfully;
                retResults[2] = "" + MasterVM.TransferReceiveNo;
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
                if (currConn != null)
                {
                    transaction.Rollback();
                }

                FileLogger.Log("TransferReceiveMPLDAL", "TransReceiveMPLUpdate", ex.ToString() + "\n" + sqlText);

            }
            finally
            {
                if (currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }

            #endregion Catch and Finall

            #region Result

            return retResults;

            #endregion Result
        }

        public string[] TransReceiveMPLUpdateToMaster(TransferMPLReceiveVM Master, SqlConnection VcurrConn = null,
            SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null, DataTable settingsDt = null)
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
                sqlText += " update TransferMPLReceives SET  ";

                sqlText += " TransferReceiveNo=@TransferReceiveNo";
                sqlText += " ,BranchId=@BranchId";
                sqlText += " ,TransferFrom=@TransferFrom";
                sqlText += " ,TransferTRCode=@TransferTRCode";
                sqlText += " ,ReceiveTRCode=@ReceiveTRCode";
                sqlText += " ,TransferDateTime=@TransferDateTime";
                sqlText += " ,ReceiveDateTime=@ReceiveDateTime";
                sqlText += " ,TotalAmount=@TotalAmount";
                sqlText += " ,TransactionType=@TransactionType";
                sqlText += " ,Comments=@Comments";
                sqlText += " ,VehicleType=@VehicleType";
                sqlText += " ,VehicleNo=@VehicleNo";
                sqlText += " ,SerialNo=@SerialNo";
                sqlText += " ,ReferenceNo=@ReferenceNo";
                sqlText += " ,TotalVATAmount=@TotalVATAmount";
                sqlText += " ,TotalSDAmount=@TotalSDAmount";
                sqlText += " ,BranchToRef=@BranchToRef";
                sqlText += " ,LastModifiedBy=@LastModifiedBy";
                sqlText += " ,SignatoryDesig=@SignatoryDesig";
                sqlText += " ,LastModifiedOn=@LastModifiedOn";
                sqlText += " ,RailwayReceiptNo=@RailwayReceiptNo";
                sqlText += " ,RailwayReceiptDate=@RailwayReceiptDate";
                sqlText += " ,RailwayInvoiceNo=@RailwayInvoiceNo";
                sqlText += " ,WeightChargeed=@WeightChargeed";
                sqlText += " ,FreightToPay=@FreightToPay";
                sqlText += " ,FreightPrepaid=@FreightPrepaid";
                sqlText += " ,DIP=@DIP";
                sqlText += " ,ArrivalDate=@ArrivalDate";


                sqlText += " WHERE Id=@Id";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);

                cmdUpdate.Parameters.AddWithValueAndNullHandle("@TransferReceiveNo", Master.TransferReceiveNo);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@BranchId", Master.BranchId);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@TransferFrom", Master.TransferFrom);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@TransferTRCode", Master.TransferTRCode);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@ReceiveTRCode", Master.ReceiveTRCode);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@TransferDateTime", Master.TransferDateTime);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@ReceiveDateTime", Master.ReceiveDateTime);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@TotalAmount", Master.TotalAmount);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@TransactionType", Master.TransactionType);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@Comments", Master.Comments);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@VehicleType", Master.VehicleType);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@VehicleNo", Master.VehicleNo);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@SerialNo", Master.SerialNo);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@ReferenceNo", Master.ReferenceNo);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@TotalVATAmount", Master.TotalVATAmount);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@TotalSDAmount", Master.TotalSDAmount);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@BranchToRef", Master.BranchToRef);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@LastModifiedBy", Master.LastModifiedBy);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@SignatoryDesig", Master.SignatoryDesig);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@LastModifiedOn", Master.LastModifiedOn);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@RailwayReceiptNo", Master.RailwayReceiptNo);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@RailwayReceiptDate", Master.RailwayReceiptDate);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@RailwayInvoiceNo", Master.RailwayInvoiceNo);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@WeightChargeed", Master.WeightChargeed);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@FreightToPay", Master.FreightToPay);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@FreightPrepaid", Master.FreightPrepaid);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@DIP", Master.DIP);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@ArrivalDate", Master.ArrivalDate);


                cmdUpdate.Parameters.AddWithValueAndNullHandle("@Id", Master.Id);

                transResult = cmdUpdate.ExecuteNonQuery();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.PurchasemsgUpdateNotSuccessfully,
                        MessageVM.PurchasemsgUpdateNotSuccessfully);
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
                retResults[1] = MessageVM.PurchasemsgUpdateSuccessfully;
                retResults[2] = "" + Master.Id;
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

                FileLogger.Log("TransferReceiveMPLDAL", "TransReceiveMPLUpdateToMaster",
                    ex.ToString() + "\n" + sqlText);
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

        public DataTable SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null, string[] ids = null, string transactionType = null, string Orderby = "Y", string SelectTop = "100")
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
            string count = SelectTop;

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
                }

                sqlText += @"

TR.Id,
TR.TransferReceiveNo,
ISNULL(TI.TransferIssueNo,'') TransferIssueNo,
ISNULL(TR.TransferIssueMasterRefId,0) TransferIssueMasterRefId,
TR.BranchId,
ISNULL(TR.TransferFrom, '0') TransferFrom,
ISNULL(B.BranchName, '') TransferFromBranch,
ISNULL(BP.BranchName, '') TransferToBranch,
TR.TransferTRCode,
TR.ReceiveTRCode,
ISNULL(TR.TransferDateTime, '') TransferDateTime,
ISNULL(TR.ReceiveDateTime, '') ReceiveDateTime,
ISNULL(TR.RailwayReceiptDate, '') RailwayReceiptDate,
TR.TotalAmount,
TR.TransactionType,
TR.ReportType,
TR.Comments,
TR.VehicleType,
TR.VehicleNo,
ISNULL(TR.Post, 'N') Post,
TR.SerialNo,
TR.ReferenceNo,
TR.TotalVATAmount,
TR.TotalSDAmount,
TR.BranchFromRef,
TR.BranchToRef,
TR.IsTransfer,
TR.CreatedBy,
TR.CreatedOn,
TR.LastModifiedBy,
TR.SignatoryDesig,
TR.LastModifiedOn,
TR.RailwayReceiptNo,
TR.RailwayInvoiceNo,

ISNULL(TR.WeightChargeed, '0') WeightChargeed,
ISNULL(TR.FreightToPay, '0') FreightToPay,
ISNULL(TR.FreightPrepaid, '0') FreightPrepaid
,ISNULL(TR.DIP, '') DIP
,ISNULL(TR.ArrivalDate, '') ArrivalDate
,TR.TransferType

FROM TransferMPLReceives TR
LEFT OUTER JOIN TransferMPLIssues TI ON TI.Id = TR.TransferIssueMasterRefId
LEFT OUTER JOIN BranchProfiles B ON B.BranchID = TR.TransferFrom
LEFT OUTER JOIN BranchProfiles BP ON BP.BranchID = TR.BranchId
WHERE  1=1 ";

                #endregion SqlText

                sqlTextCount += @" 
SELECT count(TR.TransferReceiveNo) RecordCount
FROM TransferMPLReceives TR WHERE  1=1 ";

                if (ids != null && ids.Length > 0)
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

                    sqlText += " AND TR.TransferReceiveNo IN(" + sqlText2 + ")";
                }

                if (Id > 0)
                {
                    sqlTextParameter += @" AND TR.Id=@Id";
                }

                if (!String.IsNullOrEmpty(transactionType))
                {
                    sqlTextParameter += @" AND TR.TransactionType IN (@transactionType)";
                }

                string cField = "";
                if (conditionFields != null && conditionValues != null &&
                    conditionFields.Length == conditionValues.Length)
                {
                    for (int i = 0; i < conditionFields.Length; i++)
                    {
                        if (string.IsNullOrEmpty(conditionFields[i]) || string.IsNullOrEmpty(conditionValues[i]) ||
                            conditionValues[i] == "0")
                        {
                            continue;
                        }

                        cField = conditionFields[i].ToString();
                        cField = OrdinaryVATDesktop.StringReplacing(cField);
                        if (conditionFields[i].ToLower().Contains("like"))
                        {
                            sqlTextParameter += " AND " + conditionFields[i] + " '%'+ " + " @" +
                                                cField.Replace("like", "").Trim() + " +'%'";
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

                if (Orderby == "N")
                {
                    sqlTextOrderBy += " order by TR.Id desc";
                }
                else
                {
                    sqlTextOrderBy += " order by TR.Id desc";
                }

                #region SqlExecution

                sqlText = sqlText + " " + sqlTextParameter + " " + sqlTextOrderBy;
                sqlTextCount = sqlTextCount + " " + sqlTextParameter;
                sqlText = sqlText + " " + sqlTextCount;

                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;
                da.SelectCommand.CommandTimeout = 500;

                if (conditionFields != null && conditionValues != null &&
                    conditionFields.Length == conditionValues.Length)
                {
                    for (int j = 0; j < conditionFields.Length; j++)
                    {
                        if (string.IsNullOrEmpty(conditionFields[j]) || string.IsNullOrEmpty(conditionValues[j]) ||
                            conditionValues[j] == "0")
                        {
                            continue;
                        }

                        cField = conditionFields[j].ToString();
                        cField = OrdinaryVATDesktop.StringReplacing(cField);
                        if (conditionFields[j].ToLower().Contains("like"))
                        {
                            da.SelectCommand.Parameters.AddWithValue("@" + cField.Replace("like", "").Trim(),
                                conditionValues[j]);
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

                if (!String.IsNullOrEmpty(transactionType))
                {
                    da.SelectCommand.Parameters.AddWithValue("@transactionType", transactionType);
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
                FileLogger.Log("TransferReceiveMPLDAL", "SelectAll", sqlex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                FileLogger.Log("TransferReceiveMPLDAL", "SelectAll", ex.ToString() + "\n" + sqlText);
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

        public List<TransferMPLReceiveVM> SelectAllList(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null, string[] ids = null, string transactionType = null, string Orderby = "Y", string SelectTop = "100")
        {
            #region Variables

            string sqlText = "";
            List<TransferMPLReceiveVM> VMs = new List<TransferMPLReceiveVM>();
            TransferMPLReceiveVM vm;

            #endregion

            #region try

            try
            {
                #region sql statement

                #region SqlExecution

                DataTable dt = SelectAll(Id, conditionFields, conditionValues, VcurrConn, Vtransaction, connVM, ids, transactionType, Orderby, SelectTop);
                //dt.Rows.RemoveAt(dt.Rows.Count - 1);
                foreach (DataRow dr in dt.Rows)
                {
                    try
                    {
                        vm = new TransferMPLReceiveVM();
                        vm.Id = Convert.ToInt32(dr["Id"].ToString());
                        vm.TransferReceiveNo = dr["TransferReceiveNo"].ToString();
                        vm.TransferIssueNo = dr["TransferIssueNo"].ToString();
                        vm.TransferIssueMasterRefId = Convert.ToInt32(dr["TransferIssueMasterRefId"].ToString());
                        vm.BranchId = Convert.ToInt32(dr["BranchId"].ToString());
                        vm.TransferFrom = Convert.ToInt32(dr["TransferFrom"].ToString());
                        vm.TransferFromBranch = dr["TransferFromBranch"].ToString();
                        vm.TransferToBranch = dr["TransferToBranch"].ToString();
                        vm.TransferTRCode = dr["TransferTRCode"].ToString();
                        vm.ReceiveTRCode = dr["ReceiveTRCode"].ToString();
                        vm.TransferDateTime = OrdinaryVATDesktop.DateTimeToDate(dr["TransferDateTime"].ToString());
                        vm.ReceiveDateTime = OrdinaryVATDesktop.DateTimeToDate(dr["ReceiveDateTime"].ToString());
                        vm.TotalAmount = Convert.ToDecimal(dr["TotalAmount"].ToString());
                        vm.TransactionType = dr["TransactionType"].ToString();
                        vm.ReportType = dr["ReportType"].ToString();
                        vm.Comments = dr["Comments"].ToString();
                        vm.VehicleType = dr["VehicleType"].ToString();
                        vm.VehicleNo = dr["VehicleNo"].ToString();
                        vm.Post = dr["Post"].ToString();
                        vm.SerialNo = dr["SerialNo"].ToString();
                        vm.ReferenceNo = dr["ReferenceNo"].ToString();
                        vm.TotalVATAmount = Convert.ToDecimal(dr["TotalVATAmount"].ToString());
                        vm.TotalSDAmount = Convert.ToDecimal(dr["TotalSDAmount"].ToString());
                        vm.BranchFromRef = dr["BranchFromRef"].ToString();
                        vm.BranchToRef = dr["BranchToRef"].ToString();
                        vm.IsTransfer = dr["IsTransfer"].ToString();
                        vm.CreatedBy = dr["CreatedBy"].ToString();
                        vm.CreatedOn = dr["CreatedOn"].ToString();
                        vm.LastModifiedBy = dr["LastModifiedBy"].ToString();
                        vm.SignatoryDesig = dr["SignatoryDesig"].ToString();
                        vm.LastModifiedOn = dr["LastModifiedOn"].ToString();
                        vm.RailwayReceiptNo = dr["RailwayReceiptNo"].ToString();
                        vm.RailwayReceiptDate = OrdinaryVATDesktop.DateTimeToDate(dr["RailwayReceiptDate"].ToString());
                        vm.RailwayInvoiceNo = dr["RailwayInvoiceNo"].ToString();
                        vm.WeightChargeed = Convert.ToDecimal(dr["WeightChargeed"].ToString());
                        vm.FreightToPay = Convert.ToDecimal(dr["FreightToPay"].ToString());
                        vm.FreightPrepaid = Convert.ToDecimal(dr["FreightPrepaid"].ToString());
                        vm.DIP = dr["DIP"].ToString();
                        vm.TransferType = dr["TransferType"].ToString();
                        vm.ArrivalDate = OrdinaryVATDesktop.DateTimeToDate(dr["ArrivalDate"].ToString());


                        VMs.Add(vm);
                    }
                    catch (Exception e)
                    {
                        //
                    }
                }

                #endregion SqlExecution

                #endregion
            }

            #endregion

            #region catch

            catch (SqlException sqlex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());

                FileLogger.Log("TransferReceiveMPLDAL", "SelectAllList", sqlex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());

                FileLogger.Log("TransferReceiveMPLDAL", "SelectAllList", ex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", ex.Message.ToString());

            }

            #endregion

            #region finally

            #endregion

            return VMs;
        }


        public List<TransferMPLReceiveDetailVM> SearchTransReceiveMPLDetailList(string transferMPLReceiveId,
            SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            List<TransferMPLReceiveDetailVM> lst = new List<TransferMPLReceiveDetailVM>();
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            DataTable dataTable = new DataTable("TransferMPLReceiveDetails");

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
TD.Id
,TD.TransferMPLReceiveId
,TD.BranchId
,TD.ReceiveLineNo
,ISNULL(TD.TransferFrom,0) TransferFrom
,TD.TransferDateTime
,TD.ReceiveDateTime
,TD.Post
,TD.ItemNo
,TD.RequestedQuantity
,TD.RequestedVolumn
,TD.Quantity
,TD.CostPrice
,TD.UOM
,TD.SubTotal
,TD.LCF
,TD.QU
,TD.IsExcise
,TD.IsCustoms
,TD.Comments
,TD.TransactionType
,TD.ReportType
,TD.UOMQty
,TD.UOMPrice
,TD.UOMc
,TD.UOMn
,TD.WagonNo

,ISNULL(TD.VATRate,0) VATRate
,ISNULL(TD.VATAmount,0) VATAmount
,ISNULL(TD.SDRate,0) SDRate
,ISNULL(TD.SDAmount,0) SDAmount
,TD.PeriodID
,TD.TransferIssueMasterRefId
,TD.TransferIssueDetailsRefId
,ISNULL(TD.TankId,0) TankId
,ISNULL(TD.Temperature,0) Temperature
,ISNULL(TD.SP_Gravity,0) SP_Gravity
,ISNULL(TD.QtyAt30Temperature,0) QtyAt30Temperature
,ISNULL(TD.AlReadyReceivedQuantity,0) AlReadyReceivedQuantity
,ISNULL(TD.IsReceiveCompleted,'N') IsReceiveCompleted
,P.ProductName,p.ProductCode,T.TankCode 

FROM  TransferMPLReceiveDetails TD
LEFT OUTER JOIN Products P ON P.ItemNo = TD.ItemNo
LEFT OUTER JOIN TankMPLs T ON T.Id = TD.TankId AND T.ActiveStatus = 'Y' 
WHERE 1=1  ";

                if (!string.IsNullOrEmpty(transferMPLReceiveId))
                {
                    if (Convert.ToInt32(transferMPLReceiveId) > 0)
                    {
                        sqlText += @" AND TD.TransferMPLReceiveId=@TransferMPLReceiveId";
                    }
                }

                #endregion

                #region SQL Command

                SqlCommand objCommSaleDetail = new SqlCommand();
                objCommSaleDetail.Connection = currConn;
                objCommSaleDetail.Transaction = transaction;
                objCommSaleDetail.CommandText = sqlText;
                objCommSaleDetail.CommandType = CommandType.Text;

                #endregion.

                #region Parameter

                if (!string.IsNullOrEmpty(transferMPLReceiveId))
                {
                    objCommSaleDetail.Parameters.AddWithValue("@TransferMPLReceiveId", transferMPLReceiveId);
                }

                #endregion Parameter

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommSaleDetail);
                dataAdapter.Fill(dataTable);

                lst = dataTable.ToList<TransferMPLReceiveDetailVM>();
            }

            #endregion

            #region Catch & Finally

            catch (SqlException sqlex)
            {
                FileLogger.Log("TransferReceiveMPLDAL", "SearchTransReceiveMPLDetailList",
                    sqlex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                FileLogger.Log("TransferReceiveMPLDAL", "SearchTransReceiveMPLDetailList",
                    ex.ToString() + "\n" + sqlText);
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

            return lst;
        }

        public List<TransferMPLReceiveVM> DropDown(SysDBInfoVMTemp connVM = null)
        {
            throw new NotImplementedException();
        }

        public string[] Delete(TransferMPLReceiveVM vm, string[] ids, SqlConnection VcurrConn = null,
            SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            throw new NotImplementedException();
        }


        // Get Transfer Issue Data

        public List<TransferMPLIssueVM> ReceiveIndex(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null, string[] ids = null, string transactionType = null, string Orderby = "Y", string SelectTop = "100")
        {
            #region Variables

            string sqlText = "";
            List<TransferMPLIssueVM> VMs = new List<TransferMPLIssueVM>();
            TransferMPLIssueVM vm;

            #endregion

            #region try

            try
            {
                #region sql statement

                #region SqlExecution

                DataTable dt = SelectReceiveIndex(Id, conditionFields, conditionValues, VcurrConn, Vtransaction, connVM, ids, transactionType, Orderby, SelectTop);
                //dt.Rows.RemoveAt(dt.Rows.Count - 1);
                foreach (DataRow dr in dt.Rows)
                {
                    try
                    {
                        vm = new TransferMPLIssueVM();
                        vm.Id = Convert.ToInt32(dr["Id"].ToString());
                        vm.TransferIssueNo = dr["TransferIssueNo"].ToString();
                        vm.BranchId = Convert.ToInt32(dr["BranchId"].ToString());
                        vm.TransferTo = Convert.ToInt32(dr["TransferTo"].ToString());
                        vm.TransferToBranch = dr["TransferToBranch"].ToString();

                        vm.TransferTRCode = dr["TransferTRCode"].ToString();
                        vm.ReceiveTRCode = dr["ReceiveTRCode"].ToString();
                        vm.TransferDateTime = OrdinaryVATDesktop.DateTimeToDate(dr["TransferDateTime"].ToString());
                        vm.ReceiveDateTime = OrdinaryVATDesktop.DateTimeToDate(dr["ReceiveDateTime"].ToString());
                        vm.TotalAmount = Convert.ToDecimal(dr["TotalAmount"].ToString());
                        vm.TransactionType = dr["TransactionType"].ToString();
                        vm.Comments = dr["Comments"].ToString();
                        vm.VehicleType = dr["VehicleType"].ToString();
                        vm.VehicleNo = dr["VehicleNo"].ToString();
                        vm.Post = dr["Post"].ToString();
                        vm.SerialNo = dr["SerialNo"].ToString();
                        vm.ReferenceNo = dr["ReferenceNo"].ToString();
                        vm.TotalVATAmount = Convert.ToDecimal(dr["TotalVATAmount"].ToString());
                        vm.TotalSDAmount = Convert.ToDecimal(dr["TotalSDAmount"].ToString());
                        vm.BranchFromRef = dr["BranchFromRef"].ToString();
                        vm.BranchToRef = dr["BranchToRef"].ToString();
                        vm.IsTransfer = dr["IsTransfer"].ToString();
                        vm.CreatedBy = dr["CreatedBy"].ToString();
                        vm.CreatedOn = dr["CreatedOn"].ToString();
                        vm.LastModifiedBy = dr["LastModifiedBy"].ToString();
                        vm.SignatoryDesig = dr["SignatoryDesig"].ToString();
                        vm.LastModifiedOn = dr["LastModifiedOn"].ToString();
                        vm.RailwayReceiptNo = dr["RailwayReceiptNo"].ToString();
                        vm.RailwayReceiptDate = OrdinaryVATDesktop.DateTimeToDate(dr["RailwayReceiptDate"].ToString());
                        vm.RailwayInvoiceNo = dr["RailwayInvoiceNo"].ToString();
                        vm.WeightChargeed = Convert.ToDecimal(dr["WeightChargeed"].ToString());
                        vm.FreightToPay = Convert.ToDecimal(dr["FreightToPay"].ToString());
                        vm.FreightPrepaid = Convert.ToDecimal(dr["FreightPrepaid"].ToString());

                        vm.BatchNo = dr["BatchNo"].ToString();
                        vm.BatchDate = OrdinaryVATDesktop.DateTimeToDate(dr["BatchDate"].ToString());
                        vm.TestReportNo = dr["TestReportNo"].ToString();
                        vm.TestReportDate = OrdinaryVATDesktop.DateTimeToDate(dr["TestReportDate"].ToString());
                        vm.TestReportTempPF = Convert.ToDecimal(dr["TestReportTempPF"].ToString());
                        vm.TestReportSPGR = Convert.ToDecimal(dr["TestReportSPGR"].ToString());
                        vm.DepartureDate = OrdinaryVATDesktop.DateTimeToDate(dr["DepartureDate"].ToString());

                        VMs.Add(vm);
                    }
                    catch (Exception e)
                    {
                        //
                    }
                }

                #endregion SqlExecution

                #endregion
            }

            #endregion

            #region catch

            catch (SqlException sqlex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());

                FileLogger.Log("MPLSaleDAL", "SelectAllList", sqlex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());

                FileLogger.Log("MPLSaleDAL", "SelectAllList", ex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", ex.Message.ToString());

            }

            #endregion

            #region finally

            #endregion

            return VMs;
        }

        public DataTable SelectReceiveIndex(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null, string[] ids = null, string transactionType = null, string Orderby = "Y", string SelectTop = "100")
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
            string count = SelectTop;

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
                }

                sqlText += @"

TI.Id,
TI.TransferIssueNo,
TI.BranchId,
ISNULL(TI.TransferTo, '0') TransferTo,
ISNULL(B.BranchName, '') TransferToBranch,
TI.TransferTRCode,
TI.ReceiveTRCode,
ISNULL(TI.TransferDateTime, '')TransferDateTime,
ISNULL(TI.ReceiveDateTime, '')ReceiveDateTime,
ISNULL(TI.RailwayReceiptDate, '')RailwayReceiptDate,
TI.TotalAmount,
TI.TransactionType,
TI.Comments,
TI.VehicleType,
TI.VehicleNo,
ISNULL(TI.Post, 'N') Post,
TI.SerialNo,
TI.ReferenceNo,
TI.TotalVATAmount,
TI.TotalSDAmount,
TI.BranchFromRef,
TI.BranchToRef,
TI.IsTransfer,
TI.CreatedBy,
TI.CreatedOn,
TI.LastModifiedBy,
TI.SignatoryDesig,
TI.LastModifiedOn,
TI.RailwayReceiptNo,
TI.RailwayInvoiceNo,
ISNULL(TI.WeightChargeed, '0') WeightChargeed,
ISNULL(TI.FreightToPay, '0') FreightToPay,
ISNULL(TI.FreightPrepaid, '0') FreightPrepaid

,ISNULL(TI.BatchNo, '') BatchNo
,ISNULL(TI.BatchDate, '') BatchDate
,ISNULL(TI.TestReportNo, '') TestReportNo
,ISNULL(TI.TestReportDate, '') TestReportDate
,ISNULL(TI.TestReportTempPF, '0') TestReportTempPF
,ISNULL(TI.TestReportSPGR, '0') TestReportSPGR
,ISNULL(TI.DepartureDate, '') DepartureDate
,TI.TransferType
FROM TransferMPLIssues TI
LEFT OUTER JOIN BranchProfiles B ON B.BranchID = TI.BranchId
WHERE  1=1 and TI.Id in ( Select Distinct TransferMPLIssueId FROM TransferMPLIssueDetails where isnull(IsReceiveCompleted,'N')='N')  AND TI.Post = 'Y' ";

                #endregion SqlText

                sqlTextCount += @" 
SELECT count(TI.TransferIssueNo) RecordCount
FROM TransferMPLIssues TI WHERE  1=1  and TI.Id in ( Select Distinct TransferMPLIssueId FROM TransferMPLIssueDetails where isnull(IsReceiveCompleted,'N')='N')   AND TI.Post = 'Y'  ";

                if (ids != null && ids.Length > 0)
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

                    sqlText += " AND TI.TransferIssueNo IN(" + sqlText2 + ")";
                }

                if (Id > 0)
                {
                    sqlTextParameter += @" AND TI.Id=@Id";
                }

                if (!String.IsNullOrEmpty(transactionType))
                {
                    sqlTextParameter += @" AND TI.TransactionType IN (@transactionType)";
                }

                string cField = "";
                if (conditionFields != null && conditionValues != null &&
                    conditionFields.Length == conditionValues.Length)
                {
                    for (int i = 0; i < conditionFields.Length; i++)
                    {
                        if (string.IsNullOrEmpty(conditionFields[i]) || string.IsNullOrEmpty(conditionValues[i]) ||
                            conditionValues[i] == "0")
                        {
                            continue;
                        }

                        cField = conditionFields[i].ToString();
                        cField = OrdinaryVATDesktop.StringReplacing(cField);
                        if (conditionFields[i].ToLower().Contains("like"))
                        {
                            sqlTextParameter += " AND " + conditionFields[i] + " '%'+ " + " @" +
                                                cField.Replace("like", "").Trim() + " +'%'";
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

                if (Orderby == "N")
                {
                    sqlTextOrderBy += " order by TI.Id desc";
                }
                else
                {
                    sqlTextOrderBy += " order by TI.Id desc";
                }

                #region SqlExecution

                sqlText = sqlText + " " + sqlTextParameter + " " + sqlTextOrderBy;
                sqlTextCount = sqlTextCount + " " + sqlTextParameter;
                sqlText = sqlText + " " + sqlTextCount;

                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;
                da.SelectCommand.CommandTimeout = 500;

                if (conditionFields != null && conditionValues != null &&
                    conditionFields.Length == conditionValues.Length)
                {
                    for (int j = 0; j < conditionFields.Length; j++)
                    {
                        if (string.IsNullOrEmpty(conditionFields[j]) || string.IsNullOrEmpty(conditionValues[j]) ||
                            conditionValues[j] == "0")
                        {
                            continue;
                        }

                        cField = conditionFields[j].ToString();
                        cField = OrdinaryVATDesktop.StringReplacing(cField);
                        if (conditionFields[j].ToLower().Contains("like"))
                        {
                            da.SelectCommand.Parameters.AddWithValue("@" + cField.Replace("like", "").Trim(),
                                conditionValues[j]);
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

                if (!String.IsNullOrEmpty(transactionType))
                {
                    da.SelectCommand.Parameters.AddWithValue("@transactionType", transactionType);
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
                FileLogger.Log("MPLSaleDAL", "SelectAll", sqlex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                FileLogger.Log("MPLSaleDAL", "SelectAll", ex.ToString() + "\n" + sqlText);
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

        public List<TransferMPLReceiveVM> GetTransIssueAll(string Id, SqlConnection VcurrConn = null,
            SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            List<TransferMPLReceiveVM> VMs = new List<TransferMPLReceiveVM>();
            TransferMPLReceiveVM vm;

            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            DataTable dataTable = new DataTable("TransferMPLIssueDetail");

            #endregion

            #region Try

            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                #region SQL Statement

                sqlText = @" 
SELECT 
Id,
Id TransferIssueMasterRefId,
TransferIssueNo,
TransferReceiveNo,
BranchId,
ISNULL(BranchId, '0') TransferFrom,
TransferTRCode,
TransferTRCode ReceiveTRCode,
ISNULL(TransferDateTime, '')TransferDateTime,
ISNULL(ReceiveDateTime, '')ReceiveDateTime,
ISNULL(RailwayReceiptDate, '')RailwayReceiptDate,
TotalAmount,
TransactionType,
Comments,
VehicleType,
VehicleNo,
Post,
SerialNo,
ReferenceNo,
TotalVATAmount,
TotalSDAmount,
BranchToRef BranchFromRef,
BranchToRef,
IsTransfer,
CreatedBy,
CreatedOn,
LastModifiedBy,
SignatoryDesig,
LastModifiedOn,
RailwayReceiptNo,
RailwayInvoiceNo,
ReportType,
ISNULL(WeightChargeed, '0') WeightChargeed,
ISNULL(FreightToPay, '0') FreightToPay,
ISNULL(FreightPrepaid, '0') FreightPrepaid

,ISNULL(BatchNo, '') BatchNo
,ISNULL(BatchDate, '') BatchDate
,ISNULL(TestReportNo, '') TestReportNo
,ISNULL(TestReportDate, '') TestReportDate
,ISNULL(TestReportTempPF, '0') TestReportTempPF
,ISNULL(TestReportSPGR, '0') TestReportSPGR
,ISNULL(DepartureDate, '') DepartureDate
,TransferType

FROM TransferMPLIssues 
WHERE  1=1  ";

                if (!string.IsNullOrEmpty(Id))
                {
                    if (Convert.ToInt32(Id) > 0)
                    {
                        sqlText += @" AND Id=@Id";
                    }
                }

                #endregion

                #region SQL Command

                SqlCommand objCommSaleDetail = new SqlCommand();
                objCommSaleDetail.Connection = currConn;
                objCommSaleDetail.CommandText = sqlText;
                objCommSaleDetail.CommandType = CommandType.Text;

                #endregion.

                #region Parameter

                if (!string.IsNullOrEmpty(Id))
                {
                    objCommSaleDetail.Parameters.AddWithValue("@Id", Id);
                }

                #endregion Parameter

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommSaleDetail);
                dataAdapter.Fill(dataTable);

                foreach (DataRow dr in dataTable.Rows)
                {
                    try
                    {
                        vm = new TransferMPLReceiveVM();
                        vm.Id = Convert.ToInt32(dr["Id"].ToString());
                        vm.TransferIssueMasterRefId = Convert.ToInt32(dr["TransferIssueMasterRefId"].ToString());
                        vm.TransferReceiveNo = dr["TransferReceiveNo"].ToString();
                        vm.TransferIssueNo = dr["TransferIssueNo"].ToString();
                        vm.BranchId = Convert.ToInt32(dr["BranchId"].ToString());
                        vm.TransferFrom = Convert.ToInt32(dr["TransferFrom"].ToString());
                        vm.TransferTRCode = dr["TransferTRCode"].ToString();
                        vm.ReceiveTRCode = dr["ReceiveTRCode"].ToString();
                        vm.TransferDateTime = OrdinaryVATDesktop.DateTimeToDate(dr["TransferDateTime"].ToString());
                        vm.ReceiveDateTime = OrdinaryVATDesktop.DateTimeToDate(dr["ReceiveDateTime"].ToString());
                        vm.TotalAmount = Convert.ToDecimal(dr["TotalAmount"].ToString());
                        vm.TransactionType = dr["TransactionType"].ToString();
                        vm.ReportType = dr["ReportType"].ToString();
                        vm.Comments = dr["Comments"].ToString();
                        vm.VehicleType = dr["VehicleType"].ToString();
                        vm.VehicleNo = dr["VehicleNo"].ToString();
                        vm.Post = dr["Post"].ToString();
                        vm.SerialNo = dr["SerialNo"].ToString();
                        vm.ReferenceNo = dr["ReferenceNo"].ToString();
                        vm.TotalVATAmount = Convert.ToDecimal(dr["TotalVATAmount"].ToString());
                        vm.TotalSDAmount = Convert.ToDecimal(dr["TotalSDAmount"].ToString());
                        vm.BranchFromRef = dr["BranchFromRef"].ToString();
                        vm.BranchToRef = dr["BranchToRef"].ToString();
                        vm.IsTransfer = dr["IsTransfer"].ToString();
                        vm.CreatedBy = dr["CreatedBy"].ToString();
                        vm.CreatedOn = dr["CreatedOn"].ToString();
                        vm.LastModifiedBy = dr["LastModifiedBy"].ToString();
                        vm.SignatoryDesig = dr["SignatoryDesig"].ToString();
                        vm.LastModifiedOn = dr["LastModifiedOn"].ToString();
                        vm.RailwayReceiptNo = dr["RailwayReceiptNo"].ToString();
                        vm.RailwayReceiptDate = OrdinaryVATDesktop.DateTimeToDate(dr["RailwayReceiptDate"].ToString());
                        vm.RailwayInvoiceNo = dr["RailwayInvoiceNo"].ToString();
                        vm.WeightChargeed = Convert.ToDecimal(dr["WeightChargeed"].ToString());
                        vm.FreightToPay = Convert.ToDecimal(dr["FreightToPay"].ToString());
                        vm.FreightPrepaid = Convert.ToDecimal(dr["FreightPrepaid"].ToString());
                        vm.TransferType = dr["TransferType"].ToString();

                        vm.BatchNo = dr["BatchNo"].ToString();
                        vm.BatchDate = OrdinaryVATDesktop.DateTimeToDate(dr["BatchDate"].ToString());
                        vm.TestReportNo = dr["TestReportNo"].ToString();
                        vm.TestReportDate = OrdinaryVATDesktop.DateTimeToDate(dr["TestReportDate"].ToString());
                        vm.TestReportTempPF = Convert.ToDecimal(dr["TestReportTempPF"].ToString());
                        vm.TestReportSPGR = Convert.ToDecimal(dr["TestReportSPGR"].ToString());
                        vm.DepartureDate = OrdinaryVATDesktop.DateTimeToDate(dr["DepartureDate"].ToString());


                        VMs.Add(vm);
                    }
                    catch (Exception e)
                    {
                        //
                    }
                }
            }

            #endregion

            #region Catch & Finally

            catch (SqlException sqlex)
            {
                FileLogger.Log("TransferReceiveMPLDAL", "SearchTransIssueMPLDetailList",
                    sqlex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                FileLogger.Log("TransferReceiveMPLDAL", "SearchTransIssueMPLDetailList",
                    ex.ToString() + "\n" + sqlText);
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

            #endregion

            return VMs;
        }

        public List<TransferMPLReceiveDetailVM> SearchTransIssueMPLDetailList(string transferMPLIssueId,
            SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            List<TransferMPLReceiveDetailVM> lst = new List<TransferMPLReceiveDetailVM>();
            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            DataTable dataTable = new DataTable("TransferMPLIssueDetail");

            #endregion

            #region Try

            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                #region SQL Statement

                sqlText = @" 
SELECT 
TD.Id
,TD.TransferMPLIssueId TransferMPLReceiveId
,TD.TransferMPLIssueId TransferIssueMasterRefId
,TD.Id TransferIssueDetailsRefId
,TD.BranchId
,TD.IssueLineNo ReceiveLineNo
,ISNULL(TD.TransferTo,0) TransferFrom
,TD.TransferDateTime
,TD.ReceiveDateTime
,TD.Post
,TD.ItemNo
,TD.RequestedQuantity
,TD.RequestedVolumn
,TD.Quantity
,TD.CostPrice
,TD.UOM
,TD.SubTotal
,TD.LCF
,TD.QU
,TD.IsExcise
,TD.IsCustoms
,TD.Comments
,TD.TransactionType
,TD.UOMQty
,TD.UOMPrice
,TD.UOMc
,TD.UOMn
,TD.WagonNo

,ISNULL(TD.VATRate,0) VATRate
,ISNULL(TD.VATAmount,0) VATAmount
,ISNULL(TD.SDRate,0) SDRate
,ISNULL(TD.SDAmount,0) SDAmount
,TD.PeriodID
,ISNULL(TD.TankId,0) TankId
,ISNULL(TD.Temperature,0) Temperature
,ISNULL(TD.SP_Gravity,0) SP_Gravity
,ISNULL(TD.QtyAt30Temperature,0) QtyAt30Temperature
,ISNULL(TD.ReceivedQuantity,0) AlReadyReceivedQuantity
,ISNULL(TD.IsReceiveCompleted,'N') IsReceiveCompleted
,P.ProductName,p.ProductCode,T.TankCode 

FROM  TransferMPLIssueDetails TD
LEFT OUTER JOIN Products P ON P.ItemNo = TD.ItemNo
LEFT OUTER JOIN TankMPLs T ON T.Id = TD.TankId AND T.ActiveStatus = 'Y' 
WHERE 1=1  ";

                if (!string.IsNullOrEmpty(transferMPLIssueId))
                {
                    if (Convert.ToInt32(transferMPLIssueId) > 0)
                    {
                        sqlText += @" AND TD.TransferMPLIssueId=@TransferMPLIssueId";
                    }
                }

                #endregion

                #region SQL Command

                SqlCommand objCommSaleDetail = new SqlCommand();
                objCommSaleDetail.Connection = currConn;
                objCommSaleDetail.CommandText = sqlText;
                objCommSaleDetail.CommandType = CommandType.Text;

                #endregion.

                #region Parameter

                if (!string.IsNullOrEmpty(transferMPLIssueId))
                {
                    objCommSaleDetail.Parameters.AddWithValue("@TransferMPLIssueId", transferMPLIssueId);
                }

                #endregion Parameter

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommSaleDetail);
                dataAdapter.Fill(dataTable);

                lst = dataTable.ToList<TransferMPLReceiveDetailVM>();
            }

            #endregion

            #region Catch & Finally

            catch (SqlException sqlex)
            {
                FileLogger.Log("TransferReceiveMPLDAL", "SearchTransIssueMPLDetailList",
                    sqlex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                FileLogger.Log("TransferReceiveMPLDAL", "SearchTransIssueMPLDetailList",
                    ex.ToString() + "\n" + sqlText);
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

            #endregion

            return lst;
        }


        public string[] TransferMPLReceivePost(TransferMPLReceiveVM MasterVM, SqlTransaction transaction, SqlConnection currConn, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";
            string Id = "";
            string ids = "";

            int transResult = 0;
            string sqlText = "";
            string PostStatus = "";

            #endregion Initializ

            #region Try
            try
            {

                #region Validation for Header

                if (MasterVM == null)
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgNoDataToSave);
                }

                //for (int i = 0; i < MasterVM.IDs.Count; i++)
                //{
                //    if (string.IsNullOrEmpty(MasterVM.IDs[i]))
                //    {
                //        continue;
                //    }
                //    if (string.IsNullOrEmpty(ids))
                //    {
                //        ids = MasterVM.IDs[i];
                //    }
                //    else
                //    {
                //        ids += "," + MasterVM.IDs[i];
                //    }
                //}

                #endregion Validation for Header

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
                    transaction = currConn.BeginTransaction(MessageVM.saleMsgMethodNameInsert);
                }


                #endregion open connection and transaction


               
                    for (int i = 0; i < MasterVM.IDs.Count; i++)
                    {
                        if (string.IsNullOrEmpty(MasterVM.IDs[i]))
                        {
                            continue;
                        }
                        if (string.IsNullOrEmpty(ids))
                        {
                            ids = MasterVM.IDs[i];
                        }
                        else
                        {
                            ids= MasterVM.IDs[i];
                        }

                        sqlText +=
                           @" UPDATE TransferMPLReceives SET Post='Y',LastModifiedBy=@LastModifiedBy,LastModifiedOn=GETDATE() ";
                        sqlText += @"  WHERE Id IN (" + ids + ") ";
                        sqlText +=
                            @" UPDATE TransferMPLReceiveDetails SET Post='Y' ";
                        sqlText += @"  WHERE TransferMPLReceiveId IN (" + ids + ") ";

                        SqlCommand cmdPostUpdate = new SqlCommand(sqlText, currConn, transaction);
                        cmdPostUpdate.Parameters.AddWithValueAndNullHandle("@LastModifiedBy", MasterVM.LastModifiedBy);


                        transResult = cmdPostUpdate.ExecuteNonQuery();
                       

                        TransferMPLReceiveVM vm = SelectAllList(Convert.ToInt32(ids), null, null, currConn, transaction, connVM, null, null).FirstOrDefault();
                        if (vm != null)
                        {
                            TransferReceiveVM TransferReceive = new TransferReceiveVM();
                            TransferReceive.TransferReceiveNo = vm.TransferReceiveNo;
                            TransferReceive.LastModifiedOn = MasterVM.LastModifiedOn;
                            TransferReceive.LastModifiedBy = MasterVM.LastModifiedBy;
                            TransferReceive.TransactionDateTime = vm.TransferDateTime;
                            #region Update Transfer Issue Data
                            var detailVMs = SearchTransReceiveMPLDetailList(ids, currConn, transaction, connVM);
                            foreach (TransferMPLReceiveDetailVM vmD in detailVMs)
                            {
                                sqlText = "";
                                sqlText += @"

DECLARE @TotalQuantity DECIMAL(18, 2);

SELECT @TotalQuantity =  SUM(Quantity)
FROM TransferMPLReceiveDetails
WHERE BranchId = @BranchId
  AND TransferIssueDetailsRefId = @TransferIssueDetailsRefId
  AND ItemNo = @ItemNo
  AND Post ='Y'
GROUP BY ItemNo

Update TransferMPLIssueDetails set ReceivedQuantity=@TotalQuantity
where Id in(@TransferIssueDetailsRefId)   AND ItemNo = @ItemNo

Update TransferMPLReceiveDetails set AlreadyReceivedQuantity=@TotalQuantity
where TransferIssueDetailsRefId in(@TransferIssueDetailsRefId)  AND ItemNo = @ItemNo";

                                SqlCommand cmdUpdateQuantity = new SqlCommand(sqlText, currConn, transaction);
                                cmdUpdateQuantity.Parameters.AddWithValueAndNullHandle("@TransferIssueDetailsRefId", vmD.TransferIssueDetailsRefId);
                                cmdUpdateQuantity.Parameters.AddWithValueAndNullHandle("@BranchId", vmD.BranchId);
                                cmdUpdateQuantity.Parameters.AddWithValueAndNullHandle("@ItemNo", vmD.ItemNo);

                                transResult = cmdUpdateQuantity.ExecuteNonQuery();
                            }
                           
                            #endregion



                            retResults = _TransferReceiveDAL.Post(TransferReceive, connVM, currConn, transaction);
                        }
                    }
             


                #region Commit

                if (transaction != null)
                {
                    transaction.Commit();
                }

                #endregion

                #region SuccessResult
                retResults = new string[5];
                retResults[0] = "Success";
                retResults[1] = MessageVM.saleMsgSuccessfullyPost;
                retResults[2] = "" + MasterVM.TransferReceiveNo;
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
                if (currConn != null)
                {
                    transaction.Rollback();
                }
                FileLogger.Log("TransferReceiveMPLDAL", "TransferMPLReceivePost", ex.ToString() + "\n" + sqlText);

            }
            finally
            {
                if (currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }

            #endregion Catch and Finall

            #region Result
            return retResults;
            #endregion Result
        }


        public string[] MultipleReceiveSave(string[] Ids, string transactionType, int BranchId, string TransactionDateTime, string CurrentUser = "", SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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
                
                for (int i = 0; i < Ids.Length; i++)
                {
                    TransferMPLReceiveVM vm = new TransferMPLReceiveVM();
                    DataTable dt = new DataTable();
                    List<TransferMPLReceiveDetailVM> receiveDetails = new List<TransferMPLReceiveDetailVM>();

                    #region Master data select

                    List<TransferMPLReceiveVM> vmList = GetTransIssueAll(Ids[i], currConn,transaction, connVM);

                    foreach (var item in vmList)
                    {
                        try
                        {
                            vm = new TransferMPLReceiveVM();
                            vm.Id = item.Id;
                            vm.TransferReceiveNo = item.TransferReceiveNo;
                            vm.TransferIssueNo = item.TransferIssueNo;
                            vm.BranchId = BranchId;
                            vm.TransferFrom = item.TransferFrom;
                            vm.TransferTRCode = item.TransferTRCode;
                            vm.ReceiveTRCode = item.ReceiveTRCode;
                            vm.TransferDateTime = item.TransferDateTime;
                            vm.ReceiveDateTime = TransactionDateTime;
                            vm.TotalAmount = item.TotalAmount;
                            vm.TransactionType = item.TransactionType;
                            vm.Comments = item.Comments;
                            vm.VehicleType = item.VehicleType;
                            vm.VehicleNo = item.VehicleNo;
                            vm.Post = item.Post;
                            vm.SerialNo = item.SerialNo;
                            vm.ReferenceNo = item.ReferenceNo;
                            vm.TotalVATAmount = item.TotalVATAmount;
                            vm.TotalSDAmount = item.TotalSDAmount;
                            vm.BranchFromRef = item.BranchFromRef;
                            vm.BranchToRef = item.BranchToRef;
                            vm.IsTransfer = item.IsTransfer;
                            vm.CreatedBy = item.CreatedBy;
                            vm.CreatedOn = item.CreatedOn;
                            vm.LastModifiedBy = item.LastModifiedBy;
                            vm.SignatoryDesig = item.SignatoryDesig;
                            vm.LastModifiedOn = item.LastModifiedOn;
                            vm.RailwayReceiptNo = item.RailwayReceiptNo;
                            vm.RailwayReceiptDate = item.RailwayReceiptDate;
                            vm.RailwayInvoiceNo = item.RailwayInvoiceNo;
                            vm.WeightChargeed = item.WeightChargeed;
                            vm.FreightToPay = item.FreightToPay;
                            vm.FreightPrepaid = item.FreightPrepaid;
                        }
                        catch (Exception e)
                        {
                            //
                        }

                    }
                    
                    #endregion

                    #region Transfer Detail

                    if (!string.IsNullOrEmpty(vm.TransferIssueNo))
                    {
                        receiveDetails =  SearchTransIssueMPLDetailList(vm.Id.ToString(), currConn, transaction, connVM);
                        vm.TransferMPLReceiveDetailVMs = receiveDetails;
                    }

                    #endregion

                    retResults = TransReceiveMPLInsert(vm,transaction,currConn, connVM);

                    if (retResults[0] != "Success")
                    {
                        throw new Exception(retResults[1]);
                    }
                    else
                    {
                        retResults = new string[5];
                        retResults[0] = "Success";
                        retResults[1] = MessageVM.saleMsgSaveSuccessfully;
                        retResults[2] = "" + vm.TransferReceiveNo;
                        retResults[3] = "N";
                        retResults[4] = "0";
                    }
                }

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion
            }
            #region Exception
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
            #region Result
            return retResults;
            #endregion Result
        }

    }
}
