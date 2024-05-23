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
    public class TransferIssueMPLDAL : ITransferIssueMPL
    {
        #region Global Variables

        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        CommonDAL _cDAL = new CommonDAL();
        TransferIssueDAL _TransferIssueDAL = new TransferIssueDAL();

        #endregion


        public string[] TransIssueMPLInsert(TransferMPLIssueVM MasterVM, SqlTransaction Vtransaction = null, SqlConnection VcurrConn = null, SysDBInfoVMTemp connVM = null)
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
            TransferIssueVM TransferIssueMaster = new TransferIssueVM();
            List<TransferIssueDetailVM> Details = new List<TransferIssueDetailVM>();
            #endregion Initializ

            #region Try
            try
            {

                #region Validation for Header


                if (MasterVM == null)
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgNoDataToSave);
                }
                else if (Convert.ToDateTime(MasterVM.TransferDateTime) < DateTime.MinValue || Convert.ToDateTime(MasterVM.TransferDateTime) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, "Please Check Invoice Data and Time");

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

                sqlText = "";
                sqlText = sqlText + "select COUNT(TransferIssueNo) from TransferMPLIssues WHERE TransferIssueNo=@MasterTransferIssueNo ";
                SqlCommand cmdExistTran = new SqlCommand(sqlText, currConn, transaction);
                cmdExistTran.Parameters.AddWithValueAndNullHandle("@MasterTransferIssueNo", MasterVM.TransferIssueNo);
                IDExist = (int)cmdExistTran.ExecuteScalar();

                if (IDExist > 0)
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgFindExistID);
                }

                #endregion Find Transaction Exist

                #region Sale ID Create
                if (string.IsNullOrEmpty(MasterVM.TransactionType)) // start
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.msgTransactionNotDeclared);
                }
                if (string.IsNullOrEmpty(MasterVM.TransferType)) // start
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.msgTransactionNotDeclared);
                }
                #region Other

                if (MasterVM.TransactionType == "62Out")
                {
                    newIDCreate = commonDal.TransactionCode("Transfer", MasterVM.TransferType, "TransferMPLIssues", "TransferIssueNo",
                                              "TransferDateTime", MasterVM.TransferDateTime, MasterVM.BranchId.ToString(), currConn, transaction);
                }

                #endregion


                if (string.IsNullOrEmpty(newIDCreate) || newIDCreate == "")
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert,
                                                            "ID Prefetch not set please update Prefetch first");
                }


                #region checkId

                sqlText = @"
