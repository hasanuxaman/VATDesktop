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
    public class SaleMPLDAL : ISaleMPL
    {
        #region Global Variables

        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        CommonDAL _cDAL = new CommonDAL();
        SaleDAL _SaleDAL = new SaleDAL();

        #endregion

        public List<EnumModeOfPaymentVM> DropDown(string type = "SA-01", SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<EnumModeOfPaymentVM> VMs = new List<EnumModeOfPaymentVM>();
            EnumModeOfPaymentVM vm;
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
                sqlText = @" Select 
Id
,Code
,Name
,Type
,IsActive
From EnumModeOfPayment
WHERE  1=1 AND IsActive = 'Y' and Type=@Type
";


                SqlCommand objComm = new SqlCommand(sqlText, currConn);
                objComm.Parameters.AddWithValueAndNullHandle("@Type", type);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EnumModeOfPaymentVM();
                    vm.Code = dr["Code"].ToString();
                    vm.Name = dr["Name"].ToString();
                    VMs.Add(vm);
                }
                dr.Close();
                #endregion
            }
            #endregion

            #region catch
            catch (SqlException sqlex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());

                FileLogger.Log("SaleMPLDAL", "DropDown", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());

                FileLogger.Log("SaleMPLDAL", "DropDown", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

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
            return VMs;
        }

        public List<EnumVehicleTypeVM> DropDownEnumVehicleType(string type = "",SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<EnumVehicleTypeVM> VMs = new List<EnumVehicleTypeVM>();
            EnumVehicleTypeVM vm;
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
                sqlText = @" Select 
Id
,Code
,Name
,IsActive
From EnumVehicleType
WHERE  1=1 AND IsActive = 'Y' 
";
                if (!string.IsNullOrWhiteSpace(type))
                {
                    sqlText+= @" AND Type=@Type ";
                }

                SqlCommand objComm = new SqlCommand(sqlText, currConn);
                if (!string.IsNullOrWhiteSpace(type))
                {
                    objComm.Parameters.AddWithValueAndNullHandle("@Type", type);

                }
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EnumVehicleTypeVM();
                    vm.Code = dr["Code"].ToString();
                    vm.Name = dr["Name"].ToString();
                    VMs.Add(vm);
                }
                dr.Close();
                #endregion
            }
            #endregion

            #region catch
            catch (SqlException sqlex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());

                FileLogger.Log("SaleMPLDAL", "DropDown", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());

                FileLogger.Log("SaleMPLDAL", "DropDown", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

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
            return VMs;
        }

        public string[] SalesMPLInsert(SalesInvoiceMPLHeaderVM MasterVM, SqlTransaction Vtransaction = null, SqlConnection VcurrConn = null, SysDBInfoVMTemp connVM = null)
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
            decimal TotalAmount = 0;
            decimal TotalPaymentAmount = 0;
            decimal TotalCRAmount = 0;
            decimal TotalNewCRAmount = 0;
            decimal TotalOtherTotal = 0;

            SaleMasterVM SaleMaster = new SaleMasterVM();
            List<SaleDetailVm> Details = new List<SaleDetailVm>();

            #endregion Initializ

            #region Try
            try
            {

                #region Validation for Header


                if (MasterVM == null)
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgNoDataToSave);
                }
                else if (Convert.ToDateTime(MasterVM.InvoiceDateTime) < DateTime.MinValue || Convert.ToDateTime(MasterVM.InvoiceDateTime) > DateTime.MaxValue)
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
                sqlText = sqlText + "select COUNT(SalesInvoiceNo) from SalesInvoiceHeaders WHERE SalesInvoiceNo=@MasterSalesInvoiceNo ";
                SqlCommand cmdExistTran = new SqlCommand(sqlText, currConn, transaction);
                cmdExistTran.Parameters.AddWithValueAndNullHandle("@MasterSalesInvoiceNo", MasterVM.SalesInvoiceNo);
                IDExist = (int)cmdExistTran.ExecuteScalar();

                if (IDExist > 0)
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgFindExistID);
                }

                #endregion Find Transaction Exist

                var latestId = "";
                var fiscalYear = "";

                #region Sale ID Create
                if (string.IsNullOrEmpty(MasterVM.TransactionType)) // start
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.msgTransactionNotDeclared);
                }
                if (string.IsNullOrEmpty(MasterVM.ReportType)) // start
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.msgTransactionNotDeclared);
                }
                #region Other

                if (MasterVM.TransactionType == "Other")
                {
                    newIDCreate = commonDal.TransactionCode("Sale", MasterVM.ReportType, "SalesInvoiceMPLHeaders", "SalesInvoiceNo",
                                              "InvoiceDateTime", MasterVM.InvoiceDateTime, MasterVM.BranchId.ToString(), currConn, transaction);

                    //New Method

                    var resultCode = commonDal.GetCurrentCode("Sale", MasterVM.ReportType, MasterVM.InvoiceDateTime, MasterVM.BranchId.ToString(), currConn,
                        transaction);

                    var newIdara = resultCode.Split('~');

                    latestId = newIdara[0];
                    fiscalYear = newIdara[1];
                }

                #endregion


                if (string.IsNullOrEmpty(newIDCreate) || newIDCreate == "")
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert,
                                                            "ID Prefetch not set please update Prefetch first");
                }


                #region checkId

                sqlText = @"
SELECT COUNT(SalesInvoiceNo) FROM SalesInvoiceMPLHeaders 
where SalesInvoiceNo = @SaleInvoiceNumber and TransactionType = @TransactionType and BranchId = @BranchId";

                var sqlCmd = new SqlCommand(sqlText, currConn, transaction);
                sqlCmd.Parameters.AddWithValue("@SaleInvoiceNumber", newIDCreate);
                sqlCmd.Parameters.AddWithValue("@TransactionType", MasterVM.TransactionType);
                sqlCmd.Parameters.AddWithValue("@BranchId", MasterVM.BranchId);

                var count = (int)sqlCmd.ExecuteScalar();

                if (count > 0)
                {
                    FileLogger.Log("SaleMPLDAL", "Insert", "SaleInvoiceNumber " + newIDCreate + " Already Exists");
                    throw new Exception("Sales Id " + newIDCreate + " Already Exists");
                }

                #endregion

                #region Check Existing Id

                sqlText = "select count(SalesInvoiceNo) from SalesInvoiceMPLHeaders where SalesInvoiceNo = @SalesInvoiceNo";

                var cmd = new SqlCommand(sqlText, currConn, transaction);
                cmd.Parameters.AddWithValue("@SalesInvoiceNo", newIDCreate);

                var count1 = (int)cmd.ExecuteScalar();

                if (count1 > 0)
                {
                    FileLogger.Log("SaleMPLDAL", "Insert", "Sales Id " + newIDCreate + " Already Exists");
                    throw new Exception("Sales Id " + newIDCreate + " Already Exists");
                }

                #endregion

                MasterVM.SalesInvoiceNo = newIDCreate;

                #endregion Purchase ID Create Not Complete

                #region ID generated completed,Insert new Information in Header

                var vTotalAmount = commonDal.decimal259(MasterVM.TotalAmount);

                MasterVM.TotalAmount = Convert.ToDecimal(commonDal.decimal259(MasterVM.TotalAmount));
                MasterVM.TotalSDAmount = Convert.ToDecimal(commonDal.decimal259(MasterVM.TotalSDAmount));
                MasterVM.TotalVATAmount = Convert.ToDecimal(commonDal.decimal259(MasterVM.TotalVATAmount));
                MasterVM.ShortExcessAmnt = Convert.ToDecimal(commonDal.decimal259(MasterVM.ShortExcessAmnt));
                MasterVM.OtherTotalAmnt = Convert.ToDecimal(commonDal.decimal259(MasterVM.OtherTotalAmnt));
                MasterVM.CurrencyRateFromBDT = Convert.ToDecimal(commonDal.decimal259(MasterVM.CurrencyRateFromBDT));

                MasterVM.InvoiceDateTime = OrdinaryVATDesktop.DateToDate(MasterVM.InvoiceDateTime);
                MasterVM.DeliveryDate = MasterVM.InvoiceDateTime;
                MasterVM.DeliveryDate = OrdinaryVATDesktop.DateToDate(MasterVM.DeliveryDate);
                MasterVM.CustomerOrderDate = OrdinaryVATDesktop.DateToDate(MasterVM.CustomerOrderDate);
                MasterVM.RailReceiptDate = OrdinaryVATDesktop.DateToDate(MasterVM.RailReceiptDate);

                retResults = SalesMPLInsertToMaster(MasterVM, currConn, transaction, null, settings);

                Id = retResults[4];

                if (retResults[0] != "Success")
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, retResults[1]);
                }

                #endregion ID generated completed,Insert new Information in Header


                #region Insert into Details(Insert complete in Header)

                if (MasterVM.SalesInvoiceMPLDetailVMs != null)
                {
                    if (MasterVM.SalesInvoiceMPLDetailVMs.Count() < 0)
                    {
                        throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgNoDataToSave);
                    }
                    int Record = 1;
                    foreach (SalesInvoiceMPLDetailVM collection in MasterVM.SalesInvoiceMPLDetailVMs)
                    {
                        ProductDAL productDal = new ProductDAL();

                        var product = productDal.SelectAll("0", new[] { "pr.ItemNo" }, new[] { collection.ItemNo }, currConn, transaction,
                      null, connVM,null).FirstOrDefault();

                      

                        #region SalesDeatils
                        SaleDetailVm SaleDetails = new SaleDetailVm();
                        SaleDetails.InvoiceLineNo = Record.ToString();
                          if (product.IsFixedVAT == "Y")
                        {
                            SaleDetails.Type = "FixedVAT";
                            collection.VATType = "FixedVAT";

                        }
                          else
                          {
                            SaleDetails.Type="VAT";
                            collection.VATType = "VAT";

                          }

                        SaleDetails.SalesInvoiceNo = newIDCreate;
                        SaleDetails.ItemNo = collection.ItemNo;
                        SaleDetails.SaleTypeD = "New";
                        collection.VATName = "VAT 4.3";

                        SaleDetails.Quantity = Convert.ToDecimal((collection.Quantity));
                        SaleDetails.PromotionalQuantity = 0;
                        if (collection.IsPackCal == "Y")
                        {
                            SaleDetails.SalesPrice = Convert.ToDecimal((collection.NBRPrice)/(collection.Quantity));
                            SaleDetails.NBRPrice = SaleDetails.SalesPrice;
                            SaleDetails.UOMPrice = SaleDetails.SalesPrice;
                        }
                        else
                        {
                            SaleDetails.SalesPrice = Convert.ToDecimal((collection.NBRPrice));
                            SaleDetails.NBRPrice = Convert.ToDecimal((collection.NBRPrice));
                            SaleDetails.UOMPrice = SaleDetails.SalesPrice;

                        }
            
                        SaleDetails.AvgRate = 0;
                        SaleDetails.VATRate = collection.VATRate;
                        SaleDetails.SubTotal = collection.SubTotal;
                        SaleDetails.VATAmount = collection.VATAmount;
                        SaleDetails.CreatedBy = MasterVM.CreatedBy;
                        SaleDetails.CreatedOn = MasterVM.CreatedOn;
                        SaleDetails.SD = 0;
                        SaleDetails.SDAmount = 0;
                        SaleDetails.VDSAmountD = 0;
                        SaleDetails.InvoiceDateTime = MasterVM.InvoiceDateTime;
                        SaleDetails.TransactionType = MasterVM.TransactionType;
                        SaleDetails.ReturnId = "0";
                        SaleDetails.Post = "N";
                        SaleDetails.TradingD = "N";
                        SaleDetails.NonStockD = "N";
                        SaleDetails.UOM = collection.UOMn;
                        SaleDetails.UOMn = collection.UOMn;
                        
                        SaleDetails.UOMQty = Convert.ToDecimal((collection.Quantity));
                        SaleDetails.UOMc = 1;
                        SaleDetails.DiscountAmount = 0;
                        SaleDetails.DiscountedNBRPrice = 0;
                        SaleDetails.DollerValue = 0;
                        SaleDetails.CurrencyValue = 0;
                      
                        SaleDetails.BranchId = MasterVM.BranchId;
                        SaleDetails.PreviousInvoiceDateTime = "1990-01-01 00:00:00.000";
                        Details.Add(SaleDetails);


                        #endregion



                        collection.UOMPrice = Convert.ToDecimal((collection.NBRPrice));
                        collection.UOMc = 1;
                        collection.UOMQty = collection.Quantity;
                        collection.SalesInvoiceMPLHeaderId = Convert.ToInt32(Id);
                        collection.BranchId = MasterVM.BranchId;
                        collection.InvoiceDateTime = MasterVM.InvoiceDateTime;
                        collection.Post = "N";
                        collection.SaleType = "New";
                        collection.VATName = "VAT 4.3";


                       

                        TotalAmount += collection.LineTotal;




                        retResults = SalesMPLInsertToDetails(collection, currConn, transaction, null, settings);
                        Record++;
                    }
                }

                if (retResults[0] != "Success")
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, retResults[1]);
                }

                #endregion Insert into Details(Insert complete in Header)

                #region Insert into BankPayment

                if (MasterVM.SalesInvoiceMPLBankPaymentVMs != null)
                {
                    if (MasterVM.SalesInvoiceMPLBankPaymentVMs.Count() < 0)
                    {
                        throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgNoDataToSave);
                    }

                    foreach (SalesInvoiceMPLBankPaymentVM collection in MasterVM.SalesInvoiceMPLBankPaymentVMs)
                    {
                        TotalPaymentAmount += collection.Amount;
                        collection.SalesInvoiceMPLHeaderId = Convert.ToInt32(Id);
                        collection.BranchId = MasterVM.BranchId;
                        retResults = SalesMPLInsertToBankPayment(collection, currConn, transaction, null, settings);

                        if (retResults[0] == "Success")
                        {
                            MPLBankPaymentReceiveVM vm = new MPLBankPaymentReceiveVM();
                            vm.Id = collection.BankPaymentReceiveId;
                            vm.SalesInvoiceRefId = Id;

                            retResults = UpdateBankPaymentReceiveInfo(vm, transaction, currConn,connVM);
                        }
                       

                    }
                }

                if (retResults[0] != "Success")
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, retResults[1]);
                }

                #region Update sale Headers

                sqlText = "";
                sqlText += @"

