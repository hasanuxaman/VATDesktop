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
    public class MPLSalesInvSA4DAL : IMPLSalesInvSA4
    {
        #region Global Variables

        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        CommonDAL _cDAL = new CommonDAL();

        #endregion


        public string[] MPLSalesInvSA4Insert(MPLSaleInvoiceSA4VM MasterVM, SqlTransaction Vtransaction = null, SqlConnection VcurrConn = null, SysDBInfoVMTemp connVM = null)
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

                #region Validation for Header

                if (MasterVM == null)
                {
                    throw new ArgumentNullException(MessageVM.insert, MessageVM.saleMsgNoDataToSave);
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

                #region ID generation

                newIDCreate = commonDal.TransactionCode("Sale", "Other", "MPLSaleInvoiceSA4", "SalesInvoiceNo",
                    "InvoiceDateTime", MasterVM.InvoiceDateTime, MasterVM.BranchId.ToString(), currConn, transaction);

                MasterVM.SalesInvoiceNo = newIDCreate;

                #endregion

                #region Find SaleInvoiceReceiptNo Exist

                sqlText = "";
                sqlText = sqlText + "select COUNT(Id) from MPLSaleInvoiceSA4 WHERE SalesInvoiceNo=@SalesInvoiceNo AND BranchId = @BranchId ";
                SqlCommand cmdExistTran = new SqlCommand(sqlText, currConn, transaction);
                cmdExistTran.Parameters.AddWithValueAndNullHandle("@SalesInvoiceNo", MasterVM.SalesInvoiceNo);
                cmdExistTran.Parameters.AddWithValueAndNullHandle("@BranchId", MasterVM.BranchId);
                IDExist = (int)cmdExistTran.ExecuteScalar();

                if (IDExist > 0)
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgFindExistID);
                }

                #endregion Find SaleInvoiceReceiptNo Exist

                
                #region ID generated completed,Insert new Information in Header

                MasterVM.Amount = Convert.ToDecimal(commonDal.decimal259(MasterVM.Amount));
                MasterVM.PricePerMTon = Convert.ToDecimal(commonDal.decimal259(MasterVM.PricePerMTon));
                MasterVM.CostOfOil = Convert.ToDecimal(commonDal.decimal259(MasterVM.CostOfOil));
                MasterVM.BargeHireCharge = Convert.ToDecimal(commonDal.decimal259(MasterVM.BargeHireCharge));
                MasterVM.StatutoryCharge = Convert.ToDecimal(commonDal.decimal259(MasterVM.StatutoryCharge));
                MasterVM.CurrentRateFromUSD = Convert.ToDecimal(commonDal.decimal259(MasterVM.CurrentRateFromUSD));
                MasterVM.TotalCostBDT = Convert.ToDecimal(commonDal.decimal259(MasterVM.TotalCostBDT));

                MasterVM.InvoiceDateTime = OrdinaryVATDesktop.DateToDate(MasterVM.InvoiceDateTime);
                MasterVM.DeliveryDate = OrdinaryVATDesktop.DateToDate(MasterVM.DeliveryDate);
                MasterVM.CustomGuaranteeDate = OrdinaryVATDesktop.DateToDate(MasterVM.CustomGuaranteeDate);
                MasterVM.InstrumentDate = OrdinaryVATDesktop.DateToDate(MasterVM.InstrumentDate);
                MasterVM.CRDate = OrdinaryVATDesktop.DateToDate(MasterVM.CRDate);

                retResults = MPLSalesInvSA4InsertToMaster(MasterVM, currConn, transaction, null, settings);

                Id = retResults[4];

                if (retResults[0] != "Success")
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, retResults[1]);
                }

                #endregion ID generated completed,Insert new Information in Header


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

        public string[] MPLSalesInvSA4InsertToMaster(MPLSaleInvoiceSA4VM Master, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null, DataTable settingsDt = null)
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
                sqlText += " INSERT INTO MPLSaleInvoiceSA4";
                sqlText += " (";
                sqlText += " BranchId";
                sqlText += " ,SalesInvoiceNo";
                sqlText += " ,InvoiceDateTime";
                sqlText += " ,PortOfDelivery";
                sqlText += " ,DeliveryDate";
                sqlText += " ,CustomerID";
                sqlText += " ,DeliveredFor";
                sqlText += " ,ProuctDelivered";
                sqlText += " ,DeliveredBy";
                sqlText += " ,VesselDockedAt";
                sqlText += " ,Started";
                sqlText += " ,Finished";
                sqlText += " ,PumpingFrom";
                sqlText += " ,PumpingTo";
                sqlText += " ,TankId";
                sqlText += " ,DIPBefore";
                sqlText += " ,DIPAfter";
                sqlText += " ,LocalMeasuredVolume";
                sqlText += " ,LocalVolume";
                sqlText += " ,LocalVolumeBefore";
                sqlText += " ,LocalVolumeAfter";
                sqlText += " ,GrossOilDeliveryQty";
                sqlText += " ,IsBondedDutyPaid";
                sqlText += " ,MTones";
                sqlText += " ,ObservedTankTemp";
                sqlText += " ,APIGravityAt";
                sqlText += " ,FlashPointPM";
                sqlText += " ,ViscosityKinematicCSTA";
                sqlText += " ,ViscosityKinematicCSTB";
                sqlText += " ,BSW";
                sqlText += " ,RemarksObsSPGR";
                sqlText += " ,SulphurContentWT";
                sqlText += " ,LocalAgent";
                sqlText += " ,NextPortofCall";
                sqlText += " ,ExportRotationNo";
                sqlText += " ,Plag";
                sqlText += " ,CustomGuaranteeNo";
                sqlText += " ,CustomGuaranteeDate";
                sqlText += " ,CustomGuaranteeImoNo";
                sqlText += " ,CustomGuaranteeSealMark";
                sqlText += " ,AdditionalInfo";
                sqlText += " ,BankId";
                sqlText += " ,ModeOfPayment";
                sqlText += " ,InstrumentNo";
                sqlText += " ,InstrumentDate";
                sqlText += " ,Amount";
                sqlText += " ,CRNo";
                sqlText += " ,CRDate";
                sqlText += " ,SupplierConformation";
                sqlText += " ,PricePerMTon";
                sqlText += " ,CostOfOil";
                sqlText += " ,BargeHireCharge";
                sqlText += " ,StatutoryCharge";
                sqlText += " ,CurrentRateFromUSD";
                sqlText += " ,TotalCostBDT";
                sqlText += " ,ReceivedVessel";
                sqlText += " ,ReceivedOwner";
                sqlText += " ,SupplierOrBargeCaptainCert";
                sqlText += " ,Comments";
                sqlText += " ,Post";
                sqlText += " ,CreatedBy";
                sqlText += " ,CreatedOn";

                sqlText += " )";

                sqlText += " VALUES";
                sqlText += " (";
                sqlText += "  @BranchId";
                sqlText += "  ,@SalesInvoiceNo";
                sqlText += "  ,@InvoiceDateTime";
                sqlText += "  ,@PortOfDelivery";
                sqlText += "  ,@DeliveryDate";
                sqlText += "  ,@CustomerID";
                sqlText += "  ,@DeliveredFor";
                sqlText += "  ,@ProuctDelivered";
                sqlText += "  ,@DeliveredBy";
                sqlText += "  ,@VesselDockedAt";
                sqlText += "  ,@Started";
                sqlText += "  ,@Finished";
                sqlText += "  ,@PumpingFrom";
                sqlText += "  ,@PumpingTo";
                sqlText += "  ,@TankId";
                sqlText += "  ,@DIPBefore";
                sqlText += "  ,@DIPAfter";
                sqlText += "  ,@LocalMeasuredVolume";
                sqlText += "  ,@LocalVolume";
                sqlText += "  ,@LocalVolumeBefore";
                sqlText += "  ,@LocalVolumeAfter";
                sqlText += "  ,@GrossOilDeliveryQty";
                sqlText += "  ,@IsBondedDutyPaid";
                sqlText += "  ,@MTones";
                sqlText += "  ,@ObservedTankTemp";
                sqlText += "  ,@APIGravityAt";
                sqlText += "  ,@FlashPointPM";
                sqlText += "  ,@ViscosityKinematicCSTA";
                sqlText += "  ,@ViscosityKinematicCSTB";
                sqlText += "  ,@BSW";
                sqlText += "  ,@RemarksObsSPGR";
                sqlText += "  ,@SulphurContentWT";
                sqlText += "  ,@LocalAgent";
                sqlText += "  ,@NextPortofCall";
                sqlText += "  ,@ExportRotationNo";
                sqlText += "  ,@Plag";
                sqlText += "  ,@CustomGuaranteeNo";
                sqlText += "  ,@CustomGuaranteeDate";
                sqlText += "  ,@CustomGuaranteeImoNo";
                sqlText += "  ,@CustomGuaranteeSealMark";
                sqlText += "  ,@AdditionalInfo";
                sqlText += "  ,@BankId";
                sqlText += "  ,@ModeOfPayment";
                sqlText += "  ,@InstrumentNo";
                sqlText += "  ,@InstrumentDate";
                sqlText += "  ,@Amount";
                sqlText += "  ,@CRNo";
                sqlText += "  ,@CRDate";
                sqlText += "  ,@SupplierConformation";
                sqlText += "  ,@PricePerMTon";
                sqlText += "  ,@CostOfOil";
                sqlText += "  ,@BargeHireCharge";
                sqlText += "  ,@StatutoryCharge";
                sqlText += "  ,@CurrentRateFromUSD";
                sqlText += "  ,@TotalCostBDT";
                sqlText += "  ,@ReceivedVessel";
                sqlText += "  ,@ReceivedOwner";
                sqlText += "  ,@SupplierOrBargeCaptainCert";
                sqlText += "  ,@Comments";
                sqlText += "  ,@Post";
                sqlText += "  ,@CreatedBy";
                sqlText += "  ,@CreatedOn";


                sqlText += ")  SELECT SCOPE_IDENTITY() ";

                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);

                cmdInsert.Parameters.AddWithValueAndNullHandle("@BranchId", Master.BranchId);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@SalesInvoiceNo", Master.SalesInvoiceNo);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@InvoiceDateTime", Master.InvoiceDateTime);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@PortOfDelivery", Master.PortOfDelivery);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@DeliveryDate", Master.DeliveryDate);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@CustomerID", Master.CustomerID);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@DeliveredFor", Master.DeliveredFor);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@ProuctDelivered", Master.ProuctDelivered);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@DeliveredBy", Master.DeliveredBy);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@VesselDockedAt", Master.VesselDockedAt);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Started", Master.Started);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Finished", Master.Finished);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@PumpingFrom", Master.PumpingFrom);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@PumpingTo", Master.PumpingTo);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@TankId", Master.TankId);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@DIPBefore", Master.DIPBefore);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@DIPAfter", Master.DIPAfter);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@LocalMeasuredVolume", Master.LocalMeasuredVolume);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@LocalVolume", Master.LocalVolume);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@LocalVolumeBefore", Master.LocalVolumeBefore);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@LocalVolumeAfter", Master.LocalVolumeAfter);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@GrossOilDeliveryQty", Master.GrossOilDeliveryQty);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@IsBondedDutyPaid", Master.IsBondedDutyPaid);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MTones", Master.MTones);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@ObservedTankTemp", Master.ObservedTankTemp);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@APIGravityAt", Master.APIGravityAt);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@FlashPointPM", Master.FlashPointPM);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@ViscosityKinematicCSTA", Master.ViscosityKinematicCSTA);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@ViscosityKinematicCSTB", Master.ViscosityKinematicCSTB);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@BSW", Master.BSW);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@RemarksObsSPGR", Master.RemarksObsSPGR);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@SulphurContentWT", Master.SulphurContentWT);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@LocalAgent", Master.LocalAgent);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@NextPortofCall", Master.NextPortofCall);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@ExportRotationNo", Master.ExportRotationNo);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Plag", Master.Plag);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@CustomGuaranteeNo", Master.CustomGuaranteeNo);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@CustomGuaranteeDate", Master.CustomGuaranteeDate);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@CustomGuaranteeImoNo", Master.CustomGuaranteeImoNo);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@CustomGuaranteeSealMark", Master.CustomGuaranteeSealMark);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@AdditionalInfo", Master.AdditionalInfo);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@BankId", Master.BankId);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@ModeOfPayment", Master.ModeOfPayment);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@InstrumentNo", Master.InstrumentNo);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@InstrumentDate", Master.InstrumentDate);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Amount", Master.Amount);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@CRNo", Master.CRNo);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@CRDate", Master.CRDate);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@SupplierConformation", Master.SupplierConformation);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@PricePerMTon", Master.PricePerMTon);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@CostOfOil", Master.CostOfOil);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@BargeHireCharge", Master.BargeHireCharge);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@StatutoryCharge", Master.StatutoryCharge);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@CurrentRateFromUSD", Master.CurrentRateFromUSD);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@TotalCostBDT", Master.TotalCostBDT);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@ReceivedVessel", Master.ReceivedVessel);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@ReceivedOwner", Master.ReceivedOwner);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@SupplierOrBargeCaptainCert", Master.SupplierOrBargeCaptainCert);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Comments", Master.Comments);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Post", Master.Post);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@CreatedBy", Master.CreatedBy);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@CreatedOn", Master.CreatedOn);

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

        public string[] MPLSalesInvSA4Update(MPLSaleInvoiceSA4VM MasterVM, SqlTransaction transaction, SqlConnection currConn, SysDBInfoVMTemp connVM = null)
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

            #endregion Initializ

            #region Try
            try
            {

                #region Validation for Header

                if (MasterVM == null)
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgNoDataToSave);
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

                MasterVM.Amount = Convert.ToDecimal(commonDal.decimal259(MasterVM.Amount));
                MasterVM.PricePerMTon = Convert.ToDecimal(commonDal.decimal259(MasterVM.PricePerMTon));
                MasterVM.CostOfOil = Convert.ToDecimal(commonDal.decimal259(MasterVM.CostOfOil));
                MasterVM.BargeHireCharge = Convert.ToDecimal(commonDal.decimal259(MasterVM.BargeHireCharge));
                MasterVM.StatutoryCharge = Convert.ToDecimal(commonDal.decimal259(MasterVM.StatutoryCharge));
                MasterVM.CurrentRateFromUSD = Convert.ToDecimal(commonDal.decimal259(MasterVM.CurrentRateFromUSD));
                MasterVM.TotalCostBDT = Convert.ToDecimal(commonDal.decimal259(MasterVM.TotalCostBDT));

                MasterVM.InvoiceDateTime = OrdinaryVATDesktop.DateToDate(MasterVM.InvoiceDateTime);
                MasterVM.DeliveryDate = OrdinaryVATDesktop.DateToDate(MasterVM.DeliveryDate);
                MasterVM.CustomGuaranteeDate = OrdinaryVATDesktop.DateToDate(MasterVM.CustomGuaranteeDate);
                MasterVM.InstrumentDate = OrdinaryVATDesktop.DateToDate(MasterVM.InstrumentDate);
                MasterVM.CRDate = OrdinaryVATDesktop.DateToDate(MasterVM.CRDate);

                retResults = MPLSalesInvSA4UpdateToMaster(MasterVM, currConn, transaction, null, settings);

                Id = retResults[2];

                if (retResults[0] != "Success")
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, retResults[1]);
                }

                #endregion Update Information in Header

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

        public string[] MPLSalesInvSA4UpdateToMaster(MPLSaleInvoiceSA4VM Master, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null, DataTable settingsDt = null)
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
                sqlText += " UPDATE MPLSaleInvoiceSA4 SET  ";

                sqlText += " InvoiceDateTime=@InvoiceDateTime";
                sqlText += " ,PortOfDelivery=@PortOfDelivery";
                sqlText += " ,DeliveryDate=@DeliveryDate";
                sqlText += " ,CustomerID=@CustomerID";
                sqlText += " ,DeliveredFor=@DeliveredFor";
                sqlText += " ,ProuctDelivered=@ProuctDelivered";
                sqlText += " ,DeliveredBy=@DeliveredBy";
                sqlText += " ,VesselDockedAt=@VesselDockedAt";
                sqlText += " ,Started=@Started";
                sqlText += " ,Finished=@Finished";
                sqlText += " ,PumpingFrom=@PumpingFrom";
                sqlText += " ,PumpingTo=@PumpingTo";
                sqlText += " ,TankId=@TankId";
                sqlText += " ,DIPBefore=@DIPBefore";
                sqlText += " ,DIPAfter=@DIPAfter";
                sqlText += " ,LocalMeasuredVolume=@LocalMeasuredVolume";
                sqlText += " ,LocalVolume=@LocalVolume";
                sqlText += " ,LocalVolumeBefore=@LocalVolumeBefore";
                sqlText += " ,LocalVolumeAfter=@LocalVolumeAfter";
                sqlText += " ,GrossOilDeliveryQty=@GrossOilDeliveryQty";
                sqlText += " ,IsBondedDutyPaid=@IsBondedDutyPaid";
                sqlText += " ,MTones=@MTones";
                sqlText += " ,ObservedTankTemp=@ObservedTankTemp";
                sqlText += " ,APIGravityAt=@APIGravityAt";
                sqlText += " ,FlashPointPM=@FlashPointPM";
                sqlText += " ,ViscosityKinematicCSTA=@ViscosityKinematicCSTA";
                sqlText += " ,ViscosityKinematicCSTB=@ViscosityKinematicCSTB";
                sqlText += " ,BSW=@BSW";
                sqlText += " ,RemarksObsSPGR=@RemarksObsSPGR";
                sqlText += " ,SulphurContentWT=@SulphurContentWT";
                sqlText += " ,LocalAgent=@LocalAgent";
                sqlText += " ,NextPortofCall=@NextPortofCall";
                sqlText += " ,ExportRotationNo=@ExportRotationNo";
                sqlText += " ,Plag=@Plag";
                sqlText += " ,CustomGuaranteeNo=@CustomGuaranteeNo";
                sqlText += " ,CustomGuaranteeDate=@CustomGuaranteeDate";
                sqlText += " ,CustomGuaranteeImoNo=@CustomGuaranteeImoNo";
                sqlText += " ,CustomGuaranteeSealMark=@CustomGuaranteeSealMark";
                sqlText += " ,AdditionalInfo=@AdditionalInfo";
                sqlText += " ,BankId=@BankId";
                sqlText += " ,ModeOfPayment=@ModeOfPayment";
                sqlText += " ,InstrumentNo=@InstrumentNo";
                sqlText += " ,InstrumentDate=@InstrumentDate";
                sqlText += " ,Amount=@Amount";
                sqlText += " ,CRNo=@CRNo";
                sqlText += " ,CRDate=@CRDate";
                sqlText += " ,SupplierConformation=@SupplierConformation";
                sqlText += " ,PricePerMTon=@PricePerMTon";
                sqlText += " ,CostOfOil=@CostOfOil";
                sqlText += " ,BargeHireCharge=@BargeHireCharge";
                sqlText += " ,StatutoryCharge=@StatutoryCharge";
                sqlText += " ,CurrentRateFromUSD=@CurrentRateFromUSD";
                sqlText += " ,TotalCostBDT=@TotalCostBDT";
                sqlText += " ,ReceivedVessel=@ReceivedVessel";
                sqlText += " ,ReceivedOwner=@ReceivedOwner";
                sqlText += " ,SupplierOrBargeCaptainCert=@SupplierOrBargeCaptainCert";
                sqlText += " ,Comments=@Comments";
                sqlText += " ,LastModifiedBy=@LastModifiedBy";
                sqlText += " ,LastModifiedOn=@LastModifiedOn";

                sqlText += " WHERE Id=@Id";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@InvoiceDateTime", Master.InvoiceDateTime);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@PortOfDelivery", Master.PortOfDelivery);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@DeliveryDate", Master.DeliveryDate);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@CustomerID", Master.CustomerID);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@DeliveredFor", Master.DeliveredFor);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@ProuctDelivered", Master.ProuctDelivered);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@DeliveredBy", Master.DeliveredBy);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@VesselDockedAt", Master.VesselDockedAt);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@Started", Master.Started);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@Finished", Master.Finished);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@PumpingFrom", Master.PumpingFrom);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@PumpingTo", Master.PumpingTo);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@TankId", Master.TankId);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@DIPBefore", Master.DIPBefore);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@DIPAfter", Master.DIPAfter);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@LocalMeasuredVolume", Master.LocalMeasuredVolume);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@LocalVolume", Master.LocalVolume);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@LocalVolumeBefore", Master.LocalVolumeBefore);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@LocalVolumeAfter", Master.LocalVolumeAfter);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@GrossOilDeliveryQty", Master.GrossOilDeliveryQty);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@IsBondedDutyPaid", Master.IsBondedDutyPaid);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@MTones", Master.MTones);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@ObservedTankTemp", Master.ObservedTankTemp);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@APIGravityAt", Master.APIGravityAt);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@FlashPointPM", Master.FlashPointPM);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@ViscosityKinematicCSTA", Master.ViscosityKinematicCSTA);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@ViscosityKinematicCSTB", Master.ViscosityKinematicCSTB);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@BSW", Master.BSW);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@RemarksObsSPGR", Master.RemarksObsSPGR);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@SulphurContentWT", Master.SulphurContentWT);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@LocalAgent", Master.LocalAgent);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@NextPortofCall", Master.NextPortofCall);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@ExportRotationNo", Master.ExportRotationNo);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@Plag", Master.Plag);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@CustomGuaranteeNo", Master.CustomGuaranteeNo);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@CustomGuaranteeDate", Master.CustomGuaranteeDate);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@CustomGuaranteeImoNo", Master.CustomGuaranteeImoNo);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@CustomGuaranteeSealMark", Master.CustomGuaranteeSealMark);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@AdditionalInfo", Master.AdditionalInfo);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@BankId", Master.BankId);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@ModeOfPayment", Master.ModeOfPayment);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@InstrumentNo", Master.InstrumentNo);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@InstrumentDate", Master.InstrumentDate);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@Amount", Master.Amount);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@CRNo", Master.CRNo);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@CRDate", Master.CRDate);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@SupplierConformation", Master.SupplierConformation);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@PricePerMTon", Master.PricePerMTon);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@CostOfOil", Master.CostOfOil);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@BargeHireCharge", Master.BargeHireCharge);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@StatutoryCharge", Master.StatutoryCharge);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@CurrentRateFromUSD", Master.CurrentRateFromUSD);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@TotalCostBDT", Master.TotalCostBDT);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@ReceivedVessel", Master.ReceivedVessel);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@ReceivedOwner", Master.ReceivedOwner);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@SupplierOrBargeCaptainCert", Master.SupplierOrBargeCaptainCert);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@Comments", Master.Comments);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@LastModifiedBy", Master.LastModifiedBy);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@LastModifiedOn", Master.LastModifiedOn);

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

        public DataTable SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null, string[] ids = null, string ActiveStatus = "", string SelectTop = "100")
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


 TD.Id