SELECT COUNT(TransferIssueNo) FROM TransferMPLIssues 
where TransferIssueNo = @MasterTransferIssueNo and TransactionType = @TransactionType and BranchId = @BranchId";

                var sqlCmd = new SqlCommand(sqlText, currConn, transaction);
                sqlCmd.Parameters.AddWithValue("@MasterTransferIssueNo", newIDCreate);
                sqlCmd.Parameters.AddWithValue("@TransactionType", MasterVM.TransactionType);
                sqlCmd.Parameters.AddWithValue("@BranchId", MasterVM.BranchId);

                var count = (int)sqlCmd.ExecuteScalar();

                if (count > 0)
                {
                    FileLogger.Log("TransferIssueMPLDAL", "Insert", "TransferIssueNo " + newIDCreate + " Already Exists");
                    throw new Exception("Trans Id " + newIDCreate + " Already Exists");
                }

                #endregion

                #region Check Existing Id

                sqlText = "select count(TransferIssueNo) from TransferMPLIssues where TransferIssueNo = @MasterTransferIssueNo";

                var cmd = new SqlCommand(sqlText, currConn, transaction);
                cmd.Parameters.AddWithValue("@MasterTransferIssueNo", newIDCreate);

                var count1 = (int)cmd.ExecuteScalar();

                if (count1 > 0)
                {
                    FileLogger.Log("TransferIssueMPLDAL", "Insert", "Trans Id " + newIDCreate + " Already Exists");
                    throw new Exception("Trans Id " + newIDCreate + " Already Exists");
                }

                #endregion

                MasterVM.TransferIssueNo = newIDCreate;

                #endregion Purchase ID Create Not Complete

                #region ID generated completed,Insert new Information in Header

                MasterVM.TotalAmount = Convert.ToDecimal(commonDal.decimal259(MasterVM.TotalAmount));
                MasterVM.TotalSDAmount = Convert.ToDecimal(commonDal.decimal259(MasterVM.TotalSDAmount));
                MasterVM.TotalVATAmount = Convert.ToDecimal(commonDal.decimal259(MasterVM.TotalVATAmount));

                MasterVM.TransferDateTime = OrdinaryVATDesktop.DateToDate(MasterVM.TransferDateTime);
                MasterVM.ReceiveDateTime = OrdinaryVATDesktop.DateToDate(MasterVM.ReceiveDateTime);
                MasterVM.RailwayReceiptDate = OrdinaryVATDesktop.DateToDate(MasterVM.RailwayReceiptDate);
                MasterVM.BatchDate = OrdinaryVATDesktop.DateToDate(MasterVM.BatchDate);
                MasterVM.DepartureDate = OrdinaryVATDesktop.DateToDate(MasterVM.DepartureDate);
                MasterVM.TestReportDate = OrdinaryVATDesktop.DateToDate(MasterVM.TestReportDate);

                MasterVM.Post = "N";
                MasterVM.IsTransfer = "N";
                retResults = TransIssueMPLInsertToMaster(MasterVM, currConn, transaction, null, settings);

                Id = retResults[4];

                if (retResults[0] != "Success")
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, retResults[1]);
                }

                #endregion ID generated completed,Insert new Information in Header


                #region Insert into Details(Insert complete in Header)

                if (MasterVM.TransferMPLIssueDetailVMs != null)
                {
                    if (MasterVM.TransferMPLIssueDetailVMs.Count() < 0)
                    {
                        throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgNoDataToSave);
                    }
                    int IssueLineNo = 1;
                    foreach (TransferMPLIssueDetailVM collection in MasterVM.TransferMPLIssueDetailVMs)
                    {

                        #region TransferIssueDeatils

                        TransferIssueDetailVM transferDetails = new TransferIssueDetailVM();
                        transferDetails.TransferIssueNo = newIDCreate;
                        transferDetails.ItemNo = collection.ItemNo;
                        transferDetails.BranchId = MasterVM.BranchId;
                        transferDetails.TransferTo = MasterVM.TransferTo;
                        transferDetails.TransactionDateTime = MasterVM.TransferDateTime;
                        transferDetails.Post = "N";

                        transferDetails.UOMQty = collection.Quantity * collection.UOMc;
                        transferDetails.Quantity = collection.UOMQty;
                        transferDetails.CostPrice = collection.CostPrice;

                        transferDetails.UOMPrice = (collection.CostPrice * collection.UOMQty);
                        transferDetails.SubTotal = transferDetails.UOMPrice;

                        transferDetails.UOMn = collection.UOM;
                        transferDetails.UOM = transferDetails.UOMn;
                        transferDetails.UOMc = collection.UOMc;
                        transferDetails.IssueLineNo = IssueLineNo.ToString();

                        transferDetails.Weight = "";
                        transferDetails.OtherRef = "";
                        transferDetails.Comments = "";

                        transferDetails.TransactionType = MasterVM.TransactionType;

                        transferDetails.CreatedBy = MasterVM.CreatedBy;
                        transferDetails.CreatedOn = MasterVM.CreatedOn;
                        transferDetails.LastModifiedBy = MasterVM.LastModifiedBy;
                        transferDetails.LastModifiedOn = MasterVM.LastModifiedOn;

                        Details.Add(transferDetails);


                        #endregion

                        collection.TransferMPLIssueId = Convert.ToInt32(Id);
                        collection.ReportType = MasterVM.ReportType;

                        collection.BranchId = MasterVM.BranchId;
                        collection.TransferDateTime = MasterVM.TransferDateTime;
                        collection.ReceiveDateTime = MasterVM.ReceiveDateTime;
                        collection.Post = "N";

                        collection.UOMQty = collection.Quantity * collection.UOMc;
                        collection.UOMPrice = (collection.CostPrice * collection.UOMQty);
                        collection.UOMn = collection.UOMn;
                        collection.IssueLineNo = IssueLineNo.ToString();

                        retResults = TransIssueMPLInsertToDetails(collection, currConn, transaction, null, settings);
                        IssueLineNo++;
                    }
                }

                if (retResults[0] != "Success")
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, retResults[1]);
                }

                #region TransferIssueInsert
                TransferIssueMaster.TransferIssueNo = MasterVM.TransferIssueNo;
                TransferIssueMaster.TransactionDateTime = MasterVM.TransferDateTime;
                TransferIssueMaster.TransferTo = MasterVM.TransferTo;
                TransferIssueMaster.ReferenceNo = MasterVM.ReferenceNo;
                TransferIssueMaster.BranchId = MasterVM.BranchId;
                TransferIssueMaster.VehicleNo = MasterVM.VehicleNo;
                TransferIssueMaster.VehicleType = MasterVM.VehicleType;
                TransferIssueMaster.TransactionType = MasterVM.TransactionType;
                TransferIssueMaster.Post = "N";
                TransferIssueMaster.CreatedBy = MasterVM.CreatedBy;
                TransferIssueMaster.CreatedOn = MasterVM.CreatedOn;
                TransferIssueMaster.LastModifiedBy = MasterVM.CreatedOn;
                TransferIssueMaster.LastModifiedOn = MasterVM.CreatedOn;
                retResults = _TransferIssueDAL.Insert(TransferIssueMaster, Details, transaction, currConn, connVM, null, false);
                #endregion

                #endregion Insert into Details(Insert complete in Header)

                if (retResults[0] != "Success")
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, retResults[1]);
                }
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
                retResults[2] = "" + MasterVM.TransferIssueNo;
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
                FileLogger.Log("SaleDAL", "SalesInsert", ex.ToString() + "\n" + sqlText);

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

        public string[] TransIssueMPLInsertToMaster(TransferMPLIssueVM Master, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null, DataTable settingsDt = null)
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
                sqlText += " INSERT INTO TransferMPLIssues";
                sqlText += " (";
                sqlText += " TransferIssueNo";
                sqlText += " ,BranchId";
                sqlText += " ,TransferTo";
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
                sqlText += " ,BatchNo";
                sqlText += " ,BatchDate";
                sqlText += " ,TestReportNo";
                sqlText += " ,TestReportDate";
                sqlText += " ,TestReportTempPF";
                sqlText += " ,TestReportSPGR";
                sqlText += " ,DepartureDate";
                sqlText += " ,ImportId";
                sqlText += " ,TransferType";

                sqlText += " )";

                sqlText += " VALUES";
                sqlText += " (";
                sqlText += "  @TransferIssueNo";
                sqlText += "  ,@BranchId";
                sqlText += "  ,@TransferTo";
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
                sqlText += "  ,@BatchNo";
                sqlText += "  ,@BatchDate";
                sqlText += "  ,@TestReportNo";
                sqlText += "  ,@TestReportDate";
                sqlText += "  ,@TestReportTempPF";
                sqlText += "  ,@TestReportSPGR";
                sqlText += "  ,@DepartureDate";
                sqlText += "  ,@ImportId";
                sqlText += "  ,@TransferType";

                sqlText += ")  SELECT SCOPE_IDENTITY() ";

                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);

                cmdInsert.Parameters.AddWithValueAndNullHandle("@TransferIssueNo", Master.TransferIssueNo);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@BranchId", Master.BranchId);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@TransferTo", Master.TransferTo);
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
                cmdInsert.Parameters.AddWithValueAndNullHandle("@BatchNo", Master.BatchNo);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@BatchDate", Master.BatchDate);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@TestReportNo", Master.TestReportNo);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@TestReportDate", Master.TestReportDate);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@TestReportTempPF", Master.TestReportTempPF);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@TestReportSPGR", Master.TestReportSPGR);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@DepartureDate", Master.DepartureDate);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@ImportId", Master.ImportId);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@TransferType", Master.TransferType);


                transResult = Convert.ToInt32(cmdInsert.ExecuteScalar());
                retResults[4] = transResult.ToString();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgSaveNotSuccessfully);
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
                retResults[2] = "" + Master.TransferIssueNo;
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

                FileLogger.Log("SaleMPLDAL", "SalesMPLInsertToMaster", ex.ToString() + "\n" + sqlText);
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

        public string[] TransIssueMPLInsertToDetails(TransferMPLIssueDetailVM details, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null, DataTable settingsDt = null)
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
                sqlText += " INSERT INTO TransferMPLIssueDetails";
                sqlText += " (";
                sqlText += "TransferMPLIssueId ";
                sqlText += ",BranchId ";
                sqlText += ",IssueLineNo ";
                sqlText += ",TransferTo ";
                sqlText += ",TransferDateTime ";
                sqlText += ",ReceiveDateTime ";
                sqlText += ",Post ";
                sqlText += ",ItemNo ";
                sqlText += ",RequestedQuantity ";
                sqlText += ",RequestedVolumn ";
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
                sqlText += ",DIP ";
                sqlText += ",WagonNo ";

                sqlText += " )";

                sqlText += " VALUES";
                sqlText += " (";
                sqlText += " @TransferMPLIssueId";
                sqlText += " ,@BranchId";
                sqlText += " ,@IssueLineNo";
                sqlText += " ,@TransferTo";
                sqlText += " ,@TransferDateTime";
                sqlText += " ,@ReceiveDateTime";
                sqlText += " ,@Post";
                sqlText += " ,@ItemNo";
                sqlText += " ,@RequestedQuantity";
                sqlText += " ,@RequestedVolumn";
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
                sqlText += " ,@DIP";
                sqlText += " ,@WagonNo";


                sqlText += ")  SELECT SCOPE_IDENTITY() ";

                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);

                cmdInsert.Parameters.AddWithValueAndNullHandle("@TransferMPLIssueId", details.TransferMPLIssueId);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@BranchId", details.BranchId);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@IssueLineNo", details.IssueLineNo);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@TransferTo", details.TransferTo);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@TransferDateTime", details.TransferDateTime);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@ReceiveDateTime", details.ReceiveDateTime);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Post", details.Post);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@ItemNo", details.ItemNo);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@RequestedQuantity", details.RequestedQuantity);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@RequestedVolumn", details.RequestedVolumn);
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
                cmdInsert.Parameters.AddWithValueAndNullHandle("@DIP", details.DIP);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@WagonNo", details.WagonNo ?? Convert.DBNull);


                transResult = Convert.ToInt32(cmdInsert.ExecuteScalar());
                retResults[4] = transResult.ToString();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgSaveNotSuccessfully);
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
                retResults[2] = "" + details.TransferMPLIssueId;
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

                FileLogger.Log("SaleMPLDAL", "SalesMPLInsertToDetails", ex.ToString() + "\n" + sqlText);
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


        public string[] TransIssueMPLUpdate(TransferMPLIssueVM MasterVM, SqlTransaction transaction, SqlConnection currConn, SysDBInfoVMTemp connVM = null)
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

            TransferIssueVM TransferIssueMaster = new TransferIssueVM();
            List<TransferIssueDetailVM> Details = new List<TransferIssueDetailVM>();
            #endregion Initializ

            #region Try
            try
            {

                #region Validation for Header

                if (MasterVM == null)
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgNoDataToSave);
                }
                else if (Convert.ToDateTime(MasterVM.TransferDateTime) < DateTime.MinValue || Convert.ToDateTime(MasterVM.TransferDateTime) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, "Please Check Invoice Data and Time");

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

                MasterVM.TransferDateTime = OrdinaryVATDesktop.DateToDate(MasterVM.TransferDateTime);
                MasterVM.ReceiveDateTime = OrdinaryVATDesktop.DateToDate(MasterVM.ReceiveDateTime);
                MasterVM.RailwayReceiptDate = OrdinaryVATDesktop.DateToDate(MasterVM.RailwayReceiptDate);
                MasterVM.BatchDate = OrdinaryVATDesktop.DateToDate(MasterVM.BatchDate);
                MasterVM.DepartureDate = OrdinaryVATDesktop.DateToDate(MasterVM.DepartureDate);
                MasterVM.TestReportDate = OrdinaryVATDesktop.DateToDate(MasterVM.TestReportDate);

                retResults = TransIssueMPLUpdateToMaster(MasterVM, currConn, transaction, null, settings);

                Id = retResults[2];

                if (retResults[0] != "Success")
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, retResults[1]);
                }

                #endregion Update Information in Header

                #region Delete Existing Detail Data

                sqlText = "";
                sqlText += @" DELETE FROM TransferMPLIssueDetails WHERE TransferMPLIssueId=@MasterTransferMPLIssueId ";

                SqlCommand cmdDeleteDetail = new SqlCommand(sqlText, currConn, transaction);
                cmdDeleteDetail.Parameters.AddWithValueAndNullHandle("@MasterTransferMPLIssueId", MasterVM.Id);

                transResult = cmdDeleteDetail.ExecuteNonQuery();

                #endregion

                #region Insert into Details(Insert complete in Header)

                if (MasterVM.TransferMPLIssueDetailVMs != null)
                {
                    if (MasterVM.TransferMPLIssueDetailVMs.Count() < 0)
                    {
                        throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgNoDataToSave);
                    }
                    int IssueLineNo = 1;
                    foreach (TransferMPLIssueDetailVM collection in MasterVM.TransferMPLIssueDetailVMs)
                    {
                        #region TransferIssueDeatils

                        TransferIssueDetailVM transferDetails = new TransferIssueDetailVM();
                        transferDetails.TransferIssueNo = MasterVM.TransferIssueNo;
                        transferDetails.ItemNo = collection.ItemNo;
                        transferDetails.BranchId = MasterVM.BranchId;
                        transferDetails.TransferTo = MasterVM.TransferTo;
                        transferDetails.TransactionDateTime = MasterVM.TransferDateTime;
                        transferDetails.Post = "N";

                        transferDetails.UOMQty = collection.Quantity * collection.UOMc;
                        transferDetails.Quantity = collection.UOMQty;
                        transferDetails.CostPrice = collection.CostPrice;

                        transferDetails.UOMPrice = (collection.CostPrice * collection.UOMQty);
                        transferDetails.SubTotal = transferDetails.UOMPrice;

                        transferDetails.UOMn = collection.UOM;
                        transferDetails.UOM = transferDetails.UOMn;
                        transferDetails.UOMc = collection.UOMc;
                        transferDetails.IssueLineNo = IssueLineNo.ToString();

                        transferDetails.Weight = "";
                        transferDetails.OtherRef = "";
                        transferDetails.Comments = "";

                        transferDetails.TransactionType = MasterVM.TransactionType;

                        transferDetails.CreatedBy = MasterVM.CreatedBy;
                        transferDetails.CreatedOn = MasterVM.CreatedOn;
                        transferDetails.LastModifiedBy = MasterVM.LastModifiedBy;
                        transferDetails.LastModifiedOn = MasterVM.LastModifiedOn;

                        Details.Add(transferDetails);


                        #endregion

                        collection.TransferMPLIssueId = MasterVM.Id;
                        collection.ReportType = MasterVM.ReportType;
                        collection.IssueLineNo = IssueLineNo.ToString();

                        collection.IssueLineNo = IssueLineNo.ToString();
                        collection.BranchId = MasterVM.BranchId;
                        collection.TransferDateTime = MasterVM.TransferDateTime;
                        collection.ReceiveDateTime = MasterVM.ReceiveDateTime;
                        collection.Post = "N";

                        collection.UOMQty = collection.Quantity * collection.UOMc;
                        collection.UOMPrice = (collection.CostPrice * collection.UOMQty);
                        collection.UOMn = "LTR";

                        retResults = TransIssueMPLInsertToDetails(collection, currConn, transaction, null, settings);
                    }
                }

                if (retResults[0] != "Success")
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, retResults[1]);
                }

                #endregion Insert into Details(Insert complete in Header)


                #region TransferIssueInsert
                TransferIssueMaster.TransferIssueNo = MasterVM.TransferIssueNo;
                TransferIssueMaster.TransactionDateTime = MasterVM.TransferDateTime;
                TransferIssueMaster.TransferTo = MasterVM.TransferTo;
                TransferIssueMaster.ReferenceNo = MasterVM.ReferenceNo;
                TransferIssueMaster.BranchId = MasterVM.BranchId;
                TransferIssueMaster.VehicleNo = MasterVM.VehicleNo;
                TransferIssueMaster.VehicleType = MasterVM.VehicleType;
                TransferIssueMaster.TransactionType = MasterVM.TransactionType;
                TransferIssueMaster.Post = "N";
                TransferIssueMaster.CreatedBy = MasterVM.LastModifiedBy;
                TransferIssueMaster.CreatedOn = MasterVM.LastModifiedOn;
                TransferIssueMaster.LastModifiedBy = MasterVM.LastModifiedBy;
                TransferIssueMaster.LastModifiedOn = MasterVM.LastModifiedOn;
                retResults = _TransferIssueDAL.Update(TransferIssueMaster, Details, connVM, "", transaction, currConn);
                #endregion

                if (retResults[0] != "Success")
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, retResults[1]);
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
                retResults[1] = MessageVM.saleMsgUpdateSuccessfully;
                retResults[2] = "" + MasterVM.TransferIssueNo;
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
                FileLogger.Log("SaleDAL", "SalesInsert", ex.ToString() + "\n" + sqlText);

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


        public string[] TransIssueMPLUpdateToMaster(TransferMPLIssueVM Master, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null, DataTable settingsDt = null)
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
                sqlText += " update TransferMPLIssues SET  ";

                sqlText += " TransferIssueNo=@TransferIssueNo";
                sqlText += " ,BranchId=@BranchId";
                sqlText += " ,TransferTo=@TransferTo";
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
                sqlText += " ,BatchNo=@BatchNo";
                sqlText += " ,BatchDate=@BatchDate";
                sqlText += " ,TestReportNo=@TestReportNo";
                sqlText += " ,TestReportDate=@TestReportDate";
                sqlText += " ,TestReportTempPF=@TestReportTempPF";
                sqlText += " ,TestReportSPGR=@TestReportSPGR";
                sqlText += " ,DepartureDate=@DepartureDate";


                sqlText += " WHERE Id=@Id";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);

                cmdUpdate.Parameters.AddWithValueAndNullHandle("@TransferIssueNo", Master.TransferIssueNo);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@BranchId", Master.BranchId);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@TransferTo", Master.TransferTo);
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
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@BatchNo", Master.BatchNo);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@BatchDate", Master.BatchDate);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@TestReportNo", Master.TestReportNo);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@TestReportDate", Master.TestReportDate);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@TestReportTempPF", Master.TestReportTempPF);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@TestReportSPGR", Master.TestReportSPGR);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@DepartureDate", Master.DepartureDate);


                cmdUpdate.Parameters.AddWithValueAndNullHandle("@Id", Master.Id);

                transResult = cmdUpdate.ExecuteNonQuery();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.PurchasemsgUpdateNotSuccessfully, MessageVM.PurchasemsgUpdateNotSuccessfully);
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
                FileLogger.Log("SaleDAL", "SalesUpdateToMaster", ex.ToString() + "\n" + sqlText);
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