update SalesInvoiceMPLHeaders set TotalAmount=d.LineTotal,TotalSDAmount=d.SDAmount,TotalVATAmount=d.VATAmount
from (select distinct   SalesInvoiceMPLHeaderId, sum(LineTotal)LineTotal,sum(VATAmount)VATAmount,sum(SDAmount)SDAmount from SalesInvoiceMPLDetails
group by SalesInvoiceMPLHeaderId) d
where SalesInvoiceMPLHeaders.Id=d.SalesInvoiceMPLHeaderId
and SalesInvoiceMPLHeaders.Id = @Id



";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                cmdUpdate.CommandTimeout = 500;
                cmdUpdate.Parameters.AddWithValue("@Id", Id);

                transResult = cmdUpdate.ExecuteNonQuery();

                #endregion


                #endregion Insert into BankPayment

                if (MasterVM.ReportType=="SA-01")
                {
                 #region Insert into Credit Info

                if (MasterVM.SalesInvoiceMPLCRInfoVMs != null)
                {
                    if (MasterVM.SalesInvoiceMPLCRInfoVMs.Count() < 0)
                    {
                        throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgNoDataToSave);
                    }
                    TotalCRAmount = 0;
                    foreach (SalesInvoiceMPLCRInfoVM collection in MasterVM.SalesInvoiceMPLCRInfoVMs)
                    {
                        collection.SalesInvoiceRefId = Convert.ToInt32(Id);
                        collection.SalesInvoiceMPLHeaderId = collection.SalesInvoiceMPLHeaderId;
                        collection.IsUsed = true;

                        retResults = SalesMPLUpdateCreditInfo(collection, currConn, transaction, null);
                        TotalCRAmount += collection.Amount;


                    }
                    //foreach (SalesInvoiceMPLCRInfoVM collection in MasterVM.SalesInvoiceMPLCRInfoVMs)
                    //{
                    //    //int SalesInvoiceMPLHeaderId = collection.SalesInvoiceMPLHeaderId;
                    //    collection.CRCode = newIDCreate;
                    //    collection.SalesInvoiceMPLHeaderId = Convert.ToInt32(Id);
                    //    collection.CustomerId = Convert.ToInt32(MasterVM.CustomerID);
                    //    TotalCRAmount += collection.Amount;
                    //    //collection.SalesInvoiceRefId = SalesInvoiceMPLHeaderId;
                    //    collection.BranchId = MasterVM.BranchId;
                    //    collection.IsUsed = true;

                    //    retResults = SalesMPLInsertToCreditInfo(collection, currConn, transaction, null, settings);


                    //}
                   
                }

                if (retResults[0] != "Success")
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, retResults[1]);
                }


              

                TotalNewCRAmount = (TotalPaymentAmount + TotalCRAmount) - TotalAmount;

                if (TotalNewCRAmount != 0)
                {
                    newIDCreate = commonDal.TransactionCode("Sale", "CRInvoice", "SalesInvoiceMPLCRInfos", "CRCode",
                                          "InvoiceDateTime", MasterVM.InvoiceDateTime, MasterVM.BranchId.ToString(), currConn, transaction);
                
                    SalesInvoiceMPLCRInfoVM details = new SalesInvoiceMPLCRInfoVM();
                    details.Amount = TotalNewCRAmount;
                    details.BranchId = MasterVM.BranchId;
                    details.CustomerId = Convert.ToInt32(MasterVM.CustomerID);
                    details.SalesInvoiceMPLHeaderId = Convert.ToInt32(Id);
                    details.CRCode = newIDCreate;
                    details.CRDate = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss");
                    details.SalesInvoiceRefId = 0;
                    details.IsUsed = false;

                    retResults = SalesMPLInsertToCreditInfo(details, currConn, transaction, null, settings);

                }
                #endregion Insert into Credit Info
                }
                else
                {
                    #region Insert into Credit Info

                    if (MasterVM.SalesInvoiceMPLCRInfoVMs != null)
                    {
                        if (MasterVM.SalesInvoiceMPLCRInfoVMs.Count() < 0)
                        {
                            throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgNoDataToSave);
                        }
                        decimal totalCRAmount = MasterVM.SalesInvoiceMPLCRInfoVMs.Sum(item => item.Amount);
                       decimal NewCRAmount = (TotalPaymentAmount + totalCRAmount) - TotalAmount;

                       if (NewCRAmount != 0)
                        {
                            throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert,
                                                              "Balance Not Match,Please try to Create SA-01 Invoice");
                        }
                        TotalCRAmount = 0;
                        foreach (SalesInvoiceMPLCRInfoVM collection in MasterVM.SalesInvoiceMPLCRInfoVMs)
                        {
                            collection.SalesInvoiceRefId = Convert.ToInt32(Id);
                            collection.SalesInvoiceMPLHeaderId = collection.SalesInvoiceMPLHeaderId;
                            collection.IsUsed = true;

                            retResults = SalesMPLUpdateCreditInfo(collection, currConn, transaction, null);
                            TotalCRAmount += collection.Amount;


                        }
                        

                    }

                    if (retResults[0] != "Success")
                    {
                        throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, retResults[1]);
                    }




                    TotalNewCRAmount = (TotalPaymentAmount + TotalCRAmount) - (TotalAmount + MasterVM.OtherTotalAmnt);

                    if (TotalNewCRAmount != 0)
                    {
                        throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert,
                                                          "Balance Not Match,Please try to Create SA-01 Invoice");
                    }
                    
                    #endregion Insert into Credit Info

                }
            

                #region SaleInsert
                SaleMaster.SalesInvoiceNo = MasterVM.SalesInvoiceNo;
                SaleMaster.CustomerID = MasterVM.CustomerID;
                SaleMaster.DeliveryAddress1 = MasterVM.DeliveryAddress;
                SaleMaster.InvoiceDateTime = MasterVM.InvoiceDateTime;
                SaleMaster.DeliveryDate = MasterVM.InvoiceDateTime;
                SaleMaster.BranchId = MasterVM.BranchId;
                SaleMaster.VehicleNo = MasterVM.VehicleNo;
                SaleMaster.VehicleType = MasterVM.VehicleType;
                SaleMaster.SaleType = "New";
                SaleMaster.TransactionType = MasterVM.TransactionType;
                SaleMaster.CurrencyCode = "BDT";
                SaleMaster.CurrencyID = "260";
                SaleMaster.CurrencyRateFromBDT = 1;
                SaleMaster.Post = "N";
                SaleMaster.IsPrint = "N";
                SaleMaster.IsGatePass = "N";
                SaleMaster.IsDeemedExport = "N";
                SaleMaster.TransactionType = MasterVM.TransactionType;
                SaleMaster.CreatedBy = MasterVM.CreatedBy;
                SaleMaster.CreatedOn = MasterVM.CreatedOn;
                SaleMaster.FiscalYear = fiscalYear;
                SaleMaster.LastModifiedBy = MasterVM.CreatedOn;
                SaleMaster.LastModifiedOn = MasterVM.CreatedOn;
                retResults = _SaleDAL.SalesInsert(SaleMaster, Details, null, null, transaction, currConn, SaleMaster.BranchId, connVM, MasterVM.CreatedBy, true, null, false);
                #endregion

                #region Commit

                if (retResults[0] != "Success")
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, retResults[1]);
                }

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion

                #region SuccessResult
                retResults = new string[5];
                retResults[0] = "Success";
                retResults[1] = MessageVM.saleMsgSaveSuccessfully;
                retResults[2] = "" + MasterVM.SalesInvoiceNo;
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
                try
                {
                    if (Vtransaction == null)
                    {
                        transaction.Rollback();
                    }
                }
                catch (Exception e)
                { 

                }

                
                FileLogger.Log("SaleDAL", "SalesInsert", ex.ToString() + "\n" + sqlText);

            }
            finally
            {

                #region Comment 28 Oct 2020


                #endregion
                try
                {
                    if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
                catch (Exception e)
                {

                }
                


            }
            #endregion Catch and Finall

            #region Result
            return retResults;
            #endregion Result
        }

        public string[] SalesMPLInsertToMaster(SalesInvoiceMPLHeaderVM Master, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null, DataTable settingsDt = null)
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

                #region Entry Date Check

                CommonDAL commonDal = new CommonDAL();


                #endregion

                #region Insert

                sqlText = "";
                sqlText += " INSERT INTO SalesInvoiceMPLHeaders";
                sqlText += " (";
                sqlText += " SalesInvoiceNo";
                sqlText += " ,BranchId";
                sqlText += " ,CustomerID";
                sqlText += " ,DeliveryAddress";
                sqlText += " ,InvoiceDateTime";
                sqlText += " ,DeliveryDate";
                sqlText += " ,TransactionType";
                sqlText += " ,SaleType";
                sqlText += " ,ReportType";
                sqlText += " ,VehicleType";
                sqlText += " ,VehicleNo";
                sqlText += " ,TotalAmount";
                sqlText += " ,TotalSDAmount";
                sqlText += " ,TotalVATAmount";
                sqlText += " ,SerialNo";
                sqlText += " ,Comments";
                sqlText += " ,CreatedBy";
                sqlText += " ,CreatedOn";
                sqlText += " ,IsPrint";
                sqlText += " ,Post";
                sqlText += " ,CurrencyID";
                sqlText += " ,CurrencyRateFromBDT";
                sqlText += " ,AlReadyPrint";
                sqlText += " ,CustomerOrder";
                sqlText += " ,CustomerOrderDate";
                sqlText += " ,Tarcat";
                sqlText += " ,SupplyVAT";
                sqlText += " ,TC";
                sqlText += " ,LF";
                sqlText += " ,RF";
                sqlText += " ,ShortExcessAmnt";
                sqlText += " ,Toll";
                sqlText += " ,DC";
                sqlText += " ,ATV";
                sqlText += " ,SC";
                sqlText += " ,LessFrightToPay";
                sqlText += " ,OtherTotalAmnt";
                sqlText += " ,GrandTotal";
                sqlText += " ,RailReceiptNo";
                sqlText += " ,RailReceiptDate";
                sqlText += " ,RlyInvNo";
                sqlText += " ,WetCharge";
                sqlText += " ,ToPay";
                sqlText += " ,OtherAmnt";
                sqlText += " ,Prepaid";
                sqlText += " ,ImportId";
                sqlText += " )";

                sqlText += " VALUES";
                sqlText += " (";
                sqlText += "  @SalesInvoiceNo";
                sqlText += " ,@BranchId";
                sqlText += " ,@CustomerID";
                sqlText += " ,@DeliveryAddress";
                sqlText += " ,@InvoiceDateTime";
                sqlText += " ,@DeliveryDate";
                sqlText += " ,@TransactionType";
                sqlText += " ,@SaleType";
                sqlText += " ,@ReportType";
                sqlText += " ,@VehicleType";
                sqlText += " ,@VehicleNo";
                sqlText += " ,@TotalAmount";
                sqlText += " ,@TotalSDAmount";
                sqlText += " ,@TotalVATAmount";
                sqlText += " ,@SerialNo";
                sqlText += " ,@Comments";
                sqlText += " ,@CreatedBy";
                sqlText += " ,@CreatedOn";
                sqlText += " ,@IsPrint";
                sqlText += " ,@Post";
                sqlText += " ,@CurrencyID";
                sqlText += " ,@CurrencyRateFromBDT";
                sqlText += " ,@AlReadyPrint";
                sqlText += " ,@CustomerOrder";
                sqlText += " ,@CustomerOrderDate";
                sqlText += " ,@Tarcat";
                sqlText += " ,@SupplyVAT";
                sqlText += " ,@TC";
                sqlText += " ,@LF";
                sqlText += " ,@RF";
                sqlText += " ,@ShortExcessAmnt";
                sqlText += " ,@Toll";
                sqlText += " ,@DC";
                sqlText += " ,@ATV";
                sqlText += " ,@SC";
                sqlText += " ,@LessFrightToPay";
                sqlText += " ,@OtherTotalAmnt";
                sqlText += " ,@GrandTotal";
                sqlText += " ,@RailReceiptNo";
                sqlText += " ,@RailReceiptDate";
                sqlText += " ,@RlyInvNo";
                sqlText += " ,@WetCharge";
                sqlText += " ,@ToPay";
                sqlText += " ,@OtherAmnt";
                sqlText += " ,@Prepaid";
                sqlText += " ,@ImportId";

                sqlText += ")  SELECT SCOPE_IDENTITY() ";

                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);

                cmdInsert.Parameters.AddWithValueAndNullHandle("@SalesInvoiceNo", Master.SalesInvoiceNo);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@BranchId", Master.BranchId);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@CustomerID", Master.CustomerID);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@DeliveryAddress", Master.DeliveryAddress);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@InvoiceDateTime", Master.InvoiceDateTime);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@DeliveryDate", Master.DeliveryDate);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@TransactionType", Master.TransactionType);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@SaleType", Master.SaleType);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@ReportType", Master.ReportType);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@VehicleType", Master.VehicleType);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@VehicleNo", Master.VehicleNo);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@TotalAmount", Master.TotalAmount);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@TotalSDAmount", Master.TotalSDAmount);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@TotalVATAmount", Master.TotalVATAmount);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@SerialNo", Master.SerialNo);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Comments", Master.Comments);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@CreatedBy", Master.CreatedBy);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@CreatedOn", Master.CreatedOn);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@IsPrint", "N");
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Post", "N");
                cmdInsert.Parameters.AddWithValueAndNullHandle("@CurrencyID", Master.CurrencyID);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@CurrencyRateFromBDT", Master.CurrencyRateFromBDT);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@AlReadyPrint", "0");
                cmdInsert.Parameters.AddWithValueAndNullHandle("@CustomerOrder", Master.CustomerOrder);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@CustomerOrderDate", Master.CustomerOrderDate);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Tarcat", Master.Tarcat);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@SupplyVAT", Master.SupplyVAT);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@TC", Master.TC);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@LF", Master.LF);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@RF", Master.RF);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@ShortExcessAmnt", Master.ShortExcessAmnt);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Toll", Master.Toll);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@DC", Master.DC);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@SC", Master.SC);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@ATV", Master.ATV);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@LessFrightToPay", Master.LessFrightToPay);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@OtherTotalAmnt", Master.OtherTotalAmnt);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@GrandTotal", Master.GrandTotal);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@RailReceiptNo", Master.RailReceiptNo);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@RailReceiptDate", Master.RailReceiptDate);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@RlyInvNo", Master.RlyInvNo);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@WetCharge", Master.WetCharge);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@ToPay", Master.ToPay);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@OtherAmnt", Master.OtherAmnt);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Prepaid", Master.Prepaid);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@ImportId", Master.ImportId);


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
                retResults[2] = "" + Master.SalesInvoiceNo;
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

        public string[] SalesMPLInsertToDetails(SalesInvoiceMPLDetailVM details, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null, DataTable settingsDt = null)
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

                #region Entry Date Check

                CommonDAL commonDal = new CommonDAL();


                #endregion

                #region Insert

                sqlText = "";
                sqlText += " INSERT INTO SalesInvoiceMPLDetails";
                sqlText += " (";
                sqlText += "SalesInvoiceMPLHeaderId ";
                sqlText += ",BranchId ";
                sqlText += ",InvoiceDateTime ";
                sqlText += ",ItemNo ";
                sqlText += ",InputQuantity ";
                sqlText += ",Quantity ";
                sqlText += ",NBRPrice ";
                sqlText += ",UnitPriceWithVAT ";
                sqlText += ",UOM ";
                sqlText += ",SDRate ";
                sqlText += ",SDAmount ";
                sqlText += ",VATRate ";
                sqlText += ",VATAmount ";
                sqlText += ",SubTotal ";
                sqlText += ",LineTotal ";
                sqlText += ",LineComments ";
                sqlText += ",SaleType ";
                sqlText += ",VATType ";
                sqlText += ",Post ";
                sqlText += ",UOMQty ";
                sqlText += ",UOMPrice ";
                sqlText += ",UOMc ";
                sqlText += ",UOMn ";
                sqlText += ",DollerValue ";
                sqlText += ",CurrencyValue ";
                sqlText += ",CConversionDate ";
                sqlText += ",TransactionType ";
                sqlText += ",VATName ";
                sqlText += ",TankId ";
                sqlText += ",IsPackCal ";

                sqlText += " )";

                sqlText += " VALUES";
                sqlText += " (";
                sqlText += " @SalesInvoiceMPLHeaderId";
                sqlText += " ,@BranchId";
                sqlText += " ,@InvoiceDateTime";
                sqlText += " ,@ItemNo";
                sqlText += " ,@InputQuantity";
                sqlText += " ,@Quantity";
                sqlText += " ,@NBRPrice";
                sqlText += " ,@UnitPriceWithVAT";
                sqlText += " ,@UOM";
                sqlText += " ,@SDRate";
                sqlText += " ,@SDAmount";
                sqlText += " ,@VATRate";
                sqlText += " ,@VATAmount";
                sqlText += " ,@SubTotal";
                sqlText += " ,@LineTotal";
                sqlText += " ,@LineComments";
                sqlText += " ,@SaleType";
                sqlText += " ,@VATType";
                sqlText += " ,@Post";
                sqlText += " ,@UOMQty";
                sqlText += " ,@UOMPrice";
                sqlText += " ,@UOMc";
                sqlText += " ,@UOMn";
                sqlText += " ,@DollerValue";
                sqlText += " ,@CurrencyValue";
                sqlText += " ,@CConversionDate";
                sqlText += " ,@TransactionType";
                sqlText += " ,@VATName";
                sqlText += " ,@TankId";
                sqlText += " ,@IsPackCal";


                sqlText += ")  SELECT SCOPE_IDENTITY() ";

                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);

                cmdInsert.Parameters.AddWithValueAndNullHandle("@SalesInvoiceMPLHeaderId", details.SalesInvoiceMPLHeaderId);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@BranchId", details.BranchId);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@InvoiceDateTime", details.InvoiceDateTime);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@ItemNo", details.ItemNo);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@InputQuantity", details.InputQuantity);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Quantity", details.Quantity);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@NBRPrice", details.NBRPrice);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@UnitPriceWithVAT", details.UnitPriceWithVAT);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@UOM", details.UOM);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@SDRate", details.SDRate);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@SDAmount", details.SDAmount);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@VATRate", details.VATRate);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@VATAmount", details.VATAmount);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@SubTotal", details.SubTotal);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@LineTotal", details.LineTotal);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@LineComments", details.LineComments);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@SaleType", details.SaleType);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@VATType", details.VATType);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Post", details.Post);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@UOMQty", details.UOMQty);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@UOMPrice", details.UOMPrice);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@UOMc", details.UOMc);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@UOMn", details.UOMn);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@DollerValue", details.DollerValue);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@CurrencyValue", details.CurrencyValue);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@CConversionDate", details.CConversionDate);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@TransactionType", details.TransactionType);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@VATName", details.VATName);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@TankId", details.TankId);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@IsPackCal", details.IsPackCal);

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
                retResults[2] = "" + details.SalesInvoiceMPLHeaderId;
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

        public string[] SalesMPLInsertToBankPayment(SalesInvoiceMPLBankPaymentVM details, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null, DataTable settingsDt = null)
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

                #region Entry Date Check

                CommonDAL commonDal = new CommonDAL();


                #endregion

                #region Insert

                sqlText = "";
                sqlText += " INSERT INTO SalesInvoiceMPLBankPayments";
                sqlText += " (";
                sqlText += "SalesInvoiceMPLHeaderId ";
                sqlText += ",BankPaymentReceiveId ";
                sqlText += ",BranchId ";
                sqlText += ",BankId ";
                sqlText += ",ModeOfPayment ";
                sqlText += ",InstrumentNo ";
                sqlText += ",InstrumentDate ";
                sqlText += ",Amount ";
                sqlText += ",IsUsedDS ";
                sqlText += " )";

                sqlText += " VALUES";
                sqlText += " (";
                sqlText += " @SalesInvoiceMPLHeaderId";
                sqlText += " ,@BankPaymentReceiveId";
                sqlText += " ,@BranchId";
                sqlText += " ,@BankId";
                sqlText += " ,@ModeOfPayment";
                sqlText += " ,@InstrumentNo";
                sqlText += " ,@InstrumentDate";
                sqlText += " ,@Amount";
                sqlText += " ,@IsUsedDS";

                sqlText += ")  SELECT SCOPE_IDENTITY() ";

                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);

                cmdInsert.Parameters.AddWithValueAndNullHandle("@SalesInvoiceMPLHeaderId", details.SalesInvoiceMPLHeaderId);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@BankPaymentReceiveId", details.BankPaymentReceiveId);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@BranchId", details.BranchId);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@BankId", details.BankId);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@ModeOfPayment", details.ModeOfPayment);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@InstrumentNo", details.InstrumentNo);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@InstrumentDate", details.InstrumentDate);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Amount", details.Amount);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@IsUsedDS", details.IsUsedDS);

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
                retResults[2] = "" + details.SalesInvoiceMPLHeaderId;
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

        public string[] SalesMPLInsertToCreditInfo(SalesInvoiceMPLCRInfoVM details, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null, DataTable settingsDt = null)
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

                #region Entry Date Check

                CommonDAL commonDal = new CommonDAL();


                #endregion



                #region Insert

                sqlText = "";
                sqlText += " INSERT INTO SalesInvoiceMPLCRInfos";
                sqlText += " (";
                sqlText += "CRCode ";
                sqlText += ",BranchId ";
                sqlText += ",CustomerId ";
                sqlText += ",SalesInvoiceMPLHeaderId ";
                sqlText += ",CRDate ";
                sqlText += ",Amount ";
                sqlText += ",SalesInvoiceRefId ";
                sqlText += ",IsUsed ";

                sqlText += " )";

                sqlText += " VALUES";
                sqlText += " (";
                sqlText += " @CRCode";
                sqlText += " ,@BranchId";
                sqlText += " ,@CustomerId";
                sqlText += " ,@SalesInvoiceMPLHeaderId";
                sqlText += " ,@CRDate";
                sqlText += " ,@Amount";
                sqlText += " ,@SalesInvoiceRefId";
                sqlText += " ,@IsUsed";


                sqlText += ")  SELECT SCOPE_IDENTITY() ";

                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);

                cmdInsert.Parameters.AddWithValueAndNullHandle("@CRCode", details.CRCode);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@BranchId", details.BranchId);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@CustomerId", details.CustomerId);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@SalesInvoiceMPLHeaderId", details.SalesInvoiceMPLHeaderId);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@CRDate", details.CRDate);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Amount", details.Amount);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@SalesInvoiceRefId", details.SalesInvoiceRefId);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@IsUsed", details.IsUsed);

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
                retResults[2] = "" + details.SalesInvoiceMPLHeaderId;
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

        public string[] SalesMPLUpdate(SalesInvoiceMPLHeaderVM MasterVM, SqlTransaction transaction, SqlConnection currConn, SysDBInfoVMTemp connVM = null)
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

            decimal TotalAmount = 0;
            decimal TotalPaymentAmount = 0;
            decimal TotalCRAmount = 0;
            decimal TotalNewCRAmount = 0;

            SaleMasterVM SaleMaster = new SaleMasterVM();
            List<SaleDetailVm> Details = new List<SaleDetailVm>();
            #endregion Initializ

            #region Try
            try
            {

                #region Validation for Header

                if (MasterVM == null)
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgNoDataToSave);
                }
                else if (Convert.ToDateTime(MasterVM.InvoiceDateTime) < DateTime.MinValue || Convert.ToDateTime(MasterVM.InvoiceDateTime) > DateTime.MaxValue)
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
                MasterVM.ShortExcessAmnt = Convert.ToDecimal(commonDal.decimal259(MasterVM.ShortExcessAmnt));
                MasterVM.OtherTotalAmnt = Convert.ToDecimal(commonDal.decimal259(MasterVM.OtherTotalAmnt));
                MasterVM.CurrencyRateFromBDT = Convert.ToDecimal(commonDal.decimal259(MasterVM.CurrencyRateFromBDT));

                MasterVM.InvoiceDateTime = OrdinaryVATDesktop.DateToDate(MasterVM.InvoiceDateTime);
                MasterVM.DeliveryDate = MasterVM.InvoiceDateTime;
                MasterVM.CustomerOrderDate = OrdinaryVATDesktop.DateToDate(MasterVM.CustomerOrderDate);
                MasterVM.RailReceiptDate = OrdinaryVATDesktop.DateTimeToDate(MasterVM.RailReceiptDate);
                retResults = SalesMPLUpdateToMaster(MasterVM, currConn, transaction, null, settings);

                Id = retResults[2];

                if (retResults[0] != "Success")
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, retResults[1]);
                }

                #endregion Update Information in Header

                #region Delete Existing Detail Data

                sqlText = "";
                sqlText += @" DELETE FROM SalesInvoiceMPLDetails WHERE SalesInvoiceMPLHeaderId=@MasterSalesInvoiceId ";
                sqlText += @" DELETE FROM SalesInvoiceMPLBankPayments WHERE SalesInvoiceMPLHeaderId=@MasterSalesInvoiceId ";
                sqlText += @" DELETE FROM SalesInvoiceMPLCRInfos WHERE SalesInvoiceMPLHeaderId=@MasterSalesInvoiceId ";
                sqlText += @" Update  SalesInvoiceMPLCRInfos set SalesInvoiceRefId=0 ,IsUsed=0 WHERE SalesInvoiceRefId=@MasterSalesInvoiceId ";
                sqlText += @" Update  MPLBankPaymentReceiveDetails set SalesInvoiceRefId=0 ,IsUsed=0 WHERE SalesInvoiceRefId=@MasterSalesInvoiceId ";

                SqlCommand cmdDeleteDetail = new SqlCommand(sqlText, currConn, transaction);
                cmdDeleteDetail.Parameters.AddWithValueAndNullHandle("@MasterSalesInvoiceId", MasterVM.Id);

                transResult = cmdDeleteDetail.ExecuteNonQuery();

                #endregion

                #region Insert into Details(Insert complete in Header)

                if (MasterVM.SalesInvoiceMPLDetailVMs != null)
                {
                    if (MasterVM.SalesInvoiceMPLDetailVMs.Count() < 0)
                    {
                        throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgNoDataToSave);
                    }
                    int Record=0;
                    foreach (SalesInvoiceMPLDetailVM collection in MasterVM.SalesInvoiceMPLDetailVMs)
                    {

                        ProductDAL productDal = new ProductDAL();

                        var product = productDal.SelectAll("0", new[] { "pr.ItemNo" }, new[] { collection.ItemNo }, currConn, transaction,
                      null, connVM, null).FirstOrDefault();


                        #region SalesDeatils
                        SaleDetailVm SaleDetails = new SaleDetailVm();
                        SaleDetails.InvoiceLineNo = Record.ToString();
                        if (product.IsFixedVAT == "Y")
                        {
                            SaleDetails.Type = "FixedVAT";
                            collection.VATType = "FixedVAT";

                        }
                        else
                        {
                            SaleDetails.Type = "VAT";
                            collection.VATType = "VAT";

                        }

                        SaleDetails.SalesInvoiceNo = MasterVM.SalesInvoiceNo;
                        SaleDetails.ItemNo = collection.ItemNo;
                        SaleDetails.SaleTypeD = "New";
                        collection.VATName = "VAT 4.3";

                        SaleDetails.Quantity = Convert.ToDecimal((collection.Quantity));
                        SaleDetails.PromotionalQuantity = 0;
                        if (collection.IsPackCal == "Y")
                        {
                            SaleDetails.SalesPrice = Convert.ToDecimal((collection.NBRPrice) / (collection.Quantity));
                            SaleDetails.NBRPrice = SaleDetails.SalesPrice;
                            SaleDetails.UOMPrice = SaleDetails.SalesPrice;
                        }
                        else
                        {
                            SaleDetails.SalesPrice = Convert.ToDecimal((collection.NBRPrice));
                            SaleDetails.NBRPrice = Convert.ToDecimal((collection.NBRPrice));
                            SaleDetails.UOMPrice = SaleDetails.SalesPrice;

                        }

                        SaleDetails.AvgRate = 0;
                        SaleDetails.VATRate = collection.VATRate;
                        SaleDetails.SubTotal = collection.SubTotal;
                        SaleDetails.VATAmount = collection.VATAmount;
                        SaleDetails.CreatedBy = MasterVM.CreatedBy;
                        SaleDetails.CreatedOn = MasterVM.CreatedOn;
                        SaleDetails.SD = 0;
                        SaleDetails.SDAmount = 0;
                        SaleDetails.VDSAmountD = 0;
                        SaleDetails.InvoiceDateTime = MasterVM.InvoiceDateTime;
                        SaleDetails.TransactionType = MasterVM.TransactionType;
                        SaleDetails.ReturnId = "0";
                        SaleDetails.Post = "N";
                        SaleDetails.TradingD = "N";
                        SaleDetails.NonStockD = "N";
                        SaleDetails.UOM = collection.UOMn;
                        SaleDetails.UOMn = collection.UOMn;

                        SaleDetails.UOMQty = Convert.ToDecimal((collection.Quantity));
                        SaleDetails.UOMc = 1;
                        SaleDetails.DiscountAmount = 0;
                        SaleDetails.DiscountedNBRPrice = 0;
                        SaleDetails.DollerValue = 0;
                        SaleDetails.CurrencyValue = 0;

                        SaleDetails.BranchId = MasterVM.BranchId;
                        SaleDetails.PreviousInvoiceDateTime = "1990-01-01 00:00:00.000";
                        Details.Add(SaleDetails);


                        #endregion



                        collection.UOMPrice = Convert.ToDecimal((collection.NBRPrice));
                        collection.UOMc = 1;
                        collection.UOMQty = collection.Quantity;
                        collection.SalesInvoiceMPLHeaderId = Convert.ToInt32(Id);
                        collection.BranchId = MasterVM.BranchId;
                        collection.InvoiceDateTime = MasterVM.InvoiceDateTime;
                        collection.Post = "N";
                        collection.SaleType = "New";
                        collection.VATName = "VAT 4.3";




                        TotalAmount += collection.LineTotal;

                        retResults = SalesMPLInsertToDetails(collection, currConn, transaction, null, settings);
                    }
                }

                if (retResults[0] != "Success")
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, retResults[1]);
                }

                #region Update sale Headers

                sqlText = "";
                sqlText += @"

