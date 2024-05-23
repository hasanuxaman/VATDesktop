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
using Newtonsoft.Json;
using Excel;
using OfficeOpenXml;
using System.IO;
using OfficeOpenXml.Style;
using System.Globalization;
namespace VATServer.Library
{
    public class TransferIssueDAL : ITransferIssue
    {
        #region Global Variables
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        private const string FieldDelimeter = DBConstant.FieldDelimeter;

        ProductDAL _ProductDAL = new ProductDAL();

        #endregion

        #region Navigation

        public NavigationVM TransferIssue_Navigation(NavigationVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {

            #region Variables

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            string sqlText = "";

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

                #region Check Point

                if (vm.FiscalYear == 0)
                {
                    DateTime now = DateTime.Now;
                    string startDate = new DateTime(now.Year, now.Month, 1).ToString("yyyy-MMM-dd");
                    FiscalYearVM varFiscalYearVM = new FiscalYearDAL().SelectAll(0, new[] { "PeriodStart" }, new[] { startDate }, currConn, transaction, connVM).FirstOrDefault();
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
------declare @Id as int = 14393;
------declare @FiscalYear as int = 2021;
------declare @TransactionType as varchar(50) = 'Other';
------declare @BranchId as int = 1;

";
                if (vm.ButtonName == "Current")
                {
                    #region Current Item

                    sqlText = sqlText + @"
--------------------------------------------------Current--------------------------------------------------
------------------------------------------------------------------------------------------------------------
select top 1 inv.Id, inv.TransferIssueNo InvoiceNo from TransferIssues inv
where 1=1 
and inv.TransferIssueNo=@InvoiceNo

";
                    #endregion
                }
                else if (vm.Id == 0 || vm.ButtonName == "First")
                {

                    #region First Item

                    sqlText = sqlText + @"
--------------------------------------------------First--------------------------------------------------
---------------------------------------------------------------------------------------------------------
select top 1 inv.Id, inv.TransferIssueNo InvoiceNo from TransferIssues inv
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
select top 1 inv.Id, inv.TransferIssueNo InvoiceNo from TransferIssues inv
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
select top 1 inv.Id, inv.TransferIssueNo InvoiceNo from TransferIssues inv
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
select top 1 inv.Id, inv.TransferIssueNo InvoiceNo from TransferIssues inv
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
                        vm = TransferIssue_Navigation(vm, currConn, transaction);

                    }
                    else if (vm.ButtonName == "Next")
                    {
                        vm.ButtonName = "Last";
                        vm = TransferIssue_Navigation(vm, currConn, transaction);

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

            #region catch

            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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

            return vm;

        }


        #endregion

        #region Basic Methods

        //currConn to VcurrConn 25-Aug-2020
        public string[] Insert(TransferIssueVM Master, List<TransferIssueDetailVM> Details, SqlTransaction Vtransaction, SqlConnection VcurrConn, SysDBInfoVMTemp connVM = null, List<TrackingVM> Trackings = null, bool CodeGenaration = true)
        {
            #region Initializ

            CommonDAL commonDAL = new CommonDAL();

            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";
            string Id = "";

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

            int transResult = 0;
            string sqlText = "";
            string newID = "";
            string PostStatus = "";
            int IDExist = 0;
            string vehicleId = "0";

            #endregion Initializ

            #region Try
            try
            {

                #region Find Month Lock

                string PeriodName = Convert.ToDateTime(Master.TransactionDateTime).ToString("MMMM-yyyy");
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
                if (Master == null)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, MessageVM.issueMsgNoDataToSave);
                }
                else if (Convert.ToDateTime(Master.TransactionDateTime) < DateTime.MinValue || Convert.ToDateTime(Master.TransactionDateTime) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, "Please Check Issue Data and Time");
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

                string multiple = commonDAL.settings("TransferIssue", "MultipleProduct", currConn, transaction, connVM);
                string CompanyCode = commonDAL.settings("CompanyCode", "Code", currConn, transaction, connVM);

                #region Fiscal Year Check

                string transactionDate = Master.TransactionDateTime;

                string transactionYearCheck = Convert.ToDateTime(Master.TransactionDateTime).ToString("yyyy-MM-dd");
                if (Convert.ToDateTime(transactionYearCheck) > DateTime.MinValue || Convert.ToDateTime(transactionYearCheck) < DateTime.MaxValue)
                {

                    #region YearLock
                    sqlText = "";
                    sqlText += "select distinct isnull(PeriodLock,'Y') MLock,isnull(GLLock,'Y')YLock from fiscalyear " +
                                                   " where @transactionYearCheck between PeriodStart and PeriodEnd";


                                                    //" where '" + transactionYearCheck + "' between PeriodStart and PeriodEnd";

                    DataTable dataTable = new DataTable("ProductDataT");
                    SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn,transaction);

                    cmdIdExist.Parameters.AddWithValueAndNullHandle("@transactionYearCheck", transactionYearCheck);

                    ////BugsBD
                    //SqlParameter parameter4 = new SqlParameter("@transactionYearCheck", SqlDbType.VarChar, 250);
                    //parameter4.Value = transactionYearCheck;
                    //cmdIdExist.Parameters.Add(parameter4);

                   
                   

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

                #region Product Type check

                if (CompanyCode.ToLower() != "mbl" && CompanyCode.ToLower() != "MBLShirirchala".ToLower() && CompanyCode.ToLower() != "MBLMouchak".ToLower())
                {
                    foreach (TransferIssueDetailVM Items in Details.ToList())
                    {

                        ProductDAL _ProductDAL = new ProductDAL();

                        ProductVM ProductVM = _ProductDAL.SelectAll(Items.ItemNo, null, null, currConn, transaction, null, connVM).FirstOrDefault();
                        string ProductType = ProductVM.IsRaw;

                        if (ProductType.ToLower() != "finish" && ProductType.ToLower() != "trading" && Master.TransactionType.ToLower() == "62out")
                        {
                            throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, "This Product: " + ProductVM.ProductName + "( " + ProductVM.ProductCode + " ) " + " Type: " + ProductType + " is not valid for FG(Out)");
                        }

                        if (Master.TransactionType.ToLower() == "61out")
                        {
                            if (ProductType.ToLower() != "raw" && ProductType.ToLower() != "wip" && ProductType.ToLower() != "pack")
                            {
                                throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, "This Product: " + ProductVM.ProductName + "( " + ProductVM.ProductCode + " ) " + " Type: " + ProductType + " is not valid for RM(Out)");

                            }

                        }

                    }

                }

                #endregion Product Type check

                #region Vehicle

                if (string.IsNullOrWhiteSpace(Master.VehicleNo)
                    )
                {
                    vehicleId = "0";
                }
                else
                {

                    if (string.IsNullOrEmpty(Master.VehicleID) || Convert.ToDecimal(Master.VehicleID) <= 0)
                    {
                        string vehicleID = "0";
                        sqlText = "";
                        sqlText = sqlText + "select VehicleID from Vehicles WHERE VehicleNo=@MasterVehicleNo and VehicleType=@VehicleType";

                        SqlCommand cmdExistVehicleId = new SqlCommand(sqlText, currConn);
                        cmdExistVehicleId.Transaction = transaction;
                        cmdExistVehicleId.Parameters.AddWithValue("@MasterVehicleNo", Master.VehicleNo);
                        cmdExistVehicleId.Parameters.AddWithValue("@VehicleType", Master.VehicleType);
                        string vehicleIDExist = Convert.ToString(cmdExistVehicleId.ExecuteScalar());
                        vehicleId = vehicleIDExist;


                        if (string.IsNullOrEmpty(vehicleId) || vehicleId == null || Convert.ToDecimal(vehicleId) <= 0)
                        {

                            sqlText = "";
                            sqlText = sqlText + "select isnull(max(cast(VehicleID as int)),0)+1 FROM  Vehicles ";
                            SqlCommand cmdVehicleId = new SqlCommand(sqlText, currConn);
                            cmdVehicleId.Transaction = transaction;
                            int NewvehicleID = (int)cmdVehicleId.ExecuteScalar();
                            vehicleID = NewvehicleID.ToString();


                            sqlText = "";
                            sqlText +=
                                " INSERT INTO Vehicles (VehicleID,	VehicleType,	VehicleNo,	Description,	Comments,	ActiveStatus,CreatedBy,	CreatedOn,	LastModifiedBy,	LastModifiedOn)";
                            sqlText += "values(@VehicleID";
                            sqlText += " ,@MasterVehicleType ";
                            sqlText += " ,@MasterVehicleNo ";
                            sqlText += " ,'NA'";
                            sqlText += " ,'NA'";
                            if (Master.vehicleSaveInDB == true)
                            {
                                sqlText += ",'Y'";

                            }
                            else
                            {
                                sqlText += ",'N'";
                            }

                            sqlText += " ,@MasterCreatedBy ";
                            sqlText += " ,@MasterCreatedOn ";
                            sqlText += " ,@MasterLastModifiedBy ";
                            sqlText += " ,@MasterLastModifiedOn)";
                            //sqlText += " from Vehicles;";

                            SqlCommand cmdExistVehicleIns = new SqlCommand(sqlText, currConn);
                            cmdExistVehicleIns.Transaction = transaction;
                            cmdExistVehicleIns.Parameters.AddWithValue("@VehicleID", vehicleID);
                            cmdExistVehicleIns.Parameters.AddWithValue("@MasterVehicleType", Master.VehicleType ?? "-");
                            cmdExistVehicleIns.Parameters.AddWithValue("@MasterVehicleNo", Master.VehicleNo);
                            cmdExistVehicleIns.Parameters.AddWithValue("@MasterCreatedBy", Master.CreatedBy);
                            cmdExistVehicleIns.Parameters.AddWithValue("@MasterCreatedOn", OrdinaryVATDesktop.DateToDate(Master.CreatedOn));
                            cmdExistVehicleIns.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy);
                            cmdExistVehicleIns.Parameters.AddWithValue("@MasterLastModifiedOn", OrdinaryVATDesktop.DateToDate(Master.LastModifiedOn));
                            transResult = (int)cmdExistVehicleIns.ExecuteNonQuery();
                            if (transResult <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert,
                                    MessageVM.saleMsgUnableCreatID);
                            }
                            vehicleId = vehicleID.ToString();
                            if (string.IsNullOrEmpty(vehicleId))
                            {
                                throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgUnableCreatID);
                            }

                        }

                        Master.VehicleID = vehicleId;

                    }

                }

                #endregion Vehicle

                CommonDAL commonDal = new CommonDAL();

                #region Invoice ID Create
                if (CodeGenaration)
                {
                    if (string.IsNullOrEmpty(Master.TransactionType)) //start
                    {
                        throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, MessageVM.msgTransactionNotDeclared);
                    }

                    #region TransferIssues ID Create For Other
                    if (Master.TransactionType == "61Out" || Master.TransactionType.ToLower() == "raw")
                    {
                        newID = commonDal.TransactionCode("Transfer", "61Out", "TransferIssues", "TransferIssueNo",
                                                  "TransactionDateTime", Master.TransactionDateTime, Master.BranchId.ToString(), currConn, transaction, connVM);
                    }
                    if (Master.TransactionType == "62Out" || Master.TransactionType.ToLower() == "finish" || Master.TransactionType.ToLower() == "finished" || Master.TransactionType.ToLower() == "trading")
                    {
                        newID = commonDal.TransactionCode("Transfer", "62Out", "TransferIssues", "TransferIssueNo",
                                                  "TransactionDateTime", Master.TransactionDateTime, Master.BranchId.ToString(), currConn, transaction, connVM);
                    }

                    if (Master.TransactionType.ToLower() == "raw")
                    {

                        Master.TransactionType = "61Out";
                    }

                    if (Master.TransactionType.ToLower() == "finish" || Master.TransactionType.ToLower() == "finished" || Master.TransactionType.ToLower() == "trading")
                    {

                        Master.TransactionType = "62Out";
                    }
                    Master.TransferIssueNo = newID;

                }
                    #endregion TransferIssues ID Create For Other

                #endregion

                #region ID generated completed,Insert new Information in Header


                #region Find Transaction Exist
                sqlText = "";

                //sqlText = sqlText + "select COUNT(TransferIssueNo) from TransferIssues WHERE TransferIssueNo=@TransferIssueNo ";

                //sqlText = sqlText + "select COUNT(TransferIssueNo) from TransferIssues WHERE TransferIssueNo='" + Master.TransferIssueNo + "' ";
                sqlText = sqlText + "select COUNT(TransferIssueNo) from TransferIssues WHERE TransferIssueNo = @TransferIssueNo";

                SqlCommand cmdExistTran = new SqlCommand(sqlText, currConn,transaction);
                cmdExistTran.Parameters.AddWithValueAndNullHandle("@TransferIssueNo", Master.TransferIssueNo);

                ////BugsBD
                //SqlParameter parameter = new SqlParameter("@TransferIssueNo", SqlDbType.VarChar, 250);
                //parameter.Value = Master.TransferIssueNo;
                //cmdExistTran.Parameters.Add(parameter);

                //cmdExistTran.Transaction = transaction;

                IDExist = (int)cmdExistTran.ExecuteScalar();
                if (IDExist > 0)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert,
                        MessageVM.issueMsgFindExistID + "-" + Master.TransferIssueNo);
                }
                #endregion Find Transaction Exist

                retResults = TransferIssueInsertToMaster(Master, currConn, transaction, connVM);

                Id = retResults[4];
                if (retResults[0] != "Success")
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, MessageVM.issueMsgSaveNotSuccessfully);
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

                foreach (TransferIssueDetailVM Item in Details.ToList())
                {
                    #region Find Transaction Exist

                    if (multiple == "N")
                    {
                        sqlText = "";

                        //sqlText += "select COUNT(TransferIssueNo) from TransferIssueDetails WHERE TransferIssueNo= @TransferIssueNo ";
                        //sqlText += " AND ItemNo='" + Item.ItemNo + "'";
                        //sqlText += "select COUNT(TransferIssueNo) from TransferIssueDetails WHERE TransferIssueNo='" + Master.TransferIssueNo + "' ";
                        //sqlText += " AND ItemNo='" + Item.ItemNo + "'";

                        sqlText += "select COUNT(TransferIssueNo) from TransferIssueDetails WHERE TransferIssueNo = @TransferIssueNo";
                        sqlText += " AND ItemNo = @ItemNo";

                        SqlCommand cmdFindId = new SqlCommand(sqlText, currConn, transaction);
                        cmdFindId.Parameters.AddWithValueAndNullHandle("@TransferIssueNo", Master.TransferIssueNo);
                        cmdFindId.Parameters.AddWithValueAndNullHandle("@ItemNo", Item.ItemNo);



                        //BugsBD
                        ////SqlParameter parameter2 = new SqlParameter("@TransferIssueNo", SqlDbType.VarChar, 250);
                        ////parameter2.Value = Master.TransferIssueNo;
                        ////cmdFindId.Parameters.Add(parameter2);

                        //parameter2 = new SqlParameter("@ItemNo", SqlDbType.VarChar, 250);
                        //parameter2.Value = Item.ItemNo;
                        //cmdFindId.Parameters.Add(parameter2);



                        IDExist = (int)cmdFindId.ExecuteScalar();
                        if (IDExist > 0)
                        {
                            throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, MessageVM.issueMsgFindExistID + "-" + Master.TransferIssueNo);
                        }
                    }

                    #endregion Find Transaction Exist

                    #region Insert only DetailTable
                    Item.TransferIssueNo = Master.TransferIssueNo;
                    Item.TransactionType = Master.TransactionType;
                    Item.TransactionDateTime = Master.TransactionDateTime;
                    Item.TransferTo = Master.TransferTo;
                    Item.Post = Master.Post;
                    Item.CreatedBy = Master.CreatedBy;
                    Item.CreatedOn = Master.CreatedOn;
                    Item.LastModifiedBy = Master.LastModifiedBy;
                    Item.LastModifiedOn = Master.LastModifiedOn;

                    retResults = TransferIssueInsertToDetail(Item, currConn, transaction, connVM);

                    if (retResults[0] != "Success")
                    {
                        throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, MessageVM.issueMsgSaveNotSuccessfully);
                    }

                    #endregion Insert only DetailTable
                }

                #endregion Insert Detail Table

                #region Tracking

                List<TransferIssueTrackingVM> TransferVms = new List<TransferIssueTrackingVM>();

                if (Trackings != null && Trackings.Count > 0)
                {
                    for (int i = 0; i < Trackings.Count; i++)
                    {

                        if (Trackings[i].IsSale == "Y")
                        {
                            //Trackings[i].SaleInvoiceNo = MasterVM.SalesInvoiceNo;
                            Trackings[i].TransferIssueNo = Master.TransferIssueNo;

                            TransferIssueTrackingVM transferIssueTracking = new TransferIssueTrackingVM();
                            transferIssueTracking.TransferIssueNo = Trackings[i].TransferIssueNo;
                            transferIssueTracking.ItemNo = Trackings[i].ItemNo;
                            transferIssueTracking.TrackingLineNo = Trackings[i].TrackingLineNo;
                            transferIssueTracking.Heading1 = Trackings[i].Heading1;
                            transferIssueTracking.Heading2 = Trackings[i].Heading2;
                            transferIssueTracking.Quantity = Trackings[i].Quantity;
                            transferIssueTracking.Post = Trackings[i].Post;
                            //transferIssueTracking.IsTransfer = Trackings[i].IsSale;
                            transferIssueTracking.FinishItemNo = Trackings[i].FinishItemNo;

                            TransferVms.Add(transferIssueTracking);


                        }

                    }


                    string trackingInsert = string.Empty;
                    TrackingDAL trackingDal = new TrackingDAL();
                    trackingInsert = trackingDal.TransferIssueTrackingInsert(TransferVms, Master, transaction, currConn, connVM);

                    if (trackingInsert == "Fail")
                    {
                        throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert, "Tracking Information not added.");
                    }
                }
                #endregion

                #region Update PeriodId and Fiscal Year

                sqlText = "";
                sqlText += @"

update  TransferIssues                             
set PeriodId=RIGHT('0'+CONVERT(VARCHAR(2),MONTH(TransactionDateTime)) +  CONVERT(VARCHAR(4),YEAR(TransactionDateTime)),6)
WHERE TransferIssueNo = @TransferIssueNo


update  TransferIssueDetails                             
set PeriodId=RIGHT('0'+CONVERT(VARCHAR(2),MONTH(TransactionDateTime)) +  CONVERT(VARCHAR(4),YEAR(TransactionDateTime)),6)
WHERE TransferIssueNo = @TransferIssueNo


update TransferIssues set FiscalYear=fyr.CurrentYear
From TransferIssues inv
left outer join  FiscalYear fyr on inv.PeriodID=fyr.PeriodID

";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                cmdUpdate.Parameters.AddWithValue("@TransferIssueNo", Master.TransferIssueNo);

                transResult = cmdUpdate.ExecuteNonQuery();
                #endregion

                #endregion Insert into Details(Insert complete in Header)

                #region return Current ID and Post Status
                sqlText = "";

                //sqlText = sqlText + "select distinct  Post from dbo.TransferIssues WHERE TransferIssueNo= @TransferIssueNo ";

                //sqlText = sqlText + "select distinct  Post from dbo.TransferIssues WHERE TransferIssueNo='" + Master.TransferIssueNo + "'";
                sqlText = sqlText + "select distinct  Post from dbo.TransferIssues WHERE TransferIssueNo = @TransferIssueNo";

                SqlCommand cmdIPS = new SqlCommand(sqlText, currConn, transaction);
                cmdIPS.Parameters.AddWithValueAndNullHandle("@TransferIssueNo", Master.TransferIssueNo);

                //BugsBD
                //SqlParameter parameter3 = new SqlParameter("@TransferIssueNo", SqlDbType.VarChar, 250);
                //parameter3.Value = Master.TransferIssueNo;
                //cmdIPS.Parameters.Add(parameter3);

                //cmdIPS.Transaction = transaction;

                PostStatus = (string)cmdIPS.ExecuteScalar();
                if (string.IsNullOrEmpty(PostStatus))
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, MessageVM.issueMsgUnableCreatID);
                }
                #endregion Prefetch

                #region Master Setting Update

                commonDal.UpdateProcessFlag(Master.TransferIssueNo, Master.TransactionDateTime, currConn, transaction, connVM);

                #endregion

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
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
            catch (Exception ex)
            {
                retResults = new string[5];
                retResults[0] = "Fail";
                retResults[1] = ex.Message;
                retResults[2] = "0";
                retResults[3] = "N";
                retResults[4] = "0";

                FileLogger.Log("TransferIssueDAL", "Insert", ex.ToString());

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Rollback();
                }
                //////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                throw new ArgumentNullException("", ex.Message.ToString());
                //throw ex;
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

        public string[] Update(TransferIssueVM Master, List<TransferIssueDetailVM> Details, SysDBInfoVMTemp connVM = null, string UserId = "", SqlTransaction Vtransaction = null, SqlConnection VcurrConn = null)
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
            string vehicleId = "0";

            #endregion Initializ

            #region Try
            try
            {
                CommonDAL commonDal = new CommonDAL();
                #region Find Month Lock

                string PeriodName = Convert.ToDateTime(Master.TransactionDateTime).ToString("MMMM-yyyy");
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
                if (Master == null)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, MessageVM.issueMsgNoDataToUpdate);
                }
                else if (Convert.ToDateTime(Master.TransactionDateTime) < DateTime.MinValue || Convert.ToDateTime(Master.TransactionDateTime) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, "Please Check Invoice Data and Time");
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
----------declare @InvoiceNo as varchar(100)='T1O-2/0001/1220'

select TransferIssueNo, Post from TransferIssues
where 1=1 
and TransferIssueNo=@InvoiceNo

";
                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
                cmd.Parameters.AddWithValue("@InvoiceNo", Master.TransferIssueNo);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt != null && dt.Rows.Count > 0)
                {
                    currentPostStatus = dt.Rows[0]["Post"].ToString();
                }

                #endregion

                #region Current Items
                DataTable dtCurrentItems = new DataTable();

                if (currentPostStatus == "Y")
                {
                    sqlText = "";
                    sqlText = @"
----------declare @InvoiceNo as varchar(100)='T1O-2/0001/1220'


select ItemNo, TransferIssueNo from TransferIssueDetails
where 1=1 
and TransferIssueNo=@InvoiceNo

";

                    cmd = new SqlCommand(sqlText, currConn, transaction);
                    cmd.Parameters.AddWithValue("@InvoiceNo", Master.TransferIssueNo);

                    da = new SqlDataAdapter(cmd);
                    da.Fill(dtCurrentItems);

                }
                #endregion

                #endregion

                #region Fiscal Year Check
                string transactionDate = Master.TransactionDateTime;
                string transactionYearCheck = Convert.ToDateTime(Master.TransactionDateTime).ToString("yyyy-MM-dd");
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

                List<TransferIssueDetailVM> previousDetailVMs = SelectDetail(Master.TransferIssueNo, null, null, currConn, transaction, connVM);

                TransferIssueVM prevInvoice = SelectAllList(0, new[] { "ti.TransferIssueNo" },
                    new[] { Master.TransferIssueNo }, currConn, transaction, connVM).FirstOrDefault();


                #region Master Setting Update

                commonDal.UpdateProcessFlag(Master.TransferIssueNo, prevInvoice.TransactionDateTime, currConn, transaction, connVM);

                #endregion

                #region Product Type check

                foreach (TransferIssueDetailVM Items in Details.ToList())
                {

                    ProductDAL _ProductDAL = new ProductDAL();

                    ProductVM ProductVM = _ProductDAL.SelectAll(Items.ItemNo, null, null, currConn, transaction, null, connVM).FirstOrDefault();
                    string ProductType = ProductVM.IsRaw;

                    if (ProductType.ToLower() != "finish" && ProductType.ToLower() != "trading" && Master.TransactionType.ToLower() == "62out")
                    {
                        throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, "This Product: " + ProductVM.ProductName + "( " + ProductVM.ProductCode + " ) " + " Type: " + ProductType + " is not valid for FG(Out)");
                    }

                    if (Master.TransactionType.ToLower() == "61out")
                    {
                        if (ProductType.ToLower() != "raw" && ProductType.ToLower() != "wip" && ProductType.ToLower() != "pack")
                        {
                            throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, "This Product: " + ProductVM.ProductName + "( " + ProductVM.ProductCode + " ) " + " Type: " + ProductType + " is not valid for RM(Out)");

                        }

                    }

                }

                #endregion Product Type check

                #region Vehicle

                if (string.IsNullOrWhiteSpace(Master.VehicleNo)
                    )
                {
                    vehicleId = "0";
                }
                else
                {

                    if (string.IsNullOrEmpty(Master.VehicleID) || Convert.ToDecimal(Master.VehicleID) <= 0)
                    {
                        string vehicleID = "0";
                        sqlText = "";
                        sqlText = sqlText + "select VehicleID from Vehicles WHERE VehicleNo=@MasterVehicleNo ";

                        SqlCommand cmdExistVehicleId = new SqlCommand(sqlText, currConn);
                        cmdExistVehicleId.Transaction = transaction;
                        cmdExistVehicleId.Parameters.AddWithValue("@MasterVehicleNo", Master.VehicleNo);
                        string vehicleIDExist = Convert.ToString(cmdExistVehicleId.ExecuteScalar());
                        vehicleId = vehicleIDExist;

                        if (string.IsNullOrEmpty(vehicleId) || vehicleId == null || Convert.ToDecimal(vehicleId) <= 0)
                        {

                            sqlText = "";
                            sqlText = sqlText + "select isnull(max(cast(VehicleID as int)),0)+1 FROM  Vehicles ";
                            SqlCommand cmdVehicleId = new SqlCommand(sqlText, currConn);
                            cmdVehicleId.Transaction = transaction;
                            int NewvehicleID = (int)cmdVehicleId.ExecuteScalar();
                            vehicleID = NewvehicleID.ToString();

                            sqlText = "";
                            sqlText +=
                                " INSERT INTO Vehicles (VehicleID,	VehicleType,	VehicleNo,	Description,	Comments,	ActiveStatus,CreatedBy,	CreatedOn,	LastModifiedBy,	LastModifiedOn)";
                            sqlText += "values(@VehicleID";
                            sqlText += " ,@MasterVehicleType ";
                            sqlText += " ,@MasterVehicleNo ";
                            sqlText += " ,'NA'";
                            sqlText += " ,'NA'";
                            if (Master.vehicleSaveInDB == true)
                            {
                                sqlText += ",'Y'";
                            }
                            else
                            {
                                sqlText += ",'N'";
                            }

                            sqlText += " ,@MasterCreatedBy ";
                            sqlText += " ,@MasterCreatedOn ";
                            sqlText += " ,@MasterLastModifiedBy ";
                            sqlText += " ,@MasterLastModifiedOn)";
                            //sqlText += " from Vehicles;";

                            SqlCommand cmdExistVehicleIns = new SqlCommand(sqlText, currConn);
                            cmdExistVehicleIns.Transaction = transaction;
                            cmdExistVehicleIns.Parameters.AddWithValue("@VehicleID", vehicleID);
                            cmdExistVehicleIns.Parameters.AddWithValue("@MasterVehicleType", Master.VehicleType ?? "-");
                            cmdExistVehicleIns.Parameters.AddWithValue("@MasterVehicleNo", Master.VehicleNo);
                            cmdExistVehicleIns.Parameters.AddWithValue("@MasterCreatedBy", Master.CreatedBy);
                            cmdExistVehicleIns.Parameters.AddWithValue("@MasterCreatedOn", OrdinaryVATDesktop.DateToDate(Master.CreatedOn));
                            cmdExistVehicleIns.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy);
                            cmdExistVehicleIns.Parameters.AddWithValue("@MasterLastModifiedOn", OrdinaryVATDesktop.DateToDate(Master.LastModifiedOn));
                            transResult = (int)cmdExistVehicleIns.ExecuteNonQuery();
                            if (transResult <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert,
                                    MessageVM.saleMsgUnableCreatID);
                            }
                            vehicleId = vehicleID.ToString();
                            if (string.IsNullOrEmpty(vehicleId))
                            {
                                throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgUnableCreatID);
                            }

                        }

                        Master.VehicleID = vehicleId;

                    }

                }

                #endregion Vehicle

                #region Find ID for Update
                sqlText = "";
                sqlText = sqlText + "select COUNT(TransferIssueNo) from TransferIssues WHERE TransferIssueNo='" + Master.TransferIssueNo + "' ";
                SqlCommand cmdFindIdUpd = new SqlCommand(sqlText, currConn);
                cmdFindIdUpd.Transaction = transaction;
                int IDExist = (int)cmdFindIdUpd.ExecuteScalar();
                if (IDExist <= 0)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, MessageVM.issueMsgUnableFindExistID);
                }
                #endregion Find ID for Update

                #region ID check completed,update Information in Header

                #region update Header

                retResults = TransferIssueUpdateToMaster(Master, currConn, transaction, connVM);

                if (retResults[0] != "Success")
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, MessageVM.issueMsgUpdateNotSuccessfully);
                }

                #region Update PeriodId and Fiscal Year

                sqlText = "";
                sqlText += @"