TI.Id,
TI.TransferIssueNo,
TI.BranchId,
ISNULL(TI.TransferTo, '0') TransferTo,
ISNULL(B.BranchName, '0') TransferToBranch,
TI.TransferTRCode,
TI.ReceiveTRCode,
ISNULL(TI.TransferDateTime, '') TransferDateTime,
ISNULL(TI.ReceiveDateTime, '') ReceiveDateTime,
ISNULL(TI.RailwayReceiptDate, '') RailwayReceiptDate,
TI.TotalAmount,
TI.TransactionType,
TI.ReportType,
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
LEFT OUTER JOIN BranchProfiles B ON B.BranchID = TI.TransferTo
WHERE  1=1 ";
                #endregion SqlText

                sqlTextCount += @" 
SELECT count(TI.TransferIssueNo) RecordCount
FROM TransferMPLIssues TI WHERE  1=1 ";

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

                if (Orderby == "N")
                {
                    //sqlTextOrderBy += " order by TI.TransferIssueNo desc, TI.TransferDateTime desc";
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


        public List<TransferMPLIssueVM> SelectAllList(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null, string[] ids = null, string transactionType = null, string Orderby = "Y", string SelectTop = "100")
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

                DataTable dt = SelectAll(Id, conditionFields, conditionValues, VcurrConn, Vtransaction, connVM, ids, transactionType, Orderby, SelectTop);
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

                        vm.BatchNo = dr["BatchNo"].ToString();
                        vm.TransferType = dr["TransferType"].ToString();
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


        public List<TransferMPLIssueDetailVM> SearchTransIssueMPLDetailList(string transferMPLIssueId, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            List<TransferMPLIssueDetailVM> lst = new List<TransferMPLIssueDetailVM>();
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
,TD.TransferMPLIssueId
,TD.BranchId
,TD.IssueLineNo
,ISNULL(TD.TransferTo,0) TransferTo
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
,TD.UOMQty Volumn
,TD.UOMPrice
,TD.UOMc
,TD.UOMn

,ISNULL(TD.VATRate,0) VATRate
,ISNULL(TD.VATAmount,0) VATAmount
,ISNULL(TD.SDRate,0) SDRate
,ISNULL(TD.SDAmount,0) SDAmount
,TD.PeriodID
,ISNULL(TD.TankId,0) TankId
,ISNULL(TD.Temperature,0) Temperature
,ISNULL(TD.SP_Gravity,0) SP_Gravity
,ISNULL(TD.QtyAt30Temperature,0) QtyAt30Temperature
,ISNULL(TD.DIP,0) DIP
,TD.ReportType
,ISNULL(TD.WagonNo,'') WagonNo
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

                lst = dataTable.ToList<TransferMPLIssueDetailVM>();
            }
            #endregion

            #region Catch & Finally

            catch (SqlException sqlex)
            {
                FileLogger.Log("SaleMPLDAL", "SearchSaleMPLDetailList", sqlex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                FileLogger.Log("SaleMPLDAL", "SearchSaleMPLDetailList", ex.ToString() + "\n" + sqlText);
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

        public List<TransferMPLIssueVM> DropDown(SysDBInfoVMTemp connVM = null)
        {
            throw new NotImplementedException();
        }

        public string[] Delete(TransferMPLIssueVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            throw new NotImplementedException();
        }



        public string[] TransferMPLIssuePost(TransferMPLIssueVM MasterVM, SqlTransaction transaction, SqlConnection currConn, SysDBInfoVMTemp connVM = null)
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
                        ids += "," + MasterVM.IDs[i];
                    }
                }


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

                #region Update Transfer Issue Data

                sqlText = "";
                sqlText +=
                 @" UPDATE TransferMPLIssues SET Post='Y',LastModifiedBy=@LastModifiedBy,LastModifiedOn=GETDATE() ";
                sqlText += @"  WHERE Id IN (" + ids + ") ";
                sqlText +=
                    @" UPDATE TransferMPLIssueDetails SET Post='Y'  ";
                sqlText += @"  WHERE TransferMPLIssueId IN (" + ids + ") ";

                SqlCommand cmdDeleteDetail = new SqlCommand(sqlText, currConn, transaction);
                cmdDeleteDetail.Parameters.AddWithValueAndNullHandle("@LastModifiedBy", MasterVM.LastModifiedBy);

                transResult = cmdDeleteDetail.ExecuteNonQuery();
                if (transResult > 0)
                {
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
                            ids = MasterVM.IDs[i];
                        }
                    TransferMPLIssueVM vm= SelectAllList(Convert.ToInt32(ids),null,null,currConn,transaction,connVM,null,null).FirstOrDefault();
                    if (vm != null)
                    {
                        TransferIssueVM TransferIssue = new TransferIssueVM();
                        TransferIssue.TransferIssueNo = vm.TransferIssueNo;
                        TransferIssue.LastModifiedOn = MasterVM.LastModifiedOn;
                        TransferIssue.LastModifiedBy = MasterVM.LastModifiedBy;
                        TransferIssue.TransactionDateTime = vm.TransferDateTime;
                       
                        TransferIssue.IsTransfer = "Y";
                        TransferIssue.SignatoryName ="";
                        TransferIssue.SignatoryDesig = "";
                        List<TransferIssueDetailVM> Vms = new TransferIssueDAL().SelectDetail(TransferIssue.TransferIssueNo, null, null, currConn, transaction, connVM);

                        retResults = _TransferIssueDAL.Post(TransferIssue, Vms, currConn, transaction, connVM);

                    }

                    }
                }
               
                #endregion
                if (retResults[0] != "Success")
                {
                    throw new ArgumentNullException(MessageVM.saleMsgPostNotSuccessfully, retResults[1]);
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
                retResults[2] = "" + MasterVM.TransferIssueNo;
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
                FileLogger.Log("TransferIssueMPLDAL", "TransferMPLIssuePost", ex.ToString() + "\n" + sqlText);

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



    }
}