update SalesInvoiceMPLHeaders set TotalAmount=d.LineTotal,TotalSDAmount=d.SDAmount,TotalVATAmount=d.VATAmount
from (select distinct   SalesInvoiceMPLHeaderId, sum(LineTotal)LineTotal,sum(VATAmount)VATAmount,sum(SDAmount)SDAmount from SalesInvoiceMPLDetails
group by SalesInvoiceMPLHeaderId) d
where SalesInvoiceMPLHeaders.Id=d.SalesInvoiceMPLHeaderId
and SalesInvoiceMPLHeaders.Id = @Id



";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                cmdUpdate.CommandTimeout = 500;
                cmdUpdate.Parameters.AddWithValue("@Id", MasterVM.Id);

                transResult = cmdUpdate.ExecuteNonQuery();

                #endregion

                #endregion Insert into Details(Insert complete in Header)

                #region Insert into BankPayment

                if (MasterVM.SalesInvoiceMPLBankPaymentVMs != null)
                {
                    if (MasterVM.SalesInvoiceMPLBankPaymentVMs.Count() < 0)
                    {
                        throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgNoDataToSave);
                    }

                    foreach (SalesInvoiceMPLBankPaymentVM collection in MasterVM.SalesInvoiceMPLBankPaymentVMs)
                    {
                        TotalPaymentAmount += collection.Amount;
                        collection.SalesInvoiceMPLHeaderId = MasterVM.Id;
                        collection.BranchId = MasterVM.BranchId;
                        retResults = SalesMPLInsertToBankPayment(collection, currConn, transaction, null, settings);

                        if (retResults[0] == "Success")
                        {
                            MPLBankPaymentReceiveVM vm = new MPLBankPaymentReceiveVM();
                            vm.Id = collection.BankPaymentReceiveId;
                            vm.SalesInvoiceRefId = Id;

                            retResults = UpdateBankPaymentReceiveInfo(vm, transaction, currConn, connVM);
                        }
                    }
                }

                if (retResults[0] != "Success")
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, retResults[1]);
                }

                #endregion Insert into BankPayment

                if (MasterVM.ReportType == "SA-01")
                {

                    #region Insert into Credit Info


                    if (MasterVM.SalesInvoiceMPLCRInfoVMs != null)
                    {
                        if (MasterVM.SalesInvoiceMPLCRInfoVMs.Count() < 0)
                        {
                            throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgNoDataToSave);
                        }

                        foreach (SalesInvoiceMPLCRInfoVM collection in MasterVM.SalesInvoiceMPLCRInfoVMs)
                        {
                            collection.SalesInvoiceRefId = Convert.ToInt32(MasterVM.Id);
                            collection.SalesInvoiceMPLHeaderId = collection.SalesInvoiceMPLHeaderId;
                            collection.IsUsed = true;

                            retResults = SalesMPLUpdateCreditInfo(collection, currConn, transaction, null);
                            TotalCRAmount += collection.Amount;

                        }
                    }
                    TotalNewCRAmount = (TotalPaymentAmount + TotalCRAmount) - TotalAmount;

                    if (TotalNewCRAmount != 0)
                    {
                        SalesInvoiceMPLCRInfoVM details = new SalesInvoiceMPLCRInfoVM();
                        details.Amount = TotalNewCRAmount;
                        details.BranchId = MasterVM.BranchId;
                        details.CustomerId = Convert.ToInt32(MasterVM.CustomerID);
                        details.SalesInvoiceMPLHeaderId = Convert.ToInt32(Id);
                        details.CRCode = MasterVM.NewCRCode;
                        details.CRDate = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss");
                        details.SalesInvoiceRefId = 0;
                        details.IsUsed = false;

                        retResults = SalesMPLInsertToCreditInfo(details, currConn, transaction, null, settings);

                    }

                    if (retResults[0] != "Success")
                    {
                        throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, retResults[1]);
                    }

                    #endregion Insert into Credit Info
                }
                else
                {

                    #region Insert into Credit Info


                    if (MasterVM.SalesInvoiceMPLCRInfoVMs != null)
                    {
                        if (MasterVM.SalesInvoiceMPLCRInfoVMs.Count() < 0)
                        {
                            throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgNoDataToSave);
                        }
                        decimal totalCRAmount = MasterVM.SalesInvoiceMPLCRInfoVMs.Sum(item => item.Amount);
                        decimal NewCRAmount = (TotalPaymentAmount + totalCRAmount) - TotalAmount;

                        if (NewCRAmount != 0)
                        {
                            throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert,
                                                              "Balance Not Match,Please try to Create SA-01 Invoice");
                        }
                        foreach (SalesInvoiceMPLCRInfoVM collection in MasterVM.SalesInvoiceMPLCRInfoVMs)
                        {
                            collection.SalesInvoiceRefId = Convert.ToInt32(MasterVM.Id);
                            collection.SalesInvoiceMPLHeaderId = collection.SalesInvoiceMPLHeaderId;
                            collection.IsUsed = true;

                            retResults = SalesMPLUpdateCreditInfo(collection, currConn, transaction, null);
                            TotalCRAmount += collection.Amount;

                        }
                    }
                    TotalNewCRAmount = (TotalPaymentAmount + TotalCRAmount) - (TotalAmount + MasterVM.OtherTotalAmnt);

                    if (TotalNewCRAmount != 0)
                    {
                        throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert,
                                                          "Balance Not Match,Please try to Create SA-01 Invoice");
                    }

                   

                    #endregion Insert into Credit Info

                }
                #region SaleInsert
                SaleMaster.SalesInvoiceNo = MasterVM.SalesInvoiceNo;
                SaleMaster.CustomerID = MasterVM.CustomerID;
                SaleMaster.DeliveryAddress1 = MasterVM.DeliveryAddress;
                SaleMaster.InvoiceDateTime = MasterVM.InvoiceDateTime;
                SaleMaster.DeliveryDate = MasterVM.InvoiceDateTime;
                SaleMaster.BranchId = MasterVM.BranchId;
                SaleMaster.VehicleNo = MasterVM.VehicleNo;
                SaleMaster.VehicleType = MasterVM.VehicleType;
                SaleMaster.SaleType = "New";
                SaleMaster.TransactionType = MasterVM.TransactionType;
                SaleMaster.CurrencyCode = "BDT";
                SaleMaster.CurrencyID = "260";
                SaleMaster.CurrencyRateFromBDT = 1;
                SaleMaster.Post = "N";
                SaleMaster.IsPrint = "N";
                SaleMaster.IsGatePass = "N";
                SaleMaster.IsDeemedExport = "N";
                SaleMaster.Trading = "N";
                SaleMaster.TransactionType = MasterVM.TransactionType;
                SaleMaster.CreatedBy = MasterVM.CreatedBy;
                SaleMaster.CreatedOn = MasterVM.CreatedOn;
                SaleMaster.LastModifiedBy = MasterVM.CreatedOn;
                SaleMaster.LastModifiedOn = MasterVM.CreatedOn;
                retResults = _SaleDAL.SalesUpdate(SaleMaster, Details, null, null, connVM, MasterVM.CreatedBy, true, null);
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
                retResults[2] = "" + MasterVM.SalesInvoiceNo;
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

        public string[] SalesMPLUpdateToMaster(SalesInvoiceMPLHeaderVM Master, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null, DataTable settingsDt = null)
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
                sqlText += " update SalesInvoiceMPLHeaders SET  ";

                sqlText += " SalesInvoiceNo=@SalesInvoiceNo";
                sqlText += " ,CustomerID=@CustomerID";
                sqlText += " ,DeliveryAddress=@DeliveryAddress";
                sqlText += " ,InvoiceDateTime=@InvoiceDateTime";
                sqlText += " ,DeliveryDate=@DeliveryDate";
                sqlText += " ,TransactionType=@TransactionType";
                sqlText += " ,ReportType=@ReportType";
                sqlText += " ,VehicleType=@VehicleType";
                sqlText += " ,VehicleNo=@VehicleNo";
                sqlText += " ,TotalAmount=@TotalAmount";
                sqlText += " ,TotalSDAmount=@TotalSDAmount";
                sqlText += " ,TotalVATAmount=@TotalVATAmount";
                sqlText += " ,SerialNo=@SerialNo";
                sqlText += " ,Comments=@Comments";
                sqlText += " ,LastModifiedBy=@LastModifiedBy";
                sqlText += " ,LastModifiedOn=@LastModifiedOn";
                sqlText += " ,CurrencyID=@CurrencyID";
                sqlText += " ,CurrencyRateFromBDT=@CurrencyRateFromBDT";
                sqlText += " ,CustomerOrder=@CustomerOrder";
                sqlText += " ,CustomerOrderDate=@CustomerOrderDate";
                sqlText += " ,Tarcat=@Tarcat";
                sqlText += " ,SupplyVAT=@SupplyVAT";
                sqlText += " ,TC=@TC";
                sqlText += " ,LF=@LF";
                sqlText += " ,RF=@RF";
                sqlText += " ,ShortExcessAmnt=@ShortExcessAmnt";
                sqlText += " ,Toll=@Toll";
                sqlText += " ,DC=@DC";
                sqlText += " ,SC=@SC";
                sqlText += " ,ATV=@ATV";
                sqlText += " ,LessFrightToPay=@LessFrightToPay";
                sqlText += " ,OtherTotalAmnt=@OtherTotalAmnt";
                sqlText += " ,GrandTotal=@GrandTotal";
                sqlText += " ,RailReceiptNo=@RailReceiptNo";
                sqlText += " ,RailReceiptDate=@RailReceiptDate";
                sqlText += " ,RlyInvNo=@RlyInvNo";
                sqlText += " ,WetCharge=@WetCharge";
                sqlText += " ,ToPay=@ToPay";
                sqlText += " ,OtherAmnt=@OtherAmnt";
                sqlText += " ,Prepaid=@Prepaid";

                sqlText += " WHERE Id=@Id";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);

                cmdUpdate.Parameters.AddWithValueAndNullHandle("@SalesInvoiceNo", Master.SalesInvoiceNo);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@CustomerID", Master.CustomerID);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@DeliveryAddress", Master.DeliveryAddress);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@InvoiceDateTime", Master.InvoiceDateTime);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@DeliveryDate", Master.DeliveryDate);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@TransactionType", Master.TransactionType);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@SaleType", Master.SaleType);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@ReportType", Master.ReportType);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@VehicleType", Master.VehicleType);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@VehicleNo", Master.VehicleNo);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@TotalAmount", Master.TotalAmount);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@TotalSDAmount", Master.TotalSDAmount);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@TotalVATAmount", Master.TotalVATAmount);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@SerialNo", Master.SerialNo);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@Comments", Master.Comments);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@LastModifiedBy", Master.LastModifiedBy);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@LastModifiedOn", Master.LastModifiedOn);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@CurrencyID", Master.CurrencyID);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@CurrencyRateFromBDT", Master.CurrencyRateFromBDT);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@CustomerOrder", Master.CustomerOrder);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@CustomerOrderDate", Master.CustomerOrderDate);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@Tarcat", Master.Tarcat);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@SupplyVAT", Master.SupplyVAT);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@TC", Master.TC);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@LF", Master.LF);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@RF", Master.RF);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@ShortExcessAmnt", Master.ShortExcessAmnt);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@Toll", Master.Toll);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@DC", Master.DC);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@SC", Master.SC);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@ATV", Master.ATV);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@LessFrightToPay", Master.LessFrightToPay);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@OtherTotalAmnt", Master.OtherTotalAmnt);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@GrandTotal", Master.GrandTotal);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@RailReceiptNo", Master.RailReceiptNo);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@RailReceiptDate", Master.RailReceiptDate);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@RlyInvNo", Master.RlyInvNo);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@WetCharge", Master.WetCharge);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@ToPay", Master.ToPay);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@OtherAmnt", Master.OtherAmnt);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@Prepaid", Master.Prepaid);

                cmdUpdate.Parameters.AddWithValueAndNullHandle("@Id", Master.Id);

                transResult = cmdUpdate.ExecuteNonQuery();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.PurchasemsgSaveNotSuccessfully);
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


        public string[] SalesMPLUpdateCreditInfo(SalesInvoiceMPLCRInfoVM details, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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

                #region Entry Date Check

                CommonDAL commonDal = new CommonDAL();


                #endregion

                #region Insert

                sqlText += " update SalesInvoiceMPLCRInfos SET  ";
                sqlText += " SalesInvoiceRefId=@SalesInvoiceRefId";
                sqlText += " ,IsUsed=@IsUsed";
                sqlText += " WHERE SalesInvoiceMPLHeaderId=@SalesInvoiceMPLHeaderId";
                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);

                cmdUpdate.Parameters.AddWithValueAndNullHandle("@SalesInvoiceRefId", details.SalesInvoiceRefId);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@SalesInvoiceMPLHeaderId", details.SalesInvoiceMPLHeaderId);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@IsUsed", true);
 

                 transResult = cmdUpdate.ExecuteNonQuery();
                 if (transResult <= 0)
                 {
                     throw new ArgumentNullException(MessageVM.tpMsgUpdateNotSuccessfully, MessageVM.saleMsgUpdateNotSuccessfully);
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
                retResults[1] = MessageVM.saleMsgUpdateSuccessfully;
                retResults[2] = "" + details.SalesInvoiceMPLHeaderId;
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
        public DataTable SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SalesInvoiceMPLHeaderVM likeVM = null, bool Dt = false, string transactionType = null, string MultipleSearch = "N", SysDBInfoVMTemp connVM = null, string Orderby = "Y", string[] ids = null, string Is6_3TollCompleted = null, string IsVDSCompleted = null)
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
                }

                sqlText += @"