,TD.BranchId
,TD.SalesInvoiceNo
,ISNULL(TD.InvoiceDateTime,'') InvoiceDateTime
,TD.PortOfDelivery
,ISNULL(TD.DeliveryDate,'') DeliveryDate
,ISNULL(TD.CustomerID,'0') CustomerID
,TD.DeliveredFor
,TD.ProuctDelivered
,TD.DeliveredBy
,TD.VesselDockedAt
,TD.Started
,TD.Finished
,TD.PumpingFrom
,TD.PumpingTo
,ISNULL(TD.TankId,'0') TankId
,TD.DIPBefore
,TD.DIPAfter
,TD.LocalMeasuredVolume
,TD.LocalVolume
,TD.LocalVolumeBefore
,TD.LocalVolumeAfter
,ISNULL(TD.GrossOilDeliveryQty,'0') GrossOilDeliveryQty
,ISNULL(TD.IsBondedDutyPaid,'N') IsBondedDutyPaid
,TD.MTones
,TD.ObservedTankTemp
,TD.APIGravityAt
,TD.FlashPointPM
,TD.ViscosityKinematicCSTA
,TD.ViscosityKinematicCSTB
,TD.BSW
,TD.RemarksObsSPGR
,TD.SulphurContentWT
,TD.LocalAgent
,TD.NextPortofCall
,TD.ExportRotationNo
,TD.Plag
,TD.CustomGuaranteeNo
,TD.CustomGuaranteeDate
,TD.CustomGuaranteeImoNo
,TD.CustomGuaranteeSealMark
,TD.AdditionalInfo
,ISNULL(TD.BankId,'0') BankId
,TD.ModeOfPayment
,TD.InstrumentNo
,ISNULL(TD.InstrumentDate,'') InstrumentDate
,TD.CRNo
,ISNULL(TD.CRDate,'') CRDate
,TD.SupplierConformation
,ISNULL(TD.Amount,'0') Amount
,ISNULL(TD.PricePerMTon,'0') PricePerMTon
,ISNULL(TD.CostOfOil,'0') CostOfOil
,ISNULL(TD.BargeHireCharge,'0') BargeHireCharge
,ISNULL(TD.StatutoryCharge,'0') StatutoryCharge
,ISNULL(TD.CurrentRateFromUSD,'0') CurrentRateFromUSD
,ISNULL(TD.TotalCostBDT,'0') TotalCostBDT
,TD.ReceivedVessel
,TD.ReceivedOwner
,TD.SupplierOrBargeCaptainCert
,TD.Comments
,ISNULL(TD.Post,'N') Post
,TD.CreatedBy
,ISNULL(TD.CreatedOn,'') CreatedOn
,TD.LastModifiedBy
,ISNULL(TD.LastModifiedOn,'') LastModifiedOn