update  TransferIssues                             
set PeriodId=RIGHT('0'+CONVERT(VARCHAR(2),MONTH(TransactionDateTime)) +  CONVERT(VARCHAR(4),YEAR(TransactionDateTime)),6)
WHERE TransferIssueNo = @TransferIssueNo


update  TransferIssueDetails                             
set PeriodId=RIGHT('0'+CONVERT(VARCHAR(2),MONTH(TransactionDateTime)) +  CONVERT(VARCHAR(4),YEAR(TransactionDateTime)),6)
WHERE TransferIssueNo = @TransferIssueNo


update TransferIssues set FiscalYear=fyr.CurrentYear
From TransferIssues inv
left outer join  FiscalYear fyr on inv.PeriodID=fyr.PeriodID

";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                cmdUpdate.Parameters.AddWithValue("@TransferIssueNo", Master.TransferIssueNo);

                transResult = cmdUpdate.ExecuteNonQuery();
                #endregion

                #endregion update Header

                #endregion ID check completed,update Information in Header

                #region Update into Details(Update complete in Header)

                #region Validation for Detail

                if (Details.Count() < 0)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, MessageVM.issueMsgNoDataToUpdate);
                }

                #endregion Validation for Detail

                #region Update Detail Table

                // Insert
                #region Insert only DetailTable

                #region Delete Details

                sqlText = "";
                sqlText += " delete FROM TransferIssueDetails  ";
                sqlText += " WHERE TransferIssueNo=@TransferIssueNo ";

                SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                cmdInsDetail.Transaction = transaction;
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@TransferIssueNo", Master.TransferIssueNo);

                transResult = (int)cmdInsDetail.ExecuteNonQuery();

                #endregion

                foreach (TransferIssueDetailVM Detail in Details.ToList())
                {
                    Detail.TransactionType = Master.TransactionType;
                    Detail.TransferTo = Master.TransferTo;
                    Detail.Post = Master.Post;

                    Detail.TransferIssueNo = Master.TransferIssueNo;
                    Detail.CreatedBy = Master.CreatedBy;
                    Detail.CreatedOn = Master.CreatedOn;
                    Detail.LastModifiedBy = Master.LastModifiedBy;
                    Detail.LastModifiedOn = Master.LastModifiedOn;

                    retResults = TransferIssueInsertToDetail(Detail, currConn, transaction, connVM);

                    if (retResults[0] != "Success")
                    {
                        throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, MessageVM.issueMsgUpdateNotSuccessfully);
                    }
                }

                #endregion Insert only DetailTable

                #region Old Process

                ////foreach (var Item in Details.ToList())
                ////{
                ////    #region Find Transaction Mode Insert or Update

                ////    sqlText = "";
                ////    sqlText += "select COUNT(TransferIssueNo) from TransferIssueDetails WHERE TransferIssueNo='" + Master.TransferIssueNo + "' ";
                ////    sqlText += " AND ItemNo='" + Item.ItemNo + "'";
                ////    SqlCommand cmdFindId = new SqlCommand(sqlText, currConn);
                ////    cmdFindId.Transaction = transaction;
                ////    IDExist = (int)cmdFindId.ExecuteScalar();
                ////    if (IDExist <= 0)
                ////    {
                ////        // Insert
                ////        #region Insert only DetailTable

                ////        retResults = TransferIssueInsertToDetail(Item, currConn, transaction);

                ////        if (retResults[0] != "Success")
                ////        {
                ////            throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, MessageVM.issueMsgUpdateNotSuccessfully);
                ////        }
                ////        #endregion Insert only DetailTable
                ////    }
                ////    else
                ////    {
                ////        //Update
                ////        #region Update only DetailTable


                ////        Item.TransactionType = Master.TransactionType;
                ////        Item.TransferTo = Master.TransferTo;
                ////        Item.Post = Master.Post;
                ////        Item.TransferIssueNo = Master.TransferIssueNo;
                ////        Item.CreatedBy = Master.CreatedBy;
                ////        Item.CreatedOn = Master.CreatedOn;
                ////        Item.LastModifiedBy = Master.LastModifiedBy;
                ////        Item.LastModifiedOn = Master.LastModifiedOn;



                ////        retResults = TransferIssueUpdateToDetail(Item, currConn, transaction);
                ////        if (retResults[0] != "Success")
                ////        {
                ////            throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, MessageVM.issueMsgUpdateNotSuccessfully);
                ////        }
                ////        #endregion Update only DetailTable
                ////    }
                ////    #endregion Find Transaction Mode Insert or Update
                ////}


                ////#region Remove row

                ////sqlText = "";
                ////sqlText += " SELECT  distinct ItemNo";
                ////sqlText += " from TransferIssueDetails WHERE TransferIssueNo='" + Master.TransferIssueNo + "'";
                ////dt = new DataTable("Previous");
                ////SqlCommand cmdRIFB = new SqlCommand(sqlText, currConn);
                ////cmdRIFB.Transaction = transaction;
                ////SqlDataAdapter dta = new SqlDataAdapter(cmdRIFB);
                ////dta.Fill(dt);
                ////foreach (DataRow pItem in dt.Rows)
                ////{
                ////    var p = pItem["ItemNo"].ToString();
                ////    //var tt= Details.Find(x => x.ItemNo == p);
                ////    var tt = Details.Count(x => x.ItemNo.Trim() == p.Trim());
                ////    if (tt == 0)
                ////    {
                ////        sqlText = "";
                ////        sqlText += " delete FROM TransferIssueDetails  ";
                ////        sqlText += " WHERE TransferIssueNo='" + Master.TransferIssueNo + "' ";
                ////        sqlText += " AND ItemNo='" + p + "'";
                ////        SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                ////        cmdInsDetail.Transaction = transaction;
                ////        transResult = (int)cmdInsDetail.ExecuteNonQuery();
                ////    }
                ////}

                ////#endregion Remove row

                #endregion

                #endregion Update Detail Table

                #region Update PeriodId and Fiscal Year

                sqlText = "";
                sqlText += @"

update  TransferIssues                             
set PeriodId=RIGHT('0'+CONVERT(VARCHAR(2),MONTH(TransactionDateTime)) +  CONVERT(VARCHAR(4),YEAR(TransactionDateTime)),6)
WHERE TransferIssueNo = @TransferIssueNo


update  TransferIssueDetails                             
set PeriodId=RIGHT('0'+CONVERT(VARCHAR(2),MONTH(TransactionDateTime)) +  CONVERT(VARCHAR(4),YEAR(TransactionDateTime)),6)
WHERE TransferIssueNo = @TransferIssueNo


update TransferIssues set FiscalYear=fyr.CurrentYear
From TransferIssues inv
left outer join  FiscalYear fyr on inv.PeriodID=fyr.PeriodID

";

                SqlCommand cmdPeriodIdUpdate = new SqlCommand(sqlText, currConn, transaction);
                cmdPeriodIdUpdate.Parameters.AddWithValue("@TransferIssueNo", Master.TransferIssueNo);

                transResult = cmdPeriodIdUpdate.ExecuteNonQuery();

                #endregion

                #endregion  Update into Details(Update complete in Header)

                #region return Current ID and Post Status
                sqlText = "";
                sqlText = sqlText + "select distinct Post from TransferIssues WHERE TransferIssueNo='" + Master.TransferIssueNo + "'";
                SqlCommand cmdIPS = new SqlCommand(sqlText, currConn);
                cmdIPS.Transaction = transaction;
                PostStatus = (string)cmdIPS.ExecuteScalar();
                if (string.IsNullOrEmpty(PostStatus))
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, MessageVM.issueMsgUnableCreatID);
                }
                #endregion Prefetch

                #region Update Product Stock

                if (currentPostStatus == "Y" && dtCurrentItems != null && dtCurrentItems.Rows.Count > 0)
                {

                    ProductDAL productDal = new ProductDAL();
                    List<string> transactionTypes = new List<string>() { "62Out", "61Out" };

                    if (transactionTypes.Contains(Master.TransactionType.ToLower()))
                    {
                        DataTable dtItemNo = previousDetailVMs.Select(x => new { x.ItemNo, Quantity = x.UOMQty }).ToList().ToDataTable();
                        productDal.Product_IN_OUT(new ParameterVM() { dt = dtItemNo, BranchId = Master.BranchId }, currConn, transaction, connVM);
                    }
                    else
                    {

                        ResultVM rVM = new ResultVM();

                        ParameterVM paramVM = new ParameterVM();
                        paramVM.BranchId = Master.BranchId;
                        paramVM.InvoiceNo = Master.TransferIssueNo;
                        paramVM.dt = dtCurrentItems;

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

                commonDal.UpdateProcessFlag(Master.TransferIssueNo,
                    Convert.ToDateTime(prevInvoice.TransactionDateTime) <=
                    Convert.ToDateTime(Master.TransactionDateTime)
                        ? prevInvoice.TransactionDateTime
                        : Master.TransactionDateTime, currConn, transaction, connVM);

                #endregion

                #region Commit
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion Commit

                #region SuccessResult
                retResults[0] = "Success";
                retResults[1] = MessageVM.issueMsgUpdateSuccessfully;
                retResults[2] = Master.TransferIssueNo;
                retResults[3] = PostStatus;
                #endregion SuccessResult
            }
            #endregion Try

            #region Catch and Finall

            ////////catch (SqlException sqlex)
            ////////{
            ////////    transaction.Rollback();
            ////////    throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            ////////    //throw sqlex;
            ////////}

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
                throw new ArgumentNullException("", ex.Message.ToString());
                //throw ex;
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
        //        public string[] Update(TransferIssueVM Master, List<TransferIssueDetailVM> Details, SysDBInfoVMTemp connVM = null, string UserId = "")
        //        {

        //            #region Initializ

        //            string[] retResults = new string[4];
        //            retResults[0] = "Fail";
        //            retResults[1] = "Fail";
        //            retResults[2] = "";
        //            retResults[3] = "";
        //            SqlConnection currConn = null;
        //            SqlTransaction transaction = null;
        //            int transResult = 0;
        //            string sqlText = "";
        //            //DateTime MinDate = DateTime.MinValue;
        //            //DateTime MaxDate = DateTime.MaxValue;
        //            string PostStatus = "";
        //            string vehicleId = "0";

        //            #endregion Initializ

        //            #region Try
        //            try
        //            {

        //                #region Find Month Lock

        //                string PeriodName = Convert.ToDateTime(Master.TransactionDateTime).ToString("MMMM-yyyy");
        //                string[] vValues = { PeriodName };
        //                string[] vFields = { "PeriodName" };
        //                FiscalYearVM varFiscalYearVM = new FiscalYearVM();
        //                varFiscalYearVM = new FiscalYearDAL().SelectAll(0, vFields, vValues, null, null, connVM).FirstOrDefault();

        //                if (varFiscalYearVM == null)
        //                {
        //                    throw new Exception(PeriodName + ": This Fiscal Period is not Exist!");

        //                }

        //                if (varFiscalYearVM.VATReturnPost == "Y")
        //                {
        //                    throw new Exception(PeriodName + ": VAT Return (9.1) already submitted for this month!");

        //                }

        //                #endregion Find Month Lock

        //                #region Validation for Header
        //                if (Master == null)
        //                {
        //                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, MessageVM.issueMsgNoDataToUpdate);
        //                }
        //                else if (Convert.ToDateTime(Master.TransactionDateTime) < DateTime.MinValue || Convert.ToDateTime(Master.TransactionDateTime) > DateTime.MaxValue)
        //                {
        //                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, "Please Check Invoice Data and Time");
        //                }
        //                #endregion Validation for Header

        //                #region open connection and transaction
        //                currConn = _dbsqlConnection.GetConnection(connVM);
        //                if (currConn.State != ConnectionState.Open)
        //                {
        //                    currConn.Open();
        //                }
        //                #region Add BOMId
        //                CommonDAL commonDal = new CommonDAL();
        //                #endregion Add BOMId
        //                transaction = currConn.BeginTransaction(MessageVM.issueMsgMethodNameUpdate);
        //                #endregion open connection and transaction

        //                #region Current Status

        //                #region Post Status

        //                string currentPostStatus = "N";

        //                sqlText = "";
        //                sqlText = @"
        //----------declare @InvoiceNo as varchar(100)='T1O-2/0001/1220'
        //
        //select TransferIssueNo, Post from TransferIssues
        //where 1=1 
        //and TransferIssueNo=@InvoiceNo
        //
        //";
        //                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
        //                cmd.Parameters.AddWithValue("@InvoiceNo", Master.TransferIssueNo);

        //                SqlDataAdapter da = new SqlDataAdapter(cmd);
        //                DataTable dt = new DataTable();
        //                da.Fill(dt);

        //                if (dt != null && dt.Rows.Count > 0)
        //                {
        //                    currentPostStatus = dt.Rows[0]["Post"].ToString();
        //                }

        //                #endregion

        //                #region Current Items
        //                DataTable dtCurrentItems = new DataTable();

        //                if (currentPostStatus == "Y")
        //                {
        //                    sqlText = "";
        //                    sqlText = @"
        //----------declare @InvoiceNo as varchar(100)='T1O-2/0001/1220'
        //
        //
        //select ItemNo, TransferIssueNo from TransferIssueDetails
        //where 1=1 
        //and TransferIssueNo=@InvoiceNo
        //
        //";

        //                    cmd = new SqlCommand(sqlText, currConn, transaction);
        //                    cmd.Parameters.AddWithValue("@InvoiceNo", Master.TransferIssueNo);

        //                    da = new SqlDataAdapter(cmd);
        //                    da.Fill(dtCurrentItems);

        //                }
        //                #endregion

        //                #endregion

        //                #region Fiscal Year Check
        //                string transactionDate = Master.TransactionDateTime;
        //                string transactionYearCheck = Convert.ToDateTime(Master.TransactionDateTime).ToString("yyyy-MM-dd");
        //                if (Convert.ToDateTime(transactionYearCheck) > DateTime.MinValue || Convert.ToDateTime(transactionYearCheck) < DateTime.MaxValue)
        //                {
        //                    #region YearLock
        //                    sqlText = "";
        //                    sqlText += "select distinct isnull(PeriodLock,'Y') MLock,isnull(GLLock,'Y')YLock from fiscalyear " +
        //                                                   " where '" + transactionYearCheck + "' between PeriodStart and PeriodEnd";
        //                    DataTable dataTable = new DataTable("ProductDataT");
        //                    SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
        //                    cmdIdExist.Transaction = transaction;
        //                    SqlDataAdapter reportDataAdapt = new SqlDataAdapter(cmdIdExist);
        //                    reportDataAdapt.Fill(dataTable);
        //                    if (dataTable == null)
        //                    {
        //                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
        //                    }
        //                    else if (dataTable.Rows.Count <= 0)
        //                    {
        //                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
        //                    }
        //                    else
        //                    {
        //                        if (dataTable.Rows[0]["MLock"].ToString() != "N")
        //                        {
        //                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
        //                        }
        //                        else if (dataTable.Rows[0]["YLock"].ToString() != "N")
        //                        {
        //                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearisLock);
        //                        }
        //                    }
        //                    #endregion YearLock
        //                    #region YearNotExist
        //                    sqlText = "";
        //                    sqlText = sqlText + "select  min(PeriodStart) MinDate, max(PeriodEnd)  MaxDate from fiscalyear";
        //                    DataTable dtYearNotExist = new DataTable("ProductDataT");
        //                    SqlCommand cmdYearNotExist = new SqlCommand(sqlText, currConn);
        //                    cmdYearNotExist.Transaction = transaction;
        //                    //countId = (int)cmdIdExist.ExecuteScalar();
        //                    SqlDataAdapter YearNotExistDataAdapt = new SqlDataAdapter(cmdYearNotExist);
        //                    YearNotExistDataAdapt.Fill(dtYearNotExist);
        //                    if (dtYearNotExist == null)
        //                    {
        //                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearNotExist);
        //                    }
        //                    else if (dtYearNotExist.Rows.Count < 0)
        //                    {
        //                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearNotExist);
        //                    }
        //                    else
        //                    {
        //                        if (Convert.ToDateTime(transactionYearCheck) < Convert.ToDateTime(dtYearNotExist.Rows[0]["MinDate"].ToString())
        //                            || Convert.ToDateTime(transactionYearCheck) > Convert.ToDateTime(dtYearNotExist.Rows[0]["MaxDate"].ToString()))
        //                        {
        //                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgFiscalYearNotExist);
        //                        }
        //                    }
        //                    #endregion YearNotExist
        //                }
        //                #endregion Fiscal Year CHECK

        //                List<TransferIssueDetailVM> previousDetailVMs = SelectDetail(Master.TransferIssueNo, null, null, currConn, transaction, connVM);

        //                TransferIssueVM prevInvoice = SelectAllList(0, new[] { "ti.TransferIssueNo" },
        //                    new[] { Master.TransferIssueNo }, currConn, transaction, connVM).FirstOrDefault();


        //                #region Master Setting Update

        //                commonDal.UpdateProcessFlag(Master.TransferIssueNo, prevInvoice.TransactionDateTime, currConn, transaction, connVM);

        //                #endregion

        //                #region Product Type check

        //                foreach (TransferIssueDetailVM Items in Details.ToList())
        //                {

        //                    ProductDAL _ProductDAL = new ProductDAL();

        //                    ProductVM ProductVM = _ProductDAL.SelectAll(Items.ItemNo, null, null, currConn, transaction, null, connVM).FirstOrDefault();
        //                    string ProductType = ProductVM.IsRaw;

        //                    if (ProductType.ToLower() != "finish" && ProductType.ToLower() != "trading" && Master.TransactionType.ToLower() == "62out")
        //                    {
        //                        throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, "This Product: " + ProductVM.ProductName + "( " + ProductVM.ProductCode + " ) " + " Type: " + ProductType + " is not valid for FG(Out)");
        //                    }

        //                    if (Master.TransactionType.ToLower() == "61out")
        //                    {
        //                        if (ProductType.ToLower() != "raw" && ProductType.ToLower() != "wip" && ProductType.ToLower() != "pack")
        //                        {
        //                            throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, "This Product: " + ProductVM.ProductName + "( " + ProductVM.ProductCode + " ) " + " Type: " + ProductType + " is not valid for RM(Out)");

        //                        }

        //                    }

        //                }

        //                #endregion Product Type check

        //                #region Vehicle

        //                if (string.IsNullOrWhiteSpace(Master.VehicleNo)
        //                    )
        //                {
        //                    vehicleId = "0";
        //                }
        //                else
        //                {

        //                    if (string.IsNullOrEmpty(Master.VehicleID) || Convert.ToDecimal(Master.VehicleID) <= 0)
        //                    {
        //                        string vehicleID = "0";
        //                        sqlText = "";
        //                        sqlText = sqlText + "select VehicleID from Vehicles WHERE VehicleNo=@MasterVehicleNo ";

        //                        SqlCommand cmdExistVehicleId = new SqlCommand(sqlText, currConn);
        //                        cmdExistVehicleId.Transaction = transaction;
        //                        cmdExistVehicleId.Parameters.AddWithValue("@MasterVehicleNo", Master.VehicleNo);
        //                        string vehicleIDExist = Convert.ToString(cmdExistVehicleId.ExecuteScalar());
        //                        vehicleId = vehicleIDExist;

        //                        if (string.IsNullOrEmpty(vehicleId) || vehicleId == null || Convert.ToDecimal(vehicleId) <= 0)
        //                        {

        //                            sqlText = "";
        //                            sqlText = sqlText + "select isnull(max(cast(VehicleID as int)),0)+1 FROM  Vehicles ";
        //                            SqlCommand cmdVehicleId = new SqlCommand(sqlText, currConn);
        //                            cmdVehicleId.Transaction = transaction;
        //                            int NewvehicleID = (int)cmdVehicleId.ExecuteScalar();
        //                            vehicleID = NewvehicleID.ToString();

        //                            sqlText = "";
        //                            sqlText +=
        //                                " INSERT INTO Vehicles (VehicleID,	VehicleType,	VehicleNo,	Description,	Comments,	ActiveStatus,CreatedBy,	CreatedOn,	LastModifiedBy,	LastModifiedOn)";
        //                            sqlText += "values(@VehicleID";
        //                            sqlText += " ,@MasterVehicleType ";
        //                            sqlText += " ,@MasterVehicleNo ";
        //                            sqlText += " ,'NA'";
        //                            sqlText += " ,'NA'";
        //                            if (Master.vehicleSaveInDB == true)
        //                            {
        //                                sqlText += ",'Y'";
        //                            }
        //                            else
        //                            {
        //                                sqlText += ",'N'";
        //                            }

        //                            sqlText += " ,@MasterCreatedBy ";
        //                            sqlText += " ,@MasterCreatedOn ";
        //                            sqlText += " ,@MasterLastModifiedBy ";
        //                            sqlText += " ,@MasterLastModifiedOn)";
        //                            //sqlText += " from Vehicles;";

        //                            SqlCommand cmdExistVehicleIns = new SqlCommand(sqlText, currConn);
        //                            cmdExistVehicleIns.Transaction = transaction;
        //                            cmdExistVehicleIns.Parameters.AddWithValue("@VehicleID", vehicleID);
        //                            cmdExistVehicleIns.Parameters.AddWithValue("@MasterVehicleType", Master.VehicleType ?? "-");
        //                            cmdExistVehicleIns.Parameters.AddWithValue("@MasterVehicleNo", Master.VehicleNo);
        //                            cmdExistVehicleIns.Parameters.AddWithValue("@MasterCreatedBy", Master.CreatedBy);
        //                            cmdExistVehicleIns.Parameters.AddWithValue("@MasterCreatedOn", OrdinaryVATDesktop.DateToDate(Master.CreatedOn));
        //                            cmdExistVehicleIns.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy);
        //                            cmdExistVehicleIns.Parameters.AddWithValue("@MasterLastModifiedOn", OrdinaryVATDesktop.DateToDate(Master.LastModifiedOn));
        //                            transResult = (int)cmdExistVehicleIns.ExecuteNonQuery();
        //                            if (transResult <= 0)
        //                            {
        //                                throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert,
        //                                    MessageVM.saleMsgUnableCreatID);
        //                            }
        //                            vehicleId = vehicleID.ToString();
        //                            if (string.IsNullOrEmpty(vehicleId))
        //                            {
        //                                throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgUnableCreatID);
        //                            }

        //                        }

        //                        Master.VehicleID = vehicleId;

        //                    }

        //                }

        //                #endregion Vehicle

        //                #region Find ID for Update
        //                sqlText = "";
        //                sqlText = sqlText + "select COUNT(TransferIssueNo) from TransferIssues WHERE TransferIssueNo='" + Master.TransferIssueNo + "' ";
        //                SqlCommand cmdFindIdUpd = new SqlCommand(sqlText, currConn);
        //                cmdFindIdUpd.Transaction = transaction;
        //                int IDExist = (int)cmdFindIdUpd.ExecuteScalar();
        //                if (IDExist <= 0)
        //                {
        //                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, MessageVM.issueMsgUnableFindExistID);
        //                }
        //                #endregion Find ID for Update

        //                #region ID check completed,update Information in Header

        //                #region update Header

        //                retResults = TransferIssueUpdateToMaster(Master, currConn, transaction, connVM);

        //                if (retResults[0] != "Success")
        //                {
        //                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, MessageVM.issueMsgUpdateNotSuccessfully);
        //                }

        //                #region Update PeriodId and Fiscal Year

        //                sqlText = "";
        //                sqlText += @"
        //
        //update  TransferIssues                             
        //set PeriodId=RIGHT('0'+CONVERT(VARCHAR(2),MONTH(TransactionDateTime)) +  CONVERT(VARCHAR(4),YEAR(TransactionDateTime)),6)
        //WHERE TransferIssueNo = @TransferIssueNo
        //
        //
        //update  TransferIssueDetails                             
        //set PeriodId=RIGHT('0'+CONVERT(VARCHAR(2),MONTH(TransactionDateTime)) +  CONVERT(VARCHAR(4),YEAR(TransactionDateTime)),6)
        //WHERE TransferIssueNo = @TransferIssueNo
        //
        //
        //update TransferIssues set FiscalYear=fyr.CurrentYear
        //From TransferIssues inv
        //left outer join  FiscalYear fyr on inv.PeriodID=fyr.PeriodID
        //
        //";

        //                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
        //                cmdUpdate.Parameters.AddWithValue("@TransferIssueNo", Master.TransferIssueNo);

        //                transResult = cmdUpdate.ExecuteNonQuery();
        //                #endregion

        //                #endregion update Header

        //                #endregion ID check completed,update Information in Header

        //                #region Update into Details(Update complete in Header)

        //                #region Validation for Detail

        //                if (Details.Count() < 0)
        //                {
        //                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, MessageVM.issueMsgNoDataToUpdate);
        //                }

        //                #endregion Validation for Detail

        //                #region Update Detail Table

        //                // Insert
        //                #region Insert only DetailTable

        //                #region Delete Details

        //                sqlText = "";
        //                sqlText += " delete FROM TransferIssueDetails  ";
        //                sqlText += " WHERE TransferIssueNo=@TransferIssueNo ";

        //                SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
        //                cmdInsDetail.Transaction = transaction;
        //                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@TransferIssueNo", Master.TransferIssueNo);

        //                transResult = (int)cmdInsDetail.ExecuteNonQuery();

        //                #endregion

        //                foreach (TransferIssueDetailVM Detail in Details.ToList())
        //                {
        //                    Detail.TransactionType = Master.TransactionType;
        //                    Detail.TransferTo = Master.TransferTo;
        //                    Detail.Post = Master.Post;

        //                    Detail.TransferIssueNo = Master.TransferIssueNo;
        //                    Detail.CreatedBy = Master.CreatedBy;
        //                    Detail.CreatedOn = Master.CreatedOn;
        //                    Detail.LastModifiedBy = Master.LastModifiedBy;
        //                    Detail.LastModifiedOn = Master.LastModifiedOn;

        //                    retResults = TransferIssueInsertToDetail(Detail, currConn, transaction, connVM);

        //                    if (retResults[0] != "Success")
        //                    {
        //                        throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, MessageVM.issueMsgUpdateNotSuccessfully);
        //                    }
        //                }

        //                #endregion Insert only DetailTable

        //                #region Old Process

        //                ////foreach (var Item in Details.ToList())
        //                ////{
        //                ////    #region Find Transaction Mode Insert or Update

        //                ////    sqlText = "";
        //                ////    sqlText += "select COUNT(TransferIssueNo) from TransferIssueDetails WHERE TransferIssueNo='" + Master.TransferIssueNo + "' ";
        //                ////    sqlText += " AND ItemNo='" + Item.ItemNo + "'";
        //                ////    SqlCommand cmdFindId = new SqlCommand(sqlText, currConn);
        //                ////    cmdFindId.Transaction = transaction;
        //                ////    IDExist = (int)cmdFindId.ExecuteScalar();
        //                ////    if (IDExist <= 0)
        //                ////    {
        //                ////        // Insert
        //                ////        #region Insert only DetailTable

        //                ////        retResults = TransferIssueInsertToDetail(Item, currConn, transaction);

        //                ////        if (retResults[0] != "Success")
        //                ////        {
        //                ////            throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, MessageVM.issueMsgUpdateNotSuccessfully);
        //                ////        }
        //                ////        #endregion Insert only DetailTable
        //                ////    }
        //                ////    else
        //                ////    {
        //                ////        //Update
        //                ////        #region Update only DetailTable


        //                ////        Item.TransactionType = Master.TransactionType;
        //                ////        Item.TransferTo = Master.TransferTo;
        //                ////        Item.Post = Master.Post;
        //                ////        Item.TransferIssueNo = Master.TransferIssueNo;
        //                ////        Item.CreatedBy = Master.CreatedBy;
        //                ////        Item.CreatedOn = Master.CreatedOn;
        //                ////        Item.LastModifiedBy = Master.LastModifiedBy;
        //                ////        Item.LastModifiedOn = Master.LastModifiedOn;



        //                ////        retResults = TransferIssueUpdateToDetail(Item, currConn, transaction);
        //                ////        if (retResults[0] != "Success")
        //                ////        {
        //                ////            throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, MessageVM.issueMsgUpdateNotSuccessfully);
        //                ////        }
        //                ////        #endregion Update only DetailTable
        //                ////    }
        //                ////    #endregion Find Transaction Mode Insert or Update
        //                ////}


        //                ////#region Remove row

        //                ////sqlText = "";
        //                ////sqlText += " SELECT  distinct ItemNo";
        //                ////sqlText += " from TransferIssueDetails WHERE TransferIssueNo='" + Master.TransferIssueNo + "'";
        //                ////dt = new DataTable("Previous");
        //                ////SqlCommand cmdRIFB = new SqlCommand(sqlText, currConn);
        //                ////cmdRIFB.Transaction = transaction;
        //                ////SqlDataAdapter dta = new SqlDataAdapter(cmdRIFB);
        //                ////dta.Fill(dt);
        //                ////foreach (DataRow pItem in dt.Rows)
        //                ////{
        //                ////    var p = pItem["ItemNo"].ToString();
        //                ////    //var tt= Details.Find(x => x.ItemNo == p);
        //                ////    var tt = Details.Count(x => x.ItemNo.Trim() == p.Trim());
        //                ////    if (tt == 0)
        //                ////    {
        //                ////        sqlText = "";
        //                ////        sqlText += " delete FROM TransferIssueDetails  ";
        //                ////        sqlText += " WHERE TransferIssueNo='" + Master.TransferIssueNo + "' ";
        //                ////        sqlText += " AND ItemNo='" + p + "'";
        //                ////        SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
        //                ////        cmdInsDetail.Transaction = transaction;
        //                ////        transResult = (int)cmdInsDetail.ExecuteNonQuery();
        //                ////    }
        //                ////}

        //                ////#endregion Remove row

        //                #endregion

        //                #endregion Update Detail Table

        //                #region Update PeriodId and Fiscal Year

        //                sqlText = "";
        //                sqlText += @"
        //
        //update  TransferIssues                             
        //set PeriodId=RIGHT('0'+CONVERT(VARCHAR(2),MONTH(TransactionDateTime)) +  CONVERT(VARCHAR(4),YEAR(TransactionDateTime)),6)
        //WHERE TransferIssueNo = @TransferIssueNo
        //
        //
        //update  TransferIssueDetails                             
        //set PeriodId=RIGHT('0'+CONVERT(VARCHAR(2),MONTH(TransactionDateTime)) +  CONVERT(VARCHAR(4),YEAR(TransactionDateTime)),6)
        //WHERE TransferIssueNo = @TransferIssueNo
        //
        //
        //update TransferIssues set FiscalYear=fyr.CurrentYear
        //From TransferIssues inv
        //left outer join  FiscalYear fyr on inv.PeriodID=fyr.PeriodID
        //
        //";

        //                SqlCommand cmdPeriodIdUpdate = new SqlCommand(sqlText, currConn, transaction);
        //                cmdPeriodIdUpdate.Parameters.AddWithValue("@TransferIssueNo", Master.TransferIssueNo);

        //                transResult = cmdPeriodIdUpdate.ExecuteNonQuery();

        //                #endregion

        //                #endregion  Update into Details(Update complete in Header)

        //                #region return Current ID and Post Status
        //                sqlText = "";
        //                sqlText = sqlText + "select distinct Post from TransferIssues WHERE TransferIssueNo='" + Master.TransferIssueNo + "'";
        //                SqlCommand cmdIPS = new SqlCommand(sqlText, currConn);
        //                cmdIPS.Transaction = transaction;
        //                PostStatus = (string)cmdIPS.ExecuteScalar();
        //                if (string.IsNullOrEmpty(PostStatus))
        //                {
        //                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, MessageVM.issueMsgUnableCreatID);
        //                }
        //                #endregion Prefetch

        //                #region Update Product Stock

        //                if (currentPostStatus == "Y" && dtCurrentItems != null && dtCurrentItems.Rows.Count > 0)
        //                {

        //                    ProductDAL productDal = new ProductDAL();
        //                    List<string> transactionTypes = new List<string>() { "62Out", "61Out" };

        //                    if (transactionTypes.Contains(Master.TransactionType.ToLower()))
        //                    {
        //                        DataTable dtItemNo = previousDetailVMs.Select(x => new { x.ItemNo, Quantity = x.UOMQty }).ToList().ToDataTable();
        //                        productDal.Product_IN_OUT(new ParameterVM() { dt = dtItemNo, BranchId = Master.BranchId }, currConn, transaction, connVM);
        //                    }
        //                    else
        //                    {

        //                        ResultVM rVM = new ResultVM();

        //                        ParameterVM paramVM = new ParameterVM();
        //                        paramVM.BranchId = Master.BranchId;
        //                        paramVM.InvoiceNo = Master.TransferIssueNo;
        //                        paramVM.dt = dtCurrentItems;

        //                        paramVM.IDs = new List<string>();

        //                        foreach (DataRow dr in paramVM.dt.Rows)
        //                        {
        //                            paramVM.IDs.Add(dr["ItemNo"].ToString());
        //                        }

        //                        if (paramVM.IDs.Count > 0)
        //                        {
        //                            rVM = _ProductDAL.Product_Stock_Update(paramVM, currConn, transaction, connVM, UserId);
        //                        }
        //                    }
        //                }

        //                #endregion

        //                #region Master Setting Update

        //                commonDal.UpdateProcessFlag(Master.TransferIssueNo,
        //                    Convert.ToDateTime(prevInvoice.TransactionDateTime) <=
        //                    Convert.ToDateTime(Master.TransactionDateTime)
        //                        ? prevInvoice.TransactionDateTime
        //                        : Master.TransactionDateTime, currConn, transaction, connVM);

        //                #endregion

        //                #region Commit
        //                if (transaction != null)
        //                {
        //                    //if (transResult > 0)
        //                    //{
        //                    transaction.Commit();
        //                    //}
        //                }
        //                #endregion Commit

        //                #region SuccessResult
        //                retResults[0] = "Success";
        //                retResults[1] = MessageVM.issueMsgUpdateSuccessfully;
        //                retResults[2] = Master.TransferIssueNo;
        //                retResults[3] = PostStatus;
        //                #endregion SuccessResult
        //            }
        //            #endregion Try

        //            #region Catch and Finall

        //            ////////catch (SqlException sqlex)
        //            ////////{
        //            ////////    transaction.Rollback();
        //            ////////    throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
        //            ////////    //throw sqlex;
        //            ////////}

        //            catch (Exception ex)
        //            {
        //                retResults = new string[5];
        //                retResults[0] = "Fail";
        //                retResults[1] = ex.Message;
        //                retResults[2] = "0";
        //                retResults[3] = "N";
        //                retResults[4] = "0";
        //                if (currConn != null)
        //                {
        //                    transaction.Rollback();
        //                }
        //                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
        //                throw new ArgumentNullException("", ex.Message.ToString());
        //                //throw ex;
        //            }
        //            finally
        //            {
        //                if (currConn != null)
        //                {
        //                    if (currConn.State == ConnectionState.Open)
        //                    {
        //                        currConn.Close();
        //                    }
        //                }
        //            }
        //            #endregion Catch and Finall

        //            #region Result
        //            return retResults;
        //            #endregion Result

        //        }

        public string[] Post(TransferIssueVM Master, List<TransferIssueDetailVM> Details, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string[] retResults = new string[5];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";
            retResults[4] = "";// DB Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            string sqlText = "";
            string PostStatus = "";
            #endregion Initializ

            #region Try
            try
            {

                #region Find Month Lock

                string PeriodName = Convert.ToDateTime(Master.TransactionDateTime).ToString("MMMM-yyyy");
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

                #region Checkpoint

                string vNegStockAllow = string.Empty;
                string AutoReceive = "N";
                CommonDAL commonDal = new CommonDAL();
                vNegStockAllow = commonDal.settings("TransferIssue", "NegStockAllow", null, null, connVM);
                AutoReceive = commonDal.settings("TransferReceive", "AutoReceive", null, null, connVM);
                if (string.IsNullOrEmpty(vNegStockAllow))
                {
                    throw new ArgumentNullException(MessageVM.msgSettingValueNotSave, "TransferIssue");
                }
                bool NegStockAllow = Convert.ToBoolean(vNegStockAllow == "Y" ? true : false);
                #endregion

                #region Validation for Header
                if (Master == null)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNamePost, MessageVM.issueMsgNoDataToPost);
                }
                else if (Convert.ToDateTime(Master.TransactionDateTime) < DateTime.MinValue || Convert.ToDateTime(Master.TransactionDateTime) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNamePost, MessageVM.issueMsgCheckDatePost);
                }
                #endregion Validation for Header

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

                #region Post Status

                string currentPostStatus = "N";

                sqlText = "";
                sqlText = @"