sih.Id,
sih.SalesInvoiceNo,
sih.BranchId,
sih.CustomerID,
sih.DeliveryAddress,
ISNULL(sih.InvoiceDateTime,'')InvoiceDateTime,
ISNULL(sih.DeliveryDate,'')DeliveryDate,
sih.TransactionType,
sih.SaleType,
sih.ReportType,
sih.VehicleType,
sih.VehicleNo,
ISNULL(sih.TotalAmount,0)TotalAmount,
ISNULL(sih.TotalSDAmount,0)TotalSDAmount,
ISNULL(sih.TotalVATAmount,0)TotalVATAmount,
sih.SerialNo,
sih.Comments,
sih.IsPrint,
sih.Post,
sih.CurrencyID,
ISNULL(sih.CurrencyRateFromBDT,0)CurrencyRateFromBDT,
sih.AlReadyPrint,
sih.CustomerOrder,
ISNULL(sih.CustomerOrderDate,'')CustomerOrderDate,
sih.Tarcat,
sih.TC,
sih.LF,
sih.RF,
sih.SC,
sih.ATV,
ISNULL(sih.ShortExcessAmnt,0)ShortExcessAmnt,
sih.Toll,
sih.DC,
sih.LessFrightToPay,
ISNULL(sih.OtherTotalAmnt,0)OtherTotalAmnt,
ISNULL(sih.GrandTotal,0)GrandTotal,
ISNULL(sih.OtherAmnt,0)OtherAmnt,
ISNULL(sih.SupplyVAT,0)SupplyVAT,
sih.RailReceiptNo,
ISNULL(sih.RailReceiptDate,'')RailReceiptDate,
sih.RlyInvNo,
sih.WetCharge,
sih.ToPay,
sih.Prepaid