,ISNULL(C.CustomerName,'') CustomerName
,ISNULL(CC.CustomerName,'') DeliveredForName
,ISNULL(CCC.CustomerName,'') DeliveredByName

,ISNULL(B.BranchName,'') BranchName
,ISNULL(T.TankCode,'') TankCode
,ISNULL(MB.BankName,'') BankName

FROM MPLSaleInvoiceSA4 TD
LEFT OUTER JOIN Customers C ON TD.CustomerID = C.CustomerID
LEFT OUTER JOIN Customers CC ON TD.DeliveredFor = CC.CustomerID
LEFT OUTER JOIN Customers CCC ON TD.DeliveredBy = CCC.CustomerID
LEFT OUTER JOIN BranchProfiles B ON B.BranchID = TD.BranchId
LEFT OUTER JOIN TankMPLs T ON TD.TankId = T.Id
LEFT OUTER JOIN MPLBDBankInformations MB ON TD.BankId = MB.BankID
WHERE  1=1 ";
                #endregion SqlText

                sqlTextCount += @" 
SELECT count(TD.Id) RecordCount
FROM MPLSaleInvoiceSA4 TD WHERE  1=1 ";

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
                    sqlText += " AND TD.SalesInvoiceNo IN (" + sqlText2 + ")";
                }

                if (Id > 0)
                {
                    sqlTextParameter += @" AND TD.Id=@Id";
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

                sqlTextOrderBy += " order by TD.Id desc";

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
            catch (SqlException sqlex)
            {
                FileLogger.Log("MPLSalesInvSA4DAL", "SelectAll", sqlex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                FileLogger.Log("MPLSalesInvSA4DAL", "SelectAll", ex.ToString() + "\n" + sqlText);
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

        public List<MPLSaleInvoiceSA4VM> SelectAllList(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null, string[] ids = null, string ActiveStatus = "", string SelectTop = "100")
        {
            #region Variables

            string sqlText = "";
            List<MPLSaleInvoiceSA4VM> VMs = new List<MPLSaleInvoiceSA4VM>();
            MPLSaleInvoiceSA4VM vm;
            #endregion

            #region try

            try
            {
                #region sql statement

                #region SqlExecution

                DataTable dt = SelectAll(Id, conditionFields, conditionValues, VcurrConn, Vtransaction, connVM, ids, ActiveStatus, SelectTop);
                //dt.Rows.RemoveAt(dt.Rows.Count - 1);
                foreach (DataRow dr in dt.Rows)
                {
                    try
                    {
                        vm = new MPLSaleInvoiceSA4VM();

                        vm.Id = Convert.ToInt32(dr["Id"].ToString());
                        vm.BranchId = Convert.ToInt32(dr["BranchId"].ToString());
                        vm.TankId = Convert.ToInt32(dr["TankId"].ToString());

                        vm.SalesInvoiceNo = dr["SalesInvoiceNo"].ToString();
                        vm.InvoiceDateTime = OrdinaryVATDesktop.DateTimeToDate(dr["InvoiceDateTime"].ToString());
                        vm.PortOfDelivery = dr["PortOfDelivery"].ToString();
                        vm.DeliveryDate = OrdinaryVATDesktop.DateTimeToDate(dr["DeliveryDate"].ToString());
                        vm.CustomerID = dr["CustomerID"].ToString();
                        vm.DeliveredFor = dr["DeliveredFor"].ToString();
                        vm.ProuctDelivered = dr["ProuctDelivered"].ToString();
                        vm.DeliveredBy = dr["DeliveredBy"].ToString();
                        vm.VesselDockedAt = dr["VesselDockedAt"].ToString();
                        vm.Started = dr["Started"].ToString();
                        vm.Finished = dr["Finished"].ToString();
                        vm.PumpingFrom = dr["PumpingFrom"].ToString();
                        vm.PumpingTo = dr["PumpingTo"].ToString();
                        vm.DIPBefore = dr["DIPBefore"].ToString();
                        vm.DIPAfter = dr["DIPAfter"].ToString();
                        vm.LocalMeasuredVolume = dr["LocalMeasuredVolume"].ToString();
                        vm.LocalVolume = dr["LocalVolume"].ToString();
                        vm.LocalVolumeBefore = dr["LocalVolumeBefore"].ToString();
                        vm.LocalVolumeAfter = dr["LocalVolumeAfter"].ToString();
                        vm.GrossOilDeliveryQty = dr["GrossOilDeliveryQty"].ToString();
                        vm.IsBondedDutyPaid = dr["IsBondedDutyPaid"].ToString();
                        vm.MTones = dr["MTones"].ToString();
                        vm.ObservedTankTemp = dr["ObservedTankTemp"].ToString();
                        vm.APIGravityAt = dr["APIGravityAt"].ToString();
                        vm.FlashPointPM = dr["FlashPointPM"].ToString();
                        vm.ViscosityKinematicCSTA = dr["ViscosityKinematicCSTA"].ToString();
                        vm.ViscosityKinematicCSTB = dr["ViscosityKinematicCSTB"].ToString();
                        vm.BSW = dr["BSW"].ToString();
                        vm.RemarksObsSPGR = dr["RemarksObsSPGR"].ToString();
                        vm.SulphurContentWT = dr["SulphurContentWT"].ToString();
                        vm.LocalAgent = dr["LocalAgent"].ToString();
                        vm.NextPortofCall = dr["NextPortofCall"].ToString();
                        vm.ExportRotationNo = dr["ExportRotationNo"].ToString();
                        vm.Plag = dr["Plag"].ToString();
                        vm.CustomGuaranteeNo = dr["CustomGuaranteeNo"].ToString();
                        vm.CustomGuaranteeDate = OrdinaryVATDesktop.DateTimeToDate(dr["CustomGuaranteeDate"].ToString());
                        vm.CustomGuaranteeImoNo = dr["CustomGuaranteeImoNo"].ToString();
                        vm.CustomGuaranteeSealMark = dr["CustomGuaranteeSealMark"].ToString();
                        vm.AdditionalInfo = dr["AdditionalInfo"].ToString();
                        vm.BankId = dr["BankId"].ToString();
                        vm.ModeOfPayment = dr["ModeOfPayment"].ToString();
                        vm.InstrumentNo = dr["InstrumentNo"].ToString();
                        vm.InstrumentDate = OrdinaryVATDesktop.DateTimeToDate(dr["InstrumentDate"].ToString());

                        vm.CRNo = dr["CRNo"].ToString();
                        vm.CRDate = OrdinaryVATDesktop.DateTimeToDate(dr["CRDate"].ToString());

                        vm.SupplierConformation = dr["SupplierConformation"].ToString();

                        vm.Amount = Convert.ToDecimal(dr["Amount"].ToString());
                        vm.PricePerMTon = Convert.ToDecimal(dr["PricePerMTon"].ToString());
                        vm.CostOfOil = Convert.ToDecimal(dr["CostOfOil"].ToString());
                        vm.BargeHireCharge = Convert.ToDecimal(dr["BargeHireCharge"].ToString());
                        vm.StatutoryCharge = Convert.ToDecimal(dr["StatutoryCharge"].ToString());
                        vm.CurrentRateFromUSD = Convert.ToDecimal(dr["CurrentRateFromUSD"].ToString());
                        vm.TotalCostBDT = Convert.ToDecimal(dr["TotalCostBDT"].ToString());

                        vm.ReceivedVessel = dr["ReceivedVessel"].ToString();
                        vm.ReceivedOwner = dr["ReceivedOwner"].ToString();
                        vm.SupplierOrBargeCaptainCert = dr["SupplierOrBargeCaptainCert"].ToString();
                        vm.Comments = dr["Comments"].ToString();
                        vm.Post = dr["Post"].ToString();
                        vm.Status = dr["Post"].ToString() == "N" ? "Not Posted" : "Posted";
                        vm.CreatedBy = dr["CreatedBy"].ToString();
                        vm.CreatedOn = dr["CreatedOn"].ToString();
                        vm.LastModifiedBy = dr["LastModifiedBy"].ToString();
                        vm.LastModifiedOn = dr["LastModifiedOn"].ToString();

                        vm.CustomerName = dr["CustomerName"].ToString();
                        vm.DeliveredByName = dr["DeliveredByName"].ToString();
                        vm.DeliveredForName = dr["DeliveredForName"].ToString();
                        vm.BranchName = dr["BranchName"].ToString();
                        vm.TankCode = dr["TankCode"].ToString();
                        vm.BankName = dr["BankName"].ToString();

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

                FileLogger.Log("MPLSalesInvSA4DAL", "SelectAllList", sqlex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {

                FileLogger.Log("MPLSalesInvSA4DAL", "SelectAllList", ex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", ex.Message.ToString());

            }
            #endregion
            #region finally
            #endregion
            return VMs;
        }

        public List<MPLSaleInvoiceSA4VM> DropDown(int branchId, string ItemNo, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<MPLSaleInvoiceSA4VM> VMs = new List<MPLSaleInvoiceSA4VM>();
            MPLSaleInvoiceSA4VM vm;
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
                sqlText = @" SELECT Id,SalesInvoiceNo FROM MPLSaleInvoiceSA4 WHERE  1=1 AND BranchId = @BranchId";

                SqlCommand objComm = new SqlCommand(sqlText, currConn);
                objComm.Parameters.AddWithValueAndNullHandle("@BranchId", branchId);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new MPLSaleInvoiceSA4VM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.SalesInvoiceNo = dr["SalesInvoiceNo"].ToString();
                    VMs.Add(vm);
                }
                dr.Close();
                #endregion
            }
            #endregion

            #region catch
            catch (SqlException sqlex)
            {
                FileLogger.Log("MPLSalesInvSA4DAL", "DropDown", sqlex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                FileLogger.Log("MPLSalesInvSA4DAL", "DropDown", ex.ToString() + "\n" + sqlText);

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

        public string[] Delete(MPLSaleInvoiceSA4VM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            throw new NotImplementedException();
        }

        public string[] SaleInvoiceSA4Post(MPLSaleInvoiceSA4VM MasterVM, SqlTransaction transaction, SqlConnection currConn, SysDBInfoVMTemp connVM = null)
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
                 @" UPDATE MPLSaleInvoiceSA4 SET Post='Y',LastModifiedBy=@LastModifiedBy,LastModifiedOn=GETDATE() ";
                sqlText += @"  WHERE Id IN (" + ids + ") ";

                SqlCommand upd = new SqlCommand(sqlText, currConn, transaction);
                upd.Parameters.AddWithValueAndNullHandle("@LastModifiedBy", MasterVM.LastModifiedBy);

                transResult = upd.ExecuteNonQuery();

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
                FileLogger.Log("MPLSalesInvSA4DAL", "SaleInvoiceSA4Post", ex.ToString() + "\n" + sqlText);

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