----------declare @InvoiceNo as varchar(100)='T1O-2/0001/1220'

select TransferIssueNo, Post from TransferIssues
where 1=1 
and TransferIssueNo=@InvoiceNo

";
                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
                cmd.Parameters.AddWithValue("@InvoiceNo", Master.TransferIssueNo);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt != null && dt.Rows.Count > 0)
                {
                    currentPostStatus = dt.Rows[0]["Post"].ToString();
                }

                if (currentPostStatus == "Y")
                {
                    throw new Exception("This Invoice Already Posted! Invoice No: " + Master.TransferIssueNo);
                }

                #endregion

                #region Fiscal Year Check
                string transactionDate = Master.TransactionDateTime;
                string transactionYearCheck = Convert.ToDateTime(Master.TransactionDateTime).ToString("yyyy-MM-dd");
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
                sqlText = sqlText + "select COUNT(TransferIssueNo) from TransferIssues WHERE TransferIssueNo='" + Master.TransferIssueNo + "' ";
                SqlCommand cmdFindIdUpd = new SqlCommand(sqlText, currConn);
                cmdFindIdUpd.Transaction = transaction;
                int IDExist = (int)cmdFindIdUpd.ExecuteScalar();
                if (IDExist <= 0)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNamePost, MessageVM.issueMsgUnableFindExistIDPost);
                }
                #endregion Find ID for Update

                #region ID check completed,update Information in Header

                #region update Header

                sqlText = "";
                sqlText += @" Update  TransferIssues                set  Post ='Y',SignatoryName =@SignatoryName, SignatoryDesig =@SignatoryDesig,LastModifiedBy =@LastModifiedBy, LastModifiedOn =@LastModifiedOn, IsTransfer=@IsTransfer    WHERE TransferIssueNo=@TransferIssueNo ";
                sqlText += @" Update  TransferIssueDetails          set  Post ='Y',LastModifiedBy =@LastModifiedBy, LastModifiedOn =@LastModifiedOn                    WHERE TransferIssueNo=@TransferIssueNo ";
                sqlText += @" Update  TransferIssueTrackings        set  Post ='Y',LastModifiedBy =@LastModifiedBy, LastModifiedOn =@LastModifiedOn                    WHERE TransferIssueNo=@TransferIssueNo ";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@TransferIssueNo", Master.TransferIssueNo);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@LastModifiedOn", Convert.ToDateTime(Master.LastModifiedOn).ToString("yyyy-MMM-dd HH:mm:ss"));
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@LastModifiedBy", Master.LastModifiedBy);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@IsTransfer", Master.IsTransfer);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@SignatoryName", Master.SignatoryName);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@SignatoryDesig", Master.SignatoryDesig);

                transResult = cmdUpdate.ExecuteNonQuery();


                #endregion update Header

                #endregion ID check completed,update Information in Header

                #region Stock Check

                if (!NegStockAllow)
                {
                    #region Update into Details(Update complete in Header)
                    #region Validation for Detail
                    if (Details.Count() < 0)
                    {
                        throw new ArgumentNullException(MessageVM.issueMsgMethodNamePost, MessageVM.issueMsgNoDataToPost);
                    }
                    #endregion Validation for Detail
                    #region Update Detail Table
                    foreach (var Item in Details.ToList())
                    {
                        #region Find Transaction Mode Insert or Update
                        sqlText = "";
                        sqlText += "select COUNT(TransferIssueNo) from TransferIssueDetails WHERE TransferIssueNo='" + Master.TransferIssueNo + "' ";
                        sqlText += " AND ItemNo='" + Item.ItemNo + "'";
                        SqlCommand cmdFindId = new SqlCommand(sqlText, currConn);
                        cmdFindId.Transaction = transaction;
                        IDExist = (int)cmdFindId.ExecuteScalar();
                        if (IDExist <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.issueMsgMethodNamePost, MessageVM.issueMsgNoDataToPost);
                        }
                        else
                        {
                            #region Update only DetailTable
                            sqlText = "";
                            sqlText += " update TransferIssueDetails set ";
                            sqlText += " Post='" + Master.Post + "'";

                            sqlText += " where  TransferIssueNo ='" + Master.TransferIssueNo + "' ";
                            sqlText += " and ItemNo='" + Item.ItemNo + "'";
                            SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                            cmdInsDetail.Transaction = transaction;
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
                                decimal oldStock = Convert.ToDecimal(productDal.AvgPriceNew(Item.ItemNo, Master.TransactionDateTime,
                                                                  currConn, transaction, true).Rows[0]["Quantity"].ToString());


                                #endregion Find Quantity From Transaction
                                #region Find Quantity From Transaction
                                sqlText = "";
                                sqlText += "select isnull(Quantity ,0) from TransferIssueDetails ";
                                sqlText += " WHERE ItemNo='" + Item.ItemNo + "' and TransferIssueNo= '" + Master.TransferIssueNo + "'";
                                SqlCommand cmdTranQty = new SqlCommand(sqlText, currConn);
                                cmdTranQty.Transaction = transaction;
                                decimal TranQty = (decimal)cmdTranQty.ExecuteScalar();

                               
                                #endregion Find Quantity From Transaction


                                #region Find ProductCode & ProductName From Transaction

                                var productCode = Item.ProductCode;
                               
                                sqlText = "";
                                sqlText += "SELECT ProductName FROM Products ";
                                sqlText += " WHERE ProductCode='" + productCode + "'";
                                SqlCommand cmdProductName= new SqlCommand(sqlText, currConn);
                                cmdProductName.Transaction = transaction;
                                string productName = (string)cmdProductName.ExecuteScalar();


                                #endregion Find ProductCode & ProductName From Transaction


                                #region Qty  check and Update
                                if (NegStockAllow == false)
                                {
                                    //if (TranQty > (oldStock + TranQty))
                                    if (TranQty > oldStock )
                                    {
                                        throw new ArgumentNullException(MessageVM.issueMsgMethodNamePost,
                                                                        MessageVM.issueMsgStockNotAvailablePost + ". Product Code is : " + productCode + " & Product Name is : " + productName);
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

                }

                #endregion

                #region return Current ID and Post Status
                sqlText = "";
                sqlText = sqlText + "select distinct Post from TransferIssues WHERE TransferIssueNo='" + Master.TransferIssueNo + "'";
                SqlCommand cmdIPS = new SqlCommand(sqlText, currConn);
                cmdIPS.Transaction = transaction;
                PostStatus = (string)cmdIPS.ExecuteScalar();
                if (string.IsNullOrEmpty(PostStatus))
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNamePost, MessageVM.issueMsgPostNotSelect);
                }
                #endregion Prefetch

                #region TransferIssue_Product_IN_OUT

                ResultVM rVM = new ResultVM();

                ParameterVM paramVM = new ParameterVM();
                paramVM.InvoiceNo = Master.TransferIssueNo;

                rVM = TransferIssue_Product_IN_OUT(paramVM, currConn, transaction, connVM);

                #endregion

                #region Auto TransferRecive
                if (AutoReceive.ToLower() == "y")
                {
                    DataTable TransferReceivedt = new DataTable();

                    TransferReceivedt = SearchTransferIssue(new TransferIssueVM() { TransferIssueNo = paramVM.InvoiceNo, TransactionType = "", IssueDateFrom = "", IssueDateTo = "", Post = "", ReferenceNo = "" }, currConn, transaction, connVM);
                    TransferReceiveVM TransferReceive = new TransferReceiveVM();
                    TransferReceive.TransactionDateTime = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                    TransferReceive.Comments = "-";
                    TransferReceive.CreatedBy = TransferReceivedt.Rows[0]["CreatedBy"].ToString();
                    TransferReceive.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                    TransferReceive.TransactionType = TransferReceivedt.Rows[0]["TransactionType"].ToString() == "62Out" ? "62In" : "61In";
                    TransferReceive.TransferNo = "-";
                    TransferReceive.TransferFromNo = TransferReceivedt.Rows[0]["TransferIssueNo"].ToString();
                    TransferReceive.TransferFrom = Convert.ToInt32(TransferReceivedt.Rows[0]["BranchId"].ToString());
                    TransferReceive.Post = "Y";
                    TransferReceive.TotalAmount = Convert.ToDecimal(TransferReceivedt.Rows[0]["TotalAmount"].ToString());
                    TransferReceive.BranchId = Convert.ToInt32(TransferReceivedt.Rows[0]["TransferTo"].ToString());

                    DataTable transferDetailResult = SearchTransferDetail(Master.TransferIssueNo, currConn, transaction, connVM);
                    List<TransferReceiveDetailVM> TransferReceiveDetailVMs = new List<TransferReceiveDetailVM>();
                    int i = 0;
                    foreach (DataRow item in transferDetailResult.Rows)
                    {
                        TransferReceiveDetailVM detail = new TransferReceiveDetailVM();
                        detail.ReceiveLineNo = i++.ToString();
                        detail.ItemNo = item["ItemNo"].ToString();
                        detail.Quantity = Convert.ToDecimal(item["Quantity"].ToString());
                        detail.CostPrice = Convert.ToDecimal(item["CostPrice"].ToString());
                        detail.UOM = item["UOM"].ToString();
                        detail.SubTotal = Convert.ToDecimal(item["SubTotal"].ToString());
                        detail.Comments = "FromTransferReceive";
                        detail.TransactionDateTime = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                        detail.TransferFrom = Convert.ToInt32(TransferReceivedt.Rows[0]["BranchId"].ToString());
                        detail.UOMQty = Convert.ToDecimal(item["UOMQty"].ToString());
                        detail.UOMn = item["UOMn"].ToString();
                        detail.UOMc = Convert.ToDecimal(item["UOMc"].ToString());
                        detail.UOMPrice = Convert.ToDecimal(item["UOMPrice"].ToString());

                        detail.VATRate = Convert.ToDecimal(item["VATRate"].ToString());
                        detail.VATAmount = Convert.ToDecimal(item["VATAmount"].ToString());
                        detail.SDRate = Convert.ToDecimal(item["SDRate"].ToString());
                        detail.SDAmount = Convert.ToDecimal(item["SDAmount"].ToString());
                        detail.Weight = item["Weight"].ToString();

                        detail.BranchId = TransferReceive.BranchId;
                        detail.TransferFromNo = TransferReceive.TransferFromNo;


                        TransferReceiveDetailVMs.Add(detail);
                    }

                    string[] sqlResult = new TransferReceiveDAL().Insert(TransferReceive, TransferReceiveDetailVMs, transaction, currConn, connVM);

                }

                #endregion

                #region Commit
                if (transaction != null && Vtransaction == null)
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
                retResults[2] = Master.TransferIssueNo;
                retResults[3] = PostStatus;
                #endregion SuccessResult
            }
            #endregion Try

            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[1] = ex.Message.ToString(); //catch ex
                retResults[4] = ex.Message.ToString(); //catch ex
                if (transaction != null && Vtransaction == null) { transaction.Rollback(); }
                //return retResults;

                FileLogger.Log("TransferIssueDAL", "Post", ex.ToString());

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

            #region Result
            return retResults;
            #endregion Result
        }


        public ResultVM TransferIssue_Product_IN_OUT(ParameterVM paramVM, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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

                #region Invoice Status

                sqlText = "";
                sqlText = @"
----------declare @InvoiceNo as varchar(100)='T1O-2/0001/1220'

select TransferIssueNo, BranchId, Post from TransferIssues
where 1=1 
and TransferIssueNo=@InvoiceNo
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
----------declare @InvoiceNo as varchar(100)='T1O-2/0001/1220'


select ItemNo, TransferIssueNo, TransactionType, UOMQty * (-1) as Quantity, Post from TransferIssueDetails
where 1=1 
and TransferIssueNo=@InvoiceNo

";
                #endregion


                cmd = new SqlCommand(sqlText, currConn, transaction);
                cmd.Parameters.AddWithValue("@InvoiceNo", paramVM.InvoiceNo);

                da = new SqlDataAdapter(cmd);
                dt = new DataTable();
                da.Fill(dt);

                paramVM.dt = dt;

                rVM = _ProductDAL.Product_IN_OUT(paramVM, currConn, transaction, connVM);

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


        public string[] MultiplePost(string[] Ids, SysDBInfoVMTemp connVM = null, string UserId = null)
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

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                transaction = currConn.BeginTransaction(MessageVM.PurchasemsgMethodNameUpdate);


                #endregion open connection and transaction

                for (int i = 0; i < Ids.Length - 1; i++)
                {
                    TransferIssueVM Master = SelectAllList(Convert.ToInt32(Ids[i]), null, null, currConn, transaction, connVM).FirstOrDefault();
                    UserInformationVM userinformation = new UserInformationDAL().SelectAll(0, new[] { "ui.UserID" },
                           new[] { UserId }, null, null, connVM).FirstOrDefault();
                    Master.Post = "Y";
                    Master.IsTransfer = "N";
                    Master.SignatoryName = userinformation.FullName;
                    Master.SignatoryDesig = userinformation.Designation;
                    List<TransferIssueDetailVM> Details = SelectDetail(Master.TransferIssueNo, null, null, currConn, transaction, connVM);
                    retResults = Post(Master, Details, currConn, transaction);

                    if (retResults[0] != "Success")
                    {
                        throw new Exception();
                    }
                }

                #region Commit
                if (transaction != null)
                {
                    transaction.Commit();
                }
                #endregion

            }
            catch (Exception ex)
            {
                retResults = new string[5];
                retResults[0] = "Fail";
                retResults[1] = ex.Message;
                retResults[2] = "0";
                retResults[3] = "N";
                retResults[4] = "0";
                throw ex;
            }
            finally
            {

            }

            #region Result
            return retResults;
            #endregion Result

        }

        public string[] PostTransfer(TransferIssueVM Master, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string[] retResults = new string[5];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";
            retResults[4] = "";
            string sqlText = "";
            string PostStatus = "";
            string dbNameFrom = "";
            //////string dbNameTo = "";
            #endregion Initializ

            #region Try
            try
            {
                string SignatoryName = Master.SignatoryName;
                string SignatoryDesig = Master.SignatoryDesig;
                string[] cFields = { "TransferIssueNo" };
                string[] cVals = { Master.TransferIssueNo };
                Master = SelectAllList(0, cFields, cVals, VcurrConn, Vtransaction, connVM).FirstOrDefault();
                Master.Post = "Y";
                Master.IsTransfer = "N";
                Master.SignatoryName = SignatoryName;
                Master.SignatoryDesig = SignatoryDesig;

                List<TransferIssueDetailVM> Details = SelectDetail(Master.TransferIssueNo, null, null, VcurrConn, Vtransaction, connVM);
                retResults = Post(Master, Details, VcurrConn, Vtransaction, connVM);
                if (retResults[0] != "Success")
                {
                    throw new ArgumentNullException("", retResults[1]);
                }

                #region Comments - Nov-01-2020

                ////#region Transfer Table

                ////else
                ////{
                ////    ////PostStatus = retResults[3];
                ////    ////////////dbNameFrom = retResults[4];
                ////    ////TransferVM vTrnsMasterVM = new TransferVM();
                ////    ////BranchDAL br = new BranchDAL();
                ////    ////vTrnsMasterVM.TransferFrom = Master.BranchId; //retResults[4] = DBName;
                ////    //////////dbNameTo = Master.TransferTo;
                ////    ////Master.IssueDateFrom = "";
                ////    ////Master.IssueDateTo = "";
                ////    ////Master.Post = "";
                ////    // db name from master 
                ////    #region Read Data From Master Table

                ////    ////DataTable IssueResult = SearchTransferIssue(Master, VcurrConn, Vtransaction);
                ////    //////DataTable IssueResult =  SearchTransferIssue(Master.TransferIssueNo, "", "",Master.TransactionType, "");
                ////    ////// Master
                ////    ////vTrnsMasterVM.TransferFromNo = Master.TransferIssueNo;
                ////    ////vTrnsMasterVM.TransactionDateTime = IssueResult.Rows[0]["IssueDateTime"].ToString();
                ////    ////vTrnsMasterVM.TotalAmount = Convert.ToDecimal(IssueResult.Rows[0]["TotalAmount"].ToString());
                ////    ////vTrnsMasterVM.TransactionType = IssueResult.Rows[0]["TransactionType"].ToString();
                ////    ////vTrnsMasterVM.SerialNo = IssueResult.Rows[0]["SerialNo"].ToString();
                ////    ////vTrnsMasterVM.Comments = IssueResult.Rows[0]["Comments"].ToString();
                ////    ////vTrnsMasterVM.Post = "N";
                ////    ////vTrnsMasterVM.ReferenceNo = IssueResult.Rows[0]["ReferenceNo"].ToString();
                ////    ////vTrnsMasterVM.TransferFrom = Master.BranchId;
                ////    ////vTrnsMasterVM.BranchId = Master.TransferTo;

                ////    ////vTrnsMasterVM.TotalVATAmount = Master.TotalVATAmount;
                ////    ////vTrnsMasterVM.TotalSDAmount = Master.TotalSDAmount;

                ////    #endregion

                ////    #region Read Data From Detail Table
                ////    ////List<TransferDetailVM> vTrnsDetailVMs = new List<TransferDetailVM>();
                ////    ////TransferDetailVM vTrnsDetailVM;// = new TransferDetailVM();
                ////    ////DataTable IssueDetail = SearchTransferDetail(Master.TransferIssueNo, VcurrConn, Vtransaction);// Detail
                ////    ////foreach (DataRow item in IssueDetail.Rows)
                ////    ////{
                ////    ////    vTrnsDetailVM = new TransferDetailVM();
                ////    ////    vTrnsDetailVM.TransferNo = Master.TransferIssueNo;
                ////    ////    vTrnsDetailVM.TransferLineNo = item["IssueLineNo"].ToString();
                ////    ////    vTrnsDetailVM.ItemNo = item["ItemNo"].ToString();
                ////    ////    vTrnsDetailVM.Quantity = Convert.ToDecimal(item["Quantity"].ToString());
                ////    ////    vTrnsDetailVM.CostPrice = Convert.ToDecimal(item["CostPrice"].ToString());
                ////    ////    vTrnsDetailVM.UOM = item["UOM"].ToString();
                ////    ////    vTrnsDetailVM.SubTotal = Convert.ToDecimal(item["SubTotal"].ToString());
                ////    ////    vTrnsDetailVM.Comments = item["Comments"].ToString();
                ////    ////    vTrnsDetailVM.TransactionType = item["TransactionType"].ToString();
                ////    ////    vTrnsDetailVM.TransactionDateTime = item["IssueDateTime"].ToString();
                ////    ////    vTrnsDetailVM.Post = "N";
                ////    ////    vTrnsDetailVM.TransferFrom = Master.BranchId;
                ////    ////    vTrnsDetailVM.BranchId = Convert.ToInt32(item["TransferTo"]);
                ////    ////    vTrnsDetailVM.UOMQty = Convert.ToDecimal(item["UOMQty"].ToString());
                ////    ////    vTrnsDetailVM.UOMn = item["UOMn"].ToString();
                ////    ////    vTrnsDetailVM.UOMc = Convert.ToDecimal(item["UOMc"].ToString());
                ////    ////    vTrnsDetailVM.UOMPrice = Convert.ToDecimal(item["UOMPrice"].ToString());

                ////    ////    vTrnsDetailVM.VATRate = Convert.ToDecimal(item["VATRate"].ToString());
                ////    ////    vTrnsDetailVM.VATAmount = Convert.ToDecimal(item["VATAmount"].ToString());
                ////    ////    vTrnsDetailVM.SDRate = Convert.ToDecimal(item["SDRate"].ToString());
                ////    ////    vTrnsDetailVM.SDAmount = Convert.ToDecimal(item["SDAmount"].ToString());



                ////    ////    vTrnsDetailVMs.Add(vTrnsDetailVM);
                ////    ////}
                ////    #endregion

                ////    #region Insert Data Into Transfer Table
                ////    //TransferDAL _dal = new TransferDAL();
                ////    //retResults = _dal.Insert(vTrnsMasterVM, vTrnsDetailVMs,VcurrConn,Vtransaction);
                ////    //if (retResults[0] != "Success")
                ////    //{
                ////    //    Master.Post = "N";
                ////    //    retResults = Post(Master, Details,VcurrConn,Vtransaction);
                ////    //}
                ////    #endregion

                ////}

                ////#endregion

                #endregion

                #region SuccessResult
                retResults[0] = "Success";
                retResults[1] = MessageVM.issueMsgSuccessfullyPost;
                retResults[2] = Master.TransferIssueNo;
                //////retResults[3] = PostStatus;
                retResults[3] = retResults[3];
                #endregion SuccessResult

            }
            #endregion Try

            #region Catch and Finall
            //catch (SqlException sqlex)
            //{
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
                //////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                throw new ArgumentNullException("", ex.Message.ToString());
                //throw ex;
            }
            finally
            {

            }
            #endregion Catch and Finall
            #region Result
            return retResults;
            #endregion Result
        }

        public ResultVM Multiple_Post(IntegrationParam paramVM, SqlTransaction Vtransaction = null, SqlConnection VcurrConn = null, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            ResultVM rVM = new ResultVM();
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

                #region User Information

                UserInformationDAL dal = new UserInformationDAL();

                UserInformationVM user = dal.SelectAll(Convert.ToInt32(paramVM.CurrentUser)).FirstOrDefault();

                #endregion

                #region Post

                string IDs = "";
                if (paramVM.IDs != null && paramVM.IDs.Count > 0)
                {

                    IDs = string.Join("','", paramVM.IDs);

                    sqlText = "";
                    sqlText += @" Update  TransferIssues                set SignatoryName=@SignatoryName,SignatoryDesig=@SignatoryDesig, Post ='Y',LastModifiedBy =@LastModifiedBy, LastModifiedOn =@LastModifiedOn, IsTransfer='N'    WHERE TransferIssueNo IN('" + IDs + "')";
                    sqlText += @" Update  TransferIssueDetails          set  Post ='Y',LastModifiedBy =@LastModifiedBy, LastModifiedOn =@LastModifiedOn                    WHERE TransferIssueNo IN('" + IDs + "')";

                    SqlCommand cmdPost = new SqlCommand(sqlText, currConn);
                    cmdPost.Transaction = transaction;

                    string LastModifiedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");

                    cmdPost.Parameters.AddWithValueAndNullHandle("@LastModifiedOn", LastModifiedOn);
                    cmdPost.Parameters.AddWithValueAndNullHandle("@LastModifiedBy", paramVM.CurrentUser);

                    if (user != null)
                    {
                        cmdPost.Parameters.AddWithValueAndNullHandle("@SignatoryDesig", user.Designation);
                        cmdPost.Parameters.AddWithValueAndNullHandle("@SignatoryName", user.FullName);
                    }
                    else
                    {
                        cmdPost.Parameters.AddWithValueAndNullHandle("@SignatoryDesig", "");
                        cmdPost.Parameters.AddWithValueAndNullHandle("@SignatoryName", "");
                    }

                    transResult = cmdPost.ExecuteNonQuery();

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
                rVM.Message = MessageVM.saleMsgSuccessfullyPost;
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

        #region Plain Methods

        public TransferIssueDetailVM TransferIssuePriceCal(TransferIssueDetailVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {

            #region Initializ

            ProductDAL productDal = new ProductDAL();

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            string sqlText = "";

            CommonImportDAL cImport = new CommonImportDAL();
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

                bool uomDefault = commonDal.settings("TransferIssue", "AutoUOM", currConn, transaction, connVM) == "Y";

                #region Insert into Details

                string uomn = "";
                decimal uomc = 1;

                string[] conditionalFields = new string[] { "Pr.ItemNo" };
                string[] conditionalValues = new string[] { vm.ItemNo };
                ProductVM pvm = new ProductDAL().SelectAll("0", conditionalFields, conditionalValues, currConn, transaction, null, connVM).FirstOrDefault();

                uomn = pvm.UOM;

                if (uomDefault)
                {
                    uomc = 1;
                }
                else
                {

                    if (uomn.ToLower() == vm.UOM.ToLower())
                    {
                        uomc = 1;
                    }
                    else
                    {
                        uomc = Convert.ToDecimal(cImport.FindUOMc(vm.UOMn, vm.UOM, currConn, transaction, connVM, vm.ItemNo));
                    }
                }


                vm.UOMn = uomn;
                vm.UOMc = uomc;
                vm.UOMQty = vm.Quantity * uomc;
                vm.UOMPrice = vm.CostPrice;
                vm.CostPrice = vm.CostPrice * uomc;
                vm.SubTotal = vm.CostPrice * vm.Quantity;
                vm.SDAmount = vm.SubTotal * vm.SDRate / 100;
                vm.VATAmount = (vm.SubTotal + vm.SDAmount) * vm.VATRate / 100;

                #endregion Insert into Details(Insert complete in Header)

            }
            #endregion Try

            #region Catch and Finall

            catch (Exception ex)
            {

                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
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

            return vm;

            #endregion Result

        }

        public string[] TransferIssueInsertToMaster(TransferIssueVM Master, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {

            #region Initializ
            string[] retResults = new string[5];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";
            retResults[4] = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            int transResult = 0;
            string sqlText = "";
            //string PostStatus = "";
            string newID = "";
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
                sqlText += "  insert into TransferIssues";
                sqlText += " (";
                sqlText += " TransferIssueNo";
                sqlText += " ,TransactionDateTime";
                sqlText += " ,TotalAmount";
                sqlText += " ,TransactionType";
                sqlText += " ,SerialNo";
                sqlText += " ,Comments";
                sqlText += " ,Post";
                sqlText += " ,ReferenceNo";
                sqlText += " ,TransferTo";
                sqlText += " ,BranchId";

                sqlText += " ,TotalVATAmount";
                sqlText += " ,TotalSDAmount";
                sqlText += " ,VehicleNo";
                sqlText += " ,VehicleType";
                sqlText += " ,TripNo";

                sqlText += " ,CreatedBy";
                sqlText += " ,CreatedOn";
                sqlText += " ,LastModifiedOn";
                sqlText += " ,LastModifiedBy";
                sqlText += " ,ImportIDExcel";
                sqlText += " ,ShiftId";
                sqlText += " ,BranchToRef";
                sqlText += " ,BranchFromRef";
                sqlText += " ,SignatoryName";
                sqlText += " ,SignatoryDesig";

                sqlText += " )";

                sqlText += " values";
                sqlText += " (";
                sqlText += "@Master_TransferIssueNo";
                sqlText += ",@Master_TransactionDateTime";
                sqlText += ",@Master_TotalAmount";
                sqlText += ",@Master_TransactionType";
                sqlText += ",@Master_SerialNo";
                sqlText += ",@Master_Comments";
                sqlText += ",@Master_Post";
                sqlText += ",@Master_ReferenceNo";
                sqlText += ",@Master_TransferTo";
                sqlText += ",@BranchId";

                sqlText += ",@TotalVATAmount";
                sqlText += ",@TotalSDAmount";
                sqlText += ",@VehicleNo";
                sqlText += ",@VehicleType";
                sqlText += ",@TripNo";

                sqlText += ",@Master_CreatedBy";
                sqlText += ",@Master_CreatedOn";
                sqlText += ",@Master_LastModifiedOn";
                sqlText += ",@Master_LastModifiedBy";
                sqlText += ",@ImportIDExcel";
                sqlText += ",@ShiftId";
                sqlText += ",@BranchToRef";
                sqlText += ",@BranchFromRef";
                sqlText += ",@SignatoryName";
                sqlText += ",@SignatoryDesig";

                sqlText += ")  SELECT SCOPE_IDENTITY() ";


                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;

                cmdInsert.Parameters.AddWithValueAndNullHandle("@Master_TransferIssueNo", Master.TransferIssueNo);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Master_TransactionDateTime", OrdinaryVATDesktop.DateToDate(Master.TransactionDateTime));
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Master_TotalAmount", Master.TotalAmount);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Master_TransactionType", Master.TransactionType);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Master_SerialNo", Master.SerialNo);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Master_Comments", Master.Comments);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Master_Post", Master.Post);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Master_ReferenceNo", Master.ReferenceNo);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Master_TransferTo", Master.TransferTo);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@BranchId", Master.BranchId);

                cmdInsert.Parameters.AddWithValueAndNullHandle("@TotalVATAmount", Master.TotalVATAmount);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@TotalSDAmount", Master.TotalSDAmount);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@VehicleNo", Master.VehicleNo);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@VehicleType", Master.VehicleType);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@TripNo", Master.TripNo);


                cmdInsert.Parameters.AddWithValueAndNullHandle("@Master_CreatedBy", Master.CreatedBy);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Master_CreatedOn", OrdinaryVATDesktop.DateToDate(Master.CreatedOn));
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Master_LastModifiedBy", Master.LastModifiedBy);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Master_LastModifiedOn", OrdinaryVATDesktop.DateToDate(Master.LastModifiedOn));
                cmdInsert.Parameters.AddWithValueAndNullHandle("@ImportIDExcel", Master.ImportExcelId);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@ShiftId", Master.ShiftId);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@BranchToRef", Master.BranchToRef);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@BranchFromRef", Master.BranchFromRef);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@SignatoryName", Master.SignatoryName);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@SignatoryDesig", Master.SignatoryDesig);


                transResult = Convert.ToInt32(cmdInsert.ExecuteScalar());
                retResults[4] = transResult.ToString();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, MessageVM.issueMsgSaveNotSuccessfully);
                }
                #region Update  Receive header for trip
                if (!string.IsNullOrEmpty(Master.TripNo) && (Master.TripNo) != "-")
                {
                    sqlText = @"
                     update ReceiveHeaders set IsTripComplete = 'Y'
                       where ReferenceNo=@TripNo";
                    SqlCommand cmdRecieveUpdate = new SqlCommand(sqlText, currConn);
                    cmdRecieveUpdate.Transaction = transaction;
                    cmdRecieveUpdate.Parameters.AddWithValueAndNullHandle("@TripNo", Master.TripNo);

                    transResult = cmdRecieveUpdate.ExecuteNonQuery();
                    //if (transResult <= 0)
                    //{
                    //    throw new ArgumentNullException("", "Unable to Update ReceiveHeaders in Insert");
                    //}
                }

                #endregion

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
                retResults[1] = "Requested insert into TransferIssues successfully Added";
                retResults[2] = Master.TransferIssueNo;
                //retResults[3] = "" + PostStatus;
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
                if (Vtransaction == null)
                {

                    transaction.Rollback();
                }

                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
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

        public string[] TransferIssueInsertToDetail(TransferIssueDetailVM Detail, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {

            #region Initializ
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";
            ProductDAL productDal = new ProductDAL();

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            int transResult = 0;
            string sqlText = "";
            string newID = "";
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

                #region Insert into Details(Insert complete in Header)

                #region Insert Detail Table

                #region Insert only DetailTable TransferIssueDetails

                sqlText = "";
                sqlText += " insert into TransferIssueDetails(";
                sqlText += " TransferIssueNo";
                sqlText += ",BOMId";
                sqlText += ",IssueLineNo";
                sqlText += ",ItemNo";
                sqlText += ",Quantity";
                sqlText += ",CostPrice";
                sqlText += ",UOM";
                sqlText += ",SubTotal";
                sqlText += ",Comments";
                sqlText += ",TransactionType";
                sqlText += ",TransactionDateTime";
                sqlText += ",TransferTo";
                sqlText += ",BranchId";
                sqlText += ",Post";
                sqlText += ",UOMQty";
                sqlText += ",UOMPrice";
                sqlText += ",UOMc";
                sqlText += ",UOMn";
                sqlText += ",VATRate";
                sqlText += ",VATAmount";
                sqlText += ",SDRate ";
                sqlText += ",SDAmount";
                sqlText += ",Weight";
                sqlText += ",OtherRef";

                sqlText += ",CreatedBy";
                sqlText += ",CreatedOn";
                sqlText += ",LastModifiedBy";
                sqlText += ",LastModifiedOn";

                sqlText += " )";
                sqlText += " values(	";
                sqlText += "@Detail_TransferIssueNo";
                sqlText += ",@BOMId";
                sqlText += ",@Detail_IssueLineNo";
                sqlText += ",@Detail_ItemNo";
                sqlText += ",@Detail_Quantity";
                sqlText += ",@Detail_CostPrice";
                sqlText += ",@Detail_UOM";
                sqlText += ",@Detail_SubTotal";
                sqlText += ",@Detail_Comments";
                sqlText += ",@Detail_TransactionType";
                sqlText += ",@Detail_TransactionDateTime";
                sqlText += ",@Detail_TransferTo";
                sqlText += ",@BranchId";
                sqlText += ",@Detail_Post";
                sqlText += ",@Detail_UOMQty";
                sqlText += ",@Detail_UOMPrice";
                sqlText += ",@Detail_UOMc";
                sqlText += ",@Detail_UOMn";
                sqlText += ",@VATRate";
                sqlText += ",@VATAmount";
                sqlText += ",@SDRate ";
                sqlText += ",@SDAmount";
                sqlText += ",@Weight";
                sqlText += ",@OtherRef";

                sqlText += ",@Detail_CreatedBy";
                sqlText += ",@Detail_CreatedOn";
                sqlText += ",@Detail_LastModifiedBy";
                sqlText += ",@Detail_LastModifiedOn";


                sqlText += ")	";


                SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                cmdInsDetail.Transaction = transaction;

                Detail = TransferIssuePriceCal(Detail, currConn, transaction, connVM);

                ////string uomn = "";
                ////decimal uomc = 1;

                ////Detail.UOMn = uomn;
                ////Detail.UOMc = uomc;
                ////Detail.UOMQty = Detail.Quantity * uomc;
                ////Detail.UOMPrice = Detail.CostPrice;
                ////Detail.CostPrice = Detail.CostPrice * uomc;
                ////Detail.SubTotal = Detail.CostPrice * Detail.Quantity;
                ////Detail.SDAmount = Detail.SubTotal * Detail.SDRate / 100;
                ////Detail.VATAmount = (Detail.SubTotal + Detail.SDAmount) * Detail.VATRate / 100;

                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@Detail_TransferIssueNo", Detail.TransferIssueNo);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BOMId", Detail.BOMId);

                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@Detail_IssueLineNo", Detail.IssueLineNo);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@Detail_ItemNo", Detail.ItemNo);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@Detail_Quantity", Detail.Quantity);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@Detail_UOM", Detail.UOM);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@Detail_SubTotal", Detail.SubTotal);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@Detail_Comments", Detail.Comments);

                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@Detail_Post", Detail.Post);

                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@Detail_CostPrice", Detail.CostPrice);

                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@Detail_UOMQty", Detail.UOMQty);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@Detail_UOMPrice", Detail.UOMPrice);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@Detail_UOMc", Detail.UOMc);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@Detail_UOMn", Detail.UOMn);

                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@Detail_TransactionType", Detail.TransactionType);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@Detail_TransactionDateTime", OrdinaryVATDesktop.DateToDate(Detail.TransactionDateTime));
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@Detail_TransferTo", Detail.TransferTo);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BranchId", Detail.BranchId);

                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@VATRate", Detail.VATRate);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@VATAmount", Detail.VATAmount);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@SDRate ", Detail.SDRate);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@SDAmount", Detail.SDAmount);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@Weight", Detail.Weight);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@OtherRef", Detail.OtherRef);

                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@Detail_CreatedBy", Detail.CreatedBy);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@Detail_CreatedOn", OrdinaryVATDesktop.DateToDate(Detail.CreatedOn));
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@Detail_LastModifiedBy", Detail.LastModifiedBy);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@Detail_LastModifiedOn", OrdinaryVATDesktop.DateToDate(Detail.LastModifiedOn));


                transResult = (int)cmdInsDetail.ExecuteNonQuery();

                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                    MessageVM.PurchasemsgSaveNotSuccessfully);
                }

                #endregion Insert only DetailTable

                #endregion Insert Detail Table

                #endregion Insert into Details(Insert complete in Header)

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
                retResults[1] = "Requested insert into TransferIssueDetails successfully Added";
                retResults[2] = Detail.TransferIssueNo;
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
                if (Vtransaction == null)
                {

                    transaction.Rollback();
                }

                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
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

        public string[] TransferIssueUpdateToMaster(TransferIssueVM Master, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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
                sqlText += " update TransferIssues set  ";

                sqlText += " TransactionDateTime  =@TransactionDateTime";
                sqlText += " ,TotalAmount         =@TotalAmount";
                sqlText += " ,TransactionType     =@TransactionType";
                sqlText += " ,SerialNo            =@SerialNo";
                sqlText += " ,Comments            =@Comments";
                sqlText += " ,ReferenceNo         =@ReferenceNo";
                sqlText += " ,TransferTo          =@TransferTo";
                sqlText += " ,Post                =@Post";
                sqlText += " ,ShiftId             =@ShiftId";

                sqlText += " ,TotalVATAmount      =@TotalVATAmount";
                sqlText += " ,TotalSDAmount       =@TotalSDAmount";
                sqlText += " ,VehicleNo           =@VehicleNo";
                sqlText += " ,VehicleType         =@VehicleType";
                sqlText += " ,SignatoryName       =@SignatoryName";
                sqlText += " ,SignatoryDesig      =@SignatoryDesig";



                sqlText += " ,CreatedBy           =@CreatedBy";
                sqlText += " ,CreatedOn           =@CreatedOn";
                sqlText += " ,LastModifiedBy      =@LastModifiedBy";
                sqlText += " ,LastModifiedOn      =@LastModifiedOn";

                sqlText += " where              TransferIssueNo=@TransferIssueNo";


                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;

                cmdUpdate.Parameters.AddWithValueAndNullHandle("@TransferIssueNo", Master.TransferIssueNo);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@TransactionDateTime", OrdinaryVATDesktop.DateToDate(Master.TransactionDateTime));
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@TotalAmount", Master.TotalAmount);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@TransactionType", Master.TransactionType);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@SerialNo", Master.SerialNo);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@Comments", Master.Comments);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@ReferenceNo", Master.ReferenceNo);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@TransferTo", Master.TransferTo);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@Post", Master.Post);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@ShiftId", Master.ShiftId);

                cmdUpdate.Parameters.AddWithValueAndNullHandle("@TotalVATAmount", Master.TotalVATAmount);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@TotalSDAmount", Master.TotalSDAmount);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@VehicleNo", Master.VehicleNo);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@VehicleType", Master.VehicleType);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@SignatoryName", Master.SignatoryName);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@SignatoryDesig", Master.SignatoryDesig);



                cmdUpdate.Parameters.AddWithValueAndNullHandle("@CreatedBy", Master.CreatedBy);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@CreatedOn", OrdinaryVATDesktop.DateToDate(Master.CreatedOn));
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@LastModifiedBy", Master.LastModifiedBy);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@LastModifiedOn", OrdinaryVATDesktop.DateToDate(Master.LastModifiedOn));



                transResult = (int)cmdUpdate.ExecuteNonQuery();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, MessageVM.issueMsgUpdateNotSuccessfully);
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
                retResults[1] = "Requested update into TransferIssues successfully updated ";
                retResults[2] = "" + Master.TransferIssueNo;
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
                if (Vtransaction == null)
                {

                    transaction.Rollback();
                }

                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
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

        public string[] TransferIssueUpdateToDetail(TransferIssueDetailVM Detail, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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
                sqlText += " update TransferIssueDetails set  ";

                sqlText += " IssueLineNo               =@IssueLineNo";
                sqlText += " ,BranchId                 =@BranchId";
                sqlText += " ,BOMId                    =@BOMId";
                sqlText += " ,Quantity                 =@Quantity";
                sqlText += " ,CostPrice                =@CostPrice";
                sqlText += " ,UOM                      =@UOM";
                sqlText += " ,SubTotal                 =@SubTotal";
                sqlText += " ,Comments                 =@Comments";
                sqlText += " ,TransactionType          =@TransactionType";
                sqlText += " ,TransactionDateTime      =@TransactionDateTime";
                sqlText += " ,TransferTo               =@TransferTo";
                sqlText += " ,Post                     =@Post";
                sqlText += " ,UOMQty                   =@UOMQty";
                sqlText += " ,UOMPrice                 =@UOMPrice";
                sqlText += " ,UOMc                     =@UOMc";
                sqlText += " ,UOMn                     =@UOMn";

                sqlText += " ,VATRate                  =@VATRate";
                sqlText += " ,VATAmount                =@VATAmount";
                sqlText += " ,SDRate                   =@SDRate ";
                sqlText += " ,SDAmount                 =@SDAmount";
                sqlText += " ,Weight                   =@Weight";

                sqlText += " ,CreatedBy                =@CreatedBy";
                sqlText += " ,CreatedOn                =@CreatedOn";
                sqlText += " ,LastModifiedBy           =@LastModifiedBy";
                sqlText += " ,LastModifiedOn           =@LastModifiedOn";

                sqlText += " where      TransferIssueNo=@TransferIssueNo";
                sqlText += "  and                ItemNo=@ItemNo";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;

                Detail = TransferIssuePriceCal(Detail, currConn, transaction, connVM);


                cmdUpdate.Parameters.AddWithValueAndNullHandle("@IssueLineNo", Detail.IssueLineNo);

                cmdUpdate.Parameters.AddWithValueAndNullHandle("@BranchId", Detail.BranchId);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@BOMId", Detail.BOMId);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@Quantity", Detail.Quantity);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@CostPrice", Detail.CostPrice);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@UOM", Detail.UOM);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@SubTotal", Detail.SubTotal);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@Comments", Detail.Comments);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@TransactionType", Detail.TransactionType);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@TransactionDateTime", OrdinaryVATDesktop.DateToDate(Detail.TransactionDateTime));
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@TransferTo", Detail.TransferTo);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@Post", Detail.Post);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@UOMQty", Detail.UOMQty);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@UOMPrice", Detail.UOMPrice);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@UOMc", Detail.UOMc);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@UOMn", Detail.UOMn);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@TransferIssueNo", Detail.TransferIssueNo);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@ItemNo", Detail.ItemNo);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@VATRate", Detail.VATRate);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@VATAmount", Detail.VATAmount);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@SDRate ", Detail.SDRate);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@SDAmount", Detail.SDAmount);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@Weight", Detail.Weight);

                cmdUpdate.Parameters.AddWithValueAndNullHandle("@CreatedBy", Detail.CreatedBy);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@CreatedOn", OrdinaryVATDesktop.DateToDate(Detail.CreatedOn));
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@LastModifiedBy", Detail.LastModifiedBy);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@LastModifiedOn", OrdinaryVATDesktop.DateToDate(Detail.LastModifiedOn));

                transResult = (int)cmdUpdate.ExecuteNonQuery();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, MessageVM.issueMsgUpdateNotSuccessfully);
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
                retResults[1] = "Requested update into TransferIssueDetails successfully updated ";
                retResults[2] = Detail.TransferIssueNo;
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
                if (Vtransaction == null)
                {

                    transaction.Rollback();
                }

                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
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

        //public DataTable SearchTransferIssue(string TransferIssueNo, string DateTimeFrom,
        //    string DateTimeTo,  string TransactionType, string Post)

        #region Search Methods

        public DataTable SearchTransferIssue(TransferIssueVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            DataTable dataTable = new DataTable("SearchTransferIssue");
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
                sqlText = " ";
                sqlText = @" 
SELECT 
ti.TransferIssueNo
,ti.Id
,ISNULL(ti.FiscalYear,0) FiscalYear
, convert (varchar, ti.TransactionDateTime,120)IssueDateTime
,isnull(ti.TotalAmount     ,'0')TotalAmount
,isnull(ti.TransactionType ,'N/A')TransactionType
,isnull(ti.SerialNo        ,'N/A')SerialNo
,isnull(ti.Comments        ,'N/A')Comments
,isnull(ti.Post            ,'N')Post
,isnull(ti.ReferenceNo     ,'N/A')ReferenceNo
,isnull(ti.TransferTo      ,0)TransferTo
,isnull(ti.BranchId      ,0)BranchId
,isnull(ti.IsTransfer      ,0) IsTransfer 
,isnull(br.BranchName,'N/A') BranchName
,isnull(bp.BranchName,'N/A') BranchNameFrom

,isnull(ti.TotalVATAmount       ,0)TotalVATAmount
,isnull(ti.TotalSDAmount       ,0)TotalSDAmount
,isnull(ti.TripNo,'')TripNo
,isnull(ti.VehicleNo,'N/A')VehicleNo
,isnull(ti.VehicleType,'N/A')VehicleType
,isnull(ti.ShiftId     ,'0')ShiftId


,isnull(ti.CreatedBy       ,'N/A')CreatedBy
,isnull(ti.CreatedOn       ,'19900101')CreatedOn
,isnull(ti.LastModifiedBy  ,'N/A')LastModifiedBy
,isnull(ti.LastModifiedOn  ,'19900101')LastModifiedOn
FROM         dbo.TransferIssues ti
LEFT OUTER JOIN BranchProfiles br on ti.TransferTo = br.BranchID
LEFT OUTER JOIN BranchProfiles bp on ti.BranchId = bp.BranchID
                            WHERE 1=1
                            AND (ti.TransferIssueNo  LIKE '%' +  @TransferIssueNo   + '%' OR @TransferIssueNo IS NULL) 
                            AND (ti.TransactionDateTime>= @DateTimeFrom OR @DateTimeFrom IS NULL)
                            AND (ti.TransactionDateTime <dateadd(d,1, @DateTimeTo) OR @DateTimeTo IS NULL)
                            AND (ti.Post  LIKE '%' +  @Post   + '%' OR @Post IS NULL) 
                            AND  ti.TransferTo =@TransferTo  
                            ";

                if (!string.IsNullOrEmpty(vm.TransactionType))
                {
                    sqlText += "  AND (ti.transactionType=@transactionType)";
                }

                if (vm.BranchId > 0)
                {
                    sqlText = sqlText + @" AND ti.BranchId=@BranchId";
                }
                if (!string.IsNullOrEmpty(vm.ReferenceNo))
                {
                    sqlText += "  AND (ti.ReferenceNo=@ReferenceNo)";
                }
                if (vm.TransferTo == 0)
                {
                    sqlText = sqlText.Replace("=@TransferTo", ">@TransferTo");
                }
                if (!string.IsNullOrEmpty(vm.VehicleNo))
                {
                    sqlText += "  AND (ti.VehicleNo  LIKE '%' +  @VehicleNo   + '%') ";
                }

                sqlText += @"  order by ti.TransferIssueNo desc";
                #endregion
                #region SQL Command
                SqlCommand objCommHeader = new SqlCommand();
                objCommHeader.Connection = currConn;
                objCommHeader.Transaction = transaction;
                objCommHeader.CommandText = sqlText;
                objCommHeader.CommandType = CommandType.Text;
                #endregion


                #region Parameter
                if (!objCommHeader.Parameters.Contains("@Post"))
                { objCommHeader.Parameters.AddWithValue("@Post", vm.Post); }
                else { objCommHeader.Parameters["@Post"].Value = vm.Post; }


                if (!objCommHeader.Parameters.Contains("@TransferIssueNo"))
                { objCommHeader.Parameters.AddWithValue("@TransferIssueNo", vm.TransferIssueNo); }
                else { objCommHeader.Parameters["@TransferIssueNo"].Value = vm.TransferIssueNo; }


                if (vm.IssueDateFrom == "")
                {
                    if (!objCommHeader.Parameters.Contains("@DateTimeFrom"))
                    { objCommHeader.Parameters.AddWithValue("@DateTimeFrom", "1900-01-01"); }
                    else { objCommHeader.Parameters["@DateTimeFrom"].Value = "1900-01-01"; }
                }
                else
                {
                    if (!objCommHeader.Parameters.Contains("@DateTimeFrom"))
                    { objCommHeader.Parameters.AddWithValue("@DateTimeFrom", vm.IssueDateFrom); }
                    else { objCommHeader.Parameters["@DateTimeFrom"].Value = vm.IssueDateFrom; }
                }
                if (vm.IssueDateTo == "")
                {
                    if (!objCommHeader.Parameters.Contains("@DateTimeTo"))
                    { objCommHeader.Parameters.AddWithValue("@DateTimeTo", "9000-12-31"); }
                    else { objCommHeader.Parameters["@DateTimeTo"].Value = "9000-12-31"; }
                }
                else
                {
                    if (!objCommHeader.Parameters.Contains("@DateTimeTo"))
                    { objCommHeader.Parameters.AddWithValue("@DateTimeTo", vm.IssueDateTo); }
                    else { objCommHeader.Parameters["@DateTimeTo"].Value = vm.IssueDateTo; }
                }
                if (!objCommHeader.Parameters.Contains("@transactionType"))
                { objCommHeader.Parameters.AddWithValue("@transactionType", vm.TransactionType); }
                else { objCommHeader.Parameters["@transactionType"].Value = vm.TransactionType; }

                if (!objCommHeader.Parameters.Contains("@ReferenceNo"))
                { objCommHeader.Parameters.AddWithValue("@ReferenceNo", vm.ReferenceNo); }
                else { objCommHeader.Parameters["@ReferenceNo"].Value = vm.ReferenceNo; }

                if (!objCommHeader.Parameters.Contains("@TransferTo"))
                { objCommHeader.Parameters.AddWithValue("@TransferTo", vm.TransferTo); }
                else { objCommHeader.Parameters["@TransferTo"].Value = vm.TransferTo; }

                if (vm.BranchId > 0)
                {
                    objCommHeader.Parameters.AddWithValue("@BranchId", vm.BranchId);
                }
                if (!string.IsNullOrEmpty(vm.VehicleNo))
                {
                    objCommHeader.Parameters.AddWithValue("@VehicleNo", vm.VehicleNo);
                }

                //if (!objCommHeader.Parameters.Contains("@VehicleNo"))
                //{ 
                //    objCommHeader.Parameters.AddWithValue("@VehicleNo", vm.VehicleNo); 
                //}
                //else 
                //{ 
                //    objCommHeader.Parameters["@VehicleNo"].Value = vm.VehicleNo; 
                //}


                #endregion Parameter
                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommHeader);
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
                if (Vtransaction == null)
                {

                    transaction.Rollback();
                }

                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
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
            return dataTable;
        }

        public DataTable SearchTransferIssue_TempTable(TransferIssueVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            DataTable dataTable = new DataTable("SearchTransferIssue");
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
                sqlText = " ";
                sqlText = @" 
SELECT 
''TransferIssueNo
,ti.Id
, convert (varchar, ti.TransactionDateTime,120)IssueDateTime
,'0' TotalAmount
,isnull(ti.TransactionType ,'N/A')TransactionType
,'N/A'SerialNo
,isnull(ti.Comments        ,'N/A')Comments
,isnull(ti.Post            ,'N')Post
,isnull(ti.ReferenceNo     ,'N/A')ReferenceNo
,isnull(ti.TransferToBranchId      ,0)TransferTo
,isnull(ti.BranchId      ,0)BranchId
,'N' IsTransfer 
,isnull(br.BranchName,'N/A') BranchName
,isnull(bp.BranchName,'N/A') BranchNameFrom

,0 TotalVATAmount
,     0TotalSDAmount
,''TripNo
,isnull(ti.VehicleNo,'N/A')VehicleNo
,'0' ShiftId


FROM  TempTransferData ti
LEFT OUTER JOIN BranchProfiles br on ti.TransferToBranchId = br.BranchID
LEFT OUTER JOIN BranchProfiles bp on ti.BranchId = bp.BranchID
where ti.ID =@ID  and ti.UserId = @userId
                            ";


                #endregion
                #region SQL Command
                SqlCommand objCommHeader = new SqlCommand();
                objCommHeader.Connection = currConn;
                objCommHeader.Transaction = transaction;
                objCommHeader.CommandText = sqlText;
                objCommHeader.CommandType = CommandType.Text;
                #endregion
                #region Parameter

                objCommHeader.Parameters.AddWithValue("@Id", vm.ReferenceNo);
                objCommHeader.Parameters.AddWithValue("@userId", vm.UserId);


                #endregion Parameter
                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommHeader);
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
            #region Catch and Finall
            catch (Exception ex)
            {
                if (Vtransaction == null)
                {

                    transaction.Rollback();
                }

                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
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
            return dataTable;
        }

        public DataTable SearchTransfer(TransferVM vm, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            DataTable dataTable = new DataTable("SearchTransfer");
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
                #region Add BOMId
                CommonDAL commonDal = new CommonDAL();
                #endregion Add BOMId
                #endregion open connection and transaction
                #region SQL Statement
                sqlText = " ";

                sqlText = @"SELECT 

t.TransferIssueNo TransferFromNo
, convert (varchar,t.TransactionDateTime,120)TransferDateTime
,isnull(t.TotalAmount     ,'0')TotalAmount
--,isnull(t.TransactionType ,'N/A')TransactionType
,case when TransactionType='61Out' then '6.1 Out' else '6.2 Out' end TransactionType

,isnull(t.SerialNo        ,'N/A')SerialNo
,isnull(t.Comments        ,'N/A')Comments
,isnull(t.Post            ,'N')Post
,isnull(t.IsTransfer            ,'N')IsTransfer
,isnull(t.ReferenceNo     ,'N/A')ReferenceNo

,isnull(br.BranchName, 'N/A') BranchName
,isnull(t.TransferTo      ,0)BranchId
,isnull(t.TripNo,'')TripNo
,isnull(t.VehicleNo,'N/A')VehicleNo


,isnull(t.TotalVATAmount       ,0)TotalVATAmount
,isnull(t.TotalSDAmount       ,0)TotalSDAmount

,isnull(t.CreatedBy       ,'N/A')CreatedBy
,isnull(t.CreatedOn       ,'19900101')CreatedOn
,isnull(t.LastModifiedBy  ,'N/A')LastModifiedBy
,isnull(t.LastModifiedOn  ,'19900101')LastModifiedOn
                            FROM        TransferIssues t
LEFT OUTER JOIN BranchProfiles br on t.BranchId = br.BranchId
                            WHERE 1=1 AND t.IsTransfer = 'N'

                            AND (t.TransferIssueNo  LIKE '%' +  @TransferFromNo   + '%' OR @TransferFromNo IS NULL) 
                            AND (t.TransactionDateTime>= @DateTimeFrom OR @DateTimeFrom IS NULL)
                            AND (t.TransactionDateTime <dateadd(d,1, @DateTimeTo) OR @DateTimeTo IS NULL)
                            AND (t.Post  LIKE '%' +  @Post   + '%' OR @Post IS NULL) 
                            AND (t.transactionType  LIKE '%' +  @transactionType   + '%' OR @transactionType IS NULL) 
                            AND (t.VehicleNo  LIKE '%' +  @VehicleNo   + '%' OR @VehicleNo IS NULL) 
                            AND  br.BranchId =@TransferFrom ";


                if (!string.IsNullOrEmpty(vm.ReferenceNo))
                {
                    sqlText = sqlText + @" AND (t.ReferenceNo  LIKE '%' +  @ReferenceNo   + '%') ";
                }
                if (vm.BranchId > 0)
                {
                    // sqlText = sqlText+@" AND t.BranchId = @BranchId";

                    sqlText = sqlText + @" AND t.TransferTo = @BranchId";
                }
                if (vm.TransferFrom == 0)
                {
                    sqlText = sqlText.Replace("=@TransferFrom", ">@TransferFrom");
                }

                #endregion
                #region SQL Command
                SqlCommand objCommHeader = new SqlCommand();
                objCommHeader.Connection = currConn;
                objCommHeader.CommandText = sqlText;
                objCommHeader.CommandType = CommandType.Text;
                #endregion
                #region Parameter
                if (!objCommHeader.Parameters.Contains("@Post"))
                { objCommHeader.Parameters.AddWithValue("@Post", vm.Post); }
                else { objCommHeader.Parameters["@Post"].Value = vm.Post; }




                if (!objCommHeader.Parameters.Contains("@TransferNo"))
                { objCommHeader.Parameters.AddWithValue("@TransferNo", vm.TransferNo); }
                else { objCommHeader.Parameters["@TransferNo"].Value = vm.TransferNo; }

                if (!objCommHeader.Parameters.Contains("@TransferFromNo"))
                { objCommHeader.Parameters.AddWithValue("@TransferFromNo", vm.TransferFromNo); }
                else { objCommHeader.Parameters["@TransferFromNo"].Value = vm.TransferFromNo; }

                if (vm.DateTimeFrom == "")
                {
                    if (!objCommHeader.Parameters.Contains("@DateTimeFrom"))
                    { objCommHeader.Parameters.AddWithValue("@DateTimeFrom", System.DBNull.Value); }
                    else { objCommHeader.Parameters["@DateTimeFrom"].Value = System.DBNull.Value; }
                }
                else
                {
                    if (!objCommHeader.Parameters.Contains("@DateTimeFrom"))
                    { objCommHeader.Parameters.AddWithValue("@DateTimeFrom", vm.DateTimeFrom); }
                    else { objCommHeader.Parameters["@DateTimeFrom"].Value = vm.DateTimeFrom; }
                }
                if (vm.DateTimeTo == "")
                {
                    if (!objCommHeader.Parameters.Contains("@DateTimeTo"))
                    { objCommHeader.Parameters.AddWithValue("@DateTimeTo", System.DBNull.Value); }
                    else { objCommHeader.Parameters["@DateTimeTo"].Value = System.DBNull.Value; }
                }
                else
                {
                    if (!objCommHeader.Parameters.Contains("@DateTimeTo"))
                    { objCommHeader.Parameters.AddWithValue("@DateTimeTo", vm.DateTimeTo); }
                    else { objCommHeader.Parameters["@DateTimeTo"].Value = vm.DateTimeTo; }
                }
                if (!objCommHeader.Parameters.Contains("@transactionType"))
                { objCommHeader.Parameters.AddWithValue("@transactionType", vm.TransactionType); }
                else { objCommHeader.Parameters["@transactionType"].Value = vm.TransactionType; }

                if (!objCommHeader.Parameters.Contains("@TransferFrom"))
                { objCommHeader.Parameters.AddWithValue("@TransferFrom", vm.TransferFrom); }
                else { objCommHeader.Parameters["@TransferFrom"].Value = vm.TransferFrom; }

                if (vm.BranchId > 0)
                {
                    objCommHeader.Parameters.AddWithValue("@BranchId", vm.BranchId);
                }
                if (!string.IsNullOrEmpty(vm.ReferenceNo))
                {
                    objCommHeader.Parameters.AddWithValue("@ReferenceNo", vm.ReferenceNo);
                }
                if (vm.VehicleNo == "")
                {
                    if (!objCommHeader.Parameters.Contains("@VehicleNo"))
                    { objCommHeader.Parameters.AddWithValue("@VehicleNo", System.DBNull.Value); }
                    else { objCommHeader.Parameters["@VehicleNo"].Value = System.DBNull.Value; }
                }
                else
                {
                    if (!objCommHeader.Parameters.Contains("@VehicleNo"))
                    { objCommHeader.Parameters.AddWithValue("@VehicleNo", vm.VehicleNo); }
                    else { objCommHeader.Parameters["@VehicleNo"].Value = vm.VehicleNo; }
                }
                #endregion Parameter
                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommHeader);
                dataAdapter.Fill(dataTable);
            }
            #endregion
            #region Catch & Finally
            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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
            #endregion
            return dataTable;
        }

        public DataTable SearchTransferDetail(string TransferIssueNo, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            DataTable dataTable = new DataTable("SearchTransferDetail");
            #endregion
            #region Try
            try
            {
                #region Settings

                DataTable settingsDt = new DataTable();

                if (settingVM.SettingsDTUser == null)
                {
                    settingsDt = new CommonDAL().SettingDataAll(null, null, connVM);
                }

                string VAT6_5OrderByName = new CommonDAL().settingsDesktop("Reports", "VAT6_5OrderByProductName", settingsDt, connVM);


                #endregion

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
tid.TransferIssueNo
,ISNULL(tid.BOMId,0)BOMId
,tid.IssueLineNo
,tid.ItemNo
,tid.Quantity
,tid.CostPrice
,tid.UOM
,tid.SubTotal
,tid.Comments
,tid.TransactionType
,tid.TransactionDateTime IssueDateTime
,isnull(tid.TransferTo,0)TransferTo
,isnull(tid.BranchId,0)BranchId
,isnull(br.BranchName,'N/A') BranchName
,tid.Post
,isnull(tid.UOMQty,isnull(tid.Quantity,0))UOMQty
,isnull(tid.UOMn,tid.UOM)UOMn
,isnull(tid.UOMc,1)UOMc
,isnull(tid.UOMPrice,isnull(tid.CostPrice,0))UOMPrice
,isnull(tid.VATRate,0)VATRate
,isnull(tid.VATAmount,0)VATAmount
,isnull(tid.SDRate ,0)SDRate 
,isnull(tid.SDAmount,0)SDAmount
,isnull(tid.Weight,'')Weight


,tid.CreatedBy
,tid.CreatedOn
,tid.LastModifiedBy
,tid.LastModifiedOn,
isnull(Products.ProductCode,'N/A')ProductCode,
isnull(Products.ProductName,'N/A')ProductName,
isnull(isnull(Products.OpeningBalance,0)+
isnull(Products.QuantityInHand,0),0) as Stock,
Products.ProductName
 from TransferIssueDetails tid
 left outer join Products on tid.ItemNo=Products.ItemNo
LEFT OUTER JOIN BranchProfiles br on tid.TransferTo = br.BranchID
                            WHERE 1=1
                            and (TransferIssueNo = @TransferIssueNo ) 
                            ";
                if (VAT6_5OrderByName.ToLower() == "y")
                {
                    sqlText += @"  order by Products.ProductName";
                }
                else
                {
                    sqlText += @"  order by Products.ProductCode";

                }

                #endregion

                #region SQL Command
                SqlCommand objCommTransferIssueDetail = new SqlCommand();
                objCommTransferIssueDetail.Connection = currConn;
                objCommTransferIssueDetail.CommandText = sqlText;
                objCommTransferIssueDetail.Transaction = transaction;
                objCommTransferIssueDetail.CommandType = CommandType.Text;
                #endregion

                #region Parameter
                if (!objCommTransferIssueDetail.Parameters.Contains("@TransferIssueNo"))
                { objCommTransferIssueDetail.Parameters.AddWithValue("@TransferIssueNo", TransferIssueNo); }
                else { objCommTransferIssueDetail.Parameters["@TransferIssueNo"].Value = TransferIssueNo; }
                #endregion Parameter

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommTransferIssueDetail);
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

                if (Vtransaction == null)
                {
                    transaction.Rollback();
                }

                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
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
            return dataTable;
        }

        public DataTable SearchTransferDetail_TempTable(string TransferIssueNo, string userId, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            DataTable dataTable = new DataTable("SearchTransferDetail");
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
'' TransferIssueNo
,ISNULL(tid.BOMId,0)BOMId
,0 IssueLineNo
,tid.ItemNo
,tid.Quantity
,tid.CostPrice
,tid.UOM
,tid.SubTotal
,tid.Comments
,tid.TransactionType
,tid.TransactionDateTime IssueDateTime
,isnull(tid.TransferToBranchId,0)TransferTo
,isnull(tid.BranchId,0)BranchId
,isnull(br.BranchName,'N/A') BranchName
,tid.Post
,isnull(tid.UOMQty,isnull(tid.Quantity,0))UOMQty
,isnull(tid.UOMn,tid.UOM)UOMn
,isnull(tid.UOMc,1)UOMc
,isnull(tid.UOMPrice,isnull(tid.CostPrice,0))UOMPrice
,isnull(tid.VATRate,0)VATRate
,isnull(tid.VATAmount,0)VATAmount
,isnull(tid.SDRate ,0)SDRate 
,isnull(tid.SDAmount,0)SDAmount
,isnull(tid.Weight,'')Weight

,isnull(Products.ProductCode,'N/A')ProductCode,
isnull(Products.ProductName,'N/A')ProductName,
isnull(isnull(Products.OpeningBalance,0)+
isnull(Products.QuantityInHand,0),0) as Stock,
Products.ProductName
 from TempTransferData tid
 left outer join Products on tid.ItemNo=Products.ItemNo
LEFT OUTER JOIN BranchProfiles br on tid.TransferToBranchId = br.BranchID
WHERE 1=1
and (tid.ID = @TransferIssueNo and tid.UserId = @userId) 

order by Products.ProductName
                            ";
                #endregion
                #region SQL Command
                SqlCommand objCommTransferIssueDetail = new SqlCommand();
                objCommTransferIssueDetail.Connection = currConn;
                objCommTransferIssueDetail.CommandText = sqlText;
                objCommTransferIssueDetail.Transaction = transaction;
                objCommTransferIssueDetail.CommandType = CommandType.Text;
                #endregion
                #region Parameter
                if (!objCommTransferIssueDetail.Parameters.Contains("@TransferIssueNo"))
                { objCommTransferIssueDetail.Parameters.AddWithValue("@TransferIssueNo", TransferIssueNo); }
                else { objCommTransferIssueDetail.Parameters["@TransferIssueNo"].Value = TransferIssueNo; }

                objCommTransferIssueDetail.Parameters.AddWithValue("@userId", userId);

                #endregion Parameter
                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommTransferIssueDetail);
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

                if (Vtransaction == null)
                {
                    transaction.Rollback();
                }

                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
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
            return dataTable;
        }

        public DataSet FormLoad(UOMVM UOMvm, ProductVM Productvm, string Name, SysDBInfoVMTemp connVM = null)
        {
            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            DataSet ds = new DataSet("UOM_Product_Branch");
            try
            {
                #region open connection and transaction
                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                #endregion open connection and transaction

                #region sqlText
                sqlText = @"
--------------------------------UOM--------------------------------
select DISTINCT UOMId,
UOMFrom,
UOMTo,
Convertion,
CTypes,ActiveStatus
FROM UOMs	
WHERE 	(UOMId  LIKE '%' +  @UOMId + '%' OR @UOMId IS NULL) 
and (UOMFrom LIKE '%' + @UOMFrom + '%' OR @UOMFrom IS NULL)	
and (UOMTo LIKE '%' + @UOMTo + '%' OR @UOMTo IS NULL)
and (ActiveStatus LIKE '%' + @ActiveStatus + '%' OR @ActiveStatus IS NULL)
order by UOMFrom
--------------------------------Product--------------------------------
SELECT   
Products.ItemNo,
isnull(Products.ProductName,'N/A')ProductName,
isnull(Products.ProductCode,'N/A')ProductCode,
isnull(Products.ProductName,'N/A')+'~'+isnull(Products.ProductCode,'N/A')        AS ProductNameCode,
Products.CategoryID, 
isnull(ProductCategories.CategoryName,'N/A')CategoryName ,
isnull(Products.UOM,'N/A')UOM,
isnull(Products.HSCodeNo,'N/A')HSCodeNo,
isnull(ProductCategories.IsRaw,'N/A')IsRaw,
isnull(Products.OpeningTotalCost,0)OpeningTotalCost,
isnull(Products.CostPrice,0)CostPrice,
isnull(Products.OpeningBalance,0)OpeningBalance, 
isnull(Products.SalesPrice,0)SalesPrice,
isnull(Products.NBRPrice,0)NBRPrice,
isnull( Products.ReceivePrice,0)ReceivePrice,
isnull( Products.IssuePrice,0)IssuePrice,
isnull(Products.Packetprice,0)Packetprice, 
isnull(Products.TenderPrice,0)TenderPrice, 
isnull(Products.ExportPrice,0)ExportPrice, 
isnull(Products.InternalIssuePrice,0)InternalIssuePrice, 
isnull(Products.TollIssuePrice,0)TollIssuePrice, 
isnull(Products.TollCharge,0)TollCharge, 
isnull(Products.VATRate,0)VATRate,
isnull(Products.SD,0)SD,
isnull(Products.TradingMarkUp,0)TradingMarkUp,
isnull(isnull(Products.OpeningBalance,0)+isnull(Products.QuantityInHand,0),0) as Stock,
isnull(Products.QuantityInHand,0)QuantityInHand,
isnull(Products.Trading,'N')Trading, 
isnull(Products.NonStock,'N')NonStock,
isnull(Products.RebatePercent,0)RebatePercent,
                                    isnull(Products.IsVATRate,'N')IsVATRate,
                                    isnull(Products.IsSDRate,'N')IsSDRate
FROM Products LEFT OUTER JOIN
ProductCategories ON Products.CategoryID = ProductCategories.CategoryID ";
                sqlText += "  WHERE (Products.ItemNo LIKE '%' +'" + Productvm.ItemNo + "'+ '%'  OR Products.ItemNo IS NULL) AND";
                sqlText += " (Products.CategoryID  LIKE '%'  +'" + Productvm.CategoryID + "'+  '%' OR Products.CategoryID IS NULL) ";
                sqlText += " AND (ProductCategories.IsRaw LIKE '%'+'" + Productvm.IsRaw + "'+  '%' OR ProductCategories.IsRaw IS NULL)  ";
                sqlText += " AND (ProductCategories.CategoryName LIKE '%'+'" + Productvm.CategoryName + "'+  '%' OR ProductCategories.CategoryName IS NULL)  ";
                sqlText += " AND (Products.ActiveStatus LIKE '%'+'" + Productvm.ActiveStatus + "'+  '%' OR Products.ActiveStatus IS NULL)";
                sqlText += " AND (Products.Trading LIKE '%' +'" + Productvm.Trading + "'+  '%' OR Products.Trading IS NULL)";
                sqlText += " AND (Products.NonStock LIKE '%' +'" + Productvm.NonStock + "'+  '%' OR Products.NonStock IS NULL)";
                sqlText += " AND (Products.ProductCode LIKE '%'+'" + Productvm.ProductCode + "'+  '%' OR Products.ProductCode IS NULL)";
                sqlText += " order by Products.ItemNo ";

                sqlText += @"
--------------------------------Branch--------------------------------
SELECT Id, Name, DBName,isnull(BrAddress,'-')BrAddress
FROM Branchs
WHERE 	(Name  LIKE '%' +  @Name + '%' OR @Name IS NULL) 
order by Name
";
                #endregion sqlText

                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                #region Parameters for UOM
                if (!objComm.Parameters.Contains("@UOMId"))
                {
                    objComm.Parameters.AddWithValue("@UOMId", UOMvm.UOMID);
                }
                else
                {
                    objComm.Parameters["@UOMId"].Value = UOMvm.UOMID;
                }
                if (!objComm.Parameters.Contains("@UOMFrom"))
                {
                    objComm.Parameters.AddWithValue("@UOMFrom", UOMvm.UOMFrom);
                }
                else
                {
                    objComm.Parameters["@UOMFrom"].Value = UOMvm.UOMFrom;
                }
                if (!objComm.Parameters.Contains("@ActiveStatus"))
                {
                    objComm.Parameters.AddWithValue("@ActiveStatus", UOMvm.ActiveStatus);
                }
                else
                {
                    objComm.Parameters["@ActiveStatus"].Value = UOMvm.ActiveStatus;
                }
                if (!objComm.Parameters.Contains("@UOMTo"))
                {
                    objComm.Parameters.AddWithValue("@UOMTo", UOMvm.UOMTo);
                }
                else
                {
                    objComm.Parameters["@UOMTo"].Value = UOMvm.UOMTo;
                }
                #endregion
                #region Parameters for Branch
                if (!objComm.Parameters.Contains("@Name"))
                {
                    objComm.Parameters.AddWithValue("@Name", Name);
                }
                else
                {
                    objComm.Parameters["@Name"].Value = Name;
                }
                #endregion Parameters for Branch
                SqlDataAdapter dataAdapter = new SqlDataAdapter(objComm);
                dataAdapter.Fill(ds);
            }
            #region Catch
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
            return ds;
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


                sqlText = @"
SELECT
      ti.[TransferIssueNo] ID
	  , bp.BranchCode
      ,convert(varchar(50),ti.[TransactionDateTime],111) Transaction_Date
      ,convert(varchar(50),ti.[TransactionDateTime],108) Transaction_Time
      --,cast(ti.[TransactionDateTime] as varchar(100)) TransactionDateTime
	  ,pd.ProductCode 
	  , pd.ProductName
	  , tid.Quantity
	  , tid.UOM
      ,tid.Weight
	  ,tid.CostPrice
	  , bpp.BranchCode TransferToBranchCode
	  ,ti.[ReferenceNo]
	  ,ti.VehicleNo
	  ,ti.VehicleType
	  ,ti.[TransactionType]
      ,ti.[Post]
	  , tid.VATRate VAT_Rate
      ,ti.[Comments]
	  , tid.Comments CommentsD


  FROM [TransferIssues] ti left outer join TransferIssueDetails tid

  on ti.TransferIssueNo = tid.TransferIssueNo left outer join BranchProfiles bp
  on ti.BranchId = bp.BranchID left outer join Products pd 
  on tid.ItemNo = pd.ItemNo left outer join BranchProfiles bpp
  on ti.TransferTo = bpp.BranchID

  where ti.TransferIssueNo in ( ";

                int len = invoiceList.Count;

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

                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);

                DataTable table = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

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

                for (int i = 0; i < table.Rows.Count; i++)
                {
                    table.Rows[i]["TransactionType"] =
                        table.Rows[i]["TransactionType"].ToString() == "61Out" ? "RAW" : "FINISH";
                }
                //Master.TransactionType = Master.TransactionType == "RAW" ? "61Out" : "62Out";
                return table;

            }
            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
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


                sqlText = @"