--,cg.CustomerGroupName
,c.CustomerName
--,c.CustomerCode
--,cr.CurrencyCode

FROM SalesInvoiceMPLHeaders sih  

left outer join Customers c on sih.CustomerID=c.CustomerID 
left outer join Vehicles v on sih.VehicleNo = v.VehicleID
left outer join Currencies cr on sih.CurrencyID=cr.CurrencyId
left outer join CustomerGroups cg on c.CustomerGroupID=cg.CustomerGroupID
WHERE  1=1 
";
                #endregion SqlText

                sqlTextCount += @" 
SELECT count(sih.SalesInvoiceNo) RecordCount
FROM SalesInvoiceMPLHeaders sih 

left outer join Customers c on sih.CustomerID=c.CustomerID 
left outer join Vehicles v on sih.VehicleNo = v.VehicleID
left outer join Currencies cr on sih.CurrencyID=cr.CurrencyId
left outer join CustomerGroups cg on c.CustomerGroupID=cg.CustomerGroupID
WHERE  1=1 ";

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
                    sqlText += " AND sih.SalesInvoiceNo IN(" + sqlText2 + ")";
                }

                if (Id > 0)
                {
                    sqlTextParameter += @" and sih.Id=@Id";
                }
                if (!String.IsNullOrEmpty(transactionType))
                {
                    sqlTextParameter += @" and sih.TransactionType in(@transactionType)";
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
                    sqlTextOrderBy += " order by sih.SalesInvoiceNo desc, sih.InvoiceDateTime desc";

                }
                else
                {
                    sqlTextOrderBy += " order by sih.InvoiceDateTime desc, sih.SalesInvoiceNo desc";
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

        public List<SalesInvoiceMPLHeaderVM> SelectAllList(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SalesInvoiceMPLHeaderVM likeVM = null, SysDBInfoVMTemp connVM = null, string transactionType = null, string Orderby = "Y", string[] ids = null, string Is6_3TollCompleted = null)
        {
            #region Variables

            string sqlText = "";
            List<SalesInvoiceMPLHeaderVM> VMs = new List<SalesInvoiceMPLHeaderVM>();
            SalesInvoiceMPLHeaderVM vm;
            #endregion

            #region try

            try
            {
                #region sql statement

                #region SqlExecution

                DataTable dt = SelectAll(Id, conditionFields, conditionValues, VcurrConn, Vtransaction, null, false, transactionType, "N", connVM, Orderby, ids, Is6_3TollCompleted);
                //dt.Rows.RemoveAt(dt.Rows.Count - 1);
                foreach (DataRow dr in dt.Rows)
                {
                    try
                    {
                        vm = new SalesInvoiceMPLHeaderVM();
                        vm.Id = Convert.ToInt32(dr["Id"].ToString());
                        vm.SalesInvoiceNo = dr["SalesInvoiceNo"].ToString();
                        vm.BranchId = Convert.ToInt32(dr["BranchId"]);
                        vm.CustomerID = dr["CustomerID"].ToString();
                        vm.DeliveryAddress = dr["DeliveryAddress"].ToString();
                        vm.InvoiceDateTime = OrdinaryVATDesktop.DateTimeToDate(dr["InvoiceDateTime"].ToString());
                        vm.DeliveryDate = OrdinaryVATDesktop.DateTimeToDate(dr["DeliveryDate"].ToString());
                        vm.TransactionType = dr["TransactionType"].ToString();
                        vm.SaleType = dr["SaleType"].ToString();
                        vm.ReportType = dr["ReportType"].ToString();
                        vm.VehicleType = dr["VehicleType"].ToString();
                        vm.VehicleNo = dr["VehicleNo"].ToString();
                        vm.TotalAmount = Convert.ToDecimal(dr["TotalAmount"].ToString());
                        vm.TotalSDAmount = Convert.ToDecimal(dr["TotalSDAmount"].ToString());
                        vm.TotalVATAmount = Convert.ToDecimal(dr["TotalVATAmount"].ToString());
                        vm.SerialNo = dr["SerialNo"].ToString();
                        vm.Comments = dr["Comments"].ToString();
                        vm.IsPrint = dr["IsPrint"].ToString();
                        vm.Post = dr["Post"].ToString();
                        vm.CurrencyID = dr["CurrencyID"].ToString();
                        vm.CurrencyRateFromBDT = Convert.ToDecimal(dr["CurrencyRateFromBDT"].ToString());
                        vm.AlReadyPrint = dr["AlReadyPrint"].ToString();
                        vm.CustomerOrder = dr["CustomerOrder"].ToString();
                        vm.CustomerOrderDate = OrdinaryVATDesktop.DateTimeToDate(dr["CustomerOrderDate"].ToString());
                        vm.Tarcat = dr["Tarcat"].ToString();
                        vm.TC = Convert.ToDecimal(dr["TC"].ToString());
                        vm.LF = Convert.ToDecimal(dr["LF"].ToString());
                        vm.RF = Convert.ToDecimal(dr["RF"].ToString());
                        vm.SC = Convert.ToDecimal(dr["SC"].ToString());
                        vm.ATV = Convert.ToDecimal(dr["ATV"].ToString());
                        vm.ShortExcessAmnt = Convert.ToDecimal(dr["ShortExcessAmnt"].ToString());
                        vm.Toll = Convert.ToDecimal(dr["Toll"].ToString());
                        vm.DC = Convert.ToDecimal(dr["DC"].ToString());
                        vm.LessFrightToPay = Convert.ToDecimal(dr["LessFrightToPay"].ToString());
                        vm.OtherTotalAmnt = Convert.ToDecimal(dr["OtherTotalAmnt"].ToString());
                        vm.GrandTotal = Convert.ToDecimal(dr["GrandTotal"].ToString());
                        vm.RailReceiptNo = dr["RailReceiptNo"].ToString();
                        vm.RailReceiptDate = OrdinaryVATDesktop.DateTimeToDate(dr["RailReceiptDate"].ToString());
                        vm.RlyInvNo = dr["RlyInvNo"].ToString();
                        vm.WetCharge = Convert.ToDecimal(dr["WetCharge"].ToString());
                        vm.ToPay = Convert.ToDecimal(dr["ToPay"].ToString());
                        vm.SupplyVAT = Convert.ToDecimal(dr["SupplyVAT"].ToString());
                        vm.OtherAmnt = Convert.ToDecimal(dr["OtherAmnt"].ToString());
                        vm.Prepaid = dr["Prepaid"].ToString();

                        //vm.CustomerGroupName = dr["CustomerGroupName"].ToString();
                        vm.CustomerName = dr["CustomerName"].ToString();
                        //vm.CustomerCode = dr["CustomerCode"].ToString();
                        //vm.CurrencyCode = dr["CurrencyCode"].ToString();

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


        public List<SalesInvoiceMPLDetailVM> SearchSaleMPLDetailList(string salesInvoiceMPLHeaderId, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            List<SalesInvoiceMPLDetailVM> lst = new List<SalesInvoiceMPLDetailVM>();
            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            DataTable dataTable = new DataTable("SalesInvoiceMPLDetail");

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
SELECT p.ProductName,p.ProductCode,ISNULL(t.TankCode,'') TankCode, p.IsFixedVAT ,SID.* FROM  SalesInvoiceMPLDetails SID
left outer join Products p on p.ItemNo= SID.ItemNo
left outer join TankMPLs t on t.Id= SID.TankId
WHERE 1=1  ";

                if (!string.IsNullOrEmpty(salesInvoiceMPLHeaderId))
                {
                    if (Convert.ToInt32(salesInvoiceMPLHeaderId) > 0)
                    {
                        sqlText += @" AND SID.SalesInvoiceMPLHeaderId=@SalesInvoiceMPLHeaderId";
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

                if (!string.IsNullOrEmpty(salesInvoiceMPLHeaderId))
                {
                    objCommSaleDetail.Parameters.AddWithValue("@SalesInvoiceMPLHeaderId", salesInvoiceMPLHeaderId);
                }

                #endregion Parameter

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommSaleDetail);
                dataAdapter.Fill(dataTable);

                lst = dataTable.ToList<SalesInvoiceMPLDetailVM>();
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


        public List<SalesInvoiceMPLBankPaymentVM> SearchSaleMPLBankPaymentList(string salesInvoiceMPLHeaderId, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            List<SalesInvoiceMPLBankPaymentVM> lst = new List<SalesInvoiceMPLBankPaymentVM>();
            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            DataTable dataTable = new DataTable("SalesInvoiceMPLBankPayment");

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
SELECT B.BankName
,b.BankCode
 ,SIB.Id 
 ,SIB.SalesInvoiceMPLHeaderId 
 ,Isnull(SIB.BankPaymentReceiveId,0)BankPaymentReceiveId
 ,SIB.BranchId 
 ,SIB.BankId 

 ,SIB.ModeOfPayment 
 ,SIB.InstrumentNo 
 ,SIB.InstrumentDate 
 ,SIB.Amount 

 ,isnull (SIB.IsUsedDS,0)IsUsedDS
FROM  SalesInvoiceMPLBankPayments SIB
left outer join MPLBDBankInformations b on b.BankID= SIB.BankID
WHERE 1=1
  ";

                if (!string.IsNullOrEmpty(salesInvoiceMPLHeaderId))
                {
                    if (Convert.ToInt32(salesInvoiceMPLHeaderId) > 0)
                    {
                        sqlText += @" AND SIB.SalesInvoiceMPLHeaderId=@SalesInvoiceMPLHeaderId";
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
                if (!string.IsNullOrEmpty(salesInvoiceMPLHeaderId))
                {
                    objCommSaleDetail.Parameters.AddWithValue("@SalesInvoiceMPLHeaderId", salesInvoiceMPLHeaderId);
                }
                #endregion Parameter

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommSaleDetail);
                dataAdapter.Fill(dataTable);

                lst = dataTable.ToList<SalesInvoiceMPLBankPaymentVM>();
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


        public List<SalesInvoiceMPLCRInfoVM> SearchSaleMPLCRInfoList(string salesInvoiceMPLHeaderId, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            List<SalesInvoiceMPLCRInfoVM> lst = new List<SalesInvoiceMPLCRInfoVM>();
            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            DataTable dataTable = new DataTable("SalesInvoiceMPLCRInfo");

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

                sqlText = @" SELECT  * FROM  SalesInvoiceMPLCRInfos SIC
left outer join SalesInvoiceMPLHeaders SIH on SIC.SalesInvoiceMPLHeaderId=SIH.Id
WHERE 1=1";

                if (!string.IsNullOrEmpty(salesInvoiceMPLHeaderId))
                {
                    if (Convert.ToInt32(salesInvoiceMPLHeaderId) > 0)
                    {
                        sqlText += @" AND SalesInvoiceRefId=@SalesInvoiceMPLHeaderId";
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
                if (!string.IsNullOrEmpty(salesInvoiceMPLHeaderId))
                {
                    objCommSaleDetail.Parameters.AddWithValue("@SalesInvoiceMPLHeaderId", salesInvoiceMPLHeaderId);
                }
                #endregion Parameter

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommSaleDetail);
                dataAdapter.Fill(dataTable);

                lst = dataTable.ToList<SalesInvoiceMPLCRInfoVM>();
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
        public List<SalesInvoiceMPLCRInfoVM> SearchSaleMPLCRInfoListById(string Id, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            List<SalesInvoiceMPLCRInfoVM> lst = new List<SalesInvoiceMPLCRInfoVM>();
            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            DataTable dataTable = new DataTable("SalesInvoiceMPLCRInfo");

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

                sqlText = @" SELECT  * FROM  SalesInvoiceMPLCRInfos SIC
left outer join SalesInvoiceMPLHeaders SIH on SIC.SalesInvoiceMPLHeaderId=SIH.Id
WHERE 1=1  ";

                if (!string.IsNullOrEmpty(Id))
                {
                    if (Convert.ToInt32(Id) > 0)
                    {
                        sqlText += @" AND SIC.SalesInvoiceMPLHeaderId=@Id";
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

                lst = dataTable.ToList<SalesInvoiceMPLCRInfoVM>();
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

        public List<SalesInvoiceMPLHeaderVM> DropDown(SysDBInfoVMTemp connVM = null)
        {
            throw new NotImplementedException();
        }

        public string[] Delete(SalesInvoiceMPLHeaderVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            throw new NotImplementedException();
        }


        public List<SalesInvoiceMPLCRInfoVM> SelectAllList(SalesInvoiceMPLCRInfoVM Vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            string sqlText = "";
            List<SalesInvoiceMPLCRInfoVM> VMs = new List<SalesInvoiceMPLCRInfoVM>();
            SalesInvoiceMPLCRInfoVM vm;
            #endregion

            #region try

            try
            {
                #region sql statement

                #region SqlExecution

                DataTable dt = SelectAll(Vm, VcurrConn, Vtransaction, connVM);
                //dt.Rows.RemoveAt(dt.Rows.Count - 1);
                foreach (DataRow dr in dt.Rows)
                {
                    try
                    {
                        vm = new SalesInvoiceMPLCRInfoVM();
                        vm.SalesInvoiceMPLHeaderId = Convert.ToInt32(dr["SalesInvoiceMPLHeaderId"].ToString());
                        vm.SalesInvoiceNo = dr["SalesInvoiceNo"].ToString();
                        vm.Used = dr["IsUsed"].ToString();
                        vm.CustomerCode = dr["CustomerCode"].ToString();
                        vm.CustomerName = dr["CustomerName"].ToString();
                        vm.CRDate = OrdinaryVATDesktop.DateTimeToDate(dr["CRDate"].ToString());
                        vm.CRCode = dr["CRCode"].ToString();
                        vm.Amount = Convert.ToDecimal(dr["Amount"].ToString());

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

        public DataTable SelectAll(SalesInvoiceMPLCRInfoVM Vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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

                #region SqlText


                sqlText = @"

SELECT 
      Sh.SalesInvoiceNo
      ,C.[CustomerCode]
      ,C.[CustomerName]
      ,CR.[SalesInvoiceMPLHeaderId]
      ,CR.[CRCode]
      ,[CRDate]
      ,[Amount]
      , case when [IsUsed] =0 then 'N' else 'Y'end IsUsed
  FROM [SalesInvoiceMPLCRInfos] CR
  left outer join SalesInvoiceMPLHeaders  Sh on sh.Id= CR.SalesInvoiceMPLHeaderId
  left outer join Customers  C on CR.CustomerId= C.CustomerID
WHERE  1=1 
And CR.IsUsed=@IsUsed
";
                #endregion SqlText
                if (Vm != null)
                {
                    if (!string.IsNullOrEmpty(Vm.SearchField))
                    {
                        if (!string.IsNullOrEmpty(Vm.SearchValue))
                        {
                            sqlText += @" 
                                   And CR.@SearchField=@SearchValue";
                        }
                    }
                    if (Vm.CustomerId != 0)
                    {
                        sqlText += @" 
                                   And CR.CustomerId=@CustomerId";
                    }
                    if (Vm.SalesInvoiceMPLHeaderId != 0)
                    {
                        sqlText += @" 
                                   And CR.SalesInvoiceMPLHeaderId!=@SalesInvoiceMPLHeaderId";
                    }
                    if (!string.IsNullOrEmpty(Vm.CRDateFrom))
                    {
                        sqlText += @" 
                                   And CR.CRDate>=@CRDateFrom";
                    }
                    if (!string.IsNullOrEmpty(Vm.CRDateTo))
                    {
                        sqlText += @" 
                                  And  CR.CRDate>=@CRDateTo";
                    }
                }
                #region Perameter

                #endregion SqlText
                #region SqlExecution


                if (!string.IsNullOrEmpty(Vm.SearchField))
                {
                    if (!string.IsNullOrEmpty(Vm.SearchValue))
                    {
                        sqlText = sqlText.Replace("@SearchField", Vm.SearchField);
                    }
                }

                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                if (Vm != null)
                {

                    if (!string.IsNullOrEmpty(Vm.SearchField))
                    {
                        if (!string.IsNullOrEmpty(Vm.SearchValue))
                        {

                            da.SelectCommand.Parameters.AddWithValue("@SearchValue", Vm.SearchValue);
                        }
                    }
                    if (Vm.CustomerId != 0)
                    {
                            da.SelectCommand.Parameters.AddWithValue("@CustomerId", Vm.CustomerId);

                    }
                    if (Vm.SalesInvoiceMPLHeaderId != 0)
                    {
                        da.SelectCommand.Parameters.AddWithValue("@SalesInvoiceMPLHeaderId", Vm.SalesInvoiceMPLHeaderId);

                    }
                    if (!string.IsNullOrEmpty(Vm.CRDateFrom))
                    {
                        da.SelectCommand.Parameters.AddWithValue("@CRDateFrom", Vm.CRDateFrom);

                    }
                    if (!string.IsNullOrEmpty(Vm.CRDateTo))
                    {
                        da.SelectCommand.Parameters.AddWithValue("@CRDateTo", Vm.CRDateTo);

                    }

                    da.SelectCommand.Parameters.AddWithValue("@IsUsed", Vm.IsUsed ? "1" : "0");

                }

                da.SelectCommand.Transaction = transaction;
                da.SelectCommand.CommandTimeout = 500;




                da.Fill(dt);

                #endregion SqlExecution

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }



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


        public string[] SalesPost(SalesInvoiceMPLHeaderVM Master
           , SqlTransaction Vtransaction = null, SqlConnection VcurrConn = null, SysDBInfoVMTemp connVM = null, bool BulkPost = false, string UserId = "")
        {
            #region Initializ
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            ProductDAL productDal = new ProductDAL();
            SaleDAL SaleDal = new SaleDAL();
            int transResult = 0;
            string sqlText = "";
            ReceiveDAL recDal = new ReceiveDAL();
            IssueDAL issDal = new IssueDAL();
            List<SaleDetailVm> Details;
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
                    transaction = currConn.BeginTransaction(MessageVM.saleMsgMethodNamePost);
                }

                #endregion open connection and transaction

                #region Post Status

                string currentPostStatus = "N";
                string TransactionType = "Other";

                sqlText = "";
                sqlText = @"
----------declare @InvoiceNo as varchar(100)='SNS-BBA/0720/0001'

select top 1 SalesInvoiceNo, TransactionType, Post from SalesInvoiceMPLHeaders
where 1=1 
and Id=@Id

";
                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
                cmd.CommandTimeout = 500;
                cmd.Parameters.AddWithValue("@Id", Master.Id);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt != null && dt.Rows.Count > 0)
                {
                    currentPostStatus = dt.Rows[0]["Post"].ToString();
                    TransactionType = dt.Rows[0]["TransactionType"].ToString();
                }

                if (currentPostStatus == "Y")
                {
                    throw new Exception("This Invoice Already Posted! Invoice No: " + Master.SalesInvoiceNo);
                }

                #endregion

                #region Find Month Lock

                string PeriodName = Convert.ToDateTime(Master.InvoiceDateTime).ToString("MMMM-yyyy");
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

                #region Settings

                CommonDAL commonDal = new CommonDAL();

                string vNegStockAllow = string.Empty;
                vNegStockAllow = commonDal.settings("Sale", "NegStockAllow", null, null, connVM);
                string postWithDate = commonDal.settings("Integration", "PostWithDateTime", null, null, connVM);
                if (string.IsNullOrEmpty(vNegStockAllow))
                {
                    throw new ArgumentNullException(MessageVM.msgSettingValueNotSave, "Sale");
                }
                bool NegStockAllow = Convert.ToBoolean(vNegStockAllow == "Y" ? true : false);

                #endregion

                if (BulkPost == false)
                {

                    #region Validation for Header

                    if (Master == null)
                    {
                        throw new ArgumentNullException(MessageVM.saleMsgMethodNamePost, MessageVM.saleMsgNoDataToPost);
                    }
                    else if (Convert.ToDateTime(Master.InvoiceDateTime) < DateTime.MinValue || Convert.ToDateTime(Master.InvoiceDateTime) > DateTime.MaxValue)
                    {
                        throw new ArgumentNullException(MessageVM.saleMsgMethodNamePost, MessageVM.saleMsgCheckDatePost);

                    }

                    #endregion Validation for Header

                    #region Old connection 28 Oct 2020

                    #region open connection and transaction
                 

                    #region comment before 28 Oct 2020

                   
                    #endregion

                    #endregion open connection and transaction

                    #endregion Old connection

                    #region Fiscal Year Check

                    string transactionDate = Master.InvoiceDateTime;
                    string transactionYearCheck = Convert.ToDateTime(Master.InvoiceDateTime).ToString("yyyy-MM-dd");
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
                    sqlText = sqlText + "select COUNT(SalesInvoiceNo) from SalesInvoiceMPLHeaders WHERE Id=@Id ";
                    SqlCommand cmdFindIdUpd = new SqlCommand(sqlText, currConn);
                    cmdFindIdUpd.CommandTimeout = 500;
                    cmdFindIdUpd.Transaction = transaction;

                    cmdFindIdUpd.Parameters.AddWithValue("@Id", Master.Id);

                    int IDExist = (int)cmdFindIdUpd.ExecuteScalar();

                    if (IDExist <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.saleMsgMethodNamePost, MessageVM.saleMsgUnableFindExistIDPost);
                    }



                    #endregion Find ID for Update

                }

                #region Stock Check


                if (NegStockAllow == false && Master.TransactionType != "Credit")
                {
                    decimal StockQuantity = 0;

                    Details = SaleDal.SelectSaleDetail(Master.SalesInvoiceNo, null, null, currConn, transaction);

                    #region Stock Check

                    if (Master.TransactionType == "SaleWastage")
                    {
                        #region Comments - Oct-01-2020

                        //DataSet dsWastage = new DataSet();

                        //foreach (var Item in Details)
                        //{
                        //    StockQuantity = 0;

                        //    string MinDate = "1900-Jan-01";
                        //    string MaxDate = "2100-Dec-31";

                        //    dsWastage = new ReportDSDAL().Wastage(Item.ItemNo, "", "", "Y", "Y", MinDate, MaxDate, Master.BranchId, null);

                        //    if (dsWastage != null && dsWastage.Tables.Count > 0)
                        //    {
                        //        StockQuantity = Convert.ToInt32(dsWastage.Tables[0].Rows[0]["WastageBalance"]);
                        //    }


                        //    if (Item.UOMQty > StockQuantity)
                        //    {
                        //        throw new ArgumentNullException(MessageVM.saleMsgMethodNamePost, "(" + Item.ProductCode + ") " + Item.ProductName + Environment.NewLine + MessageVM.saleMsgStockNotAvailablePost);

                        //    }
                        //}

                        #endregion
                    }
                    else if (string.Equals(Master.TransactionType, "TollFinishIssue",
                                 StringComparison.OrdinalIgnoreCase) || string.Equals(Master.TransactionType,
                                 "ContractorRawIssue", StringComparison.OrdinalIgnoreCase))
                    {
                        foreach (var Item in Details)
                        {
                            StockQuantity = 0;
                            var dtTollStock = productDal.GetTollStock(new ParameterVM()
                            {
                                ItemNo = Item.ItemNo,
                            });

                            StockQuantity = Convert.ToDecimal(dtTollStock.Rows[0][0].ToString());

                            if (Item.UOMQty > StockQuantity)
                            {
                                throw new ArgumentNullException(MessageVM.saleMsgMethodNamePost, "(" + Item.ProductCode + ") " + Item.ProductName + Environment.NewLine + MessageVM.saleMsgStockNotAvailablePost);

                            }
                        }

                    }
                    else
                    {
                        DataTable dtAvgPrice = new DataTable();
                        foreach (var Item in Details)
                        {
                            StockQuantity = 0;
                            dtAvgPrice = productDal.AvgPriceNew(Item.ItemNo.Trim(),
                                                                 Convert.ToDateTime(Master.InvoiceDateTime).ToString("yyyy-MMM-dd") +
                                                                DateTime.Now.ToString(" HH:mm:00"), currConn, transaction, true, true, true, false, null, UserId);
                            StockQuantity = Convert.ToDecimal(dtAvgPrice.Rows[0]["Quantity"].ToString());

                            if (Master.TransactionType.ToLower() == "Debit".ToLower())
                            {
                                if (Item.ValueOnly == "N")
                                {
                                    if (Item.UOMQty > StockQuantity)
                                    {
                                        throw new ArgumentNullException(MessageVM.saleMsgMethodNamePost, "(" + Item.ProductCode + ") " + Item.ProductName + Environment.NewLine + MessageVM.saleMsgStockNotAvailablePost);

                                    }
                                }

                            }
                            else
                            {
                                if (Item.UOMQty > StockQuantity)
                                {
                                    throw new ArgumentNullException(MessageVM.saleMsgMethodNamePost, "(" + Item.ProductCode + ") " + Item.ProductName + Environment.NewLine + MessageVM.saleMsgStockNotAvailablePost);

                                }
                            }

                        }
                    }

                    #endregion

                }

                #endregion

                if (BulkPost == false)
                {

                    #region Post

                    sqlText = "";
                    sqlText += @" Update  ReceiveDetails             set  Post ='Y',LastModifiedBy =@MasterLastModifiedBy,LastModifiedOn =@MasterLastModifiedOn    WHERE ReceiveNo=@MasterSalesInvoiceNo ";
                    sqlText += @" Update  ReceiveHeaders             set  Post ='Y',LastModifiedBy =@MasterLastModifiedBy,LastModifiedOn =@MasterLastModifiedOn    WHERE ReceiveNo=@MasterSalesInvoiceNo ";
                    sqlText += @" Update  IssueDetails               set  Post ='Y',LastModifiedBy =@MasterLastModifiedBy,LastModifiedOn =@MasterLastModifiedOn    WHERE IssueNo=@MasterSalesInvoiceNo ";
                    sqlText += @" Update  IssueHeaders               set  Post ='Y',LastModifiedBy =@MasterLastModifiedBy,LastModifiedOn =@MasterLastModifiedOn    WHERE IssueNo=@MasterSalesInvoiceNo ";
                    sqlText += @" Update  IssueDetailBOMs            set  Post ='Y',LastModifiedBy =@MasterLastModifiedBy,LastModifiedOn =@MasterLastModifiedOn    WHERE IssueNo=@MasterSalesInvoiceNo ";
                    sqlText += @" Update  IssueHeaderBOMs            set  Post ='Y',LastModifiedBy =@MasterLastModifiedBy,LastModifiedOn =@MasterLastModifiedOn    WHERE IssueNo=@MasterSalesInvoiceNo ";
                    sqlText += @" Update  PurchaseInvoiceDuties      set  Post ='Y',LastModifiedBy =@MasterLastModifiedBy,LastModifiedOn =@MasterLastModifiedOn    WHERE PurchaseInvoiceNo=@MasterSalesInvoiceNo ";
                    sqlText += @" Update  PurchaseInvoiceDetails     set  Post ='Y',LastModifiedBy =@MasterLastModifiedBy,LastModifiedOn =@MasterLastModifiedOn    WHERE PurchaseInvoiceNo=@MasterSalesInvoiceNo ";
                    sqlText += @" Update  PurchaseInvoiceHeaders     set  Post ='Y',LastModifiedBy =@MasterLastModifiedBy,LastModifiedOn =@MasterLastModifiedOn    WHERE PurchaseInvoiceNo=@MasterSalesInvoiceNo ";
                    sqlText += @" Update  PurchaseTDSs               set  Post ='Y'    WHERE PurchaseInvoiceNo=@MasterSalesInvoiceNo ";
                    // sqlText += @" Update  PurchaseSaleTrackings      set  Post ='Y',LastModifiedBy =@MasterLastModifiedBy,LastModifiedOn =@MasterLastModifiedOn    WHERE PurchaseInvoiceNo=@MasterSalesInvoiceNo ";
                    // sqlText += @" Update  SalesInvoiceHeadersExport  set  Post ='Y',LastModifiedBy =@MasterLastModifiedBy,LastModifiedOn =@MasterLastModifiedOn    WHERE SalesInvoiceNo=@MasterSalesInvoiceNo ";
                    //sqlText += @" Update  SalesInvoiceHeaders        set  SignatoryName=@SignatoryName,SignatoryDesig=@SignatoryDesig, Post ='Y',LastModifiedBy =@MasterLastModifiedBy,LastModifiedOn =@MasterLastModifiedOn    WHERE SalesInvoiceNo=@MasterSalesInvoiceNo ";
                    sqlText += @" Update  SalesInvoiceHeaders        set  Post ='Y',LastModifiedBy =@MasterLastModifiedBy,LastModifiedOn =@MasterLastModifiedOn    WHERE SalesInvoiceNo=@MasterSalesInvoiceNo ";

                    sqlText += @" Update  SalesInvoiceDetails        set  Post ='Y',LastModifiedBy =@MasterLastModifiedBy,LastModifiedOn =@MasterLastModifiedOn    WHERE SalesInvoiceNo=@MasterSalesInvoiceNo ";
                    sqlText += @" update SalesInvoiceHeaders set SignatoryName = UserInformations.FullName,SignatoryDesig=UserInformations.Designation
                                  from UserInformations where UserInformations.UserName = SalesInvoiceHeaders.LastModifiedBy  and SalesInvoiceHeaders.SalesInvoiceNo=@MasterSalesInvoiceNo ";

                    if (postWithDate == "Y")
                    {
                        sqlText += @" Update  SalesInvoiceHeaders        set  InvoiceDateTime = @InvoiceDateTime   WHERE SalesInvoiceNo=@MasterSalesInvoiceNo ";
                        sqlText += @" Update  SalesInvoiceDetails        set  InvoiceDateTime = @InvoiceDateTime    WHERE SalesInvoiceNo=@MasterSalesInvoiceNo ";


                    }

                    sqlText += @" Update  SalesInvoiceMPLHeaders        set  Post ='Y',LastModifiedBy =@MasterLastModifiedBy,LastModifiedOn =@MasterLastModifiedOn    WHERE Id=@Id ";
                    sqlText += @" Update  SalesInvoiceMPLDetails        set  Post ='Y' WHERE SalesInvoiceMPLHeaderId=@Id  ";


                    SqlCommand cmdDeleteDetail = new SqlCommand(sqlText, currConn);
                    cmdDeleteDetail.CommandTimeout = 500;
                    cmdDeleteDetail.Transaction = transaction;
                    cmdDeleteDetail.Parameters.AddWithValueAndNullHandle("@MasterSalesInvoiceNo", Master.SalesInvoiceNo);
                    cmdDeleteDetail.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", OrdinaryVATDesktop.DateToDate(Master.LastModifiedOn));
                    cmdDeleteDetail.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
                    cmdDeleteDetail.Parameters.AddWithValueAndNullHandle("@Id", Master.Id);
                    //cmdDeleteDetail.Parameters.AddWithValueAndNullHandle("@SignatoryDesig", Master.SignatoryDesig);
                    //cmdDeleteDetail.Parameters.AddWithValueAndNullHandle("@SignatoryName", Master.SignatoryName);

                    if (postWithDate == "Y")
                    {
                        string dateTime = commonDal.ServerDateWithTime();
                        cmdDeleteDetail.Parameters.AddWithValueAndNullHandle("@InvoiceDateTime", dateTime);
                    }

                    transResult = cmdDeleteDetail.ExecuteNonQuery();


                    #endregion


                }


                #region Sale_Product_IN_OUT

                if (TransactionType.ToLower() != "ServiceNS")
                {
                    ResultVM rVM = new ResultVM();

                    ParameterVM paramVM = new ParameterVM();
                    paramVM.InvoiceNo = Master.SalesInvoiceNo;

                    rVM = SaleDal.Sale_Product_IN_OUT(paramVM, currConn, transaction);
                }

                #endregion





                #region Commit 28 Oct 2020

                //if (vcurrConn == null)
                //{
                //    if (Vtransaction != null)
                //    {
                //        //if (transResult > 0)
                //        //{
                //        Vtransaction.Commit();
                //        //}

                //    }
                //}

                #endregion Commit

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion

                #region SuccessResult

                retResults[0] = "Success";
                retResults[1] = MessageVM.saleMsgSuccessfullyPost;
                retResults[2] = Master.SalesInvoiceNo;
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
                if (currConn != null)
                {
                    transaction.Rollback();
                }

                FileLogger.Log("SaleDAL", "SalesPost", ex.ToString() + "\n" + sqlText);

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                ////throw ex;
            }

            finally
            {

                #region comment 28 Oct 2020

               

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


        public string[] UpdateSalesDetailsTankInfo(SalesInvoiceMPLDetailVM details, SqlTransaction Vtransaction = null, SqlConnection VcurrConn = null, SysDBInfoVMTemp connVM = null)
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

            #endregion Initializ

            #region Try

            try
            {

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


                #region Update Sales Details TankInfo

                sqlText = @" UPDATE SalesInvoiceMPLDetails SET TankId = @TankId WHERE Id=@Id ";

                SqlCommand cmdDeleteDetail = new SqlCommand(sqlText, currConn, transaction);
                cmdDeleteDetail.Parameters.AddWithValueAndNullHandle("@TankId", details.TankId);
                cmdDeleteDetail.Parameters.AddWithValueAndNullHandle("@Id", details.Id);

                transResult = cmdDeleteDetail.ExecuteNonQuery();

                #endregion

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion

                #region SuccessResult

                retResults = new string[5];
                retResults[0] = "Success";
                retResults[1] = MessageVM.saleMsgUpdateSuccessfully;
                retResults[2] = "" + details.Id;
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


        public string[] UpdateBankPaymentReceiveInfo(MPLBankPaymentReceiveVM details, SqlTransaction Vtransaction = null, SqlConnection VcurrConn = null, SysDBInfoVMTemp connVM = null)
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

            #endregion Initializ

            #region Try

            try
            {

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


                #region Update Sales Details TankInfo

                sqlText = @" UPDATE MPLBankPaymentReceiveDetails SET SalesInvoiceRefId = @SalesInvoiceRefId , IsUsed=@IsUsed WHERE Id=@Id ";

                SqlCommand cmdDeleteDetail = new SqlCommand(sqlText, currConn, transaction);
                cmdDeleteDetail.Parameters.AddWithValueAndNullHandle("@Id", details.Id);
                cmdDeleteDetail.Parameters.AddWithValueAndNullHandle("@SalesInvoiceRefId", details.SalesInvoiceRefId);
                cmdDeleteDetail.Parameters.AddWithValueAndNullHandle("@IsUsed", true);

                transResult = cmdDeleteDetail.ExecuteNonQuery();

                #endregion

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion

                #region SuccessResult

                retResults = new string[5];
                retResults[0] = "Success";
                retResults[1] = MessageVM.saleMsgUpdateSuccessfully;
                retResults[2] = "" + details.Id;
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

                FileLogger.Log("UpdateBankPaymentReceiveInfo", "SaleMPLDAL", ex.ToString() + "\n" + sqlText);

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

        public string[] SaleMPLPost(SalesInvoiceMPLHeaderVM MasterVM, SqlTransaction transaction, SqlConnection currConn, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";
            string Id = "";
            string ids = "";
            string salesInvoiceNo = "";
            string invoiceNo = "";
            string saleId = "";

            int transResult = 0;
            string sqlText = "";
            string PostStatus = "";

            #endregion Initializ

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
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction(MessageVM.saleMsgMethodNameInsert);
                }


                #endregion open connection and transaction

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

                    if (!string.IsNullOrEmpty(MasterVM.IDs[i]))
                    {
                        DataTable dt = new DataTable();
                        sqlText = "";
                        sqlText = sqlText + "SELECT TOP 1 SalesInvoiceNo FROM SalesInvoiceMPLHeaders WHERE Id=@Id ";

                        SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                        da.SelectCommand.Transaction = transaction;
                        da.SelectCommand.Parameters.AddWithValue("@Id", MasterVM.IDs[i]);
                        da.Fill(dt);

                        if (dt.Rows.Count > 0)
                        {
                            invoiceNo = dt.Rows[0][0].ToString();
                        }

                        if (string.IsNullOrEmpty(salesInvoiceNo))
                        {
                            salesInvoiceNo = "'" + invoiceNo + "'";
                        }
                        else
                        {
                            salesInvoiceNo += "," + "'" + invoiceNo + "'";
                        }
                    }
                }


                #endregion Validation for Header


                #region MPL Sale Update Issue Data

                sqlText = "";
                sqlText +=
                 @" UPDATE SalesInvoiceMPLHeaders SET Post='Y',LastModifiedBy=@LastModifiedBy,LastModifiedOn=GETDATE() ";
                sqlText += @"  WHERE Id IN (" + ids + ") ";
                sqlText +=
                    @" UPDATE SalesInvoiceMPLDetails SET Post='Y'  ";
                sqlText += @"  WHERE SalesInvoiceMPLHeaderId IN (" + ids + ") ";
                //if (!string.IsNullOrEmpty(salesInvoiceNo))
                //{
                //    sqlText +=
                //        @" UPDATE SalesInvoiceHeaders SET Post='Y'  ";
                //    sqlText += @"  WHERE SalesInvoiceNo IN (" + salesInvoiceNo + ") ";
                //}

                SqlCommand cmdDeleteDetail = new SqlCommand(sqlText, currConn, transaction);
                cmdDeleteDetail.Parameters.AddWithValueAndNullHandle("@LastModifiedBy", MasterVM.LastModifiedBy);


                transResult = cmdDeleteDetail.ExecuteNonQuery();
                
                #endregion


                #region General Sale Update  Issue Data

                if (!string.IsNullOrEmpty(salesInvoiceNo))
                {
                    string[] sale = salesInvoiceNo.Split(',');

                    for (int j = 0; j < sale.Length; j++)
                    {
                        string[] rresult = new string[6];
                        DataTable saledt = new DataTable();
                        SaleDAL dal = new SaleDAL();
                        sqlText = "";
                        sqlText = sqlText + "SELECT TOP 1 Id FROM SalesInvoiceHeaders WHERE SalesInvoiceNo IN (" + sale[j] + ") ";

                        SqlDataAdapter sda = new SqlDataAdapter(sqlText, currConn);
                        sda.SelectCommand.Transaction = transaction;
                        sda.Fill(saledt);

                        if (saledt.Rows.Count > 0)
                        {
                            saleId = saledt.Rows[0][0].ToString();

                            SaleMasterVM vm = new SaleMasterVM();
                            vm = dal.SelectAllList(Convert.ToInt32(saleId)).FirstOrDefault();
                            List<SaleDetailVm> SaleDetailVMS = new List<SaleDetailVm>();
                            List<TrackingVM> TrackingVM = new List<TrackingVM>();

                            SaleDetailVMS = dal.SelectSaleDetail(vm.SalesInvoiceNo);
                            TrackingVM = dal.GetTrackingsWeb(SaleDetailVMS, vm.SalesInvoiceNo, null);

                            vm.Details = SaleDetailVMS;
                            vm.Trackings = TrackingVM;

                            vm.SignatoryName =  UserInfoVM.UserName;
                            vm.SignatoryDesig = UserInfoVM.UserName;
                            vm.LastModifiedBy = UserInfoVM.UserName;
                            vm.LastModifiedOn = DateTime.Now.ToString();
                            vm.Post = "Y";
                            rresult = dal.SalesPost(vm, vm.Details, vm.Trackings);
                        }
                    }
                }
                

                #endregion

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
                retResults[2] = "" + MasterVM.SalesInvoiceNo;
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
                FileLogger.Log("SaleMPLDAL", "SaleMPLPost", ex.ToString() + "\n" + sqlText);

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