SELECT
      ti.[TransferIssueNo] ID
	  , bp.BranchCode
      ,convert(varchar(50),ti.[TransactionDateTime],111) Transaction_Date
      ,convert(varchar(50),ti.[TransactionDateTime],108) Transaction_Time
      --,cast(ti.[TransactionDateTime] as varchar(100)) TransactionDateTime
	  ,pd.ProductCode 
	  , pd.ProductName
	  , tid.Quantity
	  , tid.UOM
      ,tid.Weight
	  ,tid.CostPrice
	  , bpp.BranchCode TransferToBranchCode
	  ,ti.[ReferenceNo]
	  ,ti.VehicleNo
	  ,ti.VehicleType
	  ,ti.[TransactionType]
      ,ti.[Post]
	  , tid.VATRate VAT_Rate
      ,ti.[Comments]
	  , tid.Comments CommentsD


  FROM [TransferIssues] ti left outer join TransferIssueDetails tid

  on ti.TransferIssueNo = tid.TransferIssueNo left outer join BranchProfiles bp
  on ti.BranchId = bp.BranchID left outer join Products pd 
  on tid.ItemNo = pd.ItemNo left outer join BranchProfiles bpp
  on ti.TransferTo = bpp.BranchID

  where ti.Id in ( ";

                int len = invoiceList.Count;

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

                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);

                DataTable table = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

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

                for (int i = 0; i < table.Rows.Count; i++)
                {
                    table.Rows[i]["TransactionType"] =
                        table.Rows[i]["TransactionType"].ToString() == "61Out" ? "RAW" : "FINISH";
                }
                //Master.TransactionType = Master.TransactionType == "RAW" ? "61Out" : "62Out";
                return table;

            }
            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
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
        }


        public DataTable GetExcelTransferData(List<string> invoiceList, string transactionType, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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

	ti.TransferIssueNo
, convert (varchar, ti.TransactionDateTime,120)IssueDateTime
,isnull(ti.TotalAmount     ,'0')TotalAmount
,isnull(ti.TransactionType ,'N/A')TransactionType
,isnull(ti.SerialNo        ,'N/A')SerialNo
,isnull(ti.Comments        ,'N/A')Comments
,isnull(ti.Post            ,'N')Post
,isnull(ti.ReferenceNo     ,'N/A')ReferenceNo
,isnull(ti.TransferTo      ,0)TransferTo
,isnull(ti.BranchId      ,0)BranchId
,isnull(ti.IsTransfer      ,0) IsTransfer 
,isnull(br.BranchName,'N/A') BranchName
,isnull(bp.BranchName,'N/A') BranchNameFrom

,isnull(ti.VehicleNo,'N/A')VehicleNo

,isnull(ti.TotalVATAmount       ,0)TotalVATAmount
,isnull(ti.TotalSDAmount       ,0)TotalSDAmount
,tid.ItemNo
,tid.Quantity
,tid.CostPrice
,tid.UOM
,tid.SubTotal

--,tid.TransactionType
--,tid.TransactionDateTime IssueDateTime
--,isnull(tid.TransferTo,0)TransferTo
--,isnull(tid.BranchId,0)BranchId

--,tid.Post
,isnull(tid.UOMQty,isnull(tid.Quantity,0))UOMQty
,isnull(tid.UOMn,tid.UOM)UOMn
,isnull(tid.UOMc,1)UOMc
,isnull(tid.UOMPrice,isnull(tid.CostPrice,0))UOMPrice
,isnull(tid.VATRate,0)VATRate
,isnull(tid.VATAmount,0)VATAmount
,isnull(tid.SDRate ,0)SDRate 
,isnull(tid.SDAmount,0)SDAmount


,isnull(Products.ProductCode,'N/A')ProductCode,
isnull(Products.ProductName,'N/A')ProductName,
isnull(isnull(Products.OpeningBalance,0)+
isnull(Products.QuantityInHand,0),0) as Stock


from TransferIssues ti left outer join TransferIssueDetails tid

on ti.TransferIssueNo = tid.TransferIssueNo 

LEFT OUTER JOIN BranchProfiles br on ti.TransferTo = br.BranchID
LEFT OUTER JOIN BranchProfiles bp on ti.BranchId = bp.BranchID
left outer join Products on tid.ItemNo = Products.ItemNo

where  ti.Post = 'Y' and ti.IsTransfer = 'N' and ti.TransactionType like '%' +@type + '%'

  --where ti.TransferIssueNo in ( ";

                //var len = invoiceList.Count;

                //for (int i = 0; i < len; i++)
                //{
                //    sqlText += "'" + invoiceList[i] + "'";

                //    if (i != (len - 1))
                //    {
                //        sqlText += ",";
                //    }
                //}

                //if (len == 0)
                //{
                //    sqlText += "''";
                //}

                //sqlText += ")";

                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
                cmd.Parameters.AddWithValue("@type", transactionType);
                DataTable table = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

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


                //Master.TransactionType = Master.TransactionType == "RAW" ? "61Out" : "62Out";
                return table;

            }
            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
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
        }

        public string[] UpdateTransferIssue(List<string> invoiceList, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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


                sqlText = @"
update TransferIssues set IsTransfer = 'Y'
  where TransferIssueNo in ( ";

                int len = invoiceList.Count;

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

                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
                int rows = cmd.ExecuteNonQuery();

                retResults[0] = "success";
                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                #endregion Commit


                return retResults;

            }
            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
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
        }

        #endregion

        #region Web Methods

        public List<TransferIssueVM> SelectAllList(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            ////SqlConnection currConn = null;
            ////SqlTransaction transaction = null;
            string sqlText = "";
            List<TransferIssueVM> VMs = new List<TransferIssueVM>();
            TransferIssueVM vm;
            #endregion
            try
            {

                #region sql statement

                #region SqlExecution


                DataTable dt = SelectAll(Id, conditionFields, conditionValues, VcurrConn, Vtransaction, true, connVM);

                foreach (DataRow dr in dt.Rows)
                {
                    vm = new TransferIssueVM();
                    vm.Id = dr["Id"].ToString();
                    vm.TransferIssueNo = dr["TransferIssueNo"].ToString();
                    vm.TransactionDateTime = dr["IssueDateTime"].ToString();
                    vm.TransactionType = dr["TransactionType"].ToString();
                    vm.SerialNo = dr["SerialNo"].ToString();
                    vm.Comments = dr["Comments"].ToString();
                    vm.Post = dr["Post"].ToString();
                    vm.ReferenceNo = dr["ReferenceNo"].ToString();
                    vm.SignatoryName = dr["SignatoryName"].ToString();
                    vm.SignatoryDesig = dr["SignatoryDesig"].ToString();
                    vm.TransferTo = Convert.ToInt32(dr["TransferTo"]);
                    vm.TransferBranchTo = dr["BranchName"].ToString();
                    vm.BranchId = Convert.ToInt32(dr["BranchId"]);
                    vm.VehicleNo = dr["VehicleNo"].ToString();
                    vm.VehicleType = dr["VehicleType"].ToString();
                    vm.TripNo = dr["TripNo"].ToString();

                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedOn = dr["CreatedOn"].ToString();
                    vm.LastModifiedBy = dr["LastModifiedBy"].ToString();
                    vm.LastModifiedOn = dr["LastModifiedOn"].ToString();
                    vm.TotalVATAmount = Convert.ToDecimal(dr["TotalVATAmount"].ToString());
                    vm.TotalSDAmount = Convert.ToDecimal(dr["TotalSDAmount"].ToString());
                    vm.TotalAmount = Convert.ToDecimal(dr["TotalAmount"].ToString());
                    vm.IsTransfer = dr["IsTransfer"].ToString();

                    vm.ImportExcelId = dr["ImportIDExcel"].ToString();
                    vm.ImportIDExcel = dr["ImportIDExcel"].ToString();

                    VMs.Add(vm);
                }


                #endregion SqlExecution

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
            //finally
            //{
            //}
            #endregion
            return VMs;
        }

        public DataTable SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, bool Dt = false, SysDBInfoVMTemp connVM = null)
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
ti.Id 
,ISNULL(ti.FiscalYear,0) FiscalYear
,ti.TransferIssueNo
, convert (varchar, ti.TransactionDateTime,120)IssueDateTime
, convert (varchar, ti.TransactionDateTime,120)TransactionDateTime
,isnull(ti.TotalAmount,'0')TotalAmount
,isnull(ti.TransactionType ,'N/A')TransactionType
,isnull(ti.SerialNo,'N/A')SerialNo
,isnull(ti.Comments,'N/A')Comments
,isnull(ti.Post,'N')Post
,isnull(ti.IsTransfer,'N') IsTransfer
,isnull(ti.ReferenceNo,'N/A')ReferenceNo
,isnull(ti.SignatoryName,'N/A')SignatoryName
,isnull(ti.SignatoryDesig,'N/A')SignatoryDesig
,isnull(ti.TransferTo,0)TransferTo
,isnull(ti.BranchId,0)BranchId
,isnull(ti.VehicleNo,'N/A')VehicleNo
,isnull(ti.VehicleType,'N/A')VehicleType
,isnull(ti.TripNo,'')TripNo
,isnull(br.BranchName,'N/A') BranchName
,isnull(ti.TotalVATAmount,0)TotalVATAmount
,isnull(ti.TotalSDAmount,0)TotalSDAmount
,isnull(ti.CreatedBy,'N/A')CreatedBy
,isnull(ti.CreatedOn,'19900101')CreatedOn
,isnull(ti.LastModifiedBy,'N/A')LastModifiedBy
,isnull(ti.LastModifiedOn,'19900101')LastModifiedOn
,ti.ImportIDExcel
FROM  dbo.TransferIssues ti
LEFT OUTER JOIN BranchProfiles br on ti.TransferTo = br.BranchID
WHERE 1=1
";


                if (Id > 0)
                {
                    sqlText += @" and ti.Id=@Id";
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

        public List<TransferIssueDetailVM> SelectDetail(string TransferIssueNo, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<TransferIssueDetailVM> VMs = new List<TransferIssueDetailVM>();
            TransferIssueDetailVM vm;
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
                DataTable dt = new DataTable();
                dt = SelectDetail_DT(TransferIssueNo, conditionFields, conditionValues, currConn, transaction, connVM);

                string JSONString = JsonConvert.SerializeObject(dt);
                VMs = new List<TransferIssueDetailVM>();

                VMs = JsonConvert.DeserializeObject<List<TransferIssueDetailVM>>(JSONString);


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

        public DataTable SelectDetail_DT(string TransferIssueNo, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            DataTable dt = new DataTable();
            #endregion
            try
            {
                #region Settings

                DataTable settingsDt = new DataTable();

                if (settingVM.SettingsDTUser == null)
                {
                    settingsDt = new CommonDAL().SettingDataAll(null, null, connVM);
                }

                string VAT6_5OrderByName = new CommonDAL().settingsDesktop("Reports", "VAT6_5OrderByProductName", settingsDt, connVM);


                #endregion

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
iss.TransferIssueNo
,ISNULL(iss.BOMId,0) BOMId
,iss.IssueLineNo
,iss.ItemNo
,iss.Quantity
,ISNULL(iss.CostPrice,0) CostPrice
,iss.UOM
,ISNULL(iss.SubTotal,0) SubTotal
,iss.Comments
,iss.TransactionType
,iss.TransactionDateTime
,iss.TransferTo
,iss.BranchId
,iss.Post
,ISNULL(iss.UOMQty,0) UOMQty
,ISNULL(iss.UOMPrice,0) UOMPrice
,iss.UOMc
,iss.UOMn

,iss.CreatedBy
,iss.CreatedOn
,iss.LastModifiedBy
,iss.LastModifiedOn
,ISNULL(iss.VATRate,0) VATRate
,ISNULL(iss.VATAmount,0) VATAmount
,ISNULL(iss.SDRate,0) SDRate
,ISNULL(iss.SDAmount,0) SDAmount
,ISNULL(iss.Weight,0) Weight
,ISNULL(iss.OtherRef,'')OtherRef
,p.ProductCode
,p.ProductName
,p.ProductName ItemName
FROM TransferIssueDetails iss left outer join Products p on iss.ItemNo=p.ItemNo
WHERE  1=1

";
                if (TransferIssueNo != null)
                {
                    sqlText += "AND iss.TransferIssueNo=@TransferIssueNo";
                }
                if (VAT6_5OrderByName.ToLower() == "y")
                {
                    sqlText += @"  order by P.ProductName";
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
                #endregion SqlText
                #region SqlExecution

                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);

                if (TransferIssueNo != null)
                {
                    objComm.Parameters.AddWithValue("@TransferIssueNo", TransferIssueNo);
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

                SqlDataAdapter da = new SqlDataAdapter(objComm);
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

        public string[] ImportExcelFile(TransferIssueVM paramVM, SysDBInfoVMTemp connVM = null, string UserId = "")
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
                DataTable dt = new DataTable();


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


                dt = ds.Tables[0];
                reader.Close();
                //System.IO.File.Delete(Fullpath);
                #endregion

                DataTable dtTransferIssueM = new DataTable();
                dtTransferIssueM = ds.Tables["TransferIssueM"];

                // dtPurchaseM = OrdinaryVATDesktop.DtDateCheck(dtPurchaseM, new string[] { "Invoice_Date", "Receive_Date" });

                #region Data Insert
                // PurchaseDAL puchaseDal = new PurchaseDAL();
                retResults = SaveTempTransfer(dtTransferIssueM, paramVM.BranchCode, paramVM.TransactionType, paramVM.CurrentUser, paramVM.BranchId, () => { }, null, null, false, connVM, "", UserId);
                if (retResults[0] == "Fail")
                {
                    throw new ArgumentNullException("", retResults[4]);
                }
                #endregion

                #region SuccessResult
                retResults[0] = "Success";
                retResults[1] = "Data Save Successfully.";
                ////////retResults[2] = vm.Id.ToString();
                #endregion SuccessResult
            }
            #endregion try
            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[4] = ex.Message.ToString(); //catch ex
                FileLogger.Log("PurchaseDAL", "ImportExcelFile", ex.ToString() + "\n" + sqlText);
                throw ex;
            }
            finally
            {
            }
            #endregion
            #region Results
            return retResults;
            #endregion
        }

        #endregion

        #region Import and Integration Methods

        public string[] ImportData(DataTable dtTrIssueM, DataTable dtTrIssueD, int branchId = 0, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, Action callBack = null, bool integration = false, SysDBInfoVMTemp connVM = null, string UserId = "")
        {
            #region variable
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            #endregion variable

            #region try
            try
            {
                ////row count
                if (dtTrIssueD.Rows.Count <= 0)
                {
                    retResults[0] = "Information";
                    retResults[1] = "You do Not Have Data to Import";
                    return retResults;
                }

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

                CommonImportDAL cImport = new CommonImportDAL();
                CommonDAL commonDal = new CommonDAL();
                ProductDAL productDal = new ProductDAL();
                int temp = branchId;

                DataTable productDt = new DataTable();

                #region Process Model

                #region VAT Rate Column Exist

                bool IsVATRateColumnExist = dtTrIssueD.Columns.Contains("VAT_Rate");
                string autoVATRate = commonDal.settings("Integration", "6_5AutoVATRate", currConn, transaction);
                string CompanyCode = commonDal.settings("CompanyCode", "Code", currConn, transaction);
                bool uomDefault = commonDal.settings("TransferIssue", "AutoUOM", currConn, transaction) == "Y";

                #endregion

                string productSave = commonDal.settingValue("AutoSave", "SaleProduct", null, currConn, transaction);

                bool ProductAutoSave = false;
                ProductAutoSave = productSave == "Y" ? true : false;

                foreach (DataRow dr in dtTrIssueM.Rows)
                {
                    #region Master

                    TransferIssueVM Master;
                    List<TransferIssueDetailVM> Details = new List<TransferIssueDetailVM>();
                    decimal totalVATAmount = 0;
                    decimal totalSDAmount = 0;
                    decimal totalAmount = 0;
                    string vehicleType = dr["VehicleType"].ToString();
                    string VehicleNo = dr["VehicleNo"].ToString();

                    if ((VehicleNo == null || VehicleNo.Trim() == "-" ||
                         string.IsNullOrEmpty(VehicleNo)) ||
                        (vehicleType == null || string.IsNullOrEmpty(vehicleType)))
                    {
                        throw new Exception("Vehicle and Vehicle Type is required");
                    }


                    Master = new TransferIssueVM();
                    string Id = dr["ID"].ToString();
                    Master.TransferIssueNo = "";
                    Master.TransactionType = dr["TransactionType"].ToString();
                    Master.TransactionDateTime = dr["TransactionDateTime"].ToString();
                    //Master.SerialNo = //dr["SerialNo"].ToString();
                    Master.ReferenceNo = dr["ReferenceNo"].ToString();
                    Master.Comments = dr["Comments"].ToString();
                    Master.Post = dr["Post"].ToString();
                    Master.VehicleNo = dr["VehicleNo"].ToString();
                    Master.VehicleType = vehicleType;
                    Master.ImportExcelId = Id;

                    string branch_Id = dr["BranchId"].ToString();
                    string transferBranch_Id = dr["TransferToBranchId"].ToString();
                    string branchIdInDb = "0";

                    if (string.IsNullOrEmpty(branch_Id))
                    {
                        if (!string.IsNullOrEmpty(dr["BranchCode"].ToString()))
                        {
                            branchIdInDb = cImport.FindBranchId(dr["BranchCode"].ToString(), currConn, transaction, connVM);
                        }
                        Master.BranchId = branchIdInDb != "0" ? Convert.ToInt32(branchIdInDb) : temp;
                    }
                    else
                    {
                        Master.BranchId = Convert.ToInt32(branch_Id);
                    }


                    if (string.IsNullOrEmpty(transferBranch_Id))
                    {
                        branchIdInDb = cImport.FindBranchId(dr["TransferToBranchCode"].ToString(), currConn, transaction, connVM);

                        Master.TransferTo = branchIdInDb != "0" ? Convert.ToInt32(branchIdInDb) : temp;
                    }
                    else
                    {
                        Master.TransferTo = Convert.ToInt32(transferBranch_Id);
                    }

                    Master.BranchToRef = dr["BranchToRef"].ToString();
                    Master.BranchFromRef = dr["BranchFromRef"].ToString();

                    Master.CreatedBy = dr["CreatedBy"].ToString();
                    Master.CreatedOn = dr["CreatedOn"].ToString();
                    Master.LastModifiedBy = dr["LastModifiedBy"].ToString();
                    Master.LastModifiedOn = dr["LastModifiedOn"].ToString();

                    DataRow[] DetailsRaws;
                    if (!string.IsNullOrEmpty(Id))
                    {
                        DetailsRaws = dtTrIssueD.Select("ID='" + Id + "'");
                    }
                    else
                    {
                        DetailsRaws = null;
                    }

                    #endregion

                    #region Detail

                    int i = 1;
                    foreach (DataRow row in DetailsRaws)
                    {
                        #region local variables
                        TransferIssueDetailVM Detail = new TransferIssueDetailVM();
                        decimal CostPrice = 0;
                        decimal SubTotal = 0;
                        decimal UOMPrice = 0;
                        decimal VATRate = 0;
                        decimal VATAmount = 0;
                        decimal SDRate = 0;
                        decimal SDAmount = 0;
                        decimal UOMc = 1;
                        decimal priceWithSD = 0;
                        decimal UOMQty = 0;
                        string UOMn = "";
                        #endregion

                        Detail.ItemName = row["ProductName"].ToString();
                        Detail.Quantity = Convert.ToDecimal(row["Quantity"].ToString());
                        Detail.UOM = row["UOM"].ToString();
                        Detail.Comments = row["CommentsD"].ToString();
                        Detail.ItemName = row["ProductName"].ToString();
                        Detail.ProductCode = row["ProductCode"].ToString();
                        Detail.Weight = row["Weight"].ToString();
                        Detail.OtherRef = row["OtherRef"].ToString();


                        //here define the local variables;
                        string itemNo = row["ItemNo"].ToString();

                        #region New Product

                        if (string.IsNullOrEmpty(itemNo) || itemNo == "0")
                        {
                            if (CompanyCode.ToLower() == "mbl" || CompanyCode.ToLower() == "MBLShirirchala".ToLower() || CompanyCode.ToLower() == "MBLMouchak".ToLower())
                            {
                                string pGroup = "";
                                if (Master.TransactionType == "61Out")
                                {
                                    pGroup = "Raw";
                                }
                                else if (Master.TransactionType == "62Out")
                                {
                                    pGroup = "Finish";
                                }

                                itemNo = cImport.FindItemId(Detail.ItemName, Detail.ProductCode, currConn, transaction, ProductAutoSave, Detail.UOM, 1, Convert.ToDecimal(row["VAT_Rate"].ToString()), 0, connVM, pGroup);

                            }
                            else
                            {
                                itemNo = cImport.FindItemId(Detail.ItemName, Detail.ProductCode, currConn, transaction, ProductAutoSave, Detail.UOM, 1, Convert.ToDecimal(row["VAT_Rate"].ToString()));
                            }
                        }

                        #endregion


                        productDt = new DataTable();


                        productDt = SearchByItemNo(itemNo, null, currConn, transaction);

                        if (IsVATRateColumnExist)
                        {
                            VATRate = Convert.ToDecimal(row["VAT_Rate"].ToString());
                        }

                        if (productDt.Rows.Count > 0)
                        {
                            UOMn = productDt.Rows[0]["UOM"].ToString();
                            //  CostPrice = Convert.ToDecimal(productDt.Rows[0]["CostPrice"].ToString());

                            if (autoVATRate == "Y")
                            {
                                if (VATRate == 0)
                                {
                                    VATRate = Convert.ToDecimal(productDt.Rows[0]["VATRate"].ToString());
                                }
                            }


                            SDRate = Convert.ToDecimal(productDt.Rows[0]["SD"].ToString());
                        }
                        #region Cost price

                        CostPrice = Convert.ToDecimal(row["CostPrice"].ToString());

                        if (CostPrice <= 0)
                        {
                            DataTable priceData = new ProductDAL().AvgPriceNew(Detail.ProductCode, Convert.ToDateTime(Master.TransactionDateTime).ToString("yyyy-MM-dd") + DateTime.Now.ToString(" HH:mm:ss"), currConn, transaction, false, true, true, true, connVM, UserId);
                            decimal amount = Convert.ToDecimal(priceData.Rows[0]["Amount"].ToString());
                            decimal quanA = Convert.ToDecimal(priceData.Rows[0]["Quantity"].ToString());
                            if (quanA > 0)
                            {
                                CostPrice = (amount / quanA);
                            }
                        }

                        #endregion

                        //calculated properties(needed to check)

                        if (integration)
                        {
                            CostPrice = Convert.ToDecimal(row["CostPrice"].ToString());
                        }

                        if (uomDefault)
                        {
                            UOMc = 1;
                        }
                        else
                        {
                            if (UOMn.ToLower() == Detail.UOM.ToLower())
                            {
                                UOMc = 1;
                            }
                            else
                            {
                                UOMc = Convert.ToDecimal(cImport.FindUOMc(UOMn, Detail.UOM, currConn, transaction, null, itemNo));
                            }
                        }

                        UOMPrice = CostPrice;
                        UOMQty = UOMc * Detail.Quantity;
                        SubTotal = CostPrice * Detail.Quantity * UOMc;
                        SDAmount = SubTotal * SDRate / 100;
                        priceWithSD = SubTotal + SDAmount;
                        VATAmount = priceWithSD * VATRate / 100;

                        Detail.CostPrice = CostPrice;
                        Detail.SubTotal = SubTotal;
                        Detail.TransactionType = Master.TransactionType;
                        Detail.TransferTo = Master.TransferTo;
                        Detail.Post = Master.Post;
                        Detail.CreatedBy = Master.CreatedBy;
                        Detail.CreatedOn = Master.CreatedOn;
                        Detail.LastModifiedBy = Master.LastModifiedBy;
                        Detail.LastModifiedOn = Master.LastModifiedOn;
                        Detail.UOMn = UOMn;
                        Detail.UOMc = UOMc;
                        Detail.UOMQty = UOMQty;
                        Detail.UOMPrice = UOMPrice;
                        Detail.VATRate = VATRate;
                        Detail.VATAmount = VATAmount;
                        Detail.SDRate = SDRate;
                        Detail.SDAmount = SDAmount;
                        Detail.ItemNo = itemNo;
                        Detail.BranchId = Master.BranchId;
                        Detail.IssueLineNo = i.ToString();
                        Details.Add(Detail);
                        i++;
                        //here increment total amounts
                        totalSDAmount += SDAmount;
                        totalVATAmount += VATAmount;
                        totalAmount += SubTotal + SDAmount + VATAmount;////needs to check
                    }

                    #endregion

                    Master.TotalSDAmount = totalSDAmount;
                    Master.TotalVATAmount = totalVATAmount;
                    Master.TotalAmount = totalAmount;


                    #region Insert

                    string[] result = Insert(Master, Details, transaction, currConn, connVM);
                    if (CompanyCode.ToLower() == "bata")
                    {
                        if (result[0].ToLower() == "success")
                        {
                            FileLogger.Log("TransferIssueDAL", "ImportData", "Entered in transfer receive: " + JsonConvert.SerializeObject(result));

                            #region Auto TransferRecive

                            //DataTable TransferReceivedt = new DataTable();

                            //TransferReceivedt = SearchTransferIssue(new TransferIssueVM() { TransferIssueNo = result[2].ToString(), TransactionType = "", IssueDateFrom = "", IssueDateTo = "", Post = "", ReferenceNo = "" }, currConn, transaction, connVM);

                            TransferReceiveVM TransferReceive = new TransferReceiveVM();
                            TransferReceive.TransactionDateTime = Master.TransactionDateTime;
                            TransferReceive.Comments = "-";
                            TransferReceive.CreatedBy = Master.CreatedBy;
                            TransferReceive.CreatedOn = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                            TransferReceive.TransactionType = Master.TransactionType == "62Out" ? "62In" : "61In";
                            TransferReceive.TransferNo = "-";
                            TransferReceive.TransferFromNo = result[2].ToString();
                            TransferReceive.TransferFrom = Master.BranchId;
                            TransferReceive.Post = "Y";
                            //TransferReceive.TotalAmount = Convert.ToDecimal(TransferReceivedt.Rows[0]["TotalAmount"].ToString());
                            TransferReceive.BranchId = Master.TransferTo;

                            DataTable transferDetailResult = SearchTransferDetail(result[2].ToString(), currConn, transaction, connVM);
                            List<TransferReceiveDetailVM> TransferReceiveDetailVMs = new List<TransferReceiveDetailVM>();
                            int count = 0;
                            foreach (DataRow item in transferDetailResult.Rows)
                            {
                                TransferReceiveDetailVM detail = new TransferReceiveDetailVM();
                                detail.ReceiveLineNo = count++.ToString();
                                detail.ItemNo = item["ItemNo"].ToString();
                                detail.Quantity = Convert.ToDecimal(item["Quantity"].ToString());
                                detail.CostPrice = Convert.ToDecimal(item["CostPrice"].ToString());
                                detail.UOM = item["UOM"].ToString();
                                detail.SubTotal = Convert.ToDecimal(item["SubTotal"].ToString());
                                detail.Comments = "FromTransferReceive";
                                detail.TransactionDateTime = DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss");
                                detail.TransferFrom = Master.BranchId;
                                detail.UOMQty = Convert.ToDecimal(item["UOMQty"].ToString());
                                detail.UOMn = item["UOMn"].ToString();
                                detail.UOMc = Convert.ToDecimal(item["UOMc"].ToString());
                                detail.UOMPrice = Convert.ToDecimal(item["UOMPrice"].ToString());

                                detail.VATRate = Convert.ToDecimal(item["VATRate"].ToString());
                                detail.VATAmount = Convert.ToDecimal(item["VATAmount"].ToString());
                                detail.SDRate = Convert.ToDecimal(item["SDRate"].ToString());
                                detail.SDAmount = Convert.ToDecimal(item["SDAmount"].ToString());
                                detail.Weight = item["Weight"].ToString();

                                detail.BranchId = TransferReceive.BranchId;
                                detail.TransferFromNo = TransferReceive.TransferFromNo;


                                TransferReceiveDetailVMs.Add(detail);
                            }

                            var sqlResult = new TransferReceiveDAL().Insert(TransferReceive, TransferReceiveDetailVMs, transaction, currConn, connVM);

                            if (sqlResult[0].ToLower() != "success")
                            {
                                throw new Exception("Import failed to TransferReceive");
                            }
                            #endregion
                        }
                    }
                    #endregion

                    if (result[0].ToLower() != "success")
                    {
                        throw new Exception("Import failed");
                    }

                    if (callBack != null) callBack();
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
                retResults = new string[5];
                retResults[0] = "Success";
                retResults[1] = "Transfer Issues imported successfully";
                #endregion SuccessResult

            }
            #endregion try
            #region Catch and Finall
            catch (Exception ex)
            {
                //retResults[0] = "Fail";//Success or Fail
                //retResults[4] = ex.Message.ToString(); //catch ex
                FileLogger.Log("TransferIssueDAL", "ImportData", ex.ToString());
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
            return retResults;
        }

        public string[] ImportTransferData(DataTable dtTrIssueM, DataTable dtTrIssueD, int branchId = 0, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region variable
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            #endregion variable

            #region try
            try
            {
                ////row count
                if (dtTrIssueD.Rows.Count <= 0)
                {
                    retResults[0] = "Information";
                    retResults[1] = "You do Not Have Data to Import";
                    return retResults;
                }

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

                //CommonImportDAL cImport = new CommonImportDAL();
                //CommonDAL commonDal = new CommonDAL();
                //ProductDAL productDal = new ProductDAL();
                int temp = branchId;


                TransferReceiveDAL receiveDal = new TransferReceiveDAL();


                #region Process Model
                foreach (DataRow row in dtTrIssueM.Rows)
                {
                    TransferReceiveVM master = new TransferReceiveVM();
                    List<TransferReceiveDetailVM> detailList = new List<TransferReceiveDetailVM>();

                    string masterTransferFromNo = row["TransferIssueNo"].ToString();
                    string transactionDateTime = row["IssueDateTime"].ToString();


                    DataTable transfer = receiveDal.SelectAll(0, new[] { "TransferFromNo" }, new[] { masterTransferFromNo }, currConn,
                        transaction, false, connVM);


                    if (transfer != null && transfer.Rows.Count > 0)
                    {
                        throw new Exception("TransferIssue No Already Imported");
                    }

                    master.TransferReceiveNo = masterTransferFromNo;
                    master.TransactionDateTime = transactionDateTime; //dtpReceiveDate.Value.ToString("yyyy-MM-dd") + DateTime.Now.ToString(" HH:mm:ss");
                    master.TotalAmount = Convert.ToDecimal(row["TotalAmount"]);

                    //Convert.ToDecimal(txtTotalAmount.Text.Trim());

                    master.BranchId = branchId;
                    master.SerialNo = row["SerialNo"].ToString();

                    master.ReferenceNo = row["ReferenceNo"].ToString();

                    master.Comments = row["Comments"].ToString();

                    master.CreatedBy = row["CurrentUser"].ToString();//Program.CurrentUser;
                    master.CreatedOn = row["CreatedOn"].ToString();//DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    //master.LastModifiedBy = row["CurrentUser"].ToString();//Program.CurrentUser;
                    //master.LastModifiedOn = row["CurrentUser"].ToString();//DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                    master.TransactionType = row["TransactionType"].ToString();

                    //master.TransferNo = txtTransferNo.Text.Trim();

                    master.TransferFromNo = masterTransferFromNo;
                    master.TransferFrom = Convert.ToInt32(row["BranchId"]);
                    master.Post = "N";
                    //master.BranchId = Program.BranchId;

                    DataRow[] details = dtTrIssueD.Select("TransferIssueNo='" + masterTransferFromNo + "'");

                    int i = 1;
                    foreach (DataRow detailRow in details)
                    {
                        TransferReceiveDetailVM detail = new TransferReceiveDetailVM
                        {
                            TransferReceiveNo = master.TransferReceiveNo,
                            ReceiveLineNo = i.ToString(),
                            ItemNo = detailRow["ItemNo"].ToString(),
                            Quantity = Convert.ToDecimal(detailRow["Quantity"].ToString()),
                            CostPrice = Convert.ToDecimal(detailRow["CostPrice"].ToString()),
                            UOM = detailRow["UOM"].ToString(),
                            SubTotal = Convert.ToDecimal(detailRow["SubTotal"].ToString()),
                            Comments = "FromTransferReceive",
                            TransactionDateTime = transactionDateTime,
                            TransferFrom = Convert.ToInt32(detailRow["BranchId"]),
                            UOMQty = Convert.ToDecimal(detailRow["UOMQty"].ToString()),
                            UOMn = detailRow["UOMn"].ToString(),
                            UOMc = Convert.ToDecimal(detailRow["UOMc"].ToString()),
                            UOMPrice = Convert.ToDecimal(detailRow["UOMPrice"].ToString()),
                            VATRate = Convert.ToDecimal(detailRow["VATRate"].ToString()),
                            VATAmount = Convert.ToDecimal(detailRow["VATAmount"].ToString()),
                            SDRate = Convert.ToDecimal(detailRow["SDRate"].ToString()),
                            SDAmount = Convert.ToDecimal(detailRow["SDAmount"].ToString()),
                            BranchId = branchId
                        };

                        detailList.Add(detail);

                        i++;
                    }


                    retResults = receiveDal.Insert(master, detailList, null, null);

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
                retResults = new string[5];
                retResults[0] = "Success";
                retResults[1] = "Transfer Issues imported successfully";
                #endregion SuccessResult

            }
            #endregion try
            #region Catch and Finall
            catch (Exception ex)
            {
                //retResults[0] = "Fail";//Success or Fail
                //retResults[4] = ex.Message.ToString(); //catch ex
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
            return retResults;
        }

        public string[] SaveTempTransfer(DataTable data, string BranchCode, string transactionType, string CurrentUser,
            int branchId, Action callBack, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null,
            bool integration = false, SysDBInfoVMTemp connVM = null, string entryTime = "", string UserId = "")
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
            retResults[5] = "Insert"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            #endregion

            try
            {
                CommonDAL commonDAL = new CommonDAL();
                CommonDAL commonDal = new CommonDAL();

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

                string CompanyCode = commonDAL.settings("CompanyCode", "Code", currConn, transaction, connVM);
                string duplicate = commonDAL.settings("Import", "TransferIssueDuplicateInsert", currConn, transaction, connVM);
                string CustomeDateFormat = commonDal.settingValue("Integration", "CustomeDateFormat", connVM, currConn, transaction);

                //////var bulkResult = commonDal.BulkInsert("TempTransferIssue_Audit", data, currConn, transaction);


                #region Merge DateTime

                if (data.Columns.Contains("Transaction_Date") && data.Columns.Contains("Transaction_Time"))
                {
                    if (!data.Columns.Contains("TransactionDateTime"))
                    {
                        data.Columns.Add("TransactionDateTime");
                    }

                    if (!string.IsNullOrWhiteSpace(CustomeDateFormat) && CustomeDateFormat != "-")
                    {
                        if (CustomeDateFormat == "DD.MM.YYYY" || CustomeDateFormat == "dd.mm.yyyy")
                        {
                            CustomeDateFormat = "dd.MM.yyyy";
                        }
                        string pattern = CustomeDateFormat;

                        foreach (DataRow dataRow in data.Rows)
                        {
                            DateTime parsedDate;

                            if (DateTime.TryParseExact(dataRow["Transaction_Date"].ToString().Trim(), pattern, null, DateTimeStyles.None, out parsedDate))
                            {
                                string TransactionDateTime = parsedDate.ToString("yyyy-MMM-dd") + " " + dataRow["Transaction_Time"].ToString();
                                dataRow["TransactionDateTime"] = TransactionDateTime;
                            }
                            ////dataRow["TransactionDateTime"] = OrdinaryVATDesktop.DateToDate_YMD(dataRow["Transaction_Date"].ToString()) + " " + OrdinaryVATDesktop.DateToTime_HMS(dataRow["Transaction_Time"].ToString());

                        }

                    }
                    else
                    {
                        foreach (DataRow dataRow in data.Rows)
                        {
                            dataRow["TransactionDateTime"] = OrdinaryVATDesktop.DateToDate_YMD(dataRow["Transaction_Date"].ToString()) + " " + OrdinaryVATDesktop.DateToTime_HMS(dataRow["Transaction_Time"].ToString());
                        }
                    }

                    data.Columns.Remove("Transaction_Date");
                    data.Columns.Remove("Transaction_Time");
                }
                else
                {
                    foreach (DataRow dataRow in data.Rows)
                    {
                        dataRow["TransactionDateTime"] = OrdinaryVATDesktop.DateToDate(dataRow["TransactionDateTime"].ToString());
                    }

                }

                #endregion

                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);

                try
                {
                    #region Clear TempTransferData

                    sqlText = @"delete from TempTransferData;";
                    cmd.CommandText = sqlText;
                    transResult = cmd.ExecuteNonQuery();

                    #endregion

                    #region Bulk Insert

                    string[] bulkRes = commonDal.BulkInsert("TempTransferData", data, currConn, transaction, 10000, null, connVM);

                    #endregion
                }
                catch (Exception ex)
                {
                    FileLogger.Log("TransferIssueDAL", "TransferIssue1", ex.ToString());

                    throw ex;
                }


                #region SqlText

                string updateItemNo = @"
update TempTransferData set ItemNo = Products.ItemNo from Products where Products.ProductCode = TempTransferData.ProductCode and (TempTransferData.ItemNo is null or TempTransferData.ItemNo = '0');
update TempTransferData set ItemNo = Products.ItemNo from Products where Products.ProductName = TempTransferData.ProductName and (TempTransferData.ItemNo is null or TempTransferData.ItemNo = '0');
update TempTransferData set ItemNo = ProductDetails.ItemNo 
from ProductDetails where TempTransferData.ProductName = ProductDetails.ProductName  and (TempTransferData.ItemNo is null or TempTransferData.ItemNo = '0');
";

                string branchUpdate = @"update TempTransferData set BranchId = BranchProfiles.BranchID 
from BranchProfiles where BranchProfiles.BranchCode = TempTransferData.BranchCode 
or BranchProfiles.IntegrationCode = TempTransferData.BranchCode;

update TempTransferData set TransferToBranchId = BranchProfiles.BranchID 
from BranchProfiles where BranchProfiles.BranchCode = TempTransferData.TransferToBranchCode
or BranchProfiles.IntegrationCode = TempTransferData.TransferToBranchCode;


update TempTransferData set BranchId = BranchMapDetails.BranchId from BranchMapDetails
where BranchMapDetails.IntegrationCode = TempTransferData.BranchCode 
and (TempTransferData.BranchId is null or TempTransferData.BranchId = '0')

update TempTransferData set TransferToBranchId = BranchMapDetails.BranchId from BranchMapDetails
where BranchMapDetails.IntegrationCode = TempTransferData.TransferToBranchCode 
and (TempTransferData.TransferToBranchId is null or TempTransferData.TransferToBranchId = '0')


";


                string updateCostPrice = @"
update TempTransferData set CostPrice = Products.NBRPrice 
from Products 
where Products.ProductCode = TempTransferData.ProductCode and (ISNULL(TempTransferData.CostPrice,0)=0);
";

                string updateTransactionType = @"
--update TempTransferData set TransactionType = 'Finish'
--where TransactionType = 'Trading'

update TempTransferData set TransactionType = '62Out' 
where TransactionType in ('finish','finished','trading')

update TempTransferData set TransactionType = '61Out' 
where TransactionType in ('raw','Packaging')

";

                string deleteDuplicate = @"delete from TempTransferData where ID in (
select TempTransferData.ID from 
TempTransferData join TransferIssues on TempTransferData.ID = TransferIssues.ImportIDExcel
where TempTransferData.TransactionType=TransferIssues.TransactionType
and TempTransferData.ID=TransferIssues.ImportIDExcel
)";

                string getNewProducts = @"
select * from TempTransferData where ISNULL(ItemNo,0) = 0";

                string updateTime =
                    @"update TempTransferData set TransactionDateTime = FORMAT(cast(transactiondatetime as datetime),'yyyy-MM-dd')+' '+'" +
                    entryTime + "'";

                string TransactionDateTimeUpdate = @"
create table #Temp
(
    ImportId Varchar(2000), 
    TransactionDateTime DatetIme, 

)
insert into #Temp (ImportId,TransactionDateTime)
 
select ID , min(TransactionDateTime)
from TempTransferData 
group by ID

update TempTransferData set TransactionDateTime=#Temp.TransactionDateTime
from #Temp where TempTransferData.ID=#Temp.ImportId

drop Table #Temp

";

                string getAll = @"
SELECT 
	   [ID]
      ,[BranchCode]
      ,min([TransactionDateTime])[TransactionDateTime]
      ,[TransactionType]
      ,[ProductCode]
      ,[ProductName]
      ,[UOM]
      ,sum([Quantity]) Quantity
      ,[CostPrice]
      ,[TransferToBranchCode]
      ,[Post]
      ,[VAT_Rate]
      ,[ReferenceNo]
      ,[Comments]
      ,[ItemNo]
      ,[BranchId]
      ,[BomId]
      ,[TransferToBranchId]
      ,[CommentsD]
      ,[Weight]
      ,[VehicleNo]
      ,[UserId]
      ,[UOMn]
      ,[UOMQty]
      ,[UOMc]
      ,[UOMPrice]
      ,[VATRate]
      ,[VATAmount]
      ,[SDRate]
      ,[SDAmount]
      ,[SubTotal]
      ,[VehicleType]
      ,[BranchFromRef]
      ,[BranchToRef]
      ,[OtherRef]
  FROM [TempTransferData]
  group by 

       [ID]
      ,[BranchCode]
      ,[TransactionType]
      ,[ProductCode]
      ,[ProductName]
      ,[UOM]
      ,[Quantity]
      ,[CostPrice]
      ,[TransferToBranchCode]
      ,[Post]
      ,[VAT_Rate]
      ,[ReferenceNo]
      ,[Comments]
      ,[ItemNo]
      ,[BranchId]
      ,[BomId]
      ,[TransferToBranchId]
      ,[CommentsD]
      ,[Weight]
      ,[VehicleNo]
      ,[UserId]
      ,[UOMn]
      ,[UOMQty]
      ,[UOMc]
      ,[UOMPrice]
      ,[VATRate]
      ,[VATAmount]
      ,[SDRate]
      ,[SDAmount]
      ,[SubTotal]
      ,[VehicleType]
      ,[BranchFromRef]
      ,[BranchToRef]
      ,[OtherRef]";


                string selectDuplicate = @"select distinct TempTransferData.ID from 
TempTransferData join TransferIssues on TempTransferData.ID = TransferIssues.ImportIDExcel";


                #endregion

                #region SqlExecution

                cmd.CommandText = updateTransactionType;

                int updateTransactionTypeData = cmd.ExecuteNonQuery();

                #region Delete Duplicate

                if (CompanyCode == "ACIGDJ" || CompanyCode == "CP" || OrdinaryVATDesktop.IsACICompany(CompanyCode) || CompanyCode.ToLower() == "eon" || CompanyCode.ToLower() == "purofood"
                    || CompanyCode.ToLower() == "eahpl" || CompanyCode.ToLower() == "eail" || CompanyCode.ToLower() == "eeufl" || CompanyCode.ToLower() == "exfl" || CompanyCode.ToLower() == "mbl" || CompanyCode.ToLower() == "bcl".ToLower()
                    || CompanyCode.ToLower() == "Berger".ToLower() || CompanyCode.ToLower() == "MBLMouchak".ToLower() || CompanyCode.ToLower() == "MBLShirirchala".ToLower() || CompanyCode.ToLower() == "MBLMirsarai".ToLower()
                    || CompanyCode.ToLower() == "SMC".ToLower() || CompanyCode.ToLower() == "SMCHOLDING".ToLower() || CompanyCode.ToLower() == "JAPFA".ToLower())
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
                else
                {
                    if (duplicate.ToLower() == "n")
                    {
                        cmd.CommandText = deleteDuplicate;

                        int deletedData = cmd.ExecuteNonQuery();
                    }
                }
                #endregion


                ////cmd.CommandText = updateItemNo + " " + branchUpdate + " " + updateCostPrice + "  " + updateTransactionType;
                cmd.CommandText = updateItemNo + " " + branchUpdate + " " + updateCostPrice;


                if (OrdinaryVATDesktop.IsACICompany(CompanyCode))
                {
                    string dateupdate = "update TempTransferData set TransactionDateTime = @datetime";
                    SqlCommand updateDateCmd = new SqlCommand(dateupdate, currConn, transaction);
                    updateDateCmd.Parameters.AddWithValue("@datetime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    updateDateCmd.ExecuteNonQuery();
                }
                else
                {
                    if (!string.IsNullOrEmpty(entryTime))
                    {
                        cmd.CommandText += updateTime;
                    }
                }


                FileLogger.Log("TransferIssueDAL", "TransferIssue", cmd.CommandText);


                cmd.ExecuteNonQuery();

                cmd.CommandText = TransactionDateTimeUpdate;
                cmd.ExecuteNonQuery();

                DataTable table = new DataTable();

                cmd.CommandText = getAll;
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                adapter.Fill(table);

                #endregion

                DataTable filterdData = new DataTable();

                //dtTableResult =  OrdinaryVATDesktop.FormatSaleData(dtTableResult);


                foreach (DataColumn column in table.Columns)
                {
                    filterdData.Columns.Add(column.ColumnName);
                }


                #region Quantity Summation
                if (CompanyCode.ToLower() != "kccl")
                {
                    foreach (DataRow row in table.Rows)
                    {

                        DataRow[] currentRows = filterdData.Select("ProductCode= '" + row["ProductCode"] + "' and ID='" + row["ID"] + "' and CostPrice = '" + row["CostPrice"] + "'");


                        if (currentRows.Length == 0)
                        {
                            DataRow[] rows = table.Select("ProductCode= '" + row["ProductCode"] + "' and ID='" + row["ID"] + "' and CostPrice = '" + row["CostPrice"] + "'");

                            decimal quantity = 0;
                            decimal weight = 0;
                            string absWeight = "";
                            foreach (DataRow dataRow in rows)
                            {
                                quantity += Convert.ToDecimal(dataRow["Quantity"]);

                                if (OrdinaryVATDesktop.IsNumber((dataRow["Weight"] ?? "").ToString()))
                                {
                                    weight += Convert.ToDecimal(dataRow["Weight"]);
                                }
                                else
                                {
                                    absWeight = dataRow["Weight"] == null ? "" : dataRow["Weight"].ToString();
                                }

                            }

                            row["Quantity"] = quantity;
                            row["Weight"] = weight == 0 ? absWeight : weight.ToString();

                            object[] items = row.ItemArray;

                            filterdData.Rows.Add(items);
                        }

                    }
                }
                else
                {
                    filterdData = table.Copy();
                }


                #endregion

                #region Comments

                #region Product Auto Save
                CommonImportDAL commonImportDal = new CommonImportDAL();

                string productSave = commonDal.settingValue("AutoSave", "SaleProduct", connVM, currConn, transaction);

                DataTable dtNewProducts = new DataTable();
                cmd.CommandText = getNewProducts;
                adapter = new SqlDataAdapter(cmd);

                adapter.Fill(dtNewProducts);

                int BranchId = 1;

                foreach (DataRow row in dtNewProducts.Rows)
                {
                    #region Products

                    if (row["ItemNo"].ToString().Trim() == "0" || string.IsNullOrEmpty(row["ItemNo"].ToString().Trim()))
                    {

                        if (productSave == "Y")
                        {
                            string itemCode = row["ProductCode"].ToString().Trim();
                            string itemName = row["ProductName"].ToString().Trim();
                            string uom = row["UOM"].ToString().Trim();
                            decimal varRate = !string.IsNullOrEmpty(row["VAT_Rate"].ToString())
                                ? Convert.ToDecimal(row["VAT_Rate"].ToString())
                                : 0;

                            decimal nbrPrice = !string.IsNullOrEmpty(row["CostPrice"].ToString())
                                ? Convert.ToDecimal(row["CostPrice"].ToString())
                                : 0;

                            commonImportDal.FindItemId(itemName, itemCode, currConn, transaction, true, uom, BranchId, varRate, nbrPrice, connVM);
                        }
                        else
                        {
                            throw new Exception("Product Not Found-" + row["ProductCode"].ToString() + row["ProductName"].ToString().Trim());
                        }

                    }

                    #endregion
                }

                #endregion

                #endregion

                retResults = SaveTransferIssue(filterdData, BranchCode, transactionType, CurrentUser, branchId, currConn, transaction, callBack, integration, connVM, UserId);

                #region Transfer Receive
                DataView dv = new DataView(filterdData);
                DataTable dtTransferIssue = new DataTable();
                dtTransferIssue = dv.ToTable(true, "ReferenceNo");


                TransferReceiveDAL _TransferReceiveDAL = new TransferReceiveDAL();

                if (CompanyCode == "SMC" || CompanyCode.ToLower() == "smcholding")
                {
                    retResults = _TransferReceiveDAL.ImportData(dtTransferIssue, currConn, transaction, connVM);
                    if (retResults[0] == "Fail")
                    {
                        throw new ArgumentNullException("", retResults[1]);
                    }
                }

                #endregion

                if (OrdinaryVATDesktop.IsACICompany(CompanyCode))
                {
                    ImportDAL dal = new ImportDAL();

                    retResults = dal.UpdateACITransactions(filterdData, "TransferIssues", currConn, transaction, connVM);
                }



                #region Comments


                //var ids = new List<string>();

                //foreach (DataRow tableRow in table.Rows)
                //{
                //    if (tableRow["Post"].ToString() == "Y")
                //    {
                //        ids.Add(tableRow["ID"].ToString());
                //    }
                //}

                //var getMasterWithPost = @"select * from [dbo].[TransferIssues] where ImportIDExcel in (";


                //for (int i = 0; i < ids.Count; i++)
                //{
                //    getMasterWithPost += "'"+ids[i] + "',";
                //}

                //getMasterWithPost = getMasterWithPost.TrimEnd(',');


                //getMasterWithPost += ")";

                //table.Clear();

                //if (ids.Count > 0)
                //{
                //    cmd.CommandText = getMasterWithPost;

                //    adapter = new SqlDataAdapter(cmd);

                //    adapter.Fill(table);


                //    foreach (DataRow row in table.Rows)
                //    {
                //        var vm = new TransferIssueVM
                //        {
                //            TransferIssueNo = row["TransferIssueNo"].ToString()
                //        };

                //        PostTransfer(vm, currConn, transaction);
                //    }
                //}
                #endregion

                #region Commit
                if (Vtransaction == null)
                {
                    transaction.Commit();
                }
                #endregion Commit

                #region SuccessResult
                retResults[0] = "Success";
                retResults[1] = "Data Synchronized Successfully.";
                retResults[2] = "";
                #endregion SuccessResult

            }

            #region Catch and Finall
            catch (Exception ex)
            {
                //retResults[0] = "Fail";//Success or Fail
                //retResults[4] = ex.Message.ToString(); //catch ex

                FileLogger.Log("TransferIssueDAL", "TransferIssue", ex.ToString());

                if (transaction != null && Vtransaction == null)
                {
                    transaction.Rollback();
                    //transaction.Commit();
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
            #endregion

            #region Results
            return retResults;
            #endregion
        }

        public string[] SaveTempTransfer_Integration(DataTable data, string BranchCode, string transactionType, string CurrentUser, int branchId, Action callBack, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, bool integration = false, SysDBInfoVMTemp connVM = null)
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
            retResults[5] = "Insert"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion

            try
            {
                CommonDAL commonDAL = new CommonDAL();
                CommonDAL commonDal = new CommonDAL();

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

                string duplicate = commonDAL.settings("Import", "TransferIssueDuplicateInsert", currConn, transaction, connVM);



                #region Merge DateTime

                if (data.Columns.Contains("Transaction_Date") && data.Columns.Contains("Transaction_Time"))
                {
                    if (!data.Columns.Contains("TransactionDateTime"))
                    {
                        data.Columns.Add("TransactionDateTime");
                    }

                    foreach (DataRow dataRow in data.Rows)
                    {
                        dataRow["TransactionDateTime"] = OrdinaryVATDesktop.DateToDate_YMD(dataRow["Transaction_Date"].ToString()) + " " + OrdinaryVATDesktop.DateToTime_HMS(dataRow["Transaction_Time"].ToString());

                    }

                    data.Columns.Remove("Transaction_Date");
                    data.Columns.Remove("Transaction_Time");
                }
                else
                {
                    foreach (DataRow dataRow in data.Rows)
                    {
                        dataRow["TransactionDateTime"] = OrdinaryVATDesktop.DateToDate(dataRow["TransactionDateTime"].ToString());

                    }

                }

                #endregion

                #region Clear TempTransferData

                sqlText = @"delete from TempTransferData where UserId =" + CurrentUser;

                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);

                cmd.ExecuteNonQuery();

                #endregion

                #region Bulk Insert

                string[] bulkRes = commonDal.BulkInsert("TempTransferData", data, currConn, transaction, 10000, null, connVM);

                #endregion

                #region SqlText

                string updateItemNo = @"
update TempTransferData set ItemNo = Products.ItemNo from Products where Products.ProductCode = TempTransferData.ProductCode and (TempTransferData.ItemNo is null or TempTransferData.ItemNo = '0');
update TempTransferData set ItemNo = Products.ItemNo from Products where Products.ProductName = TempTransferData.ProductName and (TempTransferData.ItemNo is null or TempTransferData.ItemNo = '0');
update TempTransferData set ItemNo = ProductDetails.ItemNo 
from ProductDetails where TempTransferData.ProductName = ProductDetails.ProductName  and (TempTransferData.ItemNo is null or TempTransferData.ItemNo = '0');
";

                string branchUpdate = @"update TempTransferData set BranchId = BranchProfiles.BranchID from BranchProfiles where BranchProfiles.BranchCode = TempTransferData.BranchCode;
update TempTransferData set TransferToBranchId = BranchProfiles.BranchID from BranchProfiles where BranchProfiles.BranchCode = TempTransferData.TransferToBranchCode;";


                string updateCostPrice = @"
update TempTransferData set CostPrice = Products.NBRPrice 
from Products 
where Products.ProductCode = TempTransferData.ProductCode and (ISNULL(TempTransferData.CostPrice,0)=0);
";

                string deleteDuplicate = @"delete from TempTransferData where ID in (
select TempTransferData.ID from 
TempTransferData join TransferIssues on TempTransferData.ID = TransferIssues.ImportIDExcel
)";

                string getNewProducts = @"
select * from TempTransferData where ISNULL(ItemNo,0) = 0";

                string updateVatSD = @"update TempTransferData set   SubTotal= CostPrice*Quantity
 where SubTotal<=0 or SubTotal is null

update TempTransferData set  SDAmount=(SubTotal*SDRate)/100
 where SDAmount<=0 or SDAmount is null

update TempTransferData set  VATAmount=(SubTotal+SDAmount)*VAT_Rate/100
 where VATAmount<=0 or VATAmount is null

update TempTransferData set UOMn = Products.UOM from Products 
where TempTransferData.ItemNo = Products.ItemNo;

update TempTransferData set TempTransferData.UOMc = UOMs.Convertion from UOMs 
where  UOMs.UOMFrom = TempTransferData.UOMn 
and UOMs.UOMTo = TempTransferData.UOM and (TempTransferData.UOMc = 0 or TempTransferData.UOMc is null)

update TempTransferData set UOMc = (case when UOM = UOMn then 1.00 else UOMc end)
where UOMc = 0 or UOMc is null

update TempTransferData set UOMPrice = CostPrice;

update TempTransferData set UOMQty = UOMc * Quantity where UOMQty = 0 or UOMQty is null ;";

                string getAll = @"select * from TempTransferData";

                #endregion

                #region SqlExecution

                #region Delete Duplicate
                if (duplicate.ToLower() == "n")
                {
                    cmd.CommandText = deleteDuplicate;

                    int deletedData = cmd.ExecuteNonQuery();
                }
                #endregion


                cmd.CommandText = updateItemNo + " " + branchUpdate + " " + updateCostPrice + " " + updateVatSD;

                cmd.ExecuteNonQuery();

                //DataTable table = new DataTable();

                //cmd.CommandText = getAll;
                //SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                //adapter.Fill(table);

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
                retResults[0] = "Success";
                retResults[1] = "Data Synchronized Successfully.";
                retResults[2] = "";
                #endregion SuccessResult

            }

            #region Catch and Finall
            catch (Exception ex)
            {
                //retResults[0] = "Fail";//Success or Fail
                //retResults[4] = ex.Message.ToString(); //catch ex
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

        private string[] SaveTransferIssue(DataTable dtTableResult, string BranchCode, string transactionType, string CurrentUser, int branchId, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, Action callBack = null, bool integration = false, SysDBInfoVMTemp connVM = null, string UserId = "")
        {
            #region Variables

            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";


            DataTable dtTransferM = new System.Data.DataTable();
            DataTable dtTransferD = new System.Data.DataTable();
            TransferIssueVM Master = new TransferIssueVM();
            List<TransferIssueDetailVM> Details = new List<TransferIssueDetailVM>();

            #endregion

            try
            {
                if (!dtTableResult.Columns.Contains("SerialNo"))
                {
                    DataColumn column = new DataColumn("SerialNo") { DefaultValue = "-" };
                    dtTableResult.Columns.Add(column);
                }

                DataView view = new DataView(dtTableResult);

                dtTransferM = view.ToTable(true, "ID", "TransferToBranchCode", "TransactionDateTime", "SerialNo", "ReferenceNo",
                    "TransactionType",
                    "Comments", "Post", "BranchCode", "TransferToBranchId", "BranchId", "VehicleNo", "VehicleType", "BranchFromRef", "BranchToRef");
                dtTransferD = view.ToTable(true, "ID", "TransactionDateTime", "ReferenceNo", "Comments",
                    "Post", "ProductCode", "ProductName", "Quantity", "Weight", "UOM", "CommentsD", "ItemNo", "SerialNo", "CostPrice", "VAT_Rate", "OtherRef");


                // dtTransferM = OrdinaryVATDesktop.DtColumnAdd(dtTransferM, "TransactionType", transactionType, "string");
                string today = OrdinaryVATDesktop.DateToDate(DateTime.Now.ToString());

                dtTransferM = OrdinaryVATDesktop.DtColumnAdd(dtTransferM, "CreatedBy", CurrentUser, "string");
                dtTransferM = OrdinaryVATDesktop.DtColumnAdd(dtTransferM, "CreatedOn", today, "string");
                dtTransferM = OrdinaryVATDesktop.DtColumnAdd(dtTransferM, "LastModifiedBy", CurrentUser, "string");
                dtTransferM = OrdinaryVATDesktop.DtColumnAdd(dtTransferM, "LastModifiedOn", today, "string");


                dtTransferM = OrdinaryVATDesktop.DtDateCheck(dtTransferM, new string[] { "TransactionDateTime" });
                dtTransferD = OrdinaryVATDesktop.DtDateCheck(dtTransferD, new string[] { "TransactionDateTime" });
                retResults = ImportData(dtTransferM, dtTransferD, branchId, VcurrConn, Vtransaction, callBack, integration, connVM, UserId);
                return retResults;
            }
            catch (Exception ex)
            {
                FileLogger.Log("TransferIssueDAL", "SaveTransferIssue", ex.ToString());
                throw ex;
            }


        }

        public DataSet TransferMovement(string ItemNo, string FDate, string TDate, int BranchId = 0, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, bool Summery = false, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            DataSet dataSet = new DataSet("ReportVAT17");


            #endregion

            #region Try

            try
            {
                #region vat19 value

                string vExportInBDT = "";
                CommonDAL commonDal = new CommonDAL();
                vExportInBDT = commonDal.settings("VAT9_1", "ExportInBDT", null, null, connVM);

                #endregion vat19 value

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


                string top = "";
                sqlText = " ";


                #region SQL

                sqlText += @"
--declare @FDate as date='01/01/2019'
--declare @TDate as date='01/01/2029'
--declare @BranchId as varchar(10)='1'
--declare @ItemNo as varchar(100)='7324'
";
                if (Summery == true)
                {
                    sqlText += @"

select distinct ProductCode,ProductName,sum(Opening)Opening,sum(Receive)Receive,sum(Sale)Sale,sum(Issue)Issue,sum(Balance)Balance
from
(
";
                }
                sqlText += @"
select p.ProductCode,p.ProductName, TransactionNo,TransactionDate,Opening,Receive,Sale,Issue, Quantity Balance
 from (
(
select distinct SL,TType,ItemNo,TransactionNo,TransactionDate,sum(Quantity) Opening,Receive,Sale,Issue,sum(Quantity)Quantity
  from (
SELECT 'A'SL,'Opening'TType, @itemNo ItemNo, '-' TransactionNo, @FDate TransactionDate
, 0 Opening,0 Receive,0 Sale,0 Issue, isnull(OpeningBalance,0) Quantity  
FROM Products p  
WHERE 1=1
and ItemNo=@itemNo  
AND BranchId=@BranchId

union all
select 'A'SL,'Opening'TType,ItemNo, TransferReceiveNo TransactionNo,@FDate TransactionDate
,0 Opening,0 Receive,0 Sale,0 Issue,UOMQty Quantity 
from TransferReceiveDetails
WHERE 1=1
and ItemNo=@itemNo  
AND BranchId=@BranchId
 and Post='Y'
 and TransactionDateTime < @FDate 
union all
 
 select 'A'SL,'Opening'TType, ItemNo, TransferIssueNo,@FDate
 ,0 Opening,0 Receive,0 Sale,0 Issue
 ,-1*UOMQty  Quantity
 from TransferIssueDetails
WHERE 1=1
and ItemNo=@itemNo  
AND BranchId=@BranchId
 and Post='Y'
 and TransactionDateTime < @FDate 
union all

select 'A'SL,'Opening'TType, ItemNo, SalesInvoiceNo,@FDate
 ,0 Opening,0 Receive,0 Sale,0 Issue ,-1*UOMQty Quantity
 from SalesInvoiceDetails
WHERE 1=1
and ItemNo=@itemNo  
AND BranchId=@BranchId
 and Post='Y'
 and TransactionType in('other')
 and InvoiceDateTime < @FDate  
 ) as a
 group by SL,TType,ItemNo,TransactionNo,TransactionDate,Opening,Receive,Sale,Issue)


union all
select 'B'SL,'Receive'TType,ItemNo, TransferReceiveNo TransactionNo,TransactionDateTime TransactionDate
,0 Opening,UOMQty Receive,0 Sale,0 Issue,UOMQty Quantity 
from TransferReceiveDetails
WHERE 1=1
and ItemNo=@itemNo  
AND BranchId=@BranchId
 and Post='Y'
 and TransactionDateTime >= @FDate and TransactionDateTime< DATEADD(d,1,@TDate)
union all
 
 select 'C'SL,'Issue'TType, ItemNo, TransferIssueNo,TransactionDateTime
 ,0 Opening,0 Receive,0 Sale,UOMQty Issue
 ,-1*UOMQty  Quantity
 from TransferIssueDetails
WHERE 1=1
and ItemNo=@itemNo  
AND BranchId=@BranchId
 and Post='Y'
 and TransactionDateTime >= @FDate and TransactionDateTime< DATEADD(d,1,@TDate)
union all

select 'D'SL,'Sale'TType, ItemNo, SalesInvoiceNo,InvoiceDateTime
 ,0 Opening,0 Receive,UOMQty Sale,0 Issue ,-1*UOMQty Quantity
 from SalesInvoiceDetails
WHERE 1=1
and ItemNo=@itemNo  
AND BranchId=@BranchId
 and Post='Y'
 and TransactionType in('other')
 and InvoiceDateTime >= @FDate and InvoiceDateTime< DATEADD(d,1,@TDate)
 ) as a
 left outer join Products p on a.ItemNo=p.itemNo
";
                if (Summery == true)
                {
                    sqlText += @"
 --order by a.ItemNo,TransactionDate,Sl
 ) as a
 group by  ProductCode,ProductName

                ";
                }
                else
                {
                    sqlText += @"
 order by a.ItemNo,TransactionDate,Sl

                ";
                }

                #endregion SQL

                if (ItemNo == "0")
                {
                    sqlText = sqlText.Replace("ItemNo=@itemNo", "2=2");
                }
                top = "A";



                #region SQL Command

                SqlCommand objCommVAT17 = new SqlCommand(sqlText, currConn, transaction);


                #endregion

                #region Parameter
                objCommVAT17.Parameters.AddWithValue("@BranchId", BranchId);



                if (!objCommVAT17.Parameters.Contains("@ItemNo"))
                {
                    objCommVAT17.Parameters.AddWithValue("@ItemNo", ItemNo);
                }
                else
                {
                    objCommVAT17.Parameters["@ItemNo"].Value = ItemNo;
                }

                if (!objCommVAT17.Parameters.Contains("@FDate"))
                {
                    objCommVAT17.Parameters.AddWithValue("@FDate", FDate);
                }
                else
                {
                    objCommVAT17.Parameters["@FDate"].Value = FDate;
                }
                if (!objCommVAT17.Parameters.Contains("@TDate"))
                {
                    objCommVAT17.Parameters.AddWithValue("@TDate", TDate);
                }
                else
                {
                    objCommVAT17.Parameters["@TDate"].Value = TDate;
                }

                #endregion Parameter

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommVAT17);
                dataAdapter.Fill(dataSet);

            }
            #endregion

            #region Catch & Finally

            catch (SqlException sqlex)
            {
                throw sqlex;
            }
            catch (Exception ex)
            {
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

            return dataSet;
        }

        public DataSet TransferStock(string ItemNo, string FDate, int BranchId = 0
            , SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            DataSet dataSet = new DataSet("ReportVAT17");


            #endregion

            #region Try

            try
            {
                #region vat19 value

                string vExportInBDT = "";
                CommonDAL commonDal = new CommonDAL();
                vExportInBDT = commonDal.settings("VAT9_1", "ExportInBDT");

                #endregion vat19 value

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


                string top = "";
                sqlText = " ";


                #region SQL

                sqlText += @"
                 select distinct ItemNo, sum(Quantity)Quantity,sum(Amount)Amount from 
 (
SELECT @ItemNo ItemNo, -1*isnull(sum(p.UOMQty),0) Quantity, -1*isnull(sum(p.UOMPrice*p.UOMQty),0) Amount  
FROM TransferIssueDetails p 
 WHERE 1=1
 and p.Post='Y'
 and  p.ItemNo = @ItemNo
AND BranchId=@BranchId
 and p.TransactionDateTime< dateadd(d,1, @StartDate ) 
 union all
SELECT @ItemNo ItemNo, isnull(sum(p.UOMQty),0) Quantity, isnull(sum(p.UOMPrice*p.UOMQty),0) Amount  
FROM TransferReceiveDetails p 
 WHERE 1=1
 and p.Post='Y'
 and  p.ItemNo = @ItemNo
 and p.TransactionDateTime< dateadd(d,1, @StartDate ) 
AND BranchId=@BranchId
)
as a
group by ItemNo

                ";

                #endregion SQL


                top = "A";



                #region SQL Command

                SqlCommand objCommVAT17 = new SqlCommand(sqlText, currConn, transaction);


                #endregion

                #region Parameter
                objCommVAT17.Parameters.AddWithValue("@BranchId", BranchId);



                if (!objCommVAT17.Parameters.Contains("@ItemNo"))
                {
                    objCommVAT17.Parameters.AddWithValue("@ItemNo", ItemNo);
                }
                else
                {
                    objCommVAT17.Parameters["@ItemNo"].Value = ItemNo;
                }

                if (!objCommVAT17.Parameters.Contains("@StartDate"))
                {
                    objCommVAT17.Parameters.AddWithValue("@StartDate", FDate);
                }
                else
                {
                    objCommVAT17.Parameters["@StartDate"].Value = FDate;
                }


                #endregion Parameter

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommVAT17);
                dataAdapter.Fill(dataSet);

            }
            #endregion

            #region Catch & Finally

            catch (SqlException sqlex)
            {
                throw sqlex;
            }
            catch (Exception ex)
            {
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

            return dataSet;
        }


        public DataTable SearchByItemNo(string ItemNo, SysDBInfoVMTemp connVM = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            string sqlText = "";

            DataTable dataTable = new DataTable("Product");

            #endregion

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

                #region sql statement

                sqlText = @"SELECT
                                    Products.ItemNo,
                                    isnull(Products.ProductName,'N/A')ProductName,
                                    isnull(Products.ProductDescription,'N/A')ProductDescription,
                                    Products.CategoryID, 
                                    isnull(ProductCategories.CategoryName,'N/A')CategoryName ,
                                    isnull(Products.UOM,'N/A')UOM,
                                    isnull(Products.OpeningTotalCost,0)OpeningTotalCost,
                                    isnull(Products.CostPrice,0)CostPrice,
                                    isnull(Products.SalesPrice,0)SalesPrice,
                                    isnull(Products.NBRPrice,0)NBRPrice,
                                    isnull(ProductCategories.IsRaw,'N')IsRaw,
                                    isnull(Products.SerialNo,'N/A')SerialNo ,
                                    isnull(Products.HSCodeNo,'N/A')HSCodeNo,
                                    isnull(Products.VATRate,0)VATRate,
                                    isnull(Products.ActiveStatus,'N')ActiveStatus,
                                    isnull(Products.OpeningBalance,0)OpeningBalance,
                                    isnull(Products.Comments,'N/A')Comments,
                                    'N/A' HSDescription, 
                                    isnull(isnull(Products.OpeningBalance,0)+isnull(Products.QuantityInHand,0),0) as Stock,
                                    isnull(Products.SD,0)SD, 
                                    isnull(Products.Packetprice,0)Packetprice, Products.Trading, 
                                    Products.TradingMarkUp,Products.NonStock,
                                    isnull(Products.QuantityInHand,0)QuantityInHand,
                                    convert(varchar, Products.OpeningDate,120)OpeningDate,
                                    isnull(Products.ReceivePrice,0)ReceivePrice,
                                    isnull(Products.IssuePrice,0)IssuePrice,
                                    isnull(Products.ProductCode,'N/A')ProductCode,
                                    isnull(Products.RebatePercent,0)RebatePercent,
                                    isnull(Products.TollCharge,0)TollCharge,
                                    isnull(Products.Banderol,'N')Banderol,
                                    isnull(Products.IsTransport,'N')IsTransport,
                                    isnull(Products.TDSCode,'-')TDSCode,
isnull(Products.FixedVATAmount,0)FixedVATAmount,
isnull(Products.IsFixedVAT,'N')IsFixedVAT,
                                    isnull(Products.TollProduct,'N')TollProduct

                                    FROM Products LEFT OUTER JOIN
                                    ProductCategories ON Products.CategoryID = ProductCategories.CategoryID 
                      
                                    WHERE (Products.ItemNo = @ItemNo)  ";

                SqlCommand objCommProduct = new SqlCommand();
                objCommProduct.Connection = currConn;
                objCommProduct.Transaction = transaction;
                objCommProduct.CommandText = sqlText;
                objCommProduct.CommandType = CommandType.Text;

                if (!objCommProduct.Parameters.Contains("@ItemNo"))
                { objCommProduct.Parameters.AddWithValue("@ItemNo", ItemNo); }
                else { objCommProduct.Parameters["@ItemNo"].Value = ItemNo; }


                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommProduct);
                dataAdapter.Fill(dataTable);

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

        public string[] SaveTransferIssue_Split(IntegrationParam param, string UserId = "", SysDBInfoVMTemp connVM = null)
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


                CommonDAL commonDal = new CommonDAL();

                #region delete and bulk insert to Source

                string deleteSource = @"delete from VAT_Source_TransferIssues";
                SqlCommand cmd = new SqlCommand(deleteSource, currConn, transaction);
                cmd.ExecuteNonQuery();

                param.Data.Columns.Add("TransactionDateTime");


                string[] result = commonDal.BulkInsert("VAT_Source_TransferIssues", param.Data, currConn, transaction, 10000, null, connVM);

                #endregion




                #region delete duplicate

                string deleteDuplicate = @"
update  VAT_Source_TransferIssues                             
set PeriodId=RIGHT('0'+CONVERT(VARCHAR(2),MONTH(Transaction_Date)) +  CONVERT(VARCHAR(4),YEAR(Transaction_Date)),6)
where PeriodId is null or PeriodId = ''

delete from VAT_Source_TransferIssues where ID in (
                select vr.ID from VAT_Source_TransferIssues vr inner join TransferIssues rh
                on vr.ID = rh.ImportIDExcel and vr.PeriodId = rh.PeriodId)";

                cmd.CommandText = deleteDuplicate;
                cmd.ExecuteNonQuery();
                #endregion

                #region Loop counter

                string getLoopCount = @"select Ceiling(count(distinct ID)/10.00) from VAT_Source_TransferIssues";

                cmd.CommandText = getLoopCount;
                int counter = Convert.ToInt32(cmd.ExecuteScalar());

                #endregion


                transaction.Commit();
                currConn.Close();

                DataTable sourceData = new DataTable();

                if (counter == 0)
                {
                    retResults[0] = "Fail";
                    retResults[1] = "All Data already exists!";
                    return retResults;
                }

                for (int i = 0; i < counter; i++)
                {
                    currConn = _dbsqlConnection.GetConnection();
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
select  distinct top 10 ID 
from VAT_Source_TransferIssues
where isnull(IsProcessed,'N') = 'N'";

                    cmd.CommandText = insertIds;
                    cmd.ExecuteNonQuery();

                    string getData = @"SELECT
      ID 
      ,BranchCode 
	  ,Transaction_Date
	  ,Transaction_Time
      ,ProductCode
      ,ProductName
      ,Quantity
      ,UOM
      ,Weight
      ,CostPrice
      ,TransferToBranchCode
      ,ReferenceNo
      ,VehicleNo
	  ,VehicleType
      ,TransactionType
      ,Post
      ,VAT_Rate
      ,Comments
      ,CommentsD

  FROM VAT_Source_TransferIssues where ID in (select ID from #tempIds)";

                    cmd.CommandText = getData;
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(sourceData);

                    //sourceData.Columns.Remove("IsProcessed");

                    #endregion

                    retResults = SaveTempTransfer(sourceData, param.BranchCode, param.TransactionType, param.CurrentUser, param.DefaultBranchId,
                        param.callBack, currConn, transaction, false, connVM, "", UserId);


                    #region updateSourceTable

                    string updateSourceAndClearTemp = @"update VAT_Source_TransferIssues set IsProcessed = 'Y' where ID  in (select ID from #tempIds);
                                            --delete from #tempIds;";

                    cmd.CommandText = updateSourceAndClearTemp;
                    cmd.ExecuteNonQuery();

                    #endregion


                    transaction.Commit();
                    currConn.Close();
                    transaction.Dispose();
                    currConn.Dispose();

                    sourceData.Clear();
                }

                return retResults;
            }
            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
                if (transaction != null) { transaction.Rollback(); }

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


        public MISExcelVM TransferIssueMISExcelDownload(string IssueNo, string IssueDateFrom, string IssueDateTo, string TType, int BranchId = 0
            , int TransferTo = 0, SysDBInfoVMTemp connVM = null, string ShiftId = "0")
        {
            try
            {
                ReportDSDAL reportDsdal = new ReportDSDAL();
                MISExcelVM misExcelObj = new MISExcelVM();
                misExcelObj.FileName = "TransferIssueInformation";
                DataSet ReportResult = reportDsdal.TransferIssueOutReport(IssueNo, IssueDateFrom, IssueDateTo, TType
                    , BranchId, TransferTo, connVM, ShiftId);

                misExcelObj.varExcelPackage = SaveExcel(ReportResult, "", connVM);

                return misExcelObj;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public ExcelPackage SaveExcel(DataSet ds, string ReportType = "", SysDBInfoVMTemp connVM = null)
        {
            DataTable dt = new DataTable();
            DataTable dtresult = ds.Tables["Table"];

            DataView dataView = new DataView(dtresult);
            dt = dataView.ToTable(true, "TransferIssueNo", "TransactionDateTime", "BranchCode", "BranchName", "BranchFrom", "BranchFromCode"
                , "ProductCode", "ProductName", "Quantity", "UOM", "SubTotal", "SDAmount", "VATAmount", "TransactionType");
            DataTable dtComapnyProfile = new DataTable();

            DataSet tempDS = new DataSet();
            tempDS = new ReportDSDAL().ComapnyProfile("", connVM);

            dtComapnyProfile = tempDS.Tables[0].Copy();
            string ComapnyName = dtComapnyProfile.Rows[0]["CompanyLegalName"].ToString();
            string VatRegistrationNo = dtComapnyProfile.Rows[0]["VatRegistrationNo"].ToString();
            string Address1 = dtComapnyProfile.Rows[0]["Address1"].ToString();

            string[] ReportHeaders = new string[] { " Name of Company: " + ComapnyName, " Address: " + Address1, " e-BIN: " + VatRegistrationNo };

            dt = OrdinaryVATDesktop.DtSlColumnAdd(dt);

            string[] DtcolumnName = new string[dt.Columns.Count];
            int j = 0;
            foreach (DataColumn column in dt.Columns)
            {
                DtcolumnName[j] = column.ColumnName;
                j++;
            }

            for (int k = 0; k < DtcolumnName.Length; k++)
            {
                dt = OrdinaryVATDesktop.DtColumnNameChange(dt, DtcolumnName[k], OrdinaryVATDesktop.AddSpacesToSentence(DtcolumnName[k]));
            }

            string pathRoot = AppDomain.CurrentDomain.BaseDirectory;
            string fileDirectory = pathRoot + "//Excel Files";
            Directory.CreateDirectory(fileDirectory);

            fileDirectory += "\\" + ReportType + "-" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss") + ".xlsx";
            FileStream objFileStrm = File.Create(fileDirectory);

            int TableHeadRow = 0;
            TableHeadRow = ReportHeaders.Length + 2;

            int RowCount = 0;
            RowCount = dt.Rows.Count;

            int ColumnCount = 0;
            ColumnCount = dt.Columns.Count;

            int GrandTotalRow = 0;
            GrandTotalRow = TableHeadRow + RowCount + 1;
            string sheetName = "SaleInformation";
            if (string.IsNullOrEmpty(sheetName))
            {
                sheetName = ReportType;
            }

            ExcelPackage package = new ExcelPackage(objFileStrm);

            ExcelWorksheet ws = package.Workbook.Worksheets.Add(sheetName);
            ws.Cells[TableHeadRow, 1].LoadFromDataTable(dt, true);

            #region Format

            ExcelTextFormat format = new OfficeOpenXml.ExcelTextFormat();
            format.Delimiter = '~';
            format.TextQualifier = '"';
            format.DataTypes = new[] { eDataTypes.String };



            for (int i = 0; i < ReportHeaders.Length; i++)
            {
                ws.Cells[i + 1, 1, (i + 1), ColumnCount].Merge = true;
                ws.Cells[i + 1, 1, (i + 1), ColumnCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[i + 1, 1, (i + 1), ColumnCount].Style.Font.Size = 16 - i;
                ws.Cells[i + 1, 1].LoadFromText(ReportHeaders[i], format);

            }
            int colNumber = 0;

            foreach (DataColumn col in dt.Columns)
            {
                colNumber++;
                if (col.DataType == typeof(DateTime))
                {
                    ws.Column(colNumber).Style.Numberformat.Format = "dd-MMM-yyyy hh:mm:ss AM/PM";
                }
                else if (col.DataType == typeof(Decimal))
                {

                    ws.Column(colNumber).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";

                    #region Grand Total
                    ws.Cells[GrandTotalRow, colNumber].Formula = "=Sum(" + ws.Cells[TableHeadRow + 1, colNumber].Address + ":" + ws.Cells[(TableHeadRow + RowCount), colNumber].Address + ")";
                    #endregion
                }

            }
            ws.Cells[TableHeadRow, 1, TableHeadRow, ColumnCount].Style.Font.Bold = true;
            ws.Cells[GrandTotalRow, 1, GrandTotalRow, ColumnCount].Style.Font.Bold = true;

            ws.Cells["A" + (TableHeadRow) + ":" + OrdinaryVATDesktop.Alphabet[(ColumnCount - 1)] + (TableHeadRow + RowCount + 2)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            ws.Cells["A" + (TableHeadRow) + ":" + OrdinaryVATDesktop.Alphabet[(ColumnCount)] + (TableHeadRow + RowCount + 1)].Style.Border.Left.Style = ExcelBorderStyle.Thin;

            ws.Cells[GrandTotalRow, 1].LoadFromText("Grand Total");
            #endregion

            return package;
        }

        public DataTable SearchTransferIssueTrackItems(string itemNo, string transactionId, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";

            DataTable dataTable = new DataTable();

            #endregion

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
                #region Parameter

                #endregion

                sqlText = "";
                sqlText += @"

--Declare @itemNo varchar(100) 
--Declare @isTransaction varchar(100) 
--Declare @transactionId varchar(100) 

--SET @itemNo='19'
--SET @isTransaction='N'
--SET @transactionId =''

SELECT pr.[ProductCode]
	  ,pr.[ProductName] 
      ,t.[ItemNo]
	  ,t.[TrackLineNo]
      ,t.[Heading1]
      ,t.[Heading2]
      ,t.[IsTransfer]
      ,t.[TransferIssueNo]
      ,t.[FinishItemNo]

  FROM TransferIssueTrackings t,Products pr
  WHERE t.[ItemNo]=pr.[ItemNo] 
--And (t.IsPurchase='Y' OR t.Post='Y') 
--And t.Post='Y' 
AND t.ItemNo=@itemNo

";
                if (!string.IsNullOrEmpty(transactionId))
                {
                    sqlText += @" and t.TransferIssueNo=@transactionId";
                }

                SqlCommand objCommProduct = new SqlCommand();
                objCommProduct.Connection = currConn;
                objCommProduct.CommandText = sqlText;
                objCommProduct.CommandType = CommandType.Text;

                if (itemNo == "" || string.IsNullOrEmpty(itemNo))
                {
                    if (!objCommProduct.Parameters.Contains("@itemNo"))
                    { objCommProduct.Parameters.AddWithValue("@itemNo", System.DBNull.Value); }
                    else { objCommProduct.Parameters["@itemNo"].Value = System.DBNull.Value; }
                }
                else
                {
                    if (!objCommProduct.Parameters.Contains("@itemNo"))
                    { objCommProduct.Parameters.AddWithValue("@itemNo", itemNo); }
                    else { objCommProduct.Parameters["@itemNo"].Value = itemNo; }
                }

                if (!string.IsNullOrEmpty(transactionId))
                {
                    objCommProduct.Parameters.AddWithValue("@transactionId", transactionId);
                }


                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommProduct);
                dataAdapter.Fill(dataTable);

                #endregion
            }
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
    }
}
